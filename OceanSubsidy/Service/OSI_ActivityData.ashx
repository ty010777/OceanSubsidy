<%@ WebHandler Language="C#" Class="OSI_ActivityData" %>

using System;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OSI_ActivityData : IHttpHandler
{
    DbHelper db;
    public void ProcessRequest(HttpContext context)
    {
                context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        db = new DbHelper();

        // 1. 讀參數
        string yearFrom = context.Request["yearFrom"];
        string quarterFrom = context.Request["quarterFrom"];
        string yearTo = context.Request["yearTo"];
        string quarterTo = context.Request["quarterTo"];
        string unitParam = context.Request["unit"];    // 例如 "3,5,7"
        string actName = context.Request["actName"]; // 關鍵字
        string reportIds = context.Request["reportIds"]; // 報告 ID 篩選 (用於縣市模式)

        // 2. 先查得起訖日期 (用 QuarterStartDate/QuarterEndDate 判斷哪些 PeriodID 在範圍內)
        DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;

        // 起始
        db.CommandText =
@"SELECT TOP 1 QuarterStartDate 
FROM dbo.OSI_DataPeriods
WHERE PeriodYear = @yFrom AND PeriodQuarter = @qFrom";

        db.Parameters.Clear();
        db.Parameters.Add("@yFrom", yearFrom ?? "");
        db.Parameters.Add("@qFrom", quarterFrom ?? "");
        var tbl = db.GetTable();
        if (tbl.Rows.Count > 0)
        {
            var row = tbl.Rows[0];
            if (row["QuarterStartDate"] != null && row["QuarterStartDate"] != DBNull.Value)
            {
                dtStart = (DateTime)row["QuarterStartDate"];
            }
        }


        // 結束
        db.CommandText =
@"SELECT TOP 1 QuarterEndDate 
FROM dbo.OSI_DataPeriods
WHERE PeriodYear = @yTo   AND PeriodQuarter = @qTo";

        db.Parameters.Clear();
        db.Parameters.Add("@yTo", yearTo ?? "");
        db.Parameters.Add("@qTo", quarterTo ?? "");
        tbl = db.GetTable();
        if (tbl.Rows.Count > 0)
        {
            var row = tbl.Rows[0];
            if (row["QuarterEndDate"] != null && row["QuarterEndDate"] != DBNull.Value)
            {
                dtEnd = (DateTime)row["QuarterEndDate"];
            }
        }

        // 3. 組主查詢 SQL
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(
@"SELECT 
    r.ReportID,
    u.UnitName    AS ReportingUnit,
    r.ActivityName,
    g.GeoData.STAsText() AS WKT,
    dp.Color
FROM dbo.OSI_ActivityReports r
JOIN dbo.OSI_DataPeriods dp ON r.PeriodID = dp.PeriodID
JOIN dbo.OSI_Geom g ON r.ReportID = g.ReportID
LEFT JOIN dbo.Sys_Unit       u  ON r.ReportingUnitID = u.UnitID
WHERE dp.QuarterStartDate >= @dtStart
AND dp.QuarterEndDate   <= @dtEnd
AND r.IsValid = 1
AND g.IsValid = 1
AND g.DeletedAt IS NULL"
        );

        // 準備參數集合
        db.CommandText = sb.ToString();
        db.Parameters.Clear();
        db.Parameters.Add("@dtStart", dtStart);
        db.Parameters.Add("@dtEnd", dtEnd);

        // 單位篩選
        if (!string.IsNullOrWhiteSpace(unitParam))
        {
            var rawUnits = unitParam.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim())
                           .ToArray();
            if (!rawUnits.Any(u => u.Equals("all", StringComparison.OrdinalIgnoreCase)))
            {
                // 只挑能 parse 的整數，其他跳過
                var unitIds = rawUnits
                    .Select(u => { int id; return int.TryParse(u, out id) ? (int?)id : null; })
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Distinct()
                    .ToList();

                if (unitIds.Count > 0)
                {
                    // 動態產生 @unit0,@unit1,...
                    sb.Append(" AND r.ReportingUnitID IN (");
                    for (int i = 0; i < unitIds.Count; i++)
                    {
                        string paramName = "@unit" + i;
                        sb.Append(paramName);
                        if (i < unitIds.Count - 1) sb.Append(",");
                        // 加參數 - 修正：移到正確位置
                        db.Parameters.Add(paramName, unitIds[i]);
                    }
                    sb.AppendLine(")");
                }
            }
        }
        
        // 活動名稱 LIKE
        bool useKeyword = !string.IsNullOrWhiteSpace(actName);
        if (useKeyword)
        {
            sb.AppendLine(" AND r.ActivityName LIKE @kw");
            db.Parameters.Add("@kw", "%" + actName + "%");
        }

        // 報告 ID 篩選（用於縣市模式）
        if (!string.IsNullOrWhiteSpace(reportIds))
        {
            var rawReportIds = reportIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .ToArray();
            
            var reportIdInts = rawReportIds
                .Select(id => { int result; return int.TryParse(id, out result) ? (int?)result : null; })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            if (reportIdInts.Count > 0)
            {
                sb.Append(" AND r.ReportID IN (");
                for (int i = 0; i < reportIdInts.Count; i++)
                {
                    string paramName = "@reportId" + i;
                    sb.Append(paramName);
                    if (i < reportIdInts.Count - 1) sb.Append(",");
                    db.Parameters.Add(paramName, reportIdInts[i]);
                }
                sb.AppendLine(")");
            }
        }

        // 更新 SQL 命令
        db.CommandText = sb.ToString();
        
        DataTable resultTbl = db.GetTable();

        // 組成回傳物件
        var list = new List<response>();
        foreach (DataRow row in resultTbl.Rows)
        {
            list.Add(new response
            {
                reportID = row["ReportID"].ToString(),
                reportingUnit = row["ReportingUnit"] == DBNull.Value
                                ? null
                                : row["ReportingUnit"].ToString(),
                activityName = row["ActivityName"].ToString(),
                wkt = row["WKT"].ToString(),
                color = row["Color"] == DBNull.Value
                        ? "#3388ff"  // 預設顏色
                        : row["Color"].ToString()
            });
        }

        // 回傳 JSON
        context.Response.Write(JsonConvert.SerializeObject(list));
    }

    private class response
    {
        public string reportID { get; set; }
        public string reportingUnit { get; set; }
        public string activityName { get; set; }
        public string wkt { get; set; }
        public string color { get; set; }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
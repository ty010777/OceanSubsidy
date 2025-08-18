<%@ WebHandler Language="C#" Class="OSI_CountyData" %>

using System;
using System.Web;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using GS.Data;
using GS.Data.Sql;

public class OSI_CountyData : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        string data = context.Request["data"];

        try
        {
            switch (data)
            {
                case "countyActivities":
                    GetCountyActivities(context);
                    break;
                default:
                    context.Response.Write(JsonConvert.SerializeObject(new { error = "Invalid data parameter" }));
                    break;
            }
        }
        catch (Exception ex)
        {
            context.Response.Write(JsonConvert.SerializeObject(new { error = ex.Message }));
        }
    }

    private void GetCountyActivities(HttpContext context)
    {
        string yearFrom = context.Request["yearFrom"];
        string quarterFrom = context.Request["quarterFrom"];
        string yearTo = context.Request["yearTo"];
        string quarterTo = context.Request["quarterTo"];
        string units = context.Request["unit"];
        string actName = context.Request["actName"];

        var result = new List<object>();

        // 建立查詢活動的 SQL
        var db = new GS.Data.Sql.DbHelper();
        db.CommandText = @"
WITH FilteredActivities AS (
    SELECT 
        r.ReportID,
        r.ActivityName,
        g.GeoData,
        g.GeoData.STAsText() as GeoDataWKT,
        u.UnitName as ReportingUnit,
        dp.PeriodYear,
        dp.PeriodQuarter
    FROM OSI_ActivityReports r
    JOIN OSI_DataPeriods dp ON r.PeriodID = dp.PeriodID
    JOIN OSI_Geom g ON r.ReportID = g.ReportID
    LEFT JOIN Sys_Unit u ON r.ReportingUnitID = u.UnitID
    WHERE r.IsValid = 1
        AND g.IsValid = 1
        AND g.DeletedAt IS NULL
        AND g.GeoData IS NOT NULL
";

        // 加入查詢條件
        var conditions = new List<string>();

        if (!string.IsNullOrEmpty(yearFrom))
        {
            conditions.Add("dp.PeriodYear >= @yearFrom");
            db.Parameters.Add("@yearFrom", yearFrom);
        }

        if (!string.IsNullOrEmpty(yearTo))
        {
            conditions.Add("dp.PeriodYear <= @yearTo");
            db.Parameters.Add("@yearTo", yearTo);
        }

        if (!string.IsNullOrEmpty(quarterFrom) && !string.IsNullOrEmpty(yearFrom))
        {
            conditions.Add("(dp.PeriodYear > @yearFrom OR (dp.PeriodYear = @yearFrom AND dp.PeriodQuarter >= @quarterFrom))");
            db.Parameters.Add("@quarterFrom", quarterFrom);
        }

        if (!string.IsNullOrEmpty(quarterTo) && !string.IsNullOrEmpty(yearTo))
        {
            conditions.Add("(dp.PeriodYear < @yearTo OR (dp.PeriodYear = @yearTo AND dp.PeriodQuarter <= @quarterTo))");
            db.Parameters.Add("@quarterTo", quarterTo);
        }

        if (!string.IsNullOrEmpty(units) && units != "all")
        {
            var unitList = units.Split(',');
            var unitParams = new List<string>();
            for (int i = 0; i < unitList.Length; i++)
            {
                var paramName = "@unit" + i;
                unitParams.Add(paramName);
                db.Parameters.Add(paramName, unitList[i]);
            }
            conditions.Add("r.ReportingUnitID IN (" + string.Join(",", unitParams) + ")");
        }

        if (!string.IsNullOrEmpty(actName))
        {
            conditions.Add("r.ActivityName LIKE '%' + @actName + '%'");
            db.Parameters.Add("@actName", actName);
        }

        if (conditions.Count > 0)
        {
            db.CommandText += " AND " + string.Join(" AND ", conditions);
        }

        // OSI_ActivityReports.GeoData 和 OSI_MapCounty.geom 都是 SRID 3826 (TWD97 / TM2 zone 121)
        // 使用 MakeValid 處理無效的幾何資料，避免空間比對錯誤
        db.CommandText += @"
), ValidGeometry AS (
    SELECT 
        ReportID,
        ActivityName,
        CASE 
            WHEN GeoData.STIsValid() = 1 THEN GeoData
            ELSE GeoData.MakeValid()
        END as ValidGeoData,
        GeoDataWKT,
        ReportingUnit,
        PeriodYear,
        PeriodQuarter
    FROM FilteredActivities
    WHERE GeoData IS NOT NULL
)
SELECT 
    c.county_id,
    c.c_name,
    CASE 
        WHEN c.geom.STIsValid() = 1 THEN c.geom.STAsText()
        ELSE c.geom.MakeValid().STAsText()
    END as county_geom,
    COUNT(DISTINCT vg.ReportID) as activity_count,
    STRING_AGG(CAST(vg.ReportID as VARCHAR), ',') WITHIN GROUP (ORDER BY vg.ReportID) as report_ids
FROM OSI_MapCounty c
LEFT JOIN ValidGeometry vg
    ON (CASE 
            WHEN c.geom.STIsValid() = 1 THEN c.geom
            ELSE c.geom.MakeValid()
        END).STIntersects(vg.ValidGeoData) = 1
WHERE c.geom IS NOT NULL
GROUP BY c.county_id, c.c_name, 
    CASE 
        WHEN c.geom.STIsValid() = 1 THEN c.geom.STAsText()
        ELSE c.geom.MakeValid().STAsText()
    END
ORDER BY c.county_id
";

        DataTable countyActivities = db.GetTable();

        // 處理結果
        foreach (DataRow row in countyActivities.Rows)
        {
            var countyResult = new
            {
                county_id = row["county_id"],
                c_name = row["c_name"].ToString(),
                geom = row["county_geom"].ToString(),
                activity_count = Convert.ToInt32(row["activity_count"]),
                report_ids = row["report_ids"]?.ToString() ?? "",
                color = GetColorByCount(Convert.ToInt32(row["activity_count"]))
            };
            result.Add(countyResult);
        }

        context.Response.Write(JsonConvert.SerializeObject(result));
    }

    private string GetColorByCount(int count)
    {
        if (count == 0) return "transparent";
        if (count <= 4) return "#28a745"; // 綠色
        if (count <= 9) return "#ffc107"; // 橘色
        return "#dc3545"; // 紅色
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
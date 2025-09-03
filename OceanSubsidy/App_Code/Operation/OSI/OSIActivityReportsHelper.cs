using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.App;

/// <summary>
/// OSIActivityReportsHelper 的摘要描述
/// </summary>
public class OSIActivityReportsHelper
{
    public OSIActivityReportsHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 查詢所有
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ReportID]
    ,[PeriodID]
    ,[ReportingUnitID]
    ,[ActivityName]
    ,[NatureID]
    ,[NatureText]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityReports]
WHERE IsValid = 1
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ReportID]
    ,[PeriodID]
    ,[ReportingUnitID]
    ,[ActivityName]
    ,[NatureID]
    ,[NatureText]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityReports]
WHERE IsValid = 1
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ID
    /// </summary>
    /// <returns></returns>
    public static OSI_ActivityReports QueryByIDWithClass(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ReportID]
    ,[PeriodID]
    ,[ReportingUnitID]
    ,[ActivityName]
    ,[NatureID]
    ,[NatureText]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityReports]
WHERE IsValid = 1
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetList<OSI_ActivityReports>().FirstOrDefault();
    }

    /// <summary>
    /// 查詢報表 By PeriodID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByPeriodID(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ReportID]
    ,[PeriodID]
    ,[ReportingUnitID]
    ,[ActivityName]
    ,[NatureID]
    ,[NatureText]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityReports]
WHERE IsValid = 1
AND PeriodID = @PeriodID
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢報表 By PeriodID 和 ReportingUnitID
    /// </summary>
    /// <returns></returns>
    public static GisTable QuerySimpleReports(string periodID, string unitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ReportID]
    ,[PeriodID]
    ,[ReportingUnitID]
    ,[ActivityName]
    ,[NatureID]
    ,[NatureText]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityReports]
WHERE IsValid = 1
AND PeriodID = @PeriodID
AND ReportingUnitID = @ReportingUnitID
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        db.Parameters.Add("@ReportingUnitID", unitID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢簡易報表列表（摘要），依 PeriodID 與 ReportingUnitID 過濾
    /// </summary>
    public static GisTable QueryActReport(
        string periodID,
        string unitID = "-99",
        string natureID = "-99",
        string carrierTypeID = "-99",
        string itemID = "-99",
        string key = "")
    {
        var db = new DbHelper();
        db.CommandText = @"
WITH ExecCTE AS (
  SELECT
    ae.ReportID,
    Executors = STUFF(
      (
        SELECT '、' + ae2.ExecutorName
        FROM OSI_ActivityExecutors ae2
        WHERE ae2.ReportID = ae.ReportID
        AND ae2.IsValid = 1
        FOR XML PATH(''), TYPE
      ).value('.', 'NVARCHAR(MAX)')
    , 1, 1, '')
  FROM OSI_ActivityExecutors ae
  GROUP BY ae.ReportID
)

SELECT 
  r.ReportID,
  u.UnitName      AS ReportingUnit,
  r.ActivityName,
  a.NatureName,
  e.Executors,
  d.StartDate,
  d.EndDate,
  r.LastUpdated,
  PeriodYear
  + CASE PeriodQuarter
        WHEN N'第一季' THEN 'Q1'
        WHEN N'第二季' THEN 'Q2'
        WHEN N'第三季' THEN 'Q3'
        WHEN N'第四季' THEN 'Q4'
        ELSE PeriodQuarter
    END AS Period
FROM OSI_ActivityReports r
LEFT JOIN Sys_Unit u 
  ON r.ReportingUnitID = u.UnitID
LEFT JOIN ExecCTE e
  ON e.ReportID = r.ReportID
LEFT JOIN OSI_ActivityNatures a
  ON a.NatureID = r.NatureID
LEFT JOIN OSI_DataPeriods dp
  ON dp.PeriodID = r.PeriodID

CROSS APPLY (
  SELECT 
    MIN(rp.StartDate) AS StartDate,
    MAX(rp.EndDate)   AS EndDate
  FROM OSI_ResearchPeriods rp
  WHERE rp.ReportID = r.ReportID
) AS d

WHERE (@PeriodID = '-99' OR r.PeriodID = @PeriodID)
  AND r.IsValid   = 1
  AND (
    @UnitID = '-99'
    OR r.ReportingUnitID = @UnitID
    OR (@UnitID = '-1' AND r.ReportingUnitID IS NULL)
  )
  AND (
    @NatureID = '-99'
    OR r.NatureID = @NatureID
  )
  AND (
    @CarrierTypeID = '-99'
    OR EXISTS (
      SELECT 1 FROM OSI_Carrier c 
      WHERE c.ReportID = r.ReportID 
        AND c.CarrierTypeID = @CarrierTypeID 
        AND c.IsValid = 1
    )
  )
  AND (
    @ResearchItemID = '-99'
    OR r.ResearchItemID = @ResearchItemID
  )
  AND (
    @Key = ''
    OR r.ActivityName LIKE '%' + @Key + '%'
    OR r.NatureText   LIKE '%' + @Key + '%'
    OR e.Executors    LIKE '%' + @Key + '%'
    OR EXISTS (
      SELECT 1 FROM OSI_Carrier c 
      WHERE c.ReportID = r.ReportID 
        AND c.IsValid = 1
        AND (c.CarrierNo LIKE '%' + @Key + '%' OR c.CarrierDetail LIKE '%' + @Key + '%')
    )
    OR r.ResearchItemNote LIKE '%' + @Key + '%'
    OR r.Instruments      LIKE '%' + @Key + '%'
    OR r.ActivityOverview LIKE '%' + @Key + '%'
  )
ORDER BY u.UnitName, r.ActivityName DESC;
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID ?? "");
        db.Parameters.Add("@UnitID", unitID ?? "-99");
        db.Parameters.Add("@NatureID", natureID ?? "-99");
        db.Parameters.Add("@CarrierTypeID", carrierTypeID ?? "-99");
        db.Parameters.Add("@ResearchItemID", itemID ?? "-99");
        db.Parameters.Add("@Key", key ?? "");

        var tbl = db.GetTable();
        tbl.Columns.Add("StartDateDisplay", typeof(string));
        tbl.Columns.Add("EndDateDisplay", typeof(string));
        tbl.Columns.Add("LastUpdatedDisplay", typeof(string));
        foreach (DataRow row in tbl.Rows)
        {
            // Executors過長的話，截斷顯示
            var exec = row["Executors"]?.ToString() ?? "";
            if (exec.Length > 20)
                row["Executors"] = exec.Substring(0, 20) + "…";

            // 將日期欄位轉換為民國年格式
            if (row["StartDate"] != DBNull.Value)
            {
                DateTime startDate = Convert.ToDateTime(row["StartDate"]);
                row["StartDateDisplay"] = DateTimeHelper.ToMinguoDate(startDate);
            }

            if (row["EndDate"] != DBNull.Value)
            {
                DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                row["EndDateDisplay"] = DateTimeHelper.ToMinguoDate(endDate);
            }

            if (row["LastUpdated"] != DBNull.Value)
            {
                DateTime lastUpdated = Convert.ToDateTime(row["LastUpdated"]);
                row["LastUpdatedDisplay"] = DateTimeHelper.ToMinguoDateTime(lastUpdated);
            }
        }

        return tbl;
    }

    /// <summary>
    /// 查詢Excelc匯出資料
    /// </summary>
    public static GisTable QueryExcelActReport(int[] reportIDs)
    {
        var db = new DbHelper();
        var sb = new StringBuilder(@"
WITH ExecCTE AS (
  SELECT
    ae.ReportID,
    Executors = STUFF(
      (
        SELECT '、' + ec.CategoryName + ':' + ae2.ExecutorName
        FROM OSI_ActivityExecutors ae2
        JOIN OSI_ExecutorCategories ec
          ON ae2.CategoryID = ec.CategoryID
        WHERE ae2.ReportID = ae.ReportID
          AND ae2.IsValid = 1
        FOR XML PATH(''), TYPE
      ).value('.', 'NVARCHAR(MAX)')
    ,1,1,'')
  FROM OSI_ActivityExecutors ae
  GROUP BY ae.ReportID
),
PeriodCTE AS (
  SELECT
    rp.ReportID,
    ResearchDates = STUFF((
      SELECT 
        N'、' 
        + REPLACE(CONVERT(CHAR(10), rp2.StartDate, 23), '-', '.') 
        + ' - ' 
        + REPLACE(CONVERT(CHAR(10), rp2.EndDate,   23), '-', '.') 
        + CASE 
            WHEN ISNULL(rp2.PeriodLabel, '') <> '' 
              THEN ' ' + rp2.PeriodLabel 
            ELSE '' 
          END
      FROM dbo.OSI_ResearchPeriods rp2
      WHERE rp2.ReportID = rp.ReportID
        AND rp2.IsValid   = 1
      FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'),
    1, 1, N'')
  FROM dbo.OSI_ResearchPeriods rp
  WHERE rp.IsValid = 1
  GROUP BY rp.ReportID
),
ScopeCTE AS (
  SELECT
    ss.ReportID,
    SurveyScopes = STUFF(
      (
        SELECT '、' + ss2.SurveyScope
        FROM OSI_SurveyScopes ss2
        WHERE ss2.ReportID = ss.ReportID
          AND ss2.IsValid = 1
        FOR XML PATH(''), TYPE
      ).value('.', 'NVARCHAR(MAX)')
    ,1,1,'')
  FROM OSI_SurveyScopes ss
  WHERE ss.IsValid = 1
  GROUP BY ss.ReportID
),
CarrierCTE AS (
  SELECT
    c.ReportID,
    Carriers = STUFF((
      SELECT 
        N'、' + ISNULL(ct.CarrierTypeName, '') + ':' + ISNULL(c2.CarrierDetail, '') + 
        CASE 
          WHEN ISNULL(c2.CarrierNo, '') <> '' 
            THEN '(' + c2.CarrierNo + ')' 
          ELSE '' 
        END
      FROM OSI_Carrier c2
      LEFT JOIN OSI_CarrierTypes ct ON c2.CarrierTypeID = ct.CarrierTypeID
      WHERE c2.ReportID = c.ReportID
        AND c2.IsValid = 1
      FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'),
    1, 1, N'')
  FROM OSI_Carrier c
  WHERE c.IsValid = 1
  GROUP BY c.ReportID
)
SELECT
  Period    = dp.PeriodYear
            + CASE dp.PeriodQuarter
                WHEN N'第一季' THEN 'Q1'
                WHEN N'第二季' THEN 'Q2'
                WHEN N'第三季' THEN 'Q3'
                WHEN N'第四季' THEN 'Q4'
                ELSE dp.PeriodQuarter
              END,
  ReportingUnit      = ISNULL(u.UnitName, ''),
  ActivityName       = ISNULL(r.ActivityName, ''),
  NatureName         = ISNULL(an.NatureName, ''),
  NatureText         = ISNULL(r.NatureText, ''),
  Executors          = ISNULL(e.Executors, ''),
  ResearchDates      = ISNULL(p.ResearchDates, ''),
  Carriers           = ISNULL(car.Carriers, ''),
  ItemName           = ISNULL(ri.ItemName, ''),
  ResearchItemNote   = ISNULL(r.ResearchItemNote, ''),
  Instruments        = ISNULL(r.Instruments, ''),
  ActivityOverview   = ISNULL(r.ActivityOverview, ''),
  SurveyScopes       = ISNULL(sc.SurveyScopes, ''),
  ContactName        = ISNULL(su.Name, ''),
  ContactTel         = ISNULL(su.Tel, ''),
  ContactEmail       = ISNULL(su.Account, '')
FROM OSI_ActivityReports r
JOIN OSI_DataPeriods dp
  ON r.PeriodID = dp.PeriodID
LEFT JOIN Sys_Unit u
  ON r.ReportingUnitID = u.UnitID
JOIN OSI_ActivityNatures an
  ON r.NatureID = an.NatureID
LEFT JOIN OSI_ResearchItems ri
  ON r.ResearchItemID = ri.ItemID
LEFT JOIN Sys_User su
  ON r.LastUpdatedBy = su.UserID
LEFT JOIN ExecCTE e
  ON e.ReportID = r.ReportID
LEFT JOIN PeriodCTE p
  ON p.ReportID = r.ReportID
LEFT JOIN ScopeCTE sc
  ON sc.ReportID = r.ReportID
LEFT JOIN CarrierCTE car
  ON car.ReportID = r.ReportID
WHERE r.IsValid = 1
");
        if (reportIDs != null && reportIDs.Length > 0)
        {
            var names = reportIDs
                .Select((_, idx) => "@id" + idx)
                .ToArray();

            // 在 SQL 接上： AND r.ReportID IN (@id0, @id1, @id2, …)
            sb.Append("\n  AND r.ReportID IN (" + string.Join(", ", names) + ")\n");

            // 把每個整數值加到 db.Parameters
            for (int i = 0; i < reportIDs.Length; i++)
            {
                db.Parameters.Add(names[i], reportIDs[i]);
            }
        }
        sb.Append("ORDER BY u.UnitName, r.ActivityName DESC;");

        // 最後把拼好的 SQL 下到 db
        db.CommandText = sb.ToString();
        var tbl = db.GetTable();

        // 將研究調查日期欄位轉換為民國年格式
        foreach (DataRow row in tbl.Rows)
        {
            var researchDates = row["ResearchDates"]?.ToString();
            if (!string.IsNullOrEmpty(researchDates))
            {
                // 處理多個日期範圍，以「、」分隔
                var dateRanges = researchDates.Split('、');
                var convertedRanges = new List<string>();

                foreach (var range in dateRanges)
                {
                    // 處理每個日期範圍 (YYYY.MM.DD - YYYY.MM.DD [標籤])
                    var parts = range.Split(new[] { " - " }, StringSplitOptions.None);
                    if (parts.Length >= 2)
                    {
                        try
                        {
                            // 解析開始日期
                            var startDateStr = parts[0].Trim();
                            var startDate = DateTime.ParseExact(startDateStr, "yyyy.MM.dd", null);
                            var minguoStart = DateTimeHelper.ToMinguoDate(startDate);

                            // 解析結束日期和標籤
                            var endPart = parts[1].Trim();
                            var labelIndex = endPart.IndexOf(' ');
                            var endDateStr = labelIndex > 0 ? endPart.Substring(0, labelIndex) : endPart;
                            var label = labelIndex > 0 ? endPart.Substring(labelIndex) : "";

                            var endDate = DateTime.ParseExact(endDateStr, "yyyy.MM.dd", null);
                            var minguoEnd = DateTimeHelper.ToMinguoDate(endDate);

                            // 重組日期範圍
                            convertedRanges.Add($"{minguoStart} - {minguoEnd}{label}");
                        }
                        catch
                        {
                            // 如果轉換失敗，保留原始格式
                            convertedRanges.Add(range);
                        }
                    }
                    else
                    {
                        convertedRanges.Add(range);
                    }
                }

                row["ResearchDates"] = string.Join("、", convertedRanges);
            }
        }

        return tbl;
    }


    /// <summary>
    /// 查詢幾筆資料
    /// </summary>
    public static int CountByPeriodAndUnit(string periodID, string unitID)
    {
        var db = new DbHelper();
        db.CommandText = @"
SELECT COUNT(*)
FROM OSI_ActivityReports r
WHERE r.PeriodID        = @PeriodID
AND r.ReportingUnitID = @UnitID
AND r.IsValid         = 1;";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID ?? "");
        db.Parameters.Add("@UnitID", unitID ?? "");

        var tbl = db.GetTable();
        if (tbl.Rows.Count == 0)
            return 0;
        else
            return Convert.ToInt32(tbl.Rows[0][0]);
    }

    /// <summary>
    /// 查詢幾筆資料 By PeriodID且不包含複製資料
    /// </summary>
    public static int CountByPeriodIDWithOutCopy(string periodID)
    {
        var db = new DbHelper();
        db.CommandText = @"
SELECT COUNT(*)
FROM OSI_ActivityReports
WHERE PeriodID = @PeriodID
AND IsValid = 1
AND CopyReportID IS NULL;";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID ?? "");

        var tbl = db.GetTable();
        if (tbl.Rows.Count == 0)
            return 0;
        else
            return Convert.ToInt32(tbl.Rows[0][0]);
    }

    /// <summary>
    /// 查詢研究調查項目圖表
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryResearchDashboard(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
    i.ItemName AS Name, 
    COUNT(*) AS Num
FROM OSI_ResearchItems i
JOIN OSI_ActivityReports r ON r.ResearchItemID = i.ItemID
WHERE r.IsValid = 1
AND r.PeriodID = @PeriodID
GROUP BY i.ItemName
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢填報單位圖表
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUnitPercentDashboard(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
	u.UnitName AS Name, 
	COUNT(*) AS Num
FROM Sys_Unit u
JOIN OSI_ActivityReports r ON r.ReportingUnitID = u.UnitID
WHERE r.IsValid = 1
AND r.PeriodID = @PeriodID
GROUP BY u.UnitName
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢活動性質圖表
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryNatureDashboard(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
	n.NatureName AS Name, 
	COUNT(*) AS Num
FROM OSI_ActivityNatures n
JOIN OSI_ActivityReports r ON r.NatureID = n.NatureID
WHERE r.IsValid = 1
AND r.PeriodID = @PeriodID
GROUP BY n.NatureName
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢載具類別圖表
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryCarrierTypeDashboard(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
	c.CarrierTypeName AS Name, 
	COUNT(*) AS Num
FROM OSI_CarrierTypes c
JOIN OSI_Carrier carr ON c.CarrierTypeID = carr.CarrierTypeID
JOIN OSI_ActivityReports r ON carr.ReportID = r.ReportID
WHERE r.IsValid = 1
AND carr.IsValid = 1
AND r.PeriodID = @PeriodID
GROUP BY c.CarrierTypeName
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢地圖資訊 BY ID
    /// </summary>
    /// <returns></returns>
    public static string QueryGeoDataByID(string reportID)
    {
        string rtVal = "";

        // 優先從 OSI_Geom 表取得資料
        var geomsTable = OSIGeomHelper.QueryByReportID(reportID);
        if (geomsTable != null && geomsTable.Rows.Count > 0)
        {
            if (geomsTable.Rows.Count == 1)
            {
                // 單一圖徵
                rtVal = geomsTable.Rows[0]["GeoData"].ToString();
            }
            else
            {
                // 多個圖徵，組合成 GeometryCollection
                var geoms = new List<string>();
                foreach (DataRow row in geomsTable.Rows)
                {
                    if (row["GeoData"] != null && row["GeoData"] != DBNull.Value)
                    {
                        geoms.Add(row["GeoData"].ToString());
                    }
                }
                if (geoms.Count > 0)
                {
                    rtVal = "GEOMETRYCOLLECTION(" + string.Join(",", geoms) + ")";
                }
            }
        }

        return rtVal;
    }

    /// <summary>
    /// 新增一筆活動資料及其子表資料
    /// </summary>
    public static int InsertReport(
        OSI_ActivityReports report,
        List<OSI_ActivityExecutors> executors,
        List<OSI_ResearchPeriods> resPeriods,
        List<OSI_ActivityFiles> files,
        List<OSI_SurveyScopes> surveyScopes,
        List<OSI_Carrier> carriers,
        List<OSI_SurveyCounties> surveyCounties,
        List<int> delExecutors,
        List<int> delResPeriods,
        List<int> delFiles,
        List<int> delSurveyScopes,
        List<int> delCarriers,
        List<int> delSurveyCounties,
        string baseDir)
    {
        int reportID = 0;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityReports]([PeriodID],[ReportingUnitID],[ActivityName],[NatureID],[NatureText],[ResearchItemID],[ResearchItemNote],[Instruments],[ActivityOverview],[LastUpdated],[LastUpdatedBy],[IsValid])
VALUES(@PeriodID,@ReportingUnitID,@ActivityName,@NatureID,@NatureText,@ResearchItemID,@ResearchItemNote,@Instruments,@ActivityOverview,@LastUpdated,@LastUpdatedBy,@IsValid)

 SELECT SCOPE_IDENTITY();
";

            db.Parameters.Clear();
            db.Parameters.Add("@PeriodID", report.PeriodID);
            db.Parameters.Add("@ReportingUnitID", report.ReportingUnitID);
            db.Parameters.Add("@ActivityName", report.ActivityName);
            db.Parameters.Add("@NatureID", report.NatureID);
            db.Parameters.Add("@NatureText", report.NatureText);
            db.Parameters.Add("@ResearchItemID", report.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", report.ResearchItemNote);
            db.Parameters.Add("@Instruments", report.Instruments);
            db.Parameters.Add("@ActivityOverview", report.ActivityOverview);
            db.Parameters.Add("@LastUpdated", report.LastUpdated);
            db.Parameters.Add("@LastUpdatedBy", report.LastUpdatedBy);
            db.Parameters.Add("@IsValid", report.IsValid);

            object result = db.GetDataSet().Tables[0].Rows[0][0];
            if (result == null || result == DBNull.Value)
                throw new Exception("取得 ReportID 失敗：結果為 null 或 DBNull");
            reportID = Convert.ToInt32(result);

            // 新增活動執行者
            foreach (var executor in executors)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityExecutors]([ReportID],[CategoryID],[ExecutorName],[IsValid])
VALUES(@ReportID,@CategoryID,@ExecutorName,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@CategoryID", executor.CategoryID);
                db.Parameters.Add("@ExecutorName", executor.ExecutorName);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除活動執行者
            if (delExecutors.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_ActivityExecutors]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE ExecutorID IN (" + string.Join(",", delExecutors) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增研究調查日期
            foreach (var resPeriod in resPeriods)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_ResearchPeriods]([ReportID],[StartDate],[EndDate],[PeriodLabel],[IsValid])
VALUES(@ReportID,@StartDate,@EndDate,@PeriodLabel,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@StartDate", resPeriod.StartDate);
                db.Parameters.Add("@EndDate", resPeriod.EndDate);
                db.Parameters.Add("@PeriodLabel", resPeriod.PeriodLabel);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除研究調查日期
            if (delResPeriods.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_ResearchPeriods]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE PeriodID IN (" + string.Join(",", delResPeriods) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增活動檔案
            var realDir = Path.Combine(baseDir, "Report_" + reportID);
            foreach (var file in files)
            {
                file.FilePath = Path.Combine(realDir, file.FileName);

                db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityFiles]([ReportID],[FileName],[FilePath],[IsValid])
VALUES(@ReportID,@FileName,@FilePath,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@FileName", file.FileName);
                db.Parameters.Add("@FilePath", file.FilePath);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }
            // 刪除活動檔案
            if (delFiles.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_ActivityFiles]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE AttachmentID IN (" + string.Join(",", delFiles) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增研究調查範圍
            foreach (var scope in surveyScopes)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_SurveyScopes]([ReportID],[SurveyScope],[IsValid],[CreatedAt])
VALUES(@ReportID,@SurveyScope,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@SurveyScope", scope.SurveyScope);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除研究調查範圍
            if (delSurveyScopes.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_SurveyScopes]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE ScopeID IN (" + string.Join(",", delSurveyScopes) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增載具
            foreach (var carrier in carriers)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_Carrier]([ReportID],[CarrierTypeID],[CarrierDetail],[CarrierNo],[IsValid],[CreatedAt])
VALUES(@ReportID,@CarrierTypeID,@CarrierDetail,@CarrierNo,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@CarrierTypeID", carrier.CarrierTypeID);
                db.Parameters.Add("@CarrierDetail", carrier.CarrierDetail);
                db.Parameters.Add("@CarrierNo", carrier.CarrierNo);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除載具
            if (delCarriers.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_Carrier]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE CarrierID IN (" + string.Join(",", delCarriers) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增研究調查範圍(縣市)
            foreach (var county in surveyCounties)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_SurveyCounties]([ReportID],[CountyID],[IsValid],[CreatedAt])
VALUES(@ReportID,@CountyID,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@CountyID", county.CountyID);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除研究調查範圍(縣市)
            if (delSurveyCounties.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_SurveyCounties]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE SurveyCountyID IN (" + string.Join(",", delSurveyCounties) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            db.Commit();
        }
        catch (Exception ex)
        {
            db.Rollback();
            reportID = 0;
        }

        return reportID;
    }

    /// <summary>
    /// 更新一筆活動資料及其子表資料
    /// </summary>
    public static bool UpdateReport(
        int reportID,
        OSI_ActivityReports report,
        List<OSI_ActivityExecutors> executors,
        List<OSI_ResearchPeriods> resPeriods,
        List<OSI_ActivityFiles> files,
        List<OSI_SurveyScopes> surveyScopes,
        List<OSI_Carrier> carriers,
        List<OSI_SurveyCounties> surveyCounties,
        List<int> delExecutors,
        List<int> delResPeriods,
        List<int> delFiles,
        List<int> delSurveyScopes,
        List<int> delCarriers,
        List<int> delSurveyCounties)
    {
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
UPDATE [dbo].[OSI_ActivityReports]
SET 
     [PeriodID] = @PeriodID
    ,[ReportingUnitID] = @ReportingUnitID
    ,[ActivityName] = @ActivityName
    ,[NatureID] = @NatureID
    ,[NatureText] = @NatureText
    ,[ResearchItemID] = @ResearchItemID
    ,[ResearchItemNote] = @ResearchItemNote
    ,[Instruments] = @Instruments
    ,[ActivityOverview] = @ActivityOverview
    ,[LastUpdated] = @LastUpdated
    ,[LastUpdatedBy] = @LastUpdatedBy
    ,[IsValid] = @IsValid
 WHERE ReportID = @ReportID
";

            db.Parameters.Clear();
            db.Parameters.Add("@ReportID", reportID);
            db.Parameters.Add("@PeriodID", report.PeriodID);
            db.Parameters.Add("@ReportingUnitID", report.ReportingUnitID);
            db.Parameters.Add("@ActivityName", report.ActivityName);
            db.Parameters.Add("@NatureID", report.NatureID);
            db.Parameters.Add("@NatureText", report.NatureText);
            db.Parameters.Add("@ResearchItemID", report.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", report.ResearchItemNote);
            db.Parameters.Add("@Instruments", report.Instruments);
            db.Parameters.Add("@ActivityOverview", report.ActivityOverview);
            db.Parameters.Add("@LastUpdated", report.LastUpdated);
            db.Parameters.Add("@LastUpdatedBy", report.LastUpdatedBy);
            db.Parameters.Add("@IsValid", report.IsValid);
            db.ExecuteNonQuery();

            // 新增活動執行者
            foreach (var executor in executors)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityExecutors]([ReportID],[CategoryID],[ExecutorName],[IsValid])
VALUES(@ReportID,@CategoryID,@ExecutorName,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@CategoryID", executor.CategoryID);
                db.Parameters.Add("@ExecutorName", executor.ExecutorName);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除活動執行者
            if (delExecutors.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_ActivityExecutors]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE ExecutorID IN (" + string.Join(",", delExecutors) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增研究調查日期
            foreach (var resPeriod in resPeriods)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_ResearchPeriods]([ReportID],[StartDate],[EndDate],[PeriodLabel],[IsValid])
VALUES(@ReportID,@StartDate,@EndDate,@PeriodLabel,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@StartDate", resPeriod.StartDate);
                db.Parameters.Add("@EndDate", resPeriod.EndDate);
                db.Parameters.Add("@PeriodLabel", resPeriod.PeriodLabel);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除研究調查日期
            if (delResPeriods.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_ResearchPeriods]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE PeriodID IN (" + string.Join(",", delResPeriods) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增活動檔案
            foreach (var file in files)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityFiles]([ReportID],[FileName],[FilePath],[IsValid])
VALUES(@ReportID,@FileName,@FilePath,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@FileName", file.FileName);
                db.Parameters.Add("@FilePath", file.FilePath);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }
            // 刪除活動檔案
            if (delFiles.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_ActivityFiles]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE AttachmentID IN (" + string.Join(",", delFiles) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增研究調查範圍
            foreach (var scope in surveyScopes)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_SurveyScopes]([ReportID],[SurveyScope],[IsValid],[CreatedAt])
VALUES(@ReportID,@SurveyScope,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@SurveyScope", scope.SurveyScope);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除研究調查範圍
            if (delSurveyScopes.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_SurveyScopes]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE ScopeID IN (" + string.Join(",", delSurveyScopes) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增載具
            foreach (var carrier in carriers)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_Carrier]([ReportID],[CarrierTypeID],[CarrierDetail],[CarrierNo],[IsValid],[CreatedAt])
VALUES(@ReportID,@CarrierTypeID,@CarrierDetail,@CarrierNo,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@CarrierTypeID", carrier.CarrierTypeID);
                db.Parameters.Add("@CarrierDetail", carrier.CarrierDetail);
                db.Parameters.Add("@CarrierNo", carrier.CarrierNo);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除載具
            if (delCarriers.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_Carrier]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE CarrierID IN (" + string.Join(",", delCarriers) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            // 新增研究調查範圍(縣市)
            foreach (var county in surveyCounties)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_SurveyCounties]([ReportID],[CountyID],[IsValid],[CreatedAt])
VALUES(@ReportID,@CountyID,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@CountyID", county.CountyID);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 刪除研究調查範圍(縣市)
            if (delSurveyCounties.Count > 0)
            {
                db.CommandText = @"
UPDATE [dbo].[OSI_SurveyCounties]
SET IsValid = 0
, DeletedAt = GETDATE()
, DeletedBy = @DeletedBy
WHERE SurveyCountyID IN (" + string.Join(",", delSurveyCounties) + @")
";
                db.Parameters.Clear();
                db.Parameters.Add("@DeletedBy", report.LastUpdatedBy);
                db.ExecuteNonQuery();
            }

            db.Commit();
            return true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            return false;
        }
    }

    /// <summary>
    /// 複製一筆活動資料及其子表資料
    /// </summary>
    public static int CopyReportByID(int reportID, string periodID, out string sourcePath, out string destPath)
    {
        sourcePath = "";
        destPath = "";
        int newReportID = 0;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            OSI_ActivityReports copyActRptData = QueryByIDWithClass(reportID.ToString());
            db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityReports]([PeriodID],[ReportingUnitID],[ActivityName],[NatureID],[NatureText],[ResearchItemID],[ResearchItemNote],[Instruments],[ActivityOverview],[LastUpdated],[LastUpdatedBy],[IsValid],[CopyReportID])
VALUES(@PeriodID,@ReportingUnitID,@ActivityName,@NatureID,@NatureText,@ResearchItemID,@ResearchItemNote,@Instruments,@ActivityOverview,@LastUpdated,@LastUpdatedBy,@IsValid,@CopyReportID)

 SELECT SCOPE_IDENTITY();
";

            db.Parameters.Clear();
            db.Parameters.Add("@PeriodID", periodID);
            db.Parameters.Add("@ReportingUnitID", copyActRptData.ReportingUnitID);
            db.Parameters.Add("@ActivityName", copyActRptData.ActivityName);
            db.Parameters.Add("@NatureID", copyActRptData.NatureID);
            db.Parameters.Add("@NatureText", copyActRptData.NatureText);
            db.Parameters.Add("@ResearchItemID", copyActRptData.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", copyActRptData.ResearchItemNote);
            db.Parameters.Add("@Instruments", copyActRptData.Instruments);
            db.Parameters.Add("@ActivityOverview", copyActRptData.ActivityOverview);
            db.Parameters.Add("@LastUpdated", copyActRptData.LastUpdated);
            db.Parameters.Add("@LastUpdatedBy", copyActRptData.LastUpdatedBy);
            db.Parameters.Add("@IsValid", copyActRptData.IsValid);
            db.Parameters.Add("@CopyReportID", reportID);

            object result = db.GetDataSet().Tables[0].Rows[0][0];
            if (result == null || result == DBNull.Value)
                throw new Exception("取得 ReportID 失敗：結果為 null 或 DBNull");
            newReportID = Convert.ToInt32(result);

            // 新增活動執行者
            List<OSI_ActivityExecutors> copyExecutors = OSIActivityExecutorsHelper.QueryByReportIDWithClass(reportID.ToString());
            foreach (var executor in copyExecutors)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityExecutors]([ReportID],[CategoryID],[ExecutorName],[IsValid])
VALUES(@ReportID,@CategoryID,@ExecutorName,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", newReportID);
                db.Parameters.Add("@CategoryID", executor.CategoryID);
                db.Parameters.Add("@ExecutorName", executor.ExecutorName);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 新增研究調查日期
            List<OSI_ResearchPeriods> copyResPeriods = OSIResearchPeriodsHelper.QueryByReportIDWithClass(reportID.ToString());
            foreach (var resPeriod in copyResPeriods)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_ResearchPeriods]([ReportID],[StartDate],[EndDate],[PeriodLabel],[IsValid])
VALUES(@ReportID,@StartDate,@EndDate,@PeriodLabel,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", newReportID);
                db.Parameters.Add("@StartDate", resPeriod.StartDate);
                db.Parameters.Add("@EndDate", resPeriod.EndDate);
                db.Parameters.Add("@PeriodLabel", resPeriod.PeriodLabel);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 新增活動檔案
            List<OSI_ActivityFiles> copyFiles = OSIActivityFilesHelper.QueryByReportIDWithClass(reportID.ToString());
            foreach (var file in copyFiles)
            {
                file.FilePath.Replace($"Report_{reportID}", $"Report_{newReportID}");

                db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityFiles]([ReportID],[FileName],[FilePath],[IsValid])
VALUES(@ReportID,@FileName,@FilePath,@IsValid)
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", newReportID);
                db.Parameters.Add("@FileName", file.FileName);
                db.Parameters.Add("@FilePath", file.FilePath);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 新增研究調查範圍
            List<OSI_SurveyScopes> copySurveyScopes = OSISurveyScopesHelper.QueryByReportIDWithClass(reportID.ToString());
            foreach (var scope in copySurveyScopes)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_SurveyScopes]([ReportID],[SurveyScope],[IsValid],[CreatedAt])
VALUES(@ReportID,@SurveyScope,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", newReportID);
                db.Parameters.Add("@SurveyScope", scope.SurveyScope);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            // 新增載具
            List<OSI_Carrier> copyCarriers = OSICarrierHelper.QueryByReportIDWithClass(reportID.ToString());
            foreach (var carrier in copyCarriers)
            {
                db.CommandText = @"
INSERT INTO [dbo].[OSI_Carrier]([ReportID],[CarrierTypeID],[CarrierDetail],[CarrierNo],[IsValid],[CreatedAt])
VALUES(@ReportID,@CarrierTypeID,@CarrierDetail,@CarrierNo,@IsValid,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", newReportID);
                db.Parameters.Add("@CarrierTypeID", carrier.CarrierTypeID);
                db.Parameters.Add("@CarrierDetail", carrier.CarrierDetail);
                db.Parameters.Add("@CarrierNo", carrier.CarrierNo);
                db.Parameters.Add("@IsValid", 1);
                db.ExecuteNonQuery();
            }

            if (copyFiles.Count > 0)
            {
                string oldFolder = Path.GetDirectoryName(copyFiles[0].FilePath);
                sourcePath = oldFolder;
                var parentFolder = Path.GetFullPath(Path.Combine(oldFolder, ".."));
                string newFolderName = $"Report_{newReportID}";
                destPath = Path.Combine(parentFolder, newFolderName);
            }

            // 更新DataPeriods
            OSIDataPeriodsHelper.UpdateIsCopyByID(periodID);

            db.Commit();
        }
        catch (Exception ex)
        {
            db.Rollback();
            newReportID = 0;
            sourcePath = "";
            destPath = "";
        }

        return newReportID;
    }



    /// <summary>
    /// 刪除 By ID
    /// </summary>
    /// <returns></returns>
    public static bool DeleteByID(string reportID)
    {
        var db = new DbHelper();
        int rowsAffected = 0;

        db.BeginTrans();
        try
        {
            db.CommandText = @"
            UPDATE OSI_ActivityReports
               SET IsValid = 0
            WHERE ReportID  = @ReportID

            SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@ReportID", reportID);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            if (rowsAffected > 0)
                db.Commit();
            else
                db.Rollback();
        }
        catch
        {
            db.Rollback();
            rowsAffected = 0;
        }

        return rowsAffected > 0;
    }

    /// <summary>
    /// 查詢活動分布行政區分析資料
    /// </summary>
    /// <param name="periodID">期間ID</param>
    /// <returns></returns>
    public static GisTable QueryRegionDashboard(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
WITH ActivityCountyMapping AS (
    SELECT 
        r.ReportID,
        c.c_name,
        u.UnitName
    FROM OSI_ActivityReports r
    INNER JOIN OSI_Geom g ON r.ReportID = g.ReportID
    INNER JOIN OSI_MapCounty c ON 
        (CASE WHEN c.geom.STIsValid() = 1 THEN c.geom ELSE c.geom.MakeValid() END)
        .STIntersects(
            CASE WHEN g.GeoData.STIsValid() = 1 THEN g.GeoData ELSE g.GeoData.MakeValid() END
        ) = 1
    INNER JOIN Sys_Unit u ON r.ReportingUnitID = u.UnitID
    WHERE r.IsValid = 1 
        AND r.PeriodID = @PeriodID
        AND g.IsValid = 1
        AND g.DeletedAt IS NULL
        AND g.GeoData IS NOT NULL
        AND c.geom IS NOT NULL
),
CountiesBySurvey AS (
    SELECT
        r.ReportID,
        c.c_name,
        u.UnitName
    FROM OSI_ActivityReports AS r
    INNER JOIN OSI_SurveyCounties AS sc
        ON r.ReportID = sc.ReportID
       AND sc.IsValid = 1
       AND sc.DeletedAt IS NULL
    INNER JOIN OSI_MapCounty AS c
        ON sc.CountyID = c.qgs_fid
    INNER JOIN Sys_Unit AS u
        ON r.ReportingUnitID = u.UnitID
    WHERE r.IsValid = 1
      AND r.PeriodID = @PeriodID
      AND sc.IsValid = 1
	  AND sc.DeletedAt IS NULL
),
 Combined AS (
  SELECT ReportID, c_name, UnitName FROM ActivityCountyMapping
  UNION
  SELECT ReportID, c_name, UnitName FROM CountiesBySurvey
)
SELECT 
    c_name AS RegionName,
    COALESCE(UnitName, '未分類') AS SeriesName,
    COUNT(*) AS Value
FROM Combined
GROUP BY c_name, UnitName
HAVING COUNT(*) > 0
ORDER BY c_name, UnitName
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢單位活動填報情形分析資料
    /// </summary>
    /// <param name="periodID">期間ID</param>
    /// <returns></returns>
    public static GisTable QueryUnitDashboard(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
WITH UnitCountyData AS (
    SELECT
        r.ReportID,
        c.c_name,
		u.UnitName
    FROM OSI_ActivityReports r
    INNER JOIN OSI_Geom g ON r.ReportID = g.ReportID
    INNER JOIN Sys_Unit u ON r.ReportingUnitID = u.UnitID
    INNER JOIN OSI_MapCounty c ON 
        (CASE WHEN c.geom.STIsValid() = 1 THEN c.geom ELSE c.geom.MakeValid() END)
        .STIntersects(
            CASE WHEN g.GeoData.STIsValid() = 1 THEN g.GeoData ELSE g.GeoData.MakeValid() END
        ) = 1
    WHERE r.IsValid = 1 
        AND r.PeriodID = @PeriodID
        AND g.IsValid = 1
        AND g.DeletedAt IS NULL
        AND g.GeoData IS NOT NULL
        AND c.geom IS NOT NULL
    GROUP BY u.UnitName, c.c_name, r.ReportID
    HAVING COUNT(*) > 0
),
CountiesBySurvey AS (
    SELECT
        r.ReportID,
        c.c_name,
		u.UnitName
    FROM OSI_ActivityReports r
    INNER JOIN OSI_SurveyCounties sc ON r.ReportID = sc.ReportID
    INNER JOIN Sys_Unit u ON r.ReportingUnitID = u.UnitID
    INNER JOIN OSI_MapCounty c ON sc.CountyID = c.qgs_fid
    WHERE r.IsValid = 1 
        AND r.PeriodID = @PeriodID
		AND sc.IsValid = 1
		AND sc.DeletedAt IS NULL
    GROUP BY u.UnitName, c.c_name, r.ReportID
    HAVING COUNT(*) > 0
),
 Combined AS (
  SELECT ReportID, c_name, UnitName FROM UnitCountyData
  UNION
  SELECT ReportID, c_name, UnitName FROM CountiesBySurvey
)
SELECT 
	UnitName,
    c_name AS CountyName,
    COUNT(*) AS ActivityCount
FROM Combined
GROUP BY UnitName, c_name
ORDER BY UnitName, c_name
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 檢查指定單位是否有活動報告
    /// </summary>
    /// <param name="unitID">單位ID</param>
    /// <returns>true表示有活動報告，false表示沒有</returns>
    public static bool HasReportsByUnitID(string unitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT COUNT(*) as ReportCount
            FROM OSI_ActivityReports
            WHERE ReportingUnitID = @UnitID
            AND IsValid = 1";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitID", unitID);

        var tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            int reportCount = Convert.ToInt32(tbl.Rows[0]["ReportCount"]);
            return reportCount > 0;
        }

        return false;
    }

    /// <summary>
    /// 處理 OSI_Geom 的儲存（新增或更新）- 支援 JSON 格式和智慧更新
    /// </summary>
    public static bool SaveGeometries(int reportID, string featuresJson, int userID)
    {
        bool success = false;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            // 如果是空的或 null，則軟刪除所有現有圖徵
            if (string.IsNullOrEmpty(featuresJson) || string.Equals(featuresJson, "null", StringComparison.OrdinalIgnoreCase))
            {
                db.CommandText = @"
UPDATE OSI_Geom 
SET IsValid = 0, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
WHERE ReportID = @ReportID AND IsValid = 1 AND DeletedAt IS NULL";

                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@DeletedBy", userID);
                db.ExecuteNonQuery();
            }
            else
            {
                // 嘗試解析 JSON 格式
                try
                {
                    var featureCollection = Newtonsoft.Json.JsonConvert.DeserializeObject<FeatureCollection>(featuresJson);
                    if (featureCollection != null && featureCollection.features != null)
                    {
                        // 取得現有的圖徵
                        var existingGeoms = OSIGeomHelper.QueryByReportIDWithClass(reportID.ToString());
                        var existingDict = existingGeoms.ToDictionary(g => g.GeomID, g => g);
                        var processedIds = new HashSet<string>();

                        // 處理每個 feature
                        foreach (var feature in featureCollection.features)
                        {
                            if (!string.IsNullOrEmpty(feature.id) && existingDict.ContainsKey(feature.id))
                            {
                                // 檢查現有圖徵是否有變更
                                var existingGeom = existingDict[feature.id];
                                var newName = feature.name ?? "";
                                var newWkt = feature.wkt ?? "";

                                // 判斷 GeomName 或 WKT 是否有變更
                                bool hasNameChanges = !string.Equals(existingGeom.GeomName, newName, StringComparison.Ordinal);

                                // 比較 WKT 到小數第4位
                                bool hasWktChanges = false;
                                if (!string.IsNullOrEmpty(existingGeom.GeoData) && !string.IsNullOrEmpty(newWkt))
                                {
                                    // 正規化 WKT 字串到小數第6位來比較
                                    var normalizedExisting = NormalizeWktPrecision(existingGeom.GeoData, 4);
                                    var normalizedNew = NormalizeWktPrecision(newWkt, 4);
                                    hasWktChanges = !string.Equals(normalizedExisting, normalizedNew, StringComparison.Ordinal);
                                }
                                else if (string.IsNullOrEmpty(existingGeom.GeoData) != string.IsNullOrEmpty(newWkt))
                                {
                                    // 一個是空的，另一個不是
                                    hasWktChanges = true;
                                }

                                bool hasChanges = hasNameChanges || hasWktChanges;

                                if (hasChanges)
                                {
                                    // 有變更：軟刪除舊資料
                                    OSIGeomHelper.DeleteGeom(existingGeom.GeomID, userID);

                                    // 新增一筆新資料
                                    var newGeom = new OSI_Geom
                                    {
                                        ReportID = reportID,
                                        GeomName = newName,
                                        GeoData = newWkt,
                                        IsValid = true,
                                        CreatedAt = DateTime.Now
                                    };
                                    OSIGeomHelper.InsertGeom(newGeom);
                                }

                                processedIds.Add(feature.id);
                            }
                            else
                            {
                                // 新增圖徵
                                var newGeom = new OSI_Geom
                                {
                                    ReportID = reportID,
                                    GeomName = feature.name ?? "",
                                    GeoData = feature.wkt,
                                    IsValid = true,
                                    CreatedAt = DateTime.Now
                                };
                                OSIGeomHelper.InsertGeom(newGeom);
                            }
                        }

                        // 軟刪除未處理的圖徵
                        foreach (var existingGeom in existingGeoms)
                        {
                            if (!processedIds.Contains(existingGeom.GeomID))
                            {
                                OSIGeomHelper.DeleteGeom(existingGeom.GeomID, userID);
                            }
                        }
                    }
                }
                catch (Newtonsoft.Json.JsonException)
                {
                }
            }

            db.Commit();
            success = true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            throw ex;
        }

        return success;
    }

    /// <summary>
    /// 將 WKT 字串中的座標正規化到指定的小數位數，並標準化格式
    /// </summary>
    /// <param name="wkt">原始 WKT 字串</param>
    /// <param name="precision">小數位數</param>
    /// <returns>正規化後的 WKT 字串</returns>
    private static string NormalizeWktPrecision(string wkt, int precision)
    {
        if (string.IsNullOrEmpty(wkt))
            return wkt;

        // 先正規化座標精度
        var pattern = @"-?\d+\.?\d*";
        var result = System.Text.RegularExpressions.Regex.Replace(wkt, pattern, match =>
        {
            if (double.TryParse(match.Value, out double value))
            {
                return Math.Round(value, precision).ToString($"F{precision}");
            }
            return match.Value;
        });

        // 標準化 WKT 格式：移除幾何類型和括號之間的空格
        // 例如：POINT (x y) -> POINT(x y)
        result = System.Text.RegularExpressions.Regex.Replace(result, @"(POINT|LINESTRING|POLYGON|MULTIPOINT|MULTILINESTRING|MULTIPOLYGON|GEOMETRYCOLLECTION)\s+\(", "$1(");

        // 標準化座標之間的空格：確保只有一個空格
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");

        // 移除括號內部開頭和結尾的空格
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\(\s+", "(");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+\)", ")");

        // 標準化逗號後的空格：確保逗號後有一個空格
        result = System.Text.RegularExpressions.Regex.Replace(result, @",\s*", ", ");

        return result.Trim();
    }

    /// <summary>
    /// Feature Collection 類別，用於解析前端傳來的 JSON 資料
    /// </summary>
    private class FeatureCollection
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }

    /// <summary>
    /// Feature 類別
    /// </summary>
    private class Feature
    {
        public string id { get; set; }
        public string name { get; set; }
        public string wkt { get; set; }
    }

}
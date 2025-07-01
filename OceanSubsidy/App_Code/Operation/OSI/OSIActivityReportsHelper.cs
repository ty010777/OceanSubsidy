using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

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
    ,[CarrierTypeID]
    ,[CarrierDetail]
    ,[CarrierNo]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[SurveyScope]
    ,[GeoData]
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
    ,[CarrierTypeID]
    ,[CarrierDetail]
    ,[CarrierNo]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[SurveyScope]
    ,[GeoData]
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
    ,[CarrierTypeID]
    ,[CarrierDetail]
    ,[CarrierNo]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[SurveyScope]
    ,[GeoData]
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
    ,[CarrierTypeID]
    ,[CarrierDetail]
    ,[CarrierNo]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[SurveyScope]
    ,[GeoData]
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
  r.NatureText,
  e.Executors,
  d.StartDate,
  d.EndDate,
  r.GeoData,
  r.LastUpdated
FROM OSI_ActivityReports r
LEFT JOIN Sys_Unit u 
  ON r.ReportingUnitID = u.UnitID
LEFT JOIN ExecCTE e
  ON e.ReportID = r.ReportID

CROSS APPLY (
  SELECT 
    MIN(rp.StartDate) AS StartDate,
    MAX(rp.EndDate)   AS EndDate
  FROM OSI_ResearchPeriods rp
  WHERE rp.ReportID = r.ReportID
) AS d

WHERE r.PeriodID  = @PeriodID
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
    OR r.CarrierTypeID = @CarrierTypeID
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
    OR r.CarrierNo    LIKE '%' + @Key + '%'
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
        // Executors過長的話，截斷顯示
        foreach (DataRow row in tbl.Rows)
        {
            var exec = row["Executors"]?.ToString() ?? "";
            if (exec.Length > 20)
                row["Executors"] = exec.Substring(0, 20) + "…";
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
  CarrierTypeName    = ISNULL(ct.CarrierTypeName, ''),
  CarrierDetail      = ISNULL(r.CarrierDetail, ''),
  CarrierNo          = ISNULL(r.CarrierNo, ''),
  ItemName           = ISNULL(ri.ItemName, ''),
  ResearchItemNote   = ISNULL(r.ResearchItemNote, ''),
  Instruments        = ISNULL(r.Instruments, ''),
  ActivityOverview   = ISNULL(r.ActivityOverview, ''),
  SurveyScope        = ISNULL(r.SurveyScope, ''),
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
LEFT JOIN OSI_CarrierTypes ct
  ON r.CarrierTypeID = ct.CarrierTypeID
LEFT JOIN Sys_User su
  ON r.LastUpdatedBy = su.UserID
LEFT JOIN ExecCTE e
  ON e.ReportID = r.ReportID
LEFT JOIN PeriodCTE p
  ON p.ReportID = r.ReportID
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

        sb.Append("ORDER BY dp.PeriodYear, dp.QuarterStartDate;");

        // 最後把拼好的 SQL 下到 db
        db.CommandText = sb.ToString();
        return db.GetTable();
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
JOIN OSI_ActivityReports r ON r.CarrierTypeID = c.CarrierTypeID
WHERE r.IsValid = 1
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
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT GeoData
FROM OSI_ActivityReports
WHERE ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);
        var tbl = db.GetTable();
        if (tbl.Rows.Count > 0)
        {
            var row = tbl.Rows[0];
            if (row["GeoData"] != null && row["GeoData"] != DBNull.Value)
            {
                rtVal = row["GeoData"].ToString();
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
        List<int> delExecutors,
        List<int> delResPeriods,
        List<int> delFiles,
        string baseDir)
    {
        int reportID = 0;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityReports]([PeriodID],[ReportingUnitID],[ActivityName],[NatureID],[NatureText],[CarrierTypeID],[CarrierDetail],[CarrierNo],[ResearchItemID],[ResearchItemNote],[Instruments],[ActivityOverview],[SurveyScope],[GeoData],[LastUpdated],[LastUpdatedBy],[IsValid])
VALUES(@PeriodID,@ReportingUnitID,@ActivityName,@NatureID,@NatureText,@CarrierTypeID,@CarrierDetail,@CarrierNo,@ResearchItemID,@ResearchItemNote,@Instruments,@ActivityOverview,@SurveyScope,
geometry::STGeomFromText(@GeoData, 3826),
@LastUpdated,@LastUpdatedBy,@IsValid)

 SELECT SCOPE_IDENTITY();
";

            db.Parameters.Clear();
            db.Parameters.Add("@PeriodID", report.PeriodID);
            db.Parameters.Add("@ReportingUnitID", report.ReportingUnitID);
            db.Parameters.Add("@ActivityName", report.ActivityName);
            db.Parameters.Add("@NatureID", report.NatureID);
            db.Parameters.Add("@NatureText", report.NatureText);
            db.Parameters.Add("@CarrierTypeID", report.CarrierTypeID);
            db.Parameters.Add("@CarrierDetail", report.CarrierDetail);
            db.Parameters.Add("@CarrierNo", report.CarrierNo);
            db.Parameters.Add("@ResearchItemID", report.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", report.ResearchItemNote);
            db.Parameters.Add("@Instruments", report.Instruments);
            db.Parameters.Add("@ActivityOverview", report.ActivityOverview);
            db.Parameters.Add("@SurveyScope", report.SurveyScope);
            db.Parameters.Add("@GeoData", report.GeoData);
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
        List<int> delExecutors,
        List<int> delResPeriods,
        List<int> delFiles)
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
    ,[CarrierTypeID] = @CarrierTypeID
    ,[CarrierDetail] = @CarrierDetail
    ,[CarrierNo] = @CarrierNo
    ,[ResearchItemID] = @ResearchItemID
    ,[ResearchItemNote] = @ResearchItemNote
    ,[Instruments] = @Instruments
    ,[ActivityOverview] = @ActivityOverview
    ,[SurveyScope] = @SurveyScope
    ,[GeoData] = geometry::STGeomFromText(@GeoData, 3826)
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
            db.Parameters.Add("@CarrierTypeID", report.CarrierTypeID);
            db.Parameters.Add("@CarrierDetail", report.CarrierDetail);
            db.Parameters.Add("@CarrierNo", report.CarrierNo);
            db.Parameters.Add("@ResearchItemID", report.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", report.ResearchItemNote);
            db.Parameters.Add("@Instruments", report.Instruments);
            db.Parameters.Add("@ActivityOverview", report.ActivityOverview);
            db.Parameters.Add("@SurveyScope", report.SurveyScope);
            db.Parameters.Add("@GeoData", report.GeoData);
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
INSERT INTO [dbo].[OSI_ActivityReports]([PeriodID],[ReportingUnitID],[ActivityName],[NatureID],[NatureText],[CarrierTypeID],[CarrierDetail],[CarrierNo],[ResearchItemID],[ResearchItemNote],[Instruments],[ActivityOverview],[SurveyScope],[GeoData],[LastUpdated],[LastUpdatedBy],[IsValid],[CopyReportID])
VALUES(@PeriodID,@ReportingUnitID,@ActivityName,@NatureID,@NatureText,@CarrierTypeID,@CarrierDetail,@CarrierNo,@ResearchItemID,@ResearchItemNote,@Instruments,@ActivityOverview,@SurveyScope,
geometry::STGeomFromText(@GeoData, 3826),
@LastUpdated,@LastUpdatedBy,@IsValid,@CopyReportID)

 SELECT SCOPE_IDENTITY();
";

            db.Parameters.Clear();
            db.Parameters.Add("@PeriodID", periodID);
            db.Parameters.Add("@ReportingUnitID", copyActRptData.ReportingUnitID);
            db.Parameters.Add("@ActivityName", copyActRptData.ActivityName);
            db.Parameters.Add("@NatureID", copyActRptData.NatureID);
            db.Parameters.Add("@NatureText", copyActRptData.NatureText);
            db.Parameters.Add("@CarrierTypeID", copyActRptData.CarrierTypeID);
            db.Parameters.Add("@CarrierDetail", copyActRptData.CarrierDetail);
            db.Parameters.Add("@CarrierNo", copyActRptData.CarrierNo);
            db.Parameters.Add("@ResearchItemID", copyActRptData.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", copyActRptData.ResearchItemNote);
            db.Parameters.Add("@Instruments", copyActRptData.Instruments);
            db.Parameters.Add("@ActivityOverview", copyActRptData.ActivityOverview);
            db.Parameters.Add("@SurveyScope", copyActRptData.SurveyScope);
            db.Parameters.Add("@GeoData", copyActRptData.GeoData);
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
    INNER JOIN OSI_MapCounty c ON 
        (CASE WHEN c.geom.STIsValid() = 1 THEN c.geom ELSE c.geom.MakeValid() END)
        .STIntersects(
            CASE WHEN r.GeoData.STIsValid() = 1 THEN r.GeoData ELSE r.GeoData.MakeValid() END
        ) = 1
    INNER JOIN Sys_Unit u ON r.ReportingUnitID = u.UnitID
    WHERE r.IsValid = 1 
        AND r.PeriodID = @PeriodID
        AND r.GeoData IS NOT NULL
        AND c.geom IS NOT NULL
)
SELECT 
    c_name AS RegionName,
    COALESCE(UnitName, '未分類') AS SeriesName,
    COUNT(*) AS Value
FROM ActivityCountyMapping
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
        u.UnitName,
        c.c_name AS CountyName,
        COUNT(*) AS ActivityCount
    FROM OSI_ActivityReports r
    INNER JOIN Sys_Unit u ON r.ReportingUnitID = u.UnitID
    INNER JOIN OSI_MapCounty c ON 
        (CASE WHEN c.geom.STIsValid() = 1 THEN c.geom ELSE c.geom.MakeValid() END)
        .STIntersects(
            CASE WHEN r.GeoData.STIsValid() = 1 THEN r.GeoData ELSE r.GeoData.MakeValid() END
        ) = 1
    WHERE r.IsValid = 1 
        AND r.PeriodID = @PeriodID
        AND r.GeoData IS NOT NULL
        AND c.geom IS NOT NULL
    GROUP BY u.UnitName, c.c_name
    HAVING COUNT(*) > 0
)
SELECT 
    UnitName,
    CountyName,
    ActivityCount
FROM UnitCountyData
ORDER BY UnitName, CountyName
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        
        return db.GetTable();
    }

}
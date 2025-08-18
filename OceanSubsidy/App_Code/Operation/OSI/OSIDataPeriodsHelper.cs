using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using NPOI.HSSF.Record.Chart;

/// <summary>
/// OSIDataPeriodsHelper 的摘要描述
/// </summary>
public class OSIDataPeriodsHelper
{
    public OSIDataPeriodsHelper()
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
SELECT [PeriodID]
      ,[PeriodYear]
      ,[PeriodQuarter]
      ,[StartDate]
      ,[EndDate]
      ,[Color]
      ,[QuarterStartDate]
      ,[QuarterEndDate]
      ,[IsCopy]
  FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    public static List<OSI_DataPeriods> QueryAllWithClass()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
      ,[PeriodYear]
      ,[PeriodQuarter]
      ,[StartDate]
      ,[EndDate]
      ,[Color]
      ,[QuarterStartDate]
      ,[QuarterEndDate]
      ,[IsCopy]
  FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
";
        db.Parameters.Clear();

        return db.GetList<OSI_DataPeriods>();
    }


    /// <summary>
    /// 查詢 BY ID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByID(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE PeriodID = @PeriodID
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢所有年分
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAllYears(string minYear = "0")
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT DISTINCT  [PeriodYear]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE PeriodYear >= @MinYear
ORDER BY PeriodYear
";
        db.Parameters.Clear();
        db.Parameters.Add("@MinYear", minYear);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢季度 BY 年
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryQuartersByYear(string year)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE PeriodYear = @PeriodYear
ORDER BY PeriodQuarter
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodYear", year);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢所有年分（含篩選條件：指定日期 >= StartDate）
    /// </summary>
    /// <param name="targetDate">目標日期，預設為今天</param>
    /// <returns></returns>
    public static GisTable QueryAllYearsWithFilter(DateTime? targetDate = null)
    {
        // 若未指定日期，使用今天
        DateTime dateToCheck = targetDate ?? DateTime.Now;
        
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT DISTINCT [PeriodYear]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE @TargetDate >= StartDate
ORDER BY PeriodYear
";
        db.Parameters.Clear();
        db.Parameters.Add("@TargetDate", dateToCheck);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢季度 BY 年（含篩選條件：今天 >= StartDate）
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryQuartersByYearWithFilter(string year, DateTime? targetDate = null)
    {
        // 若未指定日期，使用今天
        DateTime dateToCheck = targetDate ?? DateTime.Now;

        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE PeriodYear = @PeriodYear
AND @TargetDate >= StartDate
ORDER BY PeriodQuarter
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodYear", year);
        db.Parameters.Add("@TargetDate", dateToCheck);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢ID BY 年跟季度
    /// </summary>
    /// <returns></returns>
    public static int QueryIDByYearAndQuarter(int year, string quarter)
    {
        int reVal = 0;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE PeriodYear = @PeriodYear
AND PeriodQuarter = @PeriodQuarter
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodYear", year);
        db.Parameters.Add("@PeriodQuarter", quarter);
        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            reVal = Convert.ToInt32(tbl.Rows[0]["PeriodID"]);
        }

        return reVal;
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByRange(string minYear, string maxYear)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
    ,[QuarterStartDate]
    ,[QuarterEndDate]
    ,[IsCopy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE PeriodYear >= @MinYear
AND PeriodYear <= @MaxYear
ORDER BY PeriodYear
";
        db.Parameters.Clear();
        db.Parameters.Add("@MinYear", minYear);
        db.Parameters.Add("@MaxYear", maxYear);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢那些區間包含某時間
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByDateTime(DateTime dateTime)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
    ,[QuarterStartDate]
    ,[QuarterEndDate]
    ,[IsCopy]
FROM OSI_DataPeriods
WHERE StartDate <= @DateTime
AND EndDate >= DATEADD(day, -1, @DateTime)
";
        db.Parameters.Clear();
        db.Parameters.Add("@DateTime", dateTime);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢那些區間包含某時間
    /// </summary>
    /// <returns></returns>
    public static List<OSI_DataPeriods> QueryByDateTimeWithClass(DateTime dateTime)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
    ,[QuarterStartDate]
    ,[QuarterEndDate]
    ,[IsCopy]
FROM OSI_DataPeriods
WHERE StartDate <= @DateTime
AND EndDate >= DATEADD(day, -1, @DateTime)
";
        db.Parameters.Clear();
        db.Parameters.Add("@DateTime", dateTime);

        return db.GetList<OSI_DataPeriods>();
    }


    /// <summary>
    /// 查詢 BY 每季開始時間
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByQuarterStartDate(DateTime quarterStartDate)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
    ,[QuarterStartDate]
    ,[QuarterEndDate]
FROM [OCA_OceanSubsidy].[dbo].[OSI_DataPeriods]
WHERE CONVERT(date, QuarterStartDate) = CONVERT(date, @QuarterStartDate);
";
        db.Parameters.Clear();
        db.Parameters.Add("@QuarterStartDate", quarterStartDate);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢有填報的父單位數量（包含子單位有填報的情況）
    /// </summary>
    /// <returns></returns>
    public static int QueryUnitCountByID(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT COUNT(DISTINCT p.UnitID) as ParentUnitCount
FROM Sys_Unit p
WHERE p.IsValid = 1
AND p.ParentUnitID IS NULL
AND p.UnitName <> N'其他'
AND (
    EXISTS (
        SELECT 1 FROM OSI_ActivityReports r 
        WHERE r.PeriodID = @PeriodID 
        AND r.ReportingUnitID = p.UnitID
        AND r.IsValid = 1
    )
    OR
    EXISTS (
        SELECT 1 FROM OSI_ActivityReports r 
        INNER JOIN Sys_Unit c ON r.ReportingUnitID = c.UnitID
        WHERE r.PeriodID = @PeriodID 
        AND c.ParentUnitID = p.UnitID
        AND c.IsValid = 1
        AND r.IsValid = 1
    )
)
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        var tbl = db.GetTable();

        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["ParentUnitCount"]);
        }

        return 0;
    }

    /// <summary>
    /// 根據 PeriodID 及機關類型 查詢填報活動資料的單位數（包含子單位有填報資料的）
    /// </summary>
    /// <param name="periodID">期間ID</param>
    /// <param name="govUnitTypeID">機關類型ID (1=中央機關, 2=縣市政府)</param>
    /// <returns></returns>
    public static int QueryUnitCountByIDAndGovType(string periodID, int govUnitTypeID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT COUNT(DISTINCT p.UnitID) as ParentUnitCount
FROM Sys_Unit p
WHERE p.IsValid = 1
AND p.ParentUnitID IS NULL
AND p.UnitName <> N'其他'
AND p.GovUnitTypeID = @GovUnitTypeID
AND (
    EXISTS (
        SELECT 1 FROM OSI_ActivityReports r 
        WHERE r.PeriodID = @PeriodID 
        AND r.ReportingUnitID = p.UnitID
        AND r.IsValid = 1
    )
    OR
    EXISTS (
        SELECT 1 FROM OSI_ActivityReports r 
        INNER JOIN Sys_Unit c ON r.ReportingUnitID = c.UnitID
        WHERE r.PeriodID = @PeriodID 
        AND c.ParentUnitID = p.UnitID
        AND c.IsValid = 1
        AND r.IsValid = 1
    )
)
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        db.Parameters.Add("@GovUnitTypeID", govUnitTypeID);
        var tbl = db.GetTable();

        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["ParentUnitCount"]);
        }

        return 0;
    }

    /// <summary>
    /// 更新區間
    /// </summary>
    /// <returns></returns>
    public static void UpdateRangeAndColerByID(int periodID, DateTime startDate, DateTime endDate, string color)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
UPDATE OSI_DataPeriods
SET StartDate = @StartDate,
	EndDate = @EndDate,
    Color = @Color
WHERE PeriodID = @PeriodID
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        db.Parameters.Add("@StartDate", startDate);
        db.Parameters.Add("@EndDate", endDate);
        db.Parameters.Add("@Color", color);
        db.ExecuteNonQuery();

        return;
    }


    /// <summary>
    /// 自動新增明年資料
    /// </summary>
    public static int InsertNextYear()
    {
        int nextROC = DateTime.Today.Year - 1911 + 1;   // 明年民國年
        int curROC = nextROC - 1;                     // 今年民國年
        string curYear = curROC.ToString();
        string nextYear = nextROC.ToString();

        DbHelper db = new DbHelper();

        int rowsAffected = 0;

        db.BeginTrans();
        try
        {
            db.CommandText =
            @"
INSERT INTO OSI_DataPeriods
  (PeriodYear, PeriodQuarter, StartDate, EndDate, Color, QuarterStartDate, QuarterEndDate, IsCopy)
SELECT
    @NextYear
    , src.PeriodQuarter
    , DATEADD(year, 1, src.StartDate)
    , DATEADD(year, 1, src.EndDate)
    , src.Color
    , DATEADD(year, 1, src.QuarterStartDate)
    , DATEADD(year, 1, src.QuarterEndDate)
    , False
FROM OSI_DataPeriods AS src
WHERE src.PeriodYear = @CurYear
    AND NOT EXISTS (
        SELECT 1
        FROM OSI_DataPeriods AS tgt
        WHERE tgt.PeriodYear      = @NextYear
        AND tgt.PeriodQuarter   = src.PeriodQuarter
    );

SELECT CAST(@@ROWCOUNT AS INT);
";
            db.Parameters.Clear();
            db.Parameters.Add("@NextYear", nextYear);
            db.Parameters.Add("@CurYear", curYear);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            db.Commit();
        }
        catch
        {
            db.Rollback();
            rowsAffected = 0;
        }

        return rowsAffected;
    }

    /// <summary>
    /// 判斷是否在填報區間內 BY ID
    /// </summary>
    /// <returns></returns>
    public static bool IsInPeriodByID(int periodID)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 1
FROM dbo.OSI_DataPeriods
WHERE PeriodID = @PeriodID
AND CAST(GETDATE() AS DATE) BETWEEN StartDate AND EndDate
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            rtVal = true;
        }

        return rtVal;
    }

    /// <summary>
    /// 更新是否複製 BY ID
    /// </summary>
    /// <returns></returns>
    public static void UpdateIsCopyByID(string periodID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
UPDATE OSI_DataPeriods
SET IsCopy = 1
WHERE PeriodID = @PeriodID
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        db.ExecuteNonQuery();

        return;
    }

    /// <summary>
    /// 查詢最近已結束的期間
    /// </summary>
    /// <returns>最近已結束的期間資料</returns>
    public static OSI_DataPeriods QueryLatestEndedPeriod()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP 1 
    [PeriodID]
    ,[PeriodYear]
    ,[PeriodQuarter]
    ,[StartDate]
    ,[EndDate]
    ,[Color]
    ,[QuarterStartDate]
    ,[QuarterEndDate]
    ,[IsCopy]
FROM OSI_DataPeriods 
WHERE GETDATE() > EndDate
ORDER BY StartDate DESC";
        
        db.Parameters.Clear();
        var list = db.GetList<OSI_DataPeriods>();
        
        return list != null && list.Count > 0 ? list[0] : null;
    }

    /// <summary>
    /// 查詢昨天結束的期間
    /// </summary>
    /// <returns>昨天結束的期間資料列表</returns>
    public static List<OSI_DataPeriods> QueryEndedYesterday()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT [PeriodID]
      ,[PeriodYear]
      ,[PeriodQuarter]
      ,[StartDate]
      ,[EndDate]
      ,[Color]
      ,[QuarterStartDate]
      ,[QuarterEndDate]
      ,[IsCopy]
FROM OSI_DataPeriods 
WHERE CONVERT(date, EndDate) = CONVERT(date, DATEADD(day, -1, GETDATE()))";
        
        db.Parameters.Clear();
        return db.GetList<OSI_DataPeriods>();
    }

    /// <summary>
    /// 查詢今天開始的期間
    /// </summary>
    /// <returns>今天開始的期間資料列表</returns>
    public static List<OSI_DataPeriods> QueryStartingToday()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT [PeriodID]
      ,[PeriodYear]
      ,[PeriodQuarter]
      ,[StartDate]
      ,[EndDate]
      ,[Color]
      ,[QuarterStartDate]
      ,[QuarterEndDate]
      ,[IsCopy]
FROM OSI_DataPeriods 
WHERE CONVERT(date, StartDate) = CONVERT(date, GETDATE())";
        
        db.Parameters.Clear();
        return db.GetList<OSI_DataPeriods>();
    }




}
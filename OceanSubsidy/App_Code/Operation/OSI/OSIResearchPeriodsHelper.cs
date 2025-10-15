using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIResearchPeriodsHelper 的摘要描述
/// </summary>
public class OSIResearchPeriodsHelper
{
    public OSIResearchPeriodsHelper()
    {
        
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
    ,[ReportID]
    ,[StartDate]
    ,[EndDate]
    ,[PeriodLabel]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchPeriods]
WHERE IsValid = 1
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ReportID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByReportID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[ReportID]
    ,[StartDate]
    ,[EndDate]
    ,[PeriodLabel]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchPeriods]
WHERE IsValid = 1 
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ReportID
    /// </summary>
    /// <returns></returns>
    public static List<OSI_ResearchPeriods> QueryByReportIDWithClass(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [PeriodID]
    ,[ReportID]
    ,[StartDate]
    ,[EndDate]
    ,[PeriodLabel]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchPeriods]
WHERE IsValid = 1 
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetList<OSI_ResearchPeriods>();
    }

    /// <summary>
    /// 查詢不重複 ReportID By Range
    /// </summary>
    /// <returns></returns>
    public static List<int> QueryReportIDByRange(DateTime startDate, DateTime endDate)
    {
        List<int> reportIDs = new List<int>();

        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT DISTINCT rp.ReportID
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchPeriods] rp
JOIN [OCA_OceanSubsidy].[dbo].[OSI_ActivityReports] ar ON rp.ReportID = ar.ReportID
WHERE rp.IsValid = 1
AND ar.IsValid = 1
AND StartDate <= CONVERT(date, @endDate)
AND EndDate   >= CONVERT(date, @startDate);
";
        db.Parameters.Clear();
        db.Parameters.Add("@startDate", startDate);
        db.Parameters.Add("@endDate", endDate);
        var tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                reportIDs.Add(tbl.Rows[i]["ReportID"].ToString().toInt());
            }
        }

        return reportIDs;
    }


}
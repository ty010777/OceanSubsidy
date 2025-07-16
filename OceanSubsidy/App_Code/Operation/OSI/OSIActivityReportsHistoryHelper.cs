using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIActivityReportsHistoryHelper 的摘要描述
/// </summary>
public class OSIActivityReportsHistoryHelper
{
    public OSIActivityReportsHistoryHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 新增一筆活動資料歷史紀錄
    /// </summary>
    public static bool InsertReport(OSI_ActivityReports report)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityReports_History]
    ([ReportID]
    ,[PeriodID]
    ,[ReportingUnitID]
    ,[ActivityName]
    ,[NatureID]
    ,[NatureText]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[GeoData]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
    ,[CopyReportID]
    ,[AuditAt]
    ,[AuditBy])
VALUES
    (@ReportID
    ,@PeriodID
    ,@ReportingUnitID
    ,@ActivityName
    ,@NatureID
    ,@NatureText
    ,@ResearchItemID
    ,@ResearchItemNote
    ,@Instruments
    ,@ActivityOverview
    ,@GeoData
    ,@LastUpdated
    ,@LastUpdatedBy
    ,@IsValid
    ,@CopyReportID
    ,GETDATE()
    ,@AuditBy)
";

            db.Parameters.Clear();
            db.Parameters.Add("@ReportID", report.ReportID);
            db.Parameters.Add("@PeriodID", report.PeriodID);
            db.Parameters.Add("@ReportingUnitID", report.ReportingUnitID);
            db.Parameters.Add("@ActivityName", report.ActivityName);
            db.Parameters.Add("@NatureID", report.NatureID);
            db.Parameters.Add("@NatureText", report.NatureText);
            db.Parameters.Add("@ResearchItemID", report.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", report.ResearchItemNote);
            db.Parameters.Add("@Instruments", report.Instruments);
            db.Parameters.Add("@ActivityOverview", report.ActivityOverview);
            db.Parameters.Add("@GeoData", report.GeoData);
            db.Parameters.Add("@LastUpdated", report.LastUpdated);
            db.Parameters.Add("@LastUpdatedBy", report.LastUpdatedBy);
            db.Parameters.Add("@CopyReportID", report.CopyReportID);
            db.Parameters.Add("@IsValid", report.IsValid);
            db.Parameters.Add("@AuditBy", report.LastUpdatedBy);

            db.ExecuteNonQuery();
            db.Commit();
            rtVal = true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            rtVal = false;
        }

        return rtVal;
    }




}
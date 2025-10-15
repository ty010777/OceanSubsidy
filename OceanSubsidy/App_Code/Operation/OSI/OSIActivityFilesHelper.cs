using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIActivityFilesHelper 的摘要描述
/// </summary>
public class OSIActivityFilesHelper
{
    public OSIActivityFilesHelper()
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
SELECT [AttachmentID]
    ,[ReportID]
    ,[FileName]
    ,[FilePath]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityFiles]
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
SELECT [AttachmentID]
    ,[ReportID]
    ,[FileName]
    ,[FilePath]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityFiles]
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
    public static List<OSI_ActivityFiles> QueryByReportIDWithClass(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [AttachmentID]
    ,[ReportID]
    ,[FileName]
    ,[FilePath]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityFiles]
WHERE IsValid = 1
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetList<OSI_ActivityFiles>();
    }


}
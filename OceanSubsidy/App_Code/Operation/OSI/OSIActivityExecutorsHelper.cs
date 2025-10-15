using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIActivityExecutorsHelper 的摘要描述
/// </summary>
public class OSIActivityExecutorsHelper
{
    public OSIActivityExecutorsHelper()
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
SELECT [ExecutorID]
    ,[ReportID]
    ,[CategoryID]
    ,[ExecutorName]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityExecutors]
WHERE IsValid = 1
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ID
    /// </summary>
    /// <returns></returns>
    public static List<OSI_ActivityExecutors> QueryByReportIDWithClass(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ExecutorID]
    ,[ReportID]
    ,[CategoryID]
    ,[ExecutorName]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityExecutors]
WHERE IsValid = 1
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetList<OSI_ActivityExecutors>();
    }

    /// <summary>
    /// 查詢 BY ReportID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryNameByReportID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
ae.ExecutorID,
ec.CategoryID,
ec.CategoryName,
ae.ExecutorName
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityExecutors] ae
JOIN OSI_ExecutorCategories ec ON ec.CategoryID = ae.CategoryID
WHERE ae.IsValid = 1
AND ae.ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }


}
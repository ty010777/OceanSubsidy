using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSISurveyScopesHelper 的摘要描述
/// </summary>
public class OSISurveyScopesHelper
{
    public OSISurveyScopesHelper()
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
SELECT [ScopeID]
    ,[ReportID]
    ,[SurveyScope]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_SurveyScopes]
WHERE IsValid = 1
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ID
    /// </summary>
    /// <returns></returns>
    public static List<OSI_SurveyScopes> QueryByReportIDWithClass(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ScopeID]
    ,[ReportID]
    ,[SurveyScope]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_SurveyScopes]
WHERE IsValid = 1
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetList<OSI_SurveyScopes>();
    }

    /// <summary>
    /// 查詢 BY ReportID (DataTable)
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByReportID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ScopeID]
    ,[ReportID]
    ,[SurveyScope]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_SurveyScopes]
WHERE IsValid = 1
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }

}
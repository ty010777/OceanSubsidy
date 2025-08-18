using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSISurveyCountiesHelper 的摘要描述
/// </summary>
public class OSISurveyCountiesHelper
{
    public OSISurveyCountiesHelper()
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
SELECT [SurveyCountyID]
    ,[ReportID]
    ,[CountyID]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_SurveyCounties]
WHERE [IsValid] = 1
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 根據 ReportID 查詢相關的縣市資料（含縣市名稱）
    /// </summary>
    /// <param name="reportID">報告ID</param>
    /// <returns></returns>
    public static GisTable QueryByReportID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT sc.[SurveyCountyID]
    ,sc.[ReportID]
    ,sc.[CountyID]
    ,c.[c_name] as [CountyName]
    ,sc.[IsValid]
    ,sc.[CreatedAt]
FROM [OCA_OceanSubsidy].[dbo].[OSI_SurveyCounties] sc
INNER JOIN [OCA_OceanSubsidy].[dbo].[OSI_MapCounty] c ON sc.[CountyID] = c.[qgs_fid]
WHERE sc.[ReportID] = @ReportID
    AND sc.[IsValid] = 1
ORDER BY c.[qgs_fid]
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }

    /// <summary>
    /// 根據 ReportID 查詢並返回實體類別清單
    /// </summary>
    /// <param name="reportID">報告ID</param>
    /// <returns></returns>
    public static List<OSI_SurveyCounties> QueryByReportIDWithClass(string reportID)
    {
        List<OSI_SurveyCounties> list = new List<OSI_SurveyCounties>();
        GisTable table = QueryByReportID(reportID);

        foreach (DataRow row in table.Rows)
        {
            OSI_SurveyCounties item = new OSI_SurveyCounties();
            item.SurveyCountyID = row["SurveyCountyID"].ToString().toInt();
            item.ReportID = row["ReportID"].ToString().toInt();
            item.CountyID = row["CountyID"].ToString().toInt();
            item.IsValid = bool.Parse(row["IsValid"].ToString());
            item.CreatedAt = Convert.ToDateTime(row["CreatedAt"]);
            list.Add(item);
        }

        return list;
    }

    /// <summary>
    /// 批次新增縣市資料
    /// </summary>
    /// <param name="reportID">報告ID</param>
    /// <param name="countyList">縣市清單</param>
    /// <returns></returns>
    public static bool InsertBatch(int reportID, List<OSI_SurveyCounties> countyList)
    {
        if (countyList == null || countyList.Count == 0)
            return true;

        DbHelper db = new DbHelper();
        try
        {
            foreach (var county in countyList)
            {
                db.CommandText =
                    @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_SurveyCounties]
    ([ReportID]
    ,[CountyID]
    ,[IsValid]
    ,[CreatedAt])
VALUES
    (@ReportID
    ,@CountyID
    ,@IsValid
    ,GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@ReportID", reportID);
                db.Parameters.Add("@CountyID", county.CountyID);
                db.Parameters.Add("@IsValid", true);

                db.ExecuteNonQuery();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 根據 ReportID 和 CountyID 清單刪除資料（軟刪除）
    /// </summary>
    /// <param name="reportID">報告ID</param>
    /// <param name="surveyCountyIDs">要刪除的 SurveyCountyID 清單</param>
    /// <param name="deletedBy">刪除者</param>
    /// <returns></returns>
    public static bool DeleteBySurveyCountyIDs(int reportID, List<int> surveyCountyIDs, string deletedBy)
    {
        if (surveyCountyIDs == null || surveyCountyIDs.Count == 0)
            return true;

        DbHelper db = new DbHelper();
        try
        {
            string ids = string.Join(",", surveyCountyIDs);
            db.CommandText =
                @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_SurveyCounties]
SET [IsValid] = 0
    ,[DeletedAt] = GETDATE()
    ,[DeletedBy] = @DeletedBy
WHERE [ReportID] = @ReportID
    AND [SurveyCountyID] IN (" + ids + @")
";
            db.Parameters.Clear();
            db.Parameters.Add("@ReportID", reportID);
            db.Parameters.Add("@DeletedBy", deletedBy);

            db.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 判斷該名稱是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsExistByCountyName(string countyName)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 1
FROM [OCA_OceanSubsidy].[dbo].[OSI_MapCounty]
WHERE c_name = @CountyName
";
        db.Parameters.Clear();
        db.Parameters.Add("@CountyName", countyName);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            rtVal = true;
        }

        return rtVal;
    }

    /// <summary>
    /// 查詢ID By Name
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public static int QueryIDByName(string name)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [qgs_fid] as [CountyID]
FROM [OCA_OceanSubsidy].[dbo].[OSI_MapCounty]
WHERE c_name = @CountyName
";
        db.Parameters.Clear();
        db.Parameters.Add("@CountyName", name);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["CountyID"]);
        }

        return 0;
    }

}
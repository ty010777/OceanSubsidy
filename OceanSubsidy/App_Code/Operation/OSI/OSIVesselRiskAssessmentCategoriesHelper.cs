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
/// OSIVesselRiskAssessmentCategoriesHelper 的摘要描述
/// </summary>
public class OSIVesselRiskAssessmentCategoriesHelper
{
    public OSIVesselRiskAssessmentCategoriesHelper()
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
SELECT [Id]
    ,[AssessmentId]
    ,[RiskCategoryId]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By ID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByID(string id)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [Id]
    ,[AssessmentId]
    ,[RiskCategoryId]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
WHERE Id = @Id
";
        db.Parameters.Clear();
        db.Parameters.Add("@Id", id);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By AssessmentId 並取得類別名稱
    /// </summary>
    /// <param name="assessmentId">AssessmentId</param>
    /// <returns>類別名稱字串（多個以頓號分隔）</returns>
    public static string GetCategoryNamesByAssessmentId(int assessmentId)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT c.CategoryName
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories] ac
INNER JOIN [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskCategory] c 
    ON ac.RiskCategoryId = c.RiskCategoryId
WHERE ac.AssessmentId = @AssessmentId
ORDER BY c.RiskCategoryId
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);

        var tbl = db.GetTable();
        
        if (tbl == null || tbl.Rows.Count == 0)
            return "無";
            
        var categoryNames = new List<string>();
        foreach (DataRow row in tbl.Rows)
        {
            if (row["CategoryName"] != DBNull.Value)
            {
                categoryNames.Add(row["CategoryName"].ToString());
            }
        }
        
        return categoryNames.Count > 0 ? string.Join("、", categoryNames) : "無";
    }

    /// <summary>
    /// 查詢 By AssessmentId
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <returns></returns>
    public static GisTable QueryByAssessmentId(int assessmentId)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [Id]
    ,[AssessmentId]
    ,[RiskCategoryId]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
WHERE AssessmentId = @AssessmentId
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);

        return db.GetTable();
    }

    /// <summary>
    /// 刪除特定 AssessmentId 的所有關聯
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <returns></returns>
    public static bool DeleteByAssessmentId(int assessmentId)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
DELETE FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
WHERE AssessmentId = @AssessmentId
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 新增關聯
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <param name="riskCategoryId"></param>
    /// <returns></returns>
    public static bool Insert(int assessmentId, int riskCategoryId)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
    ([AssessmentId], [RiskCategoryId])
VALUES
    (@AssessmentId, @RiskCategoryId)
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);
        db.Parameters.Add("@RiskCategoryId", riskCategoryId);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

}
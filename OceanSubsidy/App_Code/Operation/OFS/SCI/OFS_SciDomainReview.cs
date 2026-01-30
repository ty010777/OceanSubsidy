using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// 實質審查相關資料操作 Helper
/// </summary>
public class OFS_SciDomainReviewHelper
{
    public OFS_SciDomainReviewHelper()
    {
        
    }

    /// <summary>
    /// 根據 ProjectID 取得計畫基本資料（支援科專與文化補助案）
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>計畫資料 DataRow，如果找不到則回傳 null</returns>
    public static DataRow GetProjectData(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
            return null;

        DbHelper db = new DbHelper();

        if (projectID.Contains("SCI"))
        {
            // 科專資料查詢
            db.CommandText = @"SELECT TOP (1)
                AM.[ProjectID],
                [Year],
                [SubsidyPlanType],
                [ProjectNameTw] as ProjectName,
                 PM.Statuses as Status,
                (SELECT Descname
                 FROM Sys_ZgsCode
                 WHERE Code = AM.[Topic]
                ) as Topic,
                [OrgName],
                '科專' as ProjectCategory
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] AM
			LEFT JOIN OFS_SCI_Project_Main PM ON  AM.ProjectID = PM.ProjectID 
            WHERE AM.ProjectID = @ProjectID";
        }
        else if (projectID.Contains("CUL"))
        {
            // 文化補助案資料查詢
            db.CommandText = @"SELECT TOP (1)
                [ProjectID],
                [Year],
                [SubsidyPlanType],
                [ProjectName],
                [Status],
                (SELECT Descname
                 FROM Sys_ZgsCode
                 WHERE Code = CP.[Field] and CodeGroup = 'CULField'
                ) as Field,
                [OrgName],
                [UpdateTime] as updated_at,
                '文化' as ProjectCategory
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CUL_Project] CP
            WHERE ProjectID = @ProjectID";
        }
        else
        {
            return null;
        }

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        try
        {
            DataTable dt = db.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢計畫資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

   


    /// <summary>
    /// 根據 Token 取得審查記錄和評分資料（多筆）
    /// </summary>
    /// <param name="token">審查Token</param>
    /// <returns>審查記錄資料表</returns>
    public static DataTable GetReviewDataByToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        DbHelper db = new DbHelper();
        db.CommandText = @"SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords]
        LEFT JOIN OFS_ReviewScores on
        [OFS_ReviewRecords].ReviewID = OFS_ReviewScores.ReviewID
        WHERE Token = @Token";

        db.Parameters.Clear();
        db.Parameters.Add("@Token", token);

        try
        {
            DataTable dt = db.GetTable();
            return dt;
        }
        catch (Exception ex)
        {
            throw new Exception($"根據Token查詢審查資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    



    /// <summary>
    /// 更新評審項目評分和總分
    /// </summary>
    /// <param name="reviewID">審查記錄ID</param>
    /// <param name="itemScores">評審項目評分列表 (ItemId, Score)</param>
    /// <param name="reviewComment">審查意見</param>
    /// <param name="isSubmit">是否提送</param>
    /// <returns>是否更新成功</returns>
    public static bool UpdateReviewScores(string reviewID, Dictionary<string, decimal> itemScores, string reviewComment, bool isSubmit)
    {
        if (string.IsNullOrEmpty(reviewID) || itemScores == null)
            return false;

        DbHelper db = new DbHelper();
        
        try
        {
            decimal totalScore = 0;

            // 1. 更新各評審項目評分
            foreach (var item in itemScores)
            {
                string itemId = item.Key;
                decimal score = item.Value;

                // 更新 OFS_ReviewScores 的 Score
                db.CommandText = @"UPDATE OFS_ReviewScores 
                                 SET Score = @Score
                                 WHERE Id = @ItemId AND ReviewID = @ReviewID";

                db.Parameters.Clear();
                db.Parameters.Add("@Score", score);
                db.Parameters.Add("@ItemId", itemId);
                db.Parameters.Add("@ReviewID", reviewID);

                db.ExecuteNonQuery();

                // 取得該項目的權重來計算總分
                db.CommandText = @"SELECT Weight FROM OFS_ReviewScores WHERE Id = @ItemId";
                db.Parameters.Clear();
                db.Parameters.Add("@ItemId", itemId);

                DataTable weightTable = db.GetTable();
                if (weightTable != null && weightTable.Rows.Count > 0)
                {
                    object weightObj = weightTable.Rows[0]["Weight"];
                    if (weightObj != null && decimal.TryParse(weightObj.ToString(), out decimal weight))
                    {
                        totalScore += (score * weight / 100); // 權重是百分比，需要除以100
                    }
                }
            }

            // 2. 更新 OFS_ReviewRecords 的總分、審查意見和提送狀態
            db.CommandText = @"UPDATE OFS_ReviewRecords 
                             SET TotalScore = @TotalScore, 
                                 ReviewComment = @ReviewComment,
                                 IsSubmit = @IsSubmit,
                                 updated_at = GETDATE()
                             WHERE ReviewID = @ReviewID";

            db.Parameters.Clear();
            db.Parameters.Add("@TotalScore", totalScore);
            db.Parameters.Add("@ReviewComment", reviewComment ?? "");
            db.Parameters.Add("@IsSubmit", isSubmit ? 1 : 0);
            db.Parameters.Add("@ReviewID", reviewID);

            db.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"更新評審資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }
    public static bool IsReviewSubmitted(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        DbHelper db = new DbHelper();
        db.CommandText = "SELECT ISNULL(isSubmit, 0) FROM OFS_ReviewRecords WHERE Token = @token";
        db.Parameters.Clear();
        db.Parameters.Add("@token", token);
            
        try
        {
            DataTable dt = db.GetTable();
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]) == 1;
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查審查提交狀態時發生錯誤: {ex.Message}");
            return false;
        }
        finally
        {
            db.Dispose();
        }
    }
}
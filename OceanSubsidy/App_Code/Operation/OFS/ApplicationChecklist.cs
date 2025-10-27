using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Configuration;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class ApplicationChecklistHelper
{
    public ApplicationChecklistHelper()
    {
       
    }
    public static List<ReviewChecklistItem> GetLatestApplicationChecklist()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
    SELECT TOP (1000) [ProjectID]
      ,[Statuses]
      ,[StatusesName]
      ,[ExpirationDate]
      ,[SupervisoryUnit]
      ,[SupervisoryPersonName]
      ,[SupervisoryPersonAccount]
      ,[UserAccount]
      ,[UserOrg]
      ,[UserName]
      ,[isWithdrawal]
      ,[isExist]
      ,[SubsidyPlanType]
      ,[ProjectNameTw]
      ,[OrgName]
      ,[Year]
      ,[TotalSubsidyAmount]
  FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ApplicationChecklistSearch]

";

        try
        {
            db.CommandText += "WHERE isExist = 1 ";
            DataTable dt = db.GetTable();
            List<ReviewChecklistItem> resultList = new List<ReviewChecklistItem>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    // 基本欄位
                    ProjectID = row["ProjectID"]?.ToString(),
                    // 申請計畫列表主要欄位
                    Year = row["Year"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    Req_SubsidyAmount = row["TotalSubsidyAmount"] != DBNull.Value ?
                        Convert.ToDecimal(row["TotalSubsidyAmount"]).ToString("N0") : "0",

                    // 狀態相關
                    Statuses = row["Statuses"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,

                    // 審查相關
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    UserAccount = row["UserAccount"]?.ToString(),
                    UserOrg = row["UserOrg"]?.ToString(),
                    UserName = row["UserName"]?.ToString(),
                    SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),

                    // 操作狀態相關
                    isWithdrawal = row["isWithdrawal"] != DBNull.Value ? (bool?)row["isWithdrawal"] : false,
                    isExists = row["isExist"] != DBNull.Value ? (bool?)row["isExist"] : true
                };

                resultList.Add(item);
            }

            return resultList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢版本清單時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得可申請的計畫類別清單
    /// </summary>
    /// <returns>計畫類別清單</returns>
    public static List<ListItem> GetAvailableGrantTypes()
    {
        List<ListItem> result = new List<ListItem>();
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT *
                FROM OFS_GrantType
                WHERE GETDATE() BETWEEN ApplyStartDate AND ApplyEndDate
                ORDER BY TypeID;";

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                string grantTypeId = row["TypeCode"]?.ToString();
                string fullName = row["FullName"]?.ToString();

                if (!string.IsNullOrEmpty(grantTypeId) && !string.IsNullOrEmpty(fullName))
                {
                    result.Add(new ListItem(fullName, grantTypeId));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得計畫類別清單時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    /// <summary>
    /// 根據計畫類別ID取得計畫詳細資訊
    /// </summary>
    /// <param name="grantTypeId">計畫類別ID</param>
    /// <returns>計畫資訊</returns>
    public static GrantTypeInfo GetGrantTypeInfo(string grantTypeId)
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT
                    GrantTypeID,
                    FullName,
                    ShortName,
                    Description,
                    IsActive,
                    StartDate,
                    EndDate
                FROM OFS_GrantType
                WHERE GrantTypeID = @GrantTypeID AND IsActive = 1";

            db.Parameters.Add("@GrantTypeID", grantTypeId);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new GrantTypeInfo
                {
                    GrantTypeID = row["GrantTypeID"]?.ToString(),
                    FullName = row["FullName"]?.ToString(),
                    ShortName = row["ShortName"]?.ToString(),
                    Description = row["Description"]?.ToString(),
                    IsActive = row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : false,
                    StartDate = row["StartDate"] != DBNull.Value ? (DateTime?)row["StartDate"] : null,
                    EndDate = row["EndDate"] != DBNull.Value ? (DateTime?)row["EndDate"] : null
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得計畫類別資訊時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return null;
    }

    /// <summary>
    /// 驗證使用者是否可以申請指定的計畫類別
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="grantTypeId">計畫類別ID</param>
    /// <returns>是否可以申請</returns>
    public static bool CanUserApplyGrantType(string userId, string grantTypeId)
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT COUNT(1)
                FROM OFS_GrantType gt
                WHERE gt.GrantTypeID = @GrantTypeID
                  AND gt.IsActive = 1
                  AND (gt.StartDate IS NULL OR gt.StartDate <= GETDATE())
                  AND (gt.EndDate IS NULL OR gt.EndDate >= GETDATE())";

            db.Parameters.Add("@GrantTypeID", grantTypeId);

            object result = db.GetTable();
            int count = result != null ? Convert.ToInt32(result) : 0;
            return count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"驗證申請權限時發生錯誤：{ex.Message}");
            return false;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新計畫的撤案狀態
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="isWithdrawal">是否撤案</param>
    /// <param name="reason">撤案原因</param>
    /// <returns>更新是否成功</returns>
    public static void UpdateWithdrawalStatus(string projectId, bool isWithdrawal, string reason = "")
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET isWithdrawal = @IsWithdrawal,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectId";

            db.Parameters.Add("@IsWithdrawal", isWithdrawal);
            db.Parameters.Add("@ProjectId", projectId);

           db.ExecuteNonQuery();



        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新撤案狀態時發生錯誤：{ex.Message}");

        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新計畫的撤案狀態
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="isWithdrawal">是否撤案</param>
    /// <param name="reason">撤案原因</param>
    /// <returns>更新是否成功</returns>
    public static void CLB_UpdateWithdrawalStatus(string projectId, bool isWithdrawal, string reason = "")
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                UPDATE OFS_CLB_Project_Main
                SET isWithdrawal = @IsWithdrawal,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectId";

            db.Parameters.Add("@IsWithdrawal", isWithdrawal);
            db.Parameters.Add("@ProjectId", projectId);

            db.ExecuteNonQuery();



        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新撤案狀態時發生錯誤：{ex.Message}");

        }
        finally
        {
            db.Dispose();
        }
    }
    /// <summary>
    /// 更新計畫的存在狀態（軟刪除）
    /// </summary>
    /// <param name="projectId">版本ID</param>
    /// <param name="isExists">是否存在</param>
    /// <param name="reason">刪除原因</param>
    /// <returns>更新是否成功</returns>
    public static void UpdateExistsStatus(string projectId, bool isExists)
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET isExist = @IsExists,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectId";

            db.Parameters.Add("@IsExists", isExists);
            db.Parameters.Add("@ProjectId", projectId);

             db.ExecuteNonQuery();



        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新存在狀態時發生錯誤：{ex.Message}");

        }
        finally
        {
            db.Dispose();
        }
    }
    public static void CLB_UpdateExistsStatus(string projectId, bool isExists, string reason = "")
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                UPDATE OFS_CLB_Project_Main
                SET isExist = @IsExists,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectId";

            db.Parameters.Add("@IsExists", isExists);
            db.Parameters.Add("@ProjectId", projectId);

            db.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新存在狀態時發生錯誤：{ex.Message}");

        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查是否有需要回覆的審查記錄
    /// </summary>
    /// <param name="projectId">版本ID</param>
    /// <returns>是否需要回覆</returns>
    public static bool HasPendingReply(string projectId)
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT COUNT(1)
                FROM OFS_ReviewRecords
                WHERE ProjectID = @ProjectId
                  AND (ReplyComment IS NULL OR ReplyComment = '')";

            db.Parameters.Add("@ProjectId", projectId);

            object result = db.GetTable();
            int count = result != null ? Convert.ToInt32(result) : 0;
            return count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查回覆狀態時發生錯誤：{ex.Message}");
            return false;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 新增案件歷程記錄到資料庫
    /// </summary>
    /// <param name="caseHistoryLog">案件歷程記錄物件</param>
    /// <returns>是否成功</returns>
    public static bool InsertCaseHistoryLog(OFS_CaseHistoryLog caseHistoryLog)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();

        db.BeginTrans();

        try
        {
            db.CommandText = @"
        INSERT INTO [dbo].[OFS_CaseHistoryLog]
    ([ProjectID],[ChangeTime],[UserName],[StageStatusBefore],[StageStatusAfter],[Description])
        VALUES
    (@ProjectID,@ChangeTime,@UserName,@StageStatusBefore,@StageStatusAfter,@Description)";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", caseHistoryLog.ProjectID);
            db.Parameters.Add("@ChangeTime", caseHistoryLog.ChangeTime);
            db.Parameters.Add("@UserName", caseHistoryLog.UserName);
            db.Parameters.Add("@StageStatusBefore", caseHistoryLog.StageStatusBefore);
            db.Parameters.Add("@StageStatusAfter", caseHistoryLog.StageStatusAfter);
            db.Parameters.Add("@Description", caseHistoryLog.Description);

            GisTable Dt1 = db.GetTable();
            db.Commit();
        }
        catch (Exception ex)
        {
            db.Rollback();
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 取得指定 ProjectID 的最新歷史記錄狀態
    /// </summary>
    /// <param name="projectId">計畫 ID</param>
    /// <returns>最新的 StageStatusAfter，若無記錄則返回 null</returns>
    public static string GetLatestStageStatus(string projectId)
    {
        string result = null;
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT TOP 1 StageStatusAfter
                FROM OFS_CaseHistoryLog
                WHERE ProjectID = @ProjectID
                ORDER BY ChangeTime DESC, Id DESC";

            db.Parameters.Add("@ProjectID", projectId);

            var dt = db.GetTable();
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["StageStatusAfter"]?.ToString();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得最新歷史狀態時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    /// <summary>
    /// 取得指定 ProjectID 的案件歷程記錄
    /// </summary>
    /// <param name="projectId">計畫 ID</param>
    /// <returns>案件歷程記錄清單</returns>
    public static List<OFS_CaseHistoryLog> GetCaseHistoryByProjectId(string projectId)
    {
        List<OFS_CaseHistoryLog> result = new List<OFS_CaseHistoryLog>();
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT [Id], [ProjectID], [ChangeTime], [UserName],
                       [StageStatusBefore], [StageStatusAfter], [Description]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CaseHistoryLog]
                WHERE [ProjectID] = @ProjectID
                ORDER BY [ProjectID], [ChangeTime] ASC";

            db.Parameters.Add("@ProjectID", projectId);

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                var historyLog = new OFS_CaseHistoryLog
                {
                    Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    ChangeTime = row["ChangeTime"] != DBNull.Value ? Convert.ToDateTime(row["ChangeTime"]) : DateTime.MinValue,
                    UserName = row["UserName"]?.ToString(),
                    StageStatusBefore = row["StageStatusBefore"]?.ToString(),
                    StageStatusAfter = row["StageStatusAfter"]?.ToString(),
                    Description = row["Description"]?.ToString()
                };

                result.Add(historyLog);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得案件歷程時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    /// <summary>
    /// 取得計畫資料用於審查意見回覆 (參考 SciDomainReview 的 GetProjectData)
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>計畫資料</returns>
    public static ProjectDataForReview GetProjectDataForReview(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            return null;

        DbHelper db = new DbHelper();

        try
        {
            // 根據 ProjectID 判斷計畫類型並查詢對應的表格
            if (projectId.Contains("SCI"))
            {
                // 科專計畫
                db.CommandText = @"SELECT TOP (1)
                    AM.[ProjectID],
                    [Year],
                    [SubsidyPlanType],
                    [ProjectNameTw] as ProjectName,
					AM.OrgName,
                    UserName,
					(SELECT Descname
                     FROM Sys_ZgsCode
                     WHERE Code = AM.[Field] and CodeGroup = 'SCIField'
                    ) as ReviewGroup,
                    [OrgName] as ApplicantUnit
                    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] AM
				LEFT JOIN OFS_SCI_Project_Main PM ON AM.ProjectID = PM.ProjectID
                WHERE AM.ProjectID = @ProjectID";
            }
            else if (projectId.Contains("CUL"))
            {
                db.CommandText = @"SELECT TOP (1)
                    [ProjectID],
                    [Year],
                    [SubsidyPlanType],
                    ProjectName,
                    (SELECT TOP(1)Descname
                        FROM Sys_ZgsCode
                        WHERE Code = CP.Field and CodeGroup = 'CULField'
                    ) as ReviewGroup,
                    [OrgName] as ApplicantUnit,
                    UserName
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CUL_Project] CP
	            where ProjectID = @ProjectID
	";
                
            }
            else
            {
                return null;
            }

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            DataTable dt = db.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new ProjectDataForReview
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    ProjectName = row["ProjectName"]?.ToString(),
                    ReviewGroup = row["ReviewGroup"]?.ToString(),
                    ApplicantUnit = row["ApplicantUnit"]?.ToString(),
                    UserName = row["UserName"]?.ToString()
                };
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
    /// 取得當前的審查階段
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>審查階段</returns>
    public static string GetCurrentReviewStage(string projectId)
    {
        DbHelper db = new DbHelper();

        try
        {
            // 根據 ProjectID 判斷計畫類型並查詢對應的表格
            if (projectId.Contains("SCI"))
            {
                db.CommandText = @"SELECT [Statuses]
                                  FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                                  WHERE ProjectID = @ProjectID";
            }
            else if (projectId.Contains("CUL"))
            {
                // TODO: 文化計畫審查階段查詢的 SQL
                // TODO: 查詢文化計畫的 Project_Main 表
                return "領域審查"; // 預設
            }
            else
            {
                return "領域審查"; // 預設
            }

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            DataTable dt = db.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                string statuses = dt.Rows[0]["Statuses"]?.ToString();

                // 根據 Statuses 決定 ReviewStage
                if (!string.IsNullOrEmpty(statuses))
                {
                    if (statuses.Contains("領域審查")) return "領域審查";
                    if (statuses.Contains("技術審查")) return "技術審查";
                }
            }

            return "領域審查"; // 預設
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查階段時發生錯誤：{ex.Message}");
            return "領域審查"; // 預設
        }
        finally
        {
            db.Dispose();
        }
    }

    public static void UpdateReplyContent(string ReviewID, string ReplyComment)
    {
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                update [OFS_ReviewRecords]
                    set ReplyComment = @ReplyComment,
                    updated_at = GETDATE()
                    WHERE ReviewID  = @ReviewID
            ";
            db.Parameters.Add("@ReviewID", ReviewID);
            db.Parameters.Add("@ReplyComment", ReplyComment);

            db.ExecuteNonQuery();


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新使用者回覆時發生錯誤：{ex.Message}");

        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 根據關鍵字搜尋專案ID
    /// </summary>
    /// <param name="keyword">關鍵字</param>
    /// <returns>符合條件的專案ID清單</returns>
    public static List<string> SearchProjectIDsByKeyword(string keyword)
    {
        List<string> projectIDs = new List<string>();

        if (string.IsNullOrWhiteSpace(keyword))
            return projectIDs;

        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT DISTINCT KeywordID
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_KeyWord]
                WHERE KeyWordTw LIKE @Keyword
                   OR KeyWordEn LIKE @Keyword";

            db.Parameters.Add("@Keyword", "%" + keyword.Trim() + "%");

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                string keywordID = row["KeywordID"]?.ToString();
                if (!string.IsNullOrEmpty(keywordID))
                {
                    projectIDs.Add(keywordID);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"搜尋關鍵字時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return projectIDs;
    }

    /// <summary>
    /// 取得需要回覆的 ProjectID 清單
    /// 條件：OFS_ReviewRecords.isSubmit = 1 但 ReplyComment 是 null 或空值
    /// </summary>
    /// <returns>需要回覆的 ProjectID 清單</returns>
    public static List<string> GetWaitingReplyProjectIds()
    {
        List<string> projectIds = new List<string>();
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT DISTINCT ProjectID
                FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords]
                WHERE isSubmit = 1
                  AND (ReplyComment IS NULL OR ReplyComment = '' OR LTRIM(RTRIM(ReplyComment)) = '')
                  AND ProjectID IS NOT NULL";

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                string projectId = row["ProjectID"]?.ToString();
                if (!string.IsNullOrEmpty(projectId))
                {
                    projectIds.Add(projectId);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得待回覆 ProjectID 清單時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return projectIds;
    }
}

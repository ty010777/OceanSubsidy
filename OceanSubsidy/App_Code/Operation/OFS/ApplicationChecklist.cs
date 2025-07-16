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
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }
    public static List<ReviewChecklistItem> GetLatestApplicationChecklist()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        WITH SubsidySummary AS (
    SELECT 
        ProjectID, 
        SUM(SubsidyAmount) AS TotalSubsidyAmount
    FROM 
        [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_TotalFee]
    GROUP BY 
        ProjectID
)
        SELECT 
            v.ProjectID,
            v.Statuses,
            v.StatusesName,
            v.ExpirationDate,
            v.SupervisoryUnit,	
            v.SupervisoryPersonName,
            v.SupervisoryPersonAccount,
            v.UserAccount,
            v.UserOrg,
            v.UserName,
            v.isWithdrawal,
            v.isExist,
            m.SubsidyPlanType,
            m.ProjectNameTw,
            m.OrgName,
            m.Year,
            ISNULL(s.TotalSubsidyAmount, 0) AS TotalSubsidyAmount
        FROM OFS_SCI_Project_Main v
        LEFT JOIN OFS_SCI_Application_Main m ON v.ProjectID = m.ProjectID
		LEFT JOIN SubsidySummary s on v.ProjectID = s.ProjectID
        WHERE (v.isExist = 1)
        ORDER BY v.ProjectID DESC
";

        try
        {
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
                    ApplicationAmount = row["TotalSubsidyAmount"] != DBNull.Value ? 
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
                SELECT*
                FROM OFS_GrantType 
                ORDER BY TypeID";
            
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
            
            // 如果有提供撤案原因，記錄到歷程
            if (isWithdrawal)
            {
                LogCaseHistory(projectId, "撤案", reason);
            }
            else if (!isWithdrawal)
            {
                LogCaseHistory(projectId, "恢復案件", "恢復已撤案的申請案件");
            }
            
            
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
    public static void UpdateExistsStatus(string projectId, bool isExists, string reason = "")
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
            
            // 記錄到歷程
            if (!string.IsNullOrEmpty(reason) && !isExists)
            {
                LogCaseHistory(projectId, "刪除案件", reason);
            }
            
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
    /// 記錄案件歷程
    /// </summary>
    /// <param name="projectId">版本ID</param>
    /// <param name="action">動作</param>
    /// <param name="description">說明</param>
    private static void LogCaseHistory(string projectId, string action, string description)
    {
        DbHelper db = new DbHelper();
        
        try
        {
            db.CommandText = @"
                INSERT INTO OFS_CaseHistoryLog 
                (ProjectID, Action, Description, CreatedBy, CreatedAt)
                VALUES 
                (@ProjectId, @Action, @Description, @CreatedBy, GETDATE())";
            
            db.Parameters.Add("@ProjectId", projectId);
            db.Parameters.Add("@Action", action);
            db.Parameters.Add("@Description", description);
            db.Parameters.Add("@CreatedBy", HttpContext.Current?.Session["UserAccount"]?.ToString() ?? "System");
            
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"記錄案件歷程時發生錯誤：{ex.Message}");
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
}

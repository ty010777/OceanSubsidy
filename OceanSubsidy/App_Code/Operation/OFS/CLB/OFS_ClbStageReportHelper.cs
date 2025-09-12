using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.App;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>1
/// CLB 階段報告相關的 Helper 類別
/// </summary>
public class OFS_ClbStageReportHelper
{
    public OFS_ClbStageReportHelper()
    {
    }

    #region 階段審查相關方法

    /// <summary>
    /// 儲存階段審查資料 (暫存或提送)
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="isDraft">是否為暫存</param>
    public static void SaveStageExamData(string projectID, bool isDraft)
    {
        try
        {
            // 先刪除現有記錄，再新增
            DeleteStageExamRecord(projectID);
            
            // 新增新的記錄
            InsertStageExamRecord(projectID, isDraft);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存階段審查資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 刪除指定計畫的 StageExam 記錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private static void DeleteStageExamRecord(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_StageExam] 
            WHERE [ProjectID] = @ProjectID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        
        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 新增 StageExam 記錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="isDraft">是否為暫存</param>
    private static void InsertStageExamRecord(string projectID, bool isDraft)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_StageExam]
            (
                [ProjectID],
                [Status],
                [Reviewer],
                [Account],
                [create_at],
                [update_at]
            )
            VALUES
            (
                @ProjectID,
                @Status,
                @Reviewer,
                @Account,
                GETDATE(),
                GETDATE()
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Status", isDraft ? "暫存" : "審核中");
        db.Parameters.Add("@Reviewer", DBNull.Value); // 設為 null
        db.Parameters.Add("@Account", DBNull.Value);   // 設為 null

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 取得階段審查資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>階段審查資料物件</returns>
    public static OFS_CLB_StageExam GetStageExamData(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_StageExam] 
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_StageExam
                {
                    id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    Status = row["Status"]?.ToString(),
                    Reviewer = row["Reviewer"]?.ToString(),
                    Account = row["Account"]?.ToString(),
                    create_at = row["create_at"] != DBNull.Value ? Convert.ToDateTime(row["create_at"]) : (DateTime?)null,
                    update_at = row["update_at"] != DBNull.Value ? Convert.ToDateTime(row["update_at"]) : (DateTime?)null
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得階段審查資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 檢查階段審查資料是否存在
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>是否存在</returns>
    public static bool CheckStageExamDataExists(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT COUNT(*) 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_StageExam] 
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataSet ds = db.GetDataSet();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0][0];
                if (result != null && result != DBNull.Value)
                {
                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查階段審查資料是否存在失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新審查結果
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="status">審查狀態</param>
    /// <param name="reviewer">審查委員</param>
    /// <param name="account">審查帳號</param>
    public static void UpdateStageExamResult(string projectID, string status, string reviewer = null, string account = null)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_StageExam] 
                SET [Status] = @Status,
                    [Reviewer] = @Reviewer,
                    [Account] = @Account,
                    [update_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@Status", status ?? "");
            db.Parameters.Add("@Reviewer", string.IsNullOrEmpty(reviewer) ? DBNull.Value : (object)reviewer);
            db.Parameters.Add("@Account", string.IsNullOrEmpty(account) ? DBNull.Value : (object)account);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新審查結果失敗：{ex.Message}");
        }
    }

    #endregion

    #region 檔案上傳相關方法

    /// <summary>
    /// 取得指定計畫和檔案代碼的上傳檔案
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <returns>上傳檔案物件</returns>
    public static OFS_CLB_UploadFile GetUploadedFile(string projectID, string fileCode)
    {
        try
        {
            return OFS_ClbApplicationHelper.GetUploadedFile(projectID, fileCode);
        }
        catch (Exception ex)
        {
            throw new Exception($"取得上傳檔案失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 取得指定計畫的所有上傳檔案
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>上傳檔案清單</returns>
    public static List<OFS_CLB_UploadFile> GetUploadedFiles(string projectID)
    {
        try
        {
            return OFS_ClbApplicationHelper.GetUploadedFiles(projectID);
        }
        catch (Exception ex)
        {
            throw new Exception($"取得上傳檔案清單失敗：{ex.Message}");
        }
    }

    #endregion

    #region 計畫資料相關方法

    /// <summary>
    /// 取得計畫基本資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>基本資料物件</returns>
    public static OFS_CLB_Application_Basic GetProjectBasicData(string projectID)
    {
        try
        {
            return OFS_ClbApplicationHelper.GetBasicData(projectID);
        }
        catch (Exception ex)
        {
            throw new Exception($"取得計畫基本資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 取得計畫主要資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>主要資料物件</returns>
    public static OFS_CLB_Project_Main GetProjectMainData(string projectID)
    {
        try
        {
            return OFS_ClbApplicationHelper.GetProjectMainData(projectID);
        }
        catch (Exception ex)
        {
            throw new Exception($"取得計畫主要資料失敗：{ex.Message}");
        }
    }

    #endregion

    #region 使用者權限檢查

    /// <summary>
    /// 檢查使用者是否為審核人員（主管單位人員、主管單位窗口、系統管理者）
    /// </summary>
    /// <returns>是否為審核人員</returns>
    public static bool IsReviewUser(SessionHelper.UserInfoClass userInfo)
    {
        try
        {
            if (userInfo == null || userInfo.OFS_RoleName == null)
                return false;

            // 檢查是否為審核人員角色
            string[] reviewRoles = { "主管單位人員", "主管單位窗口", "系統管理者" };
            
            // 檢查使用者的任一角色是否為審核人員角色
            return userInfo.OFS_RoleName.Any(userRole => reviewRoles.Contains(userRole));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查使用者權限時發生錯誤: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 取得當前使用者資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
    public static SessionHelper.UserInfoClass GetCurrentUserInfo()
    {    
        try
        {
            var userInfo = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            return userInfo;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得使用者資訊時發生錯誤: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region 階段審查狀態管理

    /// <summary>
    /// 取得階段審查狀態資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>階段審查狀態資訊</returns>
    public static (string Status, bool CanEdit, bool CanReview, bool ShowReviewPanel) GetStageExamStatus(string projectID)
    {
        try
        {
            var stageExam = GetStageExamData(projectID);
            var userInfo = GetCurrentUserInfo();
            
            string status = stageExam?.Status ?? ""; // null 或空值視為初始狀態
            bool isReviewUser = IsReviewUser(userInfo);
            
            bool canEdit = false;
            bool canReview = false;
            bool showReviewPanel = false;

            switch (status)
            {
                case "通過":
                    // 已通過：所有功能都隱藏
                    canEdit = false;
                    canReview = false;
                    showReviewPanel = false;
                    break;
                    
                case "審核中":
                    // 審核中：一般使用者不能編輯，審核人員可以審核
                    canEdit = false;
                    canReview = isReviewUser;
                    showReviewPanel = isReviewUser;
                    break;
                    
                case "暫存":
                case "":
                case null:
                default:
                    // 暫存或初始狀態：可以編輯，不顯示審核面板
                    canEdit = true;
                    canReview = false;
                    showReviewPanel = false;
                    break;
            }

            return (status, canEdit, canReview, showReviewPanel);
        }
        catch (Exception ex)
        {
            throw new Exception($"取得階段審查狀態失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 處理審查結果提交
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="reviewResult">審查結果 (pass/reject)</param>
    /// <param name="reviewComment">審查意見</param>
    /// <returns>處理結果</returns>
    public static bool SubmitReviewResult(string projectID, string reviewResult, string reviewComment = "")
    {
        try
        {
            var userInfo = GetCurrentUserInfo();
            
            if (string.IsNullOrEmpty(userInfo.Account))
            {
                throw new Exception("無法取得使用者資訊");
            }

            // 檢查使用者權限
            if (!IsReviewUser(userInfo))
            {
                throw new Exception("權限不足，無法進行審核");
            }

            // 根據審查結果設定狀態
            string newStatus = reviewResult == "pass" ? "通過" : "暫存";
            
            // 更新階段審查記錄
            UpdateStageExamResult(projectID, newStatus, userInfo.UserName, userInfo.Account);
            
            // TODO: 寄送通知信件
            // SendNotificationEmail(projectID, newStatus, reviewComment);
            
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"提交審查結果失敗：{ex.Message}");
        }
    }

    #endregion
}
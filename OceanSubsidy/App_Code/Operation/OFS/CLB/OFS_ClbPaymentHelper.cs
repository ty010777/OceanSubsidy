using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using GS.App;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// CLB 請款核銷相關的 Helper 類別
/// </summary>
public class OFS_ClbPaymentHelper
{
    public OFS_ClbPaymentHelper()
    {
    }

    #region 檔案上傳相關方法

    /// <summary>
    /// 上傳檔案
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <param name="originalFileName">原始檔案名稱</param>
    /// <param name="fileData">檔案資料 (Base64)</param>
    /// <returns>上傳結果</returns>
    public static (bool success, string message, string savedFileName) UploadFile(string projectID, string fileCode, string originalFileName, string fileData)
    {
        try
        {
            // 驗證輸入參數
            if (string.IsNullOrEmpty(projectID))
            {
                return (false, "計畫編號不能為空", "");
            }

            if (string.IsNullOrEmpty(fileCode))
            {
                return (false, "檔案代碼不能為空", "");
            }

            if (string.IsNullOrEmpty(originalFileName))
            {
                return (false, "檔案名稱不能為空", "");
            }

            if (string.IsNullOrEmpty(fileData))
            {
                return (false, "檔案資料不能為空", "");
            }

            // 解碼 Base64 檔案資料
            byte[] fileBytes;
            try
            {
                // 移除 data URL 前綴（如果有的話）
                if (fileData.Contains(","))
                {
                    fileData = fileData.Split(',')[1];
                }
                fileBytes = Convert.FromBase64String(fileData);
            }
            catch
            {
                return (false, "檔案資料格式錯誤", "");
            }

            // 檢查檔案大小 (100MB 限制)
            if (fileBytes.Length > 100 * 1024 * 1024)
            {
                return (false, "檔案大小不能超過 100MB", "");
            }

            // 取得檔案類型對應的附件名稱
            string attachmentName = GetAttachmentNameByFileCode(fileCode);
            if (string.IsNullOrEmpty(attachmentName))
            {
                return (false, "無效的檔案代碼", "");
            }

            // 建立新的檔案名稱：{ProjectID}_{附件名稱}.pdf
            string fileExtension = Path.GetExtension(originalFileName);
            string newFileName = $"{projectID}_{attachmentName}{fileExtension}";

            // 建立上傳目錄
            string uploadPath = HttpContext.Current.Server.MapPath($"~/UploadFiles/OFS/CLB/{projectID}");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 儲存檔案到伺服器
            string filePath = Path.Combine(uploadPath, newFileName);
            
            File.WriteAllBytes(filePath, fileBytes);

            // 先刪除現有記錄（如果存在）
            DeleteUploadFileRecord(projectID, fileCode);

            // 建立相對路徑
            string relativePath = $"~/UploadFiles/OFS/CLB/{projectID}/{newFileName}";
            
            // 儲存檔案資訊到資料庫
            SaveUploadFileRecord(projectID, fileCode, newFileName, relativePath);

            return (true, "檔案上傳成功", newFileName);
        }
        catch (Exception ex)
        {
            return (false, $"檔案上傳失敗：{ex.Message}", "");
        }
    }

    /// <summary>
    /// 取得已上傳的檔案
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <returns>上傳檔案物件</returns>
    public static OFS_CLB_UploadFile GetUploadedFile(string projectID, string fileCode)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile] 
                WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@FileCode", fileCode);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_UploadFile
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    FileCode = row["FileCode"]?.ToString(),
                    FileName = row["FileName"]?.ToString(),
                    TemplatePath = row["TemplatePath"]?.ToString()
                };
            }

            return null;
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
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile] 
                WHERE [ProjectID] = @ProjectID
                ORDER BY [FileCode]";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();
            List<OFS_CLB_UploadFile> files = new List<OFS_CLB_UploadFile>();

            foreach (DataRow row in dt.Rows)
            {
                files.Add(new OFS_CLB_UploadFile
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    FileCode = row["FileCode"]?.ToString(),
                    FileName = row["FileName"]?.ToString(),
                    TemplatePath = row["TemplatePath"]?.ToString()
                });
            }

            return files;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得上傳檔案清單失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 刪除已上傳的檔案
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <returns>刪除結果</returns>
    public static bool DeleteUploadedFile(string projectID, string fileCode)
    {
        try
        {
            // 先取得檔案資訊
            var uploadedFile = GetUploadedFile(projectID, fileCode);
            if (uploadedFile != null)
            {
                // 刪除實體檔案
                string uploadPath = HttpContext.Current.Server.MapPath($"~/UploadFiles/OFS/CLB/{projectID}");
                string filePath = Path.Combine(uploadPath, uploadedFile.FileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // 刪除資料庫記錄
                DeleteUploadFileRecord(projectID, fileCode);
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"刪除檔案失敗：{ex.Message}");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 儲存檔案上傳記錄到資料庫
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <param name="fileName">檔案名稱</param>
    /// <param name="templatePath">範本路徑</param>
    private static void SaveUploadFileRecord(string projectID, string fileCode, string fileName, string templatePath)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile]
            (
                [ProjectID],
                [FileCode],
                [FileName],
                [TemplatePath]
            )
            VALUES
            (
                @ProjectID,
                @FileCode,
                @FileName,
                @TemplatePath
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@FileCode", fileCode);
        db.Parameters.Add("@FileName", fileName);
        db.Parameters.Add("@TemplatePath", templatePath ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 刪除檔案上傳記錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">檔案代碼</param>
    private static void DeleteUploadFileRecord(string projectID, string fileCode)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile] 
            WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@FileCode", fileCode);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據檔案代碼取得附件名稱
    /// </summary>
    /// <param name="fileCode">檔案代碼</param>
    /// <returns>附件名稱</returns>
    private static string GetAttachmentNameByFileCode(string fileCode)
    {
        var mapping = new Dictionary<string, string>
        {
            { "PaymentIncomeStatement", "收支明細表" },
            { "PaymentSubsidyList", "受補助清單" },
            { "PaymentCostAllocation", "經費分攤表" },
            { "PaymentVouchers", "憑證" },
            { "PaymentReceipts", "領據（含帳戶資料）" }
        };

        return mapping.ContainsKey(fileCode) ? mapping[fileCode] : "";
    }

    #endregion

    #region 請款資料處理方法

    /// <summary>
    /// 取得計畫的核定經費
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>核定經費</returns>
    public static decimal GetApprovedSubsidy(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return 0;
            }

            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT [ApprovedSubsidy] 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main] 
                WHERE [ProjectID] = @ProjectID AND [isExist] = 1";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                var approvedSubsidy = dt.Rows[0]["ApprovedSubsidy"];
                if (approvedSubsidy != DBNull.Value)
                {
                    return Convert.ToDecimal(approvedSubsidy);
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得核定經費失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 檢查檔案上傳狀態
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>檢查結果</returns>
    public static (bool success, string message) CheckFileUploadStatus(string projectID)
    {
        try
        {
            // 需要檢查的檔案代碼
            string[] requiredFiles = { 
                "PaymentIncomeStatement", 
                "PaymentSubsidyList", 
                "PaymentCostAllocation", 
                "PaymentVouchers", 
                "PaymentReceipts" 
            };

            string[] fileNames = { 
                "收支明細表", 
                "受補助清單", 
                "經費分攤表", 
                "憑證", 
                "領據（含帳戶資料）" 
            };

            List<string> missingFiles = new List<string>();

            for (int i = 0; i < requiredFiles.Length; i++)
            {
                var uploadedFile = GetUploadedFile(projectID, requiredFiles[i]);
                if (uploadedFile == null)
                {
                    missingFiles.Add(fileNames[i]);
                }
            }

            if (missingFiles.Count > 0)
            {
                string message = $"以下檔案尚未上傳：{string.Join("、", missingFiles)}";
                return (false, message);
            }

            return (true, "所有檔案已上傳");
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查檔案上傳狀態失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存請款資料 (暫存或提送)
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="isDraft">是否為暫存</param>
    /// <param name="currentAmount">本期請款金額</param>
    /// <param name="totalSpentAmount">累積實支金額</param>
    /// <returns>操作結果</returns>
    public static (bool success, string message) SaveReimbursementData(string projectID, bool isDraft, decimal currentAmount, decimal totalSpentAmount)
    {
        try
        {
            // 如果是提送，需要檢查檔案是否齊全
            if (!isDraft)
            {
                var fileCheckResult = CheckFileUploadStatus(projectID);
                if (!fileCheckResult.success)
                {
                    return (false, fileCheckResult.message);
                }
            }

            // 設定狀態
            string status = isDraft ? "暫存" : "審核中";

            // 檢查是否已存在記錄
            var existingPayment = GetPaymentRecord(projectID, 1);
            
            if (existingPayment != null)
            {
                // 更新現有記錄
                UpdatePaymentRecord(projectID, 1, currentAmount, totalSpentAmount, status);
            }
            else
            {
                // 建立新記錄
                CreatePaymentRecord(projectID, 1, currentAmount, totalSpentAmount, status);
            }

            // 如果是提送，觸發寄信功能
            if (!isDraft)
            {
                var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
                var projectMainData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

                if (basicData != null && projectMainData != null)
                {
                    string projectName = basicData.ProjectNameTw;
                    string supervisoryAccount = projectMainData.SupervisoryPersonAccount;

                    // 根據承辦人帳號取得 UserID
                    int? organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);

                    // 寄送通知信
                    NotificationHelper.G2("社團", projectName, "請款", organizer);
                }            
            }

            string actionText = isDraft ? "暫存" : "提送";
            return (true, $"{actionText}成功");
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存請款資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 取得請款記錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="stage">期別</param>
    /// <returns>請款記錄</returns>
    public static OFS_CLB_Payment GetPaymentRecord(string projectID, int stage)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Payment] 
                WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@Stage", stage);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_Payment
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    Stage = row["Stage"] != DBNull.Value ? Convert.ToInt32(row["Stage"]) : (int?)null,
                    CurrentRequestAmount = row["CurrentRequestAmount"] != DBNull.Value ? Convert.ToDecimal(row["CurrentRequestAmount"]) : (decimal?)null,
                    TotalSpentAmount = row["TotalSpentAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalSpentAmount"]) : (decimal?)null,
                    CurrentActualPaidAmount = row["CurrentActualPaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["CurrentActualPaidAmount"]) : (decimal?)null,
                    Status = row["Status"]?.ToString(),
                    ReviewerComment = row["ReviewerComment"]?.ToString(),
                    ReviewUser = row["ReviewUser"]?.ToString(),
                    ReviewTime = row["ReviewTime"] != DBNull.Value ? Convert.ToDateTime(row["ReviewTime"]) : (DateTime?)null,
                    CreateTime = row["CreateTime"] != DBNull.Value ? Convert.ToDateTime(row["CreateTime"]) : (DateTime?)null,
                    UpdateTime = row["UpdateTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdateTime"]) : (DateTime?)null
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得請款記錄失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 建立請款記錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="stage">期別</param>
    /// <param name="currentAmount">本期請款金額</param>
    /// <param name="totalSpentAmount">累積實支金額</param>
    /// <param name="status">狀態</param>
    private static void CreatePaymentRecord(string projectID, int stage, decimal currentAmount, decimal totalSpentAmount, string status)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Payment]
            (
                [ProjectID],
                [Stage],
                [CurrentRequestAmount],
                [TotalSpentAmount],
                [CurrentActualPaidAmount],
                [Status],
                [CreateTime],
                [UpdateTime]
            )
            VALUES
            (
                @ProjectID,
                @Stage,
                @CurrentRequestAmount,
                @TotalSpentAmount,
                NULL,
                @Status,
                GETDATE(),
                GETDATE()
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);
        db.Parameters.Add("@CurrentRequestAmount", currentAmount);
        db.Parameters.Add("@TotalSpentAmount", totalSpentAmount);
        db.Parameters.Add("@Status", status);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新請款記錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="stage">期別</param>
    /// <param name="currentAmount">本期請款金額</param>
    /// <param name="totalSpentAmount">累積實支金額</param>
    /// <param name="status">狀態</param>
    private static void UpdatePaymentRecord(string projectID, int stage, decimal currentAmount, decimal totalSpentAmount, string status)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Payment]
            SET 
                [CurrentRequestAmount] = @CurrentRequestAmount,
                [TotalSpentAmount] = @TotalSpentAmount,
                [Status] = @Status,
                [UpdateTime] = GETDATE()
            WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);
        db.Parameters.Add("@CurrentRequestAmount", currentAmount);
        db.Parameters.Add("@TotalSpentAmount", totalSpentAmount);
        db.Parameters.Add("@Status", status);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 提交審查結果
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="reviewResult">審查結果 (pass/return)</param>
    /// <param name="reviewComment">審查意見</param>
    /// <param name="currentPayment">本期撥款金額</param>
    /// <param name="reviewUser">審查人員</param>
    /// <returns>處理結果</returns>
    public static (bool success, string message) SubmitReviewResult(string projectID, string reviewResult, string reviewComment, decimal currentPayment, string reviewUser)
    {
        try
        {
            // 取得當前請款記錄
            var paymentRecord = GetPaymentRecord(projectID, 1);
            if (paymentRecord == null)
            {
                return (false, "找不到請款記錄");
            }

            // 檢查狀態是否為審核中
            if (paymentRecord.Status != "審核中")
            {
                return (false, "此請款案件目前不在審核狀態");
            }

            string newStatus;
            decimal? actualPaidAmount = null;

            if (reviewResult == "pass")
            {
                // 通過：不需填寫原因，更新狀態為「通過」，記錄本期撥款金額
                newStatus = "通過";
                actualPaidAmount = currentPayment;
            }
            else if (reviewResult == "return")
            {
                // 退回修改：必須填寫原因，更新狀態為「暫存」
                if (string.IsNullOrWhiteSpace(reviewComment))
                {
                    return (false, "退回修改時必須填寫原因");
                }
                newStatus = "暫存";
            }
            else
            {
                return (false, "無效的審查結果");
            }

            // 更新請款記錄
            UpdateReviewResult(projectID, 1, newStatus, reviewComment, reviewUser, actualPaidAmount);

            // 寄送通知信件
            var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
            var projectMainData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            string projectName = basicData.ProjectNameTw;
            string UserAccount = projectMainData.UserAccount;

            if (reviewResult == "return")
            {
                
                // 寄送通知信
                NotificationHelper.G3("社團", projectName, "請款", reviewComment,UserAccount);
            
            }
            else
            {
                // 寄送通知信
                NotificationHelper.G6("社團", projectName, "請款",actualPaidAmount, reviewComment,UserAccount);
            }

            

            string resultText = reviewResult == "pass" ? "通過" : "退回修改";
            return (true, $"審查結果已提交：{resultText}");
        }
        catch (Exception ex)
        {
            throw new Exception($"提交審查結果失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新審查結果
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="stage">期別</param>
    /// <param name="status">狀態</param>
    /// <param name="reviewComment">審查意見</param>
    /// <param name="reviewUser">審查人員</param>
    /// <param name="actualPaidAmount">實際撥款金額</param>
    private static void UpdateReviewResult(string projectID, int stage, string status, string reviewComment, string reviewUser, decimal? actualPaidAmount)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Payment]
            SET 
                [Status] = @Status,
                [ReviewerComment] = @ReviewerComment,
                [ReviewUser] = @ReviewUser,
                [ReviewTime] = GETDATE(),
                [CurrentActualPaidAmount] = @CurrentActualPaidAmount,
                [UpdateTime] = GETDATE()
            WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);
        db.Parameters.Add("@Status", status);
        db.Parameters.Add("@ReviewerComment", reviewComment ?? "");
        db.Parameters.Add("@ReviewUser", reviewUser);
        db.Parameters.Add("@CurrentActualPaidAmount", actualPaidAmount.HasValue ? (object)actualPaidAmount.Value : DBNull.Value);

        db.ExecuteNonQuery();
    }

    #endregion
}
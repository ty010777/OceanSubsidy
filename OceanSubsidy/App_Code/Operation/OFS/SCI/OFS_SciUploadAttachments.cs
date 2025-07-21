using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// 海洋科技專案計畫申請 - 上傳附件相關資料操作
/// </summary>
public class OFS_SciUploadAttachmentsHelper
{
    public OFS_SciUploadAttachmentsHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 更新專案狀態為暫存 - 設定 Form5Status 為 '暫存'
    /// </summary>
    /// <param name="projectID">專案ID</param>
    public static void UpdateProjectSaveStatus(string projectID)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                UPDATE [OFS_SCI_Project_Main] 
                SET Form5Status = @Form5Status,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@Form5Status", "暫存");
            db.Parameters.Add("@ProjectID", projectID);

            try
            {
                db.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"專案 {projectID} 暫存狀態更新成功：Form5Status=暫存");
            }
            catch (Exception ex)
            {
                throw new Exception($"更新專案暫存狀態時發生錯誤：{ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 更新專案狀態為提送申請完成 - 設定 Form5Status 為 '完成'，CurrentStep 為 5
    /// </summary>
    /// <param name="projectID">專案ID</param>
    public static void UpdateProjectSubmissionStatus(string projectID)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                UPDATE [OFS_SCI_Project_Main] 
                SET Form5Status = @Form5Status,
                    CurrentStep = @CurrentStep,
                    Statuses = @Statuses,
                    StatusesName = @StatusesName,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@Form5Status", "完成");
            db.Parameters.Add("@CurrentStep", "6");
            db.Parameters.Add("@Statuses", "資格審查");
            db.Parameters.Add("@StatusesName", "審查中");
            db.Parameters.Add("@ProjectID", projectID);

            try
            {
                 db.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"專案 {projectID} 提送申請狀態更新成功：Form5Status=完成, CurrentStep=5, Statuses=資格審查, StatusesName=審查中");
            }
            catch (Exception ex)
            {
                throw new Exception($"更新專案提送申請狀態時發生錯誤：{ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 儲存附件上傳記錄到資料庫
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="fileCode">附件代碼</param>
    /// <param name="fileName">檔案名稱</param>
    /// <param name="templatePath">檔案路徑</param>
    /// <param name="statuses">狀態</param>
    public static void InsertAttachmentRecord(string projectID, string fileCode, string fileName, 
        string templatePath, string statuses = "已上傳")
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                INSERT INTO [OFS_SCI_UploadFile] (
                    ProjectID,
                    FileCode,
                    FileName,
                    TemplatePath,
                    Statuses
                )
                VALUES (
                    @ProjectID,
                    @FileCode,
                    @FileName,
                    @TemplatePath,
                    @Statuses
                )";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@FileCode", fileCode);
            db.Parameters.Add("@FileName", fileName);
            db.Parameters.Add("@TemplatePath", templatePath);
            db.Parameters.Add("@Statuses", statuses);

            try
            {
                db.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"附件上傳記錄已儲存：{projectID} - {fileCode} - {fileName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"儲存附件上傳記錄時發生錯誤：{ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 取得專案的附件上傳記錄
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>附件記錄清單</returns>
    public static List<OFS_SCI_UploadFile> GetAttachmentsByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OFS_SCI_UploadFile]
            WHERE ProjectID = @ProjectID
            ORDER BY ID DESC";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        try
        {
            DataTable dt = db.GetTable();
            List<OFS_SCI_UploadFile> attachmentList = new List<OFS_SCI_UploadFile>();

            foreach (DataRow row in dt.Rows)
            {
                var attachment = new OFS_SCI_UploadFile
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    FileCode = row["FileCode"]?.ToString(),
                    FileName = row["FileName"]?.ToString(),
                    TemplatePath = row["TemplatePath"]?.ToString(),
                    Statuses = row["Statuses"]?.ToString()
                };

                attachmentList.Add(attachment);
            }

            return attachmentList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢附件記錄時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 刪除附件記錄
    /// </summary>
    /// <param name="attachmentId">附件ID</param>
    public static void DeleteAttachmentRecord(int attachmentId)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                DELETE FROM [OFS_SCI_UploadFile]
                WHERE ID = @ID";

            db.Parameters.Clear();
            db.Parameters.Add("@ID", attachmentId);

            try
            {
                db.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"附件記錄已刪除：ID {attachmentId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"刪除附件記錄時發生錯誤：{ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 根據FileCode和專案ID查詢附件記錄
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <returns>附件記錄清單</returns>
    public static List<OFS_SCI_UploadFile> GetAttachmentsByFileCodeAndProject(string projectID, string fileCode)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OFS_SCI_UploadFile]
            WHERE ProjectID = @ProjectID AND FileCode = @FileCode
            ORDER BY ID DESC";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@FileCode", fileCode);

        try
        {
            DataTable dt = db.GetTable();
            List<OFS_SCI_UploadFile> attachmentList = new List<OFS_SCI_UploadFile>();

            foreach (DataRow row in dt.Rows)
            {
                var attachment = new OFS_SCI_UploadFile
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    FileCode = row["FileCode"]?.ToString(),
                    FileName = row["FileName"]?.ToString(),
                    TemplatePath = row["TemplatePath"]?.ToString(),
                    Statuses = row["Statuses"]?.ToString()
                };

                attachmentList.Add(attachment);
            }

            return attachmentList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢特定類型附件記錄時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查專案是否已上傳所有必要附件
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>true: 已上傳所有必要附件, false: 尚有必要附件未上傳</returns>
    public static bool ValidateRequiredAttachments(string projectID)
    {
        // 定義必要附件代碼對應
        var requiredFileCodes = new Dictionary<string, string>
        {
            { "FILE_002", "海洋科技科專案計畫書" },
            { "FILE_003", "建議迴避之審查委員清單" },
            { "FILE_004", "未違反公職人員利益衝突迴避法切結書" },
            { "FILE_005", "蒐集個人資料告知事項暨個人資料提供同意書" },
            { "FILE_006", "共同執行單位基本資料表" },
            { "FILE_007", "申請人自我檢查表" },
            { "FILE_009", "海洋委員會補助科技專案計畫契約書" },
            { "FILE_011", "海洋科技專案成效追蹤自評表" }
        };

        try
        {
            var uploadedAttachments = GetAttachmentsByProjectID(projectID);
            var uploadedFileCodes = uploadedAttachments.Select(a => a.FileCode).Distinct().ToList();

            // 檢查每個必要附件代碼是否都已上傳
            foreach (var requiredCode in requiredFileCodes.Keys)
            {
                if (!uploadedFileCodes.Contains(requiredCode))
                {
                    System.Diagnostics.Debug.WriteLine($"專案 {projectID} 缺少必要附件：{requiredFileCodes[requiredCode]} (代碼: {requiredCode})");
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"驗證必要附件時發生錯誤：{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 更新附件記錄 - 替換現有附件
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <param name="fileName">新檔案名稱</param>
    /// <param name="templatePath">檔案路徑</param>
    /// <param name="statuses">狀態</param>
    public static void UpdateAttachmentRecord(string projectID, string fileCode, string fileName,
        string templatePath, string statuses = "已上傳")
    {
        using (DbHelper db = new DbHelper())
        {
            // 先刪除該類型的舊附件記錄
            db.CommandText = @"
                DELETE FROM [OFS_SCI_UploadFile]
                WHERE ProjectID = @ProjectID AND FileCode = @FileCode";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@FileCode", fileCode);

            try
            {
                db.ExecuteNonQuery();

                // 插入新的附件記錄
                InsertAttachmentRecord(projectID, fileCode, fileName, templatePath, statuses);

                System.Diagnostics.Debug.WriteLine($"附件記錄已更新：{projectID} - {fileCode} - {fileName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"更新附件記錄時發生錯誤：{ex.Message}", ex);
            }
        }
    }
}
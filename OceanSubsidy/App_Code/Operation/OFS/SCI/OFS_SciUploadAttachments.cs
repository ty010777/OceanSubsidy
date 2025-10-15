using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;

/// <summary>
/// 海洋科技專案計畫申請 - 上傳附件相關資料操作
/// </summary>
public class OFS_SciUploadAttachmentsHelper
{
    public OFS_SciUploadAttachmentsHelper()
    {
        
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
            db.Parameters.Add("@StatusesName", "審核中");
            db.Parameters.Add("@ProjectID", projectID);

            try
            {
                 db.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"專案 {projectID} 提送申請狀態更新成功：Form5Status=完成, CurrentStep=5, Statuses=資格審查, StatusesName=審核中");
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
    public static void InsertAttachmentRecord(string projectID, string fileCode, string fileName, 
        string templatePath)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                INSERT INTO [OFS_SCI_UploadFile] (
                    ProjectID,
                    FileCode,
                    FileName,
                    TemplatePath
                )
                VALUES (
                    @ProjectID,
                    @FileCode,
                    @FileName,
                    @TemplatePath
                )";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@FileCode", fileCode);
            db.Parameters.Add("@FileName", fileName);
            db.Parameters.Add("@TemplatePath", templatePath);

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
    public static void UpdateAttachmentRecord(string projectID, string fileCode, string fileName,
        string templatePath)
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
                InsertAttachmentRecord(projectID, fileCode, fileName, templatePath);

                System.Diagnostics.Debug.WriteLine($"附件記錄已更新：{projectID} - {fileCode} - {fileName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"更新附件記錄時發生錯誤：{ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 生成附件檔案名稱（新格式：計畫編號_附件名稱.pdf）
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <returns>檔案名稱</returns>
    public static string GenerateFileName(string projectID, string fileCode)
    {
        var attachmentNames = new Dictionary<string, string>
        {
            // 學研範本附件名稱對應
            { "FILE_AC2", "海洋科技科專案計畫書" },
            { "FILE_AC3", "建議迴避之審查委員清單" },
            { "FILE_AC4", "未違反公職人員利益衝突迴避法切結書" },
            { "FILE_AC5", "蒐集個人資料告知事項暨個人資料提供同意書" },
            { "FILE_AC6", "共同執行單位基本資料表" },
            { "FILE_AC7", "申請人自我檢查表" },
            { "FILE_AC9", "海洋委員會補助科技專案計畫契約書" },
            { "FILE_AC11", "海洋科技專案成效追蹤自評表" },
            // 業者範本附件名稱對應
            { "FILE_OTech2", "海洋科技科專案計畫書" },
            { "FILE_OTech3", "建議迴避之審查委員清單" },
            { "FILE_OTech4", "未違反公職人員利益衝突迴避法切結書" },
            { "FILE_OTech5", "蒐集個人資料告知事項暨個人資料提供同意書" },
            { "FILE_OTech6", "申請人自我檢查表" },
            { "FILE_OTech8", "海洋科技業者科專計畫補助契約書" },
       
        };

        if (attachmentNames.ContainsKey(fileCode))
        {
            return $"{projectID}_{attachmentNames[fileCode]}.pdf";
        }

        return $"{projectID}_{fileCode}.pdf";
    }

    /// <summary>
    /// 取得檔案實體路徑
    /// </summary>
    /// <param name="templatePath">資料庫儲存的相對路徑</param>
    /// <returns>實體檔案路徑</returns>
    public static string GetPhysicalFilePath(string templatePath)
    {
        if (string.IsNullOrEmpty(templatePath))
            return "";
        string root = AppDomain.CurrentDomain.BaseDirectory;

        return Path.Combine(root, templatePath.TrimStart('~', '/', '\\'));
    }

    /// <summary>
    /// 刪除附件檔案（實體檔案和資料庫記錄）
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="fileCode">檔案代碼</param>
    public static void DeleteAttachmentFile(string projectID, string fileCode)
    {
        try
        {
            // 先取得現有的附件記錄
            var attachments = GetAttachmentsByFileCodeAndProject(projectID, fileCode);
            
            foreach (var attachment in attachments)
            {
                // 刪除實體檔案
                string physicalPath = GetPhysicalFilePath(attachment.TemplatePath);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                    System.Diagnostics.Debug.WriteLine($"實體檔案已刪除：{physicalPath}");
                }

                // 刪除資料庫記錄
                DeleteAttachmentRecord(attachment.ID);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"刪除附件檔案時發生錯誤：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 更新計畫變更狀態為已完成 (IsProjChanged = 2)
    /// </summary>
    /// <param name="projectID">專案ID</param>
    public static void UpdateProjectChangeCompleted(string projectID)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                UPDATE [OFS_SCI_Project_Main]
                SET IsProjChanged = @IsProjChanged,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@IsProjChanged", 2);
            db.Parameters.Add("@ProjectID", projectID);

            try
            {
                db.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"專案 {projectID} 計畫變更狀態更新成功：IsProjChanged=2");
            }
            catch (Exception ex)
            {
                throw new Exception($"更新計畫變更狀態時發生錯誤：{ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 取得計畫變更最新版本號
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>最新版本號（計畫變更紀錄筆數）</returns>
    public static int GetLatestChangeVersionNumber(string projectID)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                SELECT COUNT(ID)
                FROM [OFS_ProjectChangeRecord]
                WHERE Type = @Type AND DataID = @DataID AND Method = @Method";

            db.Parameters.Clear();
            db.Parameters.Add("@Type", "SCI");
            db.Parameters.Add("@DataID", projectID);
            db.Parameters.Add("@Method", 1);

            try
            {
                DataTable dt = db.GetTable();
                int count = 0;

                if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                {
                    count = Convert.ToInt32(dt.Rows[0][0]);
                }

                System.Diagnostics.Debug.WriteLine($"專案 {projectID} 的計畫變更記錄數：{count}");
                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得計畫變更版本號時發生錯誤：{ex.Message}");
                throw new Exception($"取得計畫變更版本號時發生錯誤：{ex.Message}", ex);
            }
        }
    }
}
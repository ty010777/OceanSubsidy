<%@ WebHandler Language="C#" Class="SCI_Upload" %>

using System;
using System.Web;
using System.IO;
using System.Linq;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;

public class SCI_Upload : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        var action = context.Request.Form["action"];

        if (string.IsNullOrEmpty(action))
        {
            context.Response.Write("{\"success\":false,\"message\":\"Missing action parameter. Use 'upload' or 'delete'.\"}");
            return;
        }

        try
        {
            switch (action.ToLower())
            {
                case "upload":
                    HandleFileUpload(context);
                    break;
                case "delete":
                    HandleFileDelete(context);
                    break;
                case "uploadtechdiagram":
                    HandleTechDiagramUpload(context);
                    break;
                default:
                    context.Response.Write("{\"success\":false,\"message\":\"Invalid action. Use 'upload', 'delete', or 'uploadTechDiagram'.\"}");
                    break;
            }
        }
        catch (Exception ex)
        {
            context.Response.Write($"{{\"success\":false,\"message\":\"Error processing request: {ex.Message}\"}}");
        }

        context.Response.End();
    }

    /// <summary>
    /// 處理檔案上傳
    /// </summary>
    private void HandleFileUpload(HttpContext context)
    {
        try
        {
            string projectID = context.Request.Form["projectID"];
            string fileCode = context.Request.Form["fileCode"];
            string fileType = context.Request.Form["fileType"];
            string attachmentName = "";

            if (string.IsNullOrEmpty(projectID))
            {
                context.Response.Write("{\"success\":false,\"message\":\"計畫編號不能為空\"}");
                return;
            }

            if (context.Request.Files.Count == 0 || context.Request.Files[0] == null)
            {
                context.Response.Write("{\"success\":false,\"message\":\"請選擇要上傳的檔案\"}");
                return;
            }

            HttpPostedFile uploadedFile = context.Request.Files[0];

            // 驗證檔案
            string validationResult = ValidateUploadedFile(uploadedFile);
            if (!string.IsNullOrEmpty(validationResult))
            {
                context.Response.Write($"{{\"success\":false,\"message\":\"{validationResult}\"}}");
                return;
            }

            // 使用 Helper 方法生成檔案名稱
            string fileName = OFS_SciUploadAttachmentsHelper.GenerateFileName(projectID, fileCode);

            // 上傳檔案
            string relativePath = SaveUploadedFile(context, uploadedFile, fileName, projectID);
            if (string.IsNullOrEmpty(relativePath))
            {
                context.Response.Write("{\"success\":false,\"message\":\"檔案上傳失敗\"}");
                return;
            }

            // 儲存到資料庫
            SaveFileToDatabase(projectID, fileCode, fileName, relativePath);

            context.Response.Write($"{{\"success\":true,\"message\":\"檔案上傳成功\",\"fileName\":\"{fileName}\",\"relativePath\":\"{relativePath}\"}}");
        }
        catch (Exception ex)
        {
            context.Response.Write($"{{\"success\":false,\"message\":\"檔案上傳失敗：{ex.Message}\"}}");
        }
    }

    /// <summary>
    /// 處理檔案刪除
    /// </summary>
    private void HandleFileDelete(HttpContext context)
    {
        try
        {
            string projectID = context.Request.Form["projectID"];
            string fileCode = context.Request.Form["fileCode"];
            string fileType = context.Request.Form["fileType"];

            if (string.IsNullOrEmpty(projectID))
            {
                context.Response.Write("{\"success\":false,\"message\":\"計畫編號不能為空\"}");
                return;
            }

            // 使用 Helper 方法刪除附件檔案（實體檔案和資料庫記錄）
            OFS_SciUploadAttachmentsHelper.DeleteAttachmentFile(projectID, fileCode);

            context.Response.Write("{\"success\":true,\"message\":\"檔案刪除成功\"}");
        }
        catch (Exception ex)
        {
            context.Response.Write($"{{\"success\":false,\"message\":\"檔案刪除失敗：{ex.Message}\"}}");
        }
    }

    /// <summary>
    /// 驗證上傳的檔案
    /// </summary>
    private string ValidateUploadedFile(HttpPostedFile file)
    {
        if (file == null || file.ContentLength == 0)
        {
            return "請選擇要上傳的檔案";
        }

        // 檔案大小限制：10MB (與 SCI 系統一致)
        if (file.ContentLength > 10 * 1024 * 1024)
        {
            return "檔案大小不能超過 10MB";
        }

        // 檔案副檔名檢查 (SCI 系統只接受 PDF)
        string fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (fileExtension != ".pdf")
        {
            return "僅接受 PDF 檔案格式";
        }

        return ""; // 驗證通過
    }


    /// <summary>
    /// 儲存上傳的檔案到指定路徑
    /// </summary>
    private string SaveUploadedFile(HttpContext context, HttpPostedFile file, string fileName, string projectID)
    {
        try
        {
            // 建立目錄路徑
            string uploadDir = $"~/UploadFiles/OFS/SCI/{projectID}/SciApplication";
            string physicalPath = context.Server.MapPath(uploadDir);

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            // 完整檔案路徑
            string fullPath = Path.Combine(physicalPath, fileName);

            // 如果檔案已存在，先刪除
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 儲存檔案
            file.SaveAs(fullPath);

            // 回傳相對路徑
            return $"UploadFiles/OFS/SCI/{projectID}/SciApplication/{fileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"檔案儲存失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存檔案資訊到資料庫
    /// </summary>
    private void SaveFileToDatabase(string projectID, string fileCode, string fileName, string relativePath)
    {
        try
        {
            // 使用 Helper 方法更新附件記錄（會先刪除同類型的舊記錄，再插入新記錄）
            OFS_SciUploadAttachmentsHelper.UpdateAttachmentRecord(projectID, fileCode, fileName, relativePath);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存檔案資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 處理技術能力與技術關聯圖上傳
    /// </summary>
    private void HandleTechDiagramUpload(HttpContext context)
    {
        try
        {
            string projectID = context.Request.Form["projectID"];

            if (string.IsNullOrEmpty(projectID))
            {
                context.Response.Write("{\"success\":false,\"message\":\"缺少 ProjectID 參數\"}");
                return;
            }

            if (context.Request.Files.Count == 0 || context.Request.Files[0] == null)
            {
                context.Response.Write("{\"success\":false,\"message\":\"沒有檔案被上傳\"}");
                return;
            }

            HttpPostedFile uploadedFile = context.Request.Files[0];

            // 驗證檔案
            string validationResult = ValidateTechDiagramFile(uploadedFile);
            if (!string.IsNullOrEmpty(validationResult))
            {
                context.Response.Write($"{{\"success\":false,\"message\":\"{validationResult}\"}}");
                return;
            }

            // 建立檔案名稱
            string fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
            string fileName = $"TechnologyDiagram{fileExtension}";

            // 上傳檔案
            string relativePath = SaveTechDiagramFile(context, uploadedFile, fileName, projectID);
            if (string.IsNullOrEmpty(relativePath))
            {
                context.Response.Write("{\"success\":false,\"message\":\"檔案上傳失敗\"}");
                return;
            }

            // 儲存到資料庫
            SaveTechDiagramToDatabase(projectID, fileName, relativePath);

            context.Response.Write($"{{\"success\":true,\"message\":\"檔案上傳成功\",\"fileName\":\"{fileName}\",\"relativePath\":\"{relativePath}\"}}");
        }
        catch (Exception ex)
        {
            context.Response.Write($"{{\"success\":false,\"message\":\"檔案上傳失敗：{ex.Message}\"}}");
        }
    }

    /// <summary>
    /// 驗證技術關聯圖檔案
    /// </summary>
    private string ValidateTechDiagramFile(HttpPostedFile file)
    {
        if (file == null || file.ContentLength == 0)
        {
            return "請選擇要上傳的檔案";
        }

        // 檔案大小限制：10MB
        if (file.ContentLength > 10 * 1024 * 1024)
        {
            return "檔案大小不能超過 10MB";
        }

        // 檔案副檔名檢查（只接受圖片格式）
        string fileExtension = Path.GetExtension(file.FileName).ToLower();
        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

        if (!allowedExtensions.Contains(fileExtension))
        {
            return "只支援 JPG 和 PNG 格式的圖片";
        }

        return ""; // 驗證通過
    }

    /// <summary>
    /// 儲存技術關聯圖檔案到指定路徑
    /// </summary>
    private string SaveTechDiagramFile(HttpContext context, HttpPostedFile file, string fileName, string projectID)
    {
        try
        {
            // 建立目錄路徑
            string uploadDir = $"~/UploadFiles/OFS/SCI/{projectID}/";
            string physicalPath = context.Server.MapPath(uploadDir);

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            // 完整檔案路徑
            string fullPath = Path.Combine(physicalPath, fileName);

            // 如果檔案已存在，先刪除
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 儲存檔案
            file.SaveAs(fullPath);

            // 回傳相對路徑
            return $"UploadFiles/OFS/SCI/{projectID}/{fileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"檔案儲存失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存技術關聯圖資訊到資料庫
    /// </summary>
    private void SaveTechDiagramToDatabase(string projectID, string fileName, string relativePath)
    {
        try
        {
            // 先檢查是否已有該版本的技術能力與技術關聯圖記錄
            var existingFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(projectID, "TechnologyDiagram");

            // 如果有舊記錄，先刪除
            if (existingFiles != null && existingFiles.Any())
            {
                foreach (var existingFile in existingFiles)
                {
                    OFS_SciWorkSchHelper.DeleteUploadFile(existingFile.ProjectID, existingFile.FileCode);
                }
            }

            // 建立新的上傳檔案記錄
            var uploadFile = new OFS_SCI_UploadFile
            {
                ProjectID = projectID,
                FileCode = "TechnologyDiagram",
                FileName = fileName,
                TemplatePath = relativePath
            };

            // 儲存新記錄到資料庫
            OFS_SciWorkSchHelper.InsertUploadFile(uploadFile);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存檔案資訊失敗：{ex.Message}");
        }
    }

    public bool IsReusable => false;
}
<%@ WebHandler Language="C#" Class="CLB_Upload" %>

using System;
using System.Web;
using System.IO;
using GS.OCA_OceanSubsidy.Entity;

public class CLB_Upload : IHttpHandler
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
                default:
                    context.Response.Write("{\"success\":false,\"message\":\"Invalid action. Use 'upload' or 'delete'.\"}");
                    break;
            }
        }
        catch (Exception ex)
        {
            string escapedMessage = EscapeJsonString(ex.Message);
            context.Response.Write($"{{\"success\":false,\"message\":\"Error processing request: {escapedMessage}\"}}");
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
            string fileCode = context.Request.Form["fileCode"]; // 1 2 3 4 5
            string fileType = context.Request.Form["fileType"]; // 用於 StageReport ,Payment
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

            // 決定 FileCode 和附件名稱
             if (!string.IsNullOrEmpty(fileCode))
            {
                fileCode = GetFileCodeByType(fileType, fileCode);
                attachmentName = GetAttachmentNameByFileType(fileType, fileCode);
            }
            // else if (string.IsNullOrEmpty(attachmentName))
            // {
            //     // 如果沒有提供 attachmentName，根據 fileCode 生成
            //     // attachmentName = GetAttachmentNameByFileCode(fileCode);
            // }

            // 生成檔案名稱
            string fileExtension = Path.GetExtension(uploadedFile.FileName);
            string fileName = $"{projectID}_{attachmentName}{fileExtension}";

            // 上傳檔案
            string relativePath = SaveUploadedFile(context, uploadedFile, fileName, projectID);
            if (string.IsNullOrEmpty(relativePath))
            {
                context.Response.Write("{\"success\":false,\"message\":\"檔案上傳失敗\"}");
                return;
            }

            // 儲存到資料庫
            SaveFileToDatabase(projectID, fileCode, fileName, relativePath);

            // 使用 System.Web.HttpUtility.JavaScriptStringEncode 或手動跳脫特殊字元
            string escapedFileName = EscapeJsonString(fileName);
            string escapedRelativePath = EscapeJsonString(relativePath);

            context.Response.Write($"{{\"success\":true,\"message\":\"檔案上傳成功\",\"fileName\":\"{escapedFileName}\",\"relativePath\":\"{escapedRelativePath}\"}}");
        }
        catch (Exception ex)
        {
            string escapedMessage = EscapeJsonString(ex.Message);
            context.Response.Write($"{{\"success\":false,\"message\":\"檔案上傳失敗：{escapedMessage}\"}}");
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
            string fileType = context.Request.Form["fileType"]; // 用於 StageReport

            if (string.IsNullOrEmpty(projectID))
            {
                context.Response.Write("{\"success\":false,\"message\":\"計畫編號不能為空\"}");
                return;
            }

            // 決定 FileCode
          
            fileCode = GetFileCodeByType(fileType,fileCode);
            

            // 從資料庫取得檔案資訊
            var uploadedFile = OFS_ClbApplicationHelper.GetUploadedFile(projectID, fileCode);
            if (uploadedFile != null)
            {
                // 刪除實體檔案
                string templatePath = uploadedFile.TemplatePath;

                // 如果路徑不是以 ~/ 開頭，則加上
                if (!templatePath.StartsWith("~/"))
                {
                    // 移除開頭的 / 或 \ 避免重複
                    templatePath = templatePath.TrimStart('/', '\\');
                    templatePath = "~/" + templatePath;
                }

                string physicalPath = context.Server.MapPath(templatePath);
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }

                // 從資料庫刪除記錄
                OFS_ClbApplicationHelper.DeleteUploadFile(projectID, fileCode);
            }

            context.Response.Write("{\"success\":true,\"message\":\"檔案刪除成功\"}");
        }
        catch (Exception ex)
        {
            string escapedMessage = EscapeJsonString(ex.Message);
            context.Response.Write($"{{\"success\":false,\"message\":\"檔案刪除失敗：{escapedMessage}\"}}");
        }
    }

    /// <summary>
    /// 跳脫 JSON 字串中的特殊字元
    /// </summary>
    private string EscapeJsonString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return str.Replace("\\", "\\\\")    // 反斜線
                  .Replace("\"", "\\\"")    // 雙引號
                  .Replace("\n", "\\n")     // 換行
                  .Replace("\r", "\\r")     // 回車
                  .Replace("\t", "\\t");    // Tab
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

        // 檔案大小限制：100MB
        if (file.ContentLength > 100 * 1024 * 1024)
        {
            return "檔案大小不能超過 100MB";
        }

        // 檔案副檔名檢查
        string fileExtension = Path.GetExtension(file.FileName).ToLower();
        string[] allowedExtensions = { ".pdf", ".zip", ".doc", ".docx" };

        bool isAllowed = false;
        foreach (string ext in allowedExtensions)
        {
            if (fileExtension == ext)
            {
                isAllowed = true;
                break;
            }
        }

        if (!isAllowed)
        {
            return "僅接受 PDF、ZIP、DOC、DOCX 檔案格式";
        }

        return ""; // 驗證通過
    }

    /// <summary>
    /// 根據檔案類型取得檔案代碼（用於 StageReport）
    /// </summary>
    private string GetFileCodeByType(string fileType, string fileCode)
    {
        if (string.IsNullOrEmpty(fileType)) return "";

        if (fileType == "StageReport")
        {
            switch (fileCode)
            {
                case "1":
                    return "StageReport1"; // 成果報告書_初版
                default:
                    return "StageReport1";
            }
        }
        else if (fileType == "Payment")
        {
            switch (fileCode)
            {
                case "1":
                    return "PaymentIncomeStatement";
                case "2":
                    return "PaymentSubsidyList";
                case "3":
                    return "PaymentCostAllocation";
                case "4":
                    return "PaymentVouchers";
                case "5":
                    return "PaymentReceipts";
                default:
                    return "";
            }
        }
        else if (fileType == "Application")
        {
            // 對於申請表的檔案上傳，直接使用傳入的 fileCode
            return fileCode;
        }

        return"";
    }

    /// <summary>
    /// 根據檔案類型取得附件名稱
    /// </summary>
    private string GetAttachmentNameByFileType(string fileType,string fileCode)
    {
     if (string.IsNullOrEmpty(fileType)) return "";
     
             if (fileType == "StageReport")
             {
                 switch (fileCode)
                 {
                     case "StageReport":
                         return "成果報告書_初版"; // 成果報告書_初版
                     default:
                         return "成果報告書_初版";
                 }
             }
             else if (fileType == "Payment")
             {
                 switch (fileCode)
                 {
                     case "PaymentIncomeStatement":
                         return "收支明細表";
                     case "PaymentSubsidyList":
                         return "受補助清單";
                     case "PaymentCostAllocation":
                         return "經費分攤表";
                     case "PaymentVouchers":
                         return "憑證";
                     case "PaymentReceipts":
                         return "領據（含帳戶資料）";
                     default:
                         return "";
                 }
             }
            else if (fileType == "Application")
            {
                switch (fileCode)
                {
                    case "FILE_CLB1":
                        return "申請表";
                    case "FILE_CLB2":
                        return "計畫書";
                    case "FILE_CLB3":
                        return "切結書";
                    case "FILE_CLB4":
                        return "其他附件";
                    default:
                        return "附件";
                }
            }
     
             return"";
    }



    /// <summary>
    /// 儲存上傳的檔案到指定路徑
    /// </summary>
    private string SaveUploadedFile(HttpContext context, HttpPostedFile file, string fileName, string projectID)
    {
        try
        {
            // 建立目錄路徑
            string uploadDir = $"~/UploadFiles/OFS/CLB/{projectID}";
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
            return $"UploadFiles/OFS/CLB/{projectID}/{fileName}";
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
            var uploadFile = new OFS_CLB_UploadFile
            {
                ProjectID = projectID,
                FileCode = fileCode,
                FileName = fileName,
                TemplatePath = relativePath
            };

            // 檢查是否已存在同樣的記錄，如果有就刪除
            OFS_ClbApplicationHelper.DeleteUploadFile(projectID, fileCode);

            // 插入新記錄
            OFS_ClbApplicationHelper.InsertUploadFile(uploadFile);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存檔案資訊失敗：{ex.Message}");
        }
    }

    public bool IsReusable => false;
}
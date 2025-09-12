<%@ WebHandler Language="C#" Class="DownloadApplicationChecklistFile" %>

using System;
using System.Web;
using System.IO;

public class DownloadApplicationChecklistFile : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var action = context.Request.QueryString["action"];
        var projectId = context.Request.QueryString["projectId"];

        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(projectId))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing required parameters.");
            return;
        }

        try
        {
            switch (action)
            {
                case "downloadTechReview":
                    DownloadTechReviewFile(context, projectId);
                    break;
                default:
                    context.Response.StatusCode = 400;
                    context.Response.Write("Invalid action parameter.");
                    break;
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"Error processing request: {ex.Message}");
        }
    }

    private void DownloadTechReviewFile(HttpContext context, string projectId)
    {
        // 判斷專案類型
        string projectType = GetProjectTypeFromId(projectId);
        if (string.IsNullOrEmpty(projectType))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("無法識別專案類型");
            return;
        }

        string uploadFolder = context.Server.MapPath($"~/UploadFiles/OFS/{projectType}/{projectId}/TechReviewFiles/");
        
        if (!Directory.Exists(uploadFolder))
        {
            context.Response.StatusCode = 404;
            context.Response.Write("檔案不存在");
            return;
        }

        // 尋找對應的檔案
        string[] files = Directory.GetFiles(uploadFolder, $"{projectId}_TechReview_*");
        
        if (files.Length == 0)
        {
            context.Response.StatusCode = 404;
            context.Response.Write("找不到對應的檔案");
            return;
        }

        // 取得最新的檔案
        string filePath = files[0];
        string fileName = Path.GetFileName(filePath);
        
        // 產生下載檔案名稱
        string extension = Path.GetExtension(fileName);
        string downloadFileName = $"{projectId}_技術審查檔案{extension}";

        // 處理中文檔名編碼問題
        string encodedFileName = System.Web.HttpUtility.UrlEncode(downloadFileName, System.Text.Encoding.UTF8);

        // 設定回應標頭
        string contentType = GetContentType(extension);
        context.Response.Clear();
        context.Response.ContentType = contentType;
        context.Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
        context.Response.AddHeader("Content-Length", new FileInfo(filePath).Length.ToString());
        context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.AddHeader("Pragma", "no-cache");
        context.Response.AddHeader("Expires", "0");

        // 讀取檔案並寫入 Response
        byte[] fileData = File.ReadAllBytes(filePath);
        context.Response.BinaryWrite(fileData);
        context.Response.Flush();
        context.Response.SuppressContent = true;
        System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
    }

    /// <summary>
    /// 從專案ID判斷專案類型
    /// </summary>
    private static string GetProjectTypeFromId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            return null;

        if (projectId.Contains("SCI"))
            return "SCI";
        else if (projectId.Contains("CUL"))
            return "CUL";
        
        return null; // 目前只支援 SCI 和 CUL
    }

    /// <summary>
    /// 取得檔案的 MIME 類型
    /// </summary>
    private static string GetContentType(string extension)
    {
        switch (extension.ToLower())
        {
            case ".ppt":
                return "application/vnd.ms-powerpoint";
            case ".pptx":
                return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            default:
                return "application/octet-stream";
        }
    }

    public bool IsReusable => false;
}
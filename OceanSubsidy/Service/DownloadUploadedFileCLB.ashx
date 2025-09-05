<%@ WebHandler Language="C#" Class="DownloadUploadedFileCLB" %>

using System;
using System.Web;
using System.IO;

public class DownloadUploadedFileCLB : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var projectID = context.Request.QueryString["projectID"];
        var fileCode = context.Request.QueryString["fileCode"];

        if (string.IsNullOrEmpty(projectID))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing projectID parameter.");
            return;
        }

        if (string.IsNullOrEmpty(fileCode))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing fileCode parameter.");
            return;
        }

        try
        {
            // 從資料庫取得檔案資訊
            var uploadedFile = OFS_ClbApplicationHelper.GetUploadedFile(projectID, fileCode);
            
            if (uploadedFile == null)
            {
                context.Response.StatusCode = 404;
                context.Response.Write("File not found in database.");
                return;
            }

            // 建構檔案的完整路徑
            string filePath = context.Server.MapPath(uploadedFile.TemplatePath);
            
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write($"Physical file not found: {uploadedFile.FileName}");
                return;
            }

            // 設置下載響應 (針對 PDF 檔案)
            // 處理中文檔名編碼問題
            string encodedFileName = System.Web.HttpUtility.UrlEncode(uploadedFile.FileName, System.Text.Encoding.UTF8);
            
            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
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
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"Error processing request: {ex.Message}");
        }
    }

    public bool IsReusable => false;
}
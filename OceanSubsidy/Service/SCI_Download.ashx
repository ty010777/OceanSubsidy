<%@ WebHandler Language="C#" Class="SCI_Download" %>

using System;
using System.Web;
using System.IO;
using GS.App;

public class SCI_Download : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var action = context.Request.QueryString["action"];

        if (string.IsNullOrEmpty(action))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing action parameter. Use 'downloadPlan' or 'downloadApprovedPlan'.");
            return;
        }

        try
        {
            switch (action.ToLower())
            {
                case "downloadplan":
                    DownloadPlan(context);
                    break;
                case "downloadapprovedplan":
                    DownloadApprovedPlan(context);
                    break;
                default:
                    context.Response.StatusCode = 400;
                    context.Response.Write("Invalid action. Use 'downloadPlan' or 'downloadApprovedPlan'.");
                    break;
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"Error processing request: {ex.Message}");
        }
    }

    /// <summary>
    /// 下載計畫書
    /// 路徑格式：~\UploadFiles\OFS\SCI\{ProjectID}\SciApplication\{ProjectID}_科專_{ProjectName}_送審版.pdf
    /// </summary>
    private void DownloadPlan(HttpContext context)
    {
        var projectID = context.Request.QueryString["projectID"];
        var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
        string ProjectName = applicationMain.ProjectNameTw ?? "";
                        
        // 驗證參數
        if (string.IsNullOrEmpty(projectID))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing projectID parameter.");
            return;
        }
        
        // 構建檔案路徑
        string fileName = $"{projectID}_科專_{ProjectName}_送審版.pdf";
        string relativePath = $"~/UploadFiles/OFS/SCI/{projectID}/SciApplication/{fileName}";
        string filePath = context.Server.MapPath(relativePath);

        // 檢查檔案是否存在
        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Plan file not found: {fileName}");
            return;
        }

        // 下載檔案
        DownloadFile(context, filePath, fileName, "application/pdf");
    }

    /// <summary>
    /// 下載核定版計畫書
    /// 路徑格式：~\UploadFiles\OFS\SCI\{ProjectID}\SciApplication\{ProjectID}_科專_{ProjectName}_核定版.pdf
    /// </summary>
    private void DownloadApprovedPlan(HttpContext context)
    {
        var projectID = context.Request.QueryString["projectID"];
        var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
        string ProjectName = applicationMain.ProjectNameTw ?? "";
                        
        // 驗證參數
        if (string.IsNullOrEmpty(projectID))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing projectID parameter.");
            return;
        }

        // 構建檔案路徑
        string fileName = $"{projectID}_科專_{ProjectName}_核定版.pdf";
        string relativePath = $"~/UploadFiles/OFS/SCI/{projectID}/SciApplication/{fileName}";
        string filePath = context.Server.MapPath(relativePath);

        // 檢查檔案是否存在
        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Approved plan file not found: {fileName}");
            return;
        }

        // 下載檔案
        DownloadFile(context, filePath, fileName, "application/pdf");
    }

    
   

    /// <summary>
    /// 執行檔案下載
    /// </summary>
    private void DownloadFile(HttpContext context, string filePath, string fileName, string contentType)
    {
        // 處理中文檔名編碼問題
        string encodedFileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);

        context.Response.Clear();
        context.Response.ContentType = contentType;
        context.Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
        context.Response.AddHeader("Content-Length", new FileInfo(filePath).Length.ToString());
        context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.AddHeader("Pragma", "no-cache");
        context.Response.AddHeader("Expires", "0");

        // 使用 TransmitFile 來提高大檔案的效能
        try
        {
            context.Response.TransmitFile(filePath);
            context.Response.Flush();
            context.Response.SuppressContent = true;
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (Exception)
        {
            // 如果 TransmitFile 失敗，則使用 BinaryWrite
            byte[] fileData = File.ReadAllBytes(filePath);
            context.Response.BinaryWrite(fileData);
            context.Response.Flush();
            context.Response.SuppressContent = true;
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }

    public bool IsReusable => false;
}
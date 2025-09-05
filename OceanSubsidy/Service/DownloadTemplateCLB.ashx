<%@ WebHandler Language="C#" Class="DownloadTemplateCLB" %>

using System;
using System.Web;
using System.IO;

public class DownloadTemplateCLB : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var templateType = context.Request.QueryString["type"];
        var projectID = context.Request.QueryString["projectID"];

        if (string.IsNullOrEmpty(templateType))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing template type parameter.");
            return;
        }

        try
        {
            string subsidyType = GetSubsidyType(projectID);
            string fileName = GetTemplateFileName(templateType, subsidyType);
            string templatePath = context.Server.MapPath($"~/Template/CLB/{fileName}");

            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write($"Template file not found: {fileName}");
                return;
            }

            // 設置下載響應 (針對 Word 檔案)
            // 處理中文檔名編碼問題
            string encodedFileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
            
            context.Response.Clear();
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            context.Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
            context.Response.AddHeader("Content-Length", new FileInfo(templatePath).Length.ToString());
            context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Expires", "0");
            
            // 讀取檔案並寫入 Response
            byte[] fileData = File.ReadAllBytes(templatePath);
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

    private string GetSubsidyType(string projectID)
    {
        // 如果沒有 projectID，預設為 Admin (創社,社務)
        if (string.IsNullOrEmpty(projectID))
        {
            return "Admin";
        }

        try
        {
            var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
            return basicData?.SubsidyType ?? "Admin";
        }
        catch
        {
            // 如果查詢失敗，預設為 Admin
            return "Admin";
        }
    }

    private string GetTemplateFileName(string templateType, string subsidyType)
    {
        switch (templateType)
        {
            case "1": // 申請表
                if (subsidyType == "Public")
                    return "1-申請表_公共活動費.docx";
                else
                    return "1-申請表_創社,社務.docx";

            case "2": // 計畫書
                if (subsidyType == "Public")
                    return "2-計畫書_公共活動費.docx";
                else
                    return "2-計畫書_創社,社務.docx";

            case "3": // 切結書
                return "3-未違反公職人員利益衝突迴避法切結書及事前揭露表.docx";

            default:
                throw new ArgumentException($"Invalid template type: {templateType}");
        }
    }

    public bool IsReusable => false;
}
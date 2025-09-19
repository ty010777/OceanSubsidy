<%@ WebHandler Language="C#" Class="CLB_download" %>

using System;
using System.Web;
using System.IO;

public class CLB_download : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var action = context.Request.QueryString["action"];
        
        if (string.IsNullOrEmpty(action))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing action parameter. Use 'template' or 'file'.");
            return;
        }

        try
        {
            switch (action.ToLower())
            {
                case "template":
                    DownloadTemplate(context);
                    break;
                case "file":
                    DownloadUploadedFile(context);
                    break;
                case "downloadplan":
                    DownloadApplicationPlan(context);
                    break;
                case "downloadapprovedplan":
                    DownloadApprovedPlan(context);
                    break;
                default:
                    context.Response.StatusCode = 400;
                    context.Response.Write("Invalid action. Use 'template', 'file', 'downloadPlan' or 'downloadApprovedPlan'.");
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
    /// 下載範本檔案
    /// </summary>
    private void DownloadTemplate(HttpContext context)
    {
        var templateType = context.Request.QueryString["type"];
        var fileType = context.Request.QueryString["fileType"];
        var projectID = context.Request.QueryString["projectID"];

        if (string.IsNullOrEmpty(templateType))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing template type parameter.");
            return;
        }

        string subsidyType = GetSubsidyType(projectID);
        string fileName = GetTemplateFileName(templateType, subsidyType, fileType);
        string templatePath = context.Server.MapPath($"~/Template/CLB/{fileName}");
 
        if (!File.Exists(templatePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Template file not found: {fileName}");
            return;
        }
        DownloadFile(context, templatePath, fileName, GetContentType(fileName));
    }

    /// <summary>
    /// 下載已上傳的檔案
    /// </summary>
    private void DownloadUploadedFile(HttpContext context)
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

        // 從資料庫取得檔案資訊
        var uploadedFile = OFS_ClbApplicationHelper.GetUploadedFile(projectID, fileCode);
        
        if (uploadedFile == null)
        {
            context.Response.StatusCode = 404;
            context.Response.Write("File not found in database.");
            return;
        }

        // 處理路徑格式
        string templatePath = uploadedFile.TemplatePath;
        if (!templatePath.StartsWith("~/"))
        {
            templatePath = "~/" + templatePath;
        }
        string filePath = context.Server.MapPath(templatePath);
        
        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Physical file not found: {uploadedFile.FileName}");
            return;
        }

        DownloadFile(context, filePath, uploadedFile.FileName, GetContentType(uploadedFile.FileName));
    }

    /// <summary>
    /// 下載申請資料
    /// 路徑格式：~\UploadFiles\OFS\CLB\{ProjectID}\{ProjectID}_社團_{ProjectName}_送審版.pdf
    /// </summary>
    private void DownloadApplicationPlan(HttpContext context)
    {
        var projectID = context.Request.QueryString["projectID"];

        // 驗證參數
        if (string.IsNullOrEmpty(projectID))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing projectID parameter.");
            return;
        }
        var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
        string ProjectName = basicData.ProjectNameTw; 


        // 構建檔案路徑
        string fileName = $"{projectID}_社團_{ProjectName}_送審版.pdf";
        string relativePath = $"~/UploadFiles/OFS/CLB/{projectID}/{fileName}";
        string filePath = context.Server.MapPath(relativePath);

        // 檢查檔案是否存在
        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Application plan file not found: {fileName}");
            return;
        }

        // 下載檔案
        DownloadFile(context, filePath, fileName, "application/pdf");
    }

    /// <summary>
    /// 下載核定計畫書
    /// 路徑格式：~\UploadFiles\OFS\CLB\{ProjectID}\{ProjectID}_社團_{ProjectName}_核定版.pdf
    /// </summary>
    private void DownloadApprovedPlan(HttpContext context)
    {
        var projectID = context.Request.QueryString["projectID"];
        var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
        string ProjectName = basicData.ProjectNameTw; 

        // 驗證參數
        if (string.IsNullOrEmpty(projectID))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing projectID parameter.");
            return;
        }

      

        // 構建檔案路徑
        string fileName = $"{projectID}_社團_{ProjectName}_核定版.pdf";
        string relativePath = $"~/UploadFiles/OFS/CLB/{projectID}/{fileName}";
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
        
        // 讀取檔案並寫入 Response
        byte[] fileData = File.ReadAllBytes(filePath);
        context.Response.BinaryWrite(fileData);
        context.Response.Flush();
        context.Response.SuppressContent = true;
        System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
    }

    /// <summary>
    /// 取得補助類型
    /// </summary>
    private string GetSubsidyType(string projectID)
    {
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
            return "Admin";
        }
    }


    /// <summary>
    /// 取得範本檔案名稱
    /// </summary>
    private string GetTemplateFileName(string templateType, string subsidyType, string fileType = null)
    {
        // 處理請款範本
        if (templateType == "payment" && !string.IsNullOrEmpty(fileType))
        {
            switch (fileType)
            {
                case "1": // 收支明細表
                    return "收支明細表.docx";
                case "2": // 受補助清單
                    return "受補助清單.docx";
                case "3": // 經費分攤表
                    return "經費分攤表.docx";
                case "4": // 憑證
                    return "憑證.docx";
                case "5": // 領據（含帳戶資料）
                    return "領據.docx";
                default:
                    throw new ArgumentException($"Invalid payment template fileType: {fileType}");
            }
        }

        // 處理一般申請範本
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

            case "report": // 成果報告書
                return "社團-成果報告書.docx";

            default:
                throw new ArgumentException($"Invalid template type: {templateType}");
        }
    }

    /// <summary>
    /// 根據檔案名稱取得 Content-Type
    /// </summary>
    private string GetContentType(string fileName)
    {
        string fileExtension = Path.GetExtension(fileName).ToLower();
        switch (fileExtension)
        {
            case ".docx":
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            case ".pdf":
                return "application/pdf";
            case ".zip":
                return "application/zip";
            default:
                return "application/octet-stream";
        }
    }

    public bool IsReusable => false;
}
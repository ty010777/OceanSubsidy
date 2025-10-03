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

        // 🔧 對範本進行加工處理（填入專案資料等）
        string processedFilePath = ProcessDynamicTemplate(templateType, templatePath, projectID, subsidyType, fileType);

        // 下載加工後的檔案
        DownloadFile(context, processedFilePath, fileName, GetContentType(fileName));

        // 清理暫存檔案
        CleanupTempFile(processedFilePath);
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

    #region 範本加工處理

    /// <summary>
    /// 處理需要動態內容的範本檔案
    /// </summary>
    private string ProcessDynamicTemplate(string templateType, string originalFilePath, string projectID, string subsidyType, string fileType)
    {
        // 如果 projectID 為空，代表是系統管理員下載空白範本，不需要加工
        if (string.IsNullOrEmpty(projectID))
        {
            return originalFilePath;
        }

        // 根據 templateType 決定是否需要加工
        switch (templateType)
        {
            case "1": // 申請表
                return ProcessApplicationForm(originalFilePath, projectID, subsidyType);
            case "2": // 計畫書
                return ProcessPlanForm(originalFilePath, projectID, subsidyType);
            case "3": // 切結書
                return ProcessAffidavitForm(originalFilePath, projectID);
            case "report": // 成果報告書
                return ProcessReportForm(originalFilePath, projectID);
            case "payment": // 請款範本
                return ProcessPaymentForm(originalFilePath, projectID, fileType);
            default:
                // 不需要動態處理的範本直接返回原路徑
                return originalFilePath;
        }
    }

    /// <summary>
    /// 加工申請表（填入專案資料）
    /// </summary>
    private string ProcessApplicationForm(string originalFilePath, string projectID, string subsidyType)
    {
        try
        {
            // 建立暫存檔案路徑
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(originalFilePath));

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // TODO: 使用 OpenXmlHelper 處理 Word 文件，填入專案資料
            // 例如：替換 {{ProjectName}}, {{OrgName}} 等佔位符
            // var helper = new OpenXmlHelper(tempFilePath);
            // var placeholder = new Dictionary<string, string>();
            // placeholder.Add("{{ProjectName}}", projectData.ProjectName);
            // helper.GenerateWord(placeholder, new List<Dictionary<string, string>>());

            return tempFilePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessApplicationForm Error: {ex.Message}");
            return originalFilePath; // 如果處理失敗，返回原檔案路徑
        }
    }

    /// <summary>
    /// 加工計畫書（填入專案資料）
    /// </summary>
    private string ProcessPlanForm(string originalFilePath, string projectID, string subsidyType)
    {
        try
        {
            // 建立暫存檔案路徑
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(originalFilePath));

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // TODO: 加工邏輯
            // 填入專案相關資料

            return tempFilePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessPlanForm Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 加工切結書（填入專案資料）
    /// </summary>
    private string ProcessAffidavitForm(string originalFilePath, string projectID)
    {
        try
        {
            // 建立暫存檔案路徑
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(originalFilePath));

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // TODO: 加工邏輯
            // 填入組織名稱、日期等

            return tempFilePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessAffidavitForm Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 加工成果報告書（填入專案資料）
    /// </summary>
    private string ProcessReportForm(string originalFilePath, string projectID)
    {
        try
        {
            // 建立暫存檔案路徑
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(originalFilePath));

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // TODO: 加工邏輯

            return tempFilePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessReportForm Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 加工請款範本（填入專案資料）
    /// </summary>
    private string ProcessPaymentForm(string originalFilePath, string projectID, string fileType)
    {
        try
        {
            // 建立暫存檔案路徑
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(originalFilePath));

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // TODO: 根據 fileType 進行不同的加工
            // fileType: 1=收支明細表, 2=受補助清單, 3=經費分攤表, 4=憑證, 5=領據

            return tempFilePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessPaymentForm Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 清理暫存檔案
    /// </summary>
    private void CleanupTempFile(string filePath)
    {
        try
        {
            // 判斷是否為暫存檔案（在 Temp 目錄中的檔案）
            if (!string.IsNullOrEmpty(filePath) &&
                filePath.Contains(Path.GetTempPath()) &&
                File.Exists(filePath))
            {
                File.Delete(filePath);
                System.Diagnostics.Debug.WriteLine($"已清理暫存檔案：{filePath}");
            }
        }
        catch (Exception ex)
        {
            // 清理失敗不影響主要功能，只記錄錯誤
            System.Diagnostics.Debug.WriteLine($"清理暫存檔案失敗：{ex.Message}");
        }
    }

    #endregion

    public bool IsReusable => false;
}
<%@ WebHandler Language="C#" Class="CLB_download" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.IO;
using GS.OCA_OceanSubsidy.Entity;

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

        // 如果路徑不是以 ~/ 開頭，則加上
        if (!templatePath.StartsWith("~/"))
        {
            // 移除開頭的 / 或 \ 避免重複
            templatePath = templatePath.TrimStart('/', '\\');
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

        // 驗證參數
        if (string.IsNullOrEmpty(projectID))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing projectID parameter.");
            return;
        }

        try
        {
            var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);

            if (basicData == null)
            {
                context.Response.StatusCode = 404;
                context.Response.Write($"Project not found in database: {projectID}");
                return;
            }

            string ProjectName = basicData.ProjectNameTw;

            // 構建檔案路徑 - 使用與 SCI 相同的標準格式（不使用 AppRootPath）
            string fileName = $"{projectID}_社團_{ProjectName}_核定版.pdf";
            string relativePath = $"~/UploadFiles/OFS/CLB/{projectID}/{fileName}";
            string filePath = context.Server.MapPath(relativePath);

            // 詳細的錯誤診斷
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.Write($"File not found\n");
                context.Response.Write($"ProjectID: {projectID}\n");
                context.Response.Write($"ProjectName: {ProjectName}\n");
                context.Response.Write($"Expected file name: {fileName}\n");
                context.Response.Write($"Relative path: {relativePath}\n");
                context.Response.Write($"Physical path: {filePath}\n");
                context.Response.Write($"\n");

                // 列出目錄中的檔案
                string dirPath = Path.GetDirectoryName(filePath);
                if (Directory.Exists(dirPath))
                {
                    context.Response.Write($"Directory exists. Files in directory:\n");
                    foreach (string file in Directory.GetFiles(dirPath))
                    {
                        context.Response.Write($"  - {Path.GetFileName(file)}\n");
                    }
                }
                else
                {
                    context.Response.Write($"Directory does not exist: {dirPath}\n");
                }

                return;
            }

            // 下載檔案
            DownloadFile(context, filePath, fileName, "application/pdf");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Write($"Error: {ex.Message}\n");
            context.Response.Write($"Stack trace: {ex.StackTrace}");
        }
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

        OFS_CLB_Application_Plan plan = OFS_ClbApplicationHelper.GetPlanData(projectID);
        List<OFS_CLB_Application_Personnel> Personnel = OFS_ClbApplicationHelper.GetPersonnelData(projectID);
        OFS_CLB_Application_Funds Funds = OFS_ClbApplicationHelper.GetFundsData(projectID);
        OFS_CLB_Application_Basic Basic = OFS_ClbApplicationHelper.GetBasicData(projectID);
        // 根據 templateType 決定是否需要加工
        switch (templateType)
        {
            case "1": // 申請表
                return ProcessApplicationForm(originalFilePath, subsidyType, plan, Personnel, Funds, Basic);
            case "2": // 計畫書
                return ProcessPlanForm(originalFilePath, subsidyType, plan, Personnel, Funds, Basic);
            default:
                // 不需要動態處理的範本直接返回原路徑
                return originalFilePath;
        }
    }

    /// <summary>
    /// 加工申請表（填入專案資料）
    /// </summary>
    private string ProcessApplicationForm(string originalFilePath, string subsidyType,OFS_CLB_Application_Plan  plan,List<OFS_CLB_Application_Personnel> Personnel ,OFS_CLB_Application_Funds Funds ,OFS_CLB_Application_Basic Basic)
    {
        
        try
        {
            // 建立暫存檔案路徑
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(originalFilePath));

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            if (subsidyType == "Public")
            {
                // 公共活動費申請表加工
                using (var docHelper = new GS.OCA_OceanSubsidy.Operation.OSI.OpenXml.OpenXmlHelper(tempFilePath))
                {
                    var replacements = new Dictionary<string, string>();

                    replacements.Add("SchoolName", Basic?.SchoolName ?? "");
                    replacements.Add("ClubName", Basic?.ClubName ?? "");
                    string creationDate = GS.App.DateTimeHelper.ToMinguoDate(Basic.CreationDate);
                    replacements.Add("CreationDate", creationDate??"");                    
                    replacements.Add("ProjectName", Basic?.ProjectNameTw ?? "");
                    replacements.Add("Address", Basic?.Address ?? "");
                    replacements.Add("IDNumber", Basic?.School_IDNumber ?? "");
                    // Personnel[0] - 指導老師
                    replacements.Add("teacherName", Personnel != null && Personnel.Count > 0 ? (Personnel[0]?.Name ?? "") : "");
                    replacements.Add("teacherTitle", Personnel != null && Personnel.Count > 0 ? (Personnel[0]?.JobTitle ?? "") : "");
                    replacements.Add("teacherPhone", Personnel != null && Personnel.Count > 0 ? (Personnel[0]?.PhoneNum ?? "") : "");

                    // Personnel[1] - 聯絡人
                    replacements.Add("contactName", Personnel != null && Personnel.Count > 1 ? (Personnel[1]?.Name ?? "") : "");
                    replacements.Add("contactTitle", Personnel != null && Personnel.Count > 1 ? (Personnel[1]?.JobTitle ?? "") : "");
                    replacements.Add("contactPhone", Personnel != null && Personnel.Count > 1 ? (Personnel[1]?.PhoneNum ?? "") : "");
                    // Plan.StartDate - 開始日期（民國年）
                    replacements.Add("SYear", plan?.StartDate != null ? GS.App.DateTimeHelper.GregorianYearToMinguo(plan.StartDate.Value.Year).ToString() : "");
                    replacements.Add("SMonth", plan?.StartDate?.Month.ToString() ?? "");
                    replacements.Add("SDay", plan?.StartDate?.Day.ToString() ?? "");

                    // Plan.EndDate - 結束日期（民國年）
                    replacements.Add("EYear", plan?.EndDate != null ? GS.App.DateTimeHelper.GregorianYearToMinguo(plan.EndDate.Value.Year).ToString() : "");
                    replacements.Add("EMonth", plan?.EndDate?.Month.ToString() ?? "");
                    replacements.Add("EDay", plan?.EndDate?.Day.ToString() ?? "");
                    
                    replacements.Add("PlanContent", plan?.PlanContent ?? "");
                    replacements.Add("Benefits", plan?.PreBenefits ?? "");
                    
                    replacements.Add("SubsidyFunds", Funds?.SubsidyFunds != null ? ((int)Funds.SubsidyFunds).ToString() : "");
                    replacements.Add("SelfFunds", Funds?.SelfFunds != null ? ((int)Funds.SelfFunds).ToString() : "");
                    replacements.Add("OtherGovFunds", Funds?.OtherGovFunds != null ? ((int)Funds.OtherGovFunds).ToString() : "");
                    replacements.Add("OtherUnitFunds", Funds?.OtherUnitFunds != null ? ((int)Funds.OtherUnitFunds).ToString() : "");
                    replacements.Add("TotalFunds", Funds?.TotalFunds != null ? ((int)Funds.TotalFunds).ToString() : "");

                    // Funds.PreviouslySubsidized - 過去是否受補助
                    bool previouslySubsidized = Funds?.PreviouslySubsidized ?? false;
                    string FundingDescription = Funds?.FundingDescription ?? "";
                    replacements.Add("yesSubsidy", previouslySubsidized ? "☒" : "☐");
                    replacements.Add("noSubsidy", previouslySubsidized ? "☐" : "☒");
                    replacements.Add("FundingDescription", FundingDescription);

                    // 今日日期（民國年）
                    DateTime today = DateTime.Now;
                    replacements.Add("TYear", GS.App.DateTimeHelper.GregorianYearToMinguo(today.Year).ToString());
                    replacements.Add("TMonth", today.Month.ToString());
                    replacements.Add("TDay", today.Day.ToString());
             
                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());
                }
            }
            else
            {
                // 創社、社務申請表加工
                using (var docHelper = new GS.OCA_OceanSubsidy.Operation.OSI.OpenXml.OpenXmlHelper(tempFilePath))
                {
                    // 建立替換字典
                    var replacements = new Dictionary<string, string>();

                    string societyName = (Basic?.SchoolName ?? "") + (Basic?.ClubName ?? "");
                    replacements.Add("SocietyName", societyName);
                    replacements.Add("ProjectName", Basic?.ProjectNameTw ?? "");
                    // Personnel[0] - 指導老師
                    replacements.Add("teacherName", Personnel != null && Personnel.Count > 0 ? (Personnel[0]?.Name ?? "") : "");
                    replacements.Add("teacherTitle", Personnel != null && Personnel.Count > 0 ? (Personnel[0]?.JobTitle ?? "") : "");
                    replacements.Add("teacherPhone", Personnel != null && Personnel.Count > 0 ? (Personnel[0]?.PhoneNum ?? "") : "");

                    // Personnel[1] - 聯絡人
                    replacements.Add("contactName", Personnel != null && Personnel.Count > 1 ? (Personnel[1]?.Name ?? "") : "");
                    replacements.Add("contactTitle", Personnel != null && Personnel.Count > 1 ? (Personnel[1]?.JobTitle ?? "") : "");
                    replacements.Add("contactPhone", Personnel != null && Personnel.Count > 1 ? (Personnel[1]?.PhoneNum ?? "") : "");
                    
                    // Plan.StartDate - 開始日期（民國年）
                    replacements.Add("SYear", plan?.StartDate != null ? GS.App.DateTimeHelper.GregorianYearToMinguo(plan.StartDate.Value.Year).ToString() : "");
                    replacements.Add("SMonth", plan?.StartDate?.Month.ToString() ?? "");
                    replacements.Add("SDay", plan?.StartDate?.Day.ToString() ?? "");

                    // Plan.EndDate - 結束日期（民國年）
                    replacements.Add("EYear", plan?.EndDate != null ? GS.App.DateTimeHelper.GregorianYearToMinguo(plan.EndDate.Value.Year).ToString() : "");
                    replacements.Add("EMonth", plan?.EndDate?.Month.ToString() ?? "");
                    replacements.Add("EDay", plan?.EndDate?.Day.ToString() ?? "");
                    
                    replacements.Add("PlanContent", plan?.PlanContent ?? "");
                    replacements.Add("Benefits", plan?.PreBenefits ?? "");
                    
                    
                    replacements.Add("SubsidyFunds", Funds?.SubsidyFunds != null ? ((int)Funds.SubsidyFunds).ToString() : "");
                    replacements.Add("SelfFunds", Funds?.SelfFunds != null ? ((int)Funds.SelfFunds).ToString() : "");
                    replacements.Add("OtherGovFunds", Funds?.OtherGovFunds != null ? ((int)Funds.OtherGovFunds).ToString() : "");
                    replacements.Add("OtherUnitFunds", Funds?.OtherUnitFunds != null ? ((int)Funds.OtherUnitFunds).ToString() : "");
                    replacements.Add("TotalFunds", Funds?.TotalFunds != null ? ((int)Funds.TotalFunds).ToString() : "");

                    // Funds.PreviouslySubsidized - 過去是否受補助
                    bool previouslySubsidized = Funds?.PreviouslySubsidized ?? false;
                    string FundingDescription = Funds?.FundingDescription ?? "";
                    replacements.Add("yesSubsidy", previouslySubsidized ? "☒" : "☐");
                    replacements.Add("noSubsidy", previouslySubsidized ? "☐" : "☒");
                    replacements.Add("FundingDescription", FundingDescription);

                    // 今日日期（民國年）
                    DateTime today = DateTime.Now;
                    replacements.Add("TYear", GS.App.DateTimeHelper.GregorianYearToMinguo(today.Year).ToString());
                    replacements.Add("TMonth", today.Month.ToString());
                    replacements.Add("TDay", today.Day.ToString());

                    // Basic.SubsidyType - 補助類型勾選
                    bool isStartup = subsidyType == "Startup";
                    replacements.Add("Check1", isStartup ? "☒" : "☐");
                    replacements.Add("Check2", isStartup ? "☐" : "☒");

                    // 年度 - 今年和去年（民國年）
                    int currentRocYear = GS.App.DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year);
                    replacements.Add("Check1Year", currentRocYear.ToString());
                    replacements.Add("Check2Year", (currentRocYear - 1).ToString());

                    // 執行替換
                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());
                }
            }

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
    private string ProcessPlanForm(string originalFilePath, string subsidyType, OFS_CLB_Application_Plan plan, List<OFS_CLB_Application_Personnel> Personnel, OFS_CLB_Application_Funds Funds, OFS_CLB_Application_Basic Basic)
    {
        try
        {
            // 建立暫存檔案路徑
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(originalFilePath));

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            if (subsidyType == "Public")
            {
                // 公共活動費計畫書加工
                using (var docHelper = new GS.OCA_OceanSubsidy.Operation.OSI.OpenXml.OpenXmlHelper(tempFilePath))
                {
                    var replacements = new Dictionary<string, string>();

                    replacements.Add("SchoolName", Basic?.SchoolName ?? "");
                    replacements.Add("ProjectName", Basic?.ProjectNameTw ?? "");
                    replacements.Add("Purpose", plan.Purpose??"");

                    // Plan.StartDate - 開始日期（民國年）
                    replacements.Add("SYear", plan?.StartDate != null ? GS.App.DateTimeHelper.GregorianYearToMinguo(plan.StartDate.Value.Year).ToString() : "");
                    replacements.Add("SMonth", plan?.StartDate?.Month.ToString() ?? "");
                    replacements.Add("SDay", plan?.StartDate?.Day.ToString() ?? "");
                    
                    // Plan.EndDate - 結束日期（民國年）
                    replacements.Add("EYear", plan?.EndDate != null ? GS.App.DateTimeHelper.GregorianYearToMinguo(plan.EndDate.Value.Year).ToString() : "");
                    replacements.Add("EMonth", plan?.EndDate?.Month.ToString() ?? "");
                    replacements.Add("EDay", plan?.EndDate?.Day.ToString() ?? "");                    
                    
                    //計畫地點、預估人數
                    replacements.Add("PlanLocation", plan.PlanLocation ?? "");
                    replacements.Add("EstimatedPeople", plan.EstimatedPeople ?? "");
                    replacements.Add("PlanContent", plan.PlanContent ?? "");

                    // CreationDate - 成立日期（民國年格式）
                    string creationDate = GS.App.DateTimeHelper.ToMinguoDate(Basic.CreationDate);
                    replacements.Add("CreationDate", creationDate??"");
                    
                    replacements.Add("SubsidyFunds", Funds?.SubsidyFunds != null ? ((int)Funds.SubsidyFunds).ToString() : "");
                    replacements.Add("SelfFunds", Funds?.SelfFunds != null ? ((int)Funds.SelfFunds).ToString() : "");
                    replacements.Add("OtherGovFunds", Funds?.OtherGovFunds != null ? ((int)Funds.OtherGovFunds).ToString() : "");
                    replacements.Add("OtherUnitFunds", Funds?.OtherUnitFunds != null ? ((int)Funds.OtherUnitFunds).ToString() : "");
                    replacements.Add("TotalFunds", Funds?.TotalFunds != null ? ((int)Funds.TotalFunds).ToString() : "");
                    replacements.Add("Benefits", plan?.PreBenefits ?? "");
                    //緊急計畫
                    replacements.Add("EmergencyPlan", plan.EmergencyPlan??"");

                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());
                }
            }
            else
            {
                // 創社、社務計畫書加工
                using (var docHelper = new GS.OCA_OceanSubsidy.Operation.OSI.OpenXml.OpenXmlHelper(tempFilePath))
                {
                    var replacements = new Dictionary<string, string>();

                    // 今日日期（民國年）
                    DateTime today = DateTime.Now;
                    replacements.Add("TYear", GS.App.DateTimeHelper.GregorianYearToMinguo(today.Year).ToString());
                    replacements.Add("SchoolName", Basic?.SchoolName ?? "");
                    replacements.Add("ClubName", Basic?.ClubName ?? "");

                    // CreationDate - 成立日期（民國年格式）
                    string creationDate = GS.App.DateTimeHelper.ToMinguoDate(Basic.CreationDate);
                    replacements.Add("CreationDate", creationDate??"");
                    replacements.Add("EstimatedPeople", plan.EstimatedPeople??"");
                    replacements.Add("Purpose", plan.Purpose??"");
                    replacements.Add("SubsidyFunds", Funds?.SubsidyFunds != null ? ((int)Funds.SubsidyFunds).ToString() : "");
                    replacements.Add("SelfFunds", Funds?.SelfFunds != null ? ((int)Funds.SelfFunds).ToString() : "");
                    replacements.Add("OtherGovFunds", Funds?.OtherGovFunds != null ? ((int)Funds.OtherGovFunds).ToString() : "");
                    replacements.Add("OtherUnitFunds", Funds?.OtherUnitFunds != null ? ((int)Funds.OtherUnitFunds).ToString() : "");
                    replacements.Add("TotalFunds", Funds?.TotalFunds != null ? ((int)Funds.TotalFunds).ToString() : "");
                    replacements.Add("Benefits", plan?.PreBenefits ?? "");
                    // Basic.SubsidyType - 補助類型勾選
                    bool isStartup = subsidyType == "Startup";
                    replacements.Add("Check1", isStartup ? "☒" : "☐");
                    replacements.Add("Check2", isStartup ? "☐" : "☒");
                        
                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());
                }
            }

            return tempFilePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessPlanForm Error: {ex.Message}");
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
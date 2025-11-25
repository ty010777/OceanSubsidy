<%@ WebHandler Language="C#" Class="CLB_download" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.IO;
using System.Linq;
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
                case "filebyid":
                    DownloadUploadedFileById(context);
                    break;
                case "downloadplan":
                    DownloadApplicationPlan(context);
                    break;
                case "downloadapprovedplan":
                    DownloadApprovedPlan(context);
                    break;
                default:
                    context.Response.StatusCode = 400;
                    context.Response.Write("Invalid action. Use 'template', 'file', 'fileById', 'downloadPlan' or 'downloadApprovedPlan'.");
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
    /// 根據 ID 下載已上傳的檔案
    /// </summary>
    private void DownloadUploadedFileById(HttpContext context)
    {
        var fileIdStr = context.Request.QueryString["fileId"];

        if (string.IsNullOrEmpty(fileIdStr) || !int.TryParse(fileIdStr, out int fileId))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing or invalid fileId parameter.");
            return;
        }

        // 從資料庫取得檔案資訊
        var uploadedFile = OFS_ClbApplicationHelper.GetUploadedFileById(fileId);

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
        OFS_CLB_Application_Basic Basic = OFS_ClbApplicationHelper.GetBasicData(projectID);
        // 根據 templateType 決定是否需要加工
        switch (templateType)
        {
            case "1": // 申請表
                return ProcessApplicationForm(originalFilePath, subsidyType, plan, Personnel, Basic,projectID);
            case "2": // 計畫書
                return ProcessPlanForm(originalFilePath, subsidyType, plan, Personnel, Basic, projectID);
            default:
                // 不需要動態處理的範本直接返回原路徑
                return originalFilePath;
        }
    }

    /// <summary>
    /// 加工申請表（填入專案資料）
    /// </summary>
    private string ProcessApplicationForm(string originalFilePath, string subsidyType,OFS_CLB_Application_Plan  plan,List<OFS_CLB_Application_Personnel> Personnel, OFS_CLB_Application_Basic Basic,string projectID)
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

                    // 今日日期（民國年）
                    DateTime today = DateTime.Now;
                    replacements.Add("TYear", GS.App.DateTimeHelper.GregorianYearToMinguo(today.Year).ToString());
                    replacements.Add("TMonth", today.Month.ToString());
                    replacements.Add("TDay", today.Day.ToString());

                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());

                    // 計算經費金額（用於整合表格）
                    int subsidyAmount = Basic?.ApplyAmount != null ? (int)Basic.ApplyAmount : 0;
                    int selfAmount = Basic?.SelfAmount != null ? (int)Basic.SelfAmount : 0;
                    int otherAmount = Basic?.OtherAmount != null ? (int)Basic.OtherAmount : 0;

                    // Insert integrated funds table
                    string integratedFundsTableHtml = GenerateIntegratedFundsTableHtml(subsidyAmount, selfAmount, otherAmount, projectID);
                    if (!string.IsNullOrEmpty(integratedFundsTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{IntegratedFundsTable}}", integratedFundsTableHtml);
                    }

                    // Insert received subsidy table
                    string receivedSubsidyTableHtml = GenerateReceivedSubsidyTableHtml(projectID);
                    if (!string.IsNullOrEmpty(receivedSubsidyTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{ReceivedSubsidyTable}}", receivedSubsidyTableHtml);
                    }
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

                    // 今日日期（民國年）
                    DateTime today = DateTime.Now;
                    replacements.Add("TYear", GS.App.DateTimeHelper.GregorianYearToMinguo(today.Year).ToString());
                    replacements.Add("TMonth", today.Month.ToString());
                    replacements.Add("TDay", today.Day.ToString());

                    // Basic.SubsidyType - 補助類型勾選
                    bool isStartup = subsidyType == "Startup";
                    replacements.Add("Check1", isStartup ? "■" : "□");
                    replacements.Add("Check2", isStartup ? "□" : "■");

                    // 年度 - 今年和去年（民國年）
                    int currentRocYear = GS.App.DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year);
                    replacements.Add("CheckYear1", currentRocYear.ToString());
                    replacements.Add("CheckYear2", (currentRocYear - 1).ToString());

                    // 執行替換
                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());

                    // 計算經費金額（用於整合表格）
                    int subsidyAmount = Basic?.ApplyAmount != null ? (int)Basic.ApplyAmount : 0;
                    int selfAmount = Basic?.SelfAmount != null ? (int)Basic.SelfAmount : 0;
                    int otherAmount = Basic?.OtherAmount != null ? (int)Basic.OtherAmount : 0;

                    // Insert integrated funds table
                    string integratedFundsTableHtml = GenerateIntegratedFundsTableHtml(subsidyAmount, selfAmount, otherAmount, projectID);
                    if (!string.IsNullOrEmpty(integratedFundsTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{IntegratedFundsTable}}", integratedFundsTableHtml);
                    }

                    // Insert received subsidy table
                    string receivedSubsidyTableHtml = GenerateReceivedSubsidyTableHtml(projectID);
                    if (!string.IsNullOrEmpty(receivedSubsidyTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{ReceivedSubsidyTable}}", receivedSubsidyTableHtml);
                    }
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
    private string ProcessPlanForm(string originalFilePath, string subsidyType, OFS_CLB_Application_Plan plan, List<OFS_CLB_Application_Personnel> Personnel, OFS_CLB_Application_Basic Basic, string projectID)
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
                    
                    replacements.Add("SubsidyFunds", Basic?.ApplyAmount != null ? ((int)Basic.ApplyAmount).ToString() : "0");
                    replacements.Add("SelfFunds", Basic?.SelfAmount != null ? ((int)Basic.SelfAmount).ToString() : "0");
                    replacements.Add("OtherUnitFunds", Basic?.OtherAmount != null ? ((int)Basic.OtherAmount).ToString() : "0");
                    replacements.Add("TotalFunds", ((Basic?.ApplyAmount ?? 0) + (Basic?.SelfAmount ?? 0) + (Basic?.OtherAmount ?? 0)).ToString());
                    replacements.Add("Benefits", plan?.PreBenefits ?? "");
                    //緊急計畫
                    replacements.Add("EmergencyPlan", plan.EmergencyPlan??"");

                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());

                    // 插入經費預算表格
                    string budgetTableHtml = GenerateBudgetPlanTableHtml(projectID);
                    if (!string.IsNullOrEmpty(budgetTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{BudgetTable}}", budgetTableHtml);
                    }

                    // 插入其他機關補助表格
                    string otherSubsidyTableHtml = GenerateOtherSubsidyTableHtml(projectID);
                    if (!string.IsNullOrEmpty(otherSubsidyTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{OtherSubsidyTable}}", otherSubsidyTableHtml);
                    }
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
                    replacements.Add("SubsidyFunds", Basic?.ApplyAmount != null ? ((int)Basic.ApplyAmount).ToString() : "0");
                    replacements.Add("SelfFunds", Basic?.SelfAmount != null ? ((int)Basic.SelfAmount).ToString() : "0");
                    replacements.Add("OtherUnitFunds", Basic?.OtherAmount != null ? ((int)Basic.OtherAmount).ToString() : "0");
                    replacements.Add("TotalFunds", ((Basic?.ApplyAmount ?? 0) + (Basic?.SelfAmount ?? 0) + (Basic?.OtherAmount ?? 0)).ToString());
                    replacements.Add("Benefits", plan?.PreBenefits ?? "");
                    // Basic.SubsidyType - 補助類型勾選
                    bool isStartup = subsidyType == "Startup";
                    replacements.Add("Check1", isStartup ? "■" : "□");
                    replacements.Add("Check2", isStartup ? "□" : "■");

                    docHelper.GenerateWord(replacements, new List<Dictionary<string, string>>());

                    // 插入經費預算表格
                    string budgetTableHtml = GenerateBudgetPlanTableHtml(projectID);
                    if (!string.IsNullOrEmpty(budgetTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{BudgetTable}}", budgetTableHtml);
                    }

                    // 插入其他機關補助表格
                    string otherSubsidyTableHtml = GenerateOtherSubsidyTableHtml(projectID);
                    if (!string.IsNullOrEmpty(otherSubsidyTableHtml))
                    {
                        docHelper.InsertHtmlAsTable("{{OtherSubsidyTable}}", otherSubsidyTableHtml);
                    }
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

    /// <summary>
    /// 生成經費預算規劃表格的 HTML
    /// </summary>
    private string GenerateBudgetPlanTableHtml(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
        {
            return "";
        }

        try
        {
            // 從資料庫取得經費預算資料
            var budgetPlans = OFS_ClbApplicationHelper.GetBudgetPlanData(projectID);

            if (budgetPlans == null || budgetPlans.Count == 0)
            {
                // 無資料時返回簡單的 HTML 表格（無格線）
                return @"
<table style='border: none;'>
    <tr>
        <td style='text-align: center; vertical-align: middle; border: none;'>無經費預算資料</td>
    </tr>
</table>";
            }

            // 建立 HTML 表格（雙層表頭結構）
            var html = @"
<table>
    <tr>
        <th rowspan='2' style='text-align: center; vertical-align: middle;  width: 20%;'>預算項目</th>
        <th colspan='3' style='text-align: center; vertical-align: middle;  width: 45%;'>預算金額</th>
        <th rowspan='2' style='text-align: center; vertical-align: middle;  width: 35%;'>計算方式及說明</th>
    </tr>
    <tr>
        <th style='text-align: center; vertical-align: middle;  width: 15%;'>海洋委員會經費</th>
        <th style='text-align: center; vertical-align: middle;  width: 15%;'>其他配合經費</th>
        <th style='text-align: center; vertical-align: middle;  width: 15%;'>小計</th>
    </tr>";

            // 加入資料列
            foreach (var item in budgetPlans)
            {
                int amount = item.Amount ?? 0;
                int otherAmount = item.OtherAmount ?? 0;
                int subtotal = amount + otherAmount;

                html += $@"
    <tr>
        <td style='vertical-align: middle;'>{item.Title}</td>
        <td style='text-align: right; vertical-align: middle;'>{amount:N0}</td>
        <td style='text-align: right; vertical-align: middle;'>{otherAmount:N0}</td>
        <td style='text-align: right; vertical-align: middle;'>{subtotal:N0}</td>
        <td style='vertical-align: middle;'>{item.Description}</td>
    </tr>";
            }

            html += @"
</table>";

            return html;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateBudgetPlanTableHtml Error: {ex.Message}");
            return "";
        }
    }

    /// <summary>
    /// 生成其他機關補助/合作金額表格的 HTML
    /// </summary>
    private string GenerateOtherSubsidyTableHtml(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
        {
            return "";
        }

        try
        {
            // 從資料庫取得其他機關補助資料
            var otherSubsidies = OFS_ClbApplicationHelper.GetOtherSubsidyData(projectID);

            if (otherSubsidies == null || otherSubsidies.Count == 0)
            {
                // 無資料時返回簡單的 HTML 表格（無格線）
                return @"
<table style='border: none;'>
    <tr>
        <td style='text-align: center; vertical-align: middle; border: none;'>無其他機關補助資料</td>
    </tr>
</table>";
            }

            // 計算總金額
            int totalAmount = otherSubsidies.Sum(x => x.Amount ?? 0);

            // 建立 HTML 表格
            var html = @"
<table>
    <tr>
        <th style='text-align: center; vertical-align: middle;  width: 10%;'>序號</th>
        <th style='text-align: center; vertical-align: middle;  width: 25%;'>機關(單位)名稱</th>
        <th style='text-align: center; vertical-align: middle;  width: 20%;'>金額(千元)</th>
        <th style='text-align: center; vertical-align: middle;  width: 15%;'>比例(%)</th>
        <th style='text-align: center; vertical-align: middle;  width: 30%;'>申請合作項目</th>
    </tr>";

            // 加入資料列
            int rowIndex = 1;
            foreach (var item in otherSubsidies)
            {
                int amount = item.Amount ?? 0;
                decimal amountInThousand = amount / 1000m;  // 轉換為千元單位

                // 計算比例
                decimal percentage = totalAmount > 0 ? (decimal)amount / totalAmount * 100 : 0;

                html += $@"
    <tr>
        <td style='text-align: center; vertical-align: middle;'>{rowIndex}</td>
        <td style='vertical-align: middle;'>{item.Unit}</td>
        <td style='text-align: right; vertical-align: middle;'>{amountInThousand:F2}</td>
        <td style='text-align: right; vertical-align: middle;'>{percentage:F2}</td>
        <td style='vertical-align: middle;'>{item.Content}</td>
    </tr>";

                rowIndex++;
            }

            // 加入合計列
            decimal totalAmountInThousand = totalAmount / 1000m;  // 轉換為千元單位
            html += $@"
    <tr>
        <td colspan='2' style='text-align: center; vertical-align: middle; '><strong>合計</strong></td>
        <td style='text-align: right; vertical-align: middle; '><strong>{totalAmountInThousand:F2}</strong></td>
        <td style='text-align: right; vertical-align: middle; '><strong>100.00</strong></td>
        <td style='vertical-align: middle; '></td>
    </tr>";

            html += @"
</table>";

            return html;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateOtherSubsidyTableHtml Error: {ex.Message}");
            return "";
        }
    }

    /// <summary>
    /// 生成其他補助單位簡表（僅兩欄）的 HTML
    /// </summary>
    private string GenerateOtherSubsidySimpleTableHtml(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
        {
            return "";
        }

        try
        {
            // 從資料庫取得其他機關補助資料
            var otherSubsidies = OFS_ClbApplicationHelper.GetOtherSubsidyData(projectID);

            if (otherSubsidies == null || otherSubsidies.Count == 0)
            {
                // 無資料時返回簡單的 HTML 表格（無格線）
                return @"
<table style='border: none;'>
    <tr>
        <td style='text-align: center; vertical-align: middle; border: none;'>無其他補助單位資料</td>
    </tr>
</table>";
            }

            // 建立 HTML 表格
            var html = @"
<table>
    <tr>
        <th style='text-align: center; vertical-align: middle;  width: 50%;'>單位名稱</th>
        <th style='text-align: center; vertical-align: middle;  width: 50%;'>申請／分攤補助金額（含尚未核定者）</th>
    </tr>";

            // 加入資料列
            foreach (var item in otherSubsidies)
            {
                int amount = item.Amount ?? 0;

                html += $@"
    <tr>
        <td style='vertical-align: middle;'>{item.Unit}</td>
        <td style='text-align: right; vertical-align: middle;'>{amount:N0}</td>
    </tr>";
            }

            html += @"
</table>";

            return html;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateOtherSubsidySimpleTableHtml Error: {ex.Message}");
            return "";
        }
    }

    /// <summary>
    /// 生成整合經費表格的 HTML（包含 A/B/C 所有經費資訊和其他補助單位明細）
    /// </summary>
    private string GenerateIntegratedFundsTableHtml(int subsidyAmount, int selfAmount, int otherAmount, string projectID)
    {
        try
        {
            int totalAmount = subsidyAmount + selfAmount + otherAmount;

            // 取得其他補助單位資料
            var otherSubsidies = OFS_ClbApplicationHelper.GetOtherSubsidyData(projectID);
            int otherSubsidyCount = (otherSubsidies != null && otherSubsidies.Count > 0) ? otherSubsidies.Count : 1;

            // 計算 rowspan
            int totalRowspan = 3 + otherSubsidyCount; // A列 + B列 + C的header列 + 資料列數
            int cRowspan = 1 + otherSubsidyCount; // header列 + 資料列數

            // 建立 HTML 表格
            var html = @"
<table>
    <tr>";

            // 第一列：計畫總經費（rowspan動態）+ A + 申請本會分攤金額 + 金額
            html += $@"
        <td rowspan='{totalRowspan}' style='vertical-align: middle; width: 20%;'>
            計畫總經費（A+B+C）
            {totalAmount / 10000}萬{totalAmount % 10000}元整
        </td>
        <td style='vertical-align: middle; width: 8%;'>A</td>
        <td style='vertical-align: middle; width: 32%;'>申請本會分攤金額</td>
        <td colspan='2' style='vertical-align: middle; width: 40%;'>{subsidyAmount / 10000} 萬 {subsidyAmount % 10000} 元整</td>
    </tr>";

            // 第二列：B + 申請單位自籌款 + 金額
            html += $@"
    <tr>
        <td style='vertical-align: middle; width: 8%;'>B</td>
        <td style='vertical-align: middle; width: 32%;'>申請單位自籌款</td>
        <td colspan='2' style='vertical-align: middle; width: 40%;'>{selfAmount / 10000} 萬 {selfAmount % 10000} 元整</td>
    </tr>";

            // 第三列：C（rowspan動態）+ 其他機關總金額（rowspan動態）+ 單位名稱 header + 金額 header
            html += $@"
    <tr>
        <td rowspan='{cRowspan}' style='vertical-align: middle; width: 8%;'>C</td>
        <td rowspan='{cRowspan}' style='vertical-align: middle; width: 32%;'>
            其他機關補助/分攤總金額：
            {otherAmount / 10000}萬{otherAmount % 10000}元整
        </td>
        <td style='vertical-align: middle; width: 20%;'>單位名稱</td>
        <td style='vertical-align: middle; width: 20%;'>申請／分攤補助金額<br/>（含尚未核定者）</td>
    </tr>";

            // 第四列及之後：其他補助單位資料
            if (otherSubsidies != null && otherSubsidies.Count > 0)
            {
                foreach (var item in otherSubsidies)
                {
                    int amount = item.Amount ?? 0;
                    html += $@"
    <tr>
        <td style='vertical-align: middle;'>{item.Unit}</td>
        <td style='vertical-align: middle;'>{amount:N0}</td>
    </tr>";
                }
            }
            else
            {
                // 無資料時顯示空白列
                html += @"
    <tr>
        <td style='vertical-align: middle; height: 60px;'></td>
        <td style='vertical-align: middle;'></td>
    </tr>";
            }

            html += @"
</table>";

            return html;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateIntegratedFundsTableHtml Error: {ex.Message}");
            return "";
        }
    }

    /// <summary>
    /// 生成社團最近兩年曾獲本會補助計畫及經費表格的 HTML
    /// </summary>
    private string GenerateReceivedSubsidyTableHtml(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
        {
            return "";
        }

        try
        {
            // 從資料庫取得曾獲補助資料
            var receivedSubsidies = OFS_ClbApplicationHelper.GetReceivedSubsidyData(projectID);

            // 計算 rowspan（資料筆數 + header 列）
            int dataCount = (receivedSubsidies != null && receivedSubsidies.Count > 0) ? receivedSubsidies.Count : 1;
            int rowspan = dataCount + 1;

            // 建立 HTML 表格
            var html = $@"
<table>
    <tr>
        <td rowspan='{rowspan}' style='vertical-align: middle; width: 40%;'>社團最近兩年曾獲本會補助計畫及經費</td>
        <td style='vertical-align: middle; width: 40%;'>計畫名稱</td>
        <td style='vertical-align: middle; width: 20%;'>補助金額</td>
    </tr>";

            // 加入資料列
            if (receivedSubsidies != null && receivedSubsidies.Count > 0)
            {
                foreach (var item in receivedSubsidies)
                {
                    html += $@"
    <tr>
        <td style='vertical-align: middle;'>{item.Name}</td>
        <td style='vertical-align: middle;'>{item.Amount:N0}</td>
    </tr>";
                }
            }
            else
            {
                // 無資料時顯示空白列
                html += @"
    <tr>
        <td style='vertical-align: middle; height: 60px;'></td>
        <td style='vertical-align: middle;'></td>
    </tr>";
            }

            html += @"
</table>";

            return html;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateReceivedSubsidyTableHtml Error: {ex.Message}");
            return "";
        }
    }

    #endregion

    public bool IsReusable => false;
}
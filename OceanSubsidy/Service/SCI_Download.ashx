<%@ WebHandler Language="C#" Class="SCI_Download" %>
<%@ Assembly Name="System.IO.Compression.FileSystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" %>

using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.IO.Compression;
using GS.App;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using GS.OCA_OceanSubsidy.Operation.OFS;

public class SCI_Download : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var action = context.Request.QueryString["action"];

        if (string.IsNullOrEmpty(action))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing action parameter. Use 'downloadPlan', 'downloadApprovedPlan', 'downloadTemplate', or 'downloadFile'.");
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
                case "downloadtemplate":
                    DownloadTemplate(context);
                    break;
                case "downloadfile":
                    DownloadUploadedFile(context);
                    break;
                case "downloadreviewfile":
                    DownloadReviewFile(context);
                    break;
                case "getcontractfiles":
                    GetContractFiles(context);
                    break;
                default:
                    context.Response.StatusCode = 400;
                    context.Response.Write("Invalid action. Use 'downloadPlan', 'downloadApprovedPlan', 'downloadTemplate', 'downloadFile', 'downloadReviewFile', or 'getContractFiles'.");
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
    /// 下載範本檔案
    /// </summary>
    private void DownloadTemplate(HttpContext context)
    {
        var fileCode = context.Request.QueryString["fileCode"];
        var projectID = context.Request.QueryString["ProjectID"];

        if (string.IsNullOrEmpty(fileCode))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing fileCode parameter.");
            return;
        }

        // 根據 fileCode 決定範本檔案路徑
        string templatePath = GetTemplateFilePath(fileCode);
        if (string.IsNullOrEmpty(templatePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Template not found for fileCode: {fileCode}");
            return;
        }

        string filePath = context.Server.MapPath(templatePath);

        // 檢查是資料夾還是檔案
        if (Directory.Exists(filePath))
        {
            // 如果是資料夾，動態壓縮並下載
            string zipFilePath = ProcessFolderToZip(fileCode, filePath, projectID);
            if (string.IsNullOrEmpty(zipFilePath))
            {
                context.Response.StatusCode = 500;
                context.Response.Write("Failed to create zip file from folder.");
                return;
            }

            string fileName = Path.GetFileName(zipFilePath);
            DownloadFile(context, zipFilePath, fileName, "application/zip");
            CleanupTempFile(zipFilePath);
        }
        else if (File.Exists(filePath))
        {
            // 如果是檔案，按原邏輯處理
            // 對於需要動態處理內容的範本檔案進行處理
            filePath = ProcessDynamicTemplate(fileCode, filePath, projectID);

            string fileName = Path.GetFileName(filePath);
            string fileExt = Path.GetExtension(filePath).ToLower();

            // 根據檔案類型設定 Content-Type
            string contentType = "application/octet-stream";
            switch (fileExt)
            {
                case ".docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".zip":
                    contentType = "application/zip";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
            }

            DownloadFile(context, filePath, fileName, contentType);

            // 如果是暫存檔案，下載完成後清理
            CleanupTempFile(filePath);
        }
        else
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Template file or folder not found: {templatePath}");
            return;
        }
    }

    /// <summary>
    /// 下載已上傳的檔案
    /// </summary>
    private void DownloadUploadedFile(HttpContext context)
    {
        var projectId = context.Request.QueryString["projectId"];
        var fileCode = context.Request.QueryString["fileCode"];
        var fileName = context.Request.QueryString["fileName"];

        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(fileCode))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing projectId or fileCode parameter.");
            return;
        }

        // 從資料庫取得檔案資訊
        var attachments = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectId, fileCode);
        if (attachments == null || attachments.Count == 0)
        {
            context.Response.StatusCode = 404;
            context.Response.Write("File not found in database.");
            return;
        }

        var attachment = attachments[0]; // 取第一個（最新的）
        string templatePath = attachment.TemplatePath;

        if (!templatePath.StartsWith("~/"))
        {
            templatePath = "~/" + templatePath;
        }

        string filePath = context.Server.MapPath(templatePath);
        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Physical file not found: {templatePath}");
            return;
        }

        string downloadFileName = !string.IsNullOrEmpty(fileName) ? fileName : attachment.FileName;
        DownloadFile(context, filePath, downloadFileName, "application/pdf");
    }

    /// <summary>
    /// 下載審查意見檔案
    /// </summary>
    private void DownloadReviewFile(HttpContext context)
    {
        var filePath = context.Request.QueryString["filePath"];

        if (string.IsNullOrEmpty(filePath))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing filePath parameter.");
            return;
        }

        // 確保路徑以 ~/ 開頭
        if (!filePath.StartsWith("~/"))
        {
            filePath = "~/" + filePath;
        }

        string physicalPath = context.Server.MapPath(filePath);

        if (!File.Exists(physicalPath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write("Review file not found.");
            return;
        }

        string fileName = Path.GetFileName(physicalPath);
        DownloadFile(context, physicalPath, fileName, "application/pdf");
    }

    /// <summary>
    /// 根據檔案代碼取得範本檔案路徑
    /// </summary>
    private string GetTemplateFilePath(string fileCode)
    {
        switch (fileCode)
        {
            // OceanTech 業者範本檔案對應
            case "FILE_OTech1":
                return "~/Template/SCI/OTech/附件-01海洋委員會海洋科技專案補助作業要點.pdf";
            case "FILE_OTech2":
                return "~/Template/SCI/OTech/附件-02海洋科技科專案計畫書";
            case "FILE_OTech3":
                return "~/Template/SCI/OTech/附件-03建議迴避之審查委員清單.docx";
            case "FILE_OTech4":
                return "~/Template/SCI/OTech/附件-04未違反公職人員利益衝突迴避法切結書.docx";
            case "FILE_OTech5":
                return "~/Template/SCI/OTech/附件-05蒐集個人資料告知事項暨個人資料提供同意書.docx";
            case "FILE_OTech6":
                return "~/Template/SCI/OTech/附件-06申請人自我檢查表.docx";
            case "FILE_OTech7":
                return "~/Template/SCI/OTech/附件-07簽約注意事項.pdf";
            case "FILE_OTech8":
                return "~/Template/SCI/OTech/附件-08海洋科技業界科專計畫補助契約書.docx";
            case "FILE_OTech9":
                return "~/Template/SCI/OTech/附件-09研究紀錄簿使用原則.pdf";
            case "FILE_OTech10":
                return "~/Template/SCI/OTech/附件-10海洋科技專案計畫會計科目編列與執行原則.pdf";
            case "FILE_OTech11":
                return "~/Template/SCI/OTech/附件-11計畫書書脊（側邊）格式.docx";

            // Academic 學研範本檔案對應
            case "FILE_AC1":
                return "~/Template/SCI/Academic/附件-01海洋委員會海洋科技專案補助作業要點.pdf";
            case "FILE_AC2":
                return "~/Template/SCI/Academic/附件-02海洋科技科專案計畫書";
            case "FILE_AC3":
                return "~/Template/SCI/Academic/附件-03建議迴避之審查委員清單.docx";
            case "FILE_AC4":
                return "~/Template/SCI/Academic/附件-04未違反公職人員利益衝突迴避法切結書.docx";
            case "FILE_AC5":
                return "~/Template/SCI/Academic/附件-05蒐集個人資料告知事項暨個人資料提供同意書.docx";
            case "FILE_AC6":
                return "~/Template/SCI/Academic/附件-06共同執行單位基本資料表.docx";
            case "FILE_AC7":
                return "~/Template/SCI/Academic/附件-07申請人自我檢查表.docx";
            case "FILE_AC8":
                return "~/Template/SCI/Academic/附件-08簽約注意事項.pdf";
            case "FILE_AC9":
                return "~/Template/SCI/Academic/附件-09海洋委員會補助科技專案計畫契約書.docx";
            case "FILE_AC10":
                return "~/Template/SCI/Academic/附件-10海洋科技專案計畫會計科目編列與執行原則.pdf";
            case "FILE_AC11":
                return "~/Template/SCI/Academic/附件-11海洋科技專案成效追蹤自評表.docx";
            case "FILE_AC12":
                return "~/Template/SCI/Academic/附件-12研究紀錄簿使用原則.pdf";
            case "FILE_AC13":
                return "~/Template/SCI/Academic/附件-13計畫書書脊（側邊）格式.docx";

            // 契約書範本 - 學研單位
            case "CONTRACT_AC_CONFIDENTIALITY":
                return "~/Template/SCI/Academic/科專-學研-保密切結書.docx";
            case "CONTRACT_AC_PRIVACY":
                return "~/Template/SCI/Academic/科專-學研-個資同意書.docx";

            // 契約書範本 - 業者
            case "CONTRACT_OTECH_PRIVACY":
                return "~/Template/SCI/OTech/科專-業者-個資同意書.docx";

            default:
                return "";
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

    /// <summary>
    /// 處理資料夾壓縮成 ZIP（支援動態加工檔案）
    /// </summary>
    private string ProcessFolderToZip(string fileCode, string folderPath, string projectID)
    {
        try
        {
            if (!Directory.Exists(folderPath))
            {
                return null;
            }

            // 建立暫存 ZIP 檔案路徑
            string folderName = Path.GetFileName(folderPath);
            string tempZipPath = Path.Combine(Path.GetTempPath(), $"{folderName}.zip");

            // 如果暫存檔案已存在，先刪除
            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }

            // 建立暫存資料夾用於處理檔案
            string tempProcessFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempProcessFolder);

            try
            {
                // 取得資料夾內所有檔案
                var files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

                // 定義需要進行套表處理的檔案
                var templateFiles = new HashSet<string>
                {
                    "附件-02-1海洋科技科專案計畫書.docx",
                    "附件-02-2預定進度及查核標準-查核標準.xlsx",
                    "附件-02-2預定進度及查核標準-預定進度.xlsx",
                    "附件-02-4-0經費概算彙總表.xlsx"
                };

                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string tempFilePath = Path.Combine(tempProcessFolder, fileName);

                    // 複製檔案到暫存資料夾
                    File.Copy(file, tempFilePath, true);

                    // 檢查是否為需要套表的檔案
                    if (templateFiles.Contains(fileName))
                    {
                        ProcessTemplateFile_AC2(tempFilePath, fileName, projectID);
                    }
                }

                // 將處理後的檔案壓縮成 ZIP
                ZipFile.CreateFromDirectory(tempProcessFolder, tempZipPath);

                return tempZipPath;
            }
            finally
            {
                // 清理暫存處理資料夾
                if (Directory.Exists(tempProcessFolder))
                {
                    Directory.Delete(tempProcessFolder, true);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessFolderToZip Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 處理 FILE_AC2 和 FILE_OTech2 資料夾內的特定範本檔案
    /// </summary>
    private void ProcessTemplateFile_AC2(string filePath, string fileName, string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID) || !File.Exists(filePath))
            {
                return;
            }

            // 根據不同的檔案名稱進行不同的處理
            switch (fileName)
            {
                case "附件-02-1海洋科技科專案計畫書.docx":
                    ProcessTemplate_AC2_File1(filePath, projectID);
                    break;
                case "附件-02-2預定進度及查核標準-查核標準.xlsx":
                    ProcessTemplate_AC2_File2_CheckStandard(filePath, projectID);
                    break;
                case "附件-02-2預定進度及查核標準-預定進度.xlsx":
                    ProcessTemplate_AC2_File2_Schedule(filePath, projectID);
                    break;
                case "附件-02-4-0經費概算彙總表.xlsx":
                    ProcessTemplate_AC2_File4_Budget(filePath, projectID);
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessTemplateFile_AC2 Error ({fileName}): {ex.Message}");
        }
    }

    /// <summary>
    /// 處理附件-02-1海洋科技科專案計畫書.docx
    /// </summary>
    private void ProcessTemplate_AC2_File1(string filePath, string projectID)
    {
        try
        {
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            if (applicationMain == null) return;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);
                var placeholder = new Dictionary<string, string>();

                // 加入基本資料
                placeholder.Add("ProjectID", projectID ?? "");
                placeholder.Add("ProjectNameTw", applicationMain.ProjectNameTw ?? "");
                placeholder.Add("ProjectNameEn", applicationMain.ProjectNameEn ?? "");
                placeholder.Add("OrgName", applicationMain.OrgName ?? "");

                // 取得當前日期（民國年月日）
                DateTime currentDate = DateTime.Now;
                int year = DateTimeHelper.GregorianYearToMinguo(currentDate.Year);
                int month = currentDate.Month;
                int day = currentDate.Day;
                placeholder.Add("TYear", year.ToString());
                placeholder.Add("TMonth", month.ToString());
                placeholder.Add("TDay", day.ToString());
                //加入地址
                placeholder.Add("RegisteredAddress", applicationMain.RegisteredAddress ?? "");
                placeholder.Add("CorrespondenceAddress", applicationMain.CorrespondenceAddress ?? "");

                // 處理 OrgCategory 欄位勾選（機構類別）
                string orgCategory = applicationMain.OrgCategory ?? "";
                placeholder.Add("Academic", orgCategory == "Academic" ? "☒" : "☐");
                placeholder.Add("Legal", orgCategory == "Legal" ? "☒" : "☐");

                // 處理 Field 欄位勾選（領域）
                string field = applicationMain.Field ?? "";
                placeholder.Add("Field1", field == "Information" ? "☒" : "☐");
                placeholder.Add("Field2", field == "Environment" ? "☒" : "☐");
                placeholder.Add("Field3", field == "Material" ? "☒" : "☐");
                placeholder.Add("Field4", field == "Mechanical" ? "☒" : "☐");

                // 處理 Topic 欄位勾選（主題）
                string topic = applicationMain.Topic ?? "";
                placeholder.Add("Topic1", topic == "Protection" ? "☒" : "☐");
                placeholder.Add("Topic2", topic == "Security" ? "☒" : "☐");
                placeholder.Add("Topic3", topic == "Sustainability" ? "☒" : "☐");
                placeholder.Add("Topic4", topic == "Other" ? "☒" : "☐");

                // 處理開始日期（民國年月日）
                int sYear = 0, sMonth = 0, sDay = 0;
                if (applicationMain.StartTime != null)
                {
                    DateTime startTime = applicationMain.StartTime.Value;
                    sYear = DateTimeHelper.GregorianYearToMinguo(startTime.Year);
                    sMonth = startTime.Month;
                    sDay = startTime.Day;
                }

                placeholder.Add("SYear", sYear.ToString());
                placeholder.Add("SMonth", sMonth.ToString());
                placeholder.Add("SDay", sDay.ToString());

                // 處理結束日期（民國年月日）
                int eYear = 0, eMonth = 0, eDay = 0;
                if (applicationMain.EndTime != null)
                {
                    DateTime endTime = applicationMain.EndTime.Value;
                    eYear = DateTimeHelper.GregorianYearToMinguo(endTime.Year);
                    eMonth = endTime.Month;
                    eDay = endTime.Day;
                }

                placeholder.Add("EYear", eYear.ToString());
                placeholder.Add("EMonth", eMonth.ToString());
                placeholder.Add("EDay", eDay.ToString());

                // 先算總月數差
                int totalMonths = (eYear - sYear) * 12 + (eMonth - sMonth);

                // 如果結束日的日數比開始日小，代表還沒滿一個月，要減 1
                if (eDay < sDay)
                {
                    totalMonths--;
                }

                placeholder.Add("TotalMonths", totalMonths.ToString());


                // 取得經費總計資料
                var totalFeeSum = OFS_SciFundingHelper.GetTotalFeeSum(projectID);

                // 建立使用點號作為千分位分隔符的格式化設定
                var numberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                numberFormat.NumberGroupSeparator = ".";

                // 加入經費資料（千分位格式：1.100）
                placeholder.Add("SubsidyAmount", totalFeeSum.SubsidyAmount.ToString("N0", numberFormat));
                placeholder.Add("CoopAmount", totalFeeSum.CoopAmount.ToString("N0", numberFormat));
                placeholder.Add("TotalAmount", totalFeeSum.TotalAmount.ToString("N0", numberFormat));

                // 取得人員資料（主持人和聯絡人）
                string personID = "P" + projectID;
                var personnelList = OFS_SciApplicationHelper.GetPersonnelByPersonID(personID);
                var host = personnelList?.FirstOrDefault(p => p.Role == "計畫主持人");
                var contact = personnelList?.FirstOrDefault(p => p.Role == "計畫聯絡人");
                var Accounting = personnelList?.FirstOrDefault(p => p.Role == "會計聯絡人");

                // 加入主持人資料
                placeholder.Add("HostName", host?.Name ?? "");
                placeholder.Add("HostMPhone", host?.MobilePhone ?? "");
                placeholder.Add("HostPhone", host?.Phone ?? "");
                placeholder.Add("HostExt", host?.PhoneExt ?? "");
                placeholder.Add("HostJobTitle", host?.JobTitle ?? "");

                // 加入聯絡人資料
                placeholder.Add("ContactName", contact?.Name ?? "");
                placeholder.Add("ContactMPhone", contact?.MobilePhone ?? "");
                placeholder.Add("ContactPhone", contact?.Phone ?? "");
                placeholder.Add("ContactExt", contact?.PhoneExt ?? "");
                placeholder.Add("ContactJobTitle", contact?.JobTitle ?? "");

                // 加入會計聯絡人資料
                placeholder.Add("AccountingName", Accounting?.Name ?? "");
                placeholder.Add("AccountingMPhone", Accounting?.MobilePhone ?? "");
                placeholder.Add("AccountingPhone", Accounting?.Phone ?? "");
                placeholder.Add("AccountingExt", Accounting?.PhoneExt ?? "");
                placeholder.Add("AccountingJobTitle", Accounting?.JobTitle ?? "");

                //*計畫內容摘要、關鍵字
                placeholder.Add("Summary", applicationMain?.Summary ?? "");

                // 取得關鍵字資料
                var keywords = OFS_SciApplicationHelper.GetKeywordsByID(projectID);
                string keywordsTw = "";
                string keywordsEn = "";

                if (keywords != null && keywords.Count > 0)
                {
                    // 收集所有中文關鍵字
                    var twList = keywords
                        .Where(k => !string.IsNullOrEmpty(k.KeyWordTw))
                        .Select(k => k.KeyWordTw)
                        .ToList();
                    keywordsTw = string.Join(",", twList);

                    // 收集所有英文關鍵字
                    var enList = keywords
                        .Where(k => !string.IsNullOrEmpty(k.KeyWordEn))
                        .Select(k => k.KeyWordEn)
                        .ToList();
                    keywordsEn = string.Join(",", enList);
                }

                placeholder.Add("KeywordsTw", keywordsTw);
                placeholder.Add("KeywordsEn", keywordsEn);

                var repeatData = new List<Dictionary<string, string>>();
                helper.GenerateWord(placeholder, repeatData);

                // 取得技術成熟度資料並處理表格
                var techReadinessList = OFS_SciRecusedList.GetTechReadinessListByProjectID(projectID);

                if (techReadinessList != null && techReadinessList.Count > 0)
                {
                    // 新增表格行數（扣除標題行，所以要新增 techReadinessList.Count 行）
                    helper.SetTableRowCount("TechReadiness", techReadinessList.Count);

                    // 填入資料到表格（從第 1 行開始，第 0 行是標題）
                    for (int i = 0; i < techReadinessList.Count; i++)
                    {
                        var tech = techReadinessList[i];
                        int rowIndex = i + 1; // 第 0 行是標題，資料從第 1 行開始

                        // 轉換 TRL 層級代碼為描述
                        string befTRLDesc = SysZgsCodeHelper.GetDescnameByCode("SCI_TRL", tech.Bef_TRLevel ?? "");
                        string aftTRLDesc = SysZgsCodeHelper.GetDescnameByCode("SCI_TRL", tech.Aft_TRLevel ?? "");

                        // 欄位順序：研發技術項目、執行前TRL層級、執行後TRL層級、技術進程概述
                        helper.SetTableCellValue("TechReadiness", rowIndex, 0, tech.Name ?? "");
                        helper.SetTableCellValue("TechReadiness", rowIndex, 1, befTRLDesc);
                        helper.SetTableCellValue("TechReadiness", rowIndex, 2, aftTRLDesc);
                        helper.SetTableCellValue("TechReadiness", rowIndex, 3, tech.Description ?? "");
                    }
                }
                else
                {
                    // 如果沒有資料，新增一個空白行
                    helper.SetTableRowCount("TechReadiness", 1);
                    helper.SetTableCellValue("TechReadiness", 1, 0, "");
                    helper.SetTableCellValue("TechReadiness", 1, 1, "");
                    helper.SetTableCellValue("TechReadiness", 1, 2, "");
                    helper.SetTableCellValue("TechReadiness", 1, 3, "");
                }

                List<OFS_SCI_UploadFile> L_TechnologyDiagram = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectID, "TechnologyDiagram");
                string relativePath = L_TechnologyDiagram[0].TemplatePath; // 或 FilePath，看你模型欄位名稱
                string absolutePath = HttpContext.Current.Server.MapPath("~/" + relativePath);
                helper.InsertImageAtBookmark("TechnologyDiagram", absolutePath, 6400800, 4257567);

                List<OFS_SCI_UploadFile> L_WorkSchStructure = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectID, "WorkSchStructure");
                relativePath = L_WorkSchStructure[0].TemplatePath;
                absolutePath = HttpContext.Current.Server.MapPath("~/" + relativePath);
                // 插入圖片：TechnologyDiagram 和 WorkSchStructure
                helper.InsertImageAtBookmark("WorkSchStructure", absolutePath, 6400800, 4257567);

                helper.CloseAsSave();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessTemplate_AC2_File1 Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 處理附件-02-2預定進度及查核標準-查核標準.xlsx
    /// </summary>
    private void ProcessTemplate_AC2_File2_CheckStandard(string filePath, string projectID)
    {
        try
        {
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            if (applicationMain == null) return;

            using (var excel = new ExcelHelper(filePath, true))
            {
                // 取得第一個工作表名稱
                var sheetNames = excel.GetWorksheetNames();
                if (!sheetNames.Any()) return;

                string sheetName = sheetNames[0];

                // 從資料庫讀取查核標準資料
                var StandardList = OFS_SciWorkSchHelper.GetWorkScheduleWithCheckStandard(projectID);

                if (StandardList != null && StandardList.Rows.Count > 0)
                {
                    // 資料插入在第2行和第3行之間，所以從第3行開始插入
                    int insertPosition = 3;

                    // 先插入需要的行數 (資料筆數)
                    if (StandardList.Rows.Count > 0)
                    {
                        excel.InsertRows(sheetName, insertPosition, StandardList.Rows.Count);
                    }

                    // 填入資料 (從第3行開始)
                    for (int i = 0; i < StandardList.Rows.Count; i++)
                    {
                        DataRow row = StandardList.Rows[i];

                        // 取欄位值
                        string serialNumber = row["SerialNumber"]?.ToString();
                        DateTime? PlannedFinishDate = row["PlannedFinishDate"] != DBNull.Value 
                        ? Convert.ToDateTime(row["PlannedFinishDate"])
                        : (DateTime?)null;
                        String MinguoFinishDate = DateTimeHelper.ToMinguoDate(PlannedFinishDate);
                        string checkDescription = row["CheckDescription"]?.ToString();
                        bool isOutsourced = row["IsOutsourced"] != DBNull.Value 
                                            && Convert.ToBoolean(row["IsOutsourced"]);
                        int currentRow = insertPosition + i;

                        // A欄: 序號
                        var dataA = new List<List<object>>
                        {
                            new List<object> { serialNumber ?? "" }
                        };
                        excel.WriteRange(sheetName, dataA, currentRow, 1);

                        // B欄: 預定完成時間
                        var dataB = new List<List<object>>
                        {
                            new List<object> { MinguoFinishDate ?? "" }
                        };
                        excel.WriteRange(sheetName, dataB, currentRow, 2);

                        // C欄: 查核標準
                        var dataC = new List<List<object>>
                        {
                            new List<object> { checkDescription }
                        };
                        excel.WriteRange(sheetName, dataC, currentRow, 3);

                        // D欄: 是否委外
                      string OrgName = (!isOutsourced) ? applicationMain.OrgName : "";

                        var dataD = new List<List<object>>
                        {
                            new List<object> { OrgName }
                        };
                        excel.WriteRange(sheetName, dataD, currentRow, 4);

                    }
                }

                excel.Save();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessTemplate_AC2_File2_CheckStandard Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 處理附件-02-2預定進度及查核標準-預定進度.xlsx
    /// </summary>
    private void ProcessTemplate_AC2_File2_Schedule(string filePath, string projectID)
    {
        try
        {
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            if (applicationMain == null) return;

            using (var excel = new ExcelHelper(filePath, true))
            {
                var sheetNames = excel.GetWorksheetNames();
                if (!sheetNames.Any()) return;

                string sheetName = sheetNames[0];

                // 取得開始和結束日期
                DateTime? startTime = applicationMain.StartTime;
                DateTime? endTime = applicationMain.EndTime;

                if (startTime.HasValue && endTime.HasValue)
                {
                    // 渲染年度到 Excel
                    RenderYearHeaders(excel, sheetName, startTime.Value, endTime.Value);

                    // 渲染月份到 Excel，並取得月份與欄位的對應關係
                    var monthColumnMap = RenderMonthHeaders(excel, sheetName, startTime.Value, endTime.Value);

                    // 取得工作項目資料
                    var workItems = OFS_SciWorkSchHelper.GetWorkItemsByProjectID(projectID);

                    // 渲染工作項目與進度
                    RenderWorkItemsSchedule(excel, sheetName, workItems, monthColumnMap, applicationMain);
                }

                excel.Save();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessTemplate_AC2_File2_Schedule Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 根據 StartTime 和 EndTime 渲染年度到 Excel
    /// 從 D3 開始,根據實際月份數動態計算每個年度標題的位置
    /// </summary>
    private void RenderYearHeaders(ExcelHelper excel, string sheetName, DateTime startTime, DateTime endTime)
    {
        // 起始欄位是 D 欄 (column 4)
        int currentColumn = 4;
        DateTime currentYearStart = startTime;

        // 逐年渲染
        while (currentYearStart <= endTime)
        {
            int minguoYear = DateTimeHelper.GregorianYearToMinguo(currentYearStart.Year);
            string yearText = $"{minguoYear}年度";

            // 寫入到第3行 (row 3)
            var data = new List<List<object>>
            {
                new List<object> { yearText }
            };
            excel.WriteRange(sheetName, data, 3, currentColumn);

            // 計算該年還有多少個月
            int monthsInCurrentYear;
            if (currentYearStart.Year == endTime.Year)
            {
                // 如果是最後一年,只計算到 endTime.Month
                monthsInCurrentYear = endTime.Month - currentYearStart.Month + 1;
            }
            else
            {
                // 否則計算到該年12月
                monthsInCurrentYear = 12 - currentYearStart.Month + 1;
            }

            // 移動到下一年度標題的欄位位置
            currentColumn += monthsInCurrentYear;

            // 移動到下一年的1月
            currentYearStart = new DateTime(currentYearStart.Year + 1, 1, 1);
        }
    }

    /// <summary>
    /// 根據 StartTime 和 EndTime 渲染月份到 Excel
    /// 從 D4 開始,顯示 1~12 的數字
    /// </summary>
    /// <returns>月份與欄位的對應字典，key 為 "年_月" 格式（民國年），value 為欄位編號</returns>
    private Dictionary<string, int> RenderMonthHeaders(ExcelHelper excel, string sheetName, DateTime startTime, DateTime endTime)
    {
        // 建立月份與欄位的對應字典
        var monthColumnMap = new Dictionary<string, int>();

        // 起始欄位是 D 欄 (column 4)
        int currentColumn = 4;

        // 從 startTime 開始,逐月渲染到 endTime
        DateTime currentDate = new DateTime(startTime.Year, startTime.Month, 1);
        DateTime endDate = new DateTime(endTime.Year, endTime.Month, 1);

        while (currentDate <= endDate)
        {
            // 寫入月份數字 (1~12)
            var data = new List<List<object>>
            {
                new List<object> { currentDate.Month }
            };
            excel.WriteRange(sheetName, data, 4, currentColumn);

            // 紀錄月份與欄位的對應關係（使用民國年）
            int minguoYear = DateTimeHelper.GregorianYearToMinguo(currentDate.Year);
            string key = $"{minguoYear}_{currentDate.Month}";
            monthColumnMap[key] = currentColumn;

            // 移動到下一個月
            currentDate = currentDate.AddMonths(1);
            currentColumn++;
        }

        return monthColumnMap;
    }

    /// <summary>
    /// 渲染工作項目與進度到 Excel
    /// </summary>
    /// <param name="excel">Excel Helper</param>
    /// <param name="sheetName">工作表名稱</param>
    /// <param name="workItems">工作項目列表</param>
    /// <param name="monthColumnMap">月份與欄位的對應字典</param>
    /// <param name="applicationMain">申請主檔資料</param>
    private void RenderWorkItemsSchedule(ExcelHelper excel, string sheetName, List<OFS_SCI_WorkSch_Main> workItems, Dictionary<string, int> monthColumnMap, OFS_SCI_Application_Main applicationMain)
    {
        if (workItems == null || workItems.Count == 0)
            return;

        try
        {
            // 1. 根據資料筆數插入行數（插入在第4~5行之間）
            excel.InsertRows(sheetName, 5, workItems.Count);

            // 統計變數（只統計子項）
            decimal totalWeighting = 0;
            decimal totalInvestMonth = 0;

            // 建立 WorkItem_id 與行號的對應字典（用於後續檢查點渲染）
            var workItemRowMap = new Dictionary<string, int>();

            // 2. 填入 WorkDisplayName 和渲染灰色格子
            for (int i = 0; i < workItems.Count; i++)
            {
                var item = workItems[i];
                int currentRow = 5 + i;

                // 記錄 WorkItem_id 與行號的對應關係
                workItemRowMap[item.WorkItem_id] = currentRow;

                // 取得 WorkCode (底線後的部分)
                string workCode = OFS_SciWorkSchHelper.ExtractItemCodeFromWorkItemId(item.WorkItem_id);

                // 註記：StartYear 或 StartMonth 為 NULL 者，代表工作項目母項，由子項組成
                bool isParentItem = !item.StartYear.HasValue || !item.StartMonth.HasValue;

                // 組合顯示文字
                string workDisplayName;
                if (isParentItem)
                {
                    // 母項：只顯示工作代碼和名稱，不顯示執行單位
                    workDisplayName = $"{workCode}.{item.WorkName}";
                }
                else
                {
                    // 子項：判斷執行單位
                    string orgName = "";
                    if (item.IsOutsourced.HasValue && !item.IsOutsourced.Value)
                    {
                        // 非委外，使用 applicationMain 的 OrgName
                        orgName = applicationMain?.OrgName ?? "";
                    }
                    // 如果是委外（IsOutsourced = true），orgName 保持為空字串

                    // 組合顯示文字（同一單元格內換行）
                    workDisplayName = $"{workCode}.{item.WorkName}\n(執行單位：{orgName})";
                }

                // 寫入 A 欄（工作項目名稱與執行單位）
                var dataA = new List<List<object>>
                {
                    new List<object> { workDisplayName }
                };
                excel.WriteRange(sheetName, dataA, currentRow, 1);

                // 寫入 B 欄（計畫權重）
                var dataB = new List<List<object>>
                {
                    new List<object> { item.Weighting.HasValue ? (object)item.Weighting.Value : "" }
                };
                excel.WriteRange(sheetName, dataB, currentRow, 2);

                // 寫入 C 欄（投入人月數）
                var dataC = new List<List<object>>
                {
                    new List<object> { item.InvestMonth.HasValue ? (object)item.InvestMonth.Value : "" }
                };
                excel.WriteRange(sheetName, dataC, currentRow, 3);

                // 累加統計（只統計子項）
                if (!isParentItem)
                {
                    if (item.Weighting.HasValue)
                        totalWeighting += item.Weighting.Value;

                    if (item.InvestMonth.HasValue)
                        totalInvestMonth += item.InvestMonth.Value;
                }

                // 3. 渲染灰色格子
                if (item.StartYear.HasValue && item.StartMonth.HasValue && item.InvestMonth.HasValue)
                {
                    // 注意：資料庫中的 StartYear 是西元年，需要轉換為民國年來查找欄位對應
                    int minguoYear = DateTimeHelper.GregorianYearToMinguo(item.StartYear.Value);
                    string key = $"{minguoYear}_{item.StartMonth.Value}";

                    if (monthColumnMap.ContainsKey(key))
                    {
                        int startColumn = monthColumnMap[key];
                        int investMonths = (int)item.InvestMonth.Value;

                        // 從該欄位開始向右渲染灰色格子 InvestMonth 格
                        for (int j = 0; j < investMonths; j++)
                        {
                            excel.SetCellBackgroundColor(sheetName, currentRow, startColumn + j, "FFD3D3D3");
                        }
                    }
                }
            }

            // 4. 渲染檢查點（SerialNumber）到對應格子
            RenderCheckPoints(excel, sheetName, applicationMain.ProjectID, workItemRowMap, monthColumnMap);

            // 5. 寫入統計行（X = 5 + N，N 為工作項目數）
            int summaryRow = 5 + workItems.Count;

            // 寫入 B 欄：計畫權重總計
            var summaryB = new List<List<object>>
            {
                new List<object> { totalWeighting }
            };
            excel.WriteRange(sheetName, summaryB, summaryRow, 2);

            // 寫入 C 欄：投入人月數總計
            var summaryC = new List<List<object>>
            {
                new List<object> { totalInvestMonth }
            };
            excel.WriteRange(sheetName, summaryC, summaryRow +1 , 3);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RenderWorkItemsSchedule Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 渲染檢查點（SerialNumber）到對應格子
    /// </summary>
    /// <param name="excel">Excel Helper</param>
    /// <param name="sheetName">工作表名稱</param>
    /// <param name="projectID">專案ID</param>
    /// <param name="workItemRowMap">WorkItem_id 與行號的對應字典</param>
    /// <param name="monthColumnMap">月份與欄位的對應字典</param>
    private void RenderCheckPoints(ExcelHelper excel, string sheetName, string projectID,
        Dictionary<string, int> workItemRowMap, Dictionary<string, int> monthColumnMap)
    {
        try
        {
            // 取得檢查點資料
            var checkStandards = OFS_SciWorkSchHelper.GetCheckStandardsByProjectID(projectID);

            if (checkStandards == null || checkStandards.Count == 0)
                return;

            // 建立格子與 SerialNumber 的對應字典（用於合併同一格子的多個檢查點）
            // Key: "{row}_{column}", Value: List of SerialNumbers
            var cellCheckPointsMap = new Dictionary<string, List<string>>();

            foreach (var checkStandard in checkStandards)
            {
                // 檢查必要欄位
                if (string.IsNullOrEmpty(checkStandard.WorkItem) ||
                    !checkStandard.PlannedFinishDate.HasValue)
                    continue;

                // 根據 WorkItem 找到對應的 WorkItem_id
                // WorkItem 可能是 "A" 或 "A1"，需要找到對應的完整 WorkItem_id（例如：114SCI0036_A）
                string fullWorkItemId = FindFullWorkItemId(workItemRowMap, projectID, checkStandard.WorkItem);

                if (string.IsNullOrEmpty(fullWorkItemId) || !workItemRowMap.ContainsKey(fullWorkItemId))
                    continue;

                int row = workItemRowMap[fullWorkItemId];

                // 根據 PlannedFinishDate 找到對應的欄位
                DateTime plannedDate = checkStandard.PlannedFinishDate.Value;
                int minguoYear = DateTimeHelper.GregorianYearToMinguo(plannedDate.Year);
                string key = $"{minguoYear}_{plannedDate.Month}";

                if (!monthColumnMap.ContainsKey(key))
                    continue;

                int column = monthColumnMap[key];

                // 建立格子的 key
                string cellKey = $"{row}_{column}";

                // 將 SerialNumber 加入到對應格子的列表中
                if (!cellCheckPointsMap.ContainsKey(cellKey))
                {
                    cellCheckPointsMap[cellKey] = new List<string>();
                }
                cellCheckPointsMap[cellKey].Add(checkStandard.SerialNumber ?? "");
            }

            // 將所有檢查點寫入對應格子（用逗號分隔）
            foreach (var kvp in cellCheckPointsMap)
            {
                var parts = kvp.Key.Split('_');
                int row = int.Parse(parts[0]);
                int column = int.Parse(parts[1]);

                // 將 SerialNumbers 用逗號分隔
                string serialNumbers = string.Join(",", kvp.Value);

                // 寫入格子
                var data = new List<List<object>>
                {
                    new List<object> { serialNumbers }
                };
                excel.WriteRange(sheetName, data, row, column);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RenderCheckPoints Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 根據 WorkItem 片段找到完整的 WorkItem_id
    /// </summary>
    /// <param name="workItemRowMap">WorkItem_id 與行號的對應字典</param>
    /// <param name="projectID">專案ID</param>
    /// <param name="workItem">WorkItem 片段（例如："A" 或 "A1"）</param>
    /// <returns>完整的 WorkItem_id（例如："114SCI0036_A"）</returns>
    private string FindFullWorkItemId(Dictionary<string, int> workItemRowMap, string projectID, string workItem)
    {
        // 構建完整的 WorkItem_id
        string fullWorkItemId = $"{projectID}_{workItem}";

        // 檢查是否存在
        if (workItemRowMap.ContainsKey(fullWorkItemId))
        {
            return fullWorkItemId;
        }

        // 如果找不到，嘗試在所有 key 中尋找包含該 workItem 的
        foreach (var key in workItemRowMap.Keys)
        {
            if (key.EndsWith($"_{workItem}"))
            {
                return key;
            }
        }

        return null;
    }

    /// <summary>
    /// 處理附件-02-4-0經費概算彙總表.xlsx
    /// </summary>
    private void ProcessTemplate_AC2_File4_Budget(string filePath, string projectID)
    {
        try
        {
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            if (applicationMain == null) return;

            using (var excel = new ExcelHelper(filePath, true))
            {
                var sheetNames = excel.GetWorksheetNames();
                if (!sheetNames.Any()) return;

                // 經費概算彙總表
                FillBudgetSummarySheet(excel, sheetNames, projectID);
                FillPersonnelCostSheet(excel, sheetNames, projectID); // 人事費明細表
                FillMaterialCostSheet(excel, sheetNames, projectID); // 消耗性器材及原材料費
                FillResearchFeeSheet(excel, sheetNames, projectID); // 技術移轉及委託研究費用
                FillTravelExpenseSheet(excel, sheetNames, projectID); // 國內差旅費
                FillOtherExpenseSheet(excel, sheetNames, projectID); // 其他業務費

                excel.Save();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessTemplate_AC2_File4_Budget Error: {ex.Message}");
        }
    }

    #region 附件-02-4-0 經費概算彙總表 - Sheet 填寫方法

    /// <summary>
    /// 填寫經費概算彙總表 sheet
    /// </summary>
    private void FillBudgetSummarySheet(ExcelHelper excel, List<string> sheetNames, string projectID)
    {
        try
        {
            // 找到 '經費概算彙總表' sheet
            string targetSheet = "經費概算彙總表";
            if (!sheetNames.Contains(targetSheet))
            {
                // 如果沒有找到目標 sheet，使用第一個 sheet
                targetSheet = sheetNames[0];
            }

            // 取得 applicationMain 來判斷 OrgCategory
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            bool isOceanTech = applicationMain?.OrgCategory == "OceanTech";

            // 從資料庫讀取經費概算彙總表資料 (包含百分比計算)
            var budgetSummaryList = OFS_SciFundingHelper.GetBudgetSummaryList(projectID);

            if (budgetSummaryList != null && budgetSummaryList.Count > 0)
            {
                // 固定有8筆資料（前6筆為費用項目，後2筆為總計）
                // 一般情況：填入 B6~B13, C6~C13, D6~D13
                // OceanTech：跳過第6筆（索引5），填入 B6~B10, B12~B13（第11行空白）
                int startRow = 6; // 從第6行開始
                int rowOffset = 0; // 用於調整行號（當跳過某筆時）

                for (int i = 0; i < budgetSummaryList.Count && i < 8; i++)
                {
                    // 如果是 OceanTech 且為第 6 筆（索引 5），跳過不列印（固定為 0）
                    if (isOceanTech && i == 5)
                    {
                        rowOffset = -1; // 後續行號往上移一行
                        continue;
                    }

                    var item = budgetSummaryList[i];
                    int currentRow = startRow + i + rowOffset;

                    // 判斷是否為最後1筆  因為最後顯示為百分比欄位
                    bool isLastTwoRows = (i >= 7);

                    // B欄: 補助款 (單位:千元)
                    if (isLastTwoRows)
                    {
                        var dataB = new List<List<object>>
                        {
                            new List<object> { item.SubsidyAmount.ToString() + "%" }
                        };
                        excel.WriteRange(targetSheet, dataB, currentRow, 2);
                    }
                    else
                    {
                        var dataB = new List<List<object>>
                        {
                            new List<object> { item.SubsidyAmount }
                        };
                        excel.WriteRange(targetSheet, dataB, currentRow, 2);
                    }

                    // C欄: 配合款 (單位:千元)
                    if (isLastTwoRows)
                    {
                        var dataC = new List<List<object>>
                        {
                            new List<object> { item.CoopAmount.ToString() + "%" }
                        };
                        excel.WriteRange(targetSheet, dataC, currentRow, 3);
                    }
                    else
                    {
                        var dataC = new List<List<object>>
                        {
                            new List<object> { item.CoopAmount }
                        };
                        excel.WriteRange(targetSheet, dataC, currentRow, 3);
                    }

                    // D欄: 總計 (單位:千元)
                    // D12, D13 加上 '%' 符號
                    if (isLastTwoRows)
                    {
                        var dataD = new List<List<object>>
                        {
                            new List<object> { item.TotalAmount.ToString() + "%" }
                        };
                        excel.WriteRange(targetSheet, dataD, currentRow, 4);
                    }
                    else
                    {
                        var dataD = new List<List<object>>
                        {
                            new List<object> { item.TotalAmount }
                        };
                        excel.WriteRange(targetSheet, dataD, currentRow, 4);
                    }

                    // E欄: 占總經費比率(C)/(II) - 只填寫前6筆 (E6~E11)，加上 '%' 符號
                    // E12, E13 固定為NULL，不寫入
                    if (i < 6 && item.RatioOfTotalBudget.HasValue)
                    {
                        var dataE = new List<List<object>>
                        {
                            new List<object> { item.RatioOfTotalBudget.Value.ToString() + "%" }
                        };
                        excel.WriteRange(targetSheet, dataE, currentRow, 5);
                    }

                    // F欄: 各科目補助比率(A)/(I) - 只填寫前6筆 (F6~F11)，加上 '%' 符號
                    // F12, F13 固定為NULL，不寫入
                    if (i < 6 && item.RatioOfSubsidy.HasValue)
                    {
                        var dataF = new List<List<object>>
                        {
                            new List<object> { item.RatioOfSubsidy.Value.ToString() + "%" }
                        };
                        excel.WriteRange(targetSheet, dataF, currentRow, 6);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FillBudgetSummarySheet Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 填寫人事費明細表 sheet
    /// </summary>
    private void FillPersonnelCostSheet(ExcelHelper excel, List<string> sheetNames, string projectID)
    {
        try
        {
            // 找到 '人事費明細表' sheet
            string targetSheet = "人事費明細表";
            if (!sheetNames.Contains(targetSheet))
            {
                // 如果沒有找到目標 sheet，使用第一個 sheet
                targetSheet = sheetNames[1];
            }

            // 從資料庫讀取人事費明細表資料
            var personnelList = OFS_SciFundingHelper.GetPersonnelCostList(projectID);

            if (personnelList != null && personnelList.Count > 0)
            {
                // 資料插入在第4行和第5行之間，所以從第5行開始插入
                int insertPosition = 5;

                // 先插入需要的行數 (資料筆數)
                if (personnelList.Count > 0)
                {
                    excel.InsertRows(targetSheet, insertPosition, personnelList.Count);
                }

                // 填入資料 (從第5行開始)
                for (int i = 0; i < personnelList.Count; i++)
                {
                    var item = personnelList[i];
                    int currentRow = insertPosition + i;

                    // A欄: 姓名
                    var dataA = new List<List<object>>
                    {
                        new List<object> { item.Name ?? "" }
                    };
                    excel.WriteRange(targetSheet, dataA, currentRow, 1);

                    // B欄: 職稱 (顯示中文名稱)
                    var dataB = new List<List<object>>
                    {
                        new List<object> { item.JobTitleDesc ?? "" }
                    };
                    excel.WriteRange(targetSheet, dataB, currentRow, 2);

                    // C欄: 平均月薪 (單位: 千元)
                    var dataC = new List<List<object>>
                    {
                        new List<object> { item.AvgSalary / 1000 }
                    };
                    excel.WriteRange(targetSheet, dataC, currentRow, 3);

                    // D欄: 月份
                    var dataD = new List<List<object>>
                    {
                        new List<object> { item.Month }
                    };
                    excel.WriteRange(targetSheet, dataD, currentRow, 4);

                    // E欄: 小計 (平均月薪 * 月份，單位: 千元)
                    var dataE = new List<List<object>>
                    {
                        new List<object> { item.Subtotal / 1000 }
                    };
                    excel.WriteRange(targetSheet, dataE, currentRow, 5);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FillPersonnelCostSheet Error: {ex.Message}");
        }
    }

    // 例如：
    private void FillMaterialCostSheet(ExcelHelper excel, List<string> sheetNames, string projectID)
    {
        try
        {
            // 找到 '消耗性器材及原材料費' sheet
            string targetSheet = "消耗性器材及原材料費";
            if (!sheetNames.Contains(targetSheet))
            {
                // 如果沒有找到目標 sheet，使用第2個 sheet
                targetSheet = sheetNames[2];
            }

            // 從資料庫讀取材料明細表資料
            var MaterialList = OFS_SciFundingHelper.GetMaterialCostList(projectID);

            if (MaterialList != null && MaterialList.Count > 0)
            {
                // 資料插入在第3行和第4行之間，所以從第4行開始插入
                int insertPosition = 4;

                // 先插入需要的行數 (資料筆數)
                if (MaterialList.Count > 0)
                {
                    excel.InsertRows(targetSheet, insertPosition, MaterialList.Count);
                }

                // 填入資料 (從第4行開始)
                for (int i = 0; i < MaterialList.Count; i++)
                {
                    var item = MaterialList[i];
                    int currentRow = insertPosition + i;

                    // A欄: 品名
                    var dataA = new List<List<object>>
                    {
                        new List<object> { item.ItemName ?? "" }
                    };
                    excel.WriteRange(targetSheet, dataA, currentRow, 1);

                    // B欄: 說明 
                    var dataB = new List<List<object>>
                    {
                        new List<object> { item.Description ?? "" }
                    };
                    excel.WriteRange(targetSheet, dataB, currentRow, 2);

                    // C欄: 單位
                    var dataC = new List<List<object>>
                    {
                        new List<object> { item.UnitDesc }
                    };
                    excel.WriteRange(targetSheet, dataC, currentRow, 3);

                    // D欄: 預估需求數量
                    var dataD = new List<List<object>>
                    {
                        new List<object> { item.PreNum }
                    };
                    excel.WriteRange(targetSheet, dataD, currentRow, 4);

                    // E欄: 單價 (單位: 千元)
                    var dataE = new List<List<object>>
                    {
                        new List<object> { item.UnitPrice / 1000 }
                    };
                    excel.WriteRange(targetSheet, dataE, currentRow, 5);

                    // F欄: 總價 (單位: 千元)
                    var dataF = new List<List<object>>
                    {
                        new List<object> { item.TotalPrice / 1000 }
                    };
                    excel.WriteRange(targetSheet, dataF, currentRow, 6);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FillMaterialCostSheet Error: {ex.Message}");
        }
    }


    private void FillResearchFeeSheet(ExcelHelper excel, List<string> sheetNames, string projectID)
    {
        // 遺珠: 原先規劃之文件 並沒有讓使用者可以新增多筆的需求， 所以 這邊寫入到 EXCEL 屬於 直接塞入 非動態產生
        // 直接先寫死兩筆資料，待之後進行修改，如果經費/人事的 技術移轉、委託研究或驗證費 有修改成動態 ，這邊要一同修改。
        try
        {
            // 找到 '技術移轉及委託研究費用' sheet
            string targetSheet = "技術移轉及委託研究費用";
            if (!sheetNames.Contains(targetSheet))
            {
                // 如果沒有找到目標 sheet，使用第3個 sheet
                targetSheet = sheetNames[3];
            }

            // 從資料庫讀取技術移轉及委託研究費用資料
            var ResearchFeesList = OFS_SciFundingHelper.GetResearchFeesList(projectID);


            if (ResearchFeesList != null && ResearchFeesList.Count > 0)
            {
                // 只取第一筆資料
                var item = ResearchFeesList[0];

                // ==== 第一次寫入：第4行 ====
                int targetRow1 = 4;

                // A欄: 費用類別
                excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item.category ?? "" } }, targetRow1, 1);
                // B欄: 起訖日期 (StartDate ~ EndDate)
                DateTime? startDate = DateTime.TryParse(item.dateStart, out var s) ? s : (DateTime?)null;
                DateTime? endDate = DateTime.TryParse(item.dateEnd, out var e) ? e : (DateTime?)null;
                // 轉成民國日期
                string startMinguo = startDate.HasValue ? DateTimeHelper.ToMinguoDate(startDate.Value) : "";
                string endMinguo = endDate.HasValue ? DateTimeHelper.ToMinguoDate(endDate.Value) : "";

                string dateRange = $"{startMinguo} ~ {endMinguo}";
                excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { dateRange } }, targetRow1, 2);
                // C欄: 名稱
                excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item.projectName ?? "" } }, targetRow1, 3);
                // D欄: 負責人
                excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item.targetPerson ?? "" } }, targetRow1, 4);
                // E欄: 金額 (單位: 千元)
                excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item.price / 1000 } }, targetRow1, 5);

                // ==== 第二次寫入：第7行 ====
                if (ResearchFeesList.Count > 1)
                {
                    var item2 = ResearchFeesList[1];
                    int targetRow2 = 7;
                    DateTime? startDate2 = DateTime.TryParse(item2.dateStart, out var s2) ? s2 : (DateTime?)null;
                    DateTime? endDate2 = DateTime.TryParse(item2.dateEnd, out var e2) ? e2 : (DateTime?)null;
                    // 轉成民國日期
                    string startMinguo2 = startDate2.HasValue ? DateTimeHelper.ToMinguoDate(startDate2.Value) : "";
                    string endMinguo2 = endDate2.HasValue ? DateTimeHelper.ToMinguoDate(endDate2.Value) : "";

                    string dateRange2 = $"{startMinguo2} ~ {endMinguo2}";
                    excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item2.category ?? "" } }, targetRow2, 1);
                    excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { dateRange2 } }, targetRow2, 2);
                    excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item2.projectName ?? "" } }, targetRow2, 3);
                    excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item2.targetPerson ?? "" } }, targetRow2, 4);
                    excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { item2.price / 1000 } }, targetRow2, 5);

                    // 計算總和：ResearchFeesList[0].price + ResearchFeesList[1].price (單位: 千元)
                    decimal totalAmount = (item.price + item2.price) / 1000;

                    // 總和位置：E9
                    int totalRow = 9;
                    excel.WriteRange(targetSheet, new List<List<object>> { new List<object> { totalAmount } }, totalRow, 5);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FillResearchFeeSheet Error: {ex.Message}");
        }
    }

    private void FillTravelExpenseSheet(ExcelHelper excel, List<string> sheetNames, string projectID)
    {
        try
        {
            // 找到 '消耗性器材及原材料費' sheet
            string targetSheet = "國內差旅費";
            if (!sheetNames.Contains(targetSheet))
            {
                // 如果沒有找到目標 sheet，使用第4個 sheet
                targetSheet = sheetNames[4];
            }

            // 從資料庫讀取材料明細表資料
            var TripFormList = OFS_SciFundingHelper.GetTripFormList(projectID);

            if (TripFormList != null && TripFormList.Count > 0)
            {
                // 資料插入在第3行和第4行之間，所以從第4行開始插入
                int insertPosition = 4;

                // 先插入需要的行數 (資料筆數)
                if (TripFormList.Count > 0)
                {
                    excel.InsertRows(targetSheet, insertPosition, TripFormList.Count);
                }

                // 填入資料 (從第4行開始)
                for (int i = 0; i < TripFormList.Count; i++)
                {
                    var item = TripFormList[i];
                    int currentRow = insertPosition + i;

                    // A欄: 出差事由
                    var dataA = new List<List<object>>
                    {
                        new List<object> { item.reason ?? "" }
                    };
                    excel.WriteRange(targetSheet, dataA, currentRow, 1);

                    // B欄: 地區 
                    var dataB = new List<List<object>>
                    {
                        new List<object> { item.area ?? "" }
                    };
                    excel.WriteRange(targetSheet, dataB, currentRow, 2);

                    // C欄: 天數
                    var dataC = new List<List<object>>
                    {
                        new List<object> { item.days }
                    };
                    excel.WriteRange(targetSheet, dataC, currentRow, 3);

                    // D欄: 人次
                    var dataD = new List<List<object>>
                    {
                        new List<object> { item.people }
                    };
                    excel.WriteRange(targetSheet, dataD, currentRow, 4);

                    // E欄: 金額 (單位: 千元)
                    var dataE = new List<List<object>>
                    {
                        new List<object> { item.price / 1000 }
                    };
                    excel.WriteRange(targetSheet, dataE, currentRow, 5);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FillTravelExpenseSheet Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 填寫其他業務費 sheet
    /// </summary>
    private void FillOtherExpenseSheet(ExcelHelper excel, List<string> sheetNames, string projectID)
    {
        try
        {
            // 找到 '其他業務費' sheet
            string targetSheet = "其他業務費";
            if (!sheetNames.Contains(targetSheet))
            {
                // 如果沒有找到目標 sheet，使用第5個 sheet
                targetSheet = sheetNames[5];
            }

            // 從資料庫讀取其他業務費資料
            var otherExpenseList = OFS_SciFundingHelper.GetOtherObjectFeeList(projectID);

            if (otherExpenseList != null && otherExpenseList.Count > 0)
            {
                // 第一筆資料
                var firstItem = otherExpenseList[0];

                // B4: LIST[0].Price (金額，單位: 千元)
                var dataB4 = new List<List<object>>
                {
                    new List<object> { firstItem.amount / 1000 }
                };
                excel.WriteRange(targetSheet, dataB4, 4, 2);

                // C4: LIST[0].CalDescription (計算說明)
                var dataC4 = new List<List<object>>
                {
                    new List<object> { firstItem.note ?? "" }
                };
                excel.WriteRange(targetSheet, dataC4, 4, 3);

                // 如果有第二筆資料
                if (otherExpenseList.Count > 1)
                {
                    var secondItem = otherExpenseList[1];

                    // B5: LIST[1].Price (金額，單位: 千元)
                    var dataB5 = new List<List<object>>
                    {
                        new List<object> { secondItem.amount / 1000 }
                    };
                    excel.WriteRange(targetSheet, dataB5, 5, 2);

                    // C5: LIST[1].CalDescription 根據「,」切割，取第一段
                    int remainingCount = 0;
                    if (!string.IsNullOrEmpty(secondItem.note))
                    {
                        string[] segments = secondItem.note.Split(',');

                        if (segments.Length > 0)
                        {
                            // C5: 第一段
                            string firstSegment = segments[0];
                            var dataC5 = new List<List<object>>
                            {
                                new List<object> { firstSegment }
                            };
                            excel.WriteRange(targetSheet, dataC5, 5, 3);

                            // 如果有剩余的 segments (segments.Length - 1)
                            remainingCount = segments.Length - 1;
                            if (remainingCount > 0)
                            {
                                // 插入新行（從第6行開始插入 remainingCount 行）
                                excel.InsertRows(targetSheet, 6, remainingCount);

                                // 填入剩余的 segments 到 C 列（從 C6 開始）
                                for (int i = 1; i < segments.Length; i++)
                                {
                                    int rowIndex = 5 + i; // C6 = row 6, C7 = row 7, ...
                                    var dataC = new List<List<object>>
                                    {
                                        new List<object> { segments[i] }
                                    };
                                    excel.WriteRange(targetSheet, dataC, rowIndex, 3);
                                }
                            }
                        }
                    }

                    // 計算總和：LIST[0].Price + LIST[1].Price (單位: 千元)
                    decimal totalAmount = (firstItem.amount + secondItem.amount) / 1000;

                    // 總和位置：B{6 + segments.length - 1} = B{6 + remainingCount}
                    int totalRow = 6 + remainingCount;
                    var dataTotalB = new List<List<object>>
                    {
                        new List<object> { totalAmount }
                    };
                    excel.WriteRange(targetSheet, dataTotalB, totalRow, 2);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FillOtherExpenseSheet Error: {ex.Message}");
        }
    }

    #endregion

    /// <summary>
    /// 處理需要動態內容的範本檔案
    /// </summary>
    private string ProcessDynamicTemplate(string fileCode, string originalFilePath, string projectID)
    {
        // 從資料庫取得申請主檔資料


        switch (fileCode)
        {
            //-----------以下為科技業者----------------
            case "FILE_OTech1":
                return ApplyProjectDataToWord_FILE_OTech1(originalFilePath, projectID);
            case "FILE_OTech3":
                return ApplyProjectDataToWord_FILE_OTech3(originalFilePath, projectID);
            case "FILE_OTech4":
                return ApplyProjectDataToWord_FILE_OTech4(originalFilePath, projectID);
            case "FILE_OTech8":
                return ApplyProjectDataToWord_FILE_OTech8(originalFilePath, projectID);
            case "FILE_OTech11":
                return ApplyProjectDataToWord_FILE_OTech11(originalFilePath, projectID);
            //---------------以下為學界、法人----------------
            case "FILE_AC1":
                return ApplyProjectDataToWord_FILE_AC1(originalFilePath, projectID);
            case "FILE_AC3":
                return ApplyProjectDataToWord_FILE_OTech3(originalFilePath, projectID); // 檔案相同 借用
            case "FILE_AC4":
                return ApplyProjectDataToWord_FILE_OTech4(originalFilePath, projectID); // 檔案相同 借用
            case "FILE_AC9":
                return ApplyProjectDataToWord_FILE_OTech11(originalFilePath, projectID); //替換文字與OTech11相同 借用
            case "FILE_AC11":
                return ApplyProjectDataToWord_FILE_AC11(originalFilePath, projectID); //替換文字與OTech11相同 借用
            case "FILE_AC13":
                return ApplyProjectDataToWord_FILE_OTech11(originalFilePath, projectID); //檔案相同 借用
            default:
                return originalFilePath; // 不需要動態處理的範本直接返回原路徑
        }
    }

    #region 動態範本處理方法

    /// <summary>
    /// 處理海洋委員會海洋科技專案補助作業要點 (OTech)
    /// </summary>
    private string ApplyProjectDataToWord_FILE_OTech1(string originalFilePath, string projectId)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);
                // 取得當前年月日 (參考 DownloadTemplateCUL 的實作)
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year - 1911; // 民國年
                int month = currentDate.Month;

                // 建立替換字典
                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{Year}}", year.ToString());
                placeholder.Add("{{Month}}", month.ToString());

                var repeatData = new List<Dictionary<string, string>>();

                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
            }

            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_OTech1 Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 處理海洋委員會海洋科技專案補助作業要點 (Academic)
    /// </summary>
    private string ApplyProjectDataToWord_FILE_AC1(string originalFilePath, string projectId)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);
                // 取得當前年月日 (參考 DownloadTemplateCUL 的實作)
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year - 1911; // 民國年
                int month = currentDate.Month;

                // 建立替換字典
                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{Year}}", year.ToString());
                placeholder.Add("{{Month}}", month.ToString());
                var repeatData = new List<Dictionary<string, string>>();

                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
            }

            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_AC1 Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 處理建議迴避之審查委員清單 (OTech)
    /// </summary>
    private string ApplyProjectDataToWord_FILE_OTech3(string originalFilePath, string projectID)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            OFS_SCI_Application_Main applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);

            // 取得迴避名單資料
            var recusedList = OFS_SciRecusedList.GetRecusedListByProjectID(projectID);

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);

                // 取得當前日期（民國年月日）
                DateTime currentDate = DateTime.Now;
                int year = DateTimeHelper.GregorianYearToMinguo(currentDate.Year);
                int month = currentDate.Month;
                int day = currentDate.Day;

                // 建立替換字典
                var placeholder = new Dictionary<string, string>();

                // 加入計畫名稱
                placeholder.Add("ProjectNameTw", applicationMain?.ProjectNameTw ?? "");
                placeholder.Add("OrgName", applicationMain?.OrgName ?? "");
                placeholder.Add("TYear", year.ToString());
                placeholder.Add("TMonth", month.ToString());
                placeholder.Add("TDay", day.ToString());

                var repeatData = new List<Dictionary<string, string>>();

                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);

                // 處理迴避名單表格
                if (recusedList != null && recusedList.Count > 0)
                {
                    // 新增表格行數（扣除標題行，所以要新增 recusedList.Count 行）
                    helper.SetTableRowCount("recused", recusedList.Count);

                    // 填入資料到表格（從第 1 行開始，第 0 行是標題）
                    for (int i = 0; i < recusedList.Count; i++)
                    {
                        var recused = recusedList[i];
                        int rowIndex = i + 1; // 第 0 行是標題，資料從第 1 行開始

                        // 欄位順序：姓名、任職單位、職稱、應迴避之具體理由及事證
                        helper.SetTableCellValue("recused", rowIndex, 0, recused.RecusedName ?? "");
                        helper.SetTableCellValue("recused", rowIndex, 1, recused.EmploymentUnit ?? "");
                        helper.SetTableCellValue("recused", rowIndex, 2, recused.JobTitle ?? "");
                        helper.SetTableCellValue("recused", rowIndex, 3, recused.RecusedReason ?? "");
                    }
                }
                else
                {
                    // 如果沒有資料，新增一個空白行
                    helper.SetTableRowCount("recused", 1);
                    helper.SetTableCellValue("recused", 1, 0, "無");
                    helper.SetTableCellValue("recused", 1, 1, "無");
                    helper.SetTableCellValue("recused", 1, 2, "無");
                    helper.SetTableCellValue("recused", 1, 3, "無");
                }

                helper.CloseAsSave();
            }

            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_OTech3 Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 處理未違反公職人員利益衝突迴避法切結書 (OTech)
    /// </summary>
    private string ApplyProjectDataToWord_FILE_OTech4(string originalFilePath, string projectID)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);
            OFS_SCI_Application_Main applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);

                // 取得當前年月日 (參考 DownloadTemplateCUL 的實作)
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year - 1911; // 民國年
                int month = currentDate.Month;
                int day = currentDate.Day;


                // 建立替換字典
                var placeholder = new Dictionary<string, string>();
                placeholder.Add("year", year.ToString());
                placeholder.Add("month", month.ToString());
                placeholder.Add("day", day.ToString());

                // 加入申請資料
                placeholder.Add("{{A3}}", applicationMain?.ProjectNameTw ?? "");
                placeholder.Add("{{A9}}", applicationMain?.OrgName ?? "");

                var repeatData = new List<Dictionary<string, string>>();

                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
            }

            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_OTech4 Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 處理海洋科技業界科專計畫補助契約書 (OTech)
    /// </summary>
    private string ApplyProjectDataToWord_FILE_OTech8(string originalFilePath, string projectID)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 取得專案資料
            OFS_SCI_Application_Main applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

            // 取得計畫主持人資料
            string personID = "P" + projectID;
            var personnelList = OFS_SciApplicationHelper.GetPersonnelByPersonID(personID);
            var host = personnelList?.FirstOrDefault(p => p.Role == "計畫主持人");
            string hostName = host?.Name ?? "";

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);

                // 取得當前日期（民國年月日）
                DateTime currentDate = DateTime.Now;
                int year = DateTimeHelper.GregorianYearToMinguo(currentDate.Year);
                int month = currentDate.Month;
                int day = currentDate.Day;

                // 建立替換字典
                var placeholder = new Dictionary<string, string>();

                // 加入日期
                placeholder.Add("TYear", year.ToString());
                placeholder.Add("TMonth", month.ToString());
                placeholder.Add("TDay", day.ToString());

                // 加入計畫資料 
                placeholder.Add("ProjectNameTw", applicationMain?.ProjectNameTw ?? "");
                placeholder.Add("HostSign", hostName);

                var repeatData = new List<Dictionary<string, string>>();

                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
            }

            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_OTech8 Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 處理研究紀錄簿使用原則 (OTech)
    /// </summary>
    private string ApplyProjectDataToWord_FILE_OTech11(string originalFilePath, string projectID)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 取得專案資料
            OFS_SCI_Application_Main applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);
                // 建立替換字典
                var placeholder = new Dictionary<string, string>();

                // 加入計畫資料
                placeholder.Add("ProjectID", projectID ?? "");
                placeholder.Add("ProjectNameTw", applicationMain?.ProjectNameTw ?? "");
                placeholder.Add("OrgName", applicationMain?.OrgName ?? "");
                var repeatData = new List<Dictionary<string, string>>();
                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
            }

            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_OTech11 Error: {ex.Message}");
            return originalFilePath;
        }
    }

    /// <summary>
    /// 處理海洋科技專案成效追蹤自評表 (Academic)
    /// </summary>
    private string ApplyProjectDataToWord_FILE_AC11(string originalFilePath, string projectID)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 取得專案資料
            OFS_SCI_Application_Main applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);

            // 取得經費總計資料
            var totalFeeSum = OFS_SciFundingHelper.GetTotalFeeSum(projectID);

            // 取得人員資料（主持人和聯絡人）
            string personID = "P" + projectID;
            var personnelList = OFS_SciApplicationHelper.GetPersonnelByPersonID(personID);
            var host = personnelList?.FirstOrDefault(p => p.Role == "計畫主持人");
            var contact = personnelList?.FirstOrDefault(p => p.Role == "計畫聯絡人");

            // 將 Topic 和 Field 的代碼轉換為中文名稱
            string topicName = SysZgsCodeHelper.GetDescnameByCode("SCITopic", applicationMain?.Topic ?? "");
            string fieldName = SysZgsCodeHelper.GetDescnameByCode("SCIField", applicationMain?.Field ?? "");

            // 處理開始日期和結束日期（轉換為民國年月日）
            int sYear = 0, sMonth = 0, sDay = 0;
            int eYear = 0, eMonth = 0, eDay = 0;

            if (applicationMain?.StartTime != null)
            {
                DateTime startTime = applicationMain.StartTime.Value;
                sYear = DateTimeHelper.GregorianYearToMinguo(startTime.Year);
                sMonth = startTime.Month;
                sDay = startTime.Day;
            }

            if (applicationMain?.EndTime != null)
            {
                DateTime endTime = applicationMain.EndTime.Value;
                eYear = DateTimeHelper.GregorianYearToMinguo(endTime.Year);
                eMonth = endTime.Month;
                eDay = endTime.Day;
            }

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);

            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);

            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);

                // 建立替換字典
                var placeholder = new Dictionary<string, string>();

                // 加入計畫資料 
                placeholder.Add("ProjectID", projectID ?? "");
                placeholder.Add("ProjectNameTw", applicationMain?.ProjectNameTw ?? "");
                placeholder.Add("OrgName", applicationMain?.OrgName ?? "");
                placeholder.Add("Topic", topicName);
                placeholder.Add("Field", fieldName);

                // 加入主持人資料
                placeholder.Add("HostName", host?.Name ?? "");
                placeholder.Add("HostMPhone", host?.MobilePhone ?? "");

                // 加入聯絡人資料
                placeholder.Add("ContactName", contact?.Name ?? "");
                placeholder.Add("ContactMPhone", contact?.MobilePhone ?? "");

                // 加入開始日期（民國年月日）
                placeholder.Add("SYear", sYear.ToString());
                placeholder.Add("SMonth", sMonth.ToString());
                placeholder.Add("SDay", sDay.ToString());

                // 加入結束日期（民國年月日）
                placeholder.Add("EYear", eYear.ToString());
                placeholder.Add("EMonth", eMonth.ToString());
                placeholder.Add("EDay", eDay.ToString());

                // 建立使用點號作為千分位分隔符的格式化設定
                var numberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                numberFormat.NumberGroupSeparator = ".";

                // 加入經費資料（千分位格式：1.100）
                placeholder.Add("SubsidyAmount", totalFeeSum.SubsidyAmount.ToString("N0", numberFormat));
                placeholder.Add("CoopAmount", totalFeeSum.CoopAmount.ToString("N0", numberFormat));
                placeholder.Add("TotalAmount", totalFeeSum.TotalAmount.ToString("N0", numberFormat));

                var repeatData = new List<Dictionary<string, string>>();

                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
            }

            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_AC11 Error: {ex.Message}");
            return originalFilePath;
        }
    }

    #endregion

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
    /// 取得契約檔案列表
    /// </summary>
    private void GetContractFiles(HttpContext context)
    {
        try
        {
            context.Response.ContentType = "application/json";

            string projectID = context.Request.QueryString["projectID"];
            string fileCode = context.Request.QueryString["fileCode"];

            if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(fileCode))
            {
                context.Response.Write("{\"success\":false,\"message\":\"缺少必要參數\"}");
                return;
            }

            // 從資料庫取得檔案列表
            var files = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectID, fileCode);

            if (files == null || files.Count == 0)
            {
                context.Response.Write("{\"success\":true,\"files\":[]}");
                return;
            }

            // 組成 JSON 回應
            var fileList = new System.Text.StringBuilder();
            fileList.Append("{\"success\":true,\"files\":[");

            for (int i = 0; i < files.Count; i++)
            {
                if (i > 0) fileList.Append(",");

                var file = files[i];
                string fileName = System.Web.HttpUtility.JavaScriptStringEncode(file.FileName ?? "");
                string templatePath = System.Web.HttpUtility.JavaScriptStringEncode(file.TemplatePath ?? "");

                fileList.Append($"{{\"FileName\":\"{fileName}\",\"TemplatePath\":\"{templatePath}\"}}");
            }

            fileList.Append("]}");
            context.Response.Write(fileList.ToString());
        }
        catch (Exception ex)
        {
            string escapedMessage = System.Web.HttpUtility.JavaScriptStringEncode(ex.Message);
            context.Response.Write($"{{\"success\":false,\"message\":\"取得檔案列表失敗：{escapedMessage}\"}}");
        }
    }

    public bool IsReusable => false;
}
<%@ WebHandler Language="C#" Class="SCI_Download" %>
<%@ Assembly Name="System.IO.Compression.FileSystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" %>

using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
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
                default:
                    context.Response.StatusCode = 400;
                    context.Response.Write("Invalid action. Use 'downloadPlan', 'downloadApprovedPlan', 'downloadTemplate', 'downloadFile', or 'downloadReviewFile'.");
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
                return "~/Template/SCI/OTech/附件-01海洋委員會海洋科技專案補助作業要點.docx";
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
                return "~/Template/SCI/OTech/附件-07簽約注意事項.docx";
            case "FILE_OTech8":
                return "~/Template/SCI/OTech/附件-08海洋科技業界科專計畫補助契約書.docx";
            case "FILE_OTech9":
                return "~/Template/SCI/OTech/附件-09研究紀錄簿使用原則.docx";
            case "FILE_OTech10":
                return "~/Template/SCI/OTech/附件-10海洋科技專案計畫會計科目編列與執行原則.docx";
            case "FILE_OTech11":
                return "~/Template/SCI/OTech/附件-11計畫書書脊（側邊）格式.docx";

            // Academic 學研範本檔案對應
            case "FILE_AC1":
                return "~/Template/SCI/Academic/附件-01海洋委員會海洋科技專案補助作業要點.docx";
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
                return "~/Template/SCI/Academic/附件-08簽約注意事項.docx";
            case "FILE_AC9":
                return "~/Template/SCI/Academic/附件-09海洋委員會補助科技專案計畫契約書.docx";
            case "FILE_AC10":
                return "~/Template/SCI/Academic/附件-10海洋科技專案計畫會計科目編列與執行原則.docx";
            case "FILE_AC11":
                return "~/Template/SCI/Academic/附件-11海洋科技專案成效追蹤自評表.docx";
            case "FILE_AC12":
                return "~/Template/SCI/Academic/附件-12研究紀錄簿使用原則.docx";
            case "FILE_AC13":
                return "~/Template/SCI/Academic/附件-13計畫書書脊（側邊）格式.docx";

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
                string relativePath = L_TechnologyDiagram[0].TemplatePath;  // 或 FilePath，看你模型欄位名稱
                string absolutePath = HttpContext.Current.Server.MapPath("~/" + relativePath);
                helper.InsertImageAtBookmark("TechnologyDiagram", absolutePath,6400800, 4257567 );

                List<OFS_SCI_UploadFile> L_WorkSchStructure = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectID, "WorkSchStructure");
                relativePath = L_WorkSchStructure[0].TemplatePath;
                absolutePath = HttpContext.Current.Server.MapPath("~/" + relativePath);
                // 插入圖片：TechnologyDiagram 和 WorkSchStructure
                helper.InsertImageAtBookmark("WorkSchStructure", absolutePath,6400800, 4257567);

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

                // TODO: 根據實際需求設定儲存格值
                // 範例：excel.WriteRange(sheetName, data, startRow, startColumn);

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

                // TODO: 根據實際需求設定儲存格值

                excel.Save();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessTemplate_AC2_File2_Schedule Error: {ex.Message}");
        }
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

                string sheetName = sheetNames[0];

                // TODO: 根據實際需求設定儲存格值
                // 範例：
                // var data = new List<List<object>>
                // {
                //     new List<object> { "計畫編號", projectID },
                //     new List<object> { "計畫名稱", applicationMain.ProjectNameTw },
                //     new List<object> { "申請單位", applicationMain.OrgName }
                // };
                // excel.WriteRange(sheetName, data, 1, 1);

                excel.Save();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ProcessTemplate_AC2_File4_Budget Error: {ex.Message}");
        }
    }

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
                return ApplyProjectDataToWord_FILE_OTech4(originalFilePath,projectID);
            case "FILE_OTech8":
                return ApplyProjectDataToWord_FILE_OTech8(originalFilePath,projectID); 
            case "FILE_OTech11":
                return ApplyProjectDataToWord_FILE_OTech11(originalFilePath,projectID);
            //---------------以下為學界、法人----------------
            case "FILE_AC1":
                return ApplyProjectDataToWord_FILE_AC1(originalFilePath, projectID);
            case "FILE_AC3":
                return ApplyProjectDataToWord_FILE_OTech3(originalFilePath, projectID); // 檔案相同 借用
            case "FILE_AC4":
                return ApplyProjectDataToWord_FILE_OTech4(originalFilePath,projectID); // 檔案相同 借用
            case "FILE_AC9":
                return ApplyProjectDataToWord_FILE_OTech11(originalFilePath,projectID); //替換文字與OTech11相同 借用
            case "FILE_AC11":
                return ApplyProjectDataToWord_FILE_AC11(originalFilePath,projectID); //替換文字與OTech11相同 借用
            case "FILE_AC13":
                return ApplyProjectDataToWord_FILE_OTech11(originalFilePath,projectID); //檔案相同 借用
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

    public bool IsReusable => false;
}
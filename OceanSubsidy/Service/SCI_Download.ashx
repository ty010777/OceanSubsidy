<%@ WebHandler Language="C#" Class="SCI_Download" %>

using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using GS.App;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;

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
        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write($"Template file not found: {templatePath}");
            return;
        }

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
                return "~/Template/SCI/OTech/附件-02海洋科技科專案計畫書.zip";
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
                return "~/Template/SCI/Academic/附件-02海洋科技科專案計畫書.zip";
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
    /// 處理需要動態內容的範本檔案
    /// </summary>
    private string ProcessDynamicTemplate(string fileCode, string originalFilePath, string projectID)
    {
         // 從資料庫取得申請主檔資料
    

        switch (fileCode)
        {
            case "FILE_OTech1":
                return ApplyProjectDataToWord_FILE_OTech1(originalFilePath, projectID);
            case "FILE_OTech3":
                return ApplyProjectDataToWord_FILE_OTech3(originalFilePath, projectID);
            case "FILE_OTech4":
                return ApplyProjectDataToWord_FILE_OTech4(originalFilePath,projectID);
            case "FILE_OTech8":
                return ApplyProjectDataToWord_FILE_OTech8(originalFilePath,projectID);
            case "FILE_AC1":
                return ApplyProjectDataToWord_FILE_AC1(originalFilePath, projectID);
            
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
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

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
                placeholder.Add("UserName", projectData?.UserName ?? "");
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
            TYear TMonth TDay
            // 取得專案資料
            OFS_SCI_Application_Main applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

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
                placeholder.Add("year", year.ToString());
                placeholder.Add("month", month.ToString());
                placeholder.Add("day", day.ToString());

                // 加入計畫資料
                placeholder.Add("ProjectNameTw", applicationMain?.ProjectNameTw ?? "");
                placeholder.Add("OrgName", applicationMain?.OrgName ?? "");
                placeholder.Add("UserName", projectData?.UserName ?? "");
                placeholder.Add("ProjectID", projectID ?? "");

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
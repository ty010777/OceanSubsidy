<%@ WebHandler Language="C#" Class="DownloadApplicationChecklistFile" %>

using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;

public class DownloadApplicationChecklistFile : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var action = context.Request.QueryString["action"];

        try
        {
            switch (action)
            {
                case "downloadTechReview":
                    var projectId = context.Request.QueryString["projectId"];
                    if (string.IsNullOrEmpty(projectId))
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Write("Missing projectId parameter.");
                        return;
                    }
                    DownloadTechReviewFile(context, projectId);
                    break;

                case "batchExportPresentations":
                    var projectIds = context.Request.QueryString["projectIds"];
                    if (string.IsNullOrEmpty(projectIds))
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Write("Missing projectIds parameter.");
                        return;
                    }
                    BatchExportPresentations(context, projectIds);
                    break;

                case "exportReviewCommentReply":
                    var exportProjectId = context.Request.QueryString["projectId"];
                    var reviewType = context.Request.QueryString["reviewType"];
                    if (string.IsNullOrEmpty(exportProjectId))
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Write("Missing projectId parameter.");
                        return;
                    }
                    if (string.IsNullOrEmpty(reviewType))
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Write("Missing reviewType parameter.");
                        return;
                    }
                    ExportReviewCommentReply(context, exportProjectId, reviewType);
                    break;

                case "exportApplicationData":
                    var applicationProjectId = context.Request.QueryString["projectId"];
                    if (string.IsNullOrEmpty(applicationProjectId))
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Write("Missing projectId parameter.");
                        return;
                    }
                    ExportApplicationData(context, applicationProjectId);
                    break;

                default:
                    context.Response.StatusCode = 400;
                    context.Response.Write("Invalid action parameter.");
                    break;
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"Error processing request: {ex.Message}");
        }
    }

    private void DownloadTechReviewFile(HttpContext context, string projectId)
    {
        // 判斷專案類型
        string projectType = GetProjectTypeFromId(projectId);
        if (string.IsNullOrEmpty(projectType))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("無法識別專案類型");
            return;
        }

        string uploadFolder = context.Server.MapPath($"~/UploadFiles/OFS/{projectType}/{projectId}/TechReviewFiles/");
        
        if (!Directory.Exists(uploadFolder))
        {
            context.Response.StatusCode = 404;
            context.Response.Write("檔案不存在");
            return;
        }

        // 尋找對應的檔案
        string[] files = Directory.GetFiles(uploadFolder, $"{projectId}_TechReview_*");
        
        if (files.Length == 0)
        {
            context.Response.StatusCode = 404;
            context.Response.Write("找不到對應的檔案");
            return;
        }

        // 取得最新的檔案
        string filePath = files[0];
        string fileName = Path.GetFileName(filePath);
        
        // 產生下載檔案名稱
        string extension = Path.GetExtension(fileName);
        string downloadFileName = $"{projectId}_技術審查檔案{extension}";

        // 處理中文檔名編碼問題
        string encodedFileName = System.Web.HttpUtility.UrlEncode(downloadFileName, System.Text.Encoding.UTF8);

        // 設定回應標頭
        string contentType = GetContentType(extension);
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
    /// 批次匯出簡報檔案
    /// </summary>
    private void BatchExportPresentations(HttpContext context, string projectIdsParam)
    {
        try
        {
            // 解析專案編號列表
            var projectIdsList = projectIdsParam.Split(',')
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .ToList();

            if (projectIdsList.Count == 0)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("沒有有效的專案編號");
                return;
            }

            // 呼叫 Helper 方法進行檔案彙整
            var exportResult = ReviewCheckListHelper.ExportBatchPresentations(projectIdsList);

            if (exportResult == null || !File.Exists(exportResult.ZipFilePath))
            {
                context.Response.StatusCode = 500;
                context.Response.Write("檔案匯出失敗");
                return;
            }

            // 產生下載檔案名稱
            string downloadFileName = $"技術審查簡報匯出_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            string encodedFileName = System.Web.HttpUtility.UrlEncode(downloadFileName, System.Text.Encoding.UTF8);

            // 設定回應標頭
            context.Response.Clear();
            context.Response.ContentType = "application/zip";
            context.Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
            context.Response.AddHeader("Content-Length", new FileInfo(exportResult.ZipFilePath).Length.ToString());
            context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Expires", "0");

            // 讀取檔案並寫入 Response
            byte[] fileData = File.ReadAllBytes(exportResult.ZipFilePath);
            context.Response.BinaryWrite(fileData);
            context.Response.Flush();

            // 清理暫時檔案
            CleanupTempFiles(exportResult);

            context.Response.SuppressContent = true;
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (FileNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            context.Response.Write(ex.Message);
        }
        catch (ArgumentException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.Write(ex.Message);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"處理批次匯出時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 清理暫時檔案
    /// </summary>
    private void CleanupTempFiles(BatchPresentationExportResult exportResult)
    {
        try
        {
            // 刪除ZIP檔案
            if (!string.IsNullOrEmpty(exportResult.ZipFilePath) && File.Exists(exportResult.ZipFilePath))
            {
                File.Delete(exportResult.ZipFilePath);
            }

            // 刪除暫時目錄
            if (!string.IsNullOrEmpty(exportResult.TempDirectory) && Directory.Exists(exportResult.TempDirectory))
            {
                Directory.Delete(exportResult.TempDirectory, true);
            }
        }
        catch (Exception ex)
        {
            // 記錄清理錯誤，但不影響主要流程
            System.Diagnostics.Debug.WriteLine($"清理暫時檔案時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 從專案ID判斷專案類型
    /// </summary>
    private static string GetProjectTypeFromId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            return null;

        // 技術審查階段僅支援 SCI 和 CUL 兩種補助案
        if (projectId.ToUpper().Contains("SCI"))
            return "SCI";
        else if (projectId.ToUpper().Contains("CUL"))
            return "CUL";

        return null; // 僅支援 SCI 和 CUL
    }

    /// <summary>
    /// 取得檔案的 MIME 類型
    /// </summary>
    private static string GetContentType(string extension)
    {
        switch (extension.ToLower())
        {
            case ".ppt":
                return "application/vnd.ms-powerpoint";
            case ".pptx":
                return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            default:
                return "application/octet-stream";
        }
    }

    /// <summary>
    /// 匯出審查意見回覆表
    /// </summary>
    private void ExportReviewCommentReply(HttpContext context, string projectId, string reviewType)
    {
        try
        {
            // 取得計畫基本資料
            var projectData = ApplicationChecklistHelper.GetProjectDataForReview(projectId);
            if (projectData == null)
            {
                context.Response.StatusCode = 404;
                context.Response.Write("找不到計畫資料");
                return;
            }

            // 取得審查意見資料
            var reviewComments = GetReviewCommentsForExport(projectId, reviewType);
            if (reviewComments == null || reviewComments.Count == 0)
            {
                // 根據專案類型決定審查階段名稱
                string reviewStageTypeName = "";
                if (projectId.Contains("SCI"))
                {
                    reviewStageTypeName = reviewType == "domain" ? "實質審查" : "技術審查";
                }
                else if (projectId.Contains("CUL"))
                {
                    reviewStageTypeName = reviewType == "domain" ? "初審" : "複審";
                }
                else
                {
                    reviewStageTypeName = reviewType == "domain" ? "2" : "3"; // 預設值
                }

                context.Response.StatusCode = 404;
                context.Response.Write($"找不到{reviewStageTypeName}意見資料");
                return;
            }

            // 建立 Word 檔案
            var wordFileBytes = GenerateReviewCommentReplyWord(projectData, reviewComments, reviewType);
            if (wordFileBytes == null)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("產生 Word 檔案失敗");
                return;
            }

            // 產生下載檔案名稱
            string downloadFileTypeName = "";
            if (projectId.Contains("SCI"))
            {
                downloadFileTypeName = reviewType == "domain" ? "實質審查意見回覆表" : "技術審查意見回覆表";
            }
            else if (projectId.Contains("CUL"))
            {
                downloadFileTypeName = reviewType == "domain" ? "初審意見回覆表" : "複審意見回覆表";
            }
            else
            {
                downloadFileTypeName = reviewType == "domain" ? "實質審查意見回覆表" : "技術審查意見回覆表"; // 預設值
            }

            string downloadFileName = $"{projectId}_{downloadFileTypeName}_{DateTime.Now:yyyyMMdd}.docx";
            string encodedFileName = System.Web.HttpUtility.UrlEncode(downloadFileName, System.Text.Encoding.UTF8);

            // 設定回應標頭
            context.Response.Clear();
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            context.Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
            context.Response.AddHeader("Content-Length", wordFileBytes.Length.ToString());
            context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Expires", "0");

            // 寫入檔案內容
            context.Response.BinaryWrite(wordFileBytes);
            context.Response.Flush();
            context.Response.SuppressContent = true;
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"匯出實質審查意見回覆表時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 取得審查意見資料供匯出使用
    /// </summary>
    private List<ReviewCommentItem> GetReviewCommentsForExport(string projectId, string reviewType)
    {
        var reviewComments = new List<ReviewCommentItem>();

        try
        {
            string stage1 = "";
            string stage2 = "";

            if (projectId.Contains("SCI"))
            {
                stage1 = "2";
                stage2 = "3";

                // 根據 reviewType 決定查詢哪種審查意見
                if (reviewType == "domain")
                {
                    // 只查詢實質審查意見
                    var domainCommentsTable = ReviewCheckListHelper.GetSciReviewComments(projectId, stage1);
                    if (domainCommentsTable != null && domainCommentsTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in domainCommentsTable.Rows)
                        {
                            reviewComments.Add(new ReviewCommentItem
                            {
                                ReviewerName = row["ReviewerName"]?.ToString() ?? "",
                                ReviewComment = row["ReviewComment"]?.ToString() ?? "",
                                ReplyComment = row["ReplyComment"]?.ToString() ?? "",
                                Stage = "實質審查"
                            });
                        }
                    }
                }
                else if (reviewType == "technical")
                {
                    // 只查詢技術審查意見
                    var technicalCommentsTable = ReviewCheckListHelper.GetSciReviewComments(projectId, stage2);
                    if (technicalCommentsTable != null && technicalCommentsTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in technicalCommentsTable.Rows)
                        {
                            reviewComments.Add(new ReviewCommentItem
                            {
                                ReviewerName = row["ReviewerName"]?.ToString() ?? "",
                                ReviewComment = row["ReviewComment"]?.ToString() ?? "",
                                ReplyComment = row["ReplyComment"]?.ToString() ?? "",
                                Stage = "技術審查"
                            });
                        }
                    }
                }
            }
            else if (projectId.Contains("CUL"))
            {
                stage1 = "2"; // 初審
                stage2 = "3"; // 複審

                // 根據 reviewType 決定查詢哪種審查意見
                if (reviewType == "domain")
                {
                    // 只查詢初審意見
                    var domainCommentsTable = ReviewCheckListHelper.GetCulturalReviewComments(projectId, stage1);
                    if (domainCommentsTable != null && domainCommentsTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in domainCommentsTable.Rows)
                        {
                            reviewComments.Add(new ReviewCommentItem
                            {
                                ReviewerName = row["ReviewerName"]?.ToString() ?? "",
                                ReviewComment = row["ReviewComment"]?.ToString() ?? "",
                                ReplyComment = row["ReplyComment"]?.ToString() ?? "",
                                Stage = "初審"
                            });
                        }
                    }
                }
                else if (reviewType == "technical")
                {
                    // 只查詢複審意見
                    var technicalCommentsTable = ReviewCheckListHelper.GetCulturalReviewComments(projectId, stage2);
                    if (technicalCommentsTable != null && technicalCommentsTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in technicalCommentsTable.Rows)
                        {
                            reviewComments.Add(new ReviewCommentItem
                            {
                                ReviewerName = row["ReviewerName"]?.ToString() ?? "",
                                ReviewComment = row["ReviewComment"]?.ToString() ?? "",
                                ReplyComment = row["ReplyComment"]?.ToString() ?? "",
                                Stage = "複審"
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查意見資料時發生錯誤: {ex.Message}");
        }

        return reviewComments;
    }

    /// <summary>
    /// 產生審查意見回覆表 Word 檔案
    /// </summary>
    private byte[] GenerateReviewCommentReplyWord(dynamic projectData, List<ReviewCommentItem> reviewComments, string reviewType)
    {
        try
        {
            // Word 範本檔案路徑
            string templatePath = HttpContext.Current.Server.MapPath("~/Template/Shared/審查意見及回覆意見對照表.docx");
            
            // 檢查範本檔案是否存在
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"找不到 Word 範本檔案: {templatePath}");
            }

            // 建立暫時檔案
            string tempFilePath = Path.GetTempFileName();
            File.Copy(templatePath, tempFilePath, true);

            using (var helper = new OpenXmlHelper(tempFilePath))
            {
                // 建立替換字典
                var placeholder = new Dictionary<string, string>();
                var currentDate = DateTime.Now;

                // 根據專案類型決定審查階段名稱
                string reviewTypeName = "";
                if (projectData.ProjectID != null && projectData.ProjectID.Contains("SCI"))
                {
                    reviewTypeName = reviewType == "domain" ? "實質審查" : "技術審查";
                }
                else if (projectData.ProjectID != null && projectData.ProjectID.Contains("CUL"))
                {
                    reviewTypeName = reviewType == "domain" ? "初審" : "複審";
                }
                else
                {
                    reviewTypeName = reviewType == "domain" ? "實質審查" : "技術審查"; // 預設值
                }

                placeholder.Add("{UserName}", projectData.UserName ?? "");
                placeholder.Add("{ProjectName}", projectData.ProjectName ?? "");
                placeholder.Add("{Year}", currentDate.Year.ToString());
                placeholder.Add("{Month}", currentDate.Month.ToString());
                placeholder.Add("{Day}", currentDate.Day.ToString());
                placeholder.Add("{reviewType}", reviewTypeName);

                // 準備審查意見重複資料
                var repeatData = new List<Dictionary<string, string>>();
                foreach (var comment in reviewComments)
                {
                    var commentData = new Dictionary<string, string>();
                    commentData.Add("{ReviewComment}", comment.ReviewComment ?? "");
                    commentData.Add("{ReplyComment}", comment.ReplyComment ?? "");
                    commentData.Add("{PageNumber}", ""); // 頁碼欄位預設空白
                    repeatData.Add(commentData);
                }

                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);

                // 如果需要額外處理表格行數，可以使用書籤方式
                if (reviewComments.Count > 0)
                {
                    helper.SetTableRowCount("ReviewTable", reviewComments.Count);

                    // 填入審查意見資料
                    for (int i = 0; i < reviewComments.Count; i++)
                    {
                        var comment = reviewComments[i];
                        helper.SetTableCellValue("ReviewTable", i + 1, 0, comment.ReviewComment);
                        helper.SetTableCellValue("ReviewTable", i + 1, 1, comment.ReplyComment);
                        helper.SetTableCellValue("ReviewTable", i + 1, 2, ""); // 頁碼欄位預設空白
                    }
                }
            }

            // 讀取檔案內容
            byte[] fileBytes = File.ReadAllBytes(tempFilePath);

            // 刪除暫時檔案
            File.Delete(tempFilePath);

            return fileBytes;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"產生 Word 檔案時發生錯誤: {ex.Message}");
            return null;
        }
    }


    /// <summary>
    /// 匯出申請資料（計畫書 PDF）
    /// </summary>
    private void ExportApplicationData(HttpContext context, string projectId)
    {
        try
        {
            string filePath = "";
            string fileName = "";
            string projectName = "";

            // 根據專案類型決定檔案路徑
            if (projectId.Contains("SCI"))
            {
                // 科專專案
                var applicationMain = ApplicationChecklistHelper.GetProjectDataForReview(projectId);
                projectName = applicationMain?.ProjectName ?? "";

                fileName = $"{projectId}_科專_{projectName}_送審版.pdf";
                string relativePath = $"~/UploadFiles/OFS/SCI/{projectId}/SciApplication/{fileName}";
                filePath = context.Server.MapPath(relativePath);
            }
            else if (projectId.Contains("CUL"))
            {
                // 文化專案
                var applicationMain = ApplicationChecklistHelper.GetProjectDataForReview(projectId);
                projectName = applicationMain?.ProjectName ?? "";

                fileName = $"{projectId}_文化_{projectName}_送審版.pdf";
                string relativePath = $"~/UploadFiles/OFS/CUL/{projectId}/TechReviewFiles/{fileName}";
                filePath = context.Server.MapPath(relativePath);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Write("不支援的專案類型");
                return;
            }

            // 檢查檔案是否存在
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write($"找不到申請資料檔案: {fileName}");
                return;
            }

            // 下載檔案
            DownloadPdfFile(context, filePath, fileName);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"匯出申請資料時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 執行 PDF 檔案下載
    /// </summary>
    private void DownloadPdfFile(HttpContext context, string filePath, string fileName)
    {
        try
        {
            // 處理中文檔名編碼問題
            string encodedFileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);

            // 設定回應標頭
            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
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
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"下載檔案時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 審查意見資料項目
    /// </summary>
    public class ReviewCommentItem
    {
        public string ReviewerName { get; set; }
        public string ReviewComment { get; set; }
        public string ReplyComment { get; set; }
        public string Stage { get; set; }
    }

    public bool IsReusable => false;
}
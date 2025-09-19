<%@ WebHandler Language="C#" Class="DownloadApplicationChecklistFile" %>

using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS;

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

    public bool IsReusable => false;
}
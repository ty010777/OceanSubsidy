<%@ WebHandler Language="C#" Class="DownloadReviewChecklistFile" %>

using System;
using System.Data;
using System.IO;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using GS.App;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;

/// <summary>
/// 審查清單檔案下載處理器
/// </summary>
public class DownloadReviewChecklistFile : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string type = context.Request.QueryString["type"] ?? "1";
            string exportType = context.Request.QueryString["exportType"] ?? "reviewing"; // reviewing, results, application, list, applicationPdf
            string year = context.Request.QueryString["year"] ?? "";
            string category = context.Request.QueryString["category"] ?? "";
            string status = context.Request.QueryString["status"] ?? "";
            string progress = context.Request.QueryString["progress"] ?? "";
            string replyStatus = context.Request.QueryString["replyStatus"] ?? "";
            string orgName = context.Request.QueryString["orgName"] ?? "";
            string supervisor = context.Request.QueryString["supervisor"] ?? "";
            string keyword = context.Request.QueryString["keyword"] ?? "";


            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            DataTable dataTable = null;
            string fileName = GetFileName(type, exportType);
            context.Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");

            // 根據 type 和 exportType 決定要匯出的資料
            switch (type)
            {
                case "1": // 資格審查/內容審查
                    switch (exportType)
                    {
                        case "reviewing":
                            dataTable = ReviewCheckListHelper.GetType1ReviewingData(year, category, status, orgName, supervisor, keyword);
                            break;
                        case "results":
                            dataTable = ReviewCheckListHelper.GetType1ReviewResultsData(year, category, status, orgName, supervisor, keyword);
                            break;
                        default:
                            throw new ArgumentException("Type1 不支援的匯出類型");
                    }

                    break;

                case "2": // 實質審查/初審
                    switch (exportType)
                    {
                        case "applicationPdf":
                            string mode = context.Request.QueryString["mode"] ?? "prepare";
                            if (mode == "download")
                            {
                                // 第二步：下載已產生的暫存 ZIP
                                string tempFile = context.Request.QueryString["file"] ?? "";
                                ServeTempZipFile(context, tempFile, fileName);
                            }
                            else
                            {
                                // 第一步：產生 ZIP 暫存檔，回傳檔名
                                dataTable = ReviewCheckListHelper.GetType2ApplicationData(year, category, status, orgName, supervisor, keyword, progress, replyStatus);
                                PreparePdfZipFile(context, dataTable);
                            }
                            return;
                        case "results":
                            dataTable = ReviewCheckListHelper.GetType2ReviewResultsData(year, category, status, orgName, supervisor, keyword);
                            break;
                        default:
                            throw new ArgumentException("Type2 不支援的匯出類型");
                    }

                    break;

                case "3": // 技術審查/複審
                    switch (exportType)
                    {
                        case "application":
                            dataTable = ReviewCheckListHelper.GetType3ApplicationData(year, category, status, orgName, supervisor, keyword);
                            break;
                        case "results":
                            dataTable = ReviewCheckListHelper.GetType3ReviewResultsData(year, category, status, orgName, supervisor, keyword);
                            break;
                        default:
                            throw new ArgumentException("Type3 不支援的匯出類型");
                    }

                    break;

                case "4": // 決審核定
                    switch (exportType)
                    {
                        case "list":
                            dataTable = ReviewCheckListHelper.GetType4ListData(year, category, status, orgName, supervisor, keyword);
                            break;
                        default:
                            throw new ArgumentException("Type4 不支援的匯出類型");
                    }

                    break;

                default:
                    throw new ArgumentException("不支援的匯出類型");
            }

            // if (dataTable == null || dataTable.Rows.Count == 0)
            // {
            //     // 如果沒有資料，返回錯誤
            //     context.Response.ContentType = "text/plain";
            //     context.Response.Write("查無資料可匯出");
            //     return;
            // }

            // 使用 ExcelHelper 產生 Excel 檔案
            string tempFilePath = Path.GetTempFileName();
            try
            {
                using (var excel = ExcelHelper.CreateNew(tempFilePath))
                {
                    string sheetName = GetSheetName(type, exportType);
                    excel.RenameWorksheet("工作表1", sheetName);
                    excel.ExportFromDataTable(dataTable, sheetName, true);
                }

                // 讀取檔案並寫入Response
                byte[] excelData = File.ReadAllBytes(tempFilePath);
                context.Response.BinaryWrite(excelData);
            }
            finally
            {
                // 清理臨時檔案
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            context.Response.ContentType = "text/plain";
            context.Response.Write($"匯出時發生錯誤：{ex.Message}");
            System.Diagnostics.Debug.WriteLine($"DownloadReviewChecklistFile 錯誤: {ex}");
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }

    /// <summary>
    /// 根據類型和匯出類型取得檔案名稱
    /// </summary>
    private string GetFileName(string type, string exportType)
    {
        string timestamp = DateTimeHelper.ToMinguoDateTime(DateTime.Now, "MMdd_HHmmss");
        string typeText = GetTypeText(type);
        string exportText = GetExportTypeText(exportType);

        return $"{typeText}_{exportText}_{timestamp}.xlsx";
    }

    /// <summary>
    /// 根據類型和匯出類型取得工作表名稱
    /// </summary>
    private string GetSheetName(string type, string exportType)
    {
        string typeText = GetTypeText(type);
        string exportText = GetExportTypeText(exportType);

        return $"{typeText}_{exportText}";
    }

    /// <summary>
    /// 取得類型文字
    /// </summary>
    private string GetTypeText(string type)
    {
        switch (type)
        {
            case "1": return "資格審查";
            case "2": return "實質審查.初審";
            case "3": return "技術審查.複審";
            case "4": return "決審核定";
            default: return "審查";
        }
    }

    /// <summary>
    /// 取得匯出類型文字
    /// </summary>
    private string GetExportTypeText(string exportType)
    {
        switch (exportType)
        {
            case "reviewing": return "審查中資料";
            case "results": return "審查結果";
            case "application": return "申請資料";
            case "applicationPdf": return "_審查案件資料";
            case "list": return "列表資料";
            default: return "資料";
        }
    }

    /// <summary>
    /// 第一步：產生 ZIP 暫存檔，回傳檔名給前端（AJAX）
    /// </summary>
    private void PreparePdfZipFile(HttpContext context, DataTable dataTable)
    {
        string tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
        try
        {
            using (var tempFileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
            using (var zipStream = new ZipOutputStream(tempFileStream))
            {
                zipStream.SetLevel(0); // 僅打包不壓縮（PDF 已是壓縮格式）

                int fileCount = 0;
                byte[] buffer = new byte[67108864]; // 64MB 讀取緩衝區

                foreach (DataRow row in dataTable.Rows)
                {
                    string projectId = row["ProjectID"]?.ToString() ?? "";
                    string pdfPath = row["PdfPath"]?.ToString() ?? "";
                    string category = row["Category"]?.ToString() ?? "";
                    string projectName = row["ProjectName"]?.ToString() ?? "";
                    string categoryName = category == "SCI" ? "科專" : "文化";
                    if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(pdfPath))
                    {
                        if (File.Exists(pdfPath))
                        {
                            try
                            {
                                string zipEntryName = $"{projectId}_{categoryName}_{projectName}_送審版.pdf";
                                zipEntryName = SanitizeFileName(zipEntryName);

                                var fileInfo = new FileInfo(pdfPath);
                                var zipEntry = new ZipEntry(ZipEntry.CleanName(zipEntryName));
                                zipEntry.DateTime = fileInfo.LastWriteTime;
                                zipEntry.Size = fileInfo.Length;
                                zipStream.PutNextEntry(zipEntry);

                                using (var fileStream = File.OpenRead(pdfPath))
                                {
                                    int bytesRead;
                                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        zipStream.Write(buffer, 0, bytesRead);
                                    }
                                }

                                fileCount++;
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"處理 PDF 檔案 {pdfPath} 時發生錯誤：{ex.Message}");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"PDF 檔案不存在：{pdfPath}");
                        }
                    }
                }

                if (fileCount == 0)
                {
                    string message = "查無符合條件的 PDF 檔案。\r\n可能原因：\r\n1. 篩選條件沒有找到相符的專案\r\n2. 專案的 PDF 檔案尚未上傳或路徑不正確\r\n建立時間：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    byte[] messageData = System.Text.Encoding.UTF8.GetBytes(message);

                    var noFileEntry = new ZipEntry(ZipEntry.CleanName("說明.txt"));
                    noFileEntry.DateTime = DateTime.Now;
                    noFileEntry.Size = messageData.Length;
                    zipStream.PutNextEntry(noFileEntry);
                    zipStream.Write(messageData, 0, messageData.Length);
                }

                zipStream.Finish();
            }

            // 回傳暫存檔名給前端
            context.Response.ContentType = "application/json";
            string tempFileName = Path.GetFileName(tempZipPath);
            context.Response.Write($"{{\"success\":true,\"file\":\"{tempFileName}\"}}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PreparePdfZipFile 錯誤: {ex}");
            context.Response.ContentType = "application/json";
            context.Response.Write($"{{\"success\":false,\"message\":\"產生ZIP時發生錯誤\"}}");
        }
    }

    /// <summary>
    /// 第二步：下載已產生的暫存 ZIP 檔案
    /// </summary>
    private void ServeTempZipFile(HttpContext context, string tempFileName, string downloadFileName)
    {
        // 安全檢查：只允許檔名，不允許路徑
        if (string.IsNullOrEmpty(tempFileName) || tempFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            context.Response.StatusCode = 400;
            context.Response.Write("無效的檔案參數");
            return;
        }

        string tempZipPath = Path.Combine(Path.GetTempPath(), tempFileName);
        if (!File.Exists(tempZipPath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write("暫存檔案不存在或已過期");
            return;
        }

        try
        {
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ContentType = "application/zip";
            context.Response.AddHeader("Content-Disposition", $"attachment; filename={downloadFileName.Replace(".xlsx", ".zip")}");
            context.Response.TransmitFile(tempZipPath);
            context.Response.Flush();
            context.ApplicationInstance.CompleteRequest();
        }
        finally
        {
            // 延遲刪除暫存檔（給 TransmitFile 時間完成傳輸）
            System.Threading.Tasks.Task.Delay(10000).ContinueWith(_ =>
            {
                try { if (File.Exists(tempZipPath)) File.Delete(tempZipPath); } catch { }
            });
        }
    }
    
    /// <summary>
    /// 清理檔案名稱中的無效字元
    /// </summary>
    private string SanitizeFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName;
    }

}
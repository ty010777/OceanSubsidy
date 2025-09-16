<%@ WebHandler Language="C#" Class="DownloadReviewChecklistFile" %>

using System;
using System.Data;
using System.IO;
using System.Web;
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
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // 取得查詢參數
            string type = context.Request.QueryString["type"] ?? "1";
            string exportType = context.Request.QueryString["exportType"] ?? "reviewing"; // reviewing, results, application, list
            string year = context.Request.QueryString["year"] ?? "";
            string category = context.Request.QueryString["category"] ?? "";
            string status = context.Request.QueryString["status"] ?? "";
            string orgName = context.Request.QueryString["orgName"] ?? "";
            string supervisor = context.Request.QueryString["supervisor"] ?? "";
            string keyword = context.Request.QueryString["keyword"] ?? "";

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

                case "2": // 領域審查/初審
                    switch (exportType)
                    {
                        case "application":
                            dataTable = ReviewCheckListHelper.GetType2ApplicationData(year, category, status, orgName, supervisor, keyword);
                            break;
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
        string timestamp = DateTimeHelper.ToMinguoDateTime(DateTime.Now,"MMdd_HHmmss");
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
            case "2": return "領域審查";
            case "3": return "技術審查";
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
            case "list": return "列表資料";
            default: return "資料";
        }
    }
}
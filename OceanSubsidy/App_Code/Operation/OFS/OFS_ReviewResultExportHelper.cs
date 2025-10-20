using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using GS.Data;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using GS.Data.Sql;

/// <summary>
/// 審查結果匯出功能 Helper
/// 按照審查組別分工作表匯出 XLSX 格式
/// </summary>
public class OFS_ReviewResultExportHelper
{
    /// <summary>
    /// 匯出所有審查結果到 XLSX 檔案
    /// </summary>
    /// <param name="requests">匯出請求清單</param>
    /// <returns>匯出的檔案位元組陣列</returns>
    public static byte[] ExportAllReviewResultsToXlsx(List<ReviewExportRequest> requests)
    {
        if (requests == null || !requests.Any())
        {
            throw new ArgumentException("匯出請求清單不能為空");
        }

        var worksheetData = new List<(string SheetName, DataTable Data)>();

        // 合併所有請求的資料
        foreach (var request in requests)
        {
            if (request.GrantType == "SCI")
            {
                foreach (var field in request.Fields)
                {
                    string sheetName = GetZgsCodeDescname("SCIField", field);
                    if (string.IsNullOrEmpty(sheetName))
                    {
                        sheetName = field; // 如果找不到描述名稱，使用原始 field 值
                    }

                    DataTable reviewerData = SCI_BuildReviewResultQuery(field, request.ReviewStage);

                    if (reviewerData != null && reviewerData.Rows.Count > 0)
                    {
                        worksheetData.Add((sheetName, reviewerData));
                    }
                }
            }
            else
            {
                foreach (var field in request.Fields)
                {
                    string sheetName = GetZgsCodeDescname("CULField", field);
                    if (string.IsNullOrEmpty(sheetName))
                    {
                        sheetName = field; // 如果找不到描述名稱，使用原始 field 值
                    }

                    DataTable reviewerData = CUL_BuildReviewResultQuery(field, request.ReviewStage);

                    if (reviewerData != null && reviewerData.Rows.Count > 0)
                    {
                        worksheetData.Add((sheetName, reviewerData));
                    }
                }
                
            }
        }

        if (!worksheetData.Any())
        {
            throw new Exception("沒有找到符合條件的審查結果資料");
        }

        // 建立臨時檔案路徑
        string tempFilePath = Path.GetTempFileName();
        tempFilePath = Path.ChangeExtension(tempFilePath, ".xlsx");

        try
        {
            // 使用臨時檔案建立 Excel
            using (var excelHelper = ExcelHelper.CreateNew(tempFilePath))
            {
                // 刪除預設的工作表
                var defaultSheets = excelHelper.GetWorksheetNames();
                foreach (var defaultSheet in defaultSheets)
                {
                    excelHelper.DeleteWorksheet(defaultSheet);
                }

                // 將每個 DataTable 匯出到對應的工作表
                foreach (var (sheetName, data) in worksheetData)
                {
                    try
                    {
                        excelHelper.ExportFromDataTable(data, sheetName, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"匯出工作表 '{sheetName}' 時發生錯誤: {ex.Message}", ex);
                    }
                }

                excelHelper.Save();
            }

            // 讀取檔案內容並回傳位元組陣列
            return File.ReadAllBytes(tempFilePath);
        }
        finally
        {
            // 清理臨時檔案
            if (File.Exists(tempFilePath))
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                {
                    // 忽略刪除錯誤
                }
            }
        }
    }


    /// <summary>
    /// 建構審查結果查詢 SQL
    /// </summary>
    private static DataTable SCI_BuildReviewResultQuery(string field, string reviewStage)
    {
        string Statuses = reviewStage == "2" ? "領域審查" : "技術審查";
        DbHelper db = new DbHelper();
        string sql = @"
           -- 建立臨時表存 Pivoted 資料
            IF OBJECT_ID('tempdb..#Pivoted') IS NOT NULL DROP TABLE #Pivoted;

            SELECT 
                RR.ProjectID,
                AM.ProjectNameTw,
                RR.ReviewerName,
                RR.TotalScore
            INTO #Pivoted
            FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords] RR
            INNER JOIN OFS_SCI_Application_Main AM ON RR.ProjectID = AM.ProjectID
            INNER JOIN OFS_SCI_Project_Main PM ON PM.ProjectID = AM.ProjectID
            WHERE RR.ReviewStage = @ReviewStage 
              AND RR.IsSubmit = 1
              AND AM.Field = @Field
              AND RR.TotalScore IS NOT NULL
              AND PM.Statuses = @Statuses;

            -- 取得有分數的 ReviewerName
            DECLARE @cols NVARCHAR(MAX);
            DECLARE @countCols NVARCHAR(MAX);
            DECLARE @sumCols NVARCHAR(MAX);
            DECLARE @query NVARCHAR(MAX);

            SELECT 
                @cols = STRING_AGG(QUOTENAME(ReviewerName), ','),
                @countCols = STRING_AGG('CASE WHEN ' + QUOTENAME(ReviewerName) + ' IS NOT NULL THEN 1 ELSE 0 END', ' + '),
                @sumCols = STRING_AGG('ISNULL(' + QUOTENAME(ReviewerName) + ',0)', ' + ')
            FROM (SELECT DISTINCT ReviewerName FROM #Pivoted) AS ReviewerList;

            -- 動態 PIVOT
            SET @query = N'
            WITH PivotTable AS (
                SELECT ProjectID, ProjectNameTw, ' + @cols + '
                FROM #Pivoted
                PIVOT
                (
                    MAX(TotalScore)
                    FOR ReviewerName IN (' + @cols + ')
                ) AS P
            )
            SELECT 
                PT.ProjectID AS ''計畫編號'',
                PT.ProjectNameTw AS ''計畫名稱'',
                
                (' + @sumCols + ') AS ''總分'',
                CAST((' + @sumCols + ') * 1.0 / NULLIF((' + @countCols + '),0) AS DECIMAL(10,2)) AS ''平均分數'',
                ' + @cols + '
            FROM PivotTable PT
            ORDER BY PT.ProjectID;
            ';

            EXEC sp_executesql @query;

            -- 刪除臨時表
            DROP TABLE #Pivoted;

        ";

        db.CommandText = sql;
        db.Parameters.Clear();
        db.Parameters.Add("@Field", field);
        db.Parameters.Add("@Statuses", Statuses);
        db.Parameters.Add("@ReviewStage", reviewStage);
        DataTable dt = db.GetTable();
        int a = dt.Rows.Count;
        db.Dispose();

        return dt;
    }
  /// <summary>
    /// 建構審查結果查詢 SQL
    /// </summary>
    private static DataTable CUL_BuildReviewResultQuery(string field, string reviewStage)
    {
        DbHelper db = new DbHelper();
        string sql = @"
            -- 建立臨時表存 Pivoted 資料
            IF OBJECT_ID('tempdb..#Pivoted') IS NOT NULL DROP TABLE #Pivoted;

            SELECT 
                RR.ProjectID,
                PM.ProjectName as ProjectNameTw,
                RR.ReviewerName,
                RR.TotalScore
            INTO #Pivoted
            FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords] RR
            INNER JOIN [OFS_CUL_Project] PM ON RR.ProjectID = PM.ProjectID
            WHERE RR.ReviewStage = @ReviewStage 
              AND RR.IsSubmit = 1
              AND PM.Field = @Field
              AND RR.TotalScore IS NOT NULL
              AND PM.ProgressStatus = @ReviewStage;

            -- 取得有分數的 ReviewerName
            DECLARE @cols NVARCHAR(MAX);
            DECLARE @countCols NVARCHAR(MAX);
            DECLARE @sumCols NVARCHAR(MAX);
            DECLARE @query NVARCHAR(MAX);

            SELECT 
                @cols = STRING_AGG(QUOTENAME(ReviewerName), ','),
                @countCols = STRING_AGG('CASE WHEN ' + QUOTENAME(ReviewerName) + ' IS NOT NULL THEN 1 ELSE 0 END', ' + '),
                @sumCols = STRING_AGG('ISNULL(' + QUOTENAME(ReviewerName) + ',0)', ' + ')
            FROM (SELECT DISTINCT ReviewerName FROM #Pivoted) AS ReviewerList;

            -- 動態 PIVOT
            SET @query = N'
            WITH PivotTable AS (
                SELECT ProjectID, ProjectNameTw, ' + @cols + '
                FROM #Pivoted
                PIVOT
                (
                    MAX(TotalScore)
                    FOR ReviewerName IN (' + @cols + ')
                ) AS P
            )
            SELECT 
                PT.ProjectID AS ''計畫編號'',
                PT.ProjectNameTw AS ''計畫名稱'',
                
                (' + @sumCols + ') AS ''總分'',
                CAST((' + @sumCols + ') * 1.0 / NULLIF((' + @countCols + '),0) AS DECIMAL(10,2)) AS ''平均分數'',
                ' + @cols + '
            FROM PivotTable PT
            ORDER BY PT.ProjectID;
            ';

            EXEC sp_executesql @query;

            -- 刪除臨時表
            DROP TABLE #Pivoted;


        ";

        db.CommandText = sql;
        db.Parameters.Clear();
        db.Parameters.Add("@Field", field);
        db.Parameters.Add("@ReviewStage", reviewStage);
        DataTable dt = db.GetTable();
        int a = dt.Rows.Count;
        db.Dispose();

        return dt;
    }
    /// <summary>
    /// 查詢 Sys_ZgsCode 資料
    /// </summary>
    /// <param name="codeGroup">代碼群組</param>
    /// <param name="code">代碼</param>
    /// <returns>描述名稱</returns>
    public static string GetZgsCodeDescname(string codeGroup, string code)
    {
        DbHelper db = new DbHelper();
        string sql = @"
            SELECT Descname
            FROM [OCA_OceanSubsidy].[dbo].[Sys_ZgsCode]
            WHERE CodeGroup = @CodeGroup AND Code = @Code
        ";

        db.CommandText = sql;
        db.Parameters.Clear();
        db.Parameters.Add("@CodeGroup", codeGroup);
        db.Parameters.Add("@Code", code);

        DataTable dt = db.GetTable();
        string result = dt.Rows.Count > 0 ? dt.Rows[0]["Descname"].ToString() : string.Empty;

        db.Dispose();
        return result;
    }
}
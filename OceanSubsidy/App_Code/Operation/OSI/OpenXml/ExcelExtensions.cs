using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace GS.OCA_OceanSubsidy.Operation.OSI.OpenXml
{
    /// <summary>
    /// Excel 操作的擴充功能類別
    /// </summary>
    public static class ExcelExtensions
    {
        #region 資料庫相關擴充

        /// <summary>
        /// 從 SQL 查詢結果直接匯出到 Excel
        /// </summary>
        /// <param name="connectionString">資料庫連線字串</param>
        /// <param name="sql">SQL 查詢語句</param>
        /// <param name="filePath">輸出檔案路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="includeHeader">是否包含標題</param>
        public static void ExportFromSql(string connectionString, string sql, string filePath, string sheetName = "資料", bool includeHeader = true)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var adapter = new SqlDataAdapter(sql, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    using (var excel = ExcelHelper.CreateNew(filePath))
                    {
                        excel.ExportFromDataTable(dataTable, sheetName, includeHeader);
                    }
                }
            }
        }

        /// <summary>
        /// 從多個 SQL 查詢結果匯出到多個工作表
        /// </summary>
        /// <param name="connectionString">資料庫連線字串</param>
        /// <param name="queries">SQL 查詢字典 (工作表名稱 => SQL)</param>
        /// <param name="filePath">輸出檔案路徑</param>
        /// <param name="includeHeaders">是否包含標題</param>
        public static void ExportMultipleSheetsFromSql(string connectionString, Dictionary<string, string> queries, string filePath, bool includeHeaders = true)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var excel = ExcelHelper.CreateNew(filePath))
                {
                    bool isFirstSheet = true;
                    foreach (var query in queries)
                    {
                        using (var adapter = new SqlDataAdapter(query.Value, connection))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            if (isFirstSheet)
                            {
                                excel.RenameWorksheet("工作表1", query.Key);
                                isFirstSheet = false;
                            }
                            else
                            {
                                excel.AddWorksheet(query.Key);
                            }

                            excel.ExportFromDataTable(dataTable, query.Key, includeHeaders);
                        }
                    }
                }
            }
        }

        #endregion

        #region 快速操作方法

        /// <summary>
        /// 快速讀取整個工作表的資料
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="sheetName">工作表名稱（為空時讀取第一個工作表）</param>
        /// <param name="hasHeader">是否有標題行</param>
        /// <returns>資料列表</returns>
        public static List<Dictionary<string, string>> ReadWorksheetAsDictionary(string filePath, string sheetName = null, bool hasHeader = true)
        {
            using (var excel = new ExcelHelper(filePath, false))
            {
                var sheets = excel.GetWorksheetNames();
                var targetSheet = string.IsNullOrEmpty(sheetName) ? sheets.FirstOrDefault() : sheetName;

                if (string.IsNullOrEmpty(targetSheet))
                    throw new InvalidOperationException("找不到工作表");

                // 先讀取一小部分來確定範圍
                var sampleData = excel.ReadRange(targetSheet, 1, 1, 10, 10);

                // 找出實際的資料範圍
                var maxRow = 1;
                var maxCol = 1;

                for (int i = 0; i < sampleData.Count; i++)
                {
                    if (sampleData[i].Any(cell => !string.IsNullOrEmpty(cell)))
                        maxRow = Math.Max(maxRow, i + 1);
                }

                for (int j = 0; j < sampleData[0].Count; j++)
                {
                    if (sampleData.Any(row => j < row.Count && !string.IsNullOrEmpty(row[j])))
                        maxCol = Math.Max(maxCol, j + 1);
                }

                // 擴大範圍重新讀取
                var data = excel.ReadRange(targetSheet, 1, 1, Math.Max(maxRow, 100), maxCol);

                var result = new List<Dictionary<string, string>>();
                var headers = hasHeader ? data.FirstOrDefault() :
                    Enumerable.Range(1, maxCol).Select(i => $"Column{i}").ToList();

                var startRow = hasHeader ? 1 : 0;
                for (int i = startRow; i < data.Count; i++)
                {
                    var row = data[i];
                    if (row.All(cell => string.IsNullOrEmpty(cell)))
                        break; // 遇到空行就停止

                    var rowDict = new Dictionary<string, string>();
                    for (int j = 0; j < Math.Min(headers.Count, row.Count); j++)
                    {
                        rowDict[headers[j]] = row[j] ?? string.Empty;
                    }
                    result.Add(rowDict);
                }

                return result;
            }
        }

        /// <summary>
        /// 快速寫入字典資料到 Excel
        /// </summary>
        /// <param name="data">資料字典列表</param>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="includeHeader">是否包含標題</param>
        public static void WriteDictionaryToExcel(List<Dictionary<string, object>> data, string filePath, string sheetName = "資料", bool includeHeader = true)
        {
            if (!data.Any()) return;

            using (var excel = ExcelHelper.CreateNew(filePath))
            {
                excel.RenameWorksheet("工作表1", sheetName);

                var headers = data.First().Keys.ToList();
                var exportData = new List<List<object>>();

                if (includeHeader)
                {
                    exportData.Add(headers.Cast<object>().ToList());
                }

                foreach (var row in data)
                {
                    var rowData = headers.Select(header => row.ContainsKey(header) ? row[header] : null).ToList();
                    exportData.Add(rowData);
                }

                excel.WriteRange(sheetName, exportData, 1, 1);
            }
        }

        /// <summary>
        /// 比較兩個 Excel 檔案的差異
        /// </summary>
        /// <param name="file1Path">第一個檔案路徑</param>
        /// <param name="file2Path">第二個檔案路徑</param>
        /// <param name="sheetName">要比較的工作表名稱</param>
        /// <returns>差異報告</returns>
        public static ExcelComparisonResult CompareExcelFiles(string file1Path, string file2Path, string sheetName = null)
        {
            var result = new ExcelComparisonResult();

            using (var excel1 = new ExcelHelper(file1Path, false))
            using (var excel2 = new ExcelHelper(file2Path, false))
            {
                var sheets1 = excel1.GetWorksheetNames();
                var sheets2 = excel2.GetWorksheetNames();

                var targetSheet = string.IsNullOrEmpty(sheetName) ?
                    (sheets1.FirstOrDefault() ?? sheets2.FirstOrDefault()) : sheetName;

                if (string.IsNullOrEmpty(targetSheet))
                {
                    result.HasDifferences = true;
                    result.Differences.Add("兩個檔案都沒有工作表");
                    return result;
                }

                if (!sheets1.Contains(targetSheet))
                {
                    result.HasDifferences = true;
                    result.Differences.Add($"檔案1 缺少工作表: {targetSheet}");
                }

                if (!sheets2.Contains(targetSheet))
                {
                    result.HasDifferences = true;
                    result.Differences.Add($"檔案2 缺少工作表: {targetSheet}");
                }

                if (result.HasDifferences) return result;

                // 比較資料內容
                var data1 = excel1.ReadRange(targetSheet, 1, 1, 100, 50); // 假設最大 100行50列
                var data2 = excel2.ReadRange(targetSheet, 1, 1, 100, 50);

                var maxRows = Math.Max(data1.Count, data2.Count);
                var maxCols = Math.Max(
                    data1.Any() ? data1.Max(row => row.Count) : 0,
                    data2.Any() ? data2.Max(row => row.Count) : 0
                );

                for (int i = 0; i < maxRows; i++)
                {
                    for (int j = 0; j < maxCols; j++)
                    {
                        var value1 = i < data1.Count && j < data1[i].Count ? data1[i][j] : string.Empty;
                        var value2 = i < data2.Count && j < data2[i].Count ? data2[i][j] : string.Empty;

                        if (value1 != value2)
                        {
                            result.HasDifferences = true;
                            result.Differences.Add($"儲存格 {GetCellReference(i + 1, j + 1)}: 檔案1='{value1}', 檔案2='{value2}'");
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 合併多個 Excel 檔案到一個檔案的不同工作表
        /// </summary>
        /// <param name="inputFiles">輸入檔案路徑字典 (工作表名稱 => 檔案路徑)</param>
        /// <param name="outputPath">輸出檔案路徑</param>
        public static void MergeExcelFiles(Dictionary<string, string> inputFiles, string outputPath)
        {
            using (var outputExcel = ExcelHelper.CreateNew(outputPath))
            {
                bool isFirstSheet = true;

                foreach (var file in inputFiles)
                {
                    using (var inputExcel = new ExcelHelper(file.Value, false))
                    {
                        var inputSheets = inputExcel.GetWorksheetNames();
                        var sourceSheet = inputSheets.FirstOrDefault();

                        if (string.IsNullOrEmpty(sourceSheet)) continue;

                        var data = inputExcel.ReadRange(sourceSheet, 1, 1, 1000, 100); // 讀取大範圍

                        // 過濾空行
                        var nonEmptyData = data.Where(row => row.Any(cell => !string.IsNullOrEmpty(cell))).ToList();

                        if (!nonEmptyData.Any()) continue;

                        var exportData = nonEmptyData.Select(row => row.Cast<object>().ToList()).ToList();

                        if (isFirstSheet)
                        {
                            outputExcel.RenameWorksheet("工作表1", file.Key);
                            isFirstSheet = false;
                        }
                        else
                        {
                            outputExcel.AddWorksheet(file.Key);
                        }

                        outputExcel.WriteRange(file.Key, exportData, 1, 1);
                    }
                }
            }
        }

        #endregion

        #region 輔助方法

        private static string GetCellReference(int row, int column)
        {
            string columnName = "";
            while (column > 0)
            {
                column--;
                columnName = (char)('A' + (column % 26)) + columnName;
                column /= 26;
            }
            return columnName + row;
        }

        #endregion
    }

    /// <summary>
    /// Excel 比較結果類別
    /// </summary>
    public class ExcelComparisonResult
    {
        public bool HasDifferences { get; set; } = false;
        public List<string> Differences { get; set; } = new List<string>();
    }

    /// <summary>
    /// Excel 操作的靜態輔助方法
    /// </summary>
    public static class ExcelUtilities
    {
        /// <summary>
        /// 驗證 Excel 檔案格式
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>是否為有效的 Excel 檔案</returns>
        public static bool IsValidExcelFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            var extension = Path.GetExtension(filePath).ToLower();
            if (extension != ".xlsx" && extension != ".xlsm") return false;

            try
            {
                using (var excel = new ExcelHelper(filePath, false))
                {
                    var sheets = excel.GetWorksheetNames();
                    return sheets.Any();
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 取得 Excel 檔案資訊
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>檔案資訊</returns>
        public static ExcelFileInfo GetExcelFileInfo(string filePath)
        {
            var info = new ExcelFileInfo { FilePath = filePath };

            if (!File.Exists(filePath))
            {
                info.IsValid = false;
                info.ErrorMessage = "檔案不存在";
                return info;
            }

            try
            {
                var fileInfo = new FileInfo(filePath);
                info.FileSize = fileInfo.Length;
                info.LastModified = fileInfo.LastWriteTime;

                using (var excel = new ExcelHelper(filePath, false))
                {
                    info.WorksheetNames = excel.GetWorksheetNames();
                    info.WorksheetCount = info.WorksheetNames.Count;
                    info.IsValid = true;
                }
            }
            catch (Exception ex)
            {
                info.IsValid = false;
                info.ErrorMessage = ex.Message;
            }

            return info;
        }

        /// <summary>
        /// 建立範本檔案
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="headers">標題列</param>
        /// <param name="sheetName">工作表名稱</param>
        public static void CreateTemplate(string filePath, List<string> headers, string sheetName = "模板")
        {
            using (var excel = ExcelHelper.CreateNew(filePath))
            {
                excel.RenameWorksheet("工作表1", sheetName);

                var headerData = new List<List<object>>
                {
                    headers.Cast<object>().ToList()
                };

                excel.WriteRange(sheetName, headerData, 1, 1);

                // 合併標題列（可選）
                if (headers.Count > 1)
                {
                    // 這裡可以加入一些格式設定的邏輯
                }
            }
        }
    }

    /// <summary>
    /// Excel 檔案資訊類別
    /// </summary>
    public class ExcelFileInfo
    {
        public string FilePath { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public List<string> WorksheetNames { get; set; } = new List<string>();
        public int WorksheetCount { get; set; }
    }
}
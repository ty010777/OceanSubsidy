using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

public class NPOIHelper
{
    public static MemoryStream CreateExcel(string name, List<string> headers, List<List<string>> rows)
    {
        IWorkbook workBook = new XSSFWorkbook();

        ISheet sheet = workBook.CreateSheet(name);

        int rowIndex = 0;

        if (headers.Count > 0)
        {
            IRow headerRow = sheet.CreateRow(rowIndex);

            for (int i = 0; i < headers.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(headers[i]);
            }

            rowIndex++;
        }

        foreach (var row in rows)
        {
            IRow dataRow = sheet.CreateRow(rowIndex);

            for (int i = 0; i < row.Count; i++)
            {
                dataRow.CreateCell(i).SetCellValue(row[i]);
            }

            rowIndex++;
        }

        using (var ms = new MemoryStream())
        {
            workBook.Write(ms);

            return ms;
        }
    }

    /// <summary>
    /// 讀取 Excel 檔案中指定工作表的特定儲存格值
    /// </summary>
    /// <param name="filePath">Excel 檔案實體路徑</param>
    /// <param name="sheetName">工作表名稱</param>
    /// <param name="columnLetter">欄位字母 (例如: "K", "C")</param>
    /// <param name="rowNumber">列數字 (例如: 14, 23，注意：1-based)</param>
    /// <returns>儲存格的數值，如果無法解析則返回 null</returns>
    public static decimal? ReadCellValue(string filePath, string sheetName, string columnLetter, int rowNumber)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            throw new FileNotFoundException("Excel 檔案不存在", filePath);
        }

        if (string.IsNullOrEmpty(sheetName))
        {
            throw new ArgumentException("工作表名稱不可為空", nameof(sheetName));
        }

        if (string.IsNullOrEmpty(columnLetter))
        {
            throw new ArgumentException("欄位字母不可為空", nameof(columnLetter));
        }

        try
        {
            IWorkbook workbook = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // 根據副檔名判斷使用哪種 Workbook
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (extension == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    // 不支援的格式，返回 null
                    return null;
                }

                // 取得指定的工作表
                ISheet sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    // 找不到工作表，返回 null（使用者可能自行修改了 Sheet 名稱）
                    return null;
                }

                // 將欄位字母轉換為索引 (A=0, B=1, ..., K=10)
                int columnIndex = ConvertColumnLetterToIndex(columnLetter);
                int rowIndex = rowNumber - 1; // 轉為 0-based index

                // 取得儲存格
                IRow row = sheet.GetRow(rowIndex);
                if (row == null)
                {
                    return null;
                }

                ICell cell = row.GetCell(columnIndex);
                if (cell == null)
                {
                    return null;
                }

                // 根據儲存格類型取值
                if (cell.CellType == CellType.NUMERIC)
                {
                    return (decimal)cell.NumericCellValue;
                }
                else if (cell.CellType == CellType.STRING)
                {
                    // 嘗試將字串轉換為數值
                    string cellValue = cell.StringCellValue.Trim();
                    if (decimal.TryParse(cellValue, out decimal result))
                    {
                        return result;
                    }
                    return null;
                }
                else if (cell.CellType == CellType.FORMULA)
                {
                    // 如果是公式，嘗試取得計算後的值
                    try
                    {
                        return (decimal)cell.NumericCellValue;
                    }
                    catch
                    {
                        return null;
                    }
                }

                return null;
            }
        }
        catch (Exception ex)
        {
            // 發生任何錯誤都返回 null，不中斷流程
            System.Diagnostics.Debug.WriteLine($"讀取 Excel 儲存格時發生錯誤: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 將欄位字母轉換為索引 (A=0, B=1, ..., Z=25, AA=26, ...)
    /// </summary>
    /// <param name="columnLetter">欄位字母</param>
    /// <returns>欄位索引 (0-based)</returns>
    private static int ConvertColumnLetterToIndex(string columnLetter)
    {
        columnLetter = columnLetter.ToUpper().Trim();
        int columnIndex = 0;

        for (int i = 0; i < columnLetter.Length; i++)
        {
            columnIndex = columnIndex * 26 + (columnLetter[i] - 'A' + 1);
        }

        return columnIndex - 1; // 轉為 0-based index
    }
}

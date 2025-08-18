using System;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders
{
    /// <summary>
    /// Excel 檔案讀取器（使用 EPPlus）
    /// </summary>
    public class ExcelReader : ISpreadsheetReader
    {
        static ExcelReader()
        {
            ExcelPackage.License.SetNonCommercialOrganization("GIS.FCU");
        }
        
        public DataTable ReadToDataTable(Stream stream)
        {
            var dt = new DataTable();
            
            try
            {
                using (var pkg = new ExcelPackage(stream))
                {
                    var ws = pkg.Workbook.Worksheets.First();
                    
                    if (ws.Dimension == null)
                    {
                        throw new InvalidOperationException("Excel 檔案沒有資料");
                    }
                    
                    // 建立 DataTable 欄位（使用 Column1, Column2... 的格式）
                    int colCount = ws.Dimension.End.Column;
                    for (int col = 1; col <= colCount; col++)
                    {
                        dt.Columns.Add($"Column{col}", typeof(string));
                    }
                    
                    // 讀取資料（包含標題列）
                    for (int row = 1; row <= ws.Dimension.End.Row; row++)
                    {
                        var values = new object[colCount];
                        for (int col = 1; col <= colCount; col++)
                        {
                            values[col - 1] = ws.Cells[row, col].Text.Trim();
                        }
                        dt.Rows.Add(values);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"讀取 Excel 檔案失敗: {ex.Message}", ex);
            }
            
            return dt;
        }
    }
}
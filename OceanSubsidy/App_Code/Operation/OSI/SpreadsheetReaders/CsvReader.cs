using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders
{
    /// <summary>
    /// CSV 檔案讀取器
    /// </summary>
    public class CsvReader : ISpreadsheetReader
    {
        public DataTable ReadToDataTable(Stream stream)
        {
            var dt = new DataTable();
            
            try
            {
                // 使用 UTF-8 編碼，並支援 BOM
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    int rowIndex = 0;
                    int columnCount = 0;
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = ParseCsvLine(line);
                        
                        if (rowIndex == 0)
                        {
                            // 第一行決定欄位數量
                            columnCount = values.Length;
                            
                            // 建立 DataTable 欄位
                            for (int i = 0; i < columnCount; i++)
                            {
                                dt.Columns.Add($"Column{i + 1}", typeof(string));
                            }
                        }
                        
                        // 確保值的數量與欄位數量一致
                        var rowValues = new string[columnCount];
                        for (int i = 0; i < columnCount && i < values.Length; i++)
                        {
                            rowValues[i] = values[i];
                        }
                        
                        // 不足的欄位補空字串
                        for (int i = values.Length; i < columnCount; i++)
                        {
                            rowValues[i] = string.Empty;
                        }
                        
                        dt.Rows.Add(rowValues);
                        rowIndex++;
                    }
                }
                
                if (dt.Rows.Count == 0)
                {
                    throw new InvalidOperationException("CSV 檔案沒有資料");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"讀取 CSV 檔案失敗: {ex.Message}", ex);
            }
            
            return dt;
        }
        
        /// <summary>
        /// 解析 CSV 行，處理引號和逗號
        /// </summary>
        private string[] ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = new StringBuilder();
            bool inQuotes = false;
            bool wasInQuotes = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (c == '"')
                {
                    if (!inQuotes)
                    {
                        inQuotes = true;
                        wasInQuotes = true;
                    }
                    else if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // 兩個連續的引號代表一個引號字元
                        currentValue.Append('"');
                        i++; // 跳過下一個引號
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // 遇到逗號且不在引號內，表示欄位結束
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                    wasInQuotes = false;
                }
                else
                {
                    currentValue.Append(c);
                }
            }
            
            // 加入最後一個值
            values.Add(currentValue.ToString());
            
            return values.ToArray();
        }
    }
}
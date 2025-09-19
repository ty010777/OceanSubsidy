using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace GS.OCA_OceanSubsidy.Operation.OSI.OpenXml
{
    /// <summary>
    /// 處理 OpenXML Excel 文件的輔助類別
    /// </summary>
    public class ExcelHelper : IDisposable
    {
        private SpreadsheetDocument Document;
        private WorkbookPart WorkbookPart;
        private bool IsDisposed = false;

        #region 建構函數與初始化

        /// <summary>
        /// 開啟現有的 Excel 檔案
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="isEditable">是否可編輯</param>
        public ExcelHelper(string filePath, bool isEditable = true)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Excel 檔案不存在", filePath);
            }

            Document = SpreadsheetDocument.Open(filePath, isEditable);
            WorkbookPart = Document.WorkbookPart;
        }

        /// <summary>
        /// 從 Stream 開啟 Excel 檔案
        /// </summary>
        /// <param name="stream">檔案串流</param>
        /// <param name="isEditable">是否可編輯</param>
        public ExcelHelper(Stream stream, bool isEditable = true)
        {
            Document = SpreadsheetDocument.Open(stream, isEditable);
            WorkbookPart = Document.WorkbookPart;
        }

        /// <summary>
        /// 建立新的 Excel 檔案
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        public static ExcelHelper CreateNew(string filePath)
        {
            var document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

            // 建立 WorkbookPart
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // 建立 Sheets 集合
            var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

            // 建立第一個工作表
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            var sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "工作表1"
            };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();
            document.Close();

            return new ExcelHelper(filePath);
        }

        /// <summary>
        /// 從 Stream 建立新的 Excel 檔案
        /// </summary>
        /// <param name="stream">檔案串流</param>
        public static ExcelHelper CreateNew(Stream stream)
        {
            var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            var sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "工作表1"
            };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();

            return new ExcelHelper(stream);
        }

        #endregion

        #region 工作表操作

        /// <summary>
        /// 取得所有工作表名稱
        /// </summary>
        public List<string> GetWorksheetNames()
        {
            return WorkbookPart.Workbook.Sheets.Cast<Sheet>()
                .Select(sheet => sheet.Name.Value)
                .ToList();
        }

        /// <summary>
        /// 新增工作表
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        public void AddWorksheet(string sheetName)
        {
            var worksheetPart = WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            var sheets = WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var relationshipId = WorkbookPart.GetIdOfPart(worksheetPart);

            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            var sheet = new Sheet()
            {
                Id = relationshipId,
                SheetId = sheetId,
                Name = sheetName
            };

            sheets.Append(sheet);
        }

        /// <summary>
        /// 刪除工作表
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        public void DeleteWorksheet(string sheetName)
        {
            var sheet = WorkbookPart.Workbook.Sheets.Cast<Sheet>()
                .FirstOrDefault(s => s.Name == sheetName);

            if (sheet != null)
            {
                var worksheetPart = (WorksheetPart)WorkbookPart.GetPartById(sheet.Id);
                WorkbookPart.DeletePart(worksheetPart);
                sheet.Remove();
            }
        }

        /// <summary>
        /// 重新命名工作表
        /// </summary>
        /// <param name="oldName">原名稱</param>
        /// <param name="newName">新名稱</param>
        public void RenameWorksheet(string oldName, string newName)
        {
            var sheet = WorkbookPart.Workbook.Sheets.Cast<Sheet>()
                .FirstOrDefault(s => s.Name == oldName);

            if (sheet != null)
            {
                sheet.Name = newName;
            }
        }

        #endregion

        #region 資料操作

        /// <summary>
        /// 取得指定工作表的 WorksheetPart
        /// </summary>
        private WorksheetPart GetWorksheetPart(string sheetName)
        {
            var sheet = WorkbookPart.Workbook.Sheets.Cast<Sheet>()
                .FirstOrDefault(s => s.Name == sheetName);

            if (sheet == null)
                throw new ArgumentException($"找不到工作表: {sheetName}");

            return (WorksheetPart)WorkbookPart.GetPartById(sheet.Id);
        }

        /// <summary>
        /// 讀取指定範圍的資料
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="startRow">起始行</param>
        /// <param name="startColumn">起始列</param>
        /// <param name="endRow">結束行</param>
        /// <param name="endColumn">結束列</param>
        public List<List<string>> ReadRange(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            var result = new List<List<string>>();

            for (int row = startRow; row <= endRow; row++)
            {
                var rowData = new List<string>();
                for (int col = startColumn; col <= endColumn; col++)
                {
                    var cellReference = GetCellReference(row, col);
                    var cell = GetCell(sheetData, cellReference);
                    var cellValue = GetCellValue(cell);
                    rowData.Add(cellValue);
                }
                result.Add(rowData);
            }

            return result;
        }

        /// <summary>
        /// 寫入資料到指定範圍
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="data">資料</param>
        /// <param name="startRow">起始行</param>
        /// <param name="startColumn">起始列</param>
        public void WriteRange(string sheetName, List<List<object>> data, int startRow, int startColumn)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            for (int i = 0; i < data.Count; i++)
            {
                var rowData = data[i];
                for (int j = 0; j < rowData.Count; j++)
                {
                    var cellReference = GetCellReference(startRow + i, startColumn + j);
                    var cell = GetCell(sheetData, cellReference);
                    SetCellValue(cell, rowData[j]);
                }
            }
        }

        /// <summary>
        /// 插入新行
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="rowIndex">插入位置（1-based）</param>
        /// <param name="count">插入行數</param>
        public void InsertRows(string sheetName, int rowIndex, int count = 1)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            var existingRows = sheetData.Elements<Row>()
                .Where(r => r.RowIndex >= rowIndex)
                .OrderByDescending(r => r.RowIndex)
                .ToList();

            foreach (var row in existingRows)
            {
                row.RowIndex = row.RowIndex + (uint)count;

                foreach (var cell in row.Elements<Cell>())
                {
                    var cellRef = cell.CellReference.Value;
                    var newRow = GetRowFromCellReference(cellRef) + count;
                    var col = GetColumnFromCellReference(cellRef);
                    cell.CellReference = GetCellReference(newRow, col);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var newRow = new Row() { RowIndex = (uint)(rowIndex + i) };
                sheetData.Append(newRow);
            }
        }

        /// <summary>
        /// 刪除行
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="rowIndex">刪除位置（1-based）</param>
        /// <param name="count">刪除行數</param>
        public void DeleteRows(string sheetName, int rowIndex, int count = 1)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            var rowsToDelete = sheetData.Elements<Row>()
                .Where(r => r.RowIndex >= rowIndex && r.RowIndex < rowIndex + count)
                .ToList();

            foreach (var row in rowsToDelete)
            {
                row.Remove();
            }

            var remainingRows = sheetData.Elements<Row>()
                .Where(r => r.RowIndex >= rowIndex + count)
                .ToList();

            foreach (var row in remainingRows)
            {
                row.RowIndex = row.RowIndex - (uint)count;

                foreach (var cell in row.Elements<Cell>())
                {
                    var cellRef = cell.CellReference.Value;
                    var newRow = GetRowFromCellReference(cellRef) - count;
                    var col = GetColumnFromCellReference(cellRef);
                    cell.CellReference = GetCellReference(newRow, col);
                }
            }
        }

        /// <summary>
        /// 合併儲存格
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="startRow">起始行</param>
        /// <param name="startColumn">起始列</param>
        /// <param name="endRow">結束行</param>
        /// <param name="endColumn">結束列</param>
        public void MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var worksheet = worksheetPart.Worksheet;

            var mergeCells = worksheet.Elements<MergeCells>().FirstOrDefault();
            if (mergeCells == null)
            {
                mergeCells = new MergeCells();
                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().FirstOrDefault());
            }

            var startCell = GetCellReference(startRow, startColumn);
            var endCell = GetCellReference(endRow, endColumn);
            var mergeCell = new MergeCell() { Reference = new StringValue($"{startCell}:{endCell}") };

            mergeCells.Append(mergeCell);
            mergeCells.Count = (uint)mergeCells.ChildElements.Count;
        }

        /// <summary>
        /// 自動調整欄位寬度
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="columnCount">欄位數量</param>
        /// <param name="minWidth">最小寬度</param>
        /// <param name="maxWidth">最大寬度</param>
        public void AutoSizeColumns(string sheetName, int columnCount, double minWidth = 15, double maxWidth = 50)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var worksheet = worksheetPart.Worksheet;

            // 取得或建立 Columns 元素
            var columns = worksheet.Elements<Columns>().FirstOrDefault();
            if (columns == null)
            {
                columns = new Columns();
                worksheet.InsertBefore(columns, worksheet.Elements<SheetData>().FirstOrDefault());
            }

            // 為每個欄位設定寬度
            for (int i = 1; i <= columnCount; i++)
            {
                // 檢查是否已存在該欄位的設定
                var existingColumn = columns.Elements<Column>()
                    .FirstOrDefault(col => col.Min <= i && col.Max >= i);

                if (existingColumn == null)
                {
                    var column = new Column()
                    {
                        Min = (uint)i,
                        Max = (uint)i,
                        Width = CalculateColumnWidth(sheetName, i, minWidth, maxWidth),
                        CustomWidth = true
                    };
                    columns.Append(column);
                }
            }
        }

        /// <summary>
        /// 計算欄位寬度（根據內容自動調整）
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="columnIndex">欄位索引（從1開始）</param>
        /// <param name="minWidth">最小寬度</param>
        /// <param name="maxWidth">最大寬度</param>
        /// <returns>計算後的寬度</returns>
        private double CalculateColumnWidth(string sheetName, int columnIndex, double minWidth, double maxWidth)
        {
            try
            {
                var worksheetPart = GetWorksheetPart(sheetName);
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                double maxContentWidth = 0;
                int sampleCount = 0;
                const int maxSamples = 20; // 限制樣本數量以提高效能

                // 檢查該欄位的內容長度
                foreach (var row in sheetData.Elements<Row>())
                {
                    if (sampleCount >= maxSamples) break;

                    var cellReference = GetCellReference((int)row.RowIndex.Value, columnIndex);
                    var cell = GetCell(sheetData, cellReference);
                    var cellValue = GetCellValue(cell);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        // 估算文字寬度（中文字符算2個寬度，英文算1個）
                        double textWidth = 0;
                        foreach (char c in cellValue)
                        {
                            if (c > 127) // 中文或其他多位元組字符
                                textWidth += 2;
                            else
                                textWidth += 1;
                        }

                        maxContentWidth = Math.Max(maxContentWidth, textWidth);
                        sampleCount++;
                    }
                }

                // 計算適當的欄位寬度
                double calculatedWidth = Math.Max(minWidth, maxContentWidth * 1.2); // 加20%的緩衝
                return Math.Min(calculatedWidth, maxWidth);
            }
            catch
            {
                // 如果計算失敗，回傳預設寬度
                return minWidth;
            }
        }

        #endregion

        #region 匯出功能

        /// <summary>
        /// 從 DataTable 匯出到 Excel
        /// </summary>
        /// <param name="dataTable">資料表</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="includeHeader">是否包含標題行</param>
        public void ExportFromDataTable(DataTable dataTable, string sheetName, bool includeHeader = true)
        {
            // 確保工作表存在
            if (!GetWorksheetNames().Contains(sheetName))
            {
                AddWorksheet(sheetName);
            }

            var data = new List<List<object>>();

            // 加入標題行
            if (includeHeader)
            {
                var headerRow = dataTable.Columns.Cast<DataColumn>()
                    .Select(column => (object)column.ColumnName)
                    .ToList();
                data.Add(headerRow);
            }

            // 加入資料行
            foreach (DataRow row in dataTable.Rows)
            {
                var dataRow = row.ItemArray.ToList();
                data.Add(dataRow);
            }

            WriteRange(sheetName, data, 1, 1);

            // 自動調整欄位寬度
            AutoSizeColumns(sheetName, dataTable.Columns.Count);
        }

        /// <summary>
        /// 從二維陣列匯出到 Excel
        /// </summary>
        /// <param name="data">資料陣列</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="headers">標題列</param>
        public void ExportFromArray(object[,] data, string sheetName, string[] headers = null)
        {
            if (!GetWorksheetNames().Contains(sheetName))
            {
                AddWorksheet(sheetName);
            }

            var exportData = new List<List<object>>();

            // 加入標題行
            if (headers != null)
            {
                exportData.Add(headers.Cast<object>().ToList());
            }

            // 加入資料
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                var row = new List<object>();
                for (int j = 0; j < cols; j++)
                {
                    row.Add(data[i, j]);
                }
                exportData.Add(row);
            }

            WriteRange(sheetName, exportData, 1, 1);
        }

        #endregion

        #region 輔助方法

        /// <summary>
        /// 取得儲存格參考 (例如: A1, B2)
        /// </summary>
        private string GetCellReference(int row, int column)
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

        /// <summary>
        /// 從儲存格參考取得行號
        /// </summary>
        private int GetRowFromCellReference(string cellReference)
        {
            var rowString = new string(cellReference.Where(char.IsDigit).ToArray());
            return int.Parse(rowString);
        }

        /// <summary>
        /// 從儲存格參考取得列號
        /// </summary>
        private int GetColumnFromCellReference(string cellReference)
        {
            var columnString = new string(cellReference.Where(char.IsLetter).ToArray());
            int column = 0;
            for (int i = 0; i < columnString.Length; i++)
            {
                column = column * 26 + (columnString[i] - 'A' + 1);
            }
            return column;
        }

        /// <summary>
        /// 取得或建立儲存格
        /// </summary>
        private Cell GetCell(SheetData sheetData, string cellReference)
        {
            var row = GetRowFromCellReference(cellReference);
            var column = GetColumnFromCellReference(cellReference);

            var sheetRow = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == row);
            if (sheetRow == null)
            {
                sheetRow = new Row() { RowIndex = (uint)row };
                sheetData.Append(sheetRow);
            }

            var cell = sheetRow.Elements<Cell>().FirstOrDefault(c => c.CellReference == cellReference);
            if (cell == null)
            {
                cell = new Cell() { CellReference = cellReference };
                sheetRow.Append(cell);
            }

            return cell;
        }

        /// <summary>
        /// 取得儲存格值
        /// </summary>
        private string GetCellValue(Cell cell)
        {
            if (cell == null) return string.Empty;

            var value = cell.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTable = WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (sharedStringTable != null)
                {
                    value = sharedStringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                }
            }

            return value;
        }

        /// <summary>
        /// 設定儲存格值
        /// </summary>
        private void SetCellValue(Cell cell, object value)
        {
            if (value == null)
            {
                cell.CellValue = new CellValue(string.Empty);
                cell.DataType = CellValues.String;
                return;
            }

            var stringValue = value.ToString();

            if (IsNumber(stringValue))
            {
                cell.CellValue = new CellValue(stringValue);
                cell.DataType = CellValues.Number;
            }
            else
            {
                cell.CellValue = new CellValue(stringValue);
                cell.DataType = CellValues.String;
            }
        }

        /// <summary>
        /// 判斷字串是否為數字
        /// </summary>
        private bool IsNumber(string value)
        {
            return double.TryParse(value, out _);
        }

        #endregion

        #region IDisposable 實作

        /// <summary>
        /// 儲存並關閉文件
        /// </summary>
        public void Save()
        {
            WorkbookPart?.Workbook?.Save();
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Save();
                Document?.Dispose();
                IsDisposed = true;
            }
        }

        #endregion

        public void CreateWorksheet(string sheetName)
        {
            throw new NotImplementedException();
        }
    }
}
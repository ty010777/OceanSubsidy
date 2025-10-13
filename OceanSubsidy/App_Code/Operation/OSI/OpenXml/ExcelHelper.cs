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
        /// 插入新行（改良版 - 複製上一行格式,正確處理行順序、公式和合併儲存格）
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="rowIndex">插入位置（1-based）</param>
        /// <param name="count">插入行數</param>
        public void InsertRows(string sheetName, int rowIndex, int count = 1)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            // 取得要複製格式的來源行 (rowIndex - 1)
            var templateRow = sheetData.Elements<Row>()
                .FirstOrDefault(r => r.RowIndex == (uint)(rowIndex - 1));

            // 1. 先將現有行向下移動
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

            // 2. 在正確位置插入新行,並複製上一行格式
            for (int i = 0; i < count; i++)
            {
                Row newRow;

                // 如果有範本行,複製它的格式
                if (templateRow != null)
                {
                    newRow = (Row)templateRow.CloneNode(true);
                    newRow.RowIndex = (uint)(rowIndex + i);

                    // 清除儲存格的值,只保留格式和公式
                    foreach (var cell in newRow.Elements<Cell>().ToList())
                    {
                        var col = GetColumnFromCellReference(cell.CellReference.Value);
                        cell.CellReference = GetCellReference(rowIndex + i, col);

                        // 如果儲存格有公式,保留公式
                        // 否則清除值,只保留樣式
                        if (cell.CellFormula == null)
                        {
                            // 移除 CellValue 元素來清除值
                            cell.RemoveAllChildren<CellValue>();
                        }
                    }
                }
                else
                {
                    // 沒有範本行,建立空白行
                    newRow = new Row() { RowIndex = (uint)(rowIndex + i) };
                }

                // 找到正確的插入位置（按照 RowIndex 順序）
                Row refRow = sheetData.Elements<Row>()
                    .FirstOrDefault(r => r.RowIndex > (uint)(rowIndex + i));

                if (refRow != null)
                {
                    sheetData.InsertBefore(newRow, refRow);
                }
                else
                {
                    sheetData.Append(newRow);
                }
            }

            // 3. 更新合併儲存格範圍
            UpdateMergedCellsOnInsert(worksheet, rowIndex, count);

            // 4. 刪除計算鏈,讓 Excel 重新建立以避免公式錯誤警告
            RemoveCalculationChain();
        }

        /// <summary>
        /// 移除計算鏈(Calculation Chain)
        /// 當插入或刪除行時,需要刪除計算鏈讓 Excel 重新建立,避免公式錯誤警告
        /// </summary>
        private void RemoveCalculationChain()
        {
            var calculationChainPart = WorkbookPart.CalculationChainPart;
            if (calculationChainPart != null)
            {
                WorkbookPart.DeletePart(calculationChainPart);
            }
        }

        /// <summary>
        /// 更新合併儲存格範圍（插入行時）
        /// </summary>
        /// <param name="worksheet">工作表</param>
        /// <param name="rowIndex">插入的行索引</param>
        /// <param name="count">插入的行數</param>
        private void UpdateMergedCellsOnInsert(Worksheet worksheet, int rowIndex, int count)
        {
            var mergeCells = worksheet.Elements<MergeCells>().FirstOrDefault();
            if (mergeCells == null) return;

            var mergeCellsList = mergeCells.Elements<MergeCell>().ToList();

            foreach (var mergeCell in mergeCellsList)
            {
                var reference = mergeCell.Reference.Value;
                var parts = reference.Split(':');

                if (parts.Length == 2)
                {
                    // 解析起始和結束儲存格
                    var startCell = parts[0];
                    var endCell = parts[1];

                    var startRow = GetRowFromCellReference(startCell);
                    var startCol = GetColumnFromCellReference(startCell);
                    var endRow = GetRowFromCellReference(endCell);
                    var endCol = GetColumnFromCellReference(endCell);

                    // 如果合併範圍的起始行 >= 插入位置，整個範圍向下移動
                    if (startRow >= rowIndex)
                    {
                        startRow += count;
                        endRow += count;

                        var newStartCell = GetCellReference(startRow, startCol);
                        var newEndCell = GetCellReference(endRow, endCol);
                        mergeCell.Reference = new StringValue($"{newStartCell}:{newEndCell}");
                    }
                    // 如果合併範圍跨越插入位置（起始行 < 插入位置 <= 結束行），擴展結束行
                    else if (startRow < rowIndex && endRow >= rowIndex)
                    {
                        endRow += count;
                        var newEndCell = GetCellReference(endRow, endCol);
                        mergeCell.Reference = new StringValue($"{startCell}:{newEndCell}");
                    }
                }
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

            // 刪除計算鏈,讓 Excel 重新建立以避免公式錯誤警告
            RemoveCalculationChain();
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

        /// <summary>
        /// 設定儲存格背景色（保留現有樣式）
        /// </summary>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="row">行號</param>
        /// <param name="column">列號</param>
        /// <param name="colorHex">顏色十六進位值（例如：FFD3D3D3 為淺灰色）</param>
        public void SetCellBackgroundColor(string sheetName, int row, int column, string colorHex)
        {
            var worksheetPart = GetWorksheetPart(sheetName);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            var cellReference = GetCellReference(row, column);
            var cell = GetCell(sheetData, cellReference);

            // 取得或建立 StylesPart
            var stylesPart = WorkbookPart.WorkbookStylesPart;
            if (stylesPart == null)
            {
                stylesPart = WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = CreateDefaultStylesheet();
            }

            // 取得儲存格當前的樣式索引
            uint currentStyleIndex = cell.StyleIndex != null ? cell.StyleIndex.Value : 0;

            // 複製現有樣式並只修改背景色
            uint newStyleIndex = CloneStyleWithNewFill(stylesPart.Stylesheet, currentStyleIndex, colorHex);

            // 設定儲存格樣式
            cell.StyleIndex = newStyleIndex;
        }

        /// <summary>
        /// 建立預設樣式表
        /// </summary>
        private Stylesheet CreateDefaultStylesheet()
        {
            var stylesheet = new Stylesheet();

            // Fonts
            var fonts = new Fonts() { Count = 1 };
            fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font());
            stylesheet.Append(fonts);

            // Fills
            var fills = new Fills() { Count = 2 };
            fills.Append(new Fill(new PatternFill() { PatternType = PatternValues.None })); // 0: 無填充
            fills.Append(new Fill(new PatternFill() { PatternType = PatternValues.Gray125 })); // 1: 預設灰色
            stylesheet.Append(fills);

            // Borders
            var borders = new Borders() { Count = 1 };
            borders.Append(new Border());
            stylesheet.Append(borders);

            // CellFormats
            var cellFormats = new CellFormats() { Count = 1 };
            cellFormats.Append(new CellFormat()); // 0: 預設格式
            stylesheet.Append(cellFormats);

            return stylesheet;
        }

        /// <summary>
        /// 複製現有樣式並只修改背景色
        /// </summary>
        /// <param name="stylesheet">樣式表</param>
        /// <param name="sourceStyleIndex">來源樣式索引</param>
        /// <param name="colorHex">新的背景顏色</param>
        /// <returns>新樣式的索引</returns>
        private uint CloneStyleWithNewFill(Stylesheet stylesheet, uint sourceStyleIndex, string colorHex)
        {
            var cellFormats = stylesheet.CellFormats;
            var fills = stylesheet.Fills;

            // 取得來源樣式
            var sourceCellFormat = (CellFormat)cellFormats.ElementAt((int)sourceStyleIndex);

            // 取得或建立新的填充色
            uint newFillIndex = GetOrCreateFill(fills, colorHex);

            // 檢查是否已經有相同配置的樣式（避免重複建立）
            uint styleIndex = 0;
            foreach (CellFormat cellFormat in cellFormats)
            {
                if (IsSameCellFormatExceptFill(cellFormat, sourceCellFormat, newFillIndex))
                {
                    return styleIndex;
                }
                styleIndex++;
            }

            // 複製來源樣式並修改填充色
            var newCellFormat = (CellFormat)sourceCellFormat.CloneNode(true);
            newCellFormat.FillId = newFillIndex;
            newCellFormat.ApplyFill = true;

            // 加入新樣式
            cellFormats.Append(newCellFormat);
            cellFormats.Count = (uint)cellFormats.ChildElements.Count;

            return cellFormats.Count - 1;
        }

        /// <summary>
        /// 取得或建立指定顏色的填充
        /// </summary>
        /// <param name="fills">填充集合</param>
        /// <param name="colorHex">顏色十六進位值</param>
        /// <returns>填充索引</returns>
        private uint GetOrCreateFill(Fills fills, string colorHex)
        {
            // 檢查是否已有指定顏色的填充
            uint fillIndex = 0;
            foreach (Fill fill in fills)
            {
                var patternFill = fill.PatternFill;
                if (patternFill != null &&
                    patternFill.ForegroundColor != null &&
                    patternFill.ForegroundColor.Rgb != null &&
                    patternFill.ForegroundColor.Rgb.Value == colorHex)
                {
                    return fillIndex;
                }
                fillIndex++;
            }

            // 建立新的填充
            var newFill = new Fill(
                new PatternFill(
                    new ForegroundColor() { Rgb = colorHex }
                )
                { PatternType = PatternValues.Solid }
            );
            fills.Append(newFill);
            fills.Count = (uint)fills.ChildElements.Count;

            return fills.Count - 1;
        }

        /// <summary>
        /// 比較兩個 CellFormat 是否相同（除了填充色）
        /// </summary>
        private bool IsSameCellFormatExceptFill(CellFormat format1, CellFormat format2, uint newFillIndex)
        {
            return format1.FillId == newFillIndex &&
                   format1.FontId == format2.FontId &&
                   format1.BorderId == format2.BorderId &&
                   format1.NumberFormatId == format2.NumberFormatId &&
                   format1.ApplyFont == format2.ApplyFont &&
                   format1.ApplyBorder == format2.ApplyBorder &&
                   format1.ApplyNumberFormat == format2.ApplyNumberFormat &&
                   format1.ApplyAlignment == format2.ApplyAlignment;
        }

        /// <summary>
        /// 取得或建立指定顏色的樣式索引（舊方法，保留以防相容性）
        /// </summary>
        /// <param name="stylesheet">樣式表</param>
        /// <param name="colorHex">顏色十六進位值</param>
        private uint GetOrCreateColorStyle(Stylesheet stylesheet, string colorHex)
        {
            var fills = stylesheet.Fills;
            var cellFormats = stylesheet.CellFormats;

            // 檢查是否已有指定顏色的填充
            uint colorFillIndex = 0;
            bool foundColorFill = false;

            uint fillIndex = 0;
            foreach (Fill fill in fills)
            {
                var patternFill = fill.PatternFill;
                if (patternFill != null &&
                    patternFill.ForegroundColor != null &&
                    patternFill.ForegroundColor.Rgb != null &&
                    patternFill.ForegroundColor.Rgb.Value == colorHex)
                {
                    colorFillIndex = fillIndex;
                    foundColorFill = true;
                    break;
                }
                fillIndex++;
            }

            // 如果沒有找到指定顏色的填充，則建立一個
            if (!foundColorFill)
            {
                var colorFill = new Fill(
                    new PatternFill(
                        new ForegroundColor() { Rgb = colorHex }
                    )
                    { PatternType = PatternValues.Solid }
                );
                fills.Append(colorFill);
                fills.Count = (uint)fills.ChildElements.Count;
                colorFillIndex = fills.Count - 1;
            }

            // 檢查是否已有使用此填充的樣式
            uint styleIndex = 0;
            foreach (CellFormat cellFormat in cellFormats)
            {
                if (cellFormat.FillId != null && cellFormat.FillId.Value == colorFillIndex)
                {
                    return styleIndex;
                }
                styleIndex++;
            }

            // 建立新的樣式
            var newCellFormat = new CellFormat()
            {
                FillId = colorFillIndex,
                ApplyFill = true
            };
            cellFormats.Append(newCellFormat);
            cellFormats.Count = (uint)cellFormats.ChildElements.Count;

            return cellFormats.Count - 1;
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
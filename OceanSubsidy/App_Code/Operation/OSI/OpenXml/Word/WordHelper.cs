// using System;
// using System.Text.RegularExpressions;
// using DocumentFormat.OpenXml;
// using DocumentFormat.OpenXml.Packaging;
// using DocumentFormat.OpenXml.Wordprocessing;
// using Newtonsoft.Json;
// using A = DocumentFormat.OpenXml.Drawing;
// using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
// using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
// using System.Collections.Generic;
// using System.Linq;
// using System.IO;
// using System.Drawing;
// /// <summary>
// /// 專門處理 Word 文件中 Bookmark
// /// </summary>
// namespace OperationLibrary.Common.Helper.Word
// {
//
//     public class WordHelper : IDisposable
//     {
//         private bool _disposed;
//         private readonly WordprocessingDocument _document;
//         private readonly Dictionary<string, BookmarkStart> _bookmarks;
//
//         public WordHelper(string filePath)
//         {
//             _document = WordprocessingDocument.Open(filePath, true);
//             _bookmarks = LoadBookmarks();
//         }
//
//         /// <summary>
//         /// 載入所有書籤
//         /// </summary>
//         private Dictionary<string, BookmarkStart> LoadBookmarks()
//         {
//             try
//             {
//                 var bookmarks = new Dictionary<string, BookmarkStart>();
//                 if (_document.MainDocumentPart == null || _document.MainDocumentPart.Document?.Body == null)
//                     return bookmarks;
//                 var bookmarkStarts = _document.MainDocumentPart.Document.Body
//                     .Descendants<BookmarkStart>();
//
//                 foreach (var bookmark in bookmarkStarts)
//                 {
//                     if (!string.IsNullOrEmpty(bookmark.Name?.Value))
//                     {
//                         bookmarks[bookmark.Name.Value] = bookmark;
//                     }
//                 }
//
//                 return bookmarks;
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("載入書籤時發生錯誤", ex);
//             }
//         }
//
//         #region 普通替換
//
//         /// <summary>
//         /// 替代頁首內容
//         /// </summary>
//         /// <param name="replacements"></param>
//         public void ReplaceHeaderContent(Dictionary<string, string> replacements)
//         {
//             try
//             {
//                 var headerParts = _document.MainDocumentPart?.HeaderParts;
//                 if (headerParts == null) return;
//
//                 foreach (var headerPart in headerParts)
//                 {
//                     var header = headerPart.Header;
//
//                     foreach (var text in header.Descendants<Text>())
//                     {
//                         foreach (var replacement in replacements
//                                      .Where(r => text.Text.Contains(r.Key)))
//                         {
//                             text.Text = text.Text.Replace(replacement.Key, replacement.Value);
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("替換文字內容時發生錯誤", ex);
//             }
//         }
//
//         /// <summary>
//         /// 替換文字內容
//         /// </summary>
//         public void ReplaceTextContent(Dictionary<string, string> replacements)
//         {
//             var body = _document.MainDocumentPart?.Document.Body;
//             if (body == null) return;
//
//             foreach (var text in body.Descendants<Text>())
//             {
//                 foreach (var replacement in replacements
//                              .Where(replacement => text.Text.Contains(replacement.Key)))
//                 {
//                     text.Text = text.Text.Replace(replacement.Key, replacement.Value);
//                 }
//             }
//         }
//
//         #endregion
//
//         #region 書籤內容替換
//
//         /// <summary>
//         /// 替換書籤內的文字內容
//         /// </summary>
//         /// <param name="bookmarkName">書籤</param>
//         /// <param name="value">新文字內容</param>
//         public void ReplaceBookmarkText(string bookmarkName, string value)
//         {
//             ValidateBookmark(bookmarkName);
//
//             try
//             {
//                 var bookmark = _bookmarks[bookmarkName];
//                 var bookmarkEnd = bookmark.NextSibling<BookmarkEnd>();
//
//                 if (bookmarkEnd == null)
//                 {
//                     throw new Exception($"書籤結束標記未找到: {bookmarkName}");
//                 }
//
//                 // 收集書籤範圍內的所有 Run 元素
//                 var currentElement = bookmark.NextSibling();
//                 var runs = new List<Run>();
//
//                 while (currentElement != null && currentElement != bookmarkEnd)
//                 {
//                     if (currentElement is Run run)
//                     {
//                         runs.Add(run);
//                     }
//
//                     currentElement = currentElement.NextSibling();
//                 }
//
//                 if (runs.Count == 0)
//                 {
//                     // 如果沒有找到任何 Run，創建新的 Run 並插入
//                     var paragraph = bookmark.Ancestors<Paragraph>().FirstOrDefault();
//                     if (paragraph == null) return;
//
//                     var newRun = new Run(new Text(value));
//                     paragraph.InsertAfter(newRun, bookmark);
//                     return;
//                 }
//
//                 // 使用第一個 Run 的格式替換文字
//                 var firstRun = runs.First();
//                 var text = firstRun.GetFirstChild<Text>();
//                 if (text == null) return;
//                 text.Text = value;
//
//                 // 移除其他 Run
//                 foreach (var run in runs.Skip(1))
//                 {
//                     run.Remove();
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"替換文字內容時發生錯誤 (Bookmark: {bookmarkName})", ex);
//             }
//         }
//
//         /// <summary>
//         /// 替換書籤內的文字內容並套用項目符號或編號
//         /// </summary>
//         /// <param name="bookmarkName">書籤</param>
//         /// <param name="value">新文字內容</param>
//         /// <param name="listType">列表類型：null=無編號, "number"=數字編號, "bullet"=項目符號</param>
//         public void ReplaceBookmarkTextWithFormatting(string bookmarkName, string value, string listType = null)
//         {
//             ValidateBookmark(bookmarkName);
//
//             try
//             {
//                 var bookmark = _bookmarks[bookmarkName];
//                 var paragraph = bookmark.Ancestors<Paragraph>().FirstOrDefault();
//                 if (paragraph == null) return;
//
//                 var parent = paragraph.Parent;
//
//                 // 保存原始格式
//                 var originalParagraphProps =
//                     paragraph.Elements<ParagraphProperties>().FirstOrDefault()?.CloneNode(true) as ParagraphProperties;
//                 var originalRunProps =
//                     paragraph.Elements<Run>().FirstOrDefault()?.Elements<RunProperties>().FirstOrDefault()
//                         ?.CloneNode(true) as RunProperties;
//
//                 // 移除現有內容
//                 var currentElement = bookmark.NextSibling();
//                 while (currentElement != null && !(currentElement is BookmarkEnd))
//                 {
//                     var next = currentElement.NextSibling();
//                     currentElement.Remove();
//                     currentElement = next;
//                 }
//
//                 // 分割文字行
//                 var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
//
//                 // 處理每一行
//                 for (var i = 0; i < lines.Length; i++)
//                 {
//                     var line = lines[i].Trim();
//                     if (string.IsNullOrWhiteSpace(line)) continue;
//
//                     var run = new Run();
//                     if (originalRunProps != null)
//                     {
//                         run.PrependChild(originalRunProps.CloneNode(true));
//                     }
//
//                     // 根據列表類型添加前綴
//                     string prefix;
//                     switch (listType)
//                     {
//                         case "number":
//                             prefix = $"{i + 1}. ";
//                             break;
//                         case "bullet":
//                             prefix = "• ";
//                             break;
//                         default:
//                             prefix = string.Empty;
//                             break;
//                     }
//
//                     run.AppendChild(new Text(prefix + line));
//
//                     if (i == 0)
//                     {
//                         paragraph.AppendChild(run);
//                     }
//                     else
//                     {
//                         var newParagraph = new Paragraph();
//                         if (originalParagraphProps != null)
//                         {
//                             newParagraph.PrependChild(originalParagraphProps.CloneNode(true));
//                         }
//
//                         newParagraph.AppendChild(run);
//                         parent?.InsertAfter(newParagraph, paragraph);
//                         paragraph = newParagraph;
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"替換文字內容時發生錯誤 (書籤: {bookmarkName})", ex);
//             }
//         }
//
//         #endregion
//
//         #region 書籤表格替換
//
//         /// <summary>
//         /// 替換書籤內的表格內容
//         /// </summary>
//         public void ReplaceBookmarkTable(string bookmarkName, List<List<BookmarkData>> data, int headerRowCount = 1,
//             int footerRowCount = 0, bool isMarkdown = false)
//         {
//             ValidateBookmark(bookmarkName);
//
//             var bookmark = _bookmarks[bookmarkName];
//             var table = bookmark.Ancestors<Table>().FirstOrDefault();
//             if (table == null)
//             {
//                 throw new Exception($"書籤 {bookmarkName} 不在表格中");
//             }
//
//             try
//             {
//                 var existingRows = table.Elements<TableRow>().ToList();
//                 if (!existingRows.Any() || existingRows.Count < headerRowCount + footerRowCount)
//                 {
//                     throw new Exception($"表格行數不足: 需要至少 {headerRowCount} 行表頭和 {footerRowCount} 行表尾");
//                 }
//
//                 // 取得要保留的表頭行
//                 var headerRows = existingRows.Take(headerRowCount).ToList();
//
//                 // 取得要保留的表尾行
//                 var footerRows = footerRowCount > 0
//                     ? existingRows.Skip(existingRows.Count - footerRowCount).ToList()
//                     : new List<TableRow>();
//
//                 // 建立欄位順序對應
//                 var columnMapping = BuildSequentialColumnMapping(headerRows);
//
//                 // 移除所有非表頭和表尾的行
//                 foreach (var row in existingRows.Skip(headerRowCount)
//                              .Take(existingRows.Count - headerRowCount - footerRowCount)
//                              .ToList())
//                 {
//                     row.Remove();
//                 }
//
//                 // 新增資料行
//                 foreach (var rowData in data)
//                 {
//                     var templateRow = headerRows[headerRowCount - 1];
//                     var newRow = (TableRow)templateRow.Clone();
//                     var cells = newRow.Elements<TableCell>().ToList();
//
//                     // 清除所有儲存格的內容
//                     foreach (var cell in cells)
//                     {
//                         ReplaceCellTextPreserveFormat(cell, string.Empty);
//                     }
//
//                     // 使用 Dictionary 來追蹤每個欄位名稱已經使用的次數
//                     var keyUsageCount = new Dictionary<string, int>();
//
//                     // 填入資料
//                     foreach (var columnData in rowData)
//                     {
//                         // 初始化或遞增使用次數
//                         if (!keyUsageCount.ContainsKey(columnData.Label))
//                         {
//                             keyUsageCount.Add(columnData.Label, 0);
//                         }
//
//                         // 取得目前這個 key 應該對應的第幾個位置
//                         var currentIndex = keyUsageCount[columnData.Label];
//
//                         // 如果有對應的欄位位置
//                         List<int> positions;
//                         if (columnMapping.TryGetValue(columnData.Label, out positions) &&
//                             currentIndex < positions.Count)
//                         {
//                             var cellIndex = positions[currentIndex];
//                             if (cellIndex < cells.Count)
//                             {
//                                 if (isMarkdown)
//                                 {
//                                     ProcessMarkdownInCell(cells[cellIndex], columnData.Value);
//                                 }
//                                 else
//                                 {
//                                     ReplaceCellTextPreserveFormat(cells[cellIndex], columnData.Value);
//                                 }
//                             }
//                         }
//
//
//                         keyUsageCount[columnData.Label]++;
//                     }
//
//                     // 在表尾行之前插入新行
//                     if (footerRows.Any())
//                     {
//                         table.InsertBefore(newRow, footerRows.First());
//                     }
//                     else
//                     {
//                         table.AppendChild(newRow);
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"替換表格內容時發生錯誤 (Bookmark: {bookmarkName})", ex);
//             }
//         }
//
//         /// <summary>
//         /// 替換儲存格文字並保留格式
//         /// </summary>
//         private void ReplaceCellTextPreserveFormat(TableCell cell, string newText)
//         {
//             try
//             {
//                 var paragraphs = cell.Elements<Paragraph>().ToList();
//                 if (!paragraphs.Any())
//                 {
//                     cell.AppendChild(new Paragraph(new Run(new Text(newText))));
//                     return;
//                 }
//
//                 var firstParagraph = paragraphs.First();
//                 var runs = firstParagraph.Elements<Run>().ToList();
//
//                 if (!runs.Any())
//                 {
//                     firstParagraph.AppendChild(new Run(new Text(newText)));
//                     return;
//                 }
//
//                 // 保存第一個 Run 的格式
//                 var templateRun = runs.First();
//                 var newRun = (Run)templateRun.Clone();
//                 newRun.RemoveAllChildren<Text>();
//                 newRun.AppendChild(new Text(newText));
//
//                 // 清除原有內容並加入新的 Run
//                 firstParagraph.RemoveAllChildren<Run>();
//                 firstParagraph.AppendChild(newRun);
//
//                 // 移除多餘的段落
//                 foreach (var paragraph in paragraphs.Skip(1))
//                 {
//                     paragraph.Remove();
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("替換儲存格文字時發生錯誤", ex);
//             }
//         }
//
//         /// <summary>
//         /// 替換儲存格文字並保留格式 與 對齊方式
//         /// </summary>
//         /// <param name="cell"></param>
//         /// <param name="newText"></param>
//         /// <param name="alignment"></param>
//         /// <exception cref="Exception"></exception>
//         private void ReplaceCellTextPreserveFormat(TableCell cell, string newText, CellAlignment alignment)
//         {
//             try
//             {
//                 var paragraphs = cell.Elements<Paragraph>().ToList();
//                 if (!paragraphs.Any())
//                 {
//                     cell.AppendChild(new Paragraph(new Run(new Text(newText))));
//                     SetCellAlignment(cell, alignment);
//                     return;
//                 }
//
//                 var firstParagraph = paragraphs.First();
//                 var runs = firstParagraph.Elements<Run>().ToList();
//
//                 if (!runs.Any())
//                 {
//                     firstParagraph.AppendChild(new Run(new Text(newText)));
//                     SetCellAlignment(cell, alignment);
//                     return;
//                 }
//
//                 // 保存第一個 Run 的格式
//                 var templateRun = runs.First();
//                 var newRun = (Run)templateRun.Clone();
//                 newRun.RemoveAllChildren<Text>();
//                 newRun.AppendChild(new Text(newText));
//
//                 // 清除原有內容並加入新的 Run
//                 firstParagraph.RemoveAllChildren<Run>();
//                 firstParagraph.AppendChild(newRun);
//
//                 // 設定對齊方式
//                 SetCellAlignment(cell, alignment);
//
//                 // 移除多餘的段落
//                 foreach (var paragraph in paragraphs.Skip(1))
//                 {
//                     paragraph.Remove();
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("替換儲存格文字時發生錯誤", ex);
//             }
//         }
//
//
//         /// <summary>
//         /// 建立欄位順序對應資訊
//         /// </summary>
//         private Dictionary<string, List<int>> BuildSequentialColumnMapping(List<TableRow> headerRows)
//         {
//             var mapping = new Dictionary<string, List<int>>();
//
//             // 遍歷所有表頭行
//             foreach (var cells in headerRows.Select(headerRow => headerRow.Elements<TableCell>().ToList()))
//             {
//                 for (var i = 0; i < cells.Count; i++)
//                 {
//                     var cellText = GetCellText(cells[i]).Trim();
//                     if (string.IsNullOrEmpty(cellText)) continue;
//
//                     List<int> value;
//                     if (!mapping.TryGetValue(cellText, out value))
//                     {
//                         value = new List<int>();
//                         mapping[cellText] = value;
//                     }
//
//                     value.Add(i);
//                 }
//             }
//
//             return mapping;
//         }
//
//         private string GetCellText(TableCell cell)
//         {
//             return string.Join("", cell.Descendants<Text>().Select(t => t.Text));
//         }
//
//         #endregion
//
//         #region 處理多個複雜表格
//
//         /// <summary>
//         /// 處理多個複雜表格，如果在DataContent中需要合并儲存格則請一律要放第一行
//         /// <code>
//         /// Rows = new List<TableCustomRow/>
//         /// {
//         ///     new()
//         ///     {
//         ///         Cells = new List
//         ///         {
//         ///             new() { Key = "LimitName", Column = 0, IsTitle = false },
//         ///             new() { Key = "Value", Column = 1, IsTitle = false },
//         ///             new() { Key = "Unit", Column = 2, IsTitle = false }
//         ///         }
//         ///     }
//         /// },
//         /// Content = new DataContent
//         /// {
//         ///     SourceKey = "LimitList",
//         ///     Columns = new List
//         ///     {
//         ///         new() { SourceField = "LimitName", TargetField = "LimitName", Column = 0 },
//         ///         new() { SourceField = "Value", TargetField = "Value", Column = 2 },
//         ///         new() { SourceField = "Unit", TargetField = "Unit", Column = 3 }
//         ///     }
//         /// },
//         /// MergedCells = new List
//         /// {
//         ///     new() { Row = 0, StartColumn = 0, EndColumn = 1 }
//         /// }
//         /// </code>
//         /// </summary>
//         /// <param name="bookmarkName"></param>
//         /// <param name="dataList"></param>
//         /// <param name="tableTemplate"></param>
//         /// <exception cref="Exception"></exception>
//         public void HandleMultipleComplexTables(string bookmarkName, List<Dictionary<string, string>> dataList,
//             List<TableTemplate> tableTemplate)
//         {
//             ValidateBookmark(bookmarkName);
//
//             try
//             {
//                 var bookmark = _bookmarks[bookmarkName];
//                 var originalTable = bookmark.Ancestors<Table>().FirstOrDefault();
//                 if (originalTable == null)
//                 {
//                     throw new Exception($"書籤 {bookmarkName} 不在表格中");
//                 }
//
//                 // 複製原表格的樣式
//                 var originalTableProperties = originalTable.GetFirstChild<TableProperties>() ?? new TableProperties(
//                     new TableBorders(
//                         new TopBorder { Val = BorderValues.Single, Size = 24 },
//                         new BottomBorder { Val = BorderValues.Single, Size = 24 },
//                         new LeftBorder { Val = BorderValues.Single, Size = 24 },
//                         new RightBorder { Val = BorderValues.Single, Size = 24 },
//                         new InsideHorizontalBorder { Val = BorderValues.Single },
//                         new InsideVerticalBorder { Val = BorderValues.Single }
//                     ),
//                     new TableWidth { Type = TableWidthUnitValues.Auto }
//                 );
//
//                 var parent = originalTable.Parent;
//                 OpenXmlElement previousElement = originalTable;
//
//                 // 為每筆資料建立表格
//                 for (var dataIndex = 0; dataIndex < dataList.Count; dataIndex++)
//                 {
//                     // 調整表格範本
//                     var adjustedTemplate = AdjustTemplateRowIndices(tableTemplate, dataList[dataIndex]);
//
//                     // 建立新表格
//                     var currentTable = CreateTableFromTemplate(adjustedTemplate, originalTableProperties);
//
//                     // 如果是第一個表格，替換原表格
//                     if (dataIndex == 0)
//                     {
//                         parent?.ReplaceChild(currentTable, originalTable);
//                     }
//                     else
//                     {
//                         // 插入分隔段落
//                         var separator = new Paragraph(new Run(new Break()));
//                         parent?.InsertAfter(separator, previousElement);
//                         previousElement = separator;
//
//                         // 插入新表格
//                         parent?.InsertAfter(currentTable, previousElement);
//                     }
//
//                     previousElement = currentTable;
//
//                     // 填充表格資料
//                     FillTableData(currentTable, dataList[dataIndex], adjustedTemplate);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"處理多個表格時發生錯誤 (書籤: {bookmarkName})", ex);
//             }
//         }
//
//         /// <summary>
//         /// 調整表格範本的行索引
//         /// </summary>
//         private List<TableTemplate> AdjustTemplateRowIndices(List<TableTemplate> originalTemplate,
//             Dictionary<string, string> data)
//         {
//             var adjustedTemplate = new List<TableTemplate>();
//             var currentRow = 0;
//
//             foreach (var block in originalTemplate)
//             {
//                 // 計算這個區塊實際需要的列數
//                 int blockRows;
//
//                 string jsonData;
//                 if (data.TryGetValue(block.Content.SourceKey, out jsonData))
//                 {
//                     var dynamicContent = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonData);
//                     // 動態內容區塊的列數就是資料的筆數
//                     blockRows = dynamicContent != null ? dynamicContent.Count : 1;
//                 }
//                 else
//                 {
//                     // 一般區塊的列數就是其 Rows 的數量
//                     blockRows = block.Rows.Count;
//                 }
//
//                 var adjustedBlock = new TableTemplate
//                 {
//                     StartRow = currentRow,
//                     EndRow = currentRow + blockRows - 1,
//                     Rows = block.Rows,
//                     MergedCells = new List<MergeInfo>(),
//                     Content = block.Content
//                 };
//
//                 // 複製並調整合併儲存格設定
//                 foreach (var mergeInfo in block.MergedCells)
//                 {
//                     adjustedBlock.MergedCells.Add(new MergeInfo
//                     {
//                         Row = mergeInfo.Row,
//                         StartColumn = mergeInfo.StartColumn,
//                         EndColumn = mergeInfo.EndColumn
//                     });
//                 }
//
//                 // 更新當前列位置
//                 currentRow += blockRows;
//                 adjustedTemplate.Add(adjustedBlock);
//             }
//
//             return adjustedTemplate;
//         }
//
//         /// <summary>
//         /// 從範本建立表格
//         /// </summary>
//         private Table CreateTableFromTemplate(List<TableTemplate> tableTemplate, OpenXmlElement originalTableProperties)
//         {
//             var table = new Table();
//
//             // 套用原表格樣式
//             if (originalTableProperties is TableProperties tableProps)
//             {
//                 table.AppendChild(tableProps.CloneNode(true));
//             }
//
//             // 取得所有列數
//             var maxColumns = tableTemplate
//                 .SelectMany(t => t.Rows)
//                 .SelectMany(r => r.Cells)
//                 .Max(c => c.Column) + 1;
//
//             // 計算總列數（使用最後一個區塊的 EndRow + 1）
//             var totalRows = tableTemplate.Last().EndRow + 1;
//
//             // 建立所有行
//             for (var rowIndex = 0; rowIndex < totalRows; rowIndex++)
//             {
//                 var tableRow = new TableRow();
//
//                 // 建立儲存格
//                 for (var colIndex = 0; colIndex < maxColumns; colIndex++)
//                 {
//                     var alignment = GetCellAlignmentFromTemplate(tableTemplate, rowIndex, colIndex);
//                     var cell = CreateEmptyCell(alignment);
//                     tableRow.AppendChild(cell);
//                 }
//
//                 table.AppendChild(tableRow);
//             }
//
//             return table;
//         }
//
//         /// <summary>
//         /// 從範本取得儲存格對齊方式
//         /// </summary>
//         private CellAlignment GetCellAlignmentFromTemplate(List<TableTemplate> tableTemplate, int rowIndex, int colIndex)
//         {
//             return (
//                 from template in tableTemplate
//                 where rowIndex >= template.StartRow && rowIndex <= template.EndRow
//                 let relativeRow = rowIndex - template.StartRow
//                 where relativeRow < template.Rows.Count
//                 select template.Rows[relativeRow].Cells.FirstOrDefault(c => c.Column == colIndex)
//                 into cell
//                 where cell != null
//                 select cell.Alignment).FirstOrDefault();
//         }
//
//         /// <summary>
//         /// 填充表格資料
//         /// </summary>
//         private void FillTableData(Table table, Dictionary<string, string> data, List<TableTemplate> tableTemplate)
//         {
//             try
//             {
//                 var rows = table.Elements<TableRow>().ToList();
//                 if (!rows.Any()) throw new Exception("表格未包含任何列");
//
//                 // 處理所有區塊
//                 foreach (var block in tableTemplate)
//                 {
//                     // 處理合併儲存格
//                     foreach (var mergeInfo in block.MergedCells)
//                     {
//                         var rowIndex = block.StartRow + mergeInfo.Row;
//                         if (rowIndex <= block.EndRow && rowIndex < rows.Count)
//                         {
//                             MergeCells(table, rowIndex, mergeInfo.StartColumn, mergeInfo.EndColumn);
//                         }
//                     }
//
//                     // 填充一般資料
//                     var rowCount = Math.Min(block.Rows.Count, block.EndRow - block.StartRow + 1);
//                     for (var i = 0; i < rowCount; i++)
//                     {
//                         var rowIndex = block.StartRow + i;
//                         if (rowIndex >= rows.Count) continue;
//
//                         var rowTemplate = block.Rows[i];
//                         var tableRow = rows[rowIndex];
//
//                         foreach (var cellTemplate in rowTemplate.Cells)
//                         {
//                             var cells = tableRow.Elements<TableCell>().ToList();
//                             if (cellTemplate.Column >= cells.Count) continue;
//
//                             var cell = cells[cellTemplate.Column];
//
//                             if (cellTemplate.IsTitle)
//                             {
//                                 if (cellTemplate.IsMarkdown)
//                                 {
//                                     ProcessMarkdownInCell(cell, cellTemplate.Key);
//                                     SetCellAlignment(cell, cellTemplate.Alignment);
//                                 }
//                                 else
//                                 {
//                                     ReplaceCellTextPreserveFormat(cell, cellTemplate.Key, cellTemplate.Alignment);
//                                 }
//                             }
//                             else 
//                             {
//                                 string value;
//                                 if (data.TryGetValue(cellTemplate.Key, out value))
//                                 {
//                                     if (cellTemplate.IsMarkdown)
//                                     {
//                                         ProcessMarkdownInCell(cell, value);
//                                         SetCellAlignment(cell, cellTemplate.Alignment);
//                                     }
//                                     else
//                                     {
//                                         ReplaceCellTextPreserveFormat(cell, value, cellTemplate.Alignment);
//                                     }
//                                 }
//                             }
//                         }
//                     }
//
//                     // 處理動態內容
//                     string jsonData2;
//                     if (data.TryGetValue(block.Content.SourceKey, out jsonData2))
//                     {
//                         var dynamicContent = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonData2);
//                         if (dynamicContent != null && dynamicContent.Count > 0)
//                         {
//                             var templateRow = rows[block.StartRow];
//                             ProcessDynamicContent(table, block, dynamicContent, templateRow);
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("填充表格資料時發生錯誤", ex);
//             }
//         }
//
//         /// <summary>
//         /// 處理動態內容
//         /// </summary>
//         private void ProcessDynamicContent(Table table, TableTemplate block,
//             List<Dictionary<string, string>> dynamicContent, TableRow templateRow)
//         {
//             try
//             {
//                 if (!dynamicContent.Any()) return;
//
//                 var rows = table.Elements<TableRow>().ToList();
//                 var contentStartIndex = block.StartRow;
//
//                 // 移除指定範圍內的所有列（包含範本列）
//                 var rowsToRemove = rows
//                     .Skip(contentStartIndex)
//                     .Take(block.EndRow - contentStartIndex + 1)
//                     .ToList();
//
//                 foreach (var row in rowsToRemove)
//                 {
//                     row.Remove();
//                 }
//
//                 var previousRow = contentStartIndex == 0 ? null : rows[contentStartIndex - 1];
//                 // ReSharper disable once NotAccessedVariable
//                 var currentRowIndex = contentStartIndex;
//
//                 // 檢查是否有需要的合併儲存格設定
//                 var mergeSettings = block.MergedCells
//                     .Where(m => m.Row == 0)
//                     .ToList();
//
//                 // 取得範本列的所有儲存格，用於建立新列
//                 var templateCells = templateRow.Elements<TableCell>().ToList();
//                 var maxColumn = Math.Max(
//                     templateCells.Count - 1,
//                     block.Content.Columns.Max(c => c.Column)
//                 );
//
//                 // 插入動態內容
//                 foreach (var item in dynamicContent)
//                 {
//                     var newRow = (TableRow)templateRow.Clone();
//                     var cells = newRow.Elements<TableCell>().ToList();
//
//                     // 確保有足夠的儲存格
//                     while (cells.Count <= maxColumn)
//                     {
//                         var newCell = new TableCell(new Paragraph(new Run(new Text(string.Empty))));
//                         newRow.AppendChild(newCell);
//                         cells = newRow.Elements<TableCell>().ToList();
//                     }
//
//                     // 填充資料
//                     foreach (var column in block.Content.Columns)
//                     {
//                         if (column.Column >= cells.Count) continue;
//
//                         string v;
//                         var value = item.TryGetValue(column.SourceField, out v) ? v : string.Empty;
//                         if (column.IsMarkdown)
//                         {
//                             ProcessMarkdownInCell(cells[column.Column], value);
//                         }
//                         else
//                         {
//                             ReplaceCellTextPreserveFormat(cells[column.Column], value);
//                         }
//                     }
//
//                     // 插入新行
//                     if (previousRow != null)
//                     {
//                         table.InsertAfter(newRow, previousRow);
//                     }
//                     else
//                     {
//                         table.InsertAt(newRow, 0);
//                     }
//
//                     previousRow = newRow;
//
//                     // 處理合併儲存格
//                     foreach (var mergeInfo in mergeSettings)
//                     {
//                         // 考慮到可能有不連續的欄位
//                         var startColumn = mergeInfo.StartColumn;
//                         var endColumn = mergeInfo.EndColumn;
//
//                         if (startColumn < cells.Count && endColumn < cells.Count)
//                         {
//                             var firstCell = cells[startColumn];
//
//                             // 設定合併屬性
//                             var tcPr = firstCell.GetFirstChild<TableCellProperties>();
//                             if (tcPr == null)
//                             {
//                                 tcPr = new TableCellProperties();
//                                 firstCell.InsertAt(tcPr, 0);
//                             }
//
//                             var gridSpan = new GridSpan { Val = endColumn - startColumn + 1 };
//                             tcPr.RemoveAllChildren<GridSpan>();
//                             tcPr.AppendChild(gridSpan);
//
//                             // 移除被合併的儲存格
//                             for (var i = endColumn; i > startColumn; i--)
//                             {
//                                 if (i < cells.Count)
//                                 {
//                                     cells[i].Remove();
//                                 }
//                             }
//                         }
//                     }
//
//                     currentRowIndex++;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("處理動態內容時發生錯誤", ex);
//             }
//         }
//
//         /// <summary>
//         /// 合併儲存格
//         /// </summary>
//         private void MergeCells(Table table, int rowIndex, int startColumn, int endColumn)
//         {
//             var rows = table.Elements<TableRow>().ToList();
//             if (rowIndex >= rows.Count) return;
//
//             var row = rows[rowIndex];
//             var cells = row.Elements<TableCell>().ToList();
//
//             if (startColumn >= cells.Count || endColumn >= cells.Count) return;
//
//             // 設定第一個儲存格的合併屬性
//             var firstCell = cells[startColumn];
//             var gridSpan = new GridSpan { Val = endColumn - startColumn + 1 };
//             var tcPr = firstCell.GetFirstChild<TableCellProperties>() ??
//                        firstCell.InsertAt(new TableCellProperties(), 0);
//             tcPr.GridSpan = gridSpan;
//
//             // 移除其他要合併的儲存格
//             for (var i = endColumn; i > startColumn; i--)
//             {
//                 cells[i].Remove();
//             }
//         }
//
//         private void SetCellAlignment(TableCell cell, CellAlignment alignment)
//         {
//             JustificationValues justification;
//             switch (alignment)
//             {
//                 case CellAlignment.Center:
//                     justification = JustificationValues.Center;
//                     break;
//                 case CellAlignment.Right:
//                     justification = JustificationValues.Right;
//                     break;
//                 default:
//                     justification = JustificationValues.Left;
//                     break;
//             }
//
//             var paragraphs = cell.Elements<Paragraph>().ToList();
//             foreach (var paragraph in paragraphs)
//             {
//                 var properties = paragraph.GetFirstChild<ParagraphProperties>();
//                 if (properties == null)
//                 {
//                     properties = new ParagraphProperties();
//                     paragraph.PrependChild(properties);
//                 }
//
//                 var justify = properties.GetFirstChild<Justification>();
//                 if (justify == null)
//                 {
//                     properties.AppendChild(new Justification { Val = justification });
//                 }
//                 else
//                 {
//                     justify.Val = justification;
//                 }
//             }
//         }
//
//         private TableCell CreateEmptyCell(CellAlignment alignment = CellAlignment.Left)
//         {
//             var paragraph = new Paragraph(new Run(new Text(string.Empty)));
//             var cell = new TableCell(paragraph);
//
//             SetCellAlignment(cell, alignment);
//             return cell;
//         }
//
//         #endregion
//
//         #region 刪除書籤内容
//
//         /// <summary>
//         /// 清空書籤中的所有內容
//         /// </summary>
//         /// <param name="bookmarkName">書籤名稱</param>
//         // ReSharper disable once MemberCanBePrivate.Global
//         public void ClearBookmarkContent(string bookmarkName)
//         {
//             ValidateBookmark(bookmarkName);
//
//             try
//             {
//                 var bookmark = _bookmarks[bookmarkName];
//                 var bookmarkEnd = bookmark.NextSibling<BookmarkEnd>();
//
//                 if (bookmarkEnd == null)
//                 {
//                     throw new Exception($"書籤結束標記未找到: {bookmarkName}");
//                 }
//
//                 // 收集書籤範圍內的所有元素
//                 var currentElement = bookmark.NextSibling();
//                 var elementsToRemove = new List<OpenXmlElement>();
//
//                 while (currentElement != null && currentElement != bookmarkEnd)
//                 {
//                     elementsToRemove.Add(currentElement);
//                     currentElement = currentElement.NextSibling();
//                 }
//
//                 // 移除所有元素
//                 foreach (var element in elementsToRemove)
//                 {
//                     element.Remove();
//                 }
//
//                 // 確保書籤所在的段落仍然存在
//                 var paragraph = bookmark.Ancestors<Paragraph>().FirstOrDefault();
//                 if (paragraph != null && !paragraph.HasChildren)
//                 {
//                     paragraph.AppendChild(new Run());
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"清空書籤內容時發生錯誤 (Bookmark: {bookmarkName})", ex);
//             }
//         }
//
//         /// <summary>
//         /// 清空多個書籤中的所有內容
//         /// </summary>
//         /// <param name="bookmarkNames">書籤名稱清單</param>
//         public void ClearBookmarkContents(IEnumerable<string> bookmarkNames)
//         {
//             if (bookmarkNames == null)
//             {
//                 throw new ArgumentNullException(nameof(bookmarkNames));
//             }
//
//             var uniqueBookmarks = bookmarkNames.Distinct().ToList();
//
//             foreach (var bookmarkName in uniqueBookmarks)
//             {
//                 ClearBookmarkContent(bookmarkName);
//             }
//         }
//
//         /// <summary>
//         /// 刪除指定書籤所在的表格，不管表格中是否有其他書籤
//         /// </summary>
//         /// <param name="bookmarkName">書籤名稱</param>
//         public void DeleteTable(string bookmarkName)
//         {
//             ValidateBookmark(bookmarkName);
//
//             try
//             {
//                 var bookmark = _bookmarks[bookmarkName];
//                 var table = bookmark.Ancestors<Table>().FirstOrDefault();
//
//                 if (table == null)
//                 {
//                     throw new Exception($"書籤 {bookmarkName} 不在表格中");
//                 }
//
//                 // 找到表格的父層段落和表格的前一個段落（可能包含標題）
//                 var previousParagraph = table.PreviousSibling<Paragraph>();
//
//                 // 移除整個表格
//                 table.Remove();
//
//                 // 如果有前一個段落且看起來像是標題（只有一行文字），也將其移除
//                 if (previousParagraph != null &&
//                     previousParagraph.Descendants<Run>().Count() <= 2 && // 標題通常只有1-2個Run
//                     !previousParagraph.Descendants<BookmarkStart>().Any()) // 確保段落中沒有其他書籤
//                 {
//                     previousParagraph.Remove();
//                 }
//
//                 // 更新書籤對應表
//                 // 如果有其他書籤在這個表格中，它們的引用現在已經無效
//                 var bookmarksInTable = _bookmarks.Where(b => b.Value.Ancestors<Table>().Contains(table)).ToList();
//                 foreach (var kvp in bookmarksInTable)
//                 {
//                     _bookmarks.Remove(kvp.Key);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"刪除表格時發生錯誤 (Bookmark: {bookmarkName})", ex);
//             }
//         }
//
//         /// <summary>
//         /// 刪除多個書籤所在的表格
//         /// </summary>
//         /// <param name="bookmarkNames">書籤名稱清單</param>
//         public void DeleteTables(IEnumerable<string> bookmarkNames)
//         {
//             if (bookmarkNames == null)
//             {
//                 throw new ArgumentNullException(nameof(bookmarkNames));
//             }
//
//             var uniqueBookmarks = bookmarkNames.Distinct().ToList();
//
//             foreach (var bookmarkName in uniqueBookmarks)
//             {
//                 DeleteTable(bookmarkName);
//             }
//         }
//
//         #endregion
//
//         #region 目錄
//
//         /// <summary>
//         /// 更新現有的目錄
//         /// </summary>
//         public void UpdateExistingTableOfContents()
//         {
//             try
//             {
//                 var body = _document.MainDocumentPart?.Document.Body;
//                 if (body == null) return;
//
//                 // 找出所有段落中的 SDT (Structured Document Tag) 元素
//                 var sdtElements = body.Descendants<SdtElement>().ToList();
//
//                 foreach (var sdt in sdtElements)
//                 {
//                     // 檢查是否為目錄的 SDT
//                     var sdtProperties = sdt.GetFirstChild<SdtProperties>();
//                     var sdtId = sdtProperties?.GetFirstChild<SdtId>();
//
//                     if (sdtId != null)
//                     {
//                         // 更新目錄內容
//                         UpdateTocContent(sdt);
//                     }
//                 }
//
//                 // 儲存變更
//                 _document.MainDocumentPart?.Document.Save();
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("更新目錄時發生錯誤", ex);
//             }
//         }
//
//         private void UpdateTocContent(SdtElement tocSdt)
//         {
//             try
//             {
//                 var body = _document.MainDocumentPart?.Document.Body;
//                 if (body == null) return;
//
//                 // 建立新的目錄內容
//                 var newTocContent = new SdtContentBlock();
//
//                 // 尋找所有具有標題樣式的段落
//                 var headingParagraphs = body.Descendants<Paragraph>()
//                     .Where(HasHeadingStyle)
//                     .ToList();
//
//                 // 生成目錄項目
//                 foreach (var heading in headingParagraphs)
//                 {
//                     var level = GetHeadingLevel(heading);
//                     var text = GetParagraphText(heading);
//
//                     // 創建目錄項目
//                     var tocEntry = CreateTocEntry(text, level);
//                     newTocContent.AppendChild(tocEntry);
//                 }
//
//                 // 更新目錄的內容
//                 var sdtContent = tocSdt.GetFirstChild<SdtContentBlock>();
//                 if (sdtContent != null)
//                 {
//                     sdtContent.RemoveAllChildren();
//                     sdtContent.AppendChild(newTocContent);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("更新目錄內容時發生錯誤", ex);
//             }
//         }
//
//         private bool HasHeadingStyle(Paragraph paragraph)
//         {
//             var style = paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
//             return style != null && style.Value != null && style.Value.StartsWith("Heading");
//         }
//
//         private int GetHeadingLevel(Paragraph paragraph)
//         {
//             var style = paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
//             if (style == null) return 0;
//
//             // 從樣式名稱取得標題層級 (例如：Heading1 = 1)
//             int level;
//             if (style.Value != null && int.TryParse(style.Value.Replace("Heading", ""), out level))
//             {
//                 return level;
//             }
//
//             return 0;
//         }
//
//         private string GetParagraphText(Paragraph paragraph)
//         {
//             return string.Join("", paragraph.Descendants<Text>().Select(t => t.Text));
//         }
//
//         private Paragraph CreateTocEntry(string text, int level)
//         {
//             // 根據層級設定縮排
//             var indent = 720 * (level - 1); // 720 = 1/2 英吋
//
//             var paragraph = new Paragraph(
//                 new ParagraphProperties(
//                     new Indentation { Left = indent.ToString() }
//                 ),
//                 new Run(
//                     new Text(text)
//                 )
//             );
//
//             return paragraph;
//         }
//
//         #endregion
//
//         #region Markdown格式處理
//
//         /// <summary>
//         /// 替換文字內容（支援 Markdown 格式）
//         /// </summary>
//         /// <param name="replacements">替換的文字對應</param>
//         public void ReplaceTextMarkdown(Dictionary<string, string> replacements)
//         {
//             var body = _document.MainDocumentPart?.Document.Body;
//             if (body == null) return;
//
//             try
//             {
//                 foreach (var text in body.Descendants<Text>())
//                 {
//                     foreach (var replacement in replacements
//                                  .Where(replacement => text.Text.Contains(replacement.Key)))
//                     {
//                         var paragraph = text.Ancestors<Paragraph>().FirstOrDefault();
//                         if (paragraph == null) continue;
//
//                         var parent = paragraph.Parent;
//                         if (parent == null) continue;
//
//                         // 取得原始段落位置
//                         var currentParagraph = paragraph;
//
//                         // 分割行，保留空行
//                         var lines = text.Text
//                             .Replace(replacement.Key, replacement.Value)
//                             .Replace("\r\n", "\n")
//                             .Split('\n');
//
//                         var isFirstParagraph = true;
//
//                         foreach (var line in lines)
//                         {
//                             // 處理空行
//                             if (string.IsNullOrWhiteSpace(line))
//                             {
//                                 var emptyParagraph = new Paragraph();
//                                 parent.InsertAfter(emptyParagraph, currentParagraph);
//                                 currentParagraph = emptyParagraph;
//                                 continue;
//                             }
//
//                             // 如果是第一個段落，清除原有內容
//                             if (isFirstParagraph)
//                             {
//                                 currentParagraph.RemoveAllChildren();
//                             }
//                             else
//                             {
//                                 // 創建新段落
//                                 var newParagraph = new Paragraph();
//                                 parent.InsertAfter(newParagraph, currentParagraph);
//                                 currentParagraph = newParagraph;
//                             }
//
//                             // 處理當前行
//                             ProcessLine(line.TrimEnd(), currentParagraph);
//                             isFirstParagraph = false;
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("替換 Markdown 文字內容時發生錯誤", ex);
//             }
//         }
//
//         /// <summary>
//         /// Markdown 內容轉換為 Word 格式
//         /// </summary>
//         public void ReplaceBookmarkWithMarkdownContent(string bookmarkName, string content)
//         {
//             ValidateBookmark(bookmarkName);
//
//             try
//             {
//                 var bookmark = _bookmarks[bookmarkName];
//                 var paragraph = bookmark.Ancestors<Paragraph>().FirstOrDefault();
//                 if (paragraph == null) return;
//
//                 var parent = paragraph.Parent;
//
//                 // 先清除原始內容
//                 ClearBookmarkContent(bookmarkName);
//
//                 // 分割行，保留空行
//                 var lines = content.Replace("\r\n", "\n").Split('\n');
//                 var currentParagraph = paragraph;
//                 var isFirstParagraph = true;
//
//                 foreach (var line in lines)
//                 {
//                     // 處理空行
//                     if (string.IsNullOrWhiteSpace(line))
//                     {
//                         var emptyParagraph = new Paragraph();
//                         parent?.InsertAfter(emptyParagraph, currentParagraph);
//                         currentParagraph = emptyParagraph;
//                         continue;
//                     }
//
//                     // 創建新段落
//                     if (!isFirstParagraph)
//                     {
//                         var newParagraph = new Paragraph();
//                         parent?.InsertAfter(newParagraph, currentParagraph);
//                         currentParagraph = newParagraph;
//                     }
//
//                     // 處理當前行
//                     ProcessLine(line.TrimEnd(), currentParagraph);
//                     isFirstParagraph = false;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"替換 CKEditor 內容時發生錯誤 (書籤: {bookmarkName})", ex);
//             }
//         }
//
//         /// <summary>
//         /// 處理 CKEditor 內容
//         /// </summary>
//         private void ProcessMarkdownInCell(TableCell cell, string content)
//         {
//             try
//             {
//                 // 取得或建立段落
//                 var paragraph = cell.Elements<Paragraph>().FirstOrDefault();
//                 if (paragraph == null)
//                 {
//                     paragraph = new Paragraph();
//                     cell.AppendChild(paragraph);
//                 }
//
//                 // 清除現有內容
//                 paragraph.RemoveAllChildren();
//
//                 // 分割行
//                 var lines = content.Replace("\r\n", "\n").Split('\n');
//                 var currentParagraph = paragraph;
//                 var parent = cell;
//
//                 var isFirstParagraph = true;
//                 foreach (var line in lines)
//                 {
//                     // 處理空行
//                     if (string.IsNullOrWhiteSpace(line))
//                     {
//                         var emptyParagraph = new Paragraph();
//                         parent.InsertAfter(emptyParagraph, currentParagraph);
//                         currentParagraph = emptyParagraph;
//                         continue;
//                     }
//
//                     // 建立新段落
//                     if (!isFirstParagraph)
//                     {
//                         var newParagraph = new Paragraph();
//                         parent.InsertAfter(newParagraph, currentParagraph);
//                         currentParagraph = newParagraph;
//                     }
//
//                     ProcessLine(line.TrimEnd(), currentParagraph);
//                     isFirstParagraph = false;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception("處理儲存格 CKEditor 內容時發生錯誤", ex);
//             }
//         }
//
//         /// <summary>
//         /// 處理每一行文字
//         /// </summary>
//         private void ProcessLine(string line, Paragraph paragraph)
//         {
//             var text = line;
//
//             // 轉換項目符號和編號為純文字
//             if (text.StartsWith("* "))
//             {
//                 text = "• " + text.Substring(2);
//             }
//             else if (Regex.IsMatch(text, @"^\d+\.\s+"))
//             {
//                 // 保持原始數字格式
//             }
//
//             // 解析 Markdown 格式
//             var segments = ParseMarkdownSegments(text);
//
//             foreach (var segment in segments)
//             {
//                 var run = new Run();
//                 var runProperties = new RunProperties();
//
//                 // 套用文字樣式
//                 if (segment.Style.IsBold)
//                 {
//                     runProperties.AppendChild(new Bold());
//                 }
//
//                 if (segment.Style.IsItalic)
//                 {
//                     runProperties.AppendChild(new Italic());
//                 }
//
//                 if (segment.Style.IsLargeText)
//                 {
//                     runProperties.AppendChild(new FontSize { Val = "28" });
//                     runProperties.AppendChild(new Bold());
//                 }
//
//                 run.AppendChild(runProperties);
//
//                 // 處理超連結
//                 if (segment.Style.IsHyperlink)
//                 {
//                     var hyperlink = new Hyperlink();
//
//                     var hyperlinkStyle = new RunProperties(
//                         new RunStyle { Val = "Hyperlink" },
//                         new Underline { Val = UnderlineValues.Single }
//                     );
//
//                     run.PrependChild(hyperlinkStyle);
//                     run.AppendChild(new Text(segment.Text) { Space = SpaceProcessingModeValues.Preserve });
//                     hyperlink.AppendChild(run);
//
//                     if (_document.MainDocumentPart != null)
//                     {
//                         var hyperlinkRelation = _document.MainDocumentPart.AddHyperlinkRelationship(
//                             new Uri(segment.Style.HyperlinkUrl), true);
//                         hyperlink.Id = hyperlinkRelation.Id;
//                     }
//
//                     paragraph.AppendChild(hyperlink);
//                 }
//                 else
//                 {
//                     run.AppendChild(new Text(segment.Text) { Space = SpaceProcessingModeValues.Preserve });
//                     paragraph.AppendChild(run);
//                 }
//             }
//         }
//
//         /// <summary>
//         /// 解析 Markdown 格式文字
//         /// </summary>
//         private List<(string Text, TextStyle Style)> ParseMarkdownSegments(string text)
//         {
//             var segments = new List<(string Text, TextStyle Style)>();
//             var currentPos = 0;
//
//             // 檢查是否為中文數字標題格式
//             var isChineseNumberTitle = Regex.IsMatch(text, @"^\([\u4e00-\u9fa5]+\)");
//
//             while (currentPos < text.Length)
//             {
//                 // 檢查粗體+斜體組合
//                 if ((text.Substring(currentPos).StartsWith("_**") &&
//                      text.IndexOf("**_", currentPos, StringComparison.Ordinal) > -1) ||
//                     (text.Substring(currentPos).StartsWith("**_") &&
//                      text.IndexOf("_**", currentPos, StringComparison.Ordinal) > -1))
//                 {
//                     var endPos = text.IndexOf("**_", currentPos, StringComparison.Ordinal);
//                     if (endPos == -1) endPos = text.IndexOf("_**", currentPos, StringComparison.Ordinal);
//
//                     if (endPos != -1)
//                     {
//                         var combinedText = text.Substring(currentPos + 3, endPos - (currentPos + 3));
//                         segments.Add((combinedText, new TextStyle
//                         {
//                             IsBold = true,
//                             IsItalic = true,
//                             IsLargeText = isChineseNumberTitle
//                         }));
//                         currentPos = endPos + 3;
//                         continue;
//                     }
//                 }
//
//                 // 檢查粗體
//                 if (text.Substring(currentPos).StartsWith("**"))
//                 {
//                     var endPos = text.IndexOf("**", currentPos + 2, StringComparison.Ordinal);
//                     if (endPos != -1)
//                     {
//                         var boldText = text.Substring(currentPos + 2, endPos - (currentPos + 2));
//                         segments.Add((boldText, new TextStyle
//                         {
//                             IsBold = true,
//                             IsLargeText = isChineseNumberTitle
//                         }));
//                         currentPos = endPos + 2;
//                         continue;
//                     }
//                 }
//
//                 // 檢查斜體
//                 if (text.Substring(currentPos).StartsWith("_"))
//                 {
//                     var endPos = text.IndexOf("_", currentPos + 1, StringComparison.Ordinal);
//                     if (endPos != -1)
//                     {
//                         var italicText = text.Substring(currentPos + 1, endPos - (currentPos + 1));
//                         segments.Add((italicText, new TextStyle
//                         {
//                             IsItalic = true,
//                             IsLargeText = isChineseNumberTitle
//                         }));
//                         currentPos = endPos + 1;
//                         continue;
//                     }
//                 }
//
//                 // 檢查超連結
//                 if (text.Substring(currentPos).StartsWith("["))
//                 {
//                     var closeBracket = text.IndexOf("]", currentPos, StringComparison.Ordinal);
//                     var openParen = text.IndexOf("(", closeBracket, StringComparison.Ordinal);
//                     var closeParen = text.IndexOf(")", openParen, StringComparison.Ordinal);
//
//                     if (closeBracket != -1 && openParen != -1 && closeParen != -1)
//                     {
//                         var linkText = text.Substring(currentPos + 1, closeBracket - (currentPos + 1));
//                         var url = text.Substring(openParen + 1, closeParen - (openParen + 1));
//
//                         segments.Add((linkText, new TextStyle
//                         {
//                             IsHyperlink = true,
//                             HyperlinkUrl = url,
//                             IsLargeText = isChineseNumberTitle
//                         }));
//
//                         currentPos = closeParen + 1;
//                         continue;
//                     }
//                 }
//
//                 // 處理一般文字
//                 var nextSpecialChar = text.Length;
//                 foreach (var marker in new[] { "**", "_", "[", "_**", "**_" })
//                 {
//                     var pos = text.IndexOf(marker, currentPos, StringComparison.Ordinal);
//                     if (pos != -1 && pos < nextSpecialChar)
//                     {
//                         nextSpecialChar = pos;
//                     }
//                 }
//
//                 var plainText = text.Substring(currentPos, nextSpecialChar - currentPos);
//                 if (!string.IsNullOrEmpty(plainText))
//                 {
//                     segments.Add((plainText, new TextStyle { IsLargeText = isChineseNumberTitle }));
//                 }
//
//                 currentPos = nextSpecialChar;
//             }
//
//             return segments;
//         }
//
//         #endregion
//         
//         /// <summary>
//         /// 插入圖片到書籤位置
//         /// </summary>
//         /// <param name="bookmarkName">書籤名稱</param>
//         /// <param name="imagePath">圖片路徑</param>
//         /// <param name="width">寬度（點數，0為自動）</param>
//         /// <param name="height">高度（點數，0為自動）</param>
//         public void InsertImageAtBookmark(string bookmarkName, string imagePath, long width = 0, long height = 0)
//         {
//             ValidateBookmark(bookmarkName);
//
//             try
//             {
//                 // 檢查圖片是否存在
//                 if (!File.Exists(imagePath))
//                 {
//                     throw new FileNotFoundException($"找不到圖片檔案：{imagePath}");
//                 }
//
//                 var bookmark = _bookmarks[bookmarkName];
//                 var paragraph = bookmark.Ancestors<Paragraph>().FirstOrDefault();
//                 if (paragraph == null) return;
//
//                 // 先清除書籤內的內容
//                 ClearBookmarkContent(bookmarkName);
//
//                 // 取得圖片的擴展名，確定圖片類型
//                 var extension = Path.GetExtension(imagePath).ToLower();
//                 ImagePartType imageType;
//                 switch (extension)
//                 {
//                     case ".png":
//                         imageType = ImagePartType.Png;
//                         break;
//                     case ".jpeg":
//                         imageType = ImagePartType.Jpeg;
//                         break;
//                     case ".jpg":
//                         imageType = ImagePartType.Jpeg;
//                         break;
//                     case ".gif":
//                         imageType = ImagePartType.Gif;
//                         break;
//                     case ".bmp":
//                         imageType = ImagePartType.Bmp;
//                         break;
//                     case ".tiff":
//                         imageType = ImagePartType.Tiff;
//                         break;
//                     default:
//                         throw new NotSupportedException($"不支援的圖片格式：{extension}");
//                 }
//
//                 // 讀取圖片資料
//                 var imageBytes = File.ReadAllBytes(imagePath);
//
//                 // 取得原始圖片尺寸
//                 long originalWidth, originalHeight;
//                 using (var imageStream = new MemoryStream(imageBytes))
//                 using (var bitmap = new Bitmap(imageStream))
//                 {
//                     // 計算圖片尺寸
//                     originalWidth = bitmap.Width * 9525; // 轉換為 EMU 單位（1 英寸 = 914400 EMU）
//                     originalHeight = bitmap.Height * 9525;
//                 }
//
//                 // 如果沒有指定寬度或高度，則使用原始尺寸
//                 if (width <= 0 && height <= 0)
//                 {
//                     // 如果圖片太大，限制最大尺寸
//                     const long maxWidth = 6000000; // 約 630 點
//                     const long maxHeight = 4000000; // 約 420 點
//
//                     if (originalWidth > maxWidth)
//                     {
//                         var ratio = (double)maxWidth / originalWidth;
//                         originalWidth = maxWidth;
//                         originalHeight = (long)(originalHeight * ratio);
//                     }
//
//                     if (originalHeight > maxHeight)
//                     {
//                         var ratio = (double)maxHeight / originalHeight;
//                         originalHeight = maxHeight;
//                         originalWidth = (long)(originalWidth * ratio);
//                     }
//
//                     width = originalWidth;
//                     height = originalHeight;
//                 }
//                 else if (width <= 0)
//                 {
//                     // 只指定高度，按比例計算寬度
//                     var ratio = (double)height / originalHeight;
//                     width = (long)(originalWidth * ratio);
//                 }
//                 else if (height <= 0)
//                 {
//                     // 只指定寬度，按比例計算高度
//                     var ratio = (double)width / originalWidth;
//                     height = (long)(originalHeight * ratio);
//                 }
//
//                 // 轉換點數為 EMU 單位
//                 if (width > 0 && width < 10000)
//                     width *= 9525;
//                 if (height > 0 && height < 10000)
//                     height *= 9525;
//
//                 // 開始插入圖片
//                 var mainPart = _document.MainDocumentPart;
//                 if (mainPart == null) return;
//
//                 // 新增圖片到文件
//                 var imagePart = mainPart.AddImagePart(imageType);
//                 using (var stream = new MemoryStream(imageBytes))
//                 {
//                     imagePart.FeedData(stream);
//                 }
//
//                 // 計算圖片 ID
//                 var imageId = mainPart.GetIdOfPart(imagePart);
//
//                 // 創建 Drawing 元素
//                 var drawing = new Drawing(
//                     new DW.Inline(
//                         new DW.Extent { Cx = width, Cy = height },
//                         new DW.EffectExtent
//                         {
//                             LeftEdge = 0L,
//                             TopEdge = 0L,
//                             RightEdge = 0L,
//                             BottomEdge = 0L
//                         },
//                         new DW.DocProperties
//                         {
//                             Id = 1U,
//                             Name = Path.GetFileNameWithoutExtension(imagePath)
//                         },
//                         new DW.NonVisualGraphicFrameDrawingProperties(
//                             new A.GraphicFrameLocks { NoChangeAspect = true }),
//                         new A.Graphic(
//                             new A.GraphicData(
//                                 new PIC.Picture(
//                                     new PIC.NonVisualPictureProperties(
//                                         new PIC.NonVisualDrawingProperties()
//                                         {
//                                             Id = 0U,
//                                             Name = Path.GetFileName(imagePath)
//                                         },
//                                         new PIC.NonVisualPictureDrawingProperties()),
//                                     new PIC.BlipFill(
//                                         new A.Blip(
//                                             new A.BlipExtensionList(
//                                                 new A.BlipExtension
//                                                 {
//                                                     Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
//                                                 })
//                                         )
//                                         {
//                                             Embed = imageId,
//                                             CompressionState = A.BlipCompressionValues.Print
//                                         },
//                                         new A.Stretch(
//                                             new A.FillRectangle())),
//                                     new PIC.ShapeProperties(
//                                         new A.Transform2D(
//                                             new A.Offset { X = 0L, Y = 0L },
//                                             new A.Extents { Cx = width, Cy = height }),
//                                         new A.PresetGeometry(
//                                             new A.AdjustValueList()
//                                         )
//                                         { Preset = A.ShapeTypeValues.Rectangle }))
//                             )
//                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
//                     )
//                     {
//                         DistanceFromTop = 0U,
//                         DistanceFromBottom = 0U,
//                         DistanceFromLeft = 0U,
//                         DistanceFromRight = 0U,
//                         EditId = "50D07946"
//                     });
//
//                 // 在書籤位置後插入圖片
//                 var run = new Run(drawing);
//                 paragraph.InsertAfter(run, bookmark);
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception($"插入圖片時發生錯誤 (書籤: {bookmarkName})", ex);
//             }
//         }
//
//         /// <summary>
//         /// 驗證書籤是否存在
//         /// </summary>
//         /// <param name="bookmarkName"></param>
//         /// <exception cref="Exception"></exception>
//         public void ValidateBookmark(string bookmarkName)
//         {
//             if (!_bookmarks.ContainsKey(bookmarkName))
//             {
//                 throw new Exception($"找不到書籤: {bookmarkName}");
//             }
//         }
//
//         /// <summary>
//         /// 儲存變更
//         /// </summary>
//         public void Save()
//         {
//             _document.MainDocumentPart?.Document.Save();
//         }
//         
//         protected virtual void Dispose(bool disposing)
//         {
//             if (_disposed) return;
//             if (disposing)
//             {
//                 _document.Dispose();
//             }
//             _disposed = true;
//         }
//
//         public void Dispose()
//         {
//             Dispose(true);
//             GC.SuppressFinalize(this);
//         }
//         /// <summary>
//         /// 刪除多列table
//         /// </summary>
//         public void RemoveTableRows(string bookmarkName, int startRowIndex, int endRowIndex)
//         {
//             if (!_bookmarks.ContainsKey(bookmarkName))
//                 throw new ArgumentException($"Bookmark '{bookmarkName}' 不存在");
//
//             var bookmarkStart = _bookmarks[bookmarkName];
//
//             // 找到 bookmark 所在的 table
//             var table = bookmarkStart.Ancestors<Table>().FirstOrDefault();
//             if (table == null)
//                 throw new InvalidOperationException("找不到 bookmark 所在的表格");
//
//             var rows = table.Elements<TableRow>().ToList();
//             int rowCount = rows.Count;
//
//             if (startRowIndex < 0 || startRowIndex >= rowCount)
//                 throw new ArgumentOutOfRangeException(nameof(startRowIndex), "起始列索引超出範圍");
//             if (endRowIndex < 0 || endRowIndex >= rowCount)
//                 throw new ArgumentOutOfRangeException(nameof(endRowIndex), "結束列索引超出範圍");
//             if (startRowIndex > endRowIndex)
//                 throw new ArgumentException("起始列索引不能大於結束列索引");
//
//             // 反向刪除，避免索引錯亂
//             for (int i = endRowIndex; i >= startRowIndex; i--)
//             {
//                 rows[i].Remove();
//             }
//         }
//         
//         // /// <summary>
//         // /// 合併指定書籤所在表格中，水平合併（同一列、跨多欄）
//         // /// 合併第rowIndex列，第startCol欄~第endCol欄
//         // /// </summary>
//         //不太能使用(待修復)
//         public void MergeTableCellsHorizontallyAtBookmark(string bookmarkName, int rowIndex, int startCol, int endCol)
//         {
//             if (!_bookmarks.ContainsKey(bookmarkName))
//                 throw new ArgumentException($"Bookmark '{bookmarkName}' not found.");
//
//             var bookmark = _bookmarks[bookmarkName];
//             var table = bookmark.Ancestors<Table>().FirstOrDefault();
//             if (table == null)
//                 throw new InvalidOperationException("Bookmark is not inside a table.");
//
//             var rows = table.Elements<TableRow>().ToList();
//             if (rowIndex < 0 || rowIndex >= rows.Count)
//                 throw new ArgumentOutOfRangeException(nameof(rowIndex), "Invalid row index.");
//
//             var row = rows[rowIndex];
//             var cells = row.Elements<TableCell>().ToList();
//             if (startCol < 0 || endCol >= cells.Count || startCol > endCol)
//                 throw new ArgumentOutOfRangeException("Invalid column range.");
//
//             // 設定起始儲存格的 GridSpan
//             var firstCell = cells[startCol];
//             if (firstCell.TableCellProperties == null)
//                 firstCell.TableCellProperties = new TableCellProperties();
//             firstCell.TableCellProperties.GridSpan = new GridSpan() { Val = endCol - startCol + 1 };
//
//             // 移除其他儲存格：注意每次都要重新取得 Elements<TableCell>()，以避免 index 錯位
//             for (int col = endCol; col > startCol; col--)
//             {
//                 var cellToRemove = row.Elements<TableCell>().ElementAt(col);
//                 row.RemoveChild(cellToRemove);
//             }
//         }
//
//
//
//         /// <summary>
//         /// 合併指定書籤所在表格中，垂直合併（同一欄、跨多列）
//         /// 合併第rowIndex欄，第startCol列~第endCol列
//         /// </summary>
//         /// 合併後會把被合併的值吃掉。
//         public void MergeTableCellsVerticallyAtBookmark(string bookmarkName, int colIndex, int startRow, int endRow)
//         {
//             if (!_bookmarks.ContainsKey(bookmarkName))
//                 throw new ArgumentException($"Bookmark '{bookmarkName}' not found.");
//
//             var bookmark = _bookmarks[bookmarkName];
//             var table = bookmark.Ancestors<Table>().FirstOrDefault();
//             if (table == null)
//                 throw new InvalidOperationException("Bookmark is not inside a table.");
//
//             var rows = table.Elements<TableRow>().ToList();
//             if (startRow < 0 || endRow >= rows.Count || startRow > endRow)
//                 throw new ArgumentOutOfRangeException("Invalid row range.");
//
//             for (int row = startRow; row <= endRow; row++)
//             {
//                 var cells = rows[row].Elements<TableCell>().ToList();
//                 if (colIndex < 0 || colIndex >= cells.Count)
//                     throw new ArgumentOutOfRangeException("Invalid column index.");
//
//                 var cell = cells[colIndex];
//                 if (cell.TableCellProperties == null)
//                     cell.TableCellProperties = new TableCellProperties();
//
//                 if (row == startRow)
//                 {
//                     // 開始儲存格
//                     cell.TableCellProperties.VerticalMerge = new VerticalMerge() { Val = MergedCellValues.Restart };
//                 }
//                 else
//                 {
//                     // 合併中的儲存格
//                     cell.TableCellProperties.VerticalMerge = new VerticalMerge() { Val = MergedCellValues.Continue };
//                 }
//             }
//         }
//
//     }
// }
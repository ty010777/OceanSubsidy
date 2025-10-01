using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GS.OCA_OceanSubsidy.Operation.OSI.OpenXml
{
    /* * 掛載 Nuget 套件
     * <package id="DocumentFormat.OpenXml" version="2.15.0" targetFramework="net48" />
     * <package id="HtmlAgilityPack" version="1.12.3" targetFramework="net48" />
     * */

    /// <summary>
    /// 處理 OpenXML Word 文件的輔助類別
    /// </summary>
    public class OpenXmlHelper : IDisposable
    {
        private WordprocessingDocument Word;
        private Dictionary<string, OpenXmlElement> Bookmarks;

        public OpenXmlHelper(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("File Not Found", FilePath);
            }
            Word = WordprocessingDocument.Open(FilePath, true);
            FilterAllBookmarks();
        }

        public OpenXmlHelper(System.IO.Stream stream)
        {
            Word = WordprocessingDocument.Open(stream, true);
            FilterAllBookmarks();
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            CloseAsSave();
        }

        /// <summary>
        /// 取得所有書籤元素，並存儲在字典中
        /// </summary>
        private void FilterAllBookmarks()
        {
            if (Word?.MainDocumentPart?.Document == null) return;

            Bookmarks = LoadBookmarkElement(Word.MainDocumentPart.Document)
                                .ToDictionary(x => x.GetAttributes().FirstOrDefault(attr => attr.LocalName.ToLower() == "name").Value, x => x);
        }

        /// <summary>
        /// 遞迴找出所有書籤元素
        /// </summary>
        private List<OpenXmlElement> LoadBookmarkElement(OpenXmlElement element)
        {
            List<OpenXmlElement> list = new List<OpenXmlElement>();

            // 檢查當前元素是否為書籤
            if (element.LocalName.ToLower() == "bookmarkstart" &&
                element.GetAttributes().Any(x => x.LocalName.ToLower() == "name"))
            {
                list.Add(element);
            }

            // 遞迴尋找子元素
            foreach (var child in element.ChildElements)
            {
                list.AddRange(LoadBookmarkElement(child));
            }

            return list;
        }

        /// <summary>
        /// 取出 BookmarkStart 和 BookmarkEnd 中間所有的兄弟元素
        /// </summary>
        private List<OpenXmlElement> GetBookmarkSiblingElements(OpenXmlElement bookmarkStartElement)
        {
            string bookmarkEndLocalName = "bookmarkend";
            var id = bookmarkStartElement.GetAttributes().FirstOrDefault(at => at.LocalName.ToLower() == "id").Value;

            var siblingElements = new List<OpenXmlElement>();
            var currentElement = bookmarkStartElement.NextSibling();

            while (currentElement != null)
            {
                // 如果找到對應的結束書籤，則跳出迴圈
                if (currentElement.LocalName.ToLower() == bookmarkEndLocalName &&
                    currentElement.GetAttributes().FirstOrDefault(at => at.LocalName.ToLower() == "id").Value == id)
                {
                    break;
                }

                siblingElements.Add(currentElement);
                currentElement = currentElement.NextSibling();
            }

            return siblingElements;
        }

        /// <summary>
        /// 異動書籤內容
        /// </summary>
        public bool ChangeBookmarkValue(string bookmarkName, string value)
        {
            bool hasRun = false;
            if (Bookmarks == null || !Bookmarks.ContainsKey(bookmarkName)) return false;

            var bookmarkStartElement = Bookmarks[bookmarkName];
            var siblingElements = GetBookmarkSiblingElements(bookmarkStartElement);

            // 尋找第一個可寫入的 Run 元素
            var firstRun = siblingElements.FirstOrDefault(x => x is Run) as Run;

            // 如果沒有 Run 元素，則在書籤後新增一個
            if (firstRun == null)
            {
                firstRun = new Run();
                bookmarkStartElement.InsertAfterSelf(firstRun);
            }

            // 清空所有兄弟元素，只留下第一個 Run
            foreach (var se in siblingElements)
            {
                se.Remove();
            }

            // 處理特殊字元或普通文字
            if (value.Length == 4 && value.StartsWith("F0"))
            {
                // 使用 ASCII Code (Wingdings)
                var sym = firstRun.ChildElements.FirstOrDefault(x => x is Symbol) as Symbol;
                if (sym != null) sym.Remove();
                // 新的、可行的語法，直接設定 XML 屬性：
                var newSymbol = new Symbol();
                var ns = newSymbol.LookupNamespace("http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                newSymbol.SetAttribute(new OpenXmlAttribute(ns, "font", null, "Wingdings"));
                newSymbol.SetAttribute(new OpenXmlAttribute(ns, "char", null, value));
                firstRun.AppendChild(newSymbol);
            }
            else
            {
                // 處理普通文字
                var text = firstRun.ChildElements.FirstOrDefault(x => x is Text) as Text;
                if (text == null)
                {
                    firstRun.AppendChild(new Text(value));
                }
                else
                {
                    text.Text = value;
                }
            }

            hasRun = true;
            return hasRun;
        }

        public void CloseAsSave()
        {
            Word?.MainDocumentPart?.Document?.Save();
            Word?.Dispose();
        }

        /// <summary>
        /// 在文件中替換 XML 佔位符（已修正為更通用的方法）
        /// </summary>
        private void ReplaceXmlPlaceholder(WordprocessingDocument document, string placeholder, string xmlContent)
        {
            var body = document.MainDocumentPart.Document.Body;
            var paragraphs = body.Descendants<Paragraph>().Where(p => p.InnerText.Contains(placeholder)).ToList();

            foreach (var paragraph in paragraphs)
            {
                // 這裡的 XML 解析可能會出錯，建議使用 OpenXML 類別來構建
                try
                {
                    XElement xmlElement = XElement.Parse(xmlContent);
                    OpenXmlElement newContent = ConvertXElementToOpenXmlElement(xmlElement);

                    // 確保父元素可以接受這個新內容
                    if (newContent != null)
                    {
                        paragraph.InsertAfterSelf(newContent);
                        paragraph.Remove();
                    }
                }
                catch (System.Xml.XmlException)
                {
                    // 處理 XML 解析錯誤
                    continue;
                }
            }
        }

        /// <summary>
        /// 輔助方法：將 XElement 轉換為 OpenXmlElement
        /// </summary>
        private static OpenXmlElement ConvertXElementToOpenXmlElement(XElement element)
        {
            // 這是將 XElement 轉換為 OpenXmlElement 的簡單範例，
            // 實際應用中可能需要更複雜的邏輯
            if (element.Name.LocalName == "p")
            {
                var newParagraph = new Paragraph();
                foreach (var child in element.Elements())
                {
                    if (child.Name.LocalName == "r")
                    {
                        var newRun = new Run();
                        if (child.Element("t") != null)
                        {
                            newRun.Append(new Text(child.Element("t").Value));
                        }
                        newParagraph.Append(newRun);
                    }
                }
                return newParagraph;
            }
            return null;
        }

        /// <summary>
        /// 生成 Word 文件，替換單一值和重複區塊
        /// </summary>
        public void GenerateWord(Dictionary<string, string> replacements, List<Dictionary<string, string>> repeatData)
        {
            var body = Word.MainDocumentPart.Document.Body;

            // 1. 替換單一值
            foreach (var text in body.Descendants<Text>())
            {
                foreach (var key in replacements.Keys)
                {
                    if (text.Text.Contains(key))
                    {
                        var value = replacements[key];

                        // 處理多行文字(包含換行符號)
                        if (value.Contains("\n"))
                        {
                            var run = text.Parent as Run;
                            var para = run?.Parent as Paragraph;
                            if (para == null) continue;

                            // 先移除原本的 text
                            text.Text = text.Text.Replace(key, "");

                            // 插入多行（Run+Break）
                            var lines = value.Split(new[] { "\n" }, StringSplitOptions.None);
                            bool first = true;
                            foreach (var line in lines)
                            {
                                if (!first)
                                    para.AppendChild(new Run(new Break()));
                                para.AppendChild(new Run(new Text(line)));
                                first = false;
                            }
                        }
                        else
                        {
                            // 沒有換行，直接用一行 replace
                            text.Text = text.Text.Replace(key, value);
                        }
                    }
                }
            }

            // 2. 處理重複區塊
            var repeatStart = body.Descendants<Paragraph>().FirstOrDefault(p => p.InnerText.Contains("{{RepeatSectionStart}}"));
            var repeatEnd = body.Descendants<Paragraph>().FirstOrDefault(p => p.InnerText.Contains("{{RepeatSectionEnd}}"));

            if (repeatStart != null && repeatEnd != null)
            {
                var parent = repeatStart.Parent;
                var templateElements = new List<OpenXmlElement>();
                var currentElement = repeatStart.NextSibling();

                while (currentElement != null && currentElement != repeatEnd)
                {
                    templateElements.Add(currentElement.CloneNode(true));
                    currentElement = currentElement.NextSibling();
                }

                foreach (var data in repeatData)
                {
                    foreach (var template in templateElements)
                    {
                        var clonedElement = template.CloneNode(true);
                        foreach (var text in clonedElement.Descendants<Text>())
                        {
                            foreach (var key in data.Keys)
                            {
                                if (text.Text.Contains(key))
                                {
                                    text.Text = text.Text.Replace(key, data[key]);
                                }
                            }
                        }
                        parent.InsertBefore(clonedElement, repeatEnd);
                    }
                }

                // 移除原始的重複區塊
                currentElement = repeatStart.NextSibling();
                while (currentElement != null && currentElement != repeatEnd)
                {
                    var nextElement = currentElement.NextSibling();
                    currentElement.Remove();
                    currentElement = nextElement;
                }
                repeatStart.Remove();
                repeatEnd.Remove();
            }

            Word.MainDocumentPart.Document.Save();
        }
        public void GenerateWords(string sectionStart, string sectionEnd, List<Dictionary<string, string>> repeatData)
        {
            var body = Word.MainDocumentPart.Document.Body;

            var repeatStart = body.Descendants<Paragraph>().FirstOrDefault(p => p.InnerText.Contains(sectionStart));
            var repeatEnd = body.Descendants<Paragraph>().FirstOrDefault(p => p.InnerText.Contains(sectionEnd));

            if (repeatStart != null && repeatEnd != null)
            {
                var parent = repeatStart.Parent;

                foreach (var data in repeatData)
                {
                    var currentElement = repeatStart.NextSibling();

                    while (currentElement != null && currentElement != repeatEnd)
                    {
                        var clonedElement = currentElement.CloneNode(true);
                        foreach (var text in clonedElement.Descendants<Text>())
                        {
                            foreach (var key in data.Keys)
                            {
                                if (text.Text.Contains(key))
                                {
                                    text.Text = text.Text.Replace(key, data[key]);
                                }
                            }
                        }
                        parent.InsertBefore(clonedElement, repeatStart);
                        currentElement = currentElement.NextSibling();
                    }
                }

                // 移除原始的重複區塊
                var elementToRemove = repeatStart.NextSibling();
                while (elementToRemove != null && elementToRemove != repeatEnd)
                {
                    var nextElement = elementToRemove.NextSibling();
                    parent.RemoveChild(elementToRemove);
                    elementToRemove = nextElement;
                }

                // 移除開始和結束標記
                parent.RemoveChild(repeatStart);
                parent.RemoveChild(repeatEnd);
            }

            Word.MainDocumentPart.Document.Save();
        }

        /// <summary>
        /// 處理帶有跨 Run 的替換文字 (已優化，但可能仍然會有複雜問題)
        /// </summary>
        public void GenerateWord2(Dictionary<string, string> replacements, List<Dictionary<string, string>> repeatData)
        {
            var body = Word.MainDocumentPart.Document.Body;
            var paras = body.Descendants<Paragraph>();

            foreach (var para in paras)
            {
                foreach (var replacement in replacements)
                {
                    string paragraphText = para.InnerText;

                    if (paragraphText.Contains(replacement.Key))
                    {
                        // 在段落層級處理替換，然後重新構建 Run
                        // 這個方法比逐個 Run 替換更可靠
                        string newText = paragraphText.Replace(replacement.Key, replacement.Value);

                        // 清除舊的 Run 元素
                        para.RemoveAllChildren<Run>();

                        // 創建一個新的 Run 來承載新文字
                        para.AppendChild(new Run(new Text(newText)));
                    }
                }
            }

            // 處理重複區塊，此處與 GenerateWord 方法相同
            GenerateWord(new Dictionary<string, string>(), repeatData);

            Word.MainDocumentPart.Document.Save();
        }

        /// <summary>
        /// 將 HTML 表格內容插入到 Word 文件中的指定位置，支援 rowspan 與 colspan 欄位合併，並呈現部分 CSS 樣式
        /// </summary>
        public void InsertHtmlAsTable(string placeholder, string htmlContent)
        {
            // 使用 HtmlAgilityPack 解析 HTML
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // 取得 HTML 中的第一個表格
            var htmlTable = htmlDoc.DocumentNode.SelectSingleNode("//table");
            if (htmlTable == null) return;

            // 創建 Open XML 表格
            Table wordTable = new Table();

            // 處理表格屬性 (這裡簡化為只設定基本屬性)
            TableProperties tableProps = new TableProperties(
                // width 100%
                new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" },
                new TableBorders(
                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 },
                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 10 }
                ),
                // 新增儲存格 padding
                new TableCellMarginDefault(
                    new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                    new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                    new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                    new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa }
                )
            );
            wordTable.AppendChild(tableProps);

            // 先建立一個記錄每個 cell 是否被 rowspan 佔用的結構
            var htmlRows = htmlTable.SelectNodes("tr").ToList();
            int rowCount = htmlRows.Count;
            // 預估最大欄數（處理 colspan 時可再擴充）
            int maxCol = htmlRows.Max(tr => tr.SelectNodes("td|th").Count) * 2;
            // 0:無, >0:還剩幾行要 continue
            int[,] rowspanMap = new int[rowCount, maxCol];

            // 計算最大格數（合併後的 cell 數）
            int maxCellCount = 0;
            foreach (var row in htmlRows)
            {
                int count = 0;
                foreach (var cell in row.SelectNodes("td|th"))
                {
                    int colspan = 1;
                    int.TryParse(cell.GetAttributeValue("colspan", "1"), out colspan);
                    count += colspan;
                }
                if (count > maxCellCount) maxCellCount = count;
            }

            for (int r = 0; r < rowCount; r++)
            {
                var htmlRow = htmlRows[r];
                TableRow wordRow = new TableRow();
                var htmlCells = htmlRow.SelectNodes("td|th");

                int c = 0, i = 0;
                int cellCount = 0;
                while (cellCount < maxCellCount)
                {
                    TableCell wordCell = new TableCell();
                    TableCellProperties cellProps = new TableCellProperties();

                    if (rowspanMap[r, c] == 1)
                    {
                        cellProps.Append(new VerticalMerge() { Val = MergedCellValues.Continue });
                        wordCell.Append(cellProps);
                        wordCell.Append(new Paragraph());
                        wordRow.Append(wordCell);
                        c++;
                        cellCount++;
                        continue;
                    }

                    if (i >= htmlCells.Count)
                    {
                        // 補空 cell
                        wordCell.Append(new Paragraph());
                        wordRow.Append(wordCell);
                        c++;
                        cellCount++;
                        continue;
                    }

                    var htmlCell = htmlCells[i];
                    i++;

                    // 解析 rowspan
                    int rowspan = 1;
                    int.TryParse(htmlCell.GetAttributeValue("rowspan", "1"), out rowspan);

                    if (rowspan > 1)
                    {
                        cellProps.Append(new VerticalMerge() { Val = MergedCellValues.Restart });
                        // 標記後續 row 這一欄要 continue
                        for (int rr = 1; rr < rowspan; rr++)
                            rowspanMap[r + rr, c] = 1;
                    }

                    // 解析 colspan
                    int colspan = 1;
                    int.TryParse(htmlCell.GetAttributeValue("colspan", "1"), out colspan);
                    if (colspan > 1)
                    {
                        cellProps.Append(new GridSpan() { Val = colspan });
                    }

                    // 創建段落並插入文字
                    Paragraph p = new Paragraph();

                    // 解析 style
                    var style = htmlCell.GetAttributeValue("style", "").ToLower();

                    // 水平置中
                    if (style.Replace(" ", "").Contains("text-align:center"))
                    {
                        p.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    }

                    // 垂直置中
                    if (style.Replace(" ", "").Contains("vertical-align:middle"))
                    {
                        cellProps.Append(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });
                    }

                    // 處理 width:xx%
                    var widthMatch = System.Text.RegularExpressions.Regex.Match(style, @"width\s*:\s*(\d+)%");
                    if (widthMatch.Success)
                    {
                        int percent = int.Parse(widthMatch.Groups[1].Value);
                        // Word 百分比單位是 1/50%，所以要乘 50
                        cellProps.Append(new TableCellWidth()
                        {
                            Type = TableWidthUnitValues.Pct,
                            Width = (percent * 50).ToString()
                        });
                    }

                    // 處理背景色 background-color: #xxxxxx;
                    var bgMatch = System.Text.RegularExpressions.Regex.Match(style, @"background-color\s*:\s*#?([0-9a-f]{6})");
                    if (bgMatch.Success)
                    {
                        string color = bgMatch.Groups[1].Value;
                        cellProps.Append(new Shading()
                        {
                            Val = ShadingPatternValues.Clear,
                            Color = "auto",
                            Fill = color
                        });
                    }

                    // 解析 cell 內容，處理 <br> 與 \n
                    foreach (var node in htmlCell.ChildNodes)
                    {
                        if (node.Name == "br")
                        {
                            p.Append(new Run(new Break()));
                        }
                        else if (node.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                        {
                            // 處理 \n
                            var lines = node.InnerText.Split('\n');
                            for (int j = 0; j < lines.Length; j++)
                            {
                                if (j > 0) p.Append(new Run(new Break()));
                                var innerRun = new Run(new Text(lines[j]));

                                // 設定字型大小 12pt (24 half-points)
                                innerRun.PrependChild(new RunProperties(new FontSize() { Val = "24" }));
                                p.Append(innerRun);
                            }
                        }
                        else
                        {
                            // 其他節點（如 span），遞迴處理
                            var innerRun = new Run(new Text(node.InnerText));

                            // 設定字型大小 12pt (24 half-points)
                            innerRun.PrependChild(new RunProperties(new FontSize() { Val = "24" }));
                            p.Append(innerRun);
                        }
                    }

                    wordCell.Append(cellProps);
                    wordCell.Append(p);
                    wordRow.Append(wordCell);

                    c += colspan;
                    cellCount += colspan;
                }

                wordTable.Append(wordRow);
            }

            var body = Word.MainDocumentPart.Document.Body;

            // 找到包含 placeholder 的 Text 節點
            var textNode = body.Descendants<Text>().FirstOrDefault(t => t.Text.Contains(placeholder));
            if (textNode == null) return;

            // 找到 Text 的 Paragraph
            var run = textNode.Parent as Run;
            var para = run?.Parent as Paragraph;
            if (para == null) return;

            // 移除 placeholder 文字
            textNode.Text = textNode.Text.Replace(placeholder, "");

            // 將轉換後的元素插入到 placeholder 位置
            para.InsertAfterSelf(wordTable);

            // 移除原本的段落，避免多出一個 enter
            para.Remove();

            Word.MainDocumentPart.Document.Save();
        }

        /// <summary>
        /// 將 Word 表格中的對應欄位字串替換為資料內容，支援巢狀表格與合併欄位之多筆資料列
        /// <summary>
        public void InsertSubTableRows(string[] columnKeys, List<Dictionary<string, string>> repeatData)
        {
            var tables = Word.MainDocumentPart.Document.Body.Descendants<Table>().ToList();

            for (int i = 0; i < tables.Count; i++)
            {
                var table = tables[i];
                if (table == null) return;

                // 找到範例 row（cell 內容有 cellKeys[0]）
                var targetRow = table.Descendants<TableRow>()
                    .FirstOrDefault(row => row.Elements<TableCell>()
                        .Any(cell => cell.InnerText.Contains(columnKeys[0])));

                if (targetRow == null) continue;

                // 取得範例 row 的所有 cell 的 placeholder key（依 cell 順序）
                var cellKeys = targetRow.Elements<TableCell>()
                    .Select(cell => columnKeys.FirstOrDefault(key => cell.InnerText.Contains(key)))
                    .ToList();

                if (repeatData == null || repeatData.Count == 0)
                {
                    // 無資料時插入一個空 row
                    TableRow newRow = (TableRow)targetRow.Clone();
                    var cells = newRow.Elements<TableCell>().ToList();
                    for (int k = 0; k < cells.Count; k++)
                    {
                        cells[k].RemoveAllChildren<Paragraph>();
                        cells[k].AppendChild(new Paragraph(new Run(new Text(""))));
                    }
                    table.InsertBefore(newRow, targetRow);
                }
                else
                {
                    foreach (var data in repeatData)
                    {
                        TableRow newRow = (TableRow)targetRow.Clone();
                        var cells = newRow.Elements<TableCell>().ToList();

                        for (int k = 0; k < cells.Count; k++)
                        {
                            var cell = cells[k];
                            cell.RemoveAllChildren<Paragraph>();

                            string key = cellKeys[k];
                            string value = (key != null && data.ContainsKey(key)) ? data[key] : "";

                            // 解析 cell 內容，處理 與 \n
                            var p = new Paragraph();
                            var lines = value.Split(new[] { "\n" }, StringSplitOptions.None);

                            for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
                            {
                                if (lineIdx > 0)
                                    p.AppendChild(new Run(new Break()));
                                var innerRun = new Run(new Text(lines[lineIdx]));

                                // 設定字型大小 12pt (24 half-points)
                                innerRun.PrependChild(new RunProperties(new FontSize() { Val = "24" }));
                                p.Append(innerRun);
                            }

                            cell.AppendChild(p);
                        }

                        table.InsertBefore(newRow, targetRow);
                    }
                }

                // 移除原本的 placeholder row
                table.RemoveChild(targetRow);
                Word.MainDocumentPart.Document.Save();
            }
        }

        /// <summary>
        /// 替換佔位符為多行文字 (每行換行)
        /// </summary>
        public void ReplacePlaceholderWithLines(string placeholder, IEnumerable<string> lines)
        {
            var body = Word.MainDocumentPart.Document.Body;
            var texts = body.Descendants<Text>().Where(t => t.Text.Contains(placeholder)).ToList();

            foreach (var text in texts)
            {
                var parentRun = text.Parent as Run;
                var parentPara = parentRun?.Parent as Paragraph;
                if (parentPara == null) continue;

                // 移除原本的文字
                text.Text = "";

                bool first = true;
                foreach (var line in lines)
                {
                    if (!first)
                        parentPara.AppendChild(new Run(new Break())); // 加換行
                    parentPara.AppendChild(new Run(new Text(line)));
                    first = false;
                }
            }

            Word.MainDocumentPart.Document.Save();
        }
          /// <summary>
        /// 根據書籤名稱找到表格，並向下延伸指定數量的新行
        /// </summary>
        /// <param name="bookmarkName">表格書籤名稱</param>
        /// <param name="addRowCount">要新增的行數</param>
        /// <returns>延伸成功返回 true，否則返回 false</returns>
        public bool SetTableRowCount(string bookmarkName, int addRowCount)
        {
            if (Bookmarks == null || !Bookmarks.ContainsKey(bookmarkName) || addRowCount < 0)
                return false;

            var bookmarkElement = Bookmarks[bookmarkName];

            // 找到包含書籤的表格
            var table = FindParentTable(bookmarkElement);
            if (table == null) return false;

            var currentRows = table.Elements<TableRow>().ToList();
            if (currentRows.Count == 0) return false;

            // 使用第一行（標題行）作為模板來複製結構
            var templateRow = currentRows[0];

            // 新增指定數量的行
            for (int i = 0; i < addRowCount; i++)
            {
                var newRow = (TableRow)templateRow.CloneNode(true);

                // 清空新行的內容，保持格式
                foreach (var cell in newRow.Elements<TableCell>())
                {
                    foreach (var paragraph in cell.Elements<Paragraph>())
                    {
                        paragraph.RemoveAllChildren<Run>();
                        paragraph.AppendChild(new Run(new Text("")));
                    }
                }

                table.AppendChild(newRow);
            }

            return true;
        }

        /// <summary>
        /// 根據書籤名稱找到表格，並設定指定位置的單元格內容
        /// </summary>
        /// <param name="bookmarkName">表格書籤名稱</param>
        /// <param name="rowIndex">行索引（0為第一行）</param>
        /// <param name="columnIndex">列索引（0為第一列）</param>
        /// <param name="content">要輸入的內容</param>
        /// <returns>設定成功返回 true，否則返回 false</returns>
        public bool SetTableCellValue(string bookmarkName, int rowIndex, int columnIndex, string content)
        {
            if (Bookmarks == null || !Bookmarks.ContainsKey(bookmarkName) ||
                rowIndex < 0 || columnIndex < 0 || string.IsNullOrEmpty(content))
                return false;

            var bookmarkElement = Bookmarks[bookmarkName];

            // 找到包含書籤的表格
            var table = FindParentTable(bookmarkElement);
            if (table == null) return false;

            var rows = table.Elements<TableRow>().ToList();
            if (rowIndex >= rows.Count) return false;

            var targetRow = rows[rowIndex];
            var cells = targetRow.Elements<TableCell>().ToList();
            if (columnIndex >= cells.Count) return false;

            var targetCell = cells[columnIndex];
            var paragraph = targetCell.Elements<Paragraph>().FirstOrDefault();

            if (paragraph == null)
            {
                paragraph = new Paragraph();
                targetCell.AppendChild(paragraph);
            }

            // 清空單元格內容
            paragraph.RemoveAllChildren<Run>();

            // 插入新內容
            paragraph.AppendChild(new Run(new Text(content)));

            return true;
        }

        /// <summary>
        /// 輔助方法：找到包含指定元素的表格
        /// </summary>
        private Table FindParentTable(OpenXmlElement element)
        {
            var currentElement = element;
            while (currentElement != null)
            {
                if (currentElement is Table table)
                    return table;

                currentElement = currentElement.Parent;
            }
            return null;
        }
    }
}

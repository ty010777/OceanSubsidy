using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GS.OCA_OceanSubsidy.Operation.OSI.OpenXml
{
    /* * 掛載 Nuget 套件
     * <PackageReference Include="DocumentFormat.OpenXml" Version="2.15.0" />
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
                        text.Text = text.Text.Replace(key, replacements[key]);
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

        public void InsertTableRows(List<List<List<string>>> datas)
        {
            var mainPart = Word.MainDocumentPart;

            for (int i = 0; i < datas.Count; i++)
            {
                var table = mainPart.Document.Body.Descendants<Table>().ElementAtOrDefault(i);
                if (table == null) continue;

                var headerRow = table.Elements<TableRow>().FirstOrDefault();
                if (headerRow == null) continue;

                var dataRows = datas[i];

                foreach (var rowData in dataRows)
                {
                    TableRow newRow = (TableRow)headerRow.Clone();
                    var cells = newRow.Elements<TableCell>().ToList();

                    for (int k = 0; k < rowData.Count && k < cells.Count; k++)
                    {
                        var cell = cells[k];
                        var cellParagraph = cell.Elements<Paragraph>().FirstOrDefault() ?? cell.AppendChild(new Paragraph());

                        // 清空單元格內所有內容
                        cellParagraph.RemoveAllChildren<Run>();

                        // 插入新 Run
                        cellParagraph.AppendChild(new Run(new Text(rowData[k])));
                    }

                    table.AppendChild(newRow);
                }
            }

            mainPart.Document.Save();
        }
    }
}

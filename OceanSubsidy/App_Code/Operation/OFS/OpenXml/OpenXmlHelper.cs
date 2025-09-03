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

        public void CloseAsSave()
        {
            //Word.Save();
            //Word.Close();
            Word.Dispose();
        }

        /// <summary>
        ///  找出所有書籤 Element
        /// </summary>
        /// <param name="Element"></param>
        /// <returns></returns>
        private List<OpenXmlElement> LoadBookmarkElement(OpenXmlElement Element)
        {
            string BookmarkLocalName = "bookmarkstart";
            string[] RequiredAttr = { "name" };
            List<OpenXmlElement> list = new List<OpenXmlElement>();
            if (Element.HasChildren)
            {
                foreach (var c in Element.ChildElements)
                {
                    list.AddRange(LoadBookmarkElement(c));
                }
            }

            if (Element.LocalName.ToLower() == BookmarkLocalName
                    && Element.GetAttributes().Any(x => RequiredAttr.Contains(x.LocalName.ToLower()))
                )
                list.Add(Element);


            return list;
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

        /// <summary>
        /// 生成 Word 文件，替換指定區段的重複內容
        /// </summary>
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
        /// 在含有 "合併欄位的表格" 中插入多行資料
        /// </summary>
        public void InsertSubTableRows(List<Dictionary<string, string>> subsidies, string[] columnKeys)
        {
            var mainPart = Word.MainDocumentPart;
            var table = mainPart.Document.Body.Descendants<Table>().FirstOrDefault();
            if (table == null) return;

            // 找到第一個 cell 內容包含 columnKeys[0] 的 row
            var targetRow = table.Descendants<TableRow>()
                .FirstOrDefault(row => row.Elements<TableCell>()
                    .Any(cell => cell.InnerText.Contains(columnKeys[0])));
            if (targetRow == null) return;

            // 取得所有 cell 的 placeholder key（依 cell 順序）
            var cellKeys = targetRow.Elements<TableCell>()
                .Select(cell => columnKeys.FirstOrDefault(key => cell.InnerText.Contains(key)))
                .ToList();

            foreach (var subsidy in subsidies)
            {
                TableRow newRow = (TableRow)targetRow.Clone();
                var cells = newRow.Elements<TableCell>().ToList();

                for (int k = 0; k < cells.Count; k++)
                {
                    var cell = cells[k];
                    cell.RemoveAllChildren<Paragraph>();
                    string key = cellKeys[k];
                    string value = (key != null && subsidy.ContainsKey(key)) ? subsidy[key] : "";
                    var para = new Paragraph(new Run(new Text(value)));
                    cell.AppendChild(para);
                }

                table.InsertBefore(newRow, targetRow);
            }

            // 移除原本的 placeholder row
            table.RemoveChild(targetRow);

            mainPart.Document.Save();
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

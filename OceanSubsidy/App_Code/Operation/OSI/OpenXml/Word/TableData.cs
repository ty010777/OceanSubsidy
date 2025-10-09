using System.Collections.Generic;
using DocumentFormat.OpenXml;

namespace OperationLibrary.Common.Helper.Word{

/// <summary>
/// 表格資料類別
/// </summary>
    public class TableData
    {
        public List<string> Headers { get; set; } = new List<string>();
        public List<List<string>> Rows { get; set; } = new List<List<string>>();
    }

    /// <summary>
    /// 區段資料類別
    /// </summary>
    public class SectionData
    {
        public bool IsTable { get; set; }
        public string TargetName { get; set; }
        public List<OpenXmlElement> Contents { get; set; }
    }

    /// <summary>
    /// 書簽資料類別
    /// </summary>
    public class BookmarkData
    {
        public string Label { get; set; }
        public string Value { get; set; }
        
        public BookmarkData()
        {
            Label = string.Empty;
            Value = string.Empty;
        }

        public BookmarkData(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
}
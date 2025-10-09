using System.Collections.Generic;
namespace OperationLibrary.Common.Helper.Word{

    /// <summary>
    /// 表格區塊設定
    /// </summary>
    public class TableTemplate
    {
        /// <summary>
        /// 起始行索引
        /// </summary>
        public int StartRow { get; set; }

        /// <summary>
        /// 結束行索引
        /// </summary>
        public int EndRow { get; set; }

        /// <summary>
        /// 區塊資料
        /// </summary>
        public List<TableCustomRow> Rows { get; set; } = new List<TableCustomRow>();

        /// <summary>
        /// 合併儲存格設定
        /// </summary>
        public List<MergeInfo> MergedCells { get; set; } = new List<MergeInfo>();
        
        /// <summary>
        /// 資料內容設定（用於處理多筆資料）
        /// </summary>
        public DataContent Content { get; set; }
    }

    /// <summary>
    /// 資料內容設定
    /// </summary>
    public class DataContent
    {
        /// <summary>
        /// 資料來源的鍵值
        /// </summary>
        public string SourceKey { get; set; }

        /// <summary>
        /// 欄位映射設定
        /// </summary>
        public List<ColumnMapping> Columns { get; set; } = new List<ColumnMapping>();
    }

    /// <summary>
    /// 欄位映射設定
    /// </summary>
    public class ColumnMapping
    {
        /// <summary>
        /// 來源欄位名稱
        /// </summary>
        public string SourceField { get; set; }

        /// <summary>
        /// 目標欄位名稱
        /// </summary>
        public string TargetField { get; set; }

        /// <summary>
        /// 欄位索引
        /// </summary>
        public int Column { get; set; }
        
        /// <summary>
        /// 是否為 Markdown 內容
        /// </summary>
        public bool IsMarkdown { get; set; } = false;
    }

    /// <summary>
    /// 表格行設定
    /// </summary>
    public class TableCustomRow
    {
        /// <summary>
        /// 儲存格定義
        /// </summary>
        public List<TableCustomCell> Cells { get; set; } = new List<TableCustomCell>();
    }

    /// <summary>
    /// 儲存格定義，注意key名稱不可以一樣！
    /// </summary>
    public class TableCustomCell
    {
        /// <summary>
        /// 對應鍵值或標題文字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 欄位索引
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 是否為標題
        /// </summary>
        public bool IsTitle { get; set; }

        /// <summary>
        /// 是否為 Markdown 內容
        /// </summary>
        public bool IsMarkdown { get; set; } = false;
        
        /// <summary>
        /// 儲存格對齊方式
        /// </summary>
        public CellAlignment Alignment { get; set; } = CellAlignment.Left;
    }

    /// <summary>
    /// 合併儲存格資訊
    /// </summary>
    public class MergeInfo
    {
        /// <summary>
        /// 列索引 (相對於區塊起始列)
        /// </summary>
        public int Row { get; set; }
        
        /// <summary>
        /// 起始欄索引
        /// </summary>
        public int StartColumn { get; set; }
        
        /// <summary>
        /// 結束欄索引
        /// </summary>
        public int EndColumn { get; set; }
    }

    public enum CellAlignment
    {
        /// <summary>
        /// 靠左對齊
        /// </summary>
        Left,
        
        /// <summary>
        /// 置中對齊
        /// </summary>
        Center,
        
        /// <summary>
        /// 靠右對齊
        /// </summary>
        Right
    }
}
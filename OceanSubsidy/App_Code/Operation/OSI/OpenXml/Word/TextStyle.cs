namespace OperationLibrary.Common.Helper.Word{

    /// <summary>
    /// 文字樣式
    /// </summary>
    public class TextStyle
    {
        /// <summary>
        /// 是否粗體
        /// </summary>
        public bool IsBold { get; set; }

        /// <summary>
        /// 是否斜體
        /// </summary>
        public bool IsItalic { get; set; }
        
        /// <summary>
        /// 是大寫
        /// </summary>
        public bool IsLargeText { get; set; }

        /// <summary>
        /// 是否為超連結
        /// </summary>
        public bool IsHyperlink { get; set; }

        /// <summary>
        /// 超連結網址
        /// </summary>
        public string HyperlinkUrl { get; set; }
    }
}
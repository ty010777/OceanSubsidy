using System;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    /// <summary>
    /// 下拉選單項目
    /// </summary>
    public class DropdownItem
    {
        /// <summary>
        /// 選項值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 顯示文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 是否為預設選項
        /// </summary>
        public bool IsSelected { get; set; } = false;
    }
    #region 內部資料類別

    /// <summary>
    /// 進度資料類別
    /// </summary>
    public class ProgressData
    {
        public string ProjectID { get; set; }
        public int TotalCount { get; set; }
        public int CommentCount { get; set; }
        public int ReplyCount { get; set; }
        public string ReviewProgress { get; set; }
        public string ReplyProgress { get; set; }
        public string ReviewProgressDisplay { get; set; }
        public string ReplyProgressDisplay { get; set; }
    }

    /// <summary>
    /// 審查組別資料類別
    /// </summary>
    public class ReviewGroupData
    {
        public string ProjectID { get; set; }
        public string Field_Descname { get; set; }
    }

    #endregion
}
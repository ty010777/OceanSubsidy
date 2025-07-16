using System;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    /// <summary>
    /// 計畫類別資訊模型類別
    /// </summary>
    public class GrantTypeInfo
    {
        /// <summary>
        /// 計畫類別ID
        /// </summary>
        public string GrantTypeID { get; set; }
        
        /// <summary>
        /// 計畫類別全名
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// 計畫類別簡稱
        /// </summary>
        public string ShortName { get; set; }
        
        /// <summary>
        /// 計畫類別描述
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// 開始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// 結束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
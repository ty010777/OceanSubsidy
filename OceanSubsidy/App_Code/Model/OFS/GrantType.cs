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

        /// <summary>
        /// 計畫結束日期
        /// </summary>
        public DateTime? PlanEndDate { get; set; }

        public int? Year { get; set; }
    }

    public class GrantType
    {
        public int TypeID { get; set; }
        public int? Year { get; set; }
        public string TypeCode { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }

        public int? BudgetFees { get; set; }

        public string TargetTags { get; set; }

        public DateTime? ApplyStartDate { get; set; }
        public DateTime? ApplyEndDate { get; set; }
        public DateTime? PlanEndDate { get; set; }

        public string Review1Title { get; set; }
        public bool? Review1Enabled { get; set; }
        public string Review2Title { get; set; }
        public bool? Review2Enabled { get; set; }
        public string Review3Title { get; set; }
        public bool? Review3Enabled { get; set; }
        public string Review4Title { get; set; }
        public bool? Review4Enabled { get; set; }

        public int? OverduePeriod { get; set; }

        public DateTime? MidtermDeadline { get; set; }
        public DateTime? FinalDeadline { get; set; }
        public bool? FinalOneMonth { get; set; }

        public string Path { get; set; }
        public string Filename { get; set; }

        public string AdminUnit { get; set; }
    }
}

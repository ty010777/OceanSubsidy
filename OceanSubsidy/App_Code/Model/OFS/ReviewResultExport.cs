using System;
using System.Collections.Generic;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    /// <summary>
    /// 審查結果匯出資料模型
    /// </summary>
    public class ReviewResultExportModel
    {
        /// <summary>
        /// 組別名稱（如：資通訊、環境工程、科技材料）
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 該組別的所有專案審查結果
        /// </summary>
        public List<ProjectReviewResult> Projects { get; set; } = new List<ProjectReviewResult>();

        /// <summary>
        /// 該組別的審查委員名單
        /// </summary>
        public List<string> ReviewerNames { get; set; } = new List<string>();
    }

    /// <summary>
    /// 個別專案審查結果
    /// </summary>
    public class ProjectReviewResult
    {
        /// <summary>
        /// 組內排名
        /// </summary>
        public int Ranking { get; set; }

        /// <summary>
        /// 計畫編號
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// 計畫名稱
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 總分
        /// </summary>
        public decimal TotalScore { get; set; }

        /// <summary>
        /// 平均分數
        /// </summary>
        public decimal AverageScore { get; set; }

        /// <summary>
        /// 各委員評分 (委員名稱 => 分數)
        /// </summary>
        public Dictionary<string, decimal> ReviewerScores { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// 審查階段
        /// </summary>
        public string ReviewStage { get; set; }

        /// <summary>
        /// 申請單位
        /// </summary>
        public string ApplicantUnit { get; set; }
    }

    /// <summary>
    /// 審查結果匯出參數
    /// </summary>
    public class ReviewExportRequest
    {
        public string GrantType { get; set; }
        public string ReviewStage { get; set; }
        public List<string> Fields { get; set; } = new List<string>();
    }
}
using System;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    /// <summary>
    /// 計畫資料 (用於審查意見回覆)
    /// </summary>
    public class ProjectDataForReview
    {
        public string ProjectID { get; set; }
        public string Year { get; set; }
        public string ProjectName { get; set; }
        public string ReviewGroup { get; set; }
        public string ApplicantUnit { get; set; }
        public string UserName { get; set; }
    }
    public class ReplyItem
    {
        public string reviewId { get; set; }
        public string replyContent { get; set; }
    }
}
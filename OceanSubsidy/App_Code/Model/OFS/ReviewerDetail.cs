namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class ReviewerDetail
    {
        public string CommitteeUser { get; set; }
        public string ReviewStage { get; set; }
        public string FieldName { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
    }

    /// <summary>
    /// 審查人員資訊（用於批次設置審查人員）
    /// </summary>
    public class ReviewerInfo
    {
        public string Account { get; set; }  // 帳號 (Email)
        public string Name { get; set; }     // 中文名稱 (CommitteeUser)
    }
}

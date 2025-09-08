namespace GS.OCA_OceanSubsidy.Model.Report
{
    public class ApplyPlan
    {
        public int Year { get; set; }
        public string ProjectID { get; set; }
        public string Category { get; set; }
        public string ProjectName { get; set; }
        public string UserOrg { get; set; }
        public int ApprovedAmount { get; set; }
        public int ApplyAmount { get; set; }
        public int OtherAmount { get; set; }
        public int SpendAmount { get; set; }
        public int PaymentAmount { get; set; }
        public string SupervisoryUnit { get; set; }
        public string StageName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}

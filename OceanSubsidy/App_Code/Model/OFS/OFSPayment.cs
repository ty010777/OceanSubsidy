using System;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class OFSPayment
    {
        public int ID  { get; set; }
        public string ProjectID  { get; set; }
        public int? Stage  { get; set; }
        public decimal? ActDisbursementRatioPct  { get; set; }
        public decimal? TotalSpentAmount  { get; set; }
        public decimal? CurrentRequestAmount  { get; set; }
        public decimal? CurrentActualPaidAmount  { get; set; }
        public string Status  { get; set; }
        public string ReviewerComment  { get; set; }
        public string ReviewUser  { get; set; }
        public DateTime? ReviewTime  { get; set; }
        public DateTime? CreateTime  { get; set; }
        public DateTime? UpdateTime  { get; set; }
    }
}

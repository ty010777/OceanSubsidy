namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class GrantTargetSetting
    {
        public int ID { get; set; }

        public string GrantTypeID { get; set; }

        public string TargetTypeID { get; set; }

        public string TargetName { get; set; }

        public decimal? MatchingFund { get; set; }

        public decimal? GrantLimit { get; set; }

        public string Note { get; set; }
    }
}

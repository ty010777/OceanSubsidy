namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class ReviewCommittee
    {
        public int ID { get; set; }
        public string CommitteeUser { get; set; }
        public string Email { get; set; }
        public string SubjectTypeID { get; set; }

        public bool Deleted { get; set; }
    }
}

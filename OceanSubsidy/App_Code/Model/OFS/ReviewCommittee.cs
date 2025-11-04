using System;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class ReviewCommittee
    {
        public int ID { get; set; }
        public string CommitteeUser { get; set; }
        public string Email { get; set; }
        public string SubjectTypeID { get; set; }
        public string Token { get; set; }
        public string BankCode { get; set; }
        public string BankAccount { get; set; }
        public string BankPhoto { get; set; }
        public string RegistrationAddress { get; set; }

        public bool Deleted { get; set; }
        public int Type { get; set; }
        public int Count { get; set; }
        public string BankName { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}

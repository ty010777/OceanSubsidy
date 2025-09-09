using System;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class News
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime EnableTime { get; set; }

        public DateTime? DisableTime { get; set; }

        public string UserName { get; set; }

        public string UserOrg { get; set; }
    }
}

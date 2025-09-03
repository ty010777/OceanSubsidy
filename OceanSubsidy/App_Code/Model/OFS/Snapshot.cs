using System;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class Snapshot
    {
        public int ID { get; set; }

        public string Type { get; set; }

        public int DataID { get; set; }

        public int Status { get; set; }

        public string Data { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUser { get; set; }
    }
}

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class ProjectChangeRecord
    {
        public int ID { get; set; }

        public string Type { get; set; }

        public string DataID { get; set; }

        public string Reason { get; set; }

        public string Form1Before { get; set; }

        public string Form1After { get; set; }

        public string Form2Before { get; set; }

        public string Form2After { get; set; }

        public string Form3Before { get; set; }

        public string Form3After { get; set; }

        public string Form4Before { get; set; }

        public string Form4After { get; set; }

        public string Form5Before { get; set; }

        public string Form5After { get; set; }

        // 狀態
        // 1: 編輯中
        // 2: 待審核
        // 3: 審核通過
        // 4: 退回修改
        public int Status { get; set; }

        public string RejectReason { get; set; }
    }
}

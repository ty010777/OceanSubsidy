public class OFS_EdcBudgetPlan
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 預算項目
    public string Title { get; set; }

    // 海洋委員會經費
    public int Amount { get; set; }

    // 其他配合經費
    public int OtherAmount { get; set; }

    // 計算方式及說明
    public string Description { get; set; }

    public bool Deleted { get; set; }
}

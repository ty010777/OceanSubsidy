public class OFS_LitOtherSubsidy
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 單位名稱
    public string Unit { get; set; }

    // 補助金額
    public int Amount { get; set; }

    // 申請合作項目
    public string Content { get; set; }

    public bool Deleted { get; set; }
}

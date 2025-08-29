public class OFS_EdcReceivedSubsidy
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 計畫名稱
    public string Name { get; set; }

    // 補助金額
    public int Amount { get; set; }

    public bool Deleted { get; set; }
}

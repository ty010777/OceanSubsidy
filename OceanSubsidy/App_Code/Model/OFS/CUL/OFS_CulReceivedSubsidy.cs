public class OFS_CulReceivedSubsidy
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 計劃名稱
    public string Name { get; set; }

    // 補助單位
    public string Unit { get; set; }

    // 補助金額
    public int Amount { get; set; }

    public bool Deleted { get; set; }
}

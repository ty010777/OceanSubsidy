public class OFS_CulMonthlyProgress
{
    public int ID { get; set; }
    public int PID { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }

    // 實際工作執行情形
    public string Description { get; set; }

    // 狀態
    // 0:暫存
    // 1:提交
    public int Status { get; set; }
}

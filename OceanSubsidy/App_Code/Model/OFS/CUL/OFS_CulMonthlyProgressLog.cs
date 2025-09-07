public class OFS_CulMonthlyProgressLog
{
    public int ID { get; set; }
    public int PID { get; set; }
    public int MPID { get; set; }
    public int ScheduleID { get; set; }

    // 狀態
    // 1:未完成
    // 2:部分完成
    // 3:完成
    public int Status { get; set; }

    // 落後原因
    public string DelayReason { get; set; }

    // 改善措施
    public string ImprovedWay { get; set; }
}

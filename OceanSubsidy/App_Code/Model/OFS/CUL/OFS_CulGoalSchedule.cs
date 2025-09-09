public class OFS_CulGoalSchedule
{
    public int ID { get; set; }
    public int PID { get; set; }
    public int ItemID { get; set; }

    // 1:50%, 2:100%
    public int Type { get; set; }

    // 月份
    public int Month { get; set; }

    // 實施步驟
    public int StepID { get; set; }

    // 狀態
    // 1:未完成
    // 2:部分完成
    // 3:完成
    public int? Status { get; set; }

    public bool Deleted { get; set; }
}

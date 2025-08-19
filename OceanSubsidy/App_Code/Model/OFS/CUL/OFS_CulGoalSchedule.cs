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

    public bool Deleted { get; set; }
}

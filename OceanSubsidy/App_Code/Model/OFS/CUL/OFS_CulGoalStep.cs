public class OFS_CulGoalStep
{
    public int ID { get; set; }
    public int PID { get; set; }
    public int ItemID { get; set; }

    // 步驟
    public string Title { get; set; }

    // 月份 (起)
    public int Begin { get; set; }

    // 月份 (迄)
    public int End { get; set; }

    public bool Deleted { get; set; }
}

using System.Collections.Generic;

public class OFS_CulGoalItem
{
    public int ID { get; set; }
    public int PID { get; set; }
    public int GoalID { get; set; }

    // 重要工作項目
    public string Title { get; set; }

    // 績效指標
    public string Indicator { get; set; }

    public bool Deleted { get; set; }

    public List<OFS_CulGoalStep> Steps { get; set; }
    public List<OFS_CulGoalSchedule> Schedules { get; set; }
}

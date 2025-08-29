using System.Collections.Generic;

public class OFS_CulGoal
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 計畫目標
    public string Title { get; set; }

    // 預期效益（含量化或質化說明）
    public string Content { get; set; }

    public bool Deleted { get; set; }

    public List<OFS_CulGoalItem> Items { get; set; }
}

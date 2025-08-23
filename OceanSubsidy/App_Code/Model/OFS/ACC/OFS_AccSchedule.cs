using System;

public class OFS_AccSchedule
{
    public int ID { get; set; }
    public int PID { get; set; }
    public int ItemID { get; set; }

    // 1:50%, 2:100%
    public int Type { get; set; }

    // 預定完成日
    public DateTime? Deadline { get; set; }

    // 查核內容概述
    public string Content { get; set; }

    public bool Deleted { get; set; }
}

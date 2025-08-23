using System;

public class OFS_MulItem
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 工作項目
    public string Title { get; set; }

    // 月份 (起)
    public int Begin { get; set; }

    // 月份 (迄)
    public int End { get; set; }

    // 預定完成日
    public DateTime? Deadline { get; set; }

    // 詳細執行內容說明
    public string Content { get; set; }

    public bool Deleted { get; set; }
}

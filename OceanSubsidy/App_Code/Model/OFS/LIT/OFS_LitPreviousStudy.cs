using System;

public class OFS_LitPreviousStudy
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 研習/課程名稱
    public string Title { get; set; }

    // 辦理日期
    public DateTime? TheDate { get; set; }

    public bool Deleted { get; set; }
}

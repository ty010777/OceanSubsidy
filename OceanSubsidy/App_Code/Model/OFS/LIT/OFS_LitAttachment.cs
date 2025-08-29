public class OFS_LitAttachment
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 類型
    public int Type { get; set; }

    // 路徑
    public string Path { get; set; }

    // 名稱
    public string Name { get; set; }

    public bool Deleted { get; set; }
}

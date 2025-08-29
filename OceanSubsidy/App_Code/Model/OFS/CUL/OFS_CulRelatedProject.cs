public class OFS_CulRelatedProject
{
    public int ID { get; set; }
    public int PID { get; set; }

    // 相關計畫名稱
    public string Title { get; set; }

    // 執行年度
    public int Year { get; set; }

    // 執行單位
    public string OrgName { get; set; }

    // 計畫經費
    public int Amount { get; set; }

    // 內容摘述
    public string Description { get; set; }

    // 執行效益
    public string Benefit { get; set; }

    public bool Deleted { get; set; }
}

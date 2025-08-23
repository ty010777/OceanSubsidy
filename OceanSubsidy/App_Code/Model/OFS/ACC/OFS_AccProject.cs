using System;

public class OFS_AccProject
{
    public int ID { get; set; }

    // 年度
    public int Year { get; set; }

    // 計畫編號
    public string ProjectID { get; set; }

    // 補助計畫類別
    public string SubsidyPlanType { get; set; }

    // 計畫名稱
    public string ProjectName { get; set; }

    // 申請單位
    public string OrgName { get; set; }

    // 申請單位類別
    public string OrgCategory { get; set; }

    // 立案登記證字號
    public string RegisteredNum { get; set; }

    // 統一編號
    public string TaxID { get; set; }

    // 立案聯絡地址
    public string Address { get; set; }

    // 計畫目標
    public string Target { get; set; }

    // 計畫內容概要
    public string Summary { get; set; }

    // 預期效益 (量化)
    public string Quantified { get; set; }

    // 預期效益 (質化)
    public string Qualitative { get; set; }

    // 計畫期程 (起)
    public DateTime? StartTime { get; set; }

    // 計畫期程 (迄)
    public DateTime? EndTime { get; set; }

    // 申請海委會補助／合作金額
    public int ApplyAmount { get; set; }

    // 申請單位自籌款
    public int SelfAmount { get; set; }

    // 其他機關補助／合作總金額
    public int OtherAmount { get; set; }

    // 不可量化成果
    public string Benefit { get; set; }

    // 申請進度
    public int FormStep { get; set; }

    // 狀態
    public int Status { get; set; }

    public string UserAccount { get; set; }
    public string UserName { get; set; }
    public string UserOrg { get; set; }
}

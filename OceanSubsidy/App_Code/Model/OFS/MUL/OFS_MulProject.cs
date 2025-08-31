using System;

public class OFS_MulProject
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

    // 計畫類別
    public string Field { get; set; }

    // 申請單位
    public string OrgName { get; set; }

    // 申請單位類別
    public string OrgCategory { get; set; }

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
    public int? ApplyAmount { get; set; }

    // 申請單位自籌款
    public int? SelfAmount { get; set; }

    // 其他機關補助／合作總金額
    public int? OtherAmount { get; set; }

    // 不可量化成果
    public string Benefit { get; set; }

    // 申請進度
    public int FormStep { get; set; }

    // 狀態
    //  1: 申請中
    //  2: 資格審查
    //  3: 退回補正
    //  4: 資格審查不通過
    //  9: 核定補助經費
    // 10: 修正計畫書
    // 11: 決審
    // 12: 決審不通過
    // 13: 核定通過
    public int Status { get; set; }

    // 執行狀態
    public int ProgressStatus { get; set; }

    // 承辦
    public int? Organizer { get; set; }
    public string OrganizerName { get; set; }

    // 不通過原因
    public string RejectReason { get; set; }

    // 補正期限
    public DateTime? CorrectionDeadline { get; set; }

    public string UserAccount { get; set; }
    public string UserName { get; set; }
    public string UserOrg { get; set; }

    // 是否撤銷
    public bool IsWithdrawal { get; set; }

    // 是否有效
    public bool IsExists { get; set; }
}

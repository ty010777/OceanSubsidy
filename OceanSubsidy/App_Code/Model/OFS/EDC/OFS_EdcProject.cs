using System;

public class OFS_EdcProject
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

    // 申請單位類型
    public string OrgCategory { get; set; }

    // 申請單位
    public string OrgName { get; set; }

    // 立案登記證字號
    public string RegisteredNum { get; set; }

    // 統一編號
    public string TaxID { get; set; }

    // 立案聯絡地址
    public string Address { get; set; }

    // 計畫期程 (起)
    public DateTime? StartTime { get; set; }

    // 計畫期程 (迄)
    public DateTime? EndTime { get; set; }

    // 參加對象及人數
    public string Target { get; set; }

    // 計畫內容摘要
    public string Summary { get; set; }

    // 預期效益
    public string Quantified { get; set; }

    // 申請海委會補助經費
    public int? ApplyAmount { get; set; }

    // 申請單位自籌款
    public int? SelfAmount { get; set; }

    // 其他政府機關補助經費
    public int? OtherGovAmount { get; set; }

    // 其他單位補助經費（含總收費）
    public int? OtherUnitAmount { get; set; }

    // 申請進度
    public int FormStep { get; set; }

    // 狀態
    //  1: 申請中
    //  2: 資格審查
    //  3: 退回補正
    //  4: 資格審查不通過
    //  9: 核定補助經費
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

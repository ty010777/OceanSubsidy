using GS.OCA_OceanSubsidy.Model.OFS;
using System;

public class OFS_CulProject
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

    // 徵件類別
    public string Field { get; set; }

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
    public int? ApplyAmount { get; set; }

    // 申請單位自籌款
    public int? SelfAmount { get; set; }

    // 其他機關補助／合作總金額
    public int? OtherAmount { get; set; }

    // 核定金額
    public int? ApprovedAmount { get; set; }

    // 追回金額
    public int? RecoveryAmount { get; set; }

    // 申請進度
    public int FormStep { get; set; }

    // 狀態
    //  1: 編輯中
    // 11: 審查中
    // 12: 通過
    // 13: 不通過
    // 14: 補正補件
    // 15: 逾期未補
    // 19: 結案(未通過)
    // 21: 審查中
    // 22: 通過
    // 23: 不通過
    // 29: 結案(未通過)
    // 31: 審查中
    // 32: 通過
    // 33: 不通過
    // 39: 結案(未通過)
    // 41: 核定中
    // 42: 計畫書修正中
    // 43: 計畫書審核中
    // 44: 計畫書已確認
    // 45: 已核定
    // 46: 不通過
    // 49: 結案(未通過)
    // 51: 審核中
    // 52: 通過
    // 53: 不通過
    // 91: 已結案
    // 99: 已終止
    public int Status { get; set; }

    // 階段
    // 0: 尚未提送申請
    // 1: 資格審查
    // 2: 初審
    // 3: 複審
    // 4: 決審
    // 5: 執行階段
    // 9: 結案
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

    // 是否計畫變更中
    public bool IsProjChanged { get; set; }

    // 是否撤銷
    public bool IsWithdrawal { get; set; }

    // 是否有效
    public bool IsExists { get; set; }

    public ProjectChangeRecord changeApply { get; set; }
}

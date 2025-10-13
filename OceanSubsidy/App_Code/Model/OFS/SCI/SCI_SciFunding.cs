using System.Collections.Generic;
public class SciFundingDataSave
{
    public string projectId { get; set; }
    public List<PersonRow> personnel { get; set; }
    public List<MaterialRow> materials { get; set; }
    public List<ResearchFeeRow> researchFees { get; set; }
    public List<TravelRow> travel { get; set; }
    public List<OtherFeeRow> otherFees { get; set; }
    public List<OtherRent> otherRent { get; set; }
    public List<TotalFeeRow> totalFees { get; set; }
}
public class PersonRow
{
    public string name { get; set; }
    public bool stay { get; set; }
    public string title { get; set; }
    public decimal salary { get; set; }
    public decimal months { get; set; }
}

public class MaterialRow
{
    public string name { get; set; }
    public string description { get; set; }
    public string unit { get; set; }
    public decimal quantity { get; set; }
    public decimal unitPrice { get; set; }
}

public class ResearchFeeRow
{
    public string category { get; set; }
    public string dateStart { get; set; }
    public string dateEnd { get; set; }
    public string projectName { get; set; }
    public string targetPerson { get; set; }
    public decimal price { get; set; }
}

public class TravelRow
{
    public string reason { get; set; }
    public string area { get; set; }
    public decimal days { get; set; }
    public decimal people { get; set; }
    public decimal price { get; set; }
}

public class OtherFeeRow
{
    public string title { get; set; }
    public decimal avgSalary { get; set; }
    public decimal months { get; set; }
    public decimal people { get; set; }
}

public class OtherRent
{
    public string item { get; set; }
    public decimal amount { get; set; }
    public string note { get; set; }
}

public class TotalFeeRow
{
    public string accountingItem { get; set; }
    public decimal? subsidyAmount { get; set; }
    public decimal? coopAmount { get; set; }
}

/// <summary>
/// 經費概算彙總表資料模型 (包含百分比計算)
/// </summary>
public class BudgetSummaryRow
{
    public int ID { get; set; }
    public string ProjectID { get; set; }
    public string AccountingItem { get; set; }
    /// <summary>
    /// 補助款 (單位:千元)
    /// </summary>
    public decimal SubsidyAmount { get; set; }
    /// <summary>
    /// 配合款 (單位:千元)
    /// </summary>
    public decimal CoopAmount { get; set; }
    /// <summary>
    /// 總計 (單位:千元)
    /// </summary>
    public decimal TotalAmount { get; set; }
    /// <summary>
    /// 占總經費比率 (C)/(II) - 各項目總計/合計總計(II)
    /// </summary>
    public decimal? RatioOfTotalBudget { get; set; }
    /// <summary>
    /// 各科目補助比率 (A)/(I) - 各項目補助款/補助款總計(I)
    /// </summary>
    public decimal? RatioOfSubsidy { get; set; }
}

/// <summary>
/// 人事費明細表資料模型
/// </summary>
public class PersonnelCostRow
{
    public string ProjectID { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 職稱代碼
    /// </summary>
    public string JobTitle { get; set; }
    /// <summary>
    /// 職稱名稱
    /// </summary>
    public string JobTitleDesc { get; set; }
    /// <summary>
    /// 平均月薪
    /// </summary>
    public decimal AvgSalary { get; set; }
    /// <summary>
    /// 月份
    /// </summary>
    public decimal Month { get; set; }
    /// <summary>
    /// 是否待聘
    /// </summary>
    public bool IsPending { get; set; }
    /// <summary>
    /// 小計 (平均月薪 * 月份)
    /// </summary>
    public decimal Subtotal { get; set; }
}

/// <summary>
/// 消耗性器材及原材料費明細表資料模型
/// </summary>
public class MaterialCostRow
{
    public string ProjectID { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    public string ItemName { get; set; }
    /// <summary>
    /// 說明
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 單位代碼
    /// </summary>
    public string Unit { get; set; }
    /// <summary>
    /// 單位名稱
    /// </summary>
    public string UnitDesc { get; set; }
    /// <summary>
    /// 預估數量
    /// </summary>
    public decimal PreNum { get; set; }
    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// 總價 (預估數量 * 單價)
    /// </summary>
    public decimal TotalPrice { get; set; }
}

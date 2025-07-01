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
    public decimal subsidyAmount { get; set; }
    public decimal coopAmount { get; set; }
}

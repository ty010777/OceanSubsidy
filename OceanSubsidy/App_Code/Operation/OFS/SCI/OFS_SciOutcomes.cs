using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class OFS_SciOutcomeHelper : System.Web.UI.Page
{
    public OFS_SciOutcomeHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }
public static OFS_SCI_Outcomes GetOutcomeData(string Version_ID)
{
    using (DbHelper db = new DbHelper())
    {
        try
        {
            db.CommandText = "SELECT * FROM OFS_SCI_Outcomes WHERE Version_ID = @Version_ID";
            db.Parameters.Clear();
            db.Parameters.Add("@Version_ID", Version_ID);
            
            DataTable dt = db.GetTable();
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return BuildVueDataModel(row);
            }
            else
            {
                OFS_SCI_Outcomes data = new OFS_SCI_Outcomes();
                return data;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("載入成果資料發生錯誤：" + ex.Message, ex);
        }
    }
}

private static OFS_SCI_Outcomes BuildVueDataModel(DataRow row)
{
    return new OFS_SCI_Outcomes
    {
        Version_ID = row["Version_ID"].ToString(),

        TechTransfer_Plan_Count = GetNullableInt(row["TechTransfer_Plan_Count"]),
        TechTransfer_Plan_Price = GetNullableDecimal(row["TechTransfer_Plan_Price"]),
        TechTransfer_Track_Count = GetNullableInt(row["TechTransfer_Track_Count"]),
        TechTransfer_Track_Price = GetNullableDecimal(row["TechTransfer_Track_Price"]),
        TechTransfer_Description = row["TechTransfer_Description"].ToString(),

        Patent_Plan_Apply = GetNullableInt(row["Patent_Plan_Apply"]),
        Patent_Plan_Grant = GetNullableInt(row["Patent_Plan_Grant"]),
        Patent_Track_Apply = GetNullableInt(row["Patent_Track_Apply"]),
        Patent_Track_Grant = GetNullableInt(row["Patent_Track_Grant"]),
        Patent_Description = row["Patent_Description"].ToString(),

        Talent_Plan_PhD = GetNullableInt(row["Talent_Plan_PhD"]),
        Talent_Plan_Master = GetNullableInt(row["Talent_Plan_Master"]),
        Talent_Plan_Others = GetNullableInt(row["Talent_Plan_Others"]),
        Talent_Track_PhD = GetNullableInt(row["Talent_Track_PhD"]),
        Talent_Track_Master = GetNullableInt(row["Talent_Track_Master"]),
        Talent_Track_Others = GetNullableInt(row["Talent_Track_Others"]),
        Talent_Description = row["Talent_Description"].ToString(),

        Papers_Plan = GetNullableInt(row["Papers_Plan"]),
        Papers_Track = GetNullableInt(row["Papers_Track"]),
        Papers_Description = row["Papers_Description"].ToString(),

        IndustryCollab_Plan_Count = GetNullableInt(row["IndustryCollab_Plan_Count"]),
        IndustryCollab_Plan_Price = GetNullableDecimal(row["IndustryCollab_Plan_Price"]),
        IndustryCollab_Track_Count = GetNullableInt(row["IndustryCollab_Track_Count"]),
        IndustryCollab_Track_Price = GetNullableDecimal(row["IndustryCollab_Track_Price"]),
        IndustryCollab_Description = row["IndustryCollab_Description"].ToString(),

        Investment_Plan_Price = GetNullableDecimal(row["Investment_Plan_Price"]),
        Investment_Track_Price = GetNullableDecimal(row["Investment_Track_Price"]),
        Investment_Description = row["Investment_Description"].ToString(),

        Products_Plan_Count = GetNullableInt(row["Products_Plan_Count"]),
        Products_Plan_Price = GetNullableDecimal(row["Products_Plan_Price"]),
        Products_Track_Count = GetNullableInt(row["Products_Track_Count"]),
        Products_Track_Price = GetNullableDecimal(row["Products_Track_Price"]),
        Products_Description = row["Products_Description"].ToString(),

        CostReduction_Plan_Price = GetNullableDecimal(row["CostReduction_Plan_Price"]),
        CostReduction_Track_Price = GetNullableDecimal(row["CostReduction_Track_Price"]),
        CostReduction_Description = row["CostReduction_Description"].ToString(),

        PromoEvents_Plan = GetNullableInt(row["PromoEvents_Plan"]),
        PromoEvents_Track = GetNullableInt(row["PromoEvents_Track"]),
        PromoEvents_Description = row["PromoEvents_Description"].ToString(),

        TechServices_Plan_Count = GetNullableInt(row["TechServices_Plan_Count"]),
        TechServices_Plan_Price = GetNullableDecimal(row["TechServices_Plan_Price"]),
        TechServices_Track_Count = GetNullableInt(row["TechServices_Track_Count"]),
        TechServices_Track_Price = GetNullableDecimal(row["TechServices_Track_Price"]),
        TechServices_Description = row["TechServices_Description"].ToString(),

        Other_Plan_Description = row["Other_Plan_Description"].ToString(),
        Other_Track_Description = row["Other_Track_Description"].ToString()
    };
    
}


private static int? GetNullableInt(object value)
{
    if (value == null || value == DBNull.Value) return null;
    if (int.TryParse(value.ToString(), out int result)) return result;
    return null;
}

private static decimal? GetNullableDecimal(object value)
{
    if (value == null || value == DBNull.Value) return null;
    if (decimal.TryParse(value.ToString(), out decimal result)) return result;
    return null;
}

public static void SaveOutcomeData(OFS_SCI_Outcomes entityData)
{
    using (DbHelper db = new DbHelper())
    {
        try
        {
            // 1. 刪除舊資料
            db.CommandText = "DELETE FROM OFS_SCI_Outcomes WHERE Version_ID = @Version_ID";
            db.Parameters.Clear();
            db.Parameters.Add("@Version_ID", entityData.Version_ID);
            db.ExecuteNonQuery();

            // 2. 插入新資料
            db.CommandText = @"
                INSERT INTO OFS_SCI_Outcomes (
                    Version_ID,
                    TechTransfer_Plan_Count, TechTransfer_Plan_Price, TechTransfer_Track_Count, TechTransfer_Track_Price, TechTransfer_Description,
                    Patent_Plan_Apply, Patent_Plan_Grant, Patent_Track_Apply, Patent_Track_Grant, Patent_Description,
                    Talent_Plan_PhD, Talent_Plan_Master, Talent_Plan_Others, Talent_Track_PhD, Talent_Track_Master, Talent_Track_Others, Talent_Description,
                    Papers_Plan, Papers_Track, Papers_Description,
                    IndustryCollab_Plan_Count, IndustryCollab_Plan_Price, IndustryCollab_Track_Count, IndustryCollab_Track_Price, IndustryCollab_Description,
                    Investment_Plan_Price, Investment_Track_Price, Investment_Description,
                    Products_Plan_Count, Products_Plan_Price, Products_Track_Count, Products_Track_Price, Products_Description,
                    CostReduction_Plan_Price, CostReduction_Track_Price, CostReduction_Description,
                    PromoEvents_Plan, PromoEvents_Track, PromoEvents_Description,
                    TechServices_Plan_Count, TechServices_Plan_Price, TechServices_Track_Count, TechServices_Track_Price, TechServices_Description,
                    Other_Plan_Description, Other_Track_Description
                )
                VALUES (
                    @Version_ID,
                    @TechTransfer_Plan_Count, @TechTransfer_Plan_Price, @TechTransfer_Track_Count, @TechTransfer_Track_Price, @TechTransfer_Description,
                    @Patent_Plan_Apply, @Patent_Plan_Grant, @Patent_Track_Apply, @Patent_Track_Grant, @Patent_Description,
                    @Talent_Plan_PhD, @Talent_Plan_Master, @Talent_Plan_Others, @Talent_Track_PhD, @Talent_Track_Master, @Talent_Track_Others, @Talent_Description,
                    @Papers_Plan, @Papers_Track, @Papers_Description,
                    @IndustryCollab_Plan_Count, @IndustryCollab_Plan_Price, @IndustryCollab_Track_Count, @IndustryCollab_Track_Price, @IndustryCollab_Description,
                    @Investment_Plan_Price, @Investment_Track_Price, @Investment_Description,
                    @Products_Plan_Count, @Products_Plan_Price, @Products_Track_Count, @Products_Track_Price, @Products_Description,
                    @CostReduction_Plan_Price, @CostReduction_Track_Price, @CostReduction_Description,
                    @PromoEvents_Plan, @PromoEvents_Track, @PromoEvents_Description,
                    @TechServices_Plan_Count, @TechServices_Plan_Price, @TechServices_Track_Count, @TechServices_Track_Price, @TechServices_Description,
                    @Other_Plan_Description, @Other_Track_Description
                )
            ";

            db.Parameters.Clear();
            db.Parameters.Add("@Version_ID", entityData.Version_ID);

            // 以下請根據你的表單填入對應的資料
            db.Parameters.Add("@TechTransfer_Plan_Count", entityData.TechTransfer_Plan_Count);
            db.Parameters.Add("@TechTransfer_Plan_Price", entityData.TechTransfer_Plan_Price);
            db.Parameters.Add("@TechTransfer_Track_Count", entityData.TechTransfer_Track_Count);
            db.Parameters.Add("@TechTransfer_Track_Price",entityData.TechTransfer_Track_Price);
            db.Parameters.Add("@TechTransfer_Description",entityData.TechTransfer_Description );
            db.Parameters.Add("@Patent_Plan_Apply",entityData.Patent_Plan_Apply) ;
            db.Parameters.Add("@Patent_Plan_Grant", entityData.Patent_Plan_Grant);
            db.Parameters.Add("@Patent_Track_Apply",entityData.Patent_Track_Apply );
            db.Parameters.Add("@Patent_Track_Grant", entityData.Patent_Track_Grant);
            db.Parameters.Add("@Patent_Description", entityData.Patent_Description);
            db.Parameters.Add("@Talent_Plan_PhD", entityData.Talent_Plan_PhD);
            db.Parameters.Add("@Talent_Plan_Master", entityData.Talent_Plan_Master);
            db.Parameters.Add("@Talent_Plan_Others", entityData.Talent_Plan_Others);
            db.Parameters.Add("@Talent_Track_PhD", entityData.Talent_Track_PhD);
            db.Parameters.Add("@Talent_Track_Master", entityData.Talent_Track_Master);
            db.Parameters.Add("@Talent_Track_Others", entityData.Talent_Track_Others);
            db.Parameters.Add("@Talent_Description", entityData.Talent_Description);
            db.Parameters.Add("@Papers_Plan", entityData.Papers_Plan);
            db.Parameters.Add("@Papers_Track", entityData.Papers_Track);
            db.Parameters.Add("@Papers_Description", entityData.Papers_Description);
            db.Parameters.Add("@IndustryCollab_Plan_Count", entityData.IndustryCollab_Plan_Count);
            db.Parameters.Add("@IndustryCollab_Plan_Price", entityData.IndustryCollab_Plan_Price);
            db.Parameters.Add("@IndustryCollab_Track_Count", entityData.IndustryCollab_Track_Count);
            db.Parameters.Add("@IndustryCollab_Track_Price", entityData.IndustryCollab_Track_Price);
            db.Parameters.Add("@IndustryCollab_Description", entityData.IndustryCollab_Description);
            db.Parameters.Add("@Investment_Plan_Price", entityData.Investment_Plan_Price);
            db.Parameters.Add("@Investment_Track_Price", entityData.Investment_Track_Price);
            db.Parameters.Add("@Investment_Description", entityData.Investment_Description);
            db.Parameters.Add("@Products_Plan_Count", entityData.Products_Plan_Count);
            db.Parameters.Add("@Products_Plan_Price", entityData.Products_Plan_Price);
            db.Parameters.Add("@Products_Track_Count", entityData.Products_Track_Count);
            db.Parameters.Add("@Products_Track_Price", entityData.Products_Track_Price);
            db.Parameters.Add("@Products_Description", entityData.Products_Description);
            db.Parameters.Add("@CostReduction_Plan_Price", entityData.CostReduction_Plan_Price);
            db.Parameters.Add("@CostReduction_Track_Price", entityData.CostReduction_Track_Price);
            db.Parameters.Add("@CostReduction_Description", entityData.CostReduction_Description);
            db.Parameters.Add("@PromoEvents_Plan", entityData.PromoEvents_Plan);
            db.Parameters.Add("@PromoEvents_Track", entityData.PromoEvents_Track);
            db.Parameters.Add("@PromoEvents_Description", entityData.PromoEvents_Description);
            db.Parameters.Add("@TechServices_Plan_Count", entityData.TechServices_Plan_Count);
            db.Parameters.Add("@TechServices_Plan_Price", entityData.TechServices_Plan_Price);
            db.Parameters.Add("@TechServices_Track_Count", entityData.TechServices_Track_Count);
            db.Parameters.Add("@TechServices_Track_Price", entityData.TechServices_Track_Price);
            db.Parameters.Add("@TechServices_Description", entityData.TechServices_Description);
            db.Parameters.Add("@Other_Plan_Description", entityData.Other_Plan_Description);
            db.Parameters.Add("@Other_Track_Description", entityData.Other_Track_Description);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception("儲存成果資料發生錯誤：" + ex.Message, ex);
        }
    }
    
}

private static int? ToInt(Dictionary<string, string> values, string key)
{
    return values.ContainsKey(key) && int.TryParse(values[key], out int val) ? val : (int?)null;
}

private static decimal? ToDecimal(Dictionary<string, string> values, string key)
{
    return values.ContainsKey(key) && decimal.TryParse(values[key], out decimal val) ? val : (decimal?)null;
}

private static string GetValue(Dictionary<string, string> values, string key)
{
    return values.ContainsKey(key) ? values[key] : null;
}
}
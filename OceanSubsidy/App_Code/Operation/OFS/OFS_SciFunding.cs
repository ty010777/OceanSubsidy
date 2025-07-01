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
public class OFS_SciFundingHelper
{
    public OFS_SciFundingHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }


    public static List<Sys_ZgsCode> GetSysZgsCodeByCodeGroup(string codeGroup)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM Sys_ZgsCode
        WHERE CodeGroup = @CodeGroup
        ORDER BY OrderNo";

        db.Parameters.Clear();
        db.Parameters.Add("@CodeGroup", codeGroup);

        try
        {
            DataTable dt = db.GetTable();
            List<Sys_ZgsCode> codeList = new List<Sys_ZgsCode>();

            foreach (DataRow row in dt.Rows)
            {
                var code = new Sys_ZgsCode
                {
                    CodeGroup = row["CodeGroup"]?.ToString(),
                    Code = row["Code"]?.ToString(),
                    Descname = row["Descname"]?.ToString(),
                    OrderNo = row["OrderNo"] != DBNull.Value ? Convert.ToInt32(row["OrderNo"]) : 0,
                    IsValid = row["IsValid"] != DBNull.Value ? Convert.ToString(row["IsValid"]) : "0",
                    ValidBeginDate = row["ValidBeginDate"] != DBNull.Value ? Convert.ToString(row["ValidBeginDate"]) :null,
                    ValidEndDate = row["ValidEndDate"] != DBNull.Value ? Convert.ToString(row["ValidEndDate"]) : null,
                    ParentCode = row["ParentCode"]?.ToString(),
                    MaxPriceLimit = row["MaxPriceLimit"] != DBNull.Value ? Convert.ToDecimal(row["MaxPriceLimit"]) : 0
                };

                codeList.Add(code);
            }

            return codeList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢 Sys_ZgsCode 資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }
    public static void ReplacePersonFormList(List<PersonRow> personList, string Version_ID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 Version_ID 所有資料
                    db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_PersonForm WHERE Version_ID = @Version_ID";
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.ExecuteNonQuery();
                // 一筆一筆插入 personList 的每筆資料
                foreach (var formRow in personList)
                {
                    // 驗證數值範圍，避免溢位
                    decimal? avgSalary = null;
                    decimal? months = null;
                    
                    try
                    {
                        // 檢查薪資範圍 (假設最大值為 999999999.99)
                        if (formRow.salary >= 0 && formRow.salary <= 999999999.99m)
                        {
                            avgSalary = formRow.salary;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("salary", $"薪資數值超出範圍: {formRow.salary}");
                        }
                        
                    }
                    catch (OverflowException)
                    {
                        throw new Exception($"人員 {formRow.name} 的數值發生溢位錯誤");
                    }

                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_PersonForm
                    (Version_ID, Name, IsPending, JobTitle, AvgSalary, Month)
                    VALUES
                    (@Version_ID, @Name, @IsPending, @JobTitle, @AvgSalary, @Month)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.Parameters.Add("@Name", formRow.name ?? "");
                    db.Parameters.Add("@IsPending", formRow.stay);
                    db.Parameters.Add("@JobTitle", formRow.title ?? "");
                    db.Parameters.Add("@AvgSalary", avgSalary.HasValue ? (object)avgSalary.Value : DBNull.Value);
                    db.Parameters.Add("@Month", formRow.months );

                    db.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"批次儲存 PersonForm 時發生錯誤: {ex.Message}", ex);
            }
        }
    }


    public static void ReplaceMaterialList(List<MaterialRow> materialList, string Version_ID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 Version_ID 的所有資料
                db.CommandText = @"
                DELETE FROM OFS_SCI_PersonnelCost_Material
                WHERE Version_ID = @Version_ID";
                db.Parameters.Clear();
                db.Parameters.Add("@Version_ID", Version_ID);
                db.ExecuteNonQuery();

                // 再逐筆插入
                foreach (var material in materialList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_Material
                    (Version_ID, ItemName, Description, Unit, PreNum, UnitPrice)
                    VALUES
                    (@Version_ID, @ItemName, @Description, @Unit, @PreNum, @UnitPrice)";
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.Parameters.Add("@ItemName", material.name);
                    db.Parameters.Add("@Description", material.description);
                    db.Parameters.Add("@Unit", material.unit);
                    db.Parameters.Add("@PreNum", material.quantity);
                    db.Parameters.Add("@UnitPrice", material.unitPrice);

                    db.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"更新材料資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }
    public static void ReplaceResearchFees(List<ResearchFeeRow> feeList, string Version_ID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 Version_ID 的所有資料
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_ResearchFees WHERE Version_ID = @Version_ID";
                db.Parameters.Clear();
                db.Parameters.Add("@Version_ID", Version_ID);
                db.ExecuteNonQuery();

                // 再一筆一筆插入
                foreach (var row in feeList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_ResearchFees
                    (Version_ID, FeeCategory, StartDate, EndDate, Name, PersonName, Price)
                    VALUES
                    (@Version_ID, @FeeCategory, @StartDate, @EndDate, @Name, @PersonName, @Price)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.Parameters.Add("@FeeCategory", row.category ?? "");
                    db.Parameters.Add("@StartDate", string.IsNullOrWhiteSpace(row.dateStart) ? (object)DBNull.Value : row.dateStart);
                    db.Parameters.Add("@EndDate", string.IsNullOrWhiteSpace(row.dateEnd) ? (object)DBNull.Value : row.dateEnd);
                    db.Parameters.Add("@Name", row.projectName ?? "");
                    db.Parameters.Add("@PersonName", row.targetPerson ?? "");
                    db.Parameters.Add("@Price", row.price);

                    db.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"儲存 OFS_SCI_PersonnelCost_ResearchFee 時發生錯誤: {ex.Message}", ex);
            }
        }
    }
    public static void ReplaceTripForm(List<TravelRow> tripList, string Version_ID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_TripForm WHERE Version_ID = @Version_ID";
                db.Parameters.Clear();
                db.Parameters.Add("@Version_ID", Version_ID);
                db.ExecuteNonQuery();

                foreach (var row in tripList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_TripForm
                    (Version_ID, TripReason, Area, Days, Times, Price)
                    VALUES
                    (@Version_ID, @TripReason, @Area, @Days, @Times, @Price)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.Parameters.Add("@TripReason", row.reason ?? "");
                    db.Parameters.Add("@Area", row.area ?? "");
                    db.Parameters.Add("@Days", row.days);
                    db.Parameters.Add("@Times", row.people);
                    db.Parameters.Add("@Price", row.price);

                    db.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"儲存 TripForm 時發生錯誤: {ex.Message}", ex);
            }
        }
    }
    public static void ReplaceOtherPersonFee(List<OtherFeeRow> feeList, string Version_ID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_OtherPersonFee WHERE Version_ID = @Version_ID";
                db.Parameters.Clear();
                db.Parameters.Add("@Version_ID", Version_ID);
                db.ExecuteNonQuery();

                foreach (var row in feeList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_OtherPersonFee
                    (Version_ID, JobTitle, AvgSalary, Month, PeopleNum)
                    VALUES
                    (@Version_ID, @JobTitle, @AvgSalary, @Month, @PeopleNum)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.Parameters.Add("@JobTitle", row.title ?? "");
                    db.Parameters.Add("@AvgSalary", row.avgSalary);
                    db.Parameters.Add("@Month", row.months);
                    db.Parameters.Add("@PeopleNum", row.people);

                    db.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"儲存 OtherPersonFee 時發生錯誤: {ex.Message}", ex);
            }
        }
    }
    public static void 
        ReplaceOtherObjectFee(List<OtherRent> rentList, string Version_ID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_OtherObjectFee WHERE Version_ID = @Version_ID";
                db.Parameters.Clear();
                db.Parameters.Add("@Version_ID", Version_ID);
                db.ExecuteNonQuery();

                foreach (var row in rentList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_OtherObjectFee
                    (Version_ID, Name, Price, CalDescription)
                    VALUES
                    (@Version_ID, @Name, @Price, @CalDescription)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.Parameters.Add("@Name", row.item ?? "");
                    db.Parameters.Add("@Price", row.amount);
                    db.Parameters.Add("@CalDescription", row.note ?? "");

                    db.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"儲存 OtherObjectFee 時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static void ReplaceTotalFeeList(List<TotalFeeRow> totalFeeList, string Version_ID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 Version_ID 的所有資料
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_TotalFee WHERE Version_ID = @Version_ID";
                db.Parameters.Clear();
                db.Parameters.Add("@Version_ID", Version_ID);
                db.ExecuteNonQuery();

                // 再逐筆插入經費總表資料
                foreach (var totalFee in totalFeeList)
                {
                    // 驗證數值範圍，避免溢位
                    decimal? subsidyAmount = null;
                    decimal? coopAmount = null;
                    decimal? totalAmount = null;
                    
                    try
                    {
                        // 檢查補助款範圍
                        if (totalFee.subsidyAmount >= 0 && totalFee.subsidyAmount <= 999999999999999.99m)
                        {
                            subsidyAmount = totalFee.subsidyAmount;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("subsidyAmount", $"補助款數值超出範圍: {totalFee.subsidyAmount}");
                        }
                        
                        // 檢查配合款範圍
                        if (totalFee.coopAmount >= 0 && totalFee.coopAmount <= 999999999999999.99m)
                        {
                            coopAmount = totalFee.coopAmount;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("coopAmount", $"配合款數值超出範圍: {totalFee.coopAmount}");
                        }
                        
                    }
                    catch (OverflowException)
                    {
                        throw new Exception($"經費科目 {totalFee.accountingItem} 的數值發生溢位錯誤");
                    }

                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_TotalFee
                    (Version_ID, AccountingItem, SubsidyAmount, CoopAmount )
                    VALUES
                    (@Version_ID, @AccountingItem, @SubsidyAmount, @CoopAmount)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.Parameters.Add("@AccountingItem", totalFee.accountingItem ?? "");
                    db.Parameters.Add("@SubsidyAmount", subsidyAmount.HasValue ? (object)subsidyAmount.Value : DBNull.Value);
                    db.Parameters.Add("@CoopAmount", coopAmount.HasValue ? (object)coopAmount.Value : DBNull.Value);

                    db.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"儲存經費總表時發生錯誤: {ex.Message}", ex);
            }
        }
    }

}


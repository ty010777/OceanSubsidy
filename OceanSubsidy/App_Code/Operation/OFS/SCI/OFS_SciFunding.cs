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
    public static void ReplacePersonFormList(List<PersonRow> personList, string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 ProjectID 所有資料
                    db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_PersonForm WHERE ProjectID = @ProjectID";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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
                    (ProjectID, Name, IsPending, JobTitle, AvgSalary, Month)
                    VALUES
                    (@ProjectID, @Name, @IsPending, @JobTitle, @AvgSalary, @Month)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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


    public static void ReplaceMaterialList(List<MaterialRow> materialList, string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 ProjectID 的所有資料
                db.CommandText = @"
                DELETE FROM OFS_SCI_PersonnelCost_Material
                WHERE ProjectID = @ProjectID";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                db.ExecuteNonQuery();

                // 再逐筆插入
                foreach (var material in materialList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_Material
                    (ProjectID, ItemName, Description, Unit, PreNum, UnitPrice)
                    VALUES
                    (@ProjectID, @ItemName, @Description, @Unit, @PreNum, @UnitPrice)";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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
    public static void ReplaceResearchFees(List<ResearchFeeRow> feeList, string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 ProjectID 的所有資料
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_ResearchFees WHERE ProjectID = @ProjectID";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                db.ExecuteNonQuery();

                // 再一筆一筆插入
                foreach (var row in feeList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_ResearchFees
                    (ProjectID, FeeCategory, StartDate, EndDate, Name, PersonName, Price)
                    VALUES
                    (@ProjectID, @FeeCategory, @StartDate, @EndDate, @Name, @PersonName, @Price)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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
    public static void ReplaceTripForm(List<TravelRow> tripList, string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_TripForm WHERE ProjectID = @ProjectID";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                db.ExecuteNonQuery();

                foreach (var row in tripList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_TripForm
                    (ProjectID, TripReason, Area, Days, Times, Price)
                    VALUES
                    (@ProjectID, @TripReason, @Area, @Days, @Times, @Price)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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
    public static void ReplaceOtherPersonFee(List<OtherFeeRow> feeList, string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_OtherPersonFee WHERE ProjectID = @ProjectID";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                db.ExecuteNonQuery();

                foreach (var row in feeList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_OtherPersonFee
                    (ProjectID, JobTitle, AvgSalary, Month, PeopleNum)
                    VALUES
                    (@ProjectID, @JobTitle, @AvgSalary, @Month, @PeopleNum)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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
        ReplaceOtherObjectFee(List<OtherRent> rentList, string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_OtherObjectFee WHERE ProjectID = @ProjectID";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                db.ExecuteNonQuery();

                foreach (var row in rentList)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_PersonnelCost_OtherObjectFee
                    (ProjectID, Name, Price, CalDescription)
                    VALUES
                    (@ProjectID, @Name, @Price, @CalDescription)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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

    public static void ReplaceTotalFeeList(List<TotalFeeRow> totalFeeList, string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 先刪除該 ProjectID 的所有資料
                db.CommandText = @"DELETE FROM OFS_SCI_PersonnelCost_TotalFee WHERE ProjectID = @ProjectID";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
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
                    (ProjectID, AccountingItem, SubsidyAmount, CoopAmount )
                    VALUES
                    (@ProjectID, @AccountingItem, @SubsidyAmount, @CoopAmount)";
                
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
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

    // 新增讀取資料的方法
    public static List<PersonRow> GetPersonFormList(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT ProjectID, Name, IsPending, JobTitle, AvgSalary, Month
                FROM OFS_SCI_PersonnelCost_PersonForm
                WHERE ProjectID = @ProjectID
                ORDER BY Name";
                
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                
                DataTable dt = db.GetTable();
                List<PersonRow> personList = new List<PersonRow>();
                
                foreach (DataRow row in dt.Rows)
                {
                    var person = new PersonRow
                    {
                        name = row["Name"]?.ToString() ?? "",
                        stay = row["IsPending"] != DBNull.Value ? Convert.ToBoolean(row["IsPending"]) : false,
                        title = row["JobTitle"]?.ToString() ?? "",
                        salary = row["AvgSalary"] != DBNull.Value ? Convert.ToDecimal(row["AvgSalary"]) : 0,
                        months = row["Month"] != DBNull.Value ? Convert.ToDecimal(row["Month"]) : 0
                    };
                    personList.Add(person);
                }
                
                return personList;
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取人員資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static List<MaterialRow> GetMaterialList(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT ProjectID, ItemName, Description, Unit, PreNum, UnitPrice
                FROM OFS_SCI_PersonnelCost_Material
                WHERE ProjectID = @ProjectID
                ORDER BY ItemName";
                
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                
                DataTable dt = db.GetTable();
                List<MaterialRow> materialList = new List<MaterialRow>();
                
                foreach (DataRow row in dt.Rows)
                {
                    var material = new MaterialRow
                    {
                        name = row["ItemName"]?.ToString() ?? "",
                        description = row["Description"]?.ToString() ?? "",
                        unit = row["Unit"]?.ToString() ?? "",
                        quantity = row["PreNum"] != DBNull.Value ? Convert.ToDecimal(row["PreNum"]) : 0,
                        unitPrice = row["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(row["UnitPrice"]) : 0
                    };
                    materialList.Add(material);
                }
                
                return materialList;
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取材料資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static List<ResearchFeeRow> GetResearchFeesList(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT ProjectID, FeeCategory, StartDate, EndDate, Name, PersonName, Price
                FROM OFS_SCI_PersonnelCost_ResearchFees
                WHERE ProjectID = @ProjectID
                ORDER BY FeeCategory";
                
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                
                DataTable dt = db.GetTable();
                List<ResearchFeeRow> feeList = new List<ResearchFeeRow>();
                
                foreach (DataRow row in dt.Rows)
                {
                    var fee = new ResearchFeeRow
                    {
                        category = row["FeeCategory"]?.ToString() ?? "",
                        dateStart = ConvertDateToString(row["StartDate"]),
                        dateEnd = ConvertDateToString(row["EndDate"]),
                        projectName = row["Name"]?.ToString() ?? "",
                        targetPerson = row["PersonName"]?.ToString() ?? "",
                        price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0
                    };
                    feeList.Add(fee);
                }
                
                return feeList;
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取研究費資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static List<TravelRow> GetTripFormList(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT ProjectID, TripReason, Area, Days, Times, Price
                FROM OFS_SCI_PersonnelCost_TripForm
                WHERE ProjectID = @ProjectID
                ORDER BY TripReason";
                
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                
                DataTable dt = db.GetTable();
                List<TravelRow> travelList = new List<TravelRow>();
                
                foreach (DataRow row in dt.Rows)
                {
                    var travel = new TravelRow
                    {
                        reason = row["TripReason"]?.ToString() ?? "",
                        area = row["Area"]?.ToString() ?? "",
                        days = row["Days"] != DBNull.Value ? Convert.ToDecimal(row["Days"]) : 0,
                        people = row["Times"] != DBNull.Value ? Convert.ToDecimal(row["Times"]) : 0,
                        price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0
                    };
                    travelList.Add(travel);
                }
                
                return travelList;
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取差旅費資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static List<OtherFeeRow> GetOtherPersonFeeList(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT ProjectID, JobTitle, AvgSalary, Month, PeopleNum
                FROM OFS_SCI_PersonnelCost_OtherPersonFee
                WHERE ProjectID = @ProjectID
                ORDER BY JobTitle";
                
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                
                DataTable dt = db.GetTable();
                List<OtherFeeRow> feeList = new List<OtherFeeRow>();
                
                foreach (DataRow row in dt.Rows)
                {
                    var fee = new OtherFeeRow
                    {
                        title = row["JobTitle"]?.ToString() ?? "",
                        avgSalary = row["AvgSalary"] != DBNull.Value ? Convert.ToDecimal(row["AvgSalary"]) : 0,
                        months = row["Month"] != DBNull.Value ? Convert.ToDecimal(row["Month"]) : 0,
                        people = row["PeopleNum"] != DBNull.Value ? Convert.ToDecimal(row["PeopleNum"]) : 0
                    };
                    feeList.Add(fee);
                }
                
                return feeList;
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取其他人事費資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static List<OtherRent> GetOtherObjectFeeList(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT ProjectID, Name, Price, CalDescription
                FROM OFS_SCI_PersonnelCost_OtherObjectFee
                WHERE ProjectID = @ProjectID
                ORDER BY Name";
                
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);
                
                DataTable dt = db.GetTable();
                List<OtherRent> rentList = new List<OtherRent>();
                
                foreach (DataRow row in dt.Rows)
                {
                    var rent = new OtherRent
                    {
                        item = row["Name"]?.ToString() ?? "",
                        amount = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0,
                        note = row["CalDescription"]?.ToString() ?? ""
                    };
                    rentList.Add(rent);
                }
                
                return rentList;
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取其他物件費資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static List<TotalFeeRow> GetTotalFeeList(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT ProjectID, AccountingItem, SubsidyAmount, CoopAmount
                FROM OFS_SCI_PersonnelCost_TotalFee
                WHERE ProjectID = @ProjectID
                ORDER BY AccountingItem";

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);

                DataTable dt = db.GetTable();
                List<TotalFeeRow> totalFeeList = new List<TotalFeeRow>();

                foreach (DataRow row in dt.Rows)
                {
                    var totalFee = new TotalFeeRow
                    {
                        accountingItem = row["AccountingItem"]?.ToString() ?? "",
                        subsidyAmount = row["SubsidyAmount"] != DBNull.Value ? Convert.ToDecimal(row["SubsidyAmount"]) : 0,
                        coopAmount = row["CoopAmount"] != DBNull.Value ? Convert.ToDecimal(row["CoopAmount"]) : 0
                    };
                    totalFeeList.Add(totalFee);
                }

                return totalFeeList;
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取經費總表資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 取得專案的經費總計金額（補助款、配合款、總計）
    /// </summary>
    public static (decimal SubsidyAmount, decimal CoopAmount, decimal TotalAmount) GetTotalFeeSum(string ProjectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                SELECT
                    ISNULL(SUM([SubsidyAmount]), 0) AS SubsidyAmount,
                    ISNULL(SUM([CoopAmount]), 0) AS CoopAmount,
                    ISNULL(SUM([TotalAmount]), 0) AS TotalAmount
                FROM [OFS_SCI_PersonnelCost_TotalFee]
                WHERE ProjectID = @ProjectID
                GROUP BY ProjectID";

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", ProjectID);

                DataTable dt = db.GetTable();

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    decimal subsidyAmount = row["SubsidyAmount"] != DBNull.Value ? Convert.ToDecimal(row["SubsidyAmount"]) : 0;
                    decimal coopAmount = row["CoopAmount"] != DBNull.Value ? Convert.ToDecimal(row["CoopAmount"]) : 0;
                    decimal totalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0;

                    return (subsidyAmount, coopAmount, totalAmount);
                }

                return (0, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception($"讀取經費總計資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    // 輔助方法：將日期轉換為 HTML Date input 所需的格式 (yyyy-MM-dd)
    private static string ConvertDateToString(object dateValue)
    {
        if (dateValue == null || dateValue == DBNull.Value)
            return "";
            
        try
        {
            // 如果已經是字串格式
            if (dateValue is string dateString)
            {
                // 嘗試解析字串日期
                if (DateTime.TryParse(dateString, out DateTime parsedDate))
                {
                    return parsedDate.ToString("yyyy-MM-dd");
                }
                // 如果已經是正確格式，直接返回
                if (System.Text.RegularExpressions.Regex.IsMatch(dateString, @"^\d{4}-\d{2}-\d{2}$"))
                {
                    return dateString;
                }
                return "";
            }
            
            // 如果是 DateTime 類型
            if (dateValue is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd");
            }
            
            // 嘗試轉換為 DateTime
            if (DateTime.TryParse(dateValue.ToString(), out DateTime convertedDate))
            {
                return convertedDate.ToString("yyyy-MM-dd");
            }
            
            return "";
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// 更新 Form3Status
    /// </summary>
    /// <param name="projectId">ProjectID</param>
    /// <param name="status">狀態 (暫存 或 完成)</param>
    public static void UpdateForm3Status(string projectId, string status)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                UPDATE OFS_SCI_Project_Main 
                SET Form3Status = @Status
                WHERE ProjectID = @ProjectId";
                
                db.Parameters.Clear();
                db.Parameters.Add("@Status", status);
                db.Parameters.Add("@ProjectId", projectId);
                
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新 Form3Status 時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 更新 Form3Status 和 CurrentStep
    /// </summary>
    /// <param name="projectId">ProjectID</param>
    /// <param name="status">狀態 (暫存 或 完成)</param>
    /// <param name="currentStep">當前步驟</param>
    public static void UpdateForm3StatusAndCurrentStep(string projectId, string status, string currentStep)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                UPDATE OFS_SCI_Project_Main 
                SET Form3Status = @Status, CurrentStep = @CurrentStep
                WHERE ProjectID = @ProjectId";
                
                db.Parameters.Clear();
                db.Parameters.Add("@Status", status);
                db.Parameters.Add("@CurrentStep", currentStep);
                db.Parameters.Add("@ProjectId", projectId);
                
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新 Form3Status 和 CurrentStep 時發生錯誤: {ex.Message}", ex);
            }
        }
    }

}


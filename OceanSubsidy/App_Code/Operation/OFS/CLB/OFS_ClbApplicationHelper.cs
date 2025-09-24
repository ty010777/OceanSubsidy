using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.App;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// CLB 申請相關的 Helper 類別
/// </summary>
public class OFS_ClbApplicationHelper
{
    public OFS_ClbApplicationHelper()
    {
    }

    /// <summary>
    /// 產生新的 ProjectID
    /// 格式：年份 + "CLB" + 序號（補零到3位數）
    /// 例如：114CLB001, 114CLB002
    /// </summary>
    /// <param name="year">民國年</param>
    /// <returns>新的 ProjectID</returns>
    public static string GenerateNewProjectID(int year)
    {
        try
        {
            DbHelper db = new DbHelper();
            
            // 查詢同年度的最大序號
            db.CommandText = @"
                SELECT MAX([Serial]) 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic] 
                WHERE [Year] = @Year";
            
            db.Parameters.Clear();
            db.Parameters.Add("@Year", year);
            
            DataSet ds = db.GetDataSet();
            
            int nextSerial = 1; // 預設從1開始
            
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0][0];
                if (result != null && result != DBNull.Value)
                {
                    nextSerial = Convert.ToInt32(result) + 1;
                }
            }
            
            // 產生 ProjectID：年份 + "CLB" + 序號（補零到3位數）
            string projectID = $"CLB{year}{nextSerial:D4}";
            
            return projectID;
        }
        catch (Exception ex)
        {
            throw new Exception($"產生 ProjectID 失敗：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 檢查 ProjectID 是否已存在
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>是否存在</returns>
    public static bool CheckProjectIDExists(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT COUNT(*) 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic] 
                WHERE [ProjectID] = @ProjectID";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            
            DataSet ds = db.GetDataSet();
            
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0][0];
                if (result != null && result != DBNull.Value)
                {
                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查 ProjectID 失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 新增基本資料
    /// </summary>
    /// <param name="basicData">基本資料物件</param>
    public static void InsertBasicData(OFS_CLB_Application_Basic basicData)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic]
            (
                [ProjectID],
                [Year],
                [Serial],
                [SubsidyPlanType],
                [ProjectNameTw],
                [SubsidyType],
                [SchoolName],
                [ClubName],
                [CreationDate],
                [School_IDNumber],
                [Address],
                [created_at],
                [updated_at]
            )
            VALUES
            (
                @ProjectID,
                @Year,
                @Serial,
                @SubsidyPlanType,
                @ProjectNameTw,
                @SubsidyType,
                @SchoolName,
                @ClubName,
                @CreationDate,
                @School_IDNumber,
                @Address,
                GETDATE(),
                GETDATE()
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", basicData.ProjectID);
        db.Parameters.Add("@Year", basicData.Year);
        db.Parameters.Add("@Serial", basicData.Serial);
        db.Parameters.Add("@SubsidyPlanType", basicData.SubsidyPlanType ?? "");
        db.Parameters.Add("@ProjectNameTw", basicData.ProjectNameTw ?? "");
        db.Parameters.Add("@SubsidyType", basicData.SubsidyType ?? "");
        db.Parameters.Add("@SchoolName", basicData.SchoolName ?? "");
        db.Parameters.Add("@ClubName", basicData.ClubName ?? "");
        db.Parameters.Add("@CreationDate", basicData.CreationDate);
        db.Parameters.Add("@School_IDNumber", basicData.School_IDNumber ?? "");
        db.Parameters.Add("@Address", basicData.Address ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新基本資料
    /// </summary>
    /// <param name="basicData">基本資料物件</param>
    public static void UpdateBasicData(OFS_CLB_Application_Basic basicData)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic]
            SET 
                [SubsidyPlanType] = @SubsidyPlanType,
                [ProjectNameTw] = @ProjectNameTw,
                [SubsidyType] = @SubsidyType,
                [SchoolName] = @SchoolName,
                [ClubName] = @ClubName,
                [CreationDate] = @CreationDate,
                [School_IDNumber] = @School_IDNumber,
                [Address] = @Address,
                [updated_at] = GETDATE()
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", basicData.ProjectID);
        db.Parameters.Add("@SubsidyPlanType", basicData.SubsidyPlanType ?? "");
        db.Parameters.Add("@ProjectNameTw", basicData.ProjectNameTw ?? "");
        db.Parameters.Add("@SubsidyType", basicData.SubsidyType ?? "");
        db.Parameters.Add("@SchoolName", basicData.SchoolName ?? "");
        db.Parameters.Add("@ClubName", basicData.ClubName ?? "");
        db.Parameters.Add("@CreationDate", basicData.CreationDate);
        db.Parameters.Add("@School_IDNumber", basicData.School_IDNumber ?? "");
        db.Parameters.Add("@Address", basicData.Address ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 取得基本資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>基本資料物件</returns>
    public static OFS_CLB_Application_Basic GetBasicData(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic] 
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_Application_Basic
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Year = row["Year"] != DBNull.Value ? Convert.ToInt32(row["Year"]) : (int?)null,
                    Serial = row["Serial"] != DBNull.Value ? Convert.ToInt32(row["Serial"]) : (int?)null,
                    SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    SubsidyType = row["SubsidyType"]?.ToString(),
                    SchoolName = row["SchoolName"]?.ToString(),
                    ClubName = row["ClubName"]?.ToString(),
                    CreationDate = row["CreationDate"] != DBNull.Value ? Convert.ToDateTime(row["CreationDate"]) : (DateTime?)null,
                    School_IDNumber = row["School_IDNumber"]?.ToString(),
                    Address = row["Address"]?.ToString()
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得基本資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存人員資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="personnelData">人員資料清單</param>
    public static void SavePersonnelData(string projectID, List<OFS_CLB_Application_Personnel> personnelData)
    {
        try
        {
            // 先刪除該計畫的所有人員資料
            DeletePersonnelDataByProjectID(projectID);
            
            // 新增人員資料
            if (personnelData != null && personnelData.Count > 0)
            {
                foreach (var personnel in personnelData)
                {
                    InsertPersonnelData(personnel);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存人員資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 刪除指定計畫的所有人員資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private static void DeletePersonnelDataByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Personnel]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 新增人員資料
    /// </summary>
    /// <param name="personnel">人員資料物件</param>
    private static void InsertPersonnelData(OFS_CLB_Application_Personnel personnel)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Personnel]
            (
                [ProjectID],
                [Personnel],
                [Name],
                [JobTitle],
                [PhoneNum]
            )
            VALUES
            (
                @ProjectID,
                @Personnel,
                @Name,
                @JobTitle,
                @PhoneNum
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", personnel.ProjectID ?? "");
        db.Parameters.Add("@Personnel", personnel.Personnel ?? "");
        db.Parameters.Add("@Name", personnel.Name ?? "");
        db.Parameters.Add("@JobTitle", personnel.JobTitle ?? "");
        db.Parameters.Add("@PhoneNum", personnel.PhoneNum ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 取得指定計畫的人員資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>人員資料清單</returns>
    public static List<OFS_CLB_Application_Personnel> GetPersonnelData(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Personnel] 
                WHERE [ProjectID] = @ProjectID
                ORDER BY [Personnel]";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();
            List<OFS_CLB_Application_Personnel> personnelList = new List<OFS_CLB_Application_Personnel>();

            foreach (DataRow row in dt.Rows)
            {
                personnelList.Add(new OFS_CLB_Application_Personnel
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Personnel = row["Personnel"]?.ToString(),
                    Name = row["Name"]?.ToString(),
                    JobTitle = row["JobTitle"]?.ToString(),
                    PhoneNum = row["PhoneNum"]?.ToString()
                });
            }

            return personnelList;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得人員資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存計畫資訊
    /// </summary>
    /// <param name="planData">計畫資訊物件</param>
    public static void SavePlanData(OFS_CLB_Application_Plan planData)
    {
        try
        {
            // 檢查是否已存在
            bool isUpdate = CheckPlanDataExists(planData.ProjectID);
            
            if (isUpdate)
            {
                UpdatePlanData(planData);
            }
            else
            {
                InsertPlanData(planData);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存計畫資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 檢查計畫資訊是否已存在
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>是否存在</returns>
    private static bool CheckPlanDataExists(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT COUNT(*) 
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Plan] 
            WHERE [ProjectID] = @ProjectID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        
        DataSet ds = db.GetDataSet();
        
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            object result = ds.Tables[0].Rows[0][0];
            if (result != null && result != DBNull.Value)
            {
                int count = Convert.ToInt32(result);
                return count > 0;
            }
        }
        
        return false;
    }

    /// <summary>
    /// 新增計畫資訊
    /// </summary>
    /// <param name="planData">計畫資訊物件</param>
    private static void InsertPlanData(OFS_CLB_Application_Plan planData)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Plan]
            (
                [ProjectID],
                [StartDate],
                [EndDate],
                [Purpose],
                [PlanContent],
                [PreBenefits],
                [PlanLocation],
                [EstimatedPeople],
                [EmergencyPlan]

            )
            VALUES
            (
                @ProjectID,
                @StartDate,
                @EndDate,
                @Purpose,
                @PlanContent,
                @PreBenefits,
                @PlanLocation,
                @EstimatedPeople,
                @EmergencyPlan

            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", planData.ProjectID ?? "");
        db.Parameters.Add("@StartDate", planData.StartDate);
        db.Parameters.Add("@EndDate", planData.EndDate);
        db.Parameters.Add("@Purpose", planData.Purpose ?? "");
        db.Parameters.Add("@PlanContent", planData.PlanContent ?? "");
        db.Parameters.Add("@PreBenefits", planData.PreBenefits ?? "");
        db.Parameters.Add("@PlanLocation", planData.PlanLocation ?? "");
        db.Parameters.Add("@EstimatedPeople", planData.EstimatedPeople ?? "");
        db.Parameters.Add("@EmergencyPlan", planData.EmergencyPlan ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新計畫資訊
    /// </summary>
    /// <param name="planData">計畫資訊物件</param>
    private static void UpdatePlanData(OFS_CLB_Application_Plan planData)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Plan]
            SET 
                [StartDate] = @StartDate,
                [EndDate] = @EndDate,
                [Purpose] = @Purpose,
                [PlanContent] = @PlanContent,
                [PreBenefits] = @PreBenefits,
                [PlanLocation] = @PlanLocation,
                [EstimatedPeople] = @EstimatedPeople,
                [EmergencyPlan] = @EmergencyPlan
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", planData.ProjectID ?? "");
        db.Parameters.Add("@StartDate", planData.StartDate);
        db.Parameters.Add("@EndDate", planData.EndDate);
        db.Parameters.Add("@Purpose", planData.Purpose ?? "");
        db.Parameters.Add("@PlanContent", planData.PlanContent ?? "");
        db.Parameters.Add("@PreBenefits", planData.PreBenefits ?? "");
        db.Parameters.Add("@PlanLocation", planData.PlanLocation ?? "");
        db.Parameters.Add("@EstimatedPeople", planData.EstimatedPeople ?? "");
        db.Parameters.Add("@EmergencyPlan", planData.EmergencyPlan ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 取得計畫資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>計畫資訊物件</returns>
    public static OFS_CLB_Application_Plan GetPlanData(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Plan] 
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_Application_Plan
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    StartDate = row["StartDate"] != DBNull.Value ? Convert.ToDateTime(row["StartDate"]) : (DateTime?)null,
                    EndDate = row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null,
                    Purpose = row["Purpose"]?.ToString(),
                    PlanContent = row["PlanContent"]?.ToString(),
                    PreBenefits = row["PreBenefits"]?.ToString(),
                    PlanLocation = row["PlanLocation"]?.ToString(),
                    EstimatedPeople = row["EstimatedPeople"]?.ToString(),
                    EmergencyPlan = row["EmergencyPlan"]?.ToString()
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得計畫資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存經費資訊
    /// </summary>
    /// <param name="fundsData">經費資訊物件</param>
    public static void SaveFundsData(OFS_CLB_Application_Funds fundsData)
    {
        try
        {
            // 檢查是否已存在
            bool isUpdate = CheckFundsDataExists(fundsData.ProjectID);
            
            if (isUpdate)
            {
                UpdateFundsData(fundsData);
            }
            else
            {
                InsertFundsData(fundsData);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存經費資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 檢查經費資訊是否已存在
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>是否存在</returns>
    private static bool CheckFundsDataExists(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT COUNT(*) 
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Funds] 
            WHERE [ProjectID] = @ProjectID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        
        DataSet ds = db.GetDataSet();
        
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            object result = ds.Tables[0].Rows[0][0];
            if (result != null && result != DBNull.Value)
            {
                int count = Convert.ToInt32(result);
                return count > 0;
            }
        }
        
        return false;
    }

    /// <summary>
    /// 新增經費資訊
    /// </summary>
    /// <param name="fundsData">經費資訊物件</param>
    private static void InsertFundsData(OFS_CLB_Application_Funds fundsData)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Funds]
            (
                [ProjectID],
                [SubsidyFunds],
                [SelfFunds],
                [OtherGovFunds],
                [OtherUnitFunds],
                [PreviouslySubsidized],
                [FundingDescription]
            )
            VALUES
            (
                @ProjectID,
                @SubsidyFunds,
                @SelfFunds,
                @OtherGovFunds,
                @OtherUnitFunds,
                @PreviouslySubsidized,
                @FundingDescription
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", fundsData.ProjectID ?? "");
        db.Parameters.Add("@SubsidyFunds", fundsData.SubsidyFunds ?? 0);
        db.Parameters.Add("@SelfFunds", fundsData.SelfFunds ?? 0);
        db.Parameters.Add("@OtherGovFunds", fundsData.OtherGovFunds ?? 0);
        db.Parameters.Add("@OtherUnitFunds", fundsData.OtherUnitFunds ?? 0);
        db.Parameters.Add("@TotalFunds", fundsData.TotalFunds ?? 0);
        db.Parameters.Add("@PreviouslySubsidized", fundsData.PreviouslySubsidized ?? false);
        db.Parameters.Add("@FundingDescription", fundsData.FundingDescription ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新經費資訊
    /// </summary>
    /// <param name="fundsData">經費資訊物件</param>
    private static void UpdateFundsData(OFS_CLB_Application_Funds fundsData)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Funds]
            SET 
                [SubsidyFunds] = @SubsidyFunds,
                [SelfFunds] = @SelfFunds,
                [OtherGovFunds] = @OtherGovFunds,
                [OtherUnitFunds] = @OtherUnitFunds,
                [PreviouslySubsidized] = @PreviouslySubsidized,
                [FundingDescription] = @FundingDescription
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", fundsData.ProjectID ?? "");
        db.Parameters.Add("@SubsidyFunds", fundsData.SubsidyFunds ?? 0);
        db.Parameters.Add("@SelfFunds", fundsData.SelfFunds ?? 0);
        db.Parameters.Add("@OtherGovFunds", fundsData.OtherGovFunds ?? 0);
        db.Parameters.Add("@OtherUnitFunds", fundsData.OtherUnitFunds ?? 0);
        db.Parameters.Add("@TotalFunds", fundsData.TotalFunds ?? 0);
        db.Parameters.Add("@PreviouslySubsidized", fundsData.PreviouslySubsidized ?? false);
        db.Parameters.Add("@FundingDescription", fundsData.FundingDescription ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 取得經費資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>經費資訊物件</returns>
    public static OFS_CLB_Application_Funds GetFundsData(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Funds] 
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_Application_Funds
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    SubsidyFunds = row["SubsidyFunds"] != DBNull.Value ? Convert.ToDecimal(row["SubsidyFunds"]) : (decimal?)null,
                    SelfFunds = row["SelfFunds"] != DBNull.Value ? Convert.ToDecimal(row["SelfFunds"]) : (decimal?)null,
                    OtherGovFunds = row["OtherGovFunds"] != DBNull.Value ? Convert.ToDecimal(row["OtherGovFunds"]) : (decimal?)null,
                    OtherUnitFunds = row["OtherUnitFunds"] != DBNull.Value ? Convert.ToDecimal(row["OtherUnitFunds"]) : (decimal?)null,
                    TotalFunds = row["TotalFunds"] != DBNull.Value ? Convert.ToDecimal(row["TotalFunds"]) : (decimal?)null,
                    PreviouslySubsidized = row["PreviouslySubsidized"] != DBNull.Value ? Convert.ToBoolean(row["PreviouslySubsidized"]) : (bool?)null,
                    FundingDescription = row["FundingDescription"]?.ToString()
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得經費資訊失敗：{ex.Message}");
        }
    }

 

    /// <summary>
    /// 檢查 Project_Main 資訊是否已存在
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>是否存在</returns>
    public static bool CheckProjectMainDataExists(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT COUNT(*) 
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main] 
            WHERE [ProjectID] = @ProjectID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        
        DataSet ds = db.GetDataSet();
        
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            object result = ds.Tables[0].Rows[0][0];
            if (result != null && result != DBNull.Value)
            {
                int count = Convert.ToInt32(result);
                return count > 0;
            }
        }
        
        return false;
    }

    /// <summary>
    /// 新增 Project_Main 資訊
    /// </summary>
    /// <param name="projectMainData">Project_Main 資訊物件</param>
    public static void InsertProjectMainData(OFS_CLB_Project_Main projectMainData)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
            (
                [ProjectID],
                [Statuses],
                [StatusesName],
                [UserAccount],
                [UserName],
                [UserOrg],
                [CurrentStep],
                [created_at],
                [updated_at],
                [isWithdrawal],
                [isExist]
            )
            VALUES
            (
                @ProjectID,
                @Statuses,
                @StatusesName,
                @UserAccount,
                @UserName,
                @UserOrg,
                @CurrentStep,
                GETDATE(),
                GETDATE(),
                @isWithdrawal,
                @isExist
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectMainData.ProjectID ?? "");
        db.Parameters.Add("@Statuses", projectMainData.Statuses ?? "");
        db.Parameters.Add("@StatusesName", projectMainData.StatusesName ?? "");
        db.Parameters.Add("@UserAccount", projectMainData.UserAccount ?? "");
        db.Parameters.Add("@UserName", projectMainData.UserName ?? "");
        db.Parameters.Add("@UserOrg", projectMainData.UserOrg ?? "");
        db.Parameters.Add("@CurrentStep", projectMainData.CurrentStep ?? "");
        db.Parameters.Add("@isWithdrawal", projectMainData.isWithdrawal ?? false);
        db.Parameters.Add("@isExist", projectMainData.isExist ?? true);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新 Project_Main 資訊
    /// </summary>
    /// <param name="projectMainData">Project_Main 資訊物件</param>
    public static void UpdateProjectMainData(OFS_CLB_Project_Main projectMainData)
    {
        // 先取得目前的狀態
        var currentProjectData = GetProjectMainData(projectMainData.ProjectID);
        
        // 檢查當前狀態是否為不應更新狀態的情況
        bool shouldNotUpdateStatus = false;
        if (currentProjectData != null && !string.IsNullOrEmpty(currentProjectData.Statuses))
        {
            string currentStatus = currentProjectData.Statuses.Trim();
            // 如果當前狀態為"內容審查"或"決審核定"，則不更新狀態
            shouldNotUpdateStatus = currentStatus == "內容審查" || currentStatus == "決審核定" || currentStatus == "計畫執行";
        }
        
        DbHelper db = new DbHelper();
        
        // 根據是否應該更新狀態來決定 SQL 語句
        if (shouldNotUpdateStatus)
        {
            // 不更新 Statuses 和 StatusesName
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
                SET 
                    [UserAccount] = @UserAccount,
                    [UserName] = @UserName,
                    [UserOrg] = @UserOrg,
                    [CurrentStep] = @CurrentStep,
                    [updated_at] = GETDATE(),
                    [isWithdrawal] = @isWithdrawal,
                    [isExist] = @isExist
                WHERE [ProjectID] = @ProjectID";
        }
        else
        {
            // 正常更新所有欄位，包含 Statuses 和 StatusesName
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
                SET 
                    [Statuses] = @Statuses,
                    [StatusesName] = @StatusesName,
                    [UserAccount] = @UserAccount,
                    [UserName] = @UserName,
                    [UserOrg] = @UserOrg,
                    [CurrentStep] = @CurrentStep,
                    [updated_at] = GETDATE(),
                    [isWithdrawal] = @isWithdrawal,
                    [isExist] = @isExist
                WHERE [ProjectID] = @ProjectID";
        }

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectMainData.ProjectID ?? "");
        
        // 只有在需要更新狀態時才加入這些參數
        if (!shouldNotUpdateStatus)
        {
            db.Parameters.Add("@Statuses", projectMainData.Statuses ?? "");
            db.Parameters.Add("@StatusesName", projectMainData.StatusesName ?? "");
        }
        
        db.Parameters.Add("@UserAccount", projectMainData.UserAccount ?? "");
        db.Parameters.Add("@UserName", projectMainData.UserName ?? "");
        db.Parameters.Add("@UserOrg", projectMainData.UserOrg ?? "");
        db.Parameters.Add("@CurrentStep", projectMainData.CurrentStep ?? "");
        db.Parameters.Add("@isWithdrawal", projectMainData.isWithdrawal ?? false);
        db.Parameters.Add("@isExist", projectMainData.isExist ?? true);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 取得 Project_Main 資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>Project_Main 資訊物件</returns>
    public static OFS_CLB_Project_Main GetProjectMainData(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main] 
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_Project_Main
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Statuses = row["Statuses"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    UserAccount = row["UserAccount"]?.ToString(),
                    UserName = row["UserName"]?.ToString(),
                    UserOrg = row["UserOrg"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    CurrentStep = row["CurrentStep"]?.ToString(),
                    created_at = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : (DateTime?)null,
                    updated_at = row["updated_at"] != DBNull.Value ? Convert.ToDateTime(row["updated_at"]) : (DateTime?)null,
                    isWithdrawal = row["isWithdrawal"] != DBNull.Value ? Convert.ToBoolean(row["isWithdrawal"]) : (bool?)null,
                    isExist = row["isExist"] != DBNull.Value ? Convert.ToBoolean(row["isExist"]) : (bool?)null,
                    IsProjChanged = row["IsProjChanged"] != DBNull.Value ? Convert.ToInt32(row["IsProjChanged"]) : 0
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 Project_Main 資訊失敗：{ex.Message}");
        }
    }

    #region 文件上傳相關方法

    /// <summary>
    /// 插入上傳文件記錄
    /// </summary>
    /// <param name="uploadFile">上傳文件物件</param>
    public static void InsertUploadFile(OFS_CLB_UploadFile uploadFile)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile]
                (
                    [ProjectID],
                    [FileCode],
                    [FileName],
                    [TemplatePath]
                )
                VALUES
                (
                    @ProjectID,
                    @FileCode,
                    @FileName,
                    @TemplatePath
                )";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", uploadFile.ProjectID ?? "");
            db.Parameters.Add("@FileCode", uploadFile.FileCode ?? "");
            db.Parameters.Add("@FileName", uploadFile.FileName ?? "");
            db.Parameters.Add("@TemplatePath", uploadFile.TemplatePath ?? "");

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"插入上傳文件記錄失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 刪除上傳文件記錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">文件代碼</param>
    public static void DeleteUploadFile(string projectID, string fileCode)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile]
                WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@FileCode", fileCode);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"刪除上傳文件記錄失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 取得指定計畫的所有上傳文件
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>上傳文件清單</returns>
    public static List<OFS_CLB_UploadFile> GetUploadedFiles(string projectID)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile] 
                WHERE [ProjectID] = @ProjectID
                ORDER BY [FileCode]";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();
            List<OFS_CLB_UploadFile> uploadFiles = new List<OFS_CLB_UploadFile>();

            foreach (DataRow row in dt.Rows)
            {
                uploadFiles.Add(new OFS_CLB_UploadFile
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    FileCode = row["FileCode"]?.ToString(),
                    FileName = row["FileName"]?.ToString(),
                    TemplatePath = row["TemplatePath"]?.ToString()
                });
            }

            return uploadFiles;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得上傳文件清單失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 取得指定計畫和文件代碼的單個上傳文件
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">文件代碼</param>
    /// <returns>上傳文件物件</returns>
    public static OFS_CLB_UploadFile GetUploadedFile(string projectID, string fileCode)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT * 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile] 
                WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@FileCode", fileCode);

            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_CLB_UploadFile
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    ProjectID = row["ProjectID"]?.ToString(),
                    FileCode = row["FileCode"]?.ToString(),
                    FileName = row["FileName"]?.ToString(),
                    TemplatePath = row["TemplatePath"]?.ToString()
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得上傳文件失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 檢查上傳文件是否存在
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="fileCode">文件代碼</param>
    /// <returns>是否存在</returns>
    public static bool CheckUploadFileExists(string projectID, string fileCode)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT COUNT(*) 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile] 
                WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@FileCode", fileCode);

            DataSet ds = db.GetDataSet();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0][0];
                if (result != null && result != DBNull.Value)
                {
                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查上傳文件是否存在失敗：{ex.Message}");
        }
    }

    #endregion

    #region 計畫狀態更新

    /// <summary>
    /// 取得計畫的當前步驟
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>當前步驟，如果計畫不存在則返回空字串</returns>
    public static string GetProjectCurrentStep(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
                return "";

            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT [CurrentStep] 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main] 
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataSet ds = db.GetDataSet();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                object result = ds.Tables[0].Rows[0]["CurrentStep"];
                if (result != null && result != DBNull.Value)
                {
                    return result.ToString();
                }
            }

            return "";
        }
        catch (Exception ex)
        {
            throw new Exception($"取得計畫當前步驟失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新計畫狀態
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="statuses">狀態代碼</param>
    /// <param name="statusesName">狀態名稱</param>
    /// <param name="currentStep">當前步驟</param>
    /// <returns>是否更新成功</returns>
    public static void UpdateProjectStatus(string projectID, string statuses, string statusesName, string currentStep)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main] 
                SET [Statuses] = @Statuses,
                    [StatusesName] = @StatusesName,
                    [CurrentStep] = @CurrentStep,
                    [updated_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@Statuses", statuses);
            db.Parameters.Add("@StatusesName", statusesName);
            db.Parameters.Add("@CurrentStep", currentStep);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計畫狀態失敗：{ex.Message}");
        }
    }

    
    
    /// <summary>
    /// 更新專案承辦人資料（用於案件移轉）
    /// </summary>
    /// <param name="projectID">專案編號</param>
    /// <param name="supervisoryPersonAccount">承辦人員帳號</param>
    /// <param name="supervisoryPersonName">承辦人員姓名</param>
    /// <param name="supervisoryUnit">承辦單位</param>
    public static void UpdateProjectSupervisoryInfo(string projectID, string supervisoryPersonAccount, string supervisoryPersonName, string supervisoryUnit)
    {
        try
        {
            DbHelper db = new DbHelper();
            
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main] 
                SET [SupervisoryPersonAccount] = @SupervisoryPersonAccount,
                    [SupervisoryPersonName] = @SupervisoryPersonName,
                    [SupervisoryUnit] = @SupervisoryUnit,
                    [updated_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@SupervisoryPersonAccount", supervisoryPersonAccount ?? "");
            db.Parameters.Add("@SupervisoryPersonName", supervisoryPersonName ?? "");
            db.Parameters.Add("@SupervisoryUnit", supervisoryUnit ?? "");
            
            db.ExecuteNonQuery();
            
         
        }
        catch (Exception ex)
        {
            throw new Exception($"更新專案承辦人資料時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新計畫變更狀態
    /// </summary>
    /// <param name="projectID">專案編號</param>
    /// <param name="changeStatus">變更狀態 (0=沒有計畫變更申請, 1=計畫變更中, 2=計畫變更審核中)</param>
    public static void UpdateProjectChangeStatus(string projectID, int changeStatus)
    {
        try
        {
            DbHelper db = new DbHelper();

            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
                SET [IsProjChanged] = @IsProjChanged,
                    [updated_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@IsProjChanged", changeStatus);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計畫變更狀態時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新審查結果（用於內容審查）
    /// </summary>
    /// <param name="projectID">專案編號</param>
    /// <param name="statusesName">審查結果狀態名稱</param>
    /// <param name="reviewNotes">審查備註</param>
    /// <param name="expirationDate">補正期限（可選，用於退回補正補件）</param>
    public static void UpdateReviewResult(string projectID, string statusesName, string reviewNotes = null, DateTime? expirationDate = null)
    {
        try
        {
            DbHelper db = new DbHelper();
            
            string sql = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
                SET [StatusesName] = @StatusesName,
                    [updated_at] = GETDATE()";

            // 如果有審查備註，則一併更新
            if (!string.IsNullOrEmpty(reviewNotes))
            {
                sql += ", [QualReviewNotes] = @ReviewNotes";
            }

            // 如果有補正期限，則一併更新
            if (expirationDate.HasValue)
            {
                sql += ", [ExpirationDate] = @ExpirationDate";
            }

            sql += " WHERE [ProjectID] = @ProjectID";
            
            db.CommandText = sql;
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@StatusesName", statusesName ?? "");

            if (!string.IsNullOrEmpty(reviewNotes))
            {
                db.Parameters.Add("@ReviewNotes", reviewNotes);
            }

            if (expirationDate.HasValue)
            {
                db.Parameters.Add("@ExpirationDate", expirationDate.Value);
            }
            
            db.ExecuteNonQuery();
            
           
        }
        catch (Exception ex)
        {
            throw new Exception($"更新審查結果時發生錯誤: {ex.Message}");
        }
    }

    #endregion
}
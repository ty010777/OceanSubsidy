using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// SCI 科專快照相關的 Helper 類別
/// </summary>
public class OFS_SciSnapshotHelper
{
    /// <summary>
    /// 建立 SCI 專案的快照
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    public static void CreateSnapshot(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                throw new ArgumentException("計畫編號不能為空", nameof(projectID));
            }

            // 查詢所有相關資料
            var applicationKeyWord = GetApplicationKeyWord(projectID);
            var applicationMain = GetApplicationMain(projectID);
            var applicationPersonnel = GetApplicationPersonnel(projectID);
            var otherRecused = GetOtherRecused(projectID);
            var otherTechReadiness = GetOtherTechReadiness(projectID);
            var payment = GetPayment(projectID);
            var personnelCostMaterial = GetPersonnelCostMaterial(projectID);
            var personnelCostOtherObjectFee = GetPersonnelCostOtherObjectFee(projectID);
            var personnelCostOtherPersonFee = GetPersonnelCostOtherPersonFee(projectID);
            var personnelCostPersonForm = GetPersonnelCostPersonForm(projectID);
            var personnelCostResearchFees = GetPersonnelCostResearchFees(projectID);
            var personnelCostTotalFee = GetPersonnelCostTotalFee(projectID);
            var personnelCostTripForm = GetPersonnelCostTripForm(projectID);
            var preMonthProgress = GetPreMonthProgress(projectID);
            var projectMain = GetProjectMain(projectID);
            var stageExam = GetStageExam(projectID);
            var stageExamReviewerList = GetStageExamReviewerList(projectID);
            var uploadFile = GetUploadFile(projectID);
            var workSchCheckStandard = GetWorkSchCheckStandard(projectID);
            var workSchMain = GetWorkSchMain(projectID);

            // 根據 Statuses 和 StatusesName 轉換 Status
            int status = ConvertToStatusCode(projectMain?.Statuses, projectMain?.StatusesName);

            // 組合所有資料並序列化為 JSON
            var snapshotData = new
            {
                ApplicationKeyWord = applicationKeyWord,
                ApplicationMain = applicationMain,
                ApplicationPersonnel = applicationPersonnel,
                OtherRecused = otherRecused,
                OtherTechReadiness = otherTechReadiness,
                Payment = payment,
                PersonnelCostMaterial = personnelCostMaterial,
                PersonnelCostOtherObjectFee = personnelCostOtherObjectFee,
                PersonnelCostOtherPersonFee = personnelCostOtherPersonFee,
                PersonnelCostPersonForm = personnelCostPersonForm,
                PersonnelCostResearchFees = personnelCostResearchFees,
                PersonnelCostTotalFee = personnelCostTotalFee,
                PersonnelCostTripForm = personnelCostTripForm,
                PreMonthProgress = preMonthProgress,
                ProjectMain = projectMain,
                StageExam = stageExam,
                StageExamReviewerList = stageExamReviewerList,
                UploadFile = uploadFile,
                WorkSchCheckStandard = workSchCheckStandard,
                WorkSchMain = workSchMain
            };

            string jsonData = JsonConvert.SerializeObject(snapshotData);

            // 建立快照記錄
            OFSSnapshotHelper.insert(new Snapshot
            {
                Type = "SCI",
                DataID = GetProjectMainID(projectID),
                Status = status,
                Data = jsonData
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"建立 SCI 快照失敗：{ex.Message}");
        }
    }

    #region 狀態轉換

    /// <summary>
    /// 將 Statuses 和 StatusesName 轉換為 Status 代碼
    /// </summary>
    /// <param name="statuses">狀態</param>
    /// <param name="statusesName">狀態名稱</param>
    /// <returns>狀態代碼</returns>
    private static int ConvertToStatusCode(string statuses, string statusesName)
    {
        // 處理 null 或空值
        statuses = statuses ?? "";
        statusesName = statusesName ?? "";

        // 尚未提送
        if (statuses == "尚未提送" && statusesName == "編輯中")
            return 1;

        // 資格/內容審查
        if (statuses == "資格審查" || statuses == "內容審查")
        {
            if (statusesName == "審核中") return 11;
            if (statusesName == "通過") return 12;
            if (statusesName == "不通過") return 13;
            if (statusesName == "補正補件") return 14;
            if (statusesName == "逾期未補") return 15;
            if (statusesName == "結案(未通過)") return 19;
        }

        // 實質審查
        if (statuses == "實質審查")
        {
            if (statusesName == "審核中") return 21;
            if (statusesName == "通過") return 22;
            if (statusesName == "不通過") return 23;
            if (statusesName == "結案(未通過)") return 29;
        }

        // 技術審查
        if (statuses == "技術審查")
        {
            if (statusesName == "審核中") return 31;
            if (statusesName == "通過") return 32;
            if (statusesName == "不通過") return 33;
            if (statusesName == "結案(未通過)") return 39;
        }

        // 決審核定
        if (statuses == "決審核定")
        {
            if (statusesName == "核定中") return 41;
            if (statusesName == "計畫書修正中") return 42;
            if (statusesName == "計畫書審核中") return 43;
            if (statusesName == "計畫書已確認") return 44;
            if (statusesName == "已核定") return 45;
            if (statusesName == "不通過") return 46;
            if (statusesName == "結案(未通過)") return 49;
        }

        // 計畫執行
        if (statuses == "計畫執行")
        {
            if (statusesName == "" || statusesName == "審核中") return 51;
            if (statusesName == "通過") return 52;
            if (statusesName == "不通過") return 53;
            if (statusesName == "已結案") return 91;
            if (statusesName == "已終止") return 92;
        }

        // 預設返回 0
        return 0;
    }

    #endregion

    #region 私有查詢方法

    /// <summary>
    /// 取得關鍵字資料
    /// </summary>
    private static List<OFS_SCI_Application_KeyWord> GetApplicationKeyWord(string KeywordID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_KeyWord]
            WHERE [KeywordID] = @KeywordID";

        db.Parameters.Add("@KeywordID", KeywordID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_Application_KeyWord> list = new List<OFS_SCI_Application_KeyWord>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_Application_KeyWord
            {
                Idx = row["Idx"] != DBNull.Value ? Convert.ToInt32(row["Idx"]) : 0,
                KeywordID = row["KeywordID"]?.ToString(),
                KeyWordTw = row["KeyWordTw"]?.ToString(),
                KeyWordEn = row["KeyWordEn"]?.ToString()
            });
        }
        return list;
    }

    /// <summary>
    /// 取得申請表主檔資料
    /// </summary>
    private static OFS_SCI_Application_Main GetApplicationMain(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            return new OFS_SCI_Application_Main
            {
                ProjectID = row["ProjectID"]?.ToString(),
                PersonID = row["PersonID"]?.ToString(),
                Year = row["Year"] != DBNull.Value ? Convert.ToInt32(row["Year"]) : (int?)null,
                Serial = row["Serial"] != DBNull.Value ? Convert.ToInt32(row["Serial"]) : (int?)null,
                SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                ProjectNameEn = row["ProjectNameEn"]?.ToString(),
                OrgCategory = row["OrgCategory"]?.ToString(),
                Topic = row["Topic"]?.ToString(),
                Field = row["Field"]?.ToString(),
                CountryTech_Underwater = row["CountryTech_Underwater"] != DBNull.Value ? Convert.ToBoolean(row["CountryTech_Underwater"]) : (bool?)null,
                CountryTech_Geology = row["CountryTech_Geology"] != DBNull.Value ? Convert.ToBoolean(row["CountryTech_Geology"]) : (bool?)null,
                CountryTech_Physics = row["CountryTech_Physics"] != DBNull.Value ? Convert.ToBoolean(row["CountryTech_Physics"]) : (bool?)null,
                OrgName = row["OrgName"]?.ToString(),
                OrgPartner = row["OrgPartner"]?.ToString(),
                RegisteredAddress = row["RegisteredAddress"]?.ToString(),
                CorrespondenceAddress = row["CorrespondenceAddress"]?.ToString(),
                Target = row["Target"]?.ToString(),
                Summary = row["Summary"]?.ToString(),
                Innovation = row["Innovation"]?.ToString(),
                Declaration = row["Declaration"] != DBNull.Value ? Convert.ToBoolean(row["Declaration"]) : (bool?)null,
                IsRecused = row["IsRecused"] != DBNull.Value ? Convert.ToBoolean(row["IsRecused"]) : (bool?)null,
                StartTime = row["StartTime"] != DBNull.Value ? Convert.ToDateTime(row["StartTime"]) : (DateTime?)null,
                EndTime = row["EndTime"] != DBNull.Value ? Convert.ToDateTime(row["EndTime"]) : (DateTime?)null,
                created_at = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : (DateTime?)null,
                updated_at = row["updated_at"] != DBNull.Value ? Convert.ToDateTime(row["updated_at"]) : (DateTime?)null,
                isExist = row["isExist"] != DBNull.Value ? Convert.ToBoolean(row["isExist"]) : (bool?)null
            };
        }
        return null;
    }

    /// <summary>
    /// 取得申請人員資料
    /// </summary>
    private static List<OFS_SCI_Application_Personnel> GetApplicationPersonnel(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Personnel]
            WHERE [PersonID] = @PersonID
            ORDER BY [Role]";

        db.Parameters.Add("@PersonID", "P"+projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_Application_Personnel> list = new List<OFS_SCI_Application_Personnel>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_Application_Personnel
            {
                idx = row["idx"] != DBNull.Value ? Convert.ToInt32(row["idx"]) : 0, 
                PersonID = row["PersonID"]?.ToString(),
                Role = row["Role"]?.ToString(),
                Name = row["Name"]?.ToString(),
                JobTitle = row["JobTitle"]?.ToString(),
                Phone = row["Phone"]?.ToString(),
                PhoneExt = row["PhoneExt"]?.ToString(),
                MobilePhone = row["MobilePhone"]?.ToString(),
                
            });
        }
        return list;
    }

    /// <summary>
    /// 取得其他迴避委員資料
    /// </summary>
    private static List<OFS_SCI_Other_Recused> GetOtherRecused(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Other_Recused]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        
        List<OFS_SCI_Other_Recused> list = new List<OFS_SCI_Other_Recused>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_Other_Recused
            {
                ProjectID = row["ProjectID"].ToString(), 
                RecusedName = row["RecusedName"]?.ToString(),
                EmploymentUnit = row["EmploymentUnit"]?.ToString(),
                JobTitle = row["JobTitle"]?.ToString(),
                RecusedReason = row["RecusedReason"]?.ToString(),
            });
        }
        return list;
    }

    /// <summary>
    /// 取得技術準備度資料
    /// </summary>
    private static List<OFS_SCI_Other_TechReadiness> GetOtherTechReadiness(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Other_TechReadiness]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();
        
        List<OFS_SCI_Other_TechReadiness> list = new List<OFS_SCI_Other_TechReadiness>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_Other_TechReadiness
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Name = row["Name"]?.ToString(),
                Bef_TRLevel = row["Bef_TRLevel"]?.ToString(),
                Aft_TRLevel = row["Aft_TRLevel"]?.ToString(),
                Description = row["Description"]?.ToString(),
                
            });
        }
        return list;
       
    }
    

    /// <summary>
    /// 取得請款資料
    /// </summary>
    private static List<OFS_SCI_Payment> GetPayment(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Payment]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [Stage]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_Payment> list = new List<OFS_SCI_Payment>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_Payment
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Stage = row["Stage"] != DBNull.Value ? Convert.ToInt32(row["Stage"]) : (int?)null,
                ActDisbursementRatioPct = row["ActDisbursementRatioPct"] != DBNull.Value ? Convert.ToDecimal(row["ActDisbursementRatioPct"]) : (decimal?)null,
                TotalSpentAmount = row["TotalSpentAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalSpentAmount"]) : (decimal?)null,
                CurrentRequestAmount = row["CurrentRequestAmount"] != DBNull.Value ? Convert.ToDecimal(row["CurrentRequestAmount"]) : (decimal?)null,
                CurrentActualPaidAmount = row["CurrentActualPaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["CurrentActualPaidAmount"]) : (decimal?)null,
                Status = row["Status"]?.ToString(),
                ReviewerComment = row["ReviewerComment"]?.ToString(),
                ReviewUser = row["ReviewUser"]?.ToString(),
                ReviewTime = row["ReviewTime"] != DBNull.Value ? Convert.ToDateTime(row["ReviewTime"]) : (DateTime?)null,
                CreateTime = row["CreateTime"] != DBNull.Value ? Convert.ToDateTime(row["CreateTime"]) : (DateTime?)null,
                UpdateTime = row["UpdateTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdateTime"]) : (DateTime?)null
            });
        }
        return list;
    }

    /// <summary>
    /// 取得人事費-材料費資料
    /// </summary>
    private static List<OFS_SCI_PersonnelCost_Material> GetPersonnelCostMaterial(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_Material]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [ItemName]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PersonnelCost_Material> list = new List<OFS_SCI_PersonnelCost_Material>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PersonnelCost_Material
            {
                ProjectID = row["ProjectID"]?.ToString(),
                ItemName = row["ItemName"]?.ToString(),
                Description = row["Description"]?.ToString(),
                Unit = row["Unit"]?.ToString(),
                PreNum = row["PreNum"] != DBNull.Value ? Convert.ToDecimal(row["PreNum"]) : (decimal?)null,
                UnitPrice = row["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(row["UnitPrice"]) : (decimal?)null
            });
        }

        return list;
    }


    /// <summary>
    /// 取得人事費-其他業務費資料
    /// </summary>
    private static List<OFS_SCI_PersonnelCost_OtherObjectFee> GetPersonnelCostOtherObjectFee(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_OtherObjectFee]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [Name]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PersonnelCost_OtherObjectFee> list = new List<OFS_SCI_PersonnelCost_OtherObjectFee>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PersonnelCost_OtherObjectFee
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Name = row["Name"]?.ToString(),
                Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : (decimal?)null,
                CalDescription = row["CalDescription"]?.ToString()
            });
        }

        return list;
    }


    /// <summary>
    /// 取得人事費-其他人事費資料
    /// </summary>
    private static List<OFS_SCI_PersonnelCost_OtherPersonFee> GetPersonnelCostOtherPersonFee(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_OtherPersonFee]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [JobTitle]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PersonnelCost_OtherPersonFee> list = new List<OFS_SCI_PersonnelCost_OtherPersonFee>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PersonnelCost_OtherPersonFee
            {
                ProjectID = row["ProjectID"]?.ToString(),
                JobTitle = row["JobTitle"]?.ToString(),
                AvgSalary = row["AvgSalary"] != DBNull.Value ? Convert.ToDecimal(row["AvgSalary"]) : (decimal?)null,
                Month = row["Month"] != DBNull.Value ? Convert.ToInt32(row["Month"]) : (int?)null,
                PeopleNum = row["PeopleNum"] != DBNull.Value ? Convert.ToInt32(row["PeopleNum"]) : (int?)null
            });
        }

        return list;
    }


    /// <summary>
    /// 取得人事費-人員表單資料
    /// </summary>
    private static List<OFS_SCI_PersonnelCost_PersonForm> GetPersonnelCostPersonForm(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_PersonForm]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [Name]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PersonnelCost_PersonForm> list = new List<OFS_SCI_PersonnelCost_PersonForm>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PersonnelCost_PersonForm
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Name = row["Name"]?.ToString(),
                JobTitle = row["JobTitle"]?.ToString(),
                AvgSalary = row["AvgSalary"] != DBNull.Value ? Convert.ToDecimal(row["AvgSalary"]) : (decimal?)null,
                Month = row["Month"] != DBNull.Value ? Convert.ToInt32(row["Month"]) : (int?)null,
                IsPending = row["IsPending"] != DBNull.Value ? Convert.ToBoolean(row["IsPending"]) : (bool?)null
            });
        }

        return list;
    }


    /// <summary>
    /// 取得人事費-研究費資料
    /// </summary>
    private static List<OFS_SCI_PersonnelCost_ResearchFees> GetPersonnelCostResearchFees(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_ResearchFees]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [FeeCategory], [Name]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PersonnelCost_ResearchFees> list = new List<OFS_SCI_PersonnelCost_ResearchFees>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PersonnelCost_ResearchFees
            {
                ProjectID = row["ProjectID"]?.ToString(),
                FeeCategory = row["FeeCategory"]?.ToString(),
                StartDate = row["StartDate"] != DBNull.Value ? Convert.ToDateTime(row["StartDate"]) : (DateTime?)null,
                EndDate = row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null,
                Name = row["Name"]?.ToString(),
                PersonName = row["PersonName"]?.ToString(),
                Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : (decimal?)null
            });
        }

        return list;
    }

    /// <summary>
    /// 取得人事費-總費用資料
    /// </summary>
    private static List<OFS_SCI_PersonnelCost_TotalFee> GetPersonnelCostTotalFee(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_TotalFee]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [AccountingItem]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PersonnelCost_TotalFee> list = new List<OFS_SCI_PersonnelCost_TotalFee>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PersonnelCost_TotalFee
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                AccountingItem = row["AccountingItem"]?.ToString(),
                SubsidyAmount = row["SubsidyAmount"] != DBNull.Value ? Convert.ToDecimal(row["SubsidyAmount"]) : (decimal?)null,
                CoopAmount = row["CoopAmount"] != DBNull.Value ? Convert.ToDecimal(row["CoopAmount"]) : (decimal?)null,
                TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : (decimal?)null
            });
        }

        return list;
    }

    /// <summary>
    /// 取得人事費-差旅表單資料
    /// </summary>
    private static List<OFS_SCI_PersonnelCost_TripForm> GetPersonnelCostTripForm(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_TripForm]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [TripReason], [Area]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PersonnelCost_TripForm> list = new List<OFS_SCI_PersonnelCost_TripForm>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PersonnelCost_TripForm
            {
                ProjectID = row["ProjectID"]?.ToString(),
                TripReason = row["TripReason"]?.ToString(),
                Area = row["Area"]?.ToString(),
                Days = row["Days"] != DBNull.Value ? Convert.ToInt32(row["Days"]) : (int?)null,
                Times = row["Times"] != DBNull.Value ? Convert.ToInt32(row["Times"]) : (int?)null,
                Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : (decimal?)null
            });
        }

        return list;
    }


    /// <summary>
    /// 取得預期月進度資料
    /// </summary>
    private static List<OFS_SCI_PreMonthProgress> GetPreMonthProgress(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [Month]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_PreMonthProgress> list = new List<OFS_SCI_PreMonthProgress>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_PreMonthProgress
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Month = row["Month"] .ToString(),
                PreWorkAbstract = row["PreWorkAbstract"]?.ToString(),
                ActWorkAbstract = row["ActWorkAbstract"]?.ToString(),
                CheckDescription = row["CheckDescription"]?.ToString(),
                PreProgress = row["PreProgress"] != DBNull.Value ? Convert.ToDecimal(row["PreProgress"]) : (decimal?)null,
                ActProgress = row["ActProgress"] != DBNull.Value ? Convert.ToDecimal(row["ActProgress"]) : (decimal?)null,
                MonthlySubsidy = row["MonthlySubsidy"] != DBNull.Value ? Convert.ToDecimal(row["MonthlySubsidy"]) : (decimal?)null,
                MonthlyCoop = row["MonthlyCoop"] != DBNull.Value ? Convert.ToDecimal(row["MonthlyCoop"]) : (decimal?)null,
                MonthlyTotal = row["MonthlyTotal"] != DBNull.Value ? Convert.ToDecimal(row["MonthlyTotal"]) : (decimal?)null
            });
        }

        return list;
    }


    /// <summary>
    /// 取得專案主檔資料
    /// </summary>
    private static OFS_SCI_Project_Main GetProjectMain(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            return new OFS_SCI_Project_Main
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Statuses = row["Statuses"]?.ToString(),
                StatusesName = row["StatusesName"]?.ToString(),
                ExpirationDate = row["ExpirationDate"] != DBNull.Value ? Convert.ToDateTime(row["ExpirationDate"]) : (DateTime?)null,
                SeqPoint = row["SeqPoint"] != DBNull.Value ? Convert.ToDecimal(row["SeqPoint"]) : (decimal?)null,
                SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                UserAccount = row["UserAccount"]?.ToString(),
                UserName = row["UserName"]?.ToString(),
                UserOrg = row["UserOrg"]?.ToString(),
                Form1Status = row["Form1Status"]?.ToString(),
                Form2Status = row["Form2Status"]?.ToString(),
                Form3Status = row["Form3Status"]?.ToString(),
                Form4Status = row["Form4Status"]?.ToString(),
                Form5Status = row["Form5Status"]?.ToString(),
                CurrentStep = row["CurrentStep"]?.ToString(),
                isWithdrawal = row["isWithdrawal"] != DBNull.Value ? Convert.ToBoolean(row["isWithdrawal"]) : (bool?)null,
                isExist = row["isExist"] != DBNull.Value ? Convert.ToBoolean(row["isExist"]) : (bool?)null,
                ApprovedSubsidy = row["ApprovedSubsidy"] != DBNull.Value ? Convert.ToDouble(row["ApprovedSubsidy"]) : (double?)null,
                QualReviewNotes = row["QualReviewNotes"]?.ToString(),
                FinalReviewNotes = row["FinalReviewNotes"]?.ToString(),
                FinalReviewOrder = row["FinalReviewOrder"] != DBNull.Value ? Convert.ToInt32(row["FinalReviewOrder"]) : (int?)null,
                MidtermExamDate = row["MidtermExamDate"] != DBNull.Value ? Convert.ToDateTime(row["MidtermExamDate"]) : (DateTime?)null,
                FinalExamDate = row["FinalExamDate"] != DBNull.Value ? Convert.ToDateTime(row["FinalExamDate"]) : (DateTime?)null,
                PubNumber = row["PubNumber"]?.ToString(),
                ContractDate = row["ContractDate"] != DBNull.Value ? Convert.ToDateTime(row["ContractDate"]) : (DateTime?)null,
                LastOperation = row["LastOperation"]?.ToString(),
                IsProjChanged = row["IsProjChanged"] != DBNull.Value ? Convert.ToInt32(row["IsProjChanged"]) : (int?)null,
                updated_at = row["updated_at"] != DBNull.Value ? Convert.ToDateTime(row["updated_at"]) : (DateTime?)null,
                created_at = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : (DateTime?)null
            };
        }
        return null;
    }

    /// <summary>
    /// 取得階段審查資料
    /// </summary>
    private static List<OFS_SCI_StageExam> GetStageExam(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_StageExam]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [Stage], [ID]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_StageExam> list = new List<OFS_SCI_StageExam>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_StageExam
            {
                id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Stage = row["Stage"] != DBNull.Value ? Convert.ToInt32(row["Stage"]) : (int?)null,
                ExamVersion = row["ExamVersion"] != DBNull.Value ? Convert.ToInt32(row["ExamVersion"]) : 0,
                ReviewMethod = row["ReviewMethod"]?.ToString(),
                Status = row["Status"]?.ToString(),
                Reviewer = row["Reviewer"]?.ToString(),
                Account = row["Account"]?.ToString(),
                create_at = row["create_at"] != DBNull.Value ? Convert.ToDateTime(row["create_at"]) : (DateTime?)null,
                update_at = row["update_at"] != DBNull.Value ? Convert.ToDateTime(row["update_at"]) : (DateTime?)null
            });
        }

        return list;
    }


    /// <summary>
    /// 取得階段審查委員名單資料
    /// </summary>
    private static List<OFS_SCI_StageExam_ReviewerList> GetStageExamReviewerList(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT rl.*
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_StageExam_ReviewerList] rl
            LEFT JOIN [OCA_OceanSubsidy].[dbo].[OFS_SCI_StageExam] se
                ON rl.ExamID = se.id
            WHERE se.ProjectID = @ProjectID
            ORDER BY rl.id";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_StageExam_ReviewerList> list = new List<OFS_SCI_StageExam_ReviewerList>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_StageExam_ReviewerList
            {
                id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
                ExamID = row["ExamID"] != DBNull.Value ? Convert.ToInt32(row["ExamID"]) : (int?)null,
                Account = row["Account"]?.ToString(),
                Reviewer = row["Reviewer"]?.ToString(),
                ReviewFilePath = row["ReviewFilePath"]?.ToString(),
                token = row["token"]?.ToString(),
                isSubmit = row["isSubmit"] != DBNull.Value ? Convert.ToBoolean(row["isSubmit"]) : (bool?)null,
                BankCode = row["BankCode"]?.ToString(),
                BankAccount = row["BankAccount"]?.ToString(),
                RegistrationAddress = row["RegistrationAddress"]?.ToString()
            });
        }
        return list;
    }

    /// <summary>
    /// 取得上傳文件資料
    /// </summary>
    private static List<OFS_SCI_UploadFile> GetUploadFile(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_UploadFile]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [FileCode]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_UploadFile> list = new List<OFS_SCI_UploadFile>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_UploadFile
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                FileCode = row["FileCode"]?.ToString(),
                FileName = row["FileName"]?.ToString(),
                TemplatePath = row["TemplatePath"]?.ToString()
            });
        }
        return list;
    }

    /// <summary>
    /// 取得工作項目檢核標準資料
    /// </summary>
    private static List<OFS_SCI_WorkSch_CheckStandard> GetWorkSchCheckStandard(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_CheckStandard]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [SerialNumber]";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_WorkSch_CheckStandard> list = new List<OFS_SCI_WorkSch_CheckStandard>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_WorkSch_CheckStandard
            {
                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                WorkItem = row["WorkItem"]?.ToString(),
                SerialNumber = row["SerialNumber"] ?.ToString(),
                PlannedFinishDate = row["PlannedFinishDate"] != DBNull.Value ? Convert.ToDateTime(row["PlannedFinishDate"]) : (DateTime?)null,
                CheckDescription = row["CheckDescription"]?.ToString(),
                ActFinishTime = row["ActFinishTime"] != DBNull.Value ? Convert.ToDateTime(row["ActFinishTime"]) : (DateTime?)null,
                DelayReason = row["DelayReason"]?.ToString(),
                ImprovedWay = row["ImprovedWay"]?.ToString(),
                IsFinish = row["IsFinish"] != DBNull.Value ? Convert.ToInt32(row["IsFinish"]) : 0, 
                CreatedAt =  Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = row["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedAt"]) : (DateTime?)null
            });
        }

        return list;
    }

    /// <summary>
    /// 取得工作項目主檔資料
    /// </summary>
    private static List<OFS_SCI_WorkSch_Main> GetWorkSchMain(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_Main]
        WHERE [ProjectID] = @ProjectID
        ORDER BY [WorkItem_id]";
    
        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_SCI_WorkSch_Main> list = new List<OFS_SCI_WorkSch_Main>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_SCI_WorkSch_Main
            {
                ProjectID = row["ProjectID"]?.ToString(),
                WorkItem_id = row["WorkItem_id"]?.ToString(),
                WorkName = row["WorkName"]?.ToString(),
                StartYear = row["StartYear"] != DBNull.Value ? Convert.ToInt32(row["StartYear"]) : (int?)null,
                StartMonth = row["StartMonth"] != DBNull.Value ? Convert.ToInt32(row["StartMonth"]) : (int?)null,
                EndYear = row["EndYear"] != DBNull.Value ? Convert.ToInt32(row["EndYear"]) : (int?)null,
                EndMonth = row["EndMonth"] != DBNull.Value ? Convert.ToInt32(row["EndMonth"]) : (int?)null,
                Weighting = row["Weighting"] != DBNull.Value ? Convert.ToDecimal(row["Weighting"]) : (decimal?)null,
                InvestMonth = row["InvestMonth"] != DBNull.Value ? Convert.ToDecimal(row["InvestMonth"]) : (decimal?)null,
                IsOutsourced = row["IsOutsourced"] != DBNull.Value ? Convert.ToBoolean(row["IsOutsourced"]) : (bool?)null
            });
        }

        return list;
    }


    /// <summary>
    /// 取得 Project_Main 的 ID (用於快照的 DataID)
    /// </summary>
    private static int GetProjectMainID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [ID]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);
        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        if (dt.Rows.Count > 0 && dt.Rows[0]["ID"] != DBNull.Value)
        {
            return Convert.ToInt32(dt.Rows[0]["ID"]);
        }

        // 如果找不到記錄，返回 0
        return 0;
    }

    #endregion
}

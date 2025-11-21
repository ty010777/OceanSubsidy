using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/// <summary>
/// CLB 快照相關的 Helper 類別
/// </summary>
public class OFS_ClbSnapshotHelper
{
    /// <summary>
    /// 建立 CLB 專案的快照
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
            var basicData = GetBasicData(projectID);
            var personnelData = GetPersonnelData(projectID);
            var planData = GetPlanData(projectID);
            var paymentData = GetPaymentData(projectID);
            var projectMainData = GetProjectMainData(projectID);
            var stageExamData = GetStageExamData(projectID);
            var uploadFileData = GetUploadFileData(projectID);
            var otherSubsidyData = GetOtherSubsidyData(projectID);
            var budgetPlanData = GetBudgetPlanData(projectID);
            var receivedSubsidyData = GetReceivedSubsidyData(projectID);

            // 根據 Statuses 和 StatusesName 轉換 Status
            int status = ConvertToStatusCode(projectMainData?.Statuses, projectMainData?.StatusesName);

            // 組合所有資料並序列化為 JSON
            var snapshotData = new
            {
                ApplicationBasic = basicData,
                ApplicationPersonnel = personnelData,
                ApplicationPlan = planData,
                Payment = paymentData,
                ProjectMain = projectMainData,
                StageExam = stageExamData,
                UploadFile = uploadFileData,
                OtherSubsidy = otherSubsidyData,
                BudgetPlan = budgetPlanData,
                ReceivedSubsidy = receivedSubsidyData
            };

            string jsonData = JsonConvert.SerializeObject(snapshotData);

            // 建立快照記錄
            OFSSnapshotHelper.insert(new Snapshot
            {
                Type = "CLB",
                DataID = GetProjectMainID(projectID),
                Status = status,
                Data = jsonData
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"建立 CLB 快照失敗：{ex.Message}");
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
    /// 取得基本資料
    /// </summary>
    private static OFS_CLB_Application_Basic GetBasicData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

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
                Address = row["Address"]?.ToString(),
                ApplyAmount = row["ApplyAmount"] != DBNull.Value ? Convert.ToInt32(row["ApplyAmount"]) : (int?)null,
                SelfAmount = row["SelfAmount"] != DBNull.Value ? Convert.ToInt32(row["SelfAmount"]) : (int?)null,
                OtherAmount = row["OtherAmount"] != DBNull.Value ? Convert.ToInt32(row["OtherAmount"]) : (int?)null,
                IsPreviouslySubsidized = row["IsPreviouslySubsidized"] != DBNull.Value ? Convert.ToBoolean(row["IsPreviouslySubsidized"]) : (bool?)null
            };
        }

        return null;
    }
    

    /// <summary>
    /// 取得人員資料
    /// </summary>
    private static List<OFS_CLB_Application_Personnel> GetPersonnelData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Personnel]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [Personnel]";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

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

    /// <summary>
    /// 取得計畫資訊
    /// </summary>
    private static OFS_CLB_Application_Plan GetPlanData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Plan]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

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

    /// <summary>
    /// 取得請款資料
    /// </summary>
    private static List<OFS_CLB_Payment> GetPaymentData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Payment]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [Stage]";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_CLB_Payment> paymentList = new List<OFS_CLB_Payment>();

        foreach (DataRow row in dt.Rows)
        {
            paymentList.Add(new OFS_CLB_Payment
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Stage = row["Stage"] != DBNull.Value ? Convert.ToInt32(row["Stage"]) : (int?)null,
                CurrentRequestAmount = row["CurrentRequestAmount"] != DBNull.Value ? Convert.ToDecimal(row["CurrentRequestAmount"]) : (decimal?)null,
                TotalSpentAmount = row["TotalSpentAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalSpentAmount"]) : (decimal?)null,
                CurrentActualPaidAmount = row["CurrentActualPaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["CurrentActualPaidAmount"]) : (decimal?)null,
                Status = row["Status"]?.ToString(),
                ReviewerComment = row["ReviewerComment"]?.ToString(),
                ReviewUser = row["ReviewUser"]?.ToString(),
                ReviewTime = row["ReviewTime"] != DBNull.Value ? Convert.ToDateTime(row["ReviewTime"]) : (DateTime?)null,
                CreateTime = row["CreateTime"] != DBNull.Value ? Convert.ToDateTime(row["CreateTime"]) : (DateTime?)null,
                UpdateTime = row["UpdateTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdateTime"]) : (DateTime?)null
            });
        }

        return paymentList;
    }

    /// <summary>
    /// 取得專案主檔資料
    /// </summary>
    private static OFS_CLB_Project_Main GetProjectMainData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

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

    /// <summary>
    /// 取得階段審查資料
    /// </summary>
    private static OFS_CLB_StageExam GetStageExamData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_StageExam]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            return new OFS_CLB_StageExam
            {
                id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Status = row["Status"]?.ToString(),
                Reviewer = row["Reviewer"]?.ToString(),
                Account = row["Account"]?.ToString(),
                create_at = row["create_at"] != DBNull.Value ? Convert.ToDateTime(row["create_at"]) : (DateTime?)null,
                update_at = row["update_at"] != DBNull.Value ? Convert.ToDateTime(row["update_at"]) : (DateTime?)null
            };
        }

        return null;
    }

    /// <summary>
    /// 取得上傳文件資料
    /// </summary>
    private static List<OFS_CLB_UploadFile> GetUploadFileData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_UploadFile]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [FileCode]";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

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

    /// <summary>
    /// 取得 Project_Main 的 ID (用於快照的 DataID)
    /// </summary>
    private static int GetProjectMainID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [ID]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
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

    /// <summary>
    /// 取得其他補助資料
    /// </summary>
    private static List<OFS_CLB_Other_Subsidy> GetOtherSubsidyData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Other_Subsidy]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [ID]";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_CLB_Other_Subsidy> otherSubsidyList = new List<OFS_CLB_Other_Subsidy>();

        foreach (DataRow row in dt.Rows)
        {
            otherSubsidyList.Add(new OFS_CLB_Other_Subsidy
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Unit = row["Unit"]?.ToString(),
                Amount = row["Amount"] != DBNull.Value ? Convert.ToInt32(row["Amount"]) : (int?)null,
                Content = row["Content"]?.ToString()
            });
        }

        return otherSubsidyList;
    }

    /// <summary>
    /// 取得經費預算規劃資料
    /// </summary>
    private static List<OFS_CLB_Budget_Plan> GetBudgetPlanData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [ID]";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_CLB_Budget_Plan> budgetPlanList = new List<OFS_CLB_Budget_Plan>();

        foreach (DataRow row in dt.Rows)
        {
            budgetPlanList.Add(new OFS_CLB_Budget_Plan
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Title = row["Title"]?.ToString(),
                Amount = row["Amount"] != DBNull.Value ? Convert.ToInt32(row["Amount"]) : (int?)null,
                OtherAmount = row["OtherAmount"] != DBNull.Value ? Convert.ToInt32(row["OtherAmount"]) : (int?)null,
                Description = row["Description"]?.ToString()
            });
        }

        return budgetPlanList;
    }

    /// <summary>
    /// 取得已獲補助資料
    /// </summary>
    private static List<OFS_CLB_Received_Subsidy> GetReceivedSubsidyData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT *
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [ID]";

        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        db.Parameters.Clear();

        List<OFS_CLB_Received_Subsidy> receivedSubsidyList = new List<OFS_CLB_Received_Subsidy>();

        foreach (DataRow row in dt.Rows)
        {
            receivedSubsidyList.Add(new OFS_CLB_Received_Subsidy
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                ProjectID = row["ProjectID"]?.ToString(),
                Name = row["Name"]?.ToString(),
                Amount = row["Amount"] != DBNull.Value ? Convert.ToInt32(row["Amount"]) : 0
            });
        }

        return receivedSubsidyList;
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using DataTable = DocumentFormat.OpenXml.Drawing.Charts.DataTable;

/// <summary>
/// 科專排程任務處理 Helper - 僅處理 SQL 操作
/// </summary>
public class OFS_ScienceTaskHelper
{
    /// <summary>
    /// 取得所有活躍的專案ID
    /// </summary>
    /// <returns>專案ID清單</returns>
    public static List<string> GetActiveProjectIds()
    {
        var projectIds = new List<string>();
        
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT DISTINCT [ProjectID]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                WHERE [isExist] = 1 
                  AND [isWithdrawal] = 0
                  AND [Statuses] = '計畫執行'
                  AND [StatusesName] NOT LIKE '%終止%'
                  AND [StatusesName] NOT LIKE '%結案%'
            ";
            
            var dt = db.GetTable();
            foreach (DataRow row in dt.Rows)
            {
                var projectId = row["ProjectID"]?.ToString();
                if (!string.IsNullOrEmpty(projectId))
                {
                    projectIds.Add(projectId);
                }
            }
        }
        finally
        {
            db.Dispose();
        }
        
        return projectIds;
    }

    /// <summary>
    /// 檢查任務是否已存在
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <returns>是否存在</returns>
    public static bool TaskExists(string projectId, string taskNameEn)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT COUNT(1)
                FROM [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                WHERE [ProjectID] = @ProjectID 
                AND [TaskNameEn] = @TaskNameEn";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@TaskNameEn", taskNameEn);
            
            var countResult = db.GetTable();
            return countResult.Rows.Count > 0 && Convert.ToInt32(countResult.Rows[0][0]) > 0;
        }
        finally
        {
            db.Dispose();
        }
    }
    /// <summary>
    /// gets the task 
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <returns>是否存在</returns>
    public static OFS_TaskQueue getTask(string projectId, string taskNameEn)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT TOP(1)*
                FROM [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                WHERE [ProjectID] = @ProjectID 
                AND [TaskNameEn] = @TaskNameEn";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@TaskNameEn", taskNameEn);

            var TaskQueue = new OFS_TaskQueue();
            var countResult = db.GetTable();

            if (countResult != null && countResult.Rows.Count > 0)
            {
                DataRow row = countResult.Rows[0];

                TaskQueue.ProjectID = row["ProjectID"]?.ToString();
                TaskQueue.TaskNameEn = row["TaskNameEn"]?.ToString();
                TaskQueue.TaskName = row["TaskName"]?.ToString();
                TaskQueue.PriorityLevel = row["PriorityLevel"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["PriorityLevel"]);
                TaskQueue.IsTodo = row["IsTodo"] == DBNull.Value ? null : (bool?)Convert.ToBoolean(row["IsTodo"]);
                TaskQueue.IsCompleted = row["IsCompleted"] == DBNull.Value ? null : (bool?)Convert.ToBoolean(row["IsCompleted"]);
                TaskQueue.OverdueDate = row["OverdueDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["OverdueDate"]);
            }
            
            return TaskQueue;
        }
        finally
        {
            db.Dispose();
        }
    }
    /// <summary>
    /// 更新現有任務狀態
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <param name="isTodo">是否為待辦</param>
    /// <param name="overdueDate">逾期日期（可選）</param>
    public static void UpdateTaskStatus(string projectId, string taskNameEn, bool isTodo, DateTime? overdueDate = null)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                SET [IsTodo] = @IsTodo,
                    [OverdueDate] = @OverdueDate
                WHERE [ProjectID] = @ProjectID
                AND [TaskNameEn] = @TaskNameEn";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@TaskNameEn", taskNameEn);
            db.Parameters.Add("@IsTodo", isTodo ? 1 : 0);
            db.Parameters.Add("@OverdueDate", overdueDate.HasValue ? (object)overdueDate.Value : DBNull.Value);

            db.ExecuteNonQuery();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查期中報告期限
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>Tuple(是否需要填寫期中報告, 期中審查預定日期)</returns>
    public static Tuple<bool, DateTime?> CheckMidReportDeadline(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT [MidtermExamDate]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                WHERE [ProjectID] = @ProjectID
                AND [isExist] = 1";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            var dt = db.GetTable();
            if (dt.Rows.Count > 0 && dt.Rows[0]["MidtermExamDate"] != DBNull.Value)
            {
                var midtermExamDate = Convert.ToDateTime(dt.Rows[0]["MidtermExamDate"]);
                var oneMonthBefore = midtermExamDate.AddMonths(-1);
                bool isTodo = DateTime.Now >= oneMonthBefore;
                return Tuple.Create(isTodo, (DateTime?)midtermExamDate);
            }

            return Tuple.Create(false, (DateTime?)null);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查期末報告期限
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>Tuple(是否需要填寫期末報告, 期末審查預定日期)</returns>
    public static Tuple<bool, DateTime?> CheckFinalReportDeadline(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT [FinalExamDate]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                WHERE [ProjectID] = @ProjectID
                AND [isExist] = 1";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            var dt = db.GetTable();
            if (dt.Rows.Count > 0 && dt.Rows[0]["FinalExamDate"] != DBNull.Value)
            {
                var finalExamDate = Convert.ToDateTime(dt.Rows[0]["FinalExamDate"]);
                var oneMonthBefore = finalExamDate.AddMonths(-1);
                bool isTodo = DateTime.Now >= oneMonthBefore;
                return Tuple.Create(isTodo, (DateTime?)finalExamDate);
            }

            return Tuple.Create(false, (DateTime?)null);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查每月進度報告期限
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>是否需要填寫每月進度報告</returns>
    public static bool CheckMonthlyReportDeadline(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            // 取得專案的期間資料
            db.CommandText = @"
                SELECT [StartTime], [EndTime]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
                WHERE [ProjectID] = @ProjectID";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            
            var dt = db.GetTable();
            if (dt.Rows.Count == 0 || dt.Rows[0]["StartTime"] == DBNull.Value || dt.Rows[0]["EndTime"] == DBNull.Value)
            {
                return false; // 沒有期間資料，無法檢查
            }
            
            DateTime startTime = Convert.ToDateTime(dt.Rows[0]["StartTime"]);
            DateTime endTime = Convert.ToDateTime(dt.Rows[0]["EndTime"]);
            DateTime today = DateTime.Today;
            
            // 計算檢查範圍
            DateTime startMonth = new DateTime(startTime.Year, startTime.Month, 1);
            DateTime endMonth = new DateTime(endTime.Year, endTime.Month, 1);
            DateTime currentMonth = new DateTime(today.Year, today.Month, 1);
            
            DateTime checkEndMonth = currentMonth < endMonth ? currentMonth : endMonth;
            
            // 檢查每個月份
            DateTime checkMonth = startMonth;
            while (checkMonth <= checkEndMonth)
            {
                bool shouldCheck = true;
                
                // 如果是當月，需要過20號才檢查
                if (checkMonth.Year == today.Year && checkMonth.Month == today.Month)
                {
                    if (today.Day <= 20)
                    {
                        shouldCheck = false;
                    }
                }
                
                if (shouldCheck)
                {
                    // 將月份轉換為民國年月格式
                    int minguoYear = checkMonth.Year - 1911;
                    string monthString = $"{minguoYear}年{checkMonth.Month}月";
                    
                    // 查詢該月份的進度資料
                    db.CommandText = @"
                        SELECT [ActProgress]
                        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                        WHERE [ProjectID] = @ProjectID 
                        AND [Month] = @Month";
                    
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);
                    db.Parameters.Add("@Month", monthString);
                    
                    var progressDt = db.GetTable();
                    
                    // 如果沒有資料或 ActProgress 為 null，表示需要填寫
                    if (progressDt.Rows.Count == 0 || progressDt.Rows[0]["ActProgress"] == DBNull.Value)
                    {
                        return true; // 有未填寫的月份
                    }
                }
                
                checkMonth = checkMonth.AddMonths(1);
            }
            
            return false; // 所有月份都已填寫
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得每月進度報告逾期日期
    /// 邏輯：檢查所有應填寫且小於今天月份的月份，找出第一個未填寫的月份，逾期日期為該月份的下個月10號
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>逾期日期（如果有未填寫的月份）</returns>
    public static DateTime? GetMonthlyReportOverdueDate(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            // 取得專案的期間資料
            db.CommandText = @"
                SELECT [StartTime], [EndTime]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            var dt = db.GetTable();
            if (dt.Rows.Count == 0 || dt.Rows[0]["StartTime"] == DBNull.Value || dt.Rows[0]["EndTime"] == DBNull.Value)
            {
                return null; // 沒有期間資料，無法檢查
            }

            DateTime startTime = Convert.ToDateTime(dt.Rows[0]["StartTime"]);
            DateTime endTime = Convert.ToDateTime(dt.Rows[0]["EndTime"]);
            DateTime today = DateTime.Today;

            // 計算檢查範圍
            DateTime startMonth = new DateTime(startTime.Year, startTime.Month, 1);
            DateTime endMonth = new DateTime(endTime.Year, endTime.Month, 1);
            DateTime currentMonth = new DateTime(today.Year, today.Month, 1);

            // 檢查每個月份（只檢查 < 今天月份的月份）
            DateTime checkMonth = startMonth;
            while (checkMonth < currentMonth) // 只檢查小於當月的月份
            {
                if (checkMonth > endMonth) break; // 超過專案結束月份

                // 將月份轉換為民國年月格式
                int minguoYear = checkMonth.Year - 1911;
                string monthString = $"{minguoYear}年{checkMonth.Month}月";

                // 查詢該月份的進度資料
                db.CommandText = @"
                    SELECT [ActProgress]
                    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                    WHERE [ProjectID] = @ProjectID
                    AND [Month] = @Month";

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectId);
                db.Parameters.Add("@Month", monthString);

                var progressDt = db.GetTable();

                // 如果沒有資料或 ActProgress 為 null，表示未填寫
                if (progressDt.Rows.Count == 0 || progressDt.Rows[0]["ActProgress"] == DBNull.Value)
                {
                    // 找到第一個未填寫的月份，逾期日期為下個月10號
                    DateTime nextMonth = checkMonth.AddMonths(1);
                    return new DateTime(nextMonth.Year, nextMonth.Month, 10);
                }

                checkMonth = checkMonth.AddMonths(1);
            }

            return null; // 所有應檢查的月份都已填寫
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 完成月報任務
    /// </summary>
    /// <param name="projectId">專案ID</param>
    public static void CompleteMonthlyReportTask(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                SET [IsCompleted] = 1,
                    [OverdueDate] = NULL
                WHERE [ProjectID] = @ProjectID 
                AND [TaskNameEn] = 'MonthlyReport'";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            
            db.ExecuteNonQuery();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新逾期專案狀態（補正補件 → 逾期未補）
    /// </summary>
    public static void UpdateExpiredProjects()
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                SET [StatusesName] = '逾期未補',
                    [updated_at] = GETDATE()
                WHERE [StatusesName] = '補正補件'
                  AND [ExpirationDate] IS NOT NULL
                  AND [ExpirationDate] < GETDATE()
                  AND [isExist] = 1";

            db.ExecuteNonQuery();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得最新的科專補助案申請截止日期
    /// </summary>
    /// <returns>申請截止日期，如果沒有則回傳 null</returns>
    public static DateTime? GetLatestApplyEndDate()
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT TOP(1) ApplyEndDate
                FROM [OCA_OceanSubsidy].[dbo].[OFS_GrantType]
                WHERE TypeCode = 'SCI'
                  AND DATEADD(DAY, -1, GETDATE()) >= ApplyStartDate
                  AND DATEADD(DAY, -1, GETDATE()) <= ApplyEndDate
                ORDER BY TypeID DESC;";

            var dt = db.GetTable();

            if (dt.Rows.Count > 0 && dt.Rows[0]["ApplyEndDate"] != DBNull.Value)
            {
                return Convert.ToDateTime(dt.Rows[0]["ApplyEndDate"]);
            }

            return null;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新尚未提送的逾期專案狀態（尚未提送 → 逾期）
    /// </summary>
    public static void UpdateUnsubmittedExpiredProjects()
    {
        // 使用共用方法取得申請截止日期
        DateTime? applyEndDate = GetLatestApplyEndDate();

        if (applyEndDate.HasValue && DateTime.Now > applyEndDate.Value)
        {
            DbHelper db = new DbHelper();
            try
            {
                db.CommandText = @"
                    UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                    SET [StatusesName] = '逾期',
                        [updated_at] = GETDATE()
                    WHERE [Statuses] = '尚未提送'
                      AND [isExist] = 1";

                db.ExecuteNonQuery();
            }
            finally
            {
                db.Dispose();
            }
        }
    }

    /// <summary>
    /// 取得所有尚未提送的專案（Statuses = '尚未提送' 且 StatusesName = '編輯中'）
    /// </summary>
    /// <returns>未提送專案的資料表（包含 ProjectID, UserAccount, ProjectNameTw）</returns>
    public static System.Data.DataTable GetUnsubmittedProjects()
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT PM.ProjectID, PM.UserAccount, AM.ProjectNameTw
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
                LEFT JOIN [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] AM
                  ON PM.ProjectID = AM.ProjectID
                WHERE PM.Statuses = '尚未提送'
                  AND PM.StatusesName = '編輯中'
                  AND PM.isExist = 1 AND PM.isWithdrawal = 0";

            return db.GetTable();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得專案資訊（用於郵件通知）
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>Tuple(計畫名稱, 申請人帳號, 主辦人ID)</returns>
    public static Tuple<string, string, int?> GetProjectInfo(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT AM.ProjectNameTw, PM.UserAccount, SU.UserID as Organizer
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
                LEFT JOIN [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] AM
                  ON PM.ProjectID = AM.ProjectID
                LEFT JOIN [OCA_OceanSubsidy].[dbo].[Sys_User] SU
                  ON PM.SupervisoryPersonAccount = SU.Account
                WHERE PM.ProjectID = @ProjectID
                  AND PM.isExist = 1";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            var dt = db.GetTable();
            if (dt.Rows.Count > 0)
            {
                string projectName = dt.Rows[0]["ProjectNameTw"]?.ToString() ?? "";
                string account = dt.Rows[0]["UserAccount"]?.ToString() ?? "";
                int? organizer = dt.Rows[0]["Organizer"] != DBNull.Value
                    ? Convert.ToInt32(dt.Rows[0]["Organizer"])
                    : (int?)null;

                return Tuple.Create(projectName, account, organizer);
            }

            return Tuple.Create("", "", (int?)null);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查任務是否已完成
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <returns>任務是否已完成</returns>
    public static bool IsTaskCompleted(string projectId, string taskNameEn)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT IsCompleted
                FROM [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                WHERE ProjectID = @ProjectID
                  AND TaskNameEn = @TaskNameEn";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@TaskNameEn", taskNameEn);

            var dt = db.GetTable();
            if (dt.Rows.Count > 0 && dt.Rows[0]["IsCompleted"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["IsCompleted"]) == 1;
            }

            return false;
        }
        finally
        {
            db.Dispose();
        }
    }
}
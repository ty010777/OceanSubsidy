using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

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
                AND [isWithdrawal] = 0 and Statuses ='計畫執行'";
            
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
    /// 更新現有任務狀態
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <param name="isTodo">是否為待辦</param>
    public static void UpdateTaskStatus(string projectId, string taskNameEn, bool isTodo)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                SET [IsTodo] = @IsTodo
                WHERE [ProjectID] = @ProjectID 
                AND [TaskNameEn] = @TaskNameEn";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@TaskNameEn", taskNameEn);
            db.Parameters.Add("@IsTodo", isTodo ? 1 : 0);
            
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
    /// <returns>是否需要填寫期中報告</returns>
    public static bool CheckMidReportDeadline(string projectId)
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
                return DateTime.Now >= oneMonthBefore;
            }
            
            return false;
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
    /// <returns>是否需要填寫期末報告</returns>
    public static bool CheckFinalReportDeadline(string projectId)
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
                return DateTime.Now >= oneMonthBefore;
            }
            
            return false;
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
                SET [IsCompleted] = 1
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
    /// 更新逾期專案狀態
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
}
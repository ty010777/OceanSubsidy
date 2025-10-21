using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// 社團排程任務處理 Helper - 僅處理 SQL 操作
/// </summary>
public class OFS_ClubTaskHelper
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
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
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
    /// 檢查成果報告期限
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>Tuple(是否需要填寫成果報告, 逾期日期)</returns>
    public static Tuple<bool, DateTime?> CheckReportDeadline(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            // 查詢補助案資訊
            db.CommandText = @"
                SELECT gt.[ApplyEndDate], gt.[FinalOneMonth], gt.[FinalDeadline]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_GrantType] gt
                WHERE GETDATE() BETWEEN gt.[ApplyStartDate] AND gt.[ApplyEndDate]
                  AND gt.[TypeCode] = 'CLB'";

            var dt = db.GetTable();
            if (dt.Rows.Count > 0)
            {
                var applyEndDate = dt.Rows[0]["ApplyEndDate"] != DBNull.Value
                    ? Convert.ToDateTime(dt.Rows[0]["ApplyEndDate"])
                    : (DateTime?)null;

                var finalOneMonth = dt.Rows[0]["FinalOneMonth"] != DBNull.Value
                    ? Convert.ToBoolean(dt.Rows[0]["FinalOneMonth"])
                    : false;

                var finalDeadline = dt.Rows[0]["FinalDeadline"] != DBNull.Value
                    ? Convert.ToDateTime(dt.Rows[0]["FinalDeadline"])
                    : (DateTime?)null;

                if (applyEndDate.HasValue)
                {
                    // 判斷是否應該打開待辦
                    bool isTodo = false;
                    DateTime? overdueDate = null;

                    if (finalOneMonth)
                    {
                        
                        isTodo = DateTime.Today >= applyEndDate.Value;
                        overdueDate = applyEndDate.Value.AddDays(30);
                    }
                    else
                    {
                        if (finalDeadline.HasValue)
                        {
                            DateTime openDate = finalDeadline.Value.AddDays(-30);
                            isTodo = DateTime.Today >= openDate;
                            overdueDate = finalDeadline.Value;
                        }
                    }

                    return Tuple.Create(isTodo, overdueDate);
                }
            }

            return Tuple.Create(false, (DateTime?)null);
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
                SELECT AB.ProjectNameTw, PM.UserAccount, SU.UserID as Organizer
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main] PM
                LEFT JOIN [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic] AB
                  ON PM.ProjectID = AB.ProjectID
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

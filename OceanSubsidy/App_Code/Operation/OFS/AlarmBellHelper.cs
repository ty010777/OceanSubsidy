using GS.Data.Sql;
using System;
using System.Data;

/// <summary>
/// 小鈴鐺提醒數量統計 Helper
/// </summary>
public class AlarmBellHelper
{
    /// <summary>
    /// 取得計畫申請待處理數量
    /// </summary>
    /// <param name="userAccount">使用者帳號</param>
    /// <returns>待處理數量</returns>
    public static int GetApplicationPendingCount(string userAccount)
    {
        DbHelper db = new DbHelper();

        try
        {
            // 1. 編輯中/補正補件/計畫書修正中的數量
            db.CommandText = @"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ApplicationChecklistSearch]
                WHERE isExist = 1
                  AND isWithdrawal = 0
                  AND StatusesName NOT LIKE '%結案%'
                  AND StatusesName <> '已終止'
                  AND (StatusesName LIKE '%編輯中%'
                       OR StatusesName LIKE '%補正補件%'
                       OR StatusesName LIKE '%計畫書修正中%')
                  AND UserAccount = @UserAccount";

            db.Parameters.Add("@UserAccount", userAccount);

            var dt1 = db.GetTable();
            int editingCount = dt1.Rows.Count > 0 ? Convert.ToInt32(dt1.Rows[0]["Count"]) : 0;

            db.Parameters.Clear();

            // 2. 審查意見待回覆的數量
            db.CommandText = @"
                SELECT COUNT(DISTINCT ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords]
                WHERE isSubmit = 1
                  AND (ReplyComment IS NULL OR ReplyComment = '' OR LTRIM(RTRIM(ReplyComment)) = '')
                  AND ProjectID IN (
                      SELECT ProjectID
                      FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ApplicationChecklistSearch]
                      WHERE UserAccount = @UserAccount
                        AND isExist = 1
                        AND isWithdrawal = 0
                        AND StatusesName NOT LIKE '%結案%'
                        AND StatusesName <> '已終止'
                  )";

            db.Parameters.Add("@UserAccount", userAccount);

            var dt2 = db.GetTable();
            int replyCount = dt2.Rows.Count > 0 ? Convert.ToInt32(dt2.Rows[0]["Count"]) : 0;

            return editingCount + replyCount;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得計畫申請待處理數量時發生錯誤：{ex.Message}");
            return 0;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得計畫執行待處理數量
    /// </summary>
    /// <param name="userAccount">使用者帳號</param>
    /// <returns>待處理數量</returns>
    public static int GetInprogressPendingCount(string userAccount)
    {
        DbHelper db = new DbHelper();

        try
        {
            // 1. TaskNameEn is not null 的數量
            db.CommandText = @"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_InprogressList]
                WHERE UserAccount = @UserAccount
                  AND TaskNameEn IS NOT NULL
                  AND StatusName <> '已終止'";

            db.Parameters.Add("@UserAccount", userAccount);

            var dt1 = db.GetTable();
            int taskCount = dt1.Rows.Count > 0 ? Convert.ToInt32(dt1.Rows[0]["Count"]) : 0;

            db.Parameters.Clear();

            // 2. 與 OFS_AuditRecords 關聯且 ExecutorComment 為空的數量
            db.CommandText = @"
                SELECT COUNT(V.ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_InprogressList] V
                INNER JOIN [OCA_OceanSubsidy].[dbo].[OFS_AuditRecords] AR
                    ON V.ProjectID = AR.ProjectID
                WHERE V.UserAccount = @UserAccount
                  AND V.StatusName <> '已終止'
                  AND (AR.ExecutorComment IS NULL OR AR.ExecutorComment = '')";

            db.Parameters.Add("@UserAccount", userAccount);

            var dt2 = db.GetTable();
            int auditCount = dt2.Rows.Count > 0 ? Convert.ToInt32(dt2.Rows[0]["Count"]) : 0;

            return taskCount + auditCount;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得計畫執行待處理數量時發生錯誤：{ex.Message}");
            return 0;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得申請計畫待審核數量
    /// </summary>
    /// <param name="isOrganizer">是否為主管單位人員</param>
    /// <param name="isSupervisor">是否為主管單位窗口</param>
    /// <param name="isSysAdmin">是否為系統管理者</param>
    /// <param name="userAccount">使用者帳號</param>
    /// <param name="supervisoryUnit">使用者所屬單位</param>
    /// <returns>待審核數量</returns>
    public static int GetApplicationReviewPendingCount(bool isOrganizer, bool isSupervisor, bool isSysAdmin,
        string userAccount = "", string supervisoryUnit = "")
    {
        DbHelper db = new DbHelper();

        try
        {
            int totalCount = 0;
            string additionalCondition = "";

            // 根據權限決定額外條件
            if (isSysAdmin)
            {
                // 系統管理者：不限制
                additionalCondition = "";
            }
            else if (isSupervisor)
            {
                // 主管單位窗口：限制單位
                additionalCondition = " AND SupervisoryUnit = @SupervisoryUnit";
            }
            else if (isOrganizer)
            {
                // 主管單位人員：限制帳號
                additionalCondition = " AND SupervisoryPersonAccount = @Account";
            }

            // 1. V_OFS_ReviewChecklist_type1
            db.CommandText = $@"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1]
                WHERE StatusesName NOT LIKE '%結案%'{additionalCondition}";

            if (isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
            }
            else if (isOrganizer && !isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@Account", userAccount);
            }

            var dt1 = db.GetTable();
            totalCount += dt1.Rows.Count > 0 ? Convert.ToInt32(dt1.Rows[0]["Count"]) : 0;

            db.Parameters.Clear();

            // 2. V_OFS_ReviewChecklist_type4
            db.CommandText = $@"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]
                WHERE StatusesName NOT LIKE '%結案%'{additionalCondition}";

            if (isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
            }
            else if (isOrganizer && !isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@Account", userAccount);
            }

            var dt2 = db.GetTable();
            totalCount += dt2.Rows.Count > 0 ? Convert.ToInt32(dt2.Rows[0]["Count"]) : 0;

            db.Parameters.Clear();

            // 3. OFS_SCI_Project_Main (領域審查、技術審查)
            db.CommandText = $@"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                WHERE (Statuses = '領域審查' OR Statuses = '技術審查')
                  AND StatusesName NOT LIKE '%結案%'{additionalCondition}";

            if (isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
            }
            else if (isOrganizer && !isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@Account", userAccount);
            }

            var dt3 = db.GetTable();
            totalCount += dt3.Rows.Count > 0 ? Convert.ToInt32(dt3.Rows[0]["Count"]) : 0;

            db.Parameters.Clear();

            // 4. OFS_CUL_Project (ProgressStatus = 2 或 3)
            db.CommandText = $@"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[OFS_CUL_Project]
                WHERE ProgressStatus IN (2, 3){additionalCondition}";

            if (isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
            }
            else if (isOrganizer && !isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@Account", userAccount);
            }

            var dt4 = db.GetTable();
            totalCount += dt4.Rows.Count > 0 ? Convert.ToInt32(dt4.Rows[0]["Count"]) : 0;

            return totalCount;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得申請計畫待審核數量時發生錯誤：{ex.Message}");
            return 0;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得執行計畫待審核數量
    /// </summary>
    /// <param name="isOrganizer">是否為主管單位人員</param>
    /// <param name="isSupervisor">是否為主管單位窗口</param>
    /// <param name="isSysAdmin">是否為系統管理者</param>
    /// <param name="userAccount">使用者帳號</param>
    /// <param name="supervisoryUnit">使用者所屬單位</param>
    /// <returns>待審核數量</returns>
    public static int GetInprogressReviewPendingCount(bool isOrganizer, bool isSupervisor, bool isSysAdmin,
        string userAccount = "", string supervisoryUnit = "")
    {
        DbHelper db = new DbHelper();

        try
        {
            int totalCount = 0;
            string additionalCondition = "";

            // 根據權限決定額外條件
            if (isSysAdmin)
            {
                // 系統管理者：不限制
                additionalCondition = "";
            }
            else if (isSupervisor)
            {
                // 主管單位窗口：限制單位
                additionalCondition = " AND SupervisoryUnit = @SupervisoryUnit";
            }
            else if (isOrganizer)
            {
                // 主管單位人員：限制帳號
                additionalCondition = " AND SupervisoryPersonAccount = @Account";
            }

            // 1. V_OFS_ReviewChecklist_type6
            db.CommandText = $@"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type6]
                WHERE 1=1{additionalCondition}";

            if (isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
            }
            else if (isOrganizer && !isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@Account", userAccount);
            }

            var dt1 = db.GetTable();
            totalCount += dt1.Rows.Count > 0 ? Convert.ToInt32(dt1.Rows[0]["Count"]) : 0;

            db.Parameters.Clear();

            // 2. V_OFS_ReviewChecklist_type5
            db.CommandText = $@"
                SELECT COUNT(ProjectID) AS Count
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type5]
                WHERE 1=1{additionalCondition}";

            if (isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
            }
            else if (isOrganizer && !isSupervisor && !isSysAdmin)
            {
                db.Parameters.Add("@Account", userAccount);
            }

            var dt2 = db.GetTable();
            totalCount += dt2.Rows.Count > 0 ? Convert.ToInt32(dt2.Rows[0]["Count"]) : 0;

            return totalCount;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得執行計畫待審核數量時發生錯誤：{ex.Message}");
            return 0;
        }
        finally
        {
            db.Dispose();
        }
    }
}

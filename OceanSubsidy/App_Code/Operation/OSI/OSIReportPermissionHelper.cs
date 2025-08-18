using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSI 活動報告權限檢查輔助類別
/// </summary>
public static class OSIReportPermissionHelper
{
    /// <summary>
    /// 檢查使用者是否有權限存取指定的報告資料
    /// </summary>
    /// <param name="reportId">報告ID</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>權限檢查結果</returns>
    public static PermissionCheckResult CheckReportPermission(int reportId, SessionHelper.UserInfoClass userInfo)
    {
        var result = new PermissionCheckResult
        {
            HasPermission = false,
            DeniedReason = "未知錯誤"
        };

        try
        {
            // 檢查使用者資訊
            if (userInfo == null)
            {
                result.DeniedReason = "使用者資訊無效";
                return result;
            }

            // 查詢報告資料
            OSI_ActivityReports report = OSIActivityReportsHelper.QueryByIDWithClass(reportId.ToString());
            if (report == null)
            {
                result.DeniedReason = "資料不存在";
                return result;
            }

            // 系統管理者擁有所有權限
            if (userInfo.OSI_RoleName == "系統管理者")
            {
                result.HasPermission = true;
                result.Report = report;
                return result;
            }

            // 檢查使用者是否有權限存取此單位的資料
            if (HasUnitPermission(report.ReportingUnitID, userInfo))
            {
                result.HasPermission = true;
                result.Report = report;
            }
            else
            {
                result.DeniedReason = "您沒有權限存取此單位的資料";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.DeniedReason = "系統錯誤：" + ex.Message;
            System.Diagnostics.Debug.WriteLine($"CheckReportPermission Error: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// 檢查使用者是否有權限存取指定的報告歷史資料
    /// </summary>
    /// <param name="historyId">歷史記錄ID</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>權限檢查結果</returns>
    public static PermissionCheckResult CheckReportHistoryPermission(long historyId, SessionHelper.UserInfoClass userInfo)
    {
        var result = new PermissionCheckResult
        {
            HasPermission = false,
            DeniedReason = "未知錯誤"
        };

        try
        {
            // 檢查使用者資訊
            if (userInfo == null)
            {
                result.DeniedReason = "使用者資訊無效";
                return result;
            }

            // 查詢歷史記錄
            var history = OSIActivityReportsHistoryHelper.GetHistoryById(Convert.ToInt32(historyId));
            if (history == null)
            {
                result.DeniedReason = "歷史資料不存在";
                return result;
            }

            // 系統管理者擁有所有權限
            if (userInfo.OSI_RoleName == "系統管理者")
            {
                result.HasPermission = true;
                result.History = history;
                return result;
            }

            // 檢查使用者是否有權限存取此單位的資料
            if (HasUnitPermission(history.ReportingUnitID, userInfo))
            {
                result.HasPermission = true;
                result.History = history;
            }
            else
            {
                result.DeniedReason = "您沒有權限存取此單位的資料";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.DeniedReason = "系統錯誤：" + ex.Message;
            System.Diagnostics.Debug.WriteLine($"CheckReportHistoryPermission Error: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// 檢查使用者是否有權限存取指定單位的資料
    /// </summary>
    /// <param name="reportingUnitId">報告機關的單位ID</param>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>有權限返回true，無權限返回false</returns>
    private static bool HasUnitPermission(int? reportingUnitId, SessionHelper.UserInfoClass userInfo)
    {
        try
        {
            // 如果是非政府機關資料(reportingUnitId為null或-1)
            if (!reportingUnitId.HasValue || reportingUnitId.Value == -1)
            {
                // 只有非政府機關用戶(UserInfo.UnitID為空)可以存取
                return string.IsNullOrEmpty(userInfo.UnitID);
            }

            // 如果使用者是非政府機關用戶，不能存取政府機關資料
            if (string.IsNullOrEmpty(userInfo.UnitID))
            {
                return false;
            }

            // 查詢使用者有權限的單位列表(包含自己的單位和子單位)
            var userUnitId = userInfo.UnitID.toInt();
            var allowedUnits = SysUnitHelper.QueryAllChildByID(userUnitId);
            
            if (allowedUnits == null || allowedUnits.Rows.Count == 0)
            {
                return false;
            }

            // 檢查報告的單位是否在允許的單位列表中
            foreach (DataRow row in allowedUnits.Rows)
            {
                if (row["UnitID"].ToString().toInt() == reportingUnitId.Value)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"HasUnitPermission Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 記錄未授權存取嘗試
    /// </summary>
    /// <param name="userInfo">使用者資訊</param>
    /// <param name="attemptedId">嘗試存取的ID</param>
    /// <param name="idType">ID類型(Report或History)</param>
    /// <param name="ipAddress">IP位址</param>
    public static void LogUnauthorizedAccess(SessionHelper.UserInfoClass userInfo, string attemptedId, string idType, string ipAddress)
    {
        try
        {
            string logMessage = $"未授權存取嘗試 - 使用者: {userInfo?.Account ?? "Unknown"}, " +
                              $"嘗試存取 {idType}ID: {attemptedId}, " +
                              $"IP: {ipAddress}, " +
                              $"時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            
            System.Diagnostics.Debug.WriteLine(logMessage);
            
            // TODO: 未來可以將日誌寫入資料庫或檔案系統
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LogUnauthorizedAccess Error: {ex.Message}");
        }
    }
}

/// <summary>
/// 權限檢查結果類別
/// </summary>
public class PermissionCheckResult
{
    /// <summary>
    /// 是否有權限
    /// </summary>
    public bool HasPermission { get; set; }

    /// <summary>
    /// 拒絕原因
    /// </summary>
    public string DeniedReason { get; set; }

    /// <summary>
    /// 報告資料(有權限時回傳)
    /// </summary>
    public OSI_ActivityReports Report { get; set; }

    /// <summary>
    /// 歷史資料(有權限時回傳)
    /// </summary>
    public OSI_ActivityReports_History History { get; set; }
}
<%@ WebHandler Language="C#" Class="ClubDayTask" %>

using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

public class ClubDayTask : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        try
        {
            // 1.檢查逾期補正補件 轉 逾期未補
            CheckDeadlines();
            // 2.檢查所有待辦事項
            CheckAllTasks();
            // 3.檢查申請截止日前一天並發送提醒郵件
            CheckAndSendDeadlineReminder();

            var response = new
            {
                success = true,
                executedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            context.Response.Write(JsonConvert.SerializeObject(response));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            var error = new
            {
                success = false,
                message = ex.Message,
                executedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            context.Response.Write(JsonConvert.SerializeObject(error));
        }
    }

    /// <summary>
    /// 檢查截止日期並更新逾期專案
    /// </summary>
    public static void CheckDeadlines()
    {
        try
        {
            // 更新「補正補件」逾期的專案 → 「逾期未補」
            OFS_ClubTaskHelper.UpdateExpiredProjects();

            // 更新「尚未提送」逾期的專案 → 「逾期」
            OFS_ClubTaskHelper.UpdateUnsubmittedExpiredProjects();
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查截止日期時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 檢查所有專案的待辦事項
    /// </summary>
    public static void CheckAllTasks()
    {
        try
        {
            // 任務類型定義
            var taskList = new Dictionary<string, string>
            {
                { "Report", "成果報告" }
            };

            // 取得所有活躍專案
            var projectIds = OFS_ClubTaskHelper.GetActiveProjectIds();

            foreach (string projectId in projectIds)
            {
                CheckProjectTasks(projectId, taskList);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查待辦事項時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 檢查單一專案的所有任務
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskList">任務清單</param>
    private static void CheckProjectTasks(string projectId, Dictionary<string, string> taskList)
    {
        if (string.IsNullOrEmpty(projectId)) return;

        // 檢查成果報告任務
        CheckReportTask(projectId, taskList);
    }

    /// <summary>
    /// 檢查成果報告任務
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskList">任務清單</param>
    private static void CheckReportTask(string projectId, Dictionary<string, string> taskList)
    {
        var result = OFS_ClubTaskHelper.CheckReportDeadline(projectId);
        bool isTodo = result.Item1;
        DateTime? overdueDate = result.Item2;

        // 檢查任務是否已完成
        bool isCompleted = OFS_ClubTaskHelper.IsTaskCompleted(projectId, "Report");

        // 如果需要填報、有截止日期、且任務尚未完成，發送郵件通知
        if (isTodo && overdueDate.HasValue && !isCompleted)
        {
            var projectInfo = OFS_ClubTaskHelper.GetProjectInfo(projectId);
            string projectName = projectInfo.Item1;
            string account = projectInfo.Item2;
            int? organizer = projectInfo.Item3;

            if (!string.IsNullOrEmpty(account))
            {
                // 如果今天日期已經大於逾期日期，使用 F2（進度落後提醒），否則使用 F11（資料填報提醒）
                if (DateTime.Today > overdueDate.Value)
                {
                    NotificationHelper.F2("學校社團", projectName, "成果報告", account, organizer);
                }
                else
                {
                    NotificationHelper.F11("學校社團", projectName, "成果報告", overdueDate.Value, account);
                }
            }
        }

        UpdateOrInsertTask(projectId, "Report", taskList["Report"], isTodo, overdueDate);
    }

    /// <summary>
    /// 更新或新增任務
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <param name="taskName">任務中文名稱</param>
    /// <param name="isTodo">是否為待辦</param>
    /// <param name="overdueDate">逾期日期（可選）</param>
    private static void UpdateOrInsertTask(string projectId, string taskNameEn, string taskName, bool isTodo, DateTime? overdueDate = null)
    {
        OFS_ClubTaskHelper.UpdateTaskStatus(projectId, taskNameEn, isTodo, overdueDate);
    }

    /// <summary>
    /// 檢查申請截止日前一天並發送提醒郵件
    /// </summary>
    private static void CheckAndSendDeadlineReminder()
    {
        try
        {
            // 取得最新的社團補助案申請截止日期
            DateTime? applyEndDate = OFS_ClubTaskHelper.GetLatestApplyEndDate();

            if (applyEndDate.HasValue)
            {
                DateTime today = DateTime.Today;

                // 檢查今天是否為申請截止日的前一天
                if (applyEndDate.Value.Date == today.AddDays(1)||applyEndDate.Value.Date == today.AddDays(2)||applyEndDate.Value.Date == today.AddDays(3))
                {
                    // 取得所有尚未提送的專案
                    var projectDt = OFS_ClubTaskHelper.GetUnsubmittedProjects();

                    // 為每個未提送的專案發送提醒郵件
                    foreach (System.Data.DataRow row in projectDt.Rows)
                    {
                        string projectId = row["ProjectID"]?.ToString();
                        string account = row["UserAccount"]?.ToString();
                        string projectName = row["ProjectNameTw"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(account))
                        {
                            // 使用 NotificationHelper.A0 發送提醒郵件
                            NotificationHelper.A0("學校社團", projectName, applyEndDate.Value, account, "CLB");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查申請截止日提醒時發生錯誤: {ex.Message}");
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}

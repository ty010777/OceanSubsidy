<%@ WebHandler Language="C#" Class="ScienceDayTask" %>

using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

public class ScienceDayTask : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        
        try
        {
            //1.檢查逾期補正補件 轉 逾期未補
            CheckDeadlines();
            //2.檢查所有待辦事項 
            CheckAllTasks(); 
            
            var response = new
            {
                success = true,
                executedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                // result = result
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
    /// <returns>處理結果</returns>
    public static void CheckDeadlines()
    {
        try
        {
            OFS_ScienceTaskHelper.UpdateExpiredProjects();
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
                //檢查期中、期末、每月報告是否逾期
             
                { "MidReport", "填寫期中報告" },
                { "FinalReport", "填寫期末報告" },
                { "MonthlyReport", "填寫每月進度報告" },
            };

            // 取得所有活躍專案
            var projectIds = OFS_ScienceTaskHelper.GetActiveProjectIds();
            
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

        // 依序檢查 3 個任務
        CheckMidReportTask(projectId, taskList);
        CheckFinalReportTask(projectId, taskList);
        CheckMonthlyReportTask(projectId, taskList);
    }

    #region 任務檢查函數



    /// <summary>
    /// 檢查填寫期中報告任務
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskList">任務清單</param>
    private static void CheckMidReportTask(string projectId, Dictionary<string, string> taskList)
    {
        var result = OFS_ScienceTaskHelper.CheckMidReportDeadline(projectId);
        bool isTodo = result.Item1;
        DateTime? overdueDate = result.Item2; // 期中審查預定日期直接作為逾期日期

        UpdateOrInsertTask(projectId, "MidReport", taskList["MidReport"], isTodo, overdueDate);
    }

    /// <summary>
    /// 檢查填寫期末報告任務
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskList">任務清單</param>
    private static void CheckFinalReportTask(string projectId, Dictionary<string, string> taskList)
    {
        var result = OFS_ScienceTaskHelper.CheckFinalReportDeadline(projectId);
        bool isTodo = result.Item1;
        DateTime? overdueDate = result.Item2; // 期末審查預定日期直接作為逾期日期

        UpdateOrInsertTask(projectId, "FinalReport", taskList["FinalReport"], isTodo, overdueDate);
    }

    /// <summary>
    /// 檢查填寫每月進度報告任務
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="taskList">任務清單</param>
    private static void CheckMonthlyReportTask(string projectId, Dictionary<string, string> taskList)
    {
        // 檢查每月進度報告：如果有任何應填寫而尚未填寫的月份，設為待辦
        bool isTodo = OFS_ScienceTaskHelper.CheckMonthlyReportDeadline(projectId);

        // 取得月報逾期日期（第一個未填寫月份的下個月10號）
        DateTime? overdueDate = OFS_ScienceTaskHelper.GetMonthlyReportOverdueDate(projectId);

        UpdateOrInsertTask(projectId, "MonthlyReport", taskList["MonthlyReport"], isTodo, overdueDate);
    }

   
    #endregion

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
        OFS_ScienceTaskHelper.UpdateTaskStatus(projectId, taskNameEn, isTodo, overdueDate);
    }
    public bool IsReusable
    {
        get { return false; }
    }
}
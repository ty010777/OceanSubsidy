using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// 任務佇列資料處理 Helper - 僅處理 SQL 操作
/// </summary>
public class OFS_TaskQueueHelper
{
    /// <summary>
    /// 取得專案的待辦事項清單
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>待辦事項清單</returns>
    public static DataTable GetProjectTodoTasks(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT [TaskNameEn], [TaskName]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                WHERE [ProjectID] = @ProjectID
                AND [IsTodo] = 1
                AND ([IsCompleted] = 0 OR [IsCompleted] IS NULL)
                ORDER BY [PriorityLevel], [TaskNameEn]";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            return db.GetTable();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新特定任務的狀態
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <param name="isTodo">是否為待辦 (1: 是, 0: 否)</param>
    public static void UpdateTaskStatus(string projectId, string taskNameEn, int isTodo ,int IsCompleted)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_TaskQueue]
                SET [IsTodo] = @IsTodo, [IsCompleted] = @IsCompleted
                WHERE [ProjectID] = @ProjectID
                  AND [TaskNameEn] = @TaskNameEn";

            db.Parameters.Clear();
            db.Parameters.Add("@IsTodo", isTodo);
            db.Parameters.Add("@IsCompleted", IsCompleted);
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@TaskNameEn", taskNameEn);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新任務 {taskNameEn} 的狀態時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    public static OFS_TaskQueue Get(string projectId, string taskNameEn)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ProjectID]
                  ,[TaskNameEn]
                  ,[TaskName]
                  ,[PriorityLevel]
                  ,[IsTodo]
                  ,[IsCompleted]
              FROM [OFS_TaskQueue]
             WHERE [ProjectID] = @ProjectID
               AND [TaskNameEn] = @TaskNameEn
        ";

        db.Parameters.Add("@ProjectID", projectId);
        db.Parameters.Add("@TaskNameEn", taskNameEn);

        var table = db.GetTable();

        if (table.Rows.Count != 1)
        {
            return null;
        }

        var row = table.Rows[0];

        return new OFS_TaskQueue
        {
            ProjectID = row.Field<string>("ProjectID"),
            TaskName = row.Field<string>("TaskName"),
            PriorityLevel = row.Field<int?>("PriorityLevel"),
            IsTodo = row.Field<bool?>("IsTodo"),
            IsCompleted = row.Field<bool?>("IsCompleted")
        };
    }

}

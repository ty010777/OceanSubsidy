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
}
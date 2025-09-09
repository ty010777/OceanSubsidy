using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class InprogressListHelper
{
    public InprogressListHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

   
    /// <summary>
    /// 更新 OFS_SCI_Project_Main 的 LastOperation 欄位
    /// </summary>
    /// <param name="projectId">計畫 ID</param>
    /// <param name="lastOperation">最後操作</param>
    /// <returns>是否更新成功</returns>
    public static void UpdateLastOperation(string projectId, string lastOperation)
    {
    

        DbHelper db = new DbHelper();
        db.CommandText = @"UPDATE OFS_SCI_Project_Main 
                          SET LastOperation = @LastOperation,
                              updated_at = @UpdatedAt
                          WHERE ProjectID = @ProjectID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectId);
        db.Parameters.Add("@LastOperation", lastOperation ?? "");
        db.Parameters.Add("@UpdatedAt", DateTime.Now);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ProjectID 和 TaskNameEn 更新 OFS_TaskQueue 的 IsCompleted 欄位
    /// </summary>
    /// <param name="projectId">計畫 ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <param name="isCompleted">是否完成</param>
    /// <returns>是否更新成功</returns>
    public static void UpdateTaskCompleted(string projectId, string taskNameEn, bool isCompleted)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"UPDATE OFS_TaskQueue 
                          SET IsCompleted = @IsCompleted
                          WHERE ProjectID = @ProjectID AND TaskNameEn = @TaskNameEn";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectId);
        db.Parameters.Add("@TaskNameEn", taskNameEn);
        db.Parameters.Add("@IsCompleted", isCompleted);

         db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ProjectID 和 TaskNameEn 更新 OFS_TaskQueue 的 IsTodo 欄位
    /// </summary>
    /// <param name="projectId">計畫 ID</param>
    /// <param name="taskNameEn">任務英文名稱</param>
    /// <param name="isTodo">是否為待辦</param>
    /// <returns>是否更新成功</returns>
    public static void UpdateTaskTodo(string projectId, string taskNameEn, bool isTodo)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"UPDATE OFS_TaskQueue 
                          SET IsTodo = @IsTodo
                          WHERE ProjectID = @ProjectID AND TaskNameEn = @TaskNameEn";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectId);
        db.Parameters.Add("@TaskNameEn", taskNameEn);
        db.Parameters.Add("@IsTodo", isTodo);

        db.ExecuteNonQuery();
    }

}
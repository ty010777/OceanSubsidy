using System;
using System.Collections.Generic;
using System.Data;
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

    /// <summary>
    /// 取得申請單位清單
    /// </summary>
    /// <returns>申請單位清單的 DataTable</returns>
    public static DataTable GetApplyUnits()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"SELECT DISTINCT OrgName
                          FROM V_OFS_InprogressList
                          WHERE OrgName IS NOT NULL AND OrgName <> ''
                          ORDER BY OrgName";

        DataTable result = db.GetTable();
        db.Dispose();

        return result;
    }

    /// <summary>
    /// 取得主管單位清單
    /// </summary>
    /// <returns>主管單位清單的 DataTable</returns>
    public static DataTable GetSupervisoryUnits()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"SELECT DISTINCT SupervisoryUnit
                          FROM V_OFS_InprogressList
                          WHERE SupervisoryUnit IS NOT NULL AND SupervisoryUnit <> ''
                          ORDER BY SupervisoryUnit";

        DataTable result = db.GetTable();
        db.Dispose();

        return result;
    }

    /// <summary>
    /// 取得進度清單資料
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="projectKeyword">計畫關鍵字</param>
    /// <param name="contentKeyword">內容關鍵字</param>
    /// <returns>進度清單資料的 DataTable</returns>
    public static DataTable GetInprogressListData(string year, string category, string orgName,
        string supervisoryUnit, string projectKeyword, string contentKeyword)
    {
        // 建構查詢條件
        string whereClause = "WHERE 1=1";
        DbHelper db = new DbHelper();

        if (!string.IsNullOrEmpty(year))
        {
            whereClause += " AND [Year] = @Year";
            db.Parameters.Add("@Year", year);
        }

        if (!string.IsNullOrEmpty(category))
        {
            whereClause += " AND Category = @Category";
            db.Parameters.Add("@Category", category);
        }

        if (!string.IsNullOrEmpty(orgName))
        {
            whereClause += " AND OrgName = @OrgName";
            db.Parameters.Add("@OrgName", orgName);
        }

        if (!string.IsNullOrEmpty(supervisoryUnit))
        {
            whereClause += " AND SupervisoryUnit = @SupervisoryUnit";
            db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
        }

        if (!string.IsNullOrEmpty(projectKeyword))
        {
            whereClause += " AND (ProjectID LIKE @Keyword OR ProjectNameTw LIKE @Keyword)";
            db.Parameters.Add("@Keyword", $"%{projectKeyword}%");
        }

        if (!string.IsNullOrEmpty(contentKeyword))
        {
            whereClause += " AND (ProjectContent LIKE @ContentKeyword OR KeyWords LIKE @ContentKeyword)";
            db.Parameters.Add("@ContentKeyword", $"%{contentKeyword}%");
        }

        // 執行查詢
        db.CommandText = $@"
            SELECT
                V.[Year],
                V.Category,
                V.ProjectID,
                V.StatusName,
                V.ProjectNameTw,
                V.OrgName,
                V.SupervisoryUnit,
                V.LastOperation,
                V.TaskNameEn,
                V.TaskName,
                V.ProjectContent,
                V.KeyWords,
                CASE
                    WHEN (V.StatusName = '' OR V.StatusName = '審核中')
                         AND EXISTS (
                        SELECT 1
                        FROM OFS_TaskQueue TQ
                        WHERE TQ.ProjectID = V.ProjectID
                          AND TQ.IsTodo = 1
                          AND TQ.IsCompleted = 0
                          AND TQ.OverdueDate <= CAST(GETDATE() AS DATE)
                    ) THEN 1
                    ELSE 0
                END AS IsOverdue
            FROM V_OFS_InprogressList V
            {whereClause}
            ORDER BY V.[Year] DESC, V.ProjectID";

        DataTable result = db.GetTable();
        db.Dispose();

        return result;
    }

    /// <summary>
    /// 取得進度清單的統計資訊
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="projectKeyword">計畫關鍵字</param>
    /// <param name="contentKeyword">內容關鍵字</param>
    /// <returns>包含統計數量的 DataRow (Total, InProgress, Overdue, Closed, Terminated)</returns>
    public static DataRow GetInprogressStatistics(string year, string category, string orgName,
        string supervisoryUnit, string projectKeyword, string contentKeyword)
    {
        // 建構查詢條件
        string whereClause = "WHERE 1=1";
        DbHelper db = new DbHelper();

        if (!string.IsNullOrEmpty(year))
        {
            whereClause += " AND [Year] = @Year";
            db.Parameters.Add("@Year", year);
        }

        if (!string.IsNullOrEmpty(category))
        {
            whereClause += " AND Category = @Category";
            db.Parameters.Add("@Category", category);
        }

        if (!string.IsNullOrEmpty(orgName))
        {
            whereClause += " AND OrgName = @OrgName";
            db.Parameters.Add("@OrgName", orgName);
        }

        if (!string.IsNullOrEmpty(supervisoryUnit))
        {
            whereClause += " AND SupervisoryUnit = @SupervisoryUnit";
            db.Parameters.Add("@SupervisoryUnit", supervisoryUnit);
        }

        if (!string.IsNullOrEmpty(projectKeyword))
        {
            whereClause += " AND (ProjectID LIKE @Keyword OR ProjectNameTw LIKE @Keyword)";
            db.Parameters.Add("@Keyword", $"%{projectKeyword}%");
        }

        if (!string.IsNullOrEmpty(contentKeyword))
        {
            whereClause += " AND (ProjectContent LIKE @ContentKeyword OR KeyWords LIKE @ContentKeyword)";
            db.Parameters.Add("@ContentKeyword", $"%{contentKeyword}%");
        }

        // 查詢統計數量
        db.CommandText = $@"
            WITH DataWithOverdue AS (
                SELECT
                    V.StatusName,
                    V.ProjectID,
                    CASE
                        WHEN (V.StatusName = '' OR V.StatusName = '審核中')
                             AND EXISTS (
                            SELECT 1
                            FROM OFS_TaskQueue TQ
                            WHERE TQ.ProjectID = V.ProjectID
                              AND TQ.IsTodo = 1
                              AND TQ.IsCompleted = 0
                              AND TQ.OverdueDate <= CAST(GETDATE() AS DATE)
                        ) THEN 1
                        ELSE 0
                    END AS IsOverdue
                FROM V_OFS_InprogressList V
                {whereClause}
            )
            SELECT
                COUNT(*) AS Total,
                SUM(CASE
                    WHEN StatusName = '' OR StatusName = '審核中' THEN 1
                    ELSE 0
                END) AS InProgress,
                SUM(CASE
                    WHEN StatusName = '已結案' THEN 1
                    ELSE 0
                END) AS Closed,
                SUM(CASE
                    WHEN StatusName = '已終止' THEN 1
                    ELSE 0
                END) AS Terminated,
                SUM(IsOverdue) AS Overdue
            FROM DataWithOverdue";

        DataTable result = db.GetTable();
        db.Dispose();

        if (result.Rows.Count > 0)
        {
            return result.Rows[0];
        }

        return null;
    }

}
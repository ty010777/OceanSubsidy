using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GS.Data;
using GS.OCA_OceanSubsidy.Entity;
using GS.Data.Sql;

/// <summary>
/// CLB 已獲補助資料的 Helper 類別
/// 對應資料表: OFS_CLB_Received_Subsidy
/// </summary>
public class OFS_ClbReceivedSubsidyHelper
{
    public OFS_ClbReceivedSubsidyHelper()
    {
    }

    /// <summary>
    /// 儲存已獲補助資料（先刪除後新增）
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="subsidyList">已獲補助資料列表</param>
    public static void SaveReceivedSubsidyData(string projectID, List<OFS_CLB_Received_Subsidy> subsidyList)
    {
        // 先刪除該 ProjectID 的所有舊資料
        DeleteByProjectID(projectID);

        // 如果有新資料，則新增
        if (subsidyList != null && subsidyList.Count > 0)
        {
            foreach (var subsidy in subsidyList)
            {
                subsidy.ProjectID = projectID;
                Insert(subsidy);
            }
        }
    }

    /// <summary>
    /// 新增已獲補助資料
    /// </summary>
    /// <param name="subsidy">已獲補助資料物件</param>
    public static void Insert(OFS_CLB_Received_Subsidy subsidy)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            (
                [ProjectID],
                [Name],
                [Amount],
                [CreateTime],
                [UpdateTime]
            )
            VALUES
            (
                @ProjectID,
                @Name,
                @Amount,
                GETDATE(),
                GETDATE()
            )
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", subsidy.ProjectID);
        db.Parameters.Add("@Name", subsidy.Name ?? "");
        db.Parameters.Add("@Amount", subsidy.Amount);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新已獲補助資料
    /// </summary>
    /// <param name="subsidy">已獲補助資料物件</param>
    public static void Update(OFS_CLB_Received_Subsidy subsidy)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            SET
                [Name] = @Name,
                [Amount] = @Amount,
                [UpdateTime] = GETDATE()
            WHERE [ID] = @ID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ID", subsidy.ID);
        db.Parameters.Add("@Name", subsidy.Name ?? "");
        db.Parameters.Add("@Amount", subsidy.Amount);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ProjectID 刪除已獲補助資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    public static void DeleteByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ID 刪除已獲補助資料
    /// </summary>
    /// <param name="id">資料 ID</param>
    public static void DeleteByID(int id)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            WHERE [ID] = @ID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ProjectID 查詢已獲補助資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>已獲補助資料列表</returns>
    public static List<OFS_CLB_Received_Subsidy> GetByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [ID],
                   [ProjectID],
                   [Name],
                   [Amount],
                   [CreateTime],
                   [UpdateTime]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [ID]
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        List<OFS_CLB_Received_Subsidy> list = new List<OFS_CLB_Received_Subsidy>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_CLB_Received_Subsidy
            {
                ID = Convert.ToInt32(row["ID"]),
                ProjectID = row["ProjectID"].ToString(),
                Name = row["Name"].ToString(),
                Amount = Convert.ToInt32(row["Amount"]),
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                UpdateTime = row["UpdateTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdateTime"]) : (DateTime?)null
            });
        }

        return list;
    }

    /// <summary>
    /// 根據 ID 查詢單筆已獲補助資料
    /// </summary>
    /// <param name="id">資料 ID</param>
    /// <returns>已獲補助資料物件</returns>
    public static OFS_CLB_Received_Subsidy GetByID(int id)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [ID],
                   [ProjectID],
                   [Name],
                   [Amount],
                   [CreateTime],
                   [UpdateTime]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            WHERE [ID] = @ID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ID", id);

        DataTable dt = db.GetTable();

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            return new OFS_CLB_Received_Subsidy
            {
                ID = Convert.ToInt32(row["ID"]),
                ProjectID = row["ProjectID"].ToString(),
                Name = row["Name"].ToString(),
                Amount = Convert.ToInt32(row["Amount"]),
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                UpdateTime = row["UpdateTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdateTime"]) : (DateTime?)null
            };
        }

        return null;
    }

    /// <summary>
    /// 計算某個 ProjectID 的已獲補助總金額
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>總金額</returns>
    public static int GetTotalAmountByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT ISNULL(SUM([Amount]), 0) AS TotalAmount
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Received_Subsidy]
            WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();

        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0]["TotalAmount"]);
        }

        return 0;
    }
}

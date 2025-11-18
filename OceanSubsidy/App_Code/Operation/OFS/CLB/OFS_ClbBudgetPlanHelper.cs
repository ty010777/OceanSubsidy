using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// CLB 經費預算規劃的 Helper 類別
/// </summary>
public class OFS_ClbBudgetPlanHelper
{
    public OFS_ClbBudgetPlanHelper()
    {
    }

    /// <summary>
    /// 儲存經費預算規劃資料（先刪除後新增）
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="budgetPlanList">經費預算規劃資料列表</param>
    public static void SaveBudgetPlanData(string projectID, List<OFS_CLB_Budget_Plan> budgetPlanList)
    {
        // 先刪除該 ProjectID 的所有舊資料
        DeleteByProjectID(projectID);

        // 如果有新資料，則新增
        if (budgetPlanList != null && budgetPlanList.Count > 0)
        {
            foreach (var budgetPlan in budgetPlanList)
            {
                budgetPlan.ProjectID = projectID;
                Insert(budgetPlan);
            }
        }
    }

    /// <summary>
    /// 新增經費預算規劃資料
    /// </summary>
    /// <param name="budgetPlan">經費預算規劃資料物件</param>
    public static void Insert(OFS_CLB_Budget_Plan budgetPlan)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            (
                [ProjectID],
                [Title],
                [Amount],
                [OtherAmount],
                [Description],
                [CreateTime],
                [UpdateTime]
            )
            VALUES
            (
                @ProjectID,
                @Title,
                @Amount,
                @OtherAmount,
                @Description,
                GETDATE(),
                GETDATE()
            )
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", budgetPlan.ProjectID);
        db.Parameters.Add("@Title", budgetPlan.Title ?? "");
        db.Parameters.Add("@Amount", budgetPlan.Amount ?? 0);
        db.Parameters.Add("@OtherAmount", budgetPlan.OtherAmount ?? 0);
        db.Parameters.Add("@Description", budgetPlan.Description ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新經費預算規劃資料
    /// </summary>
    /// <param name="budgetPlan">經費預算規劃資料物件</param>
    public static void Update(OFS_CLB_Budget_Plan budgetPlan)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            SET
                [Title] = @Title,
                [Amount] = @Amount,
                [OtherAmount] = @OtherAmount,
                [Description] = @Description,
                [UpdateTime] = GETDATE()
            WHERE [ID] = @ID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ID", budgetPlan.ID);
        db.Parameters.Add("@Title", budgetPlan.Title ?? "");
        db.Parameters.Add("@Amount", budgetPlan.Amount ?? 0);
        db.Parameters.Add("@OtherAmount", budgetPlan.OtherAmount ?? 0);
        db.Parameters.Add("@Description", budgetPlan.Description ?? "");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ProjectID 刪除經費預算規劃資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    public static void DeleteByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ID 刪除經費預算規劃資料
    /// </summary>
    /// <param name="id">資料 ID</param>
    public static void DeleteByID(int id)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            DELETE FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            WHERE [ID] = @ID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 根據 ProjectID 查詢經費預算規劃資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>經費預算規劃資料列表</returns>
    public static List<OFS_CLB_Budget_Plan> GetByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [ID],
                   [ProjectID],
                   [Title],
                   [Amount],
                   [OtherAmount],
                   [Description],
                   [CreateTime],
                   [UpdateTime]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [ID]
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();
        List<OFS_CLB_Budget_Plan> list = new List<OFS_CLB_Budget_Plan>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new OFS_CLB_Budget_Plan
            {
                ID = Convert.ToInt32(row["ID"]),
                ProjectID = row["ProjectID"].ToString(),
                Title = row["Title"].ToString(),
                Amount = row["Amount"] != DBNull.Value ? Convert.ToInt32(row["Amount"]) : (int?)null,
                OtherAmount = row["OtherAmount"] != DBNull.Value ? Convert.ToInt32(row["OtherAmount"]) : (int?)null,
                Description = row["Description"].ToString(),
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                UpdateTime = row["UpdateTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdateTime"]) : (DateTime?)null
            });
        }

        return list;
    }

    /// <summary>
    /// 根據 ID 查詢單筆經費預算規劃資料
    /// </summary>
    /// <param name="id">資料 ID</param>
    /// <returns>經費預算規劃資料物件</returns>
    public static OFS_CLB_Budget_Plan GetByID(int id)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [ID],
                   [ProjectID],
                   [Title],
                   [Amount],
                   [OtherAmount],
                   [Description],
                   [CreateTime],
                   [UpdateTime]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            WHERE [ID] = @ID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ID", id);

        DataTable dt = db.GetTable();

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            return new OFS_CLB_Budget_Plan
            {
                ID = Convert.ToInt32(row["ID"]),
                ProjectID = row["ProjectID"].ToString(),
                Title = row["Title"].ToString(),
                Amount = row["Amount"] != DBNull.Value ? Convert.ToInt32(row["Amount"]) : (int?)null,
                OtherAmount = row["OtherAmount"] != DBNull.Value ? Convert.ToInt32(row["OtherAmount"]) : (int?)null,
                Description = row["Description"].ToString(),
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                UpdateTime = row["UpdateTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdateTime"]) : (DateTime?)null
            };
        }

        return null;
    }

    /// <summary>
    /// 計算某個 ProjectID 的海洋委員會經費總金額
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>總金額</returns>
    public static int GetTotalAmountByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT ISNULL(SUM([Amount]), 0) AS TotalAmount
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
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

    /// <summary>
    /// 計算某個 ProjectID 的其他配合經費總金額
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>總金額</returns>
    public static int GetTotalOtherAmountByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT ISNULL(SUM([OtherAmount]), 0) AS TotalOtherAmount
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Budget_Plan]
            WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        DataTable dt = db.GetTable();

        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0]["TotalOtherAmount"]);
        }

        return 0;
    }
}

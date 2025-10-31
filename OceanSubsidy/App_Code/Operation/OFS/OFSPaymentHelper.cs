using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

public class OFSPaymentHelper
{
    public static List<OFSPayment> query(string projectID)
    {
        var result = new List<OFSPayment>();
        var db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[ProjectID]
                  ,[Stage]
                  ,[ActDisbursementRatioPct]
                  ,[CurrentRequestAmount]
                  ,[TotalSpentAmount]
                  ,[CurrentActualPaidAmount]
                  ,[Status]
                  ,[ReviewerComment]
                  ,[ReviewUser]
                  ,[ReviewTime]
              FROM [OFS_Payment]
             WHERE [ProjectID] = @ProjectID
          ORDER BY [Stage]
        ";

        db.Parameters.Add("@ProjectID", projectID);

        var dt = db.GetTable();

        foreach (var row in dt.Rows.Cast<DataRow>())
        {
            result.Add(new OFSPayment
            {
                ID = row.Field<int>("ID"),
                ProjectID = row.Field<string>("ProjectID"),
                Stage = row.Field<int?>("Stage"),
                ActDisbursementRatioPct = row.Field<decimal?>("ActDisbursementRatioPct"),
                TotalSpentAmount = row.Field<decimal?>("TotalSpentAmount"),
                CurrentRequestAmount = row.Field<decimal?>("CurrentRequestAmount"),
                CurrentActualPaidAmount = row.Field<decimal?>("CurrentActualPaidAmount"),
                Status = row.Field<string>("Status"),
                ReviewerComment = row.Field<string>("ReviewerComment"),
                ReviewUser = row.Field<string>("ReviewUser"),
                ReviewTime = row.Field<DateTime?>("ReviewTime")
            });
        }

        return result;
    }

    public static void review(OFSPayment model)
    {
        var db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_Payment]
               SET [CurrentActualPaidAmount] = @CurrentActualPaidAmount
                  ,[Status] = @Status
                  ,[ReviewerComment] = @ReviewerComment
                  ,[ReviewUser] = @ReviewUser
                  ,[ReviewTime] = @ReviewTime
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@CurrentActualPaidAmount", model.CurrentActualPaidAmount);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@ReviewerComment", model.ReviewerComment);
        db.Parameters.Add("@ReviewUser", model.ReviewUser);
        db.Parameters.Add("@ReviewTime", model.ReviewTime);

        db.ExecuteNonQuery();
    }

    public static void submit(OFSPayment model)
    {
        var db = new DbHelper();

        if (model.ID == 0)
        {
            db.CommandText = @"
                INSERT INTO [OFS_Payment] ([ProjectID],[Stage],[ActDisbursementRatioPct],[CurrentRequestAmount],[TotalSpentAmount],[Status],[CreateTime])
                                   VALUES (@ProjectID, @Stage, @ActDisbursementRatioPct, @CurrentRequestAmount, @TotalSpentAmount, @Status, GETDATE())
            ";
        }
        else
        {
            db.CommandText = @"
                UPDATE [OFS_Payment]
                   SET [ActDisbursementRatioPct] = @ActDisbursementRatioPct
                      ,[CurrentRequestAmount] = @CurrentRequestAmount
                      ,[TotalSpentAmount] = @TotalSpentAmount
                      ,[Status] = @Status
                 WHERE [ProjectID] = @ProjectID
                   AND [Stage] = @Stage
            ";
        }

        db.Parameters.Add("@ProjectID", model.ProjectID);
        db.Parameters.Add("@Stage", model.Stage);
        db.Parameters.Add("@ActDisbursementRatioPct", model.ActDisbursementRatioPct);
        db.Parameters.Add("@CurrentRequestAmount", model.CurrentRequestAmount);
        db.Parameters.Add("@TotalSpentAmount", model.TotalSpentAmount);
        db.Parameters.Add("@Status", model.Status);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 查詢計畫的總撥款金額（僅計算狀態為「通過」的撥款）
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <returns>總撥款金額，若無資料則回傳 0</returns>
    public static decimal GetTotalPaidAmount(string projectID)
    {
        var db = new DbHelper();

        db.CommandText = @"
            SELECT SUM(CurrentActualPaidAmount) as TotalActualPaid
            FROM [OFS_SCI_Payment]
            WHERE [ProjectID] = @ProjectID
              AND [Status] = @Status
            GROUP BY ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Status", "通過");

        var dt = db.GetTable();

        if (dt.Rows.Count > 0)
        {
            var totalPaid = dt.Rows[0].Field<decimal?>("TotalActualPaid");
            return totalPaid ?? 0;
        }

        return 0;
    }
}

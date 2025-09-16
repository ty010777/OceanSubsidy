using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

public class OFSPaymentHelper
{
    public static List<OFS_SCI_Payment> query(string projectID)
    {
        var result = new List<OFS_SCI_Payment>();
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
              FROM [OFS_SCI_Payment]
             WHERE [ProjectID] = @ProjectID
          ORDER BY [Stage]
        ";

        db.Parameters.Add("@ProjectID", projectID);

        var dt = db.GetTable();

        foreach (var row in dt.Rows.Cast<DataRow>())
        {
            result.Add(new OFS_SCI_Payment
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

    public static void review(OFS_SCI_Payment model)
    {
        var db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_SCI_Payment]
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

    public static void submit(OFS_SCI_Payment model)
    {
        var db = new DbHelper();

        if (model.ID == 0)
        {
            db.CommandText = @"
                INSERT INTO [OFS_SCI_Payment] ([ProjectID],[Stage],[ActDisbursementRatioPct],[CurrentRequestAmount],[TotalSpentAmount],[Status],[CreateTime])
                                       VALUES (@ProjectID, @Stage, @ActDisbursementRatioPct, @CurrentRequestAmount, @TotalSpentAmount, @Status, GETDATE())
            ";
        }
        else
        {
            db.CommandText = @"
                UPDATE [OFS_SCI_Payment]
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
}

using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulBudgetPlanHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Budget_Plan]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulBudgetPlan model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Budget_Plan] ([PID],[ItemID],[Title],[Amount],[OtherAmount],[Description])
                    OUTPUT Inserted.ID VALUES (@PID ,@ItemID ,@Title ,@Amount ,@OtherAmount ,@Description)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@ItemID", model.ItemID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@OtherAmount", model.OtherAmount);
        db.Parameters.Add("@Description", model.Description);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulBudgetPlan> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[ItemID]
                  ,[Title]
                  ,[Amount]
                  ,[OtherAmount]
                  ,[Description]
              FROM [OFS_CUL_Budget_Plan]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulBudgetPlan model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Budget_Plan]
               SET [ItemID] = @ItemID
                  ,[Title] = @Title
                  ,[Amount] = @Amount
                  ,[OtherAmount] = @OtherAmount
                  ,[Description] = @Description
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@ItemID", model.ItemID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@OtherAmount", model.OtherAmount);
        db.Parameters.Add("@Description", model.Description);

        db.ExecuteNonQuery();
    }

    private static OFS_CulBudgetPlan toModel(DataRow row)
    {
        return new OFS_CulBudgetPlan
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            ItemID = row.Field<int>("ItemID"),
            Title = row.Field<string>("Title"),
            Amount = row.Field<int>("Amount"),
            OtherAmount = row.Field<int>("OtherAmount"),
            Description = row.Field<string>("Description")
        };
    }
}

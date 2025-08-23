using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_MulBudgetPlanHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_MUL_Budget_Plan]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_MulBudgetPlan model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_MUL_Budget_Plan] ([PID],[Title],[Amount],[OtherAmount],[Description],[CreateTime],[CreateUser])
                    OUTPUT Inserted.ID VALUES (@PID ,@Title ,@Amount ,@OtherAmount ,@Description, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@OtherAmount", model.OtherAmount);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_MulBudgetPlan> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Title]
                  ,[Amount]
                  ,[OtherAmount]
                  ,[Description]
              FROM [OFS_MUL_Budget_Plan]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_MulBudgetPlan model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Budget_Plan]
               SET [Title] = @Title
                  ,[Amount] = @Amount
                  ,[OtherAmount] = @OtherAmount
                  ,[Description] = @Description
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@OtherAmount", model.OtherAmount);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_MulBudgetPlan toModel(DataRow row)
    {
        return new OFS_MulBudgetPlan
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Title = row.Field<string>("Title"),
            Amount = row.Field<int>("Amount"),
            OtherAmount = row.Field<int>("OtherAmount"),
            Description = row.Field<string>("Description")
        };
    }
}

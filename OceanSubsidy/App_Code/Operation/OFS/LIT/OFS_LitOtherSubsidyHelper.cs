using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_LitOtherSubsidyHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_LIT_Other_Subsidy]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_LitOtherSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_LIT_Other_Subsidy] ([PID],[Unit],[Amount],[Content],[CreateTime],[CreateUser])
                      OUTPUT Inserted.ID VALUES (@PID ,@Unit ,@Amount, @Content, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Unit", model.Unit);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_LitOtherSubsidy> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Unit]
                  ,[Amount]
                  ,[Content]
              FROM [OFS_LIT_Other_Subsidy]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_LitOtherSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Other_Subsidy]
               SET [Unit] = @Unit
                  ,[Amount] = @Amount
                  ,[Content] = @Content
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Unit", model.Unit);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_LitOtherSubsidy toModel(DataRow row)
    {
        return new OFS_LitOtherSubsidy
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Unit = row.Field<string>("Unit"),
            Amount = row.Field<int>("Amount"),
            Content = row.Field<string>("Content")
        };
    }
}

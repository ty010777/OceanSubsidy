using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_EdcReceivedSubsidyHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_EDC_Received_Subsidy]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_EdcReceivedSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_EDC_Received_Subsidy] ([PID],[Name],[Amount],[CreateTime],[CreateUser])
                         OUTPUT Inserted.ID VALUES (@PID ,@Name ,@Amount, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_EdcReceivedSubsidy> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Name]
                  ,[Amount]
              FROM [OFS_EDC_Received_Subsidy]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_EdcReceivedSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Received_Subsidy]
               SET [Name] = @Name
                  ,[Amount] = @Amount
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_EdcReceivedSubsidy toModel(DataRow row)
    {
        return new OFS_EdcReceivedSubsidy
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Name = row.Field<string>("Name"),
            Amount = row.Field<int>("Amount")
        };
    }
}

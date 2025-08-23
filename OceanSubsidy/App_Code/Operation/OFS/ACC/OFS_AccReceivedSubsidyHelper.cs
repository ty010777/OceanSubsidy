using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_AccReceivedSubsidyHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_ACC_Received_Subsidy]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_AccReceivedSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_ACC_Received_Subsidy] ([PID],[Name],[Unit],[Amount],[CreateTime],[CreateUser])
                         OUTPUT Inserted.ID VALUES (@PID ,@Name ,@Unit ,@Amount, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@Unit", model.Unit);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_AccReceivedSubsidy> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Name]
                  ,[Unit]
                  ,[Amount]
              FROM [OFS_ACC_Received_Subsidy]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_AccReceivedSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_ACC_Received_Subsidy]
               SET [Name] = @Name
                  ,[Unit] = @Unit
                  ,[Amount] = @Amount
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@Unit", model.Unit);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_AccReceivedSubsidy toModel(DataRow row)
    {
        return new OFS_AccReceivedSubsidy
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Name = row.Field<string>("Name"),
            Unit = row.Field<string>("Unit"),
            Amount = row.Field<int>("Amount")
        };
    }
}

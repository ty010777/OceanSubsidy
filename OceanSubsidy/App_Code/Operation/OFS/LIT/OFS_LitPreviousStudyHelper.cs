using GS.Data.Sql;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;

public class OFS_LitPreviousStudyHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_LIT_Previous_Study]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_LitPreviousStudy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_LIT_Previous_Study] ([PID],[Title],[TheDate],[CreateTime],[CreateUser])
                       OUTPUT Inserted.ID VALUES (@PID ,@Title ,@TheDate ,GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@TheDate", model.TheDate);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_LitPreviousStudy> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Title]
                  ,[TheDate]
              FROM [OFS_LIT_Previous_Study]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_LitPreviousStudy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Previous_Study]
               SET [Title] = @Title
                  ,[TheDate] = @TheDate
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@TheDate", model.TheDate);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_LitPreviousStudy toModel(DataRow row)
    {
        return new OFS_LitPreviousStudy
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Title = row.Field<string>("Title"),
            TheDate = row.Field<DateTime>("TheDate")
        };
    }
}

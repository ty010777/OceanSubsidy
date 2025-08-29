using GS.Data.Sql;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;

public class OFS_LitItemHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_LIT_Item]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_LitItem model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_LIT_Item] ([PID],[Title],[Begin],[End],[Deadline],[Content],[CreateTime],[CreateUser])
             OUTPUT Inserted.ID VALUES (@PID ,@Title ,@Begin, @End, @Deadline, @Content, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Begin", model.Begin);
        db.Parameters.Add("@End", model.End);
        db.Parameters.Add("@Deadline", model.Deadline);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_LitItem> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Title]
                  ,[Begin]
                  ,[End]
                  ,[Deadline]
                  ,[Content]
              FROM [OFS_LIT_Item]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_LitItem model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Item]
               SET [Title] = @Title
                  ,[Begin] = @Begin
                  ,[End] = @End
                  ,[Deadline] = @Deadline
                  ,[Content] = @Content
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Begin", model.Begin);
        db.Parameters.Add("@End", model.End);
        db.Parameters.Add("@Deadline", model.Deadline);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_LitItem toModel(DataRow row)
    {
        return new OFS_LitItem
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Title = row.Field<string>("Title"),
            Begin = row.Field<int>("Begin"),
            End = row.Field<int>("End"),
            Deadline = row.Field<DateTime?>("Deadline"),
            Content = row.Field<string>("Content")
        };
    }
}

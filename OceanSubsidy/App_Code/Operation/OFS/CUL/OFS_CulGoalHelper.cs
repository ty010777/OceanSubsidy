using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulGoalHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Goal]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulGoal model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Goal] ([PID],[Title],[Content],[CreateTime],[CreateUser])
             OUTPUT Inserted.ID VALUES (@PID ,@Title ,@Content, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulGoal> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Title]
                  ,[Content]
              FROM [OFS_CUL_Goal]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulGoal model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Goal]
               SET [Title] = @Title
                  ,[Content] = @Content
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_CulGoal toModel(DataRow row)
    {
        return new OFS_CulGoal
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Title = row.Field<string>("Title"),
            Content = row.Field<string>("Content")
        };
    }
}

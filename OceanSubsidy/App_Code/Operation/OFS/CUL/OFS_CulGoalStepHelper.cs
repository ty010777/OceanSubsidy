using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulGoalStepHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Goal_Step]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void deleteByItemID(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Goal_Step]
             WHERE [ItemID] = @ItemID
        ";

        db.Parameters.Add("@ItemID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulGoalStep model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Goal_Step] ([PID],[ItemID],[Title],[Begin],[End],[CreateTime],[CreateUser])
                  OUTPUT Inserted.ID VALUES (@PID ,@ItemID ,@Title ,@Begin ,@End, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@ItemID", model.ItemID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Begin", model.Begin);
        db.Parameters.Add("@End", model.End);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulGoalStep> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[ItemID]
                  ,[Title]
                  ,[Begin]
                  ,[End]
              FROM [OFS_CUL_Goal_Step]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulGoalStep model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Goal_Step]
               SET [Title] = @Title
                  ,[Begin] = @Begin
                  ,[End] = @End
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Begin", model.Begin);
        db.Parameters.Add("@End", model.End);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_CulGoalStep toModel(DataRow row)
    {
        return new OFS_CulGoalStep
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            ItemID = row.Field<int>("ItemID"),
            Title = row.Field<string>("Title"),
            Begin = row.Field<int>("Begin"),
            End = row.Field<int>("End")
        };
    }
}

using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulGoalItemHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Goal_Item]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulGoalItem model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Goal_Item] ([PID],[GoalID],[Title],[Indicator],[CreateTime],[CreateUser])
                  OUTPUT Inserted.ID VALUES (@PID ,@GoalID ,@Title ,@Indicator, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@GoalID", model.GoalID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Indicator", model.Indicator);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulGoalItem> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[GoalID]
                  ,[Title]
                  ,[Indicator]
              FROM [OFS_CUL_Goal_Item]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulGoalItem model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Goal_Item]
               SET [Title] = @Title
                  ,[Indicator] = @Indicator
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Indicator", model.Indicator);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_CulGoalItem toModel(DataRow row)
    {
        return new OFS_CulGoalItem
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            GoalID = row.Field<int>("GoalID"),
            Title = row.Field<string>("Title"),
            Indicator = row.Field<string>("Indicator")
        };
    }
}

using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulGoalScheduleHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Goal_Schedule]
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
              FROM [OFS_CUL_Goal_Schedule]
             WHERE [ItemID] = @ItemID
        ";

        db.Parameters.Add("@ItemID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulGoalSchedule model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Goal_Schedule] ([PID],[ItemID],[Type],[Month],[StepID],[CreateTime],[CreateUser])
                      OUTPUT Inserted.ID VALUES (@PID ,@ItemID ,@Type ,@Month ,@StepID, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@ItemID", model.ItemID);
        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@Month", model.Month);
        db.Parameters.Add("@StepID", model.StepID);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulGoalSchedule> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[ItemID]
                  ,[Type]
                  ,[Month]
                  ,[StepID]
                  ,[Status]
              FROM [OFS_CUL_Goal_Schedule]
             WHERE [PID] = @PID
          ORDER BY [Type], [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulGoalSchedule model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Goal_Schedule]
               SET [Month] = @Month
                  ,[StepID] = @StepID
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Month", model.Month);
        db.Parameters.Add("@StepID", model.StepID);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateStatus(int id, int status)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Goal_Schedule]
               SET [Status] = @Status
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);
        db.Parameters.Add("@Status", status);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_CulGoalSchedule toModel(DataRow row)
    {
        return new OFS_CulGoalSchedule
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            ItemID = row.Field<int>("ItemID"),
            Type = row.Field<int>("Type"),
            Month = row.Field<int>("Month"),
            StepID = row.Field<int>("StepID"),
            Status = row.Field<int?>("Status")
        };
    }
}

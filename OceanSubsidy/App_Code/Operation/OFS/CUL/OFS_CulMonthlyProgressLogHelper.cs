using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulMonthlyProgressLogHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Monthly_Progress_Log]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulMonthlyProgressLog model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Monthly_Progress_Log] ([PID],[MPID],[ScheduleID],[Status],[DelayReason],[ImprovedWay],[CreateTime],[CreateUser])
                             OUTPUT Inserted.ID VALUES (@PID, @MPID, @ScheduleID, @Status, @DelayReason, @ImprovedWay, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@MPID", model.MPID);
        db.Parameters.Add("@ScheduleID", model.ScheduleID);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@DelayReason", model.DelayReason);
        db.Parameters.Add("@ImprovedWay", model.ImprovedWay);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulMonthlyProgressLog> query(int mpid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[MPID]
                  ,[ScheduleID]
                  ,[Status]
                  ,[DelayReason]
                  ,[ImprovedWay]
              FROM [OFS_CUL_Monthly_Progress_Log]
             WHERE [MPID] = @MPID
        ";

        db.Parameters.Add("@MPID", mpid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulMonthlyProgressLog model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Monthly_Progress_Log]
               SET [Status] = @Status
                  ,[DelayReason] = @DelayReason
                  ,[ImprovedWay] = @ImprovedWay
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@DelayReason", model.DelayReason);
        db.Parameters.Add("@ImprovedWay", model.ImprovedWay);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_CulMonthlyProgressLog toModel(DataRow row)
    {
        return new OFS_CulMonthlyProgressLog
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            MPID = row.Field<int>("MPID"),
            ScheduleID = row.Field<int>("ScheduleID"),
            Status = row.Field<int>("Status"),
            DelayReason = row.Field<string>("DelayReason"),
            ImprovedWay = row.Field<string>("ImprovedWay")
        };
    }
}

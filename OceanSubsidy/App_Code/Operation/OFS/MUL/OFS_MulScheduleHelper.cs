using GS.Data.Sql;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;

public class OFS_MulScheduleHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_MUL_Schedule]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_MulSchedule model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_MUL_Schedule] ([PID],[Type],[ItemID],[Deadline],[Content],[CreateTime],[CreateUser])
                 OUTPUT Inserted.ID VALUES (@PID ,@Type ,@ItemID ,@Deadline ,@Content, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@ItemID", model.ItemID);
        db.Parameters.Add("@Deadline", model.Deadline);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_MulSchedule> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Type]
                  ,[ItemID]
                  ,[Deadline]
                  ,[Content]
              FROM [OFS_MUL_Schedule]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_MulSchedule model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Schedule]
               SET [Type] = @Type
                  ,[ItemID] = @ItemID
                  ,[Deadline] = @Deadline
                  ,[Content] = @Content
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@ItemID", model.ItemID);
        db.Parameters.Add("@Deadline", model.Deadline);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_MulSchedule toModel(DataRow row)
    {
        return new OFS_MulSchedule
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Type = row.Field<int>("Type"),
            ItemID = row.Field<int>("ItemID"),
            Deadline = row.Field<DateTime?>("Deadline"),
            Content = row.Field<string>("Content")
        };
    }
}

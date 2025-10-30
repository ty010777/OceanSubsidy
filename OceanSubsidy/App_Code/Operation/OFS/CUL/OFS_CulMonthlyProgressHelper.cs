using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulMonthlyProgressHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Monthly_Progress]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulMonthlyProgress model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Monthly_Progress] ([PID],[Year],[Month],[Description],[Status],[CreateTime],[CreateUser])
                         OUTPUT Inserted.ID VALUES (@PID ,@Year ,@Month ,@Description, 0,       GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@Month", model.Month);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static OFS_CulMonthlyProgress get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Year]
                  ,[Month]
                  ,[Description]
                  ,[Status]
              FROM [OFS_CUL_Monthly_Progress]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static OFS_CulMonthlyProgress get(int pid, int year, int month)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Year]
                  ,[Month]
                  ,[Description]
                  ,[Status]
              FROM [OFS_CUL_Monthly_Progress]
             WHERE [PID] = @PID
               AND [Year] = @Year
               AND [Month] = @Month
        ";

        db.Parameters.Add("@PID", pid);
        db.Parameters.Add("@Year", year);
        db.Parameters.Add("@Month", month);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static List<OFS_CulMonthlyProgress> querySubmited(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Year]
                  ,[Month]
                  ,[Description]
                  ,[Status]
              FROM [OFS_CUL_Monthly_Progress]
             WHERE [PID] = @PID
               AND [Status] = 1
          ORDER BY [Year]
                  ,[Month]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulMonthlyProgress model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Monthly_Progress]
               SET [Description] = @Description
                  ,[Status] = @Status
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_CulMonthlyProgress toModel(DataRow row)
    {
        return new OFS_CulMonthlyProgress
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Year = row.Field<int>("Year"),
            Month = row.Field<int>("Month"),
            Description = row.Field<string>("Description"),
            Status = row.Field<int>("Status")
        };
    }
}

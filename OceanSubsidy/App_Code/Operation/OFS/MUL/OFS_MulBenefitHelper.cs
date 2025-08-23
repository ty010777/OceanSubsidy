using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_MulBenefitHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_MUL_Benefit]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_MulBenefit model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_MUL_Benefit] ([PID],[Title],[Target],[Description],[CreateTime],[CreateUser])
                OUTPUT Inserted.ID VALUES (@PID ,@Title ,@Target ,@Description, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Target", model.Target);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_MulBenefit> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Title]
                  ,[Target]
                  ,[Description]
              FROM [OFS_MUL_Benefit]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_MulBenefit model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Benefit]
               SET [Title] = @Title
                  ,[Target] = @Target
                  ,[Description] = @Description
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Target", model.Target);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_MulBenefit toModel(DataRow row)
    {
        return new OFS_MulBenefit
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Title = row.Field<string>("Title"),
            Target = row.Field<string>("Target"),
            Description = row.Field<string>("Description")
        };
    }
}

using GS.Data;
using GS.Data.Sql;
using System.Linq;

public class OFS_CulOtherSubsidyHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Other_Subsidy]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulOtherSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Other_Subsidy] ([PID],[Unit],[Amount])
                      OUTPUT Inserted.ID VALUES (@PID ,@Unit ,@Amount)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Unit", model.Unit);
        db.Parameters.Add("@Amount", model.Amount);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static GisTable query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Unit]
                  ,[Amount]
              FROM [OFS_CUL_Other_Subsidy]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable();
    }

    public static void update(OFS_CulOtherSubsidy model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Other_Subsidy]
               SET [Unit] = @Unit
                  ,[Amount] = @Amount
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Unit", model.Unit);
        db.Parameters.Add("@Amount", model.Amount);

        db.ExecuteNonQuery();
    }
}

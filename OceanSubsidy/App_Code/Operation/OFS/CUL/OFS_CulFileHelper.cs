using GS.Data;
using GS.Data.Sql;
using System.Linq;

public class OFS_CulFileHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_File]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulFile model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_File] ([PID],[AttID],[FileID])
             OUTPUT Inserted.ID VALUES (@PID ,@AttID ,@FileID)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@AttID", model.AttID);
        db.Parameters.Add("@FileID", model.FileID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static GisTable query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[AttID]
                  ,[FileID]
              FROM [OFS_CUL_File]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable();
    }
}

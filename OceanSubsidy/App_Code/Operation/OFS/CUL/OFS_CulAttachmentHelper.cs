using GS.Data;
using GS.Data.Sql;
using System.Linq;

public class OFS_CulAttachmentHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Attachment]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulAttachment model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Attachment] ([PID],[FileCode],[Template])
                   OUTPUT Inserted.ID VALUES (@PID ,@FileCode ,@Template)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@FileCode", model.FileCode);
        db.Parameters.Add("@Template", model.Template);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static GisTable query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[FileCode]
                  ,[Template]
              FROM [OFS_CUL_Attachment]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable();
    }

    public static void update(OFS_CulAttachment model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Attachment]
               SET [FileCode] = @FileCode
                  ,[Template] = @Template
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@FileCode", model.FileCode);
        db.Parameters.Add("@Template", model.Template);

        db.ExecuteNonQuery();
    }
}

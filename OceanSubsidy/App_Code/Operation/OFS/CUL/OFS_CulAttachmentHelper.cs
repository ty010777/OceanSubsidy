using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
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
            INSERT INTO [OFS_CUL_Attachment] ([PID],[Type],[Path],[Name],[CreateTime],[CreateUser])
                   OUTPUT Inserted.ID VALUES (@PID ,@Type ,@Path ,@Name, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@Path", model.Path);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulAttachment> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Type]
                  ,[Path]
                  ,[Name]
              FROM [OFS_CUL_Attachment]
             WHERE [PID] = @PID
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    private static OFS_CulAttachment toModel(DataRow row)
    {
        return new OFS_CulAttachment
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Type = row.Field<int>("Type"),
            Path = row.Field<string>("Path"),
            Name = row.Field<string>("Name")
        };
    }
}

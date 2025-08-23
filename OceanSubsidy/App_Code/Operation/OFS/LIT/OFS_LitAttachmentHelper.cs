using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_LitAttachmentHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_LIT_Attachment]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_LitAttachment model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_LIT_Attachment] ([PID],[Type],[Path],[Name],[CreateTime],[CreateUser])
                   OUTPUT Inserted.ID VALUES (@PID ,@Type ,@Path ,@Name, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@Path", model.Path);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_LitAttachment> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Type]
                  ,[Path]
                  ,[Name]
              FROM [OFS_LIT_Attachment]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    private static OFS_LitAttachment toModel(DataRow row)
    {
        return new OFS_LitAttachment
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Type = row.Field<int>("Type"),
            Path = row.Field<string>("Path"),
            Name = row.Field<string>("Name")
        };
    }
}

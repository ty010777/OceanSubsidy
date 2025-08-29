using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_EdcContactHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_EDC_Contact]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_EdcContact model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_EDC_Contact] ([PID],[Role],[Name],[JobTitle],[Phone],[PhoneExt],[MobilePhone],[CreateTime],[CreateUser])
                OUTPUT Inserted.ID VALUES (@PID ,@Role ,@Name ,@JobTitle ,@Phone ,@PhoneExt ,@MobilePhone, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Role", model.Role);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@JobTitle", model.JobTitle);
        db.Parameters.Add("@Phone", model.Phone);
        db.Parameters.Add("@PhoneExt", model.PhoneExt);
        db.Parameters.Add("@MobilePhone", model.MobilePhone);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_EdcContact> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Role]
                  ,[Name]
                  ,[JobTitle]
                  ,[Phone]
                  ,[PhoneExt]
                  ,[MobilePhone]
              FROM [OFS_EDC_Contact]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_EdcContact model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Contact]
               SET [Role] = @Role
                  ,[Name] = @Name
                  ,[JobTitle] = @JobTitle
                  ,[Phone] = @Phone
                  ,[PhoneExt] = @PhoneExt
                  ,[MobilePhone] = @MobilePhone
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Role", model.Role);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@JobTitle", model.JobTitle);
        db.Parameters.Add("@Phone", model.Phone);
        db.Parameters.Add("@PhoneExt", model.PhoneExt);
        db.Parameters.Add("@MobilePhone", model.MobilePhone);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_EdcContact toModel(DataRow row)
    {
        return new OFS_EdcContact
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Role = row.Field<string>("Role"),
            Name = row.Field<string>("Name"),
            JobTitle = row.Field<string>("JobTitle"),
            Phone = row.Field<string>("Phone"),
            PhoneExt = row.Field<string>("PhoneExt"),
            MobilePhone = row.Field<string>("MobilePhone")
        };
    }
}

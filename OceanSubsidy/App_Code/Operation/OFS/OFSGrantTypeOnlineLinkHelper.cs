using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSGrantTypeOnlineLinkHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_GrantTypeOnlineLink]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(GrantTypeOnlineLink model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_GrantTypeOnlineLink] ([TypeID],[Title],[URL],[CreateTime],[CreateUser])
                        OUTPUT Inserted.ID VALUES (@TypeID, @Title, @URL, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@URL", model.URL);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<GrantTypeOnlineLink> query(int typeID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[TypeID]
                  ,[Title]
                  ,[URL]
              FROM [OFS_GrantTypeOnlineLink]
             WHERE [TypeID] = @TypeID
        ";

        db.Parameters.Add("@TypeID", typeID);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(GrantTypeOnlineLink model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_GrantTypeOnlineLink]
               SET [Title] = @Title
                  ,[URL] = @URL
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@URL", model.URL);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static GrantTypeOnlineLink toModel(DataRow row)
    {
        return new GrantTypeOnlineLink
        {
            ID = row.Field<int>("ID"),
            TypeID = row.Field<int>("TypeID"),
            Title = row.Field<string>("Title"),
            URL = row.Field<string>("URL")
        };
    }
}

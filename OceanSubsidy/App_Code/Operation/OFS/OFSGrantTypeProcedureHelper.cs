using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSGrantTypeProcedureHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_GrantTypeProcedure]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(GrantTypeProcedure model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_GrantTypeProcedure] ([TypeID],[Content],[CreateTime],[CreateUser])
                       OUTPUT Inserted.ID VALUES (@TypeID, @Content, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<GrantTypeProcedure> query(int typeID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[TypeID]
                  ,[Content]
              FROM [OFS_GrantTypeProcedure]
             WHERE [TypeID] = @TypeID
        ";

        db.Parameters.Add("@TypeID", typeID);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(GrantTypeProcedure model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_GrantTypeProcedure]
               SET [Content] = @Content
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static GrantTypeProcedure toModel(DataRow row)
    {
        return new GrantTypeProcedure
        {
            ID = row.Field<int>("ID"),
            TypeID = row.Field<int>("TypeID"),
            Content = row.Field<string>("Content")
        };
    }
}

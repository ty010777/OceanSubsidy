using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSNewsHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_News]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static News get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[Title]
                  ,[Content]
                  ,[EnableTime]
                  ,[DisableTime]
                  ,[UserName]
                  ,[UserOrg]
              FROM [OFS_News]
        ";

        db.Parameters.Add("@ID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(News model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_News] ([Title],[Content],[EnableTime],[DisableTime],[UserName],[UserOrg],[CreateTime],[CreateUser])
         OUTPUT Inserted.ID VALUES (@Title ,@Content, @EnableTime ,@DisableTime ,@UserName ,@UserOrg, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@EnableTime", model.EnableTime);
        db.Parameters.Add("@DisableTime", model.DisableTime);
        db.Parameters.Add("@UserName", model.UserName);
        db.Parameters.Add("@UserOrg", model.UserOrg);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<News> query()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[Title]
                  ,'' AS [Content]
                  ,[EnableTime]
                  ,[DisableTime]
                  ,[UserName]
                  ,[UserOrg]
              FROM [OFS_News]
          ORDER BY EnableTime DESC
        ";

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(News model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_News]
               SET [Title] = @Title
                  ,[Content] = @Content
                  ,[EnableTime] = @EnableTime
                  ,[DisableTime] = @DisableTime
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@EnableTime", model.EnableTime);
        db.Parameters.Add("@DisableTime", model.DisableTime);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static News toModel(DataRow row)
    {
        return new News
        {
            ID = row.Field<int>("ID"),
            Title = row.Field<string>("Title"),
            Content = row.Field<string>("Content"),
            EnableTime = row.Field<DateTime>("EnableTime"),
            DisableTime = row.Field<DateTime?>("DisableTime"),
            UserName = row.Field<string>("UserName"),
            UserOrg = row.Field<string>("UserOrg")
        };
    }
}

using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSNewsVideoHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_News_Video]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(NewsVideo model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_News_Video] ([NewsID],[Title],[URL],[CreateTime],[CreateUser])
              OUTPUT Inserted.ID VALUES  (@NewsID, @Title, @URL, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@NewsID", model.NewsID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@URL", model.URL);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<NewsVideo> query(int newsId)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[NewsID]
                  ,[Title]
                  ,[URL]
              FROM [OFS_News_Video]
             WHERE [NewsID] = @NewsID
          ORDER BY ID
        ";

        db.Parameters.Add("@NewsID", newsId);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(NewsVideo model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_News_Video]
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

    private static NewsVideo toModel(DataRow row)
    {
        return new NewsVideo
        {
            ID = row.Field<int>("ID"),
            NewsID = row.Field<int>("NewsID"),
            Title = row.Field<string>("Title"),
            URL = row.Field<string>("URL")
        };
    }
}

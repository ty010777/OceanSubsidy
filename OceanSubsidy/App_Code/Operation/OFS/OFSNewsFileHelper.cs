using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSNewsFileHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_News_File]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(NewsFile model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_News_File] ([NewsID],[Path],[Name],[CreateTime],[CreateUser])
              OUTPUT Inserted.ID VALUES (@NewsID, @Path, @Name, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@NewsID", model.NewsID);
        db.Parameters.Add("@Path", model.Path);
        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<NewsFile> query(int newsId)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[NewsID]
                  ,[Path]
                  ,[Name]
              FROM [OFS_News_File]
             WHERE [NewsID] = @NewsID
          ORDER BY ID
        ";

        db.Parameters.Add("@NewsID", newsId);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    private static NewsFile toModel(DataRow row)
    {
        return new NewsFile
        {
            ID = row.Field<int>("ID"),
            NewsID = row.Field<int>("NewsID"),
            Path = row.Field<string>("Path"),
            Name = row.Field<string>("Name")
        };
    }
}

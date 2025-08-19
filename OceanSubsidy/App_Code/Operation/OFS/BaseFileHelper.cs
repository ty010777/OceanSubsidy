using GS.Data.Sql;
using System;
using System.Data;

public class BaseFileHelper
{
    public static BaseFile getByPath(string path)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[Name]
                  ,[Path]
                  ,[Size]
                  ,[Type]
                  ,[CreateTime]
                  ,[CreateUser]
              FROM [OFS_Base_File]
             WHERE [Path] = @Path
        ";

        db.Parameters.Add("@Path", path);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(BaseFile model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_Base_File] ([Name],[Path],[Size],[Type],[CreateTime],[CreateUser])
              OUTPUT Inserted.ID VALUES (@Name, @Path, @Size, @Type, @CreateTime, @CreateUser)
        ";

        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@Path", model.Path);
        db.Parameters.Add("@Size", model.Size);
        db.Parameters.Add("@Type", model.Type);
// TODO
db.Parameters.Add("@CreateTime", DateTime.Now);
db.Parameters.Add("@CreateUser", "N/A");

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    private static BaseFile toModel(DataRow row)
    {
        return new BaseFile
        {
            ID = row.Field<int>("ID"),
            Name = row.Field<string>("Name"),
            Path = row.Field<string>("Path"),
            Size = row.Field<long>("Size"),
            Type = row.Field<string>("Type"),
            CreateTime = row.Field<DateTime>("CreateTime"),
            CreateUser = row.Field<string>("CreateUser")
        };
    }
}

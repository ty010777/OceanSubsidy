using GS.Data.Sql;
using System;
using System.Data;

public class BaseFileHelper
{
    public static BaseFile get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[Name]
                  ,[Path]
                  ,[Size]
                  ,[CreateTime]
                  ,[CreateUser]
              FROM [OFS_Base_File]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(BaseFile model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_Base_File] ([Name],[Path],[Size],[CreateTime],[CreateUser])
              OUTPUT Inserted.ID VALUES (@Name, @Path, @Size, @CreateTime, @CreateUser)
        ";

        db.Parameters.Add("@Name", model.Name);
        db.Parameters.Add("@Path", model.Path);
        db.Parameters.Add("@Size", model.Size);
        db.Parameters.Add("@CreateTime", model.CreateTime);
        db.Parameters.Add("@CreateUser", model.CreateUser);

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
            CreateTime = row.Field<DateTime>("CreateTime"),
            CreateUser = row.Field<string>("CreateUser")
        };
    }
}

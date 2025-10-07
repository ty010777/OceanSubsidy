using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Data;

public class OFSGrantTypeContentHelper
{
    public static GrantTypeContent get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [TypeID]
                  ,[ServiceContent]
                  ,[Keywords]
                  ,[Criteria]
                  ,[Documentary]
                  ,[ContactPerson]
                  ,[ContactTel]
                  ,[Path]
                  ,[Filename]
                  ,[WorkingDays]
                  ,[Remark]
                  ,[Status]
                  ,[StatusReason]
              FROM [OFS_GrantTypeContent]
             WHERE [TypeID] = @TypeID
        ";

        db.Parameters.Add("@TypeID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(GrantTypeContent model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_GrantTypeContent] ([TypeID],[Status],[CreateTime],[CreateUser])
                                        VALUES (@TypeID, 0,       GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void update(GrantTypeContent model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_GrantTypeContent]
               SET [ServiceContent] = @ServiceContent
                  ,[Keywords] = @Keywords
                  ,[Criteria] = @Criteria
                  ,[Documentary] = @Documentary
                  ,[ContactPerson] = @ContactPerson
                  ,[ContactTel] = @ContactTel
                  ,[Path] = @Path
                  ,[Filename] = @Filename
                  ,[WorkingDays] = @WorkingDays
                  ,[Remark] = @Remark
                  ,[Status] = @Status
                  ,[StatusReason] = @StatusReason
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [TypeID] = @TypeID
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@ServiceContent", model.ServiceContent);
        db.Parameters.Add("@Keywords", model.Keywords);
        db.Parameters.Add("@Criteria", model.Criteria);
        db.Parameters.Add("@Documentary", model.Documentary);
        db.Parameters.Add("@ContactPerson", model.ContactPerson);
        db.Parameters.Add("@ContactTel", model.ContactTel);
        db.Parameters.Add("@Path", model.Path);
        db.Parameters.Add("@Filename", model.Filename);
        db.Parameters.Add("@WorkingDays", model.WorkingDays);
        db.Parameters.Add("@Remark", model.Remark);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@StatusReason", model.StatusReason);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static GrantTypeContent toModel(DataRow row)
    {
        return new GrantTypeContent
        {
            TypeID = row.Field<int>("TypeID"),
            ServiceContent = row.Field<string>("ServiceContent"),
            Keywords = row.Field<string>("Keywords"),
            Criteria = row.Field<string>("Criteria"),
            Documentary = row.Field<string>("Documentary"),
            ContactPerson = row.Field<string>("ContactPerson"),
            ContactTel = row.Field<string>("ContactTel"),
            Path = row.Field<string>("Path"),
            Filename = row.Field<string>("Filename"),
            WorkingDays = row.Field<int?>("WorkingDays"),
            Remark = row.Field<string>("Remark"),
            Status = row.Field<int>("Status"),
            StatusReason = row.Field<string>("StatusReason")
        };
    }
}

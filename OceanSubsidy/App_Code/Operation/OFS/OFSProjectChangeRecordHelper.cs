using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Data;

public class OFSProjectChangeRecordHelper
{
    public static ProjectChangeRecord getApplying(string type, string dataID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT TOP (1) [ID]
                  ,[Type]
                  ,[Method]
                  ,[DataID]
                  ,[Reason]
                  ,[Form1Before]
                  ,[Form1After]
                  ,[Form2Before]
                  ,[Form2After]
                  ,[Form3Before]
                  ,[Form3After]
                  ,[Form4Before]
                  ,[Form4After]
                  ,[Form5Before]
                  ,[Form5After]
                  ,[Status]
                  ,[RejectReason]
              FROM [OFS_ProjectChangeRecord]
             WHERE [Type] = @Type
               AND [Method] = 1
               AND [DataID] = @DataID
               AND [Status] <> 3
          ORDER BY CreateTime DESC
        ";

        db.Parameters.Add("@Type", type);
        db.Parameters.Add("@DataID", dataID);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(ProjectChangeRecord model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_ProjectChangeRecord] ([Type],[Method],[DataID],[Reason],[Status],[CreateTime],[CreateUser])
                        OUTPUT Inserted.ID VALUES (@Type, @Method, @DataID, @Reason, 1,       GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@Method", model.Method);
        db.Parameters.Add("@DataID", model.DataID);
        db.Parameters.Add("@Reason", model.Reason);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static void update(ProjectChangeRecord model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_ProjectChangeRecord]
               SET [Form1Before] = @Form1Before
                  ,[Form1After] = @Form1After
                  ,[Form2Before] = @Form2Before
                  ,[Form2After] = @Form2After
                  ,[Form3Before] = @Form3Before
                  ,[Form3After] = @Form3After
                  ,[Form4Before] = @Form4Before
                  ,[Form4After] = @Form4After
                  ,[Form5Before] = @Form5Before
                  ,[Form5After] = @Form5After
                  ,[Status] = @Status
                  ,[RejectReason] = @RejectReason
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Form1Before", model.Form1Before);
        db.Parameters.Add("@Form1After", model.Form1After);
        db.Parameters.Add("@Form2Before", model.Form2Before);
        db.Parameters.Add("@Form2After", model.Form2After);
        db.Parameters.Add("@Form3Before", model.Form3Before);
        db.Parameters.Add("@Form3After", model.Form3After);
        db.Parameters.Add("@Form4Before", model.Form4Before);
        db.Parameters.Add("@Form4After", model.Form4After);
        db.Parameters.Add("@Form5Before", model.Form5Before);
        db.Parameters.Add("@Form5After", model.Form5After);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@RejectReason", model.RejectReason);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static ProjectChangeRecord toModel(DataRow row)
    {
        return new ProjectChangeRecord
        {
            ID = row.Field<int>("ID"),
            Type = row.Field<string>("Type"),
            Method = row.Field<int>("Method"),
            DataID = row.Field<string>("DataID"),
            Reason = row.Field<string>("Reason"),
            Form1Before = row.Field<string>("Form1Before"),
            Form1After = row.Field<string>("Form1After"),
            Form2Before = row.Field<string>("Form2Before"),
            Form2After = row.Field<string>("Form2After"),
            Form3Before = row.Field<string>("Form3Before"),
            Form3After = row.Field<string>("Form3After"),
            Form4Before = row.Field<string>("Form4Before"),
            Form4After = row.Field<string>("Form4After"),
            Form5Before = row.Field<string>("Form5Before"),
            Form5After = row.Field<string>("Form5After"),
            Status = row.Field<int>("Status"),
            RejectReason = row.Field<string>("RejectReason")
        };
    }
}

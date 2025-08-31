using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Data;

public class OFSProjectChangeRecordHelper
{
    public static void insert(ProjectChangeRecord model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_ProjectChangeRecord] ([Type],[DataID],[Reason],[CreateTime],[CreateUser])
                        OUTPUT Inserted.ID VALUES (@Type, @DataID, @Reason, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@DataID", model.DataID);
        db.Parameters.Add("@Reason", model.Reason);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }
}

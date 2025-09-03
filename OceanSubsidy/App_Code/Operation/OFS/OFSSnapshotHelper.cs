using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Data;

public class OFSSnapshotHelper
{
    public static void insert(Snapshot model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_Snapshot] ([Type],[DataID],[Status],[Data],[CreateTime],[CreateUser])
             OUTPUT Inserted.ID VALUES (@Type, @DataID, @Status, @Data, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@Type", model.Type);
        db.Parameters.Add("@DataID", model.DataID);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@Data", model.Data);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }
}

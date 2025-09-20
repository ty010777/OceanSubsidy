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

    public static Snapshot get(string type, int dataID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT TOP 1 [ID]
                  ,[Type]
                  ,[DataID]
                  ,[Status]
                  ,[Data]
              FROM [OFS_Snapshot]
             WHERE [Type] = @Type
               AND [DataID] = @DataID
          ORDER BY CreateTime DESC
        ";

        db.Parameters.Add("@Type", type);
        db.Parameters.Add("@DataID", dataID);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    private static Snapshot toModel(DataRow row)
    {
        return new Snapshot
        {
            ID = row.Field<int>("ID"),
            Type = row.Field<string>("Type"),
            DataID = row.Field<int>("DataID"),
            Status = row.Field<int>("Status"),
            Data = row.Field<string>("Data")
        };
    }
}

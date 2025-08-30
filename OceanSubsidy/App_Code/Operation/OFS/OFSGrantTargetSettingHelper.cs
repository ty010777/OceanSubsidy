using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Data;

public class OFSGrantTargetSettingHelper
{
    public static GrantTargetSetting getByTargetTypeID(string targetTypeID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[GrantTypeID]
                  ,[TargetTypeID]
                  ,[TargetName]
                  ,[MatchingFund]
                  ,[GrantLimit]
                  ,[Note]
              FROM [OFS_GrantTargetSetting]
             WHERE TargetTypeID = @TargetTypeID
        ";

        db.Parameters.Add("@TargetTypeID", targetTypeID);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    private static GrantTargetSetting toModel(DataRow row)
    {
        return new GrantTargetSetting
        {
            ID = row.Field<int>("ID"),
            GrantTypeID = row.Field<string>("GrantTypeID"),
            TargetTypeID = row.Field<string>("TargetTypeID"),
            TargetName = row.Field<string>("TargetName"),
            MatchingFund = row.Field<decimal?>("MatchingFund"),
            GrantLimit = row.Field<decimal?>("GrantLimit"),
            Note = row.Field<string>("Note")
        };
    }
}

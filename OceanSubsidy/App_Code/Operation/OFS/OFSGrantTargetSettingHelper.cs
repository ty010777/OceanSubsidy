using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

    public static List<GrantTargetSetting> query(string grantTypeID)
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
             WHERE GrantTypeID = @GrantTypeID
        ";

        db.Parameters.Add("@GrantTypeID", grantTypeID);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void updateLimit(int id, decimal? fund, decimal? limit)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_GrantTargetSetting]
               SET [MatchingFund] = @MatchingFund
                  ,[GrantLimit] = @GrantLimit
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);
        db.Parameters.Add("@MatchingFund", fund);
        db.Parameters.Add("@GrantLimit", limit);

        db.ExecuteNonQuery();
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

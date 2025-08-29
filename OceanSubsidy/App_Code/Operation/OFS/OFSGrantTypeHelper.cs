using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Data;

public class OFSGrantTypeHelper
{
    public static GrantTypeInfo getByCode(string code)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ShortName]
                  ,[FullName]
                  ,[ApplyStartDate]
                  ,[ApplyEndDate]
              FROM [OFS_GrantType]
             WHERE [TypeCode] = @TypeCode
               AND [ApplyStartDate] <= GETDATE()
               AND [ApplyEndDate] >= GETDATE()
        ";

        db.Parameters.Add("@TypeCode", code);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    private static GrantTypeInfo toModel(DataRow row)
    {
        return new GrantTypeInfo
        {
            ShortName = row.Field<string>("ShortName"),
            FullName = row.Field<string>("FullName"),
            StartDate = row.Field<DateTime>("ApplyStartDate"),
            EndDate = row.Field<DateTime>("ApplyEndDate")
        };
    }
}

using GS.Data;
using GS.Data.Sql;

public class SysZgsCodeHelper
{
    public static GisTable getZgsCodes(string codeGroup)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [CodeGroup]
                  ,[Code]
                  ,[Descname]
                  ,[OrderNo]
                  ,[IsValid]
                  ,[MaxPriceLimit]
                  ,[ValidBeginDate]
                  ,[ValidEndDate]
                  ,[ParentCode]
              FROM [Sys_ZgsCode]
             WHERE [IsValid] = 1
               AND [CodeGroup] = @CodeGroup
          ORDER BY [OrderNo]
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@CodeGroup", codeGroup);

        return db.GetTable();
    }
}

using GS.Data.Sql;
using System.Data;

public class SampleHelper
{
    public DataTable GetTableName()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP 3 [UUID]
      ,[CONN_ID]
      ,[TABLE_SCHEMA]
      ,[TABLE_NAME]
      ,[TABLE_DESC]
      ,[TABLE_KIND]
  FROM [IDAT].[dbo].[BACKUP_TABLE_DESC]; ";
        db.Parameters.Clear();
        return db.GetTable();
    }
}
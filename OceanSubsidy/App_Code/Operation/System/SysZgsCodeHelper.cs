using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

    public static List<DropdownItem> getCulReviewGroups()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT S.[Code],
                   M.[Descname] + '-' + S.[Descname] AS [Descname]
              FROM [Sys_ZgsCode] AS S
              JOIN [Sys_ZgsCode] AS M ON (M.[CodeGroup] = 'CULField' AND S.[ParentCode] = M.[Code])
             WHERE S.[CodeGroup] = 'CULField'
               AND S.[IsValid] = 1
          ORDER BY S.[OrderNo]
        ";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new DropdownItem
        {
            Value = row.Field<string>("Code"),
            Text = row.Field<string>("Descname")
        }).ToList();
    }

    public static List<DropdownItem> getSciReviewGroups()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [Code],
                   [Descname]
              FROM [Sys_ZgsCode]
             WHERE [CodeGroup] = 'SCIField'
               AND [IsValid] = 1
          ORDER BY [OrderNo]
        ";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new DropdownItem
        {
            Value = row.Field<string>("Code"),
            Text = row.Field<string>("Descname")
        }).ToList();
    }
}

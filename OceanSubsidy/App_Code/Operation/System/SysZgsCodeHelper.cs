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
             WHERE [CodeGroup] = 'SCITopic'
               AND [IsValid] = 1
          ORDER BY [OrderNo]
        ";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new DropdownItem
        {
            Value = row.Field<string>("Code"),
            Text = row.Field<string>("Descname")
        }).ToList();
    }

    /// <summary>
    /// 根據 CodeGroup 和 Code 查詢 Descname
    /// </summary>
    /// <param name="codeGroup">代碼群組（如 SCITopic, SCIField）</param>
    /// <param name="code">代碼</param>
    /// <returns>對應的 Descname，若查無資料則回傳空字串</returns>
    public static string GetDescnameByCode(string codeGroup, string code)
    {
        if (string.IsNullOrEmpty(codeGroup) || string.IsNullOrEmpty(code))
            return "";

        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                    SELECT [Descname]
                    FROM [Sys_ZgsCode]
                    WHERE [CodeGroup] = @CodeGroup
                      AND [Code] = @Code
                      AND [IsValid] = 1";

                db.Parameters.Clear();
                db.Parameters.Add("@CodeGroup", codeGroup);
                db.Parameters.Add("@Code", code);

                DataTable dt = db.GetTable();

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["Descname"]?.ToString() ?? "";
                }

                return "";
            }
            catch (Exception ex)
            {
                throw new Exception($"查詢 Sys_ZgsCode 資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }
}

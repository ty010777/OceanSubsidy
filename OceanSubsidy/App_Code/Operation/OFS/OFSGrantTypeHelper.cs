using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSGrantTypeHelper
{
    public static int count(int exclude, string typeCode, DateTime begin, DateTime end)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT COUNT(*) AS [Count]
              FROM [OFS_GrantType]
             WHERE [TypeID] != @exclude
               AND [TypeCode] = @typeCode
               AND [ApplyStartDate] <= @end
               AND [ApplyEndDate] >= @begin
        ";

        db.Parameters.Add("@exclude", exclude);
        db.Parameters.Add("@typeCode", typeCode);
        db.Parameters.Add("@begin", begin);
        db.Parameters.Add("@end", end);

        return int.Parse(db.GetTable().Rows[0]["Count"].ToString());
    }

    public static GrantType get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [TypeID]
                  ,[Year]
                  ,[TypeCode]
                  ,[ShortName]
                  ,[FullName]
                  ,[BudgetFees]
                  ,[TargetTags]
                  ,[ApplyStartDate]
                  ,[ApplyEndDate]
                  ,[PlanEndDate]
                  ,[Review1Title]
                  ,[Review1Enabled]
                  ,[Review2Title]
                  ,[Review2Enabled]
                  ,[Review3Title]
                  ,[Review3Enabled]
                  ,[Review4Title]
                  ,[Review4Enabled]
                  ,[OverduePeriod]
                  ,[MidtermDeadline]
                  ,[FinalDeadline]
                  ,[FinalOneMonth]
                  ,[AdminUnit]
              FROM [OFS_GrantType]
             WHERE [TypeID] = @TypeID
        ";

        db.Parameters.Add("@TypeID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static GrantTypeInfo getByCode(string code)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ShortName]
                  ,[FullName]
                  ,[ApplyStartDate]
                  ,[ApplyEndDate]
                  ,[Year]
              FROM [OFS_GrantType]
             WHERE [TypeCode] = @TypeCode
               AND [ApplyStartDate] <= GETDATE()
               AND [ApplyEndDate] >= GETDATE()
        ";

        db.Parameters.Add("@TypeCode", code);

        var table = db.GetTable();

        if (table.Rows.Count != 1)
        {
            return null;
        }

        DataRow row = table.Rows[0];

        return new GrantTypeInfo
        {
            ShortName = row.Field<string>("ShortName"),
            FullName = row.Field<string>("FullName"),
            StartDate = row.Field<DateTime>("ApplyStartDate"),
            EndDate = row.Field<DateTime>("ApplyEndDate"),
            Year = row.Field<int?>("Year")
        };
    }
    public static GrantTypeInfo getByTypeID(string ID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ShortName]
                  ,[FullName]
                  ,[ApplyStartDate]
                  ,[ApplyEndDate]
                  ,[Year]
              FROM [OFS_GrantType]
             WHERE TypeID = @TypeID
        ";

        db.Parameters.Add("@TypeID", ID);

        var table = db.GetTable();

        if (table.Rows.Count != 1)
        {
            return null;
        }

        DataRow row = table.Rows[0];

        return new GrantTypeInfo
        {
            ShortName = row.Field<string>("ShortName"),
            FullName = row.Field<string>("FullName"),
            StartDate = row.Field<DateTime>("ApplyStartDate"),
            EndDate = row.Field<DateTime>("ApplyEndDate"),
            Year = row.Field<int?>("Year")
        };
    }
    public static void insert(GrantType model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_GrantType] ([TypeCode],[Year],[FullName],[ShortName])
          OUTPUT Inserted.TypeID VALUES (@TypeCode ,@Year ,@FullName ,@ShortName)
        ";

        db.Parameters.Add("@TypeCode", model.TypeCode);
        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@FullName", model.FullName);
        db.Parameters.Add("@ShortName", model.ShortName);

        model.TypeID = int.Parse(db.GetTable().Rows[0]["TypeID"].ToString());
    }

    public static List<GrantType> query(string code)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [TypeID]
                  ,[Year]
                  ,[TypeCode]
                  ,[ShortName]
                  ,[FullName]
                  ,[BudgetFees]
                  ,[TargetTags]
                  ,[ApplyStartDate]
                  ,[ApplyEndDate]
                  ,[PlanEndDate]
                  ,[Review1Title]
                  ,[Review1Enabled]
                  ,[Review2Title]
                  ,[Review2Enabled]
                  ,[Review3Title]
                  ,[Review3Enabled]
                  ,[Review4Title]
                  ,[Review4Enabled]
                  ,[OverduePeriod]
                  ,[MidtermDeadline]
                  ,[FinalDeadline]
                  ,[FinalOneMonth]
                  ,[AdminUnit]
              FROM [OFS_GrantType]
             WHERE [TypeCode] = @TypeCode
        ";

        db.Parameters.Add("@TypeCode", code);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static List<GrantType> query(bool published = false)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT G.[TypeID]
                  ,G.[Year]
                  ,G.[FullName]
                  ,G.[ShortName]
                  ,G.[TypeCode]
                  ,G.[ApplyStartDate]
                  ,G.[ApplyEndDate]
                  ,G.[TargetTags]
                  ,C.[Path]
                  ,C.[Filename]
              FROM [OFS_GrantType] AS G
         LEFT JOIN [OFS_GrantTypeContent] AS C ON (C.[TypeID] = G.[TypeID])
        ";

        if (published)
        {
            db.CommandText += " WHERE G.[ApplyStartDate] <= GETDATE() AND G.[ApplyEndDate] >= GETDATE()";
        }

        db.CommandText += " ORDER BY G.[Year] DESC, G.[TypeCode]";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new GrantType
        {
            TypeID = row.Field<int>("TypeID"),
            Year = row.Field<int>("Year"),
            FullName = row.Field<string>("FullName"),
            ShortName = row.Field<string>("ShortName"),
            TypeCode = row.Field<string>("TypeCode"),
            ApplyStartDate = row.Field<DateTime?>("ApplyStartDate"),
            ApplyEndDate = row.Field<DateTime?>("ApplyEndDate"),
            TargetTags = row.Field<string>("TargetTags"),
            Path = row.Field<string>("Path"),
            Filename = row.Field<string>("Filename")
        }).ToList();
    }

    public static void updateContent(GrantType model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_GrantType]
               SET [Year] = @Year
                  ,[TypeCode] = @TypeCode
                  ,[ShortName] = @ShortName
                  ,[FullName] = @FullName
             WHERE [TypeID] = @TypeID
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@TypeCode", model.TypeCode);
        db.Parameters.Add("@ShortName", model.ShortName);
        db.Parameters.Add("@FullName", model.FullName);

        db.ExecuteNonQuery();
    }

    public static void update(GrantType model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_GrantType]
               SET [BudgetFees] = @BudgetFees
                  ,[TargetTags] = @TargetTags
                  ,[ApplyStartDate] = @ApplyStartDate
                  ,[ApplyEndDate] = @ApplyEndDate
                  ,[PlanEndDate] = @PlanEndDate
                  ,[Review1Title] = @Review1Title
                  ,[Review1Enabled] = @Review1Enabled
                  ,[Review2Title] = @Review2Title
                  ,[Review2Enabled] = @Review2Enabled
                  ,[Review3Title] = @Review3Title
                  ,[Review3Enabled] = @Review3Enabled
                  ,[Review4Title] = @Review4Title
                  ,[Review4Enabled] = @Review4Enabled
                  ,[OverduePeriod] = @OverduePeriod
                  ,[MidtermDeadline] = @MidtermDeadline
                  ,[FinalDeadline] = @FinalDeadline
                  ,[FinalOneMonth] = @FinalOneMonth
                  ,[AdminUnit] = @AdminUnit
             WHERE [TypeID] = @TypeID
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@BudgetFees", model.BudgetFees);
        db.Parameters.Add("@TargetTags", model.TargetTags);
        db.Parameters.Add("@ApplyStartDate", model.ApplyStartDate);
        db.Parameters.Add("@ApplyEndDate", model.ApplyEndDate);
        db.Parameters.Add("@PlanEndDate", model.PlanEndDate);
        db.Parameters.Add("@Review1Title", model.Review1Title);
        db.Parameters.Add("@Review1Enabled", model.Review1Enabled);
        db.Parameters.Add("@Review2Title", model.Review2Title);
        db.Parameters.Add("@Review2Enabled", model.Review2Enabled);
        db.Parameters.Add("@Review3Title", model.Review3Title);
        db.Parameters.Add("@Review3Enabled", model.Review3Enabled);
        db.Parameters.Add("@Review4Title", model.Review4Title);
        db.Parameters.Add("@Review4Enabled", model.Review4Enabled);
        db.Parameters.Add("@OverduePeriod", model.OverduePeriod);
        db.Parameters.Add("@MidtermDeadline", model.MidtermDeadline);
        db.Parameters.Add("@FinalDeadline", model.FinalDeadline);
        db.Parameters.Add("@FinalOneMonth", model.FinalOneMonth);
        db.Parameters.Add("@AdminUnit", model.AdminUnit);

        db.ExecuteNonQuery();
    }

    private static GrantType toModel(DataRow row)
    {
        return new GrantType
        {
            TypeID = row.Field<int>("TypeID"),
            Year = row.Field<int?>("Year"),
            TypeCode = row.Field<string>("TypeCode"),
            ShortName = row.Field<string>("ShortName"),
            FullName = row.Field<string>("FullName"),
            BudgetFees = row.Field<int?>("BudgetFees"),
            TargetTags = row.Field<string>("TargetTags"),
            ApplyStartDate = row.Field<DateTime?>("ApplyStartDate"),
            ApplyEndDate = row.Field<DateTime?>("ApplyEndDate"),
            PlanEndDate = row.Field<DateTime?>("PlanEndDate"),
            Review1Title = row.Field<string>("Review1Title"),
            Review1Enabled = row.Field<bool?>("Review1Enabled"),
            Review2Title = row.Field<string>("Review2Title"),
            Review2Enabled = row.Field<bool?>("Review2Enabled"),
            Review3Title = row.Field<string>("Review3Title"),
            Review3Enabled = row.Field<bool?>("Review3Enabled"),
            Review4Title = row.Field<string>("Review4Title"),
            Review4Enabled = row.Field<bool?>("Review4Enabled"),
            OverduePeriod = row.Field<int?>("OverduePeriod"),
            MidtermDeadline = row.Field<DateTime?>("MidtermDeadline"),
            FinalDeadline = row.Field<DateTime?>("FinalDeadline"),
            FinalOneMonth = row.Field<bool?>("FinalOneMonth"),
            AdminUnit = row.Field<string>("AdminUnit")
        };
    }
}

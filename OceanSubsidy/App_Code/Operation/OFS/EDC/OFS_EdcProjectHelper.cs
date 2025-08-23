using GS.Data.Sql;
using System;
using System.Data;

public class OFS_EdcProjectHelper
{
    public static int count(int year)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT COUNT(*) AS Count
              FROM [OFS_EDC_Project]
             WHERE [Year] = @Year
        ";

        db.Parameters.Add("@Year", year);

        return int.Parse(db.GetTable().Rows[0]["Count"].ToString());
    }

    public static OFS_EdcProject get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[Year]
                  ,[ProjectID]
                  ,[SubsidyPlanType]
                  ,[ProjectName]
                  ,[OrgCategory]
                  ,[OrgName]
                  ,[RegisteredNum]
                  ,[TaxID]
                  ,[Address]
                  ,[StartTime]
                  ,[EndTime]
                  ,[Target]
                  ,[Summary]
                  ,[Quantified]
                  ,[ApplyAmount]
                  ,[SelfAmount]
                  ,[OtherGovAmount]
                  ,[OtherUnitAmount]
                  ,[FormStep]
                  ,[Status]
                  ,[UserAccount]
                  ,[UserName]
                  ,[UserOrg]
              FROM [OFS_EDC_Project]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(OFS_EdcProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_EDC_Project] ([Year],[ProjectID],[SubsidyPlanType],[ProjectName],[OrgCategory],[OrgName],[RegisteredNum],[TaxID],[Address],[StartTime],
                                           [EndTime],[Target],[Summary],[Quantified],[ApplyAmount],[SelfAmount],[OtherGovAmount],[OtherUnitAmount],[FormStep],[Status],
                                           [UserAccount],[UserName],[UserOrg],[CreateTime],[CreateUser])
                OUTPUT Inserted.ID VALUES (@Year, @ProjectID, @SubsidyPlanType, @ProjectName, @OrgCategory, @OrgName, @RegisteredNum, @TaxID, @Address, @StartTime,
                                           @EndTime, @Target, @Summary, @Quantified, @ApplyAmount, @SelfAmount, @OtherGovAmount, @OtherUnitAmount, 1, 1,
                                           @UserAccount, @UserName, @UserOrg, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@ProjectID", model.ProjectID);
        db.Parameters.Add("@SubsidyPlanType", model.SubsidyPlanType);
        db.Parameters.Add("@ProjectName", model.ProjectName);
        db.Parameters.Add("@OrgCategory", model.OrgCategory);
        db.Parameters.Add("@OrgName", model.OrgName);
        db.Parameters.Add("@RegisteredNum", model.RegisteredNum);
        db.Parameters.Add("@TaxID", model.TaxID);
        db.Parameters.Add("@Address", model.Address);
        db.Parameters.Add("@StartTime", model.StartTime);
        db.Parameters.Add("@EndTime", model.EndTime);
        db.Parameters.Add("@Target", model.Target);
        db.Parameters.Add("@Summary", model.Summary);
        db.Parameters.Add("@Quantified", model.Quantified);
        db.Parameters.Add("@ApplyAmount", model.ApplyAmount);
        db.Parameters.Add("@SelfAmount", model.SelfAmount);
        db.Parameters.Add("@OtherGovAmount", model.OtherGovAmount);
        db.Parameters.Add("@OtherUnitAmount", model.OtherUnitAmount);
        db.Parameters.Add("@UserAccount", model.UserAccount);
        db.Parameters.Add("@UserName", model.UserName);
        db.Parameters.Add("@UserOrg", model.UserOrg);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static void update(OFS_EdcProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
               SET [ProjectName] = @ProjectName
                  ,[OrgCategory] = @OrgCategory
                  ,[OrgName] = @OrgName
                  ,[RegisteredNum] = @RegisteredNum
                  ,[TaxID] = @TaxID
                  ,[Address] = @Address
                  ,[StartTime] = @StartTime
                  ,[EndTime] = @EndTime
                  ,[Target] = @Target
                  ,[Summary] = @Summary
                  ,[Quantified] = @Quantified
                  ,[ApplyAmount] = @ApplyAmount
                  ,[SelfAmount] = @SelfAmount
                  ,[OtherGovAmount] = @OtherGovAmount
                  ,[OtherUnitAmount] = @OtherUnitAmount
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@ProjectName", model.ProjectName);
        db.Parameters.Add("@OrgCategory", model.OrgCategory);
        db.Parameters.Add("@OrgName", model.OrgName);
        db.Parameters.Add("@RegisteredNum", model.RegisteredNum);
        db.Parameters.Add("@TaxID", model.TaxID);
        db.Parameters.Add("@Address", model.Address);
        db.Parameters.Add("@StartTime", model.StartTime);
        db.Parameters.Add("@EndTime", model.EndTime);
        db.Parameters.Add("@Target", model.Target);
        db.Parameters.Add("@Summary", model.Summary);
        db.Parameters.Add("@Quantified", model.Quantified);
        db.Parameters.Add("@ApplyAmount", model.ApplyAmount);
        db.Parameters.Add("@SelfAmount", model.SelfAmount);
        db.Parameters.Add("@OtherGovAmount", model.OtherGovAmount);
        db.Parameters.Add("@OtherUnitAmount", model.OtherUnitAmount);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateFormStep(int id, int step)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
               SET [FormStep] = @FormStep
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
               AND [FormStep] + 1 = @FormStep
        ";

        db.Parameters.Add("@ID", id);
        db.Parameters.Add("@FormStep", step);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateStatus(int id, int status)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
               SET [Status] = @Status
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);
        db.Parameters.Add("@Status", status);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_EdcProject toModel(DataRow row)
    {
        return new OFS_EdcProject
        {
            ID = row.Field<int>("ID"),
            Year = row.Field<int>("Year"),
            ProjectID = row.Field<string>("ProjectID"),
            SubsidyPlanType = row.Field<string>("SubsidyPlanType"),
            ProjectName = row.Field<string>("ProjectName"),
            OrgCategory = row.Field<string>("OrgCategory"),
            OrgName = row.Field<string>("OrgName"),
            RegisteredNum = row.Field<string>("RegisteredNum"),
            TaxID = row.Field<string>("TaxID"),
            Address = row.Field<string>("Address"),
            StartTime = row.Field<DateTime?>("StartTime"),
            EndTime = row.Field<DateTime?>("EndTime"),
            Target = row.Field<string>("Target"),
            Summary = row.Field<string>("Summary"),
            Quantified = row.Field<string>("Quantified"),
            ApplyAmount = row.Field<int>("ApplyAmount"),
            SelfAmount = row.Field<int>("SelfAmount"),
            OtherGovAmount = row.Field<int>("OtherGovAmount"),
            OtherUnitAmount = row.Field<int>("OtherUnitAmount"),
            FormStep = row.Field<int>("FormStep"),
            Status = row.Field<int>("Status"),
            UserAccount = row.Field<string>("UserAccount"),
            UserName = row.Field<string>("UserName"),
            UserOrg = row.Field<string>("UserOrg")
        };
    }
}

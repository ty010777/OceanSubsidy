using GS.Data.Sql;
using System;
using System.Data;

public class OFS_MulProjectHelper
{
    public static int count(int year)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT COUNT(*) AS Count
              FROM [OFS_MUL_Project]
             WHERE [Year] = @Year
        ";

        db.Parameters.Add("@Year", year);

        return int.Parse(db.GetTable().Rows[0]["Count"].ToString());
    }

    public static OFS_MulProject get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[Year]
                  ,[ProjectID]
                  ,[SubsidyPlanType]
                  ,[ProjectName]
                  ,[Field]
                  ,[OrgName]
                  ,[OrgCategory]
                  ,[TaxID]
                  ,[Address]
                  ,[Target]
                  ,[Summary]
                  ,[Quantified]
                  ,[Qualitative]
                  ,[StartTime]
                  ,[EndTime]
                  ,[ApplyAmount]
                  ,[SelfAmount]
                  ,[OtherAmount]
                  ,[Benefit]
                  ,[FormStep]
                  ,[Status]
                  ,[UserAccount]
                  ,[UserName]
                  ,[UserOrg]
              FROM [OFS_MUL_Project]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(OFS_MulProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_MUL_Project] ([Year],[ProjectID],[SubsidyPlanType],[ProjectName],[Field],[OrgName],[OrgCategory],[TaxID],[Address],[Target],
                                           [Summary],[Quantified],[Qualitative],[ApplyAmount],[SelfAmount],[OtherAmount],[FormStep],[Status],[UserAccount],[UserName],
                                           [UserOrg],[CreateTime],[CreateUser])
                OUTPUT Inserted.ID VALUES (@Year, @ProjectID, @SubsidyPlanType, @ProjectName, @Field, @OrgName, @OrgCategory, @TaxID, @Address, @Target,
                                           @Summary, @Quantified, @Qualitative, 0,            0,           0,            1,         1,       @UserAccount, @UserName,
                                           @UserOrg, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@ProjectID", model.ProjectID);
        db.Parameters.Add("@SubsidyPlanType", model.SubsidyPlanType);
        db.Parameters.Add("@ProjectName", model.ProjectName);
        db.Parameters.Add("@Field", model.Field);
        db.Parameters.Add("@OrgName", model.OrgName);
        db.Parameters.Add("@OrgCategory", model.OrgCategory);
        db.Parameters.Add("@TaxID", model.TaxID);
        db.Parameters.Add("@Address", model.Address);
        db.Parameters.Add("@Target", model.Target);
        db.Parameters.Add("@Summary", model.Summary);
        db.Parameters.Add("@Quantified", model.Quantified);
        db.Parameters.Add("@Qualitative", model.Qualitative);
        db.Parameters.Add("@UserAccount", model.UserAccount);
        db.Parameters.Add("@UserName", model.UserName);
        db.Parameters.Add("@UserOrg", model.UserOrg);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static void update(OFS_MulProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Project]
               SET [ProjectName] = @ProjectName
                  ,[Field] = @Field
                  ,[OrgName] = @OrgName
                  ,[OrgCategory] = @OrgCategory
                  ,[TaxID] = @TaxID
                  ,[Address] = @Address
                  ,[Target] = @Target
                  ,[Summary] = @Summary
                  ,[Quantified] = @Quantified
                  ,[Qualitative] = @Qualitative
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@ProjectName", model.ProjectName);
        db.Parameters.Add("@Field", model.Field);
        db.Parameters.Add("@OrgName", model.OrgName);
        db.Parameters.Add("@OrgCategory", model.OrgCategory);
        db.Parameters.Add("@TaxID", model.TaxID);
        db.Parameters.Add("@Address", model.Address);
        db.Parameters.Add("@Target", model.Target);
        db.Parameters.Add("@Summary", model.Summary);
        db.Parameters.Add("@Quantified", model.Quantified);
        db.Parameters.Add("@Qualitative", model.Qualitative);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateBenefit(OFS_MulProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Project]
               SET [Benefit] = @Benefit
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Benefit", model.Benefit);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateFormStep(int id, int step)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Project]
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

    public static void updateFunding(OFS_MulProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Project]
               SET [ApplyAmount] = @ApplyAmount
                  ,[SelfAmount] = @SelfAmount
                  ,[OtherAmount] = @OtherAmount
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@ApplyAmount", model.ApplyAmount);
        db.Parameters.Add("@SelfAmount", model.SelfAmount);
        db.Parameters.Add("@OtherAmount", model.OtherAmount);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateSchedule(OFS_MulProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Project]
               SET [StartTime] = @StartTime
                  ,[EndTime] = @EndTime
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@StartTime", model.StartTime);
        db.Parameters.Add("@EndTime", model.EndTime);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateStatus(int id, int status)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_MUL_Project]
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

    private static OFS_MulProject toModel(DataRow row)
    {
        return new OFS_MulProject
        {
            ID = row.Field<int>("ID"),
            Year = row.Field<int>("Year"),
            ProjectID = row.Field<string>("ProjectID"),
            SubsidyPlanType = row.Field<string>("SubsidyPlanType"),
            ProjectName = row.Field<string>("ProjectName"),
            Field = row.Field<string>("Field"),
            OrgName = row.Field<string>("OrgName"),
            OrgCategory = row.Field<string>("OrgCategory"),
            TaxID = row.Field<string>("TaxID"),
            Address = row.Field<string>("Address"),
            Target = row.Field<string>("Target"),
            Summary = row.Field<string>("Summary"),
            Quantified = row.Field<string>("Quantified"),
            Qualitative = row.Field<string>("Qualitative"),
            StartTime = row.Field<DateTime?>("StartTime"),
            EndTime = row.Field<DateTime?>("EndTime"),
            ApplyAmount = row.Field<int>("ApplyAmount"),
            SelfAmount = row.Field<int>("SelfAmount"),
            OtherAmount = row.Field<int>("OtherAmount"),
            Benefit = row.Field<string>("Benefit"),
            FormStep = row.Field<int>("FormStep"),
            Status = row.Field<int>("Status"),
            UserAccount = row.Field<string>("UserAccount"),
            UserName = row.Field<string>("UserName"),
            UserOrg = row.Field<string>("UserOrg")
        };
    }
}

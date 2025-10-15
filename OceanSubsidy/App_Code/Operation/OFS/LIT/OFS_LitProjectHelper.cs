using GS.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_LitProjectHelper
{
    public static int count(int year)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT COUNT(*) AS Count
              FROM [OFS_LIT_Project]
             WHERE [Year] = @Year
        ";

        db.Parameters.Add("@Year", year);

        return int.Parse(db.GetTable().Rows[0]["Count"].ToString());
    }

    public static OFS_LitProject get(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT P.[ID]
                  ,P.[Year]
                  ,P.[ProjectID]
                  ,P.[SubsidyPlanType]
                  ,P.[ProjectName]
                  ,P.[Field]
                  ,P.[OrgName]
                  ,P.[OrgLeader]
                  ,P.[Address]
                  ,P.[Target]
                  ,P.[Summary]
                  ,P.[Quantified]
                  ,P.[Qualitative]
                  ,P.[StartTime]
                  ,P.[EndTime]
                  ,P.[ApplyAmount]
                  ,P.[SelfAmount]
                  ,P.[OtherAmount]
                  ,P.[ApprovedAmount]
                  ,P.[RecoveryAmount]
                  ,P.[Benefit]
                  ,P.[FormStep]
                  ,P.[Status]
                  ,P.[ProgressStatus]
                  ,P.[Organizer]
                  ,U.[Name] AS [OrganizerName]
                  ,P.[RejectReason]
                  ,P.[CorrectionDeadline]
                  ,P.[UserAccount]
                  ,P.[UserName]
                  ,P.[UserOrg]
                  ,P.[IsProjChanged]
                  ,P.[IsWithdrawal]
                  ,P.[IsExists]
                  ,P.[FinalReviewNotes]
                  ,P.[FinalReviewOrder]
              FROM [OFS_LIT_Project] AS P
         LEFT JOIN [Sys_User] AS U ON (U.UserID = P.Organizer)
             WHERE P.[ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static List<string> GetInprogressProjectIds(int year)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ProjectID]
              FROM [OFS_LIT_Project]
             WHERE [Year] = @Year
               AND [IsExists] = 1
               AND [IsWithdrawal] = 0
               AND [ProgressStatus] = 5
        ";

        db.Parameters.Add("@Year", year);

        return db.GetTable().Rows.Cast<DataRow>().Select(row => row.Field<string>("ProjectID")).ToList();
    }

    public static string getUserAccount(string projectID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [UserAccount]
              FROM [OFS_LIT_Project]
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? table.Rows[0].Field<string>("UserAccount") : "";
    }

    public static int getID(string projectID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
              FROM [OFS_LIT_Project]
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? table.Rows[0].Field<int>("ID") : 0;
    }

    public static void insert(OFS_LitProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_LIT_Project] ([Year],[ProjectID],[SubsidyPlanType],[ProjectName],[Field],[OrgName],[OrgLeader],[Address],[Target],[Summary],
                                           [Quantified],[Qualitative],[FormStep],[Status],[ProgressStatus],[UserAccount],[UserName],[UserOrg],[IsProjChanged],[IsWithdrawal],
                                           [IsExists],[CreateTime],[CreateUser])
                OUTPUT Inserted.ID VALUES (@Year, @ProjectID, @SubsidyPlanType, @ProjectName, @Field, @OrgName, @OrgLeader, @Address, @Target, @Summary,
                                           @Quantified, @Qualitative, 1,         1,       0,               @UserAccount, @UserName, @UserOrg, 0,              0,
                                           1,         GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@ProjectID", model.ProjectID);
        db.Parameters.Add("@SubsidyPlanType", model.SubsidyPlanType);
        db.Parameters.Add("@ProjectName", model.ProjectName);
        db.Parameters.Add("@Field", model.Field);
        db.Parameters.Add("@OrgName", model.OrgName);
        db.Parameters.Add("@OrgLeader", model.OrgLeader);
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

    public static void reviewApplication(OFS_LitProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [Status] = @Status
                  ,[RejectReason] = @RejectReason
                  ,[CorrectionDeadline] = @CorrectionDeadline
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
               AND [Status] = 11
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Status", model.Status);
        db.Parameters.Add("@RejectReason", model.RejectReason);
        db.Parameters.Add("@CorrectionDeadline", model.CorrectionDeadline);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void setProjectChanged(int id, bool changed)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [IsProjChanged] = @IsProjChanged
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);
        db.Parameters.Add("@IsProjChanged", changed);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void terminate(int id, string reason, int recovery)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [ProgressStatus] = 9
                  ,[Status] = 99
                  ,[RejectReason] = @RejectReason
                  ,[RecoveryAmount] = @RecoveryAmount
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);
        db.Parameters.Add("@RejectReason", reason);
        db.Parameters.Add("@RecoveryAmount", recovery);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void update(OFS_LitProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [ProjectName] = @ProjectName
                  ,[Field] = @Field
                  ,[OrgName] = @OrgName
                  ,[OrgLeader] = @OrgLeader
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
        db.Parameters.Add("@OrgLeader", model.OrgLeader);
        db.Parameters.Add("@Address", model.Address);
        db.Parameters.Add("@Target", model.Target);
        db.Parameters.Add("@Summary", model.Summary);
        db.Parameters.Add("@Quantified", model.Quantified);
        db.Parameters.Add("@Qualitative", model.Qualitative);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateApprovedAmount(string projectID, int amount, string notes)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [ApprovedAmount] = @ApprovedAmount
                  ,[FinalReviewNotes] = @FinalReviewNotes
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@ApprovedAmount", amount);
        db.Parameters.Add("@FinalReviewNotes", notes);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateBenefit(OFS_LitProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
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

    public static void updateExistsStatus(string projectID, bool isExists)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [IsExists] = @IsExists
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@IsExists", isExists);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateFormStep(string projectID, int step)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [FormStep] = @FormStep
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ProjectID] = @ProjectID
               AND [FormStep] + 1 = @FormStep
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@FormStep", step);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateFunding(OFS_LitProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
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

    public static void updateOrganizer(int id, int organizer)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [Organizer] = @Organizer
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);
        db.Parameters.Add("@Organizer", organizer);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateProgressStatus(string projectID, int status)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [ProgressStatus] = @ProgressStatus
                  ,[Status] = @Status
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@ProgressStatus", status);
        db.Parameters.Add("@Status", (status * 10) + 1);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateSchedule(OFS_LitProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
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

    public static void updateStatus(string projectID, int status)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [Status] = @Status
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Status", status);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    public static void updateWithdrawalStatus(string projectID, bool isWithdrawal)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_LIT_Project]
               SET [IsWithdrawal] = @IsWithdrawal
                  ,[UpdateTime] = GETDATE()
                  ,[UpdateUser] = @UpdateUser
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@IsWithdrawal", isWithdrawal);
        db.Parameters.Add("@UpdateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }

    private static OFS_LitProject toModel(DataRow row)
    {
        return new OFS_LitProject
        {
            ID = row.Field<int>("ID"),
            Year = row.Field<int>("Year"),
            ProjectID = row.Field<string>("ProjectID"),
            SubsidyPlanType = row.Field<string>("SubsidyPlanType"),
            ProjectName = row.Field<string>("ProjectName"),
            Field = row.Field<string>("Field"),
            OrgName = row.Field<string>("OrgName"),
            OrgLeader = row.Field<string>("OrgLeader"),
            Address = row.Field<string>("Address"),
            Target = row.Field<string>("Target"),
            Summary = row.Field<string>("Summary"),
            Quantified = row.Field<string>("Quantified"),
            Qualitative = row.Field<string>("Qualitative"),
            StartTime = row.Field<DateTime?>("StartTime"),
            EndTime = row.Field<DateTime?>("EndTime"),
            ApplyAmount = row.Field<int?>("ApplyAmount"),
            SelfAmount = row.Field<int?>("SelfAmount"),
            OtherAmount = row.Field<int?>("OtherAmount"),
            ApprovedAmount = row.Field<int?>("ApprovedAmount"),
            RecoveryAmount = row.Field<int?>("RecoveryAmount"),
            Benefit = row.Field<string>("Benefit"),
            FormStep = row.Field<int>("FormStep"),
            Status = row.Field<int>("Status"),
            ProgressStatus = row.Field<int>("ProgressStatus"),
            Organizer = row.Field<int?>("Organizer"),
            OrganizerName = row.Field<string>("OrganizerName"),
            RejectReason = row.Field<string>("RejectReason"),
            CorrectionDeadline = row.Field<DateTime?>("CorrectionDeadline"),
            UserAccount = row.Field<string>("UserAccount"),
            UserName = row.Field<string>("UserName"),
            UserOrg = row.Field<string>("UserOrg"),
            IsProjChanged = row.Field<bool>("IsProjChanged"),
            IsWithdrawal = row.Field<bool>("IsWithdrawal"),
            IsExists = row.Field<bool>("IsExists"),
            FinalReviewNotes = row.Field<string>("FinalReviewNotes"),
            FinalReviewOrder = row.Field<int?>("FinalReviewOrder")
        };
    }
}

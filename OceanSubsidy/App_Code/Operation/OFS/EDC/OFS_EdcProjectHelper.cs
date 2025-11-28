using GS.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_EdcProjectHelper
{
    public static int count(int year, string orgName = "")
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT COUNT(*) AS Count
              FROM [OFS_EDC_Project]
             WHERE [Year] = @Year
        ";

        db.Parameters.Add("@Year", year);

        if (!string.IsNullOrWhiteSpace(orgName))
        {
            db.CommandText += " AND [OrgName] = @OrgName AND IsExists = 1 AND IsWithdrawal = 0";
            db.Parameters.Add("@OrgName", orgName);
        }

        return int.Parse(db.GetTable().Rows[0]["Count"].ToString());
    }

    public static OFS_EdcProject get(int id)
    {
        return query(new OFS_EdcProject { ID = id }, false).FirstOrDefault();
    }

    public static string getUserAccount(string projectID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [UserAccount]
              FROM [OFS_EDC_Project]
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
              FROM [OFS_EDC_Project]
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? table.Rows[0].Field<int>("ID") : 0;
    }

    public static void insert(OFS_EdcProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_EDC_Project] ([Year],[ProjectID],[SubsidyPlanType],[ProjectName],[OrgCategory],[OrgName],[RegisteredNum],[TaxID],[Address],[StartTime],
                                           [EndTime],[Target],[Summary],[Quantified],[ApplyAmount],[SelfAmount],[OtherGovAmount],[OtherUnitAmount],[FormStep],[Status],
                                           [ProgressStatus],[UserAccount],[UserName],[UserOrg],[IsProjChanged],[IsWithdrawal],[IsExists],[CreateTime],[CreateUser])
                OUTPUT Inserted.ID VALUES (@Year, @ProjectID, @SubsidyPlanType, @ProjectName, @OrgCategory, @OrgName, @RegisteredNum, @TaxID, @Address, @StartTime,
                                           @EndTime, @Target, @Summary, @Quantified, @ApplyAmount, @SelfAmount, @OtherGovAmount, @OtherUnitAmount, 1,         1,
                                           0,               @UserAccount, @UserName, @UserOrg, 0,              0,             1,         GETDATE(),   @CreateUser)
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

    public static List<OFS_EdcProject> query(OFS_EdcProject conditions, bool valid = true)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT P.[ID]
                  ,P.[Year]
                  ,P.[ProjectID]
                  ,P.[SubsidyPlanType]
                  ,P.[ProjectName]
                  ,P.[OrgCategory]
                  ,P.[OrgName]
                  ,P.[RegisteredNum]
                  ,P.[TaxID]
                  ,P.[Address]
                  ,P.[StartTime]
                  ,P.[EndTime]
                  ,P.[Target]
                  ,P.[Summary]
                  ,P.[Quantified]
                  ,P.[ApplyAmount]
                  ,P.[SelfAmount]
                  ,P.[OtherGovAmount]
                  ,P.[OtherUnitAmount]
                  ,P.[ApprovedAmount]
                  ,P.[RecoveryAmount]
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
                  ,P.[ApplyTime]
              FROM [OFS_EDC_Project] AS P
         LEFT JOIN [Sys_User] AS U ON (U.UserID = P.Organizer)
        ";

        db.CommandText += valid ? " WHERE [IsExists] = 1 AND [IsWithdrawal] = 0" : " WHERE 1 = 1";

        if (conditions.ID > 0)
        {
            db.CommandText += " AND P.[ID] = @ID";
            db.Parameters.Add("@ID", conditions.ID);
        }

        if (conditions.Year > 0)
        {
            db.CommandText += " AND P.[Year] = @Year";
            db.Parameters.Add("@Year", conditions.Year);
        }

        if (conditions.Status > 0)
        {
            db.CommandText += " AND P.[Status] = @Status";
            db.Parameters.Add("@Status", conditions.Status);
        }

        if (conditions.ProgressStatus > 0)
        {
            db.CommandText += " AND P.[ProgressStatus] = @ProgressStatus";
            db.Parameters.Add("@ProgressStatus", conditions.ProgressStatus);
        }

        if (conditions.EndTime.HasValue)
        {
            db.CommandText += " AND P.[EndTime] = @EndTime";
            db.Parameters.Add("@EndTime", conditions.EndTime.Value);
        }

        if (conditions.CorrectionDeadline.HasValue)
        {
            db.CommandText += " AND P.[CorrectionDeadline] = @CorrectionDeadline";
            db.Parameters.Add("@CorrectionDeadline", conditions.CorrectionDeadline.Value);
        }

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void reviewApplication(OFS_EdcProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
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
            UPDATE [OFS_EDC_Project]
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
            UPDATE [OFS_EDC_Project]
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

    public static void updateApplyTime(string projectID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
               SET [ApplyTime] = GETDATE()
             WHERE [ProjectID] = @ProjectID
               AND [ApplyTime] IS NULL
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.ExecuteNonQuery();
    }

    public static void updateApprovedAmount(string projectID, int amount, string notes)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
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

    public static void updateExistsStatus(string projectID, bool isExists)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
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
            UPDATE [OFS_EDC_Project]
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

    public static void updateLastOperation(string projectID, string lastOperation)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
               SET [LastOperation] = @LastOperation
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@LastOperation", lastOperation);

        db.ExecuteNonQuery();
    }

    public static void updateOrganizer(int id, int organizer)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
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
            UPDATE [OFS_EDC_Project]
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

    public static void updateStatus(string projectID, int status)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_EDC_Project]
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
            UPDATE [OFS_EDC_Project]
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
            ApplyAmount = row.Field<int?>("ApplyAmount"),
            SelfAmount = row.Field<int?>("SelfAmount"),
            OtherGovAmount = row.Field<int?>("OtherGovAmount"),
            OtherUnitAmount = row.Field<int?>("OtherUnitAmount"),
            ApprovedAmount = row.Field<int?>("ApprovedAmount"),
            RecoveryAmount = row.Field<int?>("RecoveryAmount"),
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
            FinalReviewOrder = row.Field<int?>("FinalReviewOrder"),
            ApplyTime = row.Field<DateTime?>("ApplyTime")
        };
    }
}

using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSReviewCommitteeHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_ReviewCommitteeList]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static ReviewCommittee getByToken(string token)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[CommitteeUser]
                  ,[Email]
                  ,[SubjectTypeID]
                  ,[Token]
                  ,[BankCode]
                  ,[BankAccount]
                  ,[RegistrationAddress]
              FROM [OFS_ReviewCommitteeList]
             WHERE [Token] = @Token
        ";

        db.Parameters.Add("@Token", token);

        var table = db.GetTable();

        return table.Rows.Count == 1 ? toModel(table.Rows[0]) : null;
    }

    public static void insert(ReviewCommittee model)
    {
        string guid = Guid.NewGuid().ToString("N");
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        model.Token = $"{guid}_{timestamp}";

        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_ReviewCommitteeList] ([CommitteeUser],[Email],[SubjectTypeID],[Token])
                        OUTPUT Inserted.ID VALUES (@CommitteeUser, @Email, @SubjectTypeID, @Token)
        ";

        db.Parameters.Add("@CommitteeUser", model.CommitteeUser);
        db.Parameters.Add("@Email", model.Email);
        db.Parameters.Add("@SubjectTypeID", model.SubjectTypeID);
        db.Parameters.Add("@Token", model.Token);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<ReviewCommittee> query(string subjectTypeID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[CommitteeUser]
                  ,[Email]
                  ,[SubjectTypeID]
                  ,[Token]
                  ,[BankCode]
                  ,[BankAccount]
                  ,[RegistrationAddress]
              FROM [OFS_ReviewCommitteeList]
             WHERE [SubjectTypeID] = @SubjectTypeID
        ";

        db.Parameters.Add("@SubjectTypeID", subjectTypeID);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static List<ReviewerDetail> queryReviewerDetails(int type, int id)
    {
        DbHelper db = new DbHelper();

        if (type == 1)
        {
            db.CommandText = @"
                SELECT C.[CommitteeUser]
                      ,R.[ReviewStage]
                      ,P.[FieldName]
                      ,P.[ProjectID]
                      ,P.[ProjectName]
                  FROM [OFS_ReviewCommitteeList] AS C
                  JOIN [OFS_ReviewRecords] AS R ON (R.[Email] = C.[Email])
            ";
        }
        else
        {
            db.CommandText = @"
                SELECT C.[Reviewer] AS [CommitteeUser]
                      ,CASE WHEN R.[Stage] = 1 THEN '期中' ELSE '期末' END AS [ReviewStage]
                      ,P.[FieldName]
                      ,P.[ProjectID]
                      ,P.[ProjectName]
                  FROM [OFS_SCI_StageExam_ReviewerList] AS C
                  JOIN [OFS_SCI_StageExam] AS R ON (R.[id] = C.[ExamID])
            ";
        }

        db.CommandText += @"
             JOIN (SELECT P.[ProjectID]
                         ,P.[ProjectNameTw] AS [ProjectName]
                         ,'科專／' + C.[Descname] AS [FieldName]
                     FROM [OFS_SCI_Application_Main] AS P
                     JOIN [OFS_SCI_Project_Main] AS M ON (M.ProjectID = P.ProjectID)
                     JOIN [Sys_ZgsCode] AS C ON (C.[CodeGroup] = 'SCIField' AND C.[Code] = P.[Field])
                    WHERE M.[isExist] = 1
                      AND M.[isWithdrawal] = 0
                   UNION
                   SELECT P.[ProjectID]
                         ,P.[ProjectName]
                         ,'文化／' + M.[Descname] + '-' + C.[Descname] AS [FieldName]
                     FROM [OFS_CUL_Project] AS P
                     JOIN [Sys_ZgsCode] AS C ON (C.[CodeGroup] = 'CULField' AND C.[Code] = P.[Field])
                     JOIN [Sys_ZgsCode] AS M ON (M.[CodeGroup] = 'CULField' AND M.[Code] = C.[ParentCode])
                    WHERE P.[isExists] = 1
                      AND P.[isWithdrawal] = 0) AS P ON (P.[ProjectID] = R.[ProjectID])
            WHERE C.ID = @ID
        ";

        db.Parameters.Add("@ID", id);

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ReviewerDetail
        {
            CommitteeUser = row.Field<string>("CommitteeUser"),
            ReviewStage = row.Field<string>("ReviewStage"),
            FieldName = row.Field<string>("FieldName"),
            ProjectID = row.Field<string>("ProjectID"),
            ProjectName = row.Field<string>("ProjectName")
        }).ToList();
    }

    public static List<ReviewCommittee> queryReviewerList(int type, string keyword, DateTime? begin, DateTime? end)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT A.[ID]
                  ,A.[CommitteeUser]
                  ,A.[Email]
                  ,A.[Type]
                  ,A.[Count]
                  ,A.[BankCode]
                  ,Z.[Descname] AS [BankName]
                  ,A.[BankAccount]
                  ,A.[RegistrationAddress]
                  ,A.[UpdateTime]
              FROM (SELECT C.[ID]
                          ,C.[CommitteeUser]
                          ,C.[Email]
                          ,1 AS [Type]
                          ,R.[Count]
                          ,C.[BankCode]
                          ,C.[BankAccount]
                          ,C.[RegistrationAddress]
                          ,C.[UpdateTime]
                      FROM [OFS_ReviewCommitteeList] AS C
                      JOIN (SELECT [Email], COUNT(*) AS [Count] FROM [OFS_ReviewRecords] GROUP BY [Email]) AS R ON (R.[Email] = C.[Email])
                    UNION
                    SELECT [id] AS [ID]
                          ,[Reviewer] AS [CommitteeUser]
                          ,[Account] AS [Email]
                          ,2 AS [Type]
                          ,1 AS [Count]
                          ,[BankCode]
                          ,[BankAccount]
                          ,[RegistrationAddress]
                          ,[UpdateTime]
                      FROM [OFS_SCI_StageExam_ReviewerList]) AS A
         LEFT JOIN [Sys_ZgsCode] AS Z ON (Z.[CodeGroup] = 'BankCode' AND Z.[Code] = A.[BankCode])
             WHERE 1 = 1
        ";

        if (type > 0)
        {
            db.CommandText += " AND A.Type = @Type";
            db.Parameters.Add("@Type", type);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            db.CommandText += " AND (A.CommitteeUser LIKE @Keyword OR A.Email LIKE @Keyword)";
            db.Parameters.Add("@Keyword", $"%{keyword}%");
        }

        if (begin.HasValue)
        {
            db.CommandText += " AND CAST(A.UpdateTime AS DATE) >= @Begin";
            db.Parameters.Add("@Begin", begin.Value);
        }

        if (end.HasValue)
        {
            db.CommandText += " AND CAST(A.UpdateTime AS DATE) <= @End";
            db.Parameters.Add("@End", end.Value);
        }

        db.CommandText += " ORDER BY A.[UpdateTime] DESC";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ReviewCommittee
        {
            ID = row.Field<int>("ID"),
            CommitteeUser = row.Field<string>("CommitteeUser"),
            Email = row.Field<string>("Email"),
            Type = row.Field<int>("Type"),
            Count = row.Field<int>("Count"),
            BankCode = row.Field<string>("BankCode"),
            BankName = row.Field<string>("BankName"),
            BankAccount = row.Field<string>("BankAccount"),
            RegistrationAddress = row.Field<string>("RegistrationAddress"),
            UpdateTime = row.Field<DateTime?>("UpdateTime")
        }).ToList();
    }

    public static void update(ReviewCommittee model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_ReviewCommitteeList]
               SET [CommitteeUser] = @CommitteeUser
                  ,[Email] = @Email
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@CommitteeUser", model.CommitteeUser);
        db.Parameters.Add("@Email", model.Email);

        db.ExecuteNonQuery();
    }

    public static void updateBankInfo(string token, string bankCode, string bankAccount, string registrationAddress)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_ReviewCommitteeList]
               SET [BankCode] = @BankCode
                  ,[BankAccount] = @BankAccount
                  ,[RegistrationAddress] = @RegistrationAddress
                  ,[UpdateTime] = GETDATE()
             WHERE [Token] = @Token
        ";

        db.Parameters.Add("@Token", token);
        db.Parameters.Add("@BankCode", bankCode);
        db.Parameters.Add("@BankAccount", bankAccount);
        db.Parameters.Add("@RegistrationAddress", registrationAddress);

        db.ExecuteNonQuery();
    }

    private static ReviewCommittee toModel(DataRow row)
    {
        return new ReviewCommittee
        {
            ID = row.Field<int>("ID"),
            CommitteeUser = row.Field<string>("CommitteeUser"),
            Email = row.Field<string>("Email"),
            SubjectTypeID = row.Field<string>("SubjectTypeID"),
            Token = row.Field<string>("Token"),
            BankCode = row.Field<string>("BankCode"),
            BankAccount = row.Field<string>("BankAccount"),
            RegistrationAddress = row.Field<string>("RegistrationAddress")
        };
    }
}

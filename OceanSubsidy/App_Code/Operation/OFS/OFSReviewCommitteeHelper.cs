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

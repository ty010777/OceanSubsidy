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

    public static void insert(ReviewCommittee model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_ReviewCommitteeList] ([CommitteeUser],[Email],[SubjectTypeID])
                        OUTPUT Inserted.ID VALUES (@CommitteeUser, @Email, @SubjectTypeID)
        ";

        db.Parameters.Add("@CommitteeUser", model.CommitteeUser);
        db.Parameters.Add("@Email", model.Email);
        db.Parameters.Add("@SubjectTypeID", model.SubjectTypeID);

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

    private static ReviewCommittee toModel(DataRow row)
    {
        return new ReviewCommittee
        {
            ID = row.Field<int>("ID"),
            CommitteeUser = row.Field<string>("CommitteeUser"),
            Email = row.Field<string>("Email"),
            SubjectTypeID = row.Field<string>("SubjectTypeID")
        };
    }
}

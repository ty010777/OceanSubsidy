using GS.Data.Sql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFS_CulRelatedProjectHelper
{
    public static void delete(int id)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            DELETE
              FROM [OFS_CUL_Related_Project]
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", id);

        db.ExecuteNonQuery();
    }

    public static void insert(OFS_CulRelatedProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_CUL_Related_Project] ([PID],[Title],[Year],[OrgName],[Amount],[Description],[Benefit])
                        OUTPUT Inserted.ID VALUES (@PID ,@Title ,@Year ,@OrgName ,@Amount ,@Description ,@Benefit)
        ";

        db.Parameters.Add("@PID", model.PID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@OrgName", model.OrgName);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@Benefit", model.Benefit);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }

    public static List<OFS_CulRelatedProject> query(int pid)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[PID]
                  ,[Title]
                  ,[Year]
                  ,[OrgName]
                  ,[Amount]
                  ,[Description]
                  ,[Benefit]
              FROM [OFS_CUL_Related_Project]
             WHERE [PID] = @PID
          ORDER BY [ID]
        ";

        db.Parameters.Add("@PID", pid);

        return db.GetTable().Rows.Cast<DataRow>().Select(r => toModel(r)).ToList();
    }

    public static void update(OFS_CulRelatedProject model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            UPDATE [OFS_CUL_Related_Project]
               SET [Title] = @Title
                  ,[Year] = @Year
                  ,[OrgName] = @OrgName
                  ,[Amount] = @Amount
                  ,[Description] = @Description
                  ,[Benefit] = @Benefit
             WHERE [ID] = @ID
        ";

        db.Parameters.Add("@ID", model.ID);
        db.Parameters.Add("@Title", model.Title);
        db.Parameters.Add("@Year", model.Year);
        db.Parameters.Add("@OrgName", model.OrgName);
        db.Parameters.Add("@Amount", model.Amount);
        db.Parameters.Add("@Description", model.Description);
        db.Parameters.Add("@Benefit", model.Benefit);

        db.ExecuteNonQuery();
    }

    private static OFS_CulRelatedProject toModel(DataRow row)
    {
        return new OFS_CulRelatedProject
        {
            ID = row.Field<int>("ID"),
            PID = row.Field<int>("PID"),
            Title = row.Field<string>("Title"),
            Year = row.Field<int>("Year"),
            OrgName = row.Field<string>("OrgName"),
            Amount = row.Field<int>("Amount"),
            Description = row.Field<string>("Description"),
            Benefit = row.Field<string>("Benefit")
        };
    }
}

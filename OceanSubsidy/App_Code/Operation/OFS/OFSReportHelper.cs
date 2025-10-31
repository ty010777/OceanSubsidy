using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

public class OFSReportHelper
{
    public class StageExamStatus
    {
        public string Status { get; set; }
        public string ReviewMethod { get; set; }
        public string Reviewer { get; set; }
        public string Account { get; set; }
    }

    public static StageExamStatus GetStageExamStatus(string projectID, int stage)
    {
        DbHelper db = new DbHelper();

        // 1. 先檢查是否有任何一筆記錄的 Status = '通過'

        db.CommandText = @"
            SELECT TOP 1 [Status]
                  ,[ReviewMethod]
                  ,[Reviewer]
                  ,[Account]
              FROM [OFS_StageExam]
             WHERE [ProjectID] = @ProjectID
               AND [Stage] = @Stage
               AND [Status] = N'通過'
        ";

        db.Parameters.Clear();

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);

        DataTable dt = db.GetTable();

        if (dt.Rows.Count == 0)
        {
            // 2. 沒有「通過」記錄，查詢最新版本的審查狀態

            db.CommandText = @"
                SELECT TOP 1 [Status]
                      ,[ReviewMethod]
                      ,[Reviewer]
                      ,[Account]
                  FROM [OFS_StageExam]
                 WHERE [ProjectID] = @ProjectID
                   AND [Stage] = @Stage
              ORDER BY [ExamVersion] DESC
            ";

            db.Parameters.Clear();

            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@Stage", stage);

            dt = db.GetTable();

            if (dt.Rows.Count == 0)
            {
                return new StageExamStatus { Status = "", ReviewMethod = "", Reviewer = "", Account = "" };
            }
        }

        DataRow row = dt.Rows[0];

        return new StageExamStatus
        {
            Status = row["Status"]?.ToString() ?? "",
            ReviewMethod = row["ReviewMethod"]?.ToString() ?? "",
            Reviewer = row["Reviewer"]?.ToString() ?? "",
            Account = row["Account"]?.ToString() ?? ""
        };
    }

    public static void ReviewStageExam(string projectID, int stage, string reviewMethod, string status, string reviewComment, string reviewer, string account)
    {
        DbHelper db = new DbHelper();

        // 先取得最新版本號

        db.CommandText = @"
            SELECT TOP 1 [ExamVersion]
              FROM [OFS_StageExam]
             WHERE [ProjectID] = @ProjectID
               AND [Stage] = @Stage
          ORDER BY [ExamVersion] DESC
        ";

        db.Parameters.Clear();

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);

        DataTable dt = db.GetTable();

        if (dt.Rows.Count == 0)
        {
            throw new Exception("找不到對應的階段審查記錄");
        }

        int latestVersion = Convert.ToInt32(dt.Rows[0]["ExamVersion"]);

        // 更新最新版本的記錄

        db.CommandText = @"
            UPDATE [OFS_StageExam]
               SET [ReviewMethod] = @ReviewMethod
                  ,[Status] = @Status
                  ,[Reviewer] = @Reviewer
                  ,[Account] = @Account
                  ,[update_at] = @update_at
             WHERE [ProjectID] = @ProjectID
               AND [Stage] = @Stage
               AND [ExamVersion] = @ExamVersion
        ";

        db.Parameters.Clear();

        db.Parameters.Add("@ReviewMethod", reviewMethod);
        db.Parameters.Add("@Status", status);
        db.Parameters.Add("@Reviewer", reviewer);
        db.Parameters.Add("@Account", account);
        db.Parameters.Add("@update_at", DateTime.Now);
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);
        db.Parameters.Add("@ExamVersion", latestVersion);

        db.ExecuteNonQuery();
    }

    public static void SubmitStageExam(string projectID, int stage, string status)
    {
        DbHelper db = new DbHelper();

        // 取得下一個版本號

        int nextVersion = GetNextExamVersion(projectID, stage);

        // 每次都新增記錄

        db.CommandText = @"
            INSERT INTO [OFS_StageExam] ([ProjectID],[Stage],[ExamVersion],[Status],[create_at],[update_at])
                                 VALUES (@ProjectID, @Stage, @ExamVersion, @Status, @create_at, @update_at)
        ";

        db.Parameters.Clear();

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);
        db.Parameters.Add("@ExamVersion", nextVersion);
        db.Parameters.Add("@Status", status);
        db.Parameters.Add("@create_at", DateTime.Now);
        db.Parameters.Add("@update_at", DateTime.Now);

        db.ExecuteNonQuery();
    }

    private static int GetNextExamVersion(string projectID, int stage)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT MAX([ExamVersion])
              FROM [OFS_StageExam]
             WHERE [ProjectID] = @ProjectID
               AND [Stage] = @Stage
        ";

        db.Parameters.Clear();

        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@Stage", stage);

        DataTable dt = db.GetTable();

        if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
        {
            return Convert.ToInt32(dt.Rows[0][0]) + 1;
        }

        return 0; // 第一次提送，版本號為 0
    }
}

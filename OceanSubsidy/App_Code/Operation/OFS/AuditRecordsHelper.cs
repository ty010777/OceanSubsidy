using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// 查核紀錄資料存取協助類別
/// </summary>
public class AuditRecordsHelper
{
    public AuditRecordsHelper()
    {

    }


    /// <summary>
    /// 根據 ProjectID 取得 CLB 計畫基本資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>計畫基本資料</returns>
    public static GisTable CLB_GetProjectBasicData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT TOP (1000) AB.[ProjectID]
                  ,[ProjectNameTw]
                  ,[SchoolName]
                  ,[ClubName]
                  ,(ISNULL([SchoolName], '') + ' ' + ISNULL([ClubName], '')) AS [OrgName]
                 ,StartDate AS [StartTime]
                 ,EndDate AS [EndTime]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Application_Basic] AB
            LEFT JOIN OFS_CLB_Application_Plan AP on AB.ProjectID = AP.ProjectID
            WHERE AB.[ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        return db.GetTable();
    }

    /// <summary>
    /// 根據 ProjectID 取得 SCI 計畫基本資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>計畫基本資料</returns>
    public static GisTable SCI_GetProjectBasicData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT TOP 1
                [ProjectID],
                [ProjectNameTw],
                [OrgName],
                [StartTime],
                [EndTime]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        return db.GetTable();
    }

    public static GisTable Other_GetProjectBasicData(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [ProjectID], [ProjectName] AS [ProjectNameTw], [OrgName], [StartTime], [EndTime] FROM [OFS_CUL_Project]
            UNION
            SELECT [ProjectID], [ProjectName] AS [ProjectNameTw], [OrgName], [StartTime], [EndTime] FROM [OFS_EDC_Project]
            UNION
            SELECT [ProjectID], [ProjectName] AS [ProjectNameTw], [OrgName], [StartTime], [EndTime] FROM [OFS_MUL_Project]
            UNION
            SELECT [ProjectID], [ProjectName] AS [ProjectNameTw], [OrgName], [StartTime], [EndTime] FROM [OFS_LIT_Project]
            UNION
            SELECT [ProjectID], [ProjectName] AS [ProjectNameTw], [OrgName], [StartTime], [EndTime] FROM [OFS_ACC_Project]
             WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        return db.GetTable();
    }

    /// <summary>
    /// 根據 ProjectID 取得查核紀錄列表
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>查核紀錄列表</returns>
    public static GisTable GetAuditRecordsByProjectID(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT
                [idx],
                [ProjectID],
                [ReviewerName],
                [CheckDate],
                [Risk],
                [ReviewerComment],
                [ExecutorComment],
                [create_at],
                [update_at]
            FROM [OCA_OceanSubsidy].[dbo].[OFS_AuditRecords]
            WHERE [ProjectID] = @ProjectID
            ORDER BY [CheckDate] DESC, [create_at] DESC";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);

        return db.GetTable();
    }

    /// <summary>
    /// 新增查核紀錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="reviewerName">查核人員</param>
    /// <param name="checkDate">查核日期</param>
    /// <param name="risk">風險評估</param>
    /// <param name="reviewerComment">查核意見</param>
    /// <returns>是否成功</returns>
    public static void InsertAuditRecord(string projectID, string reviewerName, DateTime? checkDate, string risk, string reviewerComment)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_AuditRecords]
                ([ProjectID]
                ,[ReviewerName]
                ,[CheckDate]
                ,[Risk]
                ,[ReviewerComment]
                ,[ExecutorComment]
                ,[create_at]
                ,[update_at])
            VALUES
                (@ProjectID
                ,@ReviewerName
                ,@CheckDate
                ,@Risk
                ,@ReviewerComment
                ,@ExecutorComment
                ,@create_at
                ,@update_at)";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID ?? "");
        db.Parameters.Add("@ReviewerName", reviewerName ?? "");
        db.Parameters.Add("@CheckDate", checkDate);
        db.Parameters.Add("@Risk", risk ?? "");
        db.Parameters.Add("@ReviewerComment", reviewerComment ?? "");
        db.Parameters.Add("@ExecutorComment", ""); // 初始為空，等待執行單位回覆
        db.Parameters.Add("@create_at", DateTime.Now);
        db.Parameters.Add("@update_at", DateTime.Now);

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新執行單位回覆
    /// </summary>
    /// <param name="idx">紀錄編號</param>
    /// <param name="executorComment">執行單位回覆</param>
    /// <returns>是否成功</returns>
    public static void UpdateExecutorComment(int idx, string executorComment)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_AuditRecords]
            SET [ExecutorComment] = @ExecutorComment,
                [update_at] = @UpdateTime
            WHERE [idx] = @idx";

        db.Parameters.Clear();
        db.Parameters.Add("@ExecutorComment", executorComment ?? "");
        db.Parameters.Add("@UpdateTime", DateTime.Now);
        db.Parameters.Add("@idx", idx);

        db.ExecuteNonQuery();
    }

    public static List<OFS_AuditRecords> GetAuditRecordsByOrgName(string name)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT A.[idx]
                  ,A.[ProjectID]
                  ,B.[ProjectName]
                  ,A.[ReviewerName]
                  ,A.[CheckDate]
                  ,A.[Risk]
                  ,A.[ReviewerComment]
                  ,A.[ExecutorComment]
              FROM [OFS_AuditRecords] AS A
              JOIN (SELECT [ProjectID], [ProjectNameTw] AS [ProjectName], [OrgName] FROM [OFS_SCI_Application_Main]
                    UNION
                    SELECT [ProjectID], [ProjectName], [OrgName] FROM [OFS_CUL_Project]
                    UNION
                    SELECT [ProjectID], [ProjectName], [OrgName] FROM [OFS_EDC_Project]
                    UNION
                    SELECT [ProjectID], [ProjectNameTw] AS [ProjectName], [SchoolName] + [ClubName] AS [OrgName] FROM [OFS_CLB_Application_Basic]
                    UNION
                    SELECT [ProjectID], [ProjectName], [OrgName] FROM [OFS_MUL_Project]
                    UNION
                    SELECT [ProjectID], [ProjectName], [OrgName] FROM [OFS_LIT_Project]
                    UNION
                    SELECT [ProjectID], [ProjectName], [OrgName] FROM [OFS_ACC_Project]) AS B ON (B.ProjectID = A.ProjectID)
              WHERE B.OrgName = @OrgName
           ORDER BY A.[CheckDate] DESC
        ";

        db.Parameters.Add("@OrgName", name);

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new OFS_AuditRecords
        {
            idx = row.Field<int>("idx"),
            ProjectID = row.Field<string>("ProjectID"),
            ProjectName = row.Field<string>("ProjectName"),
            ReviewerName = row.Field<string>("ReviewerName"),
            CheckDate = row.Field<DateTime?>("CheckDate"),
            Risk = row.Field<string>("Risk"),
            ReviewerComment = row.Field<string>("ReviewerComment"),
            ExecutorComment = row.Field<string>("ExecutorComment")
        }).ToList();
    }
}

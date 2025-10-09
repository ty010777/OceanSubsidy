using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ReportHelper
{
    // TODO 嘉良: approved=0 申請計畫報表, approved=1 核定計畫報表, approved=2 執行計畫報表
    public static List<ApplyPlan> queryApplyList(int approved = 0)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT O.[Year]
                  ,O.[ProjectID]
                  ,O.[Category]
                  ,O.[ProjectName]
                  ,O.[UserOrg]
                  ,O.[ApprovedAmount]
                  ,O.[ApplyAmount]
                  ,O.[OtherAmount]
                  ,O.[SpendAmount]
                  ,O.[PaymentAmount]
                  ,U.UnitName AS [SupervisoryUnit]
                  ,P.Descname AS [StageName]
                  ,O.[Status]
                  ,S.Descname AS [StatusName]
              FROM (SELECT [Year]
                          ,[ProjectID]
                          ,'文化' AS [Category]
                          ,[ProjectName]
                          ,[UserOrg]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                      FROM OFS_CUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'學校／民間' AS [Category]
                          ,[ProjectName]
                          ,[UserOrg]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherGovAmount],0) + ISNULL([OtherUnitAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                      FROM OFS_EDC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'多元' AS [Category]
                          ,[ProjectName]
                          ,[UserOrg]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                      FROM OFS_MUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'素養' AS [Category]
                          ,[ProjectName]
                          ,[UserOrg]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                      FROM OFS_LIT_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'無障礙' AS [Category]
                          ,[ProjectName]
                          ,[UserOrg]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                      FROM OFS_ACC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1) AS O
         LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
         LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
              JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
              JOIN Sys_ZgsCode AS P ON (P.CodeGroup = 'ProjectProgressStatus' AND P.Code = O.ProgressStatus)
        ";

        switch (approved)
        {
            case 1:
                db.CommandText += " WHERE O.[Status] IN (45,51,52,91)";
                break;
            case 2:
                db.CommandText += " WHERE O.[Status] IN (51,52,91)";
                break;
        }

        db.CommandText += " ORDER BY ProjectID";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ApplyPlan
        {
            Year = row.Field<int>("Year"),
            ProjectID = row.Field<string>("ProjectID"),
            Category = row.Field<string>("Category"),
            ProjectName = row.Field<string>("ProjectName"),
            UserOrg = row.Field<string>("UserOrg"),
            ApprovedAmount = row.Field<int>("ApprovedAmount"),
            ApplyAmount = row.Field<int>("ApplyAmount"),
            OtherAmount = row.Field<int>("OtherAmount"),
            SpendAmount = row.Field<int>("SpendAmount"),
            PaymentAmount = row.Field<int>("PaymentAmount"),
            SupervisoryUnit = row.Field<string>("SupervisoryUnit"),
            StageName = row.Field<string>("StageName"),
            Status = row.Field<int>("Status"),
            StatusName = row.Field<string>("StatusName")
        }).ToList();
    }

    // TODO 嘉良: 首頁的統計區塊, 查詢指定使用的申請的補助案
    public static List<ApplyPlan> queryApplyListByUser(string account)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT O.[ProjectID], O.[Status], O.[UserAccount]
              FROM (SELECT [ProjectID], [Status], [UserAccount]
                      FROM OFS_CUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [ProjectID], [Status], [UserAccount]
                      FROM OFS_EDC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [ProjectID], [Status], [UserAccount]
                      FROM OFS_MUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [ProjectID], [Status], [UserAccount]
                      FROM OFS_LIT_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [ProjectID], [Status], [UserAccount]
                      FROM OFS_ACC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1) AS O
             WHERE O.[UserAccount] = @UserAccount
        ";

        db.Parameters.Add("@UserAccount", account);

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ApplyPlan
        {
            ProjectID = row.Field<string>("ProjectID"),
            Status = row.Field<int>("Status")
        }).ToList();
    }

    // TODO 嘉良: 首頁的統計區塊, 計算各類申請案的 [核定補助經費], [實支金額], [已撥付], [核定件數], [總預算]
    public static List<ApplyPlan> queryApplyStat()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT O.[Year]
                  ,O.[Category]
                  ,SUM(O.[ApprovedAmount]) AS [ApprovedAmount]
                  ,SUM(O.[SpendAmount]) AS [SpendAmount]
                  ,SUM(O.[PaymentAmount]) AS [PaymentAmount]
                  ,COUNT(*) AS [Count]
                  ,ISNULL(T.[BudgetFees], 0) AS [BudgetFees]
              FROM (SELECT [Year]
                          ,'CUL' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_CUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION
                    SELECT [Year]
                          ,'EDC' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_EDC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION
                    SELECT [Year]
                          ,'MUL' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_MUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION
                    SELECT [Year]
                          ,'LIT' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_LIT_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION
                    SELECT [Year]
                          ,'ACC' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_ACC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)) AS O
         LEFT JOIN [OFS_GrantType] AS T ON (T.[TypeCode] = O.[Category] AND T.[Year] = O.[Year])
             WHERE 1 = 1
        ";

        db.CommandText += " GROUP BY O.[Year], O.[Category], T.[BudgetFees]";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ApplyPlan
        {
            Year = row.Field<int>("Year"),
            Category = row.Field<string>("Category"),
            ApprovedAmount = row.Field<int>("ApprovedAmount"),
            SpendAmount = row.Field<int>("SpendAmount"),
            PaymentAmount = row.Field<int>("PaymentAmount"),
            Count = row.Field<int>("Count"),
            BudgetFees = row.Field<int>("BudgetFees")
        }).ToList();
    }
}

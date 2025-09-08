using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ReportHelper
{
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
                          ,[ProjectID], '文化' AS [Category]
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
                          ,[ProjectID], '學校／民間' AS [Category]
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
                          ,[ProjectID], '多元' AS [Category]
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
                          ,[ProjectID], '素養' AS [Category]
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
                          ,[ProjectID], '無障礙' AS [Category]
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
}

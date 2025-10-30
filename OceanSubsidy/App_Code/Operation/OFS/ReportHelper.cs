using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ReportHelper
{
    // TODO 嘉良: approved=0 申請計畫報表, approved=1 核定計畫報表, approved=2 執行計畫報表
    public static List<ApplyPlan> queryApplyList(string account, int approved = 0)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
          WITH StatusMapping AS (
    SELECT '尚未提送' AS Statuses, '編輯中' AS StatusesName, 1 AS Status
    UNION ALL SELECT '資格審查', '審核中', 11
    UNION ALL SELECT '資格審查', '通過', 12
    UNION ALL SELECT '資格審查', '不通過', 13
    UNION ALL SELECT '資格審查', '補正補件', 14
    UNION ALL SELECT '資格審查', '逾期未補', 15
    UNION ALL SELECT '資格審查', '結案(未通過)', 19
    UNION ALL SELECT '內容審查', '審核中', 11
    UNION ALL SELECT '內容審查', '通過', 12
    UNION ALL SELECT '內容審查', '不通過', 13
    UNION ALL SELECT '內容審查', '補正補件', 14
    UNION ALL SELECT '內容審查', '逾期未補', 15
    UNION ALL SELECT '內容審查', '結案(未通過)', 19
    UNION ALL SELECT '領域審查', '審核中', 21
    UNION ALL SELECT '領域審查', '通過', 22
    UNION ALL SELECT '領域審查', '不通過', 23
    UNION ALL SELECT '領域審查', '結案(未通過)', 29
    UNION ALL SELECT '技術審查', '審核中', 31
    UNION ALL SELECT '技術審查', '通過', 32
    UNION ALL SELECT '技術審查', '不通過', 33
    UNION ALL SELECT '技術審查', '結案(未通過)', 39
    UNION ALL SELECT '決審核定', '核定中', 41
    UNION ALL SELECT '決審核定', '計畫書修正中', 42
    UNION ALL SELECT '決審核定', '計畫書審核中', 43
    UNION ALL SELECT '決審核定', '計畫書已確認', 44
    UNION ALL SELECT '決審核定', '已核定', 45
    UNION ALL SELECT '決審核定', '不通過', 46
    UNION ALL SELECT '決審核定', '結案(未通過)', 49
    UNION ALL SELECT '計畫執行', '', 51
    UNION ALL SELECT '計畫執行', '審核中', 51
    UNION ALL SELECT '計畫執行', '通過', 52
    UNION ALL SELECT '計畫執行', '不通過', 53
    UNION ALL SELECT '計畫執行', '已結案', 91
    UNION ALL SELECT '計畫執行', '已終止', 92
), FinalApplyList as (

            SELECT O.[Year]
                  ,O.[ProjectID]
                  ,O.[Category]
                  ,O.[ProjectName]
                  ,O.[OrgName]
                  ,O.[ApprovedAmount]
                  ,O.[ApplyAmount]
                  ,O.[OtherAmount]
                  ,O.[SpendAmount]
                  ,O.[PaymentAmount]
                  ,U.UnitName AS [SupervisoryUnit]
                  ,P.Descname AS [StageName]
                  ,O.[Status]
                  ,S.Descname AS [StatusName]
                  ,O.[UserAccount]
              FROM (SELECT [Year]
                          ,[ProjectID]
                          ,'文化' AS [Category]
                          ,[ProjectName]
                          ,[OrgName]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                          ,[UserAccount]
                      FROM OFS_CUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'學校／民間' AS [Category]
                          ,[ProjectName]
                          ,[OrgName]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherGovAmount],0) + ISNULL([OtherUnitAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                          ,[UserAccount]
                      FROM OFS_EDC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'多元' AS [Category]
                          ,[ProjectName]
                          ,[OrgName]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                          ,[UserAccount]
                      FROM OFS_MUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'素養' AS [Category]
                          ,[ProjectName]
                          ,[OrgName]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                          ,[UserAccount]
                      FROM OFS_LIT_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1
                    UNION
                    SELECT [Year]
                          ,[ProjectID]
                          ,'無障礙' AS [Category]
                          ,[ProjectName]
                          ,[OrgName]
                          ,[Organizer]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([ApplyAmount], 0) AS [ApplyAmount]
                          ,ISNULL([SelfAmount],0) + ISNULL([OtherAmount],0) AS [OtherAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                          ,[ProgressStatus]
                          ,[Status]
                          ,[UserAccount]
                      FROM OFS_ACC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1


					 ) AS O
         LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
         LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
              JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
              JOIN Sys_ZgsCode AS P ON (P.CodeGroup = 'ProjectProgressStatus' AND P.Code = O.ProgressStatus)

			  UNION

						 SELECT Year(PM.created_at)-1911 as Year,
						 PM.ProjectID ,
						  '科專' AS [Category],
						  ProjectNameTw AS [ProjectName] ,
						  AM.[OrgName],
						 ISNULL(PM.ApprovedSubsidy,0) AS ApprovedAmount,
						  ISNULL(PT.ApplyAmount,0) AS ApplyAmount,
						  ISNULL(PT.OtherAmount,0) AS OtherAmount,
						  ISNULL(PP.SpendAmount,0) AS SpendAmount,
						  ISNULL(PP.PaymentAmount,0) AS PaymentAmount,
						  PM.SupervisoryUnit,
						  PM.Statuses as StageName,
						  ISNULL(SM.Status,0) AS Status,
						  PM.StatusesName as StatusName,
                          PM.[UserAccount]
						  FROM OFS_SCI_Project_Main  PM
						  LEFT JOIN OFS_SCI_Application_Main AM ON  PM.ProjectID = AM.ProjectID
						  LEFT JOIN (
						  SELECT ProjectID,
						  SUM(SubsidyAmount) AS ApplyAmount ,
						  SUM(CoopAmount) AS OtherAmount
						  FROM OFS_SCI_PersonnelCost_TotalFee
						  GROUP BY ProjectID
						  )PT ON PT.ProjectID = PM.ProjectID
						  LEFT JOIN (
						  SELECT ProjectID,
						  SUM(TotalSpentAmount) AS SpendAmount,
						  SUM(CurrentActualPaidAmount) AS PaymentAmount
						  FROM OFS_SCI_Payment
						  Group by ProjectID
						  ) PP ON PP.ProjectID = PM.ProjectID
						  LEFT JOIN StatusMapping SM
						  ON PM.Statuses = SM.Statuses and PM.StatusesName = SM.StatusesName
						  WHERE PM.IsExist = 1 AND IsWithdrawal <> 1
			UNION
					 	SELECT  YEAR(PM.created_at) -1911 as Year ,
						PM.ProjectID,
						'學校社團' AS Category,
						ProjectNameTw AS [ProjectName],
						AB.[SchoolName] + AB.[ClubName] AS [OrgName],
						ISNULL(PM.ApprovedSubsidy,0) AS ApprovedAmount,
						ISNULL(AF.SubsidyFunds,0) AS ApplyAmount,
						ISNULL(SelfFunds,0) + ISNULL(OtherGovFunds,0) + ISNULL(OtherUnitFunds,0) AS [OtherAmount],
						ISNULL(CP.TotalSpentAmount,0) AS SpendAmount,
						ISNULL(CP.CurrentActualPaidAmount,0) AS PaymentAmount,
						PM.SupervisoryUnit AS SupervisoryUnit,
						PM.Statuses as StageName,
						ISNULL(SM.Status,0) AS Status,
						PM.StatusesName as StatusName,
                        PM.[UserAccount]
						 FROM OFS_CLB_Project_Main PM
						 LEFT JOIN OFS_CLB_Application_Basic AB ON PM.ProjectID = AB.ProjectID
						 LEFT JOIN OFS_CLB_Application_Funds AF ON PM.ProjectID = AF.ProjectID
						 LEFT JOIN OFS_CLB_Payment CP ON CP.ProjectID = PM.ProjectID
						 LEFT JOIN StatusMapping SM ON PM.Statuses = SM.Statuses and PM.StatusesName = SM.StatusesName
						 WHERE PM.IsExist = 1 AND IsWithdrawal <> 1
						 )
						 SELECT * FROM FinalApplyList
                          WHERE 1 = 1
";

        switch (approved)
        {
            case 1:
                db.CommandText += " AND [Status] IN (45,51,52,91)";
                break;
            case 2:
                db.CommandText += " AND [Status] IN (51,52,91)";
                break;
        }

        if (!string.IsNullOrWhiteSpace(account))
        {
            db.CommandText += " AND [UserAccount] = @UserAccount";
            db.Parameters.Add("@UserAccount", account);
        }

        db.CommandText += " ORDER BY ProjectID";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ApplyPlan
        {
            Year = Convert.ToInt32(row["Year"]),
            ProjectID = row.Field<string>("ProjectID"),
            Category = row.Field<string>("Category"),
            ProjectName = row.Field<string>("ProjectName"),
            OrgName = row.Field<string>("OrgName"),
            ApprovedAmount = Convert.ToInt32(row["ApprovedAmount"]),
            ApplyAmount = Convert.ToInt32(row["ApplyAmount"]),
            OtherAmount = Convert.ToInt32(row["OtherAmount"]),
            SpendAmount = Convert.ToInt32(row["SpendAmount"]),
            PaymentAmount = Convert.ToInt32(row["PaymentAmount"]),
            SupervisoryUnit = row.Field<string>("SupervisoryUnit"),
            StageName = row.Field<string>("StageName"),
            Status = Convert.ToInt32(row["Status"]),
            StatusName = row.Field<string>("StatusName")
        }).ToList();
    }

    // TODO 嘉良: 首頁的統計區塊, 查詢指定使用的申請的補助案
    public static List<ApplyPlan> queryApplyListByUser(string account)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT O.ProjectID,
               -- 統一 Status
               CASE
                    -- 尚未提送
                    WHEN O.Statuses = '尚未提送' AND O.StatusesName = '編輯中' THEN 1

                    -- 資格/內容審查
                    WHEN (O.Statuses = '資格審查' OR O.Statuses = '內容審查') AND O.StatusesName = '審核中' THEN 11
                    WHEN (O.Statuses = '資格審查' OR O.Statuses = '內容審查') AND O.StatusesName = '通過' THEN 12
                    WHEN (O.Statuses = '資格審查' OR O.Statuses = '內容審查') AND O.StatusesName = '不通過' THEN 13
                    WHEN (O.Statuses = '資格審查' OR O.Statuses = '內容審查') AND O.StatusesName = '補正補件' THEN 14
                    WHEN (O.Statuses = '資格審查' OR O.Statuses = '內容審查') AND O.StatusesName = '逾期未補' THEN 15
                    WHEN (O.Statuses = '資格審查' OR O.Statuses = '內容審查') AND O.StatusesName = '結案(未通過)' THEN 19

                    -- 領域審查
                    WHEN O.Statuses = '領域審查' AND O.StatusesName = '審核中' THEN 21
                    WHEN O.Statuses = '領域審查' AND O.StatusesName = '通過' THEN 22
                    WHEN O.Statuses = '領域審查' AND O.StatusesName = '不通過' THEN 23
                    WHEN O.Statuses = '領域審查' AND O.StatusesName = '結案(未通過)' THEN 29

                    -- 技術審查
                    WHEN O.Statuses = '技術審查' AND O.StatusesName = '審核中' THEN 31
                    WHEN O.Statuses = '技術審查' AND O.StatusesName = '通過' THEN 32
                    WHEN O.Statuses = '技術審查' AND O.StatusesName = '不通過' THEN 33
                    WHEN O.Statuses = '技術審查' AND O.StatusesName = '結案(未通過)' THEN 39

                    -- 決審核定
                    WHEN O.Statuses = '決審核定' AND O.StatusesName = '核定中' THEN 41
                    WHEN O.Statuses = '決審核定' AND O.StatusesName = '計畫書修正中' THEN 42
                    WHEN O.Statuses = '決審核定' AND O.StatusesName = '計畫書審核中' THEN 43
                    WHEN O.Statuses = '決審核定' AND O.StatusesName = '計畫書已確認' THEN 44
                    WHEN O.Statuses = '決審核定' AND O.StatusesName = '已核定' THEN 45
                    WHEN O.Statuses = '決審核定' AND O.StatusesName = '不通過' THEN 46
                    WHEN O.Statuses = '決審核定' AND O.StatusesName = '結案(未通過)' THEN 49

                    -- 計畫執行
                    WHEN O.Statuses = '計畫執行' AND (O.StatusesName = '' OR O.StatusesName = '審核中') THEN 51
                    WHEN O.Statuses = '計畫執行' AND O.StatusesName = '通過' THEN 52
                    WHEN O.Statuses = '計畫執行' AND O.StatusesName = '不通過' THEN 53
                    WHEN O.Statuses = '計畫執行' AND O.StatusesName = '已結案' THEN 91
                    WHEN O.Statuses = '計畫執行' AND O.StatusesName = '已終止' THEN 92

                    -- 其他專案保留原本 Status
                    ELSE ISNULL(O.Status, 0)
               END AS Status,
               O.UserAccount
        FROM (
            -- 科專專案
            SELECT ProjectID, Statuses, StatusesName, UserAccount, NULL AS Status
            FROM OFS_SCI_Project_Main
            WHERE IsExist = 1 AND IsWithdrawal <> 1

            -- CLB 專案
            UNION ALL
            SELECT ProjectID, Statuses, StatusesName, UserAccount, NULL AS Status
            FROM OFS_CLB_Project_Main
            WHERE IsExist = 1 AND IsWithdrawal <> 1

            -- 社團或其他專案
            UNION ALL
            SELECT ProjectID, NULL, NULL, UserAccount, Status
            FROM OFS_CUL_Project
            WHERE IsExists = 1 AND IsWithdrawal <> 1

            UNION ALL
            SELECT ProjectID, NULL, NULL, UserAccount, Status
            FROM OFS_EDC_Project
            WHERE IsExists = 1 AND IsWithdrawal <> 1

            UNION ALL
            SELECT ProjectID, NULL, NULL, UserAccount, Status
            FROM OFS_MUL_Project
            WHERE IsExists = 1 AND IsWithdrawal <> 1

            UNION ALL
            SELECT ProjectID, NULL, NULL, UserAccount, Status
            FROM OFS_LIT_Project
            WHERE IsExists = 1 AND IsWithdrawal <> 1

            UNION ALL
            SELECT ProjectID, NULL, NULL, UserAccount, Status
            FROM OFS_ACC_Project
            WHERE IsExists = 1 AND IsWithdrawal <> 1
        ) AS O
             WHERE O.[UserAccount] = @UserAccount

        ";

        db.Parameters.Add("@UserAccount", account);

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ApplyPlan
        {
            ProjectID = row.Field<string>("ProjectID"),
            Status = Convert.ToInt32(row["Status"])
        }).ToList();
    }

    // TODO 嘉良: 首頁的統計區塊, 計算各類申請案的 [核定補助經費], [實支金額], [已撥付], [核定件數], [總預算]
    public static List<ApplyPlan> queryApplyStat()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT T.[Year]
                  ,T.[TypeCode]
                  ,T.[BudgetFees]
                  ,ISNULL(SUM(O.[ApprovedAmount]), 0) AS [ApprovedAmount]
                  ,ISNULL(SUM(O.[SpendAmount]), 0) AS [SpendAmount]
                  ,ISNULL(SUM(O.[PaymentAmount]), 0) AS [PaymentAmount]
                  ,SUM(CASE WHEN O.Year IS NOT NULL THEN 1 ELSE 0 END) AS [Count]
              FROM (SELECT [Year]
                          ,[TypeCode]
                          ,ISNULL([BudgetFees], 0) AS [BudgetFees]
                      FROM [OFS_GrantType]) AS T
         LEFT JOIN (SELECT [Year]
                          ,'CUL' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_CUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION ALL
                    SELECT [Year]
                          ,'EDC' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_EDC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION ALL
                    SELECT [Year]
                          ,'MUL' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_MUL_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION ALL
                    SELECT [Year]
                          ,'LIT' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_LIT_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
                    UNION ALL
                    SELECT [Year]
                          ,'ACC' AS [Category]
                          ,ISNULL([ApprovedAmount], 0) AS [ApprovedAmount]
                          ,ISNULL([SpendAmount], 0) AS [SpendAmount]
                          ,ISNULL([PaymentAmount], 0) AS [PaymentAmount]
                      FROM OFS_ACC_Project
                     WHERE IsExists = 1 AND IsWithdrawal <> 1 AND Status IN (51,52,91)
					 UNION ALL
					 SELECT
							YEAR(PM.created_at) - 1911 AS [Year],
							'SCI'  AS [Category],
							ISNULL(PM.ApprovedSubsidy, 0) AS [ApprovedAmount],
							ISNULL(SP.SpendAmount, 0) AS [SpendAmount],
							ISNULL(SP.PaymentAmount, 0) AS [PaymentAmount]
						FROM OFS_SCI_Project_Main PM
						LEFT JOIN (
							SELECT ProjectID, SUM(ISNULL(TotalSpentAmount, 0)) as [SpendAmount] ,SUM(CurrentActualPaidAmount) AS PaymentAmount
							FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Payment]
							GROUP BY ProjectID
						) AS SP ON SP.ProjectID = PM.ProjectID
						WHERE PM.IsExist = 1
						  AND PM.Statuses = '計畫執行'
					 UNION ALL
						SELECT YEAR(PM.created_at) - 1911 AS [Year],
													'CLB'  AS [Category],
													ISNULL(ApprovedSubsidy,0) AS [ApprovedAmount],
													ISNULL(CP.SpendAmount,0) AS [SpendAmount],
													ISNULL(CP.PaymentAmount,0) AS [PaymentAmount]
													FROM OFS_CLB_Project_Main PM
						LEFT JOIN (
						 SELECT [ProjectID]
							  ,SUM([TotalSpentAmount])  as [SpendAmount]
							  ,SUM([CurrentActualPaidAmount])as [PaymentAmount]
						  FROM [OCA_OceanSubsidy].[dbo].[OFS_CLB_Payment]
						  Group by ProjectID
						  ) as CP on CP.ProjectID = PM.ProjectID
							WHERE PM.IsExist = 1
						  AND PM.Statuses = '計畫執行'

					 ) AS O ON (T.[TypeCode] = O.[Category] AND T.[Year] = O.[Year])
             WHERE 1 = 1

        ";

        db.CommandText += " GROUP BY T.[Year], T.[TypeCode], T.[BudgetFees]";

        return db.GetTable().Rows.Cast<DataRow>().Select(row => new ApplyPlan
        {
            Year = Convert.ToInt32(row["Year"]),
            Category = row.Field<string>("TypeCode"),
            ApprovedAmount = Convert.ToInt32(row["ApprovedAmount"]),
            SpendAmount = Convert.ToInt32(row["SpendAmount"]),
            PaymentAmount = Convert.ToInt32(row["PaymentAmount"]),
            Count = Convert.ToInt32(row["Count"]),
            BudgetFees = Convert.ToInt32(row["BudgetFees"])
        }).ToList();
    }
}

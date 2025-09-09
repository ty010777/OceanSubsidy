

-- V_OFS_ApplicationChecklistSearch

ALTER VIEW [dbo].[V_OFS_ApplicationChecklistSearch]
AS
WITH SubsidySummary AS (
    SELECT ProjectID, SUM(SubsidyAmount) AS TotalSubsidyAmount
    FROM dbo.OFS_SCI_PersonnelCost_TotalFee
    GROUP BY ProjectID
),
SCI_CTE AS (
    SELECT
        v.ProjectID,
        v.Statuses,
        v.StatusesName,
        v.ExpirationDate,
        v.SupervisoryUnit,
        v.SupervisoryPersonName,
        v.SupervisoryPersonAccount,
        v.UserAccount,
        v.UserOrg,
        v.UserName,
        v.isWithdrawal,
        v.isExist,
        m.SubsidyPlanType,
        m.ProjectNameTw,
        m.OrgName,
        m.Year,
        ISNULL(s.TotalSubsidyAmount, 0) AS TotalSubsidyAmount,
        'SCI' AS SourceSystem   -- 來源標記
    FROM dbo.OFS_SCI_Project_Main v
    LEFT JOIN dbo.OFS_SCI_Application_Main m
        ON v.ProjectID = m.ProjectID
    LEFT JOIN SubsidySummary s
        ON v.ProjectID = s.ProjectID
),
CLB_CTE AS (
    SELECT
        CLB_PM.ProjectID,
        CLB_PM.Statuses,
        CLB_PM.StatusesName,
        CLB_PM.ExpirationDate,
        CLB_PM.SupervisoryUnit,
        CLB_PM.SupervisoryPersonName,
        CLB_PM.SupervisoryPersonAccount,
        CLB_PM.UserAccount,
        CLB_PM.UserOrg,
        CLB_PM.UserName,
        CLB_PM.isWithdrawal,
        CLB_PM.isExist,
        CLB_AB.SubsidyPlanType,
        CLB_AB.ProjectNameTw,
        (CLB_AB.SchoolName + CLB_AB.ClubName) AS OrgName,
        CLB_AB.Year,
        ISNULL(CLB_AF.TotalFunds, 0) AS TotalSubsidyAmount,
        'CLB' AS SourceSystem   -- 來源標記
    FROM dbo.OFS_CLB_Project_Main CLB_PM
    LEFT JOIN dbo.OFS_CLB_Application_Basic CLB_AB
        ON CLB_PM.ProjectID = CLB_AB.ProjectID
    LEFT JOIN dbo.OFS_CLB_Application_Funds CLB_AF
        ON CLB_PM.ProjectID = CLB_AF.ProjectID
)
SELECT * FROM SCI_CTE
UNION ALL
SELECT * FROM CLB_CTE
UNION
    SELECT O.ProjectID,
           P.Descname AS Statuses,
           S.Descname AS StatusesName,
           O.EndTime AS ExpirationDate,
           U.UnitName AS SupervisoryUnit,
           R.Name AS SupervisoryPersonName,
           R.Account AS SupervisoryPersonAccount,
           O.UserAccount,
           O.UserOrg,
           O.UserName,
           O.isWithdrawal,
           O.IsExists AS isExist,
           O.SubsidyPlanType,
           O.ProjectName AS ProjectNameTw,
           O.OrgName,
           O.Year,
           O.ApplyAmount AS TotalSubsidyAmount,
           O.SourceSystem
      FROM (SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'CUL' AS SourceSystem
              FROM OFS_CUL_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'EDC' AS SourceSystem
              FROM OFS_EDC_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'MUL' AS SourceSystem
              FROM OFS_MUL_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'LIT' AS SourceSystem
              FROM OFS_LIT_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'ACC' AS SourceSystem
              FROM OFS_ACC_Project) AS O
 LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
 LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
      JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
      JOIN Sys_ZgsCode AS P ON (P.CodeGroup = 'ProjectProgressStatus' AND P.Code = O.ProgressStatus)
GO


-- V_OFS_ReviewChecklist_type1

ALTER VIEW [dbo].[V_OFS_ReviewChecklist_type1]
AS
WITH SCI_Review_Type1 AS (
    SELECT
        PM.ProjectID,
        AM.ProjectNameTw,
        AM.OrgName,
        PM.StatusesName,
        PM.ExpirationDate,
        PM.SupervisoryPersonAccount,
        PM.SupervisoryPersonName,
        PM.SupervisoryUnit,
        PM.created_at,
        SUM(PT.SubsidyAmount) AS Req_SubsidyAmount,
        YEAR(PM.created_at) - 1911 AS Year,
        'SCI' AS Category
    FROM dbo.OFS_SCI_Project_Main AS PM
    LEFT JOIN dbo.OFS_SCI_Application_Main AS AM
        ON AM.ProjectID = PM.ProjectID
    LEFT JOIN dbo.OFS_SCI_PersonnelCost_TotalFee AS PT
        ON PM.ProjectID = PT.ProjectID
    WHERE
        PM.isExist = 1
        AND PM.Statuses = '資格審查'
        AND PM.StatusesName <> '不通過'
        AND PM.isWithdrawal <> 1
    GROUP BY
        PM.ProjectID, AM.ProjectNameTw, AM.OrgName, PM.StatusesName,
        PM.ExpirationDate, PM.SupervisoryPersonAccount, PM.SupervisoryPersonName,
        PM.SupervisoryUnit, PM.created_at
),
CLB_Review_Type1 AS (
    SELECT
        CLB_PM.ProjectID,
        CLB_AB.ProjectNameTw,
        (CLB_AB.SchoolName + CLB_AB.ClubName) AS OrgName,
        CLB_PM.StatusesName,
        CLB_PM.ExpirationDate,
        CLB_PM.SupervisoryPersonAccount,
        CLB_PM.SupervisoryPersonName,
        CLB_PM.SupervisoryUnit,
        CLB_PM.created_at,
        ISNULL(CLB_AF.SubsidyFunds,0) AS Req_SubsidyAmount,
        CLB_AB.Year,
        'CLB' AS Category
    FROM dbo.OFS_CLB_Project_Main CLB_PM
    LEFT JOIN dbo.OFS_CLB_Application_Basic CLB_AB
        ON CLB_PM.ProjectID = CLB_AB.ProjectID
    LEFT JOIN dbo.OFS_CLB_Application_Funds CLB_AF
        ON CLB_AF.ProjectID = CLB_PM.ProjectID
    WHERE
        CLB_PM.isExist = 1
        AND CLB_PM.Statuses = '內容審查'
        AND CLB_PM.StatusesName <> '不通過'
        AND CLB_PM.isWithdrawal <> 1

)
SELECT * FROM SCI_Review_Type1
UNION ALL
SELECT * FROM CLB_Review_Type1
UNION
    SELECT O.ProjectID,
           O.ProjectName AS ProjectNameTw,
           O.OrgName,
           S.Descname AS StatusesName,
           O.EndTime AS ExpirationDate,
           R.Account AS SupervisoryPersonAccount,
           R.Name AS SupervisoryPersonName,
           U.UnitName AS SupervisoryUnit,
           O.CreateTime AS created_at,
           O.ApplyAmount AS Req_SubsidyAmount,
           O.Year,
           O.Category
      FROM (SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'CUL' AS Category
              FROM OFS_CUL_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'EDC' AS Category
              FROM OFS_EDC_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'MUL' AS Category
              FROM OFS_MUL_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'LIT' AS Category
              FROM OFS_LIT_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'ACC' AS Category
              FROM OFS_ACC_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13) AS O
 LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
 LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
      JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
GO


-- V_OFS_ReviewChecklist_type4

ALTER VIEW [dbo].[V_OFS_ReviewChecklist_type4]
AS
WITH SCI_CTE AS (
    SELECT
        PM.ProjectID,
        AM.Year,
        AM.ProjectNameTw,
        AM.OrgName,
        AM.Field,
        PM.SupervisoryPersonAccount,
        PM.SupervisoryPersonName,
        SUM(PT.SubsidyAmount) AS TotalSubsidyPrice,
        PM.StatusesName,
        PM.ApprovedSubsidy,
        PM.FinalReviewNotes,
        PM.FinalReviewOrder,
        SUM(ORR.TotalScore) AS TotalScore
    FROM dbo.OFS_SCI_Project_Main AS PM
    LEFT JOIN dbo.OFS_SCI_Application_Main AS AM
        ON PM.ProjectID = AM.ProjectID
    LEFT JOIN dbo.OFS_SCI_PersonnelCost_TotalFee AS PT
        ON PM.ProjectID = PT.ProjectID
    LEFT JOIN dbo.OFS_ReviewRecords AS ORR
        ON PM.ProjectID = ORR.ProjectID
    WHERE PM.Statuses = '決審核定'
      AND PM.isExist = 1
      AND PM.isWithdrawal <> 1
    GROUP BY
        PM.ProjectID, AM.Year, AM.ProjectNameTw, AM.OrgName, AM.Field,
        PM.SupervisoryPersonAccount, PM.SupervisoryPersonName,
        PM.ApprovedSubsidy, PM.FinalReviewNotes, PM.StatusesName, PM.FinalReviewOrder
),
CLB_CTE AS (
    SELECT
        CLB_PM.ProjectID,
        CLB_AB.Year,
        CLB_AB.ProjectNameTw,
        (CLB_AB.SchoolName + CLB_AB.ClubName) AS OrgName,
        '' AS Field,
        CLB_PM.SupervisoryPersonAccount,
        CLB_PM.SupervisoryPersonName,
        ISNULL(CLB_AF.SubsidyFunds, 0) AS TotalSubsidyPrice,
        CLB_PM.StatusesName,
        CLB_PM.ApprovedSubsidy,
        CLB_PM.FinalReviewNotes,
        CLB_PM.FinalReviewOrder,
        0 AS TotalScore
    FROM dbo.OFS_CLB_Project_Main CLB_PM
    LEFT JOIN dbo.OFS_CLB_Application_Basic CLB_AB
        ON CLB_PM.ProjectID = CLB_AB.ProjectID
    LEFT JOIN dbo.OFS_CLB_Application_Funds CLB_AF
        ON CLB_AF.ProjectID = CLB_PM.ProjectID
    WHERE CLB_PM.Statuses = '決審核定'
      AND CLB_PM.isExist = 1
      AND CLB_PM.isWithdrawal <> 1
)
SELECT * FROM SCI_CTE
UNION ALL
SELECT * FROM CLB_CTE
UNION
    SELECT O.ProjectID,
           O.Year,
           O.ProjectName AS ProjectNameTw,
           O.OrgName,
           O.Field,
           R.Account AS SupervisoryPersonAccount,
           R.Name AS SupervisoryPersonName,
           O.ApplyAmount AS TotalSubsidyPrice,
           S.Descname AS StatusesName,
           O.ApprovedAmount AS ApprovedSubsidy,
           O.FinalReviewNotes,
           O.FinalReviewOrder,
           C.TotalScore
      FROM (SELECT ProjectID, Year, ProjectName, OrgName, Field, Organizer, ApplyAmount, ApprovedAmount, FinalReviewNotes, Status, FinalReviewOrder
              FROM OFS_CUL_Project
             WHERE ProgressStatus = 4
            UNION
            SELECT ProjectID, Year, ProjectName, OrgName, '' AS Field, Organizer, ApplyAmount, ApprovedAmount, FinalReviewNotes, Status, FinalReviewOrder
              FROM OFS_EDC_Project
             WHERE ProgressStatus = 4
            UNION
            SELECT ProjectID, Year, ProjectName, OrgName, '' AS Field, Organizer, ApplyAmount, ApprovedAmount, FinalReviewNotes, Status, FinalReviewOrder
              FROM OFS_MUL_Project
             WHERE ProgressStatus = 4
            UNION
            SELECT ProjectID, Year, ProjectName, OrgName, '' AS Field, Organizer, ApplyAmount, ApprovedAmount, FinalReviewNotes, Status, FinalReviewOrder
              FROM OFS_LIT_Project
             WHERE ProgressStatus = 4
            UNION
            SELECT ProjectID, Year, ProjectName, OrgName, '' AS Field, Organizer, ApplyAmount, ApprovedAmount, FinalReviewNotes, Status, FinalReviewOrder
              FROM OFS_ACC_Project
             WHERE ProgressStatus = 4) AS O
 LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
 LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
      JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
 LEFT JOIN (SELECT ProjectID,
                   SUM(TotalScore) AS TotalScore
              FROM OFS_ReviewRecords
          GROUP BY ProjectID) AS C ON (C.ProjectID = O.ProjectID)
GO


-- V_OFS_InprogressList

ALTER VIEW [dbo].[V_OFS_InprogressList]
AS
WITH SCI_inprogressList AS (SELECT          AM.Year, 'SCI' AS Category, PM.ProjectID, AM.ProjectNameTw, AM.OrgName,
                                                                                            PM.SupervisoryUnit, PM.LastOperation, TQ.TaskNameEn, TQ.TaskName,
                                                                                            CONCAT_WS(' , ', AM.Target, AM.Summary, AM.Innovation) AS ProjectContent,
                                                                                            KW.KeyWords
                                                                FROM               dbo.OFS_SCI_Project_Main AS PM LEFT OUTER JOIN
                                                                                            dbo.OFS_SCI_Application_Main AS AM ON
                                                                                            PM.ProjectID = AM.ProjectID LEFT OUTER JOIN
                                                                                            dbo.OFS_TaskQueue AS TQ ON PM.ProjectID = TQ.ProjectID AND
                                                                                            TQ.PriorityLevel =
                                                                                                (SELECT          MIN(PriorityLevel) AS Expr1
                                                                                                  FROM               dbo.OFS_TaskQueue
                                                                                                  WHERE           (ProjectID = PM.ProjectID) AND (IsTodo = 1) AND (IsCompleted = 0))
                                                                                            LEFT OUTER JOIN
                                                                                                (SELECT          KeywordID AS ProjectID, STRING_AGG(KeyWordTw, ', ')
                                                                                                                              AS KeyWords
                                                                                                  FROM               dbo.OFS_SCI_Application_KeyWord AS AK
                                                                                                  GROUP BY    KeywordID) AS KW ON PM.ProjectID = KW.ProjectID
                                                                WHERE           (PM.Statuses = N'計畫執行'))
    SELECT          Year, Category, ProjectID, ProjectNameTw, OrgName, SupervisoryUnit, LastOperation, TaskNameEn, TaskName,
                                 ProjectContent, KeyWords
     FROM               SCI_inprogressList AS SCI_inprogressList_1
UNION
    SELECT O.Year,
           O.Category,
           O.ProjectID,
           O.ProjectName AS ProjectNameTw,
           O.OrgName,
           U.UnitName AS SupervisoryUnit,
           O.LastOperation,
           Q.TaskNameEn,
           Q.TaskName,
           O.ProjectContent,
           '' AS KeyWords
      FROM (SELECT Year, 'CUL' AS Category, ProjectID, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_CUL_Project
             WHERE ProgressStatus = 5
            UNION
            SELECT Year, 'EDC' AS Category, ProjectID, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified) AS ProjectContent
              FROM OFS_EDC_Project
             WHERE ProgressStatus = 5
            UNION
            SELECT Year, 'MUL' AS Category, ProjectID, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_MUL_Project
             WHERE ProgressStatus = 5
            UNION
            SELECT Year, 'LIT' AS Category, ProjectID, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_LIT_Project
             WHERE ProgressStatus = 5
            UNION
            SELECT Year, 'ACC' AS Category, ProjectID, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_ACC_Project
             WHERE ProgressStatus = 5) AS O
 LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
 LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
 LEFT JOIN OFS_TaskQueue AS Q ON (O.ProjectID = Q.ProjectID AND Q.PriorityLevel = (SELECT MIN(PriorityLevel) FROM OFS_TaskQueue WHERE ProjectID = O.ProjectID AND IsTodo = 1 AND IsCompleted = 0))
GO


-- V_OFS_ReviewChecklist_type5

ALTER VIEW [dbo].[V_OFS_ReviewChecklist_type5]
AS
WITH SCI_CTE AS (
    SELECT
        AM.Year,
        AM.ProjectID,
        'SCI' AS Category,
        AM.ProjectNameTw,
        AM.OrgName,
        PM.SupervisoryUnit
    FROM dbo.OFS_SCI_Project_Main AS PM
    LEFT JOIN dbo.OFS_SCI_Application_Main AS AM
        ON AM.ProjectID = PM.ProjectID
    WHERE
        PM.IsProjChanged = 1
        AND PM.isExist = 1
        AND PM.isWithdrawal <> 1
),
CLB_CTE AS (
    SELECT
        CLB_AB.Year,
        CLB_PM.ProjectID,
        'CLB' AS Category,
        CLB_AB.ProjectNameTw,
        (CLB_AB.SchoolName + CLB_AB.ClubName) AS OrgName,
        CLB_PM.SupervisoryUnit
    FROM dbo.OFS_CLB_Project_Main AS CLB_PM
    LEFT JOIN dbo.OFS_CLB_Application_Basic AS CLB_AB
        ON CLB_PM.ProjectID = CLB_AB.ProjectID
    WHERE
        CLB_PM.IsProjChanged = 1
        AND CLB_PM.isExist = 1
        AND CLB_PM.isWithdrawal <> 1
)
SELECT * FROM SCI_CTE
UNION ALL
SELECT * FROM CLB_CTE
UNION
    SELECT O.Year,
           O.ProjectID,
           O.Category,
           O.ProjectName AS ProjectNameTw,
           O.OrgName,
           U.UnitName AS SupervisoryUnit
      FROM (SELECT Year, ProjectID, 'CUL' AS Category, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_CUL_Project
             WHERE IsProjChanged = 1
            UNION
            SELECT Year, ProjectID, 'EDC' AS Category, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified) AS ProjectContent
              FROM OFS_EDC_Project
             WHERE IsProjChanged = 1
            UNION
            SELECT Year, ProjectID, 'MUL' AS Category, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_MUL_Project
             WHERE IsProjChanged = 1
            UNION
            SELECT Year, ProjectID, 'LIT' AS Category, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_LIT_Project
             WHERE IsProjChanged = 1
            UNION
            SELECT Year, ProjectID, 'ACC' AS Category, ProjectName, OrgName, Organizer, LastOperation, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
              FROM OFS_ACC_Project
             WHERE IsProjChanged = 1) AS O
 LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
 LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
GO





ALTER VIEW [dbo].[V_OFS_ApplicationChecklistSearch]
AS
WITH SubsidySummary AS (SELECT          ProjectID, SUM(SubsidyAmount) AS TotalSubsidyAmount
                                                             FROM               dbo.OFS_SCI_PersonnelCost_TotalFee
                                                             GROUP BY    ProjectID)
    SELECT          v.ProjectID, v.Statuses, v.StatusesName, v.ExpirationDate, v.SupervisoryUnit, v.SupervisoryPersonName,
                                 v.SupervisoryPersonAccount, v.UserAccount, v.UserOrg, v.UserName, v.isWithdrawal, v.isExist,
                                 m.SubsidyPlanType, m.ProjectNameTw, m.OrgName, m.Year, ISNULL(s.TotalSubsidyAmount, 0)
                                 AS TotalSubsidyAmount
     FROM               dbo.OFS_SCI_Project_Main AS v LEFT OUTER JOIN
                                 dbo.OFS_SCI_Application_Main AS m ON v.ProjectID = m.ProjectID LEFT OUTER JOIN
                                 SubsidySummary AS s ON v.ProjectID = s.ProjectID
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
           O.ApplyAmount AS TotalSubsidyAmount
      FROM (SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer
              FROM OFS_CUL_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer
              FROM OFS_EDC_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer
              FROM OFS_MUL_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer
              FROM OFS_LIT_Project
            UNION
            SELECT ProjectID, ProgressStatus, Status, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer
              FROM OFS_ACC_Project) AS O
 LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
 LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
      JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
      JOIN Sys_ZgsCode AS P ON (P.CodeGroup = 'ProjectProgressStatus' AND P.Code = O.ProgressStatus)
GO


ALTER VIEW [dbo].[V_OFS_ReviewChecklist_type1]
AS
WITH SCI_Review_Type1 AS (SELECT          PM.ProjectID, AM.ProjectNameTw, AM.OrgName, PM.StatusesName,
                                                                                             PM.ExpirationDate, PM.SupervisoryPersonAccount, PM.SupervisoryPersonName,
                                                                                             PM.SupervisoryUnit, PM.created_at, SUM(PT.SubsidyAmount) AS Req_SubsidyAmount,
                                                                                             YEAR(PM.created_at) - 1911 AS Year
                                                                 FROM              dbo.OFS_SCI_Project_Main AS PM LEFT OUTER JOIN
                                                                                             dbo.OFS_SCI_Application_Main AS AM ON
                                                                                             AM.ProjectID = PM.ProjectID LEFT OUTER JOIN
                                                                                             dbo.OFS_SCI_PersonnelCost_TotalFee AS PT ON PM.ProjectID = PT.ProjectID
                                                                 WHERE          (PM.isExist = 1) AND (PM.Statuses = '資格審查') AND (PM.StatusesName <> '不通過')
                                                                                             AND (PM.isWithdrawal <> 1)
                                                                 GROUP BY   PM.ProjectID, AM.ProjectNameTw, AM.OrgName, PM.StatusesName,
                                                                                             PM.ExpirationDate, PM.SupervisoryPersonAccount, PM.SupervisoryPersonName,
                                                                                             PM.SupervisoryUnit, PM.created_at)
    SELECT          ProjectID, ProjectNameTw, OrgName, StatusesName, ExpirationDate, SupervisoryPersonAccount,
                                 SupervisoryPersonName, SupervisoryUnit, created_at, Req_SubsidyAmount, Year, '科專' AS Category
     FROM               SCI_Review_Type1 AS SCI_Review_Type1_1
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
      FROM (SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, '文化' AS Category
              FROM OFS_CUL_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, '學校/民間' AS Category
              FROM OFS_EDC_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, '多元' AS Category
              FROM OFS_MUL_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, '素養' AS Category
              FROM OFS_LIT_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13
            UNION
            SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, '無障礙' AS Category
              FROM OFS_ACC_Project
             WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1 AND Status <> 13) AS O
 LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
 LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
      JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
GO



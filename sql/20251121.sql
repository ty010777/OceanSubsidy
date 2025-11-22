

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
        SUM(PT.TotalAmount) AS Req_TotalAmount,
        YEAR(PM.created_at) - 1911 AS Year,
        'SCI' AS Category,
        PM.QualReviewNotes
    FROM dbo.OFS_SCI_Project_Main AS PM
    LEFT JOIN dbo.OFS_SCI_Application_Main AS AM
        ON AM.ProjectID = PM.ProjectID
    LEFT JOIN dbo.OFS_SCI_PersonnelCost_TotalFee AS PT
        ON PM.ProjectID = PT.ProjectID
    WHERE
        PM.isExist = 1
        AND PM.Statuses = '資格審查'
        AND PM.isWithdrawal <> 1
    GROUP BY
        PM.ProjectID, AM.ProjectNameTw, AM.OrgName, PM.StatusesName,
        PM.ExpirationDate, PM.SupervisoryPersonAccount, PM.SupervisoryPersonName,
        PM.SupervisoryUnit, PM.created_at,
        PM.QualReviewNotes
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
        ISNULL(CLB_AB.ApplyAmount,0) AS Req_SubsidyAmount,
        ISNULL(CLB_AB.ApplyAmount+CLB_AB.SelfAmount+CLB_AB.OtherAmount ,0) AS Req_TotalAmount,
        CLB_AB.Year,
        'CLB' AS Category,
        CLB_PM.QualReviewNotes
    FROM dbo.OFS_CLB_Project_Main CLB_PM
    LEFT JOIN dbo.OFS_CLB_Application_Basic CLB_AB
        ON CLB_PM.ProjectID = CLB_AB.ProjectID
    WHERE
        CLB_PM.isExist = 1
        AND CLB_PM.Statuses = '內容審查'
        AND CLB_PM.isWithdrawal <> 1
)
SELECT * FROM SCI_Review_Type1
UNION ALL
SELECT * FROM CLB_Review_Type1
UNION
SELECT
    O.ProjectID,
    O.ProjectName AS ProjectNameTw,
    O.OrgName,
    S.Descname AS StatusesName,
    O.EndTime AS ExpirationDate,
    R.Account AS SupervisoryPersonAccount,
    R.Name AS SupervisoryPersonName,
    U.UnitName AS SupervisoryUnit,
    O.CreateTime AS created_at,
    O.ApplyAmount AS Req_SubsidyAmount,
    O.Req_TotalAmount,
    O.Year,
    O.Category,
    O.RejectReason AS QualReviewNotes
FROM (
    SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'CUL' AS Category, ISNULL(ApplyAmount,0)+ISNULL(SelfAmount,0)+ISNULL(OtherAmount,0) AS Req_TotalAmount, RejectReason
    FROM OFS_CUL_Project
    WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
    UNION
    SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'EDC' AS Category, ISNULL(ApplyAmount,0)+ISNULL(SelfAmount,0)+ISNULL(OtherGovAmount,0)+ISNULL(OtherUnitAmount,0) AS Req_TotalAmount, RejectReason
    FROM OFS_EDC_Project
    WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
    UNION
    SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'MUL' AS Category, ISNULL(ApplyAmount,0)+ISNULL(SelfAmount,0)+ISNULL(OtherAmount,0) AS Req_TotalAmount, RejectReason
    FROM OFS_MUL_Project
    WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
    UNION
    SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'LIT' AS Category, ISNULL(ApplyAmount,0)+ISNULL(SelfAmount,0)+ISNULL(OtherAmount,0) AS Req_TotalAmount, RejectReason
    FROM OFS_LIT_Project
    WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
    UNION
    SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'ACC' AS Category, ISNULL(ApplyAmount,0)+ISNULL(SelfAmount,0)+ISNULL(OtherAmount,0) AS Req_TotalAmount, RejectReason
    FROM OFS_ACC_Project
    WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
) AS O
LEFT JOIN Sys_User AS R ON R.UserID = O.Organizer
LEFT JOIN Sys_Unit AS U ON U.UnitID = R.UnitID
JOIN Sys_ZgsCode AS S ON S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status;
GO



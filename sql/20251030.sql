

-- 請款

CREATE TABLE [OFS_Payment] (
    [ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProjectID] [nvarchar](50) NULL,
    [Stage] [int] NULL,
    [ActDisbursementRatioPct] [decimal](5, 2) NULL,
    [CurrentRequestAmount] [decimal](18, 2) NULL,
    [CurrentActualPaidAmount] [decimal](18, 2) NULL,
    [TotalSpentAmount] [decimal](18, 2) NULL,
    [Status] [nvarchar](50) NULL,
    [ReviewerComment] [nvarchar](max) NULL,
    [ReviewUser] [nvarchar](100) NULL,
    [ReviewTime] [date] NULL,
    [CreateTime] [date] NULL,
    [UpdateTime] [date] NULL
)
GO


INSERT INTO OFS_Payment ([ProjectID], [Stage], [ActDisbursementRatioPct], [CurrentRequestAmount], [CurrentActualPaidAmount], [TotalSpentAmount], [Status], [ReviewerComment], [ReviewUser], [ReviewTime], [CreateTime], [UpdateTime])
SELECT [ProjectID], [Stage], [ActDisbursementRatioPct], [CurrentRequestAmount], [CurrentActualPaidAmount], [TotalSpentAmount], [Status], [ReviewerComment], [ReviewUser], [ReviewTime], [CreateTime], [UpdateTime]
  FROM OFS_SCI_Payment
 WHERE ProjectID LIKE '%CUL%' OR ProjectID LIKE '%EDC%' OR ProjectID LIKE '%MUL%' OR ProjectID LIKE '%LIT%' OR ProjectID LIKE '%ACC%'
GO


DELETE
  FROM OFS_SCI_Payment
 WHERE ProjectID LIKE '%CUL%' OR ProjectID LIKE '%EDC%' OR ProjectID LIKE '%MUL%' OR ProjectID LIKE '%LIT%' OR ProjectID LIKE '%ACC%'
GO


-- 報告

CREATE TABLE [OFS_StageExam] (
    [id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProjectID] [nvarchar](50) NULL,
    [Stage] [int] NULL,
    [ExamVersion] [int] NULL,
    [ReviewMethod] [nvarchar](50) NULL,
    [Status] [nvarchar](50) NULL,
    [Reviewer] [nvarchar](50) NULL,
    [Account] [nvarchar](200) NULL,
    [create_at] [date] NULL,
    [update_at] [date] NULL
)
GO


INSERT INTO OFS_StageExam ([ProjectID], [Stage], [ExamVersion], [ReviewMethod], [Status], [Reviewer], [Account], [create_at], [update_at])
SELECT [ProjectID], [Stage], [ExamVersion], [ReviewMethod], [Status], [Reviewer], [Account], [create_at], [update_at]
  FROM [OFS_SCI_StageExam]
 WHERE ProjectID LIKE '%CUL%' OR ProjectID LIKE '%EDC%' OR ProjectID LIKE '%MUL%' OR ProjectID LIKE '%LIT%' OR ProjectID LIKE '%ACC%'
GO


DELETE
  FROM [OFS_SCI_StageExam]
 WHERE ProjectID LIKE '%CUL%' OR ProjectID LIKE '%EDC%' OR ProjectID LIKE '%MUL%' OR ProjectID LIKE '%LIT%' OR ProjectID LIKE '%ACC%'
GO


-- 待審核

ALTER VIEW [V_OFS_ReviewChecklist_type6]
AS
WITH SCI_ReviewTodo_Raw AS (
    SELECT ProjectID,
           Stage,
           CASE WHEN A.Stage = 1 THEN '期中檢核' WHEN A.Stage = 2 THEN '期末檢核' ELSE '' END AS ReviewTodo
      FROM dbo.OFS_SCI_StageExam AS A
     WHERE (Status = '審核中')
    UNION ALL
    SELECT ProjectID,
           Stage,
           CASE WHEN B.Stage = 1 THEN '第一期請款審核' WHEN B.Stage = 2 THEN '第二期請款審核' ELSE '' END AS ReviewTodo
      FROM dbo.OFS_SCI_Payment AS B
     WHERE (Status = '審核中')
), SCI_ReviewTodo AS (
    SELECT AM.Year,
           SR.ProjectID,
           'SCI' AS Category,
           SR.Stage,
           SR.ReviewTodo,
           PM.SupervisoryPersonAccount,
           PM.SupervisoryUnit,
           AM.OrgName,
           AM.ProjectNameTw
      FROM SCI_ReviewTodo_Raw AS SR
LEFT OUTER JOIN dbo.OFS_SCI_Project_Main AS PM ON PM.ProjectID = SR.ProjectID
LEFT OUTER JOIN dbo.OFS_SCI_Application_Main AS AM ON PM.ProjectID = AM.ProjectID
)
SELECT Year,
       ProjectID,
       Category,
       Stage,
       ReviewTodo,
       SupervisoryPersonAccount,
       SupervisoryUnit,
       OrgName,
       ProjectNameTw
  FROM SCI_ReviewTodo AS SCI_ReviewTodo_1
UNION
SELECT B.[Year],
       B.[ProjectID],
       B.[Category],
       A.[Stage],
       C.[ReviewTodo],
       B.[SupervisoryPersonAccount],
       B.[SupervisoryUnit],
       B.[OrgName],
       B.[ProjectNameTw]
  FROM (SELECT [ProjectID], 1 AS [Type], [Stage] FROM [OFS_Payment] WHERE Status = '審核中'
        UNION ALL
        SELECT [ProjectID], 2 AS [Type], [Stage] FROM [OFS_StageExam] WHERE Status = '審核中') AS A
  JOIN (SELECT O.Year,
               O.ProjectID,
               O.Category,
               R.Account AS SupervisoryPersonAccount,
               U.UnitName AS SupervisoryUnit,
               O.OrgName,
               O.ProjectName AS ProjectNameTw
          FROM (SELECT Year, ProjectID, 'CUL' AS Category, OrgName, ProjectName, Organizer FROM OFS_CUL_Project
                UNION
                SELECT Year, ProjectID, 'EDC' AS Category, OrgName, ProjectName, Organizer FROM OFS_EDC_Project
                UNION
                SELECT Year, ProjectID, 'MUL' AS Category, OrgName, ProjectName, Organizer FROM OFS_MUL_Project
                UNION
                SELECT Year, ProjectID, 'LIT' AS Category, OrgName, ProjectName, Organizer FROM OFS_LIT_Project
                UNION
                SELECT Year, ProjectID, 'ACC' AS Category, OrgName, ProjectName, Organizer FROM OFS_ACC_Project) AS O
      LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
      LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)) AS B ON (B.ProjectID = A.ProjectID)
  JOIN (SELECT [TypeCode],
               1 AS [Type],
               [PhaseOrder],
               [PhaseName] AS [ReviewTodo]
          FROM [OFS_PaymentPhaseSetting]
        UNION SELECT 'CUL', 2, 1, '期中報告'
        UNION SELECT 'CUL', 2, 2, '期末報告'
        UNION SELECT 'EDC', 2, 1, '成果報告'
        UNION SELECT 'MUL', 2, 1, '成果報告'
        UNION SELECT 'LIT', 2, 1, '成果報告'
        UNION SELECT 'ACC', 2, 1, '期中報告'
        UNION SELECT 'ACC', 2, 2, '成果報告') AS C ON (C.[TypeCode] = B.[Category] AND C.[Type] = A.[Type] AND C.[PhaseOrder] = A.[Stage])
GO



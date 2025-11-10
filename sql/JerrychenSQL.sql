
----------
-- 20250908 目前給正文新版資料庫
-- 20250908 新增一個查核紀錄table

USE [OCA_OceanSubsidy]
GO

/****** Object:  Table [dbo].[OFS_AuditRecords]    Script Date: 2025/9/8 下午 10:42:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OFS_AuditRecords](
    [idx] [int] IDENTITY(1,1) NOT NULL,
    [ProjectID] [nvarchar](50) NULL,
    [ReviewerName] [nvarchar](100) NULL,
    [CheckDate] [date] NULL,
    [Risk] [nvarchar](20) NULL,
    [ReviewerComment] [nvarchar](1000) NULL,
    [ExecutorComment] [nvarchar](1000) NULL,
    [create_at] [datetime] NULL,
    [update_at] [datetime] NULL
    ) ON [PRIMARY]
    GO



--------------------------------------
    USE [OCA_OceanSubsidy]
    GO

/****** Object:  Table [dbo].[OFS_CLB_StageExam]    Script Date: 2025/9/11 上午 10:45:08 ******/
    SET ANSI_NULLS ON
    GO

    SET QUOTED_IDENTIFIER ON
    GO

CREATE TABLE [dbo].[OFS_CLB_StageExam](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [ProjectID] [nvarchar](50) NULL,
    [Status] [nvarchar](50) NULL,
    [Reviewer] [nvarchar](50) NULL,
    [Account] [nvarchar](200) NULL,
    [create_at] [date] NULL,
    [update_at] [date] NULL
    ) ON [PRIMARY]
    GO

-----------------------------
CREATE TABLE [dbo].[OFS_CLB_Payment](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [ProjectID] [nvarchar](50) NULL,
    [Stage] [int] NULL,
    [CurrentRequestAmount] [decimal](18, 2) NULL,
    [TotalSpentAmount] [decimal](18, 2) NULL,
    [CurrentActualPaidAmount] [decimal](18, 2) NULL,
    [Status] [nvarchar](50) NULL,
    [ReviewerComment] [nvarchar](max) NULL,
    [ReviewUser] [nvarchar](100) NULL,
    [ReviewTime] [date] NULL,
    [CreateTime] [date] NULL,
    [UpdateTime] [date] NULL
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]





----------
--     20250916
CREATE VIEW [dbo].[V_Excel_Worksch]
AS
    WITH SciWorksch AS
    (
    SELECT
    ProjectID,
    STRING_AGG(
    CONCAT(
    SUBSTRING(WorkItem_id, CHARINDEX('_', WorkItem_id) + 1, 1), -- A/B/C
    '. ',
    WorkName
),
    CHAR(13)+CHAR(10)  -- 換行符號 (CR+LF)
) WITHIN GROUP (
    ORDER BY SUBSTRING(WorkItem_id, CHARINDEX('_', WorkItem_id) + 1, 1)
) AS WorkNames
    FROM OFS_SCI_WorkSch_Main
    WHERE ISNUMERIC(RIGHT(WorkItem_id, 1)) = 0
    GROUP BY ProjectID
)
SELECT *
FROM SciWorksch;
----------------------------------------------------------------

-- view 修改 V_OFS_ReviewChecklist_type1 新增 Req_TotalAmount,QualReviewNotes



USE [OCA_OceanSubsidy]
GO

/****** Object:  View [dbo].[V_OFS_ReviewChecklist_type1]    Script Date: 2025/10/13 下午 02:05:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
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
        ISNULL(CLB_AF.SubsidyFunds,0) AS Req_SubsidyAmount,
        ISNULL(CLB_AF.TotalFunds ,0) AS Req_TotalAmount,
        CLB_AB.Year,
        'CLB' AS Category,
        CLB_PM.QualReviewNotes
    FROM dbo.OFS_CLB_Project_Main CLB_PM
    LEFT JOIN dbo.OFS_CLB_Application_Basic CLB_AB
        ON CLB_PM.ProjectID = CLB_AB.ProjectID
    LEFT JOIN dbo.OFS_CLB_Application_Funds CLB_AF
        ON CLB_AF.ProjectID = CLB_PM.ProjectID
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
    -- ★★★ 外包請補上 Req_TotalAmount (目前只有 Req_SubsidyAmount) ★★★
    NULL AS Req_TotalAmount,  -- TODO: 請補正確的計算方式 「請求總金額 (海委補助 +  配合款...等等) 」 
    O.Year,
    O.Category,
    NULL AS QualReviewNotes  -- ★★★ 資格/內容審核 不通過 或退回補正時的備註欄位 請補上
FROM (
         SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'CUL' AS Category
         FROM OFS_CUL_Project
         WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
         UNION
         SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'EDC' AS Category
         FROM OFS_EDC_Project
         WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
         UNION
         SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'MUL' AS Category
         FROM OFS_MUL_Project
         WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
         UNION
         SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'LIT' AS Category
         FROM OFS_LIT_Project
         WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
         UNION
         SELECT ProjectID, ProjectName, OrgName, Status, EndTime, Organizer, CreateTime, ApplyAmount, Year, 'ACC' AS Category
         FROM OFS_ACC_Project
         WHERE IsExists = 1 AND isWithdrawal <> 1 AND ProgressStatus = 1
     ) AS O
         LEFT JOIN Sys_User AS R ON R.UserID = O.Organizer
         LEFT JOIN Sys_Unit AS U ON U.UnitID = R.UnitID
         JOIN Sys_ZgsCode AS S ON S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status;

GO

--------------------------------------------------


-- OFS_CLB_Project_Main 新增欄位
ALTER TABLE [OCA_OceanSubsidy].[dbo].[OFS_CLB_Project_Main]
    ADD [QualReviewNotes] NVARCHAR(500) NULL;

-- OFS_SCI_Project_Main 新增欄位
ALTER TABLE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
    ADD [QualReviewNotes] NVARCHAR(500) NULL;

---------------
-- 20251008
-- 先刪除舊資料
DELETE FROM Sys_ZgsCode
WHERE CodeGroup = 'SCI_TRL';

-- 再插入新的 TRL 清單
INSERT INTO Sys_ZgsCode
(CodeGroup, Code, Descname, OrderNo, IsValid, MaxPriceLimit, ValidBeginDate, ValidEndDate, ParentCode)
VALUES
    ('SCI_TRL', '1', N'TRL1：界定機會與挑戰', 1, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '2', N'TRL2：構思因應方案', 2, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '3', N'TRL3：進行概念性驗證實驗', 3, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '4', N'TRL4：進行關鍵要素之現場試驗', 4, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '5', N'TRL5：驗證商品化之可行性', 5, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '6', N'TRL6：完成實用性原型開發', 6, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '7', N'TRL7：市場可及性', 7, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '8', N'TRL8：建立商用', 8, 1, NULL, NULL, NULL, ''),
    ('SCI_TRL', '9', N'TRL9：達成持續生產', 9, 1, NULL, NULL, NULL, '');

--------------------
20251010
ALTER TABLE [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_Main]
    ADD
    [StartYear] INT NULL,
    [EndYear] INT NULL;

---------------------
--銀行代碼 10/13 
INSERT INTO Sys_ZgsCode
    (CodeGroup, Code, Descname, OrderNo, IsValid, MaxPriceLimit, ValidBeginDate, ValidEndDate, ParentCode)
VALUES
    ('BankCode', '004', N'臺灣銀行', 1, 1, NULL, NULL, NULL, ''),
    ('BankCode', '005', N'土地銀行', 2, 1, NULL, NULL, NULL, ''),
    ('BankCode', '006', N'合作金庫', 3, 1, NULL, NULL, NULL, ''),
    ('BankCode', '007', N'第一銀行', 4, 1, NULL, NULL, NULL, ''),
    ('BankCode', '008', N'華南商業銀行', 5, 1, NULL, NULL, NULL, ''),
    ('BankCode', '009', N'彰化銀行', 6, 1, NULL, NULL, NULL, ''),
    ('BankCode', '011', N'上海商業儲蓄銀行', 7, 1, NULL, NULL, NULL, ''),
    ('BankCode', '012', N'台北富邦銀行', 8, 1, NULL, NULL, NULL, ''),
    ('BankCode', '013', N'國泰世華銀行', 9, 1, NULL, NULL, NULL, ''),
    ('BankCode', '016', N'高雄銀行', 10, 1, NULL, NULL, NULL, ''),
    ('BankCode', '017', N'兆豐國際商業銀行', 11, 1, NULL, NULL, NULL, ''),
    ('BankCode', '021', N'花旗銀行', 12, 1, NULL, NULL, NULL, ''),
    ('BankCode', '048', N'O-Bank (王道銀行)', 13, 1, NULL, NULL, NULL, ''),
    ('BankCode', '052', N'渣打國際商業銀行', 14, 1, NULL, NULL, NULL, ''),
    ('BankCode', '053', N'台中商業銀行', 15, 1, NULL, NULL, NULL, ''),
    ('BankCode', '054', N'京城銀行', 16, 1, NULL, NULL, NULL, ''),
    ('BankCode', '081', N'滙豐銀行', 17, 1, NULL, NULL, NULL, ''),
    ('BankCode', '103', N'新光銀行', 18, 1, NULL, NULL, NULL, ''),
    ('BankCode', '108', N'陽信銀行', 19, 1, NULL, NULL, NULL, ''),
    ('BankCode', '118', N'板信銀行', 20, 1, NULL, NULL, NULL, ''),
    ('BankCode', '147', N'三信商業銀行', 21, 1, NULL, NULL, NULL, ''),
    ('BankCode', '803', N'聯邦銀行', 22, 1, NULL, NULL, NULL, ''),
    ('BankCode', '805', N'遠東國際商業銀行', 23, 1, NULL, NULL, NULL, ''),
    ('BankCode', '806', N'元大銀行', 24, 1, NULL, NULL, NULL, ''),
    ('BankCode', '807', N'永豐銀行', 25, 1, NULL, NULL, NULL, ''),
    ('BankCode', '808', N'玉山銀行', 26, 1, NULL, NULL, NULL, ''),
    ('BankCode', '809', N'凱基銀行', 27, 1, NULL, NULL, NULL, ''),
    ('BankCode', '810', N'星展銀行 (DBS)', 28, 1, NULL, NULL, NULL, ''),
    ('BankCode', '812', N'台新銀行', 29, 1, NULL, NULL, NULL, ''),
    ('BankCode', '816', N'安泰銀行', 30, 1, NULL, NULL, NULL, ''),
    ('BankCode', '822', N'中國信託銀行', 31, 1, NULL, NULL, NULL, ''),
    ('BankCode', '823', N'將來銀行 (NEXT)', 32, 1, NULL, NULL, NULL, ''),
    ('BankCode', '824', N'連線銀行 (LINE Bank)', 33, 1, NULL, NULL, NULL, ''),
    ('BankCode', '826', N'樂天銀行', 34, 1, NULL, NULL, NULL, ''),
    ('BankCode', '381', N'交通銀行 (中國)', 35, 1, NULL, NULL, NULL, ''),
    ('BankCode', '380', N'中國銀行', 36, 1, NULL, NULL, NULL, ''),
    ('BankCode', '382', N'中國建設銀行', 37, 1, NULL, NULL, NULL, '');
---------------------------------
--10/13 新增 銀行、地址欄位
ALTER TABLE [OCA_OceanSubsidy].[dbo].[OFS_SCI_StageExam_ReviewerList]
    ADD
    [BankCode] VARCHAR(10),
    [BankAccount] VARCHAR(30),
    [RegistrationAddress] NVARCHAR(300);
--------------------------------
-- 10/14  新增 V_OFS_InprogressList StatusName 欄位
USE [OCA_OceanSubsidy]
GO

/****** Object:  View [dbo].[V_OFS_InprogressList]    Script Date: 2025/10/14 下午 01:39:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






-- V_OFS_InprogressList

ALTER VIEW [dbo].[V_OFS_InprogressList]
AS
WITH O AS (
    -- 其他類別先組好單位
    SELECT 
        P.Year,
        P.Category,
        P.ProjectID,
		P.StatusName,
        P.ProjectName AS ProjectNameTw,
        P.OrgName,
        U.UnitName AS SupervisoryUnit,
        P.LastOperation,
        P.ProjectContent,
        '' AS KeyWords
    FROM (
        -- CUL
        SELECT 
            Year,
            'CUL' AS Category,
            ProjectID,
			S.Descname AS StatusName,
            ProjectName,
            OrgName,
            Organizer,
            LastOperation,
            CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
        FROM OFS_CUL_Project
		LEFT JOIN Sys_ZgsCode AS S
		ON OFS_CUL_Project.Status = S.Code and S.CodeGroup = 'ProjectStatus'

        WHERE ProgressStatus = 5

        UNION ALL

        -- EDC
        SELECT 
            Year,
            'EDC' AS Category,
            ProjectID,
			S.Descname AS StatusName,
            ProjectName,
            OrgName,
            Organizer,
            LastOperation,
            CONCAT_WS(' , ', Target, Summary, Quantified) AS ProjectContent
        FROM OFS_EDC_Project
		LEFT JOIN Sys_ZgsCode AS S
		ON OFS_EDC_Project.Status = S.Code and S.CodeGroup = 'ProjectStatus'

        WHERE ProgressStatus = 5

        UNION ALL

        -- MUL
        SELECT 
            Year,
            'MUL' AS Category,
            ProjectID,
			S.Descname AS StatusName,
            ProjectName,
            OrgName,
            Organizer,
            LastOperation,
            CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
        FROM OFS_MUL_Project
				LEFT JOIN Sys_ZgsCode AS S
		ON OFS_MUL_Project.Status = S.Code and S.CodeGroup = 'ProjectStatus'

        WHERE ProgressStatus = 5

        UNION ALL

        -- LIT
        SELECT 
            Year,
            'LIT' AS Category,
            ProjectID,
			S.Descname as StatusesName,
            ProjectName,
            OrgName,
            Organizer,
            LastOperation,
            CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
        FROM OFS_LIT_Project
		LEFT JOIN Sys_ZgsCode AS S
		ON OFS_LIT_Project.Status = S.Code and S.CodeGroup = 'ProjectStatus'

        WHERE ProgressStatus = 5

        UNION ALL

        -- ACC
        SELECT 
            Year,
            'ACC' AS Category,
            ProjectID,
			S.Descname AS StatusesName,
            ProjectName,
            OrgName,
            Organizer,
            LastOperation,
            CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
        FROM OFS_ACC_Project
		LEFT JOIN Sys_ZgsCode AS S
		ON OFS_ACC_Project.Status = S.Code and S.CodeGroup = 'ProjectStatus'
		WHERE ProgressStatus = 5
	
		   
    ) AS P
    LEFT JOIN Sys_User AS R 
           ON R.UserID = P.Organizer
    LEFT JOIN Sys_Unit AS U 
           ON U.UnitID = R.UnitID



    UNION ALL

    -- SCI 專案
    SELECT 
        AM.Year,
        'SCI' AS Category,
        PM.ProjectID,
		PM.StatusesName,
        AM.ProjectNameTw AS ProjectNameTw,
        AM.OrgName,
        PM.SupervisoryUnit,
        PM.LastOperation,
        CONCAT_WS(' , ', AM.Target, AM.Summary, AM.Innovation) AS ProjectContent,
        KW.KeyWords
    FROM OFS_SCI_Project_Main AS PM
    LEFT JOIN OFS_SCI_Application_Main AS AM 
           ON PM.ProjectID = AM.ProjectID
    LEFT JOIN (
        SELECT 
            KeywordID AS ProjectID,
            STRING_AGG(KeyWordTw, ', ') AS KeyWords
        FROM OFS_SCI_Application_KeyWord
        GROUP BY KeywordID
    ) AS KW 
           ON PM.ProjectID = KW.ProjectID
    WHERE PM.Statuses = N'計畫執行' and PM.isExist = 1 and isWithdrawal = 0

    UNION ALL

    -- CLB 社團專案（直接取得 SupervisoryUnit）
    SELECT
        CLB_AB.Year,
        'CLB' AS Category,
        CLB_PM.ProjectID,
		CLB_PM.StatusesName,
        CLB_AB.ProjectNameTW AS ProjectNameTw,
        CLB_AB.SchoolName + CLB_AB.ClubName AS OrgName,
        CLB_PM.SupervisoryUnit,
        CLB_PM.LastOperation,
        CONCAT_WS(' , ', CLB_AP.Purpose, CLB_AP.PlanContent, CLB_AP.PreBenefits) AS ProjectContent,
        '' AS KeyWords
    FROM OFS_CLB_Project_Main CLB_PM
    LEFT JOIN OFS_CLB_Application_Basic CLB_AB 
           ON CLB_PM.ProjectID = CLB_AB.ProjectID
    LEFT JOIN OFS_CLB_Application_Plan CLB_AP 
           ON CLB_PM.ProjectID = CLB_AP.ProjectID
	where CLB_PM.Statuses = N'計畫執行'  and CLB_PM.isExist = 1 and isWithdrawal = 0
)
SELECT
    Ｏ.Year,
    O.Category,
    O.ProjectID,
    O.StatusName,
    O.ProjectNameTw,
    O.OrgName,
    O.SupervisoryUnit,
    O.LastOperation,
    Q.TaskNameEn,
    Q.TaskName,
    O.ProjectContent,
    O.KeyWords
FROM O
    OUTER APPLY (
    SELECT TOP 1 TaskNameEn, TaskName
    FROM OFS_TaskQueue T
    WHERE T.ProjectID = O.ProjectID
      AND T.IsTodo = 1
      AND T.IsCompleted = 0
    ORDER BY T.PriorityLevel ASC
) AS Q;


GO
------------------------------------以上在10/30上一版到正式站上 ----------------------------------------
--------------------------
ALTER TABLE dbo.OFS_SCI_Application_Main
ALTER COLUMN Serial INT;
------------------------------------------------------

ALTER VIEW [dbo].[V_OFS_ReviewChecklist_type6]
AS
WITH ReviewTodo_Raw AS (
    ---------------------------------------
    -- SCI StageExam
    ---------------------------------------
    SELECT
        AM.Year,
        A.ProjectID,
        'SCI' AS Category,
        A.Stage,
        CASE
            WHEN A.Stage = 1 THEN '期中報告'
            WHEN A.Stage = 2 THEN '期末報告'
            ELSE ''
        END AS ReviewTodo,
        P.SupervisoryPersonAccount,
        P.SupervisoryUnit,
        AM.OrgName,
        AM.ProjectNameTw
    FROM dbo.OFS_SCI_StageExam AS A
    LEFT JOIN dbo.OFS_SCI_Project_Main AS P
        ON P.ProjectID = A.ProjectID
    LEFT JOIN dbo.OFS_SCI_Application_Main AS AM
        ON AM.ProjectID = A.ProjectID
    WHERE A.Status = '審核中' and P.StatusesName != '已終止'
    ---------------------------------------
    UNION ALL
    ---------------------------------------
    -- SCI Payment
    SELECT
        AM.Year,
        B.ProjectID,
        'SCI' AS Category,
        B.Stage,
        CASE
            WHEN B.Stage = 1 THEN '第一期請款審核'
            WHEN B.Stage = 2 THEN '第二期請款審核'
            ELSE ''
        END AS ReviewTodo,
        P.SupervisoryPersonAccount,
        P.SupervisoryUnit,
        AM.OrgName,
        AM.ProjectNameTw
    FROM dbo.OFS_SCI_Payment AS B
    LEFT JOIN dbo.OFS_SCI_Project_Main AS P
        ON P.ProjectID = B.ProjectID
    LEFT JOIN dbo.OFS_SCI_Application_Main AS AM
        ON AM.ProjectID = B.ProjectID
    WHERE B.Status = '審核中' and P.StatusesName != '已終止'
    ---------------------------------------
    UNION ALL
    ---------------------------------------
    -- CLB StageExam
    SELECT
        A_CLB.Year,
        S.ProjectID,
        'CLB' AS Category,
        1 AS Stage,
        '成果報告' AS ReviewTodo,
        P.SupervisoryPersonAccount,
        P.SupervisoryUnit,
        P.UserOrg AS OrgName,
        P.UserOrg AS ProjectNameTw
    FROM dbo.OFS_CLB_StageExam AS S
    INNER JOIN dbo.OFS_CLB_Project_Main AS P
        ON P.ProjectID = S.ProjectID
    LEFT JOIN dbo.OFS_CLB_Application_Basic AS A_CLB
        ON A_CLB.ProjectID = S.ProjectID
    WHERE S.Status = '審核中'  and P.StatusesName != '已終止'
    ---------------------------------------
    UNION ALL
    ---------------------------------------
    -- CLB Payment
    SELECT
        A_CLB.Year,
        C.ProjectID,
        'CLB' AS Category,
        Stage,
        '請款核銷' AS ReviewTodo,
        P.SupervisoryPersonAccount,
        P.SupervisoryUnit,
        P.UserOrg AS OrgName,
        P.UserOrg AS ProjectNameTw
    FROM dbo.OFS_CLB_Payment AS C
    INNER JOIN dbo.OFS_CLB_Project_Main AS P
        ON P.ProjectID = C.ProjectID
    LEFT JOIN dbo.OFS_CLB_Application_Basic AS A_CLB
        ON A_CLB.ProjectID = C.ProjectID
    WHERE C.Status = '審核中' and P.StatusesName != '已終止'
)
SELECT * FROM ReviewTodo_Raw
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
    UNION
    SELECT [ProjectID], 2 AS [Type], [Stage] FROM [OFS_StageExam] WHERE Status = '審核中') AS A
    JOIN (SELECT O.Year,
    O.ProjectID,
    O.Category,
    R.Account AS SupervisoryPersonAccount,
    U.UnitName AS SupervisoryUnit,
    O.OrgName,
    O.ProjectName AS ProjectNameTw
    FROM (SELECT Year, ProjectID, 'CUL' AS Category, OrgName, ProjectName, Organizer,Status FROM OFS_CUL_Project
    UNION
    SELECT Year, ProjectID, 'EDC' AS Category, OrgName, ProjectName, Organizer,Status FROM OFS_EDC_Project
    UNION
    SELECT Year, ProjectID, 'MUL' AS Category, OrgName, ProjectName, Organizer,Status FROM OFS_MUL_Project
    UNION
    SELECT Year, ProjectID, 'LIT' AS Category, OrgName, ProjectName, Organizer,Status FROM OFS_LIT_Project
    UNION
    SELECT Year, ProjectID, 'ACC' AS Category, OrgName, ProjectName, Organizer,Status  FROM OFS_ACC_Project) AS O
    LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
    LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
    where O.Status not in (99)
    ) AS B ON (B.ProjectID = A.ProjectID)
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
------------------------------------------
    20251107
ALTER TABLE OFS_SCI_StageExam_ReviewerList
    ADD BankBookPath NVARCHAR(1000) NULL;

ALTER TABLE OFS_CLB_Project_Main
    ADD ApplyTime DATETIME NULL;

ALTER TABLE OFS_SCI_Project_Main
    ADD ApplyTime DATETIME NULL;

-------------------------------------------
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
		CONCAT_WS(' , ', m.Target, m.Summary, m.Innovation) AS ProjectContent,
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
		CONCAT_WS(' , ', CLB_AP.Purpose, CLB_AP.PlanContent, CLB_AP.PreBenefits) AS ProjectContent,
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
	 LEFT JOIN OFS_CLB_Application_Plan CLB_AP 
           ON CLB_PM.ProjectID = CLB_AP.ProjectID
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
       O.ProjectContent,
       O.SubsidyPlanType,
       O.ProjectName AS ProjectNameTw,
       O.OrgName,
       O.Year,
       O.ApplyAmount AS TotalSubsidyAmount,
       O.SourceSystem
FROM (SELECT ProjectID, ProgressStatus, Status, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent,EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'CUL' AS SourceSystem
      FROM OFS_CUL_Project
      UNION
      SELECT ProjectID, ProgressStatus, Status,CONCAT_WS(' , ', Target, Summary, Quantified) AS ProjectContent, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'EDC' AS SourceSystem
      FROM OFS_EDC_Project
      UNION
      SELECT ProjectID, ProgressStatus, Status,CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'MUL' AS SourceSystem
      FROM OFS_MUL_Project
      UNION
      SELECT ProjectID, ProgressStatus, Status,CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'LIT' AS SourceSystem
      FROM OFS_LIT_Project
      UNION
      SELECT ProjectID, ProgressStatus, Status,CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent, EndTime, UserAccount, UserOrg, UserName, isWithdrawal, IsExists, SubsidyPlanType, ProjectName, OrgName, Year, ApplyAmount, Organizer, 'ACC' AS SourceSystem
      FROM OFS_ACC_Project) AS O
         LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
         LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
         JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
         JOIN Sys_ZgsCode AS P ON (P.CodeGroup = 'ProjectProgressStatus' AND P.Code = O.ProgressStatus)
    GO

----------------------------

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
        PM.IsProjChanged = 2
        AND PM.isExist = 1
        AND PM.isWithdrawal <> 1
		AND PM.StatusesName <> '已終止'
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
        CLB_PM.IsProjChanged =  2 
        AND CLB_PM.isExist = 1
        AND CLB_PM.isWithdrawal <> 1
		AND CLB_PM.StatusesName <> '已終止'
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
FROM (SELECT Year, ProjectID, 'CUL' AS Category, ProjectName, OrgName, Organizer, LastOperation,Status, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
      FROM OFS_CUL_Project
      WHERE IsProjChanged =  2
      UNION
      SELECT Year, ProjectID, 'EDC' AS Category, ProjectName, OrgName, Organizer, LastOperation,Status, CONCAT_WS(' , ', Target, Summary, Quantified) AS ProjectContent
      FROM OFS_EDC_Project
      WHERE IsProjChanged =  2
      UNION
      SELECT Year, ProjectID, 'MUL' AS Category, ProjectName, OrgName, Organizer, LastOperation,Status, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
      FROM OFS_MUL_Project
      WHERE IsProjChanged =  2
      UNION
      SELECT Year, ProjectID, 'LIT' AS Category, ProjectName, OrgName, Organizer, LastOperation,Status, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
      FROM OFS_LIT_Project
      WHERE IsProjChanged =  2
      UNION
      SELECT Year, ProjectID, 'ACC' AS Category, ProjectName, OrgName, Organizer, LastOperation,Status, CONCAT_WS(' , ', Target, Summary, Quantified, Qualitative) AS ProjectContent
      FROM OFS_ACC_Project
      WHERE IsProjChanged =  2 ) AS O
         LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
         LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
where Status not in (91,99)
    GO




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



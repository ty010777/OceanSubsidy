

INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('MULField', '1', '海洋巡迴教育',                   1, 1, ''),
                          ('MULField', '2', '職涯探索及知海系列講座教育推廣', 2, 1, ''),
                          ('MULField', '3', '海洋素養國際研討會',             3, 1, ''),
                          ('MULField', '4', '臺灣國際學生體驗、探索海洋活動', 4, 1, '')
GO


INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('MULOrgCategory', '1', '公立之博物館、社教館所', 1, 1, ''),
                          ('MULOrgCategory', '2', '大專校院',               2, 1, '')
GO


-- 基本資料

CREATE TABLE [OFS_MUL_Project] (
    [ID]                 INT PRIMARY KEY IDENTITY,
    [Year]               INT             NOT NULL, -- 年度
    [ProjectID]          VARCHAR(50)     NOT NULL, -- 計畫編號
    [SubsidyPlanType]    VARCHAR(50)     NOT NULL, -- 補助計畫類別
    [ProjectName]        NVARCHAR(200)       NULL, -- 計畫名稱
    [Field]              VARCHAR(50)         NULL, -- 計畫類別
    [OrgName]            NVARCHAR(50)        NULL, -- 申請單位
    [OrgCategory]        VARCHAR(50)         NULL, -- 申請單位類別
    [TaxID]              VARCHAR(10)         NULL, -- 統一編號
    [Address]            NVARCHAR(200)       NULL, -- 立案聯絡地址
    [Target]             NVARCHAR(600)       NULL, -- 計畫目標
    [Summary]            NVARCHAR(600)       NULL, -- 計畫內容概要
    [Quantified]         NVARCHAR(300)       NULL, -- 預期效益 (量化)
    [Qualitative]        NVARCHAR(300)       NULL, -- 預期效益 (質化)
    [StartTime]          DATETIME            NULL, -- 計畫期程 (起)
    [EndTime]            DATETIME            NULL, -- 計畫期程 (迄)
    [ApplyAmount]        INT                 NULL, -- 申請海委會補助／合作金額
    [SelfAmount]         INT                 NULL, -- 申請單位自籌款
    [OtherAmount]        INT                 NULL, -- 其他機關補助／合作總金額
    [Benefit]            NVARCHAR(600)       NULL, -- 不可量化成果
    [FormStep]           INT             NOT NULL, -- 申請進度
    [Status]             INT             NOT NULL, -- 狀態
    [Organizer]          INT                 NULL, -- 承辦人員
    [RejectReason]       NVARCHAR(600)       NULL, -- 不通過原因
    [CorrectionDeadline] DATETIME            NULL, -- 補正期限
    [UserAccount]        VARCHAR(30)         NULL,
    [UserName]           NVARCHAR(30)        NULL,
    [UserOrg]            NVARCHAR(30)        NULL,
    [CreateTime]         DATETIME        NOT NULL,
    [CreateUser]         VARCHAR(50)     NOT NULL,
    [UpdateTime]         DATETIME            NULL,
    [UpdateUser]         VARCHAR(50)         NULL
)
GO


-- 人員聯絡資訊

CREATE TABLE [OFS_MUL_Contact] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [Role]        NVARCHAR(50)        NULL, -- 角色
    [Name]        NVARCHAR(100)       NULL, -- 姓名
    [JobTitle]    NVARCHAR(50)        NULL, -- 職稱
    [Phone]       VARCHAR(20)         NULL, -- 電話
    [PhoneExt]    VARCHAR(10)         NULL, -- 分機
    [MobilePhone] VARCHAR(20)         NULL, -- 手機號碼
    [EMail]       VARCHAR(30)         NULL, -- 電子郵件
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO


-- 最近三年曾獲本會及所屬補助／合作計畫及經費

CREATE TABLE [OFS_MUL_Received_Subsidy] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Name]       NVARCHAR(50)        NULL, -- 計畫名稱
    [Unit]       NVARCHAR(50)        NULL, -- 補助單位
    [Amount]     INT                 NULL, -- 補助金額
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 工作項目

CREATE TABLE [OFS_MUL_Item] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Title]      NVARCHAR(50)        NULL, -- 工作項目
    [Begin]      INT                 NULL, -- 月份 (起)
    [End]        INT                 NULL, -- 月份 (迄)
    [Deadline]   DATETIME            NULL, -- 預定完成日
    [Content]    NVARCHAR(600)       NULL, -- 詳細執行內容說明
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 進度說明

CREATE TABLE [OFS_MUL_Schedule] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Type]       INT             NOT NULL, -- 1:50%, 2:100%
    [ItemID]     INT             NOT NULL,
    [Deadline]   DATETIME            NULL, -- 預定完成日
    [Content]    NVARCHAR(600)       NULL, -- 查核內容概述
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 其他機關補助

CREATE TABLE [OFS_MUL_Other_Subsidy] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Unit]       NVARCHAR(50)        NULL, -- 單位名稱
    [Amount]     INT                 NULL, -- 補助金額
    [Content]    NVARCHAR(200)       NULL, -- 申請合作項目
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 經費預算規劃

CREATE TABLE [OFS_MUL_Budget_Plan] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [Title]       NVARCHAR(50)        NULL, -- 預算項目
    [Amount]      INT                 NULL, -- 海洋委員會經費
    [OtherAmount] INT                 NULL, -- 其他配合經費
    [Description] NVARCHAR(100)       NULL, -- 計算方式及說明
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO


-- 可量化成果

CREATE TABLE [OFS_MUL_Benefit] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [Title]       NVARCHAR(50)        NULL, -- 指標
    [Target]      NVARCHAR(50)        NULL, -- 目標值
    [Description] NVARCHAR(600)       NULL, -- 說明
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO


-- 附件

CREATE TABLE [OFS_MUL_Attachment] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Type]       INT             NOT NULL,
    [Path]       NVARCHAR(100)   NOT NULL,
    [Name]       NVARCHAR(200)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO



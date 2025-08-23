

CREATE TABLE [OFS_Base_File] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [Name]       NVARCHAR(200)   NOT NULL,
    [Path]       NVARCHAR(1000)  NOT NULL,
    [Size]       BIGINT          NOT NULL,
    [Type]       VARCHAR(100)        NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL
)
GO


INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('CULField', '10', '薪傳',           1, 1, ''),
                          ('CULField', '11', '航海智慧轉譯類', 1, 1, '10'),
                          ('CULField', '12', '海岸聚落發展類', 2, 1, '10'),
                          ('CULField', '13', '圖文繪本創新類', 3, 1, '10'),
                          ('CULField', '20', '船藝',           2, 1, ''),
                          ('CULField', '21', '造舟技藝傳承類', 1, 1, '20'),
                          ('CULField', '22', '航海實踐交流類', 2, 1, '20'),
                          ('CULField', '30', '藝海',           3, 1, ''),
                          ('CULField', '31', '海洋主題創作類', 1, 1, '30'),
                          ('CULField', '32', '海洋藝文扎根類', 2, 1, '30')
GO


INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('CULOrgCategory', '1', '公立之博物館及社教館所（合作案件）', 1, 1, ''),
                          ('CULOrgCategory', '2', '公私立學校、大專校院',               2, 1, ''),
                          ('CULOrgCategory', '3', '財團法人、社團法人及其他人民團體',   3, 1, '')
GO


-- 基本資料

CREATE TABLE [OFS_CUL_Project] (
    [ID]              INT PRIMARY KEY IDENTITY,
    [Year]            INT             NOT NULL, -- 年度
    [ProjectID]       VARCHAR(50)     NOT NULL, -- 計畫編號
    [SubsidyPlanType] VARCHAR(50)     NOT NULL, -- 補助計畫類別
    [ProjectName]     NVARCHAR(200)       NULL, -- 計畫名稱
    [Field]           VARCHAR(50)         NULL, -- 徵件類別
    [OrgName]         NVARCHAR(50)        NULL, -- 申請單位
    [OrgCategory]     VARCHAR(50)         NULL, -- 申請單位類別
    [RegisteredNum]   NVARCHAR(50)        NULL, -- 立案登記證字號
    [TaxID]           VARCHAR(10)         NULL, -- 統一編號
    [Address]         NVARCHAR(200)       NULL, -- 立案聯絡地址
    [Target]          NVARCHAR(600)       NULL, -- 計畫目標
    [Summary]         NVARCHAR(600)       NULL, -- 計畫內容概要
    [Quantified]      NVARCHAR(300)       NULL, -- 預期效益 (量化)
    [Qualitative]     NVARCHAR(300)       NULL, -- 預期效益 (質化)
    [StartTime]       DATETIME            NULL, -- 計畫期程 (起)
    [EndTime]         DATETIME            NULL, -- 計畫期程 (迄)
    [ApplyAmount]     INT                 NULL, -- 申請海委會補助／合作金額
    [SelfAmount]      INT                 NULL, -- 申請單位自籌款
    [OtherAmount]     INT                 NULL, -- 其他機關補助／合作總金額
    [FormStep]        INT             NOT NULL, -- 申請進度
    [Status]          INT             NOT NULL, -- 狀態
    [UserAccount]     VARCHAR(30)         NULL,
    [UserName]        NVARCHAR(30)        NULL,
    [UserOrg]         NVARCHAR(30)        NULL,
    [CreateTime]      DATETIME        NOT NULL,
    [CreateUser]      VARCHAR(50)     NOT NULL,
    [UpdateTime]      DATETIME            NULL,
    [UpdateUser]      VARCHAR(50)         NULL
)
GO


-- 人員聯絡資訊

CREATE TABLE [OFS_CUL_Contact] (
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

CREATE TABLE [OFS_CUL_Received_Subsidy] (
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


-- 計畫目標

CREATE TABLE [OFS_CUL_Goal] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Title]      NVARCHAR(50)        NULL, -- 計畫目標
    [Content]    NVARCHAR(600)       NULL, -- 預期效益（含量化或質化說明）
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 工作項目

CREATE TABLE [OFS_CUL_Goal_Item] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [GoalID]     INT             NOT NULL,
    [Title]      NVARCHAR(50)        NULL, -- 重要工作項目
    [Indicator]  NVARCHAR(100)       NULL, -- 績效指標
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 實施步驟

CREATE TABLE [OFS_CUL_Goal_Step] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [ItemID]     INT             NOT NULL,
    [Title]      NVARCHAR(50)        NULL, -- 步驟
    [Begin]      INT                 NULL, -- 月份 (起)
    [End]        INT                 NULL, -- 月份 (迄)
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 工作進度

CREATE TABLE [OFS_CUL_Goal_Schedule] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [ItemID]     INT             NOT NULL,
    [Type]       INT             NOT NULL, -- 1:50%, 2:100%
    [Month]      INT                 NULL, -- 月份
    [StepID]     INT                 NULL, -- 實施步驟ID
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 其他機關補助

CREATE TABLE [OFS_CUL_Other_Subsidy] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Unit]       NVARCHAR(50)        NULL, -- 單位名稱
    [Amount]     INT                 NULL, -- 補助金額
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 經費預算規劃

CREATE TABLE [OFS_CUL_Budget_Plan] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [ItemID]      INT             NOT NULL, -- 重要工作項目
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


-- 相關計畫

CREATE TABLE [OFS_CUL_Related_Project] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [Title]       NVARCHAR(50)        NULL, -- 相關計畫名稱
    [Year]        INT                 NULL, -- 執行年度
    [OrgName]     NVARCHAR(50)        NULL, -- 執行單位
    [Amount]      INT                 NULL, -- 計畫經費
    [Description] NVARCHAR(600)       NULL, -- 內容摘述
    [Benefit]     NVARCHAR(600)       NULL, -- 執行效益
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO


-- 附件

CREATE TABLE [OFS_CUL_Attachment] (
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





INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('EDCOrgCategory', '1', '學校',     1, 1, ''),
                          ('EDCOrgCategory', '2', '民間團體', 2, 1, '')
GO


-- 基本資料

CREATE TABLE [OFS_EDC_Project] (
    [ID]                 INT PRIMARY KEY IDENTITY,
    [Year]               INT             NOT NULL, -- 年度
    [ProjectID]          VARCHAR(50)     NOT NULL, -- 計畫編號
    [SubsidyPlanType]    VARCHAR(50)     NOT NULL, -- 補助計畫類別
    [ProjectName]        NVARCHAR(200)       NULL, -- 計畫名稱
    [OrgCategory]        VARCHAR(50)         NULL, -- 申請單位類型
    [OrgName]            NVARCHAR(50)        NULL, -- 申請單位
    [RegisteredNum]      NVARCHAR(50)        NULL, -- 立案登記證字號
    [TaxID]              VARCHAR(10)         NULL, -- 統一編號
    [Address]            NVARCHAR(200)       NULL, -- 地址
    [StartTime]          DATETIME            NULL, -- 計畫期程 (起)
    [EndTime]            DATETIME            NULL, -- 計畫期程 (迄)
    [Target]             NVARCHAR(200)       NULL, -- 參加對象及人數
    [Summary]            NVARCHAR(600)       NULL, -- 計畫內容摘要
    [Quantified]         NVARCHAR(600)       NULL, -- 預期效益
    [ApplyAmount]        INT                 NULL, -- 申請海委會補助經費
    [SelfAmount]         INT                 NULL, -- 申請單位自籌款
    [OtherGovAmount]     INT                 NULL, -- 其他政府機關補助經費
    [OtherUnitAmount]    INT                 NULL, -- 其他單位補助經費（含總收費）
    [ApprovedAmount]     INT                 NULL, -- 核定金額
    [RecoveryAmount]     INT                 NULL, -- 追回金額
    [FormStep]           INT             NOT NULL, -- 申請進度
    [Status]             INT             NOT NULL, -- 狀態
    [ProgressStatus]     INT             NOT NULL, -- 執行狀態
    [Organizer]          INT                 NULL, -- 承辦人員
    [RejectReason]       NVARCHAR(600)       NULL, -- 不通過原因
    [CorrectionDeadline] DATETIME            NULL, -- 補正期限
    [UserAccount]        VARCHAR(30)         NULL,
    [UserName]           NVARCHAR(30)        NULL,
    [UserOrg]            NVARCHAR(30)        NULL,
    [IsProjChanged]      BIT             NOT NULL, -- 是否計畫變更中
    [IsWithdrawal]       BIT             NOT NULL, -- 是否撤銷
    [IsExists]           BIT             NOT NULL, -- 是否有效
    [CreateTime]         DATETIME        NOT NULL,
    [CreateUser]         VARCHAR(50)     NOT NULL,
    [UpdateTime]         DATETIME            NULL,
    [UpdateUser]         VARCHAR(50)         NULL
)
GO


-- 人員聯絡資訊

CREATE TABLE [OFS_EDC_Contact] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [Role]        NVARCHAR(50)        NULL, -- 角色
    [Name]        NVARCHAR(100)       NULL, -- 姓名
    [JobTitle]    NVARCHAR(50)        NULL, -- 職稱
    [Phone]       VARCHAR(20)         NULL, -- 電話
    [PhoneExt]    VARCHAR(10)         NULL, -- 分機
    [MobilePhone] VARCHAR(20)         NULL, -- 手機號碼
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO


-- 最近兩年曾獲本會補助

CREATE TABLE [OFS_EDC_Received_Subsidy] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [PID]        INT             NOT NULL,
    [Name]       NVARCHAR(50)        NULL, -- 計畫名稱
    [Amount]     INT                 NULL, -- 海委會補助經費
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


-- 附件

CREATE TABLE [OFS_EDC_Attachment] (
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



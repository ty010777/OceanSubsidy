

ALTER TABLE [OFS_GrantType] ADD [Year] INT
GO
UPDATE [OFS_GrantType] SET [Year] = 114
GO
ALTER TABLE [OFS_GrantType] ADD [Year] INT NOT NULL
GO
ALTER TABLE [OFS_GrantType] ALTER COLUMN [BudgetFees] INT
GO
ALTER TABLE [OFS_GrantType] ALTER COLUMN [PlanEndDate] DATE
GO
ALTER TABLE [OFS_GrantType] ADD [MidtermDeadline] DATE
GO
ALTER TABLE [OFS_GrantType] ADD [FinalDeadline] DATE
GO
ALTER TABLE [OFS_GrantType] ADD [FinalOneMonth] BIT DEFAULT 0
GO
ALTER TABLE [OFS_GrantType] ADD Review1Title NVARCHAR(10)
GO
ALTER TABLE [OFS_GrantType] ADD Review1Enabled BIT DEFAULT 0
GO
ALTER TABLE [OFS_GrantType] ADD Review2Title NVARCHAR(10)
GO
ALTER TABLE [OFS_GrantType] ADD Review2Enabled BIT DEFAULT 0
GO
ALTER TABLE [OFS_GrantType] ADD Review3Title NVARCHAR(10)
GO
ALTER TABLE [OFS_GrantType] ADD Review3Enabled BIT DEFAULT 0
GO
ALTER TABLE [OFS_GrantType] ADD Review4Title NVARCHAR(10)
GO
ALTER TABLE [OFS_GrantType] ADD Review4Enabled BIT DEFAULT 0
GO


CREATE TABLE [OFS_GrantTypeContent] (
    [TypeID]         INT PRIMARY KEY NOT NULL,
    [ServiceContent] NVARCHAR(600)       NULL,
    [Criteria]       NVARCHAR(600)       NULL,
    [Documentary]    NVARCHAR(600)       NULL,
    [ContactPerson]  NVARCHAR(20)        NULL,
    [ContactTel]     NVARCHAR(30)        NULL,
    [Path]           NVARCHAR(100)       NULL,
    [Filename]       NVARCHAR(200)       NULL,
    [WorkingDays]    INT                 NULL,
    [Remark]         NVARCHAR(600)       NULL,
    [IsValid]        BIT             NOT NULL,
    [StatusReason]   NVARCHAR(600)       NULL,
    [CreateTime]     DATETIME        NOT NULL,
    [CreateUser]     VARCHAR(50)     NOT NULL,
    [UpdateTime]     DATETIME            NULL,
    [UpdateUser]     VARCHAR(50)         NULL
)
GO


CREATE TABLE [OFS_GrantTypeProcedure] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [TypeID]     INT             NOT NULL,
    [Content]    NVARCHAR(600)       NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO


CREATE TABLE [OFS_GrantTypeOnlineLink] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [TypeID]     INT             NOT NULL,
    [URL]        NVARCHAR(600)       NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO



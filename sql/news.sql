

CREATE TABLE [OFS_News] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [Title]       NVARCHAR(100)   NOT NULL,
    [Content]     NVARCHAR(MAX)   NOT NULL,
    [EnableTime]  DATETIME        NOT NULL,
    [DisableTime] DATETIME            NULL,
    [UserName]    NVARCHAR(30)    NOT NULL,
    [UserOrg]     NVARCHAR(30)    NOT NULL,
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO

CREATE TABLE [OFS_News_File] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [NewsID]     INT             NOT NULL,
    [Path]       NVARCHAR(100)   NOT NULL,
    [Name]       NVARCHAR(200)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL
)
GO

CREATE TABLE [OFS_News_Image] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [NewsID]     INT             NOT NULL,
    [Path]       NVARCHAR(100)   NOT NULL,
    [Name]       NVARCHAR(200)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL
)
GO

CREATE TABLE [OFS_News_Video] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [NewsID]     INT             NOT NULL,
    [Title]      NVARCHAR(100)   NOT NULL,
    [URL]        NVARCHAR(300)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL,
    [UpdateTime] DATETIME            NULL,
    [UpdateUser] VARCHAR(50)         NULL
)
GO



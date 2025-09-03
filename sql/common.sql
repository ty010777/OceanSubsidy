

CREATE TABLE [OFS_Base_File] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [Name]       NVARCHAR(200)   NOT NULL,
    [Path]       NVARCHAR(1000)  NOT NULL,
    [Size]       BIGINT          NOT NULL,
    [Type]       VARCHAR(100)        NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL
)


-- 快照

CREATE TABLE [OFS_Snapshot] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [Type]       VARCHAR(3)      NOT NULL,
    [DataID]     INT             NOT NULL,
    [Status]     INT             NOT NULL,
    [Data]       NVARCHAR(MAX)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL
);


-- 計畫變更紀錄

CREATE TABLE [OFS_ProjectChangeRecord] (
    [ID]           INT PRIMARY KEY IDENTITY,
    [Type]         VARCHAR(3)      NOT NULL,
    [DataID]       INT             NOT NULL,
    [Reason]       NVARCHAR(500)   NOT NULL,
    [Form1Before]  NVARCHAR(MAX)       NULL,
    [Form1After]   NVARCHAR(MAX)       NULL,
    [Form2Before]  NVARCHAR(MAX)       NULL,
    [Form2After]   NVARCHAR(MAX)       NULL,
    [Form3Before]  NVARCHAR(MAX)       NULL,
    [Form3After]   NVARCHAR(MAX)       NULL,
    [Form4Before]  NVARCHAR(MAX)       NULL,
    [Form4After]   NVARCHAR(MAX)       NULL,
    [Form5Before]  NVARCHAR(MAX)       NULL,
    [Form5After]   NVARCHAR(MAX)       NULL,
    [Status]       INT             NOT NULL,
    [RejectReason] NVARCHAR(600)       NULL,
    [CreateTime]   DATETIME        NOT NULL,
    [CreateUser]   VARCHAR(50)     NOT NULL,
    [UpdateTime]   DATETIME            NULL,
    [UpdateUser]   VARCHAR(50)         NULL
);



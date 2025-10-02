

CREATE TABLE [OFS_GrantTypeContentLog] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [TypeID]     INT             NOT NULL,
    [URL]        VARCHAR(100)    NOT NULL,
    [Method]     VARCHAR(10)     NOT NULL,
    [Content]    NVARCHAR(MAX)   NOT NULL,
    [Result]     NVARCHAR(MAX)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL
)
GO


ALTER TABLE [OFS_GrantTypeContent] DROP COLUMN [IsValid]
GO

ALTER TABLE [OFS_GrantTypeContent] ADD [Status] INT NOT NULL DEFAULT 0
GO

ALTER TABLE [OFS_GrantTypeContent] ADD [Keywords] NVARCHAR(MAX)
GO



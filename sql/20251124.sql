

CREATE TABLE [OFS_GrantTypeContentIdentifier] (
    [ID]         INT PRIMARY KEY IDENTITY(100000,1),
    [TypeID]     INT NOT NULL,
    [CreateTime] DATETIME NOT NULL,
    [CreateUser] VARCHAR(50) NOT NULL
)
GO


ALTER TABLE [OFS_GrantTypeContent] ADD [Identifier] INT
GO


UPDATE [OFS_GrantTypeContent] SET [Identifier] = [TypeID] WHERE [Status] = 1
GO



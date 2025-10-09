

CREATE TABLE [EmailLog] (
    [ID]         INT PRIMARY KEY IDENTITY,
    [To]         NVARCHAR(MAX)       NULL,
    [Cc]         NVARCHAR(MAX)       NULL,
    [Bcc]        NVARCHAR(MAX)       NULL,
    [Subject]    NVARCHAR(MAX)   NOT NULL,
    [Body]       NVARCHAR(MAX)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [SendTime]   DATETIME            NULL
)
GO



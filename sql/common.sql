

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
    [ID]         INT PRIMARY KEY IDENTITY,
    [Type]       VARCHAR(3)      NOT NULL,
    [DataID]     INT             NOT NULL,
    [Reason]     NVARCHAR(500)   NOT NULL,
    [CreateTime] DATETIME        NOT NULL,
    [CreateUser] VARCHAR(50)     NOT NULL
);




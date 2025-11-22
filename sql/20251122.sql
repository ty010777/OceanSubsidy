

-- 其他機關補助

CREATE TABLE [OFS_EDC_Other_Subsidy] (
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

CREATE TABLE [OFS_EDC_Budget_Plan] (
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





ALTER TABLE [OFS_CUL_Project] ADD FinalReviewNotes NVARCHAR(500)
ALTER TABLE [OFS_CUL_Project] ADD FinalReviewOrder INT

ALTER TABLE [OFS_EDC_Project] ADD FinalReviewNotes NVARCHAR(500)
ALTER TABLE [OFS_EDC_Project] ADD FinalReviewOrder INT

ALTER TABLE [OFS_MUL_Project] ADD FinalReviewNotes NVARCHAR(500)
ALTER TABLE [OFS_MUL_Project] ADD FinalReviewOrder INT

ALTER TABLE [OFS_LIT_Project] ADD FinalReviewNotes NVARCHAR(500)
ALTER TABLE [OFS_LIT_Project] ADD FinalReviewOrder INT

ALTER TABLE [OFS_ACC_Project] ADD FinalReviewNotes NVARCHAR(500)
ALTER TABLE [OFS_ACC_Project] ADD FinalReviewOrder INT

ALTER TABLE [OFS_CUL_Project] ADD LastOperation NVARCHAR(50)
ALTER TABLE [OFS_EDC_Project] ADD LastOperation NVARCHAR(50)
ALTER TABLE [OFS_MUL_Project] ADD LastOperation NVARCHAR(50)
ALTER TABLE [OFS_LIT_Project] ADD LastOperation NVARCHAR(50)
ALTER TABLE [OFS_ACC_Project] ADD LastOperation NVARCHAR(50)

ALTER TABLE [OFS_CUL_Goal_Schedule] ADD Status INT NOT NULL DEFAULT 0


-- 每月進度

CREATE TABLE [OFS_CUL_Monthly_Progress] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [Year]        INT             NOT NULL,
    [Month]       INT             NOT NULL,
    [Description] NVARCHAR(500)       NULL, -- 實際工作執行情形
    [Status]      INT             NOT NULL, -- 0:暫存, 1:提交
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO


-- 每月進度明細

CREATE TABLE [OFS_CUL_Monthly_Progress_Log] (
    [ID]          INT PRIMARY KEY IDENTITY,
    [PID]         INT             NOT NULL,
    [MPID]        INT             NOT NULL,
    [ScheduleID]  INT             NOT NULL,
    [Status]      INT                 NULL, -- 1:未完成, 2:部分完成, 3:完成
    [DelayReason] NVARCHAR(500)       NULL, -- 落後原因
    [ImprovedWay] NVARCHAR(500)       NULL, -- 改善措施
    [CreateTime]  DATETIME        NOT NULL,
    [CreateUser]  VARCHAR(50)     NOT NULL,
    [UpdateTime]  DATETIME            NULL,
    [UpdateUser]  VARCHAR(50)         NULL
)
GO



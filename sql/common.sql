

DELETE FROM [Sys_ZgsCode] WHERE [CodeGroup] = 'ProjectProgressStatus'
GO
INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('ProjectProgressStatus', '0', '尚未提送', 0, 1, ''),
                          ('ProjectProgressStatus', '1', '資格審查', 1, 1, ''),
                          ('ProjectProgressStatus', '2', '初審',     2, 1, ''),
                          ('ProjectProgressStatus', '3', '複審',     3, 1, ''),
                          ('ProjectProgressStatus', '4', '決審核定', 4, 1, ''),
                          ('ProjectProgressStatus', '5', '計畫執行', 5, 1, ''),
                          ('ProjectProgressStatus', '9', '結案',     9, 1, '')
GO


DELETE FROM [Sys_ZgsCode] WHERE [CodeGroup] = 'ProjectStatus'
GO
INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('ProjectStatus', '1' , '編輯中',        1, 1, ''),
                          ('ProjectStatus', '11', '審核中',       11, 1, ''),
                          ('ProjectStatus', '12', '通過',         12, 1, ''),
                          ('ProjectStatus', '13', '不通過',       13, 1, ''),
                          ('ProjectStatus', '14', '補正補件',     14, 1, ''),
                          ('ProjectStatus', '15', '逾期未補',     15, 1, ''),
                          ('ProjectStatus', '19', '結案(未通過)', 19, 1, ''),
                          ('ProjectStatus', '21', '審查中',       21, 1, ''),
                          ('ProjectStatus', '22', '通過',         22, 1, ''),
                          ('ProjectStatus', '23', '不通過',       23, 1, ''),
                          ('ProjectStatus', '29', '結案(未通過)', 29, 1, ''),
                          ('ProjectStatus', '31', '審查中',       31, 1, ''),
                          ('ProjectStatus', '32', '通過',         32, 1, ''),
                          ('ProjectStatus', '33', '不通過',       33, 1, ''),
                          ('ProjectStatus', '39', '結案(未通過)', 39, 1, ''),
                          ('ProjectStatus', '41', '核定中',       41, 1, ''),
                          ('ProjectStatus', '42', '計畫書修正中', 42, 1, ''),
                          ('ProjectStatus', '43', '計畫書審核中', 43, 1, ''),
                          ('ProjectStatus', '44', '計畫書已確認', 44, 1, ''),
                          ('ProjectStatus', '45', '已核定',       45, 1, ''),
                          ('ProjectStatus', '46', '不通過',       46, 1, ''),
                          ('ProjectStatus', '49', '結案(未通過)', 49, 1, ''),
                          ('ProjectStatus', '51', '審核中',       51, 1, ''),
                          ('ProjectStatus', '52', '通過',         52, 1, ''),
                          ('ProjectStatus', '53', '不通過',       53, 1, ''),
                          ('ProjectStatus', '91', '已結案',       91, 1, ''),
                          ('ProjectStatus', '99', '已終止',       99, 1, '')
GO


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



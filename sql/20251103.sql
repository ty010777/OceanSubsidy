

ALTER TABLE [OFS_CUL_Project] ADD ApplyTime DATETIME
ALTER TABLE [OFS_EDC_Project] ADD ApplyTime DATETIME
ALTER TABLE [OFS_MUL_Project] ADD ApplyTime DATETIME
ALTER TABLE [OFS_LIT_Project] ADD ApplyTime DATETIME
ALTER TABLE [OFS_ACC_Project] ADD ApplyTime DATETIME
GO


INSERT INTO [Sys_ZgsCode] ([CodeGroup], [Code], [Descname], [OrderNo], [IsValid], [ParentCode])
                   VALUES ('ProjectStatus', '2', '逾期', 2, 1, '');
GO


ALTER TABLE [OFS_ReviewCommitteeList] ALTER COLUMN [RegistrationAddress] NVARCHAR(MAX)
ALTER TABLE [OFS_ReviewCommitteeList] ADD [BankPhoto] VARCHAR(MAX)
GO





ALTER TABLE [OFS_ReviewTemplate] ADD [TemplateNote] NVARCHAR(500)
GO


INSERT INTO [OFS_ReviewTemplate] ([SubsidyProjects],[TemplateName],[TemplateNote],[TemplateWeight])
                          VALUES ('CUL', '1.計畫團隊之執行力',   '提案構想之可行性，包含組織分工完整度、執行方式與時程規劃。', 20),
                                 ('CUL', '2.海洋文化之主體性',   NULL,                                                         30),
                                 ('CUL', '3.推動策略之創新性',   NULL,                                                         20),
                                 ('CUL', '4.經費編列之合理性',   NULL,                                                         10),
                                 ('CUL', '5.效益性及永續發展性', NULL,                                                         20)
GO


ALTER TABLE [OFS_CUL_Attachment] ADD [Stage] INT NOT NULL DEFAULT 0
ALTER TABLE [OFS_EDC_Attachment] ADD [Stage] INT NOT NULL DEFAULT 0
ALTER TABLE [OFS_MUL_Attachment] ADD [Stage] INT NOT NULL DEFAULT 0
ALTER TABLE [OFS_LIT_Attachment] ADD [Stage] INT NOT NULL DEFAULT 0
ALTER TABLE [OFS_ACC_Attachment] ADD [Stage] INT NOT NULL DEFAULT 0
GO



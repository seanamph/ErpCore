-- 擴展管理資料表 (SYS9000)
-- 對應舊系統 SYS9_EXTENSION

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExtensionFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ExtensionFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展功能代碼 (EXTENSION_ID)
        [ExtensionName] NVARCHAR(100) NOT NULL, -- 擴展功能名稱 (EXTENSION_NAME)
        [ExtensionType] NVARCHAR(20) NULL, -- 擴展類型 (EXTENSION_TYPE, BASE:基礎, PROCESS:處理, REPORT:報表, QUERY:查詢)
        [ExtensionValue] NVARCHAR(500) NULL, -- 擴展值 (EXTENSION_VALUE)
        [ExtensionConfig] NVARCHAR(MAX) NULL, -- 擴展設定 (JSON格式) (EXTENSION_CONFIG)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Version] NVARCHAR(20) NULL, -- 版本號 (VERSION)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_ExtensionFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_ExtensionFunctions_ExtensionId] UNIQUE ([ExtensionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ExtensionFunctions_ExtensionId] ON [dbo].[ExtensionFunctions] ([ExtensionId]);
    CREATE NONCLUSTERED INDEX [IX_ExtensionFunctions_ExtensionType] ON [dbo].[ExtensionFunctions] ([ExtensionType]);
    CREATE NONCLUSTERED INDEX [IX_ExtensionFunctions_Status] ON [dbo].[ExtensionFunctions] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_ExtensionFunctions_SeqNo] ON [dbo].[ExtensionFunctions] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_ExtensionFunctions_ExtensionId_Status] ON [dbo].[ExtensionFunctions] ([ExtensionId], [Status]);
    CREATE NONCLUSTERED INDEX [IX_ExtensionFunctions_ExtensionType_Status] ON [dbo].[ExtensionFunctions] ([ExtensionType], [Status]);
    CREATE NONCLUSTERED INDEX [IX_ExtensionFunctions_CreatedAt] ON [dbo].[ExtensionFunctions] ([CreatedAt]);
END
GO


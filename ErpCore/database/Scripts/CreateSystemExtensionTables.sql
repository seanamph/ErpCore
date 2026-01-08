-- 系統擴展資料表 (SYSX110, SYSX120, SYSX140)
-- 對應舊系統 SYSX_EXTENSION

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemExtensions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemExtensions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展功能代碼 (EXTENSION_ID)
        [ExtensionName] NVARCHAR(100) NOT NULL, -- 擴展功能名稱 (EXTENSION_NAME)
        [ExtensionType] NVARCHAR(20) NULL, -- 擴展類型 (EXTENSION_TYPE)
        [ExtensionValue] NVARCHAR(500) NULL, -- 擴展值 (EXTENSION_VALUE)
        [ExtensionConfig] NVARCHAR(MAX) NULL, -- 擴展設定 (JSON格式) (EXTENSION_CONFIG)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_SystemExtensions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_SystemExtensions_ExtensionId] UNIQUE ([ExtensionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionId] ON [dbo].[SystemExtensions] ([ExtensionId]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionType] ON [dbo].[SystemExtensions] ([ExtensionType]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensions_Status] ON [dbo].[SystemExtensions] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensions_SeqNo] ON [dbo].[SystemExtensions] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionId_Status] ON [dbo].[SystemExtensions] ([ExtensionId], [Status]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionType_Status] ON [dbo].[SystemExtensions] ([ExtensionType], [Status]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensions_CreatedAt] ON [dbo].[SystemExtensions] ([CreatedAt]);
END
GO

-- 系統擴展報表記錄表 (SYSX140)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemExtensionReports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemExtensionReports] (
        [ReportId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportName] NVARCHAR(100) NOT NULL, -- 報表名稱
        [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型 (PDF, Excel, Word等)
        [ReportTemplate] NVARCHAR(MAX) NULL, -- 報表範本 (JSON格式)
        [QueryConditions] NVARCHAR(MAX) NULL, -- 查詢條件 (JSON格式)
        [GeneratedDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 產生時間
        [GeneratedBy] NVARCHAR(50) NULL, -- 產生者
        [FileUrl] NVARCHAR(500) NULL, -- 檔案URL
        [FileSize] BIGINT NULL, -- 檔案大小
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'COMPLETED', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SystemExtensionReports_GeneratedDate] ON [dbo].[SystemExtensionReports] ([GeneratedDate]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensionReports_GeneratedBy] ON [dbo].[SystemExtensionReports] ([GeneratedBy]);
    CREATE NONCLUSTERED INDEX [IX_SystemExtensionReports_Status] ON [dbo].[SystemExtensionReports] ([Status]);
END
GO


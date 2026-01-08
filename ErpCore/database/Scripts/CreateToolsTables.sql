-- 工具功能資料表
-- Export_Excel - Excel匯出功能
-- Encode_String - 字串編碼工具
-- ASPXTOASP - ASP轉ASPX工具

-- Excel匯出設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExcelExportConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ExcelExportConfigs] (
        [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [ExportName] NVARCHAR(200) NOT NULL,
        [ExportFields] NVARCHAR(MAX) NULL,
        [ExportSettings] NVARCHAR(MAX) NULL,
        [TemplatePath] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_ExcelExportConfigs_ModuleCode_ExportName] UNIQUE ([ModuleCode], [ExportName])
    );

    CREATE NONCLUSTERED INDEX [IX_ExcelExportConfigs_ModuleCode] ON [dbo].[ExcelExportConfigs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_ExcelExportConfigs_Status] ON [dbo].[ExcelExportConfigs] ([Status]);
    
    PRINT 'ExcelExportConfigs 表建立成功';
END
GO

-- Excel匯出記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExcelExportLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ExcelExportLogs] (
        [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [ConfigId] BIGINT NULL,
        [UserId] NVARCHAR(50) NOT NULL,
        [FileName] NVARCHAR(500) NOT NULL,
        [FileSize] BIGINT NULL,
        [RecordCount] INT NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CompletedAt] DATETIME2 NULL
    );

    CREATE NONCLUSTERED INDEX [IX_ExcelExportLogs_ModuleCode] ON [dbo].[ExcelExportLogs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_ExcelExportLogs_UserId] ON [dbo].[ExcelExportLogs] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_ExcelExportLogs_Status] ON [dbo].[ExcelExportLogs] ([Status]);
    
    PRINT 'ExcelExportLogs 表建立成功';
END
GO

-- 頁面轉換記錄表 (ASPXTOASP)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PageTransitions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PageTransitions] (
        [TransitionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SourceUrl] NVARCHAR(500) NOT NULL,
        [TargetUrl] NVARCHAR(500) NOT NULL,
        [SourceType] NVARCHAR(10) NOT NULL,
        [TargetType] NVARCHAR(10) NOT NULL,
        [UserId] NVARCHAR(50) NULL,
        [SessionId] NVARCHAR(100) NULL,
        [QueryString] NVARCHAR(MAX) NULL,
        [FormData] NVARCHAR(MAX) NULL,
        [SessionData] NVARCHAR(MAX) NULL,
        [CookieData] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'SUCCESS',
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_PageTransitions_UserId] ON [dbo].[PageTransitions] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_PageTransitions_SessionId] ON [dbo].[PageTransitions] ([SessionId]);
    CREATE NONCLUSTERED INDEX [IX_PageTransitions_CreatedAt] ON [dbo].[PageTransitions] ([CreatedAt]);
    
    PRINT 'PageTransitions 表建立成功';
END
GO

-- 頁面轉換對應設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PageTransitionMappings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PageTransitionMappings] (
        [MappingId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SourcePage] NVARCHAR(200) NOT NULL,
        [TargetPage] NVARCHAR(200) NOT NULL,
        [SourceType] NVARCHAR(10) NOT NULL,
        [TargetType] NVARCHAR(10) NOT NULL,
        [ParameterMapping] NVARCHAR(MAX) NULL,
        [SessionMapping] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_PageTransitionMappings_SourcePage_TargetPage] UNIQUE ([SourcePage], [TargetPage])
    );

    CREATE NONCLUSTERED INDEX [IX_PageTransitionMappings_SourcePage] ON [dbo].[PageTransitionMappings] ([SourcePage]);
    CREATE NONCLUSTERED INDEX [IX_PageTransitionMappings_TargetPage] ON [dbo].[PageTransitionMappings] ([TargetPage]);
    
    PRINT 'PageTransitionMappings 表建立成功';
END
GO


-- 資料維護功能資料表 (IMS30系列)
-- IMS30_FB - 資料瀏覽功能
-- IMS30_FI - 資料新增功能
-- IMS30_FQ - 資料查詢功能
-- IMS30_FS - 資料排序功能
-- IMS30_FU - 資料修改功能
-- IMS30_PR - 資料列印功能

-- 資料瀏覽設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataBrowseConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataBrowseConfigs] (
        [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [DisplayFields] NVARCHAR(MAX) NULL,
        [FilterFields] NVARCHAR(MAX) NULL,
        [SortFields] NVARCHAR(MAX) NULL,
        [PageSize] INT NOT NULL DEFAULT 20,
        [DefaultSort] NVARCHAR(200) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_DataBrowseConfigs_ModuleCode] UNIQUE ([ModuleCode])
    );

    CREATE NONCLUSTERED INDEX [IX_DataBrowseConfigs_ModuleCode] ON [dbo].[DataBrowseConfigs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_DataBrowseConfigs_Status] ON [dbo].[DataBrowseConfigs] ([Status]);
    
    PRINT 'DataBrowseConfigs 表建立成功';
END
GO

-- 資料新增設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataInsertConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataInsertConfigs] (
        [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [FormFields] NVARCHAR(MAX) NULL,
        [DefaultValues] NVARCHAR(MAX) NULL,
        [ValidationRules] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_DataInsertConfigs_ModuleCode] UNIQUE ([ModuleCode])
    );

    CREATE NONCLUSTERED INDEX [IX_DataInsertConfigs_ModuleCode] ON [dbo].[DataInsertConfigs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_DataInsertConfigs_Status] ON [dbo].[DataInsertConfigs] ([Status]);
    
    PRINT 'DataInsertConfigs 表建立成功';
END
GO

-- 資料查詢設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataQueryConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataQueryConfigs] (
        [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [QueryFields] NVARCHAR(MAX) NULL,
        [DisplayFields] NVARCHAR(MAX) NULL,
        [SortFields] NVARCHAR(MAX) NULL,
        [DefaultQuery] NVARCHAR(MAX) NULL,
        [PageSize] INT NOT NULL DEFAULT 20,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_DataQueryConfigs_ModuleCode] UNIQUE ([ModuleCode])
    );

    CREATE NONCLUSTERED INDEX [IX_DataQueryConfigs_ModuleCode] ON [dbo].[DataQueryConfigs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_DataQueryConfigs_Status] ON [dbo].[DataQueryConfigs] ([Status]);
    
    PRINT 'DataQueryConfigs 表建立成功';
END
GO

-- 儲存的查詢條件表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavedQueries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SavedQueries] (
        [QueryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [QueryName] NVARCHAR(200) NOT NULL,
        [QueryConditions] NVARCHAR(MAX) NOT NULL,
        [UserId] NVARCHAR(50) NOT NULL,
        [IsDefault] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_SavedQueries_ModuleCode_UserId] ON [dbo].[SavedQueries] ([ModuleCode], [UserId]);
    
    PRINT 'SavedQueries 表建立成功';
END
GO

-- 資料排序設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataSortConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataSortConfigs] (
        [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [SortFields] NVARCHAR(MAX) NULL,
        [DefaultSort] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_DataSortConfigs_ModuleCode] UNIQUE ([ModuleCode])
    );

    CREATE NONCLUSTERED INDEX [IX_DataSortConfigs_ModuleCode] ON [dbo].[DataSortConfigs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_DataSortConfigs_Status] ON [dbo].[DataSortConfigs] ([Status]);
    
    PRINT 'DataSortConfigs 表建立成功';
END
GO

-- 儲存的排序規則表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavedSorts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SavedSorts] (
        [SortId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [SortName] NVARCHAR(200) NOT NULL,
        [SortRules] NVARCHAR(MAX) NOT NULL,
        [UserId] NVARCHAR(50) NOT NULL,
        [IsDefault] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_SavedSorts_ModuleCode_UserId] ON [dbo].[SavedSorts] ([ModuleCode], [UserId]);
    
    PRINT 'SavedSorts 表建立成功';
END
GO

-- 資料修改設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataUpdateConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataUpdateConfigs] (
        [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [FormFields] NVARCHAR(MAX) NULL,
        [ReadOnlyFields] NVARCHAR(MAX) NULL,
        [ValidationRules] NVARCHAR(MAX) NULL,
        [UseOptimisticLock] BIT NOT NULL DEFAULT 1,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_DataUpdateConfigs_ModuleCode] UNIQUE ([ModuleCode])
    );

    CREATE NONCLUSTERED INDEX [IX_DataUpdateConfigs_ModuleCode] ON [dbo].[DataUpdateConfigs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_DataUpdateConfigs_Status] ON [dbo].[DataUpdateConfigs] ([Status]);
    
    PRINT 'DataUpdateConfigs 表建立成功';
END
GO

-- 資料列印設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataPrintConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataPrintConfigs] (
        [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [ReportName] NVARCHAR(200) NOT NULL,
        [TemplatePath] NVARCHAR(500) NULL,
        [TemplateType] NVARCHAR(20) NOT NULL,
        [PrintFields] NVARCHAR(MAX) NULL,
        [PrintSettings] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_DataPrintConfigs_ModuleCode_ReportName] UNIQUE ([ModuleCode], [ReportName])
    );

    CREATE NONCLUSTERED INDEX [IX_DataPrintConfigs_ModuleCode] ON [dbo].[DataPrintConfigs] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_DataPrintConfigs_Status] ON [dbo].[DataPrintConfigs] ([Status]);
    
    PRINT 'DataPrintConfigs 表建立成功';
END
GO


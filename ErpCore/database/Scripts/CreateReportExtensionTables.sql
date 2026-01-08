-- =============================================
-- 報表擴展類資料表建立腳本
-- =============================================

-- 1. 報表查詢設定 (ReportQueries)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportQueries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReportQueries] (
        [QueryId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ReportCode] NVARCHAR(50) NOT NULL,
        [ReportName] NVARCHAR(200) NOT NULL,
        [QueryName] NVARCHAR(200) NULL,
        [QueryParams] NVARCHAR(MAX) NULL,
        [QuerySql] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ReportQueries_ReportCode] ON [dbo].[ReportQueries] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_ReportQueries_Status] ON [dbo].[ReportQueries] ([Status]);
END
GO

-- 2. 報表查詢記錄 (ReportQueryLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportQueryLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReportQueryLogs] (
        [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [QueryId] UNIQUEIDENTIFIER NULL,
        [ReportCode] NVARCHAR(50) NOT NULL,
        [UserId] NVARCHAR(50) NOT NULL,
        [QueryParams] NVARCHAR(MAX) NULL,
        [QueryTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ExecutionTime] INT NULL,
        [RecordCount] INT NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        CONSTRAINT [FK_ReportQueryLogs_ReportQueries] FOREIGN KEY ([QueryId]) REFERENCES [dbo].[ReportQueries] ([QueryId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ReportQueryLogs_ReportCode] ON [dbo].[ReportQueryLogs] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_ReportQueryLogs_UserId] ON [dbo].[ReportQueryLogs] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_ReportQueryLogs_QueryTime] ON [dbo].[ReportQueryLogs] ([QueryTime]);
END
GO

-- 3. 報表列印記錄 (ReportPrintLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportPrintLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReportPrintLogs] (
        [PrintLogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportCode] NVARCHAR(50) NOT NULL,
        [ReportName] NVARCHAR(200) NOT NULL,
        [PrintType] NVARCHAR(20) NOT NULL,
        [PrintFormat] NVARCHAR(20) NULL,
        [FilePath] NVARCHAR(500) NULL,
        [FileName] NVARCHAR(255) NULL,
        [FileSize] BIGINT NULL,
        [PrintStatus] NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        [PrintCount] INT NOT NULL DEFAULT 1,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [PrintedAt] DATETIME2 NULL
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_ReportCode] ON [dbo].[ReportPrintLogs] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_CreatedAt] ON [dbo].[ReportPrintLogs] ([CreatedAt]);
    CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_PrintStatus] ON [dbo].[ReportPrintLogs] ([PrintStatus]);
END
GO

-- 4. 報表統計記錄 (ReportStatistics)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportStatistics]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReportStatistics] (
        [StatisticId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportCode] NVARCHAR(50) NOT NULL,
        [ReportName] NVARCHAR(200) NOT NULL,
        [StatisticType] NVARCHAR(50) NOT NULL,
        [StatisticDate] DATETIME2 NOT NULL,
        [StatisticValue] DECIMAL(18,2) NULL,
        [StatisticData] NVARCHAR(MAX) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ReportStatistics_ReportCode] ON [dbo].[ReportStatistics] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_ReportStatistics_StatisticDate] ON [dbo].[ReportStatistics] ([StatisticDate]);
    CREATE NONCLUSTERED INDEX [IX_ReportStatistics_StatisticType] ON [dbo].[ReportStatistics] ([StatisticType]);
END
GO


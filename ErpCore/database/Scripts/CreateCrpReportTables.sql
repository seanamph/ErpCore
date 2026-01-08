-- CRP報表模組資料表
-- Crystal Reports報表功能

-- Crystal Reports設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CrystalReports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CrystalReports] (
        [ReportId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportCode] NVARCHAR(50) NOT NULL,
        [ReportName] NVARCHAR(200) NOT NULL,
        [ReportPath] NVARCHAR(500) NOT NULL,
        [MdbName] NVARCHAR(200) NULL,
        [Parameters] NVARCHAR(MAX) NULL,
        [ExportOptions] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_CrystalReports_ReportCode] UNIQUE ([ReportCode])
    );

    CREATE NONCLUSTERED INDEX [IX_CrystalReports_ReportCode] ON [dbo].[CrystalReports] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_CrystalReports_Status] ON [dbo].[CrystalReports] ([Status]);
    
    PRINT 'CrystalReports 表建立成功';
END
GO

-- Crystal Reports操作記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CrystalReportLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CrystalReportLogs] (
        [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportId] BIGINT NOT NULL,
        [ReportCode] NVARCHAR(50) NOT NULL,
        [OperationType] NVARCHAR(20) NOT NULL,
        [Parameters] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [FileSize] BIGINT NULL,
        [Duration] INT NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CrystalReportLogs_CrystalReports] FOREIGN KEY ([ReportId]) REFERENCES [dbo].[CrystalReports] ([ReportId])
    );

    CREATE NONCLUSTERED INDEX [IX_CrystalReportLogs_ReportId] ON [dbo].[CrystalReportLogs] ([ReportId]);
    CREATE NONCLUSTERED INDEX [IX_CrystalReportLogs_ReportCode] ON [dbo].[CrystalReportLogs] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_CrystalReportLogs_CreatedAt] ON [dbo].[CrystalReportLogs] ([CreatedAt]);
    
    PRINT 'CrystalReportLogs 表建立成功';
END
GO


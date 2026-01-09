-- 報表模板表 (SYSG710-SYSG7I0 - 報表列印作業)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportTemplates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReportTemplates] (
        [TemplateId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [TemplateName] NVARCHAR(100) NOT NULL,
        [TemplateType] NVARCHAR(20) NOT NULL, -- PDF, EXCEL, WORD
        [ReportType] NVARCHAR(50) NOT NULL, -- SALES_ORDER, SALES_SUMMARY等
        [TemplateContent] NVARCHAR(MAX) NULL, -- 模板內容（HTML、XML等）
        [TemplateFile] NVARCHAR(500) NULL, -- 模板檔案路徑
        [Parameters] NVARCHAR(MAX) NULL, -- 報表參數JSON
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ReportTemplates_ReportType] ON [dbo].[ReportTemplates] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_ReportTemplates_Status] ON [dbo].[ReportTemplates] ([Status]);
END
GO

-- 報表列印記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportPrintLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReportPrintLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportId] NVARCHAR(50) NOT NULL, -- 報表編號
        [ReportType] NVARCHAR(50) NOT NULL,
        [TemplateId] NVARCHAR(50) NULL,
        [PrintUserId] NVARCHAR(50) NOT NULL,
        [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [PrintFormat] NVARCHAR(20) NOT NULL, -- PDF, EXCEL, WORD
        [FileUrl] NVARCHAR(500) NULL, -- 檔案下載連結
        [Parameters] NVARCHAR(MAX) NULL, -- 報表參數JSON
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'S', -- S:成功, F:失敗
        [ErrorMessage] NVARCHAR(1000) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_ReportType] ON [dbo].[ReportPrintLogs] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_PrintUserId] ON [dbo].[ReportPrintLogs] ([PrintUserId]);
    CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_PrintDate] ON [dbo].[ReportPrintLogs] ([PrintDate]);
END
GO

-- =============================================
-- HTML轉PDF工具資料表建立腳本
-- =============================================

-- 1. PDF轉換記錄 (PdfConversionLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PdfConversionLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PdfConversionLogs] (
        [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [SourceHtml] NVARCHAR(MAX) NULL,
        [PdfFilePath] NVARCHAR(500) NULL,
        [FileName] NVARCHAR(255) NULL,
        [FileSize] BIGINT NULL,
        [ConversionStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CompletedAt] DATETIME2 NULL
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PdfConversionLogs_Status] ON [dbo].[PdfConversionLogs] ([ConversionStatus]);
    CREATE NONCLUSTERED INDEX [IX_PdfConversionLogs_CreatedAt] ON [dbo].[PdfConversionLogs] ([CreatedAt]);
    CREATE NONCLUSTERED INDEX [IX_PdfConversionLogs_CreatedBy] ON [dbo].[PdfConversionLogs] ([CreatedBy]);
END
GO


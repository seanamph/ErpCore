-- =============================================
-- HT680 - BAT格式文本文件處理系列資料表建立腳本
-- 功能代碼: HT680
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 文本文件處理記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TextFileProcessLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TextFileProcessLog] (
        [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FileName] NVARCHAR(255) NOT NULL,
        [FileType] NVARCHAR(50) NOT NULL, -- BACK, INV, ORDER, ORDER_6, POP, PRIC
        [ShopId] NVARCHAR(50) NULL,
        [TotalRecords] INT NULL DEFAULT 0,
        [SuccessRecords] INT NULL DEFAULT 0,
        [FailedRecords] INT NULL DEFAULT 0,
        [ProcessStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- PENDING, PROCESSING, COMPLETED, FAILED
        [ProcessStartTime] DATETIME2 NULL,
        [ProcessEndTime] DATETIME2 NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_FileName] ON [dbo].[TextFileProcessLog] ([FileName]);
    CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_FileType] ON [dbo].[TextFileProcessLog] ([FileType]);
    CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_ShopId] ON [dbo].[TextFileProcessLog] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_ProcessStatus] ON [dbo].[TextFileProcessLog] ([ProcessStatus]);
    CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_CreatedAt] ON [dbo].[TextFileProcessLog] ([CreatedAt]);

    PRINT '資料表 TextFileProcessLog 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TextFileProcessLog 已存在';
END
GO

-- 2. 文本文件處理明細表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TextFileProcessDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TextFileProcessDetail] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [LogId] UNIQUEIDENTIFIER NOT NULL,
        [LineNumber] INT NOT NULL,
        [RawData] NVARCHAR(500) NULL,
        [ProcessStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- PENDING, SUCCESS, FAILED
        [ErrorMessage] NVARCHAR(500) NULL,
        [ProcessedData] NVARCHAR(MAX) NULL, -- JSON格式存儲解析後的數據
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_TextFileProcessDetail_Log] FOREIGN KEY ([LogId]) REFERENCES [dbo].[TextFileProcessLog] ([LogId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TextFileProcessDetail_LogId] ON [dbo].[TextFileProcessDetail] ([LogId]);
    CREATE NONCLUSTERED INDEX [IX_TextFileProcessDetail_ProcessStatus] ON [dbo].[TextFileProcessDetail] ([ProcessStatus]);

    PRINT '資料表 TextFileProcessDetail 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TextFileProcessDetail 已存在';
END
GO


-- Kiosk自助服務終端相關資料表

-- Kiosk交易主檔
CREATE TABLE [dbo].[KioskTransactions] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransactionId] NVARCHAR(50) NOT NULL, -- 交易編號
    [KioskId] NVARCHAR(50) NOT NULL, -- Kiosk機號
    [FunctionCode] NVARCHAR(10) NOT NULL, -- 功能代碼 (O2, A1, C2, D4, D8等)
    [CardNumber] NVARCHAR(50) NULL, -- 卡片編號
    [MemberId] NVARCHAR(50) NULL, -- 會員編號
    [TransactionDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 交易日期時間
    [RequestData] NVARCHAR(MAX) NULL, -- 請求資料（JSON格式）
    [ResponseData] NVARCHAR(MAX) NULL, -- 回應資料（JSON格式）
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Success', -- 狀態 (Success/Failed)
    [ReturnCode] NVARCHAR(10) NULL, -- 回應碼
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [UQ_KioskTransactions_TransactionId] UNIQUE ([TransactionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_KioskId] ON [dbo].[KioskTransactions] ([KioskId]);
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_FunctionCode] ON [dbo].[KioskTransactions] ([FunctionCode]);
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_TransactionDate] ON [dbo].[KioskTransactions] ([TransactionDate]);
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_CardNumber] ON [dbo].[KioskTransactions] ([CardNumber]);
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_Status] ON [dbo].[KioskTransactions] ([Status]);

-- Kiosk報表統計資料表
CREATE TABLE [dbo].[KioskReportStatistics] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportDate] DATE NOT NULL, -- 報表日期
    [KioskId] NVARCHAR(50) NULL, -- Kiosk機號（NULL表示全部）
    [FunctionCode] NVARCHAR(10) NULL, -- 功能代碼（NULL表示全部）
    [TotalCount] INT NOT NULL DEFAULT 0, -- 總交易數
    [SuccessCount] INT NOT NULL DEFAULT 0, -- 成功交易數
    [FailedCount] INT NOT NULL DEFAULT 0, -- 失敗交易數
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [UQ_KioskReportStatistics_Date_Kiosk_Function] UNIQUE ([ReportDate], [KioskId], [FunctionCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_KioskReportStatistics_ReportDate] ON [dbo].[KioskReportStatistics] ([ReportDate]);
CREATE NONCLUSTERED INDEX [IX_KioskReportStatistics_KioskId] ON [dbo].[KioskReportStatistics] ([KioskId]);
CREATE NONCLUSTERED INDEX [IX_KioskReportStatistics_FunctionCode] ON [dbo].[KioskReportStatistics] ([FunctionCode]);


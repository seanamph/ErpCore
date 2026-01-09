-- =============================================
-- 發票銷售管理類 (SYSG000) - 電子發票列印資料庫腳本
-- 功能：電子發票列印 (SYSG210-SYSG2B0)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 電子發票主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectronicInvoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ElectronicInvoices] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PosId] NVARCHAR(50) NULL, -- POS代碼
        [InvYm] NVARCHAR(6) NOT NULL, -- 發票年月 (YYYYMM)
        [Track] NVARCHAR(2) NULL, -- 字軌
        [InvNoB] NVARCHAR(10) NULL, -- 發票號碼起
        [InvNoE] NVARCHAR(10) NULL, -- 發票號碼迄
        [PrintCode] NVARCHAR(50) NULL, -- 列印條碼
        [InvoiceDate] DATETIME2 NULL, -- 發票日期
        [PrizeType] NVARCHAR(10) NULL, -- 獎項類型
        [PrizeAmt] DECIMAL(18, 4) NULL, -- 獎項金額
        [CarrierIdClear] NVARCHAR(50) NULL, -- 載具識別碼（明碼）
        [AwardPrint] NVARCHAR(10) NULL, -- 中獎列印標記
        [AwardPos] NVARCHAR(50) NULL, -- 中獎POS
        [AwardDate] DATETIME2 NULL, -- 中獎日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_InvYm] ON [dbo].[ElectronicInvoices] ([InvYm]);
    CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_Track] ON [dbo].[ElectronicInvoices] ([Track]);
    CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_PrintCode] ON [dbo].[ElectronicInvoices] ([PrintCode]);
    CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_PrizeType] ON [dbo].[ElectronicInvoices] ([PrizeType]);
    CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_Status] ON [dbo].[ElectronicInvoices] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_PosId] ON [dbo].[ElectronicInvoices] ([PosId]);

    PRINT '資料表 ElectronicInvoices 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ElectronicInvoices 已存在';
END
GO

-- 2. 電子發票列印設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ElectronicInvoicePrintSettings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ElectronicInvoicePrintSettings] (
        [SettingId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [PrintFormat] NVARCHAR(20) NOT NULL, -- 列印格式 (A4, A5, THERMAL)
        [BarcodeType] NVARCHAR(20) NULL, -- 條碼類型 (code128, ean13等)
        [BarcodeSize] INT NULL DEFAULT 40, -- 條碼高度
        [BarcodeMargin] INT NULL DEFAULT 5, -- 條碼間距
        [ColCount] INT NULL DEFAULT 2, -- 每頁欄數
        [PageCount] INT NULL DEFAULT 14, -- 每頁筆數
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ElectronicInvoicePrintSettings_Status] ON [dbo].[ElectronicInvoicePrintSettings] ([Status]);

    PRINT '資料表 ElectronicInvoicePrintSettings 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ElectronicInvoicePrintSettings 已存在';
END
GO

PRINT '電子發票列印資料表建立完成！';
GO

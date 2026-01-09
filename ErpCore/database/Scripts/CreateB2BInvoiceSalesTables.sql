-- =============================================
-- 發票銷售管理B2B類 (SYSG000_B2B) - 資料庫腳本
-- 功能：B2B發票資料維護、B2B電子發票列印、B2B銷售資料維護、B2B銷售查詢作業
-- 建立日期：2025-01-09
-- =============================================

-- 1. B2B發票格式代號表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[B2BInvoiceFormats]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[B2BInvoiceFormats] (
        [FormatId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [FormatName] NVARCHAR(100) NOT NULL,
        [FormatNameEn] NVARCHAR(100) NULL,
        [Description] NVARCHAR(500) NULL,
        [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y',
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_B2BInvoiceFormats_B2BFlag] ON [dbo].[B2BInvoiceFormats] ([B2BFlag]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoiceFormats_Status] ON [dbo].[B2BInvoiceFormats] ([Status]);
END
GO

-- 2. B2B發票主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[B2BInvoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[B2BInvoices] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceId] NVARCHAR(50) NOT NULL,
        [InvoiceType] NVARCHAR(20) NOT NULL,
        [InvoiceYear] INT NOT NULL,
        [InvoiceMonth] INT NOT NULL,
        [InvoiceYm] NVARCHAR(6) NOT NULL,
        [Track] NVARCHAR(2) NULL,
        [InvoiceNoB] NVARCHAR(10) NULL,
        [InvoiceNoE] NVARCHAR(10) NULL,
        [InvoiceFormat] NVARCHAR(50) NULL,
        [TaxId] NVARCHAR(20) NULL,
        [CompanyName] NVARCHAR(200) NULL,
        [CompanyNameEn] NVARCHAR(200) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(50) NULL,
        [Zone] NVARCHAR(50) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [Phone] NVARCHAR(50) NULL,
        [Fax] NVARCHAR(50) NULL,
        [Email] NVARCHAR(100) NULL,
        [SiteId] NVARCHAR(50) NULL,
        [SubCopy] NVARCHAR(10) NULL,
        [SubCopyValue] NVARCHAR(100) NULL,
        [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y',
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [Notes] NVARCHAR(1000) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_B2BInvoices_InvoiceId] UNIQUE ([InvoiceId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_B2BInvoices_InvoiceId] ON [dbo].[B2BInvoices] ([InvoiceId]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoices_InvoiceType] ON [dbo].[B2BInvoices] ([InvoiceType]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoices_InvoiceYm] ON [dbo].[B2BInvoices] ([InvoiceYm]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoices_TaxId] ON [dbo].[B2BInvoices] ([TaxId]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoices_SiteId] ON [dbo].[B2BInvoices] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoices_Status] ON [dbo].[B2BInvoices] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoices_B2BFlag] ON [dbo].[B2BInvoices] ([B2BFlag]);
END
GO

-- 3. B2B發票傳輸記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[B2BInvoiceTransfers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[B2BInvoiceTransfers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceId] NVARCHAR(50) NOT NULL,
        [TransferType] NVARCHAR(20) NOT NULL,
        [TransferStatus] NVARCHAR(20) NOT NULL,
        [TransferDate] DATETIME2 NULL,
        [TransferMessage] NVARCHAR(1000) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_B2BInvoiceTransfers_B2BInvoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[B2BInvoices] ([InvoiceId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_B2BInvoiceTransfers_InvoiceId] ON [dbo].[B2BInvoiceTransfers] ([InvoiceId]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoiceTransfers_TransferType] ON [dbo].[B2BInvoiceTransfers] ([TransferType]);
    CREATE NONCLUSTERED INDEX [IX_B2BInvoiceTransfers_TransferStatus] ON [dbo].[B2BInvoiceTransfers] ([TransferStatus]);
END
GO

-- 4. B2B電子發票主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[B2BElectronicInvoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[B2BElectronicInvoices] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceId] NVARCHAR(50) NOT NULL,
        [PosId] NVARCHAR(50) NULL,
        [InvYm] NVARCHAR(6) NOT NULL,
        [Track] NVARCHAR(2) NULL,
        [InvNoB] NVARCHAR(10) NULL,
        [InvNoE] NVARCHAR(10) NULL,
        [PrintCode] NVARCHAR(50) NULL,
        [InvoiceDate] DATETIME2 NULL,
        [PrizeType] NVARCHAR(10) NULL,
        [PrizeAmt] DECIMAL(18, 4) NULL,
        [CarrierIdClear] NVARCHAR(50) NULL,
        [AwardPrint] NVARCHAR(10) NULL,
        [AwardPos] NVARCHAR(50) NULL,
        [AwardDate] DATETIME2 NULL,
        [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y',
        [TransferType] NVARCHAR(20) NULL,
        [TransferStatus] NVARCHAR(20) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_B2BElectronicInvoices_InvoiceId] UNIQUE ([InvoiceId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_InvoiceId] ON [dbo].[B2BElectronicInvoices] ([InvoiceId]);
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_InvYm] ON [dbo].[B2BElectronicInvoices] ([InvYm]);
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_Track] ON [dbo].[B2BElectronicInvoices] ([Track]);
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_PrintCode] ON [dbo].[B2BElectronicInvoices] ([PrintCode]);
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_PrizeType] ON [dbo].[B2BElectronicInvoices] ([PrizeType]);
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_B2BFlag] ON [dbo].[B2BElectronicInvoices] ([B2BFlag]);
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_TransferType] ON [dbo].[B2BElectronicInvoices] ([TransferType]);
END
GO

-- 5. B2B電子發票列印設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[B2BElectronicInvoicePrintSettings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[B2BElectronicInvoicePrintSettings] (
        [SettingId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [PrintFormat] NVARCHAR(20) NOT NULL,
        [BarcodeType] NVARCHAR(20) NULL,
        [BarcodeSize] INT NULL DEFAULT 40,
        [BarcodeMargin] INT NULL DEFAULT 5,
        [ColCount] INT NULL DEFAULT 2,
        [PageCount] INT NULL DEFAULT 14,
        [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y',
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoicePrintSettings_B2BFlag] ON [dbo].[B2BElectronicInvoicePrintSettings] ([B2BFlag]);
    CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoicePrintSettings_Status] ON [dbo].[B2BElectronicInvoicePrintSettings] ([Status]);
END
GO

-- 6. B2B銷售單主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[B2BSalesOrders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[B2BSalesOrders] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [OrderId] NVARCHAR(50) NOT NULL,
        [OrderDate] DATETIME2 NOT NULL,
        [OrderType] NVARCHAR(20) NOT NULL,
        [ShopId] NVARCHAR(50) NOT NULL,
        [CustomerId] NVARCHAR(50) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D',
        [ApplyUserId] NVARCHAR(50) NULL,
        [ApplyDate] DATETIME2 NULL,
        [ApproveUserId] NVARCHAR(50) NULL,
        [ApproveDate] DATETIME2 NULL,
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0,
        [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0,
        [Memo] NVARCHAR(500) NULL,
        [ExpectedDate] DATETIME2 NULL,
        [SiteId] NVARCHAR(50) NULL,
        [OrgId] NVARCHAR(50) NULL,
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD',
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1,
        [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y',
        [TransferType] NVARCHAR(20) NULL,
        [TransferStatus] NVARCHAR(20) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_B2BSalesOrders_OrderId] UNIQUE ([OrderId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_OrderId] ON [dbo].[B2BSalesOrders] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_ShopId] ON [dbo].[B2BSalesOrders] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_CustomerId] ON [dbo].[B2BSalesOrders] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_Status] ON [dbo].[B2BSalesOrders] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_OrderDate] ON [dbo].[B2BSalesOrders] ([OrderDate]);
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_B2BFlag] ON [dbo].[B2BSalesOrders] ([B2BFlag]);
END
GO

-- 7. B2B銷售單明細表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[B2BSalesOrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[B2BSalesOrderDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [OrderId] NVARCHAR(50) NOT NULL,
        [LineNum] INT NOT NULL,
        [GoodsId] NVARCHAR(50) NOT NULL,
        [BarcodeId] NVARCHAR(50) NULL,
        [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0,
        [UnitPrice] DECIMAL(18, 4) NULL,
        [Amount] DECIMAL(18, 4) NULL,
        [ShippedQty] DECIMAL(18, 4) NULL DEFAULT 0,
        [ReturnQty] DECIMAL(18, 4) NULL DEFAULT 0,
        [UnitId] NVARCHAR(50) NULL,
        [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0,
        [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0,
        [Memo] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_B2BSalesOrderDetails_B2BSalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[B2BSalesOrders] ([OrderId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrderDetails_OrderId] ON [dbo].[B2BSalesOrderDetails] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_B2BSalesOrderDetails_GoodsId] ON [dbo].[B2BSalesOrderDetails] ([GoodsId]);
END
GO

PRINT '發票銷售管理B2B類資料表建立完成！';
GO


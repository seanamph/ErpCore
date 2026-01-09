-- =============================================
-- 發票銷售管理類 (SYSG000) - 資料庫腳本
-- 功能：發票資料維護 (SYSG110-SYSG190)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 發票格式代號表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceFormats]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InvoiceFormats] (
        [FormatId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [FormatName] NVARCHAR(100) NOT NULL,
        [FormatNameEn] NVARCHAR(100) NULL,
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_InvoiceFormats_Status] ON [dbo].[InvoiceFormats] ([Status]);
END
GO

-- 2. 稅籍資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaxRegistrations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TaxRegistrations] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TaxId] NVARCHAR(20) NOT NULL,
        [CompanyName] NVARCHAR(200) NOT NULL,
        [CompanyNameEn] NVARCHAR(200) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(50) NULL,
        [Zone] NVARCHAR(50) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [Phone] NVARCHAR(50) NULL,
        [Fax] NVARCHAR(50) NULL,
        [Email] NVARCHAR(100) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_TaxRegistrations_TaxId] UNIQUE ([TaxId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TaxRegistrations_TaxId] ON [dbo].[TaxRegistrations] ([TaxId]);
    CREATE NONCLUSTERED INDEX [IX_TaxRegistrations_Status] ON [dbo].[TaxRegistrations] ([Status]);
END
GO

-- 3. 發票主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Invoices] (
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
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [Notes] NVARCHAR(1000) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Invoices_InvoiceId] UNIQUE ([InvoiceId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceId] ON [dbo].[Invoices] ([InvoiceId]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceType] ON [dbo].[Invoices] ([InvoiceType]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceYm] ON [dbo].[Invoices] ([InvoiceYm]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_TaxId] ON [dbo].[Invoices] ([TaxId]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_SiteId] ON [dbo].[Invoices] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_Status] ON [dbo].[Invoices] ([Status]);
END
GO

PRINT '發票銷售管理類資料表建立完成！';
GO


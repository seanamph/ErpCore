-- =============================================
-- 客戶定制JGJN模組類 (SYSCUST_JGJN系列) - 資料庫腳本
-- 功能：客戶定制JGJN模組系列功能
-- 建立日期：2025-01-09
-- =============================================

-- 1. JgjNData - JGJN資料主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[JgjNData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[JgjNData] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
        [ModuleCode] NVARCHAR(20) NOT NULL, -- 模組代碼 (SYS1610, SYS1620, SYS1630, SYS1640, SYS1645, SYS1646, SYSC210等)
        [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
        [DataValue] NVARCHAR(MAX) NULL, -- 資料值
        [DataType] NVARCHAR(20) NULL, -- 資料類型
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SortOrder] INT NULL DEFAULT 0, -- 排序順序
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_JgjNData_DataId_ModuleCode] UNIQUE ([DataId], [ModuleCode])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_JgjNData_ModuleCode] ON [dbo].[JgjNData] ([ModuleCode]);
    CREATE NONCLUSTERED INDEX [IX_JgjNData_DataType] ON [dbo].[JgjNData] ([DataType]);
    CREATE NONCLUSTERED INDEX [IX_JgjNData_Status] ON [dbo].[JgjNData] ([Status]);
END
GO

-- 2. JgjNCustomer - JGJN客戶資料主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[JgjNCustomer]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[JgjNCustomer] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶代碼
        [CustomerName] NVARCHAR(100) NOT NULL, -- 客戶名稱
        [CustomerType] NVARCHAR(20) NULL, -- 客戶類型
        [ContactPerson] NVARCHAR(50) NULL, -- 聯絡人
        [Phone] NVARCHAR(50) NULL, -- 電話
        [Email] NVARCHAR(100) NULL, -- 電子郵件
        [Address] NVARCHAR(200) NULL, -- 地址
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_JgjNCustomer_CustomerId] UNIQUE ([CustomerId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_JgjNCustomer_CustomerName] ON [dbo].[JgjNCustomer] ([CustomerName]);
    CREATE NONCLUSTERED INDEX [IX_JgjNCustomer_Status] ON [dbo].[JgjNCustomer] ([Status]);
END
GO

-- 3. JgjNInvoice - JGJN發票資料主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[JgjNInvoice]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[JgjNInvoice] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceId] NVARCHAR(50) NOT NULL, -- 發票代碼
        [InvoiceNo] NVARCHAR(50) NULL, -- 發票號碼
        [InvoiceDate] DATETIME2 NULL, -- 發票日期
        [CustomerId] NVARCHAR(50) NULL, -- 客戶代碼
        [Amount] DECIMAL(18,2) NULL, -- 金額
        [Currency] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (PENDING:待處理, PROCESSED:已處理, FAILED:失敗)
        [PrintStatus] NVARCHAR(20) NULL, -- 列印狀態
        [PrintDate] DATETIME2 NULL, -- 列印日期
        [FilePath] NVARCHAR(500) NULL, -- 檔案路徑
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_JgjNInvoice_InvoiceId] UNIQUE ([InvoiceId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_JgjNInvoice_CustomerId] ON [dbo].[JgjNInvoice] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_JgjNInvoice_InvoiceDate] ON [dbo].[JgjNInvoice] ([InvoiceDate]);
    CREATE NONCLUSTERED INDEX [IX_JgjNInvoice_Status] ON [dbo].[JgjNInvoice] ([Status]);
END
GO


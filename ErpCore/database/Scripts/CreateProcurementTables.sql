-- =============================================
-- 採購供應商管理類資料表建立腳本
-- 功能代碼: SYSP000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 供應商主檔 (Suppliers)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Suppliers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Suppliers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPPLIER_ID)
        [SupplierName] NVARCHAR(200) NOT NULL, -- 供應商名稱 (SUPPLIER_NAME)
        [SupplierNameE] NVARCHAR(200) NULL, -- 供應商英文名稱
        [ContactPerson] NVARCHAR(100) NULL, -- 聯絡人 (CONTACT_PERSON)
        [Phone] NVARCHAR(20) NULL, -- 電話 (PHONE)
        [Fax] NVARCHAR(20) NULL, -- 傳真 (FAX)
        [Email] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
        [Address] NVARCHAR(500) NULL, -- 地址 (ADDRESS)
        [PaymentTerms] NVARCHAR(50) NULL, -- 付款條件 (PAYMENT_TERMS)
        [TaxId] NVARCHAR(20) NULL, -- 統一編號 (TAX_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [Rating] NVARCHAR(10) NULL, -- 評等 (RATING)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Suppliers_SupplierId] UNIQUE ([SupplierId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Suppliers_SupplierId] ON [dbo].[Suppliers] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_Suppliers_Status] ON [dbo].[Suppliers] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Suppliers_Rating] ON [dbo].[Suppliers] ([Rating]);

    PRINT '資料表 Suppliers 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Suppliers 已存在';
END
GO

-- 2. 付款單主檔 (Payments)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payments] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PaymentId] NVARCHAR(50) NOT NULL, -- 付款單號 (PAYMENT_ID)
        [PaymentDate] DATETIME2 NOT NULL, -- 付款日期 (PAYMENT_DATE)
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPPLIER_ID)
        [PaymentType] NVARCHAR(20) NOT NULL, -- 付款類型 (PAYMENT_TYPE)
        [Amount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 付款金額 (AMOUNT)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [BankAccountId] NVARCHAR(50) NULL, -- 銀行帳戶編號 (BANK_ACCOUNT_ID)
        [CheckNumber] NVARCHAR(50) NULL, -- 支票號碼 (CHECK_NUMBER)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, A:已確認, C:已取消)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Payments_PaymentId] UNIQUE ([PaymentId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Payments_PaymentId] ON [dbo].[Payments] ([PaymentId]);
    CREATE NONCLUSTERED INDEX [IX_Payments_SupplierId] ON [dbo].[Payments] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_Payments_PaymentDate] ON [dbo].[Payments] ([PaymentDate]);
    CREATE NONCLUSTERED INDEX [IX_Payments_Status] ON [dbo].[Payments] ([Status]);

    PRINT '資料表 Payments 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Payments 已存在';
END
GO

-- 3. 銀行帳戶主檔 (BankAccounts)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BankAccounts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [BankAccountId] NVARCHAR(50) NOT NULL, -- 銀行帳戶編號 (BANK_ACCOUNT_ID)
        [BankId] NVARCHAR(50) NOT NULL, -- 銀行代碼 (BANK_ID)
        [AccountName] NVARCHAR(200) NOT NULL, -- 帳戶名稱 (ACCOUNT_NAME)
        [AccountNumber] NVARCHAR(50) NOT NULL, -- 帳號 (ACCOUNT_NUMBER)
        [AccountType] NVARCHAR(20) NULL, -- 帳戶類型 (ACCOUNT_TYPE)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_BankAccounts_BankAccountId] UNIQUE ([BankAccountId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_BankAccountId] ON [dbo].[BankAccounts] ([BankAccountId]);
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_BankId] ON [dbo].[BankAccounts] ([BankId]);
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_Status] ON [dbo].[BankAccounts] ([Status]);

    PRINT '資料表 BankAccounts 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 BankAccounts 已存在';
END
GO

-- 4. 採購其他功能主檔 (PurchaseOtherFunctions)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOtherFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseOtherFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼
        [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱
        [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (TOOL:工具, AUX:輔助, PROCESS:處理)
        [FunctionDesc] NVARCHAR(500) NULL, -- 功能說明
        [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能配置 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_PurchaseOtherFunctions_FunctionId] UNIQUE ([FunctionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_FunctionId] ON [dbo].[PurchaseOtherFunctions] ([FunctionId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_FunctionType] ON [dbo].[PurchaseOtherFunctions] ([FunctionType]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_Status] ON [dbo].[PurchaseOtherFunctions] ([Status]);

    PRINT '資料表 PurchaseOtherFunctions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseOtherFunctions 已存在';
END
GO


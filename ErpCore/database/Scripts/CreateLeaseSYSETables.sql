-- =============================================
-- 租賃管理SYSE類資料表建立腳本
-- 功能代碼: SYSE000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 租賃主檔 (LeaseSYSE) - 與 SYS8000 的 Leases 共用，但增加版本號欄位
-- 注意：如果 Leases 表已存在，需要檢查是否有 Version 欄位，如果沒有則新增

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Leases]') AND name = 'Version')
BEGIN
    ALTER TABLE [dbo].[Leases] ADD [Version] NVARCHAR(10) NOT NULL DEFAULT '1';
    PRINT '已新增 Version 欄位至 Leases 資料表';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Leases]') AND name = 'LeaseType')
BEGIN
    ALTER TABLE [dbo].[Leases] ADD [LeaseType] NVARCHAR(20) NULL;
    PRINT '已新增 LeaseType 欄位至 Leases 資料表';
END
GO

-- 2. 租賃條件 (LeaseTerms)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseTerms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseTerms] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號
        [Version] NVARCHAR(10) NOT NULL, -- 版本號
        [TermType] NVARCHAR(20) NOT NULL, -- 條件類型 (TERM_TYPE)
        [TermName] NVARCHAR(200) NULL, -- 條件名稱 (TERM_NAME)
        [TermValue] NVARCHAR(500) NULL, -- 條件值 (TERM_VALUE)
        [TermAmount] DECIMAL(18, 4) NULL, -- 條件金額 (TERM_AMOUNT)
        [TermDate] DATETIME2 NULL, -- 條件日期 (TERM_DATE)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseTerms_LeaseId] ON [dbo].[LeaseTerms] ([LeaseId], [Version]);
    CREATE NONCLUSTERED INDEX [IX_LeaseTerms_TermType] ON [dbo].[LeaseTerms] ([TermType]);

    PRINT '資料表 LeaseTerms 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseTerms 已存在';
END
GO

-- 3. 租賃會計分類 (LeaseAccountingCategories)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseAccountingCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseAccountingCategories] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號
        [Version] NVARCHAR(10) NOT NULL, -- 版本號
        [CategoryId] NVARCHAR(50) NOT NULL, -- 會計分類代碼 (CATEGORY_ID)
        [CategoryName] NVARCHAR(200) NULL, -- 會計分類名稱 (CATEGORY_NAME)
        [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseAccountingCategories_LeaseId] ON [dbo].[LeaseAccountingCategories] ([LeaseId], [Version]);
    CREATE NONCLUSTERED INDEX [IX_LeaseAccountingCategories_CategoryId] ON [dbo].[LeaseAccountingCategories] ([CategoryId]);

    PRINT '資料表 LeaseAccountingCategories 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseAccountingCategories 已存在';
END
GO

-- 4. 租賃擴展主檔 (LeaseExtensions)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseExtensions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseExtensions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號 (EXTENSION_ID)
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
        [ExtensionType] NVARCHAR(20) NOT NULL, -- 擴展類型 (EXTENSION_TYPE, CONDITION:特殊條件, TERM:附加條款, SETTING:擴展設定)
        [ExtensionName] NVARCHAR(200) NULL, -- 擴展名稱 (EXTENSION_NAME)
        [ExtensionValue] NVARCHAR(MAX) NULL, -- 擴展值 (EXTENSION_VALUE)
        [StartDate] DATETIME2 NULL, -- 開始日期 (START_DATE)
        [EndDate] DATETIME2 NULL, -- 結束日期 (END_DATE)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LeaseExtensions_ExtensionId] UNIQUE ([ExtensionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionId] ON [dbo].[LeaseExtensions] ([ExtensionId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_LeaseId] ON [dbo].[LeaseExtensions] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionType] ON [dbo].[LeaseExtensions] ([ExtensionType]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_Status] ON [dbo].[LeaseExtensions] ([Status]);

    PRINT '資料表 LeaseExtensions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseExtensions 已存在';
END
GO

-- 5. 租賃擴展明細 (LeaseExtensionDetails)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseExtensionDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseExtensionDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [FieldName] NVARCHAR(100) NULL, -- 欄位名稱 (FIELD_NAME)
        [FieldValue] NVARCHAR(MAX) NULL, -- 欄位值 (FIELD_VALUE)
        [FieldType] NVARCHAR(20) NULL, -- 欄位類型 (FIELD_TYPE, TEXT:文字, NUMBER:數字, DATE:日期, BOOLEAN:布林)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensionDetails_ExtensionId] ON [dbo].[LeaseExtensionDetails] ([ExtensionId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensionDetails_LineNum] ON [dbo].[LeaseExtensionDetails] ([ExtensionId], [LineNum]);

    PRINT '資料表 LeaseExtensionDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseExtensionDetails 已存在';
END
GO

-- 6. 費用主檔 (LeaseFees)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseFees]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseFees] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [FeeId] NVARCHAR(50) NOT NULL, -- 費用編號 (FEE_ID)
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
        [FeeType] NVARCHAR(20) NOT NULL, -- 費用類型 (FEE_TYPE, RENT:租金, MANAGEMENT:管理費, UTILITY:水電費, OTHER:其他費用)
        [FeeItemId] NVARCHAR(50) NULL, -- 費用項目編號 (FEE_ITEM_ID)
        [FeeItemName] NVARCHAR(200) NULL, -- 費用項目名稱 (FEE_ITEM_NAME)
        [FeeAmount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 費用金額 (FEE_AMOUNT)
        [FeeDate] DATETIME2 NOT NULL, -- 費用日期 (FEE_DATE)
        [DueDate] DATETIME2 NULL, -- 到期日期 (DUE_DATE)
        [PaidDate] DATETIME2 NULL, -- 繳費日期 (PAID_DATE)
        [PaidAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 已繳金額 (PAID_AMOUNT)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:待繳, P:部分繳, F:已繳, C:已取消)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率 (TAX_RATE)
        [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMOUNT)
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMOUNT, 含稅)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LeaseFees_FeeId] UNIQUE ([FeeId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseFees_FeeId] ON [dbo].[LeaseFees] ([FeeId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseFees_LeaseId] ON [dbo].[LeaseFees] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseFees_FeeType] ON [dbo].[LeaseFees] ([FeeType]);
    CREATE NONCLUSTERED INDEX [IX_LeaseFees_Status] ON [dbo].[LeaseFees] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_LeaseFees_FeeDate] ON [dbo].[LeaseFees] ([FeeDate]);

    PRINT '資料表 LeaseFees 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseFees 已存在';
END
GO

-- 7. 費用項目主檔 (LeaseFeeItems)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseFeeItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseFeeItems] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [FeeItemId] NVARCHAR(50) NOT NULL, -- 費用項目編號 (FEE_ITEM_ID)
        [FeeItemName] NVARCHAR(200) NOT NULL, -- 費用項目名稱 (FEE_ITEM_NAME)
        [FeeType] NVARCHAR(20) NOT NULL, -- 費用類型 (FEE_TYPE)
        [DefaultAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 預設金額 (DEFAULT_AMOUNT)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LeaseFeeItems_FeeItemId] UNIQUE ([FeeItemId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseFeeItems_FeeItemId] ON [dbo].[LeaseFeeItems] ([FeeItemId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseFeeItems_FeeType] ON [dbo].[LeaseFeeItems] ([FeeType]);
    CREATE NONCLUSTERED INDEX [IX_LeaseFeeItems_Status] ON [dbo].[LeaseFeeItems] ([Status]);

    PRINT '資料表 LeaseFeeItems 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseFeeItems 已存在';
END
GO


-- 會計財務管理類資料表 (SYSN000)
-- 包含：會計科目維護、會計憑證管理、會計帳簿管理、財務交易、資產管理、財務報表等

-- ============================================
-- 1. 會計科目維護 - 會計科目資料表 (AccountSubjects)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountSubjects]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AccountSubjects] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [StypeId] NVARCHAR(50) NOT NULL, -- 科目代號 (STYPE_ID)
        [StypeName] NVARCHAR(200) NOT NULL, -- 科目名稱 (STYPE_NAME)
        [StypeNameE] NVARCHAR(200) NULL, -- 科目英文名稱 (STYPE_NAME_E)
        [Dc] NVARCHAR(1) NULL, -- 借/貸 (DC, D:借方, C:貸方)
        [LedgerMd] NVARCHAR(1) NULL, -- 統制/明細 (LEDGER_MD, L:統制, M:明細)
        [MtypeId] NVARCHAR(10) NULL, -- 三碼代號 (MTYPE_ID)
        [AbatYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否為沖帳代號 (ABAT_YN, Y/N)
        [VoucherType] NVARCHAR(10) NULL, -- 傳票格式 (VOUCHER_TYPE)
        [BudgetYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否為預算科目 (BUDGET_YN, Y/N)
        [OrgYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否設定部門代號 (ORG_YN, Y/N)
        [ExpYear] DECIMAL(18, 2) NULL, -- 折舊攤提年限 (EXP_YEAR)
        [ResiValue] DECIMAL(18, 2) NULL, -- 殘值年限 (RESI_VALUE)
        [DepreLid] NVARCHAR(50) NULL, -- 折舊會計科目 (DEPRE_LID)
        [AccudepreLid] NVARCHAR(50) NULL, -- 累計折舊會計科目 (ACCUDEPRE_LID)
        [StypeYn] NVARCHAR(1) NULL DEFAULT 'Y', -- 是否可輸 (STYPE_YN, Y/N)
        [IfrsStypeId] NVARCHAR(50) NULL, -- IFRS會計科目 (IFRS_STYPE_ID)
        [RocStypeId] NVARCHAR(50) NULL, -- 集團會計科目 (ROC_STYPE_ID)
        [SapStypeId] NVARCHAR(50) NULL, -- SAP會計科目 (SAP_STYPE_ID)
        [StypeClass] NVARCHAR(50) NULL, -- 科目別 (STYPE_CLASS)
        [StypeOrder] INT NULL, -- 排序 (STYPE_ORDER)
        [Amt0] DECIMAL(18, 2) NULL DEFAULT 0, -- 期初餘額 (AMT_0)
        [Amt1] DECIMAL(18, 2) NULL DEFAULT 0, -- 期末餘額 (AMT_1)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_AccountSubjects] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_AccountSubjects_StypeId] UNIQUE ([StypeId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_AccountSubjects_StypeId] ON [dbo].[AccountSubjects] ([StypeId]);
    CREATE NONCLUSTERED INDEX [IX_AccountSubjects_Dc] ON [dbo].[AccountSubjects] ([Dc]);
    CREATE NONCLUSTERED INDEX [IX_AccountSubjects_LedgerMd] ON [dbo].[AccountSubjects] ([LedgerMd]);
    CREATE NONCLUSTERED INDEX [IX_AccountSubjects_VoucherType] ON [dbo].[AccountSubjects] ([VoucherType]);
    CREATE NONCLUSTERED INDEX [IX_AccountSubjects_BudgetYn] ON [dbo].[AccountSubjects] ([BudgetYn]);
    CREATE NONCLUSTERED INDEX [IX_AccountSubjects_StypeClass] ON [dbo].[AccountSubjects] ([StypeClass]);
    CREATE NONCLUSTERED INDEX [IX_AccountSubjects_StypeOrder] ON [dbo].[AccountSubjects] ([StypeOrder]);
    
    PRINT 'AccountSubjects 表建立成功';
END
ELSE
BEGIN
    PRINT 'AccountSubjects 表已存在';
END
GO

-- ============================================
-- 2. 會計憑證管理 - 傳票型態資料表 (VoucherTypes)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherTypes] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherTypeId] NVARCHAR(50) NOT NULL,
        [VoucherTypeName] NVARCHAR(200) NOT NULL,
        [VoucherTypeNameE] NVARCHAR(200) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [Description] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedPriority] INT NULL,
        [CreatedGroup] NVARCHAR(50) NULL,
        CONSTRAINT [PK_VoucherTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_VoucherTypes_VoucherTypeId] UNIQUE ([VoucherTypeId])
    );

    CREATE NONCLUSTERED INDEX [IX_VoucherTypes_VoucherTypeId] ON [dbo].[VoucherTypes] ([VoucherTypeId]);
    CREATE NONCLUSTERED INDEX [IX_VoucherTypes_Status] ON [dbo].[VoucherTypes] ([Status]);
    
    PRINT 'VoucherTypes 表建立成功';
END
ELSE
BEGIN
    PRINT 'VoucherTypes 表已存在';
END
GO

-- ============================================
-- 3. 會計憑證管理 - 常用傳票主檔 (CommonVouchers)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommonVouchers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CommonVouchers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CommonVoucherId] NVARCHAR(50) NOT NULL,
        [CommonVoucherName] NVARCHAR(200) NOT NULL,
        [VoucherTypeId] NVARCHAR(50) NULL,
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [SortOrder] INT NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedPriority] INT NULL,
        [CreatedGroup] NVARCHAR(50) NULL,
        CONSTRAINT [PK_CommonVouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_CommonVouchers_CommonVoucherId] UNIQUE ([CommonVoucherId])
    );

    CREATE NONCLUSTERED INDEX [IX_CommonVouchers_CommonVoucherId] ON [dbo].[CommonVouchers] ([CommonVoucherId]);
    CREATE NONCLUSTERED INDEX [IX_CommonVouchers_VoucherTypeId] ON [dbo].[CommonVouchers] ([VoucherTypeId]);
    CREATE NONCLUSTERED INDEX [IX_CommonVouchers_Status] ON [dbo].[CommonVouchers] ([Status]);
    
    PRINT 'CommonVouchers 表建立成功';
END
ELSE
BEGIN
    PRINT 'CommonVouchers 表已存在';
END
GO

-- ============================================
-- 4. 會計憑證管理 - 常用傳票明細 (CommonVoucherDetails)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommonVoucherDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CommonVoucherDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CommonVoucherId] NVARCHAR(50) NOT NULL,
        [SeqNo] INT NOT NULL,
        [StypeId] NVARCHAR(50) NULL,
        [Dc] NVARCHAR(1) NULL,
        [Amount] DECIMAL(18, 2) NULL DEFAULT 0,
        [Description] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_CommonVoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_CommonVoucherDetails_CommonVoucherId] ON [dbo].[CommonVoucherDetails] ([CommonVoucherId]);
    CREATE NONCLUSTERED INDEX [IX_CommonVoucherDetails_StypeId] ON [dbo].[CommonVoucherDetails] ([StypeId]);
    
    PRINT 'CommonVoucherDetails 表建立成功';
END
ELSE
BEGIN
    PRINT 'CommonVoucherDetails 表已存在';
END
GO

-- ============================================
-- 5. 會計憑證管理 - 傳票主檔 (Vouchers)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vouchers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vouchers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherId] NVARCHAR(50) NOT NULL,
        [VoucherDate] DATETIME2 NOT NULL,
        [VoucherTypeId] NVARCHAR(50) NULL,
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D',
        [PostedBy] NVARCHAR(50) NULL,
        [PostedAt] DATETIME2 NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedPriority] INT NULL,
        [CreatedGroup] NVARCHAR(50) NULL,
        CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Vouchers_VoucherId] UNIQUE ([VoucherId])
    );

    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherId] ON [dbo].[Vouchers] ([VoucherId]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherDate] ON [dbo].[Vouchers] ([VoucherDate]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherTypeId] ON [dbo].[Vouchers] ([VoucherTypeId]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_Status] ON [dbo].[Vouchers] ([Status]);
    
    PRINT 'Vouchers 表建立成功';
END
ELSE
BEGIN
    PRINT 'Vouchers 表已存在';
END
GO

-- ============================================
-- 6. 會計憑證管理 - 傳票明細 (VoucherDetails)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherId] NVARCHAR(50) NOT NULL,
        [SeqNo] INT NOT NULL,
        [StypeId] NVARCHAR(50) NULL,
        [Dc] NVARCHAR(1) NOT NULL,
        [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [Description] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_VoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherId] ON [dbo].[VoucherDetails] ([VoucherId]);
    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_StypeId] ON [dbo].[VoucherDetails] ([StypeId]);
    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_Dc] ON [dbo].[VoucherDetails] ([Dc]);
    
    PRINT 'VoucherDetails 表建立成功';
END
ELSE
BEGIN
    PRINT 'VoucherDetails 表已存在';
END
GO

-- ============================================
-- 7. 財務交易 - 財務交易資料表 (FinancialTransactions)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FinancialTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FinancialTransactions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TxnNo] NVARCHAR(50) NOT NULL,
        [TxnDate] DATETIME2 NOT NULL,
        [TxnType] NVARCHAR(20) NOT NULL,
        [StypeId] NVARCHAR(50) NOT NULL,
        [Dc] NVARCHAR(1) NOT NULL,
        [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT',
        [Verifier] NVARCHAR(50) NULL,
        [VerifyDate] DATETIME2 NULL,
        [PostedBy] NVARCHAR(50) NULL,
        [PostedDate] DATETIME2 NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_FinancialTransactions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_FinancialTransactions_TxnNo] UNIQUE ([TxnNo])
    );

    CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_TxnNo] ON [dbo].[FinancialTransactions] ([TxnNo]);
    CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_TxnDate] ON [dbo].[FinancialTransactions] ([TxnDate]);
    CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_TxnType] ON [dbo].[FinancialTransactions] ([TxnType]);
    CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_StypeId] ON [dbo].[FinancialTransactions] ([StypeId]);
    CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_Status] ON [dbo].[FinancialTransactions] ([Status]);
    
    PRINT 'FinancialTransactions 表建立成功';
END
ELSE
BEGIN
    PRINT 'FinancialTransactions 表已存在';
END
GO

-- ============================================
-- 8. 資產管理 - 資產資料表 (Assets)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Assets]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Assets] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [AssetId] NVARCHAR(50) NOT NULL,
        [AssetName] NVARCHAR(200) NOT NULL,
        [AssetType] NVARCHAR(50) NULL,
        [AcquisitionDate] DATETIME2 NULL,
        [AcquisitionCost] DECIMAL(18, 2) NULL DEFAULT 0,
        [DepreciationMethod] NVARCHAR(20) NULL,
        [UsefulLife] INT NULL,
        [ResidualValue] DECIMAL(18, 2) NULL DEFAULT 0,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [Location] NVARCHAR(200) NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Assets] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Assets_AssetId] UNIQUE ([AssetId])
    );

    CREATE NONCLUSTERED INDEX [IX_Assets_AssetId] ON [dbo].[Assets] ([AssetId]);
    CREATE NONCLUSTERED INDEX [IX_Assets_AssetType] ON [dbo].[Assets] ([AssetType]);
    CREATE NONCLUSTERED INDEX [IX_Assets_Status] ON [dbo].[Assets] ([Status]);
    
    PRINT 'Assets 表建立成功';
END
ELSE
BEGIN
    PRINT 'Assets 表已存在';
END
GO


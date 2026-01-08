-- =============================================
-- 憑證管理類資料表建立腳本
-- 功能代碼: SYSK000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 憑證主檔 (Vouchers) - SYSK110-SYSK150
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vouchers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vouchers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證編號 (VCH_NO)
        [VoucherDate] DATETIME2 NOT NULL, -- 憑證日期 (VCH_DATE)
        [VoucherType] NVARCHAR(20) NOT NULL, -- 憑證類型 (VCH_TYPE, V:傳票, R:收據, I:發票)
        [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, X:已取消, C:已結案)
        [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
        [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
        [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
        [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
        [TotalDebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方總額 (TOTAL_DEBIT_AMT)
        [TotalCreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方總額 (TOTAL_CREDIT_AMT)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Vouchers_VoucherId] UNIQUE ([VoucherId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherId] ON [dbo].[Vouchers] ([VoucherId]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_ShopId] ON [dbo].[Vouchers] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_Status] ON [dbo].[Vouchers] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherDate] ON [dbo].[Vouchers] ([VoucherDate]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherType] ON [dbo].[Vouchers] ([VoucherType]);

    PRINT '資料表 Vouchers 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Vouchers 已存在';
END
GO

-- 2. 憑證明細 (VoucherDetails) - SYSK110-SYSK150
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證編號
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [AccountId] NVARCHAR(50) NOT NULL, -- 會計科目代碼 (ACCOUNT_ID)
        [DebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方金額 (DEBIT_AMT)
        [CreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方金額 (CREDIT_AMT)
        [Description] NVARCHAR(500) NULL, -- 摘要 (DESCRIPTION)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_VoucherDetails_Vouchers] FOREIGN KEY ([VoucherId]) REFERENCES [dbo].[Vouchers] ([VoucherId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherId] ON [dbo].[VoucherDetails] ([VoucherId]);
    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_AccountId] ON [dbo].[VoucherDetails] ([AccountId]);

    PRINT '資料表 VoucherDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 VoucherDetails 已存在';
END
GO

-- 3. 憑證類型設定 (VoucherTypes) - SYSK120
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherTypes] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherTypeId] NVARCHAR(20) NOT NULL, -- 憑證類型代碼
        [VoucherTypeName] NVARCHAR(100) NOT NULL, -- 憑證類型名稱
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_VoucherTypes_VoucherTypeId] UNIQUE ([VoucherTypeId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_VoucherTypes_VoucherTypeId] ON [dbo].[VoucherTypes] ([VoucherTypeId]);
    CREATE NONCLUSTERED INDEX [IX_VoucherTypes_Status] ON [dbo].[VoucherTypes] ([Status]);

    PRINT '資料表 VoucherTypes 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 VoucherTypes 已存在';
END
GO

-- 4. 憑證處理記錄 (VoucherProcessLogs) - SYSK210-SYSK230
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherProcessLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherProcessLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證編號
        [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (CHECK:檢查, IMPORT:導入, PRINT:列印, EXPORT:匯出)
        [ProcessStatus] NVARCHAR(10) NOT NULL, -- 處理狀態 (SUCCESS:成功, FAILED:失敗)
        [ProcessMessage] NVARCHAR(500) NULL, -- 處理訊息
        [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員
        [ProcessDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 處理時間
        [ProcessData] NVARCHAR(MAX) NULL, -- 處理資料（JSON格式）
        CONSTRAINT [FK_VoucherProcessLogs_Vouchers] FOREIGN KEY ([VoucherId]) REFERENCES [dbo].[Vouchers] ([VoucherId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_VoucherProcessLogs_VoucherId] ON [dbo].[VoucherProcessLogs] ([VoucherId]);
    CREATE NONCLUSTERED INDEX [IX_VoucherProcessLogs_ProcessDate] ON [dbo].[VoucherProcessLogs] ([ProcessDate]);
    CREATE NONCLUSTERED INDEX [IX_VoucherProcessLogs_ProcessType] ON [dbo].[VoucherProcessLogs] ([ProcessType]);

    PRINT '資料表 VoucherProcessLogs 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 VoucherProcessLogs 已存在';
END
GO

-- 5. 憑證報表快取 (VoucherReportCache) - SYSK310-SYSK500
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherReportCache]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherReportCache] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型
        [ReportParams] NVARCHAR(MAX) NULL, -- 報表參數（JSON格式）
        [ReportData] NVARCHAR(MAX) NULL, -- 報表資料（JSON格式）
        [CacheExpireTime] DATETIME2 NOT NULL, -- 快取過期時間
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [UQ_VoucherReportCache_Type_Params] UNIQUE ([ReportType], [ReportParams])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_VoucherReportCache_ReportType] ON [dbo].[VoucherReportCache] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_VoucherReportCache_CacheExpireTime] ON [dbo].[VoucherReportCache] ([CacheExpireTime]);

    PRINT '資料表 VoucherReportCache 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 VoucherReportCache 已存在';
END
GO


-- =============================================
-- 報表管理類資料表建立腳本
-- 功能代碼: SYSR000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 收款項目主檔 (ArItems) - SYSR110-SYSR120
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ArItems] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SiteId] NVARCHAR(50) NOT NULL, -- 分店代號 (SITE_ID)
        [AritemId] NVARCHAR(50) NOT NULL, -- 收款項目代號 (ARITEM_ID)
        [AritemName] NVARCHAR(200) NOT NULL, -- 收款項目名稱 (ARITEM_NAME)
        [StypeId] NVARCHAR(50) NULL, -- 會計科目代號 (STYPE_ID)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [Buser] NVARCHAR(50) NULL, -- 建立人員 (BUSER)
        [Btime] DATETIME2 NULL, -- 建立時間 (BTIME)
        [Cpriority] INT NULL, -- 建立優先權 (CPRIORITY)
        [Cgroup] NVARCHAR(50) NULL, -- 建立群組 (CGROUP)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_ArItems] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_ArItems_SiteId_AritemId] UNIQUE ([SiteId], [AritemId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ArItems_SiteId] ON [dbo].[ArItems] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_ArItems_AritemId] ON [dbo].[ArItems] ([AritemId]);
    CREATE NONCLUSTERED INDEX [IX_ArItems_StypeId] ON [dbo].[ArItems] ([StypeId]);

    PRINT '資料表 ArItems 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ArItems 已存在';
END
GO

-- 2. 應收帳款主檔 (AccountsReceivable) - SYSR210-SYSR240
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountsReceivable]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AccountsReceivable] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherM_TKey] BIGINT NULL, -- 傳票主檔KEY值 (VOUCHERM_T_KEY)
        [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
        [AcctKey] NVARCHAR(100) NULL, -- 對帳KEY值 (ACCT_KEY)
        [ReceiptDate] DATETIME2 NULL, -- 收款日期 (RECEIPT_DATE)
        [ReceiptAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 收款金額 (RECEIPT_AMOUNT)
        [AritemId] NVARCHAR(50) NULL, -- 收款項目代號 (ARITEM_ID)
        [ReceiptNo] NVARCHAR(50) NULL, -- 收款單號 (RECEIPT_NO)
        [VoucherNo] NVARCHAR(50) NULL, -- 傳票單號 (VOUCHER_NO)
        [VoucherStatus] NVARCHAR(10) NULL, -- 傳票狀態 (VOUCHER_STATUS)
        [ShopId] NVARCHAR(50) NULL, -- 分店代碼 (SHOP_ID)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_VoucherM_TKey] ON [dbo].[AccountsReceivable] ([VoucherM_TKey]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ObjectId] ON [dbo].[AccountsReceivable] ([ObjectId]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_AcctKey] ON [dbo].[AccountsReceivable] ([AcctKey]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ReceiptDate] ON [dbo].[AccountsReceivable] ([ReceiptDate]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ReceiptNo] ON [dbo].[AccountsReceivable] ([ReceiptNo]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_VoucherNo] ON [dbo].[AccountsReceivable] ([VoucherNo]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ShopId] ON [dbo].[AccountsReceivable] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_SiteId] ON [dbo].[AccountsReceivable] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_AritemId] ON [dbo].[AccountsReceivable] ([AritemId]);

    PRINT '資料表 AccountsReceivable 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 AccountsReceivable 已存在';
END
GO

-- 3. 收款沖帳傳票主檔 (ReceiptVoucherTransfer) - SYSR310-SYSR450
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReceiptVoucherTransfer]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReceiptVoucherTransfer] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReceiptNo] NVARCHAR(50) NOT NULL, -- 收款單號 (RECEIPT_NO)
        [ReceiptDate] DATETIME2 NOT NULL, -- 收款日期 (RECEIPT_DATE)
        [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
        [AcctKey] NVARCHAR(100) NULL, -- 對帳KEY值 (ACCT_KEY)
        [ReceiptAmount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 收款金額 (RECEIPT_AMOUNT)
        [AritemId] NVARCHAR(50) NULL, -- 收款項目代號 (ARITEM_ID)
        [VoucherNo] NVARCHAR(50) NULL, -- 傳票單號 (VOUCHER_NO)
        [VoucherM_TKey] BIGINT NULL, -- 傳票主檔KEY值 (VOUCHERM_T_KEY)
        [TransferStatus] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 拋轉狀態 (TRANSFER_STATUS, P:待拋轉, S:已拋轉, F:失敗)
        [TransferDate] DATETIME2 NULL, -- 拋轉日期 (TRANSFER_DATE)
        [TransferUser] NVARCHAR(50) NULL, -- 拋轉人員 (TRANSFER_USER)
        [ErrorMessage] NVARCHAR(500) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
        [ShopId] NVARCHAR(50) NULL, -- 分店代碼 (SHOP_ID)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_ReceiptVoucherTransfer] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_ReceiptNo] ON [dbo].[ReceiptVoucherTransfer] ([ReceiptNo]);
    CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_ReceiptDate] ON [dbo].[ReceiptVoucherTransfer] ([ReceiptDate]);
    CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_TransferStatus] ON [dbo].[ReceiptVoucherTransfer] ([TransferStatus]);
    CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_VoucherNo] ON [dbo].[ReceiptVoucherTransfer] ([VoucherNo]);
    CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_ObjectId] ON [dbo].[ReceiptVoucherTransfer] ([ObjectId]);
    CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_ShopId] ON [dbo].[ReceiptVoucherTransfer] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_SiteId] ON [dbo].[ReceiptVoucherTransfer] ([SiteId]);

    PRINT '資料表 ReceiptVoucherTransfer 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ReceiptVoucherTransfer 已存在';
END
GO

-- 4. 保證金主檔 (Deposits) - SYSR510-SYSR570
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Deposits]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Deposits] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DepositNo] NVARCHAR(50) NOT NULL, -- 保證金單號 (DEPOSIT_NO)
        [DepositDate] DATETIME2 NOT NULL, -- 保證金日期 (DEPOSIT_DATE)
        [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
        [DepositAmount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 保證金金額 (DEPOSIT_AMOUNT)
        [DepositType] NVARCHAR(20) NULL, -- 保證金類型 (DEPOSIT_TYPE)
        [DepositStatus] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 保證金狀態 (DEPOSIT_STATUS, A:有效, R:退還, C:取消)
        [ReturnDate] DATETIME2 NULL, -- 退還日期 (RETURN_DATE)
        [ReturnAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 退還金額 (RETURN_AMOUNT)
        [VoucherNo] NVARCHAR(50) NULL, -- 傳票單號 (VOUCHER_NO)
        [VoucherM_TKey] BIGINT NULL, -- 傳票主檔KEY值 (VOUCHERM_T_KEY)
        [VoucherStatus] NVARCHAR(10) NULL, -- 傳票狀態 (VOUCHER_STATUS)
        [CheckDueDate] DATETIME2 NULL, -- 票據到期日 (CHECK_DUE_DATE)
        [ShopId] NVARCHAR(50) NULL, -- 分店代碼 (SHOP_ID)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Deposits] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Deposits_DepositNo] UNIQUE ([DepositNo])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Deposits_DepositNo] ON [dbo].[Deposits] ([DepositNo]);
    CREATE NONCLUSTERED INDEX [IX_Deposits_DepositDate] ON [dbo].[Deposits] ([DepositDate]);
    CREATE NONCLUSTERED INDEX [IX_Deposits_ObjectId] ON [dbo].[Deposits] ([ObjectId]);
    CREATE NONCLUSTERED INDEX [IX_Deposits_DepositStatus] ON [dbo].[Deposits] ([DepositStatus]);
    CREATE NONCLUSTERED INDEX [IX_Deposits_VoucherNo] ON [dbo].[Deposits] ([VoucherNo]);
    CREATE NONCLUSTERED INDEX [IX_Deposits_ShopId] ON [dbo].[Deposits] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_Deposits_SiteId] ON [dbo].[Deposits] ([SiteId]);

    PRINT '資料表 Deposits 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Deposits 已存在';
END
GO


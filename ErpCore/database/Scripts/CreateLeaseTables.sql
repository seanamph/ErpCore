-- =============================================
-- 租賃管理類資料表建立腳本
-- 功能代碼: SYS8000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 租賃主檔 (Leases)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Leases]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Leases] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
        [TenantId] NVARCHAR(50) NOT NULL, -- 租戶代碼 (TENANT_ID)
        [TenantName] NVARCHAR(200) NULL, -- 租戶名稱 (TENANT_NAME)
        [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
        [FloorId] NVARCHAR(50) NULL, -- 樓層代碼 (FLOOR_ID)
        [LocationId] NVARCHAR(50) NULL, -- 位置代碼 (LOCATION_ID)
        [LeaseDate] DATETIME2 NOT NULL, -- 租賃日期 (LEASE_DATE)
        [StartDate] DATETIME2 NOT NULL, -- 租期開始日 (START_DATE)
        [EndDate] DATETIME2 NULL, -- 租期結束日 (END_DATE)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已簽約, E:已生效, T:已終止)
        [MonthlyRent] DECIMAL(18, 4) NULL DEFAULT 0, -- 月租金 (MONTHLY_RENT)
        [TotalRent] DECIMAL(18, 4) NULL DEFAULT 0, -- 總租金 (TOTAL_RENT)
        [Deposit] DECIMAL(18, 4) NULL DEFAULT 0, -- 押金 (DEPOSIT)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [PaymentMethod] NVARCHAR(20) NULL, -- 付款方式 (PAYMENT_METHOD)
        [PaymentDay] INT NULL, -- 付款日 (PAYMENT_DAY)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Leases_LeaseId] UNIQUE ([LeaseId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Leases_LeaseId] ON [dbo].[Leases] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_Leases_TenantId] ON [dbo].[Leases] ([TenantId]);
    CREATE NONCLUSTERED INDEX [IX_Leases_ShopId] ON [dbo].[Leases] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_Leases_Status] ON [dbo].[Leases] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Leases_StartDate] ON [dbo].[Leases] ([StartDate]);
    CREATE NONCLUSTERED INDEX [IX_Leases_EndDate] ON [dbo].[Leases] ([EndDate]);

    PRINT '資料表 Leases 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Leases 已存在';
END
GO

-- 2. 租賃明細 (LeaseDetails)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [ItemType] NVARCHAR(20) NULL, -- 項目類型 (ITEM_TYPE, RENT:租金, UTILITY:水電費, OTHER:其他)
        [ItemName] NVARCHAR(200) NULL, -- 項目名稱 (ITEM_NAME)
        [Amount] DECIMAL(18, 4) NULL DEFAULT 0, -- 金額 (AMOUNT)
        [StartDate] DATETIME2 NULL, -- 開始日期
        [EndDate] DATETIME2 NULL, -- 結束日期
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_LeaseDetails_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseDetails_LeaseId] ON [dbo].[LeaseDetails] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseDetails_LineNum] ON [dbo].[LeaseDetails] ([LeaseId], [LineNum]);

    PRINT '資料表 LeaseDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseDetails 已存在';
END
GO

-- 3. 租賃付款記錄 (LeasePayments)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeasePayments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeasePayments] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號
        [PaymentDate] DATETIME2 NOT NULL, -- 付款日期 (PAYMENT_DATE)
        [PaymentAmount] DECIMAL(18, 4) NOT NULL, -- 付款金額 (PAYMENT_AMOUNT)
        [PaymentMethod] NVARCHAR(20) NULL, -- 付款方式 (PAYMENT_METHOD)
        [PaymentStatus] NVARCHAR(10) NULL DEFAULT 'P', -- 付款狀態 (PAYMENT_STATUS, P:已付款, U:未付款, O:逾期)
        [InvoiceNo] NVARCHAR(50) NULL, -- 發票號碼 (INVOICE_NO)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_LeasePayments_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeasePayments_LeaseId] ON [dbo].[LeasePayments] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_LeasePayments_PaymentDate] ON [dbo].[LeasePayments] ([PaymentDate]);
    CREATE NONCLUSTERED INDEX [IX_LeasePayments_PaymentStatus] ON [dbo].[LeasePayments] ([PaymentStatus]);

    PRINT '資料表 LeasePayments 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeasePayments 已存在';
END
GO


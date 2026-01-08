-- =============================================
-- 銷售管理類資料表建立腳本
-- 功能代碼: SYSD000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 銷售單主檔 (SalesOrders)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesOrders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesOrders] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號 (SO_NO)
        [OrderDate] DATETIME2 NOT NULL, -- 銷售日期 (SO_DATE)
        [OrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (ORDER_TYPE, SO:銷售, RT:退貨)
        [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
        [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶代碼 (CUSTOMER_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, O:已出貨, X:已取消, C:已結案)
        [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
        [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
        [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
        [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
        [ShipDate] DATETIME2 NULL, -- 出貨日期 (SHIP_DATE)
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
        [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量 (TOTAL_QTY)
        [DiscountAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 折扣金額 (DISCOUNT_AMT)
        [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMT)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [ExpectedDate] DATETIME2 NULL, -- 預期交貨日期 (EXPECTED_DATE)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_SalesOrders_OrderId] UNIQUE ([OrderId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderId] ON [dbo].[SalesOrders] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_ShopId] ON [dbo].[SalesOrders] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_CustomerId] ON [dbo].[SalesOrders] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_Status] ON [dbo].[SalesOrders] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderDate] ON [dbo].[SalesOrders] ([OrderDate]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderType] ON [dbo].[SalesOrders] ([OrderType]);

    PRINT '資料表 SalesOrders 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 SalesOrders 已存在';
END
GO

-- 2. 銷售單明細 (SalesOrderDetails)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesOrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesOrderDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
        [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
        [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量 (ORDER_QTY)
        [UnitPrice] DECIMAL(18, 4) NULL, -- 單價 (UNIT_PRICE)
        [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
        [ShippedQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已出貨數量 (SHIPPED_QTY)
        [ReturnQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已退數量 (RETURN_QTY)
        [UnitId] NVARCHAR(50) NULL, -- 單位 (UNIT_ID)
        [DiscountRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 折扣率 (DISCOUNT_RATE)
        [DiscountAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 折扣金額 (DISCOUNT_AMOUNT)
        [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率 (TAX_RATE)
        [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMOUNT)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_SalesOrderDetails_SalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[SalesOrders] ([OrderId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_OrderId] ON [dbo].[SalesOrderDetails] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_GoodsId] ON [dbo].[SalesOrderDetails] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_BarcodeId] ON [dbo].[SalesOrderDetails] ([BarcodeId]);

    PRINT '資料表 SalesOrderDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 SalesOrderDetails 已存在';
END
GO

-- 3. 銷售處理記錄 (SalesProcessLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesProcessLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesProcessLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號
        [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (SHIP:出貨, RETURN:退貨, CANCEL:取消)
        [ProcessStatus] NVARCHAR(10) NOT NULL, -- 處理狀態 (SUCCESS:成功, FAILED:失敗)
        [ProcessMessage] NVARCHAR(500) NULL, -- 處理訊息
        [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員
        [ProcessDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 處理時間
        [ProcessData] NVARCHAR(MAX) NULL, -- 處理資料（JSON格式）
        CONSTRAINT [FK_SalesProcessLogs_SalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[SalesOrders] ([OrderId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SalesProcessLogs_OrderId] ON [dbo].[SalesProcessLogs] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_SalesProcessLogs_ProcessDate] ON [dbo].[SalesProcessLogs] ([ProcessDate]);

    PRINT '資料表 SalesProcessLogs 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 SalesProcessLogs 已存在';
END
GO

-- 4. 銷售報表快取 (SalesReportCache)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesReportCache]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesReportCache] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型
        [ReportParams] NVARCHAR(MAX) NULL, -- 報表參數（JSON格式）
        [ReportData] NVARCHAR(MAX) NULL, -- 報表資料（JSON格式）
        [CacheExpireTime] DATETIME2 NOT NULL, -- 快取過期時間
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [UQ_SalesReportCache_Type_Params] UNIQUE ([ReportType], [ReportParams])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SalesReportCache_ReportType] ON [dbo].[SalesReportCache] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_SalesReportCache_CacheExpireTime] ON [dbo].[SalesReportCache] ([CacheExpireTime]);

    PRINT '資料表 SalesReportCache 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 SalesReportCache 已存在';
END
GO

PRINT '銷售管理類資料表建立完成';


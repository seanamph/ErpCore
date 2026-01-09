-- =============================================
-- 發票銷售管理類 (SYSG000) - 銷售資料維護資料庫腳本
-- 功能：銷售資料維護 (SYSG410-SYSG460)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 銷售單主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesOrders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesOrders] (
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
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_SalesOrders] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_SalesOrders_OrderId] UNIQUE ([OrderId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderId] ON [dbo].[SalesOrders] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_ShopId] ON [dbo].[SalesOrders] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_CustomerId] ON [dbo].[SalesOrders] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_Status] ON [dbo].[SalesOrders] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderDate] ON [dbo].[SalesOrders] ([OrderDate]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderType] ON [dbo].[SalesOrders] ([OrderType]);
END
GO

-- 2. 銷售單明細表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesOrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesOrderDetails] (
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
        CONSTRAINT [FK_SalesOrderDetails_SalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[SalesOrders] ([OrderId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_OrderId] ON [dbo].[SalesOrderDetails] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_GoodsId] ON [dbo].[SalesOrderDetails] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_BarcodeId] ON [dbo].[SalesOrderDetails] ([BarcodeId]);
END
GO

PRINT '銷售資料維護資料表建立完成！';
GO


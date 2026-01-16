-- =============================================
-- 訂退貨申請作業資料表建立腳本
-- 功能代碼: SYSW315, SYSW316
-- 建立日期: 2024-01-01
-- 說明: 共用資料表，透過 SourceProgram 欄位區分不同功能
-- =============================================

-- 1. 採購單主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseOrders] (
        [OrderId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 採購單號 (PO_NO)
        [OrderDate] DATETIME2 NOT NULL, -- 採購日期 (PO_DATE)
        [OrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (ORDER_TYPE, PO:採購, RT:退貨)
        [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商代碼 (SUPPLIER_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, X:已取消)
        [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
        [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
        [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
        [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
        [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量 (TOTAL_QTY)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [ExpectedDate] DATETIME2 NULL, -- 預期交貨日期 (EXPECTED_DATE)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [SourceProgram] NVARCHAR(20) NULL DEFAULT 'SYSW315', -- 來源程式 (SYSW315/SYSW316/SYSW322)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_ShopId] ON [dbo].[PurchaseOrders] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_SupplierId] ON [dbo].[PurchaseOrders] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_Status] ON [dbo].[PurchaseOrders] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_OrderDate] ON [dbo].[PurchaseOrders] ([OrderDate]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_OrderType] ON [dbo].[PurchaseOrders] ([OrderType]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_SourceProgram] ON [dbo].[PurchaseOrders] ([SourceProgram]);

    PRINT '資料表 PurchaseOrders 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseOrders 已存在';
END
GO

-- 2. 採購單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseOrderDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [OrderId] NVARCHAR(50) NOT NULL, -- 採購單號
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
        [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
        [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量 (ORDER_QTY)
        [UnitPrice] DECIMAL(18, 4) NULL, -- 單價 (UNIT_PRICE)
        [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
        [ReceivedQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已收數量 (RECEIVED_QTY)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_PurchaseOrderDetails_PurchaseOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[PurchaseOrders] ([OrderId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_OrderId] ON [dbo].[PurchaseOrderDetails] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_GoodsId] ON [dbo].[PurchaseOrderDetails] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_BarcodeId] ON [dbo].[PurchaseOrderDetails] ([BarcodeId]);

    PRINT '資料表 PurchaseOrderDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseOrderDetails 已存在';
END
GO


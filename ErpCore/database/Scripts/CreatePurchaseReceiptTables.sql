-- =============================================
-- 採購單驗收作業資料表建立腳本
-- 功能代碼: SYSW324
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 採購驗收單主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseReceipts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseReceipts] (
        [ReceiptId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 驗收單號
        [OrderId] NVARCHAR(50) NOT NULL, -- 採購單號 (關聯至 PurchaseOrders)
        [ReceiptDate] DATETIME2 NOT NULL, -- 驗收日期
        [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商代碼
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (P:待驗收, R:部分驗收, C:已驗收, X:已取消)
        [ReceiptUserId] NVARCHAR(50) NULL, -- 驗收人員
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
        [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量
        [Memo] NVARCHAR(500) NULL, -- 備註
        [IsSettled] BIT NOT NULL DEFAULT 0, -- 是否已日結
        [SettledDate] DATETIME2 NULL, -- 日結日期
        [PurchaseOrderType] NVARCHAR(20) NULL DEFAULT '1', -- 單據類型 (1:採購, 2:退貨)
        [IsSettledAdjustment] BIT NOT NULL DEFAULT 0, -- 是否為已日結調整
        [OriginalReceiptId] NVARCHAR(50) NULL, -- 原始驗收單號（如有）
        [AdjustmentReason] NVARCHAR(500) NULL, -- 調整原因
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_OrderId] ON [dbo].[PurchaseReceipts] ([OrderId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_ShopId] ON [dbo].[PurchaseReceipts] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_Status] ON [dbo].[PurchaseReceipts] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_ReceiptDate] ON [dbo].[PurchaseReceipts] ([ReceiptDate]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_IsSettled] ON [dbo].[PurchaseReceipts] ([IsSettled]);

    PRINT '資料表 PurchaseReceipts 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseReceipts 已存在';
END
GO

-- 2. 採購驗收單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseReceiptDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseReceiptDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ReceiptId] NVARCHAR(50) NOT NULL, -- 驗收單號
        [OrderDetailId] UNIQUEIDENTIFIER NULL, -- 採購單明細ID (關聯至 PurchaseOrderDetails)
        [LineNum] INT NOT NULL, -- 行號
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
        [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
        [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量
        [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗收數量
        [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
        [Amount] DECIMAL(18, 4) NULL, -- 金額
        [OriginalReceiptQty] DECIMAL(18, 4) NULL, -- 原始驗收數量（已日結調整用）
        [AdjustmentQty] DECIMAL(18, 4) NULL, -- 調整數量（已日結調整用）
        [OriginalUnitPrice] DECIMAL(18, 4) NULL, -- 原始單價（已日結調整用）
        [AdjustmentPrice] DECIMAL(18, 4) NULL, -- 調整單價（已日結調整用）
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_PurchaseReceiptDetails_PurchaseReceipts] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[PurchaseReceipts] ([ReceiptId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseReceiptDetails_ReceiptId] ON [dbo].[PurchaseReceiptDetails] ([ReceiptId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReceiptDetails_GoodsId] ON [dbo].[PurchaseReceiptDetails] ([GoodsId]);

    PRINT '資料表 PurchaseReceiptDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseReceiptDetails 已存在';
END
GO


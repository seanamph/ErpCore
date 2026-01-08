-- =============================================
-- 庫存調整作業資料表建立腳本
-- 功能代碼: SYSW490
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 庫存調整單主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryAdjustments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InventoryAdjustments] (
        [AdjustmentId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 調整單號 (SP_NO)
        [AdjustmentDate] DATETIME2 NOT NULL, -- 調整日期 (SP_DATE)
        [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (D:草稿, C:已確認, X:已取消)
        [AdjustmentType] NVARCHAR(20) NULL, -- 調整類型 (SP_TYPE)
        [AdjustmentUser] NVARCHAR(50) NULL, -- 調整人員 (SP_USER)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [Memo2] NVARCHAR(500) NULL, -- 備註2 (MEMO2)
        [SourceNo] NVARCHAR(50) NULL, -- 來源單號 (SRC_NO)
        [SourceNum] NVARCHAR(50) NULL, -- 來源序號 (SRC_NUM)
        [SourceCheckDate] DATETIME2 NULL, -- 來源檢查日期 (SRC_CHECK_DATE)
        [SourceSuppId] NVARCHAR(50) NULL, -- 來源供應商 (SRC_SUPP_ID)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總調整數量
        [TotalCost] DECIMAL(18, 4) NULL DEFAULT 0, -- 總調整成本
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總調整金額
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_ShopId] ON [dbo].[InventoryAdjustments] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_Status] ON [dbo].[InventoryAdjustments] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_AdjustmentDate] ON [dbo].[InventoryAdjustments] ([AdjustmentDate]);
    CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_SourceNo] ON [dbo].[InventoryAdjustments] ([SourceNo]);

    PRINT '資料表 InventoryAdjustments 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 InventoryAdjustments 已存在';
END
GO

-- 2. 庫存調整單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryAdjustmentDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InventoryAdjustmentDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [AdjustmentId] NVARCHAR(50) NOT NULL, -- 調整單號
        [LineNum] INT NOT NULL, -- 行號
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
        [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
        [AdjustmentQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調整數量 (DIFF_QTY)
        [BeforeQty] DECIMAL(18, 4) NULL, -- 調整前數量
        [AfterQty] DECIMAL(18, 4) NULL, -- 調整後數量
        [UnitCost] DECIMAL(18, 4) NULL, -- 單位成本 (G_COST)
        [AdjustmentCost] DECIMAL(18, 4) NULL, -- 調整成本 (DIFF_COST)
        [AdjustmentAmount] DECIMAL(18, 4) NULL, -- 調整金額 (DIFF_AMT)
        [Reason] NVARCHAR(200) NULL, -- 調整原因
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_InventoryAdjustmentDetails_InventoryAdjustments] FOREIGN KEY ([AdjustmentId]) REFERENCES [dbo].[InventoryAdjustments] ([AdjustmentId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_InventoryAdjustmentDetails_AdjustmentId] ON [dbo].[InventoryAdjustmentDetails] ([AdjustmentId]);
    CREATE NONCLUSTERED INDEX [IX_InventoryAdjustmentDetails_GoodsId] ON [dbo].[InventoryAdjustmentDetails] ([GoodsId]);

    PRINT '資料表 InventoryAdjustmentDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 InventoryAdjustmentDetails 已存在';
END
GO

-- 3. 調整原因主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdjustmentReasons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AdjustmentReasons] (
        [ReasonId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [ReasonName] NVARCHAR(100) NOT NULL,
        [ReasonType] NVARCHAR(20) NULL, -- 增加/減少
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    PRINT '資料表 AdjustmentReasons 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 AdjustmentReasons 已存在';
END
GO

PRINT '庫存調整作業資料表建立完成';
GO


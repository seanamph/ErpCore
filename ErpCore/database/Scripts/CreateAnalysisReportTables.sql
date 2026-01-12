-- =============================================
-- 進銷存分析報表資料表建立腳本
-- 功能代碼: SYSA1000, SYSA1011-SYSA1024
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 庫存資料表 (對應舊系統 IMS_AM.NAM_AM_STOCKS)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryStocks]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InventoryStocks] (
        [StockId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品代碼
        [SourceId] NVARCHAR(50) NULL, -- 來源單號
        [StocksDate] DATETIME2 NOT NULL, -- 庫存日期
        [StocksStatus] NVARCHAR(10) NOT NULL, -- 庫存狀態 (1:入庫, 2:出庫, 3:退貨, 4:退回, 5:報廢, 6:出售, 8:盤點)
        [Qty] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 數量
        [McAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 單價
        [StocksNtaxAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 未稅金額
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_InventoryStocks_SiteId] ON [dbo].[InventoryStocks] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_InventoryStocks_GoodsId] ON [dbo].[InventoryStocks] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_InventoryStocks_StocksDate] ON [dbo].[InventoryStocks] ([StocksDate]);
    CREATE NONCLUSTERED INDEX [IX_InventoryStocks_StocksStatus] ON [dbo].[InventoryStocks] ([StocksStatus]);
    CREATE NONCLUSTERED INDEX [IX_InventoryStocks_SourceId] ON [dbo].[InventoryStocks] ([SourceId]);

    PRINT '資料表 InventoryStocks 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 InventoryStocks 已存在';
END
GO

-- 2. 庫存成本表 (對應舊系統 IMS_AM.NAM_COST)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryCost]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InventoryCost] (
        [CostId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SiteId] NVARCHAR(50) NOT NULL,
        [GoodsId] NVARCHAR(50) NOT NULL,
        [CostYm] NVARCHAR(6) NOT NULL, -- 成本年月 (YYYYMM)
        [LastQty] DECIMAL(18, 2) NULL DEFAULT 0, -- 上期數量
        [LastPrice] DECIMAL(18, 2) NULL DEFAULT 0, -- 上期單價
        [LastAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 上期金額
        [NextQty] DECIMAL(18, 2) NULL DEFAULT 0, -- 下期數量
        [NextPrice] DECIMAL(18, 2) NULL DEFAULT 0, -- 下期單價
        [NextAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 下期金額
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_InventoryCost_Site_Goods_Ym] UNIQUE ([SiteId], [GoodsId], [CostYm])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_InventoryCost_SiteId] ON [dbo].[InventoryCost] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_InventoryCost_GoodsId] ON [dbo].[InventoryCost] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_InventoryCost_CostYm] ON [dbo].[InventoryCost] ([CostYm]);

    PRINT '資料表 InventoryCost 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 InventoryCost 已存在';
END
GO

-- 3. 驗收單主檔 (對應舊系統 IMS_AM.NAM_ACCEPTANCEM)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AcceptanceM]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AcceptanceM] (
        [TxnNo] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [SiteId] NVARCHAR(50) NOT NULL,
        [OrgId] NVARCHAR(50) NULL, -- 單位代碼
        [SupplierId] NVARCHAR(50) NULL, -- 供應商代碼
        [SupplierName] NVARCHAR(200) NULL, -- 供應商名稱
        [AcceptanceDate] DATETIME2 NULL, -- 驗收日期
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_AcceptanceM_SiteId] ON [dbo].[AcceptanceM] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_AcceptanceM_AcceptanceDate] ON [dbo].[AcceptanceM] ([AcceptanceDate]);

    PRINT '資料表 AcceptanceM 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 AcceptanceM 已存在';
END
GO

-- 4. 驗收單明細 (對應舊系統 IMS_AM.NAM_ACCEPTANCED)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AcceptanceD]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AcceptanceD] (
        [TxnNo] NVARCHAR(50) NOT NULL,
        [GoodsId] NVARCHAR(50) NOT NULL,
        [Qty] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [Price] DECIMAL(18, 2) NULL DEFAULT 0,
        [Amt] DECIMAL(18, 2) NULL DEFAULT 0,
        CONSTRAINT [PK_AcceptanceD] PRIMARY KEY CLUSTERED ([TxnNo], [GoodsId]),
        CONSTRAINT [FK_AcceptanceD_AcceptanceM] FOREIGN KEY ([TxnNo]) REFERENCES [dbo].[AcceptanceM] ([TxnNo]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_AcceptanceD_GoodsId] ON [dbo].[AcceptanceD] ([GoodsId]);

    PRINT '資料表 AcceptanceD 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 AcceptanceD 已存在';
END
GO

-- 5. 領料單主檔 (對應舊系統 IMS_AM.NAM_REQUISITIONM)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RequisitionM]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RequisitionM] (
        [TxnNo] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [SiteId] NVARCHAR(50) NOT NULL,
        [OrgId] NVARCHAR(50) NULL, -- 單位代碼
        [OrgAllocation] NVARCHAR(200) NULL, -- 單位分配
        [RequisitionDate] DATETIME2 NULL, -- 領料日期
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_RequisitionM_SiteId] ON [dbo].[RequisitionM] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_RequisitionM_RequisitionDate] ON [dbo].[RequisitionM] ([RequisitionDate]);

    PRINT '資料表 RequisitionM 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 RequisitionM 已存在';
END
GO

-- 6. 領料單明細 (對應舊系統 IMS_AM.NAM_REQUISITIOND)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RequisitionD]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RequisitionD] (
        [TxnNo] NVARCHAR(50) NOT NULL,
        [GoodsId] NVARCHAR(50) NOT NULL,
        [Use] NVARCHAR(50) NULL, -- 用途
        [MapplyQty] DECIMAL(18, 2) NULL DEFAULT 0, -- 申請數量
        [Qty] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 實際數量
        CONSTRAINT [PK_RequisitionD] PRIMARY KEY CLUSTERED ([TxnNo], [GoodsId]),
        CONSTRAINT [FK_RequisitionD_RequisitionM] FOREIGN KEY ([TxnNo]) REFERENCES [dbo].[RequisitionM] ([TxnNo]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_RequisitionD_GoodsId] ON [dbo].[RequisitionD] ([GoodsId]);

    PRINT '資料表 RequisitionD 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 RequisitionD 已存在';
END
GO

-- 7. 工務維修主檔 (對應舊系統 IMS_AM.NAM_WORK_MAINTAINM)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkMaintainM]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[WorkMaintainM] (
        [TxnNo] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [SiteId] NVARCHAR(50) NOT NULL,
        [OrgId] NVARCHAR(50) NULL, -- 申請單位
        [ApplyDate] DATETIME2 NULL, -- 申請日期
        [ApplyType] NVARCHAR(500) NULL, -- 請修類別 (多選，以分號分隔)
        [MaintainEmp] NVARCHAR(500) NULL, -- 維保人員 (多選，以分號分隔)
        [BelongStatus] NVARCHAR(10) NULL, -- 歸屬狀態 (1:員工負擔, 2:店別負擔)
        [BelongOrg] NVARCHAR(50) NULL, -- 費用歸屬單位
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_WorkMaintainM_SiteId] ON [dbo].[WorkMaintainM] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_WorkMaintainM_ApplyDate] ON [dbo].[WorkMaintainM] ([ApplyDate]);

    PRINT '資料表 WorkMaintainM 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 WorkMaintainM 已存在';
END
GO

-- =============================================
-- 耗材管理相關資料表
-- 功能代碼: SYSA254, SYSA255, SYSA297
-- =============================================

-- 8. 耗材主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Consumables]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Consumables] (
        [ConsumableId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 耗材編號
        [ConsumableName] NVARCHAR(200) NOT NULL, -- 耗材名稱
        [CategoryId] NVARCHAR(50) NULL, -- 分類代碼
        [Unit] NVARCHAR(20) NULL, -- 單位
        [Specification] NVARCHAR(200) NULL, -- 規格
        [Brand] NVARCHAR(100) NULL, -- 品牌
        [Model] NVARCHAR(100) NULL, -- 型號
        [BarCode] NVARCHAR(50) NULL, -- 條碼
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:正常, 2:停用)
        [AssetStatus] NVARCHAR(10) NULL, -- 資產狀態
        [SiteId] NVARCHAR(50) NULL, -- 店別代碼
        [Location] NVARCHAR(200) NULL, -- 位置
        [Quantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 數量
        [MinQuantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 最小庫存量
        [MaxQuantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 最大庫存量
        [Price] DECIMAL(18, 2) NULL DEFAULT 0, -- 單價
        [SupplierId] NVARCHAR(50) NULL, -- 供應商代碼
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Consumables_SiteId] ON [dbo].[Consumables] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_Consumables_Status] ON [dbo].[Consumables] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Consumables_AssetStatus] ON [dbo].[Consumables] ([AssetStatus]);
    CREATE NONCLUSTERED INDEX [IX_Consumables_BarCode] ON [dbo].[Consumables] ([BarCode]);
    CREATE NONCLUSTERED INDEX [IX_Consumables_CategoryId] ON [dbo].[Consumables] ([CategoryId]);

    PRINT '資料表 Consumables 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Consumables 已存在';
END
GO

-- 9. 耗材分類
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsumableCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsumableCategories] (
        [CategoryId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [CategoryName] NVARCHAR(100) NOT NULL,
        [ParentCategoryId] NVARCHAR(50) NULL,
        [SeqNo] INT NULL DEFAULT 0,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        CONSTRAINT [FK_ConsumableCategories_Parent] FOREIGN KEY ([ParentCategoryId]) REFERENCES [dbo].[ConsumableCategories] ([CategoryId])
    );

    PRINT '資料表 ConsumableCategories 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumableCategories 已存在';
END
GO

-- 10. 耗材列印記錄
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsumablePrintLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsumablePrintLogs] (
        [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ConsumableId] NVARCHAR(50) NOT NULL,
        [PrintType] NVARCHAR(20) NULL, -- 1:耗材管理報表, 2:耗材標籤列印
        [PrintCount] INT NOT NULL DEFAULT 1,
        [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [PrintedBy] NVARCHAR(50) NULL,
        [SiteId] NVARCHAR(50) NULL,
        CONSTRAINT [FK_ConsumablePrintLogs_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConsumablePrintLogs_ConsumableId] ON [dbo].[ConsumablePrintLogs] ([ConsumableId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumablePrintLogs_PrintDate] ON [dbo].[ConsumablePrintLogs] ([PrintDate]);

    PRINT '資料表 ConsumablePrintLogs 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumablePrintLogs 已存在';
END
GO

-- 11. 耗材異動記錄 (SYSA255)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsumableTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsumableTransactions] (
        [TransactionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ConsumableId] NVARCHAR(50) NOT NULL, -- 耗材編號
        [TransactionType] NVARCHAR(10) NOT NULL, -- 異動類型 (1:入庫, 2:出庫, 3:退貨, 4:報廢, 5:出售, 6:領用)
        [TransactionDate] DATETIME2 NOT NULL, -- 異動日期
        [Quantity] DECIMAL(18, 2) NOT NULL, -- 數量
        [UnitPrice] DECIMAL(18, 2) NULL, -- 單價
        [Amount] DECIMAL(18, 2) NULL, -- 金額
        [SiteId] NVARCHAR(50) NULL, -- 店別代碼
        [WarehouseId] NVARCHAR(50) NULL, -- 庫別代碼
        [SourceId] NVARCHAR(50) NULL, -- 來源單號
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_ConsumableTransactions_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_ConsumableId] ON [dbo].[ConsumableTransactions] ([ConsumableId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_TransactionDate] ON [dbo].[ConsumableTransactions] ([TransactionDate]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_TransactionType] ON [dbo].[ConsumableTransactions] ([TransactionType]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_SiteId] ON [dbo].[ConsumableTransactions] ([SiteId]);

    PRINT '資料表 ConsumableTransactions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumableTransactions 已存在';
END
GO

-- 12. 耗材使用統計視圖 (SYSA255)
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ConsumableUsage]'))
BEGIN
    DROP VIEW [dbo].[ConsumableUsage];
END
GO

CREATE VIEW [dbo].[ConsumableUsage] AS
SELECT 
    c.[ConsumableId],
    c.[ConsumableName],
    c.[CategoryId],
    c.[SiteId],
    c.[WarehouseId],
    SUM(CASE WHEN t.[TransactionType] = '1' THEN t.[Quantity] ELSE 0 END) AS [InQty], -- 入庫數量
    SUM(CASE WHEN t.[TransactionType] IN ('2', '6') THEN t.[Quantity] ELSE 0 END) AS [OutQty], -- 出庫數量
    SUM(CASE WHEN t.[TransactionType] = '1' THEN t.[Amount] ELSE 0 END) AS [InAmt], -- 入庫金額
    SUM(CASE WHEN t.[TransactionType] IN ('2', '6') THEN t.[Amount] ELSE 0 END) AS [OutAmt], -- 出庫金額
    c.[Quantity] AS [CurrentQty], -- 當前庫存數量
    c.[Price] * c.[Quantity] AS [CurrentAmt] -- 當前庫存金額
FROM [dbo].[Consumables] c
LEFT JOIN [dbo].[ConsumableTransactions] t ON c.[ConsumableId] = t.[ConsumableId]
GROUP BY c.[ConsumableId], c.[ConsumableName], c.[CategoryId], c.[SiteId], c.[WarehouseId], c.[Quantity], c.[Price];
GO

PRINT '視圖 ConsumableUsage 建立成功';
GO

-- 13. 耗材出售單主檔 (SYSA297)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsumableSales]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsumableSales] (
        [TxnNo] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 交易單號
        [Rrn] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- 唯一識別碼
        [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼
        [PurchaseDate] DATETIME2 NOT NULL, -- 出售日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:待審核, 2:已審核, 3:已取消)
        [TotalAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 總金額
        [TaxAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 稅額
        [NetAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 未稅金額
        [ApplyCount] INT NULL DEFAULT 0, -- 申請數量
        [DetailCount] INT NULL DEFAULT 0, -- 明細數量
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        [ApprovedBy] NVARCHAR(50) NULL, -- 審核者
        [ApprovedAt] DATETIME2 NULL -- 審核時間
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConsumableSales_SiteId] ON [dbo].[ConsumableSales] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableSales_PurchaseDate] ON [dbo].[ConsumableSales] ([PurchaseDate]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableSales_Status] ON [dbo].[ConsumableSales] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableSales_Rrn] ON [dbo].[ConsumableSales] ([Rrn]);

    PRINT '資料表 ConsumableSales 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumableSales 已存在';
END
GO

-- 14. 耗材出售單明細檔 (SYSA297)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsumableSalesDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsumableSalesDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [TxnNo] NVARCHAR(50) NOT NULL, -- 交易單號
        [SeqNo] INT NOT NULL, -- 序號
        [ConsumableId] NVARCHAR(50) NOT NULL, -- 耗材編號
        [ConsumableName] NVARCHAR(200) NULL, -- 耗材名稱
        [Quantity] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 數量
        [Unit] NVARCHAR(20) NULL, -- 單位
        [UnitPrice] DECIMAL(18, 2) NULL DEFAULT 0, -- 單價
        [Amount] DECIMAL(18, 2) NULL DEFAULT 0, -- 金額
        [Tax] NVARCHAR(10) NULL DEFAULT '1', -- 稅別 (1:應稅, 0:免稅)
        [TaxAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 稅額
        [NetAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 未稅金額
        [PurchaseStatus] NVARCHAR(10) NULL DEFAULT '1', -- 採購驗收狀態 (1:已驗收)
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_ConsumableSalesDetails_ConsumableSales] FOREIGN KEY ([TxnNo]) REFERENCES [dbo].[ConsumableSales] ([TxnNo]) ON DELETE CASCADE,
        CONSTRAINT [FK_ConsumableSalesDetails_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConsumableSalesDetails_TxnNo] ON [dbo].[ConsumableSalesDetails] ([TxnNo]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableSalesDetails_ConsumableId] ON [dbo].[ConsumableSalesDetails] ([ConsumableId]);

    PRINT '資料表 ConsumableSalesDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumableSalesDetails 已存在';
END
GO

-- 15. 耗材庫存表 (SYSA297)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsumableInventory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsumableInventory] (
        [InventoryId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ConsumableId] NVARCHAR(50) NOT NULL,
        [SiteId] NVARCHAR(50) NOT NULL,
        [Quantity] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 庫存數量
        [ReservedQuantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 預留數量
        [AvailableQuantity] AS ([Quantity] - [ReservedQuantity]), -- 可用數量
        [LastUpdated] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_ConsumableInventory_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId]),
        CONSTRAINT [UQ_ConsumableInventory_Consumable_Site] UNIQUE ([ConsumableId], [SiteId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConsumableInventory_ConsumableId] ON [dbo].[ConsumableInventory] ([ConsumableId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableInventory_SiteId] ON [dbo].[ConsumableInventory] ([SiteId]);

    PRINT '資料表 ConsumableInventory 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumableInventory 已存在';
END
GO

PRINT '所有分析報表相關資料表建立完成';


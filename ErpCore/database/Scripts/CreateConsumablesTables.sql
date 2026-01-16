-- =============================================
-- 耗材管理資料表建立腳本
-- 功能代碼: SYSA254, SYSA255, SYSA297
-- 建立日期: 2025-01-27
-- =============================================

-- 1. 耗材主檔 (Consumables)
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
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE() -- 更新時間
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

-- 2. 耗材分類 (ConsumableCategories)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsumableCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConsumableCategories] (
        [CategoryId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [CategoryName] NVARCHAR(100) NOT NULL,
        [ParentCategoryId] NVARCHAR(50) NULL,
        [SeqNo] INT NULL DEFAULT 0,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_ConsumableCategories_Parent] FOREIGN KEY ([ParentCategoryId]) REFERENCES [dbo].[ConsumableCategories] ([CategoryId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConsumableCategories_ParentCategoryId] ON [dbo].[ConsumableCategories] ([ParentCategoryId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableCategories_Status] ON [dbo].[ConsumableCategories] ([Status]);

    PRINT '資料表 ConsumableCategories 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumableCategories 已存在';
END
GO

-- 3. 耗材異動記錄 (ConsumableTransactions) - SYSA255
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
    CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_WarehouseId] ON [dbo].[ConsumableTransactions] ([WarehouseId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_SourceId] ON [dbo].[ConsumableTransactions] ([SourceId]);

    PRINT '資料表 ConsumableTransactions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumableTransactions 已存在';
END
GO

-- 4. 耗材列印記錄 (ConsumablePrintLogs)
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
        [Notes] NVARCHAR(500) NULL,
        CONSTRAINT [FK_ConsumablePrintLogs_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConsumablePrintLogs_ConsumableId] ON [dbo].[ConsumablePrintLogs] ([ConsumableId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumablePrintLogs_PrintDate] ON [dbo].[ConsumablePrintLogs] ([PrintDate]);
    CREATE NONCLUSTERED INDEX [IX_ConsumablePrintLogs_PrintType] ON [dbo].[ConsumablePrintLogs] ([PrintType]);
    CREATE NONCLUSTERED INDEX [IX_ConsumablePrintLogs_SiteId] ON [dbo].[ConsumablePrintLogs] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_ConsumablePrintLogs_PrintedBy] ON [dbo].[ConsumablePrintLogs] ([PrintedBy]);

    PRINT '資料表 ConsumablePrintLogs 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ConsumablePrintLogs 已存在';
END
GO

PRINT '耗材管理資料表建立完成';
GO

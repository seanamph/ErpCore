-- 盤點管理模組資料表建立腳本
-- SYSW53M - 盤點維護作業

-- 1. 盤點計劃主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingPlans]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingPlans] (
        [PlanId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [PlanDate] DATETIME2 NOT NULL,
        [StartDate] DATETIME2 NULL,
        [EndDate] DATETIME2 NULL,
        [StartTime] DATETIME2 NULL,
        [EndTime] DATETIME2 NULL,
        [SakeType] NVARCHAR(50) NULL,
        [SakeDept] NVARCHAR(50) NULL,
        [PlanStatus] NVARCHAR(10) NOT NULL DEFAULT '0',
        [SiteId] NVARCHAR(50) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_PlanDate] ON [dbo].[StocktakingPlans] ([PlanDate]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_PlanStatus] ON [dbo].[StocktakingPlans] ([PlanStatus]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_SiteId] ON [dbo].[StocktakingPlans] ([SiteId]);
END
GO

-- 2. 盤點計劃店舖檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingPlanShops]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingPlanShops] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PlanId] NVARCHAR(50) NOT NULL,
        [ShopId] NVARCHAR(50) NOT NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '0',
        [InvStatus] NVARCHAR(10) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_StocktakingPlanShops_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_PlanId] ON [dbo].[StocktakingPlanShops] ([PlanId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_ShopId] ON [dbo].[StocktakingPlanShops] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_Status] ON [dbo].[StocktakingPlanShops] ([Status]);
END
GO

-- 3. 店舖盤點記錄品暫存檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingTemp]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingTemp] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PlanId] NVARCHAR(50) NOT NULL,
        [SPlanId] NVARCHAR(50) NULL,
        [ShopId] NVARCHAR(50) NOT NULL,
        [GoodsId] NVARCHAR(50) NOT NULL,
        [Kind] NVARCHAR(50) NULL,
        [ShelfNo] NVARCHAR(50) NULL,
        [SerialNo] INT NULL,
        [Qty] DECIMAL(18, 4) NULL DEFAULT 0,
        [IQty] DECIMAL(18, 4) NULL DEFAULT 0,
        [IsAdd] NVARCHAR(1) NULL DEFAULT 'N',
        [HtStatus] NVARCHAR(10) NOT NULL DEFAULT '0',
        [Status] NVARCHAR(10) NULL,
        [BUser] NVARCHAR(50) NULL,
        [BTime] DATETIME2 NULL,
        [ApprvId] NVARCHAR(50) NULL,
        [ApprvDate] DATETIME2 NULL,
        [InvDate] DATETIME2 NULL,
        [IsUpdate] NVARCHAR(1) NULL DEFAULT 'N',
        [NumNo] NVARCHAR(50) NULL,
        [HtAuto] NVARCHAR(1) NULL DEFAULT 'N',
        [IsSuccess] NVARCHAR(1) NULL DEFAULT 'N',
        [ErrMsg] NVARCHAR(500) NULL,
        [IsHt] NVARCHAR(1) NULL DEFAULT 'N',
        [SiteId] NVARCHAR(50) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_StocktakingTemp_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_PlanId] ON [dbo].[StocktakingTemp] ([PlanId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_ShopId] ON [dbo].[StocktakingTemp] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_GoodsId] ON [dbo].[StocktakingTemp] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_HtStatus] ON [dbo].[StocktakingTemp] ([HtStatus]);
END
GO

-- 4. 盤點單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [PlanId] NVARCHAR(50) NOT NULL,
        [ShopId] NVARCHAR(50) NOT NULL,
        [GoodsId] NVARCHAR(50) NOT NULL,
        [BookQty] DECIMAL(18, 4) NULL DEFAULT 0,
        [PhysicalQty] DECIMAL(18, 4) NULL DEFAULT 0,
        [DiffQty] DECIMAL(18, 4) NULL DEFAULT 0,
        [UnitCost] DECIMAL(18, 4) NULL,
        [DiffAmount] DECIMAL(18, 4) NULL,
        [Kind] NVARCHAR(50) NULL,
        [ShelfNo] NVARCHAR(50) NULL,
        [SerialNo] INT NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_StocktakingDetails_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_PlanId] ON [dbo].[StocktakingDetails] ([PlanId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_ShopId] ON [dbo].[StocktakingDetails] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_GoodsId] ON [dbo].[StocktakingDetails] ([GoodsId]);
END
GO


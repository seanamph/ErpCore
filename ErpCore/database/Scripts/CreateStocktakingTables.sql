-- =============================================
-- 盤點維護作業資料表建立腳本
-- 功能代碼: SYSW53M
-- 建立日期: 2024-01-27
-- =============================================

-- 1. 盤點計劃主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingPlans]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingPlans] (
        [PlanId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 盤點計劃單號 (PLAN_ID)
        [PlanDate] DATETIME2 NOT NULL, -- 盤點日期 (CHECK_DATE)
        [StartDate] DATETIME2 NULL, -- 開始日期 (B_DATE)
        [EndDate] DATETIME2 NULL, -- 結束日期 (E_DATE)
        [StartTime] DATETIME2 NULL, -- 開始時間 (B_TIME)
        [EndTime] DATETIME2 NULL, -- 結束時間 (E_TIME)
        [SakeType] NVARCHAR(50) NULL, -- 盤點類型 (SAKE_TYPE)
        [SakeDept] NVARCHAR(50) NULL, -- 盤點部門 (SAKE_DEPT)
        [PlanStatus] NVARCHAR(10) NOT NULL DEFAULT '0', -- 計劃狀態 (PLAN_STATUS, -1:申請中, 0:未審核, 1:已審核, 4:作廢, 5:結案)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_PlanDate] ON [dbo].[StocktakingPlans] ([PlanDate]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_PlanStatus] ON [dbo].[StocktakingPlans] ([PlanStatus]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_SiteId] ON [dbo].[StocktakingPlans] ([SiteId]);

    PRINT '資料表 StocktakingPlans 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 StocktakingPlans 已存在';
END
GO

-- 2. 盤點計劃店舖檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingPlanShops]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingPlanShops] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PlanId] NVARCHAR(50) NOT NULL, -- 盤點計劃單號 (PLAN_ID)
        [ShopId] NVARCHAR(50) NOT NULL, -- 店舖代碼 (SHOP_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '0', -- 狀態 (STATUS, 0:計劃, 1:確認, 2:盤點中, 3:計算, 4:帳面庫存, 5:作廢, 6:結案, 7:認列完成)
        [InvStatus] NVARCHAR(10) NULL, -- 盤點狀態 (INV_STATUS)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_StocktakingPlanShops_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_PlanId] ON [dbo].[StocktakingPlanShops] ([PlanId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_ShopId] ON [dbo].[StocktakingPlanShops] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_Status] ON [dbo].[StocktakingPlanShops] ([Status]);

    PRINT '資料表 StocktakingPlanShops 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 StocktakingPlanShops 已存在';
END
GO

-- 3. 店舖盤點記錄品暫存檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingTemp]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingTemp] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PlanId] NVARCHAR(50) NOT NULL, -- 盤點計劃單號 (PLAN_ID)
        [SPlanId] NVARCHAR(50) NULL, -- 子盤點單號 (SPLAN_ID)
        [ShopId] NVARCHAR(50) NOT NULL, -- 店舖代碼 (SHOP_ID)
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (GOODS_ID)
        [Kind] NVARCHAR(50) NULL, -- 盤點區域 (KIND)
        [ShelfNo] NVARCHAR(50) NULL, -- 盤點貨架 (SHELF_NO)
        [SerialNo] INT NULL, -- 盤點貨架序號 (SERIAL_NO)
        [Qty] DECIMAL(18, 4) NULL DEFAULT 0, -- HT上傳量 (QTY)
        [IQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 人工量 (IQTY)
        [IsAdd] NVARCHAR(1) NULL DEFAULT 'N', -- 是否新增 (IS_ADD, Y/N)
        [HtStatus] NVARCHAR(10) NOT NULL DEFAULT '0', -- HT狀態 (HT_STATUS, -1:申請中, 0:未審核, 1:已審核, 4:作廢)
        [Status] NVARCHAR(10) NULL, -- 狀態 (STATUS)
        [BUser] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [BTime] DATETIME2 NULL, -- 建立時間 (BTIME)
        [ApprvId] NVARCHAR(50) NULL, -- 審核者 (APRV_ID)
        [ApprvDate] DATETIME2 NULL, -- 審核日期 (APRV_DATE)
        [InvDate] DATETIME2 NULL, -- 盤點日期 (INV_DATE)
        [IsUpdate] NVARCHAR(1) NULL DEFAULT 'N', -- 是否更新 (IS_UPDATE, Y/N)
        [NumNo] NVARCHAR(50) NULL, -- 序號 (NUM_NO)
        [HtAuto] NVARCHAR(1) NULL DEFAULT 'N', -- HT自動 (HT_AUTO, Y/N)
        [IsSuccess] NVARCHAR(1) NULL DEFAULT 'N', -- 是否成功 (IS_SUCCESS, Y/N)
        [ErrMsg] NVARCHAR(500) NULL, -- 錯誤訊息 (ERR_MSG)
        [IsHt] NVARCHAR(1) NULL DEFAULT 'N', -- 是否HT (IS_HT, Y/N)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_StocktakingTemp_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_PlanId] ON [dbo].[StocktakingTemp] ([PlanId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_ShopId] ON [dbo].[StocktakingTemp] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_GoodsId] ON [dbo].[StocktakingTemp] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_HtStatus] ON [dbo].[StocktakingTemp] ([HtStatus]);

    PRINT '資料表 StocktakingTemp 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 StocktakingTemp 已存在';
END
GO

-- 4. 盤點單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StocktakingDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StocktakingDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [PlanId] NVARCHAR(50) NOT NULL, -- 盤點計劃單號
        [ShopId] NVARCHAR(50) NOT NULL, -- 店舖代碼
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
        [BookQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 帳面數量
        [PhysicalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 實盤數量
        [DiffQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 差異數量
        [UnitCost] DECIMAL(18, 4) NULL, -- 單位成本
        [DiffAmount] DECIMAL(18, 4) NULL, -- 差異金額
        [Kind] NVARCHAR(50) NULL, -- 盤點區域
        [ShelfNo] NVARCHAR(50) NULL, -- 盤點貨架
        [SerialNo] INT NULL, -- 盤點貨架序號
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_StocktakingDetails_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_PlanId] ON [dbo].[StocktakingDetails] ([PlanId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_ShopId] ON [dbo].[StocktakingDetails] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_GoodsId] ON [dbo].[StocktakingDetails] ([GoodsId]);

    PRINT '資料表 StocktakingDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 StocktakingDetails 已存在';
END
GO

-- =============================================
-- 商品進銷碼維護資料表建立腳本
-- 功能代碼: SYSW137
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 商品主檔 (Products)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products] (
        [GoodsId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 進銷碼 (GOODS_ID)
        [GoodsName] NVARCHAR(200) NOT NULL, -- 商品名稱 (GOODS_NAME)
        [InvPrintName] NVARCHAR(200) NULL, -- 發票列印名稱 (INV_PRINT_NAME)
        [GoodsSpace] NVARCHAR(100) NULL, -- 商品規格 (GOODS_SPACE)
        [ScId] NVARCHAR(50) NULL, -- 小分類代碼 (SC_ID)
        [Tax] NVARCHAR(10) NULL DEFAULT '1', -- 稅別 (TAX, 1:應稅, 0:免稅)
        [Lprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 進價 (LPRC)
        [Mprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 中價 (MPRC)
        [BarcodeId] NVARCHAR(50) NULL, -- 國際條碼 (BARCODE_ID)
        [Unit] NVARCHAR(20) NULL, -- 單位 (UNIT)
        [ConvertRate] INT NULL DEFAULT 1, -- 換算率 (CONVERT_RATE)
        [Capacity] INT NULL DEFAULT 0, -- 容量 (CAPACITY)
        [CapacityUnit] NVARCHAR(20) NULL, -- 容量單位 (CAPACITY_UNIT)
        [Status] NVARCHAR(10) NULL DEFAULT '1', -- 狀態 (STATUS, 1:正常, 2:停用)
        [Discount] NVARCHAR(1) NULL DEFAULT 'N', -- 可折扣 (DISCOUNT, Y/N)
        [AutoOrder] NVARCHAR(1) NULL DEFAULT 'N', -- 自動訂貨 (AUTO_ORDER, Y/N)
        [PriceKind] NVARCHAR(10) NULL DEFAULT '1', -- 價格種類 (PRICE_KIND)
        [CostKind] NVARCHAR(10) NULL DEFAULT '1', -- 成本種類 (COST_KIND)
        [SafeDays] INT NULL DEFAULT 0, -- 安全庫存天數 (SAFE_DAYS)
        [ExpirationDays] INT NULL DEFAULT 0, -- 有效期限天數 (EXPIRATION_DAYS)
        [National] NVARCHAR(50) NULL, -- 國別 (NATIONAL)
        [Place] NVARCHAR(100) NULL, -- 產地 (PLACE)
        [GoodsDeep] INT NULL DEFAULT 0, -- 商品-深(公分) (GOODS_DEEP)
        [GoodsWide] INT NULL DEFAULT 0, -- 商品-寬(公分) (GOODS_WIDE)
        [GoodsHigh] INT NULL DEFAULT 0, -- 商品-高(公分) (GOODS_HIGH)
        [PackDeep] INT NULL DEFAULT 0, -- 包裝-深(公分) (PACK_DEEP)
        [PackWide] INT NULL DEFAULT 0, -- 包裝-寬(公分) (PACK_WIDE)
        [PackHigh] INT NULL DEFAULT 0, -- 包裝-高(公分) (PACK_HIGH)
        [PackWeight] INT NULL DEFAULT 0, -- 包裝-重量(KG) (PACK_WEIGHT)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL -- 建立者群組 (CGROUP)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Products_GoodsName] ON [dbo].[Products] ([GoodsName]);
    CREATE NONCLUSTERED INDEX [IX_Products_BarcodeId] ON [dbo].[Products] ([BarcodeId]);
    CREATE NONCLUSTERED INDEX [IX_Products_ScId] ON [dbo].[Products] ([ScId]);
    CREATE NONCLUSTERED INDEX [IX_Products_Status] ON [dbo].[Products] ([Status]);

    PRINT '資料表 Products 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Products 已存在';
END
GO

-- 2. 商品明細檔 (ProductDetails)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductDetails] (
        [ShopId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SHOP_ID)
        [GoodsId] NVARCHAR(50) NOT NULL, -- 進銷碼 (GOODS_ID)
        [SuppId] NVARCHAR(50) NULL, -- 供應商代碼 (SUPP_ID)
        [BarcodeId] NVARCHAR(50) NULL, -- 商品條碼 (BARCODE_ID)
        [Mprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 中價 (MPRC)
        [CanBuy] NVARCHAR(1) NULL DEFAULT 'Y', -- 可進貨 (CAN_BUY, Y/N)
        [CbDate] DATETIME2 NULL, -- 可進貨日期 (CB_DATE)
        [CanReturns] NVARCHAR(1) NULL DEFAULT 'Y', -- 可退貨 (CAN_RETURNS, Y/N)
        [CrDate] DATETIME2 NULL, -- 可退貨日期 (CR_DATE)
        [CanStoreIn] NVARCHAR(1) NULL DEFAULT 'Y', -- 可入庫 (CAN_STORE_IN, Y/N)
        [CsiDate] DATETIME2 NULL, -- 可入庫日期 (CSI_DATE)
        [CanStoreOut] NVARCHAR(1) NULL DEFAULT 'Y', -- 可出庫 (CAN_STORE_OUT, Y/N)
        [CsoDate] DATETIME2 NULL, -- 可出庫日期 (CSO_DATE)
        [CanSales] NVARCHAR(1) NULL DEFAULT 'Y', -- 可銷售 (CAN_SALES, Y/N)
        [CsDate] DATETIME2 NULL, -- 可銷售日期 (CS_DATE)
        [IsOver] NVARCHAR(1) NULL DEFAULT 'N', -- 是否超量 (IS_OVER, Y/N)
        [IoDate] DATETIME2 NULL, -- 超量日期 (IO_DATE)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        CONSTRAINT [PK_ProductDetails] PRIMARY KEY CLUSTERED ([ShopId], [GoodsId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ProductDetails_ShopId] ON [dbo].[ProductDetails] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_ProductDetails_GoodsId] ON [dbo].[ProductDetails] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_ProductDetails_BarcodeId] ON [dbo].[ProductDetails] ([BarcodeId]);

    PRINT '資料表 ProductDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ProductDetails 已存在';
END
GO


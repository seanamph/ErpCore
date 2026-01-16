-- 供應商商品資料維護作業資料表 (SYSW110)
-- 對應舊系統 SUPP_GOODS 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SupplierGoods]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SupplierGoods] (
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPP_ID)
        [BarcodeId] NVARCHAR(50) NOT NULL, -- 商品條碼 (BARCODE_ID)
        [ShopId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SHOP_ID)
        [Lprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 進價 (LPRC)
        [Mprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 中價 (MPRC)
        [Tax] NVARCHAR(10) NULL DEFAULT '1', -- 稅別 (TAX, DEFAULT 1) 1:應稅, 0:免稅
        [MinQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 最小訂購量 (MINQTY)
        [MaxQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 最大訂購量 (MAXQTY)
        [Unit] NVARCHAR(20) NULL, -- 商品單位 (UNIT)
        [Rate] DECIMAL(18, 4) NULL DEFAULT 1, -- 換算率 (RATE, DEFAULT 1)
        [Status] NVARCHAR(10) NULL DEFAULT '0', -- 狀態 (STATUS, 0:正常 1:停用)
        [StartDate] DATETIME2 NULL, -- 有效起始日 (SSDATE)
        [EndDate] DATETIME2 NULL, -- 有效終止日 (SEDATE)
        [Slprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 促銷價格 (SLPRC)
        [ArrivalDays] INT NULL DEFAULT 0, -- 到貨天數 (ARRIVAL_DAYS)
        [OrdDay1] NVARCHAR(1) NULL DEFAULT 'Y', -- 週一可訂購 (ORDDAY1, Y/N)
        [OrdDay2] NVARCHAR(1) NULL DEFAULT 'Y', -- 週二可訂購 (ORDDAY2, Y/N)
        [OrdDay3] NVARCHAR(1) NULL DEFAULT 'Y', -- 週三可訂購 (ORDDAY3, Y/N)
        [OrdDay4] NVARCHAR(1) NULL DEFAULT 'Y', -- 週四可訂購 (ORDDAY4, Y/N)
        [OrdDay5] NVARCHAR(1) NULL DEFAULT 'Y', -- 週五可訂購 (ORDDAY5, Y/N)
        [OrdDay6] NVARCHAR(1) NULL DEFAULT 'Y', -- 週六可訂購 (ORDDAY6, Y/N)
        [OrdDay7] NVARCHAR(1) NULL DEFAULT 'Y', -- 週日可訂購 (ORDDAY7, Y/N)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_SupplierGoods] PRIMARY KEY CLUSTERED ([SupplierId], [BarcodeId], [ShopId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SupplierGoods_SupplierId] ON [dbo].[SupplierGoods] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_SupplierGoods_BarcodeId] ON [dbo].[SupplierGoods] ([BarcodeId]);
    CREATE NONCLUSTERED INDEX [IX_SupplierGoods_ShopId] ON [dbo].[SupplierGoods] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_SupplierGoods_Status] ON [dbo].[SupplierGoods] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SupplierGoods_StartDate_EndDate] ON [dbo].[SupplierGoods] ([StartDate], [EndDate]);

    PRINT 'SupplierGoods 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'SupplierGoods 資料表已存在';
END

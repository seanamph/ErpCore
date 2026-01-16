-- =============================================
-- 商品永久變價作業資料表建立腳本
-- 功能代碼: SYSW150
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 變價單主檔 (PriceChangeMasters)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PriceChangeMasters]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PriceChangeMasters] (
        [PriceChangeId] NVARCHAR(50) NOT NULL, -- 變價單號 (PRICECHG_ID)
        [PriceChangeType] NVARCHAR(10) NOT NULL, -- 變價類型 (PRICECHG_CLAS: 1:進價, 2:售價)
        [SupplierId] NVARCHAR(50) NULL, -- 廠商編號 (SUPPLIER_ID)
        [LogoId] NVARCHAR(50) NULL, -- 品牌編號 (LOGO_ID)
        [ApplyEmpId] NVARCHAR(50) NULL, -- 申請人員編號 (APPLY_EMP_ID)
        [ApplyOrgId] NVARCHAR(50) NULL, -- 申請單位 (APPLY_ORG_ID)
        [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
        [StartDate] DATETIME2 NULL, -- 啟用日期 (STAR_DATE)
        [ApproveEmpId] NVARCHAR(50) NULL, -- 審核人員編號 (APRV_EMP_ID)
        [ApproveDate] DATETIME2 NULL, -- 審核日期 (APRV_DATE)
        [ConfirmEmpId] NVARCHAR(50) NULL, -- 確認人員編號 (CONF_EMP_ID)
        [ConfirmDate] DATETIME2 NULL, -- 確認日期 (CONF_DATE)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (PRICECHG_STATUS: 1:已申請, 2:已審核, 9:已作廢, 10:已確認)
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (INTOLAMT/SALETOLAMT)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_PriceChangeMasters] PRIMARY KEY CLUSTERED ([PriceChangeId], [PriceChangeType])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_SupplierId] ON [dbo].[PriceChangeMasters] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_LogoId] ON [dbo].[PriceChangeMasters] ([LogoId]);
    CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_Status] ON [dbo].[PriceChangeMasters] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_ApplyDate] ON [dbo].[PriceChangeMasters] ([ApplyDate]);
    CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_StartDate] ON [dbo].[PriceChangeMasters] ([StartDate]);

    PRINT '資料表 PriceChangeMasters 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PriceChangeMasters 已存在';
END
GO

-- 2. 變價單明細 (PriceChangeDetails)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PriceChangeDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PriceChangeDetails] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PriceChangeId] NVARCHAR(50) NOT NULL, -- 變價單號 (PRICECHG_ID/SALEPRICECHG_ID)
        [PriceChangeType] NVARCHAR(10) NOT NULL, -- 變價類型 (1:進價, 2:售價)
        [LineNum] INT NOT NULL, -- 序號 (LINE_NUM)
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (GOODS_ID)
        [BeforePrice] DECIMAL(18, 4) NULL DEFAULT 0, -- 調整前單價 (COST_PRICE)
        [AfterPrice] DECIMAL(18, 4) NOT NULL, -- 調整後單價 (ACOST_PRICE)
        [ChangeQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 變價數量 (CHG_QTY)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [FK_PriceChangeDetails_Masters] FOREIGN KEY ([PriceChangeId], [PriceChangeType]) 
            REFERENCES [dbo].[PriceChangeMasters] ([PriceChangeId], [PriceChangeType]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PriceChangeDetails_PriceChangeId] ON [dbo].[PriceChangeDetails] ([PriceChangeId], [PriceChangeType]);
    CREATE NONCLUSTERED INDEX [IX_PriceChangeDetails_GoodsId] ON [dbo].[PriceChangeDetails] ([GoodsId]);

    PRINT '資料表 PriceChangeDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PriceChangeDetails 已存在';
END
GO

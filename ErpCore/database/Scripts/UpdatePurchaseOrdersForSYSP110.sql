-- =============================================
-- 更新採購單資料表以支援 SYSP110-SYSP190 功能
-- 功能代碼: SYSP110-SYSP190
-- 建立日期: 2025-01-27
-- 說明: 添加開發計劃要求的缺失欄位
-- =============================================

-- 1. 更新 PurchaseOrders 表，添加缺失欄位
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrders]') AND name = 'OrgId')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrders]
    ADD [OrgId] NVARCHAR(50) NULL; -- 組織代碼
    PRINT '已添加欄位 PurchaseOrders.OrgId';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrders]') AND name = 'CurrencyId')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrders]
    ADD [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD'; -- 幣別
    PRINT '已添加欄位 PurchaseOrders.CurrencyId';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrders]') AND name = 'ExchangeRate')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrders]
    ADD [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1; -- 匯率
    PRINT '已添加欄位 PurchaseOrders.ExchangeRate';
END
GO

-- 更新 Status 欄位，添加 'C' (已結案) 狀態支援
-- 注意：如果表已存在，Status 欄位應該已經支援 'C' 狀態，這裡只是確保預設值正確

-- 2. 更新 PurchaseOrderDetails 表，添加缺失欄位
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderDetails]') AND name = 'UnitId')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD [UnitId] NVARCHAR(50) NULL; -- 單位
    PRINT '已添加欄位 PurchaseOrderDetails.UnitId';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderDetails]') AND name = 'TaxRate')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0; -- 稅率
    PRINT '已添加欄位 PurchaseOrderDetails.TaxRate';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderDetails]') AND name = 'TaxAmount')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0; -- 稅額
    PRINT '已添加欄位 PurchaseOrderDetails.TaxAmount';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderDetails]') AND name = 'ReturnQty')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD [ReturnQty] DECIMAL(18, 4) NULL DEFAULT 0; -- 已退數量
    PRINT '已添加欄位 PurchaseOrderDetails.ReturnQty';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderDetails]') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD [UpdatedBy] NVARCHAR(50) NULL; -- 更新人員
    PRINT '已添加欄位 PurchaseOrderDetails.UpdatedBy';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderDetails]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD [UpdatedAt] DATETIME2 NULL DEFAULT GETDATE(); -- 更新時間
    PRINT '已添加欄位 PurchaseOrderDetails.UpdatedAt';
END
GO

PRINT '採購單資料表更新完成';

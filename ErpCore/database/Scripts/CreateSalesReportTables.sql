-- =============================================
-- 銷售報表管理類 (SYS1000) - 資料庫腳本
-- 功能：銷售報表模組系列 (SYS1100-SYS1D10等)、Crystal Reports報表功能 (SYS1360)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 銷售報表資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesReports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SalesReports] (
        [ReportId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 報表編號
        [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (SYS1100-SYS1D10等)
        [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
        [ReportType] NVARCHAR(50) NULL, -- 報表類型
        [ShopId] NVARCHAR(50) NULL, -- 店別代碼
        [StartDate] DATETIME2 NULL, -- 起始日期
        [EndDate] DATETIME2 NULL, -- 結束日期
        [ReportData] NVARCHAR(MAX) NULL, -- 報表資料 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_SalesReports] PRIMARY KEY CLUSTERED ([ReportId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SalesReports_ReportCode] ON [dbo].[SalesReports] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_SalesReports_ShopId] ON [dbo].[SalesReports] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_SalesReports_StartDate_EndDate] ON [dbo].[SalesReports] ([StartDate], [EndDate]);
    CREATE NONCLUSTERED INDEX [IX_SalesReports_Status] ON [dbo].[SalesReports] ([Status]);
END
GO

-- 2. 商品報表資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductReports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductReports] (
        [ProductReportId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), -- 商品報表編號
        [ProductId] NVARCHAR(50) NULL, -- 商品編號
        [BarcodeId] NVARCHAR(50) NULL, -- 商品條碼
        [ProductName] NVARCHAR(200) NULL, -- 商品名稱
        [ShopId] NVARCHAR(50) NULL, -- 店別代碼
        [ReportDate] DATETIME2 NULL, -- 報表日期
        [SalesQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 銷售數量
        [SalesAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 銷售金額
        [CostAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 成本金額
        [ProfitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 利潤金額
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [PK_ProductReports] PRIMARY KEY CLUSTERED ([ProductReportId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ProductReports_ProductId] ON [dbo].[ProductReports] ([ProductId]);
    CREATE NONCLUSTERED INDEX [IX_ProductReports_ShopId] ON [dbo].[ProductReports] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_ProductReports_ReportDate] ON [dbo].[ProductReports] ([ReportDate]);
END
GO

-- 3. Crystal Reports 報表設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CrystalReportSettings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CrystalReportSettings] (
        [SettingId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), -- 設定編號
        [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (SYS1360)
        [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
        [ReportPath] NVARCHAR(500) NULL, -- 報表檔案路徑
        [Parameters] NVARCHAR(MAX) NULL, -- 報表參數 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_CrystalReportSettings] PRIMARY KEY CLUSTERED ([SettingId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CrystalReportSettings_ReportCode] ON [dbo].[CrystalReportSettings] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_CrystalReportSettings_Status] ON [dbo].[CrystalReportSettings] ([Status]);
END
GO


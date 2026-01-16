-- SYSW170 - POP卡商品卡列印作業資料表
-- POP列印設定
CREATE TABLE [dbo].[PopPrintSettings] (
    [SettingId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ShopId] NVARCHAR(50) NULL,
    [Ip] NVARCHAR(50) NULL,
    [TypeId] NVARCHAR(50) NULL, -- 報表類型
    [Version] NVARCHAR(20) NULL, -- 版本標記 (AP, UA, STANDARD)
    [DebugMode] BIT NOT NULL DEFAULT 0,
    [HeaderHeightPadding] INT NULL DEFAULT 0,
    [HeaderHeightPaddingRemain] INT NULL DEFAULT 851,
    [PageHeaderHeightPadding] INT NULL DEFAULT 0,
    [PagePadding] NVARCHAR(100) NULL, -- 左,右,上,下
    [PageSize] NVARCHAR(100) NULL, -- 高,寬
    [ApSpecificSettings] NVARCHAR(MAX) NULL, -- AP版本專屬設定 (JSON格式)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PopPrintSettings_ShopId] ON [dbo].[PopPrintSettings] ([ShopId]);

-- POP列印記錄
CREATE TABLE [dbo].[PopPrintLogs] (
    [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [GoodsId] NVARCHAR(50) NOT NULL,
    [PrintType] NVARCHAR(20) NULL, -- POP, PRODUCT_CARD
    [PrintFormat] NVARCHAR(20) NULL, -- PR1, PR2, PR3, PR4, PR5, PR6, AP等
    [Version] NVARCHAR(20) NULL, -- 版本標記 (AP, UA, STANDARD)
    [PrintCount] INT NOT NULL DEFAULT 1,
    [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [PrintedBy] NVARCHAR(50) NULL,
    [ShopId] NVARCHAR(50) NULL
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_GoodsId] ON [dbo].[PopPrintLogs] ([GoodsId]);
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_PrintType] ON [dbo].[PopPrintLogs] ([PrintType]);
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_PrintFormat] ON [dbo].[PopPrintLogs] ([PrintFormat]);
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_ShopId] ON [dbo].[PopPrintLogs] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_PrintDate] ON [dbo].[PopPrintLogs] ([PrintDate]);
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_Version] ON [dbo].[PopPrintLogs] ([Version]);

-- 外鍵約束（如果 Products 表存在）
-- ALTER TABLE [dbo].[PopPrintLogs]
-- ADD CONSTRAINT [FK_PopPrintLogs_Products] 
-- FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId]);


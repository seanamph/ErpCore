-- =============================================
-- ADDR_ZONE_LIST - 地址區域列表資料表建立腳本
-- 開發計劃文件：開發計劃/15-下拉列表/ADDR_ZONE_LIST-地址區域列表.md
-- =============================================

-- 說明：
-- 此腳本用於建立地址區域列表（ADDR_ZONE_LIST）功能所需的資料表
-- 區域主檔（Zones）資料表已包含在 CreateDropdownListTables.sql 中
-- 如需單獨執行，請參考以下 SQL 語句

-- 區域主檔 (Zones)
-- 注意：此資料表已包含在 CreateDropdownListTables.sql 中
-- 如果資料表已存在，此腳本會檢查並添加缺失的欄位和索引

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Zones]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Zones] (
        [ZoneId] NVARCHAR(20) NOT NULL PRIMARY KEY,
        [ZoneName] NVARCHAR(100) NOT NULL,
        [CityId] NVARCHAR(20) NOT NULL,
        [ZipCode] NVARCHAR(10) NULL,
        [SeqNo] INT NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Zones] PRIMARY KEY CLUSTERED ([ZoneId] ASC),
        CONSTRAINT [FK_Zones_Cities] FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([CityId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Zones_ZoneName] ON [dbo].[Zones] ([ZoneName]);
    CREATE NONCLUSTERED INDEX [IX_Zones_CityId] ON [dbo].[Zones] ([CityId]);
    CREATE NONCLUSTERED INDEX [IX_Zones_ZipCode] ON [dbo].[Zones] ([ZipCode]);
    CREATE NONCLUSTERED INDEX [IX_Zones_Status] ON [dbo].[Zones] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Zones_SeqNo] ON [dbo].[Zones] ([SeqNo]);
    
    PRINT 'Zones 資料表建立成功'
END
ELSE
BEGIN
    -- 如果表已存在，檢查並添加 ZipCode 欄位
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Zones]') AND name = 'ZipCode')
    BEGIN
        ALTER TABLE [dbo].[Zones] ADD [ZipCode] NVARCHAR(10) NULL;
        CREATE NONCLUSTERED INDEX [IX_Zones_ZipCode] ON [dbo].[Zones] ([ZipCode]);
        PRINT '已添加 ZipCode 欄位和索引'
    END

    -- 檢查外鍵約束
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Zones_Cities')
    BEGIN
        ALTER TABLE [dbo].[Zones]
        ADD CONSTRAINT [FK_Zones_Cities] FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([CityId]);
        PRINT '已添加外鍵約束 FK_Zones_Cities'
    END

    -- 檢查索引
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Zones_ZoneName' AND object_id = OBJECT_ID(N'[dbo].[Zones]'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Zones_ZoneName] ON [dbo].[Zones] ([ZoneName]);
        PRINT '已添加索引 IX_Zones_ZoneName'
    END

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Zones_CityId' AND object_id = OBJECT_ID(N'[dbo].[Zones]'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Zones_CityId] ON [dbo].[Zones] ([CityId]);
        PRINT '已添加索引 IX_Zones_CityId'
    END

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Zones_ZipCode' AND object_id = OBJECT_ID(N'[dbo].[Zones]'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Zones_ZipCode] ON [dbo].[Zones] ([ZipCode]);
        PRINT '已添加索引 IX_Zones_ZipCode'
    END

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Zones_Status' AND object_id = OBJECT_ID(N'[dbo].[Zones]'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Zones_Status] ON [dbo].[Zones] ([Status]);
        PRINT '已添加索引 IX_Zones_Status'
    END

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Zones_SeqNo' AND object_id = OBJECT_ID(N'[dbo].[Zones]'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Zones_SeqNo] ON [dbo].[Zones] ([SeqNo]);
        PRINT '已添加索引 IX_Zones_SeqNo'
    END

    PRINT 'Zones 資料表已存在，已檢查並更新結構'
END
GO

-- =============================================
-- 資料表結構說明
-- =============================================
-- Zones 資料表欄位說明：
--   ZoneId: 區域代碼（主鍵）
--   ZoneName: 區域名稱
--   CityId: 城市代碼（外鍵至 Cities 表）
--   ZipCode: 郵遞區號
--   SeqNo: 排序序號
--   Status: 狀態（1:啟用, 0:停用）
--   CreatedBy: 建立者
--   CreatedAt: 建立時間
--   UpdatedBy: 更新者
--   UpdatedAt: 更新時間
-- =============================================

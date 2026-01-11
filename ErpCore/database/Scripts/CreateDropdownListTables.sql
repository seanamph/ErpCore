-- =============================================
-- 下拉列表類資料表建立腳本
-- =============================================

-- 1. 城市主檔 (Cities)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cities] (
        [CityId] NVARCHAR(20) NOT NULL PRIMARY KEY,
        [CityName] NVARCHAR(100) NOT NULL,
        [CountryCode] NVARCHAR(10) NULL,
        [SeqNo] INT NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Cities_CityName] ON [dbo].[Cities] ([CityName]);
    CREATE NONCLUSTERED INDEX [IX_Cities_CountryCode] ON [dbo].[Cities] ([CountryCode]);
    CREATE NONCLUSTERED INDEX [IX_Cities_Status] ON [dbo].[Cities] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Cities_SeqNo] ON [dbo].[Cities] ([SeqNo]);
END
GO

-- 2. 區域主檔 (Zones)
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
END
ELSE
BEGIN
    -- 如果表已存在，檢查並添加 ZipCode 欄位
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Zones]') AND name = 'ZipCode')
    BEGIN
        ALTER TABLE [dbo].[Zones] ADD [ZipCode] NVARCHAR(10) NULL;
        CREATE NONCLUSTERED INDEX [IX_Zones_ZipCode] ON [dbo].[Zones] ([ZipCode]);
    END

    -- 確保 CityId 不為 NULL（如果原本允許 NULL）
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Zones]') AND name = 'CityId' AND is_nullable = 1)
    BEGIN
        -- 先更新現有 NULL 值（可選，根據業務需求）
        -- UPDATE [dbo].[Zones] SET [CityId] = '' WHERE [CityId] IS NULL;
        -- 然後修改欄位為 NOT NULL
        -- ALTER TABLE [dbo].[Zones] ALTER COLUMN [CityId] NVARCHAR(20) NOT NULL;
    END
END
GO


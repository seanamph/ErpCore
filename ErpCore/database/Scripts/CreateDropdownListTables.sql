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
        [CityId] NVARCHAR(20) NULL,
        [SeqNo] INT NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_Zones_Cities] FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([CityId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Zones_ZoneName] ON [dbo].[Zones] ([ZoneName]);
    CREATE NONCLUSTERED INDEX [IX_Zones_CityId] ON [dbo].[Zones] ([CityId]);
    CREATE NONCLUSTERED INDEX [IX_Zones_Status] ON [dbo].[Zones] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Zones_SeqNo] ON [dbo].[Zones] ([SeqNo]);
END
GO


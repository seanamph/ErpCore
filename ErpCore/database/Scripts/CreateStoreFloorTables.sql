-- 商店樓層管理類資料表 (SYS6000)
-- 包含：商店資料維護、商店查詢作業、樓層資料維護、樓層查詢作業、類型代碼維護、類型代碼查詢、POS資料維護、POS查詢作業

-- =============================================
-- ShopFloors - 商店樓層管理主檔
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShopFloors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ShopFloors] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ShopId] NVARCHAR(50) NOT NULL, -- 商店編號
        [ShopName] NVARCHAR(200) NOT NULL, -- 商店名稱
        [ShopNameEn] NVARCHAR(200) NULL, -- 商店英文名稱
        [FloorId] NVARCHAR(50) NULL, -- 樓層代碼
        [FloorName] NVARCHAR(100) NULL, -- 樓層名稱
        [ShopType] NVARCHAR(50) NULL, -- 商店類型
        [Address] NVARCHAR(500) NULL, -- 地址
        [City] NVARCHAR(50) NULL, -- 城市
        [Zone] NVARCHAR(50) NULL, -- 區域
        [PostalCode] NVARCHAR(20) NULL, -- 郵遞區號
        [Phone] NVARCHAR(50) NULL, -- 電話
        [Fax] NVARCHAR(50) NULL, -- 傳真
        [Email] NVARCHAR(100) NULL, -- 電子郵件
        [ManagerName] NVARCHAR(100) NULL, -- 店長姓名
        [ManagerPhone] NVARCHAR(50) NULL, -- 店長電話
        [OpenDate] DATETIME2 NULL, -- 開店日期
        [CloseDate] DATETIME2 NULL, -- 關店日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
        [PosEnabled] BIT NULL DEFAULT 0, -- POS啟用
        [PosSystemId] NVARCHAR(50) NULL, -- POS系統代碼
        [PosTerminalId] NVARCHAR(50) NULL, -- POS終端代碼
        [Notes] NVARCHAR(1000) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_ShopFloors] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_ShopFloors_ShopId] UNIQUE ([ShopId])
    );

    CREATE NONCLUSTERED INDEX [IX_ShopFloors_ShopId] ON [dbo].[ShopFloors] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_ShopFloors_ShopType] ON [dbo].[ShopFloors] ([ShopType]);
    CREATE NONCLUSTERED INDEX [IX_ShopFloors_Status] ON [dbo].[ShopFloors] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_ShopFloors_FloorId] ON [dbo].[ShopFloors] ([FloorId]);
    CREATE NONCLUSTERED INDEX [IX_ShopFloors_City] ON [dbo].[ShopFloors] ([City]);
END
GO

-- =============================================
-- Floors - 樓層資料
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Floors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Floors] (
        [FloorId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [FloorName] NVARCHAR(100) NOT NULL,
        [FloorNameEn] NVARCHAR(100) NULL,
        [FloorNumber] INT NULL, -- 樓層號碼
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_Floors_Status] ON [dbo].[Floors] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Floors_FloorNumber] ON [dbo].[Floors] ([FloorNumber]);
END
GO

-- =============================================
-- ShopTypes - 商店類型 (如果不存在)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShopTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ShopTypes] (
        [ShopTypeId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [ShopTypeName] NVARCHAR(100) NOT NULL,
        [ShopTypeNameEn] NVARCHAR(100) NULL,
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

-- =============================================
-- TypeCodes - 類型代碼
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TypeCodes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TypeCodes] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TypeCode] NVARCHAR(50) NOT NULL, -- 類型代碼
        [TypeName] NVARCHAR(100) NOT NULL, -- 類型名稱
        [TypeNameEn] NVARCHAR(100) NULL, -- 類型英文名稱
        [Category] NVARCHAR(50) NULL, -- 分類
        [Description] NVARCHAR(500) NULL,
        [SortOrder] INT NULL DEFAULT 0, -- 排序順序
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_TypeCodes_TypeCode] UNIQUE ([TypeCode], [Category])
    );

    CREATE NONCLUSTERED INDEX [IX_TypeCodes_TypeCode] ON [dbo].[TypeCodes] ([TypeCode]);
    CREATE NONCLUSTERED INDEX [IX_TypeCodes_Category] ON [dbo].[TypeCodes] ([Category]);
    CREATE NONCLUSTERED INDEX [IX_TypeCodes_Status] ON [dbo].[TypeCodes] ([Status]);
END
GO

-- =============================================
-- ShopPosSettings - 商店POS設定
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShopPosSettings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ShopPosSettings] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ShopId] NVARCHAR(50) NOT NULL,
        [PosSystemId] NVARCHAR(50) NOT NULL,
        [PosTerminalId] NVARCHAR(50) NULL,
        [PosConfig] NVARCHAR(MAX) NULL, -- JSON格式的POS設定
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_ShopPosSettings_ShopFloors] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[ShopFloors] ([ShopId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_ShopPosSettings_ShopId] ON [dbo].[ShopPosSettings] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_ShopPosSettings_PosSystemId] ON [dbo].[ShopPosSettings] ([PosSystemId]);
END
GO

-- =============================================
-- PosTerminals - POS終端主檔
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PosTerminals]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PosTerminals] (
        [PosTerminalId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [PosSystemId] NVARCHAR(50) NOT NULL, -- POS系統代碼
        [TerminalCode] NVARCHAR(50) NOT NULL, -- 終端代碼
        [TerminalName] NVARCHAR(200) NOT NULL, -- 終端名稱
        [ShopId] NVARCHAR(50) NULL, -- 商店編號
        [FloorId] NVARCHAR(50) NULL, -- 樓層代碼
        [TerminalType] NVARCHAR(50) NULL, -- 終端類型
        [IpAddress] NVARCHAR(50) NULL, -- IP位址
        [Port] INT NULL, -- 連接埠
        [Config] NVARCHAR(MAX) NULL, -- JSON格式的設定
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
        [LastSyncDate] DATETIME2 NULL, -- 最後同步時間
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_PosTerminals] PRIMARY KEY CLUSTERED ([PosTerminalId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_PosTerminals_PosSystemId] ON [dbo].[PosTerminals] ([PosSystemId]);
    CREATE NONCLUSTERED INDEX [IX_PosTerminals_ShopId] ON [dbo].[PosTerminals] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_PosTerminals_Status] ON [dbo].[PosTerminals] ([Status]);
END
GO


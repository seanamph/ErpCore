-- 商店與會員管理類資料表 (SYS3000)
-- 包含：商店資料維護、商店查詢作業、會員資料維護、會員查詢作業、促銷活動維護、報表查詢作業

-- =============================================
-- Store - 商店資料維護相關資料表
-- =============================================

-- Shops (商店主檔)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Shops]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Shops] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ShopId] NVARCHAR(50) NOT NULL, -- 商店編號
        [ShopName] NVARCHAR(200) NOT NULL, -- 商店名稱
        [ShopNameEn] NVARCHAR(200) NULL, -- 商店英文名稱
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
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [FloorId] NVARCHAR(50) NULL, -- 樓層代碼
        [AreaId] NVARCHAR(50) NULL, -- 區域代碼
        [PosEnabled] BIT NULL DEFAULT 0, -- POS啟用
        [PosSystemId] NVARCHAR(50) NULL, -- POS系統代碼
        [Notes] NVARCHAR(1000) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Shops] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Shops_ShopId] UNIQUE ([ShopId])
    );

    CREATE NONCLUSTERED INDEX [IX_Shops_ShopId] ON [dbo].[Shops] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_Shops_ShopName] ON [dbo].[Shops] ([ShopName]);
    CREATE NONCLUSTERED INDEX [IX_Shops_ShopType] ON [dbo].[Shops] ([ShopType]);
    CREATE NONCLUSTERED INDEX [IX_Shops_Status] ON [dbo].[Shops] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Shops_FloorId] ON [dbo].[Shops] ([FloorId]);
    CREATE NONCLUSTERED INDEX [IX_Shops_AreaId] ON [dbo].[Shops] ([AreaId]);
    CREATE NONCLUSTERED INDEX [IX_Shops_City] ON [dbo].[Shops] ([City]);
END
GO

-- ShopTypes (商店類型)
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

-- ShopPosSettings (商店POS設定)
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
        CONSTRAINT [FK_ShopPosSettings_Shops] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[Shops] ([ShopId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_ShopPosSettings_ShopId] ON [dbo].[ShopPosSettings] ([ShopId]);
END
GO

-- =============================================
-- Member - 會員資料維護相關資料表
-- =============================================

-- Members (會員主檔)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Members]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Members] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [MemberId] NVARCHAR(50) NOT NULL, -- 會員編號
        [MemberName] NVARCHAR(100) NOT NULL, -- 會員姓名
        [MemberNameEn] NVARCHAR(100) NULL, -- 會員英文姓名
        [Gender] NVARCHAR(10) NULL, -- 性別 (M:男, F:女)
        [BirthDate] DATETIME2 NULL, -- 生日
        [PersonalId] NVARCHAR(50) NULL, -- 身份證字號
        [Phone] NVARCHAR(50) NULL, -- 電話
        [Mobile] NVARCHAR(50) NULL, -- 手機
        [Email] NVARCHAR(100) NULL, -- 電子郵件
        [Address] NVARCHAR(500) NULL, -- 地址
        [City] NVARCHAR(50) NULL, -- 城市
        [Zone] NVARCHAR(50) NULL, -- 區域
        [PostalCode] NVARCHAR(20) NULL, -- 郵遞區號
        [MemberLevel] NVARCHAR(50) NULL, -- 會員等級
        [Points] DECIMAL(18, 4) NULL DEFAULT 0, -- 積分
        [TotalPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 累計積分
        [CardNo] NVARCHAR(50) NULL, -- 卡片號碼
        [CardType] NVARCHAR(50) NULL, -- 卡片類型
        [JoinDate] DATETIME2 NULL, -- 加入日期
        [ExpireDate] DATETIME2 NULL, -- 到期日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用, S:暫停)
        [PhotoPath] NVARCHAR(500) NULL, -- 照片路徑
        [Notes] NVARCHAR(1000) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Members] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Members_MemberId] UNIQUE ([MemberId])
    );

    CREATE NONCLUSTERED INDEX [IX_Members_MemberId] ON [dbo].[Members] ([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_Members_MemberName] ON [dbo].[Members] ([MemberName]);
    CREATE NONCLUSTERED INDEX [IX_Members_PersonalId] ON [dbo].[Members] ([PersonalId]);
    CREATE NONCLUSTERED INDEX [IX_Members_Phone] ON [dbo].[Members] ([Phone]);
    CREATE NONCLUSTERED INDEX [IX_Members_Mobile] ON [dbo].[Members] ([Mobile]);
    CREATE NONCLUSTERED INDEX [IX_Members_Email] ON [dbo].[Members] ([Email]);
    CREATE NONCLUSTERED INDEX [IX_Members_MemberLevel] ON [dbo].[Members] ([MemberLevel]);
    CREATE NONCLUSTERED INDEX [IX_Members_Status] ON [dbo].[Members] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Members_CardNo] ON [dbo].[Members] ([CardNo]);
END
GO

-- MemberLevels (會員等級)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberLevels]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MemberLevels] (
        [LevelId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [LevelName] NVARCHAR(100) NOT NULL,
        [LevelNameEn] NVARCHAR(100) NULL,
        [MinPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 最低積分
        [MaxPoints] DECIMAL(18, 4) NULL, -- 最高積分
        [DiscountRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 折扣率
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

-- MemberCards (會員卡片)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberCards]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MemberCards] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CardId] NVARCHAR(50) NOT NULL,
        [MemberId] NVARCHAR(50) NOT NULL,
        [CardNo] NVARCHAR(50) NOT NULL,
        [CardType] NVARCHAR(50) NULL,
        [IssueDate] DATETIME2 NULL,
        [ExpireDate] DATETIME2 NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用, L:遺失
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_MemberCards_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members] ([MemberId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_MemberCards_MemberId] ON [dbo].[MemberCards] ([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_MemberCards_CardNo] ON [dbo].[MemberCards] ([CardNo]);
END
GO

-- MemberPoints (會員積分記錄)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberPoints]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MemberPoints] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [MemberId] NVARCHAR(50) NOT NULL,
        [TransactionDate] DATETIME2 NOT NULL,
        [TransactionType] NVARCHAR(20) NOT NULL, -- EARN:獲得, USE:使用, EXPIRE:過期, ADJUST:調整
        [Points] DECIMAL(18, 4) NOT NULL, -- 正數為獲得，負數為使用
        [Balance] DECIMAL(18, 4) NOT NULL, -- 餘額
        [ReferenceNo] NVARCHAR(50) NULL, -- 參考單號
        [Description] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_MemberPoints_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members] ([MemberId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_MemberPoints_MemberId] ON [dbo].[MemberPoints] ([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_MemberPoints_TransactionDate] ON [dbo].[MemberPoints] ([TransactionDate]);
    CREATE NONCLUSTERED INDEX [IX_MemberPoints_TransactionType] ON [dbo].[MemberPoints] ([TransactionType]);
END
GO

-- =============================================
-- Promotion - 促銷活動維護相關資料表
-- =============================================

-- Promotions (促銷活動主檔)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Promotions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Promotions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PromotionId] NVARCHAR(50) NOT NULL, -- 促銷活動編號
        [PromotionName] NVARCHAR(200) NOT NULL, -- 促銷活動名稱
        [PromotionType] NVARCHAR(50) NULL, -- 促銷類型
        [StartDate] DATETIME2 NOT NULL, -- 開始日期
        [EndDate] DATETIME2 NOT NULL, -- 結束日期
        [DiscountType] NVARCHAR(20) NULL, -- 折扣類型 (PERCENT:百分比, AMOUNT:金額)
        [DiscountValue] DECIMAL(18, 4) NULL DEFAULT 0, -- 折扣值
        [MinPurchaseAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 最低消費金額
        [MaxDiscountAmount] DECIMAL(18, 4) NULL, -- 最高折扣金額
        [ApplicableShops] NVARCHAR(MAX) NULL, -- 適用商店 (JSON格式)
        [ApplicableProducts] NVARCHAR(MAX) NULL, -- 適用商品 (JSON格式)
        [ApplicableMemberLevels] NVARCHAR(MAX) NULL, -- 適用會員等級 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [Notes] NVARCHAR(1000) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Promotions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Promotions_PromotionId] UNIQUE ([PromotionId])
    );

    CREATE NONCLUSTERED INDEX [IX_Promotions_PromotionId] ON [dbo].[Promotions] ([PromotionId]);
    CREATE NONCLUSTERED INDEX [IX_Promotions_StartDate] ON [dbo].[Promotions] ([StartDate]);
    CREATE NONCLUSTERED INDEX [IX_Promotions_EndDate] ON [dbo].[Promotions] ([EndDate]);
    CREATE NONCLUSTERED INDEX [IX_Promotions_Status] ON [dbo].[Promotions] ([Status]);
END
GO

-- =============================================
-- StoreReport - 報表查詢作業相關資料表
-- =============================================

-- StoreReports (商店報表)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreReports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StoreReports] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportId] NVARCHAR(50) NOT NULL, -- 報表編號
        [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型
        [ShopId] NVARCHAR(50) NULL, -- 商店編號
        [ReportDate] DATETIME2 NOT NULL, -- 報表日期
        [ReportData] NVARCHAR(MAX) NULL, -- 報表資料 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_StoreReports] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_StoreReports_ReportId] ON [dbo].[StoreReports] ([ReportId]);
    CREATE NONCLUSTERED INDEX [IX_StoreReports_ShopId] ON [dbo].[StoreReports] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_StoreReports_ReportDate] ON [dbo].[StoreReports] ([ReportDate]);
END
GO


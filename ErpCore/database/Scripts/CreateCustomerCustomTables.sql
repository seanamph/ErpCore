-- =============================================
-- 客戶定制模組類 (CUS3000系列) - 資料庫腳本
-- 功能：客戶定制模組系列功能
-- 建立日期：2025-01-09
-- =============================================

-- 1. CUS3000 會員主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cus3000Members]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cus3000Members] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [MemberId] NVARCHAR(50) NOT NULL, -- 會員編號
        [MemberName] NVARCHAR(100) NOT NULL, -- 會員姓名
        [CardNo] NVARCHAR(50) NULL, -- 會員卡號
        [Phone] NVARCHAR(20) NULL, -- 電話
        [Email] NVARCHAR(100) NULL, -- 電子郵件
        [Address] NVARCHAR(500) NULL, -- 地址
        [BirthDate] DATETIME2 NULL, -- 出生日期
        [Gender] NVARCHAR(10) NULL, -- 性別 (M:男, F:女)
        [PhotoPath] NVARCHAR(500) NULL, -- 照片路徑
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Cus3000Members_MemberId] UNIQUE ([MemberId]),
        CONSTRAINT [UQ_Cus3000Members_CardNo] UNIQUE ([CardNo])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Cus3000Members_MemberId] ON [dbo].[Cus3000Members] ([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_Cus3000Members_CardNo] ON [dbo].[Cus3000Members] ([CardNo]);
    CREATE NONCLUSTERED INDEX [IX_Cus3000Members_Status] ON [dbo].[Cus3000Members] ([Status]);
END
GO

-- 2. CUS3000 促銷活動主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cus3000Promotions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cus3000Promotions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PromotionId] NVARCHAR(50) NOT NULL, -- 促銷活動編號
        [PromotionName] NVARCHAR(100) NOT NULL, -- 促銷活動名稱
        [StartDate] DATETIME2 NOT NULL, -- 開始日期
        [EndDate] DATETIME2 NOT NULL, -- 結束日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Cus3000Promotions_PromotionId] UNIQUE ([PromotionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Cus3000Promotions_PromotionId] ON [dbo].[Cus3000Promotions] ([PromotionId]);
    CREATE NONCLUSTERED INDEX [IX_Cus3000Promotions_Status] ON [dbo].[Cus3000Promotions] ([Status]);
END
GO

-- 3. CUS3000 活動主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cus3000Activities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cus3000Activities] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ActivityId] NVARCHAR(50) NOT NULL, -- 活動編號
        [ActivityName] NVARCHAR(100) NOT NULL, -- 活動名稱
        [ActivityDate] DATETIME2 NOT NULL, -- 活動日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Cus3000Activities_ActivityId] UNIQUE ([ActivityId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Cus3000Activities_ActivityId] ON [dbo].[Cus3000Activities] ([ActivityId]);
    CREATE NONCLUSTERED INDEX [IX_Cus3000Activities_Status] ON [dbo].[Cus3000Activities] ([Status]);
END
GO

-- 4. CUS3000.ESKYLAND 會員主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cus3000EskylandMembers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cus3000EskylandMembers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [MemberId] NVARCHAR(50) NOT NULL, -- 會員編號
        [MemberName] NVARCHAR(100) NOT NULL, -- 會員姓名
        [CardNo] NVARCHAR(50) NULL, -- 會員卡號
        [EskylandSpecificField] NVARCHAR(200) NULL, -- ESKYLAND特定欄位
        [Phone] NVARCHAR(20) NULL, -- 電話
        [Email] NVARCHAR(100) NULL, -- 電子郵件
        [Address] NVARCHAR(500) NULL, -- 地址
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Cus3000EskylandMembers_MemberId] UNIQUE ([MemberId]),
        CONSTRAINT [UQ_Cus3000EskylandMembers_CardNo] UNIQUE ([CardNo])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Cus3000EskylandMembers_MemberId] ON [dbo].[Cus3000EskylandMembers] ([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_Cus3000EskylandMembers_CardNo] ON [dbo].[Cus3000EskylandMembers] ([CardNo]);
    CREATE NONCLUSTERED INDEX [IX_Cus3000EskylandMembers_Status] ON [dbo].[Cus3000EskylandMembers] ([Status]);
END
GO

-- 5. CUS5000.ESKYLAND 資料主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cus5000EskylandData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cus5000EskylandData] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL,
        [DataName] NVARCHAR(100) NOT NULL,
        [EskylandSpecificField] NVARCHAR(200) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Cus5000EskylandData_DataId] UNIQUE ([DataId])
    );
    CREATE NONCLUSTERED INDEX [IX_Cus5000EskylandData_DataId] ON [dbo].[Cus5000EskylandData] ([DataId]);
END
GO

-- 6. CUSBACKUP 客戶模組備份主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CusBackupData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CusBackupData] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [BackupId] NVARCHAR(50) NOT NULL,
        [CustomerCode] NVARCHAR(50) NOT NULL,
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [BackupVersion] NVARCHAR(50) NULL,
        [BackupDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [BackupPath] NVARCHAR(500) NULL,
        [BackupType] NVARCHAR(20) NULL,
        [FileCount] INT NULL DEFAULT 0,
        [FileSize] BIGINT NULL DEFAULT 0,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [Description] NVARCHAR(1000) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_CusBackupData_BackupId] UNIQUE ([BackupId])
    );
    CREATE NONCLUSTERED INDEX [IX_CusBackupData_BackupId] ON [dbo].[CusBackupData] ([BackupId]);
END
GO

-- 7. CUSCTS CTS客戶資料主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CusCtsData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CusCtsData] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL,
        [DataName] NVARCHAR(100) NOT NULL,
        [CtsSpecificField] NVARCHAR(200) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_CusCtsData_DataId] UNIQUE ([DataId])
    );
    CREATE NONCLUSTERED INDEX [IX_CusCtsData_DataId] ON [dbo].[CusCtsData] ([DataId]);
END
GO

-- 8. CUSHANSHIN 阪神客戶資料主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CusHanshinData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CusHanshinData] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL,
        [DataName] NVARCHAR(100) NOT NULL,
        [HanshinSpecificField] NVARCHAR(200) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_CusHanshinData_DataId] UNIQUE ([DataId])
    );
    CREATE NONCLUSTERED INDEX [IX_CusHanshinData_DataId] ON [dbo].[CusHanshinData] ([DataId]);
END
GO

-- 9. SYS8000.ESKYLAND 租賃主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sys8000EskylandLeases]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Sys8000EskylandLeases] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [LeaseId] NVARCHAR(50) NOT NULL,
        [LeaseName] NVARCHAR(100) NOT NULL,
        [EskylandSpecificField] NVARCHAR(200) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Sys8000EskylandLeases_LeaseId] UNIQUE ([LeaseId])
    );
    CREATE NONCLUSTERED INDEX [IX_Sys8000EskylandLeases_LeaseId] ON [dbo].[Sys8000EskylandLeases] ([LeaseId]);
END
GO


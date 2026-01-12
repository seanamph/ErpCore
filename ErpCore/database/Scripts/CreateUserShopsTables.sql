-- SYS0113 - 使用者基本資料維護(分店廠商部門)作業
-- 建立 UserShops, UserVendors, UserDepartments 相關資料表

-- 1. 建立 UserShops 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserShops]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserShops] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [PShopId] NVARCHAR(50) NULL, -- 總公司代號
        [ShopId] NVARCHAR(50) NULL, -- 分店代號
        [SiteId] NVARCHAR(50) NULL, -- 據點代號
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserShops_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserShops_UserId] ON [dbo].[UserShops] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserShops_PShopId] ON [dbo].[UserShops] ([PShopId]);
    CREATE NONCLUSTERED INDEX [IX_UserShops_ShopId] ON [dbo].[UserShops] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_UserShops_SiteId] ON [dbo].[UserShops] ([SiteId]);
END
GO

-- 2. 建立 UserVendors 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserVendors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserVendors] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [VendorId] NVARCHAR(50) NOT NULL, -- 廠商代號
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserVendors_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserVendors_UserId] ON [dbo].[UserVendors] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserVendors_VendorId] ON [dbo].[UserVendors] ([VendorId]);
END
GO

-- 3. 建立 UserDepartments 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserDepartments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserDepartments] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [DeptId] NVARCHAR(50) NOT NULL, -- 部門代號
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserDepartments_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserDepartments_UserId] ON [dbo].[UserDepartments] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserDepartments_DeptId] ON [dbo].[UserDepartments] ([DeptId]);
END
GO

-- 4. 確認 UserButtons 資料表存在（如果不存在則建立）
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserButtons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserButtons] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [ButtonId] NVARCHAR(50) NOT NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedPriority] INT NULL,
        [CreatedGroup] NVARCHAR(50) NULL,
        CONSTRAINT [FK_UserButtons_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserButtons_UserId] ON [dbo].[UserButtons] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserButtons_ButtonId] ON [dbo].[UserButtons] ([ButtonId]);
END
GO

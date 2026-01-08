-- =============================================
-- 角色基本資料維護資料表建立腳本
-- 功能代碼: SYS0210
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 建立 Roles 主表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [RoleId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 角色代碼
        [RoleName] NVARCHAR(100) NOT NULL, -- 角色名稱
        [RoleNote] NVARCHAR(500) NULL, -- 角色敘述
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        [CreatedPriority] INT NULL, -- 建立者等級
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組
        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Roles_RoleName] ON [dbo].[Roles] ([RoleName]);

    PRINT 'Roles 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Roles 資料表已存在';
END
GO

-- 2. 建立 UserRoles 使用者角色對應表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserRoles] (
        [UserId] NVARCHAR(50) NOT NULL, -- 使用者編號
        [RoleId] NVARCHAR(50) NOT NULL, -- 角色代碼
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);

    PRINT 'UserRoles 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'UserRoles 資料表已存在';
END
GO

-- 3. 建立 RolePermissions 角色權限對應表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RolePermissions] (
        [RoleId] NVARCHAR(50) NOT NULL, -- 角色代碼
        [SystemId] NVARCHAR(50) NOT NULL, -- 系統代碼
        [ProgramId] NVARCHAR(50) NOT NULL, -- 作業代碼
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([RoleId], [SystemId], [ProgramId]),
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_RolePermissions_RoleId] ON [dbo].[RolePermissions] ([RoleId]);
    CREATE NONCLUSTERED INDEX [IX_RolePermissions_SystemId] ON [dbo].[RolePermissions] ([SystemId]);
    CREATE NONCLUSTERED INDEX [IX_RolePermissions_ProgramId] ON [dbo].[RolePermissions] ([ProgramId]);

    PRINT 'RolePermissions 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'RolePermissions 資料表已存在';
END
GO


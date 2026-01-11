-- =============================================
-- 使用者之角色設定維護資料表建立腳本
-- 功能代碼: SYS0220
-- 建立日期: 2025-01-27
-- 說明: 為指定使用者分配或移除角色，管理使用者與角色的對應關係
-- =============================================

-- 1. 建立 UserRoles 使用者角色對應表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserRoles] (
        [UserId] NVARCHAR(50) NOT NULL, -- 使用者代碼
        [RoleId] NVARCHAR(50) NOT NULL, -- 角色代碼
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NULL, -- 更新時間
        [CreatedPriority] INT NULL, -- 建立者等級
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組
        CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);

    PRINT 'UserRoles 資料表建立成功 (SYS0220)';
END
ELSE
BEGIN
    PRINT 'UserRoles 資料表已存在，開始檢查欄位完整性...';
    
    -- 檢查並添加缺失的欄位
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND name = 'UpdatedBy')
    BEGIN
        ALTER TABLE [dbo].[UserRoles] ADD [UpdatedBy] NVARCHAR(50) NULL;
        PRINT 'UserRoles 資料表已添加 UpdatedBy 欄位';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND name = 'UpdatedAt')
    BEGIN
        ALTER TABLE [dbo].[UserRoles] ADD [UpdatedAt] DATETIME2 NULL;
        PRINT 'UserRoles 資料表已添加 UpdatedAt 欄位';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND name = 'CreatedPriority')
    BEGIN
        ALTER TABLE [dbo].[UserRoles] ADD [CreatedPriority] INT NULL;
        PRINT 'UserRoles 資料表已添加 CreatedPriority 欄位';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND name = 'CreatedGroup')
    BEGIN
        ALTER TABLE [dbo].[UserRoles] ADD [CreatedGroup] NVARCHAR(50) NULL;
        PRINT 'UserRoles 資料表已添加 CreatedGroup 欄位';
    END

    -- 檢查並添加缺失的外鍵約束
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserRoles_Users')
    BEGIN
        ALTER TABLE [dbo].[UserRoles] 
        ADD CONSTRAINT [FK_UserRoles_Users] 
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE;
        PRINT 'UserRoles 資料表已添加 FK_UserRoles_Users 外鍵約束';
    END

    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserRoles_Roles')
    BEGIN
        ALTER TABLE [dbo].[UserRoles] 
        ADD CONSTRAINT [FK_UserRoles_Roles] 
        FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE;
        PRINT 'UserRoles 資料表已添加 FK_UserRoles_Roles 外鍵約束';
    END

    -- 檢查並添加缺失的索引
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserRoles_UserId' AND object_id = OBJECT_ID(N'[dbo].[UserRoles]'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles] ([UserId]);
        PRINT 'UserRoles 資料表已添加 IX_UserRoles_UserId 索引';
    END

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserRoles_RoleId' AND object_id = OBJECT_ID(N'[dbo].[UserRoles]'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);
        PRINT 'UserRoles 資料表已添加 IX_UserRoles_RoleId 索引';
    END

    PRINT 'UserRoles 資料表欄位檢查完成 (SYS0220)';
END
GO

-- =============================================
-- 資料字典
-- =============================================
-- 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註
-- ---------|---------|------|---------|--------|------|------
-- UserId   | NVARCHAR | 50  | NO      | -      | 使用者代碼 | 主鍵，外鍵至 Users
-- RoleId   | NVARCHAR | 50  | NO      | -      | 角色代碼 | 主鍵，外鍵至 Roles
-- CreatedBy| NVARCHAR | 50  | YES     | -      | 建立者 | -
-- CreatedAt| DATETIME2| -   | NO      | GETDATE() | 建立時間 | -
-- UpdatedBy| NVARCHAR | 50  | YES     | -      | 更新者 | -
-- UpdatedAt| DATETIME2| -   | YES     | -      | 更新時間 | -
-- CreatedPriority | INT | - | YES     | -      | 建立者等級 | -
-- CreatedGroup | NVARCHAR | 50 | YES | -      | 建立者群組 | -
-- =============================================

PRINT 'SYS0220 - 使用者之角色設定維護資料表建立腳本執行完成';
GO

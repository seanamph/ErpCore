-- 權限管理相關資料表
-- SYS0310 - 角色系統權限設定
-- SYS0320 - 使用者系統權限設定

-- 1. RoleButtons - 角色按鈕權限表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleButtons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RoleButtons] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [RoleId] NVARCHAR(50) NOT NULL,
        [ButtonId] NVARCHAR(50) NOT NULL, -- 對應到 ConfigButtons 表的 ButtonId
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedPriority] INT NULL,
        [CreatedGroup] NVARCHAR(50) NULL,
        CONSTRAINT [PK_RoleButtons] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [FK_RoleButtons_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE,
        CONSTRAINT [FK_RoleButtons_ConfigButtons] FOREIGN KEY ([ButtonId]) REFERENCES [dbo].[ConfigButtons] ([ButtonId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_RoleButtons_RoleId] ON [dbo].[RoleButtons] ([RoleId]);
    CREATE NONCLUSTERED INDEX [IX_RoleButtons_ButtonId] ON [dbo].[RoleButtons] ([ButtonId]);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_RoleButtons_RoleId_ButtonId] ON [dbo].[RoleButtons] ([RoleId], [ButtonId]);

    PRINT 'RoleButtons 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'RoleButtons 資料表已存在';
END
GO

-- 2. UserButtons - 使用者按鈕權限表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserButtons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserButtons] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [ButtonId] NVARCHAR(50) NOT NULL, -- 對應到 ConfigButtons 表的 ButtonId
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedPriority] INT NULL,
        [CreatedGroup] NVARCHAR(50) NULL,
        CONSTRAINT [PK_UserButtons] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [FK_UserButtons_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserButtons_ConfigButtons] FOREIGN KEY ([ButtonId]) REFERENCES [dbo].[ConfigButtons] ([ButtonId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserButtons_UserId] ON [dbo].[UserButtons] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserButtons_ButtonId] ON [dbo].[UserButtons] ([ButtonId]);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_UserButtons_UserId_ButtonId] ON [dbo].[UserButtons] ([UserId], [ButtonId]);

    PRINT 'UserButtons 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'UserButtons 資料表已存在';
END
GO


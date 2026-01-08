-- SYS0110 - 使用者基本資料維護作業
-- 建立 Users 相關資料表

-- 1. 建立 Users 主表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 使用者編號
        [UserName] NVARCHAR(100) NOT NULL, -- 使用者名稱
        [UserPassword] NVARCHAR(255) NULL, -- 使用者密碼 (加密儲存)
        [Title] NVARCHAR(50) NULL, -- 職稱
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼
        [StartDate] DATETIME2 NULL, -- 帳號有效起始日
        [EndDate] DATETIME2 NULL, -- 帳號終止日
        [LastLoginDate] DATETIME2 NULL, -- 上次登入時間
        [LastLoginIp] NVARCHAR(50) NULL, -- 上次登入IP
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 帳號狀態 A:啟用, I:停用, L:鎖定
        [UserType] NVARCHAR(20) NULL, -- 使用者型態
        [Notes] NVARCHAR(500) NULL, -- 備註
        [UserPriority] INT NULL DEFAULT 0, -- 使用者等級
        [ShopId] NVARCHAR(50) NULL, -- 所屬分店
        [LoginCount] INT NULL DEFAULT 0, -- 登入次數
        [ChangePwdDate] DATETIME2 NULL, -- 密碼變更日期
        [FloorId] NVARCHAR(50) NULL, -- 樓層代碼
        [AreaId] NVARCHAR(50) NULL, -- 區域別代碼
        [BtypeId] NVARCHAR(50) NULL, -- 業種代碼
        [StoreId] NVARCHAR(50) NULL, -- 專櫃代碼
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        [CreatedPriority] INT NULL, -- 建立者等級
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Users_OrgId] ON [dbo].[Users] ([OrgId]);
    CREATE NONCLUSTERED INDEX [IX_Users_Status] ON [dbo].[Users] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Users_UserType] ON [dbo].[Users] ([UserType]);
    CREATE NONCLUSTERED INDEX [IX_Users_ShopId] ON [dbo].[Users] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_Users_UserName] ON [dbo].[Users] ([UserName]);

    PRINT 'Users 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Users 資料表已存在';
END
GO

-- 2. 建立 UserRoles 使用者角色對應表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserRoles] (
        [UserId] NVARCHAR(50) NOT NULL, -- 使用者編號
        [RoleId] NVARCHAR(50) NOT NULL, -- 角色編號
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);

    PRINT 'UserRoles 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'UserRoles 資料表已存在';
END
GO

-- 3. 建立 UserButtons 使用者按鈕權限表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserButtons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserButtons] (
        [UserId] NVARCHAR(50) NOT NULL, -- 使用者編號
        [ButtonId] NVARCHAR(50) NOT NULL, -- 按鈕編號
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [PK_UserButtons] PRIMARY KEY CLUSTERED ([UserId], [ButtonId]),
        CONSTRAINT [FK_UserButtons_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserButtons_ButtonId] ON [dbo].[UserButtons] ([ButtonId]);

    PRINT 'UserButtons 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'UserButtons 資料表已存在';
END
GO

-- 4. 建立 UserAgent 使用者權限代理表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAgent]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserAgent] (
        [AgentId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), -- 代理編號
        [PrincipalUserId] NVARCHAR(50) NOT NULL, -- 委託人
        [AgentUserId] NVARCHAR(50) NOT NULL, -- 代理人
        [BeginTime] DATETIME2 NOT NULL, -- 開始時間
        [EndTime] DATETIME2 NOT NULL, -- 結束時間
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [FK_UserAgent_PrincipalUser] FOREIGN KEY ([PrincipalUserId]) REFERENCES [dbo].[Users] ([UserId]),
        CONSTRAINT [FK_UserAgent_AgentUser] FOREIGN KEY ([AgentUserId]) REFERENCES [dbo].[Users] ([UserId]),
        CONSTRAINT [CK_UserAgent_TimeRange] CHECK ([EndTime] > [BeginTime]),
        CONSTRAINT [CK_UserAgent_DifferentUser] CHECK ([PrincipalUserId] <> [AgentUserId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserAgent_PrincipalUserId] ON [dbo].[UserAgent] ([PrincipalUserId]);
    CREATE NONCLUSTERED INDEX [IX_UserAgent_AgentUserId] ON [dbo].[UserAgent] ([AgentUserId]);
    CREATE NONCLUSTERED INDEX [IX_UserAgent_Status] ON [dbo].[UserAgent] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_UserAgent_TimeRange] ON [dbo].[UserAgent] ([BeginTime], [EndTime]);

    PRINT 'UserAgent 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'UserAgent 資料表已存在';
END
GO


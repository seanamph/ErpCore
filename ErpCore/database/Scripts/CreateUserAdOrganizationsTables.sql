-- SYS0114 - 使用者基本資料維護(ActiveDirectory)作業
-- 修改 Users 表添加 AD 欄位，建立 UserOrganizations 資料表

-- 1. 修改 Users 表，新增 Active Directory 相關欄位
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'UseActiveDirectory')
BEGIN
    ALTER TABLE [dbo].[Users] ADD 
        [UseActiveDirectory] BIT NOT NULL DEFAULT 0, -- 是否使用Active Directory
        [AdDomain] NVARCHAR(100) NULL, -- AD網域
        [AdUserPrincipalName] NVARCHAR(255) NULL; -- AD使用者主體名稱

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Users_UseActiveDirectory] ON [dbo].[Users] ([UseActiveDirectory]);
    CREATE NONCLUSTERED INDEX [IX_Users_AdUserPrincipalName] ON [dbo].[Users] ([AdUserPrincipalName]);
END
GO

-- 2. 建立 UserOrganizations 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOrganizations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserOrganizations] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [OrgId] NVARCHAR(50) NOT NULL, -- 組織代號
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserOrganizations_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserOrganizations_UserId] ON [dbo].[UserOrganizations] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserOrganizations_OrgId] ON [dbo].[UserOrganizations] ([OrgId]);
END
GO

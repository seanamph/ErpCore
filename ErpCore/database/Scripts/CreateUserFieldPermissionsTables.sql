-- 使用者欄位權限管理相關資料表
-- SYS0340 - 使用者欄位權限設定

-- 注意：DatabaseInfo、TableInfo、FieldInfo 表可能已由 SYS0330 建立
-- 如果不存在，請先執行 CreateRoleFieldPermissionTables.sql

-- UserFieldPermissions - 使用者欄位權限表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserFieldPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserFieldPermissions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [UserId] NVARCHAR(50) NOT NULL,
        [DbName] NVARCHAR(100) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [FieldName] NVARCHAR(100) NOT NULL,
        [PermissionType] NVARCHAR(20) NOT NULL, -- READ, WRITE, HIDE
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserFieldPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [UK_UserFieldPermissions] UNIQUE ([UserId], [DbName], [TableName], [FieldName])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserFieldPermissions_UserId] ON [dbo].[UserFieldPermissions] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserFieldPermissions_DbTable] ON [dbo].[UserFieldPermissions] ([DbName], [TableName]);
    CREATE NONCLUSTERED INDEX [IX_UserFieldPermissions_PermissionType] ON [dbo].[UserFieldPermissions] ([PermissionType]);

    PRINT 'UserFieldPermissions 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'UserFieldPermissions 資料表已存在';
END
GO

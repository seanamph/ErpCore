-- 角色欄位權限管理相關資料表
-- SYS0330 - 角色欄位權限設定

-- 1. DatabaseInfo - 資料庫資訊表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatabaseInfo]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DatabaseInfo] (
        [DbName] NVARCHAR(100) NOT NULL PRIMARY KEY,
        [DbDescription] NVARCHAR(200) NULL,
        [ConnectionString] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_DatabaseInfo_IsActive] ON [dbo].[DatabaseInfo] ([IsActive]);

    PRINT 'DatabaseInfo 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'DatabaseInfo 資料表已存在';
END
GO

-- 2. TableInfo - 表格資訊表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TableInfo]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TableInfo] (
        [DbName] NVARCHAR(100) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [TableDescription] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_TableInfo] PRIMARY KEY CLUSTERED ([DbName], [TableName]),
        CONSTRAINT [FK_TableInfo_DatabaseInfo] FOREIGN KEY ([DbName]) REFERENCES [dbo].[DatabaseInfo] ([DbName]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TableInfo_DbName] ON [dbo].[TableInfo] ([DbName]);
    CREATE NONCLUSTERED INDEX [IX_TableInfo_IsActive] ON [dbo].[TableInfo] ([IsActive]);

    PRINT 'TableInfo 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'TableInfo 資料表已存在';
END
GO

-- 3. FieldInfo - 欄位資訊表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldInfo]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FieldInfo] (
        [DbName] NVARCHAR(100) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [FieldName] NVARCHAR(100) NOT NULL,
        [FieldType] NVARCHAR(50) NULL,
        [FieldLength] INT NULL,
        [FieldDescription] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_FieldInfo] PRIMARY KEY CLUSTERED ([DbName], [TableName], [FieldName]),
        CONSTRAINT [FK_FieldInfo_TableInfo] FOREIGN KEY ([DbName], [TableName]) REFERENCES [dbo].[TableInfo] ([DbName], [TableName]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_FieldInfo_DbTable] ON [dbo].[FieldInfo] ([DbName], [TableName]);
    CREATE NONCLUSTERED INDEX [IX_FieldInfo_IsActive] ON [dbo].[FieldInfo] ([IsActive]);

    PRINT 'FieldInfo 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'FieldInfo 資料表已存在';
END
GO

-- 4. RoleFieldPermissions - 角色欄位權限表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleFieldPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RoleFieldPermissions] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [RoleId] NVARCHAR(50) NOT NULL,
        [DbName] NVARCHAR(100) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [FieldName] NVARCHAR(100) NOT NULL,
        [PermissionType] NVARCHAR(20) NOT NULL, -- READ, WRITE, HIDE
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_RoleFieldPermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE,
        CONSTRAINT [UK_RoleFieldPermissions] UNIQUE ([RoleId], [DbName], [TableName], [FieldName])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_RoleFieldPermissions_RoleId] ON [dbo].[RoleFieldPermissions] ([RoleId]);
    CREATE NONCLUSTERED INDEX [IX_RoleFieldPermissions_DbTable] ON [dbo].[RoleFieldPermissions] ([DbName], [TableName]);
    CREATE NONCLUSTERED INDEX [IX_RoleFieldPermissions_PermissionType] ON [dbo].[RoleFieldPermissions] ([PermissionType]);

    PRINT 'RoleFieldPermissions 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'RoleFieldPermissions 資料表已存在';
END
GO


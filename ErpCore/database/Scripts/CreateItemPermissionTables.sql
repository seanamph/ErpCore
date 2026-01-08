-- 系統對應權限管理相關資料表
-- SYS0360 - 系統對應權限設定

-- 1. ItemCorresponds - 項目對應主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemCorresponds]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ItemCorresponds] (
        [ItemId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [ItemName] NVARCHAR(100) NOT NULL,
        [ItemType] NVARCHAR(20) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ItemCorresponds] PRIMARY KEY CLUSTERED ([ItemId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ItemCorresponds_ItemName] ON [dbo].[ItemCorresponds] ([ItemName]);
    CREATE NONCLUSTERED INDEX [IX_ItemCorresponds_ItemType] ON [dbo].[ItemCorresponds] ([ItemType]);
    CREATE NONCLUSTERED INDEX [IX_ItemCorresponds_Status] ON [dbo].[ItemCorresponds] ([Status]);

    PRINT 'ItemCorresponds 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ItemCorresponds 資料表已存在';
END
GO

-- 2. ItemPermissions - 項目權限對應表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ItemPermissions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ItemId] NVARCHAR(50) NOT NULL,
        [ProgramId] NVARCHAR(50) NOT NULL,
        [PageId] NVARCHAR(50) NOT NULL,
        [ButtonId] NVARCHAR(50) NOT NULL,
        [ButtonKey] BIGINT NULL, -- 對應到 ConfigButtons 表的 TKey（如果有的話）
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [CreatedPriority] INT NULL,
        [CreatedGroup] NVARCHAR(50) NULL,
        CONSTRAINT [PK_ItemPermissions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [FK_ItemPermissions_ItemCorresponds] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[ItemCorresponds] ([ItemId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ItemPermissions_ItemId] ON [dbo].[ItemPermissions] ([ItemId]);
    CREATE NONCLUSTERED INDEX [IX_ItemPermissions_ProgramId] ON [dbo].[ItemPermissions] ([ProgramId]);
    CREATE NONCLUSTERED INDEX [IX_ItemPermissions_PageId] ON [dbo].[ItemPermissions] ([PageId]);
    CREATE NONCLUSTERED INDEX [IX_ItemPermissions_ButtonId] ON [dbo].[ItemPermissions] ([ButtonId]);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ItemPermissions_ItemId_ProgramId_PageId_ButtonId] 
        ON [dbo].[ItemPermissions] ([ItemId], [ProgramId], [PageId], [ButtonId]);

    PRINT 'ItemPermissions 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ItemPermissions 資料表已存在';
END
GO


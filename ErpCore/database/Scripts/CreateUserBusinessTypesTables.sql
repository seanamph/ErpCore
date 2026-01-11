-- SYS0111 - 使用者基本資料維護(業種儲位) 作業
-- 建立 UserBusinessTypes、UserWarehouseAreas、UserStores 相關資料表

-- 1. 建立 UserBusinessTypes 資料表 - 使用者業種權限
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserBusinessTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserBusinessTypes] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [BtypeMId] NVARCHAR(50) NULL, -- 業種大分類
        [BtypeId] NVARCHAR(50) NULL, -- 業種中分類
        [PtypeId] NVARCHAR(50) NULL, -- 業種小分類
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserBusinessTypes_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserBusinessTypes_UserId] ON [dbo].[UserBusinessTypes] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserBusinessTypes_BtypeMId] ON [dbo].[UserBusinessTypes] ([BtypeMId]);
    CREATE NONCLUSTERED INDEX [IX_UserBusinessTypes_BtypeId] ON [dbo].[UserBusinessTypes] ([BtypeId]);
    CREATE NONCLUSTERED INDEX [IX_UserBusinessTypes_PtypeId] ON [dbo].[UserBusinessTypes] ([PtypeId]);
END
GO

-- 2. 建立 UserWarehouseAreas 資料表 - 使用者儲位權限
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserWarehouseAreas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserWarehouseAreas] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [WareaId1] NVARCHAR(50) NULL, -- 儲位1
        [WareaId2] NVARCHAR(50) NULL, -- 儲位2
        [WareaId3] NVARCHAR(50) NULL, -- 儲位3
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserWarehouseAreas_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserWarehouseAreas_UserId] ON [dbo].[UserWarehouseAreas] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserWarehouseAreas_WareaId1] ON [dbo].[UserWarehouseAreas] ([WareaId1]);
    CREATE NONCLUSTERED INDEX [IX_UserWarehouseAreas_WareaId2] ON [dbo].[UserWarehouseAreas] ([WareaId2]);
    CREATE NONCLUSTERED INDEX [IX_UserWarehouseAreas_WareaId3] ON [dbo].[UserWarehouseAreas] ([WareaId3]);
END
GO

-- 3. 建立 UserStores 資料表 - 使用者7X承租分店權限
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserStores]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserStores] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UserId] NVARCHAR(50) NOT NULL,
        [StoreId] NVARCHAR(50) NOT NULL, -- 7X承租分店代號
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserStores_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserStores_UserId] ON [dbo].[UserStores] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserStores_StoreId] ON [dbo].[UserStores] ([StoreId]);
    
    -- 唯一約束：同一使用者不能重複設定相同的分店
    CREATE UNIQUE NONCLUSTERED INDEX [IX_UserStores_UserId_StoreId] ON [dbo].[UserStores] ([UserId], [StoreId]);
END
GO

PRINT 'SYS0111 相關資料表建立完成：UserBusinessTypes, UserWarehouseAreas, UserStores';

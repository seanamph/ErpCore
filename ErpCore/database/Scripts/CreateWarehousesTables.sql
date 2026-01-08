-- 庫別資料維護作業資料表 (SYSWB60)
-- 對應舊系統 RIM_WAREHOUSE 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Warehouses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Warehouses] (
        [WarehouseId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 庫別代碼 (WH_ID)
        [WarehouseName] NVARCHAR(100) NOT NULL, -- 庫別名稱 (WH_NAME)
        [WarehouseType] NVARCHAR(20) NULL, -- 庫別類型 (WH_TYPE)
        [Location] NVARCHAR(200) NULL, -- 庫別位置 (LOCATION)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Warehouses] PRIMARY KEY CLUSTERED ([WarehouseId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Warehouses_WarehouseName] ON [dbo].[Warehouses] ([WarehouseName]);
    CREATE NONCLUSTERED INDEX [IX_Warehouses_WarehouseType] ON [dbo].[Warehouses] ([WarehouseType]);
    CREATE NONCLUSTERED INDEX [IX_Warehouses_Status] ON [dbo].[Warehouses] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Warehouses_SeqNo] ON [dbo].[Warehouses] ([SeqNo]);
    
    PRINT 'Warehouses 表建立成功';
END
ELSE
BEGIN
    PRINT 'Warehouses 表已存在';
END
GO


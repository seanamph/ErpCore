-- =============================================
-- 租戶位置資料表建立腳本 (SYSC999)
-- 功能代碼: SYSC999
-- 建立日期: 2025-01-10
-- =============================================

-- 租戶位置主檔 (TenantLocations)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TenantLocations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TenantLocations] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [AgmTKey] BIGINT NOT NULL, -- 租戶主檔主鍵 (AGM_T_KEY)
        [LocationId] NVARCHAR(50) NOT NULL, -- 位置代碼 (LOCATION_ID)
        [AreaId] NVARCHAR(50) NULL, -- 區域代碼 (AREA_ID)
        [FloorId] NVARCHAR(50) NULL, -- 樓層代碼 (FLOOR_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_TenantLocations] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TenantLocations_AgmTKey] ON [dbo].[TenantLocations] ([AgmTKey]);
    CREATE NONCLUSTERED INDEX [IX_TenantLocations_LocationId] ON [dbo].[TenantLocations] ([LocationId]);
    CREATE NONCLUSTERED INDEX [IX_TenantLocations_AreaId] ON [dbo].[TenantLocations] ([AreaId]);
    CREATE NONCLUSTERED INDEX [IX_TenantLocations_FloorId] ON [dbo].[TenantLocations] ([FloorId]);
    CREATE NONCLUSTERED INDEX [IX_TenantLocations_Status] ON [dbo].[TenantLocations] ([Status]);
    
    -- 外鍵約束 (如果相關表存在)
    -- 注意: 以下外鍵約束需要根據實際資料表結構調整
    -- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tenants]') AND type in (N'U'))
    -- BEGIN
    --     ALTER TABLE [dbo].[TenantLocations]
    --     ADD CONSTRAINT [FK_TenantLocations_Tenants] 
    --     FOREIGN KEY ([AgmTKey]) REFERENCES [dbo].[Tenants] ([TKey]) ON DELETE CASCADE;
    -- END
    
    -- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locations]') AND type in (N'U'))
    -- BEGIN
    --     ALTER TABLE [dbo].[TenantLocations]
    --     ADD CONSTRAINT [FK_TenantLocations_Locations] 
    --     FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([LocationId]);
    -- END
    
    PRINT 'TenantLocations 表建立成功';
END
ELSE
BEGIN
    PRINT 'TenantLocations 表已存在';
END
GO


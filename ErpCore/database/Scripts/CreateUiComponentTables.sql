-- =============================================
-- UI組件類資料表建立腳本
-- =============================================

-- 1. UI組件設定 (UIComponents)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UIComponents]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UIComponents] (
        [ComponentId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ComponentCode] NVARCHAR(50) NOT NULL,
        [ComponentName] NVARCHAR(200) NOT NULL,
        [ComponentType] NVARCHAR(20) NOT NULL,
        [ComponentVersion] NVARCHAR(10) NOT NULL DEFAULT 'V1',
        [ConfigJson] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_UIComponents_Code_Version] UNIQUE ([ComponentCode], [ComponentVersion])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UIComponents_ComponentCode] ON [dbo].[UIComponents] ([ComponentCode]);
    CREATE NONCLUSTERED INDEX [IX_UIComponents_ComponentType] ON [dbo].[UIComponents] ([ComponentType]);
    CREATE NONCLUSTERED INDEX [IX_UIComponents_Status] ON [dbo].[UIComponents] ([Status]);
END
GO

-- 2. UI組件使用記錄 (UIComponentUsages)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UIComponentUsages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UIComponentUsages] (
        [UsageId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ComponentId] BIGINT NOT NULL,
        [ModuleCode] NVARCHAR(50) NOT NULL,
        [ModuleName] NVARCHAR(200) NULL,
        [UsageCount] INT NOT NULL DEFAULT 0,
        [LastUsedAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UIComponentUsages_UIComponents] FOREIGN KEY ([ComponentId]) REFERENCES [dbo].[UIComponents] ([ComponentId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UIComponentUsages_ComponentId] ON [dbo].[UIComponentUsages] ([ComponentId]);
    CREATE NONCLUSTERED INDEX [IX_UIComponentUsages_ModuleCode] ON [dbo].[UIComponentUsages] ([ModuleCode]);
END
GO

-- 3. UI組件使用統計視圖 (V_UIComponentUsageStats)
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_UIComponentUsageStats]'))
    DROP VIEW [dbo].[V_UIComponentUsageStats];
GO

CREATE VIEW [dbo].[V_UIComponentUsageStats] AS
SELECT 
    c.ComponentId,
    c.ComponentCode,
    c.ComponentName,
    c.ComponentType,
    c.ComponentVersion,
    COUNT(u.UsageId) AS TotalUsageCount,
    COUNT(DISTINCT u.ModuleCode) AS UsedModuleCount,
    MAX(u.LastUsedAt) AS LastUsedAt,
    MIN(u.CreatedAt) AS FirstUsedAt
FROM [dbo].[UIComponents] c
LEFT JOIN [dbo].[UIComponentUsages] u ON c.ComponentId = u.ComponentId
WHERE c.Status = '1'
GROUP BY 
    c.ComponentId,
    c.ComponentCode,
    c.ComponentName,
    c.ComponentType,
    c.ComponentVersion;
GO


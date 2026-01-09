-- =============================================
-- 能源管理類 (SYSO000) - 資料庫腳本
-- 功能：能源基礎功能 (SYSO100-SYSO130)、能源處理功能 (SYSO310)、能源擴展功能 (SYSOU10-SYSOU33)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 能源基礎資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnergyBases]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EnergyBases] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [EnergyId] NVARCHAR(50) NOT NULL, -- 能源編號
        [EnergyName] NVARCHAR(100) NOT NULL, -- 能源名稱
        [EnergyType] NVARCHAR(20) NULL, -- 能源類型 (ELECTRICITY:電力, WATER:水, GAS:瓦斯, OTHER:其他)
        [Unit] NVARCHAR(20) NULL, -- 單位 (KWH, M3, LITER等)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_EnergyBases_EnergyId] UNIQUE ([EnergyId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EnergyBases_EnergyId] ON [dbo].[EnergyBases] ([EnergyId]);
    CREATE NONCLUSTERED INDEX [IX_EnergyBases_EnergyType] ON [dbo].[EnergyBases] ([EnergyType]);
    CREATE NONCLUSTERED INDEX [IX_EnergyBases_Status] ON [dbo].[EnergyBases] ([Status]);
END
GO

-- 2. 能源處理記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnergyProcesses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EnergyProcesses] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號
        [EnergyId] NVARCHAR(50) NOT NULL, -- 能源編號
        [ProcessDate] DATETIME2 NOT NULL, -- 處理日期
        [ProcessType] NVARCHAR(20) NULL, -- 處理類型
        [Amount] DECIMAL(18,2) NULL, -- 數量
        [Cost] DECIMAL(18,2) NULL, -- 成本
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_EnergyProcesses_ProcessId] UNIQUE ([ProcessId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EnergyProcesses_ProcessId] ON [dbo].[EnergyProcesses] ([ProcessId]);
    CREATE NONCLUSTERED INDEX [IX_EnergyProcesses_EnergyId] ON [dbo].[EnergyProcesses] ([EnergyId]);
    CREATE NONCLUSTERED INDEX [IX_EnergyProcesses_ProcessDate] ON [dbo].[EnergyProcesses] ([ProcessDate]);
END
GO

-- 3. 能源擴展資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EnergyExtensions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EnergyExtensions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號
        [EnergyId] NVARCHAR(50) NOT NULL, -- 能源編號
        [ExtensionType] NVARCHAR(20) NULL, -- 擴展類型
        [ExtensionValue] NVARCHAR(500) NULL, -- 擴展值
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_EnergyExtensions_ExtensionId] UNIQUE ([ExtensionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EnergyExtensions_ExtensionId] ON [dbo].[EnergyExtensions] ([ExtensionId]);
    CREATE NONCLUSTERED INDEX [IX_EnergyExtensions_EnergyId] ON [dbo].[EnergyExtensions] ([EnergyId]);
END
GO


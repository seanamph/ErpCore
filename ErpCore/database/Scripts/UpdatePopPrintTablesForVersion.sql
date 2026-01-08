-- 更新 PopPrintSettings 表，添加 Version 和 ApSpecificSettings 字段
-- 用於支援 SYSW171 (AP版本) 和 SYSW172 (UA版本)

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PopPrintSettings]') AND name = 'Version')
BEGIN
    ALTER TABLE [dbo].[PopPrintSettings]
    ADD [Version] NVARCHAR(20) NULL DEFAULT 'STANDARD';
    
    PRINT '已添加 Version 欄位到 PopPrintSettings 表';
END
ELSE
BEGIN
    PRINT 'PopPrintSettings 表已存在 Version 欄位';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PopPrintSettings]') AND name = 'ApSpecificSettings')
BEGIN
    ALTER TABLE [dbo].[PopPrintSettings]
    ADD [ApSpecificSettings] NVARCHAR(MAX) NULL;
    
    PRINT '已添加 ApSpecificSettings 欄位到 PopPrintSettings 表';
END
ELSE
BEGIN
    PRINT 'PopPrintSettings 表已存在 ApSpecificSettings 欄位';
END
GO

-- 更新 PopPrintLogs 表，添加 Version 字段
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PopPrintLogs]') AND name = 'Version')
BEGIN
    ALTER TABLE [dbo].[PopPrintLogs]
    ADD [Version] NVARCHAR(20) NULL DEFAULT 'STANDARD';
    
    PRINT '已添加 Version 欄位到 PopPrintLogs 表';
END
ELSE
BEGIN
    PRINT 'PopPrintLogs 表已存在 Version 欄位';
END
GO

-- 建立索引以提升查詢效能
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PopPrintSettings_ShopId_Version' AND object_id = OBJECT_ID(N'[dbo].[PopPrintSettings]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_PopPrintSettings_ShopId_Version]
    ON [dbo].[PopPrintSettings] ([ShopId], [Version]);
    
    PRINT '已建立索引 IX_PopPrintSettings_ShopId_Version';
END
ELSE
BEGIN
    PRINT '索引 IX_PopPrintSettings_ShopId_Version 已存在';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PopPrintLogs_Version' AND object_id = OBJECT_ID(N'[dbo].[PopPrintLogs]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_Version]
    ON [dbo].[PopPrintLogs] ([Version]);
    
    PRINT '已建立索引 IX_PopPrintLogs_Version';
END
ELSE
BEGIN
    PRINT '索引 IX_PopPrintLogs_Version 已存在';
END
GO

PRINT 'PopPrint 表版本更新完成';


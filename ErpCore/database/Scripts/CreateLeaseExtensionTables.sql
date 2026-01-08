-- =============================================
-- 租賃擴展管理類資料表建立腳本
-- 功能代碼: SYS8A10-SYS8A45
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 租賃擴展主檔 (LeaseExtensions)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseExtensions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseExtensions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號 (EXTENSION_ID)
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
        [ExtensionType] NVARCHAR(20) NOT NULL, -- 擴展類型 (EXTENSION_TYPE, CONDITION:特殊條件, TERM:附加條款, SETTING:擴展設定)
        [ExtensionName] NVARCHAR(200) NULL, -- 擴展名稱 (EXTENSION_NAME)
        [ExtensionValue] NVARCHAR(MAX) NULL, -- 擴展值 (EXTENSION_VALUE)
        [StartDate] DATETIME2 NULL, -- 開始日期 (START_DATE)
        [EndDate] DATETIME2 NULL, -- 結束日期 (END_DATE)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LeaseExtensions_ExtensionId] UNIQUE ([ExtensionId]),
        CONSTRAINT [FK_LeaseExtensions_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionId] ON [dbo].[LeaseExtensions] ([ExtensionId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_LeaseId] ON [dbo].[LeaseExtensions] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionType] ON [dbo].[LeaseExtensions] ([ExtensionType]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_Status] ON [dbo].[LeaseExtensions] ([Status]);

    PRINT '資料表 LeaseExtensions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseExtensions 已存在';
END
GO

-- 2. 租賃擴展明細 (LeaseExtensionDetails)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseExtensionDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseExtensionDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [FieldName] NVARCHAR(100) NULL, -- 欄位名稱 (FIELD_NAME)
        [FieldValue] NVARCHAR(MAX) NULL, -- 欄位值 (FIELD_VALUE)
        [FieldType] NVARCHAR(20) NULL, -- 欄位類型 (FIELD_TYPE, TEXT:文字, NUMBER:數字, DATE:日期, BOOLEAN:布林)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_LeaseExtensionDetails_LeaseExtensions] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[LeaseExtensions] ([ExtensionId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensionDetails_ExtensionId] ON [dbo].[LeaseExtensionDetails] ([ExtensionId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseExtensionDetails_LineNum] ON [dbo].[LeaseExtensionDetails] ([ExtensionId], [LineNum]);

    PRINT '資料表 LeaseExtensionDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseExtensionDetails 已存在';
END
GO


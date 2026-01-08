-- 其他管理類資料表 (SYSS000, SYSU000, SYSV000, SYSJ000)
-- 對應舊系統 SYSS_*, SYSU_*, SYSV_*, SYSJ_*

-- =============================================
-- SYSSFunctions - S系統功能維護 (SYSS000)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SYSSFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SYSSFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼 (FUNCTION_ID)
        [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱 (FUNCTION_NAME)
        [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (FUNCTION_TYPE)
        [FunctionValue] NVARCHAR(500) NULL, -- 功能值 (FUNCTION_VALUE)
        [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能設定 (JSON格式) (FUNCTION_CONFIG)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_SYSSFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_SYSSFunctions_FunctionId] UNIQUE ([FunctionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SYSSFunctions_FunctionId] ON [dbo].[SYSSFunctions] ([FunctionId]);
    CREATE NONCLUSTERED INDEX [IX_SYSSFunctions_FunctionType] ON [dbo].[SYSSFunctions] ([FunctionType]);
    CREATE NONCLUSTERED INDEX [IX_SYSSFunctions_Status] ON [dbo].[SYSSFunctions] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SYSSFunctions_SeqNo] ON [dbo].[SYSSFunctions] ([SeqNo]);
END
GO

-- =============================================
-- SYSUFunctions - U系統功能維護 (SYSU000)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SYSUFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SYSUFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼 (FUNCTION_ID)
        [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱 (FUNCTION_NAME)
        [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (FUNCTION_TYPE, PAYMENT:支付, BANK:銀行, FINANCE:財務, REPORT:報表, OTHER:其他)
        [FunctionCategory] NVARCHAR(50) NULL, -- 功能分類 (FUNCTION_CATEGORY)
        [FunctionValue] NVARCHAR(500) NULL, -- 功能值 (FUNCTION_VALUE)
        [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能設定 (JSON格式) (FUNCTION_CONFIG)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_SYSUFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_SYSUFunctions_FunctionId] UNIQUE ([FunctionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_FunctionId] ON [dbo].[SYSUFunctions] ([FunctionId]);
    CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_FunctionType] ON [dbo].[SYSUFunctions] ([FunctionType]);
    CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_FunctionCategory] ON [dbo].[SYSUFunctions] ([FunctionCategory]);
    CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_Status] ON [dbo].[SYSUFunctions] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_SeqNo] ON [dbo].[SYSUFunctions] ([SeqNo]);
END
GO

-- =============================================
-- SYSVFunctions - V系統功能維護 (SYSV000)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SYSVFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SYSVFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼 (FUNCTION_ID)
        [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱 (FUNCTION_NAME)
        [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (FUNCTION_TYPE, BASE:基礎, PROCESS:處理, CHECK:檢查, REPORT:報表)
        [VoucherType] NVARCHAR(20) NULL, -- 憑證類型 (VOUCHER_TYPE)
        [FunctionValue] NVARCHAR(500) NULL, -- 功能值 (FUNCTION_VALUE)
        [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能設定 (JSON格式) (FUNCTION_CONFIG)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [IsCustomerSpecific] BIT NOT NULL DEFAULT 0, -- 是否為客戶特定版本 (IS_CUSTOMER_SPECIFIC)
        [CustomerCode] NVARCHAR(50) NULL, -- 客戶代碼 (CUSTOMER_CODE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_SYSVFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_SYSVFunctions_FunctionId] UNIQUE ([FunctionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SYSVFunctions_FunctionId] ON [dbo].[SYSVFunctions] ([FunctionId]);
    CREATE NONCLUSTERED INDEX [IX_SYSVFunctions_FunctionType] ON [dbo].[SYSVFunctions] ([FunctionType]);
    CREATE NONCLUSTERED INDEX [IX_SYSVFunctions_VoucherType] ON [dbo].[SYSVFunctions] ([VoucherType]);
    CREATE NONCLUSTERED INDEX [IX_SYSVFunctions_Status] ON [dbo].[SYSVFunctions] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SYSVFunctions_CustomerCode] ON [dbo].[SYSVFunctions] ([CustomerCode]);
    CREATE NONCLUSTERED INDEX [IX_SYSVFunctions_SeqNo] ON [dbo].[SYSVFunctions] ([SeqNo]);
END
GO

-- =============================================
-- SYSJFunctions - J系統功能維護 (SYSJ000)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SYSJFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SYSJFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼 (FUNCTION_ID)
        [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱 (FUNCTION_NAME)
        [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (FUNCTION_TYPE)
        [FunctionValue] NVARCHAR(500) NULL, -- 功能值 (FUNCTION_VALUE)
        [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能設定 (JSON格式) (FUNCTION_CONFIG)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_SYSJFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_SYSJFunctions_FunctionId] UNIQUE ([FunctionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_FunctionId] ON [dbo].[SYSJFunctions] ([FunctionId]);
    CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_FunctionType] ON [dbo].[SYSJFunctions] ([FunctionType]);
    CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_Status] ON [dbo].[SYSJFunctions] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_SeqNo] ON [dbo].[SYSJFunctions] ([SeqNo]);
END
GO


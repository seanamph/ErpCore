-- SYSL210 - 員餐卡報表：員餐卡交易明細資料表
-- 建立日期：2025-01-09

-- 主要資料表: EmployeeMealCardTransactions (對應舊系統相關表)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeMealCardTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmployeeMealCardTransactions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [TxnNo] NVARCHAR(50) NOT NULL, -- 交易單號 (TXN_NO)
        [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SITE_ID)
        [CardSurfaceId] NVARCHAR(50) NULL, -- 卡片表面ID (CARD_SURFACE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [ActionType] NVARCHAR(20) NULL, -- 動作類型 (ACTION_TYPE)
        [ActionTypeName] NVARCHAR(100) NULL, -- 動作類型名稱 (ACTION_TYPE_NAME)
        [YearMonth] NVARCHAR(6) NULL, -- 年月 (YYYYMM) (YEAR_MONTH)
        [Amt1] DECIMAL(18,2) NULL DEFAULT 0, -- 金額1 (AMT1)
        [Amt4] DECIMAL(18,2) NULL DEFAULT 0, -- 金額4 (AMT4)
        [Amt5] DECIMAL(18,2) NULL DEFAULT 0, -- 金額5 (AMT5)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        CONSTRAINT [PK_EmployeeMealCardTransactions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_EmployeeMealCardTransactions_TxnNo] UNIQUE ([TxnNo])
    );
END

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardTransactions_SiteId' AND object_id = OBJECT_ID('EmployeeMealCardTransactions'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_SiteId] ON [dbo].[EmployeeMealCardTransactions] ([SiteId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardTransactions_CardSurfaceId' AND object_id = OBJECT_ID('EmployeeMealCardTransactions'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_CardSurfaceId] ON [dbo].[EmployeeMealCardTransactions] ([CardSurfaceId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardTransactions_OrgId' AND object_id = OBJECT_ID('EmployeeMealCardTransactions'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_OrgId] ON [dbo].[EmployeeMealCardTransactions] ([OrgId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardTransactions_ActionType' AND object_id = OBJECT_ID('EmployeeMealCardTransactions'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_ActionType] ON [dbo].[EmployeeMealCardTransactions] ([ActionType]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardTransactions_YearMonth' AND object_id = OBJECT_ID('EmployeeMealCardTransactions'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_YearMonth] ON [dbo].[EmployeeMealCardTransactions] ([YearMonth]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardTransactions_TxnNo' AND object_id = OBJECT_ID('EmployeeMealCardTransactions'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_TxnNo] ON [dbo].[EmployeeMealCardTransactions] ([TxnNo]);


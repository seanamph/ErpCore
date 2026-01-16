-- =============================================
-- 更新銀行帳戶資料表以支援銀行帳戶維護功能
-- 功能代碼: 銀行帳戶維護
-- 建立日期: 2025-01-27
-- 說明: 添加開發計劃要求的缺失欄位
-- =============================================

-- 1. 更新 BankAccounts 表，添加缺失欄位
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Balance')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [Balance] DECIMAL(18, 2) NULL DEFAULT 0; -- 帳戶餘額
    PRINT '已添加欄位 BankAccounts.Balance';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'OpeningDate')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [OpeningDate] DATETIME2 NULL; -- 開戶日期
    PRINT '已添加欄位 BankAccounts.OpeningDate';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ClosingDate')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [ClosingDate] DATETIME2 NULL; -- 結清日期
    PRINT '已添加欄位 BankAccounts.ClosingDate';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ContactPerson')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [ContactPerson] NVARCHAR(50) NULL; -- 聯絡人
    PRINT '已添加欄位 BankAccounts.ContactPerson';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ContactPhone')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [ContactPhone] NVARCHAR(50) NULL; -- 聯絡電話
    PRINT '已添加欄位 BankAccounts.ContactPhone';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ContactEmail')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [ContactEmail] NVARCHAR(100) NULL; -- 聯絡信箱
    PRINT '已添加欄位 BankAccounts.ContactEmail';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'BranchName')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [BranchName] NVARCHAR(100) NULL; -- 分行名稱
    PRINT '已添加欄位 BankAccounts.BranchName';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'BranchCode')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [BranchCode] NVARCHAR(50) NULL; -- 分行代號
    PRINT '已添加欄位 BankAccounts.BranchCode';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'SwiftCode')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [SwiftCode] NVARCHAR(50) NULL; -- SWIFT代號
    PRINT '已添加欄位 BankAccounts.SwiftCode';
END
GO

-- 將 Memo 欄位重命名為 Notes（如果 Memo 存在且 Notes 不存在）
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Memo')
    AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Notes')
BEGIN
    EXEC sp_rename '[dbo].[BankAccounts].[Memo]', 'Notes', 'COLUMN';
    PRINT '已將欄位 Memo 重命名為 Notes';
END
GO

-- 如果 Notes 欄位不存在，則添加它
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Notes')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [Notes] NVARCHAR(500) NULL; -- 備註
    PRINT '已添加欄位 BankAccounts.Notes';
END
GO

-- 添加 CreatedPriority 和 CreatedGroup 欄位（如果不存在）
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'CreatedPriority')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [CreatedPriority] INT NULL; -- 建立者等級
    PRINT '已添加欄位 BankAccounts.CreatedPriority';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'CreatedGroup')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD [CreatedGroup] NVARCHAR(50) NULL; -- 建立者群組
    PRINT '已添加欄位 BankAccounts.CreatedGroup';
END
GO

-- 更新 Status 欄位預設值為 '1'（如果當前是 'A'）
-- 注意：根據開發計劃，Status 應該是 '1' (啟用) 或 '0' (停用)，但現有代碼使用 'A'/'I'
-- 這裡保持現有邏輯，但添加註釋說明

-- 添加索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'IX_BankAccounts_AccountNumber')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_AccountNumber] ON [dbo].[BankAccounts] ([AccountNumber]);
    PRINT '已添加索引 IX_BankAccounts_AccountNumber';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'IX_BankAccounts_AccountType')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_AccountType] ON [dbo].[BankAccounts] ([AccountType]);
    PRINT '已添加索引 IX_BankAccounts_AccountType';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'IX_BankAccounts_CurrencyId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_CurrencyId] ON [dbo].[BankAccounts] ([CurrencyId]);
    PRINT '已添加索引 IX_BankAccounts_CurrencyId';
END
GO

-- 添加外鍵約束（如果 Banks 表存在）
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BankAccounts_Banks')
    AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Banks]') AND type = 'U')
BEGIN
    ALTER TABLE [dbo].[BankAccounts]
    ADD CONSTRAINT [FK_BankAccounts_Banks] FOREIGN KEY ([BankId]) REFERENCES [dbo].[Banks] ([BankId]);
    PRINT '已添加外鍵約束 FK_BankAccounts_Banks';
END
GO

PRINT '銀行帳戶資料表更新完成';

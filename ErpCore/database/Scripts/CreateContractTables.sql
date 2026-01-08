-- =============================================
-- 合同管理類資料表建立腳本
-- 功能代碼: SYSF000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 合同主檔 (Contracts)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contracts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Contracts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號 (CONTRACT_ID)
        [ContractType] NVARCHAR(20) NOT NULL, -- 合同類型 (CTS_TYPE, 1:商場招商合約, 2:委外廠商合約, 3:塔樓招商合約)
        [Version] INT NOT NULL DEFAULT 1, -- 版本號 (VERSION)
        [VendorId] NVARCHAR(50) NOT NULL, -- 廠商代碼 (VENDOR_ID)
        [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME)
        [SignDate] DATETIME2 NULL, -- 簽約日期 (SIGN_DATE)
        [EffectiveDate] DATETIME2 NULL, -- 生效日期 (EFFECTIVE_DATE)
        [ExpiryDate] DATETIME2 NULL, -- 到期日期 (EXPIRY_DATE)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, P:審核中, A:已生效, E:已到期, T:已終止)
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [LocationId] NVARCHAR(50) NULL, -- 位置編號 (LOCATION_ID)
        [RecruitId] NVARCHAR(50) NULL, -- 招商編號 (RECRUIT_ID)
        [Attorney] NVARCHAR(100) NULL, -- 委託人 (ATTORNEY)
        [Salutation] NVARCHAR(50) NULL, -- 稱謂 (SALUTATION)
        [VerStatus] NVARCHAR(10) NULL, -- 版本狀態 (VER_STATUS, 0:覆蓋原版本, 1:產生新版本, 2:產生正式合約)
        [AgmStatus] NVARCHAR(10) NULL, -- 協議狀態 (AGM_STATUS)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Contracts_ContractId_Version] UNIQUE ([ContractId], [Version])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Contracts_ContractId] ON [dbo].[Contracts] ([ContractId]);
    CREATE NONCLUSTERED INDEX [IX_Contracts_VendorId] ON [dbo].[Contracts] ([VendorId]);
    CREATE NONCLUSTERED INDEX [IX_Contracts_Status] ON [dbo].[Contracts] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Contracts_ContractType] ON [dbo].[Contracts] ([ContractType]);
    CREATE NONCLUSTERED INDEX [IX_Contracts_EffectiveDate] ON [dbo].[Contracts] ([EffectiveDate]);
    CREATE NONCLUSTERED INDEX [IX_Contracts_ExpiryDate] ON [dbo].[Contracts] ([ExpiryDate]);

    PRINT '資料表 Contracts 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Contracts 已存在';
END
GO

-- 2. 合同條款 (ContractTerms)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContractTerms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ContractTerms] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號
        [Version] INT NOT NULL, -- 版本號
        [TermType] NVARCHAR(50) NULL, -- 條款類型 (TERM_TYPE)
        [TermContent] NVARCHAR(MAX) NULL, -- 條款內容 (TERM_CONTENT)
        [TermOrder] INT NULL, -- 條款順序 (TERM_ORDER)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ContractTerms_ContractId] ON [dbo].[ContractTerms] ([ContractId], [Version]);

    PRINT '資料表 ContractTerms 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ContractTerms 已存在';
END
GO

-- 3. 合同罰則 (ContractPenalties)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContractPenalties]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ContractPenalties] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號
        [Version] INT NOT NULL, -- 版本號
        [PenaltyType] NVARCHAR(50) NULL, -- 罰則類型 (PENALTY_TYPE)
        [PenaltyAmount] DECIMAL(18, 4) NULL, -- 罰則金額 (PENALTY_AMT)
        [PenaltyRate] DECIMAL(5, 2) NULL, -- 罰則比率 (PENALTY_RATE)
        [PenaltyDesc] NVARCHAR(500) NULL, -- 罰則說明 (PENALTY_DESC)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ContractPenalties_ContractId] ON [dbo].[ContractPenalties] ([ContractId], [Version]);

    PRINT '資料表 ContractPenalties 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ContractPenalties 已存在';
END
GO

-- 4. 合同會計分類 (ContractAccounting)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContractAccounting]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ContractAccounting] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號
        [Version] INT NOT NULL, -- 版本號
        [AccountCode] NVARCHAR(50) NULL, -- 會計科目代碼 (ACCOUNT_CODE)
        [AccountName] NVARCHAR(200) NULL, -- 會計科目名稱 (ACCOUNT_NAME)
        [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ContractAccounting_ContractId] ON [dbo].[ContractAccounting] ([ContractId], [Version]);

    PRINT '資料表 ContractAccounting 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ContractAccounting 已存在';
END
GO

-- 5. 合同處理主檔 (ContractProcesses)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContractProcesses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ContractProcesses] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號 (PROCESS_ID)
        [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號 (CONTRACT_ID)
        [Version] INT NOT NULL, -- 版本號 (VERSION)
        [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (PROCESS_TYPE, PAY:付款, REC:收款, CHG:變更, TER:終止)
        [ProcessDate] DATETIME2 NOT NULL, -- 處理日期 (PROCESS_DATE)
        [ProcessAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 處理金額 (PROCESS_AMT)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:待處理, I:處理中, C:已完成, X:已取消)
        [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員 (PROCESS_USER)
        [ProcessMemo] NVARCHAR(500) NULL, -- 處理備註 (PROCESS_MEMO)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_ContractProcesses_ProcessId] UNIQUE ([ProcessId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ContractProcesses_ContractId] ON [dbo].[ContractProcesses] ([ContractId], [Version]);
    CREATE NONCLUSTERED INDEX [IX_ContractProcesses_ProcessType] ON [dbo].[ContractProcesses] ([ProcessType]);
    CREATE NONCLUSTERED INDEX [IX_ContractProcesses_Status] ON [dbo].[ContractProcesses] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_ContractProcesses_ProcessDate] ON [dbo].[ContractProcesses] ([ProcessDate]);

    PRINT '資料表 ContractProcesses 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ContractProcesses 已存在';
END
GO

-- 6. 合同擴展主檔 (ContractExtensions) - SYSF350-SYSF540
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContractExtensions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ContractExtensions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號 (CONTRACT_ID)
        [Version] INT NOT NULL DEFAULT 1, -- 版本號 (VERSION)
        [ExtensionType] NVARCHAR(20) NULL, -- 擴展類型 (EXT_TYPE)
        [VendorId] NVARCHAR(50) NULL, -- 供應商代碼 (VENDOR_ID)
        [VendorName] NVARCHAR(200) NULL, -- 供應商名稱 (VENDOR_NAME)
        [ExtensionDate] DATETIME2 NULL, -- 擴展日期 (EXT_DATE)
        [ExtensionAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 擴展金額 (EXT_AMT)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_ContractExtensions_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ContractExtensions_ContractId] ON [dbo].[ContractExtensions] ([ContractId], [Version]);
    CREATE NONCLUSTERED INDEX [IX_ContractExtensions_VendorId] ON [dbo].[ContractExtensions] ([VendorId]);
    CREATE NONCLUSTERED INDEX [IX_ContractExtensions_Status] ON [dbo].[ContractExtensions] ([Status]);

    PRINT '資料表 ContractExtensions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ContractExtensions 已存在';
END
GO

-- 7. 合同傳輸記錄 (ContractTransfers) - SYSF350-SYSF540
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContractTransfers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ContractTransfers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號 (CONTRACT_ID)
        [Version] INT NOT NULL, -- 版本號 (VERSION)
        [TransferType] NVARCHAR(20) NULL, -- 傳輸類型 (TRANSFER_TYPE)
        [TransferDate] DATETIME2 NOT NULL, -- 傳輸日期 (TRANSFER_DATE)
        [TransferStatus] NVARCHAR(10) NULL, -- 傳輸狀態 (TRANSFER_STATUS)
        [TransferResult] NVARCHAR(MAX) NULL, -- 傳輸結果 (TRANSFER_RESULT)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_ContractTransfers_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ContractTransfers_ContractId] ON [dbo].[ContractTransfers] ([ContractId], [Version]);
    CREATE NONCLUSTERED INDEX [IX_ContractTransfers_TransferDate] ON [dbo].[ContractTransfers] ([TransferDate]);

    PRINT '資料表 ContractTransfers 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ContractTransfers 已存在';
END
GO

-- 8. CMS合同主檔 (CmsContracts) - CMS2310-CMS2320
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContracts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CmsContracts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CmsContractId] NVARCHAR(50) NOT NULL, -- CMS合同編號 (CMS_CONTRACT_ID)
        [ContractType] NVARCHAR(20) NOT NULL, -- 合同類型 (CONTRACT_TYPE)
        [Version] INT NOT NULL DEFAULT 1, -- 版本號 (VERSION)
        [VendorId] NVARCHAR(50) NOT NULL, -- 廠商代碼 (VENDOR_ID)
        [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME)
        [SignDate] DATETIME2 NULL, -- 簽約日期 (SIGN_DATE)
        [EffectiveDate] DATETIME2 NULL, -- 生效日期 (EFFECTIVE_DATE)
        [ExpiryDate] DATETIME2 NULL, -- 到期日期 (EXPIRY_DATE)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, P:審核中, A:已生效, E:已到期, T:已終止)
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [LocationId] NVARCHAR(50) NULL, -- 位置編號 (LOCATION_ID)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_CmsContracts] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_CmsContracts_CmsContractId_Version] UNIQUE ([CmsContractId], [Version])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CmsContracts_CmsContractId] ON [dbo].[CmsContracts] ([CmsContractId]);
    CREATE NONCLUSTERED INDEX [IX_CmsContracts_VendorId] ON [dbo].[CmsContracts] ([VendorId]);
    CREATE NONCLUSTERED INDEX [IX_CmsContracts_Status] ON [dbo].[CmsContracts] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_CmsContracts_ContractType] ON [dbo].[CmsContracts] ([ContractType]);
    CREATE NONCLUSTERED INDEX [IX_CmsContracts_EffectiveDate] ON [dbo].[CmsContracts] ([EffectiveDate]);
    CREATE NONCLUSTERED INDEX [IX_CmsContracts_ExpiryDate] ON [dbo].[CmsContracts] ([ExpiryDate]);

    PRINT '資料表 CmsContracts 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 CmsContracts 已存在';
END
GO

-- 9. CMS合同條款 (CmsContractTerms) - CMS2310-CMS2320
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContractTerms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CmsContractTerms] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CmsContractId] NVARCHAR(50) NOT NULL, -- CMS合同編號 (CMS_CONTRACT_ID)
        [Version] INT NOT NULL, -- 版本號 (VERSION)
        [TermType] NVARCHAR(50) NULL, -- 條款類型 (TERM_TYPE)
        [TermContent] NVARCHAR(MAX) NULL, -- 條款內容 (TERM_CONTENT)
        [TermOrder] INT NULL, -- 條款順序 (TERM_ORDER)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CmsContractTerms_CmsContracts] FOREIGN KEY ([CmsContractId], [Version]) REFERENCES [dbo].[CmsContracts] ([CmsContractId], [Version]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CmsContractTerms_CmsContractId] ON [dbo].[CmsContractTerms] ([CmsContractId], [Version]);

    PRINT '資料表 CmsContractTerms 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 CmsContractTerms 已存在';
END
GO


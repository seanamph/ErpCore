-- =============================================
-- 查詢管理類資料表建立腳本
-- 功能代碼: SYSQ000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 零用金參數 (CashParams)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CashParams]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CashParams] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [UnitId] NVARCHAR(50) NULL, -- 公司單位代號
        [ApexpLid] NVARCHAR(50) NOT NULL, -- 銀行存款會計科目代號
        [PtaxLid] NVARCHAR(50) NOT NULL, -- 進項稅額會計科目代號
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_CashParams] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CashParams_UnitId] ON [dbo].[CashParams] ([UnitId]);

    PRINT '資料表 CashParams 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 CashParams 已存在';
END
GO

-- 2. 保管人及額度設定 (PcKeep)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PcKeep]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PcKeep] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SiteId] NVARCHAR(50) NULL, -- 分店代號
        [KeepEmpId] NVARCHAR(50) NOT NULL, -- 保管人代碼
        [PcQuota] DECIMAL(18,2) NULL DEFAULT 0, -- 零用金額度
        [Notes] NVARCHAR(500) NULL, -- 備註
        [BUser] NVARCHAR(50) NULL, -- 建立者
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [CUser] NVARCHAR(50) NULL, -- 更新者
        [CTime] DATETIME2 NULL, -- 更新時間
        [CPriority] INT NULL, -- 建立者等級
        [CGroup] NVARCHAR(50) NULL, -- 建立者群組
        CONSTRAINT [PK_PcKeep] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PcKeep_KeepEmpId_SiteId] UNIQUE ([KeepEmpId], [SiteId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PcKeep_SiteId] ON [dbo].[PcKeep] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_PcKeep_KeepEmpId] ON [dbo].[PcKeep] ([KeepEmpId]);

    PRINT '資料表 PcKeep 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PcKeep 已存在';
END
GO

-- 3. 零用金主檔 (PcCash) - SYSQ210
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PcCash]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PcCash] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CashId] NVARCHAR(50) NOT NULL, -- 零用金單號
        [SiteId] NVARCHAR(50) NULL, -- 分店代號
        [AppleDate] DATETIME2 NOT NULL, -- 申請日期
        [AppleName] NVARCHAR(50) NOT NULL, -- 申請人
        [OrgId] NVARCHAR(50) NULL, -- 申請組織代號
        [KeepEmpId] NVARCHAR(50) NULL, -- 保管人代碼
        [CashAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 零用金金額
        [CashStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態 (DRAFT:草稿, APPLIED:已申請, REQUESTED:已請款, TRANSFERRED:已拋轉, INVENTORIED:已盤點, APPROVED:已審核)
        [Notes] NVARCHAR(500) NULL, -- 備註
        [BUser] NVARCHAR(50) NULL, -- 建立者
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [CUser] NVARCHAR(50) NULL, -- 更新者
        [CTime] DATETIME2 NULL, -- 更新時間
        [CPriority] INT NULL, -- 建立者等級
        [CGroup] NVARCHAR(50) NULL, -- 建立者群組
        CONSTRAINT [PK_PcCash] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PcCash_CashId] UNIQUE ([CashId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PcCash_SiteId] ON [dbo].[PcCash] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_PcCash_AppleDate] ON [dbo].[PcCash] ([AppleDate]);
    CREATE NONCLUSTERED INDEX [IX_PcCash_AppleName] ON [dbo].[PcCash] ([AppleName]);
    CREATE NONCLUSTERED INDEX [IX_PcCash_KeepEmpId] ON [dbo].[PcCash] ([KeepEmpId]);
    CREATE NONCLUSTERED INDEX [IX_PcCash_CashStatus] ON [dbo].[PcCash] ([CashStatus]);

    PRINT '資料表 PcCash 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PcCash 已存在';
END
GO

-- 4. 零用金請款檔 (PcCashRequest) - SYSQ220
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PcCashRequest]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PcCashRequest] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [RequestId] NVARCHAR(50) NOT NULL, -- 請款單號
        [SiteId] NVARCHAR(50) NULL, -- 分店代號
        [RequestDate] DATETIME2 NOT NULL, -- 請款日期
        [CashIds] NVARCHAR(MAX) NULL, -- 零用金單號列表 (JSON格式)
        [RequestAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 請款金額
        [RequestStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態
        [Notes] NVARCHAR(500) NULL, -- 備註
        [BUser] NVARCHAR(50) NULL, -- 建立者
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [CUser] NVARCHAR(50) NULL, -- 更新者
        [CTime] DATETIME2 NULL, -- 更新時間
        CONSTRAINT [PK_PcCashRequest] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PcCashRequest_RequestId] UNIQUE ([RequestId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PcCashRequest_SiteId] ON [dbo].[PcCashRequest] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_PcCashRequest_RequestDate] ON [dbo].[PcCashRequest] ([RequestDate]);

    PRINT '資料表 PcCashRequest 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PcCashRequest 已存在';
END
GO

-- 5. 零用金拋轉檔 (PcCashTransfer) - SYSQ230
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PcCashTransfer]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PcCashTransfer] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TransferId] NVARCHAR(50) NOT NULL, -- 拋轉單號
        [SiteId] NVARCHAR(50) NULL, -- 分店代號
        [TransferDate] DATETIME2 NOT NULL, -- 拋轉日期
        [VoucherId] NVARCHAR(50) NULL, -- 傳票編號
        [VoucherKind] NVARCHAR(20) NULL, -- 傳票種類
        [VoucherDate] DATETIME2 NULL, -- 傳票日期
        [TransferAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 拋轉金額
        [TransferStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態
        [Notes] NVARCHAR(500) NULL, -- 備註
        [BUser] NVARCHAR(50) NULL, -- 建立者
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [CUser] NVARCHAR(50) NULL, -- 更新者
        [CTime] DATETIME2 NULL, -- 更新時間
        CONSTRAINT [PK_PcCashTransfer] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PcCashTransfer_TransferId] UNIQUE ([TransferId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PcCashTransfer_SiteId] ON [dbo].[PcCashTransfer] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_PcCashTransfer_TransferDate] ON [dbo].[PcCashTransfer] ([TransferDate]);
    CREATE NONCLUSTERED INDEX [IX_PcCashTransfer_VoucherId] ON [dbo].[PcCashTransfer] ([VoucherId]);

    PRINT '資料表 PcCashTransfer 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PcCashTransfer 已存在';
END
GO

-- 6. 零用金盤點檔 (PcCashInventory) - SYSQ241, SYSQ242
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PcCashInventory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PcCashInventory] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InventoryId] NVARCHAR(50) NOT NULL, -- 盤點單號
        [SiteId] NVARCHAR(50) NULL, -- 分店代號
        [InventoryDate] DATETIME2 NOT NULL, -- 盤點日期
        [KeepEmpId] NVARCHAR(50) NOT NULL, -- 保管人代碼
        [InventoryAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 盤點金額
        [ActualAmount] DECIMAL(18,2) NULL, -- 實際金額
        [DifferenceAmount] DECIMAL(18,2) NULL, -- 差異金額
        [InventoryStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態
        [Notes] NVARCHAR(500) NULL, -- 備註
        [BUser] NVARCHAR(50) NULL, -- 建立者
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [CUser] NVARCHAR(50) NULL, -- 更新者
        [CTime] DATETIME2 NULL, -- 更新時間
        CONSTRAINT [PK_PcCashInventory] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PcCashInventory_InventoryId] UNIQUE ([InventoryId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PcCashInventory_SiteId] ON [dbo].[PcCashInventory] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_PcCashInventory_InventoryDate] ON [dbo].[PcCashInventory] ([InventoryDate]);
    CREATE NONCLUSTERED INDEX [IX_PcCashInventory_KeepEmpId] ON [dbo].[PcCashInventory] ([KeepEmpId]);

    PRINT '資料表 PcCashInventory 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PcCashInventory 已存在';
END
GO

-- 7. 傳票審核傳送檔 (VoucherAudit) - SYSQ250
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherAudit]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherAudit] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherId] NVARCHAR(50) NOT NULL, -- 傳票編號
        [VoucherKind] NVARCHAR(20) NULL, -- 傳票種類
        [VoucherDate] DATETIME2 NOT NULL, -- 傳票日期
        [AuditStatus] NVARCHAR(20) NULL DEFAULT 'PENDING', -- 審核狀態 (PENDING:待審核, APPROVED:已審核, REJECTED:已拒絕)
        [AuditUser] NVARCHAR(50) NULL, -- 審核者
        [AuditTime] DATETIME2 NULL, -- 審核時間
        [AuditNotes] NVARCHAR(500) NULL, -- 審核備註
        [SendStatus] NVARCHAR(20) NULL DEFAULT 'PENDING', -- 傳送狀態 (PENDING:待傳送, SENT:已傳送)
        [SendTime] DATETIME2 NULL, -- 傳送時間
        [Notes] NVARCHAR(500) NULL, -- 備註
        [BUser] NVARCHAR(50) NULL, -- 建立者
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [CUser] NVARCHAR(50) NULL, -- 更新者
        [CTime] DATETIME2 NULL, -- 更新時間
        CONSTRAINT [PK_VoucherAudit] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_VoucherAudit_VoucherId] UNIQUE ([VoucherId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_VoucherAudit_VoucherDate] ON [dbo].[VoucherAudit] ([VoucherDate]);
    CREATE NONCLUSTERED INDEX [IX_VoucherAudit_AuditStatus] ON [dbo].[VoucherAudit] ([AuditStatus]);
    CREATE NONCLUSTERED INDEX [IX_VoucherAudit_SendStatus] ON [dbo].[VoucherAudit] ([SendStatus]);

    PRINT '資料表 VoucherAudit 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 VoucherAudit 已存在';
END
GO

PRINT '查詢管理類資料表建立完成';


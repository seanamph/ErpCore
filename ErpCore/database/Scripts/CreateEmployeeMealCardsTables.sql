-- SYSL130 - 業務報表查詢作業：員工餐卡申請資料表
-- 建立日期：2025-01-08

-- 主要資料表: EmployeeMealCards (對應舊系統 IMS_SR.EMP_M)
CREATE TABLE [dbo].[EmployeeMealCards] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [EmpId] NVARCHAR(50) NOT NULL, -- 員工編號 (EMP_ID)
    [EmpName] NVARCHAR(100) NULL, -- 員工姓名 (EMP_NAME)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [SiteId] NVARCHAR(50) NULL, -- 店別代碼 (SITE_ID)
    [CardType] NVARCHAR(20) NULL, -- 卡片類型 (CARD_TYPE)
    [ActionType] NVARCHAR(20) NULL, -- 動作類型 (ACTION_TYPE)
    [ActionTypeD] NVARCHAR(20) NULL, -- 動作類型明細 (ACTION_TYPE_D)
    [StartDate] DATETIME2 NULL, -- 起始日期 (START_DATE)
    [EndDate] DATETIME2 NULL, -- 結束日期 (END_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS) P:待審核, A:已審核, R:已拒絕
    [Verifier] NVARCHAR(50) NULL, -- 審核者 (VERIFIER)
    [VerifyDate] DATETIME2 NULL, -- 審核日期 (VERIFY_DATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [TxnNo] NVARCHAR(50) NULL, -- 交易單號 (TXN_NO)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
    CONSTRAINT [PK_EmployeeMealCards] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_EmpId] ON [dbo].[EmployeeMealCards] ([EmpId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_OrgId] ON [dbo].[EmployeeMealCards] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_SiteId] ON [dbo].[EmployeeMealCards] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_Status] ON [dbo].[EmployeeMealCards] ([Status]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_TxnNo] ON [dbo].[EmployeeMealCards] ([TxnNo]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_StartDate] ON [dbo].[EmployeeMealCards] ([StartDate]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_EndDate] ON [dbo].[EmployeeMealCards] ([EndDate]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_CardType] ON [dbo].[EmployeeMealCards] ([CardType]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_ActionType] ON [dbo].[EmployeeMealCards] ([ActionType]);

-- 相關資料表: CardType - 卡片類型主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CardType]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CardType] (
        [CardId] NVARCHAR(20) NOT NULL PRIMARY KEY, -- 卡片類型代碼
        [CardName] NVARCHAR(100) NOT NULL, -- 卡片類型名稱
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

-- 相關資料表: ActionType - 動作類型主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActionType]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ActionType] (
        [ActionId] NVARCHAR(20) NOT NULL PRIMARY KEY, -- 動作類型代碼
        [ActionName] NVARCHAR(100) NOT NULL, -- 動作類型名稱
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

-- 相關資料表: ActionTypeDetail - 動作類型明細主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActionTypeDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ActionTypeDetail] (
        [ActionId] NVARCHAR(20) NOT NULL, -- 動作類型代碼
        [ActionIdD] NVARCHAR(20) NOT NULL, -- 動作類型明細代碼
        [ActionNameD] NVARCHAR(100) NOT NULL, -- 動作類型明細名稱
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_ActionTypeDetail] PRIMARY KEY CLUSTERED ([ActionId], [ActionIdD])
    );
END

-- 相關資料表: MainApplyMaster - 主申請單主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MainApplyMaster]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MainApplyMaster] (
        [TxnNo] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 交易單號
        [ApplyType] NVARCHAR(20) NOT NULL, -- 申請類型
        [ApplyDate] DATETIME2 NOT NULL, -- 申請日期
        [EmpId] NVARCHAR(50) NOT NULL, -- 申請人
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態
        [Verifier] NVARCHAR(50) NULL, -- 審核者
        [VerifyDate] DATETIME2 NULL, -- 審核日期
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END


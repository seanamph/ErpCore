-- =============================================
-- 忠誠度系統類 (SYSLPS) - 資料庫腳本
-- 功能：忠誠度系統初始化 (WEBLOYALTYINI)、忠誠度系統維護 (LPS)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 忠誠度系統設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoyaltySystemConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LoyaltySystemConfigs] (
        [ConfigId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [ConfigName] NVARCHAR(100) NOT NULL,
        [ConfigValue] NVARCHAR(500) NULL,
        [ConfigType] NVARCHAR(20) NOT NULL, -- PARAM, RULE, ENV
        [Description] NVARCHAR(500) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LoyaltySystemConfigs_ConfigType] ON [dbo].[LoyaltySystemConfigs] ([ConfigType]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltySystemConfigs_Status] ON [dbo].[LoyaltySystemConfigs] ([Status]);
END
GO

-- 2. 忠誠度系統初始化記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoyaltySystemInitLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LoyaltySystemInitLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InitId] NVARCHAR(50) NOT NULL, -- 初始化批次編號
        [InitStatus] NVARCHAR(20) NOT NULL, -- 初始化狀態 (SUCCESS, FAILED)
        [InitDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [InitMessage] NVARCHAR(1000) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LoyaltySystemInitLogs_InitId] UNIQUE ([InitId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LoyaltySystemInitLogs_InitId] ON [dbo].[LoyaltySystemInitLogs] ([InitId]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltySystemInitLogs_InitStatus] ON [dbo].[LoyaltySystemInitLogs] ([InitStatus]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltySystemInitLogs_InitDate] ON [dbo].[LoyaltySystemInitLogs] ([InitDate]);
END
GO

-- 3. 忠誠度會員主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoyaltyMembers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LoyaltyMembers] (
        [CardNo] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [MemberName] NVARCHAR(100) NULL,
        [Phone] NVARCHAR(20) NULL,
        [Email] NVARCHAR(100) NULL,
        [TotalPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 總點數
        [AvailablePoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 可用點數
        [ExpDate] NVARCHAR(10) NULL, -- 到期日
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LoyaltyMembers_CardNo] ON [dbo].[LoyaltyMembers] ([CardNo]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltyMembers_Status] ON [dbo].[LoyaltyMembers] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltyMembers_ExpDate] ON [dbo].[LoyaltyMembers] ([ExpDate]);
END
GO

-- 4. 忠誠度點數交易主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoyaltyPointTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LoyaltyPointTransactions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [RRN] NVARCHAR(50) NOT NULL, -- 交易編號
        [CardNo] NVARCHAR(50) NOT NULL, -- 會員卡號
        [TraceNo] NVARCHAR(50) NULL, -- 追蹤編號
        [ExpDate] NVARCHAR(10) NULL, -- 到期日
        [AwardPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 累積點數
        [RedeemPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 扣減點數
        [ReversalFlag] NVARCHAR(10) NULL, -- 取消標記
        [Amount] DECIMAL(18, 4) NULL, -- 交易金額
        [VoidFlag] NVARCHAR(10) NULL, -- 作廢標記
        [AuthCode] NVARCHAR(50) NULL, -- 授權碼
        [ForceDate] DATETIME2 NULL, -- 強制日期
        [Invoice] NVARCHAR(50) NULL, -- 發票號碼
        [TransType] NVARCHAR(20) NULL, -- 交易類型 (2, 3, 4, 11, 13, 16, 18等)
        [TxnType] NVARCHAR(20) NULL, -- 交易類型代碼 (2, 3, 4, 5, 7, 8, 9等)
        [TransTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 交易時間
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'SUCCESS', -- 交易狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LoyaltyPointTransactions_RRN] UNIQUE ([RRN])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_RRN] ON [dbo].[LoyaltyPointTransactions] ([RRN]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_CardNo] ON [dbo].[LoyaltyPointTransactions] ([CardNo]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_TransTime] ON [dbo].[LoyaltyPointTransactions] ([TransTime]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_TransType] ON [dbo].[LoyaltyPointTransactions] ([TransType]);
    CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_Status] ON [dbo].[LoyaltyPointTransactions] ([Status]);
    
    -- 外鍵約束
    ALTER TABLE [dbo].[LoyaltyPointTransactions]
    ADD CONSTRAINT [FK_LoyaltyPointTransactions_LoyaltyMembers] 
    FOREIGN KEY ([CardNo]) REFERENCES [dbo].[LoyaltyMembers] ([CardNo]);
END
GO


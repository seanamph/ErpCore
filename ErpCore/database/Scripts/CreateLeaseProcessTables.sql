-- =============================================
-- 租賃處理作業類資料表建立腳本
-- 功能代碼: SYS8B50-SYS8B90
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 租賃處理主檔 (LeaseProcesses)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseProcesses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseProcesses] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號 (PROCESS_ID)
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
        [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (PROCESS_TYPE, RENEWAL:續約, TERMINATION:終止, MODIFICATION:修改, PAYMENT:付款)
        [ProcessDate] DATETIME2 NOT NULL, -- 處理日期 (PROCESS_DATE)
        [ProcessStatus] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 處理狀態 (PROCESS_STATUS, P:待處理, I:處理中, C:已完成, X:已取消)
        [ProcessResult] NVARCHAR(20) NULL, -- 處理結果 (PROCESS_RESULT, SUCCESS:成功, FAILED:失敗, PENDING:待處理)
        [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員 (PROCESS_USER_ID)
        [ProcessUserName] NVARCHAR(100) NULL, -- 處理人員名稱 (PROCESS_USER_NAME)
        [ProcessMemo] NVARCHAR(500) NULL, -- 處理備註 (PROCESS_MEMO)
        [ApprovalUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVAL_USER_ID)
        [ApprovalDate] DATETIME2 NULL, -- 審核日期 (APPROVAL_DATE)
        [ApprovalStatus] NVARCHAR(10) NULL, -- 審核狀態 (APPROVAL_STATUS, P:待審核, A:已審核, R:已拒絕)
        [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LeaseProcesses_ProcessId] UNIQUE ([ProcessId]),
        CONSTRAINT [FK_LeaseProcesses_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessId] ON [dbo].[LeaseProcesses] ([ProcessId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_LeaseId] ON [dbo].[LeaseProcesses] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessType] ON [dbo].[LeaseProcesses] ([ProcessType]);
    CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessStatus] ON [dbo].[LeaseProcesses] ([ProcessStatus]);
    CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessDate] ON [dbo].[LeaseProcesses] ([ProcessDate]);

    PRINT '資料表 LeaseProcesses 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseProcesses 已存在';
END
GO

-- 2. 租賃處理明細 (LeaseProcessDetails)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseProcessDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseProcessDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [FieldName] NVARCHAR(100) NULL, -- 欄位名稱 (FIELD_NAME)
        [OldValue] NVARCHAR(MAX) NULL, -- 舊值 (OLD_VALUE)
        [NewValue] NVARCHAR(MAX) NULL, -- 新值 (NEW_VALUE)
        [FieldType] NVARCHAR(20) NULL, -- 欄位類型 (FIELD_TYPE, TEXT:文字, NUMBER:數字, DATE:日期, BOOLEAN:布林)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_LeaseProcessDetails_LeaseProcesses] FOREIGN KEY ([ProcessId]) REFERENCES [dbo].[LeaseProcesses] ([ProcessId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseProcessDetails_ProcessId] ON [dbo].[LeaseProcessDetails] ([ProcessId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseProcessDetails_LineNum] ON [dbo].[LeaseProcessDetails] ([ProcessId], [LineNum]);

    PRINT '資料表 LeaseProcessDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseProcessDetails 已存在';
END
GO

-- 3. 租賃處理日誌 (LeaseProcessLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseProcessLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseProcessLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號
        [LogDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 日誌日期 (LOG_DATE)
        [LogType] NVARCHAR(20) NULL, -- 日誌類型 (LOG_TYPE, INFO:資訊, WARNING:警告, ERROR:錯誤)
        [LogMessage] NVARCHAR(MAX) NULL, -- 日誌訊息 (LOG_MESSAGE)
        [LogUserId] NVARCHAR(50) NULL, -- 操作人員 (LOG_USER_ID)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_LeaseProcessLogs_LeaseProcesses] FOREIGN KEY ([ProcessId]) REFERENCES [dbo].[LeaseProcesses] ([ProcessId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseProcessLogs_ProcessId] ON [dbo].[LeaseProcessLogs] ([ProcessId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseProcessLogs_LogDate] ON [dbo].[LeaseProcessLogs] ([LogDate]);

    PRINT '資料表 LeaseProcessLogs 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseProcessLogs 已存在';
END
GO


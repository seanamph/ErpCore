-- 系統功能資料表
-- Identify - 系統識別功能
-- MakeRegFile - 系統註冊功能
-- about - 關於頁面

-- 系統識別設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemIdentities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemIdentities] (
        [IdentityId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SystemId] NVARCHAR(50) NOT NULL,
        [ProjectTitle] NVARCHAR(200) NOT NULL,
        [CompanyTitle] NVARCHAR(200) NULL,
        [EipUrl] NVARCHAR(500) NULL,
        [EipEmbedded] NVARCHAR(10) NULL DEFAULT 'N',
        [ShowTransEffect] NVARCHAR(10) NULL DEFAULT 'N',
        [InitShowTransEffect] NVARCHAR(10) NULL DEFAULT 'N',
        [NoResizeFrame] BIT NOT NULL DEFAULT 1,
        [DebugUser] NVARCHAR(50) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_SystemIdentities_SystemId] UNIQUE ([SystemId])
    );

    CREATE NONCLUSTERED INDEX [IX_SystemIdentities_SystemId] ON [dbo].[SystemIdentities] ([SystemId]);
    CREATE NONCLUSTERED INDEX [IX_SystemIdentities_Status] ON [dbo].[SystemIdentities] ([Status]);
    
    PRINT 'SystemIdentities 表建立成功';
END
GO

-- 系統識別操作日誌表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemIdentityLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemIdentityLogs] (
        [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [IdentityId] BIGINT NULL,
        [UserId] NVARCHAR(50) NOT NULL,
        [OperationType] NVARCHAR(20) NOT NULL,
        [SystemId] NVARCHAR(50) NULL,
        [IpAddress] NVARCHAR(50) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_SystemIdentityLogs_IdentityId] ON [dbo].[SystemIdentityLogs] ([IdentityId]);
    CREATE NONCLUSTERED INDEX [IX_SystemIdentityLogs_UserId] ON [dbo].[SystemIdentityLogs] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_SystemIdentityLogs_CreatedAt] ON [dbo].[SystemIdentityLogs] ([CreatedAt]);
    
    PRINT 'SystemIdentityLogs 表建立成功';
END
GO

-- 系統註冊記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemRegistrations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemRegistrations] (
        [RegistrationId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CompanyId] NVARCHAR(50) NOT NULL,
        [CpuNumber] NVARCHAR(100) NOT NULL,
        [ComputerName] NVARCHAR(100) NOT NULL,
        [MacAddress] NVARCHAR(100) NOT NULL,
        [RegistrationKey] NVARCHAR(500) NOT NULL,
        [ExpiryDate] DATETIME2 NOT NULL,
        [LastDate] NVARCHAR(50) NULL,
        [UseDownGo] NVARCHAR(10) NULL,
        [Ticket] NVARCHAR(10) NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
        [RegistrationFile] VARBINARY(MAX) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_SystemRegistrations_CompanyId_CpuNumber] UNIQUE ([CompanyId], [CpuNumber])
    );

    CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_CompanyId] ON [dbo].[SystemRegistrations] ([CompanyId]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_CpuNumber] ON [dbo].[SystemRegistrations] ([CpuNumber]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_Status] ON [dbo].[SystemRegistrations] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_ExpiryDate] ON [dbo].[SystemRegistrations] ([ExpiryDate]);
    
    PRINT 'SystemRegistrations 表建立成功';
END
GO

-- 系統註冊操作日誌表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemRegistrationLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemRegistrationLogs] (
        [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [RegistrationId] BIGINT NULL,
        [CompanyId] NVARCHAR(50) NOT NULL,
        [OperationType] NVARCHAR(20) NOT NULL,
        [CpuNumber] NVARCHAR(100) NULL,
        [ComputerName] NVARCHAR(100) NULL,
        [MacAddress] NVARCHAR(100) NULL,
        [Result] NVARCHAR(20) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [IpAddress] NVARCHAR(50) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_RegistrationId] ON [dbo].[SystemRegistrationLogs] ([RegistrationId]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_CompanyId] ON [dbo].[SystemRegistrationLogs] ([CompanyId]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_OperationType] ON [dbo].[SystemRegistrationLogs] ([OperationType]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_CreatedAt] ON [dbo].[SystemRegistrationLogs] ([CreatedAt]);
    
    PRINT 'SystemRegistrationLogs 表建立成功';
END
GO

-- 系統註冊申請表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemRegistrationRequests]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemRegistrationRequests] (
        [RequestId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CompanyId] NVARCHAR(50) NOT NULL,
        [UserId] NVARCHAR(50) NOT NULL,
        [CpuNumber] NVARCHAR(100) NOT NULL,
        [ComputerName] NVARCHAR(100) NOT NULL,
        [MacAddress] NVARCHAR(100) NOT NULL,
        [ExpiryDate] DATETIME2 NOT NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
        [ApprovedBy] NVARCHAR(50) NULL,
        [ApprovedAt] DATETIME2 NULL,
        [RejectReason] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    CREATE NONCLUSTERED INDEX [IX_SystemRegistrationRequests_CompanyId] ON [dbo].[SystemRegistrationRequests] ([CompanyId]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrationRequests_UserId] ON [dbo].[SystemRegistrationRequests] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_SystemRegistrationRequests_Status] ON [dbo].[SystemRegistrationRequests] ([Status]);
    
    PRINT 'SystemRegistrationRequests 表建立成功';
END
GO


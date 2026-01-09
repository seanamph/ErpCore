-- =============================================
-- CommunicationModule (XCOM000) 通訊模組資料表
-- =============================================

-- 1. 系統通訊設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemCommunications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemCommunications] (
        [CommunicationId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SystemCode] NVARCHAR(50) NOT NULL, -- 系統代碼
        [SystemName] NVARCHAR(200) NOT NULL, -- 系統名稱
        [CommunicationType] NVARCHAR(20) NOT NULL, -- REST, SOAP, JSON, ETEK
        [EndpointUrl] NVARCHAR(500) NULL, -- 端點URL
        [ApiKey] NVARCHAR(500) NULL, -- API金鑰 (加密)
        [ApiSecret] NVARCHAR(500) NULL, -- API密鑰 (加密)
        [ConfigData] NVARCHAR(MAX) NULL, -- 設定資料 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 1:啟用, 0:停用
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_SystemCommunications_SystemCode] UNIQUE ([SystemCode])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SystemCommunications_SystemCode] ON [dbo].[SystemCommunications] ([SystemCode]);
    CREATE NONCLUSTERED INDEX [IX_SystemCommunications_CommunicationType] ON [dbo].[SystemCommunications] ([CommunicationType]);
    CREATE NONCLUSTERED INDEX [IX_SystemCommunications_Status] ON [dbo].[SystemCommunications] ([Status]);
END
GO

-- 2. 通訊記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommunicationLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CommunicationLogs] (
        [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CommunicationId] BIGINT NOT NULL,
        [SystemCode] NVARCHAR(50) NOT NULL,
        [OperationType] NVARCHAR(20) NOT NULL, -- SEND, RECEIVE, SYNC
        [RequestData] NVARCHAR(MAX) NULL, -- 請求資料 (JSON格式)
        [ResponseData] NVARCHAR(MAX) NULL, -- 回應資料 (JSON格式)
        [Status] NVARCHAR(20) NOT NULL, -- SUCCESS, FAILED, PENDING
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [Duration] INT NULL, -- 執行時間 (毫秒)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CommunicationLogs_SystemCommunications] FOREIGN KEY ([CommunicationId]) REFERENCES [dbo].[SystemCommunications] ([CommunicationId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CommunicationLogs_CommunicationId] ON [dbo].[CommunicationLogs] ([CommunicationId]);
    CREATE NONCLUSTERED INDEX [IX_CommunicationLogs_SystemCode] ON [dbo].[CommunicationLogs] ([SystemCode]);
    CREATE NONCLUSTERED INDEX [IX_CommunicationLogs_Status] ON [dbo].[CommunicationLogs] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_CommunicationLogs_CreatedAt] ON [dbo].[CommunicationLogs] ([CreatedAt]);
END
GO

-- 3. XCOM系統參數表 (XCOM2A0)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[XComSystemParams]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[XComSystemParams] (
        [ParamCode] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 參數代碼
        [ParamName] NVARCHAR(100) NOT NULL, -- 參數名稱
        [ParamValue] NVARCHAR(500) NULL, -- 參數值
        [ParamType] NVARCHAR(20) NULL, -- 參數類型 (STRING, NUMBER, BOOLEAN, DATE等)
        [Description] NVARCHAR(500) NULL, -- 說明
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 A:啟用, I:停用
        [SystemId] NVARCHAR(50) NULL, -- 系統ID
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        [CreatedPriority] INT NULL, -- 建立者等級
        [CreatedGroup] NVARCHAR(50) NULL -- 建立者群組
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_XComSystemParams_ParamName] ON [dbo].[XComSystemParams] ([ParamName]);
    CREATE NONCLUSTERED INDEX [IX_XComSystemParams_Status] ON [dbo].[XComSystemParams] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_XComSystemParams_SystemId] ON [dbo].[XComSystemParams] ([SystemId]);
    CREATE NONCLUSTERED INDEX [IX_XComSystemParams_ParamType] ON [dbo].[XComSystemParams] ([ParamType]);
END
GO


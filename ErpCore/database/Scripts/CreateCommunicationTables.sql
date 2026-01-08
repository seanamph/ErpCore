-- =============================================
-- 通訊與通知類資料表建立腳本
-- =============================================

-- 1. 郵件發送記錄 (EmailLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailLogs] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [FromAddress] NVARCHAR(255) NOT NULL,
        [FromName] NVARCHAR(100) NULL,
        [ToAddress] NVARCHAR(MAX) NOT NULL,
        [CcAddress] NVARCHAR(MAX) NULL,
        [BccAddress] NVARCHAR(MAX) NULL,
        [Subject] NVARCHAR(500) NOT NULL,
        [Body] NVARCHAR(MAX) NULL,
        [BodyType] NVARCHAR(20) NULL DEFAULT 'Text',
        [Priority] INT NULL DEFAULT 3,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [SentAt] DATETIME2 NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [SmtpServer] NVARCHAR(255) NULL,
        [SmtpPort] INT NULL,
        [HasAttachment] BIT NOT NULL DEFAULT 0,
        [AttachmentCount] INT NULL DEFAULT 0
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EmailLogs_Status] ON [dbo].[EmailLogs] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_EmailLogs_CreatedAt] ON [dbo].[EmailLogs] ([CreatedAt]);
    CREATE NONCLUSTERED INDEX [IX_EmailLogs_FromAddress] ON [dbo].[EmailLogs] ([FromAddress]);
END
GO

-- 2. 郵件附件 (EmailAttachments)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailAttachments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailAttachments] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [EmailLogId] BIGINT NOT NULL,
        [FileName] NVARCHAR(255) NOT NULL,
        [FilePath] NVARCHAR(500) NOT NULL,
        [FileSize] BIGINT NULL,
        [ContentType] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_EmailAttachments_EmailLogs] FOREIGN KEY ([EmailLogId]) REFERENCES [dbo].[EmailLogs] ([Id]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EmailAttachments_EmailLogId] ON [dbo].[EmailAttachments] ([EmailLogId]);
END
GO

-- 3. 簡訊發送記錄 (SmsLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmsLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SmsLogs] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PhoneNumber] NVARCHAR(20) NOT NULL,
        [Message] NVARCHAR(500) NOT NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [SentAt] DATETIME2 NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [Provider] NVARCHAR(50) NULL,
        [ProviderMessageId] NVARCHAR(100) NULL
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SmsLogs_Status] ON [dbo].[SmsLogs] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_SmsLogs_CreatedAt] ON [dbo].[SmsLogs] ([CreatedAt]);
    CREATE NONCLUSTERED INDEX [IX_SmsLogs_PhoneNumber] ON [dbo].[SmsLogs] ([PhoneNumber]);
END
GO

-- 4. 通知模板 (NotificationTemplates)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NotificationTemplates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[NotificationTemplates] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TemplateCode] NVARCHAR(50) NOT NULL,
        [TemplateName] NVARCHAR(100) NOT NULL,
        [TemplateType] NVARCHAR(20) NOT NULL,
        [Subject] NVARCHAR(500) NULL,
        [Body] NVARCHAR(MAX) NOT NULL,
        [BodyType] NVARCHAR(20) NULL DEFAULT 'Text',
        [Parameters] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_NotificationTemplates_TemplateCode] UNIQUE ([TemplateCode])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_NotificationTemplates_TemplateCode] ON [dbo].[NotificationTemplates] ([TemplateCode]);
    CREATE NONCLUSTERED INDEX [IX_NotificationTemplates_TemplateType] ON [dbo].[NotificationTemplates] ([TemplateType]);
END
GO

-- 5. 郵件佇列 (EmailQueue)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailQueue]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailQueue] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [EmailLogId] BIGINT NOT NULL,
        [Priority] INT NOT NULL DEFAULT 3,
        [RetryCount] INT NOT NULL DEFAULT 0,
        [MaxRetryCount] INT NOT NULL DEFAULT 3,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        [NextRetryAt] DATETIME2 NULL,
        [ProcessedAt] DATETIME2 NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_EmailQueue_EmailLogs] FOREIGN KEY ([EmailLogId]) REFERENCES [dbo].[EmailLogs] ([Id]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EmailQueue_Status] ON [dbo].[EmailQueue] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_EmailQueue_Priority] ON [dbo].[EmailQueue] ([Priority], [CreatedAt]);
    CREATE NONCLUSTERED INDEX [IX_EmailQueue_NextRetryAt] ON [dbo].[EmailQueue] ([NextRetryAt]) WHERE [Status] = 'Pending';
END
GO

-- 6. 編碼記錄 (EncodeLogs)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EncodeLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EncodeLogs] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [EncodeType] NVARCHAR(50) NOT NULL,
        [OriginalData] NVARCHAR(MAX) NULL,
        [EncodedData] NVARCHAR(MAX) NULL,
        [KeyKind] NVARCHAR(50) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [Purpose] NVARCHAR(100) NULL
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EncodeLogs_EncodeType] ON [dbo].[EncodeLogs] ([EncodeType]);
    CREATE NONCLUSTERED INDEX [IX_EncodeLogs_CreatedAt] ON [dbo].[EncodeLogs] ([CreatedAt]);
END
GO


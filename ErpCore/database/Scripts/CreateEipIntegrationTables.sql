-- EIP系統整合資料表
-- IMS2EIP - EIP系統整合功能

-- EIP整合設定表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EipIntegrations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EipIntegrations] (
        [IntegrationId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ProgId] NVARCHAR(50) NOT NULL,
        [PageId] NVARCHAR(50) NOT NULL,
        [EipUrl] NVARCHAR(500) NOT NULL,
        [Fid] NVARCHAR(100) NULL,
        [SingleField] NVARCHAR(MAX) NULL,
        [MultiField] NVARCHAR(MAX) NULL,
        [DetailTable] NVARCHAR(200) NULL,
        [MultiMSeqNo] NVARCHAR(50) NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_EipIntegrations_ProgId_PageId] UNIQUE ([ProgId], [PageId])
    );

    CREATE NONCLUSTERED INDEX [IX_EipIntegrations_ProgId] ON [dbo].[EipIntegrations] ([ProgId]);
    CREATE NONCLUSTERED INDEX [IX_EipIntegrations_Status] ON [dbo].[EipIntegrations] ([Status]);
    
    PRINT 'EipIntegrations 表建立成功';
END
GO

-- EIP交易記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EipTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EipTransactions] (
        [TransactionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [IntegrationId] BIGINT NOT NULL,
        [ProgId] NVARCHAR(50) NOT NULL,
        [PageId] NVARCHAR(50) NOT NULL,
        [FlowId] NVARCHAR(100) NULL,
        [RequestData] NVARCHAR(MAX) NULL,
        [ResponseData] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_EipTransactions_EipIntegrations] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[EipIntegrations] ([IntegrationId])
    );

    CREATE NONCLUSTERED INDEX [IX_EipTransactions_IntegrationId] ON [dbo].[EipTransactions] ([IntegrationId]);
    CREATE NONCLUSTERED INDEX [IX_EipTransactions_FlowId] ON [dbo].[EipTransactions] ([FlowId]);
    CREATE NONCLUSTERED INDEX [IX_EipTransactions_Status] ON [dbo].[EipTransactions] ([Status]);
    
    PRINT 'EipTransactions 表建立成功';
END
GO


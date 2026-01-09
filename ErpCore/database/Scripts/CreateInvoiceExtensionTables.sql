-- =============================================
-- InvoiceExtension 電子發票擴展類資料表
-- =============================================

-- 1. 電子發票擴展資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EInvoiceExtensions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EInvoiceExtensions] (
        [ExtensionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceId] BIGINT NOT NULL,
        [ExtensionType] NVARCHAR(50) NOT NULL,
        [ExtensionData] NVARCHAR(MAX) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EInvoiceExtensions_InvoiceId] ON [dbo].[EInvoiceExtensions] ([InvoiceId]);
    CREATE NONCLUSTERED INDEX [IX_EInvoiceExtensions_ExtensionType] ON [dbo].[EInvoiceExtensions] ([ExtensionType]);
END
GO


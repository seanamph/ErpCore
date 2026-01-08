-- SYSW150 - 商品永久變價作業資料表
-- 變價單主檔
CREATE TABLE [dbo].[PriceChangeMasters] (
    [PriceChangeId] NVARCHAR(50) NOT NULL,
    [PriceChangeType] NVARCHAR(10) NOT NULL, -- 1:進價, 2:售價
    [SupplierId] NVARCHAR(50) NULL,
    [LogoId] NVARCHAR(50) NULL,
    [ApplyEmpId] NVARCHAR(50) NULL,
    [ApplyOrgId] NVARCHAR(50) NULL,
    [ApplyDate] DATETIME2 NULL,
    [StartDate] DATETIME2 NULL,
    [ApproveEmpId] NVARCHAR(50) NULL,
    [ApproveDate] DATETIME2 NULL,
    [ConfirmEmpId] NVARCHAR(50) NULL,
    [ConfirmDate] DATETIME2 NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 1:已申請, 2:已審核, 9:已作廢, 10:已確認
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0,
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CreatedPriority] INT NULL,
    [CreatedGroup] NVARCHAR(50) NULL,
    CONSTRAINT [PK_PriceChangeMasters] PRIMARY KEY CLUSTERED ([PriceChangeId], [PriceChangeType])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_SupplierId] ON [dbo].[PriceChangeMasters] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_LogoId] ON [dbo].[PriceChangeMasters] ([LogoId]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_Status] ON [dbo].[PriceChangeMasters] ([Status]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_ApplyDate] ON [dbo].[PriceChangeMasters] ([ApplyDate]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_StartDate] ON [dbo].[PriceChangeMasters] ([StartDate]);

-- 變價單明細
CREATE TABLE [dbo].[PriceChangeDetails] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PriceChangeId] NVARCHAR(50) NOT NULL,
    [PriceChangeType] NVARCHAR(10) NOT NULL,
    [LineNum] INT NOT NULL,
    [GoodsId] NVARCHAR(50) NOT NULL,
    [BeforePrice] DECIMAL(18, 4) NULL DEFAULT 0,
    [AfterPrice] DECIMAL(18, 4) NOT NULL,
    [ChangeQty] DECIMAL(18, 4) NULL DEFAULT 0,
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CreatedPriority] INT NULL,
    [CreatedGroup] NVARCHAR(50) NULL,
    CONSTRAINT [FK_PriceChangeDetails_Masters] FOREIGN KEY ([PriceChangeId], [PriceChangeType]) 
        REFERENCES [dbo].[PriceChangeMasters] ([PriceChangeId], [PriceChangeType]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PriceChangeDetails_PriceChangeId] ON [dbo].[PriceChangeDetails] ([PriceChangeId], [PriceChangeType]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeDetails_GoodsId] ON [dbo].[PriceChangeDetails] ([GoodsId]);


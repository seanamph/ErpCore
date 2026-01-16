-- =============================================
-- 調撥短溢維護作業資料表建立腳本
-- 功能代碼: SYSW384
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 調撥短溢單主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferShortages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransferShortages] (
        [ShortageId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 短溢單號
        [TransferId] NVARCHAR(50) NOT NULL, -- 調撥單號 (關聯至 TransferOrders)
        [ReceiptId] NVARCHAR(50) NULL, -- 驗收單號 (關聯至 TransferReceipts，如有)
        [ShortageDate] DATETIME2 NOT NULL, -- 短溢日期
        [FromShopId] NVARCHAR(50) NOT NULL, -- 調出分店代碼
        [ToShopId] NVARCHAR(50) NOT NULL, -- 調入分店代碼
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (P:待處理, A:已審核, C:已處理, X:已取消)
        [ProcessType] NVARCHAR(20) NULL, -- 處理方式 (ADJUST:調整庫存, PENDING:待處理, PROCESSED:已處理)
        [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員
        [ProcessDate] DATETIME2 NULL, -- 處理日期
        [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員
        [ApproveDate] DATETIME2 NULL, -- 審核日期
        [TotalShortageQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總短溢數量（正數為溢收，負數為短少）
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
        [ShortageReason] NVARCHAR(500) NULL, -- 短溢原因
        [Memo] NVARCHAR(500) NULL, -- 備註
        [IsSettled] BIT NOT NULL DEFAULT 0, -- 是否已日結
        [SettledDate] DATETIME2 NULL, -- 日結日期
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TransferShortages_TransferId] ON [dbo].[TransferShortages] ([TransferId]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortages_ReceiptId] ON [dbo].[TransferShortages] ([ReceiptId]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortages_FromShopId] ON [dbo].[TransferShortages] ([FromShopId]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortages_ToShopId] ON [dbo].[TransferShortages] ([ToShopId]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortages_Status] ON [dbo].[TransferShortages] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortages_ShortageDate] ON [dbo].[TransferShortages] ([ShortageDate]);

    -- 外鍵約束
    ALTER TABLE [dbo].[TransferShortages]
    ADD CONSTRAINT [FK_TransferShortages_TransferOrders] 
    FOREIGN KEY ([TransferId]) REFERENCES [dbo].[TransferOrders] ([TransferId]);

    ALTER TABLE [dbo].[TransferShortages]
    ADD CONSTRAINT [FK_TransferShortages_TransferReceipts] 
    FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[TransferReceipts] ([ReceiptId]);

    PRINT '資料表 TransferShortages 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TransferShortages 已存在';
END
GO

-- 2. 調撥短溢單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferShortageDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransferShortageDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ShortageId] NVARCHAR(50) NOT NULL, -- 短溢單號
        [TransferDetailId] UNIQUEIDENTIFIER NULL, -- 調撥單明細ID (關聯至 TransferOrderDetails)
        [ReceiptDetailId] UNIQUEIDENTIFIER NULL, -- 驗收單明細ID (關聯至 TransferReceiptDetails，如有)
        [LineNum] INT NOT NULL, -- 行號
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
        [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
        [TransferQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調撥數量
        [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗收數量
        [ShortageQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 短溢數量（正數為溢收，負數為短少）
        [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
        [Amount] DECIMAL(18, 4) NULL, -- 金額
        [ShortageReason] NVARCHAR(500) NULL, -- 短溢原因
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TransferShortageDetails_ShortageId] ON [dbo].[TransferShortageDetails] ([ShortageId]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortageDetails_GoodsId] ON [dbo].[TransferShortageDetails] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortageDetails_TransferDetailId] ON [dbo].[TransferShortageDetails] ([TransferDetailId]);
    CREATE NONCLUSTERED INDEX [IX_TransferShortageDetails_ReceiptDetailId] ON [dbo].[TransferShortageDetails] ([ReceiptDetailId]);

    -- 外鍵約束
    ALTER TABLE [dbo].[TransferShortageDetails]
    ADD CONSTRAINT [FK_TransferShortageDetails_TransferShortages] 
    FOREIGN KEY ([ShortageId]) REFERENCES [dbo].[TransferShortages] ([ShortageId]) ON DELETE CASCADE;

    ALTER TABLE [dbo].[TransferShortageDetails]
    ADD CONSTRAINT [FK_TransferShortageDetails_TransferOrderDetails] 
    FOREIGN KEY ([TransferDetailId]) REFERENCES [dbo].[TransferOrderDetails] ([DetailId]);

    ALTER TABLE [dbo].[TransferShortageDetails]
    ADD CONSTRAINT [FK_TransferShortageDetails_TransferReceiptDetails] 
    FOREIGN KEY ([ReceiptDetailId]) REFERENCES [dbo].[TransferReceiptDetails] ([DetailId]);

    PRINT '資料表 TransferShortageDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TransferShortageDetails 已存在';
END
GO

PRINT '調撥短溢維護作業資料表建立完成 (SYSW384)';

-- =============================================
-- 調撥單驗退作業資料表建立腳本
-- 功能代碼: SYSW362
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 調撥驗退單主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferReturns]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransferReturns] (
        [ReturnId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 驗退單號
        [TransferId] NVARCHAR(50) NOT NULL, -- 調撥單號 (關聯至 TransferOrders)
        [ReceiptId] NVARCHAR(50) NULL, -- 原驗收單號 (關聯至 TransferReceipts)
        [ReturnDate] DATETIME2 NOT NULL, -- 驗退日期
        [FromShopId] NVARCHAR(50) NOT NULL, -- 調出分店代碼
        [ToShopId] NVARCHAR(50) NOT NULL, -- 調入分店代碼
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (P:待驗退, R:部分驗退, C:已驗退, X:已取消)
        [ReturnUserId] NVARCHAR(50) NULL, -- 驗退人員
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
        [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量
        [ReturnReason] NVARCHAR(500) NULL, -- 驗退原因
        [Memo] NVARCHAR(500) NULL, -- 備註
        [IsSettled] BIT NOT NULL DEFAULT 0, -- 是否已日結
        [SettledDate] DATETIME2 NULL, -- 日結日期
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TransferReturns_TransferId] ON [dbo].[TransferReturns] ([TransferId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturns_ReceiptId] ON [dbo].[TransferReturns] ([ReceiptId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturns_FromShopId] ON [dbo].[TransferReturns] ([FromShopId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturns_ToShopId] ON [dbo].[TransferReturns] ([ToShopId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturns_Status] ON [dbo].[TransferReturns] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturns_ReturnDate] ON [dbo].[TransferReturns] ([ReturnDate]);

    PRINT '資料表 TransferReturns 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TransferReturns 已存在';
END
GO

-- 2. 調撥驗退單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferReturnDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransferReturnDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ReturnId] NVARCHAR(50) NOT NULL, -- 驗退單號
        [TransferDetailId] UNIQUEIDENTIFIER NULL, -- 調撥單明細ID (關聯至 TransferOrderDetails)
        [ReceiptDetailId] UNIQUEIDENTIFIER NULL, -- 原驗收單明細ID (關聯至 TransferReceiptDetails)
        [LineNum] INT NOT NULL, -- 行號
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
        [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
        [TransferQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調撥數量
        [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 原驗收數量
        [ReturnQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗退數量
        [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
        [Amount] DECIMAL(18, 4) NULL, -- 金額
        [ReturnReason] NVARCHAR(500) NULL, -- 驗退原因
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TransferReturnDetails_ReturnId] ON [dbo].[TransferReturnDetails] ([ReturnId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturnDetails_GoodsId] ON [dbo].[TransferReturnDetails] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturnDetails_TransferDetailId] ON [dbo].[TransferReturnDetails] ([TransferDetailId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReturnDetails_ReceiptDetailId] ON [dbo].[TransferReturnDetails] ([ReceiptDetailId]);

    PRINT '資料表 TransferReturnDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TransferReturnDetails 已存在';
END
GO

-- 3. 外鍵約束 (如果相關資料表存在)
-- 注意: 以下外鍵約束需要根據實際資料表結構調整
-- 如果 TransferOrders、TransferOrderDetails、TransferReceipts、TransferReceiptDetails 等資料表尚未建立，請先建立這些資料表

-- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferOrders]') AND type in (N'U'))
-- BEGIN
--     ALTER TABLE [dbo].[TransferReturns]
--     ADD CONSTRAINT [FK_TransferReturns_TransferOrders] 
--     FOREIGN KEY ([TransferId]) REFERENCES [dbo].[TransferOrders] ([TransferId]);
-- END

-- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferReceipts]') AND type in (N'U'))
-- BEGIN
--     ALTER TABLE [dbo].[TransferReturns]
--     ADD CONSTRAINT [FK_TransferReturns_TransferReceipts] 
--     FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[TransferReceipts] ([ReceiptId]);
-- END

-- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferOrderDetails]') AND type in (N'U'))
-- BEGIN
--     ALTER TABLE [dbo].[TransferReturnDetails]
--     ADD CONSTRAINT [FK_TransferReturnDetails_TransferOrderDetails] 
--     FOREIGN KEY ([TransferDetailId]) REFERENCES [dbo].[TransferOrderDetails] ([DetailId]);
-- END

-- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferReceiptDetails]') AND type in (N'U'))
-- BEGIN
--     ALTER TABLE [dbo].[TransferReturnDetails]
--     ADD CONSTRAINT [FK_TransferReturnDetails_TransferReceiptDetails] 
--     FOREIGN KEY ([ReceiptDetailId]) REFERENCES [dbo].[TransferReceiptDetails] ([DetailId]);
-- END

-- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferReturns]') AND type in (N'U'))
-- BEGIN
--     ALTER TABLE [dbo].[TransferReturnDetails]
--     ADD CONSTRAINT [FK_TransferReturnDetails_TransferReturns] 
--     FOREIGN KEY ([ReturnId]) REFERENCES [dbo].[TransferReturns] ([ReturnId]) ON DELETE CASCADE;
-- END

PRINT '調撥單驗退作業資料表建立完成';
GO

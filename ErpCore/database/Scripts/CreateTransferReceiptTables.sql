-- =============================================
-- 調撥單驗收作業資料表建立腳本
-- 功能代碼: SYSW352
-- 建立日期: 2024-01-01
-- =============================================

-- 1. 調撥驗收單主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferReceipts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransferReceipts] (
        [ReceiptId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 驗收單號
        [TransferId] NVARCHAR(50) NOT NULL, -- 調撥單號 (關聯至 TransferOrders)
        [ReceiptDate] DATETIME2 NOT NULL, -- 驗收日期
        [FromShopId] NVARCHAR(50) NOT NULL, -- 調出分店代碼
        [ToShopId] NVARCHAR(50) NOT NULL, -- 調入分店代碼
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (P:待驗收, R:部分驗收, C:已驗收, X:已取消)
        [ReceiptUserId] NVARCHAR(50) NULL, -- 驗收人員
        [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
        [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量
        [Memo] NVARCHAR(500) NULL, -- 備註
        [IsSettled] BIT NOT NULL DEFAULT 0, -- 是否已日結
        [SettledDate] DATETIME2 NULL, -- 日結日期
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TransferReceipts_TransferId] ON [dbo].[TransferReceipts] ([TransferId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReceipts_FromShopId] ON [dbo].[TransferReceipts] ([FromShopId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReceipts_ToShopId] ON [dbo].[TransferReceipts] ([ToShopId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReceipts_Status] ON [dbo].[TransferReceipts] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_TransferReceipts_ReceiptDate] ON [dbo].[TransferReceipts] ([ReceiptDate]);

    PRINT '資料表 TransferReceipts 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TransferReceipts 已存在';
END
GO

-- 2. 調撥驗收單明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferReceiptDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransferReceiptDetails] (
        [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ReceiptId] NVARCHAR(50) NOT NULL, -- 驗收單號
        [TransferDetailId] UNIQUEIDENTIFIER NULL, -- 調撥單明細ID (關聯至 TransferOrderDetails)
        [LineNum] INT NOT NULL, -- 行號
        [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
        [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
        [TransferQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調撥數量
        [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗收數量
        [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
        [Amount] DECIMAL(18, 4) NULL, -- 金額
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TransferReceiptDetails_ReceiptId] ON [dbo].[TransferReceiptDetails] ([ReceiptId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReceiptDetails_GoodsId] ON [dbo].[TransferReceiptDetails] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_TransferReceiptDetails_TransferDetailId] ON [dbo].[TransferReceiptDetails] ([TransferDetailId]);

    PRINT '資料表 TransferReceiptDetails 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 TransferReceiptDetails 已存在';
END
GO

-- 3. 外鍵約束 (如果相關資料表存在)
-- 注意: 以下外鍵約束需要根據實際資料表結構調整
-- 如果 TransferOrders、TransferOrderDetails 等資料表尚未建立，請先建立這些資料表

-- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferOrders]') AND type in (N'U'))
-- BEGIN
--     ALTER TABLE [dbo].[TransferReceipts]
--     ADD CONSTRAINT [FK_TransferReceipts_TransferOrders] 
--     FOREIGN KEY ([TransferId]) REFERENCES [dbo].[TransferOrders] ([TransferId]);
-- END

-- IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferOrderDetails]') AND type in (N'U'))
-- BEGIN
--     ALTER TABLE [dbo].[TransferReceiptDetails]
--     ADD CONSTRAINT [FK_TransferReceiptDetails_TransferOrderDetails] 
--     FOREIGN KEY ([TransferDetailId]) REFERENCES [dbo].[TransferOrderDetails] ([DetailId]);
-- END

PRINT '調撥單驗收作業資料表建立完成';
GO


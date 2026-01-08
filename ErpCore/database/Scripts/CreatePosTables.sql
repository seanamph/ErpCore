-- POS系統相關資料表

-- POS交易主檔
CREATE TABLE [dbo].[PosTransactions] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransactionId] NVARCHAR(50) NOT NULL, -- 交易編號
    [StoreId] NVARCHAR(50) NOT NULL, -- 店別代號
    [PosId] NVARCHAR(50) NULL, -- POS機號
    [TransactionDate] DATETIME2 NOT NULL, -- 交易日期時間
    [TransactionType] NVARCHAR(20) NOT NULL, -- 交易類型 (Sale/Return/Refund)
    [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 總金額
    [PaymentMethod] NVARCHAR(20) NULL, -- 付款方式
    [CustomerId] NVARCHAR(50) NULL, -- 客戶編號
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- 狀態 (Pending/Synced/Failed)
    [SyncAt] DATETIME2 NULL, -- 同步時間
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [UQ_PosTransactions_TransactionId] UNIQUE ([TransactionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PosTransactions_StoreId] ON [dbo].[PosTransactions] ([StoreId]);
CREATE NONCLUSTERED INDEX [IX_PosTransactions_TransactionDate] ON [dbo].[PosTransactions] ([TransactionDate]);
CREATE NONCLUSTERED INDEX [IX_PosTransactions_Status] ON [dbo].[PosTransactions] ([Status]);

-- POS交易明細
CREATE TABLE [dbo].[PosTransactionDetails] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransactionId] NVARCHAR(50) NOT NULL, -- 交易編號
    [LineNo] INT NOT NULL, -- 行號
    [ProductId] NVARCHAR(50) NOT NULL, -- 商品編號
    [ProductName] NVARCHAR(200) NULL, -- 商品名稱
    [Quantity] DECIMAL(18,3) NOT NULL DEFAULT 0, -- 數量
    [UnitPrice] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 單價
    [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 金額
    [Discount] DECIMAL(18,2) NULL DEFAULT 0, -- 折扣
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PosTransactionDetails_PosTransactions] FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[PosTransactions] ([TransactionId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PosTransactionDetails_TransactionId] ON [dbo].[PosTransactionDetails] ([TransactionId]);
CREATE NONCLUSTERED INDEX [IX_PosTransactionDetails_ProductId] ON [dbo].[PosTransactionDetails] ([ProductId]);

-- POS同步記錄
CREATE TABLE [dbo].[PosSyncLogs] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SyncType] NVARCHAR(20) NOT NULL, -- 同步類型 (Transaction/Product/Inventory)
    [SyncDirection] NVARCHAR(20) NOT NULL, -- 同步方向 (ToIMS/FromPOS)
    [RecordCount] INT NOT NULL DEFAULT 0, -- 記錄筆數
    [SuccessCount] INT NOT NULL DEFAULT 0, -- 成功筆數
    [FailedCount] INT NOT NULL DEFAULT 0, -- 失敗筆數
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Running', -- 狀態 (Running/Completed/Failed)
    [StartTime] DATETIME2 NOT NULL, -- 開始時間
    [EndTime] DATETIME2 NULL, -- 結束時間
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE() -- 建立時間
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PosSyncLogs_SyncType] ON [dbo].[PosSyncLogs] ([SyncType]);
CREATE NONCLUSTERED INDEX [IX_PosSyncLogs_CreatedAt] ON [dbo].[PosSyncLogs] ([CreatedAt]);


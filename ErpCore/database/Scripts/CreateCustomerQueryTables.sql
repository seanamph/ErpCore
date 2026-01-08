-- 客戶查詢作業相關資料表 (CUS5120)

-- 客戶交易記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerTransactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerTransactions] (
        [TransactionId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [CustomerId] NVARCHAR(50) NOT NULL,
        [TransactionDate] DATETIME2 NOT NULL,
        [TransactionNo] NVARCHAR(50) NOT NULL,
        [TransactionType] NVARCHAR(20) NULL, -- 交易類型: SALE, RETURN, ADJUST
        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Status] NVARCHAR(10) NULL,
        [Notes] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CustomerTransactions_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([CustomerId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_CustomerTransactions_CustomerId] ON [dbo].[CustomerTransactions] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_CustomerTransactions_TransactionDate] ON [dbo].[CustomerTransactions] ([TransactionDate]);
    CREATE NONCLUSTERED INDEX [IX_CustomerTransactions_TransactionNo] ON [dbo].[CustomerTransactions] ([TransactionNo]);
    
    PRINT 'CustomerTransactions 表建立成功';
END
ELSE
BEGIN
    PRINT 'CustomerTransactions 表已存在';
END
GO

-- 查詢歷史記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QueryHistory] (
        [HistoryId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [UserId] NVARCHAR(50) NOT NULL,
        [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼: CUS5120
        [QueryName] NVARCHAR(100) NULL, -- 查詢名稱
        [QueryConditions] NVARCHAR(MAX) NULL, -- 查詢條件(JSON格式)
        [IsFavorite] BIT NULL DEFAULT 0, -- 是否為常用查詢
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_QueryHistory_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_QueryHistory_UserId] ON [dbo].[QueryHistory] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_QueryHistory_ModuleCode] ON [dbo].[QueryHistory] ([ModuleCode]);
    
    PRINT 'QueryHistory 表建立成功';
END
ELSE
BEGIN
    PRINT 'QueryHistory 表已存在';
END
GO


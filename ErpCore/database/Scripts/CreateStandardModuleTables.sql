-- =============================================
-- 標準模組類 (STD3000, STD5000) - 資料庫腳本
-- 功能：標準模組系列功能
-- 建立日期：2025-01-09
-- =============================================

-- 1. STD3000 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Std3000Data]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Std3000Data] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
        [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
        [DataValue] NVARCHAR(200) NULL, -- 資料值
        [DataType] NVARCHAR(20) NULL, -- 資料類型
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SortOrder] INT NULL DEFAULT 0, -- 排序順序
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Std3000Data_DataId] UNIQUE ([DataId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Std3000Data_DataType] ON [dbo].[Std3000Data] ([DataType]);
    CREATE NONCLUSTERED INDEX [IX_Std3000Data_Status] ON [dbo].[Std3000Data] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Std3000Data_SortOrder] ON [dbo].[Std3000Data] ([SortOrder]);
END
GO

-- 2. STD5000 基礎資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Std5000BaseData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Std5000BaseData] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
        [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
        [DataType] NVARCHAR(20) NOT NULL, -- 資料類型 (SYS5110, SYS5120, SYS5130, SYS5140, SYS5150)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Std5000BaseData_DataId_DataType] UNIQUE ([DataId], [DataType])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Std5000BaseData_DataType] ON [dbo].[Std5000BaseData] ([DataType]);
    CREATE NONCLUSTERED INDEX [IX_Std5000BaseData_Status] ON [dbo].[Std5000BaseData] ([Status]);
END
GO

-- 3. STD5000 會員主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Std5000Members]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Std5000Members] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [MemberId] NVARCHAR(50) NOT NULL, -- 會員編號
        [MemberName] NVARCHAR(100) NOT NULL, -- 會員姓名
        [MemberType] NVARCHAR(20) NULL, -- 會員類型
        [IdCard] NVARCHAR(20) NULL, -- 身分證字號
        [Phone] NVARCHAR(20) NULL, -- 電話
        [Email] NVARCHAR(100) NULL, -- 電子郵件
        [Address] NVARCHAR(200) NULL, -- 地址
        [BirthDate] DATETIME2 NULL, -- 生日
        [JoinDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 加入日期
        [Points] DECIMAL(18, 4) NULL DEFAULT 0, -- 積分
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [ShopId] NVARCHAR(50) NULL, -- 店別代碼
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Std5000Members_MemberId] UNIQUE ([MemberId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Std5000Members_MemberType] ON [dbo].[Std5000Members] ([MemberType]);
    CREATE NONCLUSTERED INDEX [IX_Std5000Members_Status] ON [dbo].[Std5000Members] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Std5000Members_ShopId] ON [dbo].[Std5000Members] ([ShopId]);
END
GO

-- 4. STD5000 會員積分明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Std5000MemberPoints]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Std5000MemberPoints] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [MemberId] NVARCHAR(50) NOT NULL, -- 會員編號
        [TransDate] DATETIME2 NOT NULL, -- 交易日期
        [TransType] NVARCHAR(20) NOT NULL, -- 交易類型 (EARN:獲得, USE:使用, EXPIRE:過期)
        [Points] DECIMAL(18, 4) NOT NULL, -- 積分數量
        [TransId] NVARCHAR(50) NULL, -- 交易單號
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [FK_Std5000MemberPoints_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Std5000Members]([MemberId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Std5000MemberPoints_MemberId] ON [dbo].[Std5000MemberPoints] ([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_Std5000MemberPoints_TransDate] ON [dbo].[Std5000MemberPoints] ([TransDate]);
    CREATE NONCLUSTERED INDEX [IX_Std5000MemberPoints_TransType] ON [dbo].[Std5000MemberPoints] ([TransType]);
END
GO

-- 5. STD5000 交易主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Std5000Transactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Std5000Transactions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TransId] NVARCHAR(50) NOT NULL, -- 交易單號
        [TransDate] DATETIME2 NOT NULL, -- 交易日期
        [TransType] NVARCHAR(20) NOT NULL, -- 交易類型 (SALE:銷售, RETURN:退貨, ADJUST:調整)
        [MemberId] NVARCHAR(50) NULL, -- 會員編號
        [ShopId] NVARCHAR(50) NULL, -- 店別代碼
        [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 金額
        [Points] DECIMAL(18, 4) NULL DEFAULT 0, -- 積分
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:正常, C:取消)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Std5000Transactions_TransId] UNIQUE ([TransId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Std5000Transactions_TransDate] ON [dbo].[Std5000Transactions] ([TransDate]);
    CREATE NONCLUSTERED INDEX [IX_Std5000Transactions_TransType] ON [dbo].[Std5000Transactions] ([TransType]);
    CREATE NONCLUSTERED INDEX [IX_Std5000Transactions_MemberId] ON [dbo].[Std5000Transactions] ([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_Std5000Transactions_ShopId] ON [dbo].[Std5000Transactions] ([ShopId]);
END
GO

-- 6. STD5000 交易明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Std5000TransactionDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Std5000TransactionDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TransId] NVARCHAR(50) NOT NULL, -- 交易單號
        [SeqNo] INT NOT NULL, -- 序號
        [ProductId] NVARCHAR(50) NULL, -- 商品編號
        [ProductName] NVARCHAR(100) NULL, -- 商品名稱
        [Qty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 數量
        [Price] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 單價
        [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 金額
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        CONSTRAINT [FK_Std5000TransactionDetails_TransId] FOREIGN KEY ([TransId]) REFERENCES [dbo].[Std5000Transactions]([TransId]),
        CONSTRAINT [UQ_Std5000TransactionDetails_TransId_SeqNo] UNIQUE ([TransId], [SeqNo])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Std5000TransactionDetails_TransId] ON [dbo].[Std5000TransactionDetails] ([TransId]);
    CREATE NONCLUSTERED INDEX [IX_Std5000TransactionDetails_ProductId] ON [dbo].[Std5000TransactionDetails] ([ProductId]);
END
GO


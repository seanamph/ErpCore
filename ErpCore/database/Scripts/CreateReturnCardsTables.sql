-- SYSL310 - 銷退卡管理：銷退卡資料表
-- 建立日期：2025-01-09

-- 主要資料表: ReturnCards (對應舊系統銷退卡)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReturnCards]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReturnCards] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [Uuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- UUID
        [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼
        [CardYear] INT NOT NULL, -- 卡片年度
        [CardMonth] INT NOT NULL, -- 卡片月份
        [CardType] NVARCHAR(20) NULL, -- 卡片類型
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_ReturnCards] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_ReturnCards_Uuid] UNIQUE ([Uuid])
    );
END

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCards_SiteId' AND object_id = OBJECT_ID('ReturnCards'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCards_SiteId] ON [dbo].[ReturnCards] ([SiteId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCards_OrgId' AND object_id = OBJECT_ID('ReturnCards'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCards_OrgId] ON [dbo].[ReturnCards] ([OrgId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCards_CardYear' AND object_id = OBJECT_ID('ReturnCards'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCards_CardYear] ON [dbo].[ReturnCards] ([CardYear]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCards_CardMonth' AND object_id = OBJECT_ID('ReturnCards'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCards_CardMonth] ON [dbo].[ReturnCards] ([CardMonth]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCards_CardYear_CardMonth' AND object_id = OBJECT_ID('ReturnCards'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCards_CardYear_CardMonth] ON [dbo].[ReturnCards] ([CardYear], [CardMonth]);

-- 銷退卡明細檔: ReturnCardDetails
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReturnCardDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ReturnCardDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [ReturnCardId] BIGINT NOT NULL, -- 銷退卡主檔ID
        [EmployeeId] NVARCHAR(50) NULL, -- 員工編號
        [EmployeeName] NVARCHAR(100) NULL, -- 員工姓名
        [ReturnDate] DATETIME2 NULL, -- 銷退日期
        [ReturnReason] NVARCHAR(200) NULL, -- 銷退原因
        [Amount] DECIMAL(18, 2) NULL DEFAULT 0, -- 金額
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [FK_ReturnCardDetails_ReturnCards] FOREIGN KEY ([ReturnCardId]) REFERENCES [dbo].[ReturnCards] ([TKey]) ON DELETE CASCADE
    );
END

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCardDetails_ReturnCardId' AND object_id = OBJECT_ID('ReturnCardDetails'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCardDetails_ReturnCardId] ON [dbo].[ReturnCardDetails] ([ReturnCardId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCardDetails_EmployeeId' AND object_id = OBJECT_ID('ReturnCardDetails'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCardDetails_EmployeeId] ON [dbo].[ReturnCardDetails] ([EmployeeId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReturnCardDetails_ReturnDate' AND object_id = OBJECT_ID('ReturnCardDetails'))
    CREATE NONCLUSTERED INDEX [IX_ReturnCardDetails_ReturnDate] ON [dbo].[ReturnCardDetails] ([ReturnDate]);


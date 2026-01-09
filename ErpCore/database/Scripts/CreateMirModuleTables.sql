-- =============================================
-- MIR模組系列資料表建立腳本
-- 功能代碼: MIRH000, MIRV000, MIRW000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. MIRH000 人事主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MirH000Personnel]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MirH000Personnel] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PersonnelId] NVARCHAR(50) NOT NULL, -- 人事編號
        [PersonnelName] NVARCHAR(100) NOT NULL, -- 人事姓名
        [DepartmentId] NVARCHAR(50) NULL, -- 部門代碼
        [PositionId] NVARCHAR(50) NULL, -- 職位代碼
        [HireDate] DATETIME2 NULL, -- 到職日期
        [ResignDate] DATETIME2 NULL, -- 離職日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:在職, I:離職, L:留停)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_MirH000Personnel_PersonnelId] UNIQUE ([PersonnelId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_MirH000Personnel_DepartmentId] ON [dbo].[MirH000Personnel] ([DepartmentId]);
    CREATE NONCLUSTERED INDEX [IX_MirH000Personnel_Status] ON [dbo].[MirH000Personnel] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_MirH000Personnel_HireDate] ON [dbo].[MirH000Personnel] ([HireDate]);

    PRINT '資料表 MirH000Personnel 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 MirH000Personnel 已存在';
END
GO

-- 2. MIRH000 薪資主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MirH000Salaries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MirH000Salaries] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SalaryId] NVARCHAR(50) NOT NULL, -- 薪資編號
        [PersonnelId] NVARCHAR(50) NOT NULL, -- 人事編號
        [SalaryMonth] NVARCHAR(6) NOT NULL, -- 薪資月份 (YYYYMM)
        [BaseSalary] DECIMAL(18, 4) NULL DEFAULT 0, -- 基本薪資
        [Bonus] DECIMAL(18, 4) NULL DEFAULT 0, -- 獎金
        [TotalSalary] DECIMAL(18, 4) NULL DEFAULT 0, -- 總薪資
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_MirH000Salaries_SalaryId] UNIQUE ([SalaryId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_MirH000Salaries_PersonnelId] ON [dbo].[MirH000Salaries] ([PersonnelId]);
    CREATE NONCLUSTERED INDEX [IX_MirH000Salaries_SalaryMonth] ON [dbo].[MirH000Salaries] ([SalaryMonth]);
    CREATE NONCLUSTERED INDEX [IX_MirH000Salaries_Status] ON [dbo].[MirH000Salaries] ([Status]);

    -- 外鍵約束
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MirH000Personnel]') AND type in (N'U'))
    BEGIN
        ALTER TABLE [dbo].[MirH000Salaries]
        ADD CONSTRAINT [FK_MirH000Salaries_Personnel] 
        FOREIGN KEY ([PersonnelId]) REFERENCES [dbo].[MirH000Personnel] ([PersonnelId]);
    END

    PRINT '資料表 MirH000Salaries 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 MirH000Salaries 已存在';
END
GO

-- 3. MIRV000 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MirV000Data]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MirV000Data] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
        [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
        [DataValue] NVARCHAR(MAX) NULL, -- 資料值
        [DataType] NVARCHAR(20) NULL, -- 資料類型
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SortOrder] INT NULL DEFAULT 0, -- 排序順序
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_MirV000Data_DataId] UNIQUE ([DataId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_MirV000Data_DataType] ON [dbo].[MirV000Data] ([DataType]);
    CREATE NONCLUSTERED INDEX [IX_MirV000Data_Status] ON [dbo].[MirV000Data] ([Status]);

    PRINT '資料表 MirV000Data 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 MirV000Data 已存在';
END
GO

-- 4. MIRW000 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MirW000Data]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MirW000Data] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
        [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
        [DataValue] NVARCHAR(MAX) NULL, -- 資料值
        [DataType] NVARCHAR(20) NULL, -- 資料類型
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SortOrder] INT NULL DEFAULT 0, -- 排序順序
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_MirW000Data_DataId] UNIQUE ([DataId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_MirW000Data_DataType] ON [dbo].[MirW000Data] ([DataType]);
    CREATE NONCLUSTERED INDEX [IX_MirW000Data_Status] ON [dbo].[MirW000Data] ([Status]);

    PRINT '資料表 MirW000Data 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 MirW000Data 已存在';
END
GO

PRINT 'MIR模組系列資料表建立完成';
GO


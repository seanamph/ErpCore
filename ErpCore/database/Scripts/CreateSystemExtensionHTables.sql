-- =============================================
-- 系統擴展H類 (SYSH000_NEW) - 資料庫腳本
-- 功能：人事批量新增 (SYSH3D0_FMI)、系統擴展PH (SYSPH00)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 人事匯入記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonnelImportLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PersonnelImportLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ImportId] NVARCHAR(50) NOT NULL, -- 匯入批次編號
        [FileName] NVARCHAR(500) NULL, -- 檔案名稱
        [TotalCount] INT NULL DEFAULT 0, -- 總筆數
        [SuccessCount] INT NULL DEFAULT 0, -- 成功筆數
        [FailCount] INT NULL DEFAULT 0, -- 失敗筆數
        [ImportStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 匯入狀態 (PENDING, PROCESSING, SUCCESS, FAILED)
        [ImportDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 匯入日期
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_PersonnelImportLogs_ImportId] UNIQUE ([ImportId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PersonnelImportLogs_ImportId] ON [dbo].[PersonnelImportLogs] ([ImportId]);
    CREATE NONCLUSTERED INDEX [IX_PersonnelImportLogs_ImportStatus] ON [dbo].[PersonnelImportLogs] ([ImportStatus]);
    CREATE NONCLUSTERED INDEX [IX_PersonnelImportLogs_ImportDate] ON [dbo].[PersonnelImportLogs] ([ImportDate]);
END
GO

-- 2. 人事匯入明細表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonnelImportDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PersonnelImportDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ImportId] NVARCHAR(50) NOT NULL, -- 匯入批次編號
        [RowNum] INT NOT NULL, -- 行號
        [PersonnelId] NVARCHAR(50) NULL, -- 人事編號
        [PersonnelName] NVARCHAR(100) NULL, -- 人事姓名
        [ImportStatus] NVARCHAR(20) NOT NULL, -- 匯入狀態 (SUCCESS, FAILED)
        [ErrorMessage] NVARCHAR(1000) NULL, -- 錯誤訊息
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_PersonnelImportDetails_PersonnelImportLogs] FOREIGN KEY ([ImportId]) REFERENCES [dbo].[PersonnelImportLogs] ([ImportId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PersonnelImportDetails_ImportId] ON [dbo].[PersonnelImportDetails] ([ImportId]);
    CREATE NONCLUSTERED INDEX [IX_PersonnelImportDetails_ImportStatus] ON [dbo].[PersonnelImportDetails] ([ImportStatus]);
END
GO

-- 3. 員工感應卡主檔表 (SYSPH00)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmpCard]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmpCard] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CardNo] NVARCHAR(50) NOT NULL, -- 感應卡號
        [EmpId] NVARCHAR(50) NOT NULL, -- 員工代號
        [BeginDate] DATETIME2 NULL, -- 開始日期
        [EndDate] DATETIME2 NULL, -- 結束日期
        [CardStatus] NVARCHAR(20) NOT NULL DEFAULT '1', -- 卡片狀態 (1:啟用, 0:停用)
        [Notes] NVARCHAR(500) NULL, -- 備註
        [BUser] NVARCHAR(50) NULL, -- 建立者
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [CUser] NVARCHAR(50) NULL, -- 更新者
        [CTime] DATETIME2 NULL, -- 更新時間
        [CPriority] INT NULL, -- 建立者等級
        [CGroup] NVARCHAR(50) NULL, -- 建立者群組
        CONSTRAINT [PK_EmpCard] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_EmpCard_CardNo] UNIQUE ([CardNo])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EmpCard_CardNo] ON [dbo].[EmpCard] ([CardNo]);
    CREATE NONCLUSTERED INDEX [IX_EmpCard_EmpId] ON [dbo].[EmpCard] ([EmpId]);
    CREATE NONCLUSTERED INDEX [IX_EmpCard_CardStatus] ON [dbo].[EmpCard] ([CardStatus]);
    CREATE NONCLUSTERED INDEX [IX_EmpCard_BeginDate] ON [dbo].[EmpCard] ([BeginDate]);
    CREATE NONCLUSTERED INDEX [IX_EmpCard_EndDate] ON [dbo].[EmpCard] ([EndDate]);
END
GO


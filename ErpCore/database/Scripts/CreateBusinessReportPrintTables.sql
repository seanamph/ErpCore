-- SYSL150 - 業務報表列印作業資料表
-- 建立業務報表列印主檔

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BusinessReportPrint]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BusinessReportPrint] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [GiveYear] INT NOT NULL, -- 發放年度 (GIVE_YEAR)
        [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SITE_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [EmpId] NVARCHAR(50) NOT NULL, -- 員工編號 (EMP_ID)
        [EmpName] NVARCHAR(100) NULL, -- 員工姓名 (EMP_NAME)
        [Qty] DECIMAL(18,2) NULL DEFAULT 0, -- 數量 (QTY)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS) P:待審核, A:已審核, R:已拒絕
        [Verifier] NVARCHAR(50) NULL, -- 審核者 (VERIFIER)
        [VerifyDate] DATETIME2 NULL, -- 審核日期 (VERIFY_DATE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_BusinessReportPrint] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_GiveYear] ON [dbo].[BusinessReportPrint] ([GiveYear]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_SiteId] ON [dbo].[BusinessReportPrint] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_OrgId] ON [dbo].[BusinessReportPrint] ([OrgId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_EmpId] ON [dbo].[BusinessReportPrint] ([EmpId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_Status] ON [dbo].[BusinessReportPrint] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_Verifier] ON [dbo].[BusinessReportPrint] ([Verifier]);
END
GO

-- SYSL160 - 業務報表列印明細作業資料表
-- 建立業務報表列印明細檔

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BusinessReportPrintDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BusinessReportPrintDetail] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [PrintId] BIGINT NOT NULL, -- 報表列印ID (外鍵至BusinessReportPrint)
        [LeaveId] NVARCHAR(50) NULL, -- 請假代碼 (LEAVE_ID)
        [LeaveName] NVARCHAR(100) NULL, -- 請假名稱 (LEAVE_NAME)
        [ActEvent] NVARCHAR(50) NULL, -- 動作事件 (ACT_EVENT)
        [DeductionQty] DECIMAL(18,2) NULL DEFAULT 0, -- 扣款數量 (DEDUCTION_QTY)
        [DeductionQtyDefaultEmpty] NVARCHAR(1) NULL DEFAULT 'N', -- 扣款數量預設為空 (DEDUCTION_QTY_DEFAULT_EMPTY)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [FK_BusinessReportPrintDetail_BusinessReportPrint] FOREIGN KEY ([PrintId]) REFERENCES [dbo].[BusinessReportPrint] ([TKey]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintDetail_PrintId] ON [dbo].[BusinessReportPrintDetail] ([PrintId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintDetail_LeaveId] ON [dbo].[BusinessReportPrintDetail] ([LeaveId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintDetail_ActEvent] ON [dbo].[BusinessReportPrintDetail] ([ActEvent]);
END
GO

-- SYSL161 - 業務報表列印記錄作業資料表
-- 建立業務報表列印記錄檔

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BusinessReportPrintLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BusinessReportPrintLog] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [ReportId] NVARCHAR(50) NOT NULL, -- 報表代碼 (REPORT_ID)
        [ReportName] NVARCHAR(100) NULL, -- 報表名稱 (REPORT_NAME)
        [ReportType] NVARCHAR(20) NULL, -- 報表類型 (REPORT_TYPE)
        [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 列印日期 (PRINT_DATE)
        [PrintUserId] NVARCHAR(50) NULL, -- 列印使用者 (PRINT_USER_ID)
        [PrintUserName] NVARCHAR(100) NULL, -- 列印使用者名稱 (PRINT_USER_NAME)
        [PrintParams] NVARCHAR(MAX) NULL, -- 列印參數 (JSON格式) (PRINT_PARAMS)
        [PrintFormat] NVARCHAR(20) NULL, -- 列印格式 (PRINT_FORMAT) PDF, Excel, Print
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:成功, 0:失敗
        [ErrorMessage] NVARCHAR(500) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
        [FilePath] NVARCHAR(500) NULL, -- 檔案路徑（相對路徑） (FILE_PATH)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        CONSTRAINT [PK_BusinessReportPrintLog] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_ReportId] ON [dbo].[BusinessReportPrintLog] ([ReportId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_PrintDate] ON [dbo].[BusinessReportPrintLog] ([PrintDate]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_PrintUserId] ON [dbo].[BusinessReportPrintLog] ([PrintUserId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_Status] ON [dbo].[BusinessReportPrintLog] ([Status]);
END
GO


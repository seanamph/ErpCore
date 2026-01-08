-- SYSL135 - 業務報表查詢作業資料表
-- 建立業務報表主檔

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BusinessReports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BusinessReports] (
        [ReportId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼
        [SiteName] NVARCHAR(100) NULL, -- 店別名稱
        [CardType] NVARCHAR(20) NOT NULL, -- 卡片類型
        [CardTypeName] NVARCHAR(100) NULL, -- 卡片類型名稱
        [VendorId] NVARCHAR(50) NULL, -- 廠商代碼
        [VendorName] NVARCHAR(100) NULL, -- 廠商名稱
        [StoreId] NVARCHAR(50) NULL, -- 專櫃代碼
        [StoreName] NVARCHAR(100) NULL, -- 專櫃名稱
        [AgreementId] NVARCHAR(50) NULL, -- 合約代碼
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼
        [OrgName] NVARCHAR(100) NULL, -- 組織名稱
        [ActionType] NVARCHAR(20) NULL, -- 動作類型
        [ActionTypeName] NVARCHAR(100) NULL, -- 動作類型名稱
        [ReportDate] DATETIME2 NOT NULL, -- 報表日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_BusinessReports_SiteId] ON [dbo].[BusinessReports] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReports_CardType] ON [dbo].[BusinessReports] ([CardType]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReports_VendorId] ON [dbo].[BusinessReports] ([VendorId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReports_StoreId] ON [dbo].[BusinessReports] ([StoreId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReports_ReportDate] ON [dbo].[BusinessReports] ([ReportDate]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReports_OrgId] ON [dbo].[BusinessReports] ([OrgId]);
END
GO

-- SYSL145 - 業務報表管理作業資料表
-- 建立業務報表管理主檔

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BusinessReportManagement]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BusinessReportManagement] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SITE_ID)
        [Type] NVARCHAR(20) NOT NULL, -- 類型 (TYPE)
        [Id] NVARCHAR(50) NOT NULL, -- ID (ID)
        [UserId] NVARCHAR(50) NULL, -- 使用者編號 (USER_ID)
        [UserName] NVARCHAR(100) NULL, -- 使用者名稱 (USER_NAME)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
        CONSTRAINT [PK_BusinessReportManagement] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_BusinessReportManagement_SiteTypeId] UNIQUE ([SiteId], [Type], [Id])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_SiteId] ON [dbo].[BusinessReportManagement] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_Type] ON [dbo].[BusinessReportManagement] ([Type]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_Id] ON [dbo].[BusinessReportManagement] ([Id]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_UserId] ON [dbo].[BusinessReportManagement] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_Status] ON [dbo].[BusinessReportManagement] ([Status]);
END
GO


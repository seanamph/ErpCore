-- =============================================
-- 潛客主檔資料表 (SYSC165)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProspectMasters]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProspectMasters] (
        [MasterId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 主檔代碼 (MASTER_ID)
        [MasterName] NVARCHAR(100) NOT NULL, -- 主檔名稱 (MASTER_NAME)
        [MasterType] NVARCHAR(20) NULL, -- 主檔類型 (MASTER_TYPE) COMPANY:公司, INDIVIDUAL:個人, OTHER:其他
        [Category] NVARCHAR(50) NULL, -- 分類 (CATEGORY)
        [Industry] NVARCHAR(50) NULL, -- 產業別 (INDUSTRY)
        [BusinessType] NVARCHAR(50) NULL, -- 業種 (BUSINESS_TYPE)
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE', -- 狀態 (STATUS) ACTIVE:有效, INACTIVE:無效, ARCHIVED:已歸檔
        [Priority] INT NULL DEFAULT 0, -- 優先順序 (PRIORITY)
        [Source] NVARCHAR(50) NULL, -- 來源 (SOURCE) REFERRAL:推薦, ADVERTISEMENT:廣告, EXHIBITION:展覽, OTHER:其他
        [ContactPerson] NVARCHAR(50) NULL, -- 聯絡人 (CONTACT_PERSON)
        [ContactTel] NVARCHAR(50) NULL, -- 聯絡電話 (CONTACT_TEL)
        [ContactEmail] NVARCHAR(100) NULL, -- 電子郵件 (CONTACT_EMAIL)
        [ContactAddress] NVARCHAR(200) NULL, -- 聯絡地址 (CONTACT_ADDRESS)
        [Website] NVARCHAR(200) NULL, -- 網站 (WEBSITE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_ProspectMasters] PRIMARY KEY CLUSTERED ([MasterId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_MasterName] ON [dbo].[ProspectMasters] ([MasterName]);
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_MasterType] ON [dbo].[ProspectMasters] ([MasterType]);
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Category] ON [dbo].[ProspectMasters] ([Category]);
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Industry] ON [dbo].[ProspectMasters] ([Industry]);
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_BusinessType] ON [dbo].[ProspectMasters] ([BusinessType]);
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Status] ON [dbo].[ProspectMasters] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Source] ON [dbo].[ProspectMasters] ([Source]);
    CREATE NONCLUSTERED INDEX [IX_ProspectMasters_CreatedAt] ON [dbo].[ProspectMasters] ([CreatedAt]);

    PRINT 'ProspectMasters 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ProspectMasters 資料表已存在';
END
GO


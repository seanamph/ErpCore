-- =============================================
-- 潛客資料表 (SYSC180)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Prospects]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Prospects] (
        [ProspectId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 潛客代碼 (PROSPECT_ID)
        [ProspectName] NVARCHAR(100) NOT NULL, -- 潛客名稱 (PROSPECT_NAME)
        [ContactPerson] NVARCHAR(50) NULL, -- 聯絡人 (CONTACT)
        [ContactTel] NVARCHAR(50) NULL, -- 聯絡電話 (CONTACT_TEL)
        [ContactFax] NVARCHAR(50) NULL, -- 傳真 (CONTACT_FAX)
        [ContactEmail] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
        [ContactAddress] NVARCHAR(200) NULL, -- 聯絡地址 (CONTACT_ADDR)
        [StoreName] NVARCHAR(100) NULL, -- 店名 (STORE_NAME)
        [StoreTel] NVARCHAR(50) NULL, -- 店電話 (STORE_TEL)
        [SiteId] NVARCHAR(50) NULL, -- 據點代碼 (SITE_ID)
        [RecruitId] NVARCHAR(50) NULL, -- 招商代碼 (RECRUIT_ID)
        [StoreId] NVARCHAR(50) NULL, -- 店別代碼 (STORE_ID)
        [VendorId] NVARCHAR(50) NULL, -- 廠商代碼 (VENDOR_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [BtypeId] NVARCHAR(50) NULL, -- 業種代碼 (BTYPE_ID)
        [SalesType] NVARCHAR(50) NULL, -- 銷售型態 (SALES_TYPE)
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (STATUS) PENDING:待訪談, INTERVIEWING:訪談中, SIGNED:已簽約, CANCELLED:已取消
        [OverallStatus] NVARCHAR(20) NULL, -- 整體狀態 (OVERALL_STATUS)
        [PaperType] NVARCHAR(20) NULL, -- 營業型態 (PAPER_TYPE)
        [LocationType] NVARCHAR(20) NULL, -- 正櫃櫃別 (LOCATION_TYPE)
        [DecoType] NVARCHAR(20) NULL, -- 裝潢方式 (DECO_TYPE)
        [CommType] NVARCHAR(20) NULL, -- 抽成別 (COMM_TYPE)
        [PdType] NVARCHAR(20) NULL, -- 抽成別 (PD_TYPE)
        [BaseRent] DECIMAL(18,2) NULL, -- 基本租金 (BASE_RENT)
        [Deposit] DECIMAL(18,2) NULL, -- 保證金 (DEPOSIT)
        [CreditCard] NVARCHAR(10) NULL, -- 信用卡 (CREDIT_CARD)
        [TargetAmountM] DECIMAL(18,2) NULL, -- 目標金額(月) (TARGET_AMOUNT_M)
        [TargetAmountV] DECIMAL(18,2) NULL, -- 目標金額(年) (TARGET_AMOUNT_V)
        [ExerciseFees] DECIMAL(18,2) NULL, -- 運動費用 (EXCERCISE_FEES)
        [CheckDay] INT NULL, -- 檢查日 (CHECK_DAY)
        [AgmDateB] DATETIME2 NULL, -- 會議日期起 (AGM_DATE_B)
        [AgmDateE] DATETIME2 NULL, -- 會議日期迄 (AGM_DATE_E)
        [ContractProidB] NVARCHAR(50) NULL, -- 合約專案代碼起 (CONTRACT_PROID_B)
        [ContractProidE] NVARCHAR(50) NULL, -- 合約專案代碼迄 (CONTRACT_PROID_E)
        [FeedbackDate] DATETIME2 NULL, -- 回饋日期 (FEEDBACK_DATE)
        [DueDate] DATETIME2 NULL, -- 到期日 (DUE_DATE)
        [ContactDate] DATETIME2 NULL, -- 聯絡日期 (CONTACT_DATE)
        [VersionNo] NVARCHAR(20) NULL, -- 版本號 (VERSION_NO)
        [GuiId] NVARCHAR(50) NULL, -- GUI ID (GUI_ID)
        [BankId] NVARCHAR(50) NULL, -- 銀行代碼 (BANK_ID)
        [AccName] NVARCHAR(100) NULL, -- 帳戶名稱 (ACC_NAME)
        [AccNo] NVARCHAR(50) NULL, -- 帳戶號碼 (ACC_NO)
        [InvEmail] NVARCHAR(100) NULL, -- 發票電子郵件 (INV_EMAIL)
        [EdcYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否使用業主商店代號 (EDC_YN) Y:是, N:否
        [ReceYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否開立業主發票 (RECE_YN) Y:是, N:否
        [PosYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否使用業主收銀機 (POS_YN) Y:是, N:否
        [CashYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否為業主收現 (CASH_YN) Y:是, N:否
        [CommYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否為抽成 (COMM_YN) Y:是, N:否
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Prospects] PRIMARY KEY CLUSTERED ([ProspectId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Prospects_ProspectName] ON [dbo].[Prospects] ([ProspectName]);
    CREATE NONCLUSTERED INDEX [IX_Prospects_Status] ON [dbo].[Prospects] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Prospects_SiteId] ON [dbo].[Prospects] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_Prospects_RecruitId] ON [dbo].[Prospects] ([RecruitId]);
    CREATE NONCLUSTERED INDEX [IX_Prospects_StoreId] ON [dbo].[Prospects] ([StoreId]);
    CREATE NONCLUSTERED INDEX [IX_Prospects_VendorId] ON [dbo].[Prospects] ([VendorId]);
    CREATE NONCLUSTERED INDEX [IX_Prospects_ContactDate] ON [dbo].[Prospects] ([ContactDate]);
    CREATE NONCLUSTERED INDEX [IX_Prospects_CreatedAt] ON [dbo].[Prospects] ([CreatedAt]);

    PRINT 'Prospects 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Prospects 資料表已存在';
END
GO


-- =============================================
-- 採購供應商管理類資料表建立腳本
-- 功能代碼: SYSP000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. 供應商主檔 (Suppliers)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Suppliers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Suppliers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPPLIER_ID)
        [SupplierName] NVARCHAR(200) NOT NULL, -- 供應商名稱 (SUPPLIER_NAME)
        [SupplierNameE] NVARCHAR(200) NULL, -- 供應商英文名稱
        [ContactPerson] NVARCHAR(100) NULL, -- 聯絡人 (CONTACT_PERSON)
        [Phone] NVARCHAR(20) NULL, -- 電話 (PHONE)
        [Fax] NVARCHAR(20) NULL, -- 傳真 (FAX)
        [Email] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
        [Address] NVARCHAR(500) NULL, -- 地址 (ADDRESS)
        [PaymentTerms] NVARCHAR(50) NULL, -- 付款條件 (PAYMENT_TERMS)
        [TaxId] NVARCHAR(20) NULL, -- 統一編號 (TAX_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [Rating] NVARCHAR(10) NULL, -- 評等 (RATING)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Suppliers_SupplierId] UNIQUE ([SupplierId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Suppliers_SupplierId] ON [dbo].[Suppliers] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_Suppliers_Status] ON [dbo].[Suppliers] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Suppliers_Rating] ON [dbo].[Suppliers] ([Rating]);

    PRINT '資料表 Suppliers 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Suppliers 已存在';
END
GO

-- 2. 付款單主檔 (Payments)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payments] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PaymentId] NVARCHAR(50) NOT NULL, -- 付款單號 (PAYMENT_ID)
        [PaymentDate] DATETIME2 NOT NULL, -- 付款日期 (PAYMENT_DATE)
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPPLIER_ID)
        [PaymentType] NVARCHAR(20) NOT NULL, -- 付款類型 (PAYMENT_TYPE)
        [Amount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 付款金額 (AMOUNT)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
        [BankAccountId] NVARCHAR(50) NULL, -- 銀行帳戶編號 (BANK_ACCOUNT_ID)
        [CheckNumber] NVARCHAR(50) NULL, -- 支票號碼 (CHECK_NUMBER)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, A:已確認, C:已取消)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Payments_PaymentId] UNIQUE ([PaymentId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Payments_PaymentId] ON [dbo].[Payments] ([PaymentId]);
    CREATE NONCLUSTERED INDEX [IX_Payments_SupplierId] ON [dbo].[Payments] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_Payments_PaymentDate] ON [dbo].[Payments] ([PaymentDate]);
    CREATE NONCLUSTERED INDEX [IX_Payments_Status] ON [dbo].[Payments] ([Status]);

    PRINT '資料表 Payments 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Payments 已存在';
END
GO

-- 3. 銀行帳戶主檔 (BankAccounts)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BankAccounts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [BankAccountId] NVARCHAR(50) NOT NULL, -- 銀行帳戶編號 (ACCT_ID)
        [BankId] NVARCHAR(50) NOT NULL, -- 銀行代號 (BANK_ID)
        [AccountName] NVARCHAR(200) NOT NULL, -- 帳戶名稱 (ACCT_NAME)
        [AccountNumber] NVARCHAR(50) NOT NULL, -- 帳戶號碼 (ACCT_NO)
        [AccountType] NVARCHAR(20) NULL, -- 帳戶類型 (ACCT_TYPE) 1:活期, 2:定期, 3:外幣
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [Balance] DECIMAL(18, 2) NULL DEFAULT 0, -- 帳戶餘額 (BALANCE)
        [OpeningDate] DATETIME2 NULL, -- 開戶日期 (OPENING_DATE)
        [ClosingDate] DATETIME2 NULL, -- 結清日期 (CLOSING_DATE)
        [ContactPerson] NVARCHAR(50) NULL, -- 聯絡人 (CONTACT_PERSON)
        [ContactPhone] NVARCHAR(50) NULL, -- 聯絡電話 (CONTACT_PHONE)
        [ContactEmail] NVARCHAR(100) NULL, -- 聯絡信箱 (CONTACT_EMAIL)
        [BranchName] NVARCHAR(100) NULL, -- 分行名稱 (BRANCH_NAME)
        [BranchCode] NVARCHAR(50) NULL, -- 分行代號 (BRANCH_CODE)
        [SwiftCode] NVARCHAR(50) NULL, -- SWIFT代號 (SWIFT_CODE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_BankAccounts] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_BankAccounts_BankAccountId] UNIQUE ([BankAccountId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_BankAccountId] ON [dbo].[BankAccounts] ([BankAccountId]);
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_BankId] ON [dbo].[BankAccounts] ([BankId]);
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_AccountNumber] ON [dbo].[BankAccounts] ([AccountNumber]);
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_Status] ON [dbo].[BankAccounts] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_AccountType] ON [dbo].[BankAccounts] ([AccountType]);
    CREATE NONCLUSTERED INDEX [IX_BankAccounts_CurrencyId] ON [dbo].[BankAccounts] ([CurrencyId]);

    PRINT '資料表 BankAccounts 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 BankAccounts 已存在';
    -- 檢查並添加缺失的欄位
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Balance')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [Balance] DECIMAL(18, 2) NULL DEFAULT 0;
        PRINT '已添加欄位 Balance';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'OpeningDate')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [OpeningDate] DATETIME2 NULL;
        PRINT '已添加欄位 OpeningDate';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ClosingDate')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [ClosingDate] DATETIME2 NULL;
        PRINT '已添加欄位 ClosingDate';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ContactPerson')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [ContactPerson] NVARCHAR(50) NULL;
        PRINT '已添加欄位 ContactPerson';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ContactPhone')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [ContactPhone] NVARCHAR(50) NULL;
        PRINT '已添加欄位 ContactPhone';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'ContactEmail')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [ContactEmail] NVARCHAR(100) NULL;
        PRINT '已添加欄位 ContactEmail';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'BranchName')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [BranchName] NVARCHAR(100) NULL;
        PRINT '已添加欄位 BranchName';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'BranchCode')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [BranchCode] NVARCHAR(50) NULL;
        PRINT '已添加欄位 BranchCode';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'SwiftCode')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [SwiftCode] NVARCHAR(50) NULL;
        PRINT '已添加欄位 SwiftCode';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Notes')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [Notes] NVARCHAR(500) NULL;
        PRINT '已添加欄位 Notes';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'CreatedPriority')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [CreatedPriority] INT NULL;
        PRINT '已添加欄位 CreatedPriority';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'CreatedGroup')
    BEGIN
        ALTER TABLE [dbo].[BankAccounts] ADD [CreatedGroup] NVARCHAR(50) NULL;
        PRINT '已添加欄位 CreatedGroup';
    END
    -- 將 Memo 欄位重命名為 Notes（如果存在 Memo）
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Memo' AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BankAccounts]') AND name = 'Notes'))
    BEGIN
        EXEC sp_rename '[dbo].[BankAccounts].[Memo]', 'Notes', 'COLUMN';
        PRINT '已將欄位 Memo 重命名為 Notes';
    END
END
GO

-- 4. 付款單主檔 (PaymentVouchers) - SYSP271-SYSP2B0
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentVouchers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PaymentVouchers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PaymentNo] NVARCHAR(50) NOT NULL, -- 付款單號 (PAYMENT_NO)
        [PaymentDate] DATETIME2 NOT NULL, -- 付款日期 (PAYMENT_DATE)
        [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPPLIER_ID)
        [PaymentAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 付款金額 (PAYMENT_AMOUNT)
        [PaymentMethod] NVARCHAR(20) NULL, -- 付款方式 (PAYMENT_METHOD)
        [BankAccount] NVARCHAR(50) NULL, -- 銀行帳號 (BANK_ACCOUNT)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT', -- 狀態 (STATUS, DRAFT:草稿, CONFIRMED:確認, PAID:已付款)
        [Verifier] NVARCHAR(50) NULL, -- 審核者 (VERIFIER)
        [VerifyDate] DATETIME2 NULL, -- 審核日期 (VERIFY_DATE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_PaymentVouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PaymentVouchers_PaymentNo] UNIQUE ([PaymentNo])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_PaymentNo] ON [dbo].[PaymentVouchers] ([PaymentNo]);
    CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_SupplierId] ON [dbo].[PaymentVouchers] ([SupplierId]);
    CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_PaymentDate] ON [dbo].[PaymentVouchers] ([PaymentDate]);
    CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_Status] ON [dbo].[PaymentVouchers] ([Status]);

    PRINT '資料表 PaymentVouchers 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PaymentVouchers 已存在';
END
GO

-- 5. 採購其他功能主檔 (PurchaseOtherFunctions)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOtherFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseOtherFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼
        [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱
        [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (TOOL:工具, AUX:輔助, PROCESS:處理)
        [FunctionDesc] NVARCHAR(500) NULL, -- 功能說明
        [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能配置 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_PurchaseOtherFunctions_FunctionId] UNIQUE ([FunctionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_FunctionId] ON [dbo].[PurchaseOtherFunctions] ([FunctionId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_FunctionType] ON [dbo].[PurchaseOtherFunctions] ([FunctionType]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_Status] ON [dbo].[PurchaseOtherFunctions] ([Status]);

    PRINT '資料表 PurchaseOtherFunctions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseOtherFunctions 已存在';
END
GO

-- 6. 採購報表列印記錄 (PurchaseReportPrints) - 採購報表列印
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseReportPrints]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseReportPrints] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型 (REPORT_TYPE) PO:採購單, SU:供應商, PY:付款單
        [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (REPORT_CODE)
        [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱 (REPORT_NAME)
        [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 列印日期 (PRINT_DATE)
        [PrintUserId] NVARCHAR(50) NOT NULL, -- 列印使用者 (PRINT_USER_ID)
        [PrintUserName] NVARCHAR(100) NULL, -- 列印使用者名稱 (PRINT_USER_NAME)
        [FilterConditions] NVARCHAR(MAX) NULL, -- 篩選條件 (FILTER_CONDITIONS, JSON格式)
        [PrintSettings] NVARCHAR(MAX) NULL, -- 列印設定 (PRINT_SETTINGS, JSON格式)
        [FileFormat] NVARCHAR(20) NULL DEFAULT 'PDF', -- 檔案格式 (FILE_FORMAT) PDF, Excel
        [FilePath] NVARCHAR(500) NULL, -- 檔案路徑 (FILE_PATH)
        [FileName] NVARCHAR(200) NULL, -- 檔案名稱 (FILE_NAME)
        [FileSize] BIGINT NULL, -- 檔案大小 (FILE_SIZE, bytes)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'S', -- 狀態 (STATUS) S:成功, F:失敗, P:處理中
        [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
        [PageCount] INT NULL DEFAULT 0, -- 頁數 (PAGE_COUNT)
        [RecordCount] INT NULL DEFAULT 0, -- 記錄數 (RECORD_COUNT)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
        CONSTRAINT [PK_PurchaseReportPrints] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportPrints_ReportType] ON [dbo].[PurchaseReportPrints] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportPrints_ReportCode] ON [dbo].[PurchaseReportPrints] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportPrints_PrintDate] ON [dbo].[PurchaseReportPrints] ([PrintDate]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportPrints_PrintUserId] ON [dbo].[PurchaseReportPrints] ([PrintUserId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportPrints_Status] ON [dbo].[PurchaseReportPrints] ([Status]);

    PRINT '資料表 PurchaseReportPrints 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseReportPrints 已存在';
END
GO

-- 7. 採購報表模板 (PurchaseReportTemplates) - 採購報表列印
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseReportTemplates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseReportTemplates] (
        [TemplateId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型
        [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼
        [TemplateName] NVARCHAR(200) NOT NULL, -- 模板名稱
        [TemplatePath] NVARCHAR(500) NULL, -- 模板路徑 (RDLC, Crystal Reports等)
        [TemplateType] NVARCHAR(20) NOT NULL, -- 模板類型 (RDLC, Crystal, HTML)
        [TemplateContent] NVARCHAR(MAX) NULL, -- 模板內容
        [IsDefault] BIT NOT NULL DEFAULT 0, -- 是否為預設模板
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_PurchaseReportTemplates_ReportCode] UNIQUE ([ReportCode])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportTemplates_ReportType] ON [dbo].[PurchaseReportTemplates] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportTemplates_ReportCode] ON [dbo].[PurchaseReportTemplates] ([ReportCode]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseReportTemplates_Status] ON [dbo].[PurchaseReportTemplates] ([Status]);

    PRINT '資料表 PurchaseReportTemplates 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseReportTemplates 已存在';
END
GO

-- 8. 採購擴展功能主檔 (PurchaseExtendedFunctions) - SYSP610
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseExtendedFunctions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseExtendedFunctions] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ExtFunctionId] NVARCHAR(50) NOT NULL, -- 擴展功能代碼
        [ExtFunctionName] NVARCHAR(100) NOT NULL, -- 擴展功能名稱
        [ExtFunctionType] NVARCHAR(20) NULL, -- 擴展功能類型
        [ExtFunctionDesc] NVARCHAR(500) NULL, -- 擴展功能說明
        [ExtFunctionConfig] NVARCHAR(MAX) NULL, -- 擴展功能配置 (JSON格式)
        [ParameterConfig] NVARCHAR(MAX) NULL, -- 參數配置 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_PurchaseExtendedFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PurchaseExtendedFunctions_ExtFunctionId] UNIQUE ([ExtFunctionId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedFunctions_ExtFunctionId] ON [dbo].[PurchaseExtendedFunctions] ([ExtFunctionId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedFunctions_ExtFunctionType] ON [dbo].[PurchaseExtendedFunctions] ([ExtFunctionType]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedFunctions_Status] ON [dbo].[PurchaseExtendedFunctions] ([Status]);

    PRINT '資料表 PurchaseExtendedFunctions 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseExtendedFunctions 已存在';
END
GO

-- 9. 採購擴展維護主檔 (PurchaseExtendedMaintenance) - SYSPA10-SYSPB60
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseExtendedMaintenance]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseExtendedMaintenance] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [MaintenanceId] NVARCHAR(50) NOT NULL, -- 維護代碼
        [MaintenanceName] NVARCHAR(100) NOT NULL, -- 維護名稱
        [MaintenanceType] NVARCHAR(20) NULL, -- 維護類型
        [MaintenanceDesc] NVARCHAR(500) NULL, -- 維護說明
        [MaintenanceConfig] NVARCHAR(MAX) NULL, -- 維護配置 (JSON格式)
        [ParameterConfig] NVARCHAR(MAX) NULL, -- 參數配置 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_PurchaseExtendedMaintenance] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_PurchaseExtendedMaintenance_MaintenanceId] UNIQUE ([MaintenanceId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedMaintenance_MaintenanceId] ON [dbo].[PurchaseExtendedMaintenance] ([MaintenanceId]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedMaintenance_MaintenanceType] ON [dbo].[PurchaseExtendedMaintenance] ([MaintenanceType]);
    CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedMaintenance_Status] ON [dbo].[PurchaseExtendedMaintenance] ([Status]);

    PRINT '資料表 PurchaseExtendedMaintenance 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 PurchaseExtendedMaintenance 已存在';
END
GO


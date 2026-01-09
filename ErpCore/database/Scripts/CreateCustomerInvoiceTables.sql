-- 客戶與發票管理類資料表 (SYS2000)
-- 包含：客戶資料維護、發票列印作業、郵件傳真作業、總帳資料維護

-- =============================================
-- CustomerData - 客戶資料維護相關資料表
-- =============================================

-- Customers (客戶主檔) - 如果已存在則跳過
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerData] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶編號 (CUST_ID)
        [CustomerName] NVARCHAR(200) NOT NULL, -- 客戶名稱 (CUST_NAME)
        [CustomerType] NVARCHAR(20) NULL, -- 客戶類型 (CUST_TYPE, C:客戶, V:廠商)
        [TaxId] NVARCHAR(20) NULL, -- 統一編號 (TAX_ID)
        [ContactPerson] NVARCHAR(100) NULL, -- 聯絡人 (CONTACT_PERSON)
        [ContactPhone] NVARCHAR(50) NULL, -- 聯絡電話 (CONTACT_PHONE)
        [ContactEmail] NVARCHAR(100) NULL, -- 聯絡信箱 (CONTACT_EMAIL)
        [ContactFax] NVARCHAR(50) NULL, -- 傳真號碼 (CONTACT_FAX)
        [Address] NVARCHAR(500) NULL, -- 地址 (ADDRESS)
        [CityId] NVARCHAR(50) NULL, -- 城市代碼 (CITY_ID)
        [ZoneId] NVARCHAR(50) NULL, -- 區域代碼 (ZONE_ID)
        [ZipCode] NVARCHAR(10) NULL, -- 郵遞區號 (ZIP_CODE)
        [PaymentTerm] NVARCHAR(20) NULL, -- 付款條件 (PAYMENT_TERM)
        [CreditLimit] DECIMAL(18, 4) NULL DEFAULT 0, -- 信用額度 (CREDIT_LIMIT)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [Memo] NVARCHAR(1000) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_CustomerData] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_CustomerData_CustomerId] UNIQUE ([CustomerId])
    );

    CREATE NONCLUSTERED INDEX [IX_CustomerData_CustomerId] ON [dbo].[CustomerData] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_CustomerData_CustomerName] ON [dbo].[CustomerData] ([CustomerName]);
    CREATE NONCLUSTERED INDEX [IX_CustomerData_TaxId] ON [dbo].[CustomerData] ([TaxId]);
    CREATE NONCLUSTERED INDEX [IX_CustomerData_CustomerType] ON [dbo].[CustomerData] ([CustomerType]);
    CREATE NONCLUSTERED INDEX [IX_CustomerData_Status] ON [dbo].[CustomerData] ([Status]);
END
GO

-- CustomerContacts - 客戶聯絡人
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerContacts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerContacts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶編號
        [ContactName] NVARCHAR(100) NOT NULL, -- 聯絡人姓名
        [ContactTitle] NVARCHAR(50) NULL, -- 職稱
        [ContactPhone] NVARCHAR(50) NULL, -- 電話
        [ContactMobile] NVARCHAR(50) NULL, -- 手機
        [ContactEmail] NVARCHAR(100) NULL, -- 信箱
        [ContactFax] NVARCHAR(50) NULL, -- 傳真
        [IsPrimary] BIT NULL DEFAULT 0, -- 是否主要聯絡人
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CustomerContacts_CustomerData] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerData] ([CustomerId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_CustomerContacts_CustomerId] ON [dbo].[CustomerContacts] ([CustomerId]);
END
GO

-- CustomerAddresses - 客戶地址
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerAddresses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerAddresses] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶編號
        [AddressType] NVARCHAR(20) NOT NULL, -- 地址類型 (BILLING:帳單地址, SHIPPING:送貨地址, REGISTERED:登記地址)
        [Address] NVARCHAR(500) NOT NULL, -- 地址
        [CityId] NVARCHAR(50) NULL, -- 城市代碼
        [ZoneId] NVARCHAR(50) NULL, -- 區域代碼
        [ZipCode] NVARCHAR(10) NULL, -- 郵遞區號
        [IsDefault] BIT NULL DEFAULT 0, -- 是否預設地址
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CustomerAddresses_CustomerData] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerData] ([CustomerId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_CustomerAddresses_CustomerId] ON [dbo].[CustomerAddresses] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_CustomerAddresses_AddressType] ON [dbo].[CustomerAddresses] ([AddressType]);
END
GO

-- CustomerBankAccounts - 客戶銀行帳戶
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerBankAccounts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerBankAccounts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶編號
        [BankId] NVARCHAR(50) NOT NULL, -- 銀行代碼
        [AccountNo] NVARCHAR(50) NOT NULL, -- 帳號
        [AccountName] NVARCHAR(200) NULL, -- 戶名
        [IsDefault] BIT NULL DEFAULT 0, -- 是否預設帳戶
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CustomerBankAccounts_CustomerData] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CustomerData] ([CustomerId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_CustomerBankAccounts_CustomerId] ON [dbo].[CustomerBankAccounts] ([CustomerId]);
END
GO

-- =============================================
-- InvoicePrint - 發票列印作業相關資料表
-- =============================================

-- Invoices - 發票主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Invoices] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceNo] NVARCHAR(50) NOT NULL, -- 發票號碼 (INV_NO)
        [InvoiceType] NVARCHAR(20) NOT NULL, -- 發票類型 (INV_TYPE, NORMAL:一般, EINVOICE:電子發票, CHANGE:變更)
        [InvoiceDate] DATETIME2 NOT NULL, -- 發票日期 (INV_DATE)
        [CustomerId] NVARCHAR(50) NULL, -- 客戶編號 (CUST_ID)
        [StoreId] NVARCHAR(50) NULL, -- 店別代碼 (STORE_ID)
        [TotalAmount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
        [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMT)
        [Amount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 金額 (AMOUNT)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT', -- 狀態 (STATUS, DRAFT:草稿, ISSUED:已開立, CANCELLED:已作廢)
        [PrintCount] INT NULL DEFAULT 0, -- 列印次數 (PRINT_COUNT)
        [LastPrintDate] DATETIME2 NULL, -- 最後列印日期 (LAST_PRINT_DATE)
        [LastPrintUser] NVARCHAR(50) NULL, -- 最後列印人員 (LAST_PRINT_USER)
        [PrintFormat] NVARCHAR(50) NULL, -- 列印格式 (PRINT_FORMAT)
        [Memo] NVARCHAR(1000) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Invoices_InvoiceNo] UNIQUE ([InvoiceNo])
    );

    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceNo] ON [dbo].[Invoices] ([InvoiceNo]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_CustomerId] ON [dbo].[Invoices] ([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceDate] ON [dbo].[Invoices] ([InvoiceDate]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_Status] ON [dbo].[Invoices] ([Status]);
END
GO

-- InvoiceDetails - 發票明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InvoiceDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceNo] NVARCHAR(50) NOT NULL, -- 發票號碼
        [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
        [GoodsId] NVARCHAR(50) NULL, -- 商品編號 (G_ID)
        [GoodsName] NVARCHAR(200) NOT NULL, -- 商品名稱 (G_NAME)
        [Qty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 數量 (QTY)
        [UnitPrice] DECIMAL(18, 4) NOT NULL, -- 單價 (UNIT_PRICE)
        [Amount] DECIMAL(18, 4) NOT NULL, -- 金額 (AMOUNT)
        [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率 (TAX_RATE)
        [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMT)
        [UnitId] NVARCHAR(50) NULL, -- 單位 (UNIT_ID)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_InvoiceDetails_Invoices] FOREIGN KEY ([InvoiceNo]) REFERENCES [dbo].[Invoices] ([InvoiceNo]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_InvoiceDetails_InvoiceNo] ON [dbo].[InvoiceDetails] ([InvoiceNo]);
END
GO

-- InvoicePrintLogs - 發票列印記錄
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoicePrintLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InvoicePrintLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [InvoiceNo] NVARCHAR(50) NOT NULL, -- 發票號碼
        [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 列印日期
        [PrintUser] NVARCHAR(50) NULL, -- 列印人員
        [PrintFormat] NVARCHAR(50) NULL, -- 列印格式
        [PrintType] NVARCHAR(20) NULL, -- 列印類型 (NORMAL:一般列印, RE_PRINT:重新列印)
        [PrinterName] NVARCHAR(100) NULL, -- 印表機名稱
        [PrintCount] INT NULL DEFAULT 1, -- 列印份數
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_InvoicePrintLogs_Invoices] FOREIGN KEY ([InvoiceNo]) REFERENCES [dbo].[Invoices] ([InvoiceNo])
    );

    CREATE NONCLUSTERED INDEX [IX_InvoicePrintLogs_InvoiceNo] ON [dbo].[InvoicePrintLogs] ([InvoiceNo]);
    CREATE NONCLUSTERED INDEX [IX_InvoicePrintLogs_PrintDate] ON [dbo].[InvoicePrintLogs] ([PrintDate]);
END
GO

-- =============================================
-- MailFax - 郵件傳真作業相關資料表
-- =============================================

-- EmailFaxJobs - 郵件傳真作業記錄
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailFaxJobs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailFaxJobs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [JobId] NVARCHAR(50) NOT NULL, -- 作業編號 (JOB_ID)
        [JobType] NVARCHAR(20) NOT NULL, -- 作業類型 (JOB_TYPE, EMAIL:郵件, FAX:傳真)
        [Subject] NVARCHAR(500) NULL, -- 主旨 (SUBJECT)
        [Recipient] NVARCHAR(500) NOT NULL, -- 收件人 (RECIPIENT)
        [Cc] NVARCHAR(500) NULL, -- 副本 (CC)
        [Bcc] NVARCHAR(500) NULL, -- 密件副本 (BCC)
        [Content] NVARCHAR(MAX) NULL, -- 內容 (CONTENT)
        [AttachmentPath] NVARCHAR(1000) NULL, -- 附件路徑 (ATTACHMENT_PATH)
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (STATUS, PENDING:待發送, SENT:已發送, FAILED:失敗)
        [SendDate] DATETIME2 NULL, -- 發送日期 (SEND_DATE)
        [SendUser] NVARCHAR(50) NULL, -- 發送人員 (SEND_USER)
        [ErrorMessage] NVARCHAR(1000) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
        [RetryCount] INT NULL DEFAULT 0, -- 重試次數 (RETRY_COUNT)
        [MaxRetry] INT NULL DEFAULT 3, -- 最大重試次數 (MAX_RETRY)
        [ScheduleDate] DATETIME2 NULL, -- 排程日期 (SCHEDULE_DATE)
        [TemplateId] NVARCHAR(50) NULL, -- 範本編號 (TEMPLATE_ID)
        [Memo] NVARCHAR(1000) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_EmailFaxJobs] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_EmailFaxJobs_JobId] UNIQUE ([JobId])
    );

    CREATE NONCLUSTERED INDEX [IX_EmailFaxJobs_JobId] ON [dbo].[EmailFaxJobs] ([JobId]);
    CREATE NONCLUSTERED INDEX [IX_EmailFaxJobs_Status] ON [dbo].[EmailFaxJobs] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_EmailFaxJobs_SendDate] ON [dbo].[EmailFaxJobs] ([SendDate]);
    CREATE NONCLUSTERED INDEX [IX_EmailFaxJobs_JobType] ON [dbo].[EmailFaxJobs] ([JobType]);
END
GO

-- EmailFaxTemplates - 郵件傳真範本
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailFaxTemplates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailFaxTemplates] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TemplateId] NVARCHAR(50) NOT NULL, -- 範本編號
        [TemplateName] NVARCHAR(200) NOT NULL, -- 範本名稱
        [TemplateType] NVARCHAR(20) NOT NULL, -- 範本類型 (EMAIL, FAX)
        [Subject] NVARCHAR(500) NULL, -- 主旨範本
        [Content] NVARCHAR(MAX) NULL, -- 內容範本
        [Variables] NVARCHAR(1000) NULL, -- 變數定義 (JSON格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
        [Memo] NVARCHAR(1000) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_EmailFaxTemplates] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_EmailFaxTemplates_TemplateId] UNIQUE ([TemplateId])
    );
END
GO

-- EmailFaxLogs - 郵件傳真發送記錄
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailFaxLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailFaxLogs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [JobId] NVARCHAR(50) NOT NULL, -- 作業編號
        [LogDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 記錄日期
        [LogType] NVARCHAR(20) NOT NULL, -- 記錄類型 (SEND, RETRY, ERROR, SUCCESS)
        [LogMessage] NVARCHAR(1000) NULL, -- 記錄訊息
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_EmailFaxLogs_EmailFaxJobs] FOREIGN KEY ([JobId]) REFERENCES [dbo].[EmailFaxJobs] ([JobId])
    );

    CREATE NONCLUSTERED INDEX [IX_EmailFaxLogs_JobId] ON [dbo].[EmailFaxLogs] ([JobId]);
    CREATE NONCLUSTERED INDEX [IX_EmailFaxLogs_LogDate] ON [dbo].[EmailFaxLogs] ([LogDate]);
END
GO

-- =============================================
-- LedgerData - 總帳資料維護相關資料表
-- =============================================

-- GeneralLedger - 總帳主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GeneralLedger]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[GeneralLedger] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [LedgerId] NVARCHAR(50) NOT NULL, -- 總帳編號 (LEDGER_ID)
        [LedgerDate] DATETIME2 NOT NULL, -- 總帳日期 (LEDGER_DATE)
        [AccountId] NVARCHAR(50) NOT NULL, -- 會計科目編號 (ACCOUNT_ID)
        [VoucherNo] NVARCHAR(50) NULL, -- 憑證號碼 (VOUCHER_NO)
        [Description] NVARCHAR(500) NULL, -- 說明 (DESCRIPTION)
        [DebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方金額 (DEBIT_AMT)
        [CreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方金額 (CREDIT_AMT)
        [Balance] DECIMAL(18, 4) NULL DEFAULT 0, -- 餘額 (BALANCE)
        [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
        [Period] NVARCHAR(10) NOT NULL, -- 會計期間 (PERIOD, YYYYMM格式)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT', -- 狀態 (STATUS, DRAFT:草稿, POSTED:已過帳, CLOSED:已結帳)
        [Memo] NVARCHAR(1000) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_GeneralLedger] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_GeneralLedger_LedgerId] UNIQUE ([LedgerId])
    );

    CREATE NONCLUSTERED INDEX [IX_GeneralLedger_LedgerId] ON [dbo].[GeneralLedger] ([LedgerId]);
    CREATE NONCLUSTERED INDEX [IX_GeneralLedger_AccountId] ON [dbo].[GeneralLedger] ([AccountId]);
    CREATE NONCLUSTERED INDEX [IX_GeneralLedger_LedgerDate] ON [dbo].[GeneralLedger] ([LedgerDate]);
    CREATE NONCLUSTERED INDEX [IX_GeneralLedger_Period] ON [dbo].[GeneralLedger] ([Period]);
    CREATE NONCLUSTERED INDEX [IX_GeneralLedger_Status] ON [dbo].[GeneralLedger] ([Status]);
END
GO

-- Accounts - 會計科目
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Accounts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [AccountId] NVARCHAR(50) NOT NULL, -- 會計科目編號
        [AccountName] NVARCHAR(200) NOT NULL, -- 會計科目名稱
        [AccountType] NVARCHAR(20) NOT NULL, -- 科目類型 (ASSET:資產, LIABILITY:負債, EQUITY:權益, REVENUE:收入, EXPENSE:費用)
        [ParentAccountId] NVARCHAR(50) NULL, -- 上級科目編號
        [Level] INT NULL DEFAULT 1, -- 科目層級
        [IsLeaf] BIT NULL DEFAULT 1, -- 是否為末級科目
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
        [Memo] NVARCHAR(1000) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Accounts_AccountId] UNIQUE ([AccountId])
    );
END
GO

-- Vouchers - 會計憑證
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vouchers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vouchers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherNo] NVARCHAR(50) NOT NULL, -- 憑證號碼
        [VoucherDate] DATETIME2 NOT NULL, -- 憑證日期
        [VoucherType] NVARCHAR(20) NOT NULL, -- 憑證類型 (GENERAL:一般, ADJUSTMENT:調整, CLOSING:結帳)
        [Description] NVARCHAR(500) NULL, -- 說明
        [TotalDebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方總額
        [TotalCreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方總額
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Vouchers_VoucherNo] UNIQUE ([VoucherNo])
    );
END
GO

-- VoucherDetails - 會計憑證明細
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherDetails] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherNo] NVARCHAR(50) NOT NULL, -- 憑證號碼
        [LineNum] INT NOT NULL, -- 行號
        [AccountId] NVARCHAR(50) NOT NULL, -- 會計科目編號
        [Description] NVARCHAR(500) NULL, -- 說明
        [DebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方金額
        [CreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方金額
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_VoucherDetails_Vouchers] FOREIGN KEY ([VoucherNo]) REFERENCES [dbo].[Vouchers] ([VoucherNo]) ON DELETE CASCADE
    );
END
GO

-- AccountBalances - 會計科目餘額
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountBalances]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AccountBalances] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [AccountId] NVARCHAR(50) NOT NULL, -- 會計科目編號
        [Period] NVARCHAR(10) NOT NULL, -- 會計期間
        [OpeningBalance] DECIMAL(18, 4) NULL DEFAULT 0, -- 期初餘額
        [DebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方金額
        [CreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方金額
        [ClosingBalance] DECIMAL(18, 4) NULL DEFAULT 0, -- 期末餘額
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_AccountBalances] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_AccountBalances_AccountId_Period] UNIQUE ([AccountId], [Period])
    );
END
GO

PRINT '客戶與發票管理類資料表建立完成';
GO


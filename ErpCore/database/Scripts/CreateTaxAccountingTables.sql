-- 會計稅務管理類資料表 (SYST000)
-- 包含：會計帳簿管理（現金流量分類設定）、發票資料維護、交易資料處理、稅務報表等

-- ============================================
-- 1. 會計帳簿管理 - 現金流量大分類資料表 (CashFlowLargeTypes)
-- 對應舊系統 CASH_LTYPE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CashFlowLargeTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CashFlowLargeTypes] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CashLTypeId] NVARCHAR(1) NOT NULL, -- 大分類代號 (CASH_LTYPE_ID)
        [CashLTypeName] NVARCHAR(20) NOT NULL, -- 大分類名稱 (CASH_LTYPE_NAME)
        [AbItem] NVARCHAR(1) NULL, -- 借貸項目 (AB_ITEM, A:借方, B:貸方)
        [Sn] NVARCHAR(10) NULL, -- 排序 (SN)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_CashFlowLargeTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_CashFlowLargeTypes_CashLTypeId] UNIQUE ([CashLTypeId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CashFlowLargeTypes_CashLTypeId] ON [dbo].[CashFlowLargeTypes] ([CashLTypeId]);
    CREATE NONCLUSTERED INDEX [IX_CashFlowLargeTypes_AbItem] ON [dbo].[CashFlowLargeTypes] ([AbItem]);
    CREATE NONCLUSTERED INDEX [IX_CashFlowLargeTypes_Sn] ON [dbo].[CashFlowLargeTypes] ([Sn]);
    
    PRINT 'CashFlowLargeTypes 表建立成功';
END
ELSE
BEGIN
    PRINT 'CashFlowLargeTypes 表已存在';
END
GO

-- ============================================
-- 2. 會計帳簿管理 - 現金流量中分類資料表 (CashFlowMediumTypes)
-- 對應舊系統 CASH_MTYPE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CashFlowMediumTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CashFlowMediumTypes] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CashLTypeId] NVARCHAR(1) NOT NULL, -- 大分類代號 (CASH_LTYPE_ID)
        [CashMTypeId] NVARCHAR(2) NOT NULL, -- 中分類代號 (CASH_MTYPE_ID)
        [CashMTypeName] NVARCHAR(50) NULL, -- 中分類名稱 (CASH_MTYPE_NAME)
        [AbItem] NVARCHAR(1) NULL, -- 借貸項目 (AB_ITEM, A:借方, B:貸方)
        [Sn] NVARCHAR(10) NULL, -- 排序 (SN)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_CashFlowMediumTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_CashFlowMediumTypes] UNIQUE ([CashLTypeId], [CashMTypeId]),
        CONSTRAINT [FK_CashFlowMediumTypes_CashFlowLargeTypes] FOREIGN KEY ([CashLTypeId]) REFERENCES [dbo].[CashFlowLargeTypes] ([CashLTypeId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CashFlowMediumTypes_CashLTypeId] ON [dbo].[CashFlowMediumTypes] ([CashLTypeId]);
    CREATE NONCLUSTERED INDEX [IX_CashFlowMediumTypes_CashMTypeId] ON [dbo].[CashFlowMediumTypes] ([CashMTypeId]);
    
    PRINT 'CashFlowMediumTypes 表建立成功';
END
ELSE
BEGIN
    PRINT 'CashFlowMediumTypes 表已存在';
END
GO

-- ============================================
-- 3. 會計帳簿管理 - 現金流量科目設定資料表 (CashFlowSubjectTypes)
-- 對應舊系統 CASH_STYPE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CashFlowSubjectTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CashFlowSubjectTypes] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CashMTypeId] NVARCHAR(2) NOT NULL, -- 中分類代號 (CASH_MTYPE_ID)
        [CashSTypeId] NVARCHAR(50) NOT NULL, -- 科目代號 (CASH_STYPE_ID, 對應STYPE.STYPE_ID)
        [AbItem] NVARCHAR(1) NULL, -- 借貸項目 (AB_ITEM, A:借方, B:貸方)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_CashFlowSubjectTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_CashFlowSubjectTypes] UNIQUE ([CashMTypeId], [CashSTypeId]),
        CONSTRAINT [FK_CashFlowSubjectTypes_CashFlowMediumTypes] FOREIGN KEY ([CashMTypeId]) REFERENCES [dbo].[CashFlowMediumTypes] ([CashMTypeId]),
        CONSTRAINT [FK_CashFlowSubjectTypes_AccountSubjects] FOREIGN KEY ([CashSTypeId]) REFERENCES [dbo].[AccountSubjects] ([StypeId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CashFlowSubjectTypes_CashMTypeId] ON [dbo].[CashFlowSubjectTypes] ([CashMTypeId]);
    CREATE NONCLUSTERED INDEX [IX_CashFlowSubjectTypes_CashSTypeId] ON [dbo].[CashFlowSubjectTypes] ([CashSTypeId]);
    
    PRINT 'CashFlowSubjectTypes 表建立成功';
END
ELSE
BEGIN
    PRINT 'CashFlowSubjectTypes 表已存在';
END
GO

-- ============================================
-- 4. 會計帳簿管理 - 現金流量小計設定資料表 (CashFlowSubTotals)
-- 對應舊系統 CASH_SUB
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CashFlowSubTotals]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CashFlowSubTotals] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [CashLTypeId] NVARCHAR(1) NOT NULL, -- 大分類代號 (CASH_LTYPE_ID)
        [CashSubId] NVARCHAR(10) NOT NULL, -- 小計代號 (CASH_SUB_ID)
        [CashSubName] NVARCHAR(50) NOT NULL, -- 小計名稱 (CASH_SUB_NAME)
        [CashMTypeIdB] NVARCHAR(2) NULL, -- 中分類代號起 (CASH_MTYPE_ID_B)
        [CashMTypeIdE] NVARCHAR(2) NULL, -- 中分類代號迄 (CASH_MTYPE_ID_E)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_CashFlowSubTotals] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_CashFlowSubTotals] UNIQUE ([CashLTypeId], [CashSubId]),
        CONSTRAINT [FK_CashFlowSubTotals_CashFlowLargeTypes] FOREIGN KEY ([CashLTypeId]) REFERENCES [dbo].[CashFlowLargeTypes] ([CashLTypeId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_CashFlowSubTotals_CashLTypeId] ON [dbo].[CashFlowSubTotals] ([CashLTypeId]);
    CREATE NONCLUSTERED INDEX [IX_CashFlowSubTotals_CashSubId] ON [dbo].[CashFlowSubTotals] ([CashSubId]);
    
    PRINT 'CashFlowSubTotals 表建立成功';
END
ELSE
BEGIN
    PRINT 'CashFlowSubTotals 表已存在';
END
GO

-- ============================================
-- 5. 發票資料維護 - 發票傳票資料表 (InvoiceVouchers)
-- 對應舊系統進項/銷項發票傳票
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceVouchers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InvoiceVouchers] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔T_KEY (外鍵至Vouchers)
        [InvoiceType] NVARCHAR(10) NOT NULL, -- 發票類型 (INVOICE_TYPE, 1:進項稅額, 2:進項折讓, 3:進項退回, 4:銷項稅額, 5:銷項折讓, 6:銷項退回)
        [InvoiceNo] NVARCHAR(50) NULL, -- 發票號碼 (INVOICE_NO)
        [InvoiceDate] DATETIME2 NULL, -- 發票日期 (INVOICE_DATE)
        [InvoiceFormat] NVARCHAR(10) NULL, -- 發票格式 (INVOICE_FORMAT)
        [InvoiceAmount] DECIMAL(18, 2) NULL, -- 發票金額 (INVOICE_AMOUNT)
        [TaxAmount] DECIMAL(18, 2) NULL, -- 稅額 (TAX_AMOUNT)
        [DeductCode] NVARCHAR(50) NULL, -- 扣抵代號 (DEDUCT_CODE)
        [CategoryType] NVARCHAR(50) NULL, -- 類別區分 (CATEGORY_TYPE)
        [VoucherNo] NVARCHAR(50) NULL, -- 憑證單號 (VOUCHER_NO)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_InvoiceVouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [FK_InvoiceVouchers_Vouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[Vouchers] ([TKey]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_InvoiceVouchers_VoucherTKey] ON [dbo].[InvoiceVouchers] ([VoucherTKey]);
    CREATE NONCLUSTERED INDEX [IX_InvoiceVouchers_InvoiceNo] ON [dbo].[InvoiceVouchers] ([InvoiceNo]);
    CREATE NONCLUSTERED INDEX [IX_InvoiceVouchers_InvoiceType] ON [dbo].[InvoiceVouchers] ([InvoiceType]);
    
    PRINT 'InvoiceVouchers 表建立成功';
END
ELSE
BEGIN
    PRINT 'InvoiceVouchers 表已存在';
END
GO

-- ============================================
-- 6. 發票資料維護 - 費用/收入分攤比率設定 (AllocationRatios)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AllocationRatios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AllocationRatios] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DisYm] NVARCHAR(6) NOT NULL, -- 分攤年月 (DIS_YM, YYYYMM)
        [StypeId] NVARCHAR(50) NOT NULL, -- 會計科目 (STYPE_ID, 外鍵至AccountSubjects)
        [OrgId] NVARCHAR(50) NOT NULL, -- 組織代號 (ORG_ID)
        [Ratio] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 分攤比率 (RATIO, 0-1)
        [VoucherTKey] BIGINT NULL, -- 傳票主檔T_KEY (VOUCHER_T_KEY, 外鍵至Vouchers)
        [VoucherDTKey] BIGINT NULL, -- 傳票明細T_KEY (VOUCHER_D_T_KEY, 外鍵至VoucherDetails)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_AllocationRatios] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [FK_AllocationRatios_Vouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[Vouchers] ([TKey]),
        CONSTRAINT [FK_AllocationRatios_VoucherDetails] FOREIGN KEY ([VoucherDTKey]) REFERENCES [dbo].[VoucherDetails] ([TKey])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_AllocationRatios_DisYm] ON [dbo].[AllocationRatios] ([DisYm]);
    CREATE NONCLUSTERED INDEX [IX_AllocationRatios_StypeId] ON [dbo].[AllocationRatios] ([StypeId]);
    CREATE NONCLUSTERED INDEX [IX_AllocationRatios_OrgId] ON [dbo].[AllocationRatios] ([OrgId]);
    
    PRINT 'AllocationRatios 表建立成功';
END
ELSE
BEGIN
    PRINT 'AllocationRatios 表已存在';
END
GO

-- ============================================
-- 7. 暫存傳票審核作業 - 暫存傳票主檔 (TmpVoucherM)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TmpVoucherM]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TmpVoucherM] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherId] NVARCHAR(50) NULL, -- 傳票編號 (VOUCHER_ID)
        [VoucherDate] DATETIME2 NULL, -- 傳票日期 (VOUCHER_DATE)
        [TypeId] NVARCHAR(50) NULL, -- 傳票類型 (TYPE_ID, 如SYSTA10, SYSTA30, SYSTA40等)
        [SysId] NVARCHAR(50) NULL, -- 系統代號 (SYS_ID, 如SYSA000, SYSV000, SYSH000等)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 傳票狀態 (STATUS, 1:未審核, 2:已審核, 3:已拋轉)
        [UpFlag] NVARCHAR(1) NULL DEFAULT '0', -- 拋轉標記 (UP_FLAG, 0:未拋轉, 1:已拋轉)
        [Notes] NVARCHAR(500) NULL, -- 傳票備註 (NOTES)
        [VendorId] NVARCHAR(50) NULL, -- 廠商代號 (VENDOR_ID)
        [StoreId] NVARCHAR(50) NULL, -- 專櫃代號 (STORE_ID)
        [SiteId] NVARCHAR(50) NULL, -- 公司別/分店代號 (SITE_ID)
        [SlipType] NVARCHAR(50) NULL, -- 單據別 (SLIP_TYPE)
        [SlipNo] NVARCHAR(50) NULL, -- 單據編號 (SLIP_NO)
        [SendFlag] NVARCHAR(1) NULL DEFAULT '0', -- 傳送標記 (SEND_FLAG)
        [ProgId] NVARCHAR(50) NULL, -- 程式代號 (PROG_ID)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_TmpVoucherM] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_TypeId] ON [dbo].[TmpVoucherM] ([TypeId]);
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_SysId] ON [dbo].[TmpVoucherM] ([SysId]);
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_Status] ON [dbo].[TmpVoucherM] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_UpFlag] ON [dbo].[TmpVoucherM] ([UpFlag]);
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_VoucherDate] ON [dbo].[TmpVoucherM] ([VoucherDate]);
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_VoucherId] ON [dbo].[TmpVoucherM] ([VoucherId]);
    
    PRINT 'TmpVoucherM 表建立成功';
END
ELSE
BEGIN
    PRINT 'TmpVoucherM 表已存在';
END
GO

-- ============================================
-- 8. 暫存傳票審核作業 - 暫存傳票明細檔 (TmpVoucherD)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TmpVoucherD]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TmpVoucherD] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔TKey (VOUCHER_T_KEY)
        [Sn] NVARCHAR(10) NOT NULL, -- 序號 (SN)
        [Dc] NVARCHAR(1) NULL, -- 借貸方 (DC, 0:借方, 1:貸方)
        [SubN] NVARCHAR(50) NULL, -- 科目代號 (SUB_N)
        [OrgId] NVARCHAR(50) NULL, -- 部門代號 (ORG_ID)
        [ActId] NVARCHAR(50) NULL, -- 專案代號 (ACT_ID)
        [Notes] NVARCHAR(500) NULL, -- 摘要 (NOTES)
        [Val0] DECIMAL(18, 2) NULL DEFAULT 0, -- 借方金額 (VAL0)
        [Val1] DECIMAL(18, 2) NULL DEFAULT 0, -- 貸方金額 (VAL1)
        [SupN] NVARCHAR(50) NULL, -- 立沖對象 (SUP_N)
        [InN] NVARCHAR(50) NULL, -- 立沖憑證 (IN_N)
        [VendorId] NVARCHAR(50) NULL, -- 廠商代號 (VENDOR_ID)
        [AbatId] NVARCHAR(50) NULL, -- 立沖代號 (ABAT_ID)
        [ObjectId] NVARCHAR(50) NULL, -- 關係人代號 (OBJECT_ID)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        CONSTRAINT [PK_TmpVoucherD] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [FK_TmpVoucherD_TmpVoucherM] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[TmpVoucherM] ([TKey]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherD_VoucherTKey] ON [dbo].[TmpVoucherD] ([VoucherTKey]);
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherD_SubN] ON [dbo].[TmpVoucherD] ([SubN]);
    CREATE NONCLUSTERED INDEX [IX_TmpVoucherD_OrgId] ON [dbo].[TmpVoucherD] ([OrgId]);
    
    PRINT 'TmpVoucherD 表建立成功';
END
ELSE
BEGIN
    PRINT 'TmpVoucherD 表已存在';
END
GO

-- ============================================
-- 9. 傳票轉入作業 - 傳票轉入記錄檔 (VoucherImportLog)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherImportLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherImportLog] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ImportType] NVARCHAR(50) NOT NULL, -- 轉入類型 (IMPORT_TYPE, AHM:住金, HTV:日立)
        [FileName] NVARCHAR(500) NULL, -- 檔案名稱 (FILE_NAME)
        [FilePath] NVARCHAR(1000) NULL, -- 檔案路徑 (FILE_PATH)
        [ImportDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 轉入日期 (IMPORT_DATE)
        [TotalCount] INT NULL DEFAULT 0, -- 總筆數 (TOTAL_COUNT)
        [SuccessCount] INT NULL DEFAULT 0, -- 成功筆數 (SUCCESS_COUNT)
        [FailCount] INT NULL DEFAULT 0, -- 失敗筆數 (FAIL_COUNT)
        [SkipCount] INT NULL DEFAULT 0, -- 跳過筆數 (SKIP_COUNT, 已存在)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:處理中, S:成功, F:失敗)
        [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        CONSTRAINT [PK_VoucherImportLog] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_VoucherImportLog_ImportType] ON [dbo].[VoucherImportLog] ([ImportType]);
    CREATE NONCLUSTERED INDEX [IX_VoucherImportLog_ImportDate] ON [dbo].[VoucherImportLog] ([ImportDate]);
    CREATE NONCLUSTERED INDEX [IX_VoucherImportLog_Status] ON [dbo].[VoucherImportLog] ([Status]);
    
    PRINT 'VoucherImportLog 表建立成功';
END
ELSE
BEGIN
    PRINT 'VoucherImportLog 表已存在';
END
GO

-- ============================================
-- 10. 傳票轉入作業 - 傳票轉入明細記錄檔 (VoucherImportDetail)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VoucherImportDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[VoucherImportDetail] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ImportLogTKey] BIGINT NOT NULL, -- 轉入記錄TKey (IMPORT_LOG_T_KEY)
        [RowNumber] INT NULL, -- 行號 (ROW_NUMBER)
        [VoucherTKey] BIGINT NULL, -- 傳票主檔TKey (VOUCHER_T_KEY, 轉入成功時填入)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:處理中, S:成功, F:失敗, K:跳過)
        [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
        [SourceData] NVARCHAR(MAX) NULL, -- 原始資料 (SOURCE_DATA, JSON格式)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        CONSTRAINT [PK_VoucherImportDetail] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [FK_VoucherImportDetail_VoucherImportLog] FOREIGN KEY ([ImportLogTKey]) REFERENCES [dbo].[VoucherImportLog] ([TKey]) ON DELETE CASCADE,
        CONSTRAINT [FK_VoucherImportDetail_TmpVoucherM] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[TmpVoucherM] ([TKey])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_VoucherImportDetail_ImportLogTKey] ON [dbo].[VoucherImportDetail] ([ImportLogTKey]);
    CREATE NONCLUSTERED INDEX [IX_VoucherImportDetail_Status] ON [dbo].[VoucherImportDetail] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_VoucherImportDetail_VoucherTKey] ON [dbo].[VoucherImportDetail] ([VoucherTKey]);
    
    PRINT 'VoucherImportDetail 表建立成功';
END
ELSE
BEGIN
    PRINT 'VoucherImportDetail 表已存在';
END
GO

-- ============================================
-- 11. 稅務報表列印 - SAP銀行往來總表 (SapBankTotal)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SapBankTotal]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SapBankTotal] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [SapDate] NVARCHAR(8) NOT NULL, -- SAP日期 (SAP_DATE, YYYYMMDD)
        [SapStypeId] NVARCHAR(50) NULL, -- SAP會計科目 (SAP_STYPE_ID)
        [CompId] NVARCHAR(10) NOT NULL, -- 公司代號 (COMP_ID)
        [BankAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 銀行金額 (BANK_AMT)
        [BankBalance] DECIMAL(18, 2) NULL DEFAULT 0, -- 銀行餘額 (BANK_BALANCE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        CONSTRAINT [PK_SapBankTotal] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_SapBankTotal_SapDate] ON [dbo].[SapBankTotal] ([SapDate]);
    CREATE NONCLUSTERED INDEX [IX_SapBankTotal_CompId] ON [dbo].[SapBankTotal] ([CompId]);
    CREATE NONCLUSTERED INDEX [IX_SapBankTotal_SapStypeId] ON [dbo].[SapBankTotal] ([SapStypeId]);
    CREATE NONCLUSTERED INDEX [IX_SapBankTotal_SapDate_CompId] ON [dbo].[SapBankTotal] ([SapDate], [CompId]);
    
    PRINT 'SapBankTotal 表建立成功';
END
ELSE
BEGIN
    PRINT 'SapBankTotal 表已存在';
END
GO

-- ============================================
-- 12. 稅務報表列印 - 稅務報表列印記錄 (TaxReportPrints)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaxReportPrints]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TaxReportPrints] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ReportType] NVARCHAR(20) NOT NULL, -- 報表類型 (REPORT_TYPE, SYST510/SYST520/SYST530)
        [ReportDate] DATETIME2 NOT NULL, -- 報表日期 (REPORT_DATE)
        [DateFrom] DATETIME2 NULL, -- 查詢起始日期 (DATE_FROM)
        [DateTo] DATETIME2 NULL, -- 查詢結束日期 (DATE_TO)
        [CompId] NVARCHAR(10) NULL, -- 公司代號 (COMP_ID)
        [FileName] NVARCHAR(200) NULL, -- 檔案名稱 (FILE_NAME)
        [FileFormat] NVARCHAR(10) NULL, -- 檔案格式 (FILE_FORMAT, CSV/PDF/EXCEL)
        [PrintStatus] NVARCHAR(10) NOT NULL DEFAULT '1', -- 列印狀態 (PRINT_STATUS, 1:成功, 2:失敗)
        [PrintCount] INT NULL DEFAULT 0, -- 列印次數 (PRINT_COUNT)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        CONSTRAINT [PK_TaxReportPrints] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TaxReportPrints_ReportType] ON [dbo].[TaxReportPrints] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_TaxReportPrints_ReportDate] ON [dbo].[TaxReportPrints] ([ReportDate]);
    CREATE NONCLUSTERED INDEX [IX_TaxReportPrints_DateFrom_DateTo] ON [dbo].[TaxReportPrints] ([DateFrom], [DateTo]);
    
    PRINT 'TaxReportPrints 表建立成功';
END
ELSE
BEGIN
    PRINT 'TaxReportPrints 表已存在';
END
GO

-- ============================================
-- 擴展現有 Vouchers 表，添加發票和交易相關欄位
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vouchers]') AND name = 'VoucherKind')
BEGIN
    ALTER TABLE [dbo].[Vouchers]
    ADD [VoucherKind] NVARCHAR(10) NULL, -- 傳票種類 (VOUCHER_KIND, 1:一般, 2:建立, 3:分攤, 4:手切)
          [VoucherStatus] NVARCHAR(10) NULL DEFAULT '1', -- 傳票狀態 (VOUCHER_STATUS, 1:正常, 2:作廢, 3:已結帳)
          [InvYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否補發票 (INV_YN, Y/N)
          [SiteId] NVARCHAR(50) NULL, -- 店代號 (SITE_ID)
          [VendorId] NVARCHAR(50) NULL, -- 廠客代號 (VENDOR_ID)
          [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME)
          [ConfirmDate] DATETIME2 NULL, -- 確認日期 (CONFIRM_DATE)
          [ConfirmBy] NVARCHAR(50) NULL, -- 確認者 (CONFIRM_BY)
          [PostingYearMonth] NVARCHAR(6) NULL, -- 過帳年月 (POSTING_YEAR_MONTH, YYYYMM)
          [PostingDate] DATETIME2 NULL, -- 過帳日期 (POSTING_DATE)
          [PostingBy] NVARCHAR(50) NULL, -- 過帳者 (POSTING_BY)
          [ReversePostingDate] DATETIME2 NULL, -- 反過帳日期 (REVERSE_POSTING_DATE)
          [ReversePostingBy] NVARCHAR(50) NULL, -- 反過帳者 (REVERSE_POSTING_BY)
          [YearEndProcessYear] NVARCHAR(4) NULL; -- 年結處理年度 (YEAR_END_PROCESS_YEAR)
    
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherKind] ON [dbo].[Vouchers] ([VoucherKind]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherStatus] ON [dbo].[Vouchers] ([VoucherStatus]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_PostingYearMonth] ON [dbo].[Vouchers] ([PostingYearMonth]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_ConfirmDate] ON [dbo].[Vouchers] ([ConfirmDate]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_PostingDate] ON [dbo].[Vouchers] ([PostingDate]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_SiteId] ON [dbo].[Vouchers] ([SiteId]);
    CREATE NONCLUSTERED INDEX [IX_Vouchers_VendorId] ON [dbo].[Vouchers] ([VendorId]);
    
    PRINT 'Vouchers 表擴展成功';
END
ELSE
BEGIN
    PRINT 'Vouchers 表已擴展';
END
GO

-- ============================================
-- 擴展現有 VoucherDetails 表，添加發票相關欄位
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[VoucherDetails]') AND name = 'OrgId')
BEGIN
    ALTER TABLE [dbo].[VoucherDetails]
    ADD [VoucherTKey] BIGINT NULL, -- 傳票主檔T_KEY (VOUCHER_T_KEY, 外鍵至Vouchers)
          [OrgId] NVARCHAR(50) NULL, -- 組織代號 (ORG_ID)
          [ActId] NVARCHAR(50) NULL, -- 對象代號 (ACT_ID)
          [AbatId] NVARCHAR(50) NULL, -- 立沖代號 (ABAT_ID)
          [VendorId] NVARCHAR(50) NULL, -- 廠商/員工代號 (VENDOR_ID)
          [AcctKey] NVARCHAR(50) NULL, -- 帳戶KEY值 (ACCT_KEY)
          [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
          [CustomField1] NVARCHAR(200) NULL, -- 自訂欄位1 (CUSTOM_FIELD1)
          [DAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 借方金額 (D_AMT)
          [CAmt] DECIMAL(18, 2) NULL DEFAULT 0; -- 貸方金額 (C_AMT)
    
    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherTKey] ON [dbo].[VoucherDetails] ([VoucherTKey]);
    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_OrgId] ON [dbo].[VoucherDetails] ([OrgId]);
    CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VendorId] ON [dbo].[VoucherDetails] ([VendorId]);
    
    PRINT 'VoucherDetails 表擴展成功';
END
ELSE
BEGIN
    PRINT 'VoucherDetails 表已擴展';
END
GO


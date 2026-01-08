-- 廠/客基本資料維護作業資料表 (SYSB206)
-- 對應舊系統 VENDOR 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vendors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vendors] (
        [VendorId] NVARCHAR(12) NOT NULL PRIMARY KEY, -- 廠商編號 (VENDOR_ID)
        [GuiId] NVARCHAR(20) NOT NULL, -- 統一編號/身份證字號/自編編號 (GUI_ID)
        [GuiType] NVARCHAR(1) NOT NULL, -- 識別類型 (GUI_TYPE) 1:統一編號, 2:身份證字號, 3:自編編號
        [VendorName] NVARCHAR(200) NOT NULL, -- 廠商名稱(中文) (VENDOR_NAME)
        [VendorNameE] NVARCHAR(200) NULL, -- 廠商名稱(英文) (VENDOR_NAME_E)
        [VendorNameS] NVARCHAR(50) NULL, -- 廠商簡稱 (VENDOR_NAME_S)
        [Mcode] NVARCHAR(20) NULL, -- 郵電費負擔 (MCODE)
        [VendorRegAddr] NVARCHAR(500) NULL, -- 公司登記地址 (VENDOR_REG_ADDR)
        [TaxAddr] NVARCHAR(500) NULL, -- 發票地址 (TAX_ADDR)
        [VendorRegTel] NVARCHAR(50) NULL, -- 公司登記電話 (VENDOR_REG_TEL)
        [VendorFax] NVARCHAR(50) NULL, -- 公司傳真 (VENDOR_FAX)
        [VendorEmail] NVARCHAR(100) NULL, -- 公司電子郵件 (VENDOR_EMAIL)
        [InvEmail] NVARCHAR(100) NULL, -- 發票電子郵件 (INV_EMAIL)
        [ChargeStaff] NVARCHAR(50) NULL, -- 聯絡人 (CHARGE_STAFF)
        [ChargeTitle] NVARCHAR(50) NULL, -- 聯絡人職稱 (CHARGE_TITLE)
        [ChargePid] NVARCHAR(20) NULL, -- 聯絡人身份證字號 (CHARGE_PID)
        [ChargeTel] NVARCHAR(50) NULL, -- 聯絡人電話 (CHARGE_TEL)
        [ChargeAddr] NVARCHAR(500) NULL, -- 聯絡人聯絡地址 (CHARGE_ADDR)
        [ChargeEmail] NVARCHAR(100) NULL, -- 聯絡人電子郵件 (CHARGE_EMAIL)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [SysId] NVARCHAR(10) NOT NULL DEFAULT '1', -- 系統別 (SYS_ID)
        [PayType] NVARCHAR(20) NULL, -- 付款方式 (PAY_TYPE)
        [SuplBankId] NVARCHAR(50) NULL, -- 匯款銀行代碼 (SUPL_BANK_ID)
        [SuplBankAcct] NVARCHAR(50) NULL, -- 匯款銀行帳號 (SUPL_BANK_ACCT)
        [SuplAcctName] NVARCHAR(100) NULL, -- 帳戶名稱 (SUPL_ACCT_NAME)
        [TicketBe] NVARCHAR(20) NULL, -- 票據別 (TICKET_BE)
        [CheckTitle] NVARCHAR(100) NULL, -- 支票抬頭 (CHECK_TITLE)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED ([VendorId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Vendors_GuiId] ON [dbo].[Vendors] ([GuiId]);
    CREATE NONCLUSTERED INDEX [IX_Vendors_VendorName] ON [dbo].[Vendors] ([VendorName]);
    CREATE NONCLUSTERED INDEX [IX_Vendors_Status] ON [dbo].[Vendors] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Vendors_SysId] ON [dbo].[Vendors] ([SysId]);
    CREATE NONCLUSTERED INDEX [IX_Vendors_OrgId] ON [dbo].[Vendors] ([OrgId]);
    
    PRINT 'Vendors 表建立成功';
END
ELSE
BEGIN
    PRINT 'Vendors 表已存在';
END
GO


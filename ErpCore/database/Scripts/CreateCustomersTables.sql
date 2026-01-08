-- 客戶基本資料維護作業資料表 (CUS5110)
-- 對應舊系統 VCH_CUSTOMER 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Customers] (
        [CustomerId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 客戶編號 (CUST_ID)
        [GuiId] NVARCHAR(20) NULL, -- 統一編號/身份證字號 (GUI_ID)
        [GuiType] NVARCHAR(1) NULL, -- 識別類型 (GUI_TYPE) 1:統一編號, 2:身份證字號, 3:自編編號
        [CustomerName] NVARCHAR(200) NOT NULL, -- 客戶名稱 (CUST_NAME)
        [CustomerNameE] NVARCHAR(200) NULL, -- 客戶名稱(英文) (CUST_NAME_E)
        [ShortName] NVARCHAR(50) NULL, -- 簡稱/抬頭 (SHORT_NAME)
        [ContactStaff] NVARCHAR(50) NULL, -- 聯絡人 (CONTACT_STAFF)
        [HomeTel] NVARCHAR(50) NULL, -- 住家電話 (HOME_TEL)
        [CompTel] NVARCHAR(50) NULL, -- 公司電話 (COMP_TEL)
        [Fax] NVARCHAR(50) NULL, -- 傳真 (FAX)
        [Cell] NVARCHAR(50) NULL, -- 手機 (CELL)
        [Email] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
        [Sex] NVARCHAR(1) NULL, -- 性別 (SEX) M:男, F:女
        [Title] NVARCHAR(50) NULL, -- 稱謂/職稱 (TITLE)
        [City] NVARCHAR(50) NULL, -- 城市 (CITY)
        [Canton] NVARCHAR(50) NULL, -- 區域 (CANTON)
        [Addr] NVARCHAR(500) NULL, -- 地址 (ADDR)
        [TaxAddr] NVARCHAR(500) NULL, -- 發票地址 (TAX_ADDR)
        [DelyAddr] NVARCHAR(500) NULL, -- 送貨地址 (DELY_ADDR)
        [PostId] NVARCHAR(10) NULL, -- 郵遞區號 (POST_ID)
        [DiscountYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否享有折扣 (DISCOUNT_YN) Y:是, N:否
        [DiscountNo] NVARCHAR(20) NULL, -- 折扣類別代碼 (DISCOUNT_NO)
        [SalesId] NVARCHAR(50) NULL, -- 業務員 (SALES_ID)
        [TransDate] DATETIME2 NULL, -- 最近交易日期 (TRANS_DATE)
        [TransNo] NVARCHAR(50) NULL, -- 最近交易序號 (TRANS_NO)
        [AccAmt] DECIMAL(18,2) NULL DEFAULT 0, -- 消費累積金額 (ACC_AMT)
        [MonthlyYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否為月結客戶 (MONTHLY_YN) Y:是, N:否
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([CustomerId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Customers_GuiId] ON [dbo].[Customers] ([GuiId]);
    CREATE NONCLUSTERED INDEX [IX_Customers_CustomerName] ON [dbo].[Customers] ([CustomerName]);
    CREATE NONCLUSTERED INDEX [IX_Customers_Status] ON [dbo].[Customers] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Customers_SalesId] ON [dbo].[Customers] ([SalesId]);
    CREATE NONCLUSTERED INDEX [IX_Customers_TransDate] ON [dbo].[Customers] ([TransDate]);
    
    PRINT 'Customers 表建立成功';
END
ELSE
BEGIN
    PRINT 'Customers 表已存在';
END
GO

-- 客戶聯絡人資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerContacts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerContacts] (
        [ContactId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [CustomerId] NVARCHAR(50) NOT NULL,
        [ContactName] NVARCHAR(50) NOT NULL, -- 聯絡人姓名
        [ContactTitle] NVARCHAR(50) NULL, -- 職稱
        [ContactTel] NVARCHAR(50) NULL, -- 電話
        [ContactCell] NVARCHAR(50) NULL, -- 手機
        [ContactEmail] NVARCHAR(100) NULL, -- 電子郵件
        [IsPrimary] BIT NULL DEFAULT 0, -- 是否為主要聯絡人
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_CustomerContacts_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([CustomerId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_CustomerContacts_CustomerId] ON [dbo].[CustomerContacts] ([CustomerId]);
    
    PRINT 'CustomerContacts 表建立成功';
END
ELSE
BEGIN
    PRINT 'CustomerContacts 表已存在';
END
GO


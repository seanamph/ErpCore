-- 銀行基本資料維護資料表 (SYSBC20)
-- 對應舊系統 BANK 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Banks]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Banks] (
        [BankId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 銀行代號 (BANK_ID)
        [BankName] NVARCHAR(100) NOT NULL, -- 銀行名稱 (BANK_NAME)
        [AcctLen] INT NULL, -- 帳號最小長度 (ACCT_LEN)
        [AcctLenMax] INT NULL, -- 帳號最大長度 (ACCT_LEN_MAX)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [BankKind] NVARCHAR(10) NULL, -- 銀行種類 (BANK_KIND) 1:銀行, 2:郵局, 3:信用合作社
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Banks] PRIMARY KEY CLUSTERED ([BankId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Banks_BankName] ON [dbo].[Banks] ([BankName]);
    CREATE NONCLUSTERED INDEX [IX_Banks_Status] ON [dbo].[Banks] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Banks_BankKind] ON [dbo].[Banks] ([BankKind]);
    CREATE NONCLUSTERED INDEX [IX_Banks_SeqNo] ON [dbo].[Banks] ([SeqNo]);
    
    PRINT 'Banks 表建立成功';
END
ELSE
BEGIN
    PRINT 'Banks 表已存在';
END
GO


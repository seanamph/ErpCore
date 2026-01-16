-- =============================================
-- SYSBC20 - 銀行基本資料維護作業
-- 建立日期: 2025-01-27
-- 說明: 建立銀行基本資料表
-- =============================================

-- 檢查資料表是否存在，如果存在則刪除
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Banks]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[Banks]
    PRINT '已刪除現有的 Banks 資料表'
END
GO

-- 建立 Banks 資料表
CREATE TABLE [dbo].[Banks] (
    [BankId] NVARCHAR(50) NOT NULL, -- 銀行代號 (主鍵)
    [BankName] NVARCHAR(100) NOT NULL, -- 銀行名稱
    [AcctLen] INT NULL, -- 帳號最小長度
    [AcctLenMax] INT NULL, -- 帳號最大長度
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [BankKind] NVARCHAR(10) NULL, -- 銀行種類 (1:銀行, 2:郵局, 3:信用合作社)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    [CreatedPriority] INT NULL, -- 建立者等級
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組
    CONSTRAINT [PK_Banks] PRIMARY KEY CLUSTERED ([BankId] ASC)
)
GO

-- 建立索引
CREATE NONCLUSTERED INDEX [IX_Banks_BankName] ON [dbo].[Banks] ([BankName])
GO

CREATE NONCLUSTERED INDEX [IX_Banks_Status] ON [dbo].[Banks] ([Status])
GO

CREATE NONCLUSTERED INDEX [IX_Banks_BankKind] ON [dbo].[Banks] ([BankKind])
GO

CREATE NONCLUSTERED INDEX [IX_Banks_SeqNo] ON [dbo].[Banks] ([SeqNo])
GO

CREATE NONCLUSTERED INDEX [IX_Banks_CreatedAt] ON [dbo].[Banks] ([CreatedAt])
GO

PRINT 'Banks 資料表建立完成'
GO

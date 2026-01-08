-- SYSBC40 - 參數資料設定維護作業
-- 建立 Parameters 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Parameters]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Parameters] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [Title] NVARCHAR(100) NOT NULL, -- 參數標題 (TITLE)
        [Tag] NVARCHAR(100) NOT NULL, -- 參數標籤/代碼 (TAG)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Content] NVARCHAR(500) NULL, -- 參數內容 (CONTENT)
        [Content2] NVARCHAR(500) NULL, -- 多語言參數內容 (CONTENT_2)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [ReadOnly] NVARCHAR(10) NULL DEFAULT '0', -- 只讀標誌 (READONLY) 1:只讀, 0:可編輯
        [SystemId] NVARCHAR(50) NULL, -- 系統ID (SYS_ID)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Parameters_Title_Tag] UNIQUE ([Title], [Tag])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Parameters_Title] ON [dbo].[Parameters] ([Title]);
    CREATE NONCLUSTERED INDEX [IX_Parameters_Tag] ON [dbo].[Parameters] ([Tag]);
    CREATE NONCLUSTERED INDEX [IX_Parameters_Title_Tag] ON [dbo].[Parameters] ([Title], [Tag]);
    CREATE NONCLUSTERED INDEX [IX_Parameters_Status] ON [dbo].[Parameters] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Parameters_SeqNo] ON [dbo].[Parameters] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_Parameters_SystemId] ON [dbo].[Parameters] ([SystemId]);

    PRINT 'Parameters 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Parameters 資料表已存在';
END
GO


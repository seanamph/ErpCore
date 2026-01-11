-- SYS0440 - 系統功能按鈕資料維護作業
-- 建立 Buttons 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buttons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Buttons] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [ProgramId] NVARCHAR(50) NOT NULL, -- 作業代碼 (PROG_ID)
        [ButtonId] NVARCHAR(50) NOT NULL, -- 按鈕代碼 (BUTTON_ID)
        [ButtonName] NVARCHAR(100) NOT NULL, -- 按鈕名稱 (BUTTON_NAME)
        [PageId] NVARCHAR(50) NULL, -- 頁面代碼 (PAGE_ID)
        [ButtonMsg] NVARCHAR(500) NULL, -- 按鈕訊息 (BUTTON_MSG)
        [ButtonAttr] NVARCHAR(50) NULL, -- 按鈕屬性 (BUTTON_ATTR)
        [ButtonUrl] NVARCHAR(500) NULL, -- 網頁鏈結位址 (URL)
        [MsgType] NVARCHAR(20) NULL, -- 訊息型態 (MSG_TYPE)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Buttons] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 外鍵約束（如果 Programs 表存在）
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Programs]') AND type in (N'U'))
    BEGIN
        ALTER TABLE [dbo].[Buttons]
        ADD CONSTRAINT [FK_Buttons_Programs] FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[Programs] ([ProgramId]);
    END

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Buttons_ProgramId] ON [dbo].[Buttons] ([ProgramId]);
    CREATE NONCLUSTERED INDEX [IX_Buttons_ButtonId] ON [dbo].[Buttons] ([ButtonId]);
    CREATE NONCLUSTERED INDEX [IX_Buttons_PageId] ON [dbo].[Buttons] ([PageId]);
    CREATE NONCLUSTERED INDEX [IX_Buttons_ProgramId_PageId_ButtonId] ON [dbo].[Buttons] ([ProgramId], [PageId], [ButtonId]);

    PRINT 'Buttons 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Buttons 資料表已存在';
END
GO

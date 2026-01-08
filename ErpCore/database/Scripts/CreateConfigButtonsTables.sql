-- CFG0440 - 系統功能按鈕資料維護作業
-- 建立 ConfigButtons 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfigButtons]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfigButtons] (
        [ButtonId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 按鈕代碼 (BUTTON_ID)
        [ProgramId] NVARCHAR(50) NOT NULL, -- 作業代碼 (PROG_ID)
        [ButtonName] NVARCHAR(100) NOT NULL, -- 按鈕名稱 (BUTTON_NAME)
        [ButtonType] NVARCHAR(20) NULL, -- 按鈕型態 (BUTTON_TYPE)
        [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        CONSTRAINT [PK_ConfigButtons] PRIMARY KEY CLUSTERED ([ButtonId] ASC),
        CONSTRAINT [FK_ConfigButtons_ConfigPrograms] FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[ConfigPrograms] ([ProgramId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConfigButtons_ProgramId] ON [dbo].[ConfigButtons] ([ProgramId]);
    CREATE NONCLUSTERED INDEX [IX_ConfigButtons_ButtonType] ON [dbo].[ConfigButtons] ([ButtonType]);
    CREATE NONCLUSTERED INDEX [IX_ConfigButtons_SeqNo] ON [dbo].[ConfigButtons] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_ConfigButtons_Status] ON [dbo].[ConfigButtons] ([Status]);

    PRINT 'ConfigButtons 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ConfigButtons 資料表已存在';
END
GO


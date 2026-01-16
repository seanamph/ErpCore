-- CFG0430 - 系統作業資料維護作業
-- 建立 ConfigPrograms 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfigPrograms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfigPrograms] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [ProgramId] NVARCHAR(50) NOT NULL, -- 作業代碼 (PROGRAM_ID)
        [ProgramName] NVARCHAR(100) NOT NULL, -- 作業名稱 (PROGRAM_NAME)
        [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
        [SystemId] NVARCHAR(50) NOT NULL, -- 主系統代碼 (SYS_ID)
        [SubSystemId] NVARCHAR(50) NULL, -- 子系統代碼 (SUB_SYS_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_ConfigPrograms] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_ConfigPrograms_ProgramId] UNIQUE ([ProgramId]),
        CONSTRAINT [FK_ConfigPrograms_ConfigSystems] FOREIGN KEY ([SystemId]) REFERENCES [dbo].[ConfigSystems] ([SystemId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ConfigPrograms_ConfigSubSystems] FOREIGN KEY ([SubSystemId]) REFERENCES [dbo].[ConfigSubSystems] ([SubSystemId]) ON DELETE SET NULL
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConfigPrograms_ProgramId] ON [dbo].[ConfigPrograms] ([ProgramId]);
    CREATE NONCLUSTERED INDEX [IX_ConfigPrograms_SystemId] ON [dbo].[ConfigPrograms] ([SystemId]);
    CREATE NONCLUSTERED INDEX [IX_ConfigPrograms_SubSystemId] ON [dbo].[ConfigPrograms] ([SubSystemId]);
    CREATE NONCLUSTERED INDEX [IX_ConfigPrograms_SeqNo] ON [dbo].[ConfigPrograms] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_ConfigPrograms_Status] ON [dbo].[ConfigPrograms] ([Status]);

    PRINT 'ConfigPrograms 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ConfigPrograms 資料表已存在';
END
GO

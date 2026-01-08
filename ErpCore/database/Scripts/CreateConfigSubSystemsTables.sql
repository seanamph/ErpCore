-- CFG0420 - 子系統項目資料維護作業
-- 建立 ConfigSubSystems 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfigSubSystems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfigSubSystems] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [SubSystemId] NVARCHAR(50) NOT NULL, -- 子系統項目代碼 (SUB_SYS_ID)
        [SubSystemName] NVARCHAR(100) NOT NULL, -- 子系統項目名稱 (SUB_SYS_NAME)
        [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
        [SystemId] NVARCHAR(50) NOT NULL, -- 主系統代碼 (SYS_ID)
        [ParentSubSystemId] NVARCHAR(50) NULL, -- 上層子系統代碼 (P_SUB_SYS_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_ConfigSubSystems] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_ConfigSubSystems_SubSystemId] UNIQUE ([SubSystemId]),
        CONSTRAINT [FK_ConfigSubSystems_ConfigSystems] FOREIGN KEY ([SystemId]) REFERENCES [dbo].[ConfigSystems] ([SystemId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ConfigSubSystems_ParentSubSystem] FOREIGN KEY ([ParentSubSystemId]) REFERENCES [dbo].[ConfigSubSystems] ([SubSystemId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConfigSubSystems_SubSystemId] ON [dbo].[ConfigSubSystems] ([SubSystemId]);
    CREATE NONCLUSTERED INDEX [IX_ConfigSubSystems_SystemId] ON [dbo].[ConfigSubSystems] ([SystemId]);
    CREATE NONCLUSTERED INDEX [IX_ConfigSubSystems_ParentSubSystemId] ON [dbo].[ConfigSubSystems] ([ParentSubSystemId]);
    CREATE NONCLUSTERED INDEX [IX_ConfigSubSystems_SeqNo] ON [dbo].[ConfigSubSystems] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_ConfigSubSystems_Status] ON [dbo].[ConfigSubSystems] ([Status]);

    PRINT 'ConfigSubSystems 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ConfigSubSystems 資料表已存在';
END
GO


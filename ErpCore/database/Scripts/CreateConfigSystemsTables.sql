-- CFG0410 - 主系統項目資料維護作業
-- 建立 ConfigSystems 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConfigSystems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ConfigSystems] (
        [SystemId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 主系統代碼 (SYS_ID)
        [SystemName] NVARCHAR(100) NOT NULL, -- 主系統名稱 (SYS_NAME)
        [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
        [SystemType] NVARCHAR(20) NULL, -- 系統型態 (SYS_TYPE)
        [ServerIp] NVARCHAR(100) NULL, -- 伺服器主機名稱 (SERVER_IP)
        [ModuleId] NVARCHAR(50) NULL, -- 模組代碼 (MODULE_ID)
        [DbUser] NVARCHAR(50) NULL, -- 資料庫使用者 (DB_USER)
        [DbPass] NVARCHAR(255) NULL, -- 資料庫密碼 (DB_PASS，加密儲存)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [Status] NVARCHAR(10) NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_ConfigSystems] PRIMARY KEY CLUSTERED ([SystemId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ConfigSystems_SystemName] ON [dbo].[ConfigSystems] ([SystemName]);
    CREATE NONCLUSTERED INDEX [IX_ConfigSystems_SystemType] ON [dbo].[ConfigSystems] ([SystemType]);
    CREATE NONCLUSTERED INDEX [IX_ConfigSystems_SeqNo] ON [dbo].[ConfigSystems] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_ConfigSystems_Status] ON [dbo].[ConfigSystems] ([Status]);

    PRINT 'ConfigSystems 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ConfigSystems 資料表已存在';
END
GO


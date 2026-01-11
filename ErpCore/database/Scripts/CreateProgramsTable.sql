-- SYS0430 - 系統作業資料維護作業
-- 建立 Programs 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Programs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Programs] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [ProgramId] NVARCHAR(50) NOT NULL, -- 作業代碼 (PROG_ID)
        [ProgramName] NVARCHAR(100) NOT NULL, -- 作業名稱 (PROG_NAME)
        [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
        [MenuId] NVARCHAR(50) NOT NULL, -- 子系統項目代碼 (MENU_ID)
        [ProgramUrl] NVARCHAR(500) NULL, -- 網頁位址 (URL)
        [ProgramType] NVARCHAR(20) NULL, -- 作業型態 (PROG_TYPE)
        [MaintainUserId] NVARCHAR(50) NULL, -- 維護者代碼 (MT_USER_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Programs] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Programs_ProgramId] UNIQUE ([ProgramId]),
        CONSTRAINT [FK_Programs_Menus] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([MenuId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Programs_ProgramId] ON [dbo].[Programs] ([ProgramId]);
    CREATE NONCLUSTERED INDEX [IX_Programs_MenuId] ON [dbo].[Programs] ([MenuId]);
    CREATE NONCLUSTERED INDEX [IX_Programs_SeqNo] ON [dbo].[Programs] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_Programs_ProgramType] ON [dbo].[Programs] ([ProgramType]);
    CREATE NONCLUSTERED INDEX [IX_Programs_Status] ON [dbo].[Programs] ([Status]);

    PRINT 'Programs 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Programs 資料表已存在';
END
GO

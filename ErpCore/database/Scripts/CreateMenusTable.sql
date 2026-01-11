-- SYS0420 - 子系統項目資料維護作業
-- 建立 Menus 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Menus]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Menus] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
        [MenuId] NVARCHAR(50) NOT NULL, -- 子系統項目代碼 (MENU_ID)
        [MenuName] NVARCHAR(100) NOT NULL, -- 子系統項目名稱 (MENU_NAME)
        [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
        [SystemId] NVARCHAR(50) NOT NULL, -- 主系統代碼 (SYS_ID)
        [ParentMenuId] NVARCHAR(50) NULL, -- 上層子系統代碼 (P_MENU_ID)
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Menus] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_Menus_MenuId] UNIQUE ([MenuId]),
        CONSTRAINT [FK_Menus_Systems] FOREIGN KEY ([SystemId]) REFERENCES [dbo].[Systems] ([SystemId]),
        CONSTRAINT [FK_Menus_ParentMenu] FOREIGN KEY ([ParentMenuId]) REFERENCES [dbo].[Menus] ([MenuId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Menus_MenuId] ON [dbo].[Menus] ([MenuId]);
    CREATE NONCLUSTERED INDEX [IX_Menus_SystemId] ON [dbo].[Menus] ([SystemId]);
    CREATE NONCLUSTERED INDEX [IX_Menus_ParentMenuId] ON [dbo].[Menus] ([ParentMenuId]);
    CREATE NONCLUSTERED INDEX [IX_Menus_SeqNo] ON [dbo].[Menus] ([SeqNo]);
    CREATE NONCLUSTERED INDEX [IX_Menus_Status] ON [dbo].[Menus] ([Status]);

    PRINT 'Menus 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Menus 資料表已存在';
END
GO

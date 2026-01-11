-- SYS0760 - 使用者異常登入報表
-- 建立 LoginLog 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoginLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LoginLog] (
        [T_KEY] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [EVENT_ID] NVARCHAR(10) NOT NULL, -- 異常事件代碼 (1:密碼錯誤等)
        [USER_ID] NVARCHAR(50) NULL, -- 使用者代碼
        [LOGIN_IP] NVARCHAR(50) NULL, -- 登入IP位址
        [EVENT_TIME] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 事件發生時間
        [BUSER] NVARCHAR(50) NULL, -- 建立使用者
        [BTIME] DATETIME2 NULL, -- 建立時間
        [CUSER] NVARCHAR(50) NULL, -- 異動使用者
        [CTIME] DATETIME2 NULL, -- 異動時間
        [CPRIORITY] INT NULL, -- 建立優先權
        [CGROUP] NVARCHAR(50) NULL, -- 建立群組
        CONSTRAINT [PK_LoginLog] PRIMARY KEY CLUSTERED ([T_KEY] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LoginLog_EVENT_ID] ON [dbo].[LoginLog] ([EVENT_ID]);
    CREATE NONCLUSTERED INDEX [IX_LoginLog_USER_ID] ON [dbo].[LoginLog] ([USER_ID]);
    CREATE NONCLUSTERED INDEX [IX_LoginLog_EVENT_TIME] ON [dbo].[LoginLog] ([EVENT_TIME] DESC);
    CREATE NONCLUSTERED INDEX [IX_LoginLog_USER_EVENT_TIME] ON [dbo].[LoginLog] ([USER_ID], [EVENT_TIME] DESC);
    
    PRINT 'LoginLog 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'LoginLog 資料表已存在';
END
GO

-- SYS0610 - 使用者基本資料異動查詢
-- 建立 ChangeLogs 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChangeLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ChangeLogs] (
        [LogId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [ProgramId] NVARCHAR(50) NOT NULL, -- 程式代碼 (SYS0110)
        [ChangeUserId] NVARCHAR(50) NULL, -- 異動使用者代碼
        [ChangeDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 異動時間
        [ChangeStatus] NVARCHAR(10) NOT NULL, -- 異動狀態: 1=新增, 2=刪除, 3=修改
        [ChangeField] NVARCHAR(500) NULL, -- 異動欄位名稱 (多個欄位以逗號分隔)
        [OldValue] NVARCHAR(MAX) NULL, -- 異動前的值 (多個值以逗號分隔)
        [NewValue] NVARCHAR(MAX) NULL, -- 異動後的值 (多個值以逗號分隔)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_ChangeLogs] PRIMARY KEY CLUSTERED ([LogId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ChangeLogs_ProgramId] ON [dbo].[ChangeLogs] ([ProgramId]);
    CREATE NONCLUSTERED INDEX [IX_ChangeLogs_ChangeUserId] ON [dbo].[ChangeLogs] ([ChangeUserId]);
    CREATE NONCLUSTERED INDEX [IX_ChangeLogs_ChangeDate] ON [dbo].[ChangeLogs] ([ChangeDate]);
    CREATE NONCLUSTERED INDEX [IX_ChangeLogs_ProgramId_ChangeDate] ON [dbo].[ChangeLogs] ([ProgramId], [ChangeDate] DESC);
END
GO


-- SYS0790 - BUTTON LOG 查詢
-- 建立 ButtonLog 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ButtonLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ButtonLog] (
        [TKey] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [BUser] NVARCHAR(50) NOT NULL, -- 使用者編號
        [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 操作時間
        [ProgId] NVARCHAR(50) NULL, -- 作業代碼
        [ProgName] NVARCHAR(100) NULL, -- 作業名稱
        [ButtonName] NVARCHAR(100) NULL, -- 按鈕名稱
        [Url] NVARCHAR(500) NULL, -- 網頁位址
        [FrameName] NVARCHAR(100) NULL, -- 框架名稱
        CONSTRAINT [PK_ButtonLog] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ButtonLog_BUser] ON [dbo].[ButtonLog] ([BUser]);
    CREATE NONCLUSTERED INDEX [IX_ButtonLog_BTime] ON [dbo].[ButtonLog] ([BTime]);
    CREATE NONCLUSTERED INDEX [IX_ButtonLog_ProgId] ON [dbo].[ButtonLog] ([ProgId]);
    CREATE NONCLUSTERED INDEX [IX_ButtonLog_BUser_BTime] ON [dbo].[ButtonLog] ([BUser], [BTime]);
END
GO


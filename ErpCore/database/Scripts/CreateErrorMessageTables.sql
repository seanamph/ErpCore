-- =============================================
-- ErrorMessage (XCOMMSG) 錯誤訊息處理資料表
-- =============================================

-- 1. 錯誤訊息記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ErrorMessages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ErrorMessages] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ErrorCode] NVARCHAR(50) NOT NULL, -- 錯誤代碼
        [ErrorType] NVARCHAR(20) NOT NULL, -- 錯誤類型 (HTTP, APPLICATION, SYSTEM, WARNING)
        [HttpStatusCode] INT NULL, -- HTTP狀態碼 (401, 404, 500等)
        [ErrorMessage] NVARCHAR(500) NOT NULL, -- 錯誤訊息
        [ErrorDetail] NVARCHAR(MAX) NULL, -- 錯誤詳細資訊
        [RequestUrl] NVARCHAR(500) NULL, -- 請求URL
        [RequestMethod] NVARCHAR(10) NULL, -- 請求方法 (GET, POST等)
        [UserId] NVARCHAR(50) NULL, -- 使用者ID
        [UserIp] NVARCHAR(50) NULL, -- 使用者IP
        [UserAgent] NVARCHAR(500) NULL, -- 使用者代理
        [StackTrace] NVARCHAR(MAX) NULL, -- 堆疊追蹤
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 發生時間
        CONSTRAINT [PK_ErrorMessages] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ErrorMessages_ErrorCode] ON [dbo].[ErrorMessages] ([ErrorCode]);
    CREATE NONCLUSTERED INDEX [IX_ErrorMessages_ErrorType] ON [dbo].[ErrorMessages] ([ErrorType]);
    CREATE NONCLUSTERED INDEX [IX_ErrorMessages_HttpStatusCode] ON [dbo].[ErrorMessages] ([HttpStatusCode]);
    CREATE NONCLUSTERED INDEX [IX_ErrorMessages_UserId] ON [dbo].[ErrorMessages] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_ErrorMessages_CreatedAt] ON [dbo].[ErrorMessages] ([CreatedAt]);
END
GO

-- 2. 錯誤訊息模板表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ErrorMessageTemplates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ErrorMessageTemplates] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ErrorCode] NVARCHAR(50) NOT NULL, -- 錯誤代碼
        [Language] NVARCHAR(10) NOT NULL DEFAULT 'zh-TW', -- 語言代碼
        [Title] NVARCHAR(200) NOT NULL, -- 錯誤標題
        [Message] NVARCHAR(500) NOT NULL, -- 錯誤訊息
        [Description] NVARCHAR(1000) NULL, -- 錯誤描述
        [Solution] NVARCHAR(1000) NULL, -- 解決方案
        [IsActive] BIT NOT NULL DEFAULT 1, -- 是否啟用
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_ErrorMessageTemplates_ErrorCode_Language] UNIQUE ([ErrorCode], [Language])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ErrorMessageTemplates_ErrorCode] ON [dbo].[ErrorMessageTemplates] ([ErrorCode]);
    CREATE NONCLUSTERED INDEX [IX_ErrorMessageTemplates_Language] ON [dbo].[ErrorMessageTemplates] ([Language]);
    CREATE NONCLUSTERED INDEX [IX_ErrorMessageTemplates_IsActive] ON [dbo].[ErrorMessageTemplates] ([IsActive]);
END
GO


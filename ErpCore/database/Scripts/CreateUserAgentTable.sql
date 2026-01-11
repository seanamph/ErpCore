-- SYS0117 - 使用者權限代理
-- 建立 UserAgent 資料表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAgent]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserAgent] (
        [AgentId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [PrincipalUserId] NVARCHAR(50) NOT NULL, -- 委託人
        [AgentUserId] NVARCHAR(50) NOT NULL, -- 代理人
        [BeginTime] DATETIME2 NOT NULL, -- 代理開始時間
        [EndTime] DATETIME2 NOT NULL, -- 代理結束時間
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserAgent_PrincipalUser] FOREIGN KEY ([PrincipalUserId]) REFERENCES [dbo].[Users] ([UserId]),
        CONSTRAINT [FK_UserAgent_AgentUser] FOREIGN KEY ([AgentUserId]) REFERENCES [dbo].[Users] ([UserId]),
        CONSTRAINT [CK_UserAgent_TimeRange] CHECK ([EndTime] > [BeginTime]),
        CONSTRAINT [CK_UserAgent_DifferentUser] CHECK ([PrincipalUserId] <> [AgentUserId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserAgent_PrincipalUserId] ON [dbo].[UserAgent] ([PrincipalUserId]);
    CREATE NONCLUSTERED INDEX [IX_UserAgent_AgentUserId] ON [dbo].[UserAgent] ([AgentUserId]);
    CREATE NONCLUSTERED INDEX [IX_UserAgent_Status] ON [dbo].[UserAgent] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_UserAgent_TimeRange] ON [dbo].[UserAgent] ([BeginTime], [EndTime]);
    
    PRINT 'UserAgent 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'UserAgent 資料表已存在';
END
GO

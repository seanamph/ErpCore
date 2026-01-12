-- SYS0116 - 排程修改使用者基本資料作業
-- 建立 UserSchedules 資料表

-- 1. 建立 UserSchedules 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSchedules]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserSchedules] (
        [ScheduleId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [UserId] NVARCHAR(50) NOT NULL,
        [ScheduleDate] DATETIME2 NOT NULL, -- 排程執行時間
        [ScheduleType] NVARCHAR(20) NOT NULL, -- 排程類型: PASSWORD_RESET, USER_UPDATE, STATUS_CHANGE
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態: PENDING, EXECUTING, COMPLETED, CANCELLED, FAILED
        [ScheduleData] NVARCHAR(MAX) NULL, -- 排程資料（JSON格式）
        [ExecuteResult] NVARCHAR(MAX) NULL, -- 執行結果
        [ErrorMessage] NVARCHAR(500) NULL, -- 錯誤訊息
        [ExecutedAt] DATETIME2 NULL, -- 實際執行時間
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserSchedules_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_UserSchedules_UserId] ON [dbo].[UserSchedules] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserSchedules_ScheduleDate] ON [dbo].[UserSchedules] ([ScheduleDate]);
    CREATE NONCLUSTERED INDEX [IX_UserSchedules_Status] ON [dbo].[UserSchedules] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_UserSchedules_ScheduleType] ON [dbo].[UserSchedules] ([ScheduleType]);
END
GO

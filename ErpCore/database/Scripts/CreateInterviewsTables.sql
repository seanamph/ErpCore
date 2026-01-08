-- =============================================
-- 訪談資料表 (SYSC222)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Interviews]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Interviews] (
        [InterviewId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 訪談ID (INTERVIEW_ID)
        [ProspectId] NVARCHAR(50) NOT NULL, -- 潛客代碼 (PROSPECT_ID)
        [InterviewDate] DATETIME2 NOT NULL, -- 訪談日期 (INTERVIEW_DATE)
        [InterviewTime] TIME NULL, -- 訪談時間 (INTERVIEW_TIME)
        [InterviewType] NVARCHAR(20) NULL, -- 訪談類型 (INTERVIEW_TYPE) PHONE:電話, FACE_TO_FACE:面對面, ONLINE:線上
        [Interviewer] NVARCHAR(50) NULL, -- 訪談人員 (INTERVIEWER)
        [InterviewLocation] NVARCHAR(200) NULL, -- 訪談地點 (INTERVIEW_LOCATION)
        [InterviewContent] NVARCHAR(MAX) NULL, -- 訪談內容 (INTERVIEW_CONTENT)
        [InterviewResult] NVARCHAR(20) NULL, -- 訪談結果 (INTERVIEW_RESULT) SUCCESS:成功, FOLLOW_UP:待追蹤, CANCELLED:取消, NO_SHOW:未到
        [NextAction] NVARCHAR(200) NULL, -- 後續行動 (NEXT_ACTION)
        [NextActionDate] DATETIME2 NULL, -- 後續行動日期 (NEXT_ACTION_DATE)
        [FollowUpDate] DATETIME2 NULL, -- 追蹤日期 (FOLLOW_UP_DATE)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE', -- 狀態 (STATUS) ACTIVE:有效, CANCELLED:取消
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Interviews] PRIMARY KEY CLUSTERED ([InterviewId] ASC),
        CONSTRAINT [FK_Interviews_Prospects] FOREIGN KEY ([ProspectId]) REFERENCES [dbo].[Prospects] ([ProspectId]) ON DELETE CASCADE
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Interviews_ProspectId] ON [dbo].[Interviews] ([ProspectId]);
    CREATE NONCLUSTERED INDEX [IX_Interviews_InterviewDate] ON [dbo].[Interviews] ([InterviewDate]);
    CREATE NONCLUSTERED INDEX [IX_Interviews_InterviewResult] ON [dbo].[Interviews] ([InterviewResult]);
    CREATE NONCLUSTERED INDEX [IX_Interviews_Status] ON [dbo].[Interviews] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Interviews_NextActionDate] ON [dbo].[Interviews] ([NextActionDate]);
    CREATE NONCLUSTERED INDEX [IX_Interviews_FollowUpDate] ON [dbo].[Interviews] ([FollowUpDate]);
    CREATE NONCLUSTERED INDEX [IX_Interviews_CreatedAt] ON [dbo].[Interviews] ([CreatedAt]);

    PRINT 'Interviews 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'Interviews 資料表已存在';
END
GO


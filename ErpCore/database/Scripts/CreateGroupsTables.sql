-- 組別資料維護作業資料表 (SYSWB70)
-- 對應舊系統 MNG_GROUP 或 ORG_GROUP 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Groups]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Groups] (
        [GroupId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 組別代碼 (GROUP_ID)
        [GroupName] NVARCHAR(100) NOT NULL, -- 組別名稱 (GROUP_NAME)
        [DeptId] NVARCHAR(50) NULL, -- 部別代碼 (DEPT_ID)
        [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED ([GroupId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Groups_GroupName] ON [dbo].[Groups] ([GroupName]);
    CREATE NONCLUSTERED INDEX [IX_Groups_DeptId] ON [dbo].[Groups] ([DeptId]);
    CREATE NONCLUSTERED INDEX [IX_Groups_OrgId] ON [dbo].[Groups] ([OrgId]);
    CREATE NONCLUSTERED INDEX [IX_Groups_Status] ON [dbo].[Groups] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Groups_SeqNo] ON [dbo].[Groups] ([SeqNo]);
    
    PRINT 'Groups 表建立成功';
END
ELSE
BEGIN
    PRINT 'Groups 表已存在';
END
GO


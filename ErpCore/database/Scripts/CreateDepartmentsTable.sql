-- 部別資料維護作業資料表 (SYSWB40)
-- 對應舊系統 MNG_DEPT 或 ORG_GROUP 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Departments] (
        [DeptId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 部別代碼 (DEPT_ID)
        [DeptName] NVARCHAR(100) NOT NULL, -- 部別名稱 (DEPT_NAME)
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
        CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED ([DeptId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Departments_DeptName] ON [dbo].[Departments] ([DeptName]);
    CREATE NONCLUSTERED INDEX [IX_Departments_OrgId] ON [dbo].[Departments] ([OrgId]);
    CREATE NONCLUSTERED INDEX [IX_Departments_Status] ON [dbo].[Departments] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Departments_SeqNo] ON [dbo].[Departments] ([SeqNo]);
    
    PRINT 'Departments 表建立成功';
END
ELSE
BEGIN
    PRINT 'Departments 表已存在';
END
GO

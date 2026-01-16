-- 區域基本資料維護作業資料表 (SYSB450)
-- 對應舊系統 RIM_AREA 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Areas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Areas] (
        [AreaId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 區域代碼 (AREA_ID)
        [AreaName] NVARCHAR(100) NOT NULL, -- 區域名稱 (AREA_NAME)
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Areas] PRIMARY KEY CLUSTERED ([AreaId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Areas_AreaName] ON [dbo].[Areas] ([AreaName]);
    CREATE NONCLUSTERED INDEX [IX_Areas_Status] ON [dbo].[Areas] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Areas_SeqNo] ON [dbo].[Areas] ([SeqNo]);
    
    PRINT 'Areas 表建立成功';
END
ELSE
BEGIN
    PRINT 'Areas 表已存在';
END
GO

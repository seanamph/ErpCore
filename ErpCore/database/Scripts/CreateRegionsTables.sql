-- 地區設定資料表 (SYSBC30)
-- 對應舊系統 REGION 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Regions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Regions] (
        [RegionId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 地區編號 (REGION_ID)
        [RegionName] NVARCHAR(100) NOT NULL, -- 地區名稱 (REGION_NAME)
        [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED ([RegionId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Regions_RegionName] ON [dbo].[Regions] ([RegionName]);
    
    PRINT 'Regions 表建立成功';
END
ELSE
BEGIN
    PRINT 'Regions 表已存在';
END
GO


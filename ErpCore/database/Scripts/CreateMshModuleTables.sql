-- =============================================
-- MSH模組系列資料表建立腳本
-- 功能代碼: MSH3000
-- 建立日期: 2025-01-09
-- =============================================

-- 1. MSH3000 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Msh3000Data]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Msh3000Data] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
        [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
        [DataValue] NVARCHAR(200) NULL, -- 資料值
        [DataType] NVARCHAR(20) NULL, -- 資料類型
        [ImagePath] NVARCHAR(500) NULL, -- 圖像路徑
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SortOrder] INT NULL DEFAULT 0, -- 排序順序
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_Msh3000Data_DataId] UNIQUE ([DataId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Msh3000Data_DataType] ON [dbo].[Msh3000Data] ([DataType]);
    CREATE NONCLUSTERED INDEX [IX_Msh3000Data_Status] ON [dbo].[Msh3000Data] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Msh3000Data_SortOrder] ON [dbo].[Msh3000Data] ([SortOrder]);

    PRINT '資料表 Msh3000Data 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 Msh3000Data 已存在';
END
GO

PRINT 'MSH模組系列資料表建立完成';
GO


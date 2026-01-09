-- =============================================
-- 通用模組類 (UNIV000) - 資料庫腳本
-- 功能：通用模組系列功能
-- 建立日期：2025-01-09
-- =============================================

-- 1. Univ000 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Univ000]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Univ000] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
        [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
        [DataType] NVARCHAR(20) NULL, -- 資料類型
        [DataValue] NVARCHAR(200) NULL, -- 資料值
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [SortOrder] INT NULL DEFAULT 0, -- 排序順序
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_Univ000_DataId] UNIQUE ([DataId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Univ000_DataType] ON [dbo].[Univ000] ([DataType]);
    CREATE NONCLUSTERED INDEX [IX_Univ000_Status] ON [dbo].[Univ000] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Univ000_SortOrder] ON [dbo].[Univ000] ([SortOrder]);
END
GO


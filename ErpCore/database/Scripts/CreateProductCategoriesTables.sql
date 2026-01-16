-- 商品分類資料維護作業資料表 (SYSB110)
-- 對應舊系統 CLASSIFY / AM_CLASSIFY / RIM_CLASSIFY 表

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductCategories] (
        [TKey] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- 主鍵 (T_KEY)
        [ClassId] NVARCHAR(50) NOT NULL, -- 分類代碼 (CLASS_ID)
        [ClassName] NVARCHAR(100) NOT NULL, -- 分類名稱 (CLASS_NAME)
        [ClassType] NVARCHAR(10) NULL DEFAULT '1', -- 分類型式 (CLASS_TYPE, 1:資料, 2:耗材)
        [ClassMode] NVARCHAR(10) NOT NULL, -- 分類區分 (CLASS_MODE, 1:大分類, 2:中分類, 3:小分類)
        [BClassId] NVARCHAR(50) NULL, -- 大分類代碼 (B_CLASS_ID, 用於中分類和小分類)
        [MClassId] NVARCHAR(50) NULL, -- 中分類代碼 (M_CLASS_ID, 用於小分類)
        [ParentTKey] BIGINT NULL, -- 父分類主鍵 (PARENT_T_KEY)
        [StypeId] NVARCHAR(50) NULL, -- 所屬會計科目(借) (STYPE_ID)
        [StypeId2] NVARCHAR(50) NULL, -- 所屬會計科目(貸) (STYPE_ID2)
        [DepreStypeId] NVARCHAR(50) NULL, -- 折舊科目(借) (DEPRE_STYPE_ID)
        [DepreStypeId2] NVARCHAR(50) NULL, -- 累計折舊科目(貸) (DEPRE_STYPE_ID2)
        [StypeTax] NVARCHAR(50) NULL, -- 進項稅額科目(借) (STYPE_TAX)
        [ItemCount] INT NULL DEFAULT 0, -- 所屬項目個數 (ITEM_COUNT)
        [Status] NVARCHAR(10) NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
        [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
        [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
        [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
        CONSTRAINT [PK_ProductCategories] PRIMARY KEY CLUSTERED ([TKey] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ProductCategories_ClassId] ON [dbo].[ProductCategories] ([ClassId]);
    CREATE NONCLUSTERED INDEX [IX_ProductCategories_ClassMode] ON [dbo].[ProductCategories] ([ClassMode]);
    CREATE NONCLUSTERED INDEX [IX_ProductCategories_BClassId] ON [dbo].[ProductCategories] ([BClassId]);
    CREATE NONCLUSTERED INDEX [IX_ProductCategories_MClassId] ON [dbo].[ProductCategories] ([MClassId]);
    CREATE NONCLUSTERED INDEX [IX_ProductCategories_ParentTKey] ON [dbo].[ProductCategories] ([ParentTKey]);
    CREATE NONCLUSTERED INDEX [IX_ProductCategories_ClassType] ON [dbo].[ProductCategories] ([ClassType]);
    CREATE NONCLUSTERED INDEX [IX_ProductCategories_Status] ON [dbo].[ProductCategories] ([Status]);
    
    -- 唯一性約束：分類代碼在同一個分類區分和父分類下必須唯一
    -- 注意：由於 ParentTKey 可能為 NULL，使用唯一索引而非約束
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_ProductCategories_ClassId_ClassMode_ParentTKey] 
    ON [dbo].[ProductCategories] ([ClassId], [ClassMode], [ParentTKey])
    WHERE [ParentTKey] IS NOT NULL;
    
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_ProductCategories_ClassId_ClassMode_NullParent] 
    ON [dbo].[ProductCategories] ([ClassId], [ClassMode])
    WHERE [ParentTKey] IS NULL;
    
    PRINT 'ProductCategories 表建立成功';
END
ELSE
BEGIN
    PRINT 'ProductCategories 表已存在';
END
GO


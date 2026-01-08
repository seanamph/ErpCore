-- 可管控欄位資料維護相關資料表
-- SYS0510 - 可管控欄位資料維護

-- 1. ControllableFields - 可管控欄位主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ControllableFields]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ControllableFields] (
        [FieldId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [FieldName] NVARCHAR(100) NOT NULL,
        [DbName] NVARCHAR(100) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [FieldNameInDb] NVARCHAR(100) NOT NULL,
        [FieldType] NVARCHAR(50) NULL,
        [FieldDescription] NVARCHAR(500) NULL,
        [IsRequired] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [SortOrder] INT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ControllableFields] PRIMARY KEY CLUSTERED ([FieldId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ControllableFields_DbTable] ON [dbo].[ControllableFields] ([DbName], [TableName]);
    CREATE NONCLUSTERED INDEX [IX_ControllableFields_IsActive] ON [dbo].[ControllableFields] ([IsActive]);
    CREATE NONCLUSTERED INDEX [IX_ControllableFields_SortOrder] ON [dbo].[ControllableFields] ([SortOrder]);

    PRINT 'ControllableFields 資料表建立成功';
END
ELSE
BEGIN
    PRINT 'ControllableFields 資料表已存在';
END
GO


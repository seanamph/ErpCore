-- =============================================
-- 檔案上傳工具資料表建立腳本
-- =============================================

-- 1. 檔案上傳記錄 (FileUploads)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileUploads]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FileUploads] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [FileName] NVARCHAR(255) NOT NULL,
        [OriginalFileName] NVARCHAR(255) NOT NULL,
        [FilePath] NVARCHAR(500) NOT NULL,
        [FileSize] BIGINT NOT NULL,
        [FileType] NVARCHAR(50) NULL,
        [FileExtension] NVARCHAR(10) NULL,
        [UploadPath] NVARCHAR(200) NULL,
        [UploadedBy] NVARCHAR(50) NULL,
        [UploadedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
        [RelatedTable] NVARCHAR(50) NULL,
        [RelatedId] NVARCHAR(50) NULL,
        [Description] NVARCHAR(500) NULL
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_FileUploads_FileName] ON [dbo].[FileUploads] ([FileName]);
    CREATE NONCLUSTERED INDEX [IX_FileUploads_UploadedBy] ON [dbo].[FileUploads] ([UploadedBy]);
    CREATE NONCLUSTERED INDEX [IX_FileUploads_UploadedAt] ON [dbo].[FileUploads] ([UploadedAt]);
    CREATE NONCLUSTERED INDEX [IX_FileUploads_Status] ON [dbo].[FileUploads] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_FileUploads_Related] ON [dbo].[FileUploads] ([RelatedTable], [RelatedId]);
END
GO


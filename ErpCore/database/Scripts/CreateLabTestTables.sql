-- Lab實驗室測試功能資料表
-- 測試和開發相關功能

-- 測試結果表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestResults]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TestResults] (
        [TestId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TestName] NVARCHAR(200) NOT NULL,
        [TestType] NVARCHAR(50) NOT NULL,
        [TestData] NVARCHAR(MAX) NULL,
        [TestResult] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [Duration] INT NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_TestResults] PRIMARY KEY CLUSTERED ([TestId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_TestResults_TestType] ON [dbo].[TestResults] ([TestType]);
    CREATE NONCLUSTERED INDEX [IX_TestResults_Status] ON [dbo].[TestResults] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_TestResults_CreatedAt] ON [dbo].[TestResults] ([CreatedAt]);
    
    PRINT 'TestResults 表建立成功';
END
GO


-- =============================================
-- ChartTools 圖表與工具類資料表
-- =============================================

-- 1. 圖表配置表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChartConfigs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ChartConfigs] (
        [ChartConfigId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ChartName] NVARCHAR(100) NOT NULL,
        [ChartType] NVARCHAR(20) NOT NULL, -- BAR, LINE, PIE, COLUMN, AREA, etc.
        [DataSource] NVARCHAR(500) NULL, -- SQL Query or API Endpoint
        [XField] NVARCHAR(100) NULL,
        [YField] NVARCHAR(100) NULL,
        [Title] NVARCHAR(200) NULL,
        [XAxisTitle] NVARCHAR(100) NULL,
        [YAxisTitle] NVARCHAR(100) NULL,
        [Width] INT NULL DEFAULT 800,
        [Height] INT NULL DEFAULT 400,
        [Colors] NVARCHAR(MAX) NULL, -- JSON array of colors
        [Options] NVARCHAR(MAX) NULL, -- JSON object for chart options
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ChartConfigs_ChartName] ON [dbo].[ChartConfigs] ([ChartName]);
    CREATE NONCLUSTERED INDEX [IX_ChartConfigs_ChartType] ON [dbo].[ChartConfigs] ([ChartType]);
END
GO


-- SYSL161 - 業務報表列印記錄作業資料表
-- 更新 BusinessReportPrintLog 資料表，新增 FilePath 欄位

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessReportPrintLog]') AND name = 'FilePath')
BEGIN
    ALTER TABLE [dbo].[BusinessReportPrintLog]
    ADD [FilePath] NVARCHAR(500) NULL; -- 檔案路徑（相對路徑） (FILE_PATH)
    
    PRINT '已新增 FilePath 欄位到 BusinessReportPrintLog 資料表';
END
ELSE
BEGIN
    PRINT 'FilePath 欄位已存在於 BusinessReportPrintLog 資料表';
END
GO


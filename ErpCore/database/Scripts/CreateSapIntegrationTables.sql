-- =============================================
-- SAP整合模組類 (TransSAP) - 資料庫腳本
-- 功能：SAP系統整合功能
-- 建立日期：2025-01-09
-- =============================================

-- 1. TransSap 資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransSap]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransSap] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [TransId] NVARCHAR(50) NOT NULL, -- 交易單號
        [TransType] NVARCHAR(20) NOT NULL, -- 交易類型
        [SapSystemCode] NVARCHAR(20) NOT NULL, -- SAP系統代碼
        [TransDate] DATETIME2 NOT NULL, -- 交易日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 交易狀態 (P:處理中, S:成功, F:失敗)
        [RequestData] NVARCHAR(MAX) NULL, -- 請求資料 (JSON格式)
        [ResponseData] NVARCHAR(MAX) NULL, -- 回應資料 (JSON格式)
        [ErrorMessage] NVARCHAR(500) NULL, -- 錯誤訊息
        [RetryCount] INT NOT NULL DEFAULT 0, -- 重試次數
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_TransSap_TransId] UNIQUE ([TransId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_TransSap_TransType] ON [dbo].[TransSap] ([TransType]);
    CREATE NONCLUSTERED INDEX [IX_TransSap_SapSystemCode] ON [dbo].[TransSap] ([SapSystemCode]);
    CREATE NONCLUSTERED INDEX [IX_TransSap_Status] ON [dbo].[TransSap] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_TransSap_TransDate] ON [dbo].[TransSap] ([TransDate]);
END
GO


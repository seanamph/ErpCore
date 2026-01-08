-- =============================================
-- 更新採購單資料表：添加 SourceType 欄位
-- 功能代碼: SYSW322
-- 建立日期: 2025-01-08
-- =============================================

-- 添加 SourceType 欄位（如果不存在）
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrders]') 
    AND name = 'SourceType'
)
BEGIN
    ALTER TABLE [dbo].[PurchaseOrders]
    ADD [SourceType] NVARCHAR(20) NULL DEFAULT 'NORMAL';
    
    -- 更新現有資料
    UPDATE [dbo].[PurchaseOrders]
    SET [SourceType] = CASE 
        WHEN [SourceProgram] = 'SYSW322' THEN 'ON_SITE'
        ELSE 'NORMAL'
    END;
    
    -- 建立索引
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_SourceType] 
    ON [dbo].[PurchaseOrders] ([SourceType]);
    
    PRINT '欄位 SourceType 新增成功';
END
ELSE
BEGIN
    PRINT '欄位 SourceType 已存在';
END
GO


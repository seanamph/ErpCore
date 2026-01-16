-- =============================================
-- 採購單查詢視圖建立腳本
-- 功能代碼: SYSP310-SYSP330
-- 建立日期: 2025-01-27
-- 說明: 建立採購單查詢相關視圖，用於查詢、統計、報表功能
-- =============================================

-- 1. 採購單查詢視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_PurchaseOrderQuery]'))
BEGIN
    DROP VIEW [dbo].[v_PurchaseOrderQuery];
    PRINT '視圖 v_PurchaseOrderQuery 已刪除';
END
GO

CREATE VIEW [dbo].[v_PurchaseOrderQuery] AS
SELECT 
    po.[OrderId],
    po.[OrderDate],
    po.[OrderType],
    CASE po.[OrderType]
        WHEN 'PO' THEN '採購'
        WHEN 'RT' THEN '退貨'
        ELSE po.[OrderType]
    END AS [OrderTypeName],
    po.[ShopId],
    ISNULL(s.[ShopName], '') AS [ShopName],
    po.[SupplierId],
    ISNULL(sup.[SupplierName], '') AS [SupplierName],
    po.[Status],
    CASE po.[Status]
        WHEN 'D' THEN '草稿'
        WHEN 'S' THEN '已送出'
        WHEN 'A' THEN '已審核'
        WHEN 'X' THEN '已取消'
        WHEN 'C' THEN '已結案'
        ELSE '未知'
    END AS [StatusName],
    po.[TotalAmount],
    po.[TotalQty],
    po.[ApplyUserId],
    ISNULL(u1.[UserName], '') AS [ApplyUserName],
    po.[ApplyDate],
    po.[ApproveUserId],
    ISNULL(u2.[UserName], '') AS [ApproveUserName],
    po.[ApproveDate],
    po.[ExpectedDate],
    po.[Memo],
    po.[SiteId],
    po.[OrgId],
    po.[CurrencyId],
    po.[ExchangeRate],
    po.[SourceProgram],
    po.[CreatedBy],
    po.[CreatedAt],
    po.[UpdatedBy],
    po.[UpdatedAt],
    -- 統計欄位
    (SELECT COUNT(*) FROM [dbo].[PurchaseOrderDetails] pod WHERE pod.[OrderId] = po.[OrderId]) AS [DetailCount],
    ISNULL((SELECT SUM(pod.[ReceivedQty]) FROM [dbo].[PurchaseOrderDetails] pod WHERE pod.[OrderId] = po.[OrderId]), 0) AS [TotalReceivedQty],
    ISNULL((SELECT SUM(pod.[ReturnQty]) FROM [dbo].[PurchaseOrderDetails] pod WHERE pod.[OrderId] = po.[OrderId]), 0) AS [TotalReturnQty]
FROM [dbo].[PurchaseOrders] po
LEFT JOIN [dbo].[Shops] s ON po.[ShopId] = s.[ShopId]
LEFT JOIN [dbo].[Suppliers] sup ON po.[SupplierId] = sup.[SupplierId]
LEFT JOIN [dbo].[Users] u1 ON po.[ApplyUserId] = u1.[UserId]
LEFT JOIN [dbo].[Users] u2 ON po.[ApproveUserId] = u2.[UserId];
GO

PRINT '視圖 v_PurchaseOrderQuery 建立成功';
GO

-- 2. 採購單明細查詢視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_PurchaseOrderDetailQuery]'))
BEGIN
    DROP VIEW [dbo].[v_PurchaseOrderDetailQuery];
    PRINT '視圖 v_PurchaseOrderDetailQuery 已刪除';
END
GO

CREATE VIEW [dbo].[v_PurchaseOrderDetailQuery] AS
SELECT 
    pod.[DetailId],
    pod.[OrderId],
    po.[OrderDate],
    po.[OrderType],
    CASE po.[OrderType]
        WHEN 'PO' THEN '採購'
        WHEN 'RT' THEN '退貨'
        ELSE po.[OrderType]
    END AS [OrderTypeName],
    po.[ShopId],
    ISNULL(s.[ShopName], '') AS [ShopName],
    po.[SupplierId],
    ISNULL(sup.[SupplierName], '') AS [SupplierName],
    po.[Status],
    CASE po.[Status]
        WHEN 'D' THEN '草稿'
        WHEN 'S' THEN '已送出'
        WHEN 'A' THEN '已審核'
        WHEN 'X' THEN '已取消'
        WHEN 'C' THEN '已結案'
        ELSE '未知'
    END AS [StatusName],
    pod.[LineNum],
    pod.[GoodsId],
    ISNULL(g.[GoodsName], '') AS [GoodsName],
    pod.[BarcodeId],
    pod.[OrderQty],
    pod.[UnitPrice],
    pod.[Amount],
    ISNULL(pod.[ReceivedQty], 0) AS [ReceivedQty],
    ISNULL(pod.[ReturnQty], 0) AS [ReturnQty],
    pod.[UnitId],
    ISNULL(pod.[TaxRate], 0) AS [TaxRate],
    ISNULL(pod.[TaxAmount], 0) AS [TaxAmount],
    pod.[Memo],
    (pod.[OrderQty] - ISNULL(pod.[ReceivedQty], 0) + ISNULL(pod.[ReturnQty], 0)) AS [PendingQty]
FROM [dbo].[PurchaseOrderDetails] pod
INNER JOIN [dbo].[PurchaseOrders] po ON pod.[OrderId] = po.[OrderId]
LEFT JOIN [dbo].[Shops] s ON po.[ShopId] = s.[ShopId]
LEFT JOIN [dbo].[Suppliers] sup ON po.[SupplierId] = sup.[SupplierId]
LEFT JOIN [dbo].[Goods] g ON pod.[GoodsId] = g.[GoodsId];
GO

PRINT '視圖 v_PurchaseOrderDetailQuery 建立成功';
GO

-- =============================================
-- 耗材出庫明細表視圖建立腳本
-- 功能代碼: SYSA1013
-- 建立日期: 2025-01-27
-- =============================================

-- 檢查並刪除現有視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[MaterialOutboundDetailView]'))
BEGIN
    DROP VIEW [dbo].[MaterialOutboundDetailView];
    PRINT '已刪除現有視圖 MaterialOutboundDetailView';
END
GO

-- 建立耗材出庫明細視圖
CREATE VIEW [dbo].[MaterialOutboundDetailView] AS
SELECT 
    A.SourceId AS TxnNo,
    A.StocksDate AS TxnDate,
    CB.CategoryName AS BId,
    CM.CategoryName AS MId,
    CS.CategoryName AS SId,
    Goods.GoodsId,
    Goods.GoodsName,
    Goods.Notes AS PackUnit,
    ISNULL(P1.Content, Goods.Unit) AS Unit,
    A.McAmt AS Amt,
    (A.Qty * -1) AS ApplyQty,
    A.Qty * -1 AS Qty,
    (A.Qty * -1 * A.McAmt) AS NAmt,
    ISNULL(P3.Content, M.Use) AS Use,
    CASE 
        WHEN E.SupplierId IS NOT NULL THEN E.SupplierId + '/' + ISNULL(E.SupplierName, '')
        ELSE ''
    END AS Vendor,
    ISNULL(P2.Content, A.StocksStatus) AS StocksType,
    ISNULL(Org.OrgName, ISNULL(E.OrgId, M.OrgId)) AS OrgId,
    M.OrgAllocation
FROM InventoryStocks A
LEFT JOIN Goods ON A.GoodsId = Goods.GoodsId
LEFT JOIN GoodsCategories CB ON Goods.BId = CB.CategoryId AND CB.CategoryType = 'B'
LEFT JOIN GoodsCategories CM ON Goods.MId = CM.CategoryId AND CM.CategoryType = 'M'
LEFT JOIN GoodsCategories CS ON Goods.SId = CS.CategoryId AND CS.CategoryType = 'S'
LEFT JOIN (
    SELECT T1.TxnNo, T1.SupplierId, T1.SupplierName, T1.OrgId, T2.Qty, T2.GoodsId
    FROM AcceptanceM T1
    INNER JOIN AcceptanceD T2 ON T1.TxnNo = T2.TxnNo
) E ON A.SourceId = E.TxnNo AND A.GoodsId = E.GoodsId
LEFT JOIN (
    SELECT T1.TxnNo, T1.OrgId, T1.OrgAllocation, T2.Use, T2.GoodsId
    FROM RequisitionM T1
    INNER JOIN RequisitionD T2 ON T1.TxnNo = T2.TxnNo
) M ON A.SourceId = M.TxnNo AND A.GoodsId = M.GoodsId
LEFT JOIN Parameters P1 ON Goods.Unit = P1.Tag AND P1.Title = 'NAM_MATERIAL'
LEFT JOIN Parameters P2 ON P2.Tag = A.StocksStatus AND P2.Title = 'STOCKS_TYPE'
LEFT JOIN Parameters P3 ON P3.Tag = M.Use AND P3.Title = 'GOODS_USE'
LEFT JOIN Organizations Org ON ISNULL(E.OrgId, M.OrgId) = Org.OrgId
WHERE A.StocksStatus IN ('2', '3', '4', '5', '6') -- 只查詢出庫狀態
    AND A.Qty < 0; -- 只查詢負數數量（出庫）
GO

PRINT '視圖 MaterialOutboundDetailView 建立成功';
GO

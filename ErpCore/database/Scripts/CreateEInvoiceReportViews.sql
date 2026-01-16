-- 電子發票報表查詢視圖 (ECA4010, ECA4020, ECA4030, ECA4040, ECA4050, ECA4060)
-- 對應舊系統電子發票報表查詢功能

-- ECA4010: 訂單明細報表視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ECA4010_Report]'))
    DROP VIEW [dbo].[V_ECA4010_Report];
GO

CREATE VIEW [dbo].[V_ECA4010_Report] AS
SELECT 
    ei.[InvoiceId] AS INVOICE_ID,
    ei.[OrderNo] AS ORDERM_NO,
    ei.[OrderDate] AS ORDERM_DATE,
    ei.[OrderStatus] AS ORDERM_STATUS,
    ei.[OrderQty] AS ORDERM_QTY,
    ei.[OrderSubtotal] AS ORDERM_SUBTOTAL,
    ISNULL(s.[ShopId], ei.[StoreId]) AS STORE_ID,
    ISNULL(s.[ShopName], ei.[StoreId]) AS STORE_NAME,
    eu.[RetailerId] AS RETAILER_ID,
    NULL AS RETAILER_NAME,
    ei.[ProviderId] AS PROVIDER_ID,
    v.[VendorName] AS PROVIDER_NAME,
    NULL AS SC_ID,
    NULL AS SC_NAME,
    ei.[GoodsId] AS GOODS_ID,
    ei.[GoodsName] AS GOODS_NAME,
    ei.[SpecId] AS SPEC_ID,
    ei.[ProviderGoodsId] AS PROVIDER_GOODS_ID,
    ei.[CreatedAt] AS CREATED_AT,
    ei.[UpdatedAt] AS UPDATED_AT
FROM [dbo].[EInvoices] ei
LEFT JOIN [dbo].[Vendors] v ON ei.[ProviderId] = v.[VendorId]
LEFT JOIN [dbo].[Shops] s ON ei.[StoreId] = s.[ShopId]
LEFT JOIN [dbo].[EInvoiceUploads] eu ON ei.[UploadId] = eu.[UploadId]
WHERE ei.[ProcessStatus] = 'COMPLETED';
GO

-- ECA4020: 商品銷售統計報表視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ECA4020_Report]'))
    DROP VIEW [dbo].[V_ECA4020_Report];
GO

CREATE VIEW [dbo].[V_ECA4020_Report] AS
SELECT 
    ISNULL(sc.[ScId], '') AS SC_ID,
    ISNULL(sc.[ScName], '') AS SC_NAME,
    ei.[GoodsId] AS GOODS_ID,
    ei.[GoodsName] AS GOODS_NAME,
    ei.[ProviderGoodsId] AS PROVIDER_GOODS_ID,
    SUM(ei.[OrderQty]) AS SUM_QTY,
    SUM(ei.[OrderSubtotal]) AS SUM_SUBTOTAL,
    SUM(ei.[OrderShippingFee]) AS SUM_FEE,
    CASE 
        WHEN SUM(ei.[OrderQty]) > 0 
        THEN SUM(ei.[OrderSubtotal]) / SUM(ei.[OrderQty])
        ELSE 0 
    END AS AVG_PRICE,
    CASE 
        WHEN (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED') > 0
        THEN (SUM(ei.[OrderSubtotal]) * 100.0) / (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED')
        ELSE 0
    END AS SALES_PERCENT,
    ROW_NUMBER() OVER (ORDER BY SUM(ei.[OrderSubtotal]) DESC) AS SALES_RANKING
FROM [dbo].[EInvoices] ei
LEFT JOIN [dbo].[StoreCounters] sc ON ei.[ScId] = sc.[ScId]
WHERE ei.[ProcessStatus] = 'COMPLETED'
GROUP BY ISNULL(sc.[ScId], ''), ISNULL(sc.[ScName], ''), ei.[GoodsId], ei.[GoodsName], ei.[ProviderGoodsId];
GO

-- 索引視圖（如果需要）
-- CREATE UNIQUE CLUSTERED INDEX [IX_V_ECA4020_Report_GoodsId] ON [dbo].[V_ECA4020_Report] ([GOODS_ID], [SC_ID]);
GO

-- ECA4030: 零售商銷售統計報表視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ECA4030_Report]'))
    DROP VIEW [dbo].[V_ECA4030_Report];
GO

CREATE VIEW [dbo].[V_ECA4030_Report] AS
SELECT 
    ISNULL(r.[RetailerId], '') AS RETAILER_ID,
    ISNULL(r.[RetailerName], '') AS RETAILER_NAME,
    SUM(ei.[OrderQty]) AS SUM_QTY,
    SUM(ei.[OrderSubtotal]) AS SUM_SUBTOTAL,
    SUM(ei.[OrderShippingFee]) AS SUM_FEE,
    CASE 
        WHEN (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED') > 0
        THEN (SUM(ei.[OrderSubtotal]) * 100.0) / (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED')
        ELSE 0
    END AS SALES_PERCENT,
    ROW_NUMBER() OVER (ORDER BY SUM(ei.[OrderSubtotal]) DESC) AS SALES_RANKING
FROM [dbo].[EInvoices] ei
LEFT JOIN [dbo].[EInvoiceUploads] eu ON ei.[UploadId] = eu.[UploadId]
LEFT JOIN [dbo].[Retailers] r ON eu.[RetailerId] = r.[RetailerId]
WHERE ei.[ProcessStatus] = 'COMPLETED'
GROUP BY ISNULL(r.[RetailerId], ''), ISNULL(r.[RetailerName], '');
GO

-- ECA4040: 店別銷售統計報表視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ECA4040_Report]'))
    DROP VIEW [dbo].[V_ECA4040_Report];
GO

CREATE VIEW [dbo].[V_ECA4040_Report] AS
SELECT 
    ISNULL(s.[StoreId], '') AS STORE_ID,
    ISNULL(s.[StoreName], '') AS STORE_NAME,
    ISNULL(s.[FloorId], '') AS STORE_FLOOR,
    ISNULL(s.[StoreType], '') AS STORE_TYPE,
    SUM(ei.[OrderQty]) AS SUM_QTY,
    SUM(ei.[OrderSubtotal]) AS SUM_SUBTOTAL,
    SUM(ei.[OrderShippingFee]) AS SUM_FEE,
    CASE 
        WHEN (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED') > 0
        THEN (SUM(ei.[OrderSubtotal]) * 100.0) / (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED')
        ELSE 0
    END AS SALES_PERCENT,
    ROW_NUMBER() OVER (ORDER BY SUM(ei.[OrderSubtotal]) DESC) AS SALES_RANKING
FROM [dbo].[EInvoices] ei
LEFT JOIN [dbo].[Stores] s ON ei.[StoreId] = s.[StoreId]
WHERE ei.[ProcessStatus] = 'COMPLETED'
GROUP BY ISNULL(s.[StoreId], ''), ISNULL(s.[StoreName], ''), ISNULL(s.[FloorId], ''), ISNULL(s.[StoreType], '');
GO

-- ECA4050: 出貨日期統計報表視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ECA4050_Report]'))
    DROP VIEW [dbo].[V_ECA4050_Report];
GO

CREATE VIEW [dbo].[V_ECA4050_Report] AS
SELECT 
    ei.[OrderShipDate] AS ORDERM_SHIPDATE,
    SUM(ei.[OrderQty]) AS SUM_QTY,
    SUM(ei.[OrderSubtotal]) AS SUM_SUBTOTAL,
    SUM(ei.[OrderShippingFee]) AS SUM_FEE,
    CASE 
        WHEN (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED') > 0
        THEN (SUM(ei.[OrderSubtotal]) * 100.0) / (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED')
        ELSE 0
    END AS SALES_PERCENT
FROM [dbo].[EInvoices] ei
WHERE ei.[ProcessStatus] = 'COMPLETED'
GROUP BY ei.[OrderShipDate];
GO

-- ECA4060: 訂單日期統計報表視圖
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ECA4060_Report]'))
    DROP VIEW [dbo].[V_ECA4060_Report];
GO

CREATE VIEW [dbo].[V_ECA4060_Report] AS
SELECT 
    ei.[OrderDate] AS ORDERM_DATE,
    SUM(CASE WHEN ei.[InvoiceStatus] = 'Y' THEN ei.[OrderQty] ELSE 0 END) AS SUM_Y_QTY,
    SUM(CASE WHEN ei.[InvoiceStatus] = 'Y' THEN ei.[OrderSubtotal] ELSE 0 END) AS SUM_Y_SUBTOTAL,
    SUM(CASE WHEN ei.[InvoiceStatus] = 'Y' THEN ei.[OrderShippingFee] ELSE 0 END) AS SUM_Y_FEE,
    CASE 
        WHEN (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED' AND [InvoiceStatus] = 'Y') > 0
        THEN (SUM(CASE WHEN ei.[InvoiceStatus] = 'Y' THEN ei.[OrderSubtotal] ELSE 0 END) * 100.0) / (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED' AND [InvoiceStatus] = 'Y')
        ELSE 0
    END AS SALES_Y_PERCENT,
    SUM(CASE WHEN ei.[InvoiceStatus] = 'N' THEN ei.[OrderQty] ELSE 0 END) AS SUM_N_QTY,
    SUM(CASE WHEN ei.[InvoiceStatus] = 'N' THEN ei.[OrderSubtotal] ELSE 0 END) AS SUM_N_SUBTOTAL,
    SUM(CASE WHEN ei.[InvoiceStatus] = 'N' THEN ei.[OrderShippingFee] ELSE 0 END) AS SUM_N_FEE,
    CASE 
        WHEN (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED' AND [InvoiceStatus] = 'N') > 0
        THEN (SUM(CASE WHEN ei.[InvoiceStatus] = 'N' THEN ei.[OrderSubtotal] ELSE 0 END) * 100.0) / (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED' AND [InvoiceStatus] = 'N')
        ELSE 0
    END AS SALES_N_PERCENT,
    SUM(ei.[OrderQty]) AS SUM_A_QTY,
    SUM(ei.[OrderSubtotal]) AS SUM_A_SUBTOTAL,
    SUM(ei.[OrderShippingFee]) AS SUM_A_FEE,
    CASE 
        WHEN (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED') > 0
        THEN (SUM(ei.[OrderSubtotal]) * 100.0) / (SELECT SUM([OrderSubtotal]) FROM [dbo].[EInvoices] WHERE [ProcessStatus] = 'COMPLETED')
        ELSE 0
    END AS SALES_A_PERCENT
FROM [dbo].[EInvoices] ei
WHERE ei.[ProcessStatus] = 'COMPLETED'
GROUP BY ei.[OrderDate];
GO

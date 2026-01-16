-- 電子發票處理作業資料表 (ECA3010)
-- 對應舊系統電子發票上傳記錄和電子發票主檔

-- 1. 電子發票上傳記錄表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EInvoiceUploads]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EInvoiceUploads] (
        [UploadId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [FileName] NVARCHAR(500) NOT NULL, -- 檔案名稱
        [FileSize] BIGINT NULL, -- 檔案大小 (bytes)
        [FileType] NVARCHAR(50) NULL, -- 檔案類型 (Excel, XML等)
        [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 上傳時間
        [UploadBy] NVARCHAR(50) NULL, -- 上傳者
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (PENDING, PROCESSING, COMPLETED, FAILED)
        [ProcessStartDate] DATETIME2 NULL, -- 處理開始時間
        [ProcessEndDate] DATETIME2 NULL, -- 處理結束時間
        [TotalRecords] INT NULL DEFAULT 0, -- 總筆數
        [SuccessRecords] INT NULL DEFAULT 0, -- 成功筆數
        [FailedRecords] INT NULL DEFAULT 0, -- 失敗筆數
        [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
        [ProcessLog] NVARCHAR(MAX) NULL, -- 處理日誌
        [StoreId] NVARCHAR(50) NULL, -- 店別ID
        [RetailerId] NVARCHAR(50) NULL, -- 零售商ID
        [UploadType] NVARCHAR(50) NULL DEFAULT 'ECA3010', -- 上傳類型 (ECA2050, ECA3010, ECA3030等)
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_EInvoiceUploads] PRIMARY KEY CLUSTERED ([UploadId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EInvoiceUploads_Status] ON [dbo].[EInvoiceUploads] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_EInvoiceUploads_UploadDate] ON [dbo].[EInvoiceUploads] ([UploadDate]);
    CREATE NONCLUSTERED INDEX [IX_EInvoiceUploads_UploadBy] ON [dbo].[EInvoiceUploads] ([UploadBy]);
    CREATE NONCLUSTERED INDEX [IX_EInvoiceUploads_StoreId] ON [dbo].[EInvoiceUploads] ([StoreId]);
    CREATE NONCLUSTERED INDEX [IX_EInvoiceUploads_UploadType] ON [dbo].[EInvoiceUploads] ([UploadType]);
END
GO

-- 2. 電子發票主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EInvoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EInvoices] (
        [InvoiceId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [UploadId] BIGINT NULL, -- 上傳記錄ID
        [OrderNo] NVARCHAR(100) NULL, -- 訂單編號 (ORDERM_NO)
        [RetailerOrderNo] NVARCHAR(100) NULL, -- 零售商訂單編號 (RETAILER_ORDERM_NO)
        [RetailerOrderDetailNo] NVARCHAR(100) NULL, -- 零售商訂單明細編號 (RETAILER_ORDERD_NO)
        [OrderDate] DATETIME2 NULL, -- 訂單日期 (ORDERM_DATE)
        [StoreId] NVARCHAR(50) NULL, -- 店別ID (STORE_ID)
        [ProviderId] NVARCHAR(50) NULL, -- 供應商ID (PROVIDER_ID)
        [NdType] NVARCHAR(50) NULL, -- 類型 (ND_TYPE)
        [GoodsId] NVARCHAR(100) NULL, -- 商品ID (GOODS_ID)
        [GoodsName] NVARCHAR(500) NULL, -- 商品名稱 (GOODS_NAME)
        [SpecId] NVARCHAR(100) NULL, -- 規格ID (SPEC_ID)
        [ProviderGoodsId] NVARCHAR(100) NULL, -- 供應商商品ID (PROVIDER_GOODS_ID)
        [SpecColor] NVARCHAR(100) NULL, -- 規格顏色 (SPEC_COLOR)
        [SpecSize] NVARCHAR(100) NULL, -- 規格尺寸 (SPEC_SIZE)
        [SuggestPrice] DECIMAL(18,2) NULL, -- 建議價格 (SUGGEST_PRICE)
        [InternetPrice] DECIMAL(18,2) NULL, -- 網路價格 (INTERNET_PRICE)
        [ShippingType] NVARCHAR(50) NULL, -- 運送類型 (SHIPPING_TYPE)
        [ShippingFee] DECIMAL(18,2) NULL, -- 運費 (SHIPPING_FEE)
        [OrderQty] INT NULL, -- 訂單數量 (ORDERM_QTY)
        [OrderShippingFee] DECIMAL(18,2) NULL, -- 訂單運費 (ORDERM_SHIPPING_FEE)
        [OrderSubtotal] DECIMAL(18,2) NULL, -- 訂單小計 (ORDERM_SUBTOTAL)
        [OrderTotal] DECIMAL(18,2) NULL, -- 訂單總計 (ORDERM_TOTAL)
        [OrderStatus] NVARCHAR(50) NULL, -- 訂單狀態 (ORDERM_STATUS)
        [ProcessStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 處理狀態
        [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_EInvoices] PRIMARY KEY CLUSTERED ([InvoiceId] ASC),
        CONSTRAINT [FK_EInvoices_EInvoiceUploads] FOREIGN KEY ([UploadId]) REFERENCES [dbo].[EInvoiceUploads] ([UploadId]) ON DELETE SET NULL
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_EInvoices_UploadId] ON [dbo].[EInvoices] ([UploadId]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_OrderNo] ON [dbo].[EInvoices] ([OrderNo]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_RetailerOrderNo] ON [dbo].[EInvoices] ([RetailerOrderNo]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_OrderDate] ON [dbo].[EInvoices] ([OrderDate]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_StoreId] ON [dbo].[EInvoices] ([StoreId]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_ProcessStatus] ON [dbo].[EInvoices] ([ProcessStatus]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_ProviderId] ON [dbo].[EInvoices] ([ProviderId]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_GoodsId] ON [dbo].[EInvoices] ([GoodsId]);
    CREATE NONCLUSTERED INDEX [IX_EInvoices_OrderStatus] ON [dbo].[EInvoices] ([OrderStatus]);
    -- 複合索引：優化日期範圍 + 狀態查詢 (ECA3020)
    CREATE NONCLUSTERED INDEX [IX_EInvoices_OrderDate_ProcessStatus] ON [dbo].[EInvoices] ([OrderDate], [ProcessStatus]);
END
GO


-- =============================================
-- 租賃管理SYSM類資料表建立腳本
-- 功能代碼: SYSM000
-- 建立日期: 2025-01-09
-- =============================================

-- 注意：SYSM 類的租賃資料使用與 SYS8000 相同的 Leases 資料表
-- 但增加停車位和合同相關欄位

-- 1. 檢查並新增停車位相關欄位
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Leases]') AND name = 'ParkingSpaceId')
BEGIN
    ALTER TABLE [dbo].[Leases] ADD [ParkingSpaceId] NVARCHAR(50) NULL;
    PRINT '已新增 ParkingSpaceId 欄位至 Leases 資料表';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Leases]') AND name = 'ParkingFee')
BEGIN
    ALTER TABLE [dbo].[Leases] ADD [ParkingFee] DECIMAL(18, 4) NULL DEFAULT 0;
    PRINT '已新增 ParkingFee 欄位至 Leases 資料表';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Leases]') AND name = 'ContractNo')
BEGIN
    ALTER TABLE [dbo].[Leases] ADD [ContractNo] NVARCHAR(50) NULL;
    PRINT '已新增 ContractNo 欄位至 Leases 資料表';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Leases]') AND name = 'ContractDate')
BEGIN
    ALTER TABLE [dbo].[Leases] ADD [ContractDate] DATETIME2 NULL;
    PRINT '已新增 ContractDate 欄位至 Leases 資料表';
END
GO

-- 2. 停車位資料 (ParkingSpaces)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ParkingSpaces]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ParkingSpaces] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ParkingSpaceId] NVARCHAR(50) NOT NULL, -- 停車位代碼 (PARKING_SPACE_ID)
        [ParkingSpaceNo] NVARCHAR(50) NULL, -- 停車位編號 (PARKING_SPACE_NO)
        [ShopId] NVARCHAR(50) NULL, -- 分店代碼 (SHOP_ID)
        [FloorId] NVARCHAR(50) NULL, -- 樓層代碼 (FLOOR_ID)
        [Area] DECIMAL(18, 4) NULL, -- 面積 (AREA)
        [Status] NVARCHAR(10) NULL DEFAULT 'A', -- 狀態 (STATUS, A:可用, U:使用中, M:維護中)
        [LeaseId] NVARCHAR(50) NULL, -- 租賃編號 (LEASE_ID)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_ParkingSpaces_ParkingSpaceId] UNIQUE ([ParkingSpaceId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_ParkingSpaces_ParkingSpaceId] ON [dbo].[ParkingSpaces] ([ParkingSpaceId]);
    CREATE NONCLUSTERED INDEX [IX_ParkingSpaces_ShopId] ON [dbo].[ParkingSpaces] ([ShopId]);
    CREATE NONCLUSTERED INDEX [IX_ParkingSpaces_Status] ON [dbo].[ParkingSpaces] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_ParkingSpaces_LeaseId] ON [dbo].[ParkingSpaces] ([LeaseId]);

    PRINT '資料表 ParkingSpaces 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 ParkingSpaces 已存在';
END
GO

-- 3. 租賃合同資料 (LeaseContracts)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseContracts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseContracts] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [ContractNo] NVARCHAR(50) NOT NULL, -- 合同編號 (CONTRACT_NO)
        [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
        [ContractDate] DATETIME2 NOT NULL, -- 合同日期 (CONTRACT_DATE)
        [ContractType] NVARCHAR(20) NULL, -- 合同類型 (CONTRACT_TYPE)
        [ContractContent] NVARCHAR(MAX) NULL, -- 合同內容 (CONTRACT_CONTENT)
        [Status] NVARCHAR(10) NULL DEFAULT 'A', -- 狀態 (STATUS, A:有效, I:無效)
        [SignedBy] NVARCHAR(50) NULL, -- 簽約人 (SIGNED_BY)
        [SignedDate] DATETIME2 NULL, -- 簽約日期 (SIGNED_DATE)
        [Memo] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LeaseContracts_ContractNo] UNIQUE ([ContractNo])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseContracts_ContractNo] ON [dbo].[LeaseContracts] ([ContractNo]);
    CREATE NONCLUSTERED INDEX [IX_LeaseContracts_LeaseId] ON [dbo].[LeaseContracts] ([LeaseId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseContracts_Status] ON [dbo].[LeaseContracts] ([Status]);

    PRINT '資料表 LeaseContracts 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseContracts 已存在';
END
GO

-- 4. 租賃報表查詢記錄 (LeaseReportQueries)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaseReportQueries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LeaseReportQueries] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [QueryId] NVARCHAR(50) NOT NULL, -- 查詢編號 (QUERY_ID)
        [ReportType] NVARCHAR(20) NOT NULL, -- 報表類型 (REPORT_TYPE, SYSM141:租賃清單, SYSM142:租賃明細, SYSM143:租賃統計, SYSM144:租賃分析)
        [QueryName] NVARCHAR(200) NULL, -- 查詢名稱 (QUERY_NAME)
        [QueryParams] NVARCHAR(MAX) NULL, -- 查詢參數 (JSON格式) (QUERY_PARAMS)
        [QueryResult] NVARCHAR(MAX) NULL, -- 查詢結果 (JSON格式) (QUERY_RESULT)
        [QueryDate] DATETIME2 NOT NULL, -- 查詢日期 (QUERY_DATE)
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [UQ_LeaseReportQueries_QueryId] UNIQUE ([QueryId])
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_LeaseReportQueries_QueryId] ON [dbo].[LeaseReportQueries] ([QueryId]);
    CREATE NONCLUSTERED INDEX [IX_LeaseReportQueries_ReportType] ON [dbo].[LeaseReportQueries] ([ReportType]);
    CREATE NONCLUSTERED INDEX [IX_LeaseReportQueries_QueryDate] ON [dbo].[LeaseReportQueries] ([QueryDate]);

    PRINT '資料表 LeaseReportQueries 建立成功';
END
ELSE
BEGIN
    PRINT '資料表 LeaseReportQueries 已存在';
END
GO


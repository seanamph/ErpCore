# SYSM111-SYSM138 - 租賃資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSM111-SYSM138 系列
- **功能名稱**: 租賃資料維護系列
- **功能描述**: 提供租賃資料的新增、修改、刪除、查詢功能，包含租賃編號、租戶、租賃日期、租金、租期、狀態、停車位資訊、合同相關資訊等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FS.ASP` (保存)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM111_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM121_FB.ASP` (停車位資訊瀏覽)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM131_FB.ASP` (合同相關功能瀏覽)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM135_FB.ASP` (相關功能瀏覽)

### 1.2 業務需求
- 管理租賃基本資料
- 支援租戶選擇
- 支援租賃期間管理
- 支援租金計算與調整
- 支援租賃狀態管理（草稿、已簽約、已生效、已終止）
- 支援停車位資訊管理
- 支援合同相關資訊管理
- 支援批量新增功能
- 支援租賃報表列印
- 支援租賃歷史記錄查詢
- 支援多店別管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Leases` (租賃主檔)

```sql
CREATE TABLE [dbo].[Leases] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
    [TenantId] NVARCHAR(50) NOT NULL, -- 租戶代碼 (TENANT_ID)
    [TenantName] NVARCHAR(200) NULL, -- 租戶名稱 (TENANT_NAME)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
    [FloorId] NVARCHAR(50) NULL, -- 樓層代碼 (FLOOR_ID)
    [LocationId] NVARCHAR(50) NULL, -- 位置代碼 (LOCATION_ID)
    [ParkingSpaceId] NVARCHAR(50) NULL, -- 停車位代碼 (PARKING_SPACE_ID)
    [LeaseDate] DATETIME2 NOT NULL, -- 租賃日期 (LEASE_DATE)
    [StartDate] DATETIME2 NOT NULL, -- 租期開始日 (START_DATE)
    [EndDate] DATETIME2 NULL, -- 租期結束日 (END_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已簽約, E:已生效, T:已終止)
    [MonthlyRent] DECIMAL(18, 4) NULL DEFAULT 0, -- 月租金 (MONTHLY_RENT)
    [TotalRent] DECIMAL(18, 4) NULL DEFAULT 0, -- 總租金 (TOTAL_RENT)
    [Deposit] DECIMAL(18, 4) NULL DEFAULT 0, -- 押金 (DEPOSIT)
    [ParkingFee] DECIMAL(18, 4) NULL DEFAULT 0, -- 停車費 (PARKING_FEE)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [PaymentMethod] NVARCHAR(20) NULL, -- 付款方式 (PAYMENT_METHOD)
    [PaymentDay] INT NULL, -- 付款日 (PAYMENT_DAY)
    [ContractNo] NVARCHAR(50) NULL, -- 合同編號 (CONTRACT_NO)
    [ContractDate] DATETIME2 NULL, -- 合同日期 (CONTRACT_DATE)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Leases] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Leases_LeaseId] UNIQUE ([LeaseId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Leases_LeaseId] ON [dbo].[Leases] ([LeaseId]);
CREATE NONCLUSTERED INDEX [IX_Leases_TenantId] ON [dbo].[Leases] ([TenantId]);
CREATE NONCLUSTERED INDEX [IX_Leases_ShopId] ON [dbo].[Leases] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_Leases_Status] ON [dbo].[Leases] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Leases_StartDate] ON [dbo].[Leases] ([StartDate]);
CREATE NONCLUSTERED INDEX [IX_Leases_EndDate] ON [dbo].[Leases] ([EndDate]);
CREATE NONCLUSTERED INDEX [IX_Leases_ParkingSpaceId] ON [dbo].[Leases] ([ParkingSpaceId]);
CREATE NONCLUSTERED INDEX [IX_Leases_ContractNo] ON [dbo].[Leases] ([ContractNo]);
```

### 2.2 相關資料表

#### 2.2.1 `LeaseDetails` - 租賃明細
```sql
CREATE TABLE [dbo].[LeaseDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [ItemType] NVARCHAR(20) NULL, -- 項目類型 (ITEM_TYPE, RENT:租金, PARKING:停車費, UTILITY:水電費, OTHER:其他)
    [ItemName] NVARCHAR(200) NULL, -- 項目名稱 (ITEM_NAME)
    [Amount] DECIMAL(18, 4) NULL DEFAULT 0, -- 金額 (AMOUNT)
    [StartDate] DATETIME2 NULL, -- 開始日期
    [EndDate] DATETIME2 NULL, -- 結束日期
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseDetails_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseDetails_LeaseId] ON [dbo].[LeaseDetails] ([LeaseId]);
```

#### 2.2.2 `ParkingSpaces` - 停車位資料
```sql
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
    CONSTRAINT [PK_ParkingSpaces] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_ParkingSpaces_ParkingSpaceId] UNIQUE ([ParkingSpaceId]),
    CONSTRAINT [FK_ParkingSpaces_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ParkingSpaces_ParkingSpaceId] ON [dbo].[ParkingSpaces] ([ParkingSpaceId]);
CREATE NONCLUSTERED INDEX [IX_ParkingSpaces_ShopId] ON [dbo].[ParkingSpaces] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_ParkingSpaces_Status] ON [dbo].[ParkingSpaces] ([Status]);
```

#### 2.2.3 `LeaseContracts` - 租賃合同資料
```sql
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
    CONSTRAINT [PK_LeaseContracts] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_LeaseContracts_ContractNo] UNIQUE ([ContractNo]),
    CONSTRAINT [FK_LeaseContracts_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseContracts_ContractNo] ON [dbo].[LeaseContracts] ([ContractNo]);
CREATE NONCLUSTERED INDEX [IX_LeaseContracts_LeaseId] ON [dbo].[LeaseContracts] ([LeaseId]);
```

### 2.3 資料字典

#### 2.3.1 Leases 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| LeaseId | NVARCHAR | 50 | NO | - | 租賃編號 | 唯一，LEASE_ID |
| TenantId | NVARCHAR | 50 | NO | - | 租戶代碼 | 外鍵至租戶表 |
| TenantName | NVARCHAR | 200 | YES | - | 租戶名稱 | - |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| FloorId | NVARCHAR | 50 | YES | - | 樓層代碼 | 外鍵至樓層表 |
| LocationId | NVARCHAR | 50 | YES | - | 位置代碼 | - |
| ParkingSpaceId | NVARCHAR | 50 | YES | - | 停車位代碼 | 外鍵至停車位表 |
| LeaseDate | DATETIME2 | - | NO | - | 租賃日期 | LEASE_DATE |
| StartDate | DATETIME2 | - | NO | - | 租期開始日 | START_DATE |
| EndDate | DATETIME2 | - | YES | - | 租期結束日 | END_DATE |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, S:已簽約, E:已生效, T:已終止 |
| MonthlyRent | DECIMAL | 18,4 | YES | 0 | 月租金 | MONTHLY_RENT |
| TotalRent | DECIMAL | 18,4 | YES | 0 | 總租金 | TOTAL_RENT |
| Deposit | DECIMAL | 18,4 | YES | 0 | 押金 | DEPOSIT |
| ParkingFee | DECIMAL | 18,4 | YES | 0 | 停車費 | PARKING_FEE |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| PaymentMethod | NVARCHAR | 20 | YES | - | 付款方式 | - |
| PaymentDay | INT | - | YES | - | 付款日 | - |
| ContractNo | NVARCHAR | 50 | YES | - | 合同編號 | 外鍵至合同表 |
| ContractDate | DATETIME2 | - | YES | - | 合同日期 | CONTRACT_DATE |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢租賃列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases`
- **說明**: 查詢租賃列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "LeaseId",
    "sortOrder": "ASC",
    "filters": {
      "leaseId": "",
      "tenantId": "",
      "shopId": "",
      "status": "",
      "startDateFrom": "",
      "startDateTo": ""
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "leaseId": "L001",
          "tenantId": "T001",
          "tenantName": "租戶名稱",
          "shopId": "SHOP001",
          "leaseDate": "2024-01-01",
          "startDate": "2024-01-01",
          "endDate": "2024-12-31",
          "status": "E",
          "monthlyRent": 50000,
          "totalRent": 600000,
          "deposit": 100000,
          "parkingFee": 2000
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆租賃
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases/{leaseId}`
- **說明**: 根據租賃編號查詢單筆租賃資料
- **路徑參數**:
  - `leaseId`: 租賃編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "leaseId": "L001",
      "tenantId": "T001",
      "tenantName": "租戶名稱",
      "shopId": "SHOP001",
      "floorId": "F001",
      "locationId": "LOC001",
      "parkingSpaceId": "P001",
      "leaseDate": "2024-01-01",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31",
      "status": "E",
      "monthlyRent": 50000,
      "totalRent": 600000,
      "deposit": 100000,
      "parkingFee": 2000,
      "contractNo": "C001",
      "contractDate": "2024-01-01",
      "memo": "備註"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增租賃
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases`
- **說明**: 新增租賃資料
- **請求格式**:
  ```json
  {
    "leaseId": "L001",
    "tenantId": "T001",
    "shopId": "SHOP001",
    "floorId": "F001",
    "locationId": "LOC001",
    "parkingSpaceId": "P001",
    "leaseDate": "2024-01-01",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "status": "D",
    "monthlyRent": 50000,
    "totalRent": 600000,
    "deposit": 100000,
    "parkingFee": 2000,
    "currencyId": "TWD",
    "paymentMethod": "MONTHLY",
    "paymentDay": 1,
    "contractNo": "C001",
    "contractDate": "2024-01-01",
    "memo": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "leaseId": "L001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改租賃
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/leases/{leaseId}`
- **說明**: 修改租賃資料
- **路徑參數**:
  - `leaseId`: 租賃編號
- **請求格式**: 同新增，但 `leaseId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除租賃
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/leases/{leaseId}`
- **說明**: 刪除租賃資料（軟刪除或硬刪除）
- **路徑參數**:
  - `leaseId`: 租賃編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 批次新增租賃
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/batch`
- **說明**: 批次新增多筆租賃資料
- **請求格式**:
  ```json
  {
    "leases": [
      {
        "leaseId": "L001",
        "tenantId": "T001",
        "shopId": "SHOP001",
        "startDate": "2024-01-01",
        "endDate": "2024-12-31",
        "monthlyRent": 50000
      },
      {
        "leaseId": "L002",
        "tenantId": "T002",
        "shopId": "SHOP001",
        "startDate": "2024-01-01",
        "endDate": "2024-12-31",
        "monthlyRent": 60000
      }
    ]
  }
  ```

#### 3.1.7 查詢停車位資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases/{leaseId}/parking-spaces`
- **說明**: 查詢租賃相關的停車位資訊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "parkingSpaces": [
        {
          "parkingSpaceId": "P001",
          "parkingSpaceNo": "A001",
          "area": 10.5,
          "status": "U"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 查詢合同資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases/{leaseId}/contracts`
- **說明**: 查詢租賃相關的合同資訊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "contracts": [
        {
          "contractNo": "C001",
          "contractDate": "2024-01-01",
          "contractType": "STANDARD",
          "status": "A"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `LeasesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/leases")]
    [Authorize]
    public class LeasesController : ControllerBase
    {
        private readonly ILeaseService _leaseService;
        
        public LeasesController(ILeaseService leaseService)
        {
            _leaseService = leaseService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<LeaseDto>>>> GetLeases([FromQuery] LeaseQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{leaseId}")]
        public async Task<ActionResult<ApiResponse<LeaseDto>>> GetLease(string leaseId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLease([FromBody] CreateLeaseDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse>> CreateLeasesBatch([FromBody] CreateLeasesBatchDto dto)
        {
            // 實作批次新增邏輯
        }
        
        [HttpPut("{leaseId}")]
        public async Task<ActionResult<ApiResponse>> UpdateLease(string leaseId, [FromBody] UpdateLeaseDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{leaseId}")]
        public async Task<ActionResult<ApiResponse>> DeleteLease(string leaseId)
        {
            // 實作刪除邏輯
        }
        
        [HttpGet("{leaseId}/parking-spaces")]
        public async Task<ActionResult<ApiResponse<ParkingSpaceDto>>> GetParkingSpaces(string leaseId)
        {
            // 實作查詢停車位邏輯
        }
        
        [HttpGet("{leaseId}/contracts")]
        public async Task<ActionResult<ApiResponse<LeaseContractDto>>> GetContracts(string leaseId)
        {
            // 實作查詢合同邏輯
        }
    }
}
```

#### 3.2.2 Service: `LeaseService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ILeaseService
    {
        Task<PagedResult<LeaseDto>> GetLeasesAsync(LeaseQueryDto query);
        Task<LeaseDto> GetLeaseByIdAsync(string leaseId);
        Task<string> CreateLeaseAsync(CreateLeaseDto dto);
        Task CreateLeasesBatchAsync(CreateLeasesBatchDto dto);
        Task UpdateLeaseAsync(string leaseId, UpdateLeaseDto dto);
        Task DeleteLeaseAsync(string leaseId);
        Task<ParkingSpaceDto> GetParkingSpacesAsync(string leaseId);
        Task<LeaseContractDto> GetContractsAsync(string leaseId);
    }
}
```

#### 3.2.3 Repository: `LeaseRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ILeaseRepository
    {
        Task<Lease> GetByIdAsync(string leaseId);
        Task<PagedResult<Lease>> GetPagedAsync(LeaseQuery query);
        Task<Lease> CreateAsync(Lease lease);
        Task<Lease> UpdateAsync(Lease lease);
        Task DeleteAsync(string leaseId);
        Task<bool> ExistsAsync(string leaseId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 租賃列表頁面 (`LeaseList.vue`)
- **路徑**: `/lease/leases`
- **功能**: 顯示租賃列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (LeaseSearchForm)
  - 資料表格 (LeaseDataTable)
  - 新增/修改對話框 (LeaseDialog)
  - 刪除確認對話框
  - 批量新增對話框

#### 4.1.2 租賃詳細頁面 (`LeaseDetail.vue`)
- **路徑**: `/lease/leases/:leaseId`
- **功能**: 顯示租賃詳細資料，支援修改
- **主要元件**:
  - 租賃基本資料表單
  - 停車位資訊列表
  - 合同資訊列表
  - 租賃明細列表

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`LeaseSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="租賃編號">
      <el-input v-model="searchForm.leaseId" placeholder="請輸入租賃編號" />
    </el-form-item>
    <el-form-item label="租戶">
      <el-input v-model="searchForm.tenantId" placeholder="請輸入租戶代碼" />
    </el-form-item>
    <el-form-item label="分店">
      <el-select v-model="searchForm.shopId" placeholder="請選擇分店">
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="草稿" value="D" />
        <el-option label="已簽約" value="S" />
        <el-option label="已生效" value="E" />
        <el-option label="已終止" value="T" />
      </el-select>
    </el-form-item>
    <el-form-item label="租期開始日">
      <el-date-picker v-model="searchForm.startDateFrom" type="date" placeholder="開始日期" />
    </el-form-item>
    <el-form-item label="至">
      <el-date-picker v-model="searchForm.startDateTo" type="date" placeholder="結束日期" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`LeaseDataTable.vue`)
```vue
<template>
  <div>
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">新增</el-button>
      <el-button type="success" @click="handleBatchAdd">批量新增</el-button>
      <el-button type="danger" @click="handleBatchDelete">批量刪除</el-button>
    </div>
    <el-table :data="leaseList" v-loading="loading">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="leaseId" label="租賃編號" width="120" />
      <el-table-column prop="tenantName" label="租戶名稱" width="150" />
      <el-table-column prop="shopName" label="分店" width="120" />
      <el-table-column prop="startDate" label="租期開始" width="120" />
      <el-table-column prop="endDate" label="租期結束" width="120" />
      <el-table-column prop="monthlyRent" label="月租金" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="info" size="small" @click="handleViewDetail(row)">詳情</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
  </div>
</template>
```

#### 4.2.3 新增/修改對話框 (`LeaseDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="1000px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="租賃編號" prop="leaseId">
            <el-input v-model="form.leaseId" :disabled="isEdit" placeholder="請輸入租賃編號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="租戶" prop="tenantId">
            <el-select v-model="form.tenantId" placeholder="請選擇租戶" filterable>
              <el-option v-for="tenant in tenantList" :key="tenant.tenantId" :label="tenant.tenantName" :value="tenant.tenantId" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="分店" prop="shopId">
            <el-select v-model="form.shopId" placeholder="請選擇分店">
              <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="樓層" prop="floorId">
            <el-select v-model="form.floorId" placeholder="請選擇樓層">
              <el-option v-for="floor in floorList" :key="floor.floorId" :label="floor.floorName" :value="floor.floorId" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="停車位" prop="parkingSpaceId">
            <el-select v-model="form.parkingSpaceId" placeholder="請選擇停車位" filterable>
              <el-option v-for="parking in parkingList" :key="parking.parkingSpaceId" :label="parking.parkingSpaceNo" :value="parking.parkingSpaceId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="租賃日期" prop="leaseDate">
            <el-date-picker v-model="form.leaseDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="租期開始日" prop="startDate">
            <el-date-picker v-model="form.startDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="租期結束日" prop="endDate">
            <el-date-picker v-model="form.endDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="月租金" prop="monthlyRent">
            <el-input-number v-model="form.monthlyRent" :min="0" :precision="2" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="押金" prop="deposit">
            <el-input-number v-model="form.deposit" :min="0" :precision="2" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="停車費" prop="parkingFee">
            <el-input-number v-model="form.parkingFee" :min="0" :precision="2" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="狀態" prop="status">
            <el-select v-model="form.status" placeholder="請選擇狀態">
              <el-option label="草稿" value="D" />
              <el-option label="已簽約" value="S" />
              <el-option label="已生效" value="E" />
              <el-option label="已終止" value="T" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="合同編號" prop="contractNo">
            <el-input v-model="form.contractNo" placeholder="請輸入合同編號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="合同日期" prop="contractDate">
            <el-date-picker v-model="form.contractDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="memo">
        <el-input v-model="form.memo" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`lease.api.ts`)
```typescript
import request from '@/utils/request';

export interface LeaseDto {
  leaseId: string;
  tenantId: string;
  tenantName?: string;
  shopId: string;
  floorId?: string;
  locationId?: string;
  parkingSpaceId?: string;
  leaseDate: string;
  startDate: string;
  endDate?: string;
  status: string;
  monthlyRent?: number;
  totalRent?: number;
  deposit?: number;
  parkingFee?: number;
  contractNo?: string;
  contractDate?: string;
  memo?: string;
}

export interface LeaseQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    leaseId?: string;
    tenantId?: string;
    shopId?: string;
    status?: string;
    startDateFrom?: string;
    startDateTo?: string;
  };
}

export interface CreateLeaseDto {
  leaseId: string;
  tenantId: string;
  shopId: string;
  floorId?: string;
  locationId?: string;
  parkingSpaceId?: string;
  leaseDate: string;
  startDate: string;
  endDate?: string;
  status: string;
  monthlyRent?: number;
  totalRent?: number;
  deposit?: number;
  parkingFee?: number;
  contractNo?: string;
  contractDate?: string;
  memo?: string;
}

export interface UpdateLeaseDto extends Omit<CreateLeaseDto, 'leaseId'> {}

// API 函數
export const getLeaseList = (query: LeaseQueryDto) => {
  return request.get<ApiResponse<PagedResult<LeaseDto>>>('/api/v1/leases', { params: query });
};

export const getLeaseById = (leaseId: string) => {
  return request.get<ApiResponse<LeaseDto>>(`/api/v1/leases/${leaseId}`);
};

export const createLease = (data: CreateLeaseDto) => {
  return request.post<ApiResponse<string>>('/api/v1/leases', data);
};

export const createLeasesBatch = (data: { leases: CreateLeaseDto[] }) => {
  return request.post<ApiResponse>('/api/v1/leases/batch', data);
};

export const updateLease = (leaseId: string, data: UpdateLeaseDto) => {
  return request.put<ApiResponse>(`/api/v1/leases/${leaseId}`, data);
};

export const deleteLease = (leaseId: string) => {
  return request.delete<ApiResponse>(`/api/v1/leases/${leaseId}`);
};

export const getParkingSpaces = (leaseId: string) => {
  return request.get<ApiResponse<ParkingSpaceDto>>(`/api/v1/leases/${leaseId}/parking-spaces`);
};

export const getContracts = (leaseId: string) => {
  return request.get<ApiResponse<LeaseContractDto>>(`/api/v1/leases/${leaseId}/contracts`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 批次新增功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 批量新增對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 停車位資訊顯示
- [ ] 合同資訊顯示
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 必須驗證使用者輸入

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 租賃編號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證（開始日期不能晚於結束日期）
- 狀態值必須在允許範圍內
- 租金金額必須大於等於0

### 6.4 業務邏輯
- 刪除租賃前必須檢查是否有相關資料（付款記錄、合同等）
- 終止租賃時必須檢查是否有未結清的款項
- 停車位必須檢查是否已被其他租賃使用
- 合同編號必須唯一
- 租期開始日期不能晚於結束日期

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增租賃成功
- [ ] 新增租賃失敗 (重複編號)
- [ ] 修改租賃成功
- [ ] 修改租賃失敗 (不存在)
- [ ] 刪除租賃成功
- [ ] 刪除租賃失敗 (有相關資料)
- [ ] 查詢租賃列表成功
- [ ] 查詢單筆租賃成功
- [ ] 批次新增租賃成功
- [ ] 批次新增租賃失敗 (部分資料錯誤)

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 停車位關聯測試
- [ ] 合同關聯測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM111_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM121_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM131_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM135_FB.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSM000/SYSM111.xsd` (如果存在)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


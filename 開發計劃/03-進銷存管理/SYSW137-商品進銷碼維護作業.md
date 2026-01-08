# SYSW137 - 商品進銷碼維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW137
- **功能名稱**: 商品進銷碼維護作業
- **功能描述**: 提供商品進銷碼的新增、修改、刪除、查詢功能，進銷碼是商品的唯一識別碼，用於進銷存系統中的商品管理
- **參考舊程式**: 
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/GOODS_M.cs` (商品主檔業務邏輯)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/GOODS_D.cs` (商品明細檔業務邏輯)
  - `WEB/IMS_CORE/INFO_PROG.xml` (功能定義)

### 1.2 業務需求
- 管理商品進銷碼（GOODS_ID）的建立與維護
- 進銷碼必須唯一，不可重複
- 支援進銷碼與商品名稱的對應關係
- 支援進銷碼與國際條碼（BARCODE_ID）的對應關係
- 支援多店別的商品進銷碼管理
- 支援進銷碼的啟用/停用狀態管理
- 支援進銷碼的歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Products` (對應舊系統 `GOODS_M`)

```sql
CREATE TABLE [dbo].[Products] (
    [GoodsId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 進銷碼 (GOODS_ID)
    [GoodsName] NVARCHAR(200) NOT NULL, -- 商品名稱 (GOODS_NAME)
    [InvPrintName] NVARCHAR(200) NULL, -- 發票列印名稱 (INV_PRINT_NAME)
    [GoodsSpace] NVARCHAR(100) NULL, -- 商品規格 (GOODS_SPACE)
    [ScId] NVARCHAR(50) NULL, -- 小分類代碼 (SC_ID)
    [Tax] NVARCHAR(10) NULL DEFAULT '1', -- 稅別 (TAX, 1:應稅, 0:免稅)
    [Lprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 進價 (LPRC)
    [Mprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 中價 (MPRC)
    [BarcodeId] NVARCHAR(50) NULL, -- 國際條碼 (BARCODE_ID)
    [Unit] NVARCHAR(20) NULL, -- 單位 (UNIT)
    [ConvertRate] INT NULL DEFAULT 1, -- 換算率 (CONVERT_RATE)
    [Capacity] INT NULL DEFAULT 0, -- 容量 (CAPACITY)
    [CapacityUnit] NVARCHAR(20) NULL, -- 容量單位 (CAPACITY_UNIT)
    [Status] NVARCHAR(10) NULL DEFAULT '1', -- 狀態 (STATUS, 1:正常, 2:停用)
    [Discount] NVARCHAR(1) NULL DEFAULT 'N', -- 可折扣 (DISCOUNT, Y/N)
    [AutoOrder] NVARCHAR(1) NULL DEFAULT 'N', -- 自動訂貨 (AUTO_ORDER, Y/N)
    [PriceKind] NVARCHAR(10) NULL DEFAULT '1', -- 價格種類 (PRICE_KIND)
    [CostKind] NVARCHAR(10) NULL DEFAULT '1', -- 成本種類 (COST_KIND)
    [SafeDays] INT NULL DEFAULT 0, -- 安全庫存天數 (SAFE_DAYS)
    [ExpirationDays] INT NULL DEFAULT 0, -- 有效期限天數 (EXPIRATION_DAYS)
    [National] NVARCHAR(50) NULL, -- 國別 (NATIONAL)
    [Place] NVARCHAR(100) NULL, -- 產地 (PLACE)
    [GoodsDeep] INT NULL DEFAULT 0, -- 商品-深(公分) (GOODS_DEEP)
    [GoodsWide] INT NULL DEFAULT 0, -- 商品-寬(公分) (GOODS_WIDE)
    [GoodsHigh] INT NULL DEFAULT 0, -- 商品-高(公分) (GOODS_HIGH)
    [PackDeep] INT NULL DEFAULT 0, -- 包裝-深(公分) (PACK_DEEP)
    [PackWide] INT NULL DEFAULT 0, -- 包裝-寬(公分) (PACK_WIDE)
    [PackHigh] INT NULL DEFAULT 0, -- 包裝-高(公分) (PACK_HIGH)
    [PackWeight] INT NULL DEFAULT 0, -- 包裝-重量(KG) (PACK_WEIGHT)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([GoodsId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Products_GoodsName] ON [dbo].[Products] ([GoodsName]);
CREATE NONCLUSTERED INDEX [IX_Products_BarcodeId] ON [dbo].[Products] ([BarcodeId]);
CREATE NONCLUSTERED INDEX [IX_Products_ScId] ON [dbo].[Products] ([ScId]);
CREATE NONCLUSTERED INDEX [IX_Products_Status] ON [dbo].[Products] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `ProductDetails` - 商品明細檔 (對應舊系統 `GOODS_D`)
```sql
CREATE TABLE [dbo].[ProductDetails] (
    [ShopId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SHOP_ID)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 進銷碼 (GOODS_ID)
    [SuppId] NVARCHAR(50) NULL, -- 供應商代碼 (SUPP_ID)
    [BarcodeId] NVARCHAR(50) NULL, -- 商品條碼 (BARCODE_ID)
    [Mprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 中價 (MPRC)
    [CanBuy] NVARCHAR(1) NULL DEFAULT 'Y', -- 可進貨 (CAN_BUY, Y/N)
    [CbDate] DATETIME2 NULL, -- 可進貨日期 (CB_DATE)
    [CanReturns] NVARCHAR(1) NULL DEFAULT 'Y', -- 可退貨 (CAN_RETURNS, Y/N)
    [CrDate] DATETIME2 NULL, -- 可退貨日期 (CR_DATE)
    [CanStoreIn] NVARCHAR(1) NULL DEFAULT 'Y', -- 可入庫 (CAN_STORE_IN, Y/N)
    [CsiDate] DATETIME2 NULL, -- 可入庫日期 (CSI_DATE)
    [CanStoreOut] NVARCHAR(1) NULL DEFAULT 'Y', -- 可出庫 (CAN_STORE_OUT, Y/N)
    [CsoDate] DATETIME2 NULL, -- 可出庫日期 (CSO_DATE)
    [CanSales] NVARCHAR(1) NULL DEFAULT 'Y', -- 可銷售 (CAN_SALES, Y/N)
    [CsDate] DATETIME2 NULL, -- 可銷售日期 (CS_DATE)
    [IsOver] NVARCHAR(1) NULL DEFAULT 'N', -- 是否超量 (IS_OVER, Y/N)
    [IoDate] DATETIME2 NULL, -- 超量日期 (IO_DATE)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ProductDetails] PRIMARY KEY CLUSTERED ([ShopId], [GoodsId]),
    CONSTRAINT [FK_ProductDetails_Products] FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductDetails_Shops] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[Shops] ([ShopId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ProductDetails_ShopId] ON [dbo].[ProductDetails] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_ProductDetails_GoodsId] ON [dbo].[ProductDetails] ([GoodsId]);
CREATE NONCLUSTERED INDEX [IX_ProductDetails_BarcodeId] ON [dbo].[ProductDetails] ([BarcodeId]);
```

#### 2.2.2 `ProductCategories` - 商品分類
```sql
-- 用於查詢商品分類列表
-- 參考商品分類設計
```

#### 2.2.3 `Shops` - 店別主檔
```sql
-- 用於查詢店別列表
-- 參考店別設計
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| GoodsId | NVARCHAR | 50 | NO | - | 進銷碼 | 主鍵，唯一識別碼 |
| GoodsName | NVARCHAR | 200 | NO | - | 商品名稱 | - |
| InvPrintName | NVARCHAR | 200 | YES | - | 發票列印名稱 | - |
| GoodsSpace | NVARCHAR | 100 | YES | - | 商品規格 | - |
| ScId | NVARCHAR | 50 | YES | - | 小分類代碼 | 外鍵至ProductCategories |
| Tax | NVARCHAR | 10 | YES | '1' | 稅別 | 1:應稅, 0:免稅 |
| Lprc | DECIMAL | 18,4 | YES | 0 | 進價 | - |
| Mprc | DECIMAL | 18,4 | YES | 0 | 中價 | - |
| BarcodeId | NVARCHAR | 50 | YES | - | 國際條碼 | - |
| Unit | NVARCHAR | 20 | YES | - | 單位 | - |
| ConvertRate | INT | - | YES | 1 | 換算率 | - |
| Capacity | INT | - | YES | 0 | 容量 | - |
| CapacityUnit | NVARCHAR | 20 | YES | - | 容量單位 | - |
| Status | NVARCHAR | 10 | YES | '1' | 狀態 | 1:正常, 2:停用 |
| Discount | NVARCHAR | 1 | YES | 'N' | 可折扣 | Y/N |
| AutoOrder | NVARCHAR | 1 | YES | 'N' | 自動訂貨 | Y/N |
| PriceKind | NVARCHAR | 10 | YES | '1' | 價格種類 | - |
| CostKind | NVARCHAR | 10 | YES | '1' | 成本種類 | - |
| SafeDays | INT | - | YES | 0 | 安全庫存天數 | - |
| ExpirationDays | INT | - | YES | 0 | 有效期限天數 | - |
| National | NVARCHAR | 50 | YES | - | 國別 | - |
| Place | NVARCHAR | 100 | YES | - | 產地 | - |
| GoodsDeep | INT | - | YES | 0 | 商品-深(公分) | - |
| GoodsWide | INT | - | YES | 0 | 商品-寬(公分) | - |
| GoodsHigh | INT | - | YES | 0 | 商品-高(公分) | - |
| PackDeep | INT | - | YES | 0 | 包裝-深(公分) | - |
| PackWide | INT | - | YES | 0 | 包裝-寬(公分) | - |
| PackHigh | INT | - | YES | 0 | 包裝-高(公分) | - |
| PackWeight | INT | - | YES | 0 | 包裝-重量(KG) | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢進銷碼列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/products/goods-ids`
- **說明**: 查詢商品進銷碼列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "GoodsId",
    "sortOrder": "ASC",
    "filters": {
      "goodsId": "",
      "goodsName": "",
      "barcodeId": "",
      "scId": "",
      "status": "",
      "shopId": ""
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
          "goodsId": "G001",
          "goodsName": "商品名稱",
          "barcodeId": "1234567890123",
          "scId": "SC001",
          "scName": "小分類名稱",
          "status": "1",
          "statusName": "正常",
          "lprc": 100.00,
          "mprc": 150.00,
          "unit": "PCS",
          "createdAt": "2024-01-01T10:00:00",
          "updatedAt": "2024-01-01T10:00:00"
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

#### 3.1.2 查詢單筆進銷碼
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/products/goods-ids/{goodsId}`
- **說明**: 根據進銷碼查詢單筆商品資料
- **路徑參數**:
  - `goodsId`: 進銷碼
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "goodsId": "G001",
      "goodsName": "商品名稱",
      "invPrintName": "發票列印名稱",
      "goodsSpace": "規格",
      "scId": "SC001",
      "scName": "小分類名稱",
      "tax": "1",
      "taxName": "應稅",
      "lprc": 100.00,
      "mprc": 150.00,
      "barcodeId": "1234567890123",
      "unit": "PCS",
      "convertRate": 1,
      "capacity": 0,
      "capacityUnit": "",
      "status": "1",
      "statusName": "正常",
      "discount": "N",
      "autoOrder": "N",
      "priceKind": "1",
      "costKind": "1",
      "safeDays": 0,
      "expirationDays": 0,
      "national": "",
      "place": "",
      "goodsDeep": 0,
      "goodsWide": 0,
      "goodsHigh": 0,
      "packDeep": 0,
      "packWide": 0,
      "packHigh": 0,
      "packWeight": 0,
      "createdBy": "U001",
      "createdAt": "2024-01-01T10:00:00",
      "updatedBy": "U001",
      "updatedAt": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增進銷碼
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/products/goods-ids`
- **說明**: 新增商品進銷碼資料
- **請求格式**:
  ```json
  {
    "goodsId": "G001",
    "goodsName": "商品名稱",
    "invPrintName": "發票列印名稱",
    "goodsSpace": "規格",
    "scId": "SC001",
    "tax": "1",
    "lprc": 100.00,
    "mprc": 150.00,
    "barcodeId": "1234567890123",
    "unit": "PCS",
    "convertRate": 1,
    "capacity": 0,
    "capacityUnit": "",
    "status": "1",
    "discount": "N",
    "autoOrder": "N",
    "priceKind": "1",
    "costKind": "1",
    "safeDays": 0,
    "expirationDays": 0,
    "national": "",
    "place": "",
    "goodsDeep": 0,
    "goodsWide": 0,
    "goodsHigh": 0,
    "packDeep": 0,
    "packWide": 0,
    "packHigh": 0,
    "packWeight": 0
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "goodsId": "G001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改進銷碼
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/products/goods-ids/{goodsId}`
- **說明**: 修改商品進銷碼資料（進銷碼不可修改）
- **路徑參數**:
  - `goodsId`: 進銷碼
- **請求格式**: 同新增，但 `goodsId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除進銷碼
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/products/goods-ids/{goodsId}`
- **說明**: 刪除商品進銷碼資料（軟刪除或硬刪除）
- **路徑參數**:
  - `goodsId`: 進銷碼
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

#### 3.1.6 批次刪除進銷碼
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/products/goods-ids/batch`
- **說明**: 批次刪除多筆進銷碼
- **請求格式**:
  ```json
  {
    "goodsIds": ["G001", "G002", "G003"]
  }
  ```

#### 3.1.7 檢查進銷碼是否存在
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/products/goods-ids/{goodsId}/exists`
- **說明**: 檢查進銷碼是否已存在
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "exists": true
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 啟用/停用進銷碼
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/products/goods-ids/{goodsId}/status`
- **說明**: 啟用或停用進銷碼
- **請求格式**:
  ```json
  {
    "status": "1" // 1:正常, 2:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `ProductGoodsIdController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products/goods-ids")]
    [Authorize]
    public class ProductGoodsIdController : ControllerBase
    {
        private readonly IProductGoodsIdService _productGoodsIdService;
        
        public ProductGoodsIdController(IProductGoodsIdService productGoodsIdService)
        {
            _productGoodsIdService = productGoodsIdService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductGoodsIdDto>>>> GetProductGoodsIds([FromQuery] ProductGoodsIdQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{goodsId}")]
        public async Task<ActionResult<ApiResponse<ProductGoodsIdDto>>> GetProductGoodsId(string goodsId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateProductGoodsId([FromBody] CreateProductGoodsIdDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{goodsId}")]
        public async Task<ActionResult<ApiResponse>> UpdateProductGoodsId(string goodsId, [FromBody] UpdateProductGoodsIdDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{goodsId}")]
        public async Task<ActionResult<ApiResponse>> DeleteProductGoodsId(string goodsId)
        {
            // 實作刪除邏輯
        }
        
        [HttpGet("{goodsId}/exists")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckProductGoodsIdExists(string goodsId)
        {
            // 實作檢查邏輯
        }
        
        [HttpPut("{goodsId}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateProductGoodsIdStatus(string goodsId, [FromBody] UpdateStatusDto dto)
        {
            // 實作狀態更新邏輯
        }
    }
}
```

#### 3.2.2 Service: `ProductGoodsIdService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IProductGoodsIdService
    {
        Task<PagedResult<ProductGoodsIdDto>> GetProductGoodsIdsAsync(ProductGoodsIdQueryDto query);
        Task<ProductGoodsIdDto> GetProductGoodsIdByIdAsync(string goodsId);
        Task<string> CreateProductGoodsIdAsync(CreateProductGoodsIdDto dto);
        Task UpdateProductGoodsIdAsync(string goodsId, UpdateProductGoodsIdDto dto);
        Task DeleteProductGoodsIdAsync(string goodsId);
        Task<bool> ExistsAsync(string goodsId);
        Task UpdateStatusAsync(string goodsId, string status);
    }
}
```

#### 3.2.3 Repository: `ProductGoodsIdRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IProductGoodsIdRepository
    {
        Task<Product> GetByIdAsync(string goodsId);
        Task<PagedResult<Product>> GetPagedAsync(ProductGoodsIdQuery query);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(string goodsId);
        Task<bool> ExistsAsync(string goodsId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 進銷碼列表頁面 (`ProductGoodsIdList.vue`)
- **路徑**: `/inventory/products/goods-ids`
- **功能**: 顯示進銷碼列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (ProductGoodsIdSearchForm)
  - 資料表格 (ProductGoodsIdDataTable)
  - 新增/修改對話框 (ProductGoodsIdDialog)
  - 刪除確認對話框

#### 4.1.2 進銷碼詳細頁面 (`ProductGoodsIdDetail.vue`)
- **路徑**: `/inventory/products/goods-ids/:goodsId`
- **功能**: 顯示進銷碼詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ProductGoodsIdSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="進銷碼">
      <el-input v-model="searchForm.goodsId" placeholder="請輸入進銷碼" />
    </el-form-item>
    <el-form-item label="商品名稱">
      <el-input v-model="searchForm.goodsName" placeholder="請輸入商品名稱" />
    </el-form-item>
    <el-form-item label="國際條碼">
      <el-input v-model="searchForm.barcodeId" placeholder="請輸入國際條碼" />
    </el-form-item>
    <el-form-item label="小分類">
      <el-select v-model="searchForm.scId" placeholder="請選擇小分類">
        <el-option v-for="category in categoryList" :key="category.scId" :label="category.scName" :value="category.scId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="正常" value="1" />
        <el-option label="停用" value="2" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`ProductGoodsIdDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="goodsIdList" v-loading="loading">
      <el-table-column prop="goodsId" label="進銷碼" width="120" />
      <el-table-column prop="goodsName" label="商品名稱" width="200" />
      <el-table-column prop="barcodeId" label="國際條碼" width="150" />
      <el-table-column prop="scName" label="小分類" width="120" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="lprc" label="進價" width="100" align="right" />
      <el-table-column prop="mprc" label="中價" width="100" align="right" />
      <el-table-column prop="unit" label="單位" width="80" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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

#### 4.2.3 新增/修改對話框 (`ProductGoodsIdDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="進銷碼" prop="goodsId">
            <el-input v-model="form.goodsId" :disabled="isEdit" placeholder="請輸入進銷碼" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="商品名稱" prop="goodsName">
            <el-input v-model="form.goodsName" placeholder="請輸入商品名稱" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="發票列印名稱" prop="invPrintName">
            <el-input v-model="form.invPrintName" placeholder="請輸入發票列印名稱" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="商品規格" prop="goodsSpace">
            <el-input v-model="form.goodsSpace" placeholder="請輸入商品規格" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="小分類" prop="scId">
            <el-select v-model="form.scId" placeholder="請選擇小分類">
              <el-option v-for="category in categoryList" :key="category.scId" :label="category.scName" :value="category.scId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="稅別" prop="tax">
            <el-select v-model="form.tax" placeholder="請選擇稅別">
              <el-option label="應稅" value="1" />
              <el-option label="免稅" value="0" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="進價" prop="lprc">
            <el-input-number v-model="form.lprc" :precision="2" :min="0" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="中價" prop="mprc">
            <el-input-number v-model="form.mprc" :precision="2" :min="0" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="國際條碼" prop="barcodeId">
            <el-input v-model="form.barcodeId" placeholder="請輸入國際條碼" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="單位" prop="unit">
            <el-input v-model="form.unit" placeholder="請輸入單位" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="換算率" prop="convertRate">
            <el-input-number v-model="form.convertRate" :min="1" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="狀態" prop="status">
            <el-select v-model="form.status" placeholder="請選擇狀態">
              <el-option label="正常" value="1" />
              <el-option label="停用" value="2" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="可折扣" prop="discount">
            <el-radio-group v-model="form.discount">
              <el-radio label="Y">是</el-radio>
              <el-radio label="N">否</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="自動訂貨" prop="autoOrder">
            <el-radio-group v-model="form.autoOrder">
              <el-radio label="Y">是</el-radio>
              <el-radio label="N">否</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`productGoodsId.api.ts`)
```typescript
import request from '@/utils/request';

export interface ProductGoodsIdDto {
  goodsId: string;
  goodsName: string;
  invPrintName?: string;
  goodsSpace?: string;
  scId?: string;
  scName?: string;
  tax?: string;
  taxName?: string;
  lprc?: number;
  mprc?: number;
  barcodeId?: string;
  unit?: string;
  convertRate?: number;
  capacity?: number;
  capacityUnit?: string;
  status: string;
  statusName?: string;
  discount?: string;
  autoOrder?: string;
  priceKind?: string;
  costKind?: string;
  safeDays?: number;
  expirationDays?: number;
  national?: string;
  place?: string;
  goodsDeep?: number;
  goodsWide?: number;
  goodsHigh?: number;
  packDeep?: number;
  packWide?: number;
  packHigh?: number;
  packWeight?: number;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
}

export interface ProductGoodsIdQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    goodsId?: string;
    goodsName?: string;
    barcodeId?: string;
    scId?: string;
    status?: string;
    shopId?: string;
  };
}

export interface CreateProductGoodsIdDto {
  goodsId: string;
  goodsName: string;
  invPrintName?: string;
  goodsSpace?: string;
  scId?: string;
  tax?: string;
  lprc?: number;
  mprc?: number;
  barcodeId?: string;
  unit?: string;
  convertRate?: number;
  capacity?: number;
  capacityUnit?: string;
  status: string;
  discount?: string;
  autoOrder?: string;
  priceKind?: string;
  costKind?: string;
  safeDays?: number;
  expirationDays?: number;
  national?: string;
  place?: string;
  goodsDeep?: number;
  goodsWide?: number;
  goodsHigh?: number;
  packDeep?: number;
  packWide?: number;
  packHigh?: number;
  packWeight?: number;
}

export interface UpdateProductGoodsIdDto extends Omit<CreateProductGoodsIdDto, 'goodsId'> {}

// API 函數
export const getProductGoodsIdList = (query: ProductGoodsIdQueryDto) => {
  return request.get<ApiResponse<PagedResult<ProductGoodsIdDto>>>('/api/v1/products/goods-ids', { params: query });
};

export const getProductGoodsIdById = (goodsId: string) => {
  return request.get<ApiResponse<ProductGoodsIdDto>>(`/api/v1/products/goods-ids/${goodsId}`);
};

export const createProductGoodsId = (data: CreateProductGoodsIdDto) => {
  return request.post<ApiResponse<string>>('/api/v1/products/goods-ids', data);
};

export const updateProductGoodsId = (goodsId: string, data: UpdateProductGoodsIdDto) => {
  return request.put<ApiResponse>(`/api/v1/products/goods-ids/${goodsId}`, data);
};

export const deleteProductGoodsId = (goodsId: string) => {
  return request.delete<ApiResponse>(`/api/v1/products/goods-ids/${goodsId}`);
};

export const checkProductGoodsIdExists = (goodsId: string) => {
  return request.get<ApiResponse<{ exists: boolean }>>(`/api/v1/products/goods-ids/${goodsId}/exists`);
};

export const updateProductGoodsIdStatus = (goodsId: string, status: string) => {
  return request.put<ApiResponse>(`/api/v1/products/goods-ids/${goodsId}/status`, { status });
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

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
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

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 進銷碼必須唯一，不可重複
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 刪除前必須檢查是否有相關資料

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制
- 進銷碼查詢必須優化

### 6.3 資料驗證
- 進銷碼必須符合格式要求
- 必填欄位必須驗證
- 價格必須大於等於0
- 狀態值必須在允許範圍內
- 進銷碼長度限制

### 6.4 業務邏輯
- 刪除進銷碼前必須檢查是否有相關商品明細資料
- 停用進銷碼時必須檢查是否有進行中的業務
- 進銷碼一旦建立，原則上不可修改（除非特殊情況）
- 必須記錄進銷碼的建立與修改歷史

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增進銷碼成功
- [ ] 新增進銷碼失敗 (重複編號)
- [ ] 修改進銷碼成功
- [ ] 修改進銷碼失敗 (不存在)
- [ ] 刪除進銷碼成功
- [ ] 刪除進銷碼失敗 (有相關資料)
- [ ] 查詢進銷碼列表成功
- [ ] 查詢單筆進銷碼成功
- [ ] 檢查進銷碼是否存在

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 進銷碼唯一性檢查測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 進銷碼查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/GOODS_M.cs` (商品主檔業務邏輯)
- `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/GOODS_D.cs` (商品明細檔業務邏輯)
- `WEB/IMS_CORE/INFO_PROG.xml` (功能定義)

### 8.2 資料庫 Schema
- 商品主檔 (GOODS_M)
- 商品明細檔 (GOODS_D)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


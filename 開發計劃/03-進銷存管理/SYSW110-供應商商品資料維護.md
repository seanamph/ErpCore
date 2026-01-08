# SYSW110 - 供應商商品資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW110
- **功能名稱**: 供應商商品資料維護
- **功能描述**: 提供供應商商品資料的新增、修改、刪除、查詢功能，包含供應商編號、商品條碼、店別、價格、稅別、數量等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW110_PR.ASP` (報表)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/SUPP_GOODS.cs` (業務邏輯)

### 1.2 業務需求
- 管理供應商與商品的對應關係
- 支援供應商商品價格設定（進價、中價）
- 支援供應商商品稅別設定
- 支援最小/最大訂購數量設定
- 支援商品單位與換算率設定
- 支援供應商商品狀態管理（啟用/停用）
- 支援供應商商品有效期間設定
- 支援供應商商品訂購日期設定（週一到週日）
- 支援到貨天數設定
- 支援多店別管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SupplierGoods` (對應舊系統 `SUPP_GOODS`)

```sql
CREATE TABLE [dbo].[SupplierGoods] (
    [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPP_ID)
    [BarcodeId] NVARCHAR(50) NOT NULL, -- 商品條碼 (BARCODE_ID)
    [ShopId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SHOP_ID)
    [Lprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 進價 (LPRC)
    [Mprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 中價 (MPRC)
    [Tax] NVARCHAR(10) NULL DEFAULT '1', -- 稅別 (TAX, DEFAULT 1)
    [MinQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 最小訂購量 (MINQTY)
    [MaxQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 最大訂購量 (MAXQTY)
    [Unit] NVARCHAR(20) NULL, -- 商品單位 (UNIT)
    [Rate] DECIMAL(18, 4) NULL DEFAULT 1, -- 換算率 (RATE, DEFAULT 1)
    [Status] NVARCHAR(10) NULL DEFAULT '0', -- 狀態 (STATUS, 0:正常 1:停用)
    [StartDate] DATETIME2 NULL, -- 有效起始日 (SSDATE)
    [EndDate] DATETIME2 NULL, -- 有效終止日 (SEDATE)
    [Slprc] DECIMAL(18, 4) NULL DEFAULT 0, -- 促銷價格 (SLPRC)
    [ArrivalDays] INT NULL DEFAULT 0, -- 到貨天數 (ARRIVAL_DAYS)
    [OrdDay1] NVARCHAR(1) NULL DEFAULT 'Y', -- 週一可訂購 (ORDDAY1, Y/N)
    [OrdDay2] NVARCHAR(1) NULL DEFAULT 'Y', -- 週二可訂購 (ORDDAY2, Y/N)
    [OrdDay3] NVARCHAR(1) NULL DEFAULT 'Y', -- 週三可訂購 (ORDDAY3, Y/N)
    [OrdDay4] NVARCHAR(1) NULL DEFAULT 'Y', -- 週四可訂購 (ORDDAY4, Y/N)
    [OrdDay5] NVARCHAR(1) NULL DEFAULT 'Y', -- 週五可訂購 (ORDDAY5, Y/N)
    [OrdDay6] NVARCHAR(1) NULL DEFAULT 'Y', -- 週六可訂購 (ORDDAY6, Y/N)
    [OrdDay7] NVARCHAR(1) NULL DEFAULT 'Y', -- 週日可訂購 (ORDDAY7, Y/N)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_SupplierGoods] PRIMARY KEY CLUSTERED ([SupplierId], [BarcodeId], [ShopId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SupplierGoods_SupplierId] ON [dbo].[SupplierGoods] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_SupplierGoods_BarcodeId] ON [dbo].[SupplierGoods] ([BarcodeId]);
CREATE NONCLUSTERED INDEX [IX_SupplierGoods_ShopId] ON [dbo].[SupplierGoods] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_SupplierGoods_Status] ON [dbo].[SupplierGoods] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SupplierGoods_StartDate_EndDate] ON [dbo].[SupplierGoods] ([StartDate], [EndDate]);
```

### 2.2 相關資料表

#### 2.2.1 `Vendors` - 供應商主檔
```sql
-- 用於查詢供應商列表
-- 參考: `開發計劃/02-基本資料管理/05-廠商客戶/SYSB206-廠客基本資料維護作業.md`
```

#### 2.2.2 `Products` - 商品主檔
```sql
-- 用於查詢商品列表
-- 參考商品主檔設計
```

#### 2.2.3 `ProductBarcodes` - 商品條碼主檔
```sql
-- 用於查詢商品條碼列表
-- 參考商品條碼設計
```

#### 2.2.4 `Shops` - 店別主檔
```sql
-- 用於查詢店別列表
-- 參考店別設計
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| SupplierId | NVARCHAR | 50 | NO | - | 供應商編號 | 主鍵，外鍵至Vendors |
| BarcodeId | NVARCHAR | 50 | NO | - | 商品條碼 | 主鍵，外鍵至ProductBarcodes |
| ShopId | NVARCHAR | 50 | NO | - | 店別代碼 | 主鍵，外鍵至Shops |
| Lprc | DECIMAL | 18,4 | YES | 0 | 進價 | - |
| Mprc | DECIMAL | 18,4 | YES | 0 | 中價 | - |
| Tax | NVARCHAR | 10 | YES | '1' | 稅別 | 1:應稅, 0:免稅 |
| MinQty | DECIMAL | 18,4 | YES | 0 | 最小訂購量 | - |
| MaxQty | DECIMAL | 18,4 | YES | 0 | 最大訂購量 | - |
| Unit | NVARCHAR | 20 | YES | - | 商品單位 | - |
| Rate | DECIMAL | 18,4 | YES | 1 | 換算率 | - |
| Status | NVARCHAR | 10 | YES | '0' | 狀態 | 0:正常, 1:停用 |
| StartDate | DATETIME2 | - | YES | - | 有效起始日 | - |
| EndDate | DATETIME2 | - | YES | - | 有效終止日 | - |
| Slprc | DECIMAL | 18,4 | YES | 0 | 促銷價格 | - |
| ArrivalDays | INT | - | YES | 0 | 到貨天數 | - |
| OrdDay1 | NVARCHAR | 1 | YES | 'Y' | 週一可訂購 | Y/N |
| OrdDay2 | NVARCHAR | 1 | YES | 'Y' | 週二可訂購 | Y/N |
| OrdDay3 | NVARCHAR | 1 | YES | 'Y' | 週三可訂購 | Y/N |
| OrdDay4 | NVARCHAR | 1 | YES | 'Y' | 週四可訂購 | Y/N |
| OrdDay5 | NVARCHAR | 1 | YES | 'Y' | 週五可訂購 | Y/N |
| OrdDay6 | NVARCHAR | 1 | YES | 'Y' | 週六可訂購 | Y/N |
| OrdDay7 | NVARCHAR | 1 | YES | 'Y' | 週日可訂購 | Y/N |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢供應商商品列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/supplier-goods`
- **說明**: 查詢供應商商品列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "SupplierId",
    "sortOrder": "ASC",
    "filters": {
      "supplierId": "",
      "barcodeId": "",
      "shopId": "",
      "status": ""
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
          "supplierId": "V001",
          "supplierName": "供應商A",
          "barcodeId": "BC001",
          "barcodeName": "商品條碼A",
          "shopId": "SHOP001",
          "shopName": "店別A",
          "lprc": 100.00,
          "mprc": 120.00,
          "tax": "1",
          "minQty": 10,
          "maxQty": 1000,
          "unit": "PCS",
          "rate": 1,
          "status": "0",
          "startDate": "2024-01-01",
          "endDate": "2024-12-31",
          "slprc": 0,
          "arrivalDays": 3,
          "ordDay1": "Y",
          "ordDay2": "Y",
          "ordDay3": "Y",
          "ordDay4": "Y",
          "ordDay5": "Y",
          "ordDay6": "Y",
          "ordDay7": "Y"
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

#### 3.1.2 查詢單筆供應商商品
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/supplier-goods/{supplierId}/{barcodeId}/{shopId}`
- **說明**: 根據供應商編號、商品條碼、店別查詢單筆供應商商品資料
- **路徑參數**:
  - `supplierId`: 供應商編號
  - `barcodeId`: 商品條碼
  - `shopId`: 店別代碼
- **回應格式**: 同查詢列表單筆資料格式

#### 3.1.3 新增供應商商品
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/supplier-goods`
- **說明**: 新增供應商商品資料
- **請求格式**:
  ```json
  {
    "supplierId": "V001",
    "barcodeId": "BC001",
    "shopId": "SHOP001",
    "lprc": 100.00,
    "mprc": 120.00,
    "tax": "1",
    "minQty": 10,
    "maxQty": 1000,
    "unit": "PCS",
    "rate": 1,
    "status": "0",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "slprc": 0,
    "arrivalDays": 3,
    "ordDay1": "Y",
    "ordDay2": "Y",
    "ordDay3": "Y",
    "ordDay4": "Y",
    "ordDay5": "Y",
    "ordDay6": "Y",
    "ordDay7": "Y"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "supplierId": "V001",
      "barcodeId": "BC001",
      "shopId": "SHOP001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改供應商商品
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/supplier-goods/{supplierId}/{barcodeId}/{shopId}`
- **說明**: 修改供應商商品資料
- **路徑參數**:
  - `supplierId`: 供應商編號
  - `barcodeId`: 商品條碼
  - `shopId`: 店別代碼
- **請求格式**: 同新增，但主鍵欄位不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除供應商商品
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/supplier-goods/{supplierId}/{barcodeId}/{shopId}`
- **說明**: 刪除供應商商品資料（軟刪除或硬刪除）
- **路徑參數**:
  - `supplierId`: 供應商編號
  - `barcodeId`: 商品條碼
  - `shopId`: 店別代碼
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

#### 3.1.6 批次刪除供應商商品
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/supplier-goods/batch`
- **說明**: 批次刪除多筆供應商商品
- **請求格式**:
  ```json
  {
    "items": [
      {
        "supplierId": "V001",
        "barcodeId": "BC001",
        "shopId": "SHOP001"
      },
      {
        "supplierId": "V001",
        "barcodeId": "BC002",
        "shopId": "SHOP001"
      }
    ]
  }
  ```

#### 3.1.7 啟用/停用供應商商品
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/supplier-goods/{supplierId}/{barcodeId}/{shopId}/status`
- **說明**: 啟用或停用供應商商品
- **請求格式**:
  ```json
  {
    "status": "0" // 0:正常, 1:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `SupplierGoodsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/supplier-goods")]
    [Authorize]
    public class SupplierGoodsController : ControllerBase
    {
        private readonly ISupplierGoodsService _supplierGoodsService;
        
        public SupplierGoodsController(ISupplierGoodsService supplierGoodsService)
        {
            _supplierGoodsService = supplierGoodsService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<SupplierGoodsDto>>>> GetSupplierGoods([FromQuery] SupplierGoodsQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{supplierId}/{barcodeId}/{shopId}")]
        public async Task<ActionResult<ApiResponse<SupplierGoodsDto>>> GetSupplierGoods(string supplierId, string barcodeId, string shopId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateSupplierGoods([FromBody] CreateSupplierGoodsDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{supplierId}/{barcodeId}/{shopId}")]
        public async Task<ActionResult<ApiResponse>> UpdateSupplierGoods(string supplierId, string barcodeId, string shopId, [FromBody] UpdateSupplierGoodsDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{supplierId}/{barcodeId}/{shopId}")]
        public async Task<ActionResult<ApiResponse>> DeleteSupplierGoods(string supplierId, string barcodeId, string shopId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `SupplierGoodsService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ISupplierGoodsService
    {
        Task<PagedResult<SupplierGoodsDto>> GetSupplierGoodsAsync(SupplierGoodsQueryDto query);
        Task<SupplierGoodsDto> GetSupplierGoodsByIdAsync(string supplierId, string barcodeId, string shopId);
        Task CreateSupplierGoodsAsync(CreateSupplierGoodsDto dto);
        Task UpdateSupplierGoodsAsync(string supplierId, string barcodeId, string shopId, UpdateSupplierGoodsDto dto);
        Task DeleteSupplierGoodsAsync(string supplierId, string barcodeId, string shopId);
        Task UpdateStatusAsync(string supplierId, string barcodeId, string shopId, string status);
    }
}
```

#### 3.2.3 Repository: `SupplierGoodsRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ISupplierGoodsRepository
    {
        Task<SupplierGoods> GetByIdAsync(string supplierId, string barcodeId, string shopId);
        Task<PagedResult<SupplierGoods>> GetPagedAsync(SupplierGoodsQuery query);
        Task<SupplierGoods> CreateAsync(SupplierGoods supplierGoods);
        Task<SupplierGoods> UpdateAsync(SupplierGoods supplierGoods);
        Task DeleteAsync(string supplierId, string barcodeId, string shopId);
        Task<bool> ExistsAsync(string supplierId, string barcodeId, string shopId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 供應商商品列表頁面 (`SupplierGoodsList.vue`)
- **路徑**: `/inventory/supplier-goods`
- **功能**: 顯示供應商商品列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (SupplierGoodsSearchForm)
  - 資料表格 (SupplierGoodsDataTable)
  - 新增/修改對話框 (SupplierGoodsDialog)
  - 刪除確認對話框

#### 4.1.2 供應商商品詳細頁面 (`SupplierGoodsDetail.vue`)
- **路徑**: `/inventory/supplier-goods/:supplierId/:barcodeId/:shopId`
- **功能**: 顯示供應商商品詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SupplierGoodsSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="供應商編號">
      <el-input v-model="searchForm.supplierId" placeholder="請輸入供應商編號" />
    </el-form-item>
    <el-form-item label="商品條碼">
      <el-input v-model="searchForm.barcodeId" placeholder="請輸入商品條碼" />
    </el-form-item>
    <el-form-item label="店別">
      <el-select v-model="searchForm.shopId" placeholder="請選擇店別">
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="正常" value="0" />
        <el-option label="停用" value="1" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`SupplierGoodsDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="supplierGoodsList" v-loading="loading">
      <el-table-column prop="supplierId" label="供應商編號" width="120" />
      <el-table-column prop="supplierName" label="供應商名稱" width="150" />
      <el-table-column prop="barcodeId" label="商品條碼" width="120" />
      <el-table-column prop="barcodeName" label="商品名稱" width="150" />
      <el-table-column prop="shopId" label="店別" width="100" />
      <el-table-column prop="lprc" label="進價" width="100" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.lprc) }}
        </template>
      </el-table-column>
      <el-table-column prop="mprc" label="中價" width="100" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.mprc) }}
        </template>
      </el-table-column>
      <el-table-column prop="tax" label="稅別" width="80">
        <template #default="{ row }">
          {{ row.tax === '1' ? '應稅' : '免稅' }}
        </template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '0' ? 'success' : 'danger'">
            {{ row.status === '0' ? '正常' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
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

#### 4.2.3 新增/修改對話框 (`SupplierGoodsDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="供應商編號" prop="supplierId">
            <el-select v-model="form.supplierId" placeholder="請選擇供應商" :disabled="isEdit" filterable>
              <el-option v-for="supplier in supplierList" :key="supplier.supplierId" :label="supplier.supplierName" :value="supplier.supplierId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="商品條碼" prop="barcodeId">
            <el-select v-model="form.barcodeId" placeholder="請選擇商品條碼" :disabled="isEdit" filterable>
              <el-option v-for="barcode in barcodeList" :key="barcode.barcodeId" :label="barcode.barcodeName" :value="barcode.barcodeId" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="店別" prop="shopId">
            <el-select v-model="form.shopId" placeholder="請選擇店別" :disabled="isEdit">
              <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="狀態" prop="status">
            <el-select v-model="form.status" placeholder="請選擇狀態">
              <el-option label="正常" value="0" />
              <el-option label="停用" value="1" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="進價" prop="lprc">
            <el-input-number v-model="form.lprc" :precision="4" :min="0" :step="0.01" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="中價" prop="mprc">
            <el-input-number v-model="form.mprc" :precision="4" :min="0" :step="0.01" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="稅別" prop="tax">
            <el-select v-model="form.tax" placeholder="請選擇稅別">
              <el-option label="應稅" value="1" />
              <el-option label="免稅" value="0" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="商品單位" prop="unit">
            <el-input v-model="form.unit" placeholder="請輸入商品單位" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="最小訂購量" prop="minQty">
            <el-input-number v-model="form.minQty" :precision="4" :min="0" :step="1" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="最大訂購量" prop="maxQty">
            <el-input-number v-model="form.maxQty" :precision="4" :min="0" :step="1" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="換算率" prop="rate">
            <el-input-number v-model="form.rate" :precision="4" :min="0" :step="0.01" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="到貨天數" prop="arrivalDays">
            <el-input-number v-model="form.arrivalDays" :min="0" :step="1" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="有效起始日" prop="startDate">
            <el-date-picker v-model="form.startDate" type="date" placeholder="請選擇日期" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="有效終止日" prop="endDate">
            <el-date-picker v-model="form.endDate" type="date" placeholder="請選擇日期" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="促銷價格" prop="slprc">
            <el-input-number v-model="form.slprc" :precision="4" :min="0" :step="0.01" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-divider>訂購日期設定</el-divider>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="週一">
            <el-switch v-model="form.ordDay1" active-value="Y" inactive-value="N" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="週二">
            <el-switch v-model="form.ordDay2" active-value="Y" inactive-value="N" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="週三">
            <el-switch v-model="form.ordDay3" active-value="Y" inactive-value="N" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="週四">
            <el-switch v-model="form.ordDay4" active-value="Y" inactive-value="N" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="週五">
            <el-switch v-model="form.ordDay5" active-value="Y" inactive-value="N" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="週六">
            <el-switch v-model="form.ordDay6" active-value="Y" inactive-value="N" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="週日">
            <el-switch v-model="form.ordDay7" active-value="Y" inactive-value="N" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`supplierGoods.api.ts`)
```typescript
import request from '@/utils/request';

export interface SupplierGoodsDto {
  supplierId: string;
  supplierName?: string;
  barcodeId: string;
  barcodeName?: string;
  shopId: string;
  shopName?: string;
  lprc?: number;
  mprc?: number;
  tax?: string;
  minQty?: number;
  maxQty?: number;
  unit?: string;
  rate?: number;
  status: string;
  startDate?: string;
  endDate?: string;
  slprc?: number;
  arrivalDays?: number;
  ordDay1?: string;
  ordDay2?: string;
  ordDay3?: string;
  ordDay4?: string;
  ordDay5?: string;
  ordDay6?: string;
  ordDay7?: string;
}

export interface SupplierGoodsQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    supplierId?: string;
    barcodeId?: string;
    shopId?: string;
    status?: string;
  };
}

export interface CreateSupplierGoodsDto {
  supplierId: string;
  barcodeId: string;
  shopId: string;
  lprc?: number;
  mprc?: number;
  tax?: string;
  minQty?: number;
  maxQty?: number;
  unit?: string;
  rate?: number;
  status: string;
  startDate?: string;
  endDate?: string;
  slprc?: number;
  arrivalDays?: number;
  ordDay1?: string;
  ordDay2?: string;
  ordDay3?: string;
  ordDay4?: string;
  ordDay5?: string;
  ordDay6?: string;
  ordDay7?: string;
}

export interface UpdateSupplierGoodsDto extends Omit<CreateSupplierGoodsDto, 'supplierId' | 'barcodeId' | 'shopId'> {}

// API 函數
export const getSupplierGoodsList = (query: SupplierGoodsQueryDto) => {
  return request.get<ApiResponse<PagedResult<SupplierGoodsDto>>>('/api/v1/supplier-goods', { params: query });
};

export const getSupplierGoodsById = (supplierId: string, barcodeId: string, shopId: string) => {
  return request.get<ApiResponse<SupplierGoodsDto>>(`/api/v1/supplier-goods/${supplierId}/${barcodeId}/${shopId}`);
};

export const createSupplierGoods = (data: CreateSupplierGoodsDto) => {
  return request.post<ApiResponse>('/api/v1/supplier-goods', data);
};

export const updateSupplierGoods = (supplierId: string, barcodeId: string, shopId: string, data: UpdateSupplierGoodsDto) => {
  return request.put<ApiResponse>(`/api/v1/supplier-goods/${supplierId}/${barcodeId}/${shopId}`, data);
};

export const deleteSupplierGoods = (supplierId: string, barcodeId: string, shopId: string) => {
  return request.delete<ApiResponse>(`/api/v1/supplier-goods/${supplierId}/${barcodeId}/${shopId}`);
};

export const updateStatus = (supplierId: string, barcodeId: string, shopId: string, status: string) => {
  return request.put<ApiResponse>(`/api/v1/supplier-goods/${supplierId}/${barcodeId}/${shopId}/status`, { status });
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
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
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

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 必須驗證供應商、商品條碼、店別的有效性

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制（供應商列表、商品列表、店別列表）

### 6.3 資料驗證
- 供應商編號、商品條碼、店別組合必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證（起始日不能大於終止日）
- 數量必須驗證（最小訂購量不能大於最大訂購量）
- 價格必須大於等於0
- 訂購日期設定必須為Y或N

### 6.4 業務邏輯
- 刪除供應商商品前必須檢查是否有相關採購單、訂單等業務資料
- 停用供應商商品時必須檢查是否有進行中的業務
- 價格變更必須記錄變更歷史（可選）
- 有效期間必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增供應商商品成功
- [ ] 新增供應商商品失敗 (重複主鍵)
- [ ] 修改供應商商品成功
- [ ] 修改供應商商品失敗 (不存在)
- [ ] 刪除供應商商品成功
- [ ] 查詢供應商商品列表成功
- [ ] 查詢單筆供應商商品成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW110_FB.ASP`
- `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/SUPP_GOODS.cs`

### 8.2 資料庫 Schema
- `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/SUPP_GOODS.cs`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


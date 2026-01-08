# SYSP110-SYSP190 - 採購單維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSP110-SYSP190 系列
- **功能名稱**: 採購單維護系列
- **功能描述**: 提供採購單資料的新增、修改、刪除、查詢功能，包含採購單號、採購日期、供應商、商品明細、數量、價格、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP110_PR.ASP` (報表)

### 1.2 業務需求
- 管理採購單基本資料
- 支援採購單狀態管理（草稿、已送出、已審核、已取消、已結案）
- 支援供應商選擇
- 支援商品明細維護
- 支援數量、單價、金額計算
- 支援採購單審核流程
- 支援多店別管理
- 支援採購單報表列印
- 支援採購單歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseOrders` (採購單主檔)

```sql
CREATE TABLE [dbo].[PurchaseOrders] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 採購單號 (PO_NO)
    [OrderDate] DATETIME2 NOT NULL, -- 採購日期 (PO_DATE)
    [OrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (ORDER_TYPE, PO:採購, RT:退貨)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
    [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商代碼 (SUPPLIER_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, X:已取消, C:已結案)
    [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
    [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
    [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
    [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
    [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量 (TOTAL_QTY)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [ExpectedDate] DATETIME2 NULL, -- 預期交貨日期 (EXPECTED_DATE)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PurchaseOrders_OrderId] UNIQUE ([OrderId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_OrderId] ON [dbo].[PurchaseOrders] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_ShopId] ON [dbo].[PurchaseOrders] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_SupplierId] ON [dbo].[PurchaseOrders] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_Status] ON [dbo].[PurchaseOrders] ([Status]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_OrderDate] ON [dbo].[PurchaseOrders] ([OrderDate]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrders_OrderType] ON [dbo].[PurchaseOrders] ([OrderType]);
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseOrderDetails` - 採購單明細
```sql
CREATE TABLE [dbo].[PurchaseOrderDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 採購單號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
    [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量 (ORDER_QTY)
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價 (UNIT_PRICE)
    [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
    [ReceivedQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已收數量 (RECEIVED_QTY)
    [ReturnQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已退數量 (RETURN_QTY)
    [UnitId] NVARCHAR(50) NULL, -- 單位 (UNIT_ID)
    [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率 (TAX_RATE)
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMOUNT)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PurchaseOrderDetails_PurchaseOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[PurchaseOrders] ([OrderId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_OrderId] ON [dbo].[PurchaseOrderDetails] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_GoodsId] ON [dbo].[PurchaseOrderDetails] ([GoodsId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_BarcodeId] ON [dbo].[PurchaseOrderDetails] ([BarcodeId]);
```

### 2.3 資料字典

#### 2.3.1 PurchaseOrders 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| OrderId | NVARCHAR | 50 | NO | - | 採購單號 | 唯一，PO_NO |
| OrderDate | DATETIME2 | - | NO | - | 採購日期 | PO_DATE |
| OrderType | NVARCHAR | 20 | NO | - | 單據類型 | PO:採購, RT:退貨 |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| SupplierId | NVARCHAR | 50 | NO | - | 供應商代碼 | 外鍵至供應商表 |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, S:已送出, A:已審核, X:已取消, C:已結案 |
| ApplyUserId | NVARCHAR | 50 | YES | - | 申請人員 | 外鍵至使用者表 |
| ApplyDate | DATETIME2 | - | YES | - | 申請日期 | - |
| ApproveUserId | NVARCHAR | 50 | YES | - | 審核人員 | 外鍵至使用者表 |
| ApproveDate | DATETIME2 | - | YES | - | 審核日期 | - |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalQty | DECIMAL | 18,4 | YES | 0 | 總數量 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| ExpectedDate | DATETIME2 | - | YES | - | 預期交貨日期 | - |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | - |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |

#### 2.3.2 PurchaseOrderDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| OrderId | NVARCHAR | 50 | NO | - | 採購單號 | 外鍵至PurchaseOrders |
| LineNum | INT | - | NO | - | 行號 | - |
| GoodsId | NVARCHAR | 50 | NO | - | 商品編號 | 外鍵至商品表 |
| BarcodeId | NVARCHAR | 50 | YES | - | 條碼編號 | 外鍵至條碼表 |
| OrderQty | DECIMAL | 18,4 | NO | 0 | 訂購數量 | - |
| UnitPrice | DECIMAL | 18,4 | YES | - | 單價 | - |
| Amount | DECIMAL | 18,4 | YES | - | 金額 | - |
| ReceivedQty | DECIMAL | 18,4 | YES | 0 | 已收數量 | - |
| ReturnQty | DECIMAL | 18,4 | YES | 0 | 已退數量 | - |
| UnitId | NVARCHAR | 50 | YES | - | 單位 | - |
| TaxRate | DECIMAL | 5,2 | YES | 0 | 稅率 | - |
| TaxAmount | DECIMAL | 18,4 | YES | 0 | 稅額 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢採購單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders`
- **說明**: 查詢採購單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "OrderId",
    "sortOrder": "ASC",
    "filters": {
      "orderId": "",
      "orderType": "",
      "shopId": "",
      "supplierId": "",
      "status": "",
      "orderDateFrom": "",
      "orderDateTo": ""
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
          "orderId": "PO20240101001",
          "orderDate": "2024-01-01",
          "orderType": "PO",
          "shopId": "SHOP001",
          "shopName": "總店",
          "supplierId": "SUP001",
          "supplierName": "供應商A",
          "status": "A",
          "totalAmount": 100000.00,
          "totalQty": 100.00,
          "applyUserId": "U001",
          "applyUserName": "張三",
          "applyDate": "2024-01-01T10:00:00",
          "approveUserId": "U002",
          "approveUserName": "李四",
          "approveDate": "2024-01-01T11:00:00"
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

#### 3.1.2 查詢單筆採購單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders/{orderId}`
- **說明**: 根據採購單號查詢單筆採購單資料（含明細）
- **路徑參數**:
  - `orderId`: 採購單號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "orderId": "PO20240101001",
      "orderDate": "2024-01-01",
      "orderType": "PO",
      "shopId": "SHOP001",
      "shopName": "總店",
      "supplierId": "SUP001",
      "supplierName": "供應商A",
      "status": "A",
      "totalAmount": 100000.00,
      "totalQty": 100.00,
      "memo": "備註",
      "expectedDate": "2024-01-15",
      "siteId": "SITE001",
      "orgId": "ORG001",
      "currencyId": "TWD",
      "exchangeRate": 1.0,
      "applyUserId": "U001",
      "applyUserName": "張三",
      "applyDate": "2024-01-01T10:00:00",
      "approveUserId": "U002",
      "approveUserName": "李四",
      "approveDate": "2024-01-01T11:00:00",
      "details": [
        {
          "lineNum": 1,
          "goodsId": "G001",
          "goodsName": "商品A",
          "barcodeId": "BC001",
          "orderQty": 10.00,
          "unitPrice": 1000.00,
          "amount": 10000.00,
          "receivedQty": 0.00,
          "returnQty": 0.00,
          "unitId": "PCS",
          "taxRate": 5.00,
          "taxAmount": 500.00,
          "memo": "明細備註"
        }
      ],
      "createdBy": "U001",
      "createdAt": "2024-01-01T10:00:00",
      "updatedBy": "U002",
      "updatedAt": "2024-01-01T11:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders`
- **說明**: 新增採購單資料（含明細）
- **請求格式**:
  ```json
  {
    "orderId": "PO20240101001",
    "orderDate": "2024-01-01",
    "orderType": "PO",
    "shopId": "SHOP001",
    "supplierId": "SUP001",
    "status": "D",
    "totalAmount": 100000.00,
    "totalQty": 100.00,
    "memo": "備註",
    "expectedDate": "2024-01-15",
    "siteId": "SITE001",
    "orgId": "ORG001",
    "currencyId": "TWD",
    "exchangeRate": 1.0,
    "details": [
      {
        "lineNum": 1,
        "goodsId": "G001",
        "barcodeId": "BC001",
        "orderQty": 10.00,
        "unitPrice": 1000.00,
        "amount": 10000.00,
        "unitId": "PCS",
        "taxRate": 5.00,
        "taxAmount": 500.00,
        "memo": "明細備註"
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "orderId": "PO20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改採購單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-orders/{orderId}`
- **說明**: 修改採購單資料（僅草稿狀態可修改）
- **路徑參數**:
  - `orderId`: 採購單號
- **請求格式**: 同新增，但 `orderId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除採購單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-orders/{orderId}`
- **說明**: 刪除採購單資料（僅草稿狀態可刪除）
- **路徑參數**:
  - `orderId`: 採購單號
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

#### 3.1.6 批次刪除採購單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-orders/batch`
- **說明**: 批次刪除多筆採購單（僅草稿狀態可刪除）
- **請求格式**:
  ```json
  {
    "orderIds": ["PO20240101001", "PO20240101002", "PO20240101003"]
  }
  ```

#### 3.1.7 送出採購單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-orders/{orderId}/submit`
- **說明**: 送出採購單進行審核（狀態由草稿改為已送出）
- **路徑參數**:
  - `orderId`: 採購單號
- **回應格式**: 同新增

#### 3.1.8 審核採購單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-orders/{orderId}/approve`
- **說明**: 審核採購單（狀態由已送出改為已審核）
- **路徑參數**:
  - `orderId`: 採購單號
- **請求格式**:
  ```json
  {
    "approveUserId": "U002",
    "approveDate": "2024-01-01T11:00:00",
    "memo": "審核備註"
  }
  ```

#### 3.1.9 取消採購單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-orders/{orderId}/cancel`
- **說明**: 取消採購單（狀態改為已取消）
- **路徑參數**:
  - `orderId`: 採購單號
- **請求格式**:
  ```json
  {
    "cancelReason": "取消原因"
  }
  ```

#### 3.1.10 結案採購單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-orders/{orderId}/close`
- **說明**: 結案採購單（狀態改為已結案）
- **路徑參數**:
  - `orderId`: 採購單號

### 3.2 後端實作類別

#### 3.2.1 Controller: `PurchaseOrdersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchase-orders")]
    [Authorize]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        
        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderDto>>>> GetPurchaseOrders([FromQuery] PurchaseOrderQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDto>>> GetPurchaseOrder(string orderId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{orderId}")]
        public async Task<ActionResult<ApiResponse>> UpdatePurchaseOrder(string orderId, [FromBody] UpdatePurchaseOrderDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<ApiResponse>> DeletePurchaseOrder(string orderId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPut("{orderId}/submit")]
        public async Task<ActionResult<ApiResponse>> SubmitPurchaseOrder(string orderId)
        {
            // 實作送出邏輯
        }
        
        [HttpPut("{orderId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApprovePurchaseOrder(string orderId, [FromBody] ApprovePurchaseOrderDto dto)
        {
            // 實作審核邏輯
        }
        
        [HttpPut("{orderId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelPurchaseOrder(string orderId, [FromBody] CancelPurchaseOrderDto dto)
        {
            // 實作取消邏輯
        }
        
        [HttpPut("{orderId}/close")]
        public async Task<ActionResult<ApiResponse>> ClosePurchaseOrder(string orderId)
        {
            // 實作結案邏輯
        }
    }
}
```

#### 3.2.2 Service: `PurchaseOrderService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPurchaseOrderService
    {
        Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(PurchaseOrderQueryDto query);
        Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(string orderId);
        Task<string> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task UpdatePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto);
        Task DeletePurchaseOrderAsync(string orderId);
        Task SubmitPurchaseOrderAsync(string orderId);
        Task ApprovePurchaseOrderAsync(string orderId, ApprovePurchaseOrderDto dto);
        Task CancelPurchaseOrderAsync(string orderId, CancelPurchaseOrderDto dto);
        Task ClosePurchaseOrderAsync(string orderId);
    }
}
```

#### 3.2.3 Repository: `PurchaseOrderRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder> GetByIdAsync(string orderId);
        Task<PagedResult<PurchaseOrder>> GetPagedAsync(PurchaseOrderQuery query);
        Task<PurchaseOrder> CreateAsync(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder> UpdateAsync(PurchaseOrder purchaseOrder);
        Task DeleteAsync(string orderId);
        Task<bool> ExistsAsync(string orderId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購單列表頁面 (`PurchaseOrderList.vue`)
- **路徑**: `/procurement/purchase-orders`
- **功能**: 顯示採購單列表，支援查詢、新增、修改、刪除、送出、審核、取消、結案
- **主要元件**:
  - 查詢表單 (PurchaseOrderSearchForm)
  - 資料表格 (PurchaseOrderDataTable)
  - 新增/修改對話框 (PurchaseOrderDialog)
  - 刪除確認對話框
  - 送出/審核/取消/結案操作按鈕

#### 4.1.2 採購單詳細頁面 (`PurchaseOrderDetail.vue`)
- **路徑**: `/procurement/purchase-orders/:orderId`
- **功能**: 顯示採購單詳細資料（含明細），支援修改、送出、審核、取消、結案

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`PurchaseOrderSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="採購單號">
      <el-input v-model="searchForm.orderId" placeholder="請輸入採購單號" />
    </el-form-item>
    <el-form-item label="單據類型">
      <el-select v-model="searchForm.orderType" placeholder="請選擇單據類型">
        <el-option label="採購" value="PO" />
        <el-option label="退貨" value="RT" />
      </el-select>
    </el-form-item>
    <el-form-item label="分店">
      <el-select v-model="searchForm.shopId" placeholder="請選擇分店">
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="供應商">
      <el-select v-model="searchForm.supplierId" placeholder="請選擇供應商">
        <el-option v-for="supplier in supplierList" :key="supplier.supplierId" :label="supplier.supplierName" :value="supplier.supplierId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="草稿" value="D" />
        <el-option label="已送出" value="S" />
        <el-option label="已審核" value="A" />
        <el-option label="已取消" value="X" />
        <el-option label="已結案" value="C" />
      </el-select>
    </el-form-item>
    <el-form-item label="採購日期">
      <el-date-picker
        v-model="searchForm.orderDateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
      />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`PurchaseOrderDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="purchaseOrderList" v-loading="loading">
      <el-table-column prop="orderId" label="採購單號" width="150" />
      <el-table-column prop="orderDate" label="採購日期" width="120" />
      <el-table-column prop="orderType" label="單據類型" width="100">
        <template #default="{ row }">
          <el-tag :type="row.orderType === 'PO' ? 'success' : 'warning'">
            {{ row.orderType === 'PO' ? '採購' : '退貨' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="shopName" label="分店" width="120" />
      <el-table-column prop="supplierName" label="供應商" width="150" />
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="totalAmount" label="總金額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.totalAmount) }}
        </template>
      </el-table-column>
      <el-table-column prop="totalQty" label="總數量" width="120" align="right" />
      <el-table-column prop="applyUserName" label="申請人" width="100" />
      <el-table-column prop="applyDate" label="申請日期" width="160" />
      <el-table-column label="操作" width="300" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" v-if="row.status === 'D'">刪除</el-button>
          <el-button type="success" size="small" @click="handleSubmit(row)" v-if="row.status === 'D'">送出</el-button>
          <el-button type="warning" size="small" @click="handleApprove(row)" v-if="row.status === 'S'">審核</el-button>
          <el-button type="info" size="small" @click="handleCancel(row)" v-if="row.status === 'S' || row.status === 'A'">取消</el-button>
          <el-button type="success" size="small" @click="handleClose(row)" v-if="row.status === 'A'">結案</el-button>
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

#### 4.2.3 新增/修改對話框 (`PurchaseOrderDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="1200px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="採購單號" prop="orderId">
            <el-input v-model="form.orderId" :disabled="isEdit" placeholder="請輸入採購單號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="採購日期" prop="orderDate">
            <el-date-picker v-model="form.orderDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="單據類型" prop="orderType">
            <el-select v-model="form.orderType" placeholder="請選擇單據類型">
              <el-option label="採購" value="PO" />
              <el-option label="退貨" value="RT" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="分店" prop="shopId">
            <el-select v-model="form.shopId" placeholder="請選擇分店">
              <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="供應商" prop="supplierId">
            <el-select v-model="form.supplierId" placeholder="請選擇供應商">
              <el-option v-for="supplier in supplierList" :key="supplier.supplierId" :label="supplier.supplierName" :value="supplier.supplierId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="預期交貨日期" prop="expectedDate">
            <el-date-picker v-model="form.expectedDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="幣別" prop="currencyId">
            <el-select v-model="form.currencyId" placeholder="請選擇幣別">
              <el-option label="新台幣" value="TWD" />
              <el-option label="美元" value="USD" />
              <el-option label="人民幣" value="CNY" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="匯率" prop="exchangeRate">
            <el-input-number v-model="form.exchangeRate" :precision="6" :min="0" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="memo">
        <el-input v-model="form.memo" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
      
      <!-- 明細表格 -->
      <el-form-item label="明細">
        <el-table :data="form.details" border>
          <el-table-column type="index" label="行號" width="60" />
          <el-table-column prop="goodsId" label="商品編號" width="120">
            <template #default="{ row, $index }">
              <el-select v-model="row.goodsId" placeholder="請選擇商品" @change="handleGoodsChange($index)">
                <el-option v-for="goods in goodsList" :key="goods.goodsId" :label="goods.goodsName" :value="goods.goodsId" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="goodsName" label="商品名稱" width="150" />
          <el-table-column prop="orderQty" label="訂購數量" width="120">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.orderQty" :precision="4" :min="0" @change="handleQtyChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="unitPrice" label="單價" width="120">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.unitPrice" :precision="4" :min="0" @change="handlePriceChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="amount" label="金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.amount) }}
            </template>
          </el-table-column>
          <el-table-column prop="taxRate" label="稅率%" width="100">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.taxRate" :precision="2" :min="0" :max="100" @change="handleTaxChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="taxAmount" label="稅額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.taxAmount) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" fixed="right">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" @click="handleAddDetail" style="margin-top: 10px">新增明細</el-button>
      </el-form-item>
      
      <el-form-item label="總金額" prop="totalAmount">
        <el-input v-model="form.totalAmount" :disabled="true" />
      </el-form-item>
      <el-form-item label="總數量" prop="totalQty">
        <el-input v-model="form.totalQty" :disabled="true" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`purchaseOrder.api.ts`)
```typescript
import request from '@/utils/request';

export interface PurchaseOrderDto {
  orderId: string;
  orderDate: string;
  orderType: string;
  shopId: string;
  shopName?: string;
  supplierId: string;
  supplierName?: string;
  status: string;
  totalAmount: number;
  totalQty: number;
  memo?: string;
  expectedDate?: string;
  siteId?: string;
  orgId?: string;
  currencyId?: string;
  exchangeRate?: number;
  applyUserId?: string;
  applyUserName?: string;
  applyDate?: string;
  approveUserId?: string;
  approveUserName?: string;
  approveDate?: string;
  details?: PurchaseOrderDetailDto[];
}

export interface PurchaseOrderDetailDto {
  lineNum: number;
  goodsId: string;
  goodsName?: string;
  barcodeId?: string;
  orderQty: number;
  unitPrice?: number;
  amount?: number;
  receivedQty?: number;
  returnQty?: number;
  unitId?: string;
  taxRate?: number;
  taxAmount?: number;
  memo?: string;
}

export interface PurchaseOrderQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    orderId?: string;
    orderType?: string;
    shopId?: string;
    supplierId?: string;
    status?: string;
    orderDateFrom?: string;
    orderDateTo?: string;
  };
}

export interface CreatePurchaseOrderDto {
  orderId: string;
  orderDate: string;
  orderType: string;
  shopId: string;
  supplierId: string;
  status: string;
  totalAmount: number;
  totalQty: number;
  memo?: string;
  expectedDate?: string;
  siteId?: string;
  orgId?: string;
  currencyId?: string;
  exchangeRate?: number;
  details: CreatePurchaseOrderDetailDto[];
}

export interface CreatePurchaseOrderDetailDto {
  lineNum: number;
  goodsId: string;
  barcodeId?: string;
  orderQty: number;
  unitPrice?: number;
  amount?: number;
  unitId?: string;
  taxRate?: number;
  taxAmount?: number;
  memo?: string;
}

export interface UpdatePurchaseOrderDto extends Omit<CreatePurchaseOrderDto, 'orderId'> {}

// API 函數
export const getPurchaseOrderList = (query: PurchaseOrderQueryDto) => {
  return request.get<ApiResponse<PagedResult<PurchaseOrderDto>>>('/api/v1/purchase-orders', { params: query });
};

export const getPurchaseOrderById = (orderId: string) => {
  return request.get<ApiResponse<PurchaseOrderDto>>(`/api/v1/purchase-orders/${orderId}`);
};

export const createPurchaseOrder = (data: CreatePurchaseOrderDto) => {
  return request.post<ApiResponse<string>>('/api/v1/purchase-orders', data);
};

export const updatePurchaseOrder = (orderId: string, data: UpdatePurchaseOrderDto) => {
  return request.put<ApiResponse>(`/api/v1/purchase-orders/${orderId}`, data);
};

export const deletePurchaseOrder = (orderId: string) => {
  return request.delete<ApiResponse>(`/api/v1/purchase-orders/${orderId}`);
};

export const submitPurchaseOrder = (orderId: string) => {
  return request.put<ApiResponse>(`/api/v1/purchase-orders/${orderId}/submit`);
};

export const approvePurchaseOrder = (orderId: string, data: { approveUserId: string; approveDate: string; memo?: string }) => {
  return request.put<ApiResponse>(`/api/v1/purchase-orders/${orderId}/approve`, data);
};

export const cancelPurchaseOrder = (orderId: string, data: { cancelReason: string }) => {
  return request.put<ApiResponse>(`/api/v1/purchase-orders/${orderId}/cancel`, data);
};

export const closePurchaseOrder = (orderId: string) => {
  return request.put<ApiResponse>(`/api/v1/purchase-orders/${orderId}/close`);
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

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 狀態流程邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 明細表格開發
- [ ] 表單驗證
- [ ] 狀態流程操作
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 狀態流程測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 14天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 狀態變更必須記錄操作人員和時間

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 採購單號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- 明細金額必須等於數量×單價
- 總金額必須等於明細金額總和

### 6.4 業務邏輯
- 刪除採購單前必須檢查狀態（僅草稿可刪除）
- 修改採購單前必須檢查狀態（僅草稿可修改）
- 送出採購單前必須檢查必填欄位和明細
- 審核採購單前必須檢查狀態（僅已送出可審核）
- 取消採購單前必須檢查狀態（僅已送出或已審核可取消）
- 結案採購單前必須檢查狀態（僅已審核可結案）
- 明細金額自動計算（數量×單價）
- 總金額自動計算（明細金額總和）
- 稅額自動計算（金額×稅率）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增採購單成功
- [ ] 新增採購單失敗 (重複單號)
- [ ] 修改採購單成功
- [ ] 修改採購單失敗 (非草稿狀態)
- [ ] 刪除採購單成功
- [ ] 刪除採購單失敗 (非草稿狀態)
- [ ] 查詢採購單列表成功
- [ ] 查詢單筆採購單成功
- [ ] 送出採購單成功
- [ ] 送出採購單失敗 (非草稿狀態)
- [ ] 審核採購單成功
- [ ] 審核採購單失敗 (非已送出狀態)
- [ ] 取消採購單成功
- [ ] 結案採購單成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 狀態流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP110_FB.ASP` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP110_PR.ASP` (報表)

### 8.2 相關功能
- `開發計劃/04-採購管理/SYSW315-訂退貨申請作業.md`
- `開發計劃/04-採購管理/SYSW324-採購單驗收作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


# SYSD110-SYSD140 - 銷售資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSD110-SYSD140 系列
- **功能名稱**: 銷售資料維護系列
- **功能描述**: 提供銷售資料的新增、修改、刪除、查詢功能，包含銷售單號、銷售日期、客戶、商品明細、數量、價格、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD110_PR.ASP` (報表)

### 1.2 業務需求
- 管理銷售單基本資料
- 支援銷售單狀態管理（草稿、已送出、已審核、已出貨、已取消、已結案）
- 支援客戶選擇
- 支援商品明細維護
- 支援數量、單價、金額計算
- 支援銷售單審核流程
- 支援多店別管理
- 支援銷售單報表列印
- 支援銷售單歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SalesOrders` (銷售單主檔)

```sql
CREATE TABLE [dbo].[SalesOrders] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號 (SO_NO)
    [OrderDate] DATETIME2 NOT NULL, -- 銷售日期 (SO_DATE)
    [OrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (ORDER_TYPE, SO:銷售, RT:退貨)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
    [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶代碼 (CUSTOMER_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, O:已出貨, X:已取消, C:已結案)
    [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
    [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
    [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
    [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
    [ShipDate] DATETIME2 NULL, -- 出貨日期 (SHIP_DATE)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
    [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量 (TOTAL_QTY)
    [DiscountAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 折扣金額 (DISCOUNT_AMT)
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMT)
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
    CONSTRAINT [PK_SalesOrders] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_SalesOrders_OrderId] UNIQUE ([OrderId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderId] ON [dbo].[SalesOrders] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_ShopId] ON [dbo].[SalesOrders] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_CustomerId] ON [dbo].[SalesOrders] ([CustomerId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_Status] ON [dbo].[SalesOrders] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderDate] ON [dbo].[SalesOrders] ([OrderDate]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderType] ON [dbo].[SalesOrders] ([OrderType]);
```

### 2.2 相關資料表

#### 2.2.1 `SalesOrderDetails` - 銷售單明細
```sql
CREATE TABLE [dbo].[SalesOrderDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
    [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量 (ORDER_QTY)
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價 (UNIT_PRICE)
    [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
    [ShippedQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已出貨數量 (SHIPPED_QTY)
    [ReturnQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已退數量 (RETURN_QTY)
    [UnitId] NVARCHAR(50) NULL, -- 單位 (UNIT_ID)
    [DiscountRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 折扣率 (DISCOUNT_RATE)
    [DiscountAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 折扣金額 (DISCOUNT_AMOUNT)
    [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率 (TAX_RATE)
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMOUNT)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_SalesOrderDetails_SalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[SalesOrders] ([OrderId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_OrderId] ON [dbo].[SalesOrderDetails] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_GoodsId] ON [dbo].[SalesOrderDetails] ([GoodsId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_BarcodeId] ON [dbo].[SalesOrderDetails] ([BarcodeId]);
```

### 2.3 資料字典

#### 2.3.1 SalesOrders 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| OrderId | NVARCHAR | 50 | NO | - | 銷售單號 | 唯一，SO_NO |
| OrderDate | DATETIME2 | - | NO | - | 銷售日期 | SO_DATE |
| OrderType | NVARCHAR | 20 | NO | - | 單據類型 | SO:銷售, RT:退貨 |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| CustomerId | NVARCHAR | 50 | NO | - | 客戶代碼 | 外鍵至客戶表 |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, S:已送出, A:已審核, O:已出貨, X:已取消, C:已結案 |
| ApplyUserId | NVARCHAR | 50 | YES | - | 申請人員 | 外鍵至使用者表 |
| ApplyDate | DATETIME2 | - | YES | - | 申請日期 | - |
| ApproveUserId | NVARCHAR | 50 | YES | - | 審核人員 | 外鍵至使用者表 |
| ApproveDate | DATETIME2 | - | YES | - | 審核日期 | - |
| ShipDate | DATETIME2 | - | YES | - | 出貨日期 | - |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalQty | DECIMAL | 18,4 | YES | 0 | 總數量 | - |
| DiscountAmount | DECIMAL | 18,4 | YES | 0 | 折扣金額 | - |
| TaxAmount | DECIMAL | 18,4 | YES | 0 | 稅額 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| ExpectedDate | DATETIME2 | - | YES | - | 預期交貨日期 | - |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | - |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |

#### 2.3.2 SalesOrderDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| OrderId | NVARCHAR | 50 | NO | - | 銷售單號 | 外鍵至SalesOrders |
| LineNum | INT | - | NO | - | 行號 | - |
| GoodsId | NVARCHAR | 50 | NO | - | 商品編號 | 外鍵至商品表 |
| BarcodeId | NVARCHAR | 50 | YES | - | 條碼編號 | 外鍵至條碼表 |
| OrderQty | DECIMAL | 18,4 | NO | 0 | 訂購數量 | - |
| UnitPrice | DECIMAL | 18,4 | YES | - | 單價 | - |
| Amount | DECIMAL | 18,4 | YES | - | 金額 | - |
| ShippedQty | DECIMAL | 18,4 | YES | 0 | 已出貨數量 | - |
| ReturnQty | DECIMAL | 18,4 | YES | 0 | 已退數量 | - |
| UnitId | NVARCHAR | 50 | YES | - | 單位 | - |
| DiscountRate | DECIMAL | 5,2 | YES | 0 | 折扣率 | - |
| DiscountAmount | DECIMAL | 18,4 | YES | 0 | 折扣金額 | - |
| TaxRate | DECIMAL | 5,2 | YES | 0 | 稅率 | - |
| TaxAmount | DECIMAL | 18,4 | YES | 0 | 稅額 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢銷售單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sales-orders`
- **說明**: 查詢銷售單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "OrderId",
    "sortOrder": "ASC",
    "filters": {
      "orderId": "",
      "orderDateFrom": "",
      "orderDateTo": "",
      "shopId": "",
      "customerId": "",
      "status": "",
      "orderType": ""
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
          "tKey": 1,
          "orderId": "SO001",
          "orderDate": "2024-01-01",
          "orderType": "SO",
          "shopId": "SHOP001",
          "customerId": "C001",
          "customerName": "客戶名稱",
          "status": "A",
          "totalAmount": 10000.00,
          "totalQty": 100.00,
          "applyUserId": "U001",
          "applyDate": "2024-01-01T10:00:00",
          "approveUserId": "U002",
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

#### 3.1.2 查詢單筆銷售單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sales-orders/{orderId}`
- **說明**: 根據銷售單號查詢單筆銷售單資料（包含明細）
- **路徑參數**:
  - `orderId`: 銷售單號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "orderId": "SO001",
      "orderDate": "2024-01-01",
      "orderType": "SO",
      "shopId": "SHOP001",
      "customerId": "C001",
      "customerName": "客戶名稱",
      "status": "A",
      "totalAmount": 10000.00,
      "totalQty": 100.00,
      "discountAmount": 0.00,
      "taxAmount": 500.00,
      "memo": "備註",
      "expectedDate": "2024-01-15",
      "details": [
        {
          "tKey": 1,
          "lineNum": 1,
          "goodsId": "G001",
          "goodsName": "商品名稱",
          "barcodeId": "BC001",
          "orderQty": 10.00,
          "unitPrice": 100.00,
          "amount": 1000.00,
          "shippedQty": 0.00,
          "returnQty": 0.00,
          "unitId": "PCS",
          "discountRate": 0.00,
          "discountAmount": 0.00,
          "taxRate": 5.00,
          "taxAmount": 50.00,
          "memo": "明細備註"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增銷售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders`
- **說明**: 新增銷售單資料（包含明細）
- **請求格式**:
  ```json
  {
    "orderId": "SO001",
    "orderDate": "2024-01-01",
    "orderType": "SO",
    "shopId": "SHOP001",
    "customerId": "C001",
    "status": "D",
    "expectedDate": "2024-01-15",
    "memo": "備註",
    "currencyId": "TWD",
    "exchangeRate": 1.0,
    "details": [
      {
        "lineNum": 1,
        "goodsId": "G001",
        "barcodeId": "BC001",
        "orderQty": 10.00,
        "unitPrice": 100.00,
        "unitId": "PCS",
        "discountRate": 0.00,
        "taxRate": 5.00,
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
      "orderId": "SO001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改銷售單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/sales-orders/{orderId}`
- **說明**: 修改銷售單資料（包含明細）
- **路徑參數**:
  - `orderId`: 銷售單號
- **請求格式**: 同新增，但 `orderId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除銷售單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/sales-orders/{orderId}`
- **說明**: 刪除銷售單資料（軟刪除或硬刪除）
- **路徑參數**:
  - `orderId`: 銷售單號
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

#### 3.1.6 審核銷售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/{orderId}/approve`
- **說明**: 審核銷售單
- **路徑參數**:
  - `orderId`: 銷售單號
- **請求格式**:
  ```json
  {
    "approveUserId": "U002",
    "memo": "審核備註"
  }
  ```

#### 3.1.7 出貨銷售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/{orderId}/ship`
- **說明**: 出貨銷售單
- **路徑參數**:
  - `orderId`: 銷售單號
- **請求格式**:
  ```json
  {
    "shipDate": "2024-01-15",
    "details": [
      {
        "lineNum": 1,
        "shippedQty": 10.00
      }
    ]
  }
  ```

#### 3.1.8 取消銷售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/{orderId}/cancel`
- **說明**: 取消銷售單
- **路徑參數**:
  - `orderId`: 銷售單號
- **請求格式**:
  ```json
  {
    "memo": "取消原因"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `SalesOrdersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/sales-orders")]
    [Authorize]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;
        
        public SalesOrdersController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<SalesOrderDto>>>> GetSalesOrders([FromQuery] SalesOrderQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<SalesOrderDto>>> GetSalesOrder(string orderId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateSalesOrder([FromBody] CreateSalesOrderDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{orderId}")]
        public async Task<ActionResult<ApiResponse>> UpdateSalesOrder(string orderId, [FromBody] UpdateSalesOrderDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<ApiResponse>> DeleteSalesOrder(string orderId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{orderId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveSalesOrder(string orderId, [FromBody] ApproveSalesOrderDto dto)
        {
            // 實作審核邏輯
        }
        
        [HttpPost("{orderId}/ship")]
        public async Task<ActionResult<ApiResponse>> ShipSalesOrder(string orderId, [FromBody] ShipSalesOrderDto dto)
        {
            // 實作出貨邏輯
        }
        
        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelSalesOrder(string orderId, [FromBody] CancelSalesOrderDto dto)
        {
            // 實作取消邏輯
        }
    }
}
```

#### 3.2.2 Service: `SalesOrderService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ISalesOrderService
    {
        Task<PagedResult<SalesOrderDto>> GetSalesOrdersAsync(SalesOrderQueryDto query);
        Task<SalesOrderDto> GetSalesOrderByIdAsync(string orderId);
        Task<string> CreateSalesOrderAsync(CreateSalesOrderDto dto);
        Task UpdateSalesOrderAsync(string orderId, UpdateSalesOrderDto dto);
        Task DeleteSalesOrderAsync(string orderId);
        Task ApproveSalesOrderAsync(string orderId, ApproveSalesOrderDto dto);
        Task ShipSalesOrderAsync(string orderId, ShipSalesOrderDto dto);
        Task CancelSalesOrderAsync(string orderId, CancelSalesOrderDto dto);
    }
}
```

#### 3.2.3 Repository: `SalesOrderRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ISalesOrderRepository
    {
        Task<SalesOrder> GetByIdAsync(string orderId);
        Task<PagedResult<SalesOrder>> GetPagedAsync(SalesOrderQuery query);
        Task<SalesOrder> CreateAsync(SalesOrder salesOrder);
        Task<SalesOrder> UpdateAsync(SalesOrder salesOrder);
        Task DeleteAsync(string orderId);
        Task<bool> ExistsAsync(string orderId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 銷售單列表頁面 (`SalesOrderList.vue`)
- **路徑**: `/sales/orders`
- **功能**: 顯示銷售單列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (SalesOrderSearchForm)
  - 資料表格 (SalesOrderDataTable)
  - 新增/修改對話框 (SalesOrderDialog)
  - 刪除確認對話框

#### 4.1.2 銷售單詳細頁面 (`SalesOrderDetail.vue`)
- **路徑**: `/sales/orders/:orderId`
- **功能**: 顯示銷售單詳細資料，支援修改、審核、出貨、取消

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SalesOrderSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="銷售單號">
      <el-input v-model="searchForm.orderId" placeholder="請輸入銷售單號" />
    </el-form-item>
    <el-form-item label="銷售日期起">
      <el-date-picker v-model="searchForm.orderDateFrom" type="date" placeholder="請選擇日期" />
    </el-form-item>
    <el-form-item label="銷售日期訖">
      <el-date-picker v-model="searchForm.orderDateTo" type="date" placeholder="請選擇日期" />
    </el-form-item>
    <el-form-item label="分店">
      <el-select v-model="searchForm.shopId" placeholder="請選擇分店">
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="客戶">
      <el-input v-model="searchForm.customerId" placeholder="請輸入客戶代碼" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="草稿" value="D" />
        <el-option label="已送出" value="S" />
        <el-option label="已審核" value="A" />
        <el-option label="已出貨" value="O" />
        <el-option label="已取消" value="X" />
        <el-option label="已結案" value="C" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`SalesOrderDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="salesOrderList" v-loading="loading">
      <el-table-column prop="orderId" label="銷售單號" width="120" />
      <el-table-column prop="orderDate" label="銷售日期" width="100" />
      <el-table-column prop="customerName" label="客戶名稱" width="150" />
      <el-table-column prop="shopName" label="分店" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
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
      <el-table-column prop="totalQty" label="總數量" width="100" align="right" />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="success" size="small" @click="handleApprove(row)" v-if="row.status === 'S'">審核</el-button>
          <el-button type="warning" size="small" @click="handleShip(row)" v-if="row.status === 'A'">出貨</el-button>
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

#### 4.2.3 新增/修改對話框 (`SalesOrderDialog.vue`)
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
          <el-form-item label="銷售單號" prop="orderId">
            <el-input v-model="form.orderId" :disabled="isEdit" placeholder="請輸入銷售單號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="銷售日期" prop="orderDate">
            <el-date-picker v-model="form.orderDate" type="date" placeholder="請選擇日期" />
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
          <el-form-item label="客戶" prop="customerId">
            <el-select v-model="form.customerId" placeholder="請選擇客戶" filterable>
              <el-option v-for="customer in customerList" :key="customer.customerId" :label="customer.customerName" :value="customer.customerId" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="明細">
        <el-table :data="form.details" border>
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column prop="goodsId" label="商品編號" width="120">
            <template #default="{ row, $index }">
              <el-select v-model="row.goodsId" placeholder="請選擇商品" filterable @change="handleGoodsChange($index)">
                <el-option v-for="goods in goodsList" :key="goods.goodsId" :label="goods.goodsName" :value="goods.goodsId" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="orderQty" label="數量" width="100">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.orderQty" :min="0" :precision="2" @change="handleQtyChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="unitPrice" label="單價" width="100">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.unitPrice" :min="0" :precision="2" @change="handlePriceChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="amount" label="金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.amount) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="handleAddDetail" style="margin-top: 10px">新增明細</el-button>
      </el-form-item>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="總金額">
            <el-input v-model="form.totalAmount" disabled />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="總數量">
            <el-input v-model="form.totalQty" disabled />
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

### 4.3 API 呼叫 (`salesOrder.api.ts`)
```typescript
import request from '@/utils/request';

export interface SalesOrderDto {
  tKey: number;
  orderId: string;
  orderDate: string;
  orderType: string;
  shopId: string;
  shopName?: string;
  customerId: string;
  customerName?: string;
  status: string;
  totalAmount: number;
  totalQty: number;
  discountAmount: number;
  taxAmount: number;
  memo?: string;
  expectedDate?: string;
  details?: SalesOrderDetailDto[];
}

export interface SalesOrderDetailDto {
  tKey: number;
  lineNum: number;
  goodsId: string;
  goodsName?: string;
  barcodeId?: string;
  orderQty: number;
  unitPrice: number;
  amount: number;
  shippedQty: number;
  returnQty: number;
  unitId?: string;
  discountRate: number;
  discountAmount: number;
  taxRate: number;
  taxAmount: number;
  memo?: string;
}

export interface SalesOrderQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    orderId?: string;
    orderDateFrom?: string;
    orderDateTo?: string;
    shopId?: string;
    customerId?: string;
    status?: string;
    orderType?: string;
  };
}

export interface CreateSalesOrderDto {
  orderId: string;
  orderDate: string;
  orderType: string;
  shopId: string;
  customerId: string;
  status: string;
  expectedDate?: string;
  memo?: string;
  currencyId?: string;
  exchangeRate?: number;
  details: CreateSalesOrderDetailDto[];
}

export interface CreateSalesOrderDetailDto {
  lineNum: number;
  goodsId: string;
  barcodeId?: string;
  orderQty: number;
  unitPrice: number;
  unitId?: string;
  discountRate?: number;
  taxRate?: number;
  memo?: string;
}

export interface UpdateSalesOrderDto extends Omit<CreateSalesOrderDto, 'orderId'> {}

// API 函數
export const getSalesOrderList = (query: SalesOrderQueryDto) => {
  return request.get<ApiResponse<PagedResult<SalesOrderDto>>>('/api/v1/sales-orders', { params: query });
};

export const getSalesOrderById = (orderId: string) => {
  return request.get<ApiResponse<SalesOrderDto>>(`/api/v1/sales-orders/${orderId}`);
};

export const createSalesOrder = (data: CreateSalesOrderDto) => {
  return request.post<ApiResponse<string>>('/api/v1/sales-orders', data);
};

export const updateSalesOrder = (orderId: string, data: UpdateSalesOrderDto) => {
  return request.put<ApiResponse>(`/api/v1/sales-orders/${orderId}`, data);
};

export const deleteSalesOrder = (orderId: string) => {
  return request.delete<ApiResponse>(`/api/v1/sales-orders/${orderId}`);
};

export const approveSalesOrder = (orderId: string, data: { approveUserId: string; memo?: string }) => {
  return request.post<ApiResponse>(`/api/v1/sales-orders/${orderId}/approve`, data);
};

export const shipSalesOrder = (orderId: string, data: { shipDate: string; details: Array<{ lineNum: number; shippedQty: number }> }) => {
  return request.post<ApiResponse>(`/api/v1/sales-orders/${orderId}/ship`, data);
};

export const cancelSalesOrder = (orderId: string, data: { memo: string }) => {
  return request.post<ApiResponse>(`/api/v1/sales-orders/${orderId}/cancel`, data);
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
- [ ] 業務邏輯實作（審核、出貨、取消）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 明細表格開發
- [ ] 表單驗證
- [ ] 金額計算邏輯
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
- 必須實作資料驗證

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 銷售單號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- 數量、金額必須大於等於0

### 6.4 業務邏輯
- 刪除銷售單前必須檢查是否有相關資料
- 只有草稿狀態的銷售單可以修改
- 只有已送出狀態的銷售單可以審核
- 只有已審核狀態的銷售單可以出貨
- 出貨數量不能超過訂購數量
- 金額計算必須正確（含稅、含折扣）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增銷售單成功
- [ ] 新增銷售單失敗 (重複單號)
- [ ] 修改銷售單成功
- [ ] 修改銷售單失敗 (不存在)
- [ ] 刪除銷售單成功
- [ ] 查詢銷售單列表成功
- [ ] 查詢單筆銷售單成功
- [ ] 審核銷售單成功
- [ ] 出貨銷售單成功
- [ ] 取消銷售單成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 金額計算測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSD000/SYSD110_FB.ASP` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSD000/SYSD110_PR.ASP` (報表)

### 8.2 資料庫 Schema
- 參考舊系統 `IMS_AM.SO_ORDERS` 和 `IMS_AM.SO_ORDER_DETAILS` 資料表結構

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


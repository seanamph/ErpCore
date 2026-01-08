# SYSW530 - 已日結退貨單驗退調整作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW530
- **功能名稱**: 已日結退貨單驗退調整作業
- **功能描述**: 提供已日結退貨單的驗退調整功能，用於處理已日結的退貨單驗退作業，包含驗退單新增、修改、刪除、查詢功能，支援已日結退貨單的驗退數量、價格調整等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FD.asp` (刪除)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FS.asp` (儲存)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW530_PR.asp` (報表)
  - `WEB/IMS_CORE/SYSW000/SYSW530_PR.rpt` (報表定義)

### 1.2 業務需求
- 處理已日結的退貨單驗退作業
- 支援驗退單新增、修改、刪除、查詢
- 支援驗退數量、價格調整
- 支援驗退單狀態管理（草稿、已審核、已日結）
- 支援驗退單審核流程
- 支援多店別管理
- 支援驗退單報表列印
- 已日結退貨單的特殊處理邏輯

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseReceipts` (採購驗收單主檔)

```sql
CREATE TABLE [dbo].[PurchaseReceipts] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReceiptId] NVARCHAR(50) NOT NULL UNIQUE, -- 驗收單號 (SUBCHECK_ID)
    [PurchaseOrderId] NVARCHAR(50) NOT NULL, -- 採購單號 (SUBPOENA_ID)
    [PurchaseOrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (SUBPOENA_TYPE, 1:採購, 2:退貨)
    [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商代碼 (SUPPLIER_ID)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼
    [WarehouseId] NVARCHAR(50) NOT NULL, -- 庫別代碼 (WH_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [EmployeeId] NVARCHAR(50) NULL, -- 驗收人員 (EMP_ID)
    [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
    [CheckDate] DATETIME2 NULL, -- 驗收日期 (CHECK_DATE)
    [ApproveEmployeeId] NVARCHAR(50) NULL, -- 審核人員 (APRV_EMP_ID)
    [ApproveDate] DATETIME2 NULL, -- 審核日期 (APRV_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (SUBCHECK_STATUS, D:草稿, A:已審核, C:已日結)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (AMT)
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMT)
    [NonTaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 未稅金額 (NTAX_AMT)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CTIME)
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PurchaseReceipts] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_PurchaseReceipts_PurchaseOrders] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [dbo].[PurchaseOrders] ([OrderId]),
    CONSTRAINT [FK_PurchaseReceipts_Suppliers] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers] ([SupplierId]),
    CONSTRAINT [FK_PurchaseReceipts_Warehouses] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses] ([WarehouseId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_ReceiptId] ON [dbo].[PurchaseReceipts] ([ReceiptId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_PurchaseOrderId] ON [dbo].[PurchaseReceipts] ([PurchaseOrderId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_SupplierId] ON [dbo].[PurchaseReceipts] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_Status] ON [dbo].[PurchaseReceipts] ([Status]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_CheckDate] ON [dbo].[PurchaseReceipts] ([CheckDate]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_ShopId] ON [dbo].[PurchaseReceipts] ([ShopId]);
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseReceiptDetails` - 採購驗收單明細
```sql
CREATE TABLE [dbo].[PurchaseReceiptDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReceiptId] NVARCHAR(50) NOT NULL, -- 驗收單號 (SUBCHECK_ID)
    [LineNum] INT NOT NULL, -- 行號
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (GOODS_ID)
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BARCODE_ID)
    [PurchaseOrderId] NVARCHAR(50) NULL, -- 採購單號 (SUBPOENA_ID)
    [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量
    [CheckQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗收數量 (CHECK_QTY)
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價 (POE_PRICE)
    [CheckPrice] DECIMAL(18, 4) NULL, -- 驗收單價 (CHK_PRICE)
    [TaxCode] NVARCHAR(10) NULL, -- 稅別 (TAX_CODE)
    [NonTaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 未稅金額 (NTAX_AMT)
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMT)
    [Amount] DECIMAL(18, 4) NULL DEFAULT 0, -- 金額 (AMT)
    [SalesAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 銷售金額 (SALES_AMT)
    [SalesAmountTotal] DECIMAL(18, 4) NULL DEFAULT 0, -- 銷售總金額 (SALES_AMT_TOL)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PurchaseReceiptDetails_PurchaseReceipts] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[PurchaseReceipts] ([ReceiptId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseReceiptDetails_Products] FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseReceiptDetails_ReceiptId] ON [dbo].[PurchaseReceiptDetails] ([ReceiptId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceiptDetails_GoodsId] ON [dbo].[PurchaseReceiptDetails] ([GoodsId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceiptDetails_PurchaseOrderId] ON [dbo].[PurchaseReceiptDetails] ([PurchaseOrderId]);
```

#### 2.2.2 `PurchaseOrders` - 採購單主檔
- 參考: `開發計劃/04-採購管理/SYSW315-訂退貨申請作業.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ReceiptId | NVARCHAR | 50 | NO | - | 驗收單號 | 唯一，SUBCHECK_ID |
| PurchaseOrderId | NVARCHAR | 50 | NO | - | 採購單號 | 外鍵至PurchaseOrders，SUBPOENA_ID |
| PurchaseOrderType | NVARCHAR | 20 | NO | - | 單據類型 | 1:採購, 2:退貨 |
| SupplierId | NVARCHAR | 50 | NO | - | 供應商代碼 | 外鍵至Suppliers |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至Shops |
| WarehouseId | NVARCHAR | 50 | NO | - | 庫別代碼 | 外鍵至Warehouses |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | 外鍵至Organizations |
| EmployeeId | NVARCHAR | 50 | YES | - | 驗收人員 | 外鍵至Users |
| ApplyDate | DATETIME2 | - | YES | - | 申請日期 | - |
| CheckDate | DATETIME2 | - | YES | - | 驗收日期 | - |
| ApproveEmployeeId | NVARCHAR | 50 | YES | - | 審核人員 | 外鍵至Users |
| ApproveDate | DATETIME2 | - | YES | - | 審核日期 | - |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, A:已審核, C:已日結 |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TaxAmount | DECIMAL | 18,4 | YES | 0 | 稅額 | - |
| NonTaxAmount | DECIMAL | 18,4 | YES | 0 | 未稅金額 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢已日結退貨單驗退調整列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments`
- **說明**: 查詢已日結退貨單的驗退調整列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ReceiptId",
    "sortOrder": "DESC",
    "filters": {
      "receiptId": "",
      "purchaseOrderId": "",
      "supplierId": "",
      "shopId": "",
      "warehouseId": "",
      "status": "",
      "applyDateFrom": "",
      "applyDateTo": "",
      "checkDateFrom": "",
      "checkDateTo": "",
      "approveDateFrom": "",
      "approveDateTo": ""
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
          "receiptId": "RC001",
          "purchaseOrderId": "PO001",
          "purchaseOrderType": "2",
          "supplierId": "SUP001",
          "supplierName": "供應商A",
          "shopId": "SHOP001",
          "warehouseId": "WH001",
          "warehouseName": "主倉庫",
          "orgId": "ORG001",
          "employeeId": "U001",
          "applyDate": "2024-01-01",
          "checkDate": "2024-01-02",
          "approveEmployeeId": "U002",
          "approveDate": "2024-01-03",
          "status": "C",
          "totalAmount": 10000.00,
          "taxAmount": 500.00,
          "nonTaxAmount": 9500.00,
          "notes": "備註"
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

#### 3.1.2 查詢單筆已日結退貨單驗退調整
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}`
- **說明**: 根據驗收單號查詢單筆已日結退貨單驗退調整資料（含明細）
- **路徑參數**:
  - `receiptId`: 驗收單號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "receiptId": "RC001",
      "purchaseOrderId": "PO001",
      "purchaseOrderType": "2",
      "supplierId": "SUP001",
      "supplierName": "供應商A",
      "shopId": "SHOP001",
      "warehouseId": "WH001",
      "warehouseName": "主倉庫",
      "orgId": "ORG001",
      "employeeId": "U001",
      "applyDate": "2024-01-01",
      "checkDate": "2024-01-02",
      "approveEmployeeId": "U002",
      "approveDate": "2024-01-03",
      "status": "C",
      "totalAmount": 10000.00,
      "taxAmount": 500.00,
      "nonTaxAmount": 9500.00,
      "notes": "備註",
      "details": [
        {
          "tKey": 1,
          "lineNum": 1,
          "goodsId": "G001",
          "goodsName": "商品A",
          "barcodeId": "BC001",
          "purchaseOrderId": "PO001",
          "orderQty": 100.00,
          "checkQty": 95.00,
          "unitPrice": 100.00,
          "checkPrice": 105.00,
          "taxCode": "1",
          "nonTaxAmount": 9975.00,
          "taxAmount": 498.75,
          "amount": 10473.75,
          "salesAmount": 11000.00,
          "salesAmountTotal": 10450.00,
          "notes": "明細備註"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增已日結退貨單驗退調整
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments`
- **說明**: 新增已日結退貨單的驗退調整（含明細）
- **請求格式**:
  ```json
  {
    "purchaseOrderId": "PO001",
    "supplierId": "SUP001",
    "shopId": "SHOP001",
    "warehouseId": "WH001",
    "orgId": "ORG001",
    "employeeId": "U001",
    "applyDate": "2024-01-01",
    "checkDate": "2024-01-02",
    "notes": "備註",
    "details": [
      {
        "lineNum": 1,
        "goodsId": "G001",
        "barcodeId": "BC001",
        "purchaseOrderId": "PO001",
        "orderQty": 100.00,
        "checkQty": 95.00,
        "unitPrice": 100.00,
        "checkPrice": 105.00,
        "taxCode": "1",
        "notes": "明細備註"
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
      "receiptId": "RC001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改已日結退貨單驗退調整
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}`
- **說明**: 修改已日結退貨單驗退調整（僅草稿狀態可修改）
- **路徑參數**:
  - `receiptId`: 驗收單號
- **請求格式**: 同新增，但 `receiptId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除已日結退貨單驗退調整
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}`
- **說明**: 刪除已日結退貨單驗退調整（僅草稿狀態可刪除）
- **路徑參數**:
  - `receiptId`: 驗收單號
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

#### 3.1.6 審核已日結退貨單驗退調整
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}/approve`
- **說明**: 審核已日結退貨單驗退調整
- **路徑參數**:
  - `receiptId`: 驗收單號
- **請求格式**:
  ```json
  {
    "approveEmployeeId": "U002",
    "approveDate": "2024-01-03",
    "notes": "審核備註"
  }
  ```
- **回應格式**: 同新增

#### 3.1.7 查詢可用的已日結退貨單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/closed-return-orders`
- **說明**: 查詢可用的已日結退貨單列表，用於新增驗退調整時選擇
- **請求參數**:
  ```json
  {
    "supplierId": "",
    "warehouseId": "",
    "shopId": "",
    "status": "C"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "purchaseOrderId": "PO001",
        "supplierId": "SUP001",
        "supplierName": "供應商A",
        "warehouseId": "WH001",
        "warehouseName": "主倉庫",
        "approveDate": "2024-01-01",
        "totalQty": 100.00,
        "checkedQty": 95.00,
        "remainingQty": 5.00
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `PurchaseReceiptsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchase-receipts")]
    [Authorize]
    public class PurchaseReceiptsController : ControllerBase
    {
        private readonly IPurchaseReceiptService _purchaseReceiptService;
        
        public PurchaseReceiptsController(IPurchaseReceiptService purchaseReceiptService)
        {
            _purchaseReceiptService = purchaseReceiptService;
        }
        
        [HttpGet("closed-return-adjustments")]
        public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReceiptDto>>>> GetClosedReturnAdjustments([FromQuery] PurchaseReceiptQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("closed-return-adjustments/{receiptId}")]
        public async Task<ActionResult<ApiResponse<PurchaseReceiptDto>>> GetClosedReturnAdjustment(string receiptId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost("closed-return-adjustments")]
        public async Task<ActionResult<ApiResponse<string>>> CreateClosedReturnAdjustment([FromBody] CreatePurchaseReceiptDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("closed-return-adjustments/{receiptId}")]
        public async Task<ActionResult<ApiResponse>> UpdateClosedReturnAdjustment(string receiptId, [FromBody] UpdatePurchaseReceiptDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("closed-return-adjustments/{receiptId}")]
        public async Task<ActionResult<ApiResponse>> DeleteClosedReturnAdjustment(string receiptId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("closed-return-adjustments/{receiptId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveClosedReturnAdjustment(string receiptId, [FromBody] ApprovePurchaseReceiptDto dto)
        {
            // 實作審核邏輯
        }
        
        [HttpGet("closed-return-orders")]
        public async Task<ActionResult<ApiResponse<List<PurchaseOrderDto>>>> GetClosedReturnOrders([FromQuery] ClosedReturnOrderQueryDto query)
        {
            // 實作查詢可用已日結退貨單邏輯
        }
    }
}
```

#### 3.2.2 Service: `PurchaseReceiptService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPurchaseReceiptService
    {
        Task<PagedResult<PurchaseReceiptDto>> GetClosedReturnAdjustmentsAsync(PurchaseReceiptQueryDto query);
        Task<PurchaseReceiptDto> GetClosedReturnAdjustmentByIdAsync(string receiptId);
        Task<string> CreateClosedReturnAdjustmentAsync(CreatePurchaseReceiptDto dto);
        Task UpdateClosedReturnAdjustmentAsync(string receiptId, UpdatePurchaseReceiptDto dto);
        Task DeleteClosedReturnAdjustmentAsync(string receiptId);
        Task ApproveClosedReturnAdjustmentAsync(string receiptId, ApprovePurchaseReceiptDto dto);
        Task<List<PurchaseOrderDto>> GetClosedReturnOrdersAsync(ClosedReturnOrderQueryDto query);
    }
}
```

#### 3.2.3 Repository: `PurchaseReceiptRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IPurchaseReceiptRepository
    {
        Task<PurchaseReceipt> GetByIdAsync(string receiptId);
        Task<PagedResult<PurchaseReceipt>> GetClosedReturnAdjustmentsPagedAsync(PurchaseReceiptQuery query);
        Task<PurchaseReceipt> CreateAsync(PurchaseReceipt receipt);
        Task<PurchaseReceipt> UpdateAsync(PurchaseReceipt receipt);
        Task DeleteAsync(string receiptId);
        Task<bool> ExistsAsync(string receiptId);
        Task<List<PurchaseOrder>> GetClosedReturnOrdersAsync(ClosedReturnOrderQuery query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 已日結退貨單驗退調整列表頁面 (`ClosedReturnAdjustmentList.vue`)
- **路徑**: `/purchase/closed-return-adjustments`
- **功能**: 顯示已日結退貨單驗退調整列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (ClosedReturnAdjustmentSearchForm)
  - 資料表格 (ClosedReturnAdjustmentDataTable)
  - 新增/修改對話框 (ClosedReturnAdjustmentDialog)
  - 刪除確認對話框

#### 4.1.2 已日結退貨單驗退調整詳細頁面 (`ClosedReturnAdjustmentDetail.vue`)
- **路徑**: `/purchase/closed-return-adjustments/:receiptId`
- **功能**: 顯示已日結退貨單驗退調整詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ClosedReturnAdjustmentSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="驗收單號">
      <el-input v-model="searchForm.receiptId" placeholder="請輸入驗收單號" />
    </el-form-item>
    <el-form-item label="採購單號">
      <el-input v-model="searchForm.purchaseOrderId" placeholder="請輸入採購單號" />
    </el-form-item>
    <el-form-item label="供應商">
      <el-select v-model="searchForm.supplierId" placeholder="請選擇供應商" filterable>
        <el-option v-for="supplier in supplierList" :key="supplier.supplierId" :label="supplier.supplierName" :value="supplier.supplierId" />
      </el-select>
    </el-form-item>
    <el-form-item label="分店">
      <el-select v-model="searchForm.shopId" placeholder="請選擇分店">
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="庫別">
      <el-select v-model="searchForm.warehouseId" placeholder="請選擇庫別">
        <el-option v-for="warehouse in warehouseList" :key="warehouse.warehouseId" :label="warehouse.warehouseName" :value="warehouse.warehouseId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="草稿" value="D" />
        <el-option label="已審核" value="A" />
        <el-option label="已日結" value="C" />
      </el-select>
    </el-form-item>
    <el-form-item label="申請日期">
      <el-date-picker
        v-model="searchForm.applyDateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
        format="YYYY/MM/DD"
        value-format="YYYY-MM-DD"
      />
    </el-form-item>
    <el-form-item label="驗收日期">
      <el-date-picker
        v-model="searchForm.checkDateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
        format="YYYY/MM/DD"
        value-format="YYYY-MM-DD"
      />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`ClosedReturnAdjustmentDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="receiptList" v-loading="loading">
      <el-table-column prop="receiptId" label="驗收單號" width="120" />
      <el-table-column prop="purchaseOrderId" label="採購單號" width="120" />
      <el-table-column prop="supplierName" label="供應商" width="150" />
      <el-table-column prop="warehouseName" label="庫別" width="120" />
      <el-table-column prop="applyDate" label="申請日期" width="100" />
      <el-table-column prop="checkDate" label="驗收日期" width="100" />
      <el-table-column prop="approveDate" label="審核日期" width="100" />
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
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.status !== 'D'">刪除</el-button>
          <el-button type="success" size="small" @click="handleApprove(row)" :disabled="row.status !== 'D'">審核</el-button>
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

#### 4.2.3 新增/修改對話框 (`ClosedReturnAdjustmentDialog.vue`)
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
          <el-form-item label="採購單號" prop="purchaseOrderId">
            <el-select v-model="form.purchaseOrderId" placeholder="請選擇採購單號" filterable @change="handlePurchaseOrderChange">
              <el-option v-for="order in purchaseOrderList" :key="order.purchaseOrderId" :label="order.purchaseOrderId" :value="order.purchaseOrderId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="供應商" prop="supplierId">
            <el-select v-model="form.supplierId" placeholder="請選擇供應商" filterable>
              <el-option v-for="supplier in supplierList" :key="supplier.supplierId" :label="supplier.supplierName" :value="supplier.supplierId" />
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
          <el-form-item label="庫別" prop="warehouseId">
            <el-select v-model="form.warehouseId" placeholder="請選擇庫別">
              <el-option v-for="warehouse in warehouseList" :key="warehouse.warehouseId" :label="warehouse.warehouseName" :value="warehouse.warehouseId" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="申請日期" prop="applyDate">
            <el-date-picker v-model="form.applyDate" type="date" placeholder="請選擇日期" format="YYYY/MM/DD" value-format="YYYY-MM-DD" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="驗收日期" prop="checkDate">
            <el-date-picker v-model="form.checkDate" type="date" placeholder="請選擇日期" format="YYYY/MM/DD" value-format="YYYY-MM-DD" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
      
      <!-- 明細表格 -->
      <el-form-item label="明細">
        <el-table :data="form.details" border>
          <el-table-column prop="lineNum" label="行號" width="60" />
          <el-table-column prop="goodsId" label="商品編號" width="120">
            <template #default="{ row, $index }">
              <el-select v-model="row.goodsId" placeholder="請選擇商品" filterable @change="handleGoodsChange($index)">
                <el-option v-for="goods in goodsList" :key="goods.goodsId" :label="goods.goodsName" :value="goods.goodsId" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="goodsName" label="商品名稱" width="150" />
          <el-table-column prop="barcodeId" label="條碼" width="120" />
          <el-table-column prop="orderQty" label="訂購數量" width="100" align="right" />
          <el-table-column prop="checkQty" label="驗收數量" width="100" align="right">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.checkQty" :min="0" :precision="4" @change="handleQtyChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="unitPrice" label="訂購單價" width="100" align="right" />
          <el-table-column prop="checkPrice" label="驗收單價" width="100" align="right">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.checkPrice" :min="0" :precision="4" @change="handlePriceChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="taxCode" label="稅別" width="80">
            <template #default="{ row, $index }">
              <el-select v-model="row.taxCode" @change="handleTaxChange($index)">
                <el-option label="應稅" value="1" />
                <el-option label="免稅" value="0" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="amount" label="金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.amount) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80" fixed="right">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" @click="handleAddDetail" style="margin-top: 10px">新增明細</el-button>
      </el-form-item>
      
      <el-form-item label="總計">
        <el-row>
          <el-col :span="8">
            <span>未稅金額: {{ formatCurrency(totalNonTaxAmount) }}</span>
          </el-col>
          <el-col :span="8">
            <span>稅額: {{ formatCurrency(totalTaxAmount) }}</span>
          </el-col>
          <el-col :span="8">
            <span>總金額: {{ formatCurrency(totalAmount) }}</span>
          </el-col>
        </el-row>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`purchaseReceipt.api.ts`)
```typescript
import request from '@/utils/request';

export interface PurchaseReceiptDto {
  tKey: number;
  receiptId: string;
  purchaseOrderId: string;
  purchaseOrderType: string;
  supplierId: string;
  supplierName?: string;
  shopId: string;
  warehouseId: string;
  warehouseName?: string;
  orgId?: string;
  employeeId?: string;
  applyDate?: string;
  checkDate?: string;
  approveEmployeeId?: string;
  approveDate?: string;
  status: string;
  totalAmount: number;
  taxAmount: number;
  nonTaxAmount: number;
  notes?: string;
  details?: PurchaseReceiptDetailDto[];
}

export interface PurchaseReceiptDetailDto {
  tKey?: number;
  lineNum: number;
  goodsId: string;
  goodsName?: string;
  barcodeId?: string;
  purchaseOrderId?: string;
  orderQty: number;
  checkQty: number;
  unitPrice: number;
  checkPrice: number;
  taxCode?: string;
  nonTaxAmount: number;
  taxAmount: number;
  amount: number;
  salesAmount?: number;
  salesAmountTotal?: number;
  notes?: string;
}

export interface PurchaseReceiptQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    receiptId?: string;
    purchaseOrderId?: string;
    supplierId?: string;
    shopId?: string;
    warehouseId?: string;
    status?: string;
    applyDateFrom?: string;
    applyDateTo?: string;
    checkDateFrom?: string;
    checkDateTo?: string;
    approveDateFrom?: string;
    approveDateTo?: string;
  };
}

export interface CreatePurchaseReceiptDto {
  purchaseOrderId: string;
  supplierId: string;
  shopId: string;
  warehouseId: string;
  orgId?: string;
  employeeId?: string;
  applyDate?: string;
  checkDate?: string;
  notes?: string;
  details: CreatePurchaseReceiptDetailDto[];
}

export interface CreatePurchaseReceiptDetailDto {
  lineNum: number;
  goodsId: string;
  barcodeId?: string;
  purchaseOrderId?: string;
  orderQty: number;
  checkQty: number;
  unitPrice: number;
  checkPrice: number;
  taxCode?: string;
  notes?: string;
}

export interface UpdatePurchaseReceiptDto extends Omit<CreatePurchaseReceiptDto, 'purchaseOrderId'> {}

export interface ApprovePurchaseReceiptDto {
  approveEmployeeId: string;
  approveDate: string;
  notes?: string;
}

// API 函數
export const getClosedReturnAdjustments = (query: PurchaseReceiptQueryDto) => {
  return request.get<ApiResponse<PagedResult<PurchaseReceiptDto>>>('/api/v1/purchase-receipts/closed-return-adjustments', { params: query });
};

export const getClosedReturnAdjustmentById = (receiptId: string) => {
  return request.get<ApiResponse<PurchaseReceiptDto>>(`/api/v1/purchase-receipts/closed-return-adjustments/${receiptId}`);
};

export const createClosedReturnAdjustment = (data: CreatePurchaseReceiptDto) => {
  return request.post<ApiResponse<string>>('/api/v1/purchase-receipts/closed-return-adjustments', data);
};

export const updateClosedReturnAdjustment = (receiptId: string, data: UpdatePurchaseReceiptDto) => {
  return request.put<ApiResponse>(`/api/v1/purchase-receipts/closed-return-adjustments/${receiptId}`, data);
};

export const deleteClosedReturnAdjustment = (receiptId: string) => {
  return request.delete<ApiResponse>(`/api/v1/purchase-receipts/closed-return-adjustments/${receiptId}`);
};

export const approveClosedReturnAdjustment = (receiptId: string, data: ApprovePurchaseReceiptDto) => {
  return request.post<ApiResponse>(`/api/v1/purchase-receipts/closed-return-adjustments/${receiptId}/approve`, data);
};

export const getClosedReturnOrders = (query: any) => {
  return request.get<ApiResponse<PurchaseOrderDto[]>>('/api/v1/purchase-receipts/closed-return-orders', { params: query });
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
- [ ] 已日結退貨單查詢邏輯
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

### 6.1 業務邏輯
- 僅能處理已日結的退貨單（PurchaseOrderType = '2' 且 Status = 'C'）
- 驗收數量不能超過訂購數量
- 驗收單價可以與訂購單價不同
- 金額計算：未稅金額 = 驗收數量 × 驗收單價，稅額 = 未稅金額 × 稅率（如果應稅），總金額 = 未稅金額 + 稅額
- 僅草稿狀態的驗退調整可以修改或刪除
- 審核後狀態變更為已審核，不能再修改

### 6.2 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 操作記錄必須記錄

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.4 資料驗證
- 驗收單號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- 驗收數量必須大於0
- 驗收單價必須大於0

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增已日結退貨單驗退調整成功
- [ ] 新增已日結退貨單驗退調整失敗 (採購單不存在)
- [ ] 修改已日結退貨單驗退調整成功
- [ ] 修改已日結退貨單驗退調整失敗 (已審核)
- [ ] 刪除已日結退貨單驗退調整成功
- [ ] 刪除已日結退貨單驗退調整失敗 (已審核)
- [ ] 查詢已日結退貨單驗退調整列表成功
- [ ] 查詢單筆已日結退貨單驗退調整成功
- [ ] 審核已日結退貨單驗退調整成功

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
- `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FB.asp` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FI.asp` (新增)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FU.asp` (修改)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FD.asp` (刪除)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FQ.asp` (查詢)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW530_FS.asp` (儲存)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW530_PR.asp` (報表)

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/SYSW000/SYSW530_PR.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


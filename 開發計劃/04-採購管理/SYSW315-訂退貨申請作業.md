# SYSW315 - 訂退貨申請作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW315
- **功能名稱**: 訂退貨申請作業
- **功能描述**: 提供訂退貨申請單的新增、修改、刪除、查詢功能，用於管理採購訂單和退貨申請，包含供應商、商品明細、數量、價格等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSW000/SYSW315_PR.rpt` (報表)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW315_*.asp` (相關功能頁面)

### 1.2 業務需求
- 管理採購訂單申請
- 管理退貨申請
- 支援供應商選擇
- 支援商品明細維護
- 支援數量、單價、金額計算
- 支援申請單狀態管理（草稿、已送出、已審核、已取消）
- 支援申請單審核流程
- 支援多店別管理
- 支援申請單報表列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseOrders` (採購單主檔)

```sql
CREATE TABLE [dbo].[PurchaseOrders] (
    [OrderId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 採購單號 (PO_NO)
    [OrderDate] DATETIME2 NOT NULL, -- 採購日期 (PO_DATE)
    [OrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (ORDER_TYPE, PO:採購, RT:退貨)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
    [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商代碼 (SUPPLIER_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, X:已取消)
    [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
    [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
    [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
    [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
    [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量 (TOTAL_QTY)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [ExpectedDate] DATETIME2 NULL, -- 預期交貨日期 (EXPECTED_DATE)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED ([OrderId] ASC)
);

-- 索引
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
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [OrderId] NVARCHAR(50) NOT NULL, -- 採購單號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
    [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量 (ORDER_QTY)
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價 (UNIT_PRICE)
    [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
    [ReceivedQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已收數量 (RECEIVED_QTY)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PurchaseOrderDetails_PurchaseOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[PurchaseOrders] ([OrderId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_OrderId] ON [dbo].[PurchaseOrderDetails] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOrderDetails_GoodsId] ON [dbo].[PurchaseOrderDetails] ([GoodsId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| OrderId | NVARCHAR | 50 | NO | - | 採購單號 | 主鍵，唯一 |
| OrderDate | DATETIME2 | - | NO | - | 採購日期 | - |
| OrderType | NVARCHAR | 20 | NO | - | 單據類型 | PO:採購, RT:退貨 |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| SupplierId | NVARCHAR | 50 | NO | - | 供應商代碼 | 外鍵至供應商表 |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, S:已送出, A:已審核, X:已取消 |
| ApplyUserId | NVARCHAR | 50 | YES | - | 申請人員 | 外鍵至使用者表 |
| ApplyDate | DATETIME2 | - | YES | - | 申請日期 | - |
| ApproveUserId | NVARCHAR | 50 | YES | - | 審核人員 | 外鍵至使用者表 |
| ApproveDate | DATETIME2 | - | YES | - | 審核日期 | - |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalQty | DECIMAL | 18,4 | YES | 0 | 總數量 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| ExpectedDate | DATETIME2 | - | YES | - | 預期交貨日期 | - |

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
          "totalAmount": 10000.00,
          "totalQty": 100.00
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
- **回應格式**: 包含主檔和明細資料

#### 3.1.3 新增採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders`
- **說明**: 新增採購單（含明細）
- **請求格式**:
  ```json
  {
    "orderDate": "2024-01-01",
    "orderType": "PO",
    "shopId": "SHOP001",
    "supplierId": "SUP001",
    "expectedDate": "2024-01-10",
    "memo": "備註",
    "details": [
      {
        "lineNum": 1,
        "goodsId": "G001",
        "barcodeId": "BC001",
        "orderQty": 10.00,
        "unitPrice": 100.00,
        "memo": "明細備註"
      }
    ]
  }
  ```

#### 3.1.4 修改採購單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-orders/{orderId}`
- **說明**: 修改採購單（僅草稿狀態可修改）
- **路徑參數**:
  - `orderId`: 採購單號
- **請求格式**: 同新增

#### 3.1.5 刪除採購單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-orders/{orderId}`
- **說明**: 刪除採購單（僅草稿狀態可刪除）
- **路徑參數**:
  - `orderId`: 採購單號

#### 3.1.6 送出採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders/{orderId}/submit`
- **說明**: 送出採購單進行審核

#### 3.1.7 審核採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders/{orderId}/approve`
- **說明**: 審核通過採購單

#### 3.1.8 取消採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders/{orderId}/cancel`
- **說明**: 取消採購單

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
        public async Task<ActionResult<ApiResponse<PurchaseOrderDetailDto>>> GetPurchaseOrder(string orderId)
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
        
        [HttpPost("{orderId}/submit")]
        public async Task<ActionResult<ApiResponse>> SubmitPurchaseOrder(string orderId)
        {
            // 實作送出邏輯
        }
        
        [HttpPost("{orderId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApprovePurchaseOrder(string orderId)
        {
            // 實作審核邏輯
        }
        
        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelPurchaseOrder(string orderId)
        {
            // 實作取消邏輯
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
        Task<PurchaseOrderDetailDto> GetPurchaseOrderByIdAsync(string orderId);
        Task<string> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task UpdatePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto);
        Task DeletePurchaseOrderAsync(string orderId);
        Task SubmitPurchaseOrderAsync(string orderId);
        Task ApprovePurchaseOrderAsync(string orderId);
        Task CancelPurchaseOrderAsync(string orderId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購單列表頁面 (`PurchaseOrderList.vue`)
- **路徑**: `/procurement/purchase-orders`
- **功能**: 顯示採購單列表，支援查詢、新增、修改、刪除、送出、審核、取消
- **主要元件**:
  - 查詢表單 (PurchaseOrderSearchForm)
  - 資料表格 (PurchaseOrderDataTable)
  - 新增/修改對話框 (PurchaseOrderDialog)
  - 刪除確認對話框
  - 送出/審核/取消按鈕

#### 4.1.2 採購單詳細頁面 (`PurchaseOrderDetail.vue`)
- **路徑**: `/procurement/purchase-orders/:orderId`
- **功能**: 顯示採購單詳細資料（含明細），支援修改、送出、審核、取消

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
        <el-option label="全部" value="" />
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
        <el-option label="全部" value="" />
        <el-option label="草稿" value="D" />
        <el-option label="已送出" value="S" />
        <el-option label="已審核" value="A" />
        <el-option label="已取消" value="X" />
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
    <el-table :data="orderList" v-loading="loading">
      <el-table-column prop="orderId" label="採購單號" width="150" />
      <el-table-column prop="orderDate" label="採購日期" width="120" />
      <el-table-column prop="orderType" label="單據類型" width="100">
        <template #default="{ row }">
          <el-tag :type="row.orderType === 'PO' ? 'primary' : 'warning'">
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
      <el-table-column label="操作" width="300" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
          <el-button v-if="row.status === 'D'" type="warning" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button v-if="row.status === 'D'" type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button v-if="row.status === 'D'" type="success" size="small" @click="handleSubmit(row)">送出</el-button>
          <el-button v-if="row.status === 'S'" type="info" size="small" @click="handleApprove(row)">審核</el-button>
          <el-button v-if="['S', 'A'].includes(row.status)" type="danger" size="small" @click="handleCancel(row)">取消</el-button>
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
            <el-input v-model="form.orderId" :disabled="isEdit" placeholder="系統自動產生" />
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
            <el-select v-model="form.supplierId" placeholder="請選擇供應商" filterable>
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
      <el-form-item label="備註" prop="memo">
        <el-input v-model="form.memo" type="textarea" :rows="2" placeholder="請輸入備註" />
      </el-form-item>
      
      <!-- 明細表格 -->
      <el-divider>商品明細</el-divider>
      <el-table :data="form.details" border>
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="goodsId" label="商品編號" width="120">
          <template #default="{ row, $index }">
            <el-select v-model="row.goodsId" placeholder="請選擇商品" filterable @change="handleGoodsChange($index)">
              <el-option v-for="goods in goodsList" :key="goods.goodsId" :label="`${goods.goodsId} - ${goods.goodsName}`" :value="goods.goodsId" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column prop="goodsName" label="商品名稱" width="200" />
        <el-table-column prop="orderQty" label="訂購數量" width="120">
          <template #default="{ row, $index }">
            <el-input-number v-model="row.orderQty" :min="0" :precision="4" @change="handleQtyChange($index)" />
          </template>
        </el-table-column>
        <el-table-column prop="unitPrice" label="單價" width="120">
          <template #default="{ row, $index }">
            <el-input-number v-model="row.unitPrice" :min="0" :precision="4" @change="handlePriceChange($index)" />
          </template>
        </el-table-column>
        <el-table-column prop="amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <div style="margin-top: 10px;">
        <el-button type="primary" @click="handleAddDetail">新增明細</el-button>
        <span style="margin-left: 20px; font-weight: bold;">
          總金額: {{ formatCurrency(totalAmount) }}
        </span>
      </div>
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
}

export interface PurchaseOrderDetailDto extends PurchaseOrderDto {
  details: PurchaseOrderDetailItemDto[];
}

export interface PurchaseOrderDetailItemDto {
  detailId?: string;
  lineNum: number;
  goodsId: string;
  goodsName?: string;
  barcodeId?: string;
  orderQty: number;
  unitPrice: number;
  amount: number;
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
  orderDate: string;
  orderType: string;
  shopId: string;
  supplierId: string;
  expectedDate?: string;
  memo?: string;
  details: CreatePurchaseOrderDetailDto[];
}

export interface CreatePurchaseOrderDetailDto {
  lineNum: number;
  goodsId: string;
  barcodeId?: string;
  orderQty: number;
  unitPrice: number;
  memo?: string;
}

export interface UpdatePurchaseOrderDto extends CreatePurchaseOrderDto {}

// API 函數
export const getPurchaseOrderList = (query: PurchaseOrderQueryDto) => {
  return request.get<ApiResponse<PagedResult<PurchaseOrderDto>>>('/api/v1/purchase-orders', { params: query });
};

export const getPurchaseOrderById = (orderId: string) => {
  return request.get<ApiResponse<PurchaseOrderDetailDto>>(`/api/v1/purchase-orders/${orderId}`);
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
  return request.post<ApiResponse>(`/api/v1/purchase-orders/${orderId}/submit`);
};

export const approvePurchaseOrder = (orderId: string) => {
  return request.post<ApiResponse>(`/api/v1/purchase-orders/${orderId}/approve`);
};

export const cancelPurchaseOrder = (orderId: string) => {
  return request.post<ApiResponse>(`/api/v1/purchase-orders/${orderId}/cancel`);
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
- [ ] 審核流程邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 明細表格開發
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
- 必須實作權限檢查（申請、審核權限分離）
- 敏感資料必須加密傳輸 (HTTPS)
- 必須記錄操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 明細資料量大時需考慮分批載入

### 6.3 資料驗證
- 採購單號必須唯一
- 必填欄位必須驗證
- 數量、單價必須大於0
- 金額計算必須正確
- 狀態轉換必須符合業務規則

### 6.4 業務邏輯
- 僅草稿狀態可修改、刪除
- 送出後不可修改，需審核
- 審核通過後不可修改
- 刪除採購單前必須檢查是否有相關驗收單
- 金額計算：金額 = 數量 × 單價
- 總金額 = 所有明細金額總和

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增採購單成功
- [ ] 新增採購單失敗 (必填欄位未填)
- [ ] 修改採購單成功
- [ ] 修改採購單失敗 (非草稿狀態)
- [ ] 刪除採購單成功
- [ ] 刪除採購單失敗 (非草稿狀態)
- [ ] 送出採購單成功
- [ ] 審核採購單成功
- [ ] 取消採購單成功
- [ ] 查詢採購單列表成功
- [ ] 查詢單筆採購單成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 審核流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSW000/SYSW315_PR.rpt`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW315_*.asp`

### 8.2 資料庫 Schema
- 參考舊系統 `RIM_SUBPOENAM` (主檔)
- 參考舊系統 `RIM_SUBPOENAD` (明細)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


# SYSP310-SYSP330 - 採購單查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSP310-SYSP330 系列
- **功能名稱**: 採購單查詢系列
- **功能描述**: 提供採購單資料的查詢、瀏覽、報表功能，包含多條件查詢、明細查詢、統計報表等資訊查詢
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP310_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP310_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP310_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP320_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP330_FQ.ASP` (查詢)

### 1.2 業務需求
- 支援多條件組合查詢採購單
- 支援採購單明細查詢
- 支援採購單統計報表
- 支援採購單匯出功能
- 支援採購單列印功能
- 支援採購單歷史記錄查詢
- 支援採購單狀態追蹤
- 支援供應商採購統計
- 支援商品採購統計
- 支援分店採購統計

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseOrders` (採購單主檔)
與 SYSP110-SYSP190 系列共用相同的資料表結構，請參考 `SYSP110-SYSP190-採購單維護系列.md` 的資料庫設計章節。

### 2.2 查詢視圖設計

#### 2.2.1 `v_PurchaseOrderQuery` - 採購單查詢視圖
```sql
CREATE VIEW [dbo].[v_PurchaseOrderQuery] AS
SELECT 
    po.[TKey],
    po.[OrderId],
    po.[OrderDate],
    po.[OrderType],
    po.[ShopId],
    s.[ShopName],
    po.[SupplierId],
    sup.[SupplierName],
    po.[Status],
    CASE po.[Status]
        WHEN 'D' THEN '草稿'
        WHEN 'S' THEN '已送出'
        WHEN 'A' THEN '已審核'
        WHEN 'X' THEN '已取消'
        WHEN 'C' THEN '已結案'
        ELSE '未知'
    END AS [StatusName],
    po.[TotalAmount],
    po.[TotalQty],
    po.[ApplyUserId],
    u1.[UserName] AS [ApplyUserName],
    po.[ApplyDate],
    po.[ApproveUserId],
    u2.[UserName] AS [ApproveUserName],
    po.[ApproveDate],
    po.[ExpectedDate],
    po.[Memo],
    po.[SiteId],
    po.[OrgId],
    po.[CurrencyId],
    po.[ExchangeRate],
    po.[CreatedBy],
    po.[CreatedAt],
    po.[UpdatedBy],
    po.[UpdatedAt],
    -- 統計欄位
    (SELECT COUNT(*) FROM [dbo].[PurchaseOrderDetails] pod WHERE pod.[OrderId] = po.[OrderId]) AS [DetailCount],
    (SELECT SUM(pod.[ReceivedQty]) FROM [dbo].[PurchaseOrderDetails] pod WHERE pod.[OrderId] = po.[OrderId]) AS [TotalReceivedQty],
    (SELECT SUM(pod.[ReturnQty]) FROM [dbo].[PurchaseOrderDetails] pod WHERE pod.[OrderId] = po.[OrderId]) AS [TotalReturnQty]
FROM [dbo].[PurchaseOrders] po
LEFT JOIN [dbo].[Shops] s ON po.[ShopId] = s.[ShopId]
LEFT JOIN [dbo].[Suppliers] sup ON po.[SupplierId] = sup.[SupplierId]
LEFT JOIN [dbo].[Users] u1 ON po.[ApplyUserId] = u1.[UserId]
LEFT JOIN [dbo].[Users] u2 ON po.[ApproveUserId] = u2.[UserId];
```

#### 2.2.2 `v_PurchaseOrderDetailQuery` - 採購單明細查詢視圖
```sql
CREATE VIEW [dbo].[v_PurchaseOrderDetailQuery] AS
SELECT 
    pod.[TKey],
    pod.[OrderId],
    po.[OrderDate],
    po.[OrderType],
    po.[ShopId],
    s.[ShopName],
    po.[SupplierId],
    sup.[SupplierName],
    po.[Status],
    pod.[LineNum],
    pod.[GoodsId],
    g.[GoodsName],
    pod.[BarcodeId],
    pod.[OrderQty],
    pod.[UnitPrice],
    pod.[Amount],
    pod.[ReceivedQty],
    pod.[ReturnQty],
    pod.[UnitId],
    pod.[TaxRate],
    pod.[TaxAmount],
    pod.[Memo],
    (pod.[OrderQty] - ISNULL(pod.[ReceivedQty], 0) + ISNULL(pod.[ReturnQty], 0)) AS [PendingQty]
FROM [dbo].[PurchaseOrderDetails] pod
INNER JOIN [dbo].[PurchaseOrders] po ON pod.[OrderId] = po.[OrderId]
LEFT JOIN [dbo].[Shops] s ON po.[ShopId] = s.[ShopId]
LEFT JOIN [dbo].[Suppliers] sup ON po.[SupplierId] = sup.[SupplierId]
LEFT JOIN [dbo].[Goods] g ON pod.[GoodsId] = g.[GoodsId];
```

### 2.3 索引設計
與 SYSP110-SYSP190 系列共用相同的索引設計，請參考 `SYSP110-SYSP190-採購單維護系列.md` 的索引設計章節。

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢採購單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders/query`
- **說明**: 多條件查詢採購單列表，支援分頁、排序、篩選
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
      "orderDateTo": "",
      "applyUserId": "",
      "approveUserId": "",
      "expectedDateFrom": "",
      "expectedDateTo": "",
      "minTotalAmount": null,
      "maxTotalAmount": null
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
          "orderTypeName": "採購",
          "shopId": "SHOP001",
          "shopName": "總店",
          "supplierId": "SUP001",
          "supplierName": "供應商A",
          "status": "A",
          "statusName": "已審核",
          "totalAmount": 100000.00,
          "totalQty": 100.00,
          "applyUserId": "U001",
          "applyUserName": "張三",
          "applyDate": "2024-01-01T10:00:00",
          "approveUserId": "U002",
          "approveUserName": "李四",
          "approveDate": "2024-01-01T11:00:00",
          "expectedDate": "2024-01-15",
          "detailCount": 5,
          "totalReceivedQty": 80.00,
          "totalReturnQty": 0.00
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

#### 3.1.2 查詢採購單明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders/{orderId}/details`
- **說明**: 查詢指定採購單的明細資料
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
      "details": [
        {
          "lineNum": 1,
          "goodsId": "G001",
          "goodsName": "商品A",
          "barcodeId": "BC001",
          "orderQty": 10.00,
          "unitPrice": 1000.00,
          "amount": 10000.00,
          "receivedQty": 8.00,
          "returnQty": 0.00,
          "pendingQty": 2.00,
          "unitId": "PCS",
          "taxRate": 5.00,
          "taxAmount": 500.00,
          "memo": "明細備註"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢採購單統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders/statistics`
- **說明**: 查詢採購單統計資料
- **請求參數**:
  ```json
  {
    "groupBy": "supplier", // supplier, shop, goods, status, date
    "dateFrom": "2024-01-01",
    "dateTo": "2024-01-31",
    "shopId": "",
    "supplierId": "",
    "status": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "summary": {
        "totalOrders": 100,
        "totalAmount": 10000000.00,
        "totalQty": 10000.00,
        "avgAmount": 100000.00
      },
      "details": [
        {
          "groupKey": "SUP001",
          "groupName": "供應商A",
          "orderCount": 50,
          "totalAmount": 5000000.00,
          "totalQty": 5000.00
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 匯出採購單查詢結果
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders/export`
- **說明**: 匯出採購單查詢結果為 Excel 檔案
- **請求格式**:
  ```json
  {
    "filters": {
      "orderId": "",
      "orderType": "",
      "shopId": "",
      "supplierId": "",
      "status": "",
      "orderDateFrom": "",
      "orderDateTo": ""
    },
    "exportType": "excel", // excel, csv, pdf
    "includeDetails": false
  }
  ```
- **回應格式**: 檔案下載

#### 3.1.5 列印採購單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders/{orderId}/print`
- **說明**: 產生採購單列印資料
- **路徑參數**:
  - `orderId`: 採購單號
- **回應格式**: PDF 檔案或 HTML 格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `PurchaseOrderQueryController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchase-orders")]
    [Authorize]
    public class PurchaseOrderQueryController : ControllerBase
    {
        private readonly IPurchaseOrderQueryService _queryService;
        
        public PurchaseOrderQueryController(IPurchaseOrderQueryService queryService)
        {
            _queryService = queryService;
        }
        
        [HttpGet("query")]
        public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderQueryDto>>>> QueryPurchaseOrders([FromQuery] PurchaseOrderQueryRequestDto request)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{orderId}/details")]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDetailQueryDto>>> GetPurchaseOrderDetails(string orderId)
        {
            // 實作明細查詢邏輯
        }
        
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<PurchaseOrderStatisticsDto>>> GetPurchaseOrderStatistics([FromQuery] PurchaseOrderStatisticsRequestDto request)
        {
            // 實作統計邏輯
        }
        
        [HttpPost("export")]
        public async Task<IActionResult> ExportPurchaseOrders([FromBody] PurchaseOrderExportRequestDto request)
        {
            // 實作匯出邏輯
        }
        
        [HttpGet("{orderId}/print")]
        public async Task<IActionResult> PrintPurchaseOrder(string orderId)
        {
            // 實作列印邏輯
        }
    }
}
```

#### 3.2.2 Service: `PurchaseOrderQueryService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPurchaseOrderQueryService
    {
        Task<PagedResult<PurchaseOrderQueryDto>> QueryPurchaseOrdersAsync(PurchaseOrderQueryRequestDto request);
        Task<PurchaseOrderDetailQueryDto> GetPurchaseOrderDetailsAsync(string orderId);
        Task<PurchaseOrderStatisticsDto> GetPurchaseOrderStatisticsAsync(PurchaseOrderStatisticsRequestDto request);
        Task<byte[]> ExportPurchaseOrdersAsync(PurchaseOrderExportRequestDto request);
        Task<byte[]> PrintPurchaseOrderAsync(string orderId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購單查詢頁面 (`PurchaseOrderQuery.vue`)
- **路徑**: `/procurement/purchase-orders/query`
- **功能**: 多條件查詢採購單，支援匯出、列印
- **主要元件**:
  - 查詢表單 (PurchaseOrderQueryForm)
  - 資料表格 (PurchaseOrderQueryTable)
  - 明細對話框 (PurchaseOrderDetailDialog)
  - 統計圖表 (PurchaseOrderStatisticsChart)
  - 匯出功能
  - 列印功能

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`PurchaseOrderQueryForm.vue`)
```vue
<template>
  <el-form :model="queryForm" inline>
    <el-form-item label="採購單號">
      <el-input v-model="queryForm.orderId" placeholder="請輸入採購單號" clearable />
    </el-form-item>
    <el-form-item label="單據類型">
      <el-select v-model="queryForm.orderType" placeholder="請選擇單據類型" clearable>
        <el-option label="全部" value="" />
        <el-option label="採購" value="PO" />
        <el-option label="退貨" value="RT" />
      </el-select>
    </el-form-item>
    <el-form-item label="分店">
      <el-select v-model="queryForm.shopId" placeholder="請選擇分店" clearable filterable>
        <el-option label="全部" value="" />
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="供應商">
      <el-select v-model="queryForm.supplierId" placeholder="請選擇供應商" clearable filterable>
        <el-option label="全部" value="" />
        <el-option v-for="supplier in supplierList" :key="supplier.supplierId" :label="supplier.supplierName" :value="supplier.supplierId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="queryForm.status" placeholder="請選擇狀態" clearable>
        <el-option label="全部" value="" />
        <el-option label="草稿" value="D" />
        <el-option label="已送出" value="S" />
        <el-option label="已審核" value="A" />
        <el-option label="已取消" value="X" />
        <el-option label="已結案" value="C" />
      </el-select>
    </el-form-item>
    <el-form-item label="採購日期">
      <el-date-picker
        v-model="queryForm.orderDateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
        format="YYYY-MM-DD"
        value-format="YYYY-MM-DD"
      />
    </el-form-item>
    <el-form-item label="申請人">
      <el-select v-model="queryForm.applyUserId" placeholder="請選擇申請人" clearable filterable>
        <el-option label="全部" value="" />
        <el-option v-for="user in userList" :key="user.userId" :label="user.userName" :value="user.userId" />
      </el-select>
    </el-form-item>
    <el-form-item label="審核人">
      <el-select v-model="queryForm.approveUserId" placeholder="請選擇審核人" clearable filterable>
        <el-option label="全部" value="" />
        <el-option v-for="user in userList" :key="user.userId" :label="user.userName" :value="user.userId" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleQuery" :loading="loading">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleExport" :loading="exporting">匯出</el-button>
      <el-button type="info" @click="handleShowStatistics">統計</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`PurchaseOrderQueryTable.vue`)
```vue
<template>
  <div>
    <el-table :data="purchaseOrderList" v-loading="loading" border stripe>
      <el-table-column type="selection" width="55" />
      <el-table-column prop="orderId" label="採購單號" width="150" fixed="left" />
      <el-table-column prop="orderDate" label="採購日期" width="120" />
      <el-table-column prop="orderTypeName" label="單據類型" width="100" />
      <el-table-column prop="shopName" label="分店" width="120" />
      <el-table-column prop="supplierName" label="供應商" width="150" />
      <el-table-column prop="statusName" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ row.statusName }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="totalAmount" label="總金額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.totalAmount) }}
        </template>
      </el-table-column>
      <el-table-column prop="totalQty" label="總數量" width="120" align="right" />
      <el-table-column prop="detailCount" label="明細筆數" width="100" align="center" />
      <el-table-column prop="totalReceivedQty" label="已收數量" width="120" align="right" />
      <el-table-column prop="totalReturnQty" label="已退數量" width="120" align="right" />
      <el-table-column prop="applyUserName" label="申請人" width="100" />
      <el-table-column prop="applyDate" label="申請日期" width="160" />
      <el-table-column prop="approveUserName" label="審核人" width="100" />
      <el-table-column prop="approveDate" label="審核日期" width="160" />
      <el-table-column prop="expectedDate" label="預期交貨日期" width="120" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleViewDetails(row)">明細</el-button>
          <el-button type="success" size="small" @click="handlePrint(row)">列印</el-button>
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

### 4.3 API 呼叫 (`purchaseOrderQuery.api.ts`)
```typescript
import request from '@/utils/request';

export interface PurchaseOrderQueryDto {
  orderId: string;
  orderDate: string;
  orderType: string;
  orderTypeName: string;
  shopId: string;
  shopName: string;
  supplierId: string;
  supplierName: string;
  status: string;
  statusName: string;
  totalAmount: number;
  totalQty: number;
  applyUserId: string;
  applyUserName: string;
  applyDate: string;
  approveUserId: string;
  approveUserName: string;
  approveDate: string;
  expectedDate: string;
  detailCount: number;
  totalReceivedQty: number;
  totalReturnQty: number;
}

export interface PurchaseOrderQueryRequestDto {
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
    applyUserId?: string;
    approveUserId?: string;
    expectedDateFrom?: string;
    expectedDateTo?: string;
    minTotalAmount?: number;
    maxTotalAmount?: number;
  };
}

export interface PurchaseOrderDetailQueryDto {
  orderId: string;
  details: Array<{
    lineNum: number;
    goodsId: string;
    goodsName: string;
    barcodeId?: string;
    orderQty: number;
    unitPrice?: number;
    amount?: number;
    receivedQty?: number;
    returnQty?: number;
    pendingQty?: number;
    unitId?: string;
    taxRate?: number;
    taxAmount?: number;
    memo?: string;
  }>;
}

export interface PurchaseOrderStatisticsDto {
  summary: {
    totalOrders: number;
    totalAmount: number;
    totalQty: number;
    avgAmount: number;
  };
  details: Array<{
    groupKey: string;
    groupName: string;
    orderCount: number;
    totalAmount: number;
    totalQty: number;
  }>;
}

export interface PurchaseOrderStatisticsRequestDto {
  groupBy: 'supplier' | 'shop' | 'goods' | 'status' | 'date';
  dateFrom?: string;
  dateTo?: string;
  shopId?: string;
  supplierId?: string;
  status?: string;
}

export interface PurchaseOrderExportRequestDto {
  filters?: {
    orderId?: string;
    orderType?: string;
    shopId?: string;
    supplierId?: string;
    status?: string;
    orderDateFrom?: string;
    orderDateTo?: string;
  };
  exportType: 'excel' | 'csv' | 'pdf';
  includeDetails: boolean;
}

// API 函數
export const queryPurchaseOrders = (request: PurchaseOrderQueryRequestDto) => {
  return request.get<ApiResponse<PagedResult<PurchaseOrderQueryDto>>>('/api/v1/purchase-orders/query', { params: request });
};

export const getPurchaseOrderDetails = (orderId: string) => {
  return request.get<ApiResponse<PurchaseOrderDetailQueryDto>>(`/api/v1/purchase-orders/${orderId}/details`);
};

export const getPurchaseOrderStatistics = (request: PurchaseOrderStatisticsRequestDto) => {
  return request.get<ApiResponse<PurchaseOrderStatisticsDto>>('/api/v1/purchase-orders/statistics', { params: request });
};

export const exportPurchaseOrders = (request: PurchaseOrderExportRequestDto) => {
  return request.post('/api/v1/purchase-orders/export', request, {
    responseType: 'blob'
  });
};

export const printPurchaseOrder = (orderId: string) => {
  return request.get(`/api/v1/purchase-orders/${orderId}/print`, {
    responseType: 'blob'
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立查詢視圖
- [ ] 建立統計視圖
- [ ] 優化查詢索引
- [ ] 資料庫效能測試

### 5.2 階段二: 後端開發 (4天)
- [ ] Query Service 實作
- [ ] Statistics Service 實作
- [ ] Export Service 實作
- [ ] Print Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 明細對話框開發
- [ ] 統計圖表開發
- [ ] 匯出功能開發
- [ ] 列印功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 查詢功能測試
- [ ] 統計功能測試
- [ ] 匯出功能測試
- [ ] 列印功能測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 效能優化
- 大量資料查詢必須使用分頁
- 複雜查詢必須使用視圖或預存程序
- 必須建立適當的索引
- 必須使用快取機制（如 Redis）
- 統計查詢必須使用彙總視圖

### 6.2 查詢功能
- 支援多條件組合查詢
- 支援模糊查詢（採購單號、供應商名稱等）
- 支援日期範圍查詢
- 支援金額範圍查詢
- 支援排序功能
- 支援分頁功能

### 6.3 匯出功能
- 支援 Excel 格式匯出
- 支援 CSV 格式匯出
- 支援 PDF 格式匯出
- 支援大量資料分批匯出
- 支援自訂欄位匯出

### 6.4 列印功能
- 支援單筆採購單列印
- 支援批次採購單列印
- 支援 PDF 格式列印
- 支援自訂列印格式
- 支援列印預覽

### 6.5 統計功能
- 支援按供應商統計
- 支援按分店統計
- 支援按商品統計
- 支援按狀態統計
- 支援按日期統計
- 支援圖表顯示

---

## 七、測試案例

### 7.1 查詢功能測試
- [ ] 單一條件查詢成功
- [ ] 多條件組合查詢成功
- [ ] 模糊查詢成功
- [ ] 日期範圍查詢成功
- [ ] 金額範圍查詢成功
- [ ] 分頁查詢成功
- [ ] 排序查詢成功
- [ ] 無資料查詢處理

### 7.2 明細查詢測試
- [ ] 查詢單筆採購單明細成功
- [ ] 明細資料完整性驗證
- [ ] 明細計算欄位驗證（待收數量等）

### 7.3 統計功能測試
- [ ] 按供應商統計成功
- [ ] 按分店統計成功
- [ ] 按商品統計成功
- [ ] 按狀態統計成功
- [ ] 按日期統計成功
- [ ] 統計資料正確性驗證

### 7.4 匯出功能測試
- [ ] Excel 格式匯出成功
- [ ] CSV 格式匯出成功
- [ ] PDF 格式匯出成功
- [ ] 大量資料匯出成功
- [ ] 匯出資料完整性驗證

### 7.5 列印功能測試
- [ ] 單筆採購單列印成功
- [ ] 批次採購單列印成功
- [ ] PDF 格式列印成功
- [ ] 列印格式正確性驗證

### 7.6 效能測試
- [ ] 大量資料查詢效能測試
- [ ] 複雜查詢效能測試
- [ ] 統計查詢效能測試
- [ ] 匯出效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSP310_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP310_FB.ASP` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP310_PR.ASP` (報表)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP320_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSP000/SYSP330_FQ.ASP` (查詢)

### 8.2 相關功能
- `開發計劃/27-採購供應商管理/01-採購管理/SYSP110-SYSP190-採購單維護系列.md`
- `開發計劃/27-採購供應商管理/05-採購報表/SYSP410-SYSP4I0-採購報表查詢系列.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


# SYSA297 - 耗材出售維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSA297
- **功能名稱**: 耗材出售維護作業
- **功能描述**: 提供耗材出售單的新增、修改、刪除、查詢功能，包含耗材出售單主檔和明細檔的維護，支援與EIP系統整合
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSA000/SYSA297.ascx` (使用者控制項)
  - `WEB/IMS_CORE/SYSA000/SYSA297.ascx.cs` (後端邏輯)
  - `WEB/IMS_CORE/SYSA000/js/SYSA297.js` (前端邏輯)

### 1.2 業務需求
- 管理耗材出售單主檔和明細檔
- 支援耗材出售單的新增、修改、刪除、查詢
- 支援耗材出售單的審核流程
- 支援與EIP系統整合
- 支援庫存扣減
- 支援出售金額計算
- 支援稅額計算
- 記錄操作日誌

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ConsumableSales` (耗材出售單主檔)

```sql
CREATE TABLE [dbo].[ConsumableSales] (
    [TxnNo] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 交易單號 (TXN_NO)
    [Rrn] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- 唯一識別碼 (RRN)
    [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SITE_ID)
    [PurchaseDate] DATETIME2 NOT NULL, -- 出售日期 (PURCHASE_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS, 1:待審核, 2:已審核, 3:已取消)
    [TotalAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 總金額
    [TaxAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 稅額
    [NetAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 未稅金額
    [ApplyCount] INT NULL DEFAULT 0, -- 申請數量 (APPLY_COUNT)
    [DetailCount] INT NULL DEFAULT 0, -- 明細數量 (DETAIL_COUNT)
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    [ApprovedBy] NVARCHAR(50) NULL, -- 審核者
    [ApprovedAt] DATETIME2 NULL, -- 審核時間
    CONSTRAINT [PK_ConsumableSales] PRIMARY KEY CLUSTERED ([TxnNo] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ConsumableSales_SiteId] ON [dbo].[ConsumableSales] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_ConsumableSales_PurchaseDate] ON [dbo].[ConsumableSales] ([PurchaseDate]);
CREATE NONCLUSTERED INDEX [IX_ConsumableSales_Status] ON [dbo].[ConsumableSales] ([Status]);
CREATE NONCLUSTERED INDEX [IX_ConsumableSales_Rrn] ON [dbo].[ConsumableSales] ([Rrn]);
```

### 2.2 相關資料表

#### 2.2.1 `ConsumableSalesDetails` - 耗材出售單明細檔
```sql
CREATE TABLE [dbo].[ConsumableSalesDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [TxnNo] NVARCHAR(50) NOT NULL, -- 交易單號
    [SeqNo] INT NOT NULL, -- 序號
    [ConsumableId] NVARCHAR(50) NOT NULL, -- 耗材編號 (GOODS_ID)
    [ConsumableName] NVARCHAR(200) NULL, -- 耗材名稱
    [Quantity] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 數量 (CHECK_QTY)
    [Unit] NVARCHAR(20) NULL, -- 單位
    [UnitPrice] DECIMAL(18, 2) NULL DEFAULT 0, -- 單價
    [Amount] DECIMAL(18, 2) NULL DEFAULT 0, -- 金額
    [Tax] NVARCHAR(10) NULL DEFAULT '1', -- 稅別 (1:應稅, 0:免稅)
    [TaxAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 稅額
    [NetAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 未稅金額 (NTAX_AMT)
    [PurchaseStatus] NVARCHAR(10) NULL DEFAULT '1', -- 採購驗收狀態 (PURCHASE_STATUS, 1:已驗收)
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ConsumableSalesDetails] PRIMARY KEY CLUSTERED ([DetailId] ASC),
    CONSTRAINT [FK_ConsumableSalesDetails_ConsumableSales] FOREIGN KEY ([TxnNo]) REFERENCES [dbo].[ConsumableSales] ([TxnNo]) ON DELETE CASCADE,
    CONSTRAINT [FK_ConsumableSalesDetails_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ConsumableSalesDetails_TxnNo] ON [dbo].[ConsumableSalesDetails] ([TxnNo]);
CREATE NONCLUSTERED INDEX [IX_ConsumableSalesDetails_ConsumableId] ON [dbo].[ConsumableSalesDetails] ([ConsumableId]);
```

#### 2.2.2 `Consumables` - 耗材主檔
- 參考: `SYSA254-耗材標籤列印作業.md`

#### 2.2.3 `Sites` - 店別主檔
- 用於查詢店別列表

#### 2.2.4 `ConsumableInventory` - 耗材庫存
```sql
CREATE TABLE [dbo].[ConsumableInventory] (
    [InventoryId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ConsumableId] NVARCHAR(50) NOT NULL,
    [SiteId] NVARCHAR(50) NOT NULL,
    [Quantity] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 庫存數量
    [ReservedQuantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 預留數量
    [AvailableQuantity] AS ([Quantity] - [ReservedQuantity]), -- 可用數量
    [LastUpdated] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ConsumableInventory] PRIMARY KEY CLUSTERED ([InventoryId] ASC),
    CONSTRAINT [FK_ConsumableInventory_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId]),
    CONSTRAINT [FK_ConsumableInventory_Sites] FOREIGN KEY ([SiteId]) REFERENCES [dbo].[Sites] ([SiteId]),
    CONSTRAINT [UQ_ConsumableInventory_Consumable_Site] UNIQUE ([ConsumableId], [SiteId])
);
```

### 2.3 資料字典

#### ConsumableSales 資料表
| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TxnNo | NVARCHAR | 50 | NO | - | 交易單號 | 主鍵，格式: SEL+日期+店別 |
| Rrn | UNIQUEIDENTIFIER | - | NO | NEWID() | 唯一識別碼 | - |
| SiteId | NVARCHAR | 50 | NO | - | 店別代碼 | 外鍵至Sites |
| PurchaseDate | DATETIME2 | - | NO | - | 出售日期 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:待審核, 2:已審核, 3:已取消 |
| TotalAmount | DECIMAL | 18,2 | YES | 0 | 總金額 | - |
| TaxAmount | DECIMAL | 18,2 | YES | 0 | 稅額 | - |
| NetAmount | DECIMAL | 18,2 | YES | 0 | 未稅金額 | - |
| ApplyCount | INT | - | YES | 0 | 申請數量 | - |
| DetailCount | INT | - | YES | 0 | 明細數量 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| ApprovedBy | NVARCHAR | 50 | YES | - | 審核者 | - |
| ApprovedAt | DATETIME2 | - | YES | - | 審核時間 | - |

#### ConsumableSalesDetails 資料表
| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| DetailId | UNIQUEIDENTIFIER | - | NO | NEWID() | 明細ID | 主鍵 |
| TxnNo | NVARCHAR | 50 | NO | - | 交易單號 | 外鍵至ConsumableSales |
| SeqNo | INT | - | NO | - | 序號 | - |
| ConsumableId | NVARCHAR | 50 | NO | - | 耗材編號 | 外鍵至Consumables |
| ConsumableName | NVARCHAR | 200 | YES | - | 耗材名稱 | - |
| Quantity | DECIMAL | 18,2 | NO | 0 | 數量 | - |
| Unit | NVARCHAR | 20 | YES | - | 單位 | - |
| UnitPrice | DECIMAL | 18,2 | YES | 0 | 單價 | - |
| Amount | DECIMAL | 18,2 | YES | 0 | 金額 | - |
| Tax | NVARCHAR | 10 | YES | '1' | 稅別 | 1:應稅, 0:免稅 |
| TaxAmount | DECIMAL | 18,2 | YES | 0 | 稅額 | - |
| NetAmount | DECIMAL | 18,2 | YES | 0 | 未稅金額 | - |
| PurchaseStatus | NVARCHAR | 10 | YES | '1' | 採購驗收狀態 | 1:已驗收 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢耗材出售單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/consumable-sales`
- **說明**: 查詢耗材出售單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "PurchaseDate",
    "sortOrder": "DESC",
    "filters": {
      "txnNo": "",
      "siteId": "",
      "status": "",
      "startDate": "",
      "endDate": ""
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
          "txnNo": "SEL20240101001",
          "siteId": "SITE001",
          "siteName": "店別名稱",
          "purchaseDate": "2024-01-01",
          "status": "1",
          "statusName": "待審核",
          "totalAmount": 1000.00,
          "taxAmount": 50.00,
          "netAmount": 950.00,
          "detailCount": 5,
          "createdBy": "U001",
          "createdAt": "2024-01-01T10:00:00"
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

#### 3.1.2 查詢單筆耗材出售單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/consumable-sales/{txnNo}`
- **說明**: 根據交易單號查詢單筆耗材出售單資料（含明細）
- **路徑參數**:
  - `txnNo`: 交易單號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "txnNo": "SEL20240101001",
      "rrn": "guid",
      "siteId": "SITE001",
      "siteName": "店別名稱",
      "purchaseDate": "2024-01-01",
      "status": "1",
      "statusName": "待審核",
      "totalAmount": 1000.00,
      "taxAmount": 50.00,
      "netAmount": 950.00,
      "applyCount": 5,
      "detailCount": 5,
      "notes": "備註",
      "details": [
        {
          "detailId": "guid",
          "seqNo": 1,
          "consumableId": "C001",
          "consumableName": "耗材名稱",
          "quantity": 10.00,
          "unit": "個",
          "unitPrice": 100.00,
          "amount": 1000.00,
          "tax": "1",
          "taxAmount": 50.00,
          "netAmount": 950.00,
          "purchaseStatus": "1"
        }
      ],
      "createdBy": "U001",
      "createdAt": "2024-01-01T10:00:00",
      "updatedBy": "U001",
      "updatedAt": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增耗材出售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/consumable-sales`
- **說明**: 新增耗材出售單
- **請求格式**:
  ```json
  {
    "siteId": "SITE001",
    "purchaseDate": "2024-01-01",
    "notes": "備註",
    "details": [
      {
        "consumableId": "C001",
        "quantity": 10.00,
        "unitPrice": 100.00,
        "tax": "1",
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
      "txnNo": "SEL20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改耗材出售單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/consumable-sales/{txnNo}`
- **說明**: 修改耗材出售單（僅限待審核狀態）
- **路徑參數**:
  - `txnNo`: 交易單號
- **請求格式**: 同新增，但 `txnNo` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除耗材出售單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/consumable-sales/{txnNo}`
- **說明**: 刪除耗材出售單（僅限待審核狀態）
- **路徑參數**:
  - `txnNo`: 交易單號
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

#### 3.1.6 審核耗材出售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/consumable-sales/{txnNo}/approve`
- **說明**: 審核耗材出售單，扣減庫存
- **路徑參數**:
  - `txnNo`: 交易單號
- **請求格式**:
  ```json
  {
    "approved": true,
    "notes": "審核備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "審核成功",
    "data": {
      "txnNo": "SEL20240101001",
      "status": "2"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 取消耗材出售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/consumable-sales/{txnNo}/cancel`
- **說明**: 取消耗材出售單（僅限待審核狀態）
- **路徑參數**:
  - `txnNo`: 交易單號
- **回應格式**: 同審核

### 3.2 後端實作類別

#### 3.2.1 Controller: `ConsumableSalesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/consumable-sales")]
    [Authorize]
    public class ConsumableSalesController : ControllerBase
    {
        private readonly IConsumableSalesService _salesService;
        
        public ConsumableSalesController(IConsumableSalesService salesService)
        {
            _salesService = salesService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ConsumableSalesDto>>>> GetSales([FromQuery] ConsumableSalesQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{txnNo}")]
        public async Task<ActionResult<ApiResponse<ConsumableSalesDetailDto>>> GetSalesDetail(string txnNo)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateSales([FromBody] CreateConsumableSalesDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{txnNo}")]
        public async Task<ActionResult<ApiResponse>> UpdateSales(string txnNo, [FromBody] UpdateConsumableSalesDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{txnNo}")]
        public async Task<ActionResult<ApiResponse>> DeleteSales(string txnNo)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{txnNo}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveSales(string txnNo, [FromBody] ApproveSalesDto dto)
        {
            // 實作審核邏輯
        }
        
        [HttpPost("{txnNo}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelSales(string txnNo)
        {
            // 實作取消邏輯
        }
    }
}
```

#### 3.2.2 Service: `ConsumableSalesService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IConsumableSalesService
    {
        Task<PagedResult<ConsumableSalesDto>> GetSalesAsync(ConsumableSalesQueryDto query);
        Task<ConsumableSalesDetailDto> GetSalesDetailAsync(string txnNo);
        Task<string> CreateSalesAsync(CreateConsumableSalesDto dto, string userId);
        Task UpdateSalesAsync(string txnNo, UpdateConsumableSalesDto dto, string userId);
        Task DeleteSalesAsync(string txnNo, string userId);
        Task ApproveSalesAsync(string txnNo, ApproveSalesDto dto, string userId);
        Task CancelSalesAsync(string txnNo, string userId);
        Task<string> GenerateTxnNoAsync(string siteId, DateTime date);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 耗材出售單列表頁面 (`ConsumableSalesList.vue`)
- **路徑**: `/analysis/consumables/sales`
- **功能**: 顯示耗材出售單列表，支援查詢、新增、修改、刪除、審核
- **主要元件**:
  - 查詢表單 (ConsumableSalesSearchForm)
  - 資料表格 (ConsumableSalesDataTable)
  - 新增/修改對話框 (ConsumableSalesDialog)
  - 審核對話框 (ApproveDialog)

#### 4.1.2 耗材出售單詳細頁面 (`ConsumableSalesDetail.vue`)
- **路徑**: `/analysis/consumables/sales/:txnNo`
- **功能**: 顯示耗材出售單詳細資料，支援修改、審核

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ConsumableSalesSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="交易單號">
      <el-input v-model="searchForm.txnNo" placeholder="請輸入交易單號" />
    </el-form-item>
    <el-form-item label="店別">
      <el-select v-model="searchForm.siteId" placeholder="請選擇店別">
        <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="待審核" value="1" />
        <el-option label="已審核" value="2" />
        <el-option label="已取消" value="3" />
      </el-select>
    </el-form-item>
    <el-form-item label="出售日期">
      <el-date-picker
        v-model="searchForm.dateRange"
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

#### 4.2.2 資料表格元件 (`ConsumableSalesDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="salesList" v-loading="loading">
      <el-table-column prop="txnNo" label="交易單號" width="150" />
      <el-table-column prop="siteName" label="店別" width="120" />
      <el-table-column prop="purchaseDate" label="出售日期" width="120" />
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
      <el-table-column prop="detailCount" label="明細數量" width="100" align="center" />
      <el-table-column prop="createdBy" label="建立者" width="100" />
      <el-table-column prop="createdAt" label="建立時間" width="160" />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
          <el-button type="warning" size="small" @click="handleEdit(row)" v-if="row.status === '1'">修改</el-button>
          <el-button type="success" size="small" @click="handleApprove(row)" v-if="row.status === '1'">審核</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" v-if="row.status === '1'">刪除</el-button>
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

#### 4.2.3 新增/修改對話框 (`ConsumableSalesDialog.vue`)
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
          <el-form-item label="店別" prop="siteId">
            <el-select v-model="form.siteId" placeholder="請選擇店別" :disabled="isEdit">
              <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="出售日期" prop="purchaseDate">
            <el-date-picker v-model="form.purchaseDate" type="date" placeholder="請選擇日期" :disabled="isEdit" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
      <el-form-item label="明細">
        <el-table :data="form.details" border>
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column label="耗材編號" width="120">
            <template #default="{ row, $index }">
              <el-select v-model="row.consumableId" placeholder="請選擇耗材" @change="handleConsumableChange($index)">
                <el-option v-for="consumable in consumableList" :key="consumable.consumableId" :label="consumable.consumableName" :value="consumable.consumableId" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="耗材名稱" width="200">
            <template #default="{ row }">
              {{ row.consumableName }}
            </template>
          </el-table-column>
          <el-table-column label="數量" width="120">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.quantity" :min="0.01" :precision="2" @change="handleDetailChange($index)" />
            </template>
          </el-table-column>
          <el-table-column label="單位" width="80">
            <template #default="{ row }">
              {{ row.unit }}
            </template>
          </el-table-column>
          <el-table-column label="單價" width="120">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.unitPrice" :min="0" :precision="2" @change="handleDetailChange($index)" />
            </template>
          </el-table-column>
          <el-table-column label="稅別" width="100">
            <template #default="{ row, $index }">
              <el-select v-model="row.tax" @change="handleDetailChange($index)">
                <el-option label="應稅" value="1" />
                <el-option label="免稅" value="0" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.amount) }}
            </template>
          </el-table-column>
          <el-table-column label="稅額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.taxAmount) }}
            </template>
          </el-table-column>
          <el-table-column label="未稅金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.netAmount) }}
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
      <el-form-item label="合計">
        <el-row :gutter="20">
          <el-col :span="8">
            <span>總金額: <strong>{{ formatCurrency(totalAmount) }}</strong></span>
          </el-col>
          <el-col :span="8">
            <span>稅額: <strong>{{ formatCurrency(taxAmount) }}</strong></span>
          </el-col>
          <el-col :span="8">
            <span>未稅金額: <strong>{{ formatCurrency(netAmount) }}</strong></span>
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

### 4.3 API 呼叫 (`consumable-sales.api.ts`)
```typescript
import request from '@/utils/request';

export interface ConsumableSalesDto {
  txnNo: string;
  rrn: string;
  siteId: string;
  siteName?: string;
  purchaseDate: string;
  status: string;
  statusName?: string;
  totalAmount: number;
  taxAmount: number;
  netAmount: number;
  applyCount?: number;
  detailCount: number;
  notes?: string;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
  approvedBy?: string;
  approvedAt?: string;
}

export interface ConsumableSalesDetailDto extends ConsumableSalesDto {
  details: ConsumableSalesDetailItem[];
}

export interface ConsumableSalesDetailItem {
  detailId?: string;
  seqNo: number;
  consumableId: string;
  consumableName?: string;
  quantity: number;
  unit?: string;
  unitPrice: number;
  amount: number;
  tax: string;
  taxAmount: number;
  netAmount: number;
  purchaseStatus?: string;
  notes?: string;
}

export interface CreateConsumableSalesDto {
  siteId: string;
  purchaseDate: string;
  notes?: string;
  details: CreateConsumableSalesDetailDto[];
}

export interface CreateConsumableSalesDetailDto {
  consumableId: string;
  quantity: number;
  unitPrice: number;
  tax: string;
  notes?: string;
}

export interface UpdateConsumableSalesDto extends Omit<CreateConsumableSalesDto, 'siteId' | 'purchaseDate'> {}

export interface ConsumableSalesQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    txnNo?: string;
    siteId?: string;
    status?: string;
    startDate?: string;
    endDate?: string;
  };
}

// API 函數
export const getConsumableSalesList = (query: ConsumableSalesQueryDto) => {
  return request.get<ApiResponse<PagedResult<ConsumableSalesDto>>>('/api/v1/consumable-sales', { params: query });
};

export const getConsumableSalesDetail = (txnNo: string) => {
  return request.get<ApiResponse<ConsumableSalesDetailDto>>(`/api/v1/consumable-sales/${txnNo}`);
};

export const createConsumableSales = (data: CreateConsumableSalesDto) => {
  return request.post<ApiResponse<{ txnNo: string }>>('/api/v1/consumable-sales', data);
};

export const updateConsumableSales = (txnNo: string, data: UpdateConsumableSalesDto) => {
  return request.put<ApiResponse>(`/api/v1/consumable-sales/${txnNo}`, data);
};

export const deleteConsumableSales = (txnNo: string) => {
  return request.delete<ApiResponse>(`/api/v1/consumable-sales/${txnNo}`);
};

export const approveConsumableSales = (txnNo: string, data: { approved: boolean; notes?: string }) => {
  return request.post<ApiResponse>(`/api/v1/consumable-sales/${txnNo}/approve`, data);
};

export const cancelConsumableSales = (txnNo: string) => {
  return request.post<ApiResponse>(`/api/v1/consumable-sales/${txnNo}/cancel`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立耗材出售單主檔資料表
- [ ] 建立耗材出售單明細檔資料表
- [ ] 建立耗材庫存資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（含交易單號生成、金額計算、庫存扣減）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 審核流程實作
- [ ] EIP系統整合（如需要）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 明細表格開發
- [ ] 金額計算邏輯
- [ ] 審核功能開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 完整CRUD流程測試
- [ ] 審核流程測試
- [ ] 庫存扣減測試
- [ ] 金額計算測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 並發測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 交易單號生成
- 格式: SEL + 日期(yyyyMMdd) + 店別代碼 + 序號
- 必須保證唯一性
- 必須使用事務確保原子性

### 6.2 金額計算
- 金額 = 數量 × 單價
- 稅額 = 金額 × 稅率（應稅時）
- 未稅金額 = 金額 - 稅額
- 總金額 = 所有明細金額總和
- 必須使用DECIMAL類型確保精度

### 6.3 庫存管理
- 審核時必須檢查庫存是否足夠
- 審核時必須扣減庫存
- 取消時必須回補庫存
- 必須使用事務確保一致性

### 6.4 狀態管理
- 待審核狀態: 可修改、刪除
- 已審核狀態: 不可修改、刪除
- 已取消狀態: 不可修改、刪除
- 狀態變更必須記錄操作者

### 6.5 業務邏輯
- 新增時必須檢查耗材是否存在
- 修改時必須檢查狀態
- 刪除時必須檢查狀態和相關資料
- 審核時必須檢查庫存
- 必須記錄操作日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增耗材出售單成功
- [ ] 新增耗材出售單失敗 (耗材不存在)
- [ ] 修改耗材出售單成功
- [ ] 修改耗材出售單失敗 (已審核)
- [ ] 刪除耗材出售單成功
- [ ] 刪除耗材出售單失敗 (已審核)
- [ ] 審核耗材出售單成功
- [ ] 審核耗材出售單失敗 (庫存不足)
- [ ] 金額計算正確
- [ ] 交易單號生成正確

### 7.2 整合測試
- [ ] 完整CRUD流程測試
- [ ] 審核流程測試
- [ ] 庫存扣減測試
- [ ] 金額計算測試
- [ ] 錯誤處理測試
- [ ] 並發操作測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發審核測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSA000/SYSA297.ascx`
- `WEB/IMS_CORE/SYSA000/SYSA297.ascx.cs`
- `WEB/IMS_CORE/SYSA000/js/SYSA297.js`

### 8.2 相關功能
- SYSA254 - 耗材標籤列印作業
- SYSA255 - 耗材管理報表
- 庫存管理相關功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


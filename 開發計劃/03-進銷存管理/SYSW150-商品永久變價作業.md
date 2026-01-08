# SYSW150 - 商品永久變價作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW150
- **功能名稱**: 商品永久變價作業
- **功能描述**: 提供商品永久變價的新增、修改、刪除、查詢功能，包含進價變價和售價變價，支援變價單的申請、審核、確認流程
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FI.ASP` (新增 - 進價/售價變價)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FD.ASP` (刪除)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/` (業務邏輯)

### 1.2 業務需求
- 管理商品進價和售價的永久變價作業
- 支援變價單的申請、審核、確認流程
- 支援變價單的作廢功能
- 支援變價單明細的新增、修改、刪除
- 支援變價單的啟用日期設定
- 支援品牌、廠商維度的變價作業
- 支援變價單的查詢與報表列印
- 支援變價單狀態管理（已申請、已審核、已確認、已作廢）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PriceChangeMasters` (對應舊系統 `RIM_INPRICECHGM` 和 `RIM_SALEPRICECHGM`)

```sql
CREATE TABLE [dbo].[PriceChangeMasters] (
    [PriceChangeId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 變價單號 (PRICECHG_ID)
    [PriceChangeType] NVARCHAR(10) NOT NULL, -- 變價類型 (PRICECHG_CLAS: 1:進價, 2:售價)
    [SupplierId] NVARCHAR(50) NULL, -- 廠商編號 (SUPPLIER_ID)
    [LogoId] NVARCHAR(50) NULL, -- 品牌編號 (LOGO_ID)
    [ApplyEmpId] NVARCHAR(50) NULL, -- 申請人員編號 (APPLY_EMP_ID)
    [ApplyOrgId] NVARCHAR(50) NULL, -- 申請單位 (APPLY_ORG_ID)
    [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
    [StartDate] DATETIME2 NULL, -- 啟用日期 (STAR_DATE)
    [ApproveEmpId] NVARCHAR(50) NULL, -- 審核人員編號 (APRV_EMP_ID)
    [ApproveDate] DATETIME2 NULL, -- 審核日期 (APRV_DATE)
    [ConfirmEmpId] NVARCHAR(50) NULL, -- 確認人員編號 (CONF_EMP_ID)
    [ConfirmDate] DATETIME2 NULL, -- 確認日期 (CONF_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (PRICECHG_STATUS: 1:已申請, 2:已審核, 9:已作廢, 10:已確認)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (INTOLAMT/SALETOLAMT)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_PriceChangeMasters] PRIMARY KEY CLUSTERED ([PriceChangeId], [PriceChangeType])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_SupplierId] ON [dbo].[PriceChangeMasters] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_LogoId] ON [dbo].[PriceChangeMasters] ([LogoId]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_Status] ON [dbo].[PriceChangeMasters] ([Status]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_ApplyDate] ON [dbo].[PriceChangeMasters] ([ApplyDate]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeMasters_StartDate] ON [dbo].[PriceChangeMasters] ([StartDate]);
```

### 2.2 相關資料表

#### 2.2.1 `PriceChangeDetails` - 變價單明細 (對應舊系統 `RIM_INPRICECHGD` 和 `RIM_SALEPRICECHGD`)
```sql
CREATE TABLE [dbo].[PriceChangeDetails] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PriceChangeId] NVARCHAR(50) NOT NULL, -- 變價單號 (PRICECHG_ID/SALEPRICECHG_ID)
    [PriceChangeType] NVARCHAR(10) NOT NULL, -- 變價類型 (1:進價, 2:售價)
    [LineNum] INT NOT NULL, -- 序號 (LINE_NUM)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (GOODS_ID)
    [BeforePrice] DECIMAL(18, 4) NULL DEFAULT 0, -- 調整前單價 (COST_PRICE)
    [AfterPrice] DECIMAL(18, 4) NOT NULL, -- 調整後單價 (ACOST_PRICE)
    [ChangeQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 變價數量 (CHG_QTY)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [FK_PriceChangeDetails_Masters] FOREIGN KEY ([PriceChangeId], [PriceChangeType]) 
        REFERENCES [dbo].[PriceChangeMasters] ([PriceChangeId], [PriceChangeType]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PriceChangeDetails_PriceChangeId] ON [dbo].[PriceChangeDetails] ([PriceChangeId], [PriceChangeType]);
CREATE NONCLUSTERED INDEX [IX_PriceChangeDetails_GoodsId] ON [dbo].[PriceChangeDetails] ([GoodsId]);
```

#### 2.2.2 `Products` - 商品主檔
```sql
-- 用於查詢商品列表
-- 參考商品主檔設計
```

#### 2.2.3 `Suppliers` - 廠商主檔
```sql
-- 用於查詢廠商列表
-- 參考廠商主檔設計
```

#### 2.2.4 `Logos` - 品牌主檔
```sql
-- 用於查詢品牌列表
-- 參考品牌主檔設計
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| PriceChangeId | NVARCHAR | 50 | NO | - | 變價單號 | 主鍵 |
| PriceChangeType | NVARCHAR | 10 | NO | - | 變價類型 | 1:進價, 2:售價 |
| SupplierId | NVARCHAR | 50 | YES | - | 廠商編號 | 外鍵至Suppliers |
| LogoId | NVARCHAR | 50 | YES | - | 品牌編號 | 外鍵至Logos |
| ApplyEmpId | NVARCHAR | 50 | YES | - | 申請人員編號 | 外鍵至Users |
| ApplyOrgId | NVARCHAR | 50 | YES | - | 申請單位 | 外鍵至Organizations |
| ApplyDate | DATETIME2 | - | YES | - | 申請日期 | - |
| StartDate | DATETIME2 | - | YES | - | 啟用日期 | - |
| ApproveEmpId | NVARCHAR | 50 | YES | - | 審核人員編號 | 外鍵至Users |
| ApproveDate | DATETIME2 | - | YES | - | 審核日期 | - |
| ConfirmEmpId | NVARCHAR | 50 | YES | - | 確認人員編號 | 外鍵至Users |
| ConfirmDate | DATETIME2 | - | YES | - | 確認日期 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:已申請, 2:已審核, 9:已作廢, 10:已確認 |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢變價單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/price-changes`
- **說明**: 查詢變價單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ApplyDate",
    "sortOrder": "DESC",
    "filters": {
      "priceChangeId": "",
      "priceChangeType": "",
      "supplierId": "",
      "logoId": "",
      "status": "",
      "applyDateFrom": "",
      "applyDateTo": "",
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
          "priceChangeId": "PC20240101001",
          "priceChangeType": "1",
          "priceChangeTypeName": "進價",
          "supplierId": "SUP001",
          "supplierName": "供應商A",
          "logoId": "LOGO001",
          "logoName": "品牌A",
          "applyEmpId": "U001",
          "applyEmpName": "張三",
          "applyDate": "2024-01-01",
          "startDate": "2024-01-02",
          "status": "1",
          "statusName": "已申請",
          "totalAmount": 10000.00
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

#### 3.1.2 查詢單筆變價單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/price-changes/{priceChangeId}/{priceChangeType}`
- **說明**: 根據變價單號和類型查詢單筆變價單資料（含明細）
- **路徑參數**:
  - `priceChangeId`: 變價單號
  - `priceChangeType`: 變價類型 (1:進價, 2:售價)
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "priceChangeId": "PC20240101001",
      "priceChangeType": "1",
      "supplierId": "SUP001",
      "supplierName": "供應商A",
      "logoId": "LOGO001",
      "logoName": "品牌A",
      "applyEmpId": "U001",
      "applyEmpName": "張三",
      "applyOrgId": "ORG001",
      "applyDate": "2024-01-01",
      "startDate": "2024-01-02",
      "approveEmpId": null,
      "approveDate": null,
      "confirmEmpId": null,
      "confirmDate": null,
      "status": "1",
      "statusName": "已申請",
      "totalAmount": 10000.00,
      "notes": "備註",
      "details": [
        {
          "lineNum": 1,
          "goodsId": "G001",
          "goodsName": "商品A",
          "beforePrice": 100.00,
          "afterPrice": 120.00,
          "changeQty": 0,
          "notes": ""
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增變價單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/price-changes`
- **說明**: 新增變價單（含明細）
- **請求格式**:
  ```json
  {
    "priceChangeType": "1",
    "supplierId": "SUP001",
    "logoId": "LOGO001",
    "applyEmpId": "U001",
    "applyOrgId": "ORG001",
    "applyDate": "2024-01-01",
    "startDate": "2024-01-02",
    "notes": "備註",
    "details": [
      {
        "lineNum": 1,
        "goodsId": "G001",
        "beforePrice": 100.00,
        "afterPrice": 120.00,
        "changeQty": 0,
        "notes": ""
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
      "priceChangeId": "PC20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改變價單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/price-changes/{priceChangeId}/{priceChangeType}`
- **說明**: 修改變價單（僅限狀態為「已申請」的變價單）
- **路徑參數**:
  - `priceChangeId`: 變價單號
  - `priceChangeType`: 變價類型
- **請求格式**: 同新增，但 `priceChangeId` 和 `priceChangeType` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除變價單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/price-changes/{priceChangeId}/{priceChangeType}`
- **說明**: 刪除變價單（僅限狀態為「已申請」的變價單）
- **路徑參數**:
  - `priceChangeId`: 變價單號
  - `priceChangeType`: 變價類型
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

#### 3.1.6 審核變價單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/price-changes/{priceChangeId}/{priceChangeType}/approve`
- **說明**: 審核變價單（將狀態從「已申請」變更為「已審核」）
- **請求格式**:
  ```json
  {
    "approveDate": "2024-01-02"
  }
  ```

#### 3.1.7 確認變價單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/price-changes/{priceChangeId}/{priceChangeType}/confirm`
- **說明**: 確認變價單（將狀態從「已審核」變更為「已確認」，並執行變價作業）
- **請求格式**:
  ```json
  {
    "confirmDate": "2024-01-03"
  }
  ```

#### 3.1.8 作廢變價單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/price-changes/{priceChangeId}/{priceChangeType}/cancel`
- **說明**: 作廢變價單（將狀態變更為「已作廢」）
- **請求格式**: 無需參數

### 3.2 後端實作類別

#### 3.2.1 Controller: `PriceChangesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/price-changes")]
    [Authorize]
    public class PriceChangesController : ControllerBase
    {
        private readonly IPriceChangeService _priceChangeService;
        
        public PriceChangesController(IPriceChangeService priceChangeService)
        {
            _priceChangeService = priceChangeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PriceChangeDto>>>> GetPriceChanges([FromQuery] PriceChangeQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{priceChangeId}/{priceChangeType}")]
        public async Task<ActionResult<ApiResponse<PriceChangeDetailDto>>> GetPriceChange(string priceChangeId, string priceChangeType)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreatePriceChange([FromBody] CreatePriceChangeDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{priceChangeId}/{priceChangeType}")]
        public async Task<ActionResult<ApiResponse>> UpdatePriceChange(string priceChangeId, string priceChangeType, [FromBody] UpdatePriceChangeDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{priceChangeId}/{priceChangeType}")]
        public async Task<ActionResult<ApiResponse>> DeletePriceChange(string priceChangeId, string priceChangeType)
        {
            // 實作刪除邏輯
        }
        
        [HttpPut("{priceChangeId}/{priceChangeType}/approve")]
        public async Task<ActionResult<ApiResponse>> ApprovePriceChange(string priceChangeId, string priceChangeType, [FromBody] ApprovePriceChangeDto dto)
        {
            // 實作審核邏輯
        }
        
        [HttpPut("{priceChangeId}/{priceChangeType}/confirm")]
        public async Task<ActionResult<ApiResponse>> ConfirmPriceChange(string priceChangeId, string priceChangeType, [FromBody] ConfirmPriceChangeDto dto)
        {
            // 實作確認邏輯
        }
        
        [HttpPut("{priceChangeId}/{priceChangeType}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelPriceChange(string priceChangeId, string priceChangeType)
        {
            // 實作作廢邏輯
        }
    }
}
```

#### 3.2.2 Service: `PriceChangeService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPriceChangeService
    {
        Task<PagedResult<PriceChangeDto>> GetPriceChangesAsync(PriceChangeQueryDto query);
        Task<PriceChangeDetailDto> GetPriceChangeByIdAsync(string priceChangeId, string priceChangeType);
        Task<string> CreatePriceChangeAsync(CreatePriceChangeDto dto);
        Task UpdatePriceChangeAsync(string priceChangeId, string priceChangeType, UpdatePriceChangeDto dto);
        Task DeletePriceChangeAsync(string priceChangeId, string priceChangeType);
        Task ApprovePriceChangeAsync(string priceChangeId, string priceChangeType, ApprovePriceChangeDto dto);
        Task ConfirmPriceChangeAsync(string priceChangeId, string priceChangeType, ConfirmPriceChangeDto dto);
        Task CancelPriceChangeAsync(string priceChangeId, string priceChangeType);
    }
}
```

#### 3.2.3 Repository: `PriceChangeRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IPriceChangeRepository
    {
        Task<PriceChangeMaster> GetByIdAsync(string priceChangeId, string priceChangeType);
        Task<PagedResult<PriceChangeMaster>> GetPagedAsync(PriceChangeQuery query);
        Task<PriceChangeMaster> CreateAsync(PriceChangeMaster priceChange);
        Task<PriceChangeMaster> UpdateAsync(PriceChangeMaster priceChange);
        Task DeleteAsync(string priceChangeId, string priceChangeType);
        Task<bool> ExistsAsync(string priceChangeId, string priceChangeType);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 變價單列表頁面 (`PriceChangeList.vue`)
- **路徑**: `/inventory/price-changes`
- **功能**: 顯示變價單列表，支援查詢、新增、修改、刪除、審核、確認、作廢
- **主要元件**:
  - 查詢表單 (PriceChangeSearchForm)
  - 資料表格 (PriceChangeDataTable)
  - 新增/修改對話框 (PriceChangeDialog)
  - 審核/確認/作廢對話框

#### 4.1.2 變價單詳細頁面 (`PriceChangeDetail.vue`)
- **路徑**: `/inventory/price-changes/:priceChangeId/:priceChangeType`
- **功能**: 顯示變價單詳細資料（含明細），支援修改、審核、確認、作廢

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`PriceChangeSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="變價單號">
      <el-input v-model="searchForm.priceChangeId" placeholder="請輸入變價單號" />
    </el-form-item>
    <el-form-item label="變價類型">
      <el-select v-model="searchForm.priceChangeType" placeholder="請選擇變價類型">
        <el-option label="全部" value="" />
        <el-option label="進價" value="1" />
        <el-option label="售價" value="2" />
      </el-select>
    </el-form-item>
    <el-form-item label="廠商">
      <el-select v-model="searchForm.supplierId" placeholder="請選擇廠商" filterable>
        <el-option v-for="supplier in supplierList" :key="supplier.supplierId" 
                   :label="supplier.supplierName" :value="supplier.supplierId" />
      </el-select>
    </el-form-item>
    <el-form-item label="品牌">
      <el-select v-model="searchForm.logoId" placeholder="請選擇品牌" filterable>
        <el-option v-for="logo in logoList" :key="logo.logoId" 
                   :label="logo.logoName" :value="logo.logoId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="已申請" value="1" />
        <el-option label="已審核" value="2" />
        <el-option label="已確認" value="10" />
        <el-option label="已作廢" value="9" />
      </el-select>
    </el-form-item>
    <el-form-item label="申請日期">
      <el-date-picker v-model="searchForm.applyDateRange" type="daterange" 
                      range-separator="至" start-placeholder="開始日期" end-placeholder="結束日期" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`PriceChangeDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="priceChangeList" v-loading="loading">
      <el-table-column prop="priceChangeId" label="變價單號" width="150" />
      <el-table-column prop="priceChangeTypeName" label="變價類型" width="100" />
      <el-table-column prop="supplierName" label="廠商" width="150" />
      <el-table-column prop="logoName" label="品牌" width="150" />
      <el-table-column prop="applyEmpName" label="申請人" width="100" />
      <el-table-column prop="applyDate" label="申請日期" width="120" />
      <el-table-column prop="startDate" label="啟用日期" width="120" />
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
      <el-table-column label="操作" width="300" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
          <el-button v-if="row.status === '1'" type="warning" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button v-if="row.status === '1'" type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button v-if="row.status === '1'" type="success" size="small" @click="handleApprove(row)">審核</el-button>
          <el-button v-if="row.status === '2'" type="success" size="small" @click="handleConfirm(row)">確認</el-button>
          <el-button v-if="row.status !== '9'" type="info" size="small" @click="handleCancel(row)">作廢</el-button>
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

#### 4.2.3 新增/修改對話框 (`PriceChangeDialog.vue`)
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
        <el-form-item label="變價單號" prop="priceChangeId">
          <el-input v-model="form.priceChangeId" :disabled="true" placeholder="自動編號" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="變價類型" prop="priceChangeType">
          <el-select v-model="form.priceChangeType" :disabled="isEdit" placeholder="請選擇變價類型">
            <el-option label="進價" value="1" />
            <el-option label="售價" value="2" />
          </el-select>
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="品牌編號" prop="logoId">
          <el-select v-model="form.logoId" placeholder="請選擇品牌" filterable>
            <el-option v-for="logo in logoList" :key="logo.logoId" 
                       :label="`${logo.logoId} - ${logo.logoName}`" :value="logo.logoId" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="廠商編號" prop="supplierId">
          <el-select v-model="form.supplierId" placeholder="請選擇廠商" filterable>
            <el-option v-for="supplier in supplierList" :key="supplier.supplierId" 
                       :label="`${supplier.supplierId} - ${supplier.supplierName}`" :value="supplier.supplierId" />
          </el-select>
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="申請日期" prop="applyDate">
          <el-date-picker v-model="form.applyDate" type="date" placeholder="請選擇日期" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="啟用日期" prop="startDate">
          <el-date-picker v-model="form.startDate" type="date" placeholder="請選擇日期" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-form-item label="備註" prop="notes">
      <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
    </el-form-item>
    
    <!-- 明細表格 -->
    <el-divider>變價明細</el-divider>
    <el-table :data="form.details" border>
      <el-table-column type="selection" width="55" />
      <el-table-column prop="lineNum" label="序號" width="80" />
      <el-table-column prop="goodsId" label="商品編號" width="150">
        <template #default="{ row, $index }">
          <el-select v-model="row.goodsId" placeholder="請選擇商品" filterable @change="handleGoodsChange($index, row.goodsId)">
            <el-option v-for="goods in goodsList" :key="goods.goodsId" 
                       :label="`${goods.goodsId} - ${goods.goodsName}`" :value="goods.goodsId" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="goodsName" label="商品名稱" width="200" />
      <el-table-column prop="beforePrice" label="調整前單價" width="120" align="right">
        <template #default="{ row }">
          <el-input-number v-model="row.beforePrice" :precision="2" :min="0" :disabled="true" />
        </template>
      </el-table-column>
      <el-table-column prop="afterPrice" label="調整後單價" width="120" align="right">
        <template #default="{ row }">
          <el-input-number v-model="row.afterPrice" :precision="2" :min="0" />
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="備註" width="200">
        <template #default="{ row }">
          <el-input v-model="row.notes" placeholder="請輸入備註" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ $index }">
          <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-button type="primary" @click="handleAddDetail" style="margin-top: 10px;">新增明細</el-button>
  </el-form>
  <template #footer>
    <el-button @click="handleClose">取消</el-button>
    <el-button type="primary" @click="handleSubmit">確定</el-button>
  </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`priceChange.api.ts`)
```typescript
import request from '@/utils/request';

export interface PriceChangeDto {
  priceChangeId: string;
  priceChangeType: string;
  priceChangeTypeName: string;
  supplierId?: string;
  supplierName?: string;
  logoId?: string;
  logoName?: string;
  applyEmpId?: string;
  applyEmpName?: string;
  applyOrgId?: string;
  applyDate?: string;
  startDate?: string;
  approveEmpId?: string;
  approveDate?: string;
  confirmEmpId?: string;
  confirmDate?: string;
  status: string;
  statusName: string;
  totalAmount?: number;
  notes?: string;
}

export interface PriceChangeDetailDto extends PriceChangeDto {
  details: PriceChangeDetailItemDto[];
}

export interface PriceChangeDetailItemDto {
  lineNum: number;
  goodsId: string;
  goodsName?: string;
  beforePrice?: number;
  afterPrice: number;
  changeQty?: number;
  notes?: string;
}

export interface PriceChangeQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    priceChangeId?: string;
    priceChangeType?: string;
    supplierId?: string;
    logoId?: string;
    status?: string;
    applyDateFrom?: string;
    applyDateTo?: string;
    startDateFrom?: string;
    startDateTo?: string;
  };
}

export interface CreatePriceChangeDto {
  priceChangeType: string;
  supplierId?: string;
  logoId?: string;
  applyEmpId?: string;
  applyOrgId?: string;
  applyDate?: string;
  startDate?: string;
  notes?: string;
  details: CreatePriceChangeDetailItemDto[];
}

export interface CreatePriceChangeDetailItemDto {
  lineNum: number;
  goodsId: string;
  beforePrice?: number;
  afterPrice: number;
  changeQty?: number;
  notes?: string;
}

export interface UpdatePriceChangeDto extends Omit<CreatePriceChangeDto, 'priceChangeType'> {}

// API 函數
export const getPriceChangeList = (query: PriceChangeQueryDto) => {
  return request.get<ApiResponse<PagedResult<PriceChangeDto>>>('/api/v1/price-changes', { params: query });
};

export const getPriceChangeById = (priceChangeId: string, priceChangeType: string) => {
  return request.get<ApiResponse<PriceChangeDetailDto>>(`/api/v1/price-changes/${priceChangeId}/${priceChangeType}`);
};

export const createPriceChange = (data: CreatePriceChangeDto) => {
  return request.post<ApiResponse<string>>('/api/v1/price-changes', data);
};

export const updatePriceChange = (priceChangeId: string, priceChangeType: string, data: UpdatePriceChangeDto) => {
  return request.put<ApiResponse>(`/api/v1/price-changes/${priceChangeId}/${priceChangeType}`, data);
};

export const deletePriceChange = (priceChangeId: string, priceChangeType: string) => {
  return request.delete<ApiResponse>(`/api/v1/price-changes/${priceChangeId}/${priceChangeType}`);
};

export const approvePriceChange = (priceChangeId: string, priceChangeType: string, approveDate: string) => {
  return request.put<ApiResponse>(`/api/v1/price-changes/${priceChangeId}/${priceChangeType}/approve`, { approveDate });
};

export const confirmPriceChange = (priceChangeId: string, priceChangeType: string, confirmDate: string) => {
  return request.put<ApiResponse>(`/api/v1/price-changes/${priceChangeId}/${priceChangeType}/confirm`, { confirmDate });
};

export const cancelPriceChange = (priceChangeId: string, priceChangeType: string) => {
  return request.put<ApiResponse>(`/api/v1/price-changes/${priceChangeId}/${priceChangeType}/cancel`);
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
- [ ] Service 實作（含變價邏輯）
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
- 必須實作權限檢查（申請、審核、確認、作廢權限）
- 敏感資料必須加密傳輸 (HTTPS)
- 必須記錄操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 變價確認時必須使用交易處理

### 6.3 資料驗證
- 變價單號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- 調整後單價必須大於0

### 6.4 業務邏輯
- 只有狀態為「已申請」的變價單可以修改或刪除
- 只有狀態為「已申請」的變價單可以審核
- 只有狀態為「已審核」的變價單可以確認
- 確認變價單時必須更新商品主檔的價格
- 確認變價單時必須記錄變價歷史
- 作廢變價單時必須檢查是否有相關業務

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增變價單成功
- [ ] 新增變價單失敗 (重複單號)
- [ ] 修改變價單成功
- [ ] 修改變價單失敗 (狀態不允許)
- [ ] 刪除變價單成功
- [ ] 刪除變價單失敗 (狀態不允許)
- [ ] 審核變價單成功
- [ ] 確認變價單成功（含價格更新）
- [ ] 作廢變價單成功
- [ ] 查詢變價單列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 變價流程測試（申請→審核→確認）
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 變價確認效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW550_FD.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/SYSW000/SYSW550.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


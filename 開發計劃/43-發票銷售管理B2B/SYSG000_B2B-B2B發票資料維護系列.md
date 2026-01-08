# SYSG000_B2B - B2B發票資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG000_B2B 系列
- **功能名稱**: B2B發票資料維護系列
- **功能描述**: 提供B2B發票基本資料的新增、修改、刪除、查詢功能，包含B2B發票編號、發票日期、發票類型、稅籍資料、發票格式、B2B發票基本資料等資訊管理。此模組為SYSG000的B2B版本，功能類似但針對B2B場景優化
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP` (BREG發票功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP` (CTSG發票功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG12E_*.ASP` (CTSG發票功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG210_*.ASP` (CTSG發票功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG250_*.ASP` (CTSG發票功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP` (EINB發票功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP` (EING發票功能)

### 1.2 業務需求
- 管理B2B發票基本資料資訊
- 支援B2B發票的新增、修改、刪除、查詢
- 記錄B2B發票的建立與變更資訊
- 支援B2B發票類型設定（統一發票、電子發票等）
- 支援B2B稅籍資料維護
- 支援B2B發票格式代號維護
- 支援B2B發票年月管理
- 支援B2B發票號碼區間管理
- 支援B2B發票狀態管理（啟用/停用）
- 支援B2B發票傳輸功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `B2BInvoices` (B2B發票主檔)

```sql
CREATE TABLE [dbo].[B2BInvoices] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InvoiceId] NVARCHAR(50) NOT NULL, -- 發票編號
    [InvoiceType] NVARCHAR(20) NOT NULL, -- 發票類型 (INV_TYPE, 01:統一發票, 02:電子發票, 03:收據)
    [InvoiceYear] INT NOT NULL, -- 發票年份 (INV_YEAR)
    [InvoiceMonth] INT NOT NULL, -- 發票月份 (INV_MONTH)
    [InvoiceYm] NVARCHAR(6) NOT NULL, -- 發票年月 (INV_YM, YYYYMM格式)
    [Track] NVARCHAR(2) NULL, -- 字軌 (TRACK)
    [InvoiceNoB] NVARCHAR(10) NULL, -- 發票號碼起 (INV_NO_B)
    [InvoiceNoE] NVARCHAR(10) NULL, -- 發票號碼迄 (INV_NO_E)
    [InvoiceFormat] NVARCHAR(50) NULL, -- 發票格式代號 (INV_FORMAT)
    [TaxId] NVARCHAR(20) NULL, -- 統一編號 (TAX_ID)
    [CompanyName] NVARCHAR(200) NULL, -- 公司名稱 (COMPANY_NAME)
    [CompanyNameEn] NVARCHAR(200) NULL, -- 公司英文名稱
    [Address] NVARCHAR(500) NULL, -- 地址 (ADDRESS)
    [City] NVARCHAR(50) NULL, -- 城市 (CITY)
    [Zone] NVARCHAR(50) NULL, -- 區域 (ZONE)
    [PostalCode] NVARCHAR(20) NULL, -- 郵遞區號 (POSTAL_CODE)
    [Phone] NVARCHAR(50) NULL, -- 電話 (PHONE)
    [Fax] NVARCHAR(50) NULL, -- 傳真 (FAX)
    [Email] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [SubCopy] NVARCHAR(10) NULL, -- 副聯 (SUB_COPY)
    [SubCopyValue] NVARCHAR(100) NULL, -- 副聯值 (SUB_COPY_VALUE)
    [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y', -- B2B標記 (B2B_FLAG, Y:是, N:否)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [Notes] NVARCHAR(1000) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_B2BInvoices] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_B2BInvoices_InvoiceId] UNIQUE ([InvoiceId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_B2BInvoices_InvoiceId] ON [dbo].[B2BInvoices] ([InvoiceId]);
CREATE NONCLUSTERED INDEX [IX_B2BInvoices_InvoiceType] ON [dbo].[B2BInvoices] ([InvoiceType]);
CREATE NONCLUSTERED INDEX [IX_B2BInvoices_InvoiceYm] ON [dbo].[B2BInvoices] ([InvoiceYm]);
CREATE NONCLUSTERED INDEX [IX_B2BInvoices_TaxId] ON [dbo].[B2BInvoices] ([TaxId]);
CREATE NONCLUSTERED INDEX [IX_B2BInvoices_SiteId] ON [dbo].[B2BInvoices] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_B2BInvoices_Status] ON [dbo].[B2BInvoices] ([Status]);
CREATE NONCLUSTERED INDEX [IX_B2BInvoices_B2BFlag] ON [dbo].[B2BInvoices] ([B2BFlag]);
```

### 2.2 相關資料表

#### 2.2.1 `B2BInvoiceFormats` - B2B發票格式代號
```sql
CREATE TABLE [dbo].[B2BInvoiceFormats] (
    [FormatId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [FormatName] NVARCHAR(100) NOT NULL,
    [FormatNameEn] NVARCHAR(100) NULL,
    [Description] NVARCHAR(500) NULL,
    [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y',
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

#### 2.2.2 `B2BInvoiceTransfers` - B2B發票傳輸記錄
```sql
CREATE TABLE [dbo].[B2BInvoiceTransfers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InvoiceId] NVARCHAR(50) NOT NULL,
    [TransferType] NVARCHAR(20) NOT NULL, -- 傳輸類型 (BREG, CTSG, EINB, EING等)
    [TransferStatus] NVARCHAR(20) NOT NULL, -- 傳輸狀態 (PENDING, SUCCESS, FAILED)
    [TransferDate] DATETIME2 NULL,
    [TransferMessage] NVARCHAR(1000) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_B2BInvoiceTransfers_B2BInvoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[B2BInvoices] ([InvoiceId])
);
```

### 2.3 資料字典

#### 2.3.1 B2BInvoices 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| InvoiceId | NVARCHAR | 50 | NO | - | 發票編號 | 唯一 |
| InvoiceType | NVARCHAR | 20 | NO | - | 發票類型 | 01:統一發票, 02:電子發票, 03:收據 |
| InvoiceYear | INT | - | NO | - | 發票年份 | - |
| InvoiceMonth | INT | - | NO | - | 發票月份 | - |
| InvoiceYm | NVARCHAR | 6 | NO | - | 發票年月 | YYYYMM格式 |
| Track | NVARCHAR | 2 | YES | - | 字軌 | - |
| InvoiceNoB | NVARCHAR | 10 | YES | - | 發票號碼起 | - |
| InvoiceNoE | NVARCHAR | 10 | YES | - | 發票號碼迄 | - |
| InvoiceFormat | NVARCHAR | 50 | YES | - | 發票格式代號 | 外鍵至B2BInvoiceFormats |
| TaxId | NVARCHAR | 20 | YES | - | 統一編號 | - |
| CompanyName | NVARCHAR | 200 | YES | - | 公司名稱 | - |
| CompanyNameEn | NVARCHAR | 200 | YES | - | 公司英文名稱 | - |
| Address | NVARCHAR | 500 | YES | - | 地址 | - |
| City | NVARCHAR | 50 | YES | - | 城市 | - |
| Zone | NVARCHAR | 50 | YES | - | 區域 | - |
| PostalCode | NVARCHAR | 20 | YES | - | 郵遞區號 | - |
| Phone | NVARCHAR | 50 | YES | - | 電話 | - |
| Fax | NVARCHAR | 50 | YES | - | 傳真 | - |
| Email | NVARCHAR | 100 | YES | - | 電子郵件 | - |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | - |
| SubCopy | NVARCHAR | 10 | YES | - | 副聯 | - |
| SubCopyValue | NVARCHAR | 100 | YES | - | 副聯值 | - |
| B2BFlag | NVARCHAR | 10 | NO | 'Y' | B2B標記 | Y:是, N:否 |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| Notes | NVARCHAR | 1000 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢B2B發票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-invoices`
- **說明**: 查詢B2B發票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "InvoiceId",
    "sortOrder": "ASC",
    "filters": {
      "invoiceId": "",
      "invoiceType": "",
      "invoiceYm": "",
      "taxId": "",
      "siteId": "",
      "status": "",
      "b2BFlag": "Y"
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
          "invoiceId": "INV001",
          "invoiceType": "02",
          "invoiceYear": 2024,
          "invoiceMonth": 1,
          "invoiceYm": "202401",
          "track": "AA",
          "invoiceNoB": "00000001",
          "invoiceNoE": "00000100",
          "invoiceFormat": "FORMAT01",
          "taxId": "12345678",
          "companyName": "測試公司",
          "status": "A",
          "b2BFlag": "Y"
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

#### 3.1.2 查詢單筆B2B發票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-invoices/{invoiceId}`
- **說明**: 根據發票編號查詢單筆B2B發票資料
- **路徑參數**:
  - `invoiceId`: 發票編號
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 新增B2B發票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-invoices`
- **說明**: 新增B2B發票資料
- **請求格式**:
  ```json
  {
    "invoiceId": "INV001",
    "invoiceType": "02",
    "invoiceYear": 2024,
    "invoiceMonth": 1,
    "invoiceYm": "202401",
    "track": "AA",
    "invoiceNoB": "00000001",
    "invoiceNoE": "00000100",
    "invoiceFormat": "FORMAT01",
    "taxId": "12345678",
    "companyName": "測試公司",
    "companyNameEn": "Test Company",
    "address": "測試地址",
    "city": "台北市",
    "zone": "信義區",
    "postalCode": "110",
    "phone": "02-12345678",
    "fax": "02-87654321",
    "email": "test@example.com",
    "siteId": "SITE001",
    "subCopy": "01",
    "subCopyValue": "副聯值",
    "status": "A",
    "notes": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "invoiceId": "INV001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改B2B發票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/b2b-invoices/{invoiceId}`
- **說明**: 修改B2B發票資料
- **路徑參數**:
  - `invoiceId`: 發票編號
- **請求格式**: 同新增，但 `invoiceId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除B2B發票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/b2b-invoices/{invoiceId}`
- **說明**: 刪除B2B發票資料（軟刪除或硬刪除）
- **路徑參數**:
  - `invoiceId`: 發票編號
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

#### 3.1.6 批次刪除B2B發票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/b2b-invoices/batch`
- **說明**: 批次刪除多筆B2B發票
- **請求格式**:
  ```json
  {
    "invoiceIds": ["INV001", "INV002", "INV003"]
  }
  ```

#### 3.1.7 B2B發票傳輸
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-invoices/{invoiceId}/transfer`
- **說明**: 執行B2B發票傳輸作業
- **路徑參數**:
  - `invoiceId`: 發票編號
- **請求格式**:
  ```json
  {
    "transferType": "BREG" // BREG, CTSG, EINB, EING等
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "傳輸成功",
    "data": {
      "transferId": "TRF001",
      "transferStatus": "SUCCESS"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `B2BInvoicesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/b2b-invoices")]
    [Authorize]
    public class B2BInvoicesController : ControllerBase
    {
        private readonly IB2BInvoiceService _b2bInvoiceService;
        
        public B2BInvoicesController(IB2BInvoiceService b2bInvoiceService)
        {
            _b2bInvoiceService = b2bInvoiceService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<B2BInvoiceDto>>>> GetB2BInvoices([FromQuery] B2BInvoiceQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<ApiResponse<B2BInvoiceDto>>> GetB2BInvoice(string invoiceId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateB2BInvoice([FromBody] CreateB2BInvoiceDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{invoiceId}")]
        public async Task<ActionResult<ApiResponse>> UpdateB2BInvoice(string invoiceId, [FromBody] UpdateB2BInvoiceDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{invoiceId}")]
        public async Task<ActionResult<ApiResponse>> DeleteB2BInvoice(string invoiceId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{invoiceId}/transfer")]
        public async Task<ActionResult<ApiResponse<B2BInvoiceTransferDto>>> TransferB2BInvoice(string invoiceId, [FromBody] B2BInvoiceTransferRequestDto dto)
        {
            // 實作傳輸邏輯
        }
    }
}
```

#### 3.2.2 Service: `B2BInvoiceService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IB2BInvoiceService
    {
        Task<PagedResult<B2BInvoiceDto>> GetB2BInvoicesAsync(B2BInvoiceQueryDto query);
        Task<B2BInvoiceDto> GetB2BInvoiceByIdAsync(string invoiceId);
        Task<string> CreateB2BInvoiceAsync(CreateB2BInvoiceDto dto);
        Task UpdateB2BInvoiceAsync(string invoiceId, UpdateB2BInvoiceDto dto);
        Task DeleteB2BInvoiceAsync(string invoiceId);
        Task<B2BInvoiceTransferDto> TransferB2BInvoiceAsync(string invoiceId, B2BInvoiceTransferRequestDto dto);
    }
}
```

#### 3.2.3 Repository: `B2BInvoiceRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IB2BInvoiceRepository
    {
        Task<B2BInvoice> GetByIdAsync(string invoiceId);
        Task<PagedResult<B2BInvoice>> GetPagedAsync(B2BInvoiceQuery query);
        Task<B2BInvoice> CreateAsync(B2BInvoice invoice);
        Task<B2BInvoice> UpdateAsync(B2BInvoice invoice);
        Task DeleteAsync(string invoiceId);
        Task<bool> ExistsAsync(string invoiceId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 B2B發票列表頁面 (`B2BInvoiceList.vue`)
- **路徑**: `/b2b-invoices`
- **功能**: 顯示B2B發票列表，支援查詢、新增、修改、刪除、傳輸
- **主要元件**:
  - 查詢表單 (B2BInvoiceSearchForm)
  - 資料表格 (B2BInvoiceDataTable)
  - 新增/修改對話框 (B2BInvoiceDialog)
  - 刪除確認對話框
  - 傳輸對話框

#### 4.1.2 B2B發票詳細頁面 (`B2BInvoiceDetail.vue`)
- **路徑**: `/b2b-invoices/:invoiceId`
- **功能**: 顯示B2B發票詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`B2BInvoiceSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="發票編號">
      <el-input v-model="searchForm.invoiceId" placeholder="請輸入發票編號" />
    </el-form-item>
    <el-form-item label="發票類型">
      <el-select v-model="searchForm.invoiceType" placeholder="請選擇發票類型">
        <el-option label="統一發票" value="01" />
        <el-option label="電子發票" value="02" />
        <el-option label="收據" value="03" />
      </el-select>
    </el-form-item>
    <el-form-item label="發票年月">
      <el-date-picker
        v-model="searchForm.invoiceYm"
        type="month"
        placeholder="請選擇年月"
        format="YYYYMM"
        value-format="YYYYMM"
      />
    </el-form-item>
    <el-form-item label="統一編號">
      <el-input v-model="searchForm.taxId" placeholder="請輸入統一編號" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="啟用" value="A" />
        <el-option label="停用" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`B2BInvoiceDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="b2bInvoiceList" v-loading="loading">
      <el-table-column prop="invoiceId" label="發票編號" width="120" />
      <el-table-column prop="invoiceType" label="發票類型" width="100">
        <template #default="{ row }">
          {{ getInvoiceTypeText(row.invoiceType) }}
        </template>
      </el-table-column>
      <el-table-column prop="invoiceYm" label="發票年月" width="100" />
      <el-table-column prop="companyName" label="公司名稱" width="200" />
      <el-table-column prop="taxId" label="統一編號" width="120" />
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
          <el-button type="success" size="small" @click="handleTransfer(row)">傳輸</el-button>
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

#### 4.2.3 新增/修改對話框 (`B2BInvoiceDialog.vue`)
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
          <el-form-item label="發票編號" prop="invoiceId">
            <el-input v-model="form.invoiceId" :disabled="isEdit" placeholder="請輸入發票編號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="發票類型" prop="invoiceType">
            <el-select v-model="form.invoiceType" placeholder="請選擇發票類型">
              <el-option label="統一發票" value="01" />
              <el-option label="電子發票" value="02" />
              <el-option label="收據" value="03" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="發票年月" prop="invoiceYm">
            <el-date-picker
              v-model="form.invoiceYm"
              type="month"
              placeholder="請選擇年月"
              format="YYYYMM"
              value-format="YYYYMM"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="字軌" prop="track">
            <el-input v-model="form.track" placeholder="請輸入字軌" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="發票號碼起" prop="invoiceNoB">
            <el-input v-model="form.invoiceNoB" placeholder="請輸入發票號碼起" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="發票號碼迄" prop="invoiceNoE">
            <el-input v-model="form.invoiceNoE" placeholder="請輸入發票號碼迄" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="統一編號" prop="taxId">
        <el-input v-model="form.taxId" placeholder="請輸入統一編號" />
      </el-form-item>
      <el-form-item label="公司名稱" prop="companyName">
        <el-input v-model="form.companyName" placeholder="請輸入公司名稱" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="A" />
          <el-option label="停用" value="I" />
        </el-select>
      </el-form-item>
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

### 4.3 API 呼叫 (`b2bInvoice.api.ts`)
```typescript
import request from '@/utils/request';

export interface B2BInvoiceDto {
  tKey: number;
  invoiceId: string;
  invoiceType: string;
  invoiceYear: number;
  invoiceMonth: number;
  invoiceYm: string;
  track?: string;
  invoiceNoB?: string;
  invoiceNoE?: string;
  invoiceFormat?: string;
  taxId?: string;
  companyName?: string;
  companyNameEn?: string;
  address?: string;
  city?: string;
  zone?: string;
  postalCode?: string;
  phone?: string;
  fax?: string;
  email?: string;
  siteId?: string;
  subCopy?: string;
  subCopyValue?: string;
  b2BFlag: string;
  status: string;
  notes?: string;
}

export interface B2BInvoiceQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    invoiceId?: string;
    invoiceType?: string;
    invoiceYm?: string;
    taxId?: string;
    siteId?: string;
    status?: string;
    b2BFlag?: string;
  };
}

export interface CreateB2BInvoiceDto {
  invoiceId: string;
  invoiceType: string;
  invoiceYear: number;
  invoiceMonth: number;
  invoiceYm: string;
  track?: string;
  invoiceNoB?: string;
  invoiceNoE?: string;
  invoiceFormat?: string;
  taxId?: string;
  companyName?: string;
  companyNameEn?: string;
  address?: string;
  city?: string;
  zone?: string;
  postalCode?: string;
  phone?: string;
  fax?: string;
  email?: string;
  siteId?: string;
  subCopy?: string;
  subCopyValue?: string;
  status: string;
  notes?: string;
}

export interface UpdateB2BInvoiceDto extends Omit<CreateB2BInvoiceDto, 'invoiceId'> {}

export interface B2BInvoiceTransferRequestDto {
  transferType: string; // BREG, CTSG, EINB, EING等
}

// API 函數
export const getB2BInvoiceList = (query: B2BInvoiceQueryDto) => {
  return request.get<ApiResponse<PagedResult<B2BInvoiceDto>>>('/api/v1/b2b-invoices', { params: query });
};

export const getB2BInvoiceById = (invoiceId: string) => {
  return request.get<ApiResponse<B2BInvoiceDto>>(`/api/v1/b2b-invoices/${invoiceId}`);
};

export const createB2BInvoice = (data: CreateB2BInvoiceDto) => {
  return request.post<ApiResponse<string>>('/api/v1/b2b-invoices', data);
};

export const updateB2BInvoice = (invoiceId: string, data: UpdateB2BInvoiceDto) => {
  return request.put<ApiResponse>(`/api/v1/b2b-invoices/${invoiceId}`, data);
};

export const deleteB2BInvoice = (invoiceId: string) => {
  return request.delete<ApiResponse>(`/api/v1/b2b-invoices/${invoiceId}`);
};

export const transferB2BInvoice = (invoiceId: string, data: B2BInvoiceTransferRequestDto) => {
  return request.post<ApiResponse>(`/api/v1/b2b-invoices/${invoiceId}/transfer`, data);
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
- [ ] 傳輸功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 傳輸功能開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試
- [ ] 傳輸功能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- B2B發票資料必須與一般發票資料隔離

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制
- 傳輸功能必須支援非同步處理

### 6.3 資料驗證
- 發票編號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- B2B標記必須為'Y'

### 6.4 業務邏輯
- 刪除B2B發票前必須檢查是否有相關資料
- 停用B2B發票時必須檢查是否有進行中的業務
- 傳輸功能必須記錄傳輸狀態
- B2B發票格式必須符合B2B規範

### 6.5 B2B特定需求
- B2B發票必須標記B2BFlag為'Y'
- B2B發票傳輸必須支援多種傳輸類型（BREG, CTSG, EINB, EING等）
- B2B發票格式必須符合B2B業務規範

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增B2B發票成功
- [ ] 新增B2B發票失敗 (重複編號)
- [ ] 修改B2B發票成功
- [ ] 修改B2B發票失敗 (不存在)
- [ ] 刪除B2B發票成功
- [ ] 查詢B2B發票列表成功
- [ ] 查詢單筆B2B發票成功
- [ ] B2B發票傳輸成功
- [ ] B2B發票傳輸失敗

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 傳輸功能測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 傳輸功能效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG12E_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG210_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG250_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP`

### 8.2 相關功能
- SYSG110-SYSG190-發票資料維護系列（一般發票功能）
- SYSG210-SYSG2B0-電子發票列印系列（電子發票列印功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


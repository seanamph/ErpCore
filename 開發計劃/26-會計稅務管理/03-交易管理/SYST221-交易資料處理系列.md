# SYST221 - 交易資料處理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST221 系列
- **功能名稱**: 交易資料處理系列
- **功能描述**: 提供傳票維護作業的多筆新增功能，包含傳票主檔及明細的新增、修改、刪除、查詢功能，支援套用常用傳票、關係人功能、自訂欄位等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST221_FI.ASP` (新增 - 傳票維護作業多筆新增畫面)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FI.ASP` (新增 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FU.ASP` (修改 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FD.ASP` (刪除 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FQ.ASP` (查詢 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FB.ASP` (瀏覽 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_PR.ASP` (報表 - 傳票維護作業)

### 1.2 業務需求
- 管理傳票主檔資料（傳票日期、傳票型態、分店代號、傳票摘要等）
- 管理傳票明細資料（序號、借貸方向、會計科目、金額、組織代號、摘要等）
- 支援套用常用傳票功能
- 支援關係人功能（依據參數設定）
- 支援廠商名稱欄位（依據參數設定）
- 支援自訂欄位（依據參數設定）
- 支援多筆新增功能（預設30筆，可調整）
- 支援傳票借貸平衡檢查
- 支援會計科目類別檢查（同一張傳票需為相同科目類別）
- 支援IFRS會計科目檢查（最後一碼為A）
- 支援組織代號權限控制
- 支援傳票審批流程
- 支援傳票列印功能
- 支援傳票匯出功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Vouchers` (對應舊系統 `VOUCHER_M`)

```sql
CREATE TABLE [dbo].[Vouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherNo] NVARCHAR(50) NOT NULL, -- 傳票號碼 (VOUCHER_NO)
    [VoucherDate] DATETIME2 NOT NULL, -- 傳票日期 (VOUCHER_DATE)
    [VoucherType] NVARCHAR(50) NULL, -- 傳票型態 (VOUCHER_TYPE, 外鍵至VoucherTypes)
    [SiteId] NVARCHAR(50) NULL, -- 分店代號 (SITE_ID)
    [VoucherNotes] NVARCHAR(500) NULL, -- 傳票摘要 (VOUCHER_NOTES)
    [VendorId] NVARCHAR(50) NULL, -- 廠客代號 (VENDOR_ID)
    [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME, 依據參數顯示)
    [Status] NVARCHAR(1) NULL DEFAULT '1', -- 狀態 (STATUS, 1:正常, 0:作廢)
    [ApprovalStatus] NVARCHAR(1) NULL DEFAULT '0', -- 審批狀態 (APPROVAL_STATUS, 0:未審批, 1:已審批)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Vouchers_VoucherNo] UNIQUE ([VoucherNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherNo] ON [dbo].[Vouchers] ([VoucherNo]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherDate] ON [dbo].[Vouchers] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherType] ON [dbo].[Vouchers] ([VoucherType]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_SiteId] ON [dbo].[Vouchers] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_Status] ON [dbo].[Vouchers] ([Status]);
```

### 2.2 主要資料表: `VoucherDetails` (對應舊系統 `VOUCHER_D`)

```sql
CREATE TABLE [dbo].[VoucherDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔TKey (外鍵至Vouchers)
    [Sn] INT NOT NULL, -- 序號 (SN)
    [Dc] NVARCHAR(1) NOT NULL, -- 借貸方向 (DC, D:借方, C:貸方)
    [StypeId] NVARCHAR(50) NOT NULL, -- 會計科目 (STYPE_ID, 外鍵至AccountSubjects)
    [DAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 借方金額 (D_AMT)
    [CAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 貸方金額 (C_AMT)
    [OrgId] NVARCHAR(50) NULL, -- 組織代號 (ORG_ID)
    [Notes] NVARCHAR(500) NULL, -- 摘要 (NOTES)
    [VendorId] NVARCHAR(50) NULL, -- 廠客代號 (VENDOR_ID)
    [AcctKey] NVARCHAR(50) NULL, -- 對象別KEY值 (ACCT_KEY)
    [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
    [CustomField1] NVARCHAR(200) NULL, -- 自訂欄位1 (CUSTOM_FIELD1, 依據參數顯示)
    [CustomField2] NVARCHAR(200) NULL, -- 自訂欄位2 (CUSTOM_FIELD2, 依據參數顯示)
    [CustomField3] NVARCHAR(200) NULL, -- 自訂欄位3 (CUSTOM_FIELD3, 依據參數顯示)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_VoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_VoucherDetails_Vouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[Vouchers] ([TKey]) ON DELETE CASCADE,
    CONSTRAINT [FK_VoucherDetails_AccountSubjects] FOREIGN KEY ([StypeId]) REFERENCES [dbo].[AccountSubjects] ([StypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherTKey] ON [dbo].[VoucherDetails] ([VoucherTKey]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_StypeId] ON [dbo].[VoucherDetails] ([StypeId]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_OrgId] ON [dbo].[VoucherDetails] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_Sn] ON [dbo].[VoucherDetails] ([VoucherTKey], [Sn]);
```

### 2.3 資料字典

#### Vouchers 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherNo | NVARCHAR | 50 | NO | - | 傳票號碼 | 唯一，主鍵候選 |
| VoucherDate | DATETIME2 | - | NO | - | 傳票日期 | - |
| VoucherType | NVARCHAR | 50 | YES | - | 傳票型態 | 外鍵至VoucherTypes |
| SiteId | NVARCHAR | 50 | YES | - | 分店代號 | - |
| VoucherNotes | NVARCHAR | 500 | YES | - | 傳票摘要 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠客代號 | - |
| VendorName | NVARCHAR | 200 | YES | - | 廠商名稱 | 依據參數顯示 |
| Status | NVARCHAR | 1 | YES | '1' | 狀態 | 1:正常, 0:作廢 |
| ApprovalStatus | NVARCHAR | 1 | YES | '0' | 審批狀態 | 0:未審批, 1:已審批 |

#### VoucherDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherTKey | BIGINT | - | NO | - | 傳票主檔TKey | 外鍵至Vouchers |
| Sn | INT | - | NO | - | 序號 | - |
| Dc | NVARCHAR | 1 | NO | - | 借貸方向 | D:借方, C:貸方 |
| StypeId | NVARCHAR | 50 | NO | - | 會計科目 | 外鍵至AccountSubjects |
| DAmt | DECIMAL | 18,2 | YES | 0 | 借方金額 | - |
| CAmt | DECIMAL | 18,2 | YES | 0 | 貸方金額 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代號 | - |
| Notes | NVARCHAR | 500 | YES | - | 摘要 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠客代號 | - |
| AcctKey | NVARCHAR | 50 | YES | - | 對象別KEY值 | - |
| ObjectId | NVARCHAR | 50 | YES | - | 對象別編號 | - |
| CustomField1 | NVARCHAR | 200 | YES | - | 自訂欄位1 | 依據參數顯示 |
| CustomField2 | NVARCHAR | 200 | YES | - | 自訂欄位2 | 依據參數顯示 |
| CustomField3 | NVARCHAR | 200 | YES | - | 自訂欄位3 | 依據參數顯示 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢傳票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers`
- **說明**: 查詢傳票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherDate",
    "sortOrder": "DESC",
    "filters": {
      "voucherNo": "",
      "voucherDateFrom": "",
      "voucherDateTo": "",
      "voucherType": "",
      "siteId": "",
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
          "tKey": 1,
          "voucherNo": "V20240101001",
          "voucherDate": "2024-01-01",
          "voucherType": "1",
          "siteId": "001",
          "voucherNotes": "傳票摘要",
          "status": "1"
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

#### 3.1.2 查詢單筆傳票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/{voucherNo}`
- **說明**: 根據傳票號碼查詢單筆傳票資料（含明細）
- **路徑參數**:
  - `voucherNo`: 傳票號碼
- **回應格式**: 包含傳票主檔及明細資料

#### 3.1.3 新增傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers`
- **說明**: 新增傳票資料（含主檔及明細）
- **請求格式**:
  ```json
  {
    "voucherDate": "2024-01-01",
    "voucherType": "1",
    "siteId": "001",
    "voucherNotes": "傳票摘要",
    "vendorId": null,
    "vendorName": null,
    "details": [
      {
        "sn": 1,
        "dc": "D",
        "stypeId": "1000",
        "dAmt": 1000,
        "cAmt": 0,
        "orgId": "ORG001",
        "notes": "摘要1",
        "vendorId": null,
        "acctKey": null,
        "objectId": null,
        "customField1": null
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
      "voucherNo": "V20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改傳票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vouchers/{voucherNo}`
- **說明**: 修改傳票資料（含主檔及明細）
- **路徑參數**:
  - `voucherNo`: 傳票號碼
- **請求格式**: 同新增，但 `voucherNo` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除傳票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/vouchers/{voucherNo}`
- **說明**: 刪除傳票資料（需檢查是否已審批）
- **路徑參數**:
  - `voucherNo`: 傳票號碼
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

#### 3.1.6 檢查傳票借貸平衡
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/check-balance`
- **說明**: 檢查傳票明細的借貸是否平衡
- **請求格式**:
  ```json
  {
    "details": [
      {
        "dAmt": 1000,
        "cAmt": 0
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "檢查成功",
    "data": {
      "isBalanced": true,
      "debitTotal": 1000,
      "creditTotal": 1000,
      "difference": 0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 套用常用傳票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/common-voucher/{tKey}`
- **說明**: 根據常用傳票TKey取得傳票資料
- **路徑參數**:
  - `tKey`: 常用傳票TKey
- **回應格式**: 包含傳票主檔及明細資料

### 3.2 後端實作類別

#### 3.2.1 Controller: `VouchersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/vouchers")]
    [Authorize]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        
        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVouchers([FromQuery] VoucherQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{voucherNo}")]
        public async Task<ActionResult<ApiResponse<VoucherDetailDto>>> GetVoucher(string voucherNo)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            // 實作新增邏輯（需檢查借貸平衡、會計科目類別等）
        }
        
        [HttpPut("{voucherNo}")]
        public async Task<ActionResult<ApiResponse>> UpdateVoucher(string voucherNo, [FromBody] UpdateVoucherDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{voucherNo}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoucher(string voucherNo)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("check-balance")]
        public async Task<ActionResult<ApiResponse<BalanceCheckDto>>> CheckBalance([FromBody] BalanceCheckRequestDto dto)
        {
            // 實作借貸平衡檢查邏輯
        }
        
        [HttpGet("common-voucher/{tKey}")]
        public async Task<ActionResult<ApiResponse<VoucherDetailDto>>> GetCommonVoucher(long tKey)
        {
            // 實作套用常用傳票邏輯
        }
    }
}
```

#### 3.2.2 Service: `VoucherService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IVoucherService
    {
        Task<PagedResult<VoucherDto>> GetVouchersAsync(VoucherQueryDto query);
        Task<VoucherDetailDto> GetVoucherByIdAsync(string voucherNo);
        Task<string> CreateVoucherAsync(CreateVoucherDto dto);
        Task UpdateVoucherAsync(string voucherNo, UpdateVoucherDto dto);
        Task DeleteVoucherAsync(string voucherNo);
        Task<BalanceCheckDto> CheckBalanceAsync(List<VoucherDetailItemDto> details);
        Task<VoucherDetailDto> GetCommonVoucherAsync(long tKey);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 傳票列表頁面 (`VoucherList.vue`)
- **路徑**: `/accounting/vouchers`
- **功能**: 顯示傳票列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (VoucherSearchForm)
  - 資料表格 (VoucherDataTable)
  - 新增/修改對話框 (VoucherDialog)
  - 刪除確認對話框

#### 4.1.2 傳票新增頁面 (`VoucherAdd.vue`)
- **路徑**: `/accounting/vouchers/add`
- **功能**: 傳票多筆新增畫面
- **主要元件**:
  - 傳票主檔表單 (VoucherHeaderForm)
  - 傳票明細表格 (VoucherDetailTable)
  - 套用常用傳票按鈕
  - 新增項目按鈕
  - 借貸平衡檢查顯示

### 4.2 UI 元件設計

#### 4.2.1 傳票主檔表單元件 (`VoucherHeaderForm.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="傳票日期" prop="voucherDate">
          <el-date-picker v-model="form.voucherDate" type="date" placeholder="請選擇傳票日期" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="傳票型態" prop="voucherType">
          <el-select v-model="form.voucherType" placeholder="請選擇傳票型態" filterable>
            <el-option v-for="item in voucherTypeList" :key="item.voucherId" :label="item.voucherName" :value="item.voucherId" />
          </el-select>
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="分店代號" prop="siteId">
          <el-select v-model="form.siteId" placeholder="請選擇分店" filterable>
            <el-option v-for="item in siteList" :key="item.siteId" :label="item.siteName" :value="item.siteId" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="傳票摘要" prop="voucherNotes">
          <el-input v-model="form.voucherNotes" placeholder="請輸入傳票摘要" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row v-if="showVendorName" :gutter="20">
      <el-col :span="12">
        <el-form-item label="廠客代號" prop="vendorId">
          <el-select v-model="form.vendorId" placeholder="請選擇廠客" filterable clearable>
            <el-option v-for="item in vendorList" :key="item.vendorId" :label="item.vendorName" :value="item.vendorId" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="廠商名稱" prop="vendorName">
          <el-input v-model="form.vendorName" placeholder="請輸入廠商名稱" />
        </el-form-item>
      </el-col>
    </el-row>
  </el-form>
</template>
```

#### 4.2.2 傳票明細表格元件 (`VoucherDetailTable.vue`)
```vue
<template>
  <div>
    <div class="toolbar">
      <el-button type="primary" @click="handleAddRow">新增項目</el-button>
      <el-button @click="handleApplyCommonVoucher">套用常用傳票</el-button>
      <el-button @click="handleCheckBalance">檢查借貸平衡</el-button>
    </div>
    <el-table :data="detailList" border>
      <el-table-column type="selection" width="55" />
      <el-table-column prop="sn" label="序號" width="80">
        <template #default="{ row, $index }">
          <el-input-number v-model="row.sn" :min="1" :max="999" />
        </template>
      </el-table-column>
      <el-table-column prop="dc" label="借貸" width="100">
        <template #default="{ row }">
          <el-select v-model="row.dc" placeholder="請選擇">
            <el-option label="借方" value="D" />
            <el-option label="貸方" value="C" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="stypeId" label="會計科目" width="150">
        <template #default="{ row }">
          <el-select v-model="row.stypeId" placeholder="請選擇會計科目" filterable>
            <el-option v-for="item in accountSubjectList" :key="item.stypeId" :label="item.stypeName" :value="item.stypeId" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="dAmt" label="借方金額" width="150">
        <template #default="{ row }">
          <el-input-number v-model="row.dAmt" :min="0" :precision="2" />
        </template>
      </el-table-column>
      <el-table-column prop="cAmt" label="貸方金額" width="150">
        <template #default="{ row }">
          <el-input-number v-model="row.cAmt" :min="0" :precision="2" />
        </template>
      </el-table-column>
      <el-table-column prop="orgId" label="組織代號" width="150">
        <template #default="{ row }">
          <el-select v-model="row.orgId" placeholder="請選擇組織" filterable clearable>
            <el-option v-for="item in orgList" :key="item.orgId" :label="item.orgName" :value="item.orgId" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="摘要" width="200">
        <template #default="{ row }">
          <el-input v-model="row.notes" placeholder="請輸入摘要" />
        </template>
      </el-table-column>
      <el-table-column v-if="showObjectId" prop="objectId" label="對象別編號" width="150">
        <template #default="{ row }">
          <el-input v-model="row.objectId" placeholder="請輸入對象別編號" />
        </template>
      </el-table-column>
      <el-table-column v-for="(field, index) in customFields" :key="index" :prop="`customField${index + 1}`" :label="field.name" width="150">
        <template #default="{ row }">
          <el-input v-model="row[`customField${index + 1}`]" :maxlength="field.length" :placeholder="`請輸入${field.name}`" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ $index }">
          <el-button type="danger" size="small" @click="handleDeleteRow($index)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div class="balance-info">
      <span>借方總額: {{ debitTotal }}</span>
      <span>貸方總額: {{ creditTotal }}</span>
      <span :class="{ 'text-danger': !isBalanced }">差額: {{ difference }}</span>
    </div>
  </div>
</template>
```

### 4.3 API 呼叫 (`voucher.api.ts`)
```typescript
import request from '@/utils/request';

export interface VoucherDto {
  tKey: number;
  voucherNo: string;
  voucherDate: string;
  voucherType?: string;
  siteId?: string;
  voucherNotes?: string;
  vendorId?: string;
  vendorName?: string;
  status?: string;
  approvalStatus?: string;
}

export interface VoucherDetailDto extends VoucherDto {
  details: VoucherDetailItemDto[];
}

export interface VoucherDetailItemDto {
  tKey?: number;
  sn: number;
  dc: string;
  stypeId: string;
  dAmt: number;
  cAmt: number;
  orgId?: string;
  notes?: string;
  vendorId?: string;
  acctKey?: string;
  objectId?: string;
  customField1?: string;
  customField2?: string;
  customField3?: string;
}

export interface CreateVoucherDto {
  voucherDate: string;
  voucherType?: string;
  siteId?: string;
  voucherNotes?: string;
  vendorId?: string;
  vendorName?: string;
  details: VoucherDetailItemDto[];
}

export interface UpdateVoucherDto extends Omit<CreateVoucherDto, 'voucherDate'> {}

export interface BalanceCheckDto {
  isBalanced: boolean;
  debitTotal: number;
  creditTotal: number;
  difference: number;
}

// API 函數
export const getVoucherList = (query: VoucherQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/vouchers', { params: query });
};

export const getVoucherById = (voucherNo: string) => {
  return request.get<ApiResponse<VoucherDetailDto>>(`/api/v1/vouchers/${voucherNo}`);
};

export const createVoucher = (data: CreateVoucherDto) => {
  return request.post<ApiResponse<string>>('/api/v1/vouchers', data);
};

export const updateVoucher = (voucherNo: string, data: UpdateVoucherDto) => {
  return request.put<ApiResponse>(`/api/v1/vouchers/${voucherNo}`, data);
};

export const deleteVoucher = (voucherNo: string) => {
  return request.delete<ApiResponse>(`/api/v1/vouchers/${voucherNo}`);
};

export const checkBalance = (details: VoucherDetailItemDto[]) => {
  return request.post<ApiResponse<BalanceCheckDto>>('/api/v1/vouchers/check-balance', { details });
};

export const getCommonVoucher = (tKey: number) => {
  return request.get<ApiResponse<VoucherDetailDto>>(`/api/v1/vouchers/common-voucher/${tKey}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立 Vouchers 資料表
- [ ] 建立 VoucherDetails 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (6天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（借貸平衡檢查、會計科目類別檢查、IFRS會計科目檢查等）
- [ ] 套用常用傳票邏輯實作
- [ ] 關係人功能實作
- [ ] 自訂欄位功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (6天)
- [ ] API 呼叫函數
- [ ] 傳票列表頁面開發
- [ ] 傳票新增頁面開發（多筆新增）
- [ ] 傳票修改頁面開發
- [ ] 傳票主檔表單開發
- [ ] 傳票明細表格開發
- [ ] 套用常用傳票功能開發
- [ ] 借貸平衡檢查功能開發
- [ ] 關係人功能開發
- [ ] 自訂欄位功能開發
- [ ] 表單驗證
- [ ] 參數控制欄位顯示
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 業務邏輯測試（借貸平衡檢查、會計科目類別檢查等）

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 17天

---

## 六、注意事項

### 6.1 業務邏輯
- 傳票號碼必須唯一，通常由系統自動產生
- 傳票新增時需檢查借貸是否平衡（借方總額 = 貸方總額）
- 同一張傳票所輸入會計科目，必需為相同科目類別會科
- 選擇IFRS會計科目時，最後一碼為A
- 傳票日期預設值由參數 `DEF_VOUCHER_DATE` 控制
- 組織代號需依據使用者權限顯示（總公司/分公司）
- 傳票刪除時需檢查是否已審批

### 6.2 參數控制
- 關係人功能：由參數 `OBJECT_ID_TYPE` 控制（0:無, 1:必填, 2:選填）
- 廠商名稱欄位：由參數 `VOUCHER_VENDOR_NAME` 控制
- 自訂欄位：由參數 `VOUCHER_CUSTOM_FIELD` 控制（格式：欄位名稱1,欄位長度1,欄位名稱2,欄位長度2...）

### 6.3 資料驗證
- 傳票日期必須驗證
- 傳票明細至少需有一筆
- 每筆明細的借方金額或貸方金額至少需填一個
- 會計科目必須存在且可輸
- 組織代號必須存在
- 借貸必須平衡

### 6.4 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 傳票明細表格需支援虛擬滾動（超過30筆時）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增傳票成功
- [ ] 新增傳票失敗 (借貸不平衡)
- [ ] 新增傳票失敗 (會計科目類別不一致)
- [ ] 修改傳票成功
- [ ] 修改傳票失敗 (不存在)
- [ ] 刪除傳票成功
- [ ] 刪除傳票失敗 (已審批)
- [ ] 套用常用傳票成功
- [ ] 借貸平衡檢查
- [ ] 查詢傳票列表成功
- [ ] 查詢單筆傳票成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 參數控制欄位顯示測試
- [ ] 多筆新增功能測試
- [ ] 套用常用傳票功能測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 多筆明細新增測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST221_FI.ASP` - 傳票維護作業多筆新增畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FI.ASP` - 傳票維護作業新增畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FU.ASP` - 傳票維護作業修改畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FD.ASP` - 傳票維護作業刪除畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FQ.ASP` - 傳票維護作業查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FB.ASP` - 傳票維護作業瀏覽畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_PR.ASP` - 傳票維護作業報表畫面

### 8.2 資料庫 Schema
- 舊系統資料表：`VOUCHER_M` (傳票主檔), `VOUCHER_D` (傳票明細)
- 主要欄位：VOUCHER_NO, VOUCHER_DATE, VOUCHER_TYPE, SITE_ID, VOUCHER_NOTES, VENDOR_ID, VENDOR_NAME, STATUS, APPROVAL_STATUS, SN, DC, STYPE_ID, D_AMT, C_AMT, ORG_ID, NOTES, VENDOR_ID, ACCT_KEY, OBJECT_ID, CUSTOM_FIELD1, CUSTOM_FIELD2, CUSTOM_FIELD3

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


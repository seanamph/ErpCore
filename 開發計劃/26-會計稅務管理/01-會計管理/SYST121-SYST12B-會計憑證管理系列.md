# SYST121-SYST12B - 會計憑證管理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST121-SYST12B 系列
- **功能名稱**: 會計憑證管理系列
- **功能描述**: 提供會計憑證資料的新增、修改、刪除、查詢功能，包含傳票型態設定、常用傳票資料維護、傳票處理等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST121_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYST000/SYST122_FI.ASP` (新增 - 傳票型態設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST122_FU.ASP` (修改 - 傳票型態設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST122_FD.ASP` (刪除 - 傳票型態設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST122_FQ.ASP` (查詢 - 傳票型態設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST122_FB.ASP` (瀏覽 - 傳票型態設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST122_PR.ASP` (報表 - 傳票型態設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST123_FI.ASP` (新增 - 常用傳票資料)
  - `WEB/IMS_CORE/ASP/SYST000/SYST123_FU.ASP` (修改 - 常用傳票資料)
  - `WEB/IMS_CORE/ASP/SYST000/SYST123_FD.ASP` (刪除 - 常用傳票資料)
  - `WEB/IMS_CORE/ASP/SYST000/SYST123_FQ.ASP` (查詢 - 常用傳票資料)
  - `WEB/IMS_CORE/ASP/SYST000/SYST123_FB.ASP` (瀏覽 - 常用傳票資料)
  - `WEB/IMS_CORE/ASP/SYST000/SYST123_FS.ASP` (排序 - 常用傳票資料)
  - `WEB/IMS_CORE/ASP/SYST000/SYST125_*.ASP` (傳票處理相關)
  - `WEB/IMS_CORE/ASP/SYST000/SYST126_*.ASP` (傳票處理相關)
  - `WEB/IMS_CORE/ASP/SYST000/SYST127_*.ASP` (傳票處理相關)
  - `WEB/IMS_CORE/ASP/SYST000/SYST128_*.ASP` (傳票處理相關)
  - `WEB/IMS_CORE/ASP/SYST000/SYST12A_*.ASP` (傳票擴展功能)
  - `WEB/IMS_CORE/ASP/SYST000/SYST12B_*.ASP` (傳票擴展功能)

### 1.2 業務需求
- 管理傳票型態設定（VOUCHER_TYPE）
- 管理常用傳票資料（常用傳票主檔及明細）
- 支援傳票處理功能（傳票新增、修改、刪除、查詢）
- 支援傳票批量處理
- 支援傳票審批流程
- 支援傳票列印功能
- 支援傳票匯出功能
- 支援傳票狀態管理（啟用/停用）
- 支援傳票自訂欄位（依據參數設定）
- 支援廠商名稱欄位（依據參數設定）
- 支援關係人功能（依據參數設定）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `VoucherTypes` (對應舊系統 `VOUCHER_TYPE`)

```sql
CREATE TABLE [dbo].[VoucherTypes] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 型態代號 (VOUCHER_ID)
    [VoucherName] NVARCHAR(200) NOT NULL, -- 型態名稱 (VOUCHER_NAME)
    [Status] NVARCHAR(1) NULL DEFAULT '1', -- 狀態 (STATUS, 1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_VoucherTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_VoucherTypes_VoucherId] UNIQUE ([VoucherId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherTypes_VoucherId] ON [dbo].[VoucherTypes] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_VoucherTypes_Status] ON [dbo].[VoucherTypes] ([Status]);
```

### 2.2 主要資料表: `CommonVouchers` (對應舊系統常用傳票主檔)

```sql
CREATE TABLE [dbo].[CommonVouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 傳票代號
    [VoucherName] NVARCHAR(200) NOT NULL, -- 傳票名稱
    [VoucherType] NVARCHAR(50) NULL, -- 傳票型態 (外鍵至VoucherTypes)
    [SiteId] NVARCHAR(50) NULL, -- 店代號
    [VendorId] NVARCHAR(50) NULL, -- 廠客代號
    [VendorName] NVARCHAR(200) NULL, -- 廠商名稱
    [Notes] NVARCHAR(500) NULL, -- 摘要
    [CustomField1] NVARCHAR(200) NULL, -- 自訂欄位1
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    [CreatedPriority] INT NULL, -- 建立者等級
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組
    CONSTRAINT [PK_CommonVouchers] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CommonVouchers_VoucherId] ON [dbo].[CommonVouchers] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_CommonVouchers_VoucherType] ON [dbo].[CommonVouchers] ([VoucherType]);
CREATE NONCLUSTERED INDEX [IX_CommonVouchers_SiteId] ON [dbo].[CommonVouchers] ([SiteId]);
```

### 2.3 主要資料表: `CommonVoucherDetails` (對應舊系統常用傳票明細)

```sql
CREATE TABLE [dbo].[CommonVoucherDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔TKey (外鍵至CommonVouchers)
    [SeqNo] INT NOT NULL, -- 序號
    [StypeId] NVARCHAR(50) NULL, -- 會計科目代號 (外鍵至AccountSubjects)
    [DebitAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 借方金額
    [CreditAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 貸方金額
    [OrgId] NVARCHAR(50) NULL, -- 組織代號
    [Notes] NVARCHAR(500) NULL, -- 摘要
    [VendorId] NVARCHAR(50) NULL, -- 廠客代號
    [CustomField1] NVARCHAR(200) NULL, -- 自訂欄位1
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_CommonVoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_CommonVoucherDetails_CommonVouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[CommonVouchers]([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CommonVoucherDetails_VoucherTKey] ON [dbo].[CommonVoucherDetails] ([VoucherTKey]);
CREATE NONCLUSTERED INDEX [IX_CommonVoucherDetails_StypeId] ON [dbo].[CommonVoucherDetails] ([StypeId]);
CREATE NONCLUSTERED INDEX [IX_CommonVoucherDetails_SeqNo] ON [dbo].[CommonVoucherDetails] ([VoucherTKey], [SeqNo]);
```

### 2.4 資料字典

#### VoucherTypes 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | NO | - | 型態代號 | 唯一 |
| VoucherName | NVARCHAR | 200 | NO | - | 型態名稱 | - |
| Status | NVARCHAR | 1 | YES | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### CommonVouchers 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | NO | - | 傳票代號 | - |
| VoucherName | NVARCHAR | 200 | NO | - | 傳票名稱 | - |
| VoucherType | NVARCHAR | 50 | YES | - | 傳票型態 | 外鍵至VoucherTypes |
| SiteId | NVARCHAR | 50 | YES | - | 店代號 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠客代號 | - |
| VendorName | NVARCHAR | 200 | YES | - | 廠商名稱 | - |
| Notes | NVARCHAR | 500 | YES | - | 摘要 | - |
| CustomField1 | NVARCHAR | 200 | YES | - | 自訂欄位1 | 依據參數顯示 |

#### CommonVoucherDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherTKey | BIGINT | - | NO | - | 傳票主檔TKey | 外鍵至CommonVouchers |
| SeqNo | INT | - | NO | - | 序號 | - |
| StypeId | NVARCHAR | 50 | YES | - | 會計科目代號 | 外鍵至AccountSubjects |
| DebitAmount | DECIMAL | 18,2 | YES | 0 | 借方金額 | - |
| CreditAmount | DECIMAL | 18,2 | YES | 0 | 貸方金額 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代號 | - |
| Notes | NVARCHAR | 500 | YES | - | 摘要 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠客代號 | - |
| CustomField1 | NVARCHAR | 200 | YES | - | 自訂欄位1 | 依據參數顯示 |

---

## 三、後端 API 設計

### 3.1 傳票型態設定 API

#### 3.1.1 查詢傳票型態列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/voucher-types`
- **說明**: 查詢傳票型態列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherId",
    "sortOrder": "ASC",
    "filters": {
      "voucherId": "",
      "voucherName": "",
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
          "voucherId": "VT001",
          "voucherName": "一般傳票",
          "status": "1"
        }
      ],
      "totalCount": 10,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆傳票型態
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/voucher-types/{voucherId}`
- **說明**: 根據型態代號查詢單筆傳票型態資料

#### 3.1.3 新增傳票型態
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-types`
- **說明**: 新增傳票型態資料
- **請求格式**:
  ```json
  {
    "voucherId": "VT001",
    "voucherName": "一般傳票",
    "status": "1"
  }
  ```

#### 3.1.4 修改傳票型態
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/voucher-types/{voucherId}`
- **說明**: 修改傳票型態資料

#### 3.1.5 刪除傳票型態
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/voucher-types/{voucherId}`
- **說明**: 刪除傳票型態資料（需檢查是否有使用中的傳票）

### 3.2 常用傳票資料 API

#### 3.2.1 查詢常用傳票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/common-vouchers`
- **說明**: 查詢常用傳票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherId",
    "sortOrder": "ASC",
    "filters": {
      "voucherId": "",
      "voucherName": "",
      "voucherType": "",
      "siteId": ""
    }
  }
  ```

#### 3.2.2 查詢單筆常用傳票（含明細）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/common-vouchers/{tKey}`
- **說明**: 根據TKey查詢單筆常用傳票資料（含明細）

#### 3.2.3 新增常用傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/common-vouchers`
- **說明**: 新增常用傳票資料（含明細）
- **請求格式**:
  ```json
  {
    "voucherId": "CV001",
    "voucherName": "常用傳票範例",
    "voucherType": "VT001",
    "siteId": "SITE001",
    "vendorId": "V001",
    "vendorName": "廠商名稱",
    "notes": "摘要",
    "customField1": "",
    "details": [
      {
        "seqNo": 1,
        "stypeId": "1000",
        "debitAmount": 1000,
        "creditAmount": 0,
        "orgId": "ORG001",
        "notes": "明細摘要",
        "vendorId": "",
        "customField1": ""
      }
    ]
  }
  ```

#### 3.2.4 修改常用傳票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/common-vouchers/{tKey}`
- **說明**: 修改常用傳票資料（含明細）

#### 3.2.5 刪除常用傳票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/common-vouchers/{tKey}`
- **說明**: 刪除常用傳票資料（含明細，級聯刪除）

#### 3.2.6 批量新增常用傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/common-vouchers/batch`
- **說明**: 批量新增常用傳票資料

### 3.3 後端實作類別

#### 3.3.1 Controller: `VoucherTypesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/voucher-types")]
    [Authorize]
    public class VoucherTypesController : ControllerBase
    {
        private readonly IVoucherTypeService _voucherTypeService;
        
        public VoucherTypesController(IVoucherTypeService voucherTypeService)
        {
            _voucherTypeService = voucherTypeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherTypeDto>>>> GetVoucherTypes([FromQuery] VoucherTypeQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{voucherId}")]
        public async Task<ActionResult<ApiResponse<VoucherTypeDto>>> GetVoucherType(string voucherId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateVoucherType([FromBody] CreateVoucherTypeDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> UpdateVoucherType(string voucherId, [FromBody] UpdateVoucherTypeDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoucherType(string voucherId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.3.2 Controller: `CommonVouchersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/common-vouchers")]
    [Authorize]
    public class CommonVouchersController : ControllerBase
    {
        private readonly ICommonVoucherService _commonVoucherService;
        
        public CommonVouchersController(ICommonVoucherService commonVoucherService)
        {
            _commonVoucherService = commonVoucherService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CommonVoucherDto>>>> GetCommonVouchers([FromQuery] CommonVoucherQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<CommonVoucherDto>>> GetCommonVoucher(long tKey)
        {
            // 實作查詢單筆邏輯（含明細）
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateCommonVoucher([FromBody] CreateCommonVoucherDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateCommonVoucher(long tKey, [FromBody] UpdateCommonVoucherDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteCommonVoucher(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse>> BatchCreateCommonVouchers([FromBody] List<CreateCommonVoucherDto> dtos)
        {
            // 實作批量新增邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 傳票型態設定列表頁面 (`VoucherTypeList.vue`)
- **路徑**: `/accounting/voucher-types`
- **功能**: 顯示傳票型態列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (VoucherTypeSearchForm)
  - 資料表格 (VoucherTypeDataTable)
  - 新增/修改對話框 (VoucherTypeDialog)
  - 刪除確認對話框

#### 4.1.2 常用傳票列表頁面 (`CommonVoucherList.vue`)
- **路徑**: `/accounting/common-vouchers`
- **功能**: 顯示常用傳票列表，支援查詢、新增、修改、刪除、批量新增
- **主要元件**:
  - 查詢表單 (CommonVoucherSearchForm)
  - 資料表格 (CommonVoucherDataTable)
  - 新增/修改對話框 (CommonVoucherDialog)
  - 批量新增對話框 (CommonVoucherBatchDialog)
  - 刪除確認對話框

#### 4.1.3 常用傳票詳細頁面 (`CommonVoucherDetail.vue`)
- **路徑**: `/accounting/common-vouchers/:tKey`
- **功能**: 顯示常用傳票詳細資料（含明細），支援修改

### 4.2 UI 元件設計

#### 4.2.1 傳票型態設定表單元件 (`VoucherTypeForm.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
    <el-form-item label="型態代號" prop="voucherId">
      <el-input v-model="form.voucherId" :disabled="isEdit" placeholder="請輸入型態代號" />
    </el-form-item>
    <el-form-item label="型態名稱" prop="voucherName">
      <el-input v-model="form.voucherName" placeholder="請輸入型態名稱" />
    </el-form-item>
    <el-form-item label="狀態" prop="status" v-if="canEditStatus">
      <el-select v-model="form.status" placeholder="請選擇">
        <el-option label="啟用" value="1" />
        <el-option label="停用" value="0" />
      </el-select>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 常用傳票表單元件 (`CommonVoucherForm.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="傳票代號" prop="voucherId">
          <el-input v-model="form.voucherId" :disabled="isEdit" placeholder="請輸入傳票代號" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="傳票名稱" prop="voucherName">
          <el-input v-model="form.voucherName" placeholder="請輸入傳票名稱" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="傳票型態" prop="voucherType">
          <el-select v-model="form.voucherType" placeholder="請選擇" filterable>
            <el-option v-for="item in voucherTypeList" :key="item.voucherId" :label="item.voucherName" :value="item.voucherId" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="店代號" prop="siteId">
          <el-input v-model="form.siteId" placeholder="請輸入店代號" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20" v-if="showVendorName">
      <el-col :span="12">
        <el-form-item label="廠客代號" prop="vendorId">
          <el-input v-model="form.vendorId" placeholder="請輸入廠客代號" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="廠商名稱" prop="vendorName">
          <el-input v-model="form.vendorName" placeholder="請輸入廠商名稱" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-form-item label="摘要" prop="notes">
      <el-input v-model="form.notes" type="textarea" :rows="2" placeholder="請輸入摘要" />
    </el-form-item>
    <el-form-item label="自訂欄位1" prop="customField1" v-if="showCustomField">
      <el-input v-model="form.customField1" :maxlength="customFieldLength" placeholder="請輸入自訂欄位1" />
    </el-form-item>
    
    <!-- 明細表格 -->
    <el-divider>傳票明細</el-divider>
    <el-table :data="form.details" border>
      <el-table-column type="index" label="序號" width="60" />
      <el-table-column prop="stypeId" label="會計科目" width="150">
        <template #default="{ row, $index }">
          <el-select v-model="row.stypeId" placeholder="請選擇" filterable>
            <el-option v-for="item in accountSubjectList" :key="item.stypeId" :label="item.stypeName" :value="item.stypeId" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="debitAmount" label="借方金額" width="120">
        <template #default="{ row }">
          <el-input-number v-model="row.debitAmount" :min="0" :precision="2" />
        </template>
      </el-table-column>
      <el-table-column prop="creditAmount" label="貸方金額" width="120">
        <template #default="{ row }">
          <el-input-number v-model="row.creditAmount" :min="0" :precision="2" />
        </template>
      </el-table-column>
      <el-table-column prop="orgId" label="組織代號" width="120">
        <template #default="{ row }">
          <el-input v-model="row.orgId" placeholder="請輸入組織代號" />
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="摘要" min-width="150">
        <template #default="{ row }">
          <el-input v-model="row.notes" placeholder="請輸入摘要" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ $index }">
          <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-button type="primary" @click="handleAddDetail">新增明細</el-button>
  </el-form>
</template>
```

### 4.3 API 呼叫 (`voucher.api.ts`)
```typescript
import request from '@/utils/request';

export interface VoucherTypeDto {
  tKey: number;
  voucherId: string;
  voucherName: string;
  status?: string;
}

export interface CommonVoucherDto {
  tKey: number;
  voucherId: string;
  voucherName: string;
  voucherType?: string;
  siteId?: string;
  vendorId?: string;
  vendorName?: string;
  notes?: string;
  customField1?: string;
  details?: CommonVoucherDetailDto[];
}

export interface CommonVoucherDetailDto {
  tKey?: number;
  seqNo: number;
  stypeId?: string;
  debitAmount?: number;
  creditAmount?: number;
  orgId?: string;
  notes?: string;
  vendorId?: string;
  customField1?: string;
}

// API 函數
export const getVoucherTypeList = (query: any) => {
  return request.get<ApiResponse<PagedResult<VoucherTypeDto>>>('/api/v1/voucher-types', { params: query });
};

export const createVoucherType = (data: CreateVoucherTypeDto) => {
  return request.post<ApiResponse<string>>('/api/v1/voucher-types', data);
};

export const getCommonVoucherList = (query: any) => {
  return request.get<ApiResponse<PagedResult<CommonVoucherDto>>>('/api/v1/common-vouchers', { params: query });
};

export const getCommonVoucherById = (tKey: number) => {
  return request.get<ApiResponse<CommonVoucherDto>>(`/api/v1/common-vouchers/${tKey}`);
};

export const createCommonVoucher = (data: CreateCommonVoucherDto) => {
  return request.post<ApiResponse<number>>('/api/v1/common-vouchers', data);
};

export const updateCommonVoucher = (tKey: number, data: UpdateCommonVoucherDto) => {
  return request.put<ApiResponse>(`/api/v1/common-vouchers/${tKey}`, data);
};

export const deleteCommonVoucher = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/common-vouchers/${tKey}`);
};

export const batchCreateCommonVouchers = (data: CreateCommonVoucherDto[]) => {
  return request.post<ApiResponse>('/api/v1/common-vouchers/batch', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立VoucherTypes資料表結構
- [ ] 建立CommonVouchers資料表結構
- [ ] 建立CommonVoucherDetails資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (6天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（型態代號唯一性、借貸平衡檢查等）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (6天)
- [ ] API 呼叫函數
- [ ] 傳票型態設定列表頁面開發
- [ ] 常用傳票列表頁面開發
- [ ] 常用傳票詳細頁面開發
- [ ] 新增/修改對話框開發
- [ ] 批量新增對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 參數控制欄位顯示（自訂欄位、廠商名稱等）
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 業務邏輯測試（借貸平衡檢查、級聯刪除等）

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 17天

---

## 六、注意事項

### 6.1 業務邏輯
- 型態代號必須唯一
- 新增時需檢查型態代號是否已存在
- 刪除傳票型態時需檢查是否有使用中的傳票
- 常用傳票明細的借貸金額必須平衡（借方總額 = 貸方總額）
- 傳票名稱及摘要為必填欄位
- 套用常用傳票時將「傳票名稱」帶入「傳票摘要」

### 6.2 參數控制
- 自訂欄位顯示：由參數 `VOUCHER_CUSTOM_FIELD` 控制（格式：欄位名稱,欄位長度）
- 廠商名稱欄位：由參數 `VOUCHER_VENDOR_NAME` 控制
- 關係人功能：由參數 `OBJECT_ID_TYPE` 控制
- 組織代號：由參數 `ORG_ID` 控制預設值

### 6.3 權限控制
- 傳票型態設定的狀態欄位僅xcom使用者可維護
- 非xcom使用者僅能查看啟用狀態的傳票型態

### 6.4 資料驗證
- 型態代號必須唯一
- 必填欄位必須驗證
- 借貸金額必須平衡
- 會計科目必須存在

### 6.5 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制（傳票型態列表可快取）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增傳票型態成功
- [ ] 新增傳票型態失敗 (重複代號)
- [ ] 修改傳票型態成功
- [ ] 刪除傳票型態成功
- [ ] 刪除傳票型態失敗 (有使用中的傳票)
- [ ] 新增常用傳票成功
- [ ] 新增常用傳票失敗 (借貸不平衡)
- [ ] 修改常用傳票成功
- [ ] 刪除常用傳票成功（級聯刪除明細）
- [ ] 批量新增常用傳票成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 參數控制欄位顯示測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST121_FU.ASP` - 修改畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST122_FI.ASP` - 新增畫面（傳票型態設定）
- `WEB/IMS_CORE/ASP/SYST000/SYST122_FU.ASP` - 修改畫面（傳票型態設定）
- `WEB/IMS_CORE/ASP/SYST000/SYST122_FD.ASP` - 刪除畫面（傳票型態設定）
- `WEB/IMS_CORE/ASP/SYST000/SYST122_FQ.ASP` - 查詢畫面（傳票型態設定）
- `WEB/IMS_CORE/ASP/SYST000/SYST122_FB.ASP` - 瀏覽畫面（傳票型態設定）
- `WEB/IMS_CORE/ASP/SYST000/SYST122_PR.ASP` - 報表畫面（傳票型態設定）
- `WEB/IMS_CORE/ASP/SYST000/SYST123_FI.ASP` - 新增畫面（常用傳票資料）
- `WEB/IMS_CORE/ASP/SYST000/SYST123_FU.ASP` - 修改畫面（常用傳票資料）
- `WEB/IMS_CORE/ASP/SYST000/SYST123_FD.ASP` - 刪除畫面（常用傳票資料）
- `WEB/IMS_CORE/ASP/SYST000/SYST123_FQ.ASP` - 查詢畫面（常用傳票資料）
- `WEB/IMS_CORE/ASP/SYST000/SYST123_FB.ASP` - 瀏覽畫面（常用傳票資料）
- `WEB/IMS_CORE/ASP/SYST000/SYST123_FS.ASP` - 排序畫面（常用傳票資料）

### 8.2 資料庫 Schema
- 舊系統資料表：`VOUCHER_TYPE`（傳票型態設定）
- 舊系統資料表：常用傳票主檔及明細表
- 主要欄位：VOUCHER_ID, VOUCHER_NAME, STATUS, T_KEY等

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


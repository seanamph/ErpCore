# SYST002-SYST003 - 傳票轉入作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST002-SYST003 系列
- **功能名稱**: 傳票轉入作業系列
- **功能描述**: 提供外部系統傳票資料的轉入功能，包含住金傳票轉入作業和日立傳票資料轉入作業，支援檔案上傳、資料解析、資料驗證、資料轉入等完整流程
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST002_FI.ASP` (住金傳票轉入作業-轉入畫面)
  - `WEB/IMS_CORE/ASP/SYST000/SYST002_FQ.ASP` (住金傳票轉入作業-查詢畫面)
  - `WEB/IMS_CORE/ASP/SYST000/SYST003_FI.ASP` (日立傳票資料轉入作業-Call Stored Procedure)
  - `WEB/IMS_CORE/ASP/SYST000/SYST003_FQ.ASP` (日立傳票資料轉入作業-查詢畫面)

### 1.2 業務需求
- 支援住金傳票檔案上傳轉入
- 支援日立傳票資料轉入（日結傳票、月結傳票、供應商資料等）
- 支援檔案格式驗證
- 支援資料重複檢查（已匯入的資料不再重複匯入）
- 支援批次轉入處理
- 支援轉入結果查詢
- 支援轉入錯誤記錄
- 支援轉入進度追蹤
- 支援住金免稅發票土地註記
- 支援原卡暫存發票重複檢查（非退回暫存發票不能重複）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TmpVoucherM` (暫存傳票主檔)

與 SYSTA00-SYSTA70 系列共用相同的資料表結構，請參考 `SYSTA00-SYSTA70-暫存傳票審核作業系列.md` 的資料庫設計章節。

### 2.2 相關資料表

#### 2.2.1 `TmpVoucherD` - 暫存傳票明細檔
與 SYSTA00-SYSTA70 系列共用相同的資料表結構，請參考 `SYSTA00-SYSTA70-暫存傳票審核作業系列.md` 的資料庫設計章節。

#### 2.2.2 `VoucherImportLog` - 傳票轉入記錄檔
```sql
CREATE TABLE [dbo].[VoucherImportLog] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ImportType] NVARCHAR(50) NOT NULL, -- 轉入類型 (IMPORT_TYPE, AHM:住金, HTV:日立)
    [FileName] NVARCHAR(500) NULL, -- 檔案名稱 (FILE_NAME)
    [FilePath] NVARCHAR(1000) NULL, -- 檔案路徑 (FILE_PATH)
    [ImportDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 轉入日期 (IMPORT_DATE)
    [TotalCount] INT NULL DEFAULT 0, -- 總筆數 (TOTAL_COUNT)
    [SuccessCount] INT NULL DEFAULT 0, -- 成功筆數 (SUCCESS_COUNT)
    [FailCount] INT NULL DEFAULT 0, -- 失敗筆數 (FAIL_COUNT)
    [SkipCount] INT NULL DEFAULT 0, -- 跳過筆數 (SKIP_COUNT, 已存在)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:處理中, S:成功, F:失敗)
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_VoucherImportLog] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherImportLog_ImportType] ON [dbo].[VoucherImportLog] ([ImportType]);
CREATE NONCLUSTERED INDEX [IX_VoucherImportLog_ImportDate] ON [dbo].[VoucherImportLog] ([ImportDate]);
CREATE NONCLUSTERED INDEX [IX_VoucherImportLog_Status] ON [dbo].[VoucherImportLog] ([Status]);
```

#### 2.2.3 `VoucherImportDetail` - 傳票轉入明細記錄檔
```sql
CREATE TABLE [dbo].[VoucherImportDetail] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ImportLogTKey] BIGINT NOT NULL, -- 轉入記錄TKey (IMPORT_LOG_T_KEY)
    [RowNumber] INT NULL, -- 行號 (ROW_NUMBER)
    [VoucherTKey] BIGINT NULL, -- 傳票主檔TKey (VOUCHER_T_KEY, 轉入成功時填入)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:處理中, S:成功, F:失敗, K:跳過)
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
    [SourceData] NVARCHAR(MAX) NULL, -- 原始資料 (SOURCE_DATA, JSON格式)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    CONSTRAINT [PK_VoucherImportDetail] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_VoucherImportDetail_VoucherImportLog] FOREIGN KEY ([ImportLogTKey]) REFERENCES [dbo].[VoucherImportLog] ([TKey]) ON DELETE CASCADE,
    CONSTRAINT [FK_VoucherImportDetail_TmpVoucherM] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[TmpVoucherM] ([TKey])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherImportDetail_ImportLogTKey] ON [dbo].[VoucherImportDetail] ([ImportLogTKey]);
CREATE NONCLUSTERED INDEX [IX_VoucherImportDetail_Status] ON [dbo].[VoucherImportDetail] ([Status]);
CREATE NONCLUSTERED INDEX [IX_VoucherImportDetail_VoucherTKey] ON [dbo].[VoucherImportDetail] ([VoucherTKey]);
```

#### 2.2.4 `TmpSuplCust` - 暫存供應商客戶資料（日立轉入用）
```sql
CREATE TABLE [dbo].[TmpSuplCust] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VendorId] NVARCHAR(50) NOT NULL, -- 廠商代號 (VENDOR_ID)
    [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME)
    [UpFlag] NVARCHAR(1) NULL DEFAULT '0', -- 轉入標記 (UP_FLAG, 0:未轉入, 1:已轉入)
    [Status] NVARCHAR(10) NULL DEFAULT '1', -- 狀態 (STATUS)
    [SourceData] NVARCHAR(MAX) NULL, -- 原始資料 (SOURCE_DATA)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME),
    CONSTRAINT [PK_TmpSuplCust] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TmpSuplCust_VendorId] ON [dbo].[TmpSuplCust] ([VendorId]);
CREATE NONCLUSTERED INDEX [IX_TmpSuplCust_UpFlag] ON [dbo].[TmpSuplCust] ([UpFlag]);
```

### 2.3 資料字典

#### VoucherImportLog 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ImportType | NVARCHAR | 50 | NO | - | 轉入類型 | AHM:住金, HTV:日立 |
| FileName | NVARCHAR | 500 | YES | - | 檔案名稱 | - |
| FilePath | NVARCHAR | 1000 | YES | - | 檔案路徑 | - |
| ImportDate | DATETIME2 | - | NO | GETDATE() | 轉入日期 | - |
| TotalCount | INT | - | YES | 0 | 總筆數 | - |
| SuccessCount | INT | - | YES | 0 | 成功筆數 | - |
| FailCount | INT | - | YES | 0 | 失敗筆數 | - |
| SkipCount | INT | - | YES | 0 | 跳過筆數 | 已存在 |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:處理中, S:成功, F:失敗 |
| ErrorMessage | NVARCHAR | MAX | YES | - | 錯誤訊息 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### VoucherImportDetail 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ImportLogTKey | BIGINT | - | NO | - | 轉入記錄TKey | 外鍵至VoucherImportLog |
| RowNumber | INT | - | YES | - | 行號 | - |
| VoucherTKey | BIGINT | - | YES | - | 傳票主檔TKey | 外鍵至TmpVoucherM |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:處理中, S:成功, F:失敗, K:跳過 |
| ErrorMessage | NVARCHAR | MAX | YES | - | 錯誤訊息 | - |
| SourceData | NVARCHAR | MAX | YES | - | 原始資料 | JSON格式 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 上傳住金傳票檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-import/ahm/upload`
- **說明**: 上傳住金傳票檔案並開始轉入處理
- **請求格式**: `multipart/form-data`
  - `file`: 檔案（必填）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "檔案上傳成功，開始處理",
    "data": {
      "importLogTKey": 1,
      "fileName": "ahm_voucher_20240101.txt",
      "status": "P"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢轉入記錄列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/voucher-import/logs`
- **說明**: 查詢傳票轉入記錄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ImportDate",
    "sortOrder": "DESC",
    "filters": {
      "importType": "",
      "status": "",
      "importDateFrom": "",
      "importDateTo": "",
      "fileName": ""
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
          "importType": "AHM",
          "fileName": "ahm_voucher_20240101.txt",
          "importDate": "2024-01-01T10:00:00",
          "totalCount": 100,
          "successCount": 95,
          "failCount": 3,
          "skipCount": 2,
          "status": "S"
        }
      ],
      "totalCount": 50,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 3
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢轉入記錄明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/voucher-import/logs/{tKey}/details`
- **說明**: 查詢轉入記錄的明細資料
- **路徑參數**:
  - `tKey`: 轉入記錄TKey
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
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
      "importLog": {
        "tKey": 1,
        "importType": "AHM",
        "fileName": "ahm_voucher_20240101.txt",
        "importDate": "2024-01-01T10:00:00",
        "totalCount": 100,
        "successCount": 95,
        "failCount": 3,
        "skipCount": 2,
        "status": "S"
      },
      "details": [
        {
          "tKey": 1,
          "rowNumber": 1,
          "voucherTKey": 100,
          "status": "S",
          "errorMessage": null,
          "sourceData": "{...}"
        },
        {
          "tKey": 2,
          "rowNumber": 2,
          "voucherTKey": null,
          "status": "F",
          "errorMessage": "資料格式錯誤",
          "sourceData": "{...}"
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

#### 3.1.4 日立傳票轉入（日結傳票）
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-import/htv/daily`
- **說明**: 轉入日立日結傳票資料
- **請求格式**:
  ```json
  {
    "validateOnly": false
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "轉入成功",
    "data": {
      "importLogTKey": 1,
      "totalCount": 50,
      "successCount": 48,
      "failCount": 2,
      "skipCount": 0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 日立傳票轉入（月結傳票）
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-import/htv/monthly`
- **說明**: 轉入日立月結傳票資料
- **請求格式**: 同日結傳票
- **回應格式**: 同日結傳票

#### 3.1.6 日立供應商資料轉入
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-import/htv/supplier`
- **說明**: 轉入日立供應商資料
- **請求格式**: 同日結傳票
- **回應格式**: 同日結傳票

#### 3.1.7 查詢轉入進度
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/voucher-import/logs/{tKey}/progress`
- **說明**: 查詢轉入處理進度
- **路徑參數**:
  - `tKey`: 轉入記錄TKey
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "status": "P",
      "totalCount": 100,
      "processedCount": 50,
      "successCount": 48,
      "failCount": 2,
      "skipCount": 0,
      "progress": 50.0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 重新處理轉入記錄
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-import/logs/{tKey}/retry`
- **說明**: 重新處理失敗的轉入記錄
- **路徑參數**:
  - `tKey`: 轉入記錄TKey
- **回應格式**: 同轉入記錄

### 3.2 後端實作類別

#### 3.2.1 Controller: `VoucherImportController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/voucher-import")]
    [Authorize]
    public class VoucherImportController : ControllerBase
    {
        private readonly IVoucherImportService _voucherImportService;
        
        public VoucherImportController(IVoucherImportService voucherImportService)
        {
            _voucherImportService = voucherImportService;
        }
        
        [HttpPost("ahm/upload")]
        [RequestSizeLimit(100_000_000)] // 100MB
        public async Task<ActionResult<ApiResponse<ImportLogDto>>> UploadAhmFile(IFormFile file)
        {
            // 實作住金檔案上傳邏輯
        }
        
        [HttpGet("logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<ImportLogDto>>>> GetImportLogs([FromQuery] ImportLogQueryDto query)
        {
            // 實作查詢轉入記錄列表邏輯
        }
        
        [HttpGet("logs/{tKey}/details")]
        public async Task<ActionResult<ApiResponse<ImportLogDetailDto>>> GetImportLogDetails(long tKey, [FromQuery] ImportDetailQueryDto query)
        {
            // 實作查詢轉入記錄明細邏輯
        }
        
        [HttpPost("htv/daily")]
        public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportHtvDaily([FromBody] ImportHtvDto dto)
        {
            // 實作日立日結傳票轉入邏輯
        }
        
        [HttpPost("htv/monthly")]
        public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportHtvMonthly([FromBody] ImportHtvDto dto)
        {
            // 實作日立月結傳票轉入邏輯
        }
        
        [HttpPost("htv/supplier")]
        public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportHtvSupplier([FromBody] ImportHtvDto dto)
        {
            // 實作日立供應商資料轉入邏輯
        }
        
        [HttpGet("logs/{tKey}/progress")]
        public async Task<ActionResult<ApiResponse<ImportProgressDto>>> GetImportProgress(long tKey)
        {
            // 實作查詢轉入進度邏輯
        }
        
        [HttpPost("logs/{tKey}/retry")]
        public async Task<ActionResult<ApiResponse<ImportResultDto>>> RetryImport(long tKey)
        {
            // 實作重新處理邏輯
        }
    }
}
```

#### 3.2.2 Service: `VoucherImportService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IVoucherImportService
    {
        Task<ImportLogDto> UploadAhmFileAsync(IFormFile file, string userId);
        Task<PagedResult<ImportLogDto>> GetImportLogsAsync(ImportLogQueryDto query);
        Task<ImportLogDetailDto> GetImportLogDetailsAsync(long tKey, ImportDetailQueryDto query);
        Task<ImportResultDto> ImportHtvDailyAsync(ImportHtvDto dto, string userId);
        Task<ImportResultDto> ImportHtvMonthlyAsync(ImportHtvDto dto, string userId);
        Task<ImportResultDto> ImportHtvSupplierAsync(ImportHtvDto dto, string userId);
        Task<ImportProgressDto> GetImportProgressAsync(long tKey);
        Task<ImportResultDto> RetryImportAsync(long tKey, string userId);
    }
}
```

#### 3.2.3 Repository: `VoucherImportRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IVoucherImportRepository
    {
        Task<VoucherImportLog> CreateImportLogAsync(VoucherImportLog log);
        Task<VoucherImportLog> GetImportLogByIdAsync(long tKey);
        Task<PagedResult<VoucherImportLog>> GetImportLogsAsync(ImportLogQuery query);
        Task<VoucherImportDetail> CreateImportDetailAsync(VoucherImportDetail detail);
        Task<List<VoucherImportDetail>> GetImportDetailsAsync(long importLogTKey, ImportDetailQuery query);
        Task UpdateImportLogAsync(VoucherImportLog log);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 傳票轉入頁面 (`VoucherImport.vue`)
- **路徑**: `/accounting/voucher-import`
- **功能**: 提供傳票轉入功能，包含住金檔案上傳和日立傳票轉入
- **主要元件**:
  - 檔案上傳元件 (FileUpload)
  - 轉入按鈕組
  - 轉入記錄列表

#### 4.1.2 轉入記錄查詢頁面 (`VoucherImportLogQuery.vue`)
- **路徑**: `/accounting/voucher-import/logs`
- **功能**: 查詢轉入記錄列表
- **主要元件**:
  - 查詢表單 (ImportLogSearchForm)
  - 資料表格 (ImportLogDataTable)

#### 4.1.3 轉入記錄明細頁面 (`VoucherImportLogDetail.vue`)
- **路徑**: `/accounting/voucher-import/logs/{tKey}/details`
- **功能**: 顯示轉入記錄的明細資料
- **主要元件**:
  - 轉入記錄摘要 (ImportLogSummary)
  - 明細資料表格 (ImportDetailTable)
  - 錯誤訊息顯示

### 4.2 UI 元件設計

#### 4.2.1 檔案上傳元件 (`FileUpload.vue`)
```vue
<template>
  <div>
    <el-upload
      ref="uploadRef"
      :action="uploadUrl"
      :headers="uploadHeaders"
      :data="uploadData"
      :on-success="handleUploadSuccess"
      :on-error="handleUploadError"
      :before-upload="beforeUpload"
      :file-list="fileList"
      :auto-upload="false"
      drag
    >
      <el-icon class="el-icon--upload"><upload-filled /></el-icon>
      <div class="el-upload__text">
        將檔案拖到此處，或<em>點擊上傳</em>
      </div>
      <template #tip>
        <div class="el-upload__tip">
          支援 .txt, .csv 格式檔案，檔案大小不超過 100MB
        </div>
      </template>
    </el-upload>
    <el-button type="primary" @click="handleSubmit" :loading="uploading">開始上傳</el-button>
  </div>
</template>
```

#### 4.2.2 轉入記錄表格元件 (`ImportLogDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="logList" v-loading="loading">
      <el-table-column prop="importType" label="轉入類型" width="100">
        <template #default="{ row }">
          <el-tag>{{ getImportTypeText(row.importType) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="fileName" label="檔案名稱" min-width="200" show-overflow-tooltip />
      <el-table-column prop="importDate" label="轉入日期" width="160" />
      <el-table-column prop="totalCount" label="總筆數" width="100" align="right" />
      <el-table-column prop="successCount" label="成功" width="100" align="right">
        <template #default="{ row }">
          <span style="color: green">{{ row.successCount }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="failCount" label="失敗" width="100" align="right">
        <template #default="{ row }">
          <span style="color: red">{{ row.failCount }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="skipCount" label="跳過" width="100" align="right">
        <template #default="{ row }">
          <span style="color: orange">{{ row.skipCount }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleViewDetail(row)">查看明細</el-button>
          <el-button type="warning" size="small" @click="handleRetry(row)" v-if="row.status === 'F'">重新處理</el-button>
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

### 4.3 API 呼叫 (`voucher-import.api.ts`)
```typescript
import request from '@/utils/request';

export interface ImportLogDto {
  tKey: number;
  importType: string;
  fileName?: string;
  filePath?: string;
  importDate: string;
  totalCount: number;
  successCount: number;
  failCount: number;
  skipCount: number;
  status: string;
  errorMessage?: string;
}

export interface ImportLogDetailDto {
  importLog: ImportLogDto;
  details: ImportDetailDto[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
}

export interface ImportDetailDto {
  tKey: number;
  rowNumber?: number;
  voucherTKey?: number;
  status: string;
  errorMessage?: string;
  sourceData?: string;
}

export interface ImportLogQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    importType?: string;
    status?: string;
    importDateFrom?: string;
    importDateTo?: string;
    fileName?: string;
  };
}

export interface ImportResultDto {
  importLogTKey: number;
  totalCount: number;
  successCount: number;
  failCount: number;
  skipCount: number;
}

export interface ImportProgressDto {
  tKey: number;
  status: string;
  totalCount: number;
  processedCount: number;
  successCount: number;
  failCount: number;
  skipCount: number;
  progress: number;
}

// API 函數
export const uploadAhmFile = (file: File) => {
  const formData = new FormData();
  formData.append('file', file);
  return request.post<ApiResponse<ImportLogDto>>('/api/v1/voucher-import/ahm/upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
};

export const getImportLogs = (query: ImportLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<ImportLogDto>>>('/api/v1/voucher-import/logs', { params: query });
};

export const getImportLogDetails = (tKey: number, query: ImportDetailQueryDto) => {
  return request.get<ApiResponse<ImportLogDetailDto>>(`/api/v1/voucher-import/logs/${tKey}/details`, { params: query });
};

export const importHtvDaily = (data: ImportHtvDto) => {
  return request.post<ApiResponse<ImportResultDto>>('/api/v1/voucher-import/htv/daily', data);
};

export const importHtvMonthly = (data: ImportHtvDto) => {
  return request.post<ApiResponse<ImportResultDto>>('/api/v1/voucher-import/htv/monthly', data);
};

export const importHtvSupplier = (data: ImportHtvDto) => {
  return request.post<ApiResponse<ImportResultDto>>('/api/v1/voucher-import/htv/supplier', data);
};

export const getImportProgress = (tKey: number) => {
  return request.get<ApiResponse<ImportProgressDto>>(`/api/v1/voucher-import/logs/${tKey}/progress`);
};

export const retryImport = (tKey: number) => {
  return request.post<ApiResponse<ImportResultDto>>(`/api/v1/voucher-import/logs/${tKey}/retry`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立 VoucherImportLog 資料表
- [ ] 建立 VoucherImportDetail 資料表
- [ ] 建立 TmpSuplCust 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (6天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含檔案解析、資料驗證、轉入邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 檔案上傳處理
- [ ] 資料解析邏輯（住金、日立格式）
- [ ] 資料驗證邏輯
- [ ] 重複檢查邏輯
- [ ] 批次處理邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 檔案上傳元件開發
- [ ] 轉入記錄查詢頁面開發
- [ ] 轉入記錄明細頁面開發
- [ ] 轉入進度顯示元件
- [ ] 錯誤訊息顯示元件
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 檔案上傳測試
- [ ] 資料轉入測試
- [ ] 重複檢查測試
- [ ] 錯誤處理測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 15天

---

## 六、注意事項

### 6.1 業務邏輯
- 住金傳票檔案格式必須符合規範
- 已匯入的資料不再重複匯入（根據特定欄位判斷）
- 非退回暫存發票不能重複
- 住金免稅發票皆為土地，需增加發票土地註記
- 日立傳票轉入需支援日結、月結、供應商資料等多種類型
- 轉入處理可能需要較長時間，需支援非同步處理

### 6.2 檔案處理
- 檔案上傳大小限制（100MB）
- 檔案格式驗證（.txt, .csv等）
- 檔案編碼處理（UTF-8, Big5等）
- 檔案路徑安全管理
- 檔案清理機制（處理完成後可選擇保留或刪除）

### 6.3 資料驗證
- 檔案格式驗證
- 資料格式驗證
- 必填欄位驗證
- 資料關聯性驗證（科目、部門、專案等）
- 借貸平衡驗證

### 6.4 效能
- 大量資料轉入需使用批次處理
- 轉入處理需支援非同步處理
- 轉入進度需即時更新
- 需建立適當的索引以提升查詢效能

### 6.5 錯誤處理
- 檔案上傳錯誤處理
- 資料解析錯誤處理
- 資料驗證錯誤處理
- 資料庫錯誤處理
- 錯誤訊息需詳細記錄，方便問題排查

### 6.6 安全性
- 檔案上傳必須驗證檔案類型
- 檔案路徑必須安全處理，防止路徑遍歷攻擊
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

---

## 七、測試案例

### 7.1 單元測試
- [ ] 檔案上傳成功
- [ ] 檔案上傳失敗（檔案格式錯誤）
- [ ] 檔案上傳失敗（檔案大小超限）
- [ ] 住金傳票資料解析成功
- [ ] 住金傳票資料解析失敗（格式錯誤）
- [ ] 日立傳票轉入成功（日結）
- [ ] 日立傳票轉入成功（月結）
- [ ] 日立供應商資料轉入成功
- [ ] 資料重複檢查成功
- [ ] 資料驗證成功
- [ ] 資料驗證失敗（必填欄位缺失）
- [ ] 資料驗證失敗（借貸不平衡）

### 7.2 整合測試
- [ ] 完整轉入流程測試（住金）
- [ ] 完整轉入流程測試（日立日結）
- [ ] 完整轉入流程測試（日立月結）
- [ ] 完整轉入流程測試（日立供應商）
- [ ] 重複資料處理測試
- [ ] 錯誤處理測試
- [ ] 批次處理測試
- [ ] 非同步處理測試

### 7.3 效能測試
- [ ] 大量資料轉入測試
- [ ] 檔案上傳效能測試
- [ ] 資料解析效能測試
- [ ] 並發轉入測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST002_FI.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYST002_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYST003_FI.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYST003_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/Util/SYST000/util.asp`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYST000/` 相關資料表結構
- `WEB/IMS_CORE/ASP/include/SYST000/` 相關工具函數

### 8.3 相關功能
- 暫存傳票審核作業 (SYSTA00-SYSTA70)
- 會計憑證管理 (SYST121-SYST12B)
- 交易資料處理 (SYST221)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


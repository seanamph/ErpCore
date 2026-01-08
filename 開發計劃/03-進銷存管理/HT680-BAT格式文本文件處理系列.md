# HT680 - BAT格式文本文件處理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: HT680系列
- **功能名稱**: BAT格式文本文件處理系列
- **功能描述**: 提供HT680格式的文本文件處理功能，用於處理PDA到PC的數據傳輸，包含退貨檔、訂貨檔、盤點檔、POP卡製作檔、商品卡檔等格式的文本文件處理
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_BACK.cs` (退貨檔)
  - `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_INV.cs` (盤點檔)
  - `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_ORDER.cs` (訂貨檔)
  - `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_ORDER_6.cs` (訂貨檔版本六)
  - `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_POP.cs` (POP卡製作檔)
  - `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_PRIC.cs` (商品卡檔)

### 1.2 業務需求
- 處理PDA設備產生的文本文件
- 支援多種文本文件格式（退貨檔、訂貨檔、盤點檔、POP卡製作檔、商品卡檔）
- 支援定長欄位格式解析
- 支援數據驗證和轉換
- 支援批量文件處理
- 支援錯誤處理和日誌記錄
- 支援數據導入到資料庫

### 1.3 文件格式說明

#### 1.3.1 TXT_BACK - 退貨檔
- **檔案名稱**: BACK+店號.TXT
- **欄位區隔符**: 定長
- **資料流向**: PDA至PC
- **記錄區隔符**: CRLF
- **欄位**: 分店代號(4)、貨號/店內碼(8)、訂貨數量(8)

#### 1.3.2 TXT_INV - 盤點檔
- **檔案名稱**: INVA+店號.TXT(倉庫盤點檔) / INVB+店號.TXT(賣場盤點檔)
- **欄位區隔符**: 定長
- **資料流向**: PDA至PC
- **記錄區隔符**: CRLF
- **欄位**: 貨架(2)、盤點序號(4)、貨號/店內碼(8)、盤點數量(8)

#### 1.3.3 TXT_ORDER - 訂貨檔
- **檔案名稱**: ORDER+店號.TXT
- **欄位區隔符**: 定長
- **資料流向**: PDA至PC
- **記錄區隔符**: CRLF
- **欄位**: 分店代號(4)、貨號/店內碼(8)、訂貨數量(8)

#### 1.3.4 TXT_ORDER_6 - 訂貨檔版本六
- **檔案名稱**: ORDER+店號.TXT
- **欄位區隔符**: 定長
- **資料流向**: PDA至PC
- **記錄區隔符**: CRLF
- **欄位**: 分店代號(6)、貨號/店內碼(8)、訂貨數量(8)

#### 1.3.5 TXT_POP - POP卡製作檔
- **檔案名稱**: POP.TXT
- **欄位區隔符**: 定長
- **資料流向**: PDA至PC
- **記錄區隔符**: CRLF
- **欄位**: 分店代號(4)、貨號/店內碼(8)

#### 1.3.6 TXT_PRIC - 商品卡檔
- **檔案名稱**: Pric+店號.TXT
- **欄位區隔符**: 定長
- **資料流向**: PDA至PC
- **記錄區隔符**: CRLF
- **欄位**: 貨號/店內碼(8)

---

## 二、資料庫設計 (Schema)

### 2.1 文本文件處理記錄表: `TextFileProcessLog`

```sql
CREATE TABLE [dbo].[TextFileProcessLog] (
    [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FileName] NVARCHAR(255) NOT NULL,
    [FileType] NVARCHAR(50) NOT NULL, -- BACK, INV, ORDER, ORDER_6, POP, PRIC
    [ShopId] NVARCHAR(50) NULL,
    [TotalRecords] INT NULL DEFAULT 0,
    [SuccessRecords] INT NULL DEFAULT 0,
    [FailedRecords] INT NULL DEFAULT 0,
    [ProcessStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- PENDING, PROCESSING, COMPLETED, FAILED
    [ProcessStartTime] DATETIME2 NULL,
    [ProcessEndTime] DATETIME2 NULL,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_FileName] ON [dbo].[TextFileProcessLog] ([FileName]);
CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_FileType] ON [dbo].[TextFileProcessLog] ([FileType]);
CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_ShopId] ON [dbo].[TextFileProcessLog] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_ProcessStatus] ON [dbo].[TextFileProcessLog] ([ProcessStatus]);
CREATE NONCLUSTERED INDEX [IX_TextFileProcessLog_CreatedAt] ON [dbo].[TextFileProcessLog] ([CreatedAt]);
```

### 2.2 文本文件處理明細表: `TextFileProcessDetail`

```sql
CREATE TABLE [dbo].[TextFileProcessDetail] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [LogId] UNIQUEIDENTIFIER NOT NULL,
    [LineNumber] INT NOT NULL,
    [RawData] NVARCHAR(500) NULL,
    [ProcessStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- PENDING, SUCCESS, FAILED
    [ErrorMessage] NVARCHAR(500) NULL,
    [ProcessedData] NVARCHAR(MAX) NULL, -- JSON格式存儲解析後的數據
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_TextFileProcessDetail_Log] FOREIGN KEY ([LogId]) REFERENCES [dbo].[TextFileProcessLog] ([LogId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TextFileProcessDetail_LogId] ON [dbo].[TextFileProcessDetail] ([LogId]);
CREATE NONCLUSTERED INDEX [IX_TextFileProcessDetail_ProcessStatus] ON [dbo].[TextFileProcessDetail] ([ProcessStatus]);
```

### 2.3 相關資料表

#### 2.3.1 `Shops` - 店別主檔
```sql
-- 用於驗證分店代號
-- 參考店別設計
```

#### 2.3.2 `Products` - 商品主檔
```sql
-- 用於驗證貨號/店內碼
-- 參考商品主檔設計
```

#### 2.3.3 `InventoryRecords` - 盤點記錄表
```sql
-- 用於存儲盤點檔處理後的數據
-- 參考盤點管理設計
```

#### 2.3.4 `OrderRecords` - 訂貨記錄表
```sql
-- 用於存儲訂貨檔處理後的數據
-- 參考採購管理設計
```

#### 2.3.5 `ReturnRecords` - 退貨記錄表
```sql
-- 用於存儲退貨檔處理後的數據
-- 參考退貨管理設計
```

### 2.4 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| LogId | UNIQUEIDENTIFIER | - | NO | NEWID() | 處理記錄ID | 主鍵 |
| FileName | NVARCHAR | 255 | NO | - | 文件名稱 | - |
| FileType | NVARCHAR | 50 | NO | - | 文件類型 | BACK, INV, ORDER, ORDER_6, POP, PRIC |
| ShopId | NVARCHAR | 50 | YES | - | 店別代碼 | - |
| TotalRecords | INT | - | YES | 0 | 總記錄數 | - |
| SuccessRecords | INT | - | YES | 0 | 成功記錄數 | - |
| FailedRecords | INT | - | YES | 0 | 失敗記錄數 | - |
| ProcessStatus | NVARCHAR | 20 | NO | 'PENDING' | 處理狀態 | PENDING, PROCESSING, COMPLETED, FAILED |
| ProcessStartTime | DATETIME2 | - | YES | - | 處理開始時間 | - |
| ProcessEndTime | DATETIME2 | - | YES | - | 處理結束時間 | - |
| ErrorMessage | NVARCHAR(MAX) | - | YES | - | 錯誤訊息 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 上傳文本文件
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/textfile/upload`
- **說明**: 上傳文本文件並開始處理
- **請求格式**: Multipart/form-data
  - `file`: 文本文件
  - `fileType`: 文件類型 (BACK, INV, ORDER, ORDER_6, POP, PRIC)
  - `shopId`: 店別代碼（可選）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "文件上傳成功",
    "data": {
      "logId": "guid",
      "fileName": "BACK001.TXT",
      "fileType": "BACK",
      "totalRecords": 100
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢處理記錄列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/textfile/process-logs`
- **說明**: 查詢文本文件處理記錄列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CreatedAt",
    "sortOrder": "DESC",
    "filters": {
      "fileName": "",
      "fileType": "",
      "shopId": "",
      "processStatus": ""
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
          "logId": "guid",
          "fileName": "BACK001.TXT",
          "fileType": "BACK",
          "shopId": "SHOP001",
          "totalRecords": 100,
          "successRecords": 95,
          "failedRecords": 5,
          "processStatus": "COMPLETED",
          "processStartTime": "2024-01-01T10:00:00",
          "processEndTime": "2024-01-01T10:05:00",
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

#### 3.1.3 查詢單筆處理記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/textfile/process-logs/{logId}`
- **說明**: 根據處理記錄ID查詢單筆記錄
- **路徑參數**:
  - `logId`: 處理記錄ID
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "logId": "guid",
      "fileName": "BACK001.TXT",
      "fileType": "BACK",
      "shopId": "SHOP001",
      "totalRecords": 100,
      "successRecords": 95,
      "failedRecords": 5,
      "processStatus": "COMPLETED",
      "processStartTime": "2024-01-01T10:00:00",
      "processEndTime": "2024-01-01T10:05:00",
      "errorMessage": null,
      "createdAt": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 查詢處理明細列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/textfile/process-logs/{logId}/details`
- **說明**: 查詢處理記錄的明細列表
- **路徑參數**:
  - `logId`: 處理記錄ID
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "processStatus": "" // 可選，篩選處理狀態
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
          "detailId": "guid",
          "logId": "guid",
          "lineNumber": 1,
          "rawData": "SHOP001GOODS00100000100",
          "processStatus": "SUCCESS",
          "errorMessage": null,
          "processedData": {
            "shopId": "SHOP001",
            "goodsId": "GOODS001",
            "qty": "100"
          }
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

#### 3.1.5 重新處理文件
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/textfile/process-logs/{logId}/reprocess`
- **說明**: 重新處理失敗的文件
- **路徑參數**:
  - `logId`: 處理記錄ID
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "重新處理已開始",
    "data": {
      "logId": "guid"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 刪除處理記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/textfile/process-logs/{logId}`
- **說明**: 刪除處理記錄（軟刪除）
- **路徑參數**:
  - `logId`: 處理記錄ID
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

#### 3.1.7 下載處理結果
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/textfile/process-logs/{logId}/download`
- **說明**: 下載處理結果文件（Excel或CSV格式）
- **路徑參數**:
  - `logId`: 處理記錄ID
- **查詢參數**:
  - `format`: 文件格式 (excel, csv)
- **回應**: 文件下載

### 3.2 後端實作類別

#### 3.2.1 Controller: `TextFileController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/textfile")]
    [Authorize]
    public class TextFileController : ControllerBase
    {
        private readonly ITextFileService _textFileService;
        
        public TextFileController(ITextFileService textFileService)
        {
            _textFileService = textFileService;
        }
        
        [HttpPost("upload")]
        public async Task<ActionResult<ApiResponse<TextFileProcessLogDto>>> UploadFile(
            IFormFile file, 
            [FromForm] string fileType, 
            [FromForm] string shopId = null)
        {
            // 實作文件上傳邏輯
        }
        
        [HttpGet("process-logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<TextFileProcessLogDto>>>> GetProcessLogs(
            [FromQuery] TextFileProcessLogQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("process-logs/{logId}")]
        public async Task<ActionResult<ApiResponse<TextFileProcessLogDto>>> GetProcessLog(Guid logId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("process-logs/{logId}/details")]
        public async Task<ActionResult<ApiResponse<PagedResult<TextFileProcessDetailDto>>>> GetProcessDetails(
            Guid logId, 
            [FromQuery] TextFileProcessDetailQueryDto query)
        {
            // 實作查詢明細邏輯
        }
        
        [HttpPost("process-logs/{logId}/reprocess")]
        public async Task<ActionResult<ApiResponse>> ReprocessFile(Guid logId)
        {
            // 實作重新處理邏輯
        }
        
        [HttpDelete("process-logs/{logId}")]
        public async Task<ActionResult<ApiResponse>> DeleteProcessLog(Guid logId)
        {
            // 實作刪除邏輯
        }
        
        [HttpGet("process-logs/{logId}/download")]
        public async Task<IActionResult> DownloadProcessResult(Guid logId, [FromQuery] string format = "excel")
        {
            // 實作下載邏輯
        }
    }
}
```

#### 3.2.2 Service: `TextFileService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ITextFileService
    {
        Task<TextFileProcessLogDto> UploadAndProcessFileAsync(IFormFile file, string fileType, string shopId);
        Task<PagedResult<TextFileProcessLogDto>> GetProcessLogsAsync(TextFileProcessLogQueryDto query);
        Task<TextFileProcessLogDto> GetProcessLogByIdAsync(Guid logId);
        Task<PagedResult<TextFileProcessDetailDto>> GetProcessDetailsAsync(Guid logId, TextFileProcessDetailQueryDto query);
        Task ReprocessFileAsync(Guid logId);
        Task DeleteProcessLogAsync(Guid logId);
        Task<byte[]> DownloadProcessResultAsync(Guid logId, string format);
    }
}
```

#### 3.2.3 Parser: `HT680TextFileParser.cs`
```csharp
namespace RSL.IMS3.Application.Parsers
{
    public interface IHT680TextFileParser
    {
        TextFileParseResult ParseBackFile(string content, string shopId);
        TextFileParseResult ParseInvFile(string content, string shopId);
        TextFileParseResult ParseOrderFile(string content, string shopId);
        TextFileParseResult ParseOrder6File(string content, string shopId);
        TextFileParseResult ParsePopFile(string content, string shopId);
        TextFileParseResult ParsePricFile(string content, string shopId);
    }
}
```

#### 3.2.4 Repository: `TextFileProcessLogRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ITextFileProcessLogRepository
    {
        Task<TextFileProcessLog> GetByIdAsync(Guid logId);
        Task<PagedResult<TextFileProcessLog>> GetPagedAsync(TextFileProcessLogQuery query);
        Task<TextFileProcessLog> CreateAsync(TextFileProcessLog log);
        Task<TextFileProcessLog> UpdateAsync(TextFileProcessLog log);
        Task DeleteAsync(Guid logId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 文本文件上傳頁面 (`TextFileUpload.vue`)
- **路徑**: `/inventory/textfile/upload`
- **功能**: 上傳文本文件並開始處理
- **主要元件**:
  - 文件上傳元件 (FileUpload)
  - 文件類型選擇 (FileTypeSelect)
  - 店別選擇 (ShopSelect)
  - 處理進度顯示 (ProcessProgress)

#### 4.1.2 處理記錄列表頁面 (`TextFileProcessLogList.vue`)
- **路徑**: `/inventory/textfile/process-logs`
- **功能**: 顯示處理記錄列表，支援查詢、查看明細、重新處理、刪除
- **主要元件**:
  - 查詢表單 (ProcessLogSearchForm)
  - 資料表格 (ProcessLogDataTable)
  - 處理明細對話框 (ProcessDetailDialog)
  - 重新處理確認對話框

### 4.2 UI 元件設計

#### 4.2.1 文件上傳元件 (`FileUpload.vue`)
```vue
<template>
  <div>
    <el-upload
      ref="uploadRef"
      :action="uploadUrl"
      :data="uploadData"
      :before-upload="handleBeforeUpload"
      :on-success="handleUploadSuccess"
      :on-error="handleUploadError"
      :on-progress="handleUploadProgress"
      :file-list="fileList"
      drag
    >
      <el-icon class="el-icon--upload"><upload-filled /></el-icon>
      <div class="el-upload__text">
        將文件拖到此處，或<em>點擊上傳</em>
      </div>
      <template #tip>
        <div class="el-upload__tip">
          支援 .txt 文件，文件大小不超過 10MB
        </div>
      </template>
    </el-upload>
    <el-form :model="uploadForm" label-width="120px" style="margin-top: 20px">
      <el-form-item label="文件類型" required>
        <el-select v-model="uploadForm.fileType" placeholder="請選擇文件類型">
          <el-option label="退貨檔 (BACK)" value="BACK" />
          <el-option label="盤點檔 (INV)" value="INV" />
          <el-option label="訂貨檔 (ORDER)" value="ORDER" />
          <el-option label="訂貨檔版本六 (ORDER_6)" value="ORDER_6" />
          <el-option label="POP卡製作檔 (POP)" value="POP" />
          <el-option label="商品卡檔 (PRIC)" value="PRIC" />
        </el-select>
      </el-form-item>
      <el-form-item label="店別">
        <el-select v-model="uploadForm.shopId" placeholder="請選擇店別" clearable>
          <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
        </el-select>
      </el-form-item>
    </el-form>
  </div>
</template>
```

#### 4.2.2 處理記錄列表元件 (`ProcessLogDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="processLogList" v-loading="loading">
      <el-table-column prop="fileName" label="文件名稱" width="200" />
      <el-table-column prop="fileType" label="文件類型" width="120">
        <template #default="{ row }">
          <el-tag :type="getFileTypeTagType(row.fileType)">
            {{ getFileTypeText(row.fileType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="shopId" label="店別" width="120" />
      <el-table-column prop="totalRecords" label="總記錄數" width="100" />
      <el-table-column prop="successRecords" label="成功記錄數" width="120" />
      <el-table-column prop="failedRecords" label="失敗記錄數" width="120" />
      <el-table-column prop="processStatus" label="處理狀態" width="120">
        <template #default="{ row }">
          <el-tag :type="getStatusTagType(row.processStatus)">
            {{ getStatusText(row.processStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="processStartTime" label="處理開始時間" width="160" />
      <el-table-column prop="processEndTime" label="處理結束時間" width="160" />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleViewDetails(row)">查看明細</el-button>
          <el-button type="warning" size="small" @click="handleReprocess(row)" v-if="row.processStatus === 'FAILED'">重新處理</el-button>
          <el-button type="info" size="small" @click="handleDownload(row)">下載結果</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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

### 4.3 API 呼叫 (`textfile.api.ts`)
```typescript
import request from '@/utils/request';

export interface TextFileProcessLogDto {
  logId: string;
  fileName: string;
  fileType: string;
  shopId?: string;
  totalRecords: number;
  successRecords: number;
  failedRecords: number;
  processStatus: string;
  processStartTime?: string;
  processEndTime?: string;
  errorMessage?: string;
  createdAt: string;
}

export interface TextFileProcessDetailDto {
  detailId: string;
  logId: string;
  lineNumber: number;
  rawData: string;
  processStatus: string;
  errorMessage?: string;
  processedData?: any;
  createdAt: string;
}

export interface TextFileProcessLogQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    fileName?: string;
    fileType?: string;
    shopId?: string;
    processStatus?: string;
  };
}

// API 函數
export const uploadTextFile = (file: File, fileType: string, shopId?: string) => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('fileType', fileType);
  if (shopId) {
    formData.append('shopId', shopId);
  }
  return request.post<ApiResponse<TextFileProcessLogDto>>('/api/v1/textfile/upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
};

export const getProcessLogList = (query: TextFileProcessLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<TextFileProcessLogDto>>>('/api/v1/textfile/process-logs', { params: query });
};

export const getProcessLogById = (logId: string) => {
  return request.get<ApiResponse<TextFileProcessLogDto>>(`/api/v1/textfile/process-logs/${logId}`);
};

export const getProcessDetails = (logId: string, query: any) => {
  return request.get<ApiResponse<PagedResult<TextFileProcessDetailDto>>>(`/api/v1/textfile/process-logs/${logId}/details`, { params: query });
};

export const reprocessFile = (logId: string) => {
  return request.post<ApiResponse>(`/api/v1/textfile/process-logs/${logId}/reprocess`);
};

export const deleteProcessLog = (logId: string) => {
  return request.delete<ApiResponse>(`/api/v1/textfile/process-logs/${logId}`);
};

export const downloadProcessResult = (logId: string, format: string = 'excel') => {
  return request.get(`/api/v1/textfile/process-logs/${logId}/download`, {
    params: { format },
    responseType: 'blob'
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Parser 實作（各種文件格式解析）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 錯誤處理實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 文件上傳頁面開發
- [ ] 處理記錄列表頁面開發
- [ ] 處理明細對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 文件格式解析測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 錯誤處理測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 13天

---

## 六、注意事項

### 6.1 文件格式
- 必須嚴格按照定長格式解析
- 必須處理各種編碼格式（UTF-8, Big5等）
- 必須處理不同操作系統的行結束符（CRLF, LF, CR）
- 必須驗證文件格式正確性

### 6.2 數據驗證
- 必須驗證分店代號是否存在
- 必須驗證貨號/店內碼是否存在
- 必須驗證數量格式正確性
- 必須驗證必填欄位

### 6.3 效能
- 大量數據處理必須使用批量處理
- 必須使用異步處理避免阻塞
- 必須提供處理進度查詢
- 必須使用適當的索引

### 6.4 錯誤處理
- 必須記錄所有處理錯誤
- 必須提供詳細的錯誤訊息
- 必須支援部分成功的情況
- 必須提供重新處理功能

### 6.5 業務邏輯
- 處理後的數據必須導入到對應的業務表
- 必須支援數據回滾
- 必須記錄處理歷史
- 必須支援數據查詢和報表

---

## 七、測試案例

### 7.1 單元測試
- [ ] 退貨檔解析成功
- [ ] 盤點檔解析成功
- [ ] 訂貨檔解析成功
- [ ] 訂貨檔版本六解析成功
- [ ] POP卡製作檔解析成功
- [ ] 商品卡檔解析成功
- [ ] 文件格式錯誤處理
- [ ] 數據驗證測試
- [ ] 錯誤記錄測試

### 7.2 整合測試
- [ ] 完整文件上傳和處理流程測試
- [ ] 批量文件處理測試
- [ ] 數據導入測試
- [ ] 錯誤處理測試
- [ ] 重新處理測試

### 7.3 效能測試
- [ ] 大量數據處理測試
- [ ] 並發處理測試
- [ ] 文件大小限制測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_BACK.cs`
- `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_INV.cs`
- `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_ORDER.cs`
- `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_ORDER_6.cs`
- `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_POP.cs`
- `IMS3/HANSHIN/IMS3/App_Code/RSL/SYSW000/BAT_FORMAT/HT680/TXT_PRIC.cs`

### 8.2 相關功能
- `開發計劃/03-進銷存管理/SYSW170-POP卡商品卡列印作業.md`
- `開發計劃/04-採購管理/SYSW315-訂退貨申請作業.md`
- `開發計劃/06-盤點管理/SYSW53M-盤點維護作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


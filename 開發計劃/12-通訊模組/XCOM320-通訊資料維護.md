# XCOM320 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM320
- **功能名稱**: 通訊資料維護（檔案上傳作業）
- **功能描述**: 提供檔案上傳作業的查詢功能，包含上傳使用者代碼、上傳時間、檔案大小、附檔名等資訊查詢
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM320_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理檔案上傳記錄
- 支援依上傳使用者代碼查詢
- 支援依上傳時間範圍查詢
- 支援依檔案大小範圍查詢
- 支援依附檔名查詢
- 支援檔案上傳功能
- 支援檔案下載功能
- 支援檔案刪除功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `FileUploads` (檔案上傳記錄，對應舊系統 `FILE_UPLOAD`)

```sql
CREATE TABLE [dbo].[FileUploads] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UploadId] NVARCHAR(50) NOT NULL, -- 上傳記錄ID
    [UserId] NVARCHAR(50) NOT NULL, -- 上傳使用者代碼 (USER_ID)
    [ProgId] NVARCHAR(50) NULL, -- 系統代碼 (PROG_ID)
    [FileName] NVARCHAR(500) NOT NULL, -- 檔案名稱 (FILE_NAME)
    [FileSize] BIGINT NOT NULL DEFAULT 0, -- 檔案大小 (FILE_SIZE，單位：位元組)
    [FileExt] NVARCHAR(10) NULL, -- 附檔名 (FILE_EXT)
    [FilePath] NVARCHAR(1000) NOT NULL, -- 檔案路徑 (FILE_PATH)
    [UploadTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 上傳時間 (UPD_TIME)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:正常, 0:已刪除)
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_FileUploads] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_FileUploads_UploadId] UNIQUE ([UploadId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_FileUploads_UserId] ON [dbo].[FileUploads] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_ProgId] ON [dbo].[FileUploads] ([ProgId]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_UploadTime] ON [dbo].[FileUploads] ([UploadTime]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_FileExt] ON [dbo].[FileUploads] ([FileExt]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_Status] ON [dbo].[FileUploads] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| UploadId | NVARCHAR | 50 | NO | - | 上傳記錄ID | 唯一，主鍵候選 |
| UserId | NVARCHAR | 50 | NO | - | 上傳使用者代碼 | 外鍵至Users |
| ProgId | NVARCHAR | 50 | YES | - | 系統代碼 | 外鍵至Programs |
| FileName | NVARCHAR | 500 | NO | - | 檔案名稱 | - |
| FileSize | BIGINT | - | NO | 0 | 檔案大小 | 單位：位元組 |
| FileExt | NVARCHAR | 10 | YES | - | 附檔名 | - |
| FilePath | NVARCHAR | 1000 | NO | - | 檔案路徑 | - |
| UploadTime | DATETIME2 | - | NO | GETDATE() | 上傳時間 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:正常, 0:已刪除 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢檔案上傳記錄列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom320/file-uploads`
- **說明**: 查詢檔案上傳記錄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "UploadTime",
    "sortOrder": "DESC",
    "filters": {
      "userId": "",
      "progId": "",
      "fileExt": "",
      "uploadTimeFrom": "",
      "uploadTimeTo": "",
      "fileSizeFrom": null,
      "fileSizeTo": null,
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
          "uploadId": "UP001",
          "userId": "U001",
          "userName": "使用者名稱",
          "progId": "SYS0000",
          "progName": "系統管理",
          "fileName": "test.pdf",
          "fileSize": 1024000,
          "fileSizeFormatted": "1.00 MB",
          "fileExt": "pdf",
          "filePath": "/uploads/2024/01/UP001.pdf",
          "uploadTime": "2024-01-01T10:00:00",
          "status": "1",
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

#### 3.1.2 查詢單筆檔案上傳記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom320/file-uploads/{uploadId}`
- **說明**: 根據上傳記錄ID查詢單筆檔案上傳記錄資料
- **路徑參數**:
  - `uploadId`: 上傳記錄ID
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 上傳檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom320/file-uploads`
- **說明**: 上傳檔案
- **請求格式**: `multipart/form-data`
  ```
  file: (檔案)
  progId: (系統代碼，選填)
  notes: (備註，選填)
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "上傳成功",
    "data": {
      "uploadId": "UP001",
      "fileName": "test.pdf",
      "fileSize": 1024000,
      "filePath": "/uploads/2024/01/UP001.pdf"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 下載檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom320/file-uploads/{uploadId}/download`
- **說明**: 下載檔案
- **路徑參數**:
  - `uploadId`: 上傳記錄ID
- **回應格式**: 檔案下載（Content-Type: application/octet-stream）

#### 3.1.5 刪除檔案上傳記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom320/file-uploads/{uploadId}`
- **說明**: 刪除檔案上傳記錄（軟刪除，標記為已刪除狀態）
- **路徑參數**:
  - `uploadId`: 上傳記錄ID
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

#### 3.1.6 批次刪除檔案上傳記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom320/file-uploads/batch`
- **說明**: 批次刪除多筆檔案上傳記錄
- **請求格式**:
  ```json
  {
    "uploadIds": ["UP001", "UP002", "UP003"]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom320FileUploadsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom320/file-uploads")]
    [Authorize]
    public class XCom320FileUploadsController : ControllerBase
    {
        private readonly IXCom320FileUploadService _fileUploadService;
        
        public XCom320FileUploadsController(IXCom320FileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<FileUploadDto>>>> GetFileUploads([FromQuery] FileUploadQueryDto query)
        {
            var result = await _fileUploadService.GetFileUploadsAsync(query);
            return Ok(ApiResponse<PagedResult<FileUploadDto>>.Success(result));
        }
        
        [HttpGet("{uploadId}")]
        public async Task<ActionResult<ApiResponse<FileUploadDto>>> GetFileUpload(string uploadId)
        {
            var result = await _fileUploadService.GetFileUploadByIdAsync(uploadId);
            return Ok(ApiResponse<FileUploadDto>.Success(result));
        }
        
        [HttpPost]
        [RequestSizeLimit(100_000_000)] // 100MB
        public async Task<ActionResult<ApiResponse<FileUploadDto>>> UploadFile([FromForm] IFormFile file, [FromForm] string? progId, [FromForm] string? notes)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _fileUploadService.UploadFileAsync(userId, file, progId, notes);
            return Ok(ApiResponse<FileUploadDto>.Success(result));
        }
        
        [HttpGet("{uploadId}/download")]
        public async Task<ActionResult> DownloadFile(string uploadId)
        {
            var fileInfo = await _fileUploadService.GetFileInfoAsync(uploadId);
            if (fileInfo == null || !System.IO.File.Exists(fileInfo.FilePath))
            {
                return NotFound();
            }
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fileInfo.FilePath);
            return File(fileBytes, "application/octet-stream", fileInfo.FileName);
        }
        
        [HttpDelete("{uploadId}")]
        public async Task<ActionResult<ApiResponse>> DeleteFileUpload(string uploadId)
        {
            await _fileUploadService.DeleteFileUploadAsync(uploadId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("batch")]
        public async Task<ActionResult<ApiResponse>> BatchDeleteFileUploads([FromBody] BatchDeleteFileUploadsRequestDto request)
        {
            await _fileUploadService.BatchDeleteFileUploadsAsync(request.UploadIds);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `XCom320FileUploadService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom320FileUploadService
    {
        Task<PagedResult<FileUploadDto>> GetFileUploadsAsync(FileUploadQueryDto query);
        Task<FileUploadDto> GetFileUploadByIdAsync(string uploadId);
        Task<FileUploadDto> UploadFileAsync(string userId, IFormFile file, string? progId, string? notes);
        Task<FileInfoDto> GetFileInfoAsync(string uploadId);
        Task DeleteFileUploadAsync(string uploadId);
        Task BatchDeleteFileUploadsAsync(List<string> uploadIds);
    }
}
```

#### 3.2.3 Repository: `XCom320FileUploadRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXCom320FileUploadRepository
    {
        Task<FileUpload> GetByIdAsync(string uploadId);
        Task<PagedResult<FileUpload>> GetPagedAsync(FileUploadQuery query);
        Task<FileUpload> CreateAsync(FileUpload fileUpload);
        Task<FileUpload> UpdateAsync(FileUpload fileUpload);
        Task DeleteAsync(string uploadId);
        Task<bool> ExistsAsync(string uploadId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 檔案上傳記錄列表頁面 (`FileUploadList.vue`)
- **路徑**: `/xcom/file-uploads`
- **功能**: 顯示檔案上傳記錄列表，支援查詢、上傳、下載、刪除
- **主要元件**:
  - 查詢表單 (FileUploadSearchForm)
  - 資料表格 (FileUploadDataTable)
  - 檔案上傳對話框 (FileUploadDialog)
  - 刪除確認對話框

#### 4.1.2 檔案上傳詳細頁面 (`FileUploadDetail.vue`)
- **路徑**: `/xcom/file-uploads/:uploadId`
- **功能**: 顯示檔案上傳記錄詳細資料，支援下載、刪除

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`FileUploadSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="上傳使用者代碼">
      <el-input v-model="searchForm.userId" placeholder="請輸入使用者代碼" clearable />
    </el-form-item>
    <el-form-item label="系統代碼">
      <el-select v-model="searchForm.progId" placeholder="請選擇系統" clearable filterable>
        <el-option v-for="prog in progList" :key="prog.progId" :label="prog.progName" :value="prog.progId" />
      </el-select>
    </el-form-item>
    <el-form-item label="附檔名">
      <el-input v-model="searchForm.fileExt" placeholder="請輸入附檔名" clearable />
    </el-form-item>
    <el-form-item label="上傳時間">
      <el-date-picker
        v-model="dateRange"
        type="datetimerange"
        range-separator="至"
        start-placeholder="開始時間"
        end-placeholder="結束時間"
        format="YYYY-MM-DD HH:mm:ss"
        value-format="YYYY-MM-DD HH:mm:ss"
      />
    </el-form-item>
    <el-form-item label="檔案大小">
      <el-input-number v-model="searchForm.fileSizeFrom" :min="0" placeholder="最小" style="width: 120px" />
      <span style="margin: 0 10px">至</span>
      <el-input-number v-model="searchForm.fileSizeTo" :min="0" placeholder="最大" style="width: 120px" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`FileUploadDataTable.vue`)
```vue
<template>
  <div>
    <div style="margin-bottom: 10px">
      <el-button type="primary" @click="handleUpload">上傳檔案</el-button>
      <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">批次刪除</el-button>
    </div>
    <el-table :data="fileUploadList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="uploadId" label="上傳記錄ID" width="150" />
      <el-table-column prop="userName" label="上傳使用者" width="120" />
      <el-table-column prop="progName" label="系統" width="150" />
      <el-table-column prop="fileName" label="檔案名稱" min-width="200" />
      <el-table-column prop="fileSizeFormatted" label="檔案大小" width="100" align="right" />
      <el-table-column prop="fileExt" label="附檔名" width="80" />
      <el-table-column prop="uploadTime" label="上傳時間" width="160" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '正常' : '已刪除' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleDownload(row)">下載</el-button>
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

#### 4.2.3 檔案上傳對話框 (`FileUploadDialog.vue`)
```vue
<template>
  <el-dialog
    title="上傳檔案"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="系統代碼" prop="progId">
        <el-select v-model="form.progId" placeholder="請選擇系統" filterable clearable>
          <el-option v-for="prog in progList" :key="prog.progId" :label="prog.progName" :value="prog.progId" />
        </el-select>
      </el-form-item>
      <el-form-item label="檔案" prop="file">
        <el-upload
          ref="uploadRef"
          :auto-upload="false"
          :on-change="handleFileChange"
          :file-list="fileList"
          :limit="1"
        >
          <el-button type="primary">選擇檔案</el-button>
          <template #tip>
            <div class="el-upload__tip">支援所有檔案格式，單檔最大100MB</div>
          </template>
        </el-upload>
      </el-form-item>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit" :loading="uploading">上傳</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom320.api.ts`)
```typescript
import request from '@/utils/request';

export interface FileUploadDto {
  tKey: number;
  uploadId: string;
  userId: string;
  userName?: string;
  progId?: string;
  progName?: string;
  fileName: string;
  fileSize: number;
  fileSizeFormatted?: string;
  fileExt?: string;
  filePath: string;
  uploadTime: string;
  status: string;
  notes?: string;
  createdBy?: string;
  createdAt: string;
  updatedBy?: string;
  updatedAt: string;
}

export interface FileUploadQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    userId?: string;
    progId?: string;
    fileExt?: string;
    uploadTimeFrom?: string;
    uploadTimeTo?: string;
    fileSizeFrom?: number;
    fileSizeTo?: number;
    status?: string;
  };
}

export interface UploadFileDto {
  file: File;
  progId?: string;
  notes?: string;
}

// API 函數
export const getFileUploadList = (query: FileUploadQueryDto) => {
  return request.get<ApiResponse<PagedResult<FileUploadDto>>>('/api/v1/xcom320/file-uploads', { params: query });
};

export const getFileUploadById = (uploadId: string) => {
  return request.get<ApiResponse<FileUploadDto>>(`/api/v1/xcom320/file-uploads/${uploadId}`);
};

export const uploadFile = (data: FormData) => {
  return request.post<ApiResponse<FileUploadDto>>('/api/v1/xcom320/file-uploads', data, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
};

export const downloadFile = (uploadId: string) => {
  return request.get(`/api/v1/xcom320/file-uploads/${uploadId}/download`, {
    responseType: 'blob'
  });
};

export const deleteFileUpload = (uploadId: string) => {
  return request.delete<ApiResponse>(`/api/v1/xcom320/file-uploads/${uploadId}`);
};

export const batchDeleteFileUploads = (uploadIds: string[]) => {
  return request.delete<ApiResponse>('/api/v1/xcom320/file-uploads/batch', { data: { uploadIds } });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立唯一約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 檔案上傳處理邏輯
- [ ] 檔案下載處理邏輯
- [ ] 檔案儲存管理
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 檔案上傳對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 檔案下載功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 檔案上傳下載測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 檔案上傳必須驗證檔案類型
- 檔案上傳必須限制檔案大小（建議最大100MB）
- 檔案路徑必須驗證，防止路徑遍歷攻擊
- 檔案下載必須檢查權限
- 敏感檔案必須加密儲存

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 檔案上傳必須使用非同步處理
- 大檔案上傳必須支援斷點續傳（可選）

### 6.3 資料驗證
- 上傳記錄ID必須唯一
- 必填欄位必須驗證
- 檔案大小必須驗證
- 檔案類型必須驗證

### 6.4 業務邏輯
- 刪除檔案時，必須同時刪除實體檔案（或標記為已刪除）
- 檔案儲存路徑必須按照日期組織（如：/uploads/2024/01/）
- 檔案名稱必須處理重複問題（使用UUID或時間戳）
- 必須記錄檔案上傳操作日誌

### 6.5 檔案管理
- 檔案儲存路徑配置
- 檔案清理策略（定期清理已刪除檔案）
- 檔案備份策略
- 檔案儲存空間管理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢檔案上傳記錄列表成功
- [ ] 查詢單筆檔案上傳記錄成功
- [ ] 上傳檔案成功
- [ ] 上傳檔案失敗 (檔案過大)
- [ ] 上傳檔案失敗 (不支援的檔案類型)
- [ ] 下載檔案成功
- [ ] 下載檔案失敗 (檔案不存在)
- [ ] 刪除檔案上傳記錄成功
- [ ] 批次刪除檔案上傳記錄成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 檔案上傳下載流程測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 大檔案上傳測試
- [ ] 並發上傳測試

### 7.4 安全性測試
- [ ] 檔案類型驗證測試
- [ ] 檔案大小限制測試
- [ ] 路徑遍歷攻擊測試
- [ ] 權限檢查測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM320_FQ.ASP` - 查詢畫面

### 8.2 相關功能
- XCOM330 - 檔案下傳作業
- XCOM360 - 上下傳記錄報表
- FILE_UPLOAD - 檔案上傳工具

### 8.3 資料庫 Schema
- 舊系統資料表：`FILE_UPLOAD`
- 主要欄位：USER_ID, PROG_ID, FILE_NAME, FILE_SIZE, FILE_EXT, UPD_TIME

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


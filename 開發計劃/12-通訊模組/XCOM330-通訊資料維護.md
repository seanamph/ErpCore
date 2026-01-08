# XCOM330 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM330
- **功能名稱**: 通訊資料維護（檔案下傳作業）
- **功能描述**: 提供檔案下傳作業的查詢、瀏覽、下載功能，包含上傳使用者代碼、上傳時間、檔案大小、附檔名等資訊查詢與下載
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM330_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM330_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM330_FU.ASP` (修改)

### 1.2 業務需求
- 管理檔案下傳記錄
- 支援依上傳使用者代碼查詢
- 支援依上傳時間範圍查詢
- 支援依檔案大小範圍查詢
- 支援依附檔名查詢
- 支援檔案下載功能
- 支援檔案記錄修改（備註等）
- 支援檔案記錄刪除

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `FileDownloads` (檔案下傳記錄，對應舊系統 `FILE_DOWNLOAD`)

```sql
CREATE TABLE [dbo].[FileDownloads] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DownloadId] NVARCHAR(50) NOT NULL, -- 下傳記錄ID
    [UploadId] NVARCHAR(50) NULL, -- 上傳記錄ID (關聯至FileUploads)
    [UserId] NVARCHAR(50) NOT NULL, -- 下載使用者代碼 (USER_ID)
    [ProgId] NVARCHAR(50) NULL, -- 系統代碼 (PROG_ID)
    [FileName] NVARCHAR(500) NOT NULL, -- 檔案名稱 (FILE_NAME)
    [FileSize] BIGINT NOT NULL DEFAULT 0, -- 檔案大小 (FILE_SIZE，單位：位元組)
    [FileExt] NVARCHAR(10) NULL, -- 附檔名 (FILE_EXT)
    [FilePath] NVARCHAR(1000) NOT NULL, -- 檔案路徑 (FILE_PATH)
    [DownloadTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 下載時間 (UPD_TIME)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:正常, 0:已刪除)
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_FileDownloads] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_FileDownloads_DownloadId] UNIQUE ([DownloadId]),
    CONSTRAINT [FK_FileDownloads_FileUploads] FOREIGN KEY ([UploadId]) REFERENCES [dbo].[FileUploads] ([UploadId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_FileDownloads_DownloadId] ON [dbo].[FileDownloads] ([DownloadId]);
CREATE NONCLUSTERED INDEX [IX_FileDownloads_UserId] ON [dbo].[FileDownloads] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_FileDownloads_UploadId] ON [dbo].[FileDownloads] ([UploadId]);
CREATE NONCLUSTERED INDEX [IX_FileDownloads_ProgId] ON [dbo].[FileDownloads] ([ProgId]);
CREATE NONCLUSTERED INDEX [IX_FileDownloads_DownloadTime] ON [dbo].[FileDownloads] ([DownloadTime]);
CREATE NONCLUSTERED INDEX [IX_FileDownloads_FileExt] ON [dbo].[FileDownloads] ([FileExt]);
CREATE NONCLUSTERED INDEX [IX_FileDownloads_Status] ON [dbo].[FileDownloads] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| DownloadId | NVARCHAR | 50 | NO | - | 下傳記錄ID | 唯一，主鍵候選 |
| UploadId | NVARCHAR | 50 | YES | - | 上傳記錄ID | 外鍵至FileUploads |
| UserId | NVARCHAR | 50 | NO | - | 下載使用者代碼 | 外鍵至Users |
| ProgId | NVARCHAR | 50 | YES | - | 系統代碼 | 外鍵至Programs |
| FileName | NVARCHAR | 500 | NO | - | 檔案名稱 | - |
| FileSize | BIGINT | - | NO | 0 | 檔案大小 | 單位：位元組 |
| FileExt | NVARCHAR | 10 | YES | - | 附檔名 | - |
| FilePath | NVARCHAR | 1000 | NO | - | 檔案路徑 | - |
| DownloadTime | DATETIME2 | - | NO | GETDATE() | 下載時間 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:正常, 0:已刪除 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢檔案下傳記錄列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom330/file-downloads`
- **說明**: 查詢檔案下傳記錄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "DownloadTime",
    "sortOrder": "DESC",
    "filters": {
      "userId": "",
      "uploadId": "",
      "progId": "",
      "fileExt": "",
      "downloadTimeFrom": "",
      "downloadTimeTo": "",
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
          "downloadId": "DN001",
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
          "downloadTime": "2024-01-01T10:00:00",
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

#### 3.1.2 查詢單筆檔案下傳記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom330/file-downloads/{downloadId}`
- **說明**: 根據下傳記錄ID查詢單筆檔案下傳記錄資料
- **路徑參數**:
  - `downloadId`: 下傳記錄ID
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 下載檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom330/file-downloads/{downloadId}/download`
- **說明**: 下載檔案（記錄下載操作）
- **路徑參數**:
  - `downloadId`: 下傳記錄ID
- **回應格式**: 檔案下載（Content-Type: application/octet-stream）
- **業務邏輯**: 下載時自動建立下載記錄

#### 3.1.4 修改檔案下傳記錄
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom330/file-downloads/{downloadId}`
- **說明**: 修改檔案下傳記錄（主要用於修改備註）
- **路徑參數**:
  - `downloadId`: 下傳記錄ID
- **請求格式**:
  ```json
  {
    "notes": "備註內容"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "修改成功",
    "data": {
      "downloadId": "DN001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 刪除檔案下傳記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom330/file-downloads/{downloadId}`
- **說明**: 刪除檔案下傳記錄（軟刪除，標記為已刪除狀態）
- **路徑參數**:
  - `downloadId`: 下傳記錄ID
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

#### 3.1.6 批次刪除檔案下傳記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom330/file-downloads/batch`
- **說明**: 批次刪除多筆檔案下傳記錄
- **請求格式**:
  ```json
  {
    "downloadIds": ["DN001", "DN002", "DN003"]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom330FileDownloadsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom330/file-downloads")]
    [Authorize]
    public class XCom330FileDownloadsController : ControllerBase
    {
        private readonly IXCom330FileDownloadService _fileDownloadService;
        
        public XCom330FileDownloadsController(IXCom330FileDownloadService fileDownloadService)
        {
            _fileDownloadService = fileDownloadService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<FileDownloadDto>>>> GetFileDownloads([FromQuery] FileDownloadQueryDto query)
        {
            var result = await _fileDownloadService.GetFileDownloadsAsync(query);
            return Ok(ApiResponse<PagedResult<FileDownloadDto>>.Success(result));
        }
        
        [HttpGet("{downloadId}")]
        public async Task<ActionResult<ApiResponse<FileDownloadDto>>> GetFileDownload(string downloadId)
        {
            var result = await _fileDownloadService.GetFileDownloadByIdAsync(downloadId);
            return Ok(ApiResponse<FileDownloadDto>.Success(result));
        }
        
        [HttpGet("{downloadId}/download")]
        public async Task<ActionResult> DownloadFile(string downloadId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fileInfo = await _fileDownloadService.DownloadFileAsync(downloadId, userId);
            if (fileInfo == null || !System.IO.File.Exists(fileInfo.FilePath))
            {
                return NotFound();
            }
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fileInfo.FilePath);
            return File(fileBytes, "application/octet-stream", fileInfo.FileName);
        }
        
        [HttpPut("{downloadId}")]
        public async Task<ActionResult<ApiResponse>> UpdateFileDownload(string downloadId, [FromBody] UpdateFileDownloadDto dto)
        {
            await _fileDownloadService.UpdateFileDownloadAsync(downloadId, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("{downloadId}")]
        public async Task<ActionResult<ApiResponse>> DeleteFileDownload(string downloadId)
        {
            await _fileDownloadService.DeleteFileDownloadAsync(downloadId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("batch")]
        public async Task<ActionResult<ApiResponse>> BatchDeleteFileDownloads([FromBody] BatchDeleteFileDownloadsRequestDto request)
        {
            await _fileDownloadService.BatchDeleteFileDownloadsAsync(request.DownloadIds);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `XCom330FileDownloadService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom330FileDownloadService
    {
        Task<PagedResult<FileDownloadDto>> GetFileDownloadsAsync(FileDownloadQueryDto query);
        Task<FileDownloadDto> GetFileDownloadByIdAsync(string downloadId);
        Task<FileInfoDto> DownloadFileAsync(string downloadId, string userId);
        Task UpdateFileDownloadAsync(string downloadId, UpdateFileDownloadDto dto);
        Task DeleteFileDownloadAsync(string downloadId);
        Task BatchDeleteFileDownloadsAsync(List<string> downloadIds);
    }
}
```

#### 3.2.3 Repository: `XCom330FileDownloadRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXCom330FileDownloadRepository
    {
        Task<FileDownload> GetByIdAsync(string downloadId);
        Task<PagedResult<FileDownload>> GetPagedAsync(FileDownloadQuery query);
        Task<FileDownload> CreateAsync(FileDownload fileDownload);
        Task<FileDownload> UpdateAsync(FileDownload fileDownload);
        Task DeleteAsync(string downloadId);
        Task<bool> ExistsAsync(string downloadId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 檔案下傳記錄列表頁面 (`FileDownloadList.vue`)
- **路徑**: `/xcom/file-downloads`
- **功能**: 顯示檔案下傳記錄列表，支援查詢、下載、修改、刪除
- **主要元件**:
  - 查詢表單 (FileDownloadSearchForm)
  - 資料表格 (FileDownloadDataTable)
  - 修改對話框 (FileDownloadDialog)
  - 刪除確認對話框

#### 4.1.2 檔案下傳記錄詳細頁面 (`FileDownloadDetail.vue`)
- **路徑**: `/xcom/file-downloads/:downloadId`
- **功能**: 顯示檔案下傳記錄詳細資料，支援下載、修改、刪除

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`FileDownloadSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="下載使用者代碼">
      <el-input v-model="searchForm.userId" placeholder="請輸入使用者代碼" clearable />
    </el-form-item>
    <el-form-item label="上傳記錄ID">
      <el-input v-model="searchForm.uploadId" placeholder="請輸入上傳記錄ID" clearable />
    </el-form-item>
    <el-form-item label="系統代碼">
      <el-select v-model="searchForm.progId" placeholder="請選擇系統" clearable filterable>
        <el-option v-for="prog in progList" :key="prog.progId" :label="prog.progName" :value="prog.progId" />
      </el-select>
    </el-form-item>
    <el-form-item label="附檔名">
      <el-input v-model="searchForm.fileExt" placeholder="請輸入附檔名" clearable />
    </el-form-item>
    <el-form-item label="下載時間">
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

#### 4.2.2 資料表格元件 (`FileDownloadDataTable.vue`)
```vue
<template>
  <div>
    <div style="margin-bottom: 10px">
      <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">批次刪除</el-button>
    </div>
    <el-table :data="fileDownloadList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="downloadId" label="下傳記錄ID" width="150" />
      <el-table-column prop="uploadId" label="上傳記錄ID" width="150" />
      <el-table-column prop="userName" label="下載使用者" width="120" />
      <el-table-column prop="progName" label="系統" width="150" />
      <el-table-column prop="fileName" label="檔案名稱" min-width="200" />
      <el-table-column prop="fileSizeFormatted" label="檔案大小" width="100" align="right" />
      <el-table-column prop="fileExt" label="附檔名" width="80" />
      <el-table-column prop="downloadTime" label="下載時間" width="160" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '正常' : '已刪除' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleDownload(row)">下載</el-button>
          <el-button type="info" size="small" @click="handleEdit(row)">修改</el-button>
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

#### 4.2.3 修改對話框 (`FileDownloadDialog.vue`)
```vue
<template>
  <el-dialog
    title="修改檔案下傳記錄"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="下傳記錄ID">
        <el-input v-model="form.downloadId" disabled />
      </el-form-item>
      <el-form-item label="檔案名稱">
        <el-input v-model="form.fileName" disabled />
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

### 4.3 API 呼叫 (`xcom330.api.ts`)
```typescript
import request from '@/utils/request';

export interface FileDownloadDto {
  tKey: number;
  downloadId: string;
  uploadId?: string;
  userId: string;
  userName?: string;
  progId?: string;
  progName?: string;
  fileName: string;
  fileSize: number;
  fileSizeFormatted?: string;
  fileExt?: string;
  filePath: string;
  downloadTime: string;
  status: string;
  notes?: string;
  createdBy?: string;
  createdAt: string;
  updatedBy?: string;
  updatedAt: string;
}

export interface FileDownloadQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    userId?: string;
    uploadId?: string;
    progId?: string;
    fileExt?: string;
    downloadTimeFrom?: string;
    downloadTimeTo?: string;
    fileSizeFrom?: number;
    fileSizeTo?: number;
    status?: string;
  };
}

export interface UpdateFileDownloadDto {
  notes?: string;
}

// API 函數
export const getFileDownloadList = (query: FileDownloadQueryDto) => {
  return request.get<ApiResponse<PagedResult<FileDownloadDto>>>('/api/v1/xcom330/file-downloads', { params: query });
};

export const getFileDownloadById = (downloadId: string) => {
  return request.get<ApiResponse<FileDownloadDto>>(`/api/v1/xcom330/file-downloads/${downloadId}`);
};

export const downloadFile = (downloadId: string) => {
  return request.get(`/api/v1/xcom330/file-downloads/${downloadId}/download`, {
    responseType: 'blob'
  });
};

export const updateFileDownload = (downloadId: string, data: UpdateFileDownloadDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom330/file-downloads/${downloadId}`, data);
};

export const deleteFileDownload = (downloadId: string) => {
  return request.delete<ApiResponse>(`/api/v1/xcom330/file-downloads/${downloadId}`);
};

export const batchDeleteFileDownloads = (downloadIds: string[]) => {
  return request.delete<ApiResponse>('/api/v1/xcom330/file-downloads/batch', { data: { downloadIds } });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立唯一約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3.5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 檔案下載處理邏輯
- [ ] 下載記錄自動建立邏輯
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 檔案下載功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 檔案下載流程測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9.5天

---

## 六、注意事項

### 6.1 安全性
- 檔案下載必須檢查權限
- 檔案路徑必須驗證，防止路徑遍歷攻擊
- 敏感檔案必須加密儲存
- 必須記錄所有下載操作

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 檔案下載必須使用串流處理（大檔案）

### 6.3 資料驗證
- 下傳記錄ID必須唯一
- 必填欄位必須驗證

### 6.4 業務邏輯
- 下載檔案時，必須自動建立下載記錄
- 刪除下載記錄時，不刪除實體檔案（僅標記為已刪除）
- 必須記錄檔案下載操作日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢檔案下傳記錄列表成功
- [ ] 查詢單筆檔案下傳記錄成功
- [ ] 下載檔案成功（自動建立記錄）
- [ ] 下載檔案失敗 (檔案不存在)
- [ ] 修改檔案下傳記錄成功
- [ ] 刪除檔案下傳記錄成功
- [ ] 批次刪除檔案下傳記錄成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 檔案下載流程測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 大檔案下載測試
- [ ] 並發下載測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM330_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM330_FB.ASP` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM330_FU.ASP` - 修改畫面

### 8.2 相關功能
- XCOM320 - 檔案上傳作業
- XCOM360 - 上下傳記錄報表

### 8.3 資料庫 Schema
- 舊系統資料表：`FILE_DOWNLOAD`
- 主要欄位：USER_ID, UPLOAD_ID, FILE_NAME, FILE_SIZE, FILE_EXT, UPD_TIME

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


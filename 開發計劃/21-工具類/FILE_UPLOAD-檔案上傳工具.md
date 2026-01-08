# FILE_UPLOAD - 檔案上傳工具 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: FILE_UPLOAD
- **功能名稱**: 檔案上傳工具
- **功能描述**: 提供檔案上傳功能，支援單檔/多檔上傳、檔案類型驗證、檔案大小限制、上傳進度顯示、檔案預覽等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/UTIL/FILE_UPLOAD.aspx` (ASP.NET版本)
  - `IMS3/HANSHIN/IMS3/UTIL/FILE_UPLOAD.aspx.cs` (業務邏輯)
  - `WEB/IMS_CORE/UTIL/FILE_UPLOAD.aspx` (WEB版本)

### 1.2 業務需求
- 支援單檔上傳
- 支援多檔上傳
- 支援檔案類型驗證（副檔名、MIME類型）
- 支援檔案大小限制
- 支援上傳進度顯示
- 支援檔案預覽（圖片、PDF等）
- 支援檔案刪除
- 支援上傳路徑設定
- 支援檔案重新命名
- 支援上傳記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `FileUploads` (檔案上傳記錄)

```sql
CREATE TABLE [dbo].[FileUploads] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
    [FileName] NVARCHAR(255) NOT NULL, -- 檔案名稱 (FILE_NAME)
    [OriginalFileName] NVARCHAR(255) NOT NULL, -- 原始檔案名稱 (ORIGINAL_FILE_NAME)
    [FilePath] NVARCHAR(500) NOT NULL, -- 檔案路徑 (FILE_PATH)
    [FileSize] BIGINT NOT NULL, -- 檔案大小（位元組） (FILE_SIZE)
    [FileType] NVARCHAR(50) NULL, -- 檔案類型/MIME類型 (FILE_TYPE)
    [FileExtension] NVARCHAR(10) NULL, -- 副檔名 (FILE_EXTENSION)
    [UploadPath] NVARCHAR(200) NULL, -- 上傳路徑 (UPLOAD_PATH)
    [UploadedBy] NVARCHAR(50) NULL, -- 上傳者 (UPLOADED_BY)
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 上傳時間 (UPLOADED_AT)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:正常, 0:已刪除)
    [RelatedTable] NVARCHAR(50) NULL, -- 關聯資料表 (RELATED_TABLE)
    [RelatedId] NVARCHAR(50) NULL, -- 關聯ID (RELATED_ID)
    [Description] NVARCHAR(500) NULL, -- 描述 (DESCRIPTION)
    CONSTRAINT [PK_FileUploads] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_FileUploads_FileName] ON [dbo].[FileUploads] ([FileName]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_UploadedBy] ON [dbo].[FileUploads] ([UploadedBy]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_UploadedAt] ON [dbo].[FileUploads] ([UploadedAt]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_Status] ON [dbo].[FileUploads] ([Status]);
CREATE NONCLUSTERED INDEX [IX_FileUploads_Related] ON [dbo].[FileUploads] ([RelatedTable], [RelatedId]);
```

### 2.2 相關資料表
無

### 2.3 資料字典

#### FileUploads 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| FileName | NVARCHAR | 255 | NO | - | 檔案名稱 | 儲存後的檔案名稱 |
| OriginalFileName | NVARCHAR | 255 | NO | - | 原始檔案名稱 | 使用者上傳的原始檔案名稱 |
| FilePath | NVARCHAR | 500 | NO | - | 檔案路徑 | 完整檔案路徑 |
| FileSize | BIGINT | - | NO | - | 檔案大小 | 單位：位元組 |
| FileType | NVARCHAR | 50 | YES | - | 檔案類型 | MIME類型 |
| FileExtension | NVARCHAR | 10 | YES | - | 副檔名 | 如 .jpg, .pdf |
| UploadPath | NVARCHAR | 200 | YES | - | 上傳路徑 | 相對路徑 |
| UploadedBy | NVARCHAR | 50 | YES | - | 上傳者 | 使用者ID |
| UploadedAt | DATETIME2 | - | NO | GETDATE() | 上傳時間 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:正常, 0:已刪除 |
| RelatedTable | NVARCHAR | 50 | YES | - | 關聯資料表 | - |
| RelatedId | NVARCHAR | 50 | YES | - | 關聯ID | - |
| Description | NVARCHAR | 500 | YES | - | 描述 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 上傳檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/files/upload`
- **說明**: 上傳單個或多個檔案
- **請求格式**: `multipart/form-data`
  - `files`: 檔案（可多個）
  - `uploadPath`: 上傳路徑（可選）
  - `relatedTable`: 關聯資料表（可選）
  - `relatedId`: 關聯ID（可選）
  - `description`: 描述（可選）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "上傳成功",
    "data": [
      {
        "id": 1,
        "fileName": "20240101_123456_abc.jpg",
        "originalFileName": "photo.jpg",
        "filePath": "/uploads/2024/01/20240101_123456_abc.jpg",
        "fileSize": 1024000,
        "fileType": "image/jpeg",
        "fileExtension": ".jpg",
        "uploadPath": "uploads/2024/01",
        "uploadedBy": "U001",
        "uploadedAt": "2024-01-01T12:00:00Z"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢上傳檔案列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/files`
- **說明**: 查詢上傳檔案列表，支援篩選、排序、分頁
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "UploadedAt",
    "sortOrder": "DESC",
    "filters": {
      "fileName": "",
      "uploadedBy": "",
      "relatedTable": "",
      "relatedId": "",
      "status": "1"
    }
  }
  ```
- **回應格式**: 標準分頁回應格式

#### 3.1.3 查詢單筆檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/files/{id}`
- **說明**: 根據檔案ID查詢單筆檔案資料
- **路徑參數**:
  - `id`: 檔案ID
- **回應格式**: 同查詢上傳檔案列表單筆資料

#### 3.1.4 下載檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/files/{id}/download`
- **說明**: 下載檔案
- **路徑參數**:
  - `id`: 檔案ID
- **回應**: 檔案內容（Content-Type根據檔案類型設定）

#### 3.1.5 預覽檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/files/{id}/preview`
- **說明**: 預覽檔案（圖片、PDF等）
- **路徑參數**:
  - `id`: 檔案ID
- **回應**: 檔案內容（Content-Type根據檔案類型設定）

#### 3.1.6 刪除檔案
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/files/{id}`
- **說明**: 刪除檔案（標記為已刪除，實際檔案可選刪除）
- **路徑參數**:
  - `id`: 檔案ID
- **回應格式**: 標準回應格式

### 3.2 後端實作類別

#### 3.2.1 Entity: `FileUpload.cs`
```csharp
namespace RSL.IMS3.Domain.Entities
{
    public class FileUpload
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public string UploadPath { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Status { get; set; }
        public string RelatedTable { get; set; }
        public string RelatedId { get; set; }
        public string Description { get; set; }
    }
}
```

#### 3.2.2 Repository: `IFileUploadRepository.cs`
```csharp
namespace RSL.IMS3.Domain.Repositories
{
    public interface IFileUploadRepository
    {
        Task<FileUpload> GetByIdAsync(long id);
        Task<IEnumerable<FileUpload>> GetFilesAsync(FileUploadQuery query);
        Task<FileUpload> CreateAsync(FileUpload fileUpload);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
    }
}
```

#### 3.2.3 Service: `FileUploadService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IFileUploadService
    {
        Task<IEnumerable<FileUploadDto>> UploadFilesAsync(IFormFileCollection files, FileUploadRequest request);
        Task<PagedResult<FileUploadDto>> GetFilesAsync(FileUploadQueryDto query);
        Task<FileUploadDto> GetFileByIdAsync(long id);
        Task<FileResult> DownloadFileAsync(long id);
        Task<FileResult> PreviewFileAsync(long id);
        Task<bool> DeleteFileAsync(long id);
    }
    
    public class FileUploadService : IFileUploadService
    {
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly IFileStorageService _fileStorageService;
        // 實作檔案上傳相關邏輯
    }
}
```

#### 3.2.4 Service: `IFileStorageService.cs` (檔案儲存服務)
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string uploadPath);
        Task<bool> DeleteFileAsync(string filePath);
        Task<byte[]> ReadFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
    }
    
    public class LocalFileStorageService : IFileStorageService
    {
        // 實作本地檔案儲存邏輯
    }
}
```

#### 3.2.5 Controller: `FileUploadController.cs`
```csharp
namespace RSL.IMS3.Backend.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        // 實作API端點
    }
}
```

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 檔案上傳組件 (`FileUpload.vue`)
```vue
<template>
  <div class="file-upload">
    <el-upload
      ref="uploadRef"
      :action="uploadUrl"
      :headers="uploadHeaders"
      :data="uploadData"
      :file-list="fileList"
      :multiple="multiple"
      :limit="limit"
      :accept="accept"
      :on-success="handleSuccess"
      :on-error="handleError"
      :on-progress="handleProgress"
      :on-remove="handleRemove"
      :on-exceed="handleExceed"
      :before-upload="beforeUpload"
      :disabled="disabled"
      drag
    >
      <el-icon class="el-icon--upload"><upload-filled /></el-icon>
      <div class="el-upload__text">
        將檔案拖到此處，或<em>點擊上傳</em>
      </div>
      <template #tip>
        <div class="el-upload__tip">
          {{ tipText }}
        </div>
      </template>
    </el-upload>
    
    <el-progress
      v-if="uploading"
      :percentage="uploadProgress"
      :status="uploadStatus"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { UploadFilled } from '@element-plus/icons-vue';
import { fileUploadApi } from '@/api/files';
import { useAuthStore } from '@/stores/auth';

interface Props {
  modelValue?: any[];
  uploadPath?: string;
  relatedTable?: string;
  relatedId?: string;
  multiple?: boolean;
  limit?: number;
  accept?: string;
  maxSize?: number; // MB
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  multiple: true,
  limit: 10,
  maxSize: 10,
  disabled: false
});

const emit = defineEmits<{
  'update:modelValue': [value: any[]];
  'success': [file: any];
  'error': [error: any];
  'remove': [file: any];
}>();

const authStore = useAuthStore();
const uploadRef = ref();
const fileList = ref([]);
const uploading = ref(false);
const uploadProgress = ref(0);
const uploadStatus = ref<'success' | 'exception' | 'warning' | ''>('');

const uploadUrl = computed(() => {
  return `${import.meta.env.VITE_API_BASE_URL}/api/v1/files/upload`;
});

const uploadHeaders = computed(() => {
  return {
    'Authorization': `Bearer ${authStore.token}`
  };
});

const uploadData = computed(() => {
  return {
    uploadPath: props.uploadPath,
    relatedTable: props.relatedTable,
    relatedId: props.relatedId
  };
});

const tipText = computed(() => {
  let text = `只能上傳${props.accept || '任意'}檔案，`;
  text += `單個檔案不超過${props.maxSize}MB`;
  if (props.limit > 1) {
    text += `，最多上傳${props.limit}個檔案`;
  }
  return text;
});

const beforeUpload = (file: File) => {
  // 檢查檔案大小
  const isLtMaxSize = file.size / 1024 / 1024 < props.maxSize;
  if (!isLtMaxSize) {
    ElMessage.error(`檔案大小不能超過 ${props.maxSize}MB!`);
    return false;
  }
  
  // 檢查檔案類型
  if (props.accept) {
    const acceptTypes = props.accept.split(',').map(t => t.trim());
    const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase();
    const fileType = file.type;
    
    const isValidType = acceptTypes.some(type => {
      if (type.startsWith('.')) {
        return fileExtension === type;
      } else {
        return fileType.startsWith(type);
      }
    });
    
    if (!isValidType) {
      ElMessage.error(`只能上傳 ${props.accept} 類型的檔案!`);
      return false;
    }
  }
  
  uploading.value = true;
  uploadProgress.value = 0;
  return true;
};

const handleProgress = (event: any) => {
  uploadProgress.value = Math.round(event.percent);
};

const handleSuccess = (response: any, file: any) => {
  uploading.value = false;
  uploadProgress.value = 100;
  uploadStatus.value = 'success';
  
  if (response.success && response.data) {
    const uploadedFiles = Array.isArray(response.data) ? response.data : [response.data];
    fileList.value = [...fileList.value, ...uploadedFiles.map((f: any) => ({
      id: f.id,
      name: f.originalFileName,
      url: `/api/v1/files/${f.id}/preview`,
      status: 'success',
      response: f
    }))];
    
    emit('update:modelValue', fileList.value.map((f: any) => f.response));
    emit('success', uploadedFiles[0]);
    
    ElMessage.success('上傳成功');
  } else {
    uploadStatus.value = 'exception';
    ElMessage.error(response.message || '上傳失敗');
  }
};

const handleError = (error: any, file: any) => {
  uploading.value = false;
  uploadProgress.value = 0;
  uploadStatus.value = 'exception';
  ElMessage.error('上傳失敗');
  emit('error', error);
};

const handleRemove = (file: any) => {
  const index = fileList.value.findIndex((f: any) => f.id === file.id);
  if (index > -1) {
    fileList.value.splice(index, 1);
    emit('update:modelValue', fileList.value.map((f: any) => f.response));
    emit('remove', file);
  }
};

const handleExceed = () => {
  ElMessage.warning(`最多只能上傳 ${props.limit} 個檔案`);
};
</script>

<style scoped>
.file-upload {
  width: 100%;
}

.el-upload__tip {
  font-size: 12px;
  color: #606266;
  margin-top: 7px;
}
</style>
```

#### 4.1.2 檔案列表組件 (`FileList.vue`)
```vue
<template>
  <div class="file-list">
    <el-table :data="files" border>
      <el-table-column prop="originalFileName" label="檔案名稱" />
      <el-table-column prop="fileSize" label="檔案大小" width="120">
        <template #default="{ row }">
          {{ formatFileSize(row.fileSize) }}
        </template>
      </el-table-column>
      <el-table-column prop="fileType" label="檔案類型" width="120" />
      <el-table-column prop="uploadedAt" label="上傳時間" width="180">
        <template #default="{ row }">
          {{ formatDateTime(row.uploadedAt) }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handlePreview(row)">預覽</el-button>
          <el-button type="success" size="small" @click="handleDownload(row)">下載</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { fileUploadApi } from '@/api/files';

interface Props {
  files: any[];
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'delete': [file: any];
}>();

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
};

const formatDateTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleString('zh-TW');
};

const handlePreview = (file: any) => {
  window.open(`/api/v1/files/${file.id}/preview`, '_blank');
};

const handleDownload = async (file: any) => {
  try {
    const response = await fileUploadApi.downloadFile(file.id);
    const blob = new Blob([response.data]);
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = file.originalFileName;
    link.click();
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error('下載失敗:', error);
    ElMessage.error('下載失敗');
  }
};

const handleDelete = async (file: any) => {
  try {
    await ElMessageBox.confirm('確定要刪除此檔案嗎？', '提示', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    });
    
    await fileUploadApi.deleteFile(file.id);
    ElMessage.success('刪除成功');
    emit('delete', file);
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除失敗:', error);
      ElMessage.error('刪除失敗');
    }
  }
};
</script>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立 FileUploads 資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（含檔案儲存服務）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 檔案驗證邏輯
- [ ] 檔案儲存邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2.5天)
- [ ] API 呼叫函數
- [ ] FileUpload 組件開發
- [ ] FileList 組件開發
- [ ] 上傳進度顯示
- [ ] 檔案預覽功能
- [ ] 檔案下載功能
- [ ] 檔案刪除功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 檔案上傳測試
- [ ] 檔案下載測試
- [ ] 檔案預覽測試
- [ ] 檔案刪除測試
- [ ] 檔案大小限制測試
- [ ] 檔案類型驗證測試
- [ ] 多檔上傳測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 組件使用文件
- [ ] 部署文件
- [ ] 檔案儲存路徑設定文件

**總計**: 8天

---

## 六、注意事項

### 6.1 檔案儲存
- 支援本地檔案系統儲存
- 可擴展支援雲端儲存（Azure Blob Storage, AWS S3等）
- 檔案路徑建議使用日期分層（如：uploads/2024/01/）

### 6.2 檔案安全
- 檔案類型驗證（副檔名、MIME類型）
- 檔案大小限制
- 檔案名稱處理（防止路徑遍歷攻擊）
- 檔案權限控制

### 6.3 檔案命名
- 建議使用時間戳記 + 隨機字串命名
- 保留原始檔案名稱在資料庫中

### 6.4 檔案預覽
- 支援圖片預覽
- 支援PDF預覽
- 其他類型檔案提供下載

### 6.5 效能優化
- 大檔案上傳使用分塊上傳
- 檔案上傳使用非同步處理
- 檔案下載使用串流

---

## 七、測試案例

### 7.1 單元測試
- [ ] 檔案上傳成功
- [ ] 檔案類型驗證成功
- [ ] 檔案大小驗證成功
- [ ] 檔案刪除成功
- [ ] 檔案查詢成功

### 7.2 整合測試
- [ ] 單檔上傳正常運作
- [ ] 多檔上傳正常運作
- [ ] 檔案預覽正常運作
- [ ] 檔案下載正常運作
- [ ] 檔案刪除正常運作
- [ ] 檔案大小限制正確
- [ ] 檔案類型驗證正確

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/UTIL/FILE_UPLOAD.aspx`
- `IMS3/HANSHIN/IMS3/UTIL/FILE_UPLOAD.aspx.cs`
- `WEB/IMS_CORE/UTIL/FILE_UPLOAD.aspx`

### 8.2 相關功能
- Element Plus Upload 組件文件
- .NET Core 檔案上傳文件

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


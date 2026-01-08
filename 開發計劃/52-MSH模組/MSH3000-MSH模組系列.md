# MSH3000 - MSH模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MSH3000 系列
- **功能名稱**: MSH模組系列
- **功能描述**: 提供MSH3000系列功能，包含系統M系列功能模組
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/MSH3000/` (MSH3000模組目錄)
  - 目前目錄主要包含IMG子目錄，用於存放圖像資源

### 1.2 業務需求
- 提供MSH3000系列功能的資料維護
- 支援資料的新增、修改、刪除、查詢功能
- 支援圖像資源管理功能
- 支援資料報表功能

### 1.3 子模組清單
- **MSH3000**: MSH模組資料維護（基本CRUD功能）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Msh3000Data`

```sql
CREATE TABLE [dbo].[Msh3000Data] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
    [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
    [DataValue] NVARCHAR(200) NULL, -- 資料值
    [DataType] NVARCHAR(20) NULL, -- 資料類型
    [ImagePath] NVARCHAR(500) NULL, -- 圖像路徑
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [SortOrder] INT NULL DEFAULT 0, -- 排序順序
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_Msh3000Data_DataId] UNIQUE ([DataId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Msh3000Data_DataType] ON [dbo].[Msh3000Data] ([DataType]);
CREATE NONCLUSTERED INDEX [IX_Msh3000Data_Status] ON [dbo].[Msh3000Data] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Msh3000Data_SortOrder] ON [dbo].[Msh3000Data] ([SortOrder]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| DataId | NVARCHAR | 50 | NO | - | 資料代碼 | 唯一 |
| DataName | NVARCHAR | 100 | NO | - | 資料名稱 | - |
| DataValue | NVARCHAR | 200 | YES | - | 資料值 | - |
| DataType | NVARCHAR | 20 | YES | - | 資料類型 | - |
| ImagePath | NVARCHAR | 500 | YES | - | 圖像路徑 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SortOrder | INT | - | YES | 0 | 排序順序 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 查詢資料列表

```http
GET /api/v1/msh3000/data?dataType={dataType}&status={status}&keyword={keyword}&page={page}&pageSize={pageSize}
```

**請求參數**:
- `dataType`: 資料類型（選填）
- `status`: 狀態（選填，A:啟用, I:停用）
- `keyword`: 關鍵字搜尋（選填，搜尋資料代碼或名稱）
- `page`: 頁碼（預設1）
- `pageSize`: 每頁筆數（預設20）

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "dataId": "DATA001",
        "dataName": "資料名稱",
        "dataValue": "資料值",
        "dataType": "TYPE001",
        "imagePath": "/images/msh3000/image001.jpg",
        "status": "A",
        "sortOrder": 1,
        "memo": "備註",
        "createdBy": "USER001",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedBy": "USER001",
        "updatedAt": "2024-01-01T00:00:00Z"
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  }
}
```

### 3.2 新增資料

```http
POST /api/v1/msh3000/data
```

**請求體**:
```json
{
  "dataId": "DATA001",
  "dataName": "資料名稱",
  "dataValue": "資料值",
  "dataType": "TYPE001",
  "imagePath": "/images/msh3000/image001.jpg",
  "status": "A",
  "sortOrder": 1,
  "memo": "備註"
}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "新增成功",
  "data": {
    "tKey": 1,
    "dataId": "DATA001",
    "dataName": "資料名稱",
    "dataValue": "資料值",
    "dataType": "TYPE001",
    "imagePath": "/images/msh3000/image001.jpg",
    "status": "A",
    "sortOrder": 1,
    "memo": "備註",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

**錯誤回應**:
```json
{
  "success": false,
  "code": 400,
  "message": "資料代碼已存在",
  "data": null
}
```

### 3.3 修改資料

```http
PUT /api/v1/msh3000/data/{tKey}
```

**請求體**:
```json
{
  "dataName": "修改後的資料名稱",
  "dataValue": "修改後的資料值",
  "dataType": "TYPE002",
  "imagePath": "/images/msh3000/image002.jpg",
  "status": "I",
  "sortOrder": 2,
  "memo": "修改備註"
}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "修改成功",
  "data": {
    "tKey": 1,
    "dataId": "DATA001",
    "dataName": "修改後的資料名稱",
    "dataValue": "修改後的資料值",
    "dataType": "TYPE002",
    "imagePath": "/images/msh3000/image002.jpg",
    "status": "I",
    "sortOrder": 2,
    "memo": "修改備註",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### 3.4 刪除資料

```http
DELETE /api/v1/msh3000/data/{tKey}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "刪除成功",
  "data": null
}
```

**錯誤回應**:
```json
{
  "success": false,
  "code": 404,
  "message": "資料不存在",
  "data": null
}
```

### 3.5 查詢單筆資料

```http
GET /api/v1/msh3000/data/{tKey}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "tKey": 1,
    "dataId": "DATA001",
    "dataName": "資料名稱",
    "dataValue": "資料值",
    "dataType": "TYPE001",
    "imagePath": "/images/msh3000/image001.jpg",
    "status": "A",
    "sortOrder": 1,
    "memo": "備註",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### 3.6 上傳圖像

```http
POST /api/v1/msh3000/data/{tKey}/upload-image
Content-Type: multipart/form-data
```

**請求體**:
- `file`: 圖像檔案（multipart/form-data）

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "上傳成功",
  "data": {
    "imagePath": "/images/msh3000/image001.jpg",
    "fileName": "image001.jpg"
  }
}
```

---

## 四、前端 UI 設計

### 4.1 資料列表頁面

#### 4.1.1 路由配置
- **路由**: `/msh3000/data`
- **組件**: `Msh3000DataList.vue`
- **權限**: `MSH3000_DATA_VIEW`

#### 4.1.2 頁面結構
```
┌─────────────────────────────────────────┐
│  MSH模組資料維護                         │
├─────────────────────────────────────────┤
│  [資料類型] [狀態] [關鍵字] [查詢]      │
├─────────────────────────────────────────┤
│  ┌───────────────────────────────────┐  │
│  │ 資料代碼 │ 資料名稱 │ 狀態 │ 操作 │  │
│  ├───────────────────────────────────┤  │
│  │ DATA001 │ 資料名稱1 │ 啟用 │ ... │  │
│  │ DATA002 │ 資料名稱2 │ 停用 │ ... │  │
│  └───────────────────────────────────┘  │
│  [新增]                    [1][2][3]    │
└─────────────────────────────────────────┘
```

#### 4.1.3 功能說明
- **篩選區**:
  - 資料類型下拉選單（選填）
  - 狀態下拉選單（全部/啟用/停用）
  - 關鍵字輸入框（搜尋資料代碼或名稱）
  - 查詢按鈕
- **資料表格**:
  - 顯示欄位：資料代碼、資料名稱、資料值、資料類型、圖像預覽、狀態、排序順序、備註
  - 操作欄位：修改、刪除、上傳圖像按鈕
  - 支援排序（點擊欄位標題）
  - 支援分頁
- **操作按鈕**:
  - 新增按鈕：開啟新增對話框
  - 修改按鈕：開啟修改對話框
  - 刪除按鈕：確認後刪除
  - 上傳圖像按鈕：上傳圖像檔案

### 4.2 資料維護對話框

#### 4.2.1 新增對話框
- **組件**: `Msh3000DataDialog.vue`
- **模式**: `create`
- **表單欄位**:
  - 資料代碼（必填，文字輸入框）
  - 資料名稱（必填，文字輸入框）
  - 資料值（選填，文字輸入框）
  - 資料類型（選填，下拉選單）
  - 圖像路徑（選填，檔案上傳或文字輸入框）
  - 狀態（必填，下拉選單：啟用/停用，預設：啟用）
  - 排序順序（選填，數字輸入框，預設：0）
  - 備註（選填，文字區域）

#### 4.2.2 修改對話框
- **組件**: `Msh3000DataDialog.vue`
- **模式**: `edit`
- **表單欄位**:
  - 資料代碼（唯讀，顯示）
  - 資料名稱（必填，文字輸入框）
  - 資料值（選填，文字輸入框）
  - 資料類型（選填，下拉選單）
  - 圖像路徑（選填，檔案上傳或文字輸入框）
  - 狀態（必填，下拉選單：啟用/停用）
  - 排序順序（選填，數字輸入框）
  - 備註（選填，文字區域）

### 4.3 圖像上傳組件

#### 4.3.1 圖像上傳組件
- **組件**: `Msh3000ImageUpload.vue`
- **功能**: 上傳圖像檔案
- **支援格式**: JPG, PNG, GIF, BMP
- **檔案大小限制**: 最大5MB
- **預覽功能**: 上傳後可預覽圖像

---

## 五、後端實作類別

### 5.1 Controller: `Msh3000Controller.cs`

```csharp
[ApiController]
[Route("api/v1/msh3000")]
[Authorize]
public class Msh3000Controller : ControllerBase
{
    private readonly IMsh3000Service _msh3000Service;
    private readonly ILogger<Msh3000Controller> _logger;

    public Msh3000Controller(IMsh3000Service msh3000Service, ILogger<Msh3000Controller> logger)
    {
        _msh3000Service = msh3000Service;
        _logger = logger;
    }

    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<PagedResult<Msh3000DataDto>>>> GetDataList(
        [FromQuery] string? dataType,
        [FromQuery] string? status,
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _msh3000Service.GetDataListAsync(dataType, status, keyword, page, pageSize);
        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<Msh3000DataDto>>> GetData(long tKey)
    {
        var result = await _msh3000Service.GetDataAsync(tKey);
        if (result == null)
            return NotFound(ApiResponse.Error("資料不存在"));
        return Ok(ApiResponse.Success(result));
    }

    [HttpPost("data")]
    public async Task<ActionResult<ApiResponse<Msh3000DataDto>>> CreateData([FromBody] CreateMsh3000DataRequest request)
    {
        var result = await _msh3000Service.CreateDataAsync(request);
        return Ok(ApiResponse.Success(result));
    }

    [HttpPut("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<Msh3000DataDto>>> UpdateData(long tKey, [FromBody] UpdateMsh3000DataRequest request)
    {
        var result = await _msh3000Service.UpdateDataAsync(tKey, request);
        if (result == null)
            return NotFound(ApiResponse.Error("資料不存在"));
        return Ok(ApiResponse.Success(result));
    }

    [HttpDelete("data/{tKey}")]
    public async Task<ActionResult<ApiResponse>> DeleteData(long tKey)
    {
        await _msh3000Service.DeleteDataAsync(tKey);
        return Ok(ApiResponse.Success());
    }

    [HttpPost("data/{tKey}/upload-image")]
    public async Task<ActionResult<ApiResponse<ImageUploadResult>>> UploadImage(long tKey, IFormFile file)
    {
        var result = await _msh3000Service.UploadImageAsync(tKey, file);
        return Ok(ApiResponse.Success(result));
    }
}
```

### 5.2 Service Interface: `IMsh3000Service.cs`

```csharp
public interface IMsh3000Service
{
    Task<PagedResult<Msh3000DataDto>> GetDataListAsync(string? dataType, string? status, string? keyword, int page, int pageSize);
    Task<Msh3000DataDto?> GetDataAsync(long tKey);
    Task<Msh3000DataDto> CreateDataAsync(CreateMsh3000DataRequest request);
    Task<Msh3000DataDto?> UpdateDataAsync(long tKey, UpdateMsh3000DataRequest request);
    Task DeleteDataAsync(long tKey);
    Task<ImageUploadResult> UploadImageAsync(long tKey, IFormFile file);
}
```

### 5.3 Repository Interface: `IMsh3000Repository.cs`

```csharp
public interface IMsh3000Repository
{
    Task<PagedResult<Msh3000Data>> GetDataListAsync(string? dataType, string? status, string? keyword, int page, int pageSize);
    Task<Msh3000Data?> GetDataAsync(long tKey);
    Task<Msh3000Data> CreateDataAsync(Msh3000Data data);
    Task<Msh3000Data?> UpdateDataAsync(long tKey, Msh3000Data data);
    Task DeleteDataAsync(long tKey);
    Task<bool> ExistsDataIdAsync(string dataId);
}
```

---

## 六、前端實作

### 6.1 API 呼叫: `msh3000.api.ts`

```typescript
import axios from '@/utils/axios';
import type { ApiResponse, PagedResult } from '@/types/api';
import type { Msh3000Data, CreateMsh3000DataRequest, UpdateMsh3000DataRequest, ImageUploadResult } from '@/types/msh3000';

export const msh3000Api = {
  // 查詢資料列表
  getDataList: (params: {
    dataType?: string;
    status?: string;
    keyword?: string;
    page?: number;
    pageSize?: number;
  }): Promise<ApiResponse<PagedResult<Msh3000Data>>> => {
    return axios.get('/api/v1/msh3000/data', { params });
  },

  // 查詢單筆資料
  getData: (tKey: number): Promise<ApiResponse<Msh3000Data>> => {
    return axios.get(`/api/v1/msh3000/data/${tKey}`);
  },

  // 新增資料
  createData: (data: CreateMsh3000DataRequest): Promise<ApiResponse<Msh3000Data>> => {
    return axios.post('/api/v1/msh3000/data', data);
  },

  // 修改資料
  updateData: (tKey: number, data: UpdateMsh3000DataRequest): Promise<ApiResponse<Msh3000Data>> => {
    return axios.put(`/api/v1/msh3000/data/${tKey}`, data);
  },

  // 刪除資料
  deleteData: (tKey: number): Promise<ApiResponse<void>> => {
    return axios.delete(`/api/v1/msh3000/data/${tKey}`);
  },

  // 上傳圖像
  uploadImage: (tKey: number, file: File): Promise<ApiResponse<ImageUploadResult>> => {
    const formData = new FormData();
    formData.append('file', file);
    return axios.post(`/api/v1/msh3000/data/${tKey}/upload-image`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  }
};
```

### 6.2 類型定義: `msh3000.ts`

```typescript
export interface Msh3000Data {
  tKey: number;
  dataId: string;
  dataName: string;
  dataValue?: string;
  dataType?: string;
  imagePath?: string;
  status: 'A' | 'I';
  sortOrder: number;
  memo?: string;
  createdBy?: string;
  createdAt: string;
  updatedBy?: string;
  updatedAt: string;
}

export interface CreateMsh3000DataRequest {
  dataId: string;
  dataName: string;
  dataValue?: string;
  dataType?: string;
  imagePath?: string;
  status?: 'A' | 'I';
  sortOrder?: number;
  memo?: string;
}

export interface UpdateMsh3000DataRequest {
  dataName: string;
  dataValue?: string;
  dataType?: string;
  imagePath?: string;
  status?: 'A' | 'I';
  sortOrder?: number;
  memo?: string;
}

export interface ImageUploadResult {
  imagePath: string;
  fileName: string;
}
```

### 6.3 列表頁面: `Msh3000DataList.vue`

```vue
<template>
  <div class="msh3000-data-list">
    <el-card>
      <template #header>
        <span>MSH模組資料維護</span>
      </template>

      <!-- 篩選區 -->
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="資料類型">
          <el-select v-model="searchForm.dataType" placeholder="請選擇" clearable>
            <el-option label="類型1" value="TYPE001" />
            <el-option label="類型2" value="TYPE002" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="searchForm.status" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="關鍵字">
          <el-input v-model="searchForm.keyword" placeholder="請輸入關鍵字" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 操作區 -->
      <div class="action-bar">
        <el-button type="primary" @click="handleAdd">新增</el-button>
      </div>

      <!-- 資料表格 -->
      <el-table :data="tableData" v-loading="loading" border>
        <el-table-column prop="dataId" label="資料代碼" width="120" />
        <el-table-column prop="dataName" label="資料名稱" width="150" />
        <el-table-column prop="dataValue" label="資料值" width="150" />
        <el-table-column prop="dataType" label="資料類型" width="120" />
        <el-table-column prop="imagePath" label="圖像" width="100">
          <template #default="{ row }">
            <el-image v-if="row.imagePath" :src="row.imagePath" style="width: 50px; height: 50px" fit="cover" />
          </template>
        </el-table-column>
        <el-table-column prop="status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 'A' ? 'success' : 'info'">
              {{ row.status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="sortOrder" label="排序" width="80" />
        <el-table-column prop="memo" label="備註" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
            <el-button type="info" size="small" @click="handleUploadImage(row)">上傳圖像</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.page"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        class="pagination"
      />
    </el-card>

    <!-- 資料維護對話框 -->
    <Msh3000DataDialog
      v-model="dialogVisible"
      :mode="dialogMode"
      :data="currentData"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { msh3000Api } from '@/api/msh3000.api';
import type { Msh3000Data } from '@/types/msh3000';
import Msh3000DataDialog from './components/Msh3000DataDialog.vue';

const loading = ref(false);
const tableData = ref<Msh3000Data[]>([]);
const searchForm = ref({
  dataType: '',
  status: '',
  keyword: ''
});
const pagination = ref({
  page: 1,
  pageSize: 20,
  total: 0
});

const dialogVisible = ref(false);
const dialogMode = ref<'create' | 'edit'>('create');
const currentData = ref<Msh3000Data | null>(null);

const loadData = async () => {
  loading.value = true;
  try {
    const response = await msh3000Api.getDataList({
      ...searchForm.value,
      page: pagination.value.page,
      pageSize: pagination.value.pageSize
    });
    if (response.success) {
      tableData.value = response.data.items;
      pagination.value.total = response.data.total;
    }
  } catch (error) {
    ElMessage.error('查詢失敗');
  } finally {
    loading.value = false;
  }
};

const handleSearch = () => {
  pagination.value.page = 1;
  loadData();
};

const handleReset = () => {
  searchForm.value = {
    dataType: '',
    status: '',
    keyword: ''
  };
  handleSearch();
};

const handleAdd = () => {
  dialogMode.value = 'create';
  currentData.value = null;
  dialogVisible.value = true;
};

const handleEdit = (row: Msh3000Data) => {
  dialogMode.value = 'edit';
  currentData.value = { ...row };
  dialogVisible.value = true;
};

const handleDelete = async (row: Msh3000Data) => {
  try {
    await ElMessageBox.confirm('確定要刪除這筆資料嗎？', '確認刪除', {
      type: 'warning'
    });
    await msh3000Api.deleteData(row.tKey);
    ElMessage.success('刪除成功');
    loadData();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗');
    }
  }
};

const handleUploadImage = (row: Msh3000Data) => {
  // 實作圖像上傳功能
};

const handleDialogSuccess = () => {
  dialogVisible.value = false;
  loadData();
};

const handleSizeChange = () => {
  loadData();
};

const handlePageChange = () => {
  loadData();
};

onMounted(() => {
  loadData();
});
</script>
```

---

## 七、開發時程

### 階段一：資料庫設計（0.5週）
- 資料表設計
- 索引設計
- 資料字典建立

### 階段二：後端API開發（1週）
- 查詢API開發
- 新增API開發
- 修改API開發
- 刪除API開發
- 圖像上傳API開發

### 階段三：前端UI開發（1週）
- 資料列表頁面開發
- 資料維護對話框開發
- 圖像上傳組件開發
- 表單驗證實作

### 階段四：測試與優化（0.5週）
- 單元測試
- 整合測試
- 效能優化
- 文件整理

**總計**: 約 3 週

---

## 八、注意事項

### 8.1 資料驗證
- 資料代碼必須唯一
- 資料名稱不能為空
- 資料代碼格式驗證（根據業務需求）

### 8.2 權限控制
- 資料維護需特定權限
- 圖像上傳需特定權限
- 刪除功能需確認對話框

### 8.3 效能考量
- 大量資料查詢需使用分頁
- 關鍵字搜尋需使用索引
- 資料表格需使用虛擬滾動（如資料量大）

### 8.4 圖像管理
- 圖像檔案需限制大小（最大5MB）
- 圖像格式需驗證（JPG, PNG, GIF, BMP）
- 圖像路徑需安全處理
- 圖像檔案需定期清理（如需要）

### 8.5 資料備份
- 重要資料需定期備份
- 刪除操作需記錄日誌

---

## 九、測試案例

### 9.1 新增資料測試
- **測試案例1**: 正常新增
  - 輸入：完整的資料資訊
  - 預期：成功新增並顯示在列表中
- **測試案例2**: 資料代碼重複
  - 輸入：已存在的資料代碼
  - 預期：顯示錯誤訊息「資料代碼已存在」
- **測試案例3**: 必填欄位為空
  - 輸入：資料代碼或名稱為空
  - 預期：顯示驗證錯誤訊息

### 9.2 修改資料測試
- **測試案例1**: 正常修改
  - 輸入：修改資料名稱和狀態
  - 預期：成功修改並更新顯示
- **測試案例2**: 修改不存在的資料
  - 輸入：不存在的TKey
  - 預期：顯示錯誤訊息「資料不存在」

### 9.3 刪除資料測試
- **測試案例1**: 正常刪除
  - 輸入：選擇要刪除的資料
  - 預期：成功刪除並從列表中移除
- **測試案例2**: 刪除不存在的資料
  - 輸入：不存在的TKey
  - 預期：顯示錯誤訊息「資料不存在」

### 9.4 查詢資料測試
- **測試案例1**: 關鍵字搜尋
  - 輸入：關鍵字「DATA」
  - 預期：顯示所有包含「DATA」的資料
- **測試案例2**: 狀態篩選
  - 輸入：狀態「啟用」
  - 預期：只顯示狀態為「啟用」的資料
- **測試案例3**: 組合查詢
  - 輸入：資料類型「TYPE001」+ 狀態「啟用」
  - 預期：顯示符合兩個條件的資料

### 9.5 圖像上傳測試
- **測試案例1**: 正常上傳圖像
  - 輸入：有效的圖像檔案
  - 預期：成功上傳並更新圖像路徑
- **測試案例2**: 上傳過大檔案
  - 輸入：超過5MB的檔案
  - 預期：顯示錯誤訊息「檔案大小超過限制」
- **測試案例3**: 上傳無效格式
  - 輸入：非圖像格式檔案
  - 預期：顯示錯誤訊息「不支援的檔案格式」

---

## 十、參考資料

### 10.1 舊程式參考
- `WEB/IMS_CORE/ASP/MSH3000/` - MSH3000模組目錄
- `WEB/IMS_CORE/ASP/MSH3000/IMG/` - 圖像資源目錄

### 10.2 相關文件
- 系統架構分析.md - MSH3000 目錄分析
- 目錄掃描狀態統計.md - MSH3000 模組狀態

### 10.3 技術文件
- .NET Core 8.0 API 開發指南
- Vue 3 開發指南
- Dapper 使用手冊
- SQL Server 資料庫設計指南
- Element Plus 組件庫文件


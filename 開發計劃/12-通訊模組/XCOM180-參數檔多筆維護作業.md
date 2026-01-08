# XCOM180 - 參數檔多筆維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM180
- **功能名稱**: 參數檔多筆維護作業
- **功能描述**: 提供參數檔資料的新增、修改、刪除、查詢功能，包含參數項目(TAG)、參數內容(CONTENT)、參數內容1(CONTENT_2)、參數說明(NOTES)、參數標題(TITLE)等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM180_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM180_FU.ASP` (修改)

### 1.2 業務需求
- 管理系統參數檔資料
- 支援參數項目(TAG)維護（唯讀，不可修改）
- 支援參數內容(CONTENT)維護
- 支援參數內容1(CONTENT_2)維護
- 支援參數說明(NOTES)維護（唯讀）
- 支援參數標題(TITLE)維護（唯讀）
- 支援多筆資料批次維護
- 支援資料查詢與瀏覽
- 支援資料範圍查詢（顯示筆數範圍）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Parameters` (參數檔，對應舊系統 `PARAM`)

```sql
CREATE TABLE [dbo].[Parameters] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TAG] NVARCHAR(30) NOT NULL, -- 參數項目
    [CONTENT] NVARCHAR(512) NULL, -- 參數內容
    [CONTENT_2] NVARCHAR(128) NULL, -- 參數內容1
    [NOTES] NVARCHAR(128) NULL, -- 參數說明
    [TITLE] NVARCHAR(100) NULL, -- 參數標題
    [BTIME] DATETIME2 NULL, -- 建立時間
    [CTIME] DATETIME2 NULL, -- 變更時間
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED ([T_KEY] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Parameters_TAG] ON [dbo].[Parameters] ([TAG]);
CREATE NONCLUSTERED INDEX [IX_Parameters_CONTENT] ON [dbo].[Parameters] ([CONTENT]);
CREATE NONCLUSTERED INDEX [IX_Parameters_CONTENT_2] ON [dbo].[Parameters] ([CONTENT_2]);
CREATE NONCLUSTERED INDEX [IX_Parameters_TITLE] ON [dbo].[Parameters] ([TITLE]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| TAG | NVARCHAR | 30 | NO | - | 參數項目 | 必填，唯讀（不可修改） |
| CONTENT | NVARCHAR | 512 | YES | - | 參數內容 | 可修改 |
| CONTENT_2 | NVARCHAR | 128 | YES | - | 參數內容1 | 可修改 |
| NOTES | NVARCHAR | 128 | YES | - | 參數說明 | 唯讀 |
| TITLE | NVARCHAR | 100 | YES | - | 參數標題 | 唯讀 |
| BTIME | DATETIME2 | - | YES | - | 建立時間 | - |
| CTIME | DATETIME2 | - | YES | - | 變更時間 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢參數檔列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom180/parameters`
- **說明**: 查詢參數檔列表，支援分頁、排序、篩選、範圍查詢
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TAG",
    "sortOrder": "ASC",
    "filters": {
      "tag": "",
      "content": "",
      "content2": "",
      "notes": "",
      "title": ""
    },
    "recordCountFrom": 1,
    "recordCountTo": 100
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
          "tag": "PARAM001",
          "content": "參數內容",
          "content2": "參數內容1",
          "notes": "參數說明",
          "title": "參數標題",
          "btime": "2024-01-01T00:00:00",
          "ctime": "2024-01-01T00:00:00"
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

#### 3.1.2 查詢單筆參數檔
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom180/parameters/{tKey}`
- **說明**: 根據主鍵查詢單筆參數檔資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 標準單筆回應格式

#### 3.1.3 批次新增/修改參數檔
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom180/parameters/batch`
- **說明**: 批次新增或修改參數檔資料
- **請求格式**:
  ```json
  {
    "items": [
      {
        "tKey": null,
        "tag": "PARAM001",
        "content": "參數內容",
        "content2": "參數內容1"
      },
      {
        "tKey": 1,
        "tag": "PARAM001",
        "content": "修改後的參數內容",
        "content2": "修改後的參數內容1"
      }
    ]
  }
  ```
- **回應格式**: 標準批次操作回應格式

#### 3.1.4 批次刪除參數檔
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom180/parameters/batch`
- **說明**: 批次刪除參數檔資料
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```
- **回應格式**: 標準批次操作回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom180Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom180")]
    [Authorize]
    public class XCom180Controller : ControllerBase
    {
        private readonly IXCom180Service _service;
        
        public XCom180Controller(IXCom180Service service)
        {
            _service = service;
        }
        
        [HttpGet("parameters")]
        public async Task<ActionResult<ApiResponse<PagedResult<ParameterDto>>>> GetParameters([FromQuery] ParameterQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("parameters/{tKey}")]
        public async Task<ActionResult<ApiResponse<ParameterDto>>> GetParameter(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost("parameters/batch")]
        public async Task<ActionResult<ApiResponse<BatchResult>>> BatchUpdateParameters([FromBody] BatchParameterDto dto)
        {
            // 實作批次新增/修改邏輯
        }
        
        [HttpDelete("parameters/batch")]
        public async Task<ActionResult<ApiResponse>> BatchDeleteParameters([FromBody] BatchDeleteDto dto)
        {
            // 實作批次刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `XCom180Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom180Service
    {
        Task<PagedResult<ParameterDto>> GetParametersAsync(ParameterQueryDto query);
        Task<ParameterDto> GetParameterByIdAsync(long tKey);
        Task<BatchResult> BatchUpdateParametersAsync(BatchParameterDto dto);
        Task BatchDeleteParametersAsync(List<long> tKeys);
    }
}
```

#### 3.2.3 Repository: `ParameterRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IParameterRepository
    {
        Task<Parameter> GetByIdAsync(long tKey);
        Task<PagedResult<Parameter>> GetPagedAsync(ParameterQuery query);
        Task<Parameter> CreateAsync(Parameter parameter);
        Task<Parameter> UpdateAsync(Parameter parameter);
        Task DeleteAsync(long tKey);
        Task<bool> ExistsAsync(long tKey);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 參數檔維護頁面 (`ParameterMaintenance.vue`)
- **路徑**: `/xcom180/parameters`
- **功能**: 顯示參數檔列表，支援查詢、批次新增、批次修改、批次刪除
- **主要元件**:
  - 查詢表單 (ParameterSearchForm)
  - 資料表格 (ParameterDataTable)
  - 批次操作按鈕

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ParameterSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="參數項目">
      <el-input v-model="searchForm.tag" placeholder="請輸入參數項目" />
    </el-form-item>
    <el-form-item label="參數內容">
      <el-input v-model="searchForm.content" placeholder="請輸入參數內容" />
    </el-form-item>
    <el-form-item label="參數內容1">
      <el-input v-model="searchForm.content2" placeholder="請輸入參數內容1" />
    </el-form-item>
    <el-form-item label="參數說明">
      <el-input v-model="searchForm.notes" placeholder="請輸入參數說明" />
    </el-form-item>
    <el-form-item label="參數標題">
      <el-input v-model="searchForm.title" placeholder="請輸入參數標題" />
    </el-form-item>
    <el-form-item label="顯示筆數">
      <el-input-number v-model="searchForm.recordCountFrom" :min="1" />
      <span> ~ </span>
      <el-input-number v-model="searchForm.recordCountTo" :min="1" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`ParameterDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="parameterList" v-loading="loading">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="tag" label="參數項目" width="120">
        <template #default="{ row }">
          <el-input v-model="row.tag" :readonly="true" size="small" />
        </template>
      </el-table-column>
      <el-table-column prop="content" label="參數內容" width="200">
        <template #default="{ row }">
          <el-input v-model="row.content" size="small" maxlength="512" />
        </template>
      </el-table-column>
      <el-table-column prop="content2" label="參數內容1" width="150">
        <template #default="{ row }">
          <el-input v-model="row.content2" size="small" maxlength="128" />
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="參數說明" width="150">
        <template #default="{ row }">
          <el-input v-model="row.notes" :readonly="true" size="small" />
        </template>
      </el-table-column>
      <el-table-column prop="title" label="參數標題" width="150">
        <template #default="{ row }">
          <el-input v-model="row.title" :readonly="true" size="small" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ row }">
          <el-checkbox v-model="row.isDelete">刪除</el-checkbox>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top: 10px;">
      <el-button type="primary" @click="handleBatchSave">新增/修改</el-button>
      <el-button type="danger" @click="handleBatchDelete">刪除</el-button>
      <el-button @click="handleAddRow">新增一筆</el-button>
    </div>
  </div>
</template>
```

### 4.3 API 呼叫 (`xcom180.api.ts`)
```typescript
import request from '@/utils/request';

export interface ParameterDto {
  tKey?: number;
  tag: string;
  content?: string;
  content2?: string;
  notes?: string;
  title?: string;
  btime?: string;
  ctime?: string;
}

export interface ParameterQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    tag?: string;
    content?: string;
    content2?: string;
    notes?: string;
    title?: string;
  };
  recordCountFrom?: number;
  recordCountTo?: number;
}

export interface BatchParameterDto {
  items: ParameterDto[];
}

// API 函數
export const getParameterList = (query: ParameterQueryDto) => {
  return request.get<ApiResponse<PagedResult<ParameterDto>>>('/api/v1/xcom180/parameters', { params: query });
};

export const getParameterById = (tKey: number) => {
  return request.get<ApiResponse<ParameterDto>>(`/api/v1/xcom180/parameters/${tKey}`);
};

export const batchUpdateParameters = (data: BatchParameterDto) => {
  return request.post<ApiResponse<BatchResult>>('/api/v1/xcom180/parameters/batch', data);
};

export const batchDeleteParameters = (tKeys: number[]) => {
  return request.delete<ApiResponse>('/api/v1/xcom180/parameters/batch', { data: { tKeys } });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 批次處理邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 批次操作功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 批次操作測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 業務邏輯
- TAG欄位為唯讀，不可修改
- NOTES和TITLE欄位為唯讀，不可修改
- 只有CONTENT和CONTENT_2欄位可以修改
- 支援範圍查詢（recordCountFrom ~ recordCountTo）
- 批次操作時需要驗證必填欄位

### 6.2 資料驗證
- TAG欄位必填且不可為空
- CONTENT和CONTENT_2欄位長度限制
- 批次操作時需要檢查是否有選取資料

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 批次操作時需要考慮事務處理

### 6.4 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 批次操作需要記錄操作日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢參數檔列表成功
- [ ] 查詢單筆參數檔成功
- [ ] 批次新增參數檔成功
- [ ] 批次修改參數檔成功
- [ ] 批次刪除參數檔成功
- [ ] 範圍查詢功能測試

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 批次操作流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 批次操作效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM180_FQ.ASP` (查詢頁面)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM180_FU.ASP` (修改頁面)

### 8.2 資料庫 Schema
- 舊系統 `PARAM` 資料表結構

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


# IMS30_FQ - 資料查詢功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS30_FQ
- **功能名稱**: 資料查詢功能
- **功能描述**: 提供通用的資料查詢UI組件，用於查詢資料記錄，支援多條件查詢、排序、分頁、匯出等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FQ.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FQ.aspx.cs`
  - `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FQ.aspx`
  - `WEB/IMS_CORE/Kernel/IMS30_FQ.aspx`

### 1.2 業務需求
- 提供通用的資料查詢介面
- 支援多條件查詢
- 支援排序功能
- 支援分頁功能
- 支援資料匯出
- 支援查詢條件儲存
- 可被其他功能模組重用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `DataQueryConfigs` (資料查詢設定)

```sql
CREATE TABLE [dbo].[DataQueryConfigs] (
    [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [TableName] NVARCHAR(100) NOT NULL, -- 資料表名稱
    [QueryFields] NVARCHAR(MAX) NULL, -- 查詢欄位 (JSON格式)
    [DisplayFields] NVARCHAR(MAX) NULL, -- 顯示欄位 (JSON格式)
    [SortFields] NVARCHAR(MAX) NULL, -- 排序欄位 (JSON格式)
    [DefaultQuery] NVARCHAR(MAX) NULL, -- 預設查詢條件 (JSON格式)
    [PageSize] INT NOT NULL DEFAULT 20, -- 每頁筆數
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_DataQueryConfigs] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
    CONSTRAINT [UQ_DataQueryConfigs_ModuleCode] UNIQUE ([ModuleCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_DataQueryConfigs_ModuleCode] ON [dbo].[DataQueryConfigs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_DataQueryConfigs_Status] ON [dbo].[DataQueryConfigs] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `SavedQueries` - 儲存的查詢條件
```sql
CREATE TABLE [dbo].[SavedQueries] (
    [QueryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL,
    [QueryName] NVARCHAR(200) NOT NULL,
    [QueryConditions] NVARCHAR(MAX) NOT NULL, -- 查詢條件 (JSON格式)
    [UserId] NVARCHAR(50) NOT NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_SavedQueries_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SavedQueries_ModuleCode_UserId] ON [dbo].[SavedQueries] ([ModuleCode], [UserId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConfigId | BIGINT | - | NO | IDENTITY(1,1) | 設定ID | 主鍵 |
| ModuleCode | NVARCHAR | 50 | NO | - | 模組代碼 | 唯一 |
| TableName | NVARCHAR | 100 | NO | - | 資料表名稱 | - |
| QueryFields | NVARCHAR(MAX) | - | YES | - | 查詢欄位 | JSON格式 |
| DisplayFields | NVARCHAR(MAX) | - | YES | - | 顯示欄位 | JSON格式 |
| SortFields | NVARCHAR(MAX) | - | YES | - | 排序欄位 | JSON格式 |
| DefaultQuery | NVARCHAR(MAX) | - | YES | - | 預設查詢條件 | JSON格式 |
| PageSize | INT | - | NO | 20 | 每頁筆數 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢資料列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-query/{moduleCode}`
- **說明**: 根據模組代碼查詢資料列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "Id",
    "sortOrder": "ASC",
    "filters": {
      "field1": "value1",
      "field2": "value2"
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
      "items": [],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 取得查詢設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-query/{moduleCode}/config`
- **說明**: 取得資料查詢設定

#### 3.1.3 匯出資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-query/{moduleCode}/export`
- **說明**: 匯出查詢結果為Excel
- **請求格式**:
  ```json
  {
    "filters": {},
    "exportFields": []
  }
  ```

#### 3.1.4 儲存查詢條件
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-query/{moduleCode}/save-query`
- **說明**: 儲存查詢條件
- **請求格式**:
  ```json
  {
    "queryName": "我的查詢",
    "queryConditions": {},
    "isDefault": false
  }
  ```

#### 3.1.5 取得儲存的查詢條件
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-query/{moduleCode}/saved-queries`
- **說明**: 取得使用者儲存的查詢條件列表

#### 3.1.6 刪除儲存的查詢條件
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/data-query/saved-queries/{queryId}`
- **說明**: 刪除儲存的查詢條件

### 3.2 後端實作類別

#### 3.2.1 Controller: `DataQueryController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/data-query")]
    [Authorize]
    public class DataQueryController : ControllerBase
    {
        private readonly IDataQueryService _dataQueryService;
        
        public DataQueryController(IDataQueryService dataQueryService)
        {
            _dataQueryService = dataQueryService;
        }
        
        [HttpGet("{moduleCode}")]
        public async Task<ActionResult<ApiResponse<PagedResult<object>>>> QueryData(
            string moduleCode, 
            [FromQuery] DataQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{moduleCode}/config")]
        public async Task<ActionResult<ApiResponse<DataQueryConfigDto>>> GetConfig(string moduleCode)
        {
            // 實作取得設定邏輯
        }
        
        [HttpPost("{moduleCode}/export")]
        public async Task<ActionResult> ExportData(string moduleCode, [FromBody] ExportQueryDto dto)
        {
            // 實作匯出邏輯
        }
        
        [HttpPost("{moduleCode}/save-query")]
        public async Task<ActionResult<ApiResponse<long>>> SaveQuery(string moduleCode, [FromBody] SaveQueryDto dto)
        {
            // 實作儲存查詢條件邏輯
        }
        
        [HttpGet("{moduleCode}/saved-queries")]
        public async Task<ActionResult<ApiResponse<List<SavedQueryDto>>>> GetSavedQueries(string moduleCode)
        {
            // 實作取得儲存查詢條件邏輯
        }
        
        [HttpDelete("saved-queries/{queryId}")]
        public async Task<ActionResult<ApiResponse>> DeleteSavedQuery(long queryId)
        {
            // 實作刪除儲存查詢條件邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 資料查詢頁面 (`DataQuery.vue`)
- **路徑**: `/kernel/data-query/:moduleCode`
- **功能**: 顯示查詢表單和結果列表，支援多條件查詢、排序、分頁、匯出
- **主要元件**:
  - 查詢表單 (QueryForm)
  - 資料表格 (DataTable)
  - 分頁元件 (Pagination)

### 4.2 UI 元件設計

#### 4.2.1 資料查詢元件 (`DataQuery.vue`)
```vue
<template>
  <div class="data-query">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ moduleName }} - 查詢</span>
          <div>
            <el-button type="primary" @click="handleQuery">查詢</el-button>
            <el-button @click="handleReset">重置</el-button>
            <el-button @click="handleExport">匯出</el-button>
            <el-button @click="handleSaveQuery">儲存查詢</el-button>
          </div>
        </div>
      </template>
      
      <!-- 查詢表單 -->
      <QueryForm 
        :fields="queryFields" 
        v-model="queryForm"
        :saved-queries="savedQueries"
        @load-query="handleLoadQuery"
      />
      
      <!-- 資料表格 -->
      <DataTable 
        :data="dataList" 
        :loading="loading"
        :fields="displayFields"
        @sort-change="handleSortChange"
      />
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
      />
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`dataQuery.api.ts`)
```typescript
import request from '@/utils/request';

export interface DataQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: Record<string, any>;
}

export interface DataQueryConfigDto {
  moduleCode: string;
  tableName: string;
  queryFields: any[];
  displayFields: any[];
  sortFields: any[];
  defaultQuery?: Record<string, any>;
  pageSize: number;
}

export interface SavedQueryDto {
  queryId: number;
  queryName: string;
  queryConditions: Record<string, any>;
  isDefault: boolean;
}

// API 函數
export const queryData = (moduleCode: string, query: DataQueryDto) => {
  return request.get<ApiResponse<PagedResult<any>>>(`/api/v1/kernel/data-query/${moduleCode}`, { params: query });
};

export const getQueryConfig = (moduleCode: string) => {
  return request.get<ApiResponse<DataQueryConfigDto>>(`/api/v1/kernel/data-query/${moduleCode}/config`);
};

export const exportData = (moduleCode: string, data: ExportQueryDto) => {
  return request.post(`/api/v1/kernel/data-query/${moduleCode}/export`, data, { responseType: 'blob' });
};

export const saveQuery = (moduleCode: string, data: SaveQueryDto) => {
  return request.post<ApiResponse<number>>(`/api/v1/kernel/data-query/${moduleCode}/save-query`, data);
};

export const getSavedQueries = (moduleCode: string) => {
  return request.get<ApiResponse<SavedQueryDto[]>>(`/api/v1/kernel/data-query/${moduleCode}/saved-queries`);
};

export const deleteSavedQuery = (queryId: number) => {
  return request.delete<ApiResponse>(`/api/v1/kernel/data-query/saved-queries/${queryId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] Excel匯出功能實作
- [ ] 查詢條件儲存功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 資料查詢頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 分頁元件開發
- [ ] 匯出功能開發
- [ ] 查詢條件儲存功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須防止SQL注入
- 必須驗證輸入參數
- 必須限制匯出資料量

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制
- 匯出大量資料時必須使用背景任務

### 6.3 資料驗證
- 查詢條件必須驗證
- 分頁參數必須驗證
- 排序參數必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢資料列表成功
- [ ] 取得查詢設定成功
- [ ] 匯出資料成功
- [ ] 儲存查詢條件成功
- [ ] 取得儲存查詢條件成功
- [ ] 刪除儲存查詢條件成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 匯出功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FQ.aspx.cs`
- `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FQ.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


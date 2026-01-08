# IMS30_FB - 資料瀏覽功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS30_FB
- **功能名稱**: 資料瀏覽功能
- **功能描述**: 提供通用的資料瀏覽UI組件，用於顯示資料列表，支援分頁、排序、篩選等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FB.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FB.aspx.cs`
  - `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FB.aspx`
  - `WEB/IMS_CORE/Kernel/IMS30_FB.aspx`

### 1.2 業務需求
- 提供通用的資料瀏覽介面
- 支援資料列表顯示
- 支援分頁功能
- 支援排序功能
- 支援篩選功能
- 支援資料匯出
- 可被其他功能模組重用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `DataBrowseConfigs` (資料瀏覽設定)

```sql
CREATE TABLE [dbo].[DataBrowseConfigs] (
    [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [TableName] NVARCHAR(100) NOT NULL, -- 資料表名稱
    [DisplayFields] NVARCHAR(MAX) NULL, -- 顯示欄位 (JSON格式)
    [FilterFields] NVARCHAR(MAX) NULL, -- 篩選欄位 (JSON格式)
    [SortFields] NVARCHAR(MAX) NULL, -- 排序欄位 (JSON格式)
    [PageSize] INT NOT NULL DEFAULT 20, -- 每頁筆數
    [DefaultSort] NVARCHAR(200) NULL, -- 預設排序
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_DataBrowseConfigs] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
    CONSTRAINT [UQ_DataBrowseConfigs_ModuleCode] UNIQUE ([ModuleCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_DataBrowseConfigs_ModuleCode] ON [dbo].[DataBrowseConfigs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_DataBrowseConfigs_Status] ON [dbo].[DataBrowseConfigs] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConfigId | BIGINT | - | NO | IDENTITY(1,1) | 設定ID | 主鍵 |
| ModuleCode | NVARCHAR | 50 | NO | - | 模組代碼 | 唯一 |
| TableName | NVARCHAR | 100 | NO | - | 資料表名稱 | - |
| DisplayFields | NVARCHAR(MAX) | - | YES | - | 顯示欄位 | JSON格式 |
| FilterFields | NVARCHAR(MAX) | - | YES | - | 篩選欄位 | JSON格式 |
| SortFields | NVARCHAR(MAX) | - | YES | - | 排序欄位 | JSON格式 |
| PageSize | INT | - | NO | 20 | 每頁筆數 | - |
| DefaultSort | NVARCHAR | 200 | YES | - | 預設排序 | - |
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
- **路徑**: `/api/v1/kernel/data-browse/{moduleCode}`
- **說明**: 根據模組代碼查詢資料列表，支援分頁、排序、篩選
- **路徑參數**:
  - `moduleCode`: 模組代碼
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

#### 3.1.2 取得瀏覽設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-browse/{moduleCode}/config`
- **說明**: 取得資料瀏覽設定
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "moduleCode": "SYS0110",
      "tableName": "Users",
      "displayFields": [],
      "filterFields": [],
      "sortFields": [],
      "pageSize": 20,
      "defaultSort": "UserId ASC"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增瀏覽設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-browse/config`
- **說明**: 新增資料瀏覽設定
- **請求格式**:
  ```json
  {
    "moduleCode": "SYS0110",
    "tableName": "Users",
    "displayFields": [],
    "filterFields": [],
    "sortFields": [],
    "pageSize": 20,
    "defaultSort": "UserId ASC"
  }
  ```

#### 3.1.4 修改瀏覽設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/data-browse/config/{configId}`
- **說明**: 修改資料瀏覽設定

#### 3.1.5 刪除瀏覽設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/data-browse/config/{configId}`
- **說明**: 刪除資料瀏覽設定

### 3.2 後端實作類別

#### 3.2.1 Controller: `DataBrowseController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/data-browse")]
    [Authorize]
    public class DataBrowseController : ControllerBase
    {
        private readonly IDataBrowseService _dataBrowseService;
        
        public DataBrowseController(IDataBrowseService dataBrowseService)
        {
            _dataBrowseService = dataBrowseService;
        }
        
        [HttpGet("{moduleCode}")]
        public async Task<ActionResult<ApiResponse<PagedResult<object>>>> GetDataList(
            string moduleCode, 
            [FromQuery] DataBrowseQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{moduleCode}/config")]
        public async Task<ActionResult<ApiResponse<DataBrowseConfigDto>>> GetConfig(string moduleCode)
        {
            // 實作取得設定邏輯
        }
        
        [HttpPost("config")]
        public async Task<ActionResult<ApiResponse<long>>> CreateConfig([FromBody] CreateDataBrowseConfigDto dto)
        {
            // 實作新增設定邏輯
        }
        
        [HttpPut("config/{configId}")]
        public async Task<ActionResult<ApiResponse>> UpdateConfig(long configId, [FromBody] UpdateDataBrowseConfigDto dto)
        {
            // 實作修改設定邏輯
        }
        
        [HttpDelete("config/{configId}")]
        public async Task<ActionResult<ApiResponse>> DeleteConfig(long configId)
        {
            // 實作刪除設定邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 資料瀏覽頁面 (`DataBrowse.vue`)
- **路徑**: `/kernel/data-browse/:moduleCode`
- **功能**: 顯示資料列表，支援查詢、分頁、排序、篩選
- **主要元件**:
  - 查詢表單 (DataBrowseSearchForm)
  - 資料表格 (DataBrowseTable)
  - 分頁元件 (Pagination)

### 4.2 UI 元件設計

#### 4.2.1 資料瀏覽元件 (`DataBrowse.vue`)
```vue
<template>
  <div class="data-browse">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ moduleName }}</span>
          <el-button type="primary" @click="handleExport">匯出</el-button>
        </div>
      </template>
      
      <!-- 查詢表單 -->
      <DataBrowseSearchForm 
        :config="config" 
        @search="handleSearch" 
        @reset="handleReset" 
      />
      
      <!-- 資料表格 -->
      <DataBrowseTable 
        :data="dataList" 
        :loading="loading"
        :config="config"
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

### 4.3 API 呼叫 (`dataBrowse.api.ts`)
```typescript
import request from '@/utils/request';

export interface DataBrowseQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: Record<string, any>;
}

export interface DataBrowseConfigDto {
  moduleCode: string;
  tableName: string;
  displayFields: any[];
  filterFields: any[];
  sortFields: any[];
  pageSize: number;
  defaultSort?: string;
}

// API 函數
export const getDataList = (moduleCode: string, query: DataBrowseQueryDto) => {
  return request.get<ApiResponse<PagedResult<any>>>(`/api/v1/kernel/data-browse/${moduleCode}`, { params: query });
};

export const getBrowseConfig = (moduleCode: string) => {
  return request.get<ApiResponse<DataBrowseConfigDto>>(`/api/v1/kernel/data-browse/${moduleCode}/config`);
};

export const createBrowseConfig = (data: CreateDataBrowseConfigDto) => {
  return request.post<ApiResponse<number>>('/api/v1/kernel/data-browse/config', data);
};

export const updateBrowseConfig = (configId: number, data: UpdateDataBrowseConfigDto) => {
  return request.put<ApiResponse>(`/api/v1/kernel/data-browse/config/${configId}`, data);
};

export const deleteBrowseConfig = (configId: number) => {
  return request.delete<ApiResponse>(`/api/v1/kernel/data-browse/config/${configId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 資料瀏覽頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 分頁元件開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須防止SQL注入
- 必須驗證輸入參數

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 模組代碼必須驗證
- 分頁參數必須驗證
- 排序參數必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢資料列表成功
- [ ] 取得瀏覽設定成功
- [ ] 新增瀏覽設定成功
- [ ] 修改瀏覽設定成功
- [ ] 刪除瀏覽設定成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FB.aspx.cs`
- `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FB.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


# SYSZ1D0 - 統計報表查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSZ1D0
- **功能名稱**: 統計報表查詢 (TABLE SCHEMA 列表)
- **功能描述**: 提供資料庫表結構查詢功能，支援根據主系統代碼或使用者ID查詢資料表清單，並可產生資料庫彙總表、明細表，支援Word格式匯出和Index列表產生
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/SYSZ1D0_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/SYSZ1D0_TABLE_M_PR.asp` (資料庫彙總表)
  - `WEB/IMS_CORE/ASP/XCOM000/SYSZ1D0_TABLE_D_PR.asp` (資料庫明細表)

### 1.2 業務需求
- 根據主系統代碼查詢資料表清單
- 根據使用者ID查詢資料表清單
- 產生資料庫彙總表
- 產生資料庫明細表
- 支援Word格式匯出
- 支援Index列表產生
- 查詢結果支援分頁顯示

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TableSchemaQueries` (資料表結構查詢記錄)

```sql
CREATE TABLE [dbo].[TableSchemaQueries] (
    [QueryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 查詢ID
    [QueryType] NVARCHAR(20) NOT NULL, -- 查詢類型 (SYSTEM_ID, USER_ID)
    [SystemId] NVARCHAR(50) NULL, -- 主系統代碼
    [UserId] NVARCHAR(50) NULL, -- 使用者ID
    [TableName] NVARCHAR(200) NULL, -- 資料表名稱
    [QueryParams] NVARCHAR(MAX) NULL, -- 查詢參數 (JSON格式)
    [ExportFormat] NVARCHAR(20) NULL, -- 匯出格式 (WORD, EXCEL, PDF)
    [IncludeIndex] BIT NOT NULL DEFAULT 0, -- 是否產生Index列表
    [QueriedBy] NVARCHAR(50) NULL, -- 查詢者
    [QueriedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 查詢時間
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TableSchemaQueries] PRIMARY KEY CLUSTERED ([QueryId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TableSchemaQueries_SystemId] ON [dbo].[TableSchemaQueries] ([SystemId]);
CREATE NONCLUSTERED INDEX [IX_TableSchemaQueries_UserId] ON [dbo].[TableSchemaQueries] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_TableSchemaQueries_QueriedBy] ON [dbo].[TableSchemaQueries] ([QueriedBy]);
CREATE NONCLUSTERED INDEX [IX_TableSchemaQueries_QueriedAt] ON [dbo].[TableSchemaQueries] ([QueriedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `SystemTables` - 系統資料表對應
- 參考: 需建立系統與資料表的對應關係表

#### 2.2.2 `UserTables` - 使用者資料表對應
- 參考: 需建立使用者與資料表的對應關係表

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| QueryId | BIGINT | - | NO | IDENTITY(1,1) | 查詢ID | 主鍵 |
| QueryType | NVARCHAR | 20 | NO | - | 查詢類型 | SYSTEM_ID, USER_ID |
| SystemId | NVARCHAR | 50 | YES | - | 主系統代碼 | - |
| UserId | NVARCHAR | 50 | YES | - | 使用者ID | - |
| TableName | NVARCHAR | 200 | YES | - | 資料表名稱 | - |
| QueryParams | NVARCHAR(MAX) | - | YES | - | 查詢參數 | JSON格式 |
| ExportFormat | NVARCHAR | 20 | YES | - | 匯出格式 | WORD, EXCEL, PDF |
| IncludeIndex | BIT | - | NO | 0 | 是否產生Index列表 | - |
| QueriedBy | NVARCHAR | 50 | YES | - | 查詢者 | - |
| QueriedAt | DATETIME2 | - | NO | GETDATE() | 查詢時間 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢資料表清單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz1d0/tables`
- **說明**: 根據主系統代碼或使用者ID查詢資料表清單，支援分頁、排序、篩選
- **請求參數**:
  - `queryType`: 查詢類型 (SYSTEM_ID, USER_ID)
  - `systemId`: 主系統代碼 (當queryType為SYSTEM_ID時必填)
  - `userId`: 使用者ID (當queryType為USER_ID時必填)
  - `tableName`: 資料表名稱 (選填，模糊查詢)
  - `pageIndex`: 頁碼 (預設: 1)
  - `pageSize`: 每頁筆數 (預設: 30)
  - `sortField`: 排序欄位 (選填)
  - `sortOrder`: 排序方向 (ASC, DESC)
- **回應格式**:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "tableName": "USERS",
        "systemId": "SYS01",
        "systemName": "系統管理",
        "tableComment": "使用者資料表"
      }
    ],
    "totalCount": 100,
    "pageIndex": 1,
    "pageSize": 30
  },
  "message": null
}
```

#### 3.1.2 查詢資料表結構 (彙總表)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz1d0/tables/summary`
- **說明**: 查詢資料表結構彙總表
- **請求參數**:
  - `queryType`: 查詢類型 (SYSTEM_ID, USER_ID)
  - `systemId`: 主系統代碼
  - `userId`: 使用者ID
  - `tableNames`: 資料表名稱列表 (逗號分隔)
  - `includeIndex`: 是否產生Index列表 (true/false)
- **回應格式**: 返回資料表結構彙總資料

#### 3.1.3 查詢資料表結構 (明細表)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz1d0/tables/detail`
- **說明**: 查詢資料表結構明細表
- **請求參數**: 同彙總表
- **回應格式**: 返回資料表結構明細資料

#### 3.1.4 匯出資料表結構報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sysz1d0/tables/export`
- **說明**: 匯出資料表結構報表 (Word格式)
- **請求格式**:
```json
{
  "queryType": "SYSTEM_ID",
  "systemId": "SYS01",
  "tableNames": ["USERS", "ROLES"],
  "exportFormat": "WORD",
  "includeIndex": true,
  "reportType": "SUMMARY" // SUMMARY, DETAIL
}
```
- **回應格式**: 返回檔案下載連結或檔案內容

#### 3.1.5 查詢查詢記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz1d0/queries`
- **說明**: 查詢歷史查詢記錄
- **請求參數**: 分頁參數
- **回應格式**: 返回查詢記錄列表

### 3.2 後端實作類別

#### 3.2.1 Controller: `Sysz1d0Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/sysz1d0")]
    [Authorize]
    public class Sysz1d0Controller : ControllerBase
    {
        private readonly ISysz1d0Service _service;
        
        public Sysz1d0Controller(ISysz1d0Service service)
        {
            _service = service;
        }
        
        [HttpGet("tables")]
        public async Task<ActionResult<ApiResponse<PagedResult<TableSchemaDto>>>> GetTables([FromQuery] TableSchemaQueryDto query)
        {
            var result = await _service.GetTablesAsync(query);
            return Ok(ApiResponse<PagedResult<TableSchemaDto>>.Success(result));
        }
        
        [HttpGet("tables/summary")]
        public async Task<ActionResult<ApiResponse<TableSummaryDto>>> GetTableSummary([FromQuery] TableSchemaQueryDto query)
        {
            var result = await _service.GetTableSummaryAsync(query);
            return Ok(ApiResponse<TableSummaryDto>.Success(result));
        }
        
        [HttpGet("tables/detail")]
        public async Task<ActionResult<ApiResponse<TableDetailDto>>> GetTableDetail([FromQuery] TableSchemaQueryDto query)
        {
            var result = await _service.GetTableDetailAsync(query);
            return Ok(ApiResponse<TableDetailDto>.Success(result));
        }
        
        [HttpPost("tables/export")]
        public async Task<ActionResult> ExportTableSchema([FromBody] ExportTableSchemaDto dto)
        {
            var fileContent = await _service.ExportTableSchemaAsync(dto);
            return File(fileContent.Content, fileContent.ContentType, fileContent.FileName);
        }
        
        [HttpGet("queries")]
        public async Task<ActionResult<ApiResponse<PagedResult<TableSchemaQueryDto>>>> GetQueries([FromQuery] QueryHistoryDto query)
        {
            var result = await _service.GetQueryHistoryAsync(query);
            return Ok(ApiResponse<PagedResult<TableSchemaQueryDto>>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `Sysz1d0Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ISysz1d0Service
    {
        Task<PagedResult<TableSchemaDto>> GetTablesAsync(TableSchemaQueryDto query);
        Task<TableSummaryDto> GetTableSummaryAsync(TableSchemaQueryDto query);
        Task<TableDetailDto> GetTableDetailAsync(TableSchemaQueryDto query);
        Task<FileContentDto> ExportTableSchemaAsync(ExportTableSchemaDto dto);
        Task<PagedResult<TableSchemaQueryDto>> GetQueryHistoryAsync(QueryHistoryDto query);
    }
}
```

#### 3.2.3 Repository: `Sysz1d0Repository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ISysz1d0Repository
    {
        Task<PagedResult<TableSchema>> GetTablesBySystemIdAsync(string systemId, PagedQuery query);
        Task<PagedResult<TableSchema>> GetTablesByUserIdAsync(string userId, PagedQuery query);
        Task<TableSchema> GetTableSchemaAsync(string tableName);
        Task<List<TableColumn>> GetTableColumnsAsync(string tableName);
        Task<List<TableIndex>> GetTableIndexesAsync(string tableName);
        Task<TableSchemaQuery> SaveQueryAsync(TableSchemaQuery query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 SYSZ1D0查詢頁面 (`Sysz1d0Query.vue`)
- **路徑**: `/xcom/sysz1d0/query`
- **功能**: 顯示查詢表單，支援主系統代碼或使用者ID查詢
- **主要元件**:
  - 查詢表單 (Sysz1d0QueryForm)
  - 資料表清單 (Sysz1d0TableList)
  - 匯出選項對話框

#### 4.1.2 SYSZ1D0報表頁面 (`Sysz1d0Report.vue`)
- **路徑**: `/xcom/sysz1d0/report`
- **功能**: 顯示資料表結構報表 (彙總表或明細表)

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`Sysz1d0QueryForm.vue`)
```vue
<template>
  <el-form :model="queryForm" :rules="rules" ref="formRef">
    <el-form-item label="查詢類型" prop="queryType">
      <el-radio-group v-model="queryForm.queryType">
        <el-radio label="SYSTEM_ID">主系統代碼</el-radio>
        <el-radio label="USER_ID">使用者ID</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item 
      label="主系統代碼" 
      prop="systemId"
      v-if="queryForm.queryType === 'SYSTEM_ID'"
    >
      <el-input v-model="queryForm.systemId" placeholder="請輸入主系統代碼" clearable />
    </el-form-item>
    <el-form-item 
      label="使用者ID" 
      prop="userId"
      v-if="queryForm.queryType === 'USER_ID'"
    >
      <el-input v-model="queryForm.userId" placeholder="請輸入使用者ID" clearable />
    </el-form-item>
    <el-form-item label="資料表名稱">
      <el-input v-model="queryForm.tableName" placeholder="請輸入資料表名稱" clearable />
    </el-form-item>
    <el-form-item>
      <el-checkbox v-model="queryForm.includeIndex">是否產生Index列表</el-checkbox>
      <el-checkbox v-model="queryForm.saveWord">是否要儲存Word</el-checkbox>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleQuery">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleExportSummary">資料庫彙總表</el-button>
      <el-button type="success" @click="handleExportDetail">資料庫明細表</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表清單元件 (`Sysz1d0TableList.vue`)
```vue
<template>
  <div>
    <el-table :data="tableList" v-loading="loading">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="tableName" label="資料表名稱" width="200" sortable />
      <el-table-column prop="systemId" label="主系統代碼" width="150" />
      <el-table-column prop="systemName" label="系統名稱" width="200" />
      <el-table-column prop="tableComment" label="資料表說明" min-width="200" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleViewSummary(row)">彙總表</el-button>
          <el-button type="info" size="small" @click="handleViewDetail(row)">明細表</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 30, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
  </div>
</template>
```

### 4.3 API 呼叫 (`sysz1d0.api.ts`)
```typescript
import request from '@/utils/request';

export interface TableSchemaDto {
  tableName: string;
  systemId?: string;
  systemName?: string;
  tableComment?: string;
}

export interface TableSchemaQueryDto {
  queryType: 'SYSTEM_ID' | 'USER_ID';
  systemId?: string;
  userId?: string;
  tableName?: string;
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  includeIndex?: boolean;
}

export interface ExportTableSchemaDto {
  queryType: 'SYSTEM_ID' | 'USER_ID';
  systemId?: string;
  userId?: string;
  tableNames: string[];
  exportFormat: 'WORD' | 'EXCEL' | 'PDF';
  includeIndex: boolean;
  reportType: 'SUMMARY' | 'DETAIL';
}

// API 函數
export const getTableList = (query: TableSchemaQueryDto) => {
  return request.get<ApiResponse<PagedResult<TableSchemaDto>>>('/api/v1/sysz1d0/tables', { params: query });
};

export const getTableSummary = (query: TableSchemaQueryDto) => {
  return request.get<ApiResponse<TableSummaryDto>>('/api/v1/sysz1d0/tables/summary', { params: query });
};

export const getTableDetail = (query: TableSchemaQueryDto) => {
  return request.get<ApiResponse<TableDetailDto>>('/api/v1/sysz1d0/tables/detail', { params: query });
};

export const exportTableSchema = (dto: ExportTableSchemaDto) => {
  return request.post('/api/v1/sysz1d0/tables/export', dto, { responseType: 'blob' });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立系統與資料表對應關係表
- [ ] 建立使用者與資料表對應關係表
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作 (包含動態SQL查詢)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] Word匯出功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 查詢表單開發
- [ ] 資料表清單開發
- [ ] 報表顯示頁面開發
- [ ] 匯出功能整合
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] Word匯出測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查，僅允許SA (系統管理員) 使用
- 資料表結構查詢必須限制可查詢的資料表範圍
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料表查詢必須使用分頁
- 必須建立適當的索引
- Word匯出功能必須使用非同步處理

### 6.3 資料驗證
- 查詢類型必須驗證
- 主系統代碼或使用者ID必須至少填寫一個
- 資料表名稱必須驗證格式

### 6.4 業務邏輯
- 根據查詢類型動態查詢不同的資料表對應關係
- Word匯出必須支援中文字體
- Index列表必須正確產生

---

## 七、測試案例

### 7.1 單元測試
- [ ] 根據主系統代碼查詢資料表清單成功
- [ ] 根據使用者ID查詢資料表清單成功
- [ ] 查詢資料表結構彙總表成功
- [ ] 查詢資料表結構明細表成功
- [ ] Word匯出功能測試
- [ ] Index列表產生測試

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] Word匯出整合測試

### 7.3 效能測試
- [ ] 大量資料表查詢測試
- [ ] 並發查詢測試
- [ ] Word匯出效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/SYSZ1D0_FQ.asp` - 查詢畫面
- `WEB/IMS_CORE/ASP/XCOM000/SYSZ1D0_TABLE_M_PR.asp` - 資料庫彙總表
- `WEB/IMS_CORE/ASP/XCOM000/SYSZ1D0_TABLE_D_PR.asp` - 資料庫明細表

### 8.2 資料庫 Schema
- 舊系統資料表：需參考舊系統實際資料表結構
- 主要功能：TABLE SCHEMA 列表查詢，支援Word匯出和Index列表產生

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


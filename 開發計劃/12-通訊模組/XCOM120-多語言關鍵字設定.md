# XCOM120 - 多語言關鍵字設定 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM120
- **功能名稱**: 多語言關鍵字設定
- **功能描述**: 提供多語言關鍵字的新增、修改、刪除、查詢功能，支援多語系轉換設定
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM120_PR.ASP` (報表)

### 1.2 業務需求
- 管理系統多語言關鍵字對應關係
- 支援基準語系與多個轉換語系的設定
- 提供關鍵字查詢與維護功能
- 支援多語系切換顯示

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `MultiLangs`

```sql
CREATE TABLE [dbo].[MultiLangs] (
    [TKey] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [BaseKey] NVARCHAR(200) NOT NULL,
    [Trans1] NVARCHAR(200) NULL,
    [Trans2] NVARCHAR(200) NULL,
    [CUser] NVARCHAR(50) NULL,
    [CTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [BUser] NVARCHAR(50) NULL,
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CPriority] INT NULL DEFAULT 0,
    [CGroup] NVARCHAR(50) NULL,
    CONSTRAINT [PK_MultiLangs] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_MultiLangs_BaseKey] ON [dbo].[MultiLangs] ([BaseKey]);
CREATE NONCLUSTERED INDEX [IX_MultiLangs_Trans1] ON [dbo].[MultiLangs] ([Trans1]);
CREATE NONCLUSTERED INDEX [IX_MultiLangs_Trans2] ON [dbo].[MultiLangs] ([Trans2]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | NVARCHAR | 50 | NO | - | 關鍵字編號 | 主鍵，唯一 |
| BaseKey | NVARCHAR | 200 | NO | - | 基準語系關鍵字 | - |
| Trans1 | NVARCHAR | 200 | YES | - | 轉換語系一 | - |
| Trans2 | NVARCHAR | 200 | YES | - | 轉換語系二 | - |
| CUser | NVARCHAR | 50 | YES | - | 建立者 | - |
| CTime | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| BUser | NVARCHAR | 50 | YES | - | 更新者 | - |
| BTime | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CPriority | INT | - | YES | 0 | 建立者等級 | - |
| CGroup | NVARCHAR | 50 | YES | - | 建立者組別 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢多語言關鍵字列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/multilangs`
- **說明**: 查詢多語言關鍵字列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TKey",
    "sortOrder": "ASC",
    "filters": {
      "tKey": "",
      "baseKey": "",
      "trans1": "",
      "trans2": ""
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
          "tKey": "KEY001",
          "baseKey": "使用者",
          "trans1": "User",
          "trans2": "ユーザー",
          "cUser": "U001",
          "cTime": "2024-01-01T10:00:00",
          "bUser": "U001",
          "bTime": "2024-01-01T10:00:00"
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

#### 3.1.2 查詢單筆多語言關鍵字
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/multilangs/{tKey}`
- **說明**: 根據關鍵字編號查詢單筆資料
- **路徑參數**:
  - `tKey`: 關鍵字編號
- **回應格式**: 同列表單筆資料格式

#### 3.1.3 新增多語言關鍵字
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/multilangs`
- **說明**: 新增多語言關鍵字資料
- **請求格式**:
  ```json
  {
    "tKey": "KEY001",
    "baseKey": "使用者",
    "trans1": "User",
    "trans2": "ユーザー"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": "KEY001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改多語言關鍵字
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/multilangs/{tKey}`
- **說明**: 修改多語言關鍵字資料
- **路徑參數**:
  - `tKey`: 關鍵字編號
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除多語言關鍵字
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/multilangs/{tKey}`
- **說明**: 刪除多語言關鍵字資料
- **路徑參數**:
  - `tKey`: 關鍵字編號
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

#### 3.1.6 批次刪除多語言關鍵字
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/multilangs/batch`
- **說明**: 批次刪除多筆資料
- **請求格式**:
  ```json
  {
    "tKeys": ["KEY001", "KEY002", "KEY003"]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `MultiLangsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/multilangs")]
    [Authorize]
    public class MultiLangsController : ControllerBase
    {
        private readonly IMultiLangService _multiLangService;
        
        public MultiLangsController(IMultiLangService multiLangService)
        {
            _multiLangService = multiLangService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<MultiLangDto>>>> GetMultiLangs([FromQuery] MultiLangQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<MultiLangDto>>> GetMultiLang(string tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateMultiLang([FromBody] CreateMultiLangDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateMultiLang(string tKey, [FromBody] UpdateMultiLangDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteMultiLang(string tKey)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `MultiLangService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IMultiLangService
    {
        Task<PagedResult<MultiLangDto>> GetMultiLangsAsync(MultiLangQueryDto query);
        Task<MultiLangDto> GetMultiLangByKeyAsync(string tKey);
        Task<string> CreateMultiLangAsync(CreateMultiLangDto dto);
        Task UpdateMultiLangAsync(string tKey, UpdateMultiLangDto dto);
        Task DeleteMultiLangAsync(string tKey);
    }
}
```

#### 3.2.3 Repository: `MultiLangRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IMultiLangRepository
    {
        Task<MultiLang> GetByKeyAsync(string tKey);
        Task<PagedResult<MultiLang>> GetPagedAsync(MultiLangQuery query);
        Task<MultiLang> CreateAsync(MultiLang multiLang);
        Task<MultiLang> UpdateAsync(MultiLang multiLang);
        Task DeleteAsync(string tKey);
        Task<bool> ExistsAsync(string tKey);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 多語言關鍵字列表頁面 (`MultiLangList.vue`)
- **路徑**: `/system/multilangs`
- **功能**: 顯示多語言關鍵字列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (MultiLangSearchForm)
  - 資料表格 (MultiLangDataTable)
  - 新增/修改對話框 (MultiLangDialog)
  - 刪除確認對話框

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`MultiLangSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="關鍵字編號">
      <el-input v-model="searchForm.tKey" placeholder="請輸入關鍵字編號" />
    </el-form-item>
    <el-form-item label="基準語系關鍵字">
      <el-input v-model="searchForm.baseKey" placeholder="請輸入基準語系關鍵字" />
    </el-form-item>
    <el-form-item label="轉換語系一">
      <el-input v-model="searchForm.trans1" placeholder="請輸入轉換語系一" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`MultiLangDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="multiLangList" v-loading="loading">
      <el-table-column prop="tKey" label="關鍵字編號" width="120" />
      <el-table-column prop="baseKey" label="基準語系關鍵字" width="200" />
      <el-table-column prop="trans1" label="轉換語系一" width="200" />
      <el-table-column prop="trans2" label="轉換語系二" width="200" />
      <el-table-column prop="cTime" label="建立時間" width="160" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
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

#### 4.2.3 新增/修改對話框 (`MultiLangDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
      <el-form-item label="關鍵字編號" prop="tKey">
        <el-input v-model="form.tKey" :disabled="isEdit" placeholder="請輸入關鍵字編號" />
      </el-form-item>
      <el-form-item label="基準語系關鍵字" prop="baseKey">
        <el-input v-model="form.baseKey" placeholder="請輸入基準語系關鍵字" />
      </el-form-item>
      <el-form-item label="轉換語系一" prop="trans1">
        <el-input v-model="form.trans1" placeholder="請輸入轉換語系一" />
      </el-form-item>
      <el-form-item label="轉換語系二" prop="trans2">
        <el-input v-model="form.trans2" placeholder="請輸入轉換語系二" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`multilang.api.ts`)
```typescript
import request from '@/utils/request';

export interface MultiLangDto {
  tKey: string;
  baseKey: string;
  trans1?: string;
  trans2?: string;
  cUser?: string;
  cTime?: string;
  bUser?: string;
  bTime?: string;
}

export interface MultiLangQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    tKey?: string;
    baseKey?: string;
    trans1?: string;
    trans2?: string;
  };
}

export interface CreateMultiLangDto {
  tKey: string;
  baseKey: string;
  trans1?: string;
  trans2?: string;
}

export interface UpdateMultiLangDto extends Omit<CreateMultiLangDto, 'tKey'> {}

// API 函數
export const getMultiLangList = (query: MultiLangQueryDto) => {
  return request.get<ApiResponse<PagedResult<MultiLangDto>>>('/api/v1/multilangs', { params: query });
};

export const getMultiLangByKey = (tKey: string) => {
  return request.get<ApiResponse<MultiLangDto>>(`/api/v1/multilangs/${tKey}`);
};

export const createMultiLang = (data: CreateMultiLangDto) => {
  return request.post<ApiResponse<string>>('/api/v1/multilangs', data);
};

export const updateMultiLang = (tKey: string, data: UpdateMultiLangDto) => {
  return request.put<ApiResponse>(`/api/v1/multilangs/${tKey}`, data);
};

export const deleteMultiLang = (tKey: string) => {
  return request.delete<ApiResponse>(`/api/v1/multilangs/${tKey}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 關鍵字編號必須唯一
- 必填欄位必須驗證
- 基準語系關鍵字必須驗證

### 6.4 業務邏輯
- 刪除關鍵字前必須檢查是否有相關資料
- 多語系轉換必須正確對應

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增多語言關鍵字成功
- [ ] 新增多語言關鍵字失敗 (重複編號)
- [ ] 修改多語言關鍵字成功
- [ ] 修改多語言關鍵字失敗 (不存在)
- [ ] 刪除多語言關鍵字成功
- [ ] 查詢多語言關鍵字列表成功
- [ ] 查詢單筆多語言關鍵字成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FB.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FI.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FU.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FD.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM120_FQ.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM120_PR.ASP`

### 8.2 資料庫 Schema
- 舊系統使用 Oracle 資料庫，表名為 `MULTI_LANGS`
- 新系統使用 MS SQL Server，表名為 `MultiLangs`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


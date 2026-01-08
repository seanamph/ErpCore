# IMS30_FS - 資料排序功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS30_FS
- **功能名稱**: 資料排序功能
- **功能描述**: 提供通用的資料排序UI組件，用於設定資料顯示順序，支援多欄位排序、排序規則儲存等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FS.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FS_V2.aspx`
  - `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FS.aspx`
  - `WEB/IMS_CORE/Kernel/IMS30_FS.aspx`

### 1.2 業務需求
- 提供通用的資料排序介面
- 支援多欄位排序
- 支援排序規則儲存
- 支援排序優先順序設定
- 可被其他功能模組重用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `DataSortConfigs` (資料排序設定)

```sql
CREATE TABLE [dbo].[DataSortConfigs] (
    [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [TableName] NVARCHAR(100) NOT NULL, -- 資料表名稱
    [SortFields] NVARCHAR(MAX) NULL, -- 可排序欄位 (JSON格式)
    [DefaultSort] NVARCHAR(MAX) NULL, -- 預設排序 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_DataSortConfigs] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
    CONSTRAINT [UQ_DataSortConfigs_ModuleCode] UNIQUE ([ModuleCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_DataSortConfigs_ModuleCode] ON [dbo].[DataSortConfigs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_DataSortConfigs_Status] ON [dbo].[DataSortConfigs] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `SavedSorts` - 儲存的排序規則
```sql
CREATE TABLE [dbo].[SavedSorts] (
    [SortId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL,
    [SortName] NVARCHAR(200) NOT NULL,
    [SortRules] NVARCHAR(MAX) NOT NULL, -- 排序規則 (JSON格式)
    [UserId] NVARCHAR(50) NOT NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_SavedSorts_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SavedSorts_ModuleCode_UserId] ON [dbo].[SavedSorts] ([ModuleCode], [UserId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConfigId | BIGINT | - | NO | IDENTITY(1,1) | 設定ID | 主鍵 |
| ModuleCode | NVARCHAR | 50 | NO | - | 模組代碼 | 唯一 |
| TableName | NVARCHAR | 100 | NO | - | 資料表名稱 | - |
| SortFields | NVARCHAR(MAX) | - | YES | - | 可排序欄位 | JSON格式 |
| DefaultSort | NVARCHAR(MAX) | - | YES | - | 預設排序 | JSON格式 |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得排序設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-sort/{moduleCode}/config`
- **說明**: 取得資料排序設定
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "moduleCode": "SYS0110",
      "tableName": "Users",
      "sortFields": [],
      "defaultSort": []
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 套用排序
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-sort/{moduleCode}/apply`
- **說明**: 套用排序規則到資料查詢
- **請求格式**:
  ```json
  {
    "sortRules": [
      {
        "field": "UserId",
        "order": "ASC"
      },
      {
        "field": "UserName",
        "order": "DESC"
      }
    ]
  }
  ```

#### 3.1.3 儲存排序規則
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-sort/{moduleCode}/save-sort`
- **說明**: 儲存排序規則
- **請求格式**:
  ```json
  {
    "sortName": "我的排序",
    "sortRules": [],
    "isDefault": false
  }
  ```

#### 3.1.4 取得儲存的排序規則
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-sort/{moduleCode}/saved-sorts`
- **說明**: 取得使用者儲存的排序規則列表

#### 3.1.5 刪除儲存的排序規則
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/data-sort/saved-sorts/{sortId}`
- **說明**: 刪除儲存的排序規則

### 3.2 後端實作類別

#### 3.2.1 Controller: `DataSortController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/data-sort")]
    [Authorize]
    public class DataSortController : ControllerBase
    {
        private readonly IDataSortService _dataSortService;
        
        public DataSortController(IDataSortService dataSortService)
        {
            _dataSortService = dataSortService;
        }
        
        [HttpGet("{moduleCode}/config")]
        public async Task<ActionResult<ApiResponse<DataSortConfigDto>>> GetConfig(string moduleCode)
        {
            // 實作取得設定邏輯
        }
        
        [HttpPost("{moduleCode}/apply")]
        public async Task<ActionResult<ApiResponse>> ApplySort(string moduleCode, [FromBody] SortRulesDto dto)
        {
            // 實作套用排序邏輯
        }
        
        [HttpPost("{moduleCode}/save-sort")]
        public async Task<ActionResult<ApiResponse<long>>> SaveSort(string moduleCode, [FromBody] SaveSortDto dto)
        {
            // 實作儲存排序規則邏輯
        }
        
        [HttpGet("{moduleCode}/saved-sorts")]
        public async Task<ActionResult<ApiResponse<List<SavedSortDto>>>> GetSavedSorts(string moduleCode)
        {
            // 實作取得儲存排序規則邏輯
        }
        
        [HttpDelete("saved-sorts/{sortId}")]
        public async Task<ActionResult<ApiResponse>> DeleteSavedSort(long sortId)
        {
            // 實作刪除儲存排序規則邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 資料排序頁面 (`DataSort.vue`)
- **路徑**: `/kernel/data-sort/:moduleCode`
- **功能**: 顯示排序設定介面，支援多欄位排序、排序規則儲存
- **主要元件**:
  - 排序設定表單 (SortForm)
  - 排序規則列表 (SortRulesList)

### 4.2 UI 元件設計

#### 4.2.1 資料排序元件 (`DataSort.vue`)
```vue
<template>
  <div class="data-sort">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ moduleName }} - 排序設定</span>
        </div>
      </template>
      
      <!-- 排序規則設定 -->
      <el-form :model="sortForm" label-width="120px">
        <el-form-item label="排序欄位">
          <SortRulesList 
            v-model="sortForm.sortRules"
            :available-fields="sortFields"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleApply">套用</el-button>
          <el-button @click="handleSave">儲存</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 儲存的排序規則 -->
      <el-card v-if="savedSorts.length > 0" style="margin-top: 20px;">
        <template #header>
          <span>儲存的排序規則</span>
        </template>
        <el-table :data="savedSorts">
          <el-table-column prop="sortName" label="排序名稱" />
          <el-table-column label="操作">
            <template #default="{ row }">
              <el-button type="primary" size="small" @click="handleLoadSort(row)">載入</el-button>
              <el-button type="danger" size="small" @click="handleDeleteSort(row)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`dataSort.api.ts`)
```typescript
import request from '@/utils/request';

export interface DataSortConfigDto {
  moduleCode: string;
  tableName: string;
  sortFields: any[];
  defaultSort: any[];
}

export interface SortRule {
  field: string;
  order: 'ASC' | 'DESC';
}

export interface SavedSortDto {
  sortId: number;
  sortName: string;
  sortRules: SortRule[];
  isDefault: boolean;
}

// API 函數
export const getSortConfig = (moduleCode: string) => {
  return request.get<ApiResponse<DataSortConfigDto>>(`/api/v1/kernel/data-sort/${moduleCode}/config`);
};

export const applySort = (moduleCode: string, sortRules: SortRule[]) => {
  return request.post<ApiResponse>(`/api/v1/kernel/data-sort/${moduleCode}/apply`, { sortRules });
};

export const saveSort = (moduleCode: string, data: SaveSortDto) => {
  return request.post<ApiResponse<number>>(`/api/v1/kernel/data-sort/${moduleCode}/save-sort`, data);
};

export const getSavedSorts = (moduleCode: string) => {
  return request.get<ApiResponse<SavedSortDto[]>>(`/api/v1/kernel/data-sort/${moduleCode}/saved-sorts`);
};

export const deleteSavedSort = (sortId: number) => {
  return request.delete<ApiResponse>(`/api/v1/kernel/data-sort/saved-sorts/${sortId}`);
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
- [ ] 排序邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 資料排序頁面開發
- [ ] 排序規則設定元件開發
- [ ] 排序規則儲存功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 排序功能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須驗證排序欄位
- 必須防止SQL注入

### 6.2 效能
- 必須建立適當的索引
- 必須優化排序查詢效能

### 6.3 資料驗證
- 排序欄位必須驗證
- 排序順序必須驗證
- 排序規則必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 取得排序設定成功
- [ ] 套用排序成功
- [ ] 儲存排序規則成功
- [ ] 取得儲存排序規則成功
- [ ] 刪除儲存排序規則成功

### 7.2 整合測試
- [ ] 完整排序流程測試
- [ ] 權限檢查測試
- [ ] 多欄位排序測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FS.aspx.cs`
- `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FS.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


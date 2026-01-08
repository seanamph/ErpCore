# SYSBP00 - 基礎人事功能(參數設定) 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSBP00
- **功能名稱**: 基礎人事功能(參數設定)
- **功能描述**: 提供系統參數設定的新增、修改、刪除、查詢功能，包含參數標題、參數標籤、參數內容、排序序號、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FS.ASP` (顯示)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_PR.ASP` (報表)

### 1.2 業務需求
- 管理系統參數設定資訊
- 支援參數的新增、修改、刪除、查詢
- 記錄參數的建立與變更資訊
- 支援參數的啟用/停用
- 支援只讀參數設定
- 支援參數排序序號設定
- 支援多系統參數管理
- 支援參數標題唯一性檢查

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Parameters` (對應舊系統 `PARAM`)

```sql
CREATE TABLE [dbo].[Parameters] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [Title] NVARCHAR(100) NOT NULL, -- 參數標題 (TITLE)
    [Tag] NVARCHAR(100) NOT NULL, -- 參數標籤/代碼 (TAG)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
    [Content] NVARCHAR(500) NULL, -- 參數內容 (CONTENT)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 2:停用
    [ReadOnly] NVARCHAR(10) NULL DEFAULT '0', -- 只讀標誌 (READONLY) 1:只讀, 0:可編輯
    [SystemId] NVARCHAR(50) NULL, -- 系統ID (SYS_ID)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Parameters_Title] ON [dbo].[Parameters] ([Title]);
CREATE NONCLUSTERED INDEX [IX_Parameters_Tag] ON [dbo].[Parameters] ([Tag]);
CREATE NONCLUSTERED INDEX [IX_Parameters_Title_Tag] ON [dbo].[Parameters] ([Title], [Tag]);
CREATE NONCLUSTERED INDEX [IX_Parameters_Status] ON [dbo].[Parameters] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Parameters_SeqNo] ON [dbo].[Parameters] ([SeqNo]);
CREATE NONCLUSTERED INDEX [IX_Parameters_SystemId] ON [dbo].[Parameters] ([SystemId]);
```

### 2.2 相關資料表

#### 2.2.1 `Systems` - 系統主檔
```sql
-- 用於查詢系統列表
-- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0410-主系統項目資料維護.md`
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| Title | NVARCHAR | 100 | NO | - | 參數標題 | 需唯一 |
| Tag | NVARCHAR | 100 | NO | - | 參數標籤/代碼 | - |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Content | NVARCHAR | 500 | YES | - | 參數內容 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 2:停用 |
| ReadOnly | NVARCHAR | 10 | YES | '0' | 只讀標誌 | 1:只讀, 0:可編輯 |
| SystemId | NVARCHAR | 50 | YES | - | 系統ID | 外鍵至Systems表 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢參數列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/parameters`
- **說明**: 查詢參數列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "Title",
    "sortOrder": "ASC",
    "filters": {
      "title": "",
      "notes": "",
      "status": "",
      "systemId": ""
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
          "title": "參數標題",
          "systemId": "SYSB000",
          "notes": "參數說明",
          "readOnly": "0",
          "status": "1"
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

#### 3.1.2 查詢單筆參數
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/parameters/{tKey}`
- **說明**: 根據主鍵查詢單筆參數資料（包含所有參數項目）
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "title": "參數標題",
      "systemId": "SYSB000",
      "readOnly": "0",
      "notes": "參數說明",
      "items": [
        {
          "tag": "TAG001",
          "seqNo": 1,
          "content": "參數內容",
          "status": "1"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增參數
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/parameters`
- **說明**: 新增參數資料（包含參數標題和多個參數項目）
- **請求格式**:
  ```json
  {
    "title": "參數標題",
    "systemId": "SYSB000",
    "readOnly": "0",
    "notes": "參數說明",
    "items": [
      {
        "tag": "TAG001",
        "seqNo": 1,
        "content": "參數內容",
        "status": "1"
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改參數
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/parameters/{tKey}`
- **說明**: 修改參數資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除參數
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/parameters/{tKey}`
- **說明**: 刪除參數資料（需檢查是否為只讀參數）
- **路徑參數**:
  - `tKey`: 主鍵
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

### 3.2 後端實作類別

#### 3.2.1 Controller: `ParametersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/parameters")]
    [Authorize]
    public class ParametersController : ControllerBase
    {
        private readonly IParameterService _parameterService;
        
        public ParametersController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ParameterDto>>>> GetParameters([FromQuery] ParameterQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<ParameterDetailDto>>> GetParameter(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateParameter([FromBody] CreateParameterDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateParameter(long tKey, [FromBody] UpdateParameterDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteParameter(long tKey)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `ParameterService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IParameterService
    {
        Task<PagedResult<ParameterDto>> GetParametersAsync(ParameterQueryDto query);
        Task<ParameterDetailDto> GetParameterByIdAsync(long tKey);
        Task<long> CreateParameterAsync(CreateParameterDto dto);
        Task UpdateParameterAsync(long tKey, UpdateParameterDto dto);
        Task DeleteParameterAsync(long tKey);
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
        Task<bool> ExistsByTitleAsync(string title);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 參數列表頁面 (`ParameterList.vue`)
- **路徑**: `/system/parameters`
- **功能**: 顯示參數列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (ParameterSearchForm)
  - 資料表格 (ParameterDataTable)
  - 新增/修改對話框 (ParameterDialog)
  - 刪除確認對話框

#### 4.1.2 參數詳細頁面 (`ParameterDetail.vue`)
- **路徑**: `/system/parameters/:tKey`
- **功能**: 顯示參數詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ParameterSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="參數標題/參數說明">
      <el-input v-model="searchForm.title" placeholder="請輸入參數標題或說明" />
    </el-form-item>
    <el-form-item label="系統別">
      <el-select v-model="searchForm.systemId" placeholder="請選擇系統別">
        <el-option v-for="system in systemList" :key="system.systemId" :label="system.systemName" :value="system.systemId" />
      </el-select>
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
      <el-table-column prop="title" label="參數標題" width="200" />
      <el-table-column prop="systemName" label="系統別" width="120" />
      <el-table-column prop="notes" label="參數說明" />
      <el-table-column prop="readOnly" label="是否唯讀" width="100">
        <template #default="{ row }">
          <el-tag :type="row.readOnly === '1' ? 'warning' : 'success'">
            {{ row.readOnly === '1' ? '唯讀' : '可編輯' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.readOnly === '1'">刪除</el-button>
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

#### 4.2.3 新增/修改對話框 (`ParameterDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="參數標題" prop="title">
        <el-input v-model="form.title" :disabled="isEdit" placeholder="請輸入參數標題" />
      </el-form-item>
      <el-form-item label="系統別" prop="systemId">
        <el-select v-model="form.systemId" placeholder="請選擇系統別">
          <el-option v-for="system in systemList" :key="system.systemId" :label="system.systemName" :value="system.systemId" />
        </el-select>
      </el-form-item>
      <el-form-item label="是否唯讀" prop="readOnly">
        <el-select v-model="form.readOnly" placeholder="請選擇">
          <el-option label="可編輯" value="0" />
          <el-option label="唯讀" value="1" />
        </el-select>
      </el-form-item>
      <el-form-item label="參數說明" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="2" placeholder="請輸入參數說明" />
      </el-form-item>
      <el-form-item label="參數項目">
        <el-table :data="form.items" border>
          <el-table-column prop="tag" label="參數項目" width="150">
            <template #default="{ row, $index }">
              <el-input v-model="row.tag" placeholder="請輸入參數項目" />
            </template>
          </el-table-column>
          <el-table-column prop="status" label="參數狀態" width="120">
            <template #default="{ row }">
              <el-select v-model="row.status">
                <el-option label="啟用" value="1" />
                <el-option label="停用" value="2" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="seqNo" label="排序序號" width="120">
            <template #default="{ row }">
              <el-input-number v-model="row.seqNo" :min="0" />
            </template>
          </el-table-column>
          <el-table-column prop="content" label="參數內容">
            <template #default="{ row }">
              <el-input v-model="row.content" placeholder="請輸入參數內容" />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleRemoveItem($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" @click="handleAddItem" style="margin-top: 10px">新增項目</el-button>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`parameter.api.ts`)
```typescript
import request from '@/utils/request';

export interface ParameterDto {
  tKey: number;
  title: string;
  systemId?: string;
  systemName?: string;
  notes?: string;
  readOnly: string;
  status: string;
}

export interface ParameterItemDto {
  tag: string;
  seqNo: number;
  content: string;
  status: string;
}

export interface ParameterDetailDto extends ParameterDto {
  items: ParameterItemDto[];
}

export interface ParameterQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    title?: string;
    notes?: string;
    status?: string;
    systemId?: string;
  };
}

export interface CreateParameterDto {
  title: string;
  systemId?: string;
  readOnly: string;
  notes?: string;
  items: ParameterItemDto[];
}

export interface UpdateParameterDto extends CreateParameterDto {}

// API 函數
export const getParameterList = (query: ParameterQueryDto) => {
  return request.get<ApiResponse<PagedResult<ParameterDto>>>('/api/v1/parameters', { params: query });
};

export const getParameterById = (tKey: number) => {
  return request.get<ApiResponse<ParameterDetailDto>>(`/api/v1/parameters/${tKey}`);
};

export const createParameter = (data: CreateParameterDto) => {
  return request.post<ApiResponse<long>>('/api/v1/parameters', data);
};

export const updateParameter = (tKey: number, data: UpdateParameterDto) => {
  return request.put<ApiResponse>(`/api/v1/parameters/${tKey}`, data);
};

export const deleteParameter = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/parameters/${tKey}`);
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
- [ ] 驗證邏輯實作（參數標題唯一性檢查）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 參數項目動態新增/刪除功能
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
- 只讀參數不可刪除
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 參數標題必須唯一
- 參數項目標籤不可為空
- 參數內容不可為空
- 排序序號必須為非負整數

### 6.4 業務邏輯
- 刪除參數前必須檢查是否為只讀參數
- 參數標題重複時需提示警告
- 參數項目可動態新增/刪除
- 需支援批次新增參數項目

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增參數成功
- [ ] 新增參數失敗 (標題重複)
- [ ] 修改參數成功
- [ ] 修改參數失敗 (不存在)
- [ ] 刪除參數成功
- [ ] 刪除參數失敗 (只讀參數)
- [ ] 查詢參數列表成功
- [ ] 查詢單筆參數成功

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
- `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_FS.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSBP00_PR.ASP`

### 8.2 相關功能
- `SYSBC40-參數資料設定維護作業` - 類似功能，可參考其設計

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


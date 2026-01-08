# SYS8A10-SYS8A45 - 租賃擴展維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS8A10-SYS8A45 系列
- **功能名稱**: 租賃擴展維護系列
- **功能描述**: 提供租賃擴展資料的新增、修改、刪除、查詢功能，包含租賃擴展資訊、特殊條件、附加條款、擴展設定等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A20_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A30_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8A45_FI.ASP` (新增)

### 1.2 業務需求
- 管理租賃擴展基本資料
- 支援租賃擴展資訊的新增、修改、刪除、查詢
- 支援特殊條件設定
- 支援附加條款管理
- 支援擴展設定維護
- 支援租賃擴展報表列印
- 支援租賃擴展歷史記錄查詢
- 支援多店別管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LeaseExtensions` (租賃擴展主檔)

```sql
CREATE TABLE [dbo].[LeaseExtensions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號 (EXTENSION_ID)
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
    [ExtensionType] NVARCHAR(20) NOT NULL, -- 擴展類型 (EXTENSION_TYPE, CONDITION:特殊條件, TERM:附加條款, SETTING:擴展設定)
    [ExtensionName] NVARCHAR(200) NULL, -- 擴展名稱 (EXTENSION_NAME)
    [ExtensionValue] NVARCHAR(MAX) NULL, -- 擴展值 (EXTENSION_VALUE)
    [StartDate] DATETIME2 NULL, -- 開始日期 (START_DATE)
    [EndDate] DATETIME2 NULL, -- 結束日期 (END_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_LeaseExtensions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_LeaseExtensions_ExtensionId] UNIQUE ([ExtensionId]),
    CONSTRAINT [FK_LeaseExtensions_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionId] ON [dbo].[LeaseExtensions] ([ExtensionId]);
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_LeaseId] ON [dbo].[LeaseExtensions] ([LeaseId]);
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionType] ON [dbo].[LeaseExtensions] ([ExtensionType]);
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_Status] ON [dbo].[LeaseExtensions] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `LeaseExtensionDetails` - 租賃擴展明細
```sql
CREATE TABLE [dbo].[LeaseExtensionDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [FieldName] NVARCHAR(100) NULL, -- 欄位名稱 (FIELD_NAME)
    [FieldValue] NVARCHAR(MAX) NULL, -- 欄位值 (FIELD_VALUE)
    [FieldType] NVARCHAR(20) NULL, -- 欄位類型 (FIELD_TYPE, TEXT:文字, NUMBER:數字, DATE:日期, BOOLEAN:布林)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseExtensionDetails_LeaseExtensions] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[LeaseExtensions] ([ExtensionId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseExtensionDetails_ExtensionId] ON [dbo].[LeaseExtensionDetails] ([ExtensionId]);
```

### 2.3 資料字典

#### 2.3.1 LeaseExtensions 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ExtensionId | NVARCHAR | 50 | NO | - | 擴展編號 | 唯一，EXTENSION_ID |
| LeaseId | NVARCHAR | 50 | NO | - | 租賃編號 | 外鍵至租賃表 |
| ExtensionType | NVARCHAR | 20 | NO | - | 擴展類型 | CONDITION:特殊條件, TERM:附加條款, SETTING:擴展設定 |
| ExtensionName | NVARCHAR | 200 | YES | - | 擴展名稱 | - |
| ExtensionValue | NVARCHAR | MAX | YES | - | 擴展值 | - |
| StartDate | DATETIME2 | - | YES | - | 開始日期 | - |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢租賃擴展列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-extensions`
- **說明**: 查詢租賃擴展列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ExtensionId",
    "sortOrder": "ASC",
    "filters": {
      "extensionId": "",
      "leaseId": "",
      "extensionType": "",
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
          "extensionId": "EXT001",
          "leaseId": "L001",
          "extensionType": "CONDITION",
          "extensionName": "特殊條件1",
          "extensionValue": "條件內容",
          "startDate": "2024-01-01",
          "endDate": "2024-12-31",
          "status": "A",
          "statusName": "啟用"
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

#### 3.1.2 查詢單筆租賃擴展
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-extensions/{extensionId}`
- **說明**: 根據擴展編號查詢單筆租賃擴展資料
- **路徑參數**:
  - `extensionId`: 擴展編號
- **回應格式**: 同查詢列表，但只返回單筆資料，包含明細資料

#### 3.1.3 根據租賃編號查詢擴展列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-extensions/by-lease/{leaseId}`
- **說明**: 根據租賃編號查詢該租賃的所有擴展資料
- **路徑參數**:
  - `leaseId`: 租賃編號
- **回應格式**: 同查詢列表

#### 3.1.4 新增租賃擴展
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-extensions`
- **說明**: 新增租賃擴展資料
- **請求格式**:
  ```json
  {
    "extensionId": "EXT001",
    "leaseId": "L001",
    "extensionType": "CONDITION",
    "extensionName": "特殊條件1",
    "extensionValue": "條件內容",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "status": "A",
    "seqNo": 1,
    "memo": "備註",
    "details": [
      {
        "fieldName": "FIELD1",
        "fieldValue": "VALUE1",
        "fieldType": "TEXT"
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
      "extensionId": "EXT001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 修改租賃擴展
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lease-extensions/{extensionId}`
- **說明**: 修改租賃擴展資料
- **路徑參數**:
  - `extensionId`: 擴展編號
- **請求格式**: 同新增，但 `extensionId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除租賃擴展
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lease-extensions/{extensionId}`
- **說明**: 刪除租賃擴展資料（軟刪除或硬刪除）
- **路徑參數**:
  - `extensionId`: 擴展編號
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

#### 3.1.7 批次刪除租賃擴展
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lease-extensions/batch`
- **說明**: 批次刪除多筆租賃擴展
- **請求格式**:
  ```json
  {
    "extensionIds": ["EXT001", "EXT002", "EXT003"]
  }
  ```

#### 3.1.8 更新租賃擴展狀態
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lease-extensions/{extensionId}/status`
- **說明**: 更新租賃擴展狀態
- **請求格式**:
  ```json
  {
    "status": "I" // A:啟用, I:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `LeaseExtensionsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/lease-extensions")]
    [Authorize]
    public class LeaseExtensionsController : ControllerBase
    {
        private readonly ILeaseExtensionService _service;
        
        public LeaseExtensionsController(ILeaseExtensionService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<LeaseExtensionDto>>>> GetLeaseExtensions([FromQuery] LeaseExtensionQueryDto query)
        {
            var result = await _service.GetLeaseExtensionsAsync(query);
            return Ok(ApiResponse<PagedResult<LeaseExtensionDto>>.Success(result));
        }
        
        [HttpGet("{extensionId}")]
        public async Task<ActionResult<ApiResponse<LeaseExtensionDto>>> GetLeaseExtension(string extensionId)
        {
            var result = await _service.GetLeaseExtensionByIdAsync(extensionId);
            return Ok(ApiResponse<LeaseExtensionDto>.Success(result));
        }
        
        [HttpGet("by-lease/{leaseId}")]
        public async Task<ActionResult<ApiResponse<List<LeaseExtensionDto>>>> GetLeaseExtensionsByLeaseId(string leaseId)
        {
            var result = await _service.GetLeaseExtensionsByLeaseIdAsync(leaseId);
            return Ok(ApiResponse<List<LeaseExtensionDto>>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLeaseExtension([FromBody] CreateLeaseExtensionDto dto)
        {
            var extensionId = await _service.CreateLeaseExtensionAsync(dto);
            return Ok(ApiResponse<string>.Success(extensionId));
        }
        
        [HttpPut("{extensionId}")]
        public async Task<ActionResult<ApiResponse>> UpdateLeaseExtension(string extensionId, [FromBody] UpdateLeaseExtensionDto dto)
        {
            await _service.UpdateLeaseExtensionAsync(extensionId, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("{extensionId}")]
        public async Task<ActionResult<ApiResponse>> DeleteLeaseExtension(string extensionId)
        {
            await _service.DeleteLeaseExtensionAsync(extensionId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("{extensionId}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateStatus(string extensionId, [FromBody] UpdateLeaseExtensionStatusDto dto)
        {
            await _service.UpdateStatusAsync(extensionId, dto.Status);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `LeaseExtensionService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ILeaseExtensionService
    {
        Task<PagedResult<LeaseExtensionDto>> GetLeaseExtensionsAsync(LeaseExtensionQueryDto query);
        Task<LeaseExtensionDto> GetLeaseExtensionByIdAsync(string extensionId);
        Task<List<LeaseExtensionDto>> GetLeaseExtensionsByLeaseIdAsync(string leaseId);
        Task<string> CreateLeaseExtensionAsync(CreateLeaseExtensionDto dto);
        Task UpdateLeaseExtensionAsync(string extensionId, UpdateLeaseExtensionDto dto);
        Task DeleteLeaseExtensionAsync(string extensionId);
        Task UpdateStatusAsync(string extensionId, string status);
    }
}
```

#### 3.2.3 Repository: `LeaseExtensionRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ILeaseExtensionRepository
    {
        Task<LeaseExtension> GetByIdAsync(string extensionId);
        Task<PagedResult<LeaseExtension>> GetPagedAsync(LeaseExtensionQuery query);
        Task<List<LeaseExtension>> GetByLeaseIdAsync(string leaseId);
        Task<LeaseExtension> CreateAsync(LeaseExtension extension);
        Task<LeaseExtension> UpdateAsync(LeaseExtension extension);
        Task DeleteAsync(string extensionId);
        Task<bool> ExistsAsync(string extensionId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 租賃擴展列表頁面 (`LeaseExtensionList.vue`)
- **路徑**: `/lease/extensions`
- **功能**: 顯示租賃擴展列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (LeaseExtensionSearchForm)
  - 資料表格 (LeaseExtensionDataTable)
  - 新增/修改對話框 (LeaseExtensionDialog)
  - 刪除確認對話框

#### 4.1.2 租賃擴展詳細頁面 (`LeaseExtensionDetail.vue`)
- **路徑**: `/lease/extensions/:extensionId`
- **功能**: 顯示租賃擴展詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`LeaseExtensionSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="擴展編號">
      <el-input v-model="searchForm.extensionId" placeholder="請輸入擴展編號" />
    </el-form-item>
    <el-form-item label="租賃編號">
      <el-input v-model="searchForm.leaseId" placeholder="請輸入租賃編號" />
    </el-form-item>
    <el-form-item label="擴展類型">
      <el-select v-model="searchForm.extensionType" placeholder="請選擇擴展類型">
        <el-option label="特殊條件" value="CONDITION" />
        <el-option label="附加條款" value="TERM" />
        <el-option label="擴展設定" value="SETTING" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="啟用" value="A" />
        <el-option label="停用" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`LeaseExtensionDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="extensionList" v-loading="loading">
      <el-table-column prop="extensionId" label="擴展編號" width="150" />
      <el-table-column prop="leaseId" label="租賃編號" width="150" />
      <el-table-column prop="extensionType" label="擴展類型" width="120">
        <template #default="{ row }">
          {{ getExtensionTypeText(row.extensionType) }}
        </template>
      </el-table-column>
      <el-table-column prop="extensionName" label="擴展名稱" width="200" />
      <el-table-column prop="startDate" label="開始日期" width="120" />
      <el-table-column prop="endDate" label="結束日期" width="120" />
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="row.status === 'A' ? 'success' : 'info'">
            {{ row.status === 'A' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
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

#### 4.2.3 新增/修改對話框 (`LeaseExtensionDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="擴展編號" prop="extensionId">
        <el-input v-model="form.extensionId" :disabled="isEdit" placeholder="請輸入擴展編號" />
      </el-form-item>
      <el-form-item label="租賃編號" prop="leaseId">
        <el-select v-model="form.leaseId" placeholder="請選擇租賃" :disabled="isEdit">
          <el-option v-for="lease in leaseList" :key="lease.leaseId" :label="lease.leaseId" :value="lease.leaseId" />
        </el-select>
      </el-form-item>
      <el-form-item label="擴展類型" prop="extensionType">
        <el-select v-model="form.extensionType" placeholder="請選擇擴展類型">
          <el-option label="特殊條件" value="CONDITION" />
          <el-option label="附加條款" value="TERM" />
          <el-option label="擴展設定" value="SETTING" />
        </el-select>
      </el-form-item>
      <el-form-item label="擴展名稱" prop="extensionName">
        <el-input v-model="form.extensionName" placeholder="請輸入擴展名稱" />
      </el-form-item>
      <el-form-item label="擴展值" prop="extensionValue">
        <el-input v-model="form.extensionValue" type="textarea" :rows="4" placeholder="請輸入擴展值" />
      </el-form-item>
      <el-form-item label="開始日期" prop="startDate">
        <el-date-picker v-model="form.startDate" type="date" placeholder="請選擇日期" />
      </el-form-item>
      <el-form-item label="結束日期" prop="endDate">
        <el-date-picker v-model="form.endDate" type="date" placeholder="請選擇日期" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="A" />
          <el-option label="停用" value="I" />
        </el-select>
      </el-form-item>
      <el-form-item label="排序序號" prop="seqNo">
        <el-input-number v-model="form.seqNo" :min="0" />
      </el-form-item>
      <el-form-item label="備註" prop="memo">
        <el-input v-model="form.memo" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`leaseExtension.api.ts`)
```typescript
import request from '@/utils/request';

export interface LeaseExtensionDto {
  extensionId: string;
  leaseId: string;
  extensionType: string;
  extensionName?: string;
  extensionValue?: string;
  startDate?: string;
  endDate?: string;
  status: string;
  seqNo?: number;
  memo?: string;
  details?: LeaseExtensionDetailDto[];
}

export interface LeaseExtensionDetailDto {
  fieldName: string;
  fieldValue: string;
  fieldType: string;
}

export interface LeaseExtensionQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    extensionId?: string;
    leaseId?: string;
    extensionType?: string;
    status?: string;
  };
}

export interface CreateLeaseExtensionDto {
  extensionId: string;
  leaseId: string;
  extensionType: string;
  extensionName?: string;
  extensionValue?: string;
  startDate?: string;
  endDate?: string;
  status: string;
  seqNo?: number;
  memo?: string;
  details?: LeaseExtensionDetailDto[];
}

export interface UpdateLeaseExtensionDto extends Omit<CreateLeaseExtensionDto, 'extensionId'> {}

// API 函數
export const getLeaseExtensionList = (query: LeaseExtensionQueryDto) => {
  return request.get<ApiResponse<PagedResult<LeaseExtensionDto>>>('/api/v1/lease-extensions', { params: query });
};

export const getLeaseExtensionById = (extensionId: string) => {
  return request.get<ApiResponse<LeaseExtensionDto>>(`/api/v1/lease-extensions/${extensionId}`);
};

export const getLeaseExtensionsByLeaseId = (leaseId: string) => {
  return request.get<ApiResponse<LeaseExtensionDto[]>>(`/api/v1/lease-extensions/by-lease/${leaseId}`);
};

export const createLeaseExtension = (data: CreateLeaseExtensionDto) => {
  return request.post<ApiResponse<string>>('/api/v1/lease-extensions', data);
};

export const updateLeaseExtension = (extensionId: string, data: UpdateLeaseExtensionDto) => {
  return request.put<ApiResponse>(`/api/v1/lease-extensions/${extensionId}`, data);
};

export const deleteLeaseExtension = (extensionId: string) => {
  return request.delete<ApiResponse>(`/api/v1/lease-extensions/${extensionId}`);
};

export const updateStatus = (extensionId: string, status: string) => {
  return request.put<ApiResponse>(`/api/v1/lease-extensions/${extensionId}/status`, { status });
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
- 租賃擴展資料變更必須記錄操作日誌
- 敏感資訊必須加密儲存

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 關聯查詢應使用 JOIN 優化

### 6.3 資料驗證
- 必填欄位必須驗證
- 狀態值必須在允許範圍內
- 日期範圍必須驗證
- 擴展類型必須在允許範圍內

### 6.4 業務邏輯
- 刪除租賃擴展前必須檢查是否有關聯的明細資料
- 狀態變更必須符合業務流程
- 租賃擴展資料變更必須記錄變更資訊
- 結束日期必須大於開始日期

### 6.5 關聯資料
- 與租賃的關聯
- 與擴展明細的關聯

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增租賃擴展成功
- [ ] 新增租賃擴展失敗 (必填欄位驗證)
- [ ] 修改租賃擴展成功
- [ ] 修改租賃擴展失敗 (狀態驗證)
- [ ] 刪除租賃擴展成功
- [ ] 刪除租賃擴展失敗 (有關聯資料)
- [ ] 查詢租賃擴展列表成功
- [ ] 查詢單筆租賃擴展成功
- [ ] 根據租賃編號查詢擴展列表成功
- [ ] 更新租賃擴展狀態成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 關聯資料測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FI.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FU.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FD.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_FB.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A10_PR.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A20_FI.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A30_FI.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A45_FI.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYS8000/SYS8A10.xsd` (如果存在)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


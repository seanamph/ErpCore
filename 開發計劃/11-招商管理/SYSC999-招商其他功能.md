# SYSC999 - 招商其他功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSC999
- **功能名稱**: 招商其他功能
- **功能描述**: 提供招商管理中的其他功能，包含租戶位置管理、收款設定、抽成設定等相關功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSC000/SYSC999_FU1.aspx` (修改)
  - `WEB/IMS_CORE/SYSC000/SYSC999_PR.aspx` (報表)
  - `IMS3/HANSHIN/IMS3/XCOM000/SYSC999.ascx` (業務邏輯)

### 1.2 業務需求
- 管理租戶位置資訊
- 支援租戶位置的新增、修改、刪除、查詢
- 支援收款設定管理
- 支援抽成設定管理
- 支援位置、區域、樓層的關聯查詢
- 與租戶管理（AGM）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TenantLocations` (對應舊系統 `TENA_LOCATION`)

```sql
CREATE TABLE [dbo].[TenantLocations] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [AgmTKey] BIGINT NOT NULL, -- 租戶主檔主鍵 (AGM_T_KEY)
    [LocationId] NVARCHAR(50) NOT NULL, -- 位置代碼 (LOCATION_ID)
    [AreaId] NVARCHAR(50) NULL, -- 區域代碼 (AREA_ID)
    [FloorId] NVARCHAR(50) NULL, -- 樓層代碼 (FLOOR_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_TenantLocations] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_TenantLocations_Tenants] FOREIGN KEY ([AgmTKey]) REFERENCES [dbo].[Tenants] ([TKey]) ON DELETE CASCADE,
    CONSTRAINT [FK_TenantLocations_Locations] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([LocationId])
);
```

### 2.2 相關資料表

#### 2.2.1 `Tenants` - 租戶主檔
- 用於查詢租戶列表
- 參考: 租戶管理相關功能

#### 2.2.2 `Locations` - 位置主檔
- 用於查詢位置列表
- 參考: 位置管理相關功能

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| AgmTKey | BIGINT | - | NO | - | 租戶主檔主鍵 | 外鍵至Tenants表 |
| LocationId | NVARCHAR | 50 | NO | - | 位置代碼 | 外鍵至Locations表 |
| AreaId | NVARCHAR | 50 | YES | - | 區域代碼 | - |
| FloorId | NVARCHAR | 50 | YES | - | 樓層代碼 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢租戶位置列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tenant-locations`
- **說明**: 查詢租戶位置列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "LocationId",
    "sortOrder": "ASC",
    "filters": {
      "agmTKey": "",
      "locationId": "",
      "areaId": "",
      "floorId": "",
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
          "tKey": 1,
          "agmTKey": 100,
          "locationId": "LOC001",
          "areaId": "AREA001",
          "floorId": "FLOOR001",
          "status": "1",
          "createdAt": "2024-01-01T00:00:00"
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

#### 3.1.2 查詢單筆租戶位置
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tenant-locations/{tKey}`
- **說明**: 根據主鍵查詢單筆租戶位置資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 根據租戶查詢位置列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tenant-locations/by-tenant/{agmTKey}`
- **說明**: 根據租戶主檔主鍵查詢該租戶的所有位置
- **路徑參數**:
  - `agmTKey`: 租戶主檔主鍵
- **回應格式**: 同查詢列表

#### 3.1.4 新增租戶位置
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/tenant-locations`
- **說明**: 新增租戶位置資料
- **請求格式**:
  ```json
  {
    "agmTKey": 100,
    "locationId": "LOC001",
    "areaId": "AREA001",
    "floorId": "FLOOR001",
    "status": "1",
    "notes": "備註"
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

#### 3.1.5 修改租戶位置
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/tenant-locations/{tKey}`
- **說明**: 修改租戶位置資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `agmTKey` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除租戶位置
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/tenant-locations/{tKey}`
- **說明**: 刪除租戶位置資料
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

#### 3.1.7 批次刪除租戶位置
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/tenant-locations/batch`
- **說明**: 批次刪除多筆租戶位置
- **請求格式**:
  ```json
  {
    "items": [1, 2, 3]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `TenantLocationsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/tenant-locations")]
    [Authorize]
    public class TenantLocationsController : ControllerBase
    {
        private readonly ITenantLocationService _tenantLocationService;
        
        public TenantLocationsController(ITenantLocationService tenantLocationService)
        {
            _tenantLocationService = tenantLocationService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TenantLocationDto>>>> GetTenantLocations([FromQuery] TenantLocationQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<TenantLocationDto>>> GetTenantLocation(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("by-tenant/{agmTKey}")]
        public async Task<ActionResult<ApiResponse<List<TenantLocationDto>>>> GetTenantLocationsByTenant(long agmTKey)
        {
            // 實作根據租戶查詢邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TenantLocationKeyDto>>> CreateTenantLocation([FromBody] CreateTenantLocationDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateTenantLocation(long tKey, [FromBody] UpdateTenantLocationDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteTenantLocation(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpDelete("batch")]
        public async Task<ActionResult<ApiResponse>> DeleteTenantLocationsBatch([FromBody] BatchDeleteTenantLocationDto dto)
        {
            // 實作批次刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `TenantLocationService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ITenantLocationService
    {
        Task<PagedResult<TenantLocationDto>> GetTenantLocationsAsync(TenantLocationQueryDto query);
        Task<TenantLocationDto> GetTenantLocationAsync(long tKey);
        Task<List<TenantLocationDto>> GetTenantLocationsByTenantAsync(long agmTKey);
        Task<TenantLocationKeyDto> CreateTenantLocationAsync(CreateTenantLocationDto dto);
        Task UpdateTenantLocationAsync(long tKey, UpdateTenantLocationDto dto);
        Task DeleteTenantLocationAsync(long tKey);
        Task DeleteTenantLocationsBatchAsync(BatchDeleteTenantLocationDto dto);
    }
}
```

#### 3.2.3 Repository: `TenantLocationRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ITenantLocationRepository
    {
        Task<TenantLocation> GetByKeyAsync(long tKey);
        Task<PagedResult<TenantLocation>> GetPagedAsync(TenantLocationQuery query);
        Task<List<TenantLocation>> GetByTenantAsync(long agmTKey);
        Task<TenantLocation> CreateAsync(TenantLocation tenantLocation);
        Task<TenantLocation> UpdateAsync(TenantLocation tenantLocation);
        Task DeleteAsync(long tKey);
        Task<bool> ExistsAsync(long tKey);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 租戶位置列表頁面 (`TenantLocationList.vue`)
- **路徑**: `/recruitment/tenant-locations`
- **功能**: 顯示租戶位置列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (TenantLocationSearchForm)
  - 資料表格 (TenantLocationDataTable)
  - 新增/修改對話框 (TenantLocationDialog)
  - 刪除確認對話框

#### 4.1.2 租戶位置詳細頁面 (`TenantLocationDetail.vue`)
- **路徑**: `/recruitment/tenant-locations/:tKey`
- **功能**: 顯示租戶位置詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`TenantLocationSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="租戶">
      <el-select v-model="searchForm.agmTKey" placeholder="請選擇租戶" filterable>
        <el-option v-for="tenant in tenantList" :key="tenant.tKey" :label="tenant.tenantName" :value="tenant.tKey" />
      </el-select>
    </el-form-item>
    <el-form-item label="位置代碼">
      <el-input v-model="searchForm.locationId" placeholder="請輸入位置代碼" />
    </el-form-item>
    <el-form-item label="區域代碼">
      <el-input v-model="searchForm.areaId" placeholder="請輸入區域代碼" />
    </el-form-item>
    <el-form-item label="樓層代碼">
      <el-input v-model="searchForm.floorId" placeholder="請輸入樓層代碼" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="啟用" value="1" />
        <el-option label="停用" value="0" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`TenantLocationDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="tenantLocationList" v-loading="loading">
      <el-table-column prop="locationId" label="位置代碼" width="120" />
      <el-table-column prop="areaId" label="區域代碼" width="120" />
      <el-table-column prop="floorId" label="樓層代碼" width="120" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '啟用' : '停用' }}
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

#### 4.2.3 新增/修改對話框 (`TenantLocationDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="租戶" prop="agmTKey" v-if="!isEdit">
        <el-select v-model="form.agmTKey" placeholder="請選擇租戶" filterable>
          <el-option v-for="tenant in tenantList" :key="tenant.tKey" :label="tenant.tenantName" :value="tenant.tKey" />
        </el-select>
      </el-form-item>
      <el-form-item label="位置代碼" prop="locationId">
        <el-select v-model="form.locationId" placeholder="請選擇位置" filterable>
          <el-option v-for="location in locationList" :key="location.locationId" :label="location.locationName" :value="location.locationId" />
        </el-select>
      </el-form-item>
      <el-form-item label="區域代碼" prop="areaId">
        <el-input v-model="form.areaId" placeholder="請輸入區域代碼" />
      </el-form-item>
      <el-form-item label="樓層代碼" prop="floorId">
        <el-input v-model="form.floorId" placeholder="請輸入樓層代碼" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="1" />
          <el-option label="停用" value="0" />
        </el-select>
      </el-form-item>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`tenant-location.api.ts`)
```typescript
import request from '@/utils/request';

export interface TenantLocationDto {
  tKey: number;
  agmTKey: number;
  locationId: string;
  areaId?: string;
  floorId?: string;
  status: string;
  notes?: string;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
}

export interface TenantLocationQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    agmTKey?: number;
    locationId?: string;
    areaId?: string;
    floorId?: string;
    status?: string;
  };
}

export interface CreateTenantLocationDto {
  agmTKey: number;
  locationId: string;
  areaId?: string;
  floorId?: string;
  status: string;
  notes?: string;
}

export interface UpdateTenantLocationDto extends Omit<CreateTenantLocationDto, 'agmTKey'> {}

export interface TenantLocationKeyDto {
  tKey: number;
}

export interface BatchDeleteTenantLocationDto {
  items: number[];
}

// API 函數
export const getTenantLocationList = (query: TenantLocationQueryDto) => {
  return request.get<ApiResponse<PagedResult<TenantLocationDto>>>('/api/v1/tenant-locations', { params: query });
};

export const getTenantLocation = (tKey: number) => {
  return request.get<ApiResponse<TenantLocationDto>>(`/api/v1/tenant-locations/${tKey}`);
};

export const getTenantLocationsByTenant = (agmTKey: number) => {
  return request.get<ApiResponse<TenantLocationDto[]>>(`/api/v1/tenant-locations/by-tenant/${agmTKey}`);
};

export const createTenantLocation = (data: CreateTenantLocationDto) => {
  return request.post<ApiResponse<TenantLocationKeyDto>>('/api/v1/tenant-locations', data);
};

export const updateTenantLocation = (tKey: number, data: UpdateTenantLocationDto) => {
  return request.put<ApiResponse>(`/api/v1/tenant-locations/${tKey}`, data);
};

export const deleteTenantLocation = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/tenant-locations/${tKey}`);
};

export const deleteTenantLocationsBatch = (data: BatchDeleteTenantLocationDto) => {
  return request.delete<ApiResponse>('/api/v1/tenant-locations/batch', { data });
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
- 租戶位置變更必須記錄操作日誌
- 刪除前必須檢查關聯資料

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 位置、區域、樓層的關聯查詢需要優化

### 6.3 資料驗證
- 租戶主檔主鍵必須存在
- 位置代碼必須存在
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 租戶位置變更必須記錄變更資訊
- 刪除租戶位置前必須檢查是否有關聯資料
- 支援根據位置、區域、樓層進行關聯查詢

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增租戶位置成功
- [ ] 新增租戶位置失敗 (重複位置)
- [ ] 修改租戶位置成功
- [ ] 修改租戶位置失敗 (租戶不存在)
- [ ] 刪除租戶位置成功
- [ ] 刪除租戶位置失敗 (有關聯資料)
- [ ] 查詢租戶位置列表成功
- [ ] 查詢單筆租戶位置成功
- [ ] 根據租戶查詢位置列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 關聯查詢測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 關聯查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/XCOM000/SYSC999.ascx.cs`
- `WEB/IMS_CORE/SYSC000/SYSC999_FU1.aspx`
- `WEB/IMS_CORE/SYSC000/SYSC999_PR.aspx`

### 8.2 資料庫 Schema
- 舊系統 `TENA_LOCATION` 資料表結構
- 舊系統 `LOCATION` 資料表結構

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01



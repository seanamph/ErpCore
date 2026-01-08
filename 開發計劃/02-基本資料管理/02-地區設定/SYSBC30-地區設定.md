# SYSBC30 - 地區設定 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSBC30
- **功能名稱**: 地區設定
- **功能描述**: 提供地區資料的新增、修改、刪除、查詢功能，包含地區編號、地區名稱、備註等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSB000/SYSBC30_FI.aspx` (新增)
  - `WEB/IMS_CORE/SYSB000/SYSBC30_FI1.aspx` (新增)
  - `WEB/IMS_CORE/SYSB000/SYSBC30_FU1.aspx` (修改)
  - `WEB/IMS_CORE/SYSB000/SYSBC30_FD.aspx` (刪除)
  - `WEB/IMS_CORE/SYSB000/SYSBC30_PR.rdlc` (報表)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_BASE/REGION_ID.cs` (業務邏輯)

### 1.2 業務需求
- 管理系統地區設定資訊
- 支援地區的新增、修改、刪除、查詢
- 記錄地區的建立與變更資訊
- 支援地區名稱查詢
- 與其他模組整合使用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Regions` (對應舊系統 `REGION`)

```sql
CREATE TABLE [dbo].[Regions] (
    [RegionId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 地區編號 (REGION_ID)
    [RegionName] NVARCHAR(100) NOT NULL, -- 地區名稱 (REGION_NAME)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED ([RegionId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Regions_RegionName] ON [dbo].[Regions] ([RegionName]);
```

### 2.2 相關資料表
無

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| RegionId | NVARCHAR | 50 | NO | - | 地區編號 | 主鍵 |
| RegionName | NVARCHAR | 100 | NO | - | 地區名稱 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢地區列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/regions`
- **說明**: 查詢地區列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "RegionId",
    "sortOrder": "ASC",
    "filters": {
      "regionId": "",
      "regionName": ""
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
          "regionId": "R001",
          "regionName": "台北地區",
          "memo": "備註",
          "createdBy": "U001",
          "createdAt": "2024-01-01T00:00:00",
          "updatedBy": "U001",
          "updatedAt": "2024-01-01T00:00:00"
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

#### 3.1.2 查詢單筆地區
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/regions/{regionId}`
- **說明**: 根據地區編號查詢單筆地區資料
- **路徑參數**:
  - `regionId`: 地區編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "regionId": "R001",
      "regionName": "台北地區",
      "memo": "備註",
      "createdBy": "U001",
      "createdAt": "2024-01-01T00:00:00",
      "updatedBy": "U001",
      "updatedAt": "2024-01-01T00:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增地區
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/regions`
- **說明**: 新增地區資料
- **請求格式**:
  ```json
  {
    "regionId": "R001",
    "regionName": "台北地區",
    "memo": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "regionId": "R001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改地區
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/regions/{regionId}`
- **說明**: 修改地區資料
- **路徑參數**:
  - `regionId`: 地區編號
- **請求格式**: 同新增，但 `regionId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除地區
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/regions/{regionId}`
- **說明**: 刪除地區資料
- **路徑參數**:
  - `regionId`: 地區編號
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

#### 3.1.6 批次刪除地區
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/regions/batch`
- **說明**: 批次刪除多筆地區
- **請求格式**:
  ```json
  {
    "regionIds": ["R001", "R002", "R003"]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `RegionsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/regions")]
    [Authorize]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;
        
        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<RegionDto>>>> GetRegions([FromQuery] RegionQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{regionId}")]
        public async Task<ActionResult<ApiResponse<RegionDto>>> GetRegion(string regionId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateRegion([FromBody] CreateRegionDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{regionId}")]
        public async Task<ActionResult<ApiResponse>> UpdateRegion(string regionId, [FromBody] UpdateRegionDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{regionId}")]
        public async Task<ActionResult<ApiResponse>> DeleteRegion(string regionId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `RegionService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IRegionService
    {
        Task<PagedResult<RegionDto>> GetRegionsAsync(RegionQueryDto query);
        Task<RegionDto> GetRegionByIdAsync(string regionId);
        Task<string> CreateRegionAsync(CreateRegionDto dto);
        Task UpdateRegionAsync(string regionId, UpdateRegionDto dto);
        Task DeleteRegionAsync(string regionId);
    }
}
```

#### 3.2.3 Repository: `RegionRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IRegionRepository
    {
        Task<Region> GetByIdAsync(string regionId);
        Task<PagedResult<Region>> GetPagedAsync(RegionQuery query);
        Task<Region> CreateAsync(Region region);
        Task<Region> UpdateAsync(Region region);
        Task DeleteAsync(string regionId);
        Task<bool> ExistsAsync(string regionId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 地區列表頁面 (`RegionList.vue`)
- **路徑**: `/master-data/regions`
- **功能**: 顯示地區列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (RegionSearchForm)
  - 資料表格 (RegionDataTable)
  - 新增/修改對話框 (RegionDialog)
  - 刪除確認對話框

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`RegionSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="地區編號">
      <el-input v-model="searchForm.regionId" placeholder="請輸入地區編號" />
    </el-form-item>
    <el-form-item label="地區名稱">
      <el-input v-model="searchForm.regionName" placeholder="請輸入地區名稱" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`RegionDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="regionList" v-loading="loading">
      <el-table-column prop="regionId" label="地區編號" width="120" />
      <el-table-column prop="regionName" label="地區名稱" width="200" />
      <el-table-column prop="memo" label="備註" />
      <el-table-column prop="updatedAt" label="更新時間" width="160" />
      <el-table-column label="操作" width="150" fixed="right">
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

#### 4.2.3 新增/修改對話框 (`RegionDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="地區編號" prop="regionId">
        <el-input v-model="form.regionId" :disabled="isEdit" placeholder="請輸入地區編號" />
      </el-form-item>
      <el-form-item label="地區名稱" prop="regionName">
        <el-input v-model="form.regionName" placeholder="請輸入地區名稱" />
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

### 4.3 API 呼叫 (`region.api.ts`)
```typescript
import request from '@/utils/request';

export interface RegionDto {
  regionId: string;
  regionName: string;
  memo?: string;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
}

export interface RegionQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    regionId?: string;
    regionName?: string;
  };
}

export interface CreateRegionDto {
  regionId: string;
  regionName: string;
  memo?: string;
}

export interface UpdateRegionDto extends Omit<CreateRegionDto, 'regionId'> {}

// API 函數
export const getRegionList = (query: RegionQueryDto) => {
  return request.get<ApiResponse<PagedResult<RegionDto>>>('/api/v1/regions', { params: query });
};

export const getRegionById = (regionId: string) => {
  return request.get<ApiResponse<RegionDto>>(`/api/v1/regions/${regionId}`);
};

export const createRegion = (data: CreateRegionDto) => {
  return request.post<ApiResponse<string>>('/api/v1/regions', data);
};

export const updateRegion = (regionId: string, data: UpdateRegionDto) => {
  return request.put<ApiResponse>(`/api/v1/regions/${regionId}`, data);
};

export const deleteRegion = (regionId: string) => {
  return request.delete<ApiResponse>(`/api/v1/regions/${regionId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
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

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 6.5天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 地區編號必須唯一
- 地區名稱必填
- 地區編號格式驗證

### 6.4 業務邏輯
- 刪除地區前必須檢查是否有相關資料引用
- 地區編號不可修改

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增地區成功
- [ ] 新增地區失敗 (重複編號)
- [ ] 修改地區成功
- [ ] 修改地區失敗 (不存在)
- [ ] 刪除地區成功
- [ ] 查詢地區列表成功
- [ ] 查詢單筆地區成功

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
- `IMS3/HANSHIN/RSL_CLASS/IMS3_BASE/REGION_ID.cs`
- `WEB/IMS_CORE/SYSB000/SYSBC30_FI.aspx`
- `WEB/IMS_CORE/SYSB000/SYSBC30_FU1.aspx`
- `WEB/IMS_CORE/SYSB000/SYSBC30_FD.aspx`

### 8.2 資料庫 Schema
- 舊系統: `REGION` 表

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


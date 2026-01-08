# SYSWB60 - 庫別資料維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSWB60
- **功能名稱**: 庫別資料維護作業
- **功能描述**: 提供庫別資料的新增、修改、刪除、查詢功能，包含庫別代碼、庫別名稱、排序序號、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSW000/SYSWB60_FI1.aspx` (新增)
  - `WEB/IMS_CORE/SYSW000/SYSWB60_FU1.aspx` (修改)
  - `WEB/IMS_CORE/SYSW000/SYSWB60_FD.aspx` (刪除)
  - `WEB/IMS_CORE/SYSB000/SYSWB60_PR.rdlc` (報表)

### 1.2 業務需求
- 管理庫別基本資料資訊
- 支援庫別的新增、修改、刪除、查詢
- 記錄庫別的建立與變更資訊
- 支援庫別的啟用/停用
- 支援排序序號設定
- 與庫存管理模組整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Warehouses` (對應舊系統 `RIM_WAREHOUSE`)

```sql
CREATE TABLE [dbo].[Warehouses] (
    [WarehouseId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 庫別代碼 (WH_ID)
    [WarehouseName] NVARCHAR(100) NOT NULL, -- 庫別名稱 (WH_NAME)
    [WarehouseType] NVARCHAR(20) NULL, -- 庫別類型 (WH_TYPE)
    [Location] NVARCHAR(200) NULL, -- 庫別位置 (LOCATION)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Warehouses] PRIMARY KEY CLUSTERED ([WarehouseId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Warehouses_WarehouseName] ON [dbo].[Warehouses] ([WarehouseName]);
CREATE NONCLUSTERED INDEX [IX_Warehouses_WarehouseType] ON [dbo].[Warehouses] ([WarehouseType]);
CREATE NONCLUSTERED INDEX [IX_Warehouses_Status] ON [dbo].[Warehouses] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Warehouses_SeqNo] ON [dbo].[Warehouses] ([SeqNo]);
```

### 2.2 相關資料表

#### 2.2.1 `Inventory` - 庫存資料
- 用於查詢庫存列表
- 參考: 庫存管理相關模組

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| WarehouseId | NVARCHAR | 50 | NO | - | 庫別代碼 | 主鍵，唯一 |
| WarehouseName | NVARCHAR | 100 | NO | - | 庫別名稱 | - |
| WarehouseType | NVARCHAR | 20 | YES | - | 庫別類型 | - |
| Location | NVARCHAR | 200 | YES | - | 庫別位置 | - |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
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

#### 3.1.1 查詢庫別列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/warehouses`
- **說明**: 查詢庫別列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "WarehouseId",
    "sortOrder": "ASC",
    "filters": {
      "warehouseId": "",
      "warehouseName": "",
      "warehouseType": "",
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
          "warehouseId": "WH001",
          "warehouseName": "主倉庫",
          "warehouseType": "MAIN",
          "location": "台北市",
          "seqNo": 1,
          "status": "A",
          "notes": "備註",
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

#### 3.1.2 查詢單筆庫別
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/warehouses/{warehouseId}`
- **說明**: 根據庫別代碼查詢單筆庫別資料
- **路徑參數**:
  - `warehouseId`: 庫別代碼
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增庫別
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/warehouses`
- **說明**: 新增庫別資料
- **請求格式**:
  ```json
  {
    "warehouseId": "WH001",
    "warehouseName": "主倉庫",
    "warehouseType": "MAIN",
    "location": "台北市",
    "seqNo": 1,
    "status": "A",
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
      "warehouseId": "WH001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改庫別
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/warehouses/{warehouseId}`
- **說明**: 修改庫別資料
- **路徑參數**:
  - `warehouseId`: 庫別代碼
- **請求格式**: 同新增，但 `warehouseId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除庫別
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/warehouses/{warehouseId}`
- **說明**: 刪除庫別資料（軟刪除或硬刪除）
- **路徑參數**:
  - `warehouseId`: 庫別代碼
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

#### 3.1.6 批次刪除庫別
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/warehouses/batch`
- **說明**: 批次刪除多筆庫別
- **請求格式**:
  ```json
  {
    "warehouseIds": ["WH001", "WH002", "WH003"]
  }
  ```

#### 3.1.7 啟用/停用庫別
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/warehouses/{warehouseId}/status`
- **說明**: 啟用或停用庫別
- **請求格式**:
  ```json
  {
    "status": "A" // A:啟用, I:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `WarehousesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/warehouses")]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        
        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<WarehouseDto>>>> GetWarehouses([FromQuery] WarehouseQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{warehouseId}")]
        public async Task<ActionResult<ApiResponse<WarehouseDto>>> GetWarehouse(string warehouseId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateWarehouse([FromBody] CreateWarehouseDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{warehouseId}")]
        public async Task<ActionResult<ApiResponse>> UpdateWarehouse(string warehouseId, [FromBody] UpdateWarehouseDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{warehouseId}")]
        public async Task<ActionResult<ApiResponse>> DeleteWarehouse(string warehouseId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `WarehouseService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IWarehouseService
    {
        Task<PagedResult<WarehouseDto>> GetWarehousesAsync(WarehouseQueryDto query);
        Task<WarehouseDto> GetWarehouseByIdAsync(string warehouseId);
        Task<string> CreateWarehouseAsync(CreateWarehouseDto dto);
        Task UpdateWarehouseAsync(string warehouseId, UpdateWarehouseDto dto);
        Task DeleteWarehouseAsync(string warehouseId);
        Task UpdateStatusAsync(string warehouseId, string status);
    }
}
```

#### 3.2.3 Repository: `WarehouseRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IWarehouseRepository
    {
        Task<Warehouse> GetByIdAsync(string warehouseId);
        Task<PagedResult<Warehouse>> GetPagedAsync(WarehouseQuery query);
        Task<Warehouse> CreateAsync(Warehouse warehouse);
        Task<Warehouse> UpdateAsync(Warehouse warehouse);
        Task DeleteAsync(string warehouseId);
        Task<bool> ExistsAsync(string warehouseId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 庫別列表頁面 (`WarehouseList.vue`)
- **路徑**: `/master-data/warehouses`
- **功能**: 顯示庫別列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (WarehouseSearchForm)
  - 資料表格 (WarehouseDataTable)
  - 新增/修改對話框 (WarehouseDialog)
  - 刪除確認對話框

#### 4.1.2 庫別詳細頁面 (`WarehouseDetail.vue`)
- **路徑**: `/master-data/warehouses/:warehouseId`
- **功能**: 顯示庫別詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`WarehouseSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="庫別代碼">
      <el-input v-model="searchForm.warehouseId" placeholder="請輸入庫別代碼" />
    </el-form-item>
    <el-form-item label="庫別名稱">
      <el-input v-model="searchForm.warehouseName" placeholder="請輸入庫別名稱" />
    </el-form-item>
    <el-form-item label="庫別類型">
      <el-select v-model="searchForm.warehouseType" placeholder="請選擇庫別類型" clearable>
        <el-option label="主倉庫" value="MAIN" />
        <el-option label="分倉庫" value="BRANCH" />
        <el-option label="臨時倉庫" value="TEMPORARY" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
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

#### 4.2.2 資料表格元件 (`WarehouseDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="warehouseList" v-loading="loading">
      <el-table-column prop="warehouseId" label="庫別代碼" width="120" />
      <el-table-column prop="warehouseName" label="庫別名稱" width="200" />
      <el-table-column prop="warehouseType" label="庫別類型" width="120">
        <template #default="{ row }">
          {{ getWarehouseTypeText(row.warehouseType) }}
        </template>
      </el-table-column>
      <el-table-column prop="location" label="位置" width="200" />
      <el-table-column prop="seqNo" label="排序序號" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === 'A' ? 'success' : 'danger'">
            {{ row.status === 'A' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="備註" min-width="200" />
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

#### 4.2.3 新增/修改對話框 (`WarehouseDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="庫別代碼" prop="warehouseId">
        <el-input v-model="form.warehouseId" :disabled="isEdit" placeholder="請輸入庫別代碼" />
      </el-form-item>
      <el-form-item label="庫別名稱" prop="warehouseName">
        <el-input v-model="form.warehouseName" placeholder="請輸入庫別名稱" />
      </el-form-item>
      <el-form-item label="庫別類型" prop="warehouseType">
        <el-select v-model="form.warehouseType" placeholder="請選擇庫別類型" clearable>
          <el-option label="主倉庫" value="MAIN" />
          <el-option label="分倉庫" value="BRANCH" />
          <el-option label="臨時倉庫" value="TEMPORARY" />
        </el-select>
      </el-form-item>
      <el-form-item label="位置" prop="location">
        <el-input v-model="form.location" placeholder="請輸入位置" />
      </el-form-item>
      <el-form-item label="排序序號" prop="seqNo">
        <el-input-number v-model="form.seqNo" :min="0" placeholder="請輸入排序序號" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="A" />
          <el-option label="停用" value="I" />
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

### 4.3 API 呼叫 (`warehouse.api.ts`)
```typescript
import request from '@/utils/request';

export interface WarehouseDto {
  warehouseId: string;
  warehouseName: string;
  warehouseType?: string;
  location?: string;
  seqNo?: number;
  status: string;
  notes?: string;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
}

export interface WarehouseQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    warehouseId?: string;
    warehouseName?: string;
    warehouseType?: string;
    status?: string;
  };
}

export interface CreateWarehouseDto {
  warehouseId: string;
  warehouseName: string;
  warehouseType?: string;
  location?: string;
  seqNo?: number;
  status: string;
  notes?: string;
}

export interface UpdateWarehouseDto extends Omit<CreateWarehouseDto, 'warehouseId'> {}

// API 函數
export const getWarehouseList = (query: WarehouseQueryDto) => {
  return request.get<ApiResponse<PagedResult<WarehouseDto>>>('/api/v1/warehouses', { params: query });
};

export const getWarehouseById = (warehouseId: string) => {
  return request.get<ApiResponse<WarehouseDto>>(`/api/v1/warehouses/${warehouseId}`);
};

export const createWarehouse = (data: CreateWarehouseDto) => {
  return request.post<ApiResponse<string>>('/api/v1/warehouses', data);
};

export const updateWarehouse = (warehouseId: string, data: UpdateWarehouseDto) => {
  return request.put<ApiResponse>(`/api/v1/warehouses/${warehouseId}`, data);
};

export const deleteWarehouse = (warehouseId: string) => {
  return request.delete<ApiResponse>(`/api/v1/warehouses/${warehouseId}`);
};

export const updateStatus = (warehouseId: string, status: string) => {
  return request.put<ApiResponse>(`/api/v1/warehouses/${warehouseId}/status`, { status });
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
- 庫別代碼必須唯一
- 必填欄位必須驗證
- 狀態值必須在允許範圍內
- 庫別類型必須在允許範圍內

### 6.4 業務邏輯
- 刪除庫別前必須檢查是否有相關資料（如庫存、採購單、調撥單等）
- 停用庫別時必須檢查是否有進行中的業務

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增庫別成功
- [ ] 新增庫別失敗 (重複代碼)
- [ ] 修改庫別成功
- [ ] 修改庫別失敗 (不存在)
- [ ] 刪除庫別成功
- [ ] 查詢庫別列表成功
- [ ] 查詢單筆庫別成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 庫存關聯測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSW000/SYSWB60_FI1.aspx`
- `WEB/IMS_CORE/SYSW000/SYSWB60_FU1.aspx`
- `WEB/IMS_CORE/SYSW000/SYSWB60_FD.aspx`
- `WEB/IMS_CORE/SYSB000/SYSWB60_PR.rdlc`

### 8.2 資料庫 Schema
- 參考舊系統 `RIM_WAREHOUSE` 資料表結構

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


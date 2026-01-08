# SYSO000 - 能源管理模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSO000 系列
- **功能名稱**: 能源管理模組系列
- **功能描述**: 提供能源管理、能源查詢、能源報表等業務功能，包含能源基礎功能（SYSO100-SYSO130）、能源處理功能（SYSO310）、能源擴展功能（SYSOU10-SYSOU33）等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSO000/SYSO100_*.ASP` (能源修改)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSO120_*.ASP` (能源瀏覽、刪除、新增、查詢、保存、修改)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSO130_*.ASP` (能源新增)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSO310_*.ASP` (能源處理)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSOU10_*.ASP` (能源擴展查詢)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSOU11_*.ASP` (能源擴展維護)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSOU12_*.ASP` (能源擴展維護)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSOU15_*.ASP` (能源擴展維護)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSOU21_*.ASP` (能源自動、導入、導出、維護)
  - `WEB/IMS_CORE/ASP/UTIL/SYSO000/SYSO000_UTIL.ASP` (工具函數)

### 1.2 業務需求
- 管理能源基本資料
- 支援能源資料的新增、修改、刪除、查詢
- 支援能源資料的導入導出功能
- 支援能源自動處理功能
- 支援能源報表功能
- 支援能源位置獲取功能
- 支援能源錯誤處理功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Energy` (能源主檔)

```sql
CREATE TABLE [dbo].[Energy] (
    [EnergyId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [EnergyCode] NVARCHAR(50) NULL,
    [EnergyName] NVARCHAR(200) NOT NULL,
    [EnergyType] NVARCHAR(50) NULL,
    [Unit] NVARCHAR(20) NULL,
    [Price] DECIMAL(18, 4) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [Position] NVARCHAR(200) NULL,
    [Latitude] DECIMAL(18, 8) NULL,
    [Longitude] DECIMAL(18, 8) NULL,
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Energy] PRIMARY KEY CLUSTERED ([EnergyId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Energy_EnergyCode] ON [dbo].[Energy] ([EnergyCode]);
CREATE NONCLUSTERED INDEX [IX_Energy_EnergyType] ON [dbo].[Energy] ([EnergyType]);
CREATE NONCLUSTERED INDEX [IX_Energy_Status] ON [dbo].[Energy] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `EnergyData` - 能源資料
```sql
CREATE TABLE [dbo].[EnergyData] (
    [DataId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EnergyId] NVARCHAR(50) NOT NULL,
    [RecordDate] DATETIME2 NOT NULL,
    [Value] DECIMAL(18, 4) NOT NULL,
    [Amount] DECIMAL(18, 4) NULL,
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_EnergyData_Energy] FOREIGN KEY ([EnergyId]) REFERENCES [dbo].[Energy] ([EnergyId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EnergyData_EnergyId] ON [dbo].[EnergyData] ([EnergyId]);
CREATE NONCLUSTERED INDEX [IX_EnergyData_RecordDate] ON [dbo].[EnergyData] ([RecordDate]);
```

#### 2.2.2 `EnergyError` - 能源錯誤記錄
```sql
CREATE TABLE [dbo].[EnergyError] (
    [ErrorId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EnergyId] NVARCHAR(50) NULL,
    [ErrorType] NVARCHAR(50) NULL,
    [ErrorMessage] NVARCHAR(500) NULL,
    [ErrorDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- P:待處理, R:已處理
    [ResolvedBy] NVARCHAR(50) NULL,
    [ResolvedAt] DATETIME2 NULL,
    [Notes] NVARCHAR(500) NULL,
    CONSTRAINT [FK_EnergyError_Energy] FOREIGN KEY ([EnergyId]) REFERENCES [dbo].[Energy] ([EnergyId]) ON DELETE SET NULL
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EnergyError_EnergyId] ON [dbo].[EnergyError] ([EnergyId]);
CREATE NONCLUSTERED INDEX [IX_EnergyError_Status] ON [dbo].[EnergyError] ([Status]);
CREATE NONCLUSTERED INDEX [IX_EnergyError_ErrorDate] ON [dbo].[EnergyError] ([ErrorDate]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| EnergyId | NVARCHAR | 50 | NO | - | 能源編號 | 主鍵，唯一 |
| EnergyCode | NVARCHAR | 50 | YES | - | 能源代碼 | - |
| EnergyName | NVARCHAR | 200 | NO | - | 能源名稱 | - |
| EnergyType | NVARCHAR | 50 | YES | - | 能源類型 | - |
| Unit | NVARCHAR | 20 | YES | - | 單位 | - |
| Price | DECIMAL | 18,4 | YES | - | 價格 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| Position | NVARCHAR | 200 | YES | - | 位置 | - |
| Latitude | DECIMAL | 18,8 | YES | - | 緯度 | - |
| Longitude | DECIMAL | 18,8 | YES | - | 經度 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢能源列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/energy`
- **說明**: 查詢能源列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "EnergyId",
    "sortOrder": "ASC",
    "filters": {
      "energyId": "",
      "energyCode": "",
      "energyName": "",
      "energyType": "",
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
          "energyId": "E001",
          "energyCode": "ELEC",
          "energyName": "電力",
          "energyType": "ELECTRICITY",
          "unit": "kWh",
          "price": 3.5,
          "status": "A",
          "position": "台北市",
          "latitude": 25.0330,
          "longitude": 121.5654
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

#### 3.1.2 查詢單筆能源
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/energy/{energyId}`
- **說明**: 根據能源編號查詢單筆能源資料
- **路徑參數**:
  - `energyId`: 能源編號
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增能源
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/energy`
- **說明**: 新增能源資料
- **請求格式**:
  ```json
  {
    "energyId": "E001",
    "energyCode": "ELEC",
    "energyName": "電力",
    "energyType": "ELECTRICITY",
    "unit": "kWh",
    "price": 3.5,
    "status": "A",
    "position": "台北市",
    "latitude": 25.0330,
    "longitude": 121.5654,
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
      "energyId": "E001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改能源
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/energy/{energyId}`
- **說明**: 修改能源資料
- **路徑參數**:
  - `energyId`: 能源編號
- **請求格式**: 同新增，但 `energyId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除能源
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/energy/{energyId}`
- **說明**: 刪除能源資料（軟刪除或硬刪除）
- **路徑參數**:
  - `energyId`: 能源編號
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

#### 3.1.6 批次刪除能源
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/energy/batch`
- **說明**: 批次刪除多筆能源
- **請求格式**:
  ```json
  {
    "energyIds": ["E001", "E002", "E003"]
  }
  ```

#### 3.1.7 批次新增能源
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/energy/batch`
- **說明**: 批次新增多筆能源
- **請求格式**:
  ```json
  {
    "items": [
      {
        "energyId": "E001",
        "energyCode": "ELEC",
        "energyName": "電力",
        ...
      },
      {
        "energyId": "E002",
        "energyCode": "GAS",
        "energyName": "天然氣",
        ...
      }
    ]
  }
  ```

#### 3.1.8 能源資料導入
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/energy/import`
- **說明**: 從Excel檔案導入能源資料
- **請求格式**: `multipart/form-data`
  - `file`: Excel檔案
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "導入成功",
    "data": {
      "totalCount": 100,
      "successCount": 95,
      "failCount": 5,
      "errors": [
        {
          "row": 10,
          "message": "能源編號重複"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.9 能源資料導出
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/energy/export`
- **說明**: 導出能源資料為Excel檔案
- **請求參數**: 同查詢列表
- **回應格式**: Excel檔案下載

#### 3.1.10 能源自動處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/energy/auto-process`
- **說明**: 自動處理能源資料
- **請求格式**:
  ```json
  {
    "energyId": "E001",
    "processType": "CALCULATE",
    "parameters": {}
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "處理成功",
    "data": {
      "processId": "PROC001",
      "status": "COMPLETED"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.11 獲取位置資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/energy/position`
- **說明**: 根據座標獲取位置資訊
- **請求參數**:
  - `latitude`: 緯度
  - `longitude`: 經度
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "position": "台北市信義區",
      "address": "台北市信義區信義路五段7號"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.12 查詢能源資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/energy/{energyId}/data`
- **說明**: 查詢能源的資料記錄
- **路徑參數**:
  - `energyId`: 能源編號
- **請求參數**:
  - `startDate`: 開始日期
  - `endDate`: 結束日期
  - `pageIndex`: 頁碼
  - `pageSize`: 每頁筆數
- **回應格式**: 同查詢列表

#### 3.1.13 新增能源資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/energy/{energyId}/data`
- **說明**: 新增能源資料記錄
- **請求格式**:
  ```json
  {
    "recordDate": "2024-01-01",
    "value": 100.5,
    "amount": 351.75,
    "notes": "備註"
  }
  ```

#### 3.1.14 查詢能源錯誤
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/energy/errors`
- **說明**: 查詢能源錯誤記錄
- **請求參數**:
  - `energyId`: 能源編號（選填）
  - `status`: 狀態（選填）
  - `startDate`: 開始日期（選填）
  - `endDate`: 結束日期（選填）
  - `pageIndex`: 頁碼
  - `pageSize`: 每頁筆數
- **回應格式**: 同查詢列表

#### 3.1.15 處理能源錯誤
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/energy/errors/{errorId}/resolve`
- **說明**: 處理能源錯誤記錄
- **路徑參數**:
  - `errorId`: 錯誤編號
- **請求格式**:
  ```json
  {
    "notes": "處理備註"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `EnergyController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/energy")]
    [Authorize]
    public class EnergyController : ControllerBase
    {
        private readonly IEnergyService _energyService;
        
        public EnergyController(IEnergyService energyService)
        {
            _energyService = energyService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<EnergyDto>>>> GetEnergies([FromQuery] EnergyQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{energyId}")]
        public async Task<ActionResult<ApiResponse<EnergyDto>>> GetEnergy(string energyId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateEnergy([FromBody] CreateEnergyDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{energyId}")]
        public async Task<ActionResult<ApiResponse>> UpdateEnergy(string energyId, [FromBody] UpdateEnergyDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{energyId}")]
        public async Task<ActionResult<ApiResponse>> DeleteEnergy(string energyId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse>> BatchCreateEnergy([FromBody] BatchCreateEnergyDto dto)
        {
            // 實作批次新增邏輯
        }
        
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportEnergy([FromForm] IFormFile file)
        {
            // 實作導入邏輯
        }
        
        [HttpGet("export")]
        public async Task<IActionResult> ExportEnergy([FromQuery] EnergyQueryDto query)
        {
            // 實作導出邏輯
        }
        
        [HttpPost("auto-process")]
        public async Task<ActionResult<ApiResponse<ProcessResultDto>>> AutoProcessEnergy([FromBody] AutoProcessEnergyDto dto)
        {
            // 實作自動處理邏輯
        }
        
        [HttpGet("position")]
        public async Task<ActionResult<ApiResponse<PositionDto>>> GetPosition([FromQuery] double latitude, [FromQuery] double longitude)
        {
            // 實作位置查詢邏輯
        }
    }
}
```

#### 3.2.2 Service: `EnergyService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IEnergyService
    {
        Task<PagedResult<EnergyDto>> GetEnergiesAsync(EnergyQueryDto query);
        Task<EnergyDto> GetEnergyByIdAsync(string energyId);
        Task<string> CreateEnergyAsync(CreateEnergyDto dto);
        Task UpdateEnergyAsync(string energyId, UpdateEnergyDto dto);
        Task DeleteEnergyAsync(string energyId);
        Task BatchCreateEnergyAsync(BatchCreateEnergyDto dto);
        Task<ImportResultDto> ImportEnergyAsync(Stream fileStream, string fileName);
        Task<Stream> ExportEnergyAsync(EnergyQueryDto query);
        Task<ProcessResultDto> AutoProcessEnergyAsync(AutoProcessEnergyDto dto);
        Task<PositionDto> GetPositionAsync(double latitude, double longitude);
        Task<PagedResult<EnergyDataDto>> GetEnergyDataAsync(string energyId, EnergyDataQueryDto query);
        Task CreateEnergyDataAsync(string energyId, CreateEnergyDataDto dto);
        Task<PagedResult<EnergyErrorDto>> GetEnergyErrorsAsync(EnergyErrorQueryDto query);
        Task ResolveEnergyErrorAsync(Guid errorId, ResolveEnergyErrorDto dto);
    }
}
```

#### 3.2.3 Repository: `EnergyRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IEnergyRepository
    {
        Task<Energy> GetByIdAsync(string energyId);
        Task<PagedResult<Energy>> GetPagedAsync(EnergyQuery query);
        Task<Energy> CreateAsync(Energy energy);
        Task<Energy> UpdateAsync(Energy energy);
        Task DeleteAsync(string energyId);
        Task<bool> ExistsAsync(string energyId);
        Task<List<Energy>> CreateBatchAsync(List<Energy> energies);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 能源列表頁面 (`EnergyList.vue`)
- **路徑**: `/energy/list`
- **功能**: 顯示能源列表，支援查詢、新增、修改、刪除、導入、導出
- **主要元件**:
  - 查詢表單 (EnergySearchForm)
  - 資料表格 (EnergyDataTable)
  - 新增/修改對話框 (EnergyDialog)
  - 批次新增對話框 (EnergyBatchDialog)
  - 導入對話框 (EnergyImportDialog)
  - 刪除確認對話框

#### 4.1.2 能源詳細頁面 (`EnergyDetail.vue`)
- **路徑**: `/energy/:energyId`
- **功能**: 顯示能源詳細資料，支援修改、查看資料記錄

#### 4.1.3 能源資料頁面 (`EnergyData.vue`)
- **路徑**: `/energy/:energyId/data`
- **功能**: 顯示能源資料記錄，支援新增、查詢

#### 4.1.4 能源錯誤頁面 (`EnergyError.vue`)
- **路徑**: `/energy/errors`
- **功能**: 顯示能源錯誤記錄，支援查詢、處理

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`EnergySearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="能源編號">
      <el-input v-model="searchForm.energyId" placeholder="請輸入能源編號" />
    </el-form-item>
    <el-form-item label="能源代碼">
      <el-input v-model="searchForm.energyCode" placeholder="請輸入能源代碼" />
    </el-form-item>
    <el-form-item label="能源名稱">
      <el-input v-model="searchForm.energyName" placeholder="請輸入能源名稱" />
    </el-form-item>
    <el-form-item label="能源類型">
      <el-select v-model="searchForm.energyType" placeholder="請選擇能源類型">
        <el-option label="電力" value="ELECTRICITY" />
        <el-option label="天然氣" value="GAS" />
        <el-option label="水" value="WATER" />
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

#### 4.2.2 資料表格元件 (`EnergyDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="energyList" v-loading="loading">
      <el-table-column prop="energyId" label="能源編號" width="120" />
      <el-table-column prop="energyCode" label="能源代碼" width="120" />
      <el-table-column prop="energyName" label="能源名稱" width="200" />
      <el-table-column prop="energyType" label="能源類型" width="120" />
      <el-table-column prop="unit" label="單位" width="80" />
      <el-table-column prop="price" label="價格" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="position" label="位置" width="200" />
      <el-table-column label="操作" width="300" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="info" size="small" @click="handleViewData(row)">資料</el-button>
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

#### 4.2.3 新增/修改對話框 (`EnergyDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="能源編號" prop="energyId">
        <el-input v-model="form.energyId" :disabled="isEdit" placeholder="請輸入能源編號" />
      </el-form-item>
      <el-form-item label="能源代碼" prop="energyCode">
        <el-input v-model="form.energyCode" placeholder="請輸入能源代碼" />
      </el-form-item>
      <el-form-item label="能源名稱" prop="energyName">
        <el-input v-model="form.energyName" placeholder="請輸入能源名稱" />
      </el-form-item>
      <el-form-item label="能源類型" prop="energyType">
        <el-select v-model="form.energyType" placeholder="請選擇能源類型">
          <el-option label="電力" value="ELECTRICITY" />
          <el-option label="天然氣" value="GAS" />
          <el-option label="水" value="WATER" />
        </el-select>
      </el-form-item>
      <el-form-item label="單位" prop="unit">
        <el-input v-model="form.unit" placeholder="請輸入單位" />
      </el-form-item>
      <el-form-item label="價格" prop="price">
        <el-input-number v-model="form.price" :precision="4" :min="0" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="A" />
          <el-option label="停用" value="I" />
        </el-select>
      </el-form-item>
      <el-form-item label="位置" prop="position">
        <el-input v-model="form.position" placeholder="請輸入位置" />
      </el-form-item>
      <el-form-item label="緯度" prop="latitude">
        <el-input-number v-model="form.latitude" :precision="8" />
      </el-form-item>
      <el-form-item label="經度" prop="longitude">
        <el-input-number v-model="form.longitude" :precision="8" />
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

### 4.3 API 呼叫 (`energy.api.ts`)
```typescript
import request from '@/utils/request';

export interface EnergyDto {
  energyId: string;
  energyCode?: string;
  energyName: string;
  energyType?: string;
  unit?: string;
  price?: number;
  status: string;
  position?: string;
  latitude?: number;
  longitude?: number;
  notes?: string;
}

export interface EnergyQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    energyId?: string;
    energyCode?: string;
    energyName?: string;
    energyType?: string;
    status?: string;
  };
}

export interface CreateEnergyDto {
  energyId: string;
  energyCode?: string;
  energyName: string;
  energyType?: string;
  unit?: string;
  price?: number;
  status: string;
  position?: string;
  latitude?: number;
  longitude?: number;
  notes?: string;
}

export interface UpdateEnergyDto extends Omit<CreateEnergyDto, 'energyId'> {}

// API 函數
export const getEnergyList = (query: EnergyQueryDto) => {
  return request.get<ApiResponse<PagedResult<EnergyDto>>>('/api/v1/energy', { params: query });
};

export const getEnergyById = (energyId: string) => {
  return request.get<ApiResponse<EnergyDto>>(`/api/v1/energy/${energyId}`);
};

export const createEnergy = (data: CreateEnergyDto) => {
  return request.post<ApiResponse<string>>('/api/v1/energy', data);
};

export const updateEnergy = (energyId: string, data: UpdateEnergyDto) => {
  return request.put<ApiResponse>(`/api/v1/energy/${energyId}`, data);
};

export const deleteEnergy = (energyId: string) => {
  return request.delete<ApiResponse>(`/api/v1/energy/${energyId}`);
};

export const batchCreateEnergy = (data: { items: CreateEnergyDto[] }) => {
  return request.post<ApiResponse>('/api/v1/energy/batch', data);
};

export const importEnergy = (file: File) => {
  const formData = new FormData();
  formData.append('file', file);
  return request.post<ApiResponse<ImportResultDto>>('/api/v1/energy/import', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
};

export const exportEnergy = (query: EnergyQueryDto) => {
  return request.get('/api/v1/energy/export', { params: query, responseType: 'blob' });
};

export const autoProcessEnergy = (data: AutoProcessEnergyDto) => {
  return request.post<ApiResponse<ProcessResultDto>>('/api/v1/energy/auto-process', data);
};

export const getPosition = (latitude: number, longitude: number) => {
  return request.get<ApiResponse<PositionDto>>('/api/v1/energy/position', {
    params: { latitude, longitude }
  });
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

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 導入導出功能實作
- [ ] 自動處理功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 批次新增對話框開發
- [ ] 導入對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 能源資料頁面開發
- [ ] 能源錯誤頁面開發
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

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 導入檔案必須驗證格式和大小

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制
- 導入導出功能必須支援非同步處理

### 6.3 資料驗證
- 能源編號必須唯一
- 必填欄位必須驗證
- 座標範圍必須驗證
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 刪除能源前必須檢查是否有相關資料
- 停用能源時必須檢查是否有進行中的業務
- 導入資料必須驗證格式和資料完整性
- 自動處理功能必須支援錯誤處理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增能源成功
- [ ] 新增能源失敗 (重複編號)
- [ ] 修改能源成功
- [ ] 修改能源失敗 (不存在)
- [ ] 刪除能源成功
- [ ] 查詢能源列表成功
- [ ] 查詢單筆能源成功
- [ ] 批次新增能源成功
- [ ] 導入能源資料成功
- [ ] 導出能源資料成功
- [ ] 自動處理能源成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 導入導出流程測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 導入大量資料測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSO000/SYSO100_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSO120_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSO130_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSO310_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSOU10_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSOU11_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSOU12_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSOU15_*.ASP`
- `WEB/IMS_CORE/ASP/SYSO000/SYSOU21_*.ASP`
- `WEB/IMS_CORE/ASP/UTIL/SYSO000/SYSO000_UTIL.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/SYSO000/SYSO000.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


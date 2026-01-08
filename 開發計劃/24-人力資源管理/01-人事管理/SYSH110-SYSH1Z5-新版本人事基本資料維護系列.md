# SYSH110-SYSH1Z5 - 新版本人事基本資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSH110-SYSH1Z5系列
- **功能名稱**: 新版本人事基本資料維護系列
- **功能描述**: 提供新版本人事基本資料的新增、修改、刪除、查詢功能，包含完整的員工資料管理、組織架構、職位管理、專櫃人員管理等擴展功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH110_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FI.ASP` (專櫃人員新增)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FU.ASP` (專櫃人員修改)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FD.ASP` (專櫃人員刪除)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FQ.ASP` (專櫃人員查詢)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FS.ASP` (專櫃人員排序)

### 1.2 業務需求
- 管理員工基本資料資訊（SYSH110系列）
- 支援員工到職/離職管理
- 記錄員工異動資訊
- 支援員工組織架構設定
- 支援員工職位管理
- 支援多部門架構
- 支援專櫃人員管理（SYSH1Z5）
- 支援批量新增、批量修改功能
- 支援多種查詢條件
- 支援報表列印功能

### 1.3 功能範圍
本系列包含以下子功能：
- **SYSH110-SYSH192**: 基礎人事管理功能
- **SYSH1A0-SYSH1Z5**: 擴展人事功能（包含專櫃人員管理、薪資管理、考勤管理等）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Employees`

```sql
CREATE TABLE [dbo].[Employees] (
    [EmployeeId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [EmployeeName] NVARCHAR(100) NOT NULL,
    [IdNumber] NVARCHAR(20) NULL,
    [DepartmentId] NVARCHAR(50) NULL,
    [PositionId] NVARCHAR(50) NULL,
    [HireDate] DATETIME2 NULL,
    [ResignDate] DATETIME2 NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:在職, I:離職, L:留停
    [Email] NVARCHAR(100) NULL,
    [Phone] NVARCHAR(20) NULL,
    [Address] NVARCHAR(500) NULL,
    [BirthDate] DATETIME2 NULL,
    [Gender] NVARCHAR(10) NULL,
    [Notes] NVARCHAR(500) NULL,
    [EmployeeType] NVARCHAR(20) NULL, -- INTERNAL:內部, COUNTER:專櫃
    [ShopId] NVARCHAR(50) NULL, -- 專櫃代碼
    [CounterId] NVARCHAR(50) NULL, -- 專櫃編號
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees] ([DepartmentId]);
CREATE NONCLUSTERED INDEX [IX_Employees_Status] ON [dbo].[Employees] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Employees_PositionId] ON [dbo].[Employees] ([PositionId]);
CREATE NONCLUSTERED INDEX [IX_Employees_EmployeeType] ON [dbo].[Employees] ([EmployeeType]);
CREATE NONCLUSTERED INDEX [IX_Employees_ShopId] ON [dbo].[Employees] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_Employees_CounterId] ON [dbo].[Employees] ([CounterId]);
```

### 2.2 相關資料表

#### 2.2.1 `EmployeeExtensions` - 員工擴展資料
```sql
CREATE TABLE [dbo].[EmployeeExtensions] (
    [ExtensionId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EmployeeId] NVARCHAR(50) NOT NULL,
    [ExtensionType] NVARCHAR(50) NOT NULL, -- 擴展類型
    [ExtensionKey] NVARCHAR(100) NULL,
    [ExtensionValue] NVARCHAR(MAX) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_EmployeeExtensions_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_EmployeeExtensions_EmployeeId] ON [dbo].[EmployeeExtensions] ([EmployeeId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeExtensions_ExtensionType] ON [dbo].[EmployeeExtensions] ([ExtensionType]);
```

#### 2.2.2 `CounterEmployees` - 專櫃人員對應
```sql
CREATE TABLE [dbo].[CounterEmployees] (
    [CounterEmployeeId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [CounterId] NVARCHAR(50) NOT NULL,
    [EmployeeId] NVARCHAR(50) NOT NULL,
    [StartDate] DATETIME2 NULL,
    [EndDate] DATETIME2 NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_CounterEmployees_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_CounterEmployees_CounterId] ON [dbo].[CounterEmployees] ([CounterId]);
CREATE NONCLUSTERED INDEX [IX_CounterEmployees_EmployeeId] ON [dbo].[CounterEmployees] ([EmployeeId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| EmployeeId | NVARCHAR | 50 | NO | - | 員工編號 | 主鍵，唯一 |
| EmployeeName | NVARCHAR | 100 | NO | - | 員工姓名 | - |
| IdNumber | NVARCHAR | 20 | YES | - | 身分證字號 | - |
| DepartmentId | NVARCHAR | 50 | YES | - | 部門代碼 | 外鍵至部門表 |
| PositionId | NVARCHAR | 50 | YES | - | 職位代碼 | 外鍵至職位表 |
| HireDate | DATETIME2 | - | YES | - | 到職日期 | - |
| ResignDate | DATETIME2 | - | YES | - | 離職日期 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 員工狀態 | A:在職, I:離職, L:留停 |
| Email | NVARCHAR | 100 | YES | - | 電子郵件 | - |
| Phone | NVARCHAR | 20 | YES | - | 電話 | - |
| Address | NVARCHAR | 500 | YES | - | 地址 | - |
| BirthDate | DATETIME2 | - | YES | - | 出生日期 | - |
| Gender | NVARCHAR | 10 | YES | - | 性別 | M:男, F:女 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| EmployeeType | NVARCHAR | 20 | YES | - | 員工類型 | INTERNAL:內部, COUNTER:專櫃 |
| ShopId | NVARCHAR | 50 | YES | - | 專櫃代碼 | - |
| CounterId | NVARCHAR | 50 | YES | - | 專櫃編號 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢員工列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employees`
- **說明**: 查詢員工列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "EmployeeId",
    "sortOrder": "ASC",
    "filters": {
      "employeeId": "",
      "employeeName": "",
      "departmentId": "",
      "status": "",
      "employeeType": "",
      "shopId": "",
      "counterId": ""
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
          "employeeId": "E001",
          "employeeName": "張三",
          "departmentId": "DEPT001",
          "positionId": "POS001",
          "status": "A",
          "hireDate": "2024-01-01",
          "employeeType": "INTERNAL"
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

#### 3.1.2 查詢單筆員工
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employees/{employeeId}`
- **說明**: 根據員工編號查詢單筆員工資料
- **路徑參數**:
  - `employeeId`: 員工編號
- **回應格式**: 同查詢列表的單筆資料格式，包含擴展資料

#### 3.1.3 新增員工
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/employees`
- **說明**: 新增員工資料
- **請求格式**:
  ```json
  {
    "employeeId": "E001",
    "employeeName": "張三",
    "idNumber": "A123456789",
    "departmentId": "DEPT001",
    "positionId": "POS001",
    "hireDate": "2024-01-01",
    "status": "A",
    "email": "zhang@example.com",
    "phone": "0912345678",
    "employeeType": "INTERNAL",
    "shopId": null,
    "counterId": null
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "employeeId": "E001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改員工
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/employees/{employeeId}`
- **說明**: 修改員工資料
- **路徑參數**:
  - `employeeId`: 員工編號
- **請求格式**: 同新增，但 `employeeId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除員工
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/employees/{employeeId}`
- **說明**: 刪除員工資料（軟刪除或硬刪除）
- **路徑參數**:
  - `employeeId`: 員工編號
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

#### 3.1.6 批次新增員工
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/employees/batch`
- **說明**: 批次新增多筆員工資料
- **請求格式**:
  ```json
  {
    "items": [
      {
        "employeeId": "E001",
        "employeeName": "張三",
        ...
      },
      {
        "employeeId": "E002",
        "employeeName": "李四",
        ...
      }
    ]
  }
  ```

#### 3.1.7 批次修改員工
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/employees/batch`
- **說明**: 批次修改多筆員工資料
- **請求格式**: 同批次新增

#### 3.1.8 查詢專櫃人員
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employees/counters`
- **說明**: 查詢專櫃人員列表
- **請求參數**: 同查詢員工列表，但 `employeeType` 固定為 `COUNTER`

#### 3.1.9 新增專櫃人員
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/employees/counters`
- **說明**: 新增專櫃人員資料
- **請求格式**: 同新增員工，但 `employeeType` 固定為 `COUNTER`，且必須提供 `shopId` 和 `counterId`

### 3.2 後端實作類別

#### 3.2.1 Controller: `EmployeesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/employees")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<EmployeeDto>>>> GetEmployees([FromQuery] EmployeeQueryDto query)
        {
            var result = await _employeeService.GetEmployeesAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(string employeeId)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(employeeId);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var employeeId = await _employeeService.CreateEmployeeAsync(dto);
            return Ok(ApiResponse.Success(employeeId, "新增成功"));
        }
        
        [HttpPut("{employeeId}")]
        public async Task<ActionResult<ApiResponse>> UpdateEmployee(string employeeId, [FromBody] UpdateEmployeeDto dto)
        {
            await _employeeService.UpdateEmployeeAsync(employeeId, dto);
            return Ok(ApiResponse.Success(null, "修改成功"));
        }
        
        [HttpDelete("{employeeId}")]
        public async Task<ActionResult<ApiResponse>> DeleteEmployee(string employeeId)
        {
            await _employeeService.DeleteEmployeeAsync(employeeId);
            return Ok(ApiResponse.Success(null, "刪除成功"));
        }
        
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse>> BatchCreateEmployees([FromBody] BatchCreateEmployeeDto dto)
        {
            await _employeeService.BatchCreateEmployeesAsync(dto);
            return Ok(ApiResponse.Success(null, "批次新增成功"));
        }
        
        [HttpPut("batch")]
        public async Task<ActionResult<ApiResponse>> BatchUpdateEmployees([FromBody] BatchUpdateEmployeeDto dto)
        {
            await _employeeService.BatchUpdateEmployeesAsync(dto);
            return Ok(ApiResponse.Success(null, "批次修改成功"));
        }
        
        [HttpGet("counters")]
        public async Task<ActionResult<ApiResponse<PagedResult<EmployeeDto>>>> GetCounterEmployees([FromQuery] EmployeeQueryDto query)
        {
            query.Filters.EmployeeType = "COUNTER";
            var result = await _employeeService.GetEmployeesAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("counters")]
        public async Task<ActionResult<ApiResponse<string>>> CreateCounterEmployee([FromBody] CreateEmployeeDto dto)
        {
            dto.EmployeeType = "COUNTER";
            var employeeId = await _employeeService.CreateEmployeeAsync(dto);
            return Ok(ApiResponse.Success(employeeId, "新增專櫃人員成功"));
        }
    }
}
```

#### 3.2.2 Service: `EmployeeService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IEmployeeService
    {
        Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryDto query);
        Task<EmployeeDto> GetEmployeeByIdAsync(string employeeId);
        Task<string> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task UpdateEmployeeAsync(string employeeId, UpdateEmployeeDto dto);
        Task DeleteEmployeeAsync(string employeeId);
        Task BatchCreateEmployeesAsync(BatchCreateEmployeeDto dto);
        Task BatchUpdateEmployeesAsync(BatchUpdateEmployeeDto dto);
    }
}
```

#### 3.2.3 Repository: `EmployeeRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(string employeeId);
        Task<PagedResult<Employee>> GetPagedAsync(EmployeeQuery query);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task DeleteAsync(string employeeId);
        Task<bool> ExistsAsync(string employeeId);
        Task<List<Employee>> BatchCreateAsync(List<Employee> employees);
        Task<List<Employee>> BatchUpdateAsync(List<Employee> employees);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 員工列表頁面 (`EmployeeList.vue`)
- **路徑**: `/hr/employees`
- **功能**: 顯示員工列表，支援查詢、新增、修改、刪除、批次操作
- **主要元件**:
  - 查詢表單 (EmployeeSearchForm)
  - 資料表格 (EmployeeDataTable)
  - 新增/修改對話框 (EmployeeDialog)
  - 批次操作對話框 (BatchEmployeeDialog)
  - 刪除確認對話框

#### 4.1.2 專櫃人員管理頁面 (`CounterEmployeeList.vue`)
- **路徑**: `/hr/employees/counters`
- **功能**: 顯示專櫃人員列表，支援查詢、新增、修改、刪除
- **主要元件**: 同員工列表頁面

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`EmployeeSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="員工編號">
      <el-input v-model="searchForm.employeeId" placeholder="請輸入員工編號" />
    </el-form-item>
    <el-form-item label="員工姓名">
      <el-input v-model="searchForm.employeeName" placeholder="請輸入員工姓名" />
    </el-form-item>
    <el-form-item label="部門">
      <el-select v-model="searchForm.departmentId" placeholder="請選擇部門">
        <el-option v-for="dept in departmentList" :key="dept.departmentId" :label="dept.departmentName" :value="dept.departmentId" />
      </el-select>
    </el-form-item>
    <el-form-item label="員工類型">
      <el-select v-model="searchForm.employeeType" placeholder="請選擇員工類型">
        <el-option label="全部" value="" />
        <el-option label="內部員工" value="INTERNAL" />
        <el-option label="專櫃人員" value="COUNTER" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="在職" value="A" />
        <el-option label="離職" value="I" />
        <el-option label="留停" value="L" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`EmployeeDataTable.vue`)
```vue
<template>
  <div>
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">新增</el-button>
      <el-button type="success" @click="handleBatchAdd">批次新增</el-button>
      <el-button type="warning" @click="handleBatchEdit">批次修改</el-button>
      <el-button type="danger" @click="handleBatchDelete" :disabled="selectedRows.length === 0">批次刪除</el-button>
    </div>
    <el-table :data="employeeList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="employeeId" label="員工編號" width="120" />
      <el-table-column prop="employeeName" label="員工姓名" width="150" />
      <el-table-column prop="departmentName" label="部門" width="150" />
      <el-table-column prop="positionName" label="職位" width="100" />
      <el-table-column prop="employeeType" label="員工類型" width="100">
        <template #default="{ row }">
          <el-tag :type="row.employeeType === 'INTERNAL' ? 'primary' : 'success'">
            {{ row.employeeType === 'INTERNAL' ? '內部員工' : '專櫃人員' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="hireDate" label="到職日期" width="120" />
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

#### 4.2.3 新增/修改對話框 (`EmployeeDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="員工編號" prop="employeeId">
        <el-input v-model="form.employeeId" :disabled="isEdit" placeholder="請輸入員工編號" />
      </el-form-item>
      <el-form-item label="員工姓名" prop="employeeName">
        <el-input v-model="form.employeeName" placeholder="請輸入員工姓名" />
      </el-form-item>
      <el-form-item label="員工類型" prop="employeeType">
        <el-select v-model="form.employeeType" placeholder="請選擇員工類型">
          <el-option label="內部員工" value="INTERNAL" />
          <el-option label="專櫃人員" value="COUNTER" />
        </el-select>
      </el-form-item>
      <el-form-item label="專櫃代碼" prop="shopId" v-if="form.employeeType === 'COUNTER'">
        <el-select v-model="form.shopId" placeholder="請選擇專櫃代碼">
          <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
        </el-select>
      </el-form-item>
      <el-form-item label="專櫃編號" prop="counterId" v-if="form.employeeType === 'COUNTER'">
        <el-input v-model="form.counterId" placeholder="請輸入專櫃編號" />
      </el-form-item>
      <el-form-item label="部門" prop="departmentId">
        <el-select v-model="form.departmentId" placeholder="請選擇部門">
          <el-option v-for="dept in departmentList" :key="dept.departmentId" :label="dept.departmentName" :value="dept.departmentId" />
        </el-select>
      </el-form-item>
      <el-form-item label="職位" prop="positionId">
        <el-select v-model="form.positionId" placeholder="請選擇職位">
          <el-option v-for="pos in positionList" :key="pos.positionId" :label="pos.positionName" :value="pos.positionId" />
        </el-select>
      </el-form-item>
      <el-form-item label="到職日期" prop="hireDate">
        <el-date-picker v-model="form.hireDate" type="date" placeholder="請選擇日期" />
      </el-form-item>
      <el-form-item label="離職日期" prop="resignDate">
        <el-date-picker v-model="form.resignDate" type="date" placeholder="請選擇日期" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="在職" value="A" />
          <el-option label="離職" value="I" />
          <el-option label="留停" value="L" />
        </el-select>
      </el-form-item>
      <el-form-item label="電子郵件" prop="email">
        <el-input v-model="form.email" placeholder="請輸入電子郵件" />
      </el-form-item>
      <el-form-item label="電話" prop="phone">
        <el-input v-model="form.phone" placeholder="請輸入電話" />
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

### 4.3 API 呼叫 (`employee.api.ts`)
```typescript
import request from '@/utils/request';

export interface EmployeeDto {
  employeeId: string;
  employeeName: string;
  idNumber?: string;
  departmentId?: string;
  positionId?: string;
  hireDate?: string;
  resignDate?: string;
  status: string;
  email?: string;
  phone?: string;
  address?: string;
  birthDate?: string;
  gender?: string;
  notes?: string;
  employeeType?: string;
  shopId?: string;
  counterId?: string;
}

export interface EmployeeQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    employeeId?: string;
    employeeName?: string;
    departmentId?: string;
    status?: string;
    employeeType?: string;
    shopId?: string;
    counterId?: string;
  };
}

export interface CreateEmployeeDto {
  employeeId: string;
  employeeName: string;
  idNumber?: string;
  departmentId?: string;
  positionId?: string;
  hireDate?: string;
  resignDate?: string;
  status: string;
  email?: string;
  phone?: string;
  address?: string;
  birthDate?: string;
  gender?: string;
  notes?: string;
  employeeType?: string;
  shopId?: string;
  counterId?: string;
}

export interface UpdateEmployeeDto extends Omit<CreateEmployeeDto, 'employeeId'> {}

export interface BatchCreateEmployeeDto {
  items: CreateEmployeeDto[];
}

export interface BatchUpdateEmployeeDto {
  items: UpdateEmployeeDto[];
}

// API 函數
export const getEmployeeList = (query: EmployeeQueryDto) => {
  return request.get<ApiResponse<PagedResult<EmployeeDto>>>('/api/v1/employees', { params: query });
};

export const getEmployeeById = (employeeId: string) => {
  return request.get<ApiResponse<EmployeeDto>>(`/api/v1/employees/${employeeId}`);
};

export const createEmployee = (data: CreateEmployeeDto) => {
  return request.post<ApiResponse<string>>('/api/v1/employees', data);
};

export const updateEmployee = (employeeId: string, data: UpdateEmployeeDto) => {
  return request.put<ApiResponse>(`/api/v1/employees/${employeeId}`, data);
};

export const deleteEmployee = (employeeId: string) => {
  return request.delete<ApiResponse>(`/api/v1/employees/${employeeId}`);
};

export const batchCreateEmployees = (data: BatchCreateEmployeeDto) => {
  return request.post<ApiResponse>('/api/v1/employees/batch', data);
};

export const batchUpdateEmployees = (data: BatchUpdateEmployeeDto) => {
  return request.put<ApiResponse>('/api/v1/employees/batch', data);
};

export const getCounterEmployeeList = (query: EmployeeQueryDto) => {
  return request.get<ApiResponse<PagedResult<EmployeeDto>>>('/api/v1/employees/counters', { params: query });
};

export const createCounterEmployee = (data: CreateEmployeeDto) => {
  return request.post<ApiResponse<string>>('/api/v1/employees/counters', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本
- [ ] 擴展資料表設計

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 批次處理邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 批次操作對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 專櫃人員管理頁面開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 批次操作測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 15天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 個人資料保護法規遵循
- 批次操作必須記錄操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制
- 批次操作必須使用事務處理

### 6.3 資料驗證
- 員工編號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- 專櫃人員必須提供專櫃代碼和編號

### 6.4 業務邏輯
- 刪除員工前必須檢查是否有相關資料
- 離職員工時必須檢查是否有進行中的業務
- 員工異動資訊必須記錄
- 批次操作必須提供錯誤報告

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增員工成功
- [ ] 新增員工失敗 (重複編號)
- [ ] 修改員工成功
- [ ] 修改員工失敗 (不存在)
- [ ] 刪除員工成功
- [ ] 查詢員工列表成功
- [ ] 查詢單筆員工成功
- [ ] 批次新增員工成功
- [ ] 批次修改員工成功
- [ ] 新增專櫃人員成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 批次操作測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 批次操作效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH110_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH1Z5_FS.ASP`

### 8.2 資料庫 Schema
- 參考 `SYSH110-人事基本資料維護系列.md` 的資料庫設計

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


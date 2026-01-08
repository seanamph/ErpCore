# SYSH210 - 薪資資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSH210系列
- **功能名稱**: 薪資資料維護系列
- **功能描述**: 提供員工薪資資料的新增、修改、刪除、查詢功能，包含基本薪資、津貼、獎金、扣款、實發薪資等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH210_PR.ASP` (報表)

### 1.2 業務需求
- 管理員工薪資基本資料
- 支援多種薪資項目（基本薪資、津貼、獎金、扣款等）
- 支援薪資計算邏輯
- 支援薪資異動記錄
- 支援薪資保密機制
- 支援薪資報表產生

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SalaryData` (對應舊系統 `SALARY`)

```sql
CREATE TABLE [dbo].[SalaryData] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [EmployeeId] NVARCHAR(50) NOT NULL, -- 員工編號 (EMP_ID)
    [SalaryYear] INT NOT NULL, -- 薪資年度 (SALARY_YEAR)
    [SalaryMonth] INT NOT NULL, -- 薪資月份 (SALARY_MONTH)
    [BasicSalary] DECIMAL(18, 2) NULL DEFAULT 0, -- 基本薪資 (BASIC_SALARY)
    [Allowance] DECIMAL(18, 2) NULL DEFAULT 0, -- 津貼 (ALLOWANCE)
    [Bonus] DECIMAL(18, 2) NULL DEFAULT 0, -- 獎金 (BONUS)
    [OvertimePay] DECIMAL(18, 2) NULL DEFAULT 0, -- 加班費 (OVERTIME_PAY)
    [Deduction] DECIMAL(18, 2) NULL DEFAULT 0, -- 扣款 (DEDUCTION)
    [Insurance] DECIMAL(18, 2) NULL DEFAULT 0, -- 保險費 (INSURANCE)
    [Tax] DECIMAL(18, 2) NULL DEFAULT 0, -- 稅額 (TAX)
    [NetSalary] DECIMAL(18, 2) NULL DEFAULT 0, -- 實發薪資 (NET_SALARY)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT', -- 狀態 (STATUS, DRAFT:草稿, CONFIRMED:確認, PAID:已發放)
    [PayDate] DATETIME2 NULL, -- 發放日期 (PAY_DATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_SalaryData] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_SalaryData_Employee_Year_Month] UNIQUE ([EmployeeId], [SalaryYear], [SalaryMonth])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalaryData_EmployeeId] ON [dbo].[SalaryData] ([EmployeeId]);
CREATE NONCLUSTERED INDEX [IX_SalaryData_Year_Month] ON [dbo].[SalaryData] ([SalaryYear], [SalaryMonth]);
CREATE NONCLUSTERED INDEX [IX_SalaryData_Status] ON [dbo].[SalaryData] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SalaryData_PayDate] ON [dbo].[SalaryData] ([PayDate]);
```

### 2.2 相關資料表

#### 2.2.1 `SalaryItems` - 薪資項目明細
```sql
CREATE TABLE [dbo].[SalaryItems] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SalaryDataId] BIGINT NOT NULL, -- 薪資資料ID (外鍵至SalaryData)
    [ItemType] NVARCHAR(20) NOT NULL, -- 項目類型 (ITEM_TYPE, INCOME:收入, DEDUCTION:扣款)
    [ItemCode] NVARCHAR(50) NOT NULL, -- 項目代碼 (ITEM_CODE)
    [ItemName] NVARCHAR(200) NOT NULL, -- 項目名稱 (ITEM_NAME)
    [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 金額 (AMOUNT)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SalaryItems] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_SalaryItems_SalaryData] FOREIGN KEY ([SalaryDataId]) REFERENCES [dbo].[SalaryData] ([TKey]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_SalaryItems_SalaryDataId] ON [dbo].[SalaryItems] ([SalaryDataId]);
CREATE NONCLUSTERED INDEX [IX_SalaryItems_ItemType] ON [dbo].[SalaryItems] ([ItemType]);
```

#### 2.2.2 `Employees` - 員工主檔
```sql
-- 參考員工主檔設計
-- 用於查詢員工基本資料
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| EmployeeId | NVARCHAR | 50 | NO | - | 員工編號 | 外鍵至Employees |
| SalaryYear | INT | - | NO | - | 薪資年度 | - |
| SalaryMonth | INT | - | NO | - | 薪資月份 | 1-12 |
| BasicSalary | DECIMAL | 18,2 | YES | 0 | 基本薪資 | - |
| Allowance | DECIMAL | 18,2 | YES | 0 | 津貼 | - |
| Bonus | DECIMAL | 18,2 | YES | 0 | 獎金 | - |
| OvertimePay | DECIMAL | 18,2 | YES | 0 | 加班費 | - |
| Deduction | DECIMAL | 18,2 | YES | 0 | 扣款 | - |
| Insurance | DECIMAL | 18,2 | YES | 0 | 保險費 | - |
| Tax | DECIMAL | 18,2 | YES | 0 | 稅額 | - |
| NetSalary | DECIMAL | 18,2 | YES | 0 | 實發薪資 | 計算欄位 |
| Status | NVARCHAR | 10 | NO | 'DRAFT' | 狀態 | DRAFT:草稿, CONFIRMED:確認, PAID:已發放 |
| PayDate | DATETIME2 | - | YES | - | 發放日期 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢薪資資料列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/salary-data`
- **說明**: 查詢薪資資料列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "SalaryYear",
    "sortOrder": "DESC",
    "filters": {
      "employeeId": "",
      "salaryYear": "",
      "salaryMonth": "",
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
          "employeeId": "E001",
          "employeeName": "張三",
          "salaryYear": 2024,
          "salaryMonth": 1,
          "basicSalary": 50000,
          "allowance": 5000,
          "bonus": 10000,
          "overtimePay": 3000,
          "deduction": 2000,
          "insurance": 3000,
          "tax": 5000,
          "netSalary": 58000,
          "status": "CONFIRMED",
          "payDate": "2024-02-05"
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

#### 3.1.2 查詢單筆薪資資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/salary-data/{tKey}`
- **說明**: 根據主鍵查詢單筆薪資資料（包含明細）
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 包含薪資資料及明細項目

#### 3.1.3 新增薪資資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/salary-data`
- **說明**: 新增薪資資料
- **請求格式**:
  ```json
  {
    "employeeId": "E001",
    "salaryYear": 2024,
    "salaryMonth": 1,
    "basicSalary": 50000,
    "allowance": 5000,
    "bonus": 10000,
    "overtimePay": 3000,
    "deduction": 2000,
    "insurance": 3000,
    "tax": 5000,
    "notes": "備註",
    "items": [
      {
        "itemType": "INCOME",
        "itemCode": "BASIC",
        "itemName": "基本薪資",
        "amount": 50000
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

#### 3.1.4 修改薪資資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/salary-data/{tKey}`
- **說明**: 修改薪資資料（僅限草稿狀態）
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增
- **回應格式**: 同新增

#### 3.1.5 刪除薪資資料
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/salary-data/{tKey}`
- **說明**: 刪除薪資資料（僅限草稿狀態）
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

#### 3.1.6 確認薪資資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/salary-data/{tKey}/confirm`
- **說明**: 確認薪資資料（狀態變更為CONFIRMED）
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同新增

#### 3.1.7 計算實發薪資
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/salary-data/calculate`
- **說明**: 計算實發薪資（不儲存）
- **請求格式**: 同新增（不含tKey）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "計算成功",
    "data": {
      "netSalary": 58000,
      "breakdown": {
        "totalIncome": 68000,
        "totalDeduction": 10000
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `SalaryDataController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/salary-data")]
    [Authorize]
    public class SalaryDataController : ControllerBase
    {
        private readonly ISalaryDataService _salaryDataService;
        
        public SalaryDataController(ISalaryDataService salaryDataService)
        {
            _salaryDataService = salaryDataService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<SalaryDataDto>>>> GetSalaryDataList([FromQuery] SalaryDataQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<SalaryDataDto>>> GetSalaryData(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateSalaryData([FromBody] CreateSalaryDataDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateSalaryData(long tKey, [FromBody] UpdateSalaryDataDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteSalaryData(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{tKey}/confirm")]
        public async Task<ActionResult<ApiResponse>> ConfirmSalaryData(long tKey)
        {
            // 實作確認邏輯
        }
        
        [HttpPost("calculate")]
        public async Task<ActionResult<ApiResponse<SalaryCalculationDto>>> CalculateSalary([FromBody] CalculateSalaryDto dto)
        {
            // 實作計算邏輯
        }
    }
}
```

#### 3.2.2 Service: `SalaryDataService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ISalaryDataService
    {
        Task<PagedResult<SalaryDataDto>> GetSalaryDataListAsync(SalaryDataQueryDto query);
        Task<SalaryDataDto> GetSalaryDataByIdAsync(long tKey);
        Task<long> CreateSalaryDataAsync(CreateSalaryDataDto dto);
        Task UpdateSalaryDataAsync(long tKey, UpdateSalaryDataDto dto);
        Task DeleteSalaryDataAsync(long tKey);
        Task ConfirmSalaryDataAsync(long tKey);
        Task<SalaryCalculationDto> CalculateSalaryAsync(CalculateSalaryDto dto);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 薪資資料列表頁面 (`SalaryDataList.vue`)
- **路徑**: `/hr/salary-data`
- **功能**: 顯示薪資資料列表，支援查詢、新增、修改、刪除、確認
- **主要元件**:
  - 查詢表單 (SalaryDataSearchForm)
  - 資料表格 (SalaryDataDataTable)
  - 新增/修改對話框 (SalaryDataDialog)
  - 刪除確認對話框
  - 確認對話框

#### 4.1.2 薪資資料詳細頁面 (`SalaryDataDetail.vue`)
- **路徑**: `/hr/salary-data/:tKey`
- **功能**: 顯示薪資資料詳細資料，支援修改、確認

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SalaryDataSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="員工編號">
      <el-input v-model="searchForm.employeeId" placeholder="請輸入員工編號" />
    </el-form-item>
    <el-form-item label="年度">
      <el-date-picker
        v-model="searchForm.salaryYear"
        type="year"
        placeholder="請選擇年度"
      />
    </el-form-item>
    <el-form-item label="月份">
      <el-select v-model="searchForm.salaryMonth" placeholder="請選擇月份">
        <el-option v-for="month in 12" :key="month" :label="month" :value="month" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="草稿" value="DRAFT" />
        <el-option label="確認" value="CONFIRMED" />
        <el-option label="已發放" value="PAID" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`SalaryDataDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="salaryDataList" v-loading="loading">
      <el-table-column prop="employeeId" label="員工編號" width="120" />
      <el-table-column prop="employeeName" label="員工姓名" width="120" />
      <el-table-column prop="salaryYear" label="年度" width="80" />
      <el-table-column prop="salaryMonth" label="月份" width="80" />
      <el-table-column prop="basicSalary" label="基本薪資" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.basicSalary) }}
        </template>
      </el-table-column>
      <el-table-column prop="netSalary" label="實發薪資" width="120" align="right">
        <template #default="{ row }">
          <span style="color: #409EFF; font-weight: bold;">
            {{ formatCurrency(row.netSalary) }}
          </span>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="payDate" label="發放日期" width="120" />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button 
            v-if="row.status === 'DRAFT'"
            type="success" 
            size="small" 
            @click="handleConfirm(row)"
          >
            確認
          </el-button>
          <el-button 
            v-if="row.status === 'DRAFT'"
            type="danger" 
            size="small" 
            @click="handleDelete(row)"
          >
            刪除
          </el-button>
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

#### 4.2.3 新增/修改對話框 (`SalaryDataDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="員工編號" prop="employeeId">
            <el-select
              v-model="form.employeeId"
              filterable
              placeholder="請選擇員工"
              @change="handleEmployeeChange"
            >
              <el-option
                v-for="emp in employeeList"
                :key="emp.employeeId"
                :label="`${emp.employeeId} - ${emp.employeeName}`"
                :value="emp.employeeId"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="員工姓名">
            <el-input v-model="employeeName" disabled />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="年度" prop="salaryYear">
            <el-date-picker
              v-model="form.salaryYear"
              type="year"
              placeholder="請選擇年度"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="月份" prop="salaryMonth">
            <el-select v-model="form.salaryMonth" placeholder="請選擇月份">
              <el-option v-for="month in 12" :key="month" :label="month" :value="month" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-divider>收入項目</el-divider>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="基本薪資" prop="basicSalary">
            <el-input-number
              v-model="form.basicSalary"
              :min="0"
              :precision="2"
              placeholder="請輸入基本薪資"
              @change="handleCalculate"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="津貼" prop="allowance">
            <el-input-number
              v-model="form.allowance"
              :min="0"
              :precision="2"
              placeholder="請輸入津貼"
              @change="handleCalculate"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="獎金" prop="bonus">
            <el-input-number
              v-model="form.bonus"
              :min="0"
              :precision="2"
              placeholder="請輸入獎金"
              @change="handleCalculate"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="加班費" prop="overtimePay">
            <el-input-number
              v-model="form.overtimePay"
              :min="0"
              :precision="2"
              placeholder="請輸入加班費"
              @change="handleCalculate"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-divider>扣款項目</el-divider>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="扣款" prop="deduction">
            <el-input-number
              v-model="form.deduction"
              :min="0"
              :precision="2"
              placeholder="請輸入扣款"
              @change="handleCalculate"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="保險費" prop="insurance">
            <el-input-number
              v-model="form.insurance"
              :min="0"
              :precision="2"
              placeholder="請輸入保險費"
              @change="handleCalculate"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="稅額" prop="tax">
            <el-input-number
              v-model="form.tax"
              :min="0"
              :precision="2"
              placeholder="請輸入稅額"
              @change="handleCalculate"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="實發薪資">
            <el-input
              v-model="form.netSalary"
              disabled
              style="color: #409EFF; font-weight: bold;"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="notes">
        <el-input
          v-model="form.notes"
          type="textarea"
          :rows="3"
          placeholder="請輸入備註"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`salaryData.api.ts`)
```typescript
import request from '@/utils/request';

export interface SalaryDataDto {
  tKey: number;
  employeeId: string;
  employeeName?: string;
  salaryYear: number;
  salaryMonth: number;
  basicSalary: number;
  allowance: number;
  bonus: number;
  overtimePay: number;
  deduction: number;
  insurance: number;
  tax: number;
  netSalary: number;
  status: string;
  payDate?: string;
  notes?: string;
  items?: SalaryItemDto[];
}

export interface SalaryItemDto {
  tKey: number;
  itemType: string;
  itemCode: string;
  itemName: string;
  amount: number;
  notes?: string;
}

export interface SalaryDataQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    employeeId?: string;
    salaryYear?: number;
    salaryMonth?: number;
    status?: string;
  };
}

export interface CreateSalaryDataDto {
  employeeId: string;
  salaryYear: number;
  salaryMonth: number;
  basicSalary: number;
  allowance: number;
  bonus: number;
  overtimePay: number;
  deduction: number;
  insurance: number;
  tax: number;
  notes?: string;
  items?: CreateSalaryItemDto[];
}

export interface CreateSalaryItemDto {
  itemType: string;
  itemCode: string;
  itemName: string;
  amount: number;
  notes?: string;
}

export interface UpdateSalaryDataDto extends CreateSalaryDataDto {}

export interface SalaryCalculationDto {
  netSalary: number;
  breakdown: {
    totalIncome: number;
    totalDeduction: number;
  };
}

export interface CalculateSalaryDto {
  basicSalary: number;
  allowance: number;
  bonus: number;
  overtimePay: number;
  deduction: number;
  insurance: number;
  tax: number;
}

// API 函數
export const getSalaryDataList = (query: SalaryDataQueryDto) => {
  return request.get<ApiResponse<PagedResult<SalaryDataDto>>>('/api/v1/salary-data', { params: query });
};

export const getSalaryDataById = (tKey: number) => {
  return request.get<ApiResponse<SalaryDataDto>>(`/api/v1/salary-data/${tKey}`);
};

export const createSalaryData = (data: CreateSalaryDataDto) => {
  return request.post<ApiResponse<number>>('/api/v1/salary-data', data);
};

export const updateSalaryData = (tKey: number, data: UpdateSalaryDataDto) => {
  return request.put<ApiResponse>(`/api/v1/salary-data/${tKey}`, data);
};

export const deleteSalaryData = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/salary-data/${tKey}`);
};

export const confirmSalaryData = (tKey: number) => {
  return request.post<ApiResponse>(`/api/v1/salary-data/${tKey}/confirm`);
};

export const calculateSalary = (data: CalculateSalaryDto) => {
  return request.post<ApiResponse<SalaryCalculationDto>>('/api/v1/salary-data/calculate', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（含薪資計算邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（員工存在性、年度月份唯一性、狀態檢查等）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 薪資計算功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試（薪資保密）

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限（薪資資料需特殊權限）
- 必須實作薪資保密機制
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查（僅授權人員可查看/修改）

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 員工編號必須存在
- 年度月份組合必須唯一
- 必填欄位必須驗證
- 金額欄位必須為正數
- 實發薪資必須正確計算

### 6.4 業務邏輯
- 僅草稿狀態的資料可修改/刪除
- 確認後的資料不可修改
- 實發薪資 = 收入總額 - 扣款總額
- 收入總額 = 基本薪資 + 津貼 + 獎金 + 加班費
- 扣款總額 = 扣款 + 保險費 + 稅額

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增薪資資料成功
- [ ] 新增薪資資料失敗 (重複年度月份)
- [ ] 修改薪資資料成功
- [ ] 修改薪資資料失敗 (非草稿狀態)
- [ ] 刪除薪資資料成功
- [ ] 刪除薪資資料失敗 (非草稿狀態)
- [ ] 確認薪資資料成功
- [ ] 查詢薪資資料列表成功
- [ ] 查詢單筆薪資資料成功
- [ ] 計算實發薪資正確

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 薪資計算邏輯測試
- [ ] 狀態變更測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH210_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH210_PR.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSH000/SYSH210.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


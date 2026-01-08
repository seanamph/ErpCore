# SYSGA10-SYSGA20 - 組織擴展功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSGA10-SYSGA20系列
- **功能名稱**: 組織擴展功能系列
- **功能描述**: 提供組織擴展資料的新增、修改、刪除、查詢功能，包含所得人所得資料維護、結算年度所得資料等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FI_B.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FMI.ASP` (批次新增)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FMU.ASP` (批次修改)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FC.ASP` (複製)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_FT.ASP` (結轉)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_FS.ASP` (反結轉)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_PR.ASP` (報表)

### 1.2 業務需求
- 管理所得人所得資料資訊
- 支援所得資料的批次新增、修改
- 支援所得資料的複製功能
- 支援結算年度所得資料管理
- 支援年度結轉與反結轉功能
- 記錄所得資料異動資訊

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EmployeeIncome` (所得人所得資料)

```sql
CREATE TABLE [dbo].[EmployeeIncome] (
    [IncomeId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EmployeeId] NVARCHAR(50) NOT NULL,
    [PayYear] NVARCHAR(4) NOT NULL, -- 給付年度
    [IncomeType] NVARCHAR(20) NULL, -- 所得類別
    [IncomeAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 所得金額
    [TaxAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 稅額
    [DeductionAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 扣除額
    [NetAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 淨額
    [VoucherNo] NVARCHAR(50) NULL, -- 傳票號碼
    [VoucherSummary] NVARCHAR(500) NULL, -- 傳票摘要
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:有效, I:無效
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_EmployeeIncome] PRIMARY KEY CLUSTERED ([IncomeId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EmployeeIncome_EmployeeId] ON [dbo].[EmployeeIncome] ([EmployeeId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeIncome_PayYear] ON [dbo].[EmployeeIncome] ([PayYear]);
CREATE NONCLUSTERED INDEX [IX_EmployeeIncome_Status] ON [dbo].[EmployeeIncome] ([Status]);
CREATE NONCLUSTERED INDEX [IX_EmployeeIncome_EmployeeId_PayYear] ON [dbo].[EmployeeIncome] ([EmployeeId], [PayYear]);
```

### 2.2 主要資料表: `AnnualIncomeSettlement` (結算年度所得資料)

```sql
CREATE TABLE [dbo].[AnnualIncomeSettlement] (
    [SettlementId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [PayYear] NVARCHAR(4) NOT NULL, -- 結算年度
    [SettlementDate] DATETIME2 NULL, -- 結算日期
    [SettlementStatus] NVARCHAR(10) NOT NULL DEFAULT 'N', -- N:未結轉, Y:已結轉, R:已反結轉
    [TotalIncomeAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 總所得金額
    [TotalTaxAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 總稅額
    [TotalDeductionAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 總扣除額
    [TotalNetAmount] DECIMAL(18,2) NULL DEFAULT 0, -- 總淨額
    [EmployeeCount] INT NULL DEFAULT 0, -- 員工人數
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [SettledBy] NVARCHAR(50) NULL,
    [SettledAt] DATETIME2 NULL,
    CONSTRAINT [PK_AnnualIncomeSettlement] PRIMARY KEY CLUSTERED ([SettlementId] ASC),
    CONSTRAINT [UQ_AnnualIncomeSettlement_PayYear] UNIQUE ([PayYear])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_AnnualIncomeSettlement_PayYear] ON [dbo].[AnnualIncomeSettlement] ([PayYear]);
CREATE NONCLUSTERED INDEX [IX_AnnualIncomeSettlement_SettlementStatus] ON [dbo].[AnnualIncomeSettlement] ([SettlementStatus]);
```

### 2.3 相關資料表

#### 2.3.1 `Employees` - 員工主檔
```sql
-- 用於查詢員工列表
-- 參考員工主檔設計
```

### 2.4 資料字典

#### EmployeeIncome 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| IncomeId | UNIQUEIDENTIFIER | - | NO | NEWID() | 所得資料編號 | 主鍵 |
| EmployeeId | NVARCHAR | 50 | NO | - | 員工編號 | 外鍵至員工表 |
| PayYear | NVARCHAR | 4 | NO | - | 給付年度 | - |
| IncomeType | NVARCHAR | 20 | YES | - | 所得類別 | - |
| IncomeAmount | DECIMAL | 18,2 | YES | 0 | 所得金額 | - |
| TaxAmount | DECIMAL | 18,2 | YES | 0 | 稅額 | - |
| DeductionAmount | DECIMAL | 18,2 | YES | 0 | 扣除額 | - |
| NetAmount | DECIMAL | 18,2 | YES | 0 | 淨額 | - |
| VoucherNo | NVARCHAR | 50 | YES | - | 傳票號碼 | - |
| VoucherSummary | NVARCHAR | 500 | YES | - | 傳票摘要 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:有效, I:無效 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### AnnualIncomeSettlement 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| SettlementId | UNIQUEIDENTIFIER | - | NO | NEWID() | 結算編號 | 主鍵 |
| PayYear | NVARCHAR | 4 | NO | - | 結算年度 | 唯一 |
| SettlementDate | DATETIME2 | - | YES | - | 結算日期 | - |
| SettlementStatus | NVARCHAR | 10 | NO | 'N' | 結算狀態 | N:未結轉, Y:已結轉, R:已反結轉 |
| TotalIncomeAmount | DECIMAL | 18,2 | YES | 0 | 總所得金額 | - |
| TotalTaxAmount | DECIMAL | 18,2 | YES | 0 | 總稅額 | - |
| TotalDeductionAmount | DECIMAL | 18,2 | YES | 0 | 總扣除額 | - |
| TotalNetAmount | DECIMAL | 18,2 | YES | 0 | 總淨額 | - |
| EmployeeCount | INT | - | YES | 0 | 員工人數 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| SettledBy | NVARCHAR | 50 | YES | - | 結轉者 | - |
| SettledAt | DATETIME2 | - | YES | - | 結轉時間 | - |

---

## 三、後端 API 設計

### 3.1 SYSGA10 API 端點列表

#### 3.1.1 查詢所得資料列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/hr/employee-income`
- **說明**: 查詢所得資料列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "PayYear",
    "sortOrder": "DESC",
    "filters": {
      "employeeId": "",
      "payYear": "",
      "incomeType": "",
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
          "incomeId": "guid",
          "employeeId": "E001",
          "employeeName": "張三",
          "payYear": "2024",
          "incomeType": "SALARY",
          "incomeAmount": 500000.00,
          "taxAmount": 50000.00,
          "deductionAmount": 200000.00,
          "netAmount": 250000.00,
          "voucherNo": "V001",
          "voucherSummary": "薪資給付",
          "status": "A"
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

#### 3.1.2 查詢單筆所得資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/hr/employee-income/{incomeId}`
- **說明**: 根據所得資料編號查詢單筆資料

#### 3.1.3 新增所得資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/employee-income`
- **說明**: 新增所得資料
- **請求格式**:
  ```json
  {
    "employeeId": "E001",
    "payYear": "2024",
    "incomeType": "SALARY",
    "incomeAmount": 500000.00,
    "taxAmount": 50000.00,
    "deductionAmount": 200000.00,
    "netAmount": 250000.00,
    "voucherNo": "V001",
    "voucherSummary": "薪資給付",
    "status": "A",
    "notes": "備註"
  }
  ```

#### 3.1.4 修改所得資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/hr/employee-income/{incomeId}`
- **說明**: 修改所得資料

#### 3.1.5 刪除所得資料
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/hr/employee-income/{incomeId}`
- **說明**: 刪除所得資料

#### 3.1.6 批次新增所得資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/employee-income/batch`
- **說明**: 批次新增多筆所得資料
- **請求格式**:
  ```json
  {
    "items": [
      {
        "employeeId": "E001",
        "payYear": "2024",
        "incomeType": "SALARY",
        "incomeAmount": 500000.00
      }
    ]
  }
  ```

#### 3.1.7 批次修改所得資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/hr/employee-income/batch`
- **說明**: 批次修改多筆所得資料

#### 3.1.8 複製所得資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/employee-income/{incomeId}/copy`
- **說明**: 複製所得資料到新年度
- **請求格式**:
  ```json
  {
    "targetPayYear": "2025"
  }
  ```

### 3.2 SYSGA20 API 端點列表

#### 3.2.1 查詢結算年度列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/hr/annual-income-settlement`
- **說明**: 查詢結算年度列表

#### 3.2.2 查詢單筆結算年度
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/hr/annual-income-settlement/{payYear}`
- **說明**: 根據年度查詢結算資料

#### 3.2.3 新增結算年度
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/annual-income-settlement`
- **說明**: 新增結算年度資料

#### 3.2.4 年度結轉
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/annual-income-settlement/{payYear}/settle`
- **說明**: 執行年度結轉作業
- **請求格式**:
  ```json
  {
    "settlementDate": "2024-12-31"
  }
  ```

#### 3.2.5 年度反結轉
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/annual-income-settlement/{payYear}/reverse`
- **說明**: 執行年度反結轉作業

#### 3.2.6 查詢結算年度報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/hr/annual-income-settlement/{payYear}/report`
- **說明**: 查詢結算年度報表資料

### 3.3 後端實作類別

#### 3.3.1 Controller: `EmployeeIncomeController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/hr/employee-income")]
    [Authorize]
    public class EmployeeIncomeController : ControllerBase
    {
        private readonly IEmployeeIncomeService _employeeIncomeService;
        
        public EmployeeIncomeController(IEmployeeIncomeService employeeIncomeService)
        {
            _employeeIncomeService = employeeIncomeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<EmployeeIncomeDto>>>> GetEmployeeIncomes([FromQuery] EmployeeIncomeQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{incomeId}")]
        public async Task<ActionResult<ApiResponse<EmployeeIncomeDto>>> GetEmployeeIncome(Guid incomeId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateEmployeeIncome([FromBody] CreateEmployeeIncomeDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{incomeId}")]
        public async Task<ActionResult<ApiResponse>> UpdateEmployeeIncome(Guid incomeId, [FromBody] UpdateEmployeeIncomeDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{incomeId}")]
        public async Task<ActionResult<ApiResponse>> DeleteEmployeeIncome(Guid incomeId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse>> BatchCreateEmployeeIncome([FromBody] BatchCreateEmployeeIncomeDto dto)
        {
            // 實作批次新增邏輯
        }
        
        [HttpPut("batch")]
        public async Task<ActionResult<ApiResponse>> BatchUpdateEmployeeIncome([FromBody] BatchUpdateEmployeeIncomeDto dto)
        {
            // 實作批次修改邏輯
        }
        
        [HttpPost("{incomeId}/copy")]
        public async Task<ActionResult<ApiResponse<Guid>>> CopyEmployeeIncome(Guid incomeId, [FromBody] CopyEmployeeIncomeDto dto)
        {
            // 實作複製邏輯
        }
    }
}
```

#### 3.3.2 Controller: `AnnualIncomeSettlementController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/hr/annual-income-settlement")]
    [Authorize]
    public class AnnualIncomeSettlementController : ControllerBase
    {
        private readonly IAnnualIncomeSettlementService _settlementService;
        
        public AnnualIncomeSettlementController(IAnnualIncomeSettlementService settlementService)
        {
            _settlementService = settlementService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AnnualIncomeSettlementDto>>>> GetSettlements()
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{payYear}")]
        public async Task<ActionResult<ApiResponse<AnnualIncomeSettlementDto>>> GetSettlement(string payYear)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateSettlement([FromBody] CreateAnnualIncomeSettlementDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPost("{payYear}/settle")]
        public async Task<ActionResult<ApiResponse>> SettleYear(string payYear, [FromBody] SettleYearDto dto)
        {
            // 實作結轉邏輯
        }
        
        [HttpPost("{payYear}/reverse")]
        public async Task<ActionResult<ApiResponse>> ReverseSettle(string payYear)
        {
            // 實作反結轉邏輯
        }
        
        [HttpGet("{payYear}/report")]
        public async Task<ActionResult<ApiResponse<AnnualIncomeSettlementReportDto>>> GetSettlementReport(string payYear)
        {
            // 實作報表查詢邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 SYSGA10 頁面結構

#### 4.1.1 所得資料列表頁面 (`EmployeeIncomeList.vue`)
- **路徑**: `/hr/employee-income`
- **功能**: 顯示所得資料列表，支援查詢、新增、修改、刪除、批次操作、複製

#### 4.1.2 所得資料新增/修改對話框 (`EmployeeIncomeDialog.vue`)
- **功能**: 新增或修改所得資料

#### 4.1.3 批次新增對話框 (`EmployeeIncomeBatchDialog.vue`)
- **功能**: 批次新增所得資料

### 4.2 SYSGA20 頁面結構

#### 4.2.1 結算年度列表頁面 (`AnnualIncomeSettlementList.vue`)
- **路徑**: `/hr/annual-income-settlement`
- **功能**: 顯示結算年度列表，支援查詢、新增、結轉、反結轉

#### 4.2.2 結算年度報表頁面 (`AnnualIncomeSettlementReport.vue`)
- **路徑**: `/hr/annual-income-settlement/:payYear/report`
- **功能**: 顯示結算年度報表

### 4.3 UI 元件設計

#### 4.3.1 所得資料查詢表單 (`EmployeeIncomeSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="員工編號">
      <el-input v-model="searchForm.employeeId" placeholder="請輸入員工編號" />
    </el-form-item>
    <el-form-item label="給付年度">
      <el-date-picker v-model="searchForm.payYear" type="year" placeholder="請選擇年度" />
    </el-form-item>
    <el-form-item label="所得類別">
      <el-select v-model="searchForm.incomeType" placeholder="請選擇所得類別">
        <el-option label="薪資" value="SALARY" />
        <el-option label="獎金" value="BONUS" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="有效" value="A" />
        <el-option label="無效" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.3.2 所得資料表格 (`EmployeeIncomeDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="incomeList" v-loading="loading">
      <el-table-column prop="employeeId" label="員工編號" width="120" />
      <el-table-column prop="employeeName" label="員工姓名" width="150" />
      <el-table-column prop="payYear" label="給付年度" width="100" />
      <el-table-column prop="incomeType" label="所得類別" width="100" />
      <el-table-column prop="incomeAmount" label="所得金額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.incomeAmount) }}
        </template>
      </el-table-column>
      <el-table-column prop="taxAmount" label="稅額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.taxAmount) }}
        </template>
      </el-table-column>
      <el-table-column prop="netAmount" label="淨額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.netAmount) }}
        </template>
      </el-table-column>
      <el-table-column prop="voucherNo" label="傳票號碼" width="120" />
      <el-table-column prop="voucherSummary" label="傳票摘要" width="200" show-overflow-tooltip />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="info" size="small" @click="handleCopy(row)">複製</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
```

### 4.4 API 呼叫 (`employeeIncome.api.ts`)
```typescript
import request from '@/utils/request';

export interface EmployeeIncomeDto {
  incomeId: string;
  employeeId: string;
  employeeName?: string;
  payYear: string;
  incomeType?: string;
  incomeAmount: number;
  taxAmount: number;
  deductionAmount: number;
  netAmount: number;
  voucherNo?: string;
  voucherSummary?: string;
  status: string;
  notes?: string;
}

export interface AnnualIncomeSettlementDto {
  settlementId: string;
  payYear: string;
  settlementDate?: string;
  settlementStatus: string;
  totalIncomeAmount: number;
  totalTaxAmount: number;
  totalDeductionAmount: number;
  totalNetAmount: number;
  employeeCount: number;
}

// API 函數
export const getEmployeeIncomeList = (query: any) => {
  return request.get<ApiResponse<PagedResult<EmployeeIncomeDto>>>('/api/v1/hr/employee-income', { params: query });
};

export const getEmployeeIncomeById = (incomeId: string) => {
  return request.get<ApiResponse<EmployeeIncomeDto>>(`/api/v1/hr/employee-income/${incomeId}`);
};

export const createEmployeeIncome = (data: any) => {
  return request.post<ApiResponse<string>>('/api/v1/hr/employee-income', data);
};

export const updateEmployeeIncome = (incomeId: string, data: any) => {
  return request.put<ApiResponse>(`/api/v1/hr/employee-income/${incomeId}`, data);
};

export const deleteEmployeeIncome = (incomeId: string) => {
  return request.delete<ApiResponse>(`/api/v1/hr/employee-income/${incomeId}`);
};

export const batchCreateEmployeeIncome = (data: any) => {
  return request.post<ApiResponse>('/api/v1/hr/employee-income/batch', data);
};

export const batchUpdateEmployeeIncome = (data: any) => {
  return request.put<ApiResponse>('/api/v1/hr/employee-income/batch', data);
};

export const copyEmployeeIncome = (incomeId: string, targetPayYear: string) => {
  return request.post<ApiResponse<string>>(`/api/v1/hr/employee-income/${incomeId}/copy`, { targetPayYear });
};

export const getSettlementList = () => {
  return request.get<ApiResponse<List<AnnualIncomeSettlementDto>>>('/api/v1/hr/annual-income-settlement');
};

export const getSettlementByYear = (payYear: string) => {
  return request.get<ApiResponse<AnnualIncomeSettlementDto>>(`/api/v1/hr/annual-income-settlement/${payYear}`);
};

export const settleYear = (payYear: string, settlementDate: string) => {
  return request.post<ApiResponse>(`/api/v1/hr/annual-income-settlement/${payYear}/settle`, { settlementDate });
};

export const reverseSettle = (payYear: string) => {
  return request.post<ApiResponse>(`/api/v1/hr/annual-income-settlement/${payYear}/reverse`);
};

export const getSettlementReport = (payYear: string) => {
  return request.get<ApiResponse<any>>(`/api/v1/hr/annual-income-settlement/${payYear}/report`);
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
- [ ] 批次處理邏輯實作
- [ ] 結轉/反結轉邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 批次操作對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 結轉/反結轉功能開發
- [ ] 報表頁面開發
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
- 個人資料保護法規遵循

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 批次操作必須使用事務處理
- 必須使用快取機制

### 6.3 資料驗證
- 員工編號必須存在
- 給付年度必須在允許範圍內
- 金額必須為正數
- 結轉狀態必須驗證

### 6.4 業務邏輯
- 已結轉的年度不得修改所得資料
- 結轉前必須檢查所有資料完整性
- 反結轉必須檢查是否有後續資料
- 複製功能必須檢查目標年度是否已結轉

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增所得資料成功
- [ ] 新增所得資料失敗 (員工不存在)
- [ ] 修改所得資料成功
- [ ] 刪除所得資料成功
- [ ] 批次新增所得資料成功
- [ ] 批次修改所得資料成功
- [ ] 複製所得資料成功
- [ ] 年度結轉成功
- [ ] 年度反結轉成功
- [ ] 查詢所得資料列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 批次操作流程測試
- [ ] 結轉/反結轉流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 批次操作效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FI_B.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FMI.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FMU.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA10_FC.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_FT.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_FS.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGA20_PR.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01



# SYSN150-SYSN154 - 會計結帳作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN150-SYSN154 系列
- **功能名稱**: 會計結帳作業系列
- **功能描述**: 提供會計結帳作業的新增、修改、刪除、查詢功能，包含結帳期間、結帳狀態、結帳日期、結帳人員等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN150_PR.ASP` (報表)

### 1.2 業務需求
- 管理會計結帳作業基本資料
- 支援結帳期間設定（年度、月份）
- 支援結帳狀態管理（未結帳、結帳中、已結帳）
- 支援結帳日期記錄
- 支援結帳人員記錄
- 支援結帳備註
- 支援結帳前檢查（未過帳憑證、未平衡科目等）
- 支援結帳後處理（期末餘額轉期初餘額等）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `AccountingClosings` (對應舊系統 `CLOSING`)

```sql
CREATE TABLE [dbo].[AccountingClosings] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ClosingId] NVARCHAR(50) NOT NULL, -- 結帳代號 (CLOSING_ID)
    [ClosingYear] INT NOT NULL, -- 結帳年度 (CLOSING_YEAR)
    [ClosingMonth] INT NOT NULL, -- 結帳月份 (CLOSING_MONTH)
    [ClosingDate] DATETIME2 NULL, -- 結帳日期 (CLOSING_DATE)
    [ClosingStatus] NVARCHAR(10) NOT NULL DEFAULT 'PENDING', -- 結帳狀態 (CLOSING_STATUS, PENDING:未結帳, PROCESSING:結帳中, CLOSED:已結帳)
    [ClosingUser] NVARCHAR(50) NULL, -- 結帳人員 (CLOSING_USER)
    [ClosingNotes] NVARCHAR(500) NULL, -- 結帳備註 (CLOSING_NOTES)
    [PreClosingCheck] NVARCHAR(1) NULL DEFAULT 'N', -- 結帳前檢查 (PRE_CLOSING_CHECK, Y/N)
    [PreClosingCheckDate] DATETIME2 NULL, -- 結帳前檢查日期 (PRE_CLOSING_CHECK_DATE)
    [PreClosingCheckUser] NVARCHAR(50) NULL, -- 結帳前檢查人員 (PRE_CLOSING_CHECK_USER)
    [PreClosingCheckResult] NVARCHAR(500) NULL, -- 結帳前檢查結果 (PRE_CLOSING_CHECK_RESULT)
    [PostClosingProcess] NVARCHAR(1) NULL DEFAULT 'N', -- 結帳後處理 (POST_CLOSING_PROCESS, Y/N)
    [PostClosingProcessDate] DATETIME2 NULL, -- 結帳後處理日期 (POST_CLOSING_PROCESS_DATE)
    [PostClosingProcessUser] NVARCHAR(50) NULL, -- 結帳後處理人員 (POST_CLOSING_PROCESS_USER)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_AccountingClosings] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_AccountingClosings_ClosingId] UNIQUE ([ClosingId]),
    CONSTRAINT [UQ_AccountingClosings_YearMonth] UNIQUE ([ClosingYear], [ClosingMonth])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_AccountingClosings_ClosingYear] ON [dbo].[AccountingClosings] ([ClosingYear]);
CREATE NONCLUSTERED INDEX [IX_AccountingClosings_ClosingMonth] ON [dbo].[AccountingClosings] ([ClosingMonth]);
CREATE NONCLUSTERED INDEX [IX_AccountingClosings_ClosingStatus] ON [dbo].[AccountingClosings] ([ClosingStatus]);
CREATE NONCLUSTERED INDEX [IX_AccountingClosings_ClosingDate] ON [dbo].[AccountingClosings] ([ClosingDate]);
```

### 2.2 相關資料表

#### 2.2.1 `AccountingClosingDetails` - 結帳明細
```sql
CREATE TABLE [dbo].[AccountingClosingDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ClosingId] NVARCHAR(50) NOT NULL, -- 結帳代號
    [StypeId] NVARCHAR(50) NOT NULL, -- 科目代號
    [OpeningBalance] DECIMAL(18, 2) NULL DEFAULT 0, -- 期初餘額
    [DebitAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 借方金額
    [CreditAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 貸方金額
    [ClosingBalance] DECIMAL(18, 2) NULL DEFAULT 0, -- 期末餘額
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_AccountingClosingDetails_AccountingClosings] FOREIGN KEY ([ClosingId]) REFERENCES [dbo].[AccountingClosings] ([ClosingId]) ON DELETE CASCADE,
    CONSTRAINT [FK_AccountingClosingDetails_AccountSubjects] FOREIGN KEY ([StypeId]) REFERENCES [dbo].[AccountSubjects] ([StypeId])
);

CREATE NONCLUSTERED INDEX [IX_AccountingClosingDetails_ClosingId] ON [dbo].[AccountingClosingDetails] ([ClosingId]);
CREATE NONCLUSTERED INDEX [IX_AccountingClosingDetails_StypeId] ON [dbo].[AccountingClosingDetails] ([StypeId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ClosingId | NVARCHAR | 50 | NO | - | 結帳代號 | 唯一，主鍵候選 |
| ClosingYear | INT | - | NO | - | 結帳年度 | - |
| ClosingMonth | INT | - | NO | - | 結帳月份 | 1-12 |
| ClosingDate | DATETIME2 | - | YES | - | 結帳日期 | - |
| ClosingStatus | NVARCHAR | 10 | NO | 'PENDING' | 結帳狀態 | PENDING:未結帳, PROCESSING:結帳中, CLOSED:已結帳 |
| ClosingUser | NVARCHAR | 50 | YES | - | 結帳人員 | - |
| ClosingNotes | NVARCHAR | 500 | YES | - | 結帳備註 | - |
| PreClosingCheck | NVARCHAR | 1 | YES | 'N' | 結帳前檢查 | Y/N |
| PreClosingCheckDate | DATETIME2 | - | YES | - | 結帳前檢查日期 | - |
| PreClosingCheckUser | NVARCHAR | 50 | YES | - | 結帳前檢查人員 | - |
| PreClosingCheckResult | NVARCHAR | 500 | YES | - | 結帳前檢查結果 | - |
| PostClosingProcess | NVARCHAR | 1 | YES | 'N' | 結帳後處理 | Y/N |
| PostClosingProcessDate | DATETIME2 | - | YES | - | 結帳後處理日期 | - |
| PostClosingProcessUser | NVARCHAR | 50 | YES | - | 結帳後處理人員 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢結帳作業列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/accounting-closings`
- **說明**: 查詢結帳作業列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ClosingYear",
    "sortOrder": "DESC",
    "filters": {
      "closingYear": "",
      "closingMonth": "",
      "closingStatus": ""
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
          "closingId": "CL202401",
          "closingYear": 2024,
          "closingMonth": 1,
          "closingDate": "2024-02-01T00:00:00",
          "closingStatus": "CLOSED",
          "closingUser": "U001"
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

#### 3.1.2 查詢單筆結帳作業
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/accounting-closings/{closingId}`
- **說明**: 根據結帳代號查詢單筆結帳作業資料
- **路徑參數**:
  - `closingId`: 結帳代號
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增結帳作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/accounting-closings`
- **說明**: 新增結帳作業資料
- **請求格式**:
  ```json
  {
    "closingYear": 2024,
    "closingMonth": 1,
    "closingNotes": "2024年1月結帳"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "closingId": "CL202401"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改結帳作業
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/accounting-closings/{closingId}`
- **說明**: 修改結帳作業資料（僅限未結帳狀態）
- **路徑參數**:
  - `closingId`: 結帳代號
- **請求格式**: 同新增，但 `closingYear` 和 `closingMonth` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除結帳作業
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/accounting-closings/{closingId}`
- **說明**: 刪除結帳作業資料（僅限未結帳狀態）
- **路徑參數**:
  - `closingId`: 結帳代號
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

#### 3.1.6 執行結帳前檢查
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/accounting-closings/{closingId}/pre-closing-check`
- **說明**: 執行結帳前檢查（未過帳憑證、未平衡科目等）
- **路徑參數**:
  - `closingId`: 結帳代號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "檢查完成",
    "data": {
      "checkResult": "PASS",
      "checkDetails": [
        {
          "checkType": "UNPOSTED_VOUCHER",
          "checkStatus": "PASS",
          "checkMessage": "無未過帳憑證"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 執行結帳作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/accounting-closings/{closingId}/close`
- **說明**: 執行結帳作業（需先通過結帳前檢查）
- **路徑參數**:
  - `closingId`: 結帳代號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "結帳成功",
    "data": {
      "closingId": "CL202401",
      "closingDate": "2024-02-01T00:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 執行結帳後處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/accounting-closings/{closingId}/post-closing-process`
- **說明**: 執行結帳後處理（期末餘額轉期初餘額等）
- **路徑參數**:
  - `closingId`: 結帳代號
- **回應格式**: 同執行結帳作業

### 3.2 後端實作類別

#### 3.2.1 Controller: `AccountingClosingsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/accounting-closings")]
    [Authorize]
    public class AccountingClosingsController : ControllerBase
    {
        private readonly IAccountingClosingService _accountingClosingService;
        
        public AccountingClosingsController(IAccountingClosingService accountingClosingService)
        {
            _accountingClosingService = accountingClosingService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AccountingClosingDto>>>> GetAccountingClosings([FromQuery] AccountingClosingQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{closingId}")]
        public async Task<ActionResult<ApiResponse<AccountingClosingDto>>> GetAccountingClosing(string closingId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateAccountingClosing([FromBody] CreateAccountingClosingDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{closingId}")]
        public async Task<ActionResult<ApiResponse>> UpdateAccountingClosing(string closingId, [FromBody] UpdateAccountingClosingDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{closingId}")]
        public async Task<ActionResult<ApiResponse>> DeleteAccountingClosing(string closingId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{closingId}/pre-closing-check")]
        public async Task<ActionResult<ApiResponse<PreClosingCheckResultDto>>> PreClosingCheck(string closingId)
        {
            // 實作結帳前檢查邏輯
        }
        
        [HttpPost("{closingId}/close")]
        public async Task<ActionResult<ApiResponse<ClosingResultDto>>> Close(string closingId)
        {
            // 實作結帳邏輯
        }
        
        [HttpPost("{closingId}/post-closing-process")]
        public async Task<ActionResult<ApiResponse<PostClosingProcessResultDto>>> PostClosingProcess(string closingId)
        {
            // 實作結帳後處理邏輯
        }
    }
}
```

#### 3.2.2 Service: `AccountingClosingService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IAccountingClosingService
    {
        Task<PagedResult<AccountingClosingDto>> GetAccountingClosingsAsync(AccountingClosingQueryDto query);
        Task<AccountingClosingDto> GetAccountingClosingByIdAsync(string closingId);
        Task<string> CreateAccountingClosingAsync(CreateAccountingClosingDto dto);
        Task UpdateAccountingClosingAsync(string closingId, UpdateAccountingClosingDto dto);
        Task DeleteAccountingClosingAsync(string closingId);
        Task<PreClosingCheckResultDto> PreClosingCheckAsync(string closingId);
        Task<ClosingResultDto> CloseAsync(string closingId);
        Task<PostClosingProcessResultDto> PostClosingProcessAsync(string closingId);
    }
}
```

#### 3.2.3 Repository: `AccountingClosingRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IAccountingClosingRepository
    {
        Task<AccountingClosing> GetByIdAsync(string closingId);
        Task<PagedResult<AccountingClosing>> GetPagedAsync(AccountingClosingQuery query);
        Task<AccountingClosing> CreateAsync(AccountingClosing accountingClosing);
        Task<AccountingClosing> UpdateAsync(AccountingClosing accountingClosing);
        Task DeleteAsync(string closingId);
        Task<bool> ExistsAsync(int closingYear, int closingMonth);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 結帳作業列表頁面 (`AccountingClosingList.vue`)
- **路徑**: `/accounting/accounting-closings`
- **功能**: 顯示結帳作業列表，支援查詢、新增、修改、刪除、執行結帳
- **主要元件**:
  - 查詢表單 (AccountingClosingSearchForm)
  - 資料表格 (AccountingClosingDataTable)
  - 新增/修改對話框 (AccountingClosingDialog)
  - 刪除確認對話框
  - 結帳前檢查對話框
  - 結帳確認對話框

#### 4.1.2 結帳作業詳細頁面 (`AccountingClosingDetail.vue`)
- **路徑**: `/accounting/accounting-closings/:closingId`
- **功能**: 顯示結帳作業詳細資料，支援修改、執行結帳前檢查、執行結帳、執行結帳後處理

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`AccountingClosingSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="結帳年度">
      <el-input-number v-model="searchForm.closingYear" :min="2000" :max="2099" placeholder="請輸入結帳年度" />
    </el-form-item>
    <el-form-item label="結帳月份">
      <el-select v-model="searchForm.closingMonth" placeholder="請選擇結帳月份">
        <el-option v-for="month in 12" :key="month" :label="month + '月'" :value="month" />
      </el-select>
    </el-form-item>
    <el-form-item label="結帳狀態">
      <el-select v-model="searchForm.closingStatus" placeholder="請選擇結帳狀態">
        <el-option label="未結帳" value="PENDING" />
        <el-option label="結帳中" value="PROCESSING" />
        <el-option label="已結帳" value="CLOSED" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`AccountingClosingDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="accountingClosingList" v-loading="loading">
      <el-table-column prop="closingId" label="結帳代號" width="120" />
      <el-table-column prop="closingYear" label="結帳年度" width="100" />
      <el-table-column prop="closingMonth" label="結帳月份" width="100" />
      <el-table-column prop="closingDate" label="結帳日期" width="150" />
      <el-table-column prop="closingStatus" label="結帳狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.closingStatus)">
            {{ getStatusText(row.closingStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="closingUser" label="結帳人員" width="120" />
      <el-table-column label="操作" width="300" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)" v-if="row.closingStatus === 'PENDING'">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" v-if="row.closingStatus === 'PENDING'">刪除</el-button>
          <el-button type="success" size="small" @click="handlePreClosingCheck(row)" v-if="row.closingStatus === 'PENDING'">結帳前檢查</el-button>
          <el-button type="warning" size="small" @click="handleClose(row)" v-if="row.closingStatus === 'PENDING'">執行結帳</el-button>
          <el-button type="info" size="small" @click="handlePostClosingProcess(row)" v-if="row.closingStatus === 'CLOSED'">結帳後處理</el-button>
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

#### 4.2.3 新增/修改對話框 (`AccountingClosingDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
      <el-form-item label="結帳年度" prop="closingYear">
        <el-input-number v-model="form.closingYear" :min="2000" :max="2099" :disabled="isEdit" placeholder="請輸入結帳年度" />
      </el-form-item>
      <el-form-item label="結帳月份" prop="closingMonth">
        <el-select v-model="form.closingMonth" :disabled="isEdit" placeholder="請選擇結帳月份">
          <el-option v-for="month in 12" :key="month" :label="month + '月'" :value="month" />
        </el-select>
      </el-form-item>
      <el-form-item label="結帳備註" prop="closingNotes">
        <el-input v-model="form.closingNotes" type="textarea" :rows="4" placeholder="請輸入結帳備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`accountingClosing.api.ts`)
```typescript
import request from '@/utils/request';

export interface AccountingClosingDto {
  tKey: number;
  closingId: string;
  closingYear: number;
  closingMonth: number;
  closingDate?: string;
  closingStatus: string;
  closingUser?: string;
  closingNotes?: string;
  preClosingCheck?: string;
  preClosingCheckDate?: string;
  preClosingCheckUser?: string;
  preClosingCheckResult?: string;
  postClosingProcess?: string;
  postClosingProcessDate?: string;
  postClosingProcessUser?: string;
}

export interface AccountingClosingQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    closingYear?: number;
    closingMonth?: number;
    closingStatus?: string;
  };
}

export interface CreateAccountingClosingDto {
  closingYear: number;
  closingMonth: number;
  closingNotes?: string;
}

export interface UpdateAccountingClosingDto {
  closingNotes?: string;
}

export interface PreClosingCheckResultDto {
  checkResult: string;
  checkDetails: Array<{
    checkType: string;
    checkStatus: string;
    checkMessage: string;
  }>;
}

export interface ClosingResultDto {
  closingId: string;
  closingDate: string;
}

export interface PostClosingProcessResultDto {
  closingId: string;
  processDate: string;
}

// API 函數
export const getAccountingClosingList = (query: AccountingClosingQueryDto) => {
  return request.get<ApiResponse<PagedResult<AccountingClosingDto>>>('/api/v1/accounting-closings', { params: query });
};

export const getAccountingClosingById = (closingId: string) => {
  return request.get<ApiResponse<AccountingClosingDto>>(`/api/v1/accounting-closings/${closingId}`);
};

export const createAccountingClosing = (data: CreateAccountingClosingDto) => {
  return request.post<ApiResponse<string>>('/api/v1/accounting-closings', data);
};

export const updateAccountingClosing = (closingId: string, data: UpdateAccountingClosingDto) => {
  return request.put<ApiResponse>(`/api/v1/accounting-closings/${closingId}`, data);
};

export const deleteAccountingClosing = (closingId: string) => {
  return request.delete<ApiResponse>(`/api/v1/accounting-closings/${closingId}`);
};

export const preClosingCheck = (closingId: string) => {
  return request.post<ApiResponse<PreClosingCheckResultDto>>(`/api/v1/accounting-closings/${closingId}/pre-closing-check`);
};

export const close = (closingId: string) => {
  return request.post<ApiResponse<ClosingResultDto>>(`/api/v1/accounting-closings/${closingId}/close`);
};

export const postClosingProcess = (closingId: string) => {
  return request.post<ApiResponse<PostClosingProcessResultDto>>(`/api/v1/accounting-closings/${closingId}/post-closing-process`);
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

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（年度月份唯一性、結帳狀態驗證等）
- [ ] 結帳前檢查邏輯實作
- [ ] 結帳邏輯實作
- [ ] 結帳後處理邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 結帳前檢查對話框開發
- [ ] 結帳確認對話框開發
- [ ] 結帳後處理對話框開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 結帳流程測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12.5天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 必須驗證年度月份的唯一性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 結帳作業必須使用交易處理
- 結帳後處理必須使用批次處理

### 6.3 資料驗證
- 年度月份必須唯一
- 必填欄位必須驗證
- 結帳狀態必須在允許範圍內
- 結帳前必須通過檢查

### 6.4 業務邏輯
- 僅未結帳狀態可修改或刪除
- 結帳前必須執行檢查
- 結帳後必須執行後處理
- 結帳後不可再修改或刪除

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增結帳作業成功
- [ ] 新增結帳作業失敗 (重複年度月份)
- [ ] 修改結帳作業成功
- [ ] 修改結帳作業失敗 (已結帳)
- [ ] 刪除結帳作業成功
- [ ] 刪除結帳作業失敗 (已結帳)
- [ ] 查詢結帳作業列表成功
- [ ] 查詢單筆結帳作業成功
- [ ] 執行結帳前檢查成功
- [ ] 執行結帳作業成功
- [ ] 執行結帳後處理成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 結帳流程測試
- [ ] 結帳後處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 結帳作業效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN150_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN150_PR.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSN000/SYSN150.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


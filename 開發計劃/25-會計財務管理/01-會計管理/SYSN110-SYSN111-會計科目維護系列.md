# SYSN110-SYSN111 - 會計科目維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN110-SYSN111 系列
- **功能名稱**: 會計科目維護系列
- **功能描述**: 提供會計科目資料的新增、修改、刪除、查詢功能，包含科目代號、科目名稱、借貸方向、統制/明細、傳票格式、預算科目等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN110_PR.ASP` (報表)

### 1.2 業務需求
- 管理會計科目基本資料
- 支援科目代號階層結構（統制科目/明細科目）
- 支援借貸方向設定（借方/貸方）
- 支援傳票格式設定（一般/固定資產）
- 支援預算科目標記
- 支援部門代號設定
- 支援IFRS會計科目對應
- 支援ROC(集團)會計科目對應
- 支援SAP會計科目對應
- 支援會科類別設定
- 支援會科自訂排序功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `AccountSubjects` (對應舊系統 `STYPE`)

```sql
CREATE TABLE [dbo].[AccountSubjects] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [StypeId] NVARCHAR(50) NOT NULL, -- 科目代號 (STYPE_ID)
    [StypeName] NVARCHAR(200) NOT NULL, -- 科目名稱 (STYPE_NAME)
    [StypeNameE] NVARCHAR(200) NULL, -- 科目英文名稱 (STYPE_NAME_E)
    [Dc] NVARCHAR(1) NULL, -- 借/貸 (DC, D:借方, C:貸方)
    [LedgerMd] NVARCHAR(1) NULL, -- 統制/明細 (LEDGER_MD, L:統制, M:明細)
    [MtypeId] NVARCHAR(10) NULL, -- 三碼代號 (MTYPE_ID)
    [AbatYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否為沖帳代號 (ABAT_YN, Y/N)
    [VoucherType] NVARCHAR(10) NULL, -- 傳票格式 (VOUCHER_TYPE)
    [BudgetYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否為預算科目 (BUDGET_YN, Y/N)
    [OrgYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否設定部門代號 (ORG_YN, Y/N)
    [ExpYear] DECIMAL(18, 2) NULL, -- 折舊攤提年限 (EXP_YEAR)
    [ResiValue] DECIMAL(18, 2) NULL, -- 殘值年限 (RESI_VALUE)
    [DepreLid] NVARCHAR(50) NULL, -- 折舊會計科目 (DEPRE_LID)
    [AccudepreLid] NVARCHAR(50) NULL, -- 累計折舊會計科目 (ACCUDEPRE_LID)
    [StypeYn] NVARCHAR(1) NULL DEFAULT 'Y', -- 是否可輸 (STYPE_YN, Y/N)
    [IfrsStypeId] NVARCHAR(50) NULL, -- IFRS會計科目 (IFRS_STYPE_ID)
    [RocStypeId] NVARCHAR(50) NULL, -- 集團會計科目 (ROC_STYPE_ID)
    [SapStypeId] NVARCHAR(50) NULL, -- SAP會計科目 (SAP_STYPE_ID)
    [StypeClass] NVARCHAR(50) NULL, -- 科目別 (STYPE_CLASS)
    [StypeOrder] INT NULL, -- 排序 (STYPE_ORDER)
    [Amt0] DECIMAL(18, 2) NULL DEFAULT 0, -- 期初餘額 (AMT_0)
    [Amt1] DECIMAL(18, 2) NULL DEFAULT 0, -- 期末餘額 (AMT_1)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_AccountSubjects] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_AccountSubjects_StypeId] UNIQUE ([StypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_AccountSubjects_StypeId] ON [dbo].[AccountSubjects] ([StypeId]);
CREATE NONCLUSTERED INDEX [IX_AccountSubjects_Dc] ON [dbo].[AccountSubjects] ([Dc]);
CREATE NONCLUSTERED INDEX [IX_AccountSubjects_LedgerMd] ON [dbo].[AccountSubjects] ([LedgerMd]);
CREATE NONCLUSTERED INDEX [IX_AccountSubjects_VoucherType] ON [dbo].[AccountSubjects] ([VoucherType]);
CREATE NONCLUSTERED INDEX [IX_AccountSubjects_BudgetYn] ON [dbo].[AccountSubjects] ([BudgetYn]);
CREATE NONCLUSTERED INDEX [IX_AccountSubjects_StypeClass] ON [dbo].[AccountSubjects] ([StypeClass]);
CREATE NONCLUSTERED INDEX [IX_AccountSubjects_StypeOrder] ON [dbo].[AccountSubjects] ([StypeOrder]);
```

### 2.2 相關資料表

#### 2.2.1 `AccountSubjectMapping` - 會計科目對應關係
```sql
-- 用於處理IFRS、ROC、SAP等對應關係
-- 可視業務需求決定是否獨立成表或直接存在主表
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| StypeId | NVARCHAR | 50 | NO | - | 科目代號 | 唯一，主鍵候選 |
| StypeName | NVARCHAR | 200 | NO | - | 科目名稱 | - |
| StypeNameE | NVARCHAR | 200 | YES | - | 科目英文名稱 | - |
| Dc | NVARCHAR | 1 | YES | - | 借/貸 | D:借方, C:貸方 |
| LedgerMd | NVARCHAR | 1 | YES | - | 統制/明細 | L:統制, M:明細 |
| MtypeId | NVARCHAR | 10 | YES | - | 三碼代號 | - |
| AbatYn | NVARCHAR | 1 | YES | 'N' | 是否為沖帳代號 | Y/N |
| VoucherType | NVARCHAR | 10 | YES | - | 傳票格式 | - |
| BudgetYn | NVARCHAR | 1 | YES | 'N' | 是否為預算科目 | Y/N |
| OrgYn | NVARCHAR | 1 | YES | 'N' | 是否設定部門代號 | Y/N |
| ExpYear | DECIMAL | 18,2 | YES | - | 折舊攤提年限 | - |
| ResiValue | DECIMAL | 18,2 | YES | - | 殘值年限 | - |
| DepreLid | NVARCHAR | 50 | YES | - | 折舊會計科目 | 外鍵至AccountSubjects |
| AccudepreLid | NVARCHAR | 50 | YES | - | 累計折舊會計科目 | 外鍵至AccountSubjects |
| StypeYn | NVARCHAR | 1 | YES | 'Y' | 是否可輸 | Y/N |
| IfrsStypeId | NVARCHAR | 50 | YES | - | IFRS會計科目 | - |
| RocStypeId | NVARCHAR | 50 | YES | - | 集團會計科目 | - |
| SapStypeId | NVARCHAR | 50 | YES | - | SAP會計科目 | - |
| StypeClass | NVARCHAR | 50 | YES | - | 科目別 | - |
| StypeOrder | INT | - | YES | - | 排序 | - |
| Amt0 | DECIMAL | 18,2 | YES | 0 | 期初餘額 | - |
| Amt1 | DECIMAL | 18,2 | YES | 0 | 期末餘額 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢會計科目列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/account-subjects`
- **說明**: 查詢會計科目列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "StypeId",
    "sortOrder": "ASC",
    "filters": {
      "stypeId": "",
      "stypeName": "",
      "dc": "",
      "ledgerMd": "",
      "voucherType": "",
      "budgetYn": "",
      "stypeClass": ""
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
          "stypeId": "1000",
          "stypeName": "現金",
          "stypeNameE": "Cash",
          "dc": "D",
          "ledgerMd": "M",
          "stypeYn": "Y",
          "budgetYn": "N"
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

#### 3.1.2 查詢單筆會計科目
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/account-subjects/{stypeId}`
- **說明**: 根據科目代號查詢單筆會計科目資料
- **路徑參數**:
  - `stypeId`: 科目代號
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增會計科目
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/account-subjects`
- **說明**: 新增會計科目資料
- **請求格式**:
  ```json
  {
    "stypeId": "1000",
    "stypeName": "現金",
    "stypeNameE": "Cash",
    "dc": "D",
    "ledgerMd": "M",
    "mtypeId": "001",
    "abatYn": "N",
    "voucherType": "GENERAL",
    "budgetYn": "N",
    "orgYn": "N",
    "stypeYn": "Y",
    "stypeClass": "ASSET",
    "stypeOrder": 1
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "stypeId": "1000"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改會計科目
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/account-subjects/{stypeId}`
- **說明**: 修改會計科目資料
- **路徑參數**:
  - `stypeId`: 科目代號
- **請求格式**: 同新增，但 `stypeId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除會計科目
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/account-subjects/{stypeId}`
- **說明**: 刪除會計科目資料（需檢查是否有未沖帳餘額）
- **路徑參數**:
  - `stypeId`: 科目代號
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

#### 3.1.6 檢查科目代號是否存在
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/account-subjects/{stypeId}/exists`
- **說明**: 檢查科目代號是否已存在
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": true,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 檢查未沖帳餘額
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/account-subjects/{stypeId}/check-unsettled-balance`
- **說明**: 檢查科目是否有未沖帳餘額（刪除前檢查）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "hasUnsettledBalance": false,
      "balance": 0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `AccountSubjectsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/account-subjects")]
    [Authorize]
    public class AccountSubjectsController : ControllerBase
    {
        private readonly IAccountSubjectService _accountSubjectService;
        
        public AccountSubjectsController(IAccountSubjectService accountSubjectService)
        {
            _accountSubjectService = accountSubjectService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AccountSubjectDto>>>> GetAccountSubjects([FromQuery] AccountSubjectQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{stypeId}")]
        public async Task<ActionResult<ApiResponse<AccountSubjectDto>>> GetAccountSubject(string stypeId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateAccountSubject([FromBody] CreateAccountSubjectDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{stypeId}")]
        public async Task<ActionResult<ApiResponse>> UpdateAccountSubject(string stypeId, [FromBody] UpdateAccountSubjectDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{stypeId}")]
        public async Task<ActionResult<ApiResponse>> DeleteAccountSubject(string stypeId)
        {
            // 實作刪除邏輯
        }
        
        [HttpGet("{stypeId}/exists")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExists(string stypeId)
        {
            // 實作檢查邏輯
        }
        
        [HttpGet("{stypeId}/check-unsettled-balance")]
        public async Task<ActionResult<ApiResponse<UnsettledBalanceDto>>> CheckUnsettledBalance(string stypeId)
        {
            // 實作檢查邏輯
        }
    }
}
```

#### 3.2.2 Service: `AccountSubjectService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IAccountSubjectService
    {
        Task<PagedResult<AccountSubjectDto>> GetAccountSubjectsAsync(AccountSubjectQueryDto query);
        Task<AccountSubjectDto> GetAccountSubjectByIdAsync(string stypeId);
        Task<string> CreateAccountSubjectAsync(CreateAccountSubjectDto dto);
        Task UpdateAccountSubjectAsync(string stypeId, UpdateAccountSubjectDto dto);
        Task DeleteAccountSubjectAsync(string stypeId);
        Task<bool> ExistsAsync(string stypeId);
        Task<UnsettledBalanceDto> CheckUnsettledBalanceAsync(string stypeId);
    }
}
```

#### 3.2.3 Repository: `AccountSubjectRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IAccountSubjectRepository
    {
        Task<AccountSubject> GetByIdAsync(string stypeId);
        Task<PagedResult<AccountSubject>> GetPagedAsync(AccountSubjectQuery query);
        Task<AccountSubject> CreateAsync(AccountSubject accountSubject);
        Task<AccountSubject> UpdateAsync(AccountSubject accountSubject);
        Task DeleteAsync(string stypeId);
        Task<bool> ExistsAsync(string stypeId);
        Task<decimal> GetUnsettledBalanceAsync(string stypeId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 會計科目列表頁面 (`AccountSubjectList.vue`)
- **路徑**: `/accounting/account-subjects`
- **功能**: 顯示會計科目列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (AccountSubjectSearchForm)
  - 資料表格 (AccountSubjectDataTable)
  - 新增/修改對話框 (AccountSubjectDialog)
  - 刪除確認對話框

#### 4.1.2 會計科目詳細頁面 (`AccountSubjectDetail.vue`)
- **路徑**: `/accounting/account-subjects/:stypeId`
- **功能**: 顯示會計科目詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`AccountSubjectSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="科目代號">
      <el-input v-model="searchForm.stypeId" placeholder="請輸入科目代號" />
    </el-form-item>
    <el-form-item label="科目名稱">
      <el-input v-model="searchForm.stypeName" placeholder="請輸入科目名稱" />
    </el-form-item>
    <el-form-item label="借/貸">
      <el-select v-model="searchForm.dc" placeholder="請選擇借/貸">
        <el-option label="借方" value="D" />
        <el-option label="貸方" value="C" />
      </el-select>
    </el-form-item>
    <el-form-item label="統制/明細">
      <el-select v-model="searchForm.ledgerMd" placeholder="請選擇統制/明細">
        <el-option label="統制" value="L" />
        <el-option label="明細" value="M" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`AccountSubjectDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="accountSubjectList" v-loading="loading">
      <el-table-column prop="stypeId" label="科目代號" width="120" />
      <el-table-column prop="stypeName" label="科目名稱" width="200" />
      <el-table-column prop="stypeNameE" label="英文名稱" width="200" />
      <el-table-column prop="dc" label="借/貸" width="80">
        <template #default="{ row }">
          <el-tag :type="row.dc === 'D' ? 'success' : 'info'">
            {{ row.dc === 'D' ? '借方' : '貸方' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="ledgerMd" label="統制/明細" width="100">
        <template #default="{ row }">
          <el-tag :type="row.ledgerMd === 'L' ? 'warning' : 'primary'">
            {{ row.ledgerMd === 'L' ? '統制' : '明細' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="voucherType" label="傳票格式" width="120" />
      <el-table-column prop="budgetYn" label="預算科目" width="100">
        <template #default="{ row }">
          <el-tag :type="row.budgetYn === 'Y' ? 'success' : 'info'">
            {{ row.budgetYn === 'Y' ? '是' : '否' }}
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

#### 4.2.3 新增/修改對話框 (`AccountSubjectDialog.vue`)
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
          <el-form-item label="科目代號" prop="stypeId">
            <el-input v-model="form.stypeId" :disabled="isEdit" placeholder="請輸入科目代號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="科目名稱" prop="stypeName">
            <el-input v-model="form.stypeName" placeholder="請輸入科目名稱" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="英文名稱" prop="stypeNameE">
            <el-input v-model="form.stypeNameE" placeholder="請輸入英文名稱" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="借/貸" prop="dc">
            <el-select v-model="form.dc" placeholder="請選擇借/貸">
              <el-option label="借方" value="D" />
              <el-option label="貸方" value="C" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="統制/明細" prop="ledgerMd">
            <el-select v-model="form.ledgerMd" placeholder="請選擇統制/明細">
              <el-option label="統制" value="L" />
              <el-option label="明細" value="M" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="傳票格式" prop="voucherType">
            <el-select v-model="form.voucherType" placeholder="請選擇傳票格式">
              <el-option label="一般" value="GENERAL" />
              <el-option label="固定資產" value="FIXED_ASSET" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="是否為預算科目" prop="budgetYn">
            <el-radio-group v-model="form.budgetYn">
              <el-radio label="Y">是</el-radio>
              <el-radio label="N">否</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="是否可輸" prop="stypeYn">
            <el-radio-group v-model="form.stypeYn">
              <el-radio label="Y">是</el-radio>
              <el-radio label="N">否</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="IFRS會計科目" prop="ifrsStypeId">
            <el-input v-model="form.ifrsStypeId" placeholder="請輸入IFRS會計科目" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="集團會計科目" prop="rocStypeId">
            <el-input v-model="form.rocStypeId" placeholder="請輸入集團會計科目" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="SAP會計科目" prop="sapStypeId">
            <el-input v-model="form.sapStypeId" placeholder="請輸入SAP會計科目" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="科目別" prop="stypeClass">
            <el-input v-model="form.stypeClass" placeholder="請輸入科目別" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="排序" prop="stypeOrder">
            <el-input-number v-model="form.stypeOrder" :min="0" placeholder="請輸入排序" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`accountSubject.api.ts`)
```typescript
import request from '@/utils/request';

export interface AccountSubjectDto {
  tKey: number;
  stypeId: string;
  stypeName: string;
  stypeNameE?: string;
  dc?: string;
  ledgerMd?: string;
  mtypeId?: string;
  abatYn?: string;
  voucherType?: string;
  budgetYn?: string;
  orgYn?: string;
  expYear?: number;
  resiValue?: number;
  depreLid?: string;
  accudepreLid?: string;
  stypeYn?: string;
  ifrsStypeId?: string;
  rocStypeId?: string;
  sapStypeId?: string;
  stypeClass?: string;
  stypeOrder?: number;
  amt0?: number;
  amt1?: number;
}

export interface AccountSubjectQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    stypeId?: string;
    stypeName?: string;
    dc?: string;
    ledgerMd?: string;
    voucherType?: string;
    budgetYn?: string;
    stypeClass?: string;
  };
}

export interface CreateAccountSubjectDto {
  stypeId: string;
  stypeName: string;
  stypeNameE?: string;
  dc?: string;
  ledgerMd?: string;
  mtypeId?: string;
  abatYn?: string;
  voucherType?: string;
  budgetYn?: string;
  orgYn?: string;
  expYear?: number;
  resiValue?: number;
  depreLid?: string;
  accudepreLid?: string;
  stypeYn?: string;
  ifrsStypeId?: string;
  rocStypeId?: string;
  sapStypeId?: string;
  stypeClass?: string;
  stypeOrder?: number;
}

export interface UpdateAccountSubjectDto extends Omit<CreateAccountSubjectDto, 'stypeId'> {}

export interface UnsettledBalanceDto {
  hasUnsettledBalance: boolean;
  balance: number;
}

// API 函數
export const getAccountSubjectList = (query: AccountSubjectQueryDto) => {
  return request.get<ApiResponse<PagedResult<AccountSubjectDto>>>('/api/v1/account-subjects', { params: query });
};

export const getAccountSubjectById = (stypeId: string) => {
  return request.get<ApiResponse<AccountSubjectDto>>(`/api/v1/account-subjects/${stypeId}`);
};

export const createAccountSubject = (data: CreateAccountSubjectDto) => {
  return request.post<ApiResponse<string>>('/api/v1/account-subjects', data);
};

export const updateAccountSubject = (stypeId: string, data: UpdateAccountSubjectDto) => {
  return request.put<ApiResponse>(`/api/v1/account-subjects/${stypeId}`, data);
};

export const deleteAccountSubject = (stypeId: string) => {
  return request.delete<ApiResponse>(`/api/v1/account-subjects/${stypeId}`);
};

export const checkExists = (stypeId: string) => {
  return request.get<ApiResponse<boolean>>(`/api/v1/account-subjects/${stypeId}/exists`);
};

export const checkUnsettledBalance = (stypeId: string) => {
  return request.get<ApiResponse<UnsettledBalanceDto>>(`/api/v1/account-subjects/${stypeId}/check-unsettled-balance`);
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
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（科目代號唯一性、固定資產欄位驗證、未沖帳餘額檢查等）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
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

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 必須驗證科目代號的唯一性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 科目代號必須唯一
- 必填欄位必須驗證
- 借貸方向必須在允許範圍內
- 統制/明細必須在允許範圍內
- 固定資產相關欄位必須驗證

### 6.4 業務邏輯
- 刪除科目前必須檢查是否有未沖帳餘額
- 統制科目與明細科目的階層關係必須驗證
- 折舊相關科目必須驗證
- IFRS、ROC、SAP對應關係必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增會計科目成功
- [ ] 新增會計科目失敗 (重複代號)
- [ ] 修改會計科目成功
- [ ] 修改會計科目失敗 (不存在)
- [ ] 刪除會計科目成功
- [ ] 刪除會計科目失敗 (有未沖帳餘額)
- [ ] 查詢會計科目列表成功
- [ ] 查詢單筆會計科目成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 未沖帳餘額檢查測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN110_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN110_PR.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSN000/SYSN110.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


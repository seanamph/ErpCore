# SYSF110-SYSF140 - 合同資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSF110-SYSF140 系列
- **功能名稱**: 合同資料維護系列
- **功能描述**: 提供合同資料的新增、修改、刪除、查詢功能，包含合同編號、合同類型、廠商、簽約日期、生效日期、到期日期、合同金額、合同狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF110_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF120_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF120_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF130_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF130_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF140_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF140_FU.ASP` (修改)

### 1.2 業務需求
- 管理合同基本資料
- 支援合同類型管理（商場招商合約、委外廠商合約、塔樓招商合約）
- 支援廠商選擇
- 支援合同版本管理
- 支援合同狀態管理（草稿、審核中、已生效、已到期、已終止）
- 支援合同金額計算
- 支援合同審核流程
- 支援合同報表列印
- 支援合同歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Contracts` (合同主檔)

```sql
CREATE TABLE [dbo].[Contracts] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號 (CONTRACT_ID)
    [ContractType] NVARCHAR(20) NOT NULL, -- 合同類型 (CTS_TYPE, 1:商場招商合約, 2:委外廠商合約, 3:塔樓招商合約)
    [Version] INT NOT NULL DEFAULT 1, -- 版本號 (VERSION)
    [VendorId] NVARCHAR(50) NOT NULL, -- 廠商代碼 (VENDOR_ID)
    [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME)
    [SignDate] DATETIME2 NULL, -- 簽約日期 (SIGN_DATE)
    [EffectiveDate] DATETIME2 NULL, -- 生效日期 (EFFECTIVE_DATE)
    [ExpiryDate] DATETIME2 NULL, -- 到期日期 (EXPIRY_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, P:審核中, A:已生效, E:已到期, T:已終止)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [LocationId] NVARCHAR(50) NULL, -- 位置編號 (LOCATION_ID)
    [RecruitId] NVARCHAR(50) NULL, -- 招商編號 (RECRUIT_ID)
    [Attorney] NVARCHAR(100) NULL, -- 委託人 (ATTORNEY)
    [Salutation] NVARCHAR(50) NULL, -- 稱謂 (SALUTATION)
    [VerStatus] NVARCHAR(10) NULL, -- 版本狀態 (VER_STATUS, 0:覆蓋原版本, 1:產生新版本, 2:產生正式合約)
    [AgmStatus] NVARCHAR(10) NULL, -- 協議狀態 (AGM_STATUS)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Contracts_ContractId_Version] UNIQUE ([ContractId], [Version])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Contracts_ContractId] ON [dbo].[Contracts] ([ContractId]);
CREATE NONCLUSTERED INDEX [IX_Contracts_VendorId] ON [dbo].[Contracts] ([VendorId]);
CREATE NONCLUSTERED INDEX [IX_Contracts_Status] ON [dbo].[Contracts] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Contracts_ContractType] ON [dbo].[Contracts] ([ContractType]);
CREATE NONCLUSTERED INDEX [IX_Contracts_EffectiveDate] ON [dbo].[Contracts] ([EffectiveDate]);
CREATE NONCLUSTERED INDEX [IX_Contracts_ExpiryDate] ON [dbo].[Contracts] ([ExpiryDate]);
```

### 2.2 相關資料表

#### 2.2.1 `ContractTerms` - 合同條款
```sql
CREATE TABLE [dbo].[ContractTerms] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號
    [Version] INT NOT NULL, -- 版本號
    [TermType] NVARCHAR(50) NULL, -- 條款類型 (TERM_TYPE)
    [TermContent] NVARCHAR(MAX) NULL, -- 條款內容 (TERM_CONTENT)
    [TermOrder] INT NULL, -- 條款順序 (TERM_ORDER)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ContractTerms_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ContractTerms_ContractId] ON [dbo].[ContractTerms] ([ContractId], [Version]);
```

#### 2.2.2 `ContractPenalties` - 合同罰則
```sql
CREATE TABLE [dbo].[ContractPenalties] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號
    [Version] INT NOT NULL, -- 版本號
    [PenaltyType] NVARCHAR(50) NULL, -- 罰則類型 (PENALTY_TYPE)
    [PenaltyAmount] DECIMAL(18, 4) NULL, -- 罰則金額 (PENALTY_AMT)
    [PenaltyRate] DECIMAL(5, 2) NULL, -- 罰則比率 (PENALTY_RATE)
    [PenaltyDesc] NVARCHAR(500) NULL, -- 罰則說明 (PENALTY_DESC)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ContractPenalties_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ContractPenalties_ContractId] ON [dbo].[ContractPenalties] ([ContractId], [Version]);
```

#### 2.2.3 `ContractAccounting` - 合同會計分類
```sql
CREATE TABLE [dbo].[ContractAccounting] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號
    [Version] INT NOT NULL, -- 版本號
    [AccountCode] NVARCHAR(50) NULL, -- 會計科目代碼 (ACCOUNT_CODE)
    [AccountName] NVARCHAR(200) NULL, -- 會計科目名稱 (ACCOUNT_NAME)
    [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ContractAccounting_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ContractAccounting_ContractId] ON [dbo].[ContractAccounting] ([ContractId], [Version]);
```

### 2.3 資料字典

#### 2.3.1 Contracts 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ContractId | NVARCHAR | 50 | NO | - | 合同編號 | 唯一，CONTRACT_ID |
| ContractType | NVARCHAR | 20 | NO | - | 合同類型 | 1:商場招商合約, 2:委外廠商合約, 3:塔樓招商合約 |
| Version | INT | - | NO | 1 | 版本號 | VERSION |
| VendorId | NVARCHAR | 50 | NO | - | 廠商代碼 | 外鍵至廠商表 |
| VendorName | NVARCHAR | 200 | YES | - | 廠商名稱 | - |
| SignDate | DATETIME2 | - | YES | - | 簽約日期 | - |
| EffectiveDate | DATETIME2 | - | YES | - | 生效日期 | - |
| ExpiryDate | DATETIME2 | - | YES | - | 到期日期 | - |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, P:審核中, A:已生效, E:已到期, T:已終止 |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |
| LocationId | NVARCHAR | 50 | YES | - | 位置編號 | - |
| RecruitId | NVARCHAR | 50 | YES | - | 招商編號 | - |
| Attorney | NVARCHAR | 100 | YES | - | 委託人 | - |
| Salutation | NVARCHAR | 50 | YES | - | 稱謂 | - |
| VerStatus | NVARCHAR | 10 | YES | - | 版本狀態 | 0:覆蓋原版本, 1:產生新版本, 2:產生正式合約 |
| AgmStatus | NVARCHAR | 10 | YES | - | 協議狀態 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢合同列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/contracts`
- **說明**: 查詢合同列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ContractId",
    "sortOrder": "ASC",
    "filters": {
      "contractId": "",
      "contractType": "",
      "vendorId": "",
      "status": "",
      "effectiveDateFrom": "",
      "effectiveDateTo": ""
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
          "contractId": "CT001",
          "contractType": "2",
          "version": 1,
          "vendorId": "V001",
          "vendorName": "廠商A",
          "signDate": "2024-01-01",
          "effectiveDate": "2024-01-01",
          "expiryDate": "2024-12-31",
          "status": "A",
          "totalAmount": 1000000.00
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

#### 3.1.2 查詢單筆合同
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/contracts/{contractId}/{version}`
- **說明**: 根據合同編號和版本號查詢單筆合同資料
- **路徑參數**:
  - `contractId`: 合同編號
  - `version`: 版本號
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增合同
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/contracts`
- **說明**: 新增合同資料
- **請求格式**:
  ```json
  {
    "contractId": "CT001",
    "contractType": "2",
    "version": 1,
    "vendorId": "V001",
    "vendorName": "廠商A",
    "signDate": "2024-01-01",
    "effectiveDate": "2024-01-01",
    "expiryDate": "2024-12-31",
    "status": "D",
    "totalAmount": 1000000.00,
    "currencyId": "TWD",
    "exchangeRate": 1.0,
    "locationId": "LOC001",
    "recruitId": "REC001",
    "attorney": "委託人",
    "salutation": "先生",
    "verStatus": "1",
    "agmStatus": "",
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
      "tKey": 1,
      "contractId": "CT001",
      "version": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改合同
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/contracts/{contractId}/{version}`
- **說明**: 修改合同資料
- **路徑參數**:
  - `contractId`: 合同編號
  - `version`: 版本號
- **請求格式**: 同新增，但 `contractId` 和 `version` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除合同
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/contracts/{contractId}/{version}`
- **說明**: 刪除合同資料（軟刪除或硬刪除）
- **路徑參數**:
  - `contractId`: 合同編號
  - `version`: 版本號
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

#### 3.1.6 批次刪除合同
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/contracts/batch`
- **說明**: 批次刪除多筆合同
- **請求格式**:
  ```json
  {
    "contracts": [
      {"contractId": "CT001", "version": 1},
      {"contractId": "CT002", "version": 1}
    ]
  }
  ```

#### 3.1.7 審核合同
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/contracts/{contractId}/{version}/approve`
- **說明**: 審核合同
- **請求格式**:
  ```json
  {
    "approveUserId": "U001",
    "approveDate": "2024-01-01",
    "status": "A"
  }
  ```

#### 3.1.8 產生新版本
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/contracts/{contractId}/{version}/new-version`
- **說明**: 產生合同新版本
- **請求格式**:
  ```json
  {
    "verStatus": "1"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `ContractsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/contracts")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        
        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ContractDto>>>> GetContracts([FromQuery] ContractQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{contractId}/{version}")]
        public async Task<ActionResult<ApiResponse<ContractDto>>> GetContract(string contractId, int version)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ContractResultDto>>> CreateContract([FromBody] CreateContractDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{contractId}/{version}")]
        public async Task<ActionResult<ApiResponse>> UpdateContract(string contractId, int version, [FromBody] UpdateContractDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{contractId}/{version}")]
        public async Task<ActionResult<ApiResponse>> DeleteContract(string contractId, int version)
        {
            // 實作刪除邏輯
        }
        
        [HttpPut("{contractId}/{version}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveContract(string contractId, int version, [FromBody] ApproveContractDto dto)
        {
            // 實作審核邏輯
        }
        
        [HttpPost("{contractId}/{version}/new-version")]
        public async Task<ActionResult<ApiResponse<ContractResultDto>>> CreateNewVersion(string contractId, int version, [FromBody] NewVersionDto dto)
        {
            // 實作產生新版本邏輯
        }
    }
}
```

#### 3.2.2 Service: `ContractService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IContractService
    {
        Task<PagedResult<ContractDto>> GetContractsAsync(ContractQueryDto query);
        Task<ContractDto> GetContractByIdAsync(string contractId, int version);
        Task<ContractResultDto> CreateContractAsync(CreateContractDto dto);
        Task UpdateContractAsync(string contractId, int version, UpdateContractDto dto);
        Task DeleteContractAsync(string contractId, int version);
        Task ApproveContractAsync(string contractId, int version, ApproveContractDto dto);
        Task<ContractResultDto> CreateNewVersionAsync(string contractId, int version, NewVersionDto dto);
    }
}
```

#### 3.2.3 Repository: `ContractRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IContractRepository
    {
        Task<Contract> GetByIdAsync(string contractId, int version);
        Task<PagedResult<Contract>> GetPagedAsync(ContractQuery query);
        Task<Contract> CreateAsync(Contract contract);
        Task<Contract> UpdateAsync(Contract contract);
        Task DeleteAsync(string contractId, int version);
        Task<bool> ExistsAsync(string contractId, int version);
        Task<int> GetNextVersionAsync(string contractId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 合同列表頁面 (`ContractList.vue`)
- **路徑**: `/contracts`
- **功能**: 顯示合同列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (ContractSearchForm)
  - 資料表格 (ContractDataTable)
  - 新增/修改對話框 (ContractDialog)
  - 刪除確認對話框

#### 4.1.2 合同詳細頁面 (`ContractDetail.vue`)
- **路徑**: `/contracts/:contractId/:version`
- **功能**: 顯示合同詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ContractSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="合同編號">
      <el-input v-model="searchForm.contractId" placeholder="請輸入合同編號" />
    </el-form-item>
    <el-form-item label="合同類型">
      <el-select v-model="searchForm.contractType" placeholder="請選擇合同類型">
        <el-option label="商場招商合約" value="1" />
        <el-option label="委外廠商合約" value="2" />
        <el-option label="塔樓招商合約" value="3" />
      </el-select>
    </el-form-item>
    <el-form-item label="廠商">
      <el-input v-model="searchForm.vendorId" placeholder="請輸入廠商代碼" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="草稿" value="D" />
        <el-option label="審核中" value="P" />
        <el-option label="已生效" value="A" />
        <el-option label="已到期" value="E" />
        <el-option label="已終止" value="T" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`ContractDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="contractList" v-loading="loading">
      <el-table-column prop="contractId" label="合同編號" width="120" />
      <el-table-column prop="version" label="版本" width="80" />
      <el-table-column prop="contractType" label="合同類型" width="120">
        <template #default="{ row }">
          {{ getContractTypeText(row.contractType) }}
        </template>
      </el-table-column>
      <el-table-column prop="vendorName" label="廠商名稱" width="200" />
      <el-table-column prop="effectiveDate" label="生效日期" width="120" />
      <el-table-column prop="expiryDate" label="到期日期" width="120" />
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="totalAmount" label="總金額" width="120" />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="info" size="small" @click="handleNewVersion(row)">新版本</el-button>
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

#### 4.2.3 新增/修改對話框 (`ContractDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="合同編號" prop="contractId">
            <el-input v-model="form.contractId" :disabled="isEdit" placeholder="請輸入合同編號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="版本號" prop="version">
            <el-input-number v-model="form.version" :disabled="isEdit" :min="1" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="合同類型" prop="contractType">
            <el-select v-model="form.contractType" placeholder="請選擇合同類型">
              <el-option label="商場招商合約" value="1" />
              <el-option label="委外廠商合約" value="2" />
              <el-option label="塔樓招商合約" value="3" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="狀態" prop="status">
            <el-select v-model="form.status" placeholder="請選擇狀態">
              <el-option label="草稿" value="D" />
              <el-option label="審核中" value="P" />
              <el-option label="已生效" value="A" />
              <el-option label="已到期" value="E" />
              <el-option label="已終止" value="T" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="廠商代碼" prop="vendorId">
            <el-input v-model="form.vendorId" placeholder="請輸入廠商代碼" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="廠商名稱" prop="vendorName">
            <el-input v-model="form.vendorName" placeholder="請輸入廠商名稱" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="簽約日期" prop="signDate">
            <el-date-picker v-model="form.signDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="生效日期" prop="effectiveDate">
            <el-date-picker v-model="form.effectiveDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="到期日期" prop="expiryDate">
            <el-date-picker v-model="form.expiryDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="總金額" prop="totalAmount">
            <el-input-number v-model="form.totalAmount" :precision="2" :min="0" />
          </el-form-item>
        </el-col>
      </el-row>
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

### 4.3 API 呼叫 (`contract.api.ts`)
```typescript
import request from '@/utils/request';

export interface ContractDto {
  tKey: number;
  contractId: string;
  contractType: string;
  version: number;
  vendorId: string;
  vendorName?: string;
  signDate?: string;
  effectiveDate?: string;
  expiryDate?: string;
  status: string;
  totalAmount?: number;
  currencyId?: string;
  exchangeRate?: number;
  locationId?: string;
  recruitId?: string;
  attorney?: string;
  salutation?: string;
  verStatus?: string;
  agmStatus?: string;
  memo?: string;
}

export interface ContractQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    contractId?: string;
    contractType?: string;
    vendorId?: string;
    status?: string;
    effectiveDateFrom?: string;
    effectiveDateTo?: string;
  };
}

export interface CreateContractDto {
  contractId: string;
  contractType: string;
  version: number;
  vendorId: string;
  vendorName?: string;
  signDate?: string;
  effectiveDate?: string;
  expiryDate?: string;
  status: string;
  totalAmount?: number;
  currencyId?: string;
  exchangeRate?: number;
  locationId?: string;
  recruitId?: string;
  attorney?: string;
  salutation?: string;
  verStatus?: string;
  agmStatus?: string;
  memo?: string;
}

export interface UpdateContractDto extends Omit<CreateContractDto, 'contractId' | 'version'> {}

// API 函數
export const getContractList = (query: ContractQueryDto) => {
  return request.get<ApiResponse<PagedResult<ContractDto>>>('/api/v1/contracts', { params: query });
};

export const getContractById = (contractId: string, version: number) => {
  return request.get<ApiResponse<ContractDto>>(`/api/v1/contracts/${contractId}/${version}`);
};

export const createContract = (data: CreateContractDto) => {
  return request.post<ApiResponse<ContractResultDto>>('/api/v1/contracts', data);
};

export const updateContract = (contractId: string, version: number, data: UpdateContractDto) => {
  return request.put<ApiResponse>(`/api/v1/contracts/${contractId}/${version}`, data);
};

export const deleteContract = (contractId: string, version: number) => {
  return request.delete<ApiResponse>(`/api/v1/contracts/${contractId}/${version}`);
};

export const approveContract = (contractId: string, version: number, data: ApproveContractDto) => {
  return request.put<ApiResponse>(`/api/v1/contracts/${contractId}/${version}/approve`, data);
};

export const createNewVersion = (contractId: string, version: number, data: NewVersionDto) => {
  return request.post<ApiResponse<ContractResultDto>>(`/api/v1/contracts/${contractId}/${version}/new-version`, data);
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
- [ ] 版本管理邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 版本管理功能
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
- 合同資料必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 審核流程必須記錄操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 合同編號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證（生效日期必須早於到期日期）
- 狀態值必須在允許範圍內
- 版本號必須遞增

### 6.4 業務邏輯
- 刪除合同前必須檢查是否有相關資料
- 產生新版本時必須複製原版本資料
- 審核合同時必須檢查必填欄位
- 合同到期時必須自動更新狀態
- 版本管理必須支援覆蓋原版本或產生新版本

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增合同成功
- [ ] 新增合同失敗 (重複編號)
- [ ] 修改合同成功
- [ ] 修改合同失敗 (不存在)
- [ ] 刪除合同成功
- [ ] 查詢合同列表成功
- [ ] 查詢單筆合同成功
- [ ] 產生新版本成功
- [ ] 審核合同成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 版本管理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF110_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF120_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF120_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF130_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF130_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF140_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSF000/SYSF140_FU.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSF000/SYSF110.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

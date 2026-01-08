# SYSK110-SYSK150 - 憑證資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSK110-SYSK150 系列
- **功能名稱**: 憑證資料維護系列
- **功能描述**: 提供憑證資料的新增、修改、刪除、查詢功能，包含憑證編號、憑證日期、憑證類型、憑證狀態、憑證金額、憑證明細等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK110_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FS.ASP` (排序)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK120_*.ASP` (憑證類型設定相關)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK130_*.ASP` (憑證明細相關)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK140_*.ASP` (憑證審核相關)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK150_*.ASP` (憑證擴展功能)

### 1.2 業務需求
- 管理憑證基本資料
- 支援憑證類型設定（傳票、收據、發票等）
- 支援憑證狀態管理（草稿、已送出、已審核、已取消、已結案）
- 支援憑證明細維護
- 支援憑證金額計算
- 支援憑證審核流程
- 支援多店別管理
- 支援憑證報表列印
- 支援憑證歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Vouchers` (憑證主檔)

```sql
CREATE TABLE [dbo].[Vouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證編號 (VCH_NO)
    [VoucherDate] DATETIME2 NOT NULL, -- 憑證日期 (VCH_DATE)
    [VoucherType] NVARCHAR(20) NOT NULL, -- 憑證類型 (VCH_TYPE, V:傳票, R:收據, I:發票)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, X:已取消, C:已結案)
    [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
    [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
    [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
    [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
    [TotalDebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方總額 (TOTAL_DEBIT_AMT)
    [TotalCreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方總額 (TOTAL_CREDIT_AMT)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Vouchers_VoucherId] UNIQUE ([VoucherId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherId] ON [dbo].[Vouchers] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_ShopId] ON [dbo].[Vouchers] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_Status] ON [dbo].[Vouchers] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherDate] ON [dbo].[Vouchers] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherType] ON [dbo].[Vouchers] ([VoucherType]);
```

### 2.2 相關資料表

#### 2.2.1 `VoucherDetails` - 憑證明細
```sql
CREATE TABLE [dbo].[VoucherDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證編號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [AccountId] NVARCHAR(50) NOT NULL, -- 會計科目代碼 (ACCOUNT_ID)
    [DebitAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 借方金額 (DEBIT_AMT)
    [CreditAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 貸方金額 (CREDIT_AMT)
    [Description] NVARCHAR(500) NULL, -- 摘要 (DESCRIPTION)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_VoucherDetails_Vouchers] FOREIGN KEY ([VoucherId]) REFERENCES [dbo].[Vouchers] ([VoucherId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherId] ON [dbo].[VoucherDetails] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_AccountId] ON [dbo].[VoucherDetails] ([AccountId]);
```

#### 2.2.2 `VoucherTypes` - 憑證類型設定
```sql
CREATE TABLE [dbo].[VoucherTypes] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTypeId] NVARCHAR(20) NOT NULL, -- 憑證類型代碼
    [VoucherTypeName] NVARCHAR(100) NOT NULL, -- 憑證類型名稱
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_VoucherTypes_VoucherTypeId] UNIQUE ([VoucherTypeId])
);
```

### 2.3 資料字典

#### 2.3.1 Vouchers 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | NO | - | 憑證編號 | 唯一，VCH_NO |
| VoucherDate | DATETIME2 | - | NO | - | 憑證日期 | VCH_DATE |
| VoucherType | NVARCHAR | 20 | NO | - | 憑證類型 | V:傳票, R:收據, I:發票 |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, S:已送出, A:已審核, X:已取消, C:已結案 |
| ApplyUserId | NVARCHAR | 50 | YES | - | 申請人員 | 外鍵至使用者表 |
| ApplyDate | DATETIME2 | - | YES | - | 申請日期 | - |
| ApproveUserId | NVARCHAR | 50 | YES | - | 審核人員 | 外鍵至使用者表 |
| ApproveDate | DATETIME2 | - | YES | - | 審核日期 | - |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalDebitAmount | DECIMAL | 18,4 | YES | 0 | 借方總額 | - |
| TotalCreditAmount | DECIMAL | 18,4 | YES | 0 | 貸方總額 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | - |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |

#### 2.3.2 VoucherDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | NO | - | 憑證編號 | 外鍵至Vouchers |
| LineNum | INT | - | NO | - | 行號 | - |
| AccountId | NVARCHAR | 50 | NO | - | 會計科目代碼 | 外鍵至會計科目表 |
| DebitAmount | DECIMAL | 18,4 | YES | 0 | 借方金額 | - |
| CreditAmount | DECIMAL | 18,4 | YES | 0 | 貸方金額 | - |
| Description | NVARCHAR | 500 | YES | - | 摘要 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢憑證列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers`
- **說明**: 查詢憑證列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherId",
    "sortOrder": "ASC",
    "filters": {
      "voucherId": "",
      "voucherType": "",
      "shopId": "",
      "status": "",
      "voucherDateFrom": "",
      "voucherDateTo": ""
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
          "voucherId": "VCH001",
          "voucherDate": "2024-01-01",
          "voucherType": "V",
          "voucherTypeName": "傳票",
          "shopId": "SHOP001",
          "status": "A",
          "totalAmount": 10000.00,
          "totalDebitAmount": 10000.00,
          "totalCreditAmount": 10000.00,
          "applyUserId": "U001",
          "applyDate": "2024-01-01T10:00:00"
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

#### 3.1.2 查詢單筆憑證
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 根據憑證編號查詢單筆憑證資料（含明細）
- **路徑參數**:
  - `voucherId`: 憑證編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "voucherId": "VCH001",
      "voucherDate": "2024-01-01",
      "voucherType": "V",
      "shopId": "SHOP001",
      "status": "A",
      "totalAmount": 10000.00,
      "totalDebitAmount": 10000.00,
      "totalCreditAmount": 10000.00,
      "memo": "備註",
      "details": [
        {
          "lineNum": 1,
          "accountId": "ACC001",
          "accountName": "現金",
          "debitAmount": 10000.00,
          "creditAmount": 0.00,
          "description": "摘要"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增憑證
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers`
- **說明**: 新增憑證資料（含明細）
- **請求格式**:
  ```json
  {
    "voucherId": "VCH001",
    "voucherDate": "2024-01-01",
    "voucherType": "V",
    "shopId": "SHOP001",
    "status": "D",
    "memo": "備註",
    "details": [
      {
        "lineNum": 1,
        "accountId": "ACC001",
        "debitAmount": 10000.00,
        "creditAmount": 0.00,
        "description": "摘要"
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
      "voucherId": "VCH001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改憑證
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 修改憑證資料（含明細）
- **路徑參數**:
  - `voucherId`: 憑證編號
- **請求格式**: 同新增，但 `voucherId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除憑證
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 刪除憑證資料（軟刪除或硬刪除）
- **路徑參數**:
  - `voucherId`: 憑證編號
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

#### 3.1.6 批次刪除憑證
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/vouchers/batch`
- **說明**: 批次刪除多筆憑證
- **請求格式**:
  ```json
  {
    "voucherIds": ["VCH001", "VCH002", "VCH003"]
  }
  ```

#### 3.1.7 審核憑證
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vouchers/{voucherId}/approve`
- **說明**: 審核憑證
- **請求格式**:
  ```json
  {
    "approveUserId": "U002",
    "memo": "審核通過"
  }
  ```

#### 3.1.8 取消審核憑證
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vouchers/{voucherId}/cancel`
- **說明**: 取消審核憑證
- **請求格式**:
  ```json
  {
    "memo": "取消原因"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `VouchersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/vouchers")]
    [Authorize]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        
        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVouchers([FromQuery] VoucherQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{voucherId}")]
        public async Task<ActionResult<ApiResponse<VoucherDto>>> GetVoucher(string voucherId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> UpdateVoucher(string voucherId, [FromBody] UpdateVoucherDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoucher(string voucherId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPut("{voucherId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveVoucher(string voucherId, [FromBody] ApproveVoucherDto dto)
        {
            // 實作審核邏輯
        }
        
        [HttpPut("{voucherId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelVoucher(string voucherId, [FromBody] CancelVoucherDto dto)
        {
            // 實作取消邏輯
        }
    }
}
```

#### 3.2.2 Service: `VoucherService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IVoucherService
    {
        Task<PagedResult<VoucherDto>> GetVouchersAsync(VoucherQueryDto query);
        Task<VoucherDto> GetVoucherByIdAsync(string voucherId);
        Task<string> CreateVoucherAsync(CreateVoucherDto dto);
        Task UpdateVoucherAsync(string voucherId, UpdateVoucherDto dto);
        Task DeleteVoucherAsync(string voucherId);
        Task ApproveVoucherAsync(string voucherId, ApproveVoucherDto dto);
        Task CancelVoucherAsync(string voucherId, CancelVoucherDto dto);
    }
}
```

#### 3.2.3 Repository: `VoucherRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IVoucherRepository
    {
        Task<Voucher> GetByIdAsync(string voucherId);
        Task<PagedResult<Voucher>> GetPagedAsync(VoucherQuery query);
        Task<Voucher> CreateAsync(Voucher voucher);
        Task<Voucher> UpdateAsync(Voucher voucher);
        Task DeleteAsync(string voucherId);
        Task<bool> ExistsAsync(string voucherId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 憑證列表頁面 (`VoucherList.vue`)
- **路徑**: `/vouchers`
- **功能**: 顯示憑證列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (VoucherSearchForm)
  - 資料表格 (VoucherDataTable)
  - 新增/修改對話框 (VoucherDialog)
  - 刪除確認對話框

#### 4.1.2 憑證詳細頁面 (`VoucherDetail.vue`)
- **路徑**: `/vouchers/:voucherId`
- **功能**: 顯示憑證詳細資料（含明細），支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`VoucherSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="憑證編號">
      <el-input v-model="searchForm.voucherId" placeholder="請輸入憑證編號" />
    </el-form-item>
    <el-form-item label="憑證類型">
      <el-select v-model="searchForm.voucherType" placeholder="請選擇憑證類型">
        <el-option label="傳票" value="V" />
        <el-option label="收據" value="R" />
        <el-option label="發票" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item label="分店">
      <el-select v-model="searchForm.shopId" placeholder="請選擇分店">
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="草稿" value="D" />
        <el-option label="已送出" value="S" />
        <el-option label="已審核" value="A" />
        <el-option label="已取消" value="X" />
        <el-option label="已結案" value="C" />
      </el-select>
    </el-form-item>
    <el-form-item label="憑證日期">
      <el-date-picker
        v-model="searchForm.voucherDateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
      />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`VoucherDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="voucherList" v-loading="loading">
      <el-table-column prop="voucherId" label="憑證編號" width="120" />
      <el-table-column prop="voucherDate" label="憑證日期" width="120" />
      <el-table-column prop="voucherTypeName" label="憑證類型" width="100" />
      <el-table-column prop="shopName" label="分店" width="150" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="totalAmount" label="總金額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.totalAmount) }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button v-if="row.status === 'S'" type="success" size="small" @click="handleApprove(row)">審核</el-button>
          <el-button v-if="row.status === 'A'" type="warning" size="small" @click="handleCancel(row)">取消</el-button>
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

#### 4.2.3 新增/修改對話框 (`VoucherDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="1000px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="憑證編號" prop="voucherId">
            <el-input v-model="form.voucherId" :disabled="isEdit" placeholder="請輸入憑證編號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="憑證日期" prop="voucherDate">
            <el-date-picker v-model="form.voucherDate" type="date" placeholder="請選擇日期" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="憑證類型" prop="voucherType">
            <el-select v-model="form.voucherType" placeholder="請選擇憑證類型">
              <el-option label="傳票" value="V" />
              <el-option label="收據" value="R" />
              <el-option label="發票" value="I" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="分店" prop="shopId">
            <el-select v-model="form.shopId" placeholder="請選擇分店">
              <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="memo">
        <el-input v-model="form.memo" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
      
      <!-- 憑證明細 -->
      <el-form-item label="憑證明細">
        <el-table :data="form.details" border>
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column prop="accountId" label="會計科目" width="150">
            <template #default="{ row, $index }">
              <el-select v-model="row.accountId" placeholder="請選擇會計科目">
                <el-option v-for="account in accountList" :key="account.accountId" :label="account.accountName" :value="account.accountId" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="debitAmount" label="借方金額" width="150">
            <template #default="{ row }">
              <el-input-number v-model="row.debitAmount" :precision="2" :min="0" />
            </template>
          </el-table-column>
          <el-table-column prop="creditAmount" label="貸方金額" width="150">
            <template #default="{ row }">
              <el-input-number v-model="row.creditAmount" :precision="2" :min="0" />
            </template>
          </el-table-column>
          <el-table-column prop="description" label="摘要" width="200">
            <template #default="{ row }">
              <el-input v-model="row.description" placeholder="請輸入摘要" />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="handleAddDetail" style="margin-top: 10px">新增明細</el-button>
      </el-form-item>
      
      <el-form-item label="合計">
        <el-row :gutter="20">
          <el-col :span="8">
            <span>借方總額: {{ formatCurrency(totalDebitAmount) }}</span>
          </el-col>
          <el-col :span="8">
            <span>貸方總額: {{ formatCurrency(totalCreditAmount) }}</span>
          </el-col>
          <el-col :span="8">
            <span :class="{'text-danger': totalDebitAmount !== totalCreditAmount}">
              差額: {{ formatCurrency(totalDebitAmount - totalCreditAmount) }}
            </span>
          </el-col>
        </el-row>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`voucher.api.ts`)
```typescript
import request from '@/utils/request';

export interface VoucherDto {
  voucherId: string;
  voucherDate: string;
  voucherType: string;
  voucherTypeName?: string;
  shopId: string;
  shopName?: string;
  status: string;
  totalAmount: number;
  totalDebitAmount: number;
  totalCreditAmount: number;
  memo?: string;
  applyUserId?: string;
  applyDate?: string;
  approveUserId?: string;
  approveDate?: string;
  details?: VoucherDetailDto[];
}

export interface VoucherDetailDto {
  lineNum: number;
  accountId: string;
  accountName?: string;
  debitAmount: number;
  creditAmount: number;
  description?: string;
  memo?: string;
}

export interface VoucherQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    voucherId?: string;
    voucherType?: string;
    shopId?: string;
    status?: string;
    voucherDateFrom?: string;
    voucherDateTo?: string;
  };
}

export interface CreateVoucherDto {
  voucherId: string;
  voucherDate: string;
  voucherType: string;
  shopId: string;
  status: string;
  memo?: string;
  details: CreateVoucherDetailDto[];
}

export interface CreateVoucherDetailDto {
  lineNum: number;
  accountId: string;
  debitAmount: number;
  creditAmount: number;
  description?: string;
  memo?: string;
}

export interface UpdateVoucherDto extends Omit<CreateVoucherDto, 'voucherId'> {}

// API 函數
export const getVoucherList = (query: VoucherQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/vouchers', { params: query });
};

export const getVoucherById = (voucherId: string) => {
  return request.get<ApiResponse<VoucherDto>>(`/api/v1/vouchers/${voucherId}`);
};

export const createVoucher = (data: CreateVoucherDto) => {
  return request.post<ApiResponse<string>>('/api/v1/vouchers', data);
};

export const updateVoucher = (voucherId: string, data: UpdateVoucherDto) => {
  return request.put<ApiResponse>(`/api/v1/vouchers/${voucherId}`, data);
};

export const deleteVoucher = (voucherId: string) => {
  return request.delete<ApiResponse>(`/api/v1/vouchers/${voucherId}`);
};

export const approveVoucher = (voucherId: string, data: { approveUserId: string; memo?: string }) => {
  return request.put<ApiResponse>(`/api/v1/vouchers/${voucherId}/approve`, data);
};

export const cancelVoucher = (voucherId: string, data: { memo?: string }) => {
  return request.put<ApiResponse>(`/api/v1/vouchers/${voucherId}/cancel`, data);
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
- [ ] 驗證邏輯實作（借方貸方平衡檢查）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 憑證明細表格開發
- [ ] 表單驗證
- [ ] 借方貸方平衡檢查
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
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 審核操作必須記錄操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 憑證編號必須唯一
- 必填欄位必須驗證
- 借方總額必須等於貸方總額（會計平衡）
- 狀態值必須在允許範圍內
- 憑證日期必須在合理範圍內

### 6.4 業務邏輯
- 刪除憑證前必須檢查是否有相關資料
- 已審核的憑證不可修改或刪除
- 憑證明細至少需有一筆
- 借方金額和貸方金額不能同時為0
- 審核操作必須記錄審核人員和審核時間

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增憑證成功
- [ ] 新增憑證失敗 (重複編號)
- [ ] 新增憑證失敗 (借方貸方不平衡)
- [ ] 修改憑證成功
- [ ] 修改憑證失敗 (不存在)
- [ ] 修改憑證失敗 (已審核)
- [ ] 刪除憑證成功
- [ ] 刪除憑證失敗 (已審核)
- [ ] 查詢憑證列表成功
- [ ] 查詢單筆憑證成功
- [ ] 審核憑證成功
- [ ] 取消審核憑證成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 會計平衡檢查測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSK000/SYSK110_FB.ASP` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSK000/SYSK110_PR.ASP` (報表)

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSK000/SYSK110.xsd` (如果存在)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


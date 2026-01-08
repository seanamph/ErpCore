# SYST311-SYST352 - 交易資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST311-SYST352 系列
- **功能名稱**: 交易資料維護系列
- **功能描述**: 提供傳票確認、過帳、反過帳、年結處理等交易資料維護功能，包含傳票狀態管理、批次處理、查詢報表等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST311_FB.ASP` (瀏覽 - 傳票確認作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST311_FQ.ASP` (查詢 - 傳票確認作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST311_FU.ASP` (更新 - 傳票確認)
  - `WEB/IMS_CORE/ASP/SYST000/SYST311_FD.ASP` (刪除 - 傳票確認作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST311_PR.ASP` (報表 - 傳票確認作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST312_FB.ASP` (瀏覽 - 傳票確認作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST312_FQ.ASP` (查詢 - 傳票確認作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST312_FU.ASP` (更新 - 傳票確認)
  - `WEB/IMS_CORE/ASP/SYST000/SYST312_FD.ASP` (刪除 - 傳票確認作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST321_FB.ASP` (瀏覽 - 傳票過帳)
  - `WEB/IMS_CORE/ASP/SYST000/SYST321_FQ.ASP` (查詢 - 傳票過帳)
  - `WEB/IMS_CORE/ASP/SYST000/SYST322_FB.ASP` (瀏覽 - 傳票過帳)
  - `WEB/IMS_CORE/ASP/SYST000/SYST322_FQ.ASP` (查詢 - 傳票過帳)
  - `WEB/IMS_CORE/ASP/SYST000/SYST322_FU.ASP` (更新 - 傳票過帳)
  - `WEB/IMS_CORE/ASP/SYST000/SYST331_FB.ASP` (瀏覽 - 傳票查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST331_FQ.ASP` (查詢 - 傳票查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST332_FB.ASP` (瀏覽 - 傳票查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST332_FQ.ASP` (查詢 - 傳票查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST351_FB.ASP` (瀏覽 - 反過帳資料年結處理)
  - `WEB/IMS_CORE/ASP/SYST000/SYST351_FQ.ASP` (查詢 - 反過帳資料年結處理)
  - `WEB/IMS_CORE/ASP/SYST000/SYST352_FB.ASP` (瀏覽 - 反過帳資料年結處理)
  - `WEB/IMS_CORE/ASP/SYST000/SYST352_FQ.ASP` (查詢 - 反過帳資料年結處理)

### 1.2 業務需求
- **傳票確認作業 (SYST311/SYST312)**
  - 管理傳票確認狀態（建立 → 確認）
  - 支援傳票確認查詢、瀏覽、批次確認
  - 支援傳票確認刪除（僅限建立狀態）
  - 支援傳票確認報表
  - 支援應付/應收系統拋轉（依據參數設定）
  - 支援手開應收/應付票據維護（依據參數設定）
  - 支援傳票狀態檢查（僅建立狀態可確認）
  
- **傳票過帳作業 (SYST321/SYST322)**
  - 管理傳票過帳狀態（確認 → 過帳）
  - 支援傳票過帳查詢、瀏覽、批次過帳
  - 支援依明細勾選批次過帳
  - 支援過帳年月檢查（需大於關帳年月）
  - 支援傳票狀態統計（建立/確認/過帳狀態筆數）
  - 支援本月金額重新計算（依據參數設定）
  
- **傳票查詢作業 (SYST331/SYST332)**
  - 提供傳票查詢功能
  - 支援多條件查詢
  - 支援傳票狀態查詢
  
- **反過帳資料年結處理作業 (SYST351/SYST352)**
  - 管理反過帳資料年結處理
  - 支援反年結查詢、瀏覽
  - 支援關帳年度檢查

---

## 二、資料庫設計 (Schema)

### 2.1 擴展資料表: `Vouchers` (對應舊系統 `VOUCHER_M`)

需要在現有的 `Vouchers` 表中新增以下欄位：

```sql
-- 擴展現有 Vouchers 表
ALTER TABLE [dbo].[Vouchers]
ADD 
    [VoucherStatus] NVARCHAR(1) NULL DEFAULT '1', -- 傳票狀態 (VOUCHER_STATUS, 1:建立, 2:確認, 3:過帳)
    [ConfirmDate] DATETIME2 NULL, -- 確認日期 (CONFIRM_DATE)
    [ConfirmBy] NVARCHAR(50) NULL, -- 確認者 (CONFIRM_BY)
    [PostingYearMonth] NVARCHAR(6) NULL, -- 過帳年月 (POSTING_YEAR_MONTH, YYYYMM)
    [PostingDate] DATETIME2 NULL, -- 過帳日期 (POSTING_DATE)
    [PostingBy] NVARCHAR(50) NULL, -- 過帳者 (POSTING_BY)
    [ReversePostingDate] DATETIME2 NULL, -- 反過帳日期 (REVERSE_POSTING_DATE)
    [ReversePostingBy] NVARCHAR(50) NULL, -- 反過帳者 (REVERSE_POSTING_BY)
    [YearEndProcessYear] NVARCHAR(4) NULL; -- 年結處理年度 (YEAR_END_PROCESS_YEAR)

-- 索引
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherStatus] ON [dbo].[Vouchers] ([VoucherStatus]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_PostingYearMonth] ON [dbo].[Vouchers] ([PostingYearMonth]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_ConfirmDate] ON [dbo].[Vouchers] ([ConfirmDate]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_PostingDate] ON [dbo].[Vouchers] ([PostingDate]);
```

### 2.2 傳票狀態統計視圖: `V_VoucherStatusCount`

```sql
CREATE VIEW [dbo].[V_VoucherStatusCount] AS
SELECT 
    [PostingYearMonth],
    COUNT(CASE WHEN [VoucherStatus] = '1' THEN 1 END) AS [CreateCount],
    COUNT(CASE WHEN [VoucherStatus] = '2' THEN 1 END) AS [ConfirmCount],
    COUNT(CASE WHEN [VoucherStatus] = '3' THEN 1 END) AS [PostingCount],
    SUM(CASE WHEN [VoucherStatus] = '1' THEN 1 ELSE 0 END) AS [CreateAmount],
    SUM(CASE WHEN [VoucherStatus] = '2' THEN 1 ELSE 0 END) AS [ConfirmAmount],
    SUM(CASE WHEN [VoucherStatus] = '3' THEN 1 ELSE 0 END) AS [PostingAmount]
FROM [dbo].[Vouchers]
WHERE [Status] = '1' -- 僅正常狀態
GROUP BY [PostingYearMonth];
```

### 2.3 應付/應收暫存表: `APARTemp` (對應舊系統 `AP_TEMP`, `AR_TEMP`)

```sql
CREATE TABLE [dbo].[APARTemp] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔TKey (外鍵至Vouchers)
    [VoucherDetailTKey] BIGINT NULL, -- 傳票明細TKey (外鍵至VoucherDetails)
    [APARType] NVARCHAR(1) NOT NULL, -- 類型 (APAR_TYPE, A:應收, P:應付)
    [VendorId] NVARCHAR(50) NULL, -- 廠客代號 (VENDOR_ID)
    [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 金額 (AMOUNT)
    [DueDate] DATETIME2 NULL, -- 到期日 (DUE_DATE)
    [Status] NVARCHAR(1) NULL DEFAULT '0', -- 狀態 (STATUS, 0:暫存, 1:已轉入)
    [TransferDate] DATETIME2 NULL, -- 轉入日期 (TRANSFER_DATE)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [FK_APARTemp_Vouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[Vouchers] ([TKey]),
    CONSTRAINT [FK_APARTemp_VoucherDetails] FOREIGN KEY ([VoucherDetailTKey]) REFERENCES [dbo].[VoucherDetails] ([TKey])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_APARTemp_VoucherTKey] ON [dbo].[APARTemp] ([VoucherTKey]);
CREATE NONCLUSTERED INDEX [IX_APARTemp_APARType] ON [dbo].[APARTemp] ([APARType]);
CREATE NONCLUSTERED INDEX [IX_APARTemp_Status] ON [dbo].[APARTemp] ([Status]);
```

### 2.4 資料字典

#### Vouchers 擴展欄位

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| VoucherStatus | NVARCHAR | 1 | YES | '1' | 傳票狀態 | 1:建立, 2:確認, 3:過帳 |
| ConfirmDate | DATETIME2 | - | YES | - | 確認日期 | - |
| ConfirmBy | NVARCHAR | 50 | YES | - | 確認者 | - |
| PostingYearMonth | NVARCHAR | 6 | YES | - | 過帳年月 | YYYYMM格式 |
| PostingDate | DATETIME2 | - | YES | - | 過帳日期 | - |
| PostingBy | NVARCHAR | 50 | YES | - | 過帳者 | - |
| ReversePostingDate | DATETIME2 | - | YES | - | 反過帳日期 | - |
| ReversePostingBy | NVARCHAR | 50 | YES | - | 反過帳者 | - |
| YearEndProcessYear | NVARCHAR | 4 | YES | - | 年結處理年度 | YYYY格式 |

#### APARTemp 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherTKey | BIGINT | - | NO | - | 傳票主檔TKey | 外鍵至Vouchers |
| VoucherDetailTKey | BIGINT | - | YES | - | 傳票明細TKey | 外鍵至VoucherDetails |
| APARType | NVARCHAR | 1 | NO | - | 類型 | A:應收, P:應付 |
| VendorId | NVARCHAR | 50 | YES | - | 廠客代號 | - |
| Amount | DECIMAL | 18,2 | NO | 0 | 金額 | - |
| DueDate | DATETIME2 | - | YES | - | 到期日 | - |
| Status | NVARCHAR | 1 | YES | '0' | 狀態 | 0:暫存, 1:已轉入 |
| TransferDate | DATETIME2 | - | YES | - | 轉入日期 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢傳票確認列表 (SYST311/SYST312)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/confirm`
- **說明**: 查詢待確認或已確認傳票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherDate",
    "sortOrder": "DESC",
    "filters": {
      "voucherNoFrom": "",
      "voucherNoTo": "",
      "voucherDateFrom": "",
      "voucherDateTo": "",
      "voucherType": [],
      "voucherStatus": "1",
      "confirmDateFrom": "",
      "confirmDateTo": ""
    }
  }
  ```
- **回應格式**: 同傳票列表查詢

#### 3.1.2 批次確認傳票 (SYST311/SYST312)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/confirm/batch`
- **說明**: 批次確認傳票（僅限建立狀態）
- **請求格式**:
  ```json
  {
    "voucherNos": ["V20240101001", "V20240101002"],
    "confirmDate": "2024-01-01"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "確認成功",
    "data": {
      "successCount": 2,
      "failCount": 0,
      "failItems": []
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢傳票過帳列表 (SYST321/SYST322)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/posting`
- **說明**: 查詢待過帳或已過帳傳票列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherDate",
    "sortOrder": "DESC",
    "filters": {
      "postingYearMonth": "",
      "voucherDateFrom": "",
      "voucherDateTo": "",
      "voucherType": [],
      "voucherStatus": "2",
      "postingByDetail": false
    }
  }
  ```
- **回應格式**: 同傳票列表查詢

#### 3.1.4 批次過帳傳票 (SYST321/SYST322)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/posting/batch`
- **說明**: 批次過帳傳票（僅限確認狀態）
- **請求格式**:
  ```json
  {
    "voucherNos": ["V20240101001", "V20240101002"],
    "postingYearMonth": "202401",
    "postingDate": "2024-01-01",
    "postingByDetail": false,
    "detailTKeys": []
  }
  ```
- **回應格式**: 同批次確認

#### 3.1.5 查詢傳票狀態統計 (SYST321)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/status-count`
- **說明**: 查詢傳票狀態統計（建立/確認/過帳狀態筆數）
- **請求參數**:
  ```json
  {
    "postingYearMonth": "202401"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "postingYearMonth": "202401",
      "createCount": 10,
      "confirmCount": 8,
      "postingCount": 5,
      "createAmount": 100000,
      "confirmAmount": 80000,
      "postingAmount": 50000
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 查詢傳票列表 (SYST331/SYST332)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/query`
- **說明**: 查詢傳票列表（多條件查詢）
- **請求參數**: 同傳票列表查詢
- **回應格式**: 同傳票列表查詢

#### 3.1.7 查詢反過帳資料年結處理 (SYST351/SYST352)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/reverse-year-end`
- **說明**: 查詢反過帳資料年結處理列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "year": "2023"
    }
  }
  ```
- **回應格式**: 同傳票列表查詢

#### 3.1.8 反過帳傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/{voucherNo}/reverse-posting`
- **說明**: 反過帳傳票（僅限已過帳狀態）
- **路徑參數**:
  - `voucherNo`: 傳票號碼
- **請求格式**:
  ```json
  {
    "reversePostingDate": "2024-01-01",
    "reason": "反過帳原因"
  }
  ```
- **回應格式**: 標準回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `VoucherTransactionController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/vouchers")]
    [Authorize]
    public class VoucherTransactionController : ControllerBase
    {
        private readonly IVoucherTransactionService _voucherTransactionService;
        
        public VoucherTransactionController(IVoucherTransactionService voucherTransactionService)
        {
            _voucherTransactionService = voucherTransactionService;
        }
        
        [HttpGet("confirm")]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetConfirmVouchers([FromQuery] VoucherConfirmQueryDto query)
        {
            // 實作查詢確認傳票邏輯
        }
        
        [HttpPost("confirm/batch")]
        public async Task<ActionResult<ApiResponse<BatchConfirmResultDto>>> BatchConfirmVouchers([FromBody] BatchConfirmVoucherDto dto)
        {
            // 實作批次確認邏輯
        }
        
        [HttpGet("posting")]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetPostingVouchers([FromQuery] VoucherPostingQueryDto query)
        {
            // 實作查詢過帳傳票邏輯
        }
        
        [HttpPost("posting/batch")]
        public async Task<ActionResult<ApiResponse<BatchPostingResultDto>>> BatchPostingVouchers([FromBody] BatchPostingVoucherDto dto)
        {
            // 實作批次過帳邏輯
        }
        
        [HttpGet("status-count")]
        public async Task<ActionResult<ApiResponse<VoucherStatusCountDto>>> GetVoucherStatusCount([FromQuery] string postingYearMonth)
        {
            // 實作傳票狀態統計邏輯
        }
        
        [HttpGet("query")]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> QueryVouchers([FromQuery] VoucherQueryDto query)
        {
            // 實作傳票查詢邏輯
        }
        
        [HttpGet("reverse-year-end")]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetReverseYearEndVouchers([FromQuery] ReverseYearEndQueryDto query)
        {
            // 實作反過帳資料年結處理查詢邏輯
        }
        
        [HttpPost("{voucherNo}/reverse-posting")]
        public async Task<ActionResult<ApiResponse>> ReversePostingVoucher(string voucherNo, [FromBody] ReversePostingDto dto)
        {
            // 實作反過帳邏輯
        }
    }
}
```

#### 3.2.2 Service: `VoucherTransactionService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IVoucherTransactionService
    {
        Task<PagedResult<VoucherDto>> GetConfirmVouchersAsync(VoucherConfirmQueryDto query);
        Task<BatchConfirmResultDto> BatchConfirmVouchersAsync(BatchConfirmVoucherDto dto);
        Task<PagedResult<VoucherDto>> GetPostingVouchersAsync(VoucherPostingQueryDto query);
        Task<BatchPostingResultDto> BatchPostingVouchersAsync(BatchPostingVoucherDto dto);
        Task<VoucherStatusCountDto> GetVoucherStatusCountAsync(string postingYearMonth);
        Task<PagedResult<VoucherDto>> QueryVouchersAsync(VoucherQueryDto query);
        Task<PagedResult<VoucherDto>> GetReverseYearEndVouchersAsync(ReverseYearEndQueryDto query);
        Task ReversePostingVoucherAsync(string voucherNo, ReversePostingDto dto);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 傳票確認列表頁面 (`VoucherConfirmList.vue`) - SYST311/SYST312
- **路徑**: `/accounting/vouchers/confirm`
- **功能**: 顯示待確認或已確認傳票列表，支援查詢、批次確認、刪除
- **主要元件**:
  - 查詢表單 (VoucherConfirmSearchForm)
  - 資料表格 (VoucherConfirmDataTable)
  - 批次確認對話框
  - 刪除確認對話框

#### 4.1.2 傳票過帳列表頁面 (`VoucherPostingList.vue`) - SYST321/SYST322
- **路徑**: `/accounting/vouchers/posting`
- **功能**: 顯示待過帳或已過帳傳票列表，支援查詢、批次過帳、依明細勾選過帳
- **主要元件**:
  - 查詢表單 (VoucherPostingSearchForm)
  - 資料表格 (VoucherPostingDataTable)
  - 批次過帳對話框
  - 傳票狀態統計顯示

#### 4.1.3 傳票查詢頁面 (`VoucherQuery.vue`) - SYST331/SYST332
- **路徑**: `/accounting/vouchers/query`
- **功能**: 提供多條件傳票查詢功能
- **主要元件**:
  - 查詢表單 (VoucherQueryForm)
  - 資料表格 (VoucherDataTable)

#### 4.1.4 反過帳資料年結處理頁面 (`ReverseYearEnd.vue`) - SYST351/SYST352
- **路徑**: `/accounting/vouchers/reverse-year-end`
- **功能**: 顯示反過帳資料年結處理列表
- **主要元件**:
  - 查詢表單 (ReverseYearEndSearchForm)
  - 資料表格 (ReverseYearEndDataTable)

### 4.2 UI 元件設計

#### 4.2.1 傳票確認查詢表單元件 (`VoucherConfirmSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="傳票號碼">
      <el-input v-model="searchForm.voucherNoFrom" placeholder="從" style="width: 150px" />
      <span style="margin: 0 10px">~</span>
      <el-input v-model="searchForm.voucherNoTo" placeholder="迄" style="width: 150px" />
    </el-form-item>
    <el-form-item label="傳票日期">
      <el-date-picker v-model="searchForm.voucherDateFrom" type="date" placeholder="從" />
      <span style="margin: 0 10px">~</span>
      <el-date-picker v-model="searchForm.voucherDateTo" type="date" placeholder="迄" />
    </el-form-item>
    <el-form-item label="傳票型態">
      <el-select v-model="searchForm.voucherType" multiple placeholder="請選擇" filterable>
        <el-option v-for="item in voucherTypeList" :key="item.voucherId" :label="item.voucherName" :value="item.voucherId" />
      </el-select>
    </el-form-item>
    <el-form-item label="傳票狀態">
      <el-select v-model="searchForm.voucherStatus" placeholder="請選擇">
        <el-option label="建立" value="1" />
        <el-option label="確認" value="2" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 傳票過帳查詢表單元件 (`VoucherPostingSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="過帳年月" prop="postingYearMonth">
      <el-date-picker 
        v-model="searchForm.postingYearMonth" 
        type="month" 
        placeholder="請選擇過帳年月"
        format="YYYYMM"
        value-format="YYYYMM"
      />
    </el-form-item>
    <el-form-item label="傳票日期">
      <el-date-picker v-model="searchForm.voucherDateFrom" type="date" placeholder="從" />
      <span style="margin: 0 10px">~</span>
      <el-date-picker v-model="searchForm.voucherDateTo" type="date" placeholder="迄" />
    </el-form-item>
    <el-form-item label="依明細勾選">
      <el-checkbox v-model="searchForm.postingByDetail" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
  <div v-if="statusCount" class="status-count">
    <el-card>
      <div>建立狀態傳票張數: {{ statusCount.createCount }}</div>
      <div>確認狀態傳票張數: {{ statusCount.confirmCount }}</div>
      <div>過帳狀態傳票張數: {{ statusCount.postingCount }}</div>
    </el-card>
  </div>
</template>
```

#### 4.2.3 批次確認對話框 (`BatchConfirmDialog.vue`)
```vue
<template>
  <el-dialog
    title="批次確認傳票"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="確認日期" prop="confirmDate">
        <el-date-picker v-model="form.confirmDate" type="date" placeholder="請選擇確認日期" />
      </el-form-item>
      <el-form-item label="選取傳票">
        <el-table :data="selectedVouchers" border>
          <el-table-column prop="voucherNo" label="傳票號碼" />
          <el-table-column prop="voucherDate" label="傳票日期" />
          <el-table-column prop="voucherNotes" label="傳票摘要" />
        </el-table>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`voucherTransaction.api.ts`)
```typescript
import request from '@/utils/request';

export interface VoucherConfirmQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    voucherNoFrom?: string;
    voucherNoTo?: string;
    voucherDateFrom?: string;
    voucherDateTo?: string;
    voucherType?: string[];
    voucherStatus?: string;
    confirmDateFrom?: string;
    confirmDateTo?: string;
  };
}

export interface BatchConfirmVoucherDto {
  voucherNos: string[];
  confirmDate: string;
}

export interface BatchConfirmResultDto {
  successCount: number;
  failCount: number;
  failItems: Array<{
    voucherNo: string;
    reason: string;
  }>;
}

export interface VoucherPostingQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    postingYearMonth?: string;
    voucherDateFrom?: string;
    voucherDateTo?: string;
    voucherType?: string[];
    voucherStatus?: string;
    postingByDetail?: boolean;
  };
}

export interface BatchPostingVoucherDto {
  voucherNos?: string[];
  postingYearMonth: string;
  postingDate: string;
  postingByDetail: boolean;
  detailTKeys?: number[];
}

export interface VoucherStatusCountDto {
  postingYearMonth: string;
  createCount: number;
  confirmCount: number;
  postingCount: number;
  createAmount: number;
  confirmAmount: number;
  postingAmount: number;
}

// API 函數
export const getConfirmVoucherList = (query: VoucherConfirmQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/vouchers/confirm', { params: query });
};

export const batchConfirmVouchers = (data: BatchConfirmVoucherDto) => {
  return request.post<ApiResponse<BatchConfirmResultDto>>('/api/v1/vouchers/confirm/batch', data);
};

export const getPostingVoucherList = (query: VoucherPostingQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/vouchers/posting', { params: query });
};

export const batchPostingVouchers = (data: BatchPostingVoucherDto) => {
  return request.post<ApiResponse<BatchPostingResultDto>>('/api/v1/vouchers/posting/batch', data);
};

export const getVoucherStatusCount = (postingYearMonth: string) => {
  return request.get<ApiResponse<VoucherStatusCountDto>>('/api/v1/vouchers/status-count', { params: { postingYearMonth } });
};

export const getReverseYearEndVouchers = (query: ReverseYearEndQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/vouchers/reverse-year-end', { params: query });
};

export const reversePostingVoucher = (voucherNo: string, data: ReversePostingDto) => {
  return request.post<ApiResponse>(`/api/v1/vouchers/${voucherNo}/reverse-posting`, data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 擴展現有 Vouchers 表（新增狀態相關欄位）
- [ ] 建立 APARTemp 資料表
- [ ] 建立傳票狀態統計視圖
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (8天)
- [ ] Entity 類別擴展
- [ ] Repository 實作（確認、過帳、查詢、反過帳）
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（狀態檢查、過帳年月檢查、關帳年度檢查等）
- [ ] 批次處理邏輯實作
- [ ] 應付/應收系統拋轉邏輯實作（依據參數）
- [ ] 手開應收/應付票據維護邏輯實作（依據參數）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (8天)
- [ ] API 呼叫函數
- [ ] 傳票確認列表頁面開發 (SYST311/SYST312)
- [ ] 傳票過帳列表頁面開發 (SYST321/SYST322)
- [ ] 傳票查詢頁面開發 (SYST331/SYST332)
- [ ] 反過帳資料年結處理頁面開發 (SYST351/SYST352)
- [ ] 批次確認功能開發
- [ ] 批次過帳功能開發
- [ ] 依明細勾選過帳功能開發
- [ ] 傳票狀態統計顯示
- [ ] 表單驗證
- [ ] 參數控制欄位顯示
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 業務邏輯測試（狀態檢查、批次處理、應付/應收拋轉等）

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 21天

---

## 六、注意事項

### 6.1 業務邏輯
- 傳票確認作業 (SYST311/SYST312)
  - 僅建立狀態的傳票可以確認
  - 確認時需檢查傳票借貸是否平衡
  - 確認時需檢查會計科目類別是否一致
  - 確認後可依據參數設定寫入應付/應收暫存表
  - 確認後可依據參數設定維護手開應收/應付票據
  
- 傳票過帳作業 (SYST321/SYST322)
  - 僅確認狀態的傳票可以過帳
  - 過帳年月必須大於關帳年月
  - 支援依明細勾選批次過帳
  - 過帳後可依據參數設定將應付/應收暫存表轉入正式表
  - 過帳後可依據參數設定拋轉手開應收/應付票據
  
- 反過帳作業
  - 僅已過帳狀態的傳票可以反過帳
  - 反過帳需檢查是否有後續處理
  
- 年結處理作業 (SYST351/SYST352)
  - 需檢查關帳年度
  - 需檢查反過帳資料

### 6.2 參數控制
- 應付/應收系統拋轉時機：由參數 `APAR_TEMP_TO_APAR` 控制（0:傳票維護, 1:傳票確認, 2:傳票過帳）
- 手開應收/應付票據維護模式：由參數 `HAND_APV_ARV_MODE` 控制（0:應收應付系統產生, 1:傳票建立時維護並拋轉, 2:傳票建立時維護傳票確認時拋轉, 3:傳票建立時維護傳票過帳時拋轉）
- 關帳年月：由參數 `ACCT_CLOSE_YM` 控制
- 關帳年度：由參數 `ACCT_CLOSE_YEAR` 控制

### 6.3 資料驗證
- 傳票狀態必須驗證（建立→確認→過帳）
- 過帳年月必須驗證（需大於關帳年月）
- 關帳年度必須驗證
- 批次處理時需檢查所有傳票狀態

### 6.4 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 批次處理需使用事務處理
- 傳票狀態統計需使用視圖優化

---

## 七、測試案例

### 7.1 單元測試
- [ ] 批次確認傳票成功
- [ ] 批次確認傳票失敗 (非建立狀態)
- [ ] 批次過帳傳票成功
- [ ] 批次過帳傳票失敗 (非確認狀態)
- [ ] 批次過帳傳票失敗 (過帳年月小於關帳年月)
- [ ] 反過帳傳票成功
- [ ] 反過帳傳票失敗 (非過帳狀態)
- [ ] 查詢傳票確認列表成功
- [ ] 查詢傳票過帳列表成功
- [ ] 查詢傳票狀態統計成功
- [ ] 依明細勾選過帳成功

### 7.2 整合測試
- [ ] 完整確認流程測試
- [ ] 完整過帳流程測試
- [ ] 完整反過帳流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 參數控制欄位顯示測試
- [ ] 批次處理測試
- [ ] 應付/應收系統拋轉測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 批次處理效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST311_FB.ASP` - 傳票確認作業瀏覽畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST311_FQ.ASP` - 傳票確認作業查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST311_FU.ASP` - 傳票確認更新畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST311_FD.ASP` - 傳票確認作業刪除畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST311_PR.ASP` - 傳票確認作業報表畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST321_FB.ASP` - 傳票過帳瀏覽畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST321_FQ.ASP` - 傳票過帳查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST352_FB.ASP` - 反過帳資料年結處理瀏覽畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST352_FQ.ASP` - 反過帳資料年結處理查詢畫面

### 8.2 資料庫 Schema
- 舊系統資料表：`VOUCHER_M` (傳票主檔)
- 主要欄位擴展：VOUCHER_STATUS, CONFIRM_DATE, CONFIRM_BY, POSTING_YEAR_MONTH, POSTING_DATE, POSTING_BY, REVERSE_POSTING_DATE, REVERSE_POSTING_BY, YEAR_END_PROCESS_YEAR
- 舊系統資料表：`AP_TEMP`, `AR_TEMP` (應付/應收暫存表)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


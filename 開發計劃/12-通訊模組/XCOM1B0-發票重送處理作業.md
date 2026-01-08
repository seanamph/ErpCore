# XCOM1B0 - 發票重送處理作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM1B0
- **功能名稱**: 發票重送處理作業 (誤作廢還原)
- **功能描述**: 提供發票重送處理功能，包含誤作廢還原、發票號碼清空、發票重建等作業，支援B2C和B2B發票類型處理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1B0_FI.asp` (新增/處理)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1B0_FQ.ASP` (查詢)

### 1.2 業務需求
- 處理誤作廢發票的還原作業
- 支援發票重送功能（重送/重建）
- 支援B2C和B2B發票類型處理
- 支援多種發票類型：一般發票、作廢發票、註銷發票、發票折讓、作廢折讓
- 支援發票號碼清空並記錄備註
- 支援電子發票上傳處理
- 支援當日和非當日處理邏輯

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `InvoiceResendLog` (發票重送記錄)

```sql
CREATE TABLE [dbo].[InvoiceResendLog] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SalesDate] DATETIME2 NOT NULL, -- 交易日期(原折讓日期)
    [InvNo] NVARCHAR(50) NOT NULL, -- 發票號碼
    [InvType] NVARCHAR(20) NULL, -- 發票類型
    [EinvType] NVARCHAR(20) NOT NULL, -- 電子發票類型
    [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼
    [OriSalesDate] DATETIME2 NULL, -- 原發票日期
    [DataSource] NVARCHAR(10) NULL, -- 資料來源 (V/G/其他)
    [ResendType] NVARCHAR(10) NOT NULL, -- 重送類型 (Y:重送, N:重建)
    [ResendEinvType] NVARCHAR(20) NOT NULL, -- 重送發票類型
    [PosId] NVARCHAR(50) NULL, -- POS機號
    [TransNo] NVARCHAR(50) NULL, -- 交易序號
    [Net] DECIMAL(18,2) NULL, -- 淨額
    [ReceiverId] NVARCHAR(50) NULL, -- 接收者ID
    [BadSalesDate] DATETIME2 NULL, -- 作廢日期
    [BadPosId] NVARCHAR(50) NULL, -- 作廢POS機號
    [BadTransNo] NVARCHAR(50) NULL, -- 作廢交易序號
    [BadTransType] NVARCHAR(10) NULL, -- 作廢交易類型
    [UpFlag] NVARCHAR(10) NULL, -- 上送標記 (1:非當日重新上送, 2:當日清除不上送)
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (PENDING:待處理, PROCESSING:處理中, SUCCESS:成功, FAILED:失敗)
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [ProcessTime] DATETIME2 NULL, -- 處理時間
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_InvoiceResendLog] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_InvoiceResendLog_SalesDate] ON [dbo].[InvoiceResendLog] ([SalesDate]);
CREATE NONCLUSTERED INDEX [IX_InvoiceResendLog_InvNo] ON [dbo].[InvoiceResendLog] ([InvNo]);
CREATE NONCLUSTERED INDEX [IX_InvoiceResendLog_SiteId] ON [dbo].[InvoiceResendLog] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_InvoiceResendLog_Status] ON [dbo].[InvoiceResendLog] ([Status]);
CREATE NONCLUSTERED INDEX [IX_InvoiceResendLog_CreatedAt] ON [dbo].[InvoiceResendLog] ([CreatedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `ProdSalesM` - 商品銷售主檔 (對應舊系統)
- 此表為現有系統資料表，用於查詢和更新發票資料
- 主要欄位：SALES_DATE, TRANS_TYPE, RECE_TRACK, GUI_BEGIN, GUI_END, SITE_ID, POS_ID, TRANS_NO等

#### 2.2.2 `InvSaleB` - 發票銷售主檔 (對應舊系統)
- 此表為現有系統資料表，用於B2C發票查詢
- 主要欄位：INV_DATE, TRACK, INV_NO, SITE_ID, TAX_AMT等

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| LogId | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| SalesDate | DATETIME2 | - | NO | - | 交易日期(原折讓日期) | - |
| InvNo | NVARCHAR | 50 | NO | - | 發票號碼 | - |
| EinvType | NVARCHAR | 20 | NO | - | 電子發票類型 | C0401/C0501/C0701/D0401/D0501/A0401/A0501/B0401 |
| SiteId | NVARCHAR | 50 | NO | - | 店別代碼 | - |
| ResendType | NVARCHAR | 10 | NO | - | 重送類型 | Y:重送, N:重建 |
| ResendEinvType | NVARCHAR | 20 | NO | - | 重送發票類型 | - |
| Status | NVARCHAR | 20 | NO | 'PENDING' | 狀態 | PENDING/PROCESSING/SUCCESS/FAILED |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢發票重送記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1b0/invoice-resend-logs`
- **說明**: 查詢發票重送記錄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CreatedAt",
    "sortOrder": "DESC",
    "filters": {
      "salesDate": "",
      "invNo": "",
      "siteId": "",
      "resendType": "",
      "resendEinvType": "",
      "status": ""
    }
  }
  ```
- **回應格式**: 標準分頁回應格式

#### 3.1.2 查詢單筆發票重送記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1b0/invoice-resend-logs/{logId}`
- **說明**: 根據記錄ID查詢單筆發票重送記錄
- **路徑參數**:
  - `logId`: 記錄ID
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增發票重送處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom1b0/invoice-resend`
- **說明**: 新增發票重送處理作業
- **請求格式**:
  ```json
  {
    "salesDate": "2024/01/01",
    "invNo": "AB12345678",
    "siteId": "SITE001",
    "resendType": "Y",
    "resendEinvType": "C0401",
    "dataSource": "V"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "處理成功",
    "data": {
      "logId": 1,
      "status": "SUCCESS",
      "processTime": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 查詢發票資訊
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom1b0/invoice-info`
- **說明**: 查詢發票基本資訊（用於驗證）
- **請求格式**:
  ```json
  {
    "salesDate": "2024/01/01",
    "invNo": "AB12345678",
    "siteId": "SITE001",
    "dataSource": "V"
  }
  ```
- **回應格式**: 標準單筆回應格式

#### 3.1.5 批次處理發票重送
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom1b0/invoice-resend/batch`
- **說明**: 批次處理多筆發票重送
- **請求格式**:
  ```json
  {
    "items": [
      {
        "salesDate": "2024/01/01",
        "invNo": "AB12345678",
        "siteId": "SITE001",
        "resendType": "Y",
        "resendEinvType": "C0401"
      }
    ]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom1B0Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom1b0")]
    [Authorize]
    public class XCom1B0Controller : ControllerBase
    {
        private readonly IInvoiceResendService _invoiceResendService;
        
        public XCom1B0Controller(IInvoiceResendService invoiceResendService)
        {
            _invoiceResendService = invoiceResendService;
        }
        
        [HttpGet("invoice-resend-logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<InvoiceResendLogDto>>>> GetInvoiceResendLogs([FromQuery] InvoiceResendLogQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("invoice-resend-logs/{logId}")]
        public async Task<ActionResult<ApiResponse<InvoiceResendLogDto>>> GetInvoiceResendLog(long logId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost("invoice-resend")]
        public async Task<ActionResult<ApiResponse<InvoiceResendResultDto>>> ProcessInvoiceResend([FromBody] InvoiceResendRequestDto dto)
        {
            // 實作發票重送處理邏輯
        }
        
        [HttpPost("invoice-info")]
        public async Task<ActionResult<ApiResponse<InvoiceInfoDto>>> GetInvoiceInfo([FromBody] InvoiceInfoQueryDto dto)
        {
            // 實作查詢發票資訊邏輯
        }
        
        [HttpPost("invoice-resend/batch")]
        public async Task<ActionResult<ApiResponse<BatchInvoiceResendResultDto>>> BatchProcessInvoiceResend([FromBody] BatchInvoiceResendRequestDto dto)
        {
            // 實作批次處理邏輯
        }
    }
}
```

#### 3.2.2 Service: `InvoiceResendService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IInvoiceResendService
    {
        Task<PagedResult<InvoiceResendLogDto>> GetInvoiceResendLogsAsync(InvoiceResendLogQueryDto query);
        Task<InvoiceResendLogDto> GetInvoiceResendLogByIdAsync(long logId);
        Task<InvoiceResendResultDto> ProcessInvoiceResendAsync(InvoiceResendRequestDto dto);
        Task<InvoiceInfoDto> GetInvoiceInfoAsync(InvoiceInfoQueryDto dto);
        Task<BatchInvoiceResendResultDto> BatchProcessInvoiceResendAsync(BatchInvoiceResendRequestDto dto);
    }
}
```

#### 3.2.3 Repository: `InvoiceResendLogRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IInvoiceResendLogRepository
    {
        Task<InvoiceResendLog> GetByIdAsync(long logId);
        Task<PagedResult<InvoiceResendLog>> GetPagedAsync(InvoiceResendLogQuery query);
        Task<InvoiceResendLog> CreateAsync(InvoiceResendLog log);
        Task<InvoiceResendLog> UpdateAsync(InvoiceResendLog log);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 發票重送查詢頁面 (`InvoiceResendQuery.vue`)
- **路徑**: `/xcom/invoice-resend/query`
- **功能**: 顯示發票重送查詢表單，支援條件查詢
- **主要元件**:
  - 查詢表單 (InvoiceResendSearchForm)
  - 查詢結果列表 (InvoiceResendList)

#### 4.1.2 發票重送處理頁面 (`InvoiceResendProcess.vue`)
- **路徑**: `/xcom/invoice-resend/process`
- **功能**: 發票重送處理作業，支援單筆和批次處理

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`InvoiceResendSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="交易日期(原折讓日期)">
      <el-date-picker
        v-model="searchForm.salesDate"
        type="date"
        placeholder="請選擇日期"
        format="YYYY/MM/DD"
        value-format="YYYY/MM/DD"
      />
    </el-form-item>
    <el-form-item label="發票號碼">
      <el-input v-model="searchForm.invNo" placeholder="請輸入發票號碼" />
    </el-form-item>
    <el-form-item label="店別">
      <el-select v-model="searchForm.siteId" placeholder="請選擇店別">
        <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
      </el-select>
    </el-form-item>
    <el-form-item label="重送/重建">
      <el-radio-group v-model="searchForm.resendType">
        <el-radio label="Y">重送</el-radio>
        <el-radio label="N">重建</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item label="重送發票類型">
      <el-select v-model="searchForm.resendEinvType" placeholder="請選擇發票類型">
        <el-option-group label="B2C">
          <el-option label="C0401(B2C一般發票)" value="C0401" />
          <el-option label="C0501(B2C作廢發票)" value="C0501" />
          <el-option label="C0701(B2C註銷發票)" value="C0701" />
          <el-option label="D0401(B2C發票折讓)" value="D0401" />
          <el-option label="D0501(B2C作廢折讓)" value="D0501" />
        </el-option-group>
        <el-option-group label="B2B">
          <el-option label="A0401(B2B一般發票)" value="A0401" />
          <el-option label="A0501(B2B作廢發票)" value="A0501" />
          <el-option label="B0401(B2B發票折讓)" value="B0401" />
        </el-option-group>
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 處理表單元件 (`InvoiceResendProcessForm.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="180px">
    <el-form-item label="交易日期(原折讓日期)" prop="salesDate">
      <el-date-picker
        v-model="form.salesDate"
        type="date"
        placeholder="請選擇日期"
        format="YYYY/MM/DD"
        value-format="YYYY/MM/DD"
      />
    </el-form-item>
    <el-form-item label="發票號碼" prop="invNo">
      <el-input v-model="form.invNo" placeholder="請輸入發票號碼" />
    </el-form-item>
    <el-form-item label="店別" prop="siteId">
      <el-select v-model="form.siteId" placeholder="請選擇店別">
        <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
      </el-select>
    </el-form-item>
    <el-form-item label="重送/重建" prop="resendType">
      <el-radio-group v-model="form.resendType">
        <el-radio label="Y">重送</el-radio>
        <el-radio label="N">重建</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item label="重送發票類型" prop="resendEinvType">
      <el-select v-model="form.resendEinvType" placeholder="請選擇發票類型">
        <el-option-group label="B2C">
          <el-option label="C0401(B2C一般發票)" value="C0401" />
          <el-option label="C0501(B2C作廢發票)" value="C0501" />
          <el-option label="C0701(B2C註銷發票)" value="C0701" />
          <el-option label="D0401(B2C發票折讓)" value="D0401" />
          <el-option label="D0501(B2C作廢折讓)" value="D0501" />
        </el-option-group>
        <el-option-group label="B2B">
          <el-option label="A0401(B2B一般發票)" value="A0401" />
          <el-option label="A0501(B2B作廢發票)" value="A0501" />
          <el-option label="B0401(B2B發票折讓)" value="B0401" />
        </el-option-group>
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSubmit" :loading="loading">確定</el-button>
      <el-button @click="handleCancel">取消</el-button>
    </el-form-item>
  </el-form>
</template>
```

### 4.3 API 呼叫 (`xcom1b0.api.ts`)
```typescript
import request from '@/utils/request';

export interface InvoiceResendLogDto {
  logId: number;
  salesDate: string;
  invNo: string;
  invType?: string;
  einvType: string;
  siteId: string;
  resendType: string;
  resendEinvType: string;
  status: string;
  processTime?: string;
  errorMessage?: string;
  createdBy?: string;
  createdAt: string;
}

export interface InvoiceResendRequestDto {
  salesDate: string;
  invNo: string;
  siteId: string;
  resendType: string;
  resendEinvType: string;
  dataSource?: string;
}

export interface InvoiceResendResultDto {
  logId: number;
  status: string;
  processTime: string;
  message?: string;
}

// API 函數
export const getInvoiceResendLogs = (query: InvoiceResendLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<InvoiceResendLogDto>>>('/api/v1/xcom1b0/invoice-resend-logs', { params: query });
};

export const getInvoiceResendLogById = (logId: number) => {
  return request.get<ApiResponse<InvoiceResendLogDto>>(`/api/v1/xcom1b0/invoice-resend-logs/${logId}`);
};

export const processInvoiceResend = (data: InvoiceResendRequestDto) => {
  return request.post<ApiResponse<InvoiceResendResultDto>>('/api/v1/xcom1b0/invoice-resend', data);
};

export const getInvoiceInfo = (data: InvoiceInfoQueryDto) => {
  return request.post<ApiResponse<InvoiceInfoDto>>('/api/v1/xcom1b0/invoice-info', data);
};

export const batchProcessInvoiceResend = (data: BatchInvoiceResendRequestDto) => {
  return request.post<ApiResponse<BatchInvoiceResendResultDto>>('/api/v1/xcom1b0/invoice-resend/batch', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含發票查詢、更新、電子發票上送邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 處理表單開發
- [ ] 查詢結果列表開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試
- [ ] 發票處理邏輯測試

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
- 必須記錄操作日誌
- 必須驗證發票資料完整性

### 6.2 業務邏輯
- 必須驗證發票是否存在
- 必須驗證發票狀態是否可重送
- 必須處理當日和非當日的不同邏輯
- 必須正確處理B2C和B2B發票類型
- 必須正確處理各種發票類型（一般、作廢、註銷、折讓等）
- 必須正確更新發票資料（清空發票號碼、寫入備註等）
- 必須正確處理電子發票上送

### 6.3 資料驗證
- 交易日期必須驗證格式
- 發票號碼必須驗證格式
- 店別代碼必須存在
- 重送類型必須在允許範圍內
- 重送發票類型必須在允許範圍內

### 6.4 錯誤處理
- 必須處理發票不存在的情況
- 必須處理發票狀態不符合的情況
- 必須處理資料庫更新失敗的情況
- 必須處理電子發票上送失敗的情況
- 必須記錄錯誤訊息

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢發票資訊成功
- [ ] 查詢發票資訊失敗 (發票不存在)
- [ ] 處理發票重送成功
- [ ] 處理發票重送失敗 (發票狀態不符合)
- [ ] 批次處理發票重送成功
- [ ] 批次處理發票重送部分失敗

### 7.2 整合測試
- [ ] 完整發票重送流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 電子發票上送測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 批次處理效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1B0_FI.asp` (新增/處理)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1B0_FQ.ASP` (查詢)

### 8.2 相關功能
- 電子發票上傳作業 (ECA3000系列)
- 發票管理功能 (SYSG000系列)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


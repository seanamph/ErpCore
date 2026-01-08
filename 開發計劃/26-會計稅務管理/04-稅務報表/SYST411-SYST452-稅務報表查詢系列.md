# SYST411-SYST452 - 稅務報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST411-SYST452 系列
- **功能名稱**: 稅務報表查詢系列
- **功能描述**: 提供稅務報表查詢功能，包含傳票列印、財務報表查詢、稅務統計報表等，支援多種查詢條件與報表格式
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST411_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST411_FB.ASP` (瀏覽/列印)
  - `WEB/IMS_CORE/ASP/SYST000/SYST411_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYST000/SYST421_FB.ASP` (財務報表查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST452_FB.ASP` (稅務統計報表)

### 1.2 業務需求
- 支援傳票列印查詢（依日期、傳票號碼、傳票型態、狀態、建立者等條件）
- 支援財務報表查詢（依會計科目、日期範圍等條件）
- 支援稅務統計報表查詢
- 支援多種報表格式（直印、一般、橫印）
- 支援報表列印與匯出功能
- 支援財務報表會科角色權限設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `Vouchers` - 傳票主檔 (對應舊系統 `VOUCHER_M`)
```sql
CREATE TABLE [dbo].[Vouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 傳票號碼 (VOUCHER_ID)
    [VoucherDate] DATETIME2 NOT NULL, -- 傳票日期 (VOUCHER_DATE)
    [VoucherKind] NVARCHAR(10) NULL, -- 傳票型態 (VOUCHER_KIND)
    [VoucherType] NVARCHAR(10) NULL, -- 傳票類型 (VOUCHER_TYPE)
    [VoucherStatus] NVARCHAR(10) NOT NULL DEFAULT '1', -- 傳票狀態 (VOUCHER_STATUS, 1:正常, 2:作廢)
    [SiteId] NVARCHAR(50) NULL, -- 分店代號 (SITE_ID)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Vouchers_VoucherId] UNIQUE ([VoucherId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherDate] ON [dbo].[Vouchers] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherKind] ON [dbo].[Vouchers] ([VoucherKind]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherStatus] ON [dbo].[Vouchers] ([VoucherStatus]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_SiteId] ON [dbo].[Vouchers] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_CreatedBy] ON [dbo].[Vouchers] ([CreatedBy]);
```

#### 2.1.2 `VoucherDetails` - 傳票明細 (對應舊系統 `VOUCHER_D`)
```sql
CREATE TABLE [dbo].[VoucherDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherMTKey] BIGINT NOT NULL, -- 傳票主檔TKey (VOUCHERM_T_KEY)
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [StypeId] NVARCHAR(50) NULL, -- 會計科目 (STYPE_ID)
    [Dc] NVARCHAR(1) NULL, -- 借貸 (DC, 0:借方, 1:貸方)
    [Amt] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 金額 (AMT)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_VoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_VoucherDetails_Vouchers] FOREIGN KEY ([VoucherMTKey]) REFERENCES [dbo].[Vouchers] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherMTKey] ON [dbo].[VoucherDetails] ([VoucherMTKey]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_StypeId] ON [dbo].[VoucherDetails] ([StypeId]);
```

#### 2.1.3 `TaxReports` - 稅務報表主檔
```sql
CREATE TABLE [dbo].[TaxReports] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportType] NVARCHAR(20) NOT NULL, -- 報表類型 (REPORT_TYPE)
    [ReportDate] DATETIME2 NOT NULL, -- 報表日期 (REPORT_DATE)
    [SiteId] NVARCHAR(50) NULL, -- 分店代號 (SITE_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TaxReports_ReportType] ON [dbo].[TaxReports] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_TaxReports_ReportDate] ON [dbo].[TaxReports] ([ReportDate]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | NO | - | 傳票號碼 | 唯一 |
| VoucherDate | DATETIME2 | - | NO | - | 傳票日期 | - |
| VoucherKind | NVARCHAR | 10 | YES | - | 傳票型態 | - |
| VoucherStatus | NVARCHAR | 10 | NO | '1' | 傳票狀態 | 1:正常, 2:作廢 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢傳票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tax-reports/vouchers`
- **說明**: 查詢傳票列表，支援多種查詢條件
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "dateFrom": "2024-01-01",
    "dateTo": "2024-12-31",
    "voucherIdFrom": "",
    "voucherIdTo": "",
    "voucherKinds": [],
    "voucherStatuses": [],
    "createdBy": "",
    "createdDateFrom": "",
    "createdDateTo": "",
    "siteId": ""
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
          "voucherId": "V20240101001",
          "voucherDate": "2024-01-01",
          "voucherKind": "GF",
          "voucherStatus": "1",
          "siteId": "S001"
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

#### 3.1.2 查詢傳票明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tax-reports/vouchers/{voucherId}/details`
- **說明**: 查詢傳票明細資料
- **回應格式**: 傳票明細列表

#### 3.1.3 查詢財務報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tax-reports/financial-reports`
- **說明**: 查詢財務報表資料
- **請求參數**:
  ```json
  {
    "reportType": "SYST421",
    "dateFrom": "2024-01-01",
    "dateTo": "2024-12-31",
    "stypeIds": [],
    "siteId": ""
  }
  ```

#### 3.1.4 查詢稅務統計報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tax-reports/tax-statistics`
- **說明**: 查詢稅務統計報表資料
- **請求參數**:
  ```json
  {
    "reportType": "SYST452",
    "dateFrom": "2024-01-01",
    "dateTo": "2024-12-31",
    "siteId": ""
  }
  ```

#### 3.1.5 列印傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/tax-reports/vouchers/print`
- **說明**: 列印傳票（支援多種格式）
- **請求格式**:
  ```json
  {
    "voucherIds": ["V20240101001", "V20240101002"],
    "printType": "2",
    "printSig": "N"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `TaxReportsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/tax-reports")]
    [Authorize]
    public class TaxReportsController : ControllerBase
    {
        private readonly ITaxReportService _taxReportService;
        
        public TaxReportsController(ITaxReportService taxReportService)
        {
            _taxReportService = taxReportService;
        }
        
        [HttpGet("vouchers")]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVouchers([FromQuery] VoucherQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("vouchers/{voucherId}/details")]
        public async Task<ActionResult<ApiResponse<List<VoucherDetailDto>>>> GetVoucherDetails(string voucherId)
        {
            // 實作查詢明細邏輯
        }
        
        [HttpGet("financial-reports")]
        public async Task<ActionResult<ApiResponse<FinancialReportDto>>> GetFinancialReports([FromQuery] FinancialReportQueryDto query)
        {
            // 實作財務報表查詢邏輯
        }
        
        [HttpGet("tax-statistics")]
        public async Task<ActionResult<ApiResponse<TaxStatisticsDto>>> GetTaxStatistics([FromQuery] TaxStatisticsQueryDto query)
        {
            // 實作稅務統計報表查詢邏輯
        }
        
        [HttpPost("vouchers/print")]
        public async Task<ActionResult<ApiResponse<PrintResultDto>>> PrintVouchers([FromBody] PrintVoucherDto dto)
        {
            // 實作列印邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 傳票列印查詢頁面 (`VoucherPrintQuery.vue`)
- **路徑**: `/accounting/tax-reports/voucher-print-query`
- **功能**: 傳票列印查詢，支援多種查詢條件

#### 4.1.2 傳票列印瀏覽頁面 (`VoucherPrintBrowse.vue`)
- **路徑**: `/accounting/tax-reports/voucher-print-browse`
- **功能**: 傳票列印瀏覽與列印

#### 4.1.3 財務報表查詢頁面 (`FinancialReportQuery.vue`)
- **路徑**: `/accounting/tax-reports/financial-report-query`
- **功能**: 財務報表查詢

#### 4.1.4 稅務統計報表查詢頁面 (`TaxStatisticsQuery.vue`)
- **路徑**: `/accounting/tax-reports/tax-statistics-query`
- **功能**: 稅務統計報表查詢

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`VoucherPrintQueryForm.vue`)
```vue
<template>
  <el-form :model="queryForm" :rules="rules" ref="formRef">
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="傳票日期" prop="dateRange">
          <el-date-picker
            v-model="queryForm.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
          />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="傳票號碼" prop="voucherIdRange">
          <el-input v-model="queryForm.voucherIdFrom" placeholder="起始號碼" />
          <span>至</span>
          <el-input v-model="queryForm.voucherIdTo" placeholder="結束號碼" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="傳票型態" prop="voucherKinds">
          <el-select v-model="queryForm.voucherKinds" multiple placeholder="請選擇">
            <el-option label="一般傳票" value="GF" />
            <el-option label="調整傳票" value="AD" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="傳票狀態" prop="voucherStatuses">
          <el-select v-model="queryForm.voucherStatuses" multiple placeholder="請選擇">
            <el-option label="正常" value="1" />
            <el-option label="作廢" value="2" />
          </el-select>
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="建立者" prop="createdBy">
          <el-input v-model="queryForm.createdBy" placeholder="請輸入建立者" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="建立日期" prop="createdDateRange">
          <el-date-picker
            v-model="queryForm.createdDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
          />
        </el-form-item>
      </el-col>
    </el-row>
    <el-form-item>
      <el-button type="primary" @click="handleQuery">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

### 4.3 API 呼叫 (`taxReport.api.ts`)
```typescript
import request from '@/utils/request';

export interface VoucherDto {
  tKey: number;
  voucherId: string;
  voucherDate: string;
  voucherKind?: string;
  voucherType?: string;
  voucherStatus: string;
  siteId?: string;
  notes?: string;
}

export interface VoucherQueryDto {
  pageIndex: number;
  pageSize: number;
  dateFrom?: string;
  dateTo?: string;
  voucherIdFrom?: string;
  voucherIdTo?: string;
  voucherKinds?: string[];
  voucherStatuses?: string[];
  createdBy?: string;
  createdDateFrom?: string;
  createdDateTo?: string;
  siteId?: string;
}

// API 函數
export const getVoucherList = (query: VoucherQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/tax-reports/vouchers', { params: query });
};

export const getVoucherDetails = (voucherId: string) => {
  return request.get<ApiResponse<VoucherDetailDto[]>>(`/api/v1/tax-reports/vouchers/${voucherId}/details`);
};

export const printVouchers = (data: PrintVoucherDto) => {
  return request.post<ApiResponse<PrintResultDto>>('/api/v1/tax-reports/vouchers/print', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 查詢邏輯實作
- [ ] 列印邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 瀏覽頁面開發
- [ ] 列印功能開發
- [ ] 報表展示開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 業務邏輯測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 業務邏輯
- 傳票列印需支援多種格式（直印、一般、橫印）
- 需檢查財務報表會科角色權限設定
- 傳票明細為零金額也需顯示
- 需支援傳票列印邊界參數設定

### 6.2 參數控制
- 傳票列印方式：由參數 `VOUCHER_PRINT` 控制（1:直印, 2:一般, 3:橫印）
- 傳票列印查詢條件預設值：由參數 `PRINT_DATE_DEFAULT` 控制（0:起訖為本月, 1:無預設日期）
- 財務報表會科角色權限設定：由參數 `REPORT_STYPE` 控制

### 6.3 資料驗證
- 至少需輸入一種查詢條件
- 日期格式必須正確
- 傳票號碼範圍必須正確

### 6.4 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 列印功能需支援批次處理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢傳票列表成功
- [ ] 查詢傳票明細成功
- [ ] 查詢財務報表成功
- [ ] 查詢稅務統計報表成功
- [ ] 列印傳票成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 列印功能測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST411_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST411_FB.ASP` - 瀏覽/列印畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST411_PR.ASP` - 報表畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST421_FB.ASP` - 財務報表查詢
- `WEB/IMS_CORE/ASP/SYST000/SYST452_FB.ASP` - 稅務統計報表

### 8.2 資料庫 Schema
- 舊系統資料表：`VOUCHER_M`, `VOUCHER_D`
- 主要欄位：VOUCHER_ID, VOUCHER_DATE, VOUCHER_KIND, VOUCHER_STATUS, STYPE_ID, DC, AMT

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


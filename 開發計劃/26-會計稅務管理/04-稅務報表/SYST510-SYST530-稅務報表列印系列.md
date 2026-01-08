# SYST510-SYST530 - 稅務報表列印系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST510-SYST530 系列
- **功能名稱**: 稅務報表列印系列
- **功能描述**: 提供稅務報表列印功能，包含SAP拋轉-銀行往來、稅務報表列印等，支援多種報表格式與匯出功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST510_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST510_TF.ASP` (寫檔/列印)
  - `WEB/IMS_CORE/ASP/SYST000/SYST520_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST520_TF.ASP` (寫檔/列印)
  - `WEB/IMS_CORE/ASP/SYST000/SYST530_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYST530_TF.ASP` (寫檔/列印)

### 1.2 業務需求
- 支援SAP拋轉-銀行往來報表列印
- 支援依日期範圍查詢銀行往來資料
- 支援CSV檔案匯出功能（供SAP系統使用）
- 支援稅務報表列印（多種報表類型）
- 支援報表格式設定（直印、一般、橫印）
- 支援報表列印與匯出功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `SapBankTotal` - SAP銀行往來總表 (對應舊系統 `SAP_BANKTOTAL`)
```sql
CREATE TABLE [dbo].[SapBankTotal] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SapDate] NVARCHAR(8) NOT NULL, -- SAP日期 (SAP_DATE, YYYYMMDD)
    [SapStypeId] NVARCHAR(50) NULL, -- SAP會計科目 (SAP_STYPE_ID)
    [CompId] NVARCHAR(10) NOT NULL, -- 公司代號 (COMP_ID)
    [BankAmt] DECIMAL(18, 2) NULL DEFAULT 0, -- 銀行金額 (BANK_AMT)
    [BankBalance] DECIMAL(18, 2) NULL DEFAULT 0, -- 銀行餘額 (BANK_BALANCE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_SapBankTotal] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SapBankTotal_SapDate] ON [dbo].[SapBankTotal] ([SapDate]);
CREATE NONCLUSTERED INDEX [IX_SapBankTotal_CompId] ON [dbo].[SapBankTotal] ([CompId]);
CREATE NONCLUSTERED INDEX [IX_SapBankTotal_SapStypeId] ON [dbo].[SapBankTotal] ([SapStypeId]);
CREATE NONCLUSTERED INDEX [IX_SapBankTotal_SapDate_CompId] ON [dbo].[SapBankTotal] ([SapDate], [CompId]);
```

#### 2.1.2 `TaxReportPrints` - 稅務報表列印記錄
```sql
CREATE TABLE [dbo].[TaxReportPrints] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportType] NVARCHAR(20) NOT NULL, -- 報表類型 (REPORT_TYPE, SYST510/SYST520/SYST530)
    [ReportDate] DATETIME2 NOT NULL, -- 報表日期 (REPORT_DATE)
    [DateFrom] DATETIME2 NULL, -- 查詢起始日期 (DATE_FROM)
    [DateTo] DATETIME2 NULL, -- 查詢結束日期 (DATE_TO)
    [CompId] NVARCHAR(10) NULL, -- 公司代號 (COMP_ID)
    [FileName] NVARCHAR(200) NULL, -- 檔案名稱 (FILE_NAME)
    [FileFormat] NVARCHAR(10) NULL, -- 檔案格式 (FILE_FORMAT, CSV/PDF/EXCEL)
    [PrintStatus] NVARCHAR(10) NOT NULL DEFAULT '1', -- 列印狀態 (PRINT_STATUS, 1:成功, 2:失敗)
    [PrintCount] INT NULL DEFAULT 0, -- 列印次數 (PRINT_COUNT)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    CONSTRAINT [PK_TaxReportPrints] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TaxReportPrints_ReportType] ON [dbo].[TaxReportPrints] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_TaxReportPrints_ReportDate] ON [dbo].[TaxReportPrints] ([ReportDate]);
CREATE NONCLUSTERED INDEX [IX_TaxReportPrints_DateFrom_DateTo] ON [dbo].[TaxReportPrints] ([DateFrom], [DateTo]);
```

### 2.2 相關資料表

#### 2.2.1 `AccountSubjects` - 會計科目主檔
- 參考 `SYST111-SYST11A-會計科目維護系列.md` 的資料表設計

#### 2.2.2 `Companies` - 公司主檔
```sql
-- 假設已存在公司主檔，用於儲存公司基本資料
-- 包含 COMP_ID, COMP_NAME 等欄位
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| SapDate | NVARCHAR | 8 | NO | - | SAP日期 | YYYYMMDD格式 |
| SapStypeId | NVARCHAR | 50 | YES | - | SAP會計科目 | - |
| CompId | NVARCHAR | 10 | NO | - | 公司代號 | - |
| BankAmt | DECIMAL | 18,2 | YES | 0 | 銀行金額 | - |
| BankBalance | DECIMAL | 18,2 | YES | 0 | 銀行餘額 | - |
| ReportType | NVARCHAR | 20 | NO | - | 報表類型 | SYST510/SYST520/SYST530 |
| DateFrom | DATETIME2 | - | YES | - | 查詢起始日期 | - |
| DateTo | DATETIME2 | - | YES | - | 查詢結束日期 | - |
| FileName | NVARCHAR | 200 | YES | - | 檔案名稱 | - |
| FileFormat | NVARCHAR | 10 | YES | - | 檔案格式 | CSV/PDF/EXCEL |
| PrintStatus | NVARCHAR | 10 | NO | '1' | 列印狀態 | 1:成功, 2:失敗 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢SAP銀行往來資料 (SYST510)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tax-reports/syst510/bank-total`
- **說明**: 查詢SAP銀行往來資料，支援依日期範圍查詢
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "dateFrom": "2024-01-01",
    "dateTo": "2024-12-31",
    "compId": "",
    "sapStypeId": ""
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
          "sapDate": "20240101",
          "sapStypeId": "1000",
          "compId": "001",
          "bankAmt": 100000.00,
          "bankBalance": 500000.00
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

#### 3.1.2 產生SAP銀行往來CSV檔案 (SYST510)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/tax-reports/syst510/generate-csv`
- **說明**: 產生SAP銀行往來CSV檔案
- **請求格式**:
  ```json
  {
    "dateFrom": "2024-01-01",
    "dateTo": "2024-12-31",
    "compId": "001"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "檔案產生成功",
    "data": {
      "fileName": "001_20241231_BANKTOTAL.CSV",
      "fileUrl": "/api/v1/tax-reports/syst510/download/001_20241231_BANKTOTAL.CSV",
      "fileSize": 1024
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 下載SAP銀行往來CSV檔案 (SYST510)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tax-reports/syst510/download/{fileName}`
- **說明**: 下載產生的CSV檔案
- **路徑參數**:
  - `fileName`: 檔案名稱
- **回應**: CSV檔案內容

#### 3.1.4 查詢稅務報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tax-reports/print-logs`
- **說明**: 查詢稅務報表列印記錄
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "reportType": "SYST510",
    "dateFrom": "",
    "dateTo": "",
    "printStatus": ""
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.5 新增稅務報表列印記錄
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/tax-reports/print-logs`
- **說明**: 新增稅務報表列印記錄
- **請求格式**:
  ```json
  {
    "reportType": "SYST510",
    "reportDate": "2024-01-01",
    "dateFrom": "2024-01-01",
    "dateTo": "2024-12-31",
    "compId": "001",
    "fileName": "001_20241231_BANKTOTAL.CSV",
    "fileFormat": "CSV",
    "printStatus": "1"
  }
  ```
- **回應格式**: 標準回應格式

#### 3.1.6 修改稅務報表列印記錄
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/tax-reports/print-logs/{tKey}`
- **說明**: 修改稅務報表列印記錄
- **路徑參數**:
  - `tKey`: 記錄主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 標準回應格式

#### 3.1.7 刪除稅務報表列印記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/tax-reports/print-logs/{tKey}`
- **說明**: 刪除稅務報表列印記錄
- **路徑參數**:
  - `tKey`: 記錄主鍵
- **回應格式**: 標準回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `TaxReportPrintController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/tax-reports")]
    [Authorize]
    public class TaxReportPrintController : ControllerBase
    {
        private readonly ITaxReportPrintService _taxReportPrintService;
        
        public TaxReportPrintController(ITaxReportPrintService taxReportPrintService)
        {
            _taxReportPrintService = taxReportPrintService;
        }
        
        [HttpGet("syst510/bank-total")]
        public async Task<ActionResult<ApiResponse<PagedResult<SapBankTotalDto>>>> GetSapBankTotal([FromQuery] SapBankTotalQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpPost("syst510/generate-csv")]
        public async Task<ActionResult<ApiResponse<CsvFileDto>>> GenerateSapBankTotalCsv([FromBody] GenerateCsvDto dto)
        {
            // 實作CSV產生邏輯
        }
        
        [HttpGet("syst510/download/{fileName}")]
        public async Task<IActionResult> DownloadCsv(string fileName)
        {
            // 實作檔案下載邏輯
        }
        
        [HttpGet("print-logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<TaxReportPrintDto>>>> GetPrintLogs([FromQuery] TaxReportPrintQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpPost("print-logs")]
        public async Task<ActionResult<ApiResponse<long>>> CreatePrintLog([FromBody] CreateTaxReportPrintDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("print-logs/{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdatePrintLog(long tKey, [FromBody] UpdateTaxReportPrintDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("print-logs/{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeletePrintLog(long tKey)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `TaxReportPrintService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ITaxReportPrintService
    {
        Task<PagedResult<SapBankTotalDto>> GetSapBankTotalAsync(SapBankTotalQueryDto query);
        Task<CsvFileDto> GenerateSapBankTotalCsvAsync(GenerateCsvDto dto);
        Task<Stream> DownloadCsvAsync(string fileName);
        Task<PagedResult<TaxReportPrintDto>> GetPrintLogsAsync(TaxReportPrintQueryDto query);
        Task<long> CreatePrintLogAsync(CreateTaxReportPrintDto dto);
        Task UpdatePrintLogAsync(long tKey, UpdateTaxReportPrintDto dto);
        Task DeletePrintLogAsync(long tKey);
    }
}
```

#### 3.2.3 Repository: `TaxReportPrintRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ITaxReportPrintRepository
    {
        Task<PagedResult<SapBankTotal>> GetSapBankTotalPagedAsync(SapBankTotalQuery query);
        Task<List<SapBankTotal>> GetSapBankTotalListAsync(SapBankTotalQuery query);
        Task<TaxReportPrint> GetPrintLogByIdAsync(long tKey);
        Task<PagedResult<TaxReportPrint>> GetPrintLogsPagedAsync(TaxReportPrintQuery query);
        Task<TaxReportPrint> CreatePrintLogAsync(TaxReportPrint printLog);
        Task<TaxReportPrint> UpdatePrintLogAsync(TaxReportPrint printLog);
        Task DeletePrintLogAsync(long tKey);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 SAP銀行往來查詢頁面 (`SapBankTotalQuery.vue`)
- **路徑**: `/accounting/tax-reports/syst510/bank-total`
- **功能**: 查詢SAP銀行往來資料，支援CSV匯出
- **主要元件**:
  - 查詢表單 (SapBankTotalSearchForm)
  - 資料表格 (SapBankTotalDataTable)
  - CSV產生按鈕

#### 4.1.2 稅務報表列印記錄頁面 (`TaxReportPrintLogList.vue`)
- **路徑**: `/accounting/tax-reports/print-logs`
- **功能**: 顯示稅務報表列印記錄，支援新增、修改、刪除、查詢
- **主要元件**:
  - 查詢表單 (TaxReportPrintLogSearchForm)
  - 資料表格 (TaxReportPrintLogDataTable)
  - 新增/修改對話框 (TaxReportPrintLogDialog)

### 4.2 UI 元件設計

#### 4.2.1 SAP銀行往來查詢表單 (`SapBankTotalSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="日期起">
      <el-date-picker
        v-model="searchForm.dateFrom"
        type="date"
        placeholder="請選擇起始日期"
        format="YYYY-MM-DD"
        value-format="YYYY-MM-DD"
      />
    </el-form-item>
    <el-form-item label="日期迄">
      <el-date-picker
        v-model="searchForm.dateTo"
        type="date"
        placeholder="請選擇結束日期"
        format="YYYY-MM-DD"
        value-format="YYYY-MM-DD"
      />
    </el-form-item>
    <el-form-item label="公司代號">
      <el-input v-model="searchForm.compId" placeholder="請輸入公司代號" />
    </el-form-item>
    <el-form-item label="SAP會計科目">
      <el-input v-model="searchForm.sapStypeId" placeholder="請輸入SAP會計科目" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleGenerateCsv">產生CSV</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 SAP銀行往來資料表格 (`SapBankTotalDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="bankTotalList" v-loading="loading">
      <el-table-column prop="sapDate" label="日期" width="120" />
      <el-table-column prop="sapStypeId" label="SAP會計科目" width="150" />
      <el-table-column prop="compId" label="公司代號" width="120" />
      <el-table-column prop="bankAmt" label="銀行金額" width="150" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.bankAmt) }}
        </template>
      </el-table-column>
      <el-table-column prop="bankBalance" label="銀行餘額" width="150" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.bankBalance) }}
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

#### 4.2.3 稅務報表列印記錄對話框 (`TaxReportPrintLogDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="報表類型" prop="reportType">
            <el-select v-model="form.reportType" placeholder="請選擇">
              <el-option label="SYST510-SAP銀行往來" value="SYST510" />
              <el-option label="SYST520" value="SYST520" />
              <el-option label="SYST530" value="SYST530" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="報表日期" prop="reportDate">
            <el-date-picker
              v-model="form.reportDate"
              type="date"
              placeholder="請選擇報表日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="查詢起始日期" prop="dateFrom">
            <el-date-picker
              v-model="form.dateFrom"
              type="date"
              placeholder="請選擇起始日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="查詢結束日期" prop="dateTo">
            <el-date-picker
              v-model="form.dateTo"
              type="date"
              placeholder="請選擇結束日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="公司代號" prop="compId">
            <el-input v-model="form.compId" placeholder="請輸入公司代號" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="檔案格式" prop="fileFormat">
            <el-select v-model="form.fileFormat" placeholder="請選擇">
              <el-option label="CSV" value="CSV" />
              <el-option label="PDF" value="PDF" />
              <el-option label="EXCEL" value="EXCEL" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="列印狀態" prop="printStatus">
            <el-select v-model="form.printStatus" placeholder="請選擇">
              <el-option label="成功" value="1" />
              <el-option label="失敗" value="2" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="備註" prop="notes">
            <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
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

### 4.3 API 呼叫 (`taxReportPrint.api.ts`)
```typescript
import request from '@/utils/request';

export interface SapBankTotalDto {
  tKey: number;
  sapDate: string;
  sapStypeId?: string;
  compId: string;
  bankAmt?: number;
  bankBalance?: number;
}

export interface SapBankTotalQueryDto {
  pageIndex: number;
  pageSize: number;
  dateFrom?: string;
  dateTo?: string;
  compId?: string;
  sapStypeId?: string;
}

export interface GenerateCsvDto {
  dateFrom: string;
  dateTo: string;
  compId: string;
}

export interface CsvFileDto {
  fileName: string;
  fileUrl: string;
  fileSize: number;
}

export interface TaxReportPrintDto {
  tKey: number;
  reportType: string;
  reportDate: string;
  dateFrom?: string;
  dateTo?: string;
  compId?: string;
  fileName?: string;
  fileFormat?: string;
  printStatus: string;
  printCount?: number;
  notes?: string;
  createdBy?: string;
  createdAt: string;
}

export interface TaxReportPrintQueryDto {
  pageIndex: number;
  pageSize: number;
  reportType?: string;
  dateFrom?: string;
  dateTo?: string;
  printStatus?: string;
}

export interface CreateTaxReportPrintDto {
  reportType: string;
  reportDate: string;
  dateFrom?: string;
  dateTo?: string;
  compId?: string;
  fileName?: string;
  fileFormat?: string;
  printStatus: string;
  notes?: string;
}

export interface UpdateTaxReportPrintDto extends Omit<CreateTaxReportPrintDto, 'reportType'> {}

// API 函數
export const getSapBankTotalList = (query: SapBankTotalQueryDto) => {
  return request.get<ApiResponse<PagedResult<SapBankTotalDto>>>('/api/v1/tax-reports/syst510/bank-total', { params: query });
};

export const generateSapBankTotalCsv = (data: GenerateCsvDto) => {
  return request.post<ApiResponse<CsvFileDto>>('/api/v1/tax-reports/syst510/generate-csv', data);
};

export const downloadCsv = (fileName: string) => {
  return request.get(`/api/v1/tax-reports/syst510/download/${fileName}`, {
    responseType: 'blob'
  });
};

export const getTaxReportPrintLogList = (query: TaxReportPrintQueryDto) => {
  return request.get<ApiResponse<PagedResult<TaxReportPrintDto>>>('/api/v1/tax-reports/print-logs', { params: query });
};

export const createTaxReportPrintLog = (data: CreateTaxReportPrintDto) => {
  return request.post<ApiResponse<number>>('/api/v1/tax-reports/print-logs', data);
};

export const updateTaxReportPrintLog = (tKey: number, data: UpdateTaxReportPrintDto) => {
  return request.put<ApiResponse>(`/api/v1/tax-reports/print-logs/${tKey}`, data);
};

export const deleteTaxReportPrintLog = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/tax-reports/print-logs/${tKey}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立預設值
- [ ] 資料庫遷移腳本
- [ ] 建立預存程序 (SP_BANKTOTAL)

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] CSV產生邏輯實作
- [ ] 檔案下載邏輯實作
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] SAP銀行往來查詢頁面開發
- [ ] CSV產生功能開發
- [ ] 檔案下載功能開發
- [ ] 稅務報表列印記錄頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] CSV檔案格式測試
- [ ] 檔案下載測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 13天

---

## 六、注意事項

### 6.1 業務邏輯
- 日期範圍必須驗證（起始日期不能大於結束日期）
- CSV檔案名稱格式：`{COMP_ID}_{YYYYMMDD}_BANKTOTAL.CSV`
- 產生CSV前需先呼叫預存程序 `SP_BANKTOTAL` 處理資料
- 檔案下載需檢查檔案是否存在
- 列印記錄需記錄列印次數

### 6.2 資料驗證
- 日期格式必須正確
- 公司代號必須存在
- 檔案格式必須為支援的格式（CSV/PDF/EXCEL）

### 6.3 效能
- 大量資料查詢必須使用分頁
- CSV檔案產生需考慮大量資料的處理
- 必須建立適當的索引
- 檔案下載需使用串流方式

### 6.4 安全性
- 檔案下載需驗證使用者權限
- 檔案路徑需防止路徑遍歷攻擊
- CSV檔案內容需防止注入攻擊

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢SAP銀行往來資料成功
- [ ] 產生CSV檔案成功
- [ ] 下載CSV檔案成功
- [ ] 新增列印記錄成功
- [ ] 修改列印記錄成功
- [ ] 刪除列印記錄成功
- [ ] 查詢列印記錄列表成功

### 7.2 整合測試
- [ ] 完整 CSV 產生與下載流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 大量資料處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] CSV檔案產生效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST510_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST510_TF.ASP` - 寫檔畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST520_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST520_TF.ASP` - 寫檔畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST530_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST530_TF.ASP` - 寫檔畫面

### 8.2 資料庫 Schema
- 舊系統資料表：`SAP_BANKTOTAL`
- 預存程序：`SP_BANKTOTAL`
- 主要欄位：SAP_DATE, SAP_STYPE_ID, COMP_ID, BANK_AMT, BANK_BALANCE

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


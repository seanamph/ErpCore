# SYSN140-SYSN141 - 會計報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN140-SYSN141 系列
- **功能名稱**: 會計報表查詢系列
- **功能描述**: 提供會計報表查詢功能，包含各種會計報表的查詢、列印、匯出等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN140_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN140_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN140_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN141_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN141_PR.ASP` (報表)

### 1.2 業務需求
- 提供會計報表查詢功能
- 支援多種報表類型查詢
- 支援報表列印功能
- 支援報表匯出功能（Excel/PDF）
- 支援報表參數設定
- 支援報表資料篩選
- 支援報表排序功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `AccountingReports` (對應舊系統相關報表資料)

```sql
CREATE TABLE [dbo].[AccountingReports] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportId] NVARCHAR(50) NOT NULL, -- 報表代號
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [ReportType] NVARCHAR(50) NULL, -- 報表類型
    [ReportDate] DATETIME2 NULL, -- 報表日期
    [Period] NVARCHAR(20) NULL, -- 期間
    [Year] INT NULL, -- 年度
    [Month] INT NULL, -- 月份
    [Status] NVARCHAR(10) NULL DEFAULT 'A', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_AccountingReports] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_AccountingReports_ReportId] ON [dbo].[AccountingReports] ([ReportId]);
CREATE NONCLUSTERED INDEX [IX_AccountingReports_ReportType] ON [dbo].[AccountingReports] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_AccountingReports_ReportDate] ON [dbo].[AccountingReports] ([ReportDate]);
CREATE NONCLUSTERED INDEX [IX_AccountingReports_YearMonth] ON [dbo].[AccountingReports] ([Year], [Month]);
```

### 2.2 報表明細資料表: `AccountingReportDetails`

```sql
CREATE TABLE [dbo].[AccountingReportDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportTKey] BIGINT NOT NULL, -- 報表主檔TKey
    [AccountSubjectId] NVARCHAR(50) NULL, -- 會計科目代號
    [AccountSubjectName] NVARCHAR(200) NULL, -- 會計科目名稱
    [DebitAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 借方金額
    [CreditAmount] DECIMAL(18, 2) NULL DEFAULT 0, -- 貸方金額
    [Balance] DECIMAL(18, 2) NULL DEFAULT 0, -- 餘額
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_AccountingReportDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_AccountingReportDetails_AccountingReports] FOREIGN KEY ([ReportTKey]) REFERENCES [dbo].[AccountingReports] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_AccountingReportDetails_ReportTKey] ON [dbo].[AccountingReportDetails] ([ReportTKey]);
CREATE NONCLUSTERED INDEX [IX_AccountingReportDetails_AccountSubjectId] ON [dbo].[AccountingReportDetails] ([AccountSubjectId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ReportId | NVARCHAR | 50 | NO | - | 報表代號 | 唯一 |
| ReportName | NVARCHAR | 200 | NO | - | 報表名稱 | - |
| ReportType | NVARCHAR | 50 | YES | - | 報表類型 | - |
| ReportDate | DATETIME2 | - | YES | - | 報表日期 | - |
| Period | NVARCHAR | 20 | YES | - | 期間 | - |
| Year | INT | - | YES | - | 年度 | - |
| Month | INT | - | YES | - | 月份 | - |
| Status | NVARCHAR | 10 | YES | 'A' | 狀態 | A:啟用, I:停用 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/accounting/reports`
- **說明**: 查詢會計報表列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ReportDate",
    "sortOrder": "DESC",
    "filters": {
      "reportId": "",
      "reportName": "",
      "reportType": "",
      "year": null,
      "month": null,
      "startDate": "",
      "endDate": ""
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
          "reportId": "RPT001",
          "reportName": "資產負債表",
          "reportType": "BALANCE_SHEET",
          "reportDate": "2024-01-31",
          "year": 2024,
          "month": 1,
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

#### 3.1.2 查詢單筆報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/accounting/reports/{tKey}`
- **說明**: 根據TKey查詢單筆報表資料
- **路徑參數**:
  - `tKey`: 報表主鍵
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "reportId": "RPT001",
      "reportName": "資產負債表",
      "reportType": "BALANCE_SHEET",
      "reportDate": "2024-01-31",
      "year": 2024,
      "month": 1,
      "status": "A",
      "details": [
        {
          "tKey": 1,
          "accountSubjectId": "1000",
          "accountSubjectName": "現金",
          "debitAmount": 1000000,
          "creditAmount": 0,
          "balance": 1000000
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 產生報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/accounting/reports/generate`
- **說明**: 根據參數產生會計報表
- **請求格式**:
  ```json
  {
    "reportType": "BALANCE_SHEET",
    "year": 2024,
    "month": 1,
    "parameters": {
      "accountSubjectId": "",
      "departmentId": "",
      "includeDetails": true
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "報表產生成功",
    "data": {
      "tKey": 1,
      "reportId": "RPT001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 列印報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/accounting/reports/{tKey}/print`
- **說明**: 列印報表
- **路徑參數**:
  - `tKey`: 報表主鍵
- **請求格式**:
  ```json
  {
    "format": "PDF",
    "template": "default"
  }
  ```
- **回應格式**: 返回PDF文件流

#### 3.1.5 匯出報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/accounting/reports/{tKey}/export`
- **說明**: 匯出報表（Excel/PDF）
- **路徑參數**:
  - `tKey`: 報表主鍵
- **請求格式**:
  ```json
  {
    "format": "EXCEL",
    "includeDetails": true
  }
  ```
- **回應格式**: 返回文件流

### 3.2 後端實作類別

#### 3.2.1 Controller: `AccountingReportsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/accounting/reports")]
    [Authorize]
    public class AccountingReportsController : ControllerBase
    {
        private readonly IAccountingReportService _reportService;
        
        public AccountingReportsController(IAccountingReportService reportService)
        {
            _reportService = reportService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AccountingReportDto>>>> GetReports([FromQuery] AccountingReportQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<AccountingReportDto>>> GetReport(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost("generate")]
        public async Task<ActionResult<ApiResponse<long>>> GenerateReport([FromBody] GenerateReportDto dto)
        {
            // 實作產生報表邏輯
        }
        
        [HttpPost("{tKey}/print")]
        public async Task<ActionResult> PrintReport(long tKey, [FromBody] PrintReportDto dto)
        {
            // 實作列印邏輯
        }
        
        [HttpPost("{tKey}/export")]
        public async Task<ActionResult> ExportReport(long tKey, [FromBody] ExportReportDto dto)
        {
            // 實作匯出邏輯
        }
    }
}
```

#### 3.2.2 Service: `AccountingReportService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IAccountingReportService
    {
        Task<PagedResult<AccountingReportDto>> GetReportsAsync(AccountingReportQueryDto query);
        Task<AccountingReportDto> GetReportByIdAsync(long tKey);
        Task<long> GenerateReportAsync(GenerateReportDto dto);
        Task<byte[]> PrintReportAsync(long tKey, PrintReportDto dto);
        Task<byte[]> ExportReportAsync(long tKey, ExportReportDto dto);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 報表查詢頁面 (`AccountingReportList.vue`)
- **路徑**: `/accounting/reports`
- **功能**: 顯示報表列表，支援查詢、產生、列印、匯出
- **主要元件**:
  - 查詢表單 (ReportSearchForm)
  - 資料表格 (ReportDataTable)
  - 產生報表對話框 (GenerateReportDialog)
  - 報表預覽對話框 (ReportPreviewDialog)

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ReportSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="報表代號">
      <el-input v-model="searchForm.reportId" placeholder="請輸入報表代號" />
    </el-form-item>
    <el-form-item label="報表名稱">
      <el-input v-model="searchForm.reportName" placeholder="請輸入報表名稱" />
    </el-form-item>
    <el-form-item label="報表類型">
      <el-select v-model="searchForm.reportType" placeholder="請選擇報表類型">
        <el-option label="資產負債表" value="BALANCE_SHEET" />
        <el-option label="損益表" value="INCOME_STATEMENT" />
        <el-option label="現金流量表" value="CASH_FLOW" />
      </el-select>
    </el-form-item>
    <el-form-item label="年度">
      <el-input-number v-model="searchForm.year" :min="2000" :max="2099" />
    </el-form-item>
    <el-form-item label="月份">
      <el-input-number v-model="searchForm.month" :min="1" :max="12" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`ReportDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="reportList" v-loading="loading">
      <el-table-column prop="reportId" label="報表代號" width="120" />
      <el-table-column prop="reportName" label="報表名稱" width="200" />
      <el-table-column prop="reportType" label="報表類型" width="150">
        <template #default="{ row }">
          {{ getReportTypeText(row.reportType) }}
        </template>
      </el-table-column>
      <el-table-column prop="reportDate" label="報表日期" width="120" />
      <el-table-column prop="year" label="年度" width="80" />
      <el-table-column prop="month" label="月份" width="80" />
      <el-table-column label="操作" width="300" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
          <el-button type="success" size="small" @click="handlePrint(row)">列印</el-button>
          <el-button type="warning" size="small" @click="handleExport(row)">匯出</el-button>
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

### 4.3 API 呼叫 (`accountingReport.api.ts`)
```typescript
import request from '@/utils/request';

export interface AccountingReportDto {
  tKey: number;
  reportId: string;
  reportName: string;
  reportType?: string;
  reportDate?: string;
  year?: number;
  month?: number;
  status: string;
  details?: AccountingReportDetailDto[];
}

export interface AccountingReportQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    reportId?: string;
    reportName?: string;
    reportType?: string;
    year?: number;
    month?: number;
    startDate?: string;
    endDate?: string;
  };
}

// API 函數
export const getReportList = (query: AccountingReportQueryDto) => {
  return request.get<ApiResponse<PagedResult<AccountingReportDto>>>('/api/v1/accounting/reports', { params: query });
};

export const getReportById = (tKey: number) => {
  return request.get<ApiResponse<AccountingReportDto>>(`/api/v1/accounting/reports/${tKey}`);
};

export const generateReport = (data: GenerateReportDto) => {
  return request.post<ApiResponse<number>>('/api/v1/accounting/reports/generate', data);
};

export const printReport = (tKey: number, data: PrintReportDto) => {
  return request.post(`/api/v1/accounting/reports/${tKey}/print`, data, { responseType: 'blob' });
};

export const exportReport = (tKey: number, data: ExportReportDto) => {
  return request.post(`/api/v1/accounting/reports/${tKey}/export`, data, { responseType: 'blob' });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表產生邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 產生報表對話框開發
- [ ] 報表預覽功能開發
- [ ] 列印功能開發
- [ ] 匯出功能開發

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 效能
- 大量資料查詢必須使用分頁
- 報表產生必須使用非同步處理
- 必須建立適當的索引
- 必須使用快取機制

### 6.2 資料驗證
- 報表參數必須驗證
- 日期範圍必須驗證
- 報表類型必須在允許範圍內

### 6.3 業務邏輯
- 報表產生前必須檢查資料完整性
- 報表列印必須支援多種格式
- 報表匯出必須支援Excel和PDF格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢報表列表成功
- [ ] 查詢單筆報表成功
- [ ] 產生報表成功
- [ ] 列印報表成功
- [ ] 匯出報表成功

### 7.2 整合測試
- [ ] 完整報表查詢流程測試
- [ ] 報表產生流程測試
- [ ] 報表列印流程測試
- [ ] 報表匯出流程測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN140_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN140_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN140_PR.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN141_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN141_PR.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


# SYSGB10-SYSGB40 - 新版本人事報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSGB10-SYSGB40
- **功能名稱**: 新版本人事報表查詢系列
- **功能描述**: 提供新版本人事相關報表的查詢功能，包含薪資暨各類所得明細表、人事統計報表等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB10_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB10_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB20_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB20_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB30_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB30_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB40_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB40_PR.ASP` (報表)

### 1.2 業務需求
- **SYSGB10**: 薪資暨各類所得明細表查詢與報表
- **SYSGB20**: 人事統計報表查詢與報表
- **SYSGB30**: 人事分析報表查詢與報表
- **SYSGB40**: 人事彙總報表查詢與報表
- 支援多條件查詢
- 支援報表列印與匯出
- 支援資料統計與分析

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `SalaryDetails` - 薪資明細表
```sql
CREATE TABLE [dbo].[SalaryDetails] (
    [SalaryDetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [RevenueSN] NVARCHAR(50) NULL, -- 所得人代號
    [IDCode] NVARCHAR(10) NULL, -- 證號別
    [PID] NVARCHAR(10) NULL, -- 身分證號/統一編號
    [PayYM] NVARCHAR(6) NOT NULL, -- 所得給付年月 (YYYYMM)
    [VoucherId] NVARCHAR(11) NULL, -- 傳票編號
    [FormCode] NVARCHAR(2) NULL, -- 所得格式
    [FormNotes] NVARCHAR(1) NULL, -- 所得註記
    [ExecCode] NVARCHAR(2) NULL, -- 執行業務者業別代號
    [STypeId] NVARCHAR(8) NULL, -- 科目代號
    [Notes] NVARCHAR(500) NULL, -- 科目摘要
    [NetAmt] DECIMAL(18, 2) NULL, -- 給付淨額
    [TxAmt] DECIMAL(18, 2) NULL, -- 扣繳稅額
    [GrossAmt] DECIMAL(18, 2) NULL, -- 給付總額
    [TaxRate] DECIMAL(5, 2) NULL, -- 稅率(%)
    [CheckStatus] NVARCHAR(10) NULL, -- 審核狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SalaryDetails] PRIMARY KEY CLUSTERED ([SalaryDetailId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalaryDetails_RevenueSN] ON [dbo].[SalaryDetails] ([RevenueSN]);
CREATE NONCLUSTERED INDEX [IX_SalaryDetails_PayYM] ON [dbo].[SalaryDetails] ([PayYM]);
CREATE NONCLUSTERED INDEX [IX_SalaryDetails_PID] ON [dbo].[SalaryDetails] ([PID]);
CREATE NONCLUSTERED INDEX [IX_SalaryDetails_VoucherId] ON [dbo].[SalaryDetails] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_SalaryDetails_CheckStatus] ON [dbo].[SalaryDetails] ([CheckStatus]);
```

#### 2.1.2 `IDKind` - 證號別資料表
```sql
CREATE TABLE [dbo].[IDKind] (
    [IDCode] NVARCHAR(10) NOT NULL PRIMARY KEY,
    [Content] NVARCHAR(100) NOT NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_IDKind] PRIMARY KEY CLUSTERED ([IDCode] ASC)
);
```

#### 2.1.3 `RevenueKind` - 所得種類資料表
```sql
CREATE TABLE [dbo].[RevenueKind] (
    [FormCode] NVARCHAR(2) NOT NULL,
    [FormNotes] NVARCHAR(1) NULL,
    [Description] NVARCHAR(200) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_RevenueKind] PRIMARY KEY CLUSTERED ([FormCode] ASC)
);
```

#### 2.1.4 `ExecKind` - 執行業務者業別資料表
```sql
CREATE TABLE [dbo].[ExecKind] (
    [ExecCode] NVARCHAR(2) NOT NULL PRIMARY KEY,
    [Content] NVARCHAR(200) NOT NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ExecKind] PRIMARY KEY CLUSTERED ([ExecCode] ASC)
);
```

### 2.2 資料字典

#### SalaryDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| SalaryDetailId | UNIQUEIDENTIFIER | - | NO | NEWID() | 薪資明細編號 | 主鍵 |
| RevenueSN | NVARCHAR | 50 | YES | - | 所得人代號 | - |
| IDCode | NVARCHAR | 10 | YES | - | 證號別 | 外鍵至IDKind |
| PID | NVARCHAR | 10 | YES | - | 身分證號/統一編號 | - |
| PayYM | NVARCHAR | 6 | NO | - | 所得給付年月 | YYYYMM格式 |
| VoucherId | NVARCHAR | 11 | YES | - | 傳票編號 | - |
| FormCode | NVARCHAR | 2 | YES | - | 所得格式 | 外鍵至RevenueKind |
| FormNotes | NVARCHAR | 1 | YES | - | 所得註記 | - |
| ExecCode | NVARCHAR | 2 | YES | - | 執行業務者業別代號 | 外鍵至ExecKind |
| STypeId | NVARCHAR | 8 | YES | - | 科目代號 | - |
| Notes | NVARCHAR | 500 | YES | - | 科目摘要 | - |
| NetAmt | DECIMAL | 18,2 | YES | - | 給付淨額 | - |
| TxAmt | DECIMAL | 18,2 | YES | - | 扣繳稅額 | - |
| GrossAmt | DECIMAL | 18,2 | YES | - | 給付總額 | - |
| TaxRate | DECIMAL | 5,2 | YES | - | 稅率(%) | - |
| CheckStatus | NVARCHAR | 10 | YES | - | 審核狀態 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢薪資明細列表 (SYSGB10)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/salary-details/query`
- **說明**: 查詢薪資暨各類所得明細表，支援多條件篩選
- **請求格式**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "idCode": "",
      "pid": "",
      "payYMBegin": "202401",
      "payYMEnd": "202412",
      "voucherId": "",
      "formCode": "",
      "formNotes": "",
      "execCode": "",
      "stypeId": "",
      "notes": "",
      "netAmtFrom": null,
      "netAmtTo": null,
      "txAmtFrom": null,
      "txAmtTo": null,
      "checkStatus": ""
    },
    "sortField": "PayYM",
    "sortOrder": "DESC"
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
          "salaryDetailId": "guid",
          "revenueSN": "R001",
          "idCode": "01",
          "idCodeName": "身分證",
          "pid": "A123456789",
          "payYM": "202401",
          "voucherId": "V001",
          "formCode": "50",
          "formNotes": "1",
          "execCode": "01",
          "execCodeName": "執行業務",
          "stypeId": "5101",
          "notes": "薪資",
          "netAmt": 50000.00,
          "txAmt": 5000.00,
          "grossAmt": 55000.00,
          "taxRate": 10.00,
          "checkStatus": "Y"
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

#### 3.1.2 匯出薪資明細報表 (SYSGB10)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/salary-details/export`
- **說明**: 匯出薪資明細報表 (Excel/PDF)
- **請求格式**: 同查詢
- **回應格式**: 檔案下載

#### 3.1.3 查詢人事統計報表 (SYSGB20)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/personnel-statistics/query`
- **說明**: 查詢人事統計報表
- **請求格式**: 類似SYSGB10，但統計維度不同
- **回應格式**: 統計資料

#### 3.1.4 查詢人事分析報表 (SYSGB30)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/personnel-analysis/query`
- **說明**: 查詢人事分析報表
- **請求格式**: 類似SYSGB10，但分析維度不同
- **回應格式**: 分析資料

#### 3.1.5 查詢人事彙總報表 (SYSGB40)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/personnel-summary/query`
- **說明**: 查詢人事彙總報表
- **請求格式**: 類似SYSGB10，但彙總維度不同
- **回應格式**: 彙總資料

### 3.2 後端實作類別

#### 3.2.1 Controller: `PersonnelReportController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/hr")]
    [Authorize]
    public class PersonnelReportController : ControllerBase
    {
        private readonly IPersonnelReportService _reportService;
        
        public PersonnelReportController(IPersonnelReportService reportService)
        {
            _reportService = reportService;
        }
        
        [HttpPost("salary-details/query")]
        public async Task<ActionResult<ApiResponse<PagedResult<SalaryDetailDto>>>> QuerySalaryDetails([FromBody] SalaryDetailQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpPost("salary-details/export")]
        public async Task<IActionResult> ExportSalaryDetails([FromBody] SalaryDetailQueryDto query)
        {
            // 實作匯出邏輯
        }
        
        [HttpPost("personnel-statistics/query")]
        public async Task<ActionResult<ApiResponse<PersonnelStatisticsDto>>> QueryPersonnelStatistics([FromBody] PersonnelStatisticsQueryDto query)
        {
            // 實作統計查詢邏輯
        }
        
        [HttpPost("personnel-analysis/query")]
        public async Task<ActionResult<ApiResponse<PersonnelAnalysisDto>>> QueryPersonnelAnalysis([FromBody] PersonnelAnalysisQueryDto query)
        {
            // 實作分析查詢邏輯
        }
        
        [HttpPost("personnel-summary/query")]
        public async Task<ActionResult<ApiResponse<PersonnelSummaryDto>>> QueryPersonnelSummary([FromBody] PersonnelSummaryQueryDto query)
        {
            // 實作彙總查詢邏輯
        }
    }
}
```

#### 3.2.2 Service: `PersonnelReportService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPersonnelReportService
    {
        Task<PagedResult<SalaryDetailDto>> QuerySalaryDetailsAsync(SalaryDetailQueryDto query);
        Task<byte[]> ExportSalaryDetailsAsync(SalaryDetailQueryDto query, string format);
        Task<PersonnelStatisticsDto> QueryPersonnelStatisticsAsync(PersonnelStatisticsQueryDto query);
        Task<PersonnelAnalysisDto> QueryPersonnelAnalysisAsync(PersonnelAnalysisQueryDto query);
        Task<PersonnelSummaryDto> QueryPersonnelSummaryAsync(PersonnelSummaryQueryDto query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 薪資明細查詢頁面 (`SalaryDetailQuery.vue`)
- **路徑**: `/hr/reports/salary-details`
- **功能**: 查詢薪資暨各類所得明細表
- **主要元件**:
  - 查詢表單 (SalaryDetailSearchForm)
  - 資料表格 (SalaryDetailDataTable)
  - 報表匯出功能

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SalaryDetailSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="證號別">
      <el-select v-model="searchForm.idCode" placeholder="請選擇證號別">
        <el-option v-for="item in idCodeList" :key="item.idCode" :label="item.content" :value="item.idCode" />
      </el-select>
    </el-form-item>
    <el-form-item label="身分證號/統一編號">
      <el-input v-model="searchForm.pid" placeholder="請輸入身分證號/統一編號" maxlength="10" />
    </el-form-item>
    <el-form-item label="所得給付年月" required>
      <el-date-picker
        v-model="searchForm.payYM"
        type="monthrange"
        range-separator="至"
        start-placeholder="開始月份"
        end-placeholder="結束月份"
        format="YYYYMM"
        value-format="YYYYMM"
      />
    </el-form-item>
    <el-form-item label="傳票編號">
      <el-input v-model="searchForm.voucherId" placeholder="請輸入傳票編號" maxlength="11" />
    </el-form-item>
    <el-form-item label="所得格式">
      <el-select v-model="searchForm.formCode" placeholder="請選擇所得格式">
        <el-option v-for="item in formCodeList" :key="item.formCode" :label="item.formCode" :value="item.formCode" />
      </el-select>
    </el-form-item>
    <el-form-item label="所得註記">
      <el-select v-model="searchForm.formNotes" placeholder="請選擇所得註記">
        <el-option v-for="item in formNotesList" :key="item.formNotes" :label="item.formNotes" :value="item.formNotes" />
      </el-select>
    </el-form-item>
    <el-form-item label="執行業別代號">
      <el-select v-model="searchForm.execCode" placeholder="請選擇執行業別代號">
        <el-option v-for="item in execCodeList" :key="item.execCode" :label="`${item.execCode}-${item.content}`" :value="item.execCode" />
      </el-select>
    </el-form-item>
    <el-form-item label="科目代號">
      <el-input v-model="searchForm.stypeId" placeholder="請輸入科目代號" maxlength="8" />
    </el-form-item>
    <el-form-item label="科目摘要">
      <el-input v-model="searchForm.notes" placeholder="請輸入科目摘要" />
    </el-form-item>
    <el-form-item label="給付淨額">
      <el-input-number v-model="searchForm.netAmtFrom" :precision="2" :min="0" placeholder="最小值" />
      <span> ~ </span>
      <el-input-number v-model="searchForm.netAmtTo" :precision="2" :min="0" placeholder="最大值" />
    </el-form-item>
    <el-form-item label="扣繳稅額">
      <el-input-number v-model="searchForm.txAmtFrom" :precision="2" :min="0" placeholder="最小值" />
      <span> ~ </span>
      <el-input-number v-model="searchForm.txAmtTo" :precision="2" :min="0" placeholder="最大值" />
    </el-form-item>
    <el-form-item label="審核狀態">
      <el-select v-model="searchForm.checkStatus" placeholder="請選擇審核狀態">
        <el-option label="全部" value="" />
        <el-option label="已審核" value="Y" />
        <el-option label="未審核" value="N" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleExport">匯出Excel</el-button>
      <el-button type="warning" @click="handlePrint">列印</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`SalaryDetailDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="salaryDetailList" v-loading="loading" border>
      <el-table-column prop="revenueSN" label="所得人代號" width="120" />
      <el-table-column prop="idCodeName" label="證號別" width="100" />
      <el-table-column prop="pid" label="身分證號/統一編號" width="150" />
      <el-table-column prop="payYM" label="所得給付年月" width="120" />
      <el-table-column prop="voucherId" label="傳票編號" width="120" />
      <el-table-column prop="formCode" label="所得格式" width="100" />
      <el-table-column prop="formNotes" label="所得註記" width="100" />
      <el-table-column prop="execCodeName" label="執行業別" width="150" />
      <el-table-column prop="stypeId" label="科目代號" width="120" />
      <el-table-column prop="notes" label="科目摘要" width="200" show-overflow-tooltip />
      <el-table-column prop="netAmt" label="給付淨額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.netAmt) }}
        </template>
      </el-table-column>
      <el-table-column prop="txAmt" label="扣繳稅額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.txAmt) }}
        </template>
      </el-table-column>
      <el-table-column prop="grossAmt" label="給付總額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.grossAmt) }}
        </template>
      </el-table-column>
      <el-table-column prop="taxRate" label="稅率(%)" width="100" align="right">
        <template #default="{ row }">
          {{ row.taxRate }}%
        </template>
      </el-table-column>
      <el-table-column prop="checkStatus" label="審核狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="row.checkStatus === 'Y' ? 'success' : 'warning'">
            {{ row.checkStatus === 'Y' ? '已審核' : '未審核' }}
          </el-tag>
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

### 4.3 API 呼叫 (`personnelReport.api.ts`)
```typescript
import request from '@/utils/request';

export interface SalaryDetailDto {
  salaryDetailId: string;
  revenueSN?: string;
  idCode?: string;
  idCodeName?: string;
  pid?: string;
  payYM: string;
  voucherId?: string;
  formCode?: string;
  formNotes?: string;
  execCode?: string;
  execCodeName?: string;
  stypeId?: string;
  notes?: string;
  netAmt?: number;
  txAmt?: number;
  grossAmt?: number;
  taxRate?: number;
  checkStatus?: string;
}

export interface SalaryDetailQueryDto {
  pageIndex: number;
  pageSize: number;
  filters?: {
    idCode?: string;
    pid?: string;
    payYMBegin?: string;
    payYMEnd?: string;
    voucherId?: string;
    formCode?: string;
    formNotes?: string;
    execCode?: string;
    stypeId?: string;
    notes?: string;
    netAmtFrom?: number;
    netAmtTo?: number;
    txAmtFrom?: number;
    txAmtTo?: number;
    checkStatus?: string;
  };
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
}

// API 函數
export const querySalaryDetails = (query: SalaryDetailQueryDto) => {
  return request.post<ApiResponse<PagedResult<SalaryDetailDto>>>('/api/v1/hr/salary-details/query', query);
};

export const exportSalaryDetails = (query: SalaryDetailQueryDto, format: 'excel' | 'pdf' = 'excel') => {
  return request.post(`/api/v1/hr/salary-details/export?format=${format}`, query, {
    responseType: 'blob'
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表匯出功能 (Excel/PDF)
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 報表匯出功能
- [ ] 報表列印功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 報表格式驗證

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 15天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 報表資料必須依使用者權限篩選

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 報表匯出必須使用非同步處理

### 6.3 資料驗證
- 日期格式必須驗證 (YYYYMM)
- 金額範圍必須驗證
- 必填欄位必須驗證

### 6.4 業務邏輯
- 所得給付年月為必填
- 報表資料必須依審核狀態篩選
- 匯出報表必須包含查詢條件說明

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢薪資明細成功
- [ ] 查詢條件驗證
- [ ] 分頁功能測試
- [ ] 排序功能測試

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 報表匯出測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 報表匯出效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB10_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB10_PR.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB20_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB20_PR.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB30_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB30_PR.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB40_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSGB40_PR.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


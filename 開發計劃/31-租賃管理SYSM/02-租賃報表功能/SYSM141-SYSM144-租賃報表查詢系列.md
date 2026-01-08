# SYSM141-SYSM144 - 租賃報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSM141-SYSM144 系列
- **功能名稱**: 租賃報表查詢系列
- **功能描述**: 提供租賃報表查詢功能，包含租賃明細報表、租賃統計報表、租賃分析報表等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM141_*.ASP` (租賃報表查詢相關)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM142_*.ASP` (租賃統計報表相關)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM143_*.ASP` (租賃分析報表相關)
  - `WEB/IMS_CORE/ASP/SYSM000/SYSM144_*.ASP` (租賃擴展報表相關)

### 1.2 業務需求
- 支援租賃明細報表查詢
- 支援租賃統計報表查詢（按日期、租戶、分店等）
- 支援租賃分析報表查詢
- 支援租金收入報表查詢
- 支援停車費收入報表查詢
- 支援報表匯出功能（Excel、PDF）
- 支援報表列印功能

---

## 二、資料庫設計 (Schema)

### 2.1 相關資料表
本功能主要使用 `Leases`、`LeaseDetails`、`LeasePayments`、`ParkingSpaces`、`LeaseContracts` 資料表，參考「SYSM111-SYSM138-租賃資料維護系列」的資料庫設計。

### 2.2 報表快取表: `LeaseReportCache` (租賃報表快取)

```sql
CREATE TABLE [dbo].[LeaseReportCache] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型 (REPORT_TYPE)
    [ReportParams] NVARCHAR(MAX) NULL, -- 報表參數（JSON格式） (REPORT_PARAMS)
    [ReportData] NVARCHAR(MAX) NULL, -- 報表資料（JSON格式） (REPORT_DATA)
    [CacheExpireTime] DATETIME2 NOT NULL, -- 快取過期時間 (CACHE_EXPIRE_TIME)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [UQ_LeaseReportCache_Type_Params] UNIQUE ([ReportType], [ReportParams])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseReportCache_ReportType] ON [dbo].[LeaseReportCache] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_LeaseReportCache_CacheExpireTime] ON [dbo].[LeaseReportCache] ([CacheExpireTime]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 租賃明細報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/reports/detail`
- **說明**: 查詢租賃明細報表
- **請求格式**:
  ```json
  {
    "leaseDateFrom": "2024-01-01",
    "leaseDateTo": "2024-01-31",
    "startDateFrom": "",
    "startDateTo": "",
    "shopId": "",
    "tenantId": "",
    "status": "",
    "pageIndex": 1,
    "pageSize": 20
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
          "leaseId": "L001",
          "tenantName": "租戶名稱",
          "shopName": "分店名稱",
          "leaseDate": "2024-01-01",
          "startDate": "2024-01-01",
          "endDate": "2024-12-31",
          "status": "E",
          "monthlyRent": 50000,
          "totalRent": 600000,
          "deposit": 100000,
          "parkingFee": 2000
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

#### 3.1.2 租賃統計報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/reports/statistics`
- **說明**: 查詢租賃統計報表（按日期、租戶、分店等維度統計）
- **請求格式**:
  ```json
  {
    "reportType": "BY_DATE", // BY_DATE: 按日期, BY_TENANT: 按租戶, BY_SHOP: 按分店
    "dateFrom": "2024-01-01",
    "dateTo": "2024-01-31",
    "shopId": "",
    "tenantId": "",
    "status": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "summary": {
        "totalLeases": 100,
        "totalMonthlyRent": 5000000,
        "totalDeposit": 10000000,
        "totalParkingFee": 200000
      },
      "details": [
        {
          "groupKey": "2024-01-01",
          "groupName": "2024-01-01",
          "leaseCount": 10,
          "totalMonthlyRent": 500000,
          "totalDeposit": 1000000,
          "totalParkingFee": 20000
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 租金收入報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/reports/rent-income`
- **說明**: 查詢租金收入報表
- **請求格式**:
  ```json
  {
    "dateFrom": "2024-01-01",
    "dateTo": "2024-01-31",
    "shopId": "",
    "tenantId": "",
    "groupBy": "DATE" // DATE: 按日期, SHOP: 按分店, TENANT: 按租戶
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "summary": {
        "totalRentIncome": 5000000,
        "totalDeposit": 10000000,
        "totalParkingFee": 200000
      },
      "details": [
        {
          "groupKey": "2024-01-01",
          "groupName": "2024-01-01",
          "rentIncome": 500000,
          "deposit": 1000000,
          "parkingFee": 20000
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 停車費收入報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/reports/parking-income`
- **說明**: 查詢停車費收入報表
- **請求格式**:
  ```json
  {
    "dateFrom": "2024-01-01",
    "dateTo": "2024-01-31",
    "shopId": "",
    "groupBy": "DATE" // DATE: 按日期, SHOP: 按分店
  }
  ```
- **回應格式**: 參考租金收入報表

#### 3.1.5 租賃分析報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/reports/analysis`
- **說明**: 查詢租賃分析報表
- **請求格式**:
  ```json
  {
    "dateFrom": "2024-01-01",
    "dateTo": "2024-01-31",
    "analysisType": "TENANT_ANALYSIS", // TENANT_ANALYSIS: 租戶分析, SHOP_ANALYSIS: 分店分析, AREA_ANALYSIS: 區域分析
    "shopId": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "analysisData": [
        {
          "key": "T001",
          "name": "租戶名稱",
          "leaseCount": 10,
          "totalRent": 5000000,
          "avgRent": 500000,
          "occupancyRate": 0.85
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 報表匯出
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/reports/export`
- **說明**: 匯出報表（Excel、PDF格式）
- **請求格式**:
  ```json
  {
    "reportType": "DETAIL", // DETAIL: 明細報表, STATISTICS: 統計報表, RENT_INCOME: 租金收入, PARKING_INCOME: 停車費收入, ANALYSIS: 分析報表
    "exportFormat": "EXCEL", // EXCEL, PDF
    "reportParams": {
      "dateFrom": "2024-01-01",
      "dateTo": "2024-01-31",
      "shopId": "",
      "tenantId": ""
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "匯出成功",
    "data": {
      "fileUrl": "/api/v1/files/download/xxx.xlsx",
      "fileName": "租賃報表_20240101.xlsx"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `LeaseReportsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/leases/reports")]
    [Authorize]
    public class LeaseReportsController : ControllerBase
    {
        private readonly ILeaseReportService _leaseReportService;
        
        public LeaseReportsController(ILeaseReportService leaseReportService)
        {
            _leaseReportService = leaseReportService;
        }
        
        [HttpPost("detail")]
        public async Task<ActionResult<ApiResponse<PagedResult<LeaseReportDetailDto>>>> GetDetailReport([FromBody] LeaseDetailReportQueryDto query)
        {
            // 實作明細報表查詢邏輯
        }
        
        [HttpPost("statistics")]
        public async Task<ActionResult<ApiResponse<LeaseStatisticsReportDto>>> GetStatisticsReport([FromBody] LeaseStatisticsReportQueryDto query)
        {
            // 實作統計報表查詢邏輯
        }
        
        [HttpPost("rent-income")]
        public async Task<ActionResult<ApiResponse<LeaseRentIncomeReportDto>>> GetRentIncomeReport([FromBody] LeaseRentIncomeReportQueryDto query)
        {
            // 實作租金收入報表查詢邏輯
        }
        
        [HttpPost("parking-income")]
        public async Task<ActionResult<ApiResponse<LeaseParkingIncomeReportDto>>> GetParkingIncomeReport([FromBody] LeaseParkingIncomeReportQueryDto query)
        {
            // 實作停車費收入報表查詢邏輯
        }
        
        [HttpPost("analysis")]
        public async Task<ActionResult<ApiResponse<LeaseAnalysisReportDto>>> GetAnalysisReport([FromBody] LeaseAnalysisReportQueryDto query)
        {
            // 實作分析報表查詢邏輯
        }
        
        [HttpPost("export")]
        public async Task<ActionResult<ApiResponse<ExportFileDto>>> ExportReport([FromBody] LeaseReportExportDto dto)
        {
            // 實作報表匯出邏輯
        }
    }
}
```

#### 3.2.2 Service: `LeaseReportService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ILeaseReportService
    {
        Task<PagedResult<LeaseReportDetailDto>> GetDetailReportAsync(LeaseDetailReportQueryDto query);
        Task<LeaseStatisticsReportDto> GetStatisticsReportAsync(LeaseStatisticsReportQueryDto query);
        Task<LeaseRentIncomeReportDto> GetRentIncomeReportAsync(LeaseRentIncomeReportQueryDto query);
        Task<LeaseParkingIncomeReportDto> GetParkingIncomeReportAsync(LeaseParkingIncomeReportQueryDto query);
        Task<LeaseAnalysisReportDto> GetAnalysisReportAsync(LeaseAnalysisReportQueryDto query);
        Task<ExportFileDto> ExportReportAsync(LeaseReportExportDto dto);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 租賃報表查詢頁面 (`LeaseReport.vue`)
- **路徑**: `/lease/reports`
- **功能**: 提供租賃報表查詢功能
- **主要元件**:
  - 報表類型選擇 (ReportTypeSelector)
  - 查詢條件表單 (ReportQueryForm)
  - 報表結果顯示 (ReportResultDisplay)
  - 報表匯出按鈕
  - 報表列印按鈕

### 4.2 UI 元件設計

#### 4.2.1 報表類型選擇元件 (`ReportTypeSelector.vue`)
```vue
<template>
  <el-tabs v-model="activeTab" @tab-change="handleTabChange">
    <el-tab-pane label="明細報表" name="detail">
      <LeaseDetailReport />
    </el-tab-pane>
    <el-tab-pane label="統計報表" name="statistics">
      <LeaseStatisticsReport />
    </el-tab-pane>
    <el-tab-pane label="租金收入" name="rent-income">
      <LeaseRentIncomeReport />
    </el-tab-pane>
    <el-tab-pane label="停車費收入" name="parking-income">
      <LeaseParkingIncomeReport />
    </el-tab-pane>
    <el-tab-pane label="分析報表" name="analysis">
      <LeaseAnalysisReport />
    </el-tab-pane>
  </el-tabs>
</template>
```

#### 4.2.2 查詢條件表單元件 (`ReportQueryForm.vue`)
```vue
<template>
  <el-form :model="queryForm" inline>
    <el-form-item label="報表類型">
      <el-select v-model="queryForm.reportType" placeholder="請選擇報表類型">
        <el-option label="明細報表" value="DETAIL" />
        <el-option label="統計報表" value="STATISTICS" />
        <el-option label="租金收入" value="RENT_INCOME" />
        <el-option label="停車費收入" value="PARKING_INCOME" />
        <el-option label="分析報表" value="ANALYSIS" />
      </el-select>
    </el-form-item>
    <el-form-item label="租賃日期">
      <el-date-picker
        v-model="queryForm.dateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
      />
    </el-form-item>
    <el-form-item label="分店">
      <el-select v-model="queryForm.shopId" placeholder="請選擇分店" clearable>
        <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
      </el-select>
    </el-form-item>
    <el-form-item label="租戶">
      <el-select v-model="queryForm.tenantId" placeholder="請選擇租戶" clearable filterable>
        <el-option v-for="tenant in tenantList" :key="tenant.tenantId" :label="tenant.tenantName" :value="tenant.tenantId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="queryForm.status" placeholder="請選擇狀態" clearable>
        <el-option label="草稿" value="D" />
        <el-option label="已簽約" value="S" />
        <el-option label="已生效" value="E" />
        <el-option label="已終止" value="T" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleQuery">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleExport">匯出</el-button>
      <el-button type="info" @click="handlePrint">列印</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.3 報表結果顯示元件 (`ReportResultDisplay.vue`)
```vue
<template>
  <div>
    <el-table :data="reportData" v-loading="loading" border>
      <el-table-column prop="leaseId" label="租賃編號" width="120" />
      <el-table-column prop="tenantName" label="租戶名稱" width="150" />
      <el-table-column prop="shopName" label="分店" width="120" />
      <el-table-column prop="leaseDate" label="租賃日期" width="120" />
      <el-table-column prop="startDate" label="租期開始" width="120" />
      <el-table-column prop="endDate" label="租期結束" width="120" />
      <el-table-column prop="monthlyRent" label="月租金" width="100" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.monthlyRent) }}
        </template>
      </el-table-column>
      <el-table-column prop="totalRent" label="總租金" width="100" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.totalRent) }}
        </template>
      </el-table-column>
      <el-table-column prop="deposit" label="押金" width="100" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.deposit) }}
        </template>
      </el-table-column>
      <el-table-column prop="parkingFee" label="停車費" width="100" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.parkingFee) }}
        </template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-if="pagination"
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

### 4.3 API 呼叫 (`leaseReport.api.ts`)
```typescript
import request from '@/utils/request';

export interface LeaseDetailReportQueryDto {
  leaseDateFrom?: string;
  leaseDateTo?: string;
  startDateFrom?: string;
  startDateTo?: string;
  shopId?: string;
  tenantId?: string;
  status?: string;
  pageIndex: number;
  pageSize: number;
}

export interface LeaseStatisticsReportQueryDto {
  reportType: 'BY_DATE' | 'BY_TENANT' | 'BY_SHOP';
  dateFrom: string;
  dateTo: string;
  shopId?: string;
  tenantId?: string;
  status?: string;
}

export interface LeaseRentIncomeReportQueryDto {
  dateFrom: string;
  dateTo: string;
  shopId?: string;
  tenantId?: string;
  groupBy: 'DATE' | 'SHOP' | 'TENANT';
}

export interface LeaseParkingIncomeReportQueryDto {
  dateFrom: string;
  dateTo: string;
  shopId?: string;
  groupBy: 'DATE' | 'SHOP';
}

export interface LeaseAnalysisReportQueryDto {
  dateFrom: string;
  dateTo: string;
  analysisType: 'TENANT_ANALYSIS' | 'SHOP_ANALYSIS' | 'AREA_ANALYSIS';
  shopId?: string;
}

export interface LeaseReportExportDto {
  reportType: 'DETAIL' | 'STATISTICS' | 'RENT_INCOME' | 'PARKING_INCOME' | 'ANALYSIS';
  exportFormat: 'EXCEL' | 'PDF';
  reportParams: Record<string, any>;
}

// API 函數
export const getLeaseDetailReport = (query: LeaseDetailReportQueryDto) => {
  return request.post<ApiResponse<PagedResult<LeaseReportDetailDto>>>('/api/v1/leases/reports/detail', query);
};

export const getLeaseStatisticsReport = (query: LeaseStatisticsReportQueryDto) => {
  return request.post<ApiResponse<LeaseStatisticsReportDto>>('/api/v1/leases/reports/statistics', query);
};

export const getLeaseRentIncomeReport = (query: LeaseRentIncomeReportQueryDto) => {
  return request.post<ApiResponse<LeaseRentIncomeReportDto>>('/api/v1/leases/reports/rent-income', query);
};

export const getLeaseParkingIncomeReport = (query: LeaseParkingIncomeReportQueryDto) => {
  return request.post<ApiResponse<LeaseParkingIncomeReportDto>>('/api/v1/leases/reports/parking-income', query);
};

export const getLeaseAnalysisReport = (query: LeaseAnalysisReportQueryDto) => {
  return request.post<ApiResponse<LeaseAnalysisReportDto>>('/api/v1/leases/reports/analysis', query);
};

export const exportLeaseReport = (data: LeaseReportExportDto) => {
  return request.post<ApiResponse<ExportFileDto>>('/api/v1/leases/reports/export', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (3天)
- [ ] 報表查詢邏輯實作
- [ ] 報表統計邏輯實作
- [ ] 報表分析邏輯實作
- [ ] 租金收入報表邏輯實作
- [ ] 停車費收入報表邏輯實作
- [ ] 報表匯出邏輯實作
- [ ] 報表快取機制實作
- [ ] 單元測試

### 5.2 階段二: 前端開發 (2天)
- [ ] 報表查詢頁面開發
- [ ] 報表類型選擇元件開發
- [ ] 查詢條件表單開發
- [ ] 報表結果顯示開發
- [ ] 報表匯出功能開發
- [ ] 報表列印功能開發
- [ ] 元件測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 6天

---

## 六、注意事項

### 6.1 效能
- 大量資料查詢必須使用分頁
- 必須使用報表快取機制
- 統計報表必須使用資料庫聚合函數
- 複雜查詢必須優化SQL語句

### 6.2 資料驗證
- 查詢條件必須驗證日期範圍
- 報表參數必須驗證
- 匯出格式必須驗證

### 6.3 業務邏輯
- 統計報表必須正確計算各項金額
- 分析報表必須正確計算各項指標
- 報表快取必須設定合理的過期時間

---

## 七、測試案例

### 7.1 單元測試
- [ ] 明細報表查詢成功
- [ ] 統計報表查詢成功
- [ ] 租金收入報表查詢成功
- [ ] 停車費收入報表查詢成功
- [ ] 分析報表查詢成功
- [ ] 報表匯出成功（Excel）
- [ ] 報表匯出成功（PDF）

### 7.2 整合測試
- [ ] 完整報表查詢流程測試
- [ ] 報表快取測試
- [ ] 報表匯出功能測試
- [ ] 報表列印功能測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 報表快取效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSM000/SYSM141_*.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM142_*.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM143_*.ASP`
- `WEB/IMS_CORE/ASP/SYSM000/SYSM144_*.ASP`

### 8.2 相關開發計劃
- `開發計劃/31-租賃管理SYSM/01-租賃基礎功能/SYSM111-SYSM138-租賃資料維護系列.md`
- `開發計劃/35-銷售管理/03-銷售報表功能/SYSD310-SYSD430-銷售報表查詢系列.md`
- `開發計劃/36-憑證管理/03-憑證報表功能/SYSK310-SYSK500-憑證報表查詢系列.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


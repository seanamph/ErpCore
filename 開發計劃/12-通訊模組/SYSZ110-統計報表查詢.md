# SYSZ110 - 統計報表查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSZ110
- **功能名稱**: 流量統計報表查詢
- **功能描述**: 提供系統流量統計報表功能，支援年度、月份、日期、小時等不同時間維度的流量統計，統計訪客數量，並支援報表列印和匯出
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_Y.ASP` (年度統計報表)
  - `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_M.asp` (月份統計報表)
  - `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_D.asp` (日期統計報表)
  - `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_H.asp` (小時統計報表)

### 1.2 業務需求
- 年度流量統計報表（按年度統計訪客數）
- 月份流量統計報表（按月份統計訪客數，可點擊查看日期明細）
- 日期流量統計報表（按日期統計訪客數，可點擊查看小時明細）
- 小時流量統計報表（按小時統計訪客數）
- 支援多種統計模式切換（年度分析、月份分析、日期分析）
- 支援報表列印功能
- 支援報表匯出功能（Excel/PDF）
- 統計結果支援分頁顯示

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TrafficStatistics` (對應舊系統 `XCOM_TRAFFIC`)

```sql
CREATE TABLE [dbo].[TrafficStatistics] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VisitDate] DATETIME2 NOT NULL, -- 訪問日期 (VISIT_DATE)
    [PeriodId] INT NOT NULL, -- 時段ID (PERIOD_ID, 1-12或1-24)
    [Visitor] INT NOT NULL DEFAULT 0, -- 訪客數 (VISITOR)
    [SystemId] NVARCHAR(50) NULL, -- 系統代碼
    [UserId] NVARCHAR(50) NULL, -- 使用者ID
    [IpAddress] NVARCHAR(50) NULL, -- IP位址
    [UserAgent] NVARCHAR(500) NULL, -- 使用者代理
    [PageUrl] NVARCHAR(500) NULL, -- 頁面URL
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_TrafficStatistics] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TrafficStatistics_VisitDate] ON [dbo].[TrafficStatistics] ([VisitDate]);
CREATE NONCLUSTERED INDEX [IX_TrafficStatistics_PeriodId] ON [dbo].[TrafficStatistics] ([PeriodId]);
CREATE NONCLUSTERED INDEX [IX_TrafficStatistics_VisitDate_PeriodId] ON [dbo].[TrafficStatistics] ([VisitDate], [PeriodId]);
CREATE NONCLUSTERED INDEX [IX_TrafficStatistics_SystemId] ON [dbo].[TrafficStatistics] ([SystemId]);
CREATE NONCLUSTERED INDEX [IX_TrafficStatistics_UserId] ON [dbo].[TrafficStatistics] ([UserId]);
```

### 2.2 相關資料表

#### 2.2.1 `TrafficStatisticsQueries` - 流量統計查詢記錄
```sql
CREATE TABLE [dbo].[TrafficStatisticsQueries] (
    [QueryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportType] NVARCHAR(20) NOT NULL, -- 報表類型 (YEAR, MONTH, DAY, HOUR)
    [Year] INT NULL, -- 年份
    [Month] INT NULL, -- 月份
    [Day] INT NULL, -- 日期
    [QueryParams] NVARCHAR(MAX) NULL, -- 查詢參數 (JSON格式)
    [ExportFormat] NVARCHAR(20) NULL, -- 匯出格式 (EXCEL, PDF)
    [QueriedBy] NVARCHAR(50) NULL, -- 查詢者
    [QueriedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 查詢時間
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TrafficStatisticsQueries] PRIMARY KEY CLUSTERED ([QueryId] ASC)
);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| VisitDate | DATETIME2 | - | NO | - | 訪問日期 | - |
| PeriodId | INT | - | NO | - | 時段ID | 1-12或1-24 |
| Visitor | INT | - | NO | 0 | 訪客數 | - |
| SystemId | NVARCHAR | 50 | YES | - | 系統代碼 | - |
| UserId | NVARCHAR | 50 | YES | - | 使用者ID | - |
| IpAddress | NVARCHAR | 50 | YES | - | IP位址 | - |
| UserAgent | NVARCHAR | 500 | YES | - | 使用者代理 | - |
| PageUrl | NVARCHAR | 500 | YES | - | 頁面URL | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢年度流量統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz110/statistics/year`
- **說明**: 查詢年度流量統計報表
- **請求參數**:
  - `year`: 年份 (選填，預設為當前年份)
- **回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "year": 2024,
    "totalVisitors": 10000,
    "statistics": [
      {
        "year": 2024,
        "visitors": 10000
      }
    ],
    "monthlyStatistics": [
      {
        "month": 1,
        "visitors": 800,
        "monthName": "一月"
      }
    ]
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

#### 3.1.2 查詢月份流量統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz110/statistics/month`
- **說明**: 查詢月份流量統計報表
- **請求參數**:
  - `year`: 年份 (必填)
  - `month`: 月份 (選填，預設為當前月份)
- **回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "year": 2024,
    "month": 1,
    "totalVisitors": 800,
    "dailyStatistics": [
      {
        "day": 1,
        "visitors": 30,
        "dayName": "01"
      }
    ]
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

#### 3.1.3 查詢日期流量統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz110/statistics/day`
- **說明**: 查詢日期流量統計報表
- **請求參數**:
  - `year`: 年份 (必填)
  - `month`: 月份 (必填)
  - `day`: 日期 (選填)
- **回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "year": 2024,
    "month": 1,
    "day": 1,
    "totalVisitors": 30,
    "hourlyStatistics": [
      {
        "periodId": 1,
        "visitors": 2,
        "periodName": "01"
      }
    ]
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

#### 3.1.4 查詢小時流量統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysz110/statistics/hour`
- **說明**: 查詢小時流量統計報表
- **請求參數**:
  - `year`: 年份 (必填)
  - `month`: 月份 (必填)
  - `day`: 日期 (必填)
- **回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "year": 2024,
    "month": 1,
    "day": 1,
    "totalVisitors": 30,
    "hourlyStatistics": [
      {
        "periodId": 1,
        "visitors": 2,
        "periodName": "01"
      }
    ]
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

#### 3.1.5 匯出流量統計報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sysz110/statistics/export`
- **說明**: 匯出流量統計報表 (Excel/PDF)
- **請求格式**:
```json
{
  "reportType": "YEAR",
  "year": 2024,
  "month": 1,
  "day": 1,
  "exportFormat": "EXCEL"
}
```
- **回應格式**: 返回檔案下載連結或檔案內容

### 3.2 後端實作類別

#### 3.2.1 Controller: `Sysz110Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/sysz110")]
    [Authorize]
    public class Sysz110Controller : ControllerBase
    {
        private readonly ISysz110Service _service;
        
        public Sysz110Controller(ISysz110Service service)
        {
            _service = service;
        }
        
        [HttpGet("statistics/year")]
        public async Task<ActionResult<ApiResponse<YearStatisticsDto>>> GetYearStatistics([FromQuery] int? year)
        {
            var result = await _service.GetYearStatisticsAsync(year ?? DateTime.Now.Year);
            return Ok(ApiResponse<YearStatisticsDto>.Success(result));
        }
        
        [HttpGet("statistics/month")]
        public async Task<ActionResult<ApiResponse<MonthStatisticsDto>>> GetMonthStatistics([FromQuery] int year, [FromQuery] int? month)
        {
            var result = await _service.GetMonthStatisticsAsync(year, month ?? DateTime.Now.Month);
            return Ok(ApiResponse<MonthStatisticsDto>.Success(result));
        }
        
        [HttpGet("statistics/day")]
        public async Task<ActionResult<ApiResponse<DayStatisticsDto>>> GetDayStatistics([FromQuery] int year, [FromQuery] int month, [FromQuery] int? day)
        {
            var result = await _service.GetDayStatisticsAsync(year, month, day);
            return Ok(ApiResponse<DayStatisticsDto>.Success(result));
        }
        
        [HttpGet("statistics/hour")]
        public async Task<ActionResult<ApiResponse<HourStatisticsDto>>> GetHourStatistics([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            var result = await _service.GetHourStatisticsAsync(year, month, day);
            return Ok(ApiResponse<HourStatisticsDto>.Success(result));
        }
        
        [HttpPost("statistics/export")]
        public async Task<ActionResult> ExportStatistics([FromBody] ExportStatisticsDto dto)
        {
            var fileContent = await _service.ExportStatisticsAsync(dto);
            return File(fileContent.Content, fileContent.ContentType, fileContent.FileName);
        }
    }
}
```

#### 3.2.2 Service: `Sysz110Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ISysz110Service
    {
        Task<YearStatisticsDto> GetYearStatisticsAsync(int year);
        Task<MonthStatisticsDto> GetMonthStatisticsAsync(int year, int month);
        Task<DayStatisticsDto> GetDayStatisticsAsync(int year, int month, int? day);
        Task<HourStatisticsDto> GetHourStatisticsAsync(int year, int month, int day);
        Task<FileContentDto> ExportStatisticsAsync(ExportStatisticsDto dto);
    }
}
```

#### 3.2.3 Repository: `Sysz110Repository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ISysz110Repository
    {
        Task<List<TrafficStatistics>> GetYearStatisticsAsync(int year);
        Task<List<TrafficStatistics>> GetMonthStatisticsAsync(int year, int month);
        Task<List<TrafficStatistics>> GetDayStatisticsAsync(int year, int month, int? day);
        Task<List<TrafficStatistics>> GetHourStatisticsAsync(int year, int month, int day);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 流量統計報表頁面 (`Sysz110Report.vue`)
- **路徑**: `/xcom/sysz110/report`
- **功能**: 顯示流量統計報表，支援年度、月份、日期、小時等不同時間維度的統計
- **主要元件**:
  - 查詢表單 (Sysz110QueryForm)
  - 統計報表顯示 (Sysz110StatisticsTable)
  - 匯出選項對話框

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`Sysz110QueryForm.vue`)
```vue
<template>
  <el-form :model="queryForm" :rules="rules" ref="formRef" inline>
    <el-form-item label="統計模式" prop="reportMode">
      <el-radio-group v-model="queryForm.reportMode" @change="handleModeChange">
        <el-radio label="YEAR">年度分析</el-radio>
        <el-radio label="MONTH">月份分析</el-radio>
        <el-radio label="DAY">日期分析</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item label="年份" prop="year" v-if="queryForm.reportMode !== 'YEAR'">
      <el-date-picker
        v-model="queryForm.year"
        type="year"
        placeholder="請選擇年份"
        format="YYYY"
        value-format="YYYY"
      />
    </el-form-item>
    <el-form-item label="月份" prop="month" v-if="queryForm.reportMode === 'DAY'">
      <el-date-picker
        v-model="queryForm.month"
        type="month"
        placeholder="請選擇月份"
        format="YYYY-MM"
        value-format="YYYY-MM"
      />
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

#### 4.2.2 統計報表顯示元件 (`Sysz110StatisticsTable.vue`)
```vue
<template>
  <div>
    <el-table :data="statisticsList" v-loading="loading" border>
      <el-table-column 
        v-if="reportType === 'YEAR'"
        prop="year" 
        label="統計年度" 
        width="120" 
        align="center"
      />
      <el-table-column 
        v-if="reportType === 'MONTH'"
        prop="month" 
        label="統計月份" 
        width="120" 
        align="center"
      />
      <el-table-column 
        v-if="reportType === 'DAY'"
        prop="day" 
        label="統計日期" 
        width="120" 
        align="center"
      />
      <el-table-column 
        v-if="reportType === 'HOUR'"
        prop="periodId" 
        label="統計小時" 
        width="120" 
        align="center"
      />
      <el-table-column 
        v-for="(col, index) in dynamicColumns" 
        :key="index"
        :prop="col.prop" 
        :label="col.label" 
        width="100" 
        align="right"
      >
        <template #default="{ row }">
          <a 
            v-if="col.clickable" 
            href="javascript:void(0)" 
            @click="handleColumnClick(row, col)"
            style="color: blue; text-decoration: underline;"
          >
            {{ row[col.prop] || 0 }}
          </a>
          <span v-else>{{ row[col.prop] || 0 }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="total" label="總計" width="100" align="right" />
    </el-table>
    <div class="summary-info">
      <el-tag type="info" size="large">
        年份: {{ summary.year }}<span v-if="summary.month">，月份: {{ summary.month }}</span><span v-if="summary.day">，日期: {{ summary.day }}</span>
      </el-tag>
      <el-tag type="success" size="large" style="margin-left: 10px;">
        共 {{ summary.totalVisitors }} 人
      </el-tag>
    </div>
  </div>
</template>
```

### 4.3 API 呼叫 (`sysz110.api.ts`)
```typescript
import request from '@/utils/request';

export interface YearStatisticsDto {
  year: number;
  totalVisitors: number;
  statistics: Array<{
    year: number;
    visitors: number;
  }>;
  monthlyStatistics: Array<{
    month: number;
    visitors: number;
    monthName: string;
  }>;
}

export interface MonthStatisticsDto {
  year: number;
  month: number;
  totalVisitors: number;
  dailyStatistics: Array<{
    day: number;
    visitors: number;
    dayName: string;
  }>;
}

export interface DayStatisticsDto {
  year: number;
  month: number;
  day: number;
  totalVisitors: number;
  hourlyStatistics: Array<{
    periodId: number;
    visitors: number;
    periodName: string;
  }>;
}

export interface HourStatisticsDto {
  year: number;
  month: number;
  day: number;
  totalVisitors: number;
  hourlyStatistics: Array<{
    periodId: number;
    visitors: number;
    periodName: string;
  }>;
}

export interface ExportStatisticsDto {
  reportType: 'YEAR' | 'MONTH' | 'DAY' | 'HOUR';
  year: number;
  month?: number;
  day?: number;
  exportFormat: 'EXCEL' | 'PDF';
}

// API 函數
export const getYearStatistics = (year?: number) => {
  return request.get<ApiResponse<YearStatisticsDto>>('/api/v1/sysz110/statistics/year', {
    params: { year }
  });
};

export const getMonthStatistics = (year: number, month?: number) => {
  return request.get<ApiResponse<MonthStatisticsDto>>('/api/v1/sysz110/statistics/month', {
    params: { year, month }
  });
};

export const getDayStatistics = (year: number, month: number, day?: number) => {
  return request.get<ApiResponse<DayStatisticsDto>>('/api/v1/sysz110/statistics/day', {
    params: { year, month, day }
  });
};

export const getHourStatistics = (year: number, month: number, day: number) => {
  return request.get<ApiResponse<HourStatisticsDto>>('/api/v1/sysz110/statistics/hour', {
    params: { year, month, day }
  });
};

export const exportStatistics = (dto: ExportStatisticsDto) => {
  return request.post('/api/v1/sysz110/statistics/export', dto, { responseType: 'blob' });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立查詢記錄表
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] Excel/PDF匯出功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢表單開發
- [ ] 統計報表顯示開發
- [ ] 動態欄位生成邏輯
- [ ] 匯出功能整合
- [ ] 列印功能整合
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] Excel/PDF匯出測試
- [ ] 列印功能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 業務邏輯
- 年度統計：統計所有年份的訪客數，可點擊年份查看月份明細
- 月份統計：統計指定年份各月份的訪客數，可點擊月份查看日期明細
- 日期統計：統計指定月份各日期的訪客數，可點擊日期查看小時明細
- 小時統計：統計指定日期各時段的訪客數
- 時段ID範圍：1-12或1-24，需根據系統設定決定

### 6.2 資料驗證
- 年份必須在合理範圍內（2000-當前年份）
- 月份必須在1-12之間
- 日期必須在該月份的有效日期範圍內
- 時段ID必須在有效範圍內

### 6.3 效能
- 大量資料統計必須使用聚合查詢
- 必須建立適當的索引
- Excel/PDF匯出功能必須使用非同步處理

### 6.4 UI/UX
- 支援點擊統計項目查看明細（年度→月份→日期→小時）
- 支援多種統計模式切換
- 報表顯示必須清晰易讀
- 支援響應式設計

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢年度流量統計成功
- [ ] 查詢月份流量統計成功
- [ ] 查詢日期流量統計成功
- [ ] 查詢小時流量統計成功
- [ ] Excel匯出功能測試
- [ ] PDF匯出功能測試

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] Excel/PDF匯出整合測試
- [ ] 列印功能測試

### 7.3 效能測試
- [ ] 大量資料統計測試
- [ ] 並發查詢測試
- [ ] Excel/PDF匯出效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_Y.ASP` - 年度統計報表
- `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_M.asp` - 月份統計報表
- `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_D.asp` - 日期統計報表
- `WEB/IMS_CORE/ASP/XCOM000/SYSZ110_PR_H.asp` - 小時統計報表

### 8.2 資料庫 Schema
- 舊系統資料表：`XCOM_TRAFFIC`
- 主要欄位：VISIT_DATE, PERIOD_ID, VISITOR
- 主要功能：流量統計報表，支援年度、月份、日期、小時等不同時間維度的統計

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


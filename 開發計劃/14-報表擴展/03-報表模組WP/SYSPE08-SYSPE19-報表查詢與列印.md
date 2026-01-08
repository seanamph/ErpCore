# SYSPE08-SYSPE19 - 報表查詢與列印 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSPE08-SYSPE19
- **功能名稱**: 報表模組WP-報表查詢與列印
- **功能描述**: 提供報表模組WP的報表查詢與列印功能，包含SYSPE08到SYSPE19共12個報表，支援報表查詢、資料篩選、報表列印、報表匯出等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSWP00/SYSPE08_PR.rdlc` - SYSPE08 報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE08_ERR_PR.rdlc` - SYSPE08 錯誤報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE09_PR.rdlc` - SYSPE09 報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE09_M_PR.rdlc` - SYSPE09 月報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE14.rdlc` - SYSPE14 報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE14_M.rdlc` - SYSPE14 月報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE15.rdlc` - SYSPE15 報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE15_M.rdlc` - SYSPE15 月報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE16.rdlc` - SYSPE16 報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE17.rdlc` - SYSPE17 報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE17_M.rdlc` - SYSPE17 月報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE18.rdlc` - SYSPE18 報表定義
  - `WEB/IMS_CORE/SYSWP00/SYSPE18_PR.aspx` - SYSPE18 報表列印頁面
  - `WEB/IMS_CORE/SYSWP00/SYSPE19_PR.rdlc` - SYSPE19 報表定義

### 1.2 業務需求
- 提供多種報表查詢功能（SYSPE08-SYSPE19）
- 支援條件查詢與資料篩選
- 支援報表資料預覽
- 支援報表列印（PDF格式）
- 支援報表匯出（Excel、PDF）
- 支援月報表查詢（部分報表）
- 支援報表錯誤記錄查詢（SYSPE08_ERR）
- 記錄報表查詢與列印記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportWPQueries` (報表WP查詢設定)

```sql
CREATE TABLE [dbo].[ReportWPQueries] (
    [QueryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (SYSPE08-SYSPE19)
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [ReportType] NVARCHAR(20) NOT NULL DEFAULT 'NORMAL', -- 報表類型 (NORMAL, MONTHLY, ERROR)
    [QueryParams] NVARCHAR(MAX) NULL, -- 查詢參數 (JSON格式)
    [QuerySql] NVARCHAR(MAX) NULL, -- 查詢SQL
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ReportWPQueries] PRIMARY KEY CLUSTERED ([QueryId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportWPQueries_ReportCode] ON [dbo].[ReportWPQueries] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_ReportWPQueries_ReportType] ON [dbo].[ReportWPQueries] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_ReportWPQueries_Status] ON [dbo].[ReportWPQueries] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `ReportWPQueryLogs` - 報表WP查詢記錄
```sql
CREATE TABLE [dbo].[ReportWPQueryLogs] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [QueryId] BIGINT NULL,
    [ReportCode] NVARCHAR(50) NOT NULL,
    [UserId] NVARCHAR(50) NOT NULL,
    [QueryParams] NVARCHAR(MAX) NULL,
    [QueryTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [ExecutionTime] INT NULL, -- 執行時間(毫秒)
    [RecordCount] INT NULL, -- 記錄數
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    CONSTRAINT [FK_ReportWPQueryLogs_ReportWPQueries] FOREIGN KEY ([QueryId]) REFERENCES [dbo].[ReportWPQueries] ([QueryId]),
    CONSTRAINT [FK_ReportWPQueryLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportWPQueryLogs_ReportCode] ON [dbo].[ReportWPQueryLogs] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_ReportWPQueryLogs_UserId] ON [dbo].[ReportWPQueryLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_ReportWPQueryLogs_QueryTime] ON [dbo].[ReportWPQueryLogs] ([QueryTime]);
```

#### 2.2.2 `ReportWPPrints` - 報表WP列印記錄
```sql
CREATE TABLE [dbo].[ReportWPPrints] (
    [PrintId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportCode] NVARCHAR(50) NOT NULL,
    [ReportName] NVARCHAR(200) NOT NULL,
    [ReportType] NVARCHAR(20) NOT NULL DEFAULT 'NORMAL',
    [PrintParams] NVARCHAR(MAX) NULL,
    [PrintFormat] NVARCHAR(20) NOT NULL DEFAULT 'PDF',
    [PrintStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING',
    [FileUrl] NVARCHAR(500) NULL,
    [FileSize] BIGINT NULL,
    [PageCount] INT NULL,
    [PrintedBy] NVARCHAR(50) NULL,
    [PrintedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ReportWPPrints] PRIMARY KEY CLUSTERED ([PrintId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportWPPrints_ReportCode] ON [dbo].[ReportWPPrints] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_ReportWPPrints_PrintedBy] ON [dbo].[ReportWPPrints] ([PrintedBy]);
CREATE NONCLUSTERED INDEX [IX_ReportWPPrints_PrintedAt] ON [dbo].[ReportWPPrints] ([PrintedAt]);
CREATE NONCLUSTERED INDEX [IX_ReportWPPrints_PrintStatus] ON [dbo].[ReportWPPrints] ([PrintStatus]);
```

### 2.3 資料字典

#### ReportWPQueries 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| QueryId | BIGINT | - | NO | IDENTITY(1,1) | 查詢ID | 主鍵 |
| ReportCode | NVARCHAR | 50 | NO | - | 報表代碼 | SYSPE08-SYSPE19 |
| ReportName | NVARCHAR | 200 | NO | - | 報表名稱 | - |
| ReportType | NVARCHAR | 20 | NO | 'NORMAL' | 報表類型 | NORMAL, MONTHLY, ERROR |
| QueryParams | NVARCHAR(MAX) | - | YES | - | 查詢參數 | JSON格式 |
| QuerySql | NVARCHAR(MAX) | - | YES | - | 查詢SQL | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### SYSPE08 報表資料結構（參考XSD）
- SALES_DATE: 銷售日期
- SALES_TIME: 銷售時間
- VENDER_ID: 廠商代號
- KIND: 種類
- KIND_NAME: 種類名稱
- POS_ID: POS機代號
- VENDER_NAME: 廠商名稱
- AMOUNT: 金額
- CARD_NO: 卡號
- COMPANYNAME: 公司名稱
- ORG_NAME: 組織名稱
- COST_CENTER: 成本中心
- EMP_ID: 員工代號
- EMP_NAME: 員工姓名
- EMP_LV: 員工等級
- RETURN_MSG: 回傳訊息
- OFFLINE: 離線標誌

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/wp`
- **說明**: 查詢報表模組WP的報表列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "reportCode": "",
    "reportType": "",
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
      "items": [
        {
          "queryId": 1,
          "reportCode": "SYSPE08",
          "reportName": "SYSPE08報表",
          "reportType": "NORMAL",
          "status": "1",
          "createdAt": "2024-01-01T00:00:00"
        }
      ],
      "totalCount": 12,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 執行報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/wp/{reportCode}/query`
- **說明**: 執行報表查詢
- **路徑參數**:
  - `reportCode`: 報表代碼 (SYSPE08-SYSPE19)
- **請求格式**:
  ```json
  {
    "reportType": "NORMAL",
    "queryParams": {
      "startDate": "2024-01-01",
      "endDate": "2024-01-31",
      "venderId": "",
      "posId": "",
      "empId": ""
    },
    "pageIndex": 1,
    "pageSize": 100
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
          "salesDate": "2024-01-01",
          "salesTime": "10:00:00",
          "venderId": "V001",
          "venderName": "廠商A",
          "amount": 1000.00,
          "cardNo": "1234567890"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 100,
      "totalPages": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增報表查詢設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/wp/queries`
- **說明**: 新增報表查詢設定
- **請求格式**:
  ```json
  {
    "reportCode": "SYSPE08",
    "reportName": "SYSPE08報表",
    "reportType": "NORMAL",
    "queryParams": {},
    "querySql": "SELECT * FROM ...",
    "status": "1"
  }
  ```

#### 3.1.4 修改報表查詢設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/reports/wp/queries/{queryId}`
- **說明**: 修改報表查詢設定
- **路徑參數**:
  - `queryId`: 查詢ID
- **請求格式**: 同新增

#### 3.1.5 刪除報表查詢設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/reports/wp/queries/{queryId}`
- **說明**: 刪除報表查詢設定
- **路徑參數**:
  - `queryId`: 查詢ID

#### 3.1.6 執行報表列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/wp/{reportCode}/print`
- **說明**: 執行報表列印，產生PDF檔案
- **路徑參數**:
  - `reportCode`: 報表代碼 (SYSPE08-SYSPE19)
- **請求格式**:
  ```json
  {
    "reportType": "NORMAL",
    "printParams": {
      "startDate": "2024-01-01",
      "endDate": "2024-01-31"
    },
    "printFormat": "PDF",
    "pageSize": "A4",
    "orientation": "Portrait"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "列印成功",
    "data": {
      "printId": 1,
      "fileUrl": "/api/v1/reports/wp/prints/1/download",
      "printStatus": "COMPLETED"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 查詢報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/wp/prints`
- **說明**: 查詢報表列印記錄列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "reportCode": "",
    "printStatus": "",
    "printedBy": "",
    "startDate": "",
    "endDate": ""
  }
  ```

#### 3.1.8 下載報表檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/wp/prints/{printId}/download`
- **說明**: 下載報表檔案
- **路徑參數**:
  - `printId`: 列印ID

#### 3.1.9 匯出報表資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/wp/{reportCode}/export`
- **說明**: 匯出報表資料（Excel、PDF）
- **路徑參數**:
  - `reportCode`: 報表代碼
- **請求格式**:
  ```json
  {
    "reportType": "NORMAL",
    "queryParams": {
      "startDate": "2024-01-01",
      "endDate": "2024-01-31"
    },
    "exportFormat": "Excel",
    "fileName": "報表資料"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `ReportWPController.cs`
```csharp
[ApiController]
[Route("api/v1/reports/wp")]
[Authorize]
public class ReportWPController : ControllerBase
{
    private readonly IReportWPService _reportWPService;
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportWPQueryDto>>>> GetReports([FromQuery] ReportWPQueryRequest request)
    {
        // 實作查詢報表列表邏輯
    }
    
    [HttpPost("{reportCode}/query")]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportWPDataDto>>>> QueryReport(string reportCode, [FromBody] ReportWPQueryParamsDto queryParams)
    {
        // 實作執行報表查詢邏輯
    }
    
    [HttpPost("queries")]
    public async Task<ActionResult<ApiResponse<ReportWPQueryDto>>> CreateQuery([FromBody] CreateReportWPQueryDto dto)
    {
        // 實作新增報表查詢設定邏輯
    }
    
    [HttpPut("queries/{queryId}")]
    public async Task<ActionResult<ApiResponse<ReportWPQueryDto>>> UpdateQuery(long queryId, [FromBody] UpdateReportWPQueryDto dto)
    {
        // 實作修改報表查詢設定邏輯
    }
    
    [HttpDelete("queries/{queryId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQuery(long queryId)
    {
        // 實作刪除報表查詢設定邏輯
    }
    
    [HttpPost("{reportCode}/print")]
    public async Task<ActionResult<ApiResponse<ReportWPPrintDto>>> PrintReport(string reportCode, [FromBody] ReportWPPrintParamsDto printParams)
    {
        // 實作執行報表列印邏輯
    }
    
    [HttpGet("prints")]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportWPPrintDto>>>> GetPrints([FromQuery] ReportWPPrintQueryRequest request)
    {
        // 實作查詢報表列印記錄邏輯
    }
    
    [HttpGet("prints/{printId}/download")]
    public async Task<IActionResult> DownloadPrint(long printId)
    {
        // 實作下載報表檔案邏輯
    }
    
    [HttpPost("{reportCode}/export")]
    public async Task<IActionResult> ExportReport(string reportCode, [FromBody] ReportWPExportParamsDto exportParams)
    {
        // 實作匯出報表資料邏輯
    }
}
```

#### 3.2.2 Service: `ReportWPService.cs`
```csharp
public interface IReportWPService
{
    Task<PagedResult<ReportWPQueryDto>> GetReportsAsync(ReportWPQueryRequest request);
    Task<PagedResult<ReportWPDataDto>> QueryReportAsync(string reportCode, ReportWPQueryParamsDto queryParams);
    Task<ReportWPQueryDto> CreateQueryAsync(CreateReportWPQueryDto dto);
    Task<ReportWPQueryDto> UpdateQueryAsync(long queryId, UpdateReportWPQueryDto dto);
    Task<bool> DeleteQueryAsync(long queryId);
    Task<ReportWPPrintDto> PrintReportAsync(string reportCode, ReportWPPrintParamsDto printParams);
    Task<PagedResult<ReportWPPrintDto>> GetPrintsAsync(ReportWPPrintQueryRequest request);
    Task<byte[]> DownloadPrintAsync(long printId);
    Task<byte[]> ExportReportAsync(string reportCode, ReportWPExportParamsDto exportParams);
}
```

#### 3.2.3 Repository: `ReportWPRepository.cs`
```csharp
public interface IReportWPRepository
{
    Task<PagedResult<ReportWPQuery>> GetReportsAsync(ReportWPQueryRequest request);
    Task<ReportWPQuery> GetQueryByIdAsync(long queryId);
    Task<ReportWPQuery> CreateQueryAsync(ReportWPQuery query);
    Task<ReportWPQuery> UpdateQueryAsync(ReportWPQuery query);
    Task<bool> DeleteQueryAsync(long queryId);
    Task<List<ReportWPData>> QueryReportDataAsync(string reportCode, ReportWPQueryParams queryParams);
    Task<ReportWPPrint> CreatePrintAsync(ReportWPPrint print);
    Task<PagedResult<ReportWPPrint>> GetPrintsAsync(ReportWPPrintQueryRequest request);
    Task<ReportWPPrint> GetPrintByIdAsync(long printId);
    Task<ReportWPQueryLog> CreateQueryLogAsync(ReportWPQueryLog log);
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 報表列表頁面 (`ReportWPList.vue`)
- 顯示報表列表（SYSPE08-SYSPE19）
- 支援報表類型篩選（一般、月報表、錯誤報表）
- 支援報表狀態篩選
- 支援報表查詢、列印、匯出操作

#### 4.1.2 報表查詢頁面 (`ReportWPQuery.vue`)
- 報表查詢條件表單
- 報表資料表格顯示
- 支援分頁、排序
- 支援報表列印、匯出

#### 4.1.3 報表列印頁面 (`ReportWPPrint.vue`)
- 報表列印設定表單
- 報表預覽
- 列印記錄查詢

### 4.2 UI 元件設計

#### 4.2.1 報表列表組件 (`ReportWPList.vue`)
```vue
<template>
  <div class="report-wp-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>報表模組WP</span>
          <el-button type="primary" @click="handleCreateQuery">新增查詢設定</el-button>
        </div>
      </template>
      
      <el-form :inline="true" :model="queryForm" class="query-form">
        <el-form-item label="報表代碼">
          <el-input v-model="queryForm.reportCode" placeholder="請輸入報表代碼" clearable />
        </el-form-item>
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.reportType" placeholder="請選擇" clearable>
            <el-option label="一般" value="NORMAL" />
            <el-option label="月報表" value="MONTHLY" />
            <el-option label="錯誤報表" value="ERROR" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.status" placeholder="請選擇" clearable>
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <el-table :data="reportList" border stripe>
        <el-table-column prop="reportCode" label="報表代碼" width="120" />
        <el-table-column prop="reportName" label="報表名稱" />
        <el-table-column prop="reportType" label="報表類型" width="100">
          <template #default="{ row }">
            <el-tag v-if="row.reportType === 'NORMAL'">一般</el-tag>
            <el-tag type="success" v-else-if="row.reportType === 'MONTHLY'">月報表</el-tag>
            <el-tag type="danger" v-else-if="row.reportType === 'ERROR'">錯誤報表</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === '1' ? 'success' : 'danger'">
              {{ row.status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleQueryReport(row)">查詢</el-button>
            <el-button type="success" size="small" @click="handlePrintReport(row)">列印</el-button>
            <el-button type="warning" size="small" @click="handleExportReport(row)">匯出</el-button>
            <el-button type="info" size="small" @click="handleEditQuery(row)">編輯</el-button>
            <el-button type="danger" size="small" @click="handleDeleteQuery(row)">刪除</el-button>
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
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { reportWPApi } from '@/api/reportWP.api'

const queryForm = ref({
  reportCode: '',
  reportType: '',
  status: ''
})

const reportList = ref([])
const pagination = ref({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const loadReports = async () => {
  try {
    const response = await reportWPApi.getReports({
      ...queryForm.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    reportList.value = response.data.items
    pagination.value.totalCount = response.data.totalCount
  } catch (error) {
    ElMessage.error('載入報表列表失敗')
  }
}

const handleQuery = () => {
  pagination.value.pageIndex = 1
  loadReports()
}

const handleReset = () => {
  queryForm.value = {
    reportCode: '',
    reportType: '',
    status: ''
  }
  handleQuery()
}

const handleQueryReport = (row: any) => {
  // 導航到報表查詢頁面
  router.push({
    name: 'ReportWPQuery',
    params: { reportCode: row.reportCode }
  })
}

const handlePrintReport = async (row: any) => {
  // 開啟報表列印對話框
}

const handleExportReport = async (row: any) => {
  // 開啟報表匯出對話框
}

const handleCreateQuery = () => {
  // 開啟新增查詢設定對話框
}

const handleEditQuery = (row: any) => {
  // 開啟編輯查詢設定對話框
}

const handleDeleteQuery = async (row: any) => {
  try {
    await ElMessageBox.confirm('確定要刪除此查詢設定嗎？', '提示', {
      type: 'warning'
    })
    await reportWPApi.deleteQuery(row.queryId)
    ElMessage.success('刪除成功')
    loadReports()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadReports()
}

const handlePageChange = (page: number) => {
  pagination.value.pageIndex = page
  loadReports()
}

onMounted(() => {
  loadReports()
})
</script>
```

#### 4.2.2 報表查詢組件 (`ReportWPQuery.vue`)
```vue
<template>
  <div class="report-wp-query">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>報表查詢 - {{ reportCode }}</span>
        </div>
      </template>
      
      <el-form :model="queryForm" :inline="true" class="query-form">
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.reportType" placeholder="請選擇">
            <el-option label="一般" value="NORMAL" />
            <el-option label="月報表" value="MONTHLY" />
            <el-option label="錯誤報表" value="ERROR" />
          </el-select>
        </el-form-item>
        <el-form-item label="開始日期">
          <el-date-picker v-model="queryForm.startDate" type="date" placeholder="請選擇開始日期" />
        </el-form-item>
        <el-form-item label="結束日期">
          <el-date-picker v-model="queryForm.endDate" type="date" placeholder="請選擇結束日期" />
        </el-form-item>
        <el-form-item label="廠商代號">
          <el-input v-model="queryForm.venderId" placeholder="請輸入廠商代號" clearable />
        </el-form-item>
        <el-form-item label="POS機代號">
          <el-input v-model="queryForm.posId" placeholder="請輸入POS機代號" clearable />
        </el-form-item>
        <el-form-item label="員工代號">
          <el-input v-model="queryForm.empId" placeholder="請輸入員工代號" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handlePrint">列印</el-button>
          <el-button type="warning" @click="handleExport">匯出</el-button>
        </el-form-item>
      </el-form>
      
      <el-table :data="reportData" border stripe v-loading="loading">
        <el-table-column prop="salesDate" label="銷售日期" width="120" />
        <el-table-column prop="salesTime" label="銷售時間" width="100" />
        <el-table-column prop="venderId" label="廠商代號" width="120" />
        <el-table-column prop="venderName" label="廠商名稱" />
        <el-table-column prop="amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="cardNo" label="卡號" width="150" />
        <el-table-column prop="empName" label="員工姓名" width="120" />
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
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { reportWPApi } from '@/api/reportWP.api'

const route = useRoute()
const reportCode = ref(route.params.reportCode as string)

const queryForm = ref({
  reportType: 'NORMAL',
  startDate: '',
  endDate: '',
  venderId: '',
  posId: '',
  empId: ''
})

const reportData = ref([])
const loading = ref(false)
const pagination = ref({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const loadReportData = async () => {
  loading.value = true
  try {
    const response = await reportWPApi.queryReport(reportCode.value, {
      ...queryForm.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    reportData.value = response.data.items
    pagination.value.totalCount = response.data.totalCount
  } catch (error) {
    ElMessage.error('載入報表資料失敗')
  } finally {
    loading.value = false
  }
}

const handleQuery = () => {
  pagination.value.pageIndex = 1
  loadReportData()
}

const handleReset = () => {
  queryForm.value = {
    reportType: 'NORMAL',
    startDate: '',
    endDate: '',
    venderId: '',
    posId: '',
    empId: ''
  }
}

const handlePrint = async () => {
  // 執行報表列印
  try {
    const response = await reportWPApi.printReport(reportCode.value, {
      reportType: queryForm.value.reportType,
      printParams: queryForm.value,
      printFormat: 'PDF'
    })
    window.open(response.data.fileUrl, '_blank')
    ElMessage.success('列印成功')
  } catch (error) {
    ElMessage.error('列印失敗')
  }
}

const handleExport = async () => {
  // 執行報表匯出
  try {
    const response = await reportWPApi.exportReport(reportCode.value, {
      reportType: queryForm.value.reportType,
      queryParams: queryForm.value,
      exportFormat: 'Excel'
    })
    // 下載檔案
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `${reportCode.value}_${new Date().getTime()}.xlsx`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗')
  }
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('zh-TW', {
    style: 'currency',
    currency: 'TWD'
  }).format(value)
}

const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadReportData()
}

const handlePageChange = (page: number) => {
  pagination.value.pageIndex = page
  loadReportData()
}

onMounted(() => {
  loadReportData()
})
</script>
```

### 4.3 API 呼叫 (`reportWP.api.ts`)
```typescript
import request from '@/utils/request'
import { ApiResponse, PagedResult } from '@/types/api'

export interface ReportWPQueryDto {
  queryId: number
  reportCode: string
  reportName: string
  reportType: string
  status: string
  createdAt: string
}

export interface ReportWPDataDto {
  salesDate: string
  salesTime: string
  venderId: string
  venderName: string
  amount: number
  cardNo: string
  empName: string
}

export interface ReportWPPrintDto {
  printId: number
  reportCode: string
  reportName: string
  fileUrl: string
  printStatus: string
  printedAt: string
}

export const reportWPApi = {
  getReports: (params: any) => {
    return request.get<ApiResponse<PagedResult<ReportWPQueryDto>>>('/api/v1/reports/wp', { params })
  },
  
  queryReport: (reportCode: string, params: any) => {
    return request.post<ApiResponse<PagedResult<ReportWPDataDto>>>(`/api/v1/reports/wp/${reportCode}/query`, params)
  },
  
  createQuery: (data: any) => {
    return request.post<ApiResponse<ReportWPQueryDto>>('/api/v1/reports/wp/queries', data)
  },
  
  updateQuery: (queryId: number, data: any) => {
    return request.put<ApiResponse<ReportWPQueryDto>>(`/api/v1/reports/wp/queries/${queryId}`, data)
  },
  
  deleteQuery: (queryId: number) => {
    return request.delete<ApiResponse<boolean>>(`/api/v1/reports/wp/queries/${queryId}`)
  },
  
  printReport: (reportCode: string, params: any) => {
    return request.post<ApiResponse<ReportWPPrintDto>>(`/api/v1/reports/wp/${reportCode}/print`, params)
  },
  
  getPrints: (params: any) => {
    return request.get<ApiResponse<PagedResult<ReportWPPrintDto>>>('/api/v1/reports/wp/prints', { params })
  },
  
  downloadPrint: (printId: number) => {
    return request.get(`/api/v1/reports/wp/prints/${printId}/download`, {
      responseType: 'blob'
    })
  },
  
  exportReport: (reportCode: string, params: any) => {
    return request.post(`/api/v1/reports/wp/${reportCode}/export`, params, {
      responseType: 'blob'
    })
  }
}
```

---

## 五、開發任務清單

### 5.1 後端開發
- [ ] 建立 `ReportWPQueries`、`ReportWPQueryLogs`、`ReportWPPrints` 資料表
- [ ] 建立 Entity 類別（ReportWPQuery, ReportWPQueryLog, ReportWPPrint）
- [ ] 建立 DTO 類別（ReportWPQueryDto, ReportWPDataDto, ReportWPPrintDto等）
- [ ] 實作 `ReportWPRepository` 類別
- [ ] 實作 `ReportWPService` 類別
- [ ] 實作 `ReportWPController` 類別
- [ ] 實作報表查詢SQL邏輯（針對SYSPE08-SYSPE19）
- [ ] 實作報表列印邏輯（使用RDLC或PDF生成）
- [ ] 實作報表匯出邏輯（Excel、PDF）
- [ ] 撰寫單元測試

### 5.2 前端開發
- [ ] 建立 `ReportWPList.vue` 報表列表頁面
- [ ] 建立 `ReportWPQuery.vue` 報表查詢頁面
- [ ] 建立 `ReportWPPrint.vue` 報表列印頁面
- [ ] 建立 `reportWP.api.ts` API 呼叫函數
- [ ] 建立路由設定
- [ ] 建立類型定義
- [ ] 撰寫單元測試

### 5.3 資料庫開發
- [ ] 建立 `ReportWPQueries` 資料表
- [ ] 建立 `ReportWPQueryLogs` 資料表
- [ ] 建立 `ReportWPPrints` 資料表
- [ ] 建立必要的索引
- [ ] 撰寫查詢效能測試

### 5.4 測試
- [ ] 單元測試
- [ ] 整合測試
- [ ] 效能測試
- [ ] 使用者驗收測試

---

## 六、注意事項

### 6.1 業務邏輯
- 不同報表（SYSPE08-SYSPE19）可能有不同的查詢參數和資料結構
- 部分報表支援月報表類型（SYSPE09_M, SYSPE14_M, SYSPE15_M, SYSPE17_M）
- SYSPE08支援錯誤報表查詢（SYSPE08_ERR）
- 報表列印使用RDLC報表定義檔案
- 報表匯出支援Excel和PDF格式

### 6.2 資料驗證
- 報表代碼必須為SYSPE08-SYSPE19範圍內
- 查詢參數必須符合各報表的資料結構
- 日期範圍必須合理（開始日期不能大於結束日期）

### 6.3 效能
- 大量資料查詢必須使用分頁
- 報表列印和匯出應使用非同步處理
- 報表查詢結果應使用快取機制（可選）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢報表列表成功
- [ ] 執行報表查詢成功
- [ ] 新增報表查詢設定成功
- [ ] 修改報表查詢設定成功
- [ ] 刪除報表查詢設定成功
- [ ] 執行報表列印成功
- [ ] 匯出報表資料成功

### 7.2 整合測試
- [ ] 完整報表查詢流程測試
- [ ] 完整報表列印流程測試
- [ ] 完整報表匯出流程測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSWP00/SYSPE08_PR.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE08_ERR_PR.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE09_PR.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE09_M_PR.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE14.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE14_M.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE15.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE15_M.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE16.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE17.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE17_M.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE18.rdlc`
- `WEB/IMS_CORE/SYSWP00/SYSPE18_PR.aspx`
- `WEB/IMS_CORE/SYSWP00/SYSPE19_PR.rdlc`

### 8.2 相關功能
- `SYS7000-報表模組7-報表查詢.md` - 報表查詢功能參考
- `SYS7B10-報表列印作業.md` - 報表列印功能參考

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


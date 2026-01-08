# SYSL161 - 業務報表列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL161
- **功能名稱**: 業務報表列印作業
- **功能描述**: 提供業務報表列印功能，支援業務報表的查詢、列印、匯出等功能，包含報表參數設定、報表預覽、報表列印等
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/js/SYSL161.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSL000/style/SYSL161.css` (樣式)

### 1.2 業務需求
- 支援業務報表查詢功能
- 支援業務報表列印功能
- 支援業務報表匯出功能（Excel、PDF）
- 支援報表參數設定
- 支援報表預覽功能
- 記錄報表列印歷史

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `BusinessReportPrintLog` (對應舊系統業務報表列印記錄)

```sql
CREATE TABLE [dbo].[BusinessReportPrintLog] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
    [ReportId] NVARCHAR(50) NOT NULL, -- 報表代碼 (REPORT_ID)
    [ReportName] NVARCHAR(100) NULL, -- 報表名稱 (REPORT_NAME)
    [ReportType] NVARCHAR(20) NULL, -- 報表類型 (REPORT_TYPE)
    [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 列印日期 (PRINT_DATE)
    [PrintUserId] NVARCHAR(50) NULL, -- 列印使用者 (PRINT_USER_ID)
    [PrintUserName] NVARCHAR(100) NULL, -- 列印使用者名稱 (PRINT_USER_NAME)
    [PrintParams] NVARCHAR(MAX) NULL, -- 列印參數 (JSON格式) (PRINT_PARAMS)
    [PrintFormat] NVARCHAR(20) NULL, -- 列印格式 (PRINT_FORMAT) PDF, Excel, Print
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:成功, 0:失敗
    [ErrorMessage] NVARCHAR(500) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    CONSTRAINT [PK_BusinessReportPrintLog] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_ReportId] ON [dbo].[BusinessReportPrintLog] ([ReportId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_PrintDate] ON [dbo].[BusinessReportPrintLog] ([PrintDate]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_PrintUserId] ON [dbo].[BusinessReportPrintLog] ([PrintUserId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintLog_Status] ON [dbo].[BusinessReportPrintLog] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `BusinessReports` - 業務報表主檔
- 參考: `開發計劃/10-報表管理/01-業務報表/SYSL130-業務報表查詢作業.md` 的 `BusinessReports` 資料表結構

#### 2.2.2 `Users` - 使用者主檔
- 參考: `開發計劃/01-系統管理/01-使用者管理/SYS0110-使用者基本資料維護.md` 的 `Users` 資料表結構

### 2.3 資料字典

#### BusinessReportPrintLog 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ReportId | NVARCHAR | 50 | NO | - | 報表代碼 | - |
| ReportName | NVARCHAR | 100 | YES | - | 報表名稱 | - |
| ReportType | NVARCHAR | 20 | YES | - | 報表類型 | - |
| PrintDate | DATETIME2 | - | NO | GETDATE() | 列印日期 | - |
| PrintUserId | NVARCHAR | 50 | YES | - | 列印使用者 | - |
| PrintUserName | NVARCHAR | 100 | YES | - | 列印使用者名稱 | - |
| PrintParams | NVARCHAR(MAX) | - | YES | - | 列印參數 | JSON格式 |
| PrintFormat | NVARCHAR | 20 | YES | - | 列印格式 | PDF, Excel, Print |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:成功, 0:失敗 |
| ErrorMessage | NVARCHAR | 500 | YES | - | 錯誤訊息 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢業務報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-reports/print-logs`
- **說明**: 查詢業務報表列印記錄，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "PrintDate",
    "sortOrder": "DESC",
    "filters": {
      "reportId": "",
      "reportName": "",
      "printUserId": "",
      "printDateFrom": "",
      "printDateTo": "",
      "status": ""
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
          "reportId": "REPORT001",
          "reportName": "業務報表",
          "reportType": "TYPE001",
          "printDate": "2024-01-01T00:00:00",
          "printUserId": "U001",
          "printUserName": "張三",
          "printParams": "{\"siteId\":\"SITE001\"}",
          "printFormat": "PDF",
          "status": "1",
          "errorMessage": null,
          "createdAt": "2024-01-01T00:00:00",
          "updatedAt": "2024-01-01T00:00:00"
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

#### 3.1.2 列印業務報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/{reportId}/print`
- **說明**: 列印業務報表
- **路徑參數**:
  - `reportId`: 報表代碼
- **請求格式**:
  ```json
  {
    "printFormat": "PDF",
    "printParams": {
      "siteId": "SITE001",
      "dateFrom": "2024-01-01",
      "dateTo": "2024-01-31"
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "列印成功",
    "data": {
      "printLogId": 1,
      "fileUrl": "/api/v1/files/download/print-20240101-001.pdf",
      "fileName": "業務報表-20240101.pdf"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 預覽業務報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/{reportId}/preview`
- **說明**: 預覽業務報表
- **路徑參數**:
  - `reportId`: 報表代碼
- **請求格式**: 同列印業務報表
- **回應格式**: 返回報表資料（JSON格式）

#### 3.1.4 匯出業務報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/{reportId}/export`
- **說明**: 匯出業務報表（Excel、PDF）
- **路徑參數**:
  - `reportId`: 報表代碼
- **請求格式**:
  ```json
  {
    "exportFormat": "Excel",
    "exportParams": {
      "siteId": "SITE001",
      "dateFrom": "2024-01-01",
      "dateTo": "2024-01-31"
    }
  }
  ```
- **回應格式**: 同列印業務報表

#### 3.1.5 查詢報表列印歷史
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-reports/{reportId}/print-history`
- **說明**: 查詢指定報表的列印歷史
- **路徑參數**:
  - `reportId`: 報表代碼
- **請求參數**: 同查詢業務報表列印記錄

---

## 四、前端 UI 設計

### 4.1 報表列印頁面 (`SYSL161Print.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="sysl161-print">
    <el-card>
      <template #header>
        <span>業務報表列印作業</span>
      </template>
      
      <!-- 報表選擇 -->
      <el-form :model="printForm" ref="printFormRef" label-width="150px" inline>
        <el-form-item label="報表代碼" prop="reportId">
          <el-select 
            v-model="printForm.reportId" 
            placeholder="請選擇報表"
            clearable
            filterable
            style="width: 200px"
            @change="handleReportChange"
          >
            <el-option
              v-for="item in reportList"
              :key="item.reportId"
              :label="item.reportName"
              :value="item.reportId"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="列印格式" prop="printFormat">
          <el-select 
            v-model="printForm.printFormat" 
            placeholder="請選擇列印格式"
            style="width: 200px"
          >
            <el-option label="PDF" value="PDF" />
            <el-option label="Excel" value="Excel" />
            <el-option label="列印" value="Print" />
          </el-select>
        </el-form-item>
      </el-form>
      
      <!-- 報表參數設定 -->
      <el-card v-if="printForm.reportId" style="margin-top: 20px">
        <template #header>
          <span>報表參數設定</span>
        </template>
        <el-form :model="printParams" ref="printParamsRef" label-width="150px">
          <!-- 動態參數表單，根據報表類型顯示不同參數 -->
          <el-form-item label="店別" prop="siteId">
            <el-select 
              v-model="printParams.siteId" 
              placeholder="請選擇店別"
              clearable
              filterable
              style="width: 100%"
            >
              <el-option
                v-for="item in siteList"
                :key="item.siteId"
                :label="item.siteName"
                :value="item.siteId"
              />
            </el-select>
          </el-form-item>
          
          <el-form-item label="日期區間" prop="dateRange">
            <el-date-picker
              v-model="printParams.dateRange"
              type="daterange"
              range-separator="至"
              start-placeholder="開始日期"
              end-placeholder="結束日期"
              style="width: 100%"
            />
          </el-form-item>
        </el-form>
      </el-card>
      
      <!-- 操作按鈕 -->
      <div style="margin-top: 20px; text-align: right">
        <el-button type="primary" icon="View" @click="handlePreview">預覽</el-button>
        <el-button type="success" icon="Printer" @click="handlePrint">列印</el-button>
        <el-button type="warning" icon="Download" @click="handleExport">匯出</el-button>
        <el-button icon="Refresh" @click="handleReset">重置</el-button>
      </div>
    </el-card>
    
    <!-- 報表預覽對話框 -->
    <el-dialog 
      v-model="previewVisible" 
      title="報表預覽" 
      width="90%"
      :close-on-click-modal="false"
    >
      <div v-if="previewData" class="preview-container">
        <!-- 報表預覽內容 -->
        <el-table :data="previewData" border stripe style="width: 100%">
          <!-- 動態表格欄位，根據報表類型顯示不同欄位 -->
        </el-table>
      </div>
      <template #footer>
        <el-button @click="previewVisible = false">關閉</el-button>
        <el-button type="primary" @click="handlePrintFromPreview">列印</el-button>
      </template>
    </el-dialog>
    
    <!-- 列印歷史 -->
    <el-card style="margin-top: 20px">
      <template #header>
        <span>列印歷史</span>
      </template>
      <el-table 
        :data="printHistory" 
        border 
        stripe 
        style="width: 100%"
        v-loading="historyLoading"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="reportName" label="報表名稱" width="200" />
        <el-table-column prop="printFormat" label="列印格式" width="100" align="center" />
        <el-table-column prop="printDate" label="列印日期" width="180" />
        <el-table-column prop="printUserName" label="列印使用者" width="120" />
        <el-table-column prop="status" label="狀態" width="80" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.status === '1' ? 'success' : 'danger'">
              {{ scope.row.status === '1' ? '成功' : '失敗' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="scope">
            <el-button 
              type="primary" 
              link 
              @click="handleDownload(scope.row)"
              :disabled="scope.row.status !== '1'"
            >
              下載
            </el-button>
            <el-button 
              type="danger" 
              link 
              @click="handleDeleteHistory(scope.row)"
            >
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="historyPagination.pageIndex"
        v-model:page-size="historyPagination.pageSize"
        :total="historyPagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleHistorySizeChange"
        @current-change="handleHistoryPageChange"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { businessReportApi } from '@/api/business.api'

// 表單資料
const printForm = reactive({
  reportId: '',
  printFormat: 'PDF'
})

const printParams = reactive({
  siteId: '',
  dateRange: [] as Date[]
})

// 資料列表
const reportList = ref([])
const siteList = ref([])
const printHistory = ref([])
const previewData = ref(null)
const previewVisible = ref(false)

// 分頁
const historyPagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const historyLoading = ref(false)

// 報表變更
const handleReportChange = (reportId: string) => {
  // 根據報表類型載入對應的參數表單
  loadReportParams(reportId)
}

// 載入報表參數
const loadReportParams = async (reportId: string) => {
  try {
    const response = await businessReportApi.getReportParams(reportId)
    if (response.success) {
      // 根據報表參數設定動態表單
    }
  } catch (error) {
    ElMessage.error('載入報表參數失敗：' + error.message)
  }
}

// 預覽
const handlePreview = async () => {
  try {
    await printFormRef.value.validate()
    
    const response = await businessReportApi.previewReport(printForm.reportId, {
      printParams: printParams
    })
    
    if (response.success) {
      previewData.value = response.data
      previewVisible.value = true
    } else {
      ElMessage.error(response.message || '預覽失敗')
    }
  } catch (error) {
    ElMessage.error('預覽失敗：' + error.message)
  }
}

// 列印
const handlePrint = async () => {
  try {
    await printFormRef.value.validate()
    
    const response = await businessReportApi.printReport(printForm.reportId, {
      printFormat: printForm.printFormat,
      printParams: printParams
    })
    
    if (response.success) {
      ElMessage.success('列印成功')
      // 下載檔案
      if (response.data.fileUrl) {
        window.open(response.data.fileUrl, '_blank')
      }
      // 重新載入列印歷史
      loadPrintHistory()
    } else {
      ElMessage.error(response.message || '列印失敗')
    }
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
  }
}

// 匯出
const handleExport = async () => {
  try {
    await printFormRef.value.validate()
    
    const response = await businessReportApi.exportReport(printForm.reportId, {
      exportFormat: printForm.printFormat === 'Print' ? 'PDF' : printForm.printFormat,
      exportParams: printParams
    })
    
    if (response.success) {
      ElMessage.success('匯出成功')
      // 下載檔案
      if (response.data.fileUrl) {
        window.open(response.data.fileUrl, '_blank')
      }
      // 重新載入列印歷史
      loadPrintHistory()
    } else {
      ElMessage.error(response.message || '匯出失敗')
    }
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 從預覽列印
const handlePrintFromPreview = () => {
  previewVisible.value = false
  handlePrint()
}

// 下載
const handleDownload = (row: any) => {
  // 下載邏輯
  window.open(row.fileUrl, '_blank')
}

// 刪除歷史
const handleDeleteHistory = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除此列印記錄嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const response = await businessReportApi.deletePrintLog(row.tKey)
    if (response.success) {
      ElMessage.success('刪除成功')
      loadPrintHistory()
    } else {
      ElMessage.error(response.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 重置
const handleReset = () => {
  printForm.reportId = ''
  printForm.printFormat = 'PDF'
  printParams.siteId = ''
  printParams.dateRange = []
  printFormRef.value?.resetFields()
  printParamsRef.value?.resetFields()
}

// 載入列印歷史
const loadPrintHistory = async () => {
  historyLoading.value = true
  try {
    const response = await businessReportApi.getPrintLogs({
      pageIndex: historyPagination.pageIndex,
      pageSize: historyPagination.pageSize,
      filters: {
        reportId: printForm.reportId || undefined
      }
    })
    
    if (response.success) {
      printHistory.value = response.data.items
      historyPagination.totalCount = response.data.totalCount
    } else {
      ElMessage.error(response.message || '載入列印歷史失敗')
    }
  } catch (error) {
    ElMessage.error('載入列印歷史失敗：' + error.message)
  } finally {
    historyLoading.value = false
  }
}

// 分頁變更
const handleHistorySizeChange = (size: number) => {
  historyPagination.pageSize = size
  historyPagination.pageIndex = 1
  loadPrintHistory()
}

const handleHistoryPageChange = (page: number) => {
  historyPagination.pageIndex = page
  loadPrintHistory()
}

// 初始化
onMounted(async () => {
  // 載入報表列表
  try {
    const response = await businessReportApi.getReports()
    if (response.success) {
      reportList.value = response.data
    }
  } catch (error) {
    ElMessage.error('載入報表列表失敗：' + error.message)
  }
  
  // 載入店別列表
  try {
    const response = await siteApi.getSites()
    if (response.success) {
      siteList.value = response.data
    }
  } catch (error) {
    ElMessage.error('載入店別列表失敗：' + error.message)
  }
  
  // 載入列印歷史
  loadPrintHistory()
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`BusinessReportPrintController.cs`)

```csharp
[ApiController]
[Route("api/v1/business-reports")]
[Authorize]
public class BusinessReportPrintController : ControllerBase
{
    private readonly IBusinessReportPrintService _printService;
    private readonly ILogger<BusinessReportPrintController> _logger;

    public BusinessReportPrintController(
        IBusinessReportPrintService printService,
        ILogger<BusinessReportPrintController> logger)
    {
        _printService = printService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢業務報表列印記錄
    /// </summary>
    [HttpGet("print-logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportPrintLogDto>>>> GetPrintLogs(
        [FromQuery] BusinessReportPrintLogQueryDto query)
    {
        try
        {
            var result = await _printService.GetPrintLogsAsync(query);
            return Ok(ApiResponse<PagedResult<BusinessReportPrintLogDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢業務報表列印記錄失敗");
            return BadRequest(ApiResponse<PagedResult<BusinessReportPrintLogDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 列印業務報表
    /// </summary>
    [HttpPost("{reportId}/print")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> PrintReport(
        string reportId,
        [FromBody] BusinessReportPrintRequestDto request)
    {
        try
        {
            var result = await _printService.PrintReportAsync(reportId, request);
            return Ok(ApiResponse<BusinessReportPrintResultDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "列印業務報表失敗");
            return BadRequest(ApiResponse<BusinessReportPrintResultDto>.Error("列印失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 預覽業務報表
    /// </summary>
    [HttpPost("{reportId}/preview")]
    public async Task<ActionResult<ApiResponse<object>>> PreviewReport(
        string reportId,
        [FromBody] BusinessReportPrintRequestDto request)
    {
        try
        {
            var result = await _printService.PreviewReportAsync(reportId, request);
            return Ok(ApiResponse<object>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "預覽業務報表失敗");
            return BadRequest(ApiResponse<object>.Error("預覽失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 匯出業務報表
    /// </summary>
    [HttpPost("{reportId}/export")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> ExportReport(
        string reportId,
        [FromBody] BusinessReportExportRequestDto request)
    {
        try
        {
            var result = await _printService.ExportReportAsync(reportId, request);
            return Ok(ApiResponse<BusinessReportPrintResultDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出業務報表失敗");
            return BadRequest(ApiResponse<BusinessReportPrintResultDto>.Error("匯出失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢報表列印歷史
    /// </summary>
    [HttpGet("{reportId}/print-history")]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportPrintLogDto>>>> GetPrintHistory(
        string reportId,
        [FromQuery] BusinessReportPrintLogQueryDto query)
    {
        try
        {
            query.Filters.ReportId = reportId;
            var result = await _printService.GetPrintLogsAsync(query);
            return Ok(ApiResponse<PagedResult<BusinessReportPrintLogDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢報表列印歷史失敗");
            return BadRequest(ApiResponse<PagedResult<BusinessReportPrintLogDto>>.Error("查詢失敗：" + ex.Message));
        }
    }
}
```

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 建立 BusinessReportPrintLog 資料表
   - 建立索引
   - 建立外鍵約束

2. **後端 API 開發** (2 天)
   - 實作 Service 層
   - 實作 Controller 層
   - 實作報表生成邏輯
   - 實作檔案處理邏輯
   - 單元測試

3. **前端 UI 開發** (2 天)
   - 報表列印頁面
   - 報表參數設定
   - 報表預覽功能
   - 列印歷史查詢
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 5 天

---

## 七、注意事項

### 7.1 資料驗證
- 報表代碼必須存在
- 列印參數必須符合報表要求
- 列印格式必須有效

### 7.2 安全性
- 需驗證使用者權限
- 防止 SQL Injection（使用參數化查詢）
- 檔案下載需驗證權限

### 7.3 效能優化
- 大量資料時使用分頁查詢
- 報表生成使用非同步處理
- 考慮使用 Redis 快取報表資料

### 7.4 錯誤處理
- 記錄列印失敗原因
- 提供錯誤訊息給使用者
- 支援重試機制

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 依報表代碼查詢
   - 依列印日期查詢
   - 依列印使用者查詢
   - 組合條件查詢
   - 分頁查詢

2. **列印測試**
   - 正常列印
   - 不同列印格式測試
   - 參數驗證測試
   - 錯誤處理測試

3. **預覽測試**
   - 正常預覽
   - 參數驗證測試
   - 資料格式測試

4. **匯出測試**
   - Excel 匯出測試
   - PDF 匯出測試
   - 檔案下載測試

### 8.2 效能測試
- 大量資料查詢效能
- 報表生成效能
- 檔案下載效能

### 8.3 安全性測試
- SQL Injection 測試
- 權限驗證測試
- 檔案下載權限測試


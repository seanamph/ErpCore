# SYS7B10 - 報表列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS7B10
- **功能名稱**: 報表列印作業
- **功能描述**: 提供報表模組7的報表列印功能，使用Crystal Reports進行報表列印，支援PDF格式列印、報表預覽、列印設定等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYS7000/SYS7B10_PR.aspx` (報表列印頁面)
  - `WEB/IMS_CORE/SYS7000/SYS7B10_PR.rpt` (Crystal Reports報表定義)
  - `WEB/IMS_CORE/SYS7000/SYS7B10_PR.xsd` (資料結構定義)
  - `WEB/IMS_CORE/SYS7000/SYS7B10.ascx` (使用者控制項)

### 1.2 業務需求
- 提供報表列印功能
- 支援PDF格式列印
- 支援報表預覽
- 支援列印設定（頁面大小、邊界等）
- 支援報表資料查詢與篩選
- 記錄報表列印記錄
- 支援報表匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportPrints` (報表列印記錄)

```sql
CREATE TABLE [dbo].[ReportPrints] (
    [PrintId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 列印ID
    [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (SYS7B10)
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [PrintParams] NVARCHAR(MAX) NULL, -- 列印參數 (JSON格式)
    [PrintFormat] NVARCHAR(20) NOT NULL DEFAULT 'PDF', -- 列印格式 (PDF, Excel, Word等)
    [PrintStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 列印狀態 (PENDING, PROCESSING, COMPLETED, FAILED)
    [FileUrl] NVARCHAR(500) NULL, -- 檔案URL
    [FileSize] BIGINT NULL, -- 檔案大小(位元組)
    [PageCount] INT NULL, -- 頁數
    [PrintedBy] NVARCHAR(50) NULL, -- 列印者
    [PrintedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 列印時間
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ReportPrints] PRIMARY KEY CLUSTERED ([PrintId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportPrints_ReportCode] ON [dbo].[ReportPrints] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_ReportPrints_PrintedBy] ON [dbo].[ReportPrints] ([PrintedBy]);
CREATE NONCLUSTERED INDEX [IX_ReportPrints_PrintedAt] ON [dbo].[ReportPrints] ([PrintedAt]);
CREATE NONCLUSTERED INDEX [IX_ReportPrints_PrintStatus] ON [dbo].[ReportPrints] ([PrintStatus]);
```

### 2.2 相關資料表

#### 2.2.1 `ReportQueries` - 報表查詢設定
- 參考: `開發計劃/14-報表擴展/SYS7000-報表模組7-報表查詢.md`

#### 2.2.2 `ReportPrintDetails` - 報表列印明細
```sql
CREATE TABLE [dbo].[ReportPrintDetails] (
    [DetailId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PrintId] BIGINT NOT NULL, -- 列印ID (外鍵至ReportPrints)
    [RowNo] INT NOT NULL, -- 行號
    [DataJson] NVARCHAR(MAX) NULL, -- 資料內容 (JSON格式)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ReportPrintDetails_ReportPrints] FOREIGN KEY ([PrintId]) REFERENCES [dbo].[ReportPrints] ([PrintId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportPrintDetails_PrintId] ON [dbo].[ReportPrintDetails] ([PrintId]);
```

### 2.3 資料字典

#### ReportPrints 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| PrintId | BIGINT | - | NO | IDENTITY(1,1) | 列印ID | 主鍵 |
| ReportCode | NVARCHAR | 50 | NO | - | 報表代碼 | SYS7B10 |
| ReportName | NVARCHAR | 200 | NO | - | 報表名稱 | - |
| PrintParams | NVARCHAR(MAX) | - | YES | - | 列印參數 | JSON格式 |
| PrintFormat | NVARCHAR | 20 | NO | 'PDF' | 列印格式 | PDF, Excel, Word等 |
| PrintStatus | NVARCHAR | 20 | NO | 'PENDING' | 列印狀態 | PENDING, PROCESSING, COMPLETED, FAILED |
| FileUrl | NVARCHAR | 500 | YES | - | 檔案URL | - |
| FileSize | BIGINT | - | YES | - | 檔案大小 | 位元組 |
| PageCount | INT | - | YES | - | 頁數 | - |
| PrintedBy | NVARCHAR | 50 | YES | - | 列印者 | - |
| PrintedAt | DATETIME2 | - | NO | GETDATE() | 列印時間 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7b10/prints`
- **說明**: 查詢報表列印記錄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "PrintedAt",
    "sortOrder": "DESC",
    "filters": {
      "reportCode": "SYS7B10",
      "printStatus": "",
      "printedBy": "",
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
          "printId": 1,
          "reportCode": "SYS7B10",
          "reportName": "報表列印作業",
          "printFormat": "PDF",
          "printStatus": "COMPLETED",
          "fileUrl": "/files/reports/sys7b10_20240101_001.pdf",
          "fileSize": 1024000,
          "pageCount": 10,
          "printedBy": "U001",
          "printedAt": "2024-01-01T10:00:00"
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

#### 3.1.2 執行報表列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys7000/sys7b10/print`
- **說明**: 執行報表列印，產生PDF檔案
- **請求格式**:
  ```json
  {
    "reportCode": "SYS7B10",
    "printParams": {
      "startDate": "2024-01-01",
      "endDate": "2024-01-31",
      "filter1": "value1",
      "filter2": "value2"
    },
    "printFormat": "PDF",
    "pageSize": "A4",
    "orientation": "Portrait",
    "margins": {
      "top": 0.25,
      "bottom": 0.25,
      "left": 0.25,
      "right": 0.25
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
      "printId": 1,
      "fileUrl": "/api/v1/reports/sys7000/sys7b10/prints/1/download",
      "printStatus": "COMPLETED"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢單筆報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7b10/prints/{printId}`
- **說明**: 根據列印ID查詢單筆報表列印記錄
- **路徑參數**:
  - `printId`: 列印ID
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.4 下載報表檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7b10/prints/{printId}/download`
- **說明**: 下載報表檔案
- **路徑參數**:
  - `printId`: 列印ID
- **回應**: 檔案下載（PDF、Excel等）

#### 3.1.5 預覽報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys7000/sys7b10/preview`
- **說明**: 預覽報表（不產生檔案，直接返回預覽資料）
- **請求格式**: 同執行報表列印
- **回應格式**: 返回報表預覽資料（Base64編碼的PDF或HTML）

#### 3.1.6 刪除報表列印記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/reports/sys7000/sys7b10/prints/{printId}`
- **說明**: 刪除報表列印記錄及相關檔案
- **路徑參數**:
  - `printId`: 列印ID
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 報表列印頁面 (`SYS7B10Print.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="sys7b10-print">
    <el-card>
      <template #header>
        <span>報表列印作業 (SYS7B10)</span>
      </template>
      
      <!-- 查詢條件 -->
      <el-form :model="queryForm" ref="queryFormRef" label-width="150px" inline>
        <el-form-item label="報表日期起">
          <el-date-picker
            v-model="queryForm.startDate"
            type="date"
            placeholder="選擇開始日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        
        <el-form-item label="報表日期迄">
          <el-date-picker
            v-model="queryForm.endDate"
            type="date"
            placeholder="選擇結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handlePreview">預覽</el-button>
          <el-button type="success" icon="Printer" @click="handlePrint">列印</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 列印記錄 -->
    <el-card style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>列印記錄</span>
          <el-button type="danger" icon="Delete" @click="handleBatchDelete" :disabled="selectedRows.length === 0">批次刪除</el-button>
        </div>
      </template>
      
      <el-table 
        :data="printList" 
        border 
        stripe 
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="reportName" label="報表名稱" width="200" />
        <el-table-column prop="printFormat" label="列印格式" width="100" align="center" />
        <el-table-column prop="printStatus" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getStatusType(scope.row.printStatus)">
              {{ getStatusText(scope.row.printStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="pageCount" label="頁數" width="80" align="center" />
        <el-table-column prop="fileSize" label="檔案大小" width="100" align="right">
          <template #default="scope">
            {{ formatFileSize(scope.row.fileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="printedBy" label="列印者" width="120" />
        <el-table-column prop="printedAt" label="列印時間" width="180" />
        <el-table-column label="操作" width="200" align="center" fixed="right">
          <template #default="scope">
            <el-button type="primary" link @click="handleDownload(scope.row)">下載</el-button>
            <el-button type="info" link @click="handlePreviewRecord(scope.row)">預覽</el-button>
            <el-button type="danger" link @click="handleDelete(scope.row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
    
    <!-- 報表預覽對話框 -->
    <el-dialog 
      v-model="previewVisible" 
      title="報表預覽" 
      width="90%"
      :close-on-click-modal="false"
    >
      <div style="height: 600px; overflow: auto">
        <iframe 
          v-if="previewUrl" 
          :src="previewUrl" 
          style="width: 100%; height: 100%; border: none"
        />
      </div>
      <template #footer>
        <el-button @click="previewVisible = false">關閉</el-button>
        <el-button type="primary" @click="handlePrintFromPreview">列印</el-button>
        <el-button type="success" @click="handleDownloadFromPreview">下載</el-button>
      </template>
    </el-dialog>
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { reportApi } from '@/api/report'

// 表單資料
const queryForm = reactive({
  startDate: '',
  endDate: ''
})

// 列印記錄列表
const printList = ref([])
const selectedRows = ref([])
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

// 預覽
const previewVisible = ref(false)
const previewUrl = ref('')

// 查詢列印記錄
const loadPrintList = async () => {
  try {
    const response = await reportApi.getReportPrints('SYS7B10', {
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      filters: {
        reportCode: 'SYS7B10'
      }
    })
    
    if (response.success) {
      printList.value = response.data.items
      pagination.totalCount = response.data.totalCount
    } else {
      ElMessage.error(response.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 預覽報表
const handlePreview = async () => {
  try {
    const response = await reportApi.previewReport('SYS7B10', {
      printParams: {
        startDate: queryForm.startDate,
        endDate: queryForm.endDate
      },
      printFormat: 'PDF'
    })
    
    if (response.success) {
      previewUrl.value = response.data.previewUrl
      previewVisible.value = true
    } else {
      ElMessage.error(response.message || '預覽失敗')
    }
  } catch (error) {
    ElMessage.error('預覽失敗：' + error.message)
  }
}

// 列印報表
const handlePrint = async () => {
  try {
    const response = await reportApi.printReport('SYS7B10', {
      printParams: {
        startDate: queryForm.startDate,
        endDate: queryForm.endDate
      },
      printFormat: 'PDF',
      pageSize: 'A4',
      orientation: 'Portrait',
      margins: {
        top: 0.25,
        bottom: 0.25,
        left: 0.25,
        right: 0.25
      }
    })
    
    if (response.success) {
      ElMessage.success('列印成功')
      loadPrintList()
    } else {
      ElMessage.error(response.message || '列印失敗')
    }
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
  }
}

// 下載報表
const handleDownload = async (row: any) => {
  try {
    const url = `/api/v1/reports/sys7000/sys7b10/prints/${row.printId}/download`
    window.open(url, '_blank')
  } catch (error) {
    ElMessage.error('下載失敗：' + error.message)
  }
}

// 刪除記錄
const handleDelete = async (row: any) => {
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
    
    const response = await reportApi.deleteReportPrint('SYS7B10', row.printId)
    if (response.success) {
      ElMessage.success('刪除成功')
      loadPrintList()
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
  queryForm.startDate = ''
  queryForm.endDate = ''
}

// 狀態類型
const getStatusType = (status: string) => {
  const statusMap: Record<string, string> = {
    'PENDING': 'info',
    'PROCESSING': 'warning',
    'COMPLETED': 'success',
    'FAILED': 'danger'
  }
  return statusMap[status] || 'info'
}

// 狀態文字
const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'PENDING': '待處理',
    'PROCESSING': '處理中',
    'COMPLETED': '已完成',
    'FAILED': '失敗'
  }
  return statusMap[status] || status
}

// 格式化檔案大小
const formatFileSize = (bytes: number) => {
  if (!bytes) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

// 選擇變更
const handleSelectionChange = (selection: any[]) => {
  selectedRows.value = selection
}

// 分頁變更
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  loadPrintList()
}

const handlePageChange = (page: number) => {
  pagination.pageIndex = page
  loadPrintList()
}

// 初始化
onMounted(() => {
  loadPrintList()
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`ReportPrintController.cs`)

```csharp
[ApiController]
[Route("api/v1/reports/sys7000/sys7b10")]
[Authorize]
public class SYS7B10ReportPrintController : ControllerBase
{
    private readonly IReportPrintService _reportPrintService;
    private readonly ILogger<SYS7B10ReportPrintController> _logger;

    public SYS7B10ReportPrintController(
        IReportPrintService reportPrintService,
        ILogger<SYS7B10ReportPrintController> logger)
    {
        _reportPrintService = reportPrintService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢報表列印記錄
    /// </summary>
    [HttpGet("prints")]
    public async Task<ActionResult<ApiResponse<PagedResult<ReportPrintDto>>>> GetReportPrints(
        [FromQuery] ReportPrintQueryDto query)
    {
        try
        {
            query.ReportCode = "SYS7B10";
            var result = await _reportPrintService.GetReportPrintsAsync(query);
            return Ok(ApiResponse<PagedResult<ReportPrintDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢報表列印記錄失敗");
            return BadRequest(ApiResponse<PagedResult<ReportPrintDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 執行報表列印
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<ReportPrintResultDto>>> PrintReport(
        [FromBody] ReportPrintRequestDto dto)
    {
        try
        {
            dto.ReportCode = "SYS7B10";
            var result = await _reportPrintService.PrintReportAsync(dto);
            return Ok(ApiResponse<ReportPrintResultDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "執行報表列印失敗");
            return BadRequest(ApiResponse<ReportPrintResultDto>.Error("列印失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 預覽報表
    /// </summary>
    [HttpPost("preview")]
    public async Task<ActionResult<ApiResponse<ReportPreviewDto>>> PreviewReport(
        [FromBody] ReportPrintRequestDto dto)
    {
        try
        {
            dto.ReportCode = "SYS7B10";
            var result = await _reportPrintService.PreviewReportAsync(dto);
            return Ok(ApiResponse<ReportPreviewDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "預覽報表失敗");
            return BadRequest(ApiResponse<ReportPreviewDto>.Error("預覽失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 下載報表檔案
    /// </summary>
    [HttpGet("prints/{printId}/download")]
    public async Task<IActionResult> DownloadReportPrint(long printId)
    {
        try
        {
            var fileInfo = await _reportPrintService.GetReportPrintFileAsync(printId);
            if (fileInfo == null || !System.IO.File.Exists(fileInfo.FilePath))
            {
                return NotFound("檔案不存在");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fileInfo.FilePath);
            return File(fileBytes, fileInfo.ContentType, fileInfo.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下載報表檔案失敗");
            return BadRequest("下載失敗：" + ex.Message);
        }
    }

    /// <summary>
    /// 刪除報表列印記錄
    /// </summary>
    [HttpDelete("prints/{printId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteReportPrint(long printId)
    {
        try
        {
            await _reportPrintService.DeleteReportPrintAsync(printId);
            return Ok(ApiResponse<object>.Success(null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除報表列印記錄失敗");
            return BadRequest(ApiResponse<object>.Error("刪除失敗：" + ex.Message));
        }
    }
}
```

### 5.2 Service (`ReportPrintService.cs`)

```csharp
public class ReportPrintService : IReportPrintService
{
    private readonly IDbConnection _db;
    private readonly ILogger<ReportPrintService> _logger;
    private readonly IReportGenerator _reportGenerator;
    private readonly IFileStorageService _fileStorageService;

    public ReportPrintService(
        IDbConnection db,
        ILogger<ReportPrintService> logger,
        IReportGenerator reportGenerator,
        IFileStorageService fileStorageService)
    {
        _db = db;
        _logger = logger;
        _reportGenerator = reportGenerator;
        _fileStorageService = fileStorageService;
    }

    public async Task<PagedResult<ReportPrintDto>> GetReportPrintsAsync(ReportPrintQueryDto query)
    {
        var sql = @"
            SELECT 
                PrintId,
                ReportCode,
                ReportName,
                PrintFormat,
                PrintStatus,
                FileUrl,
                FileSize,
                PageCount,
                PrintedBy,
                PrintedAt
            FROM ReportPrints
            WHERE 1 = 1
                AND (@ReportCode IS NULL OR ReportCode = @ReportCode)
                AND (@PrintStatus IS NULL OR PrintStatus = @PrintStatus)
                AND (@PrintedBy IS NULL OR PrintedBy = @PrintedBy)
                AND (@StartDate IS NULL OR PrintedAt >= @StartDate)
                AND (@EndDate IS NULL OR PrintedAt <= @EndDate)
            ORDER BY PrintedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            
            SELECT COUNT(*) AS TotalCount
            FROM ReportPrints
            WHERE 1 = 1
                AND (@ReportCode IS NULL OR ReportCode = @ReportCode)
                AND (@PrintStatus IS NULL OR PrintStatus = @PrintStatus)
                AND (@PrintedBy IS NULL OR PrintedBy = @PrintedBy)
                AND (@StartDate IS NULL OR PrintedAt >= @StartDate)
                AND (@EndDate IS NULL OR PrintedAt <= @EndDate);
        ";

        var parameters = new
        {
            ReportCode = query.Filters?.ReportCode,
            PrintStatus = query.Filters?.PrintStatus,
            PrintedBy = query.Filters?.PrintedBy,
            StartDate = query.Filters?.StartDate,
            EndDate = query.Filters?.EndDate,
            Offset = (query.PageIndex - 1) * query.PageSize,
            PageSize = query.PageSize
        };

        using var multi = await _db.QueryMultipleAsync(sql, parameters);
        var items = (await multi.ReadAsync<ReportPrintDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();

        return new PagedResult<ReportPrintDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<ReportPrintResultDto> PrintReportAsync(ReportPrintRequestDto dto)
    {
        // 建立列印記錄
        var printRecord = new ReportPrint
        {
            ReportCode = dto.ReportCode,
            ReportName = GetReportName(dto.ReportCode),
            PrintParams = JsonSerializer.Serialize(dto.PrintParams),
            PrintFormat = dto.PrintFormat,
            PrintStatus = "PROCESSING",
            PrintedBy = GetCurrentUserId(),
            PrintedAt = DateTime.Now
        };

        var printId = await CreateReportPrintAsync(printRecord);

        try
        {
            // 產生報表檔案
            var reportData = await GetReportDataAsync(dto.ReportCode, dto.PrintParams);
            var fileInfo = await _reportGenerator.GenerateReportAsync(
                dto.ReportCode,
                reportData,
                dto.PrintFormat,
                dto.PageSize,
                dto.Orientation,
                dto.Margins
            );

            // 更新列印記錄
            printRecord.PrintId = printId;
            printRecord.PrintStatus = "COMPLETED";
            printRecord.FileUrl = fileInfo.FileUrl;
            printRecord.FileSize = fileInfo.FileSize;
            printRecord.PageCount = fileInfo.PageCount;
            await UpdateReportPrintAsync(printRecord);

            return new ReportPrintResultDto
            {
                PrintId = printId,
                FileUrl = fileInfo.FileUrl,
                PrintStatus = "COMPLETED"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "產生報表檔案失敗");
            
            // 更新列印記錄為失敗
            printRecord.PrintId = printId;
            printRecord.PrintStatus = "FAILED";
            await UpdateReportPrintAsync(printRecord);

            throw;
        }
    }

    public async Task<ReportPreviewDto> PreviewReportAsync(ReportPrintRequestDto dto)
    {
        // 取得報表資料
        var reportData = await GetReportDataAsync(dto.ReportCode, dto.PrintParams);
        
        // 產生預覽檔案（臨時檔案）
        var fileInfo = await _reportGenerator.GenerateReportAsync(
            dto.ReportCode,
            reportData,
            "PDF",
            dto.PageSize,
            dto.Orientation,
            dto.Margins
        );

        // 讀取檔案並轉換為Base64
        var fileBytes = await System.IO.File.ReadAllBytesAsync(fileInfo.FilePath);
        var base64Content = Convert.ToBase64String(fileBytes);

        // 刪除臨時檔案
        System.IO.File.Delete(fileInfo.FilePath);

        return new ReportPreviewDto
        {
            PreviewUrl = $"data:application/pdf;base64,{base64Content}",
            PageCount = fileInfo.PageCount
        };
    }

    private async Task<long> CreateReportPrintAsync(ReportPrint print)
    {
        var sql = @"
            INSERT INTO ReportPrints (
                ReportCode, ReportName, PrintParams, PrintFormat, PrintStatus,
                PrintedBy, PrintedAt, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
            ) VALUES (
                @ReportCode, @ReportName, @PrintParams, @PrintFormat, @PrintStatus,
                @PrintedBy, @PrintedAt, @CreatedBy, GETDATE(), @UpdatedBy, GETDATE()
            );
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
        ";

        var parameters = new
        {
            print.ReportCode,
            print.ReportName,
            print.PrintParams,
            print.PrintFormat,
            print.PrintStatus,
            print.PrintedBy,
            print.PrintedAt,
            CreatedBy = GetCurrentUserId(),
            UpdatedBy = GetCurrentUserId()
        };

        return await _db.QuerySingleAsync<long>(sql, parameters);
    }

    private async Task UpdateReportPrintAsync(ReportPrint print)
    {
        var sql = @"
            UPDATE ReportPrints SET
                PrintStatus = @PrintStatus,
                FileUrl = @FileUrl,
                FileSize = @FileSize,
                PageCount = @PageCount,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = GETDATE()
            WHERE PrintId = @PrintId;
        ";

        var parameters = new
        {
            print.PrintId,
            print.PrintStatus,
            print.FileUrl,
            print.FileSize,
            print.PageCount,
            UpdatedBy = GetCurrentUserId()
        };

        await _db.ExecuteAsync(sql, parameters);
    }

    private async Task<object> GetReportDataAsync(string reportCode, Dictionary<string, object> printParams)
    {
        // 根據報表代碼取得對應的資料
        // 這裡需要實作具體的資料查詢邏輯
        // 例如：查詢SYS7B10對應的資料表
        var sql = @"
            SELECT * FROM ReportData
            WHERE ReportCode = @ReportCode
                AND (@StartDate IS NULL OR ReportDate >= @StartDate)
                AND (@EndDate IS NULL OR ReportDate <= @EndDate);
        ";

        var parameters = new
        {
            ReportCode = reportCode,
            StartDate = printParams?.GetValueOrDefault("startDate"),
            EndDate = printParams?.GetValueOrDefault("endDate")
        };

        return await _db.QueryAsync(sql, parameters);
    }

    private string GetReportName(string reportCode)
    {
        var reportNames = new Dictionary<string, string>
        {
            { "SYS7B10", "報表列印作業" }
        };
        return reportNames.GetValueOrDefault(reportCode, reportCode);
    }

    private string GetCurrentUserId()
    {
        // 從 JWT Token 或 Session 取得目前使用者 ID
        return "U001"; // 暫時返回固定值，需實作
    }

    public async Task DeleteReportPrintAsync(long printId)
    {
        // 取得檔案資訊
        var print = await _db.QueryFirstOrDefaultAsync<ReportPrint>(
            "SELECT * FROM ReportPrints WHERE PrintId = @PrintId",
            new { PrintId = printId });

        if (print != null && !string.IsNullOrEmpty(print.FileUrl))
        {
            // 刪除檔案
            await _fileStorageService.DeleteFileAsync(print.FileUrl);
        }

        // 刪除記錄
        await _db.ExecuteAsync(
            "DELETE FROM ReportPrints WHERE PrintId = @PrintId",
            new { PrintId = printId });
    }

    public async Task<ReportPrintFileInfo> GetReportPrintFileAsync(long printId)
    {
        var print = await _db.QueryFirstOrDefaultAsync<ReportPrint>(
            "SELECT * FROM ReportPrints WHERE PrintId = @PrintId",
            new { PrintId = printId });

        if (print == null || string.IsNullOrEmpty(print.FileUrl))
        {
            return null;
        }

        var filePath = await _fileStorageService.GetFilePathAsync(print.FileUrl);
        if (!System.IO.File.Exists(filePath))
        {
            return null;
        }

        var fileInfo = new System.IO.FileInfo(filePath);
        return new ReportPrintFileInfo
        {
            FilePath = filePath,
            FileName = $"{print.ReportCode}_{print.PrintedAt:yyyyMMdd_HHmmss}.{print.PrintFormat.ToLower()}",
            ContentType = GetContentType(print.PrintFormat)
        };
    }

    private string GetContentType(string format)
    {
        var contentTypes = new Dictionary<string, string>
        {
            { "PDF", "application/pdf" },
            { "Excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "Word", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
        };
        return contentTypes.GetValueOrDefault(format, "application/octet-stream");
    }
}
```

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 建立 ReportPrints 資料表
   - 建立 ReportPrintDetails 資料表
   - 建立索引

2. **後端 API 開發** (3 天)
   - 實作 Service 層
   - 實作 Controller 層
   - 實作報表產生器 (使用 RDLC 或 iTextSharp)
   - 實作檔案儲存服務
   - 單元測試

3. **前端 UI 開發** (2 天)
   - 報表列印頁面
   - 報表預覽功能
   - 列印記錄查詢
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 6 天

---

## 七、注意事項

### 7.1 報表產生
- 使用 RDLC (Report Definition Language Client) 或 iTextSharp 產生 PDF
- 支援多種報表格式（PDF、Excel、Word）
- 支援自訂頁面大小和邊界設定
- 注意報表資料量，避免記憶體溢出

### 7.2 檔案管理
- 報表檔案需儲存在安全的目錄
- 定期清理過期的報表檔案
- 檔案下載需驗證權限
- 檔案大小限制

### 7.3 效能優化
- 大量資料時使用分頁查詢
- 報表產生使用非同步處理
- 考慮使用背景任務處理報表產生
- 快取常用的報表資料

### 7.4 安全性
- 驗證使用者權限
- 防止路徑遍歷攻擊
- 檔案下載需驗證權限
- 記錄報表列印操作

---

## 八、測試案例

### 8.1 功能測試
1. **列印測試**
   - 正常列印報表
   - 不同格式列印（PDF、Excel）
   - 不同頁面大小列印
   - 大量資料列印

2. **預覽測試**
   - 報表預覽功能
   - 預覽資料正確性

3. **查詢測試**
   - 依日期範圍查詢
   - 依狀態查詢
   - 依列印者查詢
   - 分頁查詢

4. **下載測試**
   - 檔案下載功能
   - 檔案完整性驗證

5. **刪除測試**
   - 刪除列印記錄
   - 刪除相關檔案

### 8.2 效能測試
- 大量資料列印效能
- 並發列印測試
- 檔案下載效能

### 8.3 安全性測試
- 權限驗證測試
- 檔案下載權限測試
- 路徑遍歷攻擊測試


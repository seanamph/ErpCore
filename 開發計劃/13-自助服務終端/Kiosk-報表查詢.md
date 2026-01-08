# Kiosk - 自助服務終端報表查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: Kiosk-Report
- **功能名稱**: 自助服務終端報表查詢
- **功能描述**: 提供自助服務終端（Kiosk）交易記錄的查詢與報表功能，包含交易統計、功能代碼分析、錯誤分析等
- **參考舊程式**: 
  - `WEB/IMS_CORE/kiosk/WebLoyaltyDataTr.asp`
  - `WEB/IMS_CORE/kiosk/DB_UTIL.ASP`
  - `開發計劃/13-自助服務終端/Kiosk-資料處理作業.md`

### 1.2 業務需求
- 查詢Kiosk交易記錄
- 統計Kiosk交易數量
- 分析功能代碼使用情況
- 分析錯誤發生情況
- 支援多種查詢條件（日期範圍、Kiosk機號、功能代碼、狀態等）
- 支援報表匯出（Excel、PDF）
- 支援圖表展示

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `KioskTransactions` (Kiosk交易主檔)
參考 `開發計劃/13-自助服務終端/Kiosk-資料處理作業.md` 的 `KioskTransactions` 資料表設計

### 2.2 報表統計資料表: `KioskReportStatistics` (Kiosk報表統計)

```sql
CREATE TABLE [dbo].[KioskReportStatistics] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportDate] DATE NOT NULL, -- 報表日期
    [KioskId] NVARCHAR(50) NULL, -- Kiosk機號（NULL表示全部）
    [FunctionCode] NVARCHAR(10) NULL, -- 功能代碼（NULL表示全部）
    [TotalCount] INT NOT NULL DEFAULT 0, -- 總交易數
    [SuccessCount] INT NOT NULL DEFAULT 0, -- 成功交易數
    [FailedCount] INT NOT NULL DEFAULT 0, -- 失敗交易數
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [UQ_KioskReportStatistics_Date_Kiosk_Function] UNIQUE ([ReportDate], [KioskId], [FunctionCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_KioskReportStatistics_ReportDate] ON [dbo].[KioskReportStatistics] ([ReportDate]);
CREATE NONCLUSTERED INDEX [IX_KioskReportStatistics_KioskId] ON [dbo].[KioskReportStatistics] ([KioskId]);
CREATE NONCLUSTERED INDEX [IX_KioskReportStatistics_FunctionCode] ON [dbo].[KioskReportStatistics] ([FunctionCode]);
```

### 2.3 資料字典

#### KioskReportStatistics 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ReportDate | DATE | - | NO | - | 報表日期 | - |
| KioskId | NVARCHAR | 50 | YES | - | Kiosk機號 | NULL表示全部 |
| FunctionCode | NVARCHAR | 10 | YES | - | 功能代碼 | NULL表示全部 |
| TotalCount | INT | - | NO | 0 | 總交易數 | - |
| SuccessCount | INT | - | NO | 0 | 成功交易數 | - |
| FailedCount | INT | - | NO | 0 | 失敗交易數 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢Kiosk交易記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kiosk/reports/transactions`
- **說明**: 查詢Kiosk交易記錄，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TransactionDate",
    "sortOrder": "DESC",
    "filters": {
      "kioskId": "",
      "functionCode": "",
      "cardNumber": "",
      "memberId": "",
      "status": "",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31"
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
          "id": 1,
          "transactionId": "TXN001",
          "kioskId": "KIOSK001",
          "functionCode": "A1",
          "functionCodeName": "確認會員卡號、密碼",
          "cardNumber": "1234567890",
          "memberId": "M001",
          "transactionDate": "2024-01-01T10:00:00",
          "status": "Success",
          "returnCode": "0000",
          "errorMessage": null
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

#### 3.1.2 查詢Kiosk交易統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kiosk/reports/statistics`
- **說明**: 查詢Kiosk交易統計資料
- **請求參數**:
  ```json
  {
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "kioskId": "",
    "functionCode": "",
    "groupBy": "Date" // Date, Kiosk, FunctionCode
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "groupKey": "2024-01-01",
        "groupName": "2024-01-01",
        "totalCount": 100,
        "successCount": 95,
        "failedCount": 5,
        "successRate": 95.0
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢Kiosk功能代碼統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kiosk/reports/function-statistics`
- **說明**: 查詢各功能代碼的使用統計
- **請求參數**: 同統計查詢
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "functionCode": "A1",
        "functionCodeName": "確認會員卡號、密碼",
        "totalCount": 500,
        "successCount": 480,
        "failedCount": 20,
        "successRate": 96.0
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 查詢Kiosk錯誤分析
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kiosk/reports/error-analysis`
- **說明**: 查詢Kiosk錯誤發生情況分析
- **請求參數**: 同統計查詢
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "returnCode": "E001",
        "errorMessage": "會員卡號不存在",
        "count": 10,
        "percentage": 50.0
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 匯出Kiosk交易報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kiosk/reports/export`
- **說明**: 匯出Kiosk交易報表（Excel、PDF）
- **請求格式**:
  ```json
  {
    "exportType": "Excel", // Excel, PDF
    "filters": {
      "kioskId": "",
      "functionCode": "",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31"
    }
  }
  ```
- **回應格式**: 檔案下載

---

## 四、前端 UI 設計

### 4.1 查詢頁面 (`KioskReportQuery.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="kiosk-report-query">
    <el-card>
      <template #header>
        <span>自助服務終端報表查詢</span>
      </template>
      
      <el-form :model="queryForm" ref="queryFormRef" label-width="150px" inline>
        <!-- Kiosk機號 -->
        <el-form-item label="Kiosk機號">
          <el-input 
            v-model="queryForm.kioskId" 
            placeholder="請輸入Kiosk機號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <!-- 功能代碼 -->
        <el-form-item label="功能代碼">
          <el-select 
            v-model="queryForm.functionCode" 
            placeholder="請選擇功能代碼"
            clearable
            style="width: 200px"
          >
            <el-option label="O2 - 網路線上快速開卡" value="O2" />
            <el-option label="A1 - 確認會員卡號、密碼" value="A1" />
            <el-option label="C2 - 密碼變更" value="C2" />
            <el-option label="D4 - 網路會員點數資訊" value="D4" />
            <el-option label="D8 - 實體會員點數資訊" value="D8" />
          </el-select>
        </el-form-item>
        
        <!-- 卡片編號 -->
        <el-form-item label="卡片編號">
          <el-input 
            v-model="queryForm.cardNumber" 
            placeholder="請輸入卡片編號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <!-- 狀態 -->
        <el-form-item label="狀態">
          <el-select 
            v-model="queryForm.status" 
            placeholder="請選擇狀態"
            clearable
            style="width: 200px"
          >
            <el-option label="成功" value="Success" />
            <el-option label="失敗" value="Failed" />
          </el-select>
        </el-form-item>
        
        <!-- 日期範圍 -->
        <el-form-item label="交易日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            style="width: 300px"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        
        <!-- 查詢按鈕 -->
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
          <el-button type="success" icon="Download" @click="handleExport">匯出</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 統計卡片 -->
    <el-row :gutter="20" style="margin-top: 20px">
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="stat-label">總交易數</div>
            <div class="stat-value">{{ statistics.totalCount }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="stat-label">成功交易數</div>
            <div class="stat-value success">{{ statistics.successCount }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="stat-label">失敗交易數</div>
            <div class="stat-value danger">{{ statistics.failedCount }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="stat-label">成功率</div>
            <div class="stat-value">{{ statistics.successRate }}%</div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 查詢結果 -->
    <el-card v-if="searchResult.length > 0" style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>查詢結果 (共 {{ totalCount }} 筆)</span>
          <div>
            <el-button type="primary" icon="Printer" @click="handlePrint">列印</el-button>
          </div>
        </div>
      </template>
      
      <el-table 
        :data="searchResult" 
        border 
        stripe 
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="transactionId" label="交易編號" width="150" />
        <el-table-column prop="kioskId" label="Kiosk機號" width="120" />
        <el-table-column prop="functionCode" label="功能代碼" width="100" />
        <el-table-column prop="functionCodeName" label="功能名稱" width="200" />
        <el-table-column prop="cardNumber" label="卡片編號" width="150" />
        <el-table-column prop="memberId" label="會員編號" width="120" />
        <el-table-column prop="transactionDate" label="交易日期時間" width="180" />
        <el-table-column prop="status" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.status === 'Success' ? 'success' : 'danger'">
              {{ scope.row.status === 'Success' ? '成功' : '失敗' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="returnCode" label="回應碼" width="100" />
        <el-table-column prop="errorMessage" label="錯誤訊息" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" width="100" align="center" fixed="right">
          <template #default="scope">
            <el-button type="primary" link @click="handleViewDetail(scope.row)">詳情</el-button>
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
    
    <!-- 詳情對話框 -->
    <el-dialog 
      v-model="detailDialogVisible" 
      title="交易詳情" 
      width="800px"
    >
      <el-descriptions :column="2" border>
        <el-descriptions-item label="交易編號">{{ detailData.transactionId }}</el-descriptions-item>
        <el-descriptions-item label="Kiosk機號">{{ detailData.kioskId }}</el-descriptions-item>
        <el-descriptions-item label="功能代碼">{{ detailData.functionCode }}</el-descriptions-item>
        <el-descriptions-item label="功能名稱">{{ detailData.functionCodeName }}</el-descriptions-item>
        <el-descriptions-item label="卡片編號">{{ detailData.cardNumber }}</el-descriptions-item>
        <el-descriptions-item label="會員編號">{{ detailData.memberId }}</el-descriptions-item>
        <el-descriptions-item label="交易日期時間">{{ detailData.transactionDate }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="detailData.status === 'Success' ? 'success' : 'danger'">
            {{ detailData.status === 'Success' ? '成功' : '失敗' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="回應碼">{{ detailData.returnCode }}</el-descriptions-item>
        <el-descriptions-item label="錯誤訊息" :span="2">{{ detailData.errorMessage }}</el-descriptions-item>
        <el-descriptions-item label="請求資料" :span="2">
          <pre>{{ JSON.stringify(JSON.parse(detailData.requestData || '{}'), null, 2) }}</pre>
        </el-descriptions-item>
        <el-descriptions-item label="回應資料" :span="2">
          <pre>{{ JSON.stringify(JSON.parse(detailData.responseData || '{}'), null, 2) }}</pre>
        </el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { kioskReportApi } from '@/api/kiosk'

// 表單資料
const queryForm = reactive({
  kioskId: '',
  functionCode: '',
  cardNumber: '',
  memberId: '',
  status: '',
  startDate: '',
  endDate: ''
})

const dateRange = ref<[Date, Date] | null>(null)

// 查詢結果
const searchResult = ref([])
const totalCount = ref(0)
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

// 統計資料
const statistics = reactive({
  totalCount: 0,
  successCount: 0,
  failedCount: 0,
  successRate: 0
})

// 詳情對話框
const detailDialogVisible = ref(false)
const detailData = ref({})

// 查詢
const handleSearch = async () => {
  try {
    const response = await kioskReportApi.getTransactions({
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      filters: {
        kioskId: queryForm.kioskId || undefined,
        functionCode: queryForm.functionCode || undefined,
        cardNumber: queryForm.cardNumber || undefined,
        memberId: queryForm.memberId || undefined,
        status: queryForm.status || undefined,
        startDate: queryForm.startDate || undefined,
        endDate: queryForm.endDate || undefined
      }
    })
    
    if (response.success) {
      searchResult.value = response.data.items
      pagination.totalCount = response.data.totalCount
      totalCount.value = response.data.totalCount
      
      // 載入統計資料
      await loadStatistics()
    } else {
      ElMessage.error(response.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 載入統計資料
const loadStatistics = async () => {
  try {
    const response = await kioskReportApi.getStatistics({
      startDate: queryForm.startDate || undefined,
      endDate: queryForm.endDate || undefined,
      kioskId: queryForm.kioskId || undefined,
      functionCode: queryForm.functionCode || undefined,
      groupBy: 'Date'
    })
    
    if (response.success && response.data.length > 0) {
      const total = response.data.reduce((sum: number, item: any) => sum + item.totalCount, 0)
      const success = response.data.reduce((sum: number, item: any) => sum + item.successCount, 0)
      const failed = response.data.reduce((sum: number, item: any) => sum + item.failedCount, 0)
      
      statistics.totalCount = total
      statistics.successCount = success
      statistics.failedCount = failed
      statistics.successRate = total > 0 ? Math.round((success / total) * 100 * 100) / 100 : 0
    }
  } catch (error) {
    console.error('載入統計資料失敗：', error)
  }
}

// 重置
const handleReset = () => {
  queryForm.kioskId = ''
  queryForm.functionCode = ''
  queryForm.cardNumber = ''
  queryForm.memberId = ''
  queryForm.status = ''
  queryForm.startDate = ''
  queryForm.endDate = ''
  dateRange.value = null
  searchResult.value = []
  pagination.pageIndex = 1
  pagination.totalCount = 0
  totalCount.value = 0
  Object.assign(statistics, {
    totalCount: 0,
    successCount: 0,
    failedCount: 0,
    successRate: 0
  })
}

// 日期範圍變更
const handleDateRangeChange = (dates: [Date, Date] | null) => {
  if (dates) {
    queryForm.startDate = dates[0].toISOString().split('T')[0]
    queryForm.endDate = dates[1].toISOString().split('T')[0]
  } else {
    queryForm.startDate = ''
    queryForm.endDate = ''
  }
}

// 查看詳情
const handleViewDetail = (row: any) => {
  detailData.value = row
  detailDialogVisible.value = true
}

// 匯出
const handleExport = async () => {
  try {
    const response = await kioskReportApi.exportReport({
      exportType: 'Excel',
      filters: {
        kioskId: queryForm.kioskId || undefined,
        functionCode: queryForm.functionCode || undefined,
        startDate: queryForm.startDate || undefined,
        endDate: queryForm.endDate || undefined
      }
    })
    
    if (response.success) {
      // 下載檔案
      const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `Kiosk報表_${new Date().toISOString().split('T')[0]}.xlsx`
      link.click()
      window.URL.revokeObjectURL(url)
      ElMessage.success('匯出成功')
    } else {
      ElMessage.error(response.message || '匯出失敗')
    }
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 列印
const handlePrint = () => {
  window.print()
}

// 分頁變更
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  handleSearch()
}

const handlePageChange = (page: number) => {
  pagination.pageIndex = page
  handleSearch()
}

// 初始化
onMounted(() => {
  // 設定預設日期範圍（最近30天）
  const endDate = new Date()
  const startDate = new Date()
  startDate.setDate(startDate.getDate() - 30)
  dateRange.value = [startDate, endDate]
  queryForm.startDate = startDate.toISOString().split('T')[0]
  queryForm.endDate = endDate.toISOString().split('T')[0]
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`KioskReportController.cs`)

```csharp
[ApiController]
[Route("api/v1/kiosk/reports")]
[Authorize]
public class KioskReportController : ControllerBase
{
    private readonly IKioskReportService _kioskReportService;
    private readonly ILogger<KioskReportController> _logger;

    public KioskReportController(
        IKioskReportService kioskReportService,
        ILogger<KioskReportController> logger)
    {
        _kioskReportService = kioskReportService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢Kiosk交易記錄
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<KioskTransactionDto>>>> GetTransactions(
        [FromQuery] KioskTransactionQueryDto query)
    {
        try
        {
            var result = await _kioskReportService.GetTransactionsAsync(query);
            return Ok(ApiResponse<PagedResult<KioskTransactionDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢Kiosk交易記錄失敗");
            return BadRequest(ApiResponse<PagedResult<KioskTransactionDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢Kiosk交易統計
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<List<KioskStatisticsDto>>>> GetStatistics(
        [FromQuery] KioskStatisticsQueryDto query)
    {
        try
        {
            var result = await _kioskReportService.GetStatisticsAsync(query);
            return Ok(ApiResponse<List<KioskStatisticsDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢Kiosk交易統計失敗");
            return BadRequest(ApiResponse<List<KioskStatisticsDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢Kiosk功能代碼統計
    /// </summary>
    [HttpGet("function-statistics")]
    public async Task<ActionResult<ApiResponse<List<KioskFunctionStatisticsDto>>>> GetFunctionStatistics(
        [FromQuery] KioskStatisticsQueryDto query)
    {
        try
        {
            var result = await _kioskReportService.GetFunctionStatisticsAsync(query);
            return Ok(ApiResponse<List<KioskFunctionStatisticsDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢Kiosk功能代碼統計失敗");
            return BadRequest(ApiResponse<List<KioskFunctionStatisticsDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢Kiosk錯誤分析
    /// </summary>
    [HttpGet("error-analysis")]
    public async Task<ActionResult<ApiResponse<List<KioskErrorAnalysisDto>>>> GetErrorAnalysis(
        [FromQuery] KioskStatisticsQueryDto query)
    {
        try
        {
            var result = await _kioskReportService.GetErrorAnalysisAsync(query);
            return Ok(ApiResponse<List<KioskErrorAnalysisDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢Kiosk錯誤分析失敗");
            return BadRequest(ApiResponse<List<KioskErrorAnalysisDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 匯出Kiosk交易報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportReport([FromBody] KioskReportExportDto dto)
    {
        try
        {
            var fileBytes = await _kioskReportService.ExportReportAsync(dto);
            var fileName = $"Kiosk報表_{DateTime.Now:yyyyMMdd}.{(dto.ExportType == "Excel" ? "xlsx" : "pdf")}";
            return File(fileBytes, 
                dto.ExportType == "Excel" 
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" 
                    : "application/pdf", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出Kiosk交易報表失敗");
            return BadRequest(ApiResponse<object>.Error("匯出失敗：" + ex.Message));
        }
    }
}
```

### 5.2 Service (`KioskReportService.cs`)

```csharp
public class KioskReportService : IKioskReportService
{
    private readonly IDbConnection _db;
    private readonly ILogger<KioskReportService> _logger;

    public KioskReportService(
        IDbConnection db,
        ILogger<KioskReportService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<KioskTransactionDto>> GetTransactionsAsync(KioskTransactionQueryDto query)
    {
        var sql = @"
            SELECT 
                kt.Id,
                kt.TransactionId,
                kt.KioskId,
                kt.FunctionCode,
                CASE kt.FunctionCode
                    WHEN 'O2' THEN '網路線上快速開卡'
                    WHEN 'A1' THEN '確認會員卡號、密碼'
                    WHEN 'C2' THEN '密碼變更'
                    WHEN 'D4' THEN '網路會員點數資訊'
                    WHEN 'D8' THEN '實體會員點數資訊'
                    ELSE kt.FunctionCode
                END AS FunctionCodeName,
                kt.CardNumber,
                kt.MemberId,
                kt.TransactionDate,
                kt.Status,
                kt.ReturnCode,
                kt.ErrorMessage
            FROM KioskTransactions kt
            WHERE 1 = 1
                AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
                AND (@FunctionCode IS NULL OR kt.FunctionCode = @FunctionCode)
                AND (@CardNumber IS NULL OR kt.CardNumber LIKE '%' + @CardNumber + '%')
                AND (@MemberId IS NULL OR kt.MemberId = @MemberId)
                AND (@Status IS NULL OR kt.Status = @Status)
                AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate)
            ORDER BY kt.TransactionDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            
            SELECT COUNT(*) AS TotalCount
            FROM KioskTransactions kt
            WHERE 1 = 1
                AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
                AND (@FunctionCode IS NULL OR kt.FunctionCode = @FunctionCode)
                AND (@CardNumber IS NULL OR kt.CardNumber LIKE '%' + @CardNumber + '%')
                AND (@MemberId IS NULL OR kt.MemberId = @MemberId)
                AND (@Status IS NULL OR kt.Status = @Status)
                AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate);
        ";

        var parameters = new
        {
            KioskId = string.IsNullOrEmpty(query.Filters?.KioskId) ? (string?)null : query.Filters.KioskId,
            FunctionCode = string.IsNullOrEmpty(query.Filters?.FunctionCode) ? (string?)null : query.Filters.FunctionCode,
            CardNumber = string.IsNullOrEmpty(query.Filters?.CardNumber) ? (string?)null : query.Filters.CardNumber,
            MemberId = string.IsNullOrEmpty(query.Filters?.MemberId) ? (string?)null : query.Filters.MemberId,
            Status = string.IsNullOrEmpty(query.Filters?.Status) ? (string?)null : query.Filters.Status,
            StartDate = string.IsNullOrEmpty(query.Filters?.StartDate) ? (DateTime?)null : DateTime.Parse(query.Filters.StartDate),
            EndDate = string.IsNullOrEmpty(query.Filters?.EndDate) ? (DateTime?)null : DateTime.Parse(query.Filters.EndDate),
            Offset = (query.PageIndex - 1) * query.PageSize,
            PageSize = query.PageSize
        };

        using var multi = await _db.QueryMultipleAsync(sql, parameters);
        var items = (await multi.ReadAsync<KioskTransactionDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();

        return new PagedResult<KioskTransactionDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<List<KioskStatisticsDto>> GetStatisticsAsync(KioskStatisticsQueryDto query)
    {
        var groupByClause = query.GroupBy switch
        {
            "Kiosk" => "kt.KioskId",
            "FunctionCode" => "kt.FunctionCode",
            _ => "CAST(kt.TransactionDate AS DATE)"
        };

        var sql = $@"
            SELECT 
                {groupByClause} AS GroupKey,
                CASE 
                    WHEN '{query.GroupBy}' = 'Date' THEN FORMAT(CAST(kt.TransactionDate AS DATE), 'yyyy-MM-dd')
                    WHEN '{query.GroupBy}' = 'Kiosk' THEN kt.KioskId
                    WHEN '{query.GroupBy}' = 'FunctionCode' THEN kt.FunctionCode
                    ELSE CAST(kt.TransactionDate AS DATE).ToString()
                END AS GroupName,
                COUNT(*) AS TotalCount,
                SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) AS SuccessCount,
                SUM(CASE WHEN kt.Status = 'Failed' THEN 1 ELSE 0 END) AS FailedCount,
                CASE 
                    WHEN COUNT(*) > 0 THEN CAST(SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2))
                    ELSE 0
                END AS SuccessRate
            FROM KioskTransactions kt
            WHERE 1 = 1
                AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate)
                AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
                AND (@FunctionCode IS NULL OR kt.FunctionCode = @FunctionCode)
            GROUP BY {groupByClause}
            ORDER BY {groupByClause};
        ";

        var parameters = new
        {
            StartDate = string.IsNullOrEmpty(query.StartDate) ? (DateTime?)null : DateTime.Parse(query.StartDate),
            EndDate = string.IsNullOrEmpty(query.EndDate) ? (DateTime?)null : DateTime.Parse(query.EndDate),
            KioskId = string.IsNullOrEmpty(query.KioskId) ? (string?)null : query.KioskId,
            FunctionCode = string.IsNullOrEmpty(query.FunctionCode) ? (string?)null : query.FunctionCode
        };

        return (await _db.QueryAsync<KioskStatisticsDto>(sql, parameters)).ToList();
    }

    public async Task<List<KioskFunctionStatisticsDto>> GetFunctionStatisticsAsync(KioskStatisticsQueryDto query)
    {
        var sql = @"
            SELECT 
                kt.FunctionCode,
                CASE kt.FunctionCode
                    WHEN 'O2' THEN '網路線上快速開卡'
                    WHEN 'A1' THEN '確認會員卡號、密碼'
                    WHEN 'C2' THEN '密碼變更'
                    WHEN 'D4' THEN '網路會員點數資訊'
                    WHEN 'D8' THEN '實體會員點數資訊'
                    ELSE kt.FunctionCode
                END AS FunctionCodeName,
                COUNT(*) AS TotalCount,
                SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) AS SuccessCount,
                SUM(CASE WHEN kt.Status = 'Failed' THEN 1 ELSE 0 END) AS FailedCount,
                CASE 
                    WHEN COUNT(*) > 0 THEN CAST(SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2))
                    ELSE 0
                END AS SuccessRate
            FROM KioskTransactions kt
            WHERE 1 = 1
                AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate)
                AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
            GROUP BY kt.FunctionCode
            ORDER BY TotalCount DESC;
        ";

        var parameters = new
        {
            StartDate = string.IsNullOrEmpty(query.StartDate) ? (DateTime?)null : DateTime.Parse(query.StartDate),
            EndDate = string.IsNullOrEmpty(query.EndDate) ? (DateTime?)null : DateTime.Parse(query.EndDate),
            KioskId = string.IsNullOrEmpty(query.KioskId) ? (string?)null : query.KioskId
        };

        return (await _db.QueryAsync<KioskFunctionStatisticsDto>(sql, parameters)).ToList();
    }

    public async Task<List<KioskErrorAnalysisDto>> GetErrorAnalysisAsync(KioskStatisticsQueryDto query)
    {
        var sql = @"
            SELECT 
                kt.ReturnCode,
                kt.ErrorMessage,
                COUNT(*) AS Count,
                CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM KioskTransactions WHERE Status = 'Failed' 
                    AND (@StartDate IS NULL OR CAST(TransactionDate AS DATE) >= @StartDate)
                    AND (@EndDate IS NULL OR CAST(TransactionDate AS DATE) <= @EndDate)
                    AND (@KioskId IS NULL OR KioskId = @KioskId)
                    AND (@FunctionCode IS NULL OR FunctionCode = @FunctionCode)) AS DECIMAL(5,2)) AS Percentage
            FROM KioskTransactions kt
            WHERE kt.Status = 'Failed'
                AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate)
                AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
                AND (@FunctionCode IS NULL OR kt.FunctionCode = @FunctionCode)
            GROUP BY kt.ReturnCode, kt.ErrorMessage
            ORDER BY Count DESC;
        ";

        var parameters = new
        {
            StartDate = string.IsNullOrEmpty(query.StartDate) ? (DateTime?)null : DateTime.Parse(query.StartDate),
            EndDate = string.IsNullOrEmpty(query.EndDate) ? (DateTime?)null : DateTime.Parse(query.EndDate),
            KioskId = string.IsNullOrEmpty(query.KioskId) ? (string?)null : query.KioskId,
            FunctionCode = string.IsNullOrEmpty(query.FunctionCode) ? (string?)null : query.FunctionCode
        };

        return (await _db.QueryAsync<KioskErrorAnalysisDto>(sql, parameters)).ToList();
    }

    public async Task<byte[]> ExportReportAsync(KioskReportExportDto dto)
    {
        var query = new KioskTransactionQueryDto
        {
            PageIndex = 1,
            PageSize = int.MaxValue,
            Filters = dto.Filters
        };

        var transactions = await GetTransactionsAsync(query);

        if (dto.ExportType == "Excel")
        {
            return ExportToExcel(transactions.Items);
        }
        else
        {
            return ExportToPdf(transactions.Items);
        }
    }

    private byte[] ExportToExcel(List<KioskTransactionDto> items)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Kiosk交易報表");

        // 標題
        worksheet.Cells[1, 1].Value = "交易編號";
        worksheet.Cells[1, 2].Value = "Kiosk機號";
        worksheet.Cells[1, 3].Value = "功能代碼";
        worksheet.Cells[1, 4].Value = "功能名稱";
        worksheet.Cells[1, 5].Value = "卡片編號";
        worksheet.Cells[1, 6].Value = "會員編號";
        worksheet.Cells[1, 7].Value = "交易日期時間";
        worksheet.Cells[1, 8].Value = "狀態";
        worksheet.Cells[1, 9].Value = "回應碼";
        worksheet.Cells[1, 10].Value = "錯誤訊息";

        // 資料
        for (int i = 0; i < items.Count; i++)
        {
            var row = i + 2;
            worksheet.Cells[row, 1].Value = items[i].TransactionId;
            worksheet.Cells[row, 2].Value = items[i].KioskId;
            worksheet.Cells[row, 3].Value = items[i].FunctionCode;
            worksheet.Cells[row, 4].Value = items[i].FunctionCodeName;
            worksheet.Cells[row, 5].Value = items[i].CardNumber;
            worksheet.Cells[row, 6].Value = items[i].MemberId;
            worksheet.Cells[row, 7].Value = items[i].TransactionDate;
            worksheet.Cells[row, 8].Value = items[i].Status;
            worksheet.Cells[row, 9].Value = items[i].ReturnCode;
            worksheet.Cells[row, 10].Value = items[i].ErrorMessage;
        }

        // 自動調整欄寬
        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }

    private byte[] ExportToPdf(List<KioskTransactionDto> items)
    {
        // 使用 iTextSharp 或其他 PDF 庫實作
        // 這裡僅為範例，需根據實際需求實作
        throw new NotImplementedException("PDF匯出功能待實作");
    }
}
```

---

## 六、開發時程

### 6.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立 KioskReportStatistics 資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 6.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] KioskReportService 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表匯出功能（Excel、PDF）
- [ ] 單元測試

### 6.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] Kiosk報表查詢頁面開發
- [ ] 統計卡片元件
- [ ] 圖表元件（可選）
- [ ] 表單驗證
- [ ] 元件測試

### 6.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 報表匯出測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 6.5天

---

## 七、注意事項

### 7.1 查詢效能
- 大量資料時需使用分頁查詢
- 需建立適當的索引
- 考慮使用資料快取

### 7.2 報表匯出
- Excel匯出使用 EPPlus
- PDF匯出使用 iTextSharp
- 大量資料時考慮分批匯出

### 7.3 統計資料
- 可考慮建立排程任務定期計算統計資料
- 統計資料可快取以提升查詢效能

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 依Kiosk機號查詢
   - 依功能代碼查詢
   - 依日期範圍查詢
   - 組合條件查詢
   - 分頁查詢

2. **統計測試**
   - 依日期統計
   - 依Kiosk機號統計
   - 依功能代碼統計
   - 統計資料正確性

3. **匯出測試**
   - Excel匯出
   - PDF匯出
   - 大量資料匯出

### 8.2 效能測試
- 大量資料查詢效能（10萬筆以上）
- 統計查詢效能
- 匯出效能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


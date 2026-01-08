# POS - POS報表查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: POS-報表
- **功能名稱**: POS報表查詢
- **功能描述**: 提供POS系統交易資料的報表查詢與列印功能，包含銷售報表、交易統計報表、商品銷售報表等
- **參考舊程式**: 
  - `WEB/IMS_CORE/rsl_pos/handshake.asp` (POS系統介接)
  - `WEB/IMS_CORE/ASP/SYS2000/SYS2A32_PR.ASP` (相關報表)
  - `WEB/IMS_CORE/ASP/SYS2000/SYS2501_PR.ASP` (相關報表)

### 1.2 業務需求
- 提供POS交易統計報表查詢
- 支援依日期區間、店別、POS機號等條件查詢
- 支援銷售金額統計
- 支援交易筆數統計
- 支援商品銷售排行
- 支援報表列印功能
- 支援報表匯出（Excel、PDF）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PosTransactions` (POS交易主檔)
參考 `POS-資料同步作業.md` 的 `PosTransactions` 資料表設計

### 2.2 主要資料表: `PosTransactionDetails` (POS交易明細)
參考 `POS-資料同步作業.md` 的 `PosTransactionDetails` 資料表設計

### 2.3 報表查詢視圖

```sql
-- POS交易報表查詢視圖
CREATE VIEW [dbo].[vw_PosTransactionReport] AS
SELECT 
    pt.Id,
    pt.TransactionId,
    pt.StoreId,
    s.StoreName,
    pt.PosId,
    pt.TransactionDate,
    pt.TransactionType,
    pt.TotalAmount,
    pt.PaymentMethod,
    pt.CustomerId,
    pt.Status,
    COUNT(ptd.Id) AS DetailCount,
    SUM(ptd.Quantity) AS TotalQuantity
FROM PosTransactions pt
LEFT JOIN Stores s ON pt.StoreId = s.StoreId
LEFT JOIN PosTransactionDetails ptd ON pt.TransactionId = ptd.TransactionId
GROUP BY 
    pt.Id, pt.TransactionId, pt.StoreId, s.StoreName, pt.PosId,
    pt.TransactionDate, pt.TransactionType, pt.TotalAmount,
    pt.PaymentMethod, pt.CustomerId, pt.Status;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢POS交易報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/transactions/report`
- **說明**: 查詢POS交易報表資料，支援多條件篩選
- **請求參數**:
  ```json
  {
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "storeId": "",
    "posId": "",
    "transactionType": "",
    "paymentMethod": "",
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
          "id": 1,
          "transactionId": "TXN20240101001",
          "storeId": "S001",
          "storeName": "台北店",
          "posId": "POS001",
          "transactionDate": "2024-01-01T10:00:00",
          "transactionType": "Sale",
          "totalAmount": 1500.00,
          "paymentMethod": "Cash",
          "customerId": "C001",
          "status": "Synced",
          "detailCount": 3,
          "totalQuantity": 5.0
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5,
      "summary": {
        "totalTransactions": 100,
        "totalAmount": 150000.00,
        "totalQuantity": 500.0,
        "averageAmount": 1500.00
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢POS銷售統計報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/sales/statistics`
- **說明**: 查詢POS銷售統計報表
- **請求參數**: 同查詢POS交易報表
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "dailyStatistics": [
        {
          "date": "2024-01-01",
          "transactionCount": 100,
          "totalAmount": 150000.00,
          "averageAmount": 1500.00
        }
      ],
      "storeStatistics": [
        {
          "storeId": "S001",
          "storeName": "台北店",
          "transactionCount": 50,
          "totalAmount": 75000.00
        }
      ],
      "paymentMethodStatistics": [
        {
          "paymentMethod": "Cash",
          "transactionCount": 60,
          "totalAmount": 90000.00
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢POS商品銷售排行
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/products/ranking`
- **說明**: 查詢POS商品銷售排行
- **請求參數**: 同查詢POS交易報表
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "productId": "P001",
        "productName": "商品A",
        "totalQuantity": 100.0,
        "totalAmount": 50000.00,
        "rank": 1
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 匯出POS報表 (Excel)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/report/export/excel`
- **說明**: 匯出POS報表為 Excel 格式
- **請求參數**: 同查詢POS交易報表
- **回應格式**: Excel 檔案流

#### 3.1.5 匯出POS報表 (PDF)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/report/export/pdf`
- **說明**: 匯出POS報表為 PDF 格式
- **請求參數**: 同查詢POS交易報表
- **回應格式**: PDF 檔案流

---

## 四、前端 UI 設計

### 4.1 報表查詢頁面 (`PosReport.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="pos-report">
    <el-card>
      <template #header>
        <span>POS報表查詢</span>
      </template>
      
      <el-form :model="queryForm" ref="queryFormRef" label-width="120px" inline>
        <!-- 日期區間 -->
        <el-form-item label="交易日期">
          <el-date-picker
            v-model="queryForm.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        
        <!-- 店別 -->
        <el-form-item label="店別">
          <el-select 
            v-model="queryForm.storeId" 
            placeholder="請選擇店別"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="item in storeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <!-- POS機號 -->
        <el-form-item label="POS機號">
          <el-input 
            v-model="queryForm.posId" 
            placeholder="請輸入POS機號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <!-- 交易類型 -->
        <el-form-item label="交易類型">
          <el-select 
            v-model="queryForm.transactionType" 
            placeholder="請選擇交易類型"
            clearable
            style="width: 150px"
          >
            <el-option label="銷售" value="Sale" />
            <el-option label="退貨" value="Return" />
            <el-option label="退款" value="Refund" />
          </el-select>
        </el-form-item>
        
        <!-- 付款方式 -->
        <el-form-item label="付款方式">
          <el-select 
            v-model="queryForm.paymentMethod" 
            placeholder="請選擇付款方式"
            clearable
            style="width: 150px"
          >
            <el-option label="現金" value="Cash" />
            <el-option label="信用卡" value="CreditCard" />
            <el-option label="行動支付" value="MobilePayment" />
          </el-select>
        </el-form-item>
        
        <!-- 查詢按鈕 -->
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 報表結果 -->
    <el-card v-if="reportData.items.length > 0" style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>報表結果 (共 {{ reportData.totalCount }} 筆)</span>
          <div>
            <el-button type="success" icon="Printer" @click="handlePrint">列印</el-button>
            <el-button type="warning" icon="Download" @click="handleExportExcel">匯出Excel</el-button>
            <el-button type="warning" icon="Document" @click="handleExportPdf">匯出PDF</el-button>
          </div>
        </div>
      </template>
      
      <!-- 統計摘要 -->
      <el-row :gutter="20" style="margin-bottom: 20px">
        <el-col :span="6">
          <el-statistic title="總交易筆數" :value="reportData.summary.totalTransactions" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="總交易金額" :value="reportData.summary.totalAmount" prefix="NT$" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="總數量" :value="reportData.summary.totalQuantity" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="平均金額" :value="reportData.summary.averageAmount" prefix="NT$" />
        </el-col>
      </el-row>
      
      <el-table 
        :data="reportData.items" 
        border 
        stripe 
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="transactionId" label="交易編號" width="150" />
        <el-table-column prop="storeName" label="店別" width="120" />
        <el-table-column prop="posId" label="POS機號" width="100" />
        <el-table-column prop="transactionDate" label="交易日期時間" width="160" />
        <el-table-column prop="transactionType" label="交易類型" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getTransactionTypeTag(scope.row.transactionType)">
              {{ getTransactionTypeText(scope.row.transactionType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="totalAmount" label="交易金額" width="120" align="right">
          <template #default="scope">
            NT$ {{ scope.row.totalAmount.toLocaleString() }}
          </template>
        </el-table-column>
        <el-table-column prop="paymentMethod" label="付款方式" width="100" />
        <el-table-column prop="detailCount" label="明細筆數" width="100" align="center" />
        <el-table-column prop="totalQuantity" label="總數量" width="100" align="right" />
        <el-table-column prop="status" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getStatusTag(scope.row.status)">
              {{ getStatusText(scope.row.status) }}
            </el-tag>
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
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { posApi } from '@/api/pos'
import { storeApi } from '@/api/store'

// 表單資料
const queryForm = reactive({
  dateRange: [] as string[],
  storeId: '',
  posId: '',
  transactionType: '',
  paymentMethod: ''
})

// 報表資料
const reportData = reactive({
  items: [] as any[],
  totalCount: 0,
  summary: {
    totalTransactions: 0,
    totalAmount: 0,
    totalQuantity: 0,
    averageAmount: 0
  }
})

// 分頁
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

// 店別選項
const storeOptions = ref([])

// 查詢
const handleSearch = async () => {
  try {
    const params = {
      startDate: queryForm.dateRange?.[0] || '',
      endDate: queryForm.dateRange?.[1] || '',
      storeId: queryForm.storeId || undefined,
      posId: queryForm.posId || undefined,
      transactionType: queryForm.transactionType || undefined,
      paymentMethod: queryForm.paymentMethod || undefined,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    
    const response = await posApi.getTransactionReport(params)
    if (response.success) {
      reportData.items = response.data.items
      reportData.totalCount = response.data.totalCount
      reportData.summary = response.data.summary
      pagination.totalCount = response.data.totalCount
    } else {
      ElMessage.error(response.message || '查詢失敗')
    }
  } catch (error: any) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 重置
const handleReset = () => {
  queryForm.dateRange = []
  queryForm.storeId = ''
  queryForm.posId = ''
  queryForm.transactionType = ''
  queryForm.paymentMethod = ''
  reportData.items = []
  reportData.totalCount = 0
  pagination.pageIndex = 1
  pagination.totalCount = 0
}

// 列印
const handlePrint = async () => {
  try {
    const params = {
      startDate: queryForm.dateRange?.[0] || '',
      endDate: queryForm.dateRange?.[1] || '',
      storeId: queryForm.storeId || undefined,
      posId: queryForm.posId || undefined,
      transactionType: queryForm.transactionType || undefined,
      paymentMethod: queryForm.paymentMethod || undefined
    }
    
    const response = await posApi.printReport(params)
    if (response.success) {
      const blob = new Blob([response.data], { type: 'application/pdf' })
      const url = window.URL.createObjectURL(blob)
      window.open(url, '_blank')
    } else {
      ElMessage.error(response.message || '列印失敗')
    }
  } catch (error: any) {
    ElMessage.error('列印失敗：' + error.message)
  }
}

// 匯出Excel
const handleExportExcel = async () => {
  try {
    const params = {
      startDate: queryForm.dateRange?.[0] || '',
      endDate: queryForm.dateRange?.[1] || '',
      storeId: queryForm.storeId || undefined,
      posId: queryForm.posId || undefined,
      transactionType: queryForm.transactionType || undefined,
      paymentMethod: queryForm.paymentMethod || undefined
    }
    
    const response = await posApi.exportExcel(params)
    if (response.success) {
      const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `POS報表_${new Date().toISOString().split('T')[0]}.xlsx`
      link.click()
      ElMessage.success('匯出成功')
    } else {
      ElMessage.error(response.message || '匯出失敗')
    }
  } catch (error: any) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 匯出PDF
const handleExportPdf = async () => {
  try {
    const params = {
      startDate: queryForm.dateRange?.[0] || '',
      endDate: queryForm.dateRange?.[1] || '',
      storeId: queryForm.storeId || undefined,
      posId: queryForm.posId || undefined,
      transactionType: queryForm.transactionType || undefined,
      paymentMethod: queryForm.paymentMethod || undefined
    }
    
    const response = await posApi.exportPdf(params)
    if (response.success) {
      const blob = new Blob([response.data], { type: 'application/pdf' })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `POS報表_${new Date().toISOString().split('T')[0]}.pdf`
      link.click()
      ElMessage.success('匯出成功')
    } else {
      ElMessage.error(response.message || '匯出失敗')
    }
  } catch (error: any) {
    ElMessage.error('匯出失敗：' + error.message)
  }
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

// 交易類型標籤
const getTransactionTypeTag = (type: string) => {
  const typeMap: Record<string, string> = {
    'Sale': 'success',
    'Return': 'warning',
    'Refund': 'danger'
  }
  return typeMap[type] || 'info'
}

// 交易類型文字
const getTransactionTypeText = (type: string) => {
  const typeMap: Record<string, string> = {
    'Sale': '銷售',
    'Return': '退貨',
    'Refund': '退款'
  }
  return typeMap[type] || type
}

// 狀態標籤
const getStatusTag = (status: string) => {
  const statusMap: Record<string, string> = {
    'Pending': 'warning',
    'Synced': 'success',
    'Failed': 'danger'
  }
  return statusMap[status] || 'info'
}

// 狀態文字
const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Pending': '待同步',
    'Synced': '已同步',
    'Failed': '同步失敗'
  }
  return statusMap[status] || status
}

// 初始化
onMounted(async () => {
  // 載入店別選項
  try {
    const response = await storeApi.getList()
    if (response.success) {
      storeOptions.value = response.data.map((item: any) => ({
        value: item.storeId,
        label: item.storeName
      }))
    }
  } catch (error: any) {
    ElMessage.error('載入店別選項失敗：' + error.message)
  }
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`PosReportController.cs`)

```csharp
[ApiController]
[Route("api/v1/pos")]
[Authorize]
public class PosReportController : ControllerBase
{
    private readonly IPosReportService _reportService;
    private readonly ILogger<PosReportController> _logger;

    public PosReportController(
        IPosReportService reportService,
        ILogger<PosReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢POS交易報表
    /// </summary>
    [HttpGet("transactions/report")]
    public async Task<ActionResult<ApiResponse<PosTransactionReportResultDto>>> GetTransactionReport(
        [FromQuery] PosTransactionReportQueryDto query)
    {
        try
        {
            var result = await _reportService.GetTransactionReportAsync(query);
            return Ok(ApiResponse<PosTransactionReportResultDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢POS交易報表失敗");
            return BadRequest(ApiResponse<PosTransactionReportResultDto>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢POS銷售統計報表
    /// </summary>
    [HttpGet("sales/statistics")]
    public async Task<ActionResult<ApiResponse<PosSalesStatisticsDto>>> GetSalesStatistics(
        [FromQuery] PosTransactionReportQueryDto query)
    {
        try
        {
            var result = await _reportService.GetSalesStatisticsAsync(query);
            return Ok(ApiResponse<PosSalesStatisticsDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢POS銷售統計報表失敗");
            return BadRequest(ApiResponse<PosSalesStatisticsDto>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢POS商品銷售排行
    /// </summary>
    [HttpGet("products/ranking")]
    public async Task<ActionResult<ApiResponse<List<PosProductRankingDto>>>> GetProductRanking(
        [FromQuery] PosTransactionReportQueryDto query)
    {
        try
        {
            var result = await _reportService.GetProductRankingAsync(query);
            return Ok(ApiResponse<List<PosProductRankingDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢POS商品銷售排行失敗");
            return BadRequest(ApiResponse<List<PosProductRankingDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 匯出Excel
    /// </summary>
    [HttpGet("report/export/excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] PosTransactionReportQueryDto query)
    {
        try
        {
            var result = await _reportService.ExportExcelAsync(query);
            return File(result, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"POS報表_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出Excel失敗");
            return BadRequest("匯出失敗：" + ex.Message);
        }
    }

    /// <summary>
    /// 匯出PDF
    /// </summary>
    [HttpGet("report/export/pdf")]
    public async Task<IActionResult> ExportPdf([FromQuery] PosTransactionReportQueryDto query)
    {
        try
        {
            var result = await _reportService.ExportPdfAsync(query);
            return File(result, 
                "application/pdf",
                $"POS報表_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出PDF失敗");
            return BadRequest("匯出失敗：" + ex.Message);
        }
    }
}
```

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 建立報表查詢視圖
   - 建立索引

2. **後端 API 開發** (2 天)
   - 實作 Service 層
   - 實作 Controller 層
   - 實作 DTO 定義
   - 實作 Excel/PDF 匯出功能
   - 單元測試

3. **前端 UI 開發** (2 天)
   - 報表查詢頁面
   - 報表列印功能
   - 報表匯出功能
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 5 天

---

## 七、注意事項

### 7.1 資料驗證
- 日期區間必須有效
- 查詢條件需進行參數驗證
- 匯出資料量大時需考慮效能

### 7.2 效能優化
- 使用索引加速查詢
- 大量資料時使用分頁查詢
- 匯出功能使用非同步處理
- 考慮使用 Redis 快取常用查詢結果

### 7.3 報表格式
- Excel 匯出需符合標準格式
- PDF 匯出需支援列印格式
- 報表需包含統計摘要資訊

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 依日期區間查詢
   - 依店別查詢
   - 依POS機號查詢
   - 依交易類型查詢
   - 依付款方式查詢
   - 組合條件查詢
   - 分頁查詢

2. **統計測試**
   - 銷售統計報表
   - 商品銷售排行

3. **匯出測試**
   - Excel 匯出
   - PDF 匯出
   - 大量資料匯出

### 8.2 效能測試
- 大量資料查詢效能（10萬筆以上）
- 匯出功能效能
- 分頁查詢效能

### 8.3 安全性測試
- 權限驗證測試
- 輸入驗證測試
- SQL Injection 測試


# SYSL510 - 加班發放報表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL510-報表
- **功能名稱**: 加班發放報表
- **功能描述**: 提供加班發放資料的報表查詢與列印功能，包含加班發放統計報表、明細報表等
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/SYSL510 - 01.rdlc` (報表定義)
  - `WEB/IMS_CORE/SYSL000/SYSL510.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSL000/SYSL525.js` (報表查詢)
  - `WEB/IMS_CORE/SYSL000/SYSL526.js` (報表列印)
  - `WEB/IMS_CORE/SYSL000/SYSL527.js` (報表匯出)

### 1.2 業務需求
- 提供加班發放統計報表查詢
- 支援依日期區間、部門、員工等條件查詢
- 支援報表列印功能
- 支援報表匯出（Excel、PDF）
- 支援報表預覽功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `OvertimePayments` (加班發放主檔)
參考 `SYSL510-加班發放管理.md` 的 `OvertimePayments` 資料表設計

### 2.2 相關資料表

#### 2.2.1 `Employees` - 員工主檔
- 用於查詢員工基本資料
- 參考: 人力資源管理系統

#### 2.2.2 `Departments` - 部門主檔
- 用於查詢部門基本資料
- 參考: 組織架構管理系統

### 2.3 報表查詢視圖

```sql
-- 加班發放報表查詢視圖
CREATE VIEW [dbo].[vw_OvertimePaymentReport] AS
SELECT 
    op.Id,
    op.PaymentNo,
    op.PaymentDate,
    op.EmployeeId,
    op.EmployeeName,
    op.DepartmentId,
    d.DepartmentName,
    op.OvertimeHours,
    op.OvertimeAmount,
    op.StartDate,
    op.EndDate,
    op.Status,
    op.ApprovedBy,
    op.ApprovedAt,
    op.Notes,
    op.CreatedBy,
    op.CreatedAt,
    op.UpdatedBy,
    op.UpdatedAt
FROM OvertimePayments op
LEFT JOIN Departments d ON op.DepartmentId = d.DepartmentId;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢加班發放報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/overtime-payments/report`
- **說明**: 查詢加班發放報表資料，支援多條件篩選
- **請求參數**:
  ```json
  {
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "departmentId": "",
    "employeeId": "",
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
          "id": 1,
          "paymentNo": "OTP20240101001",
          "paymentDate": "2024-01-15",
          "employeeId": "E001",
          "employeeName": "張三",
          "departmentId": "D001",
          "departmentName": "資訊部",
          "overtimeHours": 10.5,
          "overtimeAmount": 5250.00,
          "startDate": "2024-01-01",
          "endDate": "2024-01-31",
          "status": "Approved",
          "approvedBy": "U001",
          "approvedAt": "2024-01-10T10:00:00"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5,
      "summary": {
        "totalOvertimeHours": 1050.5,
        "totalOvertimeAmount": 525250.00,
        "totalCount": 100
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 匯出加班發放報表 (Excel)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/overtime-payments/report/export/excel`
- **說明**: 匯出加班發放報表為 Excel 格式
- **請求參數**: 同查詢報表
- **回應格式**: Excel 檔案流

#### 3.1.3 匯出加班發放報表 (PDF)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/overtime-payments/report/export/pdf`
- **說明**: 匯出加班發放報表為 PDF 格式
- **請求參數**: 同查詢報表
- **回應格式**: PDF 檔案流

#### 3.1.4 列印加班發放報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/overtime-payments/report/print`
- **說明**: 產生列印用的報表資料
- **請求格式**:
  ```json
  {
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "departmentId": "",
    "employeeId": "",
    "status": "",
    "printFormat": "A4"
  }
  ```
- **回應格式**: PDF 檔案流

---

## 四、前端 UI 設計

### 4.1 報表查詢頁面 (`OvertimePaymentReport.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="overtime-payment-report">
    <el-card>
      <template #header>
        <span>加班發放報表</span>
      </template>
      
      <el-form :model="queryForm" ref="queryFormRef" label-width="120px" inline>
        <!-- 日期區間 -->
        <el-form-item label="發放日期">
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
        
        <!-- 部門 -->
        <el-form-item label="部門">
          <el-select 
            v-model="queryForm.departmentId" 
            placeholder="請選擇部門"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="item in departmentOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <!-- 員工 -->
        <el-form-item label="員工">
          <el-input 
            v-model="queryForm.employeeId" 
            placeholder="請輸入員工編號或姓名"
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
            style="width: 150px"
          >
            <el-option label="草稿" value="Draft" />
            <el-option label="已提交" value="Submitted" />
            <el-option label="已審核" value="Approved" />
            <el-option label="已拒絕" value="Rejected" />
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
          <el-statistic title="總筆數" :value="reportData.summary.totalCount" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="總加班時數" :value="reportData.summary.totalOvertimeHours" suffix="小時" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="總加班金額" :value="reportData.summary.totalOvertimeAmount" prefix="NT$" />
        </el-col>
      </el-row>
      
      <el-table 
        :data="reportData.items" 
        border 
        stripe 
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="paymentNo" label="發放單號" width="150" />
        <el-table-column prop="paymentDate" label="發放日期" width="120" />
        <el-table-column prop="employeeId" label="員工編號" width="120" />
        <el-table-column prop="employeeName" label="員工姓名" width="120" />
        <el-table-column prop="departmentName" label="部門" width="150" />
        <el-table-column prop="overtimeHours" label="加班時數" width="100" align="right">
          <template #default="scope">
            {{ scope.row.overtimeHours.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column prop="overtimeAmount" label="加班金額" width="120" align="right">
          <template #default="scope">
            NT$ {{ scope.row.overtimeAmount.toLocaleString() }}
          </template>
        </el-table-column>
        <el-table-column prop="startDate" label="開始日期" width="120" />
        <el-table-column prop="endDate" label="結束日期" width="120" />
        <el-table-column prop="status" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getStatusType(scope.row.status)">
              {{ getStatusText(scope.row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="approvedBy" label="審核者" width="100" />
        <el-table-column prop="approvedAt" label="審核時間" width="160" />
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
import { overtimePaymentApi } from '@/api/overtimePayment'
import { departmentApi } from '@/api/department'

// 表單資料
const queryForm = reactive({
  dateRange: [] as string[],
  departmentId: '',
  employeeId: '',
  status: ''
})

// 報表資料
const reportData = reactive({
  items: [] as any[],
  totalCount: 0,
  summary: {
    totalOvertimeHours: 0,
    totalOvertimeAmount: 0,
    totalCount: 0
  }
})

// 分頁
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

// 部門選項
const departmentOptions = ref([])

// 查詢
const handleSearch = async () => {
  try {
    const params = {
      startDate: queryForm.dateRange?.[0] || '',
      endDate: queryForm.dateRange?.[1] || '',
      departmentId: queryForm.departmentId || undefined,
      employeeId: queryForm.employeeId || undefined,
      status: queryForm.status || undefined,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    
    const response = await overtimePaymentApi.getReport(params)
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
  queryForm.departmentId = ''
  queryForm.employeeId = ''
  queryForm.status = ''
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
      departmentId: queryForm.departmentId || undefined,
      employeeId: queryForm.employeeId || undefined,
      status: queryForm.status || undefined,
      printFormat: 'A4'
    }
    
    const response = await overtimePaymentApi.printReport(params)
    if (response.success) {
      // 開啟新視窗列印
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
      departmentId: queryForm.departmentId || undefined,
      employeeId: queryForm.employeeId || undefined,
      status: queryForm.status || undefined
    }
    
    const response = await overtimePaymentApi.exportExcel(params)
    if (response.success) {
      // 下載檔案
      const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `加班發放報表_${new Date().toISOString().split('T')[0]}.xlsx`
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
      departmentId: queryForm.departmentId || undefined,
      employeeId: queryForm.employeeId || undefined,
      status: queryForm.status || undefined
    }
    
    const response = await overtimePaymentApi.exportPdf(params)
    if (response.success) {
      // 下載檔案
      const blob = new Blob([response.data], { type: 'application/pdf' })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `加班發放報表_${new Date().toISOString().split('T')[0]}.pdf`
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

// 狀態類型
const getStatusType = (status: string) => {
  const statusMap: Record<string, string> = {
    'Draft': 'info',
    'Submitted': 'warning',
    'Approved': 'success',
    'Rejected': 'danger'
  }
  return statusMap[status] || 'info'
}

// 狀態文字
const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Draft': '草稿',
    'Submitted': '已提交',
    'Approved': '已審核',
    'Rejected': '已拒絕'
  }
  return statusMap[status] || status
}

// 初始化
onMounted(async () => {
  // 載入部門選項
  try {
    const response = await departmentApi.getList()
    if (response.success) {
      departmentOptions.value = response.data.map((item: any) => ({
        value: item.departmentId,
        label: item.departmentName
      }))
    }
  } catch (error: any) {
    ElMessage.error('載入部門選項失敗：' + error.message)
  }
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`OvertimePaymentReportController.cs`)

```csharp
[ApiController]
[Route("api/v1/overtime-payments/report")]
[Authorize]
public class OvertimePaymentReportController : ControllerBase
{
    private readonly IOvertimePaymentReportService _reportService;
    private readonly ILogger<OvertimePaymentReportController> _logger;

    public OvertimePaymentReportController(
        IOvertimePaymentReportService reportService,
        ILogger<OvertimePaymentReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢加班發放報表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<OvertimePaymentReportResultDto>>> GetReport(
        [FromQuery] OvertimePaymentReportQueryDto query)
    {
        try
        {
            var result = await _reportService.GetReportAsync(query);
            return Ok(ApiResponse<OvertimePaymentReportResultDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢加班發放報表失敗");
            return BadRequest(ApiResponse<OvertimePaymentReportResultDto>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 匯出Excel
    /// </summary>
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] OvertimePaymentReportQueryDto query)
    {
        try
        {
            var result = await _reportService.ExportExcelAsync(query);
            return File(result, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"加班發放報表_{DateTime.Now:yyyyMMdd}.xlsx");
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
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf([FromQuery] OvertimePaymentReportQueryDto query)
    {
        try
        {
            var result = await _reportService.ExportPdfAsync(query);
            return File(result, 
                "application/pdf",
                $"加班發放報表_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出PDF失敗");
            return BadRequest("匯出失敗：" + ex.Message);
        }
    }

    /// <summary>
    /// 列印報表
    /// </summary>
    [HttpPost("print")]
    public async Task<IActionResult> PrintReport([FromBody] OvertimePaymentReportPrintDto dto)
    {
        try
        {
            var result = await _reportService.PrintReportAsync(dto);
            return File(result, 
                "application/pdf",
                $"加班發放報表_{DateTime.Now:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "列印報表失敗");
            return BadRequest("列印失敗：" + ex.Message);
        }
    }
}
```

### 5.2 Service (`OvertimePaymentReportService.cs`)

```csharp
public class OvertimePaymentReportService : IOvertimePaymentReportService
{
    private readonly IDbConnection _db;
    private readonly ILogger<OvertimePaymentReportService> _logger;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    public OvertimePaymentReportService(
        IDbConnection db,
        ILogger<OvertimePaymentReportService> logger,
        IExcelExportService excelExportService,
        IPdfExportService pdfExportService)
    {
        _db = db;
        _logger = logger;
        _excelExportService = excelExportService;
        _pdfExportService = pdfExportService;
    }

    public async Task<OvertimePaymentReportResultDto> GetReportAsync(OvertimePaymentReportQueryDto query)
    {
        var sql = @"
            SELECT 
                op.Id,
                op.PaymentNo,
                op.PaymentDate,
                op.EmployeeId,
                op.EmployeeName,
                op.DepartmentId,
                d.DepartmentName,
                op.OvertimeHours,
                op.OvertimeAmount,
                op.StartDate,
                op.EndDate,
                op.Status,
                op.ApprovedBy,
                op.ApprovedAt,
                op.Notes
            FROM vw_OvertimePaymentReport op
            LEFT JOIN Departments d ON op.DepartmentId = d.DepartmentId
            WHERE 1 = 1
                AND (@StartDate IS NULL OR op.PaymentDate >= @StartDate)
                AND (@EndDate IS NULL OR op.PaymentDate <= @EndDate)
                AND (@DepartmentId IS NULL OR op.DepartmentId = @DepartmentId)
                AND (@EmployeeId IS NULL OR op.EmployeeId LIKE '%' + @EmployeeId + '%' OR op.EmployeeName LIKE '%' + @EmployeeId + '%')
                AND (@Status IS NULL OR op.Status = @Status)
            ORDER BY op.PaymentDate DESC, op.PaymentNo
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            
            SELECT 
                COUNT(*) AS TotalCount,
                SUM(op.OvertimeHours) AS TotalOvertimeHours,
                SUM(op.OvertimeAmount) AS TotalOvertimeAmount
            FROM vw_OvertimePaymentReport op
            WHERE 1 = 1
                AND (@StartDate IS NULL OR op.PaymentDate >= @StartDate)
                AND (@EndDate IS NULL OR op.PaymentDate <= @EndDate)
                AND (@DepartmentId IS NULL OR op.DepartmentId = @DepartmentId)
                AND (@EmployeeId IS NULL OR op.EmployeeId LIKE '%' + @EmployeeId + '%' OR op.EmployeeName LIKE '%' + @EmployeeId + '%')
                AND (@Status IS NULL OR op.Status = @Status);
        ";

        var parameters = new
        {
            StartDate = string.IsNullOrEmpty(query.StartDate) ? (DateTime?)null : DateTime.Parse(query.StartDate),
            EndDate = string.IsNullOrEmpty(query.EndDate) ? (DateTime?)null : DateTime.Parse(query.EndDate),
            DepartmentId = string.IsNullOrEmpty(query.DepartmentId) ? (string?)null : query.DepartmentId,
            EmployeeId = string.IsNullOrEmpty(query.EmployeeId) ? (string?)null : query.EmployeeId,
            Status = string.IsNullOrEmpty(query.Status) ? (string?)null : query.Status,
            Offset = (query.PageIndex - 1) * query.PageSize,
            PageSize = query.PageSize
        };

        using var multi = await _db.QueryMultipleAsync(sql, parameters);
        var items = (await multi.ReadAsync<OvertimePaymentReportItemDto>()).ToList();
        var summary = await multi.ReadSingleAsync<OvertimePaymentReportSummaryDto>();

        return new OvertimePaymentReportResultDto
        {
            Items = items,
            TotalCount = summary.TotalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(summary.TotalCount / (double)query.PageSize),
            Summary = summary
        };
    }

    public async Task<byte[]> ExportExcelAsync(OvertimePaymentReportQueryDto query)
    {
        var result = await GetReportAsync(new OvertimePaymentReportQueryDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            DepartmentId = query.DepartmentId,
            EmployeeId = query.EmployeeId,
            Status = query.Status,
            PageIndex = 1,
            PageSize = int.MaxValue
        });

        return await _excelExportService.ExportOvertimePaymentReportAsync(result);
    }

    public async Task<byte[]> ExportPdfAsync(OvertimePaymentReportQueryDto query)
    {
        var result = await GetReportAsync(new OvertimePaymentReportQueryDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            DepartmentId = query.DepartmentId,
            EmployeeId = query.EmployeeId,
            Status = query.Status,
            PageIndex = 1,
            PageSize = int.MaxValue
        });

        return await _pdfExportService.ExportOvertimePaymentReportAsync(result);
    }

    public async Task<byte[]> PrintReportAsync(OvertimePaymentReportPrintDto dto)
    {
        var query = new OvertimePaymentReportQueryDto
        {
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            DepartmentId = dto.DepartmentId,
            EmployeeId = dto.EmployeeId,
            Status = dto.Status,
            PageIndex = 1,
            PageSize = int.MaxValue
        };

        var result = await GetReportAsync(query);
        return await _pdfExportService.PrintOvertimePaymentReportAsync(result, dto.PrintFormat);
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
   - 依部門查詢
   - 依員工查詢
   - 依狀態查詢
   - 組合條件查詢
   - 分頁查詢

2. **匯出測試**
   - Excel 匯出
   - PDF 匯出
   - 大量資料匯出

3. **列印測試**
   - 報表列印
   - 列印格式驗證

### 8.2 效能測試
- 大量資料查詢效能（10萬筆以上）
- 匯出功能效能
- 分頁查詢效能

### 8.3 安全性測試
- 權限驗證測試
- 輸入驗證測試
- SQL Injection 測試

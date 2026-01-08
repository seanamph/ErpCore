# SYSL210 - 員餐卡報表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL210
- **功能名稱**: 員餐卡報表
- **功能描述**: 提供員餐卡報表的查詢、列印、匯出等功能，包含多種報表類型（SYSL21001、SYSL21002、SYSL21003），支援依店別、組織、年月、動作類型等條件查詢員餐卡報表資訊
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/SYSL210.ascx` (查詢/報表頁面)
  - `WEB/IMS_CORE/SYSL000/js/SYSL210.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSL000/style/SYSL210.css` (樣式)
  - `WEB/IMS_CORE/SYSL000/SYSL21001.rdlc` (報表定義 - 報表類型1)
  - `WEB/IMS_CORE/SYSL000/SYSL21002.rdlc` (報表定義 - 報表類型2)
  - `WEB/IMS_CORE/SYSL000/SYSL21003.rdlc` (報表定義 - 報表類型3)
  - `WEB/IMS_CORE/SYSL000/SYSL21001_PR.rdlc` (報表定義 - 報表類型1列印)
  - `WEB/IMS_CORE/SYSL000/SYSL21002_PR.rdlc` (報表定義 - 報表類型2列印)

### 1.2 業務需求
- 支援多種報表類型查詢（SYSL21001、SYSL21002、SYSL21003）
- 支援依店別查詢
- 支援依組織查詢
- 支援依年月查詢
- 支援依動作類型查詢
- 支援依交易單號查詢
- 支援報表列印功能
- 支援報表匯出功能（Excel、PDF）
- 顯示員餐卡報表資訊（卡片表面ID、組織、動作類型、金額等）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `EmployeeMealCards` - 員餐卡主檔 (對應舊系統相關表)
參考: `開發計劃/10-報表管理/02-員餐卡管理/SYSL206-員餐卡管理.md` 的 `EmployeeMealCards` 資料表結構

#### 2.1.2 `EmployeeMealCardTransactions` - 員餐卡交易明細 (對應舊系統相關表)
```sql
CREATE TABLE [dbo].[EmployeeMealCardTransactions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
    [TxnNo] NVARCHAR(50) NOT NULL, -- 交易單號 (TXN_NO)
    [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SITE_ID)
    [CardSurfaceId] NVARCHAR(50) NULL, -- 卡片表面ID (CARD_SURFACE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [ActionType] NVARCHAR(20) NULL, -- 動作類型 (ACTION_TYPE)
    [ActionTypeName] NVARCHAR(100) NULL, -- 動作類型名稱 (ACTION_TYPE_NAME)
    [YearMonth] NVARCHAR(6) NULL, -- 年月 (YYYYMM) (YEAR_MONTH)
    [Amt1] DECIMAL(18,2) NULL DEFAULT 0, -- 金額1 (AMT1)
    [Amt4] DECIMAL(18,2) NULL DEFAULT 0, -- 金額4 (AMT4)
    [Amt5] DECIMAL(18,2) NULL DEFAULT 0, -- 金額5 (AMT5)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    CONSTRAINT [PK_EmployeeMealCardTransactions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_EmployeeMealCardTransactions_TxnNo] UNIQUE ([TxnNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_SiteId] ON [dbo].[EmployeeMealCardTransactions] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_CardSurfaceId] ON [dbo].[EmployeeMealCardTransactions] ([CardSurfaceId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_OrgId] ON [dbo].[EmployeeMealCardTransactions] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_ActionType] ON [dbo].[EmployeeMealCardTransactions] ([ActionType]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_YearMonth] ON [dbo].[EmployeeMealCardTransactions] ([YearMonth]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardTransactions_TxnNo] ON [dbo].[EmployeeMealCardTransactions] ([TxnNo]);
```

### 2.2 相關資料表

#### 2.2.1 `Sites` - 店別主檔
- 參考: `開發計劃/02-基本資料管理/04-組織架構/SYSWB60-庫別資料維護作業.md` 的 `Sites` 資料表結構

#### 2.2.2 `Organizations` - 組織主檔
- 參考: `開發計劃/02-基本資料管理/04-組織架構/SYSWB40-部別資料維護作業.md` 的 `Organizations` 資料表結構

#### 2.2.3 `ActionTypes` - 動作類型主檔
- 參考: `開發計劃/10-報表管理/01-業務報表/SYSL130-業務報表查詢作業.md` 的 `ActionTypes` 資料表結構

### 2.3 報表資料結構

#### 2.3.1 報表資料集結構 (SYSL21001)
```sql
-- 報表查詢結果結構
SELECT 
    SITE_ID,
    SITE_NAME,
    REPORT_NAME,
    ORG_ID,
    CARD_SURFACE_ID,
    TXN_NO,
    ACT1,
    ACT1_NAME,
    AMT1,
    AMT4,
    AMT5
FROM 
    -- 查詢邏輯（需根據實際業務邏輯實作）
```

### 2.4 資料字典

#### EmployeeMealCardTransactions 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| TxnNo | NVARCHAR | 50 | NO | - | 交易單號 | 唯一鍵 |
| SiteId | NVARCHAR | 50 | NO | - | 店別代碼 | 外鍵至Sites表 |
| CardSurfaceId | NVARCHAR | 50 | YES | - | 卡片表面ID | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | 外鍵至Organizations表 |
| ActionType | NVARCHAR | 20 | YES | - | 動作類型 | 外鍵至ActionTypes表 |
| ActionTypeName | NVARCHAR | 100 | YES | - | 動作類型名稱 | - |
| YearMonth | NVARCHAR | 6 | YES | - | 年月 (YYYYMM) | - |
| Amt1 | DECIMAL | 18,2 | YES | 0 | 金額1 | - |
| Amt4 | DECIMAL | 18,2 | YES | 0 | 金額4 | - |
| Amt5 | DECIMAL | 18,2 | YES | 0 | 金額5 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢員餐卡報表資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employee-meal-cards/reports`
- **說明**: 查詢員餐卡報表資料，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TxnNo",
    "sortOrder": "DESC",
    "reportType": "SYSL21001",
    "filters": {
      "siteId": "",
      "orgId": "",
      "yearMonth": "",
      "actionType": "",
      "txnNo": "",
      "cardSurfaceId": ""
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
          "siteId": "SITE001",
          "siteName": "店別名稱",
          "reportName": "員餐卡報表",
          "orgId": "ORG001",
          "cardSurfaceId": "CARD001",
          "txnNo": "TXN001",
          "act1": "ACT001",
          "act1Name": "動作類型名稱",
          "amt1": 1000.00,
          "amt4": 500.00,
          "amt5": 300.00
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

#### 3.1.2 列印員餐卡報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/employee-meal-cards/reports/{reportType}/print`
- **說明**: 列印員餐卡報表
- **路徑參數**:
  - `reportType`: 報表類型 (SYSL21001, SYSL21002, SYSL21003)
- **請求格式**:
  ```json
  {
    "printFormat": "PDF",
    "printParams": {
      "siteId": "SITE001",
      "orgId": "ORG001",
      "yearMonth": "202401",
      "actionType": "ACT001",
      "txnNo": "TXN001"
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
      "fileName": "員餐卡報表-20240101.pdf"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 匯出員餐卡報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/employee-meal-cards/reports/{reportType}/export`
- **說明**: 匯出員餐卡報表（Excel、PDF）
- **路徑參數**:
  - `reportType`: 報表類型 (SYSL21001, SYSL21002, SYSL21003)
- **請求格式**: 同列印員餐卡報表
- **回應格式**: 同列印員餐卡報表

#### 3.1.4 取得報表類型選項
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employee-meal-cards/report-types`
- **說明**: 取得報表類型選項列表
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "SYSL21001",
        "label": "員餐卡報表類型1"
      },
      {
        "value": "SYSL21002",
        "label": "員餐卡報表類型2"
      },
      {
        "value": "SYSL21003",
        "label": "員餐卡報表類型3"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 報表查詢頁面 (`SYSL210Report.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="sysl210-report">
    <el-card>
      <template #header>
        <span>員餐卡報表</span>
      </template>
      
      <!-- 查詢條件 -->
      <el-form :model="queryForm" ref="queryFormRef" label-width="150px" inline>
        <el-form-item label="報表類型" prop="reportType">
          <el-select 
            v-model="queryForm.reportType" 
            placeholder="請選擇報表類型"
            clearable
            style="width: 200px"
            @change="handleReportTypeChange"
          >
            <el-option
              v-for="item in reportTypeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="店別" prop="siteId">
          <el-select 
            v-model="queryForm.siteId" 
            placeholder="請選擇店別"
            clearable
            filterable
            style="width: 200px"
          >
            <el-option
              v-for="item in siteList"
              :key="item.siteId"
              :label="item.siteName"
              :value="item.siteId"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="組織" prop="orgId">
          <el-select 
            v-model="queryForm.orgId" 
            placeholder="請選擇組織"
            clearable
            filterable
            style="width: 200px"
          >
            <el-option
              v-for="item in orgList"
              :key="item.orgId"
              :label="item.orgName"
              :value="item.orgId"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="年月" prop="yearMonth">
          <el-date-picker
            v-model="queryForm.yearMonth"
            type="month"
            placeholder="請選擇年月"
            format="YYYY/MM"
            value-format="YYYYMM"
            style="width: 200px"
          />
        </el-form-item>
        
        <el-form-item label="動作類型" prop="actionType">
          <el-select 
            v-model="queryForm.actionType" 
            placeholder="請選擇動作類型"
            clearable
            filterable
            style="width: 200px"
          >
            <el-option
              v-for="item in actionTypeList"
              :key="item.actionType"
              :label="item.actionTypeName"
              :value="item.actionType"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="交易單號" prop="txnNo">
          <el-input 
            v-model="queryForm.txnNo" 
            placeholder="請輸入交易單號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <el-form-item label="卡片表面ID" prop="cardSurfaceId">
          <el-input 
            v-model="queryForm.cardSurfaceId" 
            placeholder="請輸入卡片表面ID"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 查詢結果 -->
    <el-card v-if="searchResult.length > 0" style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>查詢結果 (共 {{ totalCount }} 筆)</span>
          <div>
            <el-button type="primary" icon="Printer" @click="handlePrint">列印</el-button>
            <el-button type="success" icon="Download" @click="handleExport">匯出</el-button>
          </div>
        </div>
      </template>
      
      <el-table 
        :data="searchResult" 
        border 
        stripe 
        style="width: 100%"
        v-loading="loading"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="siteName" label="店別名稱" width="150" />
        <el-table-column prop="orgId" label="組織代碼" width="120" />
        <el-table-column prop="cardSurfaceId" label="卡片表面ID" width="150" />
        <el-table-column prop="txnNo" label="交易單號" width="150" />
        <el-table-column prop="act1Name" label="動作類型" width="150" />
        <el-table-column prop="amt1" label="金額1" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.amt1) }}
          </template>
        </el-table-column>
        <el-table-column prop="amt4" label="金額4" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.amt4) }}
          </template>
        </el-table-column>
        <el-table-column prop="amt5" label="金額5" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.amt5) }}
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
import { employeeMealCardApi } from '@/api/business.api'

// 表單資料
const queryForm = reactive({
  reportType: '',
  siteId: '',
  orgId: '',
  yearMonth: '',
  actionType: '',
  txnNo: '',
  cardSurfaceId: ''
})

// 資料列表
const reportTypeOptions = ref([])
const siteList = ref([])
const orgList = ref([])
const actionTypeList = ref([])
const searchResult = ref([])
const totalCount = ref(0)
const loading = ref(false)

// 分頁
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

// 報表類型變更
const handleReportTypeChange = (reportType: string) => {
  // 根據報表類型載入對應的查詢條件
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const response = await employeeMealCardApi.getReports({
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      reportType: queryForm.reportType,
      filters: {
        siteId: queryForm.siteId || undefined,
        orgId: queryForm.orgId || undefined,
        yearMonth: queryForm.yearMonth || undefined,
        actionType: queryForm.actionType || undefined,
        txnNo: queryForm.txnNo || undefined,
        cardSurfaceId: queryForm.cardSurfaceId || undefined
      }
    })
    
    if (response.success) {
      searchResult.value = response.data.items
      pagination.totalCount = response.data.totalCount
      totalCount.value = response.data.totalCount
    } else {
      ElMessage.error(response.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.reportType = ''
  queryForm.siteId = ''
  queryForm.orgId = ''
  queryForm.yearMonth = ''
  queryForm.actionType = ''
  queryForm.txnNo = ''
  queryForm.cardSurfaceId = ''
  searchResult.value = []
  pagination.pageIndex = 1
  pagination.totalCount = 0
  totalCount.value = 0
  queryFormRef.value?.resetFields()
}

// 列印
const handlePrint = async () => {
  if (!queryForm.reportType) {
    ElMessage.warning('請選擇報表類型')
    return
  }
  
  try {
    const response = await employeeMealCardApi.printReport(queryForm.reportType, {
      printFormat: 'PDF',
      printParams: {
        siteId: queryForm.siteId || undefined,
        orgId: queryForm.orgId || undefined,
        yearMonth: queryForm.yearMonth || undefined,
        actionType: queryForm.actionType || undefined,
        txnNo: queryForm.txnNo || undefined,
        cardSurfaceId: queryForm.cardSurfaceId || undefined
      }
    })
    
    if (response.success) {
      ElMessage.success('列印成功')
      if (response.data.fileUrl) {
        window.open(response.data.fileUrl, '_blank')
      }
    } else {
      ElMessage.error(response.message || '列印失敗')
    }
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
  }
}

// 匯出
const handleExport = async () => {
  if (!queryForm.reportType) {
    ElMessage.warning('請選擇報表類型')
    return
  }
  
  try {
    const response = await employeeMealCardApi.exportReport(queryForm.reportType, {
      exportFormat: 'Excel',
      exportParams: {
        siteId: queryForm.siteId || undefined,
        orgId: queryForm.orgId || undefined,
        yearMonth: queryForm.yearMonth || undefined,
        actionType: queryForm.actionType || undefined,
        txnNo: queryForm.txnNo || undefined,
        cardSurfaceId: queryForm.cardSurfaceId || undefined
      }
    })
    
    if (response.success) {
      ElMessage.success('匯出成功')
      if (response.data.fileUrl) {
        window.open(response.data.fileUrl, '_blank')
      }
    } else {
      ElMessage.error(response.message || '匯出失敗')
    }
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 格式化貨幣
const formatCurrency = (value: number) => {
  if (value == null) return '0.00'
  return value.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
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
onMounted(async () => {
  // 載入報表類型選項
  try {
    const response = await employeeMealCardApi.getReportTypes()
    if (response.success) {
      reportTypeOptions.value = response.data
    }
  } catch (error) {
    ElMessage.error('載入報表類型選項失敗：' + error.message)
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
  
  // 載入組織列表
  try {
    const response = await orgApi.getOrganizations()
    if (response.success) {
      orgList.value = response.data
    }
  } catch (error) {
    ElMessage.error('載入組織列表失敗：' + error.message)
  }
  
  // 載入動作類型列表
  try {
    const response = await actionTypeApi.getActionTypes()
    if (response.success) {
      actionTypeList.value = response.data
    }
  } catch (error) {
    ElMessage.error('載入動作類型列表失敗：' + error.message)
  }
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`EmployeeMealCardReportController.cs`)

```csharp
[ApiController]
[Route("api/v1/employee-meal-cards")]
[Authorize]
public class EmployeeMealCardReportController : ControllerBase
{
    private readonly IEmployeeMealCardReportService _reportService;
    private readonly ILogger<EmployeeMealCardReportController> _logger;

    public EmployeeMealCardReportController(
        IEmployeeMealCardReportService reportService,
        ILogger<EmployeeMealCardReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢員餐卡報表資料
    /// </summary>
    [HttpGet("reports")]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeMealCardReportDto>>>> GetReports(
        [FromQuery] EmployeeMealCardReportQueryDto query)
    {
        try
        {
            var result = await _reportService.GetReportsAsync(query);
            return Ok(ApiResponse<PagedResult<EmployeeMealCardReportDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢員餐卡報表資料失敗");
            return BadRequest(ApiResponse<PagedResult<EmployeeMealCardReportDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 列印員餐卡報表
    /// </summary>
    [HttpPost("reports/{reportType}/print")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> PrintReport(
        string reportType,
        [FromBody] BusinessReportPrintRequestDto request)
    {
        try
        {
            var result = await _reportService.PrintReportAsync(reportType, request);
            return Ok(ApiResponse<BusinessReportPrintResultDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "列印員餐卡報表失敗");
            return BadRequest(ApiResponse<BusinessReportPrintResultDto>.Error("列印失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 匯出員餐卡報表
    /// </summary>
    [HttpPost("reports/{reportType}/export")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintResultDto>>> ExportReport(
        string reportType,
        [FromBody] BusinessReportExportRequestDto request)
    {
        try
        {
            var result = await _reportService.ExportReportAsync(reportType, request);
            return Ok(ApiResponse<BusinessReportPrintResultDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出員餐卡報表失敗");
            return BadRequest(ApiResponse<BusinessReportPrintResultDto>.Error("匯出失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 取得報表類型選項
    /// </summary>
    [HttpGet("report-types")]
    public async Task<ActionResult<ApiResponse<List<ReportTypeOptionDto>>>> GetReportTypes()
    {
        try
        {
            var result = await _reportService.GetReportTypesAsync();
            return Ok(ApiResponse<List<ReportTypeOptionDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得報表類型選項失敗");
            return BadRequest(ApiResponse<List<ReportTypeOptionDto>>.Error("查詢失敗：" + ex.Message));
        }
    }
}
```

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 確認 EmployeeMealCardTransactions 資料表結構
   - 建立索引
   - 建立外鍵約束

2. **後端 API 開發** (2 天)
   - 實作 Service 層
   - 實作 Controller 層
   - 實作報表生成邏輯
   - 實作檔案處理邏輯
   - 單元測試

3. **前端 UI 開發** (2 天)
   - 報表查詢頁面
   - 報表參數設定
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
- 報表類型必須有效
- 查詢參數必須符合報表要求
- 年月格式必須正確 (YYYYMM)

### 7.2 安全性
- 需驗證使用者權限
- 防止 SQL Injection（使用參數化查詢）
- 檔案下載需驗證權限

### 7.3 效能優化
- 大量資料時使用分頁查詢
- 報表生成使用非同步處理
- 考慮使用 Redis 快取報表資料

### 7.4 錯誤處理
- 記錄報表生成失敗原因
- 提供錯誤訊息給使用者
- 支援重試機制

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 依報表類型查詢
   - 依店別查詢
   - 依組織查詢
   - 依年月查詢
   - 依動作類型查詢
   - 組合條件查詢
   - 分頁查詢

2. **列印測試**
   - 不同報表類型列印測試
   - 不同列印格式測試
   - 參數驗證測試
   - 錯誤處理測試

3. **匯出測試**
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


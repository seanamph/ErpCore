# SYSX140 - 系統擴展報表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSX140
- **功能名稱**: 系統擴展報表
- **功能描述**: 提供系統擴展資料的報表查詢與列印功能，支援多種報表格式、資料統計、報表匯出等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSX000/SYSX140_PR.rdlc` (報表定義)
  - `WEB/IMS_CORE/SYSX000/SYSX140_PR.aspx` (報表頁面，推測)

### 1.2 業務需求
- 提供系統擴展資料的報表查詢
- 支援多種報表格式（PDF、Excel等）
- 支援報表列印
- 支援報表匯出
- 支援資料統計與分析
- 支援自訂報表格式

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SystemExtensions` (對應舊系統 `SYSX_EXTENSION`)

參考: `開發計劃/13-系統擴展/SYSX110-系統擴展資料維護.md`

### 2.2 報表相關資料表

#### 2.2.1 `SystemExtensionReports` - 系統擴展報表記錄
```sql
CREATE TABLE [dbo].[SystemExtensionReports] (
    [ReportId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportName] NVARCHAR(100) NOT NULL, -- 報表名稱
    [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型 (PDF, Excel, Word等)
    [ReportTemplate] NVARCHAR(MAX) NULL, -- 報表範本 (JSON格式)
    [QueryConditions] NVARCHAR(MAX) NULL, -- 查詢條件 (JSON格式)
    [GeneratedDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 產生時間
    [GeneratedBy] NVARCHAR(50) NULL, -- 產生者
    [FileUrl] NVARCHAR(500) NULL, -- 檔案URL
    [FileSize] BIGINT NULL, -- 檔案大小
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'COMPLETED', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemExtensionReports_GeneratedDate] ON [dbo].[SystemExtensionReports] ([GeneratedDate]);
CREATE NONCLUSTERED INDEX [IX_SystemExtensionReports_GeneratedBy] ON [dbo].[SystemExtensionReports] ([GeneratedBy]);
CREATE NONCLUSTERED INDEX [IX_SystemExtensionReports_Status] ON [dbo].[SystemExtensionReports] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions/reports/query`
- **說明**: 查詢系統擴展報表資料
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "extensionId": "",
      "extensionName": "",
      "extensionType": "",
      "status": "",
      "createdDateFrom": "",
      "createdDateTo": ""
    },
    "groupBy": ["extensionType"],
    "orderBy": ["seqNo"]
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
          "extensionId": "EXT001",
          "extensionName": "擴展功能1",
          "extensionType": "TYPE1",
          "extensionValue": "VALUE1",
          "status": "1",
          "createdAt": "2024-01-01T00:00:00"
        }
      ],
      "totalCount": 100,
      "statistics": {
        "totalExtensions": 100,
        "activeExtensions": 80,
        "inactiveExtensions": 20,
        "byType": {
          "TYPE1": 50,
          "TYPE2": 30,
          "TYPE3": 20
        }
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 產生報表 (PDF)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/system-extensions/reports/pdf`
- **說明**: 產生 PDF 格式報表
- **請求格式**:
  ```json
  {
    "reportName": "系統擴展報表",
    "filters": {
      "extensionId": "",
      "extensionName": "",
      "extensionType": "",
      "status": "",
      "createdDateFrom": "",
      "createdDateTo": ""
    },
    "template": "default"
  }
  ```
- **回應格式**: PDF 檔案 (application/pdf)

#### 3.1.3 產生報表 (Excel)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/system-extensions/reports/excel`
- **說明**: 產生 Excel 格式報表
- **請求格式**: 同 PDF 報表
- **回應格式**: Excel 檔案 (application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)

#### 3.1.4 查詢報表記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions/reports`
- **說明**: 查詢已產生的報表記錄
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "reportName": "",
    "reportType": "",
    "startDate": "",
    "endDate": ""
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
          "reportId": 1,
          "reportName": "系統擴展報表",
          "reportType": "PDF",
          "generatedDate": "2024-01-01T00:00:00",
          "generatedBy": "U001",
          "fileUrl": "/api/v1/system-extensions/reports/1/download",
          "fileSize": 1024000,
          "status": "COMPLETED"
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

#### 3.1.5 下載報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions/reports/{reportId}/download`
- **說明**: 下載已產生的報表
- **路徑參數**:
  - `reportId`: 報表ID
- **回應格式**: 報表檔案

#### 3.1.6 刪除報表記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/system-extensions/reports/{reportId}`
- **說明**: 刪除報表記錄
- **路徑參數**:
  - `reportId`: 報表ID
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

### 4.1 報表查詢頁面 (`SYSX140Report.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="sysx140-report">
    <el-card>
      <template #header>
        <span>系統擴展報表</span>
      </template>
      
      <!-- 查詢條件 -->
      <el-form :model="queryForm" ref="queryFormRef" label-width="150px" inline>
        <el-form-item label="擴展功能代碼">
          <el-input 
            v-model="queryForm.extensionId" 
            placeholder="請輸入擴展功能代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <el-form-item label="擴展功能名稱">
          <el-input 
            v-model="queryForm.extensionName" 
            placeholder="請輸入擴展功能名稱"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <el-form-item label="擴展類型">
          <el-select 
            v-model="queryForm.extensionType" 
            placeholder="請選擇擴展類型"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="item in extensionTypeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="狀態">
          <el-select 
            v-model="queryForm.status" 
            placeholder="請選擇狀態"
            clearable
            style="width: 200px"
          >
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="建立日期">
          <el-date-picker
            v-model="queryForm.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            style="width: 240px"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 統計資訊 -->
    <el-card v-if="statistics" style="margin-top: 20px">
      <template #header>
        <span>統計資訊</span>
      </template>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-statistic title="總擴展功能數" :value="statistics.totalExtensions" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="啟用數" :value="statistics.activeExtensions">
            <template #suffix>
              <span style="color: #67c23a">啟用</span>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="停用數" :value="statistics.inactiveExtensions">
            <template #suffix>
              <span style="color: #f56c6c">停用</span>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="類型數" :value="Object.keys(statistics.byType || {}).length" />
        </el-col>
      </el-row>
    </el-card>
    
    <!-- 查詢結果 -->
    <el-card v-if="searchResult.length > 0" style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>查詢結果 (共 {{ totalCount }} 筆)</span>
          <div>
            <el-button type="primary" icon="Document" @click="handleGeneratePDF">產生PDF</el-button>
            <el-button type="success" icon="Document" @click="handleGenerateExcel">產生Excel</el-button>
            <el-button type="info" icon="Printer" @click="handlePrint">列印</el-button>
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
        <el-table-column prop="extensionId" label="擴展功能代碼" width="150" sortable />
        <el-table-column prop="extensionName" label="擴展功能名稱" width="200" sortable />
        <el-table-column prop="extensionType" label="擴展類型" width="150" />
        <el-table-column prop="extensionValue" label="擴展值" width="200" show-overflow-tooltip />
        <el-table-column prop="seqNo" label="排序序號" width="100" align="center" sortable />
        <el-table-column prop="status" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.status === '1' ? 'success' : 'danger'">
              {{ scope.row.status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="建立時間" width="180" align="center">
          <template #default="scope">
            {{ formatDateTime(scope.row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="createdBy" label="建立者" width="120" />
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
    
    <!-- 報表記錄 -->
    <el-card style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>報表記錄</span>
          <el-button type="primary" icon="Refresh" @click="handleRefreshReports">刷新</el-button>
        </div>
      </template>
      
      <el-table 
        :data="reportRecords" 
        border 
        stripe 
        style="width: 100%"
        v-loading="reportLoading"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="reportName" label="報表名稱" width="200" />
        <el-table-column prop="reportType" label="報表類型" width="120" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.reportType === 'PDF' ? 'danger' : 'success'">
              {{ scope.row.reportType }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="generatedDate" label="產生時間" width="180" align="center">
          <template #default="scope">
            {{ formatDateTime(scope.row.generatedDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="generatedBy" label="產生者" width="120" />
        <el-table-column prop="fileSize" label="檔案大小" width="120" align="right">
          <template #default="scope">
            {{ formatFileSize(scope.row.fileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="status" label="狀態" width="120" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.status === 'COMPLETED' ? 'success' : 'warning'">
              {{ scope.row.status === 'COMPLETED' ? '已完成' : '處理中' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="scope">
            <el-button type="primary" link @click="handleDownloadReport(scope.row)">下載</el-button>
            <el-button type="danger" link @click="handleDeleteReport(scope.row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="reportPagination.pageIndex"
        v-model:page-size="reportPagination.pageSize"
        :total="reportPagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleReportSizeChange"
        @current-change="handleReportPageChange"
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
import { systemExtensionApi } from '@/api/systemExtension'

const queryForm = reactive({
  extensionId: '',
  extensionName: '',
  extensionType: '',
  status: '',
  dateRange: [] as string[]
})

const searchResult = ref([])
const totalCount = ref(0)
const statistics = ref(null)
const loading = ref(false)
const reportLoading = ref(false)
const reportRecords = ref([])

const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const reportPagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const extensionTypeOptions = ref([])

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const [startDate, endDate] = queryForm.dateRange || []
    const response = await systemExtensionApi.queryReport({
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      filters: {
        extensionId: queryForm.extensionId || undefined,
        extensionName: queryForm.extensionName || undefined,
        extensionType: queryForm.extensionType || undefined,
        status: queryForm.status || undefined,
        createdDateFrom: startDate || undefined,
        createdDateTo: endDate || undefined
      }
    })
    
    if (response.success) {
      searchResult.value = response.data.items
      pagination.totalCount = response.data.totalCount
      totalCount.value = response.data.totalCount
      statistics.value = response.data.statistics
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
  queryForm.extensionId = ''
  queryForm.extensionName = ''
  queryForm.extensionType = ''
  queryForm.status = ''
  queryForm.dateRange = []
  searchResult.value = []
  pagination.pageIndex = 1
  pagination.totalCount = 0
  totalCount.value = 0
  statistics.value = null
}

// 產生PDF
const handleGeneratePDF = async () => {
  try {
    const [startDate, endDate] = queryForm.dateRange || []
    const response = await systemExtensionApi.generatePDF({
      reportName: '系統擴展報表',
      filters: {
        extensionId: queryForm.extensionId || undefined,
        extensionName: queryForm.extensionName || undefined,
        extensionType: queryForm.extensionType || undefined,
        status: queryForm.status || undefined,
        createdDateFrom: startDate || undefined,
        createdDateTo: endDate || undefined
      },
      template: 'default'
    })
    
    // 下載PDF
    const url = window.URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `系統擴展報表_${new Date().toISOString().split('T')[0]}.pdf`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('PDF報表產生成功')
    handleRefreshReports()
  } catch (error) {
    ElMessage.error('產生PDF失敗：' + error.message)
  }
}

// 產生Excel
const handleGenerateExcel = async () => {
  try {
    const [startDate, endDate] = queryForm.dateRange || []
    const response = await systemExtensionApi.generateExcel({
      reportName: '系統擴展報表',
      filters: {
        extensionId: queryForm.extensionId || undefined,
        extensionName: queryForm.extensionName || undefined,
        extensionType: queryForm.extensionType || undefined,
        status: queryForm.status || undefined,
        createdDateFrom: startDate || undefined,
        createdDateTo: endDate || undefined
      },
      template: 'default'
    })
    
    // 下載Excel
    const url = window.URL.createObjectURL(new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' }))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `系統擴展報表_${new Date().toISOString().split('T')[0]}.xlsx`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('Excel報表產生成功')
    handleRefreshReports()
  } catch (error) {
    ElMessage.error('產生Excel失敗：' + error.message)
  }
}

// 列印
const handlePrint = () => {
  window.print()
}

// 查詢報表記錄
const handleRefreshReports = async () => {
  reportLoading.value = true
  try {
    const response = await systemExtensionApi.getReports({
      pageIndex: reportPagination.pageIndex,
      pageSize: reportPagination.pageSize
    })
    
    if (response.success) {
      reportRecords.value = response.data.items
      reportPagination.totalCount = response.data.totalCount
    } else {
      ElMessage.error(response.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    reportLoading.value = false
  }
}

// 下載報表
const handleDownloadReport = async (row: any) => {
  try {
    const response = await systemExtensionApi.downloadReport(row.reportId)
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', row.reportName)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    ElMessage.success('下載成功')
  } catch (error) {
    ElMessage.error('下載失敗：' + error.message)
  }
}

// 刪除報表
const handleDeleteReport = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除報表「${row.reportName}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const response = await systemExtensionApi.deleteReport(row.reportId)
    if (response.success) {
      ElMessage.success('刪除成功')
      handleRefreshReports()
    } else {
      ElMessage.error(response.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 格式化日期時間
const formatDateTime = (date: string) => {
  return new Date(date).toLocaleString('zh-TW')
}

// 格式化檔案大小
const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
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

const handleReportSizeChange = (size: number) => {
  reportPagination.pageSize = size
  reportPagination.pageIndex = 1
  handleRefreshReports()
}

const handleReportPageChange = (page: number) => {
  reportPagination.pageIndex = page
  handleRefreshReports()
}

// 初始化
onMounted(async () => {
  await handleRefreshReports()
  // 載入擴展類型選項
  // ...
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`SystemExtensionController.cs`)

參考 `開發計劃/13-系統擴展/SYSX110-系統擴展資料維護.md` 的 Controller 實作，並新增報表相關端點。

### 5.2 Service (`SystemExtensionService.cs`)

參考 `開發計劃/13-系統擴展/SYSX110-系統擴展資料維護.md` 的 Service 實作，並新增報表產生邏輯。

### 5.3 報表產生服務

使用 iTextSharp 或 QuestPDF 產生 PDF，使用 EPPlus 或 NPOI 產生 Excel。

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 建立 SystemExtensionReports 資料表
   - 建立索引

2. **後端 API 開發** (2 天)
   - 實作報表查詢 API
   - 實作 PDF 產生 API
   - 實作 Excel 產生 API
   - 實作報表記錄管理 API
   - 單元測試

3. **前端 UI 開發** (2 天)
   - 報表查詢頁面
   - 報表產生功能
   - 報表記錄管理
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 5 天

---

## 七、注意事項

### 7.1 報表格式
- PDF 報表需支援中文顯示
- Excel 報表需支援大量資料
- 報表範本需可自訂

### 7.2 效能優化
- 大量資料報表使用分頁或分批產生
- 報表產生使用非同步處理
- 報表檔案需定期清理

### 7.3 安全性
- 報表下載需驗證使用者權限
- 報表檔案需安全儲存
- 敏感資料需遮罩處理

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 多條件組合查詢
   - 統計資訊顯示
   - 分頁查詢

2. **報表產生測試**
   - PDF 報表產生
   - Excel 報表產生
   - 大量資料報表產生

3. **報表記錄測試**
   - 查詢報表記錄
   - 下載報表
   - 刪除報表記錄

### 8.2 效能測試
- 大量資料報表產生效能
- 報表查詢效能

### 8.3 安全性測試
- 權限驗證測試
- 檔案下載安全性測試


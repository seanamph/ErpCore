<template>
  <div class="sysx140-report">
    <div class="page-header">
      <h1>系統擴展報表 (SYSX140)</h1>
    </div>

    <!-- 查詢條件 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" ref="queryFormRef" label-width="150px" inline>
        <el-form-item label="擴展功能代碼">
          <el-input 
            v-model="queryForm.Filters.ExtensionId" 
            placeholder="請輸入擴展功能代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <el-form-item label="擴展功能名稱">
          <el-input 
            v-model="queryForm.Filters.ExtensionName" 
            placeholder="請輸入擴展功能名稱"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <el-form-item label="擴展類型">
          <el-select 
            v-model="queryForm.Filters.ExtensionType" 
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
            v-model="queryForm.Filters.Status" 
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
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            style="width: 240px"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 統計資訊 -->
    <el-card v-if="statistics" class="statistics-card" shadow="never" style="margin-top: 20px">
      <template #header>
        <span>統計資訊</span>
      </template>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-statistic title="總擴展功能數" :value="statistics.TotalCount" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="啟用數" :value="statistics.ActiveCount">
            <template #suffix>
              <span class="status-active">啟用</span>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="停用數" :value="statistics.InactiveCount">
            <template #suffix>
              <span class="status-inactive">停用</span>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="類型數" :value="Object.keys(statistics.ByType || {}).length" />
        </el-col>
      </el-row>
    </el-card>
    
    <!-- 查詢結果 -->
    <el-card v-if="searchResult.length > 0" class="table-card" shadow="never" style="margin-top: 20px">
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
        <el-table-column prop="ExtensionId" label="擴展功能代碼" width="150" sortable />
        <el-table-column prop="ExtensionName" label="擴展功能名稱" width="200" sortable />
        <el-table-column prop="ExtensionType" label="擴展類型" width="150" />
        <el-table-column prop="ExtensionValue" label="擴展值" width="200" show-overflow-tooltip />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="center" sortable />
        <el-table-column prop="Status" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.Status === '1' ? 'success' : 'danger'">
              {{ scope.row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180" align="center">
          <template #default="scope">
            {{ formatDateTime(scope.row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreatedBy" label="建立者" width="120" />
      </el-table>
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
    
    <!-- 報表記錄 -->
    <el-card class="table-card" shadow="never" style="margin-top: 20px">
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
        <el-table-column prop="ReportName" label="報表名稱" width="200" />
        <el-table-column prop="ReportType" label="報表類型" width="120" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.ReportType === 'PDF' ? 'danger' : 'success'">
              {{ scope.row.ReportType }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="GeneratedDate" label="產生時間" width="180" align="center">
          <template #default="scope">
            {{ formatDateTime(scope.row.GeneratedDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="GeneratedBy" label="產生者" width="120" />
        <el-table-column prop="FileSize" label="檔案大小" width="120" align="right">
          <template #default="scope">
            {{ formatFileSize(scope.row.FileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="120" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.Status === 'COMPLETED' ? 'success' : 'warning'">
              {{ scope.row.Status === 'COMPLETED' ? '已完成' : '處理中' }}
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
        v-model:current-page="reportPagination.PageIndex"
        v-model:page-size="reportPagination.PageSize"
        :total="reportPagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleReportSizeChange"
        @current-change="handleReportPageChange"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { systemExtensionApi } from '@/api/systemExtension'

const queryForm = reactive({
  PageIndex: 1,
  PageSize: 20,
  Filters: {
    ExtensionId: '',
    ExtensionName: '',
    ExtensionType: '',
    Status: '',
    CreatedDateFrom: null,
    CreatedDateTo: null
  }
})

const dateRange = ref([])
const searchResult = ref([])
const totalCount = ref(0)
const statistics = ref(null)
const loading = ref(false)
const reportLoading = ref(false)
const reportRecords = ref([])

const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

const reportPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

const extensionTypeOptions = ref([])

// 日期範圍變更處理
const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.Filters.CreatedDateFrom = dates[0]
    queryForm.Filters.CreatedDateTo = dates[1]
  } else {
    queryForm.Filters.CreatedDateFrom = null
    queryForm.Filters.CreatedDateTo = null
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const response = await systemExtensionApi.queryReport({
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      Filters: {
        ExtensionId: queryForm.Filters.ExtensionId || undefined,
        ExtensionName: queryForm.Filters.ExtensionName || undefined,
        ExtensionType: queryForm.Filters.ExtensionType || undefined,
        Status: queryForm.Filters.Status || undefined,
        CreatedDateFrom: queryForm.Filters.CreatedDateFrom || undefined,
        CreatedDateTo: queryForm.Filters.CreatedDateTo || undefined
      }
    })
    
    if (response.data.Success) {
      searchResult.value = response.data.Data.Items
      pagination.TotalCount = response.data.Data.TotalCount
      totalCount.value = response.data.Data.TotalCount
      statistics.value = response.data.Data.Statistics
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.Message || error.message))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.Filters.ExtensionId = ''
  queryForm.Filters.ExtensionName = ''
  queryForm.Filters.ExtensionType = ''
  queryForm.Filters.Status = ''
  queryForm.Filters.CreatedDateFrom = null
  queryForm.Filters.CreatedDateTo = null
  dateRange.value = []
  searchResult.value = []
  pagination.PageIndex = 1
  pagination.TotalCount = 0
  totalCount.value = 0
  statistics.value = null
}

// 產生PDF
const handleGeneratePDF = async () => {
  try {
    const response = await systemExtensionApi.generatePDF({
      ReportName: '系統擴展報表',
      Filters: {
        ExtensionId: queryForm.Filters.ExtensionId || undefined,
        ExtensionName: queryForm.Filters.ExtensionName || undefined,
        ExtensionType: queryForm.Filters.ExtensionType || undefined,
        Status: queryForm.Filters.Status || undefined,
        CreatedDateFrom: queryForm.Filters.CreatedDateFrom || undefined,
        CreatedDateTo: queryForm.Filters.CreatedDateTo || undefined
      },
      Template: 'default'
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
    ElMessage.error('產生PDF失敗：' + (error.response?.data?.Message || error.message))
  }
}

// 產生Excel
const handleGenerateExcel = async () => {
  try {
    const response = await systemExtensionApi.generateExcel({
      ReportName: '系統擴展報表',
      Filters: {
        ExtensionId: queryForm.Filters.ExtensionId || undefined,
        ExtensionName: queryForm.Filters.ExtensionName || undefined,
        ExtensionType: queryForm.Filters.ExtensionType || undefined,
        Status: queryForm.Filters.Status || undefined,
        CreatedDateFrom: queryForm.Filters.CreatedDateFrom || undefined,
        CreatedDateTo: queryForm.Filters.CreatedDateTo || undefined
      },
      Template: 'default'
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
    ElMessage.error('產生Excel失敗：' + (error.response?.data?.Message || error.message))
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
      PageIndex: reportPagination.PageIndex,
      PageSize: reportPagination.PageSize
    })
    
    if (response.data.Success) {
      reportRecords.value = response.data.Data.Items
      reportPagination.TotalCount = response.data.Data.TotalCount
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.Message || error.message))
  } finally {
    reportLoading.value = false
  }
}

// 下載報表
const handleDownloadReport = async (row) => {
  try {
    const response = await systemExtensionApi.downloadReport(row.ReportId)
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    const fileExtension = row.ReportType === 'PDF' ? 'pdf' : 'xlsx'
    link.setAttribute('download', `${row.ReportName}.${fileExtension}`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    ElMessage.success('下載成功')
  } catch (error) {
    ElMessage.error('下載失敗：' + (error.response?.data?.Message || error.message))
  }
}

// 刪除報表
const handleDeleteReport = async (row) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除報表「${row.ReportName}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const response = await systemExtensionApi.deleteReport(row.ReportId)
    if (response.data.Success) {
      ElMessage.success('刪除成功')
      handleRefreshReports()
    } else {
      ElMessage.error(response.data.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.response?.data?.Message || error.message))
    }
  }
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleString('zh-TW')
}

// 格式化檔案大小
const formatFileSize = (bytes) => {
  if (!bytes || bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

// 分頁變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

const handleReportSizeChange = (size) => {
  reportPagination.PageSize = size
  reportPagination.PageIndex = 1
  handleRefreshReports()
}

const handleReportPageChange = (page) => {
  reportPagination.PageIndex = page
  handleRefreshReports()
}

// 初始化
onMounted(async () => {
  await handleRefreshReports()
  // 載入擴展類型選項（可以從 API 取得）
  // extensionTypeOptions.value = ...
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sysx140-report {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 500;
  color: $text-color-primary; // 確保文字顏色對比度
}

.search-card,
.table-card,
.statistics-card {
  margin-bottom: 20px;
}

// 狀態顏色（使用主題變數，確保對比度）
.status-active {
  color: $success-color; // 使用主題成功色
}

.status-inactive {
  color: $danger-color; // 使用主題危險色
}
</style>


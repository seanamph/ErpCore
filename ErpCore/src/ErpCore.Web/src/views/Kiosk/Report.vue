<template>
  <div class="kiosk-report">
    <div class="page-header">
      <h1>Kiosk報表查詢</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="Kiosk機號">
          <el-input 
            v-model="queryForm.KioskId" 
            placeholder="請輸入Kiosk機號" 
            clearable 
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item label="功能代碼">
          <el-select 
            v-model="queryForm.FunctionCode" 
            placeholder="請選擇功能代碼" 
            clearable
            filterable
            style="width: 150px"
          >
            <el-option label="O2 - 會員點數查詢" value="O2" />
            <el-option label="A1 - 會員密碼驗證" value="A1" />
            <el-option label="C2 - 會員密碼變更" value="C2" />
            <el-option label="D4 - 快速開卡" value="D4" />
            <el-option label="D8 - 資料補登" value="D8" />
          </el-select>
        </el-form-item>
        <el-form-item label="卡片編號">
          <el-input 
            v-model="queryForm.CardNumber" 
            placeholder="請輸入卡片編號" 
            clearable 
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item label="會員編號">
          <el-input 
            v-model="queryForm.MemberId" 
            placeholder="請輸入會員編號" 
            clearable 
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item label="交易日期">
          <el-date-picker
            v-model="queryForm.DateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select 
            v-model="queryForm.Status" 
            placeholder="請選擇狀態" 
            clearable
            style="width: 120px"
          >
            <el-option label="成功" value="Success" />
            <el-option label="失敗" value="Failed" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
          <el-button type="success" icon="Download" @click="handleExport('Excel')">匯出Excel</el-button>
          <el-button type="warning" icon="Document" @click="handleExport('PDF')">匯出PDF</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 統計資訊 -->
    <el-card v-if="summaryData" class="summary-card" shadow="never">
      <el-row :gutter="20">
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">總交易數</div>
            <div class="summary-value">{{ summaryData.TotalCount || 0 }}</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">成功交易數</div>
            <div class="summary-value success">{{ summaryData.SuccessCount || 0 }}</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">失敗交易數</div>
            <div class="summary-value failed">{{ summaryData.FailedCount || 0 }}</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">成功率</div>
            <div class="summary-value">{{ formatPercent(summaryData.SuccessRate || 0) }}%</div>
          </div>
        </el-col>
      </el-row>
    </el-card>

    <!-- 報表類型切換 -->
    <el-card class="tab-card" shadow="never">
      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane label="交易記錄" name="transactions">
          <!-- 交易記錄表格 -->
          <el-table
            :data="transactionData"
            v-loading="loading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="TransactionId" label="交易編號" width="150" />
            <el-table-column prop="KioskId" label="Kiosk機號" width="120" />
            <el-table-column prop="FunctionCode" label="功能代碼" width="100" align="center">
              <template #default="{ row }">
                <el-tag size="small">{{ row.FunctionCode }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="FunctionCodeName" label="功能名稱" width="150" />
            <el-table-column prop="CardNumber" label="卡片編號" width="150" />
            <el-table-column prop="MemberId" label="會員編號" width="120" />
            <el-table-column prop="TransactionDate" label="交易日期時間" width="160">
              <template #default="{ row }">
                {{ formatDateTime(row.TransactionDate) }}
              </template>
            </el-table-column>
            <el-table-column prop="Status" label="狀態" width="100" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'Success' ? 'success' : 'danger'" size="small">
                  {{ row.Status === 'Success' ? '成功' : '失敗' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="ReturnCode" label="回應碼" width="100" align="center" />
            <el-table-column prop="ErrorMessage" label="錯誤訊息" min-width="200" show-overflow-tooltip />
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
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>

        <el-tab-pane label="交易統計" name="statistics">
          <!-- 交易統計表格 -->
          <el-table
            :data="statisticsData"
            v-loading="statisticsLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="GroupKey" label="分組鍵" width="150" />
            <el-table-column prop="GroupName" label="分組名稱" width="200" />
            <el-table-column prop="TotalCount" label="總交易數" width="120" align="right" />
            <el-table-column prop="SuccessCount" label="成功數" width="120" align="right">
              <template #default="{ row }">
                <span class="success-text">{{ row.SuccessCount }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="FailedCount" label="失敗數" width="120" align="right">
              <template #default="{ row }">
                <span class="failed-text">{{ row.FailedCount }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="SuccessRate" label="成功率" width="120" align="right">
              <template #default="{ row }">
                {{ formatPercent(row.SuccessRate) }}%
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <el-tab-pane label="功能代碼統計" name="function-statistics">
          <!-- 功能代碼統計表格 -->
          <el-table
            :data="functionStatisticsData"
            v-loading="functionStatisticsLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="FunctionCode" label="功能代碼" width="120" align="center">
              <template #default="{ row }">
                <el-tag size="small">{{ row.FunctionCode }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="FunctionCodeName" label="功能名稱" width="200" />
            <el-table-column prop="TotalCount" label="總交易數" width="120" align="right" />
            <el-table-column prop="SuccessCount" label="成功數" width="120" align="right">
              <template #default="{ row }">
                <span class="success-text">{{ row.SuccessCount }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="FailedCount" label="失敗數" width="120" align="right">
              <template #default="{ row }">
                <span class="failed-text">{{ row.FailedCount }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="SuccessRate" label="成功率" width="120" align="right">
              <template #default="{ row }">
                {{ formatPercent(row.SuccessRate) }}%
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <el-tab-pane label="錯誤分析" name="error-analysis">
          <!-- 錯誤分析表格 -->
          <el-table
            :data="errorAnalysisData"
            v-loading="errorAnalysisLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="ReturnCode" label="回應碼" width="120" align="center" />
            <el-table-column prop="ErrorMessage" label="錯誤訊息" min-width="300" show-overflow-tooltip />
            <el-table-column prop="Count" label="發生次數" width="120" align="right" />
            <el-table-column prop="Percentage" label="占比" width="120" align="right">
              <template #default="{ row }">
                {{ formatPercent(row.Percentage) }}%
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { kioskApi } from '@/api/kiosk'

// 查詢表單
const queryForm = reactive({
  KioskId: '',
  FunctionCode: '',
  CardNumber: '',
  MemberId: '',
  DateRange: [],
  Status: ''
})

// 表格資料
const transactionData = ref([])
const statisticsData = ref([])
const functionStatisticsData = ref([])
const errorAnalysisData = ref([])
const loading = ref(false)
const statisticsLoading = ref(false)
const functionStatisticsLoading = ref(false)
const errorAnalysisLoading = ref(false)
const summaryData = ref(null)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 報表類型
const activeTab = ref('transactions')

// 查詢交易記錄
const handleSearch = async () => {
  if (!queryForm.DateRange || queryForm.DateRange.length !== 2) {
    ElMessage.warning('請選擇交易日期')
    return
  }

  try {
    loading.value = true
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: 'TransactionDate',
      SortOrder: 'DESC',
      Filters: {
        KioskId: queryForm.KioskId || undefined,
        FunctionCode: queryForm.FunctionCode || undefined,
        CardNumber: queryForm.CardNumber || undefined,
        MemberId: queryForm.MemberId || undefined,
        Status: queryForm.Status || undefined,
        StartDate: queryForm.DateRange[0],
        EndDate: queryForm.DateRange[1]
      }
    }
    
    const response = await kioskApi.getTransactions(params)
    
    if (response.data.success) {
      transactionData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
      
      // 計算統計資訊
      const total = transactionData.value.length
      const success = transactionData.value.filter(item => item.Status === 'Success').length
      const failed = total - success
      summaryData.value = {
        TotalCount: pagination.TotalCount,
        SuccessCount: success,
        FailedCount: failed,
        SuccessRate: total > 0 ? (success / total) * 100 : 0
      }
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 查詢統計資料
const loadStatistics = async () => {
  if (!queryForm.DateRange || queryForm.DateRange.length !== 2) {
    return
  }

  try {
    statisticsLoading.value = true
    const params = {
      StartDate: queryForm.DateRange[0],
      EndDate: queryForm.DateRange[1],
      KioskId: queryForm.KioskId || undefined,
      FunctionCode: queryForm.FunctionCode || undefined,
      GroupBy: 'Date'
    }
    
    const response = await kioskApi.getStatistics(params)
    
    if (response.data.success) {
      statisticsData.value = response.data.data || []
    }
  } catch (error) {
    console.error('查詢統計資料失敗：', error)
  } finally {
    statisticsLoading.value = false
  }
}

// 查詢功能代碼統計
const loadFunctionStatistics = async () => {
  if (!queryForm.DateRange || queryForm.DateRange.length !== 2) {
    return
  }

  try {
    functionStatisticsLoading.value = true
    const params = {
      StartDate: queryForm.DateRange[0],
      EndDate: queryForm.DateRange[1],
      KioskId: queryForm.KioskId || undefined
    }
    
    const response = await kioskApi.getFunctionStatistics(params)
    
    if (response.data.success) {
      functionStatisticsData.value = response.data.data || []
    }
  } catch (error) {
    console.error('查詢功能代碼統計失敗：', error)
  } finally {
    functionStatisticsLoading.value = false
  }
}

// 查詢錯誤分析
const loadErrorAnalysis = async () => {
  if (!queryForm.DateRange || queryForm.DateRange.length !== 2) {
    return
  }

  try {
    errorAnalysisLoading.value = true
    const params = {
      StartDate: queryForm.DateRange[0],
      EndDate: queryForm.DateRange[1],
      KioskId: queryForm.KioskId || undefined,
      FunctionCode: queryForm.FunctionCode || undefined
    }
    
    const response = await kioskApi.getErrorAnalysis(params)
    
    if (response.data.success) {
      errorAnalysisData.value = response.data.data || []
    }
  } catch (error) {
    console.error('查詢錯誤分析失敗：', error)
  } finally {
    errorAnalysisLoading.value = false
  }
}

// 標籤切換
const handleTabChange = (tabName) => {
  if (tabName === 'statistics') {
    loadStatistics()
  } else if (tabName === 'function-statistics') {
    loadFunctionStatistics()
  } else if (tabName === 'error-analysis') {
    loadErrorAnalysis()
  }
}

// 重置
const handleReset = () => {
  queryForm.KioskId = ''
  queryForm.FunctionCode = ''
  queryForm.CardNumber = ''
  queryForm.MemberId = ''
  queryForm.DateRange = []
  queryForm.Status = ''
  pagination.PageIndex = 1
  transactionData.value = []
  statisticsData.value = []
  functionStatisticsData.value = []
  errorAnalysisData.value = []
  pagination.TotalCount = 0
  summaryData.value = null
}

// 匯出
const handleExport = async (exportType) => {
  if (!queryForm.DateRange || queryForm.DateRange.length !== 2) {
    ElMessage.warning('請選擇交易日期')
    return
  }

  try {
    const data = {
      ExportType: exportType,
      Filters: {
        KioskId: queryForm.KioskId || undefined,
        FunctionCode: queryForm.FunctionCode || undefined,
        CardNumber: queryForm.CardNumber || undefined,
        MemberId: queryForm.MemberId || undefined,
        Status: queryForm.Status || undefined,
        StartDate: queryForm.DateRange[0],
        EndDate: queryForm.DateRange[1]
      }
    }
    
    const response = await kioskApi.exportReport(data)
    
    // 下載檔案
    const blob = new Blob([response.data])
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `Kiosk報表_${new Date().toISOString().split('T')[0]}.${exportType === 'Excel' ? 'xlsx' : 'pdf'}`
    link.click()
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗：' + (error.message || '未知錯誤'))
  }
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

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return d.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 格式化百分比
const formatPercent = (num) => {
  if (num == null) return '0.00'
  return Number(num).toFixed(2)
}
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.kiosk-report {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: var(--text-color-primary);
    }
  }

  .search-card {
    margin-bottom: 20px;
  }

  .summary-card {
    margin-bottom: 20px;
    
    .summary-item {
      text-align: center;
      
      .summary-label {
        font-size: 14px;
        color: var(--text-color-secondary);
        margin-bottom: 8px;
      }
      
      .summary-value {
        font-size: 20px;
        font-weight: 500;
        color: var(--primary-color);
        
        &.success {
          color: var(--success-color);
        }
        
        &.failed {
          color: var(--danger-color);
        }
      }
    }
  }

  .tab-card {
    margin-bottom: 20px;
  }

  .success-text {
    color: var(--success-color);
    font-weight: 500;
  }

  .failed-text {
    color: var(--danger-color);
    font-weight: 500;
  }
}
</style>


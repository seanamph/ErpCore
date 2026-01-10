<template>
  <div class="pos-report">
    <div class="page-header">
      <h1>POS報表查詢</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表類型">
          <el-select 
            v-model="queryForm.ReportType" 
            placeholder="請選擇報表類型" 
            style="width: 200px"
          >
            <el-option label="交易報表" value="Transaction" />
            <el-option label="銷售統計" value="SalesStatistics" />
            <el-option label="商品銷售排行" value="ProductSalesRanking" />
          </el-select>
        </el-form-item>
        <el-form-item label="日期區間">
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
        <el-form-item label="店別">
          <el-select 
            v-model="queryForm.StoreId" 
            placeholder="請選擇店別" 
            clearable
            filterable
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
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
          <el-button type="success" icon="Download" @click="handleExport('excel')">匯出Excel</el-button>
          <el-button type="warning" icon="Document" @click="handleExport('pdf')">匯出PDF</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 統計資訊 -->
    <el-card v-if="summaryData" class="summary-card" shadow="never">
      <el-row :gutter="20">
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">總交易筆數</div>
            <div class="summary-value">{{ summaryData.TotalCount || 0 }}</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">總交易金額</div>
            <div class="summary-value">NT$ {{ formatNumber(summaryData.TotalAmount || 0) }}</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">平均交易金額</div>
            <div class="summary-value">NT$ {{ formatNumber(summaryData.AverageAmount || 0) }}</div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="summary-item">
            <div class="summary-label">總商品數量</div>
            <div class="summary-value">{{ summaryData.TotalQuantity || 0 }}</div>
          </div>
        </el-col>
      </el-row>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column 
          v-if="queryForm.ReportType === 'Transaction'"
          prop="TransactionId" 
          label="交易編號" 
          width="150" 
        />
        <el-table-column 
          v-if="queryForm.ReportType === 'ProductSalesRanking'"
          prop="ProductId" 
          label="商品編號" 
          width="120" 
        />
        <el-table-column prop="StoreName" label="店別" width="120" />
        <el-table-column 
          v-if="queryForm.ReportType === 'Transaction'"
          prop="TransactionDate" 
          label="交易日期" 
          width="120"
        >
          <template #default="{ row }">
            {{ formatDate(row.TransactionDate) }}
          </template>
        </el-table-column>
        <el-table-column 
          v-if="queryForm.ReportType === 'ProductSalesRanking'"
          prop="ProductName" 
          label="商品名稱" 
          min-width="200" 
        />
        <el-table-column 
          v-if="queryForm.ReportType === 'ProductSalesRanking'"
          prop="Quantity" 
          label="銷售數量" 
          width="120" 
          align="right"
        >
          <template #default="{ row }">
            {{ formatNumber(row.Quantity, 3) }}
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            NT$ {{ formatNumber(row.Amount || row.TotalAmount) }}
          </template>
        </el-table-column>
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
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { posApi } from '@/api/pos'
import { warehousesApi } from '@/api/warehouses'

// 查詢表單
const queryForm = reactive({
  ReportType: 'Transaction',
  DateRange: [],
  StoreId: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)
const summaryData = ref(null)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 店別選項
const storeOptions = ref([])

// 查詢
const handleSearch = async () => {
  if (!queryForm.ReportType) {
    ElMessage.warning('請選擇報表類型')
    return
  }
  
  if (!queryForm.DateRange || queryForm.DateRange.length !== 2) {
    ElMessage.warning('請選擇日期區間')
    return
  }

  try {
    loading.value = true
    const params = {
      StartDate: queryForm.DateRange[0],
      EndDate: queryForm.DateRange[1],
      StoreId: queryForm.StoreId || undefined,
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize
    }
    
    let response
    if (queryForm.ReportType === 'Transaction') {
      response = await posApi.getTransactionReport(params)
    } else if (queryForm.ReportType === 'SalesStatistics') {
      response = await posApi.getSalesStatisticsReport(params)
    } else if (queryForm.ReportType === 'ProductSalesRanking') {
      response = await posApi.getProductSalesRanking(params)
    }
    
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
      summaryData.value = response.data.data.summary || null
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ReportType = 'Transaction'
  queryForm.DateRange = []
  queryForm.StoreId = ''
  pagination.PageIndex = 1
  tableData.value = []
  pagination.TotalCount = 0
  summaryData.value = null
}

// 匯出
const handleExport = async (format) => {
  if (!queryForm.ReportType) {
    ElMessage.warning('請選擇報表類型')
    return
  }
  
  if (!queryForm.DateRange || queryForm.DateRange.length !== 2) {
    ElMessage.warning('請選擇日期區間')
    return
  }

  try {
    const params = {
      ReportType: queryForm.ReportType,
      StartDate: queryForm.DateRange[0],
      EndDate: queryForm.DateRange[1],
      StoreId: queryForm.StoreId || undefined
    }
    
    const response = await posApi.exportReport(params, format)
    
    // 下載檔案
    const blob = new Blob([response.data])
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `POS報表_${new Date().toISOString().split('T')[0]}.${format === 'excel' ? 'xlsx' : 'pdf'}`
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

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return d.toLocaleDateString('zh-TW')
}

// 格式化數字
const formatNumber = (num, decimals = 2) => {
  if (num == null) return '0'
  return Number(num).toLocaleString('zh-TW', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals
  })
}

// 載入店別選項
const loadStoreOptions = async () => {
  try {
    const response = await warehousesApi.getWarehouses({ PageSize: 1000 })
    if (response.data.success) {
      storeOptions.value = (response.data.data.items || []).map(item => ({
        value: item.WarehouseId,
        label: item.WarehouseName
      }))
    }
  } catch (error) {
    console.error('載入店別選項失敗：', error)
  }
}

// 初始化
onMounted(() => {
  loadStoreOptions()
})
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.pos-report {
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
      }
    }
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>


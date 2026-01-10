<template>
  <div class="sales-report-query">
    <div class="page-header">
      <h1>報表查詢作業 (SYSG610-SYSG640)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇" clearable>
            <el-option label="明細報表" value="DETAIL" />
            <el-option label="彙總報表" value="SUMMARY" />
          </el-select>
        </el-form-item>
        <el-form-item label="銷售日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="客戶代碼">
          <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶代碼" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
        </el-form-item>
      </el-form>
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
        <el-table-column prop="OrderId" label="銷售單號" width="150" />
        <el-table-column prop="OrderDate" label="銷售日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CustomerId" label="客戶代碼" width="120" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
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
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { salesReportQueryApi } from '@/api/invoiceSales'

// 查詢表單
const queryForm = reactive({
  ReportType: 'DETAIL',
  OrderDateFrom: '',
  OrderDateTo: '',
  CustomerId: ''
})
const dateRange = ref(null)

// 監聽日期範圍變化
watch(dateRange, (val) => {
  if (val && val.length === 2) {
    queryForm.OrderDateFrom = val[0]
    queryForm.OrderDateTo = val[1]
  } else {
    queryForm.OrderDateFrom = ''
    queryForm.OrderDateTo = ''
  }
})

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      OrderDateFrom: queryForm.OrderDateFrom || undefined,
      OrderDateTo: queryForm.OrderDateTo || undefined,
      CustomerId: queryForm.CustomerId || undefined
    }
    let response
    if (queryForm.ReportType === 'DETAIL') {
      response = await salesReportQueryApi.queryDetailReport(params)
    } else {
      response = await salesReportQueryApi.querySummaryReport(params)
    }
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ReportType = 'DETAIL'
  queryForm.OrderDateFrom = ''
  queryForm.OrderDateTo = ''
  queryForm.CustomerId = ''
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// 匯出
const handleExport = () => {
  ElMessage.info('匯出功能開發中')
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

// 格式化貨幣
const formatCurrency = (amount) => {
  if (amount == null) return '0'
  return new Intl.NumberFormat('zh-TW', { style: 'currency', currency: 'TWD' }).format(amount)
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sales-report-query {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>


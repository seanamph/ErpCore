<template>
  <div class="sales-query">
    <div class="page-header">
      <h1>銷售查詢作業 (SYSG510-SYSG5D0)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="銷售單號">
          <el-input v-model="queryForm.OrderId" placeholder="請輸入銷售單號" clearable />
        </el-form-item>
        <el-form-item label="客戶代碼">
          <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶代碼" clearable />
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
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="草稿" value="D" />
            <el-option label="已送出" value="S" />
            <el-option label="已審核" value="A" />
            <el-option label="已出貨" value="O" />
            <el-option label="已取消" value="X" />
            <el-option label="已結案" value="C" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="info" @click="handleStatistics">統計</el-button>
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
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalQty" label="總數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
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

    <!-- 統計對話框 -->
    <el-dialog
      v-model="statisticsDialogVisible"
      title="銷售統計"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-descriptions :column="2" border v-if="statisticsData">
        <el-descriptions-item label="總銷售單數">{{ statisticsData.TotalOrders }}</el-descriptions-item>
        <el-descriptions-item label="總銷售金額">{{ formatCurrency(statisticsData.TotalAmount) }}</el-descriptions-item>
        <el-descriptions-item label="總銷售數量">{{ formatNumber(statisticsData.TotalQty) }}</el-descriptions-item>
        <el-descriptions-item label="平均單價">{{ formatCurrency(statisticsData.AvgPrice) }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="statisticsDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { salesQueryApi } from '@/api/invoiceSales'

// 查詢表單
const queryForm = reactive({
  OrderId: '',
  CustomerId: '',
  OrderDateFrom: '',
  OrderDateTo: '',
  Status: ''
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

// 統計對話框
const statisticsDialogVisible = ref(false)
const statisticsData = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      OrderId: queryForm.OrderId || undefined,
      CustomerId: queryForm.CustomerId || undefined,
      OrderDateFrom: queryForm.OrderDateFrom || undefined,
      OrderDateTo: queryForm.OrderDateTo || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await salesQueryApi.query(params)
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
  queryForm.OrderId = ''
  queryForm.CustomerId = ''
  queryForm.OrderDateFrom = ''
  queryForm.OrderDateTo = ''
  queryForm.Status = ''
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// 查看
const handleView = (row) => {
  // 導航到詳情頁面或顯示詳情對話框
  ElMessage.info('查看詳情功能開發中')
}

// 統計
const handleStatistics = async () => {
  try {
    const params = {
      OrderId: queryForm.OrderId || undefined,
      CustomerId: queryForm.CustomerId || undefined,
      OrderDateFrom: queryForm.OrderDateFrom || undefined,
      OrderDateTo: queryForm.OrderDateTo || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await salesQueryApi.getStatistics(params)
    if (response.data?.success) {
      statisticsData.value = response.data.data
      statisticsDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢統計失敗')
    }
  } catch (error) {
    ElMessage.error('查詢統計失敗：' + error.message)
  }
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

// 格式化數字
const formatNumber = (num) => {
  if (num == null) return '0'
  return new Intl.NumberFormat('zh-TW').format(num)
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'D': 'info',
    'S': 'warning',
    'A': 'success',
    'O': 'success',
    'X': 'danger',
    'C': 'info'
  }
  return types[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const texts = {
    'D': '草稿',
    'S': '已送出',
    'A': '已審核',
    'O': '已出貨',
    'X': '已取消',
    'C': '已結案'
  }
  return texts[status] || status
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sales-query {
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


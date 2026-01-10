<template>
  <div class="pos-transaction">
    <div class="page-header">
      <h1>POS交易查詢</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="交易編號">
          <el-input 
            v-model="queryForm.TransactionId" 
            placeholder="請輸入交易編號" 
            clearable 
            style="width: 200px"
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
        <el-form-item label="POS機號">
          <el-input 
            v-model="queryForm.PosId" 
            placeholder="請輸入POS機號" 
            clearable 
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item label="交易類型">
          <el-select 
            v-model="queryForm.TransactionType" 
            placeholder="請選擇交易類型" 
            clearable
            style="width: 150px"
          >
            <el-option label="銷售" value="Sale" />
            <el-option label="退貨" value="Return" />
            <el-option label="退款" value="Refund" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select 
            v-model="queryForm.Status" 
            placeholder="請選擇狀態" 
            clearable
            style="width: 150px"
          >
            <el-option label="待同步" value="Pending" />
            <el-option label="已同步" value="Synced" />
            <el-option label="同步失敗" value="Failed" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
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
        @row-click="handleRowClick"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="TransactionId" label="交易編號" width="150" />
        <el-table-column prop="StoreName" label="店別" width="120" />
        <el-table-column prop="PosId" label="POS機號" width="100" />
        <el-table-column prop="TransactionDate" label="交易日期時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.TransactionDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransactionType" label="交易類型" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getTransactionTypeTag(row.TransactionType)">
              {{ getTransactionTypeText(row.TransactionType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="交易金額" width="120" align="right">
          <template #default="{ row }">
            NT$ {{ formatNumber(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="PaymentMethod" label="付款方式" width="100" />
        <el-table-column prop="Status" label="狀態" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusTag(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click.stop="handleViewDetail(row)">
              查看明細
            </el-button>
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

    <!-- 交易明細對話框 -->
    <el-dialog 
      v-model="detailDialogVisible" 
      title="交易明細" 
      width="900px"
    >
      <el-table 
        :data="transactionDetails" 
        border 
        stripe 
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="LineNo" label="行號" width="80" align="center" />
        <el-table-column prop="ProductId" label="商品編號" width="120" />
        <el-table-column prop="ProductName" label="商品名稱" min-width="200" />
        <el-table-column prop="Quantity" label="數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.Quantity, 3) }}
          </template>
        </el-table-column>
        <el-table-column prop="UnitPrice" label="單價" width="120" align="right">
          <template #default="{ row }">
            NT$ {{ formatNumber(row.UnitPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="Discount" label="折扣" width="100" align="right">
          <template #default="{ row }">
            NT$ {{ formatNumber(row.Discount || 0) }}
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            NT$ {{ formatNumber(row.Amount) }}
          </template>
        </el-table-column>
      </el-table>
      
      <template #footer>
        <el-button @click="detailDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { posApi } from '@/api/pos'
import { warehousesApi } from '@/api/warehouses'

// 查詢表單
const queryForm = reactive({
  TransactionId: '',
  DateRange: [],
  StoreId: '',
  PosId: '',
  TransactionType: '',
  Status: ''
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

// 交易明細
const detailDialogVisible = ref(false)
const transactionDetails = ref([])
const currentTransaction = ref(null)

// 店別選項
const storeOptions = ref([])

// 查詢
const handleSearch = async () => {
  try {
    loading.value = true
    const params = {
      TransactionId: queryForm.TransactionId || undefined,
      StartDate: queryForm.DateRange?.[0] || undefined,
      EndDate: queryForm.DateRange?.[1] || undefined,
      StoreId: queryForm.StoreId || undefined,
      PosId: queryForm.PosId || undefined,
      TransactionType: queryForm.TransactionType || undefined,
      Status: queryForm.Status || undefined,
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: 'TransactionDate',
      SortOrder: 'DESC'
    }
    
    const response = await posApi.getTransactions(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
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
  queryForm.TransactionId = ''
  queryForm.DateRange = []
  queryForm.StoreId = ''
  queryForm.PosId = ''
  queryForm.TransactionType = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  tableData.value = []
  pagination.TotalCount = 0
}

// 查看明細
const handleViewDetail = async (row) => {
  try {
    currentTransaction.value = row
    const response = await posApi.getTransactionDetails(row.TransactionId)
    if (response.data.success) {
      transactionDetails.value = response.data.data || []
      detailDialogVisible.value = true
    } else {
      ElMessage.error(response.data.message || '查詢明細失敗')
    }
  } catch (error) {
    ElMessage.error('查詢明細失敗：' + (error.message || '未知錯誤'))
  }
}

// 行點擊
const handleRowClick = (row) => {
  handleViewDetail(row)
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

// 交易類型標籤
const getTransactionTypeTag = (type) => {
  const typeMap = {
    'Sale': 'success',
    'Return': 'warning',
    'Refund': 'danger'
  }
  return typeMap[type] || 'info'
}

// 交易類型文字
const getTransactionTypeText = (type) => {
  const typeMap = {
    'Sale': '銷售',
    'Return': '退貨',
    'Refund': '退款'
  }
  return typeMap[type] || type
}

// 狀態標籤
const getStatusTag = (status) => {
  const statusMap = {
    'Pending': 'warning',
    'Synced': 'success',
    'Failed': 'danger'
  }
  return statusMap[status] || 'info'
}

// 狀態文字
const getStatusText = (status) => {
  const statusMap = {
    'Pending': '待同步',
    'Synced': '已同步',
    'Failed': '同步失敗'
  }
  return statusMap[status] || status
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return ''
  const date = new Date(dateTime)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
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

.pos-transaction {
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

  .table-card {
    margin-bottom: 20px;
  }
}
</style>


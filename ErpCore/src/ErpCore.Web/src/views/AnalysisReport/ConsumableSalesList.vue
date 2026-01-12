<template>
  <div class="consumable-sales-list">
    <div class="page-header">
      <h1>耗材出售維護作業 (SYSA297)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="交易單號">
          <el-input v-model="queryForm.filters.txnNo" placeholder="請輸入交易單號" clearable />
        </el-form-item>
        <el-form-item label="店別">
          <el-select v-model="queryForm.filters.siteId" placeholder="請選擇店別" clearable style="width: 200px">
            <el-option
              v-for="site in siteList"
              :key="site.value"
              :label="site.label"
              :value="site.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.filters.status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="待審核" value="1" />
            <el-option label="已審核" value="2" />
            <el-option label="已取消" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="出售日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleAdd">新增</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="txnNo" label="交易單號" width="150" />
        <el-table-column prop="siteName" label="店別" width="120" />
        <el-table-column prop="purchaseDate" label="出售日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.purchaseDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="statusName" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ row.statusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="totalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.totalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="detailCount" label="明細數量" width="100" align="center" />
        <el-table-column prop="createdBy" label="建立者" width="100" />
        <el-table-column prop="createdAt" label="建立時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" v-if="row.status === '1'">修改</el-button>
            <el-button type="success" size="small" @click="handleApprove(row)" v-if="row.status === '1'">審核</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" v-if="row.status === '1'">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.pageIndex"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.totalCount"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 新增/修改對話框 -->
    <ConsumableSalesDialog
      v-model="dialogVisible"
      :sales-data="currentSales"
      :is-edit="isEdit"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { consumableSalesApi } from '@/api/consumableSales'
import { dropdownListApi } from '@/api/dropdownList'
import ConsumableSalesDialog from './ConsumableSalesDialog.vue'

const loading = ref(false)
const tableData = ref([])
const dateRange = ref([])
const dialogVisible = ref(false)
const currentSales = ref(null)
const isEdit = ref(false)
const siteList = ref([])

const queryForm = reactive({
  pageIndex: 1,
  pageSize: 20,
  sortField: 'PurchaseDate',
  sortOrder: 'DESC',
  filters: {
    txnNo: '',
    siteId: '',
    status: '',
    startDate: null,
    endDate: null
  }
})

const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.filters.startDate = dates[0]
    queryForm.filters.endDate = dates[1]
  } else {
    queryForm.filters.startDate = null
    queryForm.filters.endDate = null
  }
}

const handleSearch = async () => {
  try {
    loading.value = true
    const params = {
      ...queryForm,
      filters: {
        ...queryForm.filters
      }
    }
    const response = await consumableSalesApi.getSales(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.totalCount = response.data.data.totalCount
      pagination.pageIndex = response.data.data.pageIndex
      pagination.pageSize = response.data.data.pageSize
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryForm.filters.txnNo = ''
  queryForm.filters.siteId = ''
  queryForm.filters.status = ''
  queryForm.filters.startDate = null
  queryForm.filters.endDate = null
  dateRange.value = []
  handleSearch()
}

const handleAdd = () => {
  currentSales.value = null
  isEdit.value = false
  dialogVisible.value = true
}

const handleView = async (row) => {
  try {
    const response = await consumableSalesApi.getSalesDetail(row.txnNo)
    if (response.data.success) {
      currentSales.value = response.data.data
      isEdit.value = false
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  }
}

const handleEdit = async (row) => {
  try {
    const response = await consumableSalesApi.getSalesDetail(row.txnNo)
    if (response.data.success) {
      currentSales.value = response.data.data
      isEdit.value = true
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  }
}

const handleApprove = async (row) => {
  try {
    await ElMessageBox.confirm('確定要審核此耗材出售單嗎？', '確認審核', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    const response = await consumableSalesApi.approveSales(row.txnNo, { approved: true })
    if (response.data.success) {
      ElMessage.success('審核成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '審核失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('審核失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此耗材出售單嗎？', '確認刪除', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    const response = await consumableSalesApi.deleteSales(row.txnNo)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

const handleSizeChange = (size) => {
  queryForm.pageSize = size
  queryForm.pageIndex = 1
  handleSearch()
}

const handlePageChange = (page) => {
  queryForm.pageIndex = page
  handleSearch()
}

const handleDialogSuccess = () => {
  dialogVisible.value = false
  handleSearch()
}

const getStatusType = (status) => {
  const types = {
    '1': 'warning',
    '2': 'success',
    '3': 'info'
  }
  return types[status] || ''
}

const formatDate = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString('zh-TW')
}

const formatDateTime = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleString('zh-TW')
}

const formatCurrency = (amount) => {
  if (amount == null) return '0.00'
  return new Intl.NumberFormat('zh-TW', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const loadSiteList = async () => {
  try {
    const response = await dropdownListApi.getShopOptions({})
    if (response.data && response.data.success && response.data.data) {
      siteList.value = response.data.data.map(item => ({
        value: item.value || item.ShopId || item.siteId,
        label: item.label || item.ShopName || item.siteName
      }))
    }
  } catch (error) {
    console.error('載入店別列表失敗:', error)
  }
}

onMounted(() => {
  loadSiteList()
  handleSearch()
})
</script>

<style scoped>
.consumable-sales-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: bold;
  color: #303133;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin-top: 10px;
}

.table-card {
  margin-bottom: 20px;
}

.pagination-container {
  margin-top: 20px;
  text-align: right;
}
</style>

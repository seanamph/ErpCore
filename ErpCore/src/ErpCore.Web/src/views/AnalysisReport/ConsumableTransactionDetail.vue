<template>
  <div class="consumable-transaction-detail">
    <el-table
      :data="tableData"
      v-loading="loading"
      border
      stripe
      style="width: 100%"
    >
      <el-table-column prop="TransactionDate" label="異動日期" width="150">
        <template #default="{ row }">
          {{ formatDate(row.TransactionDate) }}
        </template>
      </el-table-column>
      <el-table-column prop="TransactionTypeName" label="異動類型" width="100" />
      <el-table-column prop="Quantity" label="數量" width="100" align="right">
        <template #default="{ row }">
          {{ formatNumber(row.Quantity) }}
        </template>
      </el-table-column>
      <el-table-column prop="UnitPrice" label="單價" width="100" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.UnitPrice) }}
        </template>
      </el-table-column>
      <el-table-column prop="Amount" label="金額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.Amount) }}
        </template>
      </el-table-column>
      <el-table-column prop="SiteName" label="店別" width="120" />
      <el-table-column prop="WarehouseName" label="庫別" width="120" />
      <el-table-column prop="SourceId" label="來源單號" width="150" />
      <el-table-column prop="Notes" label="備註" min-width="200" />
      <el-table-column prop="CreatedBy" label="建立者" width="100" />
      <el-table-column prop="CreatedAt" label="建立時間" width="150">
        <template #default="{ row }">
          {{ formatDateTime(row.CreatedAt) }}
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
  </div>
</template>

<script setup>
import { ref, reactive, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { consumableReportApi } from '@/api/consumableReport'

const props = defineProps({
  consumableId: {
    type: String,
    required: true
  }
})

const loading = ref(false)
const tableData = ref([])

const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const loadData = async () => {
  if (!props.consumableId) return

  try {
    loading.value = true
    const params = {
      PageIndex: pagination.pageIndex,
      PageSize: pagination.pageSize
    }

    const response = await consumableReportApi.getTransactions(props.consumableId, params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.totalCount = response.data.data.totalCount
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleSizeChange = (size) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  loadData()
}

const handlePageChange = (page) => {
  pagination.pageIndex = page
  loadData()
}

const formatDate = (value) => {
  if (!value) return '-'
  return new Date(value).toLocaleDateString('zh-TW')
}

const formatDateTime = (value) => {
  if (!value) return '-'
  return new Date(value).toLocaleString('zh-TW')
}

const formatCurrency = (value) => {
  if (value == null) return '-'
  return new Intl.NumberFormat('zh-TW', {
    style: 'currency',
    currency: 'TWD',
    minimumFractionDigits: 0
  }).format(value)
}

const formatNumber = (value) => {
  if (value == null) return '-'
  return new Intl.NumberFormat('zh-TW').format(value)
}

watch(() => props.consumableId, () => {
  pagination.pageIndex = 1
  loadData()
})

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.consumable-transaction-detail {
  padding: 10px;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}
</style>

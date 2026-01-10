<template>
  <div class="b2b-sales-query">
    <div class="page-header">
      <h1>B2B?∑ÂîÆ?•Ë©¢‰ΩúÊ•≠</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?∑ÂîÆ?ÆË?">
          <el-input v-model="queryForm.OrderId" placeholder="Ë´ãËº∏?•Èä∑?ÆÂñÆ?? clearable />
        </el-form-item>
        <el-form-item label="ÂÆ¢Êà∂‰ª?¢º">
          <el-input v-model="queryForm.CustomerId" placeholder="Ë´ãËº∏?•ÂÆ¢?∂‰ª£Á¢? clearable />
        </el-form-item>
        <el-form-item label="?∑ÂîÆ?•Ê?">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="??
            start-placeholder="?ãÂ??•Ê?"
            end-placeholder="ÁµêÊ??•Ê?"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="?âÁ®ø" value="D" />
            <el-option label="Â∑≤ÈÄÅÂá∫" value="S" />
            <el-option label="Â∑≤ÂØ©?? value="A" />
            <el-option label="Â∑≤Âá∫Ë≤? value="O" />
            <el-option label="Â∑≤Â?Ê∂? value="X" />
            <el-option label="Â∑≤Á?Ê°? value="C" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
          <el-button type="info" @click="handleStatistics">Áµ±Ë?</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- Ë≥áÊ?Ë°®Ê†º -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="OrderId" label="?∑ÂîÆ?ÆË?" width="150" />
        <el-table-column prop="OrderDate" label="?∑ÂîÆ?•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CustomerId" label="ÂÆ¢Êà∂‰ª?¢º" width="120" />
        <el-table-column prop="CustomerName" label="ÂÆ¢Êà∂?çÁ®±" width="200" />
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="Á∏ΩÈ?È°? width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- ?ÜÈ? -->
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

    <!-- Áµ±Ë?Â∞çË©±Ê°?-->
    <el-dialog
      v-model="statisticsDialogVisible"
      title="B2B?∑ÂîÆÁµ±Ë?"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-descriptions :column="2" border v-if="statisticsData">
        <el-descriptions-item label="Á∏ΩÈä∑?ÆÂñÆ??>{{ statisticsData.TotalOrders }}</el-descriptions-item>
        <el-descriptions-item label="Á∏ΩÈä∑?ÆÈ?È°?>{{ formatCurrency(statisticsData.TotalAmount) }}</el-descriptions-item>
        <el-descriptions-item label="Á∏ΩÈä∑?ÆÊï∏??>{{ formatNumber(statisticsData.TotalQty) }}</el-descriptions-item>
        <el-descriptions-item label="Âπ≥Â??ÆÂÉπ">{{ formatCurrency(statisticsData.AvgPrice) }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="statisticsDialogVisible = false">?úÈ?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { b2bSalesQueryApi } from '@/api/invoiceSales'

// ?•Ë©¢Ë°®ÂñÆ
const queryForm = reactive({
  OrderId: '',
  CustomerId: '',
  OrderDateFrom: '',
  OrderDateTo: '',
  Status: ''
})
const dateRange = ref(null)

// ??ÅΩ?•Ê?ÁØÑÂ?ËÆäÂ?
watch(dateRange, (val) => {
  if (val && val.length === 2) {
    queryForm.OrderDateFrom = val[0]
    queryForm.OrderDateTo = val[1]
  } else {
    queryForm.OrderDateFrom = ''
    queryForm.OrderDateTo = ''
  }
})

// Ë°®Ê†ºË≥áÊ?
const tableData = ref([])
const loading = ref(false)

// ?ÜÈ?
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// Áµ±Ë?Â∞çË©±Ê°?
const statisticsDialogVisible = ref(false)
const statisticsData = ref(null)

// ?•Ë©¢
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
    const response = await b2bSalesQueryApi.query(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '?•Ë©¢Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('?•Ë©¢Â§±Ê?Ôº? + error.message)
  } finally {
    loading.value = false
  }
}

// ?çÁΩÆ
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

// ?•Á?
const handleView = (row) => {
  ElMessage.info('?•Á??üËÉΩ?ãÁôº‰∏?)
}

// Áµ±Ë?
const handleStatistics = async () => {
  try {
    const params = {
      OrderId: queryForm.OrderId || undefined,
      CustomerId: queryForm.CustomerId || undefined,
      OrderDateFrom: queryForm.OrderDateFrom || undefined,
      OrderDateTo: queryForm.OrderDateTo || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await b2bSalesQueryApi.getStatistics(params)
    if (response.data?.success) {
      statisticsData.value = response.data.data
      statisticsDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || 'Áµ±Ë?Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('Áµ±Ë?Â§±Ê?Ôº? + error.message)
  }
}

// ?ÜÈ?Â§ßÂ?ËÆäÊõ¥
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// ?ÜÈ?ËÆäÊõ¥
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// ?ºÂ??ñÊó•??
const formatDate = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString('zh-TW')
}

// ?ºÂ??ñË≤®Âπ?
const formatCurrency = (amount) => {
  if (!amount) return '0'
  return new Intl.NumberFormat('zh-TW', { style: 'currency', currency: 'TWD' }).format(amount)
}

// ?ºÂ??ñÊï∏Â≠?
const formatNumber = (num) => {
  if (!num) return '0'
  return new Intl.NumberFormat('zh-TW').format(num)
}

// ?ñÂ??Ä?ãÈ???
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

// ?ñÂ??Ä?ãÊ?Â≠?
const getStatusText = (status) => {
  const texts = {
    'D': '?âÁ®ø',
    'S': 'Â∑≤ÈÄÅÂá∫',
    'A': 'Â∑≤ÂØ©??,
    'O': 'Â∑≤Âá∫Ë≤?,
    'X': 'Â∑≤Â?Ê∂?,
    'C': 'Â∑≤Á?Ê°?
  }
  return texts[status] || status
}

// ?ùÂ???
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.b2b-sales-query {
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


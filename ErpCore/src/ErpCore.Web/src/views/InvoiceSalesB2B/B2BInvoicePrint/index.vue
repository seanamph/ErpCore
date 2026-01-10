<template>
  <div class="b2b-invoice-print">
    <div class="page-header">
      <h1>B2B?ªÂ??ºÁ•®?óÂç∞</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ºÁ•®Á∑®Ë?">
          <el-input v-model="queryForm.InvoiceNo" placeholder="Ë´ãËº∏?•ÁôºÁ•®Á∑®?? clearable />
        </el-form-item>
        <el-form-item label="?ºÁ•®?•Ê?">
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
            <el-option label="Â∑≤È?Á´? value="ISSUED" />
            <el-option label="Â∑≤‰?Âª? value="CANCELLED" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="InvoiceNo" label="?ºÁ•®Á∑®Ë?" width="150" />
        <el-table-column prop="InvoiceDate" label="?ºÁ•®?•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.InvoiceDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CustomerName" label="ÂÆ¢Êà∂?çÁ®±" width="200" />
        <el-table-column prop="TotalAmount" label="Á∏ΩÈ?È°? width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusName(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PrintCount" label="?óÂç∞Ê¨°Êï∏" width="100" align="center" />
        <el-table-column label="?ç‰?" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="success" size="small" @click="handlePrint(row)">?óÂç∞</el-button>
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

      <!-- ?πÊ¨°?ç‰? -->
      <div style="margin-top: 20px">
        <el-button type="success" :disabled="selectedRows.length === 0" @click="handleBatchPrint">
          ?πÊ¨°?óÂç∞ ({{ selectedRows.length }})
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { b2bInvoicePrintApi } from '@/api/invoiceSales'

// ?•Ë©¢Ë°®ÂñÆ
const queryForm = reactive({
  InvoiceNo: '',
  Status: ''
})
const dateRange = ref(null)

// Ë°®Ê†ºË≥áÊ?
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// ?ÜÈ?
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// ?•Ë©¢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      InvoiceNo: queryForm.InvoiceNo || undefined,
      Status: queryForm.Status || undefined,
      StartDate: dateRange.value?.[0] || undefined,
      EndDate: dateRange.value?.[1] || undefined
    }
    const response = await b2bInvoicePrintApi.getB2BElectronicInvoices(params)
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
  queryForm.InvoiceNo = ''
  queryForm.Status = ''
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// ?∏Ê?ËÆäÊõ¥
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// ?•Á?
const handleView = (row) => {
  // TODO: ÂØ¶‰??•Á?Ë©≥Ê?
  ElMessage.info('?•Á??üËÉΩ?ãÁôº‰∏?)
}

// ?óÂç∞
const handlePrint = async (row) => {
  try {
    const response = await b2bInvoicePrintApi.printB2BInvoice({ InvoiceNo: row.InvoiceNo })
    if (response.data?.success) {
      ElMessage.success('?óÂç∞?êÂ?')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '?óÂç∞Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('?óÂç∞Â§±Ê?Ôº? + error.message)
  }
}

// ?πÊ¨°?óÂç∞
const handleBatchPrint = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('Ë´ãÈÅ∏?áË??óÂç∞?ÑÁôºÁ•?)
    return
  }
  try {
    const invoiceNos = selectedRows.value.map(row => row.InvoiceNo)
    for (const invoiceNo of invoiceNos) {
      await b2bInvoicePrintApi.printB2BInvoice({ InvoiceNo: invoiceNo })
    }
    ElMessage.success(`?êÂ??óÂç∞ ${invoiceNos.length} ÂºµÁôºÁ•®`)
    handleSearch()
  } catch (error) {
    ElMessage.error('?πÊ¨°?óÂç∞Â§±Ê?Ôº? + error.message)
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

// ?ñÂ??Ä?ãÈ???
const getStatusType = (status) => {
  const types = {
    'ISSUED': 'success',
    'CANCELLED': 'danger'
  }
  return types[status] || 'info'
}

// ?ñÂ??Ä?ãÂ?Á®?
const getStatusName = (status) => {
  const names = {
    'ISSUED': 'Â∑≤È?Á´?,
    'CANCELLED': 'Â∑≤‰?Âª?
  }
  return names[status] || status
}

// ?ùÂ???
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.b2b-invoice-print {
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


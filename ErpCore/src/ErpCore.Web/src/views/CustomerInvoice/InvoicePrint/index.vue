<template>
  <div class="invoice-print">
    <div class="page-header">
      <h1>發票列印作業 (SYS2000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="發票號碼">
          <el-input v-model="queryForm.InvoiceNo" placeholder="請輸入發票號碼" clearable />
        </el-form-item>
        <el-form-item label="客戶編號">
          <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶編號" clearable />
        </el-form-item>
        <el-form-item label="發票日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="草稿" value="DRAFT" />
            <el-option label="已開立" value="ISSUED" />
            <el-option label="已作廢" value="CANCELLED" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="InvoiceNo" label="發票號碼" width="150" />
        <el-table-column prop="InvoiceType" label="發票類型" width="120">
          <template #default="{ row }">
            {{ getInvoiceTypeName(row.InvoiceType) }}
          </template>
        </el-table-column>
        <el-table-column prop="InvoiceDate" label="發票日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.InvoiceDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CustomerId" label="客戶編號" width="150" />
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusName(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PrintCount" label="列印次數" width="100" align="center" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="success" size="small" @click="handlePrint(row)">列印</el-button>
            <el-button type="info" size="small" @click="handleViewLogs(row)">記錄</el-button>
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

      <!-- 批次操作 -->
      <div style="margin-top: 20px">
        <el-button type="success" :disabled="selectedRows.length === 0" @click="handleBatchPrint">
          批次列印 ({{ selectedRows.length }})
        </el-button>
      </div>
    </el-card>

    <!-- 發票詳情對話框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="發票詳情"
      width="900px"
      :close-on-click-modal="false"
    >
      <el-descriptions :column="2" border v-if="currentInvoice">
        <el-descriptions-item label="發票號碼">{{ currentInvoice.InvoiceNo }}</el-descriptions-item>
        <el-descriptions-item label="發票類型">{{ getInvoiceTypeName(currentInvoice.InvoiceType) }}</el-descriptions-item>
        <el-descriptions-item label="發票日期">{{ formatDate(currentInvoice.InvoiceDate) }}</el-descriptions-item>
        <el-descriptions-item label="客戶編號">{{ currentInvoice.CustomerId }}</el-descriptions-item>
        <el-descriptions-item label="總金額">{{ formatCurrency(currentInvoice.TotalAmount) }}</el-descriptions-item>
        <el-descriptions-item label="稅額">{{ formatCurrency(currentInvoice.TaxAmount) }}</el-descriptions-item>
        <el-descriptions-item label="金額">{{ formatCurrency(currentInvoice.Amount) }}</el-descriptions-item>
        <el-descriptions-item label="狀態">{{ getStatusName(currentInvoice.Status) }}</el-descriptions-item>
        <el-descriptions-item label="列印次數">{{ currentInvoice.PrintCount }}</el-descriptions-item>
        <el-descriptions-item label="最後列印日期">{{ formatDateTime(currentInvoice.LastPrintDate) }}</el-descriptions-item>
      </el-descriptions>

      <el-divider>發票明細</el-divider>
      <el-table :data="currentInvoice?.Details || []" border style="width: 100%">
        <el-table-column prop="LineNum" label="行號" width="80" align="center" />
        <el-table-column prop="GoodsName" label="商品名稱" min-width="200" />
        <el-table-column prop="Qty" label="數量" width="100" align="right" />
        <el-table-column prop="UnitPrice" label="單價" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.UnitPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
          </template>
        </el-table-column>
      </el-table>

      <template #footer>
        <el-button @click="detailDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handlePrintFromDetail">列印</el-button>
      </template>
    </el-dialog>

    <!-- 列印設定對話框 -->
    <el-dialog
      v-model="printDialogVisible"
      title="列印設定"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form :model="printForm" label-width="120px">
        <el-form-item label="列印格式">
          <el-select v-model="printForm.PrintFormat" placeholder="請選擇">
            <el-option label="標準格式" value="STANDARD" />
            <el-option label="簡易格式" value="SIMPLE" />
          </el-select>
        </el-form-item>
        <el-form-item label="包含封面">
          <el-switch v-model="printForm.IncludeCover" />
        </el-form-item>
        <el-form-item label="列印份數">
          <el-input-number v-model="printForm.PrintCount" :min="1" :max="10" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="printDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleConfirmPrint">確定列印</el-button>
      </template>
    </el-dialog>

    <!-- 列印記錄對話框 -->
    <el-dialog
      v-model="logDialogVisible"
      title="列印記錄"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-table :data="printLogs" border style="width: 100%">
        <el-table-column prop="PrintDate" label="列印日期" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.PrintDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="PrintUser" label="列印人員" width="120" />
        <el-table-column prop="PrintFormat" label="列印格式" width="120" />
        <el-table-column prop="PrintType" label="列印類型" width="120">
          <template #default="{ row }">
            {{ row.PrintType === 'NORMAL' ? '一般列印' : '重新列印' }}
          </template>
        </el-table-column>
        <el-table-column prop="PrintCount" label="列印份數" width="100" align="center" />
        <el-table-column prop="PrinterName" label="印表機" min-width="150" />
      </el-table>
      <template #footer>
        <el-button @click="logDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { invoicePrintApi } from '@/api/customerInvoice'

// 查詢表單
const queryForm = reactive({
  InvoiceNo: '',
  CustomerId: '',
  InvoiceDateFrom: '',
  InvoiceDateTo: '',
  Status: ''
})
const dateRange = ref(null)

// 監聽日期範圍變化
watch(dateRange, (val) => {
  if (val && val.length === 2) {
    queryForm.InvoiceDateFrom = val[0]
    queryForm.InvoiceDateTo = val[1]
  } else {
    queryForm.InvoiceDateFrom = ''
    queryForm.InvoiceDateTo = ''
  }
})

// 表格資料
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const detailDialogVisible = ref(false)
const printDialogVisible = ref(false)
const logDialogVisible = ref(false)
const currentInvoice = ref(null)
const printLogs = ref([])
const currentPrintInvoiceNo = ref(null)

// 列印表單
const printForm = reactive({
  PrintFormat: 'STANDARD',
  IncludeCover: true,
  PrintCount: 1
})

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      InvoiceNo: queryForm.InvoiceNo || undefined,
      CustomerId: queryForm.CustomerId || undefined,
      InvoiceDateFrom: queryForm.InvoiceDateFrom || undefined,
      InvoiceDateTo: queryForm.InvoiceDateTo || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await invoicePrintApi.getInvoices(params)
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
  queryForm.InvoiceNo = ''
  queryForm.CustomerId = ''
  queryForm.InvoiceDateFrom = ''
  queryForm.InvoiceDateTo = ''
  queryForm.Status = ''
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// 查看
const handleView = async (row) => {
  try {
    const response = await invoicePrintApi.getInvoice(row.InvoiceNo)
    if (response.data?.success) {
      currentInvoice.value = response.data.data
      detailDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 列印
const handlePrint = (row) => {
  currentPrintInvoiceNo.value = row.InvoiceNo
  printDialogVisible.value = true
}

// 從詳情列印
const handlePrintFromDetail = () => {
  if (currentInvoice.value) {
    currentPrintInvoiceNo.value = currentInvoice.value.InvoiceNo
    printDialogVisible.value = true
  }
}

// 確認列印
const handleConfirmPrint = async () => {
  if (!currentPrintInvoiceNo.value) return
  try {
    const response = await invoicePrintApi.printInvoice(currentPrintInvoiceNo.value, printForm)
    if (response.data?.success) {
      ElMessage.success('列印成功')
      printDialogVisible.value = false
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '列印失敗')
    }
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
  }
}

// 批次列印
const handleBatchPrint = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇要列印的發票')
    return
  }
  try {
    await ElMessageBox.confirm(`確定要批次列印 ${selectedRows.value.length} 張發票嗎？`, '確認', {
      type: 'warning'
    })
    const invoiceNos = selectedRows.value.map(row => row.InvoiceNo)
    const response = await invoicePrintApi.batchPrintInvoices({
      InvoiceNos: invoiceNos,
      PrintFormat: printForm.PrintFormat,
      IncludeCover: printForm.IncludeCover
    })
    if (response.data?.success) {
      ElMessage.success('批次列印成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '批次列印失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次列印失敗：' + error.message)
    }
  }
}

// 查看列印記錄
const handleViewLogs = async (row) => {
  try {
    const response = await invoicePrintApi.getPrintLogs(row.InvoiceNo)
    if (response.data?.success) {
      printLogs.value = response.data.data || []
      logDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
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

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// 格式化貨幣
const formatCurrency = (amount) => {
  if (amount == null) return '0.00'
  return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 取得發票類型名稱
const getInvoiceTypeName = (type) => {
  const types = {
    'NORMAL': '一般',
    'EINVOICE': '電子發票',
    'CHANGE': '變更'
  }
  return types[type] || type
}

// 取得狀態名稱
const getStatusName = (status) => {
  const statuses = {
    'DRAFT': '草稿',
    'ISSUED': '已開立',
    'CANCELLED': '已作廢'
  }
  return statuses[status] || status
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'DRAFT': 'info',
    'ISSUED': 'success',
    'CANCELLED': 'danger'
  }
  return types[status] || ''
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.invoice-print {
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


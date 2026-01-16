<template>
  <div class="purchase-report-query">
    <div class="page-header">
      <h1>採購報表查詢 (SYSP410-SYSP4I0)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable>
            <el-option label="採購單報表" value="" />
            <el-option label="明細報表" value="Detail" />
          </el-select>
        </el-form-item>
        <el-form-item label="採購單號">
          <el-input v-model="queryForm.Filters.OrderId" placeholder="請輸入採購單號" clearable />
        </el-form-item>
        <el-form-item label="單據類型">
          <el-select v-model="queryForm.Filters.OrderType" placeholder="請選擇單據類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="採購" value="PO" />
            <el-option label="退貨" value="RT" />
          </el-select>
        </el-form-item>
        <el-form-item label="分店">
          <el-input v-model="queryForm.Filters.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="供應商">
          <el-input v-model="queryForm.Filters.SupplierId" placeholder="請輸入供應商代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Filters.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="草稿" value="D" />
            <el-option label="已送出" value="S" />
            <el-option label="已審核" value="A" />
            <el-option label="已取消" value="X" />
            <el-option label="已結案" value="C" />
          </el-select>
        </el-form-item>
        <el-form-item label="採購日期">
          <el-date-picker
            v-model="orderDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="商品代碼">
          <el-input v-model="queryForm.Filters.GoodsId" placeholder="請輸入商品代碼" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery" :loading="loading">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport" :loading="exporting">匯出</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <!-- 採購單報表 -->
      <el-table
        v-if="!queryForm.ReportType || queryForm.ReportType === ''"
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="OrderId" label="採購單號" width="150" fixed="left" />
        <el-table-column prop="OrderDate" label="採購日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderTypeName" label="單據類型" width="100" />
        <el-table-column prop="ShopName" label="分店" width="120" />
        <el-table-column prop="SupplierName" label="供應商" width="150" />
        <el-table-column prop="StatusName" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ row.StatusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalQty" label="總數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="DetailCount" label="明細筆數" width="100" align="center" />
        <el-table-column prop="TotalReceivedQty" label="已收數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalReceivedQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalReturnQty" label="已退數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalReturnQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="ApplyUserName" label="申請人" width="100" />
        <el-table-column prop="ApproveUserName" label="審核人" width="100" />
      </el-table>

      <!-- 明細報表 -->
      <el-table
        v-else
        :data="detailTableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="OrderId" label="採購單號" width="150" fixed="left" />
        <el-table-column prop="OrderDate" label="採購日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ShopName" label="分店" width="120" />
        <el-table-column prop="SupplierName" label="供應商" width="150" />
        <el-table-column prop="LineNum" label="行號" width="80" align="center" />
        <el-table-column prop="GoodsId" label="商品代碼" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" />
        <el-table-column prop="OrderQty" label="採購數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.OrderQty) }}
          </template>
        </el-table-column>
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
        <el-table-column prop="ReceivedQty" label="已收數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.ReceivedQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="ReturnQty" label="已退數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.ReturnQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="PendingQty" label="待收數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.PendingQty) }}
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="pagination.TotalCount"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right;"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { purchaseReportApi } from '@/api/purchaseReport'

const loading = ref(false)
const exporting = ref(false)
const orderDateRange = ref(null)
const tableData = ref([])
const detailTableData = ref([])

const queryForm = reactive({
  PageIndex: 1,
  PageSize: 20,
  SortField: 'OrderDate',
  SortOrder: 'DESC',
  ReportType: '',
  Filters: {
    OrderId: '',
    OrderType: '',
    ShopId: '',
    SupplierId: '',
    Status: '',
    OrderDateFrom: null,
    OrderDateTo: null,
    GoodsId: '',
    ApplyUserId: '',
    ApproveUserId: ''
  }
})

const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 日期範圍變更
const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.Filters.OrderDateFrom = dates[0]
    queryForm.Filters.OrderDateTo = dates[1]
  } else {
    queryForm.Filters.OrderDateFrom = null
    queryForm.Filters.OrderDateTo = null
  }
}

// 查詢
const handleQuery = async () => {
  loading.value = true
  try {
    queryForm.PageIndex = pagination.PageIndex
    queryForm.PageSize = pagination.PageSize

    if (queryForm.ReportType === 'Detail') {
      const response = await purchaseReportApi.queryPurchaseReportDetails(queryForm)
      if (response.data.Success) {
        detailTableData.value = response.data.Data.Items || []
        pagination.TotalCount = response.data.Data.TotalCount || 0
      } else {
        ElMessage.error(response.data.Message || '查詢失敗')
      }
    } else {
      const response = await purchaseReportApi.queryPurchaseReports(queryForm)
      if (response.data.Success) {
        tableData.value = response.data.Data.Items || []
        pagination.TotalCount = response.data.Data.TotalCount || 0
      } else {
        ElMessage.error(response.data.Message || '查詢失敗')
      }
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.Filters = {
    OrderId: '',
    OrderType: '',
    ShopId: '',
    SupplierId: '',
    Status: '',
    OrderDateFrom: null,
    OrderDateTo: null,
    GoodsId: '',
    ApplyUserId: '',
    ApproveUserId: ''
  }
  orderDateRange.value = null
  pagination.PageIndex = 1
  pagination.PageSize = 20
  tableData.value = []
  detailTableData.value = []
}

// 匯出
const handleExport = async () => {
  exporting.value = true
  try {
    const exportDto = {
      Query: queryForm,
      ExportType: 'Excel',
      FileName: `採購報表_${new Date().toISOString().slice(0, 10)}.xlsx`
    }

    const response = await purchaseReportApi.exportPurchaseReport(exportDto)
    
    // 下載檔案
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', exportDto.FileName)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)

    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗：' + (error.message || '未知錯誤'))
  } finally {
    exporting.value = false
  }
}

// 分頁變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleQuery()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleQuery()
}

// 格式化
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

const formatNumber = (num) => {
  if (num == null) return '0'
  return new Intl.NumberFormat('zh-TW').format(num)
}

const getStatusType = (status) => {
  const statusMap = {
    'D': 'info',
    'S': 'warning',
    'A': 'success',
    'X': 'danger',
    'C': 'success'
  }
  return statusMap[status] || 'info'
}

// 初始化
onMounted(() => {
  // 可以選擇是否自動載入資料
  // handleQuery()
})
</script>

<style scoped>
.purchase-report-query {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: bold;
  margin: 0;
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
</style>

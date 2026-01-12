<template>
  <div class="consumable-report-query">
    <div class="page-header">
      <h1>耗材管理報表 (SYSA255)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="耗材編號">
          <el-input v-model="queryForm.ConsumableId" placeholder="請輸入耗材編號" clearable />
        </el-form-item>
        <el-form-item label="耗材名稱">
          <el-input v-model="queryForm.ConsumableName" placeholder="請輸入耗材名稱" clearable />
        </el-form-item>
        <el-form-item label="分類">
          <el-input v-model="queryForm.CategoryId" placeholder="請輸入分類代碼" clearable />
        </el-form-item>
        <el-form-item label="店別">
          <el-select
            v-model="queryForm.SiteIds"
            placeholder="請選擇店別"
            multiple
            clearable
            style="width: 200px"
          >
            <!-- TODO: 從下拉列表 API 取得店別選項 -->
          </el-select>
        </el-form-item>
        <el-form-item label="庫別">
          <el-select
            v-model="queryForm.WarehouseIds"
            placeholder="請選擇庫別"
            multiple
            clearable
            style="width: 200px"
          >
            <!-- TODO: 從下拉列表 API 取得庫別選項 -->
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="正常" value="1" />
            <el-option label="停用" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="資產狀態">
          <el-input v-model="queryForm.AssetStatus" placeholder="請輸入資產狀態" clearable />
        </el-form-item>
        <el-form-item label="日期範圍">
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
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable>
            <el-option label="摘要" value="Summary" />
            <el-option label="明細" value="Detail" />
            <el-option label="成本分析" value="CostAnalysis" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel">匯出 Excel</el-button>
          <el-button type="warning" @click="handleExportPdf">匯出 PDF</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 統計資訊 -->
    <el-card class="summary-card" shadow="never" v-if="summary">
      <el-row :gutter="20">
        <el-col :span="4">
          <div class="summary-item">
            <div class="summary-label">總耗材數</div>
            <div class="summary-value">{{ summary.TotalConsumables }}</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="summary-item">
            <div class="summary-label">總當前庫存數量</div>
            <div class="summary-value">{{ formatNumber(summary.TotalCurrentQty) }}</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="summary-item">
            <div class="summary-label">總當前庫存金額</div>
            <div class="summary-value">{{ formatCurrency(summary.TotalCurrentAmt) }}</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="summary-item">
            <div class="summary-label">總入庫數量</div>
            <div class="summary-value">{{ formatNumber(summary.TotalInQty) }}</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="summary-item">
            <div class="summary-label">總出庫數量</div>
            <div class="summary-value">{{ formatNumber(summary.TotalOutQty) }}</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="summary-item">
            <div class="summary-label">總入庫金額</div>
            <div class="summary-value">{{ formatCurrency(summary.TotalInAmt) }}</div>
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
        @sort-change="handleSortChange"
      >
        <el-table-column prop="ConsumableId" label="耗材編號" width="120" sortable="custom" />
        <el-table-column prop="ConsumableName" label="耗材名稱" width="200" />
        <el-table-column prop="CategoryName" label="分類" width="120" />
        <el-table-column prop="SiteName" label="店別" width="120" />
        <el-table-column prop="WarehouseName" label="庫別" width="120" />
        <el-table-column prop="Unit" label="單位" width="80" />
        <el-table-column prop="Specification" label="規格" width="150" />
        <el-table-column prop="Brand" label="品牌" width="100" />
        <el-table-column prop="Model" label="型號" width="100" />
        <el-table-column prop="BarCode" label="條碼" width="150" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.StatusName || (row.Status === '1' ? '正常' : '停用') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="AssetStatusName" label="資產狀態" width="100" />
        <el-table-column prop="Location" label="位置" width="120" />
        <el-table-column prop="CurrentQty" label="當前庫存數量" width="120" align="right" sortable="custom">
          <template #default="{ row }">
            <span :class="{ 'low-stock': row.IsLowStock, 'over-stock': row.IsOverStock }">
              {{ formatNumber(row.CurrentQty) }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="CurrentAmt" label="當前庫存金額" width="120" align="right" sortable="custom">
          <template #default="{ row }">
            {{ formatCurrency(row.CurrentAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="InQty" label="入庫數量" width="100" align="right" sortable="custom">
          <template #default="{ row }">
            {{ formatNumber(row.InQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="OutQty" label="出庫數量" width="100" align="right" sortable="custom">
          <template #default="{ row }">
            {{ formatNumber(row.OutQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="InAmt" label="入庫金額" width="120" align="right" sortable="custom">
          <template #default="{ row }">
            {{ formatCurrency(row.InAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="OutAmt" label="出庫金額" width="120" align="right" sortable="custom">
          <template #default="{ row }">
            {{ formatCurrency(row.OutAmt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleViewDetail(row.ConsumableId)">
              查看明細
            </el-button>
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

    <!-- 使用明細對話框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="耗材使用明細"
      width="80%"
      :before-close="handleCloseDetailDialog"
    >
      <ConsumableTransactionDetail
        v-if="detailDialogVisible"
        :consumable-id="selectedConsumableId"
      />
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { consumableReportApi } from '@/api/consumableReport'
import ConsumableTransactionDetail from './ConsumableTransactionDetail.vue'

const loading = ref(false)
const tableData = ref([])
const summary = ref(null)
const dateRange = ref([])
const detailDialogVisible = ref(false)
const selectedConsumableId = ref('')

const queryForm = reactive({
  ConsumableId: '',
  ConsumableName: '',
  CategoryId: '',
  SiteIds: [],
  WarehouseIds: [],
  Status: '',
  AssetStatus: '',
  DateFrom: null,
  DateTo: null,
  ReportType: 'Summary',
  SortField: 'ConsumableId',
  SortOrder: 'ASC'
})

const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0
})

const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.DateFrom = dates[0]
    queryForm.DateTo = dates[1]
  } else {
    queryForm.DateFrom = null
    queryForm.DateTo = null
  }
}

const handleSearch = async () => {
  try {
    loading.value = true
    const params = {
      ...queryForm,
      PageIndex: pagination.pageIndex,
      PageSize: pagination.pageSize,
      SortField: queryForm.SortField,
      SortOrder: queryForm.SortOrder
    }

    const response = await consumableReportApi.getReport(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      summary.value = response.data.data.summary
      pagination.totalCount = response.data.data.totalCount
      pagination.totalPages = response.data.data.totalPages
      ElMessage.success('查詢成功')
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
  queryForm.ConsumableId = ''
  queryForm.ConsumableName = ''
  queryForm.CategoryId = ''
  queryForm.SiteIds = []
  queryForm.WarehouseIds = []
  queryForm.Status = ''
  queryForm.AssetStatus = ''
  queryForm.DateFrom = null
  queryForm.DateTo = null
  queryForm.ReportType = 'Summary'
  queryForm.SortField = 'ConsumableId'
  queryForm.SortOrder = 'ASC'
  dateRange.value = []
  pagination.pageIndex = 1
  pagination.pageSize = 20
  tableData.value = []
  summary.value = null
}

const handleSortChange = ({ prop, order }) => {
  if (prop) {
    queryForm.SortField = prop
    queryForm.SortOrder = order === 'ascending' ? 'ASC' : 'DESC'
    handleSearch()
  }
}

const handleSizeChange = (size) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  handleSearch()
}

const handlePageChange = (page) => {
  pagination.pageIndex = page
  handleSearch()
}

const handleExportExcel = async () => {
  try {
    loading.value = true
    const exportData = {
      ExportType: 'Excel',
      ReportType: queryForm.ReportType,
      Filters: { ...queryForm }
    }

    const response = await consumableReportApi.exportReport(exportData)
    
    // 下載檔案
    const blob = new Blob([response.data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `耗材管理報表_${new Date().toISOString().slice(0, 10)}.xlsx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)

    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleExportPdf = async () => {
  try {
    loading.value = true
    const exportData = {
      ExportType: 'PDF',
      ReportType: queryForm.ReportType,
      Filters: { ...queryForm }
    }

    const response = await consumableReportApi.exportReport(exportData)
    
    // 下載檔案
    const blob = new Blob([response.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `耗材管理報表_${new Date().toISOString().slice(0, 10)}.pdf`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)

    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleViewDetail = (consumableId) => {
  selectedConsumableId.value = consumableId
  detailDialogVisible.value = true
}

const handleCloseDetailDialog = () => {
  detailDialogVisible.value = false
  selectedConsumableId.value = ''
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

onMounted(() => {
  handleSearch()
})
</script>

<style scoped>
.consumable-report-query {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}

.summary-card {
  margin-bottom: 20px;
}

.summary-item {
  text-align: center;
  padding: 10px;
  background-color: #f5f7fa;
  border-radius: 4px;
}

.summary-label {
  font-size: 12px;
  color: #909399;
  margin-bottom: 5px;
}

.summary-value {
  font-size: 18px;
  font-weight: bold;
  color: #303133;
}

.table-card {
  margin-bottom: 20px;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

.low-stock {
  color: #f56c6c;
  font-weight: bold;
}

.over-stock {
  color: #e6a23c;
  font-weight: bold;
}
</style>

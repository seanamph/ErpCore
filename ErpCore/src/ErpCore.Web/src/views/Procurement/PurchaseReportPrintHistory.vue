<template>
  <div class="purchase-report-print-history">
    <div class="page-header">
      <h1>採購報表列印記錄</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable>
            <el-option label="採購單報表" value="PO" />
            <el-option label="供應商報表" value="SU" />
            <el-option label="付款單報表" value="PY" />
          </el-select>
        </el-form-item>
        <el-form-item label="報表代碼">
          <el-input v-model="queryForm.ReportCode" placeholder="請輸入報表代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="成功" value="S" />
            <el-option label="失敗" value="F" />
            <el-option label="處理中" value="P" />
          </el-select>
        </el-form-item>
        <el-form-item label="開始日期">
          <el-date-picker 
            v-model="queryForm.StartDate" 
            type="date" 
            placeholder="請選擇開始日期" 
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="結束日期">
          <el-date-picker 
            v-model="queryForm.EndDate" 
            type="date" 
            placeholder="請選擇結束日期" 
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
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
      >
        <el-table-column prop="ReportName" label="報表名稱" width="200" />
        <el-table-column prop="ReportTypeName" label="報表類型" width="120" />
        <el-table-column prop="ReportCode" label="報表代碼" width="120" />
        <el-table-column prop="PrintDate" label="列印日期" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.PrintDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="PrintUserName" label="列印人員" width="120" />
        <el-table-column prop="FileFormat" label="檔案格式" width="100" />
        <el-table-column prop="FileName" label="檔案名稱" min-width="200" />
        <el-table-column prop="FileSize" label="檔案大小" width="120">
          <template #default="{ row }">
            {{ formatFileSize(row.FileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="StatusName" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ row.StatusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PageCount" label="頁數" width="80" />
        <el-table-column prop="RecordCount" label="記錄數" width="100" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button 
              type="primary" 
              size="small" 
              @click="handleDownload(row)"
              :disabled="row.Status !== 'S'"
            >
              下載
            </el-button>
            <el-button 
              type="danger" 
              size="small" 
              @click="handleDelete(row)"
            >
              刪除
            </el-button>
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
import { ElMessage, ElMessageBox } from 'element-plus'
import { procurementApi } from '@/api/procurement'

const loading = ref(false)
const tableData = ref([])

const queryForm = reactive({
  ReportType: '',
  ReportCode: '',
  Status: '',
  StartDate: '',
  EndDate: ''
})

const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

const handleSearch = () => {
  pagination.PageIndex = 1
  loadData()
}

const handleReset = () => {
  queryForm.ReportType = ''
  queryForm.ReportCode = ''
  queryForm.Status = ''
  queryForm.StartDate = ''
  queryForm.EndDate = ''
  pagination.PageIndex = 1
  loadData()
}

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ReportType: queryForm.ReportType || undefined,
      ReportCode: queryForm.ReportCode || undefined,
      Status: queryForm.Status || undefined,
      StartDate: queryForm.StartDate || undefined,
      EndDate: queryForm.EndDate || undefined
    }

    const response = await procurementApi.getPurchaseReportPrints(params)
    if (response.data.success) {
      tableData.value = response.data.data.Items || []
      pagination.TotalCount = response.data.data.TotalCount || 0
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.message || error.message))
  } finally {
    loading.value = false
  }
}

const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData()
}

const handleDownload = async (row) => {
  try {
    const response = await procurementApi.downloadPurchaseReportPrint(row.TKey)
    const blob = new Blob([response.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = row.FileName || `report_${row.TKey}.pdf`
    link.click()
    window.URL.revokeObjectURL(url)
    ElMessage.success('下載成功')
  } catch (error) {
    ElMessage.error('下載失敗：' + (error.response?.data?.message || error.message))
  }
}

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除報表列印記錄「${row.ReportName}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const response = await procurementApi.deletePurchaseReportPrint(row.TKey)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      loadData()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

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

const formatFileSize = (bytes) => {
  if (!bytes) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

const getStatusType = (status) => {
  const statusMap = {
    'S': 'success',
    'F': 'danger',
    'P': 'warning'
  }
  return statusMap[status] || 'info'
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.purchase-report-print-history {
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

.table-card {
  margin-bottom: 20px;
}
</style>

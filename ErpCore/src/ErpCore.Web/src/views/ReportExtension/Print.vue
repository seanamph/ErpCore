<template>
  <div class="report-print">
    <div class="page-header">
      <h1>報表列印作業 (SYS7B10-SYS7B40)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表代碼">
          <el-input v-model="queryForm.ReportCode" placeholder="請輸入報表代碼" clearable />
        </el-form-item>
        <el-form-item label="報表名稱">
          <el-input v-model="queryForm.ReportName" placeholder="請輸入報表名稱" clearable />
        </el-form-item>
        <el-form-item label="列印狀態">
          <el-select v-model="queryForm.PrintStatus" placeholder="請選擇狀態" clearable>
            <el-option label="待處理" value="PENDING" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item label="列印日期">
          <el-date-picker
            v-model="queryForm.PrintDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
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

    <!-- 列印記錄列表 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="PrintId" label="列印ID" width="100" />
        <el-table-column prop="ReportCode" label="報表代碼" width="150" />
        <el-table-column prop="ReportName" label="報表名稱" width="200" />
        <el-table-column prop="PrintFormat" label="列印格式" width="100" />
        <el-table-column prop="PrintStatus" label="列印狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.PrintStatus)">
              {{ getStatusText(row.PrintStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PageCount" label="頁數" width="80" />
        <el-table-column prop="FileSize" label="檔案大小" width="120">
          <template #default="{ row }">
            {{ formatFileSize(row.FileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="PrintedBy" label="列印者" width="120" />
        <el-table-column prop="PrintedAt" label="列印時間" width="180" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button
              v-if="row.PrintStatus === 'COMPLETED' && row.FileUrl"
              type="primary"
              size="small"
              @click="handleDownload(row)"
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
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>

    <!-- 列印對話框 -->
    <el-dialog
      title="報表列印"
      v-model="printDialogVisible"
      width="800px"
      @close="handlePrintDialogClose"
    >
      <el-form :model="printForm" ref="printFormRef" label-width="120px">
        <el-form-item label="報表代碼" prop="ReportCode">
          <el-input v-model="printForm.ReportCode" placeholder="請輸入報表代碼" />
        </el-form-item>
        <el-form-item label="報表名稱" prop="ReportName">
          <el-input v-model="printForm.ReportName" placeholder="請輸入報表名稱" />
        </el-form-item>
        <el-form-item label="列印格式" prop="PrintFormat">
          <el-select v-model="printForm.PrintFormat" placeholder="請選擇列印格式">
            <el-option label="PDF" value="PDF" />
            <el-option label="Excel" value="Excel" />
            <el-option label="Word" value="Word" />
          </el-select>
        </el-form-item>
        <el-form-item label="列印參數">
          <el-input
            v-model="printForm.PrintParams"
            type="textarea"
            :rows="4"
            placeholder="請輸入列印參數（JSON格式）"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handlePrintDialogClose">取消</el-button>
        <el-button type="primary" @click="handleExecutePrint">執行列印</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { reportExtensionApi } from '@/api/reportExtension'

// 查詢表單
const queryForm = reactive({
  ReportCode: '',
  ReportName: '',
  PrintStatus: '',
  PrintDateRange: []
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

// 列印對話框
const printDialogVisible = ref(false)
const printForm = reactive({
  ReportCode: '',
  ReportName: '',
  PrintFormat: 'PDF',
  PrintParams: ''
})
const printFormRef = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ReportCode: queryForm.ReportCode || undefined,
      ReportName: queryForm.ReportName || undefined,
      PrintStatus: queryForm.PrintStatus || undefined,
      DateFrom: queryForm.PrintDateRange?.[0] || undefined,
      DateTo: queryForm.PrintDateRange?.[1] || undefined
    }
    const response = await reportExtensionApi.print.getPrintLogs(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ReportCode = ''
  queryForm.ReportName = ''
  queryForm.PrintStatus = ''
  queryForm.PrintDateRange = []
  pagination.PageIndex = 1
  handleSearch()
}

// 執行列印
const handleExecutePrint = async () => {
  if (!printFormRef.value) return
  await printFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const request = {
          ReportCode: printForm.ReportCode,
          ReportName: printForm.ReportName,
          PrintFormat: printForm.PrintFormat,
          PrintParams: printForm.PrintParams ? JSON.parse(printForm.PrintParams) : {}
        }
        const response = await reportExtensionApi.print.printReport(request)
        if (response.data.success) {
          ElMessage.success('列印任務已提交')
          printDialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '列印失敗')
        }
      } catch (error) {
        ElMessage.error('列印失敗：' + error.message)
      }
    }
  })
}

// 下載
const handleDownload = async (row) => {
  try {
    window.open(row.FileUrl, '_blank')
  } catch (error) {
    ElMessage.error('下載失敗：' + error.message)
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此列印記錄嗎？', '警告', {
      type: 'warning',
      confirmButtonText: '確定',
      cancelButtonText: '取消'
    })
    ElMessage.info('刪除功能開發中')
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 關閉列印對話框
const handlePrintDialogClose = () => {
  printDialogVisible.value = false
  printForm.ReportCode = ''
  printForm.ReportName = ''
  printForm.PrintFormat = 'PDF'
  printForm.PrintParams = ''
}

// 取得狀態類型
const getStatusType = (status) => {
  const statusMap = {
    PENDING: 'info',
    PROCESSING: 'warning',
    COMPLETED: 'success',
    FAILED: 'danger'
  }
  return statusMap[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const statusMap = {
    PENDING: '待處理',
    PROCESSING: '處理中',
    COMPLETED: '已完成',
    FAILED: '失敗'
  }
  return statusMap[status] || status
}

// 格式化檔案大小
const formatFileSize = (bytes) => {
  if (!bytes) return '-'
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB'
  return (bytes / (1024 * 1024)).toFixed(2) + ' MB'
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

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.report-print {
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
  margin: 0;
}

.table-card {
  margin-bottom: 20px;
}
</style>


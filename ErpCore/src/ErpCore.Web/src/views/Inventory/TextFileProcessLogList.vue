<template>
  <div class="text-file-process-log-list">
    <div class="page-header">
      <h1>BAT格式文本文件處理 (HT680) - 處理記錄列表</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="文件名稱">
          <el-input v-model="queryForm.FileName" placeholder="請輸入文件名稱" clearable />
        </el-form-item>
        <el-form-item label="文件類型">
          <el-select v-model="queryForm.FileType" placeholder="請選擇文件類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="退貨檔 (BACK)" value="BACK" />
            <el-option label="盤點檔 (INV)" value="INV" />
            <el-option label="訂貨檔 (ORDER)" value="ORDER" />
            <el-option label="訂貨檔版本六 (ORDER_6)" value="ORDER_6" />
            <el-option label="POP卡製作檔 (POP)" value="POP" />
            <el-option label="商品卡檔 (PRIC)" value="PRIC" />
          </el-select>
        </el-form-item>
        <el-form-item label="店別">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入店別代碼" clearable />
        </el-form-item>
        <el-form-item label="處理狀態">
          <el-select v-model="queryForm.ProcessStatus" placeholder="請選擇處理狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="待處理" value="PENDING" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="失敗" value="FAILED" />
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
      >
        <el-table-column prop="FileName" label="文件名稱" width="200" />
        <el-table-column prop="FileType" label="文件類型" width="150">
          <template #default="{ row }">
            <el-tag :type="getFileTypeTagType(row.FileType)">
              {{ getFileTypeText(row.FileType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ShopId" label="店別" width="120" />
        <el-table-column prop="TotalRecords" label="總記錄數" width="100" align="right" />
        <el-table-column prop="SuccessRecords" label="成功記錄數" width="120" align="right">
          <template #default="{ row }">
            <span style="color: #67c23a">{{ row.SuccessRecords }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="FailedRecords" label="失敗記錄數" width="120" align="right">
          <template #default="{ row }">
            <span style="color: #f56c6c">{{ row.FailedRecords }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="ProcessStatus" label="處理狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.ProcessStatus)">
              {{ getStatusText(row.ProcessStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ProcessStartTime" label="處理開始時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.ProcessStartTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="ProcessEndTime" label="處理結束時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.ProcessEndTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewDetails(row)">查看明細</el-button>
            <el-button
              v-if="row.ProcessStatus === 'FAILED'"
              type="warning"
              size="small"
              @click="handleReprocess(row)"
            >
              重新處理
            </el-button>
            <el-button type="info" size="small" @click="handleDownload(row)">下載結果</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px"
      />
    </el-card>

    <!-- 處理明細對話框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="處理明細"
      width="80%"
      @close="handleDetailDialogClose"
    >
      <el-table
        :data="detailData"
        v-loading="detailLoading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="LineNumber" label="行號" width="80" />
        <el-table-column prop="RawData" label="原始資料" width="200" />
        <el-table-column prop="ProcessStatus" label="處理狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getDetailStatusType(row.ProcessStatus)">
              {{ getDetailStatusText(row.ProcessStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ErrorMessage" label="錯誤訊息" />
        <el-table-column prop="ProcessedData" label="處理後的資料">
          <template #default="{ row }">
            <pre v-if="row.ProcessedData">{{ JSON.stringify(JSON.parse(row.ProcessedData), null, 2) }}</pre>
            <span v-else>-</span>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination
        v-model:current-page="detailPagination.PageIndex"
        v-model:page-size="detailPagination.PageSize"
        :total="detailPagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleDetailSizeChange"
        @current-change="handleDetailPageChange"
        style="margin-top: 20px"
      />
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { textFileApi } from '@/api/textFile'

// 查詢表單
const queryForm = reactive({
  FileName: '',
  FileType: '',
  ShopId: '',
  ProcessStatus: ''
})

// 表格數據
const tableData = ref([])
const loading = ref(false)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 明細對話框
const detailDialogVisible = ref(false)
const detailLoading = ref(false)
const detailData = ref([])
const currentLogId = ref(null)
const detailPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadData()
}

// 重置
const handleReset = () => {
  queryForm.FileName = ''
  queryForm.FileType = ''
  queryForm.ShopId = ''
  queryForm.ProcessStatus = ''
  pagination.PageIndex = 1
  loadData()
}

// 載入數據
const loadData = async () => {
  try {
    loading.value = true
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: 'CreatedAt',
      SortOrder: 'DESC',
      Filters: {
        FileName: queryForm.FileName || undefined,
        FileType: queryForm.FileType || undefined,
        ShopId: queryForm.ShopId || undefined,
        ProcessStatus: queryForm.ProcessStatus || undefined
      }
    }

    const response = await textFileApi.getProcessLogs(params)
    if (response.data.Success) {
      tableData.value = response.data.Data.Items
      pagination.TotalCount = response.data.Data.TotalCount
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    console.error('查詢失敗:', error)
    ElMessage.error('查詢失敗')
  } finally {
    loading.value = false
  }
}

// 查看明細
const handleViewDetails = async (row) => {
  currentLogId.value = row.LogId
  detailDialogVisible.value = true
  await loadDetails()
}

// 載入明細
const loadDetails = async () => {
  if (!currentLogId.value) return

  try {
    detailLoading.value = true
    const response = await textFileApi.getProcessDetails(currentLogId.value, {
      PageIndex: detailPagination.PageIndex,
      PageSize: detailPagination.PageSize
    })

    if (response.data.Success) {
      detailData.value = response.data.Data.Items
      detailPagination.TotalCount = response.data.Data.TotalCount
    } else {
      ElMessage.error(response.data.Message || '查詢明細失敗')
    }
  } catch (error) {
    console.error('查詢明細失敗:', error)
    ElMessage.error('查詢明細失敗')
  } finally {
    detailLoading.value = false
  }
}

// 下載結果
const handleDownload = async (row) => {
  try {
    const response = await textFileApi.downloadProcessResult(row.LogId, 'excel')
    
    // 創建下載連結
    const blob = new Blob([response.data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `處理結果_${row.LogId}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('下載成功')
  } catch (error) {
    console.error('下載失敗:', error)
    ElMessage.error('下載失敗')
  }
}

// 重新處理
const handleReprocess = async (row) => {
  try {
    await ElMessageBox.confirm('確定要重新處理此文件嗎？', '確認', {
      type: 'warning'
    })

    const response = await textFileApi.reprocessFile(row.LogId)
    if (response.data.Success) {
      ElMessage.success('重新處理已開始')
      // 重新載入數據
      loadData()
    } else {
      ElMessage.error(response.data.Message || '重新處理失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('重新處理失敗:', error)
      ElMessage.error('重新處理失敗')
    }
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此處理記錄嗎？', '確認', {
      type: 'warning'
    })

    const response = await textFileApi.deleteProcessLog(row.LogId)
    if (response.data.Success) {
      ElMessage.success('刪除成功')
      // 重新載入數據
      loadData()
    } else {
      ElMessage.error(response.data.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除失敗:', error)
      ElMessage.error('刪除失敗')
    }
  }
}

// 分頁處理
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData()
}

// 明細分頁處理
const handleDetailSizeChange = (size) => {
  detailPagination.PageSize = size
  detailPagination.PageIndex = 1
  loadDetails()
}

const handleDetailPageChange = (page) => {
  detailPagination.PageIndex = page
  loadDetails()
}

// 明細對話框關閉
const handleDetailDialogClose = () => {
  detailData.value = []
  detailPagination.PageIndex = 1
  detailPagination.TotalCount = 0
  currentLogId.value = null
}

// 狀態類型
const getStatusType = (status) => {
  const statusMap = {
    PENDING: 'info',
    PROCESSING: 'warning',
    COMPLETED: 'success',
    FAILED: 'danger'
  }
  return statusMap[status] || 'info'
}

const getStatusText = (status) => {
  const statusMap = {
    PENDING: '待處理',
    PROCESSING: '處理中',
    COMPLETED: '已完成',
    FAILED: '失敗'
  }
  return statusMap[status] || status
}

const getFileTypeTagType = (fileType) => {
  const typeMap = {
    BACK: 'danger',
    INV: 'warning',
    ORDER: 'primary',
    ORDER_6: 'primary',
    POP: 'success',
    PRIC: 'info'
  }
  return typeMap[fileType] || 'info'
}

const getFileTypeText = (fileType) => {
  const typeMap = {
    BACK: '退貨檔',
    INV: '盤點檔',
    ORDER: '訂貨檔',
    ORDER_6: '訂貨檔(版本六)',
    POP: 'POP卡製作檔',
    PRIC: '商品卡檔'
  }
  return typeMap[fileType] || fileType
}

const getDetailStatusType = (status) => {
  const statusMap = {
    PENDING: 'info',
    SUCCESS: 'success',
    FAILED: 'danger'
  }
  return statusMap[status] || 'info'
}

const getDetailStatusText = (status) => {
  const statusMap = {
    PENDING: '待處理',
    SUCCESS: '成功',
    FAILED: '失敗'
  }
  return statusMap[status] || status
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return '-'
  return new Date(date).toLocaleString('zh-TW')
}

// 初始化
onMounted(() => {
  loadData()
})
</script>

<style scoped>
.text-file-process-log-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 600;
  color: #303133;
}

.search-card,
.table-card {
  margin-bottom: 20px;
}

.search-form {
  margin-bottom: 0;
}
</style>

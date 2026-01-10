<template>
  <div class="personnel-batch">
    <div class="page-header">
      <h1>人事批量新增 (SYSH3D0_FMI)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="匯入批次編號">
          <el-input v-model="queryForm.ImportId" placeholder="請輸入匯入批次編號" clearable />
        </el-form-item>
        <el-form-item label="匯入狀態">
          <el-select v-model="queryForm.ImportStatus" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="待處理" value="PENDING" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="成功" value="SUCCESS" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item label="匯入日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleUpload">上傳檔案</el-button>
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
        <el-table-column prop="ImportId" label="匯入批次編號" width="150" />
        <el-table-column prop="FileName" label="檔案名稱" width="200" show-overflow-tooltip />
        <el-table-column prop="TotalCount" label="總筆數" width="100" />
        <el-table-column prop="SuccessCount" label="成功筆數" width="100">
          <template #default="{ row }">
            <span class="text-success">{{ row.SuccessCount }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="FailCount" label="失敗筆數" width="100">
          <template #default="{ row }">
            <span class="text-danger">{{ row.FailCount }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="ImportStatus" label="匯入狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.ImportStatus)">
              {{ getStatusText(row.ImportStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ImportDate" label="匯入日期" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.ImportDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreatedBy" label="建立人員" width="120" />
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button 
              v-if="row.ImportStatus === 'PENDING'" 
              type="success" 
              size="small" 
              @click="handleExecute(row)"
            >
              執行匯入
            </el-button>
            <el-button 
              v-if="row.ImportStatus === 'PROCESSING'" 
              type="info" 
              size="small" 
              @click="handleCheckProgress(row)"
            >
              查看進度
            </el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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

    <!-- 上傳檔案對話框 -->
    <el-dialog
      v-model="uploadDialogVisible"
      title="上傳Excel檔案"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-upload
        ref="uploadRef"
        :auto-upload="false"
        :on-change="handleFileChange"
        :file-list="fileList"
        accept=".xlsx,.xls"
        drag
      >
        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
        <div class="el-upload__text">
          將檔案拖到此處，或<em>點擊上傳</em>
        </div>
        <template #tip>
          <div class="el-upload__tip">
            只能上傳 xlsx/xls 檔案，且不超過 10MB
          </div>
        </template>
      </el-upload>
      <template #footer>
        <el-button @click="uploadDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleUploadSubmit" :loading="uploading">確定上傳</el-button>
      </template>
    </el-dialog>

    <!-- 進度對話框 -->
    <el-dialog
      v-model="progressDialogVisible"
      title="匯入進度"
      width="500px"
      :close-on-click-modal="false"
    >
      <div v-if="progressData">
        <el-progress :percentage="progressData.Progress" :status="progressData.ImportStatus === 'SUCCESS' ? 'success' : 'active'" />
        <div style="margin-top: 20px;">
          <p>總筆數: {{ progressData.TotalCount }}</p>
          <p>已處理: {{ progressData.ProcessedCount }}</p>
          <p>成功: <span class="text-success">{{ progressData.SuccessCount }}</span></p>
          <p>失敗: <span class="text-danger">{{ progressData.FailCount }}</span></p>
        </div>
      </div>
      <template #footer>
        <el-button @click="progressDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handleRefreshProgress">重新整理</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { UploadFilled } from '@element-plus/icons-vue'
import axios from '@/api/axios'

// API
const personnelBatchApi = {
  getImportLogs: (params) => axios.get('/api/v1/personnel/batch-import/logs', { params }),
  uploadFile: (formData) => axios.post('/api/v1/personnel/batch-import/upload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  executeImport: (importId) => axios.post(`/api/v1/personnel/batch-import/${importId}/execute`),
  getProgress: (importId) => axios.get(`/api/v1/personnel/batch-import/${importId}/progress`),
  deleteImport: (importId) => axios.delete(`/api/v1/personnel/batch-import/${importId}`)
}

// 查詢表單
const queryForm = reactive({
  ImportId: '',
  ImportStatus: '',
  ImportDateFrom: null,
  ImportDateTo: null
})
const dateRange = ref(null)

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 上傳對話框
const uploadDialogVisible = ref(false)
const uploadRef = ref(null)
const fileList = ref([])
const uploading = ref(false)

// 進度對話框
const progressDialogVisible = ref(false)
const progressData = ref(null)
const currentImportId = ref(null)
let progressTimer = null

// 日期範圍變更
const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.ImportDateFrom = dates[0]
    queryForm.ImportDateTo = dates[1]
  } else {
    queryForm.ImportDateFrom = null
    queryForm.ImportDateTo = null
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ImportId: queryForm.ImportId || undefined,
      ImportStatus: queryForm.ImportStatus || undefined,
      ImportDateFrom: queryForm.ImportDateFrom || undefined,
      ImportDateTo: queryForm.ImportDateTo || undefined
    }
    const response = await personnelBatchApi.getImportLogs(params)
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
  queryForm.ImportId = ''
  queryForm.ImportStatus = ''
  queryForm.ImportDateFrom = null
  queryForm.ImportDateTo = null
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// 上傳檔案
const handleUpload = () => {
  uploadDialogVisible.value = true
  fileList.value = []
}

// 檔案變更
const handleFileChange = (file) => {
  fileList.value = [file]
}

// 提交上傳
const handleUploadSubmit = async () => {
  if (fileList.value.length === 0) {
    ElMessage.warning('請選擇要上傳的檔案')
    return
  }
  uploading.value = true
  try {
    const formData = new FormData()
    formData.append('file', fileList.value[0].raw)
    const response = await personnelBatchApi.uploadFile(formData)
    if (response.data?.success) {
      ElMessage.success('上傳成功')
      uploadDialogVisible.value = false
      fileList.value = []
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '上傳失敗')
    }
  } catch (error) {
    ElMessage.error('上傳失敗：' + error.message)
  } finally {
    uploading.value = false
  }
}

// 查看
const handleView = async (row) => {
  ElMessage.info('查看功能開發中')
}

// 執行匯入
const handleExecute = async (row) => {
  try {
    await ElMessageBox.confirm('確定要執行此批次匯入嗎？', '確認', {
      type: 'warning'
    })
    const response = await personnelBatchApi.executeImport(row.ImportId)
    if (response.data?.success) {
      ElMessage.success('執行成功')
      handleSearch()
      handleCheckProgress(row)
    } else {
      ElMessage.error(response.data?.message || '執行失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('執行失敗：' + error.message)
    }
  }
}

// 查看進度
const handleCheckProgress = async (row) => {
  currentImportId.value = row.ImportId
  progressDialogVisible.value = true
  await handleRefreshProgress()
  
  // 如果正在處理中，每3秒刷新一次
  if (row.ImportStatus === 'PROCESSING') {
    progressTimer = setInterval(async () => {
      await handleRefreshProgress()
      if (progressData.value?.ImportStatus !== 'PROCESSING') {
        clearInterval(progressTimer)
        handleSearch()
      }
    }, 3000)
  }
}

// 重新整理進度
const handleRefreshProgress = async () => {
  if (!currentImportId.value) return
  try {
    const response = await personnelBatchApi.getProgress(currentImportId.value)
    if (response.data?.success) {
      progressData.value = response.data.data
      if (progressData.value?.ImportStatus !== 'PROCESSING') {
        if (progressTimer) {
          clearInterval(progressTimer)
          progressTimer = null
        }
      }
    } else {
      ElMessage.error(response.data?.message || '查詢進度失敗')
    }
  } catch (error) {
    ElMessage.error('查詢進度失敗：' + error.message)
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此匯入記錄嗎？', '確認', {
      type: 'warning'
    })
    const response = await personnelBatchApi.deleteImport(row.ImportId)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
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

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'PENDING': 'info',
    'PROCESSING': 'warning',
    'SUCCESS': 'success',
    'FAILED': 'danger'
  }
  return types[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const texts = {
    'PENDING': '待處理',
    'PROCESSING': '處理中',
    'SUCCESS': '成功',
    'FAILED': '失敗'
  }
  return texts[status] || status
}

// 清理定時器
const cleanup = () => {
  if (progressTimer) {
    clearInterval(progressTimer)
    progressTimer = null
  }
}

// 初始化
onMounted(() => {
  handleSearch()
})

// 組件卸載時清理
import { onUnmounted } from 'vue'
onUnmounted(() => {
  cleanup()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.personnel-batch {
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

  // 文字顏色類別
  .text-success {
    color: $success-color;
  }

  .text-danger {
    color: $danger-color;
  }
}
</style>


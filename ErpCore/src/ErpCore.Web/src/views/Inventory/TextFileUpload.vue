<template>
  <div class="text-file-upload">
    <div class="page-header">
      <h1>BAT格式文本文件處理 (HT680) - 文件上傳</h1>
    </div>

    <!-- 文件上傳表單 -->
    <el-card class="upload-card" shadow="never">
      <el-form :model="uploadForm" :rules="uploadRules" ref="uploadFormRef" label-width="150px">
        <el-form-item label="文件類型" prop="fileType" required>
          <el-select v-model="uploadForm.fileType" placeholder="請選擇文件類型" style="width: 100%">
            <el-option label="退貨檔 (BACK)" value="BACK" />
            <el-option label="盤點檔 (INV)" value="INV" />
            <el-option label="訂貨檔 (ORDER)" value="ORDER" />
            <el-option label="訂貨檔版本六 (ORDER_6)" value="ORDER_6" />
            <el-option label="POP卡製作檔 (POP)" value="POP" />
            <el-option label="商品卡檔 (PRIC)" value="PRIC" />
          </el-select>
        </el-form-item>
        <el-form-item label="店別">
          <el-input v-model="uploadForm.shopId" placeholder="請輸入店別代碼（可選）" clearable />
        </el-form-item>
        <el-form-item label="選擇文件" prop="file" required>
          <el-upload
            ref="uploadRef"
            :auto-upload="false"
            :on-change="handleFileChange"
            :on-remove="handleFileRemove"
            :limit="1"
            accept=".txt"
            drag
          >
            <el-icon class="el-icon--upload"><upload-filled /></el-icon>
            <div class="el-upload__text">
              將文件拖到此處，或<em>點擊上傳</em>
            </div>
            <template #tip>
              <div class="el-upload__tip">
                支援 .txt 文件，文件大小不超過 10MB
              </div>
            </template>
          </el-upload>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleUpload" :loading="uploading">上傳並處理</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 處理進度 -->
    <el-card v-if="processLog" class="progress-card" shadow="never">
      <h3>處理進度</h3>
      <el-descriptions :column="2" border>
        <el-descriptions-item label="文件名稱">{{ processLog.FileName }}</el-descriptions-item>
        <el-descriptions-item label="文件類型">{{ processLog.FileType }}</el-descriptions-item>
        <el-descriptions-item label="店別">{{ processLog.ShopId || '-' }}</el-descriptions-item>
        <el-descriptions-item label="處理狀態">
          <el-tag :type="getStatusType(processLog.ProcessStatus)">
            {{ getStatusText(processLog.ProcessStatus) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="總記錄數">{{ processLog.TotalRecords }}</el-descriptions-item>
        <el-descriptions-item label="成功記錄數">
          <span style="color: #67c23a">{{ processLog.SuccessRecords }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="失敗記錄數">
          <span style="color: #f56c6c">{{ processLog.FailedRecords }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="處理開始時間">
          {{ formatDateTime(processLog.ProcessStartTime) }}
        </el-descriptions-item>
        <el-descriptions-item label="處理結束時間">
          {{ formatDateTime(processLog.ProcessEndTime) }}
        </el-descriptions-item>
        <el-descriptions-item label="錯誤訊息" :span="2" v-if="processLog.ErrorMessage">
          <span style="color: #f56c6c">{{ processLog.ErrorMessage }}</span>
        </el-descriptions-item>
      </el-descriptions>
      <div style="margin-top: 20px">
        <el-button type="primary" @click="handleViewDetails">查看明細</el-button>
        <el-button type="info" @click="handleDownload">下載結果</el-button>
        <el-button
          v-if="processLog.ProcessStatus === 'FAILED'"
          type="warning"
          @click="handleReprocess"
        >
          重新處理
        </el-button>
      </div>
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
import { ref, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { UploadFilled } from '@element-plus/icons-vue'
import { textFileApi } from '@/api/textFile'

// 表單引用
const uploadFormRef = ref(null)
const uploadRef = ref(null)

// 表單數據
const uploadForm = reactive({
  fileType: '',
  shopId: '',
  file: null
})

// 表單驗證規則
const uploadRules = {
  fileType: [
    { required: true, message: '請選擇文件類型', trigger: 'change' }
  ],
  file: [
    { required: true, message: '請選擇文件', trigger: 'change' }
  ]
}

// 上傳狀態
const uploading = ref(false)
const processLog = ref(null)

// 明細對話框
const detailDialogVisible = ref(false)
const detailLoading = ref(false)
const detailData = ref([])
const detailPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 文件變更處理
const handleFileChange = (file) => {
  uploadForm.file = file.raw
}

// 文件移除處理
const handleFileRemove = () => {
  uploadForm.file = null
}

// 上傳並處理
const handleUpload = async () => {
  if (!uploadFormRef.value) return

  await uploadFormRef.value.validate(async (valid) => {
    if (!valid) return

    if (!uploadForm.file) {
      ElMessage.error('請選擇文件')
      return
    }

    try {
      uploading.value = true
      const response = await textFileApi.uploadFile(
        uploadForm.file,
        uploadForm.fileType,
        uploadForm.shopId || undefined
      )

      if (response.data.Success) {
        processLog.value = response.data.Data
        ElMessage.success('文件上傳成功，處理中...')
        
        // 輪詢處理狀態
        pollProcessStatus(processLog.value.LogId)
      } else {
        ElMessage.error(response.data.Message || '上傳失敗')
      }
    } catch (error) {
      console.error('上傳失敗:', error)
      ElMessage.error(error.response?.data?.Message || '上傳失敗')
    } finally {
      uploading.value = false
    }
  })
}

// 輪詢處理狀態
const pollProcessStatus = async (logId) => {
  const maxAttempts = 60 // 最多輪詢60次
  let attempts = 0

  const poll = async () => {
    try {
      const response = await textFileApi.getProcessLogById(logId)
      if (response.data.Success) {
        processLog.value = response.data.Data
        
        // 如果還在處理中，繼續輪詢
        if (processLog.value.ProcessStatus === 'PROCESSING' && attempts < maxAttempts) {
          attempts++
          setTimeout(poll, 2000) // 每2秒輪詢一次
        }
      }
    } catch (error) {
      console.error('查詢處理狀態失敗:', error)
    }
  }

  poll()
}

// 查看明細
const handleViewDetails = async () => {
  if (!processLog.value) return

  detailDialogVisible.value = true
  await loadDetails()
}

// 載入明細
const loadDetails = async () => {
  if (!processLog.value) return

  try {
    detailLoading.value = true
    const response = await textFileApi.getProcessDetails(processLog.value.LogId, {
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
const handleDownload = async () => {
  if (!processLog.value) return

  try {
    const response = await textFileApi.downloadProcessResult(processLog.value.LogId, 'excel')
    
    // 創建下載連結
    const blob = new Blob([response.data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `處理結果_${processLog.value.LogId}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('下載成功')
  } catch (error) {
    console.error('下載失敗:', error)
    ElMessage.error('下載失敗')
  }
}

// 重新處理
const handleReprocess = async () => {
  if (!processLog.value) return

  try {
    await ElMessageBox.confirm('確定要重新處理此文件嗎？', '確認', {
      type: 'warning'
    })

    const response = await textFileApi.reprocessFile(processLog.value.LogId)
    if (response.data.Success) {
      ElMessage.success('重新處理已開始')
      // 輪詢處理狀態
      pollProcessStatus(processLog.value.LogId)
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

// 重置表單
const handleReset = () => {
  uploadFormRef.value?.resetFields()
  uploadRef.value?.clearFiles()
  uploadForm.file = null
  processLog.value = null
}

// 明細對話框關閉
const handleDetailDialogClose = () => {
  detailData.value = []
  detailPagination.PageIndex = 1
  detailPagination.TotalCount = 0
}

// 分頁處理
const handleDetailSizeChange = (size) => {
  detailPagination.PageSize = size
  detailPagination.PageIndex = 1
  loadDetails()
}

const handleDetailPageChange = (page) => {
  detailPagination.PageIndex = page
  loadDetails()
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
</script>

<style scoped>
.text-file-upload {
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

.upload-card,
.progress-card {
  margin-bottom: 20px;
}

.progress-card h3 {
  margin-bottom: 20px;
  font-size: 18px;
  font-weight: 600;
}
</style>

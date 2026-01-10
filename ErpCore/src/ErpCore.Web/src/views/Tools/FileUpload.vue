<template>
  <div class="file-upload">
    <div class="page-header">
      <h1>檔案上傳工具</h1>
    </div>

    <!-- 上傳區域 -->
    <el-card class="upload-card" shadow="never">
      <el-upload
        ref="uploadRef"
        :auto-upload="false"
        :on-change="handleFileChange"
        :on-remove="handleFileRemove"
        :file-list="fileList"
        drag
        multiple
      >
        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
        <div class="el-upload__text">
          將檔案拖到此處，或<em>點擊上傳</em>
        </div>
        <template #tip>
          <div class="el-upload__tip">
            支援多檔案上傳，單個檔案大小不超過 100MB
          </div>
        </template>
      </el-upload>
      
      <div style="margin-top: 20px; text-align: center">
        <el-button type="primary" @click="handleUpload" :loading="uploading">
          開始上傳
        </el-button>
        <el-button @click="handleClear">清空</el-button>
      </div>

      <!-- 上傳進度 -->
      <el-progress
        v-if="uploadProgress > 0 && uploadProgress < 100"
        :percentage="uploadProgress"
        :status="uploadProgress === 100 ? 'success' : 'active'"
        style="margin-top: 20px"
      />
    </el-card>

    <!-- 上傳記錄 -->
    <el-card class="table-card" shadow="never">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>上傳記錄</span>
          <el-button icon="Refresh" @click="handleRefresh">刷新</el-button>
        </div>
      </template>
      
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="FileName" label="檔案名稱" min-width="200" />
        <el-table-column prop="FileSize" label="檔案大小" width="120" align="right">
          <template #default="{ row }">
            {{ formatFileSize(row.FileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="FileType" label="檔案類型" width="120" />
        <el-table-column prop="UploadStatus" label="上傳狀態" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusTag(row.UploadStatus)">
              {{ getStatusText(row.UploadStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="UploadedAt" label="上傳時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.UploadedAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="UploadedBy" label="上傳者" width="120" />
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleDownload(row)">
              下載
            </el-button>
            <el-button type="danger" link size="small" @click="handleDelete(row)">
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { UploadFilled } from '@element-plus/icons-vue'
import { toolsApi } from '@/api/tools'

// 上傳相關
const uploadRef = ref(null)
const fileList = ref([])
const uploading = ref(false)
const uploadProgress = ref(0)

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 檔案變更
const handleFileChange = (file, files) => {
  fileList.value = files
}

// 檔案移除
const handleFileRemove = (file, files) => {
  fileList.value = files
}

// 上傳
const handleUpload = async () => {
  if (fileList.value.length === 0) {
    ElMessage.warning('請選擇要上傳的檔案')
    return
  }

  try {
    uploading.value = true
    uploadProgress.value = 0

    for (const file of fileList.value) {
      if (file.raw) {
        await toolsApi.uploadFile(file.raw, (progress) => {
          uploadProgress.value = progress
        })
      }
    }

    ElMessage.success('上傳成功')
    uploadProgress.value = 100
    fileList.value = []
    handleRefresh()
  } catch (error) {
    ElMessage.error('上傳失敗：' + (error.message || '未知錯誤'))
  } finally {
    uploading.value = false
    setTimeout(() => {
      uploadProgress.value = 0
    }, 2000)
  }
}

// 清空
const handleClear = () => {
  fileList.value = []
  uploadRef.value?.clearFiles()
}

// 刷新
const handleRefresh = async () => {
  try {
    loading.value = true
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: 'UploadedAt',
      SortOrder: 'DESC'
    }
    
    const response = await toolsApi.getUploadLogs(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 下載
const handleDownload = (row) => {
  // 實作下載邏輯
  ElMessage.info('下載功能開發中')
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此檔案嗎？', '提示', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const response = await toolsApi.deleteUploadFile(row.Id)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleRefresh()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 分頁變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleRefresh()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleRefresh()
}

// 狀態標籤
const getStatusTag = (status) => {
  const statusMap = {
    'Success': 'success',
    'Failed': 'danger',
    'Uploading': 'warning'
  }
  return statusMap[status] || 'info'
}

// 狀態文字
const getStatusText = (status) => {
  const statusMap = {
    'Success': '成功',
    'Failed': '失敗',
    'Uploading': '上傳中'
  }
  return statusMap[status] || status
}

// 格式化檔案大小
const formatFileSize = (bytes) => {
  if (!bytes) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

// 格式化日期時間
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

// 初始化
onMounted(() => {
  handleRefresh()
})
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.file-upload {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: var(--text-color-primary);
    }
  }

  .upload-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>


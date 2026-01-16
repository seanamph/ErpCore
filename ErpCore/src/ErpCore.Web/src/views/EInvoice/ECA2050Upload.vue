<template>
  <div class="eca2050-upload">
    <el-card>
      <template #header>
        <span>電子發票上傳作業 (ECA2050)</span>
      </template>
      
      <!-- 上傳區域 -->
      <el-upload
        ref="uploadRef"
        :action="uploadUrl"
        :headers="uploadHeaders"
        :data="uploadData"
        :file-list="fileList"
        :on-success="handleUploadSuccess"
        :on-error="handleUploadError"
        :on-progress="handleUploadProgress"
        :before-upload="beforeUpload"
        :limit="1"
        :auto-upload="false"
        drag
      >
        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
        <div class="el-upload__text">
          將檔案拖到此處，或<em>點擊上傳</em>
        </div>
        <template #tip>
          <div class="el-upload__tip">
            支援 Excel (.xlsx, .xls) 和 XML 格式，檔案大小不超過 50MB
          </div>
        </template>
      </el-upload>
      
      <!-- 上傳參數 -->
      <el-form :model="uploadForm" label-width="120px" style="margin-top: 20px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="店別ID">
              <el-select 
                v-model="uploadForm.storeId" 
                placeholder="請選擇店別"
                clearable
                filterable
                style="width: 100%"
              >
                <el-option
                  v-for="item in storeOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="零售商ID">
              <el-select 
                v-model="uploadForm.retailerId" 
                placeholder="請選擇零售商"
                clearable
                filterable
                style="width: 100%"
              >
                <el-option
                  v-for="item in retailerOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="說明">
          <el-input 
            v-model="uploadForm.description" 
            type="textarea"
            :rows="3"
            placeholder="請輸入說明"
            maxlength="500"
            show-word-limit
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSubmit">開始上傳</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 上傳進度 -->
      <el-progress 
        v-if="uploadProgress > 0 && uploadProgress < 100"
        :percentage="uploadProgress"
        :status="uploadStatus"
        style="margin-top: 20px"
      />
    </el-card>
    
    <!-- 上傳記錄 -->
    <el-card style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>上傳記錄</span>
          <el-button type="primary" icon="Refresh" @click="handleRefresh">刷新</el-button>
        </div>
      </template>
      
      <el-table 
        :data="uploadRecords" 
        border 
        stripe 
        style="width: 100%"
        v-loading="loading"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="FileName" label="檔案名稱" min-width="200" show-overflow-tooltip />
        <el-table-column prop="FileSize" label="檔案大小" width="120" align="right">
          <template #default="scope">
            {{ formatFileSize(scope.row.FileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="FileType" label="檔案類型" width="100" align="center" />
        <el-table-column prop="UploadDate" label="上傳時間" width="180" align="center">
          <template #default="scope">
            {{ formatDateTime(scope.row.UploadDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="UploadBy" label="上傳者" width="120" />
        <el-table-column prop="Status" label="狀態" width="120" align="center">
          <template #default="scope">
            <el-tag :type="getStatusType(scope.row.Status)">
              {{ getStatusText(scope.row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalRecords" label="總筆數" width="100" align="right" />
        <el-table-column prop="SuccessRecords" label="成功" width="100" align="right">
          <template #default="scope">
            <span style="color: #67c23a">{{ scope.row.SuccessRecords }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="FailedRecords" label="失敗" width="100" align="right">
          <template #default="scope">
            <span style="color: #f56c6c">{{ scope.row.FailedRecords }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" align="center" fixed="right">
          <template #default="scope">
            <el-button type="primary" link @click="handleView(scope.row)">查看</el-button>
            <el-button type="success" link @click="handleDownload(scope.row)">下載</el-button>
            <el-button type="danger" link @click="handleDelete(scope.row)">刪除</el-button>
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
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { UploadFilled } from '@element-plus/icons-vue'
import { einvoiceApi } from '@/api/einvoice'
import axios from '@/api/axios'

const uploadRef = ref()
const fileList = ref([])
const uploadProgress = ref(0)
const uploadStatus = ref('')
const loading = ref(false)
const uploadRecords = ref([])

const uploadForm = reactive({
  storeId: '',
  retailerId: '',
  description: ''
})

const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

const storeOptions = ref([])
const retailerOptions = ref([])

const uploadUrl = '/api/v1/einvoices/upload'
const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${localStorage.getItem('token') || ''}`
}))

const uploadData = computed(() => ({
  uploadType: 'ECA2050',
  storeId: uploadForm.storeId || undefined,
  retailerId: uploadForm.retailerId || undefined,
  description: uploadForm.description || undefined
}))

// 上傳前驗證
const beforeUpload = (file) => {
  const isValidType = ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 
                       'application/vnd.ms-excel', 
                       'text/xml', 
                       'application/xml'].includes(file.type) ||
                     ['.xlsx', '.xls', '.xml'].some(ext => file.name.toLowerCase().endsWith(ext))
  const isLt50M = file.size / 1024 / 1024 < 50

  if (!isValidType) {
    ElMessage.error('只能上傳 Excel 或 XML 格式的檔案！')
    return false
  }
  if (!isLt50M) {
    ElMessage.error('檔案大小不能超過 50MB！')
    return false
  }
  return true
}

// 上傳進度
const handleUploadProgress = (event) => {
  uploadProgress.value = Math.round(event.percent)
  uploadStatus.value = ''
}

// 上傳成功
const handleUploadSuccess = (response) => {
  if (response.Success) {
    ElMessage.success('上傳成功')
    uploadProgress.value = 100
    uploadStatus.value = 'success'
    handleRefresh()
    handleReset()
  } else {
    ElMessage.error(response.Message || '上傳失敗')
    uploadStatus.value = 'exception'
  }
}

// 上傳失敗
const handleUploadError = (error) => {
  ElMessage.error('上傳失敗：' + (error.message || '未知錯誤'))
  uploadStatus.value = 'exception'
  uploadProgress.value = 0
}

// 提交上傳
const handleSubmit = () => {
  if (fileList.value.length === 0) {
    ElMessage.warning('請選擇要上傳的檔案')
    return
  }
  uploadRef.value.submit()
}

// 重置
const handleReset = () => {
  fileList.value = []
  uploadForm.storeId = ''
  uploadForm.retailerId = ''
  uploadForm.description = ''
  uploadProgress.value = 0
  uploadStatus.value = ''
  uploadRef.value?.clearFiles()
}

// 查詢上傳記錄
const handleRefresh = async () => {
  loading.value = true
  try {
    const response = await einvoiceApi.getUploads({
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: 'UploadDate',
      SortOrder: 'DESC',
      UploadType: 'ECA2050'
    })
    
    if (response.data.Success) {
      uploadRecords.value = response.data.Data.Items
      pagination.TotalCount = response.data.Data.TotalCount
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 查看詳情
const handleView = async (row) => {
  try {
    const response = await einvoiceApi.getUpload(row.UploadId)
    if (response.data.Success) {
      ElMessageBox.alert(
        JSON.stringify(response.data.Data, null, 2),
        '上傳記錄詳情',
        { type: 'info' }
      )
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  }
}

// 下載檔案
const handleDownload = async (row) => {
  try {
    const response = await einvoiceApi.downloadUpload(row.UploadId)
    // 處理檔案下載
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', row.FileName)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    ElMessage.success('下載成功')
  } catch (error) {
    ElMessage.error('下載失敗：' + (error.message || '未知錯誤'))
  }
}

// 刪除記錄
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除上傳記錄「${row.FileName}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const response = await einvoiceApi.deleteUpload(row.UploadId)
    if (response.data.Success) {
      ElMessage.success('刪除成功')
      handleRefresh()
    } else {
      ElMessage.error(response.data.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 格式化檔案大小
const formatFileSize = (bytes) => {
  if (!bytes || bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleString('zh-TW')
}

// 取得狀態類型
const getStatusType = (status) => {
  const statusMap = {
    'PENDING': 'info',
    'PROCESSING': 'warning',
    'COMPLETED': 'success',
    'FAILED': 'danger'
  }
  return statusMap[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const statusMap = {
    'PENDING': '待處理',
    'PROCESSING': '處理中',
    'COMPLETED': '已完成',
    'FAILED': '失敗'
  }
  return statusMap[status] || status
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

// 初始化
onMounted(async () => {
  await handleRefresh()
  // TODO: 載入店別和零售商選項
})
</script>

<style lang="scss" scoped>
.eca2050-upload {
  padding: 20px;
}
</style>

<template>
  <div class="e-invoice-upload">
    <h1>電子發票上傳作業 (ECA3030)</h1>

    <!-- 上傳表單 -->
    <el-card class="upload-card" shadow="never">
      <el-form :model="uploadForm" label-width="120px">
        <el-form-item label="上傳檔案" required>
          <el-upload
            ref="uploadRef"
            :action="uploadUrl"
            :headers="uploadHeaders"
            :data="uploadData"
            :before-upload="beforeUpload"
            :on-progress="onProgress"
            :on-success="onSuccess"
            :on-error="onError"
            :file-list="fileList"
            :limit="1"
            :auto-upload="false"
            drag
            accept=".xlsx,.xls,.xml"
          >
            <el-icon class="el-icon--upload"><upload-filled /></el-icon>
            <div class="el-upload__text">
              將檔案拖到此處，或<em>點擊上傳</em>
            </div>
            <template #tip>
              <div class="el-upload__tip">
                支援 Excel (.xlsx, .xls) 和 XML (.xml) 格式，檔案大小不超過 50MB
              </div>
            </template>
          </el-upload>
        </el-form-item>

        <el-form-item label="上傳類型">
          <el-select v-model="uploadForm.UploadType" placeholder="請選擇上傳類型" style="width: 200px">
            <el-option label="ECA2050" value="ECA2050" />
            <el-option label="ECA3010" value="ECA3010" />
            <el-option label="ECA3030" value="ECA3030" />
          </el-select>
        </el-form-item>

        <el-form-item label="店別">
          <el-select
            v-model="uploadForm.StoreId"
            placeholder="請選擇店別"
            filterable
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="store in storeList"
              :key="store.StoreId"
              :label="store.StoreName"
              :value="store.StoreId"
            />
          </el-select>
        </el-form-item>

        <el-form-item label="零售商">
          <el-select
            v-model="uploadForm.RetailerId"
            placeholder="請選擇零售商"
            filterable
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="retailer in retailerList"
              :key="retailer.RetailerId"
              :label="retailer.RetailerName"
              :value="retailer.RetailerId"
            />
          </el-select>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="handleUpload" :loading="uploading">
            上傳
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>

        <el-form-item v-if="uploading">
          <el-progress :percentage="uploadProgress" :status="uploadStatus" />
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 上傳記錄列表 -->
    <el-card class="table-card" shadow="never">
      <h2>上傳記錄</h2>
      <el-table
        :data="uploadList"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="UploadId" label="上傳ID" width="100" />
        <el-table-column prop="FileName" label="檔案名稱" width="300" />
        <el-table-column prop="FileSize" label="檔案大小" width="120">
          <template #default="{ row }">
            {{ formatFileSize(row.FileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="FileType" label="檔案類型" width="120" />
        <el-table-column prop="UploadDate" label="上傳時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.UploadDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="UploadBy" label="上傳者" width="120" />
        <el-table-column prop="Status" label="狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalRecords" label="總筆數" width="100" />
        <el-table-column prop="SuccessRecords" label="成功筆數" width="100" />
        <el-table-column prop="FailedRecords" label="失敗筆數" width="100" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">
              查看
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(row)"
              :disabled="row.Status === 'PROCESSING' || row.Status === 'COMPLETED'"
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
        style="margin-top: 20px"
      />
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { UploadFilled } from '@element-plus/icons-vue'
import { einvoiceApi } from '@/api/einvoice'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

export default {
  name: 'EInvoiceUpload',
  components: {
    UploadFilled
  },
  setup() {
    const uploadRef = ref(null)
    const loading = ref(false)
    const uploading = ref(false)
    const uploadProgress = ref(0)
    const uploadStatus = ref('')
    const fileList = ref([])
    const uploadList = ref([])
    const storeList = ref([])
    const retailerList = ref([])

    const uploadForm = reactive({
      UploadType: 'ECA3030',
      StoreId: '',
      RetailerId: ''
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    const uploadUrl = ref('/api/v1/einvoices/upload')
    const uploadHeaders = ref({
      Authorization: `Bearer ${localStorage.getItem('token') || ''}`
    })
    const uploadData = ref({})

    // 上傳前驗證
    const beforeUpload = (file) => {
      const allowedExtensions = ['.xlsx', '.xls', '.xml']
      const fileExtension = file.name.substring(file.name.lastIndexOf('.')).toLowerCase()
      
      if (!allowedExtensions.includes(fileExtension)) {
        ElMessage.error('不支援的檔案格式，請上傳 Excel 或 XML 檔案')
        return false
      }

      const maxSize = 50 * 1024 * 1024 // 50MB
      if (file.size > maxSize) {
        ElMessage.error('檔案大小超過限制 (50MB)')
        return false
      }

      return true
    }

    // 上傳進度
    const onProgress = (event) => {
      uploadProgress.value = Math.round(event.percent)
      uploadStatus.value = ''
    }

    // 上傳成功
    const onSuccess = (response) => {
      uploading.value = false
      uploadProgress.value = 100
      uploadStatus.value = 'success'
      
      if (response.Success) {
        ElMessage.success('上傳成功')
        handleQuery()
        handleReset()
      } else {
        ElMessage.error(response.Message || '上傳失敗')
        uploadStatus.value = 'exception'
      }
    }

    // 上傳失敗
    const onError = (error) => {
      uploading.value = false
      uploadStatus.value = 'exception'
      ElMessage.error('上傳失敗：' + (error.message || '未知錯誤'))
    }

    // 手動上傳
    const handleUpload = () => {
      if (fileList.value.length === 0) {
        ElMessage.warning('請選擇要上傳的檔案')
        return
      }

      uploading.value = true
      uploadProgress.value = 0
      uploadStatus.value = ''

      // 準備上傳資料
      uploadData.value = {
        StoreId: uploadForm.StoreId || undefined,
        RetailerId: uploadForm.RetailerId || undefined,
        UploadType: uploadForm.UploadType || 'ECA3030'
      }

      // 觸發上傳
      uploadRef.value.submit()
    }

    // 重置
    const handleReset = () => {
      uploadForm.UploadType = 'ECA3030'
      uploadForm.StoreId = ''
      uploadForm.RetailerId = ''
      fileList.value = []
      uploadRef.value?.clearFiles()
      uploadProgress.value = 0
      uploadStatus.value = ''
    }

    // 查詢上傳記錄
    const handleQuery = async () => {
      try {
        loading.value = true
        const query = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          UploadType: 'ECA3030',
          SortField: 'UploadDate',
          SortOrder: 'DESC'
        }

        const response = await einvoiceApi.getUploads(query)
        if (response.data.Success) {
          uploadList.value = response.data.Data.Items
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

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此上傳記錄嗎？', '確認刪除', {
          type: 'warning'
        })

        const response = await einvoiceApi.deleteUpload(row.UploadId)
        if (response.data.Success) {
          ElMessage.success('刪除成功')
          handleQuery()
        } else {
          ElMessage.error(response.data.Message || '刪除失敗')
        }
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
        }
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      handleQuery()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      handleQuery()
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
      return date.toLocaleString('zh-TW')
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const statusMap = {
        COMPLETED: 'success',
        PROCESSING: 'warning',
        PENDING: 'info',
        FAILED: 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const statusMap = {
        COMPLETED: '已完成',
        PROCESSING: '處理中',
        PENDING: '待處理',
        FAILED: '失敗'
      }
      return statusMap[status] || status
    }

    // 初始化
    onMounted(() => {
      handleQuery()
      // TODO: 載入店別和零售商列表
    })

    return {
      uploadRef,
      loading,
      uploading,
      uploadProgress,
      uploadStatus,
      fileList,
      uploadList,
      storeList,
      retailerList,
      uploadForm,
      pagination,
      uploadUrl,
      uploadHeaders,
      uploadData,
      beforeUpload,
      onProgress,
      onSuccess,
      onError,
      handleUpload,
      handleReset,
      handleQuery,
      handleView,
      handleDelete,
      handleSizeChange,
      handlePageChange,
      formatFileSize,
      formatDateTime,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style scoped>
.e-invoice-upload {
  padding: 20px;
}

.upload-card {
  margin-bottom: 20px;
}

.table-card {
  margin-top: 20px;
}
</style>


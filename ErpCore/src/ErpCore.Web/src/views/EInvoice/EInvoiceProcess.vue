<template>
  <div class="einvoice-process">
    <div class="page-header">
      <h1>電子發票處理作業 (ECA3010)</h1>
    </div>

    <!-- 上傳區塊 -->
    <el-card class="upload-card" shadow="never">
      <el-upload
        ref="uploadRef"
        :action="uploadUrl"
        :headers="uploadHeaders"
        :data="uploadData"
        :on-success="handleUploadSuccess"
        :on-error="handleUploadError"
        :before-upload="beforeUpload"
        :file-list="fileList"
        :limit="1"
        accept=".xlsx,.xls,.xml"
      >
        <el-button type="primary">選擇檔案</el-button>
        <template #tip>
          <div class="el-upload__tip">
            支援 Excel (.xlsx, .xls) 和 XML 格式，檔案大小不超過 50MB
          </div>
        </template>
      </el-upload>
    </el-card>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="上傳類型">
          <el-select v-model="queryForm.UploadType" placeholder="請選擇上傳類型" clearable>
            <el-option label="ECA2050" value="ECA2050" />
            <el-option label="ECA3010" value="ECA3010" />
            <el-option label="ECA3030" value="ECA3030" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="待處理" value="PENDING" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item label="上傳者">
          <el-input v-model="queryForm.UploadBy" placeholder="請輸入上傳者" clearable />
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
        <el-table-column prop="Progress" label="進度" width="120">
          <template #default="{ row }">
            <el-progress
              :percentage="calculateProgress(row)"
              :status="row.Status === 'FAILED' ? 'exception' : ''"
            />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="success" size="small" @click="handleProcess(row)" :disabled="row.Status !== 'PENDING'">處理</el-button>
            <el-button type="info" size="small" @click="handleViewStatus(row)">狀態</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status === 'PROCESSING'">刪除</el-button>
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

    <!-- 狀態對話框 -->
    <el-dialog
      v-model="statusDialogVisible"
      title="處理狀態"
      width="600px"
    >
      <el-descriptions :column="2" border>
        <el-descriptions-item label="總筆數">{{ statusData.TotalRecords || 0 }}</el-descriptions-item>
        <el-descriptions-item label="成功筆數">{{ statusData.SuccessRecords || 0 }}</el-descriptions-item>
        <el-descriptions-item label="失敗筆數">{{ statusData.FailedRecords || 0 }}</el-descriptions-item>
        <el-descriptions-item label="進度">
          <el-progress :percentage="calculateProgress(statusData)" />
        </el-descriptions-item>
        <el-descriptions-item label="處理開始時間" :span="2">
          {{ formatDateTime(statusData.ProcessStartDate) }}
        </el-descriptions-item>
        <el-descriptions-item label="處理結束時間" :span="2">
          {{ formatDateTime(statusData.ProcessEndDate) }}
        </el-descriptions-item>
        <el-descriptions-item label="錯誤訊息" :span="2" v-if="statusData.ErrorMessage">
          <el-alert :title="statusData.ErrorMessage" type="error" :closable="false" />
        </el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { einvoiceApi } from '@/api/einvoice'

export default {
  name: 'EInvoiceProcess',
  setup() {
    const loading = ref(false)
    const statusDialogVisible = ref(false)
    const uploadRef = ref(null)
    const fileList = ref([])
    const tableData = ref([])
    const statusData = ref({})

    // 上傳設定
    const uploadUrl = computed(() => {
      return '/api/v1/einvoices/upload'
    })

    const uploadHeaders = ref({})
    const uploadData = reactive({
      UploadType: 'ECA3010',
      StoreId: '',
      RetailerId: ''
    })

    // 查詢表單
    const queryForm = reactive({
      UploadType: '',
      Status: '',
      UploadBy: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 格式化檔案大小
    const formatFileSize = (size) => {
      if (!size) return '0 B'
      const units = ['B', 'KB', 'MB', 'GB']
      let index = 0
      let fileSize = size
      while (fileSize >= 1024 && index < units.length - 1) {
        fileSize /= 1024
        index++
      }
      return `${fileSize.toFixed(2)} ${units[index]}`
    }

    // 格式化日期時間
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 計算進度
    const calculateProgress = (row) => {
      if (!row.TotalRecords || row.TotalRecords === 0) return 0
      return Math.round((row.SuccessRecords / row.TotalRecords) * 100)
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'PENDING': 'info',
        'PROCESSING': 'warning',
        'COMPLETED': 'success',
        'FAILED': 'danger'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'PENDING': '待處理',
        'PROCESSING': '處理中',
        'COMPLETED': '已完成',
        'FAILED': '失敗'
      }
      return texts[status] || status
    }

    // 上傳前檢查
    const beforeUpload = (file) => {
      const isValidType = ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel', 'text/xml', 'application/xml'].includes(file.type)
      const isLt50M = file.size / 1024 / 1024 < 50

      if (!isValidType) {
        ElMessage.error('只能上傳 Excel 或 XML 格式的檔案!')
        return false
      }
      if (!isLt50M) {
        ElMessage.error('檔案大小不能超過 50MB!')
        return false
      }
      return true
    }

    // 上傳成功
    const handleUploadSuccess = (response) => {
      if (response.Success) {
        ElMessage.success('上傳成功')
        fileList.value = []
        loadData()
      } else {
        ElMessage.error(response.Message || '上傳失敗')
      }
    }

    // 上傳失敗
    const handleUploadError = (error) => {
      ElMessage.error('上傳失敗: ' + (error.message || '未知錯誤'))
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await einvoiceApi.getUploads(params)
        if (response.Data) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 查詢
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm, {
        UploadType: '',
        Status: '',
        UploadBy: ''
      })
      handleSearch()
    }

    // 查看
    const handleView = async (row) => {
      ElMessage.info('查看功能開發中')
    }

    // 處理
    const handleProcess = async (row) => {
      try {
        await ElMessageBox.confirm('確定要開始處理此檔案嗎？', '確認', {
          type: 'warning'
        })
        await einvoiceApi.startProcess(row.UploadId)
        ElMessage.success('處理已開始')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('處理失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 查看狀態
    const handleViewStatus = async (row) => {
      try {
        const response = await einvoiceApi.getProcessStatus(row.UploadId)
        if (response.Data) {
          statusData.value = response.Data
          statusDialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('查詢狀態失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此上傳記錄嗎？', '確認', {
          type: 'warning'
        })
        await einvoiceApi.deleteUpload(row.UploadId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      statusDialogVisible,
      uploadRef,
      fileList,
      tableData,
      statusData,
      uploadUrl,
      uploadHeaders,
      uploadData,
      queryForm,
      pagination,
      formatFileSize,
      formatDateTime,
      calculateProgress,
      getStatusType,
      getStatusText,
      beforeUpload,
      handleUploadSuccess,
      handleUploadError,
      handleSearch,
      handleReset,
      handleView,
      handleProcess,
      handleViewStatus,
      handleDelete,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.einvoice-process {
  .upload-card {
    margin-bottom: 20px;
  }

  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


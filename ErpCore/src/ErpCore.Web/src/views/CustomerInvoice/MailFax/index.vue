<template>
  <div class="mail-fax">
    <div class="page-header">
      <h1>郵件傳真作業 (SYS2000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="作業編號">
          <el-input v-model="queryForm.JobId" placeholder="請輸入作業編號" clearable />
        </el-form-item>
        <el-form-item label="作業類型">
          <el-select v-model="queryForm.JobType" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="郵件" value="EMAIL" />
            <el-option label="傳真" value="FAX" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="待發送" value="PENDING" />
            <el-option label="已發送" value="SENT" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item label="發送日期">
          <el-date-picker
            v-model="dateRange"
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
          <el-button type="success" @click="handleSend">發送</el-button>
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
        <el-table-column prop="JobId" label="作業編號" width="150" />
        <el-table-column prop="JobType" label="作業類型" width="100">
          <template #default="{ row }">
            <el-tag :type="row.JobType === 'EMAIL' ? 'primary' : 'warning'">
              {{ row.JobType === 'EMAIL' ? '郵件' : '傳真' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Subject" label="主旨" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Recipient" label="收件人" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusName(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="SendDate" label="發送日期" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.SendDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="SendUser" label="發送人員" width="120" />
        <el-table-column prop="RetryCount" label="重試次數" width="100" align="center" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button
              v-if="row.Status === 'FAILED'"
              type="warning"
              size="small"
              @click="handleRetry(row)"
            >
              重試
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

    <!-- 發送對話框 -->
    <el-dialog
      v-model="sendDialogVisible"
      :title="sendDialogTitle"
      width="700px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="sendFormRef"
        :model="sendForm"
        :rules="sendFormRules"
        label-width="120px"
      >
        <el-form-item label="發送類型" prop="JobType">
          <el-radio-group v-model="sendForm.JobType" @change="handleJobTypeChange">
            <el-radio label="EMAIL">郵件</el-radio>
            <el-radio label="FAX">傳真</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="收件人" prop="Recipient">
          <el-input v-model="sendForm.Recipient" placeholder="多個收件人請用逗號分隔" />
        </el-form-item>
        <el-form-item label="副本" v-if="sendForm.JobType === 'EMAIL'">
          <el-input v-model="sendForm.Cc" placeholder="多個收件人請用逗號分隔" />
        </el-form-item>
        <el-form-item label="密件副本" v-if="sendForm.JobType === 'EMAIL'">
          <el-input v-model="sendForm.Bcc" placeholder="多個收件人請用逗號分隔" />
        </el-form-item>
        <el-form-item label="主旨" prop="Subject">
          <el-input v-model="sendForm.Subject" />
        </el-form-item>
        <el-form-item label="內容" prop="Content">
          <el-input v-model="sendForm.Content" type="textarea" :rows="6" />
        </el-form-item>
        <el-form-item label="附件">
          <el-upload
            v-model:file-list="fileList"
            action="#"
            :auto-upload="false"
            :limit="5"
            :on-exceed="handleExceed"
          >
            <el-button type="primary">選擇檔案</el-button>
            <template #tip>
              <div class="el-upload__tip">最多上傳5個檔案</div>
            </template>
          </el-upload>
        </el-form-item>
        <el-form-item label="排程發送">
          <el-switch v-model="sendForm.ScheduleEnabled" />
        </el-form-item>
        <el-form-item
          v-if="sendForm.ScheduleEnabled"
          label="排程日期"
          prop="ScheduleDate"
        >
          <el-date-picker
            v-model="sendForm.ScheduleDate"
            type="datetime"
            placeholder="選擇排程日期時間"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="sendDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleConfirmSend">確定發送</el-button>
      </template>
    </el-dialog>

    <!-- 詳情對話框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="作業詳情"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-descriptions :column="2" border v-if="currentJob">
        <el-descriptions-item label="作業編號">{{ currentJob.JobId }}</el-descriptions-item>
        <el-descriptions-item label="作業類型">
          <el-tag :type="currentJob.JobType === 'EMAIL' ? 'primary' : 'warning'">
            {{ currentJob.JobType === 'EMAIL' ? '郵件' : '傳真' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="主旨">{{ currentJob.Subject }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="getStatusType(currentJob.Status)">
            {{ getStatusName(currentJob.Status) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="收件人" :span="2">{{ currentJob.Recipient }}</el-descriptions-item>
        <el-descriptions-item label="副本" v-if="currentJob.Cc" :span="2">{{ currentJob.Cc }}</el-descriptions-item>
        <el-descriptions-item label="密件副本" v-if="currentJob.Bcc" :span="2">{{ currentJob.Bcc }}</el-descriptions-item>
        <el-descriptions-item label="發送日期">{{ formatDateTime(currentJob.SendDate) }}</el-descriptions-item>
        <el-descriptions-item label="發送人員">{{ currentJob.SendUser }}</el-descriptions-item>
        <el-descriptions-item label="重試次數">{{ currentJob.RetryCount }}/{{ currentJob.MaxRetry }}</el-descriptions-item>
        <el-descriptions-item label="錯誤訊息" v-if="currentJob.ErrorMessage" :span="2">
          <el-text type="danger">{{ currentJob.ErrorMessage }}</el-text>
        </el-descriptions-item>
        <el-descriptions-item label="內容" :span="2">
          <div style="white-space: pre-wrap">{{ currentJob.Content }}</div>
        </el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="detailDialogVisible = false">關閉</el-button>
        <el-button
          v-if="currentJob && currentJob.Status === 'FAILED'"
          type="warning"
          @click="handleRetryFromDetail"
        >
          重試
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { mailFaxApi } from '@/api/customerInvoice'

// 查詢表單
const queryForm = reactive({
  JobId: '',
  JobType: '',
  Status: '',
  SendDateFrom: '',
  SendDateTo: ''
})
const dateRange = ref(null)

// 監聽日期範圍變化
watch(dateRange, (val) => {
  if (val && val.length === 2) {
    queryForm.SendDateFrom = val[0]
    queryForm.SendDateTo = val[1]
  } else {
    queryForm.SendDateFrom = ''
    queryForm.SendDateTo = ''
  }
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

// 對話框
const sendDialogVisible = ref(false)
const detailDialogVisible = ref(false)
const currentJob = ref(null)
const sendFormRef = ref(null)
const fileList = ref([])

// 發送表單
const sendForm = reactive({
  JobType: 'EMAIL',
  Recipient: '',
  Cc: '',
  Bcc: '',
  Subject: '',
  Content: '',
  AttachmentPaths: [],
  ScheduleEnabled: false,
  ScheduleDate: null
})

const sendDialogTitle = computed(() => {
  return sendForm.JobType === 'EMAIL' ? '發送郵件' : '發送傳真'
})

const sendFormRules = {
  JobType: [{ required: true, message: '請選擇發送類型', trigger: 'change' }],
  Recipient: [{ required: true, message: '請輸入收件人', trigger: 'blur' }],
  Subject: [{ required: true, message: '請輸入主旨', trigger: 'blur' }],
  Content: [{ required: true, message: '請輸入內容', trigger: 'blur' }]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      JobId: queryForm.JobId || undefined,
      JobType: queryForm.JobType || undefined,
      Status: queryForm.Status || undefined,
      SendDateFrom: queryForm.SendDateFrom || undefined,
      SendDateTo: queryForm.SendDateTo || undefined
    }
    const response = await mailFaxApi.getEmailFaxJobs(params)
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
  queryForm.JobId = ''
  queryForm.JobType = ''
  queryForm.Status = ''
  queryForm.SendDateFrom = ''
  queryForm.SendDateTo = ''
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// 發送
const handleSend = () => {
  Object.assign(sendForm, {
    JobType: 'EMAIL',
    Recipient: '',
    Cc: '',
    Bcc: '',
    Subject: '',
    Content: '',
    AttachmentPaths: [],
    ScheduleEnabled: false,
    ScheduleDate: null
  })
  fileList.value = []
  sendDialogVisible.value = true
}

// 作業類型變更
const handleJobTypeChange = (val) => {
  if (val === 'FAX') {
    sendForm.Cc = ''
    sendForm.Bcc = ''
  }
}

// 確認發送
const handleConfirmSend = async () => {
  if (!sendFormRef.value) return
  await sendFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        // 處理附件路徑
        sendForm.AttachmentPaths = fileList.value.map(file => file.name)
        
        let response
        if (sendForm.JobType === 'EMAIL') {
          response = await mailFaxApi.sendEmail(sendForm)
        } else {
          response = await mailFaxApi.sendFax(sendForm)
        }
        
        if (response.data?.success) {
          ElMessage.success('發送成功')
          sendDialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '發送失敗')
        }
      } catch (error) {
        ElMessage.error('發送失敗：' + error.message)
      }
    }
  })
}

// 查看
const handleView = async (row) => {
  try {
    const response = await mailFaxApi.getEmailFaxJob(row.JobId)
    if (response.data?.success) {
      currentJob.value = response.data.data
      detailDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 重試
const handleRetry = async (row) => {
  try {
    await ElMessageBox.confirm('確定要重試發送此作業嗎？', '確認', {
      type: 'warning'
    })
    const response = await mailFaxApi.retryJob(row.JobId)
    if (response.data?.success) {
      ElMessage.success('重試成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '重試失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('重試失敗：' + error.message)
    }
  }
}

// 從詳情重試
const handleRetryFromDetail = async () => {
  if (currentJob.value) {
    await handleRetry(currentJob.value)
    detailDialogVisible.value = false
  }
}

// 檔案上傳超出限制
const handleExceed = () => {
  ElMessage.warning('最多只能上傳5個檔案')
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

// 取得狀態名稱
const getStatusName = (status) => {
  const statuses = {
    'PENDING': '待發送',
    'SENT': '已發送',
    'FAILED': '失敗'
  }
  return statuses[status] || status
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'PENDING': 'info',
    'SENT': 'success',
    'FAILED': 'danger'
  }
  return types[status] || ''
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.mail-fax {
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
}
</style>


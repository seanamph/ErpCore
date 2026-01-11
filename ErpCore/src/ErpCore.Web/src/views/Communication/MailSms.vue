<template>
  <div class="mail-sms">
    <div class="page-header">
      <h1>郵件簡訊發送作業 (SYS5000)</h1>
    </div>

    <!-- 發送操作 -->
    <el-card class="operation-card" shadow="never">
      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane label="發送郵件" name="email">
          <el-form :model="emailForm" :rules="emailRules" ref="emailFormRef" label-width="120px">
            <el-form-item label="收件人" prop="ToAddress">
              <el-input v-model="emailForm.ToAddress" placeholder="請輸入收件人Email，多個用逗號分隔" />
            </el-form-item>
            <el-form-item label="副本">
              <el-input v-model="emailForm.CcAddress" placeholder="請輸入副本Email，多個用逗號分隔" />
            </el-form-item>
            <el-form-item label="密件副本">
              <el-input v-model="emailForm.BccAddress" placeholder="請輸入密件副本Email，多個用逗號分隔" />
            </el-form-item>
            <el-form-item label="主旨" prop="Subject">
              <el-input v-model="emailForm.Subject" placeholder="請輸入郵件主旨" />
            </el-form-item>
            <el-form-item label="內容" prop="Body">
              <el-input
                v-model="emailForm.Body"
                type="textarea"
                :rows="8"
                placeholder="請輸入郵件內容"
              />
            </el-form-item>
            <el-form-item label="內容類型">
              <el-radio-group v-model="emailForm.BodyType">
                <el-radio label="Text">純文字</el-radio>
                <el-radio label="Html">HTML</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSendEmail" :loading="sendingEmail">
                發送郵件
              </el-button>
              <el-button @click="handleEmailReset">重置</el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="發送簡訊" name="sms">
          <el-form :model="smsForm" :rules="smsRules" ref="smsFormRef" label-width="120px">
            <el-form-item label="收件人" prop="ToPhone">
              <el-input v-model="smsForm.ToPhone" placeholder="請輸入收件人手機號碼，多個用逗號分隔" />
            </el-form-item>
            <el-form-item label="內容" prop="Message">
              <el-input
                v-model="smsForm.Message"
                type="textarea"
                :rows="6"
                placeholder="請輸入簡訊內容"
                maxlength="160"
                show-word-limit
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSendSms" :loading="sendingSms">
                發送簡訊
              </el-button>
              <el-button @click="handleSmsReset">重置</el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-card>

    <!-- 記錄查詢 -->
    <el-card class="log-card" shadow="never">
      <el-tabs v-model="logTab" @tab-change="handleLogTabChange">
        <el-tab-pane label="郵件記錄" name="email">
          <el-form :inline="true" :model="emailLogQueryForm" class="search-form">
            <el-form-item label="收件人">
              <el-input v-model="emailLogQueryForm.ToAddress" placeholder="請輸入收件人" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="emailLogQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="已發送" value="Sent" />
                <el-option label="失敗" value="Failed" />
                <el-option label="待發送" value="Pending" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleEmailLogSearch">查詢</el-button>
              <el-button @click="handleEmailLogReset">重置</el-button>
            </el-form-item>
          </el-form>

          <el-table
            :data="emailLogs"
            v-loading="emailLogLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="ToAddress" label="收件人" min-width="200" />
            <el-table-column prop="Subject" label="主旨" min-width="200" show-overflow-tooltip />
            <el-table-column prop="Status" label="狀態" width="100" align="center">
              <template #default="{ row }">
                <el-tag
                  :type="row.Status === 'Sent' ? 'success' : row.Status === 'Failed' ? 'danger' : 'warning'"
                  size="small"
                >
                  {{ row.Status === 'Sent' ? '已發送' : row.Status === 'Failed' ? '失敗' : '待發送' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="SentAt" label="發送時間" width="180">
              <template #default="{ row }">
                {{ row.SentAt ? new Date(row.SentAt).toLocaleString('zh-TW') : '' }}
              </template>
            </el-table-column>
            <el-table-column prop="ErrorMessage" label="錯誤訊息" min-width="200" show-overflow-tooltip />
          </el-table>

          <el-pagination
            v-model:current-page="emailLogPagination.PageIndex"
            v-model:page-size="emailLogPagination.PageSize"
            :total="emailLogPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleEmailLogSizeChange"
            @current-change="handleEmailLogPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>

        <el-tab-pane label="簡訊記錄" name="sms">
          <el-form :inline="true" :model="smsLogQueryForm" class="search-form">
            <el-form-item label="收件人">
              <el-input v-model="smsLogQueryForm.ToPhone" placeholder="請輸入收件人" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="smsLogQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="已發送" value="Sent" />
                <el-option label="失敗" value="Failed" />
                <el-option label="待發送" value="Pending" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSmsLogSearch">查詢</el-button>
              <el-button @click="handleSmsLogReset">重置</el-button>
            </el-form-item>
          </el-form>

          <el-table
            :data="smsLogs"
            v-loading="smsLogLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="ToPhone" label="收件人" width="150" />
            <el-table-column prop="Message" label="內容" min-width="200" show-overflow-tooltip />
            <el-table-column prop="Status" label="狀態" width="100" align="center">
              <template #default="{ row }">
                <el-tag
                  :type="row.Status === 'Sent' ? 'success' : row.Status === 'Failed' ? 'danger' : 'warning'"
                  size="small"
                >
                  {{ row.Status === 'Sent' ? '已發送' : row.Status === 'Failed' ? '失敗' : '待發送' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="SentAt" label="發送時間" width="180">
              <template #default="{ row }">
                {{ row.SentAt ? new Date(row.SentAt).toLocaleString('zh-TW') : '' }}
              </template>
            </el-table-column>
            <el-table-column prop="ErrorMessage" label="錯誤訊息" min-width="200" show-overflow-tooltip />
          </el-table>

          <el-pagination
            v-model:current-page="smsLogPagination.PageIndex"
            v-model:page-size="smsLogPagination.PageSize"
            :total="smsLogPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSmsLogSizeChange"
            @current-change="handleSmsLogPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { communicationApi } from '@/api/communication'

// 當前標籤
const activeTab = ref('email')
const logTab = ref('email')

// 郵件表單
const emailForm = reactive({
  ToAddress: '',
  CcAddress: '',
  BccAddress: '',
  Subject: '',
  Body: '',
  BodyType: 'Text'
})
const emailFormRef = ref(null)
const emailRules = {
  ToAddress: [{ required: true, message: '請輸入收件人', trigger: 'blur' }],
  Subject: [{ required: true, message: '請輸入主旨', trigger: 'blur' }],
  Body: [{ required: true, message: '請輸入內容', trigger: 'blur' }]
}
const sendingEmail = ref(false)

// 簡訊表單
const smsForm = reactive({
  ToPhone: '',
  Message: ''
})
const smsFormRef = ref(null)
const smsRules = {
  ToPhone: [{ required: true, message: '請輸入收件人', trigger: 'blur' }],
  Message: [{ required: true, message: '請輸入內容', trigger: 'blur' }]
}
const sendingSms = ref(false)

// 郵件記錄查詢
const emailLogQueryForm = reactive({
  ToAddress: '',
  Status: ''
})
const emailLogs = ref([])
const emailLogLoading = ref(false)
const emailLogPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 簡訊記錄查詢
const smsLogQueryForm = reactive({
  ToPhone: '',
  Status: ''
})
const smsLogs = ref([])
const smsLogLoading = ref(false)
const smsLogPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 發送郵件
const handleSendEmail = async () => {
  if (!emailFormRef.value) return
  await emailFormRef.value.validate(async (valid) => {
    if (valid) {
      sendingEmail.value = true
      try {
        const response = await communicationApi.sendEmail(emailForm)
        if (response.data?.success) {
          ElMessage.success('郵件發送成功')
          handleEmailReset()
          await loadEmailLogs()
        } else {
          ElMessage.error(response.data?.message || '郵件發送失敗')
        }
      } catch (error) {
        ElMessage.error('郵件發送失敗：' + (error.message || '未知錯誤'))
      } finally {
        sendingEmail.value = false
      }
    }
  })
}

// 發送簡訊
const handleSendSms = async () => {
  if (!smsFormRef.value) return
  await smsFormRef.value.validate(async (valid) => {
    if (valid) {
      sendingSms.value = true
      try {
        const response = await communicationApi.sendSms(smsForm)
        if (response.data?.success) {
          ElMessage.success('簡訊發送成功')
          handleSmsReset()
          await loadSmsLogs()
        } else {
          ElMessage.error(response.data?.message || '簡訊發送失敗')
        }
      } catch (error) {
        ElMessage.error('簡訊發送失敗：' + (error.message || '未知錯誤'))
      } finally {
        sendingSms.value = false
      }
    }
  })
}

// 郵件重置
const handleEmailReset = () => {
  emailForm.ToAddress = ''
  emailForm.CcAddress = ''
  emailForm.BccAddress = ''
  emailForm.Subject = ''
  emailForm.Body = ''
  emailForm.BodyType = 'Text'
  emailFormRef.value?.resetFields()
}

// 簡訊重置
const handleSmsReset = () => {
  smsForm.ToPhone = ''
  smsForm.Message = ''
  smsFormRef.value?.resetFields()
}

// 載入郵件記錄
const loadEmailLogs = async () => {
  emailLogLoading.value = true
  try {
    const params = {
      PageIndex: emailLogPagination.PageIndex,
      PageSize: emailLogPagination.PageSize,
      ToAddress: emailLogQueryForm.ToAddress || undefined,
      Status: emailLogQueryForm.Status || undefined
    }
    const response = await communicationApi.getEmailLogs(params)
    if (response.data?.success) {
      emailLogs.value = response.data.data?.Items || []
      emailLogPagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    emailLogLoading.value = false
  }
}

// 載入簡訊記錄
const loadSmsLogs = async () => {
  smsLogLoading.value = true
  try {
    const params = {
      PageIndex: smsLogPagination.PageIndex,
      PageSize: smsLogPagination.PageSize,
      ToPhone: smsLogQueryForm.ToPhone || undefined,
      Status: smsLogQueryForm.Status || undefined
    }
    const response = await communicationApi.getSmsLogs(params)
    if (response.data?.success) {
      smsLogs.value = response.data.data?.Items || []
      smsLogPagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    smsLogLoading.value = false
  }
}

// 郵件記錄查詢
const handleEmailLogSearch = () => {
  emailLogPagination.PageIndex = 1
  loadEmailLogs()
}

// 郵件記錄重置
const handleEmailLogReset = () => {
  emailLogQueryForm.ToAddress = ''
  emailLogQueryForm.Status = ''
  handleEmailLogSearch()
}

// 簡訊記錄查詢
const handleSmsLogSearch = () => {
  smsLogPagination.PageIndex = 1
  loadSmsLogs()
}

// 簡訊記錄重置
const handleSmsLogReset = () => {
  smsLogQueryForm.ToPhone = ''
  smsLogQueryForm.Status = ''
  handleSmsLogSearch()
}

// 郵件記錄分頁大小變更
const handleEmailLogSizeChange = (size) => {
  emailLogPagination.PageSize = size
  emailLogPagination.PageIndex = 1
  loadEmailLogs()
}

// 郵件記錄分頁變更
const handleEmailLogPageChange = (page) => {
  emailLogPagination.PageIndex = page
  loadEmailLogs()
}

// 簡訊記錄分頁大小變更
const handleSmsLogSizeChange = (size) => {
  smsLogPagination.PageSize = size
  smsLogPagination.PageIndex = 1
  loadSmsLogs()
}

// 簡訊記錄分頁變更
const handleSmsLogPageChange = (page) => {
  smsLogPagination.PageIndex = page
  loadSmsLogs()
}

// 標籤切換
const handleTabChange = (tabName) => {
  // 切換標籤時可以執行一些操作
}

// 記錄標籤切換
const handleLogTabChange = (tabName) => {
  if (tabName === 'email' && emailLogs.value.length === 0) {
    loadEmailLogs()
  } else if (tabName === 'sms' && smsLogs.value.length === 0) {
    loadSmsLogs()
  }
}

// 初始化
onMounted(() => {
  loadEmailLogs()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.mail-sms {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $primary-color;
      margin: 0;
    }
  }

  .operation-card,
  .log-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .search-form {
    margin: 0;
  }
}
</style>


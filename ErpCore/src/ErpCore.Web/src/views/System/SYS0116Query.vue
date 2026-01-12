<template>
  <div class="sys0116-query">
    <div class="page-header">
      <h1>排程修改使用者基本資料 (SYS0116)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者編號">
          <el-input v-model="queryForm.UserId" placeholder="請輸入使用者編號" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
            <el-option label="待執行" value="PENDING" />
            <el-option label="執行中" value="EXECUTING" />
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="已取消" value="CANCELLED" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item label="排程類型">
          <el-select v-model="queryForm.ScheduleType" placeholder="請選擇排程類型" clearable style="width: 150px">
            <el-option label="密碼重設" value="PASSWORD_RESET" />
            <el-option label="使用者資料更新" value="USER_UPDATE" />
            <el-option label="狀態變更" value="STATUS_CHANGE" />
          </el-select>
        </el-form-item>
        <el-form-item label="排程日期">
          <el-date-picker
            v-model="queryForm.ScheduleDateRange"
            type="datetimerange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 350px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
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
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="UserId" label="使用者編號" width="120" sortable />
        <el-table-column prop="ScheduleDate" label="排程執行時間" width="180" sortable>
          <template #default="{ row }">
            {{ formatDateTime(row.ScheduleDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ScheduleType" label="排程類型" width="150">
          <template #default="{ row }">
            {{ getScheduleTypeText(row.ScheduleType) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ExecutedAt" label="實際執行時間" width="180">
          <template #default="{ row }">
            {{ row.ExecutedAt ? formatDateTime(row.ExecutedAt) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="ErrorMessage" label="錯誤訊息" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button
              v-if="row.Status === 'PENDING'"
              type="primary"
              size="small"
              @click="handleEdit(row)"
            >
              修改
            </el-button>
            <el-button
              v-if="row.Status === 'PENDING'"
              type="warning"
              size="small"
              @click="handleCancel(row)"
            >
              取消
            </el-button>
            <el-button
              type="info"
              size="small"
              @click="handleView(row)"
            >
              查看
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

    <!-- 新增/修改對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
        <el-form-item label="使用者編號" prop="UserId">
          <el-input v-model="form.UserId" placeholder="請輸入使用者編號" :disabled="isEdit" maxlength="50" />
        </el-form-item>
        <el-form-item label="排程類型" prop="ScheduleType">
          <el-select v-model="form.ScheduleType" placeholder="請選擇排程類型" style="width: 100%" @change="handleScheduleTypeChange">
            <el-option label="密碼重設" value="PASSWORD_RESET" />
            <el-option label="使用者資料更新" value="USER_UPDATE" />
            <el-option label="狀態變更" value="STATUS_CHANGE" />
          </el-select>
        </el-form-item>
        <el-form-item label="排程執行時間" prop="ScheduleDate">
          <el-date-picker
            v-model="form.ScheduleDate"
            type="datetime"
            placeholder="請選擇執行時間"
            style="width: 100%"
            value-format="YYYY-MM-DD HH:mm:ss"
            :disabled-date="disabledDate"
          />
        </el-form-item>

        <!-- 密碼重設設定 -->
        <template v-if="form.ScheduleType === 'PASSWORD_RESET'">
          <el-form-item label="自動產生密碼">
            <el-switch v-model="form.ScheduleData.AutoGenerate" @change="handleAutoGenerateChange" />
          </el-form-item>
          <el-form-item
            v-if="!form.ScheduleData.AutoGenerate"
            label="新密碼"
            prop="ScheduleData.NewPassword"
          >
            <el-input
              v-model="form.ScheduleData.NewPassword"
              type="password"
              placeholder="請輸入新密碼"
              show-password
              maxlength="255"
            />
          </el-form-item>
        </template>

        <!-- 使用者資料更新設定 -->
        <template v-if="form.ScheduleType === 'USER_UPDATE'">
          <el-form-item label="使用者名稱">
            <el-input v-model="form.ScheduleData.UserName" placeholder="請輸入使用者名稱" maxlength="100" />
          </el-form-item>
          <el-form-item label="職稱">
            <el-input v-model="form.ScheduleData.Title" placeholder="請輸入職稱" maxlength="50" />
          </el-form-item>
          <el-form-item label="組織代碼">
            <el-input v-model="form.ScheduleData.OrgId" placeholder="請輸入組織代碼" maxlength="50" />
          </el-form-item>
          <el-form-item label="狀態">
            <el-select v-model="form.ScheduleData.Status" placeholder="請選擇狀態" style="width: 100%">
              <el-option label="啟用" value="A" />
              <el-option label="停用" value="I" />
              <el-option label="鎖定" value="L" />
            </el-select>
          </el-form-item>
          <el-form-item label="備註">
            <el-input v-model="form.ScheduleData.Notes" type="textarea" :rows="3" placeholder="請輸入備註" maxlength="500" />
          </el-form-item>
        </template>

        <!-- 狀態變更設定 -->
        <template v-if="form.ScheduleType === 'STATUS_CHANGE'">
          <el-form-item label="新狀態" prop="ScheduleData.NewStatus">
            <el-select v-model="form.ScheduleData.NewStatus" placeholder="請選擇新狀態" style="width: 100%">
              <el-option label="啟用" value="A" />
              <el-option label="停用" value="I" />
              <el-option label="鎖定" value="L" />
            </el-select>
          </el-form-item>
        </template>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 查看詳情對話框 -->
    <el-dialog
      title="排程詳情"
      v-model="viewDialogVisible"
      width="800px"
    >
      <el-descriptions :column="2" border>
        <el-descriptions-item label="排程編號">{{ viewData.ScheduleId }}</el-descriptions-item>
        <el-descriptions-item label="使用者編號">{{ viewData.UserId }}</el-descriptions-item>
        <el-descriptions-item label="排程執行時間">{{ formatDateTime(viewData.ScheduleDate) }}</el-descriptions-item>
        <el-descriptions-item label="排程類型">{{ getScheduleTypeText(viewData.ScheduleType) }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="getStatusType(viewData.Status)">{{ getStatusText(viewData.Status) }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="實際執行時間">{{ viewData.ExecutedAt ? formatDateTime(viewData.ExecutedAt) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="執行結果" :span="2">{{ viewData.ExecuteResult || '-' }}</el-descriptions-item>
        <el-descriptions-item label="錯誤訊息" :span="2">{{ viewData.ErrorMessage || '-' }}</el-descriptions-item>
        <el-descriptions-item label="建立者">{{ viewData.CreatedBy || '-' }}</el-descriptions-item>
        <el-descriptions-item label="建立時間">{{ formatDateTime(viewData.CreatedAt) }}</el-descriptions-item>
        <el-descriptions-item label="更新者">{{ viewData.UpdatedBy || '-' }}</el-descriptions-item>
        <el-descriptions-item label="更新時間">{{ formatDateTime(viewData.UpdatedAt) }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getUserSchedules,
  getUserScheduleById,
  createUserSchedule,
  updateUserSchedule,
  cancelUserSchedule
} from '@/api/userSchedules'

// 查詢表單
const queryForm = reactive({
  UserId: '',
  Status: '',
  ScheduleType: '',
  ScheduleDateRange: null
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
const dialogVisible = ref(false)
const viewDialogVisible = ref(false)
const dialogTitle = ref('新增排程')
const isEdit = ref(false)
const currentScheduleId = ref(null)
const formRef = ref(null)

// 表單資料
const form = reactive({
  UserId: '',
  ScheduleType: '',
  ScheduleDate: '',
  ScheduleData: {
    AutoGenerate: false,
    NewPassword: '',
    UserName: '',
    Title: '',
    OrgId: '',
    Status: '',
    Notes: '',
    NewStatus: ''
  }
})

// 查看資料
const viewData = ref({})

// 表單驗證規則
const rules = {
  UserId: [
    { required: true, message: '請輸入使用者編號', trigger: 'blur' }
  ],
  ScheduleType: [
    { required: true, message: '請選擇排程類型', trigger: 'change' }
  ],
  ScheduleDate: [
    { required: true, message: '請選擇排程執行時間', trigger: 'change' }
  ],
  'ScheduleData.NewPassword': [
    {
      validator: (rule, value, callback) => {
        if (form.ScheduleType === 'PASSWORD_RESET' && !form.ScheduleData.AutoGenerate && !value) {
          callback(new Error('請輸入新密碼或選擇自動產生'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  'ScheduleData.NewStatus': [
    {
      validator: (rule, value, callback) => {
        if (form.ScheduleType === 'STATUS_CHANGE' && !value) {
          callback(new Error('請選擇新狀態'))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      UserId: queryForm.UserId || undefined,
      Status: queryForm.Status || undefined,
      ScheduleType: queryForm.ScheduleType || undefined,
      ScheduleDateFrom: queryForm.ScheduleDateRange ? queryForm.ScheduleDateRange[0] : undefined,
      ScheduleDateTo: queryForm.ScheduleDateRange ? queryForm.ScheduleDateRange[1] : undefined
    }

    const response = await getUserSchedules(params)
    if (response && response.Data) {
      tableData.value = response.Data.Items || []
      pagination.TotalCount = response.Data.TotalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.UserId = ''
  queryForm.Status = ''
  queryForm.ScheduleType = ''
  queryForm.ScheduleDateRange = null
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增排程'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = (row) => {
  if (row.Status !== 'PENDING') {
    ElMessage.warning('僅待執行狀態的排程可修改')
    return
  }

  isEdit.value = true
  currentScheduleId.value = row.ScheduleId
  dialogTitle.value = '修改排程'
  loadFormData(row)
  dialogVisible.value = true
}

// 取消
const handleCancel = async (row) => {
  if (row.Status !== 'PENDING') {
    ElMessage.warning('僅待執行狀態的排程可取消')
    return
  }

  try {
    await ElMessageBox.confirm('確定要取消此排程嗎？', '提示', {
      type: 'warning'
    })

    await cancelUserSchedule(row.ScheduleId)
    ElMessage.success('取消排程成功')
    handleSearch()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('取消排程失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 查看
const handleView = async (row) => {
  try {
    const response = await getUserScheduleById(row.ScheduleId)
    if (response && response.Data) {
      viewData.value = response.Data
      viewDialogVisible.value = true
    }
  } catch (error) {
    ElMessage.error('查詢排程詳情失敗: ' + (error.message || '未知錯誤'))
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (!valid) return

    try {
      const scheduleData = { ...form.ScheduleData }
      
      // 根據排程類型清理不需要的資料
      if (form.ScheduleType === 'PASSWORD_RESET') {
        if (scheduleData.AutoGenerate) {
          scheduleData.NewPassword = ''
        }
      } else if (form.ScheduleType === 'USER_UPDATE') {
        // 只保留有值的欄位
        Object.keys(scheduleData).forEach(key => {
          if (!scheduleData[key] && key !== 'Notes') {
            delete scheduleData[key]
          }
        })
      } else if (form.ScheduleType === 'STATUS_CHANGE') {
        // 只保留 NewStatus
        Object.keys(scheduleData).forEach(key => {
          if (key !== 'NewStatus') {
            delete scheduleData[key]
          }
        })
      }

      const data = {
        UserId: form.UserId,
        ScheduleType: form.ScheduleType,
        ScheduleDate: form.ScheduleDate,
        ScheduleData: scheduleData
      }

      if (isEdit.value) {
        if (!currentScheduleId.value) {
          ElMessage.error('無法取得排程編號')
          return
        }
        await updateUserSchedule(currentScheduleId.value, data)
        ElMessage.success('修改排程成功')
      } else {
        await createUserSchedule(data)
        ElMessage.success('新增排程成功')
      }

      dialogVisible.value = false
      handleSearch()
    } catch (error) {
      ElMessage.error((isEdit.value ? '修改' : '新增') + '排程失敗: ' + (error.message || '未知錯誤'))
    }
  })
}

// 對話框關閉
const handleDialogClose = () => {
  formRef.value?.resetFields()
  resetForm()
  currentScheduleId.value = null
}

// 重置表單
const resetForm = () => {
  form.UserId = ''
  form.ScheduleType = ''
  form.ScheduleDate = ''
  form.ScheduleData = {
    AutoGenerate: false,
    NewPassword: '',
    UserName: '',
    Title: '',
    OrgId: '',
    Status: '',
    Notes: '',
    NewStatus: ''
  }
}

// 載入表單資料
const loadFormData = (row) => {
  form.UserId = row.UserId
  form.ScheduleType = row.ScheduleType
  form.ScheduleDate = row.ScheduleDate

  // 解析 ScheduleData (JSON)
  if (row.ScheduleData) {
    try {
      const scheduleData = JSON.parse(row.ScheduleData)
      Object.assign(form.ScheduleData, scheduleData)
    } catch (e) {
      console.error('解析排程資料失敗:', e)
    }
  }
}

// 排程類型變更
const handleScheduleTypeChange = () => {
  // 重置排程資料
  form.ScheduleData = {
    AutoGenerate: false,
    NewPassword: '',
    UserName: '',
    Title: '',
    OrgId: '',
    Status: '',
    Notes: '',
    NewStatus: ''
  }
}

// 自動產生變更
const handleAutoGenerateChange = (value) => {
  if (value) {
    form.ScheduleData.NewPassword = ''
  }
}

// 禁用過去的日期
const disabledDate = (time) => {
  return time.getTime() < Date.now() - 8.64e7 // 禁用今天之前的日期
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

// 取得狀態類型
const getStatusType = (status) => {
  switch (status) {
    case 'PENDING':
      return 'warning'
    case 'EXECUTING':
      return 'primary'
    case 'COMPLETED':
      return 'success'
    case 'CANCELLED':
      return 'info'
    case 'FAILED':
      return 'danger'
    default:
      return 'info'
  }
}

// 取得狀態文字
const getStatusText = (status) => {
  switch (status) {
    case 'PENDING':
      return '待執行'
    case 'EXECUTING':
      return '執行中'
    case 'COMPLETED':
      return '已完成'
    case 'CANCELLED':
      return '已取消'
    case 'FAILED':
      return '失敗'
    default:
      return status
  }
}

// 取得排程類型文字
const getScheduleTypeText = (type) => {
  switch (type) {
    case 'PASSWORD_RESET':
      return '密碼重設'
    case 'USER_UPDATE':
      return '使用者資料更新'
    case 'STATUS_CHANGE':
      return '狀態變更'
    default:
      return type
  }
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return '-'
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
  handleSearch()
})
</script>

<style scoped>
.sys0116-query {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 500;
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

<template>
  <div class="sys0110-query">
    <div class="page-header">
      <h1>使用者基本資料維護 (SYS0110)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者編號">
          <el-input v-model="queryForm.UserId" placeholder="請輸入使用者編號" clearable />
        </el-form-item>
        <el-form-item label="使用者名稱">
          <el-input v-model="queryForm.UserName" placeholder="請輸入使用者名稱" clearable />
        </el-form-item>
        <el-form-item label="組織">
          <el-input v-model="queryForm.OrgId" placeholder="請輸入組織代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="鎖定" value="L" />
          </el-select>
        </el-form-item>
        <el-form-item label="使用者型態">
          <el-input v-model="queryForm.UserType" placeholder="請輸入使用者型態" clearable />
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="UserId" label="使用者編號" width="120" sortable />
        <el-table-column prop="UserName" label="使用者名稱" width="150" sortable />
        <el-table-column prop="Title" label="職稱" width="100" />
        <el-table-column prop="OrgId" label="組織代碼" width="120" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="UserType" label="使用者型態" width="120" />
        <el-table-column prop="LastLoginDate" label="最後登入" width="160">
          <template #default="{ row }">
            {{ row.LastLoginDate ? formatDateTime(row.LastLoginDate) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="LoginCount" label="登入次數" width="100" align="center" />
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
            <el-button type="warning" size="small" @click="handleResetPassword(row)">重設密碼</el-button>
            <el-button type="info" size="small" @click="handleToggleStatus(row)">
              {{ row.Status === 'A' ? '停用' : '啟用' }}
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

      <!-- 批次操作按鈕 -->
      <div style="margin-top: 10px">
        <el-button type="danger" @click="handleBatchDelete" :disabled="selectedRows.length === 0">
          批次刪除
        </el-button>
      </div>
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
        <el-form-item label="使用者名稱" prop="UserName">
          <el-input v-model="form.UserName" placeholder="請輸入使用者名稱" maxlength="100" />
        </el-form-item>
        <el-form-item label="密碼" prop="UserPassword" v-if="!isEdit">
          <el-input v-model="form.UserPassword" type="password" placeholder="請輸入密碼" show-password maxlength="255" />
        </el-form-item>
        <el-form-item label="職稱" prop="Title">
          <el-input v-model="form.Title" placeholder="請輸入職稱" maxlength="50" />
        </el-form-item>
        <el-form-item label="組織代碼" prop="OrgId">
          <el-input v-model="form.OrgId" placeholder="請輸入組織代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="有效起始日" prop="StartDate">
          <el-date-picker
            v-model="form.StartDate"
            type="date"
            placeholder="請選擇日期"
            style="width: 100%"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="有效終止日" prop="EndDate">
          <el-date-picker
            v-model="form.EndDate"
            type="date"
            placeholder="請選擇日期"
            style="width: 100%"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-radio-group v-model="form.Status">
            <el-radio label="A">啟用</el-radio>
            <el-radio label="I">停用</el-radio>
            <el-radio label="L">鎖定</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="使用者型態" prop="UserType">
          <el-input v-model="form.UserType" placeholder="請輸入使用者型態" maxlength="20" />
        </el-form-item>
        <el-form-item label="使用者等級" prop="UserPriority">
          <el-input-number v-model="form.UserPriority" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="所屬分店" prop="ShopId">
          <el-input v-model="form.ShopId" placeholder="請輸入所屬分店" maxlength="50" />
        </el-form-item>
        <el-form-item label="樓層代碼" prop="FloorId">
          <el-input v-model="form.FloorId" placeholder="請輸入樓層代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="區域別代碼" prop="AreaId">
          <el-input v-model="form.AreaId" placeholder="請輸入區域別代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="業種代碼" prop="BtypeId">
          <el-input v-model="form.BtypeId" placeholder="請輸入業種代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="專櫃代碼" prop="StoreId">
          <el-input v-model="form.StoreId" placeholder="請輸入專櫃代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" maxlength="500" show-word-limit />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 重設密碼對話框 -->
    <el-dialog
      title="重設密碼"
      v-model="resetPasswordDialogVisible"
      width="500px"
      @close="handleResetPasswordDialogClose"
    >
      <el-form :model="resetPasswordForm" :rules="resetPasswordRules" ref="resetPasswordFormRef" label-width="120px">
        <el-form-item label="使用者編號">
          <el-input v-model="resetPasswordForm.UserId" disabled />
        </el-form-item>
        <el-form-item label="使用者名稱">
          <el-input v-model="resetPasswordForm.UserName" disabled />
        </el-form-item>
        <el-form-item label="新密碼" prop="NewPassword">
          <el-input v-model="resetPasswordForm.NewPassword" type="password" placeholder="請輸入新密碼" show-password maxlength="255" />
        </el-form-item>
        <el-form-item label="確認密碼" prop="ConfirmPassword">
          <el-input v-model="resetPasswordForm.ConfirmPassword" type="password" placeholder="請再次輸入新密碼" show-password maxlength="255" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleResetPasswordDialogClose">取消</el-button>
        <el-button type="primary" @click="handleResetPasswordSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getUsers,
  getUserById,
  createUser,
  updateUser,
  deleteUser,
  deleteUsersBatch,
  resetPassword,
  updateStatus
} from '@/api/users'

// 查詢表單
const queryForm = reactive({
  UserId: '',
  UserName: '',
  OrgId: '',
  Status: '',
  UserType: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增使用者')
const isEdit = ref(false)
const formRef = ref(null)

// 重設密碼對話框
const resetPasswordDialogVisible = ref(false)
const resetPasswordFormRef = ref(null)
const resetPasswordForm = reactive({
  UserId: '',
  UserName: '',
  NewPassword: '',
  ConfirmPassword: ''
})

// 表單資料
const form = reactive({
  UserId: '',
  UserName: '',
  UserPassword: '',
  Title: '',
  OrgId: '',
  StartDate: null,
  EndDate: null,
  Status: 'A',
  UserType: '',
  UserPriority: 0,
  ShopId: '',
  FloorId: '',
  AreaId: '',
  BtypeId: '',
  StoreId: '',
  Notes: ''
})

// 表單驗證規則
const rules = {
  UserId: [
    { required: true, message: '請輸入使用者編號', trigger: 'blur' },
    { max: 50, message: '使用者編號長度不能超過50個字元', trigger: 'blur' }
  ],
  UserName: [
    { required: true, message: '請輸入使用者名稱', trigger: 'blur' },
    { max: 100, message: '使用者名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  UserPassword: [
    { required: true, message: '請輸入密碼', trigger: 'blur', validator: (rule, value, callback) => {
      if (!isEdit.value && !value) {
        callback(new Error('請輸入密碼'))
      } else {
        callback()
      }
    } }
  ],
  Status: [
    { required: true, message: '請選擇狀態', trigger: 'change' }
  ]
}

// 重設密碼表單驗證規則
const resetPasswordRules = {
  NewPassword: [
    { required: true, message: '請輸入新密碼', trigger: 'blur' },
    { min: 6, message: '密碼長度不能少於6個字元', trigger: 'blur' }
  ],
  ConfirmPassword: [
    { required: true, message: '請再次輸入新密碼', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== resetPasswordForm.NewPassword) {
          callback(new Error('兩次輸入的密碼不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 取得狀態類型
const getStatusType = (status) => {
  switch (status) {
    case 'A':
      return 'success'
    case 'I':
      return 'danger'
    case 'L':
      return 'warning'
    default:
      return 'info'
  }
}

// 取得狀態文字
const getStatusText = (status) => {
  switch (status) {
    case 'A':
      return '啟用'
    case 'I':
      return '停用'
    case 'L':
      return '鎖定'
    default:
      return status
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

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      UserId: queryForm.UserId || undefined,
      UserName: queryForm.UserName || undefined,
      OrgId: queryForm.OrgId || undefined,
      Status: queryForm.Status || undefined,
      UserType: queryForm.UserType || undefined
    }
    const response = await getUsers(params)
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

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadData()
}

// 重置
const handleReset = () => {
  Object.assign(queryForm, {
    UserId: '',
    UserName: '',
    OrgId: '',
    Status: '',
    UserType: ''
  })
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增使用者'
  Object.assign(form, {
    UserId: '',
    UserName: '',
    UserPassword: '',
    Title: '',
    OrgId: '',
    StartDate: null,
    EndDate: null,
    Status: 'A',
    UserType: '',
    UserPriority: 0,
    ShopId: '',
    FloorId: '',
    AreaId: '',
    BtypeId: '',
    StoreId: '',
    Notes: ''
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改使用者'
  try {
    const response = await getUserById(row.UserId)
    if (response && response.Data) {
      Object.assign(form, {
        UserId: response.Data.UserId,
        UserName: response.Data.UserName,
        UserPassword: '', // 不顯示密碼
        Title: response.Data.Title || '',
        OrgId: response.Data.OrgId || '',
        StartDate: response.Data.StartDate || null,
        EndDate: response.Data.EndDate || null,
        Status: response.Data.Status || 'A',
        UserType: response.Data.UserType || '',
        UserPriority: response.Data.UserPriority || 0,
        ShopId: response.Data.ShopId || '',
        FloorId: response.Data.FloorId || '',
        AreaId: response.Data.AreaId || '',
        BtypeId: response.Data.BtypeId || '',
        StoreId: response.Data.StoreId || '',
        Notes: response.Data.Notes || ''
      })
      dialogVisible.value = true
    }
  } catch (error) {
    ElMessage.error('載入資料失敗: ' + (error.message || '未知錯誤'))
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(`確定要刪除使用者「${row.UserName}」嗎？`, '確認刪除', {
      type: 'warning'
    })
    await deleteUser(row.UserId)
    ElMessage.success('刪除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 批次刪除
const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇要刪除的資料')
    return
  }
  try {
    await ElMessageBox.confirm(`確定要刪除選取的 ${selectedRows.value.length} 筆資料嗎？`, '確認批次刪除', {
      type: 'warning'
    })
    const userIds = selectedRows.value.map(row => row.UserId)
    await deleteUsersBatch({ UserIds: userIds })
    ElMessage.success('批次刪除成功')
    selectedRows.value = []
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 重設密碼
const handleResetPassword = (row) => {
  resetPasswordForm.UserId = row.UserId
  resetPasswordForm.UserName = row.UserName
  resetPasswordForm.NewPassword = ''
  resetPasswordForm.ConfirmPassword = ''
  resetPasswordDialogVisible.value = true
}

// 重設密碼提交
const handleResetPasswordSubmit = async () => {
  try {
    await resetPasswordFormRef.value.validate()
    await resetPassword(resetPasswordForm.UserId, { NewPassword: resetPasswordForm.NewPassword })
    ElMessage.success('重設密碼成功')
    resetPasswordDialogVisible.value = false
  } catch (error) {
    if (error !== false) {
      ElMessage.error('重設密碼失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 關閉重設密碼對話框
const handleResetPasswordDialogClose = () => {
  resetPasswordFormRef.value?.resetFields()
  resetPasswordDialogVisible.value = false
}

// 切換狀態
const handleToggleStatus = async (row) => {
  try {
    const newStatus = row.Status === 'A' ? 'I' : 'A'
    const statusText = newStatus === 'A' ? '啟用' : '停用'
    await ElMessageBox.confirm(`確定要${statusText}使用者「${row.UserName}」嗎？`, `確認${statusText}`, {
      type: 'warning'
    })
    await updateStatus(row.UserId, { Status: newStatus })
    ElMessage.success(`${statusText}成功`)
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('更新狀態失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 提交
const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    if (isEdit.value) {
      const updateData = {
        UserName: form.UserName,
        Title: form.Title,
        OrgId: form.OrgId,
        StartDate: form.StartDate || undefined,
        EndDate: form.EndDate || undefined,
        Status: form.Status,
        UserType: form.UserType,
        UserPriority: form.UserPriority,
        ShopId: form.ShopId,
        FloorId: form.FloorId,
        AreaId: form.AreaId,
        BtypeId: form.BtypeId,
        StoreId: form.StoreId,
        Notes: form.Notes
      }
      await updateUser(form.UserId, updateData)
      ElMessage.success('修改成功')
    } else {
      await createUser(form)
      ElMessage.success('新增成功')
    }
    dialogVisible.value = false
    loadData()
  } catch (error) {
    if (error !== false) {
      ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 關閉對話框
const handleDialogClose = () => {
  formRef.value?.resetFields()
  dialogVisible.value = false
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData()
}

// 初始化
onMounted(() => {
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0110-query {
  .page-header {
    h1 {
      color: $primary-color;
    }
  }
  
  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>

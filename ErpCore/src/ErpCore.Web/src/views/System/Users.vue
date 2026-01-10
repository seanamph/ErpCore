<template>
  <div class="users">
    <div class="page-header">
      <h1>使用者基本資料維護 (SYS0110)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者代碼">
          <el-input v-model="queryForm.UserId" placeholder="請輸入使用者代碼" clearable />
        </el-form-item>
        <el-form-item label="使用者名稱">
          <el-input v-model="queryForm.UserName" placeholder="請輸入使用者名稱" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="鎖定" value="L" />
          </el-select>
        </el-form-item>
        <el-form-item label="組織代碼">
          <el-input v-model="queryForm.OrgId" placeholder="請輸入組織代碼" clearable />
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
        <el-table-column prop="UserId" label="使用者代碼" width="150" />
        <el-table-column prop="UserName" label="使用者名稱" width="200" />
        <el-table-column prop="Title" label="職稱" width="150" />
        <el-table-column prop="OrgId" label="組織代碼" width="150" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : row.Status === 'I' ? 'danger' : 'warning'">
              {{ row.Status === 'A' ? '啟用' : row.Status === 'I' ? '停用' : '鎖定' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="UserType" label="使用者類型" width="150" />
        <el-table-column prop="ShopId" label="店別代碼" width="150" />
        <el-table-column prop="LastLoginDate" label="最後登入日期" width="180">
          <template #default="{ row }">
            {{ row.LastLoginDate ? new Date(row.LastLoginDate).toLocaleString('zh-TW') : '' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="warning" size="small" @click="handleChangePassword(row)">變更密碼</el-button>
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

    <!-- 新增/修改對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="700px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="使用者代碼" prop="UserId">
          <el-input v-model="form.UserId" placeholder="請輸入使用者代碼" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="使用者名稱" prop="UserName">
          <el-input v-model="form.UserName" placeholder="請輸入使用者名稱" />
        </el-form-item>
        <el-form-item label="密碼" prop="UserPassword" v-if="!isEdit">
          <el-input v-model="form.UserPassword" type="password" placeholder="請輸入密碼" show-password />
        </el-form-item>
        <el-form-item label="職稱" prop="Title">
          <el-input v-model="form.Title" placeholder="請輸入職稱" />
        </el-form-item>
        <el-form-item label="組織代碼" prop="OrgId">
          <el-input v-model="form.OrgId" placeholder="請輸入組織代碼" />
        </el-form-item>
        <el-form-item label="帳號有效起始日" prop="StartDate">
          <el-date-picker v-model="form.StartDate" type="date" placeholder="請選擇日期" style="width: 100%" />
        </el-form-item>
        <el-form-item label="帳號終止日" prop="EndDate">
          <el-date-picker v-model="form.EndDate" type="date" placeholder="請選擇日期" style="width: 100%" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="鎖定" value="L" />
          </el-select>
        </el-form-item>
        <el-form-item label="使用者類型" prop="UserType">
          <el-input v-model="form.UserType" placeholder="請輸入使用者類型" />
        </el-form-item>
        <el-form-item label="店別代碼" prop="ShopId">
          <el-input v-model="form.ShopId" placeholder="請輸入店別代碼" />
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 變更密碼對話框 -->
    <el-dialog
      title="變更密碼"
      v-model="passwordDialogVisible"
      width="500px"
      @close="handlePasswordDialogClose"
    >
      <el-form :model="passwordForm" :rules="passwordRules" ref="passwordFormRef" label-width="120px">
        <el-form-item label="新密碼" prop="NewPassword">
          <el-input v-model="passwordForm.NewPassword" type="password" placeholder="請輸入新密碼" show-password />
        </el-form-item>
        <el-form-item label="確認密碼" prop="ConfirmPassword">
          <el-input v-model="passwordForm.ConfirmPassword" type="password" placeholder="請再次輸入新密碼" show-password />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handlePasswordDialogClose">取消</el-button>
        <el-button type="primary" @click="handlePasswordSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getUsers, getUserById, createUser, updateUser, deleteUser, changePassword } from '@/api/users'

// 查詢表單
const queryForm = reactive({
  UserId: '',
  UserName: '',
  Status: '',
  OrgId: ''
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
const passwordDialogVisible = ref(false)
const dialogTitle = ref('新增使用者')
const isEdit = ref(false)
const formRef = ref(null)
const passwordFormRef = ref(null)
const currentUserId = ref('')

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
  ShopId: '',
  Notes: ''
})

// 密碼表單
const passwordForm = reactive({
  NewPassword: '',
  ConfirmPassword: ''
})

// 表單驗證規則
const rules = {
  UserId: [{ required: true, message: '請輸入使用者代碼', trigger: 'blur' }],
  UserName: [{ required: true, message: '請輸入使用者名稱', trigger: 'blur' }],
  UserPassword: [{ required: true, message: '請輸入密碼', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 密碼驗證規則
const passwordRules = {
  NewPassword: [{ required: true, message: '請輸入新密碼', trigger: 'blur' }],
  ConfirmPassword: [
    { required: true, message: '請確認密碼', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== passwordForm.NewPassword) {
          callback(new Error('兩次輸入的密碼不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
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
      UserName: queryForm.UserName || undefined,
      Status: queryForm.Status || undefined,
      OrgId: queryForm.OrgId || undefined
    }
    const response = await getUsers(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.TotalCount = response.data.data.totalCount
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.UserId = ''
  queryForm.UserName = ''
  queryForm.Status = ''
  queryForm.OrgId = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增使用者'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改使用者'
  try {
    const response = await getUserById(row.UserId)
    if (response.data.success) {
      Object.assign(form, response.data.data)
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
  dialogVisible.value = true
}

// 變更密碼
const handleChangePassword = (row) => {
  currentUserId.value = row.UserId
  passwordForm.NewPassword = ''
  passwordForm.ConfirmPassword = ''
  passwordDialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此使用者嗎？', '提示', {
      type: 'warning'
    })
    const response = await deleteUser(row.UserId)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          response = await updateUser(form.UserId, form)
        } else {
          response = await createUser(form)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + error.message)
      }
    }
  })
}

// 密碼提交
const handlePasswordSubmit = async () => {
  if (!passwordFormRef.value) return
  await passwordFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await changePassword(currentUserId.value, {
          NewPassword: passwordForm.NewPassword
        })
        if (response.data.success) {
          ElMessage.success('密碼變更成功')
          passwordDialogVisible.value = false
        } else {
          ElMessage.error(response.data.message || '密碼變更失敗')
        }
      } catch (error) {
        ElMessage.error('密碼變更失敗：' + error.message)
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 關閉密碼對話框
const handlePasswordDialogClose = () => {
  passwordDialogVisible.value = false
  passwordForm.NewPassword = ''
  passwordForm.ConfirmPassword = ''
  passwordFormRef.value?.resetFields()
}

// 重置表單
const resetForm = () => {
  form.UserId = ''
  form.UserName = ''
  form.UserPassword = ''
  form.Title = ''
  form.OrgId = ''
  form.StartDate = null
  form.EndDate = null
  form.Status = 'A'
  form.UserType = ''
  form.ShopId = ''
  form.Notes = ''
  formRef.value?.resetFields()
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

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.users {
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
    background-color: $card-bg;
    border-color: $card-border;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
  }
}
</style>


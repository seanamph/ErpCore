<template>
  <div class="sys0210-query">
    <div class="page-header">
      <h1>角色基本資料維護 (SYS0210)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="角色代碼">
          <el-input v-model="queryForm.RoleId" placeholder="請輸入角色代碼" clearable />
        </el-form-item>
        <el-form-item label="角色名稱">
          <el-input v-model="queryForm.RoleName" placeholder="請輸入角色名稱" clearable />
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
        <el-table-column prop="RoleId" label="角色代碼" width="150" sortable />
        <el-table-column prop="RoleName" label="角色名稱" width="200" sortable />
        <el-table-column prop="RoleNote" label="角色敘述" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedBy" label="建立者" width="120" />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ row.CreatedAt ? formatDateTime(row.CreatedAt) : '' }}
          </template>
        </el-table-column>
        <el-table-column prop="UpdatedBy" label="更新者" width="120" />
        <el-table-column prop="UpdatedAt" label="更新時間" width="180">
          <template #default="{ row }">
            {{ row.UpdatedAt ? formatDateTime(row.UpdatedAt) : '' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="warning" size="small" @click="handleCopy(row)">複製</el-button>
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
      width="600px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="角色代碼" prop="RoleId">
          <el-input v-model="form.RoleId" placeholder="請輸入角色代碼" :disabled="isEdit" maxlength="50" />
        </el-form-item>
        <el-form-item label="角色名稱" prop="RoleName">
          <el-input v-model="form.RoleName" placeholder="請輸入角色名稱" maxlength="100" />
        </el-form-item>
        <el-form-item label="角色敘述" prop="RoleNote">
          <el-input v-model="form.RoleNote" type="textarea" :rows="3" placeholder="請輸入角色敘述" maxlength="500" show-word-limit />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 複製角色對話框 -->
    <el-dialog
      title="複製角色"
      v-model="copyDialogVisible"
      width="500px"
      @close="handleCopyDialogClose"
    >
      <el-form :model="copyForm" :rules="copyRules" ref="copyFormRef" label-width="120px">
        <el-form-item label="新角色代碼" prop="NewRoleId">
          <el-input v-model="copyForm.NewRoleId" placeholder="請輸入新角色代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="新角色名稱" prop="NewRoleName">
          <el-input v-model="copyForm.NewRoleName" placeholder="請輸入新角色名稱" maxlength="100" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleCopyDialogClose">取消</el-button>
        <el-button type="primary" @click="handleCopySubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { rolesApi } from '@/api/roles'

// 查詢表單
const queryForm = reactive({
  RoleId: '',
  RoleName: ''
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
const copyDialogVisible = ref(false)
const dialogTitle = ref('新增角色')
const isEdit = ref(false)
const formRef = ref(null)
const copyFormRef = ref(null)
const currentRoleId = ref('')

// 表單資料
const form = reactive({
  RoleId: '',
  RoleName: '',
  RoleNote: ''
})

// 複製表單
const copyForm = reactive({
  NewRoleId: '',
  NewRoleName: ''
})

// 表單驗證規則
const rules = {
  RoleId: [
    { required: true, message: '請輸入角色代碼', trigger: 'blur' },
    { max: 50, message: '角色代碼長度不能超過50字元', trigger: 'blur' }
  ],
  RoleName: [
    { required: true, message: '請輸入角色名稱', trigger: 'blur' },
    { max: 100, message: '角色名稱長度不能超過100字元', trigger: 'blur' }
  ],
  RoleNote: [
    { max: 500, message: '角色敘述長度不能超過500字元', trigger: 'blur' }
  ]
}

// 複製驗證規則
const copyRules = {
  NewRoleId: [
    { required: true, message: '請輸入新角色代碼', trigger: 'blur' },
    { max: 50, message: '角色代碼長度不能超過50字元', trigger: 'blur' }
  ],
  NewRoleName: [
    { required: true, message: '請輸入新角色名稱', trigger: 'blur' },
    { max: 100, message: '角色名稱長度不能超過100字元', trigger: 'blur' }
  ]
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

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      RoleId: queryForm.RoleId || undefined,
      RoleName: queryForm.RoleName || undefined
    }
    const response = await rolesApi.getRoles(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.TotalCount = response.data.data.totalCount
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.message || error.message))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.RoleId = ''
  queryForm.RoleName = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增角色'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改角色'
  currentRoleId.value = row.RoleId
  try {
    const response = await rolesApi.getRole(row.RoleId)
    if (response.data.success) {
      Object.assign(form, {
        RoleId: response.data.data.roleId,
        RoleName: response.data.data.roleName,
        RoleNote: response.data.data.roleNote || ''
      })
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.message || error.message))
  }
  dialogVisible.value = true
}

// 複製
const handleCopy = (row) => {
  currentRoleId.value = row.RoleId
  copyForm.NewRoleId = ''
  copyForm.NewRoleName = ''
  copyDialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此角色嗎？刪除前會檢查是否有使用者使用此角色。', '提示', {
      type: 'warning'
    })
    const response = await rolesApi.deleteRole(row.RoleId)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

// 批次刪除
const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇要刪除的角色')
    return
  }

  try {
    await ElMessageBox.confirm(`確定要刪除選取的 ${selectedRows.value.length} 筆角色嗎？`, '提示', {
      type: 'warning'
    })
    const roleIds = selectedRows.value.map(row => row.RoleId)
    const response = await rolesApi.deleteRolesBatch({ RoleIds: roleIds })
    if (response.data.success) {
      ElMessage.success('批次刪除成功')
      selectedRows.value = []
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '批次刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗：' + (error.response?.data?.message || error.message))
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
          const updateData = {
            RoleName: form.RoleName,
            RoleNote: form.RoleNote
          }
          response = await rolesApi.updateRole(form.RoleId, updateData)
        } else {
          const createData = {
            RoleId: form.RoleId,
            RoleName: form.RoleName,
            RoleNote: form.RoleNote
          }
          response = await rolesApi.createRole(createData)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + (error.response?.data?.message || error.message))
      }
    }
  })
}

// 複製提交
const handleCopySubmit = async () => {
  if (!copyFormRef.value) return
  await copyFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await rolesApi.copyRole(currentRoleId.value, copyForm)
        if (response.data.success) {
          ElMessage.success('複製成功')
          copyDialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '複製失敗')
        }
      } catch (error) {
        ElMessage.error('複製失敗：' + (error.response?.data?.message || error.message))
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 關閉複製對話框
const handleCopyDialogClose = () => {
  copyDialogVisible.value = false
  copyForm.NewRoleId = ''
  copyForm.NewRoleName = ''
  copyFormRef.value?.resetFields()
}

// 重置表單
const resetForm = () => {
  form.RoleId = ''
  form.RoleName = ''
  form.RoleNote = ''
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

.sys0210-query {
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

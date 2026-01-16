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
        <el-table-column prop="RoleId" label="角色代碼" width="120" sortable />
        <el-table-column prop="RoleName" label="角色名稱" width="200" sortable />
        <el-table-column prop="RoleNote" label="角色敘述" min-width="200" />
        <el-table-column prop="CreatedAt" label="建立時間" width="160">
          <template #default="{ row }">
            {{ row.CreatedAt ? formatDateTime(row.CreatedAt) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="UpdatedAt" label="更新時間" width="160">
          <template #default="{ row }">
            {{ row.UpdatedAt ? formatDateTime(row.UpdatedAt) : '-' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
            <el-button type="info" size="small" @click="handleCopy(row)">複製</el-button>
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
        <el-form-item label="原角色代碼">
          <el-input v-model="copyForm.SourceRoleId" disabled />
        </el-form-item>
        <el-form-item label="原角色名稱">
          <el-input v-model="copyForm.SourceRoleName" disabled />
        </el-form-item>
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
const dialogTitle = ref('新增角色')
const isEdit = ref(false)
const formRef = ref(null)

// 複製對話框
const copyDialogVisible = ref(false)
const copyFormRef = ref(null)
const copyForm = reactive({
  SourceRoleId: '',
  SourceRoleName: '',
  NewRoleId: '',
  NewRoleName: ''
})

// 表單資料
const form = reactive({
  RoleId: '',
  RoleName: '',
  RoleNote: ''
})

// 表單驗證規則
const rules = {
  RoleId: [
    { required: true, message: '請輸入角色代碼', trigger: 'blur' },
    { max: 50, message: '角色代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  RoleName: [
    { required: true, message: '請輸入角色名稱', trigger: 'blur' },
    { max: 100, message: '角色名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  RoleNote: [
    { max: 500, message: '角色敘述長度不能超過500個字元', trigger: 'blur' }
  ]
}

// 複製表單驗證規則
const copyRules = {
  NewRoleId: [
    { required: true, message: '請輸入新角色代碼', trigger: 'blur' },
    { max: 50, message: '角色代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  NewRoleName: [
    { required: true, message: '請輸入新角色名稱', trigger: 'blur' },
    { max: 100, message: '角色名稱長度不能超過100個字元', trigger: 'blur' }
  ]
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
      RoleId: queryForm.RoleId || undefined,
      RoleName: queryForm.RoleName || undefined
    }
    const response = await rolesApi.getRoles(params)
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
    RoleId: '',
    RoleName: ''
  })
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增角色'
  Object.assign(form, {
    RoleId: '',
    RoleName: '',
    RoleNote: ''
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  try {
    const response = await rolesApi.getRole(row.RoleId)
    if (response && response.Data) {
      isEdit.value = true
      dialogTitle.value = '修改角色'
      Object.assign(form, {
        RoleId: response.Data.RoleId,
        RoleName: response.Data.RoleName,
        RoleNote: response.Data.RoleNote || ''
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
    await ElMessageBox.confirm(
      `確定要刪除角色「${row.RoleName}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    await rolesApi.deleteRole(row.RoleId)
    ElMessage.success('刪除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 複製
const handleCopy = (row) => {
  copyForm.SourceRoleId = row.RoleId
  copyForm.SourceRoleName = row.RoleName
  copyForm.NewRoleId = ''
  copyForm.NewRoleName = ''
  copyDialogVisible.value = true
}

// 提交表單
const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    if (isEdit.value) {
      await rolesApi.updateRole(form.RoleId, {
        RoleName: form.RoleName,
        RoleNote: form.RoleNote
      })
      ElMessage.success('修改成功')
    } else {
      await rolesApi.createRole({
        RoleId: form.RoleId,
        RoleName: form.RoleName,
        RoleNote: form.RoleNote
      })
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

// 複製提交
const handleCopySubmit = async () => {
  try {
    await copyFormRef.value.validate()
    await rolesApi.copyRole(copyForm.SourceRoleId, {
      NewRoleId: copyForm.NewRoleId,
      NewRoleName: copyForm.NewRoleName
    })
    ElMessage.success('複製成功')
    copyDialogVisible.value = false
    loadData()
  } catch (error) {
    if (error !== false) {
      ElMessage.error('複製失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 關閉對話框
const handleDialogClose = () => {
  formRef.value?.resetFields()
  dialogVisible.value = false
}

// 關閉複製對話框
const handleCopyDialogClose = () => {
  copyFormRef.value?.resetFields()
  copyDialogVisible.value = false
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

.sys0210-query {
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

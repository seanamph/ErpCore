<template>
  <div class="systems">
    <div class="page-header">
      <h1>主系統項目資料維護 (SYS0410)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="主系統代碼">
          <el-input v-model="queryForm.SystemId" placeholder="請輸入主系統代碼" clearable />
        </el-form-item>
        <el-form-item label="主系統名稱">
          <el-input v-model="queryForm.SystemName" placeholder="請輸入主系統名稱" clearable />
        </el-form-item>
        <el-form-item label="系統型態">
          <el-select v-model="queryForm.SystemType" placeholder="請選擇系統型態" clearable filterable style="width: 200px">
            <el-option
              v-for="type in systemTypeOptions"
              :key="type.Value"
              :label="type.Label"
              :value="type.Value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="伺服器主機名稱">
          <el-input v-model="queryForm.ServerIp" placeholder="請輸入伺服器主機名稱" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
          <el-button type="danger" @click="handleBatchDelete" :disabled="selectedRows.length === 0">批次刪除</el-button>
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
        <el-table-column prop="SystemId" label="主系統代碼" width="120" sortable />
        <el-table-column prop="SystemName" label="主系統名稱" width="200" sortable />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" sortable />
        <el-table-column prop="SystemTypeName" label="系統型態" width="120" />
        <el-table-column prop="ServerIp" label="伺服器主機名稱" width="150" />
        <el-table-column prop="ModuleId" label="模組代碼" width="120" />
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
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
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
        <el-form-item label="主系統代碼" prop="SystemId">
          <el-input v-model="form.SystemId" placeholder="請輸入主系統代碼" :disabled="isEdit" maxlength="50" />
        </el-form-item>
        <el-form-item label="主系統名稱" prop="SystemName">
          <el-input v-model="form.SystemName" placeholder="請輸入主系統名稱" maxlength="100" />
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="系統型態" prop="SystemType">
          <el-select v-model="form.SystemType" placeholder="請選擇系統型態" style="width: 100%">
            <el-option
              v-for="type in systemTypeOptions"
              :key="type.Value"
              :label="type.Label"
              :value="type.Value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="伺服器主機名稱" prop="ServerIp">
          <el-input v-model="form.ServerIp" placeholder="請輸入伺服器主機名稱" maxlength="100" />
        </el-form-item>
        <el-form-item label="模組代碼" prop="ModuleId">
          <el-input v-model="form.ModuleId" placeholder="請輸入模組代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="資料庫使用者" prop="DbUser">
          <el-input v-model="form.DbUser" placeholder="請輸入資料庫使用者" maxlength="50" />
        </el-form-item>
        <el-form-item label="資料庫密碼" prop="DbPass">
          <el-input v-model="form.DbPass" type="password" placeholder="請輸入資料庫密碼" maxlength="255" show-password />
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" maxlength="500" show-word-limit />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-radio-group v-model="form.Status">
            <el-radio label="A">啟用</el-radio>
            <el-radio label="I">停用</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getSystems, getSystemById, createSystem, updateSystem, deleteSystem, deleteSystemsBatch, getSystemTypes } from '@/api/systems'

// 查詢表單
const queryForm = reactive({
  SystemId: '',
  SystemName: '',
  SystemType: '',
  ServerIp: ''
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
const dialogTitle = ref('新增主系統')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  SystemId: '',
  SystemName: '',
  SeqNo: null,
  SystemType: '',
  ServerIp: '',
  ModuleId: '',
  DbUser: '',
  DbPass: '',
  Notes: '',
  Status: 'A'
})

// 系統型態選項
const systemTypeOptions = ref([])

// 表單驗證規則
const rules = {
  SystemId: [
    { required: true, message: '請輸入主系統代碼', trigger: 'blur' },
    { max: 50, message: '主系統代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  SystemName: [
    { required: true, message: '請輸入主系統名稱', trigger: 'blur' },
    { max: 100, message: '主系統名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  SystemType: [
    { required: true, message: '請選擇系統型態', trigger: 'change' }
  ]
}

// 載入系統型態選項
const loadSystemTypes = async () => {
  try {
    const response = await getSystemTypes()
    if (response.data.success) {
      systemTypeOptions.value = response.data.data || []
    }
  } catch (error) {
    console.error('載入系統型態選項失敗:', error)
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      Filters: {
        SystemId: queryForm.SystemId || undefined,
        SystemName: queryForm.SystemName || undefined,
        SystemType: queryForm.SystemType || undefined,
        ServerIp: queryForm.ServerIp || undefined
      }
    }
    const response = await getSystems(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.SystemId = ''
  queryForm.SystemName = ''
  queryForm.SystemType = ''
  queryForm.ServerIp = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增主系統'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改主系統'
  try {
    const response = await getSystemById(row.SystemId)
    if (response.data.success) {
      const data = response.data.data
      Object.assign(form, {
        SystemId: data.SystemId,
        SystemName: data.SystemName,
        SeqNo: data.SeqNo,
        SystemType: data.SystemType,
        ServerIp: data.ServerIp,
        ModuleId: data.ModuleId,
        DbUser: data.DbUser || '',
        DbPass: '', // 不顯示密碼
        Notes: data.Notes || '',
        Status: data.Status || 'A'
      })
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除主系統「${row.SystemName}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    const response = await deleteSystem(row.SystemId)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
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
    await ElMessageBox.confirm(
      `確定要刪除選取的 ${selectedRows.value.length} 筆資料嗎？`,
      '確認批次刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const systemIds = selectedRows.value.map((row) => row.SystemId)
    const response = await deleteSystemsBatch({ SystemIds: systemIds })
    if (response.data.success) {
      ElMessage.success('批次刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '批次刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
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
            SystemName: form.SystemName,
            SeqNo: form.SeqNo,
            SystemType: form.SystemType,
            ServerIp: form.ServerIp || null,
            ModuleId: form.ModuleId || null,
            DbUser: form.DbUser || null,
            DbPass: form.DbPass || null,
            Notes: form.Notes || null,
            Status: form.Status
          }
          response = await updateSystem(form.SystemId, updateData)
        } else {
          response = await createSystem(form)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + (error.message || '未知錯誤'))
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 重置表單
const resetForm = () => {
  form.SystemId = ''
  form.SystemName = ''
  form.SeqNo = null
  form.SystemType = ''
  form.ServerIp = ''
  form.ModuleId = ''
  form.DbUser = ''
  form.DbPass = ''
  form.Notes = ''
  form.Status = 'A'
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
  loadSystemTypes()
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.systems {
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

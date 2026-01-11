<template>
  <div class="sys0340-query">
    <div class="page-header">
      <h1>使用者欄位權限設定 (SYS0340)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者代碼">
          <el-select v-model="queryForm.UserId" placeholder="請選擇使用者" clearable filterable style="width: 200px" @change="handleUserChange">
            <el-option
              v-for="item in userOptions"
              :key="item.UserId"
              :label="`${item.UserId} - ${item.UserName}`"
              :value="item.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="資料庫">
          <el-select v-model="queryForm.DbName" placeholder="請選擇資料庫" clearable filterable style="width: 200px" @change="handleDbChange">
            <el-option
              v-for="item in dbOptions"
              :key="item.DbName"
              :label="item.DbDescription || item.DbName"
              :value="item.DbName"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="表格">
          <el-select v-model="queryForm.TableName" placeholder="請選擇表格" clearable filterable style="width: 200px" :disabled="!queryForm.DbName" @change="handleTableChange">
            <el-option
              v-for="item in tableOptions"
              :key="item.TableName"
              :label="item.TableDescription || item.TableName"
              :value="item.TableName"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="欄位">
          <el-input v-model="queryForm.FieldName" placeholder="請輸入欄位名稱" clearable />
        </el-form-item>
        <el-form-item label="權限類型">
          <el-select v-model="queryForm.PermissionType" placeholder="請選擇權限類型" clearable style="width: 150px">
            <el-option label="讀取" value="READ" />
            <el-option label="寫入" value="WRITE" />
            <el-option label="隱藏" value="HIDE" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleAdd" :disabled="!queryForm.UserId">新增</el-button>
          <el-button type="warning" @click="handleBatchSet" :disabled="!queryForm.UserId">批量設定</el-button>
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
        <el-table-column prop="UserId" label="使用者代碼" width="120" sortable />
        <el-table-column prop="UserName" label="使用者名稱" width="150" sortable />
        <el-table-column prop="DbName" label="資料庫" width="150" sortable />
        <el-table-column prop="TableName" label="表格" width="150" sortable />
        <el-table-column prop="FieldName" label="欄位" width="150" sortable />
        <el-table-column prop="PermissionType" label="權限類型" width="120" sortable>
          <template #default="{ row }">
            <el-tag :type="getPermissionTypeTag(row.PermissionType)">
              {{ getPermissionTypeText(row.PermissionType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedBy" label="建立者" width="100" />
        <el-table-column prop="CreatedAt" label="建立時間" width="160" sortable>
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
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
      <el-form :model="form" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="使用者" prop="UserId">
          <el-select v-model="form.UserId" placeholder="請選擇使用者" filterable style="width: 100%" :disabled="isEdit">
            <el-option
              v-for="item in userOptions"
              :key="item.UserId"
              :label="`${item.UserId} - ${item.UserName}`"
              :value="item.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="資料庫" prop="DbName">
          <el-select v-model="form.DbName" placeholder="請選擇資料庫" filterable style="width: 100%" @change="handleFormDbChange">
            <el-option
              v-for="item in dbOptions"
              :key="item.DbName"
              :label="item.DbDescription || item.DbName"
              :value="item.DbName"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="表格" prop="TableName">
          <el-select v-model="form.TableName" placeholder="請選擇表格" filterable style="width: 100%" :disabled="!form.DbName" @change="handleFormTableChange">
            <el-option
              v-for="item in tableOptions"
              :key="item.TableName"
              :label="item.TableDescription || item.TableName"
              :value="item.TableName"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="欄位" prop="FieldName">
          <el-select v-model="form.FieldName" placeholder="請選擇欄位" filterable style="width: 100%" :disabled="!form.TableName">
            <el-option
              v-for="item in fieldOptions"
              :key="item.FieldName"
              :label="item.FieldDescription || item.FieldName"
              :value="item.FieldName"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="權限類型" prop="PermissionType">
          <el-select v-model="form.PermissionType" placeholder="請選擇權限類型" style="width: 100%">
            <el-option label="讀取" value="READ" />
            <el-option label="寫入" value="WRITE" />
            <el-option label="隱藏" value="HIDE" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 批量設定對話框 -->
    <el-dialog
      title="批量設定使用者欄位權限"
      v-model="batchDialogVisible"
      width="900px"
      @close="handleBatchDialogClose"
    >
      <el-form :model="batchForm" label-width="120px">
        <el-form-item label="使用者" required>
          <el-select v-model="batchForm.UserId" placeholder="請選擇使用者" filterable style="width: 100%">
            <el-option
              v-for="item in userOptions"
              :key="item.UserId"
              :label="`${item.UserId} - ${item.UserName}`"
              :value="item.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="資料庫" required>
          <el-select v-model="batchForm.DbName" placeholder="請選擇資料庫" filterable style="width: 100%" @change="handleBatchDbChange">
            <el-option
              v-for="item in dbOptions"
              :key="item.DbName"
              :label="item.DbDescription || item.DbName"
              :value="item.DbName"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="表格" required>
          <el-select v-model="batchForm.TableName" placeholder="請選擇表格" filterable style="width: 100%" :disabled="!batchForm.DbName" @change="handleBatchTableChange">
            <el-option
              v-for="item in tableOptions"
              :key="item.TableName"
              :label="item.TableDescription || item.TableName"
              :value="item.TableName"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="權限類型" required>
          <el-select v-model="batchForm.PermissionType" placeholder="請選擇權限類型" style="width: 100%">
            <el-option label="讀取" value="READ" />
            <el-option label="寫入" value="WRITE" />
            <el-option label="隱藏" value="HIDE" />
          </el-select>
        </el-form-item>
        <el-form-item label="欄位列表">
          <el-table 
            ref="batchFieldTableRef"
            :data="fieldOptions" 
            border 
            stripe 
            style="width: 100%" 
            max-height="400"
            @selection-change="handleBatchFieldSelectionChange"
          >
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="FieldName" label="欄位名稱" width="200" />
            <el-table-column prop="FieldType" label="欄位類型" width="120" />
            <el-table-column prop="FieldDescription" label="欄位描述" />
          </el-table>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleBatchDialogClose">取消</el-button>
        <el-button type="primary" @click="handleBatchSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { userFieldPermissionsApi } from '@/api/permissions'
import { getUsers } from '@/api/users'

// 查詢表單
const queryForm = reactive({
  UserId: '',
  DbName: '',
  TableName: '',
  FieldName: '',
  PermissionType: ''
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

// 選項資料
const userOptions = ref([])
const dbOptions = ref([])
const tableOptions = ref([])
const fieldOptions = ref([])

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增使用者欄位權限')
const isEdit = ref(false)
const currentId = ref(null)
const formRef = ref(null)

// 表單資料
const form = reactive({
  UserId: '',
  DbName: '',
  TableName: '',
  FieldName: '',
  PermissionType: ''
})

// 表單驗證規則
const formRules = {
  UserId: [{ required: true, message: '請選擇使用者', trigger: 'change' }],
  DbName: [{ required: true, message: '請選擇資料庫', trigger: 'change' }],
  TableName: [{ required: true, message: '請選擇表格', trigger: 'change' }],
  FieldName: [{ required: true, message: '請選擇欄位', trigger: 'change' }],
  PermissionType: [{ required: true, message: '請選擇權限類型', trigger: 'change' }]
}

// 批量設定對話框
const batchDialogVisible = ref(false)
const batchForm = reactive({
  UserId: '',
  DbName: '',
  TableName: '',
  PermissionType: ''
})
const batchSelectedFields = ref([])
const batchFieldTableRef = ref(null)

// 載入使用者選項
const loadUserOptions = async () => {
  try {
    const response = await getUsers({
      PageIndex: 1,
      PageSize: 1000
    })
    if (response && response.Data) {
      userOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入使用者選項失敗:', error)
  }
}

// 載入資料庫選項
const loadDbOptions = async () => {
  try {
    const response = await userFieldPermissionsApi.getDatabases()
    if (response && response.Data) {
      dbOptions.value = response.Data || []
    }
  } catch (error) {
    console.error('載入資料庫選項失敗:', error)
  }
}

// 載入表格選項
const loadTableOptions = async (dbName) => {
  if (!dbName) {
    tableOptions.value = []
    return
  }
  try {
    const response = await userFieldPermissionsApi.getTables(dbName)
    if (response && response.Data) {
      tableOptions.value = response.Data || []
    }
  } catch (error) {
    console.error('載入表格選項失敗:', error)
  }
}

// 載入欄位選項
const loadFieldOptions = async (dbName, tableName) => {
  if (!dbName || !tableName) {
    fieldOptions.value = []
    return
  }
  try {
    const response = await userFieldPermissionsApi.getFields(dbName, tableName)
    if (response && response.Data) {
      fieldOptions.value = response.Data || []
    }
  } catch (error) {
    console.error('載入欄位選項失敗:', error)
  }
}

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      UserId: queryForm.UserId || undefined,
      DbName: queryForm.DbName || undefined,
      TableName: queryForm.TableName || undefined,
      FieldName: queryForm.FieldName || undefined,
      PermissionType: queryForm.PermissionType || undefined
    }
    const response = await userFieldPermissionsApi.getUserFieldPermissions(params)
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

// 使用者變更
const handleUserChange = () => {
  pagination.PageIndex = 1
  loadData()
}

// 資料庫變更
const handleDbChange = () => {
  queryForm.TableName = ''
  tableOptions.value = []
  fieldOptions.value = []
  if (queryForm.DbName) {
    loadTableOptions(queryForm.DbName)
  }
}

// 表格變更
const handleTableChange = () => {
  queryForm.FieldName = ''
  fieldOptions.value = []
  if (queryForm.DbName && queryForm.TableName) {
    loadFieldOptions(queryForm.DbName, queryForm.TableName)
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
    DbName: '',
    TableName: '',
    FieldName: '',
    PermissionType: ''
  })
  tableOptions.value = []
  fieldOptions.value = []
  tableData.value = []
  pagination.TotalCount = 0
}

// 新增
const handleAdd = () => {
  dialogTitle.value = '新增使用者欄位權限'
  isEdit.value = false
  currentId.value = null
  Object.assign(form, {
    UserId: queryForm.UserId || '',
    DbName: '',
    TableName: '',
    FieldName: '',
    PermissionType: ''
  })
  tableOptions.value = []
  fieldOptions.value = []
  dialogVisible.value = true
}

// 修改
const handleEdit = (row) => {
  dialogTitle.value = '修改使用者欄位權限'
  isEdit.value = true
  currentId.value = row.Id
  Object.assign(form, {
    UserId: row.UserId,
    DbName: row.DbName,
    TableName: row.TableName,
    FieldName: row.FieldName,
    PermissionType: row.PermissionType
  })
  loadTableOptions(row.DbName)
  loadFieldOptions(row.DbName, row.TableName)
  dialogVisible.value = true
}

// 表單資料庫變更
const handleFormDbChange = () => {
  form.TableName = ''
  form.FieldName = ''
  tableOptions.value = []
  fieldOptions.value = []
  if (form.DbName) {
    loadTableOptions(form.DbName)
  }
}

// 表單表格變更
const handleFormTableChange = () => {
  form.FieldName = ''
  fieldOptions.value = []
  if (form.DbName && form.TableName) {
    loadFieldOptions(form.DbName, form.TableName)
  }
}

// 提交表單
const handleSubmit = async () => {
  if (!formRef.value) return
  try {
    await formRef.value.validate()
    if (isEdit.value) {
      await userFieldPermissionsApi.updateUserFieldPermission(currentId.value, {
        PermissionType: form.PermissionType
      })
      ElMessage.success('修改成功')
    } else {
      await userFieldPermissionsApi.createUserFieldPermission({
        UserId: form.UserId,
        DbName: form.DbName,
        TableName: form.TableName,
        FieldName: form.FieldName,
        PermissionType: form.PermissionType
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

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  formRef.value?.resetFields()
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(`確定要刪除使用者「${row.UserName || row.UserId}」對欄位「${row.FieldName}」的權限設定嗎？`, '確認刪除', {
      type: 'warning'
    })
    await userFieldPermissionsApi.deleteUserFieldPermission(row.Id)
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
    for (const row of selectedRows.value) {
      await userFieldPermissionsApi.deleteUserFieldPermission(row.Id)
    }
    ElMessage.success('批次刪除成功')
    selectedRows.value = []
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 批量設定
const handleBatchSet = () => {
  if (!queryForm.UserId) {
    ElMessage.warning('請先選擇使用者')
    return
  }
  batchForm.UserId = queryForm.UserId
  batchForm.DbName = ''
  batchForm.TableName = ''
  batchForm.PermissionType = ''
  batchSelectedFields.value = []
  tableOptions.value = []
  fieldOptions.value = []
  // 清除表格選擇
  if (batchFieldTableRef.value) {
    batchFieldTableRef.value.clearSelection()
  }
  batchDialogVisible.value = true
}

// 批量設定資料庫變更
const handleBatchDbChange = () => {
  batchForm.TableName = ''
  tableOptions.value = []
  fieldOptions.value = []
  if (batchForm.DbName) {
    loadTableOptions(batchForm.DbName)
  }
}

// 批量設定表格變更
const handleBatchTableChange = () => {
  fieldOptions.value = []
  if (batchForm.DbName && batchForm.TableName) {
    loadFieldOptions(batchForm.DbName, batchForm.TableName)
  }
}

// 批量字段選擇變更
const handleBatchFieldSelectionChange = (selection) => {
  batchSelectedFields.value = selection
}

// 批量提交
const handleBatchSubmit = async () => {
  if (!batchForm.UserId || !batchForm.DbName || !batchForm.TableName || !batchForm.PermissionType) {
    ElMessage.warning('請填寫完整的批量設定資訊')
    return
  }
  if (fieldOptions.value.length === 0) {
    ElMessage.warning('請先選擇資料庫和表格以載入欄位列表')
    return
  }
  if (batchSelectedFields.value.length === 0) {
    ElMessage.warning('請至少選擇一個欄位')
    return
  }
  try {
    await ElMessageBox.confirm(`確定要批量設定 ${batchSelectedFields.value.length} 個欄位的權限嗎？`, '確認設定', {
      type: 'warning'
    })
    const permissions = batchSelectedFields.value.map(field => ({
      DbName: batchForm.DbName,
      TableName: batchForm.TableName,
      FieldName: field.FieldName,
      PermissionType: batchForm.PermissionType
    }))
    await userFieldPermissionsApi.batchSetPermissions({
      UserId: batchForm.UserId,
      Permissions: permissions
    })
    ElMessage.success('批量設定成功')
    batchDialogVisible.value = false
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批量設定失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 關閉批量設定對話框
const handleBatchDialogClose = () => {
  batchDialogVisible.value = false
  Object.assign(batchForm, {
    UserId: '',
    DbName: '',
    TableName: '',
    PermissionType: ''
  })
  batchSelectedFields.value = []
  tableOptions.value = []
  fieldOptions.value = []
}

// 獲取權限類型標籤
const getPermissionTypeTag = (type) => {
  const map = {
    READ: 'success',
    WRITE: 'warning',
    HIDE: 'danger'
  }
  return map[type] || 'info'
}

// 獲取權限類型文字
const getPermissionTypeText = (type) => {
  const map = {
    READ: '讀取',
    WRITE: '寫入',
    HIDE: '隱藏'
  }
  return map[type] || type
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
  loadUserOptions()
  loadDbOptions()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0340-query {
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

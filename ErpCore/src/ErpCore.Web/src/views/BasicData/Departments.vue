<template>
  <div class="departments">
    <div class="page-header">
      <h1>部別資料維護作業 (SYSWB40)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="部別代碼">
          <el-input v-model="queryForm.Filters.DeptId" placeholder="請輸入部別代碼" clearable />
        </el-form-item>
        <el-form-item label="部別名稱">
          <el-input v-model="queryForm.Filters.DeptName" placeholder="請輸入部別名稱" clearable />
        </el-form-item>
        <el-form-item label="組織">
          <el-select v-model="queryForm.Filters.OrgId" placeholder="請選擇組織" clearable style="width: 200px">
            <el-option v-for="org in orgList" :key="org.OrgId" :label="org.OrgName" :value="org.OrgId" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Filters.Status" placeholder="請選擇狀態" clearable style="width: 150px">
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
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
        <el-table-column prop="DeptId" label="部別代碼" width="150" />
        <el-table-column prop="DeptName" label="部別名稱" width="200" />
        <el-table-column prop="OrgName" label="組織" width="150" />
        <el-table-column prop="SeqNo" label="排序序號" width="100" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="UpdatedAt" label="更新時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.UpdatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button 
              :type="row.Status === 'A' ? 'warning' : 'success'" 
              size="small" 
              @click="handleToggleStatus(row)"
            >
              {{ row.Status === 'A' ? '停用' : '啟用' }}
            </el-button>
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
      width="600px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="部別代碼" prop="DeptId">
          <el-input v-model="form.DeptId" placeholder="請輸入部別代碼" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="部別名稱" prop="DeptName">
          <el-input v-model="form.DeptName" placeholder="請輸入部別名稱" />
        </el-form-item>
        <el-form-item label="組織" prop="OrgId">
          <el-select v-model="form.OrgId" placeholder="請選擇組織" clearable style="width: 100%">
            <el-option v-for="org in orgList" :key="org.OrgId" :label="org.OrgName" :value="org.OrgId" />
          </el-select>
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" placeholder="請輸入排序序號" style="width: 100%" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { departmentsApi } from '@/api/departments'

// 查詢表單
const queryForm = reactive({
  PageIndex: 1,
  PageSize: 20,
  SortField: 'DeptId',
  SortOrder: 'ASC',
  Filters: {
    DeptId: '',
    DeptName: '',
    OrgId: '',
    Status: ''
  }
})

// 組織列表
const orgList = ref([])

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
const dialogTitle = ref('新增部別')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  DeptId: '',
  DeptName: '',
  OrgId: '',
  SeqNo: 0,
  Status: 'A',
  Notes: ''
})

// 保存原始部別代碼（用於修改）
const originalDeptId = ref('')

// 表單驗證規則
const rules = {
  DeptId: [{ required: true, message: '請輸入部別代碼', trigger: 'blur' }],
  DeptName: [{ required: true, message: '請輸入部別名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
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

// 載入組織列表
const loadOrgList = async () => {
  try {
    // TODO: 實作組織列表 API 呼叫
    // const response = await organizationsApi.getOrganizations()
    // orgList.value = response.data.data.items || []
  } catch (error) {
    console.error('載入組織列表失敗：', error)
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: queryForm.SortField,
      SortOrder: queryForm.SortOrder,
      Filters: {
        DeptId: queryForm.Filters.DeptId || undefined,
        DeptName: queryForm.Filters.DeptName || undefined,
        OrgId: queryForm.Filters.OrgId || undefined,
        Status: queryForm.Filters.Status || undefined
      }
    }
    const response = await departmentsApi.getDepartments(params)
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
  queryForm.Filters.DeptId = ''
  queryForm.Filters.DeptName = ''
  queryForm.Filters.OrgId = ''
  queryForm.Filters.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增部別'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改部別'
  originalDeptId.value = row.DeptId
  try {
    const response = await departmentsApi.getDepartment(row.DeptId)
    if (response.data.success) {
      Object.assign(form, {
        DeptId: response.data.data.DeptId,
        DeptName: response.data.data.DeptName,
        OrgId: response.data.data.OrgId || '',
        SeqNo: response.data.data.SeqNo || 0,
        Status: response.data.data.Status || 'A',
        Notes: response.data.data.Notes || ''
      })
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
  dialogVisible.value = true
}

// 切換狀態
const handleToggleStatus = async (row) => {
  try {
    const newStatus = row.Status === 'A' ? 'I' : 'A'
    const statusText = newStatus === 'A' ? '啟用' : '停用'
    await ElMessageBox.confirm(`確定要${statusText}此部別嗎？`, '提示', {
      type: 'warning'
    })
    const response = await departmentsApi.updateDepartmentStatus(row.DeptId, newStatus)
    if (response.data.success) {
      ElMessage.success(`${statusText}成功`)
      handleSearch()
    } else {
      ElMessage.error(response.data.message || `${statusText}失敗`)
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('操作失敗：' + error.message)
    }
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此部別嗎？', '提示', {
      type: 'warning'
    })
    const response = await departmentsApi.deleteDepartment(row.DeptId)
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
          // 修改時使用原始部別代碼
          const updateData = {
            DeptName: form.DeptName,
            OrgId: form.OrgId || null,
            SeqNo: form.SeqNo || 0,
            Status: form.Status,
            Notes: form.Notes || null
          }
          response = await departmentsApi.updateDepartment(originalDeptId.value, updateData)
        } else {
          const createData = {
            DeptId: form.DeptId,
            DeptName: form.DeptName,
            OrgId: form.OrgId || null,
            SeqNo: form.SeqNo || 0,
            Status: form.Status,
            Notes: form.Notes || null
          }
          response = await departmentsApi.createDepartment(createData)
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

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 重置表單
const resetForm = () => {
  form.DeptId = ''
  form.DeptName = ''
  form.OrgId = ''
  form.SeqNo = 0
  form.Status = 'A'
  form.Notes = ''
  originalDeptId.value = ''
  formRef.value?.resetFields()
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  queryForm.PageSize = size
  queryForm.PageIndex = 1
  handleSearch()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  queryForm.PageIndex = page
  handleSearch()
}

// 初始化
onMounted(() => {
  loadOrgList()
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.departments {
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

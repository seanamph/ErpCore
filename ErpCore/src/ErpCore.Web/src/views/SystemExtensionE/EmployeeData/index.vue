<template>
  <div class="employee-data">
    <div class="page-header">
      <h1>員工資料維護 (SYSPE10-SYSPE11)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="員工編號">
          <el-input v-model="queryForm.EmployeeId" placeholder="請輸入員工編號" clearable />
        </el-form-item>
        <el-form-item label="員工姓名">
          <el-input v-model="queryForm.EmployeeName" placeholder="請輸入員工姓名" clearable />
        </el-form-item>
        <el-form-item label="部門">
          <el-input v-model="queryForm.DepartmentId" placeholder="請輸入部門" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="在職" value="A" />
            <el-option label="離職" value="I" />
            <el-option label="留停" value="L" />
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
        <el-table-column prop="EmployeeId" label="員工編號" width="120" />
        <el-table-column prop="EmployeeName" label="員工姓名" width="150" />
        <el-table-column prop="DepartmentId" label="部門" width="120" />
        <el-table-column prop="PositionId" label="職位" width="120" />
        <el-table-column prop="HireDate" label="到職日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.HireDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ResignDate" label="離職日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ResignDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Phone" label="電話" width="150" />
        <el-table-column prop="Email" label="電子郵件" width="200" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
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
      v-model="dialogVisible"
      :title="dialogTitle"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-form-item label="員工編號" prop="EmployeeId">
          <el-input v-model="formData.EmployeeId" :disabled="isEdit" placeholder="請輸入員工編號" />
        </el-form-item>
        <el-form-item label="員工姓名" prop="EmployeeName">
          <el-input v-model="formData.EmployeeName" placeholder="請輸入員工姓名" />
        </el-form-item>
        <el-form-item label="身份證字號">
          <el-input v-model="formData.IdNumber" placeholder="請輸入身份證字號" />
        </el-form-item>
        <el-form-item label="部門">
          <el-input v-model="formData.DepartmentId" placeholder="請輸入部門" />
        </el-form-item>
        <el-form-item label="職位">
          <el-input v-model="formData.PositionId" placeholder="請輸入職位" />
        </el-form-item>
        <el-form-item label="到職日期">
          <el-date-picker
            v-model="formData.HireDate"
            type="date"
            placeholder="請選擇到職日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="離職日期">
          <el-date-picker
            v-model="formData.ResignDate"
            type="date"
            placeholder="請選擇離職日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="在職" value="A" />
            <el-option label="離職" value="I" />
            <el-option label="留停" value="L" />
          </el-select>
        </el-form-item>
        <el-form-item label="電話">
          <el-input v-model="formData.Phone" placeholder="請輸入電話" />
        </el-form-item>
        <el-form-item label="電子郵件">
          <el-input v-model="formData.Email" type="email" placeholder="請輸入電子郵件" />
        </el-form-item>
        <el-form-item label="地址">
          <el-input v-model="formData.Address" placeholder="請輸入地址" />
        </el-form-item>
        <el-form-item label="生日">
          <el-date-picker
            v-model="formData.BirthDate"
            type="date"
            placeholder="請選擇生日"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="性別">
          <el-radio-group v-model="formData.Gender">
            <el-radio label="M">男</el-radio>
            <el-radio label="F">女</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="備註">
          <el-input v-model="formData.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

// API
const employeeApi = {
  getEmployees: (params) => axios.get('/api/v1/employees', { params }),
  getEmployee: (employeeId) => axios.get(`/api/v1/employees/${employeeId}`),
  createEmployee: (data) => axios.post('/api/v1/employees', data),
  updateEmployee: (employeeId, data) => axios.put(`/api/v1/employees/${employeeId}`, data),
  deleteEmployee: (employeeId) => axios.delete(`/api/v1/employees/${employeeId}`)
}

// 查詢表單
const queryForm = reactive({
  EmployeeId: '',
  EmployeeName: '',
  DepartmentId: '',
  Status: ''
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
const dialogTitle = computed(() => isEdit.value ? '修改員工資料' : '新增員工資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  EmployeeId: '',
  EmployeeName: '',
  IdNumber: '',
  DepartmentId: '',
  PositionId: '',
  HireDate: null,
  ResignDate: null,
  Status: 'A',
  Email: '',
  Phone: '',
  Address: '',
  BirthDate: null,
  Gender: '',
  Notes: ''
})
const formRules = {
  EmployeeId: [{ required: true, message: '請輸入員工編號', trigger: 'blur' }],
  EmployeeName: [{ required: true, message: '請輸入員工姓名', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }],
  Email: [{ type: 'email', message: '請輸入正確的電子郵件格式', trigger: 'blur' }]
}
const currentEmployeeId = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      EmployeeId: queryForm.EmployeeId || undefined,
      EmployeeName: queryForm.EmployeeName || undefined,
      DepartmentId: queryForm.DepartmentId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await employeeApi.getEmployees(params)
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
  queryForm.EmployeeId = ''
  queryForm.EmployeeName = ''
  queryForm.DepartmentId = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentEmployeeId.value = null
  Object.assign(formData, {
    EmployeeId: '',
    EmployeeName: '',
    IdNumber: '',
    DepartmentId: '',
    PositionId: '',
    HireDate: null,
    ResignDate: null,
    Status: 'A',
    Email: '',
    Phone: '',
    Address: '',
    BirthDate: null,
    Gender: '',
    Notes: ''
  })
  dialogVisible.value = true
}

// 查看
const handleView = async (row) => {
  await handleEdit(row)
}

// 修改
const handleEdit = async (row) => {
  try {
    const response = await employeeApi.getEmployee(row.EmployeeId)
    if (response.data?.success) {
      isEdit.value = true
      currentEmployeeId.value = row.EmployeeId
      Object.assign(formData, response.data.data)
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此員工資料嗎？', '確認', {
      type: 'warning'
    })
    const response = await employeeApi.deleteEmployee(row.EmployeeId)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
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
          response = await employeeApi.updateEmployee(currentEmployeeId.value, formData)
        } else {
          response = await employeeApi.createEmployee(formData)
        }
        if (response.data?.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
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

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString('zh-TW')
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'A': 'success',
    'I': 'danger',
    'L': 'warning'
  }
  return types[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const texts = {
    'A': '在職',
    'I': '離職',
    'L': '留停'
  }
  return texts[status] || status
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.employee-data {
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


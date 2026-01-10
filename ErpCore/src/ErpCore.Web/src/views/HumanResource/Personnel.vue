<template>
  <div class="personnel">
    <div class="page-header">
      <h1>人事管理 (SYSH110)</h1>
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
            <el-option label="在職" value="A" />
            <el-option label="離職" value="I" />
            <el-option label="停職" value="S" />
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
        <el-table-column prop="Position" label="職位" width="120" />
        <el-table-column prop="Phone" label="電話" width="150" />
        <el-table-column prop="Email" label="電子郵件" width="200" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="HireDate" label="到職日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.HireDate) }}
          </template>
        </el-table-column>
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
        :page-sizes="[10, 20, 50, 100]"
        :total="pagination.TotalCount"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right;"
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
        <el-form-item label="部門" prop="DepartmentId">
          <el-input v-model="formData.DepartmentId" placeholder="請輸入部門" />
        </el-form-item>
        <el-form-item label="職位" prop="Position">
          <el-input v-model="formData.Position" placeholder="請輸入職位" />
        </el-form-item>
        <el-form-item label="電話" prop="Phone">
          <el-input v-model="formData.Phone" placeholder="請輸入電話" />
        </el-form-item>
        <el-form-item label="電子郵件" prop="Email">
          <el-input v-model="formData.Email" placeholder="請輸入電子郵件" />
        </el-form-item>
        <el-form-item label="到職日期" prop="HireDate">
          <el-date-picker
            v-model="formData.HireDate"
            type="date"
            placeholder="請選擇到職日期"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="在職" value="A" />
            <el-option label="離職" value="I" />
            <el-option label="停職" value="S" />
          </el-select>
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input
            v-model="formData.Memo"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { humanResourceApi } from '@/api/humanResource'

export default {
  name: 'Personnel',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      EmployeeId: '',
      EmployeeName: '',
      DepartmentId: '',
      Status: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資料
    const formData = reactive({
      EmployeeId: '',
      EmployeeName: '',
      DepartmentId: '',
      Position: '',
      Phone: '',
      Email: '',
      HireDate: new Date(),
      Status: 'A',
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      EmployeeId: [{ required: true, message: '請輸入員工編號', trigger: 'blur' }],
      EmployeeName: [{ required: true, message: '請輸入員工姓名', trigger: 'blur' }],
      DepartmentId: [{ required: true, message: '請輸入部門', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'A': 'success',
        'I': 'danger',
        'S': 'warning'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'A': '在職',
        'I': '離職',
        'S': '停職'
      }
      return texts[status] || status
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await humanResourceApi.getEmployees(params)
        if (response.Data) {
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
        EmployeeId: '',
        EmployeeName: '',
        DepartmentId: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        EmployeeId: '',
        EmployeeName: '',
        DepartmentId: '',
        Position: '',
        Phone: '',
        Email: '',
        HireDate: new Date(),
        Status: 'A',
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await humanResourceApi.getEmployee(row.EmployeeId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, response.Data)
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此員工嗎？', '確認', {
          type: 'warning'
        })
        await humanResourceApi.deleteEmployee(row.EmployeeId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await humanResourceApi.updateEmployee(formData.EmployeeId, formData)
              ElMessage.success('修改成功')
            } else {
              await humanResourceApi.createEmployee(formData)
              ElMessage.success('新增成功')
            }
            dialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
          }
        }
      })
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 計算對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改員工' : '新增員工'
    })

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      formatDate,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.personnel {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border: 1px solid $card-border;
    
    .search-form {
      .el-form-item {
        margin-bottom: 16px;
      }
    }
  }

  .table-card {
    background-color: $card-bg;
    border: 1px solid $card-border;
  }
}
</style>


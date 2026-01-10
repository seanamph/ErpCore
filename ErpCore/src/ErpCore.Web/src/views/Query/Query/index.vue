<template>
  <div class="query">
    <div class="page-header">
      <h1>查詢功能維護 (SYSQ000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="查詢代碼">
          <el-input v-model="queryForm.QueryCode" placeholder="請輸入查詢代碼" clearable />
        </el-form-item>
        <el-form-item label="查詢名稱">
          <el-input v-model="queryForm.QueryName" placeholder="請輸入查詢名稱" clearable />
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
        <el-table-column prop="QueryCode" label="查詢代碼" width="150" />
        <el-table-column prop="QueryName" label="查詢名稱" width="200" />
        <el-table-column prop="Description" label="說明" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
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
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="150px"
      >
        <el-form-item label="查詢代碼" prop="QueryCode">
          <el-input v-model="formData.QueryCode" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="查詢名稱" prop="QueryName">
          <el-input v-model="formData.QueryName" />
        </el-form-item>
        <el-form-item label="說明">
          <el-input v-model="formData.Description" type="textarea" :rows="3" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-radio-group v-model="formData.Status">
            <el-radio label="1">啟用</el-radio>
            <el-radio label="0">停用</el-radio>
          </el-radio-group>
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
import { queryApi } from '@/api/query'

// 查詢表單
const queryForm = reactive({
  QueryCode: '',
  QueryName: ''
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
const dialogTitle = ref('新增查詢功能')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  QueryCode: '',
  QueryName: '',
  Description: '',
  Status: '1'
})
const formRules = {
  QueryCode: [{ required: true, message: '請輸入查詢代碼', trigger: 'blur' }],
  QueryName: [{ required: true, message: '請輸入查詢名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentTKey = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      QueryCode: queryForm.QueryCode || undefined,
      QueryName: queryForm.QueryName || undefined
    }
    const response = await queryApi.getQueries(params)
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
  queryForm.QueryCode = ''
  queryForm.QueryName = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  dialogTitle.value = '新增查詢功能'
  Object.assign(formData, {
    QueryCode: '',
    QueryName: '',
    Description: '',
    Status: '1'
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
    const response = await queryApi.getQuery(row.TKey)
    if (response.data?.success) {
      isEdit.value = true
      currentTKey.value = row.TKey
      dialogTitle.value = '修改查詢功能'
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
    await ElMessageBox.confirm('確定要刪除此查詢功能嗎？', '確認', {
      type: 'warning'
    })
    const response = await queryApi.deleteQuery(row.TKey)
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
          response = await queryApi.updateQuery(currentTKey.value, formData)
        } else {
          response = await queryApi.createQuery(formData)
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

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.query {
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


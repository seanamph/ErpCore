<template>
  <div class="cus3000-eskyland">
    <div class="page-header">
      <h1>CUS3000.ESKYLAND 客戶定制模組</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="會員編號">
          <el-input v-model="queryForm.MemberId" placeholder="請輸入會員編號" clearable />
        </el-form-item>
        <el-form-item label="會員名稱">
          <el-input v-model="queryForm.MemberName" placeholder="請輸入會員名稱" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
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
        <el-table-column prop="MemberId" label="會員編號" width="150" />
        <el-table-column prop="MemberName" label="會員名稱" width="200" />
        <el-table-column prop="CardNo" label="卡號" width="150" />
        <el-table-column prop="EskylandSpecificField" label="ESKYLAND特定欄位" width="200" />
        <el-table-column prop="Status" label="狀態" width="100">
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

    <!-- 對話框 -->
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
        label-width="120px"
      >
        <el-form-item label="會員編號" prop="MemberId">
          <el-input v-model="formData.MemberId" :disabled="isEdit" placeholder="請輸入會員編號" />
        </el-form-item>
        <el-form-item label="會員名稱" prop="MemberName">
          <el-input v-model="formData.MemberName" placeholder="請輸入會員名稱" />
        </el-form-item>
        <el-form-item label="卡號">
          <el-input v-model="formData.CardNo" placeholder="請輸入卡號" />
        </el-form-item>
        <el-form-item label="ESKYLAND特定欄位">
          <el-input v-model="formData.EskylandSpecificField" placeholder="請輸入ESKYLAND特定欄位" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
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

const api = {
  getList: (params) => axios.get('/api/v1/cus3000-eskyland/members', { params }),
  getById: (tKey) => axios.get(`/api/v1/cus3000-eskyland/members/${tKey}`),
  create: (data) => axios.post('/api/v1/cus3000-eskyland/members', data),
  update: (tKey, data) => axios.put(`/api/v1/cus3000-eskyland/members/${tKey}`, data),
  delete: (tKey) => axios.delete(`/api/v1/cus3000-eskyland/members/${tKey}`)
}

const queryForm = reactive({
  MemberId: '',
  MemberName: '',
  Status: ''
})
const tableData = ref([])
const loading = ref(false)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const dialogVisible = ref(false)
const dialogTitle = computed(() => isEdit.value ? '修改ESKYLAND會員' : '新增ESKYLAND會員')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  MemberId: '',
  MemberName: '',
  CardNo: '',
  EskylandSpecificField: '',
  Status: 'A'
})
const formRules = {
  MemberId: [{ required: true, message: '請輸入會員編號', trigger: 'blur' }],
  MemberName: [{ required: true, message: '請輸入會員名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentTKey = ref(null)

const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      MemberId: queryForm.MemberId || undefined,
      MemberName: queryForm.MemberName || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await api.getList(params)
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

const handleReset = () => {
  queryForm.MemberId = ''
  queryForm.MemberName = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  Object.assign(formData, {
    MemberId: '',
    MemberName: '',
    CardNo: '',
    EskylandSpecificField: '',
    Status: 'A'
  })
  dialogVisible.value = true
}

const handleEdit = async (row) => {
  try {
    const response = await api.getById(row.TKey)
    if (response.data?.success) {
      isEdit.value = true
      currentTKey.value = row.TKey
      Object.assign(formData, response.data.data)
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此資料嗎？', '確認', { type: 'warning' })
    const response = await api.delete(row.TKey)
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

const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          response = await api.update(currentTKey.value, formData)
        } else {
          response = await api.create(formData)
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

const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.cus3000-eskyland {
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


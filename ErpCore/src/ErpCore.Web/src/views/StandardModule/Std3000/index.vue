<template>
  <div class="std3000">
    <div class="page-header"><h1>STD3000 標準模組 (SYS3620)</h1></div>
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="資料編號"><el-input v-model="queryForm.DataId" placeholder="請輸入資料編號" clearable /></el-form-item>
        <el-form-item><el-button type="primary" @click="handleSearch">查詢</el-button><el-button @click="handleReset">重置</el-button><el-button type="success" @click="handleCreate">新增</el-button></el-form-item>
      </el-form>
    </el-card>
    <el-card class="table-card" shadow="never">
      <el-table :data="tableData" v-loading="loading" border stripe style="width: 100%">
        <el-table-column prop="DataId" label="資料編號" width="150" />
        <el-table-column prop="DataName" label="資料名稱" width="200" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination v-model:current-page="pagination.PageIndex" v-model:page-size="pagination.PageSize" :total="pagination.TotalCount" :page-sizes="[10, 20, 50, 100]" layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handlePageChange" style="margin-top: 20px; text-align: right" />
    </el-card>
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px" :close-on-click-modal="false">
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="120px">
        <el-form-item label="資料編號" prop="DataId"><el-input v-model="formData.DataId" :disabled="isEdit" placeholder="請輸入資料編號" /></el-form-item>
        <el-form-item label="資料名稱" prop="DataName"><el-input v-model="formData.DataName" placeholder="請輸入資料名稱" /></el-form-item>
      </el-form>
      <template #footer><el-button @click="dialogVisible = false">取消</el-button><el-button type="primary" @click="handleSubmit">確定</el-button></template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

const api = {
  getList: (params) => axios.get('/api/v1/std3000/data', { params }),
  getById: (tKey) => axios.get(`/api/v1/std3000/data/${tKey}`),
  create: (data) => axios.post('/api/v1/std3000/data', data),
  update: (tKey, data) => axios.put(`/api/v1/std3000/data/${tKey}`, data),
  delete: (tKey) => axios.delete(`/api/v1/std3000/data/${tKey}`)
}

const queryForm = reactive({ DataId: '' })
const tableData = ref([])
const loading = ref(false)
const pagination = reactive({ PageIndex: 1, PageSize: 20, TotalCount: 0 })
const dialogVisible = ref(false)
const dialogTitle = computed(() => isEdit.value ? '修改資料' : '新增資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({ DataId: '', DataName: '' })
const formRules = { DataId: [{ required: true, message: '請輸入資料編號', trigger: 'blur' }], DataName: [{ required: true, message: '請輸入資料名稱', trigger: 'blur' }] }
const currentTKey = ref(null)

const handleSearch = async () => {
  loading.value = true
  try {
    const params = { PageIndex: pagination.PageIndex, PageSize: pagination.PageSize, DataId: queryForm.DataId || undefined }
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
  queryForm.DataId = ''
  pagination.PageIndex = 1
  handleSearch()
}

const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  Object.assign(formData, { DataId: '', DataName: '' })
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
.std3000 {
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


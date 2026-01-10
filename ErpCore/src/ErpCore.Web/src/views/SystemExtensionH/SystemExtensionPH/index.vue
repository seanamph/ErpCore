<template>
  <div class="system-extension-ph">
    <div class="page-header">
      <h1>感應卡維護作業 (SYSPH00)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="感應卡號">
          <el-input v-model="queryForm.CardNo" placeholder="請輸入感應卡號" clearable />
        </el-form-item>
        <el-form-item label="員工編號">
          <el-input v-model="queryForm.EmpId" placeholder="請輸入員工編號" clearable />
        </el-form-item>
        <el-form-item label="卡片狀態">
          <el-select v-model="queryForm.CardStatus" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
          <el-button type="warning" @click="handleBatchCreate">批量新增</el-button>
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
        <el-table-column prop="CardNo" label="感應卡號" width="150" />
        <el-table-column prop="EmpId" label="員工編號" width="120" />
        <el-table-column prop="BeginDate" label="啟用日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.BeginDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="EndDate" label="失效日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.EndDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CardStatus" label="卡片狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.CardStatus === '1' ? 'success' : 'danger'">
              {{ row.CardStatus === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" width="200" show-overflow-tooltip />
        <el-table-column prop="BTime" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.BTime) }}
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
        label-width="120px"
      >
        <el-form-item label="感應卡號" prop="CardNo">
          <el-input v-model="formData.CardNo" :disabled="isEdit" placeholder="請輸入感應卡號" />
        </el-form-item>
        <el-form-item label="員工編號" prop="EmpId">
          <el-input v-model="formData.EmpId" placeholder="請輸入員工編號" />
        </el-form-item>
        <el-form-item label="啟用日期">
          <el-date-picker
            v-model="formData.BeginDate"
            type="date"
            placeholder="請選擇啟用日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="失效日期">
          <el-date-picker
            v-model="formData.EndDate"
            type="date"
            placeholder="請選擇失效日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="卡片狀態" prop="CardStatus">
          <el-radio-group v-model="formData.CardStatus">
            <el-radio label="1">啟用</el-radio>
            <el-radio label="0">停用</el-radio>
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

    <!-- 批量新增對話框 -->
    <el-dialog
      v-model="batchDialogVisible"
      title="批量新增感應卡"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-table :data="batchFormData.Items" border style="width: 100%">
        <el-table-column label="感應卡號" width="150">
          <template #default="{ $index }">
            <el-input v-model="batchFormData.Items[$index].CardNo" placeholder="請輸入感應卡號" />
          </template>
        </el-table-column>
        <el-table-column label="員工編號" width="150">
          <template #default="{ $index }">
            <el-input v-model="batchFormData.Items[$index].EmpId" placeholder="請輸入員工編號" />
          </template>
        </el-table-column>
        <el-table-column label="啟用日期" width="150">
          <template #default="{ $index }">
            <el-date-picker
              v-model="batchFormData.Items[$index].BeginDate"
              type="date"
              placeholder="請選擇"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </template>
        </el-table-column>
        <el-table-column label="失效日期" width="150">
          <template #default="{ $index }">
            <el-date-picker
              v-model="batchFormData.Items[$index].EndDate"
              type="date"
              placeholder="請選擇"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </template>
        </el-table-column>
        <el-table-column label="卡片狀態" width="120">
          <template #default="{ $index }">
            <el-select v-model="batchFormData.Items[$index].CardStatus" style="width: 100%">
              <el-option label="啟用" value="1" />
              <el-option label="停用" value="0" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="handleRemoveBatchItem($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <div style="margin-top: 10px;">
        <el-button type="primary" @click="handleAddBatchItem">新增一筆</el-button>
      </div>
      <template #footer>
        <el-button @click="batchDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleBatchSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

// API
const empCardApi = {
  getEmpCards: (params) => axios.get('/api/v1/emp-cards', { params }),
  getEmpCard: (tKey) => axios.get(`/api/v1/emp-cards/${tKey}`),
  createEmpCard: (data) => axios.post('/api/v1/emp-cards', data),
  createBatchEmpCards: (data) => axios.post('/api/v1/emp-cards/batch', data),
  updateEmpCard: (tKey, data) => axios.put(`/api/v1/emp-cards/${tKey}`, data),
  deleteEmpCard: (tKey) => axios.delete(`/api/v1/emp-cards/${tKey}`)
}

// 查詢表單
const queryForm = reactive({
  CardNo: '',
  EmpId: '',
  CardStatus: ''
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
const dialogTitle = computed(() => isEdit.value ? '修改感應卡' : '新增感應卡')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  CardNo: '',
  EmpId: '',
  BeginDate: null,
  EndDate: null,
  CardStatus: '1',
  Notes: ''
})
const formRules = {
  CardNo: [{ required: true, message: '請輸入感應卡號', trigger: 'blur' }],
  EmpId: [{ required: true, message: '請輸入員工編號', trigger: 'blur' }],
  CardStatus: [{ required: true, message: '請選擇卡片狀態', trigger: 'change' }]
}
const currentTKey = ref(null)

// 批量新增對話框
const batchDialogVisible = ref(false)
const batchFormData = reactive({
  Items: []
})

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      CardNo: queryForm.CardNo || undefined,
      EmpId: queryForm.EmpId || undefined,
      CardStatus: queryForm.CardStatus || undefined
    }
    const response = await empCardApi.getEmpCards(params)
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
  queryForm.CardNo = ''
  queryForm.EmpId = ''
  queryForm.CardStatus = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  Object.assign(formData, {
    CardNo: '',
    EmpId: '',
    BeginDate: null,
    EndDate: null,
    CardStatus: '1',
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
    const response = await empCardApi.getEmpCard(row.TKey)
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

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此感應卡嗎？', '確認', {
      type: 'warning'
    })
    const response = await empCardApi.deleteEmpCard(row.TKey)
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
          response = await empCardApi.updateEmpCard(currentTKey.value, formData)
        } else {
          response = await empCardApi.createEmpCard(formData)
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

// 批量新增
const handleBatchCreate = () => {
  batchFormData.Items = [{
    CardNo: '',
    EmpId: '',
    BeginDate: null,
    EndDate: null,
    CardStatus: '1',
    Notes: ''
  }]
  batchDialogVisible.value = true
}

// 新增批量項目
const handleAddBatchItem = () => {
  batchFormData.Items.push({
    CardNo: '',
    EmpId: '',
    BeginDate: null,
    EndDate: null,
    CardStatus: '1',
    Notes: ''
  })
}

// 移除批量項目
const handleRemoveBatchItem = (index) => {
  batchFormData.Items.splice(index, 1)
}

// 批量提交
const handleBatchSubmit = async () => {
  if (batchFormData.Items.length === 0) {
    ElMessage.warning('請至少新增一筆資料')
    return
  }
  try {
    const response = await empCardApi.createBatchEmpCards(batchFormData)
    if (response.data?.success) {
      ElMessage.success(`批量新增成功，共新增 ${response.data.data} 筆`)
      batchDialogVisible.value = false
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '批量新增失敗')
    }
  } catch (error) {
    ElMessage.error('批量新增失敗：' + error.message)
  }
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

.system-extension-ph {
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


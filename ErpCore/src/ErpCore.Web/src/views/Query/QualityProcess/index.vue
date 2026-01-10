<template>
  <div class="quality-process">
    <div class="page-header">
      <h1>質量管理處理功能 (SYSQ210-SYSQ250)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="分店代號">
          <el-input v-model="queryForm.SiteId" placeholder="請輸入分店代號" clearable />
        </el-form-item>
        <el-form-item label="保管人代碼">
          <el-input v-model="queryForm.KeepEmpId" placeholder="請輸入保管人代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.CashStatus" placeholder="請選擇狀態" clearable>
            <el-option label="已申請" value="APPLIED" />
            <el-option label="已請款" value="REQUESTED" />
            <el-option label="已拋轉" value="TRANSFERRED" />
            <el-option label="已審核" value="APPROVED" />
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
        <el-table-column prop="SiteId" label="分店代號" width="120" />
        <el-table-column prop="KeepEmpId" label="保管人代碼" width="150" />
        <el-table-column prop="CashAmount" label="零用金金額" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.CashAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="CashStatus" label="狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.CashStatus)">
              {{ getStatusText(row.CashStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="RequestDate" label="請款日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.RequestDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" width="250" fixed="right">
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
        <el-form-item label="分店代號" prop="SiteId">
          <el-input v-model="formData.SiteId" />
        </el-form-item>
        <el-form-item label="保管人代碼" prop="KeepEmpId">
          <el-input v-model="formData.KeepEmpId" />
        </el-form-item>
        <el-form-item label="零用金金額" prop="CashAmount">
          <el-input-number v-model="formData.CashAmount" :min="0" :precision="2" style="width: 100%" />
        </el-form-item>
        <el-form-item label="狀態" prop="CashStatus">
          <el-select v-model="formData.CashStatus" style="width: 100%">
            <el-option label="已申請" value="APPLIED" />
            <el-option label="已請款" value="REQUESTED" />
            <el-option label="已拋轉" value="TRANSFERRED" />
            <el-option label="已審核" value="APPROVED" />
          </el-select>
        </el-form-item>
        <el-form-item label="請款日期" prop="RequestDate">
          <el-date-picker
            v-model="formData.RequestDate"
            type="date"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="備註">
          <el-input v-model="formData.Notes" type="textarea" :rows="3" />
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
import { qualityProcessApi } from '@/api/query'

// 查詢表單
const queryForm = reactive({
  SiteId: '',
  KeepEmpId: '',
  CashStatus: ''
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
const dialogTitle = ref('新增零用金')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  SiteId: '',
  KeepEmpId: '',
  CashAmount: 0,
  CashStatus: 'APPLIED',
  RequestDate: '',
  Notes: ''
})
const formRules = {
  SiteId: [{ required: true, message: '請輸入分店代號', trigger: 'blur' }],
  KeepEmpId: [{ required: true, message: '請輸入保管人代碼', trigger: 'blur' }],
  CashAmount: [{ required: true, message: '請輸入零用金金額', trigger: 'blur' }],
  CashStatus: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentTKey = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SiteId: queryForm.SiteId || undefined,
      KeepEmpId: queryForm.KeepEmpId || undefined,
      CashStatus: queryForm.CashStatus || undefined
    }
    const response = await qualityProcessApi.getPcCashList(params)
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
  queryForm.SiteId = ''
  queryForm.KeepEmpId = ''
  queryForm.CashStatus = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  dialogTitle.value = '新增零用金'
  Object.assign(formData, {
    SiteId: '',
    KeepEmpId: '',
    CashAmount: 0,
    CashStatus: 'APPLIED',
    RequestDate: '',
    Notes: ''
  })
  dialogVisible.value = true
}

// 批量新增
const handleBatchCreate = () => {
  ElMessage.info('批量新增功能開發中')
}

// 查看
const handleView = async (row) => {
  await handleEdit(row)
}

// 修改
const handleEdit = async (row) => {
  try {
    const response = await qualityProcessApi.getPcCash(row.TKey)
    if (response.data?.success) {
      isEdit.value = true
      currentTKey.value = row.TKey
      dialogTitle.value = '修改零用金'
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
    await ElMessageBox.confirm('確定要刪除此零用金嗎？', '確認', {
      type: 'warning'
    })
    const response = await qualityProcessApi.deletePcCash(row.TKey)
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
          response = await qualityProcessApi.updatePcCash(currentTKey.value, formData)
        } else {
          response = await qualityProcessApi.createPcCash(formData)
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
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

// 格式化貨幣
const formatCurrency = (value) => {
  if (value == null) return '0.00'
  return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'APPLIED': 'info',
    'REQUESTED': 'warning',
    'TRANSFERRED': 'primary',
    'APPROVED': 'success'
  }
  return types[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const texts = {
    'APPLIED': '已申請',
    'REQUESTED': '已請款',
    'TRANSFERRED': '已拋轉',
    'APPROVED': '已審核'
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

.quality-process {
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


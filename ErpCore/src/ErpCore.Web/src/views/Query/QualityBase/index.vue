<template>
  <div class="quality-base">
    <div class="page-header">
      <h1>質量管理基礎功能 (SYSQ110-SYSQ120)</h1>
    </div>

    <!-- Tab 切換 -->
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 零用金參數維護 (SYSQ110) -->
      <el-tab-pane label="零用金參數維護" name="cashParams">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" class="search-form">
            <el-form-item>
              <el-button type="primary" @click="handleSearchCashParams">查詢</el-button>
              <el-button type="success" @click="handleCreateCashParam">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="cashParamsData"
            v-loading="loading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="UnitId" label="公司單位代號" width="150" />
            <el-table-column prop="ApexpLid" label="銀行存款會計科目代號" width="200" />
            <el-table-column prop="PtaxLid" label="進項稅額會計科目代號" width="200" />
            <el-table-column prop="CreatedAt" label="建立時間" width="180">
              <template #default="{ row }">
                {{ formatDateTime(row.CreatedAt) }}
              </template>
            </el-table-column>
            <el-table-column prop="CreatedBy" label="建立者" width="120" />
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="warning" size="small" @click="handleEditCashParam(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleDeleteCashParam(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-tab-pane>

      <!-- 保管人及額度設定 (SYSQ120) -->
      <el-tab-pane label="保管人及額度設定" name="pcKeep">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="queryForm" class="search-form">
            <el-form-item label="分店代號">
              <el-input v-model="queryForm.SiteId" placeholder="請輸入分店代號" clearable />
            </el-form-item>
            <el-form-item label="保管人代碼">
              <el-input v-model="queryForm.KeepEmpId" placeholder="請輸入保管人代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSearch">查詢</el-button>
              <el-button @click="handleReset">重置</el-button>
              <el-button type="success" @click="handleCreate">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

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
            <el-table-column prop="PcQuota" label="零用金額度" width="150" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.PcQuota) }}
              </template>
            </el-table-column>
            <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
            <el-table-column prop="BTime" label="建立時間" width="180">
              <template #default="{ row }">
                {{ formatDateTime(row.BTime) }}
              </template>
            </el-table-column>
            <el-table-column prop="BUser" label="建立者" width="120" />
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
      </el-tab-pane>
    </el-tabs>

    <!-- 零用金參數新增/修改對話框 -->
    <el-dialog
      v-model="cashParamDialogVisible"
      :title="cashParamDialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="cashParamFormRef"
        :model="cashParamForm"
        :rules="cashParamRules"
        label-width="180px"
      >
        <el-form-item label="公司單位代號" prop="UnitId">
          <el-input v-model="cashParamForm.UnitId" />
        </el-form-item>
        <el-form-item label="銀行存款會計科目代號" prop="ApexpLid">
          <el-input v-model="cashParamForm.ApexpLid" />
        </el-form-item>
        <el-form-item label="進項稅額會計科目代號" prop="PtaxLid">
          <el-input v-model="cashParamForm.PtaxLid" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="cashParamDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmitCashParam">確定</el-button>
      </template>
    </el-dialog>

    <!-- 保管人及額度新增/修改對話框 -->
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
          <el-input v-model="formData.KeepEmpId" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="零用金額度" prop="PcQuota">
          <el-input-number v-model="formData.PcQuota" :min="0" :precision="2" style="width: 100%" />
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
import { qualityBaseApi } from '@/api/query'

// Tab 切換
const activeTab = ref('cashParams')

// 零用金參數相關
const cashParamsData = ref([])
const cashParamDialogVisible = ref(false)
const cashParamDialogTitle = ref('新增零用金參數')
const cashParamFormRef = ref(null)
const cashParamForm = reactive({
  UnitId: '',
  ApexpLid: '',
  PtaxLid: ''
})
const cashParamRules = {
  ApexpLid: [{ required: true, message: '請輸入銀行存款會計科目代號', trigger: 'blur' }],
  PtaxLid: [{ required: true, message: '請輸入進項稅額會計科目代號', trigger: 'blur' }]
}
const currentCashParamTKey = ref(null)

// 保管人及額度相關
const queryForm = reactive({
  SiteId: '',
  KeepEmpId: ''
})
const tableData = ref([])
const loading = ref(false)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const dialogVisible = ref(false)
const dialogTitle = ref('新增保管人及額度')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  SiteId: '',
  KeepEmpId: '',
  PcQuota: 0,
  Notes: ''
})
const formRules = {
  KeepEmpId: [{ required: true, message: '請輸入保管人代碼', trigger: 'blur' }],
  PcQuota: [{ required: true, message: '請輸入零用金額度', trigger: 'blur' }]
}
const currentTKey = ref(null)

// Tab 切換
const handleTabChange = (tabName) => {
  if (tabName === 'cashParams') {
    handleSearchCashParams()
  } else if (tabName === 'pcKeep') {
    handleSearch()
  }
}

// ========== 零用金參數相關方法 ==========

// 查詢零用金參數
const handleSearchCashParams = async () => {
  loading.value = true
  try {
    const response = await qualityBaseApi.getCashParams()
    if (response.data?.success) {
      cashParamsData.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 新增零用金參數
const handleCreateCashParam = () => {
  currentCashParamTKey.value = null
  cashParamDialogTitle.value = '新增零用金參數'
  Object.assign(cashParamForm, {
    UnitId: '',
    ApexpLid: '',
    PtaxLid: ''
  })
  cashParamDialogVisible.value = true
}

// 修改零用金參數
const handleEditCashParam = async (row) => {
  try {
    const response = await qualityBaseApi.getCashParam(row.TKey)
    if (response.data?.success) {
      currentCashParamTKey.value = row.TKey
      cashParamDialogTitle.value = '修改零用金參數'
      Object.assign(cashParamForm, response.data.data)
      cashParamDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 刪除零用金參數
const handleDeleteCashParam = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此零用金參數嗎？', '確認', {
      type: 'warning'
    })
    const response = await qualityBaseApi.deleteCashParam(row.TKey)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleSearchCashParams()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 提交零用金參數
const handleSubmitCashParam = async () => {
  if (!cashParamFormRef.value) return
  await cashParamFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (currentCashParamTKey.value) {
          response = await qualityBaseApi.updateCashParam(currentCashParamTKey.value, cashParamForm)
        } else {
          response = await qualityBaseApi.createCashParam(cashParamForm)
        }
        if (response.data?.success) {
          ElMessage.success(currentCashParamTKey.value ? '修改成功' : '新增成功')
          cashParamDialogVisible.value = false
          handleSearchCashParams()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
}

// ========== 保管人及額度相關方法 ==========

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SiteId: queryForm.SiteId || undefined,
      KeepEmpId: queryForm.KeepEmpId || undefined
    }
    const response = await qualityBaseApi.getPcKeepList(params)
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
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  dialogTitle.value = '新增保管人及額度'
  Object.assign(formData, {
    SiteId: '',
    KeepEmpId: '',
    PcQuota: 0,
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
    const response = await qualityBaseApi.getPcKeep(row.TKey)
    if (response.data?.success) {
      isEdit.value = true
      currentTKey.value = row.TKey
      dialogTitle.value = '修改保管人及額度'
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
    await ElMessageBox.confirm('確定要刪除此保管人及額度嗎？', '確認', {
      type: 'warning'
    })
    const response = await qualityBaseApi.deletePcKeep(row.TKey)
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
          response = await qualityBaseApi.updatePcKeep(currentTKey.value, formData)
        } else {
          response = await qualityBaseApi.createPcKeep(formData)
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

// 格式化貨幣
const formatCurrency = (value) => {
  if (value == null) return '0.00'
  return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 初始化
onMounted(() => {
  handleSearchCashParams()
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.quality-base {
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


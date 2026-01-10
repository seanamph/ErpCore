<template>
  <div class="ledger-data">
    <div class="page-header">
      <h1>總帳資料維護 (SYS2000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="總帳編號">
          <el-input v-model="queryForm.LedgerId" placeholder="請輸入總帳編號" clearable />
        </el-form-item>
        <el-form-item label="會計科目">
          <el-input v-model="queryForm.AccountId" placeholder="請輸入會計科目編號" clearable />
        </el-form-item>
        <el-form-item label="會計期間">
          <el-input v-model="queryForm.Period" placeholder="YYYYMM" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="草稿" value="DRAFT" />
            <el-option label="已過帳" value="POSTED" />
            <el-option label="已結帳" value="CLOSED" />
          </el-select>
        </el-form-item>
        <el-form-item label="總帳日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
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
        <el-table-column prop="LedgerId" label="總帳編號" width="150" />
        <el-table-column prop="LedgerDate" label="總帳日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.LedgerDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="AccountId" label="會計科目" width="150" />
        <el-table-column prop="VoucherNo" label="憑證號碼" width="150" />
        <el-table-column prop="Description" label="說明" min-width="200" show-overflow-tooltip />
        <el-table-column prop="DebitAmount" label="借方金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.DebitAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreditAmount" label="貸方金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.CreditAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Balance" label="餘額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Balance) }}
          </template>
        </el-table-column>
        <el-table-column prop="Period" label="會計期間" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusName(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button
              v-if="row.Status === 'DRAFT'"
              type="warning"
              size="small"
              @click="handleEdit(row)"
            >
              修改
            </el-button>
            <el-button
              v-if="row.Status === 'DRAFT'"
              type="success"
              size="small"
              @click="handlePost(row)"
            >
              過帳
            </el-button>
            <el-button
              v-if="row.Status === 'DRAFT'"
              type="danger"
              size="small"
              @click="handleDelete(row)"
            >
              刪除
            </el-button>
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
        label-width="150px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="總帳編號" prop="LedgerId">
              <el-input v-model="formData.LedgerId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="總帳日期" prop="LedgerDate">
              <el-date-picker
                v-model="formData.LedgerDate"
                type="date"
                placeholder="選擇日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="會計科目" prop="AccountId">
              <el-input v-model="formData.AccountId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="憑證號碼">
              <el-input v-model="formData.VoucherNo" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="說明">
          <el-input v-model="formData.Description" />
        </el-form-item>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="借方金額">
              <el-input-number
                v-model="formData.DebitAmount"
                :precision="2"
                :min="0"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="貸方金額">
              <el-input-number
                v-model="formData.CreditAmount"
                :precision="2"
                :min="0"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="會計期間" prop="Period">
              <el-input v-model="formData.Period" placeholder="YYYYMM" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="幣別">
              <el-input v-model="formData.CurrencyId" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註">
          <el-input v-model="formData.Memo" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 科目餘額查詢對話框 -->
    <el-dialog
      v-model="balanceDialogVisible"
      title="科目餘額查詢"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form :inline="true" :model="balanceQueryForm">
        <el-form-item label="會計科目">
          <el-input v-model="balanceQueryForm.AccountId" placeholder="請輸入會計科目編號" clearable />
        </el-form-item>
        <el-form-item label="會計期間">
          <el-input v-model="balanceQueryForm.Period" placeholder="YYYYMM" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQueryBalance">查詢</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="balanceData" border style="width: 100%; margin-top: 20px">
        <el-table-column prop="AccountId" label="會計科目" width="150" />
        <el-table-column prop="Period" label="會計期間" width="120" />
        <el-table-column prop="OpeningBalance" label="期初餘額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.OpeningBalance) }}
          </template>
        </el-table-column>
        <el-table-column prop="DebitAmount" label="借方金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.DebitAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreditAmount" label="貸方金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.CreditAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="ClosingBalance" label="期末餘額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.ClosingBalance) }}
          </template>
        </el-table-column>
      </el-table>
      <template #footer>
        <el-button @click="balanceDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ledgerDataApi } from '@/api/customerInvoice'

// 查詢表單
const queryForm = reactive({
  LedgerId: '',
  AccountId: '',
  Period: '',
  Status: '',
  LedgerDateFrom: '',
  LedgerDateTo: ''
})
const dateRange = ref(null)

// 監聽日期範圍變化
watch(dateRange, (val) => {
  if (val && val.length === 2) {
    queryForm.LedgerDateFrom = val[0]
    queryForm.LedgerDateTo = val[1]
  } else {
    queryForm.LedgerDateFrom = ''
    queryForm.LedgerDateTo = ''
  }
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
const balanceDialogVisible = ref(false)
const dialogTitle = computed(() => isEdit.value ? '修改總帳資料' : '新增總帳資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  LedgerId: '',
  LedgerDate: '',
  AccountId: '',
  VoucherNo: '',
  Description: '',
  DebitAmount: 0,
  CreditAmount: 0,
  Balance: 0,
  CurrencyId: 'TWD',
  Period: '',
  Status: 'DRAFT',
  Memo: ''
})
const formRules = {
  LedgerId: [{ required: true, message: '請輸入總帳編號', trigger: 'blur' }],
  LedgerDate: [{ required: true, message: '請選擇總帳日期', trigger: 'change' }],
  AccountId: [{ required: true, message: '請輸入會計科目', trigger: 'blur' }],
  Period: [{ required: true, message: '請輸入會計期間', trigger: 'blur' }]
}
const currentLedgerId = ref(null)

// 科目餘額查詢
const balanceQueryForm = reactive({
  AccountId: '',
  Period: ''
})
const balanceData = ref([])

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      LedgerId: queryForm.LedgerId || undefined,
      AccountId: queryForm.AccountId || undefined,
      Period: queryForm.Period || undefined,
      Status: queryForm.Status || undefined,
      LedgerDateFrom: queryForm.LedgerDateFrom || undefined,
      LedgerDateTo: queryForm.LedgerDateTo || undefined
    }
    const response = await ledgerDataApi.getGeneralLedgers(params)
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
  queryForm.LedgerId = ''
  queryForm.AccountId = ''
  queryForm.Period = ''
  queryForm.Status = ''
  queryForm.LedgerDateFrom = ''
  queryForm.LedgerDateTo = ''
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentLedgerId.value = null
  const today = new Date()
  const period = `${today.getFullYear()}${String(today.getMonth() + 1).padStart(2, '0')}`
  Object.assign(formData, {
    LedgerId: '',
    LedgerDate: '',
    AccountId: '',
    VoucherNo: '',
    Description: '',
    DebitAmount: 0,
    CreditAmount: 0,
    Balance: 0,
    CurrencyId: 'TWD',
    Period: period,
    Status: 'DRAFT',
    Memo: ''
  })
  dialogVisible.value = true
}

// 查看
const handleView = async (row) => {
  try {
    const response = await ledgerDataApi.getGeneralLedger(row.LedgerId)
    if (response.data?.success) {
      ElMessageBox.alert(JSON.stringify(response.data.data, null, 2), '總帳詳情', {
        confirmButtonText: '確定'
      })
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 修改
const handleEdit = async (row) => {
  try {
    const response = await ledgerDataApi.getGeneralLedger(row.LedgerId)
    if (response.data?.success) {
      isEdit.value = true
      currentLedgerId.value = row.LedgerId
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
    await ElMessageBox.confirm('確定要刪除此總帳資料嗎？', '確認', {
      type: 'warning'
    })
    const response = await ledgerDataApi.deleteGeneralLedger(row.LedgerId)
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

// 過帳
const handlePost = async (row) => {
  try {
    await ElMessageBox.confirm('確定要過帳此總帳嗎？', '確認', {
      type: 'warning'
    })
    const response = await ledgerDataApi.postLedger(row.LedgerId)
    if (response.data?.success) {
      ElMessage.success('過帳成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '過帳失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('過帳失敗：' + error.message)
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
          response = await ledgerDataApi.updateGeneralLedger(currentLedgerId.value, formData)
        } else {
          response = await ledgerDataApi.createGeneralLedger(formData)
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

// 查詢科目餘額
const handleQueryBalance = async () => {
  try {
    const params = {
      AccountId: balanceQueryForm.AccountId || undefined,
      Period: balanceQueryForm.Period || undefined
    }
    const response = await ledgerDataApi.getAccountBalances(params)
    if (response.data?.success) {
      balanceData.value = Array.isArray(response.data.data) ? response.data.data : [response.data.data]
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
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
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

// 格式化貨幣
const formatCurrency = (amount) => {
  if (amount == null) return '0.00'
  return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 取得狀態名稱
const getStatusName = (status) => {
  const statuses = {
    'DRAFT': '草稿',
    'POSTED': '已過帳',
    'CLOSED': '已結帳'
  }
  return statuses[status] || status
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'DRAFT': 'info',
    'POSTED': 'success',
    'CLOSED': 'warning'
  }
  return types[status] || ''
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.ledger-data {
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


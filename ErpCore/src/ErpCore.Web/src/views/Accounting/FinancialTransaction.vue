<template>
  <div class="financial-transaction">
    <div class="page-header">
      <h1>財務交易 (SYSN210-SYSN213)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="交易單號">
          <el-input v-model="queryForm.TxnNo" placeholder="請輸入交易單號" clearable />
        </el-form-item>
        <el-form-item label="交易日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="交易類型">
          <el-input v-model="queryForm.TxnType" placeholder="請輸入交易類型" clearable />
        </el-form-item>
        <el-form-item label="會計科目">
          <el-input v-model="queryForm.StypeId" placeholder="請輸入會計科目" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="DRAFT" />
            <el-option label="已確認" value="CONFIRMED" />
            <el-option label="已過帳" value="POSTED" />
            <el-option label="已取消" value="CANCELLED" />
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
        <el-table-column prop="TxnNo" label="交易單號" width="150" />
        <el-table-column prop="TxnDate" label="交易日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TxnDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TxnType" label="交易類型" width="120" />
        <el-table-column prop="StypeId" label="會計科目" width="150" />
        <el-table-column prop="Dc" label="借貸方向" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Dc === 'D' ? 'success' : 'warning'">
              {{ row.Dc === 'D' ? '借方' : '貸方' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Description" label="說明" width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Verifier" label="確認人員" width="120" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'POSTED'">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status === 'POSTED'">刪除</el-button>
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
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="交易單號" prop="TxnNo">
              <el-input v-model="formData.TxnNo" :disabled="isEdit" placeholder="請輸入交易單號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="交易日期" prop="TxnDate">
              <el-date-picker
                v-model="formData.TxnDate"
                type="date"
                placeholder="請選擇交易日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="交易類型" prop="TxnType">
              <el-input v-model="formData.TxnType" placeholder="請輸入交易類型" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="會計科目" prop="StypeId">
              <el-input v-model="formData.StypeId" placeholder="請輸入會計科目" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="借貸方向" prop="Dc">
              <el-select v-model="formData.Dc" placeholder="請選擇借貸方向" style="width: 100%">
                <el-option label="借方" value="D" />
                <el-option label="貸方" value="C" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="金額" prop="Amount">
              <el-input-number v-model="formData.Amount" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="說明" prop="Description">
              <el-input v-model="formData.Description" type="textarea" :rows="3" placeholder="請輸入說明" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Notes">
              <el-input v-model="formData.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
          </el-col>
        </el-row>
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
import { accountingApi } from '@/api/accounting'

export default {
  name: 'FinancialTransaction',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const dateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      TxnNo: '',
      TxnDateFrom: null,
      TxnDateTo: null,
      TxnType: '',
      StypeId: '',
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
      TxnNo: '',
      TxnDate: '',
      TxnType: '',
      StypeId: '',
      Dc: 'D',
      Amount: 0,
      Description: '',
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      TxnNo: [{ required: true, message: '請輸入交易單號', trigger: 'blur' }],
      TxnDate: [{ required: true, message: '請選擇交易日期', trigger: 'change' }],
      TxnType: [{ required: true, message: '請輸入交易類型', trigger: 'blur' }],
      StypeId: [{ required: true, message: '請輸入會計科目', trigger: 'blur' }],
      Dc: [{ required: true, message: '請選擇借貸方向', trigger: 'change' }],
      Amount: [{ required: true, message: '請輸入金額', trigger: 'blur' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return '-'
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化金額
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount)
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const map = { DRAFT: 'info', CONFIRMED: 'warning', POSTED: 'success', CANCELLED: 'danger' }
      return map[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const map = { DRAFT: '草稿', CONFIRMED: '已確認', POSTED: '已過帳', CANCELLED: '已取消' }
      return map[status] || status
    }

    // 日期範圍變更
    const handleDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        queryForm.TxnDateFrom = dates[0]
        queryForm.TxnDateTo = dates[1]
      } else {
        queryForm.TxnDateFrom = null
        queryForm.TxnDateTo = null
      }
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
        const response = await accountingApi.getFinancialTransactions(params)
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
        TxnNo: '',
        TxnDateFrom: null,
        TxnDateTo: null,
        TxnType: '',
        StypeId: '',
        Status: ''
      })
      dateRange.value = null
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        TxnNo: '',
        TxnDate: '',
        TxnType: '',
        StypeId: '',
        Dc: 'D',
        Amount: 0,
        Description: '',
        Notes: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await accountingApi.getFinancialTransaction(row.TKey)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, {
            TxnNo: response.Data.TxnNo,
            TxnDate: formatDate(response.Data.TxnDate),
            TxnType: response.Data.TxnType,
            StypeId: response.Data.StypeId,
            Dc: response.Data.Dc,
            Amount: response.Data.Amount,
            Description: response.Data.Description || '',
            Notes: response.Data.Notes || ''
          })
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
        await ElMessageBox.confirm('確定要刪除此財務交易嗎？', '確認', {
          type: 'warning'
        })
        await accountingApi.deleteFinancialTransaction(row.TKey)
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
              await accountingApi.updateFinancialTransaction(formData.TxnNo, formData)
              ElMessage.success('修改成功')
            } else {
              await accountingApi.createFinancialTransaction(formData)
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
      return isEdit.value ? '修改財務交易' : '新增財務交易'
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
      dateRange,
      formatDate,
      formatCurrency,
      getStatusType,
      getStatusText,
      handleDateRangeChange,
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

.financial-transaction {
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


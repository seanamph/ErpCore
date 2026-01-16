<template>
  <div class="procurement-payment">
    <div class="page-header">
      <h1>付款單維護 (SYSP271-SYSP2B0)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="付款單號">
          <el-input v-model="queryForm.PaymentId" placeholder="請輸入付款單號" clearable />
        </el-form-item>
        <el-form-item label="付款日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="供應商代號">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入供應商代號" clearable />
        </el-form-item>
        <el-form-item label="付款類型">
          <el-select v-model="queryForm.PaymentType" placeholder="請選擇付款類型" clearable>
            <el-option label="現金" value="CASH" />
            <el-option label="支票" value="CHECK" />
            <el-option label="轉帳" value="TRANSFER" />
            <el-option label="其他" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="D" />
            <el-option label="已確認" value="A" />
            <el-option label="已取消" value="C" />
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
        <el-table-column prop="PaymentId" label="付款單號" width="150" />
        <el-table-column prop="PaymentDate" label="付款日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.PaymentDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="SupplierId" label="供應商代號" width="120" />
        <el-table-column prop="SupplierName" label="供應商名稱" width="200" />
        <el-table-column prop="PaymentType" label="付款類型" width="100">
          <template #default="{ row }">
            {{ getPaymentTypeLabel(row.PaymentType) }}
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="付款金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="CurrencyId" label="幣別" width="80" />
        <el-table-column prop="CheckNumber" label="支票號碼" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusLabel(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedBy" label="建立人員" width="100" />
        <el-table-column prop="CreatedAt" label="建立時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button 
              v-if="row.Status === 'D'" 
              type="warning" 
              size="small" 
              @click="handleEdit(row)"
            >
              修改
            </el-button>
            <el-button 
              v-if="row.Status === 'D'" 
              type="success" 
              size="small" 
              @click="handleConfirm(row)"
            >
              確認
            </el-button>
            <el-button 
              v-if="row.Status === 'D'" 
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
      width="900px"
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
            <el-form-item label="付款單號" prop="PaymentId">
              <el-input v-model="formData.PaymentId" :disabled="isEdit" placeholder="請輸入付款單號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="付款日期" prop="PaymentDate">
              <el-date-picker
                v-model="formData.PaymentDate"
                type="date"
                placeholder="請選擇付款日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="供應商代號" prop="SupplierId">
              <el-input v-model="formData.SupplierId" placeholder="請輸入供應商代號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="付款類型" prop="PaymentType">
              <el-select v-model="formData.PaymentType" placeholder="請選擇付款類型" style="width: 100%">
                <el-option label="現金" value="CASH" />
                <el-option label="支票" value="CHECK" />
                <el-option label="轉帳" value="TRANSFER" />
                <el-option label="其他" value="OTHER" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="付款金額" prop="Amount">
              <el-input-number
                v-model="formData.Amount"
                :min="0"
                :precision="2"
                placeholder="請輸入付款金額"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="幣別" prop="CurrencyId">
              <el-input v-model="formData.CurrencyId" placeholder="請輸入幣別（預設：TWD）" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="匯率" prop="ExchangeRate">
              <el-input-number
                v-model="formData.ExchangeRate"
                :min="0"
                :precision="6"
                placeholder="請輸入匯率（預設：1）"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="銀行帳戶編號" prop="BankAccountId">
              <el-input v-model="formData.BankAccountId" placeholder="請輸入銀行帳戶編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="支票號碼" prop="CheckNumber">
              <el-input v-model="formData.CheckNumber" placeholder="請輸入支票號碼" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="草稿" value="D" />
                <el-option label="已確認" value="A" />
                <el-option label="已取消" value="C" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Memo">
              <el-input v-model="formData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
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
import { procurementApi } from '@/api/procurement'

export default {
  name: 'ProcurementPayment',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const dateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      PaymentId: '',
      PaymentDateFrom: null,
      PaymentDateTo: null,
      SupplierId: '',
      PaymentType: '',
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
      PaymentId: '',
      PaymentDate: '',
      SupplierId: '',
      PaymentType: '',
      Amount: 0,
      CurrencyId: 'TWD',
      ExchangeRate: 1,
      BankAccountId: '',
      CheckNumber: '',
      Status: 'D',
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      PaymentId: [{ required: true, message: '請輸入付款單號', trigger: 'blur' }],
      PaymentDate: [{ required: true, message: '請選擇付款日期', trigger: 'change' }],
      SupplierId: [{ required: true, message: '請輸入供應商代號', trigger: 'blur' }],
      PaymentType: [{ required: true, message: '請選擇付款類型', trigger: 'change' }],
      Amount: [{ required: true, message: '請輸入付款金額', trigger: 'blur' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改付款單' : '新增付款單'
    })

    // 日期範圍變更處理
    const handleDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        queryForm.PaymentDateFrom = dates[0]
        queryForm.PaymentDateTo = dates[1]
      } else {
        queryForm.PaymentDateFrom = null
        queryForm.PaymentDateTo = null
      }
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PaymentId: queryForm.PaymentId || undefined,
          PaymentDateFrom: queryForm.PaymentDateFrom || undefined,
          PaymentDateTo: queryForm.PaymentDateTo || undefined,
          SupplierId: queryForm.SupplierId || undefined,
          PaymentType: queryForm.PaymentType || undefined,
          Status: queryForm.Status || undefined,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await procurementApi.getPayments(params)
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
        PaymentId: '',
        PaymentDateFrom: null,
        PaymentDateTo: null,
        SupplierId: '',
        PaymentType: '',
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
        PaymentId: '',
        PaymentDate: '',
        SupplierId: '',
        PaymentType: '',
        Amount: 0,
        CurrencyId: 'TWD',
        ExchangeRate: 1,
        BankAccountId: '',
        CheckNumber: '',
        Status: 'D',
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await procurementApi.getPayment(row.PaymentId)
        if (response.Data) {
          Object.assign(formData, {
            PaymentId: response.Data.PaymentId,
            PaymentDate: formatDateForInput(response.Data.PaymentDate),
            SupplierId: response.Data.SupplierId,
            PaymentType: response.Data.PaymentType,
            Amount: response.Data.Amount,
            CurrencyId: response.Data.CurrencyId || 'TWD',
            ExchangeRate: response.Data.ExchangeRate || 1,
            BankAccountId: response.Data.BankAccountId || '',
            CheckNumber: response.Data.CheckNumber || '',
            Status: response.Data.Status,
            Memo: response.Data.Memo || ''
          })
          isEdit.value = true
          dialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 確認付款單
    const handleConfirm = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要確認付款單「${row.PaymentId}」嗎？`,
          '確認付款單',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await procurementApi.confirmPayment(row.PaymentId)
        ElMessage.success('確認成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('確認失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除付款單「${row.PaymentId}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await procurementApi.deletePayment(row.PaymentId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          await procurementApi.updatePayment(formData.PaymentId, {
            PaymentDate: formData.PaymentDate,
            SupplierId: formData.SupplierId,
            PaymentType: formData.PaymentType,
            Amount: formData.Amount,
            CurrencyId: formData.CurrencyId,
            ExchangeRate: formData.ExchangeRate,
            BankAccountId: formData.BankAccountId,
            CheckNumber: formData.CheckNumber,
            Status: formData.Status,
            Memo: formData.Memo
          })
          ElMessage.success('修改成功')
        } else {
          await procurementApi.createPayment({
            PaymentId: formData.PaymentId,
            PaymentDate: formData.PaymentDate,
            SupplierId: formData.SupplierId,
            PaymentType: formData.PaymentType,
            Amount: formData.Amount,
            CurrencyId: formData.CurrencyId,
            ExchangeRate: formData.ExchangeRate,
            BankAccountId: formData.BankAccountId,
            CheckNumber: formData.CheckNumber,
            Status: formData.Status,
            Memo: formData.Memo
          })
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化日期時間
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 格式化日期用於輸入框
    const formatDateForInput = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (amount == null) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 取得付款類型標籤
    const getPaymentTypeLabel = (type) => {
      const labels = {
        'CASH': '現金',
        'CHECK': '支票',
        'TRANSFER': '轉帳',
        'OTHER': '其他'
      }
      return labels[type] || type
    }

    // 取得狀態標籤
    const getStatusLabel = (status) => {
      const labels = {
        'D': '草稿',
        'A': '已確認',
        'C': '已取消'
      }
      return labels[status] || status
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'D': 'info',
        'A': 'success',
        'C': 'danger'
      }
      return types[status] || 'info'
    }

    // 初始化
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
      handleDateRangeChange,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleConfirm,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange,
      formatDate,
      formatDateTime,
      formatCurrency,
      getPaymentTypeLabel,
      getStatusLabel,
      getStatusType
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.procurement-payment {
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
    
    .search-form {
      .el-form-item {
        margin-bottom: 0;
      }
    }
  }

  .table-card {
    .el-table {
      .el-button {
        margin-right: 5px;
      }
    }
  }
}
</style>

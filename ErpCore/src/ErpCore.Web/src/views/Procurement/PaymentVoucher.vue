<template>
  <div class="procurement-payment-voucher">
    <div class="page-header">
      <h1>付款單維護 (SYSP271-SYSP2B0)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="付款單號">
          <el-input v-model="queryForm.PaymentNo" placeholder="請輸入付款單號" clearable />
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
        <el-form-item label="付款方式">
          <el-select v-model="queryForm.PaymentMethod" placeholder="請選擇付款方式" clearable>
            <el-option label="現金" value="CASH" />
            <el-option label="支票" value="CHECK" />
            <el-option label="轉帳" value="TRANSFER" />
            <el-option label="其他" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="DRAFT" />
            <el-option label="確認" value="CONFIRMED" />
            <el-option label="已付款" value="PAID" />
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
        <el-table-column prop="PaymentNo" label="付款單號" width="150" />
        <el-table-column prop="PaymentDate" label="付款日期" width="120" />
        <el-table-column prop="SupplierId" label="供應商代號" width="120" />
        <el-table-column prop="SupplierName" label="供應商名稱" width="200" />
        <el-table-column prop="PaymentAmount" label="付款金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.PaymentAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="PaymentMethod" label="付款方式" width="100" />
        <el-table-column prop="BankAccount" label="銀行帳號" width="150" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Verifier" label="審核者" width="100" />
        <el-table-column prop="VerifyDate" label="審核日期" width="120" />
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
              type="danger" 
              size="small" 
              @click="handleDelete(row)"
            >
              刪除
            </el-button>
            <el-button 
              v-if="row.Status === 'DRAFT'" 
              type="success" 
              size="small" 
              @click="handleConfirm(row)"
            >
              確認
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
            <el-form-item label="付款單號" prop="PaymentNo">
              <el-input v-model="formData.PaymentNo" :disabled="isEdit" placeholder="請輸入付款單號" />
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
            <el-form-item label="付款金額" prop="PaymentAmount">
              <el-input-number 
                v-model="formData.PaymentAmount" 
                :min="0" 
                :precision="2"
                style="width: 100%"
                placeholder="請輸入付款金額"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="付款方式" prop="PaymentMethod">
              <el-select v-model="formData.PaymentMethod" placeholder="請選擇付款方式" style="width: 100%">
                <el-option label="現金" value="CASH" />
                <el-option label="支票" value="CHECK" />
                <el-option label="轉帳" value="TRANSFER" />
                <el-option label="其他" value="OTHER" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="銀行帳號" prop="BankAccount">
              <el-input v-model="formData.BankAccount" placeholder="請輸入銀行帳號" />
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
import { procurementApi } from '@/api/procurement'

export default {
  name: 'ProcurementPaymentVoucher',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const dateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      PaymentNo: '',
      PaymentDateFrom: null,
      PaymentDateTo: null,
      SupplierId: '',
      PaymentMethod: '',
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
      PaymentNo: '',
      PaymentDate: '',
      SupplierId: '',
      PaymentAmount: 0,
      PaymentMethod: '',
      BankAccount: '',
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      PaymentNo: [{ required: true, message: '請輸入付款單號', trigger: 'blur' }],
      PaymentDate: [{ required: true, message: '請選擇付款日期', trigger: 'change' }],
      SupplierId: [{ required: true, message: '請輸入供應商代號', trigger: 'blur' }],
      PaymentAmount: [{ required: true, message: '請輸入付款金額', trigger: 'blur' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改付款單' : '新增付款單'
    })

    // 格式化貨幣
    const formatCurrency = (value) => {
      if (!value) return '0.00'
      return new Intl.NumberFormat('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      }).format(value)
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const statusMap = {
        'DRAFT': 'info',
        'CONFIRMED': 'warning',
        'PAID': 'success'
      }
      return statusMap[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const statusMap = {
        'DRAFT': '草稿',
        'CONFIRMED': '確認',
        'PAID': '已付款'
      }
      return statusMap[status] || status
    }

    // 日期範圍變更
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
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await procurementApi.getPaymentVouchers(params)
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
      dateRange.value = null
      Object.assign(queryForm, {
        PaymentNo: '',
        PaymentDateFrom: null,
        PaymentDateTo: null,
        SupplierId: '',
        PaymentMethod: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        PaymentNo: '',
        PaymentDate: '',
        SupplierId: '',
        PaymentAmount: 0,
        PaymentMethod: '',
        BankAccount: '',
        Notes: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await procurementApi.getPaymentVoucher(row.PaymentNo)
        if (response.Data) {
          Object.assign(formData, {
            PaymentNo: response.Data.PaymentNo,
            PaymentDate: response.Data.PaymentDate,
            SupplierId: response.Data.SupplierId,
            PaymentAmount: response.Data.PaymentAmount,
            PaymentMethod: response.Data.PaymentMethod || '',
            BankAccount: response.Data.BankAccount || '',
            Notes: response.Data.Notes || ''
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

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除付款單「${row.PaymentNo}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await procurementApi.deletePaymentVoucher(row.PaymentNo)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 確認
    const handleConfirm = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要確認付款單「${row.PaymentNo}」嗎？`,
          '確認付款單',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await procurementApi.confirmPaymentVoucher(row.PaymentNo)
        ElMessage.success('確認成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('確認失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          await procurementApi.updatePaymentVoucher(formData.PaymentNo, formData)
          ElMessage.success('修改成功')
        } else {
          await procurementApi.createPaymentVoucher(formData)
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
      dateRange,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
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
      handleConfirm,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.procurement-payment-voucher {
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

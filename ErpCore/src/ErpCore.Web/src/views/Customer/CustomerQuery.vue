<template>
  <div class="customer-query">
    <div class="page-header">
      <h1>客戶查詢作業 (CUS5120)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="客戶編號">
              <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶編號" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客戶名稱">
              <el-input v-model="queryForm.CustomerName" placeholder="請輸入客戶名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="統一編號">
              <el-input v-model="queryForm.GuiId" placeholder="請輸入統一編號" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="狀態">
              <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="交易日期起">
              <el-date-picker v-model="queryForm.TransDateFrom" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="交易日期迄">
              <el-date-picker v-model="queryForm.TransDateTo" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出Excel</el-button>
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
        <el-table-column prop="CustomerId" label="客戶編號" width="150" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="GuiId" label="統一編號" width="120" />
        <el-table-column prop="ContactStaff" label="聯絡人" width="120" />
        <el-table-column prop="CompTel" label="公司電話" width="150" />
        <el-table-column prop="Cell" label="手機" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="AccAmt" label="累積金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.AccAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransDate" label="最近交易日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TransDate) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewTransactions(row)">交易記錄</el-button>
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

    <!-- 交易記錄對話框 -->
    <el-dialog
      v-model="transactionDialogVisible"
      title="客戶交易記錄"
      width="80%"
    >
      <el-table :data="transactionData" border>
        <el-table-column prop="TransactionDate" label="交易日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TransactionDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransactionNo" label="交易序號" width="150" />
        <el-table-column prop="TransactionType" label="交易類型" width="120" />
        <el-table-column prop="Amount" label="金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100" />
        <el-table-column prop="Notes" label="備註" />
      </el-table>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { customerQueryApi } from '@/api/customer'

export default {
  name: 'CustomerQuery',
  setup() {
    const loading = ref(false)
    const transactionDialogVisible = ref(false)
    const tableData = ref([])
    const transactionData = ref([])
    const currentCustomerId = ref('')

    // 查詢表單
    const queryForm = reactive({
      CustomerId: '',
      CustomerName: '',
      GuiId: '',
      Status: '',
      TransDateFrom: null,
      TransDateTo: null
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (!amount) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const data = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          Filters: {
            ...queryForm
          }
        }
        const response = await customerQueryApi.advancedQuery(data)
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
        CustomerId: '',
        CustomerName: '',
        GuiId: '',
        Status: '',
        TransDateFrom: null,
        TransDateTo: null
      })
      handleSearch()
    }

    // 匯出
    const handleExport = () => {
      ElMessage.info('匯出功能開發中')
    }

    // 查看交易記錄
    const handleViewTransactions = async (row) => {
      currentCustomerId.value = row.CustomerId
      transactionDialogVisible.value = true
      try {
        const response = await customerQueryApi.getTransactions(row.CustomerId, {
          PageIndex: 1,
          PageSize: 100
        })
        if (response.Data) {
          transactionData.value = response.Data.Items || []
        }
      } catch (error) {
        ElMessage.error('查詢交易記錄失敗: ' + (error.message || '未知錯誤'))
      }
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

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      transactionDialogVisible,
      tableData,
      transactionData,
      queryForm,
      pagination,
      formatDate,
      formatCurrency,
      handleSearch,
      handleReset,
      handleExport,
      handleViewTransactions,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
.customer-query {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


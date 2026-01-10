<template>
  <div class="financial-report">
    <div class="page-header">
      <h1>財務報表 (SYSN510-SYSN540)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表類型" prop="ReportType">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable style="width: 200px">
            <el-option label="損益表" value="INCOME" />
            <el-option label="資產負債表" value="BALANCE" />
            <el-option label="現金流量表" value="CASHFLOW" />
          </el-select>
        </el-form-item>
        <el-form-item label="日期範圍">
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
        <el-form-item label="會計科目">
          <el-input v-model="queryForm.StypeId" placeholder="請輸入會計科目" clearable />
        </el-form-item>
        <el-form-item label="店別">
          <el-input v-model="queryForm.SiteId" placeholder="請輸入店別" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
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
        :summary-method="getSummaries"
        show-summary
      >
        <el-table-column prop="StypeId" label="會計科目" width="150" />
        <el-table-column prop="StypeName" label="科目名稱" width="200" />
        <el-table-column prop="DebitAmount" label="借方金額" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.DebitAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreditAmount" label="貸方金額" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.CreditAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Balance" label="餘額" width="150" align="right">
          <template #default="{ row }">
            <span :class="row.Balance >= 0 ? 'balance-positive' : 'balance-negative'">
              {{ formatCurrency(row.Balance) }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="ReportDate" label="報表日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ReportDate) }}
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
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { accountingApi } from '@/api/accounting'

export default {
  name: 'FinancialReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const dateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      ReportType: '',
      DateFrom: null,
      DateTo: null,
      StypeId: '',
      SiteId: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

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

    // 日期範圍變更
    const handleDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        queryForm.DateFrom = dates[0]
        queryForm.DateTo = dates[1]
      } else {
        queryForm.DateFrom = null
        queryForm.DateTo = null
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
        const response = await accountingApi.getFinancialReports(params)
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
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm, {
        ReportType: '',
        DateFrom: null,
        DateTo: null,
        StypeId: '',
        SiteId: ''
      })
      dateRange.value = null
      tableData.value = []
      pagination.TotalCount = 0
    }

    // 匯出
    const handleExport = async () => {
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      try {
        const params = {
          Query: queryForm,
          ExportFormat: 'EXCEL'
        }
        const response = await accountingApi.exportFinancialReport(params)
        // 創建下載連結
        const blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `財務報表_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
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

    // 合計
    const getSummaries = (param) => {
      const { columns, data } = param
      const sums = []
      columns.forEach((column, index) => {
        if (index === 0) {
          sums[index] = '合計'
          return
        }
        if (column.property === 'DebitAmount' || column.property === 'CreditAmount' || column.property === 'Balance') {
          const values = data.map(item => Number(item[column.property]))
          if (!values.every(value => isNaN(value))) {
            sums[index] = formatCurrency(values.reduce((prev, curr) => {
              const value = Number(curr)
              if (!isNaN(value)) {
                return prev + curr
              } else {
                return prev
              }
            }, 0))
          } else {
            sums[index] = '-'
          }
        } else {
          sums[index] = '-'
        }
      })
      return sums
    }

    onMounted(() => {
      // 不自動載入，需要選擇報表類型後才能查詢
    })

    return {
      loading,
      tableData,
      queryForm,
      pagination,
      dateRange,
      formatDate,
      formatCurrency,
      handleDateRangeChange,
      handleSearch,
      handleReset,
      handleExport,
      handleSizeChange,
      handlePageChange,
      getSummaries
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.financial-report {
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

  .balance-positive {
    color: $primary-color; // 使用主色變數
  }

  .balance-negative {
    color: $danger-color; // 使用危險色變數
  }
}
</style>


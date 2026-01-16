<template>
  <div class="eca4040-report">
    <div class="page-header">
      <h1>電子發票報表查詢(店別銷售統計) (ECA4040)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="店別">
          <el-select
            v-model="queryForm.StoreId"
            placeholder="請選擇店別"
            filterable
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="store in storeList"
              :key="store.StoreId"
              :label="store.StoreName"
              :value="store.StoreId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="樓層">
          <el-input
            v-model="queryForm.StoreFloor"
            placeholder="請輸入樓層"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="類型">
          <el-input
            v-model="queryForm.StoreType"
            placeholder="請輸入類型"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="訂單日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel">匯出Excel</el-button>
          <el-button type="warning" @click="handleExportPdf">匯出PDF</el-button>
          <el-button type="info" @click="handlePrint">列印</el-button>
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
        <el-table-column prop="SalesRanking" label="排名" width="80" align="center" />
        <el-table-column prop="StoreId" label="店別代碼" width="150" />
        <el-table-column prop="StoreName" label="店別名稱" width="200" show-overflow-tooltip />
        <el-table-column prop="StoreFloor" label="樓層" width="100" />
        <el-table-column prop="StoreType" label="類型" width="100" />
        <el-table-column prop="OrderQty" label="銷售數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.OrderQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderSubtotal" label="銷售金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.OrderSubtotal) }}
          </template>
        </el-table-column>
        <el-table-column prop="ShippingFee" label="運費" width="100" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.ShippingFee) }}
          </template>
        </el-table-column>
        <el-table-column prop="SalesPercent" label="銷售占比(%)" width="120" align="right">
          <template #default="{ row }">
            {{ formatPercent(row.SalesPercent) }}
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px"
      />

      <!-- 統計資訊 -->
      <div class="summary-info" style="margin-top: 20px">
        <el-row :gutter="20">
          <el-col :span="6">
            <div class="summary-item">
              <span class="label">總筆數：</span>
              <span class="value">{{ pagination.totalCount }}</span>
            </div>
          </el-col>
          <el-col :span="6">
            <div class="summary-item">
              <span class="label">總數量：</span>
              <span class="value">{{ formatNumber(totalQty) }}</span>
            </div>
          </el-col>
          <el-col :span="6">
            <div class="summary-item">
              <span class="label">總金額：</span>
              <span class="value">{{ formatCurrency(totalAmount) }}</span>
            </div>
          </el-col>
          <el-col :span="6">
            <div class="summary-item">
              <span class="label">總運費：</span>
              <span class="value">{{ formatCurrency(totalFee) }}</span>
            </div>
          </el-col>
        </el-row>
      </div>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage } from 'element-plus'
import einvoiceReportApi from '@/api/einvoice'

export default {
  name: 'ECA4040Report',
  setup() {
    const loading = ref(false)
    const exporting = ref(false)
    const tableData = ref([])
    const storeList = ref([])
    const dateRange = ref(null)

    const queryForm = reactive({
      ReportType: 'ECA4040',
      StoreId: '',
      StoreFloor: '',
      StoreType: '',
      OrderDateFrom: null,
      OrderDateTo: null,
      PageIndex: 1,
      PageSize: 20,
      SortField: 'SalesRanking',
      SortOrder: 'ASC'
    })

    const pagination = reactive({
      pageIndex: 1,
      pageSize: 20,
      totalCount: 0
    })

    // 計算統計資訊
    const totalQty = computed(() => {
      return tableData.value.reduce((sum, item) => sum + (item.OrderQty || 0), 0)
    })

    const totalAmount = computed(() => {
      return tableData.value.reduce((sum, item) => sum + (item.OrderSubtotal || 0), 0)
    })

    const totalFee = computed(() => {
      return tableData.value.reduce((sum, item) => sum + (item.ShippingFee || 0), 0)
    })

    // 載入店別列表
    const loadStoreList = async () => {
      try {
        // TODO: 載入店別列表 API
        // const response = await storeApi.getStores()
        // storeList.value = response.data || []
      } catch (error) {
        console.error('載入店別列表失敗', error)
      }
    }

    // 查詢
    const handleSearch = async () => {
      try {
        loading.value = true

        // 處理日期範圍
        if (dateRange.value && dateRange.value.length === 2) {
          queryForm.OrderDateFrom = dateRange.value[0]
          queryForm.OrderDateTo = dateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }

        queryForm.PageIndex = pagination.pageIndex
        queryForm.PageSize = pagination.pageSize

        const response = await einvoiceReportApi.getReports(queryForm)

        if (response.success) {
          tableData.value = response.data.items || []
          pagination.totalCount = response.data.totalCount || 0
        } else {
          ElMessage.error(response.message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢失敗', error)
        ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 重置
    const handleReset = () => {
      queryForm.StoreId = ''
      queryForm.StoreFloor = ''
      queryForm.StoreType = ''
      queryForm.OrderDateFrom = null
      queryForm.OrderDateTo = null
      dateRange.value = null
      pagination.pageIndex = 1
      pagination.pageSize = 20
      tableData.value = []
      pagination.totalCount = 0
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.pageSize = size
      pagination.pageIndex = 1
      handleSearch()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.pageIndex = page
      handleSearch()
    }

    // 匯出 Excel
    const handleExportExcel = async () => {
      try {
        exporting.value = true

        // 處理日期範圍
        if (dateRange.value && dateRange.value.length === 2) {
          queryForm.OrderDateFrom = dateRange.value[0]
          queryForm.OrderDateTo = dateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }

        const response = await einvoiceReportApi.exportExcel(queryForm)

        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `電子發票報表_店別銷售統計_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)

        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗', error)
        ElMessage.error('匯出失敗：' + (error.message || '未知錯誤'))
      } finally {
        exporting.value = false
      }
    }

    // 匯出 PDF
    const handleExportPdf = async () => {
      try {
        exporting.value = true

        // 處理日期範圍
        if (dateRange.value && dateRange.value.length === 2) {
          queryForm.OrderDateFrom = dateRange.value[0]
          queryForm.OrderDateTo = dateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }

        const response = await einvoiceReportApi.exportPdf(queryForm)

        // 下載檔案
        const blob = new Blob([response], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `電子發票報表_店別銷售統計_${new Date().toISOString().slice(0, 10)}.pdf`
        link.click()
        window.URL.revokeObjectURL(url)

        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗', error)
        ElMessage.error('匯出失敗：' + (error.message || '未知錯誤'))
      } finally {
        exporting.value = false
      }
    }

    // 列印
    const handlePrint = async () => {
      try {
        exporting.value = true

        // 處理日期範圍
        if (dateRange.value && dateRange.value.length === 2) {
          queryForm.OrderDateFrom = dateRange.value[0]
          queryForm.OrderDateTo = dateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }

        const response = await einvoiceReportApi.print(queryForm)

        // 在新視窗中打開 PDF
        const blob = new Blob([response], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        window.URL.revokeObjectURL(url)
      } catch (error) {
        console.error('列印失敗', error)
        ElMessage.error('列印失敗：' + (error.message || '未知錯誤'))
      } finally {
        exporting.value = false
      }
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return d.toLocaleDateString('zh-TW', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
      })
    }

    // 格式化貨幣
    const formatCurrency = (value) => {
      if (value == null) return '0.00'
      return Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    }

    // 格式化數字
    const formatNumber = (value) => {
      if (value == null) return '0'
      return Number(value).toLocaleString('zh-TW')
    }

    // 格式化百分比
    const formatPercent = (value) => {
      if (value == null) return '0.00'
      return Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    }

    // 初始化
    onMounted(() => {
      loadStoreList()
    })

    return {
      loading,
      exporting,
      tableData,
      storeList,
      dateRange,
      queryForm,
      pagination,
      totalQty,
      totalAmount,
      totalFee,
      handleSearch,
      handleReset,
      handleSizeChange,
      handlePageChange,
      handleExportExcel,
      handleExportPdf,
      handlePrint,
      formatDate,
      formatCurrency,
      formatNumber,
      formatPercent
    }
  }
}
</script>

<style scoped>
.eca4040-report {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin-top: 10px;
}

.table-card {
  margin-bottom: 20px;
}

.summary-info {
  padding: 15px;
  background-color: #f5f7fa;
  border-radius: 4px;
}

.summary-item {
  display: flex;
  align-items: center;
}

.summary-item .label {
  font-weight: 500;
  margin-right: 10px;
}

.summary-item .value {
  font-size: 18px;
  font-weight: 600;
  color: #409eff;
}
</style>

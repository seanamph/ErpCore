<template>
  <div class="purchase-order-query">
    <div class="page-header">
      <h1>採購單查詢 (SYSP310-SYSP330)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="採購單號">
          <el-input v-model="queryForm.Filters.OrderId" placeholder="請輸入採購單號" clearable />
        </el-form-item>
        <el-form-item label="單據類型">
          <el-select v-model="queryForm.Filters.OrderType" placeholder="請選擇單據類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="採購" value="PO" />
            <el-option label="退貨" value="RT" />
          </el-select>
        </el-form-item>
        <el-form-item label="分店">
          <el-input v-model="queryForm.Filters.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="供應商">
          <el-input v-model="queryForm.Filters.SupplierId" placeholder="請輸入供應商代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Filters.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="草稿" value="D" />
            <el-option label="已送出" value="S" />
            <el-option label="已審核" value="A" />
            <el-option label="已取消" value="X" />
            <el-option label="已結案" value="C" />
          </el-select>
        </el-form-item>
        <el-form-item label="採購日期">
          <el-date-picker
            v-model="orderDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="申請人">
          <el-input v-model="queryForm.Filters.ApplyUserId" placeholder="請輸入申請人代碼" clearable />
        </el-form-item>
        <el-form-item label="審核人">
          <el-input v-model="queryForm.Filters.ApproveUserId" placeholder="請輸入審核人代碼" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery" :loading="loading">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport" :loading="exporting">匯出</el-button>
          <el-button type="info" @click="handleShowStatistics">統計</el-button>
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
        <el-table-column prop="OrderId" label="採購單號" width="150" fixed="left" />
        <el-table-column prop="OrderDate" label="採購日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderTypeName" label="單據類型" width="100" />
        <el-table-column prop="ShopName" label="分店" width="120" />
        <el-table-column prop="SupplierName" label="供應商" width="150" />
        <el-table-column prop="StatusName" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ row.StatusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalQty" label="總數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="DetailCount" label="明細筆數" width="100" align="center" />
        <el-table-column prop="TotalReceivedQty" label="已收數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalReceivedQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalReturnQty" label="已退數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalReturnQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="ApplyUserName" label="申請人" width="100" />
        <el-table-column prop="ApplyDate" label="申請日期" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.ApplyDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ApproveUserName" label="審核人" width="100" />
        <el-table-column prop="ApproveDate" label="審核日期" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.ApproveDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ExpectedDate" label="預期交貨日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ExpectedDate) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewDetails(row)">明細</el-button>
            <el-button type="success" size="small" @click="handlePrint(row)">列印</el-button>
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

    <!-- 明細對話框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="採購單明細"
      width="90%"
      :close-on-click-modal="false"
    >
      <el-table :data="detailData" border stripe>
        <el-table-column prop="LineNum" label="行號" width="80" align="center" />
        <el-table-column prop="GoodsId" label="商品編號" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" min-width="200" />
        <el-table-column prop="BarcodeId" label="條碼編號" width="120" />
        <el-table-column prop="OrderQty" label="訂購數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.OrderQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="UnitPrice" label="單價" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.UnitPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="ReceivedQty" label="已收數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.ReceivedQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="ReturnQty" label="已退數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.ReturnQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="PendingQty" label="待收數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.PendingQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="UnitId" label="單位" width="80" />
        <el-table-column prop="TaxRate" label="稅率" width="100" align="right">
          <template #default="{ row }">
            {{ formatPercent(row.TaxRate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TaxAmount" label="稅額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TaxAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Memo" label="備註" min-width="200" />
      </el-table>
    </el-dialog>

    <!-- 統計對話框 -->
    <el-dialog
      v-model="statisticsDialogVisible"
      title="採購單統計"
      width="80%"
      :close-on-click-modal="false"
    >
      <el-form :inline="true" :model="statisticsForm" style="margin-bottom: 20px;">
        <el-form-item label="分組方式">
          <el-select v-model="statisticsForm.GroupBy" placeholder="請選擇分組方式">
            <el-option label="按供應商" value="supplier" />
            <el-option label="按分店" value="shop" />
            <el-option label="按商品" value="goods" />
            <el-option label="按狀態" value="status" />
            <el-option label="按日期" value="date" />
          </el-select>
        </el-form-item>
        <el-form-item label="日期範圍">
          <el-date-picker
            v-model="statisticsDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            @change="handleStatisticsDateRangeChange"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadStatistics">查詢</el-button>
        </el-form-item>
      </el-form>

      <el-descriptions :column="2" border v-if="statisticsData.Summary">
        <el-descriptions-item label="總訂單數">{{ statisticsData.Summary.TotalOrders }}</el-descriptions-item>
        <el-descriptions-item label="總金額">{{ formatCurrency(statisticsData.Summary.TotalAmount) }}</el-descriptions-item>
        <el-descriptions-item label="總數量">{{ formatNumber(statisticsData.Summary.TotalQty) }}</el-descriptions-item>
        <el-descriptions-item label="平均金額">{{ formatCurrency(statisticsData.Summary.AvgAmount) }}</el-descriptions-item>
      </el-descriptions>

      <el-table :data="statisticsData.Details" border stripe style="margin-top: 20px;">
        <el-table-column prop="GroupName" label="分組名稱" min-width="200" />
        <el-table-column prop="OrderCount" label="訂單數" width="120" align="right" />
        <el-table-column prop="TotalAmount" label="總金額" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalQty" label="總數量" width="150" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalQty) }}
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { purchaseOrderQueryApi } from '@/api/purchase'

export default {
  name: 'PurchaseOrderQuery',
  setup() {
    const loading = ref(false)
    const exporting = ref(false)
    const tableData = ref([])
    const detailData = ref([])
    const detailDialogVisible = ref(false)
    const statisticsDialogVisible = ref(false)
    const orderDateRange = ref(null)
    const statisticsDateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      PageIndex: 1,
      PageSize: 20,
      SortField: 'OrderDate',
      SortOrder: 'DESC',
      Filters: {
        OrderId: '',
        OrderType: '',
        ShopId: '',
        SupplierId: '',
        Status: '',
        OrderDateFrom: null,
        OrderDateTo: null,
        ApplyUserId: '',
        ApproveUserId: '',
        ExpectedDateFrom: null,
        ExpectedDateTo: null,
        MinTotalAmount: null,
        MaxTotalAmount: null
      }
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 統計表單
    const statisticsForm = reactive({
      GroupBy: 'supplier',
      DateFrom: null,
      DateTo: null,
      ShopId: '',
      SupplierId: '',
      Status: ''
    })

    // 統計資料
    const statisticsData = reactive({
      Summary: {
        TotalOrders: 0,
        TotalAmount: 0,
        TotalQty: 0,
        AvgAmount: 0
      },
      Details: []
    })

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

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (!amount && amount !== 0) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 格式化數字
    const formatNumber = (num) => {
      if (!num && num !== 0) return '0'
      return Number(num).toLocaleString('zh-TW')
    }

    // 格式化百分比
    const formatPercent = (num) => {
      if (!num && num !== 0) return '0%'
      return `${Number(num).toFixed(2)}%`
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const statusMap = {
        'D': 'info',
        'S': 'warning',
        'A': 'success',
        'X': 'danger',
        'C': ''
      }
      return statusMap[status] || 'info'
    }

    // 日期範圍變更
    const handleDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        queryForm.Filters.OrderDateFrom = dates[0]
        queryForm.Filters.OrderDateTo = dates[1]
      } else {
        queryForm.Filters.OrderDateFrom = null
        queryForm.Filters.OrderDateTo = null
      }
    }

    // 統計日期範圍變更
    const handleStatisticsDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        statisticsForm.DateFrom = dates[0]
        statisticsForm.DateTo = dates[1]
      } else {
        statisticsForm.DateFrom = null
        statisticsForm.DateTo = null
      }
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          SortField: queryForm.SortField,
          SortOrder: queryForm.SortOrder,
          Filters: {
            ...queryForm.Filters
          }
        }
        const response = await purchaseOrderQueryApi.queryPurchaseOrders(params)
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
    const handleQuery = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm.Filters, {
        OrderId: '',
        OrderType: '',
        ShopId: '',
        SupplierId: '',
        Status: '',
        OrderDateFrom: null,
        OrderDateTo: null,
        ApplyUserId: '',
        ApproveUserId: '',
        ExpectedDateFrom: null,
        ExpectedDateTo: null,
        MinTotalAmount: null,
        MaxTotalAmount: null
      })
      orderDateRange.value = null
      handleQuery()
    }

    // 匯出
    const handleExport = async () => {
      exporting.value = true
      try {
        const data = {
          Filters: { ...queryForm.Filters },
          ExportType: 'excel',
          IncludeDetails: false
        }
        const response = await purchaseOrderQueryApi.exportPurchaseOrders(data)
        
        // 創建下載連結
        const blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `採購單查詢結果_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      } finally {
        exporting.value = false
      }
    }

    // 顯示統計
    const handleShowStatistics = () => {
      statisticsDialogVisible.value = true
      loadStatistics()
    }

    // 載入統計資料
    const loadStatistics = async () => {
      try {
        const params = {
          GroupBy: statisticsForm.GroupBy,
          DateFrom: statisticsForm.DateFrom,
          DateTo: statisticsForm.DateTo,
          ShopId: statisticsForm.ShopId,
          SupplierId: statisticsForm.SupplierId,
          Status: statisticsForm.Status
        }
        const response = await purchaseOrderQueryApi.getPurchaseOrderStatistics(params)
        if (response.Data) {
          Object.assign(statisticsData.Summary, response.Data.Summary || {})
          statisticsData.Details = response.Data.Details || []
        }
      } catch (error) {
        ElMessage.error('查詢統計失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 查看明細
    const handleViewDetails = async (row) => {
      try {
        const response = await purchaseOrderQueryApi.getPurchaseOrderDetails(row.OrderId)
        if (response.Data) {
          detailData.value = response.Data.Details || []
          detailDialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('查詢明細失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 列印
    const handlePrint = async (row) => {
      try {
        const response = await purchaseOrderQueryApi.printPurchaseOrder(row.OrderId)
        
        // 創建下載連結
        const blob = new Blob([response], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `採購單_${row.OrderId}.pdf`
        link.click()
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('列印成功')
      } catch (error) {
        ElMessage.error('列印失敗: ' + (error.message || '未知錯誤'))
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
      exporting,
      tableData,
      detailData,
      detailDialogVisible,
      statisticsDialogVisible,
      orderDateRange,
      statisticsDateRange,
      queryForm,
      pagination,
      statisticsForm,
      statisticsData,
      formatDate,
      formatDateTime,
      formatCurrency,
      formatNumber,
      formatPercent,
      getStatusType,
      handleDateRangeChange,
      handleStatisticsDateRangeChange,
      handleQuery,
      handleReset,
      handleExport,
      handleShowStatistics,
      loadStatistics,
      handleViewDetails,
      handlePrint,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style scoped>
.purchase-order-query {
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
  margin: 0;
}

.table-card {
  margin-bottom: 20px;
}
</style>

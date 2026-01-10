<template>
  <div class="sales-report-management">
    <div class="page-header">
      <h1>銷售報表查詢 (SYSD310-SYSD430)</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- 銷售明細報表 Tab -->
      <el-tab-pane label="銷售明細報表" name="detail">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="detailQueryForm" class="search-form">
            <el-form-item label="單據類型">
              <el-select v-model="detailQueryForm.OrderType" placeholder="請選擇單據類型" clearable>
                <el-option label="全部" value="" />
                <el-option label="銷售" value="SO" />
                <el-option label="退貨" value="RT" />
              </el-select>
            </el-form-item>
            <el-form-item label="分店代碼">
              <el-input v-model="detailQueryForm.ShopId" placeholder="請輸入分店代碼" clearable />
            </el-form-item>
            <el-form-item label="客戶代碼">
              <el-input v-model="detailQueryForm.CustomerId" placeholder="請輸入客戶代碼" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="detailQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="全部" value="" />
                <el-option label="已審核" value="A" />
                <el-option label="已出貨" value="O" />
                <el-option label="已結案" value="C" />
              </el-select>
            </el-form-item>
            <el-form-item label="銷售日期">
              <el-date-picker
                v-model="detailQueryForm.OrderDateRange"
                type="daterange"
                range-separator="至"
                start-placeholder="開始日期"
                end-placeholder="結束日期"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleDetailSearch">查詢</el-button>
              <el-button @click="handleDetailReset">重置</el-button>
              <el-button type="success" @click="handleDetailExport">匯出</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="detailTableData"
            v-loading="detailLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="OrderId" label="銷售單號" width="150" />
            <el-table-column prop="OrderDate" label="銷售日期" width="120">
              <template #default="{ row }">
                {{ formatDate(row.OrderDate) }}
              </template>
            </el-table-column>
            <el-table-column prop="ShopId" label="分店代碼" width="120" />
            <el-table-column prop="CustomerId" label="客戶代碼" width="120" />
            <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
            <el-table-column prop="GoodsId" label="商品編號" width="150" />
            <el-table-column prop="GoodsName" label="商品名稱" width="200" />
            <el-table-column prop="OrderQty" label="數量" width="100" align="right">
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
          </el-table>

          <el-pagination
            v-model:current-page="detailPagination.PageIndex"
            v-model:page-size="detailPagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="detailPagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleDetailSizeChange"
            @current-change="handleDetailPageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>

      <!-- 銷售統計報表 Tab -->
      <el-tab-pane label="銷售統計報表" name="statistics">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="statisticsQueryForm" class="search-form">
            <el-form-item label="單據類型">
              <el-select v-model="statisticsQueryForm.OrderType" placeholder="請選擇單據類型" clearable>
                <el-option label="全部" value="" />
                <el-option label="銷售" value="SO" />
                <el-option label="退貨" value="RT" />
              </el-select>
            </el-form-item>
            <el-form-item label="分店代碼">
              <el-input v-model="statisticsQueryForm.ShopId" placeholder="請輸入分店代碼" clearable />
            </el-form-item>
            <el-form-item label="客戶代碼">
              <el-input v-model="statisticsQueryForm.CustomerId" placeholder="請輸入客戶代碼" clearable />
            </el-form-item>
            <el-form-item label="銷售日期">
              <el-date-picker
                v-model="statisticsQueryForm.OrderDateRange"
                type="daterange"
                range-separator="至"
                start-placeholder="開始日期"
                end-placeholder="結束日期"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleStatisticsSearch">查詢</el-button>
              <el-button @click="handleStatisticsReset">重置</el-button>
              <el-button type="success" @click="handleStatisticsExport">匯出</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="statisticsTableData"
            v-loading="statisticsLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="ShopId" label="分店代碼" width="120" />
            <el-table-column prop="ShopName" label="分店名稱" width="200" />
            <el-table-column prop="CustomerId" label="客戶代碼" width="120" />
            <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
            <el-table-column prop="OrderCount" label="單據數量" width="120" align="right">
              <template #default="{ row }">
                {{ formatNumber(row.OrderCount) }}
              </template>
            </el-table-column>
            <el-table-column prop="TotalQty" label="總數量" width="120" align="right">
              <template #default="{ row }">
                {{ formatNumber(row.TotalQty) }}
              </template>
            </el-table-column>
            <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.TotalAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="AvgAmount" label="平均金額" width="120" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.AvgAmount) }}
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="statisticsPagination.PageIndex"
            v-model:page-size="statisticsPagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="statisticsPagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleStatisticsSizeChange"
            @current-change="handleStatisticsPageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { salesApi } from '@/api/sales'

export default {
  name: 'SalesReport',
  setup() {
    const activeTab = ref('detail')
    const detailLoading = ref(false)
    const statisticsLoading = ref(false)
    const detailTableData = ref([])
    const statisticsTableData = ref([])

    // 明細報表查詢表單
    const detailQueryForm = reactive({
      OrderType: '',
      ShopId: '',
      CustomerId: '',
      Status: '',
      OrderDateRange: null
    })

    // 統計報表查詢表單
    const statisticsQueryForm = reactive({
      OrderType: '',
      ShopId: '',
      CustomerId: '',
      OrderDateRange: null
    })

    // 明細報表分頁
    const detailPagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 統計報表分頁
    const statisticsPagination = reactive({
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
      if (!amount && amount !== 0) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 格式化數字
    const formatNumber = (num) => {
      if (!num && num !== 0) return '0'
      return Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 0, maximumFractionDigits: 4 })
    }

    // 載入明細報表資料
    const loadDetailData = async () => {
      detailLoading.value = true
      try {
        const params = {
          ...detailQueryForm,
          PageIndex: detailPagination.PageIndex,
          PageSize: detailPagination.PageSize
        }
        
        // 處理日期範圍
        if (detailQueryForm.OrderDateRange && detailQueryForm.OrderDateRange.length === 2) {
          params.OrderDateFrom = detailQueryForm.OrderDateRange[0]
          params.OrderDateTo = detailQueryForm.OrderDateRange[1]
        }
        delete params.OrderDateRange

        const response = await salesApi.getSalesDetailReport(params)
        if (response.data && response.data.Success) {
          detailTableData.value = response.data.Data.Items || []
          detailPagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢銷售明細報表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        detailLoading.value = false
      }
    }

    // 載入統計報表資料
    const loadStatisticsData = async () => {
      statisticsLoading.value = true
      try {
        const params = {
          ...statisticsQueryForm,
          PageIndex: statisticsPagination.PageIndex,
          PageSize: statisticsPagination.PageSize
        }
        
        // 處理日期範圍
        if (statisticsQueryForm.OrderDateRange && statisticsQueryForm.OrderDateRange.length === 2) {
          params.OrderDateFrom = statisticsQueryForm.OrderDateRange[0]
          params.OrderDateTo = statisticsQueryForm.OrderDateRange[1]
        }
        delete params.OrderDateRange

        const response = await salesApi.getSalesStatisticsReport(params)
        if (response.data && response.data.Success) {
          statisticsTableData.value = response.data.Data.Items || []
          statisticsPagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢銷售統計報表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        statisticsLoading.value = false
      }
    }

    // 明細報表查詢
    const handleDetailSearch = () => {
      detailPagination.PageIndex = 1
      loadDetailData()
    }

    // 明細報表重置
    const handleDetailReset = () => {
      Object.assign(detailQueryForm, {
        OrderType: '',
        ShopId: '',
        CustomerId: '',
        Status: '',
        OrderDateRange: null
      })
      handleDetailSearch()
    }

    // 明細報表匯出
    const handleDetailExport = () => {
      ElMessage.info('匯出功能開發中')
    }

    // 統計報表查詢
    const handleStatisticsSearch = () => {
      statisticsPagination.PageIndex = 1
      loadStatisticsData()
    }

    // 統計報表重置
    const handleStatisticsReset = () => {
      Object.assign(statisticsQueryForm, {
        OrderType: '',
        ShopId: '',
        CustomerId: '',
        OrderDateRange: null
      })
      handleStatisticsSearch()
    }

    // 統計報表匯出
    const handleStatisticsExport = () => {
      ElMessage.info('匯出功能開發中')
    }

    // 明細報表分頁大小變更
    const handleDetailSizeChange = (size) => {
      detailPagination.PageSize = size
      detailPagination.PageIndex = 1
      loadDetailData()
    }

    // 明細報表分頁變更
    const handleDetailPageChange = (page) => {
      detailPagination.PageIndex = page
      loadDetailData()
    }

    // 統計報表分頁大小變更
    const handleStatisticsSizeChange = (size) => {
      statisticsPagination.PageSize = size
      statisticsPagination.PageIndex = 1
      loadStatisticsData()
    }

    // 統計報表分頁變更
    const handleStatisticsPageChange = (page) => {
      statisticsPagination.PageIndex = page
      loadStatisticsData()
    }

    // 初始化
    onMounted(() => {
      loadDetailData()
    })

    return {
      activeTab,
      detailLoading,
      statisticsLoading,
      detailTableData,
      statisticsTableData,
      detailQueryForm,
      statisticsQueryForm,
      detailPagination,
      statisticsPagination,
      handleDetailSearch,
      handleDetailReset,
      handleDetailExport,
      handleStatisticsSearch,
      handleStatisticsReset,
      handleStatisticsExport,
      handleDetailSizeChange,
      handleDetailPageChange,
      handleStatisticsSizeChange,
      handleStatisticsPageChange,
      formatDate,
      formatCurrency,
      formatNumber
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sales-report-management {
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
      .el-tag {
        font-weight: 500;
      }
    }
  }
}
</style>


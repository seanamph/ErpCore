<template>
  <div class="quality-report">
    <div class="page-header">
      <h1>質量管理報表功能 (SYSQ310-SYSQ340)</h1>
    </div>

    <!-- Tab 切換 -->
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 零用金支付表報表 (SYSQ310) -->
      <el-tab-pane label="零用金支付表報表" name="payment">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="queryForm" class="search-form">
            <el-form-item label="分店代號">
              <el-input v-model="queryForm.SiteId" placeholder="請輸入分店代號" clearable />
            </el-form-item>
            <el-form-item label="保管人代碼">
              <el-input v-model="queryForm.KeepEmpId" placeholder="請輸入保管人代碼" clearable />
            </el-form-item>
            <el-form-item label="日期範圍">
              <el-date-picker
                v-model="queryForm.DateRange"
                type="daterange"
                range-separator="至"
                start-placeholder="開始日期"
                end-placeholder="結束日期"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSearchPayment">查詢</el-button>
              <el-button @click="handleReset">重置</el-button>
              <el-button type="success" @click="handleExport">匯出</el-button>
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
            <el-table-column prop="CashAmount" label="零用金金額" width="150" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.CashAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="CashStatus" label="狀態" width="120">
              <template #default="{ row }">
                <el-tag :type="getStatusType(row.CashStatus)">
                  {{ getStatusText(row.CashStatus) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="RequestDate" label="請款日期" width="120">
              <template #default="{ row }">
                {{ formatDate(row.RequestDate) }}
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

      <!-- 支付申請單報表 (SYSQ320) -->
      <el-tab-pane label="支付申請單報表" name="application">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="queryForm" class="search-form">
            <el-form-item label="分店代號">
              <el-input v-model="queryForm.SiteId" placeholder="請輸入分店代號" clearable />
            </el-form-item>
            <el-form-item label="保管人代碼">
              <el-input v-model="queryForm.KeepEmpId" placeholder="請輸入保管人代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSearchApplication">查詢</el-button>
              <el-button @click="handleReset">重置</el-button>
              <el-button type="success" @click="handleExport">匯出</el-button>
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
            <el-table-column prop="CashAmount" label="零用金金額" width="150" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.CashAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="RequestDate" label="請款日期" width="120">
              <template #default="{ row }">
                {{ formatDate(row.RequestDate) }}
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

      <!-- 零用金撥補明細表報表 (SYSQ330) -->
      <el-tab-pane label="零用金撥補明細表報表" name="replenishment">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="queryForm" class="search-form">
            <el-form-item label="分店代號">
              <el-input v-model="queryForm.SiteId" placeholder="請輸入分店代號" clearable />
            </el-form-item>
            <el-form-item label="保管人代碼">
              <el-input v-model="queryForm.KeepEmpId" placeholder="請輸入保管人代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSearchReplenishment">查詢</el-button>
              <el-button @click="handleReset">重置</el-button>
              <el-button type="success" @click="handleExport">匯出</el-button>
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
            <el-table-column prop="CashAmount" label="零用金金額" width="150" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.CashAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="RequestDate" label="請款日期" width="120">
              <template #default="{ row }">
                {{ formatDate(row.RequestDate) }}
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

      <!-- 零用金報表 (SYSQ340) -->
      <el-tab-pane label="零用金報表" name="report">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="queryForm" class="search-form">
            <el-form-item label="分店代號">
              <el-input v-model="queryForm.SiteId" placeholder="請輸入分店代號" clearable />
            </el-form-item>
            <el-form-item label="保管人代碼">
              <el-input v-model="queryForm.KeepEmpId" placeholder="請輸入保管人代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSearchReport">查詢</el-button>
              <el-button @click="handleReset">重置</el-button>
              <el-button type="success" @click="handleExport">匯出</el-button>
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
            <el-table-column prop="CashAmount" label="零用金金額" width="150" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.CashAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="CashStatus" label="狀態" width="120">
              <template #default="{ row }">
                <el-tag :type="getStatusType(row.CashStatus)">
                  {{ getStatusText(row.CashStatus) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="RequestDate" label="請款日期" width="120">
              <template #default="{ row }">
                {{ formatDate(row.RequestDate) }}
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { qualityReportApi } from '@/api/query'

// Tab 切換
const activeTab = ref('payment')

// 查詢表單
const queryForm = reactive({
  SiteId: '',
  KeepEmpId: '',
  DateRange: []
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

// Tab 切換
const handleTabChange = (tabName) => {
  tableData.value = []
  pagination.PageIndex = 1
  pagination.TotalCount = 0
}

// 查詢零用金支付表報表
const handleSearchPayment = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SiteId: queryForm.SiteId || undefined,
      KeepEmpId: queryForm.KeepEmpId || undefined,
      DateFrom: queryForm.DateRange?.[0] || undefined,
      DateTo: queryForm.DateRange?.[1] || undefined
    }
    const response = await qualityReportApi.getPcCashPaymentReport(params)
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

// 查詢支付申請單報表
const handleSearchApplication = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SiteId: queryForm.SiteId || undefined,
      KeepEmpId: queryForm.KeepEmpId || undefined
    }
    const response = await qualityReportApi.getPaymentApplicationReport(params)
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

// 查詢零用金撥補明細表報表
const handleSearchReplenishment = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SiteId: queryForm.SiteId || undefined,
      KeepEmpId: queryForm.KeepEmpId || undefined
    }
    const response = await qualityReportApi.getPcCashReplenishmentReport(params)
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

// 查詢零用金報表
const handleSearchReport = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SiteId: queryForm.SiteId || undefined,
      KeepEmpId: queryForm.KeepEmpId || undefined
    }
    const response = await qualityReportApi.getPcCashReport(params)
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
  queryForm.DateRange = []
  pagination.PageIndex = 1
}

// 匯出
const handleExport = () => {
  ElMessage.info('匯出功能開發中')
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  if (activeTab.value === 'payment') {
    handleSearchPayment()
  } else if (activeTab.value === 'application') {
    handleSearchApplication()
  } else if (activeTab.value === 'replenishment') {
    handleSearchReplenishment()
  } else if (activeTab.value === 'report') {
    handleSearchReport()
  }
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  if (activeTab.value === 'payment') {
    handleSearchPayment()
  } else if (activeTab.value === 'application') {
    handleSearchApplication()
  } else if (activeTab.value === 'replenishment') {
    handleSearchReplenishment()
  } else if (activeTab.value === 'report') {
    handleSearchReport()
  }
}

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

// 格式化貨幣
const formatCurrency = (value) => {
  if (value == null) return '0.00'
  return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 取得狀態類型
const getStatusType = (status) => {
  const types = {
    'APPLIED': 'info',
    'REQUESTED': 'warning',
    'TRANSFERRED': 'primary',
    'APPROVED': 'success'
  }
  return types[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const texts = {
    'APPLIED': '已申請',
    'REQUESTED': '已請款',
    'TRANSFERRED': '已拋轉',
    'APPROVED': '已審核'
  }
  return texts[status] || status
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.quality-report {
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


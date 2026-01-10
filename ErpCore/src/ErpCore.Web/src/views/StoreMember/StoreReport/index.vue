<template>
  <div class="store-report">
    <div class="page-header">
      <h1>商店報表查詢作業 (SYS3410-SYS3440)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" class="search-form" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="商店編號">
              <el-input v-model="queryForm.ShopId" placeholder="請輸入商店編號" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="商店名稱">
              <el-input v-model="queryForm.ShopName" placeholder="請輸入商店名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="報表類型">
              <el-select v-model="queryForm.ReportType" placeholder="請選擇" clearable>
                <el-option label="全部" value="" />
                <el-option label="銷售報表" value="SALES" />
                <el-option label="庫存報表" value="INVENTORY" />
                <el-option label="會員報表" value="MEMBER" />
                <el-option label="財務報表" value="FINANCIAL" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="開始日期">
              <el-date-picker
                v-model="queryForm.StartDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="結束日期">
              <el-date-picker
                v-model="queryForm.EndDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
          <el-button type="warning" @click="handlePrint">列印</el-button>
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
        <el-table-column prop="ShopId" label="商店編號" width="150" />
        <el-table-column prop="ShopName" label="商店名稱" width="200" />
        <el-table-column prop="ReportType" label="報表類型" width="120" />
        <el-table-column prop="ReportDate" label="報表日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ReportDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalSales" label="總銷售額" width="150">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalSales) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalOrders" label="總訂單數" width="120" />
        <el-table-column prop="TotalMembers" label="總會員數" width="120" />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { storeReportApi } from '@/api/storeMember'

// 查詢表單
const queryForm = reactive({
  ShopId: '',
  ShopName: '',
  ReportType: '',
  StartDate: '',
  EndDate: ''
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

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ShopId: queryForm.ShopId || undefined,
      ShopName: queryForm.ShopName || undefined,
      ReportType: queryForm.ReportType || undefined,
      StartDate: queryForm.StartDate || undefined,
      EndDate: queryForm.EndDate || undefined
    }
    const response = await storeReportApi.getStoreReports(params)
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
  Object.assign(queryForm, {
    ShopId: '',
    ShopName: '',
    ReportType: '',
    StartDate: '',
    EndDate: ''
  })
  pagination.PageIndex = 1
  handleSearch()
}

// 匯出
const handleExport = async () => {
  try {
    const response = await storeReportApi.exportStoreReport({
      ...queryForm
    })
    const blob = new Blob([response.data])
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `商店報表_${new Date().getTime()}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 列印
const handlePrint = async () => {
  try {
    const response = await storeReportApi.printStoreReport({
      ...queryForm
    })
    const blob = new Blob([response.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    window.open(url, '_blank')
    ElMessage.success('列印成功')
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
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

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// 格式化貨幣
const formatCurrency = (value) => {
  if (!value) return '0'
  return new Intl.NumberFormat('zh-TW', { style: 'currency', currency: 'TWD' }).format(value)
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.store-report {
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


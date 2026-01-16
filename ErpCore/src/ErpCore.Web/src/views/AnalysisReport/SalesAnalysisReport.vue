<template>
  <div class="sales-analysis-report">
    <div class="page-header">
      <h1>銷售分析報表</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="店別">
          <el-select
            v-model="queryForm.SiteId"
            placeholder="請選擇店別"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="site in siteList"
              :key="site.SiteId"
              :label="site.SiteName"
              :value="site.SiteId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="日期起">
          <el-date-picker
            v-model="queryForm.DateFrom"
            type="date"
            placeholder="請選擇日期起"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="日期迄">
          <el-date-picker
            v-model="queryForm.DateTo"
            type="date"
            placeholder="請選擇日期迄"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="大分類">
          <el-select
            v-model="queryForm.BigClassId"
            placeholder="請選擇大分類"
            clearable
            style="width: 200px"
            @change="handleBCategoryChange"
          >
            <el-option
              v-for="category in bCategoryList"
              :key="category.CategoryId"
              :label="category.CategoryName"
              :value="category.CategoryId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="中分類">
          <el-select
            v-model="queryForm.MidClassId"
            placeholder="請選擇中分類"
            clearable
            style="width: 200px"
            :disabled="!queryForm.BigClassId"
            @change="handleMCategoryChange"
          >
            <el-option
              v-for="category in mCategoryList"
              :key="category.CategoryId"
              :label="category.CategoryName"
              :value="category.CategoryId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="小分類">
          <el-select
            v-model="queryForm.SmallClassId"
            placeholder="請選擇小分類"
            clearable
            style="width: 200px"
            :disabled="!queryForm.MidClassId"
          >
            <el-option
              v-for="category in sCategoryList"
              :key="category.CategoryId"
              :label="category.CategoryName"
              :value="category.CategoryId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="商品代碼">
          <el-input
            v-model="queryForm.ProductId"
            placeholder="請輸入商品代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="廠商">
          <el-select
            v-model="queryForm.VendorId"
            placeholder="請選擇廠商"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="vendor in vendorList"
              :key="vendor.VendorId"
              :label="vendor.VendorName"
              :value="vendor.VendorId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="銷售人員">
          <el-select
            v-model="queryForm.SalesPersonId"
            placeholder="請選擇銷售人員"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="person in salesPersonList"
              :key="person.UserId"
              :label="person.UserName"
              :value="person.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="報表類型">
          <el-select
            v-model="queryForm.ReportType"
            placeholder="請選擇報表類型"
            style="width: 200px"
          >
            <el-option label="日報" value="daily" />
            <el-option label="月報" value="monthly" />
            <el-option label="年報" value="yearly" />
            <el-option label="自訂期間" value="custom" />
          </el-select>
        </el-form-item>
        <el-form-item label="群組方式">
          <el-select
            v-model="queryForm.GroupBy"
            placeholder="請選擇群組方式"
            style="width: 200px"
          >
            <el-option label="商品" value="product" />
            <el-option label="分類" value="category" />
            <el-option label="店別" value="site" />
            <el-option label="廠商" value="vendor" />
            <el-option label="銷售人員" value="salesperson" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
          <el-button type="warning" @click="handlePrint">列印</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 查詢結果 -->
    <el-card class="result-card" shadow="never" style="margin-top: 20px">
      <!-- 彙總資訊 -->
      <el-card v-if="summary" style="margin-bottom: 20px" shadow="never">
        <el-row :gutter="20">
          <el-col :span="6">
            <el-statistic title="總數量" :value="summary.TotalQuantity" :precision="2" />
          </el-col>
          <el-col :span="6">
            <el-statistic title="總金額" :value="summary.TotalAmount" :precision="2" prefix="NT$" />
          </el-col>
          <el-col :span="6">
            <el-statistic title="總成本" :value="summary.TotalCost" :precision="2" prefix="NT$" />
          </el-col>
          <el-col :span="6">
            <el-statistic title="總毛利" :value="summary.TotalProfit" :precision="2" prefix="NT$" />
          </el-col>
        </el-row>
        <el-row :gutter="20" style="margin-top: 20px">
          <el-col :span="6">
            <el-statistic title="平均毛利率" :value="summary.AvgProfitRate" :precision="2" suffix="%" />
          </el-col>
          <el-col :span="6">
            <el-statistic title="總筆數" :value="summary.TotalOrderCount" />
          </el-col>
        </el-row>
      </el-card>
      
      <el-table
        :data="tableData"
        border
        stripe
        v-loading="loading"
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="ProductId" label="商品代碼" width="120" />
        <el-table-column prop="ProductName" label="商品名稱" width="200" />
        <el-table-column prop="BigClassName" label="大分類" width="120" />
        <el-table-column prop="MidClassName" label="中分類" width="120" />
        <el-table-column prop="SmallClassName" label="小分類" width="120" />
        <el-table-column prop="VendorName" label="廠商" width="150" />
        <el-table-column prop="SiteName" label="店別" width="120" />
        <el-table-column prop="SalesPersonName" label="銷售人員" width="120" />
        <el-table-column prop="TotalQuantity" label="總數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalQuantity) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalCost" label="總成本" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalCost) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalProfit" label="總毛利" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalProfit) }}
          </template>
        </el-table-column>
        <el-table-column prop="ProfitRate" label="毛利率" width="100" align="right">
          <template #default="{ row }">
            <el-tag :type="row.ProfitRate >= 20 ? 'success' : row.ProfitRate >= 10 ? 'warning' : 'danger'">
              {{ formatNumber(row.ProfitRate) }}%
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="OrderCount" label="筆數" width="80" align="right" />
        <el-table-column prop="AvgUnitPrice" label="平均單價" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.AvgUnitPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="AvgQuantity" label="平均數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.AvgQuantity) }}
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
import { analysisReportApi } from '@/api/analysisReport'
import { dropdownListApi } from '@/api/dropdownList'

export default {
  name: 'SalesAnalysisReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const summary = ref(null)
    const siteList = ref([])
    const bCategoryList = ref([])
    const mCategoryList = ref([])
    const sCategoryList = ref([])
    const vendorList = ref([])
    const salesPersonList = ref([])

    const queryForm = reactive({
      SiteId: '',
      DateFrom: '',
      DateTo: '',
      BigClassId: '',
      MidClassId: '',
      SmallClassId: '',
      ProductId: '',
      VendorId: '',
      SalesPersonId: '',
      CustomerId: '',
      ReportType: 'custom',
      GroupBy: 'product',
      PageIndex: 1,
      PageSize: 20
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0,
      TotalPages: 0
    })

    const loadSiteList = async () => {
      try {
        const response = await dropdownListApi.getSiteOptions({})
        if (response.data && response.data.success && response.data.data) {
          siteList.value = response.data.data.map(item => ({
            SiteId: item.value || item.SiteId || item.siteId,
            SiteName: item.label || item.SiteName || item.siteName
          }))
        }
      } catch (error) {
        console.error('載入店別列表失敗:', error)
        ElMessage.warning('載入店別列表失敗')
      }
    }

    const loadCategoryList = async (type, parentId = null) => {
      try {
        const response = await analysisReportApi.getGoodsCategories(type, parentId)
        if (response.data && response.data.success && response.data.data) {
          return response.data.data.map(item => ({
            CategoryId: item.CategoryId || item.Id,
            CategoryName: item.CategoryName || item.Name
          }))
        }
        return []
      } catch (error) {
        console.error(`載入${type}分類列表失敗:`, error)
        return []
      }
    }

    const handleBCategoryChange = async () => {
      queryForm.MidClassId = ''
      queryForm.SmallClassId = ''
      mCategoryList.value = []
      sCategoryList.value = []
      if (queryForm.BigClassId) {
        mCategoryList.value = await loadCategoryList('M', queryForm.BigClassId)
      }
    }

    const handleMCategoryChange = async () => {
      queryForm.SmallClassId = ''
      sCategoryList.value = []
      if (queryForm.MidClassId) {
        sCategoryList.value = await loadCategoryList('S', queryForm.MidClassId)
      }
    }

    const handleSearch = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await analysisReportApi.getSalesAnalysisReport(params)
        if (response.data.Success) {
          tableData.value = response.data.Data.Items || []
          summary.value = response.data.Data.Summary || null
          pagination.PageIndex = response.data.Data.PageIndex || 1
          pagination.PageSize = response.data.Data.PageSize || 20
          pagination.TotalCount = response.data.Data.TotalCount || 0
          pagination.TotalPages = response.data.Data.TotalPages || 0
        } else {
          ElMessage.error(response.data.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢銷售分析報表失敗:', error)
        ElMessage.error('查詢銷售分析報表失敗')
      } finally {
        loading.value = false
      }
    }

    const handleReset = () => {
      Object.assign(queryForm, {
        SiteId: '',
        DateFrom: '',
        DateTo: '',
        BigClassId: '',
        MidClassId: '',
        SmallClassId: '',
        ProductId: '',
        VendorId: '',
        SalesPersonId: '',
        CustomerId: '',
        ReportType: 'custom',
        GroupBy: 'product',
        PageIndex: 1,
        PageSize: 20
      })
      mCategoryList.value = []
      sCategoryList.value = []
      tableData.value = []
      pagination.TotalCount = 0
    }

    const handleExport = async () => {
      try {
        const response = await analysisReportApi.exportSalesAnalysisReport(queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `銷售分析報表_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出銷售分析報表失敗:', error)
        ElMessage.error('匯出銷售分析報表失敗')
      }
    }

    const handlePrint = async () => {
      try {
        const response = await analysisReportApi.printSalesAnalysisReport(queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `銷售分析報表_${new Date().toISOString().slice(0, 10)}.pdf`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印銷售分析報表失敗:', error)
        ElMessage.error('列印銷售分析報表失敗')
      }
    }

    const handleSizeChange = (size) => {
      pagination.PageSize = size
      queryForm.PageSize = size
      handleSearch()
    }

    const handlePageChange = (page) => {
      pagination.PageIndex = page
      queryForm.PageIndex = page
      handleSearch()
    }

    const formatNumber = (value) => {
      if (value == null) return '0.00'
      return new Intl.NumberFormat('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      }).format(value)
    }

    const formatCurrency = (value) => {
      if (value == null) return '$0'
      return new Intl.NumberFormat('zh-TW', {
        style: 'currency',
        currency: 'TWD',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }).format(value)
    }

    onMounted(async () => {
      await loadSiteList()
      bCategoryList.value = await loadCategoryList('B')
      // TODO: 載入廠商列表和銷售人員列表
    })

    return {
      loading,
      tableData,
      summary,
      siteList,
      bCategoryList,
      mCategoryList,
      sCategoryList,
      vendorList,
      salesPersonList,
      queryForm,
      pagination,
      handleBCategoryChange,
      handleMCategoryChange,
      handleSearch,
      handleReset,
      handleExport,
      handlePrint,
      handleSizeChange,
      handlePageChange,
      formatNumber,
      formatCurrency
    }
  }
}
</script>

<style scoped>
.sales-analysis-report {
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

.result-card {
  margin-top: 20px;
}
</style>

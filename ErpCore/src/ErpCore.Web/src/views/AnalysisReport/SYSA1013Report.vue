<template>
  <div class="sysa1013-report">
    <div class="page-header">
      <h1>耗材出庫明細表 (SYSA1013)</h1>
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
        <el-form-item label="大分類">
          <el-select
            v-model="queryForm.BId"
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
            v-model="queryForm.MId"
            placeholder="請選擇中分類"
            clearable
            style="width: 200px"
            :disabled="!queryForm.BId"
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
            v-model="queryForm.SId"
            placeholder="請選擇小分類"
            clearable
            style="width: 200px"
            :disabled="!queryForm.MId"
          >
            <el-option
              v-for="category in sCategoryList"
              :key="category.CategoryId"
              :label="category.CategoryName"
              :value="category.CategoryId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="單位">
          <el-input
            v-model="queryForm.OrgId"
            placeholder="請輸入單位"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="商品代碼">
          <el-input
            v-model="queryForm.GoodsId"
            placeholder="請輸入商品代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="日期起">
          <el-date-picker
            v-model="beginDate"
            type="date"
            placeholder="請選擇日期起"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="日期訖">
          <el-date-picker
            v-model="endDate"
            type="date"
            placeholder="請選擇日期訖"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="廠商">
          <el-input
            v-model="queryForm.SupplierId"
            placeholder="請輸入廠商"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="用途">
          <el-input
            v-model="queryForm.Use"
            placeholder="請輸入用途"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="篩選類型">
          <el-radio-group v-model="queryForm.FilterType">
            <el-radio label="all">全部</el-radio>
            <el-radio label="specific">特定狀態</el-radio>
          </el-radio-group>
        </el-form-item>
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
        <el-table-column prop="TxnNo" label="出庫單號" width="120" fixed="left" />
        <el-table-column prop="TxnDate" label="出庫日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TxnDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="BId" label="大分類" width="100" />
        <el-table-column prop="MId" label="中分類" width="100" />
        <el-table-column prop="SId" label="小分類" width="100" />
        <el-table-column prop="GoodsId" label="商品代碼" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" />
        <el-table-column prop="PackUnit" label="包裝單位" width="100" />
        <el-table-column prop="Unit" label="單位" width="80" />
        <el-table-column prop="Amt" label="單價" width="100" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Amt) }}
          </template>
        </el-table-column>
        <el-table-column prop="ApplyQty" label="申請數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.ApplyQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="Qty" label="數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.Qty) }}
          </template>
        </el-table-column>
        <el-table-column prop="NAmt" label="未稅金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.NAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="Use" label="用途" width="120" />
        <el-table-column prop="Vendor" label="廠商" width="200" />
        <el-table-column prop="StocksType" label="庫存類型" width="100" />
        <el-table-column prop="OrgId" label="單位" width="150" />
        <el-table-column prop="OrgAllocation" label="單位分攤" width="150" />
      </el-table>
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px"
      />
    </el-card>
  </div>
</template>

<script>
import { analysisReportApi } from '@/api/analysisReport'
import { dropdownListApi } from '@/api/dropdownList'
import { ElMessage } from 'element-plus'

export default {
  name: 'SYSA1013Report',
  data() {
    return {
      loading: false,
      queryForm: {
        SiteId: '',
        BId: '',
        MId: '',
        SId: '',
        OrgId: '',
        GoodsId: '',
        BeginDate: '',
        EndDate: '',
        SupplierId: '',
        Use: '',
        FilterType: 'all',
        PageIndex: 1,
        PageSize: 20
      },
      beginDate: '',
      endDate: '',
      tableData: [],
      pagination: {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      },
      siteList: [],
      bCategoryList: [],
      mCategoryList: [],
      sCategoryList: []
    }
  },
  watch: {
    beginDate(newVal) {
      this.queryForm.BeginDate = newVal || ''
    },
    endDate(newVal) {
      this.queryForm.EndDate = newVal || ''
    }
  },
  mounted() {
    this.loadSiteList()
    this.loadBCategoryList()
    // 預設為上個月
    const lastMonth = new Date()
    lastMonth.setMonth(lastMonth.getMonth() - 1)
    this.beginDate = this.formatDateForInput(lastMonth)
    this.endDate = this.formatDateForInput(new Date())
    this.queryForm.BeginDate = this.beginDate
    this.queryForm.EndDate = this.endDate
  },
  methods: {
    async loadSiteList() {
      try {
        const response = await dropdownListApi.getShopOptions({})
        if (response.data && response.data.success && response.data.data) {
          this.siteList = response.data.data.map(item => ({
            SiteId: item.value || item.ShopId || item.siteId,
            SiteName: item.label || item.ShopName || item.siteName
          }))
        }
      } catch (error) {
        console.error('載入店別列表失敗:', error)
        ElMessage.warning('載入店別列表失敗')
      }
    },
    async loadBCategoryList() {
      try {
        const response = await analysisReportApi.getGoodsCategories('B')
        if (response.data && response.data.success && response.data.data) {
          this.bCategoryList = response.data.data.map(item => ({
            CategoryId: item.categoryId || item.CategoryId,
            CategoryName: item.categoryName || item.CategoryName
          }))
        }
      } catch (error) {
        console.error('載入大分類列表失敗:', error)
        ElMessage.warning('載入大分類列表失敗')
      }
    },
    async handleBCategoryChange() {
      this.queryForm.MId = ''
      this.queryForm.SId = ''
      this.mCategoryList = []
      this.sCategoryList = []
      if (this.queryForm.BId) {
        try {
          const response = await analysisReportApi.getGoodsCategories('M', this.queryForm.BId)
          if (response.data && response.data.success && response.data.data) {
            this.mCategoryList = response.data.data.map(item => ({
              CategoryId: item.categoryId || item.CategoryId,
              CategoryName: item.categoryName || item.CategoryName
            }))
          }
        } catch (error) {
          console.error('載入中分類列表失敗:', error)
          ElMessage.warning('載入中分類列表失敗')
        }
      }
    },
    async handleMCategoryChange() {
      this.queryForm.SId = ''
      this.sCategoryList = []
      if (this.queryForm.MId) {
        try {
          const response = await analysisReportApi.getGoodsCategories('S', this.queryForm.MId)
          if (response.data && response.data.success && response.data.data) {
            this.sCategoryList = response.data.data.map(item => ({
              CategoryId: item.categoryId || item.CategoryId,
              CategoryName: item.categoryName || item.CategoryName
            }))
          }
        } catch (error) {
          console.error('載入小分類列表失敗:', error)
          ElMessage.warning('載入小分類列表失敗')
        }
      }
    },
    async handleSearch() {
      if (!this.queryForm.BeginDate || !this.queryForm.EndDate) {
        ElMessage.warning('請選擇日期範圍')
        return
      }
      this.loading = true
      try {
        const response = await analysisReportApi.getSYSA1013Report(this.queryForm)
        if (response.data.Success) {
          this.tableData = response.data.Data.Items || []
          this.pagination = {
            PageIndex: response.data.Data.PageIndex || 1,
            PageSize: response.data.Data.PageSize || 20,
            TotalCount: response.data.Data.TotalCount || 0,
            TotalPages: response.data.Data.TotalPages || 0
          }
        } else {
          ElMessage.error(response.data.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢耗材出庫明細表失敗:', error)
        ElMessage.error('查詢耗材出庫明細表失敗')
      } finally {
        this.loading = false
      }
    },
    handleReset() {
      this.queryForm = {
        SiteId: '',
        BId: '',
        MId: '',
        SId: '',
        OrgId: '',
        GoodsId: '',
        BeginDate: '',
        EndDate: '',
        SupplierId: '',
        Use: '',
        FilterType: 'all',
        PageIndex: 1,
        PageSize: 20
      }
      const lastMonth = new Date()
      lastMonth.setMonth(lastMonth.getMonth() - 1)
      this.beginDate = this.formatDateForInput(lastMonth)
      this.endDate = this.formatDateForInput(new Date())
      this.queryForm.BeginDate = this.beginDate
      this.queryForm.EndDate = this.endDate
      this.tableData = []
      this.pagination = {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      }
      this.mCategoryList = []
      this.sCategoryList = []
    },
    async handleExport() {
      if (!this.queryForm.BeginDate || !this.queryForm.EndDate) {
        ElMessage.warning('請選擇日期範圍')
        return
      }
      try {
        const response = await analysisReportApi.exportSYSA1013Report(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `耗材出庫明細表_${this.queryForm.BeginDate}_${this.queryForm.EndDate}_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出耗材出庫明細表失敗:', error)
        ElMessage.error('匯出耗材出庫明細表失敗')
      }
    },
    async handlePrint() {
      if (!this.queryForm.BeginDate || !this.queryForm.EndDate) {
        ElMessage.warning('請選擇日期範圍')
        return
      }
      try {
        const response = await analysisReportApi.printSYSA1013Report(this.queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印耗材出庫明細表失敗:', error)
        ElMessage.error('列印耗材出庫明細表失敗')
      }
    },
    handleSizeChange(val) {
      this.queryForm.PageSize = val
      this.queryForm.PageIndex = 1
      this.handleSearch()
    },
    handlePageChange(val) {
      this.queryForm.PageIndex = val
      this.handleSearch()
    },
    formatNumber(value) {
      if (value == null) return '0'
      return Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    },
    formatCurrency(value) {
      if (value == null) return '$0.00'
      return '$' + Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    },
    formatDate(value) {
      if (!value) return ''
      const date = new Date(value)
      return date.toISOString().slice(0, 10)
    },
    formatDateForInput(date) {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }
  }
}
</script>

<style scoped>
.sysa1013-report {
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
  margin-top: 20px;
}
</style>

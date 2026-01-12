<template>
  <div class="sysa1017-report">
    <div class="page-header">
      <h1>商品分析報表 (SYSA1017)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="組織">
          <el-select
            v-model="queryForm.OrgId"
            placeholder="請選擇組織"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="org in orgList"
              :key="org.OrgId"
              :label="org.OrgName"
              :value="org.OrgId"
            />
          </el-select>
        </el-form-item>
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
        <el-form-item label="年月">
          <el-date-picker
            v-model="yearMonth"
            type="month"
            placeholder="請選擇年月"
            format="YYYYMM"
            value-format="YYYYMM"
            style="width: 200px"
          />
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
        <el-form-item label="商品代碼">
          <el-input
            v-model="queryForm.GoodsId"
            placeholder="請輸入商品代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="篩選類型">
          <el-radio-group v-model="queryForm.FilterType">
            <el-radio label="">全部</el-radio>
            <el-radio label="低於安全庫存量">低於安全庫存量</el-radio>
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
        <el-table-column prop="SeqNo" label="序號" width="80" fixed="left" />
        <el-table-column prop="SiteName" label="店別" width="120" />
        <el-table-column prop="BId" label="大分類" width="100" />
        <el-table-column prop="MId" label="中分類" width="100" />
        <el-table-column prop="SId" label="小分類" width="100" />
        <el-table-column prop="GoodsId" label="商品代碼" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" />
        <el-table-column prop="PackUnit" label="包裝單位" width="100" />
        <el-table-column prop="Unit" label="單位" width="80" />
        <el-table-column prop="Qty" label="數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.Qty) }}
          </template>
        </el-table-column>
        <el-table-column prop="SafeQty" label="安全庫存量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.SafeQty) }}
          </template>
        </el-table-column>
        <el-table-column label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Qty < row.SafeQty ? 'danger' : 'success'">
              {{ row.Qty < row.SafeQty ? '低於安全庫存' : '正常' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="YearMonth" label="年月" width="100" />
        <el-table-column prop="OrgId" label="組織" width="120" />
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
  name: 'SYSA1017Report',
  data() {
    return {
      loading: false,
      queryForm: {
        OrgId: '',
        SiteId: '',
        BId: '',
        MId: '',
        SId: '',
        GoodsId: '',
        YearMonth: '',
        FilterType: '',
        PageIndex: 1,
        PageSize: 20
      },
      yearMonth: '',
      tableData: [],
      pagination: {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      },
      orgList: [],
      siteList: [],
      bCategoryList: [],
      mCategoryList: [],
      sCategoryList: []
    }
  },
  watch: {
    yearMonth(newVal) {
      this.queryForm.YearMonth = newVal || ''
    }
  },
  mounted() {
    this.loadOrgList()
    this.loadSiteList()
    this.loadBCategoryList()
    // 預設為本月
    const now = new Date()
    this.yearMonth = this.formatMonthForInput(now)
    this.queryForm.YearMonth = this.yearMonth
  },
  methods: {
    async loadOrgList() {
      try {
        const response = await dropdownListApi.getOrgOptions({})
        if (response.data && response.data.success && response.data.data) {
          this.orgList = response.data.data.map(item => ({
            OrgId: item.value || item.OrgId || item.orgId,
            OrgName: item.label || item.OrgName || item.orgName
          }))
        }
      } catch (error) {
        console.error('載入組織列表失敗:', error)
        ElMessage.warning('載入組織列表失敗')
      }
    },
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
      this.loading = true
      try {
        const response = await analysisReportApi.getSYSA1017Report(this.queryForm)
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
        console.error('查詢商品分析報表失敗:', error)
        ElMessage.error('查詢商品分析報表失敗')
      } finally {
        this.loading = false
      }
    },
    handleReset() {
      this.queryForm = {
        OrgId: '',
        SiteId: '',
        BId: '',
        MId: '',
        SId: '',
        GoodsId: '',
        YearMonth: '',
        FilterType: '',
        PageIndex: 1,
        PageSize: 20
      }
      const now = new Date()
      this.yearMonth = this.formatMonthForInput(now)
      this.queryForm.YearMonth = this.yearMonth
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
      try {
        const response = await analysisReportApi.exportSYSA1017Report(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `SYSA1017_商品分析報表_${this.queryForm.YearMonth || '全部'}_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出商品分析報表失敗:', error)
        ElMessage.error('匯出商品分析報表失敗')
      }
    },
    async handlePrint() {
      try {
        const response = await analysisReportApi.printSYSA1017Report(this.queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印商品分析報表失敗:', error)
        ElMessage.error('列印商品分析報表失敗')
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
    formatMonthForInput(date) {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}${String(d.getMonth() + 1).padStart(2, '0')}`
    }
  }
}
</script>

<style scoped>
.sysa1017-report {
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

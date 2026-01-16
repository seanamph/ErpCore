<template>
  <div class="sysa1021-report">
    <div class="page-header">
      <h1>月成本報表 (SYSA1021)</h1>
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
        <el-form-item label="年月" required>
          <el-date-picker
            v-model="queryForm.YearMonth"
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
          >
            <el-option
              v-for="category in sCategoryList"
              :key="category.CategoryId"
              :label="category.CategoryName"
              :value="category.CategoryId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="品項編號">
          <el-input
            v-model="queryForm.GoodsId"
            placeholder="請輸入品項編號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="篩選類型">
          <el-radio-group v-model="queryForm.FilterType">
            <el-radio label="全部">全部</el-radio>
            <el-radio label="有成本">有成本</el-radio>
            <el-radio label="無成本">無成本</el-radio>
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
        <el-table-column prop="SiteName" label="店別" width="150" />
        <el-table-column prop="YearMonth" label="年月" width="100" />
        <el-table-column prop="BId" label="大分類" width="120" />
        <el-table-column prop="MId" label="中分類" width="120" />
        <el-table-column prop="SId" label="小分類" width="120" />
        <el-table-column prop="GoodsId" label="品項編號" width="120" />
        <el-table-column prop="GoodsName" label="品項名稱" width="200" />
        <el-table-column prop="Qty" label="數量" width="100" align="right">
          <template #default="scope">
            {{ formatNumber(scope.row.Qty) }}
          </template>
        </el-table-column>
        <el-table-column prop="CostAmount" label="成本金額" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.CostAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="AvgCost" label="平均成本" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.AvgCost) }}
          </template>
        </el-table-column>
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
  name: 'SYSA1021Report',
  data() {
    return {
      loading: false,
      queryForm: {
        SiteId: '',
        BId: '',
        MId: '',
        SId: '',
        GoodsId: '',
        YearMonth: '',
        FilterType: '全部',
        PageIndex: 1,
        PageSize: 20
      },
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
  mounted() {
    this.loadSiteList()
    this.loadCategoryList('B')
    this.loadCategoryList('M')
    this.loadCategoryList('S')
  },
  methods: {
    async loadSiteList() {
      try {
        const response = await dropdownListApi.getSiteOptions({})
        if (response.data && response.data.success && response.data.data) {
          this.siteList = response.data.data.map(item => ({
            SiteId: item.value || item.SiteId || item.siteId,
            SiteName: item.label || item.SiteName || item.siteName
          }))
        }
      } catch (error) {
        console.error('載入店別列表失敗:', error)
        ElMessage.warning('載入店別列表失敗')
      }
    },
    async loadCategoryList(categoryType) {
      try {
        const response = await analysisReportApi.getGoodsCategories(categoryType)
        if (response.data && response.data.Success && response.data.Data) {
          const categories = response.data.Data.map(item => ({
            CategoryId: item.CategoryId || item.categoryId,
            CategoryName: item.CategoryName || item.categoryName
          }))
          if (categoryType === 'B') {
            this.bCategoryList = categories
          } else if (categoryType === 'M') {
            this.mCategoryList = categories
          } else if (categoryType === 'S') {
            this.sCategoryList = categories
          }
        }
      } catch (error) {
        console.error(`載入${categoryType}分類列表失敗:`, error)
      }
    },
    async handleSearch() {
      // 驗證必填欄位
      if (!this.queryForm.YearMonth) {
        ElMessage.warning('請選擇年月')
        return
      }

      this.loading = true
      try {
        const response = await analysisReportApi.getSYSA1021Report(this.queryForm)
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
        console.error('查詢月成本報表失敗:', error)
        ElMessage.error('查詢月成本報表失敗')
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
        GoodsId: '',
        YearMonth: '',
        FilterType: '全部',
        PageIndex: 1,
        PageSize: 20
      }
      this.tableData = []
      this.pagination = {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      }
    },
    async handleExport() {
      if (!this.queryForm.YearMonth) {
        ElMessage.warning('請選擇年月')
        return
      }

      try {
        const response = await analysisReportApi.exportSYSA1021Report(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `SYSA1021_月成本報表_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出月成本報表失敗:', error)
        ElMessage.error('匯出月成本報表失敗')
      }
    },
    async handlePrint() {
      if (!this.queryForm.YearMonth) {
        ElMessage.warning('請選擇年月')
        return
      }

      try {
        const response = await analysisReportApi.printSYSA1021Report(this.queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印月成本報表失敗:', error)
        ElMessage.error('列印月成本報表失敗')
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
      if (value === null || value === undefined) return '0'
      return Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    },
    formatCurrency(value) {
      if (value === null || value === undefined) return '$0.00'
      return '$' + Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    }
  }
}
</script>

<style scoped>
.sysa1021-report {
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

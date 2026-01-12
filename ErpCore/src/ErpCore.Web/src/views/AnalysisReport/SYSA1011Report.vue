<template>
  <div class="sysa1011-report">
    <div class="page-header">
      <h1>商品分析報表 (SYSA1011)</h1>
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
            <el-radio label="safety">低於安全庫存量</el-radio>
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
        <el-table-column prop="SeqNo" label="序號" width="80" />
        <el-table-column prop="SiteName" label="店別" width="120" />
        <el-table-column prop="GoodsId" label="商品代碼" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" />
        <el-table-column prop="Unit" label="單位" width="80" />
        <el-table-column prop="Qty" label="數量" width="100" align="right">
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
  name: 'SYSA1011Report',
  data() {
    return {
      loading: false,
      queryForm: {
        SiteId: '',
        BId: '',
        MId: '',
        SId: '',
        GoodsId: '',
        FilterType: '',
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
    this.loadBCategoryList()
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
      this.loading = true
      try {
        const response = await analysisReportApi.getSYSA1011Report(this.queryForm)
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
        SiteId: '',
        BId: '',
        MId: '',
        SId: '',
        GoodsId: '',
        FilterType: '',
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
      try {
        const response = await analysisReportApi.exportSYSA1011Report(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `商品分析報表_${new Date().toISOString().slice(0, 10)}.xlsx`
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
        const response = await analysisReportApi.printSYSA1011Report(this.queryForm)
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
    }
  }
}
</script>

<style scoped>
.sysa1011-report {
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

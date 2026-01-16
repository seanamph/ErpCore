<template>
  <div class="sysa1020-report">
    <div class="page-header">
      <h1>商品分析報表 (SYSA1020)</h1>
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
        <el-form-item label="計劃ID">
          <el-select
            v-model="queryForm.PlanId"
            placeholder="請選擇計劃ID"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="plan in planList"
              :key="plan.PlanId"
              :label="plan.PlanName"
              :value="plan.PlanId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="顯示類型">
          <el-select
            v-model="queryForm.ShowType"
            placeholder="請選擇顯示類型"
            clearable
            style="width: 200px"
          >
            <el-option label="類型1" value="TYPE1" />
            <el-option label="類型2" value="TYPE2" />
          </el-select>
        </el-form-item>
        <el-form-item label="篩選類型">
          <el-select
            v-model="queryForm.FilterType"
            placeholder="請選擇篩選類型"
            clearable
            style="width: 200px"
          >
            <el-option label="全部" value="" />
            <el-option label="特定條件" value="特定條件" />
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
        <el-table-column prop="SiteName" label="店別" width="150" />
        <el-table-column prop="PlanName" label="計劃名稱" width="150" />
        <el-table-column prop="GoodsId" label="品項編號" width="120" />
        <el-table-column prop="GoodsName" label="品項名稱" width="200" />
        <el-table-column prop="ShowType" label="顯示類型" width="120" />
        <el-table-column prop="FilterType" label="篩選類型" width="120" />
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
  name: 'SYSA1020Report',
  data() {
    return {
      loading: false,
      queryForm: {
        SiteId: '',
        PlanId: '',
        ShowType: '',
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
      planList: []
    }
  },
  mounted() {
    this.loadSiteList()
    this.loadPlanList()
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
    async loadPlanList() {
      try {
        // TODO: 實作計劃列表 API，目前先使用空陣列
        // const response = await dropdownListApi.getPlanOptions({})
        // if (response.data && response.data.success && response.data.data) {
        //   this.planList = response.data.data.map(item => ({
        //     PlanId: item.value || item.PlanId || item.planId,
        //     PlanName: item.label || item.PlanName || item.planName
        //   }))
        // }
        this.planList = []
      } catch (error) {
        console.error('載入計劃列表失敗:', error)
        ElMessage.warning('載入計劃列表失敗')
      }
    },
    async handleSearch() {
      this.loading = true
      try {
        const response = await analysisReportApi.getSYSA1020Report(this.queryForm)
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
        PlanId: '',
        ShowType: '',
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
        const response = await analysisReportApi.exportSYSA1020Report(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `SYSA1020_商品分析報表_${new Date().toISOString().slice(0, 10)}.xlsx`
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
        const response = await analysisReportApi.printSYSA1020Report(this.queryForm)
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
    }
  }
}
</script>

<style scoped>
.sysa1020-report {
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

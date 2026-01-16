<template>
  <div class="syswc10-report">
    <div class="page-header">
      <h1>庫存分析報表 (SYSWC10)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="商品代碼起">
          <el-input
            v-model="queryForm.GoodsIdFrom"
            placeholder="請輸入商品代碼起"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="商品代碼迄">
          <el-input
            v-model="queryForm.GoodsIdTo"
            placeholder="請輸入商品代碼迄"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="商品名稱">
          <el-input
            v-model="queryForm.GoodsName"
            placeholder="請輸入商品名稱"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="店別">
          <el-select
            v-model="queryForm.SiteIds"
            placeholder="請選擇店別"
            clearable
            multiple
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
        <el-form-item label="庫別">
          <el-select
            v-model="queryForm.WarehouseIds"
            placeholder="請選擇庫別"
            clearable
            multiple
            style="width: 200px"
          >
            <el-option
              v-for="warehouse in warehouseList"
              :key="warehouse.WarehouseId"
              :label="warehouse.WarehouseName"
              :value="warehouse.WarehouseId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="大分類">
          <el-input
            v-model="queryForm.BId"
            placeholder="請輸入大分類"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="中分類">
          <el-input
            v-model="queryForm.MId"
            placeholder="請輸入中分類"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="小分類">
          <el-input
            v-model="queryForm.SId"
            placeholder="請輸入小分類"
            clearable
            style="width: 200px"
          />
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
        <el-form-item label="狀態">
          <el-select
            v-model="queryForm.Status"
            placeholder="請選擇狀態"
            clearable
            style="width: 200px"
          >
            <el-option label="全部" value="" />
            <el-option label="低庫存" value="LowStock" />
            <el-option label="過量庫存" value="OverStock" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :loading="loading">查詢</el-button>
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
        <el-table-column prop="SiteId" label="店別代碼" width="120" />
        <el-table-column prop="SiteName" label="店別名稱" width="150" />
        <el-table-column prop="GoodsId" label="商品代碼" width="150" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" />
        <el-table-column prop="BigCategoryName" label="大分類" width="120" />
        <el-table-column prop="MidCategoryName" label="中分類" width="120" />
        <el-table-column prop="SmallCategoryName" label="小分類" width="120" />
        <el-table-column prop="WarehouseName" label="庫別" width="120" />
        <el-table-column prop="InQty" label="入庫數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.InQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="OutQty" label="出庫數量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.OutQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="CurrentQty" label="當前庫存數量" width="140" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.CurrentQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="CurrentAmt" label="當前庫存金額" width="140" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.CurrentAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="SafeQty" label="安全庫存量" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.SafeQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="IsLowStock" label="低庫存" width="100" align="center">
          <template #default="{ row }">
            <el-tag v-if="row.IsLowStock" type="danger">是</el-tag>
            <el-tag v-else type="success">否</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="IsOverStock" label="過量庫存" width="120" align="center">
          <template #default="{ row }">
            <el-tag v-if="row.IsOverStock" type="warning">是</el-tag>
            <el-tag v-else type="success">否</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="LastStockDate" label="最後庫存異動日期" width="180">
          <template #default="{ row }">
            {{ formatDate(row.LastStockDate) }}
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
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { analysisReportApi } from '@/api/analysisReport'
import { dropdownListApi } from '@/api/dropdownList'

export default {
  name: 'SYSWC10Report',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const siteList = ref([])
    const warehouseList = ref([])

    const queryForm = reactive({
      GoodsIdFrom: '',
      GoodsIdTo: '',
      GoodsName: '',
      SiteIds: [],
      WarehouseIds: [],
      CategoryIds: [],
      DateFrom: '',
      DateTo: '',
      MinQty: null,
      MaxQty: null,
      Status: '',
      BId: '',
      MId: '',
      SId: '',
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

    const loadWarehouseList = async () => {
      try {
        // TODO: 實現載入庫別列表的 API
        warehouseList.value = []
      } catch (error) {
        console.error('載入庫別列表失敗:', error)
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
        const response = await analysisReportApi.getSYSWC10Report(params)
        if (response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.PageIndex = response.data.Data.PageIndex || 1
          pagination.PageSize = response.data.Data.PageSize || 20
          pagination.TotalCount = response.data.Data.TotalCount || 0
          pagination.TotalPages = response.data.Data.TotalPages || 0
        } else {
          ElMessage.error(response.data.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢庫存分析報表失敗:', error)
        ElMessage.error('查詢庫存分析報表失敗')
      } finally {
        loading.value = false
      }
    }

    const handleReset = () => {
      Object.assign(queryForm, {
        GoodsIdFrom: '',
        GoodsIdTo: '',
        GoodsName: '',
        SiteIds: [],
        WarehouseIds: [],
        CategoryIds: [],
        DateFrom: '',
        DateTo: '',
        MinQty: null,
        MaxQty: null,
        Status: '',
        BId: '',
        MId: '',
        SId: '',
        PageIndex: 1,
        PageSize: 20
      })
      pagination.PageIndex = 1
      pagination.PageSize = 20
      pagination.TotalCount = 0
      pagination.TotalPages = 0
      tableData.value = []
    }

    const handleExport = async () => {
      try {
        const response = await analysisReportApi.exportSYSWC10Report(queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `SYSWC10_庫存分析報表_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出庫存分析報表失敗:', error)
        ElMessage.error('匯出庫存分析報表失敗')
      }
    }

    const handlePrint = async () => {
      try {
        const response = await analysisReportApi.printSYSWC10Report(queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印庫存分析報表失敗:', error)
        ElMessage.error('列印庫存分析報表失敗')
      }
    }

    const handleSizeChange = (val) => {
      pagination.PageSize = val
      pagination.PageIndex = 1
      queryForm.PageSize = val
      queryForm.PageIndex = 1
      handleSearch()
    }

    const handlePageChange = (val) => {
      pagination.PageIndex = val
      queryForm.PageIndex = val
      handleSearch()
    }

    const formatNumber = (value) => {
      if (value === null || value === undefined) return '0'
      return Number(value).toLocaleString('zh-TW')
    }

    const formatCurrency = (value) => {
      if (value === null || value === undefined) return '$0.00'
      return '$' + Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    }

    const formatDate = (value) => {
      if (!value) return ''
      const date = new Date(value)
      return date.toLocaleDateString('zh-TW')
    }

    // 初始化
    loadSiteList()
    loadWarehouseList()

    return {
      loading,
      tableData,
      siteList,
      warehouseList,
      queryForm,
      pagination,
      handleSearch,
      handleReset,
      handleExport,
      handlePrint,
      handleSizeChange,
      handlePageChange,
      formatNumber,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.syswc10-report {
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
  margin-top: 20px;
}
</style>

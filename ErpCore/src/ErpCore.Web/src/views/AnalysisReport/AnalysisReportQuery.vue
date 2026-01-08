<template>
  <div class="analysis-report-query">
    <div class="page-header">
      <h1>進銷存分析報表 (SYSA1000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="報表類型" required>
              <el-select v-model="queryForm.ReportId" placeholder="請選擇報表類型" clearable>
                <el-option label="耗材庫存查詢表 (SYSA1011)" value="SYSA1011" />
                <el-option label="耗材入庫明細表 (SYSA1012)" value="SYSA1012" />
                <el-option label="耗材出庫明細表 (SYSA1013)" value="SYSA1013" />
                <el-option label="耗材領用發料退回明細表 (SYSA1014)" value="SYSA1014" />
                <el-option label="固定資產數量彙總表 (SYSA1015)" value="SYSA1015" />
                <el-option label="庫房領料進價成本分攤表 (SYSA1016)" value="SYSA1016" />
                <el-option label="工務修繕扣款報表 (SYSA1017)" value="SYSA1017" />
                <el-option label="工務維修件數統計表 (SYSA1018)" value="SYSA1018" />
                <el-option label="工務維修類別統計表 (SYSA1019)" value="SYSA1019" />
                <el-option label="盤點差異明細表 (SYSA1020)" value="SYSA1020" />
                <el-option label="耗材進銷存月報表 (SYSA1021)" value="SYSA1021" />
                <el-option label="公務費用歸屬統計表 (SYSA1022)" value="SYSA1022" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="店別">
              <el-input v-model="queryForm.SiteId" placeholder="請輸入店別代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="年月">
              <el-date-picker
                v-model="queryForm.YearMonth"
                type="month"
                placeholder="請選擇年月"
                format="YYYY/MM"
                value-format="YYYY/MM"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="開始日期">
              <el-date-picker
                v-model="queryForm.BeginDate"
                type="date"
                placeholder="請選擇開始日期"
                format="YYYY/MM/DD"
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
                placeholder="請選擇結束日期"
                format="YYYY/MM/DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="商品代碼">
              <el-input v-model="queryForm.GoodsId" placeholder="請輸入商品代碼" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="大分類">
              <el-input v-model="queryForm.BId" placeholder="請輸入大分類代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="中分類">
              <el-input v-model="queryForm.MId" placeholder="請輸入中分類代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="小分類">
              <el-input v-model="queryForm.SId" placeholder="請輸入小分類代碼" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20" v-if="showAdvancedFields">
          <el-col :span="8">
            <el-form-item label="單位代碼">
              <el-input v-model="queryForm.OrgId" placeholder="請輸入單位代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="供應商">
              <el-input v-model="queryForm.Vendor" placeholder="請輸入供應商" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="用途">
              <el-input v-model="queryForm.Use" placeholder="請輸入用途" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20" v-if="showAdvancedFields">
          <el-col :span="8">
            <el-form-item label="歸屬狀態">
              <el-select v-model="queryForm.BelongStatus" placeholder="請選擇歸屬狀態" clearable>
                <el-option label="員工負擔" value="1" />
                <el-option label="店別負擔" value="2" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="申請開始日期">
              <el-date-picker
                v-model="queryForm.ApplyDateB"
                type="date"
                placeholder="請選擇申請開始日期"
                format="YYYY/MM/DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="申請結束日期">
              <el-date-picker
                v-model="queryForm.ApplyDateE"
                type="date"
                placeholder="請選擇申請結束日期"
                format="YYYY/MM/DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel">匯出Excel</el-button>
          <el-button type="warning" @click="handleExportPdf">匯出PDF</el-button>
          <el-button type="info" @click="showAdvancedFields = !showAdvancedFields">
            {{ showAdvancedFields ? '隱藏' : '顯示' }}進階條件
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never" v-if="reportData">
      <div class="report-header">
        <h2>{{ reportData.ReportName }}</h2>
        <p v-if="reportData.SiteName">店別：{{ reportData.SiteName }}</p>
      </div>
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
        :default-sort="{ prop: 'GoodsId', order: 'ascending' }"
      >
        <el-table-column prop="SiteId" label="店別代碼" width="120" v-if="hasColumn('SiteId')" />
        <el-table-column prop="BId" label="大分類" width="120" v-if="hasColumn('BId')" />
        <el-table-column prop="MId" label="中分類" width="120" v-if="hasColumn('MId')" />
        <el-table-column prop="SId" label="小分類" width="120" v-if="hasColumn('SId')" />
        <el-table-column prop="GoodsId" label="商品代碼" width="150" v-if="hasColumn('GoodsId')" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" v-if="hasColumn('GoodsName')" />
        <el-table-column prop="PackUnit" label="包裝單位" width="120" v-if="hasColumn('PackUnit')" />
        <el-table-column prop="Unit" label="單位" width="100" v-if="hasColumn('Unit')" />
        <el-table-column prop="Qty" label="數量" width="120" align="right" v-if="hasColumn('Qty')">
          <template #default="{ row }">
            {{ formatNumber(row.Qty) }}
          </template>
        </el-table-column>
        <el-table-column prop="SafeQty" label="安全庫存量" width="120" align="right" v-if="hasColumn('SafeQty')">
          <template #default="{ row }">
            {{ formatNumber(row.SafeQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="SelectType" label="選擇類型" width="120" v-if="hasColumn('SelectType')" />
        <!-- 動態欄位 -->
        <el-table-column
          v-for="(value, key) in getAdditionalColumns()"
          :key="key"
          :prop="key"
          :label="key"
          width="150"
        >
          <template #default="{ row }">
            {{ formatValue(row.AdditionalFields?.[key]) }}
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
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { analysisReportApi } from '@/api/analysisReport'

export default {
  name: 'AnalysisReportQuery',
  setup() {
    const route = useRoute()
    const loading = ref(false)
    const showAdvancedFields = ref(false)
    const reportData = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      ReportId: route.params.reportId || '',
      SiteId: '',
      YearMonth: '',
      BeginDate: null,
      EndDate: null,
      BId: '',
      MId: '',
      SId: '',
      GoodsId: '',
      FilterType: '',
      OrgId: '',
      Vendor: '',
      Use: '',
      BelongStatus: '',
      ApplyDateB: null,
      ApplyDateE: null,
      StartMonth: '',
      EndMonth: '',
      DateType: '',
      MaintainEmp: '',
      BelongOrg: '',
      ApplyType: ''
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 格式化數字
    const formatNumber = (value) => {
      if (value === null || value === undefined) return '0'
      return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 格式化值
    const formatValue = (value) => {
      if (value === null || value === undefined) return ''
      if (typeof value === 'number') return formatNumber(value)
      return String(value)
    }

    // 檢查是否有欄位
    const hasColumn = (columnName) => {
      if (!tableData.value || tableData.value.length === 0) return false
      return tableData.value.some(row => row[columnName] !== null && row[columnName] !== undefined)
    }

    // 取得額外欄位
    const getAdditionalColumns = () => {
      if (!tableData.value || tableData.value.length === 0) return {}
      const columns = {}
      tableData.value.forEach(row => {
        if (row.AdditionalFields) {
          Object.keys(row.AdditionalFields).forEach(key => {
            if (!columns[key]) {
              columns[key] = true
            }
          })
        }
      })
      return columns
    }

    // 查詢資料
    const loadData = async () => {
      if (!queryForm.ReportId) {
        ElMessage.warning('請選擇報表類型')
        return
      }

      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          SiteId: queryForm.SiteId || null,
          YearMonth: queryForm.YearMonth || null,
          BeginDate: queryForm.BeginDate || null,
          EndDate: queryForm.EndDate || null,
          BId: queryForm.BId || null,
          MId: queryForm.MId || null,
          SId: queryForm.SId || null,
          GoodsId: queryForm.GoodsId || null,
          FilterType: queryForm.FilterType || null,
          OrgId: queryForm.OrgId || null,
          Vendor: queryForm.Vendor || null,
          Use: queryForm.Use || null,
          BelongStatus: queryForm.BelongStatus || null,
          ApplyDateB: queryForm.ApplyDateB || null,
          ApplyDateE: queryForm.ApplyDateE || null,
          StartMonth: queryForm.StartMonth || null,
          EndMonth: queryForm.EndMonth || null,
          DateType: queryForm.DateType || null,
          MaintainEmp: queryForm.MaintainEmp || null,
          BelongOrg: queryForm.BelongOrg || null,
          ApplyType: queryForm.ApplyType || null
        }

        // 移除空值
        Object.keys(params).forEach(key => {
          if (params[key] === null || params[key] === '') {
            delete params[key]
          }
        })

        const response = await analysisReportApi.getAnalysisReport(queryForm.ReportId, params)
        if (response.Data) {
          reportData.value = response.Data
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 查詢
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm, {
        ReportId: route.params.reportId || '',
        SiteId: '',
        YearMonth: '',
        BeginDate: null,
        EndDate: null,
        BId: '',
        MId: '',
        SId: '',
        GoodsId: '',
        FilterType: '',
        OrgId: '',
        Vendor: '',
        Use: '',
        BelongStatus: '',
        ApplyDateB: null,
        ApplyDateE: null,
        StartMonth: '',
        EndMonth: '',
        DateType: '',
        MaintainEmp: '',
        BelongOrg: '',
        ApplyType: ''
      })
      reportData.value = null
      tableData.value = []
      pagination.TotalCount = 0
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.ReportId) {
        ElMessage.warning('請選擇報表類型')
        return
      }

      try {
        const data = {
          Format: 'Excel',
          QueryParams: {
            SiteId: queryForm.SiteId || null,
            YearMonth: queryForm.YearMonth || null,
            BeginDate: queryForm.BeginDate || null,
            EndDate: queryForm.EndDate || null,
            BId: queryForm.BId || null,
            MId: queryForm.MId || null,
            SId: queryForm.SId || null,
            GoodsId: queryForm.GoodsId || null,
            FilterType: queryForm.FilterType || null,
            OrgId: queryForm.OrgId || null,
            Vendor: queryForm.Vendor || null,
            Use: queryForm.Use || null,
            BelongStatus: queryForm.BelongStatus || null,
            ApplyDateB: queryForm.ApplyDateB || null,
            ApplyDateE: queryForm.ApplyDateE || null,
            StartMonth: queryForm.StartMonth || null,
            EndMonth: queryForm.EndMonth || null,
            DateType: queryForm.DateType || null,
            MaintainEmp: queryForm.MaintainEmp || null,
            BelongOrg: queryForm.BelongOrg || null,
            ApplyType: queryForm.ApplyType || null
          }
        }

        // 移除空值
        Object.keys(data.QueryParams).forEach(key => {
          if (data.QueryParams[key] === null || data.QueryParams[key] === '') {
            delete data.QueryParams[key]
          }
        })

        const response = await analysisReportApi.exportAnalysisReport(queryForm.ReportId, data)

        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        const reportName = reportData.value?.ReportName || queryForm.ReportId
        link.download = `${reportName}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)

        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 匯出PDF
    const handleExportPdf = async () => {
      if (!queryForm.ReportId) {
        ElMessage.warning('請選擇報表類型')
        return
      }

      try {
        const data = {
          Format: 'PDF',
          QueryParams: {
            SiteId: queryForm.SiteId || null,
            YearMonth: queryForm.YearMonth || null,
            BeginDate: queryForm.BeginDate || null,
            EndDate: queryForm.EndDate || null,
            BId: queryForm.BId || null,
            MId: queryForm.MId || null,
            SId: queryForm.SId || null,
            GoodsId: queryForm.GoodsId || null,
            FilterType: queryForm.FilterType || null,
            OrgId: queryForm.OrgId || null,
            Vendor: queryForm.Vendor || null,
            Use: queryForm.Use || null,
            BelongStatus: queryForm.BelongStatus || null,
            ApplyDateB: queryForm.ApplyDateB || null,
            ApplyDateE: queryForm.ApplyDateE || null,
            StartMonth: queryForm.StartMonth || null,
            EndMonth: queryForm.EndMonth || null,
            DateType: queryForm.DateType || null,
            MaintainEmp: queryForm.MaintainEmp || null,
            BelongOrg: queryForm.BelongOrg || null,
            ApplyType: queryForm.ApplyType || null
          }
        }

        // 移除空值
        Object.keys(data.QueryParams).forEach(key => {
          if (data.QueryParams[key] === null || data.QueryParams[key] === '') {
            delete data.QueryParams[key]
          }
        })

        const response = await analysisReportApi.exportAnalysisReport(queryForm.ReportId, data)

        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        const reportName = reportData.value?.ReportName || queryForm.ReportId
        link.download = `${reportName}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)

        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 從路由參數初始化報表類型
    onMounted(() => {
      if (route.params.reportId) {
        queryForm.ReportId = route.params.reportId
      }
    })

    return {
      loading,
      showAdvancedFields,
      reportData,
      tableData,
      queryForm,
      pagination,
      formatNumber,
      formatValue,
      hasColumn,
      getAdditionalColumns,
      handleSearch,
      handleReset,
      handleExportExcel,
      handleExportPdf,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
.analysis-report-query {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;

    .report-header {
      margin-bottom: 20px;
      padding-bottom: 10px;
      border-bottom: 1px solid #ebeef5;

      h2 {
        margin: 0 0 10px 0;
        font-size: 18px;
        font-weight: 600;
      }

      p {
        margin: 0;
        color: #606266;
        font-size: 14px;
      }
    }
  }
}
</style>


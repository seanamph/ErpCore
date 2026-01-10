<template>
  <div class="einvoice-report">
    <div class="page-header">
      <h1>電子發票報表 (ECA3040, ECA4010-ECA4060)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="報表類型">
              <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable>
                <el-option label="訂單明細 (ECA4010)" value="ECA4010" />
                <el-option label="商品銷售統計 (ECA4020)" value="ECA4020" />
                <el-option label="零售商銷售統計 (ECA4030)" value="ECA4030" />
                <el-option label="店別銷售統計 (ECA4040)" value="ECA4040" />
                <el-option label="出貨日期統計 (ECA4050)" value="ECA4050" />
                <el-option label="訂單日期統計 (ECA4060)" value="ECA4060" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="開始日期">
              <el-date-picker v-model="queryForm.StartDate" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="結束日期">
              <el-date-picker v-model="queryForm.EndDate" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="店別ID">
              <el-input v-model="queryForm.StoreId" placeholder="請輸入店別ID" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="零售商ID">
              <el-input v-model="queryForm.RetailerId" placeholder="請輸入零售商ID" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="商品ID">
              <el-input v-model="queryForm.GoodsId" placeholder="請輸入商品ID" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel">匯出Excel</el-button>
          <el-button type="warning" @click="handleExportPdf">匯出PDF</el-button>
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
        <el-table-column prop="InvoiceNo" label="發票號碼" width="150" />
        <el-table-column prop="InvoiceDate" label="發票日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.InvoiceDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="StoreId" label="店別ID" width="120" />
        <el-table-column prop="StoreName" label="店別名稱" width="200" />
        <el-table-column prop="RetailerId" label="零售商ID" width="120" />
        <el-table-column prop="RetailerName" label="零售商名稱" width="200" />
        <el-table-column prop="GoodsId" label="商品ID" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" />
        <el-table-column prop="Quantity" label="數量" width="100" align="right" />
        <el-table-column prop="UnitPrice" label="單價" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.UnitPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="金額" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
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
import { einvoiceReportApi } from '@/api/einvoice'

export default {
  name: 'EInvoiceReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      ReportType: '',
      StartDate: null,
      EndDate: null,
      StoreId: '',
      RetailerId: '',
      GoodsId: ''
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (!amount) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const data = {
          ReportType: queryForm.ReportType,
          StartDate: queryForm.StartDate,
          EndDate: queryForm.EndDate,
          StoreId: queryForm.StoreId || null,
          RetailerId: queryForm.RetailerId || null,
          GoodsId: queryForm.GoodsId || null,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await einvoiceReportApi.getReports(data)
        if (response.Data) {
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
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm, {
        ReportType: '',
        StartDate: null,
        EndDate: null,
        StoreId: '',
        RetailerId: '',
        GoodsId: ''
      })
      handleSearch()
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      try {
        const data = {
          ReportType: queryForm.ReportType,
          StartDate: queryForm.StartDate,
          EndDate: queryForm.EndDate,
          StoreId: queryForm.StoreId || null,
          RetailerId: queryForm.RetailerId || null,
          GoodsId: queryForm.GoodsId || null
        }
        const response = await einvoiceReportApi.exportExcel(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        const reportTypeName = getReportTypeName(queryForm.ReportType)
        link.download = `電子發票報表_${reportTypeName}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      try {
        const data = {
          ReportType: queryForm.ReportType,
          StartDate: queryForm.StartDate,
          EndDate: queryForm.EndDate,
          StoreId: queryForm.StoreId || null,
          RetailerId: queryForm.RetailerId || null,
          GoodsId: queryForm.GoodsId || null
        }
        const response = await einvoiceReportApi.exportPdf(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        const reportTypeName = getReportTypeName(queryForm.ReportType)
        link.download = `電子發票報表_${reportTypeName}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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

    // 取得報表類型名稱
    const getReportTypeName = (reportType) => {
      const names = {
        'ECA4010': '訂單明細',
        'ECA4020': '商品銷售統計',
        'ECA4030': '零售商銷售統計',
        'ECA4040': '店別銷售統計',
        'ECA4050': '出貨日期統計',
        'ECA4060': '訂單日期統計'
      }
      return names[reportType] || '電子發票報表'
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

    return {
      loading,
      tableData,
      queryForm,
      pagination,
      formatDate,
      formatCurrency,
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
@import '@/assets/styles/variables.scss';

.einvoice-report {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


<template>
  <div class="einvoice-report">
    <div class="page-header">
      <h1>電子發票報表 (ECA3040, ECA4010-ECA4060)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px" inline>
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable style="width: 200px">
            <el-option label="電子發票報表查詢 (ECA3040)" value="ECA3040" />
            <el-option label="訂單明細 (ECA4010)" value="ECA4010" />
            <el-option label="商品銷售統計 (ECA4020)" value="ECA4020" />
            <el-option label="零售商銷售統計 (ECA4030)" value="ECA4030" />
            <el-option label="店別銷售統計 (ECA4040)" value="ECA4040" />
            <el-option label="出貨日期統計 (ECA4050)" value="ECA4050" />
            <el-option label="訂單日期統計 (ECA4060)" value="ECA4060" />
          </el-select>
        </el-form-item>
        <el-form-item label="訂單編號">
          <el-input v-model="queryForm.OrderNo" placeholder="請輸入訂單編號" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="零售商訂單編號">
          <el-input v-model="queryForm.RetailerOrderNo" placeholder="請輸入零售商訂單編號" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="訂單日期">
          <el-date-picker
            v-model="orderDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item label="出貨日期">
          <el-date-picker
            v-model="shipDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item label="店別">
          <el-select v-model="queryForm.StoreId" placeholder="請選擇店別" filterable clearable style="width: 200px">
            <el-option
              v-for="store in storeList"
              :key="store.StoreId"
              :label="store.StoreName"
              :value="store.StoreId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="供應商">
          <el-select v-model="queryForm.ProviderId" placeholder="請選擇供應商" filterable clearable style="width: 200px">
            <el-option
              v-for="provider in providerList"
              :key="provider.ProviderId"
              :label="provider.ProviderName"
              :value="provider.ProviderId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="商品ID">
          <el-input v-model="queryForm.GoodsId" placeholder="請輸入商品ID" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="商品名稱">
          <el-input v-model="queryForm.GoodsName" placeholder="請輸入商品名稱" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="訂單狀態">
          <el-select v-model="queryForm.OrderStatus" placeholder="請選擇訂單狀態" clearable style="width: 200px">
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="已取消" value="CANCELLED" />
          </el-select>
        </el-form-item>
        <el-form-item label="處理狀態">
          <el-select v-model="queryForm.ProcessStatus" placeholder="請選擇處理狀態" clearable style="width: 200px">
            <el-option label="待處理" value="PENDING" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :loading="loading">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="info" @click="handlePrint" :loading="exporting">列印</el-button>
          <el-button type="success" @click="handleExportExcel" :loading="exporting">匯出Excel</el-button>
          <el-button type="warning" @click="handleExportPdf" :loading="exporting">匯出PDF</el-button>
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
        <el-table-column prop="OrderNo" label="訂單編號" width="150" />
        <el-table-column prop="OrderDate" label="訂單日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ShipDate" label="出貨日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ShipDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="StoreId" label="店別ID" width="120" />
        <el-table-column prop="StoreName" label="店別名稱" width="200" />
        <el-table-column prop="RetailerId" label="零售商ID" width="120" />
        <el-table-column prop="RetailerName" label="零售商名稱" width="200" />
        <el-table-column prop="ProviderId" label="供應商ID" width="120" />
        <el-table-column prop="ProviderName" label="供應商名稱" width="200" />
        <el-table-column prop="GoodsId" label="商品ID" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" show-overflow-tooltip />
        <el-table-column prop="OrderQty" label="數量" width="100" align="right" />
        <el-table-column prop="InternetPrice" label="單價" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.InternetPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderSubtotal" label="小計" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.OrderSubtotal) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderTotal" label="總計" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.OrderTotal) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderStatus" label="訂單狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getOrderStatusType(row.OrderStatus)">
              {{ getOrderStatusText(row.OrderStatus) }}
            </el-tag>
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
import { shopsApi } from '@/api/modules/shop'
import { vendorsApi } from '@/api/vendors'

export default {
  name: 'EInvoiceReport',
  setup() {
    const loading = ref(false)
    const exporting = ref(false)
    const tableData = ref([])
    const storeList = ref([])
    const providerList = ref([])
    const orderDateRange = ref(null)
    const shipDateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      ReportType: 'ECA3040',
      PageIndex: 1,
      PageSize: 20,
      SortField: 'OrderDate',
      SortOrder: 'DESC',
      OrderNo: '',
      RetailerOrderNo: '',
      StoreId: '',
      ProviderId: '',
      GoodsId: '',
      GoodsName: '',
      OrderDateFrom: null,
      OrderDateTo: null,
      ShipDateFrom: null,
      ShipDateTo: null,
      OrderStatus: '',
      ProcessStatus: ''
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
        // 處理日期範圍
        if (orderDateRange.value && orderDateRange.value.length === 2) {
          queryForm.OrderDateFrom = orderDateRange.value[0]
          queryForm.OrderDateTo = orderDateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }
        if (shipDateRange.value && shipDateRange.value.length === 2) {
          queryForm.ShipDateFrom = shipDateRange.value[0]
          queryForm.ShipDateTo = shipDateRange.value[1]
        } else {
          queryForm.ShipDateFrom = null
          queryForm.ShipDateTo = null
        }

        const data = {
          ReportType: queryForm.ReportType,
          OrderNo: queryForm.OrderNo || null,
          RetailerOrderNo: queryForm.RetailerOrderNo || null,
          StoreId: queryForm.StoreId || null,
          ProviderId: queryForm.ProviderId || null,
          GoodsId: queryForm.GoodsId || null,
          GoodsName: queryForm.GoodsName || null,
          OrderDateFrom: queryForm.OrderDateFrom || null,
          OrderDateTo: queryForm.OrderDateTo || null,
          ShipDateFrom: queryForm.ShipDateFrom || null,
          ShipDateTo: queryForm.ShipDateTo || null,
          OrderStatus: queryForm.OrderStatus || null,
          ProcessStatus: queryForm.ProcessStatus || null,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          SortField: queryForm.SortField || null,
          SortOrder: queryForm.SortOrder || null
        }
        const response = await einvoiceReportApi.getReports(data)
        if (response.data && response.data.Success && response.data.Data) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error('查詢失敗: ' + (response.data?.Message || '未知錯誤'))
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
        ReportType: 'ECA3040',
        PageIndex: 1,
        PageSize: 20,
        SortField: 'OrderDate',
        SortOrder: 'DESC',
        OrderNo: '',
        RetailerOrderNo: '',
        StoreId: '',
        ProviderId: '',
        GoodsId: '',
        GoodsName: '',
        OrderDateFrom: null,
        OrderDateTo: null,
        ShipDateFrom: null,
        ShipDateTo: null,
        OrderStatus: '',
        ProcessStatus: ''
      })
      pagination.PageIndex = 1
      handleSearch()
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      try {
        exporting.value = true
        // 處理日期範圍
        if (orderDateRange.value && orderDateRange.value.length === 2) {
          queryForm.OrderDateFrom = orderDateRange.value[0]
          queryForm.OrderDateTo = orderDateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }
        if (shipDateRange.value && shipDateRange.value.length === 2) {
          queryForm.ShipDateFrom = shipDateRange.value[0]
          queryForm.ShipDateTo = shipDateRange.value[1]
        } else {
          queryForm.ShipDateFrom = null
          queryForm.ShipDateTo = null
        }

        const data = {
          ReportType: queryForm.ReportType || 'ECA3040',
          OrderNo: queryForm.OrderNo || null,
          RetailerOrderNo: queryForm.RetailerOrderNo || null,
          StoreId: queryForm.StoreId || null,
          ProviderId: queryForm.ProviderId || null,
          GoodsId: queryForm.GoodsId || null,
          GoodsName: queryForm.GoodsName || null,
          OrderDateFrom: queryForm.OrderDateFrom || null,
          OrderDateTo: queryForm.OrderDateTo || null,
          ShipDateFrom: queryForm.ShipDateFrom || null,
          ShipDateTo: queryForm.ShipDateTo || null,
          OrderStatus: queryForm.OrderStatus || null,
          ProcessStatus: queryForm.ProcessStatus || null
        }
        const response = await einvoiceReportApi.exportExcel(data)
        
        // 下載檔案
        const blob = new Blob([response.data], {
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
      } finally {
        exporting.value = false
      }
    }

    // 匯出PDF
    const handleExportPdf = async () => {
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      try {
        exporting.value = true
        // 處理日期範圍
        if (orderDateRange.value && orderDateRange.value.length === 2) {
          queryForm.OrderDateFrom = orderDateRange.value[0]
          queryForm.OrderDateTo = orderDateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }
        if (shipDateRange.value && shipDateRange.value.length === 2) {
          queryForm.ShipDateFrom = shipDateRange.value[0]
          queryForm.ShipDateTo = shipDateRange.value[1]
        } else {
          queryForm.ShipDateFrom = null
          queryForm.ShipDateTo = null
        }

        const data = {
          ReportType: queryForm.ReportType || 'ECA3040',
          OrderNo: queryForm.OrderNo || null,
          RetailerOrderNo: queryForm.RetailerOrderNo || null,
          StoreId: queryForm.StoreId || null,
          ProviderId: queryForm.ProviderId || null,
          GoodsId: queryForm.GoodsId || null,
          GoodsName: queryForm.GoodsName || null,
          OrderDateFrom: queryForm.OrderDateFrom || null,
          OrderDateTo: queryForm.OrderDateTo || null,
          ShipDateFrom: queryForm.ShipDateFrom || null,
          ShipDateTo: queryForm.ShipDateTo || null,
          OrderStatus: queryForm.OrderStatus || null,
          ProcessStatus: queryForm.ProcessStatus || null
        }
        const response = await einvoiceReportApi.exportPdf(data)
        
        // 下載檔案
        const blob = new Blob([response.data], {
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
      } finally {
        exporting.value = false
      }
    }

    // 列印報表 (ECA3040)
    const handlePrint = async () => {
      if (!queryForm.ReportType) {
        ElMessage.warning('請選擇報表類型')
        return
      }
      try {
        exporting.value = true
        // 處理日期範圍
        if (orderDateRange.value && orderDateRange.value.length === 2) {
          queryForm.OrderDateFrom = orderDateRange.value[0]
          queryForm.OrderDateTo = orderDateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }
        if (shipDateRange.value && shipDateRange.value.length === 2) {
          queryForm.ShipDateFrom = shipDateRange.value[0]
          queryForm.ShipDateTo = shipDateRange.value[1]
        } else {
          queryForm.ShipDateFrom = null
          queryForm.ShipDateTo = null
        }

        const data = {
          ReportType: queryForm.ReportType || 'ECA3040',
          OrderNo: queryForm.OrderNo || null,
          RetailerOrderNo: queryForm.RetailerOrderNo || null,
          StoreId: queryForm.StoreId || null,
          ProviderId: queryForm.ProviderId || null,
          GoodsId: queryForm.GoodsId || null,
          GoodsName: queryForm.GoodsName || null,
          OrderDateFrom: queryForm.OrderDateFrom || null,
          OrderDateTo: queryForm.OrderDateTo || null,
          ShipDateFrom: queryForm.ShipDateFrom || null,
          ShipDateTo: queryForm.ShipDateTo || null,
          OrderStatus: queryForm.OrderStatus || null,
          ProcessStatus: queryForm.ProcessStatus || null
        }
        const response = await einvoiceReportApi.print(data)
        
        // 在新視窗打開PDF進行列印
        const blob = new Blob([response.data], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const printWindow = window.open(url, '_blank')
        if (printWindow) {
          printWindow.onload = () => {
            printWindow.print()
          }
        } else {
          // 如果彈出視窗被阻擋，則下載檔案
          const link = document.createElement('a')
          link.href = url
          const reportTypeName = getReportTypeName(queryForm.ReportType)
          link.download = `電子發票報表_${reportTypeName}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
          document.body.appendChild(link)
          link.click()
          document.body.removeChild(link)
          ElMessage.info('請在新視窗中列印檔案')
        }
        
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印失敗:', error)
        ElMessage.error('列印失敗: ' + (error.message || '未知錯誤'))
      } finally {
        exporting.value = false
      }
    }

    // 取得報表類型名稱
    const getReportTypeName = (reportType) => {
      const names = {
        'ECA3040': '電子發票報表查詢',
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

    // 取得訂單狀態類型
    const getOrderStatusType = (status) => {
      const statusMap = {
        COMPLETED: 'success',
        PROCESSING: 'warning',
        CANCELLED: 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得訂單狀態文字
    const getOrderStatusText = (status) => {
      const statusMap = {
        COMPLETED: '已完成',
        PROCESSING: '處理中',
        CANCELLED: '已取消'
      }
      return statusMap[status] || status
    }

    // 載入店別列表
    const loadStoreList = async () => {
      try {
        const response = await shopsApi.getShops({ PageSize: 1000 })
        if (response.data.Success) {
          storeList.value = response.data.Data.map(shop => ({
            StoreId: shop.ShopId,
            StoreName: shop.ShopName
          }))
        }
      } catch (error) {
        console.error('載入店別列表失敗：', error)
      }
    }

    // 載入供應商列表
    const loadProviderList = async () => {
      try {
        const response = await vendorsApi.getVendors({ PageSize: 1000 })
        if (response.data.Success) {
          providerList.value = response.data.Data.Items.map(vendor => ({
            ProviderId: vendor.VendorId,
            ProviderName: vendor.VendorName
          }))
        }
      } catch (error) {
        console.error('載入供應商列表失敗：', error)
      }
    }

    // 初始化
    onMounted(async () => {
      await Promise.all([loadStoreList(), loadProviderList()])
    })

    return {
      loading,
      exporting,
      tableData,
      storeList,
      providerList,
      orderDateRange,
      shipDateRange,
      queryForm,
      pagination,
      formatDate,
      formatCurrency,
      handleSearch,
      handleReset,
      handlePrint,
      handleExportExcel,
      handleExportPdf,
      handleSizeChange,
      handlePageChange,
      getOrderStatusType,
      getOrderStatusText
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


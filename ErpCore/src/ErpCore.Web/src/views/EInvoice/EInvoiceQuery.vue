<template>
  <div class="e-invoice-query">
    <h1>電子發票查詢作業 (ECA3020)</h1>

    <!-- 查詢表單 -->
    <el-card class="query-card" shadow="never">
      <el-form :model="queryForm" inline label-width="120px">
        <el-form-item label="訂單編號">
          <el-input
            v-model="queryForm.OrderNo"
            placeholder="請輸入訂單編號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="零售商訂單編號">
          <el-input
            v-model="queryForm.RetailerOrderNo"
            placeholder="請輸入零售商訂單編號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="訂單日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item label="店別">
          <el-select
            v-model="queryForm.StoreId"
            placeholder="請選擇店別"
            filterable
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="store in storeList"
              :key="store.StoreId"
              :label="store.StoreName"
              :value="store.StoreId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="供應商">
          <el-select
            v-model="queryForm.ProviderId"
            placeholder="請選擇供應商"
            filterable
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="provider in providerList"
              :key="provider.ProviderId"
              :label="provider.ProviderName"
              :value="provider.ProviderId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="商品ID">
          <el-input
            v-model="queryForm.GoodsId"
            placeholder="請輸入商品ID"
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
        <el-form-item label="訂單狀態">
          <el-select
            v-model="queryForm.OrderStatus"
            placeholder="請選擇訂單狀態"
            clearable
            style="width: 200px"
          >
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="已取消" value="CANCELLED" />
          </el-select>
        </el-form-item>
        <el-form-item label="處理狀態">
          <el-select
            v-model="queryForm.ProcessStatus"
            placeholder="請選擇處理狀態"
            clearable
            style="width: 200px"
          >
            <el-option label="待處理" value="PENDING" />
            <el-option label="處理中" value="PROCESSING" />
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery" :loading="loading">
            查詢
          </el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel" :loading="exporting">
            匯出Excel
          </el-button>
          <el-button type="warning" @click="handleExportPdf" :loading="exporting">
            匯出PDF
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="invoiceList"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="InvoiceId" label="發票ID" width="100" />
        <el-table-column prop="OrderNo" label="訂單編號" width="150" />
        <el-table-column prop="RetailerOrderNo" label="零售商訂單編號" width="150" />
        <el-table-column prop="OrderDate" label="訂單日期" width="120" :formatter="formatDate" />
        <el-table-column prop="StoreName" label="店別" width="120" />
        <el-table-column prop="ProviderName" label="供應商" width="150" />
        <el-table-column prop="GoodsId" label="商品ID" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" show-overflow-tooltip />
        <el-table-column prop="OrderQty" label="數量" width="80" align="right" />
        <el-table-column prop="OrderTotal" label="總計" width="120" align="right" :formatter="formatCurrency" />
        <el-table-column prop="OrderStatus" label="訂單狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getOrderStatusType(row.OrderStatus)">
              {{ getOrderStatusText(row.OrderStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ProcessStatus" label="處理狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getProcessStatusType(row.ProcessStatus)">
              {{ getProcessStatusText(row.ProcessStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">
              查看
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
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

    <!-- 查看詳情對話框 -->
    <el-dialog v-model="detailDialogVisible" title="電子發票詳情" width="800px">
      <el-descriptions :column="2" border v-if="currentInvoice">
        <el-descriptions-item label="發票ID">{{ currentInvoice.InvoiceId }}</el-descriptions-item>
        <el-descriptions-item label="訂單編號">{{ currentInvoice.OrderNo }}</el-descriptions-item>
        <el-descriptions-item label="零售商訂單編號">{{ currentInvoice.RetailerOrderNo }}</el-descriptions-item>
        <el-descriptions-item label="零售商訂單明細編號">{{ currentInvoice.RetailerOrderDetailNo }}</el-descriptions-item>
        <el-descriptions-item label="訂單日期">{{ formatDate(null, null, currentInvoice.OrderDate) }}</el-descriptions-item>
        <el-descriptions-item label="店別">{{ currentInvoice.StoreName }}</el-descriptions-item>
        <el-descriptions-item label="供應商">{{ currentInvoice.ProviderName }}</el-descriptions-item>
        <el-descriptions-item label="商品ID">{{ currentInvoice.GoodsId }}</el-descriptions-item>
        <el-descriptions-item label="商品名稱" :span="2">{{ currentInvoice.GoodsName }}</el-descriptions-item>
        <el-descriptions-item label="規格ID">{{ currentInvoice.SpecId }}</el-descriptions-item>
        <el-descriptions-item label="供應商商品編號">{{ currentInvoice.ProviderGoodsId }}</el-descriptions-item>
        <el-descriptions-item label="規格顏色">{{ currentInvoice.SpecColor }}</el-descriptions-item>
        <el-descriptions-item label="規格尺寸">{{ currentInvoice.SpecSize }}</el-descriptions-item>
        <el-descriptions-item label="建議售價">{{ formatCurrency(null, null, currentInvoice.SuggestPrice) }}</el-descriptions-item>
        <el-descriptions-item label="網路售價">{{ formatCurrency(null, null, currentInvoice.InternetPrice) }}</el-descriptions-item>
        <el-descriptions-item label="運送方式">{{ currentInvoice.ShippingType }}</el-descriptions-item>
        <el-descriptions-item label="運費">{{ formatCurrency(null, null, currentInvoice.ShippingFee) }}</el-descriptions-item>
        <el-descriptions-item label="訂單數量">{{ currentInvoice.OrderQty }}</el-descriptions-item>
        <el-descriptions-item label="訂單運費">{{ formatCurrency(null, null, currentInvoice.OrderShippingFee) }}</el-descriptions-item>
        <el-descriptions-item label="訂單小計">{{ formatCurrency(null, null, currentInvoice.OrderSubtotal) }}</el-descriptions-item>
        <el-descriptions-item label="訂單總計">{{ formatCurrency(null, null, currentInvoice.OrderTotal) }}</el-descriptions-item>
        <el-descriptions-item label="訂單狀態">
          <el-tag :type="getOrderStatusType(currentInvoice.OrderStatus)">
            {{ getOrderStatusText(currentInvoice.OrderStatus) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="處理狀態">
          <el-tag :type="getProcessStatusType(currentInvoice.ProcessStatus)">
            {{ getProcessStatusText(currentInvoice.ProcessStatus) }}
          </el-tag>
        </el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="detailDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { einvoiceApi } from '@/api/einvoice'
import { ElMessage } from 'element-plus'

export default {
  name: 'EInvoiceQuery',
  setup() {
    const loading = ref(false)
    const exporting = ref(false)
    const invoiceList = ref([])
    const storeList = ref([])
    const providerList = ref([])
    const dateRange = ref(null)
    const detailDialogVisible = ref(false)
    const currentInvoice = ref(null)

    const queryForm = reactive({
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
      OrderStatus: '',
      ProcessStatus: ''
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 查詢
    const handleQuery = async () => {
      try {
        loading.value = true
        if (dateRange.value && dateRange.value.length === 2) {
          queryForm.OrderDateFrom = dateRange.value[0]
          queryForm.OrderDateTo = dateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }
        queryForm.PageIndex = pagination.PageIndex
        queryForm.PageSize = pagination.PageSize

        const response = await einvoiceApi.getEInvoices(queryForm)
        if (response.data.Success) {
          invoiceList.value = response.data.Data.Items
          pagination.TotalCount = response.data.Data.TotalCount
        } else {
          ElMessage.error(response.data.Message || '查詢失敗')
        }
      } catch (error) {
        ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 重置
    const handleReset = () => {
      queryForm.OrderNo = ''
      queryForm.RetailerOrderNo = ''
      queryForm.StoreId = ''
      queryForm.ProviderId = ''
      queryForm.GoodsId = ''
      queryForm.GoodsName = ''
      queryForm.OrderStatus = ''
      queryForm.ProcessStatus = ''
      dateRange.value = null
      pagination.PageIndex = 1
      handleQuery()
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        exporting.value = true
        if (dateRange.value && dateRange.value.length === 2) {
          queryForm.OrderDateFrom = dateRange.value[0]
          queryForm.OrderDateTo = dateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }

        const response = await einvoiceApi.exportExcel(queryForm)
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `電子發票查詢_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        ElMessage.error('匯出失敗：' + (error.message || '未知錯誤'))
      } finally {
        exporting.value = false
      }
    }

    // 匯出PDF
    const handleExportPdf = async () => {
      try {
        exporting.value = true
        if (dateRange.value && dateRange.value.length === 2) {
          queryForm.OrderDateFrom = dateRange.value[0]
          queryForm.OrderDateTo = dateRange.value[1]
        } else {
          queryForm.OrderDateFrom = null
          queryForm.OrderDateTo = null
        }

        const response = await einvoiceApi.exportPdf(queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `電子發票查詢_${new Date().toISOString().slice(0, 10)}.pdf`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        ElMessage.error('匯出失敗：' + (error.message || '未知錯誤'))
      } finally {
        exporting.value = false
      }
    }

    // 查看詳情
    const handleView = async (row) => {
      try {
        const response = await einvoiceApi.getEInvoice(row.InvoiceId)
        if (response.data.Success) {
          currentInvoice.value = response.data.Data
          detailDialogVisible.value = true
        } else {
          ElMessage.error(response.data.Message || '查詢失敗')
        }
      } catch (error) {
        ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      handleQuery()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      handleQuery()
    }

    // 格式化日期
    const formatDate = (row, column, cellValue) => {
      if (!cellValue) return ''
      const date = new Date(cellValue)
      return date.toLocaleDateString('zh-TW')
    }

    // 格式化金額
    const formatCurrency = (row, column, cellValue) => {
      if (cellValue == null) return ''
      return new Intl.NumberFormat('zh-TW', {
        style: 'currency',
        currency: 'TWD',
        minimumFractionDigits: 0
      }).format(cellValue)
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

    // 取得處理狀態類型
    const getProcessStatusType = (status) => {
      const statusMap = {
        COMPLETED: 'success',
        PROCESSING: 'warning',
        PENDING: 'info',
        FAILED: 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得處理狀態文字
    const getProcessStatusText = (status) => {
      const statusMap = {
        COMPLETED: '已完成',
        PROCESSING: '處理中',
        PENDING: '待處理',
        FAILED: '失敗'
      }
      return statusMap[status] || status
    }

    // 初始化
    onMounted(() => {
      handleQuery()
      // TODO: 載入店別和供應商列表
    })

    return {
      loading,
      exporting,
      invoiceList,
      storeList,
      providerList,
      dateRange,
      queryForm,
      pagination,
      detailDialogVisible,
      currentInvoice,
      handleQuery,
      handleReset,
      handleExportExcel,
      handleExportPdf,
      handleView,
      handleSizeChange,
      handlePageChange,
      formatDate,
      formatCurrency,
      getOrderStatusType,
      getOrderStatusText,
      getProcessStatusType,
      getProcessStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.e-invoice-query {
  padding: 20px;
}

.query-card {
  margin-bottom: 20px;
}

.table-card {
  margin-top: 20px;
}
</style>


<template>
  <div class="procurement-report">
    <div class="page-header">
      <h1>採購報表查詢 (SYSP410-SYSP4I0)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="採購單號">
          <el-input v-model="queryForm.PurchaseOrderNo" placeholder="請輸入採購單號" clearable />
        </el-form-item>
        <el-form-item label="供應商代號">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入供應商代號" clearable />
        </el-form-item>
        <el-form-item label="採購日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable>
            <el-option label="採購明細表" value="DETAIL" />
            <el-option label="採購統計表" value="SUMMARY" />
            <el-option label="供應商採購表" value="SUPPLIER" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="草稿" value="D" />
            <el-option label="已確認" value="C" />
            <el-option label="已完成" value="F" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport('Excel')">匯出Excel</el-button>
          <el-button type="warning" @click="handleExport('PDF')">匯出PDF</el-button>
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
        <el-table-column prop="PurchaseOrderNo" label="採購單號" width="150" />
        <el-table-column prop="PurchaseDate" label="採購日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.PurchaseDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="SupplierId" label="供應商代號" width="120" />
        <el-table-column prop="SupplierName" label="供應商名稱" width="200" />
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusName(row.Status) }}
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
import { procurementApi } from '@/api/procurement'

export default {
  name: 'ProcurementReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const dateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      PurchaseOrderNo: '',
      SupplierId: '',
      PurchaseDateFrom: null,
      PurchaseDateTo: null,
      Status: '',
      ReportType: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 日期範圍變更
    const handleDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        queryForm.PurchaseDateFrom = dates[0]
        queryForm.PurchaseDateTo = dates[1]
      } else {
        queryForm.PurchaseDateFrom = null
        queryForm.PurchaseDateTo = null
      }
    }

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

    // 取得狀態名稱
    const getStatusName = (status) => {
      const statuses = {
        'D': '草稿',
        'C': '已確認',
        'F': '已完成'
      }
      return statuses[status] || status
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'D': 'info',
        'C': 'success',
        'F': 'success'
      }
      return types[status] || 'info'
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        // 移除空值
        Object.keys(params).forEach(key => {
          if (params[key] === '' || params[key] === null) {
            delete params[key]
          }
        })
        const response = await procurementApi.getProcurementReports(params)
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
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      dateRange.value = null
      Object.assign(queryForm, {
        PurchaseOrderNo: '',
        SupplierId: '',
        PurchaseDateFrom: null,
        PurchaseDateTo: null,
        Status: '',
        ReportType: ''
      })
      handleSearch()
    }

    // 匯出
    const handleExport = async (exportType) => {
      try {
        const params = { ...queryForm }
        // 移除空值和分頁參數
        Object.keys(params).forEach(key => {
          if (params[key] === '' || params[key] === null || key === 'PageIndex' || key === 'PageSize') {
            delete params[key]
          }
        })

        const exportData = {
          Query: params,
          ExportType: exportType,
          FileName: `採購報表_${new Date().toISOString().slice(0, 10)}.${exportType === 'PDF' ? 'pdf' : 'xlsx'}`
        }

        const response = await procurementApi.exportProcurementReport(exportData)
        
        // 創建下載連結
        const blob = new Blob([response.data], {
          type: exportType === 'PDF' ? 'application/pdf' : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = exportData.FileName
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)

        ElMessage.success('匯出成功')
      } catch (error) {
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 初始化
    onMounted(() => {
      loadData()
    })

    return {
      loading,
      tableData,
      queryForm,
      pagination,
      dateRange,
      handleDateRangeChange,
      formatDate,
      formatCurrency,
      getStatusName,
      getStatusType,
      handleSearch,
      handleReset,
      handleExport,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.procurement-report {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
    }
  }

  .search-card {
    margin-bottom: 20px;
    
    .search-form {
      .el-form-item {
        margin-bottom: 0;
      }
    }
  }

  .table-card {
    .el-table {
      .el-button {
        margin-right: 5px;
      }
    }
  }
}
</style>


<template>
  <div class="customer-report">
    <div class="page-header">
      <h1>客戶報表 (CUS5130)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="客戶編號">
              <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶編號" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客戶名稱">
              <el-input v-model="queryForm.CustomerName" placeholder="請輸入客戶名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="統一編號">
              <el-input v-model="queryForm.GuiId" placeholder="請輸入統一編號" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="狀態">
              <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="城市">
              <el-input v-model="queryForm.City" placeholder="請輸入城市" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="區域">
              <el-input v-model="queryForm.Canton" placeholder="請輸入區域" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="月結客戶">
              <el-select v-model="queryForm.MonthlyYn" placeholder="請選擇" clearable>
                <el-option label="是" value="Y" />
                <el-option label="否" value="N" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="交易日期起">
              <el-date-picker v-model="queryForm.TransDateFrom" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="交易日期迄">
              <el-date-picker v-model="queryForm.TransDateTo" type="date" placeholder="請選擇日期" style="width: 100%" />
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
        <el-table-column prop="CustomerId" label="客戶編號" width="150" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="GuiId" label="統一編號" width="120" />
        <el-table-column prop="ContactStaff" label="聯絡人" width="120" />
        <el-table-column prop="CompTel" label="公司電話" width="150" />
        <el-table-column prop="City" label="城市" width="120" />
        <el-table-column prop="Canton" label="區域" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TransactionCount" label="交易次數" width="120" />
        <el-table-column prop="TotalAmount" label="總交易金額" width="150">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="AccAmt" label="累積金額" width="150">
          <template #default="{ row }">
            {{ formatCurrency(row.AccAmt) }}
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
import { customerReportApi } from '@/api/customer'

export default {
  name: 'CustomerReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      CustomerId: '',
      CustomerName: '',
      GuiId: '',
      Status: '',
      City: '',
      Canton: '',
      MonthlyYn: '',
      TransDateFrom: null,
      TransDateTo: null
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

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
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          Filters: {
            ...queryForm
          }
        }
        const response = await customerReportApi.getReport(data)
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
      Object.assign(queryForm, {
        CustomerId: '',
        CustomerName: '',
        GuiId: '',
        Status: '',
        City: '',
        Canton: '',
        MonthlyYn: '',
        TransDateFrom: null,
        TransDateTo: null
      })
      handleSearch()
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        const data = {
          Filters: { ...queryForm }
        }
        const response = await customerReportApi.exportExcel(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `客戶報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
      try {
        const data = {
          Filters: { ...queryForm }
        }
        const response = await customerReportApi.exportPdf(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `客戶報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      tableData,
      queryForm,
      pagination,
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
.customer-report {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


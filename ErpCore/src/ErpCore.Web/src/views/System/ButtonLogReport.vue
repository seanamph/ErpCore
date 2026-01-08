<template>
  <div class="button-log-report">
    <div class="page-header">
      <h1>按鈕操作記錄報表 (SYS0790)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="使用者ID">
              <el-input v-model="queryForm.UserId" placeholder="請輸入使用者ID" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="作業ID">
              <el-input v-model="queryForm.ProgramId" placeholder="請輸入作業ID" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="按鈕ID">
              <el-input v-model="queryForm.ButtonId" placeholder="請輸入按鈕ID" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="操作日期起">
              <el-date-picker v-model="queryForm.StartDate" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="操作日期迄">
              <el-date-picker v-model="queryForm.EndDate" type="date" placeholder="請選擇日期" style="width: 100%" />
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
        <el-table-column prop="UserId" label="使用者ID" width="120" />
        <el-table-column prop="UserName" label="使用者名稱" width="150" />
        <el-table-column prop="ProgramId" label="作業ID" width="120" />
        <el-table-column prop="ProgramName" label="作業名稱" width="200" />
        <el-table-column prop="ButtonId" label="按鈕ID" width="120" />
        <el-table-column prop="ButtonName" label="按鈕名稱" width="150" />
        <el-table-column prop="OperationDate" label="操作日期" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.OperationDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="IpAddress" label="IP位址" width="150" />
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
import { buttonLogApi } from '@/api/systemPermission'

export default {
  name: 'ButtonLogReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      UserId: '',
      ProgramId: '',
      ButtonId: '',
      StartDate: null,
      EndDate: null
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 格式化日期時間
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
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
        const response = await buttonLogApi.getButtonLogs(params)
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
        UserId: '',
        ProgramId: '',
        ButtonId: '',
        StartDate: null,
        EndDate: null
      })
      handleSearch()
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        const data = { ...queryForm }
        const response = await buttonLogApi.exportReport(data, 'excel')
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `按鈕操作記錄報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
        const data = { ...queryForm }
        const response = await buttonLogApi.exportReport(data, 'pdf')
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `按鈕操作記錄報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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
      formatDateTime,
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
.button-log-report {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


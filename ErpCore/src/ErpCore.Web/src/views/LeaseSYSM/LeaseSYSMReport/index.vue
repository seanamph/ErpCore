<template>
  <div class="lease-sysm-report-management">
    <div class="page-header">
      <h1>租賃報表查詢 (SYSM141-SYSM144)</h1>
    </div>

    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="查詢編號">
          <el-input v-model="queryForm.QueryId" placeholder="請輸入查詢編號" clearable />
        </el-form-item>
        <el-form-item label="報表類型">
          <el-select v-model="queryForm.ReportType" placeholder="請選擇報表類型" clearable>
            <el-option label="租賃明細報表" value="DETAIL" />
            <el-option label="租賃統計報表" value="STATISTICS" />
            <el-option label="租金收入報表" value="RENT_INCOME" />
            <el-option label="停車費收入報表" value="PARKING_INCOME" />
            <el-option label="租賃分析報表" value="ANALYSIS" />
          </el-select>
        </el-form-item>
        <el-form-item label="查詢名稱">
          <el-input v-model="queryForm.QueryName" placeholder="請輸入查詢名稱" clearable />
        </el-form-item>
        <el-form-item label="查詢日期">
          <el-date-picker
            v-model="queryForm.QueryDateFrom"
            type="date"
            placeholder="開始日期"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="至">
          <el-date-picker
            v-model="queryForm.QueryDateTo"
            type="date"
            placeholder="結束日期"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增查詢</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="QueryId" label="查詢編號" width="150" />
        <el-table-column prop="ReportType" label="報表類型" width="150">
          <template #default="{ row }">
            <el-tag :type="getReportTypeTagType(row.ReportType)">
              {{ getReportTypeText(row.ReportType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="QueryName" label="查詢名稱" width="200" />
        <el-table-column prop="QueryDate" label="查詢日期" width="150" />
        <el-table-column prop="CreatedBy" label="建立人員" width="120" />
        <el-table-column prop="CreatedAt" label="建立時間" width="180" />
        <el-table-column prop="QueryParams" label="查詢參數" min-width="200" show-overflow-tooltip>
          <template #default="{ row }">
            <el-tooltip v-if="row.QueryParams" :content="row.QueryParams" placement="top">
              <span>{{ truncateText(row.QueryParams, 50) }}</span>
            </el-tooltip>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="info" size="small" @click="handleViewResult(row)">查看結果</el-button>
            <el-button type="success" size="small" @click="handleExport(row)">匯出</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>

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

    <!-- 新增/查看查詢對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="900px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="查詢編號" prop="QueryId">
              <el-input v-model="formData.QueryId" :disabled="isEdit" placeholder="請輸入查詢編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="報表類型" prop="ReportType">
              <el-select v-model="formData.ReportType" placeholder="請選擇報表類型" style="width: 100%">
                <el-option label="租賃明細報表" value="DETAIL" />
                <el-option label="租賃統計報表" value="STATISTICS" />
                <el-option label="租金收入報表" value="RENT_INCOME" />
                <el-option label="停車費收入報表" value="PARKING_INCOME" />
                <el-option label="租賃分析報表" value="ANALYSIS" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="查詢名稱" prop="QueryName">
          <el-input v-model="formData.QueryName" placeholder="請輸入查詢名稱" />
        </el-form-item>
        <el-form-item label="查詢日期" prop="QueryDate">
          <el-date-picker
            v-model="formData.QueryDate"
            type="date"
            placeholder="請選擇查詢日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="查詢參數" prop="QueryParams">
          <el-input
            v-model="formData.QueryParams"
            type="textarea"
            :rows="5"
            placeholder="請輸入查詢參數（JSON格式）"
          />
        </el-form-item>
        <el-form-item label="查詢結果" prop="QueryResult" v-if="isEdit">
          <el-input
            v-model="formData.QueryResult"
            type="textarea"
            :rows="8"
            placeholder="查詢結果（JSON格式）"
            :disabled="true"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit" v-if="!isEdit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 查看結果對話框 -->
    <el-dialog
      v-model="resultDialogVisible"
      title="查詢結果"
      width="1200px"
      :close-on-click-modal="false"
    >
      <div v-if="resultData" class="result-container">
        <el-table
          :data="resultTableData"
          border
          stripe
          style="width: 100%"
          v-loading="resultLoading"
        >
          <el-table-column
            v-for="(column, index) in resultColumns"
            :key="index"
            :prop="column.prop"
            :label="column.label"
            :width="column.width"
            :min-width="column.minWidth"
          />
        </el-table>
      </div>
      <div v-else class="no-result">
        <el-empty description="無查詢結果" />
      </div>

      <template #footer>
        <el-button @click="resultDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handleExportResult">匯出結果</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { leaseSYSMApi } from '@/api/leaseSYSM'

export default {
  name: 'LeaseSYSMReportManagement',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const resultDialogVisible = ref(false)
    const resultLoading = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const resultData = ref(null)
    const resultTableData = ref([])
    const resultColumns = ref([])
    const currentRow = ref(null)
    
    const queryForm = reactive({
      QueryId: '',
      ReportType: '',
      QueryName: '',
      QueryDateFrom: '',
      QueryDateTo: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })
    
    const formData = reactive({
      QueryId: '',
      ReportType: '',
      QueryName: '',
      QueryDate: '',
      QueryParams: '',
      QueryResult: ''
    })
    
    const formRules = {
      QueryId: [{ required: true, message: '請輸入查詢編號', trigger: 'blur' }],
      ReportType: [{ required: true, message: '請選擇報表類型', trigger: 'change' }],
      QueryDate: [{ required: true, message: '請選擇查詢日期', trigger: 'change' }]
    }
    
    const dialogTitle = computed(() => {
      return isEdit.value ? '查看查詢記錄' : '新增查詢記錄'
    })
    
    // 載入資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await leaseSYSMApi.getLeaseReports(params)
        if (response.data && response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢報表記錄列表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }
    
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }
    
    const handleReset = () => {
      Object.assign(queryForm, {
        QueryId: '',
        ReportType: '',
        QueryName: '',
        QueryDateFrom: '',
        QueryDateTo: ''
      })
      handleSearch()
    }
    
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        QueryId: '',
        ReportType: '',
        QueryName: '',
        QueryDate: '',
        QueryParams: '',
        QueryResult: ''
      })
    }
    
    const handleView = async (row) => {
      try {
        const response = await leaseSYSMApi.getLeaseReport(row.QueryId)
        if (response.data && response.data.Success) {
          Object.assign(formData, response.data.Data)
          isEdit.value = true
          dialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢報表記錄失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleViewResult = async (row) => {
      currentRow.value = row
      resultLoading.value = true
      resultDialogVisible.value = true
      
      try {
        const response = await leaseSYSMApi.getLeaseReport(row.QueryId)
        if (response.data && response.data.Success) {
          resultData.value = response.data.Data
          
          // 解析查詢結果
          if (response.data.Data.QueryResult) {
            try {
              const result = JSON.parse(response.data.Data.QueryResult)
              if (Array.isArray(result) && result.length > 0) {
                resultTableData.value = result
                // 動態生成表格欄位
                resultColumns.value = Object.keys(result[0]).map(key => ({
                  prop: key,
                  label: key,
                  minWidth: 120
                }))
              } else if (result.items && Array.isArray(result.items)) {
                resultTableData.value = result.items
                if (result.items.length > 0) {
                  resultColumns.value = Object.keys(result.items[0]).map(key => ({
                    prop: key,
                    label: key,
                    minWidth: 120
                  }))
                }
              } else {
                resultTableData.value = []
                resultColumns.value = []
              }
            } catch (e) {
              console.error('解析查詢結果失敗:', e)
              resultTableData.value = []
              resultColumns.value = []
            }
          } else {
            resultTableData.value = []
            resultColumns.value = []
          }
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢報表結果失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        resultLoading.value = false
      }
    }
    
    const handleExport = async (row) => {
      try {
        const response = await leaseSYSMApi.exportLeaseReports({
          QueryId: row.QueryId,
          ReportType: row.ReportType
        })
        
        // 處理檔案下載
        if (response.data) {
          const blob = new Blob([response.data])
          const url = window.URL.createObjectURL(blob)
          const link = document.createElement('a')
          link.href = url
          link.download = `租賃報表_${row.QueryId}_${new Date().getTime()}.xlsx`
          document.body.appendChild(link)
          link.click()
          document.body.removeChild(link)
          window.URL.revokeObjectURL(url)
          ElMessage.success('匯出成功')
        }
      } catch (error) {
        console.error('匯出報表失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleExportResult = () => {
      if (!resultTableData.value || resultTableData.value.length === 0) {
        ElMessage.warning('無資料可匯出')
        return
      }
      
      // 簡單的 CSV 匯出
      const headers = resultColumns.value.map(col => col.label).join(',')
      const rows = resultTableData.value.map(row => {
        return resultColumns.value.map(col => {
          const value = row[col.prop]
          return value !== null && value !== undefined ? String(value) : ''
        }).join(',')
      })
      
      const csvContent = [headers, ...rows].join('\n')
      const blob = new Blob(['\uFEFF' + csvContent], { type: 'text/csv;charset=utf-8;' })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `租賃報表結果_${currentRow.value?.QueryId}_${new Date().getTime()}.csv`
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(url)
      ElMessage.success('匯出成功')
    }
    
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除查詢記錄「${row.QueryId}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseSYSMApi.deleteLeaseReport(row.QueryId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除查詢記錄失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        await leaseSYSMApi.createLeaseReport(formData)
        ElMessage.success('新增成功')
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error('新增失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }
    
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }
    
    // 格式化方法
    const truncateText = (text, length) => {
      if (!text) return '-'
      if (text.length <= length) return text
      return text.substring(0, length) + '...'
    }
    
    const getReportTypeText = (type) => {
      const map = {
        'DETAIL': '租賃明細報表',
        'STATISTICS': '租賃統計報表',
        'RENT_INCOME': '租金收入報表',
        'PARKING_INCOME': '停車費收入報表',
        'ANALYSIS': '租賃分析報表'
      }
      return map[type] || type
    }
    
    const getReportTypeTagType = (type) => {
      const map = {
        'DETAIL': 'primary',
        'STATISTICS': 'success',
        'RENT_INCOME': 'warning',
        'PARKING_INCOME': 'info',
        'ANALYSIS': 'danger'
      }
      return map[type] || 'info'
    }
    
    // 初始化
    onMounted(() => {
      loadData()
    })
    
    return {
      loading,
      dialogVisible,
      resultDialogVisible,
      resultLoading,
      isEdit,
      formRef,
      tableData,
      resultData,
      resultTableData,
      resultColumns,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleViewResult,
      handleExport,
      handleExportResult,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange,
      truncateText,
      getReportTypeText,
      getReportTypeTagType
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.lease-sysm-report-management {
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
      .el-tag {
        font-weight: 500;
      }
    }
  }

  .result-container {
    max-height: 600px;
    overflow-y: auto;
  }

  .no-result {
    padding: 40px;
    text-align: center;
  }
}
</style>


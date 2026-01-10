<template>
  <div class="module-7">
    <div class="page-header">
      <h1>報表模組7 (SYS7000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表代碼">
          <el-input v-model="queryForm.ReportCode" placeholder="請輸入報表代碼" clearable />
        </el-form-item>
        <el-form-item label="報表名稱">
          <el-input v-model="queryForm.ReportName" placeholder="請輸入報表名稱" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 報表列表 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="ReportCode" label="報表代碼" width="150" />
        <el-table-column prop="ReportName" label="報表名稱" width="200" />
        <el-table-column prop="QueryName" label="查詢名稱" width="200" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleQueryReport(row)">執行查詢</el-button>
            <el-button type="success" size="small" @click="handleExportReport(row)">匯出</el-button>
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
        style="margin-top: 20px; text-align: right"
      />
    </el-card>

    <!-- 報表查詢對話框 -->
    <el-dialog
      title="執行報表查詢"
      v-model="queryDialogVisible"
      width="800px"
      @close="handleQueryDialogClose"
    >
      <el-form :model="queryRequest" ref="queryFormRef" label-width="120px">
        <el-form-item label="報表代碼">
          <el-input v-model="currentReport.ReportCode" disabled />
        </el-form-item>
        <el-form-item label="報表名稱">
          <el-input v-model="currentReport.ReportName" disabled />
        </el-form-item>
        <el-form-item label="日期起">
          <el-date-picker
            v-model="queryRequest.QueryParams.DateFrom"
            type="date"
            placeholder="選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="日期迄">
          <el-date-picker
            v-model="queryRequest.QueryParams.DateTo"
            type="date"
            placeholder="選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleQueryDialogClose">取消</el-button>
        <el-button type="primary" @click="handleExecuteQuery">執行查詢</el-button>
      </template>
    </el-dialog>

    <!-- 查詢結果對話框 -->
    <el-dialog
      title="查詢結果"
      v-model="resultDialogVisible"
      width="1200px"
    >
      <el-table
        :data="queryResult"
        border
        stripe
        style="width: 100%"
        max-height="500"
      >
        <el-table-column
          v-for="column in resultColumns"
          :key="column.prop"
          :prop="column.prop"
          :label="column.label"
          :width="column.width"
        />
      </el-table>
      <template #footer>
        <el-button type="primary" @click="handleExportResult">匯出結果</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { reportExtensionApi } from '@/api/reportExtension'

// 查詢表單
const queryForm = reactive({
  ReportCode: '',
  ReportName: '',
  Status: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const queryDialogVisible = ref(false)
const resultDialogVisible = ref(false)
const currentReport = ref({})
const queryFormRef = ref(null)
const queryRequest = reactive({
  QueryParams: {
    DateFrom: '',
    DateTo: ''
  },
  PageIndex: 1,
  PageSize: 20
})
const queryResult = ref([])
const resultColumns = ref([])

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ReportCode: queryForm.ReportCode || undefined,
      ReportName: queryForm.ReportName || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await reportExtensionApi.module7.getReports(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ReportCode = ''
  queryForm.ReportName = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 執行報表查詢
const handleQueryReport = (row) => {
  currentReport.value = { ...row }
  queryRequest.QueryParams.DateFrom = ''
  queryRequest.QueryParams.DateTo = ''
  queryDialogVisible.value = true
}

// 執行查詢
const handleExecuteQuery = async () => {
  try {
    const request = {
      QueryParams: queryRequest.QueryParams,
      PageIndex: queryRequest.PageIndex,
      PageSize: queryRequest.PageSize
    }
    const response = await reportExtensionApi.module7.queryReport(currentReport.value.ReportCode, request)
    if (response.data.success) {
      queryResult.value = response.data.data.items || []
      if (queryResult.value.length > 0) {
        // 動態生成表格欄位
        resultColumns.value = Object.keys(queryResult.value[0]).map(key => ({
          prop: key,
          label: key,
          width: 150
        }))
      }
      queryDialogVisible.value = false
      resultDialogVisible.value = true
    } else {
      ElMessage.error(response.data.message || '執行查詢失敗')
    }
  } catch (error) {
    ElMessage.error('執行查詢失敗：' + error.message)
  }
}

// 匯出報表
const handleExportReport = async (row) => {
  try {
    ElMessage.info('匯出功能開發中')
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 匯出結果
const handleExportResult = () => {
  ElMessage.info('匯出功能開發中')
}

// 關閉查詢對話框
const handleQueryDialogClose = () => {
  queryDialogVisible.value = false
  currentReport.value = {}
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.module-7 {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: bold;
  margin: 0;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}

.table-card {
  margin-bottom: 20px;
}
</style>


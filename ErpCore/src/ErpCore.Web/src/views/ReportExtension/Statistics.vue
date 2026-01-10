<template>
  <div class="report-statistics">
    <div class="page-header">
      <h1>報表統計作業 (SYS7C10, SYS7C30)</h1>
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
        <el-form-item label="統計類型">
          <el-select v-model="queryForm.StatType" placeholder="請選擇統計類型" clearable>
            <el-option label="總和" value="SUM" />
            <el-option label="平均" value="AVG" />
            <el-option label="計數" value="COUNT" />
            <el-option label="最大值" value="MAX" />
            <el-option label="最小值" value="MIN" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 統計記錄列表 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="StatId" label="統計ID" width="100" />
        <el-table-column prop="ReportCode" label="報表代碼" width="150" />
        <el-table-column prop="ReportName" label="報表名稱" width="200" />
        <el-table-column prop="StatType" label="統計類型" width="100" />
        <el-table-column prop="ChartType" label="圖表類型" width="100" />
        <el-table-column prop="QueriedBy" label="查詢者" width="120" />
        <el-table-column prop="QueriedAt" label="查詢時間" width="180" />
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewResult(row)">查看結果</el-button>
            <el-button type="success" size="small" @click="handleExport(row)">匯出</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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

    <!-- 統計查詢對話框 -->
    <el-dialog
      title="執行統計查詢"
      v-model="statisticsDialogVisible"
      width="800px"
      @close="handleStatisticsDialogClose"
    >
      <el-form :model="statisticsForm" ref="statisticsFormRef" label-width="120px">
        <el-form-item label="報表代碼" prop="ReportCode">
          <el-input v-model="statisticsForm.ReportCode" placeholder="請輸入報表代碼" />
        </el-form-item>
        <el-form-item label="報表名稱" prop="ReportName">
          <el-input v-model="statisticsForm.ReportName" placeholder="請輸入報表名稱" />
        </el-form-item>
        <el-form-item label="統計類型" prop="StatType">
          <el-select v-model="statisticsForm.StatType" placeholder="請選擇統計類型">
            <el-option label="總和" value="SUM" />
            <el-option label="平均" value="AVG" />
            <el-option label="計數" value="COUNT" />
            <el-option label="最大值" value="MAX" />
            <el-option label="最小值" value="MIN" />
          </el-select>
        </el-form-item>
        <el-form-item label="圖表類型" prop="ChartType">
          <el-select v-model="statisticsForm.ChartType" placeholder="請選擇圖表類型">
            <el-option label="柱狀圖" value="BAR" />
            <el-option label="折線圖" value="LINE" />
            <el-option label="圓餅圖" value="PIE" />
          </el-select>
        </el-form-item>
        <el-form-item label="統計參數">
          <el-input
            v-model="statisticsForm.StatParams"
            type="textarea"
            :rows="4"
            placeholder="請輸入統計參數（JSON格式）"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleStatisticsDialogClose">取消</el-button>
        <el-button type="primary" @click="handleExecuteStatistics">執行統計</el-button>
      </template>
    </el-dialog>

    <!-- 統計結果對話框 -->
    <el-dialog
      title="統計結果"
      v-model="resultDialogVisible"
      width="1200px"
    >
      <div v-if="statisticsResult">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="報表代碼">{{ statisticsResult.ReportCode }}</el-descriptions-item>
          <el-descriptions-item label="報表名稱">{{ statisticsResult.ReportName }}</el-descriptions-item>
          <el-descriptions-item label="統計類型">{{ statisticsResult.StatType }}</el-descriptions-item>
          <el-descriptions-item label="圖表類型">{{ statisticsResult.ChartType }}</el-descriptions-item>
        </el-descriptions>
        <div style="margin-top: 20px;">
          <h3>統計結果</h3>
          <el-table
            :data="statisticsResultData"
            border
            stripe
            style="width: 100%"
            max-height="400"
          >
            <el-table-column
              v-for="column in resultColumns"
              :key="column.prop"
              :prop="column.prop"
              :label="column.label"
              :width="column.width"
            />
          </el-table>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { reportExtensionApi } from '@/api/reportExtension'

// 查詢表單
const queryForm = reactive({
  ReportCode: '',
  ReportName: '',
  StatType: ''
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

// 統計對話框
const statisticsDialogVisible = ref(false)
const statisticsForm = reactive({
  ReportCode: '',
  ReportName: '',
  StatType: '',
  ChartType: '',
  StatParams: ''
})
const statisticsFormRef = ref(null)

// 結果對話框
const resultDialogVisible = ref(false)
const statisticsResult = ref(null)
const statisticsResultData = ref([])
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
      StatType: queryForm.StatType || undefined
    }
    const response = await reportExtensionApi.statistics.getStatisticsLogs(params)
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
  queryForm.StatType = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 執行統計
const handleExecuteStatistics = async () => {
  if (!statisticsFormRef.value) return
  await statisticsFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const request = {
          ReportCode: statisticsForm.ReportCode,
          ReportName: statisticsForm.ReportName,
          StatType: statisticsForm.StatType,
          ChartType: statisticsForm.ChartType,
          StatParams: statisticsForm.StatParams ? JSON.parse(statisticsForm.StatParams) : {}
        }
        const response = await reportExtensionApi.statistics.queryStatistics(request)
        if (response.data.success) {
          ElMessage.success('統計查詢成功')
          statisticsDialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '統計查詢失敗')
        }
      } catch (error) {
        ElMessage.error('統計查詢失敗：' + error.message)
      }
    }
  })
}

// 查看結果
const handleViewResult = async (row) => {
  try {
    statisticsResult.value = row
    if (row.StatResult) {
      const result = JSON.parse(row.StatResult)
      statisticsResultData.value = Array.isArray(result) ? result : [result]
      if (statisticsResultData.value.length > 0) {
        resultColumns.value = Object.keys(statisticsResultData.value[0]).map(key => ({
          prop: key,
          label: key,
          width: 150
        }))
      }
    }
    resultDialogVisible.value = true
  } catch (error) {
    ElMessage.error('查看結果失敗：' + error.message)
  }
}

// 匯出
const handleExport = async (row) => {
  try {
    ElMessage.info('匯出功能開發中')
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此統計記錄嗎？', '警告', {
      type: 'warning',
      confirmButtonText: '確定',
      cancelButtonText: '取消'
    })
    ElMessage.info('刪除功能開發中')
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 關閉統計對話框
const handleStatisticsDialogClose = () => {
  statisticsDialogVisible.value = false
  statisticsForm.ReportCode = ''
  statisticsForm.ReportName = ''
  statisticsForm.StatType = ''
  statisticsForm.ChartType = ''
  statisticsForm.StatParams = ''
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

.report-statistics {
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


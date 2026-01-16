<template>
  <div class="system-extension-query">
    <div class="page-header">
      <h1>系統擴展查詢 (SYSX120)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="擴展功能代碼">
          <el-input v-model="queryForm.ExtensionId" placeholder="請輸入擴展功能代碼" clearable />
        </el-form-item>
        <el-form-item label="擴展功能名稱">
          <el-input v-model="queryForm.ExtensionName" placeholder="請輸入擴展功能名稱" clearable />
        </el-form-item>
        <el-form-item label="擴展類型">
          <el-input v-model="queryForm.ExtensionType" placeholder="請輸入擴展類型" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="建立日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport" :loading="exportLoading">匯出</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 統計資訊面板 -->
    <el-card class="statistics-card" shadow="never" v-if="statistics">
      <template #header>
        <span>統計資訊</span>
      </template>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-statistic title="總數" :value="statistics.TotalCount" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="啟用數" :value="statistics.ActiveCount">
            <template #suffix>
              <el-tag type="success" style="margin-left: 8px">啟用</el-tag>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="停用數" :value="statistics.InactiveCount">
            <template #suffix>
              <el-tag type="danger" style="margin-left: 8px">停用</el-tag>
            </template>
          </el-statistic>
        </el-col>
      </el-row>
      <el-divider />
      <div v-if="statistics.ByType && statistics.ByType.length > 0">
        <h4>依類型統計</h4>
        <el-table :data="statistics.ByType" border style="margin-top: 10px">
          <el-table-column prop="ExtensionType" label="擴展類型" />
          <el-table-column prop="Count" label="數量" />
        </el-table>
      </div>
    </el-card>

    <!-- 查詢結果表格 -->
    <el-card class="table-card" shadow="never">
      <div class="table-header">
        <span>查詢結果 (共 {{ pagination.TotalCount }} 筆)</span>
        <el-button type="primary" size="small" @click="handleExport" :loading="exportLoading">匯出</el-button>
      </div>
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="ExtensionId" label="擴展功能代碼" width="150" />
        <el-table-column prop="ExtensionName" label="擴展功能名稱" width="200" />
        <el-table-column prop="ExtensionType" label="擴展類型" width="120" />
        <el-table-column prop="ExtensionValue" label="擴展值" width="200" show-overflow-tooltip />
        <el-table-column prop="SeqNo" label="排序序號" width="100" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
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

    <!-- 查看對話框 -->
    <el-dialog
      title="查看系統擴展"
      v-model="viewDialogVisible"
      width="800px"
    >
      <el-descriptions :column="2" border v-if="viewData">
        <el-descriptions-item label="擴展功能代碼">{{ viewData.ExtensionId }}</el-descriptions-item>
        <el-descriptions-item label="擴展功能名稱">{{ viewData.ExtensionName }}</el-descriptions-item>
        <el-descriptions-item label="擴展類型">{{ viewData.ExtensionType || '-' }}</el-descriptions-item>
        <el-descriptions-item label="擴展值">{{ viewData.ExtensionValue || '-' }}</el-descriptions-item>
        <el-descriptions-item label="排序序號">{{ viewData.SeqNo }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="viewData.Status === '1' ? 'success' : 'danger'">
            {{ viewData.Status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="備註" :span="2">{{ viewData.Notes || '-' }}</el-descriptions-item>
        <el-descriptions-item label="擴展設定" :span="2">
          <pre v-if="viewData.ExtensionConfig" style="max-height: 200px; overflow: auto;">{{ formatJson(viewData.ExtensionConfig) }}</pre>
          <span v-else>-</span>
        </el-descriptions-item>
        <el-descriptions-item label="建立人員">{{ viewData.CreatedBy || '-' }}</el-descriptions-item>
        <el-descriptions-item label="建立時間">{{ formatDateTime(viewData.CreatedAt) }}</el-descriptions-item>
        <el-descriptions-item label="更新人員">{{ viewData.UpdatedBy || '-' }}</el-descriptions-item>
        <el-descriptions-item label="更新時間">{{ formatDateTime(viewData.UpdatedAt) }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="viewDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { systemExtensionApi } from '@/api/systemExtension'

// 查詢表單
const queryForm = reactive({
  ExtensionId: '',
  ExtensionName: '',
  ExtensionType: '',
  Status: '',
  CreatedDateFrom: '',
  CreatedDateTo: ''
})

// 日期範圍
const dateRange = ref(null)

// 表格資料
const tableData = ref([])
const loading = ref(false)
const exportLoading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 統計資訊
const statistics = ref(null)

// 查看對話框
const viewDialogVisible = ref(false)
const viewData = ref(null)

// 日期範圍變更
const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.CreatedDateFrom = dates[0]
    queryForm.CreatedDateTo = dates[1]
  } else {
    queryForm.CreatedDateFrom = ''
    queryForm.CreatedDateTo = ''
  }
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return ''
  const date = new Date(dateTime)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 格式化 JSON
const formatJson = (jsonString) => {
  try {
    const obj = JSON.parse(jsonString)
    return JSON.stringify(obj, null, 2)
  } catch (e) {
    return jsonString
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ExtensionId: queryForm.ExtensionId || undefined,
      ExtensionName: queryForm.ExtensionName || undefined,
      ExtensionType: queryForm.ExtensionType || undefined,
      Status: queryForm.Status || undefined,
      CreatedDateFrom: queryForm.CreatedDateFrom || undefined,
      CreatedDateTo: queryForm.CreatedDateTo || undefined
    }
    const response = await systemExtensionApi.getSystemExtensions(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.TotalCount = response.data.data.totalCount
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
  
  // 查詢統計資訊
  await loadStatistics()
}

// 載入統計資訊
const loadStatistics = async () => {
  try {
    const params = {
      ExtensionType: queryForm.ExtensionType || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await systemExtensionApi.getStatistics(params)
    if (response.data.success) {
      statistics.value = response.data.data
    }
  } catch (error) {
    console.error('載入統計資訊失敗：', error)
  }
}

// 重置
const handleReset = () => {
  queryForm.ExtensionId = ''
  queryForm.ExtensionName = ''
  queryForm.ExtensionType = ''
  queryForm.Status = ''
  queryForm.CreatedDateFrom = ''
  queryForm.CreatedDateTo = ''
  dateRange.value = null
  pagination.PageIndex = 1
  handleSearch()
}

// 匯出
const handleExport = async () => {
  exportLoading.value = true
  try {
    const params = {
      ExtensionId: queryForm.ExtensionId || undefined,
      ExtensionName: queryForm.ExtensionName || undefined,
      ExtensionType: queryForm.ExtensionType || undefined,
      Status: queryForm.Status || undefined,
      CreatedDateFrom: queryForm.CreatedDateFrom || undefined,
      CreatedDateTo: queryForm.CreatedDateTo || undefined
    }
    const response = await systemExtensionApi.exportToExcel(params)
    
    // 創建下載連結
    const blob = new Blob([response.data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `系統擴展查詢_${new Date().toISOString().slice(0, 19).replace(/[:-]/g, '').replace('T', '_')}.xlsx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  } finally {
    exportLoading.value = false
  }
}

// 查看
const handleView = async (row) => {
  try {
    const response = await systemExtensionApi.getSystemExtension(row.TKey)
    if (response.data.success) {
      viewData.value = response.data.data
      viewDialogVisible.value = true
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
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

.system-extension-query {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    
    .search-form {
      margin: 0;
    }
  }

  .statistics-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;

    .table-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 15px;
      font-size: 14px;
      font-weight: 500;
    }
  }
}
</style>

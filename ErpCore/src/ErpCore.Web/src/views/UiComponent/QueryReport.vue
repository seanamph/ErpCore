<template>
  <div class="query-report">
    <div class="page-header">
      <h1>UI組件查詢與報表</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="組件代碼">
          <el-input v-model="queryForm.ComponentCode" placeholder="請輸入組件代碼" clearable />
        </el-form-item>
        <el-form-item label="組件類型">
          <el-select v-model="queryForm.ComponentType" placeholder="請選擇組件類型" clearable>
            <el-option label="FB - 瀏覽" value="FB" />
            <el-option label="FI - 新增" value="FI" />
            <el-option label="FU - 修改" value="FU" />
            <el-option label="FQ - 查詢" value="FQ" />
            <el-option label="PR - 報表" value="PR" />
            <el-option label="FS - 保存" value="FS" />
          </el-select>
        </el-form-item>
        <el-form-item label="模組代碼">
          <el-input v-model="queryForm.ModuleCode" placeholder="請輸入模組代碼" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 使用記錄 -->
    <el-card class="usage-card" shadow="never">
      <template #header>
        <span>UI組件使用記錄</span>
      </template>
      <el-table
        :data="usageList"
        v-loading="usageLoading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="ComponentCode" label="組件代碼" width="150" />
        <el-table-column prop="ComponentName" label="組件名稱" min-width="200" />
        <el-table-column prop="ComponentType" label="組件類型" width="100" />
        <el-table-column prop="ModuleCode" label="模組代碼" width="150" />
        <el-table-column prop="ModuleName" label="模組名稱" min-width="200" />
        <el-table-column prop="UsageCount" label="使用次數" width="100" align="center" />
        <el-table-column prop="LastUsedAt" label="最後使用時間" width="180">
          <template #default="{ row }">
            {{ row.LastUsedAt ? new Date(row.LastUsedAt).toLocaleString('zh-TW') : '' }}
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="usagePagination.PageIndex"
        v-model:page-size="usagePagination.PageSize"
        :total="usagePagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleUsageSizeChange"
        @current-change="handleUsagePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>

    <!-- 報表統計 -->
    <el-card class="report-card" shadow="never">
      <template #header>
        <span>UI組件報表統計</span>
      </template>
      <el-table
        :data="reportList"
        v-loading="reportLoading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="ComponentType" label="組件類型" width="120" />
        <el-table-column prop="TotalCount" label="總數量" width="100" align="center" />
        <el-table-column prop="ActiveCount" label="啟用數量" width="100" align="center" />
        <el-table-column prop="InactiveCount" label="停用數量" width="100" align="center" />
        <el-table-column prop="TotalUsageCount" label="總使用次數" width="120" align="center" />
        <el-table-column prop="AvgUsageCount" label="平均使用次數" width="120" align="center" />
      </el-table>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { uiComponentApi } from '@/api/uiComponent'

// 查詢表單
const queryForm = reactive({
  ComponentCode: '',
  ComponentType: '',
  ModuleCode: ''
})

// 使用記錄
const usageList = ref([])
const usageLoading = ref(false)
const usagePagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 報表統計
const reportList = ref([])
const reportLoading = ref(false)

// 載入使用記錄
const loadUsageList = async () => {
  usageLoading.value = true
  try {
    const params = {
      PageIndex: usagePagination.PageIndex,
      PageSize: usagePagination.PageSize,
      ComponentCode: queryForm.ComponentCode || undefined,
      ComponentType: queryForm.ComponentType || undefined,
      ModuleCode: queryForm.ModuleCode || undefined
    }
    const response = await uiComponentApi.getComponentUsages(params)
    if (response.data?.success) {
      usageList.value = response.data.data?.Items || []
      usagePagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    usageLoading.value = false
  }
}

// 載入報表統計
const loadReport = async () => {
  reportLoading.value = true
  try {
    const params = {
      ComponentCode: queryForm.ComponentCode || undefined,
      ComponentType: queryForm.ComponentType || undefined
    }
    const response = await uiComponentApi.getComponentReport(params)
    if (response.data?.success) {
      reportList.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    reportLoading.value = false
  }
}

// 查詢
const handleSearch = () => {
  usagePagination.PageIndex = 1
  loadUsageList()
  loadReport()
}

// 重置
const handleReset = () => {
  queryForm.ComponentCode = ''
  queryForm.ComponentType = ''
  queryForm.ModuleCode = ''
  handleSearch()
}

// 使用記錄分頁大小變更
const handleUsageSizeChange = (size) => {
  usagePagination.PageSize = size
  usagePagination.PageIndex = 1
  loadUsageList()
}

// 使用記錄分頁變更
const handleUsagePageChange = (page) => {
  usagePagination.PageIndex = page
  loadUsageList()
}

// 初始化
onMounted(() => {
  loadUsageList()
  loadReport()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.query-report {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 500;
}

.search-card,
.usage-card,
.report-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}
</style>


<template>
  <div class="date-list">
    <div class="page-header">
      <h1>日期列表 (DATE_LIST)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="開始日期">
          <el-date-picker
            v-model="queryForm.StartDate"
            type="date"
            placeholder="請選擇開始日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="結束日期">
          <el-date-picker
            v-model="queryForm.EndDate"
            type="date"
            placeholder="請選擇結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="日期類型">
          <el-select v-model="queryForm.DateType" placeholder="請選擇日期類型" clearable>
            <el-option label="工作日" value="Workday" />
            <el-option label="假日" value="Holiday" />
            <el-option label="全部" value="" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 日期列表 -->
      <el-table
        :data="dateList"
        v-loading="loading"
        border
        stripe
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%; cursor: pointer"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="Date" label="日期" width="120" align="center">
          <template #default="{ row }">
            {{ row.Date ? new Date(row.Date).toLocaleDateString('zh-TW') : '' }}
          </template>
        </el-table-column>
        <el-table-column prop="DateType" label="日期類型" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="row.DateType === 'Workday' ? 'success' : 'warning'" size="small">
              {{ row.DateType === 'Workday' ? '工作日' : '假日' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Description" label="說明" min-width="200" />
        <el-table-column prop="IsHoliday" label="是否假日" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.IsHoliday ? 'warning' : 'success'" size="small">
              {{ row.IsHoliday ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[20, 50, 100, 200]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

// 查詢表單
const queryForm = reactive({
  StartDate: '',
  EndDate: '',
  DateType: ''
})

// 日期列表
const dateList = ref([])
const loading = ref(false)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      StartDate: queryForm.StartDate || undefined,
      EndDate: queryForm.EndDate || undefined,
      DateType: queryForm.DateType || undefined
    }
    const response = await dropdownListApi.getDates(params)
    if (response.data?.success) {
      dateList.value = response.data.data?.Items || []
      pagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
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
  queryForm.StartDate = ''
  queryForm.EndDate = ''
  queryForm.DateType = ''
  handleSearch()
}

// 行點擊
const handleRowClick = (row) => {
  ElMessage.success(`已選擇日期：${new Date(row.Date).toLocaleDateString('zh-TW')}`)
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
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.date-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}
</style>


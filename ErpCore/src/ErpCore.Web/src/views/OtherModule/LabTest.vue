<template>
  <div class="lab-test">
    <div class="page-header">
      <h1>實驗室測試功能 (Lab)</h1>
    </div>

    <!-- 測試表單 -->
    <el-card class="form-card" shadow="never">
      <el-form :model="testForm" ref="testFormRef" label-width="120px">
        <el-form-item label="測試類型" prop="TestType">
          <el-select v-model="testForm.TestType" placeholder="請選擇測試類型">
            <el-option label="單元測試" value="UnitTest" />
            <el-option label="整合測試" value="IntegrationTest" />
            <el-option label="效能測試" value="PerformanceTest" />
            <el-option label="壓力測試" value="StressTest" />
          </el-select>
        </el-form-item>
        <el-form-item label="測試參數" prop="TestParams">
          <el-input
            v-model="testForm.TestParams"
            type="textarea"
            :rows="6"
            placeholder="請輸入測試參數（JSON格式）"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleExecuteTest">執行測試</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 測試記錄列表 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="LogId" label="記錄ID" width="100" />
        <el-table-column prop="TestType" label="測試類型" width="150" />
        <el-table-column prop="Status" label="狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Result" label="測試結果" min-width="200" show-overflow-tooltip />
        <el-table-column prop="ExecutionTime" label="執行時間(ms)" width="150" />
        <el-table-column prop="CreatedAt" label="建立時間" width="180" />
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewDetail(row)">查看詳情</el-button>
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { otherModuleApi } from '@/api/otherModule'

// 測試表單
const testForm = reactive({
  TestType: '',
  TestParams: ''
})
const testFormRef = ref(null)

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 執行測試
const handleExecuteTest = async () => {
  if (!testFormRef.value) return
  await testFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const data = {
          TestType: testForm.TestType,
          TestParams: testForm.TestParams ? JSON.parse(testForm.TestParams) : {}
        }
        const response = await otherModuleApi.labTest.executeTest(data)
        if (response.data.success) {
          ElMessage.success('測試任務已提交')
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '測試失敗')
        }
      } catch (error) {
        ElMessage.error('測試失敗：' + error.message)
      }
    }
  })
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize
    }
    const response = await otherModuleApi.labTest.getTestLogs(params)
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

// 查看詳情
const handleViewDetail = (row) => {
  ElMessage.info('測試結果：' + row.Result)
}

// 取得狀態類型
const getStatusType = (status) => {
  const statusMap = {
    SUCCESS: 'success',
    FAILED: 'danger',
    PROCESSING: 'warning'
  }
  return statusMap[status] || 'info'
}

// 取得狀態文字
const getStatusText = (status) => {
  const statusMap = {
    SUCCESS: '成功',
    FAILED: '失敗',
    PROCESSING: '處理中'
  }
  return statusMap[status] || status
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

.lab-test {
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

  .form-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>


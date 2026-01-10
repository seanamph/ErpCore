<template>
  <div class="eip-integration">
    <div class="page-header">
      <h1>EIP系統整合 (IMS2EIP)</h1>
    </div>

    <!-- 整合表單 -->
    <el-card class="form-card" shadow="never">
      <el-form :model="integrationForm" ref="integrationFormRef" label-width="120px">
        <el-form-item label="整合類型" prop="IntegrationType">
          <el-select v-model="integrationForm.IntegrationType" placeholder="請選擇整合類型">
            <el-option label="資料同步" value="DataSync" />
            <el-option label="單據傳送" value="DocumentTransfer" />
            <el-option label="報表整合" value="ReportIntegration" />
          </el-select>
        </el-form-item>
        <el-form-item label="整合參數" prop="IntegrationParams">
          <el-input
            v-model="integrationForm.IntegrationParams"
            type="textarea"
            :rows="6"
            placeholder="請輸入整合參數（JSON格式）"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleExecuteIntegration">執行整合</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 整合記錄列表 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="LogId" label="記錄ID" width="100" />
        <el-table-column prop="IntegrationType" label="整合類型" width="150" />
        <el-table-column prop="Status" label="狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Message" label="訊息" min-width="200" show-overflow-tooltip />
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

// 整合表單
const integrationForm = reactive({
  IntegrationType: '',
  IntegrationParams: ''
})
const integrationFormRef = ref(null)

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 執行整合
const handleExecuteIntegration = async () => {
  if (!integrationFormRef.value) return
  await integrationFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const data = {
          IntegrationType: integrationForm.IntegrationType,
          IntegrationParams: integrationForm.IntegrationParams ? JSON.parse(integrationForm.IntegrationParams) : {}
        }
        const response = await otherModuleApi.eipIntegration.executeIntegration(data)
        if (response.data.success) {
          ElMessage.success('整合任務已提交')
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '整合失敗')
        }
      } catch (error) {
        ElMessage.error('整合失敗：' + error.message)
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
    const response = await otherModuleApi.eipIntegration.getIntegrationLogs(params)
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
  ElMessage.info('詳情：' + row.Message)
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

.eip-integration {
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


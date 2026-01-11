<template>
  <div class="auto-process-mail">
    <div class="page-header">
      <h1>自動處理郵件作業 (AutoProcessMail)</h1>
    </div>

    <!-- 操作區域 -->
    <el-card class="operation-card" shadow="never">
      <el-form :inline="true" :model="processForm" class="search-form">
        <el-form-item label="批次大小">
          <el-input-number v-model="processForm.BatchSize" :min="1" :max="1000" :step="10" />
        </el-form-item>
        <el-form-item label="最大重試次數">
          <el-input-number v-model="processForm.MaxRetryCount" :min="1" :max="10" :step="1" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleProcessQueue" :loading="processing">
            處理郵件佇列
          </el-button>
          <el-button type="warning" @click="handleRetryFailed" :loading="retrying">
            重試失敗郵件
          </el-button>
          <el-button @click="handleRefreshStatus">重新整理狀態</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 狀態統計 -->
    <el-card class="status-card" shadow="never">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>郵件佇列狀態</span>
          <el-button type="text" @click="handleRefreshStatus" :loading="statusLoading">
            重新整理
          </el-button>
        </div>
      </template>
      <el-row :gutter="20" v-loading="statusLoading">
        <el-col :span="6">
          <el-statistic title="待處理" :value="queueStatus.PendingCount || 0">
            <template #prefix>
              <el-icon><Clock /></el-icon>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="處理中" :value="queueStatus.ProcessingCount || 0">
            <template #prefix>
              <el-icon><Loading /></el-icon>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="已發送" :value="queueStatus.SentCount || 0">
            <template #prefix>
              <el-icon><CircleCheck /></el-icon>
            </template>
          </el-statistic>
        </el-col>
        <el-col :span="6">
          <el-statistic title="失敗" :value="queueStatus.FailedCount || 0">
            <template #prefix>
              <el-icon><CircleClose /></el-icon>
            </template>
          </el-statistic>
        </el-col>
      </el-row>
    </el-card>

    <!-- 處理結果 -->
    <el-card v-if="processResult" class="result-card" shadow="never">
      <template #header>
        <span>處理結果</span>
      </template>
      <el-descriptions :column="2" border>
        <el-descriptions-item label="處理數量">
          {{ processResult.ProcessedCount || 0 }}
        </el-descriptions-item>
        <el-descriptions-item label="成功數量">
          <el-tag type="success">{{ processResult.SuccessCount || 0 }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="失敗數量">
          <el-tag type="danger">{{ processResult.FailedCount || 0 }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="重試數量">
          <el-tag type="warning">{{ processResult.RetryCount || 0 }}</el-tag>
        </el-descriptions-item>
      </el-descriptions>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Clock, Loading, CircleCheck, CircleClose } from '@element-plus/icons-vue'
import { communicationApi } from '@/api/communication'

// 處理表單
const processForm = reactive({
  BatchSize: 100,
  MaxRetryCount: 3
})

// 處理中狀態
const processing = ref(false)
const retrying = ref(false)
const statusLoading = ref(false)

// 佇列狀態
const queueStatus = reactive({
  PendingCount: 0,
  ProcessingCount: 0,
  SentCount: 0,
  FailedCount: 0,
  TotalCount: 0
})

// 處理結果
const processResult = ref(null)

// 載入狀態
const loadStatus = async () => {
  statusLoading.value = true
  try {
    const response = await communicationApi.getEmailQueueStatus()
    if (response.data?.success) {
      Object.assign(queueStatus, response.data.data || {})
    } else {
      ElMessage.error(response.data?.message || '查詢狀態失敗')
    }
  } catch (error) {
    ElMessage.error('查詢狀態失敗：' + (error.message || '未知錯誤'))
  } finally {
    statusLoading.value = false
  }
}

// 處理郵件佇列
const handleProcessQueue = async () => {
  processing.value = true
  try {
    const response = await communicationApi.processEmailQueue({
      BatchSize: processForm.BatchSize,
      MaxRetryCount: processForm.MaxRetryCount
    })
    if (response.data?.success) {
      processResult.value = response.data.data
      ElMessage.success('處理完成')
      await loadStatus()
    } else {
      ElMessage.error(response.data?.message || '處理失敗')
    }
  } catch (error) {
    ElMessage.error('處理失敗：' + (error.message || '未知錯誤'))
  } finally {
    processing.value = false
  }
}

// 重試失敗郵件
const handleRetryFailed = async () => {
  retrying.value = true
  try {
    const response = await communicationApi.retryFailedEmails({
      MaxRetryCount: processForm.MaxRetryCount
    })
    if (response.data?.success) {
      processResult.value = response.data.data
      ElMessage.success('重試完成')
      await loadStatus()
    } else {
      ElMessage.error(response.data?.message || '重試失敗')
    }
  } catch (error) {
    ElMessage.error('重試失敗：' + (error.message || '未知錯誤'))
  } finally {
    retrying.value = false
  }
}

// 重新整理狀態
const handleRefreshStatus = () => {
  loadStatus()
}

// 初始化
onMounted(() => {
  loadStatus()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.auto-process-mail {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $primary-color;
      margin: 0;
    }
  }

  .operation-card,
  .status-card,
  .result-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .search-form {
    margin: 0;
  }
}
</style>


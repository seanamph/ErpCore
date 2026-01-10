<template>
  <div class="pos-sync">
    <div class="page-header">
      <h1>POS資料同步作業</h1>
    </div>

    <!-- 同步操作 -->
    <el-card class="action-card" shadow="never">
      <el-form :inline="true" :model="syncForm" class="sync-form">
        <el-form-item label="同步類型">
          <el-select 
            v-model="syncForm.SyncType" 
            placeholder="請選擇同步類型" 
            style="width: 200px"
          >
            <el-option label="交易資料" value="Transaction" />
            <el-option label="商品資料" value="Product" />
            <el-option label="庫存資料" value="Inventory" />
          </el-select>
        </el-form-item>
        <el-form-item label="同步方向">
          <el-select 
            v-model="syncForm.SyncDirection" 
            placeholder="請選擇同步方向" 
            style="width: 200px"
          >
            <el-option label="POS → IMS" value="ToIMS" />
            <el-option label="IMS → POS" value="FromPOS" />
          </el-select>
        </el-form-item>
        <el-form-item label="店別">
          <el-select 
            v-model="syncForm.StoreId" 
            placeholder="請選擇店別" 
            clearable
            filterable
            style="width: 200px"
          >
            <el-option
              v-for="item in storeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="日期區間">
          <el-date-picker
            v-model="syncForm.DateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item>
          <el-button 
            type="primary" 
            icon="Refresh" 
            @click="handleSync"
            :loading="syncing"
          >
            開始同步
          </el-button>
          <el-button icon="Search" @click="handleQueryStatus">查詢狀態</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 同步狀態 -->
    <el-card v-if="syncStatus" class="status-card" shadow="never">
      <el-descriptions title="同步狀態" :column="3" border>
        <el-descriptions-item label="同步類型">{{ syncStatus.SyncType }}</el-descriptions-item>
        <el-descriptions-item label="同步方向">{{ syncStatus.SyncDirection }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="getStatusTag(syncStatus.Status)">
            {{ getStatusText(syncStatus.Status) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="記錄筆數">{{ syncStatus.RecordCount || 0 }}</el-descriptions-item>
        <el-descriptions-item label="成功筆數">
          <span style="color: var(--success-color)">{{ syncStatus.SuccessCount || 0 }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="失敗筆數">
          <span style="color: var(--danger-color)">{{ syncStatus.FailedCount || 0 }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="開始時間">{{ formatDateTime(syncStatus.StartTime) }}</el-descriptions-item>
        <el-descriptions-item label="結束時間">{{ formatDateTime(syncStatus.EndTime) }}</el-descriptions-item>
        <el-descriptions-item label="執行時間">
          {{ syncStatus.EndTime ? calculateDuration(syncStatus.StartTime, syncStatus.EndTime) : '進行中...' }}
        </el-descriptions-item>
      </el-descriptions>
      <el-alert
        v-if="syncStatus.ErrorMessage"
        :title="syncStatus.ErrorMessage"
        type="error"
        :closable="false"
        style="margin-top: 16px"
      />
    </el-card>

    <!-- 同步記錄列表 -->
    <el-card class="table-card" shadow="never">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>同步記錄</span>
          <el-button icon="Refresh" @click="handleRefresh">刷新</el-button>
        </div>
      </template>
      
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="SyncType" label="同步類型" width="120" />
        <el-table-column prop="SyncDirection" label="同步方向" width="120" />
        <el-table-column prop="RecordCount" label="記錄筆數" width="100" align="right" />
        <el-table-column prop="SuccessCount" label="成功筆數" width="100" align="right">
          <template #default="{ row }">
            <span style="color: var(--success-color)">{{ row.SuccessCount }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="FailedCount" label="失敗筆數" width="100" align="right">
          <template #default="{ row }">
            <span style="color: var(--danger-color)">{{ row.FailedCount }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusTag(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="StartTime" label="開始時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.StartTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="EndTime" label="結束時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.EndTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleViewDetail(row)">
              查看詳情
            </el-button>
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

    <!-- 詳情對話框 -->
    <el-dialog 
      v-model="detailDialogVisible" 
      title="同步記錄詳情" 
      width="800px"
    >
      <el-descriptions :column="2" border>
        <el-descriptions-item label="同步類型">{{ currentLog?.SyncType }}</el-descriptions-item>
        <el-descriptions-item label="同步方向">{{ currentLog?.SyncDirection }}</el-descriptions-item>
        <el-descriptions-item label="記錄筆數">{{ currentLog?.RecordCount }}</el-descriptions-item>
        <el-descriptions-item label="成功筆數">{{ currentLog?.SuccessCount }}</el-descriptions-item>
        <el-descriptions-item label="失敗筆數">{{ currentLog?.FailedCount }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="getStatusTag(currentLog?.Status)">
            {{ getStatusText(currentLog?.Status) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="開始時間" :span="2">
          {{ formatDateTime(currentLog?.StartTime) }}
        </el-descriptions-item>
        <el-descriptions-item label="結束時間" :span="2">
          {{ formatDateTime(currentLog?.EndTime) }}
        </el-descriptions-item>
        <el-descriptions-item v-if="currentLog?.ErrorMessage" label="錯誤訊息" :span="2">
          <el-alert
            :title="currentLog.ErrorMessage"
            type="error"
            :closable="false"
          />
        </el-descriptions-item>
      </el-descriptions>
      
      <template #footer>
        <el-button @click="detailDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { posApi } from '@/api/pos'
import { warehousesApi } from '@/api/warehouses'

// 同步表單
const syncForm = reactive({
  SyncType: 'Transaction',
  SyncDirection: 'ToIMS',
  StoreId: '',
  DateRange: []
})

// 表格資料
const tableData = ref([])
const loading = ref(false)
const syncing = ref(false)
const syncStatus = ref(null)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 詳情對話框
const detailDialogVisible = ref(false)
const currentLog = ref(null)

// 店別選項
const storeOptions = ref([])

// 開始同步
const handleSync = async () => {
  if (!syncForm.SyncType) {
    ElMessage.warning('請選擇同步類型')
    return
  }

  if (!syncForm.DateRange || syncForm.DateRange.length !== 2) {
    ElMessage.warning('請選擇日期區間')
    return
  }

  try {
    syncing.value = true
    const data = {
      SyncType: syncForm.SyncType,
      SyncDirection: syncForm.SyncDirection,
      StoreId: syncForm.StoreId || undefined,
      StartDate: syncForm.DateRange[0],
      EndDate: syncForm.DateRange[1]
    }
    
    const response = await posApi.syncTransactions(data)
    if (response.data.success) {
      ElMessage.success('同步作業已啟動')
      // 延遲查詢狀態
      setTimeout(() => {
        handleQueryStatus()
        handleRefresh()
      }, 2000)
    } else {
      ElMessage.error(response.data.message || '同步失敗')
    }
  } catch (error) {
    ElMessage.error('同步失敗：' + (error.message || '未知錯誤'))
  } finally {
    syncing.value = false
  }
}

// 查詢狀態
const handleQueryStatus = async () => {
  try {
    const response = await posApi.getSyncStatus()
    if (response.data.success) {
      syncStatus.value = response.data.data
    }
  } catch (error) {
    console.error('查詢狀態失敗：', error)
  }
}

// 刷新列表
const handleRefresh = async () => {
  try {
    loading.value = true
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: 'CreatedAt',
      SortOrder: 'DESC'
    }
    
    const response = await posApi.getSyncLogs(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 查看詳情
const handleViewDetail = (row) => {
  currentLog.value = row
  detailDialogVisible.value = true
}

// 分頁變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleRefresh()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleRefresh()
}

// 狀態標籤
const getStatusTag = (status) => {
  const statusMap = {
    'Running': 'warning',
    'Completed': 'success',
    'Failed': 'danger'
  }
  return statusMap[status] || 'info'
}

// 狀態文字
const getStatusText = (status) => {
  const statusMap = {
    'Running': '執行中',
    'Completed': '已完成',
    'Failed': '失敗'
  }
  return statusMap[status] || status
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

// 計算執行時間
const calculateDuration = (startTime, endTime) => {
  if (!startTime || !endTime) return ''
  const start = new Date(startTime)
  const end = new Date(endTime)
  const diff = Math.floor((end - start) / 1000) // 秒
  const hours = Math.floor(diff / 3600)
  const minutes = Math.floor((diff % 3600) / 60)
  const seconds = diff % 60
  return `${hours}時${minutes}分${seconds}秒`
}

// 載入店別選項
const loadStoreOptions = async () => {
  try {
    const response = await warehousesApi.getWarehouses({ PageSize: 1000 })
    if (response.data.success) {
      storeOptions.value = (response.data.data.items || []).map(item => ({
        value: item.WarehouseId,
        label: item.WarehouseName
      }))
    }
  } catch (error) {
    console.error('載入店別選項失敗：', error)
  }
}

// 初始化
onMounted(() => {
  loadStoreOptions()
  handleRefresh()
  handleQueryStatus()
})
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.pos-sync {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: var(--text-color-primary);
    }
  }

  .action-card {
    margin-bottom: 20px;
  }

  .status-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>


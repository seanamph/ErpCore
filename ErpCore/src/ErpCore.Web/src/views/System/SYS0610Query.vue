<template>
  <div class="sys0610-query">
    <div class="page-header">
      <h1>使用者基本資料異動查詢 (SYS0610)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" :rules="rules" ref="queryFormRef" class="search-form">
        <!-- 異動使用者代碼 -->
        <el-form-item label="異動使用者代碼" prop="ChangeUserId">
          <el-input 
            v-model="queryForm.ChangeUserId" 
            placeholder="請輸入異動使用者代碼"
            clearable
            style="width: 200px"
          />
          <el-button 
            type="primary" 
            icon="Search" 
            @click="openUserListDialog('changeUser')"
            style="margin-left: 10px"
          >
            選擇
          </el-button>
          <el-input 
            v-model="changeUserName" 
            readonly 
            style="width: 200px; margin-left: 10px"
            placeholder="異動使用者名稱"
          />
        </el-form-item>
        
        <!-- 使用者代碼 (被異動的使用者) -->
        <el-form-item label="使用者代碼" prop="TargetUserId">
          <el-input 
            v-model="queryForm.TargetUserId" 
            placeholder="請輸入使用者代碼"
            clearable
            style="width: 200px"
          />
          <el-button 
            type="primary" 
            icon="Search" 
            @click="openUserListDialog('targetUser')"
            style="margin-left: 10px"
          >
            選擇
          </el-button>
          <el-input 
            v-model="targetUserName" 
            readonly 
            style="width: 200px; margin-left: 10px"
            placeholder="使用者名稱"
          />
        </el-form-item>
        
        <!-- 日期範圍 -->
        <el-form-item label="日期範圍" prop="dateRange" required>
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="~"
            start-placeholder="起始日期"
            end-placeholder="結束日期"
            format="YYYY/MM/DD"
            value-format="YYYY-MM-DD"
            style="width: 300px"
          />
        </el-form-item>
        
        <!-- 查詢按鈕 -->
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleQuery">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 查詢結果 -->
    <el-card class="result-card" shadow="never" v-if="hasSearched">
      <template #header>
        <div class="header-content">
          <span>查詢結果</span>
          <div class="header-info">
            <span v-if="queryForm.ChangeUserId">
              異動使用者代碼: <strong>{{ queryForm.ChangeUserId }}</strong>
              <span v-if="changeUserName">，異動使用者名稱: <strong>{{ changeUserName }}</strong></span>
            </span>
            <span v-if="dateRange && dateRange.length === 2">
              日期範圍: <strong>{{ dateRange[0] }} ~ {{ dateRange[1] }}</strong>
            </span>
          </div>
        </div>
      </template>
      
      <!-- 查詢結果統計 -->
      <div class="result-summary">
        <el-alert 
          :title="`符合條件者共 ${pagination.TotalCount} 筆`" 
          type="info" 
          :closable="false"
        />
      </div>
      
      <!-- 查詢結果列表 -->
      <div class="result-list" v-loading="loading">
        <el-collapse v-model="activeNames">
          <el-collapse-item 
            v-for="(item, index) in changeLogList" 
            :key="item.LogId"
            :name="index.toString()"
          >
            <template #title>
              <div class="collapse-title">
                <span class="seq-no">序號: {{ index + 1 }}</span>
                <span class="change-user">異動者代碼: <strong>{{ item.ChangeUserId || '-' }}</strong></span>
                <span class="change-user-name" v-if="item.ChangeUserName">異動者名稱: <strong>{{ item.ChangeUserName }}</strong></span>
                <span class="change-date">異動時間: <strong>{{ formatDateTime(item.ChangeDate) }}</strong></span>
                <span class="change-status">異動狀態: <strong>{{ item.ChangeStatusName }}</strong></span>
              </div>
            </template>
            
            <!-- 異動詳細資訊 -->
            <div class="change-detail">
              <el-table :data="detailTableData(item)" border style="width: 100%">
                <el-table-column prop="field" label="異動欄位" width="200" />
                <el-table-column prop="oldValue" label="異動前的值" />
                <el-table-column prop="newValue" label="異動後的值" />
              </el-table>
            </div>
          </el-collapse-item>
        </el-collapse>
      </div>
      
      <!-- 分頁 -->
      <div class="pagination">
        <el-pagination
          v-model:current-page="pagination.PageIndex"
          v-model:page-size="pagination.PageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.TotalCount"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 使用者選擇對話框 -->
    <MultiUserListDialog
      v-model="userListDialogVisible"
      :multiple="false"
      @confirm="handleUserSelected"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { changeLogsApi } from '@/api/changeLogs'
import MultiUserListDialog from '@/components/MultiUserListDialog.vue'
import dayjs from 'dayjs'

// 查詢表單
const queryFormRef = ref(null)
const queryForm = reactive({
  ChangeUserId: '',
  TargetUserId: '',
  BeginDate: '',
  EndDate: ''
})

// 日期範圍
const dateRange = ref([])

// 使用者名稱
const changeUserName = ref('')
const targetUserName = ref('')

// 查詢結果
const changeLogList = ref([])
const loading = ref(false)
const hasSearched = ref(false)
const activeNames = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 使用者選擇對話框
const userListDialogVisible = ref(false)
const currentUserType = ref('') // 'changeUser' or 'targetUser'

// 表單驗證規則
const rules = {
  dateRange: [
    { required: true, message: '請選擇日期範圍', trigger: 'change' }
  ]
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return '--'
  return dayjs(dateTime).format('YYYY/MM/DD HH:mm:ss')
}

// 將異動記錄轉換為表格資料
const detailTableData = (item) => {
  const fields = item.ChangeFields || []
  const oldValues = item.OldValues || []
  const newValues = item.NewValues || []
  
  return fields.map((field, index) => ({
    field: field,
    oldValue: oldValues[index] || '--',
    newValue: newValues[index] || '--'
  }))
}

// 打開使用者列表對話框
const openUserListDialog = (type) => {
  currentUserType.value = type
  userListDialogVisible.value = true
}

// 處理使用者選擇
const handleUserSelected = (users) => {
  if (users && users.length > 0) {
    const user = users[0]
    if (currentUserType.value === 'changeUser') {
      queryForm.ChangeUserId = user.UserId
      changeUserName.value = user.UserName || ''
    } else if (currentUserType.value === 'targetUser') {
      queryForm.TargetUserId = user.UserId
      targetUserName.value = user.UserName || ''
    }
  }
  userListDialogVisible.value = false
}

// 查詢
const handleQuery = async () => {
  if (!queryFormRef.value) return
  
  await queryFormRef.value.validate(async (valid) => {
    if (!valid) {
      ElMessage.warning('請填寫完整的查詢條件')
      return
    }
    
    if (!dateRange.value || dateRange.value.length !== 2) {
      ElMessage.warning('請選擇日期範圍')
      return
    }
    
    loading.value = true
    hasSearched.value = true
    
    try {
      const queryData = {
        ChangeUserId: queryForm.ChangeUserId || undefined,
        TargetUserId: queryForm.TargetUserId || undefined,
        BeginDate: dateRange.value[0],
        EndDate: dateRange.value[1],
        PageIndex: pagination.PageIndex,
        PageSize: pagination.PageSize
      }
      
      const response = await changeLogsApi.getUserChangeLogs(queryData)
      
      if (response.data?.Success) {
        changeLogList.value = response.data.Data?.Items || []
        pagination.TotalCount = response.data.Data?.TotalCount || 0
        
        // 展開所有項目
        activeNames.value = changeLogList.value.map((_, index) => index.toString())
        
        if (changeLogList.value.length === 0) {
          ElMessage.info('查無符合條件的資料')
        }
      } else {
        ElMessage.error(response.data?.Message || '查詢失敗')
      }
    } catch (error) {
      console.error('查詢使用者異動記錄失敗:', error)
      ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
    } finally {
      loading.value = false
    }
  })
}

// 重置
const handleReset = () => {
  queryForm.ChangeUserId = ''
  queryForm.TargetUserId = ''
  dateRange.value = []
  changeUserName.value = ''
  targetUserName.value = ''
  changeLogList.value = []
  activeNames.value = []
  hasSearched.value = false
  pagination.PageIndex = 1
  pagination.TotalCount = 0
  
  if (queryFormRef.value) {
    queryFormRef.value.clearValidate()
  }
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleQuery()
}

// 頁碼變更
const handleCurrentChange = (page) => {
  pagination.PageIndex = page
  handleQuery()
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sys0610-query {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
  
  h1 {
    color: $primary-color;
    font-size: 24px;
    font-weight: bold;
  }
}

.search-card {
  margin-bottom: 20px;
}

.result-card {
  margin-top: 20px;
  
  .header-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    
    .header-info {
      font-size: 14px;
      color: #666;
      
      span {
        margin-right: 20px;
      }
    }
  }
}

.result-summary {
  margin-bottom: 20px;
}

.result-list {
  margin-bottom: 20px;
  
  .collapse-title {
    display: flex;
    gap: 20px;
    flex-wrap: wrap;
    
    .seq-no {
      font-weight: bold;
      color: $primary-color;
    }
    
    .change-user,
    .change-user-name,
    .change-date,
    .change-status {
      color: #666;
    }
  }
  
  .change-detail {
    margin-top: 10px;
  }
}

.pagination {
  margin-top: 20px;
  text-align: right;
}
</style>

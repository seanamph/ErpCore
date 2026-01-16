<template>
  <div class="sys0630-query">
    <div class="page-header">
      <h1>使用者角色對應設定異動查詢 (SYS0630)</h1>
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
        
        <!-- 被異動使用者代碼 -->
        <el-form-item label="被異動使用者代碼" prop="SearchUserId">
          <el-input 
            v-model="queryForm.SearchUserId" 
            placeholder="請輸入被異動使用者代碼"
            clearable
            style="width: 200px"
          />
          <el-button 
            type="primary" 
            icon="Search" 
            @click="openUserListDialog('searchUser')"
            style="margin-left: 10px"
          >
            選擇
          </el-button>
        </el-form-item>
        
        <!-- 被異動角色代碼 -->
        <el-form-item label="被異動角色代碼" prop="SearchRoleId">
          <el-input 
            v-model="queryForm.SearchRoleId" 
            placeholder="請輸入被異動角色代碼"
            clearable
            style="width: 200px"
          />
          <el-button 
            type="primary" 
            icon="Search" 
            @click="openRoleListDialog()"
            style="margin-left: 10px"
          >
            選擇
          </el-button>
          <el-input 
            v-model="roleName" 
            readonly 
            style="width: 200px; margin-left: 10px"
            placeholder="角色名稱"
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
            <span v-if="queryForm.SearchUserId">
              被異動使用者代碼: <strong>{{ queryForm.SearchUserId }}</strong>
            </span>
            <span v-if="queryForm.SearchRoleId">
              被異動角色代碼: <strong>{{ queryForm.SearchRoleId }}</strong>
              <span v-if="roleName">，角色名稱: <strong>{{ roleName }}</strong></span>
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
                <el-table-column label="異動前的值" min-width="300">
                  <template #default="{ row }">
                    <div v-if="row.oldValueDisplayObj">
                      <div v-if="row.oldValueDisplayObj.UserId" class="value-display">
                        <strong>使用者:</strong> {{ row.oldValueDisplayObj.UserId }}
                        <span v-if="row.oldValueDisplayObj.UserName"> - {{ row.oldValueDisplayObj.UserName }}</span>
                      </div>
                      <div v-if="row.oldValueDisplayObj.RoleId" class="value-display">
                        <strong>角色:</strong> {{ row.oldValueDisplayObj.RoleId }}
                        <span v-if="row.oldValueDisplayObj.RoleName"> - {{ row.oldValueDisplayObj.RoleName }}</span>
                      </div>
                      <div v-if="!row.oldValueDisplayObj.UserId && !row.oldValueDisplayObj.RoleId">
                        {{ row.oldValue || '(無)' }}
                      </div>
                    </div>
                    <span v-else>{{ row.oldValue || '(無)' }}</span>
                  </template>
                </el-table-column>
                <el-table-column label="異動後的值" min-width="300">
                  <template #default="{ row }">
                    <div v-if="row.newValueDisplayObj">
                      <div v-if="row.newValueDisplayObj.UserId" class="value-display">
                        <strong>使用者:</strong> {{ row.newValueDisplayObj.UserId }}
                        <span v-if="row.newValueDisplayObj.UserName"> - {{ row.newValueDisplayObj.UserName }}</span>
                      </div>
                      <div v-if="row.newValueDisplayObj.RoleId" class="value-display">
                        <strong>角色:</strong> {{ row.newValueDisplayObj.RoleId }}
                        <span v-if="row.newValueDisplayObj.RoleName"> - {{ row.newValueDisplayObj.RoleName }}</span>
                      </div>
                      <div v-if="!row.newValueDisplayObj.UserId && !row.newValueDisplayObj.RoleId">
                        {{ row.newValue || '(無)' }}
                      </div>
                    </div>
                    <span v-else>{{ row.newValue || '(無)' }}</span>
                  </template>
                </el-table-column>
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

    <!-- 角色選擇對話框 -->
    <RoleListDialog
      v-model="roleListDialogVisible"
      :multiple="false"
      @confirm="handleRoleSelected"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { changeLogsApi } from '@/api/changeLogs'
import MultiUserListDialog from '@/components/MultiUserListDialog.vue'
import RoleListDialog from '@/components/RoleListDialog.vue'
import dayjs from 'dayjs'

// 查詢表單
const queryFormRef = ref(null)
const queryForm = reactive({
  ChangeUserId: '',
  SearchUserId: '',
  SearchRoleId: '',
  BeginDate: '',
  EndDate: ''
})

// 日期範圍
const dateRange = ref([])

// 使用者名稱和角色名稱
const changeUserName = ref('')
const searchUserName = ref('')
const roleName = ref('')

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

// 選擇對話框
const userListDialogVisible = ref(false)
const roleListDialogVisible = ref(false)
const currentUserType = ref('') // 'changeUser' or 'searchUser'

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
    newValue: newValues[index] || '--',
    oldValueDisplayObj: item.OldValueDisplayObj,
    newValueDisplayObj: item.NewValueDisplayObj
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
    } else if (currentUserType.value === 'searchUser') {
      queryForm.SearchUserId = user.UserId
      searchUserName.value = user.UserName || ''
    }
  }
  userListDialogVisible.value = false
}

// 打開角色列表對話框
const openRoleListDialog = () => {
  roleListDialogVisible.value = true
}

// 處理角色選擇
const handleRoleSelected = async (roles) => {
  if (roles && roles.length > 0) {
    const role = roles[0]
    queryForm.SearchRoleId = role.RoleId
    roleName.value = role.RoleName || ''
  }
  roleListDialogVisible.value = false
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
        SearchUserId: queryForm.SearchUserId || undefined,
        SearchRoleId: queryForm.SearchRoleId || undefined,
        BeginDate: dateRange.value[0],
        EndDate: dateRange.value[1],
        PageIndex: pagination.PageIndex,
        PageSize: pagination.PageSize
      }
      
      const response = await changeLogsApi.getUserRoleChangeLogs(queryData)
      
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
      console.error('查詢使用者角色對應設定異動記錄失敗:', error)
      ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
    } finally {
      loading.value = false
    }
  })
}

// 重置
const handleReset = () => {
  queryForm.ChangeUserId = ''
  queryForm.SearchUserId = ''
  queryForm.SearchRoleId = ''
  dateRange.value = []
  changeUserName.value = ''
  searchUserName.value = ''
  roleName.value = ''
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

// 初始化日期範圍（預設為最近一年）
onMounted(() => {
  const today = dayjs()
  const oneYearAgo = today.subtract(1, 'year')
  dateRange.value = [
    oneYearAgo.format('YYYY-MM-DD'),
    today.format('YYYY-MM-DD')
  ]
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sys0630-query {
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
    
    .value-display {
      margin-bottom: 5px;
    }
  }
}

.pagination {
  margin-top: 20px;
  text-align: right;
}
</style>

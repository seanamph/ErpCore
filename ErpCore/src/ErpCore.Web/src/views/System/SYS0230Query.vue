<template>
  <div class="sys0230-query">
    <div class="page-header">
      <h1>角色之使用者設定維護 (SYS0230)</h1>
    </div>

    <!-- 角色選擇表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="角色代碼" required>
          <el-select 
            v-model="queryForm.RoleId" 
            placeholder="請選擇角色" 
            clearable 
            filterable 
            style="width: 300px" 
            @change="handleRoleChange"
          >
            <el-option
              v-for="item in roleOptions"
              :key="item.RoleId"
              :label="`${item.RoleId} - ${item.RoleName}`"
              :value="item.RoleId"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :disabled="!queryForm.RoleId">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 角色資訊卡片 -->
    <el-card class="role-info-card" shadow="never" v-if="currentRole">
      <div class="role-info">
        <h3>角色資訊</h3>
        <el-descriptions :column="3" border>
          <el-descriptions-item label="角色代碼">{{ currentRole.RoleId }}</el-descriptions-item>
          <el-descriptions-item label="角色名稱">{{ currentRole.RoleName }}</el-descriptions-item>
          <el-descriptions-item label="角色說明">{{ currentRole.RoleNote || '-' }}</el-descriptions-item>
        </el-descriptions>
      </div>
    </el-card>

    <!-- 篩選條件 -->
    <el-card class="filter-card" shadow="never" v-if="queryForm.RoleId">
      <el-form :inline="true" :model="filterForm" class="filter-form">
        <el-form-item label="部門代碼">
          <el-input v-model="filterForm.OrgId" placeholder="請輸入部門代碼" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="店別代碼">
          <el-input v-model="filterForm.StoreId" placeholder="請輸入店別代碼" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="使用者類型">
          <el-select v-model="filterForm.UserType" placeholder="請選擇" clearable style="width: 150px">
            <el-option label="公司人員" value="1" />
            <el-option label="專櫃人員" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="篩選條件">
          <el-input 
            v-model="filterForm.Filter" 
            placeholder="使用者代碼或名稱" 
            clearable 
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleFilterReset">重置篩選</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 使用者列表 -->
    <el-card class="table-card" shadow="never" v-if="queryForm.RoleId">
      <template #header>
        <div class="card-header">
          <span>使用者列表</span>
          <div>
            <el-button type="success" @click="handleBatchAssign" :disabled="selectedRows.length === 0">批量設定</el-button>
          </div>
        </div>
      </template>

      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="UserId" label="使用者代碼" width="150" sortable />
        <el-table-column prop="UserName" label="使用者名稱" width="200" sortable />
        <el-table-column prop="OrgId" label="部門代碼" width="150" />
        <el-table-column prop="OrgName" label="部門名稱" width="200" />
        <el-table-column prop="IsAssigned" label="已分配" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.IsAssigned ? 'success' : 'info'">
              {{ row.IsAssigned ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button 
              v-if="!row.IsAssigned" 
              type="success" 
              size="small" 
              @click="handleAssign(row)"
            >
              分配
            </el-button>
            <el-button 
              v-else 
              type="danger" 
              size="small" 
              @click="handleRemove(row)"
            >
              移除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.Page"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>

    <!-- 批量設定對話框 -->
    <el-dialog
      title="批量設定角色使用者"
      v-model="batchAssignDialogVisible"
      width="600px"
      @close="handleBatchAssignDialogClose"
    >
      <div style="margin-bottom: 20px">
        <p>已選擇 <strong>{{ selectedRows.length }}</strong> 位使用者</p>
        <p>其中已分配：<strong>{{ selectedAssignedCount }}</strong> 位</p>
        <p>其中未分配：<strong>{{ selectedUnassignedCount }}</strong> 位</p>
      </div>
      <el-radio-group v-model="batchAction">
        <el-radio label="assign">分配給角色（將清除使用者的直接權限）</el-radio>
        <el-radio label="remove">從角色移除</el-radio>
      </el-radio-group>
      <template #footer>
        <el-button @click="handleBatchAssignDialogClose">取消</el-button>
        <el-button type="primary" @click="handleBatchAssignSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { rolesApi } from '@/api/roles'
import { getUsers } from '@/api/users'

// 查詢表單
const queryForm = reactive({
  RoleId: ''
})

// 篩選表單
const filterForm = reactive({
  OrgId: '',
  StoreId: '',
  UserType: '',
  Filter: ''
})

// 角色選項
const roleOptions = ref([])
const currentRole = ref(null)

// 表格資料
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 分頁
const pagination = reactive({
  Page: 1,
  PageSize: 20,
  TotalCount: 0
})

// 批量設定對話框
const batchAssignDialogVisible = ref(false)
const batchAction = ref('assign')

// 計算已選擇的已分配和未分配數量
const selectedAssignedCount = computed(() => {
  return selectedRows.value.filter(row => row.IsAssigned).length
})

const selectedUnassignedCount = computed(() => {
  return selectedRows.value.filter(row => !row.IsAssigned).length
})

// 載入角色選項
const loadRoleOptions = async () => {
  try {
    const response = await rolesApi.getRoles({ PageIndex: 1, PageSize: 1000 })
    if (response.data.success) {
      roleOptions.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('載入角色選項失敗:', error)
  }
}

// 角色變更
const handleRoleChange = async (roleId) => {
  if (roleId) {
    await loadRoleInfo(roleId)
    await handleSearch()
  } else {
    currentRole.value = null
    tableData.value = []
  }
}

// 載入角色資訊
const loadRoleInfo = async (roleId) => {
  try {
    const response = await rolesApi.getRole(roleId)
    if (response.data.success) {
      currentRole.value = response.data.data
    }
  } catch (error) {
    console.error('載入角色資訊失敗:', error)
  }
}

// 查詢
const handleSearch = async () => {
  if (!queryForm.RoleId) {
    ElMessage.warning('請選擇角色')
    return
  }

  loading.value = true
  try {
    const params = {
      RoleId: queryForm.RoleId,
      OrgId: filterForm.OrgId || undefined,
      StoreId: filterForm.StoreId || undefined,
      UserType: filterForm.UserType || undefined,
      Filter: filterForm.Filter || undefined,
      Page: pagination.Page,
      PageSize: pagination.PageSize
    }
    const response = await rolesApi.getRoleUsers(queryForm.RoleId, params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.message || error.message))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.RoleId = ''
  currentRole.value = null
  tableData.value = []
  handleFilterReset()
  pagination.Page = 1
  pagination.TotalCount = 0
}

// 重置篩選
const handleFilterReset = () => {
  filterForm.OrgId = ''
  filterForm.StoreId = ''
  filterForm.UserType = ''
  filterForm.Filter = ''
  pagination.Page = 1
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 分配使用者
const handleAssign = async (row) => {
  try {
    await ElMessageBox.confirm(
      `確定要將使用者「${row.UserName}」分配給角色「${currentRole.value?.RoleName}」嗎？\n注意：此操作將清除使用者的直接權限設定。`,
      '提示',
      {
        type: 'warning'
      }
    )
    const response = await rolesApi.assignUserToRole(queryForm.RoleId, { UserId: row.UserId })
    if (response.data.success) {
      ElMessage.success('分配成功')
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '分配失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('分配失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

// 移除使用者
const handleRemove = async (row) => {
  try {
    await ElMessageBox.confirm(
      `確定要將使用者「${row.UserName}」從角色「${currentRole.value?.RoleName}」移除嗎？`,
      '提示',
      {
        type: 'warning'
      }
    )
    const response = await rolesApi.removeUserFromRole(queryForm.RoleId, row.UserId)
    if (response.data.success) {
      ElMessage.success('移除成功')
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '移除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('移除失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

// 批量設定
const handleBatchAssign = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇要設定的使用者')
    return
  }
  batchAssignDialogVisible.value = true
  batchAction.value = 'assign'
}

// 批量設定提交
const handleBatchAssignSubmit = async () => {
  try {
    const addUserIds = []
    const removeUserIds = []

    if (batchAction.value === 'assign') {
      // 只處理未分配的使用者
      selectedRows.value
        .filter(row => !row.IsAssigned)
        .forEach(row => addUserIds.push(row.UserId))
      
      if (addUserIds.length === 0) {
        ElMessage.warning('所選使用者均已分配，無需操作')
        batchAssignDialogVisible.value = false
        return
      }

      await ElMessageBox.confirm(
        `確定要將 ${addUserIds.length} 位使用者分配給角色嗎？\n注意：此操作將清除這些使用者的直接權限設定。`,
        '提示',
        {
          type: 'warning'
        }
      )
    } else {
      // 只處理已分配的使用者
      selectedRows.value
        .filter(row => row.IsAssigned)
        .forEach(row => removeUserIds.push(row.UserId))
      
      if (removeUserIds.length === 0) {
        ElMessage.warning('所選使用者均未分配，無需操作')
        batchAssignDialogVisible.value = false
        return
      }

      await ElMessageBox.confirm(
        `確定要將 ${removeUserIds.length} 位使用者從角色移除嗎？`,
        '提示',
        {
          type: 'warning'
        }
      )
    }

    const response = await rolesApi.batchAssignRoleUsers(queryForm.RoleId, {
      AddUserIds: addUserIds,
      RemoveUserIds: removeUserIds
    })

    if (response.data.success) {
      const { addedCount, removedCount } = response.data.data
      let message = ''
      if (addedCount > 0 && removedCount > 0) {
        message = `成功新增 ${addedCount} 位使用者，移除 ${removedCount} 位使用者`
      } else if (addedCount > 0) {
        message = `成功新增 ${addedCount} 位使用者`
      } else if (removedCount > 0) {
        message = `成功移除 ${removedCount} 位使用者`
      } else {
        message = '設定未變更'
      }
      ElMessage.success(message)
      batchAssignDialogVisible.value = false
      selectedRows.value = []
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '批量設定失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批量設定失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

// 關閉批量設定對話框
const handleBatchAssignDialogClose = () => {
  batchAssignDialogVisible.value = false
  batchAction.value = 'assign'
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.Page = 1
  handleSearch()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.Page = page
  handleSearch()
}

// 初始化
onMounted(() => {
  loadRoleOptions()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sys0230-query {
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

  .search-card,
  .filter-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    
    .search-form,
    .filter-form {
      margin: 0;
    }
  }

  .role-info-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;

    .role-info {
      h3 {
        margin-top: 0;
        margin-bottom: 15px;
        color: $primary-color;
      }
    }
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
  }
}
</style>

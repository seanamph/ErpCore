<template>
  <div class="sys0220-query">
    <div class="page-header">
      <h1>使用者之角色設定維護 (SYS0220)</h1>
    </div>

    <!-- 使用者選擇表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者代碼" required>
          <el-select 
            v-model="queryForm.UserId" 
            placeholder="請選擇使用者" 
            clearable 
            filterable 
            style="width: 300px" 
            @change="handleUserChange"
          >
            <el-option
              v-for="item in userOptions"
              :key="item.UserId"
              :label="`${item.UserId} - ${item.UserName}`"
              :value="item.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :disabled="!queryForm.UserId">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 使用者資訊卡片 -->
    <el-card class="user-info-card" shadow="never" v-if="currentUser">
      <div class="user-info">
        <h3>使用者資訊</h3>
        <el-descriptions :column="3" border>
          <el-descriptions-item label="使用者編號">{{ currentUser.UserId }}</el-descriptions-item>
          <el-descriptions-item label="使用者名稱">{{ currentUser.UserName }}</el-descriptions-item>
          <el-descriptions-item label="組織">{{ currentUser.OrgId || '-' }}</el-descriptions-item>
        </el-descriptions>
      </div>
    </el-card>

    <!-- 已分配角色列表 -->
    <el-card class="table-card" shadow="never" v-if="queryForm.UserId">
      <template #header>
        <div class="card-header">
          <span>已分配角色列表</span>
          <div>
            <el-button type="success" @click="handleAddRole" :disabled="!queryForm.UserId">新增角色</el-button>
            <el-button type="warning" @click="handleBatchUpdate" :disabled="!queryForm.UserId">批量更新</el-button>
            <el-button type="danger" @click="handleBatchRemove" :disabled="selectedRows.length === 0">移除選取</el-button>
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
        <el-table-column prop="RoleId" label="角色代碼" width="150" sortable />
        <el-table-column prop="RoleName" label="角色名稱" width="200" sortable />
        <el-table-column prop="CreatedBy" label="建立者" width="120" />
        <el-table-column prop="CreatedAt" label="建立時間" width="180" sortable>
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button type="danger" size="small" @click="handleRemove(row)">移除</el-button>
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

    <!-- 新增角色對話框 -->
    <el-dialog
      title="選擇角色"
      v-model="addRoleDialogVisible"
      width="800px"
      @close="handleAddRoleDialogClose"
    >
      <el-form :inline="true" style="margin-bottom: 10px">
        <el-form-item label="搜尋">
          <el-input 
            v-model="roleSearchKeyword" 
            placeholder="請輸入角色代碼或名稱" 
            clearable 
            style="width: 300px"
            @keyup.enter="handleRoleSearch"
          />
          <el-button type="primary" @click="handleRoleSearch" style="margin-left: 10px">搜尋</el-button>
        </el-form-item>
      </el-form>

      <el-table
        :data="availableRolesData"
        v-loading="roleLoading"
        border
        stripe
        style="width: 100%"
        @selection-change="handleRoleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="RoleId" label="角色代碼" width="150" sortable />
        <el-table-column prop="RoleName" label="角色名稱" width="200" sortable />
        <el-table-column prop="RoleNote" label="角色說明" min-width="200" show-overflow-tooltip />
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="rolePagination.PageIndex"
        v-model:page-size="rolePagination.PageSize"
        :total="rolePagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleRoleSizeChange"
        @current-change="handleRolePageChange"
        style="margin-top: 20px; text-align: right"
      />

      <template #footer>
        <el-button @click="handleAddRoleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleAssignRoles" :disabled="selectedRoles.length === 0">確定</el-button>
      </template>
    </el-dialog>

    <!-- 批量更新對話框 -->
    <el-dialog
      title="批量更新角色"
      v-model="batchUpdateDialogVisible"
      width="800px"
      @close="handleBatchUpdateDialogClose"
    >
      <el-form :inline="true" style="margin-bottom: 10px">
        <el-form-item label="搜尋">
          <el-input 
            v-model="batchRoleSearchKeyword" 
            placeholder="請輸入角色代碼或名稱" 
            clearable 
            style="width: 300px"
            @keyup.enter="handleBatchRoleSearch"
          />
          <el-button type="primary" @click="handleBatchRoleSearch" style="margin-left: 10px">搜尋</el-button>
        </el-form-item>
      </el-form>

      <el-table
        :data="allRolesData"
        v-loading="batchRoleLoading"
        border
        stripe
        style="width: 100%"
        @selection-change="handleBatchRoleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center">
          <template #default="{ row }">
            <el-checkbox 
              :model-value="isRoleSelected(row.RoleId)"
              @change="(val) => toggleRoleSelection(row.RoleId, val)"
            />
          </template>
        </el-table-column>
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="RoleId" label="角色代碼" width="150" sortable />
        <el-table-column prop="RoleName" label="角色名稱" width="200" sortable />
        <el-table-column prop="RoleNote" label="角色說明" min-width="200" show-overflow-tooltip />
        <el-table-column label="狀態" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="isRoleAssigned(row.RoleId) ? 'success' : 'info'">
              {{ isRoleAssigned(row.RoleId) ? '已分配' : '未分配' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="batchRolePagination.PageIndex"
        v-model:page-size="batchRolePagination.PageSize"
        :total="batchRolePagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleBatchRoleSizeChange"
        @current-change="handleBatchRolePageChange"
        style="margin-top: 20px; text-align: right"
      />

      <template #footer>
        <el-button @click="handleBatchUpdateDialogClose">取消</el-button>
        <el-button type="primary" @click="handleBatchUpdateSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { userRolesApi } from '@/api/userRoles'
import { getUsers, getUserById } from '@/api/users'
import { rolesApi } from '@/api/roles'

// 查詢表單
const queryForm = reactive({
  UserId: ''
})

// 使用者選項
const userOptions = ref([])
const currentUser = ref(null)

// 表格資料
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 新增角色對話框
const addRoleDialogVisible = ref(false)
const roleSearchKeyword = ref('')
const availableRolesData = ref([])
const roleLoading = ref(false)
const selectedRoles = ref([])
const rolePagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 批量更新對話框
const batchUpdateDialogVisible = ref(false)
const batchRoleSearchKeyword = ref('')
const allRolesData = ref([])
const batchRoleLoading = ref(false)
const batchSelectedRoleIds = ref([])
const batchRolePagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

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

// 載入使用者選項
const loadUserOptions = async () => {
  try {
    const response = await usersApi.getUsers({ PageIndex: 1, PageSize: 1000 })
    if (response.data.success) {
      userOptions.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('載入使用者選項失敗:', error)
  }
}

// 使用者變更
const handleUserChange = async (userId) => {
  if (userId) {
    await loadUserInfo(userId)
    await handleSearch()
  } else {
    currentUser.value = null
    tableData.value = []
  }
}

// 載入使用者資訊
const loadUserInfo = async (userId) => {
  try {
    const response = await getUserById(userId)
    if (response.data.success) {
      currentUser.value = response.data.data
    }
  } catch (error) {
    console.error('載入使用者資訊失敗:', error)
  }
}

// 查詢
const handleSearch = async () => {
  if (!queryForm.UserId) {
    ElMessage.warning('請選擇使用者')
    return
  }

  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize
    }
    const response = await userRolesApi.getUserRoles(queryForm.UserId, params)
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
  queryForm.UserId = ''
  currentUser.value = null
  tableData.value = []
  pagination.PageIndex = 1
  pagination.TotalCount = 0
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 新增角色
const handleAddRole = async () => {
  if (!queryForm.UserId) {
    ElMessage.warning('請先選擇使用者')
    return
  }
  addRoleDialogVisible.value = true
  roleSearchKeyword.value = ''
  selectedRoles.value = []
  await loadAvailableRoles()
}

// 載入可用角色
const loadAvailableRoles = async () => {
  if (!queryForm.UserId) return

  roleLoading.value = true
  try {
    const params = {
      PageIndex: rolePagination.PageIndex,
      PageSize: rolePagination.PageSize,
      keyword: roleSearchKeyword.value || undefined
    }
    const response = await userRolesApi.getAvailableRoles(queryForm.UserId, params)
    if (response.data.success) {
      availableRolesData.value = response.data.data.items || []
      rolePagination.TotalCount = response.data.data.totalCount || 0
    } else {
      ElMessage.error(response.data.message || '載入可用角色失敗')
    }
  } catch (error) {
    ElMessage.error('載入可用角色失敗：' + (error.response?.data?.message || error.message))
  } finally {
    roleLoading.value = false
  }
}

// 角色搜尋
const handleRoleSearch = () => {
  rolePagination.PageIndex = 1
  loadAvailableRoles()
}

// 角色選擇變更
const handleRoleSelectionChange = (selection) => {
  selectedRoles.value = selection
}

// 分配角色
const handleAssignRoles = async () => {
  if (selectedRoles.value.length === 0) {
    ElMessage.warning('請選擇要分配的角色')
    return
  }

  try {
    const roleIds = selectedRoles.value.map(role => role.RoleId)
    const response = await userRolesApi.assignRoles(queryForm.UserId, { RoleIds: roleIds })
    if (response.data.success) {
      ElMessage.success(`成功分配 ${response.data.data.assignedCount} 個角色`)
      addRoleDialogVisible.value = false
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '分配角色失敗')
    }
  } catch (error) {
    ElMessage.error('分配角色失敗：' + (error.response?.data?.message || error.message))
  }
}

// 移除角色
const handleRemove = async (row) => {
  try {
    await ElMessageBox.confirm(`確定要移除角色「${row.RoleName}」嗎？`, '提示', {
      type: 'warning'
    })
    const response = await userRolesApi.removeRoles(queryForm.UserId, { RoleIds: [row.RoleId] })
    if (response.data.success) {
      ElMessage.success(`成功移除 ${response.data.data.removedCount} 個角色`)
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '移除角色失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('移除角色失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

// 批次移除
const handleBatchRemove = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇要移除的角色')
    return
  }

  try {
    await ElMessageBox.confirm(`確定要移除選取的 ${selectedRows.value.length} 個角色嗎？`, '提示', {
      type: 'warning'
    })
    const roleIds = selectedRows.value.map(row => row.RoleId)
    const response = await userRolesApi.removeRoles(queryForm.UserId, { RoleIds: roleIds })
    if (response.data.success) {
      ElMessage.success(`成功移除 ${response.data.data.removedCount} 個角色`)
      selectedRows.value = []
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '批次移除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次移除失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

// 批量更新
const handleBatchUpdate = async () => {
  if (!queryForm.UserId) {
    ElMessage.warning('請先選擇使用者')
    return
  }
  batchUpdateDialogVisible.value = true
  batchRoleSearchKeyword.value = ''
  batchSelectedRoleIds.value = []
  // 初始化選中已分配的角色
  const assignedRoleIds = tableData.value.map(row => row.RoleId)
  batchSelectedRoleIds.value = [...assignedRoleIds]
  await loadAllRoles()
}

// 載入所有角色
const loadAllRoles = async () => {
  batchRoleLoading.value = true
  try {
    const params = {
      PageIndex: batchRolePagination.PageIndex,
      PageSize: batchRolePagination.PageSize,
      RoleId: batchRoleSearchKeyword.value || undefined,
      RoleName: batchRoleSearchKeyword.value || undefined
    }
    const response = await rolesApi.getRoles(params)
    if (response.data.success) {
      allRolesData.value = response.data.data.items || []
      batchRolePagination.TotalCount = response.data.data.totalCount || 0
    } else {
      ElMessage.error(response.data.message || '載入角色列表失敗')
    }
  } catch (error) {
    ElMessage.error('載入角色列表失敗：' + (error.response?.data?.message || error.message))
  } finally {
    batchRoleLoading.value = false
  }
}

// 批量角色搜尋
const handleBatchRoleSearch = () => {
  batchRolePagination.PageIndex = 1
  loadAllRoles()
}

// 檢查角色是否已分配
const isRoleAssigned = (roleId) => {
  return tableData.value.some(row => row.RoleId === roleId)
}

// 檢查角色是否選中
const isRoleSelected = (roleId) => {
  return batchSelectedRoleIds.value.includes(roleId)
}

// 切換角色選中狀態
const toggleRoleSelection = (roleId, selected) => {
  if (selected) {
    if (!batchSelectedRoleIds.value.includes(roleId)) {
      batchSelectedRoleIds.value.push(roleId)
    }
  } else {
    const index = batchSelectedRoleIds.value.indexOf(roleId)
    if (index > -1) {
      batchSelectedRoleIds.value.splice(index, 1)
    }
  }
}

// 批量角色選擇變更
const handleBatchRoleSelectionChange = (selection) => {
  // 同步選中狀態
  selection.forEach(row => {
    if (!batchSelectedRoleIds.value.includes(row.RoleId)) {
      batchSelectedRoleIds.value.push(row.RoleId)
    }
  })
}

// 批量更新提交
const handleBatchUpdateSubmit = async () => {
  try {
    const response = await userRolesApi.batchUpdateRoles(queryForm.UserId, { RoleIds: batchSelectedRoleIds.value })
    if (response.data.success) {
      const { addedCount, removedCount } = response.data.data
      let message = ''
      if (addedCount > 0 && removedCount > 0) {
        message = `成功新增 ${addedCount} 個角色，移除 ${removedCount} 個角色`
      } else if (addedCount > 0) {
        message = `成功新增 ${addedCount} 個角色`
      } else if (removedCount > 0) {
        message = `成功移除 ${removedCount} 個角色`
      } else {
        message = '角色設定未變更'
      }
      ElMessage.success(message)
      batchUpdateDialogVisible.value = false
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '批量更新失敗')
    }
  } catch (error) {
    ElMessage.error('批量更新失敗：' + (error.response?.data?.message || error.message))
  }
}

// 關閉新增角色對話框
const handleAddRoleDialogClose = () => {
  addRoleDialogVisible.value = false
  roleSearchKeyword.value = ''
  selectedRoles.value = []
  rolePagination.PageIndex = 1
}

// 關閉批量更新對話框
const handleBatchUpdateDialogClose = () => {
  batchUpdateDialogVisible.value = false
  batchRoleSearchKeyword.value = ''
  batchSelectedRoleIds.value = []
  batchRolePagination.PageIndex = 1
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

// 角色分頁大小變更
const handleRoleSizeChange = (size) => {
  rolePagination.PageSize = size
  rolePagination.PageIndex = 1
  loadAvailableRoles()
}

// 角色分頁變更
const handleRolePageChange = (page) => {
  rolePagination.PageIndex = page
  loadAvailableRoles()
}

// 批量角色分頁大小變更
const handleBatchRoleSizeChange = (size) => {
  batchRolePagination.PageSize = size
  batchRolePagination.PageIndex = 1
  loadAllRoles()
}

// 批量角色分頁變更
const handleBatchRolePageChange = (page) => {
  batchRolePagination.PageIndex = page
  loadAllRoles()
}

// 初始化
onMounted(() => {
  loadUserOptions()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sys0220-query {
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

  .user-info-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;

    .user-info {
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

<template>
  <div class="sys0320-query">
    <div class="page-header">
      <h1>使用者系統權限設定 (SYS0320)</h1>
    </div>

    <!-- 警告提示 -->
    <el-alert
      title="重要提示"
      type="warning"
      :closable="false"
      show-icon
      style="margin-bottom: 20px"
    >
      <template #default>
        設定權限後，所有的權限將以本使用者為主，對應之角色將移除
      </template>
    </el-alert>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者代碼" required>
          <el-select v-model="queryForm.UserId" placeholder="請選擇使用者" clearable filterable style="width: 200px" @change="handleUserChange">
            <el-option
              v-for="item in userOptions"
              :key="item.UserId"
              :label="`${item.UserId} - ${item.UserName}`"
              :value="item.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="系統代碼">
          <el-select v-model="queryForm.SystemId" placeholder="請選擇系統" clearable filterable style="width: 200px">
            <el-option
              v-for="item in systemOptions"
              :key="item.SystemId"
              :label="item.SystemName"
              :value="item.SystemId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="子系統代碼">
          <el-select v-model="queryForm.SubSystemId" placeholder="請選擇子系統" clearable filterable style="width: 200px">
            <el-option
              v-for="item in menuOptions"
              :key="item.MenuId"
              :label="item.MenuName"
              :value="item.MenuId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="作業代碼">
          <el-select v-model="queryForm.ProgramId" placeholder="請選擇作業" clearable filterable style="width: 200px">
            <el-option
              v-for="item in programOptions"
              :key="item.ProgramId"
              :label="item.ProgramName"
              :value="item.ProgramId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="按鈕代碼">
          <el-input v-model="queryForm.ButtonId" placeholder="請輸入按鈕代碼" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :disabled="!queryForm.UserId">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleBatchSet" :disabled="!queryForm.UserId">批量設定</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
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
        <el-table-column prop="SystemName" label="系統" width="150" sortable />
        <el-table-column prop="SubSystemName" label="子系統" width="150" sortable />
        <el-table-column prop="ProgramName" label="作業" width="200" sortable />
        <el-table-column prop="ButtonId" label="按鈕代碼" width="120" sortable />
        <el-table-column prop="ButtonName" label="按鈕名稱" width="150" sortable />
        <el-table-column prop="CreatedBy" label="建立者" width="100" />
        <el-table-column prop="CreatedAt" label="建立時間" width="160" sortable>
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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

      <!-- 批次操作按鈕 -->
      <div style="margin-top: 10px">
        <el-button type="danger" @click="handleBatchDelete" :disabled="selectedRows.length === 0">
          批次刪除
        </el-button>
      </div>
    </el-card>

    <!-- 批量設定對話框 -->
    <el-dialog
      title="批量設定使用者權限"
      v-model="batchDialogVisible"
      width="900px"
      @close="handleBatchDialogClose"
    >
      <el-tabs v-model="activeTab" type="border-card">
        <!-- 系統權限設定 -->
        <el-tab-pane label="系統權限" name="system">
          <el-table :data="systemStatsData" border stripe style="width: 100%">
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="SystemName" label="系統名稱" width="200" />
            <el-table-column prop="TotalButtons" label="總按鈕數" width="100" align="center" />
            <el-table-column prop="AuthorizedButtons" label="已授權按鈕數" width="120" align="center" />
            <el-table-column prop="AuthorizedRate" label="授權率" width="100" align="center">
              <template #default="{ row }">
                {{ (row.AuthorizedRate * 100).toFixed(1) }}%
              </template>
            </el-table-column>
            <el-table-column prop="IsFullyAuthorized" label="是否全選" width="100" align="center">
              <template #default="{ row }">
                <el-tag :type="row.IsFullyAuthorized ? 'success' : 'info'">
                  {{ row.IsFullyAuthorized ? '是' : '否' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="150" align="center">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSetSystemPermission(row, true)">全選</el-button>
                <el-button type="danger" size="small" @click="handleSetSystemPermission(row, false)">取消</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <!-- 選單權限設定 -->
        <el-tab-pane label="選單權限" name="menu">
          <el-form :inline="true" style="margin-bottom: 10px">
            <el-form-item label="系統代碼">
              <el-select v-model="menuFilterSystemId" placeholder="請選擇系統" clearable filterable style="width: 200px" @change="loadMenuOptions">
                <el-option
                  v-for="item in systemOptions"
                  :key="item.SystemId"
                  :label="item.SystemName"
                  :value="item.SystemId"
                />
              </el-select>
            </el-form-item>
          </el-form>
          <el-tree
            :data="menuTreeData"
            show-checkbox
            node-key="MenuId"
            :props="{ children: 'children', label: 'MenuName' }"
            ref="menuTreeRef"
            style="max-height: 400px; overflow-y: auto"
          />
        </el-tab-pane>

        <!-- 作業權限設定 -->
        <el-tab-pane label="作業權限" name="program">
          <el-form :inline="true" style="margin-bottom: 10px">
            <el-form-item label="子系統代碼">
              <el-select v-model="programFilterMenuId" placeholder="請選擇子系統" clearable filterable style="width: 200px" @change="loadProgramOptions">
                <el-option
                  v-for="item in menuOptions"
                  :key="item.MenuId"
                  :label="item.MenuName"
                  :value="item.MenuId"
                />
              </el-select>
            </el-form-item>
          </el-form>
          <el-table :data="programOptions" border stripe style="width: 100%">
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="ProgramId" label="作業代碼" width="150" />
            <el-table-column prop="ProgramName" label="作業名稱" width="200" />
            <el-table-column label="操作" width="150" align="center">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSetProgramPermission(row, true)">授權</el-button>
                <el-button type="danger" size="small" @click="handleSetProgramPermission(row, false)">取消</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <!-- 按鈕權限設定 -->
        <el-tab-pane label="按鈕權限" name="button">
          <el-form :inline="true" style="margin-bottom: 10px">
            <el-form-item label="作業代碼">
              <el-select v-model="buttonFilterProgramId" placeholder="請選擇作業" clearable filterable style="width: 200px" @change="loadButtonOptions">
                <el-option
                  v-for="item in programOptions"
                  :key="item.ProgramId"
                  :label="item.ProgramName"
                  :value="item.ProgramId"
                />
              </el-select>
            </el-form-item>
          </el-form>
          <el-table :data="buttonOptions" border stripe style="width: 100%">
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="ButtonId" label="按鈕代碼" width="150" />
            <el-table-column prop="ButtonName" label="按鈕名稱" width="200" />
            <el-table-column label="操作" width="150" align="center">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSetButtonPermission(row, true)">授權</el-button>
                <el-button type="danger" size="small" @click="handleSetButtonPermission(row, false)">取消</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <el-button @click="handleBatchDialogClose">取消</el-button>
        <el-button type="primary" @click="handleBatchSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { userPermissionsApi } from '@/api/permissions'
import { getUsers } from '@/api/users'
import { getSystems } from '@/api/systems'
import { getMenus } from '@/api/menus'
import { getPrograms } from '@/api/programs'
import { getButtons } from '@/api/buttons'

// 查詢表單
const queryForm = reactive({
  UserId: '',
  SystemId: '',
  SubSystemId: '',
  ProgramId: '',
  ButtonId: ''
})

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

// 選項資料
const userOptions = ref([])
const systemOptions = ref([])
const menuOptions = ref([])
const programOptions = ref([])
const buttonOptions = ref([])

// 批量設定對話框
const batchDialogVisible = ref(false)
const activeTab = ref('system')
const systemStatsData = ref([])
const menuTreeData = ref([])
const menuTreeRef = ref(null)
const menuFilterSystemId = ref('')
const programFilterMenuId = ref('')
const buttonFilterProgramId = ref('')

// 載入使用者選項
const loadUserOptions = async () => {
  try {
    const response = await getUsers({
      PageIndex: 1,
      PageSize: 1000
    })
    if (response && response.Data) {
      userOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入使用者選項失敗:', error)
  }
}

// 載入系統選項
const loadSystemOptions = async () => {
  try {
    const response = await getSystems({
      PageIndex: 1,
      PageSize: 1000
    })
    if (response && response.Data) {
      systemOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入系統選項失敗:', error)
  }
}

// 載入子系統選項
const loadMenuOptions = async () => {
  try {
    const params = {
      PageIndex: 1,
      PageSize: 1000
    }
    if (menuFilterSystemId.value) {
      params.SystemId = menuFilterSystemId.value
    }
    const response = await getMenus(params)
    if (response && response.Data) {
      menuOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入子系統選項失敗:', error)
  }
}

// 載入作業選項
const loadProgramOptions = async () => {
  try {
    const params = {
      PageIndex: 1,
      PageSize: 1000
    }
    if (programFilterMenuId.value) {
      params.MenuId = programFilterMenuId.value
    }
    const response = await getPrograms(params)
    if (response && response.Data) {
      programOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入作業選項失敗:', error)
  }
}

// 載入按鈕選項
const loadButtonOptions = async () => {
  try {
    const params = {
      PageIndex: 1,
      PageSize: 1000
    }
    if (buttonFilterProgramId.value) {
      params.ProgramId = buttonFilterProgramId.value
    }
    const response = await getButtons(params)
    if (response && response.Data) {
      buttonOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入按鈕選項失敗:', error)
  }
}

// 載入系統統計資料
const loadSystemStats = async () => {
  if (!queryForm.UserId) return
  try {
    const response = await userPermissionsApi.getSystemStats(queryForm.UserId)
    if (response && response.Data) {
      systemStatsData.value = response.Data || []
    }
  } catch (error) {
    console.error('載入系統統計失敗:', error)
  }
}

// 載入資料
const loadData = async () => {
  if (!queryForm.UserId) {
    ElMessage.warning('請先選擇使用者')
    return
  }
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SystemId: queryForm.SystemId || undefined,
      SubSystemId: queryForm.SubSystemId || undefined,
      ProgramId: queryForm.ProgramId || undefined,
      ButtonId: queryForm.ButtonId || undefined
    }
    const response = await userPermissionsApi.getUserPermissions(queryForm.UserId, params)
    if (response && response.Data) {
      tableData.value = response.Data.Items || []
      pagination.TotalCount = response.Data.TotalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 使用者變更
const handleUserChange = () => {
  pagination.PageIndex = 1
  loadData()
  loadSystemStats()
}

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadData()
}

// 重置
const handleReset = () => {
  Object.assign(queryForm, {
    UserId: '',
    SystemId: '',
    SubSystemId: '',
    ProgramId: '',
    ButtonId: ''
  })
  tableData.value = []
  pagination.TotalCount = 0
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(`確定要刪除權限「${row.ButtonName}」嗎？`, '確認刪除', {
      type: 'warning'
    })
    await userPermissionsApi.deleteUserPermission(queryForm.UserId, row.TKey)
    ElMessage.success('刪除成功')
    loadData()
    loadSystemStats()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 批次刪除
const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇要刪除的資料')
    return
  }
  try {
    await ElMessageBox.confirm(`確定要刪除選取的 ${selectedRows.value.length} 筆資料嗎？`, '確認批次刪除', {
      type: 'warning'
    })
    const tKeys = selectedRows.value.map(row => row.TKey)
    await userPermissionsApi.batchDeleteUserPermissions(queryForm.UserId, { TKeys: tKeys })
    ElMessage.success('批次刪除成功')
    selectedRows.value = []
    loadData()
    loadSystemStats()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 批量設定
const handleBatchSet = () => {
  if (!queryForm.UserId) {
    ElMessage.warning('請先選擇使用者')
    return
  }
  batchDialogVisible.value = true
  activeTab.value = 'system'
  loadSystemStats()
  loadSystemOptions()
  loadMenuOptions()
}

// 設定系統權限
const handleSetSystemPermission = async (row, isAuthorized) => {
  try {
    await ElMessageBox.confirm(
      `確定要${isAuthorized ? '授權' : '取消'}系統「${row.SystemName}」的所有權限嗎？設定後，該使用者的角色權限對應將被移除。`,
      '確認設定',
      {
        type: 'warning'
      }
    )
    await userPermissionsApi.batchSetSystemPermissions(queryForm.UserId, {
      SystemPermissions: [{
        SystemId: row.SystemId,
        IsAuthorized: isAuthorized
      }],
      ClearRolePermissions: true
    })
    ElMessage.success('設定成功')
    loadSystemStats()
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('設定失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 設定作業權限
const handleSetProgramPermission = async (row, isAuthorized) => {
  try {
    await ElMessageBox.confirm(
      `確定要${isAuthorized ? '授權' : '取消'}作業「${row.ProgramName}」的所有權限嗎？設定後，該使用者的角色權限對應將被移除。`,
      '確認設定',
      {
        type: 'warning'
      }
    )
    await userPermissionsApi.batchSetProgramPermissions(queryForm.UserId, {
      ProgramPermissions: [{
        ProgramId: row.ProgramId,
        IsAuthorized: isAuthorized
      }],
      ClearRolePermissions: true
    })
    ElMessage.success('設定成功')
    loadData()
    loadSystemStats()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('設定失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 設定按鈕權限
const handleSetButtonPermission = async (row, isAuthorized) => {
  try {
    await ElMessageBox.confirm(
      `確定要${isAuthorized ? '授權' : '取消'}按鈕「${row.ButtonName}」的權限嗎？設定後，該使用者的角色權限對應將被移除。`,
      '確認設定',
      {
        type: 'warning'
      }
    )
    await userPermissionsApi.batchSetButtonPermissions(queryForm.UserId, {
      ButtonPermissions: [{
        ButtonId: row.ButtonId,
        IsAuthorized: isAuthorized
      }],
      ClearRolePermissions: true
    })
    ElMessage.success('設定成功')
    loadData()
    loadSystemStats()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('設定失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 批量提交
const handleBatchSubmit = async () => {
  try {
    if (activeTab.value === 'system') {
      // 系統權限已在單獨操作中處理
      ElMessage.success('操作完成')
    } else if (activeTab.value === 'menu') {
      const checkedKeys = menuTreeRef.value?.getCheckedKeys() || []
      const halfCheckedKeys = menuTreeRef.value?.getHalfCheckedKeys() || []
      const allKeys = [...checkedKeys, ...halfCheckedKeys]
      if (allKeys.length > 0) {
        await ElMessageBox.confirm(
          '確定要批量設定選單權限嗎？設定後，該使用者的角色權限對應將被移除。',
          '確認設定',
          {
            type: 'warning'
          }
        )
        await userPermissionsApi.batchSetMenuPermissions(queryForm.UserId, {
          MenuPermissions: allKeys.map(menuId => ({
            SubSystemId: menuId,
            IsAuthorized: true
          })),
          ClearRolePermissions: true
        })
        ElMessage.success('設定成功')
        loadData()
        loadSystemStats()
      }
    }
    batchDialogVisible.value = false
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('設定失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 關閉批量設定對話框
const handleBatchDialogClose = () => {
  batchDialogVisible.value = false
  activeTab.value = 'system'
  menuFilterSystemId.value = ''
  programFilterMenuId.value = ''
  buttonFilterProgramId.value = ''
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
  loadUserOptions()
  loadSystemOptions()
  loadMenuOptions()
  loadProgramOptions()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0320-query {
  .page-header {
    h1 {
      color: $primary-color;
    }
  }
  
  .search-card {
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

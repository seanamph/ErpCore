<template>
  <div class="sys0360-query">
    <div class="page-header">
      <h1>系統對應權限設定 (SYS0360)</h1>
    </div>

    <!-- 項目選擇區域 -->
    <el-card class="search-card" shadow="never" v-if="!selectedItemId">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="項目代碼">
          <el-input v-model="queryForm.ItemId" placeholder="請輸入項目代碼" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="項目名稱">
          <el-input v-model="queryForm.ItemName" placeholder="請輸入項目名稱" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="項目類型">
          <el-input v-model="queryForm.ItemType" placeholder="請輸入項目類型" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleAdd">新增項目</el-button>
        </el-form-item>
      </el-form>

      <!-- 項目列表表格 -->
      <el-table
        :data="itemList"
        v-loading="loading"
        border
        stripe
        style="width: 100%; margin-top: 20px"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="ItemId" label="項目代碼" width="150" sortable />
        <el-table-column prop="ItemName" label="項目名稱" width="200" sortable />
        <el-table-column prop="ItemType" label="項目類型" width="150" />
        <el-table-column prop="Status" label="狀態" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedBy" label="建立者" width="100" />
        <el-table-column prop="CreatedAt" label="建立時間" width="160" sortable>
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleSetPermission(row)">設定權限</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
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
    </el-card>

    <!-- 權限設定區域 -->
    <el-card class="permission-card" shadow="never" v-if="selectedItemId">
      <div class="item-info" style="margin-bottom: 20px">
        <el-tag type="info" size="large">項目代碼: {{ selectedItemId }}</el-tag>
        <el-tag type="info" size="large" style="margin-left: 10px">項目名稱: {{ selectedItemName }}</el-tag>
        <el-button type="primary" style="margin-left: 20px" @click="handleBackToList">返回列表</el-button>
      </div>

      <el-tabs v-model="activeTab" type="border-card">
        <!-- 系統權限設定 -->
        <el-tab-pane label="系統權限" name="system">
          <el-table :data="systemList" border stripe style="width: 100%" v-loading="systemLoading">
            <el-table-column type="index" label="序號" width="80" align="center" />
            <el-table-column label="是否全選" width="150" align="center">
              <template #default="{ row }">
                <el-select v-model="row.Status" @change="handleSystemStatusChange(row)" style="width: 120px">
                  <el-option label="全選" value="全選" />
                  <el-option label="部份" value="部份" />
                  <el-option label="未選" value="未選" />
                </el-select>
              </template>
            </el-table-column>
            <el-table-column prop="SystemId" label="系統代碼" width="150" />
            <el-table-column prop="SystemName" label="系統名稱" width="200" />
            <el-table-column label="權限統計" width="150" align="center">
              <template #default="{ row }">
                {{ row.PermissionCount }} / {{ row.TotalCount }}
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" align="center">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSetMenuPermission(row)">設定選單權限</el-button>
              </template>
            </el-table-column>
          </el-table>
          <div style="margin-top: 20px">
            <el-button type="primary" @click="handleSaveSystemPermissions">儲存系統權限設定</el-button>
          </div>
        </el-tab-pane>

        <!-- 選單權限設定 -->
        <el-tab-pane label="選單權限" name="menu" v-if="selectedSystemId">
          <div style="margin-bottom: 20px">
            <el-tag type="info">系統: {{ selectedSystemName }}</el-tag>
            <el-button type="primary" style="margin-left: 20px" @click="handleBackToSystem">返回系統列表</el-button>
          </div>
          <el-table :data="menuList" border stripe style="width: 100%" v-loading="menuLoading">
            <el-table-column type="index" label="序號" width="80" align="center" />
            <el-table-column prop="MenuId" label="選單代碼" width="150" />
            <el-table-column prop="MenuName" label="選單名稱" width="200" />
            <el-table-column label="權限統計" width="150" align="center">
              <template #default="{ row }">
                {{ row.PermissionCount }} / {{ row.TotalCount }}
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" align="center">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSetProgramPermission(row)">設定作業權限</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <!-- 作業權限設定 -->
        <el-tab-pane label="作業權限" name="program" v-if="selectedMenuId">
          <div style="margin-bottom: 20px">
            <el-tag type="info">選單: {{ selectedMenuName }}</el-tag>
            <el-button type="primary" style="margin-left: 20px" @click="handleBackToMenu">返回選單列表</el-button>
          </div>
          <el-table :data="programList" border stripe style="width: 100%" v-loading="programLoading">
            <el-table-column type="index" label="序號" width="80" align="center" />
            <el-table-column prop="ProgramId" label="作業代碼" width="150" />
            <el-table-column prop="ProgramName" label="作業名稱" width="200" />
            <el-table-column label="權限統計" width="150" align="center">
              <template #default="{ row }">
                {{ row.PermissionCount }} / {{ row.TotalCount }}
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" align="center">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSetButtonPermission(row)">設定按鈕權限</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <!-- 按鈕權限設定 -->
        <el-tab-pane label="按鈕權限" name="button" v-if="selectedProgramId">
          <div style="margin-bottom: 20px">
            <el-tag type="info">作業: {{ selectedProgramName }}</el-tag>
            <el-button type="primary" style="margin-left: 20px" @click="handleBackToProgram">返回作業列表</el-button>
          </div>
          <el-table :data="buttonList" border stripe style="width: 100%" v-loading="buttonLoading">
            <el-table-column type="index" label="序號" width="80" align="center" />
            <el-table-column prop="ButtonId" label="按鈕代碼" width="150" />
            <el-table-column prop="ButtonName" label="按鈕名稱" width="200" />
            <el-table-column label="是否授權" width="120" align="center">
              <template #default="{ row }">
                <el-tag :type="row.IsAuthorized ? 'success' : 'info'">
                  {{ row.IsAuthorized ? '是' : '否' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" align="center">
              <template #default="{ row }">
                <el-button 
                  :type="row.IsAuthorized ? 'danger' : 'success'" 
                  size="small" 
                  @click="handleToggleButtonPermission(row)"
                >
                  {{ row.IsAuthorized ? '取消授權' : '授權' }}
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <!-- 權限列表 -->
        <el-tab-pane label="權限列表" name="list">
          <el-form :inline="true" style="margin-bottom: 20px">
            <el-form-item label="系統代碼">
              <el-input v-model="permissionQuery.SystemId" placeholder="請輸入系統代碼" clearable style="width: 150px" />
            </el-form-item>
            <el-form-item label="選單代碼">
              <el-input v-model="permissionQuery.MenuId" placeholder="請輸入選單代碼" clearable style="width: 150px" />
            </el-form-item>
            <el-form-item label="作業代碼">
              <el-input v-model="permissionQuery.ProgramId" placeholder="請輸入作業代碼" clearable style="width: 150px" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handlePermissionSearch">查詢</el-button>
              <el-button @click="handlePermissionReset">重置</el-button>
            </el-form-item>
          </el-form>
          <el-table
            :data="permissionList"
            v-loading="permissionLoading"
            border
            stripe
            style="width: 100%"
            @selection-change="handlePermissionSelectionChange"
          >
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="SystemName" label="系統" width="150" />
            <el-table-column prop="MenuName" label="選單" width="150" />
            <el-table-column prop="ProgramName" label="作業" width="200" />
            <el-table-column prop="ButtonId" label="按鈕代碼" width="120" />
            <el-table-column prop="ButtonName" label="按鈕名稱" width="150" />
            <el-table-column prop="CreatedBy" label="建立者" width="100" />
            <el-table-column prop="CreatedAt" label="建立時間" width="160">
              <template #default="{ row }">
                {{ formatDateTime(row.CreatedAt) }}
              </template>
            </el-table-column>
            <el-table-column label="操作" width="100" fixed="right">
              <template #default="{ row }">
                <el-button type="danger" size="small" @click="handleDeletePermission(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="permissionPagination.PageIndex"
            v-model:page-size="permissionPagination.PageSize"
            :total="permissionPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handlePermissionSizeChange"
            @current-change="handlePermissionPageChange"
            style="margin-top: 20px; text-align: right"
          />
          <div style="margin-top: 10px">
            <el-button type="danger" @click="handleBatchDeletePermission" :disabled="selectedPermissions.length === 0">
              批次刪除
            </el-button>
          </div>
        </el-tab-pane>
      </el-tabs>
    </el-card>

    <!-- 新增/修改項目對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form :model="itemForm" :rules="itemFormRules" ref="itemFormRef" label-width="100px">
        <el-form-item label="項目代碼" prop="ItemId">
          <el-input v-model="itemForm.ItemId" :disabled="isEdit" placeholder="請輸入項目代碼" />
        </el-form-item>
        <el-form-item label="項目名稱" prop="ItemName">
          <el-input v-model="itemForm.ItemName" placeholder="請輸入項目名稱" />
        </el-form-item>
        <el-form-item label="項目類型" prop="ItemType">
          <el-input v-model="itemForm.ItemType" placeholder="請輸入項目類型" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="itemForm.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="itemForm.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { itemCorrespondsApi, itemPermissionsApi } from '@/api/permissions'

// 查詢表單
const queryForm = reactive({
  ItemId: '',
  ItemName: '',
  ItemType: '',
  Status: ''
})

// 項目列表
const itemList = ref([])
const loading = ref(false)
const selectedItemId = ref('')
const selectedItemName = ref('')

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 權限設定相關
const activeTab = ref('system')
const systemList = ref([])
const systemLoading = ref(false)
const selectedSystemId = ref('')
const selectedSystemName = ref('')
const menuList = ref([])
const menuLoading = ref(false)
const selectedMenuId = ref('')
const selectedMenuName = ref('')
const programList = ref([])
const programLoading = ref(false)
const selectedProgramId = ref('')
const selectedProgramName = ref('')
const buttonList = ref([])
const buttonLoading = ref(false)

// 權限列表
const permissionList = ref([])
const permissionLoading = ref(false)
const selectedPermissions = ref([])
const permissionQuery = reactive({
  SystemId: '',
  MenuId: '',
  ProgramId: ''
})
const permissionPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增項目')
const isEdit = ref(false)
const itemFormRef = ref(null)
const itemForm = reactive({
  ItemId: '',
  ItemName: '',
  ItemType: '',
  Status: '1',
  Notes: ''
})
const itemFormRules = {
  ItemId: [{ required: true, message: '請輸入項目代碼', trigger: 'blur' }],
  ItemName: [{ required: true, message: '請輸入項目名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 載入項目列表
const loadItemList = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ItemId: queryForm.ItemId || undefined,
      ItemName: queryForm.ItemName || undefined,
      ItemType: queryForm.ItemType || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await itemCorrespondsApi.getItemCorresponds(params)
    if (response && response.Data) {
      itemList.value = response.Data.Items || []
      pagination.TotalCount = response.Data.TotalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 載入系統列表
const loadSystemList = async () => {
  if (!selectedItemId.value) return
  systemLoading.value = true
  try {
    const response = await itemPermissionsApi.getSystemList(selectedItemId.value)
    if (response && response.Data) {
      systemList.value = response.Data || []
    }
  } catch (error) {
    ElMessage.error('載入系統列表失敗: ' + (error.message || '未知錯誤'))
  } finally {
    systemLoading.value = false
  }
}

// 載入選單列表
const loadMenuList = async () => {
  if (!selectedItemId.value || !selectedSystemId.value) return
  menuLoading.value = true
  try {
    const response = await itemPermissionsApi.getMenuList(selectedItemId.value, selectedSystemId.value)
    if (response && response.Data) {
      menuList.value = response.Data || []
    }
  } catch (error) {
    ElMessage.error('載入選單列表失敗: ' + (error.message || '未知錯誤'))
  } finally {
    menuLoading.value = false
  }
}

// 載入作業列表
const loadProgramList = async () => {
  if (!selectedItemId.value || !selectedMenuId.value) return
  programLoading.value = true
  try {
    const response = await itemPermissionsApi.getProgramList(selectedItemId.value, selectedMenuId.value)
    if (response && response.Data) {
      programList.value = response.Data || []
    }
  } catch (error) {
    ElMessage.error('載入作業列表失敗: ' + (error.message || '未知錯誤'))
  } finally {
    programLoading.value = false
  }
}

// 載入按鈕列表
const loadButtonList = async () => {
  if (!selectedItemId.value || !selectedProgramId.value) return
  buttonLoading.value = true
  try {
    const response = await itemPermissionsApi.getButtonList(selectedItemId.value, selectedProgramId.value)
    if (response && response.Data) {
      buttonList.value = response.Data || []
    }
  } catch (error) {
    ElMessage.error('載入按鈕列表失敗: ' + (error.message || '未知錯誤'))
  } finally {
    buttonLoading.value = false
  }
}

// 載入權限列表
const loadPermissionList = async () => {
  if (!selectedItemId.value) return
  permissionLoading.value = true
  try {
    const params = {
      PageIndex: permissionPagination.PageIndex,
      PageSize: permissionPagination.PageSize,
      SystemId: permissionQuery.SystemId || undefined,
      MenuId: permissionQuery.MenuId || undefined,
      ProgramId: permissionQuery.ProgramId || undefined
    }
    const response = await itemPermissionsApi.getItemPermissions(selectedItemId.value, params)
    if (response && response.Data) {
      permissionList.value = response.Data.Items || []
      permissionPagination.TotalCount = response.Data.TotalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢權限列表失敗: ' + (error.message || '未知錯誤'))
  } finally {
    permissionLoading.value = false
  }
}

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadItemList()
}

// 重置
const handleReset = () => {
  Object.assign(queryForm, {
    ItemId: '',
    ItemName: '',
    ItemType: '',
    Status: ''
  })
  pagination.PageIndex = 1
  loadItemList()
}

// 新增項目
const handleAdd = () => {
  dialogTitle.value = '新增項目'
  isEdit.value = false
  Object.assign(itemForm, {
    ItemId: '',
    ItemName: '',
    ItemType: '',
    Status: '1',
    Notes: ''
  })
  dialogVisible.value = true
}

// 修改項目
const handleEdit = (row) => {
  dialogTitle.value = '修改項目'
  isEdit.value = true
  Object.assign(itemForm, {
    ItemId: row.ItemId,
    ItemName: row.ItemName,
    ItemType: row.ItemType || '',
    Status: row.Status || '1',
    Notes: row.Notes || ''
  })
  dialogVisible.value = true
}

// 刪除項目
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(`確定要刪除項目「${row.ItemName}」嗎？刪除後將級聯刪除相關權限。`, '確認刪除', {
      type: 'warning'
    })
    await itemCorrespondsApi.deleteItemCorrespond(row.ItemId)
    ElMessage.success('刪除成功')
    loadItemList()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 設定權限
const handleSetPermission = (row) => {
  selectedItemId.value = row.ItemId
  selectedItemName.value = row.ItemName
  activeTab.value = 'system'
  loadSystemList()
  loadPermissionList()
}

// 返回列表
const handleBackToList = () => {
  selectedItemId.value = ''
  selectedItemName.value = ''
  selectedSystemId.value = ''
  selectedSystemId.value = ''
  selectedMenuId.value = ''
  selectedProgramId.value = ''
  activeTab.value = 'system'
}

// 系統狀態變更
const handleSystemStatusChange = (row) => {
  // 狀態變更邏輯，實際保存時處理
}

// 儲存系統權限設定
const handleSaveSystemPermissions = async () => {
  try {
    const systemIds = systemList.value
      .filter(s => s.Status === '全選' || s.Status === '未選')
      .map(s => s.SystemId)
    const grantSystemIds = systemList.value
      .filter(s => s.Status === '全選')
      .map(s => s.SystemId)
    const revokeSystemIds = systemList.value
      .filter(s => s.Status === '未選')
      .map(s => s.SystemId)

    if (grantSystemIds.length > 0) {
      await itemPermissionsApi.setSystemPermissions(selectedItemId.value, {
        SystemIds: grantSystemIds,
        Action: 'grant'
      })
    }
    if (revokeSystemIds.length > 0) {
      await itemPermissionsApi.setSystemPermissions(selectedItemId.value, {
        SystemIds: revokeSystemIds,
        Action: 'revoke'
      })
    }
    ElMessage.success('儲存成功')
    loadSystemList()
    loadPermissionList()
  } catch (error) {
    ElMessage.error('儲存失敗: ' + (error.message || '未知錯誤'))
  }
}

// 設定選單權限
const handleSetMenuPermission = (row) => {
  selectedSystemId.value = row.SystemId
  selectedSystemName.value = row.SystemName
  activeTab.value = 'menu'
  loadMenuList()
}

// 返回系統列表
const handleBackToSystem = () => {
  selectedSystemId.value = ''
  selectedSystemName.value = ''
  selectedMenuId.value = ''
  selectedProgramId.value = ''
  activeTab.value = 'system'
}

// 設定作業權限
const handleSetProgramPermission = (row) => {
  selectedMenuId.value = row.MenuId
  selectedMenuName.value = row.MenuName
  activeTab.value = 'program'
  loadProgramList()
}

// 返回選單列表
const handleBackToMenu = () => {
  selectedMenuId.value = ''
  selectedMenuName.value = ''
  selectedProgramId.value = ''
  activeTab.value = 'menu'
}

// 設定按鈕權限
const handleSetButtonPermission = (row) => {
  selectedProgramId.value = row.ProgramId
  selectedProgramName.value = row.ProgramName
  activeTab.value = 'button'
  loadButtonList()
}

// 返回作業列表
const handleBackToProgram = () => {
  selectedProgramId.value = ''
  selectedProgramName.value = ''
  activeTab.value = 'program'
}

// 切換按鈕權限
const handleToggleButtonPermission = async (row) => {
  try {
    const action = row.IsAuthorized ? 'revoke' : 'grant'
    await itemPermissionsApi.setButtonPermissions(selectedItemId.value, {
      ButtonKeys: [row.ButtonKey],
      Action: action
    })
    ElMessage.success('設定成功')
    loadButtonList()
    loadPermissionList()
  } catch (error) {
    ElMessage.error('設定失敗: ' + (error.message || '未知錯誤'))
  }
}

// 權限查詢
const handlePermissionSearch = () => {
  permissionPagination.PageIndex = 1
  loadPermissionList()
}

// 權限重置
const handlePermissionReset = () => {
  Object.assign(permissionQuery, {
    SystemId: '',
    MenuId: '',
    ProgramId: ''
  })
  permissionPagination.PageIndex = 1
  loadPermissionList()
}

// 刪除權限
const handleDeletePermission = async (row) => {
  try {
    await ElMessageBox.confirm(`確定要刪除權限「${row.ButtonName}」嗎？`, '確認刪除', {
      type: 'warning'
    })
    await itemPermissionsApi.deleteItemPermission(selectedItemId.value, row.TKey)
    ElMessage.success('刪除成功')
    loadPermissionList()
    loadSystemList()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 批次刪除權限
const handleBatchDeletePermission = async () => {
  if (selectedPermissions.value.length === 0) {
    ElMessage.warning('請選擇要刪除的資料')
    return
  }
  try {
    await ElMessageBox.confirm(`確定要刪除選取的 ${selectedPermissions.value.length} 筆資料嗎？`, '確認批次刪除', {
      type: 'warning'
    })
    const tKeys = selectedPermissions.value.map(row => row.TKey)
    await itemPermissionsApi.batchDeleteItemPermissions(selectedItemId.value, tKeys)
    ElMessage.success('批次刪除成功')
    selectedPermissions.value = []
    loadPermissionList()
    loadSystemList()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 權限選擇變更
const handlePermissionSelectionChange = (selection) => {
  selectedPermissions.value = selection
}

// 提交表單
const handleSubmit = async () => {
  if (!itemFormRef.value) return
  await itemFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        if (isEdit.value) {
          await itemCorrespondsApi.updateItemCorrespond(itemForm.ItemId, itemForm)
          ElMessage.success('修改成功')
        } else {
          await itemCorrespondsApi.createItemCorrespond(itemForm)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadItemList()
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  if (itemFormRef.value) {
    itemFormRef.value.resetFields()
  }
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
  loadItemList()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadItemList()
}

// 權限分頁大小變更
const handlePermissionSizeChange = (size) => {
  permissionPagination.PageSize = size
  permissionPagination.PageIndex = 1
  loadPermissionList()
}

// 權限分頁變更
const handlePermissionPageChange = (page) => {
  permissionPagination.PageIndex = page
  loadPermissionList()
}

// 初始化
onMounted(() => {
  loadItemList()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0360-query {
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

  .permission-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .item-info {
    padding: 15px;
    background-color: $card-bg;
    border-radius: 4px;
    margin-bottom: 20px;
  }
}
</style>

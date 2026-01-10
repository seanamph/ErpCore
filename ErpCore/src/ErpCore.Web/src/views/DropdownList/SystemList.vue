<template>
  <div class="system-list">
    <div class="page-header">
      <h1>系統列表 (SYSID_LIST, USER_LIST)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane label="系統列表" name="system">
          <el-form :inline="true" :model="systemQueryForm" class="search-form">
            <el-form-item label="系統名稱">
              <el-input v-model="systemQueryForm.SystemName" placeholder="請輸入系統名稱" clearable />
            </el-form-item>
            <el-form-item label="系統代碼">
              <el-input v-model="systemQueryForm.SystemId" placeholder="請輸入系統代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSystemSearch">查詢</el-button>
              <el-button @click="handleSystemReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 系統列表 -->
          <el-table
            :data="systemList"
            v-loading="systemLoading"
            border
            stripe
            highlight-current-row
            @row-click="handleSystemRowClick"
            style="width: 100%; cursor: pointer"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="SystemId" label="系統代碼" width="120" />
            <el-table-column prop="SystemName" label="系統名稱" min-width="200" />
            <el-table-column prop="SystemType" label="系統類型" width="120" />
            <el-table-column prop="Description" label="說明" min-width="200" show-overflow-tooltip />
            <el-table-column prop="Status" label="狀態" width="80" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === '1' ? 'success' : 'danger'" size="small">
                  {{ row.Status === '1' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <el-tab-pane label="使用者列表" name="user">
          <el-form :inline="true" :model="userQueryForm" class="search-form">
            <el-form-item label="使用者名稱">
              <el-input v-model="userQueryForm.UserName" placeholder="請輸入使用者名稱" clearable />
            </el-form-item>
            <el-form-item label="使用者代碼">
              <el-input v-model="userQueryForm.UserId" placeholder="請輸入使用者代碼" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="userQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUserSearch">查詢</el-button>
              <el-button @click="handleUserReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 使用者列表 -->
          <el-table
            :data="userList"
            v-loading="userLoading"
            border
            stripe
            highlight-current-row
            @row-click="handleUserRowClick"
            style="width: 100%; cursor: pointer"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="UserId" label="使用者代碼" width="120" />
            <el-table-column prop="UserName" label="使用者名稱" min-width="200" />
            <el-table-column prop="Title" label="職稱" width="150" />
            <el-table-column prop="OrgId" label="組織代碼" width="120" />
            <el-table-column prop="Status" label="狀態" width="80" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'" size="small">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分頁 -->
          <el-pagination
            v-model:current-page="userPagination.PageIndex"
            v-model:page-size="userPagination.PageSize"
            :total="userPagination.TotalCount"
            :page-sizes="[20, 50, 100, 200]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleUserSizeChange"
            @current-change="handleUserPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

// 當前標籤
const activeTab = ref('system')

// 系統查詢表單
const systemQueryForm = reactive({
  SystemName: '',
  SystemId: ''
})

// 系統列表
const systemList = ref([])
const systemLoading = ref(false)

// 使用者查詢表單
const userQueryForm = reactive({
  UserName: '',
  UserId: '',
  Status: ''
})

// 使用者列表
const userList = ref([])
const userLoading = ref(false)
const userPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 載入系統列表
const loadSystemList = async () => {
  systemLoading.value = true
  try {
    const params = {
      SystemName: systemQueryForm.SystemName || undefined,
      SystemId: systemQueryForm.SystemId || undefined
    }
    const response = await dropdownListApi.getSystems(params)
    if (response.data?.success) {
      systemList.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    systemLoading.value = false
  }
}

// 載入使用者列表
const loadUserList = async () => {
  userLoading.value = true
  try {
    const params = {
      PageIndex: userPagination.PageIndex,
      PageSize: userPagination.PageSize,
      UserName: userQueryForm.UserName || undefined,
      UserId: userQueryForm.UserId || undefined,
      Status: userQueryForm.Status || undefined
    }
    const response = await dropdownListApi.getUsers(params)
    if (response.data?.success) {
      userList.value = response.data.data?.Items || []
      userPagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    userLoading.value = false
  }
}

// 系統查詢
const handleSystemSearch = () => {
  loadSystemList()
}

// 系統重置
const handleSystemReset = () => {
  systemQueryForm.SystemName = ''
  systemQueryForm.SystemId = ''
  handleSystemSearch()
}

// 系統行點擊
const handleSystemRowClick = (row) => {
  ElMessage.success(`已選擇系統：${row.SystemName} (${row.SystemId})`)
}

// 使用者查詢
const handleUserSearch = () => {
  userPagination.PageIndex = 1
  loadUserList()
}

// 使用者重置
const handleUserReset = () => {
  userQueryForm.UserName = ''
  userQueryForm.UserId = ''
  userQueryForm.Status = ''
  handleUserSearch()
}

// 使用者行點擊
const handleUserRowClick = (row) => {
  ElMessage.success(`已選擇使用者：${row.UserName} (${row.UserId})`)
}

// 使用者分頁大小變更
const handleUserSizeChange = (size) => {
  userPagination.PageSize = size
  userPagination.PageIndex = 1
  loadUserList()
}

// 使用者分頁變更
const handleUserPageChange = (page) => {
  userPagination.PageIndex = page
  loadUserList()
}

// 標籤切換
const handleTabChange = (tabName) => {
  if (tabName === 'system' && systemList.value.length === 0) {
    loadSystemList()
  } else if (tabName === 'user' && userList.value.length === 0) {
    loadUserList()
  }
}

// 初始化
onMounted(() => {
  loadSystemList()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.system-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}
</style>


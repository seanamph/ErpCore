<template>
  <div class="change-logs">
    <div class="page-header">
      <h1>?��?記�??�詢 (SYS0610-SYS0660)</h1>
    </div>

    <!-- 標籤??-->
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 使用?�異?��???-->
      <el-tab-pane label="使用?�基?��??�異?�查�?(SYS0610)" name="users">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="userChangeLogQueryForm" class="search-form">
            <el-form-item label="使用?�代�?>
              <el-input v-model="userChangeLogQueryForm.UserId" placeholder="請輸?�使?�者代�? clearable />
            </el-form-item>
            <el-form-item label="?��??��?�?>
              <el-date-picker
                v-model="userChangeLogQueryForm.StartDate"
                type="date"
                placeholder="?��??��??��?"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item label="?��??��?�?>
              <el-date-picker
                v-model="userChangeLogQueryForm.EndDate"
                type="date"
                placeholder="?��?結�??��?"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUserChangeLogSearch">?�詢</el-button>
              <el-button @click="handleUserChangeLogReset">?�置</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="userChangeLogTableData"
            v-loading="userChangeLogLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="LogId" label="記�?ID" width="100" />
            <el-table-column prop="UserId" label="使用?�代�? width="150" />
            <el-table-column prop="UserName" label="使用?��?�? width="200" />
            <el-table-column prop="ChangeType" label="?��?類�?" width="120">
              <template #default="{ row }">
                <el-tag :type="getChangeTypeTagType(row.ChangeType)">
                  {{ getChangeTypeText(row.ChangeType) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="FieldName" label="欄�??�稱" width="150" />
            <el-table-column prop="OldValue" label="?��??��? min-width="150" show-overflow-tooltip />
            <el-table-column prop="NewValue" label="?��?後�? min-width="150" show-overflow-tooltip />
            <el-table-column prop="ChangedBy" label="?��??? width="120" />
            <el-table-column prop="ChangedAt" label="?��??��?" width="180">
              <template #default="{ row }">
                {{ row.ChangedAt ? new Date(row.ChangedAt).toLocaleString('zh-TW') : '' }}
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="userChangeLogPagination.PageIndex"
            v-model:page-size="userChangeLogPagination.PageSize"
            :total="userChangeLogPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleUserChangeLogSizeChange"
            @current-change="handleUserChangeLogPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 角色?��?記�? -->
      <el-tab-pane label="角色?�本資�??��??�詢 (SYS0620)" name="roles">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="roleChangeLogQueryForm" class="search-form">
            <el-form-item label="角色�?��">
              <el-input v-model="roleChangeLogQueryForm.RoleId" placeholder="請輸?��??�代�? clearable />
            </el-form-item>
            <el-form-item label="?��??��?�?>
              <el-date-picker
                v-model="roleChangeLogQueryForm.StartDate"
                type="date"
                placeholder="?��??��??��?"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item label="?��??��?�?>
              <el-date-picker
                v-model="roleChangeLogQueryForm.EndDate"
                type="date"
                placeholder="?��?結�??��?"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleRoleChangeLogSearch">?�詢</el-button>
              <el-button @click="handleRoleChangeLogReset">?�置</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="roleChangeLogTableData"
            v-loading="roleChangeLogLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="LogId" label="記�?ID" width="100" />
            <el-table-column prop="RoleId" label="角色�?��" width="150" />
            <el-table-column prop="RoleName" label="角色?�稱" width="200" />
            <el-table-column prop="ChangeType" label="?��?類�?" width="120">
              <template #default="{ row }">
                <el-tag :type="getChangeTypeTagType(row.ChangeType)">
                  {{ getChangeTypeText(row.ChangeType) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="FieldName" label="欄�??�稱" width="150" />
            <el-table-column prop="OldValue" label="?��??��? min-width="150" show-overflow-tooltip />
            <el-table-column prop="NewValue" label="?��?後�? min-width="150" show-overflow-tooltip />
            <el-table-column prop="ChangedBy" label="?��??? width="120" />
            <el-table-column prop="ChangedAt" label="?��??��?" width="180">
              <template #default="{ row }">
                {{ row.ChangedAt ? new Date(row.ChangedAt).toLocaleString('zh-TW') : '' }}
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="roleChangeLogPagination.PageIndex"
            v-model:page-size="roleChangeLogPagination.PageSize"
            :total="roleChangeLogPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleRoleChangeLogSizeChange"
            @current-change="handleRoleChangeLogPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 系統權�??��?記�? -->
      <el-tab-pane label="系統權�??��?記�??�詢 (SYS0640)" name="system-permissions">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="systemPermissionChangeLogQueryForm" class="search-form">
            <el-form-item label="系統�?��">
              <el-input v-model="systemPermissionChangeLogQueryForm.SystemId" placeholder="請輸?�系統代�? clearable />
            </el-form-item>
            <el-form-item label="?��??��?�?>
              <el-date-picker
                v-model="systemPermissionChangeLogQueryForm.StartDate"
                type="date"
                placeholder="?��??��??��?"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item label="?��??��?�?>
              <el-date-picker
                v-model="systemPermissionChangeLogQueryForm.EndDate"
                type="date"
                placeholder="?��?結�??��?"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSystemPermissionChangeLogSearch">?�詢</el-button>
              <el-button @click="handleSystemPermissionChangeLogReset">?�置</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="systemPermissionChangeLogTableData"
            v-loading="systemPermissionChangeLogLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="LogId" label="記�?ID" width="100" />
            <el-table-column prop="SystemId" label="系統�?��" width="150" />
            <el-table-column prop="SystemName" label="系統?�稱" width="200" />
            <el-table-column prop="ChangeType" label="?��?類�?" width="120">
              <template #default="{ row }">
                <el-tag :type="getChangeTypeTagType(row.ChangeType)">
                  {{ getChangeTypeText(row.ChangeType) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="FieldName" label="欄�??�稱" width="150" />
            <el-table-column prop="OldValue" label="?��??��? min-width="150" show-overflow-tooltip />
            <el-table-column prop="NewValue" label="?��?後�? min-width="150" show-overflow-tooltip />
            <el-table-column prop="ChangedBy" label="?��??? width="120" />
            <el-table-column prop="ChangedAt" label="?��??��?" width="180">
              <template #default="{ row }">
                {{ row.ChangedAt ? new Date(row.ChangedAt).toLocaleString('zh-TW') : '' }}
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="systemPermissionChangeLogPagination.PageIndex"
            v-model:page-size="systemPermissionChangeLogPagination.PageSize"
            :total="systemPermissionChangeLogPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSystemPermissionChangeLogSizeChange"
            @current-change="handleSystemPermissionChangeLogPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { changeLogsApi } from '@/api/changeLogs'

// 標籤??
const activeTab = ref('users')

// 使用?�異?��??�相??
const userChangeLogQueryForm = reactive({
  UserId: '',
  StartDate: '',
  EndDate: ''
})
const userChangeLogTableData = ref([])
const userChangeLogLoading = ref(false)
const userChangeLogPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 角色?��?記�??��?
const roleChangeLogQueryForm = reactive({
  RoleId: '',
  StartDate: '',
  EndDate: ''
})
const roleChangeLogTableData = ref([])
const roleChangeLogLoading = ref(false)
const roleChangeLogPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 系統權�??��?記�??��?
const systemPermissionChangeLogQueryForm = reactive({
  SystemId: '',
  StartDate: '',
  EndDate: ''
})
const systemPermissionChangeLogTableData = ref([])
const systemPermissionChangeLogLoading = ref(false)
const systemPermissionChangeLogPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 標籤?��???
const handleTabChange = (tabName) => {
  if (tabName === 'users') {
    handleUserChangeLogSearch()
  } else if (tabName === 'roles') {
    handleRoleChangeLogSearch()
  } else if (tabName === 'system-permissions') {
    handleSystemPermissionChangeLogSearch()
  }
}

// ?��?類�?標籤類�?
const getChangeTypeTagType = (changeType) => {
  const typeMap = {
    'INSERT': 'success',
    'UPDATE': 'warning',
    'DELETE': 'danger'
  }
  return typeMap[changeType] || 'info'
}

// ?��?類�??��?
const getChangeTypeText = (changeType) => {
  const textMap = {
    'INSERT': '?��?',
    'UPDATE': '修改',
    'DELETE': '?�除'
  }
  return textMap[changeType] || changeType
}

// ?�詢使用?�異?��???
const handleUserChangeLogSearch = async () => {
  userChangeLogLoading.value = true
  try {
    const data = {
      PageIndex: userChangeLogPagination.PageIndex,
      PageSize: userChangeLogPagination.PageSize,
      UserId: userChangeLogQueryForm.UserId || undefined,
      StartDate: userChangeLogQueryForm.StartDate || undefined,
      EndDate: userChangeLogQueryForm.EndDate || undefined
    }
    const response = await changeLogsApi.getUserChangeLogs(data)
    if (response.data.Success) {
      userChangeLogTableData.value = response.data.Data.Items || []
      userChangeLogPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢使用?�異?��??�失??', error)
    ElMessage.error('?�詢使用?�異?��??�失??)
  } finally {
    userChangeLogLoading.value = false
  }
}

const handleUserChangeLogReset = () => {
  userChangeLogQueryForm.UserId = ''
  userChangeLogQueryForm.StartDate = ''
  userChangeLogQueryForm.EndDate = ''
  userChangeLogPagination.PageIndex = 1
  handleUserChangeLogSearch()
}

// ?�詢角色?��?記�?
const handleRoleChangeLogSearch = async () => {
  roleChangeLogLoading.value = true
  try {
    const data = {
      PageIndex: roleChangeLogPagination.PageIndex,
      PageSize: roleChangeLogPagination.PageSize,
      RoleId: roleChangeLogQueryForm.RoleId || undefined,
      StartDate: roleChangeLogQueryForm.StartDate || undefined,
      EndDate: roleChangeLogQueryForm.EndDate || undefined
    }
    const response = await changeLogsApi.getRoleChangeLogs(data)
    if (response.data.Success) {
      roleChangeLogTableData.value = response.data.Data.Items || []
      roleChangeLogPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢角色?��?記�?失�?:', error)
    ElMessage.error('?�詢角色?��?記�?失�?')
  } finally {
    roleChangeLogLoading.value = false
  }
}

const handleRoleChangeLogReset = () => {
  roleChangeLogQueryForm.RoleId = ''
  roleChangeLogQueryForm.StartDate = ''
  roleChangeLogQueryForm.EndDate = ''
  roleChangeLogPagination.PageIndex = 1
  handleRoleChangeLogSearch()
}

// ?�詢系統權�??��?記�?
const handleSystemPermissionChangeLogSearch = async () => {
  systemPermissionChangeLogLoading.value = true
  try {
    const data = {
      PageIndex: systemPermissionChangeLogPagination.PageIndex,
      PageSize: systemPermissionChangeLogPagination.PageSize,
      SystemId: systemPermissionChangeLogQueryForm.SystemId || undefined,
      StartDate: systemPermissionChangeLogQueryForm.StartDate || undefined,
      EndDate: systemPermissionChangeLogQueryForm.EndDate || undefined
    }
    const response = await changeLogsApi.getSystemPermissionChangeLogs(data)
    if (response.data.Success) {
      systemPermissionChangeLogTableData.value = response.data.Data.Items || []
      systemPermissionChangeLogPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢系統權�??��?記�?失�?:', error)
    ElMessage.error('?�詢系統權�??��?記�?失�?')
  } finally {
    systemPermissionChangeLogLoading.value = false
  }
}

const handleSystemPermissionChangeLogReset = () => {
  systemPermissionChangeLogQueryForm.SystemId = ''
  systemPermissionChangeLogQueryForm.StartDate = ''
  systemPermissionChangeLogQueryForm.EndDate = ''
  systemPermissionChangeLogPagination.PageIndex = 1
  handleSystemPermissionChangeLogSearch()
}

// ?��?變更
const handleUserChangeLogSizeChange = (size) => {
  userChangeLogPagination.PageSize = size
  userChangeLogPagination.PageIndex = 1
  handleUserChangeLogSearch()
}

const handleUserChangeLogPageChange = (page) => {
  userChangeLogPagination.PageIndex = page
  handleUserChangeLogSearch()
}

const handleRoleChangeLogSizeChange = (size) => {
  roleChangeLogPagination.PageSize = size
  roleChangeLogPagination.PageIndex = 1
  handleRoleChangeLogSearch()
}

const handleRoleChangeLogPageChange = (page) => {
  roleChangeLogPagination.PageIndex = page
  handleRoleChangeLogSearch()
}

const handleSystemPermissionChangeLogSizeChange = (size) => {
  systemPermissionChangeLogPagination.PageSize = size
  systemPermissionChangeLogPagination.PageIndex = 1
  handleSystemPermissionChangeLogSearch()
}

const handleSystemPermissionChangeLogPageChange = (page) => {
  systemPermissionChangeLogPagination.PageIndex = page
  handleSystemPermissionChangeLogSearch()
}

// ?��???
onMounted(() => {
  handleUserChangeLogSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
.change-logs {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  color: $primary-color;
  font-size: 24px;
  font-weight: bold;
}

.search-card {
  margin-bottom: 20px;
}

.table-card {
  margin-top: 20px;
}
</style>


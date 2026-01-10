<template>
  <div class="permissions">
    <div class="page-header">
      <h1>權�?管�? (SYS0310, SYS0320, SYS0330, SYS0340)</h1>
    </div>

    <!-- 標籤??-->
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 角色權�?設�? -->
      <el-tab-pane label="角色系統權�?設�? (SYS0310)" name="role">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="roleQueryForm" class="search-form">
            <el-form-item label="角色�?��">
              <el-select
                v-model="roleQueryForm.RoleId"
                placeholder="請選?��???
                filterable
                clearable
                style="width: 200px"
                @change="handleRoleChange"
              >
                <el-option
                  v-for="role in roleList"
                  :key="role.RoleId"
                  :label="`${role.RoleId} - ${role.RoleName}`"
                  :value="role.RoleId"
                />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleRolePermissionSearch">?�詢</el-button>
              <el-button @click="handleRolePermissionReset">?�置</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never" v-if="selectedRoleId">
          <div style="margin-bottom: 15px">
            <el-button type="success" @click="handleBatchSetRoleSystemPermissions">?��?設�?系統權�?</el-button>
            <el-button type="success" @click="handleBatchSetRoleMenuPermissions">?��?設�??�單權�?</el-button>
            <el-button type="success" @click="handleBatchSetRoleProgramPermissions">?��?設�?作業權�?</el-button>
            <el-button type="success" @click="handleBatchSetRoleButtonPermissions">?��?設�??��?權�?</el-button>
          </div>
          <el-table
            :data="rolePermissionTableData"
            v-loading="rolePermissionLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="SystemId" label="系統�?��" width="120" />
            <el-table-column prop="SystemName" label="系統?�稱" width="200" />
            <el-table-column prop="MenuId" label="?�單�?��" width="120" />
            <el-table-column prop="MenuName" label="?�單?�稱" width="200" />
            <el-table-column prop="ProgramId" label="作業�?��" width="120" />
            <el-table-column prop="ProgramName" label="作業?�稱" width="200" />
            <el-table-column prop="ButtonId" label="?��?�?��" width="120" />
            <el-table-column prop="ButtonName" label="?��??�稱" width="200" />
            <el-table-column label="?��?" width="150" fixed="right">
              <template #default="{ row }">
                <el-button type="danger" size="small" @click="handleDeleteRolePermission(row)">?�除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="rolePermissionPagination.PageIndex"
            v-model:page-size="rolePermissionPagination.PageSize"
            :total="rolePermissionPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleRolePermissionSizeChange"
            @current-change="handleRolePermissionPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 使用?��??�設�?-->
      <el-tab-pane label="使用?�系統�??�設�?(SYS0320)" name="user">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="userQueryForm" class="search-form">
            <el-form-item label="使用?�代�?>
              <el-select
                v-model="userQueryForm.UserId"
                placeholder="請選?�使?��?
                filterable
                clearable
                style="width: 200px"
                @change="handleUserChange"
              >
                <el-option
                  v-for="user in userList"
                  :key="user.UserId"
                  :label="`${user.UserId} - ${user.UserName}`"
                  :value="user.UserId"
                />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUserPermissionSearch">?�詢</el-button>
              <el-button @click="handleUserPermissionReset">?�置</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never" v-if="selectedUserId">
          <div style="margin-bottom: 15px">
            <el-button type="success" @click="handleBatchSetUserSystemPermissions">?��?設�?系統權�?</el-button>
            <el-button type="success" @click="handleBatchSetUserMenuPermissions">?��?設�??�單權�?</el-button>
            <el-button type="success" @click="handleBatchSetUserProgramPermissions">?��?設�?作業權�?</el-button>
            <el-button type="success" @click="handleBatchSetUserButtonPermissions">?��?設�??��?權�?</el-button>
          </div>
          <el-table
            :data="userPermissionTableData"
            v-loading="userPermissionLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="SystemId" label="系統�?��" width="120" />
            <el-table-column prop="SystemName" label="系統?�稱" width="200" />
            <el-table-column prop="MenuId" label="?�單�?��" width="120" />
            <el-table-column prop="MenuName" label="?�單?�稱" width="200" />
            <el-table-column prop="ProgramId" label="作業�?��" width="120" />
            <el-table-column prop="ProgramName" label="作業?�稱" width="200" />
            <el-table-column prop="ButtonId" label="?��?�?��" width="120" />
            <el-table-column prop="ButtonName" label="?��??�稱" width="200" />
            <el-table-column label="?��?" width="150" fixed="right">
              <template #default="{ row }">
                <el-button type="danger" size="small" @click="handleDeleteUserPermission(row)">?�除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="userPermissionPagination.PageIndex"
            v-model:page-size="userPermissionPagination.PageSize"
            :total="userPermissionPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleUserPermissionSizeChange"
            @current-change="handleUserPermissionPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { rolePermissionsApi, userPermissionsApi } from '@/api/permissions'
import { rolesApi } from '@/api/roles'
import { usersApi } from '@/api/users'

// 標籤??
const activeTab = ref('role')

// 角色?��?
const roleList = ref([])
const selectedRoleId = ref('')
const roleQueryForm = reactive({
  RoleId: ''
})
const rolePermissionTableData = ref([])
const rolePermissionLoading = ref(false)
const rolePermissionPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 使用?�相??
const userList = ref([])
const selectedUserId = ref('')
const userQueryForm = reactive({
  UserId: ''
})
const userPermissionTableData = ref([])
const userPermissionLoading = ref(false)
const userPermissionPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 載入角色?�表
const loadRoleList = async () => {
  try {
    const response = await rolesApi.getRoles({ PageIndex: 1, PageSize: 1000 })
    if (response.data.Success) {
      roleList.value = response.data.Data.Items || []
    }
  } catch (error) {
    console.error('載入角色?�表失�?:', error)
  }
}

// 載入使用?��?�?
const loadUserList = async () => {
  try {
    const response = await usersApi.getUsers({ PageIndex: 1, PageSize: 1000 })
    if (response.data.Success) {
      userList.value = response.data.Data.Items || []
    }
  } catch (error) {
    console.error('載入使用?��?表失??', error)
  }
}

// 標籤?��???
const handleTabChange = (tabName) => {
  if (tabName === 'role') {
    if (selectedRoleId.value) {
      handleRolePermissionSearch()
    }
  } else if (tabName === 'user') {
    if (selectedUserId.value) {
      handleUserPermissionSearch()
    }
  }
}

// 角色?��?變更
const handleRoleChange = (roleId) => {
  selectedRoleId.value = roleId || ''
  if (roleId) {
    handleRolePermissionSearch()
  } else {
    rolePermissionTableData.value = []
  }
}

// ?�詢角色權�?
const handleRolePermissionSearch = async () => {
  if (!selectedRoleId.value) {
    ElMessage.warning('請選?��???)
    return
  }
  rolePermissionLoading.value = true
  try {
    const params = {
      PageIndex: rolePermissionPagination.PageIndex,
      PageSize: rolePermissionPagination.PageSize
    }
    const response = await rolePermissionsApi.getRolePermissions(selectedRoleId.value, params)
    if (response.data.Success) {
      rolePermissionTableData.value = response.data.Data.Items || []
      rolePermissionPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢角色權�?失�?:', error)
    ElMessage.error('?�詢角色權�?失�?')
  } finally {
    rolePermissionLoading.value = false
  }
}

// ?�置角色權�??�詢
const handleRolePermissionReset = () => {
  roleQueryForm.RoleId = ''
  selectedRoleId.value = ''
  rolePermissionTableData.value = []
  rolePermissionPagination.PageIndex = 1
  rolePermissionPagination.TotalCount = 0
}

// 使用?�選?��???
const handleUserChange = (userId) => {
  selectedUserId.value = userId || ''
  if (userId) {
    handleUserPermissionSearch()
  } else {
    userPermissionTableData.value = []
  }
}

// ?�詢使用?��???
const handleUserPermissionSearch = async () => {
  if (!selectedUserId.value) {
    ElMessage.warning('請選?�使?��?)
    return
  }
  userPermissionLoading.value = true
  try {
    const params = {
      PageIndex: userPermissionPagination.PageIndex,
      PageSize: userPermissionPagination.PageSize
    }
    const response = await userPermissionsApi.getUserPermissions(selectedUserId.value, params)
    if (response.data.Success) {
      userPermissionTableData.value = response.data.Data.Items || []
      userPermissionPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢使用?��??�失??', error)
    ElMessage.error('?�詢使用?��??�失??)
  } finally {
    userPermissionLoading.value = false
  }
}

// ?�置使用?��??�查�?
const handleUserPermissionReset = () => {
  userQueryForm.UserId = ''
  selectedUserId.value = ''
  userPermissionTableData.value = []
  userPermissionPagination.PageIndex = 1
  userPermissionPagination.TotalCount = 0
}

// ?��?設�?角色系統權�?
const handleBatchSetRoleSystemPermissions = () => {
  ElMessage.info('?��?設�?角色系統權�??�能?�發�?)
}

// ?��?設�?角色?�單權�?
const handleBatchSetRoleMenuPermissions = () => {
  ElMessage.info('?��?設�?角色?�單權�??�能?�發�?)
}

// ?��?設�?角色作業權�?
const handleBatchSetRoleProgramPermissions = () => {
  ElMessage.info('?��?設�?角色作業權�??�能?�發�?)
}

// ?��?設�?角色?��?權�?
const handleBatchSetRoleButtonPermissions = () => {
  ElMessage.info('?��?設�?角色?��?權�??�能?�發�?)
}

// ?�除角色權�?
const handleDeleteRolePermission = async (row) => {
  try {
    await ElMessageBox.confirm('確�?要刪?�此權�??��?', '?�示', {
      type: 'warning'
    })
    const response = await rolePermissionsApi.deleteRolePermission(selectedRoleId.value, row.TKey)
    if (response.data.Success) {
      ElMessage.success('?�除?��?')
      handleRolePermissionSearch()
    } else {
      ElMessage.error(response.data.Message || '?�除失�?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('?�除角色權�?失�?:', error)
      ElMessage.error('?�除角色權�?失�?')
    }
  }
}

// ?��?設�?使用?�系統�???
const handleBatchSetUserSystemPermissions = () => {
  ElMessage.info('?��?設�?使用?�系統�??��??��??�中')
}

// ?��?設�?使用?�選?��???
const handleBatchSetUserMenuPermissions = () => {
  ElMessage.info('?��?設�?使用?�選?��??��??��??�中')
}

// ?��?設�?使用?��?業�???
const handleBatchSetUserProgramPermissions = () => {
  ElMessage.info('?��?設�?使用?��?業�??��??��??�中')
}

// ?��?設�?使用?��??��???
const handleBatchSetUserButtonPermissions = () => {
  ElMessage.info('?��?設�?使用?��??��??��??��??�中')
}

// ?�除使用?��???
const handleDeleteUserPermission = async (row) => {
  try {
    await ElMessageBox.confirm('確�?要刪?�此權�??��?', '?�示', {
      type: 'warning'
    })
    const response = await userPermissionsApi.deleteUserPermission(selectedUserId.value, row.TKey)
    if (response.data.Success) {
      ElMessage.success('?�除?��?')
      handleUserPermissionSearch()
    } else {
      ElMessage.error(response.data.Message || '?�除失�?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('?�除使用?��??�失??', error)
      ElMessage.error('?�除使用?��??�失??)
    }
  }
}

// ?��?變更
const handleRolePermissionSizeChange = (size) => {
  rolePermissionPagination.PageSize = size
  rolePermissionPagination.PageIndex = 1
  handleRolePermissionSearch()
}

const handleRolePermissionPageChange = (page) => {
  rolePermissionPagination.PageIndex = page
  handleRolePermissionSearch()
}

const handleUserPermissionSizeChange = (size) => {
  userPermissionPagination.PageSize = size
  userPermissionPagination.PageIndex = 1
  handleUserPermissionSearch()
}

const handleUserPermissionPageChange = (page) => {
  userPermissionPagination.PageIndex = page
  handleUserPermissionSearch()
}

// ?��???
onMounted(() => {
  loadRoleList()
  loadUserList()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
.permissions {
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


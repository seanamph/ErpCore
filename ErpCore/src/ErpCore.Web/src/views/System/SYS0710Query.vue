<template>
  <div class="sys0710-query">
    <div class="page-header">
      <h1>系統權限列表 (SYS0710)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" :rules="queryRules" ref="queryFormRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="使用者代碼" prop="userId">
              <el-autocomplete
                v-model="queryForm.userId"
                :fetch-suggestions="searchUsers"
                placeholder="請輸入使用者代碼"
                clearable
                style="width: 100%"
                @select="handleUserSelect"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="角色代碼" prop="roleId">
              <el-autocomplete
                v-model="queryForm.roleId"
                :fetch-suggestions="searchRoles"
                placeholder="請輸入角色代碼"
                clearable
                style="width: 100%"
                @select="handleRoleSelect"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="系統代碼">
              <el-select v-model="queryForm.systemId" placeholder="請選擇系統" clearable style="width: 100%">
                <el-option
                  v-for="system in systemOptions"
                  :key="system.value"
                  :label="system.label"
                  :value="system.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="選單代碼">
              <el-input
                v-model="queryForm.menuId"
                placeholder="請輸入選單代碼"
                clearable
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="作業代碼">
              <el-input
                v-model="queryForm.programId"
                placeholder="請輸入作業代碼"
                clearable
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item>
          <el-button type="primary" @click="handleQuery" :loading="loading">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport('Excel')" :disabled="!hasData">匯出Excel</el-button>
          <el-button type="success" @click="handleExport('PDF')" :disabled="!hasData">匯出PDF</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 查詢結果 -->
    <el-card v-if="hasData" class="result-card" shadow="never" style="margin-top: 20px">
      <template #header>
        <div class="header-content">
          <span>查詢結果</span>
          <div class="header-info" v-if="permissionData">
            <span v-if="permissionData.userId">
              使用者: <strong>{{ permissionData.userName }} ({{ permissionData.userId }})</strong>
            </span>
            <span v-if="permissionData.roleId" style="margin-left: 20px">
              角色: <strong>{{ permissionData.roleName }} ({{ permissionData.roleId }})</strong>
            </span>
          </div>
        </div>
      </template>

      <!-- 樹狀結構顯示 -->
      <el-tree
        :data="treeData"
        :props="treeProps"
        default-expand-all
        :expand-on-click-node="false"
        style="width: 100%"
      >
        <template #default="{ node, data }">
          <span class="tree-node">
            <span class="node-label">{{ data.label }}</span>
            <span class="node-value" v-if="data.value">({{ data.value }})</span>
          </span>
        </template>
      </el-tree>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { systemPermissionApi } from '@/api/systemPermission'
import { dropdownListApi } from '@/api/dropdownList'
import { rolesApi } from '@/api/roles'
import { getUsers } from '@/api/users'

const queryFormRef = ref()
const loading = ref(false)
const permissionData = ref(null)
const systemOptions = ref([])

const queryForm = reactive({
  userId: '',
  roleId: '',
  systemId: '',
  menuId: '',
  programId: ''
})

const queryRules = {
  userId: [
    {
      validator: (rule, value, callback) => {
        if (!queryForm.userId && !queryForm.roleId) {
          callback(new Error('請至少輸入使用者代碼或角色代碼'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  roleId: [
    {
      validator: (rule, value, callback) => {
        if (!queryForm.userId && !queryForm.roleId) {
          callback(new Error('請至少輸入使用者代碼或角色代碼'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

const hasData = computed(() => permissionData.value !== null && permissionData.value.permissions?.length > 0)

// 樹狀結構屬性
const treeProps = {
  children: 'children',
  label: 'label'
}

// 轉換為樹狀結構
const treeData = computed(() => {
  if (!permissionData.value || !permissionData.value.permissions) {
    return []
  }

  return permissionData.value.permissions.map(system => ({
    label: `系統: ${system.systemName}`,
    value: system.systemId,
    children: system.menus.map(menu => ({
      label: `選單: ${menu.menuName}`,
      value: menu.menuId,
      children: menu.programs.map(program => ({
        label: `作業: ${program.programName}`,
        value: program.programId,
        children: program.buttons.map(button => ({
          label: `按鈕: ${button.buttonName}`,
          value: button.buttonId
        }))
      }))
    }))
  }))
})

// 搜尋使用者
const searchUsers = async (queryString, cb) => {
  try {
    const response = await getUsers({
      PageIndex: 1,
      PageSize: 50,
      UserId: queryString || undefined
    })
    
    if (response.data?.success) {
      const users = response.data.data?.items || response.data.data?.Items || []
      const results = users.map(user => ({
        value: user.userId,
        label: `${user.userName} (${user.userId})`
      }))
      cb(results)
    } else {
      cb([])
    }
  } catch (error) {
    console.error('搜尋使用者失敗:', error)
    cb([])
  }
}

// 搜尋角色
const searchRoles = async (queryString, cb) => {
  try {
    const response = await rolesApi.getRoles({
      PageIndex: 1,
      PageSize: 50,
      RoleId: queryString || undefined
    })
    
    if (response.data?.success) {
      const roles = response.data.data?.items || response.data.data?.Items || []
      const results = roles.map(role => ({
        value: role.roleId,
        label: `${role.roleName} (${role.roleId})`
      }))
      cb(results)
    } else {
      cb([])
    }
  } catch (error) {
    console.error('搜尋角色失敗:', error)
    cb([])
  }
}

// 使用者選擇
const handleUserSelect = (item) => {
  queryForm.userId = item.value
}

// 角色選擇
const handleRoleSelect = (item) => {
  queryForm.roleId = item.value
}

// 查詢
const handleQuery = async () => {
  await queryFormRef.value.validate(async (valid) => {
    if (!valid) return

    loading.value = true
    try {
      const params = {
        userId: queryForm.userId || undefined,
        roleId: queryForm.roleId || undefined,
        systemId: queryForm.systemId || undefined,
        menuId: queryForm.menuId || undefined,
        programId: queryForm.programId || undefined
      }
      
      const response = await systemPermissionApi.getList(params)
      if (response.data?.success) {
        permissionData.value = response.data.data
        ElMessage.success('查詢成功')
      } else {
        ElMessage.error(response.data?.message || '查詢失敗')
        permissionData.value = null
      }
    } catch (error) {
      console.error('查詢失敗:', error)
      ElMessage.error('查詢失敗: ' + (error.response?.data?.message || error.message))
      permissionData.value = null
    } finally {
      loading.value = false
    }
  })
}

// 重置
const handleReset = () => {
  queryFormRef.value.resetFields()
  permissionData.value = null
}

// 匯出
const handleExport = async (format) => {
  try {
    const data = {
      request: {
        userId: queryForm.userId || undefined,
        roleId: queryForm.roleId || undefined,
        systemId: queryForm.systemId || undefined,
        menuId: queryForm.menuId || undefined,
        programId: queryForm.programId || undefined
      },
      exportFormat: format
    }
    
    const response = await systemPermissionApi.exportReport(data)
    
    // 下載檔案
    const blob = new Blob([response.data], {
      type: format === 'Excel' 
        ? 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        : 'application/pdf'
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `系統權限列表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.${format === 'Excel' ? 'xlsx' : 'pdf'}`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('匯出成功')
  } catch (error) {
    console.error('匯出失敗:', error)
    ElMessage.error('匯出失敗: ' + (error.response?.data?.message || error.message))
  }
}

// 載入系統選項
const loadSystemOptions = async () => {
  try {
    const response = await dropdownListApi.getSystemOptions({
      PageIndex: 1,
      PageSize: 1000,
      Status: 'A'
    })
    
    if (response.data?.success) {
      const systems = response.data.data || []
      systemOptions.value = systems.map(system => ({
        value: system.value || system.systemId,
        label: system.label || system.systemName
      }))
    }
  } catch (error) {
    console.error('載入系統選項失敗:', error)
  }
}

onMounted(() => {
  loadSystemOptions()
})
</script>

<style lang="scss" scoped>
.sys0710-query {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      margin: 0;
      font-size: 24px;
      font-weight: 500;
    }
  }

  .search-card {
    margin-bottom: 20px;
  }

  .result-card {
    .header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      
      .header-info {
        font-size: 14px;
        color: #666;
      }
    }

    .tree-node {
      display: flex;
      align-items: center;
      
      .node-label {
        font-weight: 500;
      }
      
      .node-value {
        margin-left: 8px;
        color: #909399;
        font-size: 12px;
      }
    }
  }
}
</style>

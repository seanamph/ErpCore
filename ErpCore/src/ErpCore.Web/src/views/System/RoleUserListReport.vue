<template>
  <div class="role-user-list-report">
    <div class="page-header">
      <h1>角色之使用者列表 (SYS0750)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" :rules="queryRules" ref="queryFormRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="角色代碼" prop="RoleId">
              <el-autocomplete
                v-model="queryForm.RoleId"
                :fetch-suggestions="searchRoles"
                placeholder="請輸入角色代碼"
                clearable
                style="width: 100%"
                @select="handleRoleSelect"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :loading="loading">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel" :disabled="!hasData">匯出Excel</el-button>
          <el-button type="warning" @click="handleExportPdf" :disabled="!hasData">匯出PDF</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 查詢結果 -->
    <el-card v-if="hasData" class="result-card" shadow="never">
      <template #header>
        <div class="result-header">
          <span>查詢結果</span>
          <div class="role-info">
            <span>角色代碼：{{ roleData.roleId || roleData.RoleId }}</span>
            <span>角色名稱：{{ roleData.roleName || roleData.RoleName }}</span>
          </div>
        </div>
      </template>

      <!-- 使用者列表表格 -->
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="userId" label="使用者代碼" width="120" />
        <el-table-column prop="userName" label="使用者名稱" width="200" />
        <el-table-column prop="userTypeName" label="使用者型態" width="120" />
        <el-table-column prop="statusName" label="帳號狀態" width="100" />
        <el-table-column prop="title" label="職稱" width="150" />
        <el-table-column prop="orgName" label="組織" width="200" />
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="scope">
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(scope.row)"
              :loading="deleting[scope.row.userId]"
            >
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { roleUserApi } from '@/api/systemPermission'
import { rolesApi } from '@/api/roles'

export default {
  name: 'RoleUserListReport',
  setup() {
    const queryFormRef = ref(null)
    const loading = ref(false)
    const roleData = ref(null)
    const deleting = ref({})

    // 查詢表單
    const queryForm = reactive({
      RoleId: ''
    })

    // 表單驗證規則
    const queryRules = {
      RoleId: [
        { required: true, message: '請輸入角色代碼', trigger: 'blur' }
      ]
    }

    // 是否有資料
    const hasData = computed(() => {
      if (!roleData.value) return false
      const users = roleData.value.users || roleData.value.Users || []
      return users.length > 0
    })

    // 表格資料
    const tableData = computed(() => {
      if (!roleData.value || !roleData.value.Users) {
        return []
      }
      return roleData.value.Users.map(user => ({
        userId: user.userId || user.UserId,
        userName: user.userName || user.UserName,
        userType: user.userType || user.UserType,
        userTypeName: user.userTypeName || user.UserTypeName,
        status: user.status || user.Status,
        statusName: user.statusName || user.StatusName,
        title: user.title || user.Title || '',
        orgId: user.orgId || user.OrgId || '',
        orgName: user.orgName || user.OrgName || ''
      }))
    })

    // 搜尋角色（自動完成）
    const searchRoles = async (queryString, cb) => {
      try {
        const response = await rolesApi.getRoles({
          PageIndex: 1,
          PageSize: 50,
          RoleId: queryString || undefined,
          RoleName: queryString || undefined
        })
        // 處理不同的響應格式
        let roles = []
        if (response && response.data) {
          if (response.data.success && response.data.data) {
            const items = response.data.data.items || response.data.data.Items || []
            roles = items.map(item => ({
              value: item.roleId || item.RoleId,
              label: `${item.roleId || item.RoleId} - ${item.roleName || item.RoleName}`
            }))
          } else if (response.data.Data && response.data.Data.Items) {
            roles = response.data.Data.Items.map(item => ({
              value: item.RoleId,
              label: `${item.RoleId} - ${item.RoleName}`
            }))
          }
        } else if (response && response.Data && response.Data.Items) {
          roles = response.Data.Items.map(item => ({
            value: item.RoleId,
            label: `${item.RoleId} - ${item.RoleName}`
          }))
        }
        cb(roles)
      } catch (error) {
        console.error('搜尋角色失敗:', error)
        cb([])
      }
    }

    // 選擇角色
    const handleRoleSelect = (item) => {
      queryForm.RoleId = item.value
    }

    // 查詢資料
    const loadData = async () => {
      if (!queryForm.RoleId) {
        ElMessage.warning('請輸入角色代碼')
        return
      }

      loading.value = true
      try {
        const params = { roleId: queryForm.RoleId }
        const response = await roleUserApi.getList(params)
        // 處理不同的響應格式
        if (response && response.data) {
          if (response.data.success && response.data.data) {
            roleData.value = response.data.data
            ElMessage.success('查詢成功')
          } else if (response.data.Data) {
            // 兼容舊格式
            roleData.value = response.data.Data
            ElMessage.success('查詢成功')
          } else {
            ElMessage.warning(response.data.message || '查詢無資料')
            roleData.value = null
          }
        } else if (response && response.Data) {
          // 兼容舊格式
          roleData.value = response.Data
          ElMessage.success('查詢成功')
        } else {
          ElMessage.warning('查詢無資料')
          roleData.value = null
        }
      } catch (error) {
        console.error('查詢失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
        roleData.value = null
      } finally {
        loading.value = false
      }
    }

    // 查詢
    const handleSearch = async () => {
      if (!queryFormRef.value) return
      await queryFormRef.value.validate(async (valid) => {
        if (valid) {
          await loadData()
        }
      })
    }

    // 重置
    const handleReset = () => {
      if (queryFormRef.value) {
        queryFormRef.value.resetFields()
      }
      roleData.value = null
    }

    // 刪除使用者角色對應
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除使用者「${row.userName}」與角色「${queryForm.RoleId}」的對應關係嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )

        deleting.value[row.userId] = true
        try {
          await roleUserApi.deleteRoleUser(queryForm.RoleId, row.userId)
          ElMessage.success('刪除成功')
          // 重新載入資料
          await loadData()
        } finally {
          deleting.value[row.userId] = false
        }
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
        }
      }
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.RoleId) {
        ElMessage.warning('請先輸入角色代碼並查詢')
        return
      }
      try {
        const data = {
          request: { roleId: queryForm.RoleId },
          exportFormat: 'Excel'
        }
        const response = await roleUserApi.exportReport(data)
        
        // 處理響應數據（可能是 response.data 或 response）
        const blobData = response.data || response
        
        // 下載檔案
        const blob = new Blob([blobData], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `角色之使用者列表_${queryForm.RoleId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
      }
    }

    // 匯出PDF
    const handleExportPdf = async () => {
      if (!queryForm.RoleId) {
        ElMessage.warning('請先輸入角色代碼並查詢')
        return
      }
      try {
        const data = {
          request: { roleId: queryForm.RoleId },
          exportFormat: 'PDF'
        }
        const response = await roleUserApi.exportReport(data)
        
        // 處理響應數據（可能是 response.data 或 response）
        const blobData = response.data || response
        
        // 下載檔案
        const blob = new Blob([blobData], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `角色之使用者列表_${queryForm.RoleId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
      }
    }

    return {
      queryFormRef,
      loading,
      roleData,
      deleting,
      queryForm,
      queryRules,
      hasData,
      tableData,
      searchRoles,
      handleRoleSelect,
      handleSearch,
      handleReset,
      handleDelete,
      handleExportExcel,
      handleExportPdf
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.role-user-list-report {
  .page-header {
    margin-bottom: 20px;
    h1 {
      font-size: 24px;
      font-weight: 600;
      color: #303133;
    }
  }

  .search-card {
    margin-bottom: 20px;
  }

  .result-card {
    margin-top: 20px;

    .result-header {
      display: flex;
      justify-content: space-between;
      align-items: center;

      .role-info {
        display: flex;
        gap: 20px;
        font-size: 14px;
        color: #606266;

        span {
          margin-right: 10px;
        }
      }
    }
  }
}
</style>

<template>
  <div class="role-system-permission-list-report">
    <div class="page-header">
      <h1>角色系統權限列表 (SYS0731)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="角色代碼" prop="RoleId" required>
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
          <el-col :span="8">
            <el-form-item label="系統代碼">
              <el-input v-model="queryForm.SystemId" placeholder="請輸入系統代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="選單代碼">
              <el-input v-model="queryForm.MenuId" placeholder="請輸入選單代碼" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="作業代碼">
              <el-input v-model="queryForm.ProgramId" placeholder="請輸入作業代碼" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel">匯出Excel</el-button>
          <el-button type="warning" @click="handleExportPdf">匯出PDF</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 角色資訊 -->
    <el-card class="info-card" shadow="never" v-if="roleInfo.RoleId">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="角色代碼">{{ roleInfo.RoleId }}</el-descriptions-item>
        <el-descriptions-item label="角色名稱">{{ roleInfo.RoleName }}</el-descriptions-item>
      </el-descriptions>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
        :default-expand-all="false"
      >
        <el-table-column type="expand">
          <template #default="{ row }">
            <div v-if="row.Menus && row.Menus.length > 0" style="padding-left: 20px;">
              <el-table :data="row.Menus" border style="width: 100%">
                <el-table-column type="expand">
                  <template #default="{ row: menuRow }">
                    <div v-if="menuRow.Programs && menuRow.Programs.length > 0" style="padding-left: 20px;">
                      <el-table :data="menuRow.Programs" border style="width: 100%">
                        <el-table-column prop="ProgramId" label="作業代碼" width="150" />
                        <el-table-column prop="ProgramName" label="作業名稱" width="200" />
                        <el-table-column label="按鈕權限" min-width="300">
                          <template #default="{ row: programRow }">
                            <el-tag
                              v-for="(button, index) in programRow.Buttons"
                              :key="index"
                              style="margin-right: 8px; margin-bottom: 4px;"
                            >
                              {{ button.ButtonName }}
                            </el-tag>
                          </template>
                        </el-table-column>
                      </el-table>
                    </div>
                  </template>
                </el-table-column>
                <el-table-column prop="MenuId" label="選單代碼" width="150" />
                <el-table-column prop="MenuName" label="選單名稱" width="200" />
              </el-table>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="SystemId" label="系統代碼" width="150" />
        <el-table-column prop="SystemName" label="系統名稱" width="200" />
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { roleSystemPermissionApi } from '@/api/systemPermission'
import { rolesApi } from '@/api/roles'

export default {
  name: 'RoleSystemPermissionListReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const roleInfo = reactive({
      RoleId: '',
      RoleName: ''
    })

    // 查詢表單
    const queryForm = reactive({
      RoleId: '',
      SystemId: '',
      MenuId: '',
      ProgramId: ''
    })

    // 查詢資料
    const loadData = async () => {
      if (!queryForm.RoleId) {
        ElMessage.warning('請輸入角色代碼')
        return
      }

      loading.value = true
      try {
        const params = { ...queryForm }
        const response = await roleSystemPermissionApi.getList(params)
        if (response.Data) {
          roleInfo.RoleId = response.Data.RoleId || ''
          roleInfo.RoleName = response.Data.RoleName || ''
          tableData.value = response.Data.Permissions || []
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 查詢
    const handleSearch = () => {
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm, {
        RoleId: '',
        SystemId: '',
        MenuId: '',
        ProgramId: ''
      })
      roleInfo.RoleId = ''
      roleInfo.RoleName = ''
      tableData.value = []
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.RoleId) {
        ElMessage.warning('請輸入角色代碼')
        return
      }

      try {
        const data = {
          Request: { ...queryForm },
          ExportFormat: 'Excel'
        }
        const response = await roleSystemPermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `角色系統權限列表_${queryForm.RoleId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 匯出PDF
    const handleExportPdf = async () => {
      if (!queryForm.RoleId) {
        ElMessage.warning('請輸入角色代碼')
        return
      }

      try {
        const data = {
          Request: { ...queryForm },
          ExportFormat: 'PDF'
        }
        const response = await roleSystemPermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `角色系統權限列表_${queryForm.RoleId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    return {
      loading,
      tableData,
      roleInfo,
      queryForm,
      searchRoles,
      handleRoleSelect,
      handleSearch,
      handleReset,
      handleExportExcel,
      handleExportPdf
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.role-system-permission-list-report {
  .search-card {
    margin-bottom: 20px;
  }

  .info-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


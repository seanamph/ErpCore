<template>
  <div class="role-user-list-report">
    <div class="page-header">
      <h1>角色之使用者列表 (SYS0750)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="角色代碼" required>
              <el-input v-model="queryForm.RoleId" placeholder="請輸入角色代碼" clearable />
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
      >
        <el-table-column prop="UserId" label="使用者代碼" width="150" />
        <el-table-column prop="UserName" label="使用者名稱" width="200" />
        <el-table-column prop="UserTypeName" label="使用者型態" width="150" />
        <el-table-column prop="StatusName" label="帳號狀態" width="120">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.StatusName || row.Status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Title" label="職稱" width="150" />
        <el-table-column prop="OrgName" label="組織" width="200" />
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { roleUserApi } from '@/api/systemPermission'

export default {
  name: 'RoleUserListReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const roleInfo = reactive({
      RoleId: '',
      RoleName: ''
    })

    // 查詢表單
    const queryForm = reactive({
      RoleId: ''
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
        const response = await roleUserApi.getList(params)
        if (response.Data) {
          roleInfo.RoleId = response.Data.RoleId || ''
          roleInfo.RoleName = response.Data.RoleName || ''
          tableData.value = response.Data.Users || []
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
        RoleId: ''
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
        const response = await roleUserApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
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
        const response = await roleUserApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
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
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    return {
      loading,
      tableData,
      roleInfo,
      queryForm,
      handleSearch,
      handleReset,
      handleExportExcel,
      handleExportPdf
    }
  }
}
</script>

<style lang="scss" scoped>
.role-user-list-report {
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


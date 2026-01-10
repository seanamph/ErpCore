<template>
  <div class="program-role-permission-list-report">
    <div class="page-header">
      <h1>作業權限之角色列表 (SYS0740)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="作業代碼" required>
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

    <!-- 作業資訊 -->
    <el-card class="info-card" shadow="never" v-if="programInfo.ProgramId">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="作業代碼">{{ programInfo.ProgramId }}</el-descriptions-item>
        <el-descriptions-item label="作業名稱">{{ programInfo.ProgramName }}</el-descriptions-item>
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
        <el-table-column prop="RoleId" label="角色代碼" width="150" />
        <el-table-column prop="RoleName" label="角色名稱" width="200" />
        <el-table-column label="按鈕權限" min-width="300">
          <template #default="{ row }">
            <el-tag
              v-for="(button, index) in row.Buttons"
              :key="index"
              style="margin-right: 8px; margin-bottom: 4px;"
            >
              {{ button.ButtonName }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { programRolePermissionApi } from '@/api/systemPermission'

export default {
  name: 'ProgramRolePermissionListReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const programInfo = reactive({
      ProgramId: '',
      ProgramName: ''
    })

    // 查詢表單
    const queryForm = reactive({
      ProgramId: ''
    })

    // 查詢資料
    const loadData = async () => {
      if (!queryForm.ProgramId) {
        ElMessage.warning('請輸入作業代碼')
        return
      }

      loading.value = true
      try {
        const params = { ...queryForm }
        const response = await programRolePermissionApi.getList(params)
        if (response.Data) {
          programInfo.ProgramId = response.Data.ProgramId || ''
          programInfo.ProgramName = response.Data.ProgramName || ''
          tableData.value = response.Data.Roles || []
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
        ProgramId: ''
      })
      programInfo.ProgramId = ''
      programInfo.ProgramName = ''
      tableData.value = []
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.ProgramId) {
        ElMessage.warning('請輸入作業代碼')
        return
      }

      try {
        const data = {
          Request: { ...queryForm },
          ExportFormat: 'Excel'
        }
        const response = await programRolePermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `作業權限之角色列表_${queryForm.ProgramId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
      if (!queryForm.ProgramId) {
        ElMessage.warning('請輸入作業代碼')
        return
      }

      try {
        const data = {
          Request: { ...queryForm },
          ExportFormat: 'PDF'
        }
        const response = await programRolePermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `作業權限之角色列表_${queryForm.ProgramId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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
      programInfo,
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
@import '@/assets/styles/variables.scss';

.program-role-permission-list-report {
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


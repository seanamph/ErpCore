<template>
  <div class="system-permission-report">
    <div class="page-header">
      <h1>系統權限列表 (SYS0710)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="系統ID">
              <el-input v-model="queryForm.SystemId" placeholder="請輸入系統ID" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="系統名稱">
              <el-input v-model="queryForm.SystemName" placeholder="請輸入系統名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="作業ID">
              <el-input v-model="queryForm.ProgramId" placeholder="請輸入作業ID" clearable />
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

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="SystemId" label="系統ID" width="120" />
        <el-table-column prop="SystemName" label="系統名稱" width="200" />
        <el-table-column prop="ProgramId" label="作業ID" width="120" />
        <el-table-column prop="ProgramName" label="作業名稱" width="200" />
        <el-table-column prop="PermissionType" label="權限類型" width="120" />
        <el-table-column prop="PermissionName" label="權限名稱" width="200" />
        <el-table-column prop="Description" label="說明" min-width="200" show-overflow-tooltip />
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { systemPermissionApi } from '@/api/systemPermission'

export default {
  name: 'SystemPermissionReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      SystemId: '',
      SystemName: '',
      ProgramId: ''
    })

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = { ...queryForm }
        const response = await systemPermissionApi.getList(params)
        if (response.Data) {
          tableData.value = response.Data.Items || []
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
        SystemId: '',
        SystemName: '',
        ProgramId: ''
      })
      handleSearch()
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        const data = {
          Request: { ...queryForm },
          ExportFormat: 'Excel'
        }
        const response = await systemPermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `系統權限列表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
      try {
        const data = {
          Request: { ...queryForm },
          ExportFormat: 'PDF'
        }
        const response = await systemPermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `系統權限列表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      tableData,
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
.system-permission-report {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


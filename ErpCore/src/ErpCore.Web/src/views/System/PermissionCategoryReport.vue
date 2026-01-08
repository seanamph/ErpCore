<template>
  <div class="permission-category-report">
    <div class="page-header">
      <h1>權限分類報表 (SYS0770)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="授權型態">
              <el-select v-model="queryForm.PermissionType" placeholder="請選擇授權型態" clearable>
                <el-option label="角色" value="1" />
                <el-option label="使用者" value="2" />
              </el-select>
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

    <!-- 報表資訊 -->
    <el-card class="info-card" shadow="never" v-if="reportInfo.PermissionType">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="授權型態">{{ reportInfo.PermissionTypeName }}</el-descriptions-item>
        <el-descriptions-item label="資料筆數">{{ tableData.length }} 筆</el-descriptions-item>
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
        <el-table-column prop="SeqNo" label="序號" width="80" />
        <el-table-column prop="UserId" label="使用者代碼" width="150" />
        <el-table-column prop="UserName" label="使用者名稱" width="200" />
        <el-table-column v-if="queryForm.PermissionType === '1'" prop="RoleId" label="角色代碼" width="150" />
        <el-table-column v-if="queryForm.PermissionType === '1'" prop="RoleName" label="角色名稱" width="200" />
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { permissionCategoryReportApi } from '@/api/systemPermission'

export default {
  name: 'PermissionCategoryReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const reportInfo = reactive({
      PermissionType: '',
      PermissionTypeName: ''
    })

    // 查詢表單
    const queryForm = reactive({
      PermissionType: '2'
    })

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = { ...queryForm }
        const response = await permissionCategoryReportApi.getReport(params)
        if (response.Data) {
          reportInfo.PermissionType = response.Data.PermissionType || ''
          reportInfo.PermissionTypeName = response.Data.PermissionTypeName || ''
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
        PermissionType: '2'
      })
      reportInfo.PermissionType = ''
      reportInfo.PermissionTypeName = ''
      tableData.value = []
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        const data = {
          Request: { ...queryForm },
          ExportFormat: 'Excel'
        }
        const response = await permissionCategoryReportApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `權限分類報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
        const response = await permissionCategoryReportApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `權限分類報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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
      reportInfo,
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
.permission-category-report {
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


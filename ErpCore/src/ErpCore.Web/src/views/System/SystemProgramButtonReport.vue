<template>
  <div class="system-program-button-report">
    <div class="page-header">
      <h1>系統作業與功能列表 (SYS0810)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="系統代碼" required>
              <el-input v-model="queryForm.SystemId" placeholder="請輸入系統代碼" clearable />
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

    <!-- 系統資訊 -->
    <el-card class="info-card" shadow="never" v-if="systemInfo.SystemId">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="系統代碼">{{ systemInfo.SystemId }}</el-descriptions-item>
        <el-descriptions-item label="系統名稱">{{ systemInfo.SystemName }}</el-descriptions-item>
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
            <div v-if="row.Buttons && row.Buttons.length > 0" style="padding-left: 20px;">
              <el-table :data="row.Buttons" border style="width: 100%">
                <el-table-column prop="ButtonId" label="按鈕代碼" width="150" />
                <el-table-column prop="ButtonName" label="按鈕名稱" width="200" />
                <el-table-column prop="ButtonType" label="按鈕類型" width="150" />
                <el-table-column prop="PageId" label="頁面代碼" width="150" />
                <el-table-column prop="SeqNo" label="排序序號" width="120" />
              </el-table>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="ProgramId" label="作業代碼" width="150" />
        <el-table-column prop="ProgramName" label="作業名稱" width="200" />
        <el-table-column prop="ProgramType" label="作業類型" width="150" />
        <el-table-column prop="SeqNo" label="排序序號" width="120" />
        <el-table-column label="功能按鈕數量" width="150">
          <template #default="{ row }">
            {{ row.Buttons ? row.Buttons.length : 0 }}
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { systemProgramButtonApi } from '@/api/systemPermission'

export default {
  name: 'SystemProgramButtonReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const systemInfo = reactive({
      SystemId: '',
      SystemName: ''
    })

    // 查詢表單
    const queryForm = reactive({
      SystemId: ''
    })

    // 查詢資料
    const loadData = async () => {
      if (!queryForm.SystemId) {
        ElMessage.warning('請輸入系統代碼')
        return
      }

      loading.value = true
      try {
        const response = await systemProgramButtonApi.getSystemProgramButtons(queryForm.SystemId)
        if (response.Data) {
          systemInfo.SystemId = response.Data.SystemId || ''
          systemInfo.SystemName = response.Data.SystemName || ''
          tableData.value = response.Data.Programs || []
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
        SystemId: ''
      })
      systemInfo.SystemId = ''
      systemInfo.SystemName = ''
      tableData.value = []
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.SystemId) {
        ElMessage.warning('請輸入系統代碼')
        return
      }

      try {
        const response = await systemProgramButtonApi.exportSystemProgramButtons(queryForm.SystemId, 'Excel')
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `系統作業與功能列表_${queryForm.SystemId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
      if (!queryForm.SystemId) {
        ElMessage.warning('請輸入系統代碼')
        return
      }

      try {
        const response = await systemProgramButtonApi.exportSystemProgramButtons(queryForm.SystemId, 'PDF')
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `系統作業與功能列表_${queryForm.SystemId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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
      systemInfo,
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
.system-program-button-report {
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


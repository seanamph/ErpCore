<template>
  <div class="program-user-permission-report">
    <div class="page-header">
      <h1>作業權限之使用者列表 (SYS0720)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" :rules="queryRules" ref="queryFormRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="作業代碼" prop="ProgramId">
              <el-autocomplete
                v-model="queryForm.ProgramId"
                :fetch-suggestions="searchPrograms"
                placeholder="請輸入作業代碼"
                clearable
                style="width: 100%"
                @select="handleProgramSelect"
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
          <div class="program-info">
            <span>作業代碼：{{ permissionData.ProgramId }}</span>
            <span>作業名稱：{{ permissionData.ProgramName }}</span>
          </div>
        </div>
      </template>

      <!-- 使用者列表表格 -->
      <el-table
        :data="flattenedTableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="UserId" label="使用者代碼" width="120" />
        <el-table-column prop="UserName" label="使用者名稱" width="200" />
        <el-table-column prop="ButtonId" label="按鈕代碼" width="120" />
        <el-table-column prop="ButtonName" label="按鈕名稱" width="200" />
        <el-table-column prop="PageId" label="頁面代碼" width="120" />
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { programUserPermissionApi } from '@/api/systemPermission'
import { getPrograms } from '@/api/programs'

export default {
  name: 'ProgramUserPermissionReport',
  setup() {
    const queryFormRef = ref(null)
    const loading = ref(false)
    const permissionData = ref(null)
    const programOptions = ref([])

    // 查詢表單
    const queryForm = reactive({
      ProgramId: ''
    })

    // 表單驗證規則
    const queryRules = {
      ProgramId: [
        { required: true, message: '請輸入作業代碼', trigger: 'blur' }
      ]
    }

    // 是否有資料
    const hasData = computed(() => permissionData.value !== null && permissionData.value.Users && permissionData.value.Users.length > 0)

    // 扁平化表格資料
    const flattenedTableData = computed(() => {
      if (!permissionData.value || !permissionData.value.Users) {
        return []
      }
      const result = []
      permissionData.value.Users.forEach(user => {
        if (user.Buttons && user.Buttons.length > 0) {
          user.Buttons.forEach(button => {
            result.push({
              UserId: user.UserId,
              UserName: user.UserName,
              ButtonId: button.ButtonId,
              ButtonName: button.ButtonName,
              PageId: button.PageId || ''
            })
          })
        } else {
          // 如果沒有按鈕權限，至少顯示使用者資訊
          result.push({
            UserId: user.UserId,
            UserName: user.UserName,
            ButtonId: '',
            ButtonName: '',
            PageId: ''
          })
        }
      })
      return result
    })

    // 搜尋作業（自動完成）
    const searchPrograms = async (queryString, cb) => {
      try {
        const response = await getPrograms({
          PageIndex: 1,
          PageSize: 50,
          Filters: {
            ProgramId: queryString || undefined,
            ProgramName: queryString || undefined
          }
        })
        if (response && response.Data && response.Data.Items) {
          const programs = response.Data.Items.map(item => ({
            value: item.ProgramId,
            label: `${item.ProgramId} - ${item.ProgramName}`
          }))
          cb(programs)
        } else {
          cb([])
        }
      } catch (error) {
        console.error('搜尋作業失敗:', error)
        cb([])
      }
    }

    // 選擇作業
    const handleProgramSelect = (item) => {
      queryForm.ProgramId = item.value
    }

    // 查詢資料
    const loadData = async () => {
      if (!queryForm.ProgramId) {
        ElMessage.warning('請輸入作業代碼')
        return
      }

      loading.value = true
      try {
        const params = { ProgramId: queryForm.ProgramId }
        const response = await programUserPermissionApi.getList(params)
        if (response && response.Data) {
          permissionData.value = response.Data
          ElMessage.success('查詢成功')
        } else {
          ElMessage.warning('查詢無資料')
          permissionData.value = null
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
        permissionData.value = null
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
      permissionData.value = null
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (!queryForm.ProgramId) {
        ElMessage.warning('請先輸入作業代碼並查詢')
        return
      }
      try {
        const data = {
          Request: { ProgramId: queryForm.ProgramId },
          ExportFormat: 'Excel'
        }
        const response = await programUserPermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `作業權限之使用者列表_${queryForm.ProgramId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
        ElMessage.warning('請先輸入作業代碼並查詢')
        return
      }
      try {
        const data = {
          Request: { ProgramId: queryForm.ProgramId },
          ExportFormat: 'PDF'
        }
        const response = await programUserPermissionApi.exportReport(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `作業權限之使用者列表_${queryForm.ProgramId}_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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
      queryFormRef,
      loading,
      permissionData,
      queryForm,
      queryRules,
      hasData,
      flattenedTableData,
      searchPrograms,
      handleProgramSelect,
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

.program-user-permission-report {
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

      .program-info {
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


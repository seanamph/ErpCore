<template>
  <div class="program-role-permission-report">
    <div class="page-header">
      <h1>作業權限之角色列表 (SYS0740)</h1>
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
            <span>作業代碼：{{ permissionData.programId || permissionData.ProgramId }}</span>
            <span>作業名稱：{{ permissionData.programName || permissionData.ProgramName }}</span>
          </div>
        </div>
      </template>

      <!-- 角色列表表格 -->
      <el-table
        :data="flattenedTableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="roleId" label="角色代碼" width="120" />
        <el-table-column prop="roleName" label="角色名稱" width="200" />
        <el-table-column prop="buttonId" label="按鈕代碼" width="120" />
        <el-table-column prop="buttonName" label="按鈕名稱" width="200" />
        <el-table-column prop="pageId" label="頁面代碼" width="120" />
      </el-table>
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { programRolePermissionApi } from '@/api/systemPermission'
import { getPrograms } from '@/api/programs'

export default {
  name: 'ProgramRolePermissionReport',
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
    const hasData = computed(() => {
      if (!permissionData.value) return false
      const roles = permissionData.value.roles || permissionData.value.Roles || []
      return roles.length > 0
    })

    // 扁平化表格資料
    const flattenedTableData = computed(() => {
      if (!permissionData.value || !permissionData.value.Roles) {
        return []
      }
      const result = []
      permissionData.value.Roles.forEach(role => {
        if (role.Buttons && role.Buttons.length > 0) {
          role.Buttons.forEach(button => {
            result.push({
              roleId: role.RoleId,
              roleName: role.RoleName,
              buttonId: button.ButtonId,
              buttonName: button.ButtonName,
              pageId: button.PageId || ''
            })
          })
        } else {
          // 如果沒有按鈕權限，至少顯示角色資訊
          result.push({
            roleId: role.RoleId,
            roleName: role.RoleName,
            buttonId: '',
            buttonName: '',
            pageId: ''
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
          ProgramId: queryString || undefined,
          ProgramName: queryString || undefined
        })
        // 處理不同的響應格式
        let programs = []
        if (response && response.data) {
          if (response.data.success && response.data.data) {
            const items = response.data.data.items || response.data.data.Items || []
            programs = items.map(item => ({
              value: item.programId || item.ProgramId,
              label: `${item.programId || item.ProgramId} - ${item.programName || item.ProgramName}`
            }))
          } else if (response.data.Data && response.data.Data.Items) {
            programs = response.data.Data.Items.map(item => ({
              value: item.ProgramId,
              label: `${item.ProgramId} - ${item.ProgramName}`
            }))
          }
        } else if (response && response.Data && response.Data.Items) {
          programs = response.Data.Items.map(item => ({
            value: item.ProgramId,
            label: `${item.ProgramId} - ${item.ProgramName}`
          }))
        }
        cb(programs)
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
        const params = { programId: queryForm.ProgramId }
        const response = await programRolePermissionApi.getList(params)
        // 處理不同的響應格式
        if (response && response.data) {
          if (response.data.success && response.data.data) {
            permissionData.value = response.data.data
            ElMessage.success('查詢成功')
          } else if (response.data.Data) {
            // 兼容舊格式
            permissionData.value = response.data.Data
            ElMessage.success('查詢成功')
          } else {
            ElMessage.warning(response.data.message || '查詢無資料')
            permissionData.value = null
          }
        } else if (response && response.Data) {
          // 兼容舊格式
          permissionData.value = response.Data
          ElMessage.success('查詢成功')
        } else {
          ElMessage.warning('查詢無資料')
          permissionData.value = null
        }
      } catch (error) {
        console.error('查詢失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
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
          request: { programId: queryForm.ProgramId },
          exportFormat: 'Excel'
        }
        const response = await programRolePermissionApi.exportReport(data)
        
        // 處理響應數據（可能是 response.data 或 response）
        const blobData = response.data || response
        
        // 下載檔案
        const blob = new Blob([blobData], {
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
        ElMessage.error('匯出失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
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
          request: { programId: queryForm.ProgramId },
          exportFormat: 'PDF'
        }
        const response = await programRolePermissionApi.exportReport(data)
        
        // 處理響應數據（可能是 response.data 或 response）
        const blobData = response.data || response
        
        // 下載檔案
        const blob = new Blob([blobData], {
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
        ElMessage.error('匯出失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
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

.program-role-permission-report {
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

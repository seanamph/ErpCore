<template>
  <div class="button-log-report">
    <div class="page-header">
      <h1>按鈕操作記錄查詢 (SYS0790)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="使用者">
              <el-select
                v-model="queryForm.Filters.UserIds"
                placeholder="請選擇使用者"
                multiple
                filterable
                clearable
                style="width: 100%"
              >
                <el-option
                  v-for="user in userList"
                  :key="user.UserId"
                  :label="`${user.UserId} - ${user.UserName}`"
                  :value="user.UserId"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="作業代碼">
              <el-select
                v-model="queryForm.Filters.ProgId"
                placeholder="請選擇作業"
                filterable
                clearable
                style="width: 100%"
              >
                <el-option
                  v-for="program in programList"
                  :key="program.ProgramId"
                  :label="`${program.ProgramId} - ${program.ProgramName}`"
                  :value="program.ProgramId"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="LOG日期時間" required>
              <el-date-picker
                v-model="dateTimeRange"
                type="datetimerange"
                range-separator="至"
                start-placeholder="開始日期時間"
                end-placeholder="結束日期時間"
                format="YYYY/MM/DD HH:mm"
                value-format="YYYY-MM-DD HH:mm"
                style="width: 100%"
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

    <!-- 資料表格 -->
    <el-card v-if="hasData" class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="BTime" label="操作時間" width="160" sortable="custom">
          <template #default="{ row }">
            {{ formatDateTime(row.BTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="BUser" label="使用者編號" width="120" />
        <el-table-column prop="UserName" label="使用者名稱" width="150" />
        <el-table-column prop="ProgId" label="作業代碼" width="120" />
        <el-table-column prop="ProgName" label="作業名稱" width="200" />
        <el-table-column prop="ButtonName" label="按鈕名稱" width="150" />
        <el-table-column prop="Url" label="網頁位址" width="300" show-overflow-tooltip />
        <el-table-column prop="FrameName" label="框架名稱" width="120" />
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="pagination.TotalCount"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right;"
      />
    </el-card>
  </div>
</template>

<script>
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { buttonLogApi } from '@/api/systemPermission'
import { dropdownListApi } from '@/api/dropdownList'
import { getPrograms } from '@/api/programs'

export default {
  name: 'ButtonLogReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const userList = ref([])
    const programList = ref([])
    const dateTimeRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      PageIndex: 1,
      PageSize: 20,
      SortField: 'BTime',
      SortOrder: 'DESC',
      Filters: {
        UserIds: [],
        ProgId: '',
        StartDate: null,
        StartTime: null,
        EndDate: null,
        EndTime: null
      }
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 是否有資料
    const hasData = computed(() => tableData.value && tableData.value.length > 0)

    // 監聽日期時間範圍變化
    watch(dateTimeRange, (newVal) => {
      if (newVal && newVal.length === 2) {
        const startDateTime = new Date(newVal[0])
        const endDateTime = new Date(newVal[1])
        queryForm.Filters.StartDate = startDateTime
        queryForm.Filters.StartTime = `${String(startDateTime.getHours()).padStart(2, '0')}${String(startDateTime.getMinutes()).padStart(2, '0')}`
        queryForm.Filters.EndDate = endDateTime
        queryForm.Filters.EndTime = `${String(endDateTime.getHours()).padStart(2, '0')}${String(endDateTime.getMinutes()).padStart(2, '0')}`
      } else {
        queryForm.Filters.StartDate = null
        queryForm.Filters.StartTime = null
        queryForm.Filters.EndDate = null
        queryForm.Filters.EndTime = null
      }
    })

    // 載入使用者列表
    const loadUserList = async () => {
      try {
        const response = await dropdownListApi.getUserList({
          PageIndex: 1,
          PageSize: 1000,
          Status: 'A'
        })
        if (response && response.data) {
          if (response.data.success && response.data.data) {
            userList.value = response.data.data.Items || response.data.data.items || []
          } else if (response.data.Data && response.data.Data.Items) {
            userList.value = response.data.Data.Items
          }
        }
      } catch (error) {
        console.error('載入使用者列表失敗:', error)
      }
    }

    // 載入作業列表
    const loadProgramList = async () => {
      try {
        const response = await getPrograms({
          PageIndex: 1,
          PageSize: 1000,
          Status: '1'
        })
        if (response && response.data) {
          if (response.data.success && response.data.data) {
            const items = response.data.data.Items || response.data.data.items || []
            programList.value = items.map(item => ({
              ProgramId: item.ProgramId || item.programId,
              ProgramName: item.ProgramName || item.programName
            }))
          } else if (response.data.Data && response.data.Data.Items) {
            programList.value = response.data.Data.Items.map(item => ({
              ProgramId: item.ProgramId,
              ProgramName: item.ProgramName
            }))
          }
        }
      } catch (error) {
        console.error('載入作業列表失敗:', error)
      }
    }

    // 格式化日期時間
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          SortField: queryForm.SortField,
          SortOrder: queryForm.SortOrder,
          Filters: {
            UserIds: queryForm.Filters.UserIds && queryForm.Filters.UserIds.length > 0 ? queryForm.Filters.UserIds : undefined,
            ProgId: queryForm.Filters.ProgId || undefined,
            StartDate: queryForm.Filters.StartDate ? queryForm.Filters.StartDate.toISOString().split('T')[0] : undefined,
            StartTime: queryForm.Filters.StartTime || undefined,
            EndDate: queryForm.Filters.EndDate ? queryForm.Filters.EndDate.toISOString().split('T')[0] : undefined,
            EndTime: queryForm.Filters.EndTime || undefined
          }
        }
        
        const response = await buttonLogApi.getButtonLogs(params)
        
        // 處理不同的響應格式
        if (response && response.data) {
          if (response.data.success && response.data.data) {
            tableData.value = response.data.data.Items || response.data.data.items || []
            pagination.TotalCount = response.data.data.TotalCount || response.data.data.totalCount || 0
          } else if (response.data.Data && response.data.Data.Items) {
            tableData.value = response.data.Data.Items
            pagination.TotalCount = response.data.Data.TotalCount || 0
          } else {
            tableData.value = []
            pagination.TotalCount = 0
          }
        } else {
          tableData.value = []
          pagination.TotalCount = 0
        }
      } catch (error) {
        console.error('查詢失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
        tableData.value = []
        pagination.TotalCount = 0
      } finally {
        loading.value = false
      }
    }

    // 查詢
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      queryForm.Filters.UserIds = []
      queryForm.Filters.ProgId = ''
      queryForm.Filters.StartDate = null
      queryForm.Filters.StartTime = null
      queryForm.Filters.EndDate = null
      queryForm.Filters.EndTime = null
      dateTimeRange.value = null
      pagination.PageIndex = 1
      tableData.value = []
      pagination.TotalCount = 0
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        const query = {
          PageIndex: 1,
          PageSize: 999999,
          SortField: queryForm.SortField,
          SortOrder: queryForm.SortOrder,
          Filters: {
            UserIds: queryForm.Filters.UserIds && queryForm.Filters.UserIds.length > 0 ? queryForm.Filters.UserIds : undefined,
            ProgId: queryForm.Filters.ProgId || undefined,
            StartDate: queryForm.Filters.StartDate ? queryForm.Filters.StartDate.toISOString().split('T')[0] : undefined,
            StartTime: queryForm.Filters.StartTime || undefined,
            EndDate: queryForm.Filters.EndDate ? queryForm.Filters.EndDate.toISOString().split('T')[0] : undefined,
            EndTime: queryForm.Filters.EndTime || undefined
          }
        }
        const response = await buttonLogApi.exportReport(query, 'excel')
        
        // 處理響應數據（可能是 response.data 或 response）
        const blobData = response.data || response
        
        // 下載檔案
        const blob = new Blob([blobData], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `按鈕操作記錄報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
      try {
        const query = {
          PageIndex: 1,
          PageSize: 999999,
          SortField: queryForm.SortField,
          SortOrder: queryForm.SortOrder,
          Filters: {
            UserIds: queryForm.Filters.UserIds && queryForm.Filters.UserIds.length > 0 ? queryForm.Filters.UserIds : undefined,
            ProgId: queryForm.Filters.ProgId || undefined,
            StartDate: queryForm.Filters.StartDate ? queryForm.Filters.StartDate.toISOString().split('T')[0] : undefined,
            StartTime: queryForm.Filters.StartTime || undefined,
            EndDate: queryForm.Filters.EndDate ? queryForm.Filters.EndDate.toISOString().split('T')[0] : undefined,
            EndTime: queryForm.Filters.EndTime || undefined
          }
        }
        const response = await buttonLogApi.exportReport(query, 'pdf')
        
        // 處理響應數據（可能是 response.data 或 response）
        const blobData = response.data || response
        
        // 下載檔案
        const blob = new Blob([blobData], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `按鈕操作記錄報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    onMounted(() => {
      loadUserList()
      loadProgramList()
    })

    return {
      loading,
      tableData,
      userList,
      programList,
      dateTimeRange,
      queryForm,
      pagination,
      hasData,
      formatDateTime,
      handleSearch,
      handleReset,
      handleExportExcel,
      handleExportPdf,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.button-log-report {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


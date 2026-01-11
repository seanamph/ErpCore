<template>
  <div class="user-abnormal-login-report">
    <div class="page-header">
      <h1>使用者異常登入報表 (SYS0760)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="異常事件代碼">
              <el-select v-model="queryForm.EventIds" multiple placeholder="請選擇" clearable style="width: 100%">
                <el-option
                  v-for="item in eventTypes"
                  :key="item.Tag"
                  :label="item.Content"
                  :value="item.Tag">
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="使用者代碼">
              <el-input v-model="queryForm.UserId" placeholder="請輸入使用者代碼" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="事件時間起">
              <el-date-picker
                v-model="queryForm.EventTimeFrom"
                type="datetime"
                placeholder="選擇日期時間"
                format="YYYY/MM/DD HH:mm:ss"
                value-format="YYYY-MM-DD HH:mm:ss"
                style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="事件時間迄">
              <el-date-picker
                v-model="queryForm.EventTimeTo"
                type="datetime"
                placeholder="選擇日期時間"
                format="YYYY/MM/DD HH:mm:ss"
                value-format="YYYY-MM-DD HH:mm:ss"
                style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="排序欄位1">
              <el-select v-model="queryForm.SortBy1" placeholder="請選擇" clearable style="width: 100%">
                <el-option label="異常事件代碼" value="EVENT_ID" />
                <el-option label="使用者代碼" value="USER_ID" />
                <el-option label="IP位址" value="LOGIN_IP" />
                <el-option label="事件發生時間" value="EVENT_TIME" />
              </el-select>
              <el-radio-group v-model="queryForm.SortOrder1" style="margin-left: 10px;">
                <el-radio label="ASC">遞增</el-radio>
                <el-radio label="DESC">遞減</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="排序欄位2">
              <el-select v-model="queryForm.SortBy2" placeholder="請選擇" clearable style="width: 100%">
                <el-option label="異常事件代碼" value="EVENT_ID" />
                <el-option label="使用者代碼" value="USER_ID" />
                <el-option label="IP位址" value="LOGIN_IP" />
                <el-option label="事件發生時間" value="EVENT_TIME" />
              </el-select>
              <el-radio-group v-model="queryForm.SortOrder2" style="margin-left: 10px;">
                <el-radio label="ASC">遞增</el-radio>
                <el-radio label="DESC">遞減</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="排序欄位3">
              <el-select v-model="queryForm.SortBy3" placeholder="請選擇" clearable style="width: 100%">
                <el-option label="異常事件代碼" value="EVENT_ID" />
                <el-option label="使用者代碼" value="USER_ID" />
                <el-option label="IP位址" value="LOGIN_IP" />
                <el-option label="事件發生時間" value="EVENT_TIME" />
              </el-select>
              <el-radio-group v-model="queryForm.SortOrder3" style="margin-left: 10px;">
                <el-radio label="ASC">遞增</el-radio>
                <el-radio label="DESC">遞減</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleDelete">刪除選中</el-button>
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
        @selection-change="handleSelectionChange"
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="EventIdName" label="異常事件代碼" width="150" />
        <el-table-column label="使用者" width="200">
          <template #default="{ row }">
            ({{ row.UserId }}) {{ row.UserName }}
          </template>
        </el-table-column>
        <el-table-column prop="LoginIp" label="IP位址" width="150" />
        <el-table-column prop="EventTime" label="事件發生時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.EventTime) }}
          </template>
        </el-table-column>
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
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { loginLogApi } from '@/api/systemPermission'

export default {
  name: 'UserAbnormalLoginReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const selectedRows = ref([])
    const eventTypes = ref([])

    // 查詢表單
    const queryForm = reactive({
      EventIds: [],
      UserId: '',
      EventTimeFrom: null,
      EventTimeTo: null,
      SortBy1: 'EVENT_TIME',
      SortOrder1: 'DESC',
      SortBy2: '',
      SortOrder2: 'ASC',
      SortBy3: '',
      SortOrder3: 'ASC'
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 格式化日期時間
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 載入異常事件代碼選項
    const loadEventTypes = async () => {
      try {
        const response = await loginLogApi.getEventTypes()
        if (response.data && response.data.success && response.data.data) {
          eventTypes.value = response.data.data
        } else if (response.data && response.data.Data) {
          eventTypes.value = response.data.Data
        }
      } catch (error) {
        console.error('載入異常事件代碼選項失敗:', error)
      }
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await loginLogApi.getLoginLogs(params)
        
        // 處理不同的響應格式
        if (response.data && response.data.success && response.data.data) {
          tableData.value = response.data.data.Items || response.data.data.items || []
          pagination.TotalCount = response.data.data.TotalCount || response.data.data.totalCount || 0
        } else if (response.data && response.data.Data) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else if (response.Data) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
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
      Object.assign(queryForm, {
        EventIds: [],
        UserId: '',
        EventTimeFrom: null,
        EventTimeTo: null,
        SortBy1: 'EVENT_TIME',
        SortOrder1: 'DESC',
        SortBy2: '',
        SortOrder2: 'ASC',
        SortBy3: '',
        SortOrder3: 'ASC'
      })
      selectedRows.value = []
      handleSearch()
    }

    // 選擇變更
    const handleSelectionChange = (selection) => {
      selectedRows.value = selection
    }

    // 刪除
    const handleDelete = async () => {
      if (selectedRows.value.length === 0) {
        ElMessage.warning('請選擇要刪除的記錄')
        return
      }

      try {
        await ElMessageBox.confirm(
          `確定要刪除選中的 ${selectedRows.value.length} 筆記錄嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )

        const tKeys = selectedRows.value.map(row => row.TKey)
        await loginLogApi.deleteLoginLogs({ TKeys: tKeys })
        
        ElMessage.success('刪除成功')
        selectedRows.value = []
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.response?.data?.message || error.message || '未知錯誤'))
        }
      }
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        const data = {
          EventIds: queryForm.EventIds,
          UserId: queryForm.UserId,
          EventTimeFrom: queryForm.EventTimeFrom,
          EventTimeTo: queryForm.EventTimeTo,
          Format: 'EXCEL'
        }
        const response = await loginLogApi.generateReport(data, 'EXCEL')
        
        // 下載檔案
        const blob = new Blob([response.data || response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `使用者異常登入報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
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
        const data = {
          EventIds: queryForm.EventIds,
          UserId: queryForm.UserId,
          EventTimeFrom: queryForm.EventTimeFrom,
          EventTimeTo: queryForm.EventTimeTo,
          Format: 'PDF'
        }
        const response = await loginLogApi.generateReport(data, 'PDF')
        
        // 下載檔案
        const blob = new Blob([response.data || response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `使用者異常登入報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
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
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    onMounted(async () => {
      await loadEventTypes()
      loadData()
    })

    return {
      loading,
      tableData,
      selectedRows,
      eventTypes,
      queryForm,
      pagination,
      formatDateTime,
      handleSearch,
      handleReset,
      handleSelectionChange,
      handleDelete,
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

.user-abnormal-login-report {
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

  .table-card {
    margin-top: 20px;
  }
}
</style>

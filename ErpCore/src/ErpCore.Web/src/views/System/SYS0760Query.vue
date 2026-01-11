<template>
  <div class="sys0760-query">
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
          <el-col :span="12">
            <el-form-item label="顯示欄位">
              <el-select v-model="queryForm.DisplayFields" multiple placeholder="請選擇顯示欄位" clearable style="width: 100%">
                <el-option label="異常事件代碼" value="EVENT_ID" />
                <el-option label="使用者代碼" value="USER_ID" />
                <el-option label="IP位址" value="LOGIN_IP" />
                <el-option label="事件發生時間" value="EVENT_TIME" />
              </el-select>
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
              <el-radio-group v-model="queryForm.SortOrder1" style="margin-top: 8px">
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
              <el-radio-group v-model="queryForm.SortOrder2" style="margin-top: 8px">
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
              <el-radio-group v-model="queryForm.SortOrder3" style="margin-top: 8px">
                <el-radio label="ASC">遞增</el-radio>
                <el-radio label="DESC">遞減</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never" v-if="showTable">
      <div class="toolbar" style="margin-bottom: 16px">
        <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleDelete">
          刪除選中
        </el-button>
        <el-button type="primary" @click="handlePrint">列印報表</el-button>
      </div>
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
        @selection-change="handleSelectionChange"
        @sort-change="handleSortChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column
          v-if="displayFields.includes('EVENT_ID')"
          prop="EventIdName"
          label="異常事件代碼"
          sortable="custom"
          :sort-orders="['ascending', 'descending']"
          width="150" />
        <el-table-column
          v-if="displayFields.includes('USER_ID')"
          label="使用者"
          sortable="custom"
          :sort-orders="['ascending', 'descending']"
          width="200">
          <template #default="{ row }">
            ({{ row.UserId }}) {{ row.UserName || '-' }}
          </template>
        </el-table-column>
        <el-table-column
          v-if="displayFields.includes('LOGIN_IP')"
          prop="LoginIp"
          label="IP位址"
          sortable="custom"
          :sort-orders="['ascending', 'descending']"
          width="150" />
        <el-table-column
          v-if="displayFields.includes('EVENT_TIME')"
          prop="EventTime"
          label="事件發生時間"
          sortable="custom"
          :sort-orders="['ascending', 'descending']"
          width="180"
          :formatter="formatDateTime" />
      </el-table>
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { loginLogApi } from '@/api/loginLog'

// 查詢表單
const queryForm = reactive({
  EventIds: [],
  UserId: '',
  EventTimeFrom: '',
  EventTimeTo: '',
  DisplayFields: ['EVENT_ID', 'USER_ID', 'LOGIN_IP', 'EVENT_TIME'],
  SortBy1: 'EVENT_TIME',
  SortOrder1: 'DESC',
  SortBy2: '',
  SortOrder2: 'ASC',
  SortBy3: '',
  SortOrder3: 'ASC'
})

// 異常事件代碼選項
const eventTypes = ref([])

// 表格資料
const tableData = ref([])
const loading = ref(false)
const showTable = ref(false)
const selectedRows = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 顯示欄位
const displayFields = computed(() => {
  return queryForm.DisplayFields && queryForm.DisplayFields.length > 0
    ? queryForm.DisplayFields
    : ['EVENT_ID', 'USER_ID', 'LOGIN_IP', 'EVENT_TIME']
})

// 格式化日期時間
const formatDateTime = (row, column, cellValue) => {
  if (!cellValue) return '-'
  const date = new Date(cellValue)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 載入異常事件代碼選項
const loadEventTypes = async () => {
  try {
    const response = await loginLogApi.getEventTypes()
    if (response && response.Data) {
      eventTypes.value = response.Data
    }
  } catch (error) {
    ElMessage.error('載入異常事件代碼選項失敗: ' + (error.message || '未知錯誤'))
  }
}

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortBy1: queryForm.SortBy1 || undefined,
      SortOrder1: queryForm.SortOrder1 || 'ASC',
      SortBy2: queryForm.SortBy2 || undefined,
      SortOrder2: queryForm.SortOrder2 || 'ASC',
      SortBy3: queryForm.SortBy3 || undefined,
      SortOrder3: queryForm.SortOrder3 || 'ASC',
      EventIds: queryForm.EventIds && queryForm.EventIds.length > 0 ? queryForm.EventIds : undefined,
      UserId: queryForm.UserId || undefined,
      EventTimeFrom: queryForm.EventTimeFrom || undefined,
      EventTimeTo: queryForm.EventTimeTo || undefined
    }
    const response = await loginLogApi.getLoginLogs(params)
    if (response && response.Data) {
      tableData.value = response.Data.Items || []
      pagination.TotalCount = response.Data.TotalCount || 0
      showTable.value = true
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
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
    EventTimeFrom: '',
    EventTimeTo: '',
    DisplayFields: ['EVENT_ID', 'USER_ID', 'LOGIN_IP', 'EVENT_TIME'],
    SortBy1: 'EVENT_TIME',
    SortOrder1: 'DESC',
    SortBy2: '',
    SortOrder2: 'ASC',
    SortBy3: '',
    SortOrder3: 'ASC'
  })
  pagination.PageIndex = 1
  pagination.PageSize = 20
  tableData.value = []
  selectedRows.value = []
  showTable.value = false
}

// 排序變更
const handleSortChange = ({ prop, order }) => {
  if (prop) {
    // 將表格排序映射到查詢表單的排序欄位
    const fieldMap = {
      'EventIdName': 'EVENT_ID',
      'UserId': 'USER_ID',
      'LoginIp': 'LOGIN_IP',
      'EventTime': 'EVENT_TIME'
    }
    const mappedField = fieldMap[prop]
    if (mappedField) {
      queryForm.SortBy1 = mappedField
      queryForm.SortOrder1 = order === 'ascending' ? 'ASC' : order === 'descending' ? 'DESC' : 'ASC'
      loadData()
    }
  }
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
    const response = await loginLogApi.deleteLoginLogs({ TKeys: tKeys })
    if (response && response.Data) {
      ElMessage.success(`成功刪除 ${response.Data.DeletedCount} 筆記錄`)
      selectedRows.value = []
      loadData()
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 列印報表
const handlePrint = async () => {
  try {
    const reportDto = {
      EventIds: queryForm.EventIds && queryForm.EventIds.length > 0 ? queryForm.EventIds : undefined,
      UserId: queryForm.UserId || undefined,
      EventTimeFrom: queryForm.EventTimeFrom || undefined,
      EventTimeTo: queryForm.EventTimeTo || undefined,
      Format: 'PDF'
    }

    const response = await loginLogApi.generateReport(reportDto, 'PDF')
    
    // 創建下載連結
    const blob = new Blob([response], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `使用者異常登入報表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)

    ElMessage.success('報表產生成功')
  } catch (error) {
    ElMessage.error('產生報表失敗: ' + (error.message || '未知錯誤'))
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

// 初始化
onMounted(() => {
  loadEventTypes()
})
</script>

<style scoped>
.sys0760-query {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 500;
  color: #303133;
}

.search-card {
  margin-bottom: 20px;
}

.table-card {
  margin-top: 20px;
}

.toolbar {
  display: flex;
  gap: 10px;
}
</style>

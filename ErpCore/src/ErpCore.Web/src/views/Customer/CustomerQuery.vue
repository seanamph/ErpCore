<template>
  <div class="customer-query">
    <div class="page-header">
      <h1>客戶查詢作業 (CUS5120)</h1>
    </div>

    <!-- 快速搜尋區域 -->
    <el-card class="quick-search-card" shadow="never">
      <el-autocomplete
        v-model="quickSearchKeyword"
        :fetch-suggestions="handleQuickSearch"
        placeholder="快速搜尋：輸入客戶編號、名稱、統一編號、電話..."
        clearable
        style="width: 100%"
        @select="handleQuickSearchSelect"
      >
        <template #default="{ item }">
          <div class="quick-search-item">
            <div class="item-main">
              <span class="customer-id">{{ item.CustomerId }}</span>
              <span class="customer-name">{{ item.CustomerName }}</span>
            </div>
            <div class="item-sub">
              <span v-if="item.GuiId">統編: {{ item.GuiId }}</span>
              <span v-if="item.CompTel">電話: {{ item.CompTel }}</span>
              <span v-if="item.Cell">手機: {{ item.Cell }}</span>
            </div>
          </div>
        </template>
      </el-autocomplete>
    </el-card>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="客戶編號">
              <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶編號" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客戶名稱">
              <el-input v-model="queryForm.CustomerName" placeholder="請輸入客戶名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="統一編號">
              <el-input v-model="queryForm.GuiId" placeholder="請輸入統一編號" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="識別類型">
              <el-select v-model="queryForm.GuiType" placeholder="請選擇識別類型" clearable>
                <el-option label="統一編號" value="1" />
                <el-option label="身份證字號" value="2" />
                <el-option label="自編編號" value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="聯絡人">
              <el-input v-model="queryForm.ContactStaff" placeholder="請輸入聯絡人" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="公司電話">
              <el-input v-model="queryForm.CompTel" placeholder="請輸入公司電話" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="手機">
              <el-input v-model="queryForm.Cell" placeholder="請輸入手機" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="電子郵件">
              <el-input v-model="queryForm.Email" placeholder="請輸入電子郵件" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="狀態">
              <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="城市">
              <el-input v-model="queryForm.City" placeholder="請輸入城市" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="區域">
              <el-input v-model="queryForm.Canton" placeholder="請輸入區域" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="業務員">
              <el-input v-model="queryForm.SalesId" placeholder="請輸入業務員" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="是否享有折扣">
              <el-select v-model="queryForm.DiscountYn" placeholder="請選擇" clearable>
                <el-option label="是" value="Y" />
                <el-option label="否" value="N" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="是否為月結客戶">
              <el-select v-model="queryForm.MonthlyYn" placeholder="請選擇" clearable>
                <el-option label="是" value="Y" />
                <el-option label="否" value="N" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="交易日期範圍">
              <el-date-picker
                v-model="transDateRange"
                type="daterange"
                range-separator="至"
                start-placeholder="開始日期"
                end-placeholder="結束日期"
                style="width: 100%"
                @change="handleDateRangeChange"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="消費累積金額起">
              <el-input-number v-model="queryForm.AccAmtFrom" :precision="2" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="消費累積金額迄">
              <el-input-number v-model="queryForm.AccAmtTo" :precision="2" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出Excel</el-button>
          <el-button type="info" @click="handleSaveQuery">儲存查詢條件</el-button>
          <el-button type="warning" @click="handleLoadQueryHistory">載入查詢條件</el-button>
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
        <el-table-column prop="CustomerId" label="客戶編號" width="150" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="GuiId" label="統一編號" width="120" />
        <el-table-column prop="ContactStaff" label="聯絡人" width="120" />
        <el-table-column prop="CompTel" label="公司電話" width="150" />
        <el-table-column prop="Cell" label="手機" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="AccAmt" label="累積金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.AccAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransDate" label="最近交易日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TransDate) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewTransactions(row)">交易記錄</el-button>
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

    <!-- 交易記錄對話框 -->
    <el-dialog
      v-model="transactionDialogVisible"
      title="客戶交易記錄"
      width="80%"
    >
      <el-table :data="transactionData" border>
        <el-table-column prop="TransactionDate" label="交易日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TransactionDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransactionNo" label="交易序號" width="150" />
        <el-table-column prop="TransactionType" label="交易類型" width="120" />
        <el-table-column prop="Amount" label="金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100" />
        <el-table-column prop="Notes" label="備註" />
      </el-table>
    </el-dialog>

    <!-- 儲存查詢條件對話框 -->
    <el-dialog
      v-model="saveQueryDialogVisible"
      title="儲存查詢條件"
      width="500px"
    >
      <el-form :model="saveQueryForm" label-width="120px">
        <el-form-item label="查詢名稱" required>
          <el-input v-model="saveQueryForm.QueryName" placeholder="請輸入查詢名稱" />
        </el-form-item>
        <el-form-item label="設為常用查詢">
          <el-switch v-model="saveQueryForm.IsFavorite" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="saveQueryDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleConfirmSaveQuery">確定</el-button>
      </template>
    </el-dialog>

    <!-- 載入查詢條件對話框 -->
    <el-dialog
      v-model="loadQueryDialogVisible"
      title="載入查詢條件"
      width="600px"
    >
      <el-table :data="queryHistoryList" border>
        <el-table-column prop="QueryName" label="查詢名稱" />
        <el-table-column prop="IsFavorite" label="常用" width="80">
          <template #default="{ row }">
            <el-tag v-if="row.IsFavorite" type="success">是</el-tag>
            <span v-else>否</span>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDate(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleLoadQuery(row)">載入</el-button>
            <el-button type="danger" size="small" @click="handleDeleteQueryHistory(row.HistoryId)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { customerQueryApi } from '@/api/customer'

export default {
  name: 'CustomerQuery',
  setup() {
    const loading = ref(false)
    const transactionDialogVisible = ref(false)
    const saveQueryDialogVisible = ref(false)
    const loadQueryDialogVisible = ref(false)
    const tableData = ref([])
    const transactionData = ref([])
    const queryHistoryList = ref([])
    const quickSearchKeyword = ref('')
    const quickSearchResults = ref([])
    const currentCustomerId = ref('')
    const transDateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      CustomerId: '',
      CustomerName: '',
      GuiId: '',
      GuiType: '',
      ContactStaff: '',
      CompTel: '',
      Cell: '',
      Email: '',
      City: '',
      Canton: '',
      SalesId: '',
      Status: '',
      DiscountYn: '',
      MonthlyYn: '',
      TransDateFrom: null,
      TransDateTo: null,
      AccAmtFrom: null,
      AccAmtTo: null
    })

    // 儲存查詢表單
    const saveQueryForm = reactive({
      QueryName: '',
      IsFavorite: false
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (!amount) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 日期範圍變更處理
    const handleDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        queryForm.TransDateFrom = dates[0]
        queryForm.TransDateTo = dates[1]
      } else {
        queryForm.TransDateFrom = null
        queryForm.TransDateTo = null
      }
    }

    // 快速搜尋
    const handleQuickSearch = async (queryString, cb) => {
      if (!queryString || queryString.length < 2) {
        cb([])
        return
      }
      try {
        const response = await customerQueryApi.search({ keyword: queryString, limit: 10 })
        if (response.Data) {
          quickSearchResults.value = response.Data
          cb(response.Data)
        } else {
          cb([])
        }
      } catch (error) {
        console.error('快速搜尋失敗:', error)
        cb([])
      }
    }

    // 快速搜尋選擇
    const handleQuickSearchSelect = (item) => {
      queryForm.CustomerId = item.CustomerId
      handleSearch()
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const data = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          SortField: 'CustomerId',
          SortOrder: 'ASC',
          Filters: {
            CustomerId: queryForm.CustomerId || undefined,
            CustomerName: queryForm.CustomerName || undefined,
            GuiId: queryForm.GuiId || undefined,
            GuiType: queryForm.GuiType || undefined,
            ContactStaff: queryForm.ContactStaff || undefined,
            CompTel: queryForm.CompTel || undefined,
            Cell: queryForm.Cell || undefined,
            Email: queryForm.Email || undefined,
            City: queryForm.City || undefined,
            Canton: queryForm.Canton || undefined,
            SalesId: queryForm.SalesId || undefined,
            Status: queryForm.Status || undefined,
            DiscountYn: queryForm.DiscountYn || undefined,
            MonthlyYn: queryForm.MonthlyYn || undefined,
            TransDateFrom: queryForm.TransDateFrom || undefined,
            TransDateTo: queryForm.TransDateTo || undefined,
            AccAmtFrom: queryForm.AccAmtFrom || undefined,
            AccAmtTo: queryForm.AccAmtTo || undefined
          },
          DateRange: queryForm.TransDateFrom || queryForm.TransDateTo ? {
            Field: 'TransDate',
            From: queryForm.TransDateFrom || undefined,
            To: queryForm.TransDateTo || undefined
          } : undefined,
          AmountRange: queryForm.AccAmtFrom !== null || queryForm.AccAmtTo !== null ? {
            Field: 'AccAmt',
            From: queryForm.AccAmtFrom || undefined,
            To: queryForm.AccAmtTo || undefined
          } : undefined
        }
        const response = await customerQueryApi.advancedQuery(data)
        if (response.Data) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
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
        CustomerId: '',
        CustomerName: '',
        GuiId: '',
        GuiType: '',
        ContactStaff: '',
        CompTel: '',
        Cell: '',
        Email: '',
        City: '',
        Canton: '',
        SalesId: '',
        Status: '',
        DiscountYn: '',
        MonthlyYn: '',
        TransDateFrom: null,
        TransDateTo: null,
        AccAmtFrom: null,
        AccAmtTo: null
      })
      transDateRange.value = null
      quickSearchKeyword.value = ''
      handleSearch()
    }

    // 匯出
    const handleExport = async () => {
      try {
        loading.value = true
        const data = {
          PageIndex: 1,
          PageSize: 2147483647,
          SortField: 'CustomerId',
          SortOrder: 'ASC',
          Filters: {
            CustomerId: queryForm.CustomerId,
            CustomerName: queryForm.CustomerName,
            GuiId: queryForm.GuiId,
            Status: queryForm.Status,
            TransDateFrom: queryForm.TransDateFrom,
            TransDateTo: queryForm.TransDateTo
          }
        }
        
        const response = await customerQueryApi.exportToExcel(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `客戶查詢結果_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 查看交易記錄
    const handleViewTransactions = async (row) => {
      currentCustomerId.value = row.CustomerId
      transactionDialogVisible.value = true
      try {
        const response = await customerQueryApi.getTransactions(row.CustomerId, {
          PageIndex: 1,
          PageSize: 100
        })
        if (response.Data) {
          transactionData.value = response.Data.Items || []
        }
      } catch (error) {
        ElMessage.error('查詢交易記錄失敗: ' + (error.message || '未知錯誤'))
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

    // 儲存查詢條件
    const handleSaveQuery = () => {
      saveQueryForm.QueryName = ''
      saveQueryForm.IsFavorite = false
      saveQueryDialogVisible.value = true
    }

    // 確認儲存查詢條件
    const handleConfirmSaveQuery = async () => {
      if (!saveQueryForm.QueryName) {
        ElMessage.warning('請輸入查詢名稱')
        return
      }
      try {
        const data = {
          QueryName: saveQueryForm.QueryName,
          QueryConditions: { ...queryForm },
          IsFavorite: saveQueryForm.IsFavorite
        }
        await customerQueryApi.saveQueryHistory(data)
        ElMessage.success('儲存成功')
        saveQueryDialogVisible.value = false
      } catch (error) {
        ElMessage.error('儲存失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 載入查詢歷史
    const handleLoadQueryHistory = async () => {
      try {
        const response = await customerQueryApi.getQueryHistory({ moduleCode: 'CUS5120' })
        if (response.Data) {
          queryHistoryList.value = response.Data
          loadQueryDialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('載入查詢歷史失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 載入查詢條件
    const handleLoadQuery = (history) => {
      try {
        if (history.QueryConditions) {
          const conditions = typeof history.QueryConditions === 'string' 
            ? JSON.parse(history.QueryConditions) 
            : history.QueryConditions
          Object.assign(queryForm, conditions)
          
          // 處理日期範圍
          if (conditions.TransDateFrom || conditions.TransDateTo) {
            transDateRange.value = [
              conditions.TransDateFrom ? new Date(conditions.TransDateFrom) : null,
              conditions.TransDateTo ? new Date(conditions.TransDateTo) : null
            ].filter(d => d !== null)
          } else {
            transDateRange.value = null
          }
          
          loadQueryDialogVisible.value = false
          handleSearch()
          ElMessage.success('載入查詢條件成功')
        }
      } catch (error) {
        ElMessage.error('載入查詢條件失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 刪除查詢歷史
    const handleDeleteQueryHistory = async (historyId) => {
      try {
        await ElMessageBox.confirm('確定要刪除此查詢條件嗎？', '提示', {
          confirmButtonText: '確定',
          cancelButtonText: '取消',
          type: 'warning'
        })
        await customerQueryApi.deleteQueryHistory(historyId)
        ElMessage.success('刪除成功')
        handleLoadQueryHistory()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      transactionDialogVisible,
      saveQueryDialogVisible,
      loadQueryDialogVisible,
      tableData,
      transactionData,
      queryHistoryList,
      quickSearchKeyword,
      queryForm,
      saveQueryForm,
      pagination,
      transDateRange,
      formatDate,
      formatCurrency,
      handleSearch,
      handleReset,
      handleExport,
      handleViewTransactions,
      handleSizeChange,
      handlePageChange,
      handleQuickSearch,
      handleQuickSearchSelect,
      handleDateRangeChange,
      handleSaveQuery,
      handleConfirmSaveQuery,
      handleLoadQueryHistory,
      handleLoadQuery,
      handleDeleteQueryHistory
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.customer-query {
  .quick-search-card {
    margin-bottom: 20px;
  }

  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }

  .quick-search-item {
    .item-main {
      display: flex;
      align-items: center;
      gap: 10px;
      
      .customer-id {
        font-weight: bold;
        color: #409eff;
      }
      
      .customer-name {
        color: #303133;
      }
    }
    
    .item-sub {
      margin-top: 5px;
      font-size: 12px;
      color: #909399;
      
      span {
        margin-right: 10px;
      }
    }
  }
}
</style>


<template>
  <div class="sales-process-management">
    <div class="page-header">
      <h1>銷售處理作業 (SYSD210-SYSD230)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="銷售單號">
          <el-input v-model="queryForm.OrderId" placeholder="請輸入銷售單號" clearable />
        </el-form-item>
        <el-form-item label="分店代碼">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="客戶代碼">
          <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="已送出" value="S" />
            <el-option label="已審核" value="A" />
            <el-option label="已出貨" value="O" />
          </el-select>
        </el-form-item>
        <el-form-item label="銷售日期">
          <el-date-picker
            v-model="queryForm.OrderDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
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
        <el-table-column prop="OrderId" label="銷售單號" width="150" />
        <el-table-column prop="OrderDate" label="銷售日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ShopId" label="分店代碼" width="120" />
        <el-table-column prop="CustomerId" label="客戶代碼" width="120" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="ApplyUserId" label="申請人員" width="120" />
        <el-table-column prop="ApplyDate" label="申請日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ApplyDate) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button v-if="row.Status === 'S'" type="success" size="small" @click="handleApprove(row)">審核</el-button>
            <el-button v-if="row.Status === 'A'" type="primary" size="small" @click="handleShip(row)">出貨</el-button>
            <el-button v-if="['S', 'A'].includes(row.Status)" type="danger" size="small" @click="handleCancel(row)">取消</el-button>
            <el-button type="info" size="small" @click="handleView(row)">查看</el-button>
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

    <!-- 審核對話框 -->
    <el-dialog
      v-model="approveDialogVisible"
      title="審核銷售單"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="approveFormRef"
        :model="approveFormData"
        :rules="approveFormRules"
        label-width="120px"
      >
        <el-form-item label="銷售單號">
          <el-input v-model="currentOrder.OrderId" disabled />
        </el-form-item>
        <el-form-item label="審核備註" prop="Memo">
          <el-input v-model="approveFormData.Memo" type="textarea" :rows="3" placeholder="請輸入審核備註" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="approveDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleApproveSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 出貨對話框 -->
    <el-dialog
      v-model="shipDialogVisible"
      title="出貨銷售單"
      width="90%"
      :close-on-click-modal="false"
    >
      <el-form
        ref="shipFormRef"
        :model="shipFormData"
        :rules="shipFormRules"
        label-width="120px"
      >
        <el-form-item label="銷售單號">
          <el-input v-model="currentOrder.OrderId" disabled />
        </el-form-item>
        <el-form-item label="出貨日期" prop="ShipDate">
          <el-date-picker
            v-model="shipFormData.ShipDate"
            type="date"
            placeholder="請選擇出貨日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-divider>出貨明細</el-divider>
        <el-table
          :data="shipFormData.Details"
          border
          style="margin-top: 20px"
        >
          <el-table-column prop="LineNum" label="行號" width="80" />
          <el-table-column prop="GoodsId" label="商品編號" width="150" />
          <el-table-column prop="GoodsName" label="商品名稱" width="200" />
          <el-table-column prop="OrderQty" label="訂購數量" width="120" align="right">
            <template #default="{ row }">
              {{ formatNumber(row.OrderQty) }}
            </template>
          </el-table-column>
          <el-table-column prop="ShippedQty" label="已出貨數量" width="120" align="right">
            <template #default="{ row }">
              {{ formatNumber(row.ShippedQty || 0) }}
            </template>
          </el-table-column>
          <el-table-column prop="ShippedQty" label="本次出貨數量" width="150">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.ShippedQty"
                :min="0"
                :max="row.OrderQty - (row.ShippedQty || 0)"
                :precision="4"
                style="width: 100%"
              />
            </template>
          </el-table-column>
        </el-table>
      </el-form>
      <template #footer>
        <el-button @click="shipDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleShipSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 取消對話框 -->
    <el-dialog
      v-model="cancelDialogVisible"
      title="取消銷售單"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="cancelFormRef"
        :model="cancelFormData"
        :rules="cancelFormRules"
        label-width="120px"
      >
        <el-form-item label="銷售單號">
          <el-input v-model="currentOrder.OrderId" disabled />
        </el-form-item>
        <el-form-item label="取消原因" prop="Memo">
          <el-input v-model="cancelFormData.Memo" type="textarea" :rows="3" placeholder="請輸入取消原因" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="cancelDialogVisible = false">取消</el-button>
        <el-button type="danger" @click="handleCancelSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { salesApi } from '@/api/sales'

export default {
  name: 'SalesProcess',
  setup() {
    const loading = ref(false)
    const approveDialogVisible = ref(false)
    const shipDialogVisible = ref(false)
    const cancelDialogVisible = ref(false)
    const approveFormRef = ref(null)
    const shipFormRef = ref(null)
    const cancelFormRef = ref(null)
    const tableData = ref([])
    const currentOrder = ref({})

    // 查詢表單
    const queryForm = reactive({
      OrderId: '',
      ShopId: '',
      CustomerId: '',
      Status: '',
      OrderDateRange: null
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 審核表單
    const approveFormData = reactive({
      Memo: ''
    })

    const approveFormRules = {
      Memo: [{ required: false, message: '請輸入審核備註', trigger: 'blur' }]
    }

    // 出貨表單
    const shipFormData = reactive({
      ShipDate: new Date().toISOString().split('T')[0],
      Details: []
    })

    const shipFormRules = {
      ShipDate: [{ required: true, message: '請選擇出貨日期', trigger: 'change' }]
    }

    // 取消表單
    const cancelFormData = reactive({
      Memo: ''
    })

    const cancelFormRules = {
      Memo: [{ required: true, message: '請輸入取消原因', trigger: 'blur' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (!amount && amount !== 0) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 格式化數字
    const formatNumber = (num) => {
      if (!num && num !== 0) return '0'
      return Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 0, maximumFractionDigits: 4 })
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'S': 'warning',
        'A': 'success',
        'O': 'primary'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'S': '已送出',
        'A': '已審核',
        'O': '已出貨'
      }
      return texts[status] || status
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
        
        // 只查詢已送出、已審核、已出貨的單據
        if (!params.Status) {
          params.Status = 'S,A,O'
        }
        
        // 處理日期範圍
        if (queryForm.OrderDateRange && queryForm.OrderDateRange.length === 2) {
          params.OrderDateFrom = queryForm.OrderDateRange[0]
          params.OrderDateTo = queryForm.OrderDateRange[1]
        }
        delete params.OrderDateRange

        const response = await salesApi.getSalesOrders(params)
        if (response.data && response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢銷售單列表失敗:', error)
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
        OrderId: '',
        ShopId: '',
        CustomerId: '',
        Status: '',
        OrderDateRange: null
      })
      handleSearch()
    }

    // 查看
    const handleView = async (row) => {
      // TODO: 導航到查看頁面或顯示詳情對話框
      ElMessage.info('查看功能開發中')
    }

    // 審核
    const handleApprove = async (row) => {
      currentOrder.value = row
      approveFormData.Memo = ''
      approveDialogVisible.value = true
    }

    // 審核提交
    const handleApproveSubmit = async () => {
      if (!approveFormRef.value) return
      try {
        await approveFormRef.value.validate()
        await salesApi.approveSalesOrder(currentOrder.value.OrderId, approveFormData)
        ElMessage.success('審核成功')
        approveDialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('審核失敗:', error)
          ElMessage.error('審核失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 出貨
    const handleShip = async (row) => {
      currentOrder.value = row
      // 載入銷售單明細
      try {
        const response = await salesApi.getSalesOrder(row.OrderId)
        if (response.data && response.data.Success) {
          shipFormData.Details = (response.data.Data.Details || []).map(detail => ({
            ...detail,
            ShippedQty: detail.OrderQty - (detail.ShippedQty || 0) // 預設本次出貨數量為未出貨數量
          }))
        }
      } catch (error) {
        console.error('載入銷售單明細失敗:', error)
        ElMessage.error('載入失敗: ' + (error.message || '未知錯誤'))
        return
      }
      shipFormData.ShipDate = new Date().toISOString().split('T')[0]
      shipDialogVisible.value = true
    }

    // 出貨提交
    const handleShipSubmit = async () => {
      if (!shipFormRef.value) return
      try {
        await shipFormRef.value.validate()
        await salesApi.shipSalesOrder(currentOrder.value.OrderId, shipFormData)
        ElMessage.success('出貨成功')
        shipDialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('出貨失敗:', error)
          ElMessage.error('出貨失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 取消
    const handleCancel = async (row) => {
      currentOrder.value = row
      cancelFormData.Memo = ''
      cancelDialogVisible.value = true
    }

    // 取消提交
    const handleCancelSubmit = async () => {
      if (!cancelFormRef.value) return
      try {
        await cancelFormRef.value.validate()
        await salesApi.cancelSalesOrder(currentOrder.value.OrderId, cancelFormData)
        ElMessage.success('取消成功')
        cancelDialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('取消失敗:', error)
          ElMessage.error('取消失敗: ' + (error.message || '未知錯誤'))
        }
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
      loadData()
    })

    return {
      loading,
      approveDialogVisible,
      shipDialogVisible,
      cancelDialogVisible,
      approveFormRef,
      shipFormRef,
      cancelFormRef,
      tableData,
      currentOrder,
      queryForm,
      pagination,
      approveFormData,
      approveFormRules,
      shipFormData,
      shipFormRules,
      cancelFormData,
      cancelFormRules,
      handleSearch,
      handleReset,
      handleView,
      handleApprove,
      handleApproveSubmit,
      handleShip,
      handleShipSubmit,
      handleCancel,
      handleCancelSubmit,
      handleSizeChange,
      handlePageChange,
      formatDate,
      formatCurrency,
      formatNumber,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sales-process-management {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
    }
  }

  .search-card {
    margin-bottom: 20px;
    
    .search-form {
      .el-form-item {
        margin-bottom: 0;
      }
    }
  }

  .table-card {
    .el-table {
      .el-tag {
        font-weight: 500;
      }
    }
  }
}
</style>


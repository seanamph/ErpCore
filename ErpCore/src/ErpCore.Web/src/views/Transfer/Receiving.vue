<template>
  <div class="transfer-receipt">
    <div class="page-header">
      <h1>調撥單驗收作業 (SYSW352)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="驗收單號">
          <el-input v-model="queryForm.ReceiptId" placeholder="請輸入驗收單號" clearable />
        </el-form-item>
        <el-form-item label="調撥單號">
          <el-input v-model="queryForm.TransferId" placeholder="請輸入調撥單號" clearable />
        </el-form-item>
        <el-form-item label="調出分店">
          <el-input v-model="queryForm.FromShopId" placeholder="請輸入調出分店" clearable />
        </el-form-item>
        <el-form-item label="調入分店">
          <el-input v-model="queryForm.ToShopId" placeholder="請輸入調入分店" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="待驗收" value="P" />
            <el-option label="部分驗收" value="R" />
            <el-option label="已驗收" value="C" />
            <el-option label="已取消" value="X" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
          <el-button type="info" @click="handleCreateFromPending">從待驗收調撥單建立</el-button>
          <el-button type="info" @click="handleCreateFromOrder">依調撥單號建立</el-button>
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
        <el-table-column prop="ReceiptId" label="驗收單號" width="150" />
        <el-table-column prop="TransferId" label="調撥單號" width="150" />
        <el-table-column prop="ReceiptDate" label="驗收日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ReceiptDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="FromShopId" label="調出分店" width="120" />
        <el-table-column prop="ToShopId" label="調入分店" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalQty" label="總數量" width="100" />
        <el-table-column prop="TotalAmount" label="總金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.IsSettled || row.Status === 'X'">修改</el-button>
            <el-button type="success" size="small" @click="handleConfirm(row)" :disabled="row.Status === 'C' || row.IsSettled">確認</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.IsSettled || row.Status === 'X'">刪除</el-button>
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

    <!-- 待驗收調撥單選擇對話框 -->
    <el-dialog
      v-model="pendingOrderDialogVisible"
      title="選擇待驗收調撥單"
      width="80%"
      :close-on-click-modal="false"
    >
      <el-form :inline="true" :model="pendingOrderQueryForm" class="search-form">
        <el-form-item label="調撥單號">
          <el-input v-model="pendingOrderQueryForm.TransferId" placeholder="請輸入調撥單號" clearable />
        </el-form-item>
        <el-form-item label="調出分店">
          <el-input v-model="pendingOrderQueryForm.FromShopId" placeholder="請輸入調出分店" clearable />
        </el-form-item>
        <el-form-item label="調入分店">
          <el-input v-model="pendingOrderQueryForm.ToShopId" placeholder="請輸入調入分店" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handlePendingOrderSearch">查詢</el-button>
          <el-button @click="handlePendingOrderReset">重置</el-button>
        </el-form-item>
      </el-form>

      <el-table
        :data="pendingOrderTableData"
        v-loading="pendingOrderLoading"
        border
        stripe
        style="width: 100%; margin-top: 20px"
        @row-click="handlePendingOrderSelect"
        highlight-current-row
      >
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="TransferId" label="調撥單號" width="150" />
        <el-table-column prop="TransferDate" label="調撥日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TransferDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="FromShopId" label="調出分店" width="120" />
        <el-table-column prop="ToShopId" label="調入分店" width="120" />
        <el-table-column prop="TotalQty" label="調撥數量" width="120" />
        <el-table-column prop="ReceiptQty" label="已驗收數量" width="120" />
        <el-table-column prop="PendingReceiptQty" label="待驗收數量" width="120" />
      </el-table>

      <el-pagination
        v-model:current-page="pendingOrderPagination.PageIndex"
        v-model:page-size="pendingOrderPagination.PageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="pendingOrderPagination.TotalCount"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handlePendingOrderSizeChange"
        @current-change="handlePendingOrderPageChange"
        style="margin-top: 20px; text-align: right;"
      />

      <template #footer>
        <el-button @click="pendingOrderDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handlePendingOrderConfirm" :disabled="!selectedPendingOrder">確定</el-button>
      </template>
    </el-dialog>

    <!-- 新增/修改對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="80%"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="調撥單號" prop="TransferId">
              <el-input v-model="formData.TransferId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="驗收日期" prop="ReceiptDate">
              <el-date-picker
                v-model="formData.ReceiptDate"
                type="date"
                placeholder="請選擇驗收日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="驗收人員" prop="ReceiptUserId">
              <el-input v-model="formData.ReceiptUserId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="備註">
              <el-input v-model="formData.Memo" type="textarea" :rows="2" />
            </el-form-item>
          </el-col>
        </el-row>

        <!-- 明細表格 -->
        <el-table
          :data="formData.Details"
          border
          style="margin-top: 20px"
        >
          <el-table-column prop="LineNum" label="行號" width="80" />
          <el-table-column prop="GoodsId" label="商品編號" width="150" />
          <el-table-column prop="TransferQty" label="調撥數量" width="120" />
          <el-table-column prop="ReceiptQty" label="驗收數量" width="120">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.ReceiptQty"
                :min="0"
                :max="row.TransferQty"
                :precision="2"
                @change="calculateTotal"
              />
            </template>
          </el-table-column>
          <el-table-column prop="UnitPrice" label="單價" width="120">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.UnitPrice"
                :min="0"
                :precision="2"
                @change="calculateTotal"
              />
            </template>
          </el-table-column>
          <el-table-column prop="Amount" label="金額" width="120">
            <template #default="{ row }">
              {{ formatCurrency((row.UnitPrice || 0) * row.ReceiptQty) }}
            </template>
          </el-table-column>
          <el-table-column prop="Memo" label="備註">
            <template #default="{ row }">
              <el-input v-model="row.Memo" />
            </template>
          </el-table-column>
        </el-table>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { transferReceiptApi } from '@/api/transfer'

export default {
  name: 'TransferReceipt',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    
    // 待驗收調撥單選擇對話框
    const pendingOrderDialogVisible = ref(false)
    const pendingOrderLoading = ref(false)
    const pendingOrderTableData = ref([])
    const selectedPendingOrder = ref(null)
    
    // 待驗收調撥單查詢表單
    const pendingOrderQueryForm = reactive({
      TransferId: '',
      FromShopId: '',
      ToShopId: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    // 待驗收調撥單分頁資訊
    const pendingOrderPagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 查詢表單
    const queryForm = reactive({
      ReceiptId: '',
      TransferId: '',
      FromShopId: '',
      ToShopId: '',
      Status: '',
      ReceiptDateFrom: null,
      ReceiptDateTo: null,
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資料
    const formData = reactive({
      TransferId: '',
      ReceiptDate: new Date(),
      ReceiptUserId: '',
      Memo: '',
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      TransferId: [{ required: true, message: '請輸入調撥單號', trigger: 'blur' }],
      ReceiptDate: [{ required: true, message: '請選擇驗收日期', trigger: 'change' }]
    }

    // 計算總金額和總數量
    const calculateTotal = () => {
      // 此功能在提交時計算
    }

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

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'P': 'info',
        'R': 'warning',
        'C': 'success',
        'X': 'danger'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'P': '待驗收',
        'R': '部分驗收',
        'C': '已驗收',
        'X': '已取消'
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
        const response = await transferReceiptApi.getTransferReceipts(params)
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
        ReceiptId: '',
        TransferId: '',
        FromShopId: '',
        ToShopId: '',
        Status: '',
        ReceiptDateFrom: null,
        ReceiptDateTo: null
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        TransferId: '',
        ReceiptDate: new Date(),
        ReceiptUserId: '',
        Memo: '',
        Details: []
      })
    }

    // 從待驗收調撥單建立驗收單
    const handleCreateFromPending = async () => {
      try {
        pendingOrderDialogVisible.value = true
        selectedPendingOrder.value = null
        Object.assign(pendingOrderQueryForm, {
          TransferId: '',
          FromShopId: '',
          ToShopId: '',
          PageIndex: 1,
          PageSize: 20
        })
        await loadPendingOrders()
      } catch (error) {
        ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    // 載入待驗收調撥單列表
    const loadPendingOrders = async () => {
      pendingOrderLoading.value = true
      try {
        const params = {
          ...pendingOrderQueryForm,
          PageIndex: pendingOrderPagination.PageIndex,
          PageSize: pendingOrderPagination.PageSize
        }
        const response = await transferReceiptApi.getPendingOrders(params)
        if (response.Data) {
          pendingOrderTableData.value = response.Data.Items || []
          pendingOrderPagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
        ElMessage.error('查詢待驗收調撥單失敗: ' + (error.message || '未知錯誤'))
      } finally {
        pendingOrderLoading.value = false
      }
    }
    
    // 待驗收調撥單查詢
    const handlePendingOrderSearch = () => {
      pendingOrderPagination.PageIndex = 1
      loadPendingOrders()
    }
    
    // 待驗收調撥單重置
    const handlePendingOrderReset = () => {
      Object.assign(pendingOrderQueryForm, {
        TransferId: '',
        FromShopId: '',
        ToShopId: ''
      })
      handlePendingOrderSearch()
    }
    
    // 選擇待驗收調撥單
    const handlePendingOrderSelect = (row) => {
      selectedPendingOrder.value = row
    }
    
    // 確認選擇待驗收調撥單
    const handlePendingOrderConfirm = async () => {
      if (!selectedPendingOrder.value) {
        ElMessage.warning('請選擇一筆調撥單')
        return
      }
      
      try {
        const response = await transferReceiptApi.createReceiptFromOrder(selectedPendingOrder.value.TransferId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          pendingOrderDialogVisible.value = false
          Object.assign(formData, response.Data)
          ElMessage.success('建立成功')
        }
      } catch (error) {
        ElMessage.error('建立失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    // 待驗收調撥單分頁大小變更
    const handlePendingOrderSizeChange = (size) => {
      pendingOrderPagination.PageSize = size
      loadPendingOrders()
    }
    
    // 待驗收調撥單分頁變更
    const handlePendingOrderPageChange = (page) => {
      pendingOrderPagination.PageIndex = page
      loadPendingOrders()
    }

    // 依調撥單號建立驗收單
    const handleCreateFromOrder = async () => {
      try {
        const { value: transferId } = await ElMessageBox.prompt('請輸入調撥單號', '依調撥單建立驗收單', {
          confirmButtonText: '確定',
          cancelButtonText: '取消',
          inputPattern: /.+/,
          inputErrorMessage: '調撥單號不能為空'
        })
        
        const response = await transferReceiptApi.createReceiptFromOrder(transferId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, response.Data)
          ElMessage.success('建立成功')
        }
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('建立失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await transferReceiptApi.getTransferReceipt(row.ReceiptId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, response.Data)
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 確認驗收
    const handleConfirm = async (row) => {
      try {
        await ElMessageBox.confirm('確定要確認此驗收單嗎？', '確認', {
          type: 'warning'
        })
        await transferReceiptApi.confirmReceipt(row.ReceiptId)
        ElMessage.success('確認成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('確認失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此驗收單嗎？', '確認', {
          type: 'warning'
        })
        await transferReceiptApi.deleteTransferReceipt(row.ReceiptId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            // 計算總金額和總數量
            const totalQty = formData.Details.reduce((sum, item) => sum + (item.ReceiptQty || 0), 0)
            const totalAmount = formData.Details.reduce((sum, item) => sum + ((item.UnitPrice || 0) * (item.ReceiptQty || 0)), 0)

            if (isEdit.value) {
              await transferReceiptApi.updateTransferReceipt(formData.ReceiptId, {
                ReceiptDate: formData.ReceiptDate,
                ReceiptUserId: formData.ReceiptUserId,
                Memo: formData.Memo,
                Details: formData.Details.map(item => ({
                  DetailId: item.DetailId,
                  ReceiptQty: item.ReceiptQty,
                  UnitPrice: item.UnitPrice,
                  Memo: item.Memo
                }))
              })
              ElMessage.success('修改成功')
            } else {
              await transferReceiptApi.createTransferReceipt({
                TransferId: formData.TransferId,
                ReceiptDate: formData.ReceiptDate,
                ReceiptUserId: formData.ReceiptUserId,
                Memo: formData.Memo,
                Details: formData.Details.map((item, index) => ({
                  TransferDetailId: item.TransferDetailId,
                  LineNum: index + 1,
                  GoodsId: item.GoodsId,
                  BarcodeId: item.BarcodeId,
                  ReceiptQty: item.ReceiptQty,
                  UnitPrice: item.UnitPrice,
                  Memo: item.Memo
                }))
              })
              ElMessage.success('新增成功')
            }
            dialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
          }
        }
      })
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

    // 計算對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改驗收單' : '新增驗收單'
    })

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      calculateTotal,
      formatDate,
      formatCurrency,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleConfirm,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange,
      handleCreateFromPending,
      handleCreateFromOrder,
      pendingOrderDialogVisible,
      pendingOrderLoading,
      pendingOrderTableData,
      selectedPendingOrder,
      pendingOrderQueryForm,
      pendingOrderPagination,
      loadPendingOrders,
      handlePendingOrderSearch,
      handlePendingOrderReset,
      handlePendingOrderSelect,
      handlePendingOrderConfirm,
      handlePendingOrderSizeChange,
      handlePendingOrderPageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.transfer-receipt {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


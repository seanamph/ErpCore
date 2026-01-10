<template>
  <div class="purchase-order">
    <div class="page-header">
      <h1>訂退貨申請作業 (SYSW315)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="採購單號">
          <el-input v-model="queryForm.OrderId" placeholder="請輸入採購單號" clearable />
        </el-form-item>
        <el-form-item label="單據類型">
          <el-select v-model="queryForm.OrderType" placeholder="請選擇單據類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="採購" value="PO" />
            <el-option label="退貨" value="RT" />
          </el-select>
        </el-form-item>
        <el-form-item label="分店代碼">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="供應商代碼">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入供應商代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="草稿" value="D" />
            <el-option label="已送出" value="S" />
            <el-option label="已審核" value="A" />
            <el-option label="已取消" value="X" />
          </el-select>
        </el-form-item>
        <el-form-item label="採購日期">
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
          <el-button type="success" @click="handleCreate">新增</el-button>
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
        <el-table-column prop="OrderId" label="採購單號" width="150" />
        <el-table-column prop="OrderDate" label="採購日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderType" label="單據類型" width="100">
          <template #default="{ row }">
            <el-tag :type="row.OrderType === 'PO' ? 'primary' : 'warning'">
              {{ row.OrderType === 'PO' ? '採購' : '退貨' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ShopId" label="分店代碼" width="120" />
        <el-table-column prop="SupplierId" label="供應商代碼" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalQty" label="總數量" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalQty) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button v-if="row.Status === 'D'" type="warning" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button v-if="row.Status === 'D'" type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
            <el-button v-if="row.Status === 'D'" type="success" size="small" @click="handleSubmit(row)">送出</el-button>
            <el-button v-if="row.Status === 'S'" type="info" size="small" @click="handleApprove(row)">審核</el-button>
            <el-button v-if="['S', 'A'].includes(row.Status)" type="danger" size="small" @click="handleCancel(row)">取消</el-button>
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
            <el-form-item label="採購單號" prop="OrderId">
              <el-input v-model="formData.OrderId" :disabled="isEdit" placeholder="系統自動產生" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="採購日期" prop="OrderDate">
              <el-date-picker
                v-model="formData.OrderDate"
                type="date"
                placeholder="請選擇採購日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="單據類型" prop="OrderType">
              <el-select v-model="formData.OrderType" placeholder="請選擇單據類型" style="width: 100%">
                <el-option label="採購" value="PO" />
                <el-option label="退貨" value="RT" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="分店代碼" prop="ShopId">
              <el-input v-model="formData.ShopId" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="供應商代碼" prop="SupplierId">
              <el-input v-model="formData.SupplierId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="預期交貨日期">
              <el-date-picker
                v-model="formData.ExpectedDate"
                type="date"
                placeholder="請選擇預期交貨日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註">
          <el-input v-model="formData.Memo" type="textarea" :rows="2" />
        </el-form-item>

        <!-- 明細表格 -->
        <el-divider>商品明細</el-divider>
        <el-table
          :data="formData.Details"
          border
          style="margin-top: 20px"
        >
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column prop="GoodsId" label="商品編號" width="150">
            <template #default="{ row, $index }">
              <el-input v-model="row.GoodsId" @blur="handleGoodsChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="GoodsName" label="商品名稱" width="200" />
          <el-table-column prop="BarcodeId" label="條碼編號" width="150">
            <template #default="{ row, $index }">
              <el-input v-model="row.BarcodeId" />
            </template>
          </el-table-column>
          <el-table-column prop="OrderQty" label="訂購數量" width="120">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.OrderQty"
                :min="0"
                :precision="4"
                @change="handleQtyChange($index)"
                style="width: 100%"
              />
            </template>
          </el-table-column>
          <el-table-column prop="UnitPrice" label="單價" width="120">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.UnitPrice"
                :min="0"
                :precision="4"
                @change="handlePriceChange($index)"
                style="width: 100%"
              />
            </template>
          </el-table-column>
          <el-table-column prop="Amount" label="金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency((row.UnitPrice || 0) * (row.OrderQty || 0)) }}
            </template>
          </el-table-column>
          <el-table-column prop="Memo" label="備註" width="150">
            <template #default="{ row }">
              <el-input v-model="row.Memo" />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" fixed="right">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <div style="margin-top: 10px;">
          <el-button type="primary" @click="handleAddDetail">新增明細</el-button>
          <span style="margin-left: 20px; font-weight: bold;">
            總數量: {{ formatNumber(totalQty) }} | 總金額: {{ formatCurrency(totalAmount) }}
          </span>
        </div>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmitForm">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { purchaseOrderApi } from '@/api/purchase'
import { getProductById } from '@/api/modules/product'

export default {
  name: 'PurchaseOrder',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      OrderId: '',
      OrderType: '',
      ShopId: '',
      SupplierId: '',
      Status: '',
      OrderDateRange: null,
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
      OrderId: '',
      OrderDate: new Date().toISOString().split('T')[0],
      OrderType: 'PO',
      ShopId: '',
      SupplierId: '',
      ExpectedDate: null,
      Memo: '',
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      OrderDate: [{ required: true, message: '請選擇採購日期', trigger: 'change' }],
      OrderType: [{ required: true, message: '請選擇單據類型', trigger: 'change' }],
      ShopId: [{ required: true, message: '請輸入分店代碼', trigger: 'blur' }],
      SupplierId: [{ required: true, message: '請輸入供應商代碼', trigger: 'blur' }]
    }

    // 計算總金額和總數量
    const totalQty = computed(() => {
      return formData.Details.reduce((sum, item) => sum + (item.OrderQty || 0), 0)
    })

    const totalAmount = computed(() => {
      return formData.Details.reduce((sum, item) => sum + ((item.UnitPrice || 0) * (item.OrderQty || 0)), 0)
    })

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
        'D': 'info',
        'S': 'warning',
        'A': 'success',
        'X': 'danger'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'D': '草稿',
        'S': '已送出',
        'A': '已審核',
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
        
        // 處理日期範圍
        if (queryForm.OrderDateRange && queryForm.OrderDateRange.length === 2) {
          params.OrderDateFrom = queryForm.OrderDateRange[0]
          params.OrderDateTo = queryForm.OrderDateRange[1]
        }
        delete params.OrderDateRange

        const response = await purchaseOrderApi.getPurchaseOrders(params)
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
        OrderId: '',
        OrderType: '',
        ShopId: '',
        SupplierId: '',
        Status: '',
        OrderDateRange: null
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        OrderId: '',
        OrderDate: new Date().toISOString().split('T')[0],
        OrderType: 'PO',
        ShopId: '',
        SupplierId: '',
        ExpectedDate: null,
        Memo: '',
        Details: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await purchaseOrderApi.getPurchaseOrder(row.OrderId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          const order = response.Data
          Object.assign(formData, {
            OrderId: order.OrderId,
            OrderDate: formatDate(order.OrderDate),
            OrderType: order.OrderType,
            ShopId: order.ShopId,
            SupplierId: order.SupplierId,
            ExpectedDate: order.ExpectedDate ? formatDate(order.ExpectedDate) : null,
            Memo: order.Memo || '',
            Details: order.Details || []
          })
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此採購單嗎？', '確認', {
          type: 'warning'
        })
        await purchaseOrderApi.deletePurchaseOrder(row.OrderId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 送出
    const handleSubmit = async (row) => {
      try {
        await ElMessageBox.confirm('確定要送出此採購單進行審核嗎？', '確認', {
          type: 'warning'
        })
        await purchaseOrderApi.submitPurchaseOrder(row.OrderId)
        ElMessage.success('送出成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('送出失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 審核
    const handleApprove = async (row) => {
      try {
        await ElMessageBox.confirm('確定要審核通過此採購單嗎？', '確認', {
          type: 'warning'
        })
        await purchaseOrderApi.approvePurchaseOrder(row.OrderId)
        ElMessage.success('審核成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('審核失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 取消
    const handleCancel = async (row) => {
      try {
        await ElMessageBox.confirm('確定要取消此採購單嗎？', '確認', {
          type: 'warning'
        })
        await purchaseOrderApi.cancelPurchaseOrder(row.OrderId)
        ElMessage.success('取消成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('取消失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 新增明細
    const handleAddDetail = () => {
      formData.Details.push({
        DetailId: null,
        LineNum: formData.Details.length + 1,
        GoodsId: '',
        GoodsName: '',
        BarcodeId: '',
        OrderQty: 0,
        UnitPrice: 0,
        Amount: 0,
        Memo: ''
      })
    }

    // 刪除明細
    const handleDeleteDetail = (index) => {
      formData.Details.splice(index, 1)
      // 重新編號
      formData.Details.forEach((item, idx) => {
        item.LineNum = idx + 1
      })
    }

    // 商品變更
    const handleGoodsChange = async (index) => {
      const detail = formData.Details[index]
      if (!detail || !detail.GoodsId) {
        detail.GoodsName = ''
        return
      }

      try {
        const response = await getProductById(detail.GoodsId)
        if (response.Data) {
          detail.GoodsName = response.Data.GoodsName || ''
          // 如果沒有單價，可以從商品資訊取得
          if (!detail.UnitPrice && response.Data.UnitPrice) {
            detail.UnitPrice = response.Data.UnitPrice
          }
        } else {
          detail.GoodsName = ''
          ElMessage.warning(`找不到商品編號: ${detail.GoodsId}`)
        }
      } catch (error) {
        detail.GoodsName = ''
        console.error('查詢商品資訊失敗:', error)
        ElMessage.warning(`查詢商品資訊失敗: ${detail.GoodsId}`)
      }
    }

    // 數量變更
    const handleQtyChange = (index) => {
      const detail = formData.Details[index]
      if (detail) {
        detail.Amount = (detail.UnitPrice || 0) * (detail.OrderQty || 0)
      }
    }

    // 單價變更
    const handlePriceChange = (index) => {
      const detail = formData.Details[index]
      if (detail) {
        detail.Amount = (detail.UnitPrice || 0) * (detail.OrderQty || 0)
      }
    }

    // 提交表單
    const handleSubmitForm = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            // 驗證明細
            if (!formData.Details || formData.Details.length === 0) {
              ElMessage.warning('請至少新增一筆明細')
              return
            }

            // 準備資料
            const submitData = {
              OrderDate: formData.OrderDate,
              OrderType: formData.OrderType,
              ShopId: formData.ShopId,
              SupplierId: formData.SupplierId,
              ExpectedDate: formData.ExpectedDate,
              Memo: formData.Memo,
              Details: formData.Details.map((item, index) => ({
                LineNum: index + 1,
                GoodsId: item.GoodsId,
                BarcodeId: item.BarcodeId,
                OrderQty: item.OrderQty || 0,
                UnitPrice: item.UnitPrice || 0,
                Memo: item.Memo || ''
              }))
            }

            if (isEdit.value) {
              await purchaseOrderApi.updatePurchaseOrder(formData.OrderId, submitData)
              ElMessage.success('修改成功')
            } else {
              await purchaseOrderApi.createPurchaseOrder(submitData)
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
      return isEdit.value ? '修改採購單' : '新增採購單'
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
      totalQty,
      totalAmount,
      dialogTitle,
      formatDate,
      formatCurrency,
      formatNumber,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleSubmit,
      handleApprove,
      handleCancel,
      handleAddDetail,
      handleDeleteDetail,
      handleGoodsChange,
      handleQtyChange,
      handlePriceChange,
      handleSubmitForm,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.purchase-order {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;

    h1 {
      color: $primary-color;
      font-size: 24px;
      font-weight: bold;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .table-card {
    margin-top: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>


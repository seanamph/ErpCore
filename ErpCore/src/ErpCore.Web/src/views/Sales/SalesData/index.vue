<template>
  <div class="sales-data-management">
    <div class="page-header">
      <h1>銷售資料維護 (SYSD110-SYSD140)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="銷售單號">
          <el-input v-model="queryForm.OrderId" placeholder="請輸入銷售單號" clearable />
        </el-form-item>
        <el-form-item label="單據類型">
          <el-select v-model="queryForm.OrderType" placeholder="請選擇單據類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="銷售" value="SO" />
            <el-option label="退貨" value="RT" />
          </el-select>
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
            <el-option label="草稿" value="D" />
            <el-option label="已送出" value="S" />
            <el-option label="已審核" value="A" />
            <el-option label="已出貨" value="O" />
            <el-option label="已取消" value="X" />
            <el-option label="已結案" value="C" />
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
        <el-table-column prop="OrderId" label="銷售單號" width="150" />
        <el-table-column prop="OrderDate" label="銷售日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.OrderDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="OrderType" label="單據類型" width="100">
          <template #default="{ row }">
            <el-tag :type="row.OrderType === 'SO' ? 'success' : 'warning'">
              {{ row.OrderType === 'SO' ? '銷售' : '退貨' }}
            </el-tag>
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
      width="90%"
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
            <el-form-item label="銷售單號" prop="OrderId">
              <el-input v-model="formData.OrderId" :disabled="isEdit" placeholder="系統自動產生" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="銷售日期" prop="OrderDate">
              <el-date-picker
                v-model="formData.OrderDate"
                type="date"
                placeholder="請選擇銷售日期"
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
                <el-option label="銷售" value="SO" />
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
            <el-form-item label="客戶代碼" prop="CustomerId">
              <el-input v-model="formData.CustomerId" />
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
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="幣別">
              <el-select v-model="formData.CurrencyId" placeholder="請選擇幣別" style="width: 100%">
                <el-option label="新台幣" value="TWD" />
                <el-option label="美元" value="USD" />
                <el-option label="人民幣" value="CNY" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="匯率">
              <el-input-number
                v-model="formData.ExchangeRate"
                :min="0"
                :precision="6"
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
            <template #default="{ row }">
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
          <el-table-column prop="UnitId" label="單位" width="100">
            <template #default="{ row }">
              <el-input v-model="row.UnitId" />
            </template>
          </el-table-column>
          <el-table-column prop="DiscountRate" label="折扣率" width="100">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.DiscountRate"
                :min="0"
                :max="100"
                :precision="2"
                @change="handleDiscountChange($index)"
                style="width: 100%"
              />
            </template>
          </el-table-column>
          <el-table-column prop="TaxRate" label="稅率" width="100">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.TaxRate"
                :min="0"
                :max="100"
                :precision="2"
                @change="handleTaxChange($index)"
                style="width: 100%"
              />
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
import { salesApi } from '@/api/sales'

export default {
  name: 'SalesData',
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

    // 表單資料
    const formData = reactive({
      OrderId: '',
      OrderDate: new Date().toISOString().split('T')[0],
      OrderType: 'SO',
      ShopId: '',
      CustomerId: '',
      ExpectedDate: null,
      Memo: '',
      CurrencyId: 'TWD',
      ExchangeRate: 1,
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      OrderDate: [{ required: true, message: '請選擇銷售日期', trigger: 'change' }],
      OrderType: [{ required: true, message: '請選擇單據類型', trigger: 'change' }],
      ShopId: [{ required: true, message: '請輸入分店代碼', trigger: 'blur' }],
      CustomerId: [{ required: true, message: '請輸入客戶代碼', trigger: 'blur' }]
    }

    // 計算總金額和總數量
    const totalQty = computed(() => {
      return formData.Details.reduce((sum, item) => sum + (item.OrderQty || 0), 0)
    })

    const totalAmount = computed(() => {
      return formData.Details.reduce((sum, item) => {
        const amount = (item.UnitPrice || 0) * (item.OrderQty || 0)
        const discountAmount = amount * ((item.DiscountRate || 0) / 100)
        const taxAmount = (amount - discountAmount) * ((item.TaxRate || 0) / 100)
        return sum + amount - discountAmount + taxAmount
      }, 0)
    })

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改銷售單' : '新增銷售單'
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
        'O': 'primary',
        'X': 'danger',
        'C': ''
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'D': '草稿',
        'S': '已送出',
        'A': '已審核',
        'O': '已出貨',
        'X': '已取消',
        'C': '已結案'
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
        OrderType: '',
        ShopId: '',
        CustomerId: '',
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
        OrderType: 'SO',
        ShopId: '',
        CustomerId: '',
        ExpectedDate: null,
        Memo: '',
        CurrencyId: 'TWD',
        ExchangeRate: 1,
        Details: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await salesApi.getSalesOrder(row.OrderId)
        if (response.data && response.data.Success) {
          isEdit.value = true
          dialogVisible.value = true
          const order = response.data.Data
          Object.assign(formData, {
            OrderId: order.OrderId,
            OrderDate: formatDate(order.OrderDate),
            OrderType: order.OrderType,
            ShopId: order.ShopId,
            CustomerId: order.CustomerId,
            ExpectedDate: order.ExpectedDate ? formatDate(order.ExpectedDate) : null,
            Memo: order.Memo || '',
            CurrencyId: order.CurrencyId || 'TWD',
            ExchangeRate: order.ExchangeRate || 1,
            Details: order.Details || []
          })
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢銷售單失敗:', error)
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
        await ElMessageBox.confirm('確定要刪除此銷售單嗎？', '確認', {
          type: 'warning'
        })
        await salesApi.deleteSalesOrder(row.OrderId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 商品變更
    const handleGoodsChange = async (index) => {
      const detail = formData.Details[index]
      if (!detail.GoodsId) return
      
      // TODO: 根據商品編號查詢商品資訊
      // const product = await getProductById(detail.GoodsId)
      // if (product) {
      //   detail.GoodsName = product.Name
      //   detail.UnitId = product.UnitId
      //   detail.UnitPrice = product.Price
      // }
    }

    // 數量變更
    const handleQtyChange = (index) => {
      const detail = formData.Details[index]
      detail.Amount = (detail.UnitPrice || 0) * (detail.OrderQty || 0)
    }

    // 單價變更
    const handlePriceChange = (index) => {
      const detail = formData.Details[index]
      detail.Amount = (detail.UnitPrice || 0) * (detail.OrderQty || 0)
    }

    // 折扣變更
    const handleDiscountChange = (index) => {
      // 折扣金額計算
      const detail = formData.Details[index]
      const amount = (detail.UnitPrice || 0) * (detail.OrderQty || 0)
      detail.DiscountAmount = amount * ((detail.DiscountRate || 0) / 100)
    }

    // 稅率變更
    const handleTaxChange = (index) => {
      // 稅額計算
      const detail = formData.Details[index]
      const amount = (detail.UnitPrice || 0) * (detail.OrderQty || 0)
      const discountAmount = amount * ((detail.DiscountRate || 0) / 100)
      detail.TaxAmount = (amount - discountAmount) * ((detail.TaxRate || 0) / 100)
    }

    // 新增明細
    const handleAddDetail = () => {
      formData.Details.push({
        LineNum: formData.Details.length + 1,
        GoodsId: '',
        GoodsName: '',
        BarcodeId: '',
        OrderQty: 0,
        UnitPrice: 0,
        Amount: 0,
        UnitId: '',
        DiscountRate: 0,
        DiscountAmount: 0,
        TaxRate: 0,
        TaxAmount: 0,
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

    // 提交表單
    const handleSubmitForm = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        
        // 計算總金額和總數量
        const totalQty = formData.Details.reduce((sum, item) => sum + (item.OrderQty || 0), 0)
        const totalAmount = formData.Details.reduce((sum, item) => {
          const amount = (item.UnitPrice || 0) * (item.OrderQty || 0)
          const discountAmount = amount * ((item.DiscountRate || 0) / 100)
          const taxAmount = (amount - discountAmount) * ((item.TaxRate || 0) / 100)
          return sum + amount - discountAmount + taxAmount
        }, 0)

        const submitData = {
          ...formData,
          TotalQty: totalQty,
          TotalAmount: totalAmount,
          DiscountAmount: formData.Details.reduce((sum, item) => {
            const amount = (item.UnitPrice || 0) * (item.OrderQty || 0)
            return sum + (amount * ((item.DiscountRate || 0) / 100))
          }, 0),
          TaxAmount: formData.Details.reduce((sum, item) => {
            const amount = (item.UnitPrice || 0) * (item.OrderQty || 0)
            const discountAmount = amount * ((item.DiscountRate || 0) / 100)
            return sum + ((amount - discountAmount) * ((item.TaxRate || 0) / 100))
          }, 0)
        }

        if (isEdit.value) {
          await salesApi.updateSalesOrder(formData.OrderId, submitData)
          ElMessage.success('修改成功')
        } else {
          await salesApi.createSalesOrder(submitData)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
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
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      totalQty,
      totalAmount,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleGoodsChange,
      handleQtyChange,
      handlePriceChange,
      handleDiscountChange,
      handleTaxChange,
      handleAddDetail,
      handleDeleteDetail,
      handleSubmitForm,
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

.sales-data-management {
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


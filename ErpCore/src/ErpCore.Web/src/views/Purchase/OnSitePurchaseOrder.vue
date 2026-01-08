<template>
  <div class="on-site-purchase-order">
    <div class="page-header">
      <h1>現場打單作業 (SYSW322)</h1>
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

        <!-- 明細表格 - 現場打單特殊設計 -->
        <el-divider>商品明細（支援條碼掃描）</el-divider>
        
        <!-- 條碼掃描輸入區 -->
        <el-row :gutter="20" style="margin-bottom: 10px;">
          <el-col :span="12">
            <el-input
              v-model="barcodeInput"
              placeholder="請掃描或輸入條碼，按 Enter 鍵自動新增"
              @keyup.enter="handleBarcodeScan"
              ref="barcodeInputRef"
              clearable
            >
              <template #prepend>條碼</template>
              <template #append>
                <el-button @click="handleBarcodeScan" type="primary">查詢</el-button>
              </template>
            </el-input>
          </el-col>
          <el-col :span="12">
            <el-button type="primary" @click="handleQuickAddGoods">快速新增商品</el-button>
          </el-col>
        </el-row>

        <el-table
          :data="formData.Details"
          border
          style="margin-top: 20px"
        >
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column prop="BarcodeId" label="條碼" width="150">
            <template #default="{ row, $index }">
              <el-input v-model="row.BarcodeId" @blur="handleBarcodeChange($index)" />
            </template>
          </el-table-column>
          <el-table-column prop="GoodsId" label="商品編號" width="150" />
          <el-table-column prop="GoodsName" label="商品名稱" width="200" />
          <el-table-column prop="OrderQty" label="數量" width="120">
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
import { ref, reactive, computed, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { onSitePurchaseOrderApi } from '@/api/purchase'

export default {
  name: 'OnSitePurchaseOrder',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const barcodeInput = ref('')
    const barcodeInputRef = ref(null)

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
      Memo: '',
      ExpectedDate: '',
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      OrderDate: [{ required: true, message: '請選擇採購日期', trigger: 'blur' }],
      OrderType: [{ required: true, message: '請選擇單據類型', trigger: 'change' }],
      ShopId: [{ required: true, message: '請輸入分店代碼', trigger: 'blur' }],
      SupplierId: [{ required: true, message: '請輸入供應商代碼', trigger: 'blur' }]
    }

    // 計算總數量和總金額
    const totalQty = computed(() => {
      return formData.Details.reduce((sum, item) => sum + (item.OrderQty || 0), 0)
    })

    const totalAmount = computed(() => {
      return formData.Details.reduce((sum, item) => sum + ((item.UnitPrice || 0) * (item.OrderQty || 0)), 0)
    })

    const dialogTitle = computed(() => {
      return isEdit.value ? '修改現場打單申請單' : '新增現場打單申請單'
    })

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      return new Date(date).toLocaleDateString('zh-TW')
    }

    // 格式化數字
    const formatNumber = (num) => {
      if (num == null) return '0'
      return Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 0, maximumFractionDigits: 4 })
    }

    // 格式化貨幣
    const formatCurrency = (num) => {
      if (num == null) return '$0'
      return '$' + Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const statusMap = {
        'D': 'info',
        'S': 'warning',
        'A': 'success',
        'X': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const statusMap = {
        'D': '草稿',
        'S': '已送出',
        'A': '已審核',
        'X': '已取消'
      }
      return statusMap[status] || status
    }

    // 查詢列表
    const handleSearch = async () => {
      try {
        loading.value = true
        const params = {
          ...queryForm,
          OrderDateFrom: queryForm.OrderDateRange?.[0] || null,
          OrderDateTo: queryForm.OrderDateRange?.[1] || null,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        delete params.OrderDateRange

        const response = await onSitePurchaseOrderApi.getOnSitePurchaseOrders(params)
        if (response.data.success) {
          tableData.value = response.data.data.Items || []
          pagination.TotalCount = response.data.data.TotalCount || 0
        } else {
          ElMessage.error(response.data.message || '查詢失敗')
        }
      } catch (error) {
        ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 重置查詢
    const handleReset = () => {
      queryForm.OrderId = ''
      queryForm.OrderType = ''
      queryForm.ShopId = ''
      queryForm.SupplierId = ''
      queryForm.Status = ''
      queryForm.OrderDateRange = null
      pagination.PageIndex = 1
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      formData.OrderId = ''
      formData.OrderDate = new Date().toISOString().split('T')[0]
      formData.OrderType = 'PO'
      formData.ShopId = ''
      formData.SupplierId = ''
      formData.Memo = ''
      formData.ExpectedDate = ''
      formData.Details = []
      barcodeInput.value = ''
      dialogVisible.value = true
      nextTick(() => {
        barcodeInputRef.value?.focus()
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        loading.value = true
        const response = await onSitePurchaseOrderApi.getOnSitePurchaseOrder(row.OrderId)
        if (response.data.success) {
          const data = response.data.data
          Object.assign(formData, {
            OrderId: data.OrderId,
            OrderDate: data.OrderDate.split('T')[0],
            OrderType: data.OrderType,
            ShopId: data.ShopId,
            SupplierId: data.SupplierId,
            Memo: data.Memo || '',
            ExpectedDate: data.ExpectedDate ? data.ExpectedDate.split('T')[0] : '',
            Details: (data.Details || []).map(d => ({
              DetailId: d.DetailId,
              LineNum: d.LineNum,
              GoodsId: d.GoodsId,
              GoodsName: d.GoodsName || '',
              BarcodeId: d.BarcodeId || '',
              OrderQty: d.OrderQty,
              UnitPrice: d.UnitPrice,
              Amount: d.Amount,
              Memo: d.Memo || ''
            }))
          })
          isEdit.value = false
          dialogVisible.value = true
        } else {
          ElMessage.error(response.data.message || '查詢失敗')
        }
      } catch (error) {
        ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
      isEdit.value = true
      nextTick(() => {
        barcodeInputRef.value?.focus()
      })
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此現場打單申請單嗎？', '提示', {
          confirmButtonText: '確定',
          cancelButtonText: '取消',
          type: 'warning'
        })

        loading.value = true
        const response = await onSitePurchaseOrderApi.deleteOnSitePurchaseOrder(row.OrderId)
        if (response.data.success) {
          ElMessage.success('刪除成功')
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '刪除失敗')
        }
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
        }
      } finally {
        loading.value = false
      }
    }

    // 送出
    const handleSubmit = async (row) => {
      try {
        await ElMessageBox.confirm('確定要送出現場打單申請單嗎？', '提示', {
          confirmButtonText: '確定',
          cancelButtonText: '取消',
          type: 'warning'
        })

        loading.value = true
        const response = await onSitePurchaseOrderApi.submitOnSitePurchaseOrder(row.OrderId)
        if (response.data.success) {
          ElMessage.success('送出成功')
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '送出失敗')
        }
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('送出失敗：' + (error.message || '未知錯誤'))
        }
      } finally {
        loading.value = false
      }
    }

    // 條碼掃描處理
    const handleBarcodeScan = async () => {
      if (!barcodeInput.value) {
        ElMessage.warning('請輸入條碼')
        return
      }

      try {
        const response = await onSitePurchaseOrderApi.getGoodsByBarcode(barcodeInput.value)
        if (response.data.success) {
          const goods = response.data.data
          // 檢查是否已存在
          const existingIndex = formData.Details.findIndex(d => d.BarcodeId === goods.BarcodeId)
          if (existingIndex >= 0) {
            // 已存在，增加數量
            formData.Details[existingIndex].OrderQty += 1
            handleQtyChange(existingIndex)
          } else {
            // 新增商品到明細
            formData.Details.push({
              LineNum: formData.Details.length + 1,
              GoodsId: goods.GoodsId,
              GoodsName: goods.GoodsName,
              BarcodeId: goods.BarcodeId,
              OrderQty: 1,
              UnitPrice: goods.UnitPrice || 0,
              Amount: goods.UnitPrice || 0,
              Memo: ''
            })
          }
          barcodeInput.value = ''
          nextTick(() => {
            barcodeInputRef.value?.focus()
          })
          ElMessage.success('商品新增成功')
        } else {
          ElMessage.error(response.data.message || '查詢商品失敗')
        }
      } catch (error) {
        ElMessage.error('查詢商品失敗：' + (error.message || '未知錯誤'))
      }
    }

    // 條碼變更處理
    const handleBarcodeChange = async (index) => {
      const detail = formData.Details[index]
      if (detail.BarcodeId) {
        try {
          const response = await onSitePurchaseOrderApi.getGoodsByBarcode(detail.BarcodeId)
          if (response.data.success) {
            const goods = response.data.data
            detail.GoodsId = goods.GoodsId
            detail.GoodsName = goods.GoodsName
            detail.UnitPrice = goods.UnitPrice || 0
            handlePriceChange(index)
          } else {
            ElMessage.error(response.data.message || '查詢商品失敗')
          }
        } catch (error) {
          ElMessage.error('查詢商品失敗：' + (error.message || '未知錯誤'))
        }
      }
    }

    // 快速新增商品
    const handleQuickAddGoods = () => {
      formData.Details.push({
        LineNum: formData.Details.length + 1,
        GoodsId: '',
        GoodsName: '',
        BarcodeId: '',
        OrderQty: 1,
        UnitPrice: 0,
        Amount: 0,
        Memo: ''
      })
    }

    // 新增明細
    const handleAddDetail = () => {
      handleQuickAddGoods()
    }

    // 刪除明細
    const handleDeleteDetail = (index) => {
      formData.Details.splice(index, 1)
      // 重新編號
      formData.Details.forEach((item, idx) => {
        item.LineNum = idx + 1
      })
    }

    // 數量變更
    const handleQtyChange = (index) => {
      const detail = formData.Details[index]
      if (detail.UnitPrice != null && detail.OrderQty != null) {
        detail.Amount = detail.UnitPrice * detail.OrderQty
      }
    }

    // 單價變更
    const handlePriceChange = (index) => {
      handleQtyChange(index)
    }

    // 提交表單
    const handleSubmitForm = async () => {
      try {
        await formRef.value.validate()

        if (formData.Details.length === 0) {
          ElMessage.warning('請至少新增一筆商品明細')
          return
        }

        loading.value = true
        const submitData = {
          OrderDate: formData.OrderDate,
          OrderType: formData.OrderType,
          ShopId: formData.ShopId,
          SupplierId: formData.SupplierId,
          Memo: formData.Memo || '',
          ExpectedDate: formData.ExpectedDate || null,
          Details: formData.Details.map(d => ({
            LineNum: d.LineNum,
            GoodsId: d.GoodsId,
            BarcodeId: d.BarcodeId || '',
            OrderQty: d.OrderQty,
            UnitPrice: d.UnitPrice,
            Memo: d.Memo || ''
          }))
        }

        let response
        if (isEdit.value) {
          response = await onSitePurchaseOrderApi.updateOnSitePurchaseOrder(formData.OrderId, submitData)
        } else {
          response = await onSitePurchaseOrderApi.createOnSitePurchaseOrder(submitData)
        }

        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        if (error !== false) {
          ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + (error.message || '未知錯誤'))
        }
      } finally {
        loading.value = false
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      handleSearch()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      handleSearch()
    }

    // 初始化
    onMounted(() => {
      handleSearch()
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
      barcodeInput,
      barcodeInputRef,
      totalQty,
      totalAmount,
      dialogTitle,
      formatDate,
      formatNumber,
      formatCurrency,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleSubmit,
      handleBarcodeScan,
      handleBarcodeChange,
      handleQuickAddGoods,
      handleAddDetail,
      handleDeleteDetail,
      handleQtyChange,
      handlePriceChange,
      handleSubmitForm,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style scoped>
.on-site-purchase-order {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: bold;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}

.table-card {
  margin-bottom: 20px;
}
</style>


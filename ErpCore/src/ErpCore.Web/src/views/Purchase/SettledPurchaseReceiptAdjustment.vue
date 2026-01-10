<template>
  <div class="settled-adjustment">
    <div class="page-header">
      <h1>已日結採購單驗收調整作業 (SYSW333)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="驗收單號">
          <el-input v-model="queryForm.ReceiptId" placeholder="請輸入驗收單號" clearable />
        </el-form-item>
        <el-form-item label="採購單號">
          <el-input v-model="queryForm.OrderId" placeholder="請輸入採購單號" clearable />
        </el-form-item>
        <el-form-item label="分店代碼">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="供應商代碼">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入供應商代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="D" />
            <el-option label="已審核" value="A" />
            <el-option label="已日結" value="C" />
          </el-select>
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
        <el-table-column prop="ReceiptId" label="驗收單號" width="150" />
        <el-table-column prop="OrderId" label="採購單號" width="150" />
        <el-table-column prop="ReceiptDate" label="驗收日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ReceiptDate) }}
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
        <el-table-column prop="TotalAmount" label="總金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="AdjustmentReason" label="調整原因" width="200" />
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status !== 'D'">修改</el-button>
            <el-button type="success" size="small" @click="handleApprove(row)" :disabled="row.Status !== 'D'">審核</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status !== 'D'">刪除</el-button>
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
              <el-select
                v-model="formData.OrderId"
                placeholder="請選擇已日結採購單"
                filterable
                :disabled="isEdit"
                @change="handleOrderChange"
              >
                <el-option
                  v-for="order in settledOrderList"
                  :key="order.OrderId"
                  :label="order.OrderId"
                  :value="order.OrderId"
                />
              </el-select>
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
            <el-form-item label="分店代碼" prop="ShopId">
              <el-input v-model="formData.ShopId" :disabled="true" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供應商代碼" prop="SupplierId">
              <el-input v-model="formData.SupplierId" :disabled="true" />
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
            <el-form-item label="調整原因" prop="AdjustmentReason">
              <el-input v-model="formData.AdjustmentReason" type="textarea" :rows="2" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註">
          <el-input v-model="formData.Memo" type="textarea" :rows="2" />
        </el-form-item>

        <!-- 明細表格 -->
        <el-table
          :data="formData.Details"
          border
          style="margin-top: 20px"
        >
          <el-table-column prop="LineNum" label="行號" width="80" />
          <el-table-column prop="GoodsId" label="商品編號" width="150" />
          <el-table-column prop="BarcodeId" label="條碼編號" width="150" />
          <el-table-column prop="OrderQty" label="訂購數量" width="120" />
          <el-table-column prop="OriginalReceiptQty" label="原始驗收數量" width="120" />
          <el-table-column prop="ReceiptQty" label="調整後驗收數量" width="150">
            <template #default="{ row }">
              <el-input-number
                v-model="row.ReceiptQty"
                :min="0"
                :precision="4"
                @change="calculateTotal"
              />
            </template>
          </el-table-column>
          <el-table-column prop="OriginalUnitPrice" label="原始單價" width="120" />
          <el-table-column prop="UnitPrice" label="調整後單價" width="120">
            <template #default="{ row }">
              <el-input-number
                v-model="row.UnitPrice"
                :min="0"
                :precision="4"
                @change="calculateTotal"
              />
            </template>
          </el-table-column>
          <el-table-column prop="Amount" label="金額" width="120">
            <template #default="{ row }">
              {{ formatCurrency((row.UnitPrice || 0) * (row.ReceiptQty || 0)) }}
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

    <!-- 審核對話框 -->
    <el-dialog
      v-model="approveDialogVisible"
      title="審核已日結採購單驗收調整"
      width="500px"
    >
      <el-form
        ref="approveFormRef"
        :model="approveForm"
        :rules="approveFormRules"
        label-width="120px"
      >
        <el-form-item label="審核人員" prop="ApproveUserId">
          <el-input v-model="approveForm.ApproveUserId" />
        </el-form-item>
        <el-form-item label="審核日期" prop="ApproveDate">
          <el-date-picker
            v-model="approveForm.ApproveDate"
            type="date"
            placeholder="請選擇審核日期"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="備註">
          <el-input v-model="approveForm.Notes" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="approveDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleApproveSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { settledAdjustmentApi } from '@/api/purchase'

export default {
  name: 'SettledPurchaseReceiptAdjustment',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const approveDialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const approveFormRef = ref(null)
    const tableData = ref([])
    const settledOrderList = ref([])
    const currentReceiptId = ref('')

    // 查詢表單
    const queryForm = reactive({
      ReceiptId: '',
      OrderId: '',
      ShopId: '',
      SupplierId: '',
      Status: '',
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
      ReceiptId: '',
      OrderId: '',
      ReceiptDate: new Date(),
      ShopId: '',
      SupplierId: '',
      ReceiptUserId: '',
      AdjustmentReason: '',
      Memo: '',
      Details: []
    })

    // 審核表單
    const approveForm = reactive({
      ApproveUserId: '',
      ApproveDate: new Date(),
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      OrderId: [{ required: true, message: '請選擇已日結採購單', trigger: 'change' }],
      ReceiptDate: [{ required: true, message: '請選擇驗收日期', trigger: 'change' }],
      AdjustmentReason: [{ required: true, message: '請輸入調整原因', trigger: 'blur' }]
    }

    // 審核表單驗證規則
    const approveFormRules = {
      ApproveUserId: [{ required: true, message: '請輸入審核人員', trigger: 'blur' }],
      ApproveDate: [{ required: true, message: '請選擇審核日期', trigger: 'change' }]
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
        'D': 'info',
        'A': 'success',
        'C': 'warning'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'D': '草稿',
        'A': '已審核',
        'C': '已日結'
      }
      return texts[status] || status
    }

    // 載入已日結採購單列表
    const loadSettledOrders = async () => {
      try {
        const response = await settledAdjustmentApi.getSettledOrders({})
        if (response.Data) {
          settledOrderList.value = response.Data || []
        }
      } catch (error) {
        console.error('載入已日結採購單失敗:', error)
      }
    }

    // 採購單變更
    const handleOrderChange = async (orderId) => {
      if (!orderId) return
      const order = settledOrderList.value.find(o => o.OrderId === orderId)
      if (order) {
        formData.ShopId = order.ShopId || ''
        formData.SupplierId = order.SupplierId || ''
        // 載入採購單明細
        // 這裡需要根據實際 API 調整
      }
    }

    // 計算總金額
    const calculateTotal = () => {
      // 此功能在提交時計算
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
        const response = await settledAdjustmentApi.getSettledAdjustments(params)
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
        OrderId: '',
        ShopId: '',
        SupplierId: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = async () => {
      isEdit.value = false
      await loadSettledOrders()
      dialogVisible.value = true
      Object.assign(formData, {
        ReceiptId: '',
        OrderId: '',
        ReceiptDate: new Date(),
        ShopId: '',
        SupplierId: '',
        ReceiptUserId: '',
        AdjustmentReason: '',
        Memo: '',
        Details: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await settledAdjustmentApi.getSettledAdjustment(row.ReceiptId)
        if (response.Data && response.Data.Receipt) {
          isEdit.value = true
          dialogVisible.value = true
          const receipt = response.Data.Receipt
          Object.assign(formData, {
            ReceiptId: receipt.ReceiptId,
            OrderId: receipt.OrderId,
            ReceiptDate: receipt.ReceiptDate,
            ShopId: receipt.ShopId,
            SupplierId: receipt.SupplierId,
            ReceiptUserId: receipt.ReceiptUserId || '',
            AdjustmentReason: receipt.AdjustmentReason || '',
            Memo: receipt.Memo || '',
            Details: receipt.Details || []
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

    // 審核
    const handleApprove = (row) => {
      currentReceiptId.value = row.ReceiptId
      approveDialogVisible.value = true
      Object.assign(approveForm, {
        ApproveUserId: '',
        ApproveDate: new Date(),
        Notes: ''
      })
    }

    // 提交審核
    const handleApproveSubmit = async () => {
      if (!approveFormRef.value) return
      await approveFormRef.value.validate(async (valid) => {
        if (valid) {
          try {
            await settledAdjustmentApi.approveSettledAdjustment(currentReceiptId.value, {
              ApproveUserId: approveForm.ApproveUserId,
              ApproveDate: approveForm.ApproveDate,
              Notes: approveForm.Notes
            })
            ElMessage.success('審核成功')
            approveDialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error('審核失敗: ' + (error.message || '未知錯誤'))
          }
        }
      })
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此調整單嗎？', '確認', {
          type: 'warning'
        })
        await settledAdjustmentApi.deleteSettledAdjustment(row.ReceiptId)
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
            const submitData = {
              OrderId: formData.OrderId,
              ReceiptDate: formData.ReceiptDate,
              ReceiptUserId: formData.ReceiptUserId,
              AdjustmentReason: formData.AdjustmentReason,
              Memo: formData.Memo,
              Details: formData.Details.map((item, index) => ({
                OrderDetailId: item.OrderDetailId,
                LineNum: index + 1,
                GoodsId: item.GoodsId,
                BarcodeId: item.BarcodeId,
                ReceiptQty: item.ReceiptQty,
                UnitPrice: item.UnitPrice,
                Memo: item.Memo
              }))
            }

            if (isEdit.value) {
              await settledAdjustmentApi.updateSettledAdjustment(formData.ReceiptId, submitData)
              ElMessage.success('修改成功')
            } else {
              await settledAdjustmentApi.createSettledAdjustment(submitData)
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
      return isEdit.value ? '修改已日結採購單驗收調整' : '新增已日結採購單驗收調整'
    })

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      approveDialogVisible,
      isEdit,
      formRef,
      approveFormRef,
      tableData,
      settledOrderList,
      queryForm,
      pagination,
      formData,
      approveForm,
      formRules,
      approveFormRules,
      dialogTitle,
      formatDate,
      formatCurrency,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleApprove,
      handleApproveSubmit,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange,
      handleOrderChange,
      calculateTotal
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.settled-adjustment {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


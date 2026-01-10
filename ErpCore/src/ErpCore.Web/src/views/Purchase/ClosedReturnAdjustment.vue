<template>
  <div class="closed-return-adjustment">
    <div class="page-header">
      <h1>已日結退貨單驗退調整作業 (SYSW530)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="驗收單號">
          <el-input v-model="queryForm.ReceiptId" placeholder="請輸入驗收單號" clearable />
        </el-form-item>
        <el-form-item label="採購單號">
          <el-input v-model="queryForm.PurchaseOrderId" placeholder="請輸入採購單號" clearable />
        </el-form-item>
        <el-form-item label="供應商代碼">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入供應商代碼" clearable />
        </el-form-item>
        <el-form-item label="分店代碼">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="庫別代碼">
          <el-input v-model="queryForm.WarehouseId" placeholder="請輸入庫別代碼" clearable />
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
        <el-table-column prop="PurchaseOrderId" label="採購單號" width="150" />
        <el-table-column prop="SupplierId" label="供應商代碼" width="120" />
        <el-table-column prop="ShopId" label="分店代碼" width="120" />
        <el-table-column prop="WarehouseId" label="庫別代碼" width="120" />
        <el-table-column prop="ApplyDate" label="申請日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ApplyDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CheckDate" label="驗收日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.CheckDate) }}
          </template>
        </el-table-column>
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
            <el-form-item label="採購單號" prop="PurchaseOrderId">
              <el-select
                v-model="formData.PurchaseOrderId"
                placeholder="請選擇已日結退貨單"
                filterable
                :disabled="isEdit"
                @change="handleOrderChange"
              >
                <el-option
                  v-for="order in closedReturnOrderList"
                  :key="order.PurchaseOrderId"
                  :label="order.PurchaseOrderId"
                  :value="order.PurchaseOrderId"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="申請日期" prop="ApplyDate">
              <el-date-picker
                v-model="formData.ApplyDate"
                type="date"
                placeholder="請選擇申請日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="驗收日期" prop="CheckDate">
              <el-date-picker
                v-model="formData.CheckDate"
                type="date"
                placeholder="請選擇驗收日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="驗收人員" prop="EmployeeId">
              <el-input v-model="formData.EmployeeId" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="供應商代碼" prop="SupplierId">
              <el-input v-model="formData.SupplierId" :disabled="true" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="分店代碼" prop="ShopId">
              <el-input v-model="formData.ShopId" :disabled="true" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="庫別代碼" prop="WarehouseId">
              <el-input v-model="formData.WarehouseId" :disabled="true" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="組織代碼" prop="OrgId">
              <el-input v-model="formData.OrgId" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註">
          <el-input v-model="formData.Notes" type="textarea" :rows="2" />
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
          <el-table-column prop="CheckQty" label="驗收數量" width="120">
            <template #default="{ row }">
              <el-input-number
                v-model="row.CheckQty"
                :min="0"
                :precision="4"
                @change="calculateTotal"
              />
            </template>
          </el-table-column>
          <el-table-column prop="UnitPrice" label="訂購單價" width="120" />
          <el-table-column prop="CheckPrice" label="驗收單價" width="120">
            <template #default="{ row }">
              <el-input-number
                v-model="row.CheckPrice"
                :min="0"
                :precision="4"
                @change="calculateTotal"
              />
            </template>
          </el-table-column>
          <el-table-column prop="TaxCode" label="稅別" width="80">
            <template #default="{ row }">
              <el-select v-model="row.TaxCode" @change="calculateTotal">
                <el-option label="應稅" value="1" />
                <el-option label="免稅" value="0" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="Amount" label="金額" width="120">
            <template #default="{ row }">
              {{ formatCurrency((row.CheckPrice || 0) * (row.CheckQty || 0)) }}
            </template>
          </el-table-column>
          <el-table-column prop="Notes" label="備註">
            <template #default="{ row }">
              <el-input v-model="row.Notes" />
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
      title="審核已日結退貨單驗退調整"
      width="500px"
    >
      <el-form
        ref="approveFormRef"
        :model="approveForm"
        :rules="approveFormRules"
        label-width="120px"
      >
        <el-form-item label="審核人員" prop="ApproveEmployeeId">
          <el-input v-model="approveForm.ApproveEmployeeId" />
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
import { closedReturnAdjustmentApi } from '@/api/purchase'

export default {
  name: 'ClosedReturnAdjustment',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const approveDialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const approveFormRef = ref(null)
    const tableData = ref([])
    const closedReturnOrderList = ref([])
    const currentReceiptId = ref('')

    // 查詢表單
    const queryForm = reactive({
      ReceiptId: '',
      PurchaseOrderId: '',
      SupplierId: '',
      ShopId: '',
      WarehouseId: '',
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
      PurchaseOrderId: '',
      SupplierId: '',
      ShopId: '',
      WarehouseId: '',
      OrgId: '',
      EmployeeId: '',
      ApplyDate: new Date(),
      CheckDate: new Date(),
      Notes: '',
      Details: []
    })

    // 審核表單
    const approveForm = reactive({
      ApproveEmployeeId: '',
      ApproveDate: new Date(),
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      PurchaseOrderId: [{ required: true, message: '請選擇已日結退貨單', trigger: 'change' }],
      ApplyDate: [{ required: true, message: '請選擇申請日期', trigger: 'change' }],
      CheckDate: [{ required: true, message: '請選擇驗收日期', trigger: 'change' }]
    }

    // 審核表單驗證規則
    const approveFormRules = {
      ApproveEmployeeId: [{ required: true, message: '請輸入審核人員', trigger: 'blur' }],
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

    // 載入已日結退貨單列表
    const loadClosedReturnOrders = async () => {
      try {
        const response = await closedReturnAdjustmentApi.getClosedReturnOrders({ Status: 'C' })
        if (response.Data) {
          closedReturnOrderList.value = response.Data || []
        }
      } catch (error) {
        console.error('載入已日結退貨單失敗:', error)
      }
    }

    // 採購單變更
    const handleOrderChange = async (purchaseOrderId) => {
      if (!purchaseOrderId) return
      const order = closedReturnOrderList.value.find(o => o.PurchaseOrderId === purchaseOrderId)
      if (order) {
        formData.SupplierId = order.SupplierId || ''
        formData.ShopId = order.ShopId || ''
        formData.WarehouseId = order.WarehouseId || ''
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
        const response = await closedReturnAdjustmentApi.getClosedReturnAdjustments(params)
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
        PurchaseOrderId: '',
        SupplierId: '',
        ShopId: '',
        WarehouseId: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = async () => {
      isEdit.value = false
      await loadClosedReturnOrders()
      dialogVisible.value = true
      Object.assign(formData, {
        ReceiptId: '',
        PurchaseOrderId: '',
        SupplierId: '',
        ShopId: '',
        WarehouseId: '',
        OrgId: '',
        EmployeeId: '',
        ApplyDate: new Date(),
        CheckDate: new Date(),
        Notes: '',
        Details: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await closedReturnAdjustmentApi.getClosedReturnAdjustment(row.ReceiptId)
        if (response.Data && response.Data.Receipt) {
          isEdit.value = true
          dialogVisible.value = true
          const receipt = response.Data.Receipt
          Object.assign(formData, {
            ReceiptId: receipt.ReceiptId,
            PurchaseOrderId: receipt.PurchaseOrderId,
            SupplierId: receipt.SupplierId,
            ShopId: receipt.ShopId,
            WarehouseId: receipt.WarehouseId,
            OrgId: receipt.OrgId || '',
            EmployeeId: receipt.EmployeeId || '',
            ApplyDate: receipt.ApplyDate,
            CheckDate: receipt.CheckDate,
            Notes: receipt.Notes || '',
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
        ApproveEmployeeId: '',
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
            await closedReturnAdjustmentApi.approveClosedReturnAdjustment(currentReceiptId.value, {
              ApproveEmployeeId: approveForm.ApproveEmployeeId,
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
        await closedReturnAdjustmentApi.deleteClosedReturnAdjustment(row.ReceiptId)
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
              PurchaseOrderId: formData.PurchaseOrderId,
              SupplierId: formData.SupplierId,
              ShopId: formData.ShopId,
              WarehouseId: formData.WarehouseId,
              OrgId: formData.OrgId,
              EmployeeId: formData.EmployeeId,
              ApplyDate: formData.ApplyDate,
              CheckDate: formData.CheckDate,
              Notes: formData.Notes,
              Details: formData.Details.map((item, index) => ({
                LineNum: index + 1,
                GoodsId: item.GoodsId,
                BarcodeId: item.BarcodeId,
                PurchaseOrderId: item.PurchaseOrderId,
                OrderQty: item.OrderQty,
                CheckQty: item.CheckQty,
                UnitPrice: item.UnitPrice,
                CheckPrice: item.CheckPrice,
                TaxCode: item.TaxCode,
                Notes: item.Notes
              }))
            }

            if (isEdit.value) {
              await closedReturnAdjustmentApi.updateClosedReturnAdjustment(formData.ReceiptId, submitData)
              ElMessage.success('修改成功')
            } else {
              await closedReturnAdjustmentApi.createClosedReturnAdjustment(submitData)
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
      return isEdit.value ? '修改已日結退貨單驗退調整' : '新增已日結退貨單驗退調整'
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
      closedReturnOrderList,
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

.closed-return-adjustment {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


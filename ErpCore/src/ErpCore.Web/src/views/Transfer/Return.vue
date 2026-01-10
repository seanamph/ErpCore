<template>
  <div class="transfer-return">
    <div class="page-header">
      <h1>調撥單驗退作業 (SYSW362)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="驗退單號">
          <el-input v-model="queryForm.ReturnId" placeholder="請輸入驗退單號" clearable />
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
            <el-option label="待驗退" value="P" />
            <el-option label="部分驗退" value="R" />
            <el-option label="已驗退" value="C" />
            <el-option label="已取消" value="X" />
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
        <el-table-column prop="ReturnId" label="驗退單號" width="150" />
        <el-table-column prop="TransferId" label="調撥單號" width="150" />
        <el-table-column prop="ReturnDate" label="驗退日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ReturnDate) }}
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
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.IsSettled || row.Status === 'X'">修改</el-button>
            <el-button type="success" size="small" @click="handleConfirm(row)" :disabled="row.Status === 'C' || row.IsSettled">確認</el-button>
            <el-button type="info" size="small" @click="handleCancel(row)" :disabled="row.Status === 'X' || row.IsSettled">取消</el-button>
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
            <el-form-item label="驗退日期" prop="ReturnDate">
              <el-date-picker
                v-model="formData.ReturnDate"
                type="date"
                placeholder="請選擇驗退日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="驗退人員" prop="ReturnUserId">
              <el-input v-model="formData.ReturnUserId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="驗退原因">
              <el-input v-model="formData.ReturnReason" type="textarea" :rows="2" />
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
          <el-table-column prop="ReceiptQty" label="原驗收數量" width="120" />
          <el-table-column prop="ReturnQty" label="驗退數量" width="120">
            <template #default="{ row }">
              <el-input-number
                v-model="row.ReturnQty"
                :min="0"
                :max="row.ReceiptQty"
                :precision="2"
                @change="calculateTotal"
              />
            </template>
          </el-table-column>
          <el-table-column prop="UnitPrice" label="單價" width="120">
            <template #default="{ row }">
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
              {{ formatCurrency((row.UnitPrice || 0) * row.ReturnQty) }}
            </template>
          </el-table-column>
          <el-table-column prop="ReturnReason" label="驗退原因">
            <template #default="{ row }">
              <el-input v-model="row.ReturnReason" />
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
import { transferReturnApi } from '@/api/transfer'

export default {
  name: 'TransferReturn',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      ReturnId: '',
      TransferId: '',
      FromShopId: '',
      ToShopId: '',
      Status: '',
      ReturnDateFrom: null,
      ReturnDateTo: null,
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
      ReceiptId: '',
      ReturnDate: new Date(),
      ReturnUserId: '',
      ReturnReason: '',
      Memo: '',
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      TransferId: [{ required: true, message: '請輸入調撥單號', trigger: 'blur' }],
      ReturnDate: [{ required: true, message: '請選擇驗退日期', trigger: 'change' }]
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
        'P': '待驗退',
        'R': '部分驗退',
        'C': '已驗退',
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
        const response = await transferReturnApi.getTransferReturns(params)
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
        ReturnId: '',
        TransferId: '',
        FromShopId: '',
        ToShopId: '',
        Status: '',
        ReturnDateFrom: null,
        ReturnDateTo: null
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        TransferId: '',
        ReceiptId: '',
        ReturnDate: new Date(),
        ReturnUserId: '',
        ReturnReason: '',
        Memo: '',
        Details: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await transferReturnApi.getTransferReturn(row.ReturnId)
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

    // 確認驗退
    const handleConfirm = async (row) => {
      try {
        await ElMessageBox.confirm('確定要確認此驗退單嗎？', '確認', {
          type: 'warning'
        })
        await transferReturnApi.confirmReturn(row.ReturnId)
        ElMessage.success('確認成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('確認失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 取消驗退單
    const handleCancel = async (row) => {
      try {
        await ElMessageBox.confirm('確定要取消此驗退單嗎？', '確認', {
          type: 'warning'
        })
        await transferReturnApi.cancelTransferReturn(row.ReturnId)
        ElMessage.success('取消成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('取消失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此驗退單嗎？', '確認', {
          type: 'warning'
        })
        await transferReturnApi.deleteTransferReturn(row.ReturnId)
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
            if (isEdit.value) {
              await transferReturnApi.updateTransferReturn(formData.ReturnId, {
                ReturnDate: formData.ReturnDate,
                ReturnUserId: formData.ReturnUserId,
                ReturnReason: formData.ReturnReason,
                Memo: formData.Memo,
                Details: formData.Details.map(item => ({
                  DetailId: item.DetailId,
                  ReturnQty: item.ReturnQty,
                  UnitPrice: item.UnitPrice,
                  ReturnReason: item.ReturnReason,
                  Memo: item.Memo
                }))
              })
              ElMessage.success('修改成功')
            } else {
              await transferReturnApi.createTransferReturn({
                TransferId: formData.TransferId,
                ReceiptId: formData.ReceiptId,
                ReturnDate: formData.ReturnDate,
                ReturnUserId: formData.ReturnUserId,
                ReturnReason: formData.ReturnReason,
                Memo: formData.Memo,
                Details: formData.Details.map((item, index) => ({
                  TransferDetailId: item.TransferDetailId,
                  ReceiptDetailId: item.ReceiptDetailId,
                  LineNum: index + 1,
                  GoodsId: item.GoodsId,
                  BarcodeId: item.BarcodeId,
                  ReturnQty: item.ReturnQty,
                  UnitPrice: item.UnitPrice,
                  ReturnReason: item.ReturnReason,
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
      return isEdit.value ? '修改驗退單' : '新增驗退單'
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
      handleCancel,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.transfer-return {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>


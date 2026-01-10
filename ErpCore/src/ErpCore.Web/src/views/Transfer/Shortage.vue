<template>
  <div class="transfer-shortage">
    <div class="page-header">
      <h1>調撥短溢維護作業 (SYSW384)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="短溢單號">
          <el-input v-model="queryForm.ShortageId" placeholder="請輸入短溢單號" clearable />
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
            <el-option label="待處理" value="P" />
            <el-option label="已審核" value="A" />
            <el-option label="已處理" value="C" />
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
        <el-table-column prop="ShortageId" label="短溢單號" width="150" />
        <el-table-column prop="TransferId" label="調撥單號" width="150" />
        <el-table-column prop="ShortageDate" label="短溢日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ShortageDate) }}
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
        <el-table-column prop="TotalShortageQty" label="短溢數量" width="120" />
        <el-table-column prop="TotalAmount" label="總金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.IsSettled || row.Status === 'C'">修改</el-button>
            <el-button type="success" size="small" @click="handleApprove(row)" :disabled="row.Status === 'A' || row.Status === 'C' || row.IsSettled">審核</el-button>
            <el-button type="info" size="small" @click="handleProcess(row)" :disabled="row.Status !== 'A' || row.IsSettled">處理</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.IsSettled || row.Status === 'C'">刪除</el-button>
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
            <el-form-item label="短溢日期" prop="ShortageDate">
              <el-date-picker
                v-model="formData.ShortageDate"
                type="date"
                placeholder="請選擇短溢日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="處理類型" prop="ProcessType">
              <el-select v-model="formData.ProcessType" placeholder="請選擇處理類型" clearable>
                <el-option label="短少" value="S" />
                <el-option label="溢收" value="O" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="短溢原因">
              <el-input v-model="formData.ShortageReason" type="textarea" :rows="2" />
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
          <el-table-column prop="TransferQty" label="調撥數量" width="120" />
          <el-table-column prop="ReceiptQty" label="驗收數量" width="120" />
          <el-table-column prop="ShortageQty" label="短溢數量" width="120">
            <template #default="{ row }">
              <el-input-number
                v-model="row.ShortageQty"
                :min="0"
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
              {{ formatCurrency((row.UnitPrice || 0) * row.ShortageQty) }}
            </template>
          </el-table-column>
          <el-table-column prop="ShortageReason" label="短溢原因">
            <template #default="{ row }">
              <el-input v-model="row.ShortageReason" />
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
import { transferShortageApi } from '@/api/transfer'
import { getCurrentUser } from '@/api/users'

export default {
  name: 'TransferShortage',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const currentUser = ref(null)

    // 查詢表單
    const queryForm = reactive({
      ShortageId: '',
      TransferId: '',
      FromShopId: '',
      ToShopId: '',
      Status: '',
      ProcessType: '',
      ShortageDateFrom: null,
      ShortageDateTo: null,
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
      ShortageDate: new Date(),
      ProcessType: '',
      ShortageReason: '',
      Memo: '',
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      TransferId: [{ required: true, message: '請輸入調撥單號', trigger: 'blur' }],
      ShortageDate: [{ required: true, message: '請選擇短溢日期', trigger: 'change' }]
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
        'A': 'success',
        'C': 'warning'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'P': '待處理',
        'A': '已審核',
        'C': '已處理'
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
        const response = await transferShortageApi.getTransferShortages(params)
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
        ShortageId: '',
        TransferId: '',
        FromShopId: '',
        ToShopId: '',
        Status: '',
        ProcessType: '',
        ShortageDateFrom: null,
        ShortageDateTo: null
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
        ShortageDate: new Date(),
        ProcessType: '',
        ShortageReason: '',
        Memo: '',
        Details: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await transferShortageApi.getTransferShortage(row.ShortageId)
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

    // 取得當前使用者資訊
    const loadCurrentUser = async () => {
      try {
        if (!currentUser.value) {
          const response = await getCurrentUser()
          if (response.Data) {
            currentUser.value = response.Data
          }
        }
      } catch (error) {
        console.error('取得使用者資訊失敗:', error)
      }
    }

    // 審核
    const handleApprove = async (row) => {
      try {
        await ElMessageBox.confirm('確定要審核此短溢單嗎？', '確認', {
          type: 'warning'
        })
        await loadCurrentUser()
        const approveData = {
          ApproveUserId: currentUser.value?.UserId || '',
          ApproveDate: new Date(),
          Notes: ''
        }
        await transferShortageApi.approveShortage(row.ShortageId, approveData)
        ElMessage.success('審核成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('審核失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 處理
    const handleProcess = async (row) => {
      try {
        await ElMessageBox.confirm('確定要處理此短溢單嗎？', '確認', {
          type: 'warning'
        })
        await loadCurrentUser()
        const processData = {
          ProcessUserId: currentUser.value?.UserId || '',
          ProcessDate: new Date(),
          ProcessType: row.ProcessType || 'ADJUST', // ADJUST:調整庫存, PENDING:待處理, PROCESSED:已處理
          Notes: ''
        }
        await transferShortageApi.processShortage(row.ShortageId, processData)
        ElMessage.success('處理成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('處理失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此短溢單嗎？', '確認', {
          type: 'warning'
        })
        await transferShortageApi.deleteTransferShortage(row.ShortageId)
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
              await transferShortageApi.updateTransferShortage(formData.ShortageId, {
                ShortageDate: formData.ShortageDate,
                ProcessType: formData.ProcessType,
                ShortageReason: formData.ShortageReason,
                Memo: formData.Memo,
                Details: formData.Details.map(item => ({
                  DetailId: item.DetailId,
                  ShortageQty: item.ShortageQty,
                  UnitPrice: item.UnitPrice,
                  ShortageReason: item.ShortageReason,
                  Memo: item.Memo
                }))
              })
              ElMessage.success('修改成功')
            } else {
              await transferShortageApi.createTransferShortage({
                TransferId: formData.TransferId,
                ReceiptId: formData.ReceiptId,
                ShortageDate: formData.ShortageDate,
                ProcessType: formData.ProcessType,
                ShortageReason: formData.ShortageReason,
                Memo: formData.Memo,
                Details: formData.Details.map((item, index) => ({
                  TransferDetailId: item.TransferDetailId,
                  ReceiptDetailId: item.ReceiptDetailId,
                  LineNum: index + 1,
                  GoodsId: item.GoodsId,
                  BarcodeId: item.BarcodeId,
                  TransferQty: item.TransferQty,
                  ReceiptQty: item.ReceiptQty,
                  ShortageQty: item.ShortageQty,
                  UnitPrice: item.UnitPrice,
                  ShortageReason: item.ShortageReason,
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
      return isEdit.value ? '修改短溢單' : '新增短溢單'
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
      handleApprove,
      handleProcess,
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

.transfer-shortage {
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


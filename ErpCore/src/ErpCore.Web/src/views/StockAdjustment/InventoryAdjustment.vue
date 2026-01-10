<template>
  <div class="inventory-adjustment">
    <div class="page-header">
      <h1>庫存調整作業 (SYSW490)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="調整單號">
          <el-input v-model="queryForm.AdjustmentId" placeholder="請輸入調整單號" clearable />
        </el-form-item>
        <el-form-item label="分店代碼">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="D" />
            <el-option label="已確認" value="C" />
            <el-option label="已取消" value="X" />
          </el-select>
        </el-form-item>
        <el-form-item label="調整日期">
          <el-date-picker
            v-model="queryForm.AdjustmentDateFrom"
            type="date"
            placeholder="開始日期"
            style="width: 150px"
          />
          <span style="margin: 0 10px">-</span>
          <el-date-picker
            v-model="queryForm.AdjustmentDateTo"
            type="date"
            placeholder="結束日期"
            style="width: 150px"
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
        <el-table-column prop="AdjustmentId" label="調整單號" width="150" />
        <el-table-column prop="AdjustmentDate" label="調整日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.AdjustmentDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ShopId" label="分店代碼" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ row.StatusName || getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="AdjustmentType" label="調整類型" width="120" />
        <el-table-column prop="AdjustmentUser" label="調整人員" width="120" />
        <el-table-column prop="TotalQty" label="總調整數量" width="120" />
        <el-table-column prop="TotalCost" label="總調整成本" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalCost) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總調整金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status !== 'D'">修改</el-button>
            <el-button type="success" size="small" @click="handleConfirm(row)" :disabled="row.Status === 'C' || row.Status === 'X'">確認</el-button>
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
            <el-form-item label="調整單號" prop="AdjustmentId">
              <el-input v-model="formData.AdjustmentId" :disabled="isEdit" placeholder="自動產生" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="調整日期" prop="AdjustmentDate">
              <el-date-picker
                v-model="formData.AdjustmentDate"
                type="date"
                placeholder="請選擇調整日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="分店代碼" prop="ShopId">
              <el-input v-model="formData.ShopId" placeholder="請輸入分店代碼" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="調整類型" prop="AdjustmentType">
              <el-select v-model="formData.AdjustmentType" placeholder="請選擇調整類型" clearable>
                <el-option label="手動調整" value="MANUAL" />
                <el-option label="盤點調整" value="STOCKTAKING" />
                <el-option label="調撥調整" value="TRANSFER" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="調整人員" prop="AdjustmentUser">
              <el-input v-model="formData.AdjustmentUser" placeholder="請輸入調整人員" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="來源單號">
              <el-input v-model="formData.SourceNo" placeholder="請輸入來源單號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註">
          <el-input v-model="formData.Memo" type="textarea" :rows="2" placeholder="請輸入備註" />
        </el-form-item>

        <!-- 明細表格 -->
        <el-form-item label="明細">
          <el-table
            :data="formData.Details"
            border
            style="margin-top: 10px"
          >
            <el-table-column type="index" label="序號" width="60" />
            <el-table-column prop="GoodsId" label="商品編號" width="150">
              <template #default="{ row, $index }">
                <el-input v-model="row.GoodsId" placeholder="請輸入商品編號" />
              </template>
            </el-table-column>
            <el-table-column prop="BarcodeId" label="條碼" width="120">
              <template #default="{ row }">
                <el-input v-model="row.BarcodeId" placeholder="請輸入條碼" />
              </template>
            </el-table-column>
            <el-table-column prop="BeforeQty" label="調整前數量" width="120">
              <template #default="{ row }">
                {{ formatNumber(row.BeforeQty) }}
              </template>
            </el-table-column>
            <el-table-column prop="AdjustmentQty" label="調整數量" width="120">
              <template #default="{ row, $index }">
                <el-input-number
                  v-model="row.AdjustmentQty"
                  :precision="4"
                  @change="handleQtyChange(row, $index)"
                />
              </template>
            </el-table-column>
            <el-table-column prop="AfterQty" label="調整後數量" width="120">
              <template #default="{ row }">
                {{ formatNumber(row.AfterQty) }}
              </template>
            </el-table-column>
            <el-table-column prop="UnitCost" label="單位成本" width="120">
              <template #default="{ row, $index }">
                <el-input-number
                  v-model="row.UnitCost"
                  :precision="4"
                  @change="handleCostChange(row, $index)"
                />
              </template>
            </el-table-column>
            <el-table-column prop="AdjustmentCost" label="調整成本" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.AdjustmentCost) }}
              </template>
            </el-table-column>
            <el-table-column prop="AdjustmentAmount" label="調整金額" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.AdjustmentAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="Reason" label="調整原因" width="150">
              <template #default="{ row }">
                <el-select v-model="row.Reason" placeholder="請選擇原因" clearable filterable>
                  <el-option
                    v-for="reason in reasonList"
                    :key="reason.ReasonId"
                    :label="reason.ReasonName"
                    :value="reason.ReasonId"
                  />
                </el-select>
              </template>
            </el-table-column>
            <el-table-column prop="Memo" label="備註">
              <template #default="{ row }">
                <el-input v-model="row.Memo" placeholder="請輸入備註" />
              </template>
            </el-table-column>
            <el-table-column label="操作" width="100" fixed="right">
              <template #default="{ $index }">
                <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-button type="primary" @click="handleAddDetail" style="margin-top: 10px">新增明細</el-button>
        </el-form-item>
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
import {
  getInventoryAdjustments,
  getInventoryAdjustment,
  createInventoryAdjustment,
  updateInventoryAdjustment,
  deleteInventoryAdjustment,
  confirmAdjustment,
  getAdjustmentReasons
} from '@/api/stockAdjustment'

export default {
  name: 'InventoryAdjustment',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const reasonList = ref([])

    // 查詢表單
    const queryForm = reactive({
      AdjustmentId: '',
      ShopId: '',
      Status: '',
      AdjustmentDateFrom: null,
      AdjustmentDateTo: null,
      AdjustmentUser: '',
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
      AdjustmentId: '',
      AdjustmentDate: new Date(),
      ShopId: '',
      AdjustmentType: '',
      AdjustmentUser: '',
      Memo: '',
      Memo2: '',
      SourceNo: '',
      SourceNum: '',
      SourceCheckDate: null,
      SourceSuppId: '',
      SiteId: '',
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      AdjustmentDate: [{ required: true, message: '請選擇調整日期', trigger: 'change' }],
      ShopId: [{ required: true, message: '請輸入分店代碼', trigger: 'blur' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改調整單' : '新增調整單'
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
      if (!num && num !== 0) return ''
      return Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 4, maximumFractionDigits: 4 })
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'D': 'info',
        'C': 'success',
        'X': 'danger'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'D': '草稿',
        'C': '已確認',
        'X': '已取消'
      }
      return texts[status] || status
    }

    // 載入調整原因列表
    const loadReasons = async () => {
      try {
        const response = await getAdjustmentReasons()
        if (response.Data) {
          reasonList.value = response.Data
        }
      } catch (error) {
        console.error('載入調整原因失敗:', error)
      }
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
        const response = await getInventoryAdjustments(params)
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
      queryForm.AdjustmentId = ''
      queryForm.ShopId = ''
      queryForm.Status = ''
      queryForm.AdjustmentDateFrom = null
      queryForm.AdjustmentDateTo = null
      queryForm.AdjustmentUser = ''
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      formData.AdjustmentId = ''
      formData.AdjustmentDate = new Date()
      formData.ShopId = ''
      formData.AdjustmentType = ''
      formData.AdjustmentUser = ''
      formData.Memo = ''
      formData.Memo2 = ''
      formData.SourceNo = ''
      formData.SourceNum = ''
      formData.SourceCheckDate = null
      formData.SourceSuppId = ''
      formData.SiteId = ''
      formData.Details = []
      dialogVisible.value = true
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await getInventoryAdjustment(row.AdjustmentId)
        if (response.Data) {
          isEdit.value = true
          Object.assign(formData, {
            AdjustmentId: response.Data.AdjustmentId,
            AdjustmentDate: new Date(response.Data.AdjustmentDate),
            ShopId: response.Data.ShopId,
            AdjustmentType: response.Data.AdjustmentType,
            AdjustmentUser: response.Data.AdjustmentUser,
            Memo: response.Data.Memo,
            Memo2: response.Data.Memo2,
            SourceNo: response.Data.SourceNo,
            SourceNum: response.Data.SourceNum,
            SourceCheckDate: response.Data.SourceCheckDate ? new Date(response.Data.SourceCheckDate) : null,
            SourceSuppId: response.Data.SourceSuppId,
            SiteId: response.Data.SiteId,
            Details: response.Data.Details || []
          })
          dialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = (row) => {
      handleView(row)
    }

    // 確認
    const handleConfirm = async (row) => {
      try {
        await ElMessageBox.confirm('確定要確認此調整單嗎？確認後將更新庫存。', '確認', {
          type: 'warning'
        })
        await confirmAdjustment(row.AdjustmentId)
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
        await ElMessageBox.confirm('確定要刪除此調整單嗎？', '確認', {
          type: 'warning'
        })
        await deleteInventoryAdjustment(row.AdjustmentId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 新增明細
    const handleAddDetail = () => {
      formData.Details.push({
        GoodsId: '',
        BarcodeId: '',
        AdjustmentQty: 0,
        BeforeQty: null,
        AfterQty: null,
        UnitCost: null,
        AdjustmentCost: null,
        AdjustmentAmount: null,
        Reason: '',
        Memo: ''
      })
    }

    // 刪除明細
    const handleRemoveDetail = (index) => {
      formData.Details.splice(index, 1)
    }

    // 數量變更
    const handleQtyChange = (row, index) => {
      if (row.UnitCost !== null && row.UnitCost !== undefined) {
        row.AdjustmentCost = (row.UnitCost || 0) * row.AdjustmentQty
        row.AdjustmentAmount = (row.UnitCost || 0) * row.AdjustmentQty
      }
      if (row.BeforeQty !== null && row.BeforeQty !== undefined) {
        row.AfterQty = row.BeforeQty + row.AdjustmentQty
      }
    }

    // 成本變更
    const handleCostChange = (row, index) => {
      row.AdjustmentCost = (row.UnitCost || 0) * row.AdjustmentQty
      row.AdjustmentAmount = (row.UnitCost || 0) * row.AdjustmentQty
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return

      try {
        await formRef.value.validate()
        if (formData.Details.length === 0) {
          ElMessage.warning('請至少新增一筆明細')
          return
        }

        const submitData = {
          AdjustmentDate: formData.AdjustmentDate,
          ShopId: formData.ShopId,
          AdjustmentType: formData.AdjustmentType,
          AdjustmentUser: formData.AdjustmentUser,
          Memo: formData.Memo,
          Memo2: formData.Memo2,
          SourceNo: formData.SourceNo,
          SourceNum: formData.SourceNum,
          SourceCheckDate: formData.SourceCheckDate,
          SourceSuppId: formData.SourceSuppId,
          SiteId: formData.SiteId,
          Details: formData.Details.map(d => ({
            GoodsId: d.GoodsId,
            BarcodeId: d.BarcodeId,
            AdjustmentQty: d.AdjustmentQty,
            UnitCost: d.UnitCost,
            Reason: d.Reason,
            Memo: d.Memo
          }))
        }

        if (isEdit.value) {
          await updateInventoryAdjustment(formData.AdjustmentId, submitData)
          ElMessage.success('修改成功')
        } else {
          await createInventoryAdjustment(submitData)
          ElMessage.success('新增成功')
        }

        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
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
      loadReasons()
      loadData()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      reasonList,
      queryForm,
      pagination,
      formData,
      formRules,
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
      handleConfirm,
      handleDelete,
      handleAddDetail,
      handleRemoveDetail,
      handleQtyChange,
      handleCostChange,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.inventory-adjustment {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      margin: 0;
      font-size: 24px;
      font-weight: 500;
      color: $primary-color;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
  }
}
</style>


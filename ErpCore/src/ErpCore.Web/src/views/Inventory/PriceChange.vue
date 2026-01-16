<template>
  <div class="price-change">
    <div class="page-header">
      <h1>商品永久變價作業 (SYSW150)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="變價單號">
          <el-input v-model="queryForm.PriceChangeId" placeholder="請輸入變價單號" clearable />
        </el-form-item>
        <el-form-item label="變價類型">
          <el-select v-model="queryForm.PriceChangeType" placeholder="請選擇變價類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="進價" value="1" />
            <el-option label="售價" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="廠商">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入廠商編號" clearable />
        </el-form-item>
        <el-form-item label="品牌">
          <el-input v-model="queryForm.LogoId" placeholder="請輸入品牌編號" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="已申請" value="1" />
            <el-option label="已審核" value="2" />
            <el-option label="已確認" value="10" />
            <el-option label="已作廢" value="9" />
          </el-select>
        </el-form-item>
        <el-form-item label="申請日期">
          <el-date-picker
            v-model="queryForm.ApplyDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            clearable
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
        <el-table-column prop="PriceChangeId" label="變價單號" width="150" fixed="left" />
        <el-table-column prop="PriceChangeTypeName" label="變價類型" width="100" />
        <el-table-column prop="SupplierId" label="廠商編號" width="120" />
        <el-table-column prop="LogoId" label="品牌編號" width="120" />
        <el-table-column prop="ApplyEmpId" label="申請人" width="100" />
        <el-table-column prop="ApplyDate" label="申請日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ApplyDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="StartDate" label="啟用日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.StartDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="StatusName" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ row.StatusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="350" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button v-if="row.Status === '1'" type="warning" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button v-if="row.Status === '1'" type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
            <el-button v-if="row.Status === '1'" type="success" size="small" @click="handleApprove(row)">審核</el-button>
            <el-button v-if="row.Status === '2'" type="success" size="small" @click="handleConfirm(row)">確認</el-button>
            <el-button v-if="row.Status !== '9' && row.Status !== '10'" type="info" size="small" @click="handleCancel(row)">作廢</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>

    <!-- 新增/修改對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="1200px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="變價單號" prop="PriceChangeId">
              <el-input v-model="form.PriceChangeId" placeholder="自動編號" disabled />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="變價類型" prop="PriceChangeType">
              <el-select v-model="form.PriceChangeType" placeholder="請選擇變價類型" :disabled="isEdit" style="width: 100%">
                <el-option label="進價" value="1" />
                <el-option label="售價" value="2" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="品牌編號" prop="LogoId">
              <el-input v-model="form.LogoId" placeholder="請輸入品牌編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="廠商編號" prop="SupplierId">
              <el-input v-model="form.SupplierId" placeholder="請輸入廠商編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="申請日期" prop="ApplyDate">
              <el-date-picker
                v-model="form.ApplyDate"
                type="date"
                placeholder="請選擇日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="啟用日期" prop="StartDate">
              <el-date-picker
                v-model="form.StartDate"
                type="date"
                placeholder="請選擇日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
        
        <!-- 明細表格 -->
        <el-divider>變價明細</el-divider>
        <el-table :data="form.Details" border style="width: 100%">
          <el-table-column type="index" label="序號" width="80" />
          <el-table-column label="商品編號" width="150">
            <template #default="{ row, $index }">
              <el-input v-model="row.GoodsId" placeholder="請輸入商品編號" @blur="handleGoodsChange($index, row.GoodsId)" />
            </template>
          </el-table-column>
          <el-table-column label="商品名稱" width="200">
            <template #default="{ row }">
              {{ row.GoodsName || '-' }}
            </template>
          </el-table-column>
          <el-table-column label="調整前單價" width="120" align="right">
            <template #default="{ row }">
              <el-input-number v-model="row.BeforePrice" :precision="4" :min="0" :disabled="true" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="調整後單價" width="120" align="right">
            <template #default="{ row }">
              <el-input-number v-model="row.AfterPrice" :precision="4" :min="0" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="變價數量" width="120" align="right">
            <template #default="{ row }">
              <el-input-number v-model="row.ChangeQty" :precision="4" :min="0" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="備註" width="200">
            <template #default="{ row }">
              <el-input v-model="row.Notes" placeholder="請輸入備註" />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" fixed="right">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" @click="handleAddDetail" style="margin-top: 10px;">新增明細</el-button>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { priceChangeApi } from '@/api/inventory'
import { productGoodsIdApi } from '@/api/inventory'

// 查詢表單
const queryForm = reactive({
  PriceChangeId: '',
  PriceChangeType: '',
  SupplierId: '',
  LogoId: '',
  Status: '',
  ApplyDateRange: null
})

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增變價單')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  PriceChangeId: '',
  PriceChangeType: '1',
  SupplierId: '',
  LogoId: '',
  ApplyDate: '',
  StartDate: '',
  Notes: '',
  Details: []
})

// 表單驗證規則
const rules = {
  PriceChangeType: [{ required: true, message: '請選擇變價類型', trigger: 'change' }],
  ApplyDate: [{ required: true, message: '請選擇申請日期', trigger: 'change' }]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      PriceChangeId: queryForm.PriceChangeId || undefined,
      PriceChangeType: queryForm.PriceChangeType || undefined,
      SupplierId: queryForm.SupplierId || undefined,
      LogoId: queryForm.LogoId || undefined,
      Status: queryForm.Status || undefined,
      ApplyDateFrom: queryForm.ApplyDateRange?.[0] || undefined,
      ApplyDateTo: queryForm.ApplyDateRange?.[1] || undefined
    }
    const response = await priceChangeApi.getPriceChanges(params)
    if (response.data && response.data.Success !== false) {
      const data = response.data.Data || response.data
      tableData.value = data.Items || data.items || []
      pagination.TotalCount = data.TotalCount || data.totalCount || 0
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.PriceChangeId = ''
  queryForm.PriceChangeType = ''
  queryForm.SupplierId = ''
  queryForm.LogoId = ''
  queryForm.Status = ''
  queryForm.ApplyDateRange = null
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增變價單'
  resetForm()
  dialogVisible.value = true
}

// 查看
const handleView = async (row) => {
  await loadPriceChangeData(row.PriceChangeId, row.PriceChangeType, false)
}

// 修改
const handleEdit = async (row) => {
  if (row.Status !== '1') {
    ElMessage.warning('只有狀態為「已申請」的變價單可以修改')
    return
  }
  isEdit.value = true
  dialogTitle.value = '修改變價單'
  await loadPriceChangeData(row.PriceChangeId, row.PriceChangeType, true)
}

// 載入變價單資料
const loadPriceChangeData = async (priceChangeId, priceChangeType, isEditMode) => {
  try {
    const response = await priceChangeApi.getPriceChangeById(priceChangeId, priceChangeType)
    if (response.data && response.data.Success !== false) {
      const data = response.data.Data || response.data
      form.PriceChangeId = data.PriceChangeId || ''
      form.PriceChangeType = data.PriceChangeType || '1'
      form.SupplierId = data.SupplierId || ''
      form.LogoId = data.LogoId || ''
      form.ApplyDate = data.ApplyDate ? formatDateForInput(data.ApplyDate) : ''
      form.StartDate = data.StartDate ? formatDateForInput(data.StartDate) : ''
      form.Notes = data.Notes || ''
      form.Details = (data.Details || []).map((d, index) => ({
        LineNum: d.LineNum || index + 1,
        GoodsId: d.GoodsId || '',
        GoodsName: d.GoodsName || '',
        BeforePrice: d.BeforePrice || 0,
        AfterPrice: d.AfterPrice || 0,
        ChangeQty: d.ChangeQty || 0,
        Notes: d.Notes || ''
      }))
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  }
  if (isEditMode) {
    dialogVisible.value = true
  }
}

// 刪除
const handleDelete = async (row) => {
  if (row.Status !== '1') {
    ElMessage.warning('只有狀態為「已申請」的變價單可以刪除')
    return
  }
  try {
    await ElMessageBox.confirm('確定要刪除此變價單嗎？', '提示', {
      type: 'warning'
    })
    const response = await priceChangeApi.deletePriceChange(row.PriceChangeId, row.PriceChangeType)
    if (response.data && response.data.Success !== false) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 審核
const handleApprove = async (row) => {
  try {
    await ElMessageBox.confirm('確定要審核此變價單嗎？', '提示', {
      type: 'warning'
    })
    const response = await priceChangeApi.approvePriceChange(row.PriceChangeId, row.PriceChangeType, {
      ApproveDate: new Date().toISOString().split('T')[0]
    })
    if (response.data && response.data.Success !== false) {
      ElMessage.success('審核成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '審核失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('審核失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 確認
const handleConfirm = async (row) => {
  try {
    await ElMessageBox.confirm('確定要確認此變價單嗎？確認後將更新商品價格。', '提示', {
      type: 'warning'
    })
    const response = await priceChangeApi.confirmPriceChange(row.PriceChangeId, row.PriceChangeType, {
      ConfirmDate: new Date().toISOString().split('T')[0]
    })
    if (response.data && response.data.Success !== false) {
      ElMessage.success('確認成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '確認失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('確認失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 作廢
const handleCancel = async (row) => {
  try {
    await ElMessageBox.confirm('確定要作廢此變價單嗎？', '提示', {
      type: 'warning'
    })
    const response = await priceChangeApi.cancelPriceChange(row.PriceChangeId, row.PriceChangeType)
    if (response.data && response.data.Success !== false) {
      ElMessage.success('作廢成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '作廢失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('作廢失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      if (!form.Details || form.Details.length === 0) {
        ElMessage.warning('請至少新增一筆變價明細')
        return
      }
      try {
        let response
        if (isEdit.value) {
          const updateData = {
            SupplierId: form.SupplierId,
            LogoId: form.LogoId,
            ApplyDate: form.ApplyDate,
            StartDate: form.StartDate,
            Notes: form.Notes,
            Details: form.Details.map((d, index) => ({
              LineNum: index + 1,
              GoodsId: d.GoodsId,
              BeforePrice: d.BeforePrice,
              AfterPrice: d.AfterPrice,
              ChangeQty: d.ChangeQty,
              Notes: d.Notes
            }))
          }
          response = await priceChangeApi.updatePriceChange(form.PriceChangeId, form.PriceChangeType, updateData)
        } else {
          const createData = {
            PriceChangeType: form.PriceChangeType,
            SupplierId: form.SupplierId,
            LogoId: form.LogoId,
            ApplyDate: form.ApplyDate,
            StartDate: form.StartDate,
            Notes: form.Notes,
            Details: form.Details.map((d, index) => ({
              LineNum: index + 1,
              GoodsId: d.GoodsId,
              BeforePrice: d.BeforePrice,
              AfterPrice: d.AfterPrice,
              ChangeQty: d.ChangeQty,
              Notes: d.Notes
            }))
          }
          response = await priceChangeApi.createPriceChange(createData)
        }
        if (response.data && response.data.Success !== false) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.Message || response.data?.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + (error.message || '未知錯誤'))
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 重置表單
const resetForm = () => {
  form.PriceChangeId = ''
  form.PriceChangeType = '1'
  form.SupplierId = ''
  form.LogoId = ''
  form.ApplyDate = ''
  form.StartDate = ''
  form.Notes = ''
  form.Details = []
  formRef.value?.resetFields()
}

// 新增明細
const handleAddDetail = () => {
  form.Details.push({
    LineNum: form.Details.length + 1,
    GoodsId: '',
    GoodsName: '',
    BeforePrice: 0,
    AfterPrice: 0,
    ChangeQty: 0,
    Notes: ''
  })
}

// 刪除明細
const handleDeleteDetail = (index) => {
  form.Details.splice(index, 1)
  // 重新編號
  form.Details.forEach((d, i) => {
    d.LineNum = i + 1
  })
}

// 商品變更
const handleGoodsChange = async (index, goodsId) => {
  if (!goodsId) return
  try {
    const response = await productGoodsIdApi.getProductGoodsIdById(goodsId)
    if (response.data && response.data.Success !== false) {
      const data = response.data.Data || response.data
      const detail = form.Details[index]
      detail.GoodsName = data.GoodsName || ''
      if (form.PriceChangeType === '1') {
        detail.BeforePrice = data.Lprc || 0
      } else {
        detail.BeforePrice = data.Mprc || 0
      }
    }
  } catch (error) {
    // 商品不存在時不顯示錯誤，只清空商品名稱
    form.Details[index].GoodsName = ''
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

// 格式化貨幣
const formatCurrency = (value) => {
  if (value == null) return '0.00'
  return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 格式化日期
const formatDate = (value) => {
  if (!value) return '-'
  if (typeof value === 'string') {
    return value.split('T')[0]
  }
  return value
}

// 格式化日期為輸入格式
const formatDateForInput = (value) => {
  if (!value) return ''
  if (typeof value === 'string') {
    return value.split('T')[0]
  }
  return value
}

// 取得狀態類型
const getStatusType = (status) => {
  switch (status) {
    case '1': return 'warning' // 已申請
    case '2': return 'info' // 已審核
    case '10': return 'success' // 已確認
    case '9': return 'danger' // 已作廢
    default: return ''
  }
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.price-change {
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

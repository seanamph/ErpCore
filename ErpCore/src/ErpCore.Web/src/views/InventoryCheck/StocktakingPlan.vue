<template>
  <div class="stocktaking-plan">
    <div class="page-header">
      <h1>盤點維護作業 (SYSW53M)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="盤點計劃單號">
          <el-input v-model="queryForm.PlanId" placeholder="請輸入盤點計劃單號" clearable />
        </el-form-item>
        <el-form-item label="盤點日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.PlanStatus" placeholder="請選擇狀態" clearable>
            <el-option label="申請中" value="-1" />
            <el-option label="未審核" value="0" />
            <el-option label="已審核" value="1" />
            <el-option label="作廢" value="4" />
            <el-option label="結案" value="5" />
          </el-select>
        </el-form-item>
        <el-form-item label="店舖">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入店舖代碼" clearable />
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
        <el-table-column prop="PlanId" label="盤點計劃單號" width="150" />
        <el-table-column prop="PlanDate" label="盤點日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.PlanDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="SakeType" label="盤點類型" width="120" />
        <el-table-column prop="PlanStatusName" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.PlanStatus)">
              {{ row.PlanStatusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ShopCount" label="店舖數量" width="100" />
        <el-table-column prop="TotalDiffQty" label="差異數量" width="120" />
        <el-table-column prop="TotalDiffAmount" label="差異金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalDiffAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.PlanStatus !== '0'">修改</el-button>
            <el-button type="info" size="small" @click="handleApprove(row)" :disabled="row.PlanStatus !== '0'">審核</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.PlanStatus !== '0'">刪除</el-button>
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
      width="70%"
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
            <el-form-item label="盤點日期" prop="PlanDate">
              <el-date-picker
                v-model="formData.PlanDate"
                type="date"
                placeholder="請選擇盤點日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="盤點類型" prop="SakeType">
              <el-input v-model="formData.SakeType" placeholder="請輸入盤點類型" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="開始日期" prop="StartDate">
              <el-date-picker
                v-model="formData.StartDate"
                type="date"
                placeholder="請選擇開始日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結束日期" prop="EndDate">
              <el-date-picker
                v-model="formData.EndDate"
                type="date"
                placeholder="請選擇結束日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="店舖">
          <el-select
            v-model="formData.ShopIds"
            multiple
            placeholder="請選擇店舖"
            style="width: 100%"
          >
            <el-option
              v-for="shop in shopOptions"
              :key="shop.ShopId"
              :label="shop.ShopName"
              :value="shop.ShopId"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 詳細資料對話框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="盤點計劃詳細資料"
      width="90%"
      :close-on-click-modal="false"
    >
      <el-tabs v-model="activeTab">
        <el-tab-pane label="基本資料" name="basic">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="盤點計劃單號">{{ currentRow?.PlanId }}</el-descriptions-item>
            <el-descriptions-item label="盤點日期">{{ formatDate(currentRow?.PlanDate) }}</el-descriptions-item>
            <el-descriptions-item label="盤點類型">{{ currentRow?.SakeType }}</el-descriptions-item>
            <el-descriptions-item label="狀態">{{ currentRow?.PlanStatusName }}</el-descriptions-item>
            <el-descriptions-item label="店舖數量">{{ currentRow?.ShopCount }}</el-descriptions-item>
            <el-descriptions-item label="差異數量">{{ currentRow?.TotalDiffQty }}</el-descriptions-item>
            <el-descriptions-item label="差異金額">{{ formatCurrency(currentRow?.TotalDiffAmount) }}</el-descriptions-item>
          </el-descriptions>
        </el-tab-pane>
        <el-tab-pane label="店舖列表" name="shops">
          <el-table :data="currentRow?.Shops" border>
            <el-table-column prop="ShopId" label="店舖代碼" />
            <el-table-column prop="StatusName" label="狀態" />
          </el-table>
        </el-tab-pane>
        <el-tab-pane label="盤點明細" name="details">
          <el-table :data="currentRow?.Details" border>
            <el-table-column prop="ShopId" label="店舖代碼" />
            <el-table-column prop="GoodsId" label="商品編號" />
            <el-table-column prop="BookQty" label="帳面數量" />
            <el-table-column prop="PhysicalQty" label="實盤數量" />
            <el-table-column prop="DiffQty" label="差異數量" />
            <el-table-column prop="DiffAmount" label="差異金額" />
          </el-table>
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <el-button @click="detailDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handleUpload" v-if="currentRow?.PlanStatus === '1'">上傳資料</el-button>
        <el-button type="success" @click="handleCalculate" v-if="currentRow?.PlanStatus === '1'">計算差異</el-button>
        <el-button type="warning" @click="handleConfirm" v-if="currentRow?.PlanStatus === '1'">確認結果</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { stocktakingPlanApi } from '@/api/inventoryCheck'
import * as shopApi from '@/api/modules/shop'

// 查詢表單
const queryForm = reactive({
  PlanId: '',
  PlanDateFrom: null,
  PlanDateTo: null,
  PlanStatus: '',
  ShopId: '',
  PageIndex: 1,
  PageSize: 20
})

const dateRange = ref(null)

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
const dialogTitle = ref('新增盤點計劃')
const formRef = ref(null)
const formData = reactive({
  PlanDate: '',
  StartDate: null,
  EndDate: null,
  StartTime: null,
  EndTime: null,
  SakeType: '',
  SakeDept: '',
  SiteId: '',
  ShopIds: []
})

const formRules = {
  PlanDate: [{ required: true, message: '請選擇盤點日期', trigger: 'change' }]
}

// 詳細資料對話框
const detailDialogVisible = ref(false)
const activeTab = ref('basic')
const currentRow = ref(null)

// 店舖列表
const shopOptions = ref([])

// 載入店舖列表
const loadShops = async () => {
  try {
    const response = await shopApi.getShops({ Status: '1' })
    if (response.Success && response.Data) {
      shopOptions.value = response.Data || []
    }
  } catch (error) {
    console.error('載入店舖列表失敗:', error)
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      ...queryForm,
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize
    }
    const response = await stocktakingPlanApi.getStocktakingPlans(params)
    if (response.Success && response.Data) {
      tableData.value = response.Data.Items || []
      pagination.TotalCount = response.Data.TotalCount || 0
    } else {
      ElMessage.error(response.Message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.PlanId = ''
  queryForm.PlanDateFrom = null
  queryForm.PlanDateTo = null
  queryForm.PlanStatus = ''
  queryForm.ShopId = ''
  dateRange.value = null
  handleSearch()
}

// 日期範圍變更
const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.PlanDateFrom = dates[0]
    queryForm.PlanDateTo = dates[1]
  } else {
    queryForm.PlanDateFrom = null
    queryForm.PlanDateTo = null
  }
}

// 新增
const handleCreate = () => {
  dialogTitle.value = '新增盤點計劃'
  Object.assign(formData, {
    PlanDate: '',
    StartDate: null,
    EndDate: null,
    StartTime: null,
    EndTime: null,
    SakeType: '',
    SakeDept: '',
    SiteId: '',
    ShopIds: []
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  dialogTitle.value = '修改盤點計劃'
  currentRow.value = row
  try {
    const response = await stocktakingPlanApi.getStocktakingPlan(row.PlanId)
    if (response.Success && response.Data) {
      const data = response.Data
      Object.assign(formData, {
        PlanDate: data.PlanDate,
        StartDate: data.StartDate,
        EndDate: data.EndDate,
        StartTime: data.StartTime,
        EndTime: data.EndTime,
        SakeType: data.SakeType,
        SakeDept: data.SakeDept,
        SiteId: data.SiteId,
        ShopIds: data.Shops ? data.Shops.map(s => s.ShopId) : []
      })
      dialogVisible.value = true
    } else {
      ElMessage.error(response.Message || '取得資料失敗')
    }
  } catch (error) {
    ElMessage.error('取得資料失敗：' + error.message)
  }
}

// 查看
const handleView = async (row) => {
  try {
    const response = await stocktakingPlanApi.getStocktakingPlan(row.PlanId)
    if (response.Success && response.Data) {
      currentRow.value = response.Data
      detailDialogVisible.value = true
    } else {
      ElMessage.error(response.Message || '取得資料失敗')
    }
  } catch (error) {
    ElMessage.error('取得資料失敗：' + error.message)
  }
}

// 審核
const handleApprove = async (row) => {
  try {
    await ElMessageBox.confirm('確定要審核此盤點計劃嗎？', '確認', {
      type: 'warning'
    })
    const response = await stocktakingPlanApi.approveStocktakingPlan(row.PlanId)
    if (response.Success) {
      ElMessage.success('審核成功')
      handleSearch()
    } else {
      ElMessage.error(response.Message || '審核失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('審核失敗：' + error.message)
    }
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此盤點計劃嗎？', '確認', {
      type: 'warning'
    })
    const response = await stocktakingPlanApi.deleteStocktakingPlan(row.PlanId)
    if (response.Success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 提交表單
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const isEdit = dialogTitle.value === '修改盤點計劃'
        const api = isEdit
          ? stocktakingPlanApi.updateStocktakingPlan(currentRow.value?.PlanId, formData)
          : stocktakingPlanApi.createStocktakingPlan(formData)

        const response = await api
        if (response.Success) {
          ElMessage.success(isEdit ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.Message || (isEdit ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
}

// 上傳資料
const handleUpload = () => {
  ElMessage.info('上傳功能開發中')
}

// 計算差異
const handleCalculate = async () => {
  try {
    await ElMessageBox.confirm('確定要計算盤點差異嗎？', '確認', {
      type: 'warning'
    })
    const response = await stocktakingPlanApi.calculateStocktakingDiff(currentRow.value.PlanId)
    if (response.Success) {
      ElMessage.success('計算成功')
      handleView(currentRow.value)
    } else {
      ElMessage.error(response.Message || '計算失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('計算失敗：' + error.message)
    }
  }
}

// 確認結果
const handleConfirm = async () => {
  try {
    await ElMessageBox.confirm('確定要確認盤點結果嗎？確認後將產生庫存調整單。', '確認', {
      type: 'warning'
    })
    const response = await stocktakingPlanApi.confirmStocktakingResult(currentRow.value.PlanId)
    if (response.Success) {
      ElMessage.success('確認成功，已產生庫存調整單：' + (response.Data || ''))
      handleSearch()
      detailDialogVisible.value = false
    } else {
      ElMessage.error(response.Message || '確認失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('確認失敗：' + error.message)
    }
  }
}

// 分頁變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// 格式化
const formatDate = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString('zh-TW')
}

const formatCurrency = (amount) => {
  if (amount == null) return '0'
  return new Intl.NumberFormat('zh-TW', {
    style: 'currency',
    currency: 'TWD',
    minimumFractionDigits: 0
  }).format(amount)
}

const getStatusType = (status) => {
  const map = {
    '-1': 'info',
    '0': 'warning',
    '1': 'success',
    '4': 'danger',
    '5': ''
  }
  return map[status] || ''
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style scoped lang="scss">
.stocktaking-plan {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;

    h1 {
      font-size: 24px;
      font-weight: 500;
      color: #303133;
    }
  }

  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>


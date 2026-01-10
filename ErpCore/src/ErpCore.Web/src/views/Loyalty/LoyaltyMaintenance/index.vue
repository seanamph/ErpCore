<template>
  <div class="loyalty-maintenance">
    <div class="page-header">
      <h1>忠誠度系統維護 (LPS)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="交易編號">
          <el-input v-model="queryForm.RRN" placeholder="請輸入交易編號" clearable />
        </el-form-item>
        <el-form-item label="會員卡號">
          <el-input v-model="queryForm.CardNo" placeholder="請輸入會員卡號" clearable />
        </el-form-item>
        <el-form-item label="交易類型">
          <el-select v-model="queryForm.TransType" placeholder="請選擇交易類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="累積" value="2" />
            <el-option label="扣減" value="3" />
            <el-option label="取消" value="4" />
            <el-option label="調整" value="11" />
          </el-select>
        </el-form-item>
        <el-form-item label="交易狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇交易狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="成功" value="SUCCESS" />
            <el-option label="失敗" value="FAILED" />
          </el-select>
        </el-form-item>
        <el-form-item label="交易日期">
          <el-date-picker
            v-model="queryForm.TransDateRange"
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
          <el-button type="success" @click="handleCreate">新增交易</el-button>
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
        <el-table-column prop="RRN" label="交易編號" width="150" />
        <el-table-column prop="CardNo" label="會員卡號" width="150" />
        <el-table-column prop="TraceNo" label="追蹤編號" width="120" />
        <el-table-column prop="TransType" label="交易類型" width="100">
          <template #default="{ row }">
            <el-tag :type="getTransTypeTag(row.TransType)">
              {{ getTransTypeText(row.TransType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="AwardPoints" label="累積點數" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.AwardPoints) }}
          </template>
        </el-table-column>
        <el-table-column prop="RedeemPoints" label="扣減點數" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.RedeemPoints) }}
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="交易金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Invoice" label="發票號碼" width="150" />
        <el-table-column prop="TransTime" label="交易時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.TransTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'SUCCESS' ? 'success' : 'danger'">
              {{ row.Status === 'SUCCESS' ? '成功' : '失敗' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="VoidFlag" label="作廢" width="80">
          <template #default="{ row }">
            <el-tag v-if="row.VoidFlag === 'Y'" type="warning">是</el-tag>
            <span v-else>否</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button 
              v-if="row.VoidFlag !== 'Y' && row.Status === 'SUCCESS'"
              type="warning" 
              size="small" 
              @click="handleVoid(row)"
            >
              取消
            </el-button>
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
      v-model="dialogVisible"
      :title="dialogTitle"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-form-item label="會員卡號" prop="CardNo">
          <el-input v-model="formData.CardNo" placeholder="請輸入會員卡號" />
        </el-form-item>
        <el-form-item label="追蹤編號">
          <el-input v-model="formData.TraceNo" placeholder="請輸入追蹤編號" />
        </el-form-item>
        <el-form-item label="到期日">
          <el-date-picker
            v-model="formData.ExpDate"
            type="date"
            placeholder="請選擇到期日"
            value-format="YYYYMMDD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="交易類型" prop="TransType">
          <el-select v-model="formData.TransType" placeholder="請選擇交易類型" style="width: 100%">
            <el-option label="累積" value="2" />
            <el-option label="扣減" value="3" />
            <el-option label="取消" value="4" />
            <el-option label="調整" value="11" />
          </el-select>
        </el-form-item>
        <el-form-item label="累積點數">
          <el-input-number v-model="formData.AwardPoints" :min="0" :precision="4" style="width: 100%" />
        </el-form-item>
        <el-form-item label="扣減點數">
          <el-input-number v-model="formData.RedeemPoints" :min="0" :precision="4" style="width: 100%" />
        </el-form-item>
        <el-form-item label="交易金額">
          <el-input-number v-model="formData.Amount" :min="0" :precision="2" style="width: 100%" />
        </el-form-item>
        <el-form-item label="發票號碼">
          <el-input v-model="formData.Invoice" placeholder="請輸入發票號碼" />
        </el-form-item>
        <el-form-item label="授權碼">
          <el-input v-model="formData.AuthCode" placeholder="請輸入授權碼" />
        </el-form-item>
        <el-form-item label="強制日期">
          <el-date-picker
            v-model="formData.ForceDate"
            type="datetime"
            placeholder="請選擇強制日期"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 查看對話框 -->
    <el-dialog
      v-model="viewDialogVisible"
      title="查看交易詳細資料"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-descriptions :column="2" border>
        <el-descriptions-item label="交易編號">{{ viewData.RRN }}</el-descriptions-item>
        <el-descriptions-item label="會員卡號">{{ viewData.CardNo }}</el-descriptions-item>
        <el-descriptions-item label="追蹤編號">{{ viewData.TraceNo }}</el-descriptions-item>
        <el-descriptions-item label="到期日">{{ viewData.ExpDate }}</el-descriptions-item>
        <el-descriptions-item label="交易類型">{{ getTransTypeText(viewData.TransType) }}</el-descriptions-item>
        <el-descriptions-item label="交易狀態">
          <el-tag :type="viewData.Status === 'SUCCESS' ? 'success' : 'danger'">
            {{ viewData.Status === 'SUCCESS' ? '成功' : '失敗' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="累積點數">{{ formatNumber(viewData.AwardPoints) }}</el-descriptions-item>
        <el-descriptions-item label="扣減點數">{{ formatNumber(viewData.RedeemPoints) }}</el-descriptions-item>
        <el-descriptions-item label="交易金額">{{ formatCurrency(viewData.Amount) }}</el-descriptions-item>
        <el-descriptions-item label="發票號碼">{{ viewData.Invoice }}</el-descriptions-item>
        <el-descriptions-item label="授權碼">{{ viewData.AuthCode }}</el-descriptions-item>
        <el-descriptions-item label="交易時間">{{ formatDateTime(viewData.TransTime) }}</el-descriptions-item>
        <el-descriptions-item label="作廢標記">
          <el-tag v-if="viewData.VoidFlag === 'Y'" type="warning">是</el-tag>
          <span v-else>否</span>
        </el-descriptions-item>
        <el-descriptions-item label="取消標記">
          <el-tag v-if="viewData.ReversalFlag === 'Y'" type="warning">是</el-tag>
          <span v-else>否</span>
        </el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="viewDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

// API
const loyaltyApi = {
  getTransactions: (params) => axios.get('/api/v1/loyalty-point-transactions', { params }),
  getTransaction: (rrn) => axios.get(`/api/v1/loyalty-point-transactions/${rrn}`),
  createTransaction: (data) => axios.post('/api/v1/loyalty-point-transactions', data),
  voidTransaction: (rrn, data) => axios.post(`/api/v1/loyalty-point-transactions/${rrn}/void`, data)
}

// 查詢表單
const queryForm = reactive({
  RRN: '',
  CardNo: '',
  TransType: '',
  Status: '',
  TransDateRange: null
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
const dialogTitle = computed(() => '新增點數交易')
const formRef = ref(null)
const formData = reactive({
  CardNo: '',
  TraceNo: '',
  ExpDate: null,
  TransType: '2',
  AwardPoints: 0,
  RedeemPoints: 0,
  Amount: 0,
  Invoice: '',
  AuthCode: '',
  ForceDate: null
})
const formRules = {
  CardNo: [{ required: true, message: '請輸入會員卡號', trigger: 'blur' }],
  TransType: [{ required: true, message: '請選擇交易類型', trigger: 'change' }]
}

// 查看對話框
const viewDialogVisible = ref(false)
const viewData = ref({})

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      RRN: queryForm.RRN || undefined,
      CardNo: queryForm.CardNo || undefined,
      TransType: queryForm.TransType || undefined,
      Status: queryForm.Status || undefined
    }
    if (queryForm.TransDateRange && queryForm.TransDateRange.length === 2) {
      params.StartDate = queryForm.TransDateRange[0]
      params.EndDate = queryForm.TransDateRange[1]
    }
    const response = await loyaltyApi.getTransactions(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.RRN = ''
  queryForm.CardNo = ''
  queryForm.TransType = ''
  queryForm.Status = ''
  queryForm.TransDateRange = null
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  Object.assign(formData, {
    CardNo: '',
    TraceNo: '',
    ExpDate: null,
    TransType: '2',
    AwardPoints: 0,
    RedeemPoints: 0,
    Amount: 0,
    Invoice: '',
    AuthCode: '',
    ForceDate: null
  })
  dialogVisible.value = true
}

// 查看
const handleView = async (row) => {
  try {
    const response = await loyaltyApi.getTransaction(row.RRN)
    if (response.data?.success) {
      viewData.value = response.data.data
      viewDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 取消交易
const handleVoid = async (row) => {
  try {
    await ElMessageBox.confirm('確定要取消此交易嗎？', '確認', {
      type: 'warning'
    })
    const response = await loyaltyApi.voidTransaction(row.RRN, {
      ReversalFlag: 'Y',
      VoidFlag: 'Y'
    })
    if (response.data?.success) {
      ElMessage.success('取消成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '取消失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('取消失敗：' + error.message)
    }
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await loyaltyApi.createTransaction(formData)
        if (response.data?.success) {
          ElMessage.success('新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
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

// 格式化數字
const formatNumber = (num) => {
  if (!num) return '0'
  return Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 4, maximumFractionDigits: 4 })
}

// 格式化貨幣
const formatCurrency = (num) => {
  if (!num) return '$0'
  return '$' + Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleString('zh-TW')
}

// 取得交易類型標籤
const getTransTypeTag = (type) => {
  const tags = {
    '2': 'success',
    '3': 'warning',
    '4': 'danger',
    '11': 'info'
  }
  return tags[type] || 'info'
}

// 取得交易類型文字
const getTransTypeText = (type) => {
  const texts = {
    '2': '累積',
    '3': '扣減',
    '4': '取消',
    '11': '調整'
  }
  return texts[type] || type
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.loyalty-maintenance {
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
    margin-bottom: 20px;
  }
}
</style>


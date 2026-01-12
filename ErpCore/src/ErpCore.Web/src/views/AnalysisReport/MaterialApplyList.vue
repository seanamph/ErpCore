<template>
  <div class="material-apply-list">
    <div class="page-header">
      <h1>單位領用申請作業 (SYSA210)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="領用單號">
          <el-input v-model="queryForm.filters.applyId" placeholder="請輸入領用單號" clearable />
        </el-form-item>
        <el-form-item label="申請人">
          <el-input v-model="queryForm.filters.empId" placeholder="請輸入申請人代號" clearable />
        </el-form-item>
        <el-form-item label="部門">
          <el-input v-model="queryForm.filters.orgId" placeholder="請輸入部門代號" clearable />
        </el-form-item>
        <el-form-item label="分店">
          <el-select v-model="queryForm.filters.siteId" placeholder="請選擇分店" clearable style="width: 200px">
            <el-option
              v-for="site in siteList"
              :key="site.value"
              :label="site.label"
              :value="site.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="申請日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="領用單狀態">
          <el-select v-model="queryForm.filters.applyStatus" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="待審核" value="0" />
            <el-option label="已審核" value="1" />
            <el-option label="已發料" value="2" />
            <el-option label="已取消" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleAdd">新增</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="applyId" label="領用單號" width="150" />
        <el-table-column prop="empName" label="申請人" width="120" />
        <el-table-column prop="orgName" label="部門" width="150" />
        <el-table-column prop="siteName" label="分店" width="120" />
        <el-table-column prop="applyDate" label="申請日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.applyDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="applyStatusName" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.applyStatus)">
              {{ row.applyStatusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="amount" label="總價值" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="aprvEmpName" label="審核者" width="120" />
        <el-table-column prop="aprvDate" label="審核日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.aprvDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="checkDate" label="發料日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.checkDate) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" v-if="row.applyStatus === '0'">修改</el-button>
            <el-button type="success" size="small" @click="handleApprove(row)" v-if="row.applyStatus === '0'">審核</el-button>
            <el-button type="info" size="small" @click="handleIssue(row)" v-if="row.applyStatus === '1'">發料</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" v-if="row.applyStatus === '0'">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.pageIndex"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.totalCount"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 新增/修改對話框 -->
    <MaterialApplyDialog
      v-model="dialogVisible"
      :apply-data="currentApply"
      :is-edit="isEdit"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { materialApplyApi } from '@/api/materialApply'
import { dropdownListApi } from '@/api/dropdownList'
import MaterialApplyDialog from './MaterialApplyDialog.vue'

const loading = ref(false)
const tableData = ref([])
const dateRange = ref([])
const dialogVisible = ref(false)
const currentApply = ref(null)
const isEdit = ref(false)
const siteList = ref([])

const queryForm = reactive({
  pageIndex: 1,
  pageSize: 20,
  sortField: 'ApplyDate',
  sortOrder: 'DESC',
  filters: {
    applyId: '',
    empId: '',
    orgId: '',
    siteId: '',
    applyDateFrom: null,
    applyDateTo: null,
    applyStatus: ''
  }
})

const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    queryForm.filters.applyDateFrom = dates[0]
    queryForm.filters.applyDateTo = dates[1]
  } else {
    queryForm.filters.applyDateFrom = null
    queryForm.filters.applyDateTo = null
  }
}

const handleSearch = async () => {
  try {
    loading.value = true
    const params = {
      ...queryForm,
      filters: {
        ...queryForm.filters
      }
    }
    const response = await materialApplyApi.getMaterialApplies(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.totalCount = response.data.data.totalCount
      pagination.pageIndex = response.data.data.pageIndex
      pagination.pageSize = response.data.data.pageSize
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryForm.filters.applyId = ''
  queryForm.filters.empId = ''
  queryForm.filters.orgId = ''
  queryForm.filters.siteId = ''
  queryForm.filters.applyDateFrom = null
  queryForm.filters.applyDateTo = null
  queryForm.filters.applyStatus = ''
  dateRange.value = []
  handleSearch()
}

const handleAdd = () => {
  currentApply.value = null
  isEdit.value = false
  dialogVisible.value = true
}

const handleView = async (row) => {
  try {
    const response = await materialApplyApi.getMaterialApplyDetail(row.applyId)
    if (response.data.success) {
      currentApply.value = response.data.data
      isEdit.value = false
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  }
}

const handleEdit = async (row) => {
  try {
    const response = await materialApplyApi.getMaterialApplyDetail(row.applyId)
    if (response.data.success) {
      currentApply.value = response.data.data
      isEdit.value = true
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  }
}

const handleApprove = async (row) => {
  try {
    await ElMessageBox.confirm('確定要審核此領用申請單嗎？', '確認審核', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    // 獲取當前使用者資訊（這裡需要從 store 或 localStorage 獲取）
    const currentUser = JSON.parse(localStorage.getItem('user') || '{}')
    const aprvEmpId = currentUser.userId || currentUser.empId || ''
    
    const response = await materialApplyApi.approveMaterialApply(row.applyId, {
      aprvEmpId: aprvEmpId,
      aprvDate: new Date().toISOString().split('T')[0],
      notes: ''
    })
    if (response.data.success) {
      ElMessage.success('審核成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '審核失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('審核失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

const handleIssue = async (row) => {
  try {
    await ElMessageBox.confirm('確定要執行發料作業嗎？', '確認發料', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    // 先獲取詳細資料以取得明細
    const detailResponse = await materialApplyApi.getMaterialApplyDetail(row.applyId)
    if (!detailResponse.data.success) {
      ElMessage.error('獲取領用申請單詳細資料失敗')
      return
    }
    
    const details = detailResponse.data.data.details || []
    const issueDetails = details.map(d => ({
      tKey: d.tKey,
      issueQty: d.applyQty // 預設發料數量等於申請數量
    }))
    
    const response = await materialApplyApi.issueMaterialApply(row.applyId, {
      checkDate: new Date().toISOString(),
      details: issueDetails
    })
    if (response.data.success) {
      ElMessage.success('發料成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '發料失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('發料失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此領用申請單嗎？', '確認刪除', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    const response = await materialApplyApi.deleteMaterialApply(row.applyId)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

const handleSizeChange = (size) => {
  queryForm.pageSize = size
  queryForm.pageIndex = 1
  handleSearch()
}

const handlePageChange = (page) => {
  queryForm.pageIndex = page
  handleSearch()
}

const handleDialogSuccess = () => {
  dialogVisible.value = false
  handleSearch()
}

const getStatusType = (status) => {
  const types = {
    '0': 'warning',
    '1': 'success',
    '2': 'info',
    '3': 'danger'
  }
  return types[status] || ''
}

const formatDate = (date) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString('zh-TW')
}

const formatCurrency = (amount) => {
  if (amount == null) return '0.00'
  return new Intl.NumberFormat('zh-TW', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const loadSiteList = async () => {
  try {
    const response = await dropdownListApi.getShopOptions({})
    if (response.data && response.data.success && response.data.data) {
      siteList.value = response.data.data.map(item => ({
        value: item.value || item.ShopId || item.siteId,
        label: item.label || item.ShopName || item.siteName
      }))
    }
  } catch (error) {
    console.error('載入分店列表失敗:', error)
  }
}

onMounted(() => {
  loadSiteList()
  handleSearch()
})
</script>

<style scoped>
.material-apply-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: bold;
  color: #303133;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin-top: 10px;
}

.table-card {
  margin-bottom: 20px;
}

.pagination-container {
  margin-top: 20px;
  text-align: right;
}
</style>

<template>
  <div class="member-query">
    <div class="page-header">
      <h1>會員查詢作業 (SYS3330-SYS33B0)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" class="search-form" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="會員編號">
              <el-input v-model="queryForm.MemberId" placeholder="請輸入會員編號" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="會員姓名">
              <el-input v-model="queryForm.MemberName" placeholder="請輸入會員姓名" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="身分證字號">
              <el-input v-model="queryForm.PersonalId" placeholder="請輸入身分證字號" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="電話">
              <el-input v-model="queryForm.Phone" placeholder="請輸入電話" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="手機">
              <el-input v-model="queryForm.Mobile" placeholder="請輸入手機" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="電子郵件">
              <el-input v-model="queryForm.Email" placeholder="請輸入電子郵件" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="會員等級">
              <el-input v-model="queryForm.MemberLevel" placeholder="請輸入會員等級" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="狀態">
              <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
                <el-option label="全部" value="" />
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
                <el-option label="暫停" value="S" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="卡號">
              <el-input v-model="queryForm.CardNo" placeholder="請輸入卡號" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="加入日期起">
              <el-date-picker
                v-model="queryForm.JoinDateFrom"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="加入日期迄">
              <el-date-picker
                v-model="queryForm.JoinDateTo"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
          <el-button type="warning" @click="handlePrint">列印</el-button>
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
        <el-table-column prop="MemberId" label="會員編號" width="150" />
        <el-table-column prop="MemberName" label="會員姓名" width="150" />
        <el-table-column prop="PersonalId" label="身分證字號" width="150" />
        <el-table-column prop="Phone" label="電話" width="150" />
        <el-table-column prop="Mobile" label="手機" width="150" />
        <el-table-column prop="Email" label="電子郵件" width="200" />
        <el-table-column prop="MemberLevel" label="會員等級" width="120" />
        <el-table-column prop="CardNo" label="卡號" width="150" />
        <el-table-column prop="JoinDate" label="加入日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.JoinDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : row.Status === 'S' ? 'warning' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : row.Status === 'S' ? '暫停' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewTransactions(row)">交易記錄</el-button>
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

    <!-- 交易記錄對話框 -->
    <el-dialog
      v-model="transactionsDialogVisible"
      title="會員交易記錄"
      width="900px"
    >
      <el-table
        :data="transactionsData"
        v-loading="transactionsLoading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="TransactionDate" label="交易日期" width="150">
          <template #default="{ row }">
            {{ formatDateTime(row.TransactionDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransactionType" label="交易類型" width="120" />
        <el-table-column prop="Amount" label="金額" width="120" />
        <el-table-column prop="Points" label="積分" width="100" />
        <el-table-column prop="Remarks" label="備註" />
      </el-table>
      <template #footer>
        <el-button @click="transactionsDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { memberQueryApi } from '@/api/storeMember'

// 查詢表單
const queryForm = reactive({
  MemberId: '',
  MemberName: '',
  PersonalId: '',
  Phone: '',
  Mobile: '',
  Email: '',
  MemberLevel: '',
  Status: '',
  CardNo: '',
  JoinDateFrom: '',
  JoinDateTo: ''
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

// 交易記錄
const transactionsDialogVisible = ref(false)
const transactionsData = ref([])
const transactionsLoading = ref(false)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const response = await memberQueryApi.queryMembers({
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ...queryForm
    })
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
  Object.assign(queryForm, {
    MemberId: '',
    MemberName: '',
    PersonalId: '',
    Phone: '',
    Mobile: '',
    Email: '',
    MemberLevel: '',
    Status: '',
    CardNo: '',
    JoinDateFrom: '',
    JoinDateTo: ''
  })
  pagination.PageIndex = 1
  handleSearch()
}

// 查看交易記錄
const handleViewTransactions = async (row) => {
  transactionsDialogVisible.value = true
  transactionsLoading.value = true
  try {
    const response = await memberQueryApi.getMemberTransactions(row.MemberId, {})
    if (response.data?.success) {
      transactionsData.value = response.data.data?.items || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    transactionsLoading.value = false
  }
}

// 匯出
const handleExport = async () => {
  try {
    const response = await memberQueryApi.exportMembers({
      ...queryForm
    })
    const blob = new Blob([response.data])
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `會員查詢結果_${new Date().getTime()}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 列印
const handlePrint = async () => {
  try {
    const response = await memberQueryApi.printMemberReport({
      ...queryForm
    })
    const blob = new Blob([response.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    window.open(url, '_blank')
    ElMessage.success('列印成功')
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
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

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.member-query {
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


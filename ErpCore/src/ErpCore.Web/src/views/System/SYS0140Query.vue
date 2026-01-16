<template>
  <div class="sys0140-query">
    <div class="page-header">
      <h1>使用者資料查詢 (SYS0140)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者編號">
          <el-input v-model="queryForm.UserId" placeholder="請輸入使用者編號" clearable />
        </el-form-item>
        <el-form-item label="使用者名稱">
          <el-input v-model="queryForm.UserName" placeholder="請輸入使用者名稱" clearable />
        </el-form-item>
        <el-form-item label="組織">
          <el-input v-model="queryForm.OrgId" placeholder="請輸入組織代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="鎖定" value="L" />
          </el-select>
        </el-form-item>
        <el-form-item label="使用者型態">
          <el-select v-model="queryForm.UserType" placeholder="請選擇使用者型態" clearable style="width: 150px">
            <el-option label="內部" value="INTERNAL" />
            <el-option label="外部" value="EXTERNAL" />
          </el-select>
        </el-form-item>
        <el-form-item label="職稱">
          <el-input v-model="queryForm.Title" placeholder="請輸入職稱" clearable />
        </el-form-item>
        <el-form-item label="有效起始日（起）">
          <el-date-picker
            v-model="queryForm.StartDateFrom"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="有效起始日（迄）">
          <el-date-picker
            v-model="queryForm.StartDateTo"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="有效終止日（起）">
          <el-date-picker
            v-model="queryForm.EndDateFrom"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="有效終止日（迄）">
          <el-date-picker
            v-model="queryForm.EndDateTo"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="最後登入（起）">
          <el-date-picker
            v-model="queryForm.LastLoginDateFrom"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="最後登入（迄）">
          <el-date-picker
            v-model="queryForm.LastLoginDateTo"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
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
        @sort-change="handleSortChange"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="UserId" label="使用者編號" width="120" sortable="custom" />
        <el-table-column prop="UserName" label="使用者名稱" width="150" sortable="custom" />
        <el-table-column prop="Title" label="職稱" width="100" sortable="custom" />
        <el-table-column prop="OrgId" label="組織代碼" width="120" sortable="custom" />
        <el-table-column prop="Status" label="狀態" width="80" sortable="custom">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="UserType" label="使用者型態" width="100" sortable="custom" />
        <el-table-column prop="StartDate" label="有效起始日" width="120" sortable="custom">
          <template #default="{ row }">
            {{ row.StartDate ? formatDate(row.StartDate) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="EndDate" label="有效終止日" width="120" sortable="custom">
          <template #default="{ row }">
            {{ row.EndDate ? formatDate(row.EndDate) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="LastLoginDate" label="最後登入" width="160" sortable="custom">
          <template #default="{ row }">
            {{ row.LastLoginDate ? formatDateTime(row.LastLoginDate) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="LoginCount" label="登入次數" width="100" align="center" sortable="custom" />
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
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

    <!-- 詳細資料對話框（只讀模式） -->
    <el-dialog
      title="使用者詳細資料"
      v-model="detailDialogVisible"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-descriptions :column="2" border v-if="detailData">
        <el-descriptions-item label="使用者編號">{{ detailData.UserId }}</el-descriptions-item>
        <el-descriptions-item label="使用者名稱">{{ detailData.UserName }}</el-descriptions-item>
        <el-descriptions-item label="職稱">{{ detailData.Title || '-' }}</el-descriptions-item>
        <el-descriptions-item label="組織代碼">{{ detailData.OrgId || '-' }}</el-descriptions-item>
        <el-descriptions-item label="有效起始日">{{ detailData.StartDate ? formatDate(detailData.StartDate) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="有效終止日">{{ detailData.EndDate ? formatDate(detailData.EndDate) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="getStatusType(detailData.Status)">{{ getStatusText(detailData.Status) }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="使用者型態">{{ detailData.UserType || '-' }}</el-descriptions-item>
        <el-descriptions-item label="使用者等級">{{ detailData.UserPriority || 0 }}</el-descriptions-item>
        <el-descriptions-item label="登入次數">{{ detailData.LoginCount || 0 }}</el-descriptions-item>
        <el-descriptions-item label="最後登入時間">{{ detailData.LastLoginDate ? formatDateTime(detailData.LastLoginDate) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="最後登入IP">{{ detailData.LastLoginIp || '-' }}</el-descriptions-item>
        <el-descriptions-item label="密碼變更日期">{{ detailData.ChangePwdDate ? formatDateTime(detailData.ChangePwdDate) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="所屬分店">{{ detailData.ShopId || '-' }}</el-descriptions-item>
        <el-descriptions-item label="樓層代碼">{{ detailData.FloorId || '-' }}</el-descriptions-item>
        <el-descriptions-item label="區域別代碼">{{ detailData.AreaId || '-' }}</el-descriptions-item>
        <el-descriptions-item label="業種代碼">{{ detailData.BtypeId || '-' }}</el-descriptions-item>
        <el-descriptions-item label="專櫃代碼">{{ detailData.StoreId || '-' }}</el-descriptions-item>
        <el-descriptions-item label="備註" :span="2">{{ detailData.Notes || '-' }}</el-descriptions-item>
        <el-descriptions-item label="建立者">{{ detailData.CreatedBy || '-' }}</el-descriptions-item>
        <el-descriptions-item label="建立時間">{{ detailData.CreatedAt ? formatDateTime(detailData.CreatedAt) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="更新者">{{ detailData.UpdatedBy || '-' }}</el-descriptions-item>
        <el-descriptions-item label="更新時間">{{ detailData.UpdatedAt ? formatDateTime(detailData.UpdatedAt) : '-' }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="handleDetailDialogClose">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { queryUsersGet, queryUserById, exportUsers } from '@/api/users'

// 查詢表單
const queryForm = reactive({
  UserId: '',
  UserName: '',
  OrgId: '',
  Status: '',
  UserType: '',
  Title: '',
  StartDateFrom: '',
  StartDateTo: '',
  EndDateFrom: '',
  EndDateTo: '',
  LastLoginDateFrom: '',
  LastLoginDateTo: ''
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

// 排序
const sortField = ref('UserId')
const sortOrder = ref('ASC')

// 詳細資料對話框
const detailDialogVisible = ref(false)
const detailData = ref(null)

// 取得狀態類型
const getStatusType = (status) => {
  switch (status) {
    case 'A':
      return 'success'
    case 'I':
      return 'danger'
    case 'L':
      return 'warning'
    default:
      return 'info'
  }
}

// 取得狀態文字
const getStatusText = (status) => {
  switch (status) {
    case 'A':
      return '啟用'
    case 'I':
      return '停用'
    case 'L':
      return '鎖定'
    default:
      return status
  }
}

// 格式化日期
const formatDate = (date) => {
  if (!date) return '-'
  const d = new Date(date)
  return d.toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return '-'
  const date = new Date(dateTime)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: sortField.value,
      SortOrder: sortOrder.value,
      UserId: queryForm.UserId || undefined,
      UserName: queryForm.UserName || undefined,
      OrgId: queryForm.OrgId || undefined,
      Status: queryForm.Status || undefined,
      UserType: queryForm.UserType || undefined,
      Title: queryForm.Title || undefined,
      StartDateFrom: queryForm.StartDateFrom || undefined,
      StartDateTo: queryForm.StartDateTo || undefined,
      EndDateFrom: queryForm.EndDateFrom || undefined,
      EndDateTo: queryForm.EndDateTo || undefined,
      LastLoginDateFrom: queryForm.LastLoginDateFrom || undefined,
      LastLoginDateTo: queryForm.LastLoginDateTo || undefined
    }
    const response = await queryUsersGet(params)
    if (response && response.Data) {
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
    UserId: '',
    UserName: '',
    OrgId: '',
    Status: '',
    UserType: '',
    Title: '',
    StartDateFrom: '',
    StartDateTo: '',
    EndDateFrom: '',
    EndDateTo: '',
    LastLoginDateFrom: '',
    LastLoginDateTo: ''
  })
  sortField.value = 'UserId'
  sortOrder.value = 'ASC'
  handleSearch()
}

// 排序變更
const handleSortChange = ({ prop, order }) => {
  if (prop) {
    sortField.value = prop
    sortOrder.value = order === 'ascending' ? 'ASC' : 'DESC'
    loadData()
  }
}

// 查看詳細資料
const handleView = async (row) => {
  try {
    loading.value = true
    const response = await queryUserById(row.UserId)
    if (response && response.Data) {
      detailData.value = response.Data
      detailDialogVisible.value = true
    }
  } catch (error) {
    ElMessage.error('載入詳細資料失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 關閉詳細資料對話框
const handleDetailDialogClose = () => {
  detailDialogVisible.value = false
  detailData.value = null
}

// 匯出
const handleExport = async () => {
  try {
    loading.value = true
    const data = {
      PageIndex: 1,
      PageSize: 999999,
      SortField: sortField.value,
      SortOrder: sortOrder.value,
      UserId: queryForm.UserId || undefined,
      UserName: queryForm.UserName || undefined,
      OrgId: queryForm.OrgId || undefined,
      Status: queryForm.Status || undefined,
      UserType: queryForm.UserType || undefined,
      Title: queryForm.Title || undefined,
      StartDateFrom: queryForm.StartDateFrom || undefined,
      StartDateTo: queryForm.StartDateTo || undefined,
      EndDateFrom: queryForm.EndDateFrom || undefined,
      EndDateTo: queryForm.EndDateTo || undefined,
      LastLoginDateFrom: queryForm.LastLoginDateFrom || undefined,
      LastLoginDateTo: queryForm.LastLoginDateTo || undefined
    }
    const response = await exportUsers(data)
    
    // 創建下載連結
    const blob = new Blob([response], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `使用者查詢結果_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
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
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0140-query {
  .page-header {
    h1 {
      color: $primary-color;
    }
  }
  
  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>

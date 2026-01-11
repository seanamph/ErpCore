<template>
  <div class="sys0117-query">
    <div class="page-header">
      <h1>使用者權限代理 (SYS0117)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="委託人">
          <el-input v-model="queryForm.PrincipalUserId" placeholder="請輸入委託人編號" clearable />
        </el-form-item>
        <el-form-item label="代理人">
          <el-input v-model="queryForm.AgentUserId" placeholder="請輸入代理人編號" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="開始時間">
          <el-date-picker
            v-model="queryForm.BeginTimeFrom"
            type="datetime"
            placeholder="開始時間起"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 180px"
          />
        </el-form-item>
        <el-form-item label="~">
          <el-date-picker
            v-model="queryForm.BeginTimeTo"
            type="datetime"
            placeholder="開始時間迄"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 180px"
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="PrincipalUserName" label="委託人" width="150" sortable>
          <template #default="{ row }">
            {{ row.PrincipalUserName || row.PrincipalUserId }}
          </template>
        </el-table-column>
        <el-table-column prop="AgentUserName" label="代理人" width="150" sortable>
          <template #default="{ row }">
            {{ row.AgentUserName || row.AgentUserId }}
          </template>
        </el-table-column>
        <el-table-column prop="BeginTime" label="開始時間" width="180" sortable>
          <template #default="{ row }">
            {{ formatDateTime(row.BeginTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="EndTime" label="結束時間" width="180" sortable>
          <template #default="{ row }">
            {{ formatDateTime(row.EndTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'info'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
            <el-button
              :type="row.Status === 'A' ? 'warning' : 'success'"
              size="small"
              @click="handleToggleStatus(row)"
            >
              {{ row.Status === 'A' ? '停用' : '啟用' }}
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

      <!-- 批次操作按鈕 -->
      <div style="margin-top: 10px">
        <el-button type="danger" @click="handleBatchDelete" :disabled="selectedRows.length === 0">
          批次刪除
        </el-button>
      </div>
    </el-card>

    <!-- 新增/修改對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="委託人" prop="PrincipalUserId">
          <el-select
            v-model="form.PrincipalUserId"
            placeholder="請選擇委託人"
            filterable
            style="width: 100%"
            @change="handlePrincipalUserChange"
          >
            <el-option
              v-for="user in userList"
              :key="user.UserId"
              :label="`${user.UserName} (${user.UserId})`"
              :value="user.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="代理人" prop="AgentUserId">
          <el-select
            v-model="form.AgentUserId"
            placeholder="請選擇代理人"
            filterable
            style="width: 100%"
            @change="handleAgentUserChange"
          >
            <el-option
              v-for="user in userList"
              :key="user.UserId"
              :label="`${user.UserName} (${user.UserId})`"
              :value="user.UserId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="開始時間" prop="BeginTime">
          <el-date-picker
            v-model="form.BeginTime"
            type="datetime"
            placeholder="選擇開始時間"
            style="width: 100%"
            value-format="YYYY-MM-DD HH:mm:ss"
          />
        </el-form-item>
        <el-form-item label="結束時間" prop="EndTime">
          <el-date-picker
            v-model="form.EndTime"
            type="datetime"
            placeholder="選擇結束時間"
            style="width: 100%"
            value-format="YYYY-MM-DD HH:mm:ss"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input
            v-model="form.Notes"
            type="textarea"
            :rows="4"
            placeholder="請輸入備註"
            maxlength="500"
            show-word-limit
          />
        </el-form-item>
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
import {
  getUserAgents,
  getUserAgent,
  createUserAgent,
  updateUserAgent,
  deleteUserAgent,
  deleteUserAgentsBatch,
  updateUserAgentStatus
} from '@/api/userAgents'
import { getUsers } from '@/api/users'

// 查詢表單
const queryForm = reactive({
  PrincipalUserId: '',
  AgentUserId: '',
  Status: '',
  BeginTimeFrom: null,
  BeginTimeTo: null
})

// 表格資料
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 使用者列表（用於下拉選單）
const userList = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增代理')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  AgentId: null,
  PrincipalUserId: '',
  AgentUserId: '',
  BeginTime: null,
  EndTime: null,
  Status: 'A',
  Notes: ''
})

// 表單驗證規則
const rules = {
  PrincipalUserId: [
    { required: true, message: '請選擇委託人', trigger: 'change' }
  ],
  AgentUserId: [
    { required: true, message: '請選擇代理人', trigger: 'change' },
    {
      validator: (rule, value, callback) => {
        if (value && form.PrincipalUserId && value === form.PrincipalUserId) {
          callback(new Error('委託人和代理人不能相同'))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ],
  BeginTime: [
    { required: true, message: '請選擇開始時間', trigger: 'change' }
  ],
  EndTime: [
    { required: true, message: '請選擇結束時間', trigger: 'change' },
    {
      validator: (rule, value, callback) => {
        if (value && form.BeginTime && new Date(value) <= new Date(form.BeginTime)) {
          callback(new Error('結束時間必須大於開始時間'))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ],
  Status: [
    { required: true, message: '請選擇狀態', trigger: 'change' }
  ]
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

// 載入使用者列表
const loadUserList = async () => {
  try {
    const response = await getUsers({
      PageIndex: 1,
      PageSize: 1000,
      Status: 'A'
    })
    if (response && response.Data) {
      userList.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入使用者列表失敗:', error)
  }
}

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      PrincipalUserId: queryForm.PrincipalUserId || undefined,
      AgentUserId: queryForm.AgentUserId || undefined,
      Status: queryForm.Status || undefined,
      BeginTimeFrom: queryForm.BeginTimeFrom || undefined,
      BeginTimeTo: queryForm.BeginTimeTo || undefined
    }
    const response = await getUserAgents(params)
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
    PrincipalUserId: '',
    AgentUserId: '',
    Status: '',
    BeginTimeFrom: null,
    BeginTimeTo: null
  })
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增代理'
  Object.assign(form, {
    PrincipalUserId: '',
    AgentUserId: '',
    BeginTime: null,
    EndTime: null,
    Status: 'A',
    Notes: ''
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改代理'
  try {
    const response = await getUserAgent(row.AgentId)
    if (response && response.Data) {
      Object.assign(form, {
        AgentId: response.Data.AgentId,
        PrincipalUserId: response.Data.PrincipalUserId,
        AgentUserId: response.Data.AgentUserId,
        BeginTime: response.Data.BeginTime ? formatDateTime(response.Data.BeginTime) : null,
        EndTime: response.Data.EndTime ? formatDateTime(response.Data.EndTime) : null,
        Status: response.Data.Status || 'A',
        Notes: response.Data.Notes || ''
      })
      dialogVisible.value = true
    }
  } catch (error) {
    ElMessage.error('載入資料失敗: ' + (error.message || '未知錯誤'))
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除委託人「${row.PrincipalUserName || row.PrincipalUserId}」與代理人「${row.AgentUserName || row.AgentUserId}」的代理關係嗎？`,
      '確認刪除',
      {
        type: 'warning'
      }
    )
    await deleteUserAgent(row.AgentId)
    ElMessage.success('刪除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 批次刪除
const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇要刪除的資料')
    return
  }
  try {
    await ElMessageBox.confirm(
      `確定要刪除選取的 ${selectedRows.value.length} 筆資料嗎？`,
      '確認批次刪除',
      {
        type: 'warning'
      }
    )
    const agentIds = selectedRows.value.map(row => row.AgentId)
    await deleteUserAgentsBatch({ AgentIds: agentIds })
    ElMessage.success('批次刪除成功')
    selectedRows.value = []
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 切換狀態
const handleToggleStatus = async (row) => {
  try {
    const newStatus = row.Status === 'A' ? 'I' : 'A'
    const statusText = newStatus === 'A' ? '啟用' : '停用'
    await ElMessageBox.confirm(
      `確定要${statusText}此代理關係嗎？`,
      `確認${statusText}`,
      {
        type: 'warning'
      }
    )
    await updateUserAgentStatus(row.AgentId, { Status: newStatus })
    ElMessage.success(`${statusText}成功`)
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('更新狀態失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 委託人變更
const handlePrincipalUserChange = () => {
  // 如果委託人和代理人相同，清空代理人
  if (form.PrincipalUserId && form.AgentUserId && form.PrincipalUserId === form.AgentUserId) {
    form.AgentUserId = ''
    ElMessage.warning('委託人和代理人不能相同')
  }
}

// 代理人變更
const handleAgentUserChange = () => {
  // 如果委託人和代理人相同，清空代理人
  if (form.PrincipalUserId && form.AgentUserId && form.PrincipalUserId === form.AgentUserId) {
    form.AgentUserId = ''
    ElMessage.warning('委託人和代理人不能相同')
  }
}

// 提交
const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    const submitData = {
      PrincipalUserId: form.PrincipalUserId,
      AgentUserId: form.AgentUserId,
      BeginTime: form.BeginTime,
      EndTime: form.EndTime,
      Status: form.Status,
      Notes: form.Notes || undefined
    }
    if (isEdit.value) {
      if (form.AgentId) {
        await updateUserAgent(form.AgentId, submitData)
        ElMessage.success('修改成功')
      } else {
        ElMessage.error('找不到要修改的記錄')
      }
    } else {
      await createUserAgent(submitData)
      ElMessage.success('新增成功')
    }
    dialogVisible.value = false
    loadData()
  } catch (error) {
    if (error !== false) {
      ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 關閉對話框
const handleDialogClose = () => {
  formRef.value?.resetFields()
  dialogVisible.value = false
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
  loadUserList()
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0117-query {
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

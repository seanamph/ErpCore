<template>
  <div class="sys0440-query">
    <div class="page-header">
      <h1>系統功能按鈕資料維護 (SYS0440)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="作業代碼">
          <el-select v-model="queryForm.ProgramId" placeholder="請選擇作業" clearable filterable style="width: 200px">
            <el-option
              v-for="item in programOptions"
              :key="item.ProgramId"
              :label="item.ProgramName"
              :value="item.ProgramId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="按鈕代碼">
          <el-input v-model="queryForm.ButtonId" placeholder="請輸入按鈕代碼" clearable />
        </el-form-item>
        <el-form-item label="按鈕名稱">
          <el-input v-model="queryForm.ButtonName" placeholder="請輸入按鈕名稱" clearable />
        </el-form-item>
        <el-form-item label="頁面代碼">
          <el-input v-model="queryForm.PageId" placeholder="請輸入頁面代碼" clearable />
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
        <el-table-column prop="ProgramName" label="作業" width="200" />
        <el-table-column prop="ButtonId" label="按鈕代碼" width="120" sortable />
        <el-table-column prop="ButtonName" label="按鈕名稱" width="150" sortable />
        <el-table-column prop="PageId" label="頁面代碼" width="100" />
        <el-table-column prop="ButtonMsg" label="按鈕訊息" width="200" show-overflow-tooltip />
        <el-table-column prop="ButtonUrl" label="網頁鏈結位址" width="200" show-overflow-tooltip />
        <el-table-column prop="MsgTypeName" label="訊息型態" width="120" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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
      <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
        <el-form-item label="作業代碼" prop="ProgramId">
          <el-select v-model="form.ProgramId" placeholder="請選擇作業" filterable style="width: 100%">
            <el-option
              v-for="item in programOptions"
              :key="item.ProgramId"
              :label="item.ProgramName"
              :value="item.ProgramId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="按鈕代碼" prop="ButtonId">
          <el-input v-model="form.ButtonId" placeholder="請輸入按鈕代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="按鈕名稱" prop="ButtonName">
          <el-input v-model="form.ButtonName" placeholder="請輸入按鈕名稱" maxlength="100" />
        </el-form-item>
        <el-form-item label="頁面代碼" prop="PageId">
          <el-input v-model="form.PageId" placeholder="請輸入頁面代碼" maxlength="50" />
        </el-form-item>
        <el-form-item label="按鈕訊息" prop="ButtonMsg">
          <el-input v-model="form.ButtonMsg" type="textarea" :rows="3" placeholder="請輸入按鈕訊息" maxlength="500" />
        </el-form-item>
        <el-form-item label="按鈕屬性" prop="ButtonAttr">
          <el-input v-model="form.ButtonAttr" placeholder="請輸入按鈕屬性" maxlength="50" />
        </el-form-item>
        <el-form-item label="網頁鏈結位址" prop="ButtonUrl">
          <el-input v-model="form.ButtonUrl" placeholder="請輸入網頁鏈結位址" maxlength="500" />
        </el-form-item>
        <el-form-item label="訊息型態" prop="MsgType">
          <el-select v-model="form.MsgType" placeholder="請選擇訊息型態" clearable filterable style="width: 100%">
            <el-option
              v-for="item in msgTypeOptions"
              :key="item.Value"
              :label="item.Name"
              :value="item.Value"
            />
          </el-select>
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
import { getButtons, getButtonById, createButton, updateButton, deleteButton, deleteButtonsBatch } from '@/api/buttons'
import { getPrograms } from '@/api/programs'
import { parametersApi } from '@/api/parameters'

// 查詢表單
const queryForm = reactive({
  ProgramId: '',
  ButtonId: '',
  ButtonName: '',
  PageId: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增按鈕')
const isEdit = ref(false)
const formRef = ref(null)
const currentTKey = ref(null)

// 選項資料
const programOptions = ref([])
const msgTypeOptions = ref([])

// 表單資料
const form = reactive({
  ProgramId: '',
  ButtonId: '',
  ButtonName: '',
  PageId: '',
  ButtonMsg: '',
  ButtonAttr: '',
  ButtonUrl: '',
  MsgType: ''
})

// 表單驗證規則
const rules = {
  ProgramId: [
    { required: true, message: '請選擇作業代碼', trigger: 'change' }
  ],
  ButtonId: [
    { required: true, message: '請輸入按鈕代碼', trigger: 'blur' },
    { max: 50, message: '按鈕代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  ButtonName: [
    { required: true, message: '請輸入按鈕名稱', trigger: 'blur' },
    { max: 100, message: '按鈕名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  PageId: [
    { max: 50, message: '頁面代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  ButtonMsg: [
    { max: 500, message: '按鈕訊息長度不能超過500個字元', trigger: 'blur' }
  ],
  ButtonAttr: [
    { max: 50, message: '按鈕屬性長度不能超過50個字元', trigger: 'blur' }
  ],
  ButtonUrl: [
    { max: 500, message: '網頁鏈結位址長度不能超過500個字元', trigger: 'blur' }
  ]
}

// 載入作業選項
const loadProgramOptions = async () => {
  try {
    const response = await getPrograms({
      PageIndex: 1,
      PageSize: 1000
    })
    if (response && response.Data) {
      programOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入作業選項失敗:', error)
  }
}

// 載入訊息型態選項
const loadMsgTypeOptions = async () => {
  try {
    const response = await parametersApi.getParametersByTitle('BUT_MSG')
    if (response && response.Data) {
      msgTypeOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入訊息型態選項失敗:', error)
  }
}

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      Filters: {
        ProgramId: queryForm.ProgramId || undefined,
        ButtonId: queryForm.ButtonId || undefined,
        ButtonName: queryForm.ButtonName || undefined,
        PageId: queryForm.PageId || undefined
      }
    }
    const response = await getButtons(params)
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
    ProgramId: '',
    ButtonId: '',
    ButtonName: '',
    PageId: ''
  })
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增按鈕'
  currentTKey.value = null
  Object.assign(form, {
    ProgramId: '',
    ButtonId: '',
    ButtonName: '',
    PageId: '',
    ButtonMsg: '',
    ButtonAttr: '',
    ButtonUrl: '',
    MsgType: ''
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改按鈕'
  try {
    const response = await getButtonById(row.TKey)
    if (response && response.Data) {
      currentTKey.value = response.Data.TKey
      Object.assign(form, {
        ProgramId: response.Data.ProgramId,
        ButtonId: response.Data.ButtonId,
        ButtonName: response.Data.ButtonName,
        PageId: response.Data.PageId || '',
        ButtonMsg: response.Data.ButtonMsg || '',
        ButtonAttr: response.Data.ButtonAttr || '',
        ButtonUrl: response.Data.ButtonUrl || '',
        MsgType: response.Data.MsgType || ''
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
    await ElMessageBox.confirm(`確定要刪除按鈕「${row.ButtonName}」嗎？`, '確認刪除', {
      type: 'warning'
    })
    await deleteButton(row.TKey)
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
    await ElMessageBox.confirm(`確定要刪除選取的 ${selectedRows.value.length} 筆資料嗎？`, '確認批次刪除', {
      type: 'warning'
    })
    const tKeys = selectedRows.value.map(row => row.TKey)
    await deleteButtonsBatch({ TKeys: tKeys })
    ElMessage.success('批次刪除成功')
    selectedRows.value = []
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 提交
const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    if (isEdit.value) {
      await updateButton(currentTKey.value, form)
      ElMessage.success('修改成功')
    } else {
      await createButton(form)
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
  loadProgramOptions()
  loadMsgTypeOptions()
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0440-query {
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

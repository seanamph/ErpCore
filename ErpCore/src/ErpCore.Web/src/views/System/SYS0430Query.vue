<template>
  <div class="sys0430-query">
    <div class="page-header">
      <h1>系統作業資料維護 (SYS0430)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="作業代碼">
          <el-input v-model="queryForm.ProgramId" placeholder="請輸入作業代碼" clearable />
        </el-form-item>
        <el-form-item label="作業名稱">
          <el-input v-model="queryForm.ProgramName" placeholder="請輸入作業名稱" clearable />
        </el-form-item>
        <el-form-item label="子系統項目代碼">
          <el-select v-model="queryForm.MenuId" placeholder="請選擇子系統" clearable filterable style="width: 200px">
            <el-option
              v-for="item in menuOptions"
              :key="item.MenuId"
              :label="item.MenuName"
              :value="item.MenuId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="作業型態">
          <el-select v-model="queryForm.ProgramType" placeholder="請選擇作業型態" clearable filterable style="width: 200px">
            <el-option
              v-for="item in programTypeOptions"
              :key="item.Tag"
              :label="item.Content"
              :value="item.Tag"
            />
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="ProgramId" label="作業代碼" width="150" sortable />
        <el-table-column prop="ProgramName" label="作業名稱" width="200" sortable />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="center" sortable />
        <el-table-column prop="MenuName" label="子系統" width="150" />
        <el-table-column prop="ProgramUrl" label="網頁位址" width="200" show-overflow-tooltip />
        <el-table-column prop="ProgramTypeName" label="作業型態" width="120" />
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
          <el-input v-model="form.ProgramId" placeholder="請輸入作業代碼" :disabled="isEdit" maxlength="50" />
        </el-form-item>
        <el-form-item label="作業名稱" prop="ProgramName">
          <el-input v-model="form.ProgramName" placeholder="請輸入作業名稱" maxlength="100" />
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="子系統項目代碼" prop="MenuId">
          <el-select v-model="form.MenuId" placeholder="請選擇子系統" filterable style="width: 100%">
            <el-option
              v-for="item in menuOptions"
              :key="item.MenuId"
              :label="item.MenuName"
              :value="item.MenuId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="網頁位址" prop="ProgramUrl">
          <el-input v-model="form.ProgramUrl" placeholder="請輸入網頁位址" maxlength="500" />
        </el-form-item>
        <el-form-item label="作業型態" prop="ProgramType">
          <el-select v-model="form.ProgramType" placeholder="請選擇作業型態" clearable filterable style="width: 100%">
            <el-option
              v-for="item in programTypeOptions"
              :key="item.Tag"
              :label="item.Content"
              :value="item.Tag"
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
import { getPrograms, getProgramById, createProgram, updateProgram, deleteProgram, deleteProgramsBatch } from '@/api/programs'
import { getMenus } from '@/api/menus'
import { parametersApi } from '@/api/parameters'

// 查詢表單
const queryForm = reactive({
  ProgramId: '',
  ProgramName: '',
  MenuId: '',
  ProgramType: ''
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
const dialogTitle = ref('新增作業')
const isEdit = ref(false)
const formRef = ref(null)

// 選項資料
const menuOptions = ref([])
const programTypeOptions = ref([])

// 表單資料
const form = reactive({
  ProgramId: '',
  ProgramName: '',
  SeqNo: 0,
  MenuId: '',
  ProgramUrl: '',
  ProgramType: ''
})

// 表單驗證規則
const rules = {
  ProgramId: [
    { required: true, message: '請輸入作業代碼', trigger: 'blur' },
    { max: 50, message: '作業代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  ProgramName: [
    { required: true, message: '請輸入作業名稱', trigger: 'blur' },
    { max: 100, message: '作業名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  SeqNo: [
    { required: true, message: '請輸入排序序號', trigger: 'blur' },
    { type: 'number', min: 0, message: '排序序號必須大於等於0', trigger: 'blur' }
  ],
  MenuId: [
    { required: true, message: '請選擇子系統項目代碼', trigger: 'change' }
  ],
  ProgramUrl: [
    { required: true, message: '請輸入網頁位址', trigger: 'blur' },
    { max: 500, message: '網頁位址長度不能超過500個字元', trigger: 'blur' }
  ]
}

// 載入子系統選項
const loadMenuOptions = async () => {
  try {
    const response = await getMenus({
      PageIndex: 1,
      PageSize: 1000
    })
    if (response && response.Data) {
      menuOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入子系統選項失敗:', error)
  }
}

// 載入作業型態選項
const loadProgramTypeOptions = async () => {
  try {
    const response = await parametersApi.getParametersByTitle('PROG_TYPE')
    if (response && response.Data) {
      programTypeOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入作業型態選項失敗:', error)
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
        ProgramName: queryForm.ProgramName || undefined,
        MenuId: queryForm.MenuId || undefined,
        ProgramType: queryForm.ProgramType || undefined
      }
    }
    const response = await getPrograms(params)
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
    ProgramName: '',
    MenuId: '',
    ProgramType: ''
  })
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增作業'
  Object.assign(form, {
    ProgramId: '',
    ProgramName: '',
    SeqNo: 0,
    MenuId: '',
    ProgramUrl: '',
    ProgramType: ''
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改作業'
  try {
    const response = await getProgramById(row.ProgramId)
    if (response && response.Data) {
      Object.assign(form, {
        ProgramId: response.Data.ProgramId,
        ProgramName: response.Data.ProgramName,
        SeqNo: response.Data.SeqNo || 0,
        MenuId: response.Data.MenuId,
        ProgramUrl: response.Data.ProgramUrl || '',
        ProgramType: response.Data.ProgramType || ''
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
    await ElMessageBox.confirm(`確定要刪除作業「${row.ProgramName}」嗎？`, '確認刪除', {
      type: 'warning'
    })
    await deleteProgram(row.ProgramId)
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
    const programIds = selectedRows.value.map(row => row.ProgramId)
    await deleteProgramsBatch({ ProgramIds: programIds })
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
      const updateData = {
        ProgramName: form.ProgramName,
        SeqNo: form.SeqNo,
        MenuId: form.MenuId,
        ProgramUrl: form.ProgramUrl,
        ProgramType: form.ProgramType
      }
      await updateProgram(form.ProgramId, updateData)
      ElMessage.success('修改成功')
    } else {
      await createProgram(form)
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
  loadMenuOptions()
  loadProgramTypeOptions()
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0430-query {
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

<template>
  <div class="sys0420-query">
    <div class="page-header">
      <h1>子系統項目資料維護 (SYS0420)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="子系統項目代碼">
          <el-input v-model="queryForm.MenuId" placeholder="請輸入子系統項目代碼" clearable />
        </el-form-item>
        <el-form-item label="子系統項目名稱">
          <el-input v-model="queryForm.MenuName" placeholder="請輸入子系統項目名稱" clearable />
        </el-form-item>
        <el-form-item label="主系統代碼">
          <el-select v-model="queryForm.SystemId" placeholder="請選擇主系統" clearable filterable style="width: 200px">
            <el-option
              v-for="item in systemOptions"
              :key="item.SystemId"
              :label="item.SystemName"
              :value="item.SystemId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="上層子系統代碼">
          <el-select v-model="queryForm.ParentMenuId" placeholder="請選擇上層子系統" clearable filterable style="width: 200px">
            <el-option label="無（根節點）" value="0" />
            <el-option
              v-for="item in menuOptions"
              :key="item.MenuId"
              :label="item.MenuName"
              :value="item.MenuId"
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
        <el-table-column prop="MenuId" label="子系統項目代碼" width="150" sortable />
        <el-table-column prop="MenuName" label="子系統項目名稱" width="200" sortable />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="center" sortable />
        <el-table-column prop="SystemName" label="主系統" width="150" />
        <el-table-column prop="ParentMenuName" label="上層子系統" width="150" />
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
        <el-form-item label="子系統項目代碼" prop="MenuId">
          <el-input v-model="form.MenuId" placeholder="請輸入子系統項目代碼" :disabled="isEdit" maxlength="50" />
        </el-form-item>
        <el-form-item label="子系統項目名稱" prop="MenuName">
          <el-input v-model="form.MenuName" placeholder="請輸入子系統項目名稱" maxlength="100" />
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="主系統代碼" prop="SystemId">
          <el-select v-model="form.SystemId" placeholder="請選擇主系統" filterable style="width: 100%">
            <el-option
              v-for="item in systemOptions"
              :key="item.SystemId"
              :label="item.SystemName"
              :value="item.SystemId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="上層子系統代碼" prop="ParentMenuId">
          <el-select v-model="form.ParentMenuId" placeholder="請選擇上層子系統" filterable style="width: 100%">
            <el-option label="無（根節點）" value="0" />
            <el-option
              v-for="item in menuOptions"
              :key="item.MenuId"
              :label="item.MenuName"
              :value="item.MenuId"
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
import { getMenus, getMenuById, createMenu, updateMenu, deleteMenu, deleteMenusBatch } from '@/api/menus'
import { getSystems } from '@/api/systems'

// 查詢表單
const queryForm = reactive({
  MenuId: '',
  MenuName: '',
  SystemId: '',
  ParentMenuId: ''
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
const dialogTitle = ref('新增子系統')
const isEdit = ref(false)
const formRef = ref(null)

// 選項資料
const systemOptions = ref([])
const menuOptions = ref([])

// 表單資料
const form = reactive({
  MenuId: '',
  MenuName: '',
  SeqNo: 0,
  SystemId: '',
  ParentMenuId: '0'
})

// 表單驗證規則
const rules = {
  MenuId: [
    { required: true, message: '請輸入子系統項目代碼', trigger: 'blur' },
    { max: 50, message: '子系統項目代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  MenuName: [
    { required: true, message: '請輸入子系統項目名稱', trigger: 'blur' },
    { max: 100, message: '子系統項目名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  SeqNo: [
    { required: true, message: '請輸入排序序號', trigger: 'blur' },
    { type: 'number', min: 0, message: '排序序號必須大於等於0', trigger: 'blur' }
  ],
  SystemId: [
    { required: true, message: '請選擇主系統代碼', trigger: 'change' }
  ],
  ParentMenuId: [
    { required: true, message: '請選擇上層子系統代碼', trigger: 'change' }
  ]
}

// 載入主系統選項
const loadSystemOptions = async () => {
  try {
    const response = await getSystems({
      PageIndex: 1,
      PageSize: 1000,
      Filters: {
        Status: 'A'
      }
    })
    if (response && response.Data) {
      systemOptions.value = response.Data.Items || []
    }
  } catch (error) {
    console.error('載入主系統選項失敗:', error)
  }
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

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      Filters: {
        MenuId: queryForm.MenuId || undefined,
        MenuName: queryForm.MenuName || undefined,
        SystemId: queryForm.SystemId || undefined,
        ParentMenuId: queryForm.ParentMenuId || undefined
      }
    }
    const response = await getMenus(params)
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
    MenuId: '',
    MenuName: '',
    SystemId: '',
    ParentMenuId: ''
  })
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增子系統'
  Object.assign(form, {
    MenuId: '',
    MenuName: '',
    SeqNo: 0,
    SystemId: '',
    ParentMenuId: '0'
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改子系統'
  try {
    const response = await getMenuById(row.MenuId)
    if (response && response.Data) {
      Object.assign(form, {
        MenuId: response.Data.MenuId,
        MenuName: response.Data.MenuName,
        SeqNo: response.Data.SeqNo || 0,
        SystemId: response.Data.SystemId,
        ParentMenuId: response.Data.ParentMenuId || '0'
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
    await ElMessageBox.confirm(`確定要刪除子系統「${row.MenuName}」嗎？`, '確認刪除', {
      type: 'warning'
    })
    await deleteMenu(row.MenuId)
    ElMessage.success('刪除成功')
    loadData()
    loadMenuOptions() // 重新載入選項
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
    const menuIds = selectedRows.value.map(row => row.MenuId)
    await deleteMenusBatch({ MenuIds: menuIds })
    ElMessage.success('批次刪除成功')
    selectedRows.value = []
    loadData()
    loadMenuOptions() // 重新載入選項
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
        MenuName: form.MenuName,
        SeqNo: form.SeqNo,
        SystemId: form.SystemId,
        ParentMenuId: form.ParentMenuId
      }
      await updateMenu(form.MenuId, updateData)
      ElMessage.success('修改成功')
    } else {
      await createMenu(form)
      ElMessage.success('新增成功')
    }
    dialogVisible.value = false
    loadData()
    loadMenuOptions() // 重新載入選項
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
  loadSystemOptions()
  loadMenuOptions()
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0420-query {
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

<template>
  <div class="programs">
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
        <el-form-item label="子系統項目">
          <el-select v-model="queryForm.MenuId" placeholder="請選擇子系統" clearable filterable>
            <el-option
              v-for="menu in menuList"
              :key="menu.MenuId"
              :label="menu.MenuName"
              :value="menu.MenuId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="作業型態">
          <el-select v-model="queryForm.ProgramType" placeholder="請選擇作業型態" clearable>
            <el-option
              v-for="type in programTypeList"
              :key="type.Tag"
              :label="type.Content"
              :value="type.Tag"
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
      >
        <el-table-column prop="ProgramId" label="作業代碼" width="150" />
        <el-table-column prop="ProgramName" label="作業名稱" width="200" />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
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
        <el-table-column label="操作" width="200" fixed="right">
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
    </el-card>

    <!-- 新增/修改對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="作業代碼" prop="ProgramId">
          <el-input v-model="form.ProgramId" placeholder="請輸入作業代碼" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="作業名稱" prop="ProgramName">
          <el-input v-model="form.ProgramName" placeholder="請輸入作業名稱" />
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="子系統項目代碼" prop="MenuId">
          <el-select v-model="form.MenuId" placeholder="請選擇子系統" filterable style="width: 100%">
            <el-option
              v-for="menu in menuList"
              :key="menu.MenuId"
              :label="menu.MenuName"
              :value="menu.MenuId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="網頁位址" prop="ProgramUrl">
          <el-input v-model="form.ProgramUrl" placeholder="請輸入網頁位址" />
        </el-form-item>
        <el-form-item label="作業型態" prop="ProgramType">
          <el-select v-model="form.ProgramType" placeholder="請選擇作業型態" clearable style="width: 100%">
            <el-option
              v-for="type in programTypeList"
              :key="type.Tag"
              :label="type.Content"
              :value="type.Tag"
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
import {
  getPrograms,
  getProgramById,
  createProgram,
  updateProgram,
  deleteProgram
} from '@/api/programs'
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
  ProgramId: [{ required: true, message: '請輸入作業代碼', trigger: 'blur' }],
  ProgramName: [{ required: true, message: '請輸入作業名稱', trigger: 'blur' }],
  SeqNo: [{ required: true, message: '請輸入排序序號', trigger: 'blur' }],
  MenuId: [{ required: true, message: '請選擇子系統項目代碼', trigger: 'change' }],
  ProgramUrl: [{ required: true, message: '請輸入網頁位址', trigger: 'blur' }]
}

// 子系統列表
const menuList = ref([])

// 作業型態列表
const programTypeList = ref([])

// 載入子系統列表
const loadMenuList = async () => {
  try {
    const response = await getMenus({ PageIndex: 1, PageSize: 1000 })
    if (response.data.success) {
      menuList.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('載入子系統列表失敗:', error)
  }
}

// 載入作業型態列表
const loadProgramTypeList = async () => {
  try {
    const response = await parametersApi.getParametersByTitle('PROG_TYPE')
    if (response.data.success) {
      programTypeList.value = response.data.data || []
    }
  } catch (error) {
    console.error('載入作業型態列表失敗:', error)
  }
}

// 查詢
const handleSearch = async () => {
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
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.message || error.message))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ProgramId = ''
  queryForm.ProgramName = ''
  queryForm.MenuId = ''
  queryForm.ProgramType = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增作業'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改作業'
  try {
    const response = await getProgramById(row.ProgramId)
    if (response.data.success) {
      const data = response.data.data
      form.ProgramId = data.programId || ''
      form.ProgramName = data.programName || ''
      form.SeqNo = data.seqNo || 0
      form.MenuId = data.menuId || ''
      form.ProgramUrl = data.programUrl || ''
      form.ProgramType = data.programType || ''
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.response?.data?.message || error.message))
  }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此作業嗎？', '提示', {
      type: 'warning'
    })
    const response = await deleteProgram(row.ProgramId)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.response?.data?.message || error.message))
    }
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          const updateData = {
            ProgramName: form.ProgramName,
            SeqNo: form.SeqNo,
            MenuId: form.MenuId,
            ProgramUrl: form.ProgramUrl,
            ProgramType: form.ProgramType || undefined
          }
          response = await updateProgram(form.ProgramId, updateData)
        } else {
          const createData = {
            ProgramId: form.ProgramId,
            ProgramName: form.ProgramName,
            SeqNo: form.SeqNo,
            MenuId: form.MenuId,
            ProgramUrl: form.ProgramUrl,
            ProgramType: form.ProgramType || undefined
          }
          response = await createProgram(createData)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error(
          (isEdit.value ? '修改失敗' : '新增失敗') +
            '：' +
            (error.response?.data?.message || error.message)
        )
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
  form.ProgramId = ''
  form.ProgramName = ''
  form.SeqNo = 0
  form.MenuId = ''
  form.ProgramUrl = ''
  form.ProgramType = ''
  formRef.value?.resetFields()
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

// 初始化
onMounted(async () => {
  await Promise.all([loadMenuList(), loadProgramTypeList()])
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.programs {
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

<template>
  <div class="parameters">
    <div class="page-header">
      <h1>參數資料設定維護 (SYSBC40)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="參數標題">
          <el-input v-model="queryForm.Title" placeholder="請輸入參數標題" clearable />
        </el-form-item>
        <el-form-item label="參數標籤">
          <el-input v-model="queryForm.Tag" placeholder="請輸入參數標籤" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="系統">
          <el-input v-model="queryForm.SystemId" placeholder="請輸入系統ID" clearable />
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
        <el-table-column prop="Title" label="參數標題" width="150" />
        <el-table-column prop="Tag" label="參數標籤" width="150" />
        <el-table-column prop="Content" label="參數內容" width="200" show-overflow-tooltip />
        <el-table-column prop="Content2" label="多語言內容" width="200" show-overflow-tooltip />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ReadOnly" label="只讀" width="80">
          <template #default="{ row }">
            <el-tag :type="row.ReadOnly === '1' ? 'warning' : 'info'">
              {{ row.ReadOnly === '1' ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="SystemId" label="系統ID" width="120" />
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button 
              type="danger" 
              size="small" 
              @click="handleDelete(row)" 
              :disabled="row.ReadOnly === '1'"
            >
              刪除
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
      :title="dialogTitle"
      v-model="dialogVisible"
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="參數標題" prop="Title">
          <el-input v-model="form.Title" placeholder="請輸入參數標題" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="參數標籤" prop="Tag">
          <el-input v-model="form.Tag" placeholder="請輸入參數標籤" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="參數內容" prop="Content">
          <el-input v-model="form.Content" type="textarea" :rows="3" placeholder="請輸入參數內容" />
        </el-form-item>
        <el-form-item label="多語言內容" prop="Content2">
          <el-input v-model="form.Content2" type="textarea" :rows="3" placeholder="請輸入多語言參數內容" />
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="只讀" prop="ReadOnly">
          <el-select v-model="form.ReadOnly" placeholder="請選擇只讀標誌" style="width: 100%">
            <el-option label="否" value="0" />
            <el-option label="是" value="1" />
          </el-select>
        </el-form-item>
        <el-form-item label="系統ID" prop="SystemId">
          <el-input v-model="form.SystemId" placeholder="請輸入系統ID" />
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
import { parametersApi } from '@/api/parameters'

// 查詢表單
const queryForm = reactive({
  Title: '',
  Tag: '',
  Status: '',
  SystemId: ''
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
const dialogTitle = ref('新增參數')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  Title: '',
  Tag: '',
  SeqNo: 0,
  Content: '',
  Content2: '',
  Notes: '',
  Status: '1',
  ReadOnly: '0',
  SystemId: ''
})

// 保存原始標題和標籤（用於修改）
const originalTitle = ref('')
const originalTag = ref('')

// 表單驗證規則
const rules = {
  Title: [{ required: true, message: '請輸入參數標題', trigger: 'blur' }],
  Tag: [{ required: true, message: '請輸入參數標籤', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      Title: queryForm.Title || undefined,
      Tag: queryForm.Tag || undefined,
      Status: queryForm.Status || undefined,
      SystemId: queryForm.SystemId || undefined
    }
    const response = await parametersApi.getParameters(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.TotalCount = response.data.data.totalCount
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.Title = ''
  queryForm.Tag = ''
  queryForm.Status = ''
  queryForm.SystemId = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增參數'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改參數'
  originalTitle.value = row.Title
  originalTag.value = row.Tag
  try {
    const response = await parametersApi.getParameter(row.Title, row.Tag)
    if (response.data.success) {
      Object.assign(form, response.data.data)
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此參數嗎？', '提示', {
      type: 'warning'
    })
    const response = await parametersApi.deleteParameter(row.Title, row.Tag)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
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
          // 修改時使用原始標題和標籤
          const updateData = {
            SeqNo: form.SeqNo,
            Content: form.Content,
            Content2: form.Content2,
            Notes: form.Notes,
            Status: form.Status,
            ReadOnly: form.ReadOnly,
            SystemId: form.SystemId
          }
          response = await parametersApi.updateParameter(originalTitle.value, originalTag.value, updateData)
        } else {
          response = await parametersApi.createParameter(form)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + error.message)
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
  form.Title = ''
  form.Tag = ''
  form.SeqNo = 0
  form.Content = ''
  form.Content2 = ''
  form.Notes = ''
  form.Status = '1'
  form.ReadOnly = '0'
  form.SystemId = ''
  originalTitle.value = ''
  originalTag.value = ''
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
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.parameters {
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

<template>
  <div class="system-extension">
    <div class="page-header">
      <h1>系統擴展資料維護 (SYSX110)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="擴展功能代碼">
          <el-input v-model="queryForm.ExtensionId" placeholder="請輸入擴展功能代碼" clearable />
        </el-form-item>
        <el-form-item label="擴展功能名稱">
          <el-input v-model="queryForm.ExtensionName" placeholder="請輸入擴展功能名稱" clearable />
        </el-form-item>
        <el-form-item label="擴展類型">
          <el-input v-model="queryForm.ExtensionType" placeholder="請輸入擴展類型" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
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
        <el-table-column prop="ExtensionId" label="擴展功能代碼" width="150" />
        <el-table-column prop="ExtensionName" label="擴展功能名稱" width="200" />
        <el-table-column prop="ExtensionType" label="擴展類型" width="120" />
        <el-table-column prop="ExtensionValue" label="擴展值" width="200" show-overflow-tooltip />
        <el-table-column prop="SeqNo" label="排序序號" width="100" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="UpdatedAt" label="更新時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.UpdatedAt) }}
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
        <el-form-item label="擴展功能代碼" prop="ExtensionId">
          <el-input v-model="form.ExtensionId" placeholder="請輸入擴展功能代碼" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="擴展功能名稱" prop="ExtensionName">
          <el-input v-model="form.ExtensionName" placeholder="請輸入擴展功能名稱" />
        </el-form-item>
        <el-form-item label="擴展類型" prop="ExtensionType">
          <el-input v-model="form.ExtensionType" placeholder="請輸入擴展類型" />
        </el-form-item>
        <el-form-item label="擴展值" prop="ExtensionValue">
          <el-input v-model="form.ExtensionValue" placeholder="請輸入擴展值" />
        </el-form-item>
        <el-form-item label="擴展設定" prop="ExtensionConfig">
          <el-input v-model="form.ExtensionConfig" type="textarea" :rows="5" placeholder="請輸入JSON格式的擴展設定" />
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-radio-group v-model="form.Status">
            <el-radio label="1">啟用</el-radio>
            <el-radio label="0">停用</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
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
import { systemExtensionApi } from '@/api/systemExtension'

// 查詢表單
const queryForm = reactive({
  ExtensionId: '',
  ExtensionName: '',
  ExtensionType: '',
  Status: ''
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
const dialogTitle = ref('新增系統擴展')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  ExtensionId: '',
  ExtensionName: '',
  ExtensionType: '',
  ExtensionValue: '',
  ExtensionConfig: '',
  SeqNo: 0,
  Status: '1',
  Notes: ''
})

// 保存原始主鍵（用於修改）
const originalTKey = ref(null)

// 表單驗證規則
const rules = {
  ExtensionId: [{ required: true, message: '請輸入擴展功能代碼', trigger: 'blur' }],
  ExtensionName: [{ required: true, message: '請輸入擴展功能名稱', trigger: 'blur' }],
  ExtensionConfig: [
    {
      validator: (rule, value, callback) => {
        if (value && value.trim()) {
          try {
            JSON.parse(value)
            callback()
          } catch (e) {
            callback(new Error('擴展設定必須是有效的JSON格式'))
          }
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return ''
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

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ExtensionId: queryForm.ExtensionId || undefined,
      ExtensionName: queryForm.ExtensionName || undefined,
      ExtensionType: queryForm.ExtensionType || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await systemExtensionApi.getSystemExtensions(params)
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
  queryForm.ExtensionId = ''
  queryForm.ExtensionName = ''
  queryForm.ExtensionType = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增系統擴展'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改系統擴展'
  originalTKey.value = row.TKey
  try {
    const response = await systemExtensionApi.getSystemExtension(row.TKey)
    if (response.data.success) {
      const data = response.data.data
      Object.assign(form, {
        ExtensionId: data.ExtensionId || data.extensionId || '',
        ExtensionName: data.ExtensionName || data.extensionName || '',
        ExtensionType: data.ExtensionType || data.extensionType || '',
        ExtensionValue: data.ExtensionValue || data.extensionValue || '',
        ExtensionConfig: data.ExtensionConfig || data.extensionConfig || '',
        SeqNo: data.SeqNo || data.seqNo || 0,
        Status: data.Status || data.status || '1',
        Notes: data.Notes || data.notes || ''
      })
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
    await ElMessageBox.confirm('確定要刪除此系統擴展嗎？', '提示', {
      type: 'warning'
    })
    const response = await systemExtensionApi.deleteSystemExtension(row.TKey)
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
        const submitData = {
          ExtensionId: form.ExtensionId,
          ExtensionName: form.ExtensionName,
          ExtensionType: form.ExtensionType || null,
          ExtensionValue: form.ExtensionValue || null,
          ExtensionConfig: form.ExtensionConfig || null,
          SeqNo: form.SeqNo || 0,
          Status: form.Status,
          Notes: form.Notes || null
        }
        if (isEdit.value) {
          response = await systemExtensionApi.updateSystemExtension(originalTKey.value, submitData)
        } else {
          response = await systemExtensionApi.createSystemExtension(submitData)
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
  form.ExtensionId = ''
  form.ExtensionName = ''
  form.ExtensionType = ''
  form.ExtensionValue = ''
  form.ExtensionConfig = ''
  form.SeqNo = 0
  form.Status = '1'
  form.Notes = ''
  originalTKey.value = null
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

.system-extension {
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

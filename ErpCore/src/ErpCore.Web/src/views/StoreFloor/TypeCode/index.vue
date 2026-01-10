<template>
  <div class="type-code-data">
    <div class="page-header">
      <h1>È°ûÂ?‰ª?¢ºÁ∂≠Ë≠∑ (SYS6405-SYS6490)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="È°ûÂ?‰ª?¢º">
          <el-input v-model="queryForm.TypeCode" placeholder="Ë´ãËº∏?•È??ã‰ª£Á¢? clearable />
        </el-form-item>
        <el-form-item label="È°ûÂ??çÁ®±">
          <el-input v-model="queryForm.TypeName" placeholder="Ë´ãËº∏?•È??ãÂ?Á®? clearable />
        </el-form-item>
        <el-form-item label="?ÜÈ?">
          <el-input v-model="queryForm.Category" placeholder="Ë´ãËº∏?•Â?È°? clearable />
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="?üÁî®" value="A" />
            <el-option label="?úÁî®" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
          <el-button type="success" @click="handleCreate">?∞Â?</el-button>
          <el-button type="danger" @click="handleBatchDelete" :disabled="selectedRows.length === 0">?πÊ¨°?™Èô§</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- Ë≥áÊ?Ë°®Ê†º -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="TypeCode" label="È°ûÂ?‰ª?¢º" width="150" />
        <el-table-column prop="TypeName" label="È°ûÂ??çÁ®±" width="200" />
        <el-table-column prop="TypeNameEn" label="?±Ê??çÁ®±" width="200" />
        <el-table-column prop="Category" label="?ÜÈ?" width="150" />
        <el-table-column prop="Description" label="Ë™™Ê?" width="300" show-overflow-tooltip />
        <el-table-column prop="SortOrder" label="?íÂ??ÜÂ?" width="120" />
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">‰øÆÊîπ</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">?™Èô§</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- ?ÜÈ? -->
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

    <!-- ?∞Â?/‰øÆÊîπÂ∞çË©±Ê°?-->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="700px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="150px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="È°ûÂ?‰ª?¢º" prop="TypeCode">
              <el-input v-model="formData.TypeCode" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="È°ûÂ??çÁ®±" prop="TypeName">
              <el-input v-model="formData.TypeName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?±Ê??çÁ®±">
              <el-input v-model="formData.TypeNameEn" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ÜÈ?">
              <el-input v-model="formData.Category" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?íÂ??ÜÂ?">
              <el-input-number v-model="formData.SortOrder" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?Ä?? prop="Status">
              <el-radio-group v-model="formData.Status">
                <el-radio label="A">?üÁî®</el-radio>
                <el-radio label="I">?úÁî®</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="Ë™™Ê?">
          <el-input v-model="formData.Description" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmit">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { typeCodeApi } from '@/api/storeFloor'

// ?•Ë©¢Ë°®ÂñÆ
const queryForm = reactive({
  TypeCode: '',
  TypeName: '',
  Category: '',
  Status: ''
})

// Ë°®Ê†ºË≥áÊ?
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// ?ÜÈ?
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// Â∞çË©±Ê°?
const dialogVisible = ref(false)
const dialogTitle = computed(() => isEdit.value ? '‰øÆÊîπÈ°ûÂ?‰ª?¢º' : '?∞Â?È°ûÂ?‰ª?¢º')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  TypeCode: '',
  TypeName: '',
  TypeNameEn: '',
  Category: '',
  Description: '',
  SortOrder: null,
  Status: 'A'
})
const formRules = {
  TypeCode: [{ required: true, message: 'Ë´ãËº∏?•È??ã‰ª£Á¢?, trigger: 'blur' }],
  TypeName: [{ required: true, message: 'Ë´ãËº∏?•È??ãÂ?Á®?, trigger: 'blur' }],
  Status: [{ required: true, message: 'Ë´ãÈÅ∏?áÁ???, trigger: 'change' }]
}
const currentTKey = ref(null)

// ?∏Ê?ËÆäÊõ¥
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// ?•Ë©¢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      TypeCode: queryForm.TypeCode || undefined,
      TypeName: queryForm.TypeName || undefined,
      Category: queryForm.Category || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await typeCodeApi.getTypeCodes(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '?•Ë©¢Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('?•Ë©¢Â§±Ê?Ôº? + error.message)
  } finally {
    loading.value = false
  }
}

// ?çÁΩÆ
const handleReset = () => {
  queryForm.TypeCode = ''
  queryForm.TypeName = ''
  queryForm.Category = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// ?∞Â?
const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  Object.assign(formData, {
    TypeCode: '',
    TypeName: '',
    TypeNameEn: '',
    Category: '',
    Description: '',
    SortOrder: null,
    Status: 'A'
  })
  dialogVisible.value = true
}

// ?•Á?
const handleView = async (row) => {
  await handleEdit(row)
}

// ‰øÆÊîπ
const handleEdit = async (row) => {
  try {
    const response = await typeCodeApi.getTypeCode(row.TKey)
    if (response.data?.success) {
      isEdit.value = true
      currentTKey.value = row.TKey
      Object.assign(formData, response.data.data)
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '?•Ë©¢Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('?•Ë©¢Â§±Ê?Ôº? + error.message)
  }
}

// ?™Èô§
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§È°ûÂ?‰ª?¢º?éÔ?', 'Á¢∫Ë?', {
      type: 'warning'
    })
    const response = await typeCodeApi.deleteTypeCode(row.TKey)
    if (response.data?.success) {
      ElMessage.success('?™Èô§?êÂ?')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '?™Èô§Â§±Ê?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('?™Èô§Â§±Ê?Ôº? + error.message)
    }
  }
}

// ?πÊ¨°?™Èô§
const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('Ë´ãÈÅ∏?áË??™Èô§?ÑË???)
    return
  }
  try {
    await ElMessageBox.confirm(`Á¢∫Â?Ë¶ÅÂà™?§ÈÅ∏?ñÁ? ${selectedRows.value.length} Á≠ÜË??ôÂ?Ôºü`, 'Á¢∫Ë?', {
      type: 'warning'
    })
    const tKeys = selectedRows.value.map(row => row.TKey)
    const response = await typeCodeApi.batchDeleteTypeCodes(tKeys)
    if (response.data?.success) {
      ElMessage.success('?πÊ¨°?™Èô§?êÂ?')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '?πÊ¨°?™Èô§Â§±Ê?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('?πÊ¨°?™Èô§Â§±Ê?Ôº? + error.message)
    }
  }
}

// ?ê‰∫§
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          response = await typeCodeApi.updateTypeCode(currentTKey.value, formData)
        } else {
          response = await typeCodeApi.createTypeCode(formData)
        }
        if (response.data?.success) {
          ElMessage.success(isEdit.value ? '‰øÆÊîπ?êÂ?' : '?∞Â??êÂ?')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '?ç‰?Â§±Ê?')
        }
      } catch (error) {
        ElMessage.error('?ç‰?Â§±Ê?Ôº? + error.message)
      }
    }
  })
}

// ?ÜÈ?Â§ßÂ?ËÆäÊõ¥
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// ?ÜÈ?ËÆäÊõ¥
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// ?ºÂ??ñÊó•?üÊ???
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// ?ùÂ???
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.type-code-data {
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


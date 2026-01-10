<template>
  <div class="data-maintenance">
    <div class="page-header">
      <h1>資料維護UI組件 (IMS30系列)</h1>
    </div>

    <!-- 組件選擇 -->
    <el-card class="component-card" shadow="never">
      <el-form :inline="true" :model="componentForm" class="search-form">
        <el-form-item label="組件代碼">
          <el-select
            v-model="componentForm.ComponentCode"
            placeholder="請選擇組件代碼"
            filterable
            clearable
            @change="handleComponentChange"
          >
            <el-option
              v-for="item in componentOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="組件版本">
          <el-radio-group v-model="componentForm.ComponentVersion">
            <el-radio label="V1">V1</el-radio>
            <el-radio label="V2">V2</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleLoadComponent">載入組件</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料維護操作 -->
    <el-card v-if="currentComponent" class="operation-card" shadow="never">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>{{ currentComponent.ComponentName }} ({{ currentComponent.ComponentCode }})</span>
          <el-button-group>
            <el-button type="primary" @click="handleBrowse" :disabled="!currentComponent">
              瀏覽 (FB)
            </el-button>
            <el-button type="success" @click="handleInsert" :disabled="!currentComponent">
              新增 (FI)
            </el-button>
            <el-button type="warning" @click="handleUpdate" :disabled="!currentComponent || !selectedRow">
              修改 (FU)
            </el-button>
            <el-button type="danger" @click="handleDelete" :disabled="!currentComponent || !selectedRow">
              刪除 (FS)
            </el-button>
            <el-button type="info" @click="handleQuery" :disabled="!currentComponent">
              查詢 (FQ)
            </el-button>
            <el-button @click="handlePrint" :disabled="!currentComponent">
              列印 (PR)
            </el-button>
          </el-button-group>
        </div>
      </template>

      <!-- 查詢表單 -->
      <el-form
        v-if="showQueryForm"
        :inline="true"
        :model="queryForm"
        class="search-form"
        style="margin-bottom: 20px"
      >
        <el-form-item
          v-for="field in queryFields"
          :key="field.FieldName"
          :label="field.FieldLabel"
        >
          <el-input
            v-if="field.FieldType === 'Text'"
            v-model="queryForm[field.FieldName]"
            :placeholder="`請輸入${field.FieldLabel}`"
            clearable
          />
          <el-date-picker
            v-else-if="field.FieldType === 'Date'"
            v-model="queryForm[field.FieldName]"
            type="date"
            :placeholder="`請選擇${field.FieldLabel}`"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
          <el-select
            v-else-if="field.FieldType === 'Select'"
            v-model="queryForm[field.FieldName]"
            :placeholder="`請選擇${field.FieldLabel}`"
            clearable
          >
            <el-option
              v-for="option in field.Options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQueryExecute">查詢</el-button>
          <el-button @click="handleQueryReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 資料表格 -->
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        highlight-current-row
        @current-change="handleRowChange"
        style="width: 100%"
      >
        <el-table-column
          v-for="column in tableColumns"
          :key="column.prop"
          :prop="column.prop"
          :label="column.label"
          :width="column.width"
          :min-width="column.minWidth"
          :show-overflow-tooltip="column.showOverflowTooltip"
        />
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-if="showPagination"
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
      <el-form
        :model="formData"
        :rules="formRules"
        ref="formRef"
        label-width="120px"
      >
        <el-form-item
          v-for="field in formFields"
          :key="field.FieldName"
          :label="field.FieldLabel"
          :prop="field.FieldName"
        >
          <el-input
            v-if="field.FieldType === 'Text'"
            v-model="formData[field.FieldName]"
            :placeholder="`請輸入${field.FieldLabel}`"
            :disabled="field.ReadOnly"
          />
          <el-input
            v-else-if="field.FieldType === 'TextArea'"
            v-model="formData[field.FieldName]"
            type="textarea"
            :rows="4"
            :placeholder="`請輸入${field.FieldLabel}`"
            :disabled="field.ReadOnly"
          />
          <el-input-number
            v-else-if="field.FieldType === 'Number'"
            v-model="formData[field.FieldName]"
            :placeholder="`請輸入${field.FieldLabel}`"
            :disabled="field.ReadOnly"
          />
          <el-date-picker
            v-else-if="field.FieldType === 'Date'"
            v-model="formData[field.FieldName]"
            type="date"
            :placeholder="`請選擇${field.FieldLabel}`"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            :disabled="field.ReadOnly"
          />
          <el-select
            v-else-if="field.FieldType === 'Select'"
            v-model="formData[field.FieldName]"
            :placeholder="`請選擇${field.FieldLabel}`"
            :disabled="field.ReadOnly"
          >
            <el-option
              v-for="option in field.Options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
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
import { uiComponentApi } from '@/api/uiComponent'

// 組件表單
const componentForm = reactive({
  ComponentCode: '',
  ComponentVersion: 'V1'
})

// 組件選項
const componentOptions = ref([
  { label: 'IMS30_FB - 瀏覽功能', value: 'IMS30_FB' },
  { label: 'IMS30_FI - 新增功能', value: 'IMS30_FI' },
  { label: 'IMS30_FU - 修改功能', value: 'IMS30_FU' },
  { label: 'IMS30_FQ - 查詢功能', value: 'IMS30_FQ' },
  { label: 'IMS30_PR - 報表功能', value: 'IMS30_PR' },
  { label: 'IMS30_FS - 保存功能', value: 'IMS30_FS' }
])

// 當前組件
const currentComponent = ref(null)

// 查詢表單
const queryForm = reactive({})
const queryFields = ref([])
const showQueryForm = ref(false)

// 表格資料
const tableData = ref([])
const tableColumns = ref([])
const loading = ref(false)
const selectedRow = ref(null)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const showPagination = ref(false)

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({})
const formFields = ref([])
const formRules = reactive({})

// 載入組件
const handleLoadComponent = async () => {
  if (!componentForm.ComponentCode) {
    ElMessage.warning('請選擇組件代碼')
    return
  }
  loading.value = true
  try {
    const response = await uiComponentApi.getComponentByCode(
      componentForm.ComponentCode,
      componentForm.ComponentVersion
    )
    if (response.data?.success) {
      currentComponent.value = response.data.data
      // 根據組件配置初始化表單和表格
      if (currentComponent.value.ConfigJson) {
        const config = JSON.parse(currentComponent.value.ConfigJson)
        queryFields.value = config.QueryFields || []
        tableColumns.value = config.TableColumns || []
        formFields.value = config.FormFields || []
        // 初始化表單驗證規則
        formFields.value.forEach((field) => {
          if (field.Required) {
            formRules[field.FieldName] = [
              { required: true, message: `請輸入${field.FieldLabel}`, trigger: 'blur' }
            ]
          }
        })
      }
      ElMessage.success('組件載入成功')
    } else {
      ElMessage.error(response.data?.message || '載入組件失敗')
    }
  } catch (error) {
    ElMessage.error('載入組件失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 組件變更
const handleComponentChange = () => {
  currentComponent.value = null
  tableData.value = []
  queryForm = {}
}

// 瀏覽
const handleBrowse = async () => {
  showQueryForm.value = false
  showPagination.value = true
  await loadData('Browse')
}

// 新增
const handleInsert = () => {
  isEdit.value = false
  dialogTitle.value = '新增資料'
  formData.value = {}
  formFields.value.forEach((field) => {
    formData.value[field.FieldName] = field.DefaultValue || ''
  })
  dialogVisible.value = true
}

// 修改
const handleUpdate = () => {
  if (!selectedRow.value) {
    ElMessage.warning('請選擇要修改的資料')
    return
  }
  isEdit.value = true
  dialogTitle.value = '修改資料'
  formData.value = { ...selectedRow.value }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async () => {
  if (!selectedRow.value) {
    ElMessage.warning('請選擇要刪除的資料')
    return
  }
  try {
    await ElMessageBox.confirm('確定要刪除這筆資料嗎？', '提示', {
      type: 'warning'
    })
    const response = await uiComponentApi.deleteData(componentForm.ComponentCode, {
      Data: selectedRow.value
    })
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      await loadData('Browse')
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
    }
  }
}

// 查詢
const handleQuery = () => {
  showQueryForm.value = true
  showPagination.value = true
}

// 執行查詢
const handleQueryExecute = async () => {
  pagination.PageIndex = 1
  await loadData('Query')
}

// 重置查詢
const handleQueryReset = () => {
  queryFields.value.forEach((field) => {
    queryForm[field.FieldName] = ''
  })
  handleQueryExecute()
}

// 列印
const handlePrint = async () => {
  try {
    const response = await uiComponentApi.printData(componentForm.ComponentCode, {
      Data: selectedRow.value || tableData.value
    })
    if (response.data?.success) {
      ElMessage.success('列印成功')
    } else {
      ElMessage.error(response.data?.message || '列印失敗')
    }
  } catch (error) {
    ElMessage.error('列印失敗：' + (error.message || '未知錯誤'))
  }
}

// 載入資料
const loadData = async (operation) => {
  loading.value = true
  try {
    let response
    if (operation === 'Query') {
      response = await uiComponentApi.queryData(componentForm.ComponentCode, {
        ...queryForm,
        PageIndex: pagination.PageIndex,
        PageSize: pagination.PageSize
      })
    } else {
      response = await uiComponentApi.executeDataMaintenance(
        componentForm.ComponentCode,
        operation,
        {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
      )
    }
    if (response.data?.success) {
      if (response.data.data?.Items) {
        tableData.value = response.data.data.Items
        pagination.TotalCount = response.data.data.TotalCount || 0
      } else {
        tableData.value = response.data.data || []
      }
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 行變更
const handleRowChange = (row) => {
  selectedRow.value = row
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          response = await uiComponentApi.updateData(componentForm.ComponentCode, formData.value)
        } else {
          response = await uiComponentApi.insertData(componentForm.ComponentCode, formData.value)
        }
        if (response.data?.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          await loadData('Browse')
        } else {
          ElMessage.error(response.data?.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + (error.message || '未知錯誤'))
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  formData.value = {}
  formRef.value?.resetFields()
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData('Browse')
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData('Browse')
}

// 初始化
onMounted(() => {
  // 可以載入預設組件或組件列表
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.data-maintenance {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 500;
}

.component-card,
.operation-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}
</style>


<template>
  <div class="loyalty-init">
    <div class="page-header">
      <h1>忠誠度系統初始化 (WEBLOYALTYINI)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="設定類型">
          <el-select v-model="queryForm.ConfigType" placeholder="請選擇設定類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="參數設定" value="PARAM" />
            <el-option label="規則設定" value="RULE" />
            <el-option label="環境設定" value="ENV" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增設定</el-button>
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
        <el-table-column prop="ConfigId" label="設定ID" width="150" />
        <el-table-column prop="ConfigName" label="設定名稱" width="200" />
        <el-table-column prop="ConfigValue" label="設定值" width="200" />
        <el-table-column prop="ConfigType" label="設定類型" width="120">
          <template #default="{ row }">
            <el-tag :type="getConfigTypeTag(row.ConfigType)">
              {{ getConfigTypeText(row.ConfigType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Description" label="說明" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
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
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-form-item label="設定ID" prop="ConfigId">
          <el-input v-model="formData.ConfigId" :disabled="isEdit" placeholder="請輸入設定ID" />
        </el-form-item>
        <el-form-item label="設定名稱" prop="ConfigName">
          <el-input v-model="formData.ConfigName" placeholder="請輸入設定名稱" />
        </el-form-item>
        <el-form-item label="設定值" prop="ConfigValue">
          <el-input v-model="formData.ConfigValue" placeholder="請輸入設定值" />
        </el-form-item>
        <el-form-item label="設定類型" prop="ConfigType">
          <el-select v-model="formData.ConfigType" placeholder="請選擇設定類型" style="width: 100%">
            <el-option label="參數設定" value="PARAM" />
            <el-option label="規則設定" value="RULE" />
            <el-option label="環境設定" value="ENV" />
          </el-select>
        </el-form-item>
        <el-form-item label="說明">
          <el-input v-model="formData.Description" type="textarea" :rows="3" placeholder="請輸入說明" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 初始化對話框 -->
    <el-dialog
      v-model="initDialogVisible"
      title="執行系統初始化"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="initFormRef"
        :model="initForm"
        :rules="initFormRules"
        label-width="150px"
      >
        <el-form-item label="會員MID" prop="MemberMid">
          <el-input v-model="initForm.MemberMid" placeholder="請輸入會員MID" />
        </el-form-item>
        <el-form-item label="會員TID" prop="MemberTid">
          <el-input v-model="initForm.MemberTid" placeholder="請輸入會員TID" />
        </el-form-item>
        <el-form-item label="POS伺服器IP" prop="PosServerIp">
          <el-input v-model="initForm.PosServerIp" placeholder="請輸入POS伺服器IP" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="initDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleInitialize" :loading="initLoading">執行初始化</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

// API
const loyaltyApi = {
  getConfigs: (params) => axios.get('/api/v1/loyalty-system/configs', { params }),
  getConfig: (configId) => axios.get(`/api/v1/loyalty-system/configs/${configId}`),
  createConfig: (data) => axios.post('/api/v1/loyalty-system/configs', data),
  updateConfig: (configId, data) => axios.put(`/api/v1/loyalty-system/configs/${configId}`, data),
  deleteConfig: (configId) => axios.delete(`/api/v1/loyalty-system/configs/${configId}`),
  initialize: (data) => axios.post('/api/v1/loyalty-system/initialize', data)
}

// 查詢表單
const queryForm = reactive({
  ConfigType: '',
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
const dialogTitle = computed(() => isEdit.value ? '修改系統設定' : '新增系統設定')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  ConfigId: '',
  ConfigName: '',
  ConfigValue: '',
  ConfigType: 'PARAM',
  Description: '',
  Status: 'A'
})
const formRules = {
  ConfigId: [{ required: true, message: '請輸入設定ID', trigger: 'blur' }],
  ConfigName: [{ required: true, message: '請輸入設定名稱', trigger: 'blur' }],
  ConfigType: [{ required: true, message: '請選擇設定類型', trigger: 'change' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentConfigId = ref(null)

// 初始化對話框
const initDialogVisible = ref(false)
const initFormRef = ref(null)
const initLoading = ref(false)
const initForm = reactive({
  MemberMid: '',
  MemberTid: '',
  PosServerIp: ''
})
const initFormRules = {
  MemberMid: [{ required: true, message: '請輸入會員MID', trigger: 'blur' }],
  MemberTid: [{ required: true, message: '請輸入會員TID', trigger: 'blur' }],
  PosServerIp: [{ required: true, message: '請輸入POS伺服器IP', trigger: 'blur' }]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ConfigType: queryForm.ConfigType || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await loyaltyApi.getConfigs(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ConfigType = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentConfigId.value = null
  Object.assign(formData, {
    ConfigId: '',
    ConfigName: '',
    ConfigValue: '',
    ConfigType: 'PARAM',
    Description: '',
    Status: 'A'
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  try {
    const response = await loyaltyApi.getConfig(row.ConfigId)
    if (response.data?.success) {
      isEdit.value = true
      currentConfigId.value = row.ConfigId
      Object.assign(formData, response.data.data)
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此系統設定嗎？', '確認', {
      type: 'warning'
    })
    const response = await loyaltyApi.deleteConfig(row.ConfigId)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
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
          response = await loyaltyApi.updateConfig(currentConfigId.value, formData)
        } else {
          response = await loyaltyApi.createConfig(formData)
        }
        if (response.data?.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
}

// 執行初始化
const handleInitialize = async () => {
  if (!initFormRef.value) return
  await initFormRef.value.validate(async (valid) => {
    if (valid) {
      initLoading.value = true
      try {
        const response = await loyaltyApi.initialize(initForm)
        if (response.data?.success) {
          ElMessage.success('初始化成功')
          initDialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '初始化失敗')
        }
      } catch (error) {
        ElMessage.error('初始化失敗：' + error.message)
      } finally {
        initLoading.value = false
      }
    }
  })
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

// 取得設定類型標籤
const getConfigTypeTag = (type) => {
  const tags = {
    'PARAM': 'primary',
    'RULE': 'success',
    'ENV': 'warning'
  }
  return tags[type] || 'info'
}

// 取得設定類型文字
const getConfigTypeText = (type) => {
  const texts = {
    'PARAM': '參數設定',
    'RULE': '規則設定',
    'ENV': '環境設定'
  }
  return texts[type] || type
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.loyalty-init {
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


<template>
  <el-dialog
    v-model="visible"
    title="選擇城市"
    width="800px"
    @close="handleClose"
  >
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="城市名稱">
        <el-input v-model="queryForm.CityName" placeholder="請輸入城市名稱" clearable />
      </el-form-item>
      <el-form-item label="國家代碼">
        <el-input v-model="queryForm.CountryCode" placeholder="請輸入國家代碼" clearable />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
    
    <el-table
      :data="tableData"
      v-loading="loading"
      @selection-change="handleSelectionChange"
      @row-click="handleRowClick"
      highlight-current-row
      border
      stripe
    >
      <el-table-column v-if="multiple" type="selection" width="55" />
      <el-table-column type="index" label="序號" width="80" />
      <el-table-column prop="CityId" label="城市代碼" width="150" />
      <el-table-column prop="CityName" label="城市名稱" min-width="200" />
      <el-table-column prop="CountryCode" label="國家代碼" width="100" />
      <el-table-column prop="Status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.Status === '1' ? 'success' : 'danger'" size="small">
            {{ row.Status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    
    <!-- 分頁 -->
    <el-pagination
      v-model:current-page="pagination.PageIndex"
      v-model:page-size="pagination.PageSize"
      :total="pagination.TotalCount"
      :page-sizes="[20, 50, 100, 200]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
      style="margin-top: 20px; justify-content: flex-end"
    />
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleConfirm">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnFields: {
    type: String,
    default: 'CityId,CityName'
  },
  returnControl: {
    type: String,
    default: null
  },
  multiple: {
    type: Boolean,
    default: false
  },
  countryCode: {
    type: String,
    default: null
  },
  status: {
    type: String,
    default: '1'
  }
})

const emit = defineEmits(['update:modelValue', 'confirm'])

const visible = ref(props.modelValue)
const loading = ref(false)
const tableData = ref([])
const selectedRows = ref([])

const queryForm = ref({
  CityName: '',
  CountryCode: props.countryCode || ''
})

const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val) {
    loadData()
  }
})

watch(visible, (val) => {
  emit('update:modelValue', val)
})

watch(() => props.countryCode, (val) => {
  queryForm.value.CountryCode = val || ''
  if (visible.value) {
    loadData()
  }
})

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      CityName: queryForm.value.CityName || undefined,
      CountryCode: queryForm.value.CountryCode || props.countryCode || undefined,
      Status: props.status || '1',
      SortField: 'CityName',
      SortOrder: 'ASC'
    }
    const response = await dropdownListApi.getCities(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.Items || []
      pagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    console.error('載入城市列表失敗:', error)
    ElMessage.error('載入城市列表失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleQuery = () => {
  pagination.PageIndex = 1
  loadData()
}

const handleReset = () => {
  queryForm.value = {
    CityName: '',
    CountryCode: props.countryCode || ''
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

const handleRowClick = (row) => {
  if (props.multiple) {
    // 多選模式：切換選中狀態
    const index = selectedRows.value.findIndex(item => item.CityId === row.CityId)
    if (index > -1) {
      selectedRows.value.splice(index, 1)
    } else {
      selectedRows.value.push(row)
    }
  } else {
    // 單選模式：直接選中
    selectedRows.value = [row]
  }
}

const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData()
}

const handleConfirm = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇至少一個城市')
    return
  }
  
  const returnFields = props.returnFields.split(',')
  const result = selectedRows.value.map(row => {
    const item = {}
    returnFields.forEach(field => {
      const fieldName = field.trim()
      // 轉換為 PascalCase
      const pascalField = fieldName.charAt(0).toUpperCase() + fieldName.slice(1)
      item[fieldName] = row[pascalField] || row[fieldName] || ''
    })
    return item
  })
  
  emit('confirm', props.multiple ? result : result[0])
  handleClose()
}

const handleClose = () => {
  visible.value = false
  selectedRows.value = []
  queryForm.value = {
    CityName: '',
    CountryCode: props.countryCode || ''
  }
  pagination.PageIndex = 1
  pagination.TotalCount = 0
}
</script>

<style lang="scss" scoped>
.query-form {
  margin-bottom: 20px;
}
</style>

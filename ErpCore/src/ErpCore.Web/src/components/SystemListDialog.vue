<template>
  <el-dialog
    v-model="visible"
    title="選擇系統"
    width="800px"
    @close="handleClose"
  >
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="系統ID">
        <el-input v-model="queryForm.SystemId" placeholder="請輸入系統ID" clearable />
      </el-form-item>
      <el-form-item label="系統名稱">
        <el-input v-model="queryForm.SystemName" placeholder="請輸入系統名稱" clearable />
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
      <el-table-column prop="SystemId" label="系統ID" width="150" />
      <el-table-column prop="SystemName" label="系統名稱" min-width="200" />
      <el-table-column prop="Status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.Status === '1' || row.Status === 'A' ? 'success' : 'danger'" size="small">
            {{ row.Status === '1' || row.Status === 'A' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleConfirm">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnFields: {
    type: String,
    default: 'SystemId,SystemName'
  },
  returnControl: {
    type: String,
    default: null
  },
  multiple: {
    type: Boolean,
    default: false
  },
  status: {
    type: String,
    default: '1'
  },
  excludeSystems: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['update:modelValue', 'confirm'])

const visible = ref(props.modelValue)
const loading = ref(false)
const tableData = ref([])
const selectedRows = ref([])

const queryForm = ref({
  SystemId: '',
  SystemName: ''
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

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      SystemId: queryForm.value.SystemId || undefined,
      SystemName: queryForm.value.SystemName || undefined,
      Status: props.status || '1',
      ExcludeSystems: props.excludeSystems || undefined,
      SortField: 'SystemId',
      SortOrder: 'ASC'
    }
    const response = await dropdownListApi.getSystems(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    console.error('載入系統列表失敗:', error)
    ElMessage.error('載入系統列表失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleQuery = () => {
  loadData()
}

const handleReset = () => {
  queryForm.value = {
    SystemId: '',
    SystemName: ''
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

const handleRowClick = (row) => {
  if (props.multiple) {
    // 多選模式：切換選中狀態
    const index = selectedRows.value.findIndex(item => item.SystemId === row.SystemId)
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

const handleConfirm = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇至少一個系統')
    return
  }
  
  const returnFields = props.returnFields.split(',')
  const result = selectedRows.value.map(row => {
    const item = {}
    returnFields.forEach(field => {
      const fieldName = field.trim()
      // 轉換為 PascalCase (SystemId -> SystemId, SystemName -> SystemName)
      const pascalField = fieldName.charAt(0).toUpperCase() + fieldName.slice(1).toLowerCase()
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
    SystemId: '',
    SystemName: ''
  }
}
</script>

<style lang="scss" scoped>
.query-form {
  margin-bottom: 20px;
}
</style>

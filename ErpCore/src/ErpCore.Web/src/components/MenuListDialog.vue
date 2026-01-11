<template>
  <el-dialog
    v-model="visible"
    title="選擇選單"
    width="800px"
    @close="handleClose"
  >
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="選單名稱">
        <el-input v-model="queryForm.MenuName" placeholder="請輸入選單名稱" clearable />
      </el-form-item>
      <el-form-item label="系統ID">
        <el-select v-model="queryForm.SystemId" placeholder="請選擇" clearable style="width: 200px">
          <el-option
            v-for="item in systemOptions"
            :key="item.Value"
            :label="item.Label"
            :value="item.Value"
          />
        </el-select>
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
      <el-table-column prop="MenuId" label="選單ID" width="150" />
      <el-table-column prop="MenuName" label="選單名稱" min-width="200" />
      <el-table-column prop="SystemId" label="系統ID" width="120" />
      <el-table-column prop="Status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.Status === '1' ? 'success' : 'danger'" size="small">
            {{ row.Status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    
    <el-pagination
      v-model:current-page="pagination.PageIndex"
      v-model:page-size="pagination.PageSize"
      :total="pagination.TotalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
      style="margin-top: 20px"
    />
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleConfirm">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnFields: {
    type: String,
    default: 'MenuId,MenuName'
  },
  returnControl: {
    type: String,
    default: null
  },
  systemId: {
    type: String,
    default: null
  },
  multiple: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:modelValue', 'confirm'])

const visible = ref(props.modelValue)
const loading = ref(false)
const tableData = ref([])
const selectedRows = ref([])
const systemOptions = ref([])

const queryForm = ref({
  MenuName: '',
  SystemId: props.systemId || ''
})

const pagination = ref({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val) {
    loadData()
    loadSystemOptions()
  }
})

watch(visible, (val) => {
  emit('update:modelValue', val)
})

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.value.PageIndex,
      PageSize: pagination.value.PageSize,
      MenuName: queryForm.value.MenuName || undefined,
      SystemId: queryForm.value.SystemId || undefined,
      Status: '1'
    }
    const response = await dropdownListApi.getMenus(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.Items || []
      pagination.value.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    console.error('載入選單列表失敗:', error)
    ElMessage.error('載入選單列表失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const loadSystemOptions = async () => {
  try {
    const response = await dropdownListApi.getSystemOptions()
    if (response.data?.success) {
      systemOptions.value = response.data.data || []
    } else {
      console.error('載入系統選項失敗')
    }
  } catch (error) {
    console.error('載入系統選項失敗:', error)
  }
}

const handleQuery = () => {
  pagination.value.PageIndex = 1
  loadData()
}

const handleReset = () => {
  queryForm.value = {
    MenuName: '',
    SystemId: props.systemId || ''
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

const handleRowClick = (row) => {
  if (props.multiple) {
    // 多選模式：切換選中狀態
    const index = selectedRows.value.findIndex(item => item.MenuId === row.MenuId)
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
  pagination.value.PageSize = size
  loadData()
}

const handlePageChange = (page) => {
  pagination.value.PageIndex = page
  loadData()
}

const handleConfirm = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇至少一個選單')
    return
  }
  
  const returnFields = props.returnFields.split(',')
  const result = selectedRows.value.map(row => {
    const item = {}
    returnFields.forEach(field => {
      const fieldName = field.trim()
      // 轉換為 PascalCase (MenuId -> MenuId, MenuName -> MenuName)
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
    MenuName: '',
    SystemId: props.systemId || ''
  }
  pagination.value.PageIndex = 1
}
</script>

<style lang="scss" scoped>
.query-form {
  margin-bottom: 20px;
}
</style>

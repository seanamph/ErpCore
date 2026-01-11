<template>
  <el-dialog
    v-model="visible"
    title="多選區域列表"
    width="800px"
    @close="handleClose"
  >
    <div class="info-bar">
      <span>符合條件者共 {{ pagination.TotalCount }} 筆</span>
    </div>
    
    <div class="special-options">
      <el-button @click="handleSelectAll">全部區域</el-button>
      <el-button @click="handleSelectNone">不屬於任一區域</el-button>
    </div>
    
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="區域名稱">
        <el-input v-model="queryForm.AreaName" placeholder="請輸入區域名稱" clearable />
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
      <el-table-column prop="AreaId" label="區域編號" width="150" />
      <el-table-column prop="AreaName" label="區域名稱" min-width="200" />
      <el-table-column prop="Status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.Status === 'A' ? 'success' : 'danger'" size="small">
            {{ row.Status === 'A' ? '啟用' : '停用' }}
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
    default: 'AreaId,AreaName'
  },
  returnControl: {
    type: String,
    default: null
  },
  status: {
    type: String,
    default: 'A'
  },
  multiple: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['update:modelValue', 'confirm'])

const visible = ref(props.modelValue)
const loading = ref(false)
const tableData = ref([])
const selectedRows = ref([])

const queryForm = ref({
  AreaName: ''
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
  }
})

watch(visible, (val) => {
  emit('update:modelValue', val)
})

const allData = ref([])

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      AreaName: queryForm.value.AreaName || undefined,
      Status: props.status || 'A',
      SortField: 'AreaId',
      SortOrder: 'ASC'
    }
    const response = await dropdownListApi.getMultiAreas(params)
    if (response.data?.success) {
      allData.value = response.data.data || []
      updateTableData()
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    console.error('載入區域列表失敗:', error)
    ElMessage.error('載入區域列表失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const updateTableData = () => {
  // 計算分頁
  const startIndex = (pagination.value.PageIndex - 1) * pagination.value.PageSize
  const endIndex = startIndex + pagination.value.PageSize
  tableData.value = allData.value.slice(startIndex, endIndex)
  pagination.value.TotalCount = allData.value.length
}

const handleQuery = () => {
  pagination.value.PageIndex = 1
  loadData()
}

const handleReset = () => {
  queryForm.value = {
    AreaName: ''
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

const handleRowClick = (row) => {
  if (props.multiple) {
    // 多選模式：切換選中狀態
    const index = selectedRows.value.findIndex(item => item.AreaId === row.AreaId)
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

const handleSelectAll = () => {
  // 選擇所有區域（特殊代碼: 'xx'）
  const allArea = {
    AreaId: 'xx',
    AreaName: '全部區域',
    Status: 'A'
  }
  selectedRows.value = [allArea]
  ElMessage.success('已選擇全部區域')
}

const handleSelectNone = () => {
  // 選擇不屬於任一區域（特殊代碼: 'yy'）
  const noneArea = {
    AreaId: 'yy',
    AreaName: '不屬於任一區域',
    Status: 'A'
  }
  selectedRows.value = [noneArea]
  ElMessage.success('已選擇不屬於任一區域')
}

const handleSizeChange = (size) => {
  pagination.value.PageSize = size
  pagination.value.PageIndex = 1
  updateTableData()
}

const handlePageChange = (page) => {
  pagination.value.PageIndex = page
  updateTableData()
}

const handleConfirm = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇至少一個區域')
    return
  }
  
  const returnFields = props.returnFields.split(',')
  const result = selectedRows.value.map(row => {
    const item = {}
    returnFields.forEach(field => {
      const fieldName = field.trim()
      // 轉換為 PascalCase (AreaId -> AreaId, AreaName -> AreaName)
      const pascalField = fieldName.charAt(0).toUpperCase() + fieldName.slice(1).toLowerCase()
      item[fieldName] = row[pascalField] || row[fieldName] || ''
    })
    return item
  })
  
  // 如果指定了 returnControl，將結果回傳至父視窗的指定控制項
  if (props.returnControl) {
    const areaIds = result.map(item => item.AreaId || item.areaId).join(',')
    if (window.opener && window.opener.document) {
      const control = window.opener.document.getElementById(props.returnControl)
      if (control) {
        control.value = areaIds
        // 觸發 change 事件
        const event = new Event('change', { bubbles: true })
        control.dispatchEvent(event)
      }
    }
  }
  
  emit('confirm', props.multiple ? result : result[0])
  handleClose()
}

const handleClose = () => {
  visible.value = false
  selectedRows.value = []
  queryForm.value = {
    AreaName: ''
  }
  pagination.value.PageIndex = 1
  allData.value = []
  tableData.value = []
}
</script>

<style lang="scss" scoped>
.query-form {
  margin-bottom: 20px;
}

.info-bar {
  margin-bottom: 10px;
  font-size: 14px;
  color: #606266;
}

.special-options {
  margin-bottom: 20px;
  
  .el-button {
    margin-right: 10px;
  }
}
</style>

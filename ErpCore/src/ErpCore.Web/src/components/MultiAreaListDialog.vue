<template>
  <el-dialog
    v-model="visible"
    title="多選區域列表"
    width="800px"
    @close="handleClose"
  >
    <div class="info-bar">
      <span>符合條件者共 {{ totalCount }} 筆</span>
    </div>
    
    <div class="special-options">
      <el-button @click="handleSelectAll">全部區域</el-button>
      <el-button @click="handleSelectNone">不屬於任一區域</el-button>
    </div>
    
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="區域名稱">
        <el-input
          v-model="queryForm.AreaName"
          placeholder="請輸入區域名稱"
          clearable
          @input="handleSearch"
        />
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
      style="width: 100%"
    >
      <el-table-column type="index" label="序號" width="80" />
      <el-table-column prop="AreaId" label="區域編號" width="150" />
      <el-table-column prop="AreaName" label="區域名稱" min-width="200" />
      <el-table-column prop="Status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.Status === 'A' || row.Status === '1' ? 'success' : 'danger'" size="small">
            {{ row.Status === 'A' || row.Status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100">
        <template #default="{ row }">
          <el-checkbox
            :model-value="isSelected(row.AreaId)"
            @change="(val) => handleCheckboxChange(row, val)"
          />
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
      <el-button @click="handleClose">關閉</el-button>
      <el-button type="primary" @click="handleConfirm">確認選擇</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
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
const selectedAreaIds = ref(new Set())
const totalCount = ref(0)

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
      const items = response.data.data || []
      tableData.value = items
      totalCount.value = items.length
      pagination.value.TotalCount = items.length
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

const handleSearch = () => {
  // 防抖處理，可以根據需要添加
  pagination.value.PageIndex = 1
  loadData()
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
  // Element Plus 的 selection 功能，但我們使用自定義的選擇邏輯
}

const handleRowClick = (row) => {
  if (selectedAreaIds.value.has(row.AreaId)) {
    selectedAreaIds.value.delete(row.AreaId)
  } else {
    selectedAreaIds.value.add(row.AreaId)
  }
}

const handleCheckboxChange = (row, checked) => {
  if (checked) {
    selectedAreaIds.value.add(row.AreaId)
  } else {
    selectedAreaIds.value.delete(row.AreaId)
  }
}

const isSelected = (areaId) => {
  return selectedAreaIds.value.has(areaId)
}

const handleSelectAll = () => {
  // 選擇所有區域
  tableData.value.forEach(row => {
    selectedAreaIds.value.add(row.AreaId)
  })
  ElMessage.success('已選擇所有區域')
}

const handleSelectNone = () => {
  // 選擇「不屬於任一區域」選項（代碼: 'yy'）
  selectedAreaIds.value.clear()
  selectedAreaIds.value.add('yy')
  ElMessage.success('已選擇「不屬於任一區域」')
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
  if (selectedAreaIds.value.size === 0) {
    ElMessage.warning('請選擇至少一個區域')
    return
  }
  
  // 處理特殊選項
  const areaIds = Array.from(selectedAreaIds.value)
  
  // 如果選擇了「不屬於任一區域」，返回 'yy'
  if (areaIds.includes('yy')) {
    if (props.returnControl) {
      // 回傳至父視窗的指定控制項
      if (window.opener && window.opener.document) {
        const control = window.opener.document.getElementById(props.returnControl)
        if (control) {
          control.value = 'yy'
        }
      }
    }
    emit('confirm', 'yy')
    handleClose()
    return
  }
  
  // 如果選擇了「全部區域」，返回 'xx'
  if (areaIds.includes('xx')) {
    const allAreaIds = tableData.value.map(row => row.AreaId).join(',')
    if (props.returnControl) {
      if (window.opener && window.opener.document) {
        const control = window.opener.document.getElementById(props.returnControl)
        if (control) {
          control.value = allAreaIds
        }
      }
    }
    emit('confirm', allAreaIds)
    handleClose()
    return
  }
  
  // 處理正常選擇
  const returnFields = props.returnFields.split(',')
  const selectedRows = tableData.value.filter(row => selectedAreaIds.value.has(row.AreaId))
  
  if (props.multiple) {
    // 多選模式：返回區域代碼列表（逗號分隔）
    const areaIdList = selectedRows.map(row => row.AreaId).join(',')
    if (props.returnControl) {
      if (window.opener && window.opener.document) {
        const control = window.opener.document.getElementById(props.returnControl)
        if (control) {
          control.value = areaIdList
        }
      }
    }
    emit('confirm', areaIdList)
  } else {
    // 單選模式：返回第一個選擇的區域
    if (selectedRows.length > 0) {
      const row = selectedRows[0]
      const result = {}
      returnFields.forEach(field => {
        const fieldName = field.trim()
        const pascalField = fieldName.charAt(0).toUpperCase() + fieldName.slice(1).toLowerCase()
        result[fieldName] = row[pascalField] || row[fieldName] || ''
      })
      if (props.returnControl) {
        if (window.opener && window.opener.document) {
          const control = window.opener.document.getElementById(props.returnControl)
          if (control) {
            control.value = result[props.returnFields.split(',')[0].trim()]
          }
        }
      }
      emit('confirm', result)
    }
  }
  
  handleClose()
}

const handleClose = () => {
  visible.value = false
  selectedAreaIds.value.clear()
  queryForm.value = {
    AreaName: ''
  }
  pagination.value.PageIndex = 1
}
</script>

<style lang="scss" scoped>
.info-bar {
  margin-bottom: 10px;
  padding: 10px;
  background-color: #f5f7fa;
  border-radius: 4px;
  font-size: 14px;
}

.special-options {
  margin-bottom: 20px;
  display: flex;
  gap: 10px;
}

.query-form {
  margin-bottom: 20px;
}
</style>

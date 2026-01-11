<template>
  <el-dialog
    v-model="visible"
    title="多選使用者列表"
    width="1000px"
    @close="handleClose"
  >
    <div class="info-bar">
      <span>符合條件者共 {{ totalCount }} 筆</span>
    </div>
    
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="使用者編號">
        <el-input
          v-model="queryForm.UserId"
          placeholder="請輸入使用者編號"
          clearable
          style="width: 200px"
        />
      </el-form-item>
      
      <el-form-item label="使用者名稱">
        <el-input
          v-model="queryForm.UserName"
          placeholder="請輸入使用者名稱"
          clearable
          style="width: 200px"
        />
      </el-form-item>
      
      <el-form-item label="部門">
        <el-select
          v-model="queryForm.OrgId"
          placeholder="請選擇部門"
          clearable
          filterable
          style="width: 200px"
        >
          <el-option
            v-for="item in orgOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
    
    <!-- 已選使用者 -->
    <div class="selected-users" v-if="selectedUsers.length > 0">
      <div class="selected-header">
        <span>已選使用者 ({{ selectedUsers.length }})：</span>
        <el-button type="text" size="small" @click="handleClearSelected">清除全部</el-button>
      </div>
      <div class="selected-tags">
        <el-tag
          v-for="user in selectedUsers"
          :key="user.UserId"
          closable
          @close="handleRemoveSelected(user.UserId)"
          style="margin-right: 8px; margin-bottom: 8px;"
        >
          {{ user.UserName }} ({{ user.UserId }})
        </el-tag>
      </div>
    </div>
    
    <div class="toolbar">
      <div>
        <el-button @click="handleSelectAll">全部選取</el-button>
        <el-button @click="handleClearAll">全部取消</el-button>
      </div>
    </div>
    
    <el-table
      :data="tableData"
      v-loading="loading"
      @selection-change="handleSelectionChange"
      @row-click="handleRowClick"
      highlight-current-row
      border
      stripe
      style="width: 100%"
      ref="tableRef"
    >
      <el-table-column type="selection" width="55" />
      <el-table-column type="index" label="序號" width="80" />
      <el-table-column prop="UserId" label="使用者編號" width="120" />
      <el-table-column prop="UserName" label="使用者名稱" min-width="150" />
      <el-table-column prop="OrgName" label="部門" width="150" />
      <el-table-column prop="Title" label="職稱" width="120" />
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
      <el-button @click="handleClose">關閉</el-button>
      <el-button type="primary" @click="handleConfirm">確認選擇</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, onMounted, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'
import { departmentsApi } from '@/api/departments'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnFields: {
    type: String,
    default: 'UserId,UserName'
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
const selectedUsers = ref([])
const selectedUserIds = ref(new Set())
const totalCount = ref(0)
const tableRef = ref(null)

const queryForm = ref({
  UserId: '',
  UserName: '',
  OrgId: ''
})

const pagination = ref({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

const orgOptions = ref([])

watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val) {
    loadFilterOptions()
    loadData()
  }
})

watch(visible, (val) => {
  emit('update:modelValue', val)
})

onMounted(() => {
  if (visible.value) {
    loadFilterOptions()
    loadData()
  }
})

const loadFilterOptions = async () => {
  try {
    // 載入部門選項
    const deptResponse = await departmentsApi.getDepartments({ PageSize: 1000 })
    if (deptResponse.data?.success && deptResponse.data.data?.items) {
      orgOptions.value = deptResponse.data.data.items.map(dept => ({
        value: dept.DepartmentId,
        label: dept.DepartmentName || dept.DepartmentId
      }))
    } else if (deptResponse.data?.data) {
      // 如果返回格式不同，尝试其他格式
      const items = Array.isArray(deptResponse.data.data) 
        ? deptResponse.data.data 
        : deptResponse.data.data.items || []
      orgOptions.value = items.map(dept => ({
        value: dept.DepartmentId || dept.DeptId,
        label: dept.DepartmentName || dept.DeptName || dept.DepartmentId || dept.DeptId
      }))
    }
  } catch (error) {
    console.error('載入部門選項失敗:', error)
  }
}

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.value.PageIndex,
      PageSize: pagination.value.PageSize,
      UserId: queryForm.value.UserId || undefined,
      UserName: queryForm.value.UserName || undefined,
      OrgId: queryForm.value.OrgId || undefined,
      Status: props.status || 'A',
      SortField: 'UserId',
      SortOrder: 'ASC'
    }
    
    const response = await dropdownListApi.getMultiUsers(params)
    if (response.data?.success) {
      const result = response.data.data
      if (result && result.Items) {
        // 分頁結果格式
        tableData.value = result.Items || []
        totalCount.value = result.TotalCount || 0
        pagination.value.TotalCount = result.TotalCount || 0
      } else if (Array.isArray(result)) {
        // 陣列格式
        tableData.value = result
        totalCount.value = result.length
        pagination.value.TotalCount = result.length
      } else {
        tableData.value = []
        totalCount.value = 0
        pagination.value.TotalCount = 0
      }
      
      // 恢復選擇狀態
      await nextTick()
      restoreSelection()
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
      tableData.value = []
      totalCount.value = 0
      pagination.value.TotalCount = 0
    }
  } catch (error) {
    console.error('載入使用者列表失敗:', error)
    ElMessage.error('載入使用者列表失敗：' + (error.message || '未知錯誤'))
    tableData.value = []
    totalCount.value = 0
    pagination.value.TotalCount = 0
  } finally {
    loading.value = false
  }
}

const restoreSelection = () => {
  if (tableRef.value && selectedUserIds.value.size > 0) {
    tableData.value.forEach(row => {
      if (selectedUserIds.value.has(row.UserId)) {
        tableRef.value.toggleRowSelection(row, true)
      }
    })
  }
}

const handleQuery = () => {
  pagination.value.PageIndex = 1
  loadData()
}

const handleReset = () => {
  queryForm.value = {
    UserId: '',
    UserName: '',
    OrgId: ''
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  // 更新已選使用者列表
  selectedUsers.value = selection
  selectedUserIds.value = new Set(selection.map(u => u.UserId))
}

const handleRowClick = (row) => {
  // 切換選擇狀態
  if (tableRef.value) {
    const isSelected = selectedUserIds.value.has(row.UserId)
    tableRef.value.toggleRowSelection(row, !isSelected)
  }
}

const handleSelectAll = () => {
  // 選擇當前頁面的所有使用者
  if (tableRef.value) {
    tableData.value.forEach(row => {
      if (!selectedUserIds.value.has(row.UserId)) {
        tableRef.value.toggleRowSelection(row, true)
      }
    })
  }
  ElMessage.success('已選擇當前頁面的所有使用者')
}

const handleClearAll = () => {
  // 清除所有選擇
  if (tableRef.value) {
    tableRef.value.clearSelection()
  }
  selectedUsers.value = []
  selectedUserIds.value.clear()
  ElMessage.success('已清除所有選擇')
}

const handleRemoveSelected = (userId) => {
  // 移除單個已選使用者
  selectedUsers.value = selectedUsers.value.filter(u => u.UserId !== userId)
  selectedUserIds.value.delete(userId)
  
  // 更新表格選擇狀態
  if (tableRef.value) {
    const row = tableData.value.find(r => r.UserId === userId)
    if (row) {
      tableRef.value.toggleRowSelection(row, false)
    }
  }
}

const handleClearSelected = () => {
  handleClearAll()
}

const handleSizeChange = (size) => {
  pagination.value.PageSize = size
  pagination.value.PageIndex = 1
  loadData()
}

const handlePageChange = (page) => {
  pagination.value.PageIndex = page
  loadData()
}

const handleConfirm = () => {
  if (selectedUsers.value.length === 0) {
    ElMessage.warning('請選擇至少一個使用者')
    return
  }
  
  // 處理正常選擇
  const returnFields = props.returnFields.split(',')
  
  if (props.multiple) {
    // 多選模式：返回使用者編號列表（逗號分隔）
    const userIdList = selectedUsers.value.map(user => user.UserId).join(',')
    if (props.returnControl) {
      if (window.opener && window.opener.document) {
        const control = window.opener.document.getElementById(props.returnControl)
        if (control) {
          control.value = userIdList
        }
      }
    }
    emit('confirm', userIdList)
  } else {
    // 單選模式：返回第一個選擇的使用者
    if (selectedUsers.value.length > 0) {
      const user = selectedUsers.value[0]
      const result = {}
      returnFields.forEach(field => {
        const fieldName = field.trim()
        const pascalField = fieldName.charAt(0).toUpperCase() + fieldName.slice(1).toLowerCase()
        result[fieldName] = user[pascalField] || user[fieldName] || ''
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
  selectedUsers.value = []
  selectedUserIds.value.clear()
  queryForm.value = {
    UserId: '',
    UserName: '',
    OrgId: ''
  }
  pagination.value.PageIndex = 1
  if (tableRef.value) {
    tableRef.value.clearSelection()
  }
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

.toolbar {
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.query-form {
  margin-bottom: 20px;
}

.selected-users {
  margin-bottom: 20px;
  padding: 15px;
  background-color: #f9fafc;
  border-radius: 4px;
  border: 1px solid #e4e7ed;
}

.selected-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
  font-size: 14px;
  font-weight: 500;
  color: #606266;
}

.selected-tags {
  display: flex;
  flex-wrap: wrap;
}
</style>

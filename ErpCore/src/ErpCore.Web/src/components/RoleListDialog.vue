<template>
  <el-dialog
    v-model="visible"
    title="選擇角色列表"
    width="1000px"
    @close="handleClose"
  >
    <div class="info-bar">
      <span>符合條件者共 {{ totalCount }} 筆</span>
    </div>
    
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="角色代碼">
        <el-input
          v-model="queryForm.RoleId"
          placeholder="請輸入角色代碼"
          clearable
          style="width: 200px"
        />
      </el-form-item>
      
      <el-form-item label="角色名稱">
        <el-input
          v-model="queryForm.RoleName"
          placeholder="請輸入角色名稱"
          clearable
          style="width: 200px"
        />
      </el-form-item>
      
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
    
    <!-- 已選角色 -->
    <div class="selected-roles" v-if="selectedRoles.length > 0">
      <div class="selected-header">
        <span>已選角色 ({{ selectedRoles.length }})：</span>
        <el-button type="text" size="small" @click="handleClearSelected">清除全部</el-button>
      </div>
      <div class="selected-tags">
        <el-tag
          v-for="role in selectedRoles"
          :key="role.RoleId"
          closable
          @close="handleRemoveSelected(role.RoleId)"
          style="margin-right: 8px; margin-bottom: 8px;"
        >
          {{ role.RoleName }} ({{ role.RoleId }})
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
      <el-table-column prop="RoleId" label="角色代碼" width="150" />
      <el-table-column prop="RoleName" label="角色名稱" min-width="200" />
      <el-table-column prop="RoleNote" label="角色說明" min-width="250" show-overflow-tooltip />
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
import { rolesApi } from '@/api/roles'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnFields: {
    type: String,
    default: 'RoleId,RoleName'
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
const selectedRoles = ref([])
const selectedRoleIds = ref(new Set())
const totalCount = ref(0)
const tableRef = ref(null)

const queryForm = ref({
  RoleId: '',
  RoleName: ''
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

onMounted(() => {
  if (visible.value) {
    loadData()
  }
})

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.value.PageIndex,
      PageSize: pagination.value.PageSize,
      RoleId: queryForm.value.RoleId || undefined,
      RoleName: queryForm.value.RoleName || undefined,
      Status: props.status || 'A',
      SortField: 'RoleId',
      SortOrder: 'ASC'
    }
    
    const response = await rolesApi.getRoles(params)
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
    console.error('載入角色列表失敗:', error)
    ElMessage.error('載入角色列表失敗：' + (error.message || '未知錯誤'))
    tableData.value = []
    totalCount.value = 0
    pagination.value.TotalCount = 0
  } finally {
    loading.value = false
  }
}

const restoreSelection = () => {
  if (tableRef.value && selectedRoleIds.value.size > 0) {
    tableData.value.forEach(row => {
      if (selectedRoleIds.value.has(row.RoleId)) {
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
    RoleId: '',
    RoleName: ''
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  // 更新已選角色列表
  selectedRoles.value = selection
  selectedRoleIds.value = new Set(selection.map(r => r.RoleId))
}

const handleRowClick = (row) => {
  // 切換選擇狀態
  if (tableRef.value) {
    const isSelected = selectedRoleIds.value.has(row.RoleId)
    tableRef.value.toggleRowSelection(row, !isSelected)
  }
}

const handleSelectAll = () => {
  // 選擇當前頁面的所有角色
  if (tableRef.value) {
    tableData.value.forEach(row => {
      if (!selectedRoleIds.value.has(row.RoleId)) {
        tableRef.value.toggleRowSelection(row, true)
      }
    })
  }
  ElMessage.success('已選擇當前頁面的所有角色')
}

const handleClearAll = () => {
  // 清除所有選擇
  if (tableRef.value) {
    tableRef.value.clearSelection()
  }
  selectedRoles.value = []
  selectedRoleIds.value.clear()
  ElMessage.success('已清除所有選擇')
}

const handleRemoveSelected = (roleId) => {
  // 移除單個已選角色
  selectedRoles.value = selectedRoles.value.filter(r => r.RoleId !== roleId)
  selectedRoleIds.value.delete(roleId)
  
  // 更新表格選擇狀態
  if (tableRef.value) {
    const row = tableData.value.find(r => r.RoleId === roleId)
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
  if (selectedRoles.value.length === 0) {
    ElMessage.warning('請選擇至少一個角色')
    return
  }
  
  // 處理正常選擇
  const returnFields = props.returnFields.split(',')
  
  if (props.multiple) {
    // 多選模式：返回角色列表
    emit('confirm', selectedRoles.value)
  } else {
    // 單選模式：返回第一個選擇的角色
    if (selectedRoles.value.length > 0) {
      const role = selectedRoles.value[0]
      const result = {}
      returnFields.forEach(field => {
        const fieldName = field.trim()
        result[fieldName] = role[fieldName] || ''
      })
      emit('confirm', [role])
    }
  }
  
  handleClose()
}

const handleClose = () => {
  visible.value = false
  selectedRoles.value = []
  selectedRoleIds.value.clear()
  queryForm.value = {
    RoleId: '',
    RoleName: ''
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

.selected-roles {
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

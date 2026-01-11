<template>
  <div class="sys0240-query">
    <div class="page-header">
      <h1>角色複製 (SYS0240)</h1>
    </div>

    <!-- 角色複製表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="copyForm" :rules="rules" ref="copyFormRef" label-width="150px">
        <el-form-item label="來源角色代碼" prop="SourceRoleId">
          <el-input 
            v-model="copyForm.SourceRoleId" 
            placeholder="請選擇來源角色"
            readonly
            style="width: 300px"
          >
            <template #append>
              <el-button @click="showRoleLOV('source')">選擇</el-button>
            </template>
          </el-input>
          <span v-if="sourceRoleName" class="role-name" style="margin-left: 10px; color: #198754;">
            {{ sourceRoleName }}
          </span>
        </el-form-item>
        
        <el-form-item label="目的角色代碼" prop="TargetRoleId">
          <el-input 
            v-model="copyForm.TargetRoleId" 
            placeholder="請選擇目的角色"
            readonly
            style="width: 300px"
          >
            <template #append>
              <el-button @click="showRoleLOV('target')">選擇</el-button>
            </template>
          </el-input>
          <span v-if="targetRoleName" class="role-name" style="margin-left: 10px; color: #198754;">
            {{ targetRoleName }}
          </span>
        </el-form-item>
        
        <el-form-item label="複製選項">
          <el-checkbox v-model="copyForm.CopyUsers">同時複製使用者分配</el-checkbox>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleValidate" :disabled="!canValidate">驗證</el-button>
          <el-button type="danger" @click="handleCopy" :loading="copying" :disabled="!canCopy">執行複製</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 驗證結果 -->
    <el-card class="result-card" shadow="never" v-if="validateResult">
      <h3>驗證結果</h3>
      <el-descriptions :column="2" border>
        <el-descriptions-item label="來源角色存在">
          <el-tag :type="validateResult.SourceRoleExists ? 'success' : 'danger'">
            {{ validateResult.SourceRoleExists ? '是' : '否' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="目的角色存在">
          <el-tag :type="validateResult.TargetRoleExists ? 'success' : 'danger'">
            {{ validateResult.TargetRoleExists ? '是' : '否' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="來源角色有權限">
          <el-tag :type="validateResult.SourceHasPermissions ? 'success' : 'danger'">
            {{ validateResult.SourceHasPermissions ? '是' : '否' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="是否相同角色">
          <el-tag :type="!validateResult.IsSameRole ? 'success' : 'danger'">
            {{ validateResult.IsSameRole ? '是' : '否' }}
          </el-tag>
        </el-descriptions-item>
      </el-descriptions>
    </el-card>

    <!-- 角色選擇對話框 -->
    <el-dialog
      :title="roleLOVTitle"
      v-model="roleLOVDialogVisible"
      width="800px"
      @close="handleRoleLOVDialogClose"
    >
      <el-form :inline="true" style="margin-bottom: 10px">
        <el-form-item label="搜尋">
          <el-input 
            v-model="roleSearchKeyword" 
            placeholder="請輸入角色代碼或名稱" 
            clearable 
            style="width: 300px"
            @keyup.enter="handleRoleSearch"
          />
          <el-button type="primary" @click="handleRoleSearch" style="margin-left: 10px">搜尋</el-button>
        </el-form-item>
      </el-form>

      <el-table
        :data="roleLOVData"
        v-loading="roleLOVLoading"
        border
        stripe
        style="width: 100%"
        @row-click="handleRoleLOVRowClick"
        highlight-current-row
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="RoleId" label="角色代碼" width="150" sortable />
        <el-table-column prop="RoleName" label="角色名稱" width="200" sortable />
        <el-table-column prop="RoleNote" label="角色說明" min-width="200" show-overflow-tooltip />
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="roleLOVPagination.PageIndex"
        v-model:page-size="roleLOVPagination.PageSize"
        :total="roleLOVPagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleRoleLOVSizeChange"
        @current-change="handleRoleLOVPageChange"
        style="margin-top: 20px; text-align: right"
      />

      <template #footer>
        <el-button @click="roleLOVDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleRoleLOVConfirm" :disabled="!selectedRoleLOV">確定</el-button>
      </template>
    </el-dialog>

    <!-- 確認複製對話框 -->
    <el-dialog
      v-model="confirmVisible"
      title="確認複製"
      width="500px"
    >
      <div class="confirm-content">
        <el-alert
          type="warning"
          :closable="false"
          show-icon
        >
          <template #title>
            <div>
              <p><strong>複製角色會將目的角色原有的權限和使用者設定清除！</strong></p>
              <p>來源角色：{{ sourceRoleName }}</p>
              <p>目的角色：{{ targetRoleName }}</p>
              <p v-if="copyForm.CopyUsers">將同時複製使用者分配</p>
            </div>
          </template>
        </el-alert>
      </div>
      <template #footer>
        <el-button @click="confirmVisible = false">取消</el-button>
        <el-button type="danger" @click="executeCopy">確認複製</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { rolesApi } from '@/api/roles'
import '@/assets/styles/variables.scss'

// 複製表單
const copyForm = reactive({
  SourceRoleId: '',
  TargetRoleId: '',
  CopyUsers: false
})

// 表單驗證規則
const rules = {
  SourceRoleId: [
    { required: true, message: '請選擇來源角色', trigger: 'blur' }
  ],
  TargetRoleId: [
    { required: true, message: '請選擇目的角色', trigger: 'blur' }
  ]
}

const copyFormRef = ref(null)
const sourceRoleName = ref('')
const targetRoleName = ref('')
const copying = ref(false)
const validateResult = ref(null)

// 是否可以驗證
const canValidate = computed(() => {
  return copyForm.SourceRoleId && copyForm.TargetRoleId
})

// 是否可以複製
const canCopy = computed(() => {
  return copyForm.SourceRoleId && copyForm.TargetRoleId && 
         validateResult.value &&
         validateResult.value.SourceRoleExists &&
         validateResult.value.TargetRoleExists &&
         validateResult.value.SourceHasPermissions &&
         !validateResult.value.IsSameRole
})

// 角色選擇對話框
const roleLOVDialogVisible = ref(false)
const roleLOVType = ref('source') // 'source' or 'target'
const roleLOVTitle = computed(() => {
  return roleLOVType.value === 'source' ? '選擇來源角色' : '選擇目的角色'
})
const roleLOVData = ref([])
const roleLOVLoading = ref(false)
const roleLOVPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const roleSearchKeyword = ref('')
const selectedRoleLOV = ref(null)

// 確認對話框
const confirmVisible = ref(false)

// 顯示角色選擇對話框
const showRoleLOV = (type) => {
  roleLOVType.value = type
  roleLOVDialogVisible.value = true
  roleSearchKeyword.value = ''
  roleLOVPagination.PageIndex = 1
  loadRoleLOV()
}

// 載入角色選項
const loadRoleLOV = async () => {
  roleLOVLoading.value = true
  try {
    const params = {
      PageIndex: roleLOVPagination.PageIndex,
      PageSize: roleLOVPagination.PageSize
    }
    if (roleSearchKeyword.value) {
      params.RoleId = roleSearchKeyword.value
      params.RoleName = roleSearchKeyword.value
    }
    const response = await rolesApi.getRoles(params)
    if (response.data.success) {
      roleLOVData.value = response.data.data.items || []
      roleLOVPagination.TotalCount = response.data.data.totalCount || response.data.data.TotalCount || 0
    } else {
      ElMessage.error(response.data.message || '載入角色列表失敗')
    }
  } catch (error) {
    ElMessage.error('載入角色列表失敗：' + (error.response?.data?.message || error.message))
  } finally {
    roleLOVLoading.value = false
  }
}

// 角色搜尋
const handleRoleSearch = () => {
  roleLOVPagination.PageIndex = 1
  loadRoleLOV()
}

// 角色選擇對話框關閉
const handleRoleLOVDialogClose = () => {
  selectedRoleLOV.value = null
}

// 角色選擇行點擊
const handleRoleLOVRowClick = (row) => {
  selectedRoleLOV.value = row
}

// 角色選擇確認
const handleRoleLOVConfirm = () => {
  if (!selectedRoleLOV.value) {
    ElMessage.warning('請選擇角色')
    return
  }
  
  if (roleLOVType.value === 'source') {
    copyForm.SourceRoleId = selectedRoleLOV.value.RoleId
    sourceRoleName.value = `${selectedRoleLOV.value.RoleId} - ${selectedRoleLOV.value.RoleName}`
  } else {
    copyForm.TargetRoleId = selectedRoleLOV.value.RoleId
    targetRoleName.value = `${selectedRoleLOV.value.RoleId} - ${selectedRoleLOV.value.RoleName}`
  }
  
  roleLOVDialogVisible.value = false
  validateResult.value = null
}

// 角色選擇分頁大小變更
const handleRoleLOVSizeChange = (size) => {
  roleLOVPagination.PageSize = size
  roleLOVPagination.PageIndex = 1
  loadRoleLOV()
}

// 角色選擇分頁變更
const handleRoleLOVPageChange = (page) => {
  roleLOVPagination.PageIndex = page
  loadRoleLOV()
}

// 驗證
const handleValidate = async () => {
  if (!copyForm.SourceRoleId || !copyForm.TargetRoleId) {
    ElMessage.warning('請選擇來源角色和目的角色')
    return
  }

  try {
    const response = await rolesApi.validateCopyRole({
      SourceRoleId: copyForm.SourceRoleId,
      TargetRoleId: copyForm.TargetRoleId
    })
    if (response.data.success) {
      validateResult.value = response.data.data
      if (response.data.valid) {
        ElMessage.success('驗證通過，可以複製')
      } else {
        ElMessage.warning('驗證未通過，請檢查角色設定')
      }
    } else {
      ElMessage.error(response.data.message || '驗證失敗')
    }
  } catch (error) {
    ElMessage.error('驗證失敗：' + (error.response?.data?.message || error.message))
  }
}

// 執行複製（顯示確認對話框）
const handleCopy = () => {
  if (!canCopy.value) {
    ElMessage.warning('請先驗證角色複製的可行性')
    return
  }
  confirmVisible.value = true
}

// 執行複製（實際執行）
const executeCopy = async () => {
  confirmVisible.value = false
  copying.value = true
  
  try {
    const response = await rolesApi.copyRoleToTarget({
      SourceRoleId: copyForm.SourceRoleId,
      TargetRoleId: copyForm.TargetRoleId,
      CopyUsers: copyForm.CopyUsers
    })
    
    if (response.data.success) {
      const result = response.data.data
      ElMessage.success(
        `複製成功！權限複製 ${result.PermissionsCopied} 筆，使用者複製 ${result.UsersCopied} 筆`
      )
      handleReset()
    } else {
      ElMessage.error(response.data.message || '複製失敗')
    }
  } catch (error) {
    ElMessage.error('複製失敗：' + (error.response?.data?.message || error.message))
  } finally {
    copying.value = false
  }
}

// 重置
const handleReset = () => {
  copyForm.SourceRoleId = ''
  copyForm.TargetRoleId = ''
  copyForm.CopyUsers = false
  sourceRoleName.value = ''
  targetRoleName.value = ''
  validateResult.value = null
  if (copyFormRef.value) {
    copyFormRef.value.resetFields()
  }
}

onMounted(() => {
  // 初始化
})
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.sys0240-query {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      color: $primary-color;
      font-size: 24px;
      font-weight: bold;
    }
  }

  .search-card {
    margin-bottom: 20px;

    .role-name {
      font-weight: bold;
    }
  }

  .result-card {
    margin-bottom: 20px;
    
    h3 {
      color: $primary-color;
      margin-bottom: 15px;
    }
  }

  .confirm-content {
    p {
      margin: 10px 0;
      line-height: 1.6;
    }
  }
}
</style>

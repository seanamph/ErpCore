<template>
  <div class="user-management">
    <div class="page-header">
      <h1>使用者管理</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- 使用者資料 -->
      <el-tab-pane label="使用者資料" name="profile">
        <el-card class="card" shadow="never">
          <el-form :model="profileForm" :rules="profileRules" ref="profileFormRef" label-width="120px">
            <el-form-item label="使用者帳號">
              <el-input v-model="profileForm.UserId" disabled />
            </el-form-item>
            <el-form-item label="使用者名稱" prop="UserName">
              <el-input v-model="profileForm.UserName" placeholder="請輸入使用者名稱" />
            </el-form-item>
            <el-form-item label="電子郵件" prop="Email">
              <el-input v-model="profileForm.Email" placeholder="請輸入電子郵件" />
            </el-form-item>
            <el-form-item label="電話" prop="Phone">
              <el-input v-model="profileForm.Phone" placeholder="請輸入電話" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUpdateProfile">更新資料</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>

      <!-- 修改密碼 -->
      <el-tab-pane label="修改密碼" name="password">
        <el-card class="card" shadow="never">
          <el-form :model="passwordForm" :rules="passwordRules" ref="passwordFormRef" label-width="120px">
            <el-form-item label="目前密碼" prop="OldPassword">
              <el-input v-model="passwordForm.OldPassword" type="password" placeholder="請輸入目前密碼" show-password />
            </el-form-item>
            <el-form-item label="新密碼" prop="NewPassword">
              <el-input v-model="passwordForm.NewPassword" type="password" placeholder="請輸入新密碼" show-password />
            </el-form-item>
            <el-form-item label="確認新密碼" prop="ConfirmPassword">
              <el-input v-model="passwordForm.ConfirmPassword" type="password" placeholder="請再次輸入新密碼" show-password />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleChangePassword">修改密碼</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>

      <!-- 重置所有使用者密碼 -->
      <el-tab-pane label="重置所有使用者密碼" name="reset">
        <el-card class="card" shadow="never">
          <el-alert
            title="警告"
            type="warning"
            :closable="false"
            show-icon
            style="margin-bottom: 20px"
          >
            <template #default>
              此操作將重置所有使用者的密碼，請謹慎操作！
            </template>
          </el-alert>
          <el-form :model="resetForm" :rules="resetRules" ref="resetFormRef" label-width="120px">
            <el-form-item label="新密碼" prop="NewPassword">
              <el-input v-model="resetForm.NewPassword" type="password" placeholder="請輸入新密碼" show-password />
            </el-form-item>
            <el-form-item label="確認新密碼" prop="ConfirmPassword">
              <el-input v-model="resetForm.ConfirmPassword" type="password" placeholder="請再次輸入新密碼" show-password />
            </el-form-item>
            <el-form-item>
              <el-button type="danger" @click="handleResetAllPasswords">重置所有密碼</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { coreApi } from '@/api/core'

// 標籤頁
const activeTab = ref('profile')

// 使用者資料表單
const profileForm = reactive({
  UserId: '',
  UserName: '',
  Email: '',
  Phone: ''
})

const profileFormRef = ref(null)
const profileRules = {
  UserName: [{ required: true, message: '請輸入使用者名稱', trigger: 'blur' }],
  Email: [{ type: 'email', message: '請輸入正確的電子郵件格式', trigger: 'blur' }]
}

// 密碼表單
const passwordForm = reactive({
  OldPassword: '',
  NewPassword: '',
  ConfirmPassword: ''
})

const passwordFormRef = ref(null)
const validateConfirmPassword = (rule, value, callback) => {
  if (value !== passwordForm.NewPassword) {
    callback(new Error('兩次輸入的密碼不一致'))
  } else {
    callback()
  }
}
const passwordRules = {
  OldPassword: [{ required: true, message: '請輸入目前密碼', trigger: 'blur' }],
  NewPassword: [{ required: true, message: '請輸入新密碼', trigger: 'blur' }, { min: 6, message: '密碼長度至少6個字元', trigger: 'blur' }],
  ConfirmPassword: [{ required: true, message: '請確認新密碼', trigger: 'blur' }, { validator: validateConfirmPassword, trigger: 'blur' }]
}

// 重置密碼表單
const resetForm = reactive({
  NewPassword: '',
  ConfirmPassword: ''
})

const resetFormRef = ref(null)
const validateResetConfirmPassword = (rule, value, callback) => {
  if (value !== resetForm.NewPassword) {
    callback(new Error('兩次輸入的密碼不一致'))
  } else {
    callback()
  }
}
const resetRules = {
  NewPassword: [{ required: true, message: '請輸入新密碼', trigger: 'blur' }, { min: 6, message: '密碼長度至少6個字元', trigger: 'blur' }],
  ConfirmPassword: [{ required: true, message: '請確認新密碼', trigger: 'blur' }, { validator: validateResetConfirmPassword, trigger: 'blur' }]
}

// 載入使用者資料
const loadUserProfile = async () => {
  try {
    const response = await coreApi.userManagement.getUserProfile()
    if (response.data.success) {
      Object.assign(profileForm, response.data.data)
    } else {
      ElMessage.error(response.data.message || '載入使用者資料失敗')
    }
  } catch (error) {
    ElMessage.error('載入使用者資料失敗：' + error.message)
  }
}

// 更新使用者資料
const handleUpdateProfile = async () => {
  if (!profileFormRef.value) return
  await profileFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await coreApi.userManagement.updateUserProfile(profileForm)
        if (response.data.success) {
          ElMessage.success('更新成功')
          loadUserProfile()
        } else {
          ElMessage.error(response.data.message || '更新失敗')
        }
      } catch (error) {
        ElMessage.error('更新失敗：' + error.message)
      }
    }
  })
}

// 修改密碼
const handleChangePassword = async () => {
  if (!passwordFormRef.value) return
  await passwordFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await coreApi.userManagement.changePassword({
          OldPassword: passwordForm.OldPassword,
          NewPassword: passwordForm.NewPassword
        })
        if (response.data.success) {
          ElMessage.success('密碼修改成功')
          passwordForm.OldPassword = ''
          passwordForm.NewPassword = ''
          passwordForm.ConfirmPassword = ''
          passwordFormRef.value.resetFields()
        } else {
          ElMessage.error(response.data.message || '密碼修改失敗')
        }
      } catch (error) {
        ElMessage.error('密碼修改失敗：' + error.message)
      }
    }
  })
}

// 重置所有使用者密碼
const handleResetAllPasswords = async () => {
  if (!resetFormRef.value) return
  await resetFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        await ElMessageBox.confirm('確定要重置所有使用者的密碼嗎？此操作不可恢復！', '警告', {
          type: 'warning',
          confirmButtonText: '確定',
          cancelButtonText: '取消'
        })
        const response = await coreApi.userManagement.resetAllPasswords({
          NewPassword: resetForm.NewPassword
        })
        if (response.data.success) {
          ElMessage.success('重置成功')
          resetForm.NewPassword = ''
          resetForm.ConfirmPassword = ''
          resetFormRef.value.resetFields()
        } else {
          ElMessage.error(response.data.message || '重置失敗')
        }
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('重置失敗：' + error.message)
        }
      }
    }
  })
}

// 初始化
onMounted(() => {
  loadUserProfile()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.user-management {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: bold;
  margin: 0;
}

.card {
  margin-top: 20px;
}
</style>


<template>
  <div class="sys0130-query">
    <div class="page-header">
      <h1>使用者帳戶原則管理 (SYS0130)</h1>
    </div>

    <!-- 帳戶原則管理表單 -->
    <el-card class="form-card" shadow="never">
      <template #header>
        <span>使用者帳戶原則管理</span>
      </template>
      <el-form 
        :model="form" 
        :rules="rules" 
        ref="formRef" 
        label-width="150px"
        style="max-width: 800px"
      >
        <el-form-item label="使用者編號" prop="userId">
          <el-input 
            v-model="form.userId" 
            placeholder="請輸入使用者編號" 
            :maxlength="50"
            clearable
            style="width: 100%"
          />
        </el-form-item>
        
        <el-form-item label="使用者密碼" prop="password">
          <el-input 
            v-model="form.password" 
            type="password" 
            placeholder="請輸入使用者密碼" 
            :maxlength="20"
            show-password
            clearable
            style="width: 100%"
          />
          <el-text type="info" size="small" style="margin-left: 10px;">
            用於驗證使用者身份
          </el-text>
        </el-form-item>
        
        <el-form-item label="新密碼" prop="newPassword">
          <el-input 
            v-model="form.newPassword" 
            type="password" 
            placeholder="請輸入新密碼（選填）" 
            :maxlength="20"
            show-password
            clearable
            style="width: 100%"
          />
          <el-text type="info" size="small" style="margin-left: 10px;">
            如不需要修改密碼，請留空
          </el-text>
        </el-form-item>
        
        <el-form-item label="帳號終止日" prop="endDate">
          <el-date-picker
            v-model="form.endDate"
            type="date"
            placeholder="請選擇帳號終止日"
            format="YYYY/MM/DD"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleSubmit" :loading="loading">
            修改
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 成功訊息對話框 -->
    <el-dialog
      v-model="successDialogVisible"
      title="修改成功"
      width="400px"
      :close-on-click-modal="false"
    >
      <div style="text-align: center; padding: 20px;">
        <el-icon :size="48" color="#67C23A" style="margin-bottom: 10px;">
          <Check />
        </el-icon>
        <p style="font-size: 16px; margin-top: 10px;">修改成功</p>
      </div>
      <template #footer>
        <el-button type="primary" @click="handleSuccessConfirm">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Check } from '@element-plus/icons-vue'
import { validateUser, updateAccountPolicy } from '@/api/users'

const formRef = ref()
const loading = ref(false)
const successDialogVisible = ref(false)

const form = reactive({
  userId: '',
  password: '',
  newPassword: '',
  endDate: null
})

const rules = {
  userId: [
    { required: true, message: '請輸入使用者編號', trigger: 'blur' },
    { max: 50, message: '使用者編號長度不能超過50個字元', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '請輸入使用者密碼', trigger: 'blur' },
    { max: 20, message: '密碼長度不能超過20個字元', trigger: 'blur' }
  ],
  newPassword: [
    { max: 20, message: '新密碼長度不能超過20個字元', trigger: 'blur' }
  ],
  endDate: [
    { required: true, message: '請選擇帳號終止日', trigger: 'change' }
  ]
}

// 提交表單
const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    
    loading.value = true
    
    try {
      // 先驗證使用者編號和密碼
      const validation = await validateUser({
        UserId: form.userId,
        Password: form.password
      })
      
      if (!validation.Data || !validation.Data.IsValid) {
        ElMessage.error(validation.Message || '驗證失敗')
        return
      }
      
      // 準備更新資料
      const updateData = {
        Password: form.password,
        EndDate: form.endDate
      }
      
      // 如果有新密碼，則加入新密碼
      if (form.newPassword && form.newPassword.trim() !== '') {
        updateData.NewPassword = form.newPassword
      }
      
      // 修改帳戶原則
      await updateAccountPolicy(form.userId, updateData)
      
      successDialogVisible.value = true
    } catch (error) {
      const errorMessage = error.response?.data?.Message || error.message || '修改失敗'
      ElMessage.error(errorMessage)
    } finally {
      loading.value = false
    }
  } catch (error) {
    if (error !== false) {
      ElMessage.error('表單驗證失敗')
    }
  }
}

// 重置表單
const handleReset = () => {
  formRef.value?.resetFields()
  form.userId = ''
  form.password = ''
  form.newPassword = ''
  form.endDate = null
}

// 成功確認
const handleSuccessConfirm = () => {
  successDialogVisible.value = false
  handleReset()
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0130-query {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      color: $primary-color;
      font-size: 24px;
      font-weight: 600;
    }
  }
  
  .form-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>

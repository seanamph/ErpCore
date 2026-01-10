<template>
  <div class="system-function">
    <div class="page-header">
      <h1>系統功能</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- 系統識別 -->
      <el-tab-pane label="系統識別" name="identity">
        <el-card class="card" shadow="never">
          <el-form :model="identityForm" ref="identityFormRef" label-width="120px">
            <el-form-item label="系統名稱" prop="SystemName">
              <el-input v-model="identityForm.SystemName" placeholder="請輸入系統名稱" />
            </el-form-item>
            <el-form-item label="系統版本" prop="SystemVersion">
              <el-input v-model="identityForm.SystemVersion" placeholder="請輸入系統版本" />
            </el-form-item>
            <el-form-item label="系統描述" prop="Description">
              <el-input
                v-model="identityForm.Description"
                type="textarea"
                :rows="4"
                placeholder="請輸入系統描述"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUpdateIdentity">更新系統識別</el-button>
              <el-button @click="handleInitializeSystem">初始化系統</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>

      <!-- 系統註冊 -->
      <el-tab-pane label="系統註冊" name="registration">
        <el-card class="card" shadow="never">
          <el-descriptions :column="2" border style="margin-bottom: 20px">
            <el-descriptions-item label="硬體資訊">{{ hardwareInfo.HardwareId }}</el-descriptions-item>
            <el-descriptions-item label="CPU序列號">{{ hardwareInfo.CpuSerial }}</el-descriptions-item>
            <el-descriptions-item label="主機板序列號">{{ hardwareInfo.MotherboardSerial }}</el-descriptions-item>
            <el-descriptions-item label="MAC地址">{{ hardwareInfo.MacAddress }}</el-descriptions-item>
          </el-descriptions>
          <el-form :model="registrationForm" ref="registrationFormRef" label-width="120px">
            <el-form-item label="註冊檔案">
              <el-upload
                :action="uploadUrl"
                :on-success="handleUploadSuccess"
                :before-upload="beforeUpload"
                :limit="1"
              >
                <el-button type="primary">上傳註冊檔案</el-button>
              </el-upload>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleGenerateRegistration">生成註冊檔案</el-button>
              <el-button @click="handleVerifyRegistration">驗證註冊</el-button>
              <el-button @click="handleWebRegister">Web註冊</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>

      <!-- 關於 -->
      <el-tab-pane label="關於" name="about">
        <el-card class="card" shadow="never">
          <el-descriptions :column="1" border>
            <el-descriptions-item label="系統名稱">{{ aboutInfo.SystemName }}</el-descriptions-item>
            <el-descriptions-item label="系統版本">{{ aboutInfo.SystemVersion }}</el-descriptions-item>
            <el-descriptions-item label="系統描述">{{ aboutInfo.Description }}</el-descriptions-item>
            <el-descriptions-item label="版權資訊">{{ aboutInfo.Copyright }}</el-descriptions-item>
            <el-descriptions-item label="開發公司">{{ aboutInfo.Company }}</el-descriptions-item>
            <el-descriptions-item label="聯絡方式">{{ aboutInfo.Contact }}</el-descriptions-item>
          </el-descriptions>
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
const activeTab = ref('identity')

// 系統識別表單
const identityForm = reactive({
  SystemName: '',
  SystemVersion: '',
  Description: ''
})
const identityFormRef = ref(null)

// 硬體資訊
const hardwareInfo = reactive({
  HardwareId: '',
  CpuSerial: '',
  MotherboardSerial: '',
  MacAddress: ''
})

// 註冊表單
const registrationForm = reactive({
  RegistrationFile: null
})
const registrationFormRef = ref(null)
const uploadUrl = ref('')

// 關於資訊
const aboutInfo = reactive({
  SystemName: '',
  SystemVersion: '',
  Description: '',
  Copyright: '',
  Company: '',
  Contact: ''
})

// 載入系統識別
const loadSystemIdentity = async () => {
  try {
    const response = await coreApi.systemFunction.getSystemIdentity()
    if (response.data.success) {
      Object.assign(identityForm, response.data.data)
    } else {
      ElMessage.error(response.data.message || '載入系統識別失敗')
    }
  } catch (error) {
    ElMessage.error('載入系統識別失敗：' + error.message)
  }
}

// 更新系統識別
const handleUpdateIdentity = async () => {
  if (!identityFormRef.value) return
  await identityFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await coreApi.systemFunction.updateSystemIdentity(identityForm)
        if (response.data.success) {
          ElMessage.success('更新成功')
          loadSystemIdentity()
        } else {
          ElMessage.error(response.data.message || '更新失敗')
        }
      } catch (error) {
        ElMessage.error('更新失敗：' + error.message)
      }
    }
  })
}

// 初始化系統
const handleInitializeSystem = async () => {
  try {
    await ElMessageBox.confirm('確定要初始化系統嗎？此操作不可恢復！', '警告', {
      type: 'warning',
      confirmButtonText: '確定',
      cancelButtonText: '取消'
    })
    const response = await coreApi.systemFunction.initializeSystem()
    if (response.data.success) {
      ElMessage.success('初始化成功')
      loadSystemIdentity()
    } else {
      ElMessage.error(response.data.message || '初始化失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('初始化失敗：' + error.message)
    }
  }
}

// 載入硬體資訊
const loadHardwareInfo = async () => {
  try {
    const response = await coreApi.systemFunction.getHardwareInfo()
    if (response.data.success) {
      Object.assign(hardwareInfo, response.data.data)
    } else {
      ElMessage.error(response.data.message || '載入硬體資訊失敗')
    }
  } catch (error) {
    ElMessage.error('載入硬體資訊失敗：' + error.message)
  }
}

// 生成註冊檔案
const handleGenerateRegistration = async () => {
  try {
    const response = await coreApi.systemFunction.generateRegistration({
      HardwareId: hardwareInfo.HardwareId
    })
    if (response.data.success) {
      ElMessage.success('生成成功')
    } else {
      ElMessage.error(response.data.message || '生成失敗')
    }
  } catch (error) {
    ElMessage.error('生成失敗：' + error.message)
  }
}

// 驗證註冊
const handleVerifyRegistration = async () => {
  try {
    const response = await coreApi.systemFunction.verifyRegistration({
      HardwareId: hardwareInfo.HardwareId
    })
    if (response.data.success) {
      ElMessage.success('驗證成功')
    } else {
      ElMessage.error(response.data.message || '驗證失敗')
    }
  } catch (error) {
    ElMessage.error('驗證失敗：' + error.message)
  }
}

// Web註冊
const handleWebRegister = async () => {
  try {
    const response = await coreApi.systemFunction.webRegister({
      HardwareId: hardwareInfo.HardwareId
    })
    if (response.data.success) {
      ElMessage.success('註冊成功')
    } else {
      ElMessage.error(response.data.message || '註冊失敗')
    }
  } catch (error) {
    ElMessage.error('註冊失敗：' + error.message)
  }
}

// 上傳前檢查
const beforeUpload = (file) => {
  const isReg = file.name.endsWith('.reg')
  if (!isReg) {
    ElMessage.error('只能上傳.reg檔案')
  }
  return isReg
}

// 上傳成功
const handleUploadSuccess = async (response) => {
  if (response.success) {
    ElMessage.success('上傳成功')
  } else {
    ElMessage.error(response.message || '上傳失敗')
  }
}

// 載入關於資訊
const loadAboutInfo = async () => {
  try {
    const response = await coreApi.systemFunction.getAboutInfo()
    if (response.data.success) {
      Object.assign(aboutInfo, response.data.data)
    } else {
      ElMessage.error(response.data.message || '載入關於資訊失敗')
    }
  } catch (error) {
    ElMessage.error('載入關於資訊失敗：' + error.message)
  }
}

// 初始化
onMounted(() => {
  loadSystemIdentity()
  loadHardwareInfo()
  loadAboutInfo()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.system-function {
  padding: 20px;
  
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .card {
    margin-top: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>


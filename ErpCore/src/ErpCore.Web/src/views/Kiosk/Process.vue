<template>
  <div class="kiosk-process">
    <div class="page-header">
      <h1>Kiosk資料處理作業</h1>
    </div>

    <!-- 處理表單 -->
    <el-card class="form-card" shadow="never">
      <el-form :model="processForm" :rules="rules" ref="formRef" label-width="150px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="功能代碼" prop="FunCmdId">
              <el-select 
                v-model="processForm.FunCmdId" 
                placeholder="請選擇功能代碼" 
                style="width: 100%"
                @change="handleFunctionCodeChange"
              >
                <el-option label="O2 - 會員點數查詢" value="O2" />
                <el-option label="A1 - 會員密碼驗證" value="A1" />
                <el-option label="C2 - 會員密碼變更" value="C2" />
                <el-option label="D4 - 快速開卡" value="D4" />
                <el-option label="D8 - 資料補登" value="D8" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Kiosk機號" prop="KioskId">
              <el-input 
                v-model="processForm.KioskId" 
                placeholder="請輸入Kiosk機號" 
                clearable
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="卡片編號" prop="LoyalSysCard">
              <el-input 
                v-model="processForm.LoyalSysCard" 
                placeholder="請輸入卡片編號" 
                clearable
                :disabled="!requiresCardNumber"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="卡片密碼" prop="SysCardPWD" v-if="requiresPassword">
              <el-input 
                v-model="processForm.SysCardPWD" 
                type="password"
                placeholder="請輸入卡片密碼" 
                clearable
                show-password
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20" v-if="requiresMemberInfo">
          <el-col :span="12">
            <el-form-item label="會員身分證號" prop="Pid">
              <el-input 
                v-model="processForm.Pid" 
                placeholder="請輸入會員身分證號" 
                clearable
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="會員姓名" prop="Pnm">
              <el-input 
                v-model="processForm.Pnm" 
                placeholder="請輸入會員姓名" 
                clearable
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="其他資料" v-if="showOtherData">
          <el-input
            v-model="otherDataText"
            type="textarea"
            :rows="4"
            placeholder='請輸入JSON格式資料，例如：{"key": "value"}'
            @blur="handleOtherDataBlur"
          />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" icon="Check" @click="handleSubmit">處理</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 處理結果 -->
    <el-card v-if="processResult" class="result-card" shadow="never">
      <template #header>
        <div class="card-header">
          <span>處理結果</span>
          <el-tag :type="processResult.ReturnCode === '0000' ? 'success' : 'danger'" size="large">
            {{ processResult.ReturnCode === '0000' ? '成功' : '失敗' }}
          </el-tag>
        </div>
      </template>
      
      <el-descriptions :column="2" border>
        <el-descriptions-item label="回應碼">
          <el-tag :type="processResult.ReturnCode === '0000' ? 'success' : 'danger'">
            {{ processResult.ReturnCode }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="交易編號">
          {{ processResult.TransactionId }}
        </el-descriptions-item>
        <el-descriptions-item label="回應訊息" :span="2">
          {{ processResult.ReturnMessage }}
        </el-descriptions-item>
        <el-descriptions-item label="回應資料" :span="2" v-if="processResult.ResponseData">
          <pre class="response-data">{{ JSON.stringify(processResult.ResponseData, null, 2) }}</pre>
        </el-descriptions-item>
      </el-descriptions>
    </el-card>

    <!-- 處理記錄 -->
    <el-card class="history-card" shadow="never">
      <template #header>
        <div class="card-header">
          <span>處理記錄</span>
          <el-button type="primary" link icon="Refresh" @click="loadHistory">重新載入</el-button>
        </div>
      </template>

      <el-table
        :data="historyData"
        v-loading="historyLoading"
        border
        stripe
        style="width: 100%"
        max-height="400"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="TransactionId" label="交易編號" width="150" />
        <el-table-column prop="KioskId" label="Kiosk機號" width="120" />
        <el-table-column prop="FunctionCode" label="功能代碼" width="100" align="center">
          <template #default="{ row }">
            <el-tag size="small">{{ row.FunctionCode }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CardNumber" label="卡片編號" width="150" />
        <el-table-column prop="MemberId" label="會員編號" width="120" />
        <el-table-column prop="TransactionDate" label="處理時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.TransactionDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'Success' ? 'success' : 'danger'" size="small">
              {{ row.Status === 'Success' ? '成功' : '失敗' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ReturnCode" label="回應碼" width="100" align="center" />
        <el-table-column prop="ErrorMessage" label="錯誤訊息" min-width="200" show-overflow-tooltip />
      </el-table>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { kioskApi } from '@/api/kiosk'

// 表單引用
const formRef = ref(null)

// 處理表單
const processForm = reactive({
  FunCmdId: '',
  KioskId: '',
  LoyalSysCard: '',
  SysCardPWD: '',
  Pid: '',
  Pnm: '',
  OtherData: null
})

// 其他資料文字
const otherDataText = ref('')

// 驗證規則
const rules = {
  FunCmdId: [
    { required: true, message: '請選擇功能代碼', trigger: 'change' }
  ],
  KioskId: [
    { required: true, message: '請輸入Kiosk機號', trigger: 'blur' }
  ],
  LoyalSysCard: [
    { required: true, message: '請輸入卡片編號', trigger: 'blur' }
  ],
  SysCardPWD: [
    { required: true, message: '請輸入卡片密碼', trigger: 'blur' }
  ],
  Pid: [
    { required: true, message: '請輸入會員身分證號', trigger: 'blur' }
  ],
  Pnm: [
    { required: true, message: '請輸入會員姓名', trigger: 'blur' }
  ]
}

// 處理結果
const processResult = ref(null)

// 處理記錄
const historyData = ref([])
const historyLoading = ref(false)

// 計算屬性
const requiresCardNumber = computed(() => {
  return ['O2', 'A1', 'C2'].includes(processForm.FunCmdId)
})

const requiresPassword = computed(() => {
  return ['A1', 'C2'].includes(processForm.FunCmdId)
})

const requiresMemberInfo = computed(() => {
  return ['D4', 'D8'].includes(processForm.FunCmdId)
})

const showOtherData = computed(() => {
  return processForm.FunCmdId === 'D8'
})

// 功能代碼變更
const handleFunctionCodeChange = () => {
  // 清除相關欄位
  if (!requiresCardNumber.value) {
    processForm.LoyalSysCard = ''
  }
  if (!requiresPassword.value) {
    processForm.SysCardPWD = ''
  }
  if (!requiresMemberInfo.value) {
    processForm.Pid = ''
    processForm.Pnm = ''
  }
  if (!showOtherData.value) {
    otherDataText.value = ''
    processForm.OtherData = null
  }
  
  // 清除驗證狀態
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

// 其他資料失去焦點
const handleOtherDataBlur = () => {
  if (otherDataText.value.trim()) {
    try {
      processForm.OtherData = JSON.parse(otherDataText.value)
    } catch (error) {
      ElMessage.warning('其他資料格式錯誤，請輸入有效的JSON格式')
      processForm.OtherData = null
    }
  } else {
    processForm.OtherData = null
  }
}

// 提交處理
const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (!valid) {
      return false
    }

    try {
      // 根據功能代碼動態驗證
      if (requiresCardNumber.value && !processForm.LoyalSysCard) {
        ElMessage.warning('請輸入卡片編號')
        return
      }
      if (requiresPassword.value && !processForm.SysCardPWD) {
        ElMessage.warning('請輸入卡片密碼')
        return
      }
      if (requiresMemberInfo.value) {
        if (!processForm.Pid) {
          ElMessage.warning('請輸入會員身分證號')
          return
        }
        if (!processForm.Pnm) {
          ElMessage.warning('請輸入會員姓名')
          return
        }
      }

      const data = {
        FunCmdId: processForm.FunCmdId,
        KioskId: processForm.KioskId,
        LoyalSysCard: processForm.LoyalSysCard || undefined,
        SysCardPWD: processForm.SysCardPWD || undefined,
        Pid: processForm.Pid || undefined,
        Pnm: processForm.Pnm || undefined,
        OtherData: processForm.OtherData || undefined
      }

      const response = await kioskApi.processRequest(data)

      if (response.data.success) {
        processResult.value = response.data.data
        ElMessage.success('處理成功')
        
        // 重新載入處理記錄
        loadHistory()
      } else {
        ElMessage.error(response.data.message || '處理失敗')
        processResult.value = {
          ReturnCode: '9999',
          ReturnMessage: response.data.message || '處理失敗',
          TransactionId: '',
          ResponseData: null
        }
      }
    } catch (error) {
      ElMessage.error('處理失敗：' + (error.message || '未知錯誤'))
      processResult.value = {
        ReturnCode: '9999',
        ReturnMessage: error.message || '未知錯誤',
        TransactionId: '',
        ResponseData: null
      }
    }
  })
}

// 重置
const handleReset = () => {
  processForm.FunCmdId = ''
  processForm.KioskId = ''
  processForm.LoyalSysCard = ''
  processForm.SysCardPWD = ''
  processForm.Pid = ''
  processForm.Pnm = ''
  processForm.OtherData = null
  otherDataText.value = ''
  processResult.value = null
  
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

// 載入處理記錄
const loadHistory = async () => {
  try {
    historyLoading.value = true
    
    // 查詢最近20筆記錄
    const params = {
      PageIndex: 1,
      PageSize: 20,
      SortField: 'TransactionDate',
      SortOrder: 'DESC',
      Filters: {
        KioskId: processForm.KioskId || undefined
      }
    }
    
    const response = await kioskApi.getTransactions(params)
    
    if (response.data.success) {
      historyData.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('載入處理記錄失敗：', error)
  } finally {
    historyLoading.value = false
  }
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return d.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 初始化
onMounted(() => {
  // 可以載入預設的處理記錄
  // loadHistory()
})
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.kiosk-process {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: var(--text-color-primary);
    }
  }

  .form-card {
    margin-bottom: 20px;
  }

  .result-card {
    margin-bottom: 20px;
    
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    
    .response-data {
      background-color: var(--bg-color-secondary);
      padding: 10px;
      border-radius: 4px;
      font-size: 12px;
      max-height: 200px;
      overflow: auto;
      margin: 0;
    }
  }

  .history-card {
    margin-bottom: 20px;
    
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
  }
}
</style>


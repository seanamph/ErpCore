<template>
  <div class="tools">
    <div class="page-header">
      <h1>工具功能</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- Excel匯出 -->
      <el-tab-pane label="Excel匯出" name="excel">
        <el-card class="card" shadow="never">
          <el-form :model="excelForm" ref="excelFormRef" label-width="120px">
            <el-form-item label="模組代碼" prop="ModuleCode">
              <el-input v-model="excelForm.ModuleCode" placeholder="請輸入模組代碼" />
            </el-form-item>
            <el-form-item label="匯出設定">
              <el-input
                v-model="excelForm.ExportConfig"
                type="textarea"
                :rows="4"
                placeholder="請輸入匯出設定（JSON格式）"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleExportExcel">執行匯出</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>

      <!-- 字串編碼 -->
      <el-tab-pane label="字串編碼" name="encode">
        <el-card class="card" shadow="never">
          <el-form :model="encodeForm" ref="encodeFormRef" label-width="120px">
            <el-form-item label="原始字串" prop="SourceString">
              <el-input
                v-model="encodeForm.SourceString"
                type="textarea"
                :rows="4"
                placeholder="請輸入要編碼的字串"
              />
            </el-form-item>
            <el-form-item label="編碼類型" prop="EncodeType">
              <el-select v-model="encodeForm.EncodeType" placeholder="請選擇編碼類型">
                <el-option label="Base64編碼" value="Base64" />
                <el-option label="Base64解碼" value="Base64Decode" />
                <el-option label="URL編碼" value="UrlEncode" />
                <el-option label="URL解碼" value="UrlDecode" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleEncode">編碼</el-button>
              <el-button @click="handleDecode">解碼</el-button>
            </el-form-item>
            <el-form-item label="結果">
              <el-input
                v-model="encodeResult"
                type="textarea"
                :rows="4"
                readonly
              />
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>

      <!-- ASP轉ASPX -->
      <el-tab-pane label="ASP轉ASPX" name="aspx">
        <el-card class="card" shadow="never">
          <el-form :model="aspxForm" ref="aspxFormRef" label-width="120px">
            <el-form-item label="原始程式碼" prop="SourceCode">
              <el-input
                v-model="aspxForm.SourceCode"
                type="textarea"
                :rows="10"
                placeholder="請輸入ASP程式碼"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleAspToAspx">轉換為ASPX</el-button>
              <el-button @click="handleAspxToAsp">轉換為ASP</el-button>
            </el-form-item>
            <el-form-item label="轉換結果">
              <el-input
                v-model="aspxResult"
                type="textarea"
                :rows="10"
                readonly
              />
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { coreApi } from '@/api/core'

// 標籤頁
const activeTab = ref('excel')

// Excel匯出表單
const excelForm = reactive({
  ModuleCode: '',
  ExportConfig: ''
})
const excelFormRef = ref(null)

// 字串編碼表單
const encodeForm = reactive({
  SourceString: '',
  EncodeType: 'Base64'
})
const encodeFormRef = ref(null)
const encodeResult = ref('')

// ASP轉ASPX表單
const aspxForm = reactive({
  SourceCode: ''
})
const aspxFormRef = ref(null)
const aspxResult = ref('')

// Excel匯出
const handleExportExcel = async () => {
  if (!excelFormRef.value) return
  await excelFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const data = {
          ModuleCode: excelForm.ModuleCode,
          ExportConfig: excelForm.ExportConfig ? JSON.parse(excelForm.ExportConfig) : {}
        }
        const response = await coreApi.tools.exportExcel(excelForm.ModuleCode, data)
        if (response.data.success) {
          ElMessage.success('匯出任務已提交')
        } else {
          ElMessage.error(response.data.message || '匯出失敗')
        }
      } catch (error) {
        ElMessage.error('匯出失敗：' + error.message)
      }
    }
  })
}

// 編碼
const handleEncode = async () => {
  if (!encodeFormRef.value) return
  await encodeFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await coreApi.tools.encodeString({
          SourceString: encodeForm.SourceString,
          EncodeType: encodeForm.EncodeType
        })
        if (response.data.success) {
          encodeResult.value = response.data.data.EncodedString || ''
        } else {
          ElMessage.error(response.data.message || '編碼失敗')
        }
      } catch (error) {
        ElMessage.error('編碼失敗：' + error.message)
      }
    }
  })
}

// 解碼
const handleDecode = async () => {
  if (!encodeFormRef.value) return
  await encodeFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await coreApi.tools.decodeString({
          EncodedString: encodeForm.SourceString,
          EncodeType: encodeForm.EncodeType
        })
        if (response.data.success) {
          encodeResult.value = response.data.data.DecodedString || ''
        } else {
          ElMessage.error(response.data.message || '解碼失敗')
        }
      } catch (error) {
        ElMessage.error('解碼失敗：' + error.message)
      }
    }
  })
}

// ASP轉ASPX
const handleAspToAspx = async () => {
  if (!aspxFormRef.value) return
  await aspxFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await coreApi.tools.aspToAspx({
          SourceCode: aspxForm.SourceCode
        })
        if (response.data.success) {
          aspxResult.value = response.data.data.ConvertedCode || ''
        } else {
          ElMessage.error(response.data.message || '轉換失敗')
        }
      } catch (error) {
        ElMessage.error('轉換失敗：' + error.message)
      }
    }
  })
}

// ASPX轉ASP
const handleAspxToAsp = async () => {
  if (!aspxFormRef.value) return
  await aspxFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const response = await coreApi.tools.aspxToAsp({
          SourceCode: aspxForm.SourceCode
        })
        if (response.data.success) {
          aspxResult.value = response.data.data.ConvertedCode || ''
        } else {
          ElMessage.error(response.data.message || '轉換失敗')
        }
      } catch (error) {
        ElMessage.error('轉換失敗：' + error.message)
      }
    }
  })
}

// 初始化
onMounted(() => {
  // 初始化邏輯
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.tools {
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


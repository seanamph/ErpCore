<template>
  <div class="barcode">
    <div class="page-header">
      <h1>條碼處理工具</h1>
    </div>

    <el-card class="tool-card" shadow="never">
      <el-tabs v-model="activeTab">
        <!-- 產生條碼 -->
        <el-tab-pane label="產生條碼" name="generate">
          <el-form :model="generateForm" label-width="120px" style="max-width: 600px">
            <el-form-item label="條碼類型">
              <el-select v-model="generateForm.BarcodeType" placeholder="請選擇條碼類型" style="width: 100%">
                <el-option label="Code128" value="Code128" />
                <el-option label="Code39" value="Code39" />
                <el-option label="EAN13" value="EAN13" />
                <el-option label="QR Code" value="QRCode" />
              </el-select>
            </el-form-item>
            <el-form-item label="條碼內容">
              <el-input v-model="generateForm.Content" placeholder="請輸入條碼內容" />
            </el-form-item>
            <el-form-item label="寬度">
              <el-input-number v-model="generateForm.Width" :min="100" :max="1000" :step="10" style="width: 100%" />
            </el-form-item>
            <el-form-item label="高度">
              <el-input-number v-model="generateForm.Height" :min="50" :max="500" :step="10" style="width: 100%" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleGenerate" :loading="generating">產生條碼</el-button>
              <el-button @click="handleResetGenerate">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 條碼預覽 -->
          <div v-if="barcodeImage" style="margin-top: 20px; text-align: center">
            <img :src="barcodeImage" alt="條碼" style="max-width: 100%" />
            <div style="margin-top: 10px">
              <el-button type="primary" @click="handleDownloadBarcode">下載條碼</el-button>
            </div>
          </div>
        </el-tab-pane>

        <!-- 讀取條碼 -->
        <el-tab-pane label="讀取條碼" name="read">
          <el-upload
            ref="uploadRef"
            :auto-upload="false"
            :on-change="handleFileChange"
            :file-list="fileList"
            drag
            accept="image/*"
          >
            <el-icon class="el-icon--upload"><upload-filled /></el-icon>
            <div class="el-upload__text">
              將圖片拖到此處，或<em>點擊上傳</em>
            </div>
            <template #tip>
              <div class="el-upload__tip">
                支援 JPG、PNG、BMP 格式圖片
              </div>
            </template>
          </el-upload>

          <div style="margin-top: 20px; text-align: center">
            <el-button type="primary" @click="handleRead" :loading="reading">讀取條碼</el-button>
            <el-button @click="handleClear">清空</el-button>
          </div>

          <!-- 讀取結果 -->
          <el-card v-if="readResult" style="margin-top: 20px">
            <template #header>
              <span>讀取結果</span>
            </template>
            <el-descriptions :column="1" border>
              <el-descriptions-item label="條碼內容">{{ readResult.Content }}</el-descriptions-item>
              <el-descriptions-item label="條碼類型">{{ readResult.BarcodeType }}</el-descriptions-item>
              <el-descriptions-item label="讀取時間">{{ formatDateTime(readResult.ReadTime) }}</el-descriptions-item>
            </el-descriptions>
          </el-card>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { UploadFilled } from '@element-plus/icons-vue'
import { toolsApi } from '@/api/tools'

const activeTab = ref('generate')

// 產生條碼表單
const generateForm = reactive({
  BarcodeType: 'Code128',
  Content: '',
  Width: 200,
  Height: 100
})

const generating = ref(false)
const barcodeImage = ref('')

// 讀取條碼
const uploadRef = ref(null)
const fileList = ref([])
const reading = ref(false)
const readResult = ref(null)

// 產生條碼
const handleGenerate = async () => {
  if (!generateForm.Content) {
    ElMessage.warning('請輸入條碼內容')
    return
  }

  try {
    generating.value = true
    const data = {
      BarcodeType: generateForm.BarcodeType,
      Content: generateForm.Content,
      Width: generateForm.Width,
      Height: generateForm.Height
    }
    
    const response = await toolsApi.generateBarcode(data)
    
    // 將 blob 轉換為 base64
    const reader = new FileReader()
    reader.onload = () => {
      barcodeImage.value = reader.result
    }
    reader.readAsDataURL(response.data)
    
    ElMessage.success('條碼產生成功')
  } catch (error) {
    ElMessage.error('產生條碼失敗：' + (error.message || '未知錯誤'))
  } finally {
    generating.value = false
  }
}

// 重置產生表單
const handleResetGenerate = () => {
  generateForm.BarcodeType = 'Code128'
  generateForm.Content = ''
  generateForm.Width = 200
  generateForm.Height = 100
  barcodeImage.value = ''
}

// 下載條碼
const handleDownloadBarcode = () => {
  if (!barcodeImage.value) return
  
  const link = document.createElement('a')
  link.href = barcodeImage.value
  link.download = `barcode_${new Date().getTime()}.png`
  link.click()
}

// 檔案變更
const handleFileChange = (file, files) => {
  fileList.value = files
}

// 讀取條碼
const handleRead = async () => {
  if (fileList.value.length === 0) {
    ElMessage.warning('請選擇要讀取的圖片')
    return
  }

  try {
    reading.value = true
    const file = fileList.value[0].raw
    const response = await toolsApi.readBarcode(file)
    
    if (response.data.success) {
      readResult.value = response.data.data
      ElMessage.success('讀取成功')
    } else {
      ElMessage.error(response.data.message || '讀取失敗')
    }
  } catch (error) {
    ElMessage.error('讀取條碼失敗：' + (error.message || '未知錯誤'))
  } finally {
    reading.value = false
  }
}

// 清空
const handleClear = () => {
  fileList.value = []
  readResult.value = null
  uploadRef.value?.clearFiles()
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return ''
  const date = new Date(dateTime)
  return date.toLocaleString('zh-TW')
}
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.barcode {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: var(--text-color-primary);
    }
  }

  .tool-card {
    margin-bottom: 20px;
  }
}
</style>


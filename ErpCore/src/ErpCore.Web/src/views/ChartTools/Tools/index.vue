<template>
  <div class="tools">
    <div class="page-header">
      <h1>工具功能</h1>
    </div>

    <!-- 工具選單 -->
    <el-card class="tools-card" shadow="never">
      <el-row :gutter="20">
        <el-col :span="8">
          <el-card shadow="hover" class="tool-item" @click="handleExportExcel">
            <div class="tool-icon">
              <el-icon :size="48"><Document /></el-icon>
            </div>
            <div class="tool-title">匯出Excel</div>
            <div class="tool-desc">將資料匯出為Excel格式</div>
          </el-card>
        </el-col>
        <el-col :span="8">
          <el-card shadow="hover" class="tool-item" @click="handleEncodeString">
            <div class="tool-icon">
              <el-icon :size="48"><Lock /></el-icon>
            </div>
            <div class="tool-title">字串編碼</div>
            <div class="tool-desc">對字串進行編碼處理</div>
          </el-card>
        </el-col>
        <el-col :span="8">
          <el-card shadow="hover" class="tool-item" @click="handleAspxToAsp">
            <div class="tool-icon">
              <el-icon :size="48"><DocumentCopy /></el-icon>
            </div>
            <div class="tool-title">ASPX轉ASP</div>
            <div class="tool-desc">將ASPX格式轉換為ASP格式</div>
          </el-card>
        </el-col>
      </el-row>
    </el-card>

    <!-- 匯出Excel對話框 -->
    <el-dialog
      v-model="exportDialogVisible"
      title="匯出Excel"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="exportFormRef"
        :model="exportForm"
        :rules="exportFormRules"
        label-width="120px"
      >
        <el-form-item label="資料來源" prop="DataSource">
          <el-select v-model="exportForm.DataSource" placeholder="請選擇資料來源" style="width: 100%">
            <el-option label="商品資料" value="Product" />
            <el-option label="客戶資料" value="Customer" />
            <el-option label="訂單資料" value="Order" />
          </el-select>
        </el-form-item>
        <el-form-item label="檔案名稱">
          <el-input v-model="exportForm.FileName" placeholder="請輸入檔案名稱" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="exportDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleExportSubmit" :loading="exportLoading">匯出</el-button>
      </template>
    </el-dialog>

    <!-- 字串編碼對話框 -->
    <el-dialog
      v-model="encodeDialogVisible"
      title="字串編碼"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="encodeFormRef"
        :model="encodeForm"
        :rules="encodeFormRules"
        label-width="120px"
      >
        <el-form-item label="原始字串" prop="OriginalString">
          <el-input v-model="encodeForm.OriginalString" type="textarea" :rows="5" placeholder="請輸入原始字串" />
        </el-form-item>
        <el-form-item label="編碼類型" prop="EncodeType">
          <el-select v-model="encodeForm.EncodeType" placeholder="請選擇編碼類型" style="width: 100%">
            <el-option label="Base64" value="Base64" />
            <el-option label="URL編碼" value="UrlEncode" />
            <el-option label="HTML編碼" value="HtmlEncode" />
          </el-select>
        </el-form-item>
        <el-form-item label="編碼結果" v-if="encodeResult">
          <el-input v-model="encodeResult" type="textarea" :rows="5" readonly />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="encodeDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handleEncodeSubmit" :loading="encodeLoading">編碼</el-button>
        <el-button type="success" @click="handleCopyResult" v-if="encodeResult">複製結果</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { Document, Lock, DocumentCopy } from '@element-plus/icons-vue'
import axios from '@/api/axios'

// API
const toolsApi = {
  exportExcel: (data) => axios.post('/api/v1/tools/export-excel', data),
  encodeString: (data) => axios.post('/api/v1/tools/encode-string', data),
  aspxToAsp: (data) => axios.post('/api/v1/tools/aspx-to-asp', data)
}

// 匯出Excel對話框
const exportDialogVisible = ref(false)
const exportFormRef = ref(null)
const exportLoading = ref(false)
const exportForm = reactive({
  DataSource: '',
  FileName: ''
})
const exportFormRules = {
  DataSource: [{ required: true, message: '請選擇資料來源', trigger: 'change' }]
}

// 字串編碼對話框
const encodeDialogVisible = ref(false)
const encodeFormRef = ref(null)
const encodeLoading = ref(false)
const encodeForm = reactive({
  OriginalString: '',
  EncodeType: 'Base64'
})
const encodeFormRules = {
  OriginalString: [{ required: true, message: '請輸入原始字串', trigger: 'blur' }],
  EncodeType: [{ required: true, message: '請選擇編碼類型', trigger: 'change' }]
}
const encodeResult = ref('')

// 匯出Excel
const handleExportExcel = () => {
  exportForm.DataSource = ''
  exportForm.FileName = ''
  exportDialogVisible.value = true
}

// 匯出Excel提交
const handleExportSubmit = async () => {
  if (!exportFormRef.value) return
  await exportFormRef.value.validate(async (valid) => {
    if (valid) {
      exportLoading.value = true
      try {
        const response = await toolsApi.exportExcel(exportForm)
        if (response.data?.success) {
          ElMessage.success('匯出成功')
          exportDialogVisible.value = false
        } else {
          ElMessage.error(response.data?.message || '匯出失敗')
        }
      } catch (error) {
        ElMessage.error('匯出失敗：' + error.message)
      } finally {
        exportLoading.value = false
      }
    }
  })
}

// 字串編碼
const handleEncodeString = () => {
  encodeForm.OriginalString = ''
  encodeForm.EncodeType = 'Base64'
  encodeResult.value = ''
  encodeDialogVisible.value = true
}

// 字串編碼提交
const handleEncodeSubmit = async () => {
  if (!encodeFormRef.value) return
  await encodeFormRef.value.validate(async (valid) => {
    if (valid) {
      encodeLoading.value = true
      try {
        const response = await toolsApi.encodeString(encodeForm)
        if (response.data?.success) {
          encodeResult.value = response.data.data?.EncodedString || ''
          ElMessage.success('編碼成功')
        } else {
          ElMessage.error(response.data?.message || '編碼失敗')
        }
      } catch (error) {
        ElMessage.error('編碼失敗：' + error.message)
      } finally {
        encodeLoading.value = false
      }
    }
  })
}

// 複製結果
const handleCopyResult = () => {
  navigator.clipboard.writeText(encodeResult.value).then(() => {
    ElMessage.success('複製成功')
  }).catch(() => {
    ElMessage.error('複製失敗')
  })
}

// ASPX轉ASP
const handleAspxToAsp = () => {
  ElMessage.info('ASPX轉ASP功能開發中')
}
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

  .tools-card {
    margin-bottom: 20px;

    .tool-item {
      cursor: pointer;
      text-align: center;
      transition: all 0.3s;

      &:hover {
        transform: translateY(-5px);
        box-shadow: 0 4px 12px rgba(25, 135, 84, 0.2);
      }

      .tool-icon {
        margin-bottom: 10px;
        color: $primary-color;
      }

      .tool-title {
        font-size: 18px;
        font-weight: bold;
        color: $primary-color;
        margin-bottom: 10px;
      }

      .tool-desc {
        font-size: 14px;
        color: $text-color-secondary;
      }
    }
  }
}
</style>


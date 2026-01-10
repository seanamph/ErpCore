<template>
  <div class="encode-data">
    <div class="page-header">
      <h1>資料編碼作業 (EncodeData)</h1>
    </div>

    <!-- 編碼/解碼操作 -->
    <el-card class="operation-card" shadow="never">
      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane label="編碼資料" name="encode">
          <el-form :model="encodeForm" :rules="encodeRules" ref="encodeFormRef" label-width="120px">
            <el-form-item label="原始資料" prop="Data">
              <el-input
                v-model="encodeForm.Data"
                type="textarea"
                :rows="6"
                placeholder="請輸入要編碼的資料"
              />
            </el-form-item>
            <el-form-item label="編碼類型">
              <el-select v-model="encodeForm.EncodeType" placeholder="請選擇編碼類型">
                <el-option label="Base64" value="Base64" />
                <el-option label="URL編碼" value="UrlEncode" />
                <el-option label="MD5" value="MD5" />
                <el-option label="SHA256" value="SHA256" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleEncode" :loading="encoding">
                編碼
              </el-button>
              <el-button @click="handleEncodeReset">重置</el-button>
            </el-form-item>
            <el-form-item label="編碼結果" v-if="encodeResult">
              <el-input
                v-model="encodeResult"
                type="textarea"
                :rows="6"
                readonly
              />
              <el-button
                type="text"
                @click="handleCopyEncodeResult"
                style="margin-top: 10px"
              >
                複製結果
              </el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="解碼資料" name="decode">
          <el-form :model="decodeForm" :rules="decodeRules" ref="decodeFormRef" label-width="120px">
            <el-form-item label="編碼資料" prop="Data">
              <el-input
                v-model="decodeForm.Data"
                type="textarea"
                :rows="6"
                placeholder="請輸入要解碼的資料"
              />
            </el-form-item>
            <el-form-item label="解碼類型">
              <el-select v-model="decodeForm.DecodeType" placeholder="請選擇解碼類型">
                <el-option label="Base64" value="Base64" />
                <el-option label="URL解碼" value="UrlDecode" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleDecode" :loading="decoding">
                解碼
              </el-button>
              <el-button @click="handleDecodeReset">重置</el-button>
            </el-form-item>
            <el-form-item label="解碼結果" v-if="decodeResult">
              <el-input
                v-model="decodeResult"
                type="textarea"
                :rows="6"
                readonly
              />
              <el-button
                type="text"
                @click="handleCopyDecodeResult"
                style="margin-top: 10px"
              >
                複製結果
              </el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-card>

    <!-- 編碼記錄 -->
    <el-card class="log-card" shadow="never">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>編碼記錄</span>
          <el-button type="text" @click="loadEncodeLogs" :loading="logLoading">
            重新整理
          </el-button>
        </div>
      </template>
      <el-table
        :data="encodeLogs"
        v-loading="logLoading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="EncodeType" label="編碼類型" width="120" />
        <el-table-column prop="OperationType" label="操作類型" width="120">
          <template #default="{ row }">
            <el-tag :type="row.OperationType === 'Encode' ? 'primary' : 'success'" size="small">
              {{ row.OperationType === 'Encode' ? '編碼' : '解碼' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="OriginalData" label="原始資料" min-width="200" show-overflow-tooltip />
        <el-table-column prop="ResultData" label="結果資料" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ row.CreatedAt ? new Date(row.CreatedAt).toLocaleString('zh-TW') : '' }}
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="logPagination.PageIndex"
        v-model:page-size="logPagination.PageSize"
        :total="logPagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleLogSizeChange"
        @current-change="handleLogPageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { communicationApi } from '@/api/communication'

// 當前標籤
const activeTab = ref('encode')

// 編碼表單
const encodeForm = reactive({
  Data: '',
  EncodeType: 'Base64'
})
const encodeFormRef = ref(null)
const encodeRules = {
  Data: [{ required: true, message: '請輸入要編碼的資料', trigger: 'blur' }]
}
const encoding = ref(false)
const encodeResult = ref('')

// 解碼表單
const decodeForm = reactive({
  Data: '',
  DecodeType: 'Base64'
})
const decodeFormRef = ref(null)
const decodeRules = {
  Data: [{ required: true, message: '請輸入要解碼的資料', trigger: 'blur' }]
}
const decoding = ref(false)
const decodeResult = ref('')

// 編碼記錄
const encodeLogs = ref([])
const logLoading = ref(false)
const logPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 編碼
const handleEncode = async () => {
  if (!encodeFormRef.value) return
  await encodeFormRef.value.validate(async (valid) => {
    if (valid) {
      encoding.value = true
      try {
        const response = await communicationApi.encodeData({
          Data: encodeForm.Data,
          EncodeType: encodeForm.EncodeType
        })
        if (response.data?.success) {
          encodeResult.value = response.data.data?.ResultData || ''
          ElMessage.success('編碼成功')
          await loadEncodeLogs()
        } else {
          ElMessage.error(response.data?.message || '編碼失敗')
        }
      } catch (error) {
        ElMessage.error('編碼失敗：' + (error.message || '未知錯誤'))
      } finally {
        encoding.value = false
      }
    }
  })
}

// 解碼
const handleDecode = async () => {
  if (!decodeFormRef.value) return
  await decodeFormRef.value.validate(async (valid) => {
    if (valid) {
      decoding.value = true
      try {
        const response = await communicationApi.decodeData({
          Data: decodeForm.Data,
          DecodeType: decodeForm.DecodeType
        })
        if (response.data?.success) {
          decodeResult.value = response.data.data?.ResultData || ''
          ElMessage.success('解碼成功')
          await loadEncodeLogs()
        } else {
          ElMessage.error(response.data?.message || '解碼失敗')
        }
      } catch (error) {
        ElMessage.error('解碼失敗：' + (error.message || '未知錯誤'))
      } finally {
        decoding.value = false
      }
    }
  })
}

// 編碼重置
const handleEncodeReset = () => {
  encodeForm.Data = ''
  encodeForm.EncodeType = 'Base64'
  encodeResult.value = ''
  encodeFormRef.value?.resetFields()
}

// 解碼重置
const handleDecodeReset = () => {
  decodeForm.Data = ''
  decodeForm.DecodeType = 'Base64'
  decodeResult.value = ''
  decodeFormRef.value?.resetFields()
}

// 複製編碼結果
const handleCopyEncodeResult = async () => {
  try {
    await navigator.clipboard.writeText(encodeResult.value)
    ElMessage.success('已複製到剪貼簿')
  } catch (error) {
    ElMessage.error('複製失敗')
  }
}

// 複製解碼結果
const handleCopyDecodeResult = async () => {
  try {
    await navigator.clipboard.writeText(decodeResult.value)
    ElMessage.success('已複製到剪貼簿')
  } catch (error) {
    ElMessage.error('複製失敗')
  }
}

// 載入編碼記錄
const loadEncodeLogs = async () => {
  logLoading.value = true
  try {
    const params = {
      PageIndex: logPagination.PageIndex,
      PageSize: logPagination.PageSize
    }
    const response = await communicationApi.getEncodeLogs(params)
    if (response.data?.success) {
      encodeLogs.value = response.data.data?.Items || []
      logPagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    logLoading.value = false
  }
}

// 記錄分頁大小變更
const handleLogSizeChange = (size) => {
  logPagination.PageSize = size
  logPagination.PageIndex = 1
  loadEncodeLogs()
}

// 記錄分頁變更
const handleLogPageChange = (page) => {
  logPagination.PageIndex = page
  loadEncodeLogs()
}

// 標籤切換
const handleTabChange = (tabName) => {
  // 切換標籤時可以執行一些操作
}

// 初始化
onMounted(() => {
  loadEncodeLogs()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.encode-data {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $primary-color;
      margin: 0;
    }
  }

  .operation-card,
  .log-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>


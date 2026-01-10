<template>
  <div class="html2pdf">
    <div class="page-header">
      <h1>HTML轉PDF工具</h1>
    </div>

    <el-card class="tool-card" shadow="never">
      <el-form :model="convertForm" label-width="120px" style="max-width: 800px">
        <el-form-item label="HTML內容">
          <el-input
            v-model="convertForm.HtmlContent"
            type="textarea"
            :rows="15"
            placeholder="請輸入或貼上 HTML 內容"
          />
        </el-form-item>
        <el-form-item label="頁面設定">
          <el-row :gutter="20">
            <el-col :span="8">
              <el-select v-model="convertForm.PageSize" placeholder="頁面大小" style="width: 100%">
                <el-option label="A4" value="A4" />
                <el-option label="Letter" value="Letter" />
                <el-option label="Legal" value="Legal" />
              </el-select>
            </el-col>
            <el-col :span="8">
              <el-select v-model="convertForm.Orientation" placeholder="方向" style="width: 100%">
                <el-option label="直向" value="Portrait" />
                <el-option label="橫向" value="Landscape" />
              </el-select>
            </el-col>
            <el-col :span="8">
              <el-input-number
                v-model="convertForm.Margin"
                :min="0"
                :max="50"
                :step="5"
                placeholder="邊距(mm)"
                style="width: 100%"
              />
            </el-col>
          </el-row>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleConvert" :loading="converting">轉換為PDF</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button @click="handlePreview">預覽HTML</el-button>
        </el-form-item>
      </el-form>

      <!-- 預覽對話框 -->
      <el-dialog v-model="previewVisible" title="HTML預覽" width="80%">
        <div v-html="convertForm.HtmlContent" style="padding: 20px; border: 1px solid #ddd"></div>
      </el-dialog>
    </el-card>

    <!-- 轉換記錄 -->
    <el-card class="table-card" shadow="never">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>轉換記錄</span>
          <el-button icon="Refresh" @click="handleRefresh">刷新</el-button>
        </div>
      </template>
      
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="FileName" label="檔案名稱" min-width="200" />
        <el-table-column prop="FileSize" label="檔案大小" width="120" align="right">
          <template #default="{ row }">
            {{ formatFileSize(row.FileSize) }}
          </template>
        </el-table-column>
        <el-table-column prop="PageSize" label="頁面大小" width="100" />
        <el-table-column prop="Orientation" label="方向" width="100" />
        <el-table-column prop="ConvertedAt" label="轉換時間" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.ConvertedAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="ConvertedBy" label="轉換者" width="120" />
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleDownload(row)">
              下載
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { toolsApi } from '@/api/tools'

// 轉換表單
const convertForm = reactive({
  HtmlContent: '',
  PageSize: 'A4',
  Orientation: 'Portrait',
  Margin: 10
})

const converting = ref(false)
const previewVisible = ref(false)

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 轉換為PDF
const handleConvert = async () => {
  if (!convertForm.HtmlContent) {
    ElMessage.warning('請輸入 HTML 內容')
    return
  }

  try {
    converting.value = true
    const data = {
      HtmlContent: convertForm.HtmlContent,
      PageSize: convertForm.PageSize,
      Orientation: convertForm.Orientation,
      Margin: convertForm.Margin
    }
    
    const response = await toolsApi.htmlToPdf(data)
    
    // 下載 PDF
    const blob = new Blob([response.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `document_${new Date().getTime()}.pdf`
    link.click()
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('轉換成功')
    handleRefresh()
  } catch (error) {
    ElMessage.error('轉換失敗：' + (error.message || '未知錯誤'))
  } finally {
    converting.value = false
  }
}

// 重置
const handleReset = () => {
  convertForm.HtmlContent = ''
  convertForm.PageSize = 'A4'
  convertForm.Orientation = 'Portrait'
  convertForm.Margin = 10
}

// 預覽
const handlePreview = () => {
  if (!convertForm.HtmlContent) {
    ElMessage.warning('請輸入 HTML 內容')
    return
  }
  previewVisible.value = true
}

// 刷新
const handleRefresh = async () => {
  try {
    loading.value = true
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SortField: 'ConvertedAt',
      SortOrder: 'DESC'
    }
    
    const response = await toolsApi.getPdfConversionLogs(params)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      pagination.TotalCount = response.data.data.totalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 下載
const handleDownload = (row) => {
  // 實作下載邏輯
  ElMessage.info('下載功能開發中')
}

// 分頁變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleRefresh()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleRefresh()
}

// 格式化檔案大小
const formatFileSize = (bytes) => {
  if (!bytes) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return ''
  const date = new Date(dateTime)
  return date.toLocaleString('zh-TW', {
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
  handleRefresh()
})
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.html2pdf {
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

  .table-card {
    margin-bottom: 20px;
  }
}
</style>


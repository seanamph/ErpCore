<template>
  <div class="sales-report-print">
    <div class="page-header">
      <h1>報表列印作業 (SYSG710-SYSG7I0)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="printForm" label-width="150px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="報表類型" prop="ReportType">
              <el-select v-model="printForm.ReportType" placeholder="請選擇" style="width: 100%">
                <el-option label="銷售明細報表" value="SALES_DETAIL" />
                <el-option label="銷售彙總報表" value="SALES_SUMMARY" />
                <el-option label="客戶銷售報表" value="CUSTOMER_SALES" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="報表模板" prop="TemplateId">
              <el-select v-model="printForm.TemplateId" placeholder="請選擇" style="width: 100%" @change="handleTemplateChange">
                <el-option
                  v-for="template in templates"
                  :key="template.TemplateId"
                  :label="template.TemplateName"
                  :value="template.TemplateId"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="開始日期" prop="StartDate">
              <el-date-picker
                v-model="printForm.StartDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結束日期" prop="EndDate">
              <el-date-picker
                v-model="printForm.EndDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="列印選項">
          <el-checkbox v-model="printForm.IncludeHeader">包含表頭</el-checkbox>
          <el-checkbox v-model="printForm.IncludeFooter">包含表尾</el-checkbox>
          <el-checkbox v-model="printForm.IncludeSummary">包含彙總</el-checkbox>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handlePreview">預覽</el-button>
          <el-button type="success" @click="handlePrint">列印</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 預覽區域 -->
    <el-card class="preview-card" shadow="never" v-if="previewData">
      <div v-html="previewData" class="preview-content"></div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { salesReportPrintApi } from '@/api/invoiceSales'

// 列印表單
const printForm = reactive({
  ReportType: 'SALES_DETAIL',
  TemplateId: '',
  StartDate: '',
  EndDate: '',
  IncludeHeader: true,
  IncludeFooter: true,
  IncludeSummary: true
})

// 報表模板
const templates = ref([])
const previewData = ref('')

// 載入報表模板
const loadTemplates = async () => {
  try {
    const response = await salesReportPrintApi.getTemplates({
      reportType: printForm.ReportType
    })
    if (response.data?.success) {
      templates.value = response.data.data || []
    }
  } catch (error) {
    ElMessage.error('載入報表模板失敗：' + error.message)
  }
}

// 模板變更
const handleTemplateChange = () => {
  // 模板變更時的處理
}

// 預覽
const handlePreview = async () => {
  try {
    const response = await salesReportPrintApi.previewReport(printForm)
    if (response.data?.success) {
      previewData.value = response.data.data
    } else {
      ElMessage.error(response.data?.message || '預覽失敗')
    }
  } catch (error) {
    ElMessage.error('預覽失敗：' + error.message)
  }
}

// 列印
const handlePrint = async () => {
  try {
    const response = await salesReportPrintApi.printReport(printForm)
    if (response.data?.success) {
      ElMessage.success('列印成功')
    } else {
      ElMessage.error(response.data?.message || '列印失敗')
    }
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
  }
}

// 重置
const handleReset = () => {
  printForm.ReportType = 'SALES_DETAIL'
  printForm.TemplateId = ''
  printForm.StartDate = ''
  printForm.EndDate = ''
  printForm.IncludeHeader = true
  printForm.IncludeFooter = true
  printForm.IncludeSummary = true
  previewData.value = ''
}

// 初始化
onMounted(() => {
  const currentDate = new Date()
  printForm.StartDate = `${currentDate.getFullYear()}-${String(currentDate.getMonth() + 1).padStart(2, '0')}-01`
  printForm.EndDate = `${currentDate.getFullYear()}-${String(currentDate.getMonth() + 1).padStart(2, '0')}-${String(currentDate.getDate()).padStart(2, '0')}`
  loadTemplates()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sales-report-print {
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

  .search-card {
    margin-bottom: 20px;
  }

  .preview-card {
    margin-bottom: 20px;
    
    .preview-content {
      padding: 20px;
      background: #fff;
    }
  }
}
</style>


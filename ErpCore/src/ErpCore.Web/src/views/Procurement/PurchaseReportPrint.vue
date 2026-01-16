<template>
  <div class="purchase-report-print">
    <div class="page-header">
      <h1>採購報表列印</h1>
    </div>

    <!-- 報表選擇和設定 -->
    <el-card class="settings-card" shadow="never">
      <el-form :model="printForm" label-width="140px" :rules="formRules" ref="formRef">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="報表類型" prop="ReportType">
              <el-select v-model="printForm.ReportType" placeholder="請選擇報表類型" @change="handleReportTypeChange" style="width: 100%">
                <el-option label="採購單報表" value="PO" />
                <el-option label="供應商報表" value="SU" />
                <el-option label="付款單報表" value="PY" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="報表代碼" prop="ReportCode">
              <el-select v-model="printForm.ReportCode" placeholder="請選擇報表代碼" style="width: 100%">
                <el-option 
                  v-for="report in reportList" 
                  :key="report.code" 
                  :label="report.name" 
                  :value="report.code" 
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="檔案格式" prop="FileFormat">
              <el-radio-group v-model="printForm.FileFormat">
                <el-radio label="PDF">PDF</el-radio>
                <el-radio label="Excel">Excel</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="頁面大小">
              <el-select v-model="printForm.PrintSettings.PageSize" style="width: 100%">
                <el-option label="A4" value="A4" />
                <el-option label="A3" value="A3" />
                <el-option label="Letter" value="Letter" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="方向">
              <el-radio-group v-model="printForm.PrintSettings.Orientation">
                <el-radio label="Portrait">直向</el-radio>
                <el-radio label="Landscape">橫向</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </el-card>

    <!-- 篩選條件 -->
    <el-card class="filter-card" shadow="never" v-if="printForm.ReportType">
      <template #header>
        <span>篩選條件</span>
      </template>
      <el-form :model="filterForm" label-width="140px">
        <el-row :gutter="20">
          <el-col :span="12" v-if="printForm.ReportType === 'PO'">
            <el-form-item label="採購單號">
              <el-input v-model="filterForm.OrderId" placeholder="請輸入採購單號" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供應商">
              <el-select v-model="filterForm.SupplierId" placeholder="請選擇供應商" filterable clearable style="width: 100%">
                <el-option 
                  v-for="supplier in supplierList" 
                  :key="supplier.SupplierId" 
                  :label="supplier.SupplierName" 
                  :value="supplier.SupplierId" 
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="開始日期">
              <el-date-picker 
                v-model="filterForm.StartDate" 
                type="date" 
                placeholder="請選擇開始日期" 
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結束日期">
              <el-date-picker 
                v-model="filterForm.EndDate" 
                type="date" 
                placeholder="請選擇結束日期" 
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </el-card>

    <!-- 操作按鈕 -->
    <div class="action-buttons">
      <el-button type="primary" @click="handlePreview" :loading="previewLoading">預覽</el-button>
      <el-button type="success" @click="handlePrint" :loading="printLoading">列印</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="info" @click="handleViewHistory">查看列印記錄</el-button>
    </div>

    <!-- 預覽區域 -->
    <el-card class="preview-card" shadow="never" v-if="previewUrl">
      <template #header>
        <span>報表預覽</span>
      </template>
      <div v-loading="previewLoading" class="preview-content">
        <iframe 
          v-if="previewUrl" 
          :src="previewUrl" 
          class="preview-iframe"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { procurementApi } from '@/api/procurement'
import { useRouter } from 'vue-router'

const router = useRouter()

const formRef = ref(null)
const previewLoading = ref(false)
const printLoading = ref(false)
const previewUrl = ref('')
const supplierList = ref([])
const reportList = ref([])

const printForm = reactive({
  ReportType: '',
  ReportCode: '',
  ReportName: '',
  FileFormat: 'PDF',
  PrintSettings: {
    PageSize: 'A4',
    Orientation: 'Portrait',
    Margins: {
      Top: 20,
      Bottom: 20,
      Left: 20,
      Right: 20
    }
  }
})

const filterForm = reactive({
  OrderId: '',
  SupplierId: '',
  StartDate: '',
  EndDate: ''
})

const formRules = {
  ReportType: [
    { required: true, message: '請選擇報表類型', trigger: 'change' }
  ],
  ReportCode: [
    { required: true, message: '請選擇報表代碼', trigger: 'change' }
  ],
  FileFormat: [
    { required: true, message: '請選擇檔案格式', trigger: 'change' }
  ]
}

// 報表代碼列表（根據報表類型動態載入）
const reportTypeMap = {
  PO: [
    { code: 'SYSP110', name: '採購單報表' },
    { code: 'SYSP120', name: '採購單明細報表' }
  ],
  SU: [
    { code: 'SYSP210', name: '供應商報表' },
    { code: 'SYSP225', name: '供應商明細報表' }
  ],
  PY: [
    { code: 'SYSP276', name: '付款單報表' },
    { code: 'SYSP280', name: '付款單明細報表' }
  ]
}

const handleReportTypeChange = () => {
  printForm.ReportCode = ''
  reportList.value = reportTypeMap[printForm.ReportType] || []
  if (reportList.value.length > 0) {
    printForm.ReportCode = reportList.value[0].code
    printForm.ReportName = reportList.value[0].name
  }
}

const handlePreview = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid) => {
    if (!valid) return

    previewLoading.value = true
    try {
      const data = {
        ReportType: printForm.ReportType,
        ReportCode: printForm.ReportCode,
        ReportName: printForm.ReportName,
        FilterConditions: { ...filterForm },
        PrintSettings: printForm.PrintSettings,
        FileFormat: printForm.FileFormat
      }

      const response = await procurementApi.previewPurchaseReportPrint(data)
      if (response.data.success) {
        // 將 Base64 資料轉換為預覽 URL
        previewUrl.value = `data:${response.data.data.ContentType};base64,${response.data.data.PreviewData}`
        ElMessage.success('預覽成功')
      } else {
        ElMessage.error(response.data.message || '預覽失敗')
      }
    } catch (error) {
      ElMessage.error('預覽失敗：' + (error.response?.data?.message || error.message))
    } finally {
      previewLoading.value = false
    }
  })
}

const handlePrint = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid) => {
    if (!valid) return

    printLoading.value = true
    try {
      const data = {
        ReportType: printForm.ReportType,
        ReportCode: printForm.ReportCode,
        ReportName: printForm.ReportName,
        FilterConditions: { ...filterForm },
        PrintSettings: printForm.PrintSettings,
        FileFormat: printForm.FileFormat
      }

      const response = await procurementApi.createPurchaseReportPrint(data)
      if (response.data.success) {
        ElMessage.success('報表列印任務已建立')
        // 可以選擇自動下載或跳轉到記錄頁面
        if (response.data.data.FilePath) {
          // 下載檔案
          const downloadResponse = await procurementApi.downloadPurchaseReportPrint(response.data.data.TKey)
          const blob = new Blob([downloadResponse.data], { type: 'application/pdf' })
          const url = window.URL.createObjectURL(blob)
          const link = document.createElement('a')
          link.href = url
          link.download = response.data.data.FileName || `report_${response.data.data.TKey}.pdf`
          link.click()
          window.URL.revokeObjectURL(url)
        }
      } else {
        ElMessage.error(response.data.message || '列印失敗')
      }
    } catch (error) {
      ElMessage.error('列印失敗：' + (error.response?.data?.message || error.message))
    } finally {
      printLoading.value = false
    }
  })
}

const handleReset = () => {
  printForm.ReportType = ''
  printForm.ReportCode = ''
  printForm.ReportName = ''
  printForm.FileFormat = 'PDF'
  printForm.PrintSettings = {
    PageSize: 'A4',
    Orientation: 'Portrait',
    Margins: {
      Top: 20,
      Bottom: 20,
      Left: 20,
      Right: 20
    }
  }
  filterForm.OrderId = ''
  filterForm.SupplierId = ''
  filterForm.StartDate = ''
  filterForm.EndDate = ''
  previewUrl.value = ''
  reportList.value = []
  formRef.value?.resetFields()
}

const handleViewHistory = () => {
  router.push('/procurement/report-print/history')
}

// 載入供應商列表
const loadSuppliers = async () => {
  try {
    const response = await procurementApi.getSuppliers({ PageIndex: 1, PageSize: 1000 })
    if (response.data.success) {
      supplierList.value = response.data.data.Items || []
    }
  } catch (error) {
    console.error('載入供應商列表失敗', error)
  }
}

onMounted(() => {
  loadSuppliers()
})
</script>

<style scoped>
.purchase-report-print {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 500;
}

.settings-card,
.filter-card,
.preview-card {
  margin-bottom: 20px;
}

.action-buttons {
  margin-bottom: 20px;
  text-align: right;
}

.action-buttons .el-button {
  margin-left: 10px;
}

.preview-content {
  min-height: 600px;
}

.preview-iframe {
  width: 100%;
  height: 600px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
}
</style>

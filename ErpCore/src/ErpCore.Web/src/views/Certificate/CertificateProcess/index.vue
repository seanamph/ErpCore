<template>
  <div class="certificate-process-management">
    <div class="page-header">
      <h1>?ëË??ïÁ?‰ΩúÊ•≠ (SYSK210-SYSK230)</h1>
    </div>

    <!-- ?üËÉΩ?âÈ? -->
    <el-card class="action-card" shadow="never">
      <el-button type="primary" @click="handleCheck">?ëË?Ê™¢Êü•</el-button>
      <el-button type="success" @click="handleImport">?ëË?Â∞éÂÖ•</el-button>
      <el-button type="warning" @click="handlePrint">?ëË??óÂç∞</el-button>
      <el-button type="info" @click="handleExport">?ëË??ØÂá∫</el-button>
      <el-button type="danger" @click="handleBatchUpdateStatus">?πÈ??¥Êñ∞?Ä??/el-button>
    </el-card>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ëË?Á∑®Ë?">
          <el-input v-model="queryForm.VoucherId" placeholder="Ë´ãËº∏?•Ê?Ë≠âÁ∑®?? clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- Ë≥áÊ?Ë°®Ê†º -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="VoucherId" label="?ëË?Á∑®Ë?" width="150" />
        <el-table-column prop="VoucherDate" label="?ëË??•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.VoucherDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="VoucherType" label="?ëË?È°ûÂ?" width="120">
          <template #default="{ row }">
            <el-tag :type="getVoucherTypeType(row.VoucherType)">
              {{ getVoucherTypeText(row.VoucherType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="?Ä?? width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="Á∏ΩÈ?È°? width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- ?ëË?Ê™¢Êü•Â∞çË©±Ê°?-->
    <el-dialog
      v-model="checkDialogVisible"
      title="?ëË?Ê™¢Êü•ÁµêÊ?"
      width="800px"
    >
      <el-table :data="checkResults" border>
        <el-table-column prop="VoucherId" label="?ëË?Á∑®Ë?" width="150" />
        <el-table-column prop="IsValid" label="?ØÂê¶?âÊ?" width="120">
          <template #default="{ row }">
            <el-tag :type="row.IsValid ? 'success' : 'danger'">
              {{ row.IsValid ? '?âÊ?' : '?°Ê?' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Message" label="Ê™¢Êü•Ë®äÊÅØ" min-width="300" />
      </el-table>
    </el-dialog>

    <!-- Â∞éÂÖ•Â∞çË©±Ê°?-->
    <el-dialog
      v-model="importDialogVisible"
      title="?ëË?Â∞éÂÖ•"
      width="500px"
    >
      <el-upload
        ref="uploadRef"
        :auto-upload="false"
        :on-change="handleFileChange"
        :file-list="fileList"
        drag
      >
        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
        <div class="el-upload__text">
          Â∞áÊ?Ê°àÊ??∞Ê≠§?ïÔ???em>ÈªûÊ?‰∏äÂÇ≥</em>
        </div>
        <template #tip>
          <div class="el-upload__tip">
            ?ØÊè¥ Excel Ê™îÊ??ºÂ?
          </div>
        </template>
      </el-upload>
      <template #footer>
        <el-button @click="importDialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitImport">Á¢∫Â?</el-button>
      </template>
    </el-dialog>

    <!-- ?óÂç∞Â∞çË©±Ê°?-->
    <el-dialog
      v-model="printDialogVisible"
      title="?ëË??óÂç∞"
      width="500px"
    >
      <el-form :model="printFormData" label-width="120px">
        <el-form-item label="?óÂç∞?ºÂ?">
          <el-select v-model="printFormData.PrintFormat" style="width: 100%">
            <el-option label="PDF" value="PDF" />
            <el-option label="Excel" value="Excel" />
          </el-select>
        </el-form-item>
        <el-form-item label="?óÂç∞ÁØÑÂ?">
          <el-radio-group v-model="printFormData.PrintRange">
            <el-radio label="all">?®ÈÉ®</el-radio>
            <el-radio label="selected">Â∑≤ÈÅ∏??/el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="printDialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitPrint">Á¢∫Â?</el-button>
      </template>
    </el-dialog>

    <!-- ?ØÂá∫Â∞çË©±Ê°?-->
    <el-dialog
      v-model="exportDialogVisible"
      title="?ëË??ØÂá∫"
      width="500px"
    >
      <el-form :model="exportFormData" label-width="120px">
        <el-form-item label="?ØÂá∫?ºÂ?">
          <el-select v-model="exportFormData.ExportFormat" style="width: 100%">
            <el-option label="PDF" value="PDF" />
            <el-option label="Excel" value="Excel" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ØÂá∫ÁØÑÂ?">
          <el-radio-group v-model="exportFormData.ExportRange">
            <el-radio label="all">?®ÈÉ®</el-radio>
            <el-radio label="selected">Â∑≤ÈÅ∏??/el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="exportDialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitExport">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { UploadFilled } from '@element-plus/icons-vue'
import { certificateApi } from '@/api/certificate'

export default {
  name: 'CertificateProcess',
  components: {
    UploadFilled
  },
  setup() {
    const loading = ref(false)
    const checkDialogVisible = ref(false)
    const importDialogVisible = ref(false)
    const printDialogVisible = ref(false)
    const exportDialogVisible = ref(false)
    const uploadRef = ref(null)
    const tableData = ref([])
    const selectedRows = ref([])
    const checkResults = ref([])
    const fileList = ref([])

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      VoucherId: ''
    })

    // ?óÂç∞Ë°®ÂñÆË≥áÊ?
    const printFormData = reactive({
      PrintFormat: 'PDF',
      PrintRange: 'all'
    })

    // ?ØÂá∫Ë°®ÂñÆË≥áÊ?
    const exportFormData = reactive({
      ExportFormat: 'Excel',
      ExportRange: 'all'
    })

    // ?ºÂ??ñÊó•??
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // ?ºÂ??ñÊó•?üÊ???
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
    }

    // ?ºÂ??ñË≤®Âπ?
    const formatCurrency = (amount) => {
      if (amount == null) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // ?ñÂ??ëË?È°ûÂ?È°ûÂ?
    const getVoucherTypeType = (type) => {
      const typeMap = {
        'V': 'primary',
        'R': 'success',
        'I': 'warning'
      }
      return typeMap[type] || 'info'
    }

    // ?ñÂ??ëË?È°ûÂ??áÂ?
    const getVoucherTypeText = (type) => {
      const typeMap = {
        'V': '?≥Á•®',
        'R': '?∂Ê?',
        'I': '?ºÁ•®'
      }
      return typeMap[type] || type
    }

    // ?ñÂ??Ä?ãÈ???
    const getStatusType = (status) => {
      const statusMap = {
        'D': 'info',
        'S': 'warning',
        'A': 'success',
        'X': 'danger',
        'C': 'success'
      }
      return statusMap[status] || 'info'
    }

    // ?ñÂ??Ä?ãÊ?Â≠?
    const getStatusText = (status) => {
      const statusMap = {
        'D': '?âÁ®ø',
        'S': 'Â∑≤ÈÄÅÂá∫',
        'A': 'Â∑≤ÂØ©??,
        'X': 'Â∑≤Â?Ê∂?,
        'C': 'Â∑≤Á?Ê°?
      }
      return statusMap[status] || status
    }

    // ?∏Ê?ËÆäÊõ¥
    const handleSelectionChange = (selection) => {
      selectedRows.value = selection
    }

    // ?•Ë©¢Ë≥áÊ?
    const loadData = async () => {
      loading.value = true
      try {
        // ?ôË£°?âË©≤Ë™øÁî®?•Ë©¢?ëË??óË°®??API
        // ?´Ê?‰ΩøÁî®Á©∫Ë???
        tableData.value = []
      } catch (error) {
        console.error('?•Ë©¢?ëË??óË°®Â§±Ê?:', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      } finally {
        loading.value = false
      }
    }

    // ?•Ë©¢
    const handleSearch = () => {
      loadData()
    }

    // ?çÁΩÆ
    const handleReset = () => {
      Object.assign(queryForm, {
        VoucherId: ''
      })
      handleSearch()
    }

    // ?ëË?Ê™¢Êü•
    const handleCheck = async () => {
      if (selectedRows.value.length === 0) {
        ElMessage.warning('Ë´ãËá≥Â∞ëÈÅ∏?á‰?Á≠ÜÊ?Ë≠?)
        return
      }
      try {
        const voucherIds = selectedRows.value.map(row => row.VoucherId)
        const response = await certificateApi.checkVouchers(voucherIds)
        if (response.data && response.data.Success) {
          checkResults.value = response.data.Data || []
          checkDialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || 'Ê™¢Êü•Â§±Ê?')
        }
      } catch (error) {
        console.error('?ëË?Ê™¢Êü•Â§±Ê?:', error)
        ElMessage.error('Ê™¢Êü•Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ?ëË?Â∞éÂÖ•
    const handleImport = () => {
      importDialogVisible.value = true
      fileList.value = []
    }

    // Ê™îÊ?ËÆäÊõ¥
    const handleFileChange = (file) => {
      fileList.value = [file]
    }

    // ?ê‰∫§Â∞éÂÖ•
    const handleSubmitImport = async () => {
      if (fileList.value.length === 0) {
        ElMessage.warning('Ë´ãÈÅ∏?áË?Â∞éÂÖ•?ÑÊ?Ê°?)
        return
      }
      try {
        const file = fileList.value[0].raw
        const response = await certificateApi.importVouchers(file)
        if (response.data && response.data.Success) {
          ElMessage.success('Â∞éÂÖ•?êÂ?')
          importDialogVisible.value = false
          loadData()
        } else {
          ElMessage.error(response.data?.Message || 'Â∞éÂÖ•Â§±Ê?')
        }
      } catch (error) {
        console.error('?ëË?Â∞éÂÖ•Â§±Ê?:', error)
        ElMessage.error('Â∞éÂÖ•Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ?ëË??óÂç∞
    const handlePrint = () => {
      printDialogVisible.value = true
      Object.assign(printFormData, {
        PrintFormat: 'PDF',
        PrintRange: 'all'
      })
    }

    // ?ê‰∫§?óÂç∞
    const handleSubmitPrint = async () => {
      try {
        const voucherIds = printFormData.PrintRange === 'selected' 
          ? selectedRows.value.map(row => row.VoucherId)
          : tableData.value.map(row => row.VoucherId)
        
        const response = await certificateApi.printVouchers({
          VoucherIds: voucherIds,
          PrintFormat: printFormData.PrintFormat
        })
        
        if (response.data && response.data.Success) {
          ElMessage.success('?óÂç∞?êÂ?')
          printDialogVisible.value = false
        } else {
          ElMessage.error(response.data?.Message || '?óÂç∞Â§±Ê?')
        }
      } catch (error) {
        console.error('?ëË??óÂç∞Â§±Ê?:', error)
        ElMessage.error('?óÂç∞Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ?ëË??ØÂá∫
    const handleExport = () => {
      exportDialogVisible.value = true
      Object.assign(exportFormData, {
        ExportFormat: 'Excel',
        ExportRange: 'all'
      })
    }

    // ?ê‰∫§?ØÂá∫
    const handleSubmitExport = async () => {
      try {
        const voucherIds = exportFormData.ExportRange === 'selected'
          ? selectedRows.value.map(row => row.VoucherId)
          : tableData.value.map(row => row.VoucherId)
        
        const response = await certificateApi.exportVouchers({
          VoucherIds: voucherIds,
          ExportFormat: exportFormData.ExportFormat
        })
        
        // ?ïÁ?Ê™îÊ?‰∏ãË?
        const blob = new Blob([response.data])
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `vouchers_${new Date().getTime()}.${exportFormData.ExportFormat.toLowerCase()}`
        link.click()
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('?ØÂá∫?êÂ?')
        exportDialogVisible.value = false
      } catch (error) {
        console.error('?ëË??ØÂá∫Â§±Ê?:', error)
        ElMessage.error('?ØÂá∫Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ?πÈ??¥Êñ∞?Ä??
    const handleBatchUpdateStatus = async () => {
      if (selectedRows.value.length === 0) {
        ElMessage.warning('Ë´ãËá≥Â∞ëÈÅ∏?á‰?Á≠ÜÊ?Ë≠?)
        return
      }
      try {
        const { value: status } = await ElMessageBox.prompt('Ë´ãËº∏?•Êñ∞?Ä??(D:?âÁ®ø, S:Â∑≤ÈÄÅÂá∫, A:Â∑≤ÂØ©?? X:Â∑≤Â?Ê∂? C:Â∑≤Á?Ê°?', '?πÈ??¥Êñ∞?Ä??, {
          confirmButtonText: 'Á¢∫Â?',
          cancelButtonText: '?ñÊ?',
          inputPattern: /^[DSAXC]$/,
          inputErrorMessage: '?Ä?ãÊ†ºÂºè‰?Ê≠?¢∫'
        })
        
        const voucherIds = selectedRows.value.map(row => row.VoucherId)
        await certificateApi.batchUpdateVoucherStatus({
          VoucherIds: voucherIds,
          Status: status
        })
        
        ElMessage.success('?πÈ??¥Êñ∞?Ä?ãÊ???)
        selectedRows.value = []
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('?πÈ??¥Êñ∞?Ä?ãÂ§±??', error)
          ElMessage.error('?πÈ??¥Êñ∞?Ä?ãÂ§±?? ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?ùÂ???
    onMounted(() => {
      loadData()
    })

    return {
      loading,
      checkDialogVisible,
      importDialogVisible,
      printDialogVisible,
      exportDialogVisible,
      uploadRef,
      tableData,
      selectedRows,
      checkResults,
      fileList,
      queryForm,
      printFormData,
      exportFormData,
      handleSearch,
      handleReset,
      handleCheck,
      handleImport,
      handleFileChange,
      handleSubmitImport,
      handlePrint,
      handleSubmitPrint,
      handleExport,
      handleSubmitExport,
      handleBatchUpdateStatus,
      handleSelectionChange,
      formatDate,
      formatDateTime,
      formatCurrency,
      getVoucherTypeType,
      getVoucherTypeText,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.certificate-process-management {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $primary-color;
    }
  }

  .action-card {
    margin-bottom: 20px;
  }

  .search-card {
    margin-bottom: 20px;
    
    .search-form {
      .el-form-item {
        margin-bottom: 0;
      }
    }
  }

  .table-card {
    .el-table {
      .el-tag {
        font-weight: 500;
      }
    }
  }
}
</style>


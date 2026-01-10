<template>
  <div class="receiving-extension-management">
    <div class="page-header">
      <h1>?∂Ê¨æ?¥Â??üËÉΩ (SYSR310-SYSR450)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?≥Á•®?ÆË?">
          <el-input v-model="queryForm.VoucherNo" placeholder="Ë´ãËº∏?•ÂÇ≥Á•®ÂñÆ?? clearable />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÆË?">
          <el-input v-model="queryForm.ReceiptNo" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÂñÆ?? clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
          <el-button type="success" @click="handleCreate">?∞Â?</el-button>
          <el-button type="warning" @click="handleBatchTransfer" :disabled="selectedRows.length === 0">?πÊ¨°?ãË?</el-button>
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
        <el-table-column prop="TKey" label="‰∏ªÈçµ" width="100" />
        <el-table-column prop="VoucherNo" label="?≥Á•®?ÆË?" width="150" />
        <el-table-column prop="ReceiptNo" label="?∂Ê¨æ?ÆË?" width="150" />
        <el-table-column prop="TransferDate" label="?ãË??•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.TransferDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransferStatus" label="?ãË??Ä?? width="120">
          <template #default="{ row }">
            <el-tag :type="getTransferStatusType(row.TransferStatus)">
              {{ getTransferStatusText(row.TransferStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="?ôË®ª" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">‰øÆÊîπ</el-button>
            <el-button type="success" size="small" @click="handleTransfer(row)" :disabled="row.TransferStatus === 'T'">?ãË?</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">?™Èô§</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- ?ÜÈ? -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="pagination.TotalCount"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right;"
      />
    </el-card>

    <!-- ?∞Â?/‰øÆÊîπÂ∞çË©±Ê°?-->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="700px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-form-item label="?≥Á•®?ÆË?" prop="VoucherNo">
          <el-input v-model="formData.VoucherNo" placeholder="Ë´ãËº∏?•ÂÇ≥Á•®ÂñÆ?? />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÆË?" prop="ReceiptNo">
          <el-input v-model="formData.ReceiptNo" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÂñÆ?? />
        </el-form-item>
        <el-form-item label="?ãË??•Ê?" prop="TransferDate">
          <el-date-picker
            v-model="formData.TransferDate"
            type="date"
            placeholder="Ë´ãÈÅ∏?áÊ?ËΩâÊó•??
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="?ãË??Ä?? prop="TransferStatus">
          <el-select v-model="formData.TransferStatus" placeholder="Ë´ãÈÅ∏?áÊ?ËΩâÁ??? style="width: 100%">
            <el-option label="?™Ê?ËΩ? value="P" />
            <el-option label="Â∑≤Ê?ËΩ? value="T" />
            <el-option label="?ãË?Â§±Ê?" value="F" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ôË®ª">
          <el-input v-model="formData.Notes" type="textarea" :rows="3" placeholder="Ë´ãËº∏?•Â?Ë®? />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitForm">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { receiptApi } from '@/api/receipt'

export default {
  name: 'ReceivingExtension',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const selectedRows = ref([])
    const currentTKey = ref(null)

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      VoucherNo: '',
      ReceiptNo: ''
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      VoucherNo: '',
      ReceiptNo: '',
      TransferDate: '',
      TransferStatus: 'P',
      Notes: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      VoucherNo: [{ required: true, message: 'Ë´ãËº∏?•ÂÇ≥Á•®ÂñÆ??, trigger: 'blur' }],
      ReceiptNo: [{ required: true, message: 'Ë´ãËº∏?•Êî∂Ê¨æÂñÆ??, trigger: 'blur' }]
    }

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?∂Ê¨æÊ≤ñÂ∏≥?≥Á•®' : '?∞Â??∂Ê¨æÊ≤ñÂ∏≥?≥Á•®'
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

    // ?ñÂ??ãË??Ä?ãÈ???
    const getTransferStatusType = (status) => {
      const statusMap = {
        'P': 'info',
        'T': 'success',
        'F': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // ?ñÂ??ãË??Ä?ãÊ?Â≠?
    const getTransferStatusText = (status) => {
      const statusMap = {
        'P': '?™Ê?ËΩ?,
        'T': 'Â∑≤Ê?ËΩ?,
        'F': '?ãË?Â§±Ê?'
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
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          VoucherNo: queryForm.VoucherNo || undefined,
          ReceiptNo: queryForm.ReceiptNo || undefined
        }

        const response = await receiptApi.getReceiptVoucherTransfer(params)
        if (response.data && response.data.Success) {
          const result = response.data.Data
          tableData.value = result.Items || []
          pagination.TotalCount = result.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?∂Ê¨æÊ≤ñÂ∏≥?≥Á•®?óË°®Â§±Ê?:', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      } finally {
        loading.value = false
      }
    }

    // ?•Ë©¢
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // ?çÁΩÆ
    const handleReset = () => {
      Object.assign(queryForm, {
        VoucherNo: '',
        ReceiptNo: ''
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        VoucherNo: '',
        ReceiptNo: '',
        TransferDate: '',
        TransferStatus: 'P',
        Notes: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await receiptApi.getReceiptVoucherTransferById(row.TKey)
        if (response.data && response.data.Success) {
          isEdit.value = true
          currentTKey.value = row.TKey
          dialogVisible.value = true
          const data = response.data.Data
          Object.assign(formData, {
            VoucherNo: data.VoucherNo || '',
            ReceiptNo: data.ReceiptNo || '',
            TransferDate: data.TransferDate ? formatDate(data.TransferDate) : '',
            TransferStatus: data.TransferStatus || 'P',
            Notes: data.Notes || ''
          })
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?∂Ê¨æÊ≤ñÂ∏≥?≥Á•®Ë≥áÊ?Â§±Ê?:', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ‰øÆÊîπ
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // ?ãË?
    const handleTransfer = async (row) => {
      try {
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÊ?ËΩâÊ≠§?∂Ê¨æÊ≤ñÂ∏≥?≥Á•®?éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await receiptApi.transferReceiptVoucher(row.TKey)
        ElMessage.success('?ãË??êÂ?')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('?ãË?Â§±Ê?:', error)
          ElMessage.error('?ãË?Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?πÊ¨°?ãË?
    const handleBatchTransfer = async () => {
      if (selectedRows.value.length === 0) {
        ElMessage.warning('Ë´ãËá≥Â∞ëÈÅ∏?á‰?Á≠ÜË???)
        return
      }
      try {
        await ElMessageBox.confirm(`Á¢∫Â?Ë¶ÅÊâπÊ¨°Ê?ËΩ?${selectedRows.value.length} Á≠ÜÊî∂Ê¨æÊ?Â∏≥ÂÇ≥Á•®Â?Ôºü`, 'Á¢∫Ë?', {
          type: 'warning'
        })
        const tKeys = selectedRows.value.map(row => row.TKey)
        await receiptApi.batchTransferReceiptVoucher({ TKeys: tKeys })
        ElMessage.success('?πÊ¨°?ãË??êÂ?')
        selectedRows.value = []
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('?πÊ¨°?ãË?Â§±Ê?:', error)
          ElMessage.error('?πÊ¨°?ãË?Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?™Èô§
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?∂Ê¨æÊ≤ñÂ∏≥?≥Á•®?éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await receiptApi.deleteReceiptVoucherTransfer(row.TKey)
        ElMessage.success('?™Èô§?êÂ?')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('?™Èô§Â§±Ê?:', error)
          ElMessage.error('?™Èô§Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?ê‰∫§Ë°®ÂñÆ
    const handleSubmitForm = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()

        if (isEdit.value) {
          await receiptApi.updateReceiptVoucherTransfer(currentTKey.value, formData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await receiptApi.createReceiptVoucherTransfer(formData)
          ElMessage.success('?∞Â??êÂ?')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('?ê‰∫§Â§±Ê?:', error)
          ElMessage.error((isEdit.value ? '‰øÆÊîπ' : '?∞Â?') + 'Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?ÜÈ?Â§ßÂ?ËÆäÊõ¥
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // ?ÜÈ?ËÆäÊõ¥
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // ?ùÂ???
    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      selectedRows,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleTransfer,
      handleBatchTransfer,
      handleDelete,
      handleSubmitForm,
      handleSizeChange,
      handlePageChange,
      handleSelectionChange,
      formatDate,
      formatDateTime,
      getTransferStatusType,
      getTransferStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.receiving-extension-management {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $primary-color;
    }
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


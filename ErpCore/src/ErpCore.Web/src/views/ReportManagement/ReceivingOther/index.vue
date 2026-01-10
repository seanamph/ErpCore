<template>
  <div class="receiving-other-management">
    <div class="page-header">
      <h1>?∂Ê¨æ?∂‰??üËÉΩ (SYSR510-SYSR570)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="‰øùË??ëÁ∑®??>
          <el-input v-model="queryForm.DepositNo" placeholder="Ë´ãËº∏?•‰?Ë≠âÈ?Á∑®Ë?" clearable />
        </el-form-item>
        <el-form-item label="Â∞çË±°?•Á∑®??>
          <el-input v-model="queryForm.ObjectId" placeholder="Ë´ãËº∏?•Â?Ë±°Âà•Á∑®Ë?" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
          <el-button type="success" @click="handleCreate">?∞Â?</el-button>
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
      >
        <el-table-column prop="TKey" label="‰∏ªÈçµ" width="100" />
        <el-table-column prop="DepositNo" label="‰øùË??ëÁ∑®?? width="150" />
        <el-table-column prop="ObjectId" label="Â∞çË±°?•Á∑®?? width="150" />
        <el-table-column prop="DepositAmount" label="‰øùË??ëÈ?È°? width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.DepositAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="ReturnAmount" label="?Ä?ÑÈ?È°? width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.ReturnAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="BalanceAmount" label="È§òÈ?" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.BalanceAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="DepositDate" label="Â≠òÊ¨æ?•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.DepositDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="?Ä?? width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="?ôË®ª" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">‰øÆÊîπ</el-button>
            <el-button type="success" size="small" @click="handleReturn(row)" :disabled="row.BalanceAmount <= 0">?Ä??/el-button>
            <el-button type="info" size="small" @click="handleDeposit(row)">Â≠òÊ¨æ</el-button>
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
        <el-form-item label="‰øùË??ëÁ∑®?? prop="DepositNo">
          <el-input v-model="formData.DepositNo" :disabled="isEdit" placeholder="Ë´ãËº∏?•‰?Ë≠âÈ?Á∑®Ë?" />
        </el-form-item>
        <el-form-item label="Â∞çË±°?•Á∑®?? prop="ObjectId">
          <el-input v-model="formData.ObjectId" placeholder="Ë´ãËº∏?•Â?Ë±°Âà•Á∑®Ë?" />
        </el-form-item>
        <el-form-item label="‰øùË??ëÈ?È°? prop="DepositAmount">
          <el-input-number
            v-model="formData.DepositAmount"
            :precision="2"
            :min="0"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="Â≠òÊ¨æ?•Ê?" prop="DepositDate">
          <el-date-picker
            v-model="formData.DepositDate"
            type="date"
            placeholder="Ë´ãÈÅ∏?áÂ?Ê¨æÊó•??
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="?Ä?? prop="Status">
          <el-select v-model="formData.Status" placeholder="Ë´ãÈÅ∏?áÁ??? style="width: 100%">
            <el-option label="?âÊ?" value="A" />
            <el-option label="?°Ê?" value="I" />
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

    <!-- ?Ä?ÑÂ?Ë©±Ê? -->
    <el-dialog
      v-model="returnDialogVisible"
      title="‰øùË??ëÈÄÄ??
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="returnFormRef"
        :model="returnFormData"
        :rules="returnFormRules"
        label-width="120px"
      >
        <el-form-item label="?Ä?ÑÈ?È°? prop="ReturnAmount">
          <el-input-number
            v-model="returnFormData.ReturnAmount"
            :precision="2"
            :min="0"
            :max="currentRow?.BalanceAmount || 0"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="?Ä?ÑÊó•?? prop="ReturnDate">
          <el-date-picker
            v-model="returnFormData.ReturnDate"
            type="date"
            placeholder="Ë´ãÈÅ∏?áÈÄÄ?ÑÊó•??
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="?ôË®ª">
          <el-input v-model="returnFormData.Notes" type="textarea" :rows="3" placeholder="Ë´ãËº∏?•Â?Ë®? />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="returnDialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitReturn">Á¢∫Â?</el-button>
      </template>
    </el-dialog>

    <!-- Â≠òÊ¨æÂ∞çË©±Ê°?-->
    <el-dialog
      v-model="depositDialogVisible"
      title="‰øùË??ëÂ?Ê¨?
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="depositFormRef"
        :model="depositFormData"
        :rules="depositFormRules"
        label-width="120px"
      >
        <el-form-item label="Â≠òÊ¨æ?ëÈ?" prop="DepositAmount">
          <el-input-number
            v-model="depositFormData.DepositAmount"
            :precision="2"
            :min="0"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="Â≠òÊ¨æ?•Ê?" prop="DepositDate">
          <el-date-picker
            v-model="depositFormData.DepositDate"
            type="date"
            placeholder="Ë´ãÈÅ∏?áÂ?Ê¨æÊó•??
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="?ôË®ª">
          <el-input v-model="depositFormData.Notes" type="textarea" :rows="3" placeholder="Ë´ãËº∏?•Â?Ë®? />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="depositDialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitDeposit">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { receiptApi } from '@/api/receipt'

export default {
  name: 'ReceivingOther',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const returnDialogVisible = ref(false)
    const depositDialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const returnFormRef = ref(null)
    const depositFormRef = ref(null)
    const tableData = ref([])
    const currentTKey = ref(null)
    const currentRow = ref(null)

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      DepositNo: '',
      ObjectId: ''
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      DepositNo: '',
      ObjectId: '',
      DepositAmount: 0,
      DepositDate: '',
      Status: 'A',
      Notes: ''
    })

    // ?Ä?ÑË°®?ÆË???
    const returnFormData = reactive({
      ReturnAmount: 0,
      ReturnDate: '',
      Notes: ''
    })

    // Â≠òÊ¨æË°®ÂñÆË≥áÊ?
    const depositFormData = reactive({
      DepositAmount: 0,
      DepositDate: '',
      Notes: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      DepositNo: [{ required: true, message: 'Ë´ãËº∏?•‰?Ë≠âÈ?Á∑®Ë?', trigger: 'blur' }],
      ObjectId: [{ required: true, message: 'Ë´ãËº∏?•Â?Ë±°Âà•Á∑®Ë?', trigger: 'blur' }],
      DepositAmount: [{ required: true, message: 'Ë´ãËº∏?•‰?Ë≠âÈ??ëÈ?', trigger: 'blur' }],
      DepositDate: [{ required: true, message: 'Ë´ãÈÅ∏?áÂ?Ê¨æÊó•??, trigger: 'change' }]
    }

    const returnFormRules = {
      ReturnAmount: [{ required: true, message: 'Ë´ãËº∏?•ÈÄÄ?ÑÈ?È°?, trigger: 'blur' }],
      ReturnDate: [{ required: true, message: 'Ë´ãÈÅ∏?áÈÄÄ?ÑÊó•??, trigger: 'change' }]
    }

    const depositFormRules = {
      DepositAmount: [{ required: true, message: 'Ë´ãËº∏?•Â?Ê¨æÈ?È°?, trigger: 'blur' }],
      DepositDate: [{ required: true, message: 'Ë´ãÈÅ∏?áÂ?Ê¨æÊó•??, trigger: 'change' }]
    }

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ‰øùË??? : '?∞Â?‰øùË???
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

    // ?ñÂ??Ä?ãÈ???
    const getStatusType = (status) => {
      const statusMap = {
        'A': 'success',
        'I': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // ?ñÂ??Ä?ãÊ?Â≠?
    const getStatusText = (status) => {
      const statusMap = {
        'A': '?âÊ?',
        'I': '?°Ê?'
      }
      return statusMap[status] || status
    }

    // ?•Ë©¢Ë≥áÊ?
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          DepositNo: queryForm.DepositNo || undefined,
          ObjectId: queryForm.ObjectId || undefined
        }

        const response = await receiptApi.getDeposits(params)
        if (response.data && response.data.Success) {
          const result = response.data.Data
          tableData.value = result.Items || []
          pagination.TotalCount = result.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢‰øùË??ëÂ?Ë°®Â§±??', error)
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
        DepositNo: '',
        ObjectId: ''
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        DepositNo: '',
        ObjectId: '',
        DepositAmount: 0,
        DepositDate: '',
        Status: 'A',
        Notes: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await receiptApi.getDepositById(row.TKey)
        if (response.data && response.data.Success) {
          isEdit.value = true
          currentTKey.value = row.TKey
          dialogVisible.value = true
          const data = response.data.Data
          Object.assign(formData, {
            DepositNo: data.DepositNo || '',
            ObjectId: data.ObjectId || '',
            DepositAmount: data.DepositAmount || 0,
            DepositDate: data.DepositDate ? formatDate(data.DepositDate) : '',
            Status: data.Status || 'A',
            Notes: data.Notes || ''
          })
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢‰øùË??ëË??ôÂ§±??', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ‰øÆÊîπ
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // ?Ä??
    const handleReturn = (row) => {
      currentRow.value = row
      returnDialogVisible.value = true
      Object.assign(returnFormData, {
        ReturnAmount: 0,
        ReturnDate: '',
        Notes: ''
      })
    }

    // ?ê‰∫§?Ä??
    const handleSubmitReturn = async () => {
      if (!returnFormRef.value) return
      try {
        await returnFormRef.value.validate()
        await receiptApi.returnDeposit(currentRow.value.TKey, returnFormData)
        ElMessage.success('?Ä?ÑÊ???)
        returnDialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('?Ä?ÑÂ§±??', error)
          ElMessage.error('?Ä?ÑÂ§±?? ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // Â≠òÊ¨æ
    const handleDeposit = (row) => {
      currentRow.value = row
      depositDialogVisible.value = true
      Object.assign(depositFormData, {
        DepositAmount: 0,
        DepositDate: '',
        Notes: ''
      })
    }

    // ?ê‰∫§Â≠òÊ¨æ
    const handleSubmitDeposit = async () => {
      if (!depositFormRef.value) return
      try {
        await depositFormRef.value.validate()
        await receiptApi.depositAmount(currentRow.value.TKey, depositFormData)
        ElMessage.success('Â≠òÊ¨æ?êÂ?')
        depositDialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('Â≠òÊ¨æÂ§±Ê?:', error)
          ElMessage.error('Â≠òÊ¨æÂ§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?™Èô§
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§‰øùË??ëÂ?Ôº?, 'Á¢∫Ë?', {
          type: 'warning'
        })
        await receiptApi.deleteDeposit(row.TKey)
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
          await receiptApi.updateDeposit(currentTKey.value, formData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await receiptApi.createDeposit(formData)
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
      returnDialogVisible,
      depositDialogVisible,
      isEdit,
      formRef,
      returnFormRef,
      depositFormRef,
      tableData,
      currentRow,
      queryForm,
      pagination,
      formData,
      returnFormData,
      depositFormData,
      formRules,
      returnFormRules,
      depositFormRules,
      dialogTitle,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleReturn,
      handleDeposit,
      handleDelete,
      handleSubmitForm,
      handleSubmitReturn,
      handleSubmitDeposit,
      handleSizeChange,
      handlePageChange,
      formatDate,
      formatDateTime,
      formatCurrency,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.receiving-other-management {
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


<template>
  <div class="certificate-data-management">
    <div class="page-header">
      <h1>?ëË?Ë≥áÊ?Á∂≠Ë≠∑ (SYSK110-SYSK150)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ëË?Á∑®Ë?">
          <el-input v-model="queryForm.VoucherId" placeholder="Ë´ãËº∏?•Ê?Ë≠âÁ∑®?? clearable />
        </el-form-item>
        <el-form-item label="?ëË?È°ûÂ?">
          <el-select v-model="queryForm.VoucherType" placeholder="Ë´ãÈÅ∏?áÊ?Ë≠âÈ??? clearable>
            <el-option label="?≥Á•®" value="V" />
            <el-option label="?∂Ê?" value="R" />
            <el-option label="?ºÁ•®" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ÜÂ?‰ª??">
          <el-input v-model="queryForm.ShopId" placeholder="Ë´ãËº∏?•Â?Â∫ó‰ª£?? clearable />
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?áÁ??? clearable>
            <el-option label="?âÁ®ø" value="D" />
            <el-option label="Â∑≤ÈÄÅÂá∫" value="S" />
            <el-option label="Â∑≤ÂØ©?? value="A" />
            <el-option label="Â∑≤Â?Ê∂? value="X" />
            <el-option label="Â∑≤Á?Ê°? value="C" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ëË??•Ê?">
          <el-date-picker
            v-model="queryForm.VoucherDateRange"
            type="daterange"
            range-separator="??
            start-placeholder="?ãÂ??•Ê?"
            end-placeholder="ÁµêÊ??•Ê?"
            value-format="YYYY-MM-DD"
          />
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
        <el-table-column prop="ShopId" label="?ÜÂ?‰ª??" width="120" />
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
        <el-table-column prop="TotalDebitAmount" label="?üÊñπÁ∏ΩÈ?" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalDebitAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalCreditAmount" label="Ë≤∏ÊñπÁ∏ΩÈ?" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalCreditAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="ApplyUserId" label="?≥Ë?‰∫∫Âì°" width="120" />
        <el-table-column prop="ApplyDate" label="?≥Ë??•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ApplyDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ApproveUserId" label="ÂØ©Ê†∏‰∫∫Âì°" width="120" />
        <el-table-column prop="ApproveDate" label="ÂØ©Ê†∏?•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ApproveDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Memo" label="?ôË®ª" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'A' || row.Status === 'C'">‰øÆÊîπ</el-button>
            <el-button type="success" size="small" @click="handleApprove(row)" :disabled="row.Status !== 'S'">ÂØ©Ê†∏</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status === 'A' || row.Status === 'C'">?™Èô§</el-button>
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
      width="900px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ëË?Á∑®Ë?" prop="VoucherId">
              <el-input v-model="formData.VoucherId" :disabled="isEdit" placeholder="Ë´ãËº∏?•Ê?Ë≠âÁ∑®?? />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ëË??•Ê?" prop="VoucherDate">
              <el-date-picker
                v-model="formData.VoucherDate"
                type="date"
                placeholder="Ë´ãÈÅ∏?áÊ?Ë≠âÊó•??
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ëË?È°ûÂ?" prop="VoucherType">
              <el-select v-model="formData.VoucherType" placeholder="Ë´ãÈÅ∏?áÊ?Ë≠âÈ??? style="width: 100%">
                <el-option label="?≥Á•®" value="V" />
                <el-option label="?∂Ê?" value="R" />
                <el-option label="?ºÁ•®" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ÜÂ?‰ª??" prop="ShopId">
              <el-input v-model="formData.ShopId" placeholder="Ë´ãËº∏?•Â?Â∫ó‰ª£?? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?Ä?? prop="Status">
              <el-select v-model="formData.Status" placeholder="Ë´ãÈÅ∏?áÁ??? style="width: 100%">
                <el-option label="?âÁ®ø" value="D" />
                <el-option label="Â∑≤ÈÄÅÂá∫" value="S" />
                <el-option label="Â∑≤ÂØ©?? value="A" />
                <el-option label="Â∑≤Â?Ê∂? value="X" />
                <el-option label="Â∑≤Á?Ê°? value="C" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Âπ?à•" prop="CurrencyId">
              <el-input v-model="formData.CurrencyId" placeholder="Ë´ãËº∏?•Âπ£?? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="?ôË®ª">
          <el-input v-model="formData.Memo" type="textarea" :rows="3" placeholder="Ë´ãËº∏?•Â?Ë®? />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitForm">Á¢∫Â?</el-button>
      </template>
    </el-dialog>

    <!-- ÂØ©Ê†∏Â∞çË©±Ê°?-->
    <el-dialog
      v-model="approveDialogVisible"
      title="ÂØ©Ê†∏?ëË?"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="approveFormRef"
        :model="approveFormData"
        :rules="approveFormRules"
        label-width="120px"
      >
        <el-form-item label="ÂØ©Ê†∏?ôË®ª">
          <el-input v-model="approveFormData.Memo" type="textarea" :rows="3" placeholder="Ë´ãËº∏?•ÂØ©?∏Â?Ë®? />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="approveDialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitApprove">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { certificateApi } from '@/api/certificate'

export default {
  name: 'CertificateData',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const approveDialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const approveFormRef = ref(null)
    const tableData = ref([])
    const currentVoucherId = ref(null)

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      VoucherId: '',
      VoucherType: '',
      ShopId: '',
      Status: '',
      VoucherDateRange: null
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      VoucherId: '',
      VoucherDate: '',
      VoucherType: 'V',
      ShopId: '',
      Status: 'D',
      CurrencyId: 'TWD',
      Memo: ''
    })

    // ÂØ©Ê†∏Ë°®ÂñÆË≥áÊ?
    const approveFormData = reactive({
      Memo: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      VoucherId: [{ required: true, message: 'Ë´ãËº∏?•Ê?Ë≠âÁ∑®??, trigger: 'blur' }],
      VoucherDate: [{ required: true, message: 'Ë´ãÈÅ∏?áÊ?Ë≠âÊó•??, trigger: 'change' }],
      VoucherType: [{ required: true, message: 'Ë´ãÈÅ∏?áÊ?Ë≠âÈ???, trigger: 'change' }],
      ShopId: [{ required: true, message: 'Ë´ãËº∏?•Â?Â∫ó‰ª£??, trigger: 'blur' }]
    }

    const approveFormRules = {}

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?ëË?' : '?∞Â??ëË?'
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

    // ?•Ë©¢Ë≥áÊ?
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          VoucherId: queryForm.VoucherId || undefined,
          VoucherType: queryForm.VoucherType || undefined,
          ShopId: queryForm.ShopId || undefined,
          Status: queryForm.Status || undefined,
          VoucherDateFrom: queryForm.VoucherDateRange ? queryForm.VoucherDateRange[0] : undefined,
          VoucherDateTo: queryForm.VoucherDateRange ? queryForm.VoucherDateRange[1] : undefined
        }

        const response = await certificateApi.getVouchers(params)
        if (response.data && response.data.Success) {
          const result = response.data.Data
          tableData.value = result.Items || []
          pagination.TotalCount = result.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?ëË??óË°®Â§±Ê?:', error)
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
        VoucherId: '',
        VoucherType: '',
        ShopId: '',
        Status: '',
        VoucherDateRange: null
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      currentVoucherId.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        VoucherId: '',
        VoucherDate: '',
        VoucherType: 'V',
        ShopId: '',
        Status: 'D',
        CurrencyId: 'TWD',
        Memo: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await certificateApi.getVoucher(row.VoucherId)
        if (response.data && response.data.Success) {
          isEdit.value = true
          currentVoucherId.value = row.VoucherId
          dialogVisible.value = true
          const data = response.data.Data
          Object.assign(formData, {
            VoucherId: data.VoucherId || '',
            VoucherDate: data.VoucherDate ? formatDate(data.VoucherDate) : '',
            VoucherType: data.VoucherType || 'V',
            ShopId: data.ShopId || '',
            Status: data.Status || 'D',
            CurrencyId: data.CurrencyId || 'TWD',
            Memo: data.Memo || ''
          })
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?ëË?Ë≥áÊ?Â§±Ê?:', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ‰øÆÊîπ
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // ÂØ©Ê†∏
    const handleApprove = (row) => {
      currentVoucherId.value = row.VoucherId
      approveDialogVisible.value = true
      Object.assign(approveFormData, {
        Memo: ''
      })
    }

    // ?ê‰∫§ÂØ©Ê†∏
    const handleSubmitApprove = async () => {
      try {
        await certificateApi.approveVoucher(currentVoucherId.value, approveFormData)
        ElMessage.success('ÂØ©Ê†∏?êÂ?')
        approveDialogVisible.value = false
        loadData()
      } catch (error) {
        console.error('ÂØ©Ê†∏Â§±Ê?:', error)
        ElMessage.error('ÂØ©Ê†∏Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ?™Èô§
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?ëË??éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await certificateApi.deleteVoucher(row.VoucherId)
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
          await certificateApi.updateVoucher(currentVoucherId.value, formData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await certificateApi.createVoucher(formData)
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
      approveDialogVisible,
      isEdit,
      formRef,
      approveFormRef,
      tableData,
      queryForm,
      pagination,
      formData,
      approveFormData,
      formRules,
      approveFormRules,
      dialogTitle,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleApprove,
      handleDelete,
      handleSubmitForm,
      handleSubmitApprove,
      handleSizeChange,
      handlePageChange,
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

.certificate-data-management {
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


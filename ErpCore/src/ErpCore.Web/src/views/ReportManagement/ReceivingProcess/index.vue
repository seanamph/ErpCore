<template>
  <div class="receiving-process-management">
    <div class="page-header">
      <h1>?∂Ê¨æ?ïÁ??üËÉΩ (SYSR210-SYSR240)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ÜÂÖ¨?∏‰ª£??>
          <el-input v-model="queryForm.SiteId" placeholder="Ë´ãËº∏?•Â??¨Âè∏‰ª??" clearable />
        </el-form-item>
        <el-form-item label="?ÜÂ?‰ª??">
          <el-input v-model="queryForm.ShopId" placeholder="Ë´ãËº∏?•Â?Â∫ó‰ª£?? clearable />
        </el-form-item>
        <el-form-item label="Â∞çË±°?•Á∑®??>
          <el-input v-model="queryForm.ObjectId" placeholder="Ë´ãËº∏?•Â?Ë±°Âà•Á∑®Ë?" clearable />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÆË?">
          <el-input v-model="queryForm.ReceiptNo" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÂñÆ?? clearable />
        </el-form-item>
        <el-form-item label="?≥Á•®?ÆË?">
          <el-input v-model="queryForm.VoucherNo" placeholder="Ë´ãËº∏?•ÂÇ≥Á•®ÂñÆ?? clearable />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?•Ê?">
          <el-date-picker
            v-model="queryForm.ReceiptDateRange"
            type="daterange"
            range-separator="??
            start-placeholder="?ãÂ??•Ê?"
            end-placeholder="ÁµêÊ??•Ê?"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÖÁõÆ">
          <el-input v-model="queryForm.AritemId" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÈ??Æ‰ª£?? clearable />
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
        <el-table-column prop="SiteId" label="?ÜÂÖ¨?∏‰ª£?? width="120" />
        <el-table-column prop="ShopId" label="?ÜÂ?‰ª??" width="120" />
        <el-table-column prop="ObjectId" label="Â∞çË±°?•Á∑®?? width="150" />
        <el-table-column prop="AcctKey" label="Â∞çÂ∏≥KEY?? width="150" />
        <el-table-column prop="ReceiptNo" label="?∂Ê¨æ?ÆË?" width="150" />
        <el-table-column prop="VoucherNo" label="?≥Á•®?ÆË?" width="150" />
        <el-table-column prop="ReceiptDate" label="?∂Ê¨æ?•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ReceiptDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ReceiptAmount" label="?∂Ê¨æ?ëÈ?" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.ReceiptAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="AritemId" label="?∂Ê¨æ?ÖÁõÆ‰ª??" width="150" />
        <el-table-column prop="VoucherStatus" label="?≥Á•®?Ä?? width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.VoucherStatus)">
              {{ getStatusText(row.VoucherStatus) }}
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
      width="800px"
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
            <el-form-item label="?ÜÂÖ¨?∏‰ª£?? prop="SiteId">
              <el-input v-model="formData.SiteId" placeholder="Ë´ãËº∏?•Â??¨Âè∏‰ª??" />
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
            <el-form-item label="Â∞çË±°?•Á∑®?? prop="ObjectId">
              <el-input v-model="formData.ObjectId" placeholder="Ë´ãËº∏?•Â?Ë±°Âà•Á∑®Ë?" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Â∞çÂ∏≥KEY?? prop="AcctKey">
              <el-input v-model="formData.AcctKey" placeholder="Ë´ãËº∏?•Â?Â∏≥KEY?? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?∂Ê¨æ?•Ê?" prop="ReceiptDate">
              <el-date-picker
                v-model="formData.ReceiptDate"
                type="date"
                placeholder="Ë´ãÈÅ∏?áÊî∂Ê¨æÊó•??
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?∂Ê¨æ?ëÈ?" prop="ReceiptAmount">
              <el-input-number
                v-model="formData.ReceiptAmount"
                :precision="2"
                :min="0"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?∂Ê¨æ?ÖÁõÆ‰ª??" prop="AritemId">
              <el-input v-model="formData.AritemId" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÈ??Æ‰ª£?? />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?∂Ê¨æ?ÆË?" prop="ReceiptNo">
              <el-input v-model="formData.ReceiptNo" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÂñÆ?? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?≥Á•®?ÆË?" prop="VoucherNo">
              <el-input v-model="formData.VoucherNo" placeholder="Ë´ãËº∏?•ÂÇ≥Á•®ÂñÆ?? />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?≥Á•®?Ä?? prop="VoucherStatus">
              <el-select v-model="formData.VoucherStatus" placeholder="Ë´ãÈÅ∏?áÂÇ≥Á•®Á??? style="width: 100%">
                <el-option label="?âÁ®ø" value="D" />
                <el-option label="Â∑≤ÈÄÅÂá∫" value="S" />
                <el-option label="Â∑≤ÂØ©?? value="A" />
                <el-option label="Â∑≤Â?Ê∂? value="X" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
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
  name: 'ReceivingProcess',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const currentTKey = ref(null)

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      SiteId: '',
      ShopId: '',
      ObjectId: '',
      ReceiptNo: '',
      VoucherNo: '',
      ReceiptDateRange: null,
      AritemId: ''
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      SiteId: '',
      ShopId: '',
      ObjectId: '',
      AcctKey: '',
      ReceiptDate: '',
      ReceiptAmount: 0,
      AritemId: '',
      ReceiptNo: '',
      VoucherNo: '',
      VoucherStatus: 'D',
      Notes: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      SiteId: [{ required: true, message: 'Ë´ãËº∏?•Â??¨Âè∏‰ª??', trigger: 'blur' }],
      ShopId: [{ required: true, message: 'Ë´ãËº∏?•Â?Â∫ó‰ª£??, trigger: 'blur' }],
      ReceiptDate: [{ required: true, message: 'Ë´ãÈÅ∏?áÊî∂Ê¨æÊó•??, trigger: 'change' }],
      ReceiptAmount: [{ required: true, message: 'Ë´ãËº∏?•Êî∂Ê¨æÈ?È°?, trigger: 'blur' }]
    }

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?âÊî∂Â∏≥Ê¨æ' : '?∞Â??âÊî∂Â∏≥Ê¨æ'
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
        'D': 'info',
        'S': 'warning',
        'A': 'success',
        'X': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // ?ñÂ??Ä?ãÊ?Â≠?
    const getStatusText = (status) => {
      const statusMap = {
        'D': '?âÁ®ø',
        'S': 'Â∑≤ÈÄÅÂá∫',
        'A': 'Â∑≤ÂØ©??,
        'X': 'Â∑≤Â?Ê∂?
      }
      return statusMap[status] || status
    }

    // ?•Ë©¢Ë≥áÊ?
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          SiteId: queryForm.SiteId || undefined,
          ShopId: queryForm.ShopId || undefined,
          ObjectId: queryForm.ObjectId || undefined,
          ReceiptNo: queryForm.ReceiptNo || undefined,
          VoucherNo: queryForm.VoucherNo || undefined,
          StartDate: queryForm.ReceiptDateRange ? queryForm.ReceiptDateRange[0] : undefined,
          EndDate: queryForm.ReceiptDateRange ? queryForm.ReceiptDateRange[1] : undefined,
          AritemId: queryForm.AritemId || undefined
        }

        const response = await receiptApi.getAccountsReceivable(params)
        if (response.data && response.data.Success) {
          tableData.value = Array.isArray(response.data.Data) ? response.data.Data : []
          pagination.TotalCount = tableData.value.length
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?âÊî∂Â∏≥Ê¨æ?óË°®Â§±Ê?:', error)
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
        SiteId: '',
        ShopId: '',
        ObjectId: '',
        ReceiptNo: '',
        VoucherNo: '',
        ReceiptDateRange: null,
        AritemId: ''
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        SiteId: '',
        ShopId: '',
        ObjectId: '',
        AcctKey: '',
        ReceiptDate: '',
        ReceiptAmount: 0,
        AritemId: '',
        ReceiptNo: '',
        VoucherNo: '',
        VoucherStatus: 'D',
        Notes: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await receiptApi.getAccountsReceivableById(row.TKey)
        if (response.data && response.data.Success) {
          isEdit.value = true
          currentTKey.value = row.TKey
          dialogVisible.value = true
          const data = response.data.Data
          Object.assign(formData, {
            SiteId: data.SiteId || '',
            ShopId: data.ShopId || '',
            ObjectId: data.ObjectId || '',
            AcctKey: data.AcctKey || '',
            ReceiptDate: data.ReceiptDate ? formatDate(data.ReceiptDate) : '',
            ReceiptAmount: data.ReceiptAmount || 0,
            AritemId: data.AritemId || '',
            ReceiptNo: data.ReceiptNo || '',
            VoucherNo: data.VoucherNo || '',
            VoucherStatus: data.VoucherStatus || 'D',
            Notes: data.Notes || ''
          })
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?âÊî∂Â∏≥Ê¨æË≥áÊ?Â§±Ê?:', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ‰øÆÊîπ
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // ?™Èô§
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?âÊî∂Â∏≥Ê¨æ?éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await receiptApi.deleteAccountsReceivable(row.TKey)
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
          await receiptApi.updateAccountsReceivable(currentTKey.value, formData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await receiptApi.createAccountsReceivable(formData)
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
      handleDelete,
      handleSubmitForm,
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

.receiving-process-management {
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


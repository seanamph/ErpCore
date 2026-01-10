<template>
  <div class="procurement-bank-management">
    <div class="page-header">
      <h1>?ÄË°åÁÆ°??/h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ÄË°åÂ∏≥?∂‰ª£??>
          <el-input v-model="queryForm.BankAccountId" placeholder="Ë´ãËº∏?•È?Ë°åÂ∏≥?∂‰ª£?? clearable />
        </el-form-item>
        <el-form-item label="?ÄË°å‰ª£??>
          <el-input v-model="queryForm.BankId" placeholder="Ë´ãËº∏?•È?Ë°å‰ª£?? clearable />
        </el-form-item>
        <el-form-item label="Â∏≥Êà∂?çÁ®±">
          <el-input v-model="queryForm.AccountName" placeholder="Ë´ãËº∏?•Â∏≥?∂Â?Á®? clearable />
        </el-form-item>
        <el-form-item label="Â∏≥Êà∂È°ûÂ?">
          <el-select v-model="queryForm.AccountType" placeholder="Ë´ãÈÅ∏?áÂ∏≥?∂È??? clearable>
            <el-option label="Ê¥ªÊ?Â≠òÊ¨æ" value="CHECKING" />
            <el-option label="ÂÆöÊ?Â≠òÊ¨æ" value="SAVINGS" />
            <el-option label="Â§ñÂπ£Â∏≥Êà∂" value="FOREIGN" />
            <el-option label="?∂‰?" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?áÁ??? clearable>
            <el-option label="?üÁî®" value="A" />
            <el-option label="?úÁî®" value="I" />
          </el-select>
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
        <el-table-column prop="BankAccountId" label="?ÄË°åÂ∏≥?∂‰ª£?? width="150" />
        <el-table-column prop="BankId" label="?ÄË°å‰ª£?? width="120" />
        <el-table-column prop="BankName" label="?ÄË°åÂ?Á®? width="200" />
        <el-table-column prop="AccountName" label="Â∏≥Êà∂?çÁ®±" width="200" />
        <el-table-column prop="AccountNumber" label="Â∏≥Êà∂?üÁ¢º" width="200" />
        <el-table-column prop="AccountType" label="Â∏≥Êà∂È°ûÂ?" width="120">
          <template #default="{ row }">
            {{ getAccountTypeName(row.AccountType) }}
          </template>
        </el-table-column>
        <el-table-column prop="CurrencyId" label="Âπ?à•" width="80" />
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">‰øÆÊîπ</el-button>
            <el-button type="info" size="small" @click="handleViewBalance(row)">È§òÈ?</el-button>
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
            <el-form-item label="?ÄË°åÂ∏≥?∂‰ª£?? prop="BankAccountId">
              <el-input v-model="formData.BankAccountId" :disabled="isEdit" placeholder="Ë´ãËº∏?•È?Ë°åÂ∏≥?∂‰ª£?? />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ÄË°å‰ª£?? prop="BankId">
              <el-input v-model="formData.BankId" placeholder="Ë´ãËº∏?•È?Ë°å‰ª£?? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Â∏≥Êà∂?çÁ®±" prop="AccountName">
              <el-input v-model="formData.AccountName" placeholder="Ë´ãËº∏?•Â∏≥?∂Â?Á®? />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Â∏≥Êà∂?üÁ¢º" prop="AccountNumber">
              <el-input v-model="formData.AccountNumber" placeholder="Ë´ãËº∏?•Â∏≥?∂Ë?Á¢? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Â∏≥Êà∂È°ûÂ?" prop="AccountType">
              <el-select v-model="formData.AccountType" placeholder="Ë´ãÈÅ∏?áÂ∏≥?∂È??? style="width: 100%">
                <el-option label="Ê¥ªÊ?Â≠òÊ¨æ" value="CHECKING" />
                <el-option label="ÂÆöÊ?Â≠òÊ¨æ" value="SAVINGS" />
                <el-option label="Â§ñÂπ£Â∏≥Êà∂" value="FOREIGN" />
                <el-option label="?∂‰?" value="OTHER" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Âπ?à•" prop="CurrencyId">
              <el-input v-model="formData.CurrencyId" placeholder="Ë´ãËº∏?•Âπ£?? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?Ä?? prop="Status">
              <el-select v-model="formData.Status" placeholder="Ë´ãÈÅ∏?áÁ??? style="width: 100%">
                <el-option label="?üÁî®" value="A" />
                <el-option label="?úÁî®" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="?ôË®ª" prop="Memo">
              <el-input v-model="formData.Memo" type="textarea" :rows="3" placeholder="Ë´ãËº∏?•Â?Ë®? />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmit">Á¢∫Â?</el-button>
      </template>
    </el-dialog>

    <!-- È§òÈ??•Ë©¢Â∞çË©±Ê°?-->
    <el-dialog
      v-model="balanceDialogVisible"
      title="?ÄË°åÂ∏≥?∂È?È°?
      width="500px"
    >
      <el-descriptions :column="1" border>
        <el-descriptions-item label="?ÄË°åÂ∏≥?∂‰ª£??>{{ balanceData.BankAccountId }}</el-descriptions-item>
        <el-descriptions-item label="Â∏≥Êà∂?çÁ®±">{{ balanceData.AccountName }}</el-descriptions-item>
        <el-descriptions-item label="Âπ?à•">{{ balanceData.CurrencyId }}</el-descriptions-item>
        <el-descriptions-item label="È§òÈ?">
          <span style="font-size: 18px; font-weight: bold; color: $primary-color;">
            {{ formatCurrency(balanceData.Balance) }}
          </span>
        </el-descriptions-item>
        <el-descriptions-item label="?•Ë©¢?ÇÈ?">{{ formatDateTime(balanceData.QueryTime) }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="balanceDialogVisible = false">?úÈ?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { procurementApi } from '@/api/procurement'

export default {
  name: 'ProcurementBankManagement',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const balanceDialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const balanceData = ref({})

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      BankAccountId: '',
      BankId: '',
      AccountName: '',
      AccountType: '',
      Status: '',
      PageIndex: 1,
      PageSize: 20
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      BankAccountId: '',
      BankId: '',
      AccountName: '',
      AccountNumber: '',
      AccountType: '',
      CurrencyId: '',
      Status: 'A',
      Memo: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      BankAccountId: [{ required: true, message: 'Ë´ãËº∏?•È?Ë°åÂ∏≥?∂‰ª£??, trigger: 'blur' }],
      BankId: [{ required: true, message: 'Ë´ãËº∏?•È?Ë°å‰ª£??, trigger: 'blur' }],
      AccountName: [{ required: true, message: 'Ë´ãËº∏?•Â∏≥?∂Â?Á®?, trigger: 'blur' }],
      AccountNumber: [{ required: true, message: 'Ë´ãËº∏?•Â∏≥?∂Ë?Á¢?, trigger: 'blur' }],
      Status: [{ required: true, message: 'Ë´ãÈÅ∏?áÁ???, trigger: 'change' }]
    }

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?ÄË°åÂ∏≥?? : '?∞Â??ÄË°åÂ∏≥??
    })

    // ?ñÂ?Â∏≥Êà∂È°ûÂ??çÁ®±
    const getAccountTypeName = (type) => {
      const types = {
        'CHECKING': 'Ê¥ªÊ?Â≠òÊ¨æ',
        'SAVINGS': 'ÂÆöÊ?Â≠òÊ¨æ',
        'FOREIGN': 'Â§ñÂπ£Â∏≥Êà∂',
        'OTHER': '?∂‰?'
      }
      return types[type] || type
    }

    // ?ºÂ??ñË≤®Âπ?
    const formatCurrency = (amount) => {
      if (!amount) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // ?ºÂ??ñÊó•?üÊ???
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // ?•Ë©¢Ë≥áÊ?
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        // ÁßªÈô§Á©∫ÂÄ?
        Object.keys(params).forEach(key => {
          if (params[key] === '' || params[key] === null) {
            delete params[key]
          }
        })
        const response = await procurementApi.getBanks(params)
        if (response.Data) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
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
        BankAccountId: '',
        BankId: '',
        AccountName: '',
        AccountType: '',
        Status: ''
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        BankAccountId: '',
        BankId: '',
        AccountName: '',
        AccountNumber: '',
        AccountType: '',
        CurrencyId: '',
        Status: 'A',
        Memo: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await procurementApi.getBank(row.BankAccountId)
        if (response.Data) {
          Object.assign(formData, response.Data)
          isEdit.value = true
          dialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ‰øÆÊîπ
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // ?•Á?È§òÈ?
    const handleViewBalance = async (row) => {
      try {
        const response = await procurementApi.getBankBalance(row.BankAccountId)
        if (response.Data) {
          balanceData.value = {
            BankAccountId: row.BankAccountId,
            AccountName: row.AccountName,
            CurrencyId: row.CurrencyId || 'TWD',
            Balance: response.Data.Balance || 0,
            QueryTime: new Date()
          }
          balanceDialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('?•Ë©¢È§òÈ?Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ?™Èô§
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `Á¢∫Â?Ë¶ÅÂà™?§È?Ë°åÂ∏≥?∂„Ä?{row.AccountName}?çÂ?Ôºü`,
          'Á¢∫Ë??™Èô§',
          {
            confirmButtonText: 'Á¢∫Â?',
            cancelButtonText: '?ñÊ?',
            type: 'warning'
          }
        )
        await procurementApi.deleteBank(row.BankAccountId)
        ElMessage.success('?™Èô§?êÂ?')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('?™Èô§Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?ê‰∫§
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          const { BankAccountId, ...updateData } = formData
          await procurementApi.updateBank(BankAccountId, updateData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await procurementApi.createBank(formData)
          ElMessage.success('?∞Â??êÂ?')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
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
      balanceDialogVisible,
      isEdit,
      formRef,
      tableData,
      balanceData,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      getAccountTypeName,
      formatCurrency,
      formatDateTime,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleViewBalance,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.procurement-bank-management {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
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
      .el-button {
        margin-right: 5px;
      }
    }
  }
}
</style>


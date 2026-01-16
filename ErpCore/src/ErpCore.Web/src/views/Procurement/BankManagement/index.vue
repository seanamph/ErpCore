<template>
  <div class="procurement-bank-management">
    <div class="page-header">
      <h1>??????/h1>
    </div>

    <!-- ????? -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?????????>
          <el-input v-model="queryForm.BankAccountId" placeholder="????????????? clearable />
        </el-form-item>
        <el-form-item label="??????>
          <el-input v-model="queryForm.BankId" placeholder="?????????? clearable />
        </el-form-item>
        <el-form-item label="?????">
          <el-input v-model="queryForm.AccountName" placeholder="??????????? clearable />
        </el-form-item>
        <el-form-item label="?????">
          <el-select v-model="queryForm.AccountType" placeholder="??????????? clearable>
            <el-option label="?????" value="CHECKING" />
            <el-option label="?????" value="SAVINGS" />
            <el-option label="????" value="FOREIGN" />
            <el-option label="????" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="????>
          <el-select v-model="queryForm.Status" placeholder="???????? clearable>
            <el-option label="???" value="A" />
            <el-option label="???" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">???</el-button>
          <el-button @click="handleReset">???</el-button>
          <el-button type="success" @click="handleCreate">????</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- ????? -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="BankAccountId" label="????????? width="150" />
        <el-table-column prop="BankId" label="?????? width="120" />
        <el-table-column prop="BankName" label="??????? width="200" />
        <el-table-column prop="AccountName" label="?????" width="200" />
        <el-table-column prop="AccountNumber" label="?????" width="200" />
        <el-table-column prop="AccountType" label="?????" width="120">
          <template #default="{ row }">
            {{ getAccountTypeName(row.AccountType) }}
          </template>
        </el-table-column>
        <el-table-column prop="CurrencyId" label="????" width="80" />
        <el-table-column prop="Status" label="???? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '???' : '???' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="????" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">????</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">??</el-button>
            <el-button type="info" size="small" @click="handleViewBalance(row)">???</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">???</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- ???? -->
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

    <!-- ????/??????-->
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
            <el-form-item label="????????? prop="BankAccountId">
              <el-input v-model="formData.BankAccountId" :disabled="isEdit" placeholder="????????????? />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?????? prop="BankId">
              <el-input v-model="formData.BankId" placeholder="?????????? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?????" prop="AccountName">
              <el-input v-model="formData.AccountName" placeholder="??????????? />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?????" prop="AccountNumber">
              <el-input v-model="formData.AccountNumber" placeholder="??????????? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?????" prop="AccountType">
              <el-select v-model="formData.AccountType" placeholder="??????????? style="width: 100%">
                <el-option label="?????" value="CHECKING" />
                <el-option label="?????" value="SAVINGS" />
                <el-option label="????" value="FOREIGN" />
                <el-option label="????" value="OTHER" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="????" prop="CurrencyId">
              <el-input v-model="formData.CurrencyId" placeholder="??????? />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="???? prop="Status">
              <el-select v-model="formData.Status" placeholder="???????? style="width: 100%">
                <el-option label="???" value="A" />
                <el-option label="???" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="???" prop="Memo">
              <el-input v-model="formData.Memo" type="textarea" :rows="3" placeholder="???????? />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">????</el-button>
        <el-button type="primary" @click="handleSubmit">???</el-button>
      </template>
    </el-dialog>

    <!-- ??????????-->
    <el-dialog
      v-model="balanceDialogVisible"
      title="??????????
      width="500px"
    >
      <el-descriptions :column="1" border>
        <el-descriptions-item label="?????????>{{ balanceData.BankAccountId }}</el-descriptions-item>
        <el-descriptions-item label="?????">{{ balanceData.AccountName }}</el-descriptions-item>
        <el-descriptions-item label="????">{{ balanceData.CurrencyId }}</el-descriptions-item>
        <el-descriptions-item label="???">
          <span style="font-size: 18px; font-weight: bold; color: $primary-color;">
            {{ formatCurrency(balanceData.Balance) }}
          </span>
        </el-descriptions-item>
        <el-descriptions-item label="???????">{{ formatDateTime(balanceData.QueryTime) }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="balanceDialogVisible = false">????</el-button>
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

    // ?????
    const queryForm = reactive({
      BankAccountId: '',
      BankId: '',
      AccountName: '',
      AccountType: '',
      Status: '',
      PageIndex: 1,
      PageSize: 20
    })

    // ???????
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // ?????
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

    // ????????
    const formRules = {
      BankAccountId: [{ required: true, message: '?????????????, trigger: 'blur' }],
      BankId: [{ required: true, message: '??????????, trigger: 'blur' }],
      AccountName: [{ required: true, message: '???????????, trigger: 'blur' }],
      AccountNumber: [{ required: true, message: '???????????, trigger: 'blur' }],
      Status: [{ required: true, message: '????????, trigger: 'change' }]
    }

    // ???????
    const dialogTitle = computed(() => {
      return isEdit.value ? '???????? : '??????????
    })

    // ????????????
    const getAccountTypeName = (type) => {
      const types = {
        'CHECKING': '?????',
        'SAVINGS': '?????',
        'FOREIGN': '????',
        'OTHER': '????'
      }
      return types[type] || type
    }

    // ?????????
    const formatCurrency = (amount) => {
      if (!amount) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // ?????????????
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // ??????
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        // ?????
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
        ElMessage.error('??????: ' + (error.message || '??????'))
      } finally {
        loading.value = false
      }
    }

    // ???
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // ???
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

    // ????
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        BankAccountId: '',
        BankId: '',
        AccountName: '',
        AccountNumber: '',
        AccountType: '',
        CurrencyId: 'TWD',
        Status: '1',
        Balance: null,
        OpeningDate: null,
        ClosingDate: null,
        ContactPerson: '',
        ContactPhone: '',
        ContactEmail: '',
        BranchName: '',
        BranchCode: '',
        SwiftCode: '',
        Notes: ''
      })
    }

    // ????
    const handleView = async (row) => {
      try {
        const response = await procurementApi.getBank(row.BankAccountId)
        if (response.Data) {
          Object.assign(formData, response.Data)
          isEdit.value = true
          dialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('??????: ' + (error.message || '??????'))
      }
    }

    // ??
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // ???????
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
        ElMessage.error('?????????: ' + (error.message || '??????'))
      }
    }

    // ???
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `???????????????{row.AccountName}?????`,
          '??????',
          {
            confirmButtonText: '???',
            cancelButtonText: '????',
            type: 'warning'
          }
        )
        await procurementApi.deleteBank(row.BankAccountId)
        ElMessage.success('???????')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('??????: ' + (error.message || '??????'))
        }
      }
    }

    // ???
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          const { BankAccountId, ...updateData } = formData
          await procurementApi.updateBank(BankAccountId, updateData)
          ElMessage.success('??????')
        } else {
          await procurementApi.createBank(formData)
          ElMessage.success('????????')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          ElMessage.error((isEdit.value ? '??' : '????') + '???: ' + (error.message || '??????'))
        }
      }
    }

    // ?????????
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // ??????
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // ??????
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
      getStatusName,
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


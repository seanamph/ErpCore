<template>
  <div class="procurement-bank-account">
    <div class="page-header">
      <h1>銀行帳戶維護</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="帳戶編號">
          <el-input v-model="queryForm.BankAccountId" placeholder="請輸入帳戶編號" clearable />
        </el-form-item>
        <el-form-item label="銀行代號">
          <el-input v-model="queryForm.BankId" placeholder="請輸入銀行代號" clearable />
        </el-form-item>
        <el-form-item label="帳戶名稱">
          <el-input v-model="queryForm.AccountName" placeholder="請輸入帳戶名稱" clearable />
        </el-form-item>
        <el-form-item label="帳戶號碼">
          <el-input v-model="queryForm.AccountNumber" placeholder="請輸入帳戶號碼" clearable />
        </el-form-item>
        <el-form-item label="帳戶類型">
          <el-select v-model="queryForm.AccountType" placeholder="請選擇帳戶類型" clearable>
            <el-option label="活期" value="1" />
            <el-option label="定期" value="2" />
            <el-option label="外幣" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="幣別">
          <el-select v-model="queryForm.CurrencyId" placeholder="請選擇幣別" clearable>
            <el-option label="新台幣" value="TWD" />
            <el-option label="美元" value="USD" />
            <el-option label="人民幣" value="CNY" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="BankAccountId" label="帳戶編號" width="120" />
        <el-table-column prop="BankName" label="銀行名稱" width="150" />
        <el-table-column prop="AccountName" label="帳戶名稱" width="150" />
        <el-table-column prop="AccountNumber" label="帳戶號碼" width="150" />
        <el-table-column prop="AccountTypeName" label="帳戶類型" width="100" />
        <el-table-column prop="CurrencyId" label="幣別" width="80" />
        <el-table-column prop="Balance" label="餘額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Balance, row.CurrencyId) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ContactPerson" label="聯絡人" width="100" />
        <el-table-column prop="ContactPhone" label="聯絡電話" width="120" />
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
            <el-button type="info" size="small" @click="handleViewBalance(row)">查詢餘額</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
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

    <!-- 新增/修改對話框 -->
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
            <el-form-item label="帳戶編號" prop="BankAccountId">
              <el-input v-model="formData.BankAccountId" :disabled="isEdit" placeholder="請輸入帳戶編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="銀行代號" prop="BankId">
              <el-input v-model="formData.BankId" placeholder="請輸入銀行代號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="帳戶名稱" prop="AccountName">
              <el-input v-model="formData.AccountName" placeholder="請輸入帳戶名稱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="帳戶號碼" prop="AccountNumber">
              <el-input v-model="formData.AccountNumber" placeholder="請輸入帳戶號碼" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="帳戶類型" prop="AccountType">
              <el-select v-model="formData.AccountType" placeholder="請選擇帳戶類型" style="width: 100%">
                <el-option label="活期" value="1" />
                <el-option label="定期" value="2" />
                <el-option label="外幣" value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="幣別" prop="CurrencyId">
              <el-select v-model="formData.CurrencyId" placeholder="請選擇幣別" style="width: 100%">
                <el-option label="新台幣" value="TWD" />
                <el-option label="美元" value="USD" />
                <el-option label="人民幣" value="CNY" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="開戶日期" prop="OpeningDate">
              <el-date-picker
                v-model="formData.OpeningDate"
                type="date"
                placeholder="請選擇日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結清日期" prop="ClosingDate">
              <el-date-picker
                v-model="formData.ClosingDate"
                type="date"
                placeholder="請選擇日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="分行名稱" prop="BranchName">
              <el-input v-model="formData.BranchName" placeholder="請輸入分行名稱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="分行代號" prop="BranchCode">
              <el-input v-model="formData.BranchCode" placeholder="請輸入分行代號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="SWIFT代號" prop="SwiftCode">
              <el-input v-model="formData.SwiftCode" placeholder="請輸入SWIFT代號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="啟用" value="1" />
                <el-option label="停用" value="0" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="聯絡人" prop="ContactPerson">
              <el-input v-model="formData.ContactPerson" placeholder="請輸入聯絡人" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="聯絡電話" prop="ContactPhone">
              <el-input v-model="formData.ContactPhone" placeholder="請輸入聯絡電話" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="聯絡信箱" prop="ContactEmail">
              <el-input v-model="formData.ContactEmail" placeholder="請輸入聯絡信箱" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Notes">
              <el-input v-model="formData.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { procurementApi } from '@/api/procurement'

export default {
  name: 'ProcurementBankAccount',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      BankAccountId: '',
      BankId: '',
      AccountName: '',
      AccountNumber: '',
      AccountType: '',
      CurrencyId: '',
      Status: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資料
    const formData = reactive({
      BankAccountId: '',
      BankId: '',
      AccountName: '',
      AccountNumber: '',
      AccountType: '',
      CurrencyId: 'TWD',
      Status: '1',
      Balance: 0,
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

    // 表單驗證規則
    const formRules = {
      BankAccountId: [{ required: true, message: '請輸入帳戶編號', trigger: 'blur' }],
      BankId: [{ required: true, message: '請輸入銀行代號', trigger: 'blur' }],
      AccountName: [{ required: true, message: '請輸入帳戶名稱', trigger: 'blur' }],
      AccountNumber: [{ required: true, message: '請輸入帳戶號碼', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改銀行帳戶' : '新增銀行帳戶'
    })

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          BankAccountId: queryForm.BankAccountId,
          BankId: queryForm.BankId,
          AccountName: queryForm.AccountName,
          AccountNumber: queryForm.AccountNumber,
          AccountType: queryForm.AccountType,
          CurrencyId: queryForm.CurrencyId,
          Status: queryForm.Status,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await procurementApi.getBankAccounts(params)
        if (response.Data) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 查詢
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm, {
        BankAccountId: '',
        BankId: '',
        AccountName: '',
        AccountNumber: '',
        AccountType: '',
        CurrencyId: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
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
        Balance: 0,
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

    // 查看
    const handleView = async (row) => {
      try {
        const response = await procurementApi.getBankAccount(row.BankAccountId)
        if (response.Data) {
          Object.assign(formData, {
            BankAccountId: response.Data.BankAccountId,
            BankId: response.Data.BankId,
            AccountName: response.Data.AccountName,
            AccountNumber: response.Data.AccountNumber,
            AccountType: response.Data.AccountType,
            CurrencyId: response.Data.CurrencyId || 'TWD',
            Status: response.Data.Status,
            Balance: response.Data.Balance || 0,
            OpeningDate: response.Data.OpeningDate,
            ClosingDate: response.Data.ClosingDate,
            ContactPerson: response.Data.ContactPerson || '',
            ContactPhone: response.Data.ContactPhone || '',
            ContactEmail: response.Data.ContactEmail || '',
            BranchName: response.Data.BranchName || '',
            BranchCode: response.Data.BranchCode || '',
            SwiftCode: response.Data.SwiftCode || '',
            Notes: response.Data.Notes || ''
          })
          isEdit.value = true
          dialogVisible.value = true
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除銀行帳戶「${row.AccountName}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await procurementApi.deleteBankAccount(row.BankAccountId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 查詢餘額
    const handleViewBalance = async (row) => {
      try {
        const response = await procurementApi.getBankAccountBalance(row.BankAccountId)
        if (response.Data) {
          ElMessageBox.alert(
            `帳戶名稱: ${response.Data.AccountName}\n餘額: ${formatCurrency(response.Data.Balance, response.Data.CurrencyId)}\n最後更新: ${new Date(response.Data.LastUpdateDate).toLocaleString()}`,
            '帳戶餘額',
            {
              confirmButtonText: '確定'
            }
          )
        }
      } catch (error) {
        ElMessage.error('查詢餘額失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          const updateData = { ...formData }
          delete updateData.BankAccountId
          await procurementApi.updateBankAccount(formData.BankAccountId, updateData)
          ElMessage.success('修改成功')
        } else {
          await procurementApi.createBankAccount(formData)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 格式化貨幣
    const formatCurrency = (amount, currency = 'TWD') => {
      if (amount == null) return '0.00'
      const formatted = Number(amount).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
      return `${formatted} ${currency}`
    }

    // 獲取狀態類型
    const getStatusType = (status) => {
      return status === '1' || status === 'A' ? 'success' : 'danger'
    }

    // 獲取狀態文字
    const getStatusText = (status) => {
      return status === '1' || status === 'A' ? '啟用' : '停用'
    }

    // 初始化
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
      formatCurrency,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleViewBalance,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.procurement-bank-account {
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
